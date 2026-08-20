using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Foundation.Contracts.ResourceGovernance;
using Foundation.State.ResourceGovernance;

namespace Falcon.Stage6.WP08.Verifier;

internal static class ProgramV2
{
    private static int _passed;
    private static int _failed;
    private static readonly DateTimeOffset T0 = new(2026, 8, 10, 10, 0, 0, TimeSpan.Zero);
    private static readonly ResourceEpochId Epoch = new("epoch-001");
    private static readonly ResourceClassId Cpu = new("cpu");
    private static readonly ApplicationPrincipalId AppA = new("app-a");
    private static readonly ApplicationPrincipalId AppB = new("app-b");
    private static readonly ResourceGrantId GrantA = new("grant-a");
    private static readonly ResourceGrantId GrantB = new("grant-b");

    private static int Main()
    {
        Run("zero_application_projection_valid", ZeroApplicationProjectionValid);
        Run("direct_projection_binds_authoritative_truth", DirectProjectionBindsAuthoritativeTruth);
        Run("missing_effective_truth_fails_closed", MissingEffectiveTruthFailsClosed);
        Run("quiescent_effective_truth_projects_native_capacity", QuiescentEffectiveTruthProjectsNativeCapacity);
        Run("borrowed_state_requires_exact_wp07_basis", BorrowedStateRequiresExactWp07Basis);
        Run("borrowed_state_preserves_source_provenance", BorrowedStatePreservesSourceProvenance);
        Run("direct_projection_is_exact_application_scoped", DirectProjectionIsExactApplicationScoped);
        Run("aggregate_projection_preserves_constituents", AggregateProjectionPreservesConstituents);
        Run("aggregate_projection_rejects_constituent_mismatch", AggregateProjectionRejectsConstituentMismatch);
        Run("critical_pressure_is_advisory_only", CriticalPressureIsAdvisoryOnly);
        Run("enforcement_observation_does_not_mint_compliance", EnforcementObservationDoesNotMintCompliance);
        Run("unavailable_pressure_returns_state_unavailable", UnavailablePressureReturnsStateUnavailable);
        Run("normal_pressure_returns_no_action", NormalPressureReturnsNoAction);
        Run("effective_reduction_requires_exact_effect_batch", EffectiveReductionRequiresExactEffectBatch);
        Run("effective_reduction_yields_compliance", EffectiveReductionYieldsCompliance);
        Run("effective_return_is_not_reduction", EffectiveReturnIsNotReduction);
        Run("foundation_reduction_requires_exact_effect_batch", FoundationReductionRequiresExactEffectBatch);
        Run("foundation_reduction_yields_compliance", FoundationReductionYieldsCompliance);
        Run("compliance_target_equals_accepted_wp07_capacity", ComplianceTargetEqualsAcceptedWp07Capacity);
        Run("reduction_quantity_omitted_without_exact_use", ReductionQuantityOmittedWithoutExactUse);
        Run("reduction_quantity_derived_only_from_exact_use", ReductionQuantityDerivedOnlyFromExactUse);
        Run("exact_use_scope_mismatch_rejected", ExactUseScopeMismatchRejected);
        Run("pressure_does_not_reverse_engineer_exact_use", PressureDoesNotReverseEngineerExactUse);
        Run("wp06_exact_decision_reference_positive", Wp06ExactDecisionReferencePositive);
        Run("wp06_decision_not_applied_capacity", Wp06DecisionNotAppliedCapacity);
        Run("projection_identity_is_deterministic", ProjectionIdentityIsDeterministic);
        Run("no_application_business_terms", NoApplicationBusinessTerms);
        Run("no_load_shedding_executor", NoLoadSheddingExecutor);
        Run("projection_scope_not_runtime_authentication", ProjectionScopeNotRuntimeAuthentication);
        Run("no_wp09_surface", NoWp09Surface);

        Console.WriteLine();
        Console.WriteLine($"STAGE 6 WP-08 VERIFIER V2: {_passed}/{_passed + _failed} PASS");
        Console.WriteLine($"Failures: {_failed}");
        return _failed == 0 ? 0 : 1;
    }

    private static ResourceQuantity Q(decimal amount) => new(amount, "units");
    private static ResourceEffectiveLifetime Lifetime() => new(T0.AddHours(-2), null, true);
    private static ResourceEvidenceReference Evidence(string id, DateTimeOffset? at = null)
        => new(new ResourceEvidenceId(id), new ResourceScopeId("scope-" + id), at ?? T0.AddMinutes(-40), Epoch);

