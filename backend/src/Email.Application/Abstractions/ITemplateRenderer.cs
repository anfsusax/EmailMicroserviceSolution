namespace Email.Application.Abstractions;

public interface ITemplateRenderer
{
    Task<string> RenderAsync(string templateId, IDictionary<string, string> data, CancellationToken cancellationToken);
}

