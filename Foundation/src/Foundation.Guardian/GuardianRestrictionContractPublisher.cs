using System;
using Foundation.Contracts;

namespace Foundation.Guardian;

public static class GuardianRestrictionContractPublisher
{
    public static RestrictionRecord Publish(
        GuardianProtectiveRestriction restriction,
        GuardianProtectiveDecision sourceDecision)
    {
        var validation = GuardianProtectiveRestrictionRuntime.Validate(restriction, sourceDecision);
        if (!validation.Success)
            throw new ArgumentException("Invalid Guardian restriction: " + validation.Reason, nameof(restriction));

        return new RestrictionRecord(
            restriction.RestrictionId,
            ContractVersions.Con011,
            restriction.TargetId,
            sourceDecision.AuthorityReference,
            restriction.EvidenceReference,
            restriction.Severity switch
            {
                GuardianRestrictionSeverity.Moderate => "RESTRICTED",
                GuardianRestrictionSeverity.High => "ISOLATED",
                GuardianRestrictionSeverity.Critical => "SAFE",
                _ => "SAFE"
            },
            ProtectiveSafeStateContractPolicy.CanonicalAllowedSafeActions,
            "*",
            "STAGE9_INDEPENDENT_RECOVERY_VALIDATION_AND_AUTHORIZED_RELEASE_REQUIRED",
            "INDEPENDENT_GOVERNED_RELEASE_AUTHORITY",
            "IMPOSED",
            restriction.Identity,
            restriction.EffectiveAt,
            DateTimeOffset.MaxValue);
    }
}
