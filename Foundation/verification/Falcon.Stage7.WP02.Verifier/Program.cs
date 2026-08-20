using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Foundation.HealthFitness;

internal static class Program
{
    private static readonly DateTimeOffset BaseTime = new(2026, 8, 12, 12, 0, 0, TimeSpan.Zero);

    private static int Main()
    {
        try
        {
            RunAll();
            Console.WriteLine("STAGE7_WP02_VERIFIER=PASS");
            return 0;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine("STAGE7_WP02_VERIFIER=FAIL");
            Console.Error.WriteLine(exception.Message);
            return 1;
        }
    }

    private static void RunAll()
    {
        VerifyGate0BFreshnessConstants();
        VerifyHealthyAssessment();
        VerifyDeterministicAssessmentIdentity();
        VerifyMutationSensitivity();
        VerifyConfiguredFreshnessCannotLoosenProfile();
        VerifyConfiguredFreshnessCanTightenProfile();
        VerifyMissingRequiredEvidenceFailsClosed();
        VerifyStaleRequiredEvidenceFailsClosed();
        VerifyInvalidRequiredEvidenceFailsClosed();
        VerifyContradictoryRequiredEvidenceFailsClosed();
        VerifySourceBoundRequiresSourceExpiry();
        VerifyEventBoundWitnessLossFailsClosed();
        VerifyEventBoundWithoutWitnessFallsBackToSlow();
        VerifyMonitorVisibilityLossFailsClosed();
        VerifyRequiredFailureProducesUnhealthy();
        VerifyKnownRequiredDegradationProducesDegraded();
        VerifyRequiredDependencyFailureIsPreserved();
        VerifyRequiredDependencyUnknownIsPreserved();
        VerifyDegradableDependencyRequiresProvenMode();
        VerifyDegradableDependencyWithProvenModeDegrades();
        VerifyApplicableRuleWithoutRequiredEvidenceRejected();
        VerifyLimitedSupportingEvidenceCannotProduceHealthy();
        VerifyRequiredDependencyLimitedHealthyCannotProduceHealthy();
        VerifyRequiredDependencyNotApplicableFailsClosed();
        VerifyDuplicateDependencyEvidenceFailsClosed();
        VerifyFutureDatedRequiredDependencyFailsClosed();
        VerifyDependencyFailureEvidenceBindsLocalAndDependencyEvidence();
        VerifySupportingContradictionIsExplicit();
        VerifyMaterialTransitionOutput();
        VerifyNoTransitionWhenStateUnchanged();
        VerifyGovernedNotApplicable();
        VerifyNoAuthorityOrRecoveryActionSurface();
        VerifyNoApplicationBusinessDependency();
    }

    private static HealthRuleDefinition CreateRule(
        HealthFreshnessProfile profile = HealthFreshnessProfile.Fast,
        TimeSpan? configuredFreshness = null,
        bool applicable = true,
        bool usesIndependentEventWitness = false,
        IReadOnlyList<HealthDependencyRequirement>? dependencies = null)
    {
        return new HealthRuleDefinition(
            "health-rule:stage7:wp02:runtime",
            "1.0",
            "foundation.health.subject:wp02",
            "foundation.technical.health",
            profile,
            configuredFreshness,
            HealthConsequenceClass.CapabilityBlocking,
            "Falcon Operational Integrity Authority",
            "SYS-008 v1.1",
            applicable,
            usesIndependentEventWitness,
            new[]
            {
                new HealthEvidenceRequirement(
                    "requirement:runtime-availability",
                    HealthDimension.Availability,
                    HealthEvidenceRole.RequiredPrimary,
                    "source:lifecycle",
                    "Foundation Lifecycle Authority"),
                new HealthEvidenceRequirement(
                    "requirement:independent-integrity",
                    HealthDimension.Integrity,
                    HealthEvidenceRole.RequiredIndependent,
                    "source:evidence",
                    "Foundation Evidence Authority")
            },
            dependencies ?? Array.Empty<HealthDependencyRequirement>());
    }

    private static HealthObservation[] CreateObservations(
        DateTimeOffset observationTime,
        DateTimeOffset? sourceExpiry = null,
        HealthObservationCondition primaryCondition = HealthObservationCondition.Satisfied,
        HealthObservationCondition independentCondition = HealthObservationCondition.Satisfied,
        bool primaryProvenanceValid = true,
        bool primaryIntegrityValid = true,
        bool primaryClockValid = true,
        bool primaryAcyclic = true,
        bool primaryVisible = true,
        bool eventWitnessCurrent = true)
    {
        return new[]
        {
            new HealthObservation(
                "observation:wp02:primary",
                "requirement:runtime-availability",
                "foundation.health.subject:wp02",
                "foundation.technical.health",
                HealthDimension.Availability,
                "source:lifecycle",
                "Foundation Lifecycle Authority",
                "evidence:wp02:primary",
                primaryCondition,
                observationTime,
                sourceExpiry,
                primaryProvenanceValid,
                primaryIntegrityValid,
                primaryClockValid,
                primaryAcyclic,
                primaryVisible,
                eventWitnessCurrent),
            new HealthObservation(
                "observation:wp02:independent",
                "requirement:independent-integrity",
                "foundation.health.subject:wp02",
                "foundation.technical.health",
                HealthDimension.Integrity,
                "source:evidence",
                "Foundation Evidence Authority",
                "evidence:wp02:independent",
                independentCondition,
                observationTime,
                sourceExpiry,
                true,
                true,
                true,
                true,
                true,
                eventWitnessCurrent)
        };
    }

