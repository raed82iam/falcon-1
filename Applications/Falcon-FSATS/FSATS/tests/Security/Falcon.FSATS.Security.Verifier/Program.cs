using System.Text.RegularExpressions;

var repo = FindRepoRoot();
var src = Path.Combine(repo, "applications", "FSATS", "src");
var files = Directory.GetFiles(src, "*.cs", SearchOption.AllDirectories);
var failures = new List<string>();

var secretPatterns = new[]
{
    new Regex(@"-----BEGIN (RSA |EC |OPENSSH )?PRIVATE KEY-----", RegexOptions.Compiled),
    new Regex(@"\bsk-[A-Za-z0-9_-]{12,}", RegexOptions.Compiled),
    new Regex("(?i)\\b(api[_-]?key|password|client[_-]?secret)\\s*[=:]\\s*[\\\"'][^\\\"']{4,}", RegexOptions.Compiled)
};
var networkTokens = new[] { "new HttpClient", "HttpClient(", "TcpClient", "UdpClient", "ClientWebSocket", "System.Net.Sockets", "WebRequest.Create" };

foreach (var file in files)
{
    var text = File.ReadAllText(file);
    var rel = Path.GetRelativePath(repo, file).Replace('\\', '/');
    foreach (var pattern in secretPatterns)
        if (pattern.IsMatch(text)) failures.Add($"Potential secret material in {rel}");
    foreach (var token in networkTokens)
        if (text.Contains(token, StringComparison.Ordinal)) failures.Add($"Unauthorized direct network primitive '{token}' in {rel}");
}

var contracts = files.Where(x => x.Contains($"{Path.DirectorySeparatorChar}Contracts{Path.DirectorySeparatorChar}", StringComparison.Ordinal) || Path.GetDirectoryName(x)!.Contains(".Contracts", StringComparison.Ordinal)).ToArray();
foreach (var file in contracts)
{
    var text = File.ReadAllText(file);
    if (Regex.IsMatch(text, @"\b(string|byte\[\])\s+(Password|Secret|ApiKey|AccessToken|RefreshToken)\b", RegexOptions.IgnoreCase))
        failures.Add($"Reusable secret-shaped field in contract: {Path.GetRelativePath(repo, file)}");
}

if (failures.Count > 0)
{
    Console.Error.WriteLine("FSATS SECURITY VERIFIER: FAIL");
    foreach (var failure in failures) Console.Error.WriteLine(" - " + failure);
    return 1;
}

Console.WriteLine($"FSATS SECURITY VERIFIER: PASS ({files.Length} source files; lexical scan detected no listed secret literals or direct network primitives)");
Console.WriteLine("ASSURANCE BOUNDARY: lexical source scanning is defense-in-depth only; it is not proof that every possible egress mechanism is absent. Runtime route/authority controls, architecture boundaries, dependency review, and governed integration verification remain independently required.");
return 0;

static string FindRepoRoot()
{
    var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
    while (dir is not null)
    {
        if (File.Exists(Path.Combine(dir.FullName, "Falcon.Foundation.ControlledProjectFoundation.slnx"))) return dir.FullName;
        dir = dir.Parent;
    }
    throw new InvalidOperationException("Repository root not found");
}
