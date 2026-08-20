using System;
using System.Globalization;
using System.Linq;
using Foundation.Contracts;

namespace Foundation.Authority;

public static class WebOwnerStandingPreapprovalBoundary
{
    public const string CanonicalDecisionSurfaceIdentity = "shared-web:owner-command-center";
}

public static class WebOwnerPreapprovalReason
{
    public const string Accepted = "WEB_OWNER_DERIVED_AUTO_ACCEPT_ACCEPTED";
    public const string InvalidProposal = "WEB_OWNER_PREAPPROVAL_PROPOSAL_INVALID";
    public const string ProposalNotCurrent = "WEB_OWNER_PREAPPROVAL_PROPOSAL_NOT_CURRENT";
    public const string WrongDecisionSurface = "WEB_OWNER_PREAPPROVAL_WRONG_DECISION_SURFACE";
    public const string ProducerSelfApprovalForbidden = "WEB_OWNER_PREAPPROVAL_PRODUCER_SELF_APPROVAL_FORBIDDEN";
    public const string BackupPlanRequired = "WEB_OWNER_PREAPPROVAL_BACKUP_ROLLBACK_PLAN_REQUIRED";
    public const string BackupPlanInvalid = "WEB_OWNER_PREAPPROVAL_BACKUP_ROLLBACK_PLAN_INVALID";
    public const string BackupPlanScopeMismatch = "WEB_OWNER_PREAPPROVAL_BACKUP_ROLLBACK_PLAN_SCOPE_MISMATCH";
    public const string BackupPlanNotCurrent = "WEB_OWNER_PREAPPROVAL_BACKUP_ROLLBACK_PLAN_NOT_CURRENT";
    public const string RegisteredPolicyDenied = "WEB_OWNER_PREAPPROVAL_REGISTERED_POLICY_DENIED";
}

public sealed record GovernedBackupRollbackPlan
{
    public GovernedBackupRollbackPlan(
        string planId,
        string planVersion,
        string planSha256,
        string preChangeStateIdentitySha256,
        string affectedApplicationIdentity,
        string affectedScope,
        string validationEvidenceReference,
        string rollbackPreconditions,
        string recoveryConstraints,
        string expectedRollbackOutcome,
        DateTimeOffset effectiveFrom,
        DateTimeOffset expiry,
        bool isSuperseded,
        string evidenceReference)
    {
        PlanId = Require(planId, nameof(planId));
        PlanVersion = Require(planVersion, nameof(planVersion));
        PlanSha256 = RequireSha256(planSha256, nameof(planSha256));
        PreChangeStateIdentitySha256 = RequireSha256(preChangeStateIdentitySha256, nameof(preChangeStateIdentitySha256));
        AffectedApplicationIdentity = Require(affectedApplicationIdentity, nameof(affectedApplicationIdentity));
        AffectedScope = Require(affectedScope, nameof(affectedScope));
        ValidationEvidenceReference = Require(validationEvidenceReference, nameof(validationEvidenceReference));
        RollbackPreconditions = Require(rollbackPreconditions, nameof(rollbackPreconditions));
        RecoveryConstraints = Require(recoveryConstraints, nameof(recoveryConstraints));
        ExpectedRollbackOutcome = Require(expectedRollbackOutcome, nameof(expectedRollbackOutcome));
        if (effectiveFrom == default || expiry <= effectiveFrom) throw new ArgumentException("Invalid rollback-plan validity window.");
        EffectiveFrom = effectiveFrom;
        Expiry = expiry;
        IsSuperseded = isSuperseded;
        EvidenceReference = Require(evidenceReference, nameof(evidenceReference));
        IdentitySha256 = StandingOwnerPreapprovalEvaluator.ComputeSha256(string.Join("\n",
            "rollbackPlan.id=" + PlanId,
            "rollbackPlan.version=" + PlanVersion,
            "rollbackPlan.digest=" + PlanSha256,
            "rollbackPlan.preChangeState=" + PreChangeStateIdentitySha256,
            "rollbackPlan.application=" + AffectedApplicationIdentity,
            "rollbackPlan.scope=" + AffectedScope,
            "rollbackPlan.validationEvidence=" + ValidationEvidenceReference,
            "rollbackPlan.preconditions=" + RollbackPreconditions,
            "rollbackPlan.recoveryConstraints=" + RecoveryConstraints,
            "rollbackPlan.expectedOutcome=" + ExpectedRollbackOutcome,
            "rollbackPlan.effectiveFrom=" + EffectiveFrom.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture),
            "rollbackPlan.expiry=" + Expiry.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture),
            "rollbackPlan.superseded=" + (IsSuperseded ? "true" : "false"),
            "rollbackPlan.evidence=" + EvidenceReference));
    }

    public string PlanId { get; }
    public string PlanVersion { get; }
    public string PlanSha256 { get; }
    public string PreChangeStateIdentitySha256 { get; }
    public string AffectedApplicationIdentity { get; }
    public string AffectedScope { get; }
    public string ValidationEvidenceReference { get; }
    public string RollbackPreconditions { get; }
    public string RecoveryConstraints { get; }
    public string ExpectedRollbackOutcome { get; }
    public DateTimeOffset EffectiveFrom { get; }
    public DateTimeOffset Expiry { get; }
    public bool IsSuperseded { get; }
    public string EvidenceReference { get; }
    public string IdentitySha256 { get; }

    private static string Require(string value, string name) =>
        string.IsNullOrWhiteSpace(value) ? throw new ArgumentException("Value is required.", name) : value.Trim();

    private static string RequireSha256(string value, string name)
    {
        var normalized = Require(value, name);
        var digest = normalized.StartsWith("sha256/", StringComparison.OrdinalIgnoreCase) ? normalized[7..] : normalized;
        if (digest.Length != 64 || !digest.All(Uri.IsHexDigit)) throw new ArgumentException("Valid SHA-256 identity is required.", name);
        return "sha256/" + digest.ToUpperInvariant();
    }
}