    private static HealthDependencyAssessment CreateDependency(
        HealthState state,
        EvidenceQuality quality = EvidenceQuality.Sufficient,
        bool independentModeEvidenceValid = false)
    {
        return new HealthDependencyAssessment(
            "dependency:wp02:critical",
            "foundation.dependency.capability",
            state,
            quality,
            "evidence:wp02:dependency",
            BaseTime,
            BaseTime.AddMinutes(1),
            independentModeEvidenceValid);
    }

    private static void VerifyGate0BFreshnessConstants()
    {
        Require(HealthFreshnessPolicy.CriticalMaximumAge == TimeSpan.FromSeconds(5), "HFP-CRITICAL is not 5 seconds");
        Require(HealthFreshnessPolicy.FastMaximumAge == TimeSpan.FromSeconds(15), "HFP-FAST is not 15 seconds");
        Require(HealthFreshnessPolicy.StandardMaximumAge == TimeSpan.FromSeconds(60), "HFP-STANDARD is not 60 seconds");
        Require(HealthFreshnessPolicy.SlowMaximumAge == TimeSpan.FromSeconds(300), "HFP-SLOW is not 300 seconds");
    }

    private static void VerifyHealthyAssessment()
    {
        var result = HealthObservationAssessmentRuntime.Evaluate(
            CreateRule(),
            CreateObservations(BaseTime, BaseTime.AddMinutes(1)),
            Array.Empty<HealthDependencyAssessment>(),
            BaseTime.AddSeconds(1));

        Require(result.Assessment.HealthState == HealthState.Healthy, "fresh valid required evidence did not produce HEALTHY");
        Require(result.Assessment.EvidenceQuality == EvidenceQuality.Sufficient, "fresh valid required evidence is not EQ-SUFFICIENT");
        Require(string.Equals(result.Assessment.ReasonCode, "ALL_REQUIRED_EVIDENCE_HEALTHY", StringComparison.Ordinal), "positive reason code mismatch");
        Require(result.Transition is null, "transition emitted without previous state");
    }

    private static void VerifyDeterministicAssessmentIdentity()
    {
        var rule = CreateRule();
        var observations = CreateObservations(BaseTime, BaseTime.AddMinutes(1));
        var first = HealthObservationAssessmentRuntime.Evaluate(rule, observations, Array.Empty<HealthDependencyAssessment>(), BaseTime.AddSeconds(1));
        var second = HealthObservationAssessmentRuntime.Evaluate(rule, observations, Array.Empty<HealthDependencyAssessment>(), BaseTime.AddSeconds(1));

        Require(string.Equals(first.Assessment.AssessmentId, second.Assessment.AssessmentId, StringComparison.Ordinal), "identical WP-02 input produced different assessment IDs");
        Require(string.Equals(first.Assessment.Identity, second.Assessment.Identity, StringComparison.Ordinal), "identical WP-02 input produced different assessment identities");
    }

    private static void VerifyMutationSensitivity()
    {
        var rule = CreateRule();
        var original = CreateObservations(BaseTime, BaseTime.AddMinutes(1));
        var mutated = CreateObservations(BaseTime, BaseTime.AddMinutes(1));
        mutated[0] = mutated[0] with { EvidenceReference = "evidence:wp02:primary:mutated" };

        var first = HealthObservationAssessmentRuntime.Evaluate(rule, original, Array.Empty<HealthDependencyAssessment>(), BaseTime.AddSeconds(1));
        var second = HealthObservationAssessmentRuntime.Evaluate(rule, mutated, Array.Empty<HealthDependencyAssessment>(), BaseTime.AddSeconds(1));

        Require(!string.Equals(first.Assessment.AssessmentId, second.Assessment.AssessmentId, StringComparison.Ordinal), "material evidence mutation did not change WP-02 assessment ID");
    }

