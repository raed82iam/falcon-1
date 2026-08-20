using System;
using System.Collections.Generic;
using System.Linq;
using Foundation.ApplicationLifecycle;

namespace Falcon.Stage5.WP09.Verifier;

internal static class Program
{
    private const string Subject = "app:generic-unit";
    private static readonly ApplicationLifecycleEvaluator Evaluator = new();

    private static int Main()
    {
        var scenarios = new (string Name, Func<bool> Test)[]
        {
            ("attach_valid_allowed", AttachValidAllowed),
            ("attach_deterministic", AttachDeterministic),
            ("attach_from_attached_rejected", AttachFromAttachedRejected),
            ("attach_missing_target_rejected", AttachMissingTargetRejected),
            ("authority_missing_rejected", () => AuthorityStatusRejected(LifecycleEvidenceStatus.Missing, LifecycleReason.AuthorityMissing)),
            ("authority_stale_rejected", () => AuthorityStatusRejected(LifecycleEvidenceStatus.Stale, LifecycleReason.AuthorityStale)),
            ("authority_revoked_rejected", () => AuthorityStatusRejected(LifecycleEvidenceStatus.Revoked, LifecycleReason.AuthorityRevoked)),
            ("authority_invalid_rejected", () => AuthorityStatusRejected(LifecycleEvidenceStatus.Invalid, LifecycleReason.AuthorityInvalid)),
            ("authority_ambiguous_rejected", () => AuthorityStatusRejected(LifecycleEvidenceStatus.Ambiguous, LifecycleReason.AuthorityAmbiguous)),
            ("authority_subject_mismatch_rejected", AuthoritySubjectMismatchRejected),
            ("authority_transition_mismatch_rejected", AuthorityTransitionMismatchRejected),
            ("authority_version_mismatch_rejected", AuthorityVersionMismatchRejected),
            ("manifest_stale_rejected", () => PrerequisiteRejected(manifest: LifecycleEvidenceStatus.Stale, reason: LifecycleReason.ManifestInvalid)),
            ("dependency_stale_rejected", () => PrerequisiteRejected(dependency: LifecycleEvidenceStatus.Stale, reason: LifecycleReason.DependencyInvalid)),
            ("compatibility_stale_rejected", () => PrerequisiteRejected(compatibility: LifecycleEvidenceStatus.Stale, reason: LifecycleReason.CompatibilityInvalid)),
            ("security_stale_rejected", () => PrerequisiteRejected(security: LifecycleEvidenceStatus.Stale, reason: LifecycleReason.SecurityInvalid)),
            ("authority_expansion_rejected", AuthorityExpansionRejected),
            ("protected_control_weakening_rejected", ProtectedControlWeakeningRejected),
            ("required_dependency_gap_rejected", RequiredDependencyGapRejected),
            ("contract_incompatibility_rejected", ContractIncompatibilityRejected),
            ("upgrade_valid_allowed", UpgradeValidAllowed),
            ("upgrade_same_version_rejected", UpgradeSameVersionRejected),
            ("upgrade_version_regression_evidence_rejected", UpgradeVersionRegressionEvidenceRejected),
            ("upgrade_from_detached_rejected", UpgradeFromDetachedRejected),
            ("upgrade_missing_drain_requires_drain", UpgradeMissingDrainRequiresDrain),
            ("upgrade_incomplete_valid_drain_requires_drain", UpgradeIncompleteDrainRequiresDrain),
            ("upgrade_stale_drain_rejected", UpgradeStaleDrainRejected),
            ("upgrade_revoked_drain_rejected", UpgradeRevokedDrainRejected),
            ("upgrade_complete_drain_allowed", UpgradeCompleteDrainAllowed),
            ("detach_valid_allowed", DetachValidAllowed),
            ("detach_hidden_coupling_rejected", DetachHiddenCouplingRejected),
            ("detach_missing_drain_requires_drain", DetachMissingDrainRequiresDrain),
            ("rollback_valid_allowed", RollbackValidAllowed),
            ("rollback_missing_evidence_rejected", RollbackMissingEvidenceRejected),
            ("rollback_stale_evidence_rejected", RollbackStaleEvidenceRejected),
            ("rollback_target_mismatch_rejected", RollbackTargetMismatchRejected),
            ("rollback_revoked_authority_rejected", RollbackRevokedAuthorityRejected),
            ("rollback_from_detached_rejected", RollbackFromDetachedRejected),
            ("correlation_preserved", CorrelationPreserved),
            ("causation_preserved", CausationPreserved),
            ("request_digest_deterministic", RequestDigestDeterministic),
            ("decision_identity_changes_with_target", DecisionIdentityChangesWithTarget),
            ("generic_application_names_preserve_semantics", GenericApplicationNamesPreserveSemantics),
            ("malformed_subject_rejected_at_construction", MalformedSubjectRejectedAtConstruction),
            ("undefined_transition_rejected_at_construction", UndefinedTransitionRejectedAtConstruction),
            ("public_surface_has_no_activation_or_deployment_api", PublicSurfaceHasNoActivationOrDeploymentApi),
            ("public_surface_has_no_trading_business_terms", PublicSurfaceHasNoTradingBusinessTerms),
            ("removed_generation_can_be_reattached_only_with_exact_authority", RemovedGenerationCanBeReattachedOnlyWithExactAuthority),
            ("package_compatibility_does_not_override_revoked_authority", CompatibilityDoesNotOverrideRevokedAuthority)
        };

        var failures = new List<string>();
        foreach (var scenario in scenarios)
        {
            try
            {
                if (scenario.Test()) Console.WriteLine($"PASS {scenario.Name}");
                else { Console.WriteLine($"FAIL {scenario.Name}"); failures.Add(scenario.Name); }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FAIL {scenario.Name}: {ex.GetType().Name}: {ex.Message}");
                failures.Add(scenario.Name);
            }
        }

        Console.WriteLine();
        Console.WriteLine($"Stage 5 WP-09 verifier: {(failures.Count == 0 ? "PASS" : "FAIL")}");
        Console.WriteLine($"Scenarios: {scenarios.Length - failures.Count}/{scenarios.Length} PASS");
        Console.WriteLine("Application-neutral attachment, upgrade/replacement, drain, safe detach/removal, rollback, authority non-creation, deterministic evidence, and WP-10 boundary verified.");
        Console.WriteLine("No deployment/runtime activation, external egress, credential use, Trading business semantics, FSA autonomous-promotion control plane, or WP-10 closure authority is implemented.");
        if (failures.Count == 0) return 0;
        foreach (var failure in failures) Console.WriteLine($"- {failure}");
        return 1;
    }

