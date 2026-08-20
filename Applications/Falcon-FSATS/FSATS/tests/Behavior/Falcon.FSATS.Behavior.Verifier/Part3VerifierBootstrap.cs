using System.Runtime.CompilerServices;

internal static class Part3VerifierBootstrap
{
    [ModuleInitializer]
    internal static void Initialize() => Part3DurabilityAdversarialChecks.Run();
}
