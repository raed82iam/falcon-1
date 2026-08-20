using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Foundation.Contracts.ResourceGovernance;

namespace Foundation.State.ResourceGovernance;

public sealed record ResourceIntegrationCoherenceSet
{
    private readonly ReadOnlyCollection<ResourceIntegrationCoherenceBinding> _bindings;

    public ResourceIntegrationCoherenceSet(ResourceEpochId epochId, DateTimeOffset asOf, IEnumerable<ResourceIntegrationCoherenceBinding> bindings)
    {
        EpochId = epochId ?? throw new ArgumentNullException(nameof(epochId));
        ArgumentNullException.ThrowIfNull(bindings);

        var ordered = bindings
            .Select(item => item ?? throw new ArgumentException("Integration binding cannot be null.", nameof(bindings)))
            .OrderBy(item => string.Join("|", item.ApplicationId.Value, item.ResourceClassId.Value), StringComparer.Ordinal)
            .ToArray();

        if (ordered.Any(item => !StringComparer.Ordinal.Equals(item.CurrentAllocation.EpochId.Value, EpochId.Value)))
            throw new InvalidOperationException("Integrated coherence-set epoch mismatch.");
        if (ordered.Any(item => item.AsOf > asOf))
            throw new InvalidOperationException("Integrated coherence-set as-of time predates a contained binding.");

        var keys = ordered.Select(item => string.Join("|", item.ApplicationId.Value, item.ResourceClassId.Value)).ToArray();
        if (keys.Distinct(StringComparer.Ordinal).Count() != keys.Length)
            throw new InvalidOperationException("Duplicate Application/resource integration binding.");

        _bindings = Array.AsReadOnly(ordered);
        AsOf = asOf;
        IdentitySha256 = CanonicalResourceIdentity.ComputeSha256(new[]
        {
            new CanonicalIdentityField("epoch", EpochId.Value),
            new CanonicalIdentityField("asOf", AsOf.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)),
            new CanonicalIdentityField("bindings", string.Join("|", _bindings.Select(item => item.IdentitySha256)))
        });
    }

    public ResourceEpochId EpochId { get; }
    public DateTimeOffset AsOf { get; }
    public IReadOnlyList<ResourceIntegrationCoherenceBinding> Bindings => _bindings;
    public string IdentitySha256 { get; }

    public IReadOnlyList<ResourceIntegrationCoherenceBinding> GetApplicationView(ApplicationPrincipalId applicationId)
    {
        ArgumentNullException.ThrowIfNull(applicationId);
        return Array.AsReadOnly(_bindings
            .Where(item => StringComparer.Ordinal.Equals(item.ApplicationId.Value, applicationId.Value))
            .ToArray());
    }
}