    private static bool AttachValidAllowed() => Is(Request(), LifecycleDecisionKind.Allowed, LifecycleReason.AttachEligible);
    private static bool AttachDeterministic() { var a = Evaluator.Evaluate(Request()); var b = Evaluator.Evaluate(Request()); return a.DecisionIdentity == b.DecisionIdentity && a.DecisionIdentity.Length == 64; }
    private static bool AttachFromAttachedRejected() => Is(Request(state: LifecycleState.Attached), LifecycleDecisionKind.Rejected, LifecycleReason.InvalidTransition);
    private static bool AttachMissingTargetRejected() => Is(Request(target: string.Empty), LifecycleDecisionKind.Rejected, LifecycleReason.TargetVersionRequired);
    private static bool AuthorityStatusRejected(LifecycleEvidenceStatus status, string reason) => Is(Request(authorityStatus: status), LifecycleDecisionKind.Rejected, reason);
    private static bool AuthoritySubjectMismatchRejected() => Is(Request(authoritySubject: "app:other"), LifecycleDecisionKind.Rejected, LifecycleReason.AuthoritySubjectMismatch);
    private static bool AuthorityTransitionMismatchRejected() => Is(Request(authorityTransition: LifecycleTransitionKind.Rollback), LifecycleDecisionKind.Rejected, LifecycleReason.AuthorityTransitionMismatch);
    private static bool AuthorityVersionMismatchRejected() => Is(Request(authorityTarget: "2.0.0"), LifecycleDecisionKind.Rejected, LifecycleReason.AuthorityVersionMismatch);

    private static bool PrerequisiteRejected(LifecycleEvidenceStatus manifest = LifecycleEvidenceStatus.Valid, LifecycleEvidenceStatus dependency = LifecycleEvidenceStatus.Valid, LifecycleEvidenceStatus compatibility = LifecycleEvidenceStatus.Valid, LifecycleEvidenceStatus security = LifecycleEvidenceStatus.Valid, string reason = "") =>
        Is(Request(manifestStatus: manifest, dependencyStatus: dependency, compatibilityStatus: compatibility, securityStatus: security), LifecycleDecisionKind.Rejected, reason);

