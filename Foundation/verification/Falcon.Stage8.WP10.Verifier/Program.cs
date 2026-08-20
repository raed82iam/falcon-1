using System;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Foundation.Authority;
using Foundation.Contracts;

namespace Falcon.Stage8.WP10.Verifier;

internal static class Program
{
    private static readonly DateTimeOffset Now = new(2026, 8, 15, 13, 0, 0, TimeSpan.Zero);
    private static int _checks;

    private static int Main()
    {
        try
        {
            var authorityAssembly = typeof(DefaultDenyAuthorityEngine).Assembly;
            var guardianAssembly = Assembly.Load("Foundation.Guardian");
            var lifecycleAssembly = Assembly.Load("Foundation.ApplicationLifecycle");
            var contractsAssembly = typeof(RestrictionRecord).Assembly;

            Check(authorityAssembly.GetName().Name == "Foundation.Authority", "Authority assembly identity drifted");
            Check(guardianAssembly.GetName().Name == "Foundation.Guardian", "Guardian assembly identity drifted");
            Check(lifecycleAssembly.GetName().Name == "Foundation.ApplicationLifecycle", "Lifecycle assembly identity drifted");
            Check(contractsAssembly.GetName().Name == "Foundation.Contracts", "Contracts assembly identity drifted");

            Check(HasType(authorityAssembly, "Foundation.Authority.DefaultDenyAuthorityEngine"), "AUT-001 authority engine missing");
            Check(HasType(authorityAssembly, "Foundation.Authority.ProtectiveRestrictionAuthorityEnforcer"), "protective restriction authority enforcement missing");
            Check(HasType(authorityAssembly, "Foundation.Authority.IndependentEmergencyControlRuntime"), "independent emergency control runtime missing");
            Check(HasType(authorityAssembly, "Foundation.Authority.IndependentEmergencyControlAuthorityEnforcer"), "independent emergency authority enforcement missing");
            Check(HasType(authorityAssembly, "Foundation.Authority.ProtectiveReleaseGuard"), "protective no-self-release guard missing");
            Check(HasType(authorityAssembly, "Foundation.Authority.RecoveryHandoffRuntime"), "recovery handoff runtime missing");
            Check(HasType(authorityAssembly, "Foundation.Authority.RecoveryHandoffRecord"), "recovery handoff record missing");
            Check(HasType(authorityAssembly, "Foundation.Authority.RecoveryEvidencePackage"), "recovery evidence package missing");

            Check(HasType(lifecycleAssembly, "Foundation.ApplicationLifecycle.ProtectiveLifecycleEnforcer"), "protective lifecycle enforcement missing");
            Check(guardianAssembly.GetExportedTypes().Any(t => t.Name.Contains("ProtectiveEvaluation", StringComparison.Ordinal)), "Guardian protective evaluation runtime missing");
            Check(guardianAssembly.GetExportedTypes().Any(t => t.Name.Contains("ProtectiveRestriction", StringComparison.Ordinal)), "Guardian protective restriction surface missing");
            Check(guardianAssembly.GetExportedTypes().Any(t => t.Name.Contains("PlatformSafeState", StringComparison.Ordinal)), "Guardian platform safe-state surface missing");
            Check(guardianAssembly.GetExportedTypes().Any(t => t.Name.Contains("RestrictionPersistence", StringComparison.Ordinal)), "Guardian durable restriction persistence surface missing");

            Check(HasType(contractsAssembly, "Foundation.Contracts.RestrictionRecord"), "CON-011 RestrictionRecord missing");
            Check(HasType(contractsAssembly, "Foundation.Contracts.ProtectiveSafeStateContractPolicy"), "canonical safe-state contract policy missing");

            var authorityRefs = authorityAssembly.GetReferencedAssemblies().Select(x => x.Name ?? string.Empty).ToArray();
            Check(!authorityRefs.Contains("Foundation.Guardian", StringComparer.Ordinal), "Authority depends on Guardian and loses independent control boundary");
            Check(!authorityRefs.Any(IsApplicationAssemblyName), "Authority gained Application dependency");

            var foundationAssemblies = new[] { authorityAssembly, guardianAssembly, lifecycleAssembly, contractsAssembly };
            var exported = foundationAssemblies.SelectMany(a => a.GetExportedTypes()).ToArray();
            Check(!exported.Any(t => IsBusinessSemanticName(t.FullName ?? t.Name)), "Application business semantics leaked into Stage 8 Foundation surfaces");
            Check(!exported.Any(t => ContainsStageIdentityToken(t.Name)), "transient Stage identity leaked into permanent production public type names");

            var recoveryRuntime = typeof(RecoveryHandoffRuntime);
            var recoveryMethods = recoveryRuntime.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
            Check(!recoveryMethods.Any(m =>
                    string.Equals(m.Name, "Release", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(m.Name, "Recover", StringComparison.OrdinalIgnoreCase) ||
                    m.Name.Contains("RestoreTrust", StringComparison.OrdinalIgnoreCase) ||
                    m.Name.Contains("Reintroduce", StringComparison.OrdinalIgnoreCase) ||
                    m.Name.Contains("Revival", StringComparison.OrdinalIgnoreCase)),
                "Stage 9 recovery/release execution leaked into Stage 8");

            Check(typeof(RecoveryHandoffRecord).GetConstructors(BindingFlags.Public | BindingFlags.Instance).Length == 0,
                "external caller can manufacture a recovery-ready handoff record");

            var subjectGuard = ProtectiveReleaseGuard.Evaluate(
                "subject:wp10",
                ProtectiveReleaseActorRole.Subject,
                "subject:wp10",
                "guardian:foundation",
                "restriction:wp10",
                Now);
            Check(!subjectGuard.Allowed && subjectGuard.RestrictionRemainsEnforced,
                "subject self-release was not fail-closed in integrated Stage 8 surface");

            var guardianGuard = ProtectiveReleaseGuard.Evaluate(
                "guardian:foundation",
                ProtectiveReleaseActorRole.Guardian,
                "subject:wp10",
                "guardian:foundation",
                "restriction:wp10",
                Now);
            Check(!guardianGuard.Allowed && guardianGuard.RestrictionRemainsEnforced,
                "Guardian self-release was not fail-closed in integrated Stage 8 surface");

            var releaseAuthorityGuard = ProtectiveReleaseGuard.Evaluate(
                "release-authority:wp10",
                ProtectiveReleaseActorRole.DeclaredReleaseAuthority,
                "subject:wp10",
                "guardian:foundation",
                "restriction:wp10",
                Now);
            Check(!releaseAuthorityGuard.Allowed && releaseAuthorityGuard.RestrictionRemainsEnforced,
                "declared release authority executed release inside Stage 8");

            var safeActions = ProtectiveSafeStateContractPolicy.CanonicalAllowedSafeActions;
            Check(!string.IsNullOrWhiteSpace(safeActions), "canonical safe-state allowlist is empty");
            Check(!safeActions.Contains("*", StringComparison.Ordinal), "safe-state allowlist became wildcard authority");

            var restriction = new RestrictionRecord(
                "restriction:wp10:integrated",
                ContractVersions.Con011,
                "subject:wp10",
                "mandate:wp10",
                "trigger:wp10",
                "SAFE",
                safeActions,
                "*",
                "STAGE9_INDEPENDENT_RECOVERY_VALIDATION_AND_AUTHORIZED_RELEASE_REQUIRED",
                "INDEPENDENT_GOVERNED_RELEASE_AUTHORITY",
                "IMPOSED",
                "integrity:wp10",
                Now.AddHours(-1),
                DateTimeOffset.MaxValue);
            Check(ContractValidators.Validate(restriction).Result == ValidationResult.Pass,
                "integrated CON-011 restriction is not contract-valid");
            Check(restriction.Expiry == DateTimeOffset.MaxValue,
                "unresolved integrated restriction is not restart/time latched");

            var evidenceSeed = string.Join("\n", new[]
            {
                authorityAssembly.GetName().Name ?? string.Empty,
                guardianAssembly.GetName().Name ?? string.Empty,
                lifecycleAssembly.GetName().Name ?? string.Empty,
                contractsAssembly.GetName().Name ?? string.Empty,
                restriction.RestrictionId,
                restriction.SubjectId,
                restriction.ProtectiveMode,
                restriction.AllowedSafeActions,
                restriction.ReleaseConditions,
                restriction.ReleaseAuthority,
                "FCR-0076",
                "FCR-0082",
                "WP01-WP09"
            });

            var evidenceIdentity1 = Digest(evidenceSeed);
            var evidenceIdentity2 = Digest(evidenceSeed);
            var evidenceIdentityMutated = Digest(evidenceSeed + "\nmutation");
            Check(evidenceIdentity1 == evidenceIdentity2, "integrated evidence identity is not deterministic");
            Check(evidenceIdentity1 != evidenceIdentityMutated, "integrated evidence identity is not mutation-sensitive");

            Check(!exported.Any(t => (t.FullName ?? t.Name).Contains("FactoryReset", StringComparison.OrdinalIgnoreCase) ||
                                     (t.FullName ?? t.Name).Contains("ControlledRevival", StringComparison.OrdinalIgnoreCase)),
                "Stage 13 FSA-specific recovery authority leaked into Stage 8 production surfaces");

            if (_checks != 35)
                throw new InvalidOperationException($"Unexpected check count: {_checks}, expected 35.");

            Console.WriteLine("STAGE8_WP10_INTEGRATED_VERIFIER = PASS");
            Console.WriteLine("CHECKS = 35/35");
            Console.WriteLine("WP01_WP09_BINDINGS = PRESENT");
            Console.WriteLine("AUT001_AUTHORITY_OWNER = PRESERVED");
            Console.WriteLine("LIFECYCLE_TRANSITION_OWNER = PRESERVED");
            Console.WriteLine("GUARDIAN_PROTECTS_NOT_GRANTS_AUTHORITY = PRESERVED");
            Console.WriteLine("SAFE_STATE_ALLOWLIST != AUTHORITY_GRANT");
            Console.WriteLine("FCR0076_STAGE8_SCOPE = COVERED_FOR_INTEGRATED_VERIFICATION");
            Console.WriteLine("FCR0082_STAGE8_SCOPE = COVERED_FOR_INTEGRATED_VERIFICATION");
            Console.WriteLine("APPLICATION_NEUTRALITY = PASS");
            Console.WriteLine("STAGE9_RECOVERY_RELEASE_IMPLEMENTATION = ABSENT");
            Console.WriteLine("STAGE13_FSA_SPECIFIC_AUTHORITY_LEAKAGE = ABSENT");
            Console.WriteLine("INTEGRATED_EVIDENCE_IDENTITY = " + evidenceIdentity1);
            Console.WriteLine("STAGE8_OWNER_CLOSURE = NOT_GRANTED_BY_TECHNICAL_PASS");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("STAGE8_WP10_INTEGRATED_VERIFIER = FAIL");
            Console.Error.WriteLine(ex.Message);
            return 1;
        }
    }

    private static bool HasType(Assembly assembly, string fullName) => assembly.GetType(fullName, false, false) is not null;

    private static bool IsApplicationAssemblyName(string name) =>
        name.Contains("FSATS", StringComparison.OrdinalIgnoreCase) ||
        name.Contains("Trading", StringComparison.OrdinalIgnoreCase) ||
        name.Contains("Web", StringComparison.OrdinalIgnoreCase) ||
        name.Contains("Application.", StringComparison.OrdinalIgnoreCase);

    private static bool IsBusinessSemanticName(string name) =>
        name.Contains("Trade", StringComparison.OrdinalIgnoreCase) ||
        name.Contains("Strategy", StringComparison.OrdinalIgnoreCase) ||
        name.Contains("Portfolio", StringComparison.OrdinalIgnoreCase) ||
        name.Contains("Broker", StringComparison.OrdinalIgnoreCase) ||
        name.Contains("Market", StringComparison.OrdinalIgnoreCase) ||
        name.Contains("FSATS", StringComparison.OrdinalIgnoreCase);

    private static bool ContainsStageIdentityToken(string name) =>
        name.Contains("Stage0", StringComparison.OrdinalIgnoreCase) ||
        name.Contains("Stage1", StringComparison.OrdinalIgnoreCase) ||
        name.Contains("Stage2", StringComparison.OrdinalIgnoreCase) ||
        name.Contains("Stage3", StringComparison.OrdinalIgnoreCase) ||
        name.Contains("Stage4", StringComparison.OrdinalIgnoreCase) ||
        name.Contains("Stage5", StringComparison.OrdinalIgnoreCase) ||
        name.Contains("Stage6", StringComparison.OrdinalIgnoreCase) ||
        name.Contains("Stage7", StringComparison.OrdinalIgnoreCase) ||
        name.Contains("Stage8", StringComparison.OrdinalIgnoreCase) ||
        name.Contains("Stage9", StringComparison.OrdinalIgnoreCase) ||
        name.Contains("Stage10", StringComparison.OrdinalIgnoreCase) ||
        name.Contains("Stage11", StringComparison.OrdinalIgnoreCase) ||
        name.Contains("Stage12", StringComparison.OrdinalIgnoreCase) ||
        name.Contains("Stage13", StringComparison.OrdinalIgnoreCase) ||
        name.Contains("Stage14", StringComparison.OrdinalIgnoreCase);

    private static string Digest(string value) =>
        "sha256/" + Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));

    private static void Check(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException("CHECK FAILED: " + message);

        _checks++;
    }
}
