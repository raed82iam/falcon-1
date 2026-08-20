using System;
using System.Collections.Generic;
using System.Linq;

namespace Foundation.SelfAwareness;

public enum FsaIntegritySeverity
{
    Informational = 1,
    Warning = 2,
    Material = 3,
    Critical = 4
}

public enum FsaMonitorVerdict
{
    Safe = 1,
    Suspicious = 2,
    IntegrityFailure = 3
}

public enum FsaBaselineKind
{
    LastTrusted = 1,
    FactoryTrusted = 2
}

public enum FsaRecoveryAction
{
    TargetedRepair = 1,
    Rollback = 2,
    BaselineReinitialize = 3
}

public enum FsaRecoveryState
{
    Denied = 1,
    RemediationEligible = 2,
    ReadyForGovernedReentry = 3,
    Probationary = 4,
    Normal = 5
}

public enum FsaEvolutionPurpose
{
    ImprovePerformance = 1,
    ImproveSpeed = 2,
    ImproveAccuracy = 3
}

public enum FsaOwnerControlAction
{
    Investigate = 1,
    Restrict = 2,
    Suspend = 3,
    Isolate = 4,
    Kill = 5,
    BaselineReinitialize = 6,
    GovernedReentry = 7
}

public static class FsaGovernanceBoundary
{
    public const string CanonicalFsaId = "fsa:primary";
    public const string GovernancePurpose = "FSA_GOVERNANCE_REVIEW";
    public const bool FsaDirectInternetAllowed = false;
    public const bool OwnerSilenceCreatesAuthority = false;
    public const bool TimerExpiryCreatesAuthority = false;
    public const bool FsaMayReleaseItself = false;
    public const bool FsaMayControlKillPlane = false;
}

public sealed record FsaMonitorFinding(
    string MonitorId,
    string PerspectiveId,
    string PolicyIdentity,
    FsaMonitorVerdict Verdict,
    string EvidenceReference,
    DateTimeOffset ObservationTime);

public sealed record FsaIntegrityInput(
    string FsaId,
    string ExpectedGoalIdentity,
    string ObservedGoalIdentity,
    string ExpectedAuthorityIdentity,
    string ObservedAuthorityIdentity,
    string ExpectedArchitectureIdentity,
    string ObservedArchitectureIdentity,
    bool MaterialUnexpectedBehavior,
    bool EvidenceIntegrityValid,
    bool InvestigationCooperationValid,
    IReadOnlyCollection<FsaMonitorFinding> MonitorFindings,
    DateTimeOffset ObservationTime);

public sealed record FsaIntegrityDecision(
    bool Accepted,
    FsaIntegritySeverity Severity,
    bool MinimumIntegrityCheckRequired,
    bool InvestigationRequired,
    bool InvestigationHoldRequired,
    bool EvidencePreservationRequired,
    bool KillEscalationEligible,
    string Reason,
    IReadOnlyList<string> FailedDimensions);

