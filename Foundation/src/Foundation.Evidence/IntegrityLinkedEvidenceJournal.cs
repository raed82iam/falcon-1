using System;

namespace Foundation.Evidence;

public sealed class IntegrityLinkedEvidenceJournal
{
    private readonly FileEvidenceJournalProvider _provider;

    public IntegrityLinkedEvidenceJournal(FileEvidenceJournalProvider provider)
    {
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
    }

    public EvidenceAppendResult Append(EvidenceAppendRequest request)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var normalized = string.IsNullOrWhiteSpace(request.EvidenceId)
            ? request with
            {
                EvidenceId = EvidenceCanonicalEncoding.DeterministicEvidenceId(request)
            }
            : request;

        return _provider.Append(normalized);
    }

    public EvidenceAppendResult AppendCorrection(
        EvidenceAppendRequest correction,
        string correctedEvidenceId)
    {
        if (string.IsNullOrWhiteSpace(correctedEvidenceId))
        {
            return new EvidenceAppendResult(
                EvidenceJournalClassification.Malformed,
                "CORRECTION_TARGET_REQUIRED",
                null,
                false);
        }

        return Append(correction with
        {
            CorrectionOfEvidenceId = correctedEvidenceId
        });
    }

    public EvidenceJournalReadResult Read()
        => _provider.ReadJournal();

    public EvidenceCompletionBlockResult BlockEvidenceCompletion(
        string subjectId,
        string reason)
        => _provider.AppendEvidenceCompletionBlock(subjectId, reason);

    public System.Collections.Generic.IReadOnlyList<EvidenceCompletionBlock>
        ReadEvidenceCompletionBlocks()
        => _provider.ReadEvidenceCompletionBlocks();

    public System.Collections.Generic.IReadOnlyList<AcceptedFactEvent> ReadAcceptedFacts()
        => _provider.ReadAcceptedFacts();

    internal FileEvidenceJournalProvider Provider => _provider;
}
