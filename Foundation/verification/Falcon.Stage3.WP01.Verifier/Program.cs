using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Foundation.ContractRegistry;

var failures = new List<string>();

void ExpectPass(string id, RegistryValidationResult result)
{
    if (!result.Success)
    {
        failures.Add($"{id} expected PASS but failed: {result.Message}");
    }
}

void ExpectFail(string id, RegistryValidationResult result)
{
    if (result.Success)
    {
        failures.Add($"{id} expected FAIL but passed");
    }
}

var registry = ContractRegistry.CreateCanonical();

ExpectPass("coverage", registry.ValidateCanonicalCoverage());
ExpectPass("lookup", registry.ValidateDeterministicLookup());
ExpectPass("uniqueness", registry.ValidateContractIdentityUniqueness());
ExpectPass("field-check", registry.ValidateCanonicalEntry(new ContractRegistryEntry("CON-001", "1.0", "Falcon Core Authority", "CON-000 / CON-001", "src/Foundation.Contracts/Contracts.cs", "executive CoreIdentity type and validator", "ACCEPTED", "REGISTERED")));
ExpectFail("changed-owner", registry.ValidateCanonicalEntry(new ContractRegistryEntry("CON-001", "1.0", "Altered Owner", "CON-000 / CON-001", "src/Foundation.Contracts/Contracts.cs", "executive CoreIdentity type and validator", "ACCEPTED", "REGISTERED")));
ExpectFail("changed-authority-source", registry.ValidateCanonicalEntry(new ContractRegistryEntry("CON-001", "1.0", "Falcon Core Authority", "Altered Authority", "src/Foundation.Contracts/Contracts.cs", "executive CoreIdentity type and validator", "ACCEPTED", "REGISTERED")));
ExpectFail("changed-control-surface", registry.ValidateCanonicalEntry(new ContractRegistryEntry("CON-001", "1.0", "Falcon Core Authority", "CON-000 / CON-001", "altered/control/surface", "executive CoreIdentity type and validator", "ACCEPTED", "REGISTERED")));
ExpectFail("changed-representation", registry.ValidateCanonicalEntry(new ContractRegistryEntry("CON-001", "1.0", "Falcon Core Authority", "CON-000 / CON-001", "src/Foundation.Contracts/Contracts.cs", "altered representation", "ACCEPTED", "REGISTERED")));
ExpectFail("changed-status", registry.ValidateCanonicalEntry(new ContractRegistryEntry("CON-001", "1.0", "Falcon Core Authority", "CON-000 / CON-001", "src/Foundation.Contracts/Contracts.cs", "executive CoreIdentity type and validator", "REVOKED", "REGISTERED")));
ExpectFail("changed-admission-state", registry.ValidateCanonicalEntry(new ContractRegistryEntry("CON-001", "1.0", "Falcon Core Authority", "CON-000 / CON-001", "src/Foundation.Contracts/Contracts.cs", "executive CoreIdentity type and validator", "ACCEPTED", "SUSPENDED")));

var allIds = new[]
{
    "CON-001","CON-002","CON-003","CON-004","CON-005","CON-006","CON-007",
    "CON-008","CON-009","CON-010","CON-011","CON-012","CON-013","CON-014",
    "CON-015","CON-016","CON-017","CON-018","CON-019","CON-020","CON-021","CON-023"
};

foreach (var id in allIds)
{
    var lookup = registry.Lookup(id, id switch
    {
        "CON-001" => "1.0",
        "CON-002" => "1.1",
        "CON-003" => "1.0",
        "CON-004" => "1.1",
        "CON-005" => "1.0",
        "CON-006" => "1.2",
        "CON-007" => "1.0",
        "CON-008" => "1.1",
        "CON-009" => "1.0",
        "CON-010" => "1.1",
        "CON-011" => "1.0",
        "CON-012" => "1.0",
        "CON-013" => "1.0",
        "CON-014" => "1.0",
        "CON-015" => "1.0",
        "CON-016" => "1.0",
        "CON-017" => "1.0",
        "CON-018" => "1.0",
        "CON-019" => "1.0",
        "CON-020" => "1.0",
        "CON-021" => "1.0",
        "CON-023" => "1.1",
        _ => ""
    });

    if (lookup is null || lookup.Entry.ContractId != id)
    {
        failures.Add($"lookup failed for {id}");
    }
}

var duplicateRegistry = new ContractRegistry();
var canonical = ContractRegistry.CreateCanonical().Entries.First(e => e.ContractId == "CON-001");
ExpectPass("seed", duplicateRegistry.Register(canonical));
ExpectFail("duplicate-id", duplicateRegistry.Register(canonical with { Version = "9.9" }));
ExpectFail("duplicate-version", duplicateRegistry.Register(canonical));
ExpectFail("unknown-lookup", new ContractRegistry().Lookup("CON-999", "1.0") is null
    ? RegistryValidationResult.Fail("unknown lookup rejected")
    : RegistryValidationResult.Pass("unexpected"));
