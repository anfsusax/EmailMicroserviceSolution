using Email.Application.Emails;

namespace Email.Api.Contracts;

public sealed record EmailRecipientDto(string Name, string Address);

public sealed record EmailAttachmentDto(
    string FileName,
    string ContentType,
    string? Base64Content,
    string? ExternalUrl);

public sealed record SendEmailRequest(
    string Subject,
    string Body,
    bool IsHtml,
    IReadOnlyCollection<EmailRecipientDto> To,
    IReadOnlyCollection<EmailRecipientDto>? Cc,
    IReadOnlyCollection<EmailRecipientDto>? Bcc,
    IReadOnlyCollection<EmailAttachmentDto>? Attachments,
    DateTimeOffset? ScheduleAt,
    string? TemplateId,
    IDictionary<string, string>? TemplateData,
    IDictionary<string, string>? Metadata)
{
    public SendEmailCommand ToCommand() =>
        new(
            Subject,
            Body,
            IsHtml,
            To.Select(r => new RecipientInput(r.Name, r.Address)).ToArray(),
            Cc?.Select(r => new RecipientInput(r.Name, r.Address)).ToArray(),
            Bcc?.Select(r => new RecipientInput(r.Name, r.Address)).ToArray(),
            Attachments?.Select(a => new AttachmentInput(a.FileName, a.ContentType, a.Base64Content, a.ExternalUrl)).ToArray(),
            ScheduleAt,
            TemplateId,
            TemplateData,
            Metadata);
}