    private static void VerifyConfiguredFreshnessCannotLoosenProfile()
    {
        var rule = CreateRule(HealthFreshnessProfile.Fast, TimeSpan.FromMinutes(1));
        var result = HealthObservationAssessmentRuntime.Evaluate(
            rule,
            CreateObservations(BaseTime, BaseTime.AddMinutes(2)),
            Array.Empty<HealthDependencyAssessment>(),
            BaseTime.AddSeconds(16));

        Require(result.Assessment.HealthState == HealthState.Unknown, "looser configuration extended HFP-FAST");
        Require(string.Equals(result.Assessment.ReasonCode, "STALE_REQUIRED_EVIDENCE", StringComparison.Ordinal), "looser configuration stale reason mismatch");
    }

    private static void VerifyConfiguredFreshnessCanTightenProfile()
    {
        var rule = CreateRule(HealthFreshnessProfile.Fast, TimeSpan.FromSeconds(5));
        var result = HealthObservationAssessmentRuntime.Evaluate(
            rule,
            CreateObservations(BaseTime, BaseTime.AddMinutes(1)),
            Array.Empty<HealthDependencyAssessment>(),
            BaseTime.AddSeconds(6));

        Require(result.Assessment.HealthState == HealthState.Unknown, "stricter configured freshness did not tighten HFP-FAST");
        Require(string.Equals(result.Assessment.ReasonCode, "STALE_REQUIRED_EVIDENCE", StringComparison.Ordinal), "stricter configuration stale reason mismatch");
    }

    private static void VerifyMissingRequiredEvidenceFailsClosed()
    {
        var observations = CreateObservations(BaseTime, BaseTime.AddMinutes(1)).Take(1).ToArray();
        var result = HealthObservationAssessmentRuntime.Evaluate(
            CreateRule(),
            observations,
            Array.Empty<HealthDependencyAssessment>(),
            BaseTime.AddSeconds(1));

        Require(result.Assessment.HealthState == HealthState.Unknown, "missing required evidence did not fail closed to UNKNOWN");
        Require(result.Assessment.EvidenceQuality == EvidenceQuality.Insufficient, "missing required evidence is not EQ-INSUFFICIENT");
        Require(string.Equals(result.Assessment.ReasonCode, "MISSING_REQUIRED_EVIDENCE", StringComparison.Ordinal), "missing required evidence reason mismatch");
    }

    private static void VerifyStaleRequiredEvidenceFailsClosed()
    {
        var result = HealthObservationAssessmentRuntime.Evaluate(
            CreateRule(),
            CreateObservations(BaseTime, BaseTime.AddMinutes(1)),
            Array.Empty<HealthDependencyAssessment>(),
            BaseTime.AddSeconds(16));

        Require(result.Assessment.HealthState == HealthState.Unknown, "stale required evidence did not produce UNKNOWN");
        Require(string.Equals(result.Assessment.ReasonCode, "STALE_REQUIRED_EVIDENCE", StringComparison.Ordinal), "stale evidence reason mismatch");
    }

    private static void VerifyInvalidRequiredEvidenceFailsClosed()
    {
        var result = HealthObservationAssessmentRuntime.Evaluate(
            CreateRule(),
            CreateObservations(BaseTime, BaseTime.AddMinutes(1), primaryProvenanceValid: false),
            Array.Empty<HealthDependencyAssessment>(),
            BaseTime.AddSeconds(1));

        Require(result.Assessment.HealthState == HealthState.Unknown, "provenance-invalid required evidence did not produce UNKNOWN");
        Require(result.Assessment.EvidenceQuality == EvidenceQuality.Invalid, "provenance-invalid required evidence is not EQ-INVALID");
        Require(string.Equals(result.Assessment.ReasonCode, "INVALID_REQUIRED_EVIDENCE", StringComparison.Ordinal), "invalid required evidence reason mismatch");
        Require(!string.Equals(result.Assessment.EvidenceReference, "health:evidence:none", StringComparison.Ordinal), "invalid required evidence was not preserved as failure evidence");
    }

    private static void VerifyContradictoryRequiredEvidenceFailsClosed()
    {
        var observations = CreateObservations(BaseTime, BaseTime.AddMinutes(1)).ToList();
        observations.Add(observations[0] with
        {
            ObservationId = "observation:wp02:primary:conflict",
            EvidenceReference = "evidence:wp02:primary:conflict",
            Condition = HealthObservationCondition.Failed
        });

        var result = HealthObservationAssessmentRuntime.Evaluate(
            CreateRule(),
            observations,
            Array.Empty<HealthDependencyAssessment>(),
            BaseTime.AddSeconds(1));

        Require(result.Assessment.HealthState == HealthState.Unknown, "contradictory required evidence did not produce UNKNOWN");
        Require(string.Equals(result.Assessment.ReasonCode, "CONTRADICTORY_REQUIRED_EVIDENCE", StringComparison.Ordinal), "contradiction reason mismatch");
        Require(!string.Equals(result.Assessment.Contradictions, "NONE", StringComparison.Ordinal), "contradiction identity was not preserved");
    }

