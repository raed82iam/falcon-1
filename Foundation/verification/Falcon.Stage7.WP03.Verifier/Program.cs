using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Foundation.HealthFitness;
using Foundation.SelfAwareness;

namespace Falcon.Stage7.WP03.Verifier;

internal static class Program
{
    private static readonly DateTimeOffset BaseTime =
        new(
            2026,
            8,
            12,
            18,
            0,
            0,
            TimeSpan.Zero);

    private static int Main()
    {
        try
        {
            VerifyCompleteZeroApplicationModel();
            VerifyDeterministicInputOrdering();
            VerifyMissingRequiredAreaFailsClosed();
            VerifyLastKnownOnlyCannotSatisfyCurrentCoverage();
            VerifyCurrentUnknownPlusLastKnownPreserved();
            VerifyExplicitUnknownPreserved();
            VerifyAssertionKindsRemainDistinct();
            VerifySameValueDifferentKindIsNotFalseContradiction();
            VerifyTemporalViewsRemainDistinct();
            VerifyExpiredCurrentRejected();
            VerifyFutureObservationRejected();
            VerifyCurrentContradictionVisible();
            VerifyMaterialMutationSensitivity();
            VerifyDuplicateAssertionIdentityRejected();
            VerifyHealthProjectionDoesNotReevaluateHealth();
            VerifyMalformedHealthProjectionRejected();
            VerifyNoAuthorityOrFutureStageActionSurface();
            VerifyExactProductionAssemblyBoundary();
            VerifyNoApplicationOrWebBusinessSurface();

            Console.WriteLine(
                "STAGE7_WP03_VERIFIER=PASS");

            return 0;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine(
                "STAGE7_WP03_VERIFIER=FAIL");

            Console.Error.WriteLine(exception);

            return 1;
        }
    }

    private static void VerifyCompleteZeroApplicationModel()
    {
        var assertions =
            BuildCompleteAssertions(BaseTime);

        var model =
            BuildModel(
                assertions,
                BaseTime);

        Require(
            model.Assertions.Count >=
                Enum.GetValues<FoundationSelfModelArea>()
                    .Length,
            "Complete Foundation Self Model did not preserve required coverage.");

        Require(
            model.Assertions.All(assertion =>
                !assertion.SubjectId.Contains(
                    "application",
                    StringComparison.OrdinalIgnoreCase) &&
                !assertion.SubjectId.Contains(
                    "web",
                    StringComparison.OrdinalIgnoreCase)),
            "Zero-Application model unexpectedly requires Application or Web identity.");

        Require(
            model.EvidenceReference.StartsWith(
                "selfmodel:evidence:sha256:",
                StringComparison.Ordinal),
            "Self Model evidence identity missing.");

        Require(
            !string.IsNullOrWhiteSpace(
                model.Identity),
            "Self Model identity missing.");
    }

    private static void VerifyDeterministicInputOrdering()
    {
        var forward =
            BuildCompleteAssertions(BaseTime);

        var reverse =
            forward
                .Reverse()
                .ToArray();

        var modelA =
            BuildModel(
                forward,
                BaseTime);

        var modelB =
            BuildModel(
                reverse,
                BaseTime);

        RequireEqual(
            modelA.Identity,
            modelB.Identity,
            "Input ordering changed deterministic Self Model identity.");

        RequireEqual(
            modelA.EvidenceReference,
            modelB.EvidenceReference,
            "Input ordering changed deterministic Self Model evidence identity.");
    }

    private static void VerifyMissingRequiredAreaFailsClosed()
    {
        var assertions =
            BuildCompleteAssertions(BaseTime)
                .Where(assertion =>
                    assertion.Area !=
                        FoundationSelfModelArea
                            .BackupCondition)
                .ToArray();

        ExpectThrows<ArgumentException>(
            () =>
                BuildModel(
                    assertions,
                    BaseTime),
            "required current area missing",
            "Missing mandatory Self Model area did not fail closed.");
    }

    private static void VerifyLastKnownOnlyCannotSatisfyCurrentCoverage()
    {
        var source =
            BuildCompleteAssertions(BaseTime);

        var backup =
            source.Single(assertion =>
                assertion.Area ==
                    FoundationSelfModelArea
                        .BackupCondition &&
                assertion.TemporalView ==
                    FoundationSelfModelTemporalView
                        .Current);

        var assertions =
            source
                .Where(assertion =>
                    assertion.AssertionId !=
                        backup.AssertionId)
                .ToList();

        assertions.Add(
            backup with
            {
                AssertionId =
                    "selfmodel:assertion:backup:last-known-only",
                TemporalView =
                    FoundationSelfModelTemporalView
                        .LastKnown,
                ObservationTime =
                    BaseTime.AddSeconds(-30),
                EffectiveTime =
                    BaseTime.AddSeconds(-30),
                Expiry =
                    BaseTime.AddSeconds(-1)
            });

        ExpectThrows<ArgumentException>(
            () =>
                BuildModel(
                    assertions,
                    BaseTime),
            "required current area missing",
            "LAST_KNOWN alone incorrectly satisfied current Self Model coverage.");
    }

