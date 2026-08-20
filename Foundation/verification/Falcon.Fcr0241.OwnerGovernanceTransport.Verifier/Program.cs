using System.Security.Cryptography;
using System.Text;
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

Console.WriteLine("FALCON_FCR0241_OWNER_GOVERNANCE_TRANSPORT_VERIFIER");

var profiles = OwnerGovernanceRequestResponseProfiles.All;
Check(profiles.Count == 3, "exactly three separated Owner-governance transport families published");
Check(profiles.Select(x => x.RequestRouteIdentity).Distinct(StringComparer.Ordinal).Count() == 3,
    "request route identities are unique");
Check(profiles.Select(x => x.ResponseRouteIdentity).Distinct(StringComparer.Ordinal).Count() == 3,
    "response route identities are unique");
Check(profiles.All(x => x.Classification == FilMessageClassification.Governance),
    "all Owner-governance transport profiles are Governance classified");
Check(profiles.All(x => x.ContractState == PublicRequestResponseContractState.Published),
    "all Owner-governance transport profiles are explicitly published");
Check(profiles.All(x => x.RequestProducer.Value == "shared-web" && x.RequestRecipientScope.Value == "foundation.owner-governance"),
    "all requests originate from Shared Web and target Foundation owner-governance boundary");
Check(profiles.All(x => x.ResponseProducer.Value == "foundation.runtime" && x.ResponseRecipientScope.Value == "shared-web"),
    "all responses originate from Foundation runtime and return to Shared Web");
Check(profiles.All(x => x.RetryRequiresSameIdempotencyIdentity && x.MaxDeliveryAttempts == 3),
    "retry policy requires stable idempotency identity and bounded delivery attempts");
Check(profiles.All(x => x.ProfileIdentitySha256.StartsWith("sha256/", StringComparison.Ordinal) && x.ProfileIdentitySha256.Length == 71),
    "every profile exposes exact deterministic SHA-256 identity");

Check(OwnerGovernanceRequestResponseProfiles.StandingOwnerPolicyManagement.RequestMessageKind == FilMessageKind.Command,
    "standing policy management is a Command family");
Check(OwnerGovernanceRequestResponseProfiles.StandingOwnerPreapprovalEvaluation.RequestMessageKind == FilMessageKind.Query,
    "standing preapproval evaluation is a Query family");
Check(OwnerGovernanceRequestResponseProfiles.OwnerRollbackOrder.RequestMessageKind == FilMessageKind.Command,
    "Owner rollback order is a Command family");

var created = new DateTimeOffset(2026, 8, 18, 0, 0, 0, TimeSpan.Zero);
var observation = created.AddSeconds(5);

PublicRuntimeRequestResponseTransportDecision Request(
    PublicRuntimeRequestResponseProfile profile,
    string suffix = "1",
    DateTimeOffset? observedAt = null) =>
    PublicRuntimeRequestResponseTransport.BuildRequest(
        profile,
        "{\"request\":\"payload-" + suffix + "\"}",
        new MessageIdentity("message:request:" + suffix),
        new CorrelationIdentity("correlation:owner-governance:" + suffix),
        null,
        new IdempotencyIdentity("idempotency:owner-governance:" + suffix),
        new DeliveryAttemptIdentity("delivery:owner-governance:" + suffix + ":1"),
        new RetryLineageIdentity("retry:owner-governance:" + suffix),
        created,
        created.AddSeconds(60),
        observedAt ?? observation);

PublicRuntimeRequestResponseTransportDecision Response(
    PublicRuntimeRequestResponseProfile profile,
    PublicRuntimeRequestResponseTransportDecision request,
    string suffix,
    DateTimeOffset? observedAt = null) =>
    PublicRuntimeRequestResponseTransport.BuildResponse(
        profile,
        request,
        "{\"decision\":\"result\"}",
        new MessageIdentity("message:response:" + suffix),
        new IdempotencyIdentity("idempotency:response:" + suffix),
        new DeliveryAttemptIdentity("delivery:response:" + suffix + ":1"),
        new RetryLineageIdentity("retry:response:" + suffix),
        CanonicalOutcome.Succeeded("governed_response_available"),
        created.AddSeconds(1),
        created.AddSeconds(61),
        observedAt ?? observation);

