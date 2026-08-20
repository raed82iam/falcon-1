using System;
using System.Linq;
using System.Reflection;
using Foundation.Recovery;

internal static class Program
{
    private static readonly DateTimeOffset T0 = new(2026, 8, 15, 13, 30, 0, TimeSpan.Zero);

    private static int Main()
    {
        try
        {
            VerifyValidInitiation();
            VerifyInitiationDeterminism();
            VerifyExpiredInitiationRejected();
            VerifyMissingInitiationAuthorityRejected();
            VerifyValidPlanAuthorization();
            VerifyDeniedInitiationBlocksPlan();
            VerifyPlanMutationRejected();
            VerifyPlanAuthorizationDeterminism();
            VerifyValidAttemptLedger();
            VerifyPlanVersionContinuityPreservesAttempts();
            VerifyPlanVersionCannotResetAttempts();
            VerifyPlanVersionCannotExpandCaseCeiling();
            VerifyValidCeilingAdjustment();
            VerifyCeilingAdjustmentRequiresAuthority();
            VerifyValidAttemptAuthorization();
            VerifyAttemptSequenceMismatchRejected();
            VerifyAttemptBudgetExceededRejected();
            VerifyStaleRestrictionRejected();
            VerifyStaleHandoffRejected();
            VerifyStalePlanAuthorizationRejected();
            VerifyDeniedAttemptCannotBeAuthorizedDisposition();
            VerifyAttemptIdentityDeterminism();
            VerifyNoRepairOrReleaseExecutionSurface();
            VerifyApplicationNeutrality();

            Console.WriteLine("STAGE9_WP02_VERIFIER = PASS");
            Console.WriteLine("CHECKS = 24/24");
            Console.WriteLine("RT9_001 = PASS");
            Console.WriteLine("RECOVERY_ATTEMPT_BUDGET_CANNOT_RESET_BY_PLAN_VERSION_CHANGE = PRESERVED");
            Console.WriteLine("PLAN_AUTHORIZATION_IS_EXACT_VERSION_BOUND = PRESERVED");
            Console.WriteLine("REPAIR_OR_RELEASE_EXECUTION_SURFACE = NONE");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("STAGE9_WP02_VERIFIER = FAIL");
            Console.Error.WriteLine(ex.Message);
            return 1;
        }
    }

    private static RecoveryCase CreateCase() => new(
        "recovery-case:wp02:001",
        "foundation-subject:example",
        "guardian:foundation:primary",
        "restriction:example:001",
        "sha256:restriction:001",
        "handoff:example:001",
        "sha256:handoff:001",
        "evidence:trigger:001",
        "evidence:containment:001",
        "recovery-coordinator:001",
        RecoveryCaseState.InitiationPending,
        T0);

    private static RecoveryPlan CreatePlan(RecoveryCase c, int version = 1, int maxAttempts = 3) => new(
        "recovery-plan:001",
        version,
        c.RecoveryCaseId,
        c.Identity,
        "plan-owner:001",
        c.RecoveryCoordinatorIdentity,
        "repair-actor:001",
        "independent-verifier:001",
        "release-authority:001",
        "prerequisites:001",
        "restoration-sequence:001",
        "validation-criteria:001",
        "abort-conditions:001",
        "rollback-direction:001",
        maxAttempts,
        "residual-risk:001",
        T0.AddMinutes(version));

    private static RecoveryInitiationRequest CreateInitiationRequest(RecoveryCase c) => new(
        "recovery-initiation-request:001",
        c.RecoveryCaseId,
        c.Identity,
        "recovery-coordinator:001",
        "action:recovery-initiation",
        c.SubjectId,
        "purpose:controlled-recovery",
        "jurisdiction:foundation",
        "correlation:001",
        "causation:001",
        T0.AddMinutes(2),
        T0.AddHours(1));

    private static RecoveryInitiationDecision CreateInitiationDecision(RecoveryCase c, RecoveryInitiationRequest r, RecoveryAuthorizationOutcome outcome = RecoveryAuthorizationOutcome.Allow) => new(
        "recovery-initiation-decision:001",
        r.Identity,
        c.Identity,
        outcome,
        "authority-decision:aut001:initiation:001",
        "authority-basis:aut001:001",
        "conditions:recovery-initiation:001",
        outcome == RecoveryAuthorizationOutcome.Allow ? "reason:allowed" : "reason:denied",
        T0.AddMinutes(3),
        T0.AddMinutes(50));

