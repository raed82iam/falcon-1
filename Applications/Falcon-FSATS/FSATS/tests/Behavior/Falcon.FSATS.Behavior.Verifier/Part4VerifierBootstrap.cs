using System.Runtime.CompilerServices;

internal static class Part4VerifierBootstrap
{
    [ModuleInitializer]
    internal static void Initialize() => Part4LifecycleAdversarialChecks.Run();
}
