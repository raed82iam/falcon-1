namespace Falcon.FSATS.FSAPMA.Awareness;

public enum AwarenessTier { Msa, Lsa, Csa }
public sealed record AwarenessIdentity(string Id, AwarenessTier Tier, string Responsibility, string? ParentId, string? TargetComponent = null);

public static class FSAPMAAwarenessTopology
{
    public const string MsaId = "MSA-FSAPMA-01";
    public static IReadOnlyList<AwarenessIdentity> All { get; } = new List<AwarenessIdentity>
    {
        new(MsaId, AwarenessTier.Msa, "Complete FSAPMA", null),
        new("P-LSA-01", AwarenessTier.Lsa, "Provider Registry and Onboarding", MsaId),
        new("P-LSA-02", AwarenessTier.Lsa, "Data Products Semantics and Normalization", MsaId),
        new("P-LSA-03", AwarenessTier.Lsa, "Provider Capability Account and Entitlement", MsaId),
        new("P-LSA-04", AwarenessTier.Lsa, "Provider Selection Routing and Delivery", MsaId),
        new("P-LSA-05", AwarenessTier.Lsa, "Data Quality Verification and Reconciliation", MsaId),
        new("P-LSA-06", AwarenessTier.Lsa, "Quota Capacity Cost and Reliability", MsaId),
        new("CSA-P05-01", AwarenessTier.Csa, "Anomaly detector specialized self-awareness", "P-LSA-05", "AnomalyDetector")
    }.AsReadOnly();
}
