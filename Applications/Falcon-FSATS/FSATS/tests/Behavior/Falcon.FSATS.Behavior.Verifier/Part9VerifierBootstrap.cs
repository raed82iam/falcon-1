using System.Runtime.CompilerServices;

internal static class Part9VerifierBootstrap
{
    [ModuleInitializer]
    internal static void Initialize() => Part9DigitalCityAdversarialChecks.Run();
}
