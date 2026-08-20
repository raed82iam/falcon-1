using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Foundation.Contracts;

namespace Foundation.Authority;

public static class StandingOwnerPreapprovalReason
{
    public const string Accepted = "STANDING_OWNER_PREAPPROVAL_ACCEPTED";
    public const string InvalidProfile = "STANDING_OWNER_PREAPPROVAL_PROFILE_INVALID";
    public const string ProfileRevoked = "STANDING_OWNER_PREAPPROVAL_PROFILE_REVOKED";
    public const string ProfileExpired = "STANDING_OWNER_PREAPPROVAL_PROFILE_EXPIRED";
    public const string InvalidCandidate = "STANDING_OWNER_PREAPPROVAL_CANDIDATE_INVALID";
    public const string ManualOnlyClass = "STANDING_OWNER_PREAPPROVAL_MANUAL_ONLY_CLASS";
    public const string ActorMismatch = "STANDING_OWNER_PREAPPROVAL_ACTOR_MISMATCH";
    public const string ApplicationMismatch = "STANDING_OWNER_PREAPPROVAL_APPLICATION_MISMATCH";
    public const string UpdateClassMismatch = "STANDING_OWNER_PREAPPROVAL_UPDATE_CLASS_MISMATCH";
    public const string ResourceMismatch = "STANDING_OWNER_PREAPPROVAL_RESOURCE_MISMATCH";
    public const string PurposeMismatch = "STANDING_OWNER_PREAPPROVAL_PURPOSE_MISMATCH";
    public const string ScopeMismatch = "STANDING_OWNER_PREAPPROVAL_SCOPE_MISMATCH";
    public const string EnvironmentMismatch = "STANDING_OWNER_PREAPPROVAL_ENVIRONMENT_MISMATCH";
    public const string SecurityContextMismatch = "STANDING_OWNER_PREAPPROVAL_SECURITY_CONTEXT_MISMATCH";
    public const string RiskExceeded = "STANDING_OWNER_PREAPPROVAL_RISK_EXCEEDED";
    public const string EvidenceMismatch = "STANDING_OWNER_PREAPPROVAL_EVIDENCE_MISMATCH";
    public const string UnderlyingAuthorityDenied = "STANDING_OWNER_PREAPPROVAL_AUTHORITY_DENIED";
}

public sealed record StandingOwnerPreapprovalProfile
{
    private readonly ReadOnlyCollection<string> _authorizedActors;
    private readonly ReadOnlyCollection<string> _authorizedApplications;
    private readonly ReadOnlyCollection<string> _allowedUpdateClasses;
    private readonly ReadOnlyCollection<string> _allowedResources;
    private readonly ReadOnlyCollection<string> _allowedPurposes;
    private readonly ReadOnlyCollection<string> _authorizedScopes;
    private readonly ReadOnlyCollection<string> _allowedEnvironments;
    private readonly ReadOnlyCollection<string> _acceptedSecurityContexts;

    public StandingOwnerPreapprovalProfile(
        string policyId,
        string policyVersion,
        string ownerIdentity,
        string ownerAuthorityProvenance,
        string delegationId,
        IEnumerable<string> authorizedActors,
        IEnumerable<string> authorizedApplications,
        IEnumerable<string> allowedUpdateClasses,
        IEnumerable<string> allowedResources,
        IEnumerable<string> allowedPurposes,
        IEnumerable<string> authorizedScopes,
        IEnumerable<string> allowedEnvironments,
        IEnumerable<string> acceptedSecurityContexts,
        int maximumRiskTier,
        DateTimeOffset effectiveFrom,
        DateTimeOffset expiry,
        bool isRevoked,
        string evidenceReference)
    {
        PolicyId = CleanRequired(policyId, nameof(policyId));
        PolicyVersion = CleanRequired(policyVersion, nameof(policyVersion));
        OwnerIdentity = CleanRequired(ownerIdentity, nameof(ownerIdentity));
        OwnerAuthorityProvenance = CleanRequired(ownerAuthorityProvenance, nameof(ownerAuthorityProvenance));
        DelegationId = CleanRequired(delegationId, nameof(delegationId));
        EvidenceReference = CleanRequired(evidenceReference, nameof(evidenceReference));
        _authorizedActors = Materialize(authorizedActors, nameof(authorizedActors));
        _authorizedApplications = Materialize(authorizedApplications, nameof(authorizedApplications));
        _allowedUpdateClasses = Materialize(allowedUpdateClasses, nameof(allowedUpdateClasses));
        _allowedResources = Materialize(allowedResources, nameof(allowedResources));
        _allowedPurposes = Materialize(allowedPurposes, nameof(allowedPurposes));
        _authorizedScopes = Materialize(authorizedScopes, nameof(authorizedScopes));
        _allowedEnvironments = Materialize(allowedEnvironments, nameof(allowedEnvironments));
        _acceptedSecurityContexts = Materialize(acceptedSecurityContexts, nameof(acceptedSecurityContexts));
        if (maximumRiskTier < 0) throw new ArgumentOutOfRangeException(nameof(maximumRiskTier));
        if (effectiveFrom == default || expiry <= effectiveFrom) throw new ArgumentException("Invalid standing preapproval validity window.");
        MaximumRiskTier = maximumRiskTier;
        EffectiveFrom = effectiveFrom;
        Expiry = expiry;
        IsRevoked = isRevoked;
        IdentitySha256 = ComputeIdentity();
    }

