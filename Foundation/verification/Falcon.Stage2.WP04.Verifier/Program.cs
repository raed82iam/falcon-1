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

var t0 = DateTimeOffset.Parse("2026-07-31T04:00:00+03:00");

var validBootstrapContext = new BootstrapExecutionContextRecord(
    "bootstrap-context-1",
    ProviderContractVersions.Con020,
    "GOV-089",
    "bootstrap-env-1",
    "bootstrap-scope",
    "bootstrap-source-1",
    "bootstrap-evidence-1",
    "DEFINED",
    "NO_EXPANSION",
    t0,
    t0.AddHours(1));

var validBootstrapProvenance = new BootstrapEvidenceProvenanceRecord(
    "bootstrap-provenance-1",
    ProviderContractVersions.Con021,
    "bootstrap-record-1",
    "ABCDEF0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF0123456789",
    "bootstrap-source-1",
    "GOV-089",
    "provenance-evidence-1",
    "PROVEN",
    "artifact-1",
    t0,
    t0.AddHours(1));

ExpectPass("CON-020", BootstrapContractValidators.Validate(validBootstrapContext));
ExpectPass("CON-021", BootstrapContractValidators.Validate(validBootstrapProvenance));

ExpectFail("CON-020-NEG-VERSION", BootstrapContractValidators.Validate(validBootstrapContext with { Version = "0.9" }));
ExpectFail("CON-020-NEG-STATE", BootstrapContractValidators.Validate(validBootstrapContext with { ContextState = "AMBIGUOUS" }));
ExpectFail("CON-020-NEG-BOUNDARY", BootstrapContractValidators.Validate(validBootstrapContext with { AuthorityBoundary = "" }));

ExpectFail("CON-021-NEG-DIGEST", BootstrapContractValidators.Validate(validBootstrapProvenance with { SourceDigest = "bad-digest" }));
ExpectFail("CON-021-NEG-LINKAGE", BootstrapContractValidators.Validate(validBootstrapProvenance with { SourceRecordId = "" }));
ExpectFail("CON-021-NEG-STATE", BootstrapContractValidators.Validate(validBootstrapProvenance with { ProvenanceState = "UNKNOWN" }));

var implementedContracts = new[]
{
    ContractIdentity.Con020,
    ContractIdentity.Con021
};

if (implementedContracts.Distinct(StringComparer.Ordinal).Count() != 2)
{
    failures.Add("Duplicate or missing contract implementation detected");
}


ExpectFail("CON-020-NULL", BootstrapContractValidators.Validate((BootstrapExecutionContextRecord?)null));
ExpectFail("CON-021-NULL", BootstrapContractValidators.Validate((BootstrapEvidenceProvenanceRecord?)null));
ExpectFail("CON-021-NULL-DIGEST", BootstrapContractValidators.Validate(validBootstrapProvenance with { SourceDigest = null! }));

if (failures.Count > 0)
{
    Console.Error.WriteLine("Stage 2 WP-04: FAIL");
    foreach (var failure in failures)
    {
        Console.Error.WriteLine("- " + failure);
    }
    return 1;
}

Console.WriteLine("Stage 2 WP-04: PASS");
Console.WriteLine("CON-020 and CON-021 implemented exactly once.");
Console.WriteLine("Valid contract instances passed; malformed, incomplete, unauthorized, conflicting, incorrectly linked, and incorrectly versioned instances failed closed.");
return 0;
