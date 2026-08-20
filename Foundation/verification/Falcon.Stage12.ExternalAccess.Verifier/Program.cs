using System.Reflection;
using Foundation.Authority;
using Foundation.Contracts;

var now = new DateTimeOffset(2026, 8, 16, 10, 30, 0, TimeSpan.Zero);
var evaluator = new ExternalAccessEvaluator();
var passed = 0;
var total = 0;

string Endpoint(string scheme, string host, string path = "") =>
    string.Concat(scheme, ":", "/", "/", host, path);

void Check(string name, bool condition)
{
    total++;
    if (!condition)
    {
        Console.Error.WriteLine($"FAIL: {name}");
        Environment.ExitCode = 1;
        throw new InvalidOperationException(name);
    }

    passed++;
    Console.WriteLine($"PASS: {name}");
}

AuthorityResult Authority(
    string requestId = "auth-req-1",
    string decisionId = "auth-dec-1",
    string scope = "external.route/use") =>
    new(
        requestId,
        decisionId,
        AuthorityDecision.Allow,
        scope,
        "AUT-STAGE12",
        "1.0",
        "EXACT_ROUTE_POLICY_REQUIRED",
        "BOUNDED_TO_EFFECTIVE_SCOPE",
        AuthorityReason.Allowed,
        now.AddMinutes(-1),
        now.AddHours(1),
        "evidence/aut/stage12");

ExternalAccessRequest Request(
    string principal = "application/example",
    string role = "service/data",
    string environment = ExternalAccessEnvironment.Live,
    string purpose = ExternalAccessPurpose.OperationalProviderData,
    string? destination = null,
    string authMode = ExternalAuthenticationMode.Public,
    string? credential = null,
    string requestId = "ext-req-1",
    string authorityRequestId = "auth-req-1",
    string authorityDecisionId = "auth-dec-1",
    string scope = "external.route/use") =>
    new(
        requestId,
        principal,
        role,
        environment,
        purpose,
        destination ?? Endpoint("https", "example.test", "/v1/data"),
        authMode,
        credential,
        authorityRequestId,
        authorityDecisionId,
        scope,
        now.AddMinutes(-1),
        now.AddMinutes(30),
        "corr-stage12");

ExternalAccessPolicyRule Rule(
    string principal = "application/example",
    string role = "service/data",
    string environment = ExternalAccessEnvironment.Live,
    string purpose = ExternalAccessPurpose.OperationalProviderData,
    string? destination = null,
    string authMode = ExternalAuthenticationMode.Public,
    string scope = "external.route/use",
    string ruleId = "route-rule-1",
    string policyId = "EXT-STAGE12",
    string policyVersion = "1.0",
    bool revoked = false) =>
    new(
        ruleId,
        policyId,
        policyVersion,
        principal,
        role,
        environment,
        purpose,
        destination ?? Endpoint("https", "example.test", "/v1/data"),
        authMode,
        scope,
        now.AddHours(-1),
        now.AddHours(2),
        revoked,
        "evidence/policy/stage12");

ExternalCredentialReference Credential(
    string referenceId = "credref/example-primary",
    string principal = "application/example",
    string role = "service/data",
    string environment = ExternalAccessEnvironment.Live,
    string purpose = ExternalAccessPurpose.OperationalProviderData,
    string? destination = null,
    bool revoked = false,
    DateTimeOffset? expiry = null) =>
    new(
        referenceId,
        principal,
        role,
        environment,
        purpose,
        destination ?? Endpoint("https", "example.test", "/v1/data"),
        now.AddHours(-1),
        expiry ?? now.AddHours(1),
        revoked,
        "evidence/credential/stage12");

ExternalAccessEvaluationContext Context(
    IReadOnlyCollection<ExternalAccessPolicyRule>? rules = null,
    AuthorityResult? authority = null,
    ExternalCredentialReference? credential = null,
    string evidence = "evidence/stage12/verification") =>
    new(rules ?? new[] { Rule() }, authority ?? Authority(), credential, now, evidence);

var positive = evaluator.Evaluate(Request(), Context());
Check("explicit public route allows only with exact policy and authority", positive.Decision == ExternalAccessDecision.Allow);
Check("allow is bounded and non-executing", positive.Constraints.Contains("NO_NETWORK_EXECUTION", StringComparison.Ordinal) && positive.Constraints.Contains("NO_BUSINESS_AUTHORITY", StringComparison.Ordinal));

var noPolicy = evaluator.Evaluate(Request(), Context(Array.Empty<ExternalAccessPolicyRule>()));
Check("public endpoint without route policy denies", noPolicy.Decision == ExternalAccessDecision.Deny && noPolicy.Reason == ExternalAccessReason.RouteNotAuthorized);

var deniedAuthority = Authority() with { Decision = AuthorityDecision.Deny, EffectiveScope = "NONE" };
var noAuthority = evaluator.Evaluate(Request(), Context(authority: deniedAuthority));
Check("AUT-001 denial cannot be bypassed", noAuthority.Reason == ExternalAccessReason.AuthorityMissingOrDenied);

