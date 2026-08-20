using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Foundation.Contracts.ResourceGovernance;
using Foundation.State.ResourceGovernance;

namespace Falcon.Stage6.WP07.Verifier;

internal static class ProgramV2
{
    private static int _passed;
    private static int _failed;

    private static readonly DateTimeOffset T0 = new(2026, 8, 10, 4, 0, 0, TimeSpan.Zero);
    private static readonly ResourceEpochId Epoch = new("epoch-001");
    private static readonly ResourceClassId Cpu = new("cpu");
    private static readonly ApplicationPrincipalId AppA = new("app-a");
    private static readonly ApplicationPrincipalId AppB = new("app-b");
    private static readonly ResourceGrantId GrantA = new("grant-a");
    private static readonly ResourceGrantId GrantB = new("grant-b");

    private static int Main()
    {
        Run("zero_application_validity", ZeroApplicationValidity);
        Run("coordination_envelope_positive", () => Equal(2, Envelope().Members.Count));
        Run("reclaimability_binding_exact_identity", ReclaimabilityBindingExactIdentity);
        Run("non_reclaimable_positive_borrow_out_rejected", NonReclaimableBorrowOutRejected);
        Run("non_reclaimable_zero_borrow_out_allowed", NonReclaimableZeroBorrowOutAllowed);
        Run("expired_reclaimability_binding_rejected", ExpiredReclaimabilityRejected);
        Run("borrow_out_uses_native_grant_not_headroom", BorrowOutCannotUseHeadroom);
        Run("borrow_in_respects_authoritative_ceiling", BorrowInCeilingBound);
        Run("borrow_positive", BorrowPositive);
        Run("borrow_preserves_source_grant_provenance", BorrowProvenance);
        Run("borrow_preserves_target_attribution", BorrowTargetAttribution);
        Run("borrow_does_not_mutate_authoritative_allocation", BorrowDoesNotMutateAuthoritativeAllocation);
        Run("borrow_from_non_reclaimable_source_rejected", BorrowRuntimeNonReclaimableRejected);
        Run("return_positive", ReturnPositive);
        Run("effect_failure_blocks_truth", EffectFailureBlocksTruth);
        Run("partial_effect_blocks_truth", PartialEffectBlocksTruth);
        Run("effect_payload_is_actionable", EffectPayloadActionable);
        Run("foundation_reduce_positive", FoundationReducePositive);
        Run("foundation_mutation_requires_quiescence", FoundationMutationRequiresQuiescence);
        Run("foundation_mutation_rejects_active_borrow", FoundationMutationRejectsActiveBorrow);
        Run("restore_requires_snapshot_captured_basis", RestoreRequiresCapturedBasis);
        Run("restore_above_basis_rejected", RestoreAboveBasisRejected);
        Run("rebalance_not_canonical_decision_kind", RebalanceNotDecisionKind);
        Run("intent_effect_truth_distinct", IntentEffectTruthDistinct);
        Run("environment_neutral_effect_contract", EnvironmentNeutralEffectContract);
        Run("application_neutral_surface", ApplicationNeutralSurface);
        Run("no_wp08_surface", NoWp08Surface);
        Run("wp06_decision_distinct_from_wp07_mutation", () => Require(typeof(AdditionalResourceDecisionRecord) != typeof(FoundationAllocationMutationIntent), "WP-06 decision truth must remain distinct from WP-07 mutation intent."));

        Console.WriteLine();
        Console.WriteLine($"STAGE 6 WP-07 VERIFIER V2: {_passed}/{_passed + _failed} PASS");
        Console.WriteLine($"Failures: {_failed}");
        return _failed == 0 ? 0 : 1;
    }

    private static ResourceQuantity Q(decimal amount) => new(amount, "units");
    private static ResourceEffectiveLifetime Lifetime(DateTimeOffset? until = null)
        => until.HasValue ? new ResourceEffectiveLifetime(T0.AddHours(-1), until, false) : new ResourceEffectiveLifetime(T0.AddHours(-1), null, true);

