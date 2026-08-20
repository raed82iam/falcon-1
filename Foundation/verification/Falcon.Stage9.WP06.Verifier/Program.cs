using System;
using Foundation.Reconciliation;

internal static class Program
{
    private static int Main()
    {
        try
        {
            VerifyValidReadiness();
            VerifyDeterministicIdentity();
            VerifyMutationSensitivity();
            VerifyInvalidHandoffDenied();
            VerifyStaleHandoffDenied();
            VerifyRecoveryCaseMismatchDenied();
            VerifyPlanMismatchDenied();
            VerifyReconciliationMismatchDenied();
            VerifyIncompleteReconciliationDenied();
            VerifyUncertainReconciliationUncertain();
            VerifyValidationDeniedBlocksReadiness();
            VerifyValidationUncertainBlocksReadiness();
            VerifyRestrictionMismatchDenied();
            VerifyNewerStricterRestrictionDenied();
            VerifyGuardianConditionsUnsatisfiedDenied();
            VerifyGuardianConditionsUntrustedUncertain();
            VerifySecurityStateStaleUncertain();
            VerifyDependencyStateStaleUncertain();
            VerifyResidualRiskMissingOrUntrustedUncertain();
            VerifyResidualRiskOutsideBoundsDenied();
            VerifyNoReleaseOrLifecycleAuthoritySurface();
            VerifyApplicationNeutrality();

            Console.WriteLine("STAGE9_WP06_VERIFIER = PASS");
            Console.WriteLine("CHECKS = 22/22");
            Console.WriteLine("READY_FOR_RELEASE_DECISION != RELEASE");
            Console.WriteLine("GUARDIAN_CONDITIONS_CHECKED != GUARDIAN_SELF_RELEASE");
            Console.WriteLine("RESIDUAL_RISK_OUTSIDE_AUTHORIZED_BOUNDS = FAIL_CLOSED");
            Console.WriteLine("NEWER_STRICTER_RESTRICTION_INVALIDATES_READINESS");
            Console.WriteLine("WP05_VALIDATION_PASS_REQUIRED = YES");
            Console.WriteLine("RELEASE_OR_LIFECYCLE_AUTHORITY_SURFACE = NONE");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("STAGE9_WP06_VERIFIER = FAIL");
            Console.Error.WriteLine(ex.Message);
            return 1;
        }
    }

    private static readonly DateTimeOffset T0 = new(2026, 8, 15, 18, 0, 0, TimeSpan.Zero);

    private static RecoveryReconciliationComposite Reconciliation(
        RecoveryReconciliationClassification classification = RecoveryReconciliationClassification.Complete) =>
        new(
            classification,
            classification == RecoveryReconciliationClassification.Complete ? RecoveryReconciliationReason.Pass : "test-reason",
            "case:001",
            "plan:001",
            "restoration:001",
            "foundation-result:001",
            "evidence:configuration",
            "evidence:authority",
            "evidence:security",
            "evidence:durable-state",
            "evidence:dependency",
            "evidence:restriction",
            "evidence:provenance",
            T0);

    private static IndependentRecoveryValidationDecision Validation(
        RecoveryReconciliationComposite reconciliation,
        IndependentRecoveryValidationClassification classification = IndependentRecoveryValidationClassification.Validated) =>
        new(
            classification,
            classification == IndependentRecoveryValidationClassification.Validated ? IndependentRecoveryValidationReason.Pass : "test-validation-reason",
            reconciliation.RecoveryCaseIdentity,
            reconciliation.AuthorizedRecoveryPlanIdentity,
            reconciliation.RestorationOutcomeIdentity,
            reconciliation.Identity,
            "verifier:001",
            "release-authority:001",
            "evidence:validation",
            T0.AddMinutes(1));

    private static RecoveryReadinessConditionEvidence Condition(
        string name,
        bool satisfied = true,
        bool current = true,
        bool trusted = true) =>
        new("evidence:" + name, satisfied, current, trusted);

    private static RecoveryResidualRiskEvidence ResidualRisk(
        bool current = true,
        bool trusted = true,
        bool withinBounds = true) =>
        new("evidence:residual-risk", "risk-profile:authorized", current, trusted, withinBounds);

    private static RecoveryReadinessHandoffSnapshot Handoff() =>
        new(
            "handoff:001",
            "subject:001",
            "restriction:001",
            "restriction-integrity:001",
            "release-conditions:001",
            "release-authority:001",
            "verifier:001",
            true,
            true,
            true,
            T0.AddMinutes(2));

    private static RecoveryReleaseReadinessInput Input() =>
        new(
            "case:001",
            "plan:001",
            Handoff(),
            "restriction:001",
            "restriction-integrity:001",
            false,
            Condition("guardian"),
            Condition("security"),
            Condition("dependency"),
            ResidualRisk(),
            T0.AddMinutes(3));