public sealed record WebOwnerPreapprovalProposal(
    string ProposalId,
    string CandidateId,
    string CandidateVersion,
    string CandidateSha256,
    string ProposalProducerIdentity,
    string OwningApplicationIdentity,
    string DecisionSurfaceIdentity,
    string UpdateClass,
    string Resource,
    string Purpose,
    string RequestedScope,
    string Environment,
    string SecurityContext,
    int RiskTier,
    string RequiredFitnessToOperate,
    string Correlation,
    string PolicyEvidenceReference,
    GovernedBackupRollbackPlan? BackupRollbackPlan,
    bool ProducerClaimsAutoAccept,
    bool ProducerClaimsRollbackAuthority,
    DateTimeOffset RequestTime,
    DateTimeOffset Expiry);

public sealed record WebOwnerDerivedAutoAcceptDecision(
    bool AcceptedUnderStandingOwnerPolicy,
    string Reason,
    string DecisionIdentitySha256,
    string ProposalIdentitySha256,
    string BackupRollbackPlanIdentitySha256,
    string RegistrationIdentitySha256,
    string UnderlyingAuthorityDecisionId,
    bool ExecutionAuthorized,
    bool DeploymentAuthorized,
    bool BusinessAuthorityGranted,
    DateTimeOffset DecisionTime,
    DateTimeOffset Expiry,
    string EvidenceReference);

public sealed class WebOwnerStandingPreapprovalEvaluator
{
    private readonly RegisteredStandingOwnerPreapprovalEvaluator _registeredEvaluator;

    public WebOwnerStandingPreapprovalEvaluator(StandingOwnerPreapprovalRegistry registry)
    {
        _registeredEvaluator = new RegisteredStandingOwnerPreapprovalEvaluator(registry ?? throw new ArgumentNullException(nameof(registry)));
    }