    private static FoundationResourceTruthSnapshot Truth()
        => new(Epoch, T0.AddMinutes(-30), new[] { new FoundationResourceClassTruth(Cpu, Q(100), Q(10), Q(10), Evidence("truth", T0.AddMinutes(-31))) }, true);

    private static ApplicationResourceAllocation Allocation(ApplicationPrincipalId app, ResourceGrantId grant, decimal allocation = 20, decimal quota = 30, decimal ceiling = 40)
        => new(grant, app, Cpu, Q(allocation), Q(quota), Q(ceiling), Lifetime(), Evidence("allocation-" + app.Value));

    private static ApplicationResourceAllocationSnapshot Allocations()
        => new(Truth(), T0.AddMinutes(-20), new[] { Allocation(AppA, GrantA), Allocation(AppB, GrantB) }, true);

    private static ResourcePriorityClassDefinition PClass(string id) => new(new ResourcePriorityClassId(id), Lifetime(), Evidence("pc-" + id));
    private static TechnicalCriticalityClassDefinition CClass(string id) => new(new TechnicalCriticalityClassId(id), Lifetime(), Evidence("cc-" + id));
    private static ResourcePriorityClassRelation PRel(string high, string low) => new(new ResourcePriorityClassId(high), new ResourcePriorityClassId(low), Lifetime(), Evidence("pr-" + high + "-" + low));
    private static TechnicalCriticalityClassRelation CRel(string high, string low) => new(new TechnicalCriticalityClassId(high), new TechnicalCriticalityClassId(low), Lifetime(), Evidence("cr-" + high + "-" + low));
    private static ApplicationResourcePriorityBinding AppBinding(ApplicationPrincipalId app, string priority) => new(app, new ResourcePriorityClassId(priority), Lifetime(), Evidence("ab-" + app.Value));
    private static TechnicalCriticalityBinding TechBinding(string scope) => new(new ResourceScopeId(scope), Cpu, new TechnicalCriticalityClassId("c-high"), Lifetime(), Evidence("tb-" + scope));

    private static ResourcePriorityGovernanceSnapshot PrioritySnapshot(ApplicationResourceAllocationSnapshot allocations)
        => new(allocations, T0.AddMinutes(-15), "priority-policy-v1", Lifetime(), Evidence("priority-policy"), "criticality-policy-v1", Lifetime(), Evidence("criticality-policy"),
            new[] { PClass("p-high"), PClass("p-low") }, new[] { CClass("c-high"), CClass("c-low") }, new[] { PRel("p-high", "p-low") }, new[] { CRel("c-high", "c-low") },
            new[] { AppBinding(AppA, "p-high"), AppBinding(AppB, "p-low") }, new[] { TechBinding("scope-a"), TechBinding("scope-b") }, true);

    private static ResourcePressureTransitionPolicy PressurePolicy()
        => new(Cpu, 6000, 8000, 9500, 500, "pressure-policy-v1", Lifetime(), Evidence("pressure-policy"));

    private static FoundationResourcePressureSnapshot Pressure(ApplicationResourceAllocationSnapshot allocations, decimal? usedA, ResourceEnforcementObservationState enforcement = ResourceEnforcementObservationState.None)
    {
        var observations = new[] { new ResourcePressureObservation(ResourcePressureScopeKind.ApplicationResource, new ResourceScopeId("scope-a"), Cpu, AppA, usedA.HasValue ? Q(usedA.Value) : null, 1, Evidence("pressure-a")) };
        var eligibility = new[] { new ResourcePreemptionEligibilityBinding(GrantA, AppA, Cpu, ResourceReclaimability.Reclaimable, Lifetime(), Evidence("reclaim-a")) };
        var enforcementItems = enforcement == ResourceEnforcementObservationState.None ? Array.Empty<ResourceEnforcementObservation>() : new[] { new ResourceEnforcementObservation(ResourcePressureScopeKind.ApplicationResource, new ResourceScopeId("scope-a"), Cpu, AppA, enforcement, Evidence("enforcement-a")) };
        return new FoundationResourcePressureSnapshot(PrioritySnapshot(allocations), T0.AddMinutes(-10), new[] { PressurePolicy() }, observations, eligibility, enforcementItems);
    }

