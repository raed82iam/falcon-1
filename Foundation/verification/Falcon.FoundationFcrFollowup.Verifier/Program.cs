using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Foundation.ArtifactPublication;
using Foundation.Authority;
using Foundation.Contracts;

var checks = 0;
var passed = 0;

void Check(bool condition, string name)
{
    checks++;
    if (!condition)
    {
        Console.WriteLine($"FAIL: {name}");
        throw new InvalidOperationException(name);
    }
    passed++;
    Console.WriteLine($"PASS: {name}");
}

bool ThrowsInvalidOperation(Action action)
{
    try { action(); return false; }
    catch (InvalidOperationException) { return true; }
}

string Sha256(string payload) => "sha256/" + Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(payload)));

Console.WriteLine("FALCON_FOUNDATION_FCR_FOLLOWUP_VERIFIER");

// -----------------------------------------------------------------------------
// FCR-0010 / FCR-0031: exact Stage 6 canonical resource publication.
// -----------------------------------------------------------------------------
var catalog = CanonicalFoundationArtifacts.CreateCatalog();
var resource = CanonicalFoundationArtifacts.Stage6ResourceStateProjection;
var aggregate = CanonicalFoundationArtifacts.Stage6AggregateResourceStateProjection;

Check(catalog.Count == 3, "canonical catalog contains AI Kill plus two Stage 6 resource artifacts");
Check(Sha256(CanonicalFoundationArtifacts.Stage6ResourceStateCanonicalPayload) == resource.Sha256Digest,
    "resource-state canonical payload digest matches");
Check(Sha256(CanonicalFoundationArtifacts.Stage6AggregateResourceStateCanonicalPayload) == aggregate.Sha256Digest,
    "aggregate-resource canonical payload digest matches");

ArtifactConsumptionRequest Exact(FoundationArtifactDescriptor d, string consumer = "application:test") =>
    new(consumer, d.ArtifactId, d.ArtifactVersion, d.Sha256Digest, d.EvidenceReference, d.CompatibilityIdentity);

ArtifactConsumptionRequest Mutated(FoundationArtifactDescriptor d, string dimension) => dimension switch
{
    "id" => new("application:test", d.ArtifactId + ":wrong", d.ArtifactVersion, d.Sha256Digest, d.EvidenceReference, d.CompatibilityIdentity),
    "version" => new("application:test", d.ArtifactId, "9.9.9", d.Sha256Digest, d.EvidenceReference, d.CompatibilityIdentity),
    "digest" => new("application:test", d.ArtifactId, d.ArtifactVersion, "sha256/" + new string('A', 64), d.EvidenceReference, d.CompatibilityIdentity),
    "evidence" => new("application:test", d.ArtifactId, d.ArtifactVersion, d.Sha256Digest, d.EvidenceReference + ":wrong", d.CompatibilityIdentity),
    "compatibility" => new("application:test", d.ArtifactId, d.ArtifactVersion, d.Sha256Digest, d.EvidenceReference, d.CompatibilityIdentity + ":wrong"),
    _ => throw new ArgumentOutOfRangeException(nameof(dimension))
};

foreach (var descriptor in new[] { resource, aggregate })
{
    var exact = catalog.Evaluate(Exact(descriptor));
    Check(exact.AcceptedForTechnicalConsumption, $"{descriptor.ArtifactId} exact consumption accepted");
    Check(!exact.ActivationAuthorized && !exact.DeploymentAuthorized && !exact.BusinessAuthorityGranted,
        $"{descriptor.ArtifactId} consumption grants no runtime/business authority");
    foreach (var dimension in new[] { "id", "version", "digest", "evidence", "compatibility" })
        Check(!catalog.Evaluate(Mutated(descriptor, dimension)).AcceptedForTechnicalConsumption,
            $"{descriptor.ArtifactId} rejects wrong {dimension}");
}

// -----------------------------------------------------------------------------
// FCR-0237: only governed Web/Owner paths may manage/evaluate standing policy.
// -----------------------------------------------------------------------------
Check(!typeof(StandingOwnerPreapprovalEvaluator).GetMethods(BindingFlags.Public | BindingFlags.Instance).Any(m => m.Name == "Evaluate"),
    "raw preapproval evaluator is not public");
Check(!typeof(RegisteredStandingOwnerPreapprovalEvaluator).GetMethods(BindingFlags.Public | BindingFlags.Instance).Any(m => m.Name == "Evaluate"),
    "registered core evaluator is not public");
