namespace Falcon.FSATS.FSTSimA.Domain;

public readonly record struct ScenarioId(string Value);
public readonly record struct SimulationInstant(long Ticks);
public sealed record SyntheticTick(SimulationInstant Time, decimal Price, decimal Volume, string Regime);
public sealed record SimulatedOrder(string OrderId, decimal Quantity, decimal LimitPrice);
public sealed record SimulatedFill(string OrderId, decimal FilledQuantity, decimal FillPrice, bool Partial, string ReasonCode);

public sealed class SimulationClock
{
    public SimulationInstant Now { get; private set; }
    public SimulationClock(SimulationInstant start)
    {
        if (start.Ticks < 0) throw new ArgumentOutOfRangeException(nameof(start));
        Now = start;
    }
    public SimulationInstant Advance(long ticks)
    {
        if (ticks < 0) throw new ArgumentOutOfRangeException(nameof(ticks));
        checked { Now = new SimulationInstant(Now.Ticks + ticks); }
        return Now;
    }
}

public struct DeterministicPrng
{
    private ulong _state;
    public DeterministicPrng(int seed) => _state = seed == 0 ? 0x9E3779B97F4A7C15UL : unchecked((ulong)(uint)seed);
    public ulong NextUInt64()
    {
        var x = _state;
        x ^= x << 13;
        x ^= x >> 7;
        x ^= x << 17;
        _state = x;
        return x;
    }
    public decimal NextUnit() => (NextUInt64() % 1_000_000UL) / 1_000_000m;
}

public sealed class SyntheticMarketGenerator
{
    public IReadOnlyList<SyntheticTick> Generate(int seed, int count, decimal startPrice, SimulationInstant start, string regime)
    {
        if (count <= 0) throw new ArgumentOutOfRangeException(nameof(count));
        if (startPrice <= 0m) throw new ArgumentOutOfRangeException(nameof(startPrice));
        if (start.Ticks < 0) throw new ArgumentOutOfRangeException(nameof(start));
        if (string.IsNullOrWhiteSpace(regime)) throw new ArgumentException("SIMULATION_REGIME_REQUIRED", nameof(regime));
        var prng = new DeterministicPrng(seed);
        var result = new List<SyntheticTick>(count);
        var price = startPrice;
        for (var i = 0; i < count; i++)
        {
            var centered = prng.NextUnit() - 0.5m;
            var shock = regime == "STRESS" ? centered * 0.06m : centered * 0.01m;
            price = Math.Max(0.00000001m, decimal.Round(price * (1m + shock), 8, MidpointRounding.ToEven));
            var volume = 1m + decimal.Round(prng.NextUnit() * (regime == "STRESS" ? 1000m : 100m), 4, MidpointRounding.ToEven);
            result.Add(new SyntheticTick(new SimulationInstant(checked(start.Ticks + i)), price, volume, regime));
        }
        return result;
    }
}

public sealed class BrokerSimulator
{
    public SimulatedFill Execute(SimulatedOrder order, decimal marketPrice, decimal availableQuantity)
    {
        ArgumentNullException.ThrowIfNull(order);
        if (string.IsNullOrWhiteSpace(order.OrderId)) throw new ArgumentException("SIMULATED_ORDER_ID_REQUIRED", nameof(order));
        if (order.Quantity < 0m) throw new ArgumentOutOfRangeException(nameof(order), "SIMULATED_ORDER_QUANTITY_NEGATIVE");
        if (order.LimitPrice < 0m) throw new ArgumentOutOfRangeException(nameof(order), "SIMULATED_ORDER_LIMIT_PRICE_NEGATIVE");
        if (marketPrice < 0m) throw new ArgumentOutOfRangeException(nameof(marketPrice));
        if (availableQuantity < 0m) throw new ArgumentOutOfRangeException(nameof(availableQuantity));
        if (order.Quantity == 0m || availableQuantity == 0m) return new(order.OrderId, 0m, marketPrice, false, "NO_LIQUIDITY");
        if (order.LimitPrice < marketPrice) return new(order.OrderId, 0m, marketPrice, false, "LIMIT_NOT_MARKETABLE");
        var filled = Math.Min(order.Quantity, availableQuantity);
        return new(order.OrderId, filled, marketPrice, filled < order.Quantity, filled < order.Quantity ? "PARTIAL_FILL" : "FULL_FILL");
    }
}

public enum FaultType { ProviderDelay, ProviderOutage, BrokerAmbiguity, NetworkPartition, AiKill, ResourcePressure, EvidenceCorruptionChallenge }
public sealed record FaultEvent(FaultType Type, SimulationInstant At, string Target, string Parameters);

public sealed class FaultInjector
{
    public IReadOnlyList<FaultEvent> Order(IEnumerable<FaultEvent> faults)
    {
        ArgumentNullException.ThrowIfNull(faults);
        return faults
            .OrderBy(x => x.At.Ticks)
            .ThenBy(x => x.Type)
            .ThenBy(x => x.Target ?? string.Empty, StringComparer.Ordinal)
            .ThenBy(x => x.Parameters ?? string.Empty, StringComparer.Ordinal)
            .ToArray();
    }
}

public sealed record CalibrationResult(decimal Error, decimal Parameter, string EvidenceId);
public sealed class CalibrationEngine
{
    public CalibrationResult Calibrate(decimal simulated, decimal observed, decimal currentParameter, string evidenceId)
    {
        if (simulated < 0m) throw new ArgumentOutOfRangeException(nameof(simulated));
        if (observed < 0m) throw new ArgumentOutOfRangeException(nameof(observed));
        if (string.IsNullOrWhiteSpace(evidenceId)) throw new ArgumentException("CALIBRATION_EVIDENCE_ID_REQUIRED", nameof(evidenceId));
        try
        {
            var error = checked(observed - simulated);
            var proposed = decimal.Round(checked(currentParameter + checked(error * 0.1m)), 8, MidpointRounding.ToEven);
            return new(Math.Abs(error), proposed, evidenceId.Trim());
        }
        catch (OverflowException)
        {
            throw new ArgumentOutOfRangeException(nameof(observed), "CALIBRATION_NUMERIC_OVERFLOW");
        }
    }
}

public sealed record ValidationAssessment(bool Reproducible, bool CalibrationIndependent, decimal FidelityScore, string Recommendation);
public sealed class ValidationAssessor
{
    public ValidationAssessment Assess(bool sameSeedMatches, bool calibrationEvidenceExternalToAssessor, decimal fidelityScore)
    {
        if (fidelityScore is < 0m or > 1m)
            return new(sameSeedMatches, calibrationEvidenceExternalToAssessor, fidelityScore, "NOT_READY");
        var ready = sameSeedMatches && calibrationEvidenceExternalToAssessor && fidelityScore >= 0.80m;
        return new(sameSeedMatches, calibrationEvidenceExternalToAssessor, fidelityScore, ready ? "READY_FOR_PAPER_QUALIFICATION_REVIEW" : "NOT_READY");
    }
}
