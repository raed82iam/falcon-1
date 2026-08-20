using System.Runtime.CompilerServices;

internal static class Part6VerifierBootstrap
{
    [ModuleInitializer]
    internal static void Initialize() => Part6ConfigurationAdversarialChecks.Run();
}
