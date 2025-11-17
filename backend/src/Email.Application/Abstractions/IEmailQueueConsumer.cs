using Email.Domain.Entities;

namespace Email.Application.Abstractions;

public interface IEmailQueueConsumer
{
    Task<EmailMessage?> PullAsync(CancellationToken cancellationToken);
}

