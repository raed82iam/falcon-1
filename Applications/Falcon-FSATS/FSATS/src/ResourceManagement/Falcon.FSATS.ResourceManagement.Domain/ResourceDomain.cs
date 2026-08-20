namespace Falcon.FSATS.ResourceManagement.Domain;

public readonly record struct CoordinationEpoch(long Value);
public sealed record ResourceClaim(string ApplicationId, string ResourceClass, decimal Allocation, decimal Consumption, decimal MinimumSafe, decimal Desired, decimal Reclaimable, int Urgency, bool Fresh, bool IntegrityTrusted);
public sealed record FoundationEnvelope(string Reference, string ResourceClass, decimal TotalGranted, DateTimeOffset EffectiveAt, DateTimeOffset? ExpiresAt, bool Revoked);
public sealed record RedistributionDecision(string DecisionId, string SourceApplication, string TargetApplication, string ResourceClass, decimal Amount, CoordinationEpoch Epoch, string EnvelopeReference, string ReasonCode);

public static class DemandIntegrityEvaluator
{
    public static decimal SafeReclaimableHeadroom(ResourceClaim claim)
    {
        if (claim is null || claim.Allocation < 0m || claim.Consumption < 0m || claim.MinimumSafe < 0m) return 0m;
        var protectedUse = Math.Max(claim.Consumption, claim.MinimumSafe);
        return Math.Max(0m, claim.Allocation - protectedUse);
    }

    public static bool IsEligible(ResourceClaim claim)
        => claim is not null &&
           !string.IsNullOrWhiteSpace(claim.ApplicationId) &&
           !string.IsNullOrWhiteSpace(claim.ResourceClass) &&
           claim.Fresh &&
           claim.IntegrityTrusted &&
           claim.Allocation >= 0m &&
           claim.Consumption >= 0m &&
           claim.Consumption <= claim.Allocation &&
           claim.MinimumSafe >= 0m &&
           claim.Desired >= claim.MinimumSafe &&
           claim.Reclaimable >= 0m &&
           claim.Reclaimable <= SafeReclaimableHeadroom(claim);
}

public sealed class ResourceStrategyController
{
    public RedistributionDecision? Plan(
        IReadOnlyCollection<ResourceClaim> claims,
        FoundationEnvelope envelope,
        CoordinationEpoch epoch,
        string targetApplication,
        string resourceClass,
        DateTimeOffset now)
    {
        ArgumentNullException.ThrowIfNull(claims);
        ArgumentNullException.ThrowIfNull(envelope);
        if (string.IsNullOrWhiteSpace(targetApplication) || string.IsNullOrWhiteSpace(resourceClass)) return null;
        if (string.IsNullOrWhiteSpace(envelope.Reference) || string.IsNullOrWhiteSpace(envelope.ResourceClass)) return null;
        if (envelope.Revoked || envelope.TotalGranted < 0m || envelope.ResourceClass != resourceClass) return null;
        if (envelope.EffectiveAt == default || envelope.EffectiveAt > now) return null;
        if (envelope.ExpiresAt is { } expiry && expiry <= now) return null;

        var relevant = claims.Where(x => x is not null && x.ResourceClass == resourceClass).ToArray();
        if (relevant.Length == 0 || relevant.Any(x => !DemandIntegrityEvaluator.IsEligible(x))) return null;
        if (relevant.GroupBy(x => x.ApplicationId, StringComparer.Ordinal).Any(g => g.Count() != 1)) return null;

        decimal allocated;
        try { allocated = relevant.Aggregate(0m, (sum, claim) => checked(sum + claim.Allocation)); }
        catch (OverflowException) { return null; }
        if (allocated > envelope.TotalGranted) return null;

        var target = relevant.SingleOrDefault(x => x.ApplicationId == targetApplication);
        if (target is null) return null;
        var deficit = Math.Max(0m, target.Desired - target.Allocation);
        if (deficit <= 0m) return null;

        var donors = relevant.Where(x => x.ApplicationId != targetApplication && x.Reclaimable > 0m)
            .OrderBy(x => x.Urgency)
            .ThenBy(x => x.ApplicationId, StringComparer.Ordinal);
        foreach (var donor in donors)
        {
            var provenHeadroom = DemandIntegrityEvaluator.SafeReclaimableHeadroom(donor);
            var amount = Math.Min(deficit, Math.Min(donor.Reclaimable, provenHeadroom));
            if (amount > 0m)
            {
                var decisionId = $"redistribute:{epoch.Value}:{Part(donor.ApplicationId)}:{Part(target.ApplicationId)}:{Part(resourceClass)}:{Part(envelope.Reference)}";
                return new(decisionId, donor.ApplicationId, target.ApplicationId, resourceClass, amount, epoch, envelope.Reference, "INTERNAL_REDISTRIBUTION_FIRST");
            }
        }
        return null;
    }

    private static string Part(string value) => Uri.EscapeDataString(value.Trim());
}

public static class ResourceEpochFence
{
    public static bool IsCurrent(CoordinationEpoch actionEpoch, CoordinationEpoch currentEpoch, string actionEnvelope, FoundationEnvelope currentEnvelope, DateTimeOffset now)
        => currentEnvelope is not null &&
           actionEpoch == currentEpoch &&
           !string.IsNullOrWhiteSpace(actionEnvelope) &&
           actionEnvelope == currentEnvelope.Reference &&
           !currentEnvelope.Revoked &&
           currentEnvelope.EffectiveAt != default &&
           currentEnvelope.EffectiveAt <= now &&
           (currentEnvelope.ExpiresAt is null || currentEnvelope.ExpiresAt.Value > now);
}

public static class ResidualNeedCalculator
{
    public static decimal Calculate(ResourceClaim claim, decimal safelyReclaimableInsideFsats)
    {
        if (!DemandIntegrityEvaluator.IsEligible(claim)) return 0m;
        var deficit = Math.Max(0m, claim.Desired - claim.Allocation);
        return Math.Max(0m, deficit - Math.Max(0m, safelyReclaimableInsideFsats));
    }
}

public sealed class OscillationGuard
{
    private readonly object _gate = new();
    private readonly Dictionary<string, DateTimeOffset> _lastChange = new(StringComparer.Ordinal);

    public bool Allow(string resourceClass, DateTimeOffset now, TimeSpan minimumInterval)
    {
        if (string.IsNullOrWhiteSpace(resourceClass) || now == default || minimumInterval < TimeSpan.Zero) return false;
        lock (_gate)
        {
            if (_lastChange.TryGetValue(resourceClass, out var previous) && now - previous < minimumInterval) return false;
            _lastChange[resourceClass] = now;
            return true;
        }
    }
}
