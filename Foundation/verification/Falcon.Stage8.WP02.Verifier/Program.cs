using System;
using System.Linq;
using System.Reflection;
using Foundation.Guardian;

internal static class Program
{
    private static int Main()
    {
        try
        {
            VerifyBenignCaseRemainsNormal();
            VerifyDeterministicEvaluationIdentity();
            VerifyMutationSensitivity();
            VerifyModerateRiskEscalatesProportionately();
            VerifySevereUnknownFavorsProtection();
            VerifyMandatoryThresholdCannotRemainWarningOnly();
            VerifySubjectOnlyEvidenceCannotSupportOptimisticContinuation();
            VerifyUnknownEvidenceCannotSupportOptimisticContinuation();
            VerifyInvalidEnumRejected();
            VerifyMissingEvidenceRejected();
            VerifyMandatoryInvalidEvaluationIsObservableProtectionFailure();
            VerifyGeneratedDecisionValidates();
            VerifyProtectionMonotonicity();
            VerifyNoAuthorityGrantSurface();
            VerifyNoLifecycleExecutionSurface();
            VerifyNoRecoveryReleaseSurface();
            VerifyApplicationNeutrality();

            Console.WriteLine("STAGE8_WP02_VERIFIER = PASS");
            Console.WriteLine("CHECKS = 17/17");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("STAGE8_WP02_VERIFIER = FAIL");
            Console.Error.WriteLine(ex.Message);
            return 1;
        }
    }

    private static GuardianProtectiveEvaluationRequest CreateBaseline()
        => new(
            "guardian-evaluation:stage8:wp02:001",
            "foundation-subject:example",
            GuardianScopeKind.FoundationSubsystem,
            "foundation-scope:example",
            GuardianCredibleHarm.None,
            GuardianUncertainty.Low,
            GuardianReversibility.Easy,
            GuardianEvidenceIndependence.Independent,
            false,
            "TECHNICAL_CONDITION_EVALUATION",
            "evidence:stage8:wp02:001",
            "authority:guardian:approved",
            "policy:AUT-002:v1.0",
            new DateTimeOffset(2026, 8, 14, 18, 20, 0, TimeSpan.Zero));

    private static void VerifyBenignCaseRemainsNormal()
    {
        var outcome = GuardianProtectiveEvaluationRuntime.Evaluate(CreateBaseline());
        Require(outcome.Success, "benign evaluation failed: " + outcome.Reason);
        Require(outcome.Decision is not null, "benign evaluation produced no decision");
        Require(outcome.Decision!.ProtectiveMode == GuardianProtectiveMode.Normal, "benign evaluation was over-restricted");
        Require(outcome.Decision!.ProtectiveAction == GuardianProtectiveAction.Observe, "benign evaluation did not remain Observe");
        Require(!outcome.InterventionRequired, "benign evaluation incorrectly requires intervention");
    }

    private static void VerifyDeterministicEvaluationIdentity()
    {
        var a = CreateBaseline();
        var b = CreateBaseline();
        Require(string.Equals(a.Identity, b.Identity, StringComparison.Ordinal), "identical evaluation inputs produced different identities");

        var oa = GuardianProtectiveEvaluationRuntime.Evaluate(a);
        var ob = GuardianProtectiveEvaluationRuntime.Evaluate(b);
        Require(oa.Success && ob.Success, "deterministic evaluation did not succeed twice");
        Require(string.Equals(oa.Decision!.Identity, ob.Decision!.Identity, StringComparison.Ordinal), "identical evaluations produced different decision identities");
    }

    private static void VerifyMutationSensitivity()
    {
        var a = CreateBaseline();
        var b = a with { CredibleHarm = GuardianCredibleHarm.High };
        Require(!string.Equals(a.Identity, b.Identity, StringComparison.Ordinal), "material evaluation mutation did not change identity");

        var oa = GuardianProtectiveEvaluationRuntime.Evaluate(a);
        var ob = GuardianProtectiveEvaluationRuntime.Evaluate(b);
        Require(oa.Success && ob.Success, "mutation comparison evaluation failed");
        Require(!string.Equals(oa.Decision!.Identity, ob.Decision!.Identity, StringComparison.Ordinal), "material mutation did not change protective decision identity");
    }