Check(!typeof(StandingOwnerPreapprovalRegistry).GetMethods(BindingFlags.Public | BindingFlags.Instance)
        .Any(m => m.Name is "RegisterOrReplace" or "Revoke"),
    "registry mutation is Foundation-internal");
Check(typeof(StandingOwnerPolicyManagementService).GetMethods(BindingFlags.Public | BindingFlags.Instance).Any(m => m.Name == "Apply"),
    "governed policy management service is public");
Check(typeof(WebOwnerStandingPreapprovalEvaluator).GetMethods(BindingFlags.Public | BindingFlags.Instance).Any(m => m.Name == "Evaluate"),
    "Web Owner policy evaluation surface is public");

var effectiveFrom = new DateTimeOffset(2026, 8, 18, 0, 0, 0, TimeSpan.Zero);
var observation = effectiveFrom.AddHours(1);
var policyExpiry = effectiveFrom.AddDays(30);
const string webSurface = "shared-web:owner-command-center";
const string app = "application:trading";
const string producer = "application:trading:msa";
const string policyId = "owner-preapproval-policy-001";
const string evidenceV1 = "evidence:owner:standing-preapproval:policy-001:v1";
const string evidenceV2 = "evidence:owner:standing-preapproval:policy-001:v2";

AuthorityResult FreshAuthority(
    string requestId,
    string action,
    string resourceId,
    string purpose,
    string scope,
    string securityContext,
    DateTimeOffset now,
    bool allow = true)
{
    var authority = new DefaultDenyAuthorityEngine();
    var policy = new AuthorityPolicy(
        "test-authority-policy", "1.0.0", "authority:owner:test:v1", effectiveFrom, policyExpiry,
        new[] { webSurface }, new[] { action }, new[] { resourceId }, new[] { purpose },
        new[] { scope }, new[] { securityContext });
    var delegation = new DelegationEvidence(
        "delegation:owner:test:v1", webSurface, "authority:owner:test:v1", new[] { scope },
        effectiveFrom, policyExpiry, false);
    var fitness = new FitnessEvidence(webSurface, "FIT", allow, now.AddMinutes(-2), now.AddHours(1), "evidence:fitness:web-owner-surface");
    var request = new AuthorityRequest(
        requestId, webSurface, action, resourceId, purpose, scope, "OWNER_COMMAND_CENTER",
        securityContext, "FIT", "correlation:" + requestId, now.AddMinutes(-1), now.AddMinutes(30));
    return authority.Evaluate(request, new AuthorityEvaluationContext(policy, delegation, fitness, now, "evidence:authority-evaluation"));
}

StandingOwnerPreapprovalProfile Profile(string version, string evidence) => new(
    policyId, version, "owner:project", "authority:owner:standing-preapproval:" + version,
    "delegation:owner:standing-preapproval:" + version,
    new[] { webSurface }, new[] { app },
    new[] { "MODEL_TUNING", "DOCUMENTATION_REFRESH", "AI_KILL" },
    new[] { "application:model", "application:documentation", "foundation:ai-kill" },
    new[] { "BOUNDED_UPDATE", "MAINTENANCE" },
    new[] { "application:trading:update" },
    new[] { "SANDBOX", "PAPER" },
    new[] { "security:owner-preapproved-update" },
    2, effectiveFrom, policyExpiry, false, evidence);

StandingOwnerPolicyManagementRequest Manage(
    StandingOwnerPolicyMutationOperation operation,
    string version,
    StandingOwnerPreapprovalProfile? profile,
    DateTimeOffset now,
    bool ownerAuthenticated = true,
    bool mfa = true,
    string surface = webSurface,
    string? authorityScope = null,
    bool authorityAllow = true)
{
    var requestId = $"policy-mutation:{operation}:{version}";
    var expectedScope = "foundation:standing-owner-preapproval-policy:" + policyId;
    var fresh = FreshAuthority(
        requestId,
        "MANAGE_STANDING_OWNER_POLICY",
        "foundation:standing-owner-preapproval-policy",
        "OWNER_POLICY_MANAGEMENT",
        authorityScope ?? expectedScope,
        "security:owner-policy-step-up",
        now,
        authorityAllow);
    return new StandingOwnerPolicyManagementRequest(
        requestId, "owner:project", surface, operation, profile, policyId, version,
        ownerAuthenticated, mfa, "evidence:owner-authentication:phishing-resistant-mfa", fresh,
        $"evidence:policy-mutation:{operation}:{version}", now.AddMinutes(-1), now.AddMinutes(30));
}

