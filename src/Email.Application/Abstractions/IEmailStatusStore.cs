using Email.Domain.Entities;
using Email.Domain.Enums;

namespace Email.Application.Abstractions;

public interface IEmailStatusStore
{
    Task AddAsync(EmailMessage message, CancellationToken cancellationToken);
    Task UpdateStatusAsync(Guid emailId, EmailStatus status, string? reason, CancellationToken cancellationToken);
    Task<EmailMessage?> GetAsync(Guid emailId, CancellationToken cancellationToken);
}

