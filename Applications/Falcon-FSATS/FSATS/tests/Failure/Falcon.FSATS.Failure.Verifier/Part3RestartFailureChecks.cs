using System.Runtime.CompilerServices;
using TA = Falcon.FSATS.Trading.Application;
using PA = Falcon.FSATS.FSAPMA.Application;
using GA = Falcon.FSATS.TradingGuardian.Application;
using SA = Falcon.FSATS.FSTSimA.Application;
using RA = Falcon.FSATS.ResourceManagement.Application;

internal static class Part3RestartFailureChecks
{
    [ModuleInitializer]
    internal static void Run()
    {
        var now = DateTimeOffset.UtcNow;
        if (TA.TradingRestartReconstructor.Reconstruct(null, now).Accepted) throw new InvalidOperationException("P3_FAILURE_TRADING_MISSING_STATE_ACCEPTED");
        if (PA.FSAPMARestartReconstructor.Reconstruct(null, now).Accepted) throw new InvalidOperationException("P3_FAILURE_FSAPMA_MISSING_STATE_ACCEPTED");
        if (GA.GuardianRestartReconstructor.Reconstruct(null, now).Accepted) throw new InvalidOperationException("P3_FAILURE_GUARDIAN_MISSING_STATE_ACCEPTED");
        if (SA.SimulationRestartReconstructor.Reconstruct(null, now).Accepted) throw new InvalidOperationException("P3_FAILURE_SIMULATION_MISSING_STATE_ACCEPTED");
        if (RA.ResourceRestartReconstructor.Reconstruct(null, now).Accepted) throw new InvalidOperationException("P3_FAILURE_APP_RSC_MISSING_STATE_ACCEPTED");
    }
}
