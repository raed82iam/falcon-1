using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System.Text;

namespace Falcon.Stage0B.Candidates;

public enum TrustObjectStatus
{
    Valid,
    Incomplete,
    Invalid,
    Conflicted,
    Stale,
    Uncertain,
    Superseded
}

public sealed record TrustClaim(
    string ClaimId,
    string Type,
    string Value,
    string Scope,
    string GoverningPolicy,
    string Producer,
    bool Challengeable);

public sealed record Stage0BTrustObject(
    string ObjectId,
    string ObjectType,
    string Version,
    string Provenance,
    string AuthorityScope,
    TrustObjectStatus Status,
    IReadOnlyList<TrustClaim> Claims,
    string Digest,
    string? Supersedes)
{
    public static Stage0BTrustObject Create(
        string objectId,
        string objectType,
        string version,
        string provenance,
        string authorityScope,
        TrustObjectStatus status,
        IEnumerable<TrustClaim> claims,
        string? supersedes = null)
    {
        var ordered = claims.OrderBy(claim => claim.ClaimId, StringComparer.Ordinal).ToArray();
        var canonical = string.Join(
            "\n",
            objectId,
            objectType,
            version,
            provenance,
            authorityScope,
            status.ToString(),
            supersedes ?? string.Empty,
            string.Join("\n", ordered.Select(claim =>
                string.Join("|", claim.ClaimId, claim.Type, claim.Value, claim.Scope, claim.GoverningPolicy, claim.Producer, claim.Challengeable))));
        var digest = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical)));
        return new Stage0BTrustObject(
            objectId,
            objectType,
            version,
            provenance,
            authorityScope,
            status,
            new ReadOnlyCollection<TrustClaim>(ordered),
            digest,
            supersedes);
    }

    public bool IsAcceptedFor(string scope, string governingPolicy) =>
        Status == TrustObjectStatus.Valid &&
        StringComparer.Ordinal.Equals(AuthorityScope, scope) &&
        Claims.All(claim =>
            StringComparer.Ordinal.Equals(claim.Scope, scope) &&
            StringComparer.Ordinal.Equals(claim.GoverningPolicy, governingPolicy));
}

public sealed class TrustObjectPrimitivesCandidate : CandidateProviderBase
{
    public TrustObjectPrimitivesCandidate()
        : base("CND-TRUST-001")
    {
    }

    public Stage0BTrustObject Create(
        string objectId,
        string objectType,
        string version,
        string provenance,
        string authorityScope,
        TrustObjectStatus status,
        IEnumerable<TrustClaim> claims,
        string? supersedes = null) =>
        Stage0BTrustObject.Create(
            objectId,
            objectType,
            version,
            provenance,
            authorityScope,
            status,
            claims,
            supersedes);
}
