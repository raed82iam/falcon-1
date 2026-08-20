using Falcon.FSATS.ResourceManagement.Application;

namespace Falcon.FSATS.ResourceManagement.Infrastructure;

public sealed class DisabledFoundationResourcePort : IFoundationResourceBindingPort
{
    public ValueTask<FoundationResourceStateProjection?> ReadApplicationStateAsync(
        string applicationId,
        string resourceClass,
        string expectedEpochId,
        CancellationToken cancellationToken)
        => ValueTask.FromResult<FoundationResourceStateProjection?>(null);

    public ValueTask<FoundationLoadSheddingSignal?> ReadLoadSheddingSignalAsync(
        string applicationId,
        string resourceClass,
        string expectedEpochId,
        CancellationToken cancellationToken)
        => ValueTask.FromResult<FoundationLoadSheddingSignal?>(null);

    public ValueTask<FoundationAdditionalResourceOutcome> RequestAdditionalAsync(
        FoundationAdditionalResourceRequest request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        var now = DateTimeOffset.UtcNow;
        return ValueTask.FromResult(new FoundationAdditionalResourceOutcome(
            false,
            FoundationResourceDecisionKind.Deny,
            0m,
            request.Unit,
            "FOUNDATION_RESOURCE_BINDING_NOT_MATERIALIZED",
            request.RequestId,
            request.EpochId,
            "FOUNDATION_RESOURCE_BINDING_NOT_MATERIALIZED",
            now,
            now));
    }
}
