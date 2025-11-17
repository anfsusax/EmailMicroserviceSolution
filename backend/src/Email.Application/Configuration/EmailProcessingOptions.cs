namespace Email.Application.Configuration;

public sealed class EmailProcessingOptions
{
    public const string SectionName = "EmailProcessing";

    public int MaxRetryAttempts { get; set; } = 3;
    public int PrefetchCount { get; set; } = 10;
    public int VisibilityTimeoutSeconds { get; set; } = 30;
}