    public WebOwnerDerivedAutoAcceptDecision Evaluate(
        string policyId,
        WebOwnerPreapprovalProposal? proposal,
        FitnessEvidence? decisionSurfaceFitness,
        DateTimeOffset observationTime)
    {
        if (!ValidProposal(proposal) || observationTime == default || observationTime.Offset != TimeSpan.Zero)
            return Deny(WebOwnerPreapprovalReason.InvalidProposal, proposal, observationTime);
        var p = proposal!;
        if (p.RequestTime.Offset != TimeSpan.Zero || p.Expiry.Offset != TimeSpan.Zero ||
            observationTime < p.RequestTime || observationTime >= p.Expiry)
            return Deny(WebOwnerPreapprovalReason.ProposalNotCurrent, p, observationTime);
        if (!StringComparer.Ordinal.Equals(p.DecisionSurfaceIdentity.Trim(), WebOwnerStandingPreapprovalBoundary.CanonicalDecisionSurfaceIdentity))
            return Deny(WebOwnerPreapprovalReason.WrongDecisionSurface, p, observationTime);
        if (p.ProducerClaimsAutoAccept || p.ProducerClaimsRollbackAuthority)
            return Deny(WebOwnerPreapprovalReason.ProducerSelfApprovalForbidden, p, observationTime);
        if (p.BackupRollbackPlan is null)
            return Deny(WebOwnerPreapprovalReason.BackupPlanRequired, p, observationTime);

        var plan = p.BackupRollbackPlan;
        if (plan.IsSuperseded || string.IsNullOrWhiteSpace(plan.ValidationEvidenceReference) || string.IsNullOrWhiteSpace(plan.EvidenceReference))
            return Deny(WebOwnerPreapprovalReason.BackupPlanInvalid, p, observationTime);
        if (!StringComparer.Ordinal.Equals(plan.AffectedApplicationIdentity, p.OwningApplicationIdentity.Trim()) ||
            !ScopeContains(plan.AffectedScope, p.RequestedScope))
            return Deny(WebOwnerPreapprovalReason.BackupPlanScopeMismatch, p, observationTime);
        if (observationTime < plan.EffectiveFrom || observationTime >= plan.Expiry || p.RequestTime < plan.EffectiveFrom || p.Expiry > plan.Expiry)
            return Deny(WebOwnerPreapprovalReason.BackupPlanNotCurrent, p, observationTime);

        var candidate = new StandingOwnerPreapprovalCandidate(
            p.CandidateId.Trim(), p.CandidateVersion.Trim(), p.CandidateSha256.Trim(),
            WebOwnerStandingPreapprovalBoundary.CanonicalDecisionSurfaceIdentity,
            p.OwningApplicationIdentity.Trim(), p.UpdateClass.Trim(), p.Resource.Trim(), p.Purpose.Trim(),
            p.RequestedScope.Trim(), p.Environment.Trim(), p.SecurityContext.Trim(), p.RiskTier,
            p.RequiredFitnessToOperate.Trim(), p.Correlation.Trim(), p.PolicyEvidenceReference.Trim(), p.RequestTime, p.Expiry);

        var registered = _registeredEvaluator.Evaluate(policyId, candidate, decisionSurfaceFitness, observationTime);
        if (!registered.AcceptedUnderStandingPreapproval)
            return Deny(WebOwnerPreapprovalReason.RegisteredPolicyDenied + ":" + registered.Reason, p, observationTime, registered);

        var proposalIdentity = ProposalIdentity(p);
        var expiry = registered.Expiry < plan.Expiry ? registered.Expiry : plan.Expiry;
        var decisionIdentity = StandingOwnerPreapprovalEvaluator.ComputeSha256(string.Join("\n",
            "webOwnerDecision=ACCEPTED_UNDER_STANDING_POLICY",
            "decisionSurface=" + WebOwnerStandingPreapprovalBoundary.CanonicalDecisionSurfaceIdentity,
            "producer=" + p.ProposalProducerIdentity.Trim(),
            "proposal=" + proposalIdentity,
            "backupRollbackPlan=" + plan.IdentitySha256,
            "registeredDecision=" + registered.DecisionIdentitySha256,
            "observation=" + observationTime.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture),
            "expiry=" + expiry.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture),
            "executionAuthorized=false",
            "deploymentAuthorized=false",
            "businessAuthorityGranted=false"));

        return new WebOwnerDerivedAutoAcceptDecision(
            true, WebOwnerPreapprovalReason.Accepted, decisionIdentity, proposalIdentity, plan.IdentitySha256,
            registered.RegistrationIdentitySha256, registered.UnderlyingAuthorityDecisionId,
            false, false, false, observationTime, expiry, p.PolicyEvidenceReference.Trim());
    }

    private static bool ValidProposal(WebOwnerPreapprovalProposal? proposal) =>
        proposal is not null &&
        Required(proposal.ProposalId) && Required(proposal.CandidateId) && Required(proposal.CandidateVersion) && ValidSha256(proposal.CandidateSha256) &&
        Required(proposal.ProposalProducerIdentity) && Required(proposal.OwningApplicationIdentity) && Required(proposal.DecisionSurfaceIdentity) &&
        Required(proposal.UpdateClass) && Required(proposal.Resource) && Required(proposal.Purpose) && Required(proposal.RequestedScope) &&
        Required(proposal.Environment) && Required(proposal.SecurityContext) && proposal.RiskTier >= 0 &&
        Required(proposal.RequiredFitnessToOperate) && Required(proposal.Correlation) && Required(proposal.PolicyEvidenceReference) &&
        proposal.RequestTime != default && proposal.Expiry > proposal.RequestTime;

    private static string ProposalIdentity(WebOwnerPreapprovalProposal proposal) => StandingOwnerPreapprovalEvaluator.ComputeSha256(string.Join("\n",
        "proposal.id=" + proposal.ProposalId.Trim(),
        "proposal.candidateId=" + proposal.CandidateId.Trim(),
        "proposal.candidateVersion=" + proposal.CandidateVersion.Trim(),
        "proposal.candidateDigest=" + NormalizeSha256(proposal.CandidateSha256),
        "proposal.producer=" + proposal.ProposalProducerIdentity.Trim(),
        "proposal.application=" + proposal.OwningApplicationIdentity.Trim(),
        "proposal.decisionSurface=" + proposal.DecisionSurfaceIdentity.Trim(),
        "proposal.updateClass=" + proposal.UpdateClass.Trim(),
        "proposal.resource=" + proposal.Resource.Trim(),
        "proposal.purpose=" + proposal.Purpose.Trim(),
        "proposal.scope=" + proposal.RequestedScope.Trim(),
        "proposal.environment=" + proposal.Environment.Trim(),
        "proposal.securityContext=" + proposal.SecurityContext.Trim(),
        "proposal.riskTier=" + proposal.RiskTier.ToString(CultureInfo.InvariantCulture),
        "proposal.correlation=" + proposal.Correlation.Trim(),
        "proposal.policyEvidence=" + proposal.PolicyEvidenceReference.Trim(),
        "proposal.rollbackPlan=" + proposal.BackupRollbackPlan!.IdentitySha256,
        "proposal.producerClaimsAutoAccept=" + (proposal.ProducerClaimsAutoAccept ? "true" : "false"),
        "proposal.producerClaimsRollbackAuthority=" + (proposal.ProducerClaimsRollbackAuthority ? "true" : "false"),
        "proposal.requestTime=" + proposal.RequestTime.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture),
        "proposal.expiry=" + proposal.Expiry.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)));

    private static WebOwnerDerivedAutoAcceptDecision Deny(
        string reason, WebOwnerPreapprovalProposal? proposal, DateTimeOffset observationTime,
        RegisteredStandingOwnerPreapprovalDecision? registered = null)
    {
        var safeObservation = observationTime == default ? DateTimeOffset.UnixEpoch : observationTime;
        var proposalIdentity = proposal is null || !ValidProposal(proposal) || proposal.BackupRollbackPlan is null ? "NONE" : ProposalIdentity(proposal);
        var planIdentity = proposal?.BackupRollbackPlan?.IdentitySha256 ?? "NONE";
        var expiry = registered?.Expiry ?? proposal?.Expiry ?? safeObservation.AddTicks(1);
        if (expiry <= safeObservation) expiry = safeObservation.AddTicks(1);
        var decisionIdentity = StandingOwnerPreapprovalEvaluator.ComputeSha256(string.Join("\n",
            "webOwnerDecision=DENIED", "reason=" + reason, "proposal=" + proposalIdentity,
            "backupRollbackPlan=" + planIdentity, "registeredDecision=" + (registered?.DecisionIdentitySha256 ?? "NONE"),
            "observation=" + safeObservation.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)));
        return new WebOwnerDerivedAutoAcceptDecision(
            false, reason, decisionIdentity, proposalIdentity, planIdentity,
            registered?.RegistrationIdentitySha256 ?? "NONE", registered?.UnderlyingAuthorityDecisionId ?? "NONE",
            false, false, false, safeObservation, expiry, proposal?.PolicyEvidenceReference ?? "NONE");
    }

    private static bool ScopeContains(string authorizedScope, string requestedScope)
    {
        var authorized = authorizedScope.Trim();
        var requested = requestedScope.Trim();
        return authorized == "*" || StringComparer.Ordinal.Equals(authorized, requested) || requested.StartsWith(authorized + ":", StringComparison.Ordinal);
    }

    private static bool Required(string value) => !string.IsNullOrWhiteSpace(value);
    private static bool ValidSha256(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;
        var digest = value.Trim();
        if (digest.StartsWith("sha256/", StringComparison.OrdinalIgnoreCase)) digest = digest[7..];
        return digest.Length == 64 && digest.All(Uri.IsHexDigit);
    }
    private static string NormalizeSha256(string value)
    {
        var digest = value.Trim();
        if (digest.StartsWith("sha256/", StringComparison.OrdinalIgnoreCase)) digest = digest[7..];
        return "sha256/" + digest.ToUpperInvariant();
    }
}