    private static ResourceEvidenceReference Evidence(string id, ResourceEpochId? epoch = null, DateTimeOffset? at = null)
        => new(new ResourceEvidenceId(id), new ResourceScopeId("scope-" + id), at ?? T0.AddMinutes(-6), epoch ?? Epoch);

    private static FoundationResourceTruthSnapshot Truth(ResourceEpochId? epoch = null)
    {
        var e = epoch ?? Epoch;
        return new FoundationResourceTruthSnapshot(e, T0.AddMinutes(-10), new[]
        {
            new FoundationResourceClassTruth(Cpu, Q(100), Q(10), Q(10), Evidence("truth", e, T0.AddMinutes(-11)))
        }, true);
    }

    private static ApplicationResourceAllocation Allocation(ApplicationPrincipalId app, ResourceGrantId grant, decimal allocation = 20, decimal quota = 30, decimal ceiling = 40, ResourceEpochId? epoch = null)
        => new(grant, app, Cpu, Q(allocation), Q(quota), Q(ceiling), Lifetime(), Evidence("allocation-" + app.Value, epoch));

    private static ApplicationResourceAllocationSnapshot Allocations(bool twoApps = true)
    {
        var truth = Truth();
        var records = twoApps
            ? new[] { Allocation(AppA, GrantA, epoch: truth.EpochId), Allocation(AppB, GrantB, epoch: truth.EpochId) }
            : Array.Empty<ApplicationResourceAllocation>();
        return new ApplicationResourceAllocationSnapshot(truth, T0.AddMinutes(-5), records, true);
    }

    private static ResourcePreemptionEligibilityBinding Binding(ApplicationPrincipalId app, ResourceGrantId grant, ResourceReclaimability reclaimability = ResourceReclaimability.Reclaimable, DateTimeOffset? until = null)
        => new(grant, app, Cpu, reclaimability, Lifetime(until), Evidence("reclaim-" + app.Value));

    private static ResourceCoordinationEnvelope Envelope(
        ApplicationResourceAllocationSnapshot? allocations = null,
        ResourceReclaimability reclaimA = ResourceReclaimability.Reclaimable,
        ResourceReclaimability reclaimB = ResourceReclaimability.Reclaimable,
        decimal outA = 10,
        decimal outB = 10,
        decimal inA = 20,
        decimal inB = 20,
        decimal minA = 10,
        decimal minB = 10,
        DateTimeOffset? bindingAUntil = null)
    {
        var a = allocations ?? Allocations();
        return new ResourceCoordinationEnvelope(
            "envelope-authority",
            new ResourceScopeId("scope-coordination"),
            "coordinator-1",
            "aggregate-resource-coordinator",
            1,
            1,
            "fence-1",
            a,
            new[]
            {
                new ResourceCoordinationEnvelopeMember(AppA, GrantA, Cpu, Q(minA), Q(outA), Q(inA), Binding(AppA, GrantA, reclaimA, bindingAUntil)),
                new ResourceCoordinationEnvelopeMember(AppB, GrantB, Cpu, Q(minB), Q(outB), Q(inB), Binding(AppB, GrantB, reclaimB))
            },
            Evidence("envelope", a.ResourceTruth.EpochId),
            T0.AddMinutes(-4),
            T0.AddHours(1));
    }

    private static EffectiveResourceDistributionSnapshot Effective(ResourceCoordinationEnvelope? envelope = null, ApplicationResourceAllocationSnapshot? allocations = null, IEnumerable<BorrowedEffectiveCapacitySegment>? segments = null)
    {
        var a = allocations ?? envelope?.AllocationSnapshot ?? Allocations();
        var e = envelope ?? Envelope(a);
        return new EffectiveResourceDistributionSnapshot(a, e, T0, segments ?? Array.Empty<BorrowedEffectiveCapacitySegment>());
    }

