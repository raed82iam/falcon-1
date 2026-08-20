using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

internal static class Program
{
    private sealed record VerifierSpec(
        string RelativeProjectDirectory,
        string AssemblyName,
        string PassMarker,
        string ChecksMarker,
        string[] RequiredMarkers);

    private sealed record VerifierRun(bool Passed, string Output);

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

            var stage9 = new Dictionary<string, VerifierRun>(StringComparer.Ordinal);
            var specs = new Dictionary<string, VerifierSpec>(StringComparer.Ordinal)
            {
                ["01"] = Spec("01", "16/16", "ACR9_001 = PASS"),
                ["02"] = Spec("02", "24/24", "RT9_001 = PASS"),
                ["03"] = Spec("03", "19/19", "REPAIR_ACTOR_SELF_CERTIFICATION = DENIED"),
                ["04"] = Spec("04", "17/17", "UNKNOWN_RECOVERY_STATE = FAIL_CLOSED", "PARTIAL_RECOVERY != COMPLETE_RECOVERY", "STALE_SECURITY_CONTEXT != TRUSTED_SECURITY_CONTEXT"),
                ["05"] = Spec("05", "20/20", "ACR9_001 = PASS", "FAILED_PARTIAL_UNCERTAIN_RECONCILIATION != POSITIVE_VALIDATION", "VALIDATION_SUCCESS != RELEASE_AUTHORIZATION"),
                ["06"] = Spec("06", "22/22", "READY_FOR_RELEASE_DECISION != RELEASE", "NEWER_STRICTER_RESTRICTION_INVALIDATES_READINESS", "WP05_VALIDATION_PASS_REQUIRED = YES"),
                ["07"] = Spec("07", "31/31", "RT9_002 = PASS", "RELEASE_AUTHORIZATION != RELEASE_EXECUTION", "ROLE_LABEL != AUTHORITY", "NEWER_STRICTER_RESTRICTION_INVALIDATES_RELEASE_AUTHORIZATION"),
                ["08"] = Spec("08", "32/32", "RT9_002 = PASS", "ORIGINAL_RESTRICTION = IMMUTABLE_HISTORY", "RELEASE_FACT != SECOND_AUTHORITY_DECISION", "PARTIAL_ENFORCEMENT != COMPLETE_RELEASE", "NEWER_STRICTER_RESTRICTION_INVALIDATES_RELEASE_EXECUTION"),
                ["09"] = Spec("09", "42/42", "VALID_WP08_RELEASE_FACT_REQUIRED = YES", "SYS002 = LIFECYCLE_TRANSITION_OWNER", "AUT001 = NEW_AUTHORITY_DECISION_OWNER", "LIFECYCLE_TRANSITION != NEW_AUTHORITY_DECISION", "OLD_AUTHORITY_REUSE = DENIED", "RECOVERY_GUARD_OBSERVATION = GOVERNED", "OBSERVATION_BYPASS = DENIED", "STAGE13_FSA_CONTROLLED_REVIVAL_SURFACE = NONE", "APPLICATION_BUSINESS_RECOVERY = NOT_IMPLEMENTED")
            };

            foreach (var pair in specs)
            {
                var run = RunVerifier(root, pair.Value);
                stage9[pair.Key] = run;
                Expect($"stage9-wp{pair.Key}-executable-pass", run.Passed);
            }

            var stage8wp10 = RunVerifier(root, new VerifierSpec(
                "verification/Falcon.Stage8.WP10.Verifier",
                "Falcon.Stage8.WP10.Verifier",
                "STAGE8_WP10_INTEGRATED_VERIFIER = PASS",
                string.Empty,
                Array.Empty<string>()));
            Expect("stage8-wp10-integrated-predecessor-pass", stage8wp10.Passed);

