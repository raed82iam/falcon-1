using System;
using System.Runtime.CompilerServices;
using Foundation.Authority;
using Foundation.Contracts;

namespace Falcon.Stage8.WP07.Verifier;

internal static class SafeAllowlistTamperGuard
{
    [ModuleInitializer]
    internal static void ValidateAuthorityConsumerFailsClosed()
    {
        var now = new DateTimeOffset(2026, 8, 15, 0, 0, 0, TimeSpan.Zero);
        var forged = new RestrictionRecord(
            "restriction:stage8:wp07:tampered-allowlist",
            ContractVersions.Con011,
            "foundation-subject:wp07",
            "authority:guardian:approved",
            "guardian-evidence:wp07",
            "SAFE",
            ProtectiveSafeStateContractPolicy.CanonicalAllowedSafeActions + "|EXECUTE",
            "*",
            "STAGE9_INDEPENDENT_RECOVERY_VALIDATION_AND_AUTHORIZED_RELEASE_REQUIRED",
            "INDEPENDENT_GOVERNED_RELEASE_AUTHORITY",
            "IMPOSED",
            "tampered-integrity-evidence",
            now.AddMinutes(-10),
            DateTimeOffset.MaxValue);

        var outcome = new ProtectiveRestrictionAuthorityEnforcer().Evaluate(null, null, new[] { forged });
        if (outcome.Decision != AuthorityDecision.Deny ||
            outcome.Reason != ProtectiveAuthorityReason.RestrictionMalformed)
        {
            throw new InvalidOperationException(
                "WP-07 SAFE allowlist expansion was not rejected fail-closed by Authority.");
        }

        Console.WriteLine("WP07_SAFE_ALLOWLIST_TAMPER_GUARD = PASS");
    }
}
