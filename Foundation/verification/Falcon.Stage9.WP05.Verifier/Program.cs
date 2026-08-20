using System;
using Foundation.Reconciliation;
using Foundation.State;

internal static class Program
{
    private static int Main()
    {
        try
        {
            VerifyValidIndependentValidation();
            VerifyDeterministicIdentity();
            VerifyMutationSensitivity();
            VerifyReconciliationIdentityMismatchDenied();
            VerifyCaseBindingMismatchDenied();
            VerifyPlanBindingMismatchDenied();
            VerifyRestorationBindingMismatchDenied();
            VerifySubjectCannotSelfValidate();
            VerifyGuardianCannotValidate();
            VerifyRepairActorCannotSelfCertify();
            VerifyReleaseAuthorityCannotBeIndependentVerifier();
            VerifyFailedReconciliationDenied();
            VerifyPartialReconciliationDenied();
            VerifyUncertainReconciliationRemainsUncertain();
            VerifyStaleValidationEvidenceRemainsUncertain();
            VerifyUntrustedValidationEvidenceRemainsUncertain();
            VerifyMissingInputDenied();
            VerifyValidationDoesNotGrantRelease();
            VerifyValidationDoesNotReintroduceLifecycle();
            VerifyApplicationNeutrality();

            Console.WriteLine("STAGE9_WP05_VERIFIER = PASS");
            Console.WriteLine("CHECKS = 20/20");
            Console.WriteLine("ACR9_001 = PASS");
            Console.WriteLine("INDEPENDENT_RECOVERY_VERIFIER != SUBJECT_GUARDIAN_REPAIR_ACTOR_RELEASE_AUTHORITY");
            Console.WriteLine("FAILED_PARTIAL_UNCERTAIN_RECONCILIATION != POSITIVE_VALIDATION");
            Console.WriteLine("VALIDATION_SUCCESS != RECOVERY_READINESS");
            Console.WriteLine("VALIDATION_SUCCESS != RELEASE_AUTHORIZATION");
            Console.WriteLine("RELEASE_OR_LIFECYCLE_AUTHORITY_SURFACE = NONE");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("STAGE9_WP05_VERIFIER = FAIL");
            Console.Error.WriteLine(ex.Message);
            return 1;
        }
    }

    private static ReconciliationRequest FoundationRequest() =>
        new(
            "foundation-state:recovery",
            "subject:recovery:001",
            FoundationStateClass.ReconciliationState,
            "request:reconciliation:wp05",
            "decision:reconciliation:wp05");

    private static ReconciliationResult FoundationResult(
        ReconciliationClassification classification = ReconciliationClassification.Consistent,
        bool continuationAllowed = true,
        bool challengeRequired = false) =>
        new(
            classification,
            "reason:reconciliation:wp05",
            null,
            null,
            null,
            continuationAllowed,
            challengeRequired);

    private static RecoveryReconciliationDimension Dimension(
        string name,
        RecoveryReconciliationDimensionStatus status = RecoveryReconciliationDimensionStatus.Complete,
        bool current = true,
        bool trusted = true) =>
        new(
            "dimension:" + name,
            status,
            "evidence:" + name + ":wp05",
            current,
            trusted);

    private static RecoveryReconciliationComposite Composite(
        RecoveryReconciliationDimensionStatus dimensionStatus = RecoveryReconciliationDimensionStatus.Complete,
        ReconciliationClassification foundationClassification = ReconciliationClassification.Consistent,
        bool continuationAllowed = true,
        bool challengeRequired = false)
    {
        var request = FoundationRequest();
        var result = FoundationResult(foundationClassification, continuationAllowed, challengeRequired);
        var input = new RecoveryReconciliationInput(
            "case:001",
            "plan:001:v1",
            "restoration:001",
            ReconciliationCanonicalEncoding.RequestIdentity(request),
            ReconciliationCanonicalEncoding.ResultIdentity(request, result),
            Dimension("configuration", dimensionStatus),
            Dimension("authority"),
            Dimension("security"),
            Dimension("durable-state"),
            Dimension("dependency"),
            Dimension("restriction"),
            Dimension("evidence-provenance"),
            new DateTimeOffset(2026, 8, 15, 17, 0, 0, TimeSpan.Zero));

        return RecoveryReconciliationCompositeBuilder.Build(request, result, input);
    }

    private static IndependentRecoveryValidationInput Input(RecoveryReconciliationComposite composite) =>
        new(
            composite.RecoveryCaseIdentity,
            composite.AuthorizedRecoveryPlanIdentity,
            composite.RestorationOutcomeIdentity,
            composite.Identity,
            "subject:001",
            "guardian:001",
            "repair-actor:001",
            "independent-verifier:001",
            "release-authority:001",
            "evidence:independent-validation:001",
            true,
            true,
            new DateTimeOffset(2026, 8, 15, 17, 5, 0, TimeSpan.Zero));

