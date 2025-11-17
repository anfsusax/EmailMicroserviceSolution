using Email.Domain.Entities;
using Email.Domain.ValueObjects;

namespace Email.Application.Emails;

public sealed record RecipientInput(string Name, string Address);

public sealed record AttachmentInput(
    string FileName,
    string ContentType,
    string? Base64Content,
    string? ExternalUrl);

public sealed record SendEmailCommand(
    string Subject,
    string Body,
    bool IsHtml,
    IReadOnlyCollection<RecipientInput> To,
    IReadOnlyCollection<RecipientInput>? Cc,
    IReadOnlyCollection<RecipientInput>? Bcc,
    IReadOnlyCollection<AttachmentInput>? Attachments,
    DateTimeOffset? ScheduleAt,
    string? TemplateId,
    IDictionary<string, string>? TemplateData,
    IDictionary<string, string>? Metadata);

public static class SendEmailCommandExtensions
{
    public static EmailMessage ToEmail(this SendEmailCommand command, EmailContent? overrideContent = null)
    {
        var content = overrideContent ?? EmailContent.Create(command.Subject, command.Body, command.IsHtml);
        var toRecipients = command.To.Select(r => Recipient.Create(r.Name, r.Address));
        var ccRecipients = command.Cc?.Select(r => Recipient.Create(r.Name, r.Address));
        var bccRecipients = command.Bcc?.Select(r => Recipient.Create(r.Name, r.Address));
        var attachments = command.Attachments?.Select(a =>
        {
            if (!string.IsNullOrWhiteSpace(a.Base64Content))
            {
                return Attachment.FromContent(
                    a.FileName,
                    a.ContentType,
                    Convert.FromBase64String(a.Base64Content));
            }

            if (!string.IsNullOrWhiteSpace(a.ExternalUrl))
            {
                return Attachment.FromLink(a.FileName, a.ContentType, a.ExternalUrl);
            }

            throw new ArgumentException($"O anexo {a.FileName} precisa definir base64Content ou externalUrl.");
        });

        return EmailMessage.Create(
            content,
            toRecipients,
            attachments,
            ccRecipients,
            bccRecipients,
            command.Metadata,
            command.ScheduleAt);
    }
}

