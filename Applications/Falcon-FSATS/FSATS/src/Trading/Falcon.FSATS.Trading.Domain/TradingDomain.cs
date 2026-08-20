namespace Falcon.FSATS.Trading.Domain;

public enum SemanticPresence { Present, Absent, Unknown, NotApplicable }
public enum BrokerCapabilityState { Supported, Unsupported, Conditional, Unknown }
public enum OrderState { Requested, SubmissionAttempted, BrokerAcknowledged, PartiallyFilled, Filled, CancelRequested, Cancelled, Rejected, ReconciliationRequired }
public enum RiskDecision { Approved, Reduced, Denied }

public readonly record struct Currency
{
    public string Code { get; }
    public Currency(string code)
    {
        ArgumentNullException.ThrowIfNull(code);
        var normalized = code.Trim().ToUpperInvariant();
        if (normalized.Length is < 3 or > 8) throw new ArgumentOutOfRangeException(nameof(code));
        Code = normalized;
    }
}

public readonly record struct Money(decimal Amount, Currency Currency);
public readonly record struct Price
{
    public decimal Amount { get; }
    public Currency QuoteCurrency { get; }
    public Price(decimal amount, Currency quoteCurrency)
    {
        if (amount < 0m) throw new ArgumentOutOfRangeException(nameof(amount));
        Amount = amount;
        QuoteCurrency = quoteCurrency;
    }
}
public readonly record struct Quantity
{
    public decimal Amount { get; }
    public Quantity(decimal amount)
    {
        if (amount < 0m) throw new ArgumentOutOfRangeException(nameof(amount));
        Amount = amount;
    }
}
public readonly record struct InstrumentId(string Value);
public readonly record struct PositionId(string Value);
public readonly record struct OrderId(string Value);
public readonly record struct ReservationId(string Value);
public readonly record struct TrustEpoch(long Value);

public sealed record BrokerAccountContext
{
    public string BrokerId { get; }
    public string BrokerAccountId { get; }
    public string Environment { get; }

    public BrokerAccountContext(string brokerId, string brokerAccountId, string environment)
    {
        BrokerId = Require(brokerId, nameof(brokerId)).ToUpperInvariant();
        BrokerAccountId = Require(brokerAccountId, nameof(brokerAccountId));
        Environment = Require(environment, nameof(environment)).ToUpperInvariant();
    }

    public string NamespaceKey => $"{Part(BrokerId)}|{Part(BrokerAccountId)}|{Part(Environment)}";

    private static string Require(string value, string parameter)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("BROKER_ACCOUNT_CONTEXT_REQUIRED", parameter);
        return value.Trim();
    }

    private static string Part(string value) => Uri.EscapeDataString(value);
}

public sealed record PositionSafetyEnvelope(
    PositionId PositionId,
    InstrumentId InstrumentId,
    Quantity Quantity,
    Money MaximumAuthorizedLoss,
    string ProtectionOwner,
    string ProtectionState,
    string EmergencyExitRule,
    string ReconciliationState,
    TrustEpoch LastTrustedRiskEpoch)
{
    public BrokerAccountContext? AccountContext { get; init; }
    public string ProtectionEvidenceReference { get; init; } = string.Empty;
}

public static class PositionSafetyEnvelopeBindingGuard
{
    public static bool IsBoundForRiskDecision(
        PositionSafetyEnvelope? envelope,
        BrokerAccountContext account,
        InstrumentId instrument,
        Quantity approvedQuantity,
        Money requiredMaximumLoss,
        TrustEpoch trustedEpoch)
        => IsStructurallyAndIdentityBound(envelope, account, instrument, approvedQuantity, trustedEpoch) &&
           envelope!.MaximumAuthorizedLoss.Currency == requiredMaximumLoss.Currency &&
           envelope.MaximumAuthorizedLoss.Amount >= requiredMaximumLoss.Amount;