    private static void VerifyValidIndependentValidation()
    {
        var composite = Composite();
        var decision = IndependentRecoveryValidator.Evaluate(composite, Input(composite));
        Require(decision.Classification == IndependentRecoveryValidationClassification.Validated,
            "complete trusted reconciliation was not independently validated");
        Require(decision.Reason == IndependentRecoveryValidationReason.Pass,
            "positive validation reason was not PASS");
        Require(decision.Identity.Length == 64,
            "independent recovery validation identity is not SHA-256 length");
    }

    private static void VerifyDeterministicIdentity()
    {
        var composite = Composite();
        var input = Input(composite);
        var a = IndependentRecoveryValidator.Evaluate(composite, input);
        var b = IndependentRecoveryValidator.Evaluate(composite, input);
        Require(string.Equals(a.Identity, b.Identity, StringComparison.Ordinal),
            "identical independent validation inputs produced different identities");
    }

    private static void VerifyMutationSensitivity()
    {
        var composite = Composite();
        var input = Input(composite);
        var a = IndependentRecoveryValidator.Evaluate(composite, input);
        var b = IndependentRecoveryValidator.Evaluate(
            composite,
            input with { ValidationEvidenceIdentity = "evidence:independent-validation:002" });
        Require(!string.Equals(a.Identity, b.Identity, StringComparison.Ordinal),
            "material validation evidence mutation did not change decision identity");
    }

    private static void VerifyReconciliationIdentityMismatchDenied()
    {
        var composite = Composite();
        var decision = IndependentRecoveryValidator.Evaluate(
            composite,
            Input(composite) with { RecoveryReconciliationIdentity = new string('A', 64) });
        Require(decision.Classification == IndependentRecoveryValidationClassification.Denied &&
                decision.Reason == IndependentRecoveryValidationReason.ReconciliationIdentityMismatch,
            "mismatched reconciliation identity was accepted");
    }

    private static void VerifyCaseBindingMismatchDenied()
    {
        var composite = Composite();
        var decision = IndependentRecoveryValidator.Evaluate(
            composite,
            Input(composite) with { RecoveryCaseIdentity = "case:wrong" });
        Require(decision.Classification == IndependentRecoveryValidationClassification.Denied &&
                decision.Reason == IndependentRecoveryValidationReason.RecoveryBindingMismatch,
            "mismatched recovery case was accepted");
    }

    private static void VerifyPlanBindingMismatchDenied()
    {
        var composite = Composite();
        var decision = IndependentRecoveryValidator.Evaluate(
            composite,
            Input(composite) with { AuthorizedRecoveryPlanIdentity = "plan:wrong:v2" });
        Require(decision.Classification == IndependentRecoveryValidationClassification.Denied,
            "mismatched recovery plan was accepted");
    }

    private static void VerifyRestorationBindingMismatchDenied()
    {
        var composite = Composite();
        var decision = IndependentRecoveryValidator.Evaluate(
            composite,
            Input(composite) with { RestorationOutcomeIdentity = "restoration:wrong" });
        Require(decision.Classification == IndependentRecoveryValidationClassification.Denied,
            "mismatched restoration outcome was accepted");
    }

    private static void VerifySubjectCannotSelfValidate()
    {
        var composite = Composite();
        var input = Input(composite);
        var decision = IndependentRecoveryValidator.Evaluate(
            composite,
            input with { IndependentVerifierIdentity = input.SubjectIdentity });
        RequireRoleConflict(decision, "subject self-validation was accepted");
    }

    private static void VerifyGuardianCannotValidate()
    {
        var composite = Composite();
        var input = Input(composite);
        var decision = IndependentRecoveryValidator.Evaluate(
            composite,
            input with { IndependentVerifierIdentity = input.GuardianIdentity });
        RequireRoleConflict(decision, "Guardian was accepted as independent recovery verifier");
    }

    private static void VerifyRepairActorCannotSelfCertify()
    {
        var composite = Composite();
        var input = Input(composite);
        var decision = IndependentRecoveryValidator.Evaluate(
            composite,
            input with { IndependentVerifierIdentity = input.RepairActorIdentity });
        RequireRoleConflict(decision, "repair actor self-certification was accepted");
    }

    private static void VerifyReleaseAuthorityCannotBeIndependentVerifier()
    {
        var composite = Composite();
        var input = Input(composite);
        var decision = IndependentRecoveryValidator.Evaluate(
            composite,
            input with { IndependentVerifierIdentity = input.DeclaredReleaseAuthorityIdentity });
        RequireRoleConflict(decision, "ACR-9-001 verifier/release-authority separation was violated");
    }