    private static void VerifyCurrentUnknownPlusLastKnownPreserved()
    {
        var source =
            BuildCompleteAssertions(BaseTime);

        var backup =
            source.Single(assertion =>
                assertion.Area ==
                    FoundationSelfModelArea
                        .BackupCondition &&
                assertion.TemporalView ==
                    FoundationSelfModelTemporalView
                        .Current);

        var assertions =
            source
                .Where(assertion =>
                    assertion.AssertionId !=
                        backup.AssertionId)
                .ToList();

        assertions.Add(
            backup with
            {
                AssertionId =
                    "selfmodel:assertion:backup:current-unknown",
                AssertionKind =
                    FoundationSelfModelAssertionKind
                        .Unknown,
                ValueIdentity =
                    "technical:value:unknown:backupcondition",
                EvidenceQuality =
                    EvidenceQuality.Insufficient,
                Confidence =
                    "INSUFFICIENT",
                Uncertainty =
                    "current-backup-evidence-unavailable"
            });

        assertions.Add(
            backup with
            {
                AssertionId =
                    "selfmodel:assertion:backup:last-known",
                TemporalView =
                    FoundationSelfModelTemporalView
                        .LastKnown,
                ObservationTime =
                    BaseTime.AddSeconds(-30),
                EffectiveTime =
                    BaseTime.AddSeconds(-30),
                Expiry =
                    BaseTime.AddSeconds(-1)
            });

        var model =
            BuildModel(
                assertions,
                BaseTime);

        var current =
            model.Assertions.Single(assertion =>
                assertion.Area ==
                    FoundationSelfModelArea
                        .BackupCondition &&
                assertion.TemporalView ==
                    FoundationSelfModelTemporalView
                        .Current);

        var lastKnown =
            model.Assertions.Single(assertion =>
                assertion.Area ==
                    FoundationSelfModelArea
                        .BackupCondition &&
                assertion.TemporalView ==
                    FoundationSelfModelTemporalView
                        .LastKnown);

        Require(
            current.AssertionKind ==
                FoundationSelfModelAssertionKind
                    .Unknown,
            "Missing current knowledge was not represented as CURRENT UNKNOWN.");

        Require(
            current.EvidenceQuality ==
                EvidenceQuality.Insufficient,
            "CURRENT UNKNOWN did not preserve insufficient evidence.");

        Require(
            lastKnown.AssertionKind ==
                FoundationSelfModelAssertionKind
                    .Fact,
            "LAST_KNOWN trustworthy fact was not preserved.");

        Require(
            lastKnown.Expiry < BaseTime,
            "LAST_KNOWN age/expiry was not preserved.");
    }

    private static void VerifyExplicitUnknownPreserved()
    {
        var assertions =
            BuildCompleteAssertions(BaseTime);

        var model =
            BuildModel(
                assertions,
                BaseTime);

        var technicalFitness =
            model.Assertions.Single(assertion =>
                assertion.Area ==
                    FoundationSelfModelArea
                        .TechnicalFitness &&
                assertion.TemporalView ==
                    FoundationSelfModelTemporalView
                        .Current);

        Require(
            technicalFitness.AssertionKind ==
                FoundationSelfModelAssertionKind
                    .Unknown,
            "Technical Fitness was prematurely converted from explicit UNKNOWN.");

        Require(
            technicalFitness.EvidenceQuality ==
                EvidenceQuality.Insufficient,
            "Explicit UNKNOWN evidence quality was not preserved.");

        var invalid =
            assertions
                .Select(assertion =>
                    assertion.Area ==
                        FoundationSelfModelArea
                            .TechnicalFitness
                        ? assertion with
                        {
                            EvidenceQuality =
                                EvidenceQuality
                                    .Sufficient
                        }
                        : assertion)
                .ToArray();

        ExpectThrows<ArgumentException>(
            () =>
                BuildModel(
                    invalid,
                    BaseTime),
            "unknown assertion cannot claim sufficient evidence",
            "UNKNOWN assertion was allowed to claim sufficient evidence.");
    }

