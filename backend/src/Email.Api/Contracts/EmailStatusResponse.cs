using Email.Domain.Enums;

namespace Email.Api.Contracts;

public sealed record EmailStatusResponse(Guid Id, EmailStatus Status, string? FailureReason);

