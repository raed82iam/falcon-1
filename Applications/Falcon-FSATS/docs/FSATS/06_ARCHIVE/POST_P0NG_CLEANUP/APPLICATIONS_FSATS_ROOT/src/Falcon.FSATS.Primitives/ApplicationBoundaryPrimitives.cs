namespace Falcon.FSATS.Primitives;

public sealed class PackageId : CanonicalId
{
    public PackageId(string value) : base(value) { }
}

public sealed class AwarenessEntityId : CanonicalId
{
    public AwarenessEntityId(string value) : base(value) { }
}
