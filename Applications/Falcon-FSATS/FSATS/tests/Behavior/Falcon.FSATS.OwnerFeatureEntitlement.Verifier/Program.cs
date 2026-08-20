using Falcon.FSATS.Trading.Contracts;

var checks = new List<(string Name, bool Pass)>();
void Check(string name, bool pass) => checks.Add((name, pass));

var now = DateTimeOffset.UtcNow;
const string catalogHash = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA";

var features = new List<WebFsatsFeatureDefinition>
{
    new("feature.standard.dashboard", "1.0.0", WebFsatsFeatureAudience.CustomerFacing, WebFsatsCommercialTier.Standard, true, true, false, false, false),
    new("feature.vip.analysis", "1.0.0", WebFsatsFeatureAudience.CustomerFacing, WebFsatsCommercialTier.Vip, true, true, false, false, false),
    new("feature.internal.debug", "1.0.0", WebFsatsFeatureAudience.InternalOnly, WebFsatsCommercialTier.Vip, true, false, false, false, false),
    new("feature.vip.disabled", "1.0.0", WebFsatsFeatureAudience.CustomerFacing, WebFsatsCommercialTier.Vip, false, true, false, false, false),
    new("feature.vip.execution-ui", "1.0.0", WebFsatsFeatureAudience.CustomerFacing, WebFsatsCommercialTier.Vip, true, true, true, true, true)
};

var identity = new WebProjectOwnerIdentitySessionFacts(
    "owner:project",
    "session:owner:1",
    "owner-identity-governance:v1",
    "evidence:owner-session:1",
    WebFsatsEntitlementAuthoritySource.AuthoritativeOwnerIdentitySession,
    now.AddMinutes(-1),
    now.AddMinutes(30),
    true,
    false,
    false);

var catalog = new WebFsatsFeatureCatalogSnapshot(
    "fsats.customer-features",
    "2026.08.18.1",
    catalogHash,
    "evidence:fsats-feature-catalog:2026.08.18.1",
    now.AddMinutes(-1),
    now.AddMinutes(20),
    features);

var request = new WebProjectOwnerFeatureEntitlementRequest(
    WebProjectOwnerFeatureEntitlementGovernance.EntitlementId,
    WebProjectOwnerFeatureEntitlementGovernance.EntitlementVersion,
    WebProjectOwnerFeatureEntitlementGovernance.CatalogCompatibilityIdentity,
    WebFsatsEntitlementSubjectKind.ProjectOwner,
    identity,
    catalog,
    WebProjectOwnerFeatureEntitlementGovernance.CatalogCompatibilityIdentity,
    now,
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
    false);

var decision = WebProjectOwnerFeatureEntitlementGovernance.Evaluate(request);
Check("FCR0242 authoritative Project Owner entitlement accepted", decision.Accepted);
Check("FCR0242 Standard customer feature included", decision.GrantedFeatureIds.Contains("feature.standard.dashboard", StringComparer.Ordinal));
Check("FCR0242 VIP customer feature included", decision.GrantedFeatureIds.Contains("feature.vip.analysis", StringComparer.Ordinal));
Check("FCR0242 internal feature excluded", !decision.GrantedFeatureIds.Contains("feature.internal.debug", StringComparer.Ordinal));
Check("FCR0242 disabled feature excluded", !decision.GrantedFeatureIds.Contains("feature.vip.disabled", StringComparer.Ordinal));
Check("FCR0242 execution UI feature remains accessible", decision.GrantedFeatureIds.Contains("feature.vip.execution-ui", StringComparer.Ordinal));
Check("FCR0242 current and future VIP rule declared", decision.IncludesCurrentAndFutureVipCustomerFeatures);
Check("FCR0242 Owner not commercial subscription", !decision.CommercialSubscriptionRequired);
Check("FCR0242 Owner not trial", !decision.TrialApplies);
Check("FCR0242 no seven-day warning", !decision.SevenDayWarningApplies);
Check("FCR0242 no Standard downgrade", !decision.StandardDowngradeApplies);
Check("FCR0242 no upgrade prompt", !decision.UpgradePromptApplies);
Check("FCR0242 no Standard feature lock", !decision.StandardFeatureLockApplies);
Check("FCR0242 entitlement grants no action authority", !decision.ActionAuthorizationGranted);
Check("FCR0242 entitlement grants no trading authority", !decision.TradingExecutionAuthorityGranted);
Check("FCR0242 entitlement grants no broker authority", !decision.BrokerAuthorityGranted);
Check("FCR0242 entitlement grants no Foundation authority", !decision.FoundationAuthorityGranted);
Check("FCR0242 entitlement grants no Kill authority", !decision.KillAuthorityGranted);
Check("FCR0242 entitlement grants no runtime activation", !decision.RuntimeActivationAuthorized);
Check("FCR0242 entitlement grants no deployment", !decision.DeploymentAuthorized);
Check("FCR0242 decision expiry is bounded by earliest evidence expiry", decision.EvidenceExpiresAt == catalog.ExpiresAt);

Check("FCR0242 producer self-claim rejected",
    !WebProjectOwnerFeatureEntitlementGovernance.Evaluate(request with
    {
        IdentitySession = identity with { AuthoritySource = WebFsatsEntitlementAuthoritySource.ProducerSelfClaim }
    }).Accepted);

Check("FCR0242 commercial VIP customer is not Project Owner",
    !WebProjectOwnerFeatureEntitlementGovernance.Evaluate(request with { SubjectKind = WebFsatsEntitlementSubjectKind.CommercialCustomer }).Accepted);