    private static void VerifyAssertionKindsRemainDistinct()
    {
        var assertions =
            BuildCompleteAssertions(BaseTime);

        assertions =
            ReplaceArea(
                assertions,
                FoundationSelfModelArea
                    .FoundationIdentity,
                assertion =>
                    assertion with
                    {
                        AssertionKind =
                            FoundationSelfModelAssertionKind
                                .Fact
                    });

        assertions =
            ReplaceArea(
                assertions,
                FoundationSelfModelArea
                    .AdmittedBaseline,
                assertion =>
                    assertion with
                    {
                        AssertionKind =
                            FoundationSelfModelAssertionKind
                                .Estimate
                    });

        assertions =
            ReplaceArea(
                assertions,
                FoundationSelfModelArea
                    .CoreComponentIdentity,
                assertion =>
                    assertion with
                    {
                        AssertionKind =
                            FoundationSelfModelAssertionKind
                                .Assumption
                    });

        assertions =
            ReplaceArea(
                assertions,
                FoundationSelfModelArea
                    .CoreComponentVersion,
                assertion =>
                    assertion with
                    {
                        AssertionKind =
                            FoundationSelfModelAssertionKind
                                .Interpretation
                    });

        assertions =
            ReplaceArea(
                assertions,
                FoundationSelfModelArea
                    .LifecycleCondition,
                assertion =>
                    assertion with
                    {
                        AssertionKind =
                            FoundationSelfModelAssertionKind
                                .Unknown,
                        EvidenceQuality =
                            EvidenceQuality
                                .Insufficient,
                        Confidence =
                            "INSUFFICIENT",
                        Uncertainty =
                            "explicit-lifecycle-uncertainty"
                    });

        var model =
            BuildModel(
                assertions,
                BaseTime);

        RequireKind(
            model,
            FoundationSelfModelArea
                .FoundationIdentity,
            FoundationSelfModelAssertionKind
                .Fact);

        RequireKind(
            model,
            FoundationSelfModelArea
                .AdmittedBaseline,
            FoundationSelfModelAssertionKind
                .Estimate);

        RequireKind(
            model,
            FoundationSelfModelArea
                .CoreComponentIdentity,
            FoundationSelfModelAssertionKind
                .Assumption);

        RequireKind(
            model,
            FoundationSelfModelArea
                .CoreComponentVersion,
            FoundationSelfModelAssertionKind
                .Interpretation);

        RequireKind(
            model,
            FoundationSelfModelArea
                .LifecycleCondition,
            FoundationSelfModelAssertionKind
                .Unknown);
    }

    private static void VerifySameValueDifferentKindIsNotFalseContradiction()
    {
        var assertions =
            BuildCompleteAssertions(BaseTime)
                .ToList();

        var source =
            assertions.Single(assertion =>
                assertion.Area ==
                    FoundationSelfModelArea
                        .FoundationIdentity &&
                assertion.TemporalView ==
                    FoundationSelfModelTemporalView
                        .Current);

        assertions.Add(
            source with
            {
                AssertionId =
                    "selfmodel:assertion:foundationidentity:estimate",
                AssertionKind =
                    FoundationSelfModelAssertionKind
                        .Estimate
            });

        var model =
            BuildModel(
                assertions,
                BaseTime);

        Require(
            !model.Contradictions.Any(value =>
                value.Area ==
                    FoundationSelfModelArea
                        .FoundationIdentity),
            "Same technical value with a different epistemic kind created a false contradiction.");

        Require(
            model.Assertions.Count(assertion =>
                assertion.Area ==
                    FoundationSelfModelArea
                        .FoundationIdentity) == 2,
            "Epistemically distinct assertions were not both preserved.");
    }

    private static void VerifyTemporalViewsRemainDistinct()
    {
        var assertions =
            BuildCompleteAssertions(BaseTime)
                .ToList();

        var lastKnown =
            assertions.Single(assertion =>
                assertion.Area ==
                    FoundationSelfModelArea
                        .BackupCondition);

        assertions.Add(
            lastKnown with
            {
                AssertionId =
                    "selfmodel:assertion:backup:last-known-view",
                TemporalView =
                    FoundationSelfModelTemporalView
                        .LastKnown,
                EffectiveTime =
                    BaseTime.AddSeconds(-20),
                ObservationTime =
                    BaseTime.AddSeconds(-20),
                Expiry =
                    BaseTime.AddSeconds(-1)
            });

        var expected =
            assertions.Single(assertion =>
                assertion.Area ==
                    FoundationSelfModelArea
                        .ResourceCapacity &&
                assertion.TemporalView ==
                    FoundationSelfModelTemporalView
                        .Current);

        assertions.Add(
            expected with
            {
                AssertionId =
                    "selfmodel:assertion:resource-capacity:expected",
                TemporalView =
                    FoundationSelfModelTemporalView
                        .Expected,
                EffectiveTime =
                    BaseTime.AddSeconds(10),
                Expiry =
                    BaseTime.AddSeconds(40)
            });

        var desired =
            assertions.Single(assertion =>
                assertion.Area ==
                    FoundationSelfModelArea
                        .ConfigurationIntegrity &&
                assertion.TemporalView ==
                    FoundationSelfModelTemporalView
                        .Current);

        assertions.Add(
            desired with
            {
                AssertionId =
                    "selfmodel:assertion:configuration:desired",
                TemporalView =
                    FoundationSelfModelTemporalView
                        .Desired,
                EffectiveTime =
                    BaseTime.AddSeconds(15),
                Expiry =
                    BaseTime.AddSeconds(45)
            });

        var historical =
            assertions.Single(assertion =>
                assertion.Area ==
                    FoundationSelfModelArea
                        .IncidentCondition &&
                assertion.TemporalView ==
                    FoundationSelfModelTemporalView
                        .Current);

        assertions.Add(
            historical with
            {
                AssertionId =
                    "selfmodel:assertion:incident:historical",
                TemporalView =
                    FoundationSelfModelTemporalView
                        .Historical,
                EffectiveTime =
                    BaseTime.AddSeconds(-30),
                ObservationTime =
                    BaseTime.AddSeconds(-30),
                Expiry =
                    BaseTime.AddSeconds(-10)
            });

        var model =
            BuildModel(
                assertions,
                BaseTime);

        foreach (
            var temporalView in
            Enum.GetValues<
                FoundationSelfModelTemporalView>())
        {
            Require(
                model.Assertions.Any(assertion =>
                    assertion.TemporalView ==
                        temporalView),
                "Self Model lost temporal awareness view: " +
                temporalView);
        }
    }

