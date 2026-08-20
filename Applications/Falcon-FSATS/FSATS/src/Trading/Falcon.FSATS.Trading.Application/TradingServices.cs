using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Falcon.FSATS.Trading.Domain;

namespace Falcon.FSATS.Trading.Application;

public sealed record BrokerExecutionIdentity
{
    public BrokerAccountContext Account { get; }
    public string ExecutionRouteId { get; }
    public string BrokerRouteId => ExecutionRouteId;
    public string SubmissionId { get; }
    public OrderId OrderId { get; }

    public BrokerExecutionIdentity(BrokerAccountContext account, string executionRouteId, string submissionId, OrderId orderId)
    {
        Account = account ?? throw new ArgumentNullException(nameof(account));
        ExecutionRouteId = Require(executionRouteId, nameof(executionRouteId));
        SubmissionId = Require(submissionId, nameof(submissionId));
        if (string.IsNullOrWhiteSpace(orderId.Value)) throw new ArgumentException("ORDER_ID_REQUIRED", nameof(orderId));
        OrderId = new OrderId(orderId.Value.Trim());
    }

    public string NamespaceKey => $"{Account.NamespaceKey}|{Part(ExecutionRouteId)}|{Part(SubmissionId)}|{Part(OrderId.Value)}";

    private static string Require(string value, string parameter)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("EXECUTION_IDENTITY_REQUIRED", parameter);
        return value.Trim();
    }

    private static string Part(string value) => Uri.EscapeDataString(value);
}

public interface IBrokerExecutionPort
{
    ValueTask<BrokerSubmissionResult> SubmitAsync(OrderIntent intent, CancellationToken cancellationToken);
    ValueTask<BrokerOrderSnapshot> ReconcileAsync(BrokerExecutionIdentity identity, CancellationToken cancellationToken);
}

public sealed record OrderIntent(BrokerExecutionIdentity ExecutionIdentity, InstrumentId Instrument, Quantity Quantity, TrustEpoch TrustEpoch, PositionSafetyEnvelope SafetyEnvelope)
{
    public ReservationId? RiskReservationId { get; init; }
    public string DecisionBindingReference { get; init; } = string.Empty;
}
public sealed record BrokerSubmissionResult(BrokerExecutionIdentity ExecutionIdentity, bool Submitted, bool OutcomeKnown, string ReasonCode);
public sealed record BrokerOrderSnapshot(BrokerExecutionIdentity ExecutionIdentity, OrderState State, Quantity FilledQuantity, string ReasonCode);

internal static class TradingDecisionBinding
{
    public static string Compute(
        BrokerExecutionIdentity executionIdentity,
        ReservationId reservationId,
        BrokerAccountContext account,
        InstrumentId instrument,
        Quantity approvedQuantity,
        PositionSafetyEnvelope envelope,
        TrustEpoch trustedEpoch)
    {
        ArgumentNullException.ThrowIfNull(executionIdentity);
        ArgumentNullException.ThrowIfNull(account);
        ArgumentNullException.ThrowIfNull(envelope);
        if (string.IsNullOrWhiteSpace(reservationId.Value)) throw new ArgumentException("RESERVATION_ID_REQUIRED", nameof(reservationId));

        var payload = Pack(
            "FSATS.TRADING.DECISION_BINDING.v1",
            executionIdentity.NamespaceKey,
            reservationId.Value.Trim(),
            account.NamespaceKey,
            instrument.Value?.Trim() ?? string.Empty,
            approvedQuantity.Amount.ToString(CultureInfo.InvariantCulture),
            trustedEpoch.Value.ToString(CultureInfo.InvariantCulture),
            envelope.AccountContext?.NamespaceKey ?? string.Empty,
            envelope.PositionId.Value?.Trim() ?? string.Empty,
            envelope.InstrumentId.Value?.Trim() ?? string.Empty,
            envelope.Quantity.Amount.ToString(CultureInfo.InvariantCulture),
            envelope.MaximumAuthorizedLoss.Amount.ToString(CultureInfo.InvariantCulture),
            envelope.MaximumAuthorizedLoss.Currency.Code ?? string.Empty,
            envelope.ProtectionOwner?.Trim() ?? string.Empty,
            envelope.ProtectionState?.Trim() ?? string.Empty,
            envelope.EmergencyExitRule?.Trim() ?? string.Empty,
            envelope.ReconciliationState?.Trim() ?? string.Empty,
            envelope.LastTrustedRiskEpoch.Value.ToString(CultureInfo.InvariantCulture),
            envelope.ProtectionEvidenceReference?.Trim() ?? string.Empty);

        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(payload)));
    }

    public static bool Matches(OrderIntent intent)
    {
        if (intent is null ||
            intent.ExecutionIdentity is null ||
            intent.ExecutionIdentity.Account is null ||
            intent.SafetyEnvelope is null ||
            intent.RiskReservationId is not { } reservationId ||
            string.IsNullOrWhiteSpace(reservationId.Value) ||
            string.IsNullOrWhiteSpace(intent.DecisionBindingReference))
        {
            return false;
        }

        string expected;
        try
        {
            expected = Compute(
                intent.ExecutionIdentity,
                reservationId,
                intent.ExecutionIdentity.Account,
                intent.Instrument,
                intent.Quantity,
                intent.SafetyEnvelope,
                intent.TrustEpoch);
        }
        catch
        {
            return false;
        }

        return StringComparer.Ordinal.Equals(expected, intent.DecisionBindingReference.Trim().ToUpperInvariant());
    }

    private static string Pack(params string[] values)
    {
        var sb = new StringBuilder();
        foreach (var value in values)
        {
            var safe = value ?? string.Empty;
            sb.Append(safe.Length).Append(':').Append(safe).Append('|');
        }
        return sb.ToString();
    }
}

