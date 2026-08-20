using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace Falcon.Stage7.CrossStageIntegration.Verifier;

internal static class Program
{
    private static int _passed;
    private static string _integratedIdentity = string.Empty;

    private static int Main()
    {
        try
        {
            Run("governing-plan-and-authorization-bound", VerifyGoverningDocuments);
            Run("stage7-requirement-trace-and-deferrals-preserved", VerifyRequirementTraceAndDeferrals);
            Run("controlled-solution-stage7-chain-isolated", VerifyControlledSolution);
            Run("stage6-cross-stage-executable-pass", VerifyStage6CrossStageExecutable);
            Run("stage7-wp01-wp10-executable-chain-pass", VerifyStage7ExecutableChain);
            Run("material-manifest-complete", VerifyMaterialManifestComplete);
            Run("material-digests-valid-sha256", VerifyMaterialDigests);
            Run("integrated-identity-deterministic", VerifyIntegratedIdentityDeterministic);
            Run("integrated-identity-mutation-sensitive", VerifyIntegratedIdentityMutationSensitive);
            Run("final-stage7-boundary-no-authority-claim", VerifyNoAuthorityClaim);

            Console.WriteLine("STAGE7_CROSS_STAGE_INTEGRATION_VERIFIER = PASS");
            Console.WriteLine("CHECKS = " + _passed + "/" + _passed);
            Console.WriteLine("INTEGRATED_STAGE7_EVIDENCE_SHA256 = " + _integratedIdentity);
            Console.WriteLine("This verifier creates no Stage 7 closure, Stage 8 authority, recovery authority, FSA governance authority, Application authority, deployment authority, external-connectivity authority, or financial/trading authority.");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("STAGE7_CROSS_STAGE_INTEGRATION_VERIFIER = FAIL");
            Console.Error.WriteLine(ex);
            return 1;
        }
    }

    private static void Run(string name, Action action)
    {
        action();
        _passed++;
        Console.WriteLine("PASS | " + name);
    }

    private static void VerifyGoverningDocuments()
    {
        var root = FindRepositoryRoot();
        var plan = Path.Combine(root, "docs", "stage-7-planning", "07_STAGE7_IMPLEMENTATION_PLAN_v0.3_FINAL_CANDIDATE.md");
        var authorization = Path.Combine(
            root,
            "docs",
            "canonical-records",
            "owner-decisions",
            "stage7",
            "Stage7-Implementation-Authorization-20260811",
            "OWNER-AUTHORIZATION-STAGE7-IMPLEMENTATION-v0.3.md");

        Require(File.Exists(plan), "Accepted Stage 7 plan is missing.");
        Require(File.Exists(authorization), "Canonical Stage 7 implementation authorization is missing.");
        Require(IsSha256(HashFile(plan)), "Stage 7 plan digest is invalid.");
        Require(IsSha256(HashFile(authorization)), "Stage 7 authorization digest is invalid.");

        var authText = File.ReadAllText(authorization);
        Require(authText.Contains("Stage 7", StringComparison.OrdinalIgnoreCase),
            "Stage 7 authorization identity is missing from canonical authorization record.");
        Require(authText.Contains("WP-10", StringComparison.OrdinalIgnoreCase),
            "Stage 7 authorization does not preserve WP-10 sequence coverage.");
    }

    private static void VerifyRequirementTraceAndDeferrals()
    {
        var plan = File.ReadAllText(Path.Combine(
            FindRepositoryRoot(),
            "docs",
            "stage-7-planning",
            "07_STAGE7_IMPLEMENTATION_PLAN_v0.3_FINAL_CANDIDATE.md"));

        foreach (var token in new[]
                 {
                     "SYS-008", "AWR-001", "CON-006", "VPL-005",
                     "REQ-001 through REQ-020", "REQ-021", "REQ-022 through REQ-024",
                     "Section 9", "Section 10", "Stage 8", "Stage 9", "Stage 13",
                     "WP-10"
                 })
        {
            Require(plan.Contains(token, StringComparison.OrdinalIgnoreCase),
                "Required Stage 7 trace/deferral token missing: " + token);
        }

        Require(plan.Contains("Actual Guardian enforcement remains Stage 8", StringComparison.OrdinalIgnoreCase),
            "Stage 8 Guardian enforcement deferral is not explicit.");
        Require(plan.Contains("recovery acceptance or release", StringComparison.OrdinalIgnoreCase),
            "Stage 9 recovery/release deferral is not explicit.");
        Require(plan.Contains("DEFERRED_WITH_TRACE_TO_STAGE13", StringComparison.OrdinalIgnoreCase),
            "Stage 13 FSA governance deferral is not explicit.");
    }

