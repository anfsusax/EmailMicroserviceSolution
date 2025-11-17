namespace Email.Domain.ValueObjects;

public sealed class Attachment
{
    private Attachment(string fileName, string contentType, byte[]? content, Uri? externalUrl)
    {
        FileName = fileName;
        ContentType = contentType;
        Content = content;
        ExternalUrl = externalUrl;
    }

    public string FileName { get; }
    public string ContentType { get; }
    public byte[]? Content { get; }
    public Uri? ExternalUrl { get; }

    public static Attachment FromContent(string fileName, string contentType, byte[] content)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("O nome do arquivo é obrigatório.", nameof(fileName));
        }

        if (string.IsNullOrWhiteSpace(contentType))
        {
            throw new ArgumentException("O content type é obrigatório.", nameof(contentType));
        }

        if (content == null || content.Length == 0)
        {
            throw new ArgumentException("O conteúdo do arquivo é obrigatório.", nameof(content));
        }

        return new Attachment(fileName.Trim(), contentType.Trim(), content, null);
    }

    public static Attachment FromLink(string fileName, string contentType, string url)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentException("O nome do arquivo é obrigatório.", nameof(fileName));
        }

        if (string.IsNullOrWhiteSpace(contentType))
        {
            throw new ArgumentException("O content type é obrigatório.", nameof(contentType));
        }

        if (string.IsNullOrWhiteSpace(url) || !Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            throw new ArgumentException("A URL do anexo é inválida.", nameof(url));
        }

        return new Attachment(fileName.Trim(), contentType.Trim(), null, uri);
    }
}

