namespace Email.Domain.Enums;

public enum EmailStatus
{
    Pending = 0,
    Queued = 1,
    Processing = 2,
    Sent = 3,
    Failed = 4,
    DeadLettered = 5
}

