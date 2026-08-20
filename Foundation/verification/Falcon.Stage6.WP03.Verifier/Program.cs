using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Foundation.Contracts.ResourceGovernance;
using Foundation.State.ResourceGovernance;

namespace Falcon.Stage6.WP03.Verifier;

internal static class Program
{
    private static int _failures;

    private static readonly DateTimeOffset T0 = new(2026, 8, 9, 0, 0, 0, TimeSpan.Zero);
    private static readonly ResourceEpochId Epoch = new("epoch-001");
    private static readonly ResourceClassId Cpu = new("cpu");

    private static int Main()
    {
        Run("positive_snapshot", PositiveSnapshot);
        Run("allocation_below_quota_below_ceiling", AllocationBelowQuotaBelowCeiling);
        Run("allocation_exceeds_quota_rejected", AllocationExceedsQuotaRejected);
        Run("quota_exceeds_ceiling_rejected", QuotaExceedsCeilingRejected);
        Run("allocation_quota_ceiling_unit_mismatch_rejected", AllocationQuotaCeilingUnitMismatchRejected);
        Run("truth_unit_mismatch_rejected", TruthUnitMismatchRejected);
        Run("unknown_resource_class_rejected", UnknownResourceClassRejected);
        Run("evidence_epoch_mismatch_rejected", EvidenceEpochMismatchRejected);
        Run("future_evidence_rejected", FutureEvidenceRejected);
        Run("future_effective_allocation_rejected", FutureEffectiveAllocationRejected);
        Run("expired_allocation_rejected", ExpiredAllocationRejected);
        Run("snapshot_predating_resource_truth_rejected", SnapshotPredatingResourceTruthRejected);
        Run("unavailable_allocation_truth_fails_closed", UnavailableAllocationTruthFailsClosed);
        Run("duplicate_application_resource_binding_rejected", DuplicateApplicationResourceBindingRejected);
        Run("duplicate_grant_identity_rejected", DuplicateGrantIdentityRejected);
        Run("individual_ceiling_over_allocatable_rejected", IndividualCeilingOverAllocatableRejected);
        Run("aggregate_allocation_over_allocatable_rejected", AggregateAllocationOverAllocatableRejected);
        Run("aggregate_quota_over_allocatable_rejected", AggregateQuotaOverAllocatableRejected);
        Run("aggregate_ceiling_over_allocatable_rejected", AggregateCeilingOverAllocatableRejected);
        Run("exact_allocatable_boundary_is_valid", ExactAllocatableBoundaryIsValid);
        Run("zero_application_validity", ZeroApplicationValidity);
        Run("ordering_is_deterministic", OrderingIsDeterministic);
        Run("identity_repeat_is_deterministic", IdentityRepeatIsDeterministic);
        Run("allocation_mutation_changes_identity", AllocationMutationChangesIdentity);
        Run("quota_mutation_changes_identity", QuotaMutationChangesIdentity);
        Run("ceiling_mutation_changes_identity", CeilingMutationChangesIdentity);
        Run("evidence_mutation_changes_identity", EvidenceMutationChangesIdentity);
        Run("lifetime_mutation_changes_identity", LifetimeMutationChangesIdentity);
        Run("resource_truth_identity_mutation_changes_identity", ResourceTruthIdentityMutationChangesIdentity);
        Run("identity_is_uppercase_sha256", IdentityIsUppercaseSha256);
        Run("known_application_resource_lookup", KnownApplicationResourceLookup);
        Run("unknown_application_resource_lookup_fails_closed", UnknownApplicationResourceLookupFailsClosed);
        Run("application_view_contains_only_own_records", ApplicationViewContainsOnlyOwnRecords);
        Run("unknown_application_view_is_empty", UnknownApplicationViewIsEmpty);
        Run("application_view_binds_source_snapshot_identity", ApplicationViewBindsSourceSnapshotIdentity);
        Run("allocation_collection_is_read_only", AllocationCollectionIsReadOnly);
        Run("application_view_collection_is_read_only", ApplicationViewCollectionIsReadOnly);
        Run("protection_floor_not_application_capacity", ProtectionFloorNotApplicationCapacity);
        Run("recovery_reserve_not_application_capacity", RecoveryReserveNotApplicationCapacity);
        Run("production_surface_has_no_trading_terms", ProductionSurfaceHasNoTradingTerms);
        Run("production_surface_has_no_wp04_plus_runtime_terms", ProductionSurfaceHasNoWp04PlusRuntimeTerms);
        Run("allocation_surface_has_no_priority_or_pressure_fields", AllocationSurfaceHasNoPriorityOrPressureFields);
        Run("application_identity_does_not_create_authority", ApplicationIdentityDoesNotCreateAuthority);
        Run("grant_identity_does_not_create_authority", GrantIdentityDoesNotCreateAuthority);
        Run("resource_truth_remains_singular_predecessor", ResourceTruthRemainsSingularPredecessor);

        Console.WriteLine();
        Console.WriteLine($"STAGE 6 WP-03 VERIFIER: {45 - _failures}/45 PASS");
        Console.WriteLine($"Failures: {_failures}");
        return _failures == 0 ? 0 : 1;
    }

