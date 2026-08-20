using System;
using System.IO;
using Foundation.ApplicationLifecycle;
using Foundation.Authority;
using Foundation.Contracts;
using Foundation.Guardian;

namespace Falcon.Stage8.WP06.Verifier;

internal static class Program
{
    private static int _checks;

    private static int Main()
    {
        var tempRoot = Path.Combine(Path.GetTempPath(), "Falcon.Stage8.WP06.Verifier");
        var snapshotPath = Path.Combine(tempRoot, "restriction.snapshot.json");

        try
        {
            if (Directory.Exists(tempRoot))
                Directory.Delete(tempRoot, true);
            Directory.CreateDirectory(tempRoot);

            var now = new DateTimeOffset(2026, 8, 14, 22, 0, 0, TimeSpan.Zero);
            var decision = CreateDecision(now);
            var restriction = GuardianProtectiveRestrictionRuntime.CreateFromDecision(
                decision,
                "restriction/stage8/wp06/001",
                now.AddMinutes(-10),
                now.AddMinutes(30));

            var snapshot = GuardianRestrictionPersistence.CreateSnapshot(
                decision,
                restriction,
                now);

            Check(snapshot.FormatVersion == GuardianRestrictionPersistence.FormatVersion, "snapshot format version bound");
            Check(snapshot.Restriction.Identity == restriction.Identity, "snapshot restriction identity preserved");
            Check(snapshot.Decision.Identity == decision.Identity, "snapshot decision identity preserved");
            Check(!string.IsNullOrWhiteSpace(snapshot.SnapshotIdentity), "snapshot identity emitted");

            var payloadA = GuardianRestrictionPersistence.Serialize(snapshot);
            var payloadB = GuardianRestrictionPersistence.Serialize(snapshot);
            Check(Convert.ToHexString(payloadA) == Convert.ToHexString(payloadB), "snapshot serialization deterministic");

            GuardianRestrictionPersistence.SaveAtomic(snapshotPath, snapshot);
            Check(File.Exists(snapshotPath), "durable snapshot file created");
            Check(!File.Exists(snapshotPath + ".tmp"), "atomic persistence leaves no temp artifact");

            var reconstructed = GuardianRestrictionPersistence.ReconstructAfterRestart(
                snapshotPath,
                now.AddMinutes(5));

            Check(reconstructed.Success, "valid persisted restriction reconstructs after restart");
            Check(reconstructed.ContainmentFenceRequired, "restart reconstruction requires containment fence");
            Check(!reconstructed.TrustedOperationPermitted, "restart does not restore trusted operation");
            Check(reconstructed.Restriction is not null && reconstructed.Restriction.Identity == restriction.Identity, "restart preserves restriction identity");
            Check(reconstructed.Decision is not null && reconstructed.Decision.Identity == decision.Identity, "restart preserves source decision identity");
            Check(reconstructed.ContractRecord is not null, "restart republishes canonical protective contract");

            var contract = reconstructed.ContractRecord!;
            Check(ContractValidators.Validate(contract).Result == ValidationResult.Pass, "reconstructed CON-011 is canonical-valid");
            Check(contract.Result == "IMPOSED", "reconstructed restriction remains imposed");

            var authorityRequest = CreateAuthorityRequest(now.AddMinutes(5));
            var authorityContext = CreateAuthorityContext(now.AddMinutes(5));
            var authority = new ProtectiveRestrictionAuthorityEnforcer().Evaluate(
                authorityRequest,
                authorityContext,
                new[] { contract });

            Check(authority.Decision == AuthorityDecision.Deny, "reconstructed restriction continues to constrain authority");
            Check(authority.Reason == ProtectiveAuthorityReason.RestrictedByGuardian, "restart authority denial remains Guardian-attributable");

            var lifecycleRequest = new ProtectiveLifecycleRequest(
                "lifecycle/stage8/wp06/001",
                contract.SubjectId,
                contract.RestrictionId,
                contract.IntegrityEvidence,
                contract.MandateReference,
                contract.TriggerEvidence,
                contract.ProtectiveMode,
                ProtectiveLifecycleEvidenceState.Valid,
                ProtectiveLifecycleEvidenceState.Valid,
                contract.EffectiveTime,
                now.AddMinutes(5));

            var lifecycle = ProtectiveLifecycleEnforcer.Enforce(lifecycleRequest);
            Check(lifecycle.Success && lifecycle.RemainsRestricted, "restart reconstruction continues Lifecycle restriction");
            Check(!lifecycle.NewExecutionAllowed, "restart does not permit new execution");

            var afterReviewDeadline = GuardianRestrictionPersistence.ReconstructAfterRestart(
                snapshotPath,
                now.AddHours(1));
            Check(afterReviewDeadline.Success, "review deadline does not invalidate persisted restriction");
            Check(afterReviewDeadline.Reason == "RECONSTRUCTED_REVIEW_REQUIRED_RESTRICTION_REMAINS_ENFORCED", "review deadline becomes review-required without release");
            Check(afterReviewDeadline.ContainmentFenceRequired && !afterReviewDeadline.TrustedOperationPermitted, "review deadline preserves containment fence");

            var missingPath = Path.Combine(tempRoot, "missing.snapshot.json");
            var missing = GuardianRestrictionPersistence.ReconstructAfterRestart(missingPath, now.AddMinutes(5));
            Check(!missing.Success && missing.ContainmentFenceRequired && !missing.TrustedOperationPermitted, "missing persisted state fails closed");

            var tamperedPayload = File.ReadAllBytes(snapshotPath);
            tamperedPayload[tamperedPayload.Length / 2] ^= 0x01;
            File.WriteAllBytes(snapshotPath, tamperedPayload);
            var tampered = GuardianRestrictionPersistence.ReconstructAfterRestart(snapshotPath, now.AddMinutes(5));
            Check(!tampered.Success && tampered.ContainmentFenceRequired && !tampered.TrustedOperationPermitted, "tampered persisted state fails closed");

            GuardianRestrictionPersistence.SaveAtomic(snapshotPath, snapshot);
            var restoredAgain = GuardianRestrictionPersistence.ReconstructAfterRestart(snapshotPath, now.AddMinutes(5));
            Check(restoredAgain.Success && restoredAgain.Restriction?.Identity == restriction.Identity, "atomic rewrite restores same governed snapshot identity");

            var mutatedRestriction = restriction with { EvidenceReference = "evidence/stage8/wp06/mutated" };
            var mutationRejected = false;
            try
            {
                _ = GuardianRestrictionPersistence.CreateSnapshot(decision, mutatedRestriction, now);
            }
            catch (ArgumentException)
            {
                mutationRejected = true;
            }
            Check(mutationRejected, "snapshot refuses restriction/source-decision mismatch");

            Check(restriction.PersistAcrossRestart, "restriction explicitly requires restart persistence");
            Check(restriction.SubjectSelfReleaseForbidden, "restart persistence preserves self-release prohibition");

            Console.WriteLine("STAGE8_WP06_VERIFIER = PASS");
            Console.WriteLine($"CHECKS = {_checks}/{_checks}");
            Console.WriteLine("RESTART = NOT_RELEASE");
            Console.WriteLine("MISSING_OR_TAMPERED_PERSISTENCE = FAIL_CLOSED");
            Console.WriteLine("STAGE9_RECOVERY_RELEASE = NOT_IMPLEMENTED");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("STAGE8_WP06_VERIFIER = FAIL");
            Console.Error.WriteLine(ex.Message);
            return 1;
        }
        finally
        {
            try
            {
                if (Directory.Exists(tempRoot))
                    Directory.Delete(tempRoot, true);
            }
            catch
            {
            }
        }
    }

