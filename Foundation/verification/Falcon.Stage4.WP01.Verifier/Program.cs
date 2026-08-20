using System;
using System.Collections.Generic;
using Foundation.Authority;
using Foundation.Contracts;

namespace Falcon.Stage4.WP01.Verifier;

internal static class Program
{
    private static readonly DateTimeOffset Now = new(2026, 8, 5, 18, 0, 0, TimeSpan.Zero);

    private static int Main()
    {
        var failures = new List<string>();
        var engine = new DefaultDenyAuthorityEngine();

        Verify("allow", engine.Evaluate(Request(), Context()), AuthorityDecision.Allow, AuthorityReason.Allowed, failures);
        Verify("null context", engine.Evaluate(Request(), null), AuthorityDecision.Deny, AuthorityReason.MalformedContext, failures);
        Verify("unknown actor", engine.Evaluate(Request(actor: "actor/unknown"), Context()), AuthorityDecision.Deny, AuthorityReason.ActorUnknown, failures);
        Verify("missing policy", engine.Evaluate(Request(), new AuthorityEvaluationContext(null, Delegation(), Fitness(), Now, "evidence/authority/wp01/001")), AuthorityDecision.Deny, AuthorityReason.PolicyMissing, failures);
        Verify("ambiguous policy", engine.Evaluate(Request(), Context(policy: Policy(isAmbiguous: true))), AuthorityDecision.Deny, AuthorityReason.PolicyAmbiguous, failures);
        Verify("malformed policy id", engine.Evaluate(Request(), Context(policy: Policy(policyId: ""))), AuthorityDecision.Deny, AuthorityReason.PolicyMalformed, failures);
        Verify("malformed policy collection", engine.Evaluate(Request(), Context(policy: MalformedPolicyWithNullActors())), AuthorityDecision.Deny, AuthorityReason.PolicyMalformed, failures);
        Verify("malformed policy time", engine.Evaluate(Request(), Context(policy: Policy(effectiveFrom: Now.AddDays(1), expiry: Now.AddDays(-1)))), AuthorityDecision.Deny, AuthorityReason.PolicyMalformed, failures);
        Verify("missing evidence", engine.Evaluate(Request(), Context(evidenceReference: "")), AuthorityDecision.Deny, AuthorityReason.EvidenceMissing, failures);
        Verify("excessive scope", engine.Evaluate(Request(scope: "foundation:admin"), Context()), AuthorityDecision.Deny, AuthorityReason.ScopeExceeded, failures);
        Verify("expired request", engine.Evaluate(Request(expiry: Now), Context()), AuthorityDecision.Deny, AuthorityReason.Expired, failures);
        Verify("future request", engine.Evaluate(Request(requestTime: Now.AddMinutes(1)), Context()), AuthorityDecision.Deny, AuthorityReason.Expired, failures);
        Verify("revoked delegation", engine.Evaluate(Request(), Context(delegation: Delegation(revoked: true))), AuthorityDecision.Deny, AuthorityReason.DelegationRevoked, failures);
        Verify("delegation scope", engine.Evaluate(Request(scope: "foundation:authority:write"), Context(policy: Policy(scopes: ["foundation:authority"]), delegation: Delegation(scopes: ["foundation:authority:read"]))), AuthorityDecision.Deny, AuthorityReason.DelegationScopeExceeded, failures);
        Verify("malformed delegation", engine.Evaluate(Request(), Context(delegation: Delegation(delegationId: ""))), AuthorityDecision.Deny, AuthorityReason.DelegationMalformed, failures);
        Verify("malformed delegation collection", engine.Evaluate(Request(), Context(delegation: MalformedDelegationWithNullScopes())), AuthorityDecision.Deny, AuthorityReason.DelegationMalformed, failures);
        Verify("insufficient fitness", engine.Evaluate(Request(), Context(fitness: Fitness(sufficient: false))), AuthorityDecision.Deny, AuthorityReason.FitnessInsufficient, failures);
        Verify("fitness mismatch", engine.Evaluate(Request(requiredFitness: "FIT-STRONG"), Context(fitness: Fitness(level: "FIT"))), AuthorityDecision.Deny, AuthorityReason.FitnessLevelMismatch, failures);
        Verify("future fitness", engine.Evaluate(Request(), Context(fitness: Fitness(observedAt: Now.AddMinutes(1)))), AuthorityDecision.Deny, AuthorityReason.FitnessInsufficient, failures);
        Verify("expired fitness", engine.Evaluate(Request(), Context(fitness: Fitness(expiry: Now))), AuthorityDecision.Deny, AuthorityReason.FitnessInsufficient, failures);
        Verify("malformed fitness", engine.Evaluate(Request(), Context(fitness: Fitness(level: ""))), AuthorityDecision.Deny, AuthorityReason.FitnessMalformed, failures);
        Verify("security context", engine.Evaluate(Request(securityContext: "untrusted"), Context()), AuthorityDecision.Deny, AuthorityReason.SecurityContextRejected, failures);

        var first = engine.Evaluate(Request(), Context());
        var second = engine.Evaluate(Request(), Context());
        if (!StringComparer.Ordinal.Equals(first.DecisionId, second.DecisionId) || first != second)
        {
            failures.Add("deterministic replay mismatch");
        }

        if (first.Decision != AuthorityDecision.Allow || first.EffectiveScope != "foundation:authority:read")
        {
            failures.Add("allowed scope was not preserved exactly");
        }

        VerifyMutation("policy action", first, engine.Evaluate(Request(), Context(policy: Policy(actions: ["authority.inspect"]))), failures);
        VerifyMutation("policy actor set", first, engine.Evaluate(Request(), Context(policy: Policy(actors: ["actor/another"]))), failures);
        VerifyMutation("policy security set", first, engine.Evaluate(Request(), Context(policy: Policy(securityContexts: ["other-context"]))), failures);
        VerifyMutation("policy expiry", first, engine.Evaluate(Request(), Context(policy: Policy(expiry: Now.AddHours(2)))), failures);
        VerifyMutation("policy ambiguity", first, engine.Evaluate(Request(), Context(policy: Policy(isAmbiguous: true))), failures);
        VerifyMutation("delegation provenance", first, engine.Evaluate(Request(), Context(delegation: Delegation(provenance: "authority/other"))), failures);
        VerifyMutation("delegation expiry", first, engine.Evaluate(Request(), Context(delegation: Delegation(expiry: Now.AddMinutes(40)))), failures);
        VerifyMutation("delegation revocation", first, engine.Evaluate(Request(), Context(delegation: Delegation(revoked: true))), failures);
        VerifyMutation("fitness level", first, engine.Evaluate(Request(), Context(fitness: Fitness(level: "FIT-STRONG"))), failures);
        VerifyMutation("fitness observation", first, engine.Evaluate(Request(), Context(fitness: Fitness(observedAt: Now.AddMinutes(-4)))), failures);
        VerifyMutation("fitness sufficiency", first, engine.Evaluate(Request(), Context(fitness: Fitness(sufficient: false))), failures);
        VerifyMutation("evaluation evidence", first, engine.Evaluate(Request(), Context(evidenceReference: "evidence/authority/wp01/002")), failures);

        if (failures.Count > 0)
        {
            Console.Error.WriteLine("Stage 4 WP-01 verifier: FAIL");
            foreach (var failure in failures) Console.Error.WriteLine("- " + failure);
            return 1;
        }

        Console.WriteLine("Stage 4 WP-01 verifier: PASS");
        Console.WriteLine("CON-002 default-deny, exact fitness binding, malformed-context fail-closed behavior, material-input identity binding, and deterministic replay verified.");
        Console.WriteLine("No execution or authoritative state mutation surface exists in Foundation.Authority.");
        Console.WriteLine("Decision identity: " + first.DecisionId);
        return 0;
    }