var registry = new StandingOwnerPreapprovalRegistry();
var management = new StandingOwnerPolicyManagementService(registry);
var registerV1 = management.Apply(Manage(StandingOwnerPolicyMutationOperation.RegisterOrReplace, "1.0.0", Profile("1.0.0", evidenceV1), observation), observation);
Check(registerV1.Applied, "v1 standing Owner policy registered through governed management service");
Check(registry.Count == 1, "Foundation registry contains one policy");
Check(registry.GetRequired(policyId).IdentitySha256 == registerV1.RegistrationIdentitySha256,
    "management decision binds exact registry identity");
Check(!management.Apply(Manage(StandingOwnerPolicyMutationOperation.RegisterOrReplace, "1.0.0", Profile("1.0.0", evidenceV1), observation,
        ownerAuthenticated: false), observation).Applied,
    "policy management rejects unauthenticated Owner");
Check(!management.Apply(Manage(StandingOwnerPolicyMutationOperation.RegisterOrReplace, "1.0.0", Profile("1.0.0", evidenceV1), observation,
        mfa: false), observation).Applied,
    "policy management requires MFA");
Check(!management.Apply(Manage(StandingOwnerPolicyMutationOperation.RegisterOrReplace, "1.0.0", Profile("1.0.0", evidenceV1), observation,
        surface: "application:trading"), observation).Applied,
    "Application cannot manage Owner standing policy");
Check(!management.Apply(Manage(StandingOwnerPolicyMutationOperation.RegisterOrReplace, "1.0.0", Profile("1.0.0", evidenceV1), observation,
        authorityAllow: false), observation).Applied,
    "policy management requires fresh ALLOW authority");
Check(management.Apply(Manage(StandingOwnerPolicyMutationOperation.RegisterOrReplace, "1.0.0", Profile("1.0.0", evidenceV1), observation,
        authorityScope: "foundation:standing-owner-preapproval-policy:other"), observation).Reason == StandingOwnerPolicyManagementReason.AuthorityScopeMismatch,
    "policy management rejects wrong authority scope");
Check(management.Apply(Manage(StandingOwnerPolicyMutationOperation.RegisterOrReplace, "1.0.0", Profile("1.0.0", evidenceV1), observation), observation).Reason == StandingOwnerPolicyManagementReason.RegistryRejected,
    "same-version policy replacement fails closed");

GovernedBackupRollbackPlan Plan(
    bool superseded = false,
    string applicationIdentity = app,
    string scope = "application:trading:update",
    DateTimeOffset? expiry = null) => new(
        "rollback-plan:model-tuning-001", "1.0.0",
        "sha256/BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB",
        "sha256/CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC",
        applicationIdentity, scope,
        "evidence:rollback-plan:validated:v1",
        "TARGET_STOPPED_AND_PRECHANGE_SNAPSHOT_AVAILABLE",
        "NO_AUTHORITY_SECRET_OR_TRUST_RESTORATION",
        "RESTORE_PRECHANGE_TECHNICAL_STATE_ONLY",
        effectiveFrom, expiry ?? observation.AddHours(2), superseded,
        "evidence:rollback-plan:model-tuning-001:v1");

WebOwnerPreapprovalProposal Proposal(
    GovernedBackupRollbackPlan? plan,
    string policyEvidence = evidenceV1,
    string surface = webSurface,
    bool producerAutoAcceptClaim = false,
    bool producerRollbackClaim = false,
    string updateClass = "MODEL_TUNING",
    string resourceId = "application:model",
    string environment = "SANDBOX",
    int riskTier = 1) => new(
        "proposal:model-tuning-001",
        "candidate:model-tuning-001",
        "1.0.0",
        "sha256/0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF",
        producer, app, surface, updateClass, resourceId, "BOUNDED_UPDATE",
        "application:trading:update:model", environment,
        "security:owner-preapproved-update", riskTier, "FIT",
        "correlation:model-tuning-001", policyEvidence, plan,
        producerAutoAcceptClaim, producerRollbackClaim,
        observation.AddMinutes(-10), observation.AddHours(1));

