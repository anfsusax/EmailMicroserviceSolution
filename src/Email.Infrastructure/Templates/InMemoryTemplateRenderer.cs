using Email.Application.Abstractions;
using Email.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Scriban;

namespace Email.Infrastructure.Templates;

internal sealed class InMemoryTemplateRenderer : ITemplateRenderer
{
    private readonly IOptionsMonitor<EmailTemplateOptions> _options;
    private readonly ILogger<InMemoryTemplateRenderer> _logger;

    public InMemoryTemplateRenderer(IOptionsMonitor<EmailTemplateOptions> options, ILogger<InMemoryTemplateRenderer> logger)
    {
        _options = options;
        _logger = logger;
    }

    public async Task<string> RenderAsync(string templateId, IDictionary<string, string> data, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(templateId))
        {
            throw new ArgumentException("TemplateId inválido.", nameof(templateId));
        }

        if (!_options.CurrentValue.Templates.TryGetValue(templateId, out var templateContent))
        {
            throw new KeyNotFoundException($"Template '{templateId}' não encontrado.");
        }

        var template = Template.Parse(templateContent, templateId);
        if (template.HasErrors)
        {
            throw new InvalidOperationException($"Template '{templateId}' possui erros: {string.Join(",", template.Messages.Select(m => m.Message))}");
        }

        var scriptObject = new Scriban.Runtime.ScriptObject();
        foreach (var kv in data)
        {
            scriptObject[kv.Key] = kv.Value;
        }

        var context = new TemplateContext();
        context.PushGlobal(scriptObject);

        string result = await template.RenderAsync(context);
        _logger.LogDebug("Template {TemplateId} renderizado.", templateId);
        return result;
    }
}

