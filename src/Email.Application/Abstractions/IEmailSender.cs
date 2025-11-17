using Email.Domain.Entities;

namespace Email.Application.Abstractions;

public interface IEmailSender
{
    Task<EmailSendResult> SendAsync(EmailMessage message, CancellationToken cancellationToken);
}

public sealed record EmailSendResult(bool Success, string? Error);

