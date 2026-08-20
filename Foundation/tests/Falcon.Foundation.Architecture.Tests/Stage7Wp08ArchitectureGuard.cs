using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace Falcon.Foundation.Architecture.Tests;

internal static class Stage7Wp08ArchitectureGuard
{
    [ModuleInitializer]
    internal static void VerifyStage7Wp08Boundaries()
    {
        var root = Path.GetFullPath(Directory.GetCurrentDirectory());
        var solutionPath = Path.Combine(root, "Falcon.Foundation.ControlledProjectFoundation.slnx");
        var healthProjectPath = Path.Combine(root, "src", "Foundation.HealthFitness", "Foundation.HealthFitness.csproj");
        var sourcePath = Path.Combine(root, "src", "Foundation.HealthFitness", "HealthFitnessGovernedConsumptionRuntime.cs");
        var verifierPath = Path.Combine(root, "verification", "Falcon.Stage7.WP08.Verifier", "Falcon.Stage7.WP08.Verifier.csproj");

        if (!File.Exists(solutionPath) || !File.Exists(healthProjectPath) || !File.Exists(sourcePath) || !File.Exists(verifierPath))
            throw new InvalidOperationException("Stage 7 WP-08 controlled implementation surface incomplete.");

        var solution = XDocument.Load(solutionPath);
        var paths = solution.Root?.Elements("Project")
            .Select(e => (e.Attribute("Path")?.Value ?? string.Empty).Replace('\\', '/'))
            .ToArray() ?? Array.Empty<string>();

        RequireExactlyOnce(paths, "verification/Falcon.Stage7.WP08.Verifier/Falcon.Stage7.WP08.Verifier.csproj");

        var project = XDocument.Load(healthProjectPath);
        var projectDirectory = Path.GetDirectoryName(healthProjectPath)!;
        var actualReferences = project.Root?.Elements("ItemGroup").Elements("ProjectReference")
            .Select(r => Path.GetFullPath(Path.Combine(projectDirectory, r.Attribute("Include")?.Value ?? string.Empty)))
            .OrderBy(v => v, StringComparer.Ordinal)
            .ToArray() ?? Array.Empty<string>();

        var expectedReferences = new[]
        {
            Path.Combine(root, "src", "Foundation.Contracts", "Foundation.Contracts.csproj")
        }.Select(Path.GetFullPath).OrderBy(v => v, StringComparer.Ordinal).ToArray();

        if (!actualReferences.SequenceEqual(expectedReferences, StringComparer.Ordinal))
            throw new InvalidOperationException("Stage 7 WP-08 changed Foundation.HealthFitness project-reference boundary.");

        var source = File.ReadAllText(sourcePath);
        var forbidden = new[]
        {
            "using Foundation.Authority",
            "using Foundation.ApplicationLifecycle",
            "Foundation.Guardian",
            "Foundation.Recovery",
            "GuardianCommand",
            "LifecycleTransition",
            "PlatformSafeState",
            "GrantAuthority",
            "RestoreAuthority"
        };

        foreach (var fragment in forbidden)
        {
            if (source.Contains(fragment, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Stage 7 WP-08 forbidden authority/enforcement surface detected: " + fragment);
        }

        if (!source.Contains("CanSupportPositiveAuthorityCondition", StringComparison.Ordinal) ||
            !source.Contains("PositiveAuthorityInferenceBlocked", StringComparison.Ordinal) ||
            !source.Contains("IndependentReassessmentRequired", StringComparison.Ordinal) ||
            !source.Contains("NewAuthorityDecisionRequired", StringComparison.Ordinal))
            throw new InvalidOperationException("Stage 7 WP-08 required governed-consumption evidence surface incomplete.");
    }

    private static void RequireExactlyOnce(string[] paths, string required)
    {
        var count = paths.Count(p => string.Equals(p, required, StringComparison.Ordinal));
        if (count != 1)
            throw new InvalidOperationException($"Stage 7 WP-08 solution membership expected exactly once for {required}, found {count}.");
    }
}