    private static void VerifySourceBoundRequiresSourceExpiry()
    {
        var result = HealthObservationAssessmentRuntime.Evaluate(
            CreateRule(HealthFreshnessProfile.SourceBound),
            CreateObservations(BaseTime, null),
            Array.Empty<HealthDependencyAssessment>(),
            BaseTime.AddSeconds(1));

        Require(result.Assessment.HealthState == HealthState.Unknown, "HFP-SOURCE_BOUND accepted evidence without source expiry");
        Require(string.Equals(result.Assessment.ReasonCode, "STALE_REQUIRED_EVIDENCE", StringComparison.Ordinal), "source-bound missing expiry reason mismatch");
    }

    private static void VerifyEventBoundWitnessLossFailsClosed()
    {
        var result = HealthObservationAssessmentRuntime.Evaluate(
            CreateRule(HealthFreshnessProfile.EventBound, usesIndependentEventWitness: true),
            CreateObservations(BaseTime, BaseTime.AddMinutes(10), eventWitnessCurrent: false),
            Array.Empty<HealthDependencyAssessment>(),
            BaseTime.AddSeconds(1));

        Require(result.Assessment.HealthState == HealthState.Unknown, "HFP-EVENT_BOUND witness loss did not produce UNKNOWN");
        Require(string.Equals(result.Assessment.ReasonCode, "STALE_REQUIRED_EVIDENCE", StringComparison.Ordinal), "event witness loss reason mismatch");
    }

    private static void VerifyEventBoundWithoutWitnessFallsBackToSlow()
    {
        var result = HealthObservationAssessmentRuntime.Evaluate(
            CreateRule(HealthFreshnessProfile.EventBound, usesIndependentEventWitness: false),
            CreateObservations(BaseTime, BaseTime.AddMinutes(10)),
            Array.Empty<HealthDependencyAssessment>(),
            BaseTime.AddSeconds(301));

        Require(result.Assessment.HealthState == HealthState.Unknown, "HFP-EVENT_BOUND without witness exceeded HFP-SLOW fallback but stayed positive");
        Require(string.Equals(result.Assessment.ReasonCode, "STALE_REQUIRED_EVIDENCE", StringComparison.Ordinal), "event fallback stale reason mismatch");
    }

    private static void VerifyMonitorVisibilityLossFailsClosed()
    {
        var result = HealthObservationAssessmentRuntime.Evaluate(
            CreateRule(),
            CreateObservations(BaseTime, BaseTime.AddMinutes(1), primaryVisible: false),
            Array.Empty<HealthDependencyAssessment>(),
            BaseTime.AddSeconds(1));

        Require(result.Assessment.HealthState == HealthState.Unknown, "monitor-source visibility loss did not produce UNKNOWN");
        Require(string.Equals(result.Assessment.ReasonCode, "MONITOR_VISIBILITY_LOST", StringComparison.Ordinal), "monitor visibility reason mismatch");
    }

    private static void VerifyRequiredFailureProducesUnhealthy()
    {
        var result = HealthObservationAssessmentRuntime.Evaluate(
            CreateRule(),
            CreateObservations(BaseTime, BaseTime.AddMinutes(1), primaryCondition: HealthObservationCondition.Failed),
            Array.Empty<HealthDependencyAssessment>(),
            BaseTime.AddSeconds(1));

        Require(result.Assessment.HealthState == HealthState.Unhealthy, "positively evidenced required failure did not produce UNHEALTHY");
        Require(string.Equals(result.Assessment.ReasonCode, "REQUIRED_CONDITION_FAILED", StringComparison.Ordinal), "required failure reason mismatch");
    }

    private static void VerifyKnownRequiredDegradationProducesDegraded()
    {
        var result = HealthObservationAssessmentRuntime.Evaluate(
            CreateRule(),
            CreateObservations(BaseTime, BaseTime.AddMinutes(1), primaryCondition: HealthObservationCondition.Degraded),
            Array.Empty<HealthDependencyAssessment>(),
            BaseTime.AddSeconds(1));

        Require(result.Assessment.HealthState == HealthState.Degraded, "known bounded required degradation did not produce DEGRADED");
        Require(string.Equals(result.Assessment.ReasonCode, "KNOWN_BOUNDED_DEGRADATION", StringComparison.Ordinal), "degradation reason mismatch");
    }

    private static void VerifyRequiredDependencyFailureIsPreserved()
    {
        var rule = CreateRule(dependencies: new[]
        {
            new HealthDependencyRequirement(
                "dependency:wp02:critical",
                "foundation.dependency.capability",
                HealthDependencyCriticality.Required,
                false)
        });

        var result = HealthObservationAssessmentRuntime.Evaluate(
            rule,
            CreateObservations(BaseTime, BaseTime.AddMinutes(1)),
            new[] { CreateDependency(HealthState.Unhealthy) },
            BaseTime.AddSeconds(1));

        Require(result.Assessment.HealthState == HealthState.Unhealthy, "required dependency failure was hidden by healthy sibling evidence");
        Require(string.Equals(result.Assessment.ReducedByDependencyId, "dependency:wp02:critical", StringComparison.Ordinal), "critical dependency identity was not retained");
        Require(string.Equals(result.Assessment.ReasonCode, "REQUIRED_DEPENDENCY_UNHEALTHY", StringComparison.Ordinal), "required dependency failure reason mismatch");
    }

