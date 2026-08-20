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
            VerifyValidDecision();
            VerifyDeterministicIdentity();
            VerifyMutationSensitivity();
            VerifyMissingEvidenceRejected();
            VerifyMissingAuthorityRejected();
            VerifyInvalidEnumRejected();
            VerifyNormalModeContradictionRejected();
            VerifyLowConsequenceFalconWideRejected();
            VerifyNoAuthorityGrantSurface();
            VerifyNoLifecycleExecutionSurface();
            VerifyNoRecoveryReleaseSurface();
            VerifyApplicationNeutrality();

            Console.WriteLine("STAGE8_WP01_VERIFIER = PASS");
            Console.WriteLine("CHECKS = 12/12");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("STAGE8_WP01_VERIFIER = FAIL");
            Console.Error.WriteLine(ex.Message);
            return 1;
        }
    }

    private static GuardianProtectiveDecision CreateValid()
    {
        return new GuardianProtectiveDecision(
            "guardian-decision:stage8:wp01:001",
            "foundation-subject:example",
            GuardianScopeKind.FoundationSubsystem,
            "foundation-scope:example",
            GuardianProtectiveMode.Restricted,
            GuardianProtectiveAction.Restrict,
            GuardianConsequenceClass.High,
            "TECHNICAL_TRUST_UNCERTAIN",
            "evidence:stage8:wp01:001",
            "authority:guardian:approved",
            "policy:AUT-002:v1.0",
            "Material technical trust uncertainty requires bounded protective restriction.",
            "Independent governed evidence must establish resolution before any release decision.",
            new DateTimeOffset(2026, 8, 14, 17, 30, 0, TimeSpan.Zero));
    }

    private static void VerifyValidDecision()
    {
        var result = GuardianProtectiveDecisionValidator.Validate(CreateValid());
        Require(result.Success, "valid Guardian protective decision rejected: " + result.Reason);
        Require(CreateValid().Identity.Length == 64, "Guardian decision identity is not SHA-256 length");
    }

    private static void VerifyDeterministicIdentity()
    {
        var a = CreateValid();
        var b = CreateValid();
        Require(string.Equals(a.Identity, b.Identity, StringComparison.Ordinal), "identical decisions produced different identities");
    }

    private static void VerifyMutationSensitivity()
    {
        var a = CreateValid();
        var b = a with { Reason = "Material technical trust uncertainty requires isolation." };
        Require(!string.Equals(a.Identity, b.Identity, StringComparison.Ordinal), "material mutation did not change identity");
    }

    private static void VerifyMissingEvidenceRejected()
    {
        var result = GuardianProtectiveDecisionValidator.Validate(CreateValid() with { EvidenceReference = string.Empty });
        Require(!result.Success && result.Reason == "INVALID_EVIDENCE_REFERENCE", "missing evidence was not rejected exactly");
    }

    private static void VerifyMissingAuthorityRejected()
    {
        var result = GuardianProtectiveDecisionValidator.Validate(CreateValid() with { AuthorityReference = string.Empty });
        Require(!result.Success && result.Reason == "INVALID_AUTHORITY_REFERENCE", "missing authority was not rejected exactly");
    }

    private static void VerifyInvalidEnumRejected()
    {
        var result = GuardianProtectiveDecisionValidator.Validate(CreateValid() with { ProtectiveMode = (GuardianProtectiveMode)999 });
        Require(!result.Success && result.Reason == "INVALID_PROTECTIVE_MODE", "invalid protective mode was accepted");
    }

    private static void VerifyNormalModeContradictionRejected()
    {
        var result = GuardianProtectiveDecisionValidator.Validate(CreateValid() with { ProtectiveMode = GuardianProtectiveMode.Normal });
        Require(!result.Success && result.Reason == "NORMAL_MODE_CONTRADICTS_RESTRICTIVE_ACTION", "NORMAL + restrictive action contradiction accepted");
    }

    private static void VerifyLowConsequenceFalconWideRejected()
    {
        var result = GuardianProtectiveDecisionValidator.Validate(CreateValid() with
        {
            ScopeKind = GuardianScopeKind.FalconWide,
            ConsequenceClass = GuardianConsequenceClass.Low
        });
        Require(!result.Success && result.Reason == "FALCON_WIDE_SCOPE_REQUIRES_HIGHER_CONSEQUENCE", "low-consequence Falcon-wide protective scope accepted");
    }

    private static void VerifyNoAuthorityGrantSurface()
    {
        RequireNoPublicMethod("GrantAuthority", "AuthorizeAction", "MintAuthority", "CreateAuthorityInstrument");
    }

    private static void VerifyNoLifecycleExecutionSurface()
    {
        RequireNoPublicMethod("Transition", "Start", "Stop", "Suspend", "Resume", "Retire");
    }

    private static void VerifyNoRecoveryReleaseSurface()
    {
        RequireNoPublicMethod("Recover", "Release", "RestoreTrust", "Reintroduce", "ControlledRevival");
    }

    private static void RequireNoPublicMethod(params string[] forbidden)
    {
        var assembly = typeof(GuardianProtectiveDecision).Assembly;
        foreach (var type in assembly.GetExportedTypes())
        {
            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly))
            {
                Require(!forbidden.Contains(method.Name, StringComparer.Ordinal), "forbidden Stage 8 WP-01 public surface: " + type.FullName + "." + method.Name);
            }
        }
    }

    private static void VerifyApplicationNeutrality()
    {
        var refs = typeof(GuardianProtectiveDecision).Assembly.GetReferencedAssemblies()
            .Select(a => a.Name ?? string.Empty)
            .ToArray();

        // The project-level Foundation.Contracts dependency is enforced by the Architecture gate.
        // Do not require an AssemblyRef here because the C# compiler legitimately omits unused
        // metadata references until a production type is actually consumed from that assembly.
        Require(!refs.Any(r => r.Contains("Application", StringComparison.OrdinalIgnoreCase)), "Application dependency leaked into Foundation.Guardian");
        Require(!refs.Any(r => r.Contains("Trading", StringComparison.OrdinalIgnoreCase)), "Trading dependency leaked into Foundation.Guardian");
        Require(!refs.Any(r => r.Contains("Web", StringComparison.OrdinalIgnoreCase)), "Web dependency leaked into Foundation.Guardian");
        Require(!refs.Any(r => r.Contains("Recovery", StringComparison.OrdinalIgnoreCase)), "Stage 9 Recovery dependency leaked into WP-01");
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }
}