var surfaceFitness = new FitnessEvidence(webSurface, "FIT", true, observation.AddMinutes(-5), observation.AddHours(2), "evidence:fitness:web-owner-surface");
var webEvaluator = new WebOwnerStandingPreapprovalEvaluator(registry);
var validPlan = Plan();
var accepted = webEvaluator.Evaluate(policyId, Proposal(validPlan), surfaceFitness, observation);
Check(accepted.AcceptedUnderStandingOwnerPolicy, "exact Web Owner-derived auto-accept decision accepted");
Check(accepted.BackupRollbackPlanIdentitySha256 == validPlan.IdentitySha256,
    "auto-accept binds exact backup/rollback plan identity");
Check(!accepted.ExecutionAuthorized && !accepted.DeploymentAuthorized && !accepted.BusinessAuthorityGranted,
    "auto-accept grants no execution/deployment/business authority");
Check(webEvaluator.Evaluate(policyId, Proposal(validPlan), surfaceFitness, observation).DecisionIdentitySha256 == accepted.DecisionIdentitySha256,
    "auto-accept deterministic rerun");
Check(webEvaluator.Evaluate(policyId, Proposal(null), surfaceFitness, observation).Reason == WebOwnerPreapprovalReason.BackupPlanRequired,
    "auto-accept without backup/rollback plan rejected");
Check(webEvaluator.Evaluate(policyId, Proposal(Plan(superseded: true)), surfaceFitness, observation).Reason == WebOwnerPreapprovalReason.BackupPlanInvalid,
    "superseded rollback plan rejected");
Check(webEvaluator.Evaluate(policyId, Proposal(Plan(applicationIdentity: "application:other")), surfaceFitness, observation).Reason == WebOwnerPreapprovalReason.BackupPlanScopeMismatch,
    "rollback plan application mismatch rejected");
Check(webEvaluator.Evaluate(policyId, Proposal(Plan(scope: "application:other:update")), surfaceFitness, observation).Reason == WebOwnerPreapprovalReason.BackupPlanScopeMismatch,
    "rollback plan scope mismatch rejected");
Check(webEvaluator.Evaluate(policyId, Proposal(Plan(expiry: observation.AddMinutes(30))), surfaceFitness, observation).Reason == WebOwnerPreapprovalReason.BackupPlanNotCurrent,
    "rollback plan expiring before proposal rejected");
Check(webEvaluator.Evaluate(policyId, Proposal(validPlan, surface: "application:trading"), surfaceFitness, observation).Reason == WebOwnerPreapprovalReason.WrongDecisionSurface,
    "Application cannot act as Owner-derived auto-accept surface");
Check(webEvaluator.Evaluate(policyId, Proposal(validPlan, producerAutoAcceptClaim: true), surfaceFitness, observation).Reason == WebOwnerPreapprovalReason.ProducerSelfApprovalForbidden,
    "producer self-declared auto-accept rejected");
Check(webEvaluator.Evaluate(policyId, Proposal(validPlan, producerRollbackClaim: true), surfaceFitness, observation).Reason == WebOwnerPreapprovalReason.ProducerSelfApprovalForbidden,
    "producer self-declared rollback authority rejected");
Check(webEvaluator.Evaluate(policyId, Proposal(validPlan, updateClass: "AI_KILL", resourceId: "foundation:ai-kill"), surfaceFitness, observation).Reason.Contains(StandingOwnerPreapprovalReason.ManualOnlyClass, StringComparison.Ordinal),
    "AI Kill remains manual-only");
Check(webEvaluator.Evaluate(policyId, Proposal(validPlan, environment: "LIVE"), surfaceFitness, observation).Reason.Contains(StandingOwnerPreapprovalReason.EnvironmentMismatch, StringComparison.Ordinal),
    "unapproved Live environment rejected");
Check(webEvaluator.Evaluate(policyId, Proposal(validPlan, riskTier: 3), surfaceFitness, observation).Reason.Contains(StandingOwnerPreapprovalReason.RiskExceeded, StringComparison.Ordinal),
    "risk above standing policy ceiling rejected");

foreach (var manualClass in new[]
{
    "AI_KILL", "RELEASE", "CONTROLLED_REVIVAL", "LIVE_TRADING_ACTIVATION",
    "CREDENTIAL_OR_SECURITY_CHANGE", "AUTHORITY_EXPANSION", "DEPLOYMENT", "CONSTITUTION_OR_GOVERNANCE_CHANGE"
})
    Check(StandingOwnerPreapprovalEvaluator.IsManualOnlyClass(manualClass), $"manual-only fence includes {manualClass}");

