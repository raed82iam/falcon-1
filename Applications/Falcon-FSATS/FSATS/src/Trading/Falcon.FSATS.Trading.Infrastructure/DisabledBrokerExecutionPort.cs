using Falcon.FSATS.Trading.Application;
using Falcon.FSATS.Trading.Domain;

namespace Falcon.FSATS.Trading.Infrastructure;

public sealed class DisabledBrokerExecutionPort : IBrokerExecutionPort
{
    public ValueTask<BrokerSubmissionResult> SubmitAsync(OrderIntent intent, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(intent);
        return ValueTask.FromResult(new BrokerSubmissionResult(
            intent.ExecutionIdentity,
            false,
            false,
            "BROKER_EGRESS_NOT_AUTHORIZED"));
    }

    public ValueTask<BrokerOrderSnapshot> ReconcileAsync(BrokerExecutionIdentity identity, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(identity);
        return ValueTask.FromResult(new BrokerOrderSnapshot(
            identity,
            OrderState.ReconciliationRequired,
            new Quantity(0m),
            "BROKER_RECONCILIATION_BINDING_UNAVAILABLE"));
    }
}