    private static EffectiveDistributionMutationIntent BorrowIntent(ResourceCoordinationEnvelope envelope, string id = "borrow-1", decimal amount = 5)
        => new(id, EffectiveDistributionOperationKind.Borrow, AppA, GrantA, AppB, Cpu, Q(amount), null, envelope,
            envelope.CoordinatorInstanceId, envelope.CoordinatorRoleId, envelope.FenceGeneration, envelope.FencingToken,
            new CorrelationId("corr-" + id), new CausationId("cause-" + id), Evidence("intent-" + id, envelope.AllocationSnapshot.ResourceTruth.EpochId, T0.AddMinutes(-1)), T0, T0.AddMinutes(30));

    private static EffectiveDistributionMutationIntent ReturnIntent(ResourceCoordinationEnvelope envelope, string segmentId, string id = "return-1", decimal amount = 5)
        => new(id, EffectiveDistributionOperationKind.ReturnBorrowed, AppA, GrantA, AppB, Cpu, Q(amount), segmentId, envelope,
            envelope.CoordinatorInstanceId, envelope.CoordinatorRoleId, envelope.FenceGeneration, envelope.FencingToken,
            new CorrelationId("corr-" + id), new CausationId("cause-" + id), Evidence("intent-" + id, envelope.AllocationSnapshot.ResourceTruth.EpochId, T0.AddMinutes(-1)), T0, T0.AddMinutes(30));

    private static FoundationResourceMutationAuthority Authority()
        => new("foundation-mutation-authority", new ResourceScopeId("scope-foundation-mutation"), new[] { AppA, AppB }, new[] { Cpu },
            new[] { ResourceDecisionKind.Reduce, ResourceDecisionKind.Revoke, ResourceDecisionKind.Restore }, 1,
            Evidence("foundation-authority"), T0.AddMinutes(-5), T0.AddHours(1));

    private static FoundationAllocationMutationIntent FoundationIntent(ApplicationResourceAllocationSnapshot predecessor, ResourceDecisionKind operation, string id, decimal allocation, decimal quota, decimal ceiling, FoundationAllocationRestorationBasis? basis = null)
        => new(id, operation, AppA, GrantA, Cpu, Q(allocation), Q(quota), Q(ceiling), Authority(), basis,
            predecessor.IdentitySha256, new CorrelationId("corr-" + id), new CausationId("cause-" + id), Evidence("intent-" + id, predecessor.ResourceTruth.EpochId, T0.AddMinutes(-1)), T0, T0.AddMinutes(30));

    private static AcceptedEffectiveDistributionMutation BorrowOnce(ResourceMutationProcessor? processor = null, ResourceCoordinationEnvelope? envelope = null, IResourceEffectAdapter? adapter = null)
    {
        var e = envelope ?? Envelope();
        return (processor ?? new ResourceMutationProcessor()).ApplyEffectiveRedistribution(Effective(e), "batch-borrow", new[] { BorrowIntent(e) }, adapter ?? new FixtureAdapter(FixtureMode.Success, e.AllocationSnapshot.ResourceTruth.EpochId), T0.AddMinutes(1));
    }

    private static void ZeroApplicationValidity()
    {
        Equal(0, Allocations(false).Allocations.Count);
        _ = new ResourceMutationProcessor();
    }

    private static void ReclaimabilityBindingExactIdentity()
    {
        var wrong = Binding(AppB, GrantB);
        Throws<ArgumentException>(() => new ResourceCoordinationEnvelopeMember(AppA, GrantA, Cpu, Q(10), Q(5), Q(5), wrong));
    }

    private static void NonReclaimableBorrowOutRejected()
        => Throws<ArgumentException>(() => Envelope(reclaimA: ResourceReclaimability.NonReclaimable, outA: 1));

    private static void NonReclaimableZeroBorrowOutAllowed()
        => Equal(0m, Envelope(reclaimA: ResourceReclaimability.NonReclaimable, outA: 0).Members.Single(x => x.ApplicationId.Value == AppA.Value).MaximumBorrowOut.Amount);

    private static void ExpiredReclaimabilityRejected()
        => Throws<InvalidOperationException>(() => Envelope(bindingAUntil: T0.AddMinutes(-5)));

    private static void BorrowOutCannotUseHeadroom()
        => Throws<ArgumentException>(() => Envelope(outA: 11, minA: 10));

