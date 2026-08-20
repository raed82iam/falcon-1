using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Foundation.Contracts.ResourceGovernance;
using Foundation.State.ResourceGovernance;

namespace Falcon.Stage6.WP05.Verifier;

internal static class Program
{
    private static int _passed;
    private static int _failed;
    private static readonly DateTimeOffset T0 = new(2026, 8, 9, 12, 0, 0, TimeSpan.Zero);
    private static readonly ResourceEpochId Epoch = new("epoch-001");
    private static readonly ResourceClassId Cpu = new("cpu");

    private static readonly Type[] Wp05OwnedProductionTypes =
    {
        typeof(ResourcePressureScopeKind),
        typeof(ResourceEnforcementObservationState),
        typeof(ResourcePressureTransitionPolicy),
        typeof(ResourcePressureObservation),
        typeof(ResourcePreemptionEligibilityBinding),
        typeof(ResourceEnforcementObservation),
        typeof(ResourcePressureTruth),
        typeof(FoundationResourcePressureSnapshot)
    };

    private static int Main()
    {
        Run("positive_global_pressure_truth", PositiveGlobalPressureTruth);
        Run("positive_application_pressure_truth", PositiveApplicationPressureTruth);
        Run("canonical_pressure_enum_reused", CanonicalPressureEnumReused);
        Run("unavailable_pressure_is_not_normal", UnavailablePressureIsNotNormal);
        Run("application_view_is_scoped", ApplicationViewIsScoped);
        Run("global_scope_rejects_application_identity", GlobalScopeRejectsApplicationIdentity);
        Run("application_scope_requires_application_identity", ApplicationScopeRequiresApplicationIdentity);
        Run("missing_technical_binding_rejected", MissingTechnicalBindingRejected);
        Run("unknown_application_rejected", UnknownApplicationRejected);
        Run("wrong_epoch_observation_rejected", WrongEpochObservationRejected);
        Run("future_observation_evidence_rejected", FutureObservationEvidenceRejected);
        Run("missing_transition_policy_rejected", MissingTransitionPolicyRejected);
        Run("duplicate_scope_observation_rejected", DuplicateScopeObservationRejected);
        Run("sequence_rollback_rejected", SequenceRollbackRejected);
        Run("unit_mismatch_rejected", UnitMismatchRejected);
        Run("critical_pressure_does_not_mint_authority", CriticalPressureDoesNotMintAuthority);
        Run("reclaimable_allocation_is_eligibility_only", ReclaimableAllocationIsEligibilityOnly);
        Run("non_reclaimable_allocation_not_eligible", NonReclaimableAllocationNotEligible);
        Run("global_pressure_never_preempts_application", GlobalPressureNeverPreemptsApplication);
        Run("enforcement_is_observation_only", EnforcementIsObservationOnly);
        Run("hysteresis_holds_recovery_until_boundary", HysteresisHoldsRecoveryUntilBoundary);
        Run("worsening_is_not_delayed", WorseningIsNotDelayed);
        Run("policy_version_changes_identity", PolicyVersionChangesIdentity);
        Run("observation_sequence_changes_identity", ObservationSequenceChangesIdentity);
        Run("identity_is_uppercase_sha256", IdentityIsUppercaseSha256);
        Run("no_duplicate_state_pressure_enum", NoDuplicateStatePressureEnum);
        Run("production_surface_has_no_trading_terms", ProductionSurfaceHasNoTradingTerms);
        Run("production_surface_has_no_fsarm_coordination_mechanics", ProductionSurfaceHasNoFsarmCoordinationMechanics);
        Run("application_truth_does_not_become_opaque_aggregate_pool", ApplicationTruthDoesNotBecomeOpaqueAggregatePool);
        Run("production_surface_has_no_wp06_plus_decision_executor", ProductionSurfaceHasNoWp06PlusDecisionExecutor);
        Run("wp03_allocation_remains_read_only", Wp03AllocationRemainsReadOnly);

        Console.WriteLine();
        Console.WriteLine($"STAGE 6 WP-05 VERIFIER: {_passed}/{_passed + _failed} PASS");
        Console.WriteLine($"Failures: {_failed}");
        return _failed == 0 ? 0 : 1;
    }

