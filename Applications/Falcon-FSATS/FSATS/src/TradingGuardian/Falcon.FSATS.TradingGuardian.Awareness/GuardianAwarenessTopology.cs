namespace Falcon.FSATS.TradingGuardian.Awareness;

public enum AwarenessTier { Msa, Lsa, Csa }
public sealed record AwarenessIdentity(string Id, AwarenessTier Tier, string Responsibility, string? ParentId, string? TargetComponent = null);

public static class GuardianAwarenessTopology
{
    public const string MsaId = "MSA-GUARDIAN-01";
    public static IReadOnlyList<AwarenessIdentity> All { get; } = new List<AwarenessIdentity>
    {
        new(MsaId, AwarenessTier.Msa, "Complete Trading Guardian Application", null),
        new("G-LSA-01", AwarenessTier.Lsa, "Protection Observation and Incident Qualification", MsaId),
        new("G-LSA-02", AwarenessTier.Lsa, "Protection Scope Restriction and Command Governance", MsaId),
        new("G-LSA-03", AwarenessTier.Lsa, "Crisis State Survival and Protection Coordination", MsaId),
        new("G-LSA-04", AwarenessTier.Lsa, "Reconciliation Recovery and Protection Evidence", MsaId),
        new("CSA-G01-01", AwarenessTier.Csa, "Incident classifier specialized self-awareness", "G-LSA-01", "IncidentClassifier")
    }.AsReadOnly();
}

public static class GuardianAwarenessRules
{
    public static bool CanControlDeterministicSafetyKernel(string awarenessId) => false;
    public static bool CanSelfAuthorizeKill(string awarenessId) => false;
}