    private static void VerifyControlledSolution()
    {
        var root = FindRepositoryRoot();
        var solutionPath = Path.Combine(root, "Falcon.Foundation.ControlledProjectFoundation.slnx");
        var solution = XDocument.Load(solutionPath);
        var paths = solution.Root?.Elements("Project")
            .Select(value => (value.Attribute("Path")?.Value ?? string.Empty).Replace('\\', '/'))
            .ToArray() ?? Array.Empty<string>();

        var required = new List<string>
        {
            "verification/Falcon.Stage6.CrossStageIntegration.Verifier/Falcon.Stage6.CrossStageIntegration.Verifier.csproj"
        };

        for (var wp = 1; wp <= 10; wp++)
        {
            var text = wp.ToString("D2");
            required.Add($"verification/Falcon.Stage7.WP{text}.Verifier/Falcon.Stage7.WP{text}.Verifier.csproj");
        }

        required.Add("verification/Falcon.Stage7.CrossStageIntegration.Verifier/Falcon.Stage7.CrossStageIntegration.Verifier.csproj");

        foreach (var path in required)
            Require(paths.Count(candidate => string.Equals(candidate, path, StringComparison.Ordinal)) == 1,
                "Controlled solution membership missing or duplicated: " + path);

        Require(paths.All(path => !path.StartsWith("applications/", StringComparison.OrdinalIgnoreCase)),
            "Application project leaked into controlled Foundation solution.");
        Require(paths.All(path => !path.StartsWith("reference/", StringComparison.OrdinalIgnoreCase)),
            "Reference project leaked into controlled Foundation solution.");
    }

    private static void VerifyStage6CrossStageExecutable()
    {
        var root = FindRepositoryRoot();
        var dll = Path.Combine(
            root,
            "verification",
            "Falcon.Stage6.CrossStageIntegration.Verifier",
            "bin",
            "Release",
            "net10.0",
            "Falcon.Stage6.CrossStageIntegration.Verifier.dll");

        var result = ExecuteDll(dll);
        Require(result.ExitCode == 0, "Stage 6 Cross-Stage Integration executable failed.\n" + result.CombinedOutput);
        Require(result.CombinedOutput.Contains("CROSS-STAGE INTEGRATION VERIFIER", StringComparison.OrdinalIgnoreCase) &&
                result.CombinedOutput.Contains("PASS", StringComparison.OrdinalIgnoreCase),
            "Stage 6 Cross-Stage Integration executable did not emit PASS evidence.");
    }

    private static void VerifyStage7ExecutableChain()
    {
        var root = FindRepositoryRoot();
        for (var wp = 1; wp <= 10; wp++)
        {
            var text = wp.ToString("D2");
            var dll = Path.Combine(
                root,
                "verification",
                $"Falcon.Stage7.WP{text}.Verifier",
                "bin",
                "Release",
                "net10.0",
                $"Falcon.Stage7.WP{text}.Verifier.dll");

            var result = ExecuteDll(dll);
            Require(result.ExitCode == 0, $"Stage 7 WP-{text} executable failed.\n" + result.CombinedOutput);
            Require(result.CombinedOutput.Contains($"STAGE7_WP{text}_VERIFIER", StringComparison.OrdinalIgnoreCase) &&
                    result.CombinedOutput.Contains("PASS", StringComparison.OrdinalIgnoreCase),
                $"Stage 7 WP-{text} executable did not emit PASS evidence.");
        }
    }

    private static void VerifyMaterialManifestComplete()
    {
        var manifest = BuildMaterialManifest();
        Require(manifest.Count == 17, "Integrated material manifest count changed. Expected 17, actual " + manifest.Count + ".");
        Require(manifest.Select(value => value.RelativePath).Distinct(StringComparer.Ordinal).Count() == manifest.Count,
            "Integrated material manifest contains duplicate paths.");
    }

    private static void VerifyMaterialDigests()
    {
        foreach (var entry in BuildMaterialManifest())
            Require(IsSha256(entry.Sha256), "Invalid material SHA-256: " + entry.RelativePath);
    }

    private static void VerifyIntegratedIdentityDeterministic()
    {
        var first = ComputeManifestIdentity(BuildMaterialManifest());
        var second = ComputeManifestIdentity(BuildMaterialManifest());
        Require(string.Equals(first, second, StringComparison.Ordinal),
            "Identical material bytes produced different integrated identities.");
        Require(IsSha256(first), "Integrated Stage 7 identity is not SHA-256.");
        _integratedIdentity = first;
    }

    private static void VerifyIntegratedIdentityMutationSensitive()
    {
        var manifest = BuildMaterialManifest();
        var original = ComputeManifestIdentity(manifest);
        var first = manifest[0];
        var mutatedDigest = (first.Sha256[0] == 'A' ? "B" : "A") + first.Sha256[1..];
        var mutated = manifest.ToArray();
        mutated[0] = first with { Sha256 = mutatedDigest };
        var changed = ComputeManifestIdentity(mutated);
        Require(!string.Equals(original, changed, StringComparison.Ordinal),
            "Material digest mutation did not change integrated Stage 7 identity.");
    }