    private static void VerifyRequiredDependencyUnknownIsPreserved()
    {
        var rule = CreateRule(dependencies: new[]
        {
            new HealthDependencyRequirement(
                "dependency:wp02:critical",
                "foundation.dependency.capability",
                HealthDependencyCriticality.Required,
                false)
        });

        var result = HealthObservationAssessmentRuntime.Evaluate(
            rule,
            CreateObservations(BaseTime, BaseTime.AddMinutes(1)),
            new[] { CreateDependency(HealthState.Unknown) },
            BaseTime.AddSeconds(1));

        Require(result.Assessment.HealthState == HealthState.Unknown, "required UNKNOWN dependency did not force aggregate UNKNOWN");
        Require(string.Equals(result.Assessment.ReasonCode, "REQUIRED_DEPENDENCY_UNKNOWN", StringComparison.Ordinal), "required dependency unknown reason mismatch");
    }

    private static void VerifyDegradableDependencyRequiresProvenMode()
    {
        var rule = CreateRule(dependencies: new[]
        {
            new HealthDependencyRequirement(
                "dependency:wp02:critical",
                "foundation.dependency.capability",
                HealthDependencyCriticality.Degradable,
                true)
        });

        var result = HealthObservationAssessmentRuntime.Evaluate(
            rule,
            CreateObservations(BaseTime, BaseTime.AddMinutes(1)),
            new[] { CreateDependency(HealthState.Unhealthy, independentModeEvidenceValid: false) },
            BaseTime.AddSeconds(1));

        Require(result.Assessment.HealthState == HealthState.Unhealthy, "degradable dependency without independent mode proof was optimistically degraded");
        Require(string.Equals(result.Assessment.ReasonCode, "DEGRADABLE_DEPENDENCY_UNHEALTHY_WITHOUT_PROVEN_MODE", StringComparison.Ordinal), "unproven degradable dependency reason mismatch");
    }

    private static void VerifyDegradableDependencyWithProvenModeDegrades()
    {
        var rule = CreateRule(dependencies: new[]
        {
            new HealthDependencyRequirement(
                "dependency:wp02:critical",
                "foundation.dependency.capability",
                HealthDependencyCriticality.Degradable,
                true)
        });

        var result = HealthObservationAssessmentRuntime.Evaluate(
            rule,
            CreateObservations(BaseTime, BaseTime.AddMinutes(1)),
            new[] { CreateDependency(HealthState.Unhealthy, independentModeEvidenceValid: true) },
            BaseTime.AddSeconds(1));

        Require(result.Assessment.HealthState == HealthState.Degraded, "proven degradable dependency mode did not produce DEGRADED");
        Require(result.Assessment.EvidenceQuality == EvidenceQuality.Limited, "proven degradable dependency mode did not produce EQ-LIMITED");
    }

    private static void VerifyApplicableRuleWithoutRequiredEvidenceRejected()
    {
        var baseRule = CreateRule();
        var rule = baseRule with
        {
            EvidenceRequirements = new[]
            {
                new HealthEvidenceRequirement(
                    "requirement:supporting-only",
                    HealthDimension.Performance,
                    HealthEvidenceRole.Supporting,
                    "source:supporting",
                    "Foundation Supporting Evidence Authority")
            }
        };

        var rejected = false;
        try
        {
            _ = HealthObservationAssessmentRuntime.Evaluate(
                rule,
                Array.Empty<HealthObservation>(),
                Array.Empty<HealthDependencyAssessment>(),
                BaseTime.AddSeconds(1));
        }
        catch (ArgumentException)
        {
            rejected = true;
        }

        Require(rejected, "applicable rule without required evidence was allowed to evaluate");
    }

