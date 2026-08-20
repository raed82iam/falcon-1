using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Foundation.ApplicationRuntimeHosting;

public enum RuntimeSlotState
{
    Registered = 1,
    Active = 2,
    Suspended = 3,
    Isolated = 4,
    Removed = 5
}

public enum RuntimeCapabilityVisibility
{
    Private = 1,
    SharedDeclared = 2
}

public enum RuntimeAuthorityAction
{
    Activate = 1,
    Suspend = 2,
    Isolate = 3,
    Remove = 4
}

public enum RuntimeAuthorityStatus
{
    Valid = 1,
    Missing = 2,
    Stale = 3,
    Revoked = 4,
    Invalid = 5,
    Ambiguous = 6
}

public enum RuntimeLifecycleEligibilityKind
{
    Attach = 1,
    DetachOrRemove = 2
}

public sealed record RuntimeArtifactConsumptionBinding(
    bool AcceptedForTechnicalConsumption,
    string ExactArtifactIdentity,
    bool ActivationAuthorized,
    bool DeploymentAuthorized,
    bool ProductionAuthorized,
    bool BusinessAuthorityGranted,
    bool SilentUpgradePerformed);

public sealed record RuntimeAdmissionBinding(
    bool Admitted,
    string ApplicationIdentity,
    string ApplicationVersion,
    string EvidenceIdentity);

public sealed record RuntimeLifecycleEligibilityBinding(
    bool Eligible,
    RuntimeLifecycleEligibilityKind Kind,
    string ApplicationIdentity,
    string CurrentVersion,
    string TargetVersion,
    string DecisionIdentity);

public sealed record RuntimeResourceGrantBinding(
    string GrantIdentity,
    string ApplicationIdentity,
    string ResourceClassIdentity,
    decimal Allocation,
    decimal Quota,
    decimal Ceiling,
    DateTimeOffset EffectiveFrom,
    DateTimeOffset? EffectiveUntil,
    DateTimeOffset EvidenceObservedAt,
    string EvidenceIdentity);

public sealed record RuntimeCapabilityDeclaration(
    string CapabilityId,
    RuntimeCapabilityVisibility Visibility,
    bool Exclusive);

public sealed record RuntimeRegistrationRequest(
    string RuntimeInstanceId,
    string ApplicationIdentity,
    string ApplicationVersion,
    string ExpectedArtifactExactIdentity,
    RuntimeArtifactConsumptionBinding ArtifactConsumption,
    RuntimeAdmissionBinding Admission,
    RuntimeLifecycleEligibilityBinding LifecycleEligibility,
    IReadOnlyList<RuntimeResourceGrantBinding> ResourceGrants,
    IReadOnlyList<RuntimeCapabilityDeclaration> ProvidedCapabilities,
    IReadOnlyList<string> RequiredCapabilities,
    DateTimeOffset ObservedAt);

public sealed record RuntimeAuthorityEvidence(
    string AuthorityIdentity,
    RuntimeAuthorityStatus Status,
    RuntimeAuthorityAction Action,
    string RuntimeInstanceId,
    string ApplicationIdentity,
    string ApplicationVersion,
    DateTimeOffset EffectiveFrom,
    DateTimeOffset EffectiveUntil,
    string EvidenceIdentity);

public sealed record RuntimeRegistrationDecision(
    bool Registered,
    string Reason,
    string RuntimeInstanceId,
    string ApplicationIdentity,
    string ApplicationVersion,
    string DecisionIdentity,
    bool ActivationAuthorized,
    bool DeploymentAuthorized,
    bool BusinessAuthorityGranted);

public sealed record RuntimeTransitionDecision(
    bool Accepted,
    string Reason,
    string RuntimeInstanceId,
    RuntimeSlotState ResultingState,
    string DecisionIdentity,
    bool DeploymentAuthorized,
    bool BusinessAuthorityGranted);