    private static void Verify(string name, AuthorityResult result, string decision, string reason, ICollection<string> failures)
    {
        if (!StringComparer.Ordinal.Equals(result.Decision, decision) || !StringComparer.Ordinal.Equals(result.Reason, reason))
        {
            failures.Add($"{name}: expected {decision}/{reason}, actual {result.Decision}/{result.Reason}");
        }
    }

    private static void VerifyMutation(string name, AuthorityResult baseline, AuthorityResult mutated, ICollection<string> failures)
    {
        if (StringComparer.Ordinal.Equals(baseline.DecisionId, mutated.DecisionId))
        {
            failures.Add($"{name}: material mutation did not change decision identity");
        }
    }

    private static AuthorityRequest Request(
        string actor = "actor/foundation-controller",
        string scope = "foundation:authority:read",
        string securityContext = "foundation-internal",
        string requiredFitness = "FIT",
        DateTimeOffset? requestTime = null,
        DateTimeOffset? expiry = null) => new(
            "request/wp01/001", actor, "authority.evaluate", "foundation.authority", "governed-evaluation",
            scope, "foundation-control-plane", securityContext, requiredFitness, "correlation/wp01/001",
            requestTime ?? Now.AddMinutes(-1), expiry ?? Now.AddMinutes(30));

    private static AuthorityPolicy Policy(
        bool isAmbiguous = false,
        string policyId = "policy/foundation-authority",
        string version = "1.0.0",
        string provenance = "authority/owner-approved",
        DateTimeOffset? effectiveFrom = null,
        DateTimeOffset? expiry = null,
        IReadOnlyCollection<string>? actors = default,
        IReadOnlyCollection<string>? actions = default,
        IReadOnlyCollection<string>? resources = default,
        IReadOnlyCollection<string>? purposes = default,
        IReadOnlyCollection<string>? scopes = default,
        IReadOnlyCollection<string>? securityContexts = default) => new(
            policyId, version, provenance, effectiveFrom ?? Now.AddDays(-1), expiry ?? Now.AddDays(1),
            actors ?? ["actor/foundation-controller"],
            actions ?? ["authority.evaluate"],
            resources ?? ["foundation.authority"],
            purposes ?? ["governed-evaluation"],
            scopes ?? ["foundation:authority"],
            securityContexts ?? ["foundation-internal"],
            isAmbiguous);