var registerV2 = management.Apply(Manage(StandingOwnerPolicyMutationOperation.RegisterOrReplace, "2.0.0", Profile("2.0.0", evidenceV2), observation.AddMinutes(1)), observation.AddMinutes(1));
Check(registerV2.Applied && registerV2.RegistrationIdentitySha256 != registerV1.RegistrationIdentitySha256,
    "strictly newer policy version replaces current registration");
Check(!webEvaluator.Evaluate(policyId, Proposal(validPlan, policyEvidence: evidenceV1), surfaceFitness, observation.AddMinutes(2)).AcceptedUnderStandingOwnerPolicy,
    "superseded policy evidence rejected");
Check(webEvaluator.Evaluate(policyId, Proposal(validPlan, policyEvidence: evidenceV2), surfaceFitness, observation.AddMinutes(2)).AcceptedUnderStandingOwnerPolicy,
    "current policy evidence accepted");
var revokeV2 = management.Apply(Manage(StandingOwnerPolicyMutationOperation.Revoke, "2.0.0", null, observation.AddMinutes(3)), observation.AddMinutes(3));
Check(revokeV2.Applied && revokeV2.Revoked, "Owner revocation applied through governed management service");
Check(!webEvaluator.Evaluate(policyId, Proposal(validPlan, policyEvidence: evidenceV2), surfaceFitness, observation.AddMinutes(4)).AcceptedUnderStandingOwnerPolicy,
    "revoked policy cannot auto-accept future proposal");

// -----------------------------------------------------------------------------
// FCR-0237: fresh Owner rollback-order authorization is separate from execution.
// -----------------------------------------------------------------------------
AuthorityResult RollbackAuthority(string requestId, DateTimeOffset now, bool allow = true) => FreshAuthority(
    requestId,
    "OWNER_ROLLBACK_ORDER",
    "application:model",
    "ROLLBACK",
    "application:trading:update:model",
    "security:owner-rollback-step-up",
    now,
    allow);

OwnerRollbackOrderRequest RollbackRequest(
    DateTimeOffset now,
    string surface = webSurface,
    bool stepUp = true,
    bool mfa = true,
    bool admission = true,
    bool safety = true,
    string ownerAuthEvidence = "evidence:owner-authentication:rollback-step-up",
    string admissionEvidence = "evidence:target-admission:current",
    string safetyEvidence = "evidence:safety-readiness:current",
    AuthorityResult? authority = null,
    GovernedBackupRollbackPlan? plan = null) => new(
        "rollback-order:001",
        "owner:project",
        surface,
        accepted.DecisionIdentitySha256,
        accepted.ProposalIdentitySha256,
        plan ?? validPlan,
        app,
        "application:trading:update:model",
        stepUp,
        mfa,
        ownerAuthEvidence,
        admission,
        admissionEvidence,
        safety,
        safetyEvidence,
        authority ?? RollbackAuthority("rollback-order:001", now),
        "correlation:rollback-order:001",
        "evidence:owner-rollback-order:001",
        now,
        now.AddHours(1));

var rollbackEvaluator = new OwnerRollbackOrderEvaluator();
var rollbackTime = observation.AddMinutes(5);
var rollbackAccepted = rollbackEvaluator.Evaluate(RollbackRequest(rollbackTime), rollbackTime);
Check(rollbackAccepted.State == OwnerRollbackOrderState.Accepted && rollbackAccepted.RollbackAuthorized,
    "fresh Owner rollback order accepted");
Check(!rollbackAccepted.RollbackExecuted,
    "rollback authorization is not rollback execution");
Check(!rollbackAccepted.AuthorityRestored && !rollbackAccepted.TrustRestored,
    "rollback authorization restores no authority or trust");
Check(rollbackEvaluator.Evaluate(RollbackRequest(rollbackTime, surface: "application:trading"), rollbackTime).State == OwnerRollbackOrderState.Rejected,
    "rollback order must originate from Web Owner surface");
Check(rollbackEvaluator.Evaluate(RollbackRequest(rollbackTime, stepUp: false), rollbackTime).State == OwnerRollbackOrderState.Rejected,
    "rollback order requires step-up authentication");
Check(rollbackEvaluator.Evaluate(RollbackRequest(rollbackTime, mfa: false), rollbackTime).State == OwnerRollbackOrderState.Rejected,
    "rollback order requires MFA");
Check(rollbackEvaluator.Evaluate(RollbackRequest(rollbackTime, admission: false), rollbackTime).State == OwnerRollbackOrderState.Rejected,
    "rollback order requires current target admission");
