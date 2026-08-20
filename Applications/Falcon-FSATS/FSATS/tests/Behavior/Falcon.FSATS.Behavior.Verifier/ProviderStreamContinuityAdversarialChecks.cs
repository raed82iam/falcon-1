using P = Falcon.FSATS.FSAPMA.Domain;

internal static class ProviderStreamContinuityAdversarialChecks
{
    internal static void Run()
    {
        ReconnectNeverProvesGapFreeContinuity();
        SequenceGapFailsClosedUntilReconciled();
        DuplicateAndOutOfOrderAreNotFreshTruthPromotion();
        SequenceSpaceExhaustionFailsClosed();
        SessionIdentityRejectsDelimiterCollision();
        StaleConnectedStreamIsExplicit();
    }

    private static P.ProviderStreamContinuityTracker Tracker(string session = "session")
    {
        var route = new P.ProviderRouteIdentity(new P.ProviderId("ALPACA"), new P.ProviderAccountId("provider-account"), "PAPER", "MARKET_DATA_STREAM", new P.CredentialReference("credential-ref"));
        var identity = new P.ProviderStreamSessionIdentity(route, "ALPACA_US_EQUITIES_IEX", session, "AAPL:TRADES");
        return new P.ProviderStreamContinuityTracker(identity);
    }

    private static void ReconnectNeverProvesGapFreeContinuity()
    {
        var tracker = Tracker("reconnect");
        var now = DateTimeOffset.UtcNow;
        tracker.Connected(false, "connect", now.AddSeconds(-5));
        tracker.ObserveSequenced(100, now.AddSeconds(-4), now.AddSeconds(-4), "seq-100");
        var reconnect = tracker.Connected(true, "reconnect", now.AddSeconds(-1));
        var unsequenced = tracker.ObserveUnsequenced(now, now, "post-reconnect-event");
        if (reconnect.State != P.StreamContinuityState.ReconciliationRequired || unsequenced.State != P.StreamContinuityState.ReconciliationRequired)
            throw new InvalidOperationException("STREAM_RECONNECT_FALSELY_PROVED_CONTINUITY");
    }

    private static void SequenceGapFailsClosedUntilReconciled()
    {
        var tracker = Tracker("gap");
        var now = DateTimeOffset.UtcNow;
        tracker.Connected(false, "connect", now.AddSeconds(-5));
        tracker.ObserveSequenced(10, now.AddSeconds(-4), now.AddSeconds(-4), "10");
        var gap = tracker.ObserveSequenced(14, now.AddSeconds(-3), now.AddSeconds(-3), "14");
        var ignored = tracker.ObserveSequenced(15, now.AddSeconds(-2), now.AddSeconds(-2), "15");
        if (gap.State != P.StreamContinuityState.GapDetected || gap.MissingFromSequence != 11 || gap.MissingToSequence != 13 ||
            ignored.State != P.StreamContinuityState.GapDetected)
            throw new InvalidOperationException("STREAM_GAP_DID_NOT_FAIL_CLOSED");
        var reconciled = tracker.MarkReconciled(15, "snapshot-reconciliation", now.AddSeconds(-1));
        var next = tracker.ObserveSequenced(16, now, now, "16");
        if (reconciled.State != P.StreamContinuityState.Current || next.State != P.StreamContinuityState.Current)
            throw new InvalidOperationException("STREAM_RECONCILIATION_DID_NOT_RESTORE_CONTINUITY");
    }

    private static void DuplicateAndOutOfOrderAreNotFreshTruthPromotion()
    {
        var tracker = Tracker("order");
        var now = DateTimeOffset.UtcNow;
        tracker.Connected(false, "connect", now.AddSeconds(-4));
        tracker.ObserveSequenced(5, now.AddSeconds(-3), now.AddSeconds(-3), "5");
        var duplicate = tracker.ObserveSequenced(5, now.AddSeconds(-2), now.AddSeconds(-2), "dup");
        var outOfOrder = tracker.ObserveSequenced(4, now.AddSeconds(-1), now.AddSeconds(-1), "old");
        if (duplicate.State != P.StreamContinuityState.DuplicateObserved || outOfOrder.State != P.StreamContinuityState.ReconciliationRequired)
            throw new InvalidOperationException("STREAM_DUPLICATE_OR_OUT_OF_ORDER_PROMOTED_TO_CURRENT");
    }

