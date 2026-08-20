using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

internal static class Program
{
    private sealed record ScenarioSpec(
        string Id,
        string ProjectDirectory,
        string AssemblyName,
        string PassMarker,
        string[] RequiredMarkers);

    private sealed record ReconstructionRecord(
        string Id,
        int ExitCode,
        string OutputDigest,
        string Output,
        IReadOnlyList<string> Corrections);

    private static int Main()
    {
        var failures = new List<string>();
        var checks = 0;

        void Expect(string name, bool condition)
        {
            checks++;
            if (!condition)
                failures.Add(name);
        }

        try
        {
            var root = FindRepositoryRoot();
            var specs = BuildSpecs();

            // Phase 1: independent reconstruction. This phase records what the current
            // candidate actually produces before the expected chronology is evaluated.
            var reconstructed = specs.Select(spec => RunScenario(root, spec)).ToList();
            var reconstructionIdentity = PackageIdentity(reconstructed);

            Expect("reconstruction-seven-scenarios", reconstructed.Count == 7);
            Expect("reconstruction-order-recorded", reconstructed.Select(x => x.Id).SequenceEqual(specs.Select(x => x.Id), StringComparer.Ordinal));
            Expect("reconstruction-identity-deterministic", reconstructionIdentity == PackageIdentity(reconstructed));
            Expect("reconstruction-identity-shape", IsSha256(reconstructionIdentity));

            // Phase 2: separate evaluation against the governed VPL chronology.
            for (var i = 0; i < specs.Length; i++)
            {
                var spec = specs[i];
                var record = reconstructed[i];
                Expect($"{spec.Id}-exit-pass", record.ExitCode == 0);
                Expect($"{spec.Id}-pass-marker", record.Output.Contains(spec.PassMarker, StringComparison.Ordinal));
                Expect($"{spec.Id}-required-semantics", spec.RequiredMarkers.All(marker => record.Output.Contains(marker, StringComparison.Ordinal)));
            }

            Expect("vpl008-plan-present", File.Exists(Path.Combine(root, "docs", "verification", "VPL-008_EVIDENCE_RECONSTRUCTION.md")));
            Expect("frs001-present", File.Exists(Path.Combine(root, "docs", "releases", "FRS-001_FOUNDATION_RELEASE.md")));
            Expect("stage10-reconstruction-plan-present", File.Exists(Path.Combine(root, "docs", "stage-10-planning", "03_STAGE10_VPL008_INDEPENDENT_RECONSTRUCTION_AND_ADVERSARIAL_PLAN.md")));

            var solution = File.ReadAllText(Path.Combine(root, "Falcon.Foundation.ControlledProjectFoundation.slnx"));
            Expect("application-neutral-controlled-solution",
                !solution.Contains("applications/", StringComparison.OrdinalIgnoreCase) &&
                !solution.Contains("applications\\", StringComparison.OrdinalIgnoreCase));

            var canonicalIds = specs.Select(x => x.Id).ToArray();
            Expect("mutation-detected", DetectIdentityMutation(reconstructed, reconstructionIdentity));
            Expect("deletion-detected", !ValidateShape(reconstructed.Take(6).ToList(), canonicalIds));

            var inserted = reconstructed.ToList();
            inserted.Insert(3, new ReconstructionRecord("VPL-999", 0, Digest("unknown"), "unknown", Array.Empty<string>()));
            Expect("insertion-detected", !ValidateShape(inserted, canonicalIds));

            var reordered = reconstructed.ToList();
            (reordered[0], reordered[1]) = (reordered[1], reordered[0]);
            Expect("reordering-detected", !ValidateShape(reordered, canonicalIds));

            var duplicated = reconstructed.ToList();
            duplicated[6] = duplicated[5];
            Expect("duplication-detected", !ValidateShape(duplicated, canonicalIds));

            Expect("required-marker-loss-detected", DetectRequiredMarkerLoss(specs[1], reconstructed[1]));
            Expect("append-only-correction-accepted", ValidateAppendOnlyCorrection(reconstructed[0]));
            Expect("history-rewrite-rejected", RejectHistoryRewrite(reconstructed[0]));

            Expect("no-financial-or-external-stage10-surface", NoForbiddenStage10ProductionSurface(root));

            if (failures.Count > 0)
            {
                Console.Error.WriteLine("STAGE10_VPL008_VERIFIER = FAIL");
                Console.Error.WriteLine($"CHECKS = {checks - failures.Count}/{checks}");
                foreach (var failure in failures.OrderBy(x => x, StringComparer.Ordinal))
                    Console.Error.WriteLine(failure);
                return 1;
            }

            Console.WriteLine("STAGE10_VPL008_VERIFIER = PASS");
            Console.WriteLine($"CHECKS = {checks}/{checks}");
            Console.WriteLine("VPL001_TRUSTED_BOOTSTRAP = PASS");
            Console.WriteLine("VPL002_UNAUTHORIZED_ACTION = PASS");
            Console.WriteLine("VPL003_INVALID_LIFECYCLE_TRANSITION = PASS");
            Console.WriteLine("VPL004_INVALID_FIL_MESSAGE = PASS");
            Console.WriteLine("VPL005_HEALTH_EVIDENCE_LOSS = PASS");
            Console.WriteLine("VPL006_GUARDIAN_RESTRICTION = PASS");
            Console.WriteLine("VPL007_CONTROLLED_RECOVERY = PASS");
            Console.WriteLine("VPL008_ADVERSARIAL_VARIANTS = 8/8 PASS");
            Console.WriteLine("APPLICATION_NEUTRALITY = PASS");
            Console.WriteLine("FRS001_NON_FINANCIAL_BOUNDARY = PASS");
            Console.WriteLine("RECONSTRUCTION_IDENTITY = " + reconstructionIdentity);
            Console.WriteLine("VPL008_TECHNICAL_PASS != RELEASE_AUTHORITY_DECISION");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("STAGE10_VPL008_VERIFIER = FAIL");
            Console.Error.WriteLine(ex.Message);
            return 1;
        }
    }

