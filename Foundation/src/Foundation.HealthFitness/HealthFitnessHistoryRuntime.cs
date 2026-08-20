using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Foundation.HealthFitness;

public enum HealthFitnessFactKind
{
    HealthStateChanged = 1,
    FitnessChanged = 2
}

public enum HealthFitnessEventTruthClassification
{
    AuthoritativeOperational = 1,
    Replay = 2,
    Test = 3,
    Simulation = 4,
    NonAuthoritativeEvidence = 5
}

public enum HealthFitnessEventRelationKind
{
    None = 1,
    ReplayOf = 2,
    CorrectionOf = 3,
    Supersedes = 4
}

public sealed record HealthFitnessChangeFact(
    string EventId,
    HealthFitnessFactKind FactKind,
    string SubjectId,
    string Capability,
    string Scope,
    string PreviousAssessmentIdentity,
    string CurrentAssessmentIdentity,
    string SelfModelReference,
    HealthFitnessEventTruthClassification Classification,
    HealthFitnessEventRelationKind RelationKind,
    string RelatedEventId,
    string EventType,
    string SchemaId,
    string SchemaVersion,
    string Owner,
    string Provenance,
    DateTimeOffset ObservedAt,
    DateTimeOffset EffectiveAt)
{
    public string Identity => HealthFitnessHistoryIdentity.ComputeFact(this);
}

public sealed record PersistedHealthFitnessBasis(
    HealthFitnessChangeFact Fact,
    CanonicalHealthFitnessAssessment Assessment,
    string FactIdentity,
    string AssessmentIdentity);

public sealed record HealthFitnessHistoryRecord(
    string RecordId,
    string Namespace,
    string SubjectId,
    string Representation,
    string AuthoritativeOwner,
    string AuthoritativeSource,
    string PersistenceOwner,
    string WriterAuthority,
    string SourceIdentity,
    long StateVersion,
    DateTimeOffset EffectiveTime,
    string RetentionClassification,
    string Payload,
    string PreviousRecordDigest,
    string RecordDigest)
{
    public HealthFitnessHistoryRecord WithComputedDigest()
        => this with { RecordDigest = HealthFitnessHistoryIdentity.ComputeRecord(this) };
}

public sealed record HealthFitnessReconstructionResult(
    bool Trusted,
    string Reason,
    CanonicalHealthFitnessAssessment? Assessment,
    HealthFitnessChangeFact? Fact);

public static class HealthFitnessHistoryRuntime
{
    public const string HistoryNamespace = "foundation.health-fitness.history";
    public const string HistoryOwner = "Foundation.HealthFitness";
    public const string HistorySource = "Foundation.HealthFitness";
    public const string PersistenceOwner = "Foundation.State";
    public const string EventSubstrateOwner = "Foundation.EventSystem";
    public const string WriterAuthority = "stage7-wp07-health-history";
    public const string RetentionClassification = "governed-health-fitness-history";
    public const string HealthEventType = "foundation.health-state-changed";
    public const string FitnessEventType = "foundation.fitness-changed";
    public const string SchemaId = "foundation.health-fitness-change";
    public const string SchemaVersion = "1.0";

