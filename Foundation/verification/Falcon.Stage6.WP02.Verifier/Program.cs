using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Foundation.Contracts.ResourceGovernance;
using Foundation.State.ResourceGovernance;

namespace Falcon.Stage6.WP02.Verifier;

internal static class Program
{
    private static int _scenarios;
    private static int _failures;
    private static readonly DateTimeOffset T0 = new(2026, 8, 8, 20, 0, 0, TimeSpan.Zero);

    private static readonly Type[] Wp02OwnedProductionTypes =
    {
        typeof(FoundationResourceClassTruth),
        typeof(FoundationResourceTruthSnapshot)
    };

    private static int Main()
    {
        Run("positive_snapshot", () => _ = Snapshot(Entry("cpu", 100m, 20m, 10m)));
        Run("allocatable_is_derived", () => Equal(70m, Snapshot(Entry("cpu", 100m, 20m, 10m)).GetRequired(new ResourceClassId("cpu")).AllocatableCapacity.Amount));
        Run("protection_floor_is_non_reclaimable", () => Equal(ResourceReclaimability.NonReclaimable, Entry("cpu", 100m, 20m, 10m).ProtectionFloorReclaimability));
        Run("recovery_reserve_is_non_reclaimable", () => Equal(ResourceReclaimability.NonReclaimable, Entry("cpu", 100m, 20m, 10m).RecoveryReserveReclaimability));
        Run("zero_allocatable_is_valid", () => Equal(0m, Snapshot(Entry("cpu", 100m, 60m, 40m)).Resources[0].AllocatableCapacity.Amount));
        Run("unit_mismatch_rejected", () => ExpectThrows<ArgumentException>(() => Entry("cpu", 100m, 20m, 10m, floorUnit: "cores", reserveUnit: "units")));
        Run("reserve_unit_mismatch_rejected", () => ExpectThrows<ArgumentException>(() => Entry("cpu", 100m, 20m, 10m, floorUnit: "units", reserveUnit: "cores")));
        Run("protected_overcommit_rejected", () => ExpectThrows<ArgumentException>(() => Entry("cpu", 100m, 60m, 50m)));
        Run("unavailable_truth_fails_closed", () => ExpectThrows<InvalidOperationException>(() => new FoundationResourceTruthSnapshot(Epoch(), T0, new[] { Entry("cpu", 100m, 20m, 10m) }, false)));
        Run("empty_truth_fails_closed", () => ExpectThrows<ArgumentException>(() => new FoundationResourceTruthSnapshot(Epoch(), T0, Array.Empty<FoundationResourceClassTruth>(), true)));
        Run("duplicate_resource_class_rejected", () => ExpectThrows<ArgumentException>(() => Snapshot(Entry("cpu", 100m, 20m, 10m), Entry("cpu", 200m, 20m, 10m))));
        Run("evidence_epoch_mismatch_rejected", () => ExpectThrows<ArgumentException>(() => new FoundationResourceTruthSnapshot(Epoch("epoch-1"), T0, new[] { Entry("cpu", 100m, 20m, 10m, evidenceEpoch: "epoch-2") }, true)));
        Run("future_evidence_rejected", () => ExpectThrows<ArgumentException>(() => new FoundationResourceTruthSnapshot(Epoch(), T0, new[] { Entry("cpu", 100m, 20m, 10m, evidenceObservedAt: T0.AddSeconds(1)) }, true)));
        Run("ordering_is_deterministic", () => Equal(Snapshot(Entry("cpu", 100m, 20m, 10m), Entry("memory", 200m, 40m, 20m)).IdentitySha256, Snapshot(Entry("memory", 200m, 40m, 20m), Entry("cpu", 100m, 20m, 10m)).IdentitySha256));
        Run("resources_are_sorted", () => Equal("cpu", Snapshot(Entry("memory", 200m, 40m, 20m), Entry("cpu", 100m, 20m, 10m)).Resources[0].ResourceClassId.Value));
        Run("total_mutation_changes_identity", () => NotEqual(Snapshot(Entry("cpu", 100m, 20m, 10m)).IdentitySha256, Snapshot(Entry("cpu", 101m, 20m, 10m)).IdentitySha256));
        Run("floor_mutation_changes_identity", () => NotEqual(Snapshot(Entry("cpu", 100m, 20m, 10m)).IdentitySha256, Snapshot(Entry("cpu", 100m, 21m, 10m)).IdentitySha256));
        Run("reserve_mutation_changes_identity", () => NotEqual(Snapshot(Entry("cpu", 100m, 20m, 10m)).IdentitySha256, Snapshot(Entry("cpu", 100m, 20m, 11m)).IdentitySha256));
        Run("evidence_id_mutation_changes_identity", () => NotEqual(Snapshot(Entry("cpu", 100m, 20m, 10m, evidenceId: "evidence-a")).IdentitySha256, Snapshot(Entry("cpu", 100m, 20m, 10m, evidenceId: "evidence-b")).IdentitySha256));
        Run("evidence_scope_mutation_changes_identity", () => NotEqual(Snapshot(Entry("cpu", 100m, 20m, 10m, evidenceScope: "scope-a")).IdentitySha256, Snapshot(Entry("cpu", 100m, 20m, 10m, evidenceScope: "scope-b")).IdentitySha256));
        Run("evidence_time_mutation_changes_identity", () => NotEqual(Snapshot(Entry("cpu", 100m, 20m, 10m, evidenceObservedAt: T0.AddSeconds(-2))).IdentitySha256, Snapshot(Entry("cpu", 100m, 20m, 10m, evidenceObservedAt: T0.AddSeconds(-1))).IdentitySha256));
        Run("snapshot_epoch_mutation_changes_identity", () => NotEqual(SnapshotWithEpoch("epoch-1", Entry("cpu", 100m, 20m, 10m, evidenceEpoch: "epoch-1")).IdentitySha256, SnapshotWithEpoch("epoch-2", Entry("cpu", 100m, 20m, 10m, evidenceEpoch: "epoch-2")).IdentitySha256));
        Run("snapshot_time_mutation_changes_identity", () => NotEqual(new FoundationResourceTruthSnapshot(Epoch(), T0, new[] { Entry("cpu", 100m, 20m, 10m) }, true).IdentitySha256, new FoundationResourceTruthSnapshot(Epoch(), T0.AddSeconds(1), new[] { Entry("cpu", 100m, 20m, 10m) }, true).IdentitySha256));
        Run("identity_is_uppercase_sha256", () => IsCanonicalSha256(Snapshot(Entry("cpu", 100m, 20m, 10m)).IdentitySha256));
        Run("known_resource_lookup", () => Equal("cpu", Snapshot(Entry("cpu", 100m, 20m, 10m)).GetRequired(new ResourceClassId("cpu")).ResourceClassId.Value));
        Run("unknown_resource_lookup_fails_closed", () => ExpectThrows<KeyNotFoundException>(() => Snapshot(Entry("cpu", 100m, 20m, 10m)).GetRequired(new ResourceClassId("memory"))));
        Run("resource_collection_is_read_only", () => ExpectThrows<NotSupportedException>(() => ((IList<FoundationResourceClassTruth>)Snapshot(Entry("cpu", 100m, 20m, 10m)).Resources).Add(Entry("memory", 100m, 20m, 10m))));
        Run("allocatable_not_caller_supplied", () => NoConstructorParameter(typeof(FoundationResourceClassTruth), "allocatable"));
        Run("reclaimability_not_caller_supplied", () => NoConstructorParameter(typeof(FoundationResourceClassTruth), "reclaim"));
        Run("availability_is_explicit", () => ConstructorHasParameter(typeof(FoundationResourceTruthSnapshot), "truthAvailable"));
        Run("snapshot_has_no_application_identity_input", () => NoConstructorParameter(typeof(FoundationResourceTruthSnapshot), "application"));
        Run("production_surface_has_no_trading_terms", NoTradingSpecificSurface);
        Run("production_surface_has_no_wp03_plus_runtime_terms", NoLaterWpRuntimeSurface);
        Run("zero_application_neutrality", () => _ = Snapshot(Entry("cpu", 100m, 20m, 10m)));

        Console.WriteLine();
        Console.WriteLine($"STAGE 6 WP-02 VERIFIER: {_scenarios - _failures}/{_scenarios} PASS");
        Console.WriteLine($"Failures: {_failures}");
        return _failures == 0 ? 0 : 1;
    }

