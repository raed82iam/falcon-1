using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Falcon.Stage6.WP10.Verifier;

internal static class Program
{
    private static int _passed;
    private static int _failed;

    private static readonly string[] ManifestColumns =
    {
        "manifest_version", "stage_id", "work_package", "accepted_scope_label",
        "closure_evidence_kind", "closure_evidence_locator", "closure_evidence_sha256",
        "closure_decision_commit_sha", "accepted_technical_baseline_sha", "executable_evidence_sha256",
        "final_red_team_disposition", "application_compatibility_disposition", "historical_gate_note"
    };

    private static readonly string[] CensusColumns =
    {
        "census_version", "captured_at_utc", "issue_number", "issue_title", "status",
        "waiting_on", "target_foundation_stage_wp", "stage6_relevance", "issue_updated_at"
    };

    private static readonly string[] DispositionColumns =
    {
        "snapshot_version", "census_sha256", "issue_number", "status", "waiting_on",
        "target_foundation_stage_wp", "stage6_closure_blocking_disposition", "disposition_basis"
    };

    private static readonly string[] AllowedWaitingOn = { "FOUNDATION", "APPLICATION", "OWNER", "NONE" };
    private static readonly string[] AllowedStage6Relevance = { "STAGE6_RELEVANT", "NOT_STAGE6_RELEVANT" };

