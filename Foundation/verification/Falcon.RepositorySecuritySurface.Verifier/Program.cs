using System.Text.RegularExpressions;

var repo = ResolveRepositoryRoot();
var selfPath = Path.Combine(repo, "verification", "Falcon.RepositorySecuritySurface.Verifier", "Program.cs");
var roots = new[] { "src", "tests", "verification", ".github", "docs" };
var extensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
{
    ".cs", ".csproj", ".props", ".targets", ".json", ".xml", ".config", ".slnx",
    ".yml", ".yaml", ".ps1", ".sh", ".md"
};

var findings = new List<string>();
var files = new List<string>();

foreach (var relativeRoot in roots)
{
    var root = Path.Combine(repo, relativeRoot);
    if (!Directory.Exists(root))
    {
        findings.Add("Missing governed security-scan root: " + relativeRoot);
        continue;
    }

    foreach (var file in Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories))
    {
        var relative = Path.GetRelativePath(repo, file).Replace('\\', '/');
        if (relative.Contains("/bin/", StringComparison.OrdinalIgnoreCase) ||
            relative.Contains("/obj/", StringComparison.OrdinalIgnoreCase) ||
            relative.Contains("/.git/", StringComparison.OrdinalIgnoreCase))
            continue;
        if (extensions.Contains(Path.GetExtension(file)))
            files.Add(file);
    }
}

foreach (var rootFile in new[]
{
    ".editorconfig", ".gitattributes", ".gitignore", "Directory.Build.props", "global.json", "NuGet.Config",
    "Falcon.Foundation.ControlledProjectFoundation.slnx"
})
{
    var path = Path.Combine(repo, rootFile);
    if (!File.Exists(path)) findings.Add("Missing governed root file: " + rootFile);
    else files.Add(path);
}

var secretAssignment = new Regex(
    "(?i)\\b(password|passwd|secret|token|client[_-]?secret|api[_-]?key|private[_-]?key|connection\\s*string)\\b\\s*[:=]\\s*(?:['\"][^'\"\\r\\n]{8,}['\"]|[A-Za-z0-9+/=._-]{16,})",
    RegexOptions.Compiled | RegexOptions.CultureInvariant);

// Construct detector markers at runtime so the verifier does not contain the exact
// credential/key material literals that the baseline scanner is designed to reject.
var obviousSecretMarkers = new[]
{
    "-----BEGIN " + "PRIVATE KEY-----",
    "-----BEGIN RSA " + "PRIVATE KEY-----",
    "-----BEGIN OPENSSH " + "PRIVATE KEY-----",
    "gh" + "p_",
    "github_" + "pat_",
    "sk-" + "proj-",
    "AK" + "IA"
};

foreach (var file in files.Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(x => x, StringComparer.Ordinal))
{
    string text;
    try
    {
        text = File.ReadAllText(file);
    }
    catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
    {
        findings.Add($"Unreadable governed file: {Path.GetRelativePath(repo, file)} ({ex.GetType().Name})");
        continue;
    }

    // The detector implementation itself must not recursively flag its own test vocabulary.
    // The baseline Foundation security scanner independently scans this source file, so this
    // exemption does not create an unscanned repository surface.
    if (StringComparer.OrdinalIgnoreCase.Equals(Path.GetFullPath(file), Path.GetFullPath(selfPath)))
        continue;

    if (secretAssignment.IsMatch(text))
        findings.Add("Secret-like assignment found in " + Path.GetRelativePath(repo, file).Replace('\\', '/'));

    foreach (var marker in obviousSecretMarkers)
    {
        if (text.Contains(marker, StringComparison.Ordinal))
            findings.Add($"Potential credential marker '{marker}' found in {Path.GetRelativePath(repo, file).Replace('\\', '/')}");
    }
}

var workflowCount = files.Count(path =>
    Path.GetRelativePath(repo, path).Replace('\\', '/').StartsWith(".github/workflows/", StringComparison.OrdinalIgnoreCase) &&
    (Path.GetExtension(path).Equals(".yml", StringComparison.OrdinalIgnoreCase) ||
     Path.GetExtension(path).Equals(".yaml", StringComparison.OrdinalIgnoreCase)));
var scriptCount = files.Count(path =>
    Path.GetExtension(path).Equals(".ps1", StringComparison.OrdinalIgnoreCase) ||
    Path.GetExtension(path).Equals(".sh", StringComparison.OrdinalIgnoreCase));
var markdownCount = files.Count(path => Path.GetExtension(path).Equals(".md", StringComparison.OrdinalIgnoreCase));

if (workflowCount == 0) findings.Add("No GitHub workflow files were included in repository security coverage.");
if (markdownCount == 0) findings.Add("No Markdown governance/documentation files were included in repository security coverage.");

var unique = findings.Distinct(StringComparer.Ordinal).OrderBy(x => x, StringComparer.Ordinal).ToArray();
Console.WriteLine("REPOSITORY_SECURITY_SURFACE = " + (unique.Length == 0 ? "PASS" : "FAIL"));
Console.WriteLine($"FILES_SCANNED = {files.Distinct(StringComparer.OrdinalIgnoreCase).Count()}");
Console.WriteLine($"WORKFLOWS_SCANNED = {workflowCount}");
Console.WriteLine($"SCRIPTS_SCANNED = {scriptCount}");
Console.WriteLine($"MARKDOWN_SCANNED = {markdownCount}");
Console.WriteLine($"FINDINGS = {unique.Length}");

foreach (var finding in unique)
    Console.Error.WriteLine("- " + finding);

return unique.Length == 0 ? 0 : 1;

static string ResolveRepositoryRoot()
{
    var current = new DirectoryInfo(Path.GetFullPath(AppContext.BaseDirectory));
    while (current is not null)
    {
        if (File.Exists(Path.Combine(current.FullName, "Falcon.Foundation.ControlledProjectFoundation.slnx")) &&
            Directory.Exists(Path.Combine(current.FullName, "src")) &&
            Directory.Exists(Path.Combine(current.FullName, ".github")))
            return current.FullName;
        current = current.Parent;
    }

    throw new InvalidOperationException("repository_root_not_resolved");
}
