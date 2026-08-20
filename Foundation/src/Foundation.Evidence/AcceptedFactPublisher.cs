using System;
using Foundation.State;

namespace Foundation.Evidence;

public sealed class AcceptedFactPublisher
{
    private readonly IntegrityLinkedEvidenceJournal _journal;

    public AcceptedFactPublisher(IntegrityLinkedEvidenceJournal journal)
    {
        _journal = journal ?? throw new ArgumentNullException(nameof(journal));
    }

    public AcceptedFactPublishResult Publish(
        EvidenceAppendResult evidence,
        DurableStateWriteResult persistence,
        string factKind)
    {
        if (!evidence.Accepted || evidence.Record is null)
        {
            return Reject("EVIDENCE_NOT_ACCEPTED");
        }

        if (!persistence.Accepted || persistence.Current is null)
        {
            return Reject("DURABLE_COMMIT_NOT_ACCEPTED");
        }

        var journal = _journal.Read();
        if (!journal.Accepted)
        {
            return Reject("EVIDENCE_JOURNAL_NOT_ACCEPTED:" + journal.Reason);
        }

        var committedEvidence = System.Linq.Enumerable.FirstOrDefault(
            journal.Records,
            record => string.Equals(
                record.EvidenceId,
                evidence.Record.EvidenceId,
                StringComparison.Ordinal));

        if (committedEvidence is null ||
            committedEvidence != evidence.Record)
        {
            return Reject("EVIDENCE_NOT_PRESENT_IN_ACCEPTED_JOURNAL");
        }

        if (evidence.Record.Decision != EvidenceDecisionKind.Allow ||
            evidence.Record.ExecutionOutcome != EvidenceExecutionOutcome.Accepted ||
            evidence.Record.PersistenceOutcome != EvidencePersistenceOutcome.Accepted)
        {
            return Reject("EVIDENCE_NOT_ELIGIBLE_FOR_ACCEPTED_FACT");
        }

        if (!string.Equals(
                evidence.Record.SubjectId,
                persistence.Current.SubjectId,
                StringComparison.Ordinal) ||
            evidence.Record.StateVersion != persistence.Current.StateVersion ||
            !string.Equals(
                evidence.Record.StateDigest,
                persistence.Current.RecordDigest,
                StringComparison.Ordinal))
        {
            return Reject("EVIDENCE_COMMIT_BINDING_MISMATCH");
        }

        var factId = EvidenceCanonicalEncoding.DeterministicFactId(
            evidence.Record.EvidenceId,
            factKind,
            persistence.Current.Namespace,
            persistence.Current.SubjectId,
            persistence.Current.StateVersion,
            persistence.Current.RecordDigest,
            persistence.CommitIdentity);

        var fact = new AcceptedFactEvent(
            factId,
            evidence.Record.EvidenceId,
            factKind,
            persistence.Current.Namespace,
            persistence.Current.SubjectId,
            persistence.Current.StateVersion,
            persistence.Current.RecordDigest,
            persistence.CommitIdentity,
            string.Empty);

        return _journal.Provider.AppendAcceptedFact(fact);
    }

    private static AcceptedFactPublishResult Reject(string reason)
        => new(
            EvidenceJournalClassification.Conflicting,
            reason,
            null,
            false);
}