    private static void VerifyLimitedSupportingEvidenceCannotProduceHealthy()
    {
        var baseRule = CreateRule();
        var rule = baseRule with
        {
            EvidenceRequirements = baseRule.EvidenceRequirements
                .Concat(new[]
                {
                    new HealthEvidenceRequirement(
                        "requirement:supporting-performance",
                        HealthDimension.Performance,
                        HealthEvidenceRole.Supporting,
                        "source:supporting",
                        "Foundation Supporting Evidence Authority")
                })
                .ToArray()
        };

        var observations = CreateObservations(BaseTime, BaseTime.AddMinutes(1)).ToList();
        observations.Add(new HealthObservation(
            "observation:wp02:supporting-stale",
            "requirement:supporting-performance",
            "foundation.health.subject:wp02",
            "foundation.technical.health",
            HealthDimension.Performance,
            "source:supporting",
            "Foundation Supporting Evidence Authority",
            "evidence:wp02:supporting-stale",
            HealthObservationCondition.Satisfied,
            BaseTime.AddSeconds(-30),
            BaseTime.AddMinutes(1),
            true,
            true,
            true,
            true,
            true,
            true));

        var result = HealthObservationAssessmentRuntime.Evaluate(
            rule,
            observations,
            Array.Empty<HealthDependencyAssessment>(),
            BaseTime.AddSeconds(1));

        Require(result.Assessment.HealthState == HealthState.Unknown, "EQ-LIMITED supporting evidence still produced HEALTHY");
        Require(result.Assessment.EvidenceQuality == EvidenceQuality.Limited, "limited supporting evidence did not remain EQ-LIMITED");
        Require(string.Equals(result.Assessment.ReasonCode, "NON_REQUIRED_EVIDENCE_LIMITED", StringComparison.Ordinal), "limited supporting evidence reason mismatch");
    }

    private static void VerifyRequiredDependencyLimitedHealthyCannotProduceHealthy()
    {
        var rule = CreateRule(dependencies: new[]
        {
            new HealthDependencyRequirement(
                "dependency:wp02:critical",
                "foundation.dependency.capability",
                HealthDependencyCriticality.Required,
                false)
        });

        var result = HealthObservationAssessmentRuntime.Evaluate(
            rule,
            CreateObservations(BaseTime, BaseTime.AddMinutes(1)),
            new[] { CreateDependency(HealthState.Healthy, EvidenceQuality.Limited) },
            BaseTime.AddSeconds(1));

        Require(result.Assessment.HealthState == HealthState.Unknown, "required dependency HEALTHY/EQ-LIMITED produced aggregate HEALTHY");
        Require(result.Assessment.EvidenceQuality == EvidenceQuality.Limited, "required dependency limited quality was not preserved");
        Require(string.Equals(result.Assessment.ReasonCode, "REQUIRED_DEPENDENCY_EVIDENCE_LIMITED", StringComparison.Ordinal), "required dependency limited reason mismatch");
    }

    private static void VerifyRequiredDependencyNotApplicableFailsClosed()
    {
        var rule = CreateRule(dependencies: new[]
        {
            new HealthDependencyRequirement(
                "dependency:wp02:critical",
                "foundation.dependency.capability",
                HealthDependencyCriticality.Required,
                false)
        });

        var result = HealthObservationAssessmentRuntime.Evaluate(
            rule,
            CreateObservations(BaseTime, BaseTime.AddMinutes(1)),
            new[] { CreateDependency(HealthState.NotApplicable) },
            BaseTime.AddSeconds(1));

        Require(result.Assessment.HealthState == HealthState.Unknown, "required NOT_APPLICABLE dependency was treated as satisfied");
        Require(string.Equals(result.Assessment.ReasonCode, "REQUIRED_DEPENDENCY_NOT_APPLICABLE", StringComparison.Ordinal), "required NOT_APPLICABLE dependency reason mismatch");
    }

    private static void VerifyDuplicateDependencyEvidenceFailsClosed()
    {
        var rule = CreateRule(dependencies: new[]
        {
            new HealthDependencyRequirement(
                "dependency:wp02:critical",
                "foundation.dependency.capability",
                HealthDependencyCriticality.Required,
                false)
        });

        var first = CreateDependency(HealthState.Healthy);
        var second = first with
        {
            HealthState = HealthState.Unhealthy,
            EvidenceReference = "evidence:wp02:dependency:conflict"
        };

        var result = HealthObservationAssessmentRuntime.Evaluate(
            rule,
            CreateObservations(BaseTime, BaseTime.AddMinutes(1)),
            new[] { first, second },
            BaseTime.AddSeconds(1));

        Require(result.Assessment.HealthState == HealthState.Unknown, "duplicate dependency evidence did not fail closed");
        Require(string.Equals(result.Assessment.ReasonCode, "CONTRADICTORY_DEPENDENCY_EVIDENCE", StringComparison.Ordinal), "duplicate dependency contradiction reason mismatch");
        Require(!string.Equals(result.Assessment.Contradictions, "NONE", StringComparison.Ordinal), "duplicate dependency contradiction was not preserved");
    }

