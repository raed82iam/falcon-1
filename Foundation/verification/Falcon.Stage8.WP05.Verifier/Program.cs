using System;
using Foundation.ApplicationLifecycle;
using Foundation.Contracts;
using Foundation.Guardian;

namespace Falcon.Stage8.WP05.Verifier;

internal static class Program
{
    private static int _checks;

    private static int Main()
    {
        try
        {
            var now = new DateTimeOffset(2026, 8, 14, 18, 0, 0, TimeSpan.Zero);

            var restricted = PublishRestriction(
                "decision-restrict",
                GuardianConsequenceClass.Moderate,
                GuardianProtectiveMode.Restricted,
                GuardianProtectiveAction.Restrict,
                now);

            var isolated = PublishRestriction(
                "decision-isolate",
                GuardianConsequenceClass.High,
                GuardianProtectiveMode.Safe,
                GuardianProtectiveAction.Isolate,
                now);

            var stopped = PublishRestriction(
                "decision-stop",
                GuardianConsequenceClass.Critical,
                GuardianProtectiveMode.Safe,
                GuardianProtectiveAction.RequestEmergencyStop,
                now);

            Check(ContractValidators.Validate(restricted).Result == ValidationResult.Pass, "restricted CON-011 valid");
            Check(ContractValidators.Validate(isolated).Result == ValidationResult.Pass, "isolated CON-011 valid");
            Check(ContractValidators.Validate(stopped).Result == ValidationResult.Pass, "stopped CON-011 valid");

            var restrictedOutcome = Enforce(restricted, now.AddMinutes(1));
            Check(restrictedOutcome.Success, "restricted lifecycle enforcement succeeds");
            Check(restrictedOutcome.Target == ProtectiveLifecycleTarget.Restricted, "moderate maps to RESTRICTED");
            Check(!restrictedOutcome.IsolationRequired, "restricted does not falsely claim isolation");
            Check(!restrictedOutcome.NewExecutionAllowed && restrictedOutcome.RemainsRestricted, "restriction blocks new execution and remains enforced");

            var isolatedOutcome = Enforce(isolated, now.AddMinutes(1));
            Check(isolatedOutcome.Success, "isolated lifecycle enforcement succeeds");
            Check(isolatedOutcome.Target == ProtectiveLifecycleTarget.Suspended, "isolated maps to SUSPENDED lifecycle state");
            Check(isolatedOutcome.IsolationRequired, "isolated preserves isolation requirement");

            var stoppedOutcome = Enforce(stopped, now.AddMinutes(1));
            Check(stoppedOutcome.Success, "critical lifecycle enforcement succeeds");
            Check(stoppedOutcome.Target == ProtectiveLifecycleTarget.Stopped, "critical SAFE maps to STOPPED");
            Check(stoppedOutcome.IsolationRequired && !stoppedOutcome.NewExecutionAllowed, "stopped remains isolated and cannot start new execution");

            var missingEvidence = BuildRequest(restricted, now.AddMinutes(1)) with
            {
                RestrictionEvidenceState = ProtectiveLifecycleEvidenceState.Missing
            };
            var missingEvidenceOutcome = ProtectiveLifecycleEnforcer.Enforce(missingEvidence);
            Check(!missingEvidenceOutcome.Success &&
                  missingEvidenceOutcome.Reason == ProtectiveLifecycleReason.RestrictionEvidenceUnavailable &&
                  missingEvidenceOutcome.Target == ProtectiveLifecycleTarget.Stopped,
                "missing restriction evidence fails closed to STOPPED");

            var ambiguousAuthority = BuildRequest(restricted, now.AddMinutes(1)) with
            {
                AuthorityEvidenceState = ProtectiveLifecycleEvidenceState.Ambiguous
            };
            var ambiguousAuthorityOutcome = ProtectiveLifecycleEnforcer.Enforce(ambiguousAuthority);
            Check(!ambiguousAuthorityOutcome.Success &&
                  ambiguousAuthorityOutcome.Reason == ProtectiveLifecycleReason.AuthorityEvidenceUnavailable &&
                  !ambiguousAuthorityOutcome.NewExecutionAllowed,
                "ambiguous protective authority fails closed");

            var earlyRequest = BuildRequest(restricted, now.AddMinutes(-1));
            var earlyOutcome = ProtectiveLifecycleEnforcer.Enforce(earlyRequest);
            Check(!earlyOutcome.Success && earlyOutcome.Reason == ProtectiveLifecycleReason.RestrictionNotEffective,
                "pre-effective restriction cannot be fabricated as active");

            var unsupported = BuildRequest(restricted, now.AddMinutes(1)) with { ProtectiveMode = "UNKNOWN" };
            var unsupportedOutcome = ProtectiveLifecycleEnforcer.Enforce(unsupported);
            Check(!unsupportedOutcome.Success &&
                  unsupportedOutcome.Reason == ProtectiveLifecycleReason.UnsupportedMode &&
                  unsupportedOutcome.Target == ProtectiveLifecycleTarget.Stopped,
                "unknown protective mode fails closed");

            var deterministicA = Enforce(restricted, now.AddMinutes(1));
            var deterministicB = Enforce(restricted, now.AddMinutes(1));
            Check(string.Equals(deterministicA.OutcomeIdentity, deterministicB.OutcomeIdentity, StringComparison.Ordinal),
                "outcome identity deterministic");

            var mutated = BuildRequest(restricted, now.AddMinutes(1)) with { TriggerEvidence = "evidence-mutated" };
            var mutatedOutcome = ProtectiveLifecycleEnforcer.Enforce(mutated);
            Check(!string.Equals(deterministicA.OutcomeIdentity, mutatedOutcome.OutcomeIdentity, StringComparison.Ordinal),
                "outcome identity mutation sensitive");

            Check(stoppedOutcome.RemainsRestricted, "STOP does not imply release or recovery");
            Check(!stoppedOutcome.NewExecutionAllowed, "Lifecycle cannot self-return subject to RUNNING");

            Console.WriteLine("STAGE8_WP05_VERIFIER = PASS");
            Console.WriteLine($"CHECKS = {_checks}/{_checks}");
            Console.WriteLine("BOUNDARY = PROTECTIVE_LIFECYCLE_ENFORCEMENT_ONLY");
            Console.WriteLine("RECOVERY_RELEASE_AUTHORITY = NOT_GRANTED");
            Console.WriteLine("STAGE9_HANDOFF = PRESERVED");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("STAGE8_WP05_VERIFIER = FAIL");
            Console.Error.WriteLine(ex.Message);
            return 1;
        }
    }