var deterministicMaterials = new List<string>();
for (var i = 0; i < profiles.Count; i++)
{
    var profile = profiles[i];
    var suffix = (i + 1).ToString(System.Globalization.CultureInfo.InvariantCulture);
    var request = Request(profile, suffix);
    Check(request.Accepted, $"{profile.FamilyIdentity} request envelope accepted");
    Check(request.RouteAvailable, $"{profile.FamilyIdentity} reports route contract available");
    Check(!request.RouteActivated && !request.RouteAuthorized && !request.ConnectionExecuted,
        $"{profile.FamilyIdentity} contract availability grants no route activation/authorization/connection execution");
    Check(!request.ExecutionAuthorized && !request.BusinessAuthorityGranted,
        $"{profile.FamilyIdentity} transport grants no execution or business authority");

    var response = Response(profile, request, suffix);
    Check(response.Accepted, $"{profile.FamilyIdentity} response envelope accepted");
    Check(response.Envelope!.MessageKind == FilMessageKind.Response,
        $"{profile.FamilyIdentity} response kind fixed to Response");
    Check(response.Envelope.CorrelationId.Value == request.Envelope!.CorrelationId.Value,
        $"{profile.FamilyIdentity} response preserves exact request correlation");
    Check(response.Envelope.CausationId!.Value == request.Envelope.MessageId.Value,
        $"{profile.FamilyIdentity} response causation binds exact request message");
    Check(!response.RouteActivated && !response.RouteAuthorized && !response.ConnectionExecuted &&
          !response.ExecutionAuthorized && !response.BusinessAuthorityGranted,
        $"{profile.FamilyIdentity} response transport grants no runtime/action authority");

    var repeatRequest = Request(profile, suffix);
    var repeatResponse = Response(profile, repeatRequest, suffix);
    var requestDigest = CanonicalMessagingDigest.ComputeEnvelopeSha256(request.Envelope!);
    var repeatRequestDigest = CanonicalMessagingDigest.ComputeEnvelopeSha256(repeatRequest.Envelope!);
    var responseDigest = CanonicalMessagingDigest.ComputeEnvelopeSha256(response.Envelope!);
    var repeatResponseDigest = CanonicalMessagingDigest.ComputeEnvelopeSha256(repeatResponse.Envelope!);
    Check(StringComparer.Ordinal.Equals(request.ProfileIdentitySha256, repeatRequest.ProfileIdentitySha256),
        $"{profile.FamilyIdentity} profile identity deterministic");
    Check(StringComparer.Ordinal.Equals(requestDigest, repeatRequestDigest),
        $"{profile.FamilyIdentity} request envelope identity deterministic");
    Check(StringComparer.Ordinal.Equals(responseDigest, repeatResponseDigest),
        $"{profile.FamilyIdentity} response envelope identity deterministic");
    deterministicMaterials.Add(profile.ProfileIdentitySha256);
    deterministicMaterials.Add(requestDigest);
    deterministicMaterials.Add(responseDigest);
}

var policy = OwnerGovernanceRequestResponseProfiles.StandingOwnerPolicyManagement;
var acceptedPolicyRequest = Request(policy, "policy");

var inventedLocalProfile = policy with
{
    FamilyIdentity = "foundation:owner-governance:invented-local-profile:v1",
    ContractIdentity = "foundation/contracts/invented-local-owner-governance-request-response",
    CompatibilityIdentity = "compat:foundation-invented-local-owner-governance:v1",
    RequestRouteIdentity = "route:foundation:invented-local-owner-governance:web:v1",
    ResponseRouteIdentity = "route:foundation:invented-local-owner-governance-result:web:v1",
    AdmissionIdentity = "admission:foundation:invented-local-owner-governance:web:v1",
    EvidenceReference = "evidence:fcr-0241:invented-local-profile:v1"
};

var inventedLocalProfileDecision = PublicRuntimeRequestResponseTransport.BuildRequest(
    inventedLocalProfile,
    "{}",
    new MessageIdentity("message:invented-local-profile"),
    new CorrelationIdentity("correlation:invented-local-profile"),
    null,
    new IdempotencyIdentity("idempotency:invented-local-profile"),
    new DeliveryAttemptIdentity("delivery:invented-local-profile:1"),
    new RetryLineageIdentity("retry:invented-local-profile"),
    created,
    created.AddSeconds(30),
    observation);

Check(!inventedLocalProfileDecision.Accepted &&
      inventedLocalProfileDecision.Reason == "PUBLIC_REQUEST_RESPONSE_PROFILE_NOT_FOUND_IN_CANONICAL_REGISTRY",
    "structurally valid caller-created profile fails closed unless present in canonical registry");

