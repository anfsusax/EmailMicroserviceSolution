namespace Email.Application.Abstractions;

public interface IMetricsPublisher
{
    void TrackQueued();
    void TrackProcessed(TimeSpan duration, bool success);
    void TrackFailure(string reason);
}