    private static void VerifyExpiredCurrentRejected()
    {
        var assertions =
            BuildCompleteAssertions(BaseTime);

        assertions =
            ReplaceArea(
                assertions,
                FoundationSelfModelArea
                    .BackupCondition,
                assertion =>
                    assertion with
                    {
                        Expiry = BaseTime
                    });

        ExpectThrows<ArgumentException>(
            () =>
                BuildModel(
                    assertions,
                    BaseTime),
            "current assertion is not current",
            "Expired assertion masqueraded as CURRENT.");
    }

    private static void VerifyFutureObservationRejected()
    {
        var assertions =
            BuildCompleteAssertions(BaseTime);

        assertions =
            ReplaceArea(
                assertions,
                FoundationSelfModelArea
                    .RuntimeCondition,
                assertion =>
                    assertion with
                    {
                        ObservationTime =
                            BaseTime.AddSeconds(1)
                    });

        ExpectThrows<ArgumentException>(
            () =>
                BuildModel(
                    assertions,
                    BaseTime),
            "future observation rejected",
            "Future-dated Self Model observation was accepted.");
    }

    private static void VerifyCurrentContradictionVisible()
    {
        var assertions =
            BuildCompleteAssertions(BaseTime)
                .ToList();

        var source =
            assertions.Single(assertion =>
                assertion.Area ==
                    FoundationSelfModelArea
                        .ResourcePressure &&
                assertion.TemporalView ==
                    FoundationSelfModelTemporalView
                        .Current);

        assertions.Add(
            source with
            {
                AssertionId =
                    "selfmodel:assertion:resource-pressure:conflict",
                ValueIdentity =
                    "technical:value:resource-pressure:conflicting"
            });

        var model =
            BuildModel(
                assertions,
                BaseTime);

        var contradiction =
            model.Contradictions
                .SingleOrDefault(value =>
                    value.Area ==
                        FoundationSelfModelArea
                            .ResourcePressure);

        Require(
            contradiction is not null,
            "Conflicting current assertions were silently collapsed.");

        Require(
            contradiction!.AssertionIds.Count == 2,
            "Contradiction did not preserve both conflicting assertion identities.");

        Require(
            model.Assertions.Count(assertion =>
                assertion.Area ==
                    FoundationSelfModelArea
                        .ResourcePressure &&
                assertion.TemporalView ==
                    FoundationSelfModelTemporalView
                        .Current) == 2,
            "Conflicting current assertions were not both preserved.");
    }