var authorityMismatch = evaluator.Evaluate(Request(authorityDecisionId: "wrong"), Context());
Check("authority decision identity mismatch denies", authorityMismatch.Reason == ExternalAccessReason.AuthorityBindingMismatch);

var scopeMismatch = evaluator.Evaluate(Request(scope: "external.route/other"), Context());
Check("authority scope mismatch denies", scopeMismatch.Decision == ExternalAccessDecision.Deny);

var principalMismatch = evaluator.Evaluate(Request(principal: "application/other"), Context());
Check("same URL different principal is not same authority", principalMismatch.Reason == ExternalAccessReason.RouteNotAuthorized);

var roleMismatch = evaluator.Evaluate(Request(role: "service/other"), Context());
Check("same URL different service role is denied", roleMismatch.Reason == ExternalAccessReason.RouteNotAuthorized);

var purposeMismatch = evaluator.Evaluate(Request(purpose: ExternalAccessPurpose.PresentationData), Context());
Check("same URL different purpose is not same authority", purposeMismatch.Reason == ExternalAccessReason.RouteNotAuthorized);

var environmentMismatch = evaluator.Evaluate(Request(environment: ExternalAccessEnvironment.NonLive), Context());
Check("non-Live request cannot consume Live route", environmentMismatch.Decision == ExternalAccessDecision.Deny);

var destinationMismatch = evaluator.Evaluate(Request(destination: Endpoint("https", "example.test", "/v1/other")), Context());
Check("same provider host different destination is denied", destinationMismatch.Reason == ExternalAccessReason.RouteNotAuthorized);

var credentialDestination = Endpoint("https", "secure.example.test", "/v1/data");
var credentialRequest = Request(destination: credentialDestination, authMode: ExternalAuthenticationMode.CredentialReference, credential: "credref/secure-primary");
var credentialRule = Rule(destination: credentialDestination, authMode: ExternalAuthenticationMode.CredentialReference);
var credential = Credential(referenceId: "credref/secure-primary", destination: credentialDestination);
var credentialPositive = evaluator.Evaluate(credentialRequest, Context(new[] { credentialRule }, credential: credential));
Check("credential-reference route allows with exact active binding", credentialPositive.Decision == ExternalAccessDecision.Allow);

var missingCredential = evaluator.Evaluate(credentialRequest with { CredentialReferenceId = null }, Context(new[] { credentialRule }));
Check("missing credential reference denies", missingCredential.Reason == ExternalAccessReason.CredentialReferenceMissing);

var wrongCredential = evaluator.Evaluate(credentialRequest, Context(new[] { credentialRule }, credential: credential with { PrincipalIdentity = "application/other" }));
Check("credential principal mismatch denies", wrongCredential.Reason == ExternalAccessReason.CredentialReferenceMismatch);

var revokedCredential = evaluator.Evaluate(credentialRequest, Context(new[] { credentialRule }, credential: credential with { IsRevoked = true }));
Check("revoked credential reference denies", revokedCredential.Reason == ExternalAccessReason.CredentialReferenceRevoked);

var expiredCredential = evaluator.Evaluate(credentialRequest, Context(new[] { credentialRule }, credential: credential with { Expiry = now }));
Check("expired credential reference denies", expiredCredential.Reason == ExternalAccessReason.CredentialReferenceExpired);

var secretLikeReferenceId = string.Concat("token", "=", "plaintext", "-", "secret");
var secretLikeCredential = evaluator.Evaluate(credentialRequest, Context(new[] { credentialRule }, credential: credential with { ReferenceId = secretLikeReferenceId }));
Check("secret-like material is rejected as a credential reference", secretLikeCredential.Reason == ExternalAccessReason.CredentialReferenceInvalid);

var ambiguous = evaluator.Evaluate(Request(), Context(new[] { Rule(ruleId: "r1"), Rule(ruleId: "r2") }));
Check("conflicting duplicate exact rules fail closed", ambiguous.Reason == ExternalAccessReason.PolicyAmbiguous);

var revokedRule = evaluator.Evaluate(Request(), Context(new[] { Rule(revoked: true) }));
Check("revoked route rule denies", revokedRule.Decision == ExternalAccessDecision.Deny);

var missingEvidence = evaluator.Evaluate(Request(), Context(evidence: " "));
Check("missing evidence fails closed", missingEvidence.Reason == ExternalAccessReason.EvidenceMissing);

var unrelatedDestination = Endpoint("https", "unrelated.test", "/data");
var deterministicA = evaluator.Evaluate(Request(), Context(new[] { Rule(), Rule(destination: unrelatedDestination, ruleId: "other") }));
var deterministicB = evaluator.Evaluate(Request(), Context(new[] { Rule(destination: unrelatedDestination, ruleId: "other"), Rule() }));
Check("policy reorder preserves deterministic decision identity", deterministicA.DecisionId == deterministicB.DecisionId && deterministicA.Decision == deterministicB.Decision);

