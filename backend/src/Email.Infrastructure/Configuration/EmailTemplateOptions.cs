namespace Email.Infrastructure.Configuration;

public sealed class EmailTemplateOptions
{
    public const string SectionName = "EmailTemplates";

    public Dictionary<string, string> Templates { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

