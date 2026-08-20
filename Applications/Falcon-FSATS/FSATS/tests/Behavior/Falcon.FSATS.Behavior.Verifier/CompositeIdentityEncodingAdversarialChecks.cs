using T = Falcon.FSATS.Trading.Domain;
using TA = Falcon.FSATS.Trading.Application;
using P = Falcon.FSATS.FSAPMA.Domain;
using PC = Falcon.FSATS.FSAPMA.Contracts;
using GC = Falcon.FSATS.TradingGuardian.Contracts;
using SC = Falcon.FSATS.FSTSimA.Contracts;

internal static class CompositeIdentityEncodingAdversarialChecks
{
    internal static void Run()
    {
        BrokerAccountNamespaceRejectsDelimiterCollision();
        ExecutionNamespaceRejectsDelimiterCollision();
        ProviderRouteNamespaceRejectsDelimiterCollision();
        ProjectionRouteNamespaceRejectsDelimiterCollision();
        GuardianTargetNamespaceRejectsDelimiterCollision();
        SimulationScopeRejectsDelimiterCollision();
        FailureDependencyRejectsDelimiterCollision();
    }

    private static void BrokerAccountNamespaceRejectsDelimiterCollision()
    {
        var a = new T.BrokerAccountContext("A|B", "C", "PAPER");
        var b = new T.BrokerAccountContext("A", "B|C", "PAPER");
        if (a == b || StringComparer.Ordinal.Equals(a.NamespaceKey, b.NamespaceKey))
            throw new InvalidOperationException("COMPOSITE_BROKER_ACCOUNT_DELIMITER_COLLISION");
    }

    private static void ExecutionNamespaceRejectsDelimiterCollision()
    {
        var account = new T.BrokerAccountContext("ALPACA", "A", "PAPER");
        var a = new TA.BrokerExecutionIdentity(account, "R|S", "T", new T.OrderId("O"));
        var b = new TA.BrokerExecutionIdentity(account, "R", "S|T", new T.OrderId("O"));
        if (a == b || StringComparer.Ordinal.Equals(a.NamespaceKey, b.NamespaceKey))
            throw new InvalidOperationException("COMPOSITE_EXECUTION_IDENTITY_DELIMITER_COLLISION");
    }

    private static void ProviderRouteNamespaceRejectsDelimiterCollision()
    {
        var a = new P.ProviderRouteIdentity(new P.ProviderId("P|Q"), new P.ProviderAccountId("A"), "PAPER", "MARKET_DATA", new P.CredentialReference("C"));
        var b = new P.ProviderRouteIdentity(new P.ProviderId("P"), new P.ProviderAccountId("Q|A"), "PAPER", "MARKET_DATA", new P.CredentialReference("C"));
        if (a == b || StringComparer.Ordinal.Equals(a.NamespaceKey, b.NamespaceKey))
            throw new InvalidOperationException("COMPOSITE_PROVIDER_ROUTE_DELIMITER_COLLISION");
    }

    private static void ProjectionRouteNamespaceRejectsDelimiterCollision()
    {
        var now = DateTimeOffset.UtcNow;
        var a = new PC.OperationalDataProjection(new PC.ObservationId("A"), new PC.ProviderId("P|Q"), new PC.ProducerInstrumentId("P|Q", "I"), new PC.DataProductId("D"), 1m, now, now, PC.DataTruthState.Current, "prov", "1", new PC.ProviderAccountId("A"), "PAPER", "MARKET_DATA", "C");
        var b = new PC.OperationalDataProjection(new PC.ObservationId("B"), new PC.ProviderId("P"), new PC.ProducerInstrumentId("P", "I"), new PC.DataProductId("D"), 1m, now, now, PC.DataTruthState.Current, "prov", "1", new PC.ProviderAccountId("Q|A"), "PAPER", "MARKET_DATA", "C");
        if (StringComparer.Ordinal.Equals(a.ProviderRouteNamespace, b.ProviderRouteNamespace))
            throw new InvalidOperationException("COMPOSITE_PROVIDER_PROJECTION_DELIMITER_COLLISION");
    }

    private static void GuardianTargetNamespaceRejectsDelimiterCollision()
    {
        var a = new GC.ProtectionTarget(GC.ProtectionTargetKind.BrokerAccount, "A|B", "C", "PAPER");
        var b = new GC.ProtectionTarget(GC.ProtectionTargetKind.BrokerAccount, "A", "B|C", "PAPER");
        if (a == b || StringComparer.Ordinal.Equals(a.CanonicalKey, b.CanonicalKey))
            throw new InvalidOperationException("COMPOSITE_GUARDIAN_TARGET_DELIMITER_COLLISION");
    }

    private static void SimulationScopeRejectsDelimiterCollision()
    {
        var a = new SC.SimulationScope("BROKER_ACCOUNT", "A|B", "C", "PAPER");
        var b = new SC.SimulationScope("BROKER_ACCOUNT", "A", "B|C", "PAPER");
        if (a == b || StringComparer.Ordinal.Equals(a.CanonicalKey, b.CanonicalKey))
            throw new InvalidOperationException("COMPOSITE_SIMULATION_SCOPE_DELIMITER_COLLISION");
    }

    private static void FailureDependencyRejectsDelimiterCollision()
    {
        var a = T.FailureLocalityEvidence.ProviderAccountDependency("P|Q", "A");
        var b = T.FailureLocalityEvidence.ProviderAccountDependency("P", "Q|A");
        if (StringComparer.Ordinal.Equals(a, b))
            throw new InvalidOperationException("COMPOSITE_FAILURE_DEPENDENCY_DELIMITER_COLLISION");

        var r1 = T.FailureLocalityEvidence.ExecutionRouteDependency("B|X", "A", "PAPER", "R");
        var r2 = T.FailureLocalityEvidence.ExecutionRouteDependency("B", "X|A", "PAPER", "R");
        if (StringComparer.Ordinal.Equals(r1, r2))
            throw new InvalidOperationException("COMPOSITE_FAILURE_ROUTE_DEPENDENCY_DELIMITER_COLLISION");
    }
}