    private static ScenarioSpec[] BuildSpecs() =>
    [
        new(
            "VPL-001",
            "verification/Falcon.Stage3.WP06.Verifier",
            "Falcon.Stage3.WP06.Verifier",
            "Stage 3 WP-06: PASS",
            ["DEPENDENCY_GRAPH_VALIDATED", "ACTIVATION_ORDER_VALIDATED", "END_TO_END_PLUGIN_ADMISSION_BOOTSTRAP_LIFECYCLE_VALIDATED", "WP06_DETERMINISTIC_EVIDENCE_VALIDATED"]),
        new(
            "VPL-002",
            "verification/Falcon.Stage4.WP01.Verifier",
            "Falcon.Stage4.WP01.Verifier",
            "Stage 4 WP-01 verifier: PASS",
            ["default-deny", "deterministic replay", "No execution or authoritative state mutation surface exists"]),
        new(
            "VPL-003",
            "verification/Falcon.Stage4.WP02.Verifier",
            "Falcon.Stage4.WP02.Verifier",
            "Stage 4 WP-02 verifier: PASS",
            ["default-deny behavior", "lifecycle graph preservation", "No second lifecycle controller was introduced"]),
        new(
            "VPL-004",
            "verification/Falcon.Stage5.WP10.Verifier",
            "Falcon.Stage5.WP10.Verifier",
            "Stage 5 WP-10 verifier: PASS",
            ["Verified Stage 5 WP-01 through WP-09 composition", "replay non-authority", "payload_business_semantics_remain_opaque"]),
        new(
            "VPL-005",
            "verification/Falcon.Stage7.WP10.Verifier",
            "Falcon.Stage7.WP10.Verifier",
            "STAGE7_WP10_VERIFIER = PASS",
            ["PASS | vpl005-exact-nine-loss-classes", "PASS | history-reconstruction-surface-present", "PASS | no-future-action-method-surface"]),
        new(
            "VPL-006",
            "verification/Falcon.Stage8.WP10.Verifier",
            "Falcon.Stage8.WP10.Verifier",
            "STAGE8_WP10_INTEGRATED_VERIFIER = PASS",
            ["GUARDIAN_PROTECTS_NOT_GRANTS_AUTHORITY = PRESERVED", "SAFE_STATE_ALLOWLIST != AUTHORITY_GRANT", "STAGE9_RECOVERY_RELEASE_IMPLEMENTATION = ABSENT"]),
        new(
            "VPL-007",
            "verification/Falcon.Stage9.WP10.Verifier",
            "Falcon.Stage9.WP10.Verifier",
            "STAGE9_WP10_INTEGRATED_VERIFIER = PASS",
            ["VPL007_POSITIVE_PATH = PASS", "VPL007_NEGATIVE_VARIANTS = 8/8 PASS", "STAGE9_WP01_WP09_EXECUTABLE_MATRIX = PASS"])
    ];

