using System.Diagnostics.Metrics;
using Email.Application.Abstractions;

namespace Email.Infrastructure.Observability;

internal sealed class EmailMetricsPublisher : IMetricsPublisher
{
    private static readonly Meter Meter = new("EmailMicroservice.Email");
    private static readonly Counter<long> Queued = Meter.CreateCounter<long>("email_queued_total");
    private static readonly Histogram<double> ProcessingTime = Meter.CreateHistogram<double>("email_processing_seconds");
    private static readonly Counter<long> Failures = Meter.CreateCounter<long>("email_failures_total");

    public void TrackQueued() => Queued.Add(1);

    public void TrackProcessed(TimeSpan duration, bool success) =>
        ProcessingTime.Record(duration.TotalSeconds, KeyValuePair.Create<string, object?>("success", success));

    public void TrackFailure(string reason) => Failures.Add(1, KeyValuePair.Create<string, object?>("reason", reason));
}