    private static void VerifyFailedReconciliationDenied()
    {
        var composite = Composite(
            foundationClassification: ReconciliationClassification.CurrentStateCorrupted,
            continuationAllowed: false,
            challengeRequired: true);
        var decision = IndependentRecoveryValidator.Evaluate(composite, Input(composite));
        Require(decision.Classification == IndependentRecoveryValidationClassification.Denied &&
                decision.Reason == IndependentRecoveryValidationReason.ReconciliationFailed,
            "failed reconciliation became positive independent validation");
    }

    private static void VerifyPartialReconciliationDenied()
    {
        var composite = Composite(RecoveryReconciliationDimensionStatus.Partial);
        var decision = IndependentRecoveryValidator.Evaluate(composite, Input(composite));
        Require(decision.Classification == IndependentRecoveryValidationClassification.Denied &&
                decision.Reason == IndependentRecoveryValidationReason.ReconciliationPartial,
            "partial reconciliation became positive independent validation");
    }

    private static void VerifyUncertainReconciliationRemainsUncertain()
    {
        var composite = Composite(
            foundationClassification: ReconciliationClassification.UncertainAfterCommit,
            continuationAllowed: false,
            challengeRequired: true);
        var decision = IndependentRecoveryValidator.Evaluate(composite, Input(composite));
        Require(decision.Classification == IndependentRecoveryValidationClassification.Uncertain &&
                decision.Reason == IndependentRecoveryValidationReason.ReconciliationUncertain,
            "uncertain reconciliation became positive validation or false terminal certainty");
    }

    private static void VerifyStaleValidationEvidenceRemainsUncertain()
    {
        var composite = Composite();
        var decision = IndependentRecoveryValidator.Evaluate(
            composite,
            Input(composite) with { EvidenceCurrent = false });
        Require(decision.Classification == IndependentRecoveryValidationClassification.Uncertain &&
                decision.Reason == IndependentRecoveryValidationReason.EvidenceNotCurrent,
            "stale validation evidence became positive validation");
    }

    private static void VerifyUntrustedValidationEvidenceRemainsUncertain()
    {
        var composite = Composite();
        var decision = IndependentRecoveryValidator.Evaluate(
            composite,
            Input(composite) with { EvidenceTrusted = false });
        Require(decision.Classification == IndependentRecoveryValidationClassification.Uncertain &&
                decision.Reason == IndependentRecoveryValidationReason.EvidenceNotTrusted,
            "untrusted validation evidence became positive validation");
    }

    private static void VerifyMissingInputDenied()
    {
        var composite = Composite();
        var decision = IndependentRecoveryValidator.Evaluate(
            composite,
            Input(composite) with { ValidationEvidenceIdentity = string.Empty });
        Require(decision.Classification == IndependentRecoveryValidationClassification.Denied &&
                decision.Reason == IndependentRecoveryValidationReason.InvalidInput,
            "missing required independent validation input was accepted");
    }

    private static void VerifyValidationDoesNotGrantRelease()
    {
        foreach (var property in typeof(IndependentRecoveryValidationDecision).GetProperties())
        {
            Require(!property.Name.Contains("ReleaseAuthorization", StringComparison.OrdinalIgnoreCase),
                "independent validation decision exposes release authorization");
            Require(!property.Name.Contains("RestrictionRelease", StringComparison.OrdinalIgnoreCase),
                "independent validation decision exposes restriction release");
        }
    }

    private static void VerifyValidationDoesNotReintroduceLifecycle()
    {
        foreach (var property in typeof(IndependentRecoveryValidationDecision).GetProperties())
        {
            Require(!property.Name.Contains("LifecycleTransition", StringComparison.OrdinalIgnoreCase),
                "independent validation decision exposes lifecycle transition");
            Require(!property.Name.Contains("Reintroduction", StringComparison.OrdinalIgnoreCase),
                "independent validation decision exposes lifecycle reintroduction");
        }
    }

    private static void VerifyApplicationNeutrality()
    {
        foreach (var reference in typeof(IndependentRecoveryValidationDecision).Assembly.GetReferencedAssemblies())
        {
            var name = reference.Name ?? string.Empty;
            Require(!name.Contains("Trading", StringComparison.OrdinalIgnoreCase),
                "Trading dependency leaked into independent recovery validation");
            Require(!name.Contains("Web", StringComparison.OrdinalIgnoreCase),
                "Web dependency leaked into independent recovery validation");
            Require(!name.Contains("SelfAwareness", StringComparison.OrdinalIgnoreCase),
                "Stage 13/FSA dependency leaked into independent recovery validation");
        }
    }

    private static void RequireRoleConflict(IndependentRecoveryValidationDecision decision, string message)
    {
        Require(decision.Classification == IndependentRecoveryValidationClassification.Denied &&
                decision.Reason == IndependentRecoveryValidationReason.VerifierNotIndependent,
            message);
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }
}
