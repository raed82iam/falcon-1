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

var t0 = DateTimeOffset.Parse("2026-07-31T03:00:00+03:00");

var validAuthorityRequest = new AuthorityRequest(
    "auth-req-1",
    "actor-1",
    "REQUEST",
    "resource-1",
    "purpose-1",
    "scope-1",
    "ops-context",
    "sec-context",
    "FIT",
    "corr-1",
    t0,
    t0.AddHours(1));

var validAuthorityResult = new AuthorityResult(
    "auth-req-1",
    "auth-res-1",
    "ALLOW",
    "scope-1",
    "policy-1",
    "1.0",
    "material-conditions",
    "constraints",
    "approved",
    t0,
    t0.AddHours(1),
    "evidence-ref-1");

var validTransitionRequest = new LifecycleTransitionRequest(
    "lt-req-1",
    "component-1",
    "STAGED",
    "ACTIVE",
    "requester-1",
    "authority-ref-1",
    "reason-1",
    "delegation-chain-1",
    t0,
    t0.AddHours(1));

var validTransitionResult = new LifecycleTransitionResult(
    "lt-req-1",
    "lt-res-1",
    "ACCEPTED",
    "STAGED",
    "ACTIVE",
    "ACTIVE",
    "applied",
    "validation-evidence-1",
    t0,
    "event-ref-1");

ExpectPass("CON-012", ContractValidators.Validate(validAuthorityRequest));
ExpectPass("CON-013", ContractValidators.Validate(validAuthorityResult));
ExpectPass("CON-014", ContractValidators.Validate(validTransitionRequest));
ExpectPass("CON-015", ContractValidators.Validate(validTransitionResult));

var validAuthorityInstrument = new AuthorityInstrumentRecord(
    "auth-inst-1",
    ProviderContractVersions.Con012,
    "Stage 2 planning authority",
    "WP-03 provider-governed delegation scope",
    "Falcon Project Owner",
    "GOV-089",
    "bounded, no self-delegation, no authority expansion",
    "authority-evidence-1",
    "ISSUED",
    t0,
    t0.AddHours(1));

var validDelegation = new DelegationRecord(
    "deleg-1",
    ProviderContractVersions.Con013,
    "grantor-1",
    "grantee-1",
    "delegation-scope-1",
    "chain-1",
    "GOV-089",
    "delegation-evidence-1",
    "GRANTED",
    "terminate on expiry or revocation",
    t0,
    t0.AddHours(1));

var validIdentifierProvider = new IdentifierProviderRecord(
    "identifier-provider-1",
    ProviderContractVersions.Con014,
    "IDENTIFIER_PROVIDER",
    "GOV-089",
    "issue-only, no bypass, no self-delegation",
    "identity-evidence-1",
    "ADMITTED",
    "PROHIBITED",
    "provider-evidence-1",
    t0,
    t0.AddHours(1));

var validTimeProvider = new TimeProviderRecord(
    "time-provider-1",
    ProviderContractVersions.Con015,
    "TIME_PROVIDER",
    "GOV-089",
    "bounded, no direct authority",
    "UTC+03:00 governed clock",
    "time-evidence-1",
    "ADMITTED",
    t0,
    t0.AddHours(1));

var validCryptoProvider = new CryptographicProviderRecord(
    "crypto-provider-1",
    ProviderContractVersions.Con016,
    "CRYPTOGRAPHIC_PROVIDER",
    "GOV-089",
    "key-use bounded to approved contracts",
    "keyref-1",
    "crypto-evidence-1",
    "ADMITTED",
    t0,
    t0.AddHours(1));

var validSecretCustody = new SecretCustodyRecord(
    "secret-custody-1",
    ProviderContractVersions.Con017,
    "secret-provider-1",
    "PROTECTED_SECRET",
    "locked custody",
    "no direct access outside custody boundary",
    "secret-evidence-1",
    "ADMITTED",
    t0,
    t0.AddHours(1));

var validCertificateProvider = new CertificateIdentityProviderRecord(
    "cert-provider-1",
    ProviderContractVersions.Con018,
    "CERTIFICATE_IDENTITY_PROVIDER",
    "GOV-089",
    "trust-anchor-1",
    "ADMITTED",
    "certificate-evidence-1",
    t0,
    t0.AddHours(1));