var zeroApplication = evaluator.Evaluate(null, new ExternalAccessEvaluationContext(Array.Empty<ExternalAccessPolicyRule>(), null, null, now, "evidence/zero-app"));
Check("zero-Application/no-request state fails safely without requiring an Application", zeroApplication.Decision == ExternalAccessDecision.Deny);

var knownDestinations = new[]
{
    Endpoint("wss", "stream.binance.com", ":9443"),
    Endpoint("wss", "ws-feed.exchange.coinbase.com"),
    Endpoint("wss", "stream.bybit.com", "/v5/public/spot"),
    Endpoint("wss", "stream.data.alpaca.markets", "/v2/iex"),
    Endpoint("wss", "ws.finnhub.io"),
    Endpoint("https", "paper-api.alpaca.markets", "/v2/assets"),
    Endpoint("https", "data.alpaca.markets", "/v2/stocks/bars"),
    Endpoint("https", "data-api.binance.vision", "/api/v3/exchangeInfo"),
    Endpoint("https", "data-api.binance.vision", "/api/v3/klines"),
    Endpoint("wss", "stream.binance.com", ":9443/ws/!miniTicker@arr")
};
var fixtureUrisValid = knownDestinations.All(x => Uri.TryCreate(x, UriKind.Absolute, out var uri) && (uri.Scheme == "https" || uri.Scheme == "wss"));
Check("all current Stage 12 Shared-Web destination fixtures preserve exact HTTPS/WSS identities", fixtureUrisValid && knownDestinations.Distinct(StringComparer.Ordinal).Count() == knownDestinations.Length);

var purposeClasses = new[]
{
    ExternalAccessPurpose.Research,
    ExternalAccessPurpose.NonLiveValidation,
    ExternalAccessPurpose.OperationalProviderData,
    ExternalAccessPurpose.BrokerExecution,
    ExternalAccessPurpose.PresentationData
};
Check("generic egress purpose classes remain distinct", purposeClasses.Distinct(StringComparer.Ordinal).Count() == 5);

var evaluatorMethods = typeof(ExternalAccessEvaluator).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
var forbiddenExecutionNames = new[] { "Connect", "Send", "Execute", "Http", "WebSocket", "Socket", "Broker", "Order" };
Check("Stage 12 evaluator exposes no network/execution method surface", evaluatorMethods.All(m => forbiddenExecutionNames.All(x => !m.Name.Contains(x, StringComparison.OrdinalIgnoreCase))));

var publicPropertyNames = typeof(ExternalCredentialReference).GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(x => x.Name).ToArray();
var forbiddenSecretNames = new[] { "Secret", "Token", "Password", "ApiKey", "KeyValue", "CredentialValue" };
Check("credential object exposes reference metadata only, not secret-value fields", publicPropertyNames.All(p => forbiddenSecretNames.All(x => !p.Contains(x, StringComparison.OrdinalIgnoreCase))));

var stage13Leakage = typeof(ExternalAccessEvaluator).Assembly.GetTypes()
    .Where(t => t.Namespace == typeof(ExternalAccessEvaluator).Namespace && t.Name.Contains("External", StringComparison.Ordinal))
    .Select(t => t.FullName ?? t.Name)
    .All(name => !name.Contains("FSA", StringComparison.OrdinalIgnoreCase) && !name.Contains("FactoryReset", StringComparison.OrdinalIgnoreCase) && !name.Contains("ControlledRevival", StringComparison.OrdinalIgnoreCase));
Check("Stage 13 FSA-specific control-plane semantics do not leak into Stage 12", stage13Leakage);

Console.WriteLine("STAGE12_EXTERNAL_ACCESS_VERIFIER = PASS");
Console.WriteLine($"CHECKS = {passed}/{total}");
Console.WriteLine("DEFAULT_DENY = PASS");
Console.WriteLine("EXACT_ROUTE_IDENTITY = PASS");
Console.WriteLine("CREDENTIAL_REFERENCE_SECURITY = PASS");
Console.WriteLine("NON_LIVE_ISOLATION = PASS");
Console.WriteLine("PURPOSE_SEPARATION = PASS");
Console.WriteLine("NO_NETWORK_EXECUTION_SURFACE = PASS");
Console.WriteLine("ZERO_APPLICATION_OPERATION = VALID");
Console.WriteLine("PUBLIC_ENDPOINT != UNRESTRICTED_EGRESS_AUTHORITY");
Console.WriteLine("SAME_URL != SAME_AUTHORITY");
Console.WriteLine("SAME_PROVIDER != SAME_AUTHORITY");
Console.WriteLine("ROUTE_AUTHORIZED != CONNECTION_EXECUTED");
Console.WriteLine("TECHNICAL_EGRESS_AUTHORIZATION != BUSINESS_AUTHORITY");
