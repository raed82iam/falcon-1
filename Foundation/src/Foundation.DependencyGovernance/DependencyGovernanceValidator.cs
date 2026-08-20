using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Foundation.Admission;
using Foundation.ContractRegistry;
using Foundation.Contracts;
using Foundation.ServiceCatalog;

namespace Foundation.DependencyGovernance;

public sealed class DependencyGovernanceValidator
{
    private readonly record struct DependencyNodeKey(DependencySubjectKind SubjectKind, string Identity, string Version)
    {
        public string CanonicalText => $"{SubjectKind}|{Identity.Length}:{Identity}|{Version.Length}:{Version}";
    }

    public DependencyValidationResult Validate(DependencyGraphRequest request)
    {
        if (request is null)
        {
            return Fail("INVALID_GRAPH_REQUEST");
        }

        if (string.IsNullOrWhiteSpace(request.GraphId))
        {
            return Fail("MISSING_GRAPH_ID");
        }

        if (string.IsNullOrWhiteSpace(request.GraphVersion))
        {
            return Fail("MISSING_GRAPH_VERSION");
        }

        if (string.IsNullOrWhiteSpace(request.RequesterIdentity))
        {
            return Fail("MISSING_REQUESTER");
        }

        if (string.IsNullOrWhiteSpace(request.AuthoritySource))
        {
            return Fail("INVALID_GRAPH_REQUEST");
        }

        if (request.ObservationTime == default)
        {
            return Fail("INVALID_OBSERVATION_TIME");
        }

        if (request.ManifestSurface is null)
        {
            return Fail("INVALID_GRAPH_MANIFEST");
        }

        if (request.DelegationEvidence is null)
        {
            return Fail("INVALID_DELEGATION_EVIDENCE");
        }

        if (string.IsNullOrWhiteSpace(request.DelegationEvidence.ChainIdentity))
        {
            return Fail("INVALID_DELEGATION_EVIDENCE");
        }

        if (string.Equals(request.DelegationEvidence.DelegationState, "REVOKED", StringComparison.Ordinal))
        {
            if (string.IsNullOrWhiteSpace(request.DelegationEvidence.DelegationId) ||
                string.IsNullOrWhiteSpace(request.DelegationEvidence.Version) ||
                string.IsNullOrWhiteSpace(request.DelegationEvidence.Grantor) ||
                string.IsNullOrWhiteSpace(request.DelegationEvidence.Grantee) ||
                string.IsNullOrWhiteSpace(request.DelegationEvidence.Scope) ||
                string.IsNullOrWhiteSpace(request.DelegationEvidence.AuthoritySource) ||
                string.IsNullOrWhiteSpace(request.DelegationEvidence.ValidationEvidence) ||
                string.IsNullOrWhiteSpace(request.DelegationEvidence.TerminationRule))
            {
                return Fail("INVALID_DELEGATION_EVIDENCE");
            }

            return Fail("DELEGATION_REVOKED");
        }

        var manifestValidation = ContractValidators.Validate(request.ManifestSurface);
        if (manifestValidation.Result != ValidationResult.Pass)
        {
            return Fail("INVALID_GRAPH_MANIFEST");
        }

        if (!string.Equals(request.ManifestSurface.ManifestClass, "CANDIDATE_MANIFEST", StringComparison.Ordinal))
        {
            return Fail("LIFECYCLE_CLASS_SUBSTITUTION");
        }

        if (!string.Equals(request.ManifestSurface.SubjectId, request.GraphId, StringComparison.Ordinal))
        {
            return Fail("GRAPH_MANIFEST_SUBJECT_MISMATCH");
        }

        if (request.ManifestSurface.EffectiveTime > request.ObservationTime)
        {
            return Fail("GRAPH_MANIFEST_NOT_EFFECTIVE");
        }

        if (request.ManifestSurface.Expiry <= request.ObservationTime)
        {
            return Fail("GRAPH_MANIFEST_EXPIRED");
        }

        var delegationValidation = ProviderContractValidators.Validate(request.DelegationEvidence);
        if (delegationValidation.Result != ValidationResult.Pass)
        {
            return Fail("INVALID_DELEGATION_EVIDENCE");
        }

        if (!string.Equals(request.DelegationEvidence.Grantee, request.RequesterIdentity, StringComparison.Ordinal))
        {
            return Fail("DELEGATION_GRANTEE_MISMATCH");
        }

        if (!string.Equals(request.DelegationEvidence.AuthoritySource, request.AuthoritySource, StringComparison.Ordinal))
        {
            return Fail("DELEGATION_AUTHORITY_MISMATCH");
        }

        if (request.DelegationEvidence.EffectiveTime > request.ObservationTime)
        {
            return Fail("DELEGATION_NOT_EFFECTIVE");
        }

        if (request.DelegationEvidence.Expiry <= request.ObservationTime)
        {
            return Fail("DELEGATION_EXPIRED");
        }

        if (!string.Equals(request.DelegationEvidence.DelegationState, "GRANTED", StringComparison.Ordinal))
        {
            return Fail("DELEGATION_REVOKED");
        }

        if (!ContainsDelegationScope(request.DelegationEvidence.Scope))
        {
            return Fail("DELEGATION_SCOPE_MISMATCH");
        }

        var subjects = FreezeSubjects(request.Subjects, request.ObservationTime, out var subjectFailure);
        if (subjectFailure is not null)
        {
            return Fail(subjectFailure);
        }

        var subjectIdentityGroups = subjects
            .GroupBy(subject => subject.SubjectKey.CanonicalText, StringComparer.Ordinal)
            .ToArray();

        foreach (var group in subjectIdentityGroups)
        {
            if (group.Count() > 1)
            {
                if (group.Select(subject => subject.SubjectKind).Distinct().Count() > 1)
                {
                    return Fail("AMBIGUOUS_SUBJECT_IDENTITY");
                }

                return Fail("DUPLICATE_SUBJECT");
            }
        }

        var subjectMap = subjects.ToDictionary(subject => subject.SubjectKey.CanonicalText, subject => subject, StringComparer.Ordinal);

        var dependencyDeclarations = FreezeDependencies(request.Dependencies, out var dependencyFailure);
        if (dependencyFailure is not null)
        {
            return Fail(dependencyFailure);
        }

        var dependencyGroups = dependencyDeclarations
            .GroupBy(declaration => SubjectKey(declaration.Consumer), StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.ToList(), StringComparer.Ordinal);

        var coverageResult = ValidateManifestCoverage(subjects, dependencyGroups, request);
        if (coverageResult is not null)
        {
            return Fail(coverageResult);
        }

        var validationResult = ValidateDependencyDeclarations(subjects, dependencyDeclarations, subjectMap, request.ObservationTime, request.DelegationEvidence.ChainIdentity, out var edges, out var resolvedDependencies, out var unresolvedOptionalDependencies, out var graphFailure, out var cycleEvidence);
        if (graphFailure is not null)
        {
            return Fail(graphFailure, cycleEvidence);
        }

        var canonicalOrder = ComputeCanonicalOrder(subjects, edges, out var orderFailure, out var edgeViolation);
        if (orderFailure is not null)
        {
            return Fail(orderFailure, edgeViolation);
        }

        var proposedOrder = FreezeOrder(request.ProposedActivationOrder, out var orderValidationFailure);
        if (orderValidationFailure is not null)
        {
            return Fail(orderValidationFailure);
        }

        var orderValidation = ValidateActivationOrder(proposedOrder, canonicalOrder, edges, out var activationOrderFailure);
        if (activationOrderFailure is not null)
        {
            return Fail(activationOrderFailure);
        }

        var manifestDigestInput = SerializeCandidateGraphRequest(request);
        var canonicalManifestDigest = ComputeSha256(manifestDigestInput);
        if (!string.Equals(request.ManifestSurface.CanonicalDigest, canonicalManifestDigest, StringComparison.OrdinalIgnoreCase))
        {
            return Fail("GRAPH_MANIFEST_DIGEST_MISMATCH");
        }

        var graphText = SerializeGraphSnapshot(request, subjects, dependencyDeclarations, resolvedDependencies, unresolvedOptionalDependencies, canonicalOrder);
        var graphDigest = ComputeSha256(graphText);
        var orderText = SerializeActivationOrder(request.GraphId, request.GraphVersion, canonicalOrder);
        var orderDigest = ComputeSha256(orderText);
        var decisionIdentity = ComputeDecisionIdentity(request, graphDigest, orderDigest);
        var subjectSnapshots = subjects.Select(CreateSubjectSnapshot).ToArray();
        var graphSnapshot = new DependencyGraphSnapshot
        {
            GraphKey = request.GraphKey,
            Subjects = new ReadOnlyCollection<DependencySubjectSnapshot>(subjectSnapshots),
            ResolvedDependencies = new ReadOnlyCollection<DependencyResolutionSnapshot>(resolvedDependencies.ToArray()),
            UnresolvedOptionalDependencies = new ReadOnlyCollection<DependencyResolutionSnapshot>(unresolvedOptionalDependencies.ToArray()),
            CanonicalActivationOrder = new ReadOnlyCollection<DependencySubjectKey>(canonicalOrder.ToArray())
        };

        var graphEvent = BuildEvent(
            "DEPENDENCY_GRAPH_VALIDATED",
            request.GraphId,
            request.ObservationTime,
            decisionIdentity,
            decisionIdentity,
            null,
            graphDigest);
        var orderEvent = BuildEvent(
            "ACTIVATION_ORDER_VALIDATED",
            request.GraphId,
            request.ObservationTime,
            decisionIdentity,
            decisionIdentity,
            graphEvent.EventId,
            orderDigest);

        if (ContractValidators.Validate(graphEvent).Result != ValidationResult.Pass)
        {
            return Fail("INVALID_SUBJECT_EVIDENCE");
        }

        if (ContractValidators.Validate(orderEvent).Result != ValidationResult.Pass)
        {
            return Fail("INVALID_SUBJECT_EVIDENCE");
        }

        return new DependencyValidationResult
        {
            Success = true,
            ReasonCode = "DEPENDENCY_GRAPH_VALIDATED",
            DecisionIdentity = decisionIdentity,
            GraphKey = request.GraphKey,
            GraphDecision = "DEPENDENCY_GRAPH_VALIDATED",
            ActivationOrderDecision = "ACTIVATION_ORDER_VALIDATED",
            CanonicalGraphText = graphText,
            GraphDigest = graphDigest,
            CanonicalActivationOrderText = orderText,
            ActivationOrderDigest = orderDigest,
            ImmutableGraphSnapshot = graphSnapshot,
            ResolvedDependencies = new ReadOnlyCollection<DependencyResolutionSnapshot>(resolvedDependencies.ToArray()),
            UnresolvedOptionalDependencies = new ReadOnlyCollection<DependencyResolutionSnapshot>(unresolvedOptionalDependencies.ToArray()),
            EvidenceEvents = new ReadOnlyCollection<FilEvent>(new[] { graphEvent, orderEvent }),
            CycleEvidence = cycleEvidence
        };
    }

