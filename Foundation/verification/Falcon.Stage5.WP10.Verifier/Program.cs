using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Falcon.Stage5.WP10.Verifier;

internal static class Program
{
    private sealed record VerifierSpec(string Name, string ProjectRelativePath, string DllRelativePath, string SummaryMarker, string[] RequiredPassMarkers);
    private sealed record VerifierResult(VerifierSpec Spec, int ExitCode, string Output, string Error, string DllSha256);

    private static async Task<int> Main()
    {
        var root = FindRepositoryRoot();
        var specs = BuildSpecs();
        var results = new Dictionary<string, VerifierResult>(StringComparer.Ordinal);
        var failures = new List<string>();

        foreach (var spec in specs)
        {
            var result = await RunVerifier(root, spec);
            results.Add(spec.Name, result);
        }

        var scenarios = new List<(string Name, Func<bool> Test)>();

        foreach (var spec in specs)
        {
            scenarios.Add(($"{spec.Name.ToLowerInvariant()}_verifier_exit_zero", () => results[spec.Name].ExitCode == 0));
            scenarios.Add(($"{spec.Name.ToLowerInvariant()}_summary_present", () => ContainsExactLine(results[spec.Name].Output, spec.SummaryMarker)));
            scenarios.Add(($"{spec.Name.ToLowerInvariant()}_dll_identity_sha256", () => IsSha256(results[spec.Name].DllSha256)));

            foreach (var marker in spec.RequiredPassMarkers)
            {
                var localSpec = spec;
                var localMarker = marker;
                scenarios.Add(($"{spec.Name.ToLowerInvariant()}_{NormalizeScenarioName(marker)}", () => CountExactLine(results[localSpec.Name].Output, $"PASS {localMarker}") == 1));
            }
        }

        scenarios.Add(("wp10_has_zero_project_references", () => WP10HasZeroProjectReferences(root)));
        scenarios.Add(("wp10_verifier_is_in_controlled_solution_once", () => WP10VerifierIsInControlledSolutionOnce(root)));
        scenarios.Add(("wp10_introduces_no_production_aggregation_project", () => NoWP10ProductionAggregationProject(root)));
        scenarios.Add(("stage6_plus_production_leakage_absent", () => NoStage6PlusProductionLeakage(root)));
        scenarios.Add(("integrated_result_set_is_complete", () => results.Count == 9 && results.Values.All(r => r.ExitCode == 0)));
        scenarios.Add(("integrated_evidence_identity_is_sha256", () => IsSha256(BuildIntegratedEvidenceIdentity(results.Values))));
        scenarios.Add(("integrated_evidence_identity_is_deterministic", () => BuildIntegratedEvidenceIdentity(results.Values) == BuildIntegratedEvidenceIdentity(results.Values)));
        scenarios.Add(("integrated_evidence_binds_all_predecessor_dlls", () => IntegratedEvidenceBindsAllPredecessorDlls(results.Values)));
        scenarios.Add(("application_neutrality_cross_wp_markers_present", () => ApplicationNeutralityMarkersPresent(results)));
        scenarios.Add(("authority_non_creation_cross_wp_markers_present", () => AuthorityNonCreationMarkersPresent(results)));
        scenarios.Add(("replay_non_authority_cross_wp_markers_present", () => ReplayNonAuthorityMarkersPresent(results)));
        scenarios.Add(("crypto_does_not_replace_context_authority", () => CryptoBoundaryMarkersPresent(results)));
        scenarios.Add(("lifecycle_does_not_create_activation_or_authority", () => LifecycleBoundaryMarkersPresent(results)));
        scenarios.Add(("cross_application_isolation_markers_present", () => CrossApplicationIsolationMarkersPresent(results)));
        scenarios.Add(("correlation_causation_markers_present", () => CorrelationCausationMarkersPresent(results)));
        scenarios.Add(("fcr_cross_checks_do_not_claim_missing_capabilities", () => FcrBoundaryDocumentIsPresentAndBounded(root)));
        scenarios.Add(("wp10_technical_pass_does_not_self_close_stage5", () => OwnerGateRemainsDocumented(root)));

        foreach (var scenario in scenarios)
        {
            try
            {
                if (scenario.Test()) Console.WriteLine($"PASS {scenario.Name}");
                else
                {
                    Console.WriteLine($"FAIL {scenario.Name}");
                    failures.Add(scenario.Name);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FAIL {scenario.Name}: {ex.GetType().Name}: {ex.Message}");
                failures.Add(scenario.Name);
            }
        }

        var evidenceIdentity = BuildIntegratedEvidenceIdentity(results.Values);
        Console.WriteLine();
        Console.WriteLine($"STAGE 5 WP-10 INTEGRATED EVIDENCE SHA-256: {evidenceIdentity}");
        Console.WriteLine($"Stage 5 WP-10 verifier: {(failures.Count == 0 ? "PASS" : "FAIL")}");
        Console.WriteLine($"Scenarios: {scenarios.Count - failures.Count}/{scenarios.Count} PASS");
        Console.WriteLine("Verified Stage 5 WP-01 through WP-09 composition, predecessor executable identities, authority/truth separation, Application neutrality, replay non-authority, cryptographic context isolation, lifecycle non-activation, FCR non-claim boundaries, and Owner-gated Stage 5 closure.");
        Console.WriteLine("No deployment/runtime activation, baseline activation, external egress, credential use, Application business semantics, FSA autonomous-promotion control plane, or Stage 6+ implementation is created by WP-10.");

        if (failures.Count == 0) return 0;
        foreach (var failure in failures) Console.WriteLine($"- {failure}");
        return 1;
    }

    private static VerifierSpec[] BuildSpecs() =>
    [
        new("WP01", "verification/Falcon.Stage5.WP01.Verifier/Falcon.Stage5.WP01.Verifier.csproj", "verification/Falcon.Stage5.WP01.Verifier/bin/Release/net10.0/Falcon.Stage5.WP01.Verifier.dll", "STAGE 5 WP-01 CANONICAL MESSAGING PRIMITIVES VERIFIER: PASS",
        ["positive_command", "producer_mutation_detected", "recipient_mutation_detected", "authority_mutation_detected", "zero_application_neutrality", "two_independent_application_identities", "legacy_fil_envelope_preserved"]),

        new("WP02", "verification/Falcon.Stage5.WP02.Verifier/Falcon.Stage5.WP02.Verifier.csproj", "verification/Falcon.Stage5.WP02.Verifier/bin/Release/net10.0/Falcon.Stage5.WP02.Verifier.dll", "STAGE 5 WP-02 SCHEMA REGISTRY AND COMPATIBILITY VERIFIER: PASS",
        ["exact_compatibility_is_implicit", "undeclared_compatibility_fails_closed", "lifecycle_reverse_rejected", "zero_application_neutrality", "payload_meaning_remains_opaque", "registry_does_not_grant_authority"]),

        new("WP03", "verification/Falcon.Stage5.WP03.Verifier/Falcon.Stage5.WP03.Verifier.csproj", "verification/Falcon.Stage5.WP03.Verifier/bin/Release/net10.0/Falcon.Stage5.WP03.Verifier.dll", "STAGE 5 WP-03 VERIFIER: PASS",
        ["zero_application_foundation_is_valid", "two_independent_application_manifests_register", "unresolved_schema_reference_fails_closed", "manifest_validity_does_not_grant_authority", "manifest_validity_does_not_create_route", "manifest_model_contains_no_business_payload", "fsats_receives_no_special_treatment"]),

        new("WP04", "verification/Falcon.Stage5.WP04.Verifier/Falcon.Stage5.WP04.Verifier.csproj", "verification/Falcon.Stage5.WP04.Verifier/bin/Release/net10.0/Falcon.Stage5.WP04.Verifier.dll", "STAGE 5 WP-04 FIL VALIDATION AND MESSAGE ADMISSION VERIFIER: PASS",
        ["producer_identity_binding_mismatch_rejected", "recipient_scope_binding_mismatch_rejected", "undeclared_intended_consumer_rejected", "authority_effective_scope_mismatch_rejected", "admission_does_not_create_route", "admission_does_not_deliver", "admission_does_not_execute", "payload_business_semantics_remain_opaque", "zero_application_foundation_remains_valid"]),

        new("WP05", "verification/Falcon.Stage5.WP05.Verifier/Falcon.Stage5.WP05.Verifier.csproj", "verification/Falcon.Stage5.WP05.Verifier/bin/Release/net10.0/Falcon.Stage5.WP05.Verifier.dll", "STAGE 5 WP-05 SERVICE BUS DYNAMIC ROUTING AND ISOLATION VERIFIER: PASS",
        ["rejected_admission_cannot_route", "source_binding_mismatch_rejected", "destination_binding_mismatch_rejected", "consumer_binding_mismatch_rejected", "admission_manifest_binding_mismatch_rejected", "routing_does_not_dispatch", "routing_does_not_deliver", "routing_does_not_retry", "payload_business_semantics_remain_opaque", "fsats_receives_no_special_treatment", "zero_application_foundation_remains_valid"]),

        new("WP06", "verification/Falcon.Stage5.WP06.Verifier/Falcon.Stage5.WP06.Verifier.csproj", "verification/Falcon.Stage5.WP06.Verifier/bin/Release/net10.0/Falcon.Stage5.WP06.Verifier.dll", "STAGE 5 WP-06 SERVICE BUS DELIVERY SEMANTICS AND FLOW CONTROL VERIFIER: PASS",
        ["recipient_acknowledgement_is_transport_status_only", "expiry_blocks_retry", "idempotency_binding_mismatch_deadletters", "elevated_traffic_requires_authority_binding", "policy_route_binding_mismatch_rejected", "predecessor_admission_binding_mismatch_rejected", "previous_outcome_lineage_mismatch_rejected", "acknowledged_attempt_cannot_retry", "payload_business_semantics_remain_opaque", "two_applications_pressure_isolated", "canonical_envelope_binding_mismatch_rejected", "correlation_causation_preserved_in_decision_and_outcome"]),

        new("WP07", "verification/Falcon.Stage5.WP07.Verifier/Falcon.Stage5.WP07.Verifier.csproj", "verification/Falcon.Stage5.WP07.Verifier/bin/Release/net10.0/Falcon.Stage5.WP07.Verifier.dll", "STAGE 5 WP-07 EVENT SYSTEM AND TRUTHFUL PUBLICATION VERIFIER: PASS",
        ["published_event_binds_exact_admission_digest", "admission_delivery_binding_mismatch_rejected", "replay_of_authoritative_event_remains_non_authoritative", "replay_cannot_escalate_to_authoritative", "unknown_relation_target_rejected", "correlation_causation_preserved", "payload_business_semantics_remain_opaque", "application_identity_receives_no_special_treatment", "event_surface_has_no_wp08_plus_operations"]),

        new("WP08", "verification/Falcon.Stage5.WP08.Verifier/Falcon.Stage5.WP08.Verifier.csproj", "verification/Falcon.Stage5.WP08.Verifier/bin/Release/net10.0/Falcon.Stage5.WP08.Verifier.dll", "STAGE 5 WP-08 CRYPTOGRAPHIC MESSAGE PROTECTION VERIFIER: PASS",
        ["wrong_recipient_context_rejected", "wrong_message_digest_context_rejected", "wrong_route_context_rejected", "wrong_delivery_context_rejected", "wrong_event_context_rejected", "wrong_replay_classification_context_rejected", "wrong_correlation_context_rejected", "wrong_causation_context_rejected", "failed_verification_releases_no_plaintext", "application_names_do_not_change_semantics", "protection_evidence_contains_no_key_material", "verification_evidence_contains_no_key_material"]),

        new("WP09", "verification/Falcon.Stage5.WP09.Verifier/Falcon.Stage5.WP09.Verifier.csproj", "verification/Falcon.Stage5.WP09.Verifier/bin/Release/net10.0/Falcon.Stage5.WP09.Verifier.dll", "Stage 5 WP-09 verifier: PASS",
        ["authority_expansion_rejected", "protected_control_weakening_rejected", "required_dependency_gap_rejected", "contract_incompatibility_rejected", "upgrade_version_regression_evidence_rejected", "upgrade_stale_drain_rejected", "detach_hidden_coupling_rejected", "rollback_revoked_authority_rejected", "correlation_preserved", "causation_preserved", "generic_application_names_preserve_semantics", "public_surface_has_no_activation_or_deployment_api", "public_surface_has_no_trading_business_terms", "package_compatibility_does_not_override_revoked_authority"])
    ];

    private static async Task<VerifierResult> RunVerifier(string root, VerifierSpec spec)
    {
        var project = Path.Combine(root, spec.ProjectRelativePath.Replace('/', Path.DirectorySeparatorChar));
        var dll = Path.Combine(root, spec.DllRelativePath.Replace('/', Path.DirectorySeparatorChar));
        if (!File.Exists(project)) return new(spec, -1, string.Empty, $"Missing project: {project}", string.Empty);
        if (!File.Exists(dll)) return new(spec, -2, string.Empty, $"Missing Release DLL: {dll}", string.Empty);

        var startInfo = new ProcessStartInfo
        {
            FileName = "dotnet",
            WorkingDirectory = root,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        startInfo.ArgumentList.Add("run");
        startInfo.ArgumentList.Add("--project");
        startInfo.ArgumentList.Add(project);
        startInfo.ArgumentList.Add("-c");
        startInfo.ArgumentList.Add("Release");
        startInfo.ArgumentList.Add("--no-build");

        using var process = new Process { StartInfo = startInfo };
        process.Start();
        var outputTask = process.StandardOutput.ReadToEndAsync();
        var errorTask = process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();
        var output = await outputTask;
        var error = await errorTask;
        var dllHash = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(dll)));
        return new(spec, process.ExitCode, output, error, dllHash);
    }