    private static FoundationResourceTruthSnapshot Truth(decimal total = 100m, decimal floor = 10m, decimal reserve = 10m)
    {
        var evidence = new ResourceEvidenceReference(
            new ResourceEvidenceId("truth-evidence"),
            new ResourceScopeId("foundation-resource-truth"),
            T0,
            Epoch);

        return new FoundationResourceTruthSnapshot(
            Epoch,
            T0,
            new[]
            {
                new FoundationResourceClassTruth(
                    Cpu,
                    new ResourceQuantity(total, "units"),
                    new ResourceQuantity(floor, "units"),
                    new ResourceQuantity(reserve, "units"),
                    evidence)
            },
            truthAvailable: true);
    }

    private static ApplicationResourceAllocation Allocation(
        string app,
        string grant,
        decimal allocation,
        decimal quota,
        decimal ceiling,
        string resource = "cpu",
        string unit = "units",
        ResourceEpochId? epoch = null,
        DateTimeOffset? evidenceTime = null,
        DateTimeOffset? effectiveFrom = null,
        DateTimeOffset? effectiveUntil = null,
        string evidenceId = "allocation-evidence")
    {
        var lifetime = effectiveUntil.HasValue
            ? new ResourceEffectiveLifetime(effectiveFrom ?? T0, effectiveUntil, explicitlyOpenEnded: false)
            : new ResourceEffectiveLifetime(effectiveFrom ?? T0, null, explicitlyOpenEnded: true);

        return new ApplicationResourceAllocation(
            new ResourceGrantId(grant),
            new ApplicationPrincipalId(app),
            new ResourceClassId(resource),
            new ResourceQuantity(allocation, unit),
            new ResourceQuantity(quota, unit),
            new ResourceQuantity(ceiling, unit),
            lifetime,
            new ResourceEvidenceReference(
                new ResourceEvidenceId(evidenceId),
                new ResourceScopeId("application-allocation"),
                evidenceTime ?? T0,
                epoch ?? Epoch));
    }

    private static ApplicationResourceAllocationSnapshot Snapshot(
        IEnumerable<ApplicationResourceAllocation> allocations,
        FoundationResourceTruthSnapshot? truth = null,
        DateTimeOffset? observedAt = null,
        bool available = true)
        => new(truth ?? Truth(), observedAt ?? T0, allocations, available);

    private static void PositiveSnapshot()
    {
        var snapshot = Snapshot(new[] { Allocation("app-a", "grant-a", 20m, 30m, 40m) });
        Assert(snapshot.Allocations.Count == 1, "Expected one allocation.");
    }

    private static void AllocationBelowQuotaBelowCeiling()
    {
        _ = Allocation("app-a", "grant-a", 10m, 20m, 30m);
    }

