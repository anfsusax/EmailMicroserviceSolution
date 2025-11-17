using Email.Application.Abstractions;
using Email.Application.Configuration;
using Email.Domain.Entities;
using Email.Domain.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Email.Application.Processing;

public sealed class EmailProcessingService
{
    private readonly IEmailQueueConsumer _consumer;
    private readonly IEmailSender _emailSender;
    private readonly IEmailStatusStore _statusStore;
    private readonly IMetricsPublisher _metrics;
    private readonly IEmailQueueProducer _queueProducer;
    private readonly EmailProcessingOptions _options;
    private readonly IDateTimeProvider _clock;
    private readonly ILogger<EmailProcessingService> _logger;

    public EmailProcessingService(
        IEmailQueueConsumer consumer,
        IEmailSender emailSender,
        IEmailStatusStore statusStore,
        IEmailQueueProducer queueProducer,
        IMetricsPublisher metrics,
        IOptions<EmailProcessingOptions> options,
        IDateTimeProvider clock,
        ILogger<EmailProcessingService> logger)
    {
        _consumer = consumer;
        _emailSender = emailSender;
        _statusStore = statusStore;
        _queueProducer = queueProducer;
        _metrics = metrics;
        _options = options.Value;
        _clock = clock;
        _logger = logger;
    }

    public async Task ProcessAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                EmailMessage? message = await _consumer.PullAsync(stoppingToken);
                if (message is null)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                    continue;
                }

                await HandleMessageAsync(message, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado no processamento da fila.");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }

    private async Task HandleMessageAsync(EmailMessage message, CancellationToken token)
    {
        if (!message.ShouldProcess(_clock.UtcNow))
        {
            var delay = message.ScheduledFor!.Value - _clock.UtcNow;
            var safeDelay = delay < TimeSpan.Zero ? TimeSpan.Zero : delay;
            var cappedDelay = safeDelay > TimeSpan.FromMinutes(5) ? TimeSpan.FromMinutes(5) : safeDelay;
            if (cappedDelay > TimeSpan.Zero)
            {
                await Task.Delay(cappedDelay, token);
            }

            message.MarkQueued();
            await _queueProducer.PublishAsync(message, token);
            await _statusStore.UpdateStatusAsync(message.Id, EmailStatus.Queued, null, token);
            _logger.LogInformation("E-mail {EmailId} reagendado para {Schedule}.", message.Id, message.ScheduledFor);
            return;
        }

        var start = _clock.UtcNow;
        await _statusStore.UpdateStatusAsync(message.Id, EmailStatus.Processing, null, token);

        EmailSendResult result = await _emailSender.SendAsync(message, token);
        if (result.Success)
        {
            await _statusStore.UpdateStatusAsync(message.Id, EmailStatus.Sent, null, token);
            _metrics.TrackProcessed(_clock.UtcNow - start, true);
            _logger.LogInformation("E-mail {EmailId} enviado com sucesso.", message.Id);
            return;
        }

        bool retry = message.RetryCount < _options.MaxRetryAttempts;
        message.MarkFailed(result.Error ?? "Erro desconhecido", retry);
        await _statusStore.UpdateStatusAsync(message.Id, message.Status, result.Error, token);
        _metrics.TrackProcessed(_clock.UtcNow - start, false);
        _metrics.TrackFailure(result.Error ?? "Erro desconhecido");

        if (retry)
        {
            message.MarkQueued();
            await _queueProducer.PublishAsync(message, token);
            await _statusStore.UpdateStatusAsync(message.Id, EmailStatus.Queued, null, token);
        }
    }
}