    private static List<DependencySubjectEvidence> FreezeSubjects(IReadOnlyList<DependencySubjectEvidence> subjects, DateTimeOffset observationTime, out string? failure)
    {
        failure = null;
        if (subjects is null || subjects.Count == 0)
        {
            failure = "INVALID_GRAPH_REQUEST";
            return new List<DependencySubjectEvidence>();
        }

        var frozen = new List<DependencySubjectEvidence>(subjects.Count);
        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (var subject in subjects)
        {
            if (subject is null)
            {
                failure = "INVALID_SUBJECT_EVIDENCE";
                return new List<DependencySubjectEvidence>();
            }

            if (string.IsNullOrWhiteSpace(subject.SubjectKey.Identity) || string.IsNullOrWhiteSpace(subject.SubjectKey.Version))
            {
                failure = "INVALID_SUBJECT_EVIDENCE";
                return new List<DependencySubjectEvidence>();
            }

            if (!IsDefinedEnum(subject.SubjectKind))
            {
                failure = "INVALID_SUBJECT_KIND";
                return new List<DependencySubjectEvidence>();
            }

            var key = NodeKey(subject);
            if (!seen.Add(key.CanonicalText))
            {
                failure = "DUPLICATE_SUBJECT";
                return new List<DependencySubjectEvidence>();
            }

            var validation = ValidateSubjectEvidence(subject, observationTime, out var subjectFailure);
            if (!validation || subjectFailure is not null)
            {
                failure = subjectFailure ?? "INVALID_SUBJECT_EVIDENCE";
                return new List<DependencySubjectEvidence>();
            }

            frozen.Add(subject);
        }

        return frozen;
    }

    private static List<DependencyDeclaration> FreezeDependencies(IReadOnlyList<DependencyDeclaration> dependencies, out string? failure)
    {
        failure = null;
        if (dependencies is null)
        {
            failure = "INVALID_GRAPH_REQUEST";
            return new List<DependencyDeclaration>();
        }

        var frozen = new List<DependencyDeclaration>(dependencies.Count);
        foreach (var dependency in dependencies)
        {
            if (dependency is null)
            {
                failure = "INVALID_GRAPH_REQUEST";
                return new List<DependencyDeclaration>();
            }

            if (string.IsNullOrWhiteSpace(dependency.Consumer.Identity) || string.IsNullOrWhiteSpace(dependency.Consumer.Version))
            {
                failure = "INVALID_GRAPH_REQUEST";
                return new List<DependencyDeclaration>();
            }

            if (string.IsNullOrWhiteSpace(dependency.DependencyIdentity))
            {
                failure = "UNKNOWN_DEPENDENCY";
                return new List<DependencyDeclaration>();
            }

            if (!IsDefinedEnum(dependency.DependencyKind))
            {
                failure = "INVALID_SUBJECT_KIND";
                return new List<DependencyDeclaration>();
            }

            if (!IsDefinedEnum(dependency.Relationship))
            {
                failure = "INVALID_RELATIONSHIP";
                return new List<DependencyDeclaration>();
            }

            if (dependency.ConditionState.HasValue && !IsDefinedEnum(dependency.ConditionState.Value))
            {
                failure = "INVALID_CONDITION_STATE";
                return new List<DependencyDeclaration>();
            }

            if (dependency.CompatibleVersions is null || dependency.CompatibleVersions.Count == 0)
            {
                failure = "EMPTY_COMPATIBLE_VERSION_SET";
                return new List<DependencyDeclaration>();
            }

            if (dependency.CompatibleVersions.Any(string.IsNullOrWhiteSpace))
            {
                failure = "BLANK_COMPATIBLE_VERSION";
                return new List<DependencyDeclaration>();
            }

            if (dependency.CompatibleVersions.Count != dependency.CompatibleVersions.Distinct(StringComparer.Ordinal).Count())
            {
                failure = "DUPLICATE_COMPATIBLE_VERSION";
                return new List<DependencyDeclaration>();
            }

            if (string.IsNullOrWhiteSpace(dependency.DependencySource))
            {
                failure = "INVALID_GRAPH_REQUEST";
                return new List<DependencyDeclaration>();
            }

            if (string.IsNullOrWhiteSpace(dependency.DeclaredPurpose))
            {
                failure = "INVALID_GRAPH_REQUEST";
                return new List<DependencyDeclaration>();
            }

            if (string.IsNullOrWhiteSpace(dependency.IntegrityRequirement))
            {
                failure = "INVALID_GRAPH_REQUEST";
                return new List<DependencyDeclaration>();
            }

            if (string.IsNullOrWhiteSpace(dependency.AvailabilityRequirement))
            {
                failure = "INVALID_GRAPH_REQUEST";
                return new List<DependencyDeclaration>();
            }

            if (string.IsNullOrWhiteSpace(dependency.TimeoutPolicy))
            {
                failure = "INVALID_GRAPH_REQUEST";
                return new List<DependencyDeclaration>();
            }

            if (string.IsNullOrWhiteSpace(dependency.DegradedStatePolicy))
            {
                failure = "MISSING_DEGRADED_POLICY";
                return new List<DependencyDeclaration>();
            }

            if (string.IsNullOrWhiteSpace(dependency.IsolationBoundary))
            {
                failure = "MISSING_CONTAINMENT_POLICY";
                return new List<DependencyDeclaration>();
            }

            if (string.IsNullOrWhiteSpace(dependency.FailurePropagationLimit))
            {
                failure = "INVALID_GRAPH_REQUEST";
                return new List<DependencyDeclaration>();
            }

            if (string.IsNullOrWhiteSpace(dependency.ReplacementPolicy))
            {
                failure = "INVALID_GRAPH_REQUEST";
                return new List<DependencyDeclaration>();
            }

            if (string.IsNullOrWhiteSpace(dependency.MigrationPolicy))
            {
                failure = "INVALID_GRAPH_REQUEST";
                return new List<DependencyDeclaration>();
            }

            if (string.IsNullOrWhiteSpace(dependency.RollbackPolicy))
            {
                failure = "MISSING_ROLLBACK_POLICY";
                return new List<DependencyDeclaration>();
            }

            if (string.IsNullOrWhiteSpace(dependency.EvidenceRequirement))
            {
                failure = "MISSING_EVIDENCE_REFERENCE";
                return new List<DependencyDeclaration>();
            }

            if (string.IsNullOrWhiteSpace(dependency.DelegationChainEvidenceReference))
            {
                failure = "MISSING_EVIDENCE_REFERENCE";
                return new List<DependencyDeclaration>();
            }

            if (dependency.LifecycleOrder is null)
            {
                failure = "MISSING_LIFECYCLE_ORDER_DECLARATION";
                return new List<DependencyDeclaration>();
            }

            if (!ValidateLifecycleOrder(dependency.LifecycleOrder, out failure))
            {
                return new List<DependencyDeclaration>();
            }

            frozen.Add(dependency with { CompatibleVersions = FreezeStrings(dependency.CompatibleVersions) });
        }

        return frozen;
    }

    private static bool ValidateSubjectEvidence(DependencySubjectEvidence subject, DateTimeOffset observationTime, out string? failure)
    {
        failure = null;
        if (string.IsNullOrWhiteSpace(subject.EvidenceReference))
        {
            failure = "MISSING_EVIDENCE_REFERENCE";
            return false;
        }

        if (!IsDefinedEnum(subject.SubjectKind))
        {
            failure = "INVALID_SUBJECT_KIND";
            return false;
        }

        return subject switch
        {
            FoundationServiceSubjectEvidence foundation => subject.SubjectKind == DependencySubjectKind.FoundationService
                ? ValidateFoundationServiceEvidence(foundation, out failure)
                : FailSubjectKindMismatch(out failure),
            ApplicationSubjectEvidence application => subject.SubjectKind == DependencySubjectKind.Application
                ? ValidateApplicationEvidence(application, out failure)
                : FailSubjectKindMismatch(out failure),
            ExternalDependencySubjectEvidence external => subject.SubjectKind == DependencySubjectKind.External
                ? ValidateExternalEvidence(external, observationTime, out failure)
                : FailSubjectKindMismatch(out failure),
            _ => FailSubjectKindMismatch(out failure)
        };
    }

    private static bool FailSubjectKindMismatch(out string? failure)
    {
        failure = "SUBJECT_KIND_MISMATCH";
        return false;
    }

