using System;
using System.Collections.Generic;
using System.Text.Json;
using Foundation.HealthFitness;

var checks = new List<(string Name, Action Test)>
{
    ("health-change-authoritative", () =>
    {
        var pair = Samples.HealthChanged();
        var fact = HealthFitnessHistoryRuntime.CreateChangeFact("evt-health-001", HealthFitnessFactKind.HealthStateChanged, pair.Previous, pair.Current, HealthFitnessEventTruthClassification.AuthoritativeOperational, HealthFitnessEventRelationKind.None, null, "prov-health-001");
        Require(fact.EventType == HealthFitnessHistoryRuntime.HealthEventType, "health mapping");
    }),
    ("fitness-change-authoritative", () =>
    {
        var pair = Samples.FitnessChanged();
        var fact = HealthFitnessHistoryRuntime.CreateChangeFact("evt-fit-001", HealthFitnessFactKind.FitnessChanged, pair.Previous, pair.Current, HealthFitnessEventTruthClassification.AuthoritativeOperational, HealthFitnessEventRelationKind.None, null, "prov-fit-001");
        Require(fact.EventType == HealthFitnessHistoryRuntime.FitnessEventType, "fitness mapping");
    }),
    ("no-material-change-rejected", () => ExpectFailure(() =>
    {
        var pair = Samples.HealthChanged();
        HealthFitnessHistoryRuntime.CreateChangeFact("evt-none", HealthFitnessFactKind.FitnessChanged, pair.Previous, pair.Previous, HealthFitnessEventTruthClassification.AuthoritativeOperational, HealthFitnessEventRelationKind.None, null, "prov-none");
    })),
    ("cross-scope-rejected", () => ExpectFailure(() =>
    {
        var pair = Samples.HealthChanged();
        HealthFitnessHistoryRuntime.CreateChangeFact("evt-scope", HealthFitnessFactKind.HealthStateChanged, pair.Previous, pair.Current with { Scope = "scope-other" }, HealthFitnessEventTruthClassification.AuthoritativeOperational, HealthFitnessEventRelationKind.None, null, "prov-scope");
    })),
    ("time-regression-rejected", () => ExpectFailure(() =>
    {
        var pair = Samples.HealthChanged();
        var changed = pair.Current with { AssessmentTime = pair.Previous.AssessmentTime.AddMinutes(-1), EffectiveTime = pair.Previous.EffectiveTime.AddMinutes(-1) };
        HealthFitnessHistoryRuntime.CreateChangeFact("evt-time", HealthFitnessFactKind.HealthStateChanged, pair.Previous, changed, HealthFitnessEventTruthClassification.AuthoritativeOperational, HealthFitnessEventRelationKind.None, null, "prov-time");
    })),
    ("replay-requires-relation", () => ExpectFailure(() =>
    {
        var pair = Samples.HealthChanged();
        HealthFitnessHistoryRuntime.CreateChangeFact("evt-replay-bad", HealthFitnessFactKind.HealthStateChanged, pair.Previous, pair.Current, HealthFitnessEventTruthClassification.Replay, HealthFitnessEventRelationKind.None, null, "prov-replay");
    })),
    ("replay-explicit-pass", () =>
    {
        var pair = Samples.HealthChanged();
        var fact = HealthFitnessHistoryRuntime.CreateChangeFact("evt-replay-good", HealthFitnessFactKind.HealthStateChanged, pair.Previous, pair.Current, HealthFitnessEventTruthClassification.Replay, HealthFitnessEventRelationKind.ReplayOf, "evt-origin", "prov-replay-good");
        Require(fact.Classification == HealthFitnessEventTruthClassification.Replay && fact.RelationKind == HealthFitnessEventRelationKind.ReplayOf, "replay binding");
    }),
    ("nonreplay-replayof-rejected", () => ExpectFailure(() =>
    {
        var pair = Samples.HealthChanged();
        HealthFitnessHistoryRuntime.CreateChangeFact("evt-nonreplay", HealthFitnessFactKind.HealthStateChanged, pair.Previous, pair.Current, HealthFitnessEventTruthClassification.AuthoritativeOperational, HealthFitnessEventRelationKind.ReplayOf, "evt-origin", "prov-x");
    })),
    ("correction-distinct-pass", () =>
    {
        var pair = Samples.HealthChanged();
        var fact = HealthFitnessHistoryRuntime.CreateChangeFact("evt-correction-new", HealthFitnessFactKind.HealthStateChanged, pair.Previous, pair.Current, HealthFitnessEventTruthClassification.AuthoritativeOperational, HealthFitnessEventRelationKind.CorrectionOf, "evt-correction-old", "prov-correction");
        Require(fact.RelationKind == HealthFitnessEventRelationKind.CorrectionOf, "correction relation");
    }),
    ("correction-same-id-rejected", () => ExpectFailure(() =>
    {
        var pair = Samples.HealthChanged();
        HealthFitnessHistoryRuntime.CreateChangeFact("evt-same", HealthFitnessFactKind.HealthStateChanged, pair.Previous, pair.Current, HealthFitnessEventTruthClassification.AuthoritativeOperational, HealthFitnessEventRelationKind.CorrectionOf, "evt-same", "prov-same");
    })),
    ("none-with-related-id-rejected", () => ExpectFailure(() =>
    {
        var pair = Samples.HealthChanged();
        HealthFitnessHistoryRuntime.CreateChangeFact("evt-related", HealthFitnessFactKind.HealthStateChanged, pair.Previous, pair.Current, HealthFitnessEventTruthClassification.AuthoritativeOperational, HealthFitnessEventRelationKind.None, "evt-other", "prov-related");
    })),
    ("history-record-digest-valid", () =>
    {
        var (fact, current) = Samples.FactAndCurrent();
        var record = HealthFitnessHistoryRuntime.CreateHistoryRecord(fact, current, 3, "PREVIOUS-DIGEST");
        Require(record.RecordDigest == record.WithComputedDigest().RecordDigest, "record digest");
    }),
    ("trusted-reconstruction", () =>
    {
        var (fact, current) = Samples.FactAndCurrent();
        var result = HealthFitnessHistoryRuntime.Reconstruct(HealthFitnessHistoryRuntime.CreateHistoryRecord(fact, current, 3, "PREVIOUS-DIGEST"), true, true);
        Require(result.Trusted && result.Assessment?.Identity == current.Identity && result.Fact?.Identity == fact.Identity, "trusted reconstruction");
    }),
    ("logging-loss-fails-trust", () =>
    {
        var (fact, current) = Samples.FactAndCurrent();
        var result = HealthFitnessHistoryRuntime.Reconstruct(HealthFitnessHistoryRuntime.CreateHistoryRecord(fact, current, 1, string.Empty), false, true);
        Require(!result.Trusted && result.Reason == "LOGGING_EVIDENCE_UNAVAILABLE", "logging loss");
    }),
    ("persistence-loss-fails-trust", () =>
    {
        var (fact, current) = Samples.FactAndCurrent();
        var result = HealthFitnessHistoryRuntime.Reconstruct(HealthFitnessHistoryRuntime.CreateHistoryRecord(fact, current, 1, string.Empty), true, false);
        Require(!result.Trusted && result.Reason == "PERSISTENCE_EVIDENCE_UNAVAILABLE", "persistence loss");
    }),
    ("missing-history-fails-trust", () => Require(HealthFitnessHistoryRuntime.Reconstruct(null, true, true).Reason == "HISTORY_RECORD_MISSING", "missing history")),
    ("corrupted-record-digest-rejected", () =>
    {
        var (fact, current) = Samples.FactAndCurrent();
        var record = HealthFitnessHistoryRuntime.CreateHistoryRecord(fact, current, 1, string.Empty) with { Payload = "corrupted" };
        Require(HealthFitnessHistoryRuntime.Reconstruct(record, true, true).Reason == "HISTORY_RECORD_DIGEST_INVALID", "corrupted digest");
    }),
    ("ownership-mutation-rejected", () =>
    {
        var (fact, current) = Samples.FactAndCurrent();
        var record = HealthFitnessHistoryRuntime.CreateHistoryRecord(fact, current, 1, string.Empty);
        var mutated = (record with { AuthoritativeOwner = "Other.Owner", RecordDigest = string.Empty }).WithComputedDigest();
        Require(HealthFitnessHistoryRuntime.Reconstruct(mutated, true, true).Reason == "HISTORY_OWNERSHIP_BINDING_INVALID", "ownership mutation");
    }),
    ("payload-identity-mutation-rejected", () =>
    {
        var (fact, current) = Samples.FactAndCurrent();
        var record = HealthFitnessHistoryRuntime.CreateHistoryRecord(fact, current, 1, string.Empty);
        var payload = JsonSerializer.Deserialize<PersistedHealthFitnessBasis>(record.Payload) ?? throw new Exception("payload missing");
        var mutatedPayload = JsonSerializer.Serialize(payload with { Fact = payload.Fact with { Provenance = "tampered" } });
        var mutated = (record with { Payload = mutatedPayload, RecordDigest = string.Empty }).WithComputedDigest();
        Require(HealthFitnessHistoryRuntime.Reconstruct(mutated, true, true).Reason == "HISTORY_IDENTITY_BINDING_INVALID", "payload mutation");
    }),
    ("source-identity-mutation-rejected", () =>
    {
        var (fact, current) = Samples.FactAndCurrent();
        var record = HealthFitnessHistoryRuntime.CreateHistoryRecord(fact, current, 1, string.Empty);
        var mutated = (record with { SourceIdentity = "BAD-SOURCE", RecordDigest = string.Empty }).WithComputedDigest();
        Require(HealthFitnessHistoryRuntime.Reconstruct(mutated, true, true).Reason == "HISTORY_IDENTITY_BINDING_INVALID", "source mutation");
    }),
    ("replay-history-stays-replay", () =>
    {
        var pair = Samples.HealthChanged();
        var fact = HealthFitnessHistoryRuntime.CreateChangeFact("evt-r2", HealthFitnessFactKind.HealthStateChanged, pair.Previous, pair.Current, HealthFitnessEventTruthClassification.Replay, HealthFitnessEventRelationKind.ReplayOf, "evt-origin-2", "prov-r2");
        var result = HealthFitnessHistoryRuntime.Reconstruct(HealthFitnessHistoryRuntime.CreateHistoryRecord(fact, pair.Current, 1, string.Empty), true, true);
        Require(result.Trusted && result.Fact?.Classification == HealthFitnessEventTruthClassification.Replay, "replay preserved");
    }),
    ("fact-identity-deterministic", () =>
    {
        var pair = Samples.HealthChanged();
        var a = HealthFitnessHistoryRuntime.CreateChangeFact("evt-det", HealthFitnessFactKind.HealthStateChanged, pair.Previous, pair.Current, HealthFitnessEventTruthClassification.AuthoritativeOperational, HealthFitnessEventRelationKind.None, null, "prov-det");
        var b = HealthFitnessHistoryRuntime.CreateChangeFact("evt-det", HealthFitnessFactKind.HealthStateChanged, pair.Previous, pair.Current, HealthFitnessEventTruthClassification.AuthoritativeOperational, HealthFitnessEventRelationKind.None, null, "prov-det");
        Require(a.Identity == b.Identity, "deterministic identity");
    }),
    ("fact-identity-mutation-sensitive", () =>
    {
        var pair = Samples.HealthChanged();
        var a = HealthFitnessHistoryRuntime.CreateChangeFact("evt-mut", HealthFitnessFactKind.HealthStateChanged, pair.Previous, pair.Current, HealthFitnessEventTruthClassification.AuthoritativeOperational, HealthFitnessEventRelationKind.None, null, "prov-a");
        Require(a.Identity != (a with { Provenance = "prov-b" }).Identity, "mutation sensitivity");
    }),
    ("previous-digest-chain-preserved", () =>
    {
        var (fact, current) = Samples.FactAndCurrent();
        var record = HealthFitnessHistoryRuntime.CreateHistoryRecord(fact, current, 7, "CHAIN-ANCHOR");
        Require(record.PreviousRecordDigest == "CHAIN-ANCHOR" && record.StateVersion == 7, "chain preservation");
    }),
    ("substrate-ownership-declared", () => Require(HealthFitnessHistoryRuntime.EventSubstrateOwner == "Foundation.EventSystem" && HealthFitnessHistoryRuntime.PersistenceOwner == "Foundation.State", "substrate owners")),
    ("assessment-basis-exact", () =>
    {
        var (fact, current) = Samples.FactAndCurrent();
        var result = HealthFitnessHistoryRuntime.Reconstruct(HealthFitnessHistoryRuntime.CreateHistoryRecord(fact, current, 2, string.Empty), true, true);
        Require(result.Assessment?.Identity == current.Identity && result.Assessment?.SelfModelReference == current.SelfModelReference, "basis exact");
    })
};