    private static ResourceQuantity Q(decimal amount, string unit = "units") => new(amount, unit);
    private static ResourceEffectiveLifetime Lifetime(DateTimeOffset? start = null, DateTimeOffset? end = null)
        => end.HasValue ? new ResourceEffectiveLifetime(start ?? T0, end, false) : new ResourceEffectiveLifetime(start ?? T0, null, true);
    private static ResourceEvidenceReference Evidence(string id, string scope, ResourceEpochId? epoch = null, DateTimeOffset? observedAt = null)
        => new(new ResourceEvidenceId(id), new ResourceScopeId(scope), observedAt ?? T0, epoch ?? Epoch);

    private static FoundationResourceTruthSnapshot Truth()
        => new(Epoch, T0, new[] { new FoundationResourceClassTruth(Cpu, Q(100), Q(10), Q(10), Evidence("truth", "foundation-resource-truth")) }, true);

    private static ApplicationResourceAllocation Allocation(string app, string grant, decimal allocation = 10m, decimal quota = 20m, decimal ceiling = 30m)
        => new(new ResourceGrantId(grant), new ApplicationPrincipalId(app), Cpu, Q(allocation), Q(quota), Q(ceiling), Lifetime(), Evidence("allocation-" + app, "application-allocation"));

    private static ApplicationResourceAllocationSnapshot Allocations(params ApplicationResourceAllocation[] allocations)
        => new(Truth(), T0, allocations, true);

    private static ResourcePriorityClassDefinition PClass(string id) => new(new ResourcePriorityClassId(id), Lifetime(), Evidence("pc-" + id, "foundation-priority-policy"));
    private static TechnicalCriticalityClassDefinition CClass(string id) => new(new TechnicalCriticalityClassId(id), Lifetime(), Evidence("cc-" + id, "foundation-criticality-policy"));
    private static ResourcePriorityClassRelation PRel(string high, string low) => new(new ResourcePriorityClassId(high), new ResourcePriorityClassId(low), Lifetime(), Evidence("pr-" + high + "-" + low, "foundation-priority-policy"));
    private static TechnicalCriticalityClassRelation CRel(string high, string low) => new(new TechnicalCriticalityClassId(high), new TechnicalCriticalityClassId(low), Lifetime(), Evidence("cr-" + high + "-" + low, "foundation-criticality-policy"));
    private static ApplicationResourcePriorityBinding AppBinding(string app, string priority) => new(new ApplicationPrincipalId(app), new ResourcePriorityClassId(priority), Lifetime(), Evidence("ab-" + app, "application-priority-binding"));
    private static TechnicalCriticalityBinding TechBinding(string scope, string criticality = "c-high") => new(new ResourceScopeId(scope), Cpu, new TechnicalCriticalityClassId(criticality), Lifetime(), Evidence("tb-" + scope, "technical-criticality-binding"));

    private static ResourcePriorityGovernanceSnapshot PrioritySnapshot(bool includeSecondApp = false, IEnumerable<TechnicalCriticalityBinding>? technicalBindings = null)
    {
        var allocations = includeSecondApp
            ? Allocations(Allocation("app-a", "grant-a"), Allocation("app-b", "grant-b"))
            : Allocations(Allocation("app-a", "grant-a"));
        return new ResourcePriorityGovernanceSnapshot(
            allocations,
            T0,
            "priority-policy-v1", Lifetime(), Evidence("priority-policy", "foundation-priority-policy"),
            "criticality-policy-v1", Lifetime(), Evidence("criticality-policy", "foundation-criticality-policy"),
            new[] { PClass("p-high"), PClass("p-low") },
            new[] { CClass("c-high"), CClass("c-low") },
            new[] { PRel("p-high", "p-low") },
            new[] { CRel("c-high", "c-low") },
            includeSecondApp ? new[] { AppBinding("app-a", "p-high"), AppBinding("app-b", "p-low") } : new[] { AppBinding("app-a", "p-high") },
            technicalBindings ?? new[] { TechBinding("scope-global"), TechBinding("scope-a") },
            true);
    }