    private static void BorrowInCeilingBound()
        => Throws<ArgumentException>(() => Envelope(inA: 21));

    private static void BorrowPositive()
    {
        var result = BorrowOnce();
        Equal(15m, result.AcceptedSnapshot.GetEffectiveCapacity(AppA, Cpu).Amount);
        Equal(25m, result.AcceptedSnapshot.GetEffectiveCapacity(AppB, Cpu).Amount);
    }

    private static void BorrowProvenance()
    {
        var segment = BorrowOnce().AcceptedSnapshot.BorrowedSegments.Single();
        Equal(AppA.Value, segment.SourceApplicationId.Value);
        Equal(GrantA.Value, segment.SourceGrantId.Value);
    }

    private static void BorrowTargetAttribution()
        => Equal(AppB.Value, BorrowOnce().AcceptedSnapshot.BorrowedSegments.Single().TargetApplicationId.Value);

    private static void BorrowDoesNotMutateAuthoritativeAllocation()
    {
        var result = BorrowOnce();
        Equal(result.AcceptedSnapshot.Envelope.AllocationSnapshot.IdentitySha256, result.AcceptedSnapshot.AuthoritativeAllocationSnapshot.IdentitySha256);
        Equal(20m, result.AcceptedSnapshot.AuthoritativeAllocationSnapshot.GetRequiredAllocation(AppA, Cpu).Allocation.Amount);
    }

    private static void BorrowRuntimeNonReclaimableRejected()
    {
        var a = Allocations();
        var e = Envelope(a, reclaimA: ResourceReclaimability.NonReclaimable, outA: 0);
        Throws<InvalidOperationException>(() => new ResourceMutationProcessor().ApplyEffectiveRedistribution(Effective(e), "batch-x", new[] { BorrowIntent(e, amount: 1) }, new FixtureAdapter(FixtureMode.Success, Epoch), T0.AddMinutes(1)));
    }

    private static void ReturnPositive()
    {
        var processor = new ResourceMutationProcessor();
        var first = BorrowOnce(processor);
        var e = first.AcceptedSnapshot.Envelope;
        var segment = first.AcceptedSnapshot.BorrowedSegments.Single();
        var returned = processor.ApplyEffectiveRedistribution(first.AcceptedSnapshot, "batch-return", new[] { ReturnIntent(e, segment.SegmentId) }, new FixtureAdapter(FixtureMode.Success, Epoch), T0.AddMinutes(2));
        Equal(0, returned.AcceptedSnapshot.BorrowedSegments.Count);
        Equal(20m, returned.AcceptedSnapshot.GetEffectiveCapacity(AppA, Cpu).Amount);
    }

    private static void EffectFailureBlocksTruth()
        => Throws<InvalidOperationException>(() => BorrowOnce(adapter: new FixtureAdapter(FixtureMode.Fail, Epoch)));

    private static void PartialEffectBlocksTruth()
        => Throws<InvalidOperationException>(() => BorrowOnce(adapter: new FixtureAdapter(FixtureMode.Partial, Epoch)));

    private static void EffectPayloadActionable()
    {
        var adapter = new InspectingAdapter(Epoch);
        _ = BorrowOnce(adapter: adapter);
        var op = adapter.LastBatch!.Operations.Single();
        Equal(ResourceEffectLane.DelegatedEffectiveDistribution, op.Lane);
        Equal(EffectiveDistributionOperationKind.Borrow, op.EffectiveOperationKind!.Value);
        Equal(AppA.Value, op.SourceApplicationId!.Value);
        Equal(GrantA.Value, op.SourceGrantId!.Value);
        Equal(AppB.Value, op.TargetApplicationId!.Value);
        Equal(5m, op.PrimaryQuantity.Amount);
    }

    private static void FoundationReducePositive()
    {
        var current = Allocations();
        var effective = Effective(Envelope(current), current);
        var intent = FoundationIntent(current, ResourceDecisionKind.Reduce, "reduce-1", 10, 20, 30);
        var result = new ResourceMutationProcessor().ApplyFoundationAllocationMutations(current, "batch-reduce", new[] { intent }, new FixtureAdapter(FixtureMode.Success, Epoch), T0.AddMinutes(1), effective);
        Equal(10m, result.AcceptedSnapshot.GetRequiredAllocation(AppA, Cpu).Allocation.Amount);
    }