Check("FCR0242 revoked Owner rejected",
    !WebProjectOwnerFeatureEntitlementGovernance.Evaluate(request with { IdentitySession = identity with { IsRevoked = true } }).Accepted);

Check("FCR0242 superseded Owner session rejected",
    !WebProjectOwnerFeatureEntitlementGovernance.Evaluate(request with { IdentitySession = identity with { IsSuperseded = true } }).Accepted);

Check("FCR0242 expired Owner session rejected",
    !WebProjectOwnerFeatureEntitlementGovernance.Evaluate(request with
    {
        IdentitySession = identity with { ObservedAt = now.AddHours(-2), ExpiresAt = now.AddHours(-1) }
    }).Accepted);

Check("FCR0242 contract version mismatch rejected",
    !WebProjectOwnerFeatureEntitlementGovernance.Evaluate(request with { EntitlementVersion = "2.0.0" }).Accepted);

Check("FCR0242 catalog compatibility mismatch rejected",
    !WebProjectOwnerFeatureEntitlementGovernance.Evaluate(request with { CatalogCompatibilityIdentity = "compat:wrong" }).Accepted);

Check("FCR0242 malformed catalog digest rejected",
    !WebProjectOwnerFeatureEntitlementGovernance.Evaluate(request with { Catalog = catalog with { CatalogSha256 = "BAD" } }).Accepted);

Check("FCR0242 future-dated catalog rejected",
    !WebProjectOwnerFeatureEntitlementGovernance.Evaluate(request with { Catalog = catalog with { ObservedAt = now.AddMinutes(1) } }).Accepted);

Check("FCR0242 expired catalog rejected",
    !WebProjectOwnerFeatureEntitlementGovernance.Evaluate(request with
    {
        Catalog = catalog with { ObservedAt = now.AddHours(-2), ExpiresAt = now.AddHours(-1) }
    }).Accepted);

Check("FCR0242 duplicate feature identity rejected",
    !WebProjectOwnerFeatureEntitlementGovernance.Evaluate(request with
    {
        Catalog = catalog with { Features = new[] { features[0], features[0] } }
    }).Accepted);

Check("FCR0242 trial semantics rejected",
    !WebProjectOwnerFeatureEntitlementGovernance.Evaluate(request with { TrialApplies = true }).Accepted);

Check("FCR0242 downgrade semantics rejected",
    !WebProjectOwnerFeatureEntitlementGovernance.Evaluate(request with { StandardDowngradeApplies = true }).Accepted);

Check("FCR0242 action-authority smuggling rejected",
    !WebProjectOwnerFeatureEntitlementGovernance.Evaluate(request with { ActionAuthorizationRequested = true }).Accepted);

Check("FCR0242 trading-authority smuggling rejected",
    !WebProjectOwnerFeatureEntitlementGovernance.Evaluate(request with { TradingExecutionAuthorityRequested = true }).Accepted);

Check("FCR0242 broker-authority smuggling rejected",
    !WebProjectOwnerFeatureEntitlementGovernance.Evaluate(request with { BrokerAuthorityRequested = true }).Accepted);

Check("FCR0242 Foundation-authority smuggling rejected",
    !WebProjectOwnerFeatureEntitlementGovernance.Evaluate(request with { FoundationAuthorityRequested = true }).Accepted);

Check("FCR0242 Kill-authority smuggling rejected",
    !WebProjectOwnerFeatureEntitlementGovernance.Evaluate(request with { KillAuthorityRequested = true }).Accepted);

Check("FCR0242 runtime-activation smuggling rejected",
    !WebProjectOwnerFeatureEntitlementGovernance.Evaluate(request with { RuntimeActivationRequested = true }).Accepted);

Check("FCR0242 deployment-authority smuggling rejected",
    !WebProjectOwnerFeatureEntitlementGovernance.Evaluate(request with { DeploymentRequested = true }).Accepted);

var futureCatalog = catalog with
{
    CatalogVersion = "2026.08.18.2",
    CatalogSha256 = "BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB",
    Features = features.Concat(new[]
    {
        new WebFsatsFeatureDefinition("feature.vip.future", "1.0.0", WebFsatsFeatureAudience.CustomerFacing, WebFsatsCommercialTier.Vip, true, true, false, false, false)
    }).ToArray()
};
var futureDecision = WebProjectOwnerFeatureEntitlementGovernance.Evaluate(request with { Catalog = futureCatalog });
Check("FCR0242 future VIP feature auto-included on current-catalog reevaluation",
    futureDecision.Accepted && futureDecision.GrantedFeatureIds.Contains("feature.vip.future", StringComparer.Ordinal));
Check("FCR0242 projection records exact catalog version", StringComparer.Ordinal.Equals(futureDecision.CatalogVersion, "2026.08.18.2"));
Check("FCR0242 projection records exact catalog digest", StringComparer.Ordinal.Equals(futureDecision.CatalogSha256, futureCatalog.CatalogSha256));

var failures = checks.Where(x => !x.Pass).ToArray();
if (failures.Length > 0)
{
    foreach (var failure in failures)
        Console.Error.WriteLine($"FAIL: {failure.Name}");

    Console.Error.WriteLine($"OWNER FEATURE ENTITLEMENT VERIFIER: FAIL ({checks.Count - failures.Length}/{checks.Count})");
    return 1;
}

Console.WriteLine($"OWNER FEATURE ENTITLEMENT VERIFIER: PASS ({checks.Count}/{checks.Count})");
return 0;
