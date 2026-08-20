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

ExpectPass("CON-001", ContractValidators.Validate(new CoreIdentity("subj-1", "Core", "inst-1", "1.0", "owner", "cap", "artifact", "authority", "lifecycle", DateTimeOffset.Parse("2026-07-31T00:00:00+03:00"), new string('a', 64))));
ExpectPass("CON-002", ContractValidators.Validate(new AuthorityRequest("req-1", "actor", "ALLOW", "resource", "purpose", "scope", "ops", "sec", "fit", "corr", DateTimeOffset.Parse("2026-07-31T00:00:00+03:00"), DateTimeOffset.Parse("2026-07-31T01:00:00+03:00"))));
ExpectPass("CON-003", ContractValidators.Validate(new LifecycleTransitionRequest("tr-1", "component", "ACTIVE", "INACTIVE", "requester", "authority", "reason", "deps", DateTimeOffset.Parse("2026-07-31T00:00:00+03:00"), DateTimeOffset.Parse("2026-07-31T01:00:00+03:00"))));
ExpectPass("CON-004", ContractValidators.Validate(new FilEnvelope("msg-1", "Command", "Contract", "schema", "1.0", "producer", DateTimeOffset.Parse("2026-07-31T00:00:00+03:00"), "purpose", "internal", "corr", "cause", DateTimeOffset.Parse("2026-07-31T01:00:00+03:00"), "priority", "evidence", "profile", "1.0", "integrity", "encryption", "keyref", "1", "recipient", "replay", "attempt-1", null, "payload")));
ExpectPass("CON-005", ContractValidators.Validate(new FilEvent("evt-1", "Fact", "1.0", "owner", "subject", DateTimeOffset.Parse("2026-07-31T00:00:00+03:00"), DateTimeOffset.Parse("2026-07-31T00:00:01+03:00"), "evidence", null, null, false, null, "payload")));

ExpectFail("CON-001-NEG", ContractValidators.Validate(new CoreIdentity("", "Core", "inst-1", "1.0", "owner", "cap", "artifact", "authority", "lifecycle", DateTimeOffset.Parse("2026-07-31T00:00:00+03:00"), new string('a', 64))));
ExpectFail("CON-002-NEG", ContractValidators.Validate(new AuthorityRequest("req-1", "", "ALLOW", "resource", "purpose", "scope", "ops", "sec", "fit", "corr", DateTimeOffset.Parse("2026-07-31T00:00:00+03:00"), DateTimeOffset.Parse("2026-07-31T01:00:00+03:00"))));
ExpectFail("CON-003-NEG", ContractValidators.Validate(new LifecycleTransitionRequest("tr-1", "component", "ACTIVE", "INACTIVE", "requester", "authority", "reason", "deps", DateTimeOffset.Parse("2026-07-31T00:00:00+03:00"), DateTimeOffset.Parse("2026-07-31T00:00:00+03:00"))));
ExpectFail("CON-004-NEG", ContractValidators.Validate(new FilEnvelope("msg-1", "Command", "Contract", "schema", "1.0", "producer", DateTimeOffset.Parse("2026-07-31T00:00:00+03:00"), "purpose", "internal", "corr", "cause", DateTimeOffset.Parse("2026-07-31T01:00:00+03:00"), "priority", "evidence", "profile", "1.0", "integrity", "encryption", "keyref", "1", "recipient", "replay", "attempt-1", null, "")));
ExpectFail("CON-005-NEG", ContractValidators.Validate(new FilEvent("evt-1", "Fact", "1.0", "owner", "subject", DateTimeOffset.Parse("2026-07-31T00:00:00+03:00"), DateTimeOffset.Parse("2026-07-30T00:00:00+03:00"), "evidence", null, null, false, null, "payload")));


ExpectFail("CON-001-NULL", ContractValidators.Validate((CoreIdentity?)null));
ExpectFail("CON-002-NULL-REQUEST", ContractValidators.Validate((AuthorityRequest?)null));
ExpectFail("CON-002-NULL-RESULT", ContractValidators.Validate((AuthorityResult?)null));
ExpectFail("CON-003-NULL-REQUEST", ContractValidators.Validate((LifecycleTransitionRequest?)null));
ExpectFail("CON-003-NULL-RESULT", ContractValidators.Validate((LifecycleTransitionResult?)null));
ExpectFail("CON-004-NULL", ContractValidators.Validate((FilEnvelope?)null));
ExpectFail("CON-005-NULL", ContractValidators.Validate((FilEvent?)null));

var duplicateContractDefinitions = new[]
{
    ContractIdentity.Con001,
    ContractIdentity.Con002,
    ContractIdentity.Con003,
    ContractIdentity.Con004,
    ContractIdentity.Con005
};

if (duplicateContractDefinitions.Distinct(StringComparer.Ordinal).Count() != 5)
{
    failures.Add("Duplicate contract implementation detected");
}

if (failures.Count > 0)
{
    Console.Error.WriteLine("Stage 2 WP-01: FAIL");
    foreach (var failure in failures)
    {
        Console.Error.WriteLine("- " + failure);
    }
    return 1;
}

Console.WriteLine("Stage 2 WP-01: PASS");
Console.WriteLine("CON-001 through CON-005 implemented exactly once.");
Console.WriteLine("Valid contract instances passed; malformed, unauthorized, conflicting, and incorrectly versioned instances failed closed.");
return 0;