    public string PolicyId { get; }
    public string PolicyVersion { get; }
    public string OwnerIdentity { get; }
    public string OwnerAuthorityProvenance { get; }
    public string DelegationId { get; }
    public IReadOnlyList<string> AuthorizedActors => _authorizedActors;
    public IReadOnlyList<string> AuthorizedApplications => _authorizedApplications;
    public IReadOnlyList<string> AllowedUpdateClasses => _allowedUpdateClasses;
    public IReadOnlyList<string> AllowedResources => _allowedResources;
    public IReadOnlyList<string> AllowedPurposes => _allowedPurposes;
    public IReadOnlyList<string> AuthorizedScopes => _authorizedScopes;
    public IReadOnlyList<string> AllowedEnvironments => _allowedEnvironments;
    public IReadOnlyList<string> AcceptedSecurityContexts => _acceptedSecurityContexts;
    public int MaximumRiskTier { get; }
    public DateTimeOffset EffectiveFrom { get; }
    public DateTimeOffset Expiry { get; }
    public bool IsRevoked { get; }
    public string EvidenceReference { get; }
    public string IdentitySha256 { get; }

    private string ComputeIdentity() => StandingOwnerPreapprovalEvaluator.ComputeSha256(string.Join("\n",
        "policy.id=" + PolicyId,
        "policy.version=" + PolicyVersion,
        "policy.owner=" + OwnerIdentity,
        "policy.provenance=" + OwnerAuthorityProvenance,
        "policy.delegation=" + DelegationId,
        "policy.actors=" + Join(_authorizedActors),
        "policy.applications=" + Join(_authorizedApplications),
        "policy.classes=" + Join(_allowedUpdateClasses),
        "policy.resources=" + Join(_allowedResources),
        "policy.purposes=" + Join(_allowedPurposes),
        "policy.scopes=" + Join(_authorizedScopes),
        "policy.environments=" + Join(_allowedEnvironments),
        "policy.securityContexts=" + Join(_acceptedSecurityContexts),
        "policy.maximumRiskTier=" + MaximumRiskTier.ToString(CultureInfo.InvariantCulture),
        "policy.effectiveFrom=" + EffectiveFrom.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture),
        "policy.expiry=" + Expiry.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture),
        "policy.revoked=" + (IsRevoked ? "true" : "false"),
        "policy.evidence=" + EvidenceReference));

    private static string Join(IEnumerable<string> values) => string.Join(",", values.OrderBy(value => value, StringComparer.Ordinal));

    private static string CleanRequired(string value, string name) =>
        string.IsNullOrWhiteSpace(value) ? throw new ArgumentException("Value is required.", name) : value.Trim();

    private static ReadOnlyCollection<string> Materialize(IEnumerable<string> values, string name)
    {
        ArgumentNullException.ThrowIfNull(values);
        var materialized = values.Select(value => CleanRequired(value, name)).Distinct(StringComparer.Ordinal).OrderBy(value => value, StringComparer.Ordinal).ToArray();
        if (materialized.Length == 0) throw new ArgumentException("At least one value is required.", name);
        return Array.AsReadOnly(materialized);
    }
}