Check(rollbackEvaluator.Evaluate(RollbackRequest(rollbackTime, safety: false), rollbackTime).State == OwnerRollbackOrderState.Rejected,
    "rollback order requires current safety readiness");
Check(rollbackEvaluator.Evaluate(RollbackRequest(rollbackTime, ownerAuthEvidence: ""), rollbackTime).State == OwnerRollbackOrderState.Rejected,
    "rollback order requires Owner authentication evidence");
Check(rollbackEvaluator.Evaluate(RollbackRequest(rollbackTime, admissionEvidence: ""), rollbackTime).State == OwnerRollbackOrderState.Rejected,
    "rollback order requires target admission evidence");
Check(rollbackEvaluator.Evaluate(RollbackRequest(rollbackTime, safetyEvidence: ""), rollbackTime).State == OwnerRollbackOrderState.Rejected,
    "rollback order requires safety readiness evidence");
Check(rollbackEvaluator.Evaluate(RollbackRequest(rollbackTime, authority: RollbackAuthority("rollback-order:001", rollbackTime, allow: false)), rollbackTime).State == OwnerRollbackOrderState.Rejected,
    "rollback order rejects denied fresh authority");
Check(rollbackEvaluator.Evaluate(RollbackRequest(rollbackTime, authority: RollbackAuthority("rollback-order:OTHER", rollbackTime)), rollbackTime).State == OwnerRollbackOrderState.Rejected,
    "rollback order rejects authority decision for another request");
Check(rollbackEvaluator.Evaluate(RollbackRequest(rollbackTime, plan: Plan(superseded: true)), rollbackTime).State == OwnerRollbackOrderState.Rejected,
    "rollback order rejects superseded rollback plan");

var completed = new RollbackStatusProjection(
    rollbackAccepted.DecisionIdentitySha256,
    RollbackExecutionState.Completed,
    "application:trading:rollback-executor",
    "evidence:rollback-completed:001",
    rollbackTime.AddMinutes(5),
    false, false, false, false, false);
completed.Validate();
Check(true, "rollback completion projection valid without authority restoration");
Check(ThrowsInvalidOperation((completed with { AuthorityRestored = true }).Validate), "rollback cannot silently restore authority");
Check(ThrowsInvalidOperation((completed with { TrustRestored = true }).Validate), "rollback cannot silently restore trust");
Check(ThrowsInvalidOperation((completed with { CredentialsRestored = true }).Validate), "rollback cannot silently restore credentials");
Check(ThrowsInvalidOperation((completed with { LiveTradingAuthorityRestored = true }).Validate), "rollback cannot silently restore Live authority");
Check(ThrowsInvalidOperation((completed with { KillReleaseRevivalAuthorityRestored = true }).Validate), "rollback cannot silently restore Kill/release/revival authority");

Console.WriteLine($"CHECKS = {passed}/{checks}");
Console.WriteLine("FCR0010_RESOURCE_DESCRIPTOR = PUBLISHED");
Console.WriteLine("FCR0031_AGGREGATE_RESOURCE_DESCRIPTOR = PUBLISHED");
Console.WriteLine("FCR0237_WEB_OWNER_POLICY_MANAGEMENT = IMPLEMENTED_FAIL_CLOSED");
Console.WriteLine("FCR0237_WEB_OWNER_STANDING_PREAPPROVAL = IMPLEMENTED_FAIL_CLOSED");
Console.WriteLine("FCR0237_BACKUP_ROLLBACK_BINDING = IMPLEMENTED_FAIL_CLOSED");
Console.WriteLine("FCR0237_OWNER_ROLLBACK_ORDER_BOUNDARY = IMPLEMENTED_FAIL_CLOSED");
Console.WriteLine("APPLICATION_SELF_DECLARED_AUTO_ACCEPT = FORBIDDEN");
Console.WriteLine("AI_SELF_DECLARED_AUTO_ACCEPT = FORBIDDEN");
Console.WriteLine("WEB_ACCEPTED_LIST != FOUNDATION_AUTHORITY");
Console.WriteLine("AUTO_ACCEPT != EXECUTION_AUTHORITY");
Console.WriteLine("ROLLBACK_REQUEST != ROLLBACK_AUTHORIZATION != ROLLBACK_EXECUTION");
Console.WriteLine("ROLLBACK_EXECUTION != AUTHORITY_RESTORATION");
Console.WriteLine("FOUNDATION_FCR_FOLLOWUP_VERIFIER = PASS");
