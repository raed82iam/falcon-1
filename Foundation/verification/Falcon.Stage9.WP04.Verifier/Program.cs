using System;
using Foundation.Reconciliation;
using Foundation.State;

internal static class Program
{
    private static int Main()
    {
        try
        {
            VerifyCompleteComposite();
            VerifyDeterministicIdentity();
            VerifyMutationSensitivity();
            VerifyMismatchedFoundationResultRejected();
            VerifyMissingDimensionEvidenceRejected();
            VerifyUncertainCommitFailsClosed();
            VerifyCorruptedStateFailsClosed();
            VerifyEvidenceJournalFailureFailsClosed();
            VerifyStaleSecurityContextFailsClosed();
            VerifyUntrustedSecurityContextFailsClosed();
            VerifyDependencyFailureRemainsFailed();
            VerifyRestrictionUncertaintyRemainsUncertain();
            VerifyEvidenceProvenanceUncertaintyRemainsUncertain();
            VerifyPartialDimensionRemainsPartial();
            VerifyUnknownDimensionDoesNotBecomeComplete();
            VerifyCompositeDoesNotGrantRelease();
            VerifyApplicationNeutrality();

            Console.WriteLine("STAGE9_WP04_VERIFIER = PASS");
            Console.WriteLine("CHECKS = 17/17");
            Console.WriteLine("UNKNOWN_RECOVERY_STATE = FAIL_CLOSED");
            Console.WriteLine("PARTIAL_RECOVERY != COMPLETE_RECOVERY");
            Console.WriteLine("STALE_SECURITY_CONTEXT != TRUSTED_SECURITY_CONTEXT");
            Console.WriteLine("FOUNDATION_RECONCILIATION = AUTHORITATIVE_RECONCILIATION_SUBSTRATE");
            Console.WriteLine("RELEASE_AUTHORITY_SURFACE = NONE");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("STAGE9_WP04_VERIFIER = FAIL");
            Console.Error.WriteLine(ex.Message);
            return 1;
        }
    }

    private static ReconciliationRequest CreateFoundationRequest() =>
        new(
            "foundation-state:recovery",
            "foundation-subject:example",
            FoundationStateClass.ReconciliationState,
            "request:reconcile:001",
            "decision:reconcile:001");

    private static ReconciliationResult CreateFoundationResult(
        ReconciliationClassification classification = ReconciliationClassification.Consistent,
        bool continuationAllowed = true,
        bool challengeRequired = false) =>
        new(
            classification,
            "reason:reconciliation:001",
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
            "evidence:" + name + ":001",
            current,
            trusted);

    private static RecoveryReconciliationInput CreateInput(
        ReconciliationRequest request,
        ReconciliationResult result) =>
        new(
            "recovery-case-identity:001",
            "recovery-plan-identity:001",
            "restoration-outcome-identity:001",
            ReconciliationCanonicalEncoding.RequestIdentity(request),
            ReconciliationCanonicalEncoding.ResultIdentity(request, result),
            Dimension("configuration"),
            Dimension("authority"),
            Dimension("security"),
            Dimension("durable-state"),
            Dimension("dependency"),
            Dimension("restriction"),
            Dimension("evidence-provenance"),
            new DateTimeOffset(2026, 8, 15, 14, 0, 0, TimeSpan.Zero));

    private static void VerifyCompleteComposite()
    {
        var request = CreateFoundationRequest();
        var result = CreateFoundationResult();
        var input = CreateInput(request, result);
        var composite = RecoveryReconciliationCompositeBuilder.Build(request, result, input);

        Require(composite.Classification == RecoveryReconciliationClassification.Complete,
            "valid recovery reconciliation did not classify COMPLETE");
        Require(composite.Reason == RecoveryReconciliationReason.Pass,
            "valid recovery reconciliation reason was not PASS");
        Require(composite.Identity.Length == 64,
            "recovery reconciliation identity is not SHA-256 length");
    }

