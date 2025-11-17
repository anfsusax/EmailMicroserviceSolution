using Email.Domain.Enums;
using Email.Domain.ValueObjects;

namespace Email.Domain.Entities;

public sealed class EmailMessage
{
    private readonly List<Recipient> _recipients = new();
    private readonly List<Recipient> _ccRecipients = new();
    private readonly List<Recipient> _bccRecipients = new();
    private readonly List<Attachment> _attachments = new();
    private readonly Dictionary<string, string> _metadata = new(StringComparer.OrdinalIgnoreCase);

    private EmailMessage(
        Guid id,
        EmailContent content,
        IEnumerable<Recipient> recipients,
        IEnumerable<Recipient> ccRecipients,
        IEnumerable<Recipient> bccRecipients,
        IEnumerable<Attachment> attachments,
        IDictionary<string, string> metadata,
        DateTimeOffset createdAt,
        DateTimeOffset? scheduledFor,
        EmailStatus status,
        int retryCount,
        string? failureReason)
    {
        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        Content = content;
        _recipients.AddRange(recipients);
        _ccRecipients.AddRange(ccRecipients);
        _bccRecipients.AddRange(bccRecipients);
        _attachments.AddRange(attachments);
        foreach (var pair in metadata)
        {
            _metadata[pair.Key] = pair.Value;
        }
        CreatedAt = createdAt;
        ScheduledFor = scheduledFor;
        Status = status;
        RetryCount = retryCount;
        FailureReason = failureReason;
    }

    public Guid Id { get; }
    public EmailContent Content { get; private set; }
    public IReadOnlyCollection<Recipient> Recipients => _recipients.AsReadOnly();
    public IReadOnlyCollection<Recipient> CcRecipients => _ccRecipients.AsReadOnly();
    public IReadOnlyCollection<Recipient> BccRecipients => _bccRecipients.AsReadOnly();
    public IReadOnlyCollection<Attachment> Attachments => _attachments.AsReadOnly();
    public IReadOnlyDictionary<string, string> Metadata => _metadata;
    public EmailStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; }
    public DateTimeOffset? ScheduledFor { get; }
    public int RetryCount { get; private set; }
    public string? FailureReason { get; private set; }

    public static EmailMessage Create(
        EmailContent content,
        IEnumerable<Recipient> recipients,
        IEnumerable<Attachment>? attachments = null,
        IEnumerable<Recipient>? ccRecipients = null,
        IEnumerable<Recipient>? bccRecipients = null,
        IDictionary<string, string>? metadata = null,
        DateTimeOffset? scheduledFor = null)
    {
        var recipientsList = recipients?.ToList() ?? [];
        if (recipientsList.Count == 0)
        {
            throw new InvalidOperationException("Ao menos um destinatário deve ser informado.");
        }

        return new EmailMessage(
            Guid.NewGuid(),
            content,
            recipientsList,
            ccRecipients?.ToList() ?? [],
            bccRecipients?.ToList() ?? [],
            attachments?.ToList() ?? [],
            metadata is null ? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) : new Dictionary<string, string>(metadata, StringComparer.OrdinalIgnoreCase),
            DateTimeOffset.UtcNow,
            scheduledFor,
            EmailStatus.Pending,
            0,
            null);
    }

    public static EmailMessage Restore(
        Guid id,
        EmailContent content,
        IEnumerable<Recipient> recipients,
        IEnumerable<Recipient> ccRecipients,
        IEnumerable<Recipient> bccRecipients,
        IEnumerable<Attachment> attachments,
        IDictionary<string, string> metadata,
        EmailStatus status,
        int retryCount,
        DateTimeOffset createdAt,
        DateTimeOffset? scheduledFor,
        string? failureReason)
        => new(
            id,
            content,
            recipients,
            ccRecipients,
            bccRecipients,
            attachments,
            metadata,
            createdAt,
            scheduledFor,
            status,
            retryCount,
            failureReason);

    public void MarkQueued() => Status = EmailStatus.Queued;

    public void MarkProcessing() => Status = EmailStatus.Processing;

    public void MarkSent()
    {
        Status = EmailStatus.Sent;
        FailureReason = null;
    }

    public void MarkFailed(string reason, bool canRetry)
    {
        Status = canRetry ? EmailStatus.Failed : EmailStatus.DeadLettered;
        FailureReason = reason;
        if (canRetry)
        {
            RetryCount++;
        }
    }

    public bool ShouldProcess(DateTimeOffset utcNow) =>
        !ScheduledFor.HasValue || ScheduledFor <= utcNow;
}