    private static ResourcePreemptionEligibilityBinding Binding(ApplicationPrincipalId app, ResourceGrantId grant)
        => new(grant, app, Cpu, ResourceReclaimability.Reclaimable, Lifetime(), Evidence("binding-" + app.Value));

    private static ResourceCoordinationEnvelope Envelope(ApplicationResourceAllocationSnapshot? allocations = null)
    {
        var a = allocations ?? Allocations();
        return new ResourceCoordinationEnvelope("envelope-authority", new ResourceScopeId("scope-coordination"), "coordinator-1", "aggregate-resource-coordinator", 1, 1, "fence-1", a,
            new[] { new ResourceCoordinationEnvelopeMember(AppA, GrantA, Cpu, Q(10), Q(10), Q(20), Binding(AppA, GrantA)), new ResourceCoordinationEnvelopeMember(AppB, GrantB, Cpu, Q(10), Q(10), Q(20), Binding(AppB, GrantB)) },
            Evidence("envelope"), T0.AddMinutes(-18), T0.AddHours(1));
    }

    private static EffectiveResourceDistributionSnapshot Effective(ResourceCoordinationEnvelope envelope, IEnumerable<BorrowedEffectiveCapacitySegment>? segments = null, DateTimeOffset? at = null)
        => new(envelope.AllocationSnapshot, envelope, at ?? T0.AddMinutes(-5), segments ?? Array.Empty<BorrowedEffectiveCapacitySegment>());

    private static EffectiveDistributionMutationIntent BorrowIntent(ResourceCoordinationEnvelope envelope, string id = "borrow-1", decimal amount = 5)
        => new(id, EffectiveDistributionOperationKind.Borrow, AppA, GrantA, AppB, Cpu, Q(amount), null, envelope, envelope.CoordinatorInstanceId, envelope.CoordinatorRoleId, envelope.FenceGeneration, envelope.FencingToken,
            new CorrelationId("corr-" + id), new CausationId("cause-" + id), Evidence("intent-" + id), T0.AddMinutes(-4), T0.AddMinutes(30));

    private static EffectiveDistributionMutationIntent ReturnIntent(ResourceCoordinationEnvelope envelope, string segmentId, string id = "return-1", decimal amount = 5)
        => new(id, EffectiveDistributionOperationKind.ReturnBorrowed, AppA, GrantA, AppB, Cpu, Q(amount), segmentId, envelope, envelope.CoordinatorInstanceId, envelope.CoordinatorRoleId, envelope.FenceGeneration, envelope.FencingToken,
            new CorrelationId("corr-" + id), new CausationId("cause-" + id), Evidence("intent-" + id), T0.AddMinutes(-1), T0.AddMinutes(30));

    private static (EffectiveResourceDistributionSnapshot Before, AcceptedEffectiveDistributionMutation Accepted, ResourceEffectBatch Batch) BorrowTransition(ResourceMutationProcessor? processor = null)
    {
        var e = Envelope(); var before = Effective(e); var intent = BorrowIntent(e); var batch = new ResourceEffectBatch("batch-borrow", new[] { ResourceEffectOperation.ForEffective(intent) });
        var accepted = (processor ?? new ResourceMutationProcessor()).ApplyEffectiveRedistribution(before, "batch-borrow", new[] { intent }, new SuccessAdapter(), T0);
        return (before, accepted, batch);
    }

    private static FoundationResourceMutationAuthority MutationAuthority()
        => new("mutation-authority", new ResourceScopeId("scope-mutation"), new[] { AppA, AppB }, new[] { Cpu }, new[] { ResourceDecisionKind.Reduce, ResourceDecisionKind.Revoke, ResourceDecisionKind.Restore }, 1, Evidence("mutation-authority"), T0.AddHours(-1), T0.AddHours(1));