public sealed record StandingOwnerPreapprovalCandidate(
    string CandidateId,
    string CandidateVersion,
    string CandidateSha256,
    string ActorIdentity,
    string ApplicationIdentity,
    string UpdateClass,
    string Resource,
    string Purpose,
    string RequestedScope,
    string Environment,
    string SecurityContext,
    int RiskTier,
    string RequiredFitnessToOperate,
    string Correlation,
    string EvidenceReference,
    DateTimeOffset RequestTime,
    DateTimeOffset Expiry);

public sealed record StandingOwnerPreapprovalDecision(
    bool AcceptedUnderStandingPreapproval,
    string Reason,
    string DecisionIdentitySha256,
    string ProfileIdentitySha256,
    string CandidateIdentitySha256,
    string UnderlyingAuthorityDecisionId,
    bool ExecutionAuthorized,
    bool DeploymentAuthorized,
    bool BusinessAuthorityGranted,
    DateTimeOffset DecisionTime,
    DateTimeOffset Expiry,
    string EvidenceReference);

public sealed class StandingOwnerPreapprovalEvaluator
{
    private static readonly HashSet<string> ManualOnlyClasses = new(StringComparer.Ordinal)
    {
        "AI_KILL",
        "RELEASE",
        "CONTROLLED_REVIVAL",
        "LIVE_TRADING_ACTIVATION",
        "CREDENTIAL_OR_SECURITY_CHANGE",
        "AUTHORITY_EXPANSION",
        "DEPLOYMENT",
        "CONSTITUTION_OR_GOVERNANCE_CHANGE"
    };

    private readonly DefaultDenyAuthorityEngine _authorityEngine = new();

