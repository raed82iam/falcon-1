using T = Falcon.FSATS.Trading.Domain;
using TA = Falcon.FSATS.Trading.Application;

internal static class BrokerRecoveryIdentityAdversarialChecks
{
    internal static void Run()
    {
        IncompleteReconciliationCannotBecomeRecovered();
        MismatchedAccountReconciliationCannotBecomeRecovered();
        BooleanOnlySummaryCannotBecomeRecovered();
        CompleteBrokerAccountReconciliationMayRecover();
        GuidedRequestRequiresExactIdentity();
    }

    private static T.BrokerAccountContext Account(string id) => new("ALPACA", id, "PAPER");

    private static TA.BrokerAccountObservation Observation(T.BrokerAccountContext account)
        => new(account, new T.PositionId("position-a"), new T.Quantity(1m), false, true,
            TA.BrokerAccountEvidenceSource.BrokerApiConfirmed, "evidence-a", DateTimeOffset.UtcNow);

    private static IReadOnlyList<TA.BrokerReconciliationDimensionEvidence> Dimensions(bool complete)
        => Enum.GetValues<TA.BrokerReconciliationDimension>()
            .Select(x => new TA.BrokerReconciliationDimensionEvidence(x, complete, $"dimension:{x}", DateTimeOffset.UtcNow))
            .ToArray();

    private static TA.BrokerAccountReconciliationEvidence Reconciliation(T.BrokerAccountContext account, bool complete, bool bindDimensions = true)
        => new(account, true, true, true, true, true, true, complete, "reconciliation-evidence", DateTimeOffset.UtcNow,
            bindDimensions ? Dimensions(complete) : null);

    private static void IncompleteReconciliationCannotBecomeRecovered()
    {
        var account = Account("PA-ACCOUNT-A");
        var assessment = TA.BrokerOutageRecoveryPolicy.Assess(
            TA.BrokerConnectivityState.Available,
            TA.BrokerSubmissionTruth.Reconciled,
            Observation(account),
            Reconciliation(account, complete: false));

        if (assessment.RecoveryState == T.OperationalRecoveryState.Recovered || assessment.MayResumeRiskIncreasingAction)
            throw new InvalidOperationException("INCOMPLETE_ACCOUNT_RECONCILIATION_PROMOTED_TO_RECOVERED");
    }

    private static void MismatchedAccountReconciliationCannotBecomeRecovered()
    {
        var accountA = Account("PA-ACCOUNT-A");
        var accountB = Account("PA-ACCOUNT-B");
        var assessment = TA.BrokerOutageRecoveryPolicy.Assess(
            TA.BrokerConnectivityState.Available,
            TA.BrokerSubmissionTruth.Reconciled,
            Observation(accountA),
            Reconciliation(accountB, complete: true));

        if (assessment.RecoveryState == T.OperationalRecoveryState.Recovered || assessment.MayResumeRiskIncreasingAction)
            throw new InvalidOperationException("WRONG_BROKER_ACCOUNT_RECONCILIATION_ACCEPTED");
    }

    private static void BooleanOnlySummaryCannotBecomeRecovered()
    {
        var account = Account("PA-ACCOUNT-A");
        var assessment = TA.BrokerOutageRecoveryPolicy.Assess(
            TA.BrokerConnectivityState.Available,
            TA.BrokerSubmissionTruth.Reconciled,
            Observation(account),
            Reconciliation(account, complete: true, bindDimensions: false));

        if (assessment.RecoveryState == T.OperationalRecoveryState.Recovered || assessment.MayResumeRiskIncreasingAction)
            throw new InvalidOperationException("BOOLEAN_ONLY_RECONCILIATION_SUMMARY_ACCEPTED_AS_PROOF");
    }

    private static void CompleteBrokerAccountReconciliationMayRecover()
    {
        var account = Account("PA-ACCOUNT-A");
        var assessment = TA.BrokerOutageRecoveryPolicy.Assess(
            TA.BrokerConnectivityState.Available,
            TA.BrokerSubmissionTruth.Reconciled,
            Observation(account),
            Reconciliation(account, complete: true));

        if (assessment.RecoveryState != T.OperationalRecoveryState.Recovered || !assessment.MayResumeRiskIncreasingAction || assessment.TruthState != T.OperationalTruthState.BrokerConfirmed)
            throw new InvalidOperationException("COMPLETE_BROKER_ACCOUNT_RECONCILIATION_NOT_RECOGNIZED");
    }

    private static void GuidedRequestRequiresExactIdentity()
    {
        try
        {
            _ = TA.BrokerOutageRecoveryPolicy.RequestPositionOpenFact(
                "request-a", Account("PA-ACCOUNT-A"), new T.PositionId(""), "corr-a");
            throw new InvalidOperationException("EMPTY_POSITION_ID_ACCEPTED_FOR_GUIDED_RECOVERY");
        }
        catch (ArgumentException)
        {
        }
    }
}
