using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Foundation.Evidence;
using Foundation.State;

namespace Foundation.Reconciliation;

public enum ReconciliationClassification
{
    Consistent,
    NewEmptyRoot,
    DuplicateIdentical,
    ConflictingDuplicate,
    StaleWrite,
    UncertainBeforeCommit,
    UncertainAfterCommit,
    StateAheadOfEvidence,
    EvidenceAheadOfState,
    AcceptedFactMissing,
    AcceptedFactWithoutDurableState,
    CurrentStateCorrupted,
    EvidenceJournalInvalid,
    TrustedStateReconstructed,
    ChallengeRequired,
    FailedClosed
}

public sealed record ReconciliationRequest(
    string StateNamespace,
    string SubjectId,
    FoundationStateClass StateClass,
    string RequestIdentity,
    string DecisionIdentity);

public sealed record ReconciliationResult(
    ReconciliationClassification Classification,
    string Reason,
    AuthoritativeStateRecord? State,
    IntegrityLinkedEvidenceRecord? Evidence,
    AcceptedFactEvent? AcceptedFact,
    bool ContinuationAllowed,
    bool ChallengeRequired);


public static class ReconciliationCanonicalEncoding
{
    private const char Separator = '\u001F';

    public static string RequestIdentity(ReconciliationRequest request)
    {
        var canonical = string.Join(Separator, request.StateNamespace, request.SubjectId,
            request.StateClass.ToString(), request.RequestIdentity, request.DecisionIdentity);
        return "reconciliation-request/sha256/" + Hash(canonical);
    }

    public static string SubjectId(ReconciliationRequest request)
        => "reconciliation/" + Hash(string.Join(Separator, request.StateNamespace,
            request.SubjectId, request.StateClass.ToString(), request.RequestIdentity,
            request.DecisionIdentity));

    public static string ResultIdentity(ReconciliationRequest request, ReconciliationResult result)
        => "reconciliation-result/sha256/" + Hash(string.Join(Separator,
            RequestIdentity(request), result.Classification.ToString(), result.Reason,
            result.State?.RecordDigest ?? string.Empty,
            result.Evidence?.EvidenceId ?? string.Empty,
            result.Evidence?.RecordDigest ?? string.Empty,
            result.AcceptedFact?.FactId ?? string.Empty,
            result.AcceptedFact?.FactDigest ?? string.Empty,
            result.ContinuationAllowed.ToString(CultureInfo.InvariantCulture),
            result.ChallengeRequired.ToString(CultureInfo.InvariantCulture)));

    public static string SerializeResult(ReconciliationRequest request, ReconciliationResult result)
        => JsonSerializer.Serialize(new
        {
            RequestIdentity = RequestIdentity(request),
            Request = request,
            ResultIdentity = ResultIdentity(request, result),
            Classification = result.Classification.ToString(),
            result.Reason,
            StateDigest = result.State?.RecordDigest ?? string.Empty,
            StateVersion = result.State?.StateVersion ?? -1,
            CommitIdentity = result.State is null ? string.Empty : StateCanonicalEncoding.CommitIdentity(result.State),
            EvidenceIdentity = result.Evidence?.EvidenceId ?? string.Empty,
            EvidenceDigest = result.Evidence?.RecordDigest ?? string.Empty,
            AcceptedFactIdentity = result.AcceptedFact?.FactId ?? string.Empty,
            result.ContinuationAllowed,
            result.ChallengeRequired
        });

    private static string Hash(string value)
        => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
}
