using System;
using Foundation.Evidence;
using Foundation.State;

namespace Foundation.Reconciliation;

public sealed class RestartReconciler
{
    private const string ReconciliationNamespace = "foundation.reconciliation";
    private const string ReconciliationOwner = "Foundation.RestartReconciler";
    private readonly DurableAuthoritativeStateStore _state;
    private readonly DurableAuthoritativeStateStore _reconciliationState;
    private readonly IntegrityLinkedEvidenceJournal _evidence;
    private readonly AcceptedFactPublisher? _acceptedFactPublisher;
    private readonly ReconciliationClassifier _classifier;

    public RestartReconciler(
        DurableAuthoritativeStateStore state,
        IntegrityLinkedEvidenceJournal evidence,
        ReconciliationClassifier classifier,
        DurableAuthoritativeStateStore? reconciliationState = null,
        AcceptedFactPublisher? acceptedFactPublisher = null)
    {
        _state = state ?? throw new ArgumentNullException(nameof(state));
        _evidence = evidence ?? throw new ArgumentNullException(nameof(evidence));
        _acceptedFactPublisher = acceptedFactPublisher;
        _classifier = classifier ?? throw new ArgumentNullException(nameof(classifier));
        _reconciliationState = reconciliationState ?? state;
    }

    public bool MatchesStores(
        DurableAuthoritativeStateStore? state,
        IntegrityLinkedEvidenceJournal? evidence,
        AcceptedFactPublisher? acceptedFactPublisher)
        => ReferenceEquals(_state, state)
           && ReferenceEquals(_evidence, evidence)
           && ReferenceEquals(_acceptedFactPublisher, acceptedFactPublisher);

    public ReconciliationResult Reconcile(ReconciliationRequest request)
    {
        if (request is null) throw new ArgumentNullException(nameof(request));

        var state = _state.Read(request.StateNamespace, request.SubjectId, request.StateClass);
        var commit = !string.IsNullOrWhiteSpace(request.RequestIdentity)
            ? _state.LookupCommitByRequest(request.RequestIdentity)
            : _state.LookupCommitByDecision(request.DecisionIdentity);

        var identitySupplied = !string.IsNullOrWhiteSpace(request.RequestIdentity) ||
            !string.IsNullOrWhiteSpace(request.DecisionIdentity);
        if (identitySupplied && commit.Classification == DurableStateClassification.Missing)
            return Complete(request, Failed("COMMIT_RESULT_CHALLENGE_REQUIRED:" + commit.Reason));

        if (commit.Classification == DurableStateClassification.Conflicting ||
            commit.Classification == DurableStateClassification.Corrupted)
            return Complete(request, Failed(commit.Reason));

        if (commit.Classification == DurableStateClassification.UncertainBeforeCommit)
            return Complete(request, new ReconciliationResult(ReconciliationClassification.UncertainBeforeCommit,
                commit.Reason, null, null, null, false, true));

        var reconstructed = false;
        if (commit.Commit is not null &&
            (commit.Classification == DurableStateClassification.UncertainAfterCommit ||
             state.Classification is DurableStateClassification.Partial or
                 DurableStateClassification.Corrupted or
                 DurableStateClassification.Conflicting or
                 DurableStateClassification.Missing ||
             state.Current is null ||
             state.Current.StateVersion != commit.Commit.StateVersion ||
             !string.Equals(state.Current.RecordDigest, commit.Commit.StateDigest, StringComparison.Ordinal)))
        {
            var history = _state.ReadTrustedHistoryVersion(
                request.StateNamespace, request.SubjectId, request.StateClass,
                commit.Commit.StateVersion, commit.Commit.StateDigest, commit.Commit.CommitIdentity);

            if (history.Classification == DurableStateClassification.TrustedHistoryReconstructed &&
                history.Latest is not null)
            {
                state = new DurableStateReadResult(DurableStateClassification.Accepted,
                    "TRUSTED_STATE_RECONSTRUCTED", history.Latest);
                reconstructed = true;
            }
            else
            {
                return Complete(request, new ReconciliationResult(
                    commit.Classification == DurableStateClassification.UncertainAfterCommit
                        ? ReconciliationClassification.UncertainAfterCommit
                        : ReconciliationClassification.CurrentStateCorrupted,
                    history.Reason, null, null, null, false, true));
            }
        }

        var classified = _classifier.Classify(request, state, _evidence.Read(), _evidence.ReadAcceptedFacts());
        if (reconstructed)
        {
            classified = classified with
            {
                Classification = ReconciliationClassification.TrustedStateReconstructed,
                Reason = "TRUSTED_STATE_RECONSTRUCTED:" + classified.Reason
            };
        }

        return Complete(request, classified);
    }

    public static StateOwnershipDeclaration ReconciliationOwnership(ReconciliationRequest request)
        => new(
            "ownership/" + ReconciliationCanonicalEncoding.SubjectId(request),
            ReconciliationNamespace,
            ReconciliationCanonicalEncoding.SubjectId(request),
            FoundationStateClass.ReconciliationState,
            ReconciliationOwner,
            ReconciliationOwner,
            "Foundation.State",
            "Foundation.Reconciliation;Foundation.Infrastructure",
            ReconciliationOwner,
            "PERMANENT",
            1,
            DateTimeOffset.UnixEpoch,
            DateTimeOffset.MaxValue);

    private ReconciliationResult Complete(ReconciliationRequest request, ReconciliationResult result)
    {
        var record = new AuthoritativeStateRecord(
            ReconciliationCanonicalEncoding.ResultIdentity(request, result),
            ReconciliationNamespace,
            ReconciliationCanonicalEncoding.SubjectId(request),
            FoundationStateClass.ReconciliationState,
            StateRepresentationKind.Authoritative,
            ReconciliationOwner,
            ReconciliationOwner,
            "Foundation.State",
            ReconciliationOwner,
            ReconciliationCanonicalEncoding.RequestIdentity(request),
            0,
            DateTimeOffset.UnixEpoch,
            "PERMANENT",
            ReconciliationCanonicalEncoding.SerializeResult(request, result),
            string.Empty,
            string.Empty).WithComputedDigest();

        var persisted = _reconciliationState.Write(record, -1);
        if (persisted.Accepted) return result;

        return Failed("RECONCILIATION_STATE_PERSISTENCE_REJECTED:" + persisted.Reason);
    }

    private static ReconciliationResult Failed(string reason)
        => new(ReconciliationClassification.FailedClosed, reason, null, null, null, false, true);
}
