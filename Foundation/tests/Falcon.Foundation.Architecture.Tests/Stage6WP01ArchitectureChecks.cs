using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace Falcon.Foundation.Architecture.Tests;

internal static class Stage6WP01ArchitectureChecks
{
    [ModuleInitializer]
    internal static void ValidateStage6WP01Architecture()
    {
        var root = Path.GetFullPath(Directory.GetCurrentDirectory());
        var solutionPath = Path.Combine(root, "Falcon.Foundation.ControlledProjectFoundation.slnx");
        var contractsProject = Path.Combine(root, "src", "Foundation.Contracts", "Foundation.Contracts.csproj");
        var primitiveSource = Path.Combine(root, "src", "Foundation.Contracts", "ResourceGovernancePrimitives.cs");
        var verifierProject = Path.Combine(root, "verification", "Falcon.Stage6.WP01.Verifier", "Falcon.Stage6.WP01.Verifier.csproj");

        RequireFile(solutionPath, "Stage 6 WP-01 controlled solution");
        RequireFile(contractsProject, "Foundation.Contracts project");
        RequireFile(primitiveSource, "Stage 6 WP-01 primitive source");
        RequireFile(verifierProject, "Stage 6 WP-01 verifier project");

        var solutionText = File.ReadAllText(solutionPath);
        Require(solutionText.Contains("src/Foundation.Contracts/Foundation.Contracts.csproj", StringComparison.Ordinal),
            "Foundation.Contracts is missing from the controlled solution.");
        Require(solutionText.Contains("verification/Falcon.Stage6.WP01.Verifier/Falcon.Stage6.WP01.Verifier.csproj", StringComparison.Ordinal),
            "Stage 6 WP-01 verifier is missing from the controlled solution.");
        Require(!solutionText.Contains("src/Foundation.ResourceGovernance/Foundation.ResourceGovernance.csproj", StringComparison.Ordinal),
            "WP-01 must not create an unapproved standalone production project for primitives.");

        var contractsReferences = ReadProjectReferences(contractsProject);
        Require(contractsReferences.Length == 0,
            "Foundation.Contracts must remain dependency-free.");

        var verifierReferences = ReadProjectReferences(verifierProject);
        Require(verifierReferences.Length == 1,
            "Stage 6 WP-01 verifier must reference exactly one production project.");
        Require(Path.GetFullPath(verifierReferences[0]).Equals(Path.GetFullPath(contractsProject), StringComparison.OrdinalIgnoreCase),
            "Stage 6 WP-01 verifier must reference only Foundation.Contracts.");

        var source = File.ReadAllText(primitiveSource);
        Require(source.Contains("namespace Foundation.Contracts.ResourceGovernance;", StringComparison.Ordinal),
            "Stage 6 WP-01 primitives must remain in the Foundation.Contracts resource-governance namespace.");
        Require(!source.Contains("Trading", StringComparison.OrdinalIgnoreCase)
                && !source.Contains("FSATS", StringComparison.OrdinalIgnoreCase)
                && !source.Contains("Accounting", StringComparison.OrdinalIgnoreCase)
                && !source.Contains("Warehouse", StringComparison.OrdinalIgnoreCase),
            "Application-specific business identity leaked into Stage 6 WP-01 primitives.");
    }

    private static string[] ReadProjectReferences(string projectPath)
    {
        var document = XDocument.Load(projectPath);
        var directory = Path.GetDirectoryName(projectPath) ?? throw new InvalidOperationException("Project directory unavailable.");
        return document.Descendants("ProjectReference")
            .Select(element => element.Attribute("Include")?.Value)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => Path.GetFullPath(Path.Combine(directory, value!)))
            .ToArray();
    }

    private static void RequireFile(string path, string name)
    {
        if (!File.Exists(path))
        {
            throw new InvalidOperationException($"Missing {name}: {path}");
        }
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
}