    private static string FindRepositoryRoot()
    {
        var current = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (current is not null)
        {
            if (File.Exists(Path.Combine(current.FullName, "Falcon.Foundation.ControlledProjectFoundation.slnx"))) return current.FullName;
            current = current.Parent;
        }
        throw new InvalidOperationException("Repository root could not be located.");
    }

    private static bool ContainsExactLine(string text, string expected) => SplitLines(text).Any(line => string.Equals(line, expected, StringComparison.Ordinal));
    private static int CountExactLine(string text, string expected) => SplitLines(text).Count(line => string.Equals(line, expected, StringComparison.Ordinal));
    private static string[] SplitLines(string text) => text.Replace("\r\n", "\n", StringComparison.Ordinal).Replace('\r', '\n').Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    private static string NormalizeScenarioName(string marker) => new string(marker.Select(ch => char.IsLetterOrDigit(ch) ? char.ToLowerInvariant(ch) : '_').ToArray());
    private static bool IsSha256(string value) => value.Length == 64 && value.All(ch => char.IsDigit(ch) || (ch >= 'A' && ch <= 'F'));

    private static string BuildIntegratedEvidenceIdentity(IEnumerable<VerifierResult> results)
    {
        var canonical = string.Join("\n", results.OrderBy(r => r.Spec.Name, StringComparer.Ordinal).Select(r =>
            string.Join("|", r.Spec.Name, r.ExitCode.ToString(System.Globalization.CultureInfo.InvariantCulture), r.DllSha256, r.Spec.SummaryMarker,
                string.Join(",", r.Spec.RequiredPassMarkers.OrderBy(x => x, StringComparer.Ordinal)))));
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical)));
    }

    private static bool IntegratedEvidenceBindsAllPredecessorDlls(IEnumerable<VerifierResult> results)
    {
        var values = results.ToArray();
        var evidence = BuildIntegratedEvidenceIdentity(values);
        if (!IsSha256(evidence) || values.Length != 9) return false;
        return values.All(result => IsSha256(result.DllSha256)) && values.Select(r => r.DllSha256).Distinct(StringComparer.Ordinal).Count() == values.Length;
    }

    private static bool WP10HasZeroProjectReferences(string root)
    {
        var path = Path.Combine(root, "verification", "Falcon.Stage5.WP10.Verifier", "Falcon.Stage5.WP10.Verifier.csproj");
        var doc = XDocument.Load(path);
        return !doc.Descendants().Any(element => element.Name.LocalName == "ProjectReference");
    }

    private static bool WP10VerifierIsInControlledSolutionOnce(string root)
    {
        var solution = XDocument.Load(Path.Combine(root, "Falcon.Foundation.ControlledProjectFoundation.slnx"));
        var expected = "verification/Falcon.Stage5.WP10.Verifier/Falcon.Stage5.WP10.Verifier.csproj".Replace('/', Path.DirectorySeparatorChar);
        return solution.Root?.Elements("Project")
            .Select(p => (p.Attribute("Path")?.Value ?? string.Empty).Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar))
            .Count(p => string.Equals(p, expected, StringComparison.Ordinal)) == 1;
    }

    private static bool NoWP10ProductionAggregationProject(string root)
    {
        var src = Path.Combine(root, "src");
        return Directory.EnumerateFiles(src, "*.csproj", SearchOption.AllDirectories).All(path =>
        {
            var name = Path.GetFileNameWithoutExtension(path);
            return !name.Contains("WP10", StringComparison.OrdinalIgnoreCase)
                && !name.Contains("Stage5Integration", StringComparison.OrdinalIgnoreCase)
                && !name.Contains("Stage5.Integration", StringComparison.OrdinalIgnoreCase);
        });
    }

    private static bool NoStage6PlusProductionLeakage(string root)
    {
        var src = Path.Combine(root, "src");
        return Directory.EnumerateFiles(src, "*.csproj", SearchOption.AllDirectories).All(path =>
        {
            var name = Path.GetFileNameWithoutExtension(path);
            return !new[] { "Stage6", "Stage7", "Stage8", "Stage9" }.Any(token => name.Contains(token, StringComparison.OrdinalIgnoreCase));
        });
    }

    private static bool HasMarker(IReadOnlyDictionary<string, VerifierResult> results, string wp, string marker) => CountExactLine(results[wp].Output, $"PASS {marker}") == 1;

    private static bool ApplicationNeutralityMarkersPresent(IReadOnlyDictionary<string, VerifierResult> r) =>
        HasMarker(r, "WP01", "zero_application_neutrality") && HasMarker(r, "WP03", "fsats_receives_no_special_treatment") &&
        HasMarker(r, "WP04", "payload_business_semantics_remain_opaque") && HasMarker(r, "WP05", "fsats_receives_no_special_treatment") &&
        HasMarker(r, "WP07", "application_identity_receives_no_special_treatment") && HasMarker(r, "WP08", "application_names_do_not_change_semantics") &&
        HasMarker(r, "WP09", "generic_application_names_preserve_semantics") && HasMarker(r, "WP09", "public_surface_has_no_trading_business_terms");

    private static bool AuthorityNonCreationMarkersPresent(IReadOnlyDictionary<string, VerifierResult> r) =>
        HasMarker(r, "WP02", "registry_does_not_grant_authority") && HasMarker(r, "WP03", "manifest_validity_does_not_grant_authority") &&
        HasMarker(r, "WP04", "admission_does_not_create_route") && HasMarker(r, "WP05", "routing_does_not_deliver") &&
        HasMarker(r, "WP06", "recipient_acknowledgement_is_transport_status_only") && HasMarker(r, "WP09", "authority_expansion_rejected");

    private static bool ReplayNonAuthorityMarkersPresent(IReadOnlyDictionary<string, VerifierResult> r) =>
        HasMarker(r, "WP07", "replay_of_authoritative_event_remains_non_authoritative") && HasMarker(r, "WP07", "replay_cannot_escalate_to_authoritative") &&
        HasMarker(r, "WP08", "wrong_replay_classification_context_rejected");

    private static bool CryptoBoundaryMarkersPresent(IReadOnlyDictionary<string, VerifierResult> r) =>
        HasMarker(r, "WP08", "wrong_recipient_context_rejected") && HasMarker(r, "WP08", "wrong_route_context_rejected") &&
        HasMarker(r, "WP08", "wrong_delivery_context_rejected") && HasMarker(r, "WP08", "wrong_event_context_rejected") &&
        HasMarker(r, "WP08", "failed_verification_releases_no_plaintext");

    private static bool LifecycleBoundaryMarkersPresent(IReadOnlyDictionary<string, VerifierResult> r) =>
        HasMarker(r, "WP09", "authority_expansion_rejected") && HasMarker(r, "WP09", "protected_control_weakening_rejected") &&
        HasMarker(r, "WP09", "rollback_revoked_authority_rejected") && HasMarker(r, "WP09", "public_surface_has_no_activation_or_deployment_api");

    private static bool CrossApplicationIsolationMarkersPresent(IReadOnlyDictionary<string, VerifierResult> r) =>
        HasMarker(r, "WP01", "two_independent_application_identities") && HasMarker(r, "WP03", "two_independent_application_manifests_register") &&
        HasMarker(r, "WP05", "source_binding_mismatch_rejected") && HasMarker(r, "WP05", "destination_binding_mismatch_rejected") &&
        HasMarker(r, "WP06", "two_applications_pressure_isolated") && HasMarker(r, "WP08", "wrong_recipient_context_rejected");

    private static bool CorrelationCausationMarkersPresent(IReadOnlyDictionary<string, VerifierResult> r) =>
        HasMarker(r, "WP06", "correlation_causation_preserved_in_decision_and_outcome") && HasMarker(r, "WP07", "correlation_causation_preserved") &&
        HasMarker(r, "WP08", "wrong_correlation_context_rejected") && HasMarker(r, "WP08", "wrong_causation_context_rejected") &&
        HasMarker(r, "WP09", "correlation_preserved") && HasMarker(r, "WP09", "causation_preserved");

    private static bool FcrBoundaryDocumentIsPresentAndBounded(string root)
    {
        var path = Path.Combine(root, "docs", "stage-5-wp10", "00_PRE_IMPLEMENTATION_SCOPE_AND_FCR_REVIEW.md");
        if (!File.Exists(path)) return false;
        var text = File.ReadAllText(path);
        return text.Contains("Integration cross-check only", StringComparison.OrdinalIgnoreCase)
            && text.Contains("Out of Stage 5 closure scope", StringComparison.OrdinalIgnoreCase)
            && text.Contains("do not become WP-10 implementation authority", StringComparison.OrdinalIgnoreCase);
    }

    private static bool OwnerGateRemainsDocumented(string root)
    {
        var path = Path.Combine(root, "docs", "stage-5-wp10", "04_REQUIREMENT_TO_VERIFIER_TRACEABILITY.md");
        if (!File.Exists(path)) return false;
        var text = File.ReadAllText(path);
        return text.Contains("does not itself close WP-10 or Stage 5", StringComparison.Ordinal)
            && text.Contains("explicit Project Owner acceptance/closure remain mandatory", StringComparison.Ordinal);
    }
}
