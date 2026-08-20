using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace Falcon.Foundation.Architecture.Tests;

internal static class Stage8Wp01ArchitectureGuard
{
    [ModuleInitializer]
    internal static void VerifyStage8Wp01Boundaries()
    {
        var root = Path.GetFullPath(Directory.GetCurrentDirectory());
        var solutionPath = Path.Combine(root, "Falcon.Foundation.ControlledProjectFoundation.slnx");
        var guardianProject = Path.Combine(root, "src", "Foundation.Guardian", "Foundation.Guardian.csproj");
        var verifierProject = Path.Combine(root, "verification", "Falcon.Stage8.WP01.Verifier", "Falcon.Stage8.WP01.Verifier.csproj");

        if (!File.Exists(solutionPath) || !File.Exists(guardianProject) || !File.Exists(verifierProject))
            throw new InvalidOperationException("Stage 8 WP-01 Guardian surface is incomplete.");

        RequireExactReferences(
            guardianProject,
            new[] { Path.Combine(root, "src", "Foundation.Contracts", "Foundation.Contracts.csproj") },
            "Foundation.Guardian");

        RequireExactReferences(
            verifierProject,
            new[] { guardianProject },
            "Falcon.Stage8.WP01.Verifier");

        var solution = XDocument.Load(solutionPath);
        var paths = solution.Root?.Elements("Project")
            .Select(e => (e.Attribute("Path")?.Value ?? string.Empty).Replace('\\', '/'))
            .ToArray() ?? Array.Empty<string>();

        RequireExactlyOnce(paths, "src/Foundation.Guardian/Foundation.Guardian.csproj");
        RequireExactlyOnce(paths, "verification/Falcon.Stage8.WP01.Verifier/Falcon.Stage8.WP01.Verifier.csproj");

        if (paths.Any(path => path.StartsWith("applications/", StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("Stage 8 controlled solution contains Application-owned project.");

        if (paths.Any(path => path.StartsWith("reference/", StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("Stage 8 controlled solution contains reference-owned project.");

        var guardianSource = File.ReadAllText(Path.Combine(root, "src", "Foundation.Guardian", "GuardianProtectionPrimitives.cs"));
        foreach (var forbidden in new[]
                 {
                     "using Foundation.ApplicationLifecycle",
                     "using Foundation.Authority",
                     "using Foundation.Recovery",
                     "using Foundation.SelfAwareness"
                 })
        {
            if (guardianSource.Contains(forbidden, StringComparison.Ordinal))
                throw new InvalidOperationException("Stage 8 WP-01 premature dependency detected: " + forbidden);
        }
    }

    private static void RequireExactReferences(string projectPath, string[] expected, string label)
    {
        var project = XDocument.Load(projectPath);
        var directory = Path.GetDirectoryName(projectPath)!;
        var actual = project.Root?.Elements("ItemGroup").Elements("ProjectReference")
            .Select(r => Path.GetFullPath(Path.Combine(directory, r.Attribute("Include")?.Value ?? string.Empty)))
            .OrderBy(v => v, StringComparer.Ordinal)
            .ToArray() ?? Array.Empty<string>();

        var expectedSorted = expected.Select(Path.GetFullPath)
            .OrderBy(v => v, StringComparer.Ordinal)
            .ToArray();

        if (!actual.SequenceEqual(expectedSorted, StringComparer.Ordinal))
            throw new InvalidOperationException("Stage 8 WP-01 changed exact project-reference boundary for " + label + ". Actual=" + string.Join(",", actual));
    }

    private static void RequireExactlyOnce(string[] paths, string required)
    {
        var count = paths.Count(p => string.Equals(p, required, StringComparison.Ordinal));
        if (count != 1)
            throw new InvalidOperationException($"Stage 8 WP-01 solution membership expected exactly once for {required}, found {count}.");
    }
}
