using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Foundation.Contracts.ResourceGovernance;
using Foundation.State.ResourceGovernance;

namespace Falcon.Stage6.WP07.Verifier;

internal static class Program
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
        Run("coordination_envelope_exact_predecessor", () => Equal(Allocations().IdentitySha256, Envelope().AllocationSnapshot.IdentitySha256));
        Run("wrong_envelope_grant_rejected", WrongEnvelopeGrantRejected);
        Run("protected_minimum_rejected", () => Throws<ArgumentException>(() => Envelope(minA: 21)));
        Run("borrow_out_uses_granted_capacity_only", () => Throws<ArgumentException>(() => Envelope(minA: 10, outA: 11)));
        Run("borrow_in_ceiling_bound", () => Throws<ArgumentException>(() => Envelope(inA: 21)));
        Run("envelope_epoch_rejected", () => Throws<InvalidOperationException>(() => Envelope(evidence: Evidence("bad-env", new ResourceEpochId("other"), T0.AddMinutes(-6)))));
        Run("expired_envelope_rejected", ExpiredEnvelopeRejected);

        Run("borrow_positive", BorrowPositive);
        Run("borrow_source_grant_provenance", BorrowSourceProvenance);
        Run("borrow_target_attribution", BorrowTargetAttribution);
        Run("borrow_does_not_mutate_authoritative_truth", BorrowDoesNotMutateAuthoritativeTruth);
        Run("borrow_conservation", BorrowConservation);
        Run("borrow_over_envelope_rejected", () => Throws<InvalidOperationException>(() => BorrowOnce(amount: 11)));
        Run("return_positive", ReturnPositive);
        Run("return_unknown_segment_rejected", ReturnUnknownSegmentRejected);
        Run("return_provenance_mismatch_rejected", ReturnProvenanceMismatchRejected);
        Run("return_over_segment_rejected", ReturnOverSegmentRejected);

        Run("duplicate_batch_rejected", DuplicateBatchRejected);
        Run("duplicate_intent_rejected", DuplicateIntentRejected);
        Run("stale_envelope_authority_rejected", StaleEnvelopeAuthorityRejected);
        Run("same_generation_envelope_conflict_rejected", SameGenerationEnvelopeConflictRejected);
        Run("stale_fence_rejected", StaleFenceRejected);
        Run("split_brain_fence_rejected", SplitBrainFenceRejected);

        Run("effective_effect_payload_is_actionable", EffectiveEffectPayload);
        Run("foundation_effect_payload_is_actionable", FoundationEffectPayload);
        Run("effect_failure_blocks_truth", () => EffectModeRejected(FixtureEffectMode.Fail));
        Run("partial_effect_blocks_truth", () => EffectModeRejected(FixtureEffectMode.Partial));
        Run("missing_effect_operation_blocks_truth", () => EffectModeRejected(FixtureEffectMode.MissingOperation));
        Run("wrong_effect_batch_blocks_truth", () => EffectModeRejected(FixtureEffectMode.WrongBatch));
        Run("wrong_effect_epoch_blocks_truth", WrongEffectEpochBlocksTruth);

        Run("foundation_authority_positive", () => Equal(3, Authority().AllowedOperations.Count));
        Run("foundation_authority_rejects_grant", () => Throws<ArgumentException>(() => Authority(operations: new[] { ResourceDecisionKind.Grant })));
        Run("foundation_authority_scope_rejected", FoundationAuthorityScopeRejected);
        Run("foundation_authority_expiry_rejected", FoundationAuthorityExpiryRejected);
        Run("foundation_mutation_requires_quiescence", FoundationMutationRequiresQuiescence);
        Run("foundation_mutation_rejects_active_borrow", FoundationMutationRejectsActiveBorrow);
        Run("successor_snapshot_becomes_quiesced", SuccessorSnapshotBecomesQuiesced);

        Run("reduce_positive", ReducePositive);
        Run("reduce_cannot_increase", ReduceCannotIncrease);
        Run("reduce_wrong_grant_rejected", ReduceWrongGrantRejected);
        Run("revoke_positive", RevokePositive);
        Run("restore_requires_basis", RestoreRequiresBasis);
        Run("restoration_basis_is_snapshot_captured", RestorationBasisIsCaptured);
        Run("restoration_basis_constructor_not_public", RestorationBasisConstructorNotPublic);
        Run("restore_positive_after_revoke", RestorePositiveAfterRevoke);
        Run("restore_above_basis_rejected", RestoreAboveBasisRejected);
        Run("restore_basis_scope_mismatch_rejected", RestoreBasisScopeMismatchRejected);
        Run("restore_cannot_reduce_current_truth", RestoreCannotReduceCurrentTruth);
        Run("predecessor_mismatch_rejected", PredecessorMismatchRejected);
        Run("foundation_authority_supersession_rejected", FoundationAuthoritySupersessionRejected);
        Run("foundation_authority_same_generation_conflict_rejected", FoundationAuthoritySameGenerationConflictRejected);
        Run("foundation_partial_effect_blocks_truth", FoundationPartialEffectBlocksTruth);

        Run("resource_truth_identity_preserved", ResourceTruthIdentityPreserved);
        Run("protection_floor_non_reclaimable", () => Equal(ResourceReclaimability.NonReclaimable, ReduceOnce().AcceptedSnapshot.ResourceTruth.GetRequired(Cpu).ProtectionFloorReclaimability));
        Run("recovery_reserve_non_reclaimable", () => Equal(ResourceReclaimability.NonReclaimable, ReduceOnce().AcceptedSnapshot.ResourceTruth.GetRequired(Cpu).RecoveryReserveReclaimability));
        Run("rebalance_not_canonical_decision_kind", RebalanceNotDecisionKind);
        Run("rebalance_is_atomic_batch_concept", RebalanceAtomicBatch);
        Run("eligibility_not_mutation_authority", () => Require(typeof(ResourcePreemptionEligibilityBinding) != typeof(FoundationResourceMutationAuthority), "Eligibility must not mint mutation authority."));
        Run("intent_effect_truth_distinct", IntentEffectTruthDistinct);

        Run("deterministic_envelope_identity", DeterministicEnvelopeIdentity);
        Run("deterministic_effect_batch_identity", DeterministicEffectBatchIdentity);
        Run("deterministic_restoration_basis_identity", DeterministicRestorationBasisIdentity);
        Run("application_neutral_surface", ApplicationNeutralSurface);
        Run("no_fsarm_tarc_trading_type_names", NoBusinessNames);
        Run("no_wp08_surface", NoWp08Surface);
        Run("environment_neutral_effect_contract", EnvironmentNeutralEffectContract);
        Run("wp06_decision_distinct_from_wp07_mutation", () => Require(typeof(AdditionalResourceDecisionRecord) != typeof(FoundationAllocationMutationIntent), "WP-06 decision and WP-07 mutation intent must remain distinct."));
        Run("requested_granted_effective_truth_distinct", () => Require(typeof(AdditionalResourceRequest) != typeof(EffectiveResourceDistributionSnapshot) && typeof(ApplicationResourceAllocationSnapshot) != typeof(EffectiveResourceDistributionSnapshot), "Requested, granted and effective truth must remain distinct."));

        Console.WriteLine();
        Console.WriteLine($"STAGE 6 WP-07 VERIFIER: {_passed}/{_passed + _failed} PASS");
        Console.WriteLine($"Failures: {_failed}");
        return _failed == 0 ? 0 : 1;
    }

    private static ResourceQuantity Q(decimal amount) => new(amount, "units");
    private static ResourceEffectiveLifetime Lifetime() => new(T0.AddHours(-1), null, true);
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
        => new(grant, app, Cpu, Q(allocation), Q(quota), Q(ceiling), Lifetime(), Evidence("alloc-" + app.Value, epoch, T0.AddMinutes(-6)));

    private static ApplicationResourceAllocationSnapshot Allocations(bool twoApps = true, ResourceEpochId? epoch = null)
    {
        var truth = Truth(epoch);
        var records = twoApps ? new[] { Allocation(AppA, GrantA, epoch: truth.EpochId), Allocation(AppB, GrantB, epoch: truth.EpochId) } : Array.Empty<ApplicationResourceAllocation>();
        return new ApplicationResourceAllocationSnapshot(truth, T0.AddMinutes(-5), records, true);
    }

    private static ResourceCoordinationEnvelope Envelope(
        ApplicationResourceAllocationSnapshot? allocations = null,
        long authorityGeneration = 1,
        long fenceGeneration = 1,
        string coordinator = "coordinator-1",
        string fenceToken = "fence-1",
        decimal minA = 10,
        decimal outA = 10,
        decimal inA = 20,
        decimal minB = 10,
        decimal outB = 10,
        decimal inB = 20,
        ResourceEvidenceReference? evidence = null,
        DateTimeOffset? effectiveFrom = null,
        DateTimeOffset? expiresAt = null)
    {
        var a = allocations ?? Allocations();
        return new ResourceCoordinationEnvelope(
            "envelope-authority", new ResourceScopeId("scope-coordination"), coordinator, "aggregate-resource-coordinator",
            authorityGeneration, fenceGeneration, fenceToken, a,
            new[]
            {
                new ResourceCoordinationEnvelopeMember(AppA, GrantA, Cpu, Q(minA), Q(outA), Q(inA)),
                new ResourceCoordinationEnvelopeMember(AppB, GrantB, Cpu, Q(minB), Q(outB), Q(inB))
            },
            evidence ?? Evidence("envelope", a.ResourceTruth.EpochId, T0.AddMinutes(-6)),
            effectiveFrom ?? T0.AddMinutes(-4), expiresAt ?? T0.AddHours(2));
    }

    private static EffectiveResourceDistributionSnapshot Effective(ResourceCoordinationEnvelope? envelope = null, ApplicationResourceAllocationSnapshot? allocations = null, DateTimeOffset? observedAt = null)
    {
        var a = allocations ?? envelope?.AllocationSnapshot ?? Allocations();
        var e = envelope ?? Envelope(a);
        return new EffectiveResourceDistributionSnapshot(a, e, observedAt ?? T0, Array.Empty<BorrowedEffectiveCapacitySegment>());
    }

    private static EffectiveDistributionMutationIntent BorrowIntent(ResourceCoordinationEnvelope envelope, string id = "borrow-1", decimal amount = 5, ApplicationPrincipalId? source = null, ResourceGrantId? sourceGrant = null, ApplicationPrincipalId? target = null)
        => new(id, EffectiveDistributionOperationKind.Borrow, source ?? AppA, sourceGrant ?? GrantA, target ?? AppB, Cpu, Q(amount), null,
            envelope, envelope.CoordinatorInstanceId, envelope.CoordinatorRoleId, envelope.FenceGeneration, envelope.FencingToken,
            new CorrelationId("corr-" + id), new CausationId("cause-" + id), Evidence("intent-" + id, envelope.AllocationSnapshot.ResourceTruth.EpochId, T0.AddMinutes(-1)), T0, T0.AddMinutes(30));

    private static EffectiveDistributionMutationIntent ReturnIntent(ResourceCoordinationEnvelope envelope, string segmentId, string id = "return-1", decimal amount = 5, ApplicationPrincipalId? source = null, ResourceGrantId? sourceGrant = null, ApplicationPrincipalId? target = null)
        => new(id, EffectiveDistributionOperationKind.ReturnBorrowed, source ?? AppA, sourceGrant ?? GrantA, target ?? AppB, Cpu, Q(amount), segmentId,
            envelope, envelope.CoordinatorInstanceId, envelope.CoordinatorRoleId, envelope.FenceGeneration, envelope.FencingToken,
            new CorrelationId("corr-" + id), new CausationId("cause-" + id), Evidence("intent-" + id, envelope.AllocationSnapshot.ResourceTruth.EpochId, T0.AddMinutes(-1)), T0, T0.AddMinutes(30));

    private static FoundationResourceMutationAuthority Authority(long generation = 1, IEnumerable<ApplicationPrincipalId>? apps = null, IEnumerable<ResourceDecisionKind>? operations = null, DateTimeOffset? until = null, string id = "foundation-authority")
        => new(id, new ResourceScopeId("scope-foundation-mutation"), apps ?? new[] { AppA, AppB }, new[] { Cpu }, operations ?? new[] { ResourceDecisionKind.Reduce, ResourceDecisionKind.Revoke, ResourceDecisionKind.Restore }, generation,
            Evidence(id, Epoch, T0.AddMinutes(-6)), T0.AddMinutes(-5), until ?? T0.AddHours(1));

    private static FoundationAllocationRestorationBasis Basis(ApplicationResourceAllocationSnapshot source, ApplicationPrincipalId? app = null, string id = "basis-1")
        => FoundationAllocationRestorationBasis.Capture(id, source, app ?? AppA, Cpu, Evidence("basis-" + (app ?? AppA).Value, source.ResourceTruth.EpochId, source.ObservedAt.AddMinutes(-1)));

    private static FoundationAllocationMutationIntent FoundationIntent(
        ApplicationResourceAllocationSnapshot predecessor, ResourceDecisionKind operation, string id,
        decimal allocation, decimal quota, decimal ceiling,
        FoundationResourceMutationAuthority? authority = null,
        FoundationAllocationRestorationBasis? basis = null,
        ApplicationPrincipalId? app = null,
        ResourceGrantId? grant = null,
        string? predecessorIdentity = null)
        => new(id, operation, app ?? AppA, grant ?? GrantA, Cpu, Q(allocation), Q(quota), Q(ceiling), authority ?? Authority(), basis,
            predecessorIdentity ?? predecessor.IdentitySha256, new CorrelationId("corr-" + id), new CausationId("cause-" + id),
            Evidence("intent-" + id, predecessor.ResourceTruth.EpochId, T0.AddMinutes(-1)), T0, T0.AddMinutes(30));

    private static IResourceEffectAdapter SuccessAdapter(ResourceEpochId epoch) => new FixtureAdapter(FixtureEffectMode.Success, epoch);

    private static AcceptedEffectiveDistributionMutation BorrowOnce(ResourceMutationProcessor? processor = null, ResourceCoordinationEnvelope? envelope = null, decimal amount = 5, IResourceEffectAdapter? adapter = null, string batch = "borrow-batch", string intent = "borrow-1")
    {
        var e = envelope ?? Envelope();
        return (processor ?? new ResourceMutationProcessor()).ApplyEffectiveRedistribution(Effective(e), batch, new[] { BorrowIntent(e, intent, amount) }, adapter ?? SuccessAdapter(e.AllocationSnapshot.ResourceTruth.EpochId), T0.AddMinutes(1));
    }

    private static AcceptedFoundationAllocationMutation ReduceOnce(ResourceMutationProcessor? processor = null, ApplicationResourceAllocationSnapshot? current = null, FoundationResourceMutationAuthority? authority = null, string batch = "reduce-batch", string id = "reduce-1")
    {
        var snapshot = current ?? Allocations();
        var p = processor ?? new ResourceMutationProcessor();
        var intent = FoundationIntent(snapshot, ResourceDecisionKind.Reduce, id, 10, 20, 30, authority: authority);
        return p.ApplyFoundationAllocationMutations(snapshot, batch, new[] { intent }, SuccessAdapter(snapshot.ResourceTruth.EpochId), T0.AddMinutes(1), Effective(Envelope(snapshot), snapshot));
    }

    private static AcceptedFoundationAllocationMutation RevokeOnce(ResourceMutationProcessor p, ApplicationResourceAllocationSnapshot current, string batch = "revoke-batch", string id = "revoke-1")
    {
        var intent = FoundationIntent(current, ResourceDecisionKind.Revoke, id, 0, 0, 0);
        return p.ApplyFoundationAllocationMutations(current, batch, new[] { intent }, SuccessAdapter(current.ResourceTruth.EpochId), T0.AddMinutes(1), Effective(Envelope(current), current));
    }

    private static void ZeroApplicationValidity()
    {
        Equal(0, Allocations(false).Allocations.Count);
        _ = new ResourceMutationProcessor();
    }

    private static void WrongEnvelopeGrantRejected()
    {
        var a = Allocations();
        Throws<ArgumentException>(() => new ResourceCoordinationEnvelope("auth", new ResourceScopeId("scope-x"), "coord", "role", 1, 1, "fence", a,
            new[] { new ResourceCoordinationEnvelopeMember(AppA, new ResourceGrantId("wrong"), Cpu, Q(10), Q(5), Q(5)) },
            Evidence("env", a.ResourceTruth.EpochId, T0.AddMinutes(-6)), T0.AddMinutes(-4), T0.AddMinutes(10)));
    }

    private static void ExpiredEnvelopeRejected()
    {
        var a = Allocations();
        var env = Envelope(a, evidence: Evidence("expired", a.ResourceTruth.EpochId, T0.AddMinutes(-11)), effectiveFrom: T0.AddMinutes(-10), expiresAt: T0.AddMinutes(-1));
        Throws<InvalidOperationException>(() => new EffectiveResourceDistributionSnapshot(a, env, T0, Array.Empty<BorrowedEffectiveCapacitySegment>()));
    }

    private static void BorrowPositive()
    {
        var r = BorrowOnce();
        Equal(15m, r.AcceptedSnapshot.GetEffectiveCapacity(AppA, Cpu).Amount);
        Equal(25m, r.AcceptedSnapshot.GetEffectiveCapacity(AppB, Cpu).Amount);
    }

    private static void BorrowSourceProvenance()
    {
        var s = BorrowOnce().AcceptedSnapshot.BorrowedSegments.Single();
        Equal(AppA.Value, s.SourceApplicationId.Value);
        Equal(GrantA.Value, s.SourceGrantId.Value);
    }

    private static void BorrowTargetAttribution()
        => Equal(AppB.Value, BorrowOnce().AcceptedSnapshot.BorrowedSegments.Single().TargetApplicationId.Value);

    private static void BorrowDoesNotMutateAuthoritativeTruth()
    {
        var r = BorrowOnce();
        Equal(20m, r.AcceptedSnapshot.AuthoritativeAllocationSnapshot.GetRequiredAllocation(AppA, Cpu).Allocation.Amount);
        Equal(20m, r.AcceptedSnapshot.AuthoritativeAllocationSnapshot.GetRequiredAllocation(AppB, Cpu).Allocation.Amount);
    }

    private static void BorrowConservation()
    {
        var r = BorrowOnce();
        Equal(40m, r.AcceptedSnapshot.GetEffectiveCapacity(AppA, Cpu).Amount + r.AcceptedSnapshot.GetEffectiveCapacity(AppB, Cpu).Amount);
    }

    private static void ReturnPositive()
    {
        var p = new ResourceMutationProcessor();
        var env = Envelope();
        var borrowed = p.ApplyEffectiveRedistribution(Effective(env), "b1", new[] { BorrowIntent(env, "i1", 5) }, SuccessAdapter(Epoch), T0.AddMinutes(1));
        var segment = borrowed.AcceptedSnapshot.BorrowedSegments.Single();
        var returned = p.ApplyEffectiveRedistribution(borrowed.AcceptedSnapshot, "b2", new[] { ReturnIntent(env, segment.SegmentId, "i2", 3) }, SuccessAdapter(Epoch), T0.AddMinutes(2));
        Equal(2m, returned.AcceptedSnapshot.BorrowedSegments.Single().Quantity.Amount);
    }

    private static void ReturnUnknownSegmentRejected()
    {
        var env = Envelope();
        Throws<KeyNotFoundException>(() => new ResourceMutationProcessor().ApplyEffectiveRedistribution(Effective(env), "b", new[] { ReturnIntent(env, "segment.unknown") }, SuccessAdapter(Epoch), T0.AddMinutes(1)));
    }

    private static void ReturnProvenanceMismatchRejected()
    {
        var p = new ResourceMutationProcessor();
        var env = Envelope();
        var borrowed = p.ApplyEffectiveRedistribution(Effective(env), "b1", new[] { BorrowIntent(env, "i1", 5) }, SuccessAdapter(Epoch), T0.AddMinutes(1));
        var segment = borrowed.AcceptedSnapshot.BorrowedSegments.Single();
        Throws<InvalidOperationException>(() => p.ApplyEffectiveRedistribution(borrowed.AcceptedSnapshot, "b2", new[] { ReturnIntent(env, segment.SegmentId, "i2", 2, AppB, GrantB, AppA) }, SuccessAdapter(Epoch), T0.AddMinutes(2)));
    }

    private static void ReturnOverSegmentRejected()
    {
        var p = new ResourceMutationProcessor();
        var env = Envelope();
        var borrowed = p.ApplyEffectiveRedistribution(Effective(env), "b1", new[] { BorrowIntent(env, "i1", 5) }, SuccessAdapter(Epoch), T0.AddMinutes(1));
        var segment = borrowed.AcceptedSnapshot.BorrowedSegments.Single();
        Throws<InvalidOperationException>(() => p.ApplyEffectiveRedistribution(borrowed.AcceptedSnapshot, "b2", new[] { ReturnIntent(env, segment.SegmentId, "i2", 6) }, SuccessAdapter(Epoch), T0.AddMinutes(2)));
    }

    private static void DuplicateBatchRejected()
    {
        var p = new ResourceMutationProcessor();
        var env = Envelope();
        _ = p.ApplyEffectiveRedistribution(Effective(env), "dup", new[] { BorrowIntent(env, "i1", 1) }, SuccessAdapter(Epoch), T0.AddMinutes(1));
        Throws<InvalidOperationException>(() => p.ApplyEffectiveRedistribution(Effective(env), "dup", new[] { BorrowIntent(env, "i2", 1) }, SuccessAdapter(Epoch), T0.AddMinutes(2)));
    }

    private static void DuplicateIntentRejected()
    {
        var p = new ResourceMutationProcessor();
        var env = Envelope();
        _ = p.ApplyEffectiveRedistribution(Effective(env), "b1", new[] { BorrowIntent(env, "same", 1) }, SuccessAdapter(Epoch), T0.AddMinutes(1));
        Throws<InvalidOperationException>(() => p.ApplyEffectiveRedistribution(Effective(env), "b2", new[] { BorrowIntent(env, "same", 1) }, SuccessAdapter(Epoch), T0.AddMinutes(2)));
    }

    private static void StaleEnvelopeAuthorityRejected()
    {
        var p = new ResourceMutationProcessor();
        var newer = Envelope(authorityGeneration: 2, fenceGeneration: 2, fenceToken: "f2");
        _ = p.ApplyEffectiveRedistribution(Effective(newer), "b1", new[] { BorrowIntent(newer, "i1", 1) }, SuccessAdapter(Epoch), T0.AddMinutes(1));
        var old = Envelope(authorityGeneration: 1, fenceGeneration: 1, fenceToken: "f1");
        Throws<InvalidOperationException>(() => p.ApplyEffectiveRedistribution(Effective(old), "b2", new[] { BorrowIntent(old, "i2", 1) }, SuccessAdapter(Epoch), T0.AddMinutes(2)));
    }

    private static void SameGenerationEnvelopeConflictRejected()
    {
        var p = new ResourceMutationProcessor();
        var first = Envelope(authorityGeneration: 1, fenceGeneration: 1, fenceToken: "f1");
        _ = p.ApplyEffectiveRedistribution(Effective(first), "b1", new[] { BorrowIntent(first, "i1", 1) }, SuccessAdapter(Epoch), T0.AddMinutes(1));
        var conflict = Envelope(authorityGeneration: 1, fenceGeneration: 2, fenceToken: "f2");
        Throws<InvalidOperationException>(() => p.ApplyEffectiveRedistribution(Effective(conflict), "b2", new[] { BorrowIntent(conflict, "i2", 1) }, SuccessAdapter(Epoch), T0.AddMinutes(2)));
    }

    private static void StaleFenceRejected()
    {
        var p = new ResourceMutationProcessor();
        var first = Envelope(authorityGeneration: 1, fenceGeneration: 2, fenceToken: "f2");
        _ = p.ApplyEffectiveRedistribution(Effective(first), "b1", new[] { BorrowIntent(first, "i1", 1) }, SuccessAdapter(Epoch), T0.AddMinutes(1));
        var stale = Envelope(authorityGeneration: 2, fenceGeneration: 1, fenceToken: "f1");
        Throws<InvalidOperationException>(() => p.ApplyEffectiveRedistribution(Effective(stale), "b2", new[] { BorrowIntent(stale, "i2", 1) }, SuccessAdapter(Epoch), T0.AddMinutes(2)));
    }

    private static void SplitBrainFenceRejected()
    {
        var p = new ResourceMutationProcessor();
        var first = Envelope(authorityGeneration: 1, fenceGeneration: 1, coordinator: "c1", fenceToken: "f1");
        _ = p.ApplyEffectiveRedistribution(Effective(first), "b1", new[] { BorrowIntent(first, "i1", 1) }, SuccessAdapter(Epoch), T0.AddMinutes(1));
        var split = Envelope(authorityGeneration: 2, fenceGeneration: 1, coordinator: "c2", fenceToken: "f2");
        Throws<InvalidOperationException>(() => p.ApplyEffectiveRedistribution(Effective(split), "b2", new[] { BorrowIntent(split, "i2", 1) }, SuccessAdapter(Epoch), T0.AddMinutes(2)));
    }

    private static void EffectiveEffectPayload()
    {
        var op = ResourceEffectOperation.ForEffective(BorrowIntent(Envelope()));
        Equal(ResourceEffectLane.DelegatedEffectiveDistribution, op.Lane);
        Equal(EffectiveDistributionOperationKind.Borrow, op.EffectiveOperationKind!.Value);
        Equal(AppA.Value, op.SourceApplicationId!.Value);
        Equal(GrantA.Value, op.SourceGrantId!.Value);
        Equal(AppB.Value, op.TargetApplicationId!.Value);
        Equal(5m, op.PrimaryQuantity.Amount);
    }

    private static void FoundationEffectPayload()
    {
        var current = Allocations();
        var op = ResourceEffectOperation.ForFoundation(FoundationIntent(current, ResourceDecisionKind.Reduce, "r", 10, 20, 30));
        Equal(ResourceEffectLane.FoundationAuthoritativeAllocation, op.Lane);
        Equal(ResourceDecisionKind.Reduce, op.FoundationOperationKind!.Value);
        Equal(10m, op.PrimaryQuantity.Amount);
        Equal(20m, op.TargetQuota!.Amount);
        Equal(30m, op.TargetCeiling!.Amount);
    }

    private static void EffectModeRejected(FixtureEffectMode mode)
    {
        var env = Envelope();
        Throws<InvalidOperationException>(() => new ResourceMutationProcessor().ApplyEffectiveRedistribution(Effective(env), "b", new[] { BorrowIntent(env) }, new FixtureAdapter(mode, Epoch), T0.AddMinutes(1)));
    }

    private static void WrongEffectEpochBlocksTruth()
    {
        var env = Envelope();
        Throws<InvalidOperationException>(() => new ResourceMutationProcessor().ApplyEffectiveRedistribution(Effective(env), "b", new[] { BorrowIntent(env) }, new FixtureAdapter(FixtureEffectMode.Success, new ResourceEpochId("other")), T0.AddMinutes(1)));
    }

    private static void FoundationAuthorityScopeRejected()
    {
        var current = Allocations();
        var intent = FoundationIntent(current, ResourceDecisionKind.Reduce, "r", 10, 20, 30, authority: Authority(apps: new[] { AppB }));
        Throws<InvalidOperationException>(() => new ResourceMutationProcessor().ApplyFoundationAllocationMutations(current, "b", new[] { intent }, SuccessAdapter(Epoch), T0.AddMinutes(1), Effective(Envelope(current), current)));
    }

    private static void FoundationAuthorityExpiryRejected()
    {
        var current = Allocations();
        var intent = FoundationIntent(current, ResourceDecisionKind.Reduce, "r", 10, 20, 30, authority: Authority(until: T0.AddSeconds(30)));
        Throws<InvalidOperationException>(() => new ResourceMutationProcessor().ApplyFoundationAllocationMutations(current, "b", new[] { intent }, SuccessAdapter(Epoch), T0.AddMinutes(1), Effective(Envelope(current), current)));
    }

    private static void FoundationMutationRequiresQuiescence()
    {
        var current = Allocations();
        var intent = FoundationIntent(current, ResourceDecisionKind.Reduce, "r", 10, 20, 30);
        Throws<InvalidOperationException>(() => new ResourceMutationProcessor().ApplyFoundationAllocationMutations(current, "b", new[] { intent }, SuccessAdapter(Epoch), T0.AddMinutes(1)));
    }

    private static void FoundationMutationRejectsActiveBorrow()
    {
        var p = new ResourceMutationProcessor();
        var env = Envelope();
        var borrowed = p.ApplyEffectiveRedistribution(Effective(env), "borrow-b", new[] { BorrowIntent(env, "borrow-i", 5) }, SuccessAdapter(Epoch), T0.AddMinutes(1));
        var intent = FoundationIntent(env.AllocationSnapshot, ResourceDecisionKind.Reduce, "reduce", 10, 20, 30);
        Throws<InvalidOperationException>(() => p.ApplyFoundationAllocationMutations(env.AllocationSnapshot, "reduce-b", new[] { intent }, SuccessAdapter(Epoch), T0.AddMinutes(2), borrowed.AcceptedSnapshot));
    }

    private static void SuccessorSnapshotBecomesQuiesced()
    {
        var p = new ResourceMutationProcessor();
        var original = Allocations();
        var first = FoundationIntent(original, ResourceDecisionKind.Reduce, "r1", 15, 25, 35);
        var reduced = p.ApplyFoundationAllocationMutations(original, "b1", new[] { first }, SuccessAdapter(Epoch), T0.AddMinutes(1), Effective(Envelope(original), original)).AcceptedSnapshot;
        var second = FoundationIntent(reduced, ResourceDecisionKind.Reduce, "r2", 10, 20, 30);
        var result = p.ApplyFoundationAllocationMutations(reduced, "b2", new[] { second }, SuccessAdapter(Epoch), T0.AddMinutes(2));
        Equal(10m, result.AcceptedSnapshot.GetRequiredAllocation(AppA, Cpu).Allocation.Amount);
    }

    private static void ReducePositive()
        => Equal(10m, ReduceOnce().AcceptedSnapshot.GetRequiredAllocation(AppA, Cpu).Allocation.Amount);

    private static void ReduceCannotIncrease()
    {
        var current = Allocations();
        var intent = FoundationIntent(current, ResourceDecisionKind.Reduce, "r", 21, 30, 40);
        Throws<InvalidOperationException>(() => new ResourceMutationProcessor().ApplyFoundationAllocationMutations(current, "b", new[] { intent }, SuccessAdapter(Epoch), T0.AddMinutes(1), Effective(Envelope(current), current)));
    }

    private static void ReduceWrongGrantRejected()
    {
        var current = Allocations();
        var intent = FoundationIntent(current, ResourceDecisionKind.Reduce, "r", 10, 20, 30, grant: new ResourceGrantId("wrong"));
        Throws<InvalidOperationException>(() => new ResourceMutationProcessor().ApplyFoundationAllocationMutations(current, "b", new[] { intent }, SuccessAdapter(Epoch), T0.AddMinutes(1), Effective(Envelope(current), current)));
    }

    private static void RevokePositive()
    {
        var p = new ResourceMutationProcessor();
        var result = RevokeOnce(p, Allocations());
        Equal(1, result.AcceptedSnapshot.Allocations.Count);
        Throws<KeyNotFoundException>(() => result.AcceptedSnapshot.GetRequiredAllocation(AppA, Cpu));
    }

    private static void RestoreRequiresBasis()
        => Throws<ArgumentNullException>(() => FoundationIntent(Allocations(), ResourceDecisionKind.Restore, "restore", 20, 30, 40));

    private static void RestorationBasisIsCaptured()
    {
        var source = Allocations();
        var basis = Basis(source);
        Equal(source.IdentitySha256, basis.SourceAllocationSnapshotIdentitySha256);
        Equal(GrantA.Value, basis.GrantId.Value);
        Equal(20m, basis.MaximumRestorableAllocation.Amount);
        Equal(40m, basis.MaximumRestorableCeiling.Amount);
    }

    private static void RestorationBasisConstructorNotPublic()
        => Equal(0, typeof(FoundationAllocationRestorationBasis).GetConstructors(BindingFlags.Public | BindingFlags.Instance).Length);

    private static void RestorePositiveAfterRevoke()
    {
        var p = new ResourceMutationProcessor();
        var original = Allocations();
        var basis = Basis(original);
        var revoked = RevokeOnce(p, original, "b1", "revoke").AcceptedSnapshot;
        var restore = FoundationIntent(revoked, ResourceDecisionKind.Restore, "restore", 20, 30, 40, basis: basis);
        var restored = p.ApplyFoundationAllocationMutations(revoked, "b2", new[] { restore }, SuccessAdapter(Epoch), T0.AddMinutes(2));
        Equal(20m, restored.AcceptedSnapshot.GetRequiredAllocation(AppA, Cpu).Allocation.Amount);
    }

    private static void RestoreAboveBasisRejected()
    {
        var p = new ResourceMutationProcessor();
        var original = Allocations();
        var basis = Basis(original);
        var revoked = RevokeOnce(p, original, "b1", "revoke").AcceptedSnapshot;
        var restore = FoundationIntent(revoked, ResourceDecisionKind.Restore, "restore", 21, 31, 41, basis: basis);
        Throws<InvalidOperationException>(() => p.ApplyFoundationAllocationMutations(revoked, "b2", new[] { restore }, SuccessAdapter(Epoch), T0.AddMinutes(2)));
    }

    private static void RestoreBasisScopeMismatchRejected()
    {
        var p = new ResourceMutationProcessor();
        var original = Allocations();
        var basisB = Basis(original, AppB, "basis-b");
        var revoked = RevokeOnce(p, original, "b1", "revoke").AcceptedSnapshot;
        var restore = FoundationIntent(revoked, ResourceDecisionKind.Restore, "restore", 20, 30, 40, basis: basisB);
        Throws<InvalidOperationException>(() => p.ApplyFoundationAllocationMutations(revoked, "b2", new[] { restore }, SuccessAdapter(Epoch), T0.AddMinutes(2)));
    }

    private static void RestoreCannotReduceCurrentTruth()
    {
        var p = new ResourceMutationProcessor();
        var original = Allocations();
        var basis = Basis(original);
        var reduced = ReduceOnce(p, original, batch: "b1", id: "r1").AcceptedSnapshot;
        var restore = FoundationIntent(reduced, ResourceDecisionKind.Restore, "restore", 9, 19, 29, basis: basis);
        Throws<InvalidOperationException>(() => p.ApplyFoundationAllocationMutations(reduced, "b2", new[] { restore }, SuccessAdapter(Epoch), T0.AddMinutes(2)));
    }

    private static void PredecessorMismatchRejected()
    {
        var current = Allocations();
        var intent = FoundationIntent(current, ResourceDecisionKind.Reduce, "r", 10, 20, 30, predecessorIdentity: "BADPREDECESSOR");
        Throws<InvalidOperationException>(() => new ResourceMutationProcessor().ApplyFoundationAllocationMutations(current, "b", new[] { intent }, SuccessAdapter(Epoch), T0.AddMinutes(1), Effective(Envelope(current), current)));
    }

    private static void FoundationAuthoritySupersessionRejected()
    {
        var p = new ResourceMutationProcessor();
        var current = Allocations();
        var first = FoundationIntent(current, ResourceDecisionKind.Reduce, "r1", 15, 25, 35, authority: Authority(2, id: "newer"));
        var reduced = p.ApplyFoundationAllocationMutations(current, "b1", new[] { first }, SuccessAdapter(Epoch), T0.AddMinutes(1), Effective(Envelope(current), current)).AcceptedSnapshot;
        var second = FoundationIntent(reduced, ResourceDecisionKind.Reduce, "r2", 10, 20, 30, authority: Authority(1, id: "older"));
        Throws<InvalidOperationException>(() => p.ApplyFoundationAllocationMutations(reduced, "b2", new[] { second }, SuccessAdapter(Epoch), T0.AddMinutes(2)));
    }

    private static void FoundationAuthoritySameGenerationConflictRejected()
    {
        var p = new ResourceMutationProcessor();
        var current = Allocations();
        var first = FoundationIntent(current, ResourceDecisionKind.Reduce, "r1", 15, 25, 35, authority: Authority(1, id: "auth-a"));
        var reduced = p.ApplyFoundationAllocationMutations(current, "b1", new[] { first }, SuccessAdapter(Epoch), T0.AddMinutes(1), Effective(Envelope(current), current)).AcceptedSnapshot;
        var second = FoundationIntent(reduced, ResourceDecisionKind.Reduce, "r2", 10, 20, 30, authority: Authority(1, id: "auth-b"));
        Throws<InvalidOperationException>(() => p.ApplyFoundationAllocationMutations(reduced, "b2", new[] { second }, SuccessAdapter(Epoch), T0.AddMinutes(2)));
    }

    private static void FoundationPartialEffectBlocksTruth()
    {
        var current = Allocations();
        var intent = FoundationIntent(current, ResourceDecisionKind.Reduce, "r", 10, 20, 30);
        Throws<InvalidOperationException>(() => new ResourceMutationProcessor().ApplyFoundationAllocationMutations(current, "b", new[] { intent }, new FixtureAdapter(FixtureEffectMode.Partial, Epoch), T0.AddMinutes(1), Effective(Envelope(current), current)));
    }

    private static void ResourceTruthIdentityPreserved()
    {
        var current = Allocations();
        Equal(current.ResourceTruth.IdentitySha256, ReduceOnce(current: current).AcceptedSnapshot.ResourceTruth.IdentitySha256);
    }

    private static void RebalanceNotDecisionKind()
        => Require(!Enum.GetNames<ResourceDecisionKind>().Contains("Rebalance", StringComparer.Ordinal), "Rebalance must not become a canonical decision kind.");

    private static void RebalanceAtomicBatch()
    {
        var env = Envelope();
        var intents = new[] { BorrowIntent(env, "a-to-b", 2), BorrowIntent(env, "b-to-a", 2, AppB, GrantB, AppA) };
        var r = new ResourceMutationProcessor().ApplyEffectiveRedistribution(Effective(env), "rebalance-batch", intents, SuccessAdapter(Epoch), T0.AddMinutes(1));
        Equal(20m, r.AcceptedSnapshot.GetEffectiveCapacity(AppA, Cpu).Amount);
        Equal(20m, r.AcceptedSnapshot.GetEffectiveCapacity(AppB, Cpu).Amount);
    }

    private static void IntentEffectTruthDistinct()
        => Require(typeof(FoundationAllocationMutationIntent) != typeof(ResourceEffectApplicationResult) && typeof(ResourceEffectApplicationResult) != typeof(ApplicationResourceAllocationSnapshot), "Intent, applied effect and accepted truth must remain distinct.");

    private static void DeterministicEnvelopeIdentity()
    {
        var a = Allocations();
        Equal(Envelope(a).IdentitySha256, Envelope(a).IdentitySha256);
    }

    private static void DeterministicEffectBatchIdentity()
    {
        var env = Envelope();
        var a = ResourceEffectOperation.ForEffective(BorrowIntent(env, "a", 1));
        var z = ResourceEffectOperation.ForEffective(BorrowIntent(env, "z", 1));
        Equal(new ResourceEffectBatch("batch", new[] { z, a }).IdentitySha256, new ResourceEffectBatch("batch", new[] { a, z }).IdentitySha256);
    }

    private static void DeterministicRestorationBasisIdentity()
    {
        var source = Allocations();
        Equal(Basis(source).IdentitySha256, Basis(source).IdentitySha256);
    }

    private static string[] Surface()
        => typeof(ResourceMutationProcessor).Assembly.GetExportedTypes().Where(type => type.Namespace == "Foundation.State.ResourceGovernance").Select(type => type.FullName ?? type.Name).ToArray();

    private static void ApplicationNeutralSurface()
    {
        foreach (var token in new[] { "Order", "Position", "Broker", "Market", "Strategy", "Trading" })
            Require(Surface().All(name => !name.Contains(token, StringComparison.OrdinalIgnoreCase)), "Business token leaked: " + token);
    }

    private static void NoBusinessNames()
    {
        foreach (var token in new[] { "FSARM", "TARC", "Trading" })
            Require(Surface().All(name => !name.Contains(token, StringComparison.OrdinalIgnoreCase)), "Business-specific type leaked: " + token);
    }

    private static void NoWp08Surface()
    {
        foreach (var token in new[] { "LoadShedding", "ResourceProjection", "PressureSignalProjection" })
            Require(Surface().All(name => !name.Contains(token, StringComparison.OrdinalIgnoreCase)), "WP-08 surface leaked: " + token);
    }

    private static void EnvironmentNeutralEffectContract()
    {
        var method = typeof(IResourceEffectAdapter).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).Single();
        foreach (var p in method.GetParameters())
            Require(!p.ParameterType.Name.Contains("Windows", StringComparison.OrdinalIgnoreCase) && !p.ParameterType.Name.Contains("Linux", StringComparison.OrdinalIgnoreCase) && !p.ParameterType.Name.Contains("Container", StringComparison.OrdinalIgnoreCase), "Environment-specific effect contract leakage.");
    }

    private enum FixtureEffectMode { Success, Fail, Partial, MissingOperation, WrongBatch }

    private sealed class FixtureAdapter : IResourceEffectAdapter
    {
        private readonly FixtureEffectMode _mode;
        private readonly ResourceEpochId _epoch;
        public FixtureAdapter(FixtureEffectMode mode, ResourceEpochId epoch) { _mode = mode; _epoch = epoch; }

        public ResourceEffectApplicationResult Apply(ResourceEffectBatch batch, DateTimeOffset appliedAt)
        {
            var ids = batch.Operations.Select(item => item.OperationId).ToArray();
            return _mode switch
            {
                FixtureEffectMode.Success => new ResourceEffectApplicationResult(batch.IdentitySha256, true, false, ids, Evidence("effect", _epoch, appliedAt), appliedAt),
                FixtureEffectMode.Fail => new ResourceEffectApplicationResult(batch.IdentitySha256, false, false, Array.Empty<string>(), Evidence("effect-fail", _epoch, appliedAt), appliedAt),
                FixtureEffectMode.Partial => new ResourceEffectApplicationResult(batch.IdentitySha256, false, true, ids.Take(Math.Max(1, ids.Length - 1)), Evidence("effect-partial", _epoch, appliedAt), appliedAt),
                FixtureEffectMode.MissingOperation => new ResourceEffectApplicationResult(batch.IdentitySha256, true, false, Array.Empty<string>(), Evidence("effect-missing", _epoch, appliedAt), appliedAt),
                FixtureEffectMode.WrongBatch => new ResourceEffectApplicationResult("BADBATCH", true, false, ids, Evidence("effect-wrong", _epoch, appliedAt), appliedAt),
                _ => throw new InvalidOperationException()
            };
        }
    }

    private static void Run(string name, Action test)
    {
        try { test(); _passed++; Console.WriteLine("PASS " + name); }
        catch (Exception ex) { _failed++; Console.WriteLine($"FAIL {name}: {ex.GetType().Name}: {ex.Message}"); }
    }

    private static void Require(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }
    private static void Equal<T>(T expected, T actual) { if (!EqualityComparer<T>.Default.Equals(expected, actual)) throw new InvalidOperationException($"Expected '{expected}' but got '{actual}'."); }
    private static void Throws<T>(Action action) where T : Exception
    {
        try { action(); }
        catch (T) { return; }
        throw new InvalidOperationException("Expected exception: " + typeof(T).Name);
    }
}