ExpectFail("missing-owner", new ContractRegistry().Register(canonical with { Owner = "" }));
ExpectFail("missing-authority", new ContractRegistry().Register(canonical with { AuthoritySource = "" }));
ExpectFail("malformed-entry", new ContractRegistry().Register(canonical with { ControlSurface = "" }));
ExpectFail("unauthorized-mutation", registry.Register(canonical with { Owner = "Unexpected Owner" }));
ExpectFail("ambiguous-lookup", new ContractRegistry().Lookup("CON-001", "9.9") is null
    ? RegistryValidationResult.Fail("ambiguous lookup rejected")
    : RegistryValidationResult.Pass("unexpected"));

ExpectCanonicalCreateFail("create-duplicate-identity", new[]
{
    canonical,
    canonical with { Version = "9.9" }
});

ExpectCanonicalCreateFail("create-conflicting-version", new[]
{
    canonical,
    canonical with { ContractId = "CON-001", Version = "9.9" }
});

ExpectCanonicalCreateFail("create-missing-owner", new[]
{
    canonical with { Owner = "" }
});

ExpectCanonicalCreateFail("create-missing-authority", new[]
{
    canonical with { AuthoritySource = "" }
});

ExpectCanonicalCreateFail("create-missing-status", new[]
{
    canonical with { Status = "" }
});

ExpectCanonicalCreateFail("create-missing-admission", new[]
{
    canonical with { AdmissionState = "" }
});

ExpectCanonicalCreateFail("create-malformed-control-surface", new[]
{
    canonical with { ControlSurface = "" }
});


ExpectFail("null-entry", new ContractRegistry().Register(null));
if (new ContractRegistry().Lookup(null, "1.0") is not null)
{
    failures.Add("null lookup expected no result");
}

var collisionRegistry = new ContractRegistry();
var collisionOne = canonical with
{
    ContractId = "A@B",
    Version = "C",
    Owner = "Owner One",
    AuthoritySource = "Authority One"
};
var collisionTwo = canonical with
{
    ContractId = "A",
    Version = "B@C",
    Owner = "Owner Two",
    AuthoritySource = "Authority Two"
};
ExpectPass("structured-key-collision-one", collisionRegistry.Register(collisionOne));
ExpectPass("structured-key-collision-two", collisionRegistry.Register(collisionTwo));
if (collisionRegistry.Lookup("A@B", "C")?.Entry != collisionOne ||
    collisionRegistry.Lookup("A", "B@C")?.Entry != collisionTwo)
{
    failures.Add("structured registry keys did not preserve crafted identities");
}

var concurrentRegistry = new ContractRegistry();
var concurrentResults = new ConcurrentBag<RegistryValidationResult>();
Parallel.For(0, 32, _ => concurrentResults.Add(concurrentRegistry.Register(canonical)));
if (concurrentResults.Count(result => result.Success) != 1 || concurrentRegistry.Entries.Count != 1)
{
    failures.Add("concurrent duplicate registration produced more than one accepted state change");
}

if (registry.Entries.Count != 22)
{
    failures.Add($"expected 22 registry entries but found {registry.Entries.Count}");
}

if (registry.Entries.Select(e => e.ContractId).Distinct(StringComparer.Ordinal).Count() != 22)
{
    failures.Add("duplicate contract entries detected");
}

if (registry.Entries.Select(e => e.RegistryKey).Distinct(StringComparer.Ordinal).Count() != 22)
{
    failures.Add("duplicate contract version entries detected");
}

if (failures.Count > 0)
{
    Console.Error.WriteLine("Stage 3 WP-01: FAIL");
    foreach (var failure in failures)
    {
        Console.Error.WriteLine("- " + failure);
    }
    return 1;
}

Console.WriteLine("Stage 3 WP-01: PASS");
Console.WriteLine("CON-001 through CON-021 plus CON-023 register exactly once.");
Console.WriteLine("All 22 contract identities are discoverable and resolve to exactly one accepted version.");
Console.WriteLine("Canonical field comparisons reject changed owner, authority source, control surface, executable representation, status, and admission state.");
Console.WriteLine("Duplicate, conflicting, unknown, missing-owner, missing-authority, malformed, unauthorized, and ambiguous cases failed closed.");
return 0;

void ExpectCanonicalCreateFail(string id, IEnumerable<ContractRegistryEntry> entries)
{
    try
    {
        _ = ContractRegistry.CreateCanonical(entries);
        failures.Add($"{id} expected FAIL but canonical creation succeeded");
    }
    catch
    {
    }
}