    private static FoundationResourceTruthSnapshot Snapshot(params FoundationResourceClassTruth[] entries) =>
        new(Epoch(), T0, entries, true);

    private static FoundationResourceTruthSnapshot SnapshotWithEpoch(string epoch, FoundationResourceClassTruth entry) =>
        new(Epoch(epoch), T0, new[] { entry }, true);

    private static FoundationResourceClassTruth Entry(
        string resourceClass,
        decimal total,
        decimal floor,
        decimal reserve,
        string totalUnit = "units",
        string? floorUnit = null,
        string? reserveUnit = null,
        string evidenceId = "evidence-1",
        string evidenceScope = "foundation-resource-truth",
        string evidenceEpoch = "epoch-1",
        DateTimeOffset? evidenceObservedAt = null)
    {
        return new FoundationResourceClassTruth(
            new ResourceClassId(resourceClass),
            new ResourceQuantity(total, totalUnit),
            new ResourceQuantity(floor, floorUnit ?? totalUnit),
            new ResourceQuantity(reserve, reserveUnit ?? totalUnit),
            new ResourceEvidenceReference(
                new ResourceEvidenceId(evidenceId),
                new ResourceScopeId(evidenceScope),
                evidenceObservedAt ?? T0.AddSeconds(-1),
                Epoch(evidenceEpoch)));
    }