    private static void VerifyFutureDatedRequiredDependencyFailsClosed()
    {
        var rule = CreateRule(dependencies: new[]
        {
            new HealthDependencyRequirement(
                "dependency:wp02:critical",
                "foundation.dependency.capability",
                HealthDependencyCriticality.Required,
                false)
        });

        var future = CreateDependency(HealthState.Healthy) with
        {
            ObservationTime = BaseTime.AddSeconds(2),
            Expiry = BaseTime.AddMinutes(1)
        };

        var result = HealthObservationAssessmentRuntime.Evaluate(
            rule,
            CreateObservations(BaseTime, BaseTime.AddMinutes(1)),
            new[] { future },
            BaseTime.AddSeconds(1));

        Require(result.Assessment.HealthState == HealthState.Unknown, "future-dated required dependency evidence supported a current positive assessment");
        Require(result.Assessment.EvidenceQuality == EvidenceQuality.Insufficient, "future-dated dependency did not produce EQ-INSUFFICIENT");
        Require(string.Equals(result.Assessment.ReasonCode, "DEPENDENCY_EVIDENCE_FUTURE_DATED", StringComparison.Ordinal), "future-dated dependency reason mismatch");
        Require(!string.Equals(result.Assessment.EvidenceReference, "health:evidence:none", StringComparison.Ordinal), "future-dated dependency failure evidence was not preserved");
    }

    private static void VerifyDependencyFailureEvidenceBindsLocalAndDependencyEvidence()
    {
        var rule = CreateRule(dependencies: new[]
        {
            new HealthDependencyRequirement(
                "dependency:wp02:critical",
                "foundation.dependency.capability",
                HealthDependencyCriticality.Required,
                false)
        });

        var dependency = CreateDependency(HealthState.Healthy) with
        {
            Expiry = BaseTime.AddMilliseconds(500)
        };

        var dependencyMutated = dependency with
        {
            EvidenceReference = "evidence:wp02:dependency:mutated"
        };

        var observations = CreateObservations(BaseTime, BaseTime.AddMinutes(1));
        var localMutated = CreateObservations(BaseTime, BaseTime.AddMinutes(1));
        localMutated[0] = localMutated[0] with
        {
            EvidenceReference = "evidence:wp02:primary:provenance-mutated"
        };

        var first = HealthObservationAssessmentRuntime.Evaluate(
            rule,
            observations,
            new[] { dependency },
            BaseTime.AddSeconds(1));

        var second = HealthObservationAssessmentRuntime.Evaluate(
            rule,
            observations,
            new[] { dependencyMutated },
            BaseTime.AddSeconds(1));

        var third = HealthObservationAssessmentRuntime.Evaluate(
            rule,
            localMutated,
            new[] { dependency },
            BaseTime.AddSeconds(1));

        Require(first.Assessment.HealthState == HealthState.Unknown, "stale dependency did not fail closed");
        Require(string.Equals(first.Assessment.ReasonCode, "DEPENDENCY_EVIDENCE_STALE_OR_INVALID", StringComparison.Ordinal), "stale dependency provenance test reason mismatch");
        Require(!string.Equals(first.Assessment.EvidenceReference, second.Assessment.EvidenceReference, StringComparison.Ordinal), "dependency evidence mutation did not change aggregate evidence identity");
        Require(!string.Equals(first.Assessment.EvidenceReference, third.Assessment.EvidenceReference, StringComparison.Ordinal), "local evidence mutation did not change dependency-reduced aggregate evidence identity");
        Require(!string.Equals(first.Assessment.AssessmentId, second.Assessment.AssessmentId, StringComparison.Ordinal), "dependency evidence mutation did not change assessment identity");
        Require(!string.Equals(first.Assessment.AssessmentId, third.Assessment.AssessmentId, StringComparison.Ordinal), "local evidence mutation did not remain bound after dependency reduction");
    }

    private static void VerifySupportingContradictionIsExplicit()
    {
        var baseRule = CreateRule();
        var rule = baseRule with
        {
            EvidenceRequirements = baseRule.EvidenceRequirements
                .Concat(new[]
                {
                    new HealthEvidenceRequirement(
                        "requirement:supporting-contradiction",
                        HealthDimension.Performance,
                        HealthEvidenceRole.Supporting,
                        "source:supporting",
                        "Foundation Supporting Evidence Authority")
                })
                .ToArray()
        };

        var observations = CreateObservations(BaseTime, BaseTime.AddMinutes(1)).ToList();

        observations.Add(new HealthObservation(
            "observation:wp02:supporting:positive",
            "requirement:supporting-contradiction",
            "foundation.health.subject:wp02",
            "foundation.technical.health",
            HealthDimension.Performance,
            "source:supporting",
            "Foundation Supporting Evidence Authority",
            "evidence:wp02:supporting:positive",
            HealthObservationCondition.Satisfied,
            BaseTime,
            BaseTime.AddMinutes(1),
            true,
            true,
            true,
            true,
            true,
            true));

        observations.Add(new HealthObservation(
            "observation:wp02:supporting:negative",
            "requirement:supporting-contradiction",
            "foundation.health.subject:wp02",
            "foundation.technical.health",
            HealthDimension.Performance,
            "source:supporting",
            "Foundation Supporting Evidence Authority",
            "evidence:wp02:supporting:negative",
            HealthObservationCondition.Failed,
            BaseTime,
            BaseTime.AddMinutes(1),
            true,
            true,
            true,
            true,
            true,
            true));

        var result = HealthObservationAssessmentRuntime.Evaluate(
            rule,
            observations,
            Array.Empty<HealthDependencyAssessment>(),
            BaseTime.AddSeconds(1));

        Require(result.Assessment.HealthState == HealthState.Unknown, "supporting contradiction did not produce explicit uncertainty");
        Require(result.Assessment.EvidenceQuality == EvidenceQuality.Limited, "supporting contradiction did not preserve bounded EQ-LIMITED qualification");
        Require(string.Equals(result.Assessment.ReasonCode, "CONTRADICTORY_NON_REQUIRED_EVIDENCE", StringComparison.Ordinal), "supporting contradiction reason mismatch");
        Require(!string.Equals(result.Assessment.Contradictions, "NONE", StringComparison.Ordinal), "supporting contradiction identity was not exposed");
    }