    private static void AllocationExceedsQuotaRejected()
        => ExpectThrows<ArgumentException>(() => Allocation("app-a", "grant-a", 31m, 30m, 40m));

    private static void QuotaExceedsCeilingRejected()
        => ExpectThrows<ArgumentException>(() => Allocation("app-a", "grant-a", 20m, 41m, 40m));

    private static void AllocationQuotaCeilingUnitMismatchRejected()
    {
        ExpectThrows<ArgumentException>(() => new ApplicationResourceAllocation(
            new ResourceGrantId("grant-a"),
            new ApplicationPrincipalId("app-a"),
            Cpu,
            new ResourceQuantity(10m, "units"),
            new ResourceQuantity(20m, "units"),
            new ResourceQuantity(30m, "bytes"),
            new ResourceEffectiveLifetime(T0, null, true),
            new ResourceEvidenceReference(new ResourceEvidenceId("e"), new ResourceScopeId("s"), T0, Epoch)));
    }

    private static void TruthUnitMismatchRejected()
        => ExpectThrows<ArgumentException>(() => Snapshot(new[] { Allocation("app-a", "grant-a", 10m, 20m, 30m, unit: "bytes") }));

    private static void UnknownResourceClassRejected()
        => ExpectThrows<KeyNotFoundException>(() => Snapshot(new[] { Allocation("app-a", "grant-a", 10m, 20m, 30m, resource: "gpu") }));

    private static void EvidenceEpochMismatchRejected()
        => ExpectThrows<ArgumentException>(() => Snapshot(new[] { Allocation("app-a", "grant-a", 10m, 20m, 30m, epoch: new ResourceEpochId("epoch-002")) }));

    private static void FutureEvidenceRejected()
        => ExpectThrows<ArgumentException>(() => Snapshot(new[] { Allocation("app-a", "grant-a", 10m, 20m, 30m, evidenceTime: T0.AddSeconds(1)) }));

    private static void FutureEffectiveAllocationRejected()
        => ExpectThrows<ArgumentException>(() => Snapshot(new[] { Allocation("app-a", "grant-a", 10m, 20m, 30m, effectiveFrom: T0.AddSeconds(1)) }));

    private static void ExpiredAllocationRejected()
        => ExpectThrows<ArgumentException>(() => Snapshot(
            new[] { Allocation("app-a", "grant-a", 10m, 20m, 30m, effectiveFrom: T0.AddMinutes(-10), effectiveUntil: T0.AddSeconds(-1)) }));

    private static void SnapshotPredatingResourceTruthRejected()
        => ExpectThrows<ArgumentException>(() => Snapshot(Array.Empty<ApplicationResourceAllocation>(), observedAt: T0.AddSeconds(-1)));

    private static void UnavailableAllocationTruthFailsClosed()
        => ExpectThrows<InvalidOperationException>(() => Snapshot(Array.Empty<ApplicationResourceAllocation>(), available: false));

    private static void DuplicateApplicationResourceBindingRejected()
        => ExpectThrows<ArgumentException>(() => Snapshot(new[]
        {
            Allocation("app-a", "grant-a", 10m, 10m, 10m),
            Allocation("app-a", "grant-b", 10m, 10m, 10m)
        }));

    private static void DuplicateGrantIdentityRejected()
        => ExpectThrows<ArgumentException>(() => Snapshot(new[]
        {
            Allocation("app-a", "grant-x", 10m, 10m, 10m),
            Allocation("app-b", "grant-x", 10m, 10m, 10m)
        }));

    private static void IndividualCeilingOverAllocatableRejected()
        => ExpectThrows<ArgumentException>(() => Snapshot(new[] { Allocation("app-a", "grant-a", 10m, 20m, 81m) }));

    private static void AggregateAllocationOverAllocatableRejected()
        => ExpectThrows<ArgumentException>(() => Snapshot(new[]
        {
            Allocation("app-a", "grant-a", 50m, 50m, 50m),
            Allocation("app-b", "grant-b", 31m, 31m, 31m)
        }));

