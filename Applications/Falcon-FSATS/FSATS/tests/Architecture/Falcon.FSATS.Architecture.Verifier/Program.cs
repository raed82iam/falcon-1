using System.Xml.Linq;

var repo = FindRepoRoot();
var src = Path.Combine(repo, "applications", "FSATS", "src");
var projectFiles = Directory.GetFiles(src, "*.csproj", SearchOption.AllDirectories);
var failures = new List<string>();
var expectedApps = new[] { "Trading", "FSAPMA", "TradingGuardian", "FSTSimA", "ResourceManagement" };
var expectedRoles = new[] { "Contracts", "Domain", "Application", "Infrastructure", "Awareness", "Host" };

Check(projectFiles.Length == 30, $"Expected 30 source projects, found {projectFiles.Length}");

foreach (var app in expectedApps)
{
    var appRoot = Path.Combine(src, app);
    var projects = Directory.GetFiles(appRoot, "*.csproj", SearchOption.AllDirectories);
    Check(projects.Length == 6, $"{app}: expected 6 projects, found {projects.Length}");
    foreach (var role in expectedRoles)
        Check(projects.Count(x => Path.GetFileNameWithoutExtension(x).EndsWith('.' + role, StringComparison.Ordinal)) == 1, $"{app}: missing/duplicate {role} project");
}

foreach (var file in projectFiles)
{
    var rel = Path.GetRelativePath(src, file).Replace('\\', '/');
    var app = rel.Split('/')[0];
    var name = Path.GetFileNameWithoutExtension(file);
    var doc = XDocument.Load(file);
    var refs = doc.Descendants("ProjectReference").Select(x => (string?)x.Attribute("Include")).Where(x => !string.IsNullOrWhiteSpace(x)).Cast<string>().ToArray();

    foreach (var reference in refs)
    {
        var target = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file)!, reference));
        var targetRel = Path.GetRelativePath(src, target).Replace('\\', '/');
        var targetApp = targetRel.Split('/')[0];
        Check(targetApp == app, $"Cross-Application ProjectReference forbidden: {rel} -> {targetRel}");
    }

    if (name.EndsWith(".Contracts", StringComparison.Ordinal) || name.EndsWith(".Domain", StringComparison.Ordinal))
        Check(refs.Length == 0, $"{rel}: Contracts/Domain must not reference another project");

    if (name.EndsWith(".Application", StringComparison.Ordinal))
        Check(refs.All(x => !x.Contains("Infrastructure", StringComparison.Ordinal) && !x.Contains("Host", StringComparison.Ordinal)), $"{rel}: Application cannot reference Infrastructure/Host");

    if (name.EndsWith(".Awareness", StringComparison.Ordinal))
        Check(refs.All(x => !x.Contains("Infrastructure", StringComparison.Ordinal) && !x.Contains("Host", StringComparison.Ordinal)), $"{rel}: Awareness cannot reference side-effect Infrastructure/Host");
}

var forbiddenFoundationCopies = Directory.GetFiles(src, "*", SearchOption.AllDirectories)
    .Where(x => Path.GetFileName(x).StartsWith("Foundation.", StringComparison.Ordinal) || x.Contains($"{Path.DirectorySeparatorChar}Foundation{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
    .ToArray();
Check(forbiddenFoundationCopies.Length == 0, "Foundation source-copy pattern found inside FSATS source tree");

if (failures.Count > 0)
{
    Console.Error.WriteLine("FSATS ARCHITECTURE VERIFIER: FAIL");
    foreach (var failure in failures) Console.Error.WriteLine(" - " + failure);
    return 1;
}

Console.WriteLine($"FSATS ARCHITECTURE VERIFIER: PASS ({projectFiles.Length} source projects / 5 Applications / 6 roles each)");
return 0;

void Check(bool condition, string message) { if (!condition) failures.Add(message); }

static string FindRepoRoot()
{
    var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
    while (dir is not null)
    {
        if (Directory.Exists(Path.Combine(dir.FullName, ".git")) || File.Exists(Path.Combine(dir.FullName, "Falcon.Foundation.ControlledProjectFoundation.slnx"))) return dir.FullName;
        dir = dir.Parent;
    }
    throw new InvalidOperationException("Repository root not found");
}
