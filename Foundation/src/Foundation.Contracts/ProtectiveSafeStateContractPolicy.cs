using System;

namespace Foundation.Contracts;

public static class ProtectiveSafeStateContractPolicy
{
    public const string CanonicalAllowedSafeActions =
        "REPORT_HEALTH|PUBLISH_EVIDENCE|COMPLY_WITH_PROTECTIVE_CONTROL";

    public static bool IsCanonicalSafeAllowlist(string? value)
        => string.Equals(value, CanonicalAllowedSafeActions, StringComparison.Ordinal);
}