    private static void AggregateQuotaOverAllocatableRejected()
        => ExpectThrows<ArgumentException>(() => Snapshot(new[]
        {
            Allocation("app-a", "grant-a", 20m, 50m, 50m),
            Allocation("app-b", "grant-b", 20m, 31m, 31m)
        }));

    private static void AggregateCeilingOverAllocatableRejected()
        => ExpectThrows<ArgumentException>(() => Snapshot(new[]
        {
            Allocation("app-a", "grant-a", 20m, 30m, 50m),
            Allocation("app-b", "grant-b", 20m, 30m, 31m)
        }));

    private static void ExactAllocatableBoundaryIsValid()
    {
        _ = Snapshot(new[]
        {
            Allocation("app-a", "grant-a", 40m, 40m, 40m),
            Allocation("app-b", "grant-b", 40m, 40m, 40m)
        });
    }

    private static void ZeroApplicationValidity()
    {
        var snapshot = Snapshot(Array.Empty<ApplicationResourceAllocation>());
        Assert(snapshot.Allocations.Count == 0, "Zero-Application Foundation must remain valid.");
    }

    private static void OrderingIsDeterministic()
    {
        var a = Allocation("app-a", "grant-a", 10m, 20m, 30m);
        var b = Allocation("app-b", "grant-b", 10m, 20m, 30m);
        var first = Snapshot(new[] { b, a });
        var second = Snapshot(new[] { a, b });
        Assert(first.IdentitySha256 == second.IdentitySha256, "Ordering must not affect identity.");
    }

    private static void IdentityRepeatIsDeterministic()
    {
        var allocation = Allocation("app-a", "grant-a", 10m, 20m, 30m);
        Assert(Snapshot(new[] { allocation }).IdentitySha256 == Snapshot(new[] { allocation }).IdentitySha256, "Identity must repeat exactly.");
    }

    private static void AllocationMutationChangesIdentity()
        => Assert(Snapshot(new[] { Allocation("app-a", "grant-a", 10m, 20m, 30m) }).IdentitySha256 !=
                  Snapshot(new[] { Allocation("app-a", "grant-a", 11m, 20m, 30m) }).IdentitySha256, "Allocation mutation must change identity.");

    private static void QuotaMutationChangesIdentity()
        => Assert(Snapshot(new[] { Allocation("app-a", "grant-a", 10m, 20m, 30m) }).IdentitySha256 !=
                  Snapshot(new[] { Allocation("app-a", "grant-a", 10m, 21m, 30m) }).IdentitySha256, "Quota mutation must change identity.");

    private static void CeilingMutationChangesIdentity()
        => Assert(Snapshot(new[] { Allocation("app-a", "grant-a", 10m, 20m, 30m) }).IdentitySha256 !=
                  Snapshot(new[] { Allocation("app-a", "grant-a", 10m, 20m, 31m) }).IdentitySha256, "Ceiling mutation must change identity.");

    private static void EvidenceMutationChangesIdentity()
        => Assert(Snapshot(new[] { Allocation("app-a", "grant-a", 10m, 20m, 30m, evidenceId: "evidence-a") }).IdentitySha256 !=
                  Snapshot(new[] { Allocation("app-a", "grant-a", 10m, 20m, 30m, evidenceId: "evidence-b") }).IdentitySha256, "Evidence mutation must change identity.");

    private static void LifetimeMutationChangesIdentity()
        => Assert(Snapshot(new[] { Allocation("app-a", "grant-a", 10m, 20m, 30m, effectiveFrom: T0) }).IdentitySha256 !=
                  Snapshot(new[] { Allocation("app-a", "grant-a", 10m, 20m, 30m, effectiveFrom: T0.AddSeconds(-1)) }).IdentitySha256, "Lifetime mutation must change identity.");

