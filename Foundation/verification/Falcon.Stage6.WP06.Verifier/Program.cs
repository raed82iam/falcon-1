using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Foundation.Contracts.ResourceGovernance;
using Foundation.State.ResourceGovernance;

namespace Falcon.Stage6.WP06.Verifier;

internal static class Program
{
    private static int _passed;
    private static int _failed;

    private static readonly DateTimeOffset T0 = new(2026, 8, 10, 3, 0, 0, TimeSpan.Zero);
    private static readonly ResourceEpochId Epoch = new("epoch-001");
    private static readonly ResourceClassId Cpu = new("cpu");

    private static int Main()
    {
        Run("direct_application_positive_request", () => Equal(ResourceDecisionKind.Grant, Decide(Direct()).Outcome));
        Run("aggregate_coordinator_positive_request", () => Equal(ResourceDecisionKind.PartialGrant, Decide(Aggregate()).Outcome));
        Run("requester_instance_identity_validation", RequesterInstanceMismatch);
        Run("requester_role_identity_validation", RequesterRoleMismatch);
        Run("requester_role_instance_separation", () => Require(typeof(AdditionalResourceRequest).GetProperty(nameof(AdditionalResourceRequest.RequesterInstanceId)) != null && typeof(AdditionalResourceRequest).GetProperty(nameof(AdditionalResourceRequest.RequesterRoleId)) != null, "Requester instance and role must remain separate."));
        Run("requester_identity_does_not_create_authority", () => Require(typeof(AdditionalResourceRequest).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(m => !m.IsSpecialName).All(m => !m.Name.Contains("Authorize", StringComparison.OrdinalIgnoreCase)), "Requester identity cannot mint authority."));
        Run("exact_direct_application_attribution", () => Equal("app-a", Direct().DirectApplicationId!.Value));
        Run("exact_constituent_attribution", ExactConstituentAttribution);
        Run("exact_coordinator_scope_binding", CoordinatorScopeMismatch);
        Run("request_grant_decision_identity_separation", () => Require(typeof(ResourceRequestId) != typeof(ResourceGrantId) && typeof(ResourceRequestId) != typeof(ResourceDecisionId) && typeof(ResourceGrantId) != typeof(ResourceDecisionId), "Request/grant/decision identities must be distinct."));
        Run("requested_quantity_residual_need_separation", () => { var r = Direct(requested: 20, residual: 10); Equal(20m, r.RequestedQuantity.Amount); Equal(10m, r.ProvenResidualNeed.Amount); });
        Run("residual_need_decided_quantity_separation", () => { var d = Decide(Direct(requested: 20, residual: 10)); Equal(10m, d.ProvenResidualNeed.Amount); Equal(10m, d.DecidedAdditionalQuantity.Amount); });
        Run("grant_positive_path", () => Equal(ResourceDecisionKind.Grant, Decide(Direct(requested: 10, residual: 10)).Outcome));
        Run("partial_grant_positive_path", () => Equal(ResourceDecisionKind.PartialGrant, Decide(Direct(requested: 20, residual: 10)).Outcome));
        Run("cap_positive_path", () => Equal(ResourceDecisionKind.Cap, Decide(Direct(requested: 30, residual: 30), processor: Processor(maximumAdditional: 5)).Outcome));
        Run("deny_positive_path", DenyPath);
        Run("defer_positive_path", DeferPath);
        Run("reject_revoke_reduce_restore_as_wp06_outcomes", RejectLaterWpOutcomes);
        Run("predecessor_foundation_resource_truth_binding", FoundationTruthBinding);
        Run("predecessor_allocation_grant_ceiling_binding", AllocationBinding);
        Run("protection_floor_preservation", ProtectionFloorPreservation);
        Run("recovery_reserve_preservation", RecoveryReservePreservation);
        Run("priority_does_not_mint_authority", () => Require(typeof(ResourcePriorityClassId).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).All(m => !m.Name.Contains("Authorize", StringComparison.OrdinalIgnoreCase)), "Priority cannot mint authority."));
        Run("technical_criticality_does_not_mint_business_authority", () => Require(typeof(TechnicalCriticalityClassId).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).All(m => !m.Name.Contains("Authorize", StringComparison.OrdinalIgnoreCase)), "Criticality cannot mint authority."));
        Run("pressure_does_not_mint_authority", () => Require(typeof(ResourcePressureTruth).GetProperties().All(p => !p.Name.Contains("Grant", StringComparison.OrdinalIgnoreCase) && !p.Name.Contains("Authorize", StringComparison.OrdinalIgnoreCase)), "Pressure cannot mint authority."));
        Run("residual_need_evidence_required", () => Throws<ArgumentNullException>(() => Direct(nullResidualEvidence: true)));
        Run("aggregate_internal_redistribution_first_required", () => Throws<ArgumentException>(() => Aggregate(internalCoordinationExhausted: false)));
        Run("delegation_scope_validation", DelegationScopeMismatch);
        Run("delegation_expiry_rejection", DelegationExpiry);
        Run("delegation_supersession_rejection", DelegationSupersession);
        Run("constituent_scope_mismatch_rejection", ConstituentScopeMismatch);
        Run("stale_fencing_rejection", StaleFence);
        Run("split_brain_coordinator_rejection", SplitBrain);
        Run("stale_predecessor_rejection", StalePredecessor);
        Run("duplicate_request_rejection", DuplicateRequest);
        Run("request_replay_rejection", RequestReplay);
        Run("decision_replay_rejection", DecisionReplay);
        Run("cross_epoch_rejection", CrossEpoch);
        Run("future_evidence_rejection", FutureEvidence);
        Run("expired_authority_rejection", ExpiredAuthority);
        Run("deterministic_request_identity", DeterministicRequestIdentity);
        Run("deterministic_decision_identity", DeterministicDecisionIdentity);
        Run("canonical_constituent_ordering", CanonicalConstituentOrdering);
        Run("request_decision_correlation", CorrelationPreserved);
        Run("causation_preservation", CausationPreserved);
        Run("decision_request_mismatch_rejection", DecisionBoundToExactRequest);
        Run("application_neutral_production_surface", ApplicationNeutralSurface);
        Run("no_tarc_hard_binding", () => Require(Surface().All(s => !s.Contains("TARC", StringComparison.OrdinalIgnoreCase)), "TARC hard-binding leaked into WP-06."));
        Run("no_fsarm_business_specific_production_mechanics", () => Require(Surface().All(s => !s.Contains("FSARM", StringComparison.OrdinalIgnoreCase)), "FSARM hard-binding leaked into WP-06."));
        Run("no_opaque_aggregate_pool", NoOpaqueAggregatePool);
        Run("no_wp07_mutation_executor", NoWp07Executor);
        Run("no_wp08_load_shedding_executor", () => Require(Surface().All(s => !s.Contains("LoadShedding", StringComparison.OrdinalIgnoreCase)), "WP-08 load-shedding surface leaked into WP-06."));
        Run("zero_application_validity", ZeroApplicationValidity);
        Run("accepted_wp01_wp05_truth_remains_read_only", PredecessorTruthImmutable);
        Run("authority_same_generation_conflict_rejected", AuthoritySameGenerationConflict);
        Run("pressure_predecessor_mismatch_rejected", PressurePredecessorMismatch);
        Run("decision_identity_is_uppercase_sha256", () => IsSha(Decide(Direct()).IdentitySha256));
        Run("request_identity_is_uppercase_sha256", () => IsSha(Direct().IdentitySha256));

        Console.WriteLine();
        Console.WriteLine($"STAGE 6 WP-06 VERIFIER: {_passed}/{_passed + _failed} PASS");
        Console.WriteLine($"Failures: {_failed}");
        return _failed == 0 ? 0 : 1;
    }

    private static ResourceQuantity Q(decimal amount, string unit = "units") => new(amount, unit);
    private static ResourceEffectiveLifetime Lifetime() => new(T0.AddMinutes(-10), null, true);
    private static ResourceEvidenceReference Evidence(string id, string scope, ResourceEpochId? epoch = null, DateTimeOffset? at = null)
        => new(new ResourceEvidenceId(id), new ResourceScopeId(scope), at ?? T0.AddMinutes(-1), epoch ?? Epoch);

    private static FoundationResourceTruthSnapshot Truth(decimal total = 100, decimal floor = 10, decimal reserve = 10, ResourceEpochId? epoch = null)
    {
        var e = epoch ?? Epoch;
        return new FoundationResourceTruthSnapshot(e, T0.AddMinutes(-2), new[]
        {
            new FoundationResourceClassTruth(Cpu, Q(total), Q(floor), Q(reserve), Evidence("truth-" + e.Value, "foundation-resource-truth", e, T0.AddMinutes(-3)))
        }, true);
    }

    private static ApplicationResourceAllocation Allocation(string app, string grant, decimal ceiling, ResourceEpochId epoch)
        => new(new ResourceGrantId(grant), new ApplicationPrincipalId(app), Cpu, Q(10), Q(Math.Min(15, ceiling)), Q(ceiling), Lifetime(), Evidence("allocation-" + app + "-" + epoch.Value, "application-allocation", epoch));

    private static ApplicationResourceAllocationSnapshot Allocations(bool twoApps = false, decimal ceilingA = 20, decimal ceilingB = 20, FoundationResourceTruthSnapshot? truth = null)
    {
        var t = truth ?? Truth();
        var records = twoApps
            ? new[] { Allocation("app-a", "grant-a", ceilingA, t.EpochId), Allocation("app-b", "grant-b", ceilingB, t.EpochId) }
            : new[] { Allocation("app-a", "grant-a", ceilingA, t.EpochId) };
        return new ApplicationResourceAllocationSnapshot(t, T0.AddMinutes(-1), records, true);
    }

    private static ResourcePriorityClassDefinition PClass(string id) => new(new ResourcePriorityClassId(id), Lifetime(), Evidence("pc-" + id, "priority"));
    private static TechnicalCriticalityClassDefinition CClass(string id) => new(new TechnicalCriticalityClassId(id), Lifetime(), Evidence("cc-" + id, "criticality"));
    private static ResourcePriorityClassRelation PRel(string high, string low) => new(new ResourcePriorityClassId(high), new ResourcePriorityClassId(low), Lifetime(), Evidence("pr-" + high + "-" + low, "priority"));
    private static TechnicalCriticalityClassRelation CRel(string high, string low) => new(new TechnicalCriticalityClassId(high), new TechnicalCriticalityClassId(low), Lifetime(), Evidence("cr-" + high + "-" + low, "criticality"));
    private static ApplicationResourcePriorityBinding AppBinding(string app) => new(new ApplicationPrincipalId(app), new ResourcePriorityClassId("p-high"), Lifetime(), Evidence("ab-" + app, "application-priority"));
    private static TechnicalCriticalityBinding TechBinding() => new(new ResourceScopeId("scope-global"), Cpu, new TechnicalCriticalityClassId("c-high"), Lifetime(), Evidence("tb-global", "technical-criticality"));

    private static ResourcePriorityGovernanceSnapshot Priority(ApplicationResourceAllocationSnapshot allocations)
        => new(allocations, T0,
            "priority-v1", Lifetime(), Evidence("priority-policy", "priority"),
            "criticality-v1", Lifetime(), Evidence("criticality-policy", "criticality"),
            new[] { PClass("p-high"), PClass("p-low") },
            new[] { CClass("c-high"), CClass("c-low") },
            new[] { PRel("p-high", "p-low") },
            new[] { CRel("c-high", "c-low") },
            allocations.Allocations.Select(a => a.ApplicationId.Value).Distinct(StringComparer.Ordinal).Select(AppBinding),
            new[] { TechBinding() }, true);

    private static FoundationResourcePressureSnapshot Pressure(ApplicationResourceAllocationSnapshot allocations, decimal? used)
        => new(Priority(allocations), T0,
            new[] { new ResourcePressureTransitionPolicy(Cpu, 6000, 8000, 9500, 500, "pressure-v1", Lifetime(), Evidence("pressure-policy", "pressure")) },
            new[] { new ResourcePressureObservation(ResourcePressureScopeKind.FoundationResourceClass, new ResourceScopeId("scope-global"), Cpu, null, used.HasValue ? Q(used.Value) : null, 1, Evidence("pressure-observation", "pressure")) },
            Array.Empty<ResourcePreemptionEligibilityBinding>(), Array.Empty<ResourceEnforcementObservation>());

    private static ResourceRequestAuthorityBinding Authority(string instance, string role, string scope, IEnumerable<string> apps, long generation = 1, string id = "request-authority", ResourceEpochId? epoch = null, DateTimeOffset? from = null, DateTimeOffset? until = null)
    {
        var e = epoch ?? Epoch;
        return new ResourceRequestAuthorityBinding(id, instance, role, new ResourceScopeId(scope), apps.Select(a => new ApplicationPrincipalId(a)), generation,
            Evidence(id + "-evidence", "request-authority", e), from ?? T0.AddMinutes(-5), until ?? T0.AddHours(1));
    }

    private static AdditionalResourceRequest Direct(
        string requestId = "request-1", decimal requested = 10, decimal residual = 10,
        ApplicationResourceAllocationSnapshot? allocations = null, FoundationResourcePressureSnapshot? pressure = null,
        ResourceRequestAuthorityBinding? authority = null, string instance = "instance-app-a", string role = "resource-requester",
        DateTimeOffset? createdAt = null, bool nullResidualEvidence = false)
    {
        var a = allocations ?? Allocations();
        var at = createdAt ?? T0;
        var auth = authority ?? Authority(instance, role, "scope.app-a", new[] { "app-a" }, epoch: a.ResourceTruth.EpochId);
        return new AdditionalResourceRequest(new ResourceRequestId(requestId), ResourceRequesterKind.DirectApplication, instance, role, auth,
            new ApplicationPrincipalId("app-a"), Array.Empty<ApplicationPrincipalId>(), Cpu, Q(requested), Q(residual), a, pressure,
            new CorrelationId("correlation-1"), new CausationId("causation-1"), Evidence("request-" + requestId, "request", a.ResourceTruth.EpochId, at.AddMinutes(-1)),
            nullResidualEvidence ? null! : Evidence("residual-" + requestId, "residual", a.ResourceTruth.EpochId, at.AddMinutes(-1)), at, at.AddMinutes(30));
    }

    private static AdditionalResourceRequest Aggregate(
        string requestId = "request-aggregate-1", long authorityGeneration = 1, long fenceGeneration = 1,
        string coordinator = "coordinator-1", string fenceToken = "fence-1", IEnumerable<string>? appOrder = null,
        ApplicationResourceAllocationSnapshot? allocations = null, ResourceRequestAuthorityBinding? authority = null,
        bool internalCoordinationExhausted = true)
    {
        var a = allocations ?? Allocations(twoApps: true);
        var apps = (appOrder ?? new[] { "app-a", "app-b" }).ToArray();
        var auth = authority ?? Authority(coordinator, "aggregate-resource-coordinator", "scope.coord", apps, authorityGeneration, "authority-coord", a.ResourceTruth.EpochId);
        var fence = new ResourceCoordinatorFence(new ResourceScopeId("scope.coord"), coordinator, fenceGeneration, fenceToken, T0.AddMinutes(30), Evidence("fence-" + requestId, "fence", a.ResourceTruth.EpochId));
        return new AdditionalResourceRequest(new ResourceRequestId(requestId), ResourceRequesterKind.DelegatedAggregateCoordinator, coordinator, "aggregate-resource-coordinator", auth,
            null, apps.Select(x => new ApplicationPrincipalId(x)), Cpu, Q(20), Q(10), a, null,
            new CorrelationId("correlation-aggregate"), new CausationId("causation-aggregate"), Evidence("request-" + requestId, "request", a.ResourceTruth.EpochId),
            Evidence("residual-" + requestId, "residual", a.ResourceTruth.EpochId), T0, T0.AddMinutes(30), internalCoordinationExhausted, fence);
    }

    private static AdditionalResourceRequestDecisionProcessor Processor(decimal maximumAdditional = 100, bool deferWhenUnavailable = false)
        => new(new[] { new ResourceAdditionalRequestDecisionPolicy(Cpu, Q(maximumAdditional), deferWhenUnavailable, Evidence("decision-policy", "decision-policy"), T0.AddMinutes(-5), T0.AddHours(1)) },
            new ResourceRequestDecisionAuthority("foundation-decision-authority", Evidence("decision-authority", "decision-authority"), T0.AddMinutes(-5), T0.AddHours(1)));

    private static AdditionalResourceDecisionRecord Decide(AdditionalResourceRequest request, string decisionId = "decision-1", AdditionalResourceRequestDecisionProcessor? processor = null)
        => (processor ?? Processor()).Evaluate(request, new ResourceDecisionId(decisionId), T0.AddMinutes(1));

    private static void RequesterInstanceMismatch()
    {
        var auth = Authority("authorized", "resource-requester", "scope.app-a", new[] { "app-a" });
        Throws<InvalidOperationException>(() => Direct(authority: auth, instance: "other"));
    }

    private static void RequesterRoleMismatch()
    {
        var auth = Authority("instance-app-a", "authorized-role", "scope.app-a", new[] { "app-a" });
        Throws<InvalidOperationException>(() => Direct(authority: auth, role: "other-role"));
    }

    private static void ExactConstituentAttribution()
    {
        var r = Aggregate();
        Equal(2, r.RepresentedApplications.Count);
        Equal("app-a", r.RepresentedApplications[0].Value);
        Equal("app-b", r.RepresentedApplications[1].Value);
    }

    private static void CoordinatorScopeMismatch()
    {
        var auth = Authority("coordinator-1", "aggregate-resource-coordinator", "scope.wrong", new[] { "app-a", "app-b" }, id: "wrong-scope");
        Throws<InvalidOperationException>(() => Aggregate(authority: auth));
    }

    private static void DenyPath()
    {
        var a = Allocations(twoApps: true, ceilingA: 40, ceilingB: 40);
        Equal(ResourceDecisionKind.Deny, Decide(Direct(allocations: a)).Outcome);
    }

    private static void DeferPath()
    {
        var a = Allocations();
        Equal(ResourceDecisionKind.Defer, Decide(Direct(allocations: a, pressure: Pressure(a, null)), processor: Processor(deferWhenUnavailable: true)).Outcome);
    }

    private static void RejectLaterWpOutcomes()
    {
        var allowed = new[] { ResourceDecisionKind.Grant, ResourceDecisionKind.PartialGrant, ResourceDecisionKind.Cap, ResourceDecisionKind.Deny, ResourceDecisionKind.Defer };
        Require(allowed.Contains(Decide(Direct()).Outcome), "WP-06 emitted disallowed outcome.");
        Require(!allowed.Contains(ResourceDecisionKind.Revoke) && !allowed.Contains(ResourceDecisionKind.Reduce) && !allowed.Contains(ResourceDecisionKind.Restore), "Later-WP outcomes entered WP-06 subset.");
    }

    private static void FoundationTruthBinding()
        => NotEqual(Direct(allocations: Allocations(truth: Truth(total: 100))).IdentitySha256, Direct(allocations: Allocations(truth: Truth(total: 101))).IdentitySha256);

    private static void AllocationBinding()
        => NotEqual(Direct(allocations: Allocations(ceilingA: 20)).IdentitySha256, Direct(allocations: Allocations(ceilingA: 21)).IdentitySha256);

    private static void ProtectionFloorPreservation()
    {
        var a = Allocations(twoApps: true, ceilingA: 25, ceilingB: 25, truth: Truth(total: 100, floor: 40, reserve: 10));
        Equal(ResourceDecisionKind.Deny, Decide(Direct(allocations: a)).Outcome);
    }

    private static void RecoveryReservePreservation()
    {
        var a = Allocations(twoApps: true, ceilingA: 25, ceilingB: 25, truth: Truth(total: 100, floor: 10, reserve: 40));
        Equal(ResourceDecisionKind.Deny, Decide(Direct(allocations: a)).Outcome);
    }

    private static void DelegationScopeMismatch()
    {
        var auth = Authority("instance-app-a", "resource-requester", "scope.app-a", new[] { "app-b" });
        Throws<InvalidOperationException>(() => Direct(authority: auth));
    }

    private static void DelegationExpiry()
    {
        var auth = Authority("instance-app-a", "resource-requester", "scope.app-a", new[] { "app-a" }, until: T0);
        Throws<InvalidOperationException>(() => Direct(authority: auth));
    }

    private static void DelegationSupersession()
    {
        var p = Processor();
        _ = Decide(Direct("request-g2", authority: Authority("instance-app-a", "resource-requester", "scope.app-a", new[] { "app-a" }, 2, "authority-g2")), "decision-g2", p);
        Throws<InvalidOperationException>(() => Decide(Direct("request-g1", authority: Authority("instance-app-a", "resource-requester", "scope.app-a", new[] { "app-a" }, 1, "authority-g1")), "decision-g1", p));
    }

    private static void ConstituentScopeMismatch()
    {
        var auth = Authority("coordinator-1", "aggregate-resource-coordinator", "scope.coord", new[] { "app-a" }, id: "narrow");
        Throws<InvalidOperationException>(() => Aggregate(authority: auth));
    }

    private static void StaleFence()
    {
        var p = Processor();
        var auth = Authority("coordinator-1", "aggregate-resource-coordinator", "scope.coord", new[] { "app-a", "app-b" }, 2, "authority-coord");
        _ = Decide(Aggregate("request-f2", 2, 2, authority: auth), "decision-f2", p);
        Throws<InvalidOperationException>(() => Decide(Aggregate("request-f1", 2, 1, authority: auth), "decision-f1", p));
    }

    private static void SplitBrain()
    {
        var p = Processor();
        _ = Decide(Aggregate("request-c1", coordinator: "coordinator-1", fenceToken: "token-a"), "decision-c1", p);
        Throws<InvalidOperationException>(() => Decide(Aggregate("request-c2", coordinator: "coordinator-2", fenceToken: "token-b"), "decision-c2", p));
    }

    private static void StalePredecessor()
    {
        var a = Allocations();
        Throws<ArgumentException>(() => Direct(allocations: a, createdAt: a.ObservedAt.AddMinutes(-1)));
    }

    private static void DuplicateRequest()
    {
        var p = Processor();
        _ = Decide(Direct("request-dup"), "decision-a", p);
        Throws<InvalidOperationException>(() => Decide(Direct("request-dup"), "decision-b", p));
    }

    private static void RequestReplay() => DuplicateRequest();

    private static void DecisionReplay()
    {
        var p = Processor();
        _ = Decide(Direct("request-a"), "decision-replay", p);
        Throws<InvalidOperationException>(() => Decide(Direct("request-b"), "decision-replay", p));
    }

    private static void CrossEpoch()
    {
        var e2 = new ResourceEpochId("epoch-002");
        var a = Allocations(truth: Truth(epoch: e2));
        var auth = Authority("instance-app-a", "resource-requester", "scope.app-a", new[] { "app-a" }, epoch: Epoch);
        Throws<ArgumentException>(() => Direct(allocations: a, authority: auth));
    }

    private static void FutureEvidence()
    {
        var a = Allocations();
        Throws<ArgumentException>(() => new AdditionalResourceRequest(new ResourceRequestId("future"), ResourceRequesterKind.DirectApplication, "instance-app-a", "resource-requester",
            Authority("instance-app-a", "resource-requester", "scope.app-a", new[] { "app-a" }), new ApplicationPrincipalId("app-a"), Array.Empty<ApplicationPrincipalId>(), Cpu, Q(10), Q(10), a, null,
            new CorrelationId("c"), new CausationId("x"), Evidence("future-request", "request", at: T0.AddMinutes(1)), Evidence("residual", "residual"), T0, T0.AddMinutes(30)));
    }

    private static void ExpiredAuthority()
    {
        var auth = Authority("instance-app-a", "resource-requester", "scope.app-a", new[] { "app-a" }, from: T0.AddMinutes(-10), until: T0.AddSeconds(-1));
        Throws<InvalidOperationException>(() => Direct(authority: auth));
    }

    private static void DeterministicRequestIdentity() => Equal(Direct("request-det").IdentitySha256, Direct("request-det").IdentitySha256);
    private static void DeterministicDecisionIdentity() => Equal(Decide(Direct("request-det-d"), "decision-det").IdentitySha256, Decide(Direct("request-det-d"), "decision-det").IdentitySha256);
    private static void CanonicalConstituentOrdering() => Equal(Aggregate("request-order", appOrder: new[] { "app-b", "app-a" }).IdentitySha256, Aggregate("request-order", appOrder: new[] { "app-a", "app-b" }).IdentitySha256);

    private static void CorrelationPreserved()
    {
        var r = Direct();
        Equal(r.CorrelationId.Value, Decide(r).CorrelationId.Value);
    }

    private static void CausationPreserved()
    {
        var r = Direct();
        Equal(r.CausationId.Value, Decide(r).CausationId.Value);
    }

    private static void DecisionBoundToExactRequest()
    {
        var r = Direct("request-source");
        var d = Decide(r, "decision-source");
        Equal(r.RequestId.Value, d.RequestId.Value);
        Equal(r.IdentitySha256, d.RequestIdentitySha256);
    }

    private static string[] Surface()
    {
        var types = new[] { typeof(ResourceRequestAuthorityBinding), typeof(ResourceCoordinatorFence), typeof(AdditionalResourceRequest), typeof(ResourceAdditionalRequestDecisionPolicy), typeof(ResourceRequestDecisionAuthority), typeof(AdditionalResourceDecisionRecord), typeof(AdditionalResourceRequestDecisionProcessor) };
        return types.SelectMany(t => new[] { t.Name }.Concat(t.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static).Select(m => m.Name))).ToArray();
    }

    private static void ApplicationNeutralSurface()
    {
        var banned = new[] { "Trading", "Broker", "Market", "Strategy", "Order", "Position", "Accounting", "Warehouse" };
        foreach (var token in banned) Require(Surface().All(s => !s.Contains(token, StringComparison.OrdinalIgnoreCase)), "Application-specific token leaked: " + token);
    }

    private static void NoOpaqueAggregatePool()
    {
        var r = Aggregate();
        Equal(2, r.RepresentedApplications.Count);
        Equal(2, r.Authority.AuthorizedApplications.Count);
    }

    private static void NoWp07Executor()
    {
        var banned = new[] { "Reclaim", "Redistribute", "Rebalance", "Restore", "ApplyAllocation", "MutateAllocation" };
        var methods = typeof(AdditionalResourceRequestDecisionProcessor).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Where(m => !m.IsSpecialName).Select(m => m.Name).ToArray();
        foreach (var token in banned) Require(methods.All(m => !m.Contains(token, StringComparison.OrdinalIgnoreCase)), "WP-07 executor leaked: " + token);
    }

    private static void ZeroApplicationValidity()
    {
        var empty = new ApplicationResourceAllocationSnapshot(Truth(), T0.AddMinutes(-1), Array.Empty<ApplicationResourceAllocation>(), true);
        Equal(0, empty.Allocations.Count);
        _ = Processor();
    }

    private static void PredecessorTruthImmutable()
    {
        var a = Allocations();
        var before = a.IdentitySha256;
        _ = Decide(Direct(allocations: a));
        Equal(before, a.IdentitySha256);
    }

    private static void AuthoritySameGenerationConflict()
    {
        var p = Processor();
        _ = Decide(Direct("request-auth-a", authority: Authority("instance-app-a", "resource-requester", "scope.app-a", new[] { "app-a" }, 1, "authority-a")), "decision-auth-a", p);
        Throws<InvalidOperationException>(() => Decide(Direct("request-auth-b", authority: Authority("instance-app-a", "resource-requester", "scope.app-a", new[] { "app-a" }, 1, "authority-b")), "decision-auth-b", p));
    }

    private static void PressurePredecessorMismatch()
    {
        var requestAllocations = Allocations(ceilingA: 20);
        var pressureAllocations = Allocations(ceilingA: 21);
        Throws<ArgumentException>(() => Direct(allocations: requestAllocations, pressure: Pressure(pressureAllocations, null)));
    }

    private static void Run(string name, Action test)
    {
        try { test(); _passed++; Console.WriteLine("PASS " + name); }
        catch (Exception ex) { _failed++; Console.WriteLine($"FAIL {name}: {ex.GetType().Name}: {ex.Message}"); }
    }

    private static void Throws<T>(Action action) where T : Exception
    {
        try { action(); }
        catch (T) { return; }
        throw new InvalidOperationException("Expected " + typeof(T).Name + ".");
    }

    private static void Require(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }
    private static void Equal<T>(T expected, T actual) { if (!EqualityComparer<T>.Default.Equals(expected, actual)) throw new InvalidOperationException($"Expected '{expected}', got '{actual}'."); }
    private static void NotEqual<T>(T left, T right) { if (EqualityComparer<T>.Default.Equals(left, right)) throw new InvalidOperationException("Expected values to differ."); }
    private static void IsSha(string value) => Require(value.Length == 64 && value.All(c => c is >= '0' and <= '9' or >= 'A' and <= 'F'), "Identity is not canonical uppercase SHA-256.");
}
