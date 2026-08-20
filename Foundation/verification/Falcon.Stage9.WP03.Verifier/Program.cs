using System;
using System.Linq;
using System.Reflection;
using Foundation.Recovery;

internal static class Program
{
    private static int Main()
    {
        try
        {
            VerifyValidCompletedOutcome();
            VerifyDeterministicIdentity();
            VerifyMutationSensitivity();
            VerifyUnauthorizedPlanRejected();
            VerifyUnauthorizedAttemptRejected();
            VerifyAttemptBindingMismatchRejected();
            VerifyRepairActorMismatchRejected();
            VerifyMissingRestorationEvidenceRejected();
            VerifyMissingChangedStateEvidenceRejected();
            VerifyFailedWithoutEvidenceRejected();
            VerifyPartialRemainsExplicit();
            VerifyUnknownLossCannotBeComplete();
            VerifyRollbackEvidenceRequiredWhenApplicable();
            VerifyRollbackEvidenceRejectedWhenNotApplicable();
            VerifyLossDeclarationEvidenceRequired();
            VerifyEvidencePreservationRequired();
            VerifyNoIndependentValidationSurface();
            VerifyNoReleaseOrLifecycleExecutionSurface();
            VerifyApplicationNeutrality();

            Console.WriteLine("STAGE9_WP03_VERIFIER = PASS");
            Console.WriteLine("CHECKS = 19/19");
            Console.WriteLine("REPAIR_ACTOR_SELF_CERTIFICATION = DENIED");
            Console.WriteLine("PARTIAL_RESTORATION_REMAINS_EXPLICIT = PRESERVED");
            Console.WriteLine("RESTORATION_REPORTED != RECOVERY_VALIDATED");
            Console.WriteLine("REPAIR_OR_RELEASE_EXECUTION_SURFACE = NONE");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("STAGE9_WP03_VERIFIER = FAIL");
            Console.Error.WriteLine(ex.Message);
            return 1;
        }
    }

    private static RecoveryCase CreateCase() =>
        new(
            "recovery-case:stage9:wp03:001",
            "foundation-subject:example",
            "guardian:foundation:primary",
            "restriction:stage8:example",
            "sha256:restriction-integrity:001",
            "handoff:stage8:wp09:001",
            "sha256:stage8-handoff:001",
            "evidence:trigger:001",
            "evidence:containment:001",
            "recovery-coordinator:foundation:001",
            RecoveryCaseState.PlanAuthorized,
            new DateTimeOffset(2026, 8, 15, 13, 0, 0, TimeSpan.Zero));

    private static RecoveryPlan CreatePlan(RecoveryCase recoveryCase) =>
        new(
            "recovery-plan:stage9:wp03:001",
            1,
            recoveryCase.RecoveryCaseId,
            recoveryCase.Identity,
            "recovery-plan-owner:foundation:001",
            recoveryCase.RecoveryCoordinatorIdentity,
            "repair-actor:foundation:001",
            "independent-verifier:foundation:001",
            "release-authority:foundation:001",
            "prerequisites:set:001",
            "restoration-sequence:set:001",
            "validation-criteria:set:001",
            "abort-conditions:set:001",
            "rollback-direction:001",
            3,
            "residual-risk-requirements:set:001",
            new DateTimeOffset(2026, 8, 15, 13, 1, 0, TimeSpan.Zero));

    private static RecoveryPlanAuthorizationDecision CreatePlanAuthorization(RecoveryCase recoveryCase, RecoveryPlan plan) =>
        new(
            "decision:plan-authorization:001",
            "request-identity:plan-authorization:001",
            recoveryCase.Identity,
            plan.Identity,
            RecoveryAuthorizationOutcome.Allow,
            "authority-actor:foundation:001",
            "authority-decision:plan:001",
            "authority-basis:aut001:001",
            "conditions:plan:001",
            "reason:plan-authorized:001",
            new DateTimeOffset(2026, 8, 15, 13, 2, 0, TimeSpan.Zero));

