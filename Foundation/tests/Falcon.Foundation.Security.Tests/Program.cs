using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Falcon.Foundation.Security.Tests;

internal static partial class Program
{
    private static readonly string RepositoryRoot = ResolveRepositoryRoot();
    private static readonly string ServiceCatalogSourcePath = Path.Combine(RepositoryRoot, "src", "Foundation.ServiceCatalog", "ServiceCatalog.cs");
    private static readonly string DependencyGovernanceSourceRoot = Path.Combine(RepositoryRoot, "src", "Foundation.DependencyGovernance");
    private static readonly string LifecycleControlSourcePath = Path.Combine(RepositoryRoot, "src", "Foundation.Infrastructure", "BootstrapLifecycleControl.cs");
    private static readonly string StateSourceRoot = Path.Combine(RepositoryRoot, "src", "Foundation.State");
    private static readonly string EvidenceSourceRoot = Path.Combine(RepositoryRoot, "src", "Foundation.Evidence");
    private static readonly string ScannerSelfPath = Path.Combine(RepositoryRoot, "tests", "Falcon.Foundation.Security.Tests", "Program.cs");

    private static readonly string[] GovernedRoots =
    [
        "src",
        "tests",
        "verification"
    ];

    private static readonly string[] RequiredRootConfigurations =
    [
        ".editorconfig",
        ".gitattributes",
        ".gitignore",
        "Directory.Build.props",
        "global.json",
        "NuGet.Config",
        "Falcon.Foundation.ControlledProjectFoundation.slnx"
    ];

