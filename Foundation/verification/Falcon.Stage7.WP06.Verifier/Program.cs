using System;
using System.Collections.Generic;
using System.Linq;
using Foundation.HealthFitness;

var checks = new List<(string Name, Action Run)>
{
    ("all-seven-domains-current", VerifyAllSevenDomainsCurrent),
    ("aggregate-order-determinism", VerifyAggregateOrderDeterminism),
    ("missing-domain-fails-coverage", VerifyMissingDomain),
    ("duplicate-domain-rejected", VerifyDuplicateDomain),
    ("source-owner-mismatch-rejected", () => ExpectEvidenceRejected(e => e with { SourceOwner = "wrong-owner" })),
    ("source-id-mismatch-rejected", () => ExpectEvidenceRejected(e => e with { SourceId = "wrong-source" })),
    ("schema-version-mismatch-rejected", () => ExpectEvidenceRejected(e => e with { SchemaVersion = "v2" })),
    ("truth-kind-mismatch-rejected", () => ExpectEvidenceRejected(e => e with { TruthKind = "wrong-truth" })),
    ("future-time-rejected", () => ExpectEvidenceRejected(e => e with { ObservationTime = e.AssessmentTime.AddMinutes(1) })),
    ("impossible-time-order-rejected", () => ExpectEvidenceRejected(e => e with { EffectiveTime = e.ObservationTime.AddMinutes(-1) })),
    ("stale-reduces", VerifyStale),
    ("replay-reduces", () => VerifyNonCurrentClassification(PredecessorTruthOperationalClassification.Replay)),
    ("historical-reduces", () => VerifyNonCurrentClassification(PredecessorTruthOperationalClassification.AuthoritativeHistorical)),
    ("test-reduces", () => VerifyNonCurrentClassification(PredecessorTruthOperationalClassification.Test)),
    ("simulation-reduces", () => VerifyNonCurrentClassification(PredecessorTruthOperationalClassification.Simulation)),
    ("non-authoritative-reduces", () => VerifyNonCurrentClassification(PredecessorTruthOperationalClassification.NonAuthoritative)),
    ("missing-reduces", () => VerifyAvailability(PredecessorTruthAvailability.Missing)),
    ("inaccessible-reduces", () => VerifyAvailability(PredecessorTruthAvailability.Inaccessible)),
    ("authenticity-unverified-reduces", VerifyAuthenticityUnverified),
    ("authenticity-mismatch-invalid", VerifyAuthenticityMismatch),
    ("integrity-unverified-reduces", VerifyIntegrityUnverified),
    ("integrity-corrupted-invalid", VerifyIntegrityCorrupted),
    ("provenance-unverified-reduces", VerifyProvenanceUnverified),
    ("provenance-failed-invalid", VerifyProvenanceFailed),
    ("wp05-positive-binding-pass", VerifyWp05PositiveBinding),
    ("wp05-source-owner-mismatch-rejected", VerifyWp05SourceOwnerMismatch),
    ("wp05-replay-optimism-rejected", VerifyWp05ReplayOptimism),
    ("identity-mutation-sensitive", VerifyIdentityMutation)
};

var failures = 0;
foreach (var check in checks)
{
    try
    {
        check.Run();
        Console.WriteLine($"PASS | {check.Name}");
    }
    catch (Exception ex)
    {
        failures++;
        Console.WriteLine($"FAIL | {check.Name} | {ex.GetType().Name}: {ex.Message}");
    }
}

Console.WriteLine($"STAGE7_WP06_VERIFIER = {(failures == 0 ? "PASS" : "FAIL")}");
Console.WriteLine($"CHECKS = {checks.Count - failures}/{checks.Count}");
return failures == 0 ? 0 : 1;

static void VerifyAllSevenDomainsCurrent()
{
    var results = CurrentResults();
    Require(results.Length == 7, "expected seven predecessor domains");
    Require(results.All(r => r.CanSupportCurrentAwareness), "all domains must support current awareness");
    Require(results.All(r => r.EvidenceQuality == EvidenceQuality.Sufficient), "all current domains must be sufficient");
    var coverage = PredecessorTruthIntegrationRuntime.EvaluateCoverage("coverage-all", results);
    Require(coverage.CompleteCurrentCoverage, "complete coverage expected");
    Require(coverage.MissingDomains.Count == 0, "no missing domains expected");
    Require(coverage.EvidenceQuality == EvidenceQuality.Sufficient, "coverage must be sufficient");
}