    private static RestrictionRecord PublishRestriction(
        string decisionId,
        GuardianConsequenceClass consequence,
        GuardianProtectiveMode mode,
        GuardianProtectiveAction action,
        DateTimeOffset now)
    {
        var decision = new GuardianProtectiveDecision(
            decisionId,
            "subject/app-01",
            GuardianScopeKind.Application,
            "scope/app-01",
            mode,
            action,
            consequence,
            "trigger/protective",
            "evidence/protective",
            "authority/guardian",
            "policy/aut-002",
            "Protective lifecycle enforcement required.",
            "Independent Stage 9 validation and authorized release required.",
            now);

        var restriction = GuardianProtectiveRestrictionRuntime.CreateFromDecision(
            decision,
            "restriction/" + decisionId,
            now,
            now.AddHours(1));

        return GuardianRestrictionContractPublisher.Publish(restriction, decision);
    }

    private static ProtectiveLifecycleRequest BuildRequest(RestrictionRecord restriction, DateTimeOffset requestTime)
        => new(
            "lifecycle/" + restriction.RestrictionId,
            restriction.SubjectId,
            restriction.RestrictionId,
            restriction.IntegrityEvidence,
            restriction.MandateReference,
            restriction.TriggerEvidence,
            restriction.ProtectiveMode,
            ProtectiveLifecycleEvidenceState.Valid,
            ProtectiveLifecycleEvidenceState.Valid,
            restriction.EffectiveTime,
            requestTime);

    private static ProtectiveLifecycleOutcome Enforce(RestrictionRecord restriction, DateTimeOffset requestTime)
        => ProtectiveLifecycleEnforcer.Enforce(BuildRequest(restriction, requestTime));

    private static void Check(bool condition, string name)
    {
        if (!condition)
            throw new InvalidOperationException("CHECK FAILED: " + name);
        _checks++;
    }
}