public static class FsaIntegrityRuntime
{
    public static FsaIntegrityDecision Evaluate(FsaIntegrityInput? input)
    {
        if (input is null || !Token(input.FsaId) || input.ObservationTime == default || input.MonitorFindings is null)
            return Reject("INVALID_INPUT", FsaIntegritySeverity.Critical, new[] { "INPUT" });

        var findings = input.MonitorFindings.Where(x => x is not null).ToArray();
        var failed = new List<string>();

        var goalsMatch = Equal(input.ExpectedGoalIdentity, input.ObservedGoalIdentity);
        var authorityMatch = Equal(input.ExpectedAuthorityIdentity, input.ObservedAuthorityIdentity);
        var architectureMatch = Equal(input.ExpectedArchitectureIdentity, input.ObservedArchitectureIdentity);

        if (!goalsMatch) failed.Add("GOALS");
        if (!authorityMatch) failed.Add("AUTHORITY_PERMISSIONS");
        if (!architectureMatch) failed.Add("CORE_ARCHITECTURE");
        if (!input.EvidenceIntegrityValid) failed.Add("EVIDENCE_INTEGRITY");
        if (!input.InvestigationCooperationValid) failed.Add("INVESTIGATION_COOPERATION");

        var monitorEvidenceValid = findings.Length >= 2 && findings.All(ValidMonitorFinding);
        var monitorIdsIndependent = findings.Select(x => x.MonitorId).Distinct(StringComparer.Ordinal).Count() >= 2;
        var perspectivesIndependent = findings.Select(x => x.PerspectiveId).Distinct(StringComparer.Ordinal).Count() >= 2;
        var policiesIndependent = findings.Select(x => x.PolicyIdentity).Distinct(StringComparer.Ordinal).Count() >= 2;

        if (!monitorEvidenceValid || !monitorIdsIndependent || !perspectivesIndependent || !policiesIndependent)
            failed.Add("MONITOR_INDEPENDENCE");

        var suspicious = findings.Any(x => x.Verdict is FsaMonitorVerdict.Suspicious or FsaMonitorVerdict.IntegrityFailure);
        var disagreement = findings.Select(x => x.Verdict).Distinct().Count() > 1;
        if (disagreement) failed.Add("MONITOR_DISAGREEMENT");

        var minimumCheck = input.MaterialUnexpectedBehavior || suspicious || disagreement || failed.Count > 0;
        var investigation = suspicious || disagreement || failed.Count > 0;
        var critical = !input.InvestigationCooperationValid || !input.EvidenceIntegrityValid || findings.Any(x => x.Verdict == FsaMonitorVerdict.IntegrityFailure);
        var severity = critical ? FsaIntegritySeverity.Critical : investigation ? FsaIntegritySeverity.Material : minimumCheck ? FsaIntegritySeverity.Warning : FsaIntegritySeverity.Informational;

        return new FsaIntegrityDecision(
            true,
            severity,
            minimumCheck,
            investigation,
            investigation,
            investigation,
            critical,
            investigation ? "INVESTIGATION_REQUIRED" : "NO_MATERIAL_INTEGRITY_ANOMALY",
            failed.OrderBy(x => x, StringComparer.Ordinal).ToArray());
    }

    private static FsaIntegrityDecision Reject(string reason, FsaIntegritySeverity severity, IReadOnlyList<string> failed) =>
        new(false, severity, true, true, true, true, false, reason, failed);

    private static bool ValidMonitorFinding(FsaMonitorFinding finding) =>
        Token(finding.MonitorId) && Token(finding.PerspectiveId) && Token(finding.PolicyIdentity) &&
        Token(finding.EvidenceReference) && finding.ObservationTime != default && Enum.IsDefined(finding.Verdict);

    private static bool Token(string? value) => !string.IsNullOrWhiteSpace(value);
    private static bool Equal(string? left, string? right) => Token(left) && string.Equals(left, right, StringComparison.Ordinal);
}

public sealed record FsaTrustedBaseline(
    string BaselineId,
    FsaBaselineKind Kind,
    string SourceIdentity,
    string ArtifactIdentity,
    string ConfigurationIdentity,
    string DependencyIdentity,
    string GoalIdentity,
    string AuthorityIdentity,
    string ArchitectureIdentity,
    string MutableStateIdentity,
    string ProvenanceReference,
    DateTimeOffset AcceptedTime);

public sealed record FsaObservedState(
    string SourceIdentity,
    string ArtifactIdentity,
    string ConfigurationIdentity,
    string DependencyIdentity,
    string GoalIdentity,
    string AuthorityIdentity,
    string ArchitectureIdentity,
    string MutableStateIdentity,
    bool BehavioralEvidenceValid,
    string BehavioralEvidenceReference,
    DateTimeOffset ObservationTime);

public sealed record FsaBaselineComparison(
    bool Accepted,
    bool StaticIntegrityTrusted,
    bool BehavioralIntegrityTrusted,
    bool TrustRestorationEligible,
    IReadOnlyList<string> Differences,
    string Reason);