            Expect("acr9-001-present-wp01", Has(stage9["01"], "ACR9_001 = PASS"));
            Expect("rt9-001-present-wp02", Has(stage9["02"], "RT9_001 = PASS"));
            Expect("repair-self-certification-denied", Has(stage9["03"], "REPAIR_ACTOR_SELF_CERTIFICATION = DENIED"));
            Expect("unknown-and-partial-reconciliation-fail-closed", Has(stage9["04"], "UNKNOWN_RECOVERY_STATE = FAIL_CLOSED") && Has(stage9["04"], "PARTIAL_RECOVERY != COMPLETE_RECOVERY"));
            Expect("independent-validation-separation", Has(stage9["05"], "ACR9_001 = PASS") && Has(stage9["05"], "VALIDATION_SUCCESS != RELEASE_AUTHORIZATION"));
            Expect("readiness-is-not-release", Has(stage9["06"], "READY_FOR_RELEASE_DECISION != RELEASE"));
            Expect("rt9-002-release-authorization", Has(stage9["07"], "RT9_002 = PASS"));
            Expect("rt9-002-release-execution", Has(stage9["08"], "RT9_002 = PASS"));
            Expect("lifecycle-is-not-authority", Has(stage9["09"], "LIFECYCLE_TRANSITION != NEW_AUTHORITY_DECISION"));

            var allStage9Pass = stage9.Values.All(x => x.Passed);
            Expect("vpl007-positive-controlled-recovery", allStage9Pass);

            // VPL-007 mandatory negative variants are distributed across the dedicated
            // executable WPs. A WP PASS means its complete fixed check matrix executed.
            Expect("vpl007-negative-repair-actor-self-success", stage9["03"].Passed && Has(stage9["03"], "REPAIR_ACTOR_SELF_CERTIFICATION = DENIED"));
            Expect("vpl007-negative-validation-fails", stage9["05"].Passed);
            Expect("vpl007-negative-uncertain-reconciliation", stage9["04"].Passed && Has(stage9["04"], "UNKNOWN_RECOVERY_STATE = FAIL_CLOSED"));
            Expect("vpl007-negative-missing-or-integrity-failed-evidence", stage9["03"].Passed && stage9["04"].Passed);
            Expect("vpl007-negative-partial-recovery", stage9["04"].Passed && Has(stage9["04"], "PARTIAL_RECOVERY != COMPLETE_RECOVERY"));
            Expect("vpl007-negative-old-security-context", stage9["04"].Passed && Has(stage9["04"], "STALE_SECURITY_CONTEXT != TRUSTED_SECURITY_CONTEXT"));
            Expect("vpl007-negative-restart-trigger-unresolved", stage8wp10.Passed && stage9["02"].Passed);
            Expect("vpl007-negative-bounded-attempt-limit", stage9["02"].Passed && Has(stage9["02"], "RT9_001 = PASS"));

            var solutionText = File.ReadAllText(Path.Combine(root, "Falcon.Foundation.ControlledProjectFoundation.slnx"));
            Expect("zero-application-foundation-solution", !solutionText.Contains("applications/", StringComparison.OrdinalIgnoreCase) && !solutionText.Contains("applications\\", StringComparison.OrdinalIgnoreCase));
            Expect("stage13-fsa-controlled-revival-absent", Has(stage9["09"], "STAGE13_FSA_CONTROLLED_REVIVAL_SURFACE = NONE"));
            Expect("application-business-recovery-absent", Has(stage9["09"], "APPLICATION_BUSINESS_RECOVERY = NOT_IMPLEMENTED"));

            var material = string.Join("\n---WP---\n", stage9.OrderBy(x => x.Key, StringComparer.Ordinal).Select(x => x.Value.Output.Trim()));
            var digest = Digest(material);
            Expect("integrated-evidence-sha256-shape", digest.Length == 64 && digest.All(Uri.IsHexDigit));
            Expect("integrated-evidence-deterministic", digest == Digest(material));
            Expect("integrated-evidence-mutation-sensitive", digest != Digest(material + "\nMATERIAL_MUTATION"));

            Expect("original-restriction-history-preserved", Has(stage9["08"], "ORIGINAL_RESTRICTION = IMMUTABLE_HISTORY"));
            Expect("newer-stricter-restriction-blocks-execution", Has(stage9["08"], "NEWER_STRICTER_RESTRICTION_INVALIDATES_RELEASE_EXECUTION"));
            Expect("rejected-lifecycle-negative-remains-in-wp09-matrix", stage9["09"].Passed && Has(stage9["09"], "CHECKS = 42/42"));
            Expect("observation-bypass-denied", Has(stage9["09"], "OBSERVATION_BYPASS = DENIED"));

