using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Falcon.Stage6.WP10.Verifier;

internal static class ProgramV3
{
    private static readonly string[] RequiredColumns =
    {
        "manifest_version", "stage_id", "work_package", "accepted_scope_label",
        "closure_evidence_kind", "closure_evidence_locator", "closure_evidence_sha256",
        "closure_decision_commit_sha", "accepted_technical_baseline_sha", "executable_evidence_sha256",
        "final_red_team_disposition", "application_compatibility_disposition", "historical_gate_note"
    };

    private static int Main()
    {
        try
        {
            var root = FindRepositoryRoot();
            var manifestPath = Path.Combine(root, "docs", "stage-6-wp10", "STAGE6_CLOSURE_MANIFEST.tsv");
            ValidateImmutableHistoryBindings(root, manifestPath);
            Console.WriteLine("PASS immutable_history_binding_preflight");

            var main = typeof(ProgramV2).GetMethod("Main", BindingFlags.NonPublic | BindingFlags.Static);
            if (main is null)
                throw new InvalidOperationException("ProgramV2.Main entry point not found.");

            var result = main.Invoke(null, null);
            return result is int exitCode ? exitCode : 1;
        }
        catch (TargetInvocationException ex) when (ex.InnerException is not null)
        {
            Console.Error.WriteLine("FAIL immutable_history_binding_preflight_or_v2: " + ex.InnerException.Message);
            return 1;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("FAIL immutable_history_binding_preflight: " + ex.Message);
            return 1;
        }
    }

    private static void ValidateImmutableHistoryBindings(string root, string manifestPath)
    {
        if (!File.Exists(manifestPath))
            throw new InvalidOperationException("STAGE6_CLOSURE_MANIFEST.tsv missing.");

        var lines = File.ReadAllLines(manifestPath);
        if (lines.Length != 10)
            throw new InvalidOperationException("Closure manifest must contain one header plus exactly nine predecessor rows.");

        var header = lines[0].Split('\t');
        if (!RequiredColumns.SequenceEqual(header, StringComparer.Ordinal))
            throw new InvalidOperationException("Closure manifest schema mismatch.");

        var indexes = RequiredColumns
            .Select((name, index) => new { name, index })
            .ToDictionary(x => x.name, x => x.index, StringComparer.Ordinal);

        for (var rowIndex = 1; rowIndex < lines.Length; rowIndex++)
        {
            var values = lines[rowIndex].Split('\t');
            if (values.Length != RequiredColumns.Length)
                throw new InvalidOperationException($"Manifest row {rowIndex} column count mismatch.");

            var wp = values[indexes["work_package"]];
            var path = values[indexes["closure_evidence_locator"]];
            var decision = values[indexes["closure_decision_commit_sha"]];
            var baseline = values[indexes["accepted_technical_baseline_sha"]];

            RequireSafeRelativePath(path, wp);
            RequireCommit(root, decision, $"closure decision commit for {wp}");
            RequireCommit(root, baseline, $"accepted technical baseline for {wp}");
            RequireAncestor(root, decision, "HEAD", $"closure decision commit for {wp}");
            RequireAncestor(root, baseline, "HEAD", $"accepted technical baseline for {wp}");
            RequirePathAddedAtCommit(root, decision, path, wp);

            var headBlob = GitText(root, "rev-parse", "HEAD:" + path);
            var closureBlob = GitText(root, "rev-parse", decision + ":" + path);

            if (!StringComparer.Ordinal.Equals(headBlob, closureBlob))
                throw new InvalidOperationException($"Canonical closure bytes changed after Owner closure for {wp}. HEAD blob {headBlob}; closure blob {closureBlob}.");
        }
    }

    private static void RequireCommit(string root, string sha, string label)
    {
        var process = RunGit(root, "cat-file", "-e", sha + "^{commit}");
        if (process.ExitCode != 0)
            throw new InvalidOperationException($"Missing {label}: {sha}");
    }

    private static void RequireAncestor(string root, string ancestor, string descendant, string label)
    {
        var process = RunGit(root, "merge-base", "--is-ancestor", ancestor, descendant);
        if (process.ExitCode != 0)
            throw new InvalidOperationException($"{label} is not an ancestor of validation HEAD: {ancestor}");
    }

    private static void RequirePathAddedAtCommit(string root, string commit, string path, string wp)
    {
        var output = GitText(root, "diff-tree", "--root", "--no-commit-id", "--name-status", "--diff-filter=A", "-r", commit, "--", path);
        var expected = "A\t" + path;
        var rows = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        if (rows.Length != 1 || !StringComparer.Ordinal.Equals(rows[0], expected))
            throw new InvalidOperationException($"Recorded closure decision commit is not the exact commit that added the canonical closure record for {wp}: {commit}");
    }

    private static string GitText(string root, params string[] args)
    {
        var process = RunGit(root, args);
        if (process.ExitCode != 0)
            throw new InvalidOperationException("Git command failed: git " + string.Join(' ', args) + "\n" + process.StandardError.ReadToEnd());
        return process.StandardOutput.ReadToEnd().Trim();
    }

    private static Process RunGit(string root, params string[] args)
    {
        var psi = new ProcessStartInfo("git")
        {
            WorkingDirectory = root,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        foreach (var arg in args)
            psi.ArgumentList.Add(arg);
        var process = Process.Start(psi) ?? throw new InvalidOperationException("Unable to start git.");
        process.WaitForExit();
        return process;
    }

    private static void RequireSafeRelativePath(string value, string wp)
    {
        if (string.IsNullOrWhiteSpace(value) || Path.IsPathRooted(value) || value.Contains("..", StringComparison.Ordinal) || value.Contains('\\'))
            throw new InvalidOperationException($"Unsafe closure evidence path for {wp}: {value}");
    }

    private static string FindRepositoryRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current is not null)
        {
            if (Directory.Exists(Path.Combine(current.FullName, ".git")) || File.Exists(Path.Combine(current.FullName, ".git")))
                return current.FullName;
            current = current.Parent;
        }

        current = new DirectoryInfo(Environment.CurrentDirectory);
        while (current is not null)
        {
            if (Directory.Exists(Path.Combine(current.FullName, ".git")) || File.Exists(Path.Combine(current.FullName, ".git")))
                return current.FullName;
            current = current.Parent;
        }

        throw new InvalidOperationException("Repository root not found.");
    }
}