    private static RecoveryAttemptAuthorizationDecision CreateAttemptAuthorization(RecoveryCase recoveryCase, RecoveryPlan plan) =>
        new(
            "decision:attempt-authorization:001",
            "request-identity:attempt:001",
            recoveryCase.Identity,
            plan.Identity,
            1,
            RecoveryAuthorizationOutcome.Allow,
            RecoveryAttemptDisposition.Authorized,
            "authority-decision:attempt:001",
            "authority-basis:aut001:001",
            "reason:attempt-authorized:001",
            new DateTimeOffset(2026, 8, 15, 13, 3, 0, TimeSpan.Zero));

    private static RestorationOutcomeRecord CreateOutcome(
        RecoveryCase recoveryCase,
        RecoveryPlan plan,
        RecoveryPlanAuthorizationDecision planAuthorization,
        RecoveryAttemptAuthorizationDecision attemptAuthorization) =>
        new(
            "restoration-action:001",
            recoveryCase.Identity,
            plan.Identity,
            planAuthorization.Identity,
            attemptAuthorization.Identity,
            attemptAuthorization.AttemptNumber,
            plan.RepairActorIdentity,
            RestorationOutcomeKind.Completed,
            "evidence:restoration-action:001",
            "evidence:artifact-change:001",
            "evidence:config-change:001",
            "evidence:state-change:001",
            "evidence:dependency-change:001",
            false,
            string.Empty,
            string.Empty,
            RestorationLossDeclaration.None,
            "evidence:loss-declaration:001",
            true,
            new DateTimeOffset(2026, 8, 15, 13, 4, 0, TimeSpan.Zero));

    private static (RecoveryCase Case, RecoveryPlan Plan, RecoveryPlanAuthorizationDecision PlanAuthorization, RecoveryAttemptAuthorizationDecision AttemptAuthorization, RestorationOutcomeRecord Outcome) CreateValidGraph()
    {
        var recoveryCase = CreateCase();
        var plan = CreatePlan(recoveryCase);
        var planAuthorization = CreatePlanAuthorization(recoveryCase, plan);
        var attemptAuthorization = CreateAttemptAuthorization(recoveryCase, plan);
        var outcome = CreateOutcome(recoveryCase, plan, planAuthorization, attemptAuthorization);
        return (recoveryCase, plan, planAuthorization, attemptAuthorization, outcome);
    }

    private static RestorationOutcomeValidation Validate((RecoveryCase Case, RecoveryPlan Plan, RecoveryPlanAuthorizationDecision PlanAuthorization, RecoveryAttemptAuthorizationDecision AttemptAuthorization, RestorationOutcomeRecord Outcome) graph) =>
        RestorationOutcomeValidator.Validate(graph.Case, graph.Plan, graph.PlanAuthorization, graph.AttemptAuthorization, graph.Outcome);

    private static RestorationOutcomeValidation ValidateWithOutcome(
        (RecoveryCase Case, RecoveryPlan Plan, RecoveryPlanAuthorizationDecision PlanAuthorization, RecoveryAttemptAuthorizationDecision AttemptAuthorization, RestorationOutcomeRecord Outcome) graph,
        RestorationOutcomeRecord outcome) =>
        RestorationOutcomeValidator.Validate(graph.Case, graph.Plan, graph.PlanAuthorization, graph.AttemptAuthorization, outcome);

    private static void VerifyValidCompletedOutcome()
    {
        var graph = CreateValidGraph();
        var result = Validate(graph);
        Require(result.Success, "valid restoration outcome rejected: " + result.Reason);
        Require(graph.Outcome.Identity.Length == 64, "restoration outcome identity is not SHA-256 length");
    }

    private static void VerifyDeterministicIdentity()
    {
        var a = CreateValidGraph().Outcome.Identity;
        var b = CreateValidGraph().Outcome.Identity;
        Require(string.Equals(a, b, StringComparison.Ordinal), "identical restoration inputs produced different identities");
    }

    private static void VerifyMutationSensitivity()
    {
        var graph = CreateValidGraph();
        var mutated = graph.Outcome with { ChangedStateEvidenceIdentity = "evidence:state-change:002" };
        Require(!string.Equals(graph.Outcome.Identity, mutated.Identity, StringComparison.Ordinal), "material restoration mutation did not change identity");
    }

