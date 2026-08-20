using Falcon.FSATS.Primitives;

namespace Falcon.FSATS.ContractSpine;

public enum ContractTrafficContext
{
    Operational = 1,
    Protection = 2,
    Evidence = 3,
    Replay = 4,
    Simulation = 5,
    Research = 6,
    Presentation = 7,
    Notification = 8
}

public enum ContractEndpointRole
{
    Producer = 1,
    Consumer = 2,
    Requester = 3,
    Responder = 4
}

public sealed record ContractEndpoint
{
    public ContractEndpoint(FsatsApplicationId applicationId, ContractEndpointRole role)
    {
        ApplicationId = applicationId ?? throw new ArgumentNullException(nameof(applicationId));

        if (!Enum.IsDefined(role))
        {
            throw new ArgumentOutOfRangeException(nameof(role));
        }

        Role = role;
    }

    public FsatsApplicationId ApplicationId { get; }
    public ContractEndpointRole Role { get; }
}

public sealed record ContractFamilyDeclaration
{
    public ContractFamilyDeclaration(
        ContractFamilyId id,
        ContractEndpoint source,
        ContractEndpoint target,
        ContractTrafficContext context,
        bool latencySensitive,
        params string[] foundationRequestIds)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        Source = source ?? throw new ArgumentNullException(nameof(source));
        Target = target ?? throw new ArgumentNullException(nameof(target));

        if (Source.ApplicationId == Target.ApplicationId)
        {
            throw new ArgumentException("cross_application_contract_requires_distinct_applications", nameof(target));
        }

        if (!Enum.IsDefined(context))
        {
            throw new ArgumentOutOfRangeException(nameof(context));
        }

        ValidateRolePair(Source.Role, Target.Role);

        Context = context;
        LatencySensitive = latencySensitive;

        var requests = (foundationRequestIds ?? throw new ArgumentNullException(nameof(foundationRequestIds)))
            .Select(RequireFcrId)
            .ToArray();

        if (requests.Distinct(StringComparer.Ordinal).Count() != requests.Length)
        {
            throw new ArgumentException("duplicate_foundation_request_id", nameof(foundationRequestIds));
        }

        FoundationRequestIds = Array.AsReadOnly(requests.OrderBy(x => x, StringComparer.Ordinal).ToArray());
    }

    public ContractFamilyId Id { get; }
    public ContractEndpoint Source { get; }
    public ContractEndpoint Target { get; }
    public ContractTrafficContext Context { get; }
    public bool LatencySensitive { get; }
    public IReadOnlyList<string> FoundationRequestIds { get; }

    private static void ValidateRolePair(ContractEndpointRole source, ContractEndpointRole target)
    {
        var valid =
            source == ContractEndpointRole.Producer && target == ContractEndpointRole.Consumer ||
            source == ContractEndpointRole.Requester && target == ContractEndpointRole.Responder;

        if (!valid)
        {
            throw new ArgumentException("invalid_contract_endpoint_role_pair");
        }
    }

    private static string RequireFcrId(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !System.Text.RegularExpressions.Regex.IsMatch(value, "^FCR-[0-9]{4}$"))
        {
            throw new ArgumentException("canonical_fcr_id_required", nameof(value));
        }

        return value;
    }
}

public static class FsatsContractFamilies
{
    private static readonly FsatsApplicationId Guardian = new("falcon.trading.guardian");
    private static readonly FsatsApplicationId Fsapma = new("falcon.trading.fsapma");
    private static readonly FsatsApplicationId Trading = new("falcon.trading.application");

    public static IReadOnlyList<ContractFamilyDeclaration> Core { get; } = Array.AsReadOnly(
        new[]
        {
            new ContractFamilyDeclaration(
                new ContractFamilyId("fsats.contract.market-data-requirement"),
                new ContractEndpoint(Trading, ContractEndpointRole.Requester),
                new ContractEndpoint(Fsapma, ContractEndpointRole.Responder),
                ContractTrafficContext.Operational,
                true,
                "FCR-0005", "FCR-0009"),

            new ContractFamilyDeclaration(
                new ContractFamilyId("fsats.contract.normalized-operational-market-data"),
                new ContractEndpoint(Fsapma, ContractEndpointRole.Producer),
                new ContractEndpoint(Trading, ContractEndpointRole.Consumer),
                ContractTrafficContext.Operational,
                true,
                "FCR-0005", "FCR-0009"),

            new ContractFamilyDeclaration(
                new ContractFamilyId("fsats.contract.trading-protection-command"),
                new ContractEndpoint(Guardian, ContractEndpointRole.Producer),
                new ContractEndpoint(Trading, ContractEndpointRole.Consumer),
                ContractTrafficContext.Protection,
                true,
                "FCR-0004", "FCR-0009"),

            new ContractFamilyDeclaration(
                new ContractFamilyId("fsats.contract.provider-protection-command"),
                new ContractEndpoint(Guardian, ContractEndpointRole.Producer),
                new ContractEndpoint(Fsapma, ContractEndpointRole.Consumer),
                ContractTrafficContext.Protection,
                true,
                "FCR-0004", "FCR-0009"),

            new ContractFamilyDeclaration(
                new ContractFamilyId("fsats.contract.trading-safety-state-projection"),
                new ContractEndpoint(Trading, ContractEndpointRole.Producer),
                new ContractEndpoint(Guardian, ContractEndpointRole.Consumer),
                ContractTrafficContext.Evidence,
                true,
                "FCR-0006", "FCR-0009"),

            new ContractFamilyDeclaration(
                new ContractFamilyId("fsats.contract.provider-operational-status-projection"),
                new ContractEndpoint(Fsapma, ContractEndpointRole.Producer),
                new ContractEndpoint(Guardian, ContractEndpointRole.Consumer),
                ContractTrafficContext.Evidence,
                true,
                "FCR-0006", "FCR-0009")
        });
}
