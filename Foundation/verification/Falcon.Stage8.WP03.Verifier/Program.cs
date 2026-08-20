using System;
using Foundation.Guardian;

internal static class Program
{
    private static int Main()
    {
        try
        {
            VerifyValidRestriction();
            VerifySourceDecisionBinding();
            VerifyTargetBinding();
            VerifyScopeBinding();
            VerifySeverityBinding();
            VerifyActionBinding();
            VerifyRestartPersistenceRequired();
            VerifySelfReleaseForbidden();
            VerifyReviewDeadlineDoesNotRelease();
            VerifyNoDeadlineRemainsActive();
            VerifyBeforeEffectiveFails();
            VerifyMissingEvidenceRejected();
            VerifyInvalidSeverityRejected();
            VerifyNonRestrictiveDecisionRejected();
            VerifyRestrictionIdentityDeterministic();
            VerifyRestrictionIdentityMutationSensitive();
            VerifyDecisionMutationInvalidatesBinding();
            VerifyAuthorityBinding();
            VerifyPolicyBinding();
            VerifyStage9BoundaryPreserved();

            Console.WriteLine("STAGE8_WP03_VERIFIER = PASS");
            Console.WriteLine("CHECKS = 20/20");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("STAGE8_WP03_VERIFIER = FAIL");
            Console.Error.WriteLine(ex.Message);
            return 1;
        }
    }

    private static GuardianProtectiveDecision Decision()
        => new(
            "guardian-decision:stage8:wp03:001",
            "foundation-subject:example",
            GuardianScopeKind.FoundationSubsystem,
            "foundation-scope:example",
            GuardianProtectiveMode.Restricted,
            GuardianProtectiveAction.Restrict,
            GuardianConsequenceClass.High,
            "TECHNICAL_PROTECTIVE_RESTRICTION",
            "evidence:stage8:wp03:001",
            "authority:guardian:approved",
            "policy:AUT-002:v1.0",
            "Material technical protection condition requires bounded restriction.",
            "Independent governed evidence and Stage 9 release process are required.",
            new DateTimeOffset(2026, 8, 14, 19, 30, 0, TimeSpan.Zero));

    private static GuardianProtectiveRestriction Restriction(GuardianProtectiveDecision? decision = null)
    {
        var source = decision ?? Decision();
        return GuardianProtectiveRestrictionRuntime.CreateFromDecision(
            source,
            "guardian-restriction:stage8:wp03:001",
            new DateTimeOffset(2026, 8, 14, 19, 31, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 8, 15, 19, 31, 0, TimeSpan.Zero));
    }

    private static void VerifyValidRestriction()
    {
        var d = Decision(); var r = Restriction(d);
        Require(GuardianProtectiveRestrictionRuntime.Validate(r, d).Success, "valid restriction failed validation");
    }

    private static void VerifySourceDecisionBinding()
    {
        var d = Decision(); var r = Restriction(d) with { SourceDecisionIdentity = new string('A', 64) };
        Require(GuardianProtectiveRestrictionRuntime.Validate(r, d).Reason == "SOURCE_DECISION_IDENTITY_MISMATCH", "source identity mismatch not rejected");
    }

    private static void VerifyTargetBinding()
    {
        var d = Decision(); var r = Restriction(d) with { TargetId = "foundation-subject:other" };
        Require(GuardianProtectiveRestrictionRuntime.Validate(r, d).Reason == "TARGET_MISMATCH", "target mismatch not rejected");
    }

    private static void VerifyScopeBinding()
    {
        var d = Decision(); var r = Restriction(d) with { ScopeId = "foundation-scope:other" };
        Require(GuardianProtectiveRestrictionRuntime.Validate(r, d).Reason == "SCOPE_ID_MISMATCH", "scope mismatch not rejected");
    }

    private static void VerifySeverityBinding()
    {
        var d = Decision(); var r = Restriction(d) with { Severity = GuardianRestrictionSeverity.Critical };
        Require(GuardianProtectiveRestrictionRuntime.Validate(r, d).Reason == "SEVERITY_MISMATCH", "severity mismatch not rejected");
    }

    private static void VerifyActionBinding()
    {
        var d = Decision(); var r = Restriction(d) with { EnforcementAction = GuardianProtectiveAction.Suspend };
        Require(GuardianProtectiveRestrictionRuntime.Validate(r, d).Reason == "ENFORCEMENT_ACTION_MISMATCH", "action mismatch not rejected");
    }

    private static void VerifyRestartPersistenceRequired()
    {
        var d = Decision(); var r = Restriction(d) with { PersistAcrossRestart = false };
        Require(GuardianProtectiveRestrictionRuntime.Validate(r, d).Reason == "RESTART_PERSISTENCE_REQUIRED", "restart persistence not enforced");
    }

