using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace Falcon.Foundation.Architecture.Tests;

internal static class Stage7Wp06ArchitectureGuard
{
    [ModuleInitializer]
    internal static void VerifyStage7Wp06ControlledMembership()
    {
        var root = Path.GetFullPath(Directory.GetCurrentDirectory());
        var solutionPath = Path.Combine(root, "Falcon.Foundation.ControlledProjectFoundation.slnx");
        var verifierPath = Path.Combine(root, "verification", "Falcon.Stage7.WP06.Verifier", "Falcon.Stage7.WP06.Verifier.csproj");
        var runtimePath = Path.Combine(root, "src", "Foundation.HealthFitness", "PredecessorTruthIntegrationRuntime.cs");
        var healthProjectPath = Path.Combine(root, "src", "Foundation.HealthFitness", "Foundation.HealthFitness.csproj");

        if (!File.Exists(solutionPath) || !File.Exists(verifierPath) || !File.Exists(runtimePath) || !File.Exists(healthProjectPath))
            throw new InvalidOperationException("Stage 7 WP-06 controlled implementation surface incomplete.");

        const string requiredSolutionPath = "verification/Falcon.Stage7.WP06.Verifier/Falcon.Stage7.WP06.Verifier.csproj";
        var solution = XDocument.Load(solutionPath);
        var membershipCount = solution.Root?.Elements("Project")
            .Select(element => (element.Attribute("Path")?.Value ?? string.Empty).Replace('\\', '/'))
            .Count(value => string.Equals(value, requiredSolutionPath, StringComparison.Ordinal)) ?? 0;

        if (membershipCount != 1)
            throw new InvalidOperationException($"Stage 7 WP-06 verifier controlled-solution membership expected exactly once, found {membershipCount}.");

        var verifier = XDocument.Load(verifierPath);
        var verifierDirectory = Path.GetDirectoryName(verifierPath)
            ?? throw new InvalidOperationException("Stage 7 WP-06 verifier directory missing.");
        var actualVerifierReferences = verifier.Root?.Elements("ItemGroup").Elements("ProjectReference")
            .Select(reference => reference.Attribute("Include")?.Value ?? string.Empty)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => Path.GetFullPath(Path.Combine(verifierDirectory, value)))
            .OrderBy(value => value, StringComparer.Ordinal)
            .ToArray() ?? Array.Empty<string>();

        var expectedVerifierReferences = new[]
        {
            Path.GetFullPath(Path.Combine(root, "src", "Foundation.HealthFitness", "Foundation.HealthFitness.csproj"))
        };

        if (!actualVerifierReferences.SequenceEqual(expectedVerifierReferences, StringComparer.Ordinal))
            throw new InvalidOperationException("Stage 7 WP-06 verifier project-reference boundary changed or is incomplete.");

        var healthProject = XDocument.Load(healthProjectPath);
        var healthDirectory = Path.GetDirectoryName(healthProjectPath)
            ?? throw new InvalidOperationException("Foundation.HealthFitness project directory missing.");
        var actualHealthReferences = healthProject.Root?.Elements("ItemGroup").Elements("ProjectReference")
            .Select(reference => reference.Attribute("Include")?.Value ?? string.Empty)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => Path.GetFullPath(Path.Combine(healthDirectory, value)))
            .OrderBy(value => value, StringComparer.Ordinal)
            .ToArray() ?? Array.Empty<string>();

        var expectedHealthReferences = new[]
        {
            Path.GetFullPath(Path.Combine(root, "src", "Foundation.Contracts", "Foundation.Contracts.csproj"))
        };

        if (!actualHealthReferences.SequenceEqual(expectedHealthReferences, StringComparer.Ordinal))
            throw new InvalidOperationException("Stage 7 WP-06 introduced a predecessor dependency mesh into Foundation.HealthFitness.");

        var source = File.ReadAllText(runtimePath);
        var forbiddenFragments = new[]
        {
            "applications/", "web-development", "Foundation.Guardian", "Foundation.Recovery",
            "Foundation.ApplicationLifecycle", "Foundation.Authority", "Foundation.DependencyGovernance",
            "Foundation.State", "Foundation.EventSystem", "Foundation.Reconciliation"
        };

        foreach (var fragment in forbiddenFragments)
        {
            if (source.Contains(fragment, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Stage 7 WP-06 forbidden direct predecessor/later-stage reference detected: " + fragment);
        }
    }
}
