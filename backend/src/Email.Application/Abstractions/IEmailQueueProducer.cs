using Email.Domain.Entities;

namespace Email.Application.Abstractions;

public interface IEmailQueueProducer
{
    Task PublishAsync(EmailMessage message, CancellationToken cancellationToken);
}

