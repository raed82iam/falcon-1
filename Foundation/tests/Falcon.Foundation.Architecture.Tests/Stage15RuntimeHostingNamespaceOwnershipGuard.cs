using System;
using System.IO;

namespace Falcon.Foundation.Architecture.Tests;

internal static partial class Program
{
    private static readonly bool Stage15RuntimeHostingNamespaceOwnershipGuardInitialized = ValidateStage15RuntimeHostingNamespaceOwnership();

    private static bool ValidateStage15RuntimeHostingNamespaceOwnership()
    {
        var repositoryRoot = Path.GetFullPath(Directory.GetCurrentDirectory());
        var sourcePath = Path.Combine(
            repositoryRoot,
            "src",
            "Foundation.ApplicationRuntimeHosting",
            "Stage15ApplicationRuntimeHost.cs");

        if (!File.Exists(sourcePath))
        {
            throw new InvalidOperationException($"Stage 15 runtime-host source missing: {sourcePath}");
        }

        var source = File.ReadAllText(sourcePath);

        if (!source.Contains("namespace Foundation.ApplicationRuntimeHosting;", StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                "Stage 15 runtime-host public source must be owned by namespace Foundation.ApplicationRuntimeHosting.");
        }

        if (source.Contains("namespace Foundation.ApplicationLifecycle;", StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                "Stage 15 runtime-host public source must not declare the closed predecessor namespace Foundation.ApplicationLifecycle.");
        }

        return true;
    }
}
