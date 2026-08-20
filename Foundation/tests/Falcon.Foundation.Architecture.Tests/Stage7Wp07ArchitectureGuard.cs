using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace Falcon.Foundation.Architecture.Tests;

internal static class Stage7Wp07ArchitectureGuard
{
    [ModuleInitializer]
    internal static void VerifyStage7Wp07Boundaries()
    {
        var root = Path.GetFullPath(Directory.GetCurrentDirectory());
        var solutionPath = Path.Combine(root, "Falcon.Foundation.ControlledProjectFoundation.slnx");
        var healthProjectPath = Path.Combine(root, "src", "Foundation.HealthFitness", "Foundation.HealthFitness.csproj");
        var sourcePath = Path.Combine(root, "src", "Foundation.HealthFitness", "HealthFitnessHistoryRuntime.cs");
        var forbiddenProjectPath = Path.Combine(root, "src", "Foundation.HealthHistory", "Foundation.HealthHistory.csproj");
        var verifierPath = Path.Combine(root, "verification", "Falcon.Stage7.WP07.Verifier", "Falcon.Stage7.WP07.Verifier.csproj");

        if (!File.Exists(solutionPath) || !File.Exists(healthProjectPath) || !File.Exists(sourcePath) || !File.Exists(verifierPath))
            throw new InvalidOperationException("Stage 7 WP-07 controlled implementation surface incomplete.");
        if (File.Exists(forbiddenProjectPath))
            throw new InvalidOperationException("Stage 7 WP-07 must not create a permanent Foundation.HealthHistory production project.");

        var solution = XDocument.Load(solutionPath);
        var paths = solution.Root?.Elements("Project")
            .Select(e => (e.Attribute("Path")?.Value ?? string.Empty).Replace('\\', '/'))
            .ToArray() ?? Array.Empty<string>();
        RequireExactlyOnce(paths, "src/Foundation.HealthFitness/Foundation.HealthFitness.csproj");
        RequireExactlyOnce(paths, "verification/Falcon.Stage7.WP07.Verifier/Falcon.Stage7.WP07.Verifier.csproj");
        if (paths.Any(p => string.Equals(p, "src/Foundation.HealthHistory/Foundation.HealthHistory.csproj", StringComparison.Ordinal)))
            throw new InvalidOperationException("Stage 7 WP-07 forbidden project remains in controlled solution.");

        var healthProject = XDocument.Load(healthProjectPath);
        var projectDirectory = Path.GetDirectoryName(healthProjectPath)!;
        var actualReferences = healthProject.Root?.Elements("ItemGroup").Elements("ProjectReference")
            .Select(r => Path.GetFullPath(Path.Combine(projectDirectory, r.Attribute("Include")?.Value ?? string.Empty)))
            .OrderBy(v => v, StringComparer.Ordinal).ToArray() ?? Array.Empty<string>();
        var expected = new[] { Path.Combine(root, "src", "Foundation.Contracts", "Foundation.Contracts.csproj") }
            .Select(Path.GetFullPath).OrderBy(v => v, StringComparer.Ordinal).ToArray();
        if (!actualReferences.SequenceEqual(expected, StringComparer.Ordinal))
            throw new InvalidOperationException("Stage 7 WP-07 changed accepted HealthFitness project-reference boundary.");

        var verifier = XDocument.Load(verifierPath);
        var verifierDirectory = Path.GetDirectoryName(verifierPath)!;
        var verifierRefs = verifier.Root?.Elements("ItemGroup").Elements("ProjectReference")
            .Select(r => Path.GetFullPath(Path.Combine(verifierDirectory, r.Attribute("Include")?.Value ?? string.Empty)))
            .ToArray() ?? Array.Empty<string>();
        if (verifierRefs.Length != 1 || !string.Equals(verifierRefs[0], Path.GetFullPath(healthProjectPath), StringComparison.Ordinal))
            throw new InvalidOperationException("Stage 7 WP-07 verifier must reference only Foundation.HealthFitness.");

        var source = File.ReadAllText(sourcePath);
        var forbidden = new[] { "applications/", "web-development", "Foundation.Authority", "Foundation.ApplicationLifecycle", "Foundation.Guardian", "Foundation.Recovery", "broker", "trading", "market-data" };
        foreach (var fragment in forbidden)
            if (source.Contains(fragment, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Stage 7 WP-07 forbidden boundary reference detected: " + fragment);
    }

    private static void RequireExactlyOnce(string[] paths, string required)
    {
        var count = paths.Count(p => string.Equals(p, required, StringComparison.Ordinal));
        if (count != 1) throw new InvalidOperationException($"Stage 7 WP-07 solution membership expected exactly once for {required}, found {count}.");
    }
}
