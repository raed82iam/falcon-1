using System.Runtime.CompilerServices;
using TA = Falcon.FSATS.Trading.Application;
using PA = Falcon.FSATS.FSAPMA.Application;
using GA = Falcon.FSATS.TradingGuardian.Application;
using SA = Falcon.FSATS.FSTSimA.Application;
using RA = Falcon.FSATS.ResourceManagement.Application;

internal static class Fcr0226Stage13TargetKindAdversarialChecks
{
    [ModuleInitializer]
    internal static void Run()
    {
        ApplicationIdsAreOwningScopesNotStage13Targets();
        ExactAwarenessTargetInventoryIsFortySix();
    }

    private static void ApplicationIdsAreOwningScopesNotStage13Targets()
    {
        var applicationIds = new[]
        {
            TA.TradingRuntimeAdmissionReadiness.ApplicationId,
            PA.FSAPMARuntimeAdmissionReadiness.ApplicationId,
            GA.GuardianRuntimeAdmissionReadiness.ApplicationId,
            SA.FSTSimARuntimeAdmissionReadiness.ApplicationId,
            RA.ResourceRuntimeAdmissionReadiness.ApplicationId
        };

        var targets = AllTargets();
        foreach (var applicationId in applicationIds)
            if (targets.Contains(applicationId, StringComparer.Ordinal))
                throw new InvalidOperationException("FCR0226_APPLICATION_ID_FABRICATED_AS_STAGE13_AI_TARGET");
    }

    private static void ExactAwarenessTargetInventoryIsFortySix()
    {
        RequireExact(
            TA.TradingRuntimeAdmissionReadiness.ExpectedStage13AiTargetIds,
            new[] { TA.TradingManifest.Current.MsaId }.Concat(TA.TradingManifest.Current.LsaIds).Concat(TA.TradingManifest.Current.CsaIds),
            "TRADING");
        RequireExact(
            PA.FSAPMARuntimeAdmissionReadiness.ExpectedStage13AiTargetIds,
            new[] { PA.FSAPMAManifest.Current.MsaId }.Concat(PA.FSAPMAManifest.Current.LsaIds).Concat(PA.FSAPMAManifest.Current.CsaIds),
            "FSAPMA");
        RequireExact(
            GA.GuardianRuntimeAdmissionReadiness.ExpectedStage13AiTargetIds,
            new[] { GA.TradingGuardianManifest.Current.MsaId }.Concat(GA.TradingGuardianManifest.Current.LsaIds).Concat(GA.TradingGuardianManifest.Current.CsaIds),
            "GUARDIAN");
        RequireExact(
            SA.FSTSimARuntimeAdmissionReadiness.ExpectedStage13AiTargetIds,
            new[] { SA.FSTSimAManifest.Current.MsaId }.Concat(SA.FSTSimAManifest.Current.LsaIds).Concat(SA.FSTSimAManifest.Current.CsaIds),
            "FSTSIMA");
        RequireExact(
            RA.ResourceRuntimeAdmissionReadiness.ExpectedStage13AiTargetIds,
            new[] { RA.ResourceManagementManifest.Current.MsaId }.Concat(RA.ResourceManagementManifest.Current.LsaIds).Concat(RA.ResourceManagementManifest.Current.CsaIds),
            "APP_RSC");

        var all = AllTargets();
        if (all.Length != 46) throw new InvalidOperationException($"FCR0226_STAGE13_TARGET_COUNT_MISMATCH:{all.Length}");
        if (all.Distinct(StringComparer.Ordinal).Count() != 46) throw new InvalidOperationException("FCR0226_STAGE13_TARGET_IDENTITY_DUPLICATE");
    }

    private static string[] AllTargets()
        => TA.TradingRuntimeAdmissionReadiness.ExpectedStage13AiTargetIds
            .Concat(PA.FSAPMARuntimeAdmissionReadiness.ExpectedStage13AiTargetIds)
            .Concat(GA.GuardianRuntimeAdmissionReadiness.ExpectedStage13AiTargetIds)
            .Concat(SA.FSTSimARuntimeAdmissionReadiness.ExpectedStage13AiTargetIds)
            .Concat(RA.ResourceRuntimeAdmissionReadiness.ExpectedStage13AiTargetIds)
            .ToArray();

    private static void RequireExact(IReadOnlyList<string> actual, IEnumerable<string> expectedSource, string application)
    {
        var expected = expectedSource.ToArray();
        if (actual.Count != expected.Length || actual.Distinct(StringComparer.Ordinal).Count() != actual.Count ||
            !expected.All(x => actual.Contains(x, StringComparer.Ordinal)))
            throw new InvalidOperationException($"FCR0226_{application}_STAGE13_TARGET_SET_DOES_NOT_MATCH_ACCEPTED_AWARENESS_INVENTORY");
    }
}