    private static (ApplicationResourceAllocationSnapshot Before, AcceptedFoundationAllocationMutation Accepted, ResourceEffectBatch Batch) FoundationReduction()
    {
        var current = Allocations();
        var intent = new FoundationAllocationMutationIntent("reduce-1", ResourceDecisionKind.Reduce, AppA, GrantA, Cpu, Q(10), Q(20), Q(30), MutationAuthority(), null, current.IdentitySha256,
            new CorrelationId("corr-reduce"), new CausationId("cause-reduce"), Evidence("reduce-intent"), T0.AddMinutes(-4), T0.AddMinutes(30));
        var batch = new ResourceEffectBatch("batch-reduce", new[] { ResourceEffectOperation.ForFoundation(intent) });
        var quiesced = Effective(Envelope(current), Array.Empty<BorrowedEffectiveCapacitySegment>(), T0.AddMinutes(-2));
        var accepted = new ResourceMutationProcessor().ApplyFoundationAllocationMutations(current, "batch-reduce", new[] { intent }, new SuccessAdapter(), T0, quiesced);
        return (current, accepted, batch);
    }

    private static ApplicationResourceStateProjection Projection(ApplicationResourceAllocationSnapshot allocations, ApplicationPrincipalId app, FoundationResourcePressureSnapshot? pressure = null,
        EffectiveResourceDistributionSnapshot? effective = null, AdditionalResourceDecisionRecord? decision = null, Wp07AcceptedCapacityBasis? basis = null, ExactApplicationResourceUseObservation? use = null, DateTimeOffset? at = null)
        => ApplicationResourceStateProjectionBuilder.CreateDirect(allocations, app, Cpu, at ?? T0, pressure, effective, decision, basis, use);

    private static AdditionalResourceDecisionRecord DirectDecision(ApplicationResourceAllocationSnapshot allocations)
    {
        var authority = new ResourceRequestAuthorityBinding("request-authority", "requester-app-a", "application-resource-requester", new ResourceScopeId("scope-app-a"), new[] { AppA }, 1, Evidence("request-authority"), T0.AddHours(-1), T0.AddHours(1));
        var request = new AdditionalResourceRequest(new ResourceRequestId("request-app-a"), ResourceRequesterKind.DirectApplication, "requester-app-a", "application-resource-requester", authority, AppA, null, Cpu, Q(5), Q(5), allocations, null,
            new CorrelationId("corr-request"), new CausationId("cause-request"), Evidence("request"), Evidence("residual"), T0.AddMinutes(-8), T0.AddMinutes(30));
        var policy = new ResourceAdditionalRequestDecisionPolicy(Cpu, Q(10), false, Evidence("decision-policy"), T0.AddHours(-1), T0.AddHours(1));
        var decisionAuthority = new ResourceRequestDecisionAuthority("decision-authority", Evidence("decision-authority"), T0.AddHours(-1), T0.AddHours(1));
        return new AdditionalResourceRequestDecisionProcessor(new[] { policy }, decisionAuthority).Evaluate(request, new ResourceDecisionId("decision-app-a"), T0.AddMinutes(-7));
    }

