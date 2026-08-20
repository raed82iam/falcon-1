using System;

namespace Falcon.Stage6.CrossStageIntegration.Verifier;

internal sealed class FixedTimeProvider : TimeProvider
{
    private readonly DateTimeOffset _utcNow;
    private readonly long _timestamp;

    public FixedTimeProvider(DateTimeOffset utcNow)
    {
        _utcNow = utcNow;
        _timestamp = utcNow.UtcDateTime.Ticks;
    }

    public override DateTimeOffset GetUtcNow() => _utcNow;

    public override long GetTimestamp() => _timestamp;

    public override long TimestampFrequency => TimeSpan.TicksPerSecond;
}