    public static bool IsBoundForExecution(
        PositionSafetyEnvelope? envelope,
        BrokerAccountContext account,
        InstrumentId instrument,
        Quantity quantity,
        TrustEpoch trustEpoch)
        => IsStructurallyAndIdentityBound(envelope, account, instrument, quantity, trustEpoch);

    private static bool IsStructurallyAndIdentityBound(
        PositionSafetyEnvelope? envelope,
        BrokerAccountContext account,
        InstrumentId instrument,
        Quantity quantity,
        TrustEpoch trustEpoch)
        => envelope is not null &&
           account is not null &&
           envelope.AccountContext == account &&
           !string.IsNullOrWhiteSpace(envelope.PositionId.Value) &&
           !string.IsNullOrWhiteSpace(instrument.Value) &&
           !string.IsNullOrWhiteSpace(envelope.InstrumentId.Value) &&
           StringComparer.Ordinal.Equals(envelope.InstrumentId.Value.Trim(), instrument.Value.Trim()) &&
           quantity.Amount > 0m &&
           envelope.Quantity.Amount >= quantity.Amount &&
           envelope.LastTrustedRiskEpoch == trustEpoch &&
           envelope.MaximumAuthorizedLoss.Amount > 0m &&
           !string.IsNullOrWhiteSpace(envelope.MaximumAuthorizedLoss.Currency.Code) &&
           !string.IsNullOrWhiteSpace(envelope.ProtectionOwner) &&
           !string.IsNullOrWhiteSpace(envelope.ProtectionState) &&
           !string.IsNullOrWhiteSpace(envelope.EmergencyExitRule) &&
           !string.IsNullOrWhiteSpace(envelope.ReconciliationState) &&
           !string.IsNullOrWhiteSpace(envelope.ProtectionEvidenceReference);
}

public sealed record RiskRequest(InstrumentId Instrument, Quantity RequestedQuantity, Money WorstCredibleLoss, Money AvailableLossBudget, bool GuardianAllowsNewRisk, bool DataIsTrusted)
{
    public BrokerAccountContext? AccountContext { get; init; }
}

public sealed record RiskResult(RiskDecision Decision, Quantity ApprovedQuantity, string ReasonCode);

public static class UnifiedRiskGate
{
    public static RiskResult Evaluate(RiskRequest request)
    {
        if (!request.GuardianAllowsNewRisk) return Denied("GUARDIAN_RESTRICTED");
        if (!request.DataIsTrusted) return Denied("DATA_UNTRUSTED");
        if (request.WorstCredibleLoss.Currency != request.AvailableLossBudget.Currency) return Denied("CURRENCY_MISMATCH");
        if (request.WorstCredibleLoss.Amount <= 0m) return Denied("INVALID_LOSS_ESTIMATE");
        if (request.AvailableLossBudget.Amount < 0m) return Denied("INVALID_AVAILABLE_LOSS_BUDGET");
        if (request.WorstCredibleLoss.Amount <= request.AvailableLossBudget.Amount) return new(RiskDecision.Approved, request.RequestedQuantity, "WITHIN_BUDGET");

        try
        {
            var ratio = request.AvailableLossBudget.Amount / request.WorstCredibleLoss.Amount;
            var scaled = checked(request.RequestedQuantity.Amount * ratio);
            var reduced = decimal.Round(scaled, 8, MidpointRounding.ToZero);
            return reduced > 0m ? new(RiskDecision.Reduced, new Quantity(reduced), "RISK_REDUCED") : Denied("NO_SAFE_QUANTITY");
        }
        catch (OverflowException)
        {
            return Denied("RISK_SIZING_NUMERIC_OVERFLOW");
        }
    }

    private static RiskResult Denied(string reason) => new(RiskDecision.Denied, new Quantity(0m), reason);
}

public readonly record struct CapitalReservationKey(BrokerAccountContext Account, ReservationId ReservationId);

