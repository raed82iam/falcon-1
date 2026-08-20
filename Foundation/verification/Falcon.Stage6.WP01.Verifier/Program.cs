using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Foundation.Contracts.ResourceGovernance;

namespace Falcon.Stage6.WP01.Verifier;

internal static class Program
{
    private static int _passed;
    private static int _failed;

    private static int Main()
    {
        Run("valid_resource_class_identity", () => Equal("cpu.core", new ResourceClassId("cpu.core").Value));
        Run("blank_resource_class_rejected", () => Throws<ArgumentException>(() => new ResourceClassId("")));
        Run("whitespace_resource_class_rejected", () => Throws<ArgumentException>(() => new ResourceClassId(" cpu.core")));
        Run("embedded_whitespace_identifier_rejected", () => Throws<ArgumentException>(() => new ResourceClassId("cpu core")));
        Run("valid_application_identity", () => Equal("application.alpha", new ApplicationPrincipalId("application.alpha").Value));
        Run("blank_application_identity_rejected", () => Throws<ArgumentException>(() => new ApplicationPrincipalId(" ")));
        Run("grant_identity_validation", () => Equal("grant.1", new ResourceGrantId("grant.1").Value));
        Run("request_identity_validation", () => Equal("request.1", new ResourceRequestId("request.1").Value));
        Run("decision_identity_validation", () => Equal("decision.1", new ResourceDecisionId("decision.1").Value));
        Run("evidence_identity_validation", () => Equal("evidence.1", new ResourceEvidenceId("evidence.1").Value));
        Run("correlation_causation_distinct", () => Require(typeof(CorrelationId) != typeof(CausationId), "Correlation and causation types must remain distinct."));
        Run("epoch_identity_validation", () => Equal("epoch.1", new ResourceEpochId("epoch.1").Value));
        Run("priority_identity_is_value_only", () =>
        {
            var value = new ResourcePriorityClassId("priority.highest-application");
            Equal("priority.highest-application", value.Value);
            Require(value.GetType().GetProperties().All(property => !property.Name.Contains("Authority", StringComparison.OrdinalIgnoreCase)), "Priority identifier must not expose authority state.");
        });
        Run("criticality_identity_is_value_only", () =>
        {
            var value = new TechnicalCriticalityClassId("criticality.protection");
            Equal("criticality.protection", value.Value);
            Require(typeof(ResourcePriorityClassId) != typeof(TechnicalCriticalityClassId), "Application priority and technical criticality must remain distinct types.");
        });
        Run("negative_quantity_rejected", () => Throws<ArgumentOutOfRangeException>(() => new ResourceQuantity(-1m, "cores")));
        Run("missing_unit_rejected", () => Throws<ArgumentException>(() => new ResourceQuantity(1m, "")));
        Run("zero_quantity_valid", () => Equal(0m, new ResourceQuantity(0m, "bytes").Amount));
        Run("quantity_unit_preserved", () => Equal("MiB/s", new ResourceQuantity(1m, "MiB/s").Unit));
        Run("quantity_invariant_format", () =>
        {
            var original = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("ar-SA");
                Equal("12.5|cores", new ResourceQuantity(12.5m, "cores").ToCanonicalString());
            }
            finally
            {
                CultureInfo.CurrentCulture = original;
            }
        });
        Run("pressure_enum_exact", () => Equal("Normal,Constrained,Degraded,Critical", string.Join(',', Enum.GetNames<ResourcePressureState>())));
        Run("decision_enum_exact", () => Equal("Grant,PartialGrant,Cap,Deny,Defer,Revoke,Reduce,Restore", string.Join(',', Enum.GetNames<ResourceDecisionKind>())));
        Run("reclaimability_enum_exact", () => Equal("Reclaimable,NonReclaimable,Temporary", string.Join(',', Enum.GetNames<ResourceReclaimability>())));
        Run("invalid_lifetime_rejected", () =>
        {
            var now = DateTimeOffset.Parse("2026-08-08T00:00:00Z", CultureInfo.InvariantCulture);
            Throws<ArgumentException>(() => new ResourceEffectiveLifetime(now, now.AddMinutes(-1), false));
        });
        Run("implicit_open_ended_lifetime_rejected", () =>
        {
            var now = DateTimeOffset.Parse("2026-08-08T00:00:00Z", CultureInfo.InvariantCulture);
            Throws<ArgumentException>(() => new ResourceEffectiveLifetime(now, null, false));
        });
        Run("explicit_open_ended_lifetime", () =>
        {
            var now = DateTimeOffset.Parse("2026-08-08T00:00:00Z", CultureInfo.InvariantCulture);
            var lifetime = new ResourceEffectiveLifetime(now, null, true);
            Require(lifetime.ExplicitlyOpenEnded && lifetime.EffectiveUntil is null, "Open-ended lifetime must be explicit.");
        });
        Run("bounded_and_open_ended_rejected", () =>
        {
            var now = DateTimeOffset.Parse("2026-08-08T00:00:00Z", CultureInfo.InvariantCulture);
            Throws<ArgumentException>(() => new ResourceEffectiveLifetime(now, now.AddMinutes(1), true));
        });
        Run("evidence_reference_complete", () =>
        {
            var reference = new ResourceEvidenceReference(
                new ResourceEvidenceId("evidence.resource.1"),
                new ResourceScopeId("scope.application.alpha"),
                DateTimeOffset.Parse("2026-08-08T00:00:00Z", CultureInfo.InvariantCulture),
                new ResourceEpochId("epoch.42"));
            Equal("evidence.resource.1", reference.EvidenceId.Value);
            Equal("scope.application.alpha", reference.ScopeId.Value);
            Equal("epoch.42", reference.EpochId.Value);
        });
        Run("evidence_reference_null_rejected", () => Throws<ArgumentNullException>(() => new ResourceEvidenceReference(null!, new ResourceScopeId("scope.a"), DateTimeOffset.UtcNow, new ResourceEpochId("epoch.1"))));
        Run("deterministic_identity_repeat", () =>
        {
            var fields = new[] { new CanonicalIdentityField("b", "2"), new CanonicalIdentityField("a", "1") };
            Equal(CanonicalResourceIdentity.ComputeSha256(fields), CanonicalResourceIdentity.ComputeSha256(fields));
        });
        Run("deterministic_identity_order_independent", () =>
        {
            var first = CanonicalResourceIdentity.ComputeSha256(new[] { new CanonicalIdentityField("a", "1"), new CanonicalIdentityField("b", "2") });
            var second = CanonicalResourceIdentity.ComputeSha256(new[] { new CanonicalIdentityField("b", "2"), new CanonicalIdentityField("a", "1") });
            Equal(first, second);
        });
        Run("deterministic_identity_material_change", () =>
        {
            var first = CanonicalResourceIdentity.ComputeSha256(new[] { new CanonicalIdentityField("a", "1") });
            var second = CanonicalResourceIdentity.ComputeSha256(new[] { new CanonicalIdentityField("a", "2") });
            Require(!StringComparer.Ordinal.Equals(first, second), "Material change must change identity.");
        });
        Run("delimiter_collision_resistance_fixture", () =>
        {
            var first = CanonicalResourceIdentity.ComputeSha256(new[] { new CanonicalIdentityField("a", "bc"), new CanonicalIdentityField("d", "e") });
            var second = CanonicalResourceIdentity.ComputeSha256(new[] { new CanonicalIdentityField("a", "b"), new CanonicalIdentityField("c", "de") });
            Require(!StringComparer.Ordinal.Equals(first, second), "Length-delimited identity material must resist delimiter collisions.");
        });
        Run("null_empty_identity_material_distinct", () =>
        {
            var nullValue = CanonicalResourceIdentity.ComputeSha256(new[] { new CanonicalIdentityField("a", null) });
            var emptyValue = CanonicalResourceIdentity.ComputeSha256(new[] { new CanonicalIdentityField("a", string.Empty) });
            Require(!StringComparer.Ordinal.Equals(nullValue, emptyValue), "Null and empty identity values must remain distinct.");
        });
        Run("duplicate_identity_field_rejected", () => Throws<ArgumentException>(() => CanonicalResourceIdentity.ComputeSha256(new[] { new CanonicalIdentityField("a", "1"), new CanonicalIdentityField("a", "2") })));
        Run("empty_identity_field_set_rejected", () => Throws<ArgumentException>(() => CanonicalResourceIdentity.ComputeSha256(Array.Empty<CanonicalIdentityField>())));
        Run("quantity_field_is_invariant", () =>
        {
            var field = CanonicalResourceIdentity.QuantityField("quantity", new ResourceQuantity(10.2500m, "cores"));
            Equal("10.25|cores", field.Value);
        });
        Run("request_and_grant_types_distinct", () => Require(typeof(ResourceRequestId) != typeof(ResourceGrantId), "Request and grant identities must be distinct types."));
        Run("no_authority_from_quantity", () => Require(typeof(ResourceQuantity).GetProperties().All(property => !property.Name.Contains("Authority", StringComparison.OrdinalIgnoreCase)), "Quantity cannot mint authority."));
        Run("no_authority_from_priority_value", () => Require(typeof(ResourcePriorityClassId).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).All(method => !method.Name.Contains("Authorize", StringComparison.OrdinalIgnoreCase)), "Priority value cannot authorize itself."));
        Run("decision_identity_not_result", () => Require(typeof(ResourceDecisionId).GetProperties().All(property => property.PropertyType != typeof(ResourceDecisionKind)), "Decision identity must not embed a result."));
        Run("temporary_is_distinct", () => Require(ResourceReclaimability.Temporary != ResourceReclaimability.NonReclaimable, "Temporary capacity must remain distinct from permanent/non-reclaimable entitlement."));
        Run("pressure_not_authority", () => Require(!typeof(ResourcePressureState).GetEnumNames().Any(name => name.Contains("Grant", StringComparison.OrdinalIgnoreCase) || name.Contains("Allow", StringComparison.OrdinalIgnoreCase)), "Pressure state cannot imply grant authority."));
        Run("generic_control_plane_scope", () => Equal("foundation.control-plane", new ResourceScopeId("foundation.control-plane").Value));
        Run("resource_class_is_extensible_identifier", () => Require(!typeof(ResourceClassId).IsEnum, "Resource class must remain extensible and not be a closed enum."));
        Run("application_neutral_public_surface", () =>
        {
            var forbidden = new[] { "Trading", "FSATS", "Accounting", "Warehouse", "Strategy", "Market", "Broker", "Position", "Order" };
            var exportedNames = typeof(ResourceClassId).Assembly.GetExportedTypes().Where(type => type.Namespace?.StartsWith("Foundation.Contracts.ResourceGovernance", StringComparison.Ordinal) == true).Select(type => type.FullName ?? type.Name).ToArray();
            foreach (var token in forbidden)
            {
                Require(exportedNames.All(name => !name.Contains(token, StringComparison.OrdinalIgnoreCase)), $"Application/business token leaked into public surface: {token}");
            }
        });
        Run("no_later_wp_runtime_engine", () =>
        {
            var forbidden = new[] { "Allocator", "AllocationEngine", "PressureEngine", "Reclaimer", "Redistributor", "RebalanceEngine", "ResourceManager" };
            var exportedNames = typeof(ResourceClassId).Assembly.GetExportedTypes().Where(type => type.Namespace?.StartsWith("Foundation.Contracts.ResourceGovernance", StringComparison.Ordinal) == true).Select(type => type.Name).ToArray();
            foreach (var token in forbidden)
            {
                Require(exportedNames.All(name => !name.Contains(token, StringComparison.OrdinalIgnoreCase)), $"Later-WP engine leaked into WP-01: {token}");
            }
        });
        Run("no_artifact_consumption_mechanics", () =>
        {
            var forbidden = new[] { "PackageFeed", "ArtifactResolver", "NuGet", "BuildConsumption" };
            var exportedNames = typeof(ResourceClassId).Assembly.GetExportedTypes().Where(type => type.Namespace?.StartsWith("Foundation.Contracts.ResourceGovernance", StringComparison.Ordinal) == true).Select(type => type.Name).ToArray();
            foreach (var token in forbidden)
            {
                Require(exportedNames.All(name => !name.Contains(token, StringComparison.OrdinalIgnoreCase)), $"Artifact-consumption mechanic leaked into WP-01: {token}");
            }
        });
        Run("no_egress_or_credentials", () =>
        {
            var forbidden = new[] { "Credential", "Egress", "Internet", "BrokerConnection", "ProviderConnection" };
            var exportedNames = typeof(ResourceClassId).Assembly.GetExportedTypes().Where(type => type.Namespace?.StartsWith("Foundation.Contracts.ResourceGovernance", StringComparison.Ordinal) == true).Select(type => type.Name).ToArray();
            foreach (var token in forbidden)
            {
                Require(exportedNames.All(name => !name.Contains(token, StringComparison.OrdinalIgnoreCase)), $"External-connectivity surface leaked into WP-01: {token}");
            }
        });
        Run("zero_application_validity", () =>
        {
            var resource = new ResourceClassId("resource.generic");
            var quantity = new ResourceQuantity(0m, "units");
            Require(resource.Value.Length > 0 && quantity.Amount == 0m, "Primitives must remain usable with zero installed Applications.");
        });
        Run("immutable_public_primitives", () =>
        {
            var types = new[] { typeof(ResourceQuantity), typeof(ResourceEffectiveLifetime), typeof(ResourceEvidenceReference), typeof(CanonicalIdentityField) };
            foreach (var type in types)
            {
                foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    Require(property.SetMethod is null, $"Mutable public property found: {type.Name}.{property.Name}");
                }
            }
        });
        Run("malformed_primitives_fail_closed", () =>
        {
            Throws<ArgumentException>(() => new ResourcePriorityClassId("bad priority"));
            Throws<ArgumentException>(() => new ResourceQuantity(1m, "bad unit"));
        });

        Console.WriteLine();
        Console.WriteLine($"STAGE 6 WP-01 VERIFIER: {_passed}/{_passed + _failed} PASS");
        return _failed == 0 ? 0 : 1;
    }

    private static void Run(string name, Action test)
    {
        try
        {
            test();
            _passed++;
            Console.WriteLine($"PASS {name}");
        }
        catch (Exception exception)
        {
            _failed++;
            Console.WriteLine($"FAIL {name}: {exception.GetType().Name}: {exception.Message}");
        }
    }

    private static void Equal<T>(T expected, T actual)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
        {
            throw new InvalidOperationException($"Expected '{expected}' but found '{actual}'.");
        }
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }

    private static void Throws<TException>(Action action) where TException : Exception
    {
        try
        {
            action();
        }
        catch (TException)
        {
            return;
        }

        throw new InvalidOperationException($"Expected exception {typeof(TException).Name}.");
    }
}