    private static readonly HashSet<string> CandidateExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".cs",
        ".csproj",
        ".props",
        ".targets",
        ".json",
        ".xml",
        ".config",
        ".slnx"
    };

    private const string AllowedNuGetEndpoint = "https://api.nuget.org/v3/index.json";

    private static readonly Regex EndpointPattern = new(
        @"https?://[^\s""'<>`]+",
        RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly Regex SecretKeywordPattern = new(
        @"\b(api[_-]?key|client[_-]?secret|private[_-]?key|secret|token|password|passwd|connection\s*string)\b",
        RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly Regex SecretAssignmentPattern = new(
        "(?i)\\b(password|passwd|secret|token|client[_-]?secret|api[_-]?key|private[_-]?key|connection\\s*string)\\b\\s*[:=]\\s*(?:['\"][^'\"\\r\\n]{12,}['\"]|[A-Za-z0-9+/=._-]{20,})",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static int Main()
    {
        var findings = new List<string>();
        var scannedFiles = CollectGovernedFiles(findings);

        VerifyUnreadableFileFailsClosed(findings);

        foreach (var file in scannedFiles)
        {
            if (StringComparer.OrdinalIgnoreCase.Equals(file, ScannerSelfPath))
            {
                continue;
            }

            var text = ReadTextOrFinding(file, findings, File.ReadAllText);
            if (text is null)
            {
                continue;
            }

            foreach (Match match in EndpointPattern.Matches(text))
            {
                if (!IsAllowedEndpoint(file, match.Value))
                {
                    findings.Add($"External endpoint reference found in {RelativePath(file)}: {match.Value}");
                }
            }

            if (SecretKeywordPattern.IsMatch(text) && SecretAssignmentPattern.IsMatch(text))
            {
                findings.Add($"Secret-like assignment found in {RelativePath(file)}");
            }

            if (ContainsLikelyKeyMaterial(text))
            {
                findings.Add($"Potential credential or key material found in {RelativePath(file)}");
            }
        }

        ValidateCoverage(scannedFiles, findings);
        ValidateServiceCatalogSourceSurface(findings);
        ValidateDependencyGovernanceSourceSurface(findings);
        ValidateMandatoryLifecycleAuthorityBoundary(findings);
        ValidateAuthoritativeStateBoundary(findings);
        ValidateEvidenceJournalBoundary(findings);

        var sourceCount = CountUnder(scannedFiles, "src");
        var testCount = CountUnder(scannedFiles, "tests", excludeScannerSelf: true);
        var verificationCount = CountUnder(scannedFiles, "verification");
        var rootConfigurationCount = RequiredRootConfigurations.Count(name =>
            scannedFiles.Contains(Path.Combine(RepositoryRoot, name), StringComparer.OrdinalIgnoreCase));

        var uniqueFindings = findings
            .Distinct(StringComparer.Ordinal)
            .OrderBy(value => value, StringComparer.Ordinal)
            .ToArray();

        Console.WriteLine("Baseline integrity security gate: " + (uniqueFindings.Length == 0 ? "PASS" : "FAIL"));
        Console.WriteLine($"Repository root: {RepositoryRoot}");
        Console.WriteLine($"Scanned files: {scannedFiles.Count}");
        Console.WriteLine($"Source files scanned: {sourceCount}");
        Console.WriteLine($"Test files scanned: {testCount}");
        Console.WriteLine($"Verification files scanned: {verificationCount}");
        Console.WriteLine($"Root configurations scanned: {rootConfigurationCount}");
        Console.WriteLine($"Security findings: {uniqueFindings.Length}");

        if (uniqueFindings.Length == 0)
        {
            return 0;
        }

        Console.Error.WriteLine("Findings:");
        foreach (var finding in uniqueFindings)
        {
            Console.Error.WriteLine($"- {finding}");
        }

        return 1;
    }

    private static void ValidateEvidenceJournalBoundary(
        ICollection<string> findings)
    {
        if (!Directory.Exists(EvidenceSourceRoot))
        {
            findings.Add("Mandatory WP-04 evidence boundary is missing.");
            return;
        }

        var source = string.Join(
            Environment.NewLine,
            Directory.EnumerateFiles(
                    EvidenceSourceRoot,
                    "*.cs",
                    SearchOption.AllDirectories)
                .OrderBy(path => path, StringComparer.Ordinal)
                .Select(File.ReadAllText));

        foreach (var required in new[]
        {
            "IntegrityLinkedEvidenceRecord",
            "EvidenceAppendRequest",
            "PreviousRecordDigest",
            "RecordDigest",
            "AcceptedFactEvent",
            "DURABLE_COMMIT_NOT_ACCEPTED",
            "JOURNAL_TRUNCATED",
            "JOURNAL_LINK_CONFLICT",
            "CORRECTION_TARGET_NOT_FOUND",
            "EVIDENCE_ID_CANONICAL_MISMATCH",
            "ACCEPTED_FACT_EVIDENCE_NOT_FOUND",
            "internal AcceptedFactPublishResult AppendAcceptedFact"
        })
        {
            if (!source.Contains(required, StringComparison.Ordinal))
            {
                findings.Add(
                    "Mandatory WP-04 evidence boundary is missing: " +
                    required);
            }
        }

        foreach (var forbidden in new[]
        {
            "HttpClient",
            "Socket",
            "AcceptedFactBeforeCommit",
            "Thread.Sleep",
            "Task.Delay",
            "public AcceptedFactPublishResult AppendAcceptedFact"
        })
        {
            if (source.Contains(forbidden, StringComparison.Ordinal))
            {
                findings.Add(
                    "Forbidden WP-04 evidence surface found: " +
                    forbidden);
            }
        }
    }

    private static string ResolveRepositoryRoot()
    {
        var current = new DirectoryInfo(Path.GetFullPath(AppContext.BaseDirectory));
        while (current is not null)
        {
            var candidate = current.FullName;
            if (File.Exists(Path.Combine(candidate, "Falcon.Foundation.ControlledProjectFoundation.slnx")) &&
                File.Exists(Path.Combine(candidate, "Directory.Build.props")) &&
                Directory.Exists(Path.Combine(candidate, "src")) &&
                Directory.Exists(Path.Combine(candidate, "tests")) &&
                Directory.Exists(Path.Combine(candidate, "verification")))
            {
                return candidate;
            }

            current = current.Parent;
        }

        throw new InvalidOperationException("repository_root_not_resolved_from_app_context");
    }

    private static IReadOnlyList<string> CollectGovernedFiles(ICollection<string> findings)
    {
        var files = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var relativeRoot in GovernedRoots)
        {
            var root = Path.Combine(RepositoryRoot, relativeRoot);
            if (!Directory.Exists(root))
            {
                findings.Add($"Missing governed scan root: {relativeRoot}");
                continue;
            }

            CollectDirectory(root, files, findings);
        }

        foreach (var relativePath in RequiredRootConfigurations)
        {
            var path = Path.Combine(RepositoryRoot, relativePath);
            if (!File.Exists(path))
            {
                findings.Add($"Missing root configuration: {relativePath}");
                continue;
            }

            files.Add(path);
        }

        return files.ToArray();
    }

    private static void CollectDirectory(
        string directory,
        ISet<string> files,
        ICollection<string> findings)
    {
        IEnumerable<string> childFiles;
        try
        {
            childFiles = Directory.EnumerateFiles(directory, "*", SearchOption.TopDirectoryOnly).ToArray();
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            findings.Add($"Unreadable governed directory: {RelativePath(directory)} ({exception.GetType().Name})");
            return;
        }

        foreach (var file in childFiles)
        {
            if (IsCandidate(file))
            {
                files.Add(Path.GetFullPath(file));
            }
        }

        IEnumerable<string> childDirectories;
        try
        {
            childDirectories = Directory.EnumerateDirectories(directory, "*", SearchOption.TopDirectoryOnly).ToArray();
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            findings.Add($"Unreadable governed directory: {RelativePath(directory)} ({exception.GetType().Name})");
            return;
        }

        foreach (var child in childDirectories.OrderBy(value => value, StringComparer.Ordinal))
        {
            var name = Path.GetFileName(child);
            if (name.Equals("bin", StringComparison.OrdinalIgnoreCase) ||
                name.Equals("obj", StringComparison.OrdinalIgnoreCase) ||
                new DirectoryInfo(child).Attributes.HasFlag(FileAttributes.ReparsePoint))
            {
                continue;
            }

            CollectDirectory(child, files, findings);
        }
    }

    private static bool IsCandidate(string file) =>
        CandidateExtensions.Contains(Path.GetExtension(file));

    private static string? ReadTextOrFinding(
        string file,
        ICollection<string> findings,
        Func<string, string> reader)
    {
        try
        {
            return reader(file);
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException)
        {
            findings.Add($"Unreadable governed file: {RelativePath(file)} ({exception.GetType().Name})");
            return null;
        }
    }

    private static void VerifyUnreadableFileFailsClosed(ICollection<string> findings)
    {
        var synthetic = new List<string>();
        var result = ReadTextOrFinding(
            Path.Combine(RepositoryRoot, "tests", "synthetic-unreadable.cs"),
            synthetic,
            _ => throw new UnauthorizedAccessException("synthetic"));

        if (result is not null ||
            synthetic.Count != 1 ||
            !synthetic[0].StartsWith("Unreadable governed file:", StringComparison.Ordinal))
        {
            findings.Add("Unreadable-file fail-closed self-test failed.");
        }
    }

    private static void ValidateCoverage(
        IReadOnlyCollection<string> scannedFiles,
        ICollection<string> findings)
    {
        var sourceCount = CountUnder(scannedFiles, "src");
        var testCount = CountUnder(scannedFiles, "tests", excludeScannerSelf: true);
        var verificationCount = CountUnder(scannedFiles, "verification");
        var rootCount = RequiredRootConfigurations.Count(name =>
            scannedFiles.Contains(Path.Combine(RepositoryRoot, name), StringComparer.OrdinalIgnoreCase));

        if (sourceCount < 20)
        {
            findings.Add($"Insufficient source scan coverage: {sourceCount}.");
        }

        if (testCount < 3)
        {
            findings.Add($"Insufficient test scan coverage: {testCount}.");
        }

        if (verificationCount < 20)
        {
            findings.Add($"Insufficient verification scan coverage: {verificationCount}.");
        }

        if (rootCount != RequiredRootConfigurations.Length)
        {
            findings.Add($"Incomplete root-configuration coverage: {rootCount}/{RequiredRootConfigurations.Length}.");
        }

        if (scannedFiles.Count == 0)
        {
            findings.Add("No governed files were scanned.");
        }
    }

    private static int CountUnder(
        IEnumerable<string> files,
        string folderName,
        bool excludeScannerSelf = false)
    {
        var root = Path.Combine(RepositoryRoot, folderName) + Path.DirectorySeparatorChar;
        return files.Count(file =>
            file.StartsWith(root, StringComparison.OrdinalIgnoreCase) &&
            (!excludeScannerSelf || !StringComparer.OrdinalIgnoreCase.Equals(file, ScannerSelfPath)));
    }

    private static bool IsAllowedEndpoint(string file, string endpoint) =>
        StringComparer.OrdinalIgnoreCase.Equals(file, Path.Combine(RepositoryRoot, "NuGet.Config")) &&
        StringComparer.Ordinal.Equals(endpoint.TrimEnd('.', ',', ';'), AllowedNuGetEndpoint);

    private static bool ContainsLikelyKeyMaterial(string text)
    {
        if (text.Contains("BEGIN PRIVATE KEY", StringComparison.OrdinalIgnoreCase) ||
            text.Contains("BEGIN RSA PRIVATE KEY", StringComparison.OrdinalIgnoreCase) ||
            text.Contains("BEGIN OPENSSH PRIVATE KEY", StringComparison.OrdinalIgnoreCase) ||
            text.Contains("BEGIN CERTIFICATE", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return text
            .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
            .Any(line => SecretAssignmentPattern.IsMatch(line));
    }

    private static string RelativePath(string path)
    {
        var fullPath = Path.GetFullPath(path);
        return Path.GetRelativePath(RepositoryRoot, fullPath).Replace('\\', '/');
    }

    private static void ValidateServiceCatalogSourceSurface(ICollection<string> findings)
    {
        if (!File.Exists(ServiceCatalogSourcePath))
        {
            findings.Add($"Missing service catalog source surface: {RelativePath(ServiceCatalogSourcePath)}");
            return;
        }

        var text = ReadTextOrFinding(ServiceCatalogSourcePath, findings, File.ReadAllText);
        if (text is null)
        {
            return;
        }

        string[] forbiddenPhrases =
        [
            "Assembly.GetExecutingAssembly",
            "Assembly.Load",
            "AppDomain.CurrentDomain.GetAssemblies",
            "Type.GetType(",
            "Activator.CreateInstance",
            "Directory.EnumerateFiles",
            "Directory.GetFiles",
            "File.ReadAllText(",
            "File.Open(",
            "HttpClient",
            "Socket",
            "Dns.",
            "dynamic ",
            "identifier-provider",
            "cryptographic-provider",
            "certificate-identity-provider",
            "randomness-provider",
            "object ExactProviderContractRecord",
            "abstract object ExactProviderContractRecord",
            "ValidateProviderContractRecord(",
            "object record",
            "RuntimeInformation",
            "Environment.GetFolderPath"
        ];

        foreach (var phrase in forbiddenPhrases)
        {
            if (text.Contains(phrase, StringComparison.Ordinal))
            {
                findings.Add($"Prohibited ServiceCatalog source surface reference found: {phrase}");
            }
        }

        string[] requiredText =
        [
            "RegistrationEvidenceReference",
            "ProviderContractIdentity",
            "ProviderContractVersion",
            "governed service registration"
        ];

        foreach (var phrase in requiredText)
        {
            if (!text.Contains(phrase, StringComparison.Ordinal))
            {
                findings.Add($"Expected ServiceCatalog hardening text missing: {phrase}");
            }
        }
    }

    private static void ValidateDependencyGovernanceSourceSurface(ICollection<string> findings)
    {
        if (!Directory.Exists(DependencyGovernanceSourceRoot))
        {
            findings.Add($"Missing dependency-governance source root: {RelativePath(DependencyGovernanceSourceRoot)}");
            return;
        }

        string[] prohibitedPhrases =
        [
            "Assembly.GetExecutingAssembly",
            "Assembly.Load",
            "AppDomain.CurrentDomain.GetAssemblies",
            "Type.GetType(",
            "Activator.CreateInstance",
            "Directory.EnumerateFiles",
            "Directory.GetFiles",
            "File.ReadAllText(",
            "File.Open(",
            "HttpClient",
            "Socket",
            "Dns.",
            "Process.Start",
            "dynamic ",
            "IServiceProvider",
            "GetRequiredService",
            "ActivatorUtilities",
            "Environment.GetFolderPath",
            "provider-selection",
            "provider management",
            "ProviderManager",
            "SelectProvider",
            "ManageProvider",
            "Service Bus",
            "Event Bus",
            "bootstrap execution",
            "lifecycle mutation",
            "activation execution",
            "runtime transport",
            "network transport"
        ];

        var dependencyFiles = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
        CollectDirectory(DependencyGovernanceSourceRoot, dependencyFiles, findings);
        foreach (var file in dependencyFiles.Where(file => Path.GetExtension(file).Equals(".cs", StringComparison.OrdinalIgnoreCase)))
        {
            var text = ReadTextOrFinding(file, findings, File.ReadAllText);
            if (text is null)
            {
                continue;
            }

            foreach (var phrase in prohibitedPhrases)
            {
                if (text.Contains(phrase, StringComparison.Ordinal))
                {
                    findings.Add($"Prohibited dependency-governance source reference found: {phrase} in {RelativePath(file)}");
                }
            }
        }
    }
    private static void ValidateMandatoryLifecycleAuthorityBoundary(
        ICollection<string> findings)
    {
        var source = ReadTextOrFinding(
            LifecycleControlSourcePath,
            findings,
            File.ReadAllText);

        if (source is null)
        {
            return;
        }

        var requiredFragments = new[]
        {
            "LifecycleAuthorityMode.AuthorityEngineRequired",
            "AUTHORITY_ENGINE_REQUIRED",
            "TransitionCore(boundRequest, boundEvidence, authorityEngineEvaluated: true)",
            "AUTHORITY_ENGINE_EVALUATION_REQUIRED"
        };

        foreach (var fragment in requiredFragments)
        {
            if (!source.Contains(fragment, StringComparison.Ordinal))
            {
                findings.Add(
                    $"Mandatory Stage 4 lifecycle authority boundary is missing: {fragment}");
            }
        }

        var publicTransition = source.IndexOf(
            "public LifecycleControlDecision Transition(",
            StringComparison.Ordinal);
        var privateCore = source.IndexOf(
            "private LifecycleControlDecision TransitionCore(",
            StringComparison.Ordinal);
        var requiredGuard = source.IndexOf(
            "if (_authorityMode == LifecycleAuthorityMode.AuthorityEngineRequired)",
            StringComparison.Ordinal);

        if (publicTransition < 0 || privateCore < 0 || requiredGuard < publicTransition ||
            requiredGuard > privateCore)
        {
            findings.Add(
                "Public lifecycle transition surface is not guarded before the private transition core.");
        }
    }


    private static void ValidateAuthoritativeStateBoundary(
        ICollection<string> findings)
    {
        if (!Directory.Exists(StateSourceRoot))
        {
            findings.Add("Missing Foundation.State production boundary.");
            return;
        }

        var files = Directory.GetFiles(StateSourceRoot, "*.cs", SearchOption.TopDirectoryOnly);
        var combined = string.Join(
            Environment.NewLine,
            files.OrderBy(path => path, StringComparer.Ordinal)
                .Select(File.ReadAllText));

        string[] required =
        [
            "StateRepresentationKind.Authoritative",
            "StaleExpectedVersion",
            "UnauthorizedWriter",
            "OwnershipConflict",
            "IMMUTABLE_HISTORY_CONFLICT",
            "CORRUPTED_CURRENT_STATE",
            "ReconciliationState"
        ];

        foreach (var fragment in required)
        {
            if (!combined.Contains(fragment, StringComparison.Ordinal))
            {
                findings.Add($"Mandatory WP-03 state boundary is missing: {fragment}");
            }
        }

        string[] prohibited =
        [
            "HttpClient",
            "Socket",
            "Service Bus",
            "Evidence Journal",
            "AcceptedFactPublisher",
            "RestartReconciler",
            "last-write-wins"
        ];

        foreach (var fragment in prohibited)
        {
            if (combined.Contains(fragment, StringComparison.Ordinal))
            {
                findings.Add($"Prohibited WP-03 state-boundary reference found: {fragment}");
            }
        }
    }

}
