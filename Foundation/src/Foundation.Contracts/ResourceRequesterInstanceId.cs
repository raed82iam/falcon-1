namespace Foundation.Contracts.ResourceGovernance;

/// <summary>
/// Canonical identity of one concrete requester/controller instance participating in a
/// governed resource boundary. This identity is separate from both the owning
/// Application identity and the logical requester role identity, and does not itself
/// grant resource authority.
/// </summary>
public sealed record ResourceRequesterInstanceId : CanonicalResourceIdentifier
{
    public ResourceRequesterInstanceId(string value) : base(value) { }
}
