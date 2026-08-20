using Falcon.FSATS.ResourceManagement.Domain;

namespace Falcon.FSATS.ResourceManagement.Application;

public sealed class ResourceCoordinationService
{
    private readonly ResourceStrategyController _controller;

    public ResourceCoordinationService(ResourceStrategyController controller)
        => _controller = controller ?? throw new ArgumentNullException(nameof(controller));

    public RedistributionDecision? TryInternalRedistribution(
        IReadOnlyCollection<ResourceClaim> claims,
        FoundationEnvelope envelope,
        CoordinationEpoch epoch,
        string targetApplication,
        string resourceClass,
        DateTimeOffset now)
        => _controller.Plan(claims, envelope, epoch, targetApplication, resourceClass, now);
}
