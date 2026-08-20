using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace Falcon.Foundation.Architecture.Tests;

internal static class Stage7Wp09ArchitectureGuard
{
    [ModuleInitializer]
    internal static void VerifyStage7Wp09Boundaries()
    {
        var root = Path.GetFullPath(Directory.GetCurrentDirectory());
        var solutionPath = Path.Combine(root, "Falcon.Foundation.ControlledProjectFoundation.slnx");
        var healthProjectPath = Path.Combine(root, "src", "Foundation.HealthFitness", "Foundation.HealthFitness.csproj");
        var awarenessProjectPath = Path.Combine(root, "src", "Foundation.SelfAwareness", "Foundation.SelfAwareness.csproj");
        var verifierPath = Path.Combine(root, "verification", "Falcon.Stage7.WP09.Verifier", "Falcon.Stage7.WP09.Verifier.csproj");

        if (!File.Exists(solutionPath) || !File.Exists(healthProjectPath) ||
            !File.Exists(awarenessProjectPath) || !File.Exists(verifierPath))
            throw new InvalidOperationException("Stage 7 WP-09 controlled integration surface incomplete.");

        var solution = XDocument.Load(solutionPath);
        var paths = solution.Root?.Elements("Project")
            .Select(e => (e.Attribute("Path")?.Value ?? string.Empty).Replace('\\', '/'))
            .ToArray() ?? Array.Empty<string>();

        RequireExactlyOnce(paths, "verification/Falcon.Stage7.WP09.Verifier/Falcon.Stage7.WP09.Verifier.csproj");

        RequireExactReferences(
            healthProjectPath,
            new[]
            {
                Path.Combine(root, "src", "Foundation.Contracts", "Foundation.Contracts.csproj")
            },
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
            "Falcon.Stage7.WP09.Verifier");

        var verifierSource = File.ReadAllText(Path.Combine(
            root, "verification", "Falcon.Stage7.WP09.Verifier", "Program.cs"));

        // Guard actual compile/import dependencies here. Action/type names are intentionally
        // present as negative-test tokens inside the WP-09 verifier itself and therefore must
        // not be rejected merely because they appear as string literals in test code.
        foreach (var fragment in new[]
        {
            "using Foundation.Authority",
            "using Foundation.ApplicationLifecycle",
            "using Foundation.Guardian",
            "using Foundation.Recovery"
        })
        {
            if (verifierSource.Contains(fragment, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException(
                    "Stage 7 WP-09 forbidden future-stage dependency detected: " + fragment);
        }

        foreach (var required in new[]
        {
            "HealthEvidenceLossClass.Missing",
            "HealthEvidenceLossClass.Stale",
            "HealthEvidenceLossClass.Delayed",
            "HealthEvidenceLossClass.Contradictory",
            "HealthEvidenceLossClass.Unverifiable",
            "HealthEvidenceLossClass.Inaccessible",
            "HealthEvidenceLossClass.Corrupted",
            "HealthEvidenceLossClass.ProvenanceFailure",
            "HealthEvidenceLossClass.PartialVisibility",
            "EvaluateRestoration",
            "EvaluateLastKnownReliance",
            "CreateChangeFact",
            "Reconstruct",
            "PositiveAuthorityInferenceBlocked"
        })
        {
            if (!verifierSource.Contains(required, StringComparison.Ordinal))
                throw new InvalidOperationException(
                    "Stage 7 WP-09 required integration coverage missing: " + required);
        }
    }

    private static void RequireExactReferences(
        string projectPath,
        string[] expected,
        string label)
    {
        var project = XDocument.Load(projectPath);
        var projectDirectory = Path.GetDirectoryName(projectPath)!;
        var actual = project.Root?.Elements("ItemGroup").Elements("ProjectReference")
            .Select(r => Path.GetFullPath(Path.Combine(
                projectDirectory,
                r.Attribute("Include")?.Value ?? string.Empty)))
            .OrderBy(v => v, StringComparer.Ordinal)
            .ToArray() ?? Array.Empty<string>();

        var expectedSorted = expected.Select(Path.GetFullPath)
            .OrderBy(v => v, StringComparer.Ordinal)
            .ToArray();

        if (!actual.SequenceEqual(expectedSorted, StringComparer.Ordinal))
            throw new InvalidOperationException(
                "Stage 7 WP-09 changed exact project-reference boundary for " + label +
                ". Actual=" + string.Join(",", actual));
    }

    private static void RequireExactlyOnce(string[] paths, string required)
    {
        var count = paths.Count(p => string.Equals(p, required, StringComparison.Ordinal));
        if (count != 1)
            throw new InvalidOperationException(
                $"Stage 7 WP-09 solution membership expected exactly once for {required}, found {count}.");
    }
}