    private static RecoveryPlanAuthorizationRequest CreatePlanRequest(RecoveryCase c, RecoveryPlan p, RecoveryInitiationDecision i) => new(
        "recovery-plan-auth-request:001",
        c.RecoveryCaseId,
        c.Identity,
        p.RecoveryPlanId,
        p.Version,
        p.Identity,
        i.Identity,
        "recovery-coordinator:001",
        T0.AddMinutes(4));

    private static RecoveryPlanAuthorizationDecision CreatePlanDecision(RecoveryCase c, RecoveryPlan p, RecoveryPlanAuthorizationRequest r, RecoveryAuthorizationOutcome outcome = RecoveryAuthorizationOutcome.Allow) => new(
        "recovery-plan-auth-decision:001",
        r.Identity,
        c.Identity,
        p.Identity,
        outcome,
        "plan-authority:001",
        "authority-decision:aut001:plan:001",
        "authority-basis:aut001:plan:001",
        "conditions:plan:001",
        outcome == RecoveryAuthorizationOutcome.Allow ? "reason:allowed" : "reason:denied",
        T0.AddMinutes(5));

    private static RecoveryAttemptLedger CreateLedger(RecoveryCase c, int attempts = 0, int ceiling = 3, string authority = "authority-decision:aut001:ceiling:001") => new(
        c.RecoveryCaseId,
        c.Identity,
        attempts,
        ceiling,
        authority);

    private static RecoveryAttemptAuthorizationRequest CreateAttemptRequest(RecoveryCase c, RecoveryPlan p, RecoveryPlanAuthorizationDecision d, RecoveryAttemptLedger l, int? number = null) => new(
        "recovery-attempt-request:001",
        c.Identity,
        p.Identity,
        d.Identity,
        l.Identity,
        number ?? l.CumulativeAttempts + 1,
        c.ControllingRestrictionId,
        c.ControllingRestrictionIntegrityEvidence,
        c.Stage8RecoveryHandoffIdentity,
        T0.AddMinutes(6));

    private static RecoveryAttemptAuthorizationDecision CreateAttemptDecision(RecoveryCase c, RecoveryPlan p, RecoveryAttemptAuthorizationRequest r, RecoveryAuthorizationOutcome outcome = RecoveryAuthorizationOutcome.Allow, RecoveryAttemptDisposition disposition = RecoveryAttemptDisposition.Authorized) => new(
        "recovery-attempt-decision:001",
        r.Identity,
        c.Identity,
        p.Identity,
        r.RequestedAttemptNumber,
        outcome,
        disposition,
        "authority-decision:aut001:attempt:001",
        "authority-basis:aut001:attempt:001",
        outcome == RecoveryAuthorizationOutcome.Allow ? "reason:allowed" : "reason:denied",
        T0.AddMinutes(7));

    private static void VerifyValidInitiation()
    {
        var c = CreateCase(); var r = CreateInitiationRequest(c); var d = CreateInitiationDecision(c, r);
        Require(RecoveryAuthorizationValidator.ValidateInitiation(c, r, d, T0.AddMinutes(10)).Success, "valid initiation rejected");
    }

    private static void VerifyInitiationDeterminism()
    {
        var c = CreateCase(); var r = CreateInitiationRequest(c);
        Require(r.Identity == CreateInitiationRequest(c).Identity, "initiation request identity non-deterministic");
    }

    private static void VerifyExpiredInitiationRejected()
    {
        var c = CreateCase(); var r = CreateInitiationRequest(c); var d = CreateInitiationDecision(c, r);
        var v = RecoveryAuthorizationValidator.ValidateInitiation(c, r, d, T0.AddHours(2));
        Require(!v.Success && v.Reason == RecoveryAuthorizationReason.InvalidRequest, "expired initiation request accepted");
    }

    private static void VerifyMissingInitiationAuthorityRejected()
    {
        var c = CreateCase(); var r = CreateInitiationRequest(c); var d = CreateInitiationDecision(c, r) with { AuthorityDecisionIdentity = string.Empty };
        var v = RecoveryAuthorizationValidator.ValidateInitiation(c, r, d, T0.AddMinutes(10));
        Require(!v.Success && v.Reason == RecoveryAuthorizationReason.InvalidAuthority, "missing AUT-001 binding accepted");
    }

