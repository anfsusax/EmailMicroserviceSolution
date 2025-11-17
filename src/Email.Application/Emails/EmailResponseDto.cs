using Email.Domain.Enums;

namespace Email.Application.Emails;

public sealed record EmailResponseDto(Guid Id, EmailStatus Status);

