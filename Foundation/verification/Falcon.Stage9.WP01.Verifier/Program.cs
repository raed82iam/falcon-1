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
            VerifyValidCaseAndPlan();
            VerifyDeterministicCaseIdentity();
            VerifyDeterministicPlanIdentity();
            VerifyCaseMutationSensitivity();
            VerifyPlanMutationSensitivity();
            VerifyMissingRestrictionBindingRejected();
            VerifyMissingHandoffBindingRejected();
            VerifyMissingAttemptBoundRejected();
            VerifyCaseBindingMismatchRejected();
            VerifyRepairVerifierCollisionRejected();
            VerifyRepairReleaseCollisionRejected();
            VerifySubjectVerifierCollisionRejected();
            VerifyGuardianVerifierCollisionRejected();
            VerifyVerifierReleaseAuthorityCollisionRejected();
            VerifyNoAuthorityOrExecutionSurface();
            VerifyApplicationNeutrality();

            Console.WriteLine("STAGE9_WP01_VERIFIER = PASS");
            Console.WriteLine("CHECKS = 16/16");
            Console.WriteLine("ACR9_001 = PASS");
            Console.WriteLine("PLAN_DEFINED_NOT_AUTHORIZED = PRESERVED");
            Console.WriteLine("REPAIR_OR_RELEASE_EXECUTION_SURFACE = NONE");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("STAGE9_WP01_VERIFIER = FAIL");
            Console.Error.WriteLine(ex.Message);
            return 1;
        }
    }

    private static RecoveryCase CreateCase() =>
        new(
            "recovery-case:stage9:wp01:001",
            "foundation-subject:example",
            "guardian:foundation:primary",
            "restriction:stage8:example",
            "sha256:restriction-integrity:001",
            "handoff:stage8:wp09:001",
            "sha256:stage8-handoff:001",
            "evidence:trigger:001",
            "evidence:containment:001",
            "recovery-coordinator:foundation:001",
            RecoveryCaseState.InitiationPending,
            new DateTimeOffset(2026, 8, 15, 13, 0, 0, TimeSpan.Zero));

    private static RecoveryPlan CreatePlan(RecoveryCase recoveryCase) =>
        new(
            "recovery-plan:stage9:wp01:001",
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

    private static void VerifyValidCaseAndPlan()
    {
        var recoveryCase = CreateCase();
        var caseResult = RecoveryPrimitiveValidator.ValidateCase(recoveryCase);
        Require(caseResult.Success, "valid RecoveryCase rejected: " + caseResult.Reason);
        Require(recoveryCase.Identity.Length == 64, "RecoveryCase identity is not SHA-256 length");

        var plan = CreatePlan(recoveryCase);
        var planResult = RecoveryPrimitiveValidator.ValidatePlan(recoveryCase, plan);
        Require(planResult.Success, "valid RecoveryPlan rejected: " + planResult.Reason);
        Require(plan.Identity.Length == 64, "RecoveryPlan identity is not SHA-256 length");
    }

    private static void VerifyDeterministicCaseIdentity()
    {
        Require(string.Equals(CreateCase().Identity, CreateCase().Identity, StringComparison.Ordinal),
            "identical RecoveryCase inputs produced different identities");
    }

    private static void VerifyDeterministicPlanIdentity()
    {
        var recoveryCase = CreateCase();
        Require(string.Equals(CreatePlan(recoveryCase).Identity, CreatePlan(recoveryCase).Identity, StringComparison.Ordinal),
            "identical RecoveryPlan inputs produced different identities");
    }

    private static void VerifyCaseMutationSensitivity()
    {
        var a = CreateCase();
        var b = a with { ControllingRestrictionIntegrityEvidence = "sha256:restriction-integrity:002" };
        Require(!string.Equals(a.Identity, b.Identity, StringComparison.Ordinal),
            "material RecoveryCase mutation did not change identity");
    }

    private static void VerifyPlanMutationSensitivity()
    {
        var recoveryCase = CreateCase();
        var a = CreatePlan(recoveryCase);
        var b = a with { MaximumAuthorizedAttempts = a.MaximumAuthorizedAttempts + 1 };
        Require(!string.Equals(a.Identity, b.Identity, StringComparison.Ordinal),
            "material RecoveryPlan mutation did not change identity");
    }

    private static void VerifyMissingRestrictionBindingRejected()
    {
        var result = RecoveryPrimitiveValidator.ValidateCase(CreateCase() with { ControllingRestrictionId = string.Empty });
        Require(!result.Success && result.Reason == RecoveryPrimitiveReason.InvalidRestrictionId,
            "missing controlling restriction was not rejected exactly");
    }

    private static void VerifyMissingHandoffBindingRejected()
    {
        var result = RecoveryPrimitiveValidator.ValidateCase(CreateCase() with { Stage8RecoveryHandoffIdentity = string.Empty });
        Require(!result.Success && result.Reason == RecoveryPrimitiveReason.InvalidHandoffIdentity,
            "missing Stage 8 handoff binding was not rejected exactly");
    }

    private static void VerifyMissingAttemptBoundRejected()
    {
        var recoveryCase = CreateCase();
        var result = RecoveryPrimitiveValidator.ValidatePlan(recoveryCase, CreatePlan(recoveryCase) with { MaximumAuthorizedAttempts = 0 });
        Require(!result.Success && result.Reason == RecoveryPrimitiveReason.InvalidAttemptBound,
            "missing/zero attempt bound was not rejected exactly");
    }

    private static void VerifyCaseBindingMismatchRejected()
    {
        var recoveryCase = CreateCase();
        var result = RecoveryPrimitiveValidator.ValidatePlan(recoveryCase, CreatePlan(recoveryCase) with { RecoveryCaseIdentity = new string('A', 64) });
        Require(!result.Success && result.Reason == RecoveryPrimitiveReason.InvalidCaseBinding,
            "mismatched RecoveryCase binding was accepted");
    }

    private static void VerifyRepairVerifierCollisionRejected()
    {
        var recoveryCase = CreateCase();
        var plan = CreatePlan(recoveryCase);
        var result = RecoveryPrimitiveValidator.ValidatePlan(recoveryCase, plan with { IndependentRecoveryVerifierIdentity = plan.RepairActorIdentity });
        Require(!result.Success && result.Reason == RecoveryPrimitiveReason.RepairActorVerifierCollision,
            "repair actor / independent verifier collision was accepted");
    }

    private static void VerifyRepairReleaseCollisionRejected()
    {
        var recoveryCase = CreateCase();
        var plan = CreatePlan(recoveryCase);
        var result = RecoveryPrimitiveValidator.ValidatePlan(recoveryCase, plan with { DeclaredReleaseAuthorityIdentity = plan.RepairActorIdentity });
        Require(!result.Success && result.Reason == RecoveryPrimitiveReason.RepairActorReleaseAuthorityCollision,
            "repair actor / release authority collision was accepted");
    }

    private static void VerifySubjectVerifierCollisionRejected()
    {
        var recoveryCase = CreateCase();
        var plan = CreatePlan(recoveryCase);
        var result = RecoveryPrimitiveValidator.ValidatePlan(recoveryCase, plan with { IndependentRecoveryVerifierIdentity = recoveryCase.SubjectId });
        Require(!result.Success && result.Reason == RecoveryPrimitiveReason.SubjectVerifierCollision,
            "subject / independent verifier collision was accepted");
    }

    private static void VerifyGuardianVerifierCollisionRejected()
    {
        var recoveryCase = CreateCase();
        var plan = CreatePlan(recoveryCase);
        var result = RecoveryPrimitiveValidator.ValidatePlan(recoveryCase, plan with { IndependentRecoveryVerifierIdentity = recoveryCase.GuardianIdentity });
        Require(!result.Success && result.Reason == RecoveryPrimitiveReason.GuardianVerifierCollision,
            "Guardian / independent verifier collision was accepted");
    }

    private static void VerifyVerifierReleaseAuthorityCollisionRejected()
    {
        var recoveryCase = CreateCase();
        var plan = CreatePlan(recoveryCase);
        var result = RecoveryPrimitiveValidator.ValidatePlan(recoveryCase, plan with { DeclaredReleaseAuthorityIdentity = plan.IndependentRecoveryVerifierIdentity });
        Require(!result.Success && result.Reason == RecoveryPrimitiveReason.VerifierReleaseAuthorityCollision,
            "ACR-9-001 verifier / release authority collision was accepted");
    }

    private static void VerifyNoAuthorityOrExecutionSurface()
    {
        var forbidden = new[]
        {
            "Authorize", "GrantAuthority", "AuthorizePlan", "AuthorizeRestoration", "ExecuteRepair",
            "Repair", "Release", "RestoreTrust", "Transition", "Reintroduce", "ControlledRevival"
        };

        var assembly = typeof(RecoveryCase).Assembly;
        foreach (var type in assembly.GetExportedTypes())
        {
            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly))
            {
                Require(!forbidden.Contains(method.Name, StringComparer.Ordinal),
                    "forbidden WP-01 authority/execution surface: " + type.FullName + "." + method.Name);
            }
        }
    }

    private static void VerifyApplicationNeutrality()
    {
        var refs = typeof(RecoveryCase).Assembly.GetReferencedAssemblies()
            .Select(a => a.Name ?? string.Empty)
            .ToArray();

        Require(!refs.Any(r => r.Contains("Application", StringComparison.OrdinalIgnoreCase)),
            "Application dependency leaked into Foundation.Recovery");
        Require(!refs.Any(r => r.Contains("Trading", StringComparison.OrdinalIgnoreCase)),
            "Trading dependency leaked into Foundation.Recovery");
        Require(!refs.Any(r => r.Contains("Web", StringComparison.OrdinalIgnoreCase)),
            "Web dependency leaked into Foundation.Recovery");
        Require(!refs.Any(r => r.Contains("SelfAwareness", StringComparison.OrdinalIgnoreCase)),
            "Stage 13/FSA self-awareness dependency leaked into Foundation.Recovery");
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }
}
