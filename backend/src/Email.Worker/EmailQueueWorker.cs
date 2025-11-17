using Email.Application.Processing;

namespace Email.Worker;

internal sealed class EmailQueueWorker : BackgroundService
{
    private readonly EmailProcessingService _processingService;
    private readonly ILogger<EmailQueueWorker> _logger;

    public EmailQueueWorker(EmailProcessingService processingService, ILogger<EmailQueueWorker> logger)
    {
        _processingService = processingService;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Iniciando processamento de fila de e-mails.");
        await _processingService.ProcessAsync(stoppingToken);
    }
}