public enum OwnerRollbackOrderState { Requested = 0, Accepted = 1, Rejected = 2 }
public enum RollbackExecutionState { NotStarted = 0, InProgress = 1, Completed = 2, Failed = 3 }

public sealed record OwnerRollbackOrderRequest(
    string RollbackOrderId,
    string OwnerIdentity,
    string DecisionSurfaceIdentity,
    string OriginalAutoAcceptDecisionIdentitySha256,
    string ProposalIdentitySha256,
    GovernedBackupRollbackPlan BackupRollbackPlan,
    string TargetApplicationIdentity,
    string TargetScope,
    bool StepUpAuthenticationSatisfied,
    bool MfaSatisfied,
    string OwnerAuthenticationEvidenceReference,
    bool TargetAdmissionCurrent,
    string TargetAdmissionEvidenceReference,
    bool SafetyReadinessSatisfied,
    string SafetyReadinessEvidenceReference,
    AuthorityResult FreshRollbackAuthorityDecision,
    string Correlation,
    string EvidenceReference,
    DateTimeOffset RequestedAt,
    DateTimeOffset Expiry);

public sealed record OwnerRollbackOrderDecision(
    OwnerRollbackOrderState State,
    string Reason,
    string DecisionIdentitySha256,
    bool RollbackAuthorized,
    bool RollbackExecuted,
    bool AuthorityRestored,
    bool TrustRestored,
    DateTimeOffset DecisionTime,
    DateTimeOffset Expiry,
    string EvidenceReference);

