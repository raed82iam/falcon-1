using System;
using System.Linq;
using System.Reflection;
using Foundation.Authority;
using Foundation.Contracts;
using Foundation.Guardian;

namespace Falcon.Stage8.WP07.Verifier;

internal static class Program
{
    private static readonly DateTimeOffset Now = new(2026, 8, 15, 0, 0, 0, TimeSpan.Zero);
    private static int _checks;

    private static int Main()
    {
        try
        {
            var decision = CreateSafeDecision(
                "guardian-decision:stage8:wp07:001",
                "foundation-subject:wp07",
                GuardianScopeKind.FoundationSubsystem,
                "foundation:wp07");

            var restriction = GuardianProtectiveRestrictionRuntime.CreateFromDecision(
                decision,
                "restriction:stage8:wp07:001",
                Now.AddMinutes(-10),
                Now.AddHours(1));

            var safeState = GuardianPlatformSafeStateRuntime.Create(
                decision,
                restriction,
                "safe-state:stage8:wp07:001",
                Now.AddMinutes(-5));

            Check(GuardianPlatformSafeStateRuntime.Validate(safeState, decision, restriction).Success,
                "valid Safe-State rejected");
            Check(safeState.Identity == safeState.Identity,
                "Safe-State identity is not deterministic");
            Check(safeState.Identity != (safeState with { ScopeId = "foundation:wp07:mutated" }).Identity,
                "Safe-State identity is not mutation-sensitive");
            Check(GuardianPlatformSafeStateRuntime.CanonicalAllowedActions ==
                "REPORT_HEALTH|PUBLISH_EVIDENCE|COMPLY_WITH_PROTECTIVE_CONTROL",
                "canonical Safe-State allowlist changed");
            Check(Enum.GetValues<GuardianSafeStateOperation>().Length == 3,
                "unexpected Safe-State operation expanded the allowlist");
            Check(GuardianPlatformSafeStateRuntime.IsCanonicalAllowedOperation(GuardianSafeStateOperation.ReportHealth),
                "REPORT_HEALTH not allowed");
            Check(GuardianPlatformSafeStateRuntime.IsCanonicalAllowedOperation(GuardianSafeStateOperation.PublishEvidence),
                "PUBLISH_EVIDENCE not allowed");
            Check(GuardianPlatformSafeStateRuntime.IsCanonicalAllowedOperation(GuardianSafeStateOperation.ComplyWithProtectiveControl),
                "COMPLY_WITH_PROTECTIVE_CONTROL not allowed");
            Check(!GuardianPlatformSafeStateRuntime.IsCanonicalAllowedOperation((GuardianSafeStateOperation)999),
                "unknown operation escaped deny-by-default");

            var exact = GuardianPlatformSafeStateRuntime.EvaluateOperation(
                safeState,
                decision,
                restriction,
                GuardianSafeStateOperation.ReportHealth,
                "foundation-subject:wp07",
                GuardianScopeKind.FoundationSubsystem,
                "foundation:wp07",
                Now);

            Check(exact.Success && exact.AppliesToRequestedScope,
                "Safe-State did not apply to exact governed scope");
            Check(exact.OperationWithinSafeStateCeiling,
                "canonical operation was outside Safe-State ceiling");
            Check(exact.IndependentAuthorityStillRequired,
                "Safe-State incorrectly removed independent authority requirement");
            Check(!exact.AuthorityGranted,
                "Safe-State minted authority");
            Check(exact.ContainmentRemainsRequired,
                "Safe-State removed containment requirement");

            var outside = GuardianPlatformSafeStateRuntime.EvaluateOperation(
                safeState,
                decision,
                restriction,
                GuardianSafeStateOperation.ReportHealth,
                "foundation-subject:independent",
                GuardianScopeKind.FoundationSubsystem,
                "foundation:independent",
                Now);

            Check(outside.Success,
                "independent scope evaluation failed instead of remaining non-applicable");
            Check(!outside.AppliesToRequestedScope,
                "local Safe-State leaked into independent scope");
            Check(!outside.ContainmentRemainsRequired,
                "local Safe-State forced containment on independent scope");

            var record = GuardianRestrictionContractPublisher.Publish(restriction, decision);
            Check(record.ProtectiveMode == "SAFE",
                "critical Safe-State restriction did not publish SAFE mode");
            Check(record.AllowedSafeActions == GuardianPlatformSafeStateRuntime.CanonicalAllowedActions,
                "CON-011 did not publish canonical Safe-State allowlist");

            var enforcer = new ProtectiveRestrictionAuthorityEnforcer();
            var context = CreateContext();

            var baseline = enforcer.Evaluate(CreateRequest("EXECUTE"), context, Array.Empty<RestrictionRecord>());
            Check(baseline.Decision == AuthorityDecision.Allow,
                "valid baseline authority fixture did not allow EXECUTE");

            var denied = enforcer.Evaluate(CreateRequest("EXECUTE"), context, new[] { record });
            Check(denied.Decision == AuthorityDecision.Deny && denied.Reason == ProtectiveAuthorityReason.RestrictedByGuardian,
                "Safe-State did not deny non-allowlisted governed action");

            Check(enforcer.Evaluate(CreateRequest("REPORT_HEALTH"), context, new[] { record }).Decision == AuthorityDecision.Allow,
                "REPORT_HEALTH did not remain eligible for independent authority evaluation");
            Check(enforcer.Evaluate(CreateRequest("PUBLISH_EVIDENCE"), context, new[] { record }).Decision == AuthorityDecision.Allow,
                "PUBLISH_EVIDENCE did not remain eligible for independent authority evaluation");
            Check(enforcer.Evaluate(CreateRequest("COMPLY_WITH_PROTECTIVE_CONTROL"), context, new[] { record }).Decision == AuthorityDecision.Allow,
                "COMPLY_WITH_PROTECTIVE_CONTROL did not remain eligible for independent authority evaluation");

            var unrelated = record with { SubjectId = "foundation-subject:independent" };
            Check(enforcer.Evaluate(CreateRequest("EXECUTE"), context, new[] { unrelated }).Decision == AuthorityDecision.Allow,
                "Safe-State restriction leaked across subject boundary");

            var afterReview = GuardianPlatformSafeStateRuntime.EvaluateOperation(
                safeState,
                decision,
                restriction,
                GuardianSafeStateOperation.PublishEvidence,
                "foundation-subject:wp07",
                GuardianScopeKind.FoundationSubsystem,
                "foundation:wp07",
                Now.AddHours(2));

            Check(afterReview.Success && afterReview.ContainmentRemainsRequired,
                "review deadline incorrectly released Safe-State containment");
            Check(afterReview.OperationWithinSafeStateCeiling && !afterReview.AuthorityGranted,
                "review deadline changed Safe-State authority semantics");

            var notSafeDecision = decision with
            {
                DecisionId = "guardian-decision:stage8:wp07:not-safe",
                ProtectiveMode = GuardianProtectiveMode.Restricted,
                ProtectiveAction = GuardianProtectiveAction.Restrict,
                ConsequenceClass = GuardianConsequenceClass.High
            };
            var notSafeRestriction = GuardianProtectiveRestrictionRuntime.CreateFromDecision(
                notSafeDecision,
                "restriction:stage8:wp07:not-safe",
                Now.AddMinutes(-10),
                Now.AddHours(1));

            var rejectedNonSafe = false;
            try
            {
                _ = GuardianPlatformSafeStateRuntime.Create(
                    notSafeDecision,
                    notSafeRestriction,
                    "safe-state:stage8:wp07:not-safe",
                    Now);
            }
            catch (ArgumentException)
            {
                rejectedNonSafe = true;
            }
            Check(rejectedNonSafe,
                "non-SAFE protective mode created Platform Safe-State");

            var bindingMutation = safeState with { SourceRestrictionIdentity = safeState.SourceRestrictionIdentity + "A" };
            Check(!GuardianPlatformSafeStateRuntime.Validate(bindingMutation, decision, restriction).Success,
                "Safe-State restriction binding mutation was accepted");

            var publicMethods = typeof(GuardianPlatformSafeStateRuntime)
                .GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
            Check(!publicMethods.Any(m =>
                    m.Name.Contains("Release", StringComparison.OrdinalIgnoreCase) ||
                    m.Name.Contains("Recover", StringComparison.OrdinalIgnoreCase) ||
                    m.Name.Contains("RestoreTrust", StringComparison.OrdinalIgnoreCase) ||
                    m.Name.Contains("Revival", StringComparison.OrdinalIgnoreCase)),
                "Stage 9 recovery/release surface leaked into WP-07");

            var names = typeof(GuardianPlatformSafeStateRuntime).Assembly
                .GetExportedTypes()
                .Where(t => t.Namespace == "Foundation.Guardian")
                .Select(t => t.Name)
                .ToArray();
            Check(!names.Any(n =>
                    n.Contains("Trade", StringComparison.OrdinalIgnoreCase) ||
                    n.Contains("Strategy", StringComparison.OrdinalIgnoreCase) ||
                    n.Contains("Portfolio", StringComparison.OrdinalIgnoreCase) ||
                    n.Contains("Market", StringComparison.OrdinalIgnoreCase)),
                "Application business semantics leaked into Foundation Guardian");

            var falconDecision = CreateSafeDecision(
                "guardian-decision:stage8:wp07:falcon-wide",
                "foundation-subject:falcon",
                GuardianScopeKind.FalconWide,
                "falcon:platform");
            var falconRestriction = GuardianProtectiveRestrictionRuntime.CreateFromDecision(
                falconDecision,
                "restriction:stage8:wp07:falcon-wide",
                Now.AddMinutes(-10),
                Now.AddHours(1));
            var falconSafeState = GuardianPlatformSafeStateRuntime.Create(
                falconDecision,
                falconRestriction,
                "safe-state:stage8:wp07:falcon-wide",
                Now.AddMinutes(-5));
            var falconEvaluation = GuardianPlatformSafeStateRuntime.EvaluateOperation(
                falconSafeState,
                falconDecision,
                falconRestriction,
                GuardianSafeStateOperation.PublishEvidence,
                "any:independent:target",
                GuardianScopeKind.Application,
                "any:application:scope",
                Now);
            Check(falconEvaluation.Success && falconEvaluation.AppliesToRequestedScope && falconEvaluation.ContainmentRemainsRequired,
                "explicit FalconWide Safe-State did not apply platform-wide");

            if (_checks != 32)
                throw new InvalidOperationException($"Unexpected check count: {_checks}, expected 32.");

            Console.WriteLine("STAGE8_WP07_VERIFIER = PASS");
            Console.WriteLine("CHECKS = 32/32");
            Console.WriteLine("SAFE_STATE_ALLOWLIST = DENY_BY_DEFAULT");
            Console.WriteLine("SAFE_STATE_ALLOWLIST != AUTHORITY_GRANT");
            Console.WriteLine("LOCAL_SAFE_STATE != AUTOMATIC_FALCON_WIDE_SHUTDOWN");
            Console.WriteLine("RECOVERY_RELEASE_AUTHORITY = NOT_GRANTED");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("STAGE8_WP07_VERIFIER = FAIL");
            Console.Error.WriteLine(ex.Message);
            return 1;
        }
    }