    public static HealthFitnessChangeFact CreateChangeFact(
        string eventId,
        HealthFitnessFactKind factKind,
        CanonicalHealthFitnessAssessment previous,
        CanonicalHealthFitnessAssessment current,
        HealthFitnessEventTruthClassification classification,
        HealthFitnessEventRelationKind relationKind,
        string? relatedEventId,
        string provenance)
    {
        ArgumentNullException.ThrowIfNull(previous);
        ArgumentNullException.ThrowIfNull(current);

        if (!Enum.IsDefined(factKind) || !Enum.IsDefined(classification) || !Enum.IsDefined(relationKind))
            throw new ArgumentException("WP07 enum rejected");
        if (string.IsNullOrWhiteSpace(eventId) || string.IsNullOrWhiteSpace(provenance))
            throw new ArgumentException("WP07 event identity/provenance missing");
        if (!string.Equals(previous.SubjectId, current.SubjectId, StringComparison.Ordinal) ||
            !string.Equals(previous.Capability, current.Capability, StringComparison.Ordinal) ||
            !string.Equals(previous.Scope, current.Scope, StringComparison.Ordinal))
            throw new InvalidOperationException("WP07 change fact cross-scope mismatch");
        if (current.AssessmentTime < previous.AssessmentTime || current.EffectiveTime < previous.EffectiveTime)
            throw new InvalidOperationException("WP07 time regression rejected");

        var changed = factKind switch
        {
            HealthFitnessFactKind.HealthStateChanged => previous.HealthState != current.HealthState,
            HealthFitnessFactKind.FitnessChanged => previous.TechnicalFitnessState != current.TechnicalFitnessState || previous.FitnessResult != current.FitnessResult,
            _ => false
        };
        if (!changed)
            throw new InvalidOperationException("WP07 material change fact requires an actual change");

        if (classification == HealthFitnessEventTruthClassification.Replay)
        {
            if (relationKind != HealthFitnessEventRelationKind.ReplayOf || string.IsNullOrWhiteSpace(relatedEventId))
                throw new InvalidOperationException("WP07 replay must remain explicitly related to original event");
        }
        else if (relationKind == HealthFitnessEventRelationKind.ReplayOf)
        {
            throw new InvalidOperationException("WP07 non-replay event cannot claim ReplayOf");
        }

        if (relationKind is HealthFitnessEventRelationKind.CorrectionOf or HealthFitnessEventRelationKind.Supersedes)
        {
            if (string.IsNullOrWhiteSpace(relatedEventId) || string.Equals(eventId, relatedEventId, StringComparison.Ordinal))
                throw new InvalidOperationException("WP07 correction/supersession requires a distinct related event");
        }
        else if (relationKind == HealthFitnessEventRelationKind.None && !string.IsNullOrWhiteSpace(relatedEventId))
        {
            throw new InvalidOperationException("WP07 unrelated event cannot carry related-event identity");
        }

        var eventType = factKind == HealthFitnessFactKind.HealthStateChanged ? HealthEventType : FitnessEventType;
        return new HealthFitnessChangeFact(eventId, factKind, current.SubjectId, current.Capability, current.Scope,
            previous.Identity, current.Identity, current.SelfModelReference, classification, relationKind,
            relatedEventId ?? string.Empty, eventType, SchemaId, SchemaVersion, HistoryOwner, provenance,
            current.ObservationTime, current.EffectiveTime);
    }

    public static HealthFitnessHistoryRecord CreateHistoryRecord(
        HealthFitnessChangeFact fact,
        CanonicalHealthFitnessAssessment assessment,
        long stateVersion,
        string previousRecordDigest)
    {
        ArgumentNullException.ThrowIfNull(fact);
        ArgumentNullException.ThrowIfNull(assessment);
        if (stateVersion < 0) throw new ArgumentOutOfRangeException(nameof(stateVersion));
        if (!string.Equals(fact.CurrentAssessmentIdentity, assessment.Identity, StringComparison.Ordinal))
            throw new InvalidOperationException("WP07 fact/assessment identity binding mismatch");
        if (!string.Equals(fact.SubjectId, assessment.SubjectId, StringComparison.Ordinal) ||
            !string.Equals(fact.Capability, assessment.Capability, StringComparison.Ordinal) ||
            !string.Equals(fact.Scope, assessment.Scope, StringComparison.Ordinal))
            throw new InvalidOperationException("WP07 fact/assessment scope binding mismatch");

        var payload = JsonSerializer.Serialize(new PersistedHealthFitnessBasis(fact, assessment, fact.Identity, assessment.Identity));
        return new HealthFitnessHistoryRecord(
            "health-history-" + fact.EventId, HistoryNamespace, assessment.SubjectId, "AUTHORITATIVE_EVIDENCE",
            HistoryOwner, HistorySource, PersistenceOwner, WriterAuthority, fact.Identity, stateVersion,
            fact.EffectiveAt, RetentionClassification, payload, previousRecordDigest ?? string.Empty, string.Empty)
            .WithComputedDigest();
    }