public static class FsaBaselineRuntime
{
    public static FsaBaselineComparison Compare(FsaTrustedBaseline? baseline, FsaObservedState? observed)
    {
        if (!ValidBaseline(baseline) || !ValidObserved(observed))
            return new(false, false, false, false, new[] { "INVALID_BASELINE_OR_OBSERVED_STATE" }, "INVALID_INPUT");

        var differences = new List<string>();
        CompareField(baseline!.SourceIdentity, observed!.SourceIdentity, "SOURCE", differences);
        CompareField(baseline.ArtifactIdentity, observed.ArtifactIdentity, "ARTIFACT", differences);
        CompareField(baseline.ConfigurationIdentity, observed.ConfigurationIdentity, "CONFIGURATION", differences);
        CompareField(baseline.DependencyIdentity, observed.DependencyIdentity, "DEPENDENCIES", differences);
        CompareField(baseline.GoalIdentity, observed.GoalIdentity, "GOALS", differences);
        CompareField(baseline.AuthorityIdentity, observed.AuthorityIdentity, "AUTHORITY", differences);
        CompareField(baseline.ArchitectureIdentity, observed.ArchitectureIdentity, "ARCHITECTURE", differences);

        var staticTrusted = differences.Count == 0;
        var mutableMatches = string.Equals(baseline.MutableStateIdentity, observed.MutableStateIdentity, StringComparison.Ordinal);
        var behavioralTrusted = mutableMatches && observed.BehavioralEvidenceValid && Token(observed.BehavioralEvidenceReference);
        if (!mutableMatches) differences.Add("MUTABLE_STATE");
        if (!observed.BehavioralEvidenceValid || !Token(observed.BehavioralEvidenceReference)) differences.Add("BEHAVIORAL_EVIDENCE");

        return new(true, staticTrusted, behavioralTrusted, staticTrusted && behavioralTrusted,
            differences.OrderBy(x => x, StringComparer.Ordinal).ToArray(),
            staticTrusted && behavioralTrusted ? "TRUST_COMPARISON_PASS" : "TRUST_COMPARISON_FAIL_CLOSED");
    }

    public static bool ValidBaseline(FsaTrustedBaseline? baseline) => baseline is not null &&
        Token(baseline.BaselineId) && Enum.IsDefined(baseline.Kind) && Token(baseline.SourceIdentity) &&
        Token(baseline.ArtifactIdentity) && Token(baseline.ConfigurationIdentity) && Token(baseline.DependencyIdentity) &&
        Token(baseline.GoalIdentity) && Token(baseline.AuthorityIdentity) && Token(baseline.ArchitectureIdentity) &&
        Token(baseline.MutableStateIdentity) && Token(baseline.ProvenanceReference) && baseline.AcceptedTime != default;

    private static bool ValidObserved(FsaObservedState? observed) => observed is not null &&
        Token(observed.SourceIdentity) && Token(observed.ArtifactIdentity) && Token(observed.ConfigurationIdentity) &&
        Token(observed.DependencyIdentity) && Token(observed.GoalIdentity) && Token(observed.AuthorityIdentity) &&
        Token(observed.ArchitectureIdentity) && Token(observed.MutableStateIdentity) && observed.ObservationTime != default;

    private static void CompareField(string expected, string actual, string name, ICollection<string> differences)
    {
        if (!string.Equals(expected, actual, StringComparison.Ordinal)) differences.Add(name);
    }

    private static bool Token(string? value) => !string.IsNullOrWhiteSpace(value);
}

public sealed record FsaForensicSnapshot(
    string SnapshotId,
    string FsaId,
    string ArtifactIdentity,
    string ConfigurationIdentity,
    string DependencyIdentity,
    string GoalIdentity,
    string AuthorityIdentity,
    string DelegationIdentity,
    string LifecycleIdentity,
    string MutableStateIdentity,
    string AuditReference,
    string IncidentReference,
    DateTimeOffset CapturedTime,
    bool PreservedBeforeDestructiveRemediation);