    private static GuardianProtectiveDecision CreateSafeDecision(
        string decisionId,
        string targetId,
        GuardianScopeKind scopeKind,
        string scopeId)
        => new(
            decisionId,
            targetId,
            scopeKind,
            scopeId,
            GuardianProtectiveMode.Safe,
            GuardianProtectiveAction.RequestEmergencyStop,
            GuardianConsequenceClass.Critical,
            "TECHNICAL_SAFE_STATE_TRIGGER",
            "guardian-evidence:wp07",
            "authority:guardian:approved",
            "AUT-002:v1.0",
            "Technical Safe-State containment required.",
            "Independent Stage 9 validation and authorized release required.",
            Now.AddMinutes(-15));

    private static AuthorityRequest CreateRequest(string action)
        => new(
            "authority-request:stage8:wp07:" + action,
            "foundation-subject:wp07",
            action,
            "resource:wp07",
            "purpose:wp07",
            "foundation:wp07",
            "LIVE",
            "TRUSTED",
            "FIT",
            "correlation:wp07",
            Now.AddMinutes(-1),
            Now.AddHours(1));

    private static AuthorityEvaluationContext CreateContext()
    {
        var policy = new AuthorityPolicy(
            "policy:wp07",
            "1.0",
            "owner:governed",
            Now.AddHours(-1),
            Now.AddHours(2),
            new[] { "foundation-subject:wp07" },
            new[] { "EXECUTE", "REPORT_HEALTH", "PUBLISH_EVIDENCE", "COMPLY_WITH_PROTECTIVE_CONTROL" },
            new[] { "resource:wp07" },
            new[] { "purpose:wp07" },
            new[] { "foundation:wp07" },
            new[] { "TRUSTED" });

        var delegation = new DelegationEvidence(
            "delegation:wp07",
            "foundation-subject:wp07",
            "owner:governed",
            new[] { "foundation:wp07" },
            Now.AddHours(-1),
            Now.AddHours(2),
            false);

        var fitness = new FitnessEvidence(
            "foundation-subject:wp07",
            "FIT",
            true,
            Now.AddMinutes(-10),
            Now.AddHours(1),
            "fitness-evidence:wp07");

        return new AuthorityEvaluationContext(
            policy,
            delegation,
            fitness,
            Now,
            "authority-evidence:wp07");
    }

    private static void Check(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException("CHECK FAILED: " + message);
        _checks++;
    }
}