var malformedProfileDecision = PublicRuntimeRequestResponseTransport.BuildRequest(
    policy with { RequestSchemaId = null! },
    "{}",
    new MessageIdentity("message:malformed-profile"),
    new CorrelationIdentity("correlation:malformed-profile"),
    null,
    new IdempotencyIdentity("idempotency:malformed-profile"),
    new DeliveryAttemptIdentity("delivery:malformed-profile:1"),
    new RetryLineageIdentity("retry:malformed-profile"),
    created,
    created.AddSeconds(30),
    observation);
Check(!malformedProfileDecision.Accepted &&
      malformedProfileDecision.Reason == "PUBLIC_REQUEST_RESPONSE_PROFILE_REFERENCE_REQUIRED" &&
      malformedProfileDecision.ProfileIdentitySha256 == "INVALID_PROFILE",
    "malformed profile reference fails closed without exception");

Check(!PublicRuntimeRequestResponseTransport.BuildRequest(
        policy with { ContractState = PublicRequestResponseContractState.Revoked },
        "{}", new MessageIdentity("message:revoked"), new CorrelationIdentity("correlation:revoked"), null,
        new IdempotencyIdentity("idempotency:revoked"), new DeliveryAttemptIdentity("delivery:revoked:1"),
        new RetryLineageIdentity("retry:revoked"), created, created.AddSeconds(30), observation).Accepted,
    "revoked request-response contract fails closed");

Check(!PublicRuntimeRequestResponseTransport.BuildRequest(
        policy with { ContractState = PublicRequestResponseContractState.Superseded },
        "{}", new MessageIdentity("message:superseded"), new CorrelationIdentity("correlation:superseded"), null,
        new IdempotencyIdentity("idempotency:superseded"), new DeliveryAttemptIdentity("delivery:superseded:1"),
        new RetryLineageIdentity("retry:superseded"), created, created.AddSeconds(30), observation).Accepted,
    "superseded request-response contract fails closed with no silent upgrade");

Check(!PublicRuntimeRequestResponseTransport.BuildRequest(
        policy with { RequestMessageKind = FilMessageKind.Response },
        "{}", new MessageIdentity("message:wrong-kind"), new CorrelationIdentity("correlation:wrong-kind"), null,
        new IdempotencyIdentity("idempotency:wrong-kind"), new DeliveryAttemptIdentity("delivery:wrong-kind:1"),
        new RetryLineageIdentity("retry:wrong-kind"), created, created.AddSeconds(30), observation).Accepted,
    "request family rejects non Command/Query request kind");

Check(!PublicRuntimeRequestResponseTransport.BuildRequest(
        policy with { RetryRequiresSameIdempotencyIdentity = false },
        "{}", new MessageIdentity("message:no-idempotency"), new CorrelationIdentity("correlation:no-idempotency"), null,
        new IdempotencyIdentity("idempotency:no-idempotency"), new DeliveryAttemptIdentity("delivery:no-idempotency:1"),
        new RetryLineageIdentity("retry:no-idempotency"), created, created.AddSeconds(30), observation).Accepted,
    "retry without stable idempotency identity fails closed");

Check(!PublicRuntimeRequestResponseTransport.BuildRequest(
        policy,
        "{}", new MessageIdentity("message:ttl-too-wide"), new CorrelationIdentity("correlation:ttl-too-wide"), null,
        new IdempotencyIdentity("idempotency:ttl-too-wide"), new DeliveryAttemptIdentity("delivery:ttl-too-wide:1"),
        new RetryLineageIdentity("retry:ttl-too-wide"), created, created.AddMinutes(10), observation).Accepted,
    "request outside profile TTL ceiling fails closed");

var staleByNow = PublicRuntimeRequestResponseTransport.BuildRequest(
    policy,
    "{}", new MessageIdentity("message:stale-by-now"), new CorrelationIdentity("correlation:stale-by-now"), null,
    new IdempotencyIdentity("idempotency:stale-by-now"), new DeliveryAttemptIdentity("delivery:stale-by-now:1"),
    new RetryLineageIdentity("retry:stale-by-now"), created, created.AddSeconds(30), created.AddSeconds(31));
Check(!staleByNow.Accepted && staleByNow.Reason == "PUBLIC_REQUEST_FRESHNESS_INVALID",
    "request expired relative to observation time fails closed");