    private static void VerifyModerateRiskEscalatesProportionately()
    {
        var request = CreateBaseline() with
        {
            CredibleHarm = GuardianCredibleHarm.Moderate,
            Uncertainty = GuardianUncertainty.Moderate,
            Reversibility = GuardianReversibility.Difficult,
            EvidenceIndependence = GuardianEvidenceIndependence.Mixed
        };

        var outcome = GuardianProtectiveEvaluationRuntime.Evaluate(request);
        Require(outcome.Success, "moderate-risk evaluation failed");
        Require(outcome.Decision!.ProtectiveMode == GuardianProtectiveMode.Restricted, "moderate-risk evaluation did not enter Restricted mode");
        Require(outcome.Decision!.ProtectiveAction == GuardianProtectiveAction.Suspend, "moderate-risk evaluation did not choose proportionate suspension intent");
        Require(outcome.InterventionRequired, "moderate-risk evaluation did not require intervention");
    }

    private static void VerifySevereUnknownFavorsProtection()
    {
        var request = CreateBaseline() with
        {
            CredibleHarm = GuardianCredibleHarm.Critical,
            Uncertainty = GuardianUncertainty.Unknown,
            Reversibility = GuardianReversibility.Unknown,
            EvidenceIndependence = GuardianEvidenceIndependence.SubjectOnly
        };

        var outcome = GuardianProtectiveEvaluationRuntime.Evaluate(request);
        Require(outcome.Success, "severe unknown evaluation failed");
        Require(outcome.Decision!.ProtectiveMode == GuardianProtectiveMode.Safe, "severe unknown condition did not enter Safe mode");
        Require(outcome.Decision!.ProtectiveAction == GuardianProtectiveAction.RequestEmergencyStop, "severe unknown condition did not favor emergency protection");
        Require(outcome.Decision!.ConsequenceClass == GuardianConsequenceClass.Critical, "severe unknown condition did not remain Critical consequence");
    }

    private static void VerifyMandatoryThresholdCannotRemainWarningOnly()
    {
        var request = CreateBaseline() with
        {
            MandatoryInterventionThresholdMet = true,
            CredibleHarm = GuardianCredibleHarm.Low
        };

        var outcome = GuardianProtectiveEvaluationRuntime.Evaluate(request);
        Require(outcome.Success, "mandatory-threshold evaluation failed");
        Require(outcome.Decision!.ProtectiveAction is not GuardianProtectiveAction.Observe and not GuardianProtectiveAction.Warn, "mandatory intervention threshold was allowed to remain non-restrictive");
        Require(outcome.InterventionRequired, "mandatory intervention threshold did not require intervention");
        Require(!outcome.ProtectionFailureObservable, "successful mandatory intervention was incorrectly marked as protection failure");
    }

    private static void VerifySubjectOnlyEvidenceCannotSupportOptimisticContinuation()
    {
        var request = CreateBaseline() with
        {
            CredibleHarm = GuardianCredibleHarm.Low,
            EvidenceIndependence = GuardianEvidenceIndependence.SubjectOnly
        };

        var outcome = GuardianProtectiveEvaluationRuntime.Evaluate(request);
        Require(outcome.Success, "subject-only evidence evaluation failed");
        Require(outcome.Decision!.ProtectiveMode == GuardianProtectiveMode.Restricted, "subject-only evidence was allowed to support optimistic mode");
        Require(outcome.Decision!.ProtectiveAction == GuardianProtectiveAction.Restrict, "subject-only evidence did not force bounded restriction");
    }

    private static void VerifyUnknownEvidenceCannotSupportOptimisticContinuation()
    {
        var request = CreateBaseline() with
        {
            EvidenceIndependence = GuardianEvidenceIndependence.Unknown
        };

        var outcome = GuardianProtectiveEvaluationRuntime.Evaluate(request);
        Require(outcome.Success, "unknown-evidence evaluation failed");
        Require(outcome.Decision!.ProtectiveAction == GuardianProtectiveAction.Restrict, "unknown evidence independence was hidden as optimistic continuation");
    }

    private static void VerifyInvalidEnumRejected()
    {
        var outcome = GuardianProtectiveEvaluationRuntime.Evaluate(
            CreateBaseline() with { Uncertainty = (GuardianUncertainty)999 });

        Require(!outcome.Success && outcome.Reason == "INVALID_UNCERTAINTY", "invalid uncertainty enum was not rejected exactly");
    }

    private static void VerifyMissingEvidenceRejected()
    {
        var outcome = GuardianProtectiveEvaluationRuntime.Evaluate(
            CreateBaseline() with { EvidenceReference = string.Empty });

        Require(!outcome.Success && outcome.Reason == "INVALID_EVIDENCE_REFERENCE", "missing evidence was not rejected exactly");
    }

