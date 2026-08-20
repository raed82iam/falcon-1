using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Falcon.Foundation.Architecture.Tests;

internal static partial class Program
{
    private static readonly bool Stage5WP05ArchitectureGuard = ValidateStage5WP05Architecture();

    private static bool ValidateStage5WP05Architecture()
    {
        var root = Path.GetFullPath(Directory.GetCurrentDirectory());
        var solution = Path.Combine(root, "Falcon.Foundation.ControlledProjectFoundation.slnx");
        var routingProject = Path.Combine(root, "src", "Foundation.MessageRouting", "Foundation.MessageRouting.csproj");
        var verifierProject = Path.Combine(root, "verification", "Falcon.Stage5.WP05.Verifier", "Falcon.Stage5.WP05.Verifier.csproj");

        if (!File.Exists(solution)) throw new InvalidOperationException("stage5_wp05_solution_missing");
        if (!File.Exists(routingProject)) throw new InvalidOperationException("stage5_wp05_routing_project_missing");
        if (!File.Exists(verifierProject)) throw new InvalidOperationException("stage5_wp05_verifier_project_missing");

        var solutionPaths = XDocument.Load(solution).Descendants("Project")
            .Select(x => ((string?)x.Attribute("Path") ?? string.Empty).Replace('\\', '/'))
            .ToArray();

        RequireExactlyOnce(solutionPaths, "src/Foundation.MessageRouting/Foundation.MessageRouting.csproj", "stage5_wp05_routing_solution_membership");
        RequireExactlyOnce(solutionPaths, "verification/Falcon.Stage5.WP05.Verifier/Falcon.Stage5.WP05.Verifier.csproj", "stage5_wp05_verifier_solution_membership");

        RequireExactReferences(routingProject, new[]
        {
            "src/Foundation.Contracts/Foundation.Contracts.csproj",
            "src/Foundation.ApplicationManifest/Foundation.ApplicationManifest.csproj",
            "src/Foundation.MessageAdmission/Foundation.MessageAdmission.csproj"
        }, root, "stage5_wp05_routing_reference_graph");

        RequireExactReferences(verifierProject, new[]
        {
            "src/Foundation.Contracts/Foundation.Contracts.csproj",
            "src/Foundation.SchemaRegistry/Foundation.SchemaRegistry.csproj",
            "src/Foundation.ApplicationManifest/Foundation.ApplicationManifest.csproj",
            "src/Foundation.Authority/Foundation.Authority.csproj",
            "src/Foundation.MessageAdmission/Foundation.MessageAdmission.csproj",
            "src/Foundation.MessageRouting/Foundation.MessageRouting.csproj"
        }, root, "stage5_wp05_verifier_reference_graph");

        var routingXml = File.ReadAllText(routingProject);
        if (!routingXml.Contains("<AssemblyName>Foundation.MessageRouting</AssemblyName>", StringComparison.Ordinal))
            throw new InvalidOperationException("stage5_wp05_routing_assembly_identity_invalid");
        if (!routingXml.Contains("<RootNamespace>Foundation.MessageRouting</RootNamespace>", StringComparison.Ordinal))
            throw new InvalidOperationException("stage5_wp05_routing_namespace_identity_invalid");
        if (routingXml.Contains("applications/", StringComparison.OrdinalIgnoreCase) || routingXml.Contains("applications\\", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("stage5_wp05_application_dependency_detected");

        return true;
    }

    private static void RequireExactlyOnce(IEnumerable<string> paths, string expected, string reason)
    {
        if (paths.Count(x => string.Equals(x, expected, StringComparison.OrdinalIgnoreCase)) != 1)
            throw new InvalidOperationException(reason);
    }

    private static void RequireExactReferences(string projectPath, IEnumerable<string> expectedRelative, string root, string reason)
    {
        var actual = XDocument.Load(projectPath).Descendants("ProjectReference")
            .Select(x => (string?)x.Attribute("Include") ?? string.Empty)
            .Select(x => Path.GetFullPath(Path.Combine(Path.GetDirectoryName(projectPath)!, x)))
            .Select(x => Path.GetRelativePath(root, x).Replace('\\', '/'))
            .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
            .ToArray();
        var expected = expectedRelative.OrderBy(x => x, StringComparer.OrdinalIgnoreCase).ToArray();

        if (!actual.SequenceEqual(expected, StringComparer.OrdinalIgnoreCase))
            throw new InvalidOperationException(reason + ":actual=" + string.Join(",", actual));
    }
}