    private static RecoveryReleaseReadinessDecision Evaluate(
        RecoveryReleaseReadinessInput? input = null,
        RecoveryReconciliationComposite? reconciliation = null,
        IndependentRecoveryValidationDecision? validation = null)
    {
        var r = reconciliation ?? Reconciliation();
        var v = validation ?? Validation(r);
        return RecoveryReleaseReadinessEvaluator.Evaluate(r, v, input ?? Input());
    }

    private static void VerifyValidReadiness()
    {
        var decision = Evaluate();
        Require(decision.Classification == RecoveryReleaseReadinessClassification.ReadyForReleaseDecision,
            "valid recovery readiness did not become READY_FOR_RELEASE_DECISION");
        Require(decision.Reason == RecoveryReleaseReadinessReason.Pass,
            "valid readiness reason was not READY_FOR_RELEASE_DECISION");
        Require(decision.Identity.Length == 64, "readiness identity is not SHA-256 length");
    }

    private static void VerifyDeterministicIdentity()
    {
        var a = Evaluate();
        var b = Evaluate();
        Require(a.Identity == b.Identity, "same readiness inputs produced different identities");
    }

    private static void VerifyMutationSensitivity()
    {
        var a = Evaluate();
        var mutated = Input() with { ResidualRisk = new RecoveryResidualRiskEvidence("evidence:residual-risk:mutated", "risk-profile:authorized", true, true, true) };
        var b = Evaluate(mutated);
        Require(a.Identity != b.Identity, "material readiness evidence mutation did not change identity");
    }

    private static void VerifyInvalidHandoffDenied()
    {
        var input = Input() with { Handoff = Handoff() with { HandoffValid = false } };
        var d = Evaluate(input);
        Require(d.Classification == RecoveryReleaseReadinessClassification.NotReady && d.Reason == RecoveryReleaseReadinessReason.InvalidHandoff,
            "invalid handoff was accepted");
    }

    private static void VerifyStaleHandoffDenied()
    {
        var input = Input() with { Handoff = Handoff() with { HandoffTime = T0.AddMinutes(5) } };
        var d = Evaluate(input);
        Require(d.Classification == RecoveryReleaseReadinessClassification.NotReady && d.Reason == RecoveryReleaseReadinessReason.InvalidHandoff,
            "future/stale handoff timing was accepted");
    }

    private static void VerifyRecoveryCaseMismatchDenied()
    {
        var d = Evaluate(Input() with { RecoveryCaseIdentity = "case:wrong" });
        Require(d.Reason == RecoveryReleaseReadinessReason.RecoveryBindingMismatch, "recovery case mismatch was accepted");
    }

    private static void VerifyPlanMismatchDenied()
    {
        var d = Evaluate(Input() with { AuthorizedRecoveryPlanIdentity = "plan:wrong" });
        Require(d.Reason == RecoveryReleaseReadinessReason.RecoveryBindingMismatch, "authorized plan mismatch was accepted");
    }

    private static void VerifyReconciliationMismatchDenied()
    {
        var r = Reconciliation();
        var v = Validation(r) with { RecoveryReconciliationIdentity = "reconciliation:wrong" };
        var d = Evaluate(Input(), r, v);
        Require(d.Reason == RecoveryReleaseReadinessReason.RecoveryBindingMismatch, "validation/reconciliation mismatch was accepted");
    }

    private static void VerifyIncompleteReconciliationDenied()
    {
        var r = Reconciliation(RecoveryReconciliationClassification.Partial);
        var d = Evaluate(Input(), r, Validation(r));
        Require(d.Classification == RecoveryReleaseReadinessClassification.NotReady && d.Reason == RecoveryReleaseReadinessReason.ReconciliationNotComplete,
            "partial reconciliation became readiness");
    }

    private static void VerifyUncertainReconciliationUncertain()
    {
        var r = Reconciliation(RecoveryReconciliationClassification.Uncertain);
        var d = Evaluate(Input(), r, Validation(r));
        Require(d.Classification == RecoveryReleaseReadinessClassification.Uncertain,
            "uncertain reconciliation did not remain uncertain");
    }

    private static void VerifyValidationDeniedBlocksReadiness()
    {
        var r = Reconciliation();
        var d = Evaluate(Input(), r, Validation(r, IndependentRecoveryValidationClassification.Denied));
        Require(d.Classification == RecoveryReleaseReadinessClassification.NotReady && d.Reason == RecoveryReleaseReadinessReason.IndependentValidationNotPassed,
            "denied WP05 validation became readiness");
    }