    internal StandingOwnerPreapprovalDecision Evaluate(
        StandingOwnerPreapprovalProfile? profile,
        StandingOwnerPreapprovalCandidate? candidate,
        FitnessEvidence? fitness,
        DateTimeOffset observationTime)
    {
        if (!ValidProfile(profile)) return Deny(StandingOwnerPreapprovalReason.InvalidProfile, profile, candidate, observationTime);
        if (profile!.IsRevoked) return Deny(StandingOwnerPreapprovalReason.ProfileRevoked, profile, candidate, observationTime);
        if (observationTime < profile.EffectiveFrom || observationTime >= profile.Expiry)
            return Deny(StandingOwnerPreapprovalReason.ProfileExpired, profile, candidate, observationTime);
        if (!ValidCandidate(candidate)) return Deny(StandingOwnerPreapprovalReason.InvalidCandidate, profile, candidate, observationTime);

        var c = candidate!;
        if (ManualOnlyClasses.Contains(c.UpdateClass.Trim())) return Deny(StandingOwnerPreapprovalReason.ManualOnlyClass, profile, c, observationTime);
        if (!Contains(profile.AuthorizedActors, c.ActorIdentity)) return Deny(StandingOwnerPreapprovalReason.ActorMismatch, profile, c, observationTime);
        if (!Contains(profile.AuthorizedApplications, c.ApplicationIdentity)) return Deny(StandingOwnerPreapprovalReason.ApplicationMismatch, profile, c, observationTime);
        if (!Contains(profile.AllowedUpdateClasses, c.UpdateClass)) return Deny(StandingOwnerPreapprovalReason.UpdateClassMismatch, profile, c, observationTime);
        if (!Contains(profile.AllowedResources, c.Resource)) return Deny(StandingOwnerPreapprovalReason.ResourceMismatch, profile, c, observationTime);
        if (!Contains(profile.AllowedPurposes, c.Purpose)) return Deny(StandingOwnerPreapprovalReason.PurposeMismatch, profile, c, observationTime);
        if (!ScopeAllowed(profile.AuthorizedScopes, c.RequestedScope)) return Deny(StandingOwnerPreapprovalReason.ScopeMismatch, profile, c, observationTime);
        if (!Contains(profile.AllowedEnvironments, c.Environment)) return Deny(StandingOwnerPreapprovalReason.EnvironmentMismatch, profile, c, observationTime);
        if (!Contains(profile.AcceptedSecurityContexts, c.SecurityContext)) return Deny(StandingOwnerPreapprovalReason.SecurityContextMismatch, profile, c, observationTime);
        if (c.RiskTier > profile.MaximumRiskTier) return Deny(StandingOwnerPreapprovalReason.RiskExceeded, profile, c, observationTime);
        if (!StringComparer.Ordinal.Equals(profile.EvidenceReference, c.EvidenceReference.Trim()))
            return Deny(StandingOwnerPreapprovalReason.EvidenceMismatch, profile, c, observationTime);

        var authorityRequest = new AuthorityRequest(
            "standing-preapproval:" + c.CandidateId.Trim() + ":" + c.CandidateVersion.Trim(),
            c.ActorIdentity.Trim(),
            c.UpdateClass.Trim(),
            c.Resource.Trim(),
            c.Purpose.Trim(),
            c.RequestedScope.Trim(),
            c.Environment.Trim(),
            c.SecurityContext.Trim(),
            c.RequiredFitnessToOperate.Trim(),
            c.Correlation.Trim(),
            c.RequestTime,
            c.Expiry);

        var policy = new AuthorityPolicy(
            profile.PolicyId,
            profile.PolicyVersion,
            profile.OwnerAuthorityProvenance,
            profile.EffectiveFrom,
            profile.Expiry,
            profile.AuthorizedActors,
            profile.AllowedUpdateClasses,
            profile.AllowedResources,
            profile.AllowedPurposes,
            profile.AuthorizedScopes,
            profile.AcceptedSecurityContexts);

        var delegation = new DelegationEvidence(
            profile.DelegationId,
            c.ActorIdentity.Trim(),
            profile.OwnerAuthorityProvenance,
            profile.AuthorizedScopes,
            profile.EffectiveFrom,
            profile.Expiry,
            profile.IsRevoked);

        var context = new AuthorityEvaluationContext(policy, delegation, fitness, observationTime, profile.EvidenceReference);
        var authority = _authorityEngine.Evaluate(authorityRequest, context);
        if (!StringComparer.Ordinal.Equals(authority.Decision, AuthorityDecision.Allow))
            return Deny(StandingOwnerPreapprovalReason.UnderlyingAuthorityDenied, profile, c, observationTime, authority.DecisionId, authority.Expiry);

        var candidateIdentity = CandidateIdentity(c);
        var expiry = authority.Expiry < profile.Expiry ? authority.Expiry : profile.Expiry;
        var decisionIdentity = ComputeSha256(string.Join("\n",
            "result=ACCEPTED_UNDER_STANDING_OWNER_PREAPPROVAL",
            "profile=" + profile.IdentitySha256,
            "candidate=" + candidateIdentity,
            "authorityDecision=" + authority.DecisionId,
            "observation=" + observationTime.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture),
            "expiry=" + expiry.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture),
            "executionAuthorized=false",
            "deploymentAuthorized=false",
            "businessAuthorityGranted=false"));

        return new StandingOwnerPreapprovalDecision(
            true,
            StandingOwnerPreapprovalReason.Accepted,
            decisionIdentity,
            profile.IdentitySha256,
            candidateIdentity,
            authority.DecisionId,
            false,
            false,
            false,
            observationTime,
            expiry,
            profile.EvidenceReference);
    }

    public static bool IsManualOnlyClass(string updateClass) =>
        !string.IsNullOrWhiteSpace(updateClass) && ManualOnlyClasses.Contains(updateClass.Trim());

    internal static string ComputeSha256(string canonical)
    {
        var digest = SHA256.HashData(Encoding.UTF8.GetBytes(canonical));
        return "sha256/" + Convert.ToHexString(digest);
    }

    private static bool ValidProfile(StandingOwnerPreapprovalProfile? profile) =>
        profile is not null &&
        !string.IsNullOrWhiteSpace(profile.PolicyId) &&
        !string.IsNullOrWhiteSpace(profile.PolicyVersion) &&
        !string.IsNullOrWhiteSpace(profile.OwnerIdentity) &&
        !string.IsNullOrWhiteSpace(profile.OwnerAuthorityProvenance) &&
        !string.IsNullOrWhiteSpace(profile.DelegationId) &&
        !string.IsNullOrWhiteSpace(profile.EvidenceReference) &&
        profile.EffectiveFrom != default && profile.Expiry > profile.EffectiveFrom &&
        profile.AuthorizedActors.Count > 0 && profile.AuthorizedApplications.Count > 0 &&
        profile.AllowedUpdateClasses.Count > 0 && profile.AllowedResources.Count > 0 &&
        profile.AllowedPurposes.Count > 0 && profile.AuthorizedScopes.Count > 0 &&
        profile.AllowedEnvironments.Count > 0 && profile.AcceptedSecurityContexts.Count > 0;

    private static bool ValidCandidate(StandingOwnerPreapprovalCandidate? candidate) =>
        candidate is not null &&
        Required(candidate.CandidateId) && Required(candidate.CandidateVersion) && ValidSha256(candidate.CandidateSha256) &&
        Required(candidate.ActorIdentity) && Required(candidate.ApplicationIdentity) && Required(candidate.UpdateClass) &&
        Required(candidate.Resource) && Required(candidate.Purpose) && Required(candidate.RequestedScope) &&
        Required(candidate.Environment) && Required(candidate.SecurityContext) && candidate.RiskTier >= 0 &&
        Required(candidate.RequiredFitnessToOperate) && Required(candidate.Correlation) && Required(candidate.EvidenceReference) &&
        candidate.RequestTime != default && candidate.Expiry > candidate.RequestTime;

    private static string CandidateIdentity(StandingOwnerPreapprovalCandidate candidate) => ComputeSha256(string.Join("\n",
        "candidate.id=" + candidate.CandidateId.Trim(),
        "candidate.version=" + candidate.CandidateVersion.Trim(),
        "candidate.digest=" + candidate.CandidateSha256.Trim().ToUpperInvariant(),
        "candidate.actor=" + candidate.ActorIdentity.Trim(),
        "candidate.application=" + candidate.ApplicationIdentity.Trim(),
        "candidate.class=" + candidate.UpdateClass.Trim(),
        "candidate.resource=" + candidate.Resource.Trim(),
        "candidate.purpose=" + candidate.Purpose.Trim(),
        "candidate.scope=" + candidate.RequestedScope.Trim(),
        "candidate.environment=" + candidate.Environment.Trim(),
        "candidate.securityContext=" + candidate.SecurityContext.Trim(),
        "candidate.riskTier=" + candidate.RiskTier.ToString(CultureInfo.InvariantCulture),
        "candidate.requiredFitness=" + candidate.RequiredFitnessToOperate.Trim(),
        "candidate.correlation=" + candidate.Correlation.Trim(),
        "candidate.evidence=" + candidate.EvidenceReference.Trim(),
        "candidate.requestTime=" + candidate.RequestTime.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture),
        "candidate.expiry=" + candidate.Expiry.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)));

    private static StandingOwnerPreapprovalDecision Deny(
        string reason,
        StandingOwnerPreapprovalProfile? profile,
        StandingOwnerPreapprovalCandidate? candidate,
        DateTimeOffset observationTime,
        string authorityDecisionId = "NONE",
        DateTimeOffset? authorityExpiry = null)
    {
        var profileIdentity = profile?.IdentitySha256 ?? "NONE";
        var candidateIdentity = candidate is null || !ValidCandidate(candidate) ? "NONE" : CandidateIdentity(candidate);
        var expiry = authorityExpiry ?? profile?.Expiry ?? observationTime;
        if (expiry <= observationTime) expiry = observationTime.AddTicks(1);
        var decisionIdentity = ComputeSha256(string.Join("\n",
            "result=DENIED",
            "reason=" + reason,
            "profile=" + profileIdentity,
            "candidate=" + candidateIdentity,
            "authorityDecision=" + authorityDecisionId,
            "observation=" + observationTime.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)));

        return new StandingOwnerPreapprovalDecision(
            false,
            reason,
            decisionIdentity,
            profileIdentity,
            candidateIdentity,
            authorityDecisionId,
            false,
            false,
            false,
            observationTime,
            expiry,
            profile?.EvidenceReference ?? "NONE");
    }

    private static bool Contains(IEnumerable<string> values, string candidate) =>
        !string.IsNullOrWhiteSpace(candidate) && values.Any(value => StringComparer.Ordinal.Equals(value, candidate.Trim()));

    private static bool ScopeAllowed(IEnumerable<string> scopes, string requestedScope)
    {
        if (string.IsNullOrWhiteSpace(requestedScope)) return false;
        var requested = requestedScope.Trim();
        return scopes.Any(scope => scope == "*" || StringComparer.Ordinal.Equals(scope, requested) || requested.StartsWith(scope + ":", StringComparison.Ordinal));
    }

    private static bool Required(string value) => !string.IsNullOrWhiteSpace(value);

    private static bool ValidSha256(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;
        var normalized = value.Trim();
        if (normalized.StartsWith("sha256/", StringComparison.OrdinalIgnoreCase)) normalized = normalized[7..];
        return normalized.Length == 64 && normalized.All(character => Uri.IsHexDigit(character));
    }
}
