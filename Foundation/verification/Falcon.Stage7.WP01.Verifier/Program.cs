using System;
using System.Linq;
using System.Reflection;
using Foundation.Contracts;
using Foundation.ContractRegistry;
using Foundation.HealthFitness;

internal static class Program
{
    private static int Main()
    {
        try
        {
            RunAll();
            Console.WriteLine("STAGE7_WP01_VERIFIER=PASS");
            return 0;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine("STAGE7_WP01_VERIFIER=FAIL");
            Console.Error.WriteLine(exception.Message);
            return 1;
        }
    }

    private static void RunAll()
    {
        VerifyValidCanonicalAssessment();
        VerifyDeterministicIdentity();
        VerifyMutationSensitivity();
        VerifyInvalidEnumRejected();
        VerifyMalformedIdentityRejected();
        VerifyImpossibleTimeOrderRejected();
        VerifyMissingEvidenceRejected();
        VerifyContractV12Projection();
        VerifyCanonicalRegistryV12();
        VerifyNoAuthorityGrantSurface();
        VerifyNoApplicationBusinessDependency();
    }

    private static CanonicalHealthFitnessAssessment CreateValid()
    {
        var observation = new DateTimeOffset(2026, 8, 12, 10, 0, 0, TimeSpan.Zero);
        var assessment = observation.AddSeconds(1);
        var effective = assessment;

        return new CanonicalHealthFitnessAssessment(
            "assessment:stage7:wp01:positive",
            "foundation.health.subject:example",
            "foundation.technical.health",
            "FOUNDATION_TECHNICAL_OPERATION",
            HealthState.Healthy,
            TechnicalFitnessState.Fit,
            FitnessProjectionResult.Fit,
            "foundation/example",
            "evidence:set:stage7:wp01:positive",
            "selfmodel:foundation:stage7:wp01",
            EvidenceQuality.Sufficient,
            "SUFFICIENT",
            "NONE",
            "NONE",
            "NONE",
            "ALL_REQUIRED_PRIMITIVES_VALID",
            "health-rule:stage7:wp01:structural",
            "1.0",
            observation,
            assessment,
            effective,
            effective.AddMinutes(1));
    }

    private static void VerifyValidCanonicalAssessment()
    {
        var result = HealthFitnessPrimitiveValidator.Validate(CreateValid());
        Require(result.Result == ValidationResult.Pass, "positive canonical assessment rejected");
    }

    private static void VerifyDeterministicIdentity()
    {
        var first = CreateValid();
        var second = CreateValid();
        Require(string.Equals(first.Identity, second.Identity, StringComparison.Ordinal), "identical assessments produced different identities");
        Require(first.Identity.Length == 64, "assessment identity is not SHA-256 length");
    }

    private static void VerifyMutationSensitivity()
    {
        var original = CreateValid();
        var mutated = original with { Reason = "MUTATED_REASON" };
        Require(!string.Equals(original.Identity, mutated.Identity, StringComparison.Ordinal), "material mutation did not change assessment identity");
    }

    private static void VerifyInvalidEnumRejected()
    {
        var invalid = CreateValid() with { HealthState = (HealthState)999 };
        var result = HealthFitnessPrimitiveValidator.Validate(invalid);
        Require(result.Result == ValidationResult.Fail, "invalid enum accepted");
    }

    private static void VerifyMalformedIdentityRejected()
    {
        var invalid = CreateValid() with { SubjectId = " foundation subject " };
        var result = HealthFitnessPrimitiveValidator.Validate(invalid);
        Require(result.Result == ValidationResult.Fail, "malformed canonical identity accepted");
    }

    private static void VerifyImpossibleTimeOrderRejected()
    {
        var valid = CreateValid();
        var invalid = valid with { ObservationTime = valid.AssessmentTime.AddSeconds(1) };
        var result = HealthFitnessPrimitiveValidator.Validate(invalid);
        Require(result.Result == ValidationResult.Fail, "impossible assessment time order accepted");
    }

    private static void VerifyMissingEvidenceRejected()
    {
        var invalid = CreateValid() with { EvidenceReference = string.Empty };
        var result = HealthFitnessPrimitiveValidator.Validate(invalid);
        Require(result.Result == ValidationResult.Fail, "missing evidence reference accepted");
    }

    private static void VerifyContractV12Projection()
    {
        var projected = HealthFitnessContractProjection.ToContractV12(CreateValid());
        Require(string.Equals(projected.ContractId, ContractIdentity.Con006, StringComparison.Ordinal), "wrong contract identity");
        Require(string.Equals(projected.Version, "1.2", StringComparison.Ordinal), "wrong CON-006 successor version");
        Require(HealthFitnessV12Validator.Validate(projected).Result == ValidationResult.Pass, "projected CON-006 v1.2 rejected");
        Require(string.Equals(projected.HealthState, "HEALTHY", StringComparison.Ordinal), "health state projection mismatch");
        Require(string.Equals(projected.TechnicalFitnessState, "FIT", StringComparison.Ordinal), "technical fitness projection mismatch");
        Require(string.Equals(projected.FitnessResult, "FIT", StringComparison.Ordinal), "fitness result projection mismatch");
    }

    private static void VerifyCanonicalRegistryV12()
    {
        var registry = ContractRegistry.CreateCanonical();
        var lookup = registry.Lookup("CON-006", "1.2")
            ?? throw new InvalidOperationException("canonical executable registry does not expose CON-006@1.2");
        Require(
            lookup.Entry.SchemaOrExecutableRepresentation.Contains("HealthFitnessAssessmentV12", StringComparison.Ordinal),
            "canonical executable registry is not bound to HealthFitnessAssessmentV12");
        Require(registry.ValidateCanonicalCoverage().Success, "canonical registry coverage failed");
        Require(registry.ValidateDeterministicLookup().Success, "canonical registry lookup failed");
    }

    private static void VerifyNoAuthorityGrantSurface()
    {
        var assembly = typeof(CanonicalHealthFitnessAssessment).Assembly;
        var forbidden = new[] { "GrantAuthority", "AuthorizeAction", "IssueGuardianCommand", "RestrictSubject", "ReleaseSubject", "LifecycleTransition" };

        foreach (var type in assembly.GetExportedTypes())
        {
            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                Require(!forbidden.Contains(method.Name, StringComparer.Ordinal), $"authority/protection surface leaked into Stage 7 primitives: {type.FullName}.{method.Name}");
            }
        }
    }

    private static void VerifyNoApplicationBusinessDependency()
    {
        var references = typeof(CanonicalHealthFitnessAssessment).Assembly
            .GetReferencedAssemblies()
            .Select(reference => reference.Name ?? string.Empty)
            .ToArray();

        Require(references.Contains("Foundation.Contracts", StringComparer.Ordinal), "Foundation.Contracts dependency missing");
        Require(!references.Any(reference => reference.Contains("Application", StringComparison.OrdinalIgnoreCase)), "Application dependency leaked into Stage 7 primitive project");
        Require(!references.Any(reference => reference.Contains("Trading", StringComparison.OrdinalIgnoreCase)), "Trading dependency leaked into Stage 7 primitive project");
        Require(!references.Any(reference => reference.Contains("Market", StringComparison.OrdinalIgnoreCase)), "Market dependency leaked into Stage 7 primitive project");
        Require(!references.Any(reference => reference.Contains("Portfolio", StringComparison.OrdinalIgnoreCase)), "Portfolio dependency leaked into Stage 7 primitive project");
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
}