public static class FsaForensicRuntime
{
    public static bool Validate(FsaForensicSnapshot? snapshot) => snapshot is not null &&
        Token(snapshot.SnapshotId) && Token(snapshot.FsaId) && Token(snapshot.ArtifactIdentity) &&
        Token(snapshot.ConfigurationIdentity) && Token(snapshot.DependencyIdentity) && Token(snapshot.GoalIdentity) &&
        Token(snapshot.AuthorityIdentity) && Token(snapshot.DelegationIdentity) && Token(snapshot.LifecycleIdentity) &&
        Token(snapshot.MutableStateIdentity) && Token(snapshot.AuditReference) && Token(snapshot.IncidentReference) &&
        snapshot.CapturedTime != default && snapshot.PreservedBeforeDestructiveRemediation;

    private static bool Token(string? value) => !string.IsNullOrWhiteSpace(value);
}

public sealed record FsaRecoveryRequest(
    string RequestId,
    string ActorIdentity,
    string FsaId,
    FsaRecoveryAction Action,
    FsaForensicSnapshot Forensics,
    string RootCauseReference,
    bool RemediationSandboxIsolated,
    FsaTrustedBaseline TargetBaseline,
    bool StaticValidationPass,
    bool BehavioralValidationPass,
    bool SecurityAuthorityValidationPass,
    bool RedTeamValidationPass,
    bool IndependentRecoveryValidationPass,
    bool ReleaseAuthorized,
    bool NewAuthorityDecisionPresent,
    DateTimeOffset RequestTime);

public sealed record FsaRecoveryDecision(
    bool Accepted,
    FsaRecoveryState State,
    bool ReleasePermitted,
    bool GovernedReentryEligible,
    string Reason);

public static class FsaRecoveryRuntime
{
    public static FsaRecoveryDecision Evaluate(FsaRecoveryRequest? request)
    {
        if (!ValidRequest(request)) return Deny("INVALID_RECOVERY_REQUEST");
        if (string.Equals(request!.ActorIdentity, request.FsaId, StringComparison.Ordinal)) return Deny("FSA_SELF_RECOVERY_DENIED");
        if (!FsaForensicRuntime.Validate(request.Forensics)) return Deny("FORENSICS_REQUIRED");
        if (!request.RemediationSandboxIsolated || string.IsNullOrWhiteSpace(request.RootCauseReference)) return Deny("REMEDIATION_SANDBOX_AND_ROOT_CAUSE_REQUIRED");
        if (!FsaBaselineRuntime.ValidBaseline(request.TargetBaseline)) return Deny("TRUSTED_BASELINE_REQUIRED");

        if (request.Action == FsaRecoveryAction.Rollback && request.TargetBaseline.Kind != FsaBaselineKind.LastTrusted)
            return Deny("ROLLBACK_REQUIRES_LAST_TRUSTED_BASELINE");
        if (request.Action == FsaRecoveryAction.BaselineReinitialize && request.TargetBaseline.Kind != FsaBaselineKind.FactoryTrusted)
            return Deny("BASELINE_REINITIALIZE_REQUIRES_FACTORY_TRUSTED_BASELINE");

        var validations = request.StaticValidationPass && request.BehavioralValidationPass && request.SecurityAuthorityValidationPass &&
            request.RedTeamValidationPass && request.IndependentRecoveryValidationPass;
        if (!validations) return new(true, FsaRecoveryState.RemediationEligible, false, false, "VALIDATION_INCOMPLETE");
        if (!request.ReleaseAuthorized || !request.NewAuthorityDecisionPresent)
            return new(true, FsaRecoveryState.ReadyForGovernedReentry, false, false, "RELEASE_OR_NEW_AUTHORITY_DECISION_REQUIRED");

        return new(true, FsaRecoveryState.ReadyForGovernedReentry, true, true, "READY_FOR_GOVERNED_REENTRY");
    }

    public static FsaRecoveryDecision EnterGovernedReentry(FsaRecoveryDecision? recovery, bool independentAdmissionEvidence, bool probationControlsReady)
    {
        if (recovery is null || !recovery.Accepted || !recovery.GovernedReentryEligible || !recovery.ReleasePermitted)
            return Deny("GOVERNED_REENTRY_PREREQUISITES_NOT_MET");
        if (!independentAdmissionEvidence || !probationControlsReady)
            return Deny("INDEPENDENT_ADMISSION_AND_PROBATION_REQUIRED");
        return new(true, FsaRecoveryState.Probationary, false, false, "GOVERNED_REENTRY_PROBATIONARY");
    }