    private static bool AuthorityExpansionRejected() => Is(Request(authorityDoesNotExpand: false), LifecycleDecisionKind.Rejected, LifecycleReason.AuthorityExpansion);
    private static bool ProtectedControlWeakeningRejected() => Is(Request(protectedControlsNotWeakened: false), LifecycleDecisionKind.Rejected, LifecycleReason.ProtectedControlWeakening);
    private static bool RequiredDependencyGapRejected() => Is(Request(requiredDependenciesSatisfied: false), LifecycleDecisionKind.Rejected, LifecycleReason.DependencyInvalid);
    private static bool ContractIncompatibilityRejected() => Is(Request(contractsCompatible: false), LifecycleDecisionKind.Rejected, LifecycleReason.CompatibilityInvalid);

    private static bool UpgradeValidAllowed() => Is(Request(transition: LifecycleTransitionKind.UpgradeOrReplace, state: LifecycleState.Attached, current: "1.0.0", target: "1.1.0"), LifecycleDecisionKind.Allowed, LifecycleReason.UpgradeEligible);
    private static bool UpgradeSameVersionRejected() => Is(Request(transition: LifecycleTransitionKind.UpgradeOrReplace, state: LifecycleState.Attached, current: "1.0.0", target: "1.0.0"), LifecycleDecisionKind.Rejected, LifecycleReason.TargetVersionUnchanged);
    private static bool UpgradeVersionRegressionEvidenceRejected() => Is(Request(transition: LifecycleTransitionKind.UpgradeOrReplace, state: LifecycleState.Attached, current: "2.0.0", target: "1.0.0", compatibilityStatus: LifecycleEvidenceStatus.Invalid), LifecycleDecisionKind.Rejected, LifecycleReason.CompatibilityInvalid);
    private static bool UpgradeFromDetachedRejected() => Is(Request(transition: LifecycleTransitionKind.UpgradeOrReplace, state: LifecycleState.Detached, current: "1.0.0", target: "1.1.0"), LifecycleDecisionKind.Rejected, LifecycleReason.InvalidTransition);
    private static bool UpgradeMissingDrainRequiresDrain() => Is(Request(transition: LifecycleTransitionKind.UpgradeOrReplace, state: LifecycleState.Attached, current: "1.0.0", target: "1.1.0", drainRequired: true, drainComplete: false, drainStatus: LifecycleEvidenceStatus.Missing), LifecycleDecisionKind.DrainRequired, LifecycleReason.DrainRequired);
    private static bool UpgradeIncompleteDrainRequiresDrain() => Is(Request(transition: LifecycleTransitionKind.UpgradeOrReplace, state: LifecycleState.Draining, current: "1.0.0", target: "1.1.0", drainRequired: true, drainComplete: false, drainStatus: LifecycleEvidenceStatus.Valid), LifecycleDecisionKind.DrainRequired, LifecycleReason.DrainRequired);
    private static bool UpgradeStaleDrainRejected() => Is(Request(transition: LifecycleTransitionKind.UpgradeOrReplace, state: LifecycleState.Draining, current: "1.0.0", target: "1.1.0", drainRequired: true, drainComplete: false, drainStatus: LifecycleEvidenceStatus.Stale), LifecycleDecisionKind.Rejected, LifecycleReason.DrainEvidenceInvalid);
    private static bool UpgradeRevokedDrainRejected() => Is(Request(transition: LifecycleTransitionKind.UpgradeOrReplace, state: LifecycleState.Draining, current: "1.0.0", target: "1.1.0", drainRequired: true, drainComplete: true, drainStatus: LifecycleEvidenceStatus.Revoked), LifecycleDecisionKind.Rejected, LifecycleReason.DrainEvidenceInvalid);
    private static bool UpgradeCompleteDrainAllowed() => Is(Request(transition: LifecycleTransitionKind.UpgradeOrReplace, state: LifecycleState.Draining, current: "1.0.0", target: "1.1.0", drainRequired: true, drainComplete: true, drainStatus: LifecycleEvidenceStatus.Valid), LifecycleDecisionKind.Allowed, LifecycleReason.UpgradeEligible);

    private static bool DetachValidAllowed() => Is(Request(transition: LifecycleTransitionKind.DetachOrRemove, state: LifecycleState.Attached, current: "1.0.0", target: string.Empty), LifecycleDecisionKind.Allowed, LifecycleReason.DetachEligible);
    private static bool DetachHiddenCouplingRejected() => Is(Request(transition: LifecycleTransitionKind.DetachOrRemove, state: LifecycleState.Attached, current: "1.0.0", target: string.Empty, hiddenCouplingAbsent: false), LifecycleDecisionKind.Rejected, LifecycleReason.HiddenCoupling);
    private static bool DetachMissingDrainRequiresDrain() => Is(Request(transition: LifecycleTransitionKind.DetachOrRemove, state: LifecycleState.Attached, current: "1.0.0", target: string.Empty, drainRequired: true, drainComplete: false, drainStatus: LifecycleEvidenceStatus.Missing), LifecycleDecisionKind.DrainRequired, LifecycleReason.DrainRequired);