var requestExpiredBeforeResponse = PublicRuntimeRequestResponseTransport.BuildResponse(
    policy,
    acceptedPolicyRequest,
    "{}",
    new MessageIdentity("message:response-after-request-expiry"),
    new IdempotencyIdentity("idempotency:response-after-request-expiry"),
    new DeliveryAttemptIdentity("delivery:response-after-request-expiry:1"),
    new RetryLineageIdentity("retry:response-after-request-expiry"),
    CanonicalOutcome.Succeeded("governed_response_available"),
    created.AddSeconds(60),
    created.AddSeconds(90),
    created.AddSeconds(61));
Check(!requestExpiredBeforeResponse.Accepted &&
      requestExpiredBeforeResponse.Reason == "PUBLIC_RESPONSE_REQUEST_NO_LONGER_CURRENT",
    "response cannot be created from an expired accepted request");

Check(!PublicRuntimeRequestResponseTransport.BuildResponse(
        OwnerGovernanceRequestResponseProfiles.OwnerRollbackOrder,
        acceptedPolicyRequest,
        "{}", new MessageIdentity("message:cross-family-response"),
        new IdempotencyIdentity("idempotency:cross-family-response"),
        new DeliveryAttemptIdentity("delivery:cross-family-response:1"),
        new RetryLineageIdentity("retry:cross-family-response"),
        CanonicalOutcome.Succeeded("governed_response_available"),
        created.AddSeconds(1), created.AddSeconds(31), observation).Accepted,
    "cross-family response binding fails closed");

AuthorityResult AllowAuthority(string requestId, string scope, DateTimeOffset decisionTime, DateTimeOffset expiry) =>
    new(
        requestId,
        "authority-decision/sha256/AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA",
        AuthorityDecision.Allow,
        scope,
        "policy:owner-governance",
        "1.0.0",
        "owner-governance-test",
        "BOUNDED",
        "AUTHORITY_ALLOWED",
        decisionTime,
        expiry,
        "evidence:authority:owner-governance:test");

var registry = new StandingOwnerPreapprovalRegistry();
var stalePolicyRequest = new StandingOwnerPolicyManagementRequest(
    "mutation:stale",
    "owner:project",
    WebOwnerStandingPreapprovalBoundary.CanonicalDecisionSurfaceIdentity,
    StandingOwnerPolicyMutationOperation.Revoke,
    null,
    "policy:test",
    "1.0.0",
    true,
    true,
    "evidence:security:owner",
    AllowAuthority("mutation:stale", "foundation:standing-owner-preapproval-policy:policy:test", created, created.AddMinutes(5)),
    "evidence:mutation:stale",
    created,
    created.AddSeconds(30));
var stalePolicyDecision = new StandingOwnerPolicyManagementService(registry).Apply(stalePolicyRequest, created.AddSeconds(31));
Check(!stalePolicyDecision.Applied && stalePolicyDecision.Reason == StandingOwnerPolicyManagementReason.RequestNotCurrent,
    "expired Owner standing-policy mutation request fails closed");

var rollbackPlan = new GovernedBackupRollbackPlan(
    "rollback-plan:test",
    "1.0.0",
    "sha256/AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA",
    "sha256/BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB",
    "application:test",
    "scope:test",
    "evidence:rollback-plan:validation",
    "preconditions:test",
    "constraints:test",
    "outcome:test",
    created,
    created.AddMinutes(5),
    false,
    "evidence:rollback-plan:test");

var staleRollbackRequest = new OwnerRollbackOrderRequest(
    "rollback-order:stale",
    "owner:project",
    WebOwnerStandingPreapprovalBoundary.CanonicalDecisionSurfaceIdentity,
    "sha256/CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC",
    "sha256/DDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDDD",
    rollbackPlan,
    "application:test",
    "scope:test",
    true,
    true,
    "evidence:owner-auth:test",
    true,
    "evidence:admission:test",
    true,
    "evidence:safety:test",
    AllowAuthority("rollback-order:stale", "scope:test", created, created.AddMinutes(5)),
    "correlation:rollback:test",
    "evidence:rollback:test",
    created,
    created.AddSeconds(30));
var staleRollbackDecision = new OwnerRollbackOrderEvaluator().Evaluate(staleRollbackRequest, created.AddSeconds(31));
Check(staleRollbackDecision.State == OwnerRollbackOrderState.Rejected &&
      staleRollbackDecision.Reason == "ROLLBACK_ORDER_REQUEST_NOT_CURRENT",
    "expired Owner rollback order fails closed");