static void VerifyAggregateOrderDeterminism()
{
    var results = CurrentResults();
    var forward = PredecessorTruthIntegrationRuntime.EvaluateCoverage("coverage-order", results);
    var reverse = PredecessorTruthIntegrationRuntime.EvaluateCoverage("coverage-order", results.Reverse());
    Require(forward.Identity == reverse.Identity, "coverage identity changed with input order");
    Require(forward.ResultIdentities.SequenceEqual(reverse.ResultIdentities, StringComparer.Ordinal), "canonical result order changed");
}

static void VerifyMissingDomain()
{
    var results = CurrentResults().Where(r => r.Domain != PredecessorTruthDomain.LoggingPersistence).ToArray();
    var coverage = PredecessorTruthIntegrationRuntime.EvaluateCoverage("coverage-missing", results);
    Require(!coverage.CompleteCurrentCoverage, "missing domain cannot be complete");
    Require(coverage.EvidenceQuality == EvidenceQuality.Insufficient, "missing domain must be insufficient");
    Require(coverage.MissingDomains.SequenceEqual(new[] { PredecessorTruthDomain.LoggingPersistence }), "exact missing domain not reported");
}

static void VerifyDuplicateDomain()
{
    var results = CurrentResults().ToList();
    results.Add(results[0] with { ResultId = "duplicate-result" });
    ExpectThrows<ArgumentException>(() => PredecessorTruthIntegrationRuntime.EvaluateCoverage("coverage-duplicate", results));
}

static void ExpectEvidenceRejected(Func<PredecessorTruthEvidence, PredecessorTruthEvidence> mutate)
{
    var definition = Definition(PredecessorTruthDomain.Stage3DependencyConfiguration);
    var evidence = mutate(Evidence(definition));
    ExpectThrows<ArgumentException>(() => PredecessorTruthIntegrationRuntime.Evaluate("reject-result", definition, evidence));
}

static void VerifyStale()
{
    var definition = Definition(PredecessorTruthDomain.Stage3DependencyConfiguration);
    var evidence = Evidence(definition) with { Expiry = Evidence(definition).AssessmentTime };
    var result = PredecessorTruthIntegrationRuntime.Evaluate("stale-result", definition, evidence);
    Require(!result.CanSupportCurrentAwareness, "stale source cannot be current");
    Require(result.LossClass == HealthEvidenceLossClass.Stale, "stale loss expected");
    Require(result.EvidenceQuality == EvidenceQuality.Insufficient, "stale must be insufficient");
}

static void VerifyNonCurrentClassification(PredecessorTruthOperationalClassification classification)
{
    var definition = Definition(PredecessorTruthDomain.Stage5ContractMessageEventProtection);
    var result = PredecessorTruthIntegrationRuntime.Evaluate("classification-result", definition, Evidence(definition) with { OperationalClassification = classification });
    Require(!result.CanSupportCurrentAwareness, "non-current classification cannot support current awareness");
    Require(result.EvidenceQuality != EvidenceQuality.Sufficient, "non-current classification cannot be sufficient");
}

static void VerifyAvailability(PredecessorTruthAvailability availability)
{
    var definition = Definition(PredecessorTruthDomain.Stage6ResourcePressureIsolationLoadShedding);
    var result = PredecessorTruthIntegrationRuntime.Evaluate("availability-result", definition, Evidence(definition) with { Availability = availability });
    Require(!result.CanSupportCurrentAwareness, "unavailable predecessor cannot be current");
    Require(result.EvidenceQuality == EvidenceQuality.Insufficient, "unavailable predecessor must be insufficient");
}

static void VerifyAuthenticityUnverified()
{
    var definition = Definition(PredecessorTruthDomain.SecurityTrustIdentity);
    var result = PredecessorTruthIntegrationRuntime.Evaluate("auth-unverified", definition, Evidence(definition) with { AuthenticityStatus = PredecessorTruthAuthenticityStatus.Unverified });
    Require(!result.CanSupportCurrentAwareness && result.EvidenceQuality == EvidenceQuality.Insufficient, "unverified authenticity must reduce");
}