public sealed class TradingDecisionPipeline
{
    private readonly CapitalReservationLedger _reservations;
    public TradingDecisionPipeline(CapitalReservationLedger reservations) => _reservations = reservations;

    public DecisionPreparationResult Prepare(RiskRequest riskRequest, ReservationId reservationId, Money availableCapital, PositionSafetyEnvelope? safetyEnvelope, TrustEpoch workEpoch, TrustEpoch trustedEpoch, BrokerExecutionIdentity? executionIdentity = null)
    {
        if (riskRequest.AccountContext is null) return DecisionPreparationResult.Denied("BROKER_ACCOUNT_CONTEXT_REQUIRED");
        if (safetyEnvelope is null) return DecisionPreparationResult.Denied("MISSING_POSITION_SAFETY_ENVELOPE");
        if (executionIdentity is not null && executionIdentity.Account != riskRequest.AccountContext)
            return DecisionPreparationResult.Denied("EXECUTION_IDENTITY_ACCOUNT_MISMATCH");
        if (!TrustEpochFence.IsEligible(workEpoch, trustedEpoch, riskIncreasing: true)) return DecisionPreparationResult.Denied("STALE_TRUST_EPOCH");
        var risk = UnifiedRiskGate.Evaluate(riskRequest);
        if (risk.Decision == RiskDecision.Denied) return DecisionPreparationResult.Denied(risk.ReasonCode);
        if (!risk.TryWorstLossForReservation(riskRequest, out var worstLossForReservation))
            return DecisionPreparationResult.Denied("RESERVATION_SIZING_NUMERIC_OVERFLOW");
        var notionalReservation = new Money(worstLossForReservation, riskRequest.AvailableLossBudget.Currency);
        if (!PositionSafetyEnvelopeBindingGuard.IsBoundForRiskDecision(
                safetyEnvelope,
                riskRequest.AccountContext,
                riskRequest.Instrument,
                risk.ApprovedQuantity,
                notionalReservation,
                trustedEpoch))
            return DecisionPreparationResult.Denied("POSITION_SAFETY_ENVELOPE_BINDING_MISMATCH");
        if (!_reservations.TryReserve(riskRequest.AccountContext, reservationId, notionalReservation, availableCapital)) return DecisionPreparationResult.Denied("CAPITAL_RESERVATION_FAILED");

        var result = new DecisionPreparationResult(true, risk.ApprovedQuantity, reservationId, risk.ReasonCode);
        if (executionIdentity is not null)
        {
            result = result with
            {
                DecisionBindingReference = TradingDecisionBinding.Compute(
                    executionIdentity,
                    reservationId,
                    riskRequest.AccountContext,
                    riskRequest.Instrument,
                    risk.ApprovedQuantity,
                    safetyEnvelope,
                    trustedEpoch)
            };
        }

        return result;
    }
}

public sealed record DecisionPreparationResult(bool Allowed, Quantity ApprovedQuantity, ReservationId? ReservationId, string ReasonCode)
{
    public string DecisionBindingReference { get; init; } = string.Empty;
    public static DecisionPreparationResult Denied(string reason) => new(false, new Quantity(0m), null, reason);
}

internal static class RiskResultExtensions
{
    public static bool TryWorstLossForReservation(this RiskResult result, RiskRequest request, out decimal amount)
    {
        amount = 0m;
        if (request.RequestedQuantity.Amount == 0m) return result.ApprovedQuantity.Amount == 0m;
        try
        {
            var ratio = result.ApprovedQuantity.Amount / request.RequestedQuantity.Amount;
            amount = checked(request.WorstCredibleLoss.Amount * ratio);
            return amount >= 0m;
        }
        catch (OverflowException)
        {
            amount = 0m;
            return false;
        }
    }
}

public sealed class ExecutionCoordinator
{
    private readonly IBrokerExecutionPort _broker;
    private readonly AccountScopedExecutionQueue _queue;