    private static void VerifyValidPlanAuthorization()
    {
        var c = CreateCase(); var p = CreatePlan(c); var ir = CreateInitiationRequest(c); var id = CreateInitiationDecision(c, ir); var pr = CreatePlanRequest(c, p, id); var pd = CreatePlanDecision(c, p, pr);
        Require(RecoveryAuthorizationValidator.ValidatePlanAuthorization(c, p, id, pr, pd).Success, "valid plan authorization rejected");
    }

    private static void VerifyDeniedInitiationBlocksPlan()
    {
        var c = CreateCase(); var p = CreatePlan(c); var ir = CreateInitiationRequest(c); var id = CreateInitiationDecision(c, ir, RecoveryAuthorizationOutcome.Deny); var pr = CreatePlanRequest(c, p, id); var pd = CreatePlanDecision(c, p, pr);
        var v = RecoveryAuthorizationValidator.ValidatePlanAuthorization(c, p, id, pr, pd);
        Require(!v.Success && v.Reason == RecoveryAuthorizationReason.InitiationNotAllowed, "denied initiation authorized a plan");
    }

    private static void VerifyPlanMutationRejected()
    {
        var c = CreateCase(); var p = CreatePlan(c); var ir = CreateInitiationRequest(c); var id = CreateInitiationDecision(c, ir); var pr = CreatePlanRequest(c, p, id); var mutated = p with { MaximumAuthorizedAttempts = 2 }; var pd = CreatePlanDecision(c, p, pr);
        var v = RecoveryAuthorizationValidator.ValidatePlanAuthorization(c, mutated, id, pr, pd);
        Require(!v.Success && v.Reason == RecoveryAuthorizationReason.InvalidPlanBinding, "mutated plan reused prior authorization request");
    }

    private static void VerifyPlanAuthorizationDeterminism()
    {
        var c = CreateCase(); var p = CreatePlan(c); var ir = CreateInitiationRequest(c); var id = CreateInitiationDecision(c, ir); var pr = CreatePlanRequest(c, p, id);
        Require(CreatePlanDecision(c, p, pr).Identity == CreatePlanDecision(c, p, pr).Identity, "plan decision identity non-deterministic");
    }

    private static void VerifyValidAttemptLedger()
    {
        var c = CreateCase(); Require(RecoveryAuthorizationValidator.ValidateLedger(c, CreateLedger(c)).Success, "valid attempt ledger rejected");
    }

    private static void VerifyPlanVersionContinuityPreservesAttempts()
    {
        var c = CreateCase(); var prior = CreateLedger(c, 2, 3); var proposed = CreateLedger(c, 2, 3);
        Require(RecoveryAuthorizationValidator.ValidatePlanVersionContinuity(prior, proposed).Success, "valid cumulative attempt continuity rejected");
    }

    private static void VerifyPlanVersionCannotResetAttempts()
    {
        var c = CreateCase(); var prior = CreateLedger(c, 2, 3); var proposed = CreateLedger(c, 0, 3);
        var v = RecoveryAuthorizationValidator.ValidatePlanVersionContinuity(prior, proposed);
        Require(!v.Success && v.Reason == RecoveryAuthorizationReason.AttemptBudgetResetForbidden, "plan-version churn reset cumulative attempts");
    }

    private static void VerifyPlanVersionCannotExpandCaseCeiling()
    {
        var c = CreateCase(); var prior = CreateLedger(c, 1, 3); var proposed = CreateLedger(c, 1, 9);
        var v = RecoveryAuthorizationValidator.ValidatePlanVersionContinuity(prior, proposed);
        Require(!v.Success && v.Reason == RecoveryAuthorizationReason.AttemptBudgetResetForbidden, "plan-version churn expanded case ceiling");
    }