    private static void VerifySelfReleaseForbidden()
    {
        var d = Decision(); var r = Restriction(d) with { SubjectSelfReleaseForbidden = false };
        Require(GuardianProtectiveRestrictionRuntime.Validate(r, d).Reason == "SELF_RELEASE_MUST_BE_FORBIDDEN", "self release prohibition not enforced");
    }

    private static void VerifyReviewDeadlineDoesNotRelease()
    {
        var d = Decision(); var r = Restriction(d);
        var x = GuardianProtectiveRestrictionRuntime.EvaluateAt(r, d, new DateTimeOffset(2026, 8, 16, 0, 0, 0, TimeSpan.Zero));
        Require(x.Success && x.Status == GuardianRestrictionStatus.ReviewRequired && x.RemainsEnforced, "review deadline incorrectly released restriction");
    }

    private static void VerifyNoDeadlineRemainsActive()
    {
        var d = Decision(); var r = Restriction(d) with { ReviewDeadline = null };
        var x = GuardianProtectiveRestrictionRuntime.EvaluateAt(r, d, new DateTimeOffset(2027, 1, 1, 0, 0, 0, TimeSpan.Zero));
        Require(x.Success && x.Status == GuardianRestrictionStatus.Active && x.RemainsEnforced, "no-deadline restriction did not remain active");
    }

    private static void VerifyBeforeEffectiveFails()
    {
        var d = Decision(); var r = Restriction(d);
        var x = GuardianProtectiveRestrictionRuntime.EvaluateAt(r, d, new DateTimeOffset(2026, 8, 14, 19, 30, 30, TimeSpan.Zero));
        Require(!x.Success && x.Reason == "RESTRICTION_NOT_YET_EFFECTIVE", "pre-effective evaluation not rejected");
    }

    private static void VerifyMissingEvidenceRejected()
    {
        var d = Decision(); var r = Restriction(d) with { EvidenceReference = string.Empty };
        Require(GuardianProtectiveRestrictionRuntime.Validate(r, d).Reason == "INVALID_EVIDENCE_REFERENCE", "missing evidence not rejected");
    }

    private static void VerifyInvalidSeverityRejected()
    {
        var d = Decision(); var r = Restriction(d) with { Severity = (GuardianRestrictionSeverity)999 };
        Require(GuardianProtectiveRestrictionRuntime.Validate(r, d).Reason == "INVALID_RESTRICTION_SEVERITY", "invalid severity not rejected");
    }

    private static void VerifyNonRestrictiveDecisionRejected()
    {
        var d = Decision() with { ProtectiveMode = GuardianProtectiveMode.Heightened, ProtectiveAction = GuardianProtectiveAction.Warn, ConsequenceClass = GuardianConsequenceClass.Moderate };
        var threw = false;
        try { _ = Restriction(d); } catch (ArgumentException) { threw = true; }
        Require(threw, "non-restrictive decision created a restriction");
    }

    private static void VerifyRestrictionIdentityDeterministic()
    {
        var d = Decision(); var a = Restriction(d); var b = Restriction(d);
        Require(string.Equals(a.Identity, b.Identity, StringComparison.Ordinal), "restriction identity is not deterministic");
    }

    private static void VerifyRestrictionIdentityMutationSensitive()
    {
        var d = Decision(); var a = Restriction(d); var b = a with { ReviewDeadline = a.ReviewDeadline!.Value.AddMinutes(1) };
        Require(!string.Equals(a.Identity, b.Identity, StringComparison.Ordinal), "restriction identity ignored material mutation");
    }

    private static void VerifyDecisionMutationInvalidatesBinding()
    {
        var d = Decision(); var r = Restriction(d); var changed = d with { EvidenceReference = "evidence:stage8:wp03:changed" };
        Require(!GuardianProtectiveRestrictionRuntime.Validate(r, changed).Success, "changed source decision did not invalidate binding");
    }

    private static void VerifyAuthorityBinding()
    {
        var d = Decision(); var r = Restriction(d) with { AuthorityReference = "authority:other" };
        Require(GuardianProtectiveRestrictionRuntime.Validate(r, d).Reason == "AUTHORITY_MISMATCH", "authority mismatch not rejected");
    }

    private static void VerifyPolicyBinding()
    {
        var d = Decision(); var r = Restriction(d) with { PolicyReference = "policy:other" };
        Require(GuardianProtectiveRestrictionRuntime.Validate(r, d).Reason == "POLICY_MISMATCH", "policy mismatch not rejected");
    }

    private static void VerifyStage9BoundaryPreserved()
    {
        var names = typeof(GuardianProtectiveRestrictionRuntime).GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)
            .Select(m => m.Name).ToArray();
        Require(!names.Contains("Release", StringComparer.Ordinal) && !names.Contains("Recover", StringComparer.Ordinal) && !names.Contains("RestoreTrust", StringComparer.Ordinal), "Stage 9 release/recovery surface leaked into WP-03");
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