    private static void VerifyValidationUncertainBlocksReadiness()
    {
        var r = Reconciliation();
        var d = Evaluate(Input(), r, Validation(r, IndependentRecoveryValidationClassification.Uncertain));
        Require(d.Classification == RecoveryReleaseReadinessClassification.Uncertain,
            "uncertain WP05 validation did not remain uncertain");
    }

    private static void VerifyRestrictionMismatchDenied()
    {
        var d = Evaluate(Input() with { CurrentControllingRestrictionIdentity = "restriction:new" });
        Require(d.Reason == RecoveryReleaseReadinessReason.RestrictionMismatch,
            "controlling restriction mismatch was accepted");
    }

    private static void VerifyNewerStricterRestrictionDenied()
    {
        var d = Evaluate(Input() with { NewerOrStricterRestrictionPresent = true });
        Require(d.Classification == RecoveryReleaseReadinessClassification.NotReady && d.Reason == RecoveryReleaseReadinessReason.NewerStricterRestriction,
            "newer/stricter restriction did not invalidate readiness");
    }

    private static void VerifyGuardianConditionsUnsatisfiedDenied()
    {
        var d = Evaluate(Input() with { GuardianConditions = Condition("guardian", satisfied: false) });
        Require(d.Reason == RecoveryReleaseReadinessReason.GuardianConditionsUnsatisfied,
            "unsatisfied Guardian condition was accepted");
    }

    private static void VerifyGuardianConditionsUntrustedUncertain()
    {
        var d = Evaluate(Input() with { GuardianConditions = Condition("guardian", trusted: false) });
        Require(d.Classification == RecoveryReleaseReadinessClassification.Uncertain && d.Reason == RecoveryReleaseReadinessReason.GuardianConditionsUntrusted,
            "untrusted Guardian condition evidence was accepted");
    }

    private static void VerifySecurityStateStaleUncertain()
    {
        var d = Evaluate(Input() with { SecurityState = Condition("security", current: false) });
        Require(d.Classification == RecoveryReleaseReadinessClassification.Uncertain && d.Reason == RecoveryReleaseReadinessReason.SecurityStateNotCurrent,
            "stale security state became readiness");
    }

    private static void VerifyDependencyStateStaleUncertain()
    {
        var d = Evaluate(Input() with { DependencyState = Condition("dependency", current: false) });
        Require(d.Classification == RecoveryReleaseReadinessClassification.Uncertain && d.Reason == RecoveryReleaseReadinessReason.DependencyStateNotCurrent,
            "stale dependency state became readiness");
    }

    private static void VerifyResidualRiskMissingOrUntrustedUncertain()
    {
        var d = Evaluate(Input() with { ResidualRisk = ResidualRisk(trusted: false) });
        Require(d.Classification == RecoveryReleaseReadinessClassification.Uncertain && d.Reason == RecoveryReleaseReadinessReason.ResidualRiskMissing,
            "untrusted residual-risk evidence became readiness");
    }

    private static void VerifyResidualRiskOutsideBoundsDenied()
    {
        var d = Evaluate(Input() with { ResidualRisk = ResidualRisk(withinBounds: false) });
        Require(d.Classification == RecoveryReleaseReadinessClassification.NotReady && d.Reason == RecoveryReleaseReadinessReason.ResidualRiskOutsideBounds,
            "residual risk outside authorized bounds became readiness");
    }

    private static void VerifyNoReleaseOrLifecycleAuthoritySurface()
    {
        foreach (var property in typeof(RecoveryReleaseReadinessDecision).GetProperties())
        {
            Require(!property.Name.Contains("ReleaseAuthorized", StringComparison.OrdinalIgnoreCase),
                "readiness decision exposes release authorization");
            Require(!property.Name.Contains("ReleaseExecuted", StringComparison.OrdinalIgnoreCase),
                "readiness decision exposes release execution");
            Require(!property.Name.Contains("LifecycleTransition", StringComparison.OrdinalIgnoreCase),
                "readiness decision exposes lifecycle transition");
            Require(!property.Name.Contains("AuthorityGranted", StringComparison.OrdinalIgnoreCase),
                "readiness decision exposes authority grant");
        }
    }

    private static void VerifyApplicationNeutrality()
    {
        foreach (var reference in typeof(RecoveryReleaseReadinessDecision).Assembly.GetReferencedAssemblies())
        {
            var name = reference.Name ?? string.Empty;
            Require(!name.Contains("Application", StringComparison.OrdinalIgnoreCase), "Application dependency leaked into readiness");
            Require(!name.Contains("Trading", StringComparison.OrdinalIgnoreCase), "Trading dependency leaked into readiness");
            Require(!name.Contains("Web", StringComparison.OrdinalIgnoreCase), "Web dependency leaked into readiness");
            Require(!name.Contains("SelfAwareness", StringComparison.OrdinalIgnoreCase), "Stage13/FSA dependency leaked into readiness");
        }
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }
}