    private static void VerifyValidCeilingAdjustment()
    {
        var c = CreateCase(); var prior = CreateLedger(c, 2, 3);
        var a = new RecoveryAttemptCeilingAdjustmentDecision("ceiling-adjustment:001", c.Identity, prior.Identity, 5, RecoveryAuthorizationOutcome.Allow, "recovery-authority:001", "authority-decision:aut001:ceiling-adjust:001", "authority-basis:aut001:ceiling-adjust:001", "reason:bounded-extension", T0.AddMinutes(8));
        var result = CreateLedger(c, 2, 5, a.AuthorityDecisionIdentity);
        Require(RecoveryAuthorizationValidator.ValidateCeilingAdjustment(prior, a, result).Success, "valid competent ceiling adjustment rejected");
    }

    private static void VerifyCeilingAdjustmentRequiresAuthority()
    {
        var c = CreateCase(); var prior = CreateLedger(c, 2, 3);
        var a = new RecoveryAttemptCeilingAdjustmentDecision("ceiling-adjustment:001", c.Identity, prior.Identity, 5, RecoveryAuthorizationOutcome.Allow, "recovery-authority:001", string.Empty, "authority-basis:001", "reason:bounded-extension", T0.AddMinutes(8));
        var result = CreateLedger(c, 2, 5, "authority-decision:replacement");
        var v = RecoveryAuthorizationValidator.ValidateCeilingAdjustment(prior, a, result);
        Require(!v.Success && v.Reason == RecoveryAuthorizationReason.InvalidCeilingAdjustment, "ceiling expansion without competent authority accepted");
    }

    private static void VerifyValidAttemptAuthorization()
    {
        var c = CreateCase(); var p = CreatePlan(c); var ir = CreateInitiationRequest(c); var id = CreateInitiationDecision(c, ir); var pr = CreatePlanRequest(c, p, id); var pd = CreatePlanDecision(c, p, pr); var l = CreateLedger(c); var ar = CreateAttemptRequest(c, p, pd, l); var ad = CreateAttemptDecision(c, p, ar);
        Require(RecoveryAuthorizationValidator.ValidateAttempt(c, p, pd, l, ar, ad).Success, "valid attempt authorization rejected");
    }

    private static void VerifyAttemptSequenceMismatchRejected()
    {
        var c = CreateCase(); var p = CreatePlan(c); var ir = CreateInitiationRequest(c); var id = CreateInitiationDecision(c, ir); var pr = CreatePlanRequest(c, p, id); var pd = CreatePlanDecision(c, p, pr); var l = CreateLedger(c, 1, 3); var ar = CreateAttemptRequest(c, p, pd, l, 3); var ad = CreateAttemptDecision(c, p, ar);
        var v = RecoveryAuthorizationValidator.ValidateAttempt(c, p, pd, l, ar, ad);
        Require(!v.Success && v.Reason == RecoveryAuthorizationReason.AttemptSequenceMismatch, "non-sequential attempt accepted");
    }

    private static void VerifyAttemptBudgetExceededRejected()
    {
        var c = CreateCase(); var p = CreatePlan(c); var ir = CreateInitiationRequest(c); var id = CreateInitiationDecision(c, ir); var pr = CreatePlanRequest(c, p, id); var pd = CreatePlanDecision(c, p, pr); var l = CreateLedger(c, 3, 3); var ar = CreateAttemptRequest(c, p, pd, l, 4); var ad = CreateAttemptDecision(c, p, ar);
        var v = RecoveryAuthorizationValidator.ValidateAttempt(c, p, pd, l, ar, ad);
        Require(!v.Success && v.Reason == RecoveryAuthorizationReason.AttemptBudgetExceeded, "attempt above budget accepted");
    }

    private static void VerifyStaleRestrictionRejected()
    {
        var c = CreateCase(); var p = CreatePlan(c); var ir = CreateInitiationRequest(c); var id = CreateInitiationDecision(c, ir); var pr = CreatePlanRequest(c, p, id); var pd = CreatePlanDecision(c, p, pr); var l = CreateLedger(c); var ar = CreateAttemptRequest(c, p, pd, l) with { CurrentControllingRestrictionIntegrityEvidence = "sha256:stale" }; var ad = CreateAttemptDecision(c, p, ar);
        var v = RecoveryAuthorizationValidator.ValidateAttempt(c, p, pd, l, ar, ad);
        Require(!v.Success && v.Reason == RecoveryAuthorizationReason.InvalidRestrictionBinding, "stale restriction accepted");
    }