var staleProposal = new WebOwnerPreapprovalProposal(
    "proposal:stale",
    "candidate:test",
    "1.0.0",
    "sha256/EEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE",
    "application:test",
    "application:test",
    WebOwnerStandingPreapprovalBoundary.CanonicalDecisionSurfaceIdentity,
    "LOW_IMPACT_CONFIGURATION",
    "resource:test",
    "purpose:test",
    "scope:test",
    "NON_LIVE",
    "security:test",
    1,
    "FIT",
    "correlation:proposal:test",
    "evidence:policy:test",
    rollbackPlan,
    false,
    false,
    created,
    created.AddSeconds(30));
var staleProposalDecision = new WebOwnerStandingPreapprovalEvaluator(registry).Evaluate(
    "policy:test", staleProposal, null, created.AddSeconds(31));
Check(!staleProposalDecision.AcceptedUnderStandingOwnerPolicy &&
      staleProposalDecision.Reason == WebOwnerPreapprovalReason.ProposalNotCurrent,
    "expired Owner preapproval proposal fails closed before registry evaluation");

var projectionControlAttempt = PublicRuntimeProjectionTransport.Build(
    new PublicRuntimeProjectionRoute(
        "route:projection:control-forbidden:v1",
        "Foundation.Authority.OwnerRollbackOrderRequest",
        new SchemaIdentity("foundation.authority.owner-rollback-order.request"),
        "1.0.0",
        new ProducerIdentityReference("shared-web"),
        new RecipientScopeReference("foundation.owner-governance"),
        FilMessageKind.Command,
        FilMessageClassification.Governance,
        new AuthorityReference("authority:transport:projection-only"),
        new ProvenanceReference("evidence:fcr-0241:projection-control-forbidden"),
        "foundation/runtime-projection/control-forbidden",
        "1.0.0",
        "sha256/AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA",
        "evidence:fcr-0241:projection-control-forbidden",
        "compat:projection-control-forbidden:v1",
        PublicProjectionArtifactState.Published),
    "{}",
    new MessageIdentity("message:projection-control-forbidden"),
    new CorrelationIdentity("correlation:projection-control-forbidden"),
    null,
    new IdempotencyIdentity("idempotency:projection-control-forbidden"),
    new DeliveryAttemptIdentity("delivery:projection-control-forbidden:1"),
    new RetryLineageIdentity("retry:projection-control-forbidden"),
    created,
    created.AddSeconds(30));

Check(!projectionControlAttempt.Accepted && projectionControlAttempt.Reason == "PUBLIC_RUNTIME_PROJECTION_CONTROL_MESSAGE_FORBIDDEN",
    "projection-only transport remains forbidden for Owner control requests");

var deterministicMaterial = string.Join("\n", deterministicMaterials);
var deterministicDigest = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(deterministicMaterial)));
Console.WriteLine($"DETERMINISTIC_IDENTITY_SHA256 = {deterministicDigest}");
Console.WriteLine($"CHECKS = {passed}/{checks}");
Console.WriteLine("FCR0241_REQUEST_RESPONSE_FAMILIES = 3_SEPARATE_FAIL_CLOSED_PROFILES");
Console.WriteLine("REQUEST_FRESHNESS = OBSERVATION_TIME_BOUND");
Console.WriteLine("RESPONSE_REQUIRES_CURRENT_ACCEPTED_REQUEST = TRUE");
Console.WriteLine("MALFORMED_PROFILE = FAIL_CLOSED_DECISION");
Console.WriteLine("WEB_OWNER_COMMAND_CENTER = ONLY_REQUEST_PRODUCER_SURFACE");
Console.WriteLine("AUTO_ACCEPT != EXECUTION_AUTHORITY");
Console.WriteLine("ROLLBACK_REQUEST != ROLLBACK_AUTHORIZATION != ROLLBACK_EXECUTION");
Console.WriteLine("FIL_ROUTE_AVAILABLE != ROUTE_ACTIVATED");
Console.WriteLine("ROUTE_AVAILABLE != ROUTE_AUTHORIZED != CONNECTION_EXECUTED");
Console.WriteLine("PUBLIC_PROJECTION_TRANSPORT != CONTROL_REQUEST_TRANSPORT");
Console.WriteLine("FCR0241_OWNER_GOVERNANCE_TRANSPORT_VERIFIER = PASS");