    public ExecutionCoordinator(IBrokerExecutionPort broker, AccountScopedExecutionQueue queue)
    {
        _broker = broker ?? throw new ArgumentNullException(nameof(broker));
        _queue = queue ?? throw new ArgumentNullException(nameof(queue));
    }

    public async ValueTask<BrokerOrderSnapshot> SubmitOrReconcileAsync(OrderIntent intent, ExecutionDispatchPermit permit, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(intent);
        ArgumentNullException.ThrowIfNull(intent.ExecutionIdentity);
        ArgumentNullException.ThrowIfNull(permit);

        if (permit.Identity != intent.ExecutionIdentity)
            return new(intent.ExecutionIdentity, OrderState.Rejected, new Quantity(0m), "EXECUTION_DISPATCH_PERMIT_IDENTITY_MISMATCH");

        if (string.IsNullOrWhiteSpace(permit.DecisionBindingReference) ||
            !StringComparer.Ordinal.Equals(permit.DecisionBindingReference, intent.DecisionBindingReference))
            return new(intent.ExecutionIdentity, OrderState.Rejected, new Quantity(0m), "EXECUTION_DISPATCH_PERMIT_DECISION_BINDING_MISMATCH");

        if (!TradingDecisionBinding.Matches(intent))
            return new(intent.ExecutionIdentity, OrderState.Rejected, new Quantity(0m), "TRADING_DECISION_BINDING_MISSING_OR_MISMATCHED");

        if (!PositionSafetyEnvelopeBindingGuard.IsBoundForExecution(
                intent.SafetyEnvelope,
                intent.ExecutionIdentity.Account,
                intent.Instrument,
                intent.Quantity,
                intent.TrustEpoch))
            return new(intent.ExecutionIdentity, OrderState.Rejected, new Quantity(0m), "POSITION_SAFETY_ENVELOPE_EXECUTION_BINDING_MISMATCH");

        if (!_queue.TryCommitAndStartDispatch(
                permit,
                () => _broker.SubmitAsync(intent, cancellationToken),
                out var startedSubmission,
                out var synchronousStartFailure))
        {
            return new(intent.ExecutionIdentity, OrderState.Rejected, new Quantity(0m), "EXECUTION_DISPATCH_PERMIT_INVALID_OR_CONTAINED");
        }

        BrokerOrderSnapshot outcome;
        if (synchronousStartFailure is not null)
        {
            outcome = await ReconcileAndBindAsync(intent.ExecutionIdentity, $"SUBMISSION_START_EXCEPTION:{synchronousStartFailure.GetType().Name}").ConfigureAwait(false);
        }
        else
        {
            try
            {
                var result = await startedSubmission.ConfigureAwait(false);
                if (result is null || result.ExecutionIdentity != intent.ExecutionIdentity)
                    outcome = await ReconcileAndBindAsync(intent.ExecutionIdentity, "SUBMISSION_IDENTITY_MISMATCH").ConfigureAwait(false);
                else if (!result.Submitted || !result.OutcomeKnown)
                    outcome = await ReconcileAndBindAsync(intent.ExecutionIdentity, result.ReasonCode).ConfigureAwait(false);
                else
                    outcome = new(intent.ExecutionIdentity, OrderState.SubmissionAttempted, new Quantity(0m), result.ReasonCode);
            }
            catch (Exception ex)
            {
                outcome = await ReconcileAndBindAsync(intent.ExecutionIdentity, $"SUBMISSION_EXCEPTION:{ex.GetType().Name}").ConfigureAwait(false);
            }
        }

        if (!_queue.Complete(permit, outcome))
            return new(intent.ExecutionIdentity, OrderState.ReconciliationRequired, outcome.FilledQuantity, "CONTAINMENT_RACE_RECONCILIATION_REQUIRED");

        return outcome;
    }

    private async ValueTask<BrokerOrderSnapshot> ReconcileAndBindAsync(BrokerExecutionIdentity identity, string triggerReason)
    {
        try
        {
            var snapshot = await _broker.ReconcileAsync(identity, CancellationToken.None).ConfigureAwait(false);
            if (snapshot is null || snapshot.ExecutionIdentity != identity)
                return new(identity, OrderState.ReconciliationRequired, new Quantity(0m), "RECONCILIATION_IDENTITY_MISMATCH");
            return snapshot.State == OrderState.ReconciliationRequired && string.IsNullOrWhiteSpace(snapshot.ReasonCode)
                ? snapshot with { ReasonCode = triggerReason }
                : snapshot;
        }
        catch (Exception ex)
        {
            return new(identity, OrderState.ReconciliationRequired, new Quantity(0m), $"RECONCILIATION_UNAVAILABLE:{ex.GetType().Name}:{triggerReason}");
        }
    }
}