    private static DelegationEvidence Delegation(
        bool revoked = false,
        string delegationId = "delegation/wp01/001",
        string actor = "actor/foundation-controller",
        string provenance = "authority/owner-approved",
        IReadOnlyCollection<string>? scopes = default,
        DateTimeOffset? effectiveFrom = null,
        DateTimeOffset? expiry = null) => new(
            delegationId, actor, provenance,
            scopes ?? ["foundation:authority"],
            effectiveFrom ?? Now.AddDays(-1),
            expiry ?? Now.AddHours(1),
            revoked);

    private static FitnessEvidence Fitness(
        bool sufficient = true,
        string subject = "actor/foundation-controller",
        string level = "FIT",
        DateTimeOffset? observedAt = null,
        DateTimeOffset? expiry = null,
        string evidence = "evidence/fitness/wp01/001") => new(
            subject, level, sufficient,
            observedAt ?? Now.AddMinutes(-5),
            expiry ?? Now.AddMinutes(20),
            evidence);

    private static AuthorityPolicy MalformedPolicyWithNullActors() => new(
        "policy/foundation-authority", "1.0.0", "authority/owner-approved",
        Now.AddDays(-1), Now.AddDays(1), null!, ["authority.evaluate"],
        ["foundation.authority"], ["governed-evaluation"],
        ["foundation:authority"], ["foundation-internal"]);

    private static DelegationEvidence MalformedDelegationWithNullScopes() => new(
        "delegation/wp01/001", "actor/foundation-controller", "authority/owner-approved",
        null!, Now.AddDays(-1), Now.AddHours(1), false);

    private static AuthorityEvaluationContext Context(
        AuthorityPolicy? policy = default,
        DelegationEvidence? delegation = default,
        FitnessEvidence? fitness = default,
        string evidenceReference = "evidence/authority/wp01/001") => new(
            policy ?? Policy(), delegation ?? Delegation(), fitness ?? Fitness(), Now, evidenceReference);
}