    private static void ZeroApplicationProjectionValid() => Equal(0, new ApplicationResourceStateProjectionSet(Epoch, T0, Array.Empty<ApplicationResourceStateProjection>()).Projections.Count);
    private static void DirectProjectionBindsAuthoritativeTruth() { var p = Projection(Allocations(), AppA); Equal(GrantA.Value, p.GrantId.Value); Equal(20m, p.Allocation.Amount); Equal(30m, p.Quota.Amount); Equal(40m, p.Ceiling.Amount); }
    private static void MissingEffectiveTruthFailsClosed() => Require(!Projection(Allocations(), AppA).EffectiveCapacityAvailable, "Missing current WP-07 effective truth must remain unavailable.");
    private static void QuiescentEffectiveTruthProjectsNativeCapacity() { var a = Allocations(); var p = Projection(a, AppA, effective: Effective(Envelope(a))); Require(p.EffectiveCapacityAvailable, "Quiescent effective truth should be available."); Equal(20m, p.EffectiveCapacity!.Amount); }
    private static void BorrowedStateRequiresExactWp07Basis() { var t = BorrowTransition(); Throws<InvalidOperationException>(() => Projection(t.Before.AuthoritativeAllocationSnapshot, AppA, effective: t.Accepted.AcceptedSnapshot)); }
    private static void BorrowedStatePreservesSourceProvenance() { var t = BorrowTransition(); var basis = Wp07AcceptedCapacityBasis.FromEffectiveDistributionMutation(t.Before, t.Accepted, t.Batch, AppA, Cpu); var p = Projection(t.Before.AuthoritativeAllocationSnapshot, AppA, effective: t.Accepted.AcceptedSnapshot, basis: basis); Equal(AppA.Value, p.BorrowedProvenance.Single().SourceApplicationId.Value); Equal(GrantA.Value, p.BorrowedProvenance.Single().SourceGrantId.Value); }
    private static void DirectProjectionIsExactApplicationScoped() { var a = Allocations(); var p = Projection(a, AppA, effective: Effective(Envelope(a))); var set = new ApplicationResourceStateProjectionSet(Epoch, T0, new[] { p }); Require(set.GetApplicationView(AppA).Count == 1 && set.GetApplicationView(AppB).Count == 0, "Application projection scope leaked."); }
    private static void AggregateProjectionPreservesConstituents() { var a = Allocations(); var e = Envelope(a); var effective = Effective(e); var aggregate = new AggregateResourceStateProjection(e, T0, new[] { Projection(a, AppB, effective: effective), Projection(a, AppA, effective: effective) }); Equal(2, aggregate.Constituents.Count); }
    private static void AggregateProjectionRejectsConstituentMismatch() { var a = Allocations(); var e = Envelope(a); Throws<InvalidOperationException>(() => new AggregateResourceStateProjection(e, T0, new[] { Projection(a, AppA, effective: Effective(e)) })); }
    private static void CriticalPressureIsAdvisoryOnly() { var a = Allocations(); var signal = ApplicationResourceLoadSheddingSignalFactory.Create(Projection(a, AppA, pressure: Pressure(a, 39), effective: Effective(Envelope(a))), T0); Equal(TechnicalLoadSheddingSignalClass.AdvisoryReduction, signal.SignalClass); Require(signal.AcceptedCapacityBasisIdentitySha256 is null, "Pressure minted authority basis."); }
    private static void EnforcementObservationDoesNotMintCompliance() { var a = Allocations(); var signal = ApplicationResourceLoadSheddingSignalFactory.Create(Projection(a, AppA, pressure: Pressure(a, 39, ResourceEnforcementObservationState.ReductionObserved), effective: Effective(Envelope(a))), T0); Equal(TechnicalLoadSheddingSignalClass.AdvisoryReduction, signal.SignalClass); }
    private static void UnavailablePressureReturnsStateUnavailable() { var a = Allocations(); Equal(TechnicalLoadSheddingSignalClass.StateUnavailable, ApplicationResourceLoadSheddingSignalFactory.Create(Projection(a, AppA, pressure: Pressure(a, null), effective: Effective(Envelope(a))), T0).SignalClass); }
    private static void NormalPressureReturnsNoAction() { var a = Allocations(); Equal(TechnicalLoadSheddingSignalClass.NoAction, ApplicationResourceLoadSheddingSignalFactory.Create(Projection(a, AppA, pressure: Pressure(a, 10), effective: Effective(Envelope(a))), T0).SignalClass); }
    private static void EffectiveReductionRequiresExactEffectBatch() { var t = BorrowTransition(); var wrong = new ResourceEffectBatch("wrong-batch", new[] { ResourceEffectOperation.ForEffective(BorrowIntent(t.Before.Envelope, "other-intent")) }); Throws<InvalidOperationException>(() => Wp07AcceptedCapacityBasis.FromEffectiveDistributionMutation(t.Before, t.Accepted, wrong, AppA, Cpu)); }
    private static void EffectiveReductionYieldsCompliance() { var t = BorrowTransition(); var basis = Wp07AcceptedCapacityBasis.FromEffectiveDistributionMutation(t.Before, t.Accepted, t.Batch, AppA, Cpu); Equal(TechnicalLoadSheddingSignalClass.ComplianceReductionRequired, ApplicationResourceLoadSheddingSignalFactory.Create(Projection(t.Before.AuthoritativeAllocationSnapshot, AppA, effective: t.Accepted.AcceptedSnapshot, basis: basis), T0).SignalClass); }
    private static void EffectiveReturnIsNotReduction() { var processor = new ResourceMutationProcessor(); var first = BorrowTransition(processor); var segment = first.Accepted.AcceptedSnapshot.BorrowedSegments.Single(); var intent = ReturnIntent(first.Accepted.AcceptedSnapshot.Envelope, segment.SegmentId); var batch = new ResourceEffectBatch("batch-return", new[] { ResourceEffectOperation.ForEffective(intent) }); var returned = processor.ApplyEffectiveRedistribution(first.Accepted.AcceptedSnapshot, "batch-return", new[] { intent }, new SuccessAdapter(), T0.AddMinutes(1)); var basis = Wp07AcceptedCapacityBasis.FromEffectiveDistributionMutation(first.Accepted.AcceptedSnapshot, returned, batch, AppA, Cpu); Require(!basis.IsReduction && basis.IsRestorationOrIncrease, "Return should restore source capacity."); }
    private static void FoundationReductionRequiresExactEffectBatch() { var t = FoundationReduction(); var wrongIntent = new FoundationAllocationMutationIntent("other-reduce", ResourceDecisionKind.Reduce, AppA, GrantA, Cpu, Q(10), Q(20), Q(30), MutationAuthority(), null, t.Before.IdentitySha256, new CorrelationId("corr-other"), new CausationId("cause-other"), Evidence("other-intent"), T0.AddMinutes(-4), T0.AddMinutes(30)); var wrong = new ResourceEffectBatch("wrong-foundation-batch", new[] { ResourceEffectOperation.ForFoundation(wrongIntent) }); Throws<InvalidOperationException>(() => Wp07AcceptedCapacityBasis.FromFoundationMutation(t.Before, t.Accepted, wrong, AppA, Cpu)); }
    private static void FoundationReductionYieldsCompliance() { var t = FoundationReduction(); var basis = Wp07AcceptedCapacityBasis.FromFoundationMutation(t.Before, t.Accepted, t.Batch, AppA, Cpu); Equal(TechnicalLoadSheddingSignalClass.ComplianceReductionRequired, ApplicationResourceLoadSheddingSignalFactory.Create(Projection(t.Accepted.AcceptedSnapshot, AppA, basis: basis, at: basis.AcceptedAt), basis.AcceptedAt).SignalClass); }
    private static void ComplianceTargetEqualsAcceptedWp07Capacity() { var t = FoundationReduction(); var basis = Wp07AcceptedCapacityBasis.FromFoundationMutation(t.Before, t.Accepted, t.Batch, AppA, Cpu); var signal = ApplicationResourceLoadSheddingSignalFactory.Create(Projection(t.Accepted.AcceptedSnapshot, AppA, basis: basis, at: basis.AcceptedAt), basis.AcceptedAt); Equal(basis.AcceptedCapacity.Amount, signal.CompliantCapacityTarget!.Amount); }
    private static void ReductionQuantityOmittedWithoutExactUse() { var t = BorrowTransition(); var basis = Wp07AcceptedCapacityBasis.FromEffectiveDistributionMutation(t.Before, t.Accepted, t.Batch, AppA, Cpu); Require(ApplicationResourceLoadSheddingSignalFactory.Create(Projection(t.Before.AuthoritativeAllocationSnapshot, AppA, effective: t.Accepted.AcceptedSnapshot, basis: basis), T0).RequiredReduction is null, "Reduction quantity fabricated."); }
    private static void ReductionQuantityDerivedOnlyFromExactUse() { var t = BorrowTransition(); var basis = Wp07AcceptedCapacityBasis.FromEffectiveDistributionMutation(t.Before, t.Accepted, t.Batch, AppA, Cpu); var use = new ExactApplicationResourceUseObservation(AppA, Cpu, Epoch, Q(19), Evidence("exact-use"), T0); Equal(4m, ApplicationResourceLoadSheddingSignalFactory.Create(Projection(t.Before.AuthoritativeAllocationSnapshot, AppA, effective: t.Accepted.AcceptedSnapshot, basis: basis, use: use), T0).RequiredReduction!.Amount); }
    private static void ExactUseScopeMismatchRejected() { var a = Allocations(); var wrong = new ExactApplicationResourceUseObservation(AppB, Cpu, Epoch, Q(10), Evidence("wrong-use"), T0); Throws<InvalidOperationException>(() => Projection(a, AppA, effective: Effective(Envelope(a)), use: wrong)); }
    private static void PressureDoesNotReverseEngineerExactUse() { var t = BorrowTransition(); var basis = Wp07AcceptedCapacityBasis.FromEffectiveDistributionMutation(t.Before, t.Accepted, t.Batch, AppA, Cpu); var p = Projection(t.Before.AuthoritativeAllocationSnapshot, AppA, pressure: Pressure(t.Before.AuthoritativeAllocationSnapshot, 19), effective: t.Accepted.AcceptedSnapshot, basis: basis); Require(ApplicationResourceLoadSheddingSignalFactory.Create(p, T0).RequiredReduction is null, "Utilization basis points were converted to fabricated exact use."); }
    private static void Wp06ExactDecisionReferencePositive() { var a = Allocations(); Require(Projection(a, AppA, effective: Effective(Envelope(a)), decision: DirectDecision(a)).DecisionReference is not null, "WP-06 exact decision reference missing."); }
    private static void Wp06DecisionNotAppliedCapacity() { var a = Allocations(); Equal(20m, Projection(a, AppA, effective: Effective(Envelope(a)), decision: DirectDecision(a)).EffectiveCapacity!.Amount); }
    private static void ProjectionIdentityIsDeterministic() { var a = Allocations(); var e = Effective(Envelope(a)); Equal(Projection(a, AppA, effective: e).IdentitySha256, Projection(a, AppA, effective: e).IdentitySha256); }