public sealed class CapitalReservationLedger
{
    private readonly object _gate = new();
    private readonly Dictionary<CapitalReservationKey, Money> _reservations = new();

    public bool TryReserve(BrokerAccountContext account, ReservationId id, Money amount, Money available)
    {
        ArgumentNullException.ThrowIfNull(account);
        if (string.IsNullOrWhiteSpace(id.Value) ||
            string.IsNullOrWhiteSpace(amount.Currency.Code) ||
            string.IsNullOrWhiteSpace(available.Currency.Code) ||
            amount.Amount <= 0m ||
            available.Amount < 0m ||
            amount.Currency != available.Currency)
        {
            return false;
        }

        var key = new CapitalReservationKey(account, id);
        lock (_gate)
        {
            if (_reservations.ContainsKey(key)) return false;

            decimal reservedForAccountAndCurrency = 0m;
            try
            {
                foreach (var reservation in _reservations)
                {
                    if (reservation.Key.Account == account && reservation.Value.Currency == amount.Currency)
                    {
                        reservedForAccountAndCurrency = checked(reservedForAccountAndCurrency + reservation.Value.Amount);
                    }
                }

                var aggregateAfterReservation = checked(reservedForAccountAndCurrency + amount.Amount);
                if (aggregateAfterReservation > available.Amount) return false;
            }
            catch (OverflowException)
            {
                return false;
            }

            _reservations.Add(key, amount);
            return true;
        }
    }

    public bool Release(BrokerAccountContext account, ReservationId id)
    {
        ArgumentNullException.ThrowIfNull(account);
        if (string.IsNullOrWhiteSpace(id.Value)) return false;
        lock (_gate)
        {
            return _reservations.Remove(new CapitalReservationKey(account, id));
        }
    }

    public IReadOnlyDictionary<CapitalReservationKey, Money> Snapshot()
    {
        lock (_gate)
        {
            return new Dictionary<CapitalReservationKey, Money>(_reservations);
        }
    }

    public IReadOnlyDictionary<CapitalReservationKey, Money> Snapshot(BrokerAccountContext account)
    {
        ArgumentNullException.ThrowIfNull(account);
        lock (_gate)
        {
            return _reservations
                .Where(x => x.Key.Account == account)
                .ToDictionary(x => x.Key, x => x.Value);
        }
    }
}

public sealed class OrderLifecycle
{
    public OrderState State { get; private set; } = OrderState.Requested;
    public void SubmissionAttempt() => Transition(OrderState.Requested, OrderState.SubmissionAttempted);
    public void BrokerAck() => Transition(OrderState.SubmissionAttempted, OrderState.BrokerAcknowledged);
    public void PartialFill() { if (State is not (OrderState.BrokerAcknowledged or OrderState.PartiallyFilled)) throw new InvalidOperationException("INVALID_PARTIAL_FILL"); State = OrderState.PartiallyFilled; }
    public void FullFill() { if (State is not (OrderState.BrokerAcknowledged or OrderState.PartiallyFilled)) throw new InvalidOperationException("INVALID_FULL_FILL"); State = OrderState.Filled; }
    public void CancelRequest() { if (State is not (OrderState.BrokerAcknowledged or OrderState.PartiallyFilled)) throw new InvalidOperationException("INVALID_CANCEL_REQUEST"); State = OrderState.CancelRequested; }
    public void Cancelled() => Transition(OrderState.CancelRequested, OrderState.Cancelled);
    public void MarkAmbiguous() => State = OrderState.ReconciliationRequired;
    private void Transition(OrderState expected, OrderState next) { if (State != expected) throw new InvalidOperationException($"INVALID_TRANSITION_{State}_TO_{next}"); State = next; }
}

public static class TrustEpochFence
{
    public static bool IsEligible(TrustEpoch workEpoch, TrustEpoch currentTrustedEpoch, bool riskIncreasing) => !riskIncreasing || workEpoch == currentTrustedEpoch;
}