    private static bool ValidateFoundationServiceEvidence(FoundationServiceSubjectEvidence evidence, out string? failure)
    {
        failure = null;
        var entry = evidence.CatalogEntry;
        if (entry is null)
        {
            failure = "INVALID_SUBJECT_EVIDENCE";
            return false;
        }

        if (string.IsNullOrWhiteSpace(entry.Key.ServiceIdentity) || string.IsNullOrWhiteSpace(entry.Key.ServiceVersion))
        {
            failure = "INVALID_SUBJECT_EVIDENCE";
            return false;
        }

        if (!string.Equals(evidence.SubjectKey.Identity, entry.Key.ServiceIdentity, StringComparison.Ordinal) ||
            !string.Equals(evidence.SubjectKey.Version, entry.Key.ServiceVersion, StringComparison.Ordinal))
        {
            failure = "INVALID_SUBJECT_EVIDENCE";
            return false;
        }

        if (entry.Registration is null || entry.Manifest is null)
        {
            failure = "INVALID_SUBJECT_EVIDENCE";
            return false;
        }

        if (entry.Registration.Decision != ServiceRegistrationDecision.Registered ||
            entry.Registration.RegistrationState != ServiceRegistrationState.Registered ||
            entry.Registration.OperationalState != ServiceOperationalState.NotActive ||
            entry.Registration.AuthorityGranted ||
            entry.Registration.PermissionGranted ||
            entry.Registration.TrustGranted ||
            entry.Registration.AdmissionGranted)
        {
            failure = "INVALID_SUBJECT_EVIDENCE";
            return false;
        }

        if (!string.Equals(entry.Registration.RegistrationEvidenceReference, evidence.EvidenceReference, StringComparison.Ordinal))
        {
            failure = "INVALID_SUBJECT_EVIDENCE";
            return false;
        }

        if (!string.Equals(entry.Registration.CatalogKey.ServiceIdentity, evidence.SubjectKey.Identity, StringComparison.Ordinal) ||
            !string.Equals(entry.Registration.CatalogKey.ServiceVersion, evidence.SubjectKey.Version, StringComparison.Ordinal))
        {
            failure = "INVALID_SUBJECT_EVIDENCE";
            return false;
        }

        if (!string.Equals(entry.Registration.CatalogKey.ServiceIdentity, entry.Manifest.ServiceIdentity, StringComparison.Ordinal) ||
            !string.Equals(entry.Registration.CatalogKey.ServiceVersion, entry.Manifest.ServiceVersion, StringComparison.Ordinal) ||
            !string.Equals(entry.Registration.AccountableOwner, entry.Manifest.AccountableOwner, StringComparison.Ordinal))
        {
            failure = "INVALID_SUBJECT_EVIDENCE";
            return false;
        }

        if (!string.Equals(entry.Registration.ManifestDigest, entry.Manifest.ComputeDigest(), StringComparison.OrdinalIgnoreCase))
        {
            failure = "INVALID_SUBJECT_EVIDENCE";
            return false;
        }

        if (!string.Equals(entry.Registration.ManifestDigest, evidence.CatalogEntry.Registration.ManifestDigest, StringComparison.OrdinalIgnoreCase))
        {
            failure = "INVALID_SUBJECT_EVIDENCE";
            return false;
        }

        if (!string.Equals(entry.Registration.RegistrationEvidenceReference, evidence.EvidenceReference, StringComparison.Ordinal))
        {
            failure = "INVALID_SUBJECT_EVIDENCE";
            return false;
        }

        return true;
    }