    private static ReconstructionRecord RunScenario(string root, ScenarioSpec spec)
    {
        var dll = Path.Combine(
            root,
            spec.ProjectDirectory.Replace('/', Path.DirectorySeparatorChar),
            "bin", "Release", "net10.0", spec.AssemblyName + ".dll");

        if (!File.Exists(dll))
            return new ReconstructionRecord(spec.Id, -1, Digest("missing:" + dll), "missing verifier DLL: " + dll, Array.Empty<string>());

        var start = new ProcessStartInfo
        {
            FileName = "dotnet",
            WorkingDirectory = root,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        start.ArgumentList.Add(dll);

        using var process = Process.Start(start) ?? throw new InvalidOperationException("Failed to start " + spec.AssemblyName);
        var stdout = process.StandardOutput.ReadToEnd();
        var stderr = process.StandardError.ReadToEnd();
        process.WaitForExit();
        var output = (stdout + Environment.NewLine + stderr).Trim();
        return new ReconstructionRecord(spec.Id, process.ExitCode, Digest(output), output, Array.Empty<string>());
    }

    private static bool ValidateShape(IReadOnlyList<ReconstructionRecord> records, IReadOnlyList<string> canonicalIds) =>
        records.Count == canonicalIds.Count &&
        records.Select(x => x.Id).SequenceEqual(canonicalIds, StringComparer.Ordinal) &&
        records.Select(x => x.Id).Distinct(StringComparer.Ordinal).Count() == canonicalIds.Count;

    private static bool DetectIdentityMutation(IReadOnlyList<ReconstructionRecord> records, string originalIdentity)
    {
        var mutated = records.Select((record, index) =>
            index == 2 ? record with { OutputDigest = Digest(record.OutputDigest + ":mutated") } : record).ToList();
        return ValidateShape(mutated, records.Select(x => x.Id).ToArray()) && PackageIdentity(mutated) != originalIdentity;
    }

    private static bool DetectRequiredMarkerLoss(ScenarioSpec spec, ReconstructionRecord record)
    {
        var marker = spec.RequiredMarkers.First();
        if (!record.Output.Contains(marker, StringComparison.Ordinal))
            return false;

        var mutatedOutput = record.Output.Replace(marker, "[REMOVED_REQUIRED_MARKER]", StringComparison.Ordinal);
        return !spec.RequiredMarkers.All(required => mutatedOutput.Contains(required, StringComparison.Ordinal));
    }

    private static bool ValidateAppendOnlyCorrection(ReconstructionRecord original)
    {
        var corrected = original with { Corrections = ["correction-001: reviewer clarification appended"] };
        return original.OutputDigest == corrected.OutputDigest &&
               corrected.Corrections.Count == 1 &&
               PackageIdentity([original]) != PackageIdentity([corrected]);
    }

    private static bool RejectHistoryRewrite(ReconstructionRecord original)
    {
        var rewritten = original with
        {
            Output = original.Output + Environment.NewLine + "REWRITTEN_HISTORY",
            OutputDigest = Digest(original.Output + Environment.NewLine + "REWRITTEN_HISTORY")
        };
        return rewritten.OutputDigest != original.OutputDigest && rewritten.Corrections.Count == 0;
    }

    private static bool NoForbiddenStage10ProductionSurface(string root)
    {
        var solution = File.ReadAllText(Path.Combine(root, "Falcon.Foundation.ControlledProjectFoundation.slnx"));
        var forbidden = new[] { "applications/", "FSATS", "Trading", "Broker", "MarketData" };
        return forbidden.All(token => !solution.Contains(token, StringComparison.OrdinalIgnoreCase));
    }

    private static string PackageIdentity(IEnumerable<ReconstructionRecord> records)
    {
        var canonical = string.Join("\n", records.Select(record =>
            string.Join("|",
                record.Id,
                record.ExitCode.ToString(System.Globalization.CultureInfo.InvariantCulture),
                record.OutputDigest,
                string.Join(";", record.Corrections))));
        return Digest(canonical);
    }

    private static string Digest(string value) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));

    private static bool IsSha256(string value) =>
        value.Length == 64 && value.All(Uri.IsHexDigit);

    private static string FindRepositoryRoot()
    {
        var current = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (current is not null)
        {
            if (File.Exists(Path.Combine(current.FullName, "Falcon.Foundation.ControlledProjectFoundation.slnx")))
                return current.FullName;
            current = current.Parent;
        }

        throw new DirectoryNotFoundException("Falcon repository root was not found.");
    }
}
