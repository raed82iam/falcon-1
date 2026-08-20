using System;
using System.Collections.Generic;
using System.Linq;
using Foundation.Contracts;

var failures = new List<string>();

void ExpectPass(string id, ValidationOutcome outcome)
{
    if (outcome.Result != ValidationResult.Pass)
    {
        failures.Add($"{id} expected PASS but failed: {outcome.Message}");
    }
}

void ExpectFail(string id, ValidationOutcome outcome)
{
    if (outcome.Result != ValidationResult.Fail)
    {
        failures.Add($"{id} expected FAIL but passed");
    }
}

var t0 = DateTimeOffset.Parse("2026-07-31T02:00:00+03:00");

var validFitness = new HealthFitnessAssessment(
    "hf-1",
    ContractVersions.Con006,
    "subject-1",
    "operate",
    "level-1",
    "HEALTHY",
    "FIT",
    "stage-2-scope",
    "evidence-1",
    "self-model-1",
    "0.99",
    "none",
    "clear",
    t0,
    t0.AddHours(1));

var validConfiguration = new ConfigurationAdmission(
    "cfg-1",
    ContractVersions.Con007,
    "foundation.toolchain.identity",
    "Falcon Core Authority",
    "approved-source",
    "stage-2",
    "LOCKED",
    "ADMITTED",
    "config-evidence-1",
    "config-authority",
    "",
    t0,
    t0.AddHours(1));

var validEvidence = new EvidenceLinkRecord(
    "evi-1",
    ContractVersions.Con008,
    "subject-1",
    "FALCON_NATIVE",
    "COMPLETE",
    "LINKED",
    "source-1",
    "target-1",
    "A1B2C3D4E5F60718293A4B5C6D7E8F90A1B2C3D4E5F60718293A4B5C6D7E8F90",
    "evidence-authority",
    "evidence-1",
    t0);

var validSecurity = new SecurityBoundaryRecord(
    "sec-1",
    ContractVersions.Con009,
    "subject-1",
    "FOUNDATION_APPLICATION_BOUNDARY",
    "security-authority",
    "core-only,platform-only",
    "business-logic,external-connectivity",
    "security-evidence-1",
    "COMPLIANT",
    t0,
    t0.AddHours(1));

var validManifest = new ManifestSurfaceRecord(
    "man-1",
    ContractVersions.Con010,
    "ACTIVATION_MANIFEST",
    "subject-1",
    "evidence-set-1",
    "SEPARATE",
    "INTACT",
    "manifest-authority",
    "manifest-evidence-1",
    "B7D4E8F90A1B2C3D4E5F60718293A4B5C6D7E8F90A1B2C3D4E5F60718293A4B5",
    t0,
    t0.AddHours(1));

var validRestriction = new RestrictionRecord(
    "rst-1",
    ContractVersions.Con011,
    "subject-1",
    "mandate-1",
    "trigger-evidence-1",
    "SAFE_MODE",
    "observe,repair",
    "execute,release",
    "release-conditions-1",
    "release-authority",
    "IMPOSED",
    "restriction-evidence-1",
    t0,
    t0.AddHours(1));

ExpectPass("CON-006", ContractValidators.Validate(validFitness));
ExpectPass("CON-007", ContractValidators.Validate(validConfiguration));
ExpectPass("CON-008", ContractValidators.Validate(validEvidence));
ExpectPass("CON-009", ContractValidators.Validate(validSecurity));
ExpectPass("CON-010", ContractValidators.Validate(validManifest));
ExpectPass("CON-011", ContractValidators.Validate(validRestriction));

ExpectFail("CON-006-NEG-VERSION", ContractValidators.Validate(validFitness with { Version = "1.0" }));
ExpectFail("CON-006-NEG-STATE", ContractValidators.Validate(validFitness with { FitnessResult = "MAYBE" }));

ExpectFail("CON-007-NEG-VERSION", ContractValidators.Validate(validConfiguration with { Version = "0.9" }));
ExpectFail("CON-007-NEG-REJECTION", ContractValidators.Validate(validConfiguration with { ResolutionResult = "PENDING" }));

ExpectFail("CON-008-NEG-LINKAGE", ContractValidators.Validate(validEvidence with { LinkageState = "UNLINKED" }));
ExpectFail("CON-008-NEG-DIGEST", ContractValidators.Validate(validEvidence with { Digest = "1234" }));

ExpectFail("CON-009-NEG-BOUNDARY", ContractValidators.Validate(validSecurity with { BoundaryResult = "VIOLATION" }));
ExpectFail("CON-009-NEG-VERSION", ContractValidators.Validate(validSecurity with { Version = "9.9" }));

ExpectFail("CON-010-NEG-SEPARATION", ContractValidators.Validate(validManifest with { SeparationResult = "MIXED" }));
ExpectFail("CON-010-NEG-INTEGRITY", ContractValidators.Validate(validManifest with { CanonicalDigest = "bad-digest" }));

ExpectFail("CON-011-NEG-RESULT", ContractValidators.Validate(validRestriction with { Result = "REJECTED" }));
ExpectFail("CON-011-NEG-VERSION", ContractValidators.Validate(validRestriction with { Version = "9.9" }));

var implementedContracts = new[]
{
    ContractIdentity.Con006,
    ContractIdentity.Con007,
    ContractIdentity.Con008,
    ContractIdentity.Con009,
    ContractIdentity.Con010,
    ContractIdentity.Con011
};

if (implementedContracts.Distinct(StringComparer.Ordinal).Count() != 6)
{
    failures.Add("Duplicate or missing contract implementation detected");
}

if (!implementedContracts.Contains(ContractIdentity.Con006, StringComparer.Ordinal) ||
    !implementedContracts.Contains(ContractIdentity.Con007, StringComparer.Ordinal) ||
    !implementedContracts.Contains(ContractIdentity.Con008, StringComparer.Ordinal) ||
    !implementedContracts.Contains(ContractIdentity.Con009, StringComparer.Ordinal) ||
    !implementedContracts.Contains(ContractIdentity.Con010, StringComparer.Ordinal) ||
    !implementedContracts.Contains(ContractIdentity.Con011, StringComparer.Ordinal))
{
    failures.Add("Contract coverage incomplete");
}


ExpectFail("CON-006-NULL", ContractValidators.Validate((HealthFitnessAssessment?)null));
ExpectFail("CON-007-NULL", ContractValidators.Validate((ConfigurationAdmission?)null));
ExpectFail("CON-008-NULL", ContractValidators.Validate((EvidenceLinkRecord?)null));
ExpectFail("CON-009-NULL", ContractValidators.Validate((SecurityBoundaryRecord?)null));
ExpectFail("CON-010-NULL", ContractValidators.Validate((ManifestSurfaceRecord?)null));
ExpectFail("CON-011-NULL", ContractValidators.Validate((RestrictionRecord?)null));
ExpectFail("CON-008-NULL-DIGEST", ContractValidators.Validate(validEvidence with { Digest = null! }));
ExpectFail("CON-010-NULL-DIGEST", ContractValidators.Validate(validManifest with { CanonicalDigest = null! }));

if (failures.Count > 0)
{
    Console.Error.WriteLine("Stage 2 WP-02: FAIL");
    foreach (var failure in failures)
    {
        Console.Error.WriteLine("- " + failure);
    }
    return 1;
}

Console.WriteLine("Stage 2 WP-02: PASS");
Console.WriteLine("CON-006 through CON-011 implemented exactly once.");
Console.WriteLine("Valid contract instances passed; malformed, incomplete, unauthorized, conflicting, incorrectly versioned, and incorrectly linked instances failed closed.");
return 0;
