using System.Collections.Concurrent;
using Email.Application.Abstractions;
using Email.Domain.Entities;
using Email.Domain.Enums;

namespace Email.Infrastructure.Persistence;

internal sealed class InMemoryEmailStatusStore : IEmailStatusStore
{
    private readonly ConcurrentDictionary<Guid, EmailMessage> _messages = new();

    public Task AddAsync(EmailMessage message, CancellationToken cancellationToken)
    {
        message.MarkQueued();
        _messages[message.Id] = message;
        return Task.CompletedTask;
    }

    public Task UpdateStatusAsync(Guid emailId, EmailStatus status, string? reason, CancellationToken cancellationToken)
    {
        if (_messages.TryGetValue(emailId, out var message))
        {
            switch (status)
            {
                case EmailStatus.Queued:
                    message.MarkQueued();
                    break;
                case EmailStatus.Processing:
                    message.MarkProcessing();
                    break;
                case EmailStatus.Sent:
                    message.MarkSent();
                    break;
                default:
                    message.MarkFailed(reason ?? "Erro desconhecido", status != EmailStatus.DeadLettered);
                    break;
            }
        }

        return Task.CompletedTask;
    }

    public Task<EmailMessage?> GetAsync(Guid emailId, CancellationToken cancellationToken)
    {
        _messages.TryGetValue(emailId, out var message);
        return Task.FromResult(message);
    }
}

