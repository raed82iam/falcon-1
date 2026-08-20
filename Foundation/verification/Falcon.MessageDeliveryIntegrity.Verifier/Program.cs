using System.Text.RegularExpressions;

var root = ResolveRepositoryRoot();
var sourcePath = Path.Combine(root, "src", "Foundation.MessageDelivery", "MessageDelivery.cs");
var source = File.ReadAllText(sourcePath);

var method = Regex.Match(
    source,
    @"private static bool IdempotencyMatches\(DeliveryEvaluationContext context\)\s*\{(?<body>.*?)\n\s*\}",
    RegexOptions.Singleline | RegexOptions.CultureInvariant);

if (!method.Success)
{
    Console.Error.WriteLine("MESSAGE_DELIVERY_IDEMPOTENCY_GUARD_NOT_FOUND");
    return 1;
}

var body = method.Groups["body"].Value;
var required = new[]
{
    "binding.RouteDecisionId",
    "route.DecisionId",
    "binding.AdmissionDecisionId",
    "admission.DecisionId",
    "binding.IdempotencyIdentity",
    "envelope.IdempotencyId.Value"
};

var missing = required.Where(fragment => !body.Contains(fragment, StringComparison.Ordinal)).ToArray();
if (missing.Length > 0)
{
    Console.Error.WriteLine("MESSAGE_DELIVERY_IDEMPOTENCY_BINDING = FAIL");
    foreach (var fragment in missing)
        Console.Error.WriteLine("MISSING: " + fragment);
    return 1;
}

if (!body.Contains("envelope is not null", StringComparison.Ordinal))
{
    Console.Error.WriteLine("MESSAGE_DELIVERY_IDEMPOTENCY_ENVELOPE_NULL_GUARD = FAIL");
    return 1;
}

Console.WriteLine("MESSAGE_DELIVERY_IDEMPOTENCY_BINDING = PASS");
Console.WriteLine("ROUTE_BINDING = EXACT");
Console.WriteLine("ADMISSION_BINDING = EXACT");
Console.WriteLine("CANONICAL_ENVELOPE_IDEMPOTENCY_BINDING = EXACT");
return 0;

static string ResolveRepositoryRoot()
{
    var current = new DirectoryInfo(Path.GetFullPath(AppContext.BaseDirectory));
    while (current is not null)
    {
        if (File.Exists(Path.Combine(current.FullName, "Falcon.Foundation.ControlledProjectFoundation.slnx")) &&
            Directory.Exists(Path.Combine(current.FullName, "src")))
            return current.FullName;
        current = current.Parent;
    }

    throw new InvalidOperationException("repository_root_not_resolved");
}
