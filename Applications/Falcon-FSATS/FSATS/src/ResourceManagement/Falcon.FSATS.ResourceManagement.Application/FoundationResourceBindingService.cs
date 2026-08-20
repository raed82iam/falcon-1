using Falcon.FSATS.ResourceManagement.Domain;

namespace Falcon.FSATS.ResourceManagement.Application;

public interface IFoundationResourceBindingPort
{
    ValueTask<FoundationResourceStateProjection?> ReadApplicationStateAsync(
        string applicationId,
        string resourceClass,
        string expectedEpochId,
        CancellationToken cancellationToken);

    ValueTask<FoundationLoadSheddingSignal?> ReadLoadSheddingSignalAsync(
        string applicationId,
        string resourceClass,
        string expectedEpochId,
        CancellationToken cancellationToken);

    ValueTask<FoundationAdditionalResourceOutcome> RequestAdditionalAsync(
        FoundationAdditionalResourceRequest request,
        CancellationToken cancellationToken);
}

public sealed record FoundationBoundResourceResult(
    bool Bound,
    bool Granted,
    decimal GrantedAmount,
    string ReasonCode,
    string? FoundationOutcomeReference);

public sealed class FoundationResourceBindingService
{
    private readonly IFoundationResourceBindingPort _foundation;
    private readonly TimeSpan _projectionMaximumAge;

    public FoundationResourceBindingService(IFoundationResourceBindingPort foundation)
        : this(foundation, FoundationResourceBindingGuards.DefaultProjectionMaximumAge)
    {
    }

    public FoundationResourceBindingService(IFoundationResourceBindingPort foundation, TimeSpan projectionMaximumAge)
    {
        _foundation = foundation ?? throw new ArgumentNullException(nameof(foundation));
        if (projectionMaximumAge <= TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(projectionMaximumAge));
        _projectionMaximumAge = projectionMaximumAge;
    }

    public async ValueTask<FoundationResourceStateProjection?> ReadCurrentStateAsync(
        string applicationId,
        string resourceClass,
        string expectedEpochId,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        var projection = await _foundation.ReadApplicationStateAsync(applicationId, resourceClass, expectedEpochId, cancellationToken);
        return projection is not null && FoundationResourceBindingGuards.IsUsable(
                projection,
                applicationId,
                resourceClass,
                expectedEpochId,
                now,
                _projectionMaximumAge)
            ? projection
            : null;
    }

    public async ValueTask<FoundationLoadSheddingSignal?> ReadCurrentLoadSheddingAsync(
        FoundationResourceStateProjection projection,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        var signal = await _foundation.ReadLoadSheddingSignalAsync(projection.ApplicationId, projection.ResourceClass, projection.EpochId, cancellationToken);
        return signal is not null && FoundationResourceBindingGuards.IsCurrent(signal, projection, now)
            ? signal
            : null;
    }

    public async ValueTask<FoundationBoundResourceResult> RequestResidualAsync(
        string requestId,
        ResourceClaim targetClaim,
        decimal safelyReclaimableInsideFsats,
        string evidenceReference,
        string coordinatorInstanceId,
        string coordinatorRoleId,
        string foundationEpochId,
        string coordinationScopeId,
        string unit,
        string correlationId,
        string causationId,
        DateTimeOffset requestedAt,
        DateTimeOffset expiresAt,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        var residual = ResidualNeedCalculator.Calculate(targetClaim, safelyReclaimableInsideFsats);
        if (residual <= 0m)
            return new(false, false, 0m, "NO_PROVEN_RESIDUAL_NEED", null);

        if (string.IsNullOrWhiteSpace(requestId) ||
            string.IsNullOrWhiteSpace(targetClaim.ResourceClass) ||
            string.IsNullOrWhiteSpace(foundationEpochId) ||
            string.IsNullOrWhiteSpace(coordinationScopeId) ||
            string.IsNullOrWhiteSpace(evidenceReference) ||
            string.IsNullOrWhiteSpace(coordinatorInstanceId) ||
            string.IsNullOrWhiteSpace(coordinatorRoleId) ||
            string.IsNullOrWhiteSpace(unit) ||
            string.IsNullOrWhiteSpace(correlationId) ||
            string.IsNullOrWhiteSpace(causationId) ||
            correlationId == causationId ||
            requestedAt == default ||
            expiresAt <= requestedAt ||
            now < requestedAt ||
            now >= expiresAt)
        {
            return new(false, false, 0m, "INVALID_FOUNDATION_REQUEST_BINDING", null);
        }

        var request = new FoundationAdditionalResourceRequest(
            requestId.Trim(),
            "APP-RSC",
            coordinatorInstanceId.Trim(),
            coordinatorRoleId.Trim(),
            targetClaim.ResourceClass.Trim(),
            residual,
            unit.Trim(),
            foundationEpochId.Trim(),
            coordinationScopeId.Trim(),
            evidenceReference.Trim(),
            "INTERNAL_REDISTRIBUTION_FIRST_PROVEN",
            correlationId.Trim(),
            causationId.Trim(),
            requestedAt,
            expiresAt);

        FoundationAdditionalResourceOutcome outcome;
        try
        {
            outcome = await _foundation.RequestAdditionalAsync(request, cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch
        {
            return new(false, false, 0m, "FOUNDATION_REQUEST_OUTCOME_UNAVAILABLE", null);
        }

        if (outcome is null || !FoundationResourceBindingGuards.OutcomeMatches(request, outcome, now))
            return new(false, false, 0m, "FOUNDATION_OUTCOME_BINDING_REJECTED", null);

        var granted = outcome.Decision is FoundationResourceDecisionKind.Grant or FoundationResourceDecisionKind.PartialGrant;
        return new(
            true,
            granted,
            granted ? outcome.GrantedAmount : 0m,
            outcome.Decision.ToString().ToUpperInvariant(),
            outcome.FoundationOutcomeReference);
    }
}
