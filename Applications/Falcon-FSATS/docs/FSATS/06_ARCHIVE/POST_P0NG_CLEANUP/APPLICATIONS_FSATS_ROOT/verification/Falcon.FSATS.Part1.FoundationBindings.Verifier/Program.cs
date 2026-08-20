using Falcon.FSATS.FoundationBindings;

namespace Falcon.FSATS.Part1.FoundationBindings.Verifier;

internal static class Program
{
    private static readonly List<string> Failures = new();
    private const int GateCount = 10;

    private static int Main()
    {
        Run("WP03_PROJECT_IDENTITY", () => Require(FsatsWp03ManifestBindings.AcceptedWp03.ProjectPath == "src/Foundation.ApplicationManifest/Foundation.ApplicationManifest.csproj", "wp03_project_identity_mismatch"));
        Run("WP03_ASSEMBLY_IDENTITY", () => Require(FsatsWp03ManifestBindings.AcceptedWp03.AssemblyName == "Foundation.ApplicationManifest", "wp03_assembly_identity_mismatch"));
        Run("WP03_PUBLIC_TYPE_IDENTITY", () => Require(FsatsWp03ManifestBindings.AcceptedWp03.PrimaryPublicType == "Foundation.ApplicationManifest.ApplicationCommunicationManifest", "wp03_public_type_mismatch"));
        Run("WP03_IMPLEMENTATION_COMMIT_PIN", () => Require(FsatsWp03ManifestBindings.AcceptedWp03.ImplementationCommit == "5b2998d4329b518d422e815a5fdd60015627f8d8", "wp03_commit_pin_mismatch"));
        Run("WP03_BLOB_PINS", VerifyBlobPins);
        Run("WP03_DIRECT_DEPENDENCIES", VerifyDirectDependencies);
        Run("CORE_APPLICATION_BINDING_COUNT_3", () => Require(FsatsWp03ManifestBindings.CoreApplications.Count == 3, "core_application_binding_count_mismatch"));
        Run("CORE_APPLICATION_BINDINGS_UNIQUE", VerifyApplicationBindingsUnique);
        Run("CORE_APPLICATIONS_SHARE_ACCEPTED_WP03", VerifySharedAcceptedWp03Identity);
        Run("BUILD_CONSUMPTION_NOT_CLAIMED", () => Require(FsatsWp03ManifestBindings.BuildConsumptionState == "DEFERRED_OUTSIDE_PART1_CURRENT_SCOPE", "build_consumption_authority_leak"));

        if (Failures.Count == 0)
        {
            Console.WriteLine($"FSATS_P1E_FOUNDATION_BINDINGS_VERIFIER_PASS {GateCount}/{GateCount}");
            return 0;
        }

        Console.Error.WriteLine($"FSATS_P1E_FOUNDATION_BINDINGS_VERIFIER_FAIL {GateCount - Failures.Count}/{GateCount}");
        foreach (var failure in Failures)
        {
            Console.Error.WriteLine(failure);
        }

        return 1;
    }

    private static void Run(string name, Action verification)
    {
        try
        {
            verification();
            Console.WriteLine($"PASS {name}");
        }
        catch (Exception ex)
        {
            Failures.Add($"FAIL {name}: {ex.GetType().Name}: {ex.Message}");
        }
    }

    private static void VerifyBlobPins()
    {
        Require(FsatsWp03ManifestBindings.AcceptedWp03.ProjectBlob == "d086d03af1a0e5bffd45e02e6813cfdd7511dd62", "wp03_project_blob_mismatch");
        Require(FsatsWp03ManifestBindings.AcceptedWp03.SourceBlob == "556cf7ac3511e1ea614a61d5e070a4645c0377bf", "wp03_source_blob_mismatch");
    }

    private static void VerifyDirectDependencies()
    {
        var expected = new[] { "Foundation.Contracts", "Foundation.SchemaRegistry" };
        Require(FsatsWp03ManifestBindings.AcceptedDirectFoundationDependencies.SequenceEqual(expected, StringComparer.Ordinal), "wp03_direct_dependency_mismatch");
    }

    private static void VerifyApplicationBindingsUnique()
    {
        var applications = FsatsWp03ManifestBindings.CoreApplications.Select(x => x.ApplicationId.Value).ToArray();
        var packages = FsatsWp03ManifestBindings.CoreApplications.Select(x => x.PackageId.Value).ToArray();
        Require(applications.Distinct(StringComparer.Ordinal).Count() == applications.Length, "duplicate_application_binding");
        Require(packages.Distinct(StringComparer.Ordinal).Count() == packages.Length, "duplicate_package_binding");
    }

    private static void VerifySharedAcceptedWp03Identity()
    {
        foreach (var binding in FsatsWp03ManifestBindings.CoreApplications)
        {
            Require(ReferenceEquals(binding.FoundationIdentity, FsatsWp03ManifestBindings.AcceptedWp03), $"noncanonical_wp03_binding:{binding.ApplicationId.Value}");
            Require(binding.ApplicationVersion.Value == "1.4.0", $"unexpected_application_version:{binding.ApplicationId.Value}");
        }
    }

    private static void Require(bool condition, string reason)
    {
        if (!condition)
        {
            throw new InvalidOperationException(reason);
        }
    }
}