public sealed record CapabilityResolutionDecision(
    bool Available,
    string Reason,
    string CapabilityId,
    string ProviderRuntimeInstanceId,
    string ProviderApplicationIdentity,
    string DecisionIdentity);

public sealed record RuntimeSlotProjection(
    string RuntimeInstanceId,
    string ApplicationIdentity,
    string ApplicationVersion,
    RuntimeSlotState State,
    IReadOnlyList<string> ProvidedCapabilities,
    IReadOnlyList<string> RequiredCapabilities,
    string RuntimeEvidenceIdentity);

public sealed record RuntimeHostProjection(
    string HostIdentity,
    DateTimeOffset ObservedAt,
    IReadOnlyList<RuntimeSlotProjection> Slots,
    string ProjectionIdentity,
    bool ZeroApplicationStateValid,
    bool CarriesDeploymentAuthority,
    bool CarriesBusinessAuthority);

internal sealed class RuntimeSlot
{
    public RuntimeSlot(RuntimeRegistrationRequest request, string registrationDecisionIdentity)
    {
        RuntimeInstanceId = request.RuntimeInstanceId;
        ApplicationIdentity = request.ApplicationIdentity;
        ApplicationVersion = request.ApplicationVersion;
        State = RuntimeSlotState.Registered;
        ProvidedCapabilities = request.ProvidedCapabilities.ToArray();
        RequiredCapabilities = request.RequiredCapabilities.OrderBy(value => value, StringComparer.Ordinal).ToArray();
        RuntimeEvidenceIdentity = RuntimeCanonical.Hash(
            request.RuntimeInstanceId,
            request.ApplicationIdentity,
            request.ApplicationVersion,
            request.ExpectedArtifactExactIdentity,
            request.Admission.EvidenceIdentity,
            request.LifecycleEligibility.DecisionIdentity,
            registrationDecisionIdentity);
    }

    public string RuntimeInstanceId { get; }
    public string ApplicationIdentity { get; }
    public string ApplicationVersion { get; }
    public RuntimeSlotState State { get; set; }
    public IReadOnlyList<RuntimeCapabilityDeclaration> ProvidedCapabilities { get; }
    public IReadOnlyList<string> RequiredCapabilities { get; }
    public string RuntimeEvidenceIdentity { get; }
}

public sealed class ApplicationRuntimeHost
{
    private readonly object _sync = new();
    private readonly Dictionary<string, RuntimeSlot> _slots = new(StringComparer.Ordinal);

    public ApplicationRuntimeHost(string hostIdentity)
    {
        HostIdentity = RuntimeRules.Required(hostIdentity, nameof(hostIdentity));
    }

    public string HostIdentity { get; }

