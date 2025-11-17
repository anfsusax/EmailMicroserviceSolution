using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Email.Application.Abstractions;
using Email.Domain.Entities;
using Email.Domain.Enums;
using Email.Domain.ValueObjects;
using Email.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Email.Infrastructure.Messaging;

internal sealed class RabbitMqEmailQueue : IEmailQueueProducer, IEmailQueueConsumer, IDisposable
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
    private readonly RabbitMqOptions _options;
    private readonly ILogger<RabbitMqEmailQueue> _logger;
    private readonly ConnectionFactory _factory;
    private IConnection? _connection;
    private IChannel? _channel;

    public RabbitMqEmailQueue(IOptions<RabbitMqOptions> options, ILogger<RabbitMqEmailQueue> logger)
    {
        _options = options.Value;
        _logger = logger;
        _factory = new ConnectionFactory
        {
            HostName = _options.HostName,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password,
            VirtualHost = _options.VirtualHost
        };
    }

    public async Task PublishAsync(EmailMessage message, CancellationToken cancellationToken)
    {
        var channel = await EnsureChannelAsync(cancellationToken);
        await DeclareQueueAsync(channel, cancellationToken);

        var payload = JsonSerializer.Serialize(EmailMessageContract.FromDomain(message), SerializerOptions);
        var body = Encoding.UTF8.GetBytes(payload);
        var props = new BasicProperties
        {
            Persistent = true,
            ContentType = "application/json"
        };

        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: _options.QueueName,
            mandatory: false,
            basicProperties: props,
            body: body,
            cancellationToken: cancellationToken);

        _logger.LogInformation("Mensagem {EmailId} publicada na fila {Queue}.", message.Id, _options.QueueName);
    }

    public async Task<EmailMessage?> PullAsync(CancellationToken cancellationToken)
    {
        var channel = await EnsureChannelAsync(cancellationToken);
        await DeclareQueueAsync(channel, cancellationToken);

        var result = await channel.BasicGetAsync(_options.QueueName, autoAck: true, cancellationToken);
        if (result is null)
        {
            return null;
        }

        var payload = Encoding.UTF8.GetString(result.Body.ToArray());
        var contract = JsonSerializer.Deserialize<EmailMessageContract>(payload, SerializerOptions);
        return contract?.ToDomain();
    }

    private async Task<IChannel> EnsureChannelAsync(CancellationToken cancellationToken)
    {
        if (_connection is { IsOpen: true } && _channel is { IsOpen: true })
        {
            return _channel;
        }

        _channel?.Dispose();
        _connection?.Dispose();

        _connection = await _factory.CreateConnectionAsync(cancellationToken);
        _channel = await _connection.CreateChannelAsync(
            new CreateChannelOptions(
                publisherConfirmationsEnabled: false,
                publisherConfirmationTrackingEnabled: false,
                outstandingPublisherConfirmationsRateLimiter: null,
                consumerDispatchConcurrency: null),
            cancellationToken);
        return _channel;
    }

    private Task DeclareQueueAsync(IChannel channel, CancellationToken cancellationToken)
    {
        return channel.QueueDeclareAsync(
            queue: _options.QueueName,
            durable: _options.Durable,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            passive: false,
            noWait: false,
            cancellationToken: cancellationToken);
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }

    private sealed record RecipientContract(string Name, string Address)
    {
        public Recipient ToDomain() => Recipient.Create(Name, Address);
    }

    private sealed record AttachmentContract(string FileName, string ContentType, string? Base64Content, string? ExternalUrl)
    {
        public Attachment ToDomain()
        {
            if (!string.IsNullOrWhiteSpace(Base64Content))
            {
                return Attachment.FromContent(FileName, ContentType, Convert.FromBase64String(Base64Content));
            }

            if (!string.IsNullOrWhiteSpace(ExternalUrl))
            {
                return Attachment.FromLink(FileName, ContentType, ExternalUrl);
            }

            throw new InvalidOperationException($"Anexo {FileName} inválido.");
        }
    }

    private sealed record EmailMessageContract(
        Guid Id,
        string Subject,
        string Body,
        bool IsHtml,
        RecipientContract[] Recipients,
        RecipientContract[] CcRecipients,
        RecipientContract[] BccRecipients,
        AttachmentContract[] Attachments,
        Dictionary<string, string> Metadata,
        EmailStatus Status,
        int RetryCount,
        DateTimeOffset CreatedAt,
        DateTimeOffset? ScheduledFor,
        string? FailureReason)
    {
        public static EmailMessageContract FromDomain(EmailMessage message) =>
            new(
                message.Id,
                message.Content.Subject,
                message.Content.Body,
                message.Content.IsHtml,
                message.Recipients.Select(r => new RecipientContract(r.Name, r.Address.Value)).ToArray(),
                message.CcRecipients.Select(r => new RecipientContract(r.Name, r.Address.Value)).ToArray(),
                message.BccRecipients.Select(r => new RecipientContract(r.Name, r.Address.Value)).ToArray(),
                message.Attachments.Select(a => new AttachmentContract(
                    a.FileName,
                    a.ContentType,
                    a.Content is null ? null : Convert.ToBase64String(a.Content),
                    a.ExternalUrl?.ToString())).ToArray(),
                new Dictionary<string, string>(message.Metadata, StringComparer.OrdinalIgnoreCase),
                message.Status,
                message.RetryCount,
                message.CreatedAt,
                message.ScheduledFor,
                message.FailureReason);

        public EmailMessage ToDomain()
        {
            var content = EmailContent.Create(Subject, Body, IsHtml);
            var recipients = Recipients.Select(r => r.ToDomain()).ToArray();
            var ccRecipients = CcRecipients.Select(r => r.ToDomain()).ToArray();
            var bccRecipients = BccRecipients.Select(r => r.ToDomain()).ToArray();
            var attachments = Attachments.Select(a => a.ToDomain()).ToArray();

            return EmailMessage.Restore(
                Id,
                content,
                recipients,
                ccRecipients,
                bccRecipients,
                attachments,
                Metadata,
                Status,
                RetryCount,
                CreatedAt,
                ScheduledFor,
                FailureReason);
        }
    }
}