    private static void VerifyUnauthorizedPlanRejected()
    {
        var graph = CreateValidGraph();
        var denied = graph.PlanAuthorization with { Outcome = RecoveryAuthorizationOutcome.Deny };
        var outcome = graph.Outcome with { PlanAuthorizationDecisionIdentity = denied.Identity };
        var result = RestorationOutcomeValidator.Validate(graph.Case, graph.Plan, denied, graph.AttemptAuthorization, outcome);
        Require(!result.Success && result.Reason == RestorationOutcomeReason.InvalidPlanBinding, "denied plan authorization was accepted");
    }

    private static void VerifyUnauthorizedAttemptRejected()
    {
        var graph = CreateValidGraph();
        var denied = graph.AttemptAuthorization with { Outcome = RecoveryAuthorizationOutcome.Deny, Disposition = RecoveryAttemptDisposition.Aborted };
        var outcome = graph.Outcome with { AttemptAuthorizationDecisionIdentity = denied.Identity };
        var result = RestorationOutcomeValidator.Validate(graph.Case, graph.Plan, graph.PlanAuthorization, denied, outcome);
        Require(!result.Success && result.Reason == RestorationOutcomeReason.InvalidAttemptBinding, "denied restoration attempt was accepted");
    }

    private static void VerifyAttemptBindingMismatchRejected()
    {
        var graph = CreateValidGraph();
        var result = ValidateWithOutcome(graph, graph.Outcome with { AttemptAuthorizationDecisionIdentity = new string('A', 64) });
        Require(!result.Success && result.Reason == RestorationOutcomeReason.InvalidAttemptBinding, "mismatched attempt authorization identity was accepted");
    }

    private static void VerifyRepairActorMismatchRejected()
    {
        var graph = CreateValidGraph();
        var result = ValidateWithOutcome(graph, graph.Outcome with { RepairActorIdentity = "repair-actor:other:001" });
        Require(!result.Success && result.Reason == RestorationOutcomeReason.RepairActorMismatch, "wrong repair actor was accepted");
    }

    private static void VerifyMissingRestorationEvidenceRejected()
    {
        var graph = CreateValidGraph();
        var result = ValidateWithOutcome(graph, graph.Outcome with { RestorationActionEvidenceIdentity = string.Empty });
        Require(!result.Success && result.Reason == RestorationOutcomeReason.MissingRestorationEvidence, "missing restoration action evidence was accepted");
    }

    private static void VerifyMissingChangedStateEvidenceRejected()
    {
        var graph = CreateValidGraph();
        var outcome = graph.Outcome with
        {
            ChangedArtifactEvidenceIdentity = string.Empty,
            ChangedConfigurationEvidenceIdentity = string.Empty,
            ChangedStateEvidenceIdentity = string.Empty,
            ChangedDependencyEvidenceIdentity = string.Empty
        };
        var result = ValidateWithOutcome(graph, outcome);
        Require(!result.Success && result.Reason == RestorationOutcomeReason.MissingChangedStateEvidence, "completed restoration without changed-state evidence was accepted");
    }

    private static void VerifyFailedWithoutEvidenceRejected()
    {
        var graph = CreateValidGraph();
        var outcome = graph.Outcome with
        {
            Outcome = RestorationOutcomeKind.Failed,
            ChangedArtifactEvidenceIdentity = string.Empty,
            ChangedConfigurationEvidenceIdentity = string.Empty,
            ChangedStateEvidenceIdentity = string.Empty,
            ChangedDependencyEvidenceIdentity = string.Empty
        };
        var result = ValidateWithOutcome(graph, outcome);
        Require(!result.Success && result.Reason == RestorationOutcomeReason.MissingFailureEvidence, "failed restoration without failure evidence was accepted");
    }

    private static void VerifyPartialRemainsExplicit()
    {
        var graph = CreateValidGraph();
        var partial = graph.Outcome with { Outcome = RestorationOutcomeKind.Partial, LossDeclaration = RestorationLossDeclaration.CapabilityLoss };
        var result = ValidateWithOutcome(graph, partial);
        Require(result.Success, "explicit partial restoration was rejected: " + result.Reason);
        Require(partial.Outcome == RestorationOutcomeKind.Partial, "partial restoration was silently promoted to complete");
    }