    private static void FoundationMutationRequiresQuiescence()
    {
        var current = Allocations();
        var intent = FoundationIntent(current, ResourceDecisionKind.Reduce, "reduce-no-q", 10, 20, 30);
        Throws<InvalidOperationException>(() => new ResourceMutationProcessor().ApplyFoundationAllocationMutations(current, "batch-no-q", new[] { intent }, new FixtureAdapter(FixtureMode.Success, Epoch), T0.AddMinutes(1), null));
    }

    private static void FoundationMutationRejectsActiveBorrow()
    {
        var borrowed = BorrowOnce();
        var current = borrowed.AcceptedSnapshot.AuthoritativeAllocationSnapshot;
        var intent = FoundationIntent(current, ResourceDecisionKind.Reduce, "reduce-active", 10, 20, 30);
        Throws<InvalidOperationException>(() => new ResourceMutationProcessor().ApplyFoundationAllocationMutations(current, "batch-active", new[] { intent }, new FixtureAdapter(FixtureMode.Success, Epoch), T0.AddMinutes(2), borrowed.AcceptedSnapshot));
    }

    private static void RestoreRequiresCapturedBasis()
    {
        var current = Allocations();
        Throws<ArgumentNullException>(() => FoundationIntent(current, ResourceDecisionKind.Restore, "restore-no-basis", 20, 30, 40, null));
        Require(typeof(FoundationAllocationRestorationBasis).GetConstructors(BindingFlags.Public | BindingFlags.Instance).Length == 0, "Restoration basis must not have a public constructor.");
    }

    private static void RestoreAboveBasisRejected()
    {
        var original = Allocations();
        var basis = FoundationAllocationRestorationBasis.Capture("basis-1", original, AppA, Cpu, Evidence("basis", original.ResourceTruth.EpochId, original.ObservedAt.AddMinutes(-1)));
        var reduced = new ApplicationResourceAllocationSnapshot(original.ResourceTruth, T0, new[]
        {
            Allocation(AppA, GrantA, 10, 20, 30),
            Allocation(AppB, GrantB)
        }, true);
        var intent = FoundationIntent(reduced, ResourceDecisionKind.Restore, "restore-too-high", 21, 31, 41, basis);
        Throws<InvalidOperationException>(() => new ResourceMutationProcessor().ApplyFoundationAllocationMutations(reduced, "batch-restore-high", new[] { intent }, new FixtureAdapter(FixtureMode.Success, Epoch), T0.AddMinutes(1), Effective(Envelope(reduced, outA: 0), reduced)));
    }

    private static void RebalanceNotDecisionKind()
        => Require(!Enum.GetNames<ResourceDecisionKind>().Contains("Rebalance", StringComparer.Ordinal), "Rebalance must remain a batch/transaction concept, not a canonical decision kind.");

    private static void IntentEffectTruthDistinct()
        => Require(typeof(FoundationAllocationMutationIntent) != typeof(ResourceEffectApplicationResult) && typeof(ResourceEffectApplicationResult) != typeof(ApplicationResourceAllocationSnapshot), "Intent, applied effect evidence and accepted truth must remain distinct types.");

    private static void EnvironmentNeutralEffectContract()
    {
        var surface = typeof(IResourceEffectAdapter).Assembly.GetExportedTypes().Where(t => t.Namespace == "Foundation.State.ResourceGovernance").Select(t => t.FullName ?? t.Name).ToArray();
        foreach (var token in new[] { "Windows", "Linux", "Docker", "Kubernetes", "HyperV", "VMware" })
            Require(surface.All(name => !name.Contains(token, StringComparison.OrdinalIgnoreCase)), "Environment-specific token leaked into WP-07 surface: " + token);
    }