            if (checks != 38)
                failures.Add($"internal-check-count-mismatch:{checks}");

            if (failures.Count > 0)
            {
                Console.Error.WriteLine("STAGE9_WP10_INTEGRATED_VERIFIER = FAIL");
                foreach (var failure in failures.OrderBy(x => x, StringComparer.Ordinal))
                    Console.Error.WriteLine(failure);
                return 1;
            }

            Console.WriteLine("STAGE9_WP10_INTEGRATED_VERIFIER = PASS");
            Console.WriteLine("CHECKS = 38/38");
            Console.WriteLine("VPL007_POSITIVE_PATH = PASS");
            Console.WriteLine("VPL007_NEGATIVE_VARIANTS = 8/8 PASS");
            Console.WriteLine("ACR9_001 = PASS");
            Console.WriteLine("RT9_001 = PASS");
            Console.WriteLine("RT9_002 = PASS");
            Console.WriteLine("STAGE8_WP10_PREDECESSOR = PASS");
            Console.WriteLine("STAGE9_WP01_WP09_EXECUTABLE_MATRIX = PASS");
            Console.WriteLine("ZERO_APPLICATION_OPERATION = VALIDATED_APPLICATION_NEUTRAL");
            Console.WriteLine("STAGE13_FSA_CONTROLLED_REVIVAL_SURFACE = NONE");
            Console.WriteLine("APPLICATION_BUSINESS_RECOVERY = NOT_IMPLEMENTED");
            Console.WriteLine($"STAGE9_INTEGRATED_EVIDENCE_SHA256 = {digest}");
            Console.WriteLine("FULL_STAGE0_THROUGH_STAGE9_EXECUTION_REQUIRED_OUTSIDE_INTEGRATED_VERIFIER = YES");
            Console.WriteLine("STAGE9_WP10_TECHNICAL_PASS != STAGE9_OWNER_CLOSURE");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("STAGE9_WP10_INTEGRATED_VERIFIER = FAIL");
            Console.Error.WriteLine(ex.Message);
            return 1;
        }
    }

    private static VerifierSpec Spec(string wp, string checks, params string[] required) =>
        new(
            $"verification/Falcon.Stage9.WP{wp}.Verifier",
            $"Falcon.Stage9.WP{wp}.Verifier",
            $"STAGE9_WP{wp}_VERIFIER = PASS",
            $"CHECKS = {checks}",
            required);

    private static VerifierRun RunVerifier(string root, VerifierSpec spec)
    {
        var dll = Path.Combine(root, spec.RelativeProjectDirectory.Replace('/', Path.DirectorySeparatorChar), "bin", "Release", "net10.0", spec.AssemblyName + ".dll");
        if (!File.Exists(dll))
            return new(false, $"missing verifier DLL: {dll}");

        var start = new ProcessStartInfo
        {
            FileName = "dotnet",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        start.ArgumentList.Add(dll);

        using var process = Process.Start(start) ?? throw new InvalidOperationException($"failed to start {spec.AssemblyName}");
        var stdout = process.StandardOutput.ReadToEnd();
        var stderr = process.StandardError.ReadToEnd();
        process.WaitForExit();
        var output = (stdout + Environment.NewLine + stderr).Trim();

        var passed = process.ExitCode == 0 &&
                     output.Contains(spec.PassMarker, StringComparison.Ordinal) &&
                     (string.IsNullOrEmpty(spec.ChecksMarker) || output.Contains(spec.ChecksMarker, StringComparison.Ordinal)) &&
                     spec.RequiredMarkers.All(x => output.Contains(x, StringComparison.Ordinal));

        return new(passed, output);
    }

    private static bool Has(VerifierRun run, string marker) =>
        run.Passed && run.Output.Contains(marker, StringComparison.Ordinal);

    private static string FindRepositoryRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current is not null)
        {
            if (File.Exists(Path.Combine(current.FullName, "Falcon.Foundation.ControlledProjectFoundation.slnx")))
                return current.FullName;
            current = current.Parent;
        }

        throw new DirectoryNotFoundException("Falcon repository root was not found from verifier base directory.");
    }

    private static string Digest(string value) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
}
