using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace Falcon.Foundation.Architecture.Tests;

internal static class Stage7Wp10ArchitectureGuard
{
    [ModuleInitializer]
    internal static void VerifyStage7Wp10Boundaries()
    {
        var root = Path.GetFullPath(Directory.GetCurrentDirectory());
        var solutionPath = Path.Combine(root, "Falcon.Foundation.ControlledProjectFoundation.slnx");
        var healthProjectPath = Path.Combine(root, "src", "Foundation.HealthFitness", "Foundation.HealthFitness.csproj");
        var awarenessProjectPath = Path.Combine(root, "src", "Foundation.SelfAwareness", "Foundation.SelfAwareness.csproj");
        var verifierPath = Path.Combine(root, "verification", "Falcon.Stage7.WP10.Verifier", "Falcon.Stage7.WP10.Verifier.csproj");

        if (!File.Exists(solutionPath) || !File.Exists(healthProjectPath) ||
            !File.Exists(awarenessProjectPath) || !File.Exists(verifierPath))
            throw new InvalidOperationException("Stage 7 WP-10 integrated closure surface incomplete.");

        var solution = XDocument.Load(solutionPath);
        var paths = solution.Root?.Elements("Project")
            .Select(e => (e.Attribute("Path")?.Value ?? string.Empty).Replace('\\', '/'))
            .ToArray() ?? Array.Empty<string>();

        RequireExactlyOnce(paths, "verification/Falcon.Stage7.WP10.Verifier/Falcon.Stage7.WP10.Verifier.csproj");

        RequireExactReferences(
            healthProjectPath,
            new[] { Path.Combine(root, "src", "Foundation.Contracts", "Foundation.Contracts.csproj") },
            "Foundation.HealthFitness");

        RequireExactReferences(
            awarenessProjectPath,
            new[]
            {
                Path.Combine(root, "src", "Foundation.Contracts", "Foundation.Contracts.csproj"),
                Path.Combine(root, "src", "Foundation.HealthFitness", "Foundation.HealthFitness.csproj")
            },
            "Foundation.SelfAwareness");

        RequireExactReferences(
            verifierPath,
            new[]
            {
                Path.Combine(root, "src", "Foundation.HealthFitness", "Foundation.HealthFitness.csproj"),
                Path.Combine(root, "src", "Foundation.SelfAwareness", "Foundation.SelfAwareness.csproj")
            },
            "Falcon.Stage7.WP10.Verifier");

        var srcProjects = Directory.GetFiles(Path.Combine(root, "src"), "*.csproj", SearchOption.AllDirectories)
            .Select(Path.GetFullPath)
            .ToArray();

        if (srcProjects.Any(path => path.Contains("Foundation.HealthHistory", StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("Stage 7 WP-10 detected rejected duplicate HealthHistory production project.");

        var verifierSource = File.ReadAllText(Path.Combine(
            root, "verification", "Falcon.Stage7.WP10.Verifier", "Program.cs"));

        foreach (var import in new[]
                 {
                     "using Foundation.Authority",
                     "using Foundation.ApplicationLifecycle",
                     "using Foundation.Guardian",
                     "using Foundation.Recovery"
                 })
        {
            if (verifierSource.Contains(import, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Stage 7 WP-10 forbidden future-stage dependency detected: " + import);
        }

        foreach (var required in new[]
                 {
                     "SYS-008", "AWR-001", "CON-006", "VPL-005",
                     "HealthEvidenceLossClass.Missing",
                     "EvaluateRestoration",
                     "EvaluateLastKnownReliance",
                     "PositiveAuthorityInferenceBlocked",
                     "CreateChangeFact",
                     "Reconstruct"
                 })
        {
            if (!verifierSource.Contains(required, StringComparison.Ordinal))
                throw new InvalidOperationException("Stage 7 WP-10 integrated closure coverage missing: " + required);
        }
    }

    private static void RequireExactReferences(string projectPath, string[] expected, string label)
    {
        var project = XDocument.Load(projectPath);
        var projectDirectory = Path.GetDirectoryName(projectPath)!;
        var actual = project.Root?.Elements("ItemGroup").Elements("ProjectReference")
            .Select(r => Path.GetFullPath(Path.Combine(projectDirectory, r.Attribute("Include")?.Value ?? string.Empty)))
            .OrderBy(v => v, StringComparer.Ordinal)
            .ToArray() ?? Array.Empty<string>();

        var expectedSorted = expected.Select(Path.GetFullPath)
            .OrderBy(v => v, StringComparer.Ordinal)
            .ToArray();

        if (!actual.SequenceEqual(expectedSorted, StringComparer.Ordinal))
            throw new InvalidOperationException(
                "Stage 7 WP-10 changed exact project-reference boundary for " + label + ". Actual=" + string.Join(",", actual));
    }

    private static void RequireExactlyOnce(string[] paths, string required)
    {
        var count = paths.Count(p => string.Equals(p, required, StringComparison.Ordinal));
        if (count != 1)
            throw new InvalidOperationException(
                $"Stage 7 WP-10 solution membership expected exactly once for {required}, found {count}.");
    }
}