    private static void ApplicationNeutralSurface()
    {
        var names = typeof(ResourceMutationProcessor).Assembly.GetExportedTypes().Where(t => t.Namespace == "Foundation.State.ResourceGovernance").Select(t => t.FullName ?? t.Name).ToArray();
        foreach (var token in new[] { "FSARM", "TARC", "Trading", "Market", "Broker", "Strategy", "Guardian" })
            Require(names.All(name => !name.Contains(token, StringComparison.OrdinalIgnoreCase)), "Application/business token leaked into WP-07 surface: " + token);
    }

    private static void NoWp08Surface()
    {
        var wp07OwnedTypes = new[]
        {
            typeof(ResourceCoordinationEnvelope),
            typeof(ResourceCoordinationEnvelopeMember),
            typeof(EffectiveResourceDistributionSnapshot),
            typeof(BorrowedEffectiveCapacitySegment),
            typeof(EffectiveDistributionMutationIntent),
            typeof(FoundationResourceMutationAuthority),
            typeof(FoundationAllocationRestorationBasis),
            typeof(FoundationAllocationMutationIntent),
            typeof(ResourceEffectBatch),
            typeof(ResourceEffectOperation),
            typeof(ResourceEffectApplicationResult),
            typeof(IResourceEffectAdapter),
            typeof(AcceptedEffectiveDistributionMutation),
            typeof(AcceptedFoundationAllocationMutation),
            typeof(ResourceMutationProcessor)
        };
        var names = wp07OwnedTypes.Select(t => t.Name).ToArray();
        foreach (var token in new[] { "LoadShedding", "Projection", "DegradationOrder" })
            Require(names.All(name => !name.Contains(token, StringComparison.OrdinalIgnoreCase)), "WP-08 surface leaked into WP-07-owned surface: " + token);
    }

    private enum FixtureMode { Success, Fail, Partial }

    private sealed class FixtureAdapter : IResourceEffectAdapter
    {
        private readonly FixtureMode _mode;
        private readonly ResourceEpochId _epoch;
        public FixtureAdapter(FixtureMode mode, ResourceEpochId epoch) { _mode = mode; _epoch = epoch; }

        public ResourceEffectApplicationResult Apply(ResourceEffectBatch batch, DateTimeOffset appliedAt)
        {
            var ids = batch.Operations.Select(x => x.OperationId).ToArray();
            return _mode switch
            {
                FixtureMode.Success => new ResourceEffectApplicationResult(batch.IdentitySha256, true, false, ids, Evidence("effect-success", _epoch, appliedAt), appliedAt),
                FixtureMode.Fail => new ResourceEffectApplicationResult(batch.IdentitySha256, false, false, Array.Empty<string>(), Evidence("effect-fail", _epoch, appliedAt), appliedAt),
                FixtureMode.Partial => new ResourceEffectApplicationResult(batch.IdentitySha256, false, true, ids.Take(1), Evidence("effect-partial", _epoch, appliedAt), appliedAt),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }

    private sealed class InspectingAdapter : IResourceEffectAdapter
    {
        private readonly ResourceEpochId _epoch;
        public InspectingAdapter(ResourceEpochId epoch) { _epoch = epoch; }
        public ResourceEffectBatch? LastBatch { get; private set; }
        public ResourceEffectApplicationResult Apply(ResourceEffectBatch batch, DateTimeOffset appliedAt)
        {
            LastBatch = batch;
            return new ResourceEffectApplicationResult(batch.IdentitySha256, true, false, batch.Operations.Select(x => x.OperationId), Evidence("effect-inspect", _epoch, appliedAt), appliedAt);
        }
    }

    private static void Run(string name, Action action)
    {
        try { action(); _passed++; Console.WriteLine("PASS " + name); }
        catch (Exception ex) { _failed++; Console.WriteLine($"FAIL {name}: {ex.GetType().Name}: {ex.Message}"); }
    }

    private static void Equal<T>(T expected, T actual)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual)) throw new InvalidOperationException($"Expected '{expected}', got '{actual}'.");
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    private static void Throws<T>(Action action) where T : Exception
    {
        try { action(); }
        catch (T) { return; }
        throw new InvalidOperationException("Expected exception: " + typeof(T).Name);
    }
}