    private static bool ValidateApplicationEvidence(ApplicationSubjectEvidence evidence, out string? failure)
    {
        failure = null;
        if (evidence.AdmissionRequest is null || evidence.AdmissionDecision is null || evidence.Manifest is null)
        {
            failure = "INVALID_SUBJECT_EVIDENCE";
            return false;
        }

        if (!string.Equals(evidence.AdmissionDecision.Decision, "ADMITTED", StringComparison.Ordinal) ||
            !string.Equals(evidence.AdmissionDecision.AdmissionId, evidence.AdmissionRequest.AdmissionId, StringComparison.Ordinal) ||
            !string.Equals(evidence.AdmissionDecision.ContractId, "CON-023", StringComparison.Ordinal) ||
            !string.Equals(evidence.AdmissionRequest.ContractId, "CON-023", StringComparison.Ordinal))
        {
            failure = "INVALID_SUBJECT_EVIDENCE";
            return false;
        }

        if (!string.Equals(evidence.AdmissionDecision.ContractVersion, evidence.AdmissionRequest.ContractVersion, StringComparison.Ordinal) ||
            !string.Equals(evidence.AdmissionDecision.EvidenceId, evidence.AdmissionEvidenceIdentity, StringComparison.Ordinal) ||
            string.IsNullOrWhiteSpace(evidence.AdmissionRequest.ProvenanceId))
        {
            failure = "INVALID_SUBJECT_EVIDENCE";
            return false;
        }

        if (!string.Equals(evidence.AdmissionRequest.Identity, evidence.SubjectKey.Identity, StringComparison.Ordinal) ||
            !string.Equals(evidence.AdmissionRequest.Version, evidence.SubjectKey.Version, StringComparison.Ordinal) ||
            !string.Equals(evidence.AdmissionRequest.Owner, evidence.Manifest.ApplicationOwner, StringComparison.Ordinal) ||
            !string.Equals(evidence.AdmissionRequest.ManifestId, evidence.Manifest.ManifestId, StringComparison.Ordinal) ||
            !string.Equals(evidence.AdmissionRequest.ManifestDigest, evidence.ManifestDigest, StringComparison.OrdinalIgnoreCase))
        {
            failure = "INVALID_SUBJECT_EVIDENCE";
            return false;
        }

        if (!string.Equals(evidence.Manifest.ApplicationIdentity, evidence.SubjectKey.Identity, StringComparison.Ordinal) ||
            !string.Equals(evidence.Manifest.ApplicationVersion, evidence.SubjectKey.Version, StringComparison.Ordinal) ||
            !string.Equals(evidence.Manifest.ApplicationOwner, evidence.AdmissionRequest.Owner, StringComparison.Ordinal))
        {
            failure = "INVALID_SUBJECT_EVIDENCE";
            return false;
        }

        if (string.IsNullOrWhiteSpace(evidence.AdmissionEvidenceIdentity) ||
            !string.Equals(evidence.SubjectKey.Identity, evidence.AdmissionRequest.Identity, StringComparison.Ordinal) ||
            !string.Equals(evidence.SubjectKey.Version, evidence.AdmissionRequest.Version, StringComparison.Ordinal) ||
            !string.Equals(evidence.AdmissionDecision.EvidenceId, evidence.AdmissionEvidenceIdentity, StringComparison.Ordinal) ||
            !string.Equals(evidence.EvidenceReference, evidence.AdmissionEvidenceIdentity, StringComparison.Ordinal))
        {
            failure = "INVALID_SUBJECT_EVIDENCE";
            return false;
        }

        if (!string.Equals(evidence.ManifestDigest, evidence.Manifest.ComputeDigest(), StringComparison.OrdinalIgnoreCase))
        {
            failure = "INVALID_SUBJECT_EVIDENCE";
            return false;
        }

        if (!string.Equals(evidence.AdmissionRequest.ManifestDigest, evidence.ManifestDigest, StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(evidence.AdmissionRequest.ContractVersion, evidence.AdmissionDecision.ContractVersion, StringComparison.Ordinal) ||
            !string.Equals(evidence.AdmissionRequest.ContractId, evidence.AdmissionDecision.ContractId, StringComparison.Ordinal))
        {
            failure = "INVALID_SUBJECT_EVIDENCE";
            return false;
        }

        return true;
    }

    private static bool ValidateExternalEvidence(ExternalDependencySubjectEvidence evidence, DateTimeOffset observationTime, out string? failure)
    {
        failure = null;
        if (string.IsNullOrWhiteSpace(evidence.Owner) ||
            string.IsNullOrWhiteSpace(evidence.Source) ||
            string.IsNullOrWhiteSpace(evidence.IntegrityDigest) ||
            string.IsNullOrWhiteSpace(evidence.AvailabilityResult) ||
            string.IsNullOrWhiteSpace(evidence.ContainmentEvidence))
        {
            failure = "INVALID_SUBJECT_EVIDENCE";
            return false;
        }

        if (!IsHexDigest(evidence.IntegrityDigest))
        {
            failure = "INVALID_SUBJECT_EVIDENCE";
            return false;
        }

        if (!string.Equals(evidence.AvailabilityResult, "AVAILABLE", StringComparison.Ordinal))
        {
            failure = "DEPENDENCY_UNAVAILABLE";
            return false;
        }

        if (evidence.EffectiveTime == default || evidence.Expiry <= evidence.EffectiveTime)
        {
            failure = "INVALID_SUBJECT_EVIDENCE";
            return false;
        }

        if (evidence.EffectiveTime > observationTime)
        {
            failure = "INVALID_SUBJECT_EVIDENCE";
            return false;
        }

        if (evidence.Expiry <= observationTime)
        {
            failure = "SUBJECT_EVIDENCE_EXPIRED";
            return false;
        }

        return true;
    }

    private static string? ValidateManifestCoverage(
        IReadOnlyCollection<DependencySubjectEvidence> subjects,
        IReadOnlyDictionary<string, List<DependencyDeclaration>> dependenciesByConsumer,
        DependencyGraphRequest request)
    {
        foreach (var subject in subjects)
        {
            var consumerKey = SubjectKey(subject.SubjectKey);
            dependenciesByConsumer.TryGetValue(consumerKey, out var declaredDependencies);
            declaredDependencies ??= new List<DependencyDeclaration>();

            switch (subject)
            {
                case FoundationServiceSubjectEvidence foundation:
                    {
                        var expected = foundation.CatalogEntry.Manifest.Dependencies
                            .Select(dependency => new
                            {
                                Identity = dependency.Identity,
                                Versions = dependency.CompatibleVersions.OrderBy(version => version, StringComparer.Ordinal).ToArray(),
                                Kind = dependency.Kind,
                                Relation = dependency.Relation,
                                Purpose = dependency.Purpose,
                                DegradedBehavior = dependency.DegradedBehavior,
                                LifecycleOrder = dependency.LifecycleOrder
                            })
                            .OrderBy(value => value.Identity, StringComparer.Ordinal)
                            .ThenBy(value => JoinVersions(value.Versions), StringComparer.Ordinal)
                            .ToArray();

                        var actual = declaredDependencies
                            .Where(declaration => declaration.Consumer.Identity == subject.SubjectKey.Identity && declaration.Consumer.Version == subject.SubjectKey.Version)
                            .Select(declaration => new
                            {
                                Identity = declaration.DependencyIdentity,
                                Versions = declaration.CompatibleVersions.OrderBy(version => version, StringComparer.Ordinal).ToArray(),
                                Kind = ToManifestDependencyKindText(declaration.DependencyKind),
                                Relation = ToManifestRelationshipText(declaration.Relationship),
                                Purpose = declaration.DeclaredPurpose,
                                DegradedBehavior = declaration.DegradedStatePolicy,
                                LifecycleOrder = SerializeLifecycleOrder(declaration.LifecycleOrder)
                            })
                            .OrderBy(value => value.Identity, StringComparer.Ordinal)
                            .ThenBy(value => JoinVersions(value.Versions), StringComparer.Ordinal)
                            .ToArray();

                        if (expected.Length != actual.Length)
                        {
                            return expected.Length > actual.Length ? "MISSING_DECLARED_DEPENDENCY" : "HIDDEN_DEPENDENCY";
                        }

                        for (var index = 0; index < expected.Length; index++)
                        {
                            if (!string.Equals(expected[index].Identity, actual[index].Identity, StringComparison.Ordinal) ||
                                !string.Equals(JoinVersions(expected[index].Versions), JoinVersions(actual[index].Versions), StringComparison.Ordinal) ||
                                !string.Equals(expected[index].Kind, actual[index].Kind, StringComparison.Ordinal) ||
                                !string.Equals(expected[index].Relation, actual[index].Relation, StringComparison.Ordinal) ||
                                !string.Equals(expected[index].Purpose, actual[index].Purpose, StringComparison.Ordinal) ||
                                !string.Equals(expected[index].DegradedBehavior, actual[index].DegradedBehavior, StringComparison.Ordinal) ||
                                !string.Equals(expected[index].LifecycleOrder, actual[index].LifecycleOrder, StringComparison.Ordinal))
                            {
                                return "DEPENDENCY_DECLARATION_MISMATCH";
                            }
                        }

                        break;
                    }
                case ApplicationSubjectEvidence application:
                    {
                        var expectedContract = application.Manifest.DeclaredDependencies
                            .Select(dependency => (Identity: dependency.Identity, Versions: dependency.CompatibleVersions.OrderBy(version => version, StringComparer.Ordinal).ToArray(), Purpose: (string?)null))
                            .ToArray();

                        var expectedService = application.Manifest.RequiredFoundationServices
                            .Select(service => (Identity: service.Identity, Versions: new[] { service.Version }, Purpose: service.Purpose))
                            .ToArray();

                        if (HasDuplicateApplicationDependencyMappings(expectedContract.Select(value => (Identity: value.Identity, Versions: value.Versions, Purpose: (string?)value.Purpose)), expectedService.Select(value => (Identity: value.Identity, Versions: value.Versions, Purpose: (string?)value.Purpose))))
                        {
                            return "AMBIGUOUS_DEPENDENCY_DECLARATION";
                        }

                        var expected = expectedContract.Select(value => (Identity: value.Identity, Versions: value.Versions, Purpose: (string?)value.Purpose)).Concat(expectedService.Select(value => (Identity: value.Identity, Versions: value.Versions, Purpose: (string?)value.Purpose)))
                            .OrderBy(value => value.Identity, StringComparer.Ordinal)
                            .ThenBy(value => JoinVersions(value.Versions), StringComparer.Ordinal)
                            .ToArray();

                        var actualContract = declaredDependencies
                            .Where(declaration => declaration.DependencyKind != DependencySubjectKind.FoundationService)
                            .Select(declaration => (Identity: declaration.DependencyIdentity, Versions: declaration.CompatibleVersions.OrderBy(version => version, StringComparer.Ordinal).ToArray(), Purpose: (string?)null))
                            .ToArray();

                        var actualService = declaredDependencies
                            .Where(declaration => declaration.DependencyKind == DependencySubjectKind.FoundationService)
                            .Select(declaration => (Identity: declaration.DependencyIdentity, Versions: declaration.CompatibleVersions.OrderBy(version => version, StringComparer.Ordinal).ToArray(), Purpose: declaration.DeclaredPurpose))
                            .OrderBy(value => value.Identity, StringComparer.Ordinal)
                            .ThenBy(value => JoinVersions(value.Versions), StringComparer.Ordinal)
                            .ToArray();

                        if (HasDuplicateApplicationDependencyMappings(actualContract.Select(value => (Identity: value.Identity, Versions: value.Versions, Purpose: (string?)value.Purpose)), actualService.Select(value => (Identity: value.Identity, Versions: value.Versions, Purpose: (string?)value.Purpose))))
                        {
                            return "AMBIGUOUS_DEPENDENCY_DECLARATION";
                        }

                        var actual = actualContract.Select(value => (Identity: value.Identity, Versions: value.Versions, Purpose: (string?)value.Purpose))
                            .Concat(actualService.Select(value => (Identity: value.Identity, Versions: value.Versions, Purpose: (string?)value.Purpose)))
                            .OrderBy(value => value.Identity, StringComparer.Ordinal)
                            .ThenBy(value => JoinVersions(value.Versions), StringComparer.Ordinal)
                            .ToArray();

                        if (expected.Length != actual.Length)
                        {
                            return expected.Length > actual.Length ? "MISSING_DECLARED_DEPENDENCY" : "HIDDEN_DEPENDENCY";
                        }

                        for (var index = 0; index < expected.Length; index++)
                        {
                            if (!string.Equals(expected[index].Identity, actual[index].Identity, StringComparison.Ordinal) ||
                                !string.Equals(JoinVersions(expected[index].Versions), JoinVersions(actual[index].Versions), StringComparison.Ordinal) ||
                                !string.Equals(expected[index].Purpose ?? string.Empty, actual[index].Purpose ?? string.Empty, StringComparison.Ordinal))
                            {
                                return "DEPENDENCY_DECLARATION_MISMATCH";
                            }
                        }

                        break;
                    }
            }
        }

        return null;
    }

    private static string? ValidateDependencyDeclarations(
        IReadOnlyCollection<DependencySubjectEvidence> subjects,
        IReadOnlyCollection<DependencyDeclaration> declarations,
        IReadOnlyDictionary<string, DependencySubjectEvidence> subjectMap,
        DateTimeOffset observationTime,
        string expectedDelegationChainIdentity,
        out List<DependencyResolutionSnapshot> edges,
        out List<DependencyResolutionSnapshot> resolvedDependencies,
        out List<DependencyResolutionSnapshot> unresolvedOptionalDependencies,
        out string? failure,
        out string? cycleEvidence)
    {
        edges = new List<DependencyResolutionSnapshot>();
        resolvedDependencies = new List<DependencyResolutionSnapshot>();
        unresolvedOptionalDependencies = new List<DependencyResolutionSnapshot>();
        failure = null;
        cycleEvidence = null;

        var seenEdges = new HashSet<string>(StringComparer.Ordinal);
        var adjacency = new Dictionary<DependencyNodeKey, List<DependencyNodeKey>>();
        var nodeKeys = subjects.Select(NodeKey).Distinct().OrderBy(value => value.CanonicalText, StringComparer.Ordinal).ToArray();

        foreach (var declaration in declarations)
        {
            if (!IsDefinedEnum(declaration.DependencyKind))
            {
                failure = "INVALID_SUBJECT_KIND";
                return failure;
            }

            if (!IsDefinedEnum(declaration.Relationship))
            {
                failure = "INVALID_RELATIONSHIP";
                return failure;
            }

            if (declaration.ConditionState.HasValue && !IsDefinedEnum(declaration.ConditionState.Value))
            {
                failure = "INVALID_CONDITION_STATE";
                return failure;
            }

            if (declaration.LifecycleOrder is null)
            {
                failure = "MISSING_LIFECYCLE_ORDER_DECLARATION";
                return failure;
            }

            if (!ValidateLifecycleOrder(declaration.LifecycleOrder, out failure))
            {
                return failure;
            }

            var consumerKey = ResolveNodeKey(subjects, declaration.Consumer, out var consumerFailure);
            if (consumerFailure is not null)
            {
                failure = consumerFailure;
                return failure;
            }

            if (declaration.Consumer.Identity == declaration.DependencyIdentity &&
                declaration.Consumer.Version == declaration.ResolvedVersion &&
                declaration.Relationship != DependencyRelationship.Prohibited)
            {
                failure = "DIRECT_SELF_DEPENDENCY";
                return failure;
            }

            var declarationKey = string.Join("|", new[]
            {
                consumerKey.CanonicalText,
                declaration.DependencyKind.ToString(),
                declaration.DependencyIdentity,
                declaration.DependencyKind.ToString()
            });

            if (!seenEdges.Add(declarationKey))
            {
                failure = "DUPLICATE_DEPENDENCY";
                return failure;
            }

            if (declaration.Relationship == DependencyRelationship.Conditional && declaration.ConditionState is null)
            {
                failure = "INVALID_CONDITION_STATE";
                return failure;
            }

            if ((declaration.Relationship is DependencyRelationship.Required or DependencyRelationship.Optional or DependencyRelationship.Prohibited) && declaration.ConditionState is not null)
            {
                failure = "INVALID_RELATIONSHIP";
                return failure;
            }

            if (declaration.Relationship == DependencyRelationship.Prohibited && !string.IsNullOrWhiteSpace(declaration.ResolvedVersion))
            {
                failure = "PROHIBITED_DEPENDENCY_PRESENT";
                return failure;
            }

            if (string.IsNullOrWhiteSpace(declaration.DelegationChainEvidenceReference))
            {
                failure = "MISSING_EVIDENCE_REFERENCE";
                return failure;
            }

            if (!string.Equals(declaration.DelegationChainEvidenceReference, expectedDelegationChainIdentity, StringComparison.Ordinal))
            {
                failure = "DELEGATION_CHAIN_MISMATCH";
                return failure;
            }

            if (declaration.Relationship == DependencyRelationship.Conditional && declaration.ConditionState == DependencyConditionState.RequiredNow && string.IsNullOrWhiteSpace(declaration.ResolvedVersion))
            {
                failure = "CONDITIONAL_DEPENDENCY_UNRESOLVED";
                return failure;
            }

            if (declaration.Relationship == DependencyRelationship.Conditional && declaration.ConditionState == DependencyConditionState.NotRequiredNow)
            {
                if (!string.IsNullOrWhiteSpace(declaration.ResolvedVersion))
                {
                    failure = "UNRESOLVED_VERSION_CONFLICT";
                    return failure;
                }

                var conditionalSnapshot = new DependencyResolutionSnapshot
                {
                    EdgeKey = new DependencyEdgeKey(declaration.Consumer, new DependencySubjectKey(declaration.DependencyIdentity, string.Empty), declaration.Relationship, declaration.ConditionState, declaration.ResolvedVersion),
                    DependencyKind = declaration.DependencyKind,
                    DependencySource = declaration.DependencySource,
                    DeclaredPurpose = declaration.DeclaredPurpose,
                    IntegrityRequirement = declaration.IntegrityRequirement,
                    AvailabilityRequirement = declaration.AvailabilityRequirement,
                    TimeoutPolicy = declaration.TimeoutPolicy,
                    DegradedStatePolicy = declaration.DegradedStatePolicy,
                    IsolationBoundary = declaration.IsolationBoundary,
                    FailurePropagationLimit = declaration.FailurePropagationLimit,
                    ReplacementPolicy = declaration.ReplacementPolicy,
                    MigrationPolicy = declaration.MigrationPolicy,
                    RollbackPolicy = declaration.RollbackPolicy,
                    EvidenceRequirement = declaration.EvidenceRequirement,
                    DelegationChainEvidenceReference = declaration.DelegationChainEvidenceReference,
                    CompatibleVersions = new ReadOnlyCollection<string>(FreezeStrings(declaration.CompatibleVersions).ToList()),
                    Relationship = declaration.Relationship,
                    ConditionState = declaration.ConditionState,
                    ResolvedVersion = declaration.ResolvedVersion
                };
                unresolvedOptionalDependencies.Add(conditionalSnapshot);
                continue;
            }

            if (declaration.Relationship == DependencyRelationship.Prohibited)
            {
                continue;
            }

            if (declaration.Relationship == DependencyRelationship.Required && string.IsNullOrWhiteSpace(declaration.ResolvedVersion))
            {
                failure = "MISSING_RESOLVED_VERSION";
                return failure;
            }

            if (declaration.Relationship == DependencyRelationship.Required || (declaration.Relationship == DependencyRelationship.Conditional && declaration.ConditionState == DependencyConditionState.RequiredNow) || (!string.IsNullOrWhiteSpace(declaration.ResolvedVersion) && declaration.Relationship == DependencyRelationship.Optional))
            {
                if (!declaration.CompatibleVersions.Contains(declaration.ResolvedVersion ?? string.Empty, StringComparer.Ordinal))
                {
                    failure = "RESOLVED_VERSION_NOT_COMPATIBLE";
                    return failure;
                }
            }

            if (declaration.Relationship == DependencyRelationship.Optional && string.IsNullOrWhiteSpace(declaration.ResolvedVersion))
            {
                var optionalSnapshot = new DependencyResolutionSnapshot
                {
                    EdgeKey = new DependencyEdgeKey(declaration.Consumer, new DependencySubjectKey(declaration.DependencyIdentity, string.Empty), declaration.Relationship, declaration.ConditionState, declaration.ResolvedVersion),
                    DependencyKind = declaration.DependencyKind,
                    DependencySource = declaration.DependencySource,
                    DeclaredPurpose = declaration.DeclaredPurpose,
                    IntegrityRequirement = declaration.IntegrityRequirement,
                    AvailabilityRequirement = declaration.AvailabilityRequirement,
                    TimeoutPolicy = declaration.TimeoutPolicy,
                    DegradedStatePolicy = declaration.DegradedStatePolicy,
                    IsolationBoundary = declaration.IsolationBoundary,
                    FailurePropagationLimit = declaration.FailurePropagationLimit,
                    ReplacementPolicy = declaration.ReplacementPolicy,
                    MigrationPolicy = declaration.MigrationPolicy,
                    RollbackPolicy = declaration.RollbackPolicy,
                    EvidenceRequirement = declaration.EvidenceRequirement,
                    DelegationChainEvidenceReference = declaration.DelegationChainEvidenceReference,
                    CompatibleVersions = new ReadOnlyCollection<string>(FreezeStrings(declaration.CompatibleVersions).ToList()),
                    Relationship = declaration.Relationship,
                    ConditionState = declaration.ConditionState,
                    ResolvedVersion = declaration.ResolvedVersion
                };
                unresolvedOptionalDependencies.Add(optionalSnapshot);
                continue;
            }

            var dependency = ResolveDependencySubject(subjects, declaration.DependencyKind, declaration.DependencyIdentity, declaration.ResolvedVersion ?? declaration.CompatibleVersions[0], observationTime, out var dependencyFailure);
            if (dependencyFailure is not null)
            {
                failure = dependencyFailure;
                return failure;
            }

            var edgeKey = new DependencyEdgeKey(declaration.Consumer, dependency.SubjectKey, declaration.Relationship, declaration.ConditionState, declaration.ResolvedVersion);

            if (!declaration.CompatibleVersions.Contains(declaration.ResolvedVersion ?? dependency.SubjectKey.Version, StringComparer.Ordinal))
            {
                failure = "RESOLVED_VERSION_NOT_COMPATIBLE";
                return failure;
            }

            var edge = new DependencyResolutionSnapshot
            {
                EdgeKey = edgeKey,
                DependencyKind = declaration.DependencyKind,
                DependencySource = declaration.DependencySource,
                DeclaredPurpose = declaration.DeclaredPurpose,
                IntegrityRequirement = declaration.IntegrityRequirement,
                AvailabilityRequirement = declaration.AvailabilityRequirement,
                TimeoutPolicy = declaration.TimeoutPolicy,
                DegradedStatePolicy = declaration.DegradedStatePolicy,
                IsolationBoundary = declaration.IsolationBoundary,
                FailurePropagationLimit = declaration.FailurePropagationLimit,
                ReplacementPolicy = declaration.ReplacementPolicy,
                MigrationPolicy = declaration.MigrationPolicy,
                RollbackPolicy = declaration.RollbackPolicy,
                EvidenceRequirement = declaration.EvidenceRequirement,
                DelegationChainEvidenceReference = declaration.DelegationChainEvidenceReference,
                CompatibleVersions = new ReadOnlyCollection<string>(FreezeStrings(declaration.CompatibleVersions).ToList()),
                Relationship = declaration.Relationship,
                ConditionState = declaration.ConditionState,
                ResolvedVersion = declaration.ResolvedVersion ?? dependency.SubjectKey.Version
            };
            edges.Add(edge);
            resolvedDependencies.Add(edge);

            var dependencyNodeKey = NodeKey(dependency);
            if (!adjacency.TryGetValue(dependencyNodeKey, out var consumers))
            {
                consumers = new List<DependencyNodeKey>();
                adjacency.Add(dependencyNodeKey, consumers);
            }

            consumers.Add(consumerKey);
        }

        if (DetectCycle(nodeKeys, adjacency, out cycleEvidence))
        {
            failure = "CIRCULAR_DEPENDENCY";
            return failure;
        }

        return null;
    }

    private static DependencySubjectEvidence ResolveDependencySubject(
        IReadOnlyCollection<DependencySubjectEvidence> subjects,
        DependencySubjectKind dependencyKind,
        string dependencyIdentity,
        string resolvedVersion,
        DateTimeOffset observationTime,
        out string? failure)
    {
        failure = null;
        var exactMatches = subjects
            .Where(subject => subject.SubjectKind == dependencyKind &&
                              string.Equals(subject.SubjectKey.Identity, dependencyIdentity, StringComparison.Ordinal) &&
                              string.Equals(subject.SubjectKey.Version, resolvedVersion, StringComparison.Ordinal))
            .ToArray();

        if (exactMatches.Length == 1)
        {
            if (exactMatches[0] is ExternalDependencySubjectEvidence external && external.Expiry <= observationTime)
            {
                failure = "SUBJECT_EVIDENCE_EXPIRED";
                return null!;
            }

            return exactMatches[0];
        }

        if (exactMatches.Length > 1)
        {
            failure = "UNRESOLVED_VERSION_CONFLICT";
            return null!;
        }

        var sameDependencyMatches = subjects
            .Where(subject => subject.SubjectKind == dependencyKind &&
                              string.Equals(subject.SubjectKey.Identity, dependencyIdentity, StringComparison.Ordinal))
            .ToArray();

        if (sameDependencyMatches.Length == 0)
        {
            failure = "UNKNOWN_DEPENDENCY";
            return null!;
        }

        failure = "RESOLVED_VERSION_NOT_COMPATIBLE";
        return null!;
    }

    private static bool DetectCycle(
        IReadOnlyList<DependencyNodeKey> nodeKeys,
        IReadOnlyDictionary<DependencyNodeKey, List<DependencyNodeKey>> adjacency,
        out string? cycleEvidence)
    {
        var foundCycleEvidence = string.Empty;
        var visited = new HashSet<DependencyNodeKey>();
        var stack = new HashSet<DependencyNodeKey>();
        var path = new List<DependencyNodeKey>();

        foreach (var node in nodeKeys)
        {
            if (Visit(node))
            {
                cycleEvidence = foundCycleEvidence;
                return true;
            }
        }

        cycleEvidence = null;
        return false;

        bool Visit(DependencyNodeKey node)
        {
            if (stack.Contains(node))
            {
                var cycleStart = path.IndexOf(node);
                if (cycleStart >= 0)
                {
                    var cycle = path.Skip(cycleStart).Concat(new[] { node }).ToArray();
                    foundCycleEvidence = string.Join(" -> ", cycle.Select(value => value.CanonicalText));
                }
                else
                {
                    foundCycleEvidence = node.CanonicalText;
                }

                return true;
            }

            if (!visited.Add(node))
            {
                return false;
            }

            stack.Add(node);
            path.Add(node);
            if (adjacency.TryGetValue(node, out var consumers))
            {
                foreach (var next in consumers.OrderBy(value => value.CanonicalText, StringComparer.Ordinal))
                {
                    if (Visit(next))
                    {
                        return true;
                    }
                }
            }

            path.RemoveAt(path.Count - 1);
            stack.Remove(node);
            return false;
        }
    }

    private static List<DependencySubjectKey> ComputeCanonicalOrder(
        IReadOnlyCollection<DependencySubjectEvidence> subjects,
        IReadOnlyCollection<DependencyResolutionSnapshot> edges,
        out string? failure,
        out string? edgeViolation)
    {
        failure = null;
        edgeViolation = null;

        var subjectNodes = subjects.Select(NodeKey).Distinct().OrderBy(key => key.CanonicalText, StringComparer.Ordinal).ToList();
        var subjectNodeLookup = subjectNodes.ToDictionary(key => key.CanonicalText, key => key, StringComparer.Ordinal);
        var inDegree = subjectNodes.ToDictionary(key => key.CanonicalText, _ => 0, StringComparer.Ordinal);
        var adjacency = subjectNodes.ToDictionary(key => key.CanonicalText, _ => new List<string>(), StringComparer.Ordinal);

        foreach (var edge in edges)
        {
            var sourceMatch = subjectNodes.Where(key => string.Equals(key.Identity, edge.EdgeKey.Dependency.Identity, StringComparison.Ordinal) &&
                                                        string.Equals(key.Version, edge.EdgeKey.Dependency.Version, StringComparison.Ordinal)).ToArray();
            var targetMatch = subjectNodes.Where(key => string.Equals(key.Identity, edge.EdgeKey.Consumer.Identity, StringComparison.Ordinal) &&
                                                        string.Equals(key.Version, edge.EdgeKey.Consumer.Version, StringComparison.Ordinal)).ToArray();
            if (sourceMatch.Length != 1 || targetMatch.Length != 1)
            {
                failure = "INVALID_GRAPH_REQUEST";
                return new List<DependencySubjectKey>();
            }

            var source = sourceMatch[0];
            var target = targetMatch[0];
            adjacency[source.CanonicalText].Add(target.CanonicalText);
            inDegree[target.CanonicalText]++;
        }

        var queue = new SortedSet<string>(subjectNodes.Where(key => inDegree[key.CanonicalText] == 0).Select(key => key.CanonicalText), StringComparer.Ordinal);
        var ordered = new List<DependencySubjectKey>(subjectNodes.Count);

        while (queue.Count > 0)
        {
            var current = queue.Min!;
            queue.Remove(current);
            var node = subjectNodeLookup[current];
            ordered.Add(new DependencySubjectKey(node.Identity, node.Version));

            foreach (var consumer in adjacency[current].OrderBy(value => value, StringComparer.Ordinal))
            {
                inDegree[consumer]--;
                if (inDegree[consumer] == 0)
                {
                    queue.Add(consumer);
                }
            }
        }

        if (ordered.Count != subjectNodes.Count)
        {
            failure = "CIRCULAR_DEPENDENCY";
            edgeViolation ??= string.Empty;
            return new List<DependencySubjectKey>();
        }

        return ordered;
    }

    private static DependencyValidationResult ValidateActivationOrder(
        IReadOnlyList<DependencySubjectKey> proposedOrder,
        IReadOnlyList<DependencySubjectKey> canonicalOrder,
        IReadOnlyCollection<DependencyResolutionSnapshot> edges,
        out string? failure)
    {
        failure = null;
        if (proposedOrder is null || proposedOrder.Count == 0)
        {
            failure = "MISSING_ACTIVATION_ORDER";
            return Fail(failure);
        }

        var canonicalSet = canonicalOrder.Select(key => key.CanonicalText).ToHashSet(StringComparer.Ordinal);
        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (var subject in proposedOrder)
        {
            if (!canonicalSet.Contains(subject.CanonicalText))
            {
                failure = "UNKNOWN_ACTIVATION_SUBJECT";
                return Fail(failure);
            }

            if (!seen.Add(subject.CanonicalText))
            {
                failure = "DUPLICATE_ACTIVATION_SUBJECT";
                return Fail(failure);
            }
        }

        if (proposedOrder.Count != canonicalOrder.Count)
        {
            failure = "INCOMPLETE_ACTIVATION_ORDER";
            return Fail(failure);
        }

        var proposedKeys = proposedOrder.Select(key => key.CanonicalText).ToArray();
        for (var index = 0; index < canonicalOrder.Count; index++)
        {
            if (!string.Equals(proposedKeys[index], canonicalOrder[index].CanonicalText, StringComparison.Ordinal))
            {
                if (HasDependencyAfterConsumer(proposedOrder, edges))
                {
                    failure = "DEPENDENCY_AFTER_CONSUMER";
                    return Fail(failure);
                }

                failure = "NON_CANONICAL_ACTIVATION_ORDER";
                return Fail(failure);
            }
        }

        if (HasDependencyAfterConsumer(proposedOrder, edges))
        {
            failure = "DEPENDENCY_AFTER_CONSUMER";
            return Fail(failure);
        }

        return new DependencyValidationResult
        {
            Success = true,
            ReasonCode = "ACTIVATION_ORDER_VALIDATED",
            GraphDecision = "DEPENDENCY_GRAPH_VALIDATED",
            ActivationOrderDecision = "ACTIVATION_ORDER_VALIDATED"
        };
    }

    private static bool HasDependencyAfterConsumer(IReadOnlyList<DependencySubjectKey> proposedOrder, IReadOnlyCollection<DependencyResolutionSnapshot> edges)
    {
        var orderIndex = proposedOrder
            .Select((subject, index) => new { subject.CanonicalText, index })
            .ToDictionary(item => item.CanonicalText, item => item.index, StringComparer.Ordinal);

        foreach (var edge in edges)
        {
            if (!orderIndex.TryGetValue(edge.EdgeKey.Dependency.CanonicalText, out var dependencyIndex) ||
                !orderIndex.TryGetValue(edge.EdgeKey.Consumer.CanonicalText, out var consumerIndex))
            {
                return true;
            }

            if (dependencyIndex > consumerIndex)
            {
                return true;
            }
        }

        return false;
    }

    private static IReadOnlyList<string> FreezeStrings(IReadOnlyList<string> values)
        => values is null ? Array.Empty<string>() : values.ToArray();

    private static IReadOnlyList<DependencySubjectKey> FreezeOrder(IReadOnlyList<DependencySubjectKey> order, out string? failure)
    {
        failure = null;
        if (order is null)
        {
            failure = "MISSING_ACTIVATION_ORDER";
            return Array.Empty<DependencySubjectKey>();
        }

        return order.ToArray();
    }

    private static string SerializeManifestSurface(ManifestSurfaceRecord record, bool includeDigest)
        => string.Join("\n", new[]
        {
            SerializeField("ManifestId", record.ManifestId),
            SerializeField("Version", record.Version),
            SerializeField("ManifestClass", record.ManifestClass),
            SerializeField("SubjectId", record.SubjectId),
            SerializeField("EvidenceSetId", record.EvidenceSetId),
            SerializeField("SeparationResult", record.SeparationResult),
            SerializeField("IntegrityResult", record.IntegrityResult),
            SerializeField("AuthorityReference", record.AuthorityReference),
            SerializeField("ValidationEvidence", record.ValidationEvidence),
            SerializeField("CanonicalDigest", includeDigest ? record.CanonicalDigest : string.Empty),
            SerializeField("EffectiveTime", record.EffectiveTime.ToString("O", CultureInfo.InvariantCulture)),
            SerializeField("Expiry", record.Expiry.ToString("O", CultureInfo.InvariantCulture))
        }) + "\n";

    private static string SerializeGraphSnapshot(
        DependencyGraphRequest request,
        IReadOnlyCollection<DependencySubjectEvidence> subjects,
        IReadOnlyCollection<DependencyDeclaration> dependencies,
        IReadOnlyCollection<DependencyResolutionSnapshot> resolvedDependencies,
        IReadOnlyCollection<DependencyResolutionSnapshot> unresolvedOptionalDependencies,
        IReadOnlyCollection<DependencySubjectKey> canonicalOrder)
    {
        var builder = new StringBuilder();
        Append(builder, "GraphId", request.GraphId);
        Append(builder, "GraphVersion", request.GraphVersion);
        Append(builder, "RequesterIdentity", request.RequesterIdentity);
        Append(builder, "AuthoritySource", request.AuthoritySource);
        Append(builder, "ObservationTime", request.ObservationTime.ToString("O", CultureInfo.InvariantCulture));
        Append(builder, "ManifestDigest", request.ManifestSurface.CanonicalDigest);
        Append(builder, "DelegationEvidenceReference", request.DelegationEvidence.ValidationEvidence);
        Append(builder, "Subjects", string.Join(";", subjects
            .Select(SerializeSubject)
            .OrderBy(value => value, StringComparer.Ordinal)));
        Append(builder, "Dependencies", string.Join(";", dependencies
            .Select(SerializeDependency)
            .OrderBy(value => value, StringComparer.Ordinal)));
        Append(builder, "ResolvedDependencies", string.Join(";", resolvedDependencies
            .Select(SerializeResolution)
            .OrderBy(value => value, StringComparer.Ordinal)));
        Append(builder, "UnresolvedOptionalDependencies", string.Join(";", unresolvedOptionalDependencies
            .Select(SerializeResolution)
            .OrderBy(value => value, StringComparer.Ordinal)));
        Append(builder, "ActivationOrder", string.Join(";", canonicalOrder.Select(value => value.CanonicalText)));
        return builder.ToString();
    }

    private static DependencySubjectSnapshot CreateSubjectSnapshot(DependencySubjectEvidence subject)
        => subject switch
        {
            FoundationServiceSubjectEvidence foundation => new DependencySubjectSnapshot
            {
                SubjectKind = foundation.SubjectKind,
                SubjectKey = foundation.SubjectKey,
                EvidenceReference = foundation.EvidenceReference,
                Owner = foundation.CatalogEntry.Registration.AccountableOwner,
                Source = foundation.CatalogEntry.Registration.RegistrationId,
                IntegrityDigest = foundation.CatalogEntry.Registration.ManifestDigest,
                AvailabilityResult = foundation.CatalogEntry.Registration.OperationalState.ToString(),
                ContainmentEvidence = foundation.CatalogEntry.Registration.RegistrationState.ToString(),
                EffectiveTime = DateTimeOffset.UnixEpoch.AddSeconds(foundation.CatalogEntry.Registration.RegistrationSequence),
                Expiry = DateTimeOffset.UnixEpoch.AddSeconds(foundation.CatalogEntry.Registration.RegistrationSequence + 1),
                ServiceCatalogIdentity = foundation.CatalogEntry.Key.ServiceIdentity,
                ServiceCatalogVersion = foundation.CatalogEntry.Key.ServiceVersion,
                ServiceCatalogOwner = foundation.CatalogEntry.Registration.AccountableOwner,
                ServiceCatalogManifestIdentity = foundation.CatalogEntry.Manifest.ManifestId,
                ServiceCatalogManifestDigest = foundation.CatalogEntry.Manifest.ComputeDigest()
            },
            ApplicationSubjectEvidence application => new DependencySubjectSnapshot
            {
                SubjectKind = application.SubjectKind,
                SubjectKey = application.SubjectKey,
                EvidenceReference = application.EvidenceReference,
                AdmissionEvidenceIdentity = application.AdmissionEvidenceIdentity,
                ManifestIdentity = application.Manifest.ManifestId,
                ManifestDigest = application.ManifestDigest,
                AdmissionRequestIdentity = application.AdmissionRequest.AdmissionId,
                AdmissionDecisionIdentity = application.AdmissionDecision.AdmissionId,
                AdmissionDecisionReason = application.AdmissionDecision.ReasonCode,
                AdmissionRequestProvenanceId = application.AdmissionRequest.ProvenanceId,
                ManifestDeclaredDependencies = new ReadOnlyCollection<string>(
                    application.Manifest.DeclaredDependencies.SelectMany(dependency => new[] { dependency.Identity, string.Join(",", dependency.CompatibleVersions) }).ToArray()),
                ManifestRequiredFoundationServices = new ReadOnlyCollection<string>(
                    application.Manifest.RequiredFoundationServices.Select(service => $"{service.Identity}|{service.Version}|{service.Purpose}").ToArray())
            },
            ExternalDependencySubjectEvidence external => new DependencySubjectSnapshot
            {
                SubjectKind = external.SubjectKind,
                SubjectKey = external.SubjectKey,
                EvidenceReference = external.EvidenceReference,
                Owner = external.Owner,
                Source = external.Source,
                IntegrityDigest = external.IntegrityDigest,
                AvailabilityResult = external.AvailabilityResult,
                ContainmentEvidence = external.ContainmentEvidence,
                EffectiveTime = external.EffectiveTime,
                Expiry = external.Expiry
            },
            _ => new DependencySubjectSnapshot
            {
                SubjectKind = subject.SubjectKind,
                SubjectKey = subject.SubjectKey,
                EvidenceReference = subject.EvidenceReference
            }
        };

    private static string SerializeCandidateGraphRequest(DependencyGraphRequest request)
    {
        var builder = new StringBuilder();
        Append(builder, "GraphId", request.GraphId);
        Append(builder, "GraphVersion", request.GraphVersion);
        Append(builder, "RequesterIdentity", request.RequesterIdentity);
        Append(builder, "AuthoritySource", request.AuthoritySource);
        Append(builder, "ObservationTime", request.ObservationTime.ToString("O", CultureInfo.InvariantCulture));
        Append(builder, "ManifestSurface", SerializeManifestSurface(request.ManifestSurface, includeDigest: false));
        Append(builder, "DelegationEvidenceReference", request.DelegationEvidence.ValidationEvidence);
        Append(builder, "Subjects", string.Join(";", request.Subjects
            .Select(SerializeSubject)
            .OrderBy(value => value, StringComparer.Ordinal)));
        Append(builder, "Dependencies", string.Join(";", request.Dependencies
            .Select(SerializeDependency)
            .OrderBy(value => value, StringComparer.Ordinal)));
        Append(builder, "ActivationOrder", string.Join(";", request.ProposedActivationOrder.Select(value => value.CanonicalText)));
        return builder.ToString();
    }

    private static string SerializeActivationOrder(string graphId, string graphVersion, IReadOnlyCollection<DependencySubjectKey> canonicalOrder)
    {
        var builder = new StringBuilder();
        Append(builder, "GraphId", graphId);
        Append(builder, "GraphVersion", graphVersion);
        Append(builder, "ActivationOrder", string.Join(";", canonicalOrder.Select(value => value.CanonicalText)));
        return builder.ToString();
    }

    private static string SerializeSubject(DependencySubjectEvidence subject)
        => subject switch
        {
            FoundationServiceSubjectEvidence foundation => string.Join("|", new[]
            {
                Canonical(subject.SubjectKind.ToString()),
                Canonical(subject.SubjectKey.CanonicalText),
                Canonical(subject.EvidenceReference),
                Canonical(foundation.CatalogEntry.Key.ServiceIdentity),
                Canonical(foundation.CatalogEntry.Key.ServiceVersion),
                Canonical(foundation.CatalogEntry.Registration.RegistrationId),
                Canonical(foundation.CatalogEntry.Registration.ReasonCode),
                Canonical(foundation.CatalogEntry.Registration.RegistrationState.ToString()),
                Canonical(foundation.CatalogEntry.Registration.OperationalState.ToString()),
                Canonical(foundation.CatalogEntry.Registration.ManifestDigest),
                Canonical(foundation.CatalogEntry.Registration.RegistrationEvidenceReference)
            }),
            ApplicationSubjectEvidence application => string.Join("|", new[]
            {
                Canonical(subject.SubjectKind.ToString()),
                Canonical(subject.SubjectKey.CanonicalText),
                Canonical(subject.EvidenceReference),
                Canonical(application.AdmissionRequest.AdmissionId),
                Canonical(application.AdmissionDecision.Decision),
                Canonical(application.Manifest.ManifestId),
                Canonical(application.ManifestDigest),
                Canonical(application.AdmissionEvidenceIdentity)
            }),
            ExternalDependencySubjectEvidence external => string.Join("|", new[]
            {
                Canonical(subject.SubjectKind.ToString()),
                Canonical(subject.SubjectKey.CanonicalText),
                Canonical(subject.EvidenceReference),
                Canonical(external.Owner),
                Canonical(external.Source),
                Canonical(external.IntegrityDigest),
                Canonical(external.AvailabilityResult),
                Canonical(external.ContainmentEvidence),
                Canonical(external.EffectiveTime.ToString("O", CultureInfo.InvariantCulture)),
                Canonical(external.Expiry.ToString("O", CultureInfo.InvariantCulture))
            }),
            _ => Canonical(subject.SubjectKind.ToString())
        };

    private static string SerializeDependency(DependencyDeclaration dependency)
        => string.Join("|", new[]
        {
            Canonical(dependency.Consumer.CanonicalText),
            Canonical(dependency.DependencyIdentity),
            Canonical(JoinVersions(dependency.CompatibleVersions)),
            Canonical(dependency.Relationship.ToString()),
            Canonical(dependency.ConditionState?.ToString() ?? string.Empty),
            Canonical(dependency.ResolvedVersion ?? string.Empty),
            Canonical(dependency.DependencyKind.ToString()),
            Canonical(dependency.DependencySource),
            Canonical(dependency.DeclaredPurpose),
            Canonical(dependency.IntegrityRequirement),
            Canonical(dependency.AvailabilityRequirement),
            Canonical(dependency.TimeoutPolicy),
            Canonical(dependency.DegradedStatePolicy),
            Canonical(dependency.IsolationBoundary),
            Canonical(dependency.FailurePropagationLimit),
            Canonical(dependency.ReplacementPolicy),
            Canonical(dependency.MigrationPolicy),
            Canonical(dependency.RollbackPolicy),
            Canonical(dependency.EvidenceRequirement),
            Canonical(dependency.DelegationChainEvidenceReference),
            Canonical(SerializeLifecycleOrder(dependency.LifecycleOrder))
        });

    private static string SerializeResolution(DependencyResolutionSnapshot resolution)
        => string.Join("|", new[]
        {
            Canonical(resolution.EdgeKey.CanonicalText),
            Canonical(resolution.DependencyKind.ToString()),
            Canonical(resolution.DependencySource),
            Canonical(resolution.DeclaredPurpose),
            Canonical(resolution.IntegrityRequirement),
            Canonical(resolution.AvailabilityRequirement),
            Canonical(resolution.TimeoutPolicy),
            Canonical(resolution.DegradedStatePolicy),
            Canonical(resolution.IsolationBoundary),
            Canonical(resolution.FailurePropagationLimit),
            Canonical(resolution.ReplacementPolicy),
            Canonical(resolution.MigrationPolicy),
            Canonical(resolution.RollbackPolicy),
            Canonical(resolution.EvidenceRequirement),
            Canonical(resolution.DelegationChainEvidenceReference),
            Canonical(JoinVersions(resolution.CompatibleVersions)),
            Canonical(resolution.Relationship.ToString()),
            Canonical(resolution.ConditionState?.ToString() ?? string.Empty),
            Canonical(resolution.ResolvedVersion ?? string.Empty),
            Canonical(SerializeLifecycleOrder(resolution.LifecycleOrder))
        });

    private static string JoinVersions(IEnumerable<string> versions)
        => string.Join(",", versions.OrderBy(version => version, StringComparer.Ordinal).Select(Canonical));

    private static void Append(StringBuilder builder, string name, string value)
    {
        builder.Append(Canonical(name));
        builder.Append('=');
        builder.Append(Canonical(value ?? string.Empty));
        builder.Append('\n');
    }

    private static string SerializeField(string name, string value)
        => string.Concat(Canonical(name), "=", Canonical(value ?? string.Empty));

    private static string Canonical(string value)
        => $"{(value ?? string.Empty).Length}:{value ?? string.Empty}";

    private static string SerializeLifecycleOrder(DependencyLifecycleOrderDeclaration? lifecycleOrder)
        => lifecycleOrder is null
            ? string.Empty
            : string.Join("|", new[]
            {
                Canonical(lifecycleOrder.Startup.ToString()),
                Canonical(lifecycleOrder.Shutdown.ToString()),
                Canonical(lifecycleOrder.Update.ToString()),
                Canonical(lifecycleOrder.Recovery.ToString()),
                Canonical(lifecycleOrder.Removal.ToString())
            });

    private static string ToManifestDependencyKindText(DependencySubjectKind kind)
        => kind switch
        {
            DependencySubjectKind.FoundationService => "foundation-service",
            DependencySubjectKind.Application => "application",
            DependencySubjectKind.External => "external",
            _ => kind.ToString().ToLowerInvariant()
        };

    private static string ToManifestRelationshipText(DependencyRelationship relationship)
        => relationship switch
        {
            DependencyRelationship.Required => "requires",
            DependencyRelationship.Optional => "optional",
            DependencyRelationship.Conditional => "conditional",
            DependencyRelationship.Prohibited => "prohibited",
            _ => relationship.ToString().ToLowerInvariant()
        };

    private static string ComputeSha256(string content)
        => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(content ?? string.Empty)));

    private static string ComputeDecisionIdentity(DependencyGraphRequest request, string graphDigest, string activationOrderDigest)
    {
        var builder = new StringBuilder();
        Append(builder, "GraphId", request.GraphId);
        Append(builder, "GraphVersion", request.GraphVersion);
        Append(builder, "RequesterIdentity", request.RequesterIdentity);
        Append(builder, "AuthoritySource", request.AuthoritySource);
        Append(builder, "ObservationTime", request.ObservationTime.ToString("O", CultureInfo.InvariantCulture));
        Append(builder, "CandidateManifestDigest", request.ManifestSurface.CanonicalDigest);
        Append(builder, "GraphDigest", graphDigest);
        Append(builder, "ActivationOrderDigest", activationOrderDigest);
        return ComputeSha256(builder.ToString());
    }

    private static DependencyNodeKey NodeKey(DependencySubjectEvidence subject)
        => new(subject.SubjectKind, subject.SubjectKey.Identity, subject.SubjectKey.Version);

    private static DependencyNodeKey NodeKey(DependencySubjectKind kind, DependencySubjectKey key)
        => new(kind, key.Identity, key.Version);

    private static string SubjectKey(DependencySubjectKey key)
        => key.CanonicalText;

    private static string SubjectKey(DependencySubjectKind kind, DependencySubjectKey key)
        => string.Concat(kind.ToString(), "|", key.CanonicalText);

    private static DependencyNodeKey ResolveNodeKey(IReadOnlyCollection<DependencySubjectEvidence> subjects, DependencySubjectKey key, out string? failure)
    {
        var matches = subjects
            .Where(subject => string.Equals(subject.SubjectKey.Identity, key.Identity, StringComparison.Ordinal) &&
                              string.Equals(subject.SubjectKey.Version, key.Version, StringComparison.Ordinal))
            .ToArray();

        if (matches.Length == 0)
        {
            failure = "UNKNOWN_CONSUMER";
            return default;
        }

        if (matches.Length > 1)
        {
            if (matches.Select(subject => subject.SubjectKind).Distinct().Count() > 1)
            {
                failure = "AMBIGUOUS_SUBJECT_IDENTITY";
                return default;
            }

            failure = "DUPLICATE_SUBJECT";
            return default;
        }

        failure = null;
        return NodeKey(matches[0]);
    }

    private static bool IsDefinedEnum<TEnum>(TEnum value) where TEnum : struct, Enum
        => Enum.IsDefined(typeof(TEnum), value);

    private static bool ValidateLifecycleOrder(DependencyLifecycleOrderDeclaration lifecycleOrder, out string? failure)
    {
        failure = null;
        if (!IsDefinedEnum(lifecycleOrder.Startup) ||
            !IsDefinedEnum(lifecycleOrder.Shutdown) ||
            !IsDefinedEnum(lifecycleOrder.Update) ||
            !IsDefinedEnum(lifecycleOrder.Recovery) ||
            !IsDefinedEnum(lifecycleOrder.Removal))
        {
            failure = "INVALID_LIFECYCLE_ORDER";
            return false;
        }

        return true;
    }

    private static DependencyNodeKey ResolveNodeKey(IReadOnlyCollection<DependencySubjectEvidence> subjects, DependencySubjectKind kind, string identity, string version, out string? failure)
    {
        var matches = subjects
            .Where(subject => subject.SubjectKind == kind &&
                              string.Equals(subject.SubjectKey.Identity, identity, StringComparison.Ordinal) &&
                              string.Equals(subject.SubjectKey.Version, version, StringComparison.Ordinal))
            .ToArray();

        if (matches.Length == 1)
        {
            failure = null;
            return NodeKey(matches[0]);
        }

        if (matches.Length == 0)
        {
            failure = "UNKNOWN_CONSUMER";
            return default;
        }

        failure = "DUPLICATE_SUBJECT";
        return default;
    }

    private static bool ContainsDelegationScope(string scope)
    {
        var operations = scope
            .Split(new[] { ';', ',', '|', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(value => value.Trim())
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .ToArray();

        return operations.Length == 2 &&
               operations.Contains("dependency graph validation", StringComparer.Ordinal) &&
               operations.Contains("activation-order validation", StringComparer.Ordinal);
    }

    private static bool IsHexDigest(string value)
        => value.Length == 64 && value.All(c => (c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f'));

    private static bool HasDuplicateApplicationDependencyMappings(
        IEnumerable<(string Identity, string[] Versions, string? Purpose)> declaredDependencies,
        IEnumerable<(string Identity, string[] Versions, string? Purpose)> requiredServices)
    {
        var declaredKeys = declaredDependencies.Select(value => $"{value.Identity}|{JoinVersions(value.Versions)}").ToArray();
        var serviceKeys = requiredServices.Select(value => $"{value.Identity}|{JoinVersions(value.Versions)}").ToArray();

        if (declaredKeys.Length != declaredKeys.Distinct(StringComparer.Ordinal).Count())
        {
            return true;
        }

        if (serviceKeys.Length != serviceKeys.Distinct(StringComparer.Ordinal).Count())
        {
            return true;
        }

        return declaredKeys.Intersect(serviceKeys, StringComparer.Ordinal).Any();
    }

    private static FilEvent BuildEvent(string eventType, string graphId, DateTimeOffset occurrenceTime, string sourceEvidence, string correlation, string? causation, string payload)
        => new(
            $"{graphId}:{eventType}",
            eventType,
            "1.0",
            "Foundation.DependencyGovernance",
            graphId,
            occurrenceTime,
            occurrenceTime.AddSeconds(1),
            sourceEvidence,
            correlation,
            causation,
            false,
            null,
            payload);

    private static DependencyValidationResult Fail(string reasonCode, string? cycleEvidence = null)
        => new()
        {
            Success = false,
            ReasonCode = reasonCode,
            GraphDecision = reasonCode,
            ActivationOrderDecision = reasonCode,
            CycleEvidence = cycleEvidence
        };
}