    private static readonly IReadOnlyDictionary<string, string> ExpectedScopeByWp =
        new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["WP-01"] = "Canonical Resource Governance Primitives",
            ["WP-02"] = "Foundation Resource Truth, Protection Floors and Recovery Reserves",
            ["WP-03"] = "Application Allocation, Quota, Ceiling and Isolation",
            ["WP-04"] = "Cross-Application Priority and Technical Criticality Governance",
            ["WP-05"] = "Resource Pressure, Preemption Eligibility and Enforcement-State Truth",
            ["WP-06"] = "Additional Resource Request and Decision Boundary",
            ["WP-07"] = "Reclamation, Redistribution, Rebalance and Restoration",
            ["WP-08"] = "Per-Application Resource State and Load-Shedding Signal Boundary",
            ["WP-09"] = "Integration, Cross-Subsystem Consumption and Hardening"
        };

    private static int Main()
    {
        var root = FindRepositoryRoot();
        var docs = Path.Combine(root, "docs", "stage-6-wp10");
        var manifestPath = Path.Combine(docs, "STAGE6_CLOSURE_MANIFEST.tsv");
        var censusPath = Path.Combine(docs, "STAGE6_FCR_CENSUS.tsv");
        var dispositionPath = Path.Combine(docs, "STAGE6_FCR_DISPOSITION_SNAPSHOT.tsv");

        Run("closure_manifest_valid", () => ValidateManifest(root, manifestPath));
        Run("fcr_census_and_disposition_valid", () => ValidateFcrArtifacts(censusPath, dispositionPath));
        Run("manifest_rejects_missing_wp", NegativeMissingWp);
        Run("manifest_rejects_duplicate_wp", NegativeDuplicateWp);
        Run("manifest_rejects_future_wp", NegativeFutureWp);
        Run("manifest_rejects_wrong_order", NegativeWrongOrder);
        Run("manifest_rejects_wrong_stage", NegativeWrongStage);
        Run("manifest_rejects_wrong_version", NegativeWrongManifestVersion);
        Run("manifest_rejects_blank_required_field", NegativeBlankRequiredField);
        Run("manifest_rejects_invalid_enums", NegativeInvalidEnums);
        Run("manifest_rejects_false_historical_classification", NegativeFalseHistoricalClassification);
        Run("manifest_rejects_wrong_functional_scope", NegativeWrongFunctionalScope);
        Run("manifest_rejects_malformed_sha", NegativeMalformedSha);
        Run("manifest_rejects_closure_substitution", () => NegativeClosureSubstitution(root, manifestPath));
        Run("manifest_rejects_canonical_digest_mismatch", () => NegativeCanonicalDigestMismatch(root, manifestPath));
        Run("fcr_snapshot_rejects_missing_stage6_row", NegativeMissingStage6Disposition);
        Run("fcr_snapshot_rejects_extra_row", NegativeExtraDisposition);
        Run("fcr_snapshot_rejects_field_mismatch", NegativeDispositionMismatch);
        Run("fcr_snapshot_rejects_foundation_blocker", NegativeFoundationBlocker);
        Run("fcr_snapshot_rejects_owner_blocker", NegativeOwnerBlocker);
        Run("fcr_rejects_invalid_waiting_on", NegativeInvalidWaitingOn);
        Run("fcr_rejects_invalid_stage6_relevance", NegativeInvalidStage6Relevance);
        Run("fcr_rejects_duplicate_issue", NegativeDuplicateIssue);
        Run("fcr_rejects_invalid_capture_chronology", NegativeInvalidCaptureChronology);
        Run("application_ack_does_not_close_application_or_fcr", ApplicationAckDoesNotCloseApplicationOrFcr);
        Run("deterministic_manifest_identity", DeterministicManifestIdentity);
        Run("deterministic_integrated_closure_identity", () => DeterministicIntegratedClosureIdentity(manifestPath, censusPath, dispositionPath));
        Run("no_stage7_or_authority_claim", NoStage7OrAuthorityClaim);

        Console.WriteLine();
        Console.WriteLine($"STAGE 6 WP-10 VERIFIER: {_passed}/{_passed + _failed} PASS");
        Console.WriteLine($"Failures: {_failed}");
        return _failed == 0 ? 0 : 1;
    }

    private static void ValidateManifest(string root, string path)
    {
        Require(File.Exists(path), "STAGE6_CLOSURE_MANIFEST.tsv missing.");
        var table = Tsv.Read(path, ManifestColumns);
        ValidateManifestRows(table.Rows);
        foreach (var row in table.Rows) ValidateCanonicalEvidenceForRow(root, row);
    }

    private static void ValidateCanonicalEvidenceForRow(string root, Dictionary<string, string> row)
    {
        var wp = row["work_package"];
        var kind = row["closure_evidence_kind"];
        var decision = row["closure_decision_commit_sha"];
        Require(kind == "CANONICAL_CLOSURE_RECORD", $"False historical classification for {wp}; the current Stage 6 predecessor set has canonical closure records.");
        Require(IsGitSha(decision), $"Malformed closure decision commit for {wp}.");
        Require(GitObjectExists(root, decision + "^{commit}"), $"Closure decision commit not found for {wp}: {decision}");
        var relative = row["closure_evidence_locator"];
        Require(IsSafeRelativePath(relative), $"Canonical closure locator must be a safe repository-relative path for {wp}.");
        var full = Path.Combine(root, relative.Replace('/', Path.DirectorySeparatorChar));
        Require(File.Exists(full), $"Canonical closure record missing for {wp}: {relative}");
        Require(IsSha256(row["closure_evidence_sha256"]), $"Canonical closure SHA-256 malformed for {wp}.");
        Require(StringComparer.Ordinal.Equals(Sha256File(full), row["closure_evidence_sha256"]), $"Canonical closure SHA-256 mismatch for {wp}: {relative}");
        Require(GitObjectExists(root, decision + ":" + relative), $"Canonical closure record not present at decision commit for {wp}: {relative}");
        var closureText = File.ReadAllText(full, Encoding.UTF8);
        Require(closureText.Contains(wp, StringComparison.Ordinal), $"Closure evidence substitution detected: declared {wp} but record does not identify that Work Package.");
        Require(closureText.Contains("ACCEPTED_AND_CLOSED", StringComparison.Ordinal), $"Closure record for {wp} does not carry accepted-and-closed disposition evidence.");
    }

    private static void ValidateManifestRows(IReadOnlyList<Dictionary<string, string>> rows)
    {
        Require(rows.Count == 9, "Manifest must contain exactly WP-01 through WP-09.");
        var expected = Enumerable.Range(1, 9).Select(i => $"WP-{i:00}").ToArray();
        var actual = rows.Select(r => r["work_package"]).ToArray();
        Require(expected.SequenceEqual(actual, StringComparer.Ordinal), "Manifest WP set/order mismatch.");
        Require(rows.All(r => r["manifest_version"] == "1"), "manifest_version must be 1.");
        Require(rows.All(r => r["stage_id"] == "STAGE6"), "stage_id must be STAGE6.");
        Require(rows.All(r => !r.Values.Any(string.IsNullOrWhiteSpace)), "Manifest contains blank required value.");
        Require(rows.All(r => r["closure_evidence_kind"] == "CANONICAL_CLOSURE_RECORD"), "Current Stage 6 predecessor set has canonical closure records; historical-reference classification is invalid.");
        Require(rows.All(r => IsGitSha(r["closure_decision_commit_sha"])), "Invalid closure decision commit SHA.");
        Require(rows.All(r => IsGitSha(r["accepted_technical_baseline_sha"])), "Invalid technical baseline SHA.");
        Require(rows.All(r => IsSha256(r["closure_evidence_sha256"])), "Invalid canonical closure evidence SHA-256.");
        Require(rows.All(r => r["executable_evidence_sha256"] == "NOT_APPLICABLE_BY_HISTORICAL_GATE" || IsSha256(r["executable_evidence_sha256"])), "Invalid executable evidence SHA-256.");
        Require(rows.All(r => r["final_red_team_disposition"] is "PASS_0C_0H_0M" or "NOT_APPLICABLE_BY_HISTORICAL_GATE"), "Invalid Red-Team disposition.");
        Require(rows.All(r => r["application_compatibility_disposition"] is "VERIFIED_ACK" or "NOT_APPLICABLE_BY_HISTORICAL_GATE"), "Invalid Application compatibility disposition.");
        Require(rows.All(r => !r["application_compatibility_disposition"].Contains("CLOSED", StringComparison.OrdinalIgnoreCase)), "Application compatibility disposition must never encode Application closure.");
        foreach (var row in rows)
        {
            var wp = row["work_package"];
            Require(ExpectedScopeByWp.TryGetValue(wp, out var expectedScope), $"Unknown Work Package: {wp}");
            Require(StringComparer.Ordinal.Equals(expectedScope, row["accepted_scope_label"]), $"Functional-chain scope mismatch for {wp}.");
            if (row["final_red_team_disposition"] == "NOT_APPLICABLE_BY_HISTORICAL_GATE" || row["application_compatibility_disposition"] == "NOT_APPLICABLE_BY_HISTORICAL_GATE")
                Require(!string.IsNullOrWhiteSpace(row["historical_gate_note"]), $"Historical gate note required for {wp}.");
        }
    }

    private static void ValidateFcrArtifacts(string censusPath, string dispositionPath)
    {
        Require(File.Exists(censusPath), "STAGE6_FCR_CENSUS.tsv missing.");
        Require(File.Exists(dispositionPath), "STAGE6_FCR_DISPOSITION_SNAPSHOT.tsv missing.");
        var census = Tsv.Read(censusPath, CensusColumns);
        var disposition = Tsv.Read(dispositionPath, DispositionColumns);
        ValidateFcrRows(census.Rows, disposition.Rows, Sha256File(censusPath));
    }

    private static void ValidateFcrRows(IReadOnlyList<Dictionary<string, string>> census, IReadOnlyList<Dictionary<string, string>> dispositions, string censusSha)
    {
        Require(census.Count > 0, "FCR census must not be empty.");
        Require(census.All(r => !r.Values.Any(string.IsNullOrWhiteSpace)), "Census contains blank required value.");
        Require(dispositions.All(r => !r.Values.Any(string.IsNullOrWhiteSpace)), "Disposition contains blank required value.");
        Require(census.All(r => AllowedWaitingOn.Contains(r["waiting_on"], StringComparer.Ordinal)), "Census contains invalid Waiting On value.");
        Require(census.All(r => AllowedStage6Relevance.Contains(r["stage6_relevance"], StringComparer.Ordinal)), "Census contains invalid Stage 6 relevance value.");
        Require(dispositions.All(r => AllowedWaitingOn.Contains(r["waiting_on"], StringComparer.Ordinal)), "Disposition contains invalid Waiting On value.");
        Require(census.Select(r => r["issue_number"]).Distinct(StringComparer.Ordinal).Count() == census.Count, "Census contains duplicate issue number.");
        Require(dispositions.Select(r => r["issue_number"]).Distinct(StringComparer.Ordinal).Count() == dispositions.Count, "Disposition contains duplicate issue number.");
        Require(census.All(r => r["issue_title"].StartsWith("[FCR-", StringComparison.Ordinal)), "Census contains a non-FCR issue.");

        var censusVersions = census.Select(r => r["census_version"]).Distinct(StringComparer.Ordinal).ToArray();
        Require(censusVersions.Length == 1, "Census must contain exactly one version value.");
        Require(int.TryParse(censusVersions[0], out var censusVersion) && censusVersion > 0, "Census version must be a positive integer.");
        var snapshotVersions = dispositions.Select(r => r["snapshot_version"]).Distinct(StringComparer.Ordinal).ToArray();
        Require(snapshotVersions.Length == 1, "Disposition snapshot must contain exactly one version value.");
        Require(int.TryParse(snapshotVersions[0], out var snapshotVersion) && snapshotVersion > 0, "Snapshot version must be a positive integer.");
        Require(censusVersion == snapshotVersion, "Census and disposition snapshot versions must match.");

        var captureValues = census.Select(r => r["captured_at_utc"]).Distinct(StringComparer.Ordinal).ToArray();
        Require(captureValues.Length == 1, "Census must contain exactly one capture instant.");
        Require(TryParseUtc(captureValues[0], out var capturedAt), "Census capture time must be a valid UTC instant.");
        foreach (var row in census)
        {
            Require(TryParseUtc(row["issue_updated_at"], out var updatedAt), $"Invalid issue_updated_at for issue {row["issue_number"]}.");
            Require(updatedAt <= capturedAt, $"Issue update occurs after census capture for issue {row["issue_number"]}.");
        }

        Require(IsSha256(censusSha), "Computed census SHA-256 malformed.");
        Require(dispositions.All(r => StringComparer.Ordinal.Equals(r["census_sha256"], censusSha)), "Disposition census digest mismatch.");
        var relevant = census.Where(r => r["stage6_relevance"] == "STAGE6_RELEVANT").ToDictionary(r => r["issue_number"], StringComparer.Ordinal);
        var disp = dispositions.ToDictionary(r => r["issue_number"], StringComparer.Ordinal);
        Require(relevant.Keys.OrderBy(x => x, StringComparer.Ordinal).SequenceEqual(disp.Keys.OrderBy(x => x, StringComparer.Ordinal), StringComparer.Ordinal), "Stage-6-relevant census/disposition set mismatch.");
        foreach (var pair in relevant)
        {
            var d = disp[pair.Key];
            var c = pair.Value;
            Require(c["target_foundation_stage_wp"].Contains("Stage 6", StringComparison.Ordinal), $"Stage-6-relevant issue lacks Stage 6 target attribution: issue {pair.Key}");
            foreach (var field in new[] { "status", "waiting_on", "target_foundation_stage_wp" })
                Require(StringComparer.Ordinal.Equals(c[field], d[field]), $"FCR copied field mismatch for issue {pair.Key}: {field}");
            Require(d["stage6_closure_blocking_disposition"] is "BLOCKING_FOUNDATION_ACTION_REQUIRED" or "BLOCKING_OWNER_DECISION_REQUIRED" or "NON_BLOCKING_APPLICATION_OWNED_FUTURE_TRIGGER" or "NON_BLOCKING_ACCEPTED_CLOSURE_PRESERVED" or "NON_BLOCKING_FUTURE_STAGE_OR_SEPARATE_GATE", "Invalid FCR blocking disposition.");
            Require(d["stage6_closure_blocking_disposition"] is not "BLOCKING_FOUNDATION_ACTION_REQUIRED" and not "BLOCKING_OWNER_DECISION_REQUIRED", $"Unresolved Stage 6 blocker: issue {pair.Key}");
            if (d["stage6_closure_blocking_disposition"] == "NON_BLOCKING_APPLICATION_OWNED_FUTURE_TRIGGER")
            {
                Require(c["waiting_on"] == "APPLICATION", $"Application-owned future trigger must hand off to APPLICATION: issue {pair.Key}");
                Require(c["status"] is "FOUNDATION_IMPLEMENTED" or "APPLICATION_VERIFIED", $"Application-owned future trigger requires a canonical post-Foundation lifecycle state: issue {pair.Key}");
                Require(!c["status"].Contains("CLOSED", StringComparison.OrdinalIgnoreCase), $"Application-owned future trigger must not encode closure: issue {pair.Key}");
                Require(d["disposition_basis"].Contains("FCR remains OPEN", StringComparison.Ordinal), $"Disposition must explicitly preserve open FCR truth: issue {pair.Key}");
                Require(d["disposition_basis"].Contains("Application workstream remains OPEN", StringComparison.Ordinal), $"Disposition must explicitly preserve open Application truth: issue {pair.Key}");
                Require(d["disposition_basis"].Contains("final Application", StringComparison.Ordinal), $"Disposition must preserve pending final Application verification: issue {pair.Key}");
            }
        }
    }

    private static List<Dictionary<string, string>> ValidManifestRows()
    {
        var rows = new List<Dictionary<string, string>>();
        for (var i = 1; i <= 9; i++)
        {
            var wp = $"WP-{i:00}";
            rows.Add(new Dictionary<string, string>(StringComparer.Ordinal)
            {
                ["manifest_version"] = "1", ["stage_id"] = "STAGE6", ["work_package"] = wp,
                ["accepted_scope_label"] = ExpectedScopeByWp[wp], ["closure_evidence_kind"] = "CANONICAL_CLOSURE_RECORD",
                ["closure_evidence_locator"] = $"docs/canonical-records/{wp}.txt", ["closure_evidence_sha256"] = new string('C', 64),
                ["closure_decision_commit_sha"] = new string('a', 40), ["accepted_technical_baseline_sha"] = new string('b', 40),
                ["executable_evidence_sha256"] = "NOT_APPLICABLE_BY_HISTORICAL_GATE", ["final_red_team_disposition"] = "NOT_APPLICABLE_BY_HISTORICAL_GATE",
                ["application_compatibility_disposition"] = "NOT_APPLICABLE_BY_HISTORICAL_GATE", ["historical_gate_note"] = "historical"
            });
        }
        return rows;
    }

    private static void NegativeMissingWp() { var r = ValidManifestRows(); r.RemoveAt(3); Throws(() => ValidateManifestRows(r)); }
    private static void NegativeDuplicateWp() { var r = ValidManifestRows(); r[4]["work_package"] = "WP-04"; Throws(() => ValidateManifestRows(r)); }
    private static void NegativeFutureWp() { var r = ValidManifestRows(); r[8]["work_package"] = "WP-10"; Throws(() => ValidateManifestRows(r)); }
    private static void NegativeWrongOrder() { var r = ValidManifestRows(); (r[0], r[1]) = (r[1], r[0]); Throws(() => ValidateManifestRows(r)); }
    private static void NegativeWrongStage() { var r = ValidManifestRows(); r[0]["stage_id"] = "STAGE7"; Throws(() => ValidateManifestRows(r)); }
    private static void NegativeWrongManifestVersion() { var r = ValidManifestRows(); r[0]["manifest_version"] = "2"; Throws(() => ValidateManifestRows(r)); }
    private static void NegativeBlankRequiredField() { var r = ValidManifestRows(); r[0]["historical_gate_note"] = ""; Throws(() => ValidateManifestRows(r)); }
    private static void NegativeInvalidEnums() { var r = ValidManifestRows(); r[0]["application_compatibility_disposition"] = "CLOSED"; Throws(() => ValidateManifestRows(r)); }
    private static void NegativeFalseHistoricalClassification() { var r = ValidManifestRows(); r[0]["closure_evidence_kind"] = "HISTORICAL_ACCEPTED_CLOSURE_REFERENCE"; Throws(() => ValidateManifestRows(r)); }
    private static void NegativeWrongFunctionalScope() { var r = ValidManifestRows(); r[0]["accepted_scope_label"] = ExpectedScopeByWp["WP-02"]; Throws(() => ValidateManifestRows(r)); }
    private static void NegativeMalformedSha() { var r = ValidManifestRows(); r[0]["accepted_technical_baseline_sha"] = "bad"; Throws(() => ValidateManifestRows(r)); }

    private static void NegativeClosureSubstitution(string root, string manifestPath)
    {
        var rows = Tsv.Read(manifestPath, ManifestColumns).Rows.Select(Clone).ToList();
        foreach (var field in new[] { "closure_evidence_locator", "closure_evidence_sha256", "closure_decision_commit_sha", "accepted_technical_baseline_sha", "executable_evidence_sha256" })
            (rows[0][field], rows[1][field]) = (rows[1][field], rows[0][field]);
        Throws(() => ValidateCanonicalEvidenceForRow(root, rows[0]));
    }

    private static void NegativeCanonicalDigestMismatch(string root, string manifestPath)
    {
        var row = Clone(Tsv.Read(manifestPath, ManifestColumns).Rows[0]); row["closure_evidence_sha256"] = new string('0', 64);
        Throws(() => ValidateCanonicalEvidenceForRow(root, row));
    }

    private static List<Dictionary<string, string>> CensusRows() => new()
    {
        new(StringComparer.Ordinal) { ["census_version"]="2", ["captured_at_utc"]="2026-08-10T20:24:27Z", ["issue_number"]="10", ["issue_title"]="[FCR-0010] resource", ["status"]="FOUNDATION_IMPLEMENTED", ["waiting_on"]="APPLICATION", ["target_foundation_stage_wp"]="Stage 6", ["stage6_relevance"]="STAGE6_RELEVANT", ["issue_updated_at"]="2026-08-10T20:23:57Z" },
        new(StringComparer.Ordinal) { ["census_version"]="2", ["captured_at_utc"]="2026-08-10T20:24:27Z", ["issue_number"]="30", ["issue_title"]="[FCR-0030] future", ["status"]="NEEDS_FOUNDATION_RECONCILIATION / LINKED_TO_EXPANDED_FCR-0012", ["waiting_on"]="FOUNDATION", ["target_foundation_stage_wp"]="Stage 13", ["stage6_relevance"]="NOT_STAGE6_RELEVANT", ["issue_updated_at"]="2026-08-10T19:54:18Z" }
    };

    private static Dictionary<string, string> Disp(string issue = "10", string status = "FOUNDATION_IMPLEMENTED", string waiting = "APPLICATION", string target = "Stage 6", string disposition = "NON_BLOCKING_APPLICATION_OWNED_FUTURE_TRIGGER", string basis = "FCR remains OPEN and Application workstream remains OPEN; final Application implementation verification remains pending.")
        => new(StringComparer.Ordinal) { ["snapshot_version"]="2", ["census_sha256"]=new string('A',64), ["issue_number"]=issue, ["status"]=status, ["waiting_on"]=waiting, ["target_foundation_stage_wp"]=target, ["stage6_closure_blocking_disposition"]=disposition, ["disposition_basis"]=basis };

    private static void NegativeMissingStage6Disposition() => Throws(() => ValidateFcrRows(CensusRows(), Array.Empty<Dictionary<string,string>>(), new string('A',64)));
    private static void NegativeExtraDisposition() => Throws(() => ValidateFcrRows(CensusRows(), new[] { Disp(), Disp("30", "NEEDS_FOUNDATION_RECONCILIATION / LINKED_TO_EXPANDED_FCR-0012", "FOUNDATION", "Stage 13", "NON_BLOCKING_FUTURE_STAGE_OR_SEPARATE_GATE", "future stage") }, new string('A',64)));
    private static void NegativeDispositionMismatch() => Throws(() => ValidateFcrRows(CensusRows(), new[] { Disp(status:"APPLICATION_VERIFIED") }, new string('A',64)));
    private static void NegativeFoundationBlocker() { var c = CensusRows(); c[0]["waiting_on"] = "FOUNDATION"; Throws(() => ValidateFcrRows(c, new[] { Disp(waiting:"FOUNDATION", disposition:"BLOCKING_FOUNDATION_ACTION_REQUIRED", basis:"blocker") }, new string('A',64))); }
    private static void NegativeOwnerBlocker() { var c = CensusRows(); c[0]["waiting_on"] = "OWNER"; Throws(() => ValidateFcrRows(c, new[] { Disp(waiting:"OWNER", disposition:"BLOCKING_OWNER_DECISION_REQUIRED", basis:"blocker") }, new string('A',64))); }
    private static void NegativeInvalidWaitingOn() { var c = CensusRows(); c[0]["waiting_on"] = "APPLICATION_PENDING"; Throws(() => ValidateFcrRows(c, new[] { Disp(waiting:"APPLICATION_PENDING") }, new string('A',64))); }
    private static void NegativeInvalidStage6Relevance() { var c = CensusRows(); c[0]["stage6_relevance"] = "MAYBE"; Throws(() => ValidateFcrRows(c, new[] { Disp() }, new string('A',64))); }
    private static void NegativeDuplicateIssue() { var c = CensusRows(); c[1]["issue_number"] = "10"; Throws(() => ValidateFcrRows(c, new[] { Disp() }, new string('A',64))); }
    private static void NegativeInvalidCaptureChronology() { var c = CensusRows(); c[0]["issue_updated_at"] = "2026-08-10T20:30:00Z"; Throws(() => ValidateFcrRows(c, new[] { Disp() }, new string('A',64))); }

    private static void ApplicationAckDoesNotCloseApplicationOrFcr()
    {
        ValidateFcrRows(CensusRows(), new[] { Disp() }, new string('A',64));
        var closedStatus = CensusRows(); closedStatus[0]["status"] = "CLOSED"; Throws(() => ValidateFcrRows(closedStatus, new[] { Disp(status:"CLOSED") }, new string('A',64)));
        var noApplicationHandoff = CensusRows(); noApplicationHandoff[0]["waiting_on"] = "NONE"; Throws(() => ValidateFcrRows(noApplicationHandoff, new[] { Disp(waiting:"NONE") }, new string('A',64)));
        var missingOpenBasis = new[] { Disp(basis:"Compatibility ACK received; final Application implementation verification remains pending.") }; Throws(() => ValidateFcrRows(CensusRows(), missingOpenBasis, new string('A',64)));
    }

    private static void DeterministicManifestIdentity()
    {
        var text = string.Join("\n", ValidManifestRows().Select(r => string.Join("\t", ManifestColumns.Select(c => r[c]))));
        var a = Sha256Text(text); var b = Sha256Text(text);
        Require(StringComparer.Ordinal.Equals(a, b), "Manifest identity not deterministic.");
        Require(!StringComparer.Ordinal.Equals(a, Sha256Text(text + "x")), "Manifest mutation did not change identity.");
    }

    private static void DeterministicIntegratedClosureIdentity(string manifestPath, string censusPath, string dispositionPath)
    {
        var manifestDigest = Sha256File(manifestPath);
        var censusDigest = Sha256File(censusPath);
        var dispositionDigest = Sha256File(dispositionPath);
        var a = IntegratedIdentity(manifestDigest, censusDigest, dispositionDigest);
        var b = IntegratedIdentity(manifestDigest, censusDigest, dispositionDigest);
        Require(StringComparer.Ordinal.Equals(a, b), "Integrated closure identity not deterministic.");
        var mutatedManifestDigest = (manifestDigest[0] == '0' ? "1" : "0") + manifestDigest[1..];
        Require(!StringComparer.Ordinal.Equals(a, IntegratedIdentity(mutatedManifestDigest, censusDigest, dispositionDigest)), "Integrated closure identity is not mutation-sensitive.");
    }

    private static string IntegratedIdentity(string manifestDigest, string censusDigest, string dispositionDigest)
        => Sha256Text($"STAGE6-WP10-INTEGRATED-CLOSURE\nMANIFEST={manifestDigest}\nCENSUS={censusDigest}\nDISPOSITION={dispositionDigest}");

    private static void NoStage7OrAuthorityClaim()
    {
        var publicSurface = typeof(Program).Assembly.GetTypes().Where(t => t.IsPublic).Select(t => t.FullName ?? string.Empty).ToArray();
        Require(!publicSurface.Any(x => x.Contains("Stage7", StringComparison.OrdinalIgnoreCase)), "WP-10 verifier exposes Stage 7 surface.");
        Require(!publicSurface.Any(x => x.Contains("GrantAuthority", StringComparison.OrdinalIgnoreCase)), "WP-10 verifier exposes authority-granting surface.");
    }

    private static string FindRepositoryRoot()
    {
        var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (dir is not null) { if (File.Exists(Path.Combine(dir.FullName, "Falcon.Foundation.ControlledProjectFoundation.slnx"))) return dir.FullName; dir = dir.Parent; }
        throw new InvalidOperationException("Repository root not found.");
    }

    private static bool GitObjectExists(string root, string spec)
    {
        var psi = new ProcessStartInfo("git") { WorkingDirectory = root, RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false };
        psi.ArgumentList.Add("cat-file"); psi.ArgumentList.Add("-e"); psi.ArgumentList.Add(spec);
        using var p = Process.Start(psi) ?? throw new InvalidOperationException("Unable to start git."); p.WaitForExit(); return p.ExitCode == 0;
    }

    private static Dictionary<string, string> Clone(Dictionary<string, string> source) => new(source, StringComparer.Ordinal);
    private static bool TryParseUtc(string value, out DateTimeOffset parsed) { if (!DateTimeOffset.TryParse(value, out parsed)) return false; return parsed.Offset == TimeSpan.Zero && value.EndsWith("Z", StringComparison.Ordinal); }
    private static bool IsGitSha(string value) => value.Length == 40 && value.All(Uri.IsHexDigit);
    private static bool IsSha256(string value) => value.Length == 64 && value.All(Uri.IsHexDigit);
    private static bool IsSafeRelativePath(string value) => !Path.IsPathRooted(value) && !value.Split('/', '\\').Any(x => x == "..");
    private static string Sha256File(string path) => Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path)));
    private static string Sha256Text(string text) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(text)));

    private static void Run(string name, Action action) { try { action(); _passed++; Console.WriteLine($"PASS {name}"); } catch (Exception ex) { _failed++; Console.WriteLine($"FAIL {name}: {ex.Message}"); } }
    private static void Require(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }
    private static void Throws(Action action) { try { action(); } catch { return; } throw new InvalidOperationException("Expected rejection did not occur."); }

    private sealed record Tsv(IReadOnlyList<Dictionary<string, string>> Rows)
    {
        public static Tsv Read(string path, IReadOnlyList<string> expectedColumns)
        {
            var lines = File.ReadAllLines(path, Encoding.UTF8).Where(x => x.Length > 0).ToArray();
            Require(lines.Length >= 2, $"TSV has no data rows: {path}");
            var header = lines[0].Split('\t'); Require(expectedColumns.SequenceEqual(header, StringComparer.Ordinal), $"TSV header mismatch: {path}");
            var rows = new List<Dictionary<string, string>>();
            foreach (var line in lines.Skip(1))
            {
                var fields = line.Split('\t'); Require(fields.Length == header.Length, $"TSV field count mismatch: {path}");
                var row = new Dictionary<string, string>(StringComparer.Ordinal); for (var i = 0; i < header.Length; i++) row[header[i]] = fields[i]; rows.Add(row);
            }
            return new Tsv(rows);
        }
    }
}