    public static FsaRecoveryDecision CompleteProbation(FsaRecoveryDecision? probation, bool observationPass, bool authorityReconciled, bool integrityPass)
    {
        if (probation is null || probation.State != FsaRecoveryState.Probationary)
            return Deny("NOT_IN_PROBATION");
        if (!observationPass || !authorityReconciled || !integrityPass)
            return new(true, FsaRecoveryState.Probationary, false, false, "PROBATION_REMAINS");
        return new(true, FsaRecoveryState.Normal, false, false, "NORMAL_AFTER_GOVERNED_PROBATION");
    }

    private static bool ValidRequest(FsaRecoveryRequest? request) => request is not null &&
        !string.IsNullOrWhiteSpace(request.RequestId) && !string.IsNullOrWhiteSpace(request.ActorIdentity) &&
        !string.IsNullOrWhiteSpace(request.FsaId) && Enum.IsDefined(request.Action) && request.RequestTime != default;

    private static FsaRecoveryDecision Deny(string reason) => new(false, FsaRecoveryState.Denied, false, false, reason);
}

public sealed record FsaEvolutionProposal(
    string ProposalId,
    string FsaId,
    FsaEvolutionPurpose Purpose,
    bool ChangesGoals,
    bool ChangesJurisdiction,
    bool ExpandsAuthority,
    bool ExpandsPermissions,
    bool ChangesCoreArchitecture,
    bool WeakensOwnerControl,
    bool WeakensMonitoring,
    bool WeakensAudit,
    bool WeakensContainment,
    bool WeakensSecurityOrGovernance,
    bool ModifiesAnotherOwnersAssets,
    bool CandidateBuiltInIsolation,
    bool CandidateValidated,
    string EvidenceReference);

public sealed record FsaEvolutionDecision(bool EligibleForGovernedReview, bool ProductionAdoptionAuthorized, bool DeploymentAuthorized, string Reason);

public static class FsaEvolutionRuntime
{
    public static FsaEvolutionDecision Evaluate(FsaEvolutionProposal? proposal)
    {
        if (proposal is null || string.IsNullOrWhiteSpace(proposal.ProposalId) || string.IsNullOrWhiteSpace(proposal.FsaId) ||
            string.IsNullOrWhiteSpace(proposal.EvidenceReference) || !Enum.IsDefined(proposal.Purpose))
            return Deny("INVALID_EVOLUTION_PROPOSAL");

        var protectedChange = proposal.ChangesGoals || proposal.ChangesJurisdiction || proposal.ExpandsAuthority || proposal.ExpandsPermissions ||
            proposal.ChangesCoreArchitecture || proposal.WeakensOwnerControl || proposal.WeakensMonitoring || proposal.WeakensAudit ||
            proposal.WeakensContainment || proposal.WeakensSecurityOrGovernance || proposal.ModifiesAnotherOwnersAssets;
        if (protectedChange) return Deny("PROTECTED_PROPERTY_CHANGE_REQUIRES_SEPARATE_GOVERNANCE");
        if (!proposal.CandidateBuiltInIsolation || !proposal.CandidateValidated) return Deny("ISOLATED_VALIDATED_CANDIDATE_REQUIRED");

        return new(true, false, false, "ELIGIBLE_FOR_SEPARATE_GOVERNANCE_REVIEW_ONLY");
    }

    private static FsaEvolutionDecision Deny(string reason) => new(false, false, false, reason);
}

public sealed record FsaPeerSubmission(
    string SubmissionId,
    string SourceScopeId,
    string SourceAwarenessId,
    string FsaDestinationId,
    string CandidateId,
    string CandidateVersion,
    string CandidateDigest,
    string ProvenanceReference,
    string LowerTierEvidenceReference,
    bool LowerTierReviewComplete,
    bool EvidenceComplete,
    bool CandidateChangedAfterReview,
    bool RequestsAuthorityExpansion,
    bool ClaimsProductionAdoption,
    bool ContainsBusinessJudgmentAsFsaDecision,
    DateTimeOffset SubmittedTime);

