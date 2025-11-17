namespace Email.Domain.ValueObjects;

public sealed class EmailContent
{
    private EmailContent(string subject, string body, bool isHtml)
    {
        Subject = subject;
        Body = body;
        IsHtml = isHtml;
    }

    public string Subject { get; }
    public string Body { get; }
    public bool IsHtml { get; }

    public static EmailContent Create(string subject, string body, bool isHtml = true)
    {
        if (string.IsNullOrWhiteSpace(subject))
        {
            throw new ArgumentException("O assunto é obrigatório.", nameof(subject));
        }

        if (string.IsNullOrWhiteSpace(body))
        {
            throw new ArgumentException("O corpo do e-mail é obrigatório.", nameof(body));
        }

        return new EmailContent(subject.Trim(), body, isHtml);
    }
}

