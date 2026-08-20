using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Foundation.Contracts;
using Foundation.SelfAwareness;

namespace Falcon.Stage7.WP05.Verifier;

internal static class Wp05DriftCompetenceCoverageFixtures
{
    [ModuleInitializer]
    internal static void Run()
    {
        var coverage = Wp05FixtureSupport.Coverage();
        var competence = Wp05FixtureSupport.Competence(coverage);
        var subject = "foundation.health.subject:wp05:coverage";
        var capability = "foundation.technical.health";
        var scope = "scope:foundation:wp05:coverage";
        var t = Wp05FixtureSupport.T;

        var complete = EvidenceAwarenessRuntime.Evaluate(subject, capability, scope, coverage, competence, Array.Empty<DriftFinding>(), null, t);
        Wp05FixtureSupport.Require(complete.BlindSpots.Count == 0, "WP05 coverage: complete drift/competence produced blind spots.");

        var missingCompetence = competence.Where(x => x.Domain != EvidenceDriftDomain.Data).ToArray();
        var missing = EvidenceAwarenessRuntime.Evaluate(subject, capability, scope, coverage, missingCompetence, Array.Empty<DriftFinding>(), null, t);
        Wp05FixtureSupport.Require(missing.BlindSpots.Any(x => x.Domain == EvidenceDriftDomain.Data), "WP05 coverage: missing competence did not create blind spot.");

        var expiredCompetence = competence.Select(x => x.Domain == EvidenceDriftDomain.Behavior ? x with { Expiry = t } : x).ToArray();
        var expired = EvidenceAwarenessRuntime.Evaluate(subject, capability, scope, coverage, expiredCompetence, Array.Empty<DriftFinding>(), null, t);
        Wp05FixtureSupport.Require(expired.BlindSpots.Any(x => x.Domain == EvidenceDriftDomain.Behavior), "WP05 coverage: expired competence did not create blind spot.");

        var wrongSubjectCompetence = competence.Select(x => x.Domain == EvidenceDriftDomain.Configuration ? x with { SubjectId = "foundation.health.subject:wrong" } : x).ToArray();
        var wrongSubject = EvidenceAwarenessRuntime.Evaluate(subject, capability, scope, coverage, wrongSubjectCompetence, Array.Empty<DriftFinding>(), null, t);
        Wp05FixtureSupport.Require(wrongSubject.BlindSpots.Any(x => x.Domain == EvidenceDriftDomain.Configuration), "WP05 coverage: mismatched competence subject was accepted.");

        var wrongScopeCompetence = competence.Select(x => x.Domain == EvidenceDriftDomain.Dependency ? x with { Scope = "scope:wrong" } : x).ToArray();
        var wrongScope = EvidenceAwarenessRuntime.Evaluate(subject, capability, scope, coverage, wrongScopeCompetence, Array.Empty<DriftFinding>(), null, t);
        Wp05FixtureSupport.Require(wrongScope.BlindSpots.Any(x => x.Domain == EvidenceDriftDomain.Dependency), "WP05 coverage: mismatched competence scope was accepted.");

        var finding = new DriftFinding(
            "drift:finding:coverage", coverage[0].DeclarationId, coverage[0].Domain, subject, scope,
            DriftFindingState.MaterialDriftDetected, "observed:drifted", coverage[0].ComparisonBasisId,
            "evidence:drift:finding", "material-drift-against-governed-basis", t.AddSeconds(-1), t);
        var drifted = EvidenceAwarenessRuntime.Evaluate(subject, capability, scope, coverage, competence, new[] { finding }, null, t);
        Wp05FixtureSupport.Require(drifted.DriftFindings.Single().State == DriftFindingState.MaterialDriftDetected,
            "WP05 coverage: material drift finding was not preserved.");

        var nonApplicable = coverage.Select(x => x.Domain == EvidenceDriftDomain.FoundationModel
            ? x with { Applicability = DriftApplicability.NotApplicable, ComparisonBasisId = string.Empty, Reason = "governed-not-applicable" }
            : x).ToArray();
        var naCompetence = Wp05FixtureSupport.Competence(nonApplicable.Where(x => x.Applicability == DriftApplicability.Applicable));
        var na = EvidenceAwarenessRuntime.Evaluate(subject, capability, scope, nonApplicable, naCompetence, Array.Empty<DriftFinding>(), null, t);
        Wp05FixtureSupport.Require(!na.BlindSpots.Any(x => x.Domain == EvidenceDriftDomain.FoundationModel),
            "WP05 coverage: evidence-bound non-applicable drift domain created blind spot.");

        var invalidNa = nonApplicable.Select(x => x.Domain == EvidenceDriftDomain.FoundationModel
            ? x with { GoverningAuthority = string.Empty }
            : x).ToArray();
        var rejected = false;
        try
        {
            _ = EvidenceAwarenessRuntime.Evaluate(subject, capability, scope, invalidNa, naCompetence, Array.Empty<DriftFinding>(), null, t);
        }
        catch (ArgumentException)
        {
            rejected = true;
        }
        Wp05FixtureSupport.Require(rejected, "WP05 coverage: non-applicable drift without governing identity was accepted.");
    }
}