    private static ResourceEpochId Epoch(string value = "epoch-1") => new(value);

    private static void Run(string name, Action test)
    {
        _scenarios++;
        try
        {
            test();
            Console.WriteLine($"PASS {name}");
        }
        catch (Exception exception)
        {
            _failures++;
            Console.WriteLine($"FAIL {name}: {exception.GetType().Name}: {exception.Message}");
        }
    }

    private static void ExpectThrows<TException>(Action action) where TException : Exception
    {
        try
        {
            action();
        }
        catch (TException)
        {
            return;
        }

        throw new InvalidOperationException($"Expected {typeof(TException).Name}.");
    }

    private static void Equal<T>(T expected, T actual)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
        {
            throw new InvalidOperationException($"Expected '{expected}', got '{actual}'.");
        }
    }

    private static void NotEqual<T>(T left, T right)
    {
        if (EqualityComparer<T>.Default.Equals(left, right))
        {
            throw new InvalidOperationException("Expected values to differ.");
        }
    }

    private static void IsCanonicalSha256(string value)
    {
        if (value.Length != 64 || value.Any(character => !(character is >= '0' and <= '9' or >= 'A' and <= 'F')))
        {
            throw new InvalidOperationException("Identity is not canonical uppercase SHA-256.");
        }
    }

    private static void NoConstructorParameter(Type type, string token)
    {
        if (type.GetConstructors().SelectMany(constructor => constructor.GetParameters())
            .Any(parameter => (parameter.Name ?? string.Empty).Contains(token, StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException($"Constructor unexpectedly exposes '{token}' input.");
        }
    }

    private static void ConstructorHasParameter(Type type, string parameterName)
    {
        if (!type.GetConstructors().SelectMany(constructor => constructor.GetParameters())
            .Any(parameter => StringComparer.Ordinal.Equals(parameter.Name, parameterName) && !parameter.HasDefaultValue))
        {
            throw new InvalidOperationException($"Constructor must require explicit '{parameterName}'.");
        }
    }

    private static string[] Wp02OwnedPublicSurfaceMembers() =>
        Wp02OwnedProductionTypes
            .SelectMany(type => new[] { type.Name }
                .Concat(type.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                    .Select(member => member.Name)))
            .ToArray();

    private static void NoTradingSpecificSurface()
    {
        var banned = new[] { "TARC", "Trading", "FSATS", "Guardian" };
        var members = Wp02OwnedPublicSurfaceMembers();

        foreach (var token in banned)
        {
            if (members.Any(member => member.Contains(token, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException($"Application-specific token leaked into WP-02 production surface: {token}.");
            }
        }
    }

    private static void NoLaterWpRuntimeSurface()
    {
        var banned = new[] { "ApplicationGrant", "Quota", "ApplicationCeiling", "PriorityClass", "PressureDecision", "Preempt", "ResourceRequestHandler", "RebalanceEngine", "LoadShedding" };
        var members = Wp02OwnedPublicSurfaceMembers();

        foreach (var token in banned)
        {
            if (members.Any(member => member.Contains(token, StringComparison.OrdinalIgnoreCase)))
            {
                throw new InvalidOperationException($"Later-WP runtime surface leaked into WP-02-owned production types: {token}.");
            }
        }
    }
}