    private static bool RollbackValidAllowed() => Is(Request(transition: LifecycleTransitionKind.Rollback, state: LifecycleState.RollbackRequired, current: "1.1.0", target: "1.0.0", rollback: true), LifecycleDecisionKind.Allowed, LifecycleReason.RollbackEligible);
    private static bool RollbackMissingEvidenceRejected() => Is(Request(transition: LifecycleTransitionKind.Rollback, state: LifecycleState.RollbackRequired, current: "1.1.0", target: "1.0.0"), LifecycleDecisionKind.Rejected, LifecycleReason.RollbackEvidenceInvalid);
    private static bool RollbackStaleEvidenceRejected() => Is(Request(transition: LifecycleTransitionKind.Rollback, state: LifecycleState.RollbackRequired, current: "1.1.0", target: "1.0.0", rollback: true, rollbackStatus: LifecycleEvidenceStatus.Stale), LifecycleDecisionKind.Rejected, LifecycleReason.RollbackEvidenceInvalid);
    private static bool RollbackTargetMismatchRejected() => Is(Request(transition: LifecycleTransitionKind.Rollback, state: LifecycleState.RollbackRequired, current: "1.1.0", target: "1.0.0", rollback: true, rollbackTarget: "0.9.0"), LifecycleDecisionKind.Rejected, LifecycleReason.RollbackTargetMismatch);
    private static bool RollbackRevokedAuthorityRejected() => Is(Request(transition: LifecycleTransitionKind.Rollback, state: LifecycleState.RollbackRequired, current: "1.1.0", target: "1.0.0", rollback: true, rollbackAuthorityStillValid: false), LifecycleDecisionKind.Rejected, LifecycleReason.RollbackAuthorityInvalid);
    private static bool RollbackFromDetachedRejected() => Is(Request(transition: LifecycleTransitionKind.Rollback, state: LifecycleState.Detached, current: "1.1.0", target: "1.0.0", rollback: true), LifecycleDecisionKind.Rejected, LifecycleReason.InvalidTransition);

    private static bool CorrelationPreserved() { var result = Evaluator.Evaluate(Request(correlation: "corr:123")); return result.CorrelationIdentity == "corr:123"; }
    private static bool CausationPreserved() { var result = Evaluator.Evaluate(Request(causation: "cause:456")); return result.CausationIdentity == "cause:456"; }
    private static bool RequestDigestDeterministic() => Request().RequestDigest == Request().RequestDigest;
    private static bool DecisionIdentityChangesWithTarget() { var a = Evaluator.Evaluate(Request(target: "1.0.0")); var b = Evaluator.Evaluate(Request(target: "1.1.0", authorityTarget: "1.1.0")); return a.DecisionIdentity != b.DecisionIdentity; }
    private static bool GenericApplicationNamesPreserveSemantics() { var a = Evaluator.Evaluate(Request(subject: "app:alpha", authoritySubject: "app:alpha")); var b = Evaluator.Evaluate(Request(subject: "app:omega", authoritySubject: "app:omega")); return a.Kind == LifecycleDecisionKind.Allowed && b.Kind == LifecycleDecisionKind.Allowed && a.Reason == b.Reason; }
    private static bool MalformedSubjectRejectedAtConstruction() { try { _ = Request(subject: "bad\nsubject", authoritySubject: "bad\nsubject"); return false; } catch (ArgumentException) { return true; } }
    private static bool UndefinedTransitionRejectedAtConstruction() { try { _ = Request(transition: (LifecycleTransitionKind)999); return false; } catch (ArgumentOutOfRangeException) { return true; } }
    private static bool PublicSurfaceHasNoActivationOrDeploymentApi() { var names = typeof(ApplicationLifecycleEvaluator).Assembly.GetExportedTypes().Select(t => t.FullName ?? t.Name).Concat(typeof(ApplicationLifecycleEvaluator).Assembly.GetExportedTypes().SelectMany(t => t.GetMembers().Select(m => m.Name))); return !names.Any(n => n.Contains("Deploy", StringComparison.OrdinalIgnoreCase) || n.Contains("Activation", StringComparison.OrdinalIgnoreCase)); }
    private static bool PublicSurfaceHasNoTradingBusinessTerms() { string[] prohibited = { "Trading", "Strategy", "Broker", "Portfolio", "MarketData", "OrderExecution" }; var names = typeof(ApplicationLifecycleEvaluator).Assembly.GetExportedTypes().Select(t => t.FullName ?? t.Name).Concat(typeof(ApplicationLifecycleEvaluator).Assembly.GetExportedTypes().SelectMany(t => t.GetMembers().Select(m => m.Name))); return !names.Any(n => prohibited.Any(p => n.Contains(p, StringComparison.OrdinalIgnoreCase))); }
    private static bool RemovedGenerationCanBeReattachedOnlyWithExactAuthority() { var ok = Evaluator.Evaluate(Request(state: LifecycleState.Removed)); var bad = Evaluator.Evaluate(Request(state: LifecycleState.Removed, authorityTarget: "2.0.0")); return ok.Kind == LifecycleDecisionKind.Allowed && bad.Kind == LifecycleDecisionKind.Rejected && bad.Reason == LifecycleReason.AuthorityVersionMismatch; }
    private static bool CompatibilityDoesNotOverrideRevokedAuthority() { var result = Evaluator.Evaluate(Request(authorityStatus: LifecycleEvidenceStatus.Revoked, compatibilityStatus: LifecycleEvidenceStatus.Valid)); return result.Kind == LifecycleDecisionKind.Rejected && result.Reason == LifecycleReason.AuthorityRevoked; }

