using System.Text.RegularExpressions;
using Falcon.FSATS.Primitives;

namespace Falcon.FSATS.FoundationBindings;

public sealed record AcceptedFoundationWp03Identity
{
    public AcceptedFoundationWp03Identity(
        string projectPath,
        string assemblyName,
        string primaryPublicType,
        string implementationCommit,
        string projectBlob,
        string sourceBlob,
        string ownerClosureRecordPath)
    {
        ProjectPath = RequirePath(projectPath, nameof(projectPath));
        AssemblyName = RequireToken(assemblyName, nameof(assemblyName));
        PrimaryPublicType = RequireToken(primaryPublicType, nameof(primaryPublicType));
        ImplementationCommit = RequireGitObjectId(implementationCommit, nameof(implementationCommit));
        ProjectBlob = RequireGitObjectId(projectBlob, nameof(projectBlob));
        SourceBlob = RequireGitObjectId(sourceBlob, nameof(sourceBlob));
        OwnerClosureRecordPath = RequirePath(ownerClosureRecordPath, nameof(ownerClosureRecordPath));
    }

    public string ProjectPath { get; }
    public string AssemblyName { get; }
    public string PrimaryPublicType { get; }
    public string ImplementationCommit { get; }
    public string ProjectBlob { get; }
    public string SourceBlob { get; }
    public string OwnerClosureRecordPath { get; }

    private static string RequireGitObjectId(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value) || !Regex.IsMatch(value, "^[0-9a-f]{40}$", RegexOptions.CultureInvariant))
        {
            throw new ArgumentException("git_object_id_required", parameterName);
        }

        return value;
    }

    private static string RequirePath(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value) || !string.Equals(value, value.Trim(), StringComparison.Ordinal) || value.StartsWith("/", StringComparison.Ordinal) || value.Contains("..", StringComparison.Ordinal))
        {
            throw new ArgumentException("canonical_repository_path_required", parameterName);
        }

        return value;
    }

    private static string RequireToken(string value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value) || !string.Equals(value, value.Trim(), StringComparison.Ordinal))
        {
            throw new ArgumentException("canonical_token_required", parameterName);
        }

        return value;
    }
}

public sealed record ApplicationManifestDesignBinding
{
    public ApplicationManifestDesignBinding(
        FsatsApplicationId applicationId,
        PackageId packageId,
        VersionId applicationVersion,
        AcceptedFoundationWp03Identity foundationIdentity)
    {
        ApplicationId = applicationId ?? throw new ArgumentNullException(nameof(applicationId));
        PackageId = packageId ?? throw new ArgumentNullException(nameof(packageId));
        ApplicationVersion = applicationVersion ?? throw new ArgumentNullException(nameof(applicationVersion));
        FoundationIdentity = foundationIdentity ?? throw new ArgumentNullException(nameof(foundationIdentity));
    }

    public FsatsApplicationId ApplicationId { get; }
    public PackageId PackageId { get; }
    public VersionId ApplicationVersion { get; }
    public AcceptedFoundationWp03Identity FoundationIdentity { get; }
}

public static class FsatsWp03ManifestBindings
{
    public static AcceptedFoundationWp03Identity AcceptedWp03 { get; } = new(
        "src/Foundation.ApplicationManifest/Foundation.ApplicationManifest.csproj",
        "Foundation.ApplicationManifest",
        "Foundation.ApplicationManifest.ApplicationCommunicationManifest",
        "5b2998d4329b518d422e815a5fdd60015627f8d8",
        "d086d03af1a0e5bffd45e02e6813cfdd7511dd62",
        "556cf7ac3511e1ea614a61d5e070a4645c0377bf",
        "docs/canonical-records/owner-decisions/stage5/Stage5-WP03-Owner-Acceptance-And-Closure-20260807-204800/OWNER-ACCEPTANCE-AND-CLOSURE-STAGE5-WP03.txt");

    public static IReadOnlyList<string> AcceptedDirectFoundationDependencies { get; } = Array.AsReadOnly(
        new[]
        {
            "Foundation.Contracts",
            "Foundation.SchemaRegistry"
        });

    public static IReadOnlyList<ApplicationManifestDesignBinding> CoreApplications { get; } = Array.AsReadOnly(
        new[]
        {
            new ApplicationManifestDesignBinding(
                new FsatsApplicationId("falcon.trading.guardian"),
                new PackageId("falcon.trading.guardian.package"),
                new VersionId("1.4.0"),
                AcceptedWp03),
            new ApplicationManifestDesignBinding(
                new FsatsApplicationId("falcon.trading.fsapma"),
                new PackageId("falcon.trading.fsapma.package"),
                new VersionId("1.4.0"),
                AcceptedWp03),
            new ApplicationManifestDesignBinding(
                new FsatsApplicationId("falcon.trading.application"),
                new PackageId("falcon.trading.application.package"),
                new VersionId("1.4.0"),
                AcceptedWp03)
        });

    public const string BindingState = "FOUNDATION_IDENTITY_BOUND";
    public const string BuildConsumptionState = "DEFERRED_OUTSIDE_PART1_CURRENT_SCOPE";
}