    private static void VerifyMaterialTransitionOutput()
    {
        var result = HealthObservationAssessmentRuntime.Evaluate(
            CreateRule(),
            CreateObservations(BaseTime, BaseTime.AddMinutes(1), primaryCondition: HealthObservationCondition.Failed),
            Array.Empty<HealthDependencyAssessment>(),
            BaseTime.AddSeconds(1),
            HealthState.Healthy);

        var transition = result.Transition
            ?? throw new InvalidOperationException("material Health state change did not emit transition output");

        Require(transition.From == HealthState.Healthy, "transition source state mismatch");
        Require(transition.To == HealthState.Unhealthy, "transition target state mismatch");
        Require(string.Equals(transition.AssessmentId, result.Assessment.AssessmentId, StringComparison.Ordinal), "transition is not bound to assessment identity");
    }

    private static void VerifyNoTransitionWhenStateUnchanged()
    {
        var result = HealthObservationAssessmentRuntime.Evaluate(
            CreateRule(),
            CreateObservations(BaseTime, BaseTime.AddMinutes(1)),
            Array.Empty<HealthDependencyAssessment>(),
            BaseTime.AddSeconds(1),
            HealthState.Healthy);

        Require(result.Transition is null, "unchanged Health state emitted a material transition");
    }

    private static void VerifyGovernedNotApplicable()
    {
        var result = HealthObservationAssessmentRuntime.Evaluate(
            CreateRule(applicable: false),
            Array.Empty<HealthObservation>(),
            Array.Empty<HealthDependencyAssessment>(),
            BaseTime.AddSeconds(1));

        Require(result.Assessment.HealthState == HealthState.NotApplicable, "governed non-applicability did not produce NOT_APPLICABLE");
        Require(string.Equals(result.Assessment.ReasonCode, "RULE_NOT_APPLICABLE", StringComparison.Ordinal), "NOT_APPLICABLE reason mismatch");
    }

    private static void VerifyNoAuthorityOrRecoveryActionSurface()
    {
        var assembly = typeof(HealthObservationAssessmentRuntime).Assembly;
        var forbidden = new[]
        {
            "GrantAuthority",
            "AuthorizeAction",
            "IssueGuardianCommand",
            "RestrictSubject",
            "ReleaseSubject",
            "LifecycleTransition",
            "ExecuteRecovery",
            "AcceptRecovery",
            "PublishEvent"
        };

        foreach (var type in assembly.GetExportedTypes())
        {
            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                Require(!forbidden.Contains(method.Name, StringComparer.Ordinal), $"action authority leaked into Stage 7 Health runtime: {type.FullName}.{method.Name}");
            }
        }
    }

    private static void VerifyNoApplicationBusinessDependency()
    {
        var references = typeof(HealthObservationAssessmentRuntime).Assembly
            .GetReferencedAssemblies()
            .Select(reference => reference.Name ?? string.Empty)
            .ToArray();

        Require(references.Contains("Foundation.Contracts", StringComparer.Ordinal), "Foundation.Contracts dependency missing from Health runtime");
        Require(!references.Any(reference => reference.Contains("Application", StringComparison.OrdinalIgnoreCase)), "Application dependency leaked into Stage 7 Health runtime");
        Require(!references.Any(reference => reference.Contains("Trading", StringComparison.OrdinalIgnoreCase)), "Trading dependency leaked into Stage 7 Health runtime");
        Require(!references.Any(reference => reference.Contains("Market", StringComparison.OrdinalIgnoreCase)), "Market dependency leaked into Stage 7 Health runtime");
        Require(!references.Any(reference => reference.Contains("Portfolio", StringComparison.OrdinalIgnoreCase)), "Portfolio dependency leaked into Stage 7 Health runtime");
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
}
