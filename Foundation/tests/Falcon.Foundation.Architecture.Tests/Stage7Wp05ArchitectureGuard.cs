using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace Falcon.Foundation.Architecture.Tests;

internal static class Stage7Wp05ArchitectureGuard
{
    [ModuleInitializer]
    internal static void VerifyStage7Wp05ControlledMembership()
    {
        var root = Path.GetFullPath(Directory.GetCurrentDirectory());
        var solutionPath = Path.Combine(root, "Falcon.Foundation.ControlledProjectFoundation.slnx");
        var verifierPath = Path.Combine(root, "verification", "Falcon.Stage7.WP05.Verifier", "Falcon.Stage7.WP05.Verifier.csproj");
        var healthRuntime = Path.Combine(root, "src", "Foundation.HealthFitness", "HealthEvidenceQualityRuntime.cs");
        var awarenessRuntime = Path.Combine(root, "src", "Foundation.SelfAwareness", "EvidenceAwarenessRuntime.cs");

        if (!File.Exists(solutionPath) || !File.Exists(verifierPath) || !File.Exists(healthRuntime) || !File.Exists(awarenessRuntime))
            throw new InvalidOperationException("Stage 7 WP-05 controlled implementation surface incomplete.");

        const string requiredSolutionPath = "verification/Falcon.Stage7.WP05.Verifier/Falcon.Stage7.WP05.Verifier.csproj";
        var solution = XDocument.Load(solutionPath);
        var membershipCount = solution.Root?.Elements("Project")
            .Select(element => (element.Attribute("Path")?.Value ?? string.Empty).Replace('\\', '/'))
            .Count(value => string.Equals(value, requiredSolutionPath, StringComparison.Ordinal)) ?? 0;

        if (membershipCount != 1)
            throw new InvalidOperationException($"Stage 7 WP-05 verifier controlled-solution membership expected exactly once, found {membershipCount}.");

        var verifier = XDocument.Load(verifierPath);
        var verifierDirectory = Path.GetDirectoryName(verifierPath)
            ?? throw new InvalidOperationException("Stage 7 WP-05 verifier directory missing.");
        var actualReferences = verifier.Root?.Elements("ItemGroup").Elements("ProjectReference")
            .Select(reference => reference.Attribute("Include")?.Value ?? string.Empty)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => Path.GetFullPath(Path.Combine(verifierDirectory, value)))
            .OrderBy(value => value, StringComparer.Ordinal)
            .ToArray() ?? Array.Empty<string>();

        var expectedReferences = new[]
        {
            Path.GetFullPath(Path.Combine(root, "src", "Foundation.HealthFitness", "Foundation.HealthFitness.csproj")),
            Path.GetFullPath(Path.Combine(root, "src", "Foundation.SelfAwareness", "Foundation.SelfAwareness.csproj"))
        }.OrderBy(value => value, StringComparer.Ordinal).ToArray();

        if (!actualReferences.SequenceEqual(expectedReferences, StringComparer.Ordinal))
            throw new InvalidOperationException("Stage 7 WP-05 verifier project-reference boundary changed or is incomplete.");

        var forbiddenFragments = new[]
        {
            "applications/", "web-development", "Foundation.Guardian", "Foundation.Recovery", "Foundation.ApplicationLifecycle"
        };
        var sourceText = File.ReadAllText(healthRuntime) + "\n" + File.ReadAllText(awarenessRuntime);
        foreach (var fragment in forbiddenFragments)
        {
            if (sourceText.Contains(fragment, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Stage 7 WP-05 forbidden boundary reference detected: " + fragment);
        }
    }
}