    private static void VerifyDeterministicIdentity()
    {
        var request = CreateFoundationRequest();
        var result = CreateFoundationResult();
        var input = CreateInput(request, result);
        var a = RecoveryReconciliationCompositeBuilder.Build(request, result, input);
        var b = RecoveryReconciliationCompositeBuilder.Build(request, result, input);

        Require(string.Equals(a.Identity, b.Identity, StringComparison.Ordinal),
            "same recovery reconciliation inputs produced different identities");
    }

    private static void VerifyMutationSensitivity()
    {
        var request = CreateFoundationRequest();
        var result = CreateFoundationResult();
        var input = CreateInput(request, result);
        var a = RecoveryReconciliationCompositeBuilder.Build(request, result, input);
        var mutated = input with { Dependency = Dimension("dependency-mutated") };
        var b = RecoveryReconciliationCompositeBuilder.Build(request, result, mutated);

        Require(!string.Equals(a.Identity, b.Identity, StringComparison.Ordinal),
            "material reconciliation evidence mutation did not change identity");
    }

    private static void VerifyMismatchedFoundationResultRejected()
    {
        var request = CreateFoundationRequest();
        var result = CreateFoundationResult();
        var input = CreateInput(request, result) with
        {
            FoundationReconciliationResultIdentity = "reconciliation-result:wrong"
        };

        var validation = RecoveryReconciliationCompositeBuilder.ValidateInput(request, result, input);
        Require(!validation.Success && validation.Reason == RecoveryReconciliationReason.InvalidFoundationResult,
            "mismatched Foundation.Reconciliation result identity was accepted");
    }

    private static void VerifyMissingDimensionEvidenceRejected()
    {
        var request = CreateFoundationRequest();
        var result = CreateFoundationResult();
        var input = CreateInput(request, result) with
        {
            Configuration = Dimension("configuration") with { EvidenceIdentity = string.Empty }
        };

        var validation = RecoveryReconciliationCompositeBuilder.ValidateInput(request, result, input);
        Require(!validation.Success && validation.Reason == RecoveryReconciliationReason.MissingDimensionEvidence,
            "missing required recovery reconciliation evidence was accepted");
    }

    private static void VerifyUncertainCommitFailsClosed()
    {
        var request = CreateFoundationRequest();
        var result = CreateFoundationResult(ReconciliationClassification.UncertainAfterCommit, false, true);
        var composite = RecoveryReconciliationCompositeBuilder.Build(request, result, CreateInput(request, result));

        Require(composite.Classification == RecoveryReconciliationClassification.Uncertain,
            "uncertain commit did not remain UNCERTAIN");
    }

    private static void VerifyCorruptedStateFailsClosed()
    {
        var request = CreateFoundationRequest();
        var result = CreateFoundationResult(ReconciliationClassification.CurrentStateCorrupted, false, true);
        var composite = RecoveryReconciliationCompositeBuilder.Build(request, result, CreateInput(request, result));

        Require(composite.Classification == RecoveryReconciliationClassification.Failed,
            "corrupted authoritative state did not classify FAILED");
    }

    private static void VerifyEvidenceJournalFailureFailsClosed()
    {
        var request = CreateFoundationRequest();
        var result = CreateFoundationResult(ReconciliationClassification.EvidenceJournalInvalid, false, true);
        var composite = RecoveryReconciliationCompositeBuilder.Build(request, result, CreateInput(request, result));

        Require(composite.Classification == RecoveryReconciliationClassification.Failed,
            "invalid evidence journal did not classify FAILED");
    }

    private static void VerifyStaleSecurityContextFailsClosed()
    {
        var request = CreateFoundationRequest();
        var result = CreateFoundationResult();
        var input = CreateInput(request, result) with
        {
            Security = Dimension("security", current: false)
        };

        var composite = RecoveryReconciliationCompositeBuilder.Build(request, result, input);
        Require(composite.Classification == RecoveryReconciliationClassification.Uncertain &&
                composite.Reason == RecoveryReconciliationReason.StaleSecurityContext,
            "stale security context was treated as trusted");
    }

    private static void VerifyUntrustedSecurityContextFailsClosed()
    {
        var request = CreateFoundationRequest();
        var result = CreateFoundationResult();
        var input = CreateInput(request, result) with
        {
            Security = Dimension("security", trusted: false)
        };

        var composite = RecoveryReconciliationCompositeBuilder.Build(request, result, input);
        Require(composite.Classification == RecoveryReconciliationClassification.Uncertain,
            "untrusted security context did not fail closed");
    }