    public static HealthFitnessReconstructionResult Reconstruct(
        HealthFitnessHistoryRecord? record,
        bool loggingEvidenceAvailable,
        bool persistenceEvidenceAvailable)
    {
        if (!loggingEvidenceAvailable) return new(false, "LOGGING_EVIDENCE_UNAVAILABLE", null, null);
        if (!persistenceEvidenceAvailable) return new(false, "PERSISTENCE_EVIDENCE_UNAVAILABLE", null, null);
        if (record is null) return new(false, "HISTORY_RECORD_MISSING", null, null);
        if (!string.Equals(record.Namespace, HistoryNamespace, StringComparison.Ordinal) ||
            !string.Equals(record.Representation, "AUTHORITATIVE_EVIDENCE", StringComparison.Ordinal) ||
            !string.Equals(record.AuthoritativeOwner, HistoryOwner, StringComparison.Ordinal) ||
            !string.Equals(record.AuthoritativeSource, HistorySource, StringComparison.Ordinal) ||
            !string.Equals(record.PersistenceOwner, PersistenceOwner, StringComparison.Ordinal) ||
            !string.Equals(record.WriterAuthority, WriterAuthority, StringComparison.Ordinal))
            return new(false, "HISTORY_OWNERSHIP_BINDING_INVALID", null, null);
        if (!string.Equals(record.WithComputedDigest().RecordDigest, record.RecordDigest, StringComparison.Ordinal))
            return new(false, "HISTORY_RECORD_DIGEST_INVALID", null, null);

        PersistedHealthFitnessBasis? payload;
        try { payload = JsonSerializer.Deserialize<PersistedHealthFitnessBasis>(record.Payload); }
        catch (JsonException) { return new(false, "HISTORY_PAYLOAD_INVALID", null, null); }
        if (payload is null || payload.Fact is null || payload.Assessment is null)
            return new(false, "HISTORY_PAYLOAD_MISSING", null, null);
        if (!string.Equals(payload.FactIdentity, payload.Fact.Identity, StringComparison.Ordinal) ||
            !string.Equals(payload.AssessmentIdentity, payload.Assessment.Identity, StringComparison.Ordinal) ||
            !string.Equals(payload.Fact.CurrentAssessmentIdentity, payload.Assessment.Identity, StringComparison.Ordinal) ||
            !string.Equals(record.SourceIdentity, payload.Fact.Identity, StringComparison.Ordinal) ||
            !string.Equals(record.SubjectId, payload.Assessment.SubjectId, StringComparison.Ordinal))
            return new(false, "HISTORY_IDENTITY_BINDING_INVALID", null, null);
        if (payload.Fact.Classification == HealthFitnessEventTruthClassification.Replay && payload.Fact.RelationKind != HealthFitnessEventRelationKind.ReplayOf)
            return new(false, "HISTORY_REPLAY_BINDING_INVALID", null, null);
        return new(true, "HISTORY_RECONSTRUCTION_TRUSTED", payload.Assessment, payload.Fact);
    }
}

public static class HealthFitnessHistoryIdentity
{
    public static string ComputeFact(HealthFitnessChangeFact value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return Hash(string.Join("\u001F", value.EventId, value.FactKind, value.SubjectId, value.Capability, value.Scope,
            value.PreviousAssessmentIdentity, value.CurrentAssessmentIdentity, value.SelfModelReference, value.Classification,
            value.RelationKind, value.RelatedEventId, value.EventType, value.SchemaId, value.SchemaVersion, value.Owner,
            value.Provenance, value.ObservedAt.ToUniversalTime().ToString("O"), value.EffectiveAt.ToUniversalTime().ToString("O")));
    }

    public static string ComputeRecord(HealthFitnessHistoryRecord value)
        => Hash(string.Join("\u001F", value.RecordId, value.Namespace, value.SubjectId, value.Representation,
            value.AuthoritativeOwner, value.AuthoritativeSource, value.PersistenceOwner, value.WriterAuthority,
            value.SourceIdentity, value.StateVersion, value.EffectiveTime.ToUniversalTime().ToString("O"),
            value.RetentionClassification, value.Payload, value.PreviousRecordDigest));

    private static string Hash(string value)
        => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
}
