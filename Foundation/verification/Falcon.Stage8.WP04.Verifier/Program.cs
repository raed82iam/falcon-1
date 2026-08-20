using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Foundation.Authority;
using Foundation.Contracts;
using Foundation.Guardian;

internal static class Program
{
    private static readonly DateTimeOffset Now = new(2026, 8, 14, 20, 15, 0, TimeSpan.Zero);

    private static int Main()
    {
        try
        {
            var enforcer = new ProtectiveRestrictionAuthorityEnforcer();
            var request = CreateRequest("EXECUTE");
            var context = CreateContext();
            var decision = CreateGuardianDecision();
            var restriction = GuardianProtectiveRestrictionRuntime.CreateFromDecision(
                decision,
                "restriction:stage8:wp04:001",
                Now.AddMinutes(-5),
                Now.AddHours(1));
            var record = GuardianRestrictionContractPublisher.Publish(restriction, decision);

            var baseline = enforcer.Evaluate(request, context, Array.Empty<RestrictionRecord>());
            Require(baseline.Decision == AuthorityDecision.Allow, "baseline authority did not allow the valid fixture");

            Require(record.ContractId == ContractIdentity.Con011 && record.Version == ContractVersions.Con011, "Guardian publication did not use canonical CON-011");
            Require(record.SubjectId == request.ActorIdentity, "published restriction target did not bind to authority actor");

            var denied = enforcer.Evaluate(request, context, new[] { record });
            Require(denied.Decision == AuthorityDecision.Deny, "active Guardian restriction did not deny conflicting authority");
            Require(denied.Reason == ProtectiveAuthorityReason.RestrictedByGuardian, "active Guardian restriction emitted wrong deny reason");
            Require(denied.EffectiveScope == "NONE", "restricted authority retained effective scope");
            Require(denied.Constraints.Contains(record.RestrictionId, StringComparison.Ordinal), "authority denial did not identify controlling restriction");

            var safeRequest = CreateRequest("REPORT_HEALTH");
            var safe = enforcer.Evaluate(safeRequest, context, new[] { record });
            Require(safe.Decision == AuthorityDecision.Allow, "explicitly allowed safe technical action was incorrectly denied");

            var other = record with { SubjectId = "other:subject" };
            Require(enforcer.Evaluate(request, context, new[] { other }).Decision == AuthorityDecision.Allow, "unrelated subject restriction leaked across target boundary");

            var future = record with { EffectiveTime = Now.AddMinutes(5), Expiry = DateTimeOffset.MaxValue };
            Require(enforcer.Evaluate(request, context, new[] { future }).Decision == AuthorityDecision.Allow, "future restriction was enforced before effective time");

            var expired = record with { EffectiveTime = Now.AddHours(-2), Expiry = Now.AddMinutes(-1) };
            Require(enforcer.Evaluate(request, context, new[] { expired }).Decision == AuthorityDecision.Allow, "expired CON-011 record remained authority-active");

            var unavailable = enforcer.Evaluate(request, context, null);
            Require(unavailable.Decision == AuthorityDecision.Deny && unavailable.Reason == ProtectiveAuthorityReason.RestrictionEvidenceUnavailable, "missing restriction evidence did not fail closed");

            var malformed = record with { IntegrityEvidence = string.Empty };
            var malformedResult = enforcer.Evaluate(request, context, new[] { malformed });
            Require(malformedResult.Decision == AuthorityDecision.Deny && malformedResult.Reason == ProtectiveAuthorityReason.RestrictionMalformed, "malformed restriction evidence did not fail closed");

            var baselineDenied = enforcer.Evaluate(CreateRequest("FORBIDDEN_ACTION"), context, Array.Empty<RestrictionRecord>());
            Require(baselineDenied.Decision == AuthorityDecision.Deny, "protective enforcer improperly converted baseline deny into allow");

            var deniedAgain = enforcer.Evaluate(request, context, new[] { record });
            Require(denied.DecisionId == deniedAgain.DecisionId, "same restriction inputs produced nondeterministic authority decision identity");

            var mutatedRecord = record with { IntegrityEvidence = record.IntegrityEvidence + "A" };
            var mutatedDenied = enforcer.Evaluate(request, context, new[] { mutatedRecord });
            Require(denied.DecisionId != mutatedDenied.DecisionId, "restriction evidence mutation did not change protective authority identity");

            var authorityRefs = typeof(ProtectiveRestrictionAuthorityEnforcer).Assembly.GetReferencedAssemblies()
                .Select(a => a.Name ?? string.Empty)
                .ToArray();
            Require(!authorityRefs.Any(r => r.Contains("Guardian", StringComparison.OrdinalIgnoreCase)), "Foundation.Authority gained a direct Guardian assembly dependency");

            var publicMethods = typeof(ProtectiveRestrictionAuthorityEnforcer).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            Require(!publicMethods.Any(m => m.Name.Contains("Release", StringComparison.OrdinalIgnoreCase) || m.Name.Contains("Recover", StringComparison.OrdinalIgnoreCase)), "Stage 9 release/recovery surface leaked into WP-04 enforcer");

            Console.WriteLine("STAGE8_WP04_VERIFIER = PASS");
            Console.WriteLine("CHECKS = 17/17");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("STAGE8_WP04_VERIFIER = FAIL");
            Console.Error.WriteLine(ex.Message);
            return 1;
        }
    }

    private static AuthorityRequest CreateRequest(string action) => new(
        "authority-request:stage8:wp04:001:" + action,
        "foundation-subject:wp04",
        action,
        "resource:wp04",
        "purpose:wp04",
        "foundation:wp04",
        "LIVE",
        "TRUSTED",
        "FIT",
        "correlation:wp04",
        Now.AddMinutes(-1),
        Now.AddHours(1));

    private static AuthorityEvaluationContext CreateContext()
    {
        var policy = new AuthorityPolicy(
            "policy:wp04",
            "1.0",
            "owner:governed",
            Now.AddHours(-1),
            Now.AddHours(2),
            new[] { "foundation-subject:wp04" },
            new[] { "EXECUTE", "REPORT_HEALTH" },
            new[] { "resource:wp04" },
            new[] { "purpose:wp04" },
            new[] { "foundation:wp04" },
            new[] { "TRUSTED" });

        var delegation = new DelegationEvidence(
            "delegation:wp04",
            "foundation-subject:wp04",
            "owner:governed",
            new[] { "foundation:wp04" },
            Now.AddHours(-1),
            Now.AddHours(2),
            false);

        var fitness = new FitnessEvidence(
            "foundation-subject:wp04",
            "FIT",
            true,
            Now.AddMinutes(-10),
            Now.AddHours(1),
            "fitness-evidence:wp04");

        return new AuthorityEvaluationContext(
            policy,
            delegation,
            fitness,
            Now,
            "authority-evidence:wp04");
    }

    private static GuardianProtectiveDecision CreateGuardianDecision() => new(
        "guardian-decision:stage8:wp04:001",
        "foundation-subject:wp04",
        GuardianScopeKind.FoundationSubsystem,
        "foundation:wp04",
        GuardianProtectiveMode.Restricted,
        GuardianProtectiveAction.Restrict,
        GuardianConsequenceClass.High,
        "TECHNICAL_PROTECTIVE_TRIGGER",
        "guardian-evidence:wp04",
        "authority:guardian:approved",
        "AUT-002:v1.0",
        "Active technical protective restriction",
        "Independent Stage 9 validation and authorized release",
        Now.AddMinutes(-10));

    private static void Require(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }
}
