namespace Falcon.FSATS.Trading.Contracts;

public enum WebFsatsFeatureAudience
{
    Unknown = 0,
    CustomerFacing = 1,
    InternalOnly = 2
}

public enum WebFsatsCommercialTier
{
    None = 0,
    Standard = 1,
    Vip = 2
}

public enum WebFsatsEntitlementSubjectKind
{
    Unknown = 0,
    ProjectOwner = 1,
    CommercialCustomer = 2
}

public enum WebFsatsEntitlementAuthoritySource
{
    Unspecified = 0,
    ProducerSelfClaim = 1,
    AuthoritativeOwnerIdentitySession = 2
}

public sealed record WebFsatsFeatureDefinition(
    string FeatureId,
    string FeatureVersion,
    WebFsatsFeatureAudience Audience,
    WebFsatsCommercialTier MinimumCommercialTier,
    bool Enabled,
    bool CustomerFeature,
    bool RequiresSeparateActionAuthorization,
    bool RequiresSeparateTradingExecutionAuthority,
    bool RequiresSeparateBrokerAuthority);

public sealed record WebFsatsFeatureCatalogSnapshot(
    string CatalogId,
    string CatalogVersion,
    string CatalogSha256,
    string ProvenanceReference,
    DateTimeOffset ObservedAt,
    DateTimeOffset ExpiresAt,
    IReadOnlyList<WebFsatsFeatureDefinition> Features);

public sealed record WebProjectOwnerIdentitySessionFacts(
    string SubjectId,
    string SessionId,
    string OwnerIdentityGovernanceVersion,
    string EvidenceReference,
    WebFsatsEntitlementAuthoritySource AuthoritySource,
    DateTimeOffset ObservedAt,
    DateTimeOffset ExpiresAt,
    bool IsCurrentProjectOwner,
    bool IsRevoked,
    bool IsSuperseded);

public sealed record WebProjectOwnerFeatureEntitlementRequest(
    string EntitlementId,
    string EntitlementVersion,
    string RequiredCatalogCompatibilityIdentity,
    WebFsatsEntitlementSubjectKind SubjectKind,
    WebProjectOwnerIdentitySessionFacts IdentitySession,
    WebFsatsFeatureCatalogSnapshot Catalog,
    string CatalogCompatibilityIdentity,
    DateTimeOffset EvaluationTime,
    bool TreatAsCommercialSubscription,
    bool TrialApplies,
    bool SevenDayWarningApplies,
    bool StandardDowngradeApplies,
    bool UpgradePromptApplies,
    bool StandardFeatureLockApplies,
    bool ActionAuthorizationRequested,
    bool TradingExecutionAuthorityRequested,
    bool BrokerAuthorityRequested,
    bool FoundationAuthorityRequested,
    bool KillAuthorityRequested,
    bool RuntimeActivationRequested,
    bool DeploymentRequested);

public sealed record WebProjectOwnerFeatureEntitlementDecision(
    bool Accepted,
    string ReasonCode,
    string EntitlementId,
    string EntitlementVersion,
    string SubjectId,
    string SessionId,
    string OwnerIdentityGovernanceVersion,
    string CatalogId,
    string CatalogVersion,
    string CatalogSha256,
    IReadOnlyList<string> GrantedFeatureIds,
    bool IncludesCurrentAndFutureVipCustomerFeatures,
    bool CommercialSubscriptionRequired,
    bool TrialApplies,
    bool SevenDayWarningApplies,
    bool StandardDowngradeApplies,
    bool UpgradePromptApplies,
    bool StandardFeatureLockApplies,
    bool ActionAuthorizationGranted,
    bool TradingExecutionAuthorityGranted,
    bool BrokerAuthorityGranted,
    bool FoundationAuthorityGranted,
    bool KillAuthorityGranted,
    bool RuntimeActivationAuthorized,
    bool DeploymentAuthorized,
    DateTimeOffset EvaluatedAt,
    DateTimeOffset EvidenceExpiresAt);

public static class WebProjectOwnerFeatureEntitlementGovernance
{
    public const string EntitlementId = "fsats.entitlement.project-owner.full-vip-or-greater";
    public const string EntitlementVersion = "1.0.0";
    public const string CatalogCompatibilityIdentity = "compat:fsats-customer-feature-catalog:v1";