    private static void VerifyDependencyFailureRemainsFailed()
    {
        var request = CreateFoundationRequest();
        var result = CreateFoundationResult();
        var input = CreateInput(request, result) with
        {
            Dependency = Dimension("dependency", RecoveryReconciliationDimensionStatus.Failed)
        };

        var composite = RecoveryReconciliationCompositeBuilder.Build(request, result, input);
        Require(composite.Classification == RecoveryReconciliationClassification.Failed,
            "failed dependency reconciliation was promoted");
    }

    private static void VerifyRestrictionUncertaintyRemainsUncertain()
    {
        var request = CreateFoundationRequest();
        var result = CreateFoundationResult();
        var input = CreateInput(request, result) with
        {
            Restriction = Dimension("restriction", RecoveryReconciliationDimensionStatus.Uncertain)
        };

        var composite = RecoveryReconciliationCompositeBuilder.Build(request, result, input);
        Require(composite.Classification == RecoveryReconciliationClassification.Uncertain,
            "uncertain controlling restriction was promoted");
    }

    private static void VerifyEvidenceProvenanceUncertaintyRemainsUncertain()
    {
        var request = CreateFoundationRequest();
        var result = CreateFoundationResult();
        var input = CreateInput(request, result) with
        {
            EvidenceProvenance = Dimension("evidence-provenance", trusted: false)
        };

        var composite = RecoveryReconciliationCompositeBuilder.Build(request, result, input);
        Require(composite.Classification == RecoveryReconciliationClassification.Uncertain,
            "untrusted evidence provenance was promoted");
    }

    private static void VerifyPartialDimensionRemainsPartial()
    {
        var request = CreateFoundationRequest();
        var result = CreateFoundationResult();
        var input = CreateInput(request, result) with
        {
            DurableState = Dimension("durable-state", RecoveryReconciliationDimensionStatus.Partial)
        };

        var composite = RecoveryReconciliationCompositeBuilder.Build(request, result, input);
        Require(composite.Classification == RecoveryReconciliationClassification.Partial,
            "partial durable-state reconciliation was promoted to complete");
    }

    private static void VerifyUnknownDimensionDoesNotBecomeComplete()
    {
        var request = CreateFoundationRequest();
        var result = CreateFoundationResult();
        var input = CreateInput(request, result) with
        {
            Authority = Dimension("authority", current: false, trusted: false)
        };

        var composite = RecoveryReconciliationCompositeBuilder.Build(request, result, input);
        Require(composite.Classification != RecoveryReconciliationClassification.Complete,
            "unknown/untrusted authority dimension became COMPLETE");
    }

    private static void VerifyCompositeDoesNotGrantRelease()
    {
        var type = typeof(RecoveryReconciliationComposite);
        foreach (var property in type.GetProperties())
        {
            Require(!property.Name.Contains("ReleaseAuthorization", StringComparison.OrdinalIgnoreCase),
                "reconciliation composite exposes release authorization state");
            Require(!property.Name.Contains("LifecycleTransition", StringComparison.OrdinalIgnoreCase),
                "reconciliation composite exposes lifecycle transition state");
        }
    }

    private static void VerifyApplicationNeutrality()
    {
        var refs = typeof(RecoveryReconciliationComposite).Assembly.GetReferencedAssemblies();
        foreach (var reference in refs)
        {
            var name = reference.Name ?? string.Empty;
            Require(!name.Contains("Application", StringComparison.OrdinalIgnoreCase),
                "Application dependency leaked into recovery reconciliation");
            Require(!name.Contains("Trading", StringComparison.OrdinalIgnoreCase),
                "Trading dependency leaked into recovery reconciliation");
            Require(!name.Contains("Web", StringComparison.OrdinalIgnoreCase),
                "Web dependency leaked into recovery reconciliation");
            Require(!name.Contains("SelfAwareness", StringComparison.OrdinalIgnoreCase),
                "Stage 13/FSA dependency leaked into recovery reconciliation");
        }
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }
}