    private static void VerifyMaterialMutationSensitivity()
    {
        var baselineAssertions =
            BuildCompleteAssertions(BaseTime);

        var baseline =
            BuildModel(
                baselineAssertions,
                BaseTime);

        var target =
            baselineAssertions.Single(assertion =>
                assertion.Area ==
                    FoundationSelfModelArea
                        .HealthCondition &&
                assertion.TemporalView ==
                    FoundationSelfModelTemporalView
                        .Current);

        AssertMutationChangesIdentity(
            "source identity",
            baselineAssertions,
            target,
            assertion =>
                assertion with
                {
                    AuthoritativeSourceId =
                        "source:health:mutated"
                },
            baseline.Identity);

        AssertMutationChangesIdentity(
            "source owner",
            baselineAssertions,
            target,
            assertion =>
                assertion with
                {
                    SourceOwner =
                        "owner:foundation:health:mutated"
                },
            baseline.Identity);

        AssertMutationChangesIdentity(
            "evidence reference",
            baselineAssertions,
            target,
            assertion =>
                assertion with
                {
                    EvidenceReference =
                        "health:evidence:mutated"
                },
            baseline.Identity);

        AssertMutationChangesIdentity(
            "value identity",
            baselineAssertions,
            target,
            assertion =>
                assertion with
                {
                    ValueIdentity =
                        "health-state:degraded"
                },
            baseline.Identity);

        AssertMutationChangesIdentity(
            "observation time",
            baselineAssertions,
            target,
            assertion =>
                assertion with
                {
                    ObservationTime =
                        assertion.ObservationTime
                            .AddMilliseconds(-1)
                },
            baseline.Identity);

        AssertMutationChangesIdentity(
            "effective time",
            baselineAssertions,
            target,
            assertion =>
                assertion with
                {
                    EffectiveTime =
                        assertion.EffectiveTime
                            .AddMilliseconds(-1)
                },
            baseline.Identity);

        AssertMutationChangesIdentity(
            "expiry",
            baselineAssertions,
            target,
            assertion =>
                assertion with
                {
                    Expiry =
                        assertion.Expiry
                            .AddMilliseconds(1)
                },
            baseline.Identity);

        AssertMutationChangesIdentity(
            "evidence quality",
            baselineAssertions,
            target,
            assertion =>
                assertion with
                {
                    EvidenceQuality =
                        EvidenceQuality.Limited,
                    Confidence =
                        "LIMITED"
                },
            baseline.Identity);

        AssertMutationChangesIdentity(
            "uncertainty",
            baselineAssertions,
            target,
            assertion =>
                assertion with
                {
                    Uncertainty =
                        assertion.Uncertainty +
                        ";mutation=present"
                },
            baseline.Identity);

        AssertMutationChangesIdentity(
            "freshness reference",
            baselineAssertions,
            target,
            assertion =>
                assertion with
                {
                    FreshnessReference =
                        "freshness:mutated"
                },
            baseline.Identity);

        AssertMutationChangesIdentity(
            "rule identity",
            baselineAssertions,
            target,
            assertion =>
                assertion with
                {
                    RuleId =
                        "health:rule:wp03:mutated"
                },
            baseline.Identity);

        AssertMutationChangesIdentity(
            "rule version",
            baselineAssertions,
            target,
            assertion =>
                assertion with
                {
                    RuleVersion =
                        "1.1"
                },
            baseline.Identity);

        AssertMutationChangesIdentity(
            "source assessment reference",
            baselineAssertions,
            target,
            assertion =>
                assertion with
                {
                    SourceAssessmentReference =
                        "health:assessment:mutated"
                },
            baseline.Identity);

        AssertMutationChangesIdentity(
            "lineage",
            baselineAssertions,
            target,
            assertion =>
                assertion with
                {
                    SupersedesAssertionId =
                        "selfmodel:assertion:prior-health"
                },
            baseline.Identity);
    }

    private static void VerifyDuplicateAssertionIdentityRejected()
    {
        var assertions =
            BuildCompleteAssertions(BaseTime)
                .ToList();

        assertions.Add(
            assertions[0]);

        ExpectThrows<ArgumentException>(
            () =>
                BuildModel(
                    assertions,
                    BaseTime),
            "duplicate assertion identity",
            "Duplicate Self Model assertion identity was accepted.");
    }

    private static void VerifyHealthProjectionDoesNotReevaluateHealth()
    {
        var health =
            CreateHealthAssessment(
                assessmentId:
                    "health:assessment:wp03:degraded",
                healthState:
                    HealthState.Degraded,
                evidenceQuality:
                    EvidenceQuality.Limited,
                evidenceReference:
                    "health:evidence:wp03:degraded",
                confidence:
                    "LIMITED",
                contradictions:
                    "NONE",
                blindSpots:
                    "bounded-observability-gap",
                reasonCode:
                    "KNOWN_DEGRADATION",
                reducedByDependencyId:
                    "NONE",
                consequenceClass:
                    HealthConsequenceClass.Degrading,
                observationTime:
                    BaseTime.AddSeconds(-3),
                assessmentTime:
                    BaseTime.AddSeconds(-2));

        var assertion =
            FoundationSelfModelAssertionFactory
                .FromHealthAssessment(
                    "selfmodel:assertion:healthcondition",
                    "source:health:wp02",
                    "owner:foundation:health",
                    "foundation",
                    "freshness:health-rule",
                    BaseTime.AddSeconds(20),
                    health);

        Require(
            assertion.Area ==
                FoundationSelfModelArea
                    .HealthCondition,
            "Health projection used wrong Self Model area.");

        Require(
            assertion.AssertionKind ==
                FoundationSelfModelAssertionKind
                    .Interpretation,
            "Health projection incorrectly changed assertion kind.");

        Require(
            assertion.ValueIdentity ==
                "health-state:degraded",
            "Self Model recomputed or changed Health state.");

        Require(
            assertion.EvidenceQuality ==
                EvidenceQuality.Limited,
            "Self Model recomputed or changed Health evidence quality.");

        RequireEqual(
            assertion.EvidenceReference,
            health.EvidenceReference,
            "Self Model changed Health evidence reference.");

        RequireEqual(
            assertion.SourceAssessmentReference,
            health.Identity,
            "Self Model did not preserve exact Health assessment identity.");

        var complete =
            BuildCompleteAssertions(BaseTime)
                .Select(candidate =>
                    candidate.Area ==
                        FoundationSelfModelArea
                            .HealthCondition
                        ? assertion
                        : candidate)
                .ToArray();

        var model =
            BuildModel(
                complete,
                BaseTime);

        Require(
            model.Assertions.Any(candidate =>
                candidate.Area ==
                    FoundationSelfModelArea
                        .HealthCondition &&
                candidate.ValueIdentity ==
                    "health-state:degraded" &&
                candidate.EvidenceQuality ==
                    EvidenceQuality.Limited),
            "Projected Health evidence was not preserved by model.");
    }

