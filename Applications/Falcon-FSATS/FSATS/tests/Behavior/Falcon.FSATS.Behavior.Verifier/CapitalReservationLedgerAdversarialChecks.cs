using T = Falcon.FSATS.Trading.Domain;

internal static class CapitalReservationLedgerAdversarialChecks
{
    internal static void Run()
    {
        RejectAggregateOverReservationUnderConcurrency();
        RejectConcurrentDuplicateReservationIdentityInsideAccount();
        PreserveCurrencyIsolation();
        PreserveBrokerAccountIsolation();
        AllowSameReservationIdAcrossBrokerAccounts();
    }

    private static T.BrokerAccountContext Account(string id)
        => new("ALPACA", id, "PAPER");

    private static void RejectAggregateOverReservationUnderConcurrency()
    {
        var ledger = new T.CapitalReservationLedger();
        var account = Account("PA-ACCOUNT-A");
        var usd = new T.Currency("USD");
        var available = new T.Money(10m, usd);
        using var start = new ManualResetEventSlim(false);

        var first = Task.Run(() => { start.Wait(); return ledger.TryReserve(account, new T.ReservationId("aggregate-a"), new T.Money(8m, usd), available); });
        var second = Task.Run(() => { start.Wait(); return ledger.TryReserve(account, new T.ReservationId("aggregate-b"), new T.Money(8m, usd), available); });

        start.Set();
        Task.WaitAll(first, second);

        var accepted = (first.Result ? 1 : 0) + (second.Result ? 1 : 0);
        if (accepted != 1) throw new InvalidOperationException($"C-01_ACCOUNT_AGGREGATE_RESERVATION_CONCURRENCY_FAILED:{accepted}");

        var reserved = ledger.Snapshot(account).Values.Where(value => value.Currency == usd).Sum(value => value.Amount);
        if (reserved > available.Amount) throw new InvalidOperationException("C-01_ACCOUNT_OVER_RESERVATION_DETECTED");
    }

    private static void RejectConcurrentDuplicateReservationIdentityInsideAccount()
    {
        var ledger = new T.CapitalReservationLedger();
        var account = Account("PA-ACCOUNT-A");
        var usd = new T.Currency("USD");
        var available = new T.Money(10m, usd);
        var id = new T.ReservationId("same-id");
        using var start = new ManualResetEventSlim(false);

        var first = Task.Run(() => { start.Wait(); return ledger.TryReserve(account, id, new T.Money(4m, usd), available); });
        var second = Task.Run(() => { start.Wait(); return ledger.TryReserve(account, id, new T.Money(4m, usd), available); });
        start.Set();
        Task.WaitAll(first, second);

        var accepted = (first.Result ? 1 : 0) + (second.Result ? 1 : 0);
        if (accepted != 1 || ledger.Snapshot(account).Count != 1)
            throw new InvalidOperationException("C-01_DUPLICATE_RESERVATION_ID_INSIDE_ACCOUNT_FAILED");
    }

    private static void PreserveCurrencyIsolation()
    {
        var ledger = new T.CapitalReservationLedger();
        var account = Account("PA-ACCOUNT-A");
        var usd = new T.Currency("USD");
        var eur = new T.Currency("EUR");
        if (!ledger.TryReserve(account, new T.ReservationId("usd"), new T.Money(8m, usd), new T.Money(10m, usd)))
            throw new InvalidOperationException("C-01_USD_BASELINE_RESERVATION_FAILED");
        if (!ledger.TryReserve(account, new T.ReservationId("eur"), new T.Money(8m, eur), new T.Money(10m, eur)))
            throw new InvalidOperationException("C-01_CURRENCY_ISOLATION_FAILED");
    }

    private static void PreserveBrokerAccountIsolation()
    {
        var ledger = new T.CapitalReservationLedger();
        var first = Account("PA-ACCOUNT-A");
        var second = Account("PA-ACCOUNT-B");
        var usd = new T.Currency("USD");
        var available = new T.Money(10m, usd);

        if (!ledger.TryReserve(first, new T.ReservationId("r-a"), new T.Money(8m, usd), available))
            throw new InvalidOperationException("C-01_FIRST_BROKER_ACCOUNT_RESERVATION_FAILED");
        if (!ledger.TryReserve(second, new T.ReservationId("r-b"), new T.Money(8m, usd), available))
            throw new InvalidOperationException("C-01_SECOND_BROKER_ACCOUNT_POISONED_BY_FIRST");
    }

    private static void AllowSameReservationIdAcrossBrokerAccounts()
    {
        var ledger = new T.CapitalReservationLedger();
        var first = Account("PA-ACCOUNT-A");
        var second = Account("PA-ACCOUNT-B");
        var usd = new T.Currency("USD");
        var available = new T.Money(10m, usd);
        var sameId = new T.ReservationId("same-id-across-accounts");

        if (!ledger.TryReserve(first, sameId, new T.Money(4m, usd), available) ||
            !ledger.TryReserve(second, sameId, new T.Money(4m, usd), available))
            throw new InvalidOperationException("C-01_RESERVATION_ID_FALSELY_GLOBAL_ACROSS_ACCOUNTS");
    }
}