    public RuntimeRegistrationDecision Register(RuntimeRegistrationRequest request)
    {
        if (request is null)
        {
            return RegistrationDeny("INVALID_REGISTRATION_REQUEST");
        }

        lock (_sync)
        {
            var reason = ValidateRegistration(request);
            if (reason is not null)
            {
                return RegistrationDeny(reason, request);
            }

            var decisionIdentity = RuntimeCanonical.Hash(
                "REGISTER",
                request.RuntimeInstanceId,
                request.ApplicationIdentity,
                request.ApplicationVersion,
                request.ExpectedArtifactExactIdentity,
                request.Admission.EvidenceIdentity,
                request.LifecycleEligibility.DecisionIdentity,
                ResourceDigest(request.ResourceGrants),
                CapabilityDigest(request.ProvidedCapabilities),
                RequiredCapabilityDigest(request.RequiredCapabilities),
                request.ObservedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture));

            _slots.Add(request.RuntimeInstanceId, new RuntimeSlot(request, decisionIdentity));

            return new RuntimeRegistrationDecision(
                true,
                "RUNTIME_REGISTERED_NOT_ACTIVATED",
                request.RuntimeInstanceId,
                request.ApplicationIdentity,
                request.ApplicationVersion,
                decisionIdentity,
                false,
                false,
                false);
        }
    }

    public RuntimeTransitionDecision Activate(string runtimeInstanceId, RuntimeAuthorityEvidence authority, DateTimeOffset observedAt)
        => Transition(runtimeInstanceId, RuntimeAuthorityAction.Activate, authority, observedAt, null);

    public RuntimeTransitionDecision Suspend(string runtimeInstanceId, RuntimeAuthorityEvidence authority, DateTimeOffset observedAt)
        => Transition(runtimeInstanceId, RuntimeAuthorityAction.Suspend, authority, observedAt, null);

    public RuntimeTransitionDecision Isolate(string runtimeInstanceId, RuntimeAuthorityEvidence authority, DateTimeOffset observedAt)
        => Transition(runtimeInstanceId, RuntimeAuthorityAction.Isolate, authority, observedAt, null);

    public RuntimeTransitionDecision Remove(
        string runtimeInstanceId,
        RuntimeAuthorityEvidence authority,
        RuntimeLifecycleEligibilityBinding removalEligibility,
        DateTimeOffset observedAt)
        => Transition(runtimeInstanceId, RuntimeAuthorityAction.Remove, authority, observedAt, removalEligibility);

    public CapabilityResolutionDecision ResolveCapability(string consumerRuntimeInstanceId, string capabilityId)
    {
        lock (_sync)
        {
            if (!_slots.TryGetValue(consumerRuntimeInstanceId ?? string.Empty, out var consumer) || consumer.State != RuntimeSlotState.Active)
            {
                return CapabilityDeny("CONSUMER_NOT_ACTIVE", capabilityId);
            }

            if (string.IsNullOrWhiteSpace(capabilityId))
            {
                return CapabilityDeny("INVALID_CAPABILITY_ID", capabilityId);
            }

            var normalized = capabilityId.Trim();
            var providers = _slots.Values
                .Where(slot => slot.State == RuntimeSlotState.Active)
                .SelectMany(slot => slot.ProvidedCapabilities.Select(capability => (slot, capability)))
                .Where(item => string.Equals(item.capability.CapabilityId, normalized, StringComparison.Ordinal))
                .OrderBy(item => item.slot.RuntimeInstanceId, StringComparer.Ordinal)
                .ToArray();

            if (providers.Length == 0)
            {
                return CapabilityDeny("CAPABILITY_UNAVAILABLE", normalized);
            }

            foreach (var provider in providers)
            {
                if (provider.capability.Visibility == RuntimeCapabilityVisibility.Private)
                {
                    if (string.Equals(provider.slot.RuntimeInstanceId, consumer.RuntimeInstanceId, StringComparison.Ordinal))
                    {
                        return CapabilityAllow("PRIVATE_SELF_CAPABILITY", normalized, provider.slot);
                    }

                    continue;
                }

                if (!consumer.RequiredCapabilities.Contains(normalized, StringComparer.Ordinal))
                {
                    continue;
                }

                return CapabilityAllow("DECLARED_SHARED_CAPABILITY", normalized, provider.slot);
            }

            return CapabilityDeny("CAPABILITY_ACCESS_DENIED", normalized);
        }
    }

    public RuntimeHostProjection Snapshot(DateTimeOffset observedAt)
    {
        lock (_sync)
        {
            var slots = _slots.Values
                .Where(slot => slot.State != RuntimeSlotState.Removed)
                .OrderBy(slot => slot.RuntimeInstanceId, StringComparer.Ordinal)
                .Select(slot => new RuntimeSlotProjection(
                    slot.RuntimeInstanceId,
                    slot.ApplicationIdentity,
                    slot.ApplicationVersion,
                    slot.State,
                    slot.ProvidedCapabilities.Select(value => value.CapabilityId).OrderBy(value => value, StringComparer.Ordinal).ToArray(),
                    slot.RequiredCapabilities.OrderBy(value => value, StringComparer.Ordinal).ToArray(),
                    slot.RuntimeEvidenceIdentity))
                .ToArray();

            var projectionIdentity = RuntimeCanonical.Hash(
                HostIdentity,
                observedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture),
                string.Join("||", slots.Select(slot => string.Join("|", new[]
                {
                    slot.RuntimeInstanceId,
                    slot.ApplicationIdentity,
                    slot.ApplicationVersion,
                    slot.State.ToString(),
                    string.Join(",", slot.ProvidedCapabilities),
                    string.Join(",", slot.RequiredCapabilities),
                    slot.RuntimeEvidenceIdentity
                }))));

            return new RuntimeHostProjection(
                HostIdentity,
                observedAt,
                slots,
                projectionIdentity,
                true,
                false,
                false);
        }
    }

    private string? ValidateRegistration(RuntimeRegistrationRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.RuntimeInstanceId) ||
            string.IsNullOrWhiteSpace(request.ApplicationIdentity) ||
            string.IsNullOrWhiteSpace(request.ApplicationVersion) ||
            string.IsNullOrWhiteSpace(request.ExpectedArtifactExactIdentity) ||
            request.ObservedAt == default ||
            request.ArtifactConsumption is null ||
            request.Admission is null ||
            request.LifecycleEligibility is null ||
            request.ResourceGrants is null ||
            request.ProvidedCapabilities is null ||
            request.RequiredCapabilities is null)
        {
            return "MISSING_RUNTIME_REGISTRATION_EVIDENCE";
        }

        if (_slots.ContainsKey(request.RuntimeInstanceId))
        {
            return "DUPLICATE_RUNTIME_INSTANCE";
        }

        if (_slots.Values.Any(slot => slot.State != RuntimeSlotState.Removed &&
            string.Equals(slot.ApplicationIdentity, request.ApplicationIdentity, StringComparison.Ordinal)))
        {
            return "APPLICATION_ALREADY_HOSTED";
        }

        if (!request.ArtifactConsumption.AcceptedForTechnicalConsumption ||
            request.ArtifactConsumption.ActivationAuthorized ||
            request.ArtifactConsumption.DeploymentAuthorized ||
            request.ArtifactConsumption.ProductionAuthorized ||
            request.ArtifactConsumption.BusinessAuthorityGranted ||
            request.ArtifactConsumption.SilentUpgradePerformed ||
            !string.Equals(request.ArtifactConsumption.ExactArtifactIdentity, request.ExpectedArtifactExactIdentity, StringComparison.Ordinal))
        {
            return "INVALID_STAGE14_ARTIFACT_BINDING";
        }

        if (!request.Admission.Admitted ||
            string.IsNullOrWhiteSpace(request.Admission.EvidenceIdentity) ||
            !string.Equals(request.Admission.ApplicationIdentity, request.ApplicationIdentity, StringComparison.Ordinal) ||
            !string.Equals(request.Admission.ApplicationVersion, request.ApplicationVersion, StringComparison.Ordinal))
        {
            return "INVALID_ADMISSION_BINDING";
        }

        if (!request.LifecycleEligibility.Eligible ||
            request.LifecycleEligibility.Kind != RuntimeLifecycleEligibilityKind.Attach ||
            string.IsNullOrWhiteSpace(request.LifecycleEligibility.DecisionIdentity) ||
            !string.Equals(request.LifecycleEligibility.ApplicationIdentity, request.ApplicationIdentity, StringComparison.Ordinal) ||
            !string.Equals(request.LifecycleEligibility.TargetVersion, request.ApplicationVersion, StringComparison.Ordinal))
        {
            return "INVALID_LIFECYCLE_ATTACH_BINDING";
        }

        if (request.ResourceGrants.Count == 0)
        {
            return "MISSING_STAGE6_RESOURCE_GRANTS";
        }

        var resourceReason = ValidateResourceGrants(request);
        if (resourceReason is not null)
        {
            return resourceReason;
        }

        return ValidateCapabilities(request);
    }

    private static string? ValidateResourceGrants(RuntimeRegistrationRequest request)
    {
        var seenGrants = new HashSet<string>(StringComparer.Ordinal);
        var seenClasses = new HashSet<string>(StringComparer.Ordinal);

        foreach (var grant in request.ResourceGrants)
        {
            if (grant is null ||
                string.IsNullOrWhiteSpace(grant.GrantIdentity) ||
                string.IsNullOrWhiteSpace(grant.ResourceClassIdentity) ||
                string.IsNullOrWhiteSpace(grant.EvidenceIdentity) ||
                !string.Equals(grant.ApplicationIdentity, request.ApplicationIdentity, StringComparison.Ordinal))
            {
                return "RESOURCE_GRANT_APPLICATION_OR_EVIDENCE_MISMATCH";
            }

            if (!seenGrants.Add(grant.GrantIdentity) || !seenClasses.Add(grant.ResourceClassIdentity))
            {
                return "DUPLICATE_RESOURCE_GRANT_BINDING";
            }

            if (grant.Allocation < 0m || grant.Quota < 0m || grant.Ceiling < 0m ||
                grant.Allocation > grant.Quota || grant.Quota > grant.Ceiling)
            {
                return "INVALID_RESOURCE_GRANT_LIMITS";
            }

            if (grant.EffectiveFrom > request.ObservedAt ||
                (grant.EffectiveUntil.HasValue && grant.EffectiveUntil.Value < request.ObservedAt))
            {
                return "RESOURCE_GRANT_NOT_CURRENT";
            }

            if (grant.EvidenceObservedAt > request.ObservedAt)
            {
                return "RESOURCE_EVIDENCE_FROM_FUTURE";
            }
        }

        return null;
    }

    private string? ValidateCapabilities(RuntimeRegistrationRequest request)
    {
        var declared = new HashSet<string>(StringComparer.Ordinal);
        foreach (var capability in request.ProvidedCapabilities)
        {
            if (capability is null || string.IsNullOrWhiteSpace(capability.CapabilityId) ||
                !Enum.IsDefined(capability.Visibility) || !declared.Add(capability.CapabilityId))
            {
                return "DUPLICATE_OR_INVALID_CAPABILITY_DECLARATION";
            }

            if (capability.Exclusive && _slots.Values
                .Where(slot => slot.State != RuntimeSlotState.Removed)
                .SelectMany(slot => slot.ProvidedCapabilities)
                .Any(existing => existing.Exclusive && string.Equals(existing.CapabilityId, capability.CapabilityId, StringComparison.Ordinal)))
            {
                return "EXCLUSIVE_CAPABILITY_ALREADY_OWNED";
            }
        }

        var required = new HashSet<string>(StringComparer.Ordinal);
        foreach (var capability in request.RequiredCapabilities)
        {
            if (string.IsNullOrWhiteSpace(capability) || !required.Add(capability))
            {
                return "DUPLICATE_OR_INVALID_REQUIRED_CAPABILITY";
            }
        }

        return null;
    }

    private RuntimeTransitionDecision Transition(
        string runtimeInstanceId,
        RuntimeAuthorityAction action,
        RuntimeAuthorityEvidence authority,
        DateTimeOffset observedAt,
        RuntimeLifecycleEligibilityBinding? removalEligibility)
    {
        lock (_sync)
        {
            if (!_slots.TryGetValue(runtimeInstanceId ?? string.Empty, out var slot))
            {
                return TransitionDeny("RUNTIME_INSTANCE_NOT_FOUND", runtimeInstanceId, RuntimeSlotState.Removed);
            }

            var authorityReason = ValidateAuthority(slot, action, authority, observedAt);
            if (authorityReason is not null)
            {
                return TransitionDeny(authorityReason, runtimeInstanceId, slot.State);
            }

            RuntimeSlotState target;
            switch (action)
            {
                case RuntimeAuthorityAction.Activate:
                    if (slot.State != RuntimeSlotState.Registered)
                    {
                        return TransitionDeny("INVALID_ACTIVATION_STATE", runtimeInstanceId, slot.State);
                    }
                    target = RuntimeSlotState.Active;
                    break;

                case RuntimeAuthorityAction.Suspend:
                    if (slot.State != RuntimeSlotState.Active)
                    {
                        return TransitionDeny("INVALID_SUSPEND_STATE", runtimeInstanceId, slot.State);
                    }
                    target = RuntimeSlotState.Suspended;
                    break;

                case RuntimeAuthorityAction.Isolate:
                    if (slot.State is RuntimeSlotState.Removed or RuntimeSlotState.Isolated)
                    {
                        return TransitionDeny("INVALID_ISOLATION_STATE", runtimeInstanceId, slot.State);
                    }
                    target = RuntimeSlotState.Isolated;
                    break;

                case RuntimeAuthorityAction.Remove:
                    if (slot.State == RuntimeSlotState.Removed)
                    {
                        return TransitionDeny("RUNTIME_ALREADY_REMOVED", runtimeInstanceId, slot.State);
                    }

                    if (removalEligibility is null ||
                        !removalEligibility.Eligible ||
                        removalEligibility.Kind != RuntimeLifecycleEligibilityKind.DetachOrRemove ||
                        string.IsNullOrWhiteSpace(removalEligibility.DecisionIdentity) ||
                        !string.Equals(removalEligibility.ApplicationIdentity, slot.ApplicationIdentity, StringComparison.Ordinal) ||
                        !string.Equals(removalEligibility.CurrentVersion, slot.ApplicationVersion, StringComparison.Ordinal))
                    {
                        return TransitionDeny("INVALID_LIFECYCLE_REMOVAL_BINDING", runtimeInstanceId, slot.State);
                    }

                    target = RuntimeSlotState.Removed;
                    break;

                default:
                    return TransitionDeny("UNSUPPORTED_RUNTIME_ACTION", runtimeInstanceId, slot.State);
            }

            slot.State = target;
            var decisionIdentity = RuntimeCanonical.Hash(
                action.ToString(),
                slot.RuntimeInstanceId,
                slot.ApplicationIdentity,
                slot.ApplicationVersion,
                target.ToString(),
                authority.AuthorityIdentity,
                authority.EvidenceIdentity,
                observedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture),
                removalEligibility?.DecisionIdentity ?? string.Empty);

            return new RuntimeTransitionDecision(
                true,
                "RUNTIME_TRANSITION_ACCEPTED",
                slot.RuntimeInstanceId,
                target,
                decisionIdentity,
                false,
                false);
        }
    }

    private static string? ValidateAuthority(RuntimeSlot slot, RuntimeAuthorityAction action, RuntimeAuthorityEvidence authority, DateTimeOffset observedAt)
    {
        if (authority is null ||
            authority.Status != RuntimeAuthorityStatus.Valid ||
            authority.Action != action ||
            string.IsNullOrWhiteSpace(authority.AuthorityIdentity) ||
            string.IsNullOrWhiteSpace(authority.EvidenceIdentity) ||
            !string.Equals(authority.RuntimeInstanceId, slot.RuntimeInstanceId, StringComparison.Ordinal) ||
            !string.Equals(authority.ApplicationIdentity, slot.ApplicationIdentity, StringComparison.Ordinal) ||
            !string.Equals(authority.ApplicationVersion, slot.ApplicationVersion, StringComparison.Ordinal) ||
            authority.EffectiveFrom > observedAt ||
            authority.EffectiveUntil < observedAt ||
            authority.EffectiveUntil < authority.EffectiveFrom)
        {
            return "INVALID_RUNTIME_ACTION_AUTHORITY";
        }

        return null;
    }

    private static RuntimeRegistrationDecision RegistrationDeny(string reason, RuntimeRegistrationRequest? request = null)
    {
        var runtimeId = request?.RuntimeInstanceId ?? "NONE";
        var appId = request?.ApplicationIdentity ?? "NONE";
        var version = request?.ApplicationVersion ?? "NONE";
        return new RuntimeRegistrationDecision(
            false,
            reason,
            runtimeId,
            appId,
            version,
            RuntimeCanonical.Hash("REGISTER_DENY", reason, runtimeId, appId, version),
            false,
            false,
            false);
    }

    private static RuntimeTransitionDecision TransitionDeny(string reason, string? runtimeId, RuntimeSlotState state)
        => new(false, reason, runtimeId ?? "NONE", state,
            RuntimeCanonical.Hash("TRANSITION_DENY", reason, runtimeId ?? "NONE", state.ToString()), false, false);

    private static CapabilityResolutionDecision CapabilityDeny(string reason, string? capabilityId)
        => new(false, reason, capabilityId ?? "NONE", "NONE", "NONE",
            RuntimeCanonical.Hash("CAPABILITY_DENY", reason, capabilityId ?? "NONE"));

    private static CapabilityResolutionDecision CapabilityAllow(string reason, string capabilityId, RuntimeSlot provider)
        => new(true, reason, capabilityId, provider.RuntimeInstanceId, provider.ApplicationIdentity,
            RuntimeCanonical.Hash("CAPABILITY_ALLOW", reason, capabilityId, provider.RuntimeInstanceId, provider.ApplicationIdentity));

    private static string ResourceDigest(IReadOnlyList<RuntimeResourceGrantBinding> grants)
        => RuntimeCanonical.Hash(grants
            .OrderBy(value => value.ResourceClassIdentity, StringComparer.Ordinal)
            .Select(value => string.Join("|", new[]
            {
                value.GrantIdentity,
                value.ApplicationIdentity,
                value.ResourceClassIdentity,
                value.Allocation.ToString("G29", CultureInfo.InvariantCulture),
                value.Quota.ToString("G29", CultureInfo.InvariantCulture),
                value.Ceiling.ToString("G29", CultureInfo.InvariantCulture),
                value.EffectiveFrom.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture),
                value.EffectiveUntil?.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture) ?? "OPEN",
                value.EvidenceObservedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture),
                value.EvidenceIdentity
            })).ToArray());

    private static string CapabilityDigest(IReadOnlyList<RuntimeCapabilityDeclaration> capabilities)
        => RuntimeCanonical.Hash(capabilities
            .OrderBy(value => value.CapabilityId, StringComparer.Ordinal)
            .Select(value => $"{value.CapabilityId}|{value.Visibility}|{value.Exclusive}")
            .ToArray());

    private static string RequiredCapabilityDigest(IReadOnlyList<string> capabilities)
        => RuntimeCanonical.Hash(capabilities.OrderBy(value => value, StringComparer.Ordinal).ToArray());
}

internal static class RuntimeRules
{
    public static string Required(string value, string name)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Required runtime identifier is missing.", name);
        }

        return value.Trim();
    }
}

internal static class RuntimeCanonical
{
    public static string Hash(params string[] values)
    {
        using var hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
        Span<byte> prefix = stackalloc byte[4];
        foreach (var value in values)
        {
            var text = value ?? string.Empty;
            var bytes = Encoding.UTF8.GetBytes(text);
            prefix[0] = (byte)((bytes.Length >> 24) & 0xFF);
            prefix[1] = (byte)((bytes.Length >> 16) & 0xFF);
            prefix[2] = (byte)((bytes.Length >> 8) & 0xFF);
            prefix[3] = (byte)(bytes.Length & 0xFF);
            hash.AppendData(prefix);
            hash.AppendData(bytes);
        }

        return "sha256/" + Convert.ToHexString(hash.GetHashAndReset());
    }
}