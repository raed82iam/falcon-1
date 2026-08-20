namespace Falcon.FSATS.FSTSimA.Awareness;

public enum AwarenessTier { Msa, Lsa, Csa }
public sealed record AwarenessIdentity(string Id, AwarenessTier Tier, string Responsibility, string? ParentId, string? TargetComponent = null);

public static class FSTSimAAwarenessTopology
{
    public const string MsaId = "MSA-FSTSIMA-01";
    public static IReadOnlyList<AwarenessIdentity> All { get; } = new List<AwarenessIdentity>
    {
        new(MsaId, AwarenessTier.Msa, "Complete FSTSimA", null),
        new("S-LSA-01", AwarenessTier.Lsa, "Simulation Time and Scenario", MsaId),
        new("S-LSA-02", AwarenessTier.Lsa, "Market Environment Simulation", MsaId),
        new("S-LSA-03", AwarenessTier.Lsa, "Provider and External Service Simulation", MsaId),
        new("S-LSA-04", AwarenessTier.Lsa, "Broker Exchange and Execution Simulation", MsaId),
        new("S-LSA-05", AwarenessTier.Lsa, "Account Capital and Settlement Simulation", MsaId),
        new("S-LSA-06", AwarenessTier.Lsa, "Fault Latency and Crisis Injection", MsaId),
        new("S-LSA-07", AwarenessTier.Lsa, "Fidelity and Calibration", MsaId),
        new("S-LSA-08", AwarenessTier.Lsa, "Oracle Evidence Reproducibility and Validation", MsaId),
        new("CSA-S02-01", AwarenessTier.Csa, "Synthetic market generator specialized self-awareness", "S-LSA-02", "SyntheticMarketGenerator"),
        new("CSA-S07-01", AwarenessTier.Csa, "Calibration engine specialized self-awareness", "S-LSA-07", "CalibrationEngine")
    }.AsReadOnly();
}

public static class ValidationIndependence
{
    public static bool IsCsaEligible(string component) => component is "SyntheticMarketGenerator" or "CalibrationEngine";
    public static bool IsCsaForbiddenInitially(string component) => component is "ValidationAssessor" or "SimulationOracle";
}