    private static void VerifyMalformedHealthProjectionRejected()
    {
        var valid =
            CreateHealthAssessment(
                assessmentId:
                    "health:assessment:wp03:validation",
                healthState:
                    HealthState.Healthy,
                evidenceQuality:
                    EvidenceQuality.Sufficient,
                evidenceReference:
                    "health:evidence:wp03:validation",
                confidence:
                    "SUFFICIENT",
                contradictions:
                    "NONE",
                blindSpots:
                    "NONE",
                reasonCode:
                    "HEALTHY",
                reducedByDependencyId:
                    "NONE",
                consequenceClass:
                    HealthConsequenceClass
                        .ObservationOnly,
                observationTime:
                    BaseTime.AddSeconds(-3),
                assessmentTime:
                    BaseTime.AddSeconds(-2));

        ExpectThrows<ArgumentException>(
            () =>
                ProjectHealth(
                    valid with
                    {
                        HealthState =
                            (HealthState)999
                    }),
            "Health assessment enum rejected",
            "Undefined Health state entered the Self Model projection.");

        ExpectThrows<ArgumentException>(
            () =>
                ProjectHealth(
                    valid with
                    {
                        AssessmentId =
                            " bad "
                    }),
            "Health assessment canonical identity rejected",
            "Malformed Health assessment identity entered the Self Model projection.");

        ExpectThrows<ArgumentException>(
            () =>
                ProjectHealth(
                    valid with
                    {
                        ObservationTime =
                            BaseTime,
                        AssessmentTime =
                            BaseTime.AddSeconds(-1)
                    }),
            "Health assessment time order rejected",
            "Impossible Health assessment time order entered the Self Model projection.");
    }

