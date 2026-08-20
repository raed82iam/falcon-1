namespace Falcon.FSATS.ResourceManagement.Awareness;

public enum AwarenessTier { Msa, Lsa, Csa }
public sealed record AwarenessIdentity(string Id, AwarenessTier Tier, string Responsibility, string? ParentId, string? TargetComponent = null);

public static class ResourceManagementAwarenessTopology
{
    public const string MsaId = "MSA-APP-RSC-01";
    public static IReadOnlyList<AwarenessIdentity> All { get; } = new List<AwarenessIdentity>
    {
        new(MsaId, AwarenessTier.Msa, "Complete APP-RSC Application", null),
        new("R-LSA-01", AwarenessTier.Lsa, "Resource Picture Demand Integrity and Coordination Envelope", MsaId),
        new("R-LSA-02", AwarenessTier.Lsa, "Redistribution Degradation and Rebalance", MsaId),
        new("R-LSA-03", AwarenessTier.Lsa, "Foundation Binding Restoration and Resource Evidence", MsaId)
    }.AsReadOnly();

    public static int InitialCsaCount => 0;
}

public static class ResourceAwarenessBoundary
{
    public static bool IsAwarenessTier(string component)
        => !string.IsNullOrWhiteSpace(component)
           && ResourceManagementAwarenessTopology.All.Any(x => StringComparer.Ordinal.Equals(x.Id, component.Trim()));

    public static bool MayMintFoundationGrant => false;
}