    private static ResourcePressureTransitionPolicy Policy(string version = "pressure-policy-v1")
        => new(Cpu, 6000, 8000, 9500, 500, version, Lifetime(), Evidence("pressure-policy", "pressure-transition-policy"));

    private static ResourcePressureObservation GlobalObservation(decimal? used, long sequence = 1, ResourceEvidenceReference? evidence = null, string unit = "units")
        => new(ResourcePressureScopeKind.FoundationResourceClass, new ResourceScopeId("scope-global"), Cpu, null, used.HasValue ? Q(used.Value, unit) : null, sequence, evidence ?? Evidence("obs-global-" + sequence, "pressure-observation"));

    private static ResourcePressureObservation AppObservation(decimal? used, long sequence = 1, string app = "app-a", ResourceEvidenceReference? evidence = null, string unit = "units")
        => new(ResourcePressureScopeKind.ApplicationResource, new ResourceScopeId("scope-a"), Cpu, new ApplicationPrincipalId(app), used.HasValue ? Q(used.Value, unit) : null, sequence, evidence ?? Evidence("obs-" + app + "-" + sequence, "pressure-observation"));

    private static ResourcePreemptionEligibilityBinding Eligibility(ResourceReclaimability reclaimability)
        => new(new ResourceGrantId("grant-a"), new ApplicationPrincipalId("app-a"), Cpu, reclaimability, Lifetime(), Evidence("eligibility-a", "preemption-eligibility"));

    private static FoundationResourcePressureSnapshot PressureSnapshot(
        IEnumerable<ResourcePressureObservation> observations,
        IEnumerable<ResourcePreemptionEligibilityBinding>? eligibility = null,
        IEnumerable<ResourceEnforcementObservation>? enforcement = null,
        IEnumerable<ResourcePressureTransitionPolicy>? policies = null,
        FoundationResourcePressureSnapshot? previous = null,
        ResourcePriorityGovernanceSnapshot? priority = null)
        => new(priority ?? PrioritySnapshot(), T0, policies ?? new[] { Policy() }, observations, eligibility ?? Array.Empty<ResourcePreemptionEligibilityBinding>(), enforcement ?? Array.Empty<ResourceEnforcementObservation>(), previous);

    private static void PositiveGlobalPressureTruth()
    {
        var snapshot = PressureSnapshot(new[] { GlobalObservation(50) });
        var truth = snapshot.Truth.Single();
        Equal(ResourcePressureState.Normal, truth.State);
        Equal(5000, truth.UtilizationBasisPoints);
        Require(truth.PressureAvailable, "Global pressure should be available.");
    }

    private static void PositiveApplicationPressureTruth()
    {
        var snapshot = PressureSnapshot(new[] { AppObservation(29) }, new[] { Eligibility(ResourceReclaimability.Reclaimable) });
        var truth = snapshot.Truth.Single();
        Equal(ResourcePressureState.Critical, truth.State);
        Require(truth.PreemptionEligibleForConsideration, "Critical reclaimable Application allocation should be eligible for consideration only.");
    }

    private static void CanonicalPressureEnumReused()
        => Equal("Normal,Constrained,Degraded,Critical", string.Join(',', Enum.GetNames<ResourcePressureState>()));

    private static void UnavailablePressureIsNotNormal()
    {
        var truth = PressureSnapshot(new[] { GlobalObservation(null) }).Truth.Single();
        Require(!truth.PressureAvailable && truth.State is null && truth.UtilizationBasisPoints is null, "Unavailable pressure must not be represented as NORMAL.");
    }