    private static void SequenceSpaceExhaustionFailsClosed()
    {
        var tracker = Tracker("sequence-max");
        var now = DateTimeOffset.UtcNow;
        tracker.Connected(false, "connect", now.AddSeconds(-3));
        var max = tracker.ObserveSequenced(long.MaxValue, now.AddSeconds(-2), now.AddSeconds(-2), "max");
        var afterMax = tracker.ObserveSequenced(long.MaxValue, now.AddSeconds(-1), now.AddSeconds(-1), "dup-max");
        if (max.State != P.StreamContinuityState.Current || afterMax.State != P.StreamContinuityState.DuplicateObserved)
            throw new InvalidOperationException("STREAM_SEQUENCE_MAX_BASELINE_OR_DUPLICATE_INVALID");

        var reconciled = tracker.MarkReconciled(long.MaxValue, "reconcile-max", now);
        var impossibleAdvance = tracker.ObserveUnsequenced(now, now, "unsequenced-after-max");
        if (reconciled.State != P.StreamContinuityState.Current || impossibleAdvance.State != P.StreamContinuityState.Current)
            throw new InvalidOperationException("STREAM_SEQUENCE_MAX_RECONCILIATION_BASELINE_INVALID");

        var exhausted = tracker.ObserveSequenced(long.MaxValue, now, now, "max-again");
        if (exhausted.State != P.StreamContinuityState.DuplicateObserved)
            throw new InvalidOperationException("STREAM_SEQUENCE_MAX_DUPLICATE_NOT_PRESERVED");

        var tracker2 = Tracker("sequence-exhaustion-transition");
        tracker2.Connected(false, "connect", now.AddSeconds(-3));
        tracker2.ObserveSequenced(long.MaxValue - 1, now.AddSeconds(-2), now.AddSeconds(-2), "max-minus-one");
        var terminal = tracker2.ObserveSequenced(long.MaxValue, now.AddSeconds(-1), now.AddSeconds(-1), "max");
        var cannotAdvance = tracker2.ObserveSequenced(long.MaxValue, now, now, "max-duplicate");
        if (terminal.State != P.StreamContinuityState.Current || cannotAdvance.State != P.StreamContinuityState.DuplicateObserved)
            throw new InvalidOperationException("STREAM_SEQUENCE_EXHAUSTION_TRANSITION_INVALID");
    }

    private static void SessionIdentityRejectsDelimiterCollision()
    {
        var route = new P.ProviderRouteIdentity(new P.ProviderId("P"), new P.ProviderAccountId("A"), "PAPER", "MARKET_DATA_STREAM", new P.CredentialReference("C"));
        var a = new P.ProviderStreamSessionIdentity(route, "E|S", "X", "Y");
        var b = new P.ProviderStreamSessionIdentity(route, "E", "S|X", "Y");
        if (a == b || StringComparer.Ordinal.Equals(a.CanonicalKey, b.CanonicalKey))
            throw new InvalidOperationException("STREAM_SESSION_COMPOSITE_IDENTITY_COLLISION");
    }

    private static void StaleConnectedStreamIsExplicit()
    {
        var tracker = Tracker("stale");
        var now = DateTimeOffset.UtcNow;
        tracker.Connected(false, "connect", now.AddMinutes(-2));
        tracker.ObserveSequenced(1, now.AddMinutes(-2), now.AddMinutes(-2), "old-event");
        if (tracker.EvaluateFreshness(now, TimeSpan.FromSeconds(30)).State != P.StreamContinuityState.Stale)
            throw new InvalidOperationException("CONNECTED_BUT_STALE_STREAM_NOT_MARKED_STALE");
    }
}