    private static void VerifyStaleHandoffRejected()
    {
        var c = CreateCase(); var p = CreatePlan(c); var ir = CreateInitiationRequest(c); var id = CreateInitiationDecision(c, ir); var pr = CreatePlanRequest(c, p, id); var pd = CreatePlanDecision(c, p, pr); var l = CreateLedger(c); var ar = CreateAttemptRequest(c, p, pd, l) with { CurrentRecoveryHandoffIdentity = "sha256:stale-handoff" }; var ad = CreateAttemptDecision(c, p, ar);
        var v = RecoveryAuthorizationValidator.ValidateAttempt(c, p, pd, l, ar, ad);
        Require(!v.Success && v.Reason == RecoveryAuthorizationReason.InvalidHandoffBinding, "stale handoff accepted");
    }

    private static void VerifyStalePlanAuthorizationRejected()
    {
        var c = CreateCase(); var p = CreatePlan(c); var ir = CreateInitiationRequest(c); var id = CreateInitiationDecision(c, ir); var pr = CreatePlanRequest(c, p, id); var pd = CreatePlanDecision(c, p, pr) with { RecoveryPlanIdentity = new string('A', 64) }; var l = CreateLedger(c); var ar = CreateAttemptRequest(c, p, pd, l); var ad = CreateAttemptDecision(c, p, ar);
        var v = RecoveryAuthorizationValidator.ValidateAttempt(c, p, pd, l, ar, ad);
        Require(!v.Success && v.Reason == RecoveryAuthorizationReason.PlanMutation, "stale/mismatched plan authorization accepted");
    }

    private static void VerifyDeniedAttemptCannotBeAuthorizedDisposition()
    {
        var c = CreateCase(); var p = CreatePlan(c); var ir = CreateInitiationRequest(c); var id = CreateInitiationDecision(c, ir); var pr = CreatePlanRequest(c, p, id); var pd = CreatePlanDecision(c, p, pr); var l = CreateLedger(c); var ar = CreateAttemptRequest(c, p, pd, l); var ad = CreateAttemptDecision(c, p, ar, RecoveryAuthorizationOutcome.Deny, RecoveryAttemptDisposition.Authorized);
        var v = RecoveryAuthorizationValidator.ValidateAttempt(c, p, pd, l, ar, ad);
        Require(!v.Success && v.Reason == RecoveryAuthorizationReason.InvalidDecision, "DENY decision carried Authorized disposition");
    }

    private static void VerifyAttemptIdentityDeterminism()
    {
        var c = CreateCase(); var p = CreatePlan(c); var ir = CreateInitiationRequest(c); var id = CreateInitiationDecision(c, ir); var pr = CreatePlanRequest(c, p, id); var pd = CreatePlanDecision(c, p, pr); var l = CreateLedger(c); var ar = CreateAttemptRequest(c, p, pd, l);
        Require(CreateAttemptDecision(c, p, ar).Identity == CreateAttemptDecision(c, p, ar).Identity, "attempt decision identity non-deterministic");
    }

    private static void VerifyNoRepairOrReleaseExecutionSurface()
    {
        var forbidden = new[] { "ExecuteRepair", "Repair", "Release", "RestoreTrust", "Transition", "Reintroduce", "ControlledRevival" };
        foreach (var type in typeof(RecoveryAuthorizationValidator).Assembly.GetExportedTypes())
            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly))
                Require(!forbidden.Contains(method.Name, StringComparer.Ordinal), "forbidden recovery execution surface: " + type.FullName + "." + method.Name);
    }

    private static void VerifyApplicationNeutrality()
    {
        var refs = typeof(RecoveryAuthorizationValidator).Assembly.GetReferencedAssemblies().Select(a => a.Name ?? string.Empty).ToArray();
        Require(!refs.Any(r => r.Contains("Application", StringComparison.OrdinalIgnoreCase)), "Application dependency leaked into recovery authorization");
        Require(!refs.Any(r => r.Contains("Trading", StringComparison.OrdinalIgnoreCase)), "Trading dependency leaked into recovery authorization");
        Require(!refs.Any(r => r.Contains("Web", StringComparison.OrdinalIgnoreCase)), "Web dependency leaked into recovery authorization");
        Require(!refs.Any(r => r.Contains("SelfAwareness", StringComparison.OrdinalIgnoreCase)), "Stage 13/FSA dependency leaked into recovery authorization");
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