    private static void ResourceTruthIdentityMutationChangesIdentity()
    {
        var allocation = Allocation("app-a", "grant-a", 10m, 20m, 30m);
        var first = Snapshot(new[] { allocation }, Truth(total: 100m));
        var second = Snapshot(new[] { allocation }, Truth(total: 101m));
        Assert(first.IdentitySha256 != second.IdentitySha256, "Exact predecessor resource truth must be identity material.");
    }

    private static void IdentityIsUppercaseSha256()
    {
        var identity = Snapshot(Array.Empty<ApplicationResourceAllocation>()).IdentitySha256;
        Assert(identity.Length == 64 && identity.All(c => (c >= '0' && c <= '9') || (c >= 'A' && c <= 'F')), "Identity must be uppercase SHA-256 hex.");
    }

    private static void KnownApplicationResourceLookup()
    {
        var snapshot = Snapshot(new[] { Allocation("app-a", "grant-a", 10m, 20m, 30m) });
        Assert(snapshot.GetRequiredAllocation(new ApplicationPrincipalId("app-a"), Cpu).GrantId.Value == "grant-a", "Known allocation must resolve exactly.");
    }

    private static void UnknownApplicationResourceLookupFailsClosed()
        => ExpectThrows<KeyNotFoundException>(() => Snapshot(Array.Empty<ApplicationResourceAllocation>())
            .GetRequiredAllocation(new ApplicationPrincipalId("app-x"), Cpu));

    private static void ApplicationViewContainsOnlyOwnRecords()
    {
        var snapshot = Snapshot(new[]
        {
            Allocation("app-a", "grant-a", 10m, 20m, 30m),
            Allocation("app-b", "grant-b", 10m, 20m, 30m)
        });
        var view = snapshot.GetApplicationView(new ApplicationPrincipalId("app-a"));
        Assert(view.Allocations.Count == 1, "Application view must contain exactly its own record.");
        Assert(view.Allocations.All(item => item.ApplicationId.Value == "app-a"), "Cross-Application allocation leakage detected.");
    }

    private static void UnknownApplicationViewIsEmpty()
    {
        var snapshot = Snapshot(new[] { Allocation("app-a", "grant-a", 10m, 20m, 30m) });
        Assert(snapshot.GetApplicationView(new ApplicationPrincipalId("app-x")).Allocations.Count == 0, "Unknown Application view must disclose nothing.");
    }

    private static void ApplicationViewBindsSourceSnapshotIdentity()
    {
        var snapshot = Snapshot(new[] { Allocation("app-a", "grant-a", 10m, 20m, 30m) });
        Assert(snapshot.GetApplicationView(new ApplicationPrincipalId("app-a")).SourceSnapshotIdentitySha256 == snapshot.IdentitySha256, "View must bind exact source snapshot.");
    }

    private static void AllocationCollectionIsReadOnly()
    {
        var snapshot = Snapshot(new[] { Allocation("app-a", "grant-a", 10m, 20m, 30m) });
        var list = (IList<ApplicationResourceAllocation>)snapshot.Allocations;
        ExpectThrows<NotSupportedException>(() => list.Add(Allocation("app-b", "grant-b", 1m, 1m, 1m)));
    }

    private static void ApplicationViewCollectionIsReadOnly()
    {
        var snapshot = Snapshot(new[] { Allocation("app-a", "grant-a", 10m, 20m, 30m) });
        var list = (IList<ApplicationResourceAllocation>)snapshot.GetApplicationView(new ApplicationPrincipalId("app-a")).Allocations;
        ExpectThrows<NotSupportedException>(() => list.Clear());
    }

    private static void ProtectionFloorNotApplicationCapacity()
    {
        var truth = Truth();
        Assert(truth.GetRequired(Cpu).ProtectionFloorReclaimability == ResourceReclaimability.NonReclaimable, "Protection floor must remain non-reclaimable.");
        Assert(truth.GetRequired(Cpu).AllocatableCapacity.Amount == 80m, "Protection floor must remain outside Application capacity.");
    }

