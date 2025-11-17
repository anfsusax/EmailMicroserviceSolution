using Email.Application.Abstractions;
using Email.Domain.Entities;
using Email.Domain.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Email.Application.Emails;

public interface IEmailCommandHandler
{
    Task<EmailResponseDto> HandleAsync(SendEmailCommand command, CancellationToken cancellationToken);
}

internal sealed class SendEmailCommandHandler : IEmailCommandHandler
{
    private readonly IValidator<SendEmailCommand> _validator;
    private readonly IEmailQueueProducer _queueProducer;
    private readonly IEmailStatusStore _statusStore;
    private readonly IMetricsPublisher _metrics;
    private readonly ITemplateRenderer _templateRenderer;
    private readonly ILogger<SendEmailCommandHandler> _logger;

    public SendEmailCommandHandler(
        IValidator<SendEmailCommand> validator,
        IEmailQueueProducer queueProducer,
        IEmailStatusStore statusStore,
        IMetricsPublisher metrics,
        ITemplateRenderer templateRenderer,
        ILogger<SendEmailCommandHandler> logger)
    {
        _validator = validator;
        _queueProducer = queueProducer;
        _statusStore = statusStore;
        _metrics = metrics;
        _templateRenderer = templateRenderer;
        _logger = logger;
    }

    public async Task<EmailResponseDto> HandleAsync(SendEmailCommand command, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        EmailContent? content = null;
        if (!string.IsNullOrWhiteSpace(command.TemplateId))
        {
            var rendered = await _templateRenderer.RenderAsync(
                command.TemplateId,
                command.TemplateData ?? new Dictionary<string, string>(),
                cancellationToken);
            content = EmailContent.Create(command.Subject, rendered, true);
        }

        EmailMessage email = command.ToEmail(content);
        await _statusStore.AddAsync(email, cancellationToken);
        await _queueProducer.PublishAsync(email, cancellationToken);
        _metrics.TrackQueued();
        _logger.LogInformation("E-mail {EmailId} enfileirado.", email.Id);

        return new EmailResponseDto(email.Id, email.Status);
    }
}