    public static WebProjectOwnerFeatureEntitlementDecision Evaluate(WebProjectOwnerFeatureEntitlementRequest? request)
    {
        if (request is null)
            return Reject("ENTITLEMENT_REQUEST_MISSING");

        if (!StringComparer.Ordinal.Equals(request.EntitlementId, EntitlementId) ||
            !StringComparer.Ordinal.Equals(request.EntitlementVersion, EntitlementVersion))
            return Reject("ENTITLEMENT_CONTRACT_IDENTITY_MISMATCH", request);

        if (!StringComparer.Ordinal.Equals(request.RequiredCatalogCompatibilityIdentity, CatalogCompatibilityIdentity) ||
            !StringComparer.Ordinal.Equals(request.CatalogCompatibilityIdentity, CatalogCompatibilityIdentity))
            return Reject("FEATURE_CATALOG_COMPATIBILITY_MISMATCH", request);

        if (request.SubjectKind != WebFsatsEntitlementSubjectKind.ProjectOwner)
            return Reject("PROJECT_OWNER_SUBJECT_REQUIRED", request);

        if (!ValidIdentitySession(request.IdentitySession, request.EvaluationTime))
            return Reject("AUTHORITATIVE_PROJECT_OWNER_IDENTITY_SESSION_REQUIRED", request);

        if (!ValidCatalog(request.Catalog, request.EvaluationTime))
            return Reject("CURRENT_GOVERNED_FEATURE_CATALOG_REQUIRED", request);

        if (request.TreatAsCommercialSubscription || request.TrialApplies || request.SevenDayWarningApplies ||
            request.StandardDowngradeApplies || request.UpgradePromptApplies || request.StandardFeatureLockApplies)
            return Reject("PROJECT_OWNER_MUST_NOT_BE_TREATED_AS_COMMERCIAL_SUBSCRIPTION_OR_TRIAL", request);

        if (request.ActionAuthorizationRequested || request.TradingExecutionAuthorityRequested ||
            request.BrokerAuthorityRequested || request.FoundationAuthorityRequested || request.KillAuthorityRequested ||
            request.RuntimeActivationRequested || request.DeploymentRequested)
            return Reject("FEATURE_ENTITLEMENT_CANNOT_MINT_ACTION_OR_RUNTIME_AUTHORITY", request);

        var eligible = request.Catalog.Features
            .Where(IsOwnerFeatureEligible)
            .Select(feature => feature.FeatureId)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(id => id, StringComparer.Ordinal)
            .ToArray();

        var evidenceExpiry = request.IdentitySession.ExpiresAt <= request.Catalog.ExpiresAt
            ? request.IdentitySession.ExpiresAt
            : request.Catalog.ExpiresAt;

        return new WebProjectOwnerFeatureEntitlementDecision(
            true,
            "PROJECT_OWNER_FULL_VIP_OR_GREATER_FEATURE_ENTITLEMENT_GRANTED",
            EntitlementId,
            EntitlementVersion,
            request.IdentitySession.SubjectId,
            request.IdentitySession.SessionId,
            request.IdentitySession.OwnerIdentityGovernanceVersion,
            request.Catalog.CatalogId,
            request.Catalog.CatalogVersion,
            request.Catalog.CatalogSha256,
            eligible,
            true,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            request.EvaluationTime,
            evidenceExpiry);
    }

    public static bool IsOwnerFeatureEligible(WebFsatsFeatureDefinition? feature)
    {
        if (feature is null || !feature.Enabled || !feature.CustomerFeature ||
            feature.Audience != WebFsatsFeatureAudience.CustomerFacing)
            return false;

        if (feature.MinimumCommercialTier is not (WebFsatsCommercialTier.Standard or WebFsatsCommercialTier.Vip))
            return false;

        return Token(feature.FeatureId) && Token(feature.FeatureVersion);
    }

    private static bool ValidIdentitySession(WebProjectOwnerIdentitySessionFacts? facts, DateTimeOffset now)
    {
        if (facts is null || now == default ||
            facts.AuthoritySource != WebFsatsEntitlementAuthoritySource.AuthoritativeOwnerIdentitySession ||
            !facts.IsCurrentProjectOwner || facts.IsRevoked || facts.IsSuperseded ||
            facts.ObservedAt == default || facts.ExpiresAt <= facts.ObservedAt ||
            now < facts.ObservedAt || now >= facts.ExpiresAt)
            return false;

        return Token(facts.SubjectId) && Token(facts.SessionId) &&
               Token(facts.OwnerIdentityGovernanceVersion) && Token(facts.EvidenceReference);
    }

    private static bool ValidCatalog(WebFsatsFeatureCatalogSnapshot? catalog, DateTimeOffset now)
    {
        if (catalog is null || now == default || catalog.ObservedAt == default || catalog.ExpiresAt <= catalog.ObservedAt ||
            now < catalog.ObservedAt || now >= catalog.ExpiresAt ||
            !Token(catalog.CatalogId) || !Token(catalog.CatalogVersion) ||
            !Sha256(catalog.CatalogSha256) || !Token(catalog.ProvenanceReference) || catalog.Features is null)
            return false;

        var ids = new HashSet<string>(StringComparer.Ordinal);
        foreach (var feature in catalog.Features)
        {
            if (feature is null || !Token(feature.FeatureId) || !Token(feature.FeatureVersion) ||
                !Enum.IsDefined(feature.Audience) || !Enum.IsDefined(feature.MinimumCommercialTier) ||
                !ids.Add(feature.FeatureId))
                return false;
        }

        return true;
    }

    private static WebProjectOwnerFeatureEntitlementDecision Reject(
        string reason,
        WebProjectOwnerFeatureEntitlementRequest? request = null)
        => new(
            false,
            reason,
            request?.EntitlementId ?? string.Empty,
            request?.EntitlementVersion ?? string.Empty,
            request?.IdentitySession?.SubjectId ?? string.Empty,
            request?.IdentitySession?.SessionId ?? string.Empty,
            request?.IdentitySession?.OwnerIdentityGovernanceVersion ?? string.Empty,
            request?.Catalog?.CatalogId ?? string.Empty,
            request?.Catalog?.CatalogVersion ?? string.Empty,
            request?.Catalog?.CatalogSha256 ?? string.Empty,
            Array.Empty<string>(),
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            request?.EvaluationTime ?? default,
            default);

    private static bool Token(string? value) => !string.IsNullOrWhiteSpace(value);

    private static bool Sha256(string? value)
        => value is { Length: 64 } && value.All(c => c is >= '0' and <= '9' or >= 'A' and <= 'F');
}