static void VerifyAuthenticityMismatch()
{
    var definition = Definition(PredecessorTruthDomain.SecurityTrustIdentity);
    var result = PredecessorTruthIntegrationRuntime.Evaluate("auth-mismatch", definition, Evidence(definition) with { AuthenticityStatus = PredecessorTruthAuthenticityStatus.Mismatch });
    Require(!result.CanSupportCurrentAwareness && result.EvidenceQuality == EvidenceQuality.Invalid, "authenticity mismatch must invalidate");
}

static void VerifyIntegrityUnverified()
{
    var definition = Definition(PredecessorTruthDomain.Stage4EvidenceReconciliation);
    var result = PredecessorTruthIntegrationRuntime.Evaluate("integrity-unverified", definition, Evidence(definition) with { IntegrityStatus = PredecessorTruthIntegrityStatus.Unverified });
    Require(!result.CanSupportCurrentAwareness && result.EvidenceQuality == EvidenceQuality.Insufficient, "unverified integrity must reduce");
}

static void VerifyIntegrityCorrupted()
{
    var definition = Definition(PredecessorTruthDomain.Stage4EvidenceReconciliation);
    var result = PredecessorTruthIntegrationRuntime.Evaluate("integrity-corrupt", definition, Evidence(definition) with { IntegrityStatus = PredecessorTruthIntegrityStatus.Corrupted });
    Require(result.EvidenceQuality == EvidenceQuality.Invalid && result.LossClass == HealthEvidenceLossClass.Corrupted, "corruption must invalidate");
}

static void VerifyProvenanceUnverified()
{
    var definition = Definition(PredecessorTruthDomain.LoggingPersistence);
    var result = PredecessorTruthIntegrationRuntime.Evaluate("provenance-unverified", definition, Evidence(definition) with { ProvenanceStatus = PredecessorTruthProvenanceStatus.Unverified });
    Require(!result.CanSupportCurrentAwareness && result.EvidenceQuality == EvidenceQuality.Insufficient, "unverified provenance must reduce");
}

static void VerifyProvenanceFailed()
{
    var definition = Definition(PredecessorTruthDomain.LoggingPersistence);
    var result = PredecessorTruthIntegrationRuntime.Evaluate("provenance-failed", definition, Evidence(definition) with { ProvenanceStatus = PredecessorTruthProvenanceStatus.Failed });
    Require(result.EvidenceQuality == EvidenceQuality.Invalid && result.LossClass == HealthEvidenceLossClass.ProvenanceFailure, "failed provenance must invalidate");
}

static void VerifyWp05PositiveBinding()
{
    var definition = Definition(PredecessorTruthDomain.Stage3DependencyConfiguration);
    var evidence = Evidence(definition);
    var result = PredecessorTruthIntegrationRuntime.Evaluate("wp05-positive-result", definition, evidence);
    var relation = Relation(result, HealthEvidenceLossClass.Available, EvidenceQuality.Sufficient);
    var validation = PredecessorTruthIntegrationRuntime.ValidateWp05RelationBinding(result, relation);
    Require(validation.Result == Foundation.Contracts.ValidationResult.Pass, validation.Message);
}

static void VerifyWp05SourceOwnerMismatch()
{
    var definition = Definition(PredecessorTruthDomain.Stage3DependencyConfiguration);
    var result = PredecessorTruthIntegrationRuntime.Evaluate("wp05-owner-result", definition, Evidence(definition));
    var relation = Relation(result, HealthEvidenceLossClass.Available, EvidenceQuality.Sufficient) with { SourceOwner = "other-owner" };
    var validation = PredecessorTruthIntegrationRuntime.ValidateWp05RelationBinding(result, relation);
    Require(validation.Result != Foundation.Contracts.ValidationResult.Pass, "owner mismatch must fail");
}

