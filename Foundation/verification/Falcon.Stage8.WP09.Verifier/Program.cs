using System;
using System.Linq;
using System.Reflection;
using Foundation.Authority;
using Foundation.Contracts;

namespace Falcon.Stage8.WP09.Verifier;

internal static class Program
{
    private static readonly DateTimeOffset Now = new(2026, 8, 15, 12, 0, 0, TimeSpan.Zero);
    private static int _checks;

    private static int Main()
    {
        try
        {
            var restriction = CreateRestriction(
                "restriction:stage8:wp09:subject",
                "subject:alpha",
                "guardian-mandate:stage8",
                "trigger-evidence:stage8:wp09",
                "restriction-integrity:stage8:wp09",
                Now.AddHours(-2),
                DateTimeOffset.MaxValue);

            var evidence = CreateEvidence(
                "recovery-evidence:stage8:wp09:complete",
                restriction,
                "guardian:foundation",
                "repair-actor:one",
                "independent-verifier:one",
                "release-authority:one",
                true,
                true,
                true,
                true,
                true,
                Now.AddMinutes(-5),
                Now.AddHours(1));

            var handoff = RecoveryHandoffRuntime.Evaluate(
                "handoff:stage8:wp09:ready",
                restriction,
                evidence,
                Now);

            Check(handoff.ReadyForRecoveryEvaluation,
                "complete independently separated recovery evidence did not become recovery-evaluation ready");
            Check(handoff.Reason == RecoveryHandoffReason.ReadyForRecoveryEvaluation,
                "ready handoff reason drifted");
            Check(!handoff.ReleaseEligibleInProtectionContext,
                "protective handoff incorrectly became release-eligible");
            Check(handoff.RestrictionRemainsEnforced,
                "ready recovery handoff cleared the protective restriction");
            Check(handoff.IndependentRecoveryValidationRequired &&
                  handoff.AuthorizedReleaseDecisionRequired &&
                  handoff.LifecycleReintroductionRequired &&
                  handoff.NewAuthorityDecisionRequired,
                "required recovery/release preconditions were not all preserved");
            Check(RecoveryHandoffRuntime.ValidateHandoff(handoff),
                "canonical recovery handoff validation failed");
            Check(handoff.ReleaseConditions == restriction.ReleaseConditions,
                "handoff did not preserve exact restriction release conditions");
            Check(handoff.ReleaseAuthority == restriction.ReleaseAuthority,
                "handoff did not preserve exact declared release authority");

            var subjectRelease = ProtectiveReleaseGuard.Evaluate(
                restriction.SubjectId,
                ProtectiveReleaseActorRole.Subject,
                restriction.SubjectId,
                evidence.GuardianIdentity,
                restriction.RestrictionId,
                Now);
            Check(!subjectRelease.Allowed &&
                  subjectRelease.Reason == ProtectiveReleaseGuardReason.SubjectSelfReleaseDenied &&
                  subjectRelease.RestrictionRemainsEnforced,
                "subject self-release was not denied fail-closed");

            var subjectIdentityRelease = ProtectiveReleaseGuard.Evaluate(
                restriction.SubjectId,
                ProtectiveReleaseActorRole.Other,
                restriction.SubjectId,
                evidence.GuardianIdentity,
                restriction.RestrictionId,
                Now);
            Check(subjectIdentityRelease.Reason == ProtectiveReleaseGuardReason.SubjectSelfReleaseDenied,
                "subject identity bypassed self-release denial by changing role label");

            var guardianRelease = ProtectiveReleaseGuard.Evaluate(
                evidence.GuardianIdentity,
                ProtectiveReleaseActorRole.Guardian,
                restriction.SubjectId,
                evidence.GuardianIdentity,
                restriction.RestrictionId,
                Now);
            Check(!guardianRelease.Allowed &&
                  guardianRelease.Reason == ProtectiveReleaseGuardReason.GuardianSelfReleaseDenied,
                "Guardian self-release was not denied");

            var guardianIdentityRelease = ProtectiveReleaseGuard.Evaluate(
                evidence.GuardianIdentity,
                ProtectiveReleaseActorRole.Other,
                restriction.SubjectId,
                evidence.GuardianIdentity,
                restriction.RestrictionId,
                Now);
            Check(guardianIdentityRelease.Reason == ProtectiveReleaseGuardReason.GuardianSelfReleaseDenied,
                "Guardian identity bypassed denial by changing role label");

            var declaredAuthorityAttempt = ProtectiveReleaseGuard.Evaluate(
                evidence.DeclaredReleaseAuthorityIdentity,
                ProtectiveReleaseActorRole.DeclaredReleaseAuthority,
                restriction.SubjectId,
                evidence.GuardianIdentity,
                restriction.RestrictionId,
                Now);
            Check(!declaredAuthorityAttempt.Allowed &&
                  declaredAuthorityAttempt.Reason == ProtectiveReleaseGuardReason.IndependentRecoveryReleaseRequired,
                "declared release authority was allowed to execute release inside the protection context");

            var repairAttempt = ProtectiveReleaseGuard.Evaluate(
                evidence.RepairActorIdentity,
                ProtectiveReleaseActorRole.RepairActor,
                restriction.SubjectId,
                evidence.GuardianIdentity,
                restriction.RestrictionId,
                Now);
            Check(!repairAttempt.Allowed &&
                  repairAttempt.Reason == ProtectiveReleaseGuardReason.IndependentRecoveryReleaseRequired,
                "repair actor was allowed to execute release inside the protection context");

            var invalidGuard = ProtectiveReleaseGuard.Evaluate(
                " ",
                ProtectiveReleaseActorRole.Other,
                restriction.SubjectId,
                evidence.GuardianIdentity,
                restriction.RestrictionId,
                Now);
            Check(!invalidGuard.Allowed &&
                  invalidGuard.Reason == ProtectiveReleaseGuardReason.InvalidRequest,
                "invalid release-guard request did not fail closed");

            Check(RoleSeparationFails(
                    evidence with { IndependentVerifierIdentity = evidence.RepairActorIdentity },
                    restriction,
                    "handoff:stage8:wp09:repair-verifier"),
                "repair actor was accepted as independent verifier");
            Check(RoleSeparationFails(
                    evidence with { IndependentVerifierIdentity = restriction.SubjectId },
                    restriction,
                    "handoff:stage8:wp09:subject-verifier"),
                "restricted subject was accepted as independent verifier");
            Check(RoleSeparationFails(
                    evidence with { IndependentVerifierIdentity = evidence.GuardianIdentity },
                    restriction,
                    "handoff:stage8:wp09:guardian-verifier"),
                "Guardian was accepted as independent verifier");
            Check(RoleSeparationFails(
                    evidence with { DeclaredReleaseAuthorityIdentity = restriction.SubjectId },
                    restriction,
                    "handoff:stage8:wp09:subject-release-authority"),
                "restricted subject was accepted as release authority");
            Check(RoleSeparationFails(
                    evidence with { DeclaredReleaseAuthorityIdentity = evidence.GuardianIdentity },
                    restriction,
                    "handoff:stage8:wp09:guardian-release-authority"),
                "Guardian was accepted as release authority");
            Check(RoleSeparationFails(
                    evidence with { DeclaredReleaseAuthorityIdentity = evidence.RepairActorIdentity },
                    restriction,
                    "handoff:stage8:wp09:repair-release-authority"),
                "repair actor was accepted as release authority");

            var failedValidation = RecoveryHandoffRuntime.Evaluate(
                "handoff:stage8:wp09:failed-validation",
                restriction,
                evidence with { IndependentRecoveryValidationPassed = false },
                Now);
            Check(!failedValidation.ReadyForRecoveryEvaluation &&
                  failedValidation.Reason == RecoveryHandoffReason.RecoveryValidationFailed &&
                  failedValidation.RestrictionRemainsEnforced,
                "failed independent recovery validation did not preserve restriction");

            Check(IncompleteEvidenceFails(
                    evidence with { AuthoritativeStateReconciled = false },
                    restriction,
                    "handoff:stage8:wp09:state-incomplete"),
                "missing authoritative-state reconciliation was accepted");
            Check(IncompleteEvidenceFails(
                    evidence with { SecurityContextReestablished = false },
                    restriction,
                    "handoff:stage8:wp09:security-incomplete"),
                "missing security-context reestablishment was accepted");
            Check(IncompleteEvidenceFails(
                    evidence with { DependenciesReconciled = false },
                    restriction,
                    "handoff:stage8:wp09:dependency-incomplete"),
                "missing dependency reconciliation was accepted");
            Check(IncompleteEvidenceFails(
                    evidence with { GuardianConditionsSatisfied = false },
                    restriction,
                    "handoff:stage8:wp09:guardian-condition-incomplete"),
                "unsatisfied Guardian conditions were accepted");

            var subjectMismatch = RecoveryHandoffRuntime.Evaluate(
                "handoff:stage8:wp09:subject-mismatch",
                restriction,
                evidence with { SubjectId = "subject:other" },
                Now);
            Check(!subjectMismatch.ReadyForRecoveryEvaluation &&
                  subjectMismatch.Reason == RecoveryHandoffReason.InvalidRecoveryEvidence,
                "recovery evidence for another subject was accepted");

            var restrictionMismatch = RecoveryHandoffRuntime.Evaluate(
                "handoff:stage8:wp09:restriction-mismatch",
                restriction,
                evidence with { RestrictionId = "restriction:other" },
                Now);
            Check(!restrictionMismatch.ReadyForRecoveryEvaluation &&
                  restrictionMismatch.Reason == RecoveryHandoffReason.InvalidRecoveryEvidence,
                "recovery evidence for another restriction was accepted");

            var expiredEvidence = RecoveryHandoffRuntime.Evaluate(
                "handoff:stage8:wp09:expired-evidence",
                restriction,
                evidence with { Expiry = Now },
                Now);
            Check(!expiredEvidence.ReadyForRecoveryEvaluation &&
                  expiredEvidence.Reason == RecoveryHandoffReason.InvalidRecoveryEvidence,
                "expired recovery evidence was accepted");

            var finiteRestriction = CreateRestriction(
                "restriction:stage8:wp09:finite-review",
                "subject:finite",
                "guardian-mandate:stage8",
                "trigger-evidence:stage8:wp09:finite",
                "restriction-integrity:stage8:wp09:finite",
                Now.AddHours(-3),
                Now.AddHours(-1));
            var postExpiryEvidence = CreateEvidence(
                "recovery-evidence:stage8:wp09:post-expiry",
                finiteRestriction,
                "guardian:foundation",
                "repair-actor:two",
                "independent-verifier:two",
                "release-authority:two",
                true,
                true,
                true,
                true,
                true,
                Now.AddMinutes(-5),
                Now.AddHours(1));
            var postExpiry = RecoveryHandoffRuntime.Evaluate(
                "handoff:stage8:wp09:post-expiry",
                finiteRestriction,
                postExpiryEvidence,
                Now);
            Check(postExpiry.RestrictionRemainsEnforced &&
                  !postExpiry.ReleaseEligibleInProtectionContext,
                "restriction expiry/review time was interpreted as release");

            var identityMutation = RecoveryHandoffRuntime.Evaluate(
                "handoff:stage8:wp09:identity-mutation",
                restriction,
                evidence with { EvidencePackageId = "recovery-evidence:stage8:wp09:mutated" },
                Now);
            Check(identityMutation.ReadyForRecoveryEvaluation &&
                  identityMutation.Identity != handoff.Identity,
                "handoff identity was not mutation-sensitive to recovery evidence identity");

            var publicConstructors = typeof(RecoveryHandoffRecord)
                .GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            Check(publicConstructors.Length == 0,
                "external callers can construct a recovery-ready handoff record");

            var publicRuntimeMethods = typeof(RecoveryHandoffRuntime)
                .GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
            Check(!publicRuntimeMethods.Any(method =>
                    string.Equals(method.Name, "Release", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(method.Name, "Recover", StringComparison.OrdinalIgnoreCase) ||
                    method.Name.Contains("RestoreTrust", StringComparison.OrdinalIgnoreCase) ||
                    method.Name.Contains("Reintroduce", StringComparison.OrdinalIgnoreCase) ||
                    method.Name.Contains("Revival", StringComparison.OrdinalIgnoreCase)),
                "recovery/release execution API leaked into WP-09 protection runtime");

            var referencedAssemblies = typeof(RecoveryHandoffRuntime).Assembly
                .GetReferencedAssemblies()
                .Select(name => name.Name ?? string.Empty)
                .ToArray();
            Check(!referencedAssemblies.Any(name =>
                    string.Equals(name, "Foundation.Guardian", StringComparison.Ordinal)),
                "WP-09 recovery handoff depends on Foundation.Guardian runtime assembly");

            var exportedNames = typeof(RecoveryHandoffRuntime).Assembly
                .GetExportedTypes()
                .Select(type => type.Name)
                .ToArray();
            Check(!exportedNames.Any(name =>
                    name.Contains("Trade", StringComparison.OrdinalIgnoreCase) ||
                    name.Contains("Strategy", StringComparison.OrdinalIgnoreCase) ||
                    name.Contains("Portfolio", StringComparison.OrdinalIgnoreCase) ||
                    name.Contains("Broker", StringComparison.OrdinalIgnoreCase) ||
                    name.Contains("Market", StringComparison.OrdinalIgnoreCase)),
                "Application business semantics leaked into Foundation.Authority during WP-09");

            if (_checks != 35)
                throw new InvalidOperationException($"Unexpected check count: {_checks}, expected 35.");

            Console.WriteLine("STAGE8_WP09_VERIFIER = PASS");
            Console.WriteLine("CHECKS = 35/35");
            Console.WriteLine("SUBJECT_SELF_RELEASE = DENIED");
            Console.WriteLine("GUARDIAN_SELF_RELEASE = DENIED");
            Console.WriteLine("REPAIR_ACTOR_SELF_CERTIFICATION = DENIED");
            Console.WriteLine("RELEASE_ELIGIBLE_IN_STAGE8 = FALSE");
            Console.WriteLine("READY_FOR_STAGE9_EVALUATION != RELEASE");
            Console.WriteLine("RESTRICTION_EXPIRY != RELEASE");
            Console.WriteLine("STAGE9_RECOVERY_RELEASE_EXECUTION = NOT_IMPLEMENTED");
            Console.WriteLine("PRODUCTION_PUBLIC_IDENTITIES = PERMANENT_NOT_STAGE_NAMED");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("STAGE8_WP09_VERIFIER = FAIL");
            Console.Error.WriteLine(ex.Message);
            return 1;
        }
    }

    private static bool RoleSeparationFails(
        RecoveryEvidencePackage evidence,
        RestrictionRecord restriction,
        string handoffId)
    {
        var result = RecoveryHandoffRuntime.Evaluate(handoffId, restriction, evidence, Now);
        return !result.ReadyForRecoveryEvaluation &&
               result.Reason == RecoveryHandoffReason.RoleSeparationFailure &&
               result.RestrictionRemainsEnforced &&
               !result.ReleaseEligibleInProtectionContext;
    }

    private static bool IncompleteEvidenceFails(
        RecoveryEvidencePackage evidence,
        RestrictionRecord restriction,
        string handoffId)
    {
        var result = RecoveryHandoffRuntime.Evaluate(handoffId, restriction, evidence, Now);
        return !result.ReadyForRecoveryEvaluation &&
               result.Reason == RecoveryHandoffReason.RecoveryEvidenceIncomplete &&
               result.RestrictionRemainsEnforced &&
               !result.ReleaseEligibleInProtectionContext;
    }

    private static RestrictionRecord CreateRestriction(
        string restrictionId,
        string subjectId,
        string mandate,
        string triggerEvidence,
        string integrityEvidence,
        DateTimeOffset effectiveTime,
        DateTimeOffset expiry) =>
        new(
            restrictionId,
            ContractVersions.Con011,
            subjectId,
            mandate,
            triggerEvidence,
            "SAFE",
            ProtectiveSafeStateContractPolicy.CanonicalAllowedSafeActions,
            "*",
            "STAGE9_INDEPENDENT_RECOVERY_VALIDATION_AND_AUTHORIZED_RELEASE_REQUIRED",
            "INDEPENDENT_GOVERNED_RELEASE_AUTHORITY",
            "IMPOSED",
            integrityEvidence,
            effectiveTime,
            expiry);

    private static RecoveryEvidencePackage CreateEvidence(
        string evidencePackageId,
        RestrictionRecord restriction,
        string guardianIdentity,
        string repairActorIdentity,
        string independentVerifierIdentity,
        string releaseAuthorityIdentity,
        bool stateReconciled,
        bool securityReestablished,
        bool dependenciesReconciled,
        bool validationPassed,
        bool guardianConditionsSatisfied,
        DateTimeOffset observedAt,
        DateTimeOffset expiry) =>
        new(
            evidencePackageId,
            restriction.SubjectId,
            restriction.RestrictionId,
            restriction.IntegrityEvidence,
            guardianIdentity,
            repairActorIdentity,
            independentVerifierIdentity,
            releaseAuthorityIdentity,
            "authoritative-state-reconciliation:verified",
            "security-context-reestablishment:verified",
            "dependency-reconciliation:verified",
            "independent-recovery-validation:verified",
            "guardian-condition-evidence:verified",
            "residual-risk-evidence:recorded",
            stateReconciled,
            securityReestablished,
            dependenciesReconciled,
            validationPassed,
            guardianConditionsSatisfied,
            observedAt,
            expiry);

    private static void Check(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException("CHECK FAILED: " + message);

        _checks++;
    }
}