    private static void VerifyNoAuthorityClaim()
    {
        var root = FindRepositoryRoot();
        var design = File.ReadAllText(Path.Combine(
            root,
            "docs",
            "stage-7-implementation",
            "79_STAGE7_CROSS_STAGE_INTEGRATION_DESIGN_AND_TRACE_V1.md"));

        foreach (var token in new[]
                 {
                     "Stage 8 implementation authority",
                     "Stage 9 recovery/release authority",
                     "Stage 13 FSA/Owner governance authority",
                     "Application authority",
                     "deployment authority",
                     "external-connectivity authority",
                     "financial/trading authority"
                 })
            Require(design.Contains(token, StringComparison.OrdinalIgnoreCase),
                "Final integration non-authority declaration missing: " + token);
    }

    private static List<MaterialEntry> BuildMaterialManifest()
    {
        var root = FindRepositoryRoot();
        var paths = new List<string>
        {
            Path.Combine("verification", "Falcon.Stage6.CrossStageIntegration.Verifier", "bin", "Release", "net10.0", "Falcon.Stage6.CrossStageIntegration.Verifier.dll")
        };

        for (var wp = 1; wp <= 10; wp++)
        {
            var text = wp.ToString("D2");
            paths.Add(Path.Combine(
                "verification",
                $"Falcon.Stage7.WP{text}.Verifier",
                "bin",
                "Release",
                "net10.0",
                $"Falcon.Stage7.WP{text}.Verifier.dll"));
        }

        paths.Add(Path.Combine("src", "Foundation.HealthFitness", "bin", "Release", "net10.0", "Foundation.HealthFitness.dll"));
        paths.Add(Path.Combine("src", "Foundation.SelfAwareness", "bin", "Release", "net10.0", "Foundation.SelfAwareness.dll"));
        paths.Add(Path.Combine("tests", "Falcon.Foundation.Architecture.Tests", "bin", "Release", "net10.0", "Falcon.Foundation.Architecture.Tests.dll"));
        paths.Add(Path.Combine("tests", "Falcon.Foundation.Security.Tests", "bin", "Release", "net10.0", "Falcon.Foundation.Security.Tests.dll"));
        paths.Add(Path.Combine("verification", "Falcon.Stage7.CrossStageIntegration.Verifier", "bin", "Release", "net10.0", "Falcon.Stage7.CrossStageIntegration.Verifier.dll"));
        paths.Add(Path.Combine("Falcon.Foundation.ControlledProjectFoundation.slnx"));

        return paths
            .Select(relative =>
            {
                var full = Path.Combine(root, relative);
                Require(File.Exists(full), "Integrated material artifact missing: " + relative);
                return new MaterialEntry(Normalize(relative), HashFile(full));
            })
            .OrderBy(value => value.RelativePath, StringComparer.Ordinal)
            .ToList();
    }

    private static string ComputeManifestIdentity(IEnumerable<MaterialEntry> entries)
    {
        var canonical = string.Join("\n", entries
            .OrderBy(value => value.RelativePath, StringComparer.Ordinal)
            .Select(value => value.RelativePath + "|" + value.Sha256));
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical)));
    }

    private static ProcessResult ExecuteDll(string dll)
    {
        Require(File.Exists(dll), "Executable artifact missing: " + dll);

        var start = new ProcessStartInfo("dotnet")
        {
            WorkingDirectory = FindRepositoryRoot(),
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };
        start.ArgumentList.Add(dll);

        using var process = Process.Start(start) ?? throw new InvalidOperationException("Unable to start dotnet child verifier.");
        var stdout = process.StandardOutput.ReadToEnd();
        var stderr = process.StandardError.ReadToEnd();
        process.WaitForExit();
        return new ProcessResult(process.ExitCode, stdout, stderr);
    }

    private static string FindRepositoryRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current is not null)
        {
            if (File.Exists(Path.Combine(current.FullName, "Falcon.Foundation.ControlledProjectFoundation.slnx")))
                return current.FullName;
            current = current.Parent;
        }

        current = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (current is not null)
        {
            if (File.Exists(Path.Combine(current.FullName, "Falcon.Foundation.ControlledProjectFoundation.slnx")))
                return current.FullName;
            current = current.Parent;
        }

        throw new DirectoryNotFoundException("Falcon repository root not found.");
    }

    private static string HashFile(string path)
        => Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path)));

    private static bool IsSha256(string value)
        => value.Length == 64 && value.All(Uri.IsHexDigit);

    private static string Normalize(string path)
        => path.Replace('\\', '/');

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    private sealed record MaterialEntry(string RelativePath, string Sha256);
    private sealed record ProcessResult(int ExitCode, string StandardOutput, string StandardError)
    {
        public string CombinedOutput => StandardOutput + Environment.NewLine + StandardError;
    }
}
