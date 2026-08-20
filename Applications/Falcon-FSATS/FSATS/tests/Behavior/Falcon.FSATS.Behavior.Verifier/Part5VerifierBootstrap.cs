using System.Runtime.CompilerServices;

internal static class Part5VerifierBootstrap
{
    [ModuleInitializer]
    internal static void Initialize() => Part5HealthReadinessAdversarialChecks.Run();
}
