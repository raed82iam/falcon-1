using System.Collections;
using System.Runtime.CompilerServices;
using TA = Falcon.FSATS.Trading.Application;
using PA = Falcon.FSATS.FSAPMA.Application;
using GA = Falcon.FSATS.TradingGuardian.Application;
using SA = Falcon.FSATS.FSTSimA.Application;
using RA = Falcon.FSATS.ResourceManagement.Application;

internal static class ManifestGovernanceAdversarialChecks
{
    private static readonly string[] RequiredStringProperties =
    {
        "ApplicationId", "PackageId", "Version", "Owner", "Purpose", "PackageProvenance", "PackageIntegrityPolicy",
        "CompatibilityPolicy", "LifecycleState", "OwnedBusinessBoundary", "MsaId", "CsaEligibilityPolicy", "SecurityProfile",
        "ResourcePolicy", "PersistencePolicy", "CommunicationPolicy", "ConfigurationPolicy", "EvidencePolicy", "LifecyclePolicy",
        "HealthPolicy", "FailureContainmentPolicy", "SelfDevelopmentPolicy", "GuardianRequirement", "ProtectionInterface", "RollbackPlan",
        "SafetyContinuityPolicy", "AiRepairRecoveryPolicy", "ReplacementRemovalPolicy"
    };

    private static readonly string[] RequiredCollectionProperties =
    {
        "LsaIds", "DeclaredDependencies", "RequiredFoundationCapabilities", "RequiredFoundationContracts", "ProvidedCapabilities",
        "DeclaredConsumers", "Permissions", "AuthorityRequests"
    };

    [ModuleInitializer]
    internal static void Run()
    {
        CheckManifest(TA.TradingManifest.Current, "FSATS-TRADING");
        CheckManifest(PA.FSAPMAManifest.Current, "FSATS-FSAPMA");
        CheckManifest(GA.TradingGuardianManifest.Current, "FSATS-TRADING-GUARDIAN");
        CheckManifest(SA.FSTSimAManifest.Current, "FSATS-FSTSIMA");
        CheckManifest(RA.ResourceManagementManifest.Current, "APP-RSC");
    }

    private static void CheckManifest(object manifest, string expectedApplicationId)
    {
        var type = manifest.GetType();
        var actualId = type.GetProperty("ApplicationId")?.GetValue(manifest) as string;
        if (!StringComparer.Ordinal.Equals(actualId, expectedApplicationId))
            throw new InvalidOperationException($"MANIFEST_APPLICATION_ID_MISMATCH:{expectedApplicationId}");

        foreach (var propertyName in RequiredStringProperties)
        {
            var property = type.GetProperty(propertyName) ?? throw new InvalidOperationException($"MANIFEST_PROPERTY_MISSING:{expectedApplicationId}:{propertyName}");
            var value = property.GetValue(manifest) as string;
            if (string.IsNullOrWhiteSpace(value) || StringComparer.Ordinal.Equals(value, "UNDECLARED"))
                throw new InvalidOperationException($"MANIFEST_PROPERTY_UNDECLARED:{expectedApplicationId}:{propertyName}");
        }

        foreach (var propertyName in RequiredCollectionProperties)
        {
            var property = type.GetProperty(propertyName) ?? throw new InvalidOperationException($"MANIFEST_COLLECTION_MISSING:{expectedApplicationId}:{propertyName}");
            if (property.GetValue(manifest) is not IEnumerable values || !values.Cast<object>().Any())
                throw new InvalidOperationException($"MANIFEST_COLLECTION_EMPTY:{expectedApplicationId}:{propertyName}");
        }

        foreach (var property in type.GetProperties().Where(p => typeof(IReadOnlyList<string>).IsAssignableFrom(p.PropertyType)))
        {
            var value = property.GetValue(manifest);
            if (value is string[])
                throw new InvalidOperationException($"MANIFEST_COLLECTION_ARRAY_EXPOSED:{expectedApplicationId}:{property.Name}");
            if (value is IList<string> list && list.Count > 0)
            {
                var original = list[0];
                try
                {
                    list[0] = original + "-MUTATED";
                    throw new InvalidOperationException($"MANIFEST_COLLECTION_MUTATION_SUCCEEDED:{expectedApplicationId}:{property.Name}");
                }
                catch (NotSupportedException)
                {
                }
            }
        }

        var resource = type.GetProperty("ResourceProfile")?.GetValue(manifest)
            ?? throw new InvalidOperationException($"MANIFEST_RESOURCE_PROFILE_MISSING:{expectedApplicationId}");
        foreach (var property in resource.GetType().GetProperties())
        {
            var value = property.GetValue(resource) as string;
            if (string.IsNullOrWhiteSpace(value) || StringComparer.Ordinal.Equals(value, "UNDECLARED"))
                throw new InvalidOperationException($"MANIFEST_RESOURCE_PROPERTY_UNDECLARED:{expectedApplicationId}:{property.Name}");
        }

        var runtime = type.GetProperty("RuntimeAuthorized")?.GetValue(manifest);
        if (runtime is not false)
            throw new InvalidOperationException($"MANIFEST_RUNTIME_AUTHORITY_ESCALATED:{expectedApplicationId}");
    }
}