var passed = 0;
foreach (var check in checks)
{
    try { check.Test(); passed++; Console.WriteLine($"PASS | {check.Name}"); }
    catch (Exception ex) { Console.WriteLine($"FAIL | {check.Name} | {ex.GetType().Name}: {ex.Message}"); }
}
Console.WriteLine(passed == checks.Count ? "STAGE7_WP07_VERIFIER = PASS" : "STAGE7_WP07_VERIFIER = FAIL");
Console.WriteLine($"CHECKS = {passed}/{checks.Count}");
return passed == checks.Count ? 0 : 1;

static void Require(bool condition, string message) { if (!condition) throw new InvalidOperationException(message); }
static void ExpectFailure(Action action) { try { action(); } catch { return; } throw new InvalidOperationException("expected rejection did not occur"); }

static class Samples
{
    private static readonly DateTimeOffset T0 = new(2026, 8, 14, 9, 0, 0, TimeSpan.Zero);
    public static (CanonicalHealthFitnessAssessment Previous, CanonicalHealthFitnessAssessment Current) HealthChanged()
        => (Assessment("assessment-prev-health", HealthState.Healthy, TechnicalFitnessState.Fit, FitnessProjectionResult.Fit, T0), Assessment("assessment-current-health", HealthState.Degraded, TechnicalFitnessState.FitWithConstraints, FitnessProjectionResult.Restricted, T0.AddMinutes(5)));
    public static (CanonicalHealthFitnessAssessment Previous, CanonicalHealthFitnessAssessment Current) FitnessChanged()
        => (Assessment("assessment-prev-fit", HealthState.Healthy, TechnicalFitnessState.Fit, FitnessProjectionResult.Fit, T0), Assessment("assessment-current-fit", HealthState.Healthy, TechnicalFitnessState.Degraded, FitnessProjectionResult.Restricted, T0.AddMinutes(5)));
    public static (HealthFitnessChangeFact Fact, CanonicalHealthFitnessAssessment Current) FactAndCurrent()
    {
        var pair = HealthChanged();
        return (HealthFitnessHistoryRuntime.CreateChangeFact("evt-history-001", HealthFitnessFactKind.HealthStateChanged, pair.Previous, pair.Current, HealthFitnessEventTruthClassification.AuthoritativeOperational, HealthFitnessEventRelationKind.None, null, "prov-history-001"), pair.Current);
    }
    private static CanonicalHealthFitnessAssessment Assessment(string id, HealthState health, TechnicalFitnessState fitness, FitnessProjectionResult projection, DateTimeOffset time)
        => new(id, "subject-foundation-core", "capability-health-fitness", "authority-level-observed", health, fitness, projection, "scope-foundation", "evidence-stage7-wp07", "self-model-stage7", EvidenceQuality.Sufficient, "0.95", "none", "none", "bounded", "stage7-wp07-verifier", "rule-health-history", "1.0", time, time.AddSeconds(1), time.AddSeconds(2), time.AddMinutes(10));
}