    private static void RecoveryReserveNotApplicationCapacity()
    {
        var truth = Truth();
        Assert(truth.GetRequired(Cpu).RecoveryReserveReclaimability == ResourceReclaimability.NonReclaimable, "Recovery reserve must remain non-reclaimable.");
        Assert(truth.GetRequired(Cpu).AllocatableCapacity.Amount == 80m, "Recovery reserve must remain outside Application capacity.");
    }

    private static void ProductionSurfaceHasNoTradingTerms()
    {
        var surface = PublicSurfaceText();
        foreach (var term in new[] { "Trading", "TARC", "Broker", "Strategy", "Market", "Order" })
        {
            Assert(!surface.Contains(term, StringComparison.OrdinalIgnoreCase), $"Production surface leaked business term '{term}'.");
        }
    }

    private static void ProductionSurfaceHasNoWp04PlusRuntimeTerms()
    {
        var surface = PublicSurfaceText();
        foreach (var term in new[] { "Preempt", "LoadShed", "Rebalance", "Redistribut", "ResourceRequest", "ResourceDecision", "TechnicalCriticality", "PriorityClass" })
        {
            Assert(!surface.Contains(term, StringComparison.OrdinalIgnoreCase), $"Production surface leaked later-WP runtime term '{term}'.");
        }
    }

    private static void AllocationSurfaceHasNoPriorityOrPressureFields()
    {
        var names = typeof(ApplicationResourceAllocation).GetProperties(BindingFlags.Instance | BindingFlags.Public).Select(p => p.Name).ToArray();
        Assert(!names.Any(name => name.Contains("Priority", StringComparison.OrdinalIgnoreCase) || name.Contains("Pressure", StringComparison.OrdinalIgnoreCase)), "WP-03 allocation must not own priority or pressure.");
    }

    private static void ApplicationIdentityDoesNotCreateAuthority()
    {
        var app = new ApplicationPrincipalId("app-a");
        Assert(app.Value == "app-a", "Application identity must remain a value identity only.");
        Assert(!typeof(ApplicationPrincipalId).GetMethods(BindingFlags.Instance | BindingFlags.Public).Any(m => m.Name.Contains("Authoriz", StringComparison.OrdinalIgnoreCase)), "Application identity must not mint authority.");
    }

    private static void GrantIdentityDoesNotCreateAuthority()
    {
        var grant = new ResourceGrantId("grant-a");
        Assert(grant.Value == "grant-a", "Grant identity must remain a value identity only.");
        Assert(!typeof(ResourceGrantId).GetMethods(BindingFlags.Instance | BindingFlags.Public).Any(m => m.Name.Contains("Authoriz", StringComparison.OrdinalIgnoreCase)), "Grant identity must not mint authority.");
    }

    private static void ResourceTruthRemainsSingularPredecessor()
    {
        var ctor = typeof(ApplicationResourceAllocationSnapshot).GetConstructors().Single();
        var truthParameters = ctor.GetParameters().Count(p => p.ParameterType == typeof(FoundationResourceTruthSnapshot));
        Assert(truthParameters == 1, "WP-03 must consume exactly one WP-02 resource-truth snapshot.");
    }

    private static string PublicSurfaceText()
    {
        var types = new[]
        {
            typeof(ApplicationResourceAllocation),
            typeof(ApplicationResourceAllocationSnapshot),
            typeof(ApplicationResourceAllocationView)
        };

        return string.Join("|", types.SelectMany(type =>
            type.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                .Select(member => type.FullName + "." + member.Name)));
    }

    private static void Run(string name, Action test)
    {
        try
        {
            test();
            Console.WriteLine($"PASS {name}");
        }
        catch (Exception ex)
        {
            _failures++;
            Console.WriteLine($"FAIL {name}: {ex.GetType().Name}: {ex.Message}");
        }
    }

    private static void Assert(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }

    private static void ExpectThrows<T>(Action action) where T : Exception
    {
        try
        {
            action();
        }
        catch (T)
        {
            return;
        }

        throw new InvalidOperationException($"Expected exception {typeof(T).Name} was not thrown.");
    }
}