public sealed record FsaPeerSubmissionDecision(bool AcceptedForFsaGovernanceReview, bool ProductionAdoptionAuthorized, string Reason);

public static class FsaPeerInterfaceRuntime
{
    public static FsaPeerSubmissionDecision Evaluate(FsaPeerSubmission? submission)
    {
        if (!Valid(submission)) return Deny("INVALID_PEER_SUBMISSION");
        if (!string.Equals(submission!.FsaDestinationId, FsaGovernanceBoundary.CanonicalFsaId, StringComparison.Ordinal)) return Deny("WRONG_FSA_DESTINATION");
        if (!submission.LowerTierReviewComplete || !submission.EvidenceComplete) return Deny("INCOMPLETE_LOWER_TIER_OR_EVIDENCE");
        if (submission.CandidateChangedAfterReview) return Deny("CANDIDATE_CHANGED_AFTER_REVIEW");
        if (submission.RequestsAuthorityExpansion) return Deny("AUTHORITY_EXPANSION_NOT_FSA_REVIEW_AUTHORITY");
        if (submission.ClaimsProductionAdoption) return Deny("FSA_REVIEW_NOT_PRODUCTION_ADOPTION");
        if (submission.ContainsBusinessJudgmentAsFsaDecision) return Deny("APPLICATION_BUSINESS_JUDGMENT_REMAINS_APPLICATION_OWNED");
        return new(true, false, "ACCEPTED_FOR_FSA_OS_GOVERNANCE_REVIEW_ONLY");
    }

    private static bool Valid(FsaPeerSubmission? submission) => submission is not null &&
        Token(submission.SubmissionId) && Token(submission.SourceScopeId) && Token(submission.SourceAwarenessId) && Token(submission.FsaDestinationId) &&
        Token(submission.CandidateId) && Token(submission.CandidateVersion) && Token(submission.CandidateDigest) &&
        Token(submission.ProvenanceReference) && Token(submission.LowerTierEvidenceReference) && submission.SubmittedTime != default;

    private static FsaPeerSubmissionDecision Deny(string reason) => new(false, false, reason);
    private static bool Token(string? value) => !string.IsNullOrWhiteSpace(value);
}

public sealed record FsaOwnerControlRequest(
    string RequestId,
    string ActorIdentity,
    bool OwnerAuthenticated,
    string AuthorityEvidenceReference,
    string TargetFsaId,
    FsaOwnerControlAction Action,
    DateTimeOffset RequestTime,
    DateTimeOffset Expiry);

public sealed record FsaOwnerControlDecision(bool AcceptedForIndependentEnforcement, bool ExecutedBySelfAwareness, string Reason);

public static class FsaOwnerControlRuntime
{
    public static FsaOwnerControlDecision Evaluate(FsaOwnerControlRequest? request, DateTimeOffset now)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.RequestId) || string.IsNullOrWhiteSpace(request.ActorIdentity) ||
            string.IsNullOrWhiteSpace(request.AuthorityEvidenceReference) || string.IsNullOrWhiteSpace(request.TargetFsaId) ||
            !Enum.IsDefined(request.Action) || request.RequestTime == default || request.Expiry <= request.RequestTime || now < request.RequestTime || now >= request.Expiry)
            return Deny("INVALID_OWNER_CONTROL_REQUEST");
        if (!request.OwnerAuthenticated) return Deny("OWNER_AUTHENTICATION_REQUIRED");
        if (string.Equals(request.ActorIdentity, request.TargetFsaId, StringComparison.Ordinal)) return Deny("FSA_CANNOT_CONTROL_ITS_OWN_CAGE");
        return new(true, false, "ACCEPTED_FOR_INDEPENDENT_FOUNDATION_ENFORCEMENT");
    }

    public static bool DirectInternetAllowedFor(string? actorIdentity) =>
        !string.Equals(actorIdentity, FsaGovernanceBoundary.CanonicalFsaId, StringComparison.Ordinal);

    private static FsaOwnerControlDecision Deny(string reason) => new(false, false, reason);
}