public sealed class OwnerRollbackOrderEvaluator
{
    public OwnerRollbackOrderDecision Evaluate(OwnerRollbackOrderRequest? request, DateTimeOffset observationTime)
    {
        if (!ValidRequest(request) || observationTime == default || observationTime.Offset != TimeSpan.Zero)
            return Deny("ROLLBACK_ORDER_INVALID", request, observationTime);
        var r = request!;
        if (r.RequestedAt.Offset != TimeSpan.Zero || r.Expiry.Offset != TimeSpan.Zero ||
            observationTime < r.RequestedAt || observationTime >= r.Expiry)
            return Deny("ROLLBACK_ORDER_REQUEST_NOT_CURRENT", r, observationTime);
        if (!StringComparer.Ordinal.Equals(r.DecisionSurfaceIdentity.Trim(), WebOwnerStandingPreapprovalBoundary.CanonicalDecisionSurfaceIdentity))
            return Deny("ROLLBACK_ORDER_WRONG_DECISION_SURFACE", r, observationTime);
        if (!r.StepUpAuthenticationSatisfied || !r.MfaSatisfied || string.IsNullOrWhiteSpace(r.OwnerAuthenticationEvidenceReference))
            return Deny("ROLLBACK_ORDER_STEP_UP_AUTHENTICATION_REQUIRED", r, observationTime);
        if (!r.TargetAdmissionCurrent || string.IsNullOrWhiteSpace(r.TargetAdmissionEvidenceReference))
            return Deny("ROLLBACK_ORDER_TARGET_ADMISSION_NOT_CURRENT", r, observationTime);
        if (!r.SafetyReadinessSatisfied || string.IsNullOrWhiteSpace(r.SafetyReadinessEvidenceReference))
            return Deny("ROLLBACK_ORDER_SAFETY_READINESS_REJECTED", r, observationTime);
        if (r.BackupRollbackPlan.IsSuperseded || observationTime < r.BackupRollbackPlan.EffectiveFrom || observationTime >= r.BackupRollbackPlan.Expiry)
            return Deny("ROLLBACK_ORDER_PLAN_NOT_CURRENT", r, observationTime);
        if (!StringComparer.Ordinal.Equals(r.BackupRollbackPlan.AffectedApplicationIdentity, r.TargetApplicationIdentity.Trim()) ||
            !ScopeContains(r.BackupRollbackPlan.AffectedScope, r.TargetScope))
            return Deny("ROLLBACK_ORDER_PLAN_SCOPE_MISMATCH", r, observationTime);

        var authority = r.FreshRollbackAuthorityDecision;
        if (!StringComparer.Ordinal.Equals(authority.Decision, AuthorityDecision.Allow) ||
            authority.DecisionTime > observationTime || authority.Expiry <= observationTime ||
            !StringComparer.Ordinal.Equals(authority.RequestId, r.RollbackOrderId.Trim()) ||
            !ScopeContains(authority.EffectiveScope, r.TargetScope) ||
            string.IsNullOrWhiteSpace(authority.EvidenceReference) ||
            !authority.DecisionId.StartsWith("authority-decision/sha256/", StringComparison.Ordinal))
            return Deny("ROLLBACK_ORDER_FRESH_AUTHORITY_REQUIRED", r, observationTime);

        var expiry = authority.Expiry < r.Expiry ? authority.Expiry : r.Expiry;
        if (r.BackupRollbackPlan.Expiry < expiry) expiry = r.BackupRollbackPlan.Expiry;
        var decisionIdentity = StandingOwnerPreapprovalEvaluator.ComputeSha256(string.Join("\n",
            "rollbackOrder=ACCEPTED",
            "order=" + r.RollbackOrderId.Trim(),
            "owner=" + r.OwnerIdentity.Trim(),
            "surface=" + r.DecisionSurfaceIdentity.Trim(),
            "ownerAuthenticationEvidence=" + r.OwnerAuthenticationEvidenceReference.Trim(),
            "autoAcceptDecision=" + r.OriginalAutoAcceptDecisionIdentitySha256.Trim(),
            "proposal=" + r.ProposalIdentitySha256.Trim(),
            "plan=" + r.BackupRollbackPlan.IdentitySha256,
            "targetApplication=" + r.TargetApplicationIdentity.Trim(),
            "targetScope=" + r.TargetScope.Trim(),
            "targetAdmissionEvidence=" + r.TargetAdmissionEvidenceReference.Trim(),
            "safetyReadinessEvidence=" + r.SafetyReadinessEvidenceReference.Trim(),
            "authorityDecision=" + authority.DecisionId,
            "observation=" + observationTime.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture),
            "rollbackExecuted=false", "authorityRestored=false", "trustRestored=false"));