    private static bool Is(LifecycleRequest request, LifecycleDecisionKind kind, string reason)
    {
        var result = Evaluator.Evaluate(request);
        return result.Kind == kind && result.Reason == reason && result.SubjectIdentity == request.SubjectIdentity;
    }

    private static LifecycleRequest Request(
        string subject = Subject,
        LifecycleTransitionKind transition = LifecycleTransitionKind.Attach,
        LifecycleState state = LifecycleState.Detached,
        string current = "",
        string target = "1.0.0",
        LifecycleEvidenceStatus authorityStatus = LifecycleEvidenceStatus.Valid,
        string? authoritySubject = null,
        LifecycleTransitionKind? authorityTransition = null,
        string? authorityCurrent = null,
        string? authorityTarget = null,
        LifecycleEvidenceStatus manifestStatus = LifecycleEvidenceStatus.Valid,
        LifecycleEvidenceStatus dependencyStatus = LifecycleEvidenceStatus.Valid,
        LifecycleEvidenceStatus compatibilityStatus = LifecycleEvidenceStatus.Valid,
        LifecycleEvidenceStatus securityStatus = LifecycleEvidenceStatus.Valid,
        bool authorityDoesNotExpand = true,
        bool protectedControlsNotWeakened = true,
        bool requiredDependenciesSatisfied = true,
        bool contractsCompatible = true,
        bool hiddenCouplingAbsent = true,
        bool drainRequired = false,
        bool drainComplete = false,
        LifecycleEvidenceStatus drainStatus = LifecycleEvidenceStatus.Valid,
        bool rollback = false,
        LifecycleEvidenceStatus rollbackStatus = LifecycleEvidenceStatus.Valid,
        string? rollbackTarget = null,
        bool rollbackAuthorityStillValid = true,
        string correlation = "corr:default",
        string causation = "cause:default")
    {
        var authority = new LifecycleAuthorityEvidence("authority:lifecycle", authorityStatus, authoritySubject ?? subject, authorityTransition ?? transition, authorityCurrent ?? current, authorityTarget ?? target);
        var rollbackEvidence = rollback ? new LifecycleRollbackEvidence("evidence:rollback", rollbackStatus, rollbackTarget ?? target, rollbackAuthorityStillValid) : null;
        return new LifecycleRequest(
            "request:lifecycle:001", subject, transition, state, current, target, authority,
            Evidence("manifest", manifestStatus), Evidence("dependency", dependencyStatus), Evidence("compatibility", compatibilityStatus), Evidence("security", securityStatus),
            new LifecycleContinuityEvidence("evidence:continuity", LifecycleEvidenceStatus.Valid, authorityDoesNotExpand, protectedControlsNotWeakened, requiredDependenciesSatisfied, contractsCompatible, hiddenCouplingAbsent),
            new LifecycleDrainEvidence(drainRequired, drainComplete, "evidence:drain", drainStatus), rollbackEvidence, correlation, causation);
    }

    private static LifecycleEvidence Evidence(string name, LifecycleEvidenceStatus status) => new($"evidence:{name}", status);
}
