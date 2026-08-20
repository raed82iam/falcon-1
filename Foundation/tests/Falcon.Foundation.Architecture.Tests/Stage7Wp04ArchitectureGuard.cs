using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace Falcon.Foundation.Architecture.Tests;

internal static class Stage7Wp04ArchitectureGuard
{
    [ModuleInitializer]
    internal static void VerifyStage7Wp04ControlledMembership()
    {
        var root = Path.GetFullPath(Directory.GetCurrentDirectory());
        var solutionPath = Path.Combine(root, "Falcon.Foundation.ControlledProjectFoundation.slnx");
        var verifierPath = Path.Combine(
            root,
            "verification",
            "Falcon.Stage7.WP04.Verifier",
            "Falcon.Stage7.WP04.Verifier.csproj");

        if (!File.Exists(solutionPath))
            throw new InvalidOperationException("Controlled Foundation solution missing for Stage 7 WP-04 guard.");
        if (!File.Exists(verifierPath))
            throw new InvalidOperationException("Stage 7 WP-04 verifier project missing.");

        const string requiredSolutionPath = "verification/Falcon.Stage7.WP04.Verifier/Falcon.Stage7.WP04.Verifier.csproj";
        var solution = XDocument.Load(solutionPath);
        var membershipCount = solution.Root?.Elements("Project")
            .Select(element => (element.Attribute("Path")?.Value ?? string.Empty).Replace('\\', '/'))
            .Count(value => string.Equals(value, requiredSolutionPath, StringComparison.Ordinal)) ?? 0;

        if (membershipCount != 1)
            throw new InvalidOperationException(
                $"Stage 7 WP-04 verifier controlled-solution membership expected exactly once, found {membershipCount}.");

        var verifier = XDocument.Load(verifierPath);
        var verifierDirectory = Path.GetDirectoryName(verifierPath)
            ?? throw new InvalidOperationException("Stage 7 WP-04 verifier directory missing.");
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
            throw new InvalidOperationException(
                "Stage 7 WP-04 verifier project-reference boundary changed or is incomplete.");
    }
}