    private static void VerifyMandatoryInvalidEvaluationIsObservableProtectionFailure()
    {
        var outcome = GuardianProtectiveEvaluationRuntime.Evaluate(
            CreateBaseline() with
            {
                MandatoryInterventionThresholdMet = true,
                AuthorityReference = string.Empty
            });

        Require(!outcome.Success, "invalid mandatory evaluation unexpectedly succeeded");
        Require(outcome.ProtectionFailureObservable, "failure to produce a mandatory intervention was not observable as protection failure");
    }

    private static void VerifyGeneratedDecisionValidates()
    {
        var outcome = GuardianProtectiveEvaluationRuntime.Evaluate(
            CreateBaseline() with
            {
                CredibleHarm = GuardianCredibleHarm.High,
                Uncertainty = GuardianUncertainty.High,
                Reversibility = GuardianReversibility.Difficult
            });

        Require(outcome.Success && outcome.Decision is not null, "generated-decision validation fixture failed");
        var validation = GuardianProtectiveDecisionValidator.Validate(outcome.Decision!);
        Require(validation.Success, "runtime generated an invalid WP-01 Guardian decision: " + validation.Reason);
    }

    private static void VerifyProtectionMonotonicity()
    {
        var low = GuardianProtectiveEvaluationRuntime.Evaluate(CreateBaseline());
        var medium = GuardianProtectiveEvaluationRuntime.Evaluate(CreateBaseline() with
        {
            CredibleHarm = GuardianCredibleHarm.Moderate,
            Uncertainty = GuardianUncertainty.Moderate
        });
        var severe = GuardianProtectiveEvaluationRuntime.Evaluate(CreateBaseline() with
        {
            CredibleHarm = GuardianCredibleHarm.Critical,
            Uncertainty = GuardianUncertainty.Unknown,
            Reversibility = GuardianReversibility.Irreversible
        });

        Require(low.Success && medium.Success && severe.Success, "monotonicity fixtures failed");
        Require(low.ProtectiveScore < medium.ProtectiveScore, "moderate condition did not increase protective score");
        Require(medium.ProtectiveScore < severe.ProtectiveScore, "severe condition did not increase protective score");
        Require((int)low.Decision!.ProtectiveAction < (int)medium.Decision!.ProtectiveAction, "moderate condition did not increase protective action");
        Require((int)medium.Decision!.ProtectiveAction < (int)severe.Decision!.ProtectiveAction, "severe condition did not increase protective action");
    }

    private static void VerifyNoAuthorityGrantSurface()
        => RequireNoPublicMethod("GrantAuthority", "AuthorizeAction", "MintAuthority", "CreateAuthorityInstrument");

    private static void VerifyNoLifecycleExecutionSurface()
        => RequireNoPublicMethod("Transition", "Start", "Stop", "SuspendLifecycle", "Resume", "Retire");

    private static void VerifyNoRecoveryReleaseSurface()
        => RequireNoPublicMethod("Recover", "Release", "RestoreTrust", "Reintroduce", "ControlledRevival");

    private static void RequireNoPublicMethod(params string[] forbidden)
    {
        var assembly = typeof(GuardianProtectiveEvaluationRuntime).Assembly;
        foreach (var type in assembly.GetExportedTypes())
        {
            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly))
            {
                Require(!forbidden.Contains(method.Name, StringComparer.Ordinal), "forbidden Stage 8 WP-02 public surface: " + type.FullName + "." + method.Name);
            }
        }
    }

    private static void VerifyApplicationNeutrality()
    {
        var refs = typeof(GuardianProtectiveEvaluationRuntime).Assembly.GetReferencedAssemblies()
            .Select(a => a.Name ?? string.Empty)
            .ToArray();

        Require(!refs.Any(r => r.Contains("Application", StringComparison.OrdinalIgnoreCase)), "Application dependency leaked into Guardian evaluation runtime");
        Require(!refs.Any(r => r.Contains("Trading", StringComparison.OrdinalIgnoreCase)), "Trading dependency leaked into Guardian evaluation runtime");
        Require(!refs.Any(r => r.Contains("Web", StringComparison.OrdinalIgnoreCase)), "Web dependency leaked into Guardian evaluation runtime");
        Require(!refs.Any(r => r.Contains("Recovery", StringComparison.OrdinalIgnoreCase)), "Stage 9 Recovery dependency leaked into Guardian evaluation runtime");
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }
}