    private static readonly Type[] Wp08Types = { typeof(ResourceCapacityBasisLane), typeof(TechnicalLoadSheddingSignalClass), typeof(ExactApplicationResourceUseObservation), typeof(Wp07AcceptedCapacityBasis), typeof(Wp06DecisionProjectionReference), typeof(ApplicationResourceStateProjection), typeof(ApplicationResourceStateProjectionSet), typeof(AggregateResourceStateProjection), typeof(ApplicationResourceStateProjectionBuilder), typeof(ApplicationResourceLoadSheddingSignal), typeof(ApplicationResourceLoadSheddingSignalFactory) };
    private static void NoApplicationBusinessTerms() { var forbidden = new[] { "FSATS", "TARC", "FSARM", "Strategy", "Broker", "Market", "Trading" }; var names = Wp08Types.SelectMany(t => new[] { t.FullName ?? t.Name }.Concat(t.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly).Select(m => m.Name))).ToArray(); Require(forbidden.All(term => names.All(name => !name.Contains(term, StringComparison.OrdinalIgnoreCase))), "Application business term leaked into WP-08 surface."); }
    private static void NoLoadSheddingExecutor() => Require(Wp08Types.SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)).All(m => !m.Name.Contains("ExecuteShedding", StringComparison.OrdinalIgnoreCase) && !m.Name.Contains("StopWorkload", StringComparison.OrdinalIgnoreCase)), "Application-internal shedding executor leaked.");
    private static void ProjectionScopeNotRuntimeAuthentication() => Require(Wp08Types.SelectMany(t => t.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)).All(m => !m.Name.Contains("Authenticate", StringComparison.OrdinalIgnoreCase) && !m.Name.Contains("Session", StringComparison.OrdinalIgnoreCase) && !m.Name.Contains("AdmitApplication", StringComparison.OrdinalIgnoreCase)), "Runtime authentication/admission leaked.");
    private static void NoWp09Surface() => Require(Wp08Types.SelectMany(t => t.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly)).All(m => !m.Name.Contains("IntegrationHardening", StringComparison.OrdinalIgnoreCase) && !m.Name.Contains("WP09", StringComparison.OrdinalIgnoreCase)), "WP-09 surface leaked.");

    private sealed class SuccessAdapter : IResourceEffectAdapter
    {
        public ResourceEffectApplicationResult Apply(ResourceEffectBatch batch, DateTimeOffset appliedAt)
            => new(batch.IdentitySha256, true, false, batch.Operations.Select(x => x.OperationId), Evidence("effect", appliedAt), appliedAt);
    }

    private static void Run(string name, Action action) { try { action(); _passed++; Console.WriteLine($"PASS {name}"); } catch (Exception ex) { _failed++; Console.WriteLine($"FAIL {name}: {ex.GetType().Name}: {ex.Message}"); } }
    private static void Require(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }
    private static void Equal<T>(T expected, T actual) { if (!EqualityComparer<T>.Default.Equals(expected, actual)) throw new InvalidOperationException($"Expected '{expected}' but found '{actual}'."); }
    private static void Throws<T>(Action action) where T : Exception { try { action(); } catch (T) { return; } throw new InvalidOperationException($"Expected {typeof(T).Name} was not thrown."); }
}