    private static GuardianProtectiveDecision CreateDecision(DateTimeOffset now)
        => new(
            "guardian-decision/stage8/wp06/001",
            "foundation-subject:wp06",
            GuardianScopeKind.FoundationSubsystem,
            "foundation:wp06",
            GuardianProtectiveMode.Restricted,
            GuardianProtectiveAction.Restrict,
            GuardianConsequenceClass.High,
            "TECHNICAL_PROTECTIVE_TRIGGER",
            "evidence/stage8/wp06",
            "authority:guardian:approved",
            "AUT-002:v1.0",
            "Restart-resistant protective restriction required.",
            "Independent Stage 9 recovery validation and authorized release required.",
            now.AddMinutes(-15));

    private static AuthorityRequest CreateAuthorityRequest(DateTimeOffset now)
        => new(
            "authority-request:stage8:wp06:001",
            "foundation-subject:wp06",
            "EXECUTE",
            "resource:wp06",
            "purpose:wp06",
            "foundation:wp06",
            "LIVE",
            "TRUSTED",
            "FIT",
            "correlation:wp06",
            now.AddMinutes(-1),
            now.AddHours(1));

    private static AuthorityEvaluationContext CreateAuthorityContext(DateTimeOffset now)
    {
        var policy = new AuthorityPolicy(
            "policy:wp06",
            "1.0",
            "owner:governed",
            now.AddHours(-1),
            now.AddHours(2),
            new[] { "foundation-subject:wp06" },
            new[] { "EXECUTE", "REPORT_HEALTH" },
            new[] { "resource:wp06" },
            new[] { "purpose:wp06" },
            new[] { "foundation:wp06" },
            new[] { "TRUSTED" });

        var delegation = new DelegationEvidence(
            "delegation:wp06",
            "foundation-subject:wp06",
            "owner:governed",
            new[] { "foundation:wp06" },
            now.AddHours(-1),
            now.AddHours(2),
            false);

        var fitness = new FitnessEvidence(
            "foundation-subject:wp06",
            "FIT",
            true,
            now.AddMinutes(-10),
            now.AddHours(1),
            "fitness-evidence:wp06");

        return new AuthorityEvaluationContext(
            policy,
            delegation,
            fitness,
            now,
            "authority-evidence:wp06");
    }

    private static void Check(bool condition, string name)
    {
        if (!condition)
            throw new InvalidOperationException("CHECK FAILED: " + name);
        _checks++;
    }
}
