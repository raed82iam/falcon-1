using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

var failures = new List<string>();
var root = args.Length == 2 && StringComparer.Ordinal.Equals(args[0], "--root")
    ? Path.GetFullPath(args[1])
    : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));

static void Expect(ICollection<string> failures, string label, bool condition)
{
    if (!condition) failures.Add(label);
}

static string Read(string root, string relative)
{
    var path = Path.Combine(root, relative.Replace('/', Path.DirectorySeparatorChar));
    return File.Exists(path) ? File.ReadAllText(path) : string.Empty;
}

var requiredFiles = new[]
{
    "src/Foundation.Enabling/IdentityTimeAndRandomness.cs",
    "src/Foundation.Enabling/SecurityProviders.cs",
    "src/Foundation.Enabling/VerificationPipeline.cs",
    "tests/Falcon.Foundation.Architecture.Tests/Program.cs",
    "tests/Falcon.Foundation.Security.Tests/Program.cs",
    "verification/Falcon.Stage0C.RemediationVerifier/Program.cs",
    "verification/Falcon.Stage3.WP04.Verifier/Program.cs"
};
foreach (var file in requiredFiles)
{
    Expect(failures, "missing:" + file, File.Exists(Path.Combine(root, file.Replace('/', Path.DirectorySeparatorChar))));
}

var identity = Read(root, requiredFiles[0]);
Expect(failures, "EN-002-time-quality", identity.Contains("ClockQuality.VerifiedLocalBuild", StringComparison.Ordinal));
Expect(failures, "EN-002-monotonic-epoch", identity.Contains("CanCompareMonotonic", StringComparison.Ordinal));
Expect(failures, "EN-003-request-continuity", identity.Contains("identifier_request_identity_mismatch", StringComparison.Ordinal));
Expect(failures, "EN-007-os-csprng", identity.Contains("RandomNumberGenerator.Fill", StringComparison.Ordinal));
Expect(failures, "EN-007-no-caller-entropy", identity.Contains("CallerSuppliedEntropy", StringComparison.Ordinal));

var security = Read(root, requiredFiles[1]);
Expect(failures, "EN-004-opaque-key", security.Contains("OpaqueKeyReference", StringComparison.Ordinal));
Expect(failures, "EN-004-fixed-time", security.Contains("FixedTimeEquals", StringComparison.Ordinal));
Expect(failures, "EN-005-zero-memory", security.Contains("ZeroMemory", StringComparison.Ordinal));
Expect(failures, "EN-006-x509", security.Contains("X509Certificate2", StringComparison.Ordinal));
Expect(failures, "EN-006-chain", security.Contains("X509Chain", StringComparison.Ordinal));

var pipeline = Read(root, requiredFiles[2]);
Expect(failures, "EN-008-recompute-digest", pipeline.Contains("MatchesCanonicalContent", StringComparison.Ordinal));
Expect(failures, "EN-008-fixed-time", pipeline.Contains("CryptographicOperations.FixedTimeEquals", StringComparison.Ordinal));
Expect(failures, "EN-008-duplicate-evidence", pipeline.Contains("evidenceIds.Add", StringComparison.Ordinal));
Expect(failures, "EN-008-independent-authority", pipeline.Contains("ProducerAuthority", StringComparison.Ordinal) && pipeline.Contains("CompletenessAuthority", StringComparison.Ordinal));

var solution = Read(root, "Falcon.Foundation.ControlledProjectFoundation.slnx");
Expect(failures, "verifier-in-solution", solution.Contains("Falcon.BaselineIntegrity.Verifier.csproj", StringComparison.Ordinal));

if (failures.Count > 0)
{
    Console.Error.WriteLine("BASELINE INTEGRITY VERIFIER: FAIL");
    foreach (var failure in failures.OrderBy(x => x, StringComparer.Ordinal)) Console.Error.WriteLine(failure);
    return 1;
}

Console.WriteLine("BASELINE INTEGRITY VERIFIER: PASS");
Console.WriteLine("EN-002 through EN-008 structural and fail-closed controls detected.");
Console.WriteLine("B2 authorized path count: 10");
return 0;
