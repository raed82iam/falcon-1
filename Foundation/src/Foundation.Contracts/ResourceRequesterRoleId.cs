namespace Foundation.Contracts.ResourceGovernance;

/// <summary>
/// Canonical Application-side requester/controller role identity used at a governed
/// Foundation resource boundary. This value does not itself grant authority.
/// </summary>
public sealed record ResourceRequesterRoleId : CanonicalResourceIdentifier
{
    public ResourceRequesterRoleId(string value) : base(value) { }
}