static void VerifyWp05ReplayOptimism()
{
    var definition = Definition(PredecessorTruthDomain.Stage5ContractMessageEventProtection);
    var result = PredecessorTruthIntegrationRuntime.Evaluate("wp05-replay-result", definition, Evidence(definition) with { OperationalClassification = PredecessorTruthOperationalClassification.Replay });
    var optimistic = Relation(result, HealthEvidenceLossClass.Available, EvidenceQuality.Sufficient);
    var validation = PredecessorTruthIntegrationRuntime.ValidateWp05RelationBinding(result, optimistic);
    Require(validation.Result != Foundation.Contracts.ValidationResult.Pass, "replay cannot bind as optimistic available WP05 evidence");
}

static void VerifyIdentityMutation()
{
    var definition = Definition(PredecessorTruthDomain.Stage4AuthorityLifecycleState);
    var evidence = Evidence(definition);
    var mutated = evidence with { PayloadDigest = "digest-mutated" };
    Require(evidence.Identity != mutated.Identity, "material evidence mutation must change identity");
    var resultA = PredecessorTruthIntegrationRuntime.Evaluate("identity-result", definition, evidence);
    var resultB = PredecessorTruthIntegrationRuntime.Evaluate("identity-result", definition, mutated);
    Require(resultA.Identity != resultB.Identity, "material result mutation must change identity");
}

static PredecessorTruthIntegrationResult[] CurrentResults() =>
    PredecessorTruthIntegrationRuntime.RequiredDomains
        .Select((domain, index) =>
        {
            var definition = Definition(domain);
            return PredecessorTruthIntegrationRuntime.Evaluate($"result-{index + 1}", definition, Evidence(definition));
        })
        .ToArray();

static PredecessorTruthSourceDefinition Definition(PredecessorTruthDomain domain)
{
    var suffix = ((int)domain).ToString();
    return new PredecessorTruthSourceDefinition(
        $"definition-{suffix}", domain, $"source-{suffix}", $"owner-{suffix}", $"truth-{suffix}",
        $"schema-{suffix}", "v1", $"authority-{suffix}");
}

static PredecessorTruthEvidence Evidence(PredecessorTruthSourceDefinition definition)
{
    var now = new DateTimeOffset(2026, 8, 14, 10, 0, 0, TimeSpan.Zero);
    var suffix = ((int)definition.Domain).ToString();
    return new PredecessorTruthEvidence(
        $"evidence-{suffix}", definition.Domain, definition.SourceId, definition.SourceOwner,
        definition.TruthKind, definition.SchemaId, definition.SchemaVersion,
        "foundation-subject", "foundation-capability", "foundation-scope",
        $"record-{suffix}", "v1", $"digest-{suffix}", $"evidence-ref-{suffix}",
        $"provenance-ref-{suffix}", PredecessorTruthProvenanceStatus.Verified,
        $"integrity-ref-{suffix}", PredecessorTruthIntegrityStatus.Verified,
        PredecessorTruthAuthenticityStatus.Verified, PredecessorTruthAvailability.Available,
        PredecessorTruthOperationalClassification.AuthoritativeCurrent,
        now.AddMinutes(-3), now.AddMinutes(-2), now, now.AddMinutes(30), "verified predecessor truth");
}

static HealthEvidenceRelationAssessment Relation(
    PredecessorTruthIntegrationResult result,
    HealthEvidenceLossClass loss,
    EvidenceQuality quality)
{
    var now = result.AssessmentTime;
    return new HealthEvidenceRelationAssessment(
        "relation-wp06", "requirement-wp06", "rule-wp06", "v1",
        "foundation-subject", "foundation-capability", "foundation-scope",
        HealthEvidenceRole.RequiredPrimary, result.SourceId, result.SourceOwner,
        result.EvidenceReference, HealthEvidenceAcquisitionState.Arrived, loss, quality,
        "WP06 bridge relation", now.AddMinutes(-1), now, now.AddMinutes(20),
        "canonical-health", "canonical-health-identity");
}

static void Require(bool condition, string message)
{
    if (!condition)
        throw new InvalidOperationException(message);
}

static void ExpectThrows<T>(Action action) where T : Exception
{
    try
    {
        action();
    }
    catch (T)
    {
        return;
    }

    throw new InvalidOperationException("Expected exception was not thrown: " + typeof(T).Name);
}