        return new OwnerRollbackOrderDecision(
            OwnerRollbackOrderState.Accepted, "ROLLBACK_ORDER_ACCEPTED_FOR_SEPARATE_EXECUTION", decisionIdentity,
            true, false, false, false, observationTime, expiry, r.EvidenceReference.Trim());
    }

    private static OwnerRollbackOrderDecision Deny(string reason, OwnerRollbackOrderRequest? request, DateTimeOffset observationTime)
    {
        var safeObservation = observationTime == default ? DateTimeOffset.UnixEpoch : observationTime;
        var expiry = request?.Expiry ?? safeObservation.AddTicks(1);
        if (expiry <= safeObservation) expiry = safeObservation.AddTicks(1);
        return new OwnerRollbackOrderDecision(
            OwnerRollbackOrderState.Rejected, reason,
            StandingOwnerPreapprovalEvaluator.ComputeSha256(string.Join("\n", "rollbackOrder=REJECTED", "reason=" + reason,
                "order=" + (request?.RollbackOrderId?.Trim() ?? "NONE"),
                "observation=" + safeObservation.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture))),
            false, false, false, false, safeObservation, expiry, request?.EvidenceReference ?? "NONE");
    }

    private static bool ValidRequest(OwnerRollbackOrderRequest? request) =>
        request is not null &&
        Required(request.RollbackOrderId) && Required(request.OwnerIdentity) && Required(request.DecisionSurfaceIdentity) &&
        ValidSha256(request.OriginalAutoAcceptDecisionIdentitySha256) && ValidSha256(request.ProposalIdentitySha256) &&
        request.BackupRollbackPlan is not null && Required(request.TargetApplicationIdentity) && Required(request.TargetScope) &&
        Required(request.OwnerAuthenticationEvidenceReference) && Required(request.TargetAdmissionEvidenceReference) && Required(request.SafetyReadinessEvidenceReference) &&
        request.FreshRollbackAuthorityDecision is not null && Required(request.Correlation) && Required(request.EvidenceReference) &&
        request.RequestedAt != default && request.Expiry > request.RequestedAt;

    private static bool ScopeContains(string authorizedScope, string requestedScope)
    {
        var authorized = authorizedScope.Trim();
        var requested = requestedScope.Trim();
        return authorized == "*" || StringComparer.Ordinal.Equals(authorized, requested) || requested.StartsWith(authorized + ":", StringComparison.Ordinal);
    }
    private static bool Required(string value) => !string.IsNullOrWhiteSpace(value);
    private static bool ValidSha256(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;
        var digest = value.Trim();
        if (digest.StartsWith("sha256/", StringComparison.OrdinalIgnoreCase)) digest = digest[7..];
        return digest.Length == 64 && digest.All(Uri.IsHexDigit);
    }
}

public sealed record RollbackStatusProjection(
    string RollbackOrderDecisionIdentitySha256,
    RollbackExecutionState ExecutionState,
    string ExecutorIdentity,
    string ResultEvidenceReference,
    DateTimeOffset ObservedAt,
    bool AuthorityRestored,
    bool TrustRestored,
    bool CredentialsRestored,
    bool LiveTradingAuthorityRestored,
    bool KillReleaseRevivalAuthorityRestored)
{
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(RollbackOrderDecisionIdentitySha256) || string.IsNullOrWhiteSpace(ExecutorIdentity) ||
            string.IsNullOrWhiteSpace(ResultEvidenceReference) || ObservedAt == default || !Enum.IsDefined(ExecutionState))
            throw new InvalidOperationException("ROLLBACK_STATUS_PROJECTION_INVALID");
        if (AuthorityRestored || TrustRestored || CredentialsRestored || LiveTradingAuthorityRestored || KillReleaseRevivalAuthorityRestored)
            throw new InvalidOperationException("ROLLBACK_STATUS_CANNOT_SILENTLY_RESTORE_SEPARATE_AUTHORITY_OR_TRUST");
    }
}