    private static void VerifyUnknownLossCannotBeComplete()
    {
        var graph = CreateValidGraph();
        var result = ValidateWithOutcome(graph, graph.Outcome with { LossDeclaration = RestorationLossDeclaration.Unknown });
        Require(!result.Success && result.Reason == RestorationOutcomeReason.PartialReportedAsComplete, "unknown-loss restoration was accepted as complete");
    }

    private static void VerifyRollbackEvidenceRequiredWhenApplicable()
    {
        var graph = CreateValidGraph();
        var result = ValidateWithOutcome(graph, graph.Outcome with { RollbackApplicable = true });
        Require(!result.Success && result.Reason == RestorationOutcomeReason.InvalidRollbackEvidence, "rollback-applicable outcome without rollback evidence was accepted");
    }

    private static void VerifyRollbackEvidenceRejectedWhenNotApplicable()
    {
        var graph = CreateValidGraph();
        var result = ValidateWithOutcome(graph, graph.Outcome with { RollbackActionEvidenceIdentity = "evidence:rollback-action:001" });
        Require(!result.Success && result.Reason == RestorationOutcomeReason.InvalidRollbackEvidence, "rollback evidence with rollback-not-applicable was accepted");
    }

    private static void VerifyLossDeclarationEvidenceRequired()
    {
        var graph = CreateValidGraph();
        var result = ValidateWithOutcome(graph, graph.Outcome with { LossDeclarationEvidenceIdentity = string.Empty });
        Require(!result.Success && result.Reason == RestorationOutcomeReason.MissingLossDeclaration, "missing loss declaration evidence was accepted");
    }

    private static void VerifyEvidencePreservationRequired()
    {
        var graph = CreateValidGraph();
        var result = ValidateWithOutcome(graph, graph.Outcome with { EvidencePreserved = false });
        Require(!result.Success && result.Reason == RestorationOutcomeReason.EvidenceNotPreserved, "unpreserved restoration evidence was accepted");
    }

    private static void VerifyNoIndependentValidationSurface()
    {
        var assembly = typeof(RestorationOutcomeRecord).Assembly;
        var restorationTypes = assembly.GetExportedTypes()
            .Where(type => type.Name.Contains("Restoration", StringComparison.Ordinal))
            .ToArray();

        foreach (var type in restorationTypes)
        {
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
            {
                Require(!property.Name.Contains("IndependentValidation", StringComparison.OrdinalIgnoreCase),
                    "repair/restoration surface exposes independent validation field: " + type.FullName + "." + property.Name);
                Require(!property.Name.Contains("ReleaseAuthorization", StringComparison.OrdinalIgnoreCase),
                    "repair/restoration surface exposes release authorization field: " + type.FullName + "." + property.Name);
            }
        }
    }

    private static void VerifyNoReleaseOrLifecycleExecutionSurface()
    {
        var forbidden = new[] { "Release", "Reintroduce", "TransitionLifecycle", "RestoreAuthority", "ValidateRecovery", "CertifyRecovery" };
        var assembly = typeof(RestorationOutcomeRecord).Assembly;
        foreach (var type in assembly.GetExportedTypes().Where(t => t.Name.Contains("Restoration", StringComparison.Ordinal)))
        {
            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly))
            {
                Require(!forbidden.Contains(method.Name, StringComparer.Ordinal), "forbidden restoration authority/execution surface: " + type.FullName + "." + method.Name);
            }
        }
    }

    private static void VerifyApplicationNeutrality()
    {
        var refs = typeof(RestorationOutcomeRecord).Assembly.GetReferencedAssemblies().Select(a => a.Name ?? string.Empty).ToArray();
        Require(!refs.Any(r => r.Contains("Trading", StringComparison.OrdinalIgnoreCase)), "Trading dependency leaked into restoration boundary");
        Require(!refs.Any(r => r.Contains("Web", StringComparison.OrdinalIgnoreCase)), "Web dependency leaked into restoration boundary");
        Require(!refs.Any(r => r.Contains("SelfAwareness", StringComparison.OrdinalIgnoreCase)), "Stage 13/FSA dependency leaked into restoration boundary");
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }
}