    private static void VerifyNoAuthorityOrFutureStageActionSurface()
    {
        var assembly =
            typeof(FoundationSelfModelProjector)
                .Assembly;

        var forbiddenMethodPrefixes =
            new[]
            {
                "Grant",
                "Revoke",
                "Restrict",
                "Isolate",
                "Kill",
                "Recover",
                "Release",
                "Revive",
                "Deploy",
                "Activate",
                "Transition"
            };

        foreach (var type in assembly.GetExportedTypes())
        {
            foreach (
                var method in
                type.GetMethods(
                    BindingFlags.Public |
                    BindingFlags.Instance |
                    BindingFlags.Static |
                    BindingFlags.DeclaredOnly))
            {
                if (forbiddenMethodPrefixes.Any(prefix =>
                    method.Name.StartsWith(
                        prefix,
                        StringComparison.OrdinalIgnoreCase)))
                {
                    throw new InvalidOperationException(
                        "Forbidden authority/protection/recovery action surface: " +
                        type.FullName +
                        "." +
                        method.Name);
                }
            }

            var fullName =
                type.FullName ??
                type.Name;

            foreach (
                var forbiddenTypeToken in
                new[]
                {
                    "MonitorAI",
                    "FactoryReset",
                    "ControlledRevival",
                    "InvestigationHold",
                    "GuardianCommand"
                })
            {
                if (fullName.Contains(
                    forbiddenTypeToken,
                    StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException(
                        "Future-stage type leaked into WP-03: " +
                        fullName);
                }
            }
        }
    }

    private static void VerifyExactProductionAssemblyBoundary()
    {
        var assembly =
            typeof(FoundationSelfModelProjector)
                .Assembly;

        var foundationReferences =
            assembly
                .GetReferencedAssemblies()
                .Select(name =>
                    name.Name ?? string.Empty)
                .Where(name =>
                    name.StartsWith(
                        "Foundation.",
                        StringComparison.Ordinal))
                .OrderBy(
                    name => name,
                    StringComparer.Ordinal)
                .ToArray();

        var expected =
            new[]
            {
                "Foundation.Contracts",
                "Foundation.HealthFitness"
            }
            .OrderBy(
                name => name,
                StringComparer.Ordinal)
            .ToArray();

        Require(
            foundationReferences.SequenceEqual(
                expected,
                StringComparer.Ordinal),
            "Foundation.SelfAwareness production dependency boundary is not exact. Actual=" +
            string.Join(
                ",",
                foundationReferences));
    }

    private static void VerifyNoApplicationOrWebBusinessSurface()
    {
        var assembly =
            typeof(FoundationSelfModelProjector)
                .Assembly;

        var forbiddenTokens =
            new[]
            {
                "Application",
                "Web",
                "Trading",
                "Trade",
                "Market",
                "Portfolio",
                "Broker",
                "Strategy",
                "MSA",
                "LSA",
                "CSA"
            };

        foreach (var type in assembly.GetExportedTypes())
        {
            var symbolNames =
                new List<string>
                {
                    type.FullName ??
                    type.Name
                };

            symbolNames.AddRange(
                type.GetProperties(
                        BindingFlags.Public |
                        BindingFlags.Instance |
                        BindingFlags.Static)
                    .Select(property =>
                        property.Name));

            symbolNames.AddRange(
                type.GetMethods(
                        BindingFlags.Public |
                        BindingFlags.Instance |
                        BindingFlags.Static |
                        BindingFlags.DeclaredOnly)
                    .Select(method =>
                        method.Name));

            foreach (var symbol in symbolNames)
            {
                foreach (
                    var token in forbiddenTokens)
                {
                    if (symbol.Contains(
                        token,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException(
                            "Application/Web/business symbol leaked into WP-03: " +
                            symbol +
                            " / token=" +
                            token);
                    }
                }
            }
        }
    }

    private static CanonicalHealthAssessment
        CreateHealthAssessment(
            string assessmentId,
            HealthState healthState,
            EvidenceQuality evidenceQuality,
            string evidenceReference,
            string confidence,
            string contradictions,
            string blindSpots,
            string reasonCode,
            string reducedByDependencyId,
            HealthConsequenceClass consequenceClass,
            DateTimeOffset observationTime,
            DateTimeOffset assessmentTime)
    {
        return new CanonicalHealthAssessment(
            assessmentId,
            "foundation:core",
            "foundation-runtime",
            healthState,
            evidenceQuality,
            evidenceReference,
            confidence,
            contradictions,
            blindSpots,
            reasonCode,
            reducedByDependencyId,
            consequenceClass,
            "health:rule:wp03",
            "1.0",
            observationTime,
            assessmentTime);
    }

    private static FoundationSelfModelAssertion
        ProjectHealth(
            CanonicalHealthAssessment assessment)
    {
        return FoundationSelfModelAssertionFactory
            .FromHealthAssessment(
                "selfmodel:assertion:healthvalidation",
                "source:health:wp02",
                "owner:foundation:health",
                "foundation",
                "freshness:health-rule",
                BaseTime.AddSeconds(20),
                assessment);
    }

    private static FoundationSelfModelAssertion[]
        BuildCompleteAssertions(
            DateTimeOffset modelTime)
    {
        var assertions =
            new List<FoundationSelfModelAssertion>();

        foreach (
            var area in
            Enum.GetValues<FoundationSelfModelArea>())
        {
            if (area ==
                FoundationSelfModelArea.HealthCondition)
            {
                var health =
                    CreateHealthAssessment(
                        assessmentId:
                            "health:assessment:wp03",
                        healthState:
                            HealthState.Healthy,
                        evidenceQuality:
                            EvidenceQuality.Sufficient,
                        evidenceReference:
                            "health:evidence:wp03",
                        confidence:
                            "SUFFICIENT",
                        contradictions:
                            "NONE",
                        blindSpots:
                            "NONE",
                        reasonCode:
                            "HEALTHY",
                        reducedByDependencyId:
                            "NONE",
                        consequenceClass:
                            HealthConsequenceClass
                                .ObservationOnly,
                        observationTime:
                            modelTime.AddSeconds(-3),
                        assessmentTime:
                            modelTime.AddSeconds(-2));

                assertions.Add(
                    FoundationSelfModelAssertionFactory
                        .FromHealthAssessment(
                            "selfmodel:assertion:healthcondition",
                            "source:health:wp02",
                            "owner:foundation:health",
                            "foundation",
                            "freshness:health-rule",
                            modelTime.AddSeconds(30),
                            health));

                continue;
            }

            var slug =
                area.ToString()
                    .ToLowerInvariant();

            var isExplicitUnknown =
                area ==
                    FoundationSelfModelArea
                        .TechnicalFitness ||
                area ==
                    FoundationSelfModelArea
                        .PendingConformance;

            assertions.Add(
                new FoundationSelfModelAssertion(
                    "selfmodel:assertion:" +
                        slug,
                    "foundation:core",
                    area,
                    isExplicitUnknown
                        ? FoundationSelfModelAssertionKind
                            .Unknown
                        : FoundationSelfModelAssertionKind
                            .Fact,
                    FoundationSelfModelTemporalView
                        .Current,
                    "foundation",
                    isExplicitUnknown
                        ? "technical:value:unknown:" +
                            slug
                        : "technical:value:" +
                            slug,
                    "source:foundation:" +
                        slug,
                    "owner:foundation:technical",
                    "evidence:foundation:" +
                        slug,
                    isExplicitUnknown
                        ? EvidenceQuality.Insufficient
                        : EvidenceQuality.Sufficient,
                    isExplicitUnknown
                        ? "INSUFFICIENT"
                        : "SUFFICIENT",
                    isExplicitUnknown
                        ? "not-yet-produced-by-governed-stage"
                        : "NONE",
                    "freshness:source-bound",
                    "awr-001:wp03-projection",
                    "2.1",
                    modelTime.AddSeconds(-2),
                    modelTime.AddSeconds(-2),
                    modelTime.AddSeconds(30),
                    null,
                    null));
        }

        return assertions.ToArray();
    }

    private static FoundationSelfModelSnapshot
        BuildModel(
            IReadOnlyCollection<
                FoundationSelfModelAssertion> assertions,
            DateTimeOffset modelTime)
    {
        return FoundationSelfModelProjector.Build(
            "selfmodel:model:wp03",
            "foundation:falcon",
            "baseline:stage7:wp03",
            modelTime,
            assertions,
            null);
    }

    private static FoundationSelfModelAssertion[]
        ReplaceArea(
            IReadOnlyCollection<
                FoundationSelfModelAssertion> source,
            FoundationSelfModelArea area,
            Func<
                FoundationSelfModelAssertion,
                FoundationSelfModelAssertion> replacement)
    {
        return source
            .Select(assertion =>
                assertion.Area == area &&
                assertion.TemporalView ==
                    FoundationSelfModelTemporalView
                        .Current
                    ? replacement(assertion)
                    : assertion)
            .ToArray();
    }

    private static void RequireKind(
        FoundationSelfModelSnapshot model,
        FoundationSelfModelArea area,
        FoundationSelfModelAssertionKind expected)
    {
        var actual =
            model.Assertions.Single(assertion =>
                assertion.Area == area &&
                assertion.TemporalView ==
                    FoundationSelfModelTemporalView
                        .Current);

        Require(
            actual.AssertionKind ==
                expected,
            "Assertion kind was changed for " +
            area +
            ". Expected=" +
            expected +
            " Actual=" +
            actual.AssertionKind);
    }

    private static void AssertMutationChangesIdentity(
        string label,
        IReadOnlyCollection<
            FoundationSelfModelAssertion>
            baselineAssertions,
        FoundationSelfModelAssertion target,
        Func<
            FoundationSelfModelAssertion,
            FoundationSelfModelAssertion> mutation,
        string baselineIdentity)
    {
        var mutatedAssertions =
            baselineAssertions
                .Select(assertion =>
                    assertion.AssertionId ==
                        target.AssertionId
                        ? mutation(assertion)
                        : assertion)
                .ToArray();

        var mutated =
            BuildModel(
                mutatedAssertions,
                BaseTime);

        Require(
            !string.Equals(
                baselineIdentity,
                mutated.Identity,
                StringComparison.Ordinal),
            "Material " +
            label +
            " mutation did not change Self Model identity.");
    }

    private static void ExpectThrows<TException>(
        Action action,
        string messageFragment,
        string failureMessage)
        where TException : Exception
    {
        try
        {
            action();
        }
        catch (TException exception)
        {
            Require(
                exception.Message.Contains(
                    messageFragment,
                    StringComparison.OrdinalIgnoreCase),
                "Expected exception did not contain deterministic reason. Expected fragment=" +
                messageFragment +
                " Actual=" +
                exception.Message);

            return;
        }

        throw new InvalidOperationException(
            failureMessage);
    }

    private static void RequireEqual(
        string? left,
        string? right,
        string message)
    {
        Require(
            string.Equals(
                left,
                right,
                StringComparison.Ordinal),
            message +
            " Left=" +
            left +
            " Right=" +
            right);
    }

    private static void Require(
        bool condition,
        string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(
                message);
        }
    }
}