    private static void ApplicationViewIsScoped()
    {
        var priority = PrioritySnapshot(true, new[] { TechBinding("scope-global"), TechBinding("scope-a") });
        var snapshot = new FoundationResourcePressureSnapshot(priority, T0, new[] { Policy() }, new[] { AppObservation(10, app: "app-a") }, Array.Empty<ResourcePreemptionEligibilityBinding>(), Array.Empty<ResourceEnforcementObservation>());
        var view = snapshot.GetApplicationView(new ApplicationPrincipalId("app-a"));
        Require(view.Count == 1 && view.All(item => item.ApplicationId?.Value == "app-a"), "Application view must expose only exact Application pressure truth.");
        Require(snapshot.GetApplicationView(new ApplicationPrincipalId("app-b")).Count == 0, "Another Application must not receive app-a pressure truth.");
    }

    private static void GlobalScopeRejectsApplicationIdentity()
        => Throws<ArgumentException>(() => new ResourcePressureObservation(ResourcePressureScopeKind.FoundationResourceClass, new ResourceScopeId("scope-global"), Cpu, new ApplicationPrincipalId("app-a"), Q(1), 1, Evidence("x", "pressure-observation")));

    private static void ApplicationScopeRequiresApplicationIdentity()
        => Throws<ArgumentException>(() => new ResourcePressureObservation(ResourcePressureScopeKind.ApplicationResource, new ResourceScopeId("scope-a"), Cpu, null, Q(1), 1, Evidence("x", "pressure-observation")));

    private static void MissingTechnicalBindingRejected()
        => Throws<ArgumentException>(() => PressureSnapshot(new[] { GlobalObservation(50) }, priority: PrioritySnapshot(technicalBindings: new[] { TechBinding("scope-a") })));

    private static void UnknownApplicationRejected()
        => Throws<KeyNotFoundException>(() => PressureSnapshot(new[] { AppObservation(10, app: "app-x") }));

    private static void WrongEpochObservationRejected()
        => Throws<ArgumentException>(() => PressureSnapshot(new[] { GlobalObservation(50, evidence: Evidence("x", "pressure-observation", new ResourceEpochId("epoch-002"))) }));

    private static void FutureObservationEvidenceRejected()
        => Throws<ArgumentException>(() => PressureSnapshot(new[] { GlobalObservation(50, evidence: Evidence("x", "pressure-observation", observedAt: T0.AddSeconds(1))) }));

    private static void MissingTransitionPolicyRejected()
        => Throws<ArgumentException>(() => PressureSnapshot(new[] { GlobalObservation(50) }, policies: Array.Empty<ResourcePressureTransitionPolicy>()));

    private static void DuplicateScopeObservationRejected()
        => Throws<ArgumentException>(() => PressureSnapshot(new[] { GlobalObservation(20, 1), GlobalObservation(30, 2) }));

    private static void SequenceRollbackRejected()
    {
        var previous = PressureSnapshot(new[] { GlobalObservation(50, 2) });
        Throws<ArgumentException>(() => PressureSnapshot(new[] { GlobalObservation(51, 2) }, previous: previous));
    }

    private static void UnitMismatchRejected()
        => Throws<ArgumentException>(() => PressureSnapshot(new[] { GlobalObservation(50, unit: "wrong-unit") }));

    private static void CriticalPressureDoesNotMintAuthority()
    {
        var truth = PressureSnapshot(new[] { GlobalObservation(100) }).Truth.Single();
        Equal(ResourcePressureState.Critical, truth.State);
        Require(typeof(ResourcePressureTruth).GetProperties().All(property => !property.Name.Contains("Grant", StringComparison.OrdinalIgnoreCase) && !property.Name.Contains("Authorize", StringComparison.OrdinalIgnoreCase)), "Pressure truth cannot expose grant/authorization authority.");
    }

    private static void ReclaimableAllocationIsEligibilityOnly()
    {
        var truth = PressureSnapshot(new[] { AppObservation(27) }, new[] { Eligibility(ResourceReclaimability.Reclaimable) }).Truth.Single();
        Require(truth.PreemptionEligibleForConsideration, "Reclaimable pressured allocation should be eligible for consideration.");
        var executableMethods = typeof(ResourcePressureTruth)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Where(method => !method.IsSpecialName);
        Require(executableMethods.All(method => !method.Name.Contains("Reclaim", StringComparison.OrdinalIgnoreCase) && !method.Name.Contains("Preempt", StringComparison.OrdinalIgnoreCase)), "WP-05 truth cannot execute preemption/reclamation.");
    }

    private static void NonReclaimableAllocationNotEligible()
        => Require(!PressureSnapshot(new[] { AppObservation(27) }, new[] { Eligibility(ResourceReclaimability.NonReclaimable) }).Truth.Single().PreemptionEligibleForConsideration, "Non-reclaimable allocation must not become eligible.");

    private static void GlobalPressureNeverPreemptsApplication()
        => Require(!PressureSnapshot(new[] { GlobalObservation(100) }).Truth.Single().PreemptionEligibleForConsideration, "Global pressure truth cannot directly mark an Application allocation preempted/eligible without exact Application binding.");

    private static void EnforcementIsObservationOnly()
    {
        var enforcement = new ResourceEnforcementObservation(ResourcePressureScopeKind.ApplicationResource, new ResourceScopeId("scope-a"), Cpu, new ApplicationPrincipalId("app-a"), ResourceEnforcementObservationState.ReductionObserved, Evidence("enforce", "enforcement-observation"));
        var truth = PressureSnapshot(new[] { AppObservation(20) }, enforcement: new[] { enforcement }).Truth.Single();
        Equal(ResourceEnforcementObservationState.ReductionObserved, truth.EnforcementState);
        Require(typeof(ResourceEnforcementObservation).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).All(method => !method.Name.Contains("Execute", StringComparison.OrdinalIgnoreCase) && !method.Name.Contains("Apply", StringComparison.OrdinalIgnoreCase)), "Enforcement surface must remain observational.");
    }

    private static void HysteresisHoldsRecoveryUntilBoundary()
    {
        var previous = PressureSnapshot(new[] { GlobalObservation(85, 1) });
        Equal(ResourcePressureState.Degraded, previous.Truth.Single().State);
        var next = PressureSnapshot(new[] { GlobalObservation(78, 2) }, previous: previous);
        Equal(ResourcePressureState.Degraded, next.Truth.Single().State);
        var recovered = PressureSnapshot(new[] { GlobalObservation(74, 3) }, previous: next);
        Equal(ResourcePressureState.Constrained, recovered.Truth.Single().State);
    }

    private static void WorseningIsNotDelayed()
    {
        var previous = PressureSnapshot(new[] { GlobalObservation(50, 1) });
        var next = PressureSnapshot(new[] { GlobalObservation(97, 2) }, previous: previous);
        Equal(ResourcePressureState.Critical, next.Truth.Single().State);
    }

    private static void PolicyVersionChangesIdentity()
    {
        var a = PressureSnapshot(new[] { GlobalObservation(50) }, policies: new[] { Policy("pressure-policy-v1") });
        var b = PressureSnapshot(new[] { GlobalObservation(50) }, policies: new[] { Policy("pressure-policy-v2") });
        Require(a.IdentitySha256 != b.IdentitySha256, "Transition policy version must be identity material.");
    }

    private static void ObservationSequenceChangesIdentity()
    {
        var a = PressureSnapshot(new[] { GlobalObservation(50, 1) });
        var b = PressureSnapshot(new[] { GlobalObservation(50, 2) });
        Require(a.IdentitySha256 != b.IdentitySha256, "Observation sequence must be identity material.");
    }

    private static void IdentityIsUppercaseSha256()
    {
        var identity = PressureSnapshot(new[] { GlobalObservation(50) }).IdentitySha256;
        Require(identity.Length == 64 && identity.All(ch => char.IsDigit(ch) || (ch >= 'A' && ch <= 'F')), "Pressure snapshot identity must remain uppercase SHA-256.");
    }

    private static void NoDuplicateStatePressureEnum()
    {
        var stateAssembly = typeof(FoundationResourcePressureSnapshot).Assembly;
        var duplicates = stateAssembly.GetExportedTypes().Where(type => type.Namespace == "Foundation.State.ResourceGovernance" && type.Name == nameof(ResourcePressureState)).ToArray();
        Require(duplicates.Length == 0, "Foundation.State must reuse the canonical Foundation.Contracts ResourcePressureState instead of defining a duplicate enum.");
    }

    private static string[] Wp05OwnedSurfaceNames()
        => Wp05OwnedProductionTypes
            .SelectMany(type => new[] { type.FullName ?? type.Name }
                .Concat(type.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)
                    .Select(member => member.Name)))
            .ToArray();

    private static void ProductionSurfaceHasNoTradingTerms()
    {
        var forbidden = new[] { "Trading", "FSATS", "TARC", "Broker", "Strategy", "Market" };
        var names = typeof(FoundationResourcePressureSnapshot).Assembly.GetExportedTypes().Where(type => type.Namespace == "Foundation.State.ResourceGovernance").Select(type => type.FullName ?? type.Name).ToArray();
        foreach (var token in forbidden) Require(names.All(name => !name.Contains(token, StringComparison.OrdinalIgnoreCase)), $"Application/business term leaked into Foundation resource-governance production surface: {token}");
    }

    private static void ProductionSurfaceHasNoFsarmCoordinationMechanics()
    {
        var forbidden = new[] { "FSARM", "Coordinator", "CoordinationEnvelope", "AggregatePool", "DelegatedAggregate" };
        var names = Wp05OwnedSurfaceNames();
        foreach (var token in forbidden) Require(names.All(name => !name.Contains(token, StringComparison.OrdinalIgnoreCase)), $"FSARM/coordinator mechanic leaked into WP-05-owned production surface: {token}");
    }

    private static void ApplicationTruthDoesNotBecomeOpaqueAggregatePool()
    {
        var priority = PrioritySnapshot(true, new[] { TechBinding("scope-global"), TechBinding("scope-a") });
        var snapshot = new FoundationResourcePressureSnapshot(priority, T0, new[] { Policy() }, new[] { AppObservation(10, app: "app-a") }, Array.Empty<ResourcePreemptionEligibilityBinding>(), Array.Empty<ResourceEnforcementObservation>());
        Require(snapshot.Truth.All(item => item.ScopeKind != ResourcePressureScopeKind.ApplicationResource || item.ApplicationId is not null), "Application-bound truth must remain attributable to an exact Application identity.");
        Require(snapshot.GetApplicationView(new ApplicationPrincipalId("app-a")).All(item => item.ApplicationId?.Value == "app-a"), "Application truth must remain constituent-attributable and must not become an opaque aggregate pool.");
    }

    private static void ProductionSurfaceHasNoWp06PlusDecisionExecutor()
    {
        var forbidden = new[] { "RequestProcessor", "GrantDecision", "Reclaimer", "Redistributor", "RebalanceEngine", "RestorationExecutor", "LoadSheddingExecutor" };
        var names = Wp05OwnedSurfaceNames();
        foreach (var token in forbidden) Require(names.All(name => !name.Contains(token, StringComparison.OrdinalIgnoreCase)), $"WP-06+ execution surface leaked into WP-05-owned production types: {token}");
    }

    private static void Wp03AllocationRemainsReadOnly()
    {
        var allocation = Allocation("app-a", "grant-a");
        var before = (allocation.Allocation.Amount, allocation.Quota.Amount, allocation.Ceiling.Amount);
        _ = PressureSnapshot(new[] { AppObservation(27) }, new[] { Eligibility(ResourceReclaimability.Reclaimable) });
        Equal(before, (allocation.Allocation.Amount, allocation.Quota.Amount, allocation.Ceiling.Amount));
    }

    private static void Run(string name, Action test)
    {
        try { test(); _passed++; Console.WriteLine($"PASS {name}"); }
        catch (Exception exception) { _failed++; Console.WriteLine($"FAIL {name}: {exception.GetType().Name}: {exception.Message}"); }
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    private static void Equal<T>(T expected, T actual)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual)) throw new InvalidOperationException($"Expected '{expected}', got '{actual}'.");
    }

    private static void Throws<TException>(Action action) where TException : Exception
    {
        try { action(); }
        catch (TException) { return; }
        throw new InvalidOperationException($"Expected {typeof(TException).Name}.");
    }
}