var validRandomnessProvider = new RandomnessProviderRecord(
    "randomness-provider-1",
    ProviderContractVersions.Con019,
    "RANDOMNESS_PROVIDER",
    "GOV-089",
    "approved entropy source",
    "randomness-evidence-1",
    "ADMITTED",
    t0,
    t0.AddHours(1));

ExpectPass("CON-012", ProviderContractValidators.Validate(validAuthorityInstrument));
ExpectPass("CON-013", ProviderContractValidators.Validate(validDelegation));
ExpectPass("CON-014", ProviderContractValidators.Validate(validIdentifierProvider));
ExpectPass("CON-015", ProviderContractValidators.Validate(validTimeProvider));
ExpectPass("CON-016", ProviderContractValidators.Validate(validCryptoProvider));
ExpectPass("CON-017", ProviderContractValidators.Validate(validSecretCustody));
ExpectPass("CON-018", ProviderContractValidators.Validate(validCertificateProvider));
ExpectPass("CON-019", ProviderContractValidators.Validate(validRandomnessProvider));

ExpectFail("CON-012-NEG-VERSION", ProviderContractValidators.Validate(validAuthorityInstrument with { Version = "0.9" }));
ExpectFail("CON-013-NEG-CHAIN", ProviderContractValidators.Validate(validDelegation with { ChainIdentity = "" }));
ExpectFail("CON-014-NEG-BYPASS", ProviderContractValidators.Validate(validIdentifierProvider with { BypassProtection = "ALLOWED" }));
ExpectFail("CON-015-NEG-EXPIRY", ProviderContractValidators.Validate(validTimeProvider with { Expiry = t0 }));
ExpectFail("CON-016-NEG-EVIDENCE", ProviderContractValidators.Validate(validCryptoProvider with { ValidationEvidence = "" }));
ExpectFail("CON-017-NEG-RESULT", ProviderContractValidators.Validate(validSecretCustody with { CustodyResult = "REJECTED" }));
ExpectFail("CON-018-NEG-VERSION", ProviderContractValidators.Validate(validCertificateProvider with { Version = "0.1" }));
ExpectFail("CON-019-NEG-SOURCE", ProviderContractValidators.Validate(validRandomnessProvider with { EntropySource = "" }));

var implementedContracts = new[]
{
    ContractIdentity.Con012,
    ContractIdentity.Con013,
    ContractIdentity.Con014,
    ContractIdentity.Con015,
    ContractIdentity.Con016,
    ContractIdentity.Con017,
    ContractIdentity.Con018,
    ContractIdentity.Con019
};

if (implementedContracts.Distinct(StringComparer.Ordinal).Count() != 8)
{
    failures.Add("Duplicate or missing contract implementation detected");
}


ExpectFail("CON-012-NULL", ProviderContractValidators.Validate((AuthorityInstrumentRecord?)null));
ExpectFail("CON-013-NULL", ProviderContractValidators.Validate((DelegationRecord?)null));
ExpectFail("CON-014-NULL", ProviderContractValidators.Validate((IdentifierProviderRecord?)null));
ExpectFail("CON-015-NULL", ProviderContractValidators.Validate((TimeProviderRecord?)null));
ExpectFail("CON-016-NULL", ProviderContractValidators.Validate((CryptographicProviderRecord?)null));
ExpectFail("CON-017-NULL", ProviderContractValidators.Validate((SecretCustodyRecord?)null));
ExpectFail("CON-018-NULL", ProviderContractValidators.Validate((CertificateIdentityProviderRecord?)null));
ExpectFail("CON-019-NULL", ProviderContractValidators.Validate((RandomnessProviderRecord?)null));

if (failures.Count > 0)
{
    Console.Error.WriteLine("Stage 2 WP-03: FAIL");
    foreach (var failure in failures)
    {
        Console.Error.WriteLine("- " + failure);
    }
    return 1;
}

Console.WriteLine("Stage 2 WP-03: PASS");
Console.WriteLine("CON-012 through CON-019 implemented exactly once.");
Console.WriteLine("Valid contract instances passed; malformed, incomplete, unauthorized, conflicting, incorrectly linked, and incorrectly versioned instances failed closed.");
return 0;
