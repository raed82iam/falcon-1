using System;
using System.Linq;
using Foundation.Evidence;
using Foundation.State;

namespace Foundation.Reconciliation;

public sealed class ReconciliationClassifier
{
    public ReconciliationResult Classify(
        ReconciliationRequest request,
        DurableStateReadResult state,
        EvidenceJournalReadResult journal,
        System.Collections.Generic.IReadOnlyList<AcceptedFactEvent> facts)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));
        if (state is null) throw new ArgumentNullException(nameof(state));
        if (journal is null) throw new ArgumentNullException(nameof(journal));
        if (facts is null) throw new ArgumentNullException(nameof(facts));

        if (state.Classification is DurableStateClassification.Corrupted or
            DurableStateClassification.Partial or
            DurableStateClassification.Conflicting)
        {
            return Result(ReconciliationClassification.CurrentStateCorrupted,
                state.Reason, state.Current, null, null, false, true);
        }

        if (journal.Classification is not EvidenceJournalClassification.Accepted and
            not EvidenceJournalClassification.Missing)
        {
            return Result(ReconciliationClassification.EvidenceJournalInvalid,
                journal.Reason, state.Current, null, null, false, true);
        }

        var relevantEvidence = journal.Records
            .Where(record =>
                string.Equals(record.StateNamespace, request.StateNamespace, StringComparison.Ordinal) &&
                string.Equals(record.SubjectId, request.SubjectId, StringComparison.Ordinal))
            .OrderByDescending(record => record.Sequence)
            .ToArray();

        var relevantFacts = facts
            .Where(fact =>
                string.Equals(fact.StateNamespace, request.StateNamespace, StringComparison.Ordinal) &&
                string.Equals(fact.SubjectId, request.SubjectId, StringComparison.Ordinal))
            .ToArray();

        if (state.Classification == DurableStateClassification.Missing)
        {
            if (relevantFacts.Length > 0)
            {
                return Result(ReconciliationClassification.AcceptedFactWithoutDurableState,
                    "ACCEPTED_FACT_WITHOUT_DURABLE_STATE", null, relevantEvidence.FirstOrDefault(), relevantFacts[0], false, true);
            }

            if (relevantEvidence.Any(record => record.PersistenceOutcome == EvidencePersistenceOutcome.Accepted))
            {
                return Result(ReconciliationClassification.EvidenceAheadOfState,
                    "EVIDENCE_AHEAD_OF_STATE", null, relevantEvidence.First(), null, false, true);
            }

            return Result(ReconciliationClassification.NewEmptyRoot,
                "GENUINELY_NEW_EMPTY_ROOT", null, null, null, true, false);
        }

        var current = state.Current!;
        var matchingEvidence = relevantEvidence.FirstOrDefault(record =>
            record.StateVersion == current.StateVersion &&
            string.Equals(record.StateDigest, current.RecordDigest, StringComparison.Ordinal) &&
            record.PersistenceOutcome == EvidencePersistenceOutcome.Accepted &&
            (string.IsNullOrWhiteSpace(request.RequestIdentity) ||
             string.Equals(record.RequestIdentity, request.RequestIdentity, StringComparison.Ordinal)) &&
            (string.IsNullOrWhiteSpace(request.DecisionIdentity) ||
             string.Equals(record.DecisionIdentity, request.DecisionIdentity, StringComparison.Ordinal)));

        if (matchingEvidence is null)
        {
            return Result(ReconciliationClassification.StateAheadOfEvidence,
                "STATE_AHEAD_OF_EVIDENCE", current, null, null, false, true);
        }

        var matchingFact = relevantFacts.FirstOrDefault(fact =>
            string.Equals(fact.EvidenceId, matchingEvidence.EvidenceId, StringComparison.Ordinal) &&
            fact.StateVersion == current.StateVersion &&
            string.Equals(fact.StateDigest, current.RecordDigest, StringComparison.Ordinal));

        if (matchingFact is null)
        {
            return Result(ReconciliationClassification.AcceptedFactMissing,
                "ACCEPTED_FACT_MISSING", current, matchingEvidence, null, false, true);
        }

        return Result(ReconciliationClassification.Consistent,
            "RECONCILIATION_CONSISTENT", current, matchingEvidence, matchingFact, true, false);
    }

    private static ReconciliationResult Result(
        ReconciliationClassification classification,
        string reason,
        AuthoritativeStateRecord? state,
        IntegrityLinkedEvidenceRecord? evidence,
        AcceptedFactEvent? fact,
        bool continuationAllowed,
        bool challengeRequired)
        => new(classification, reason, state, evidence, fact, continuationAllowed, challengeRequired);
}
