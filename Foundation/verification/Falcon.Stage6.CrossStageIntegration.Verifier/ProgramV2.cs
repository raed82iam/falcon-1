using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;
using Foundation.Authority;
using Foundation.Contracts;
using Foundation.Contracts.ResourceGovernance;
using Foundation.DependencyGovernance;
using Foundation.Enabling;
using Foundation.State.ResourceGovernance;

namespace Falcon.Stage6.CrossStageIntegration.Verifier;

internal static class ProgramV2
{
    private static int _passed;
    private static int _failed;
    private static string _integratedIdentity = string.Empty;

    private static readonly DateTimeOffset T0 = new(2026, 8, 11, 10, 0, 0, TimeSpan.Zero);
    private static readonly ResourceEpochId Epoch = new("epoch-cross-stage-001");
    private static readonly ResourceClassId Cpu = new("cpu");
    private static readonly ApplicationPrincipalId AppA = new("app-a");
    private static readonly ApplicationPrincipalId AppB = new("app-b");
    private static readonly ResourceGrantId GrantA = new("grant-a");
    private static readonly ResourceGrantId GrantB = new("grant-b");

    private static int Main()
    {
        Run("stage0a_stage6_exact_closure_and_authority_binding", Stage0AStage6ExactClosureAndAuthorityBinding);
        Run("stage0b_stage6_enabling_primitives_compatible", Stage0BStage6EnablingPrimitivesCompatible);
        Run("stage0b_stage6_noncanonical_identity_fails_closed", Stage0BStage6NoncanonicalIdentityFailsClosed);
        Run("stage0c_stage6_invalid_enabling_authority_fails_closed", Stage0CStage6InvalidEnablingAuthorityFailsClosed);
        Run("stage1_stage6_controlled_solution_boundary", Stage1Stage6ControlledSolutionBoundary);
        Run("stage1_stage6_ownership_boundary_mutation_detected", Stage1Stage6OwnershipBoundaryMutationDetected);
        Run("stage2_stage6_contract_evidence_binding", Stage2Stage6ContractEvidenceBinding);
        Run("stage2_stage6_schema_mismatch_fails_closed", Stage2Stage6SchemaMismatchFailsClosed);
        Run("stage3_stage6_dependency_governance_binding", Stage3Stage6DependencyGovernanceBinding);
        Run("stage3_stage6_missing_graph_version_fails_closed", Stage3Stage6MissingGraphVersionFailsClosed);
        Run("stage3_stage6_unavailable_dependency_fails_closed", Stage3Stage6UnavailableDependencyFailsClosed);
        Run("stage4_stage6_authority_allow_binding", Stage4Stage6AuthorityAllowBinding);
        Run("stage4_stage6_revoked_delegation_fails_closed", Stage4Stage6RevokedDelegationFailsClosed);
        Run("stage4_stage6_expired_resource_authority_fails_closed", Stage4Stage6ExpiredResourceAuthorityFailsClosed);
        Run("stage5_stage6_inbound_message_contract_valid", Stage5Stage6InboundMessageContractValid);
        Run("stage5_stage6_missing_message_authority_fails_closed", Stage5Stage6MissingMessageAuthorityFailsClosed);
        Run("stage5_stage6_outbound_signal_contract_valid", Stage5Stage6OutboundSignalContractValid);
        Run("stage5_stage6_replay_stale_basis_cannot_remutate_current_truth", Stage5Stage6ReplayStaleBasisCannotRemutateCurrentTruth);
        Run("stage6_zero_application_state_valid", Stage6ZeroApplicationStateValid);
        Run("stage6_cross_application_isolation_preserved", Stage6CrossApplicationIsolationPreserved);
        Run("stage6_protected_floor_reserve_violation_rejected", Stage6ProtectedFloorReserveViolationRejected);
        Run("representative_predecessor_executable_identities_bound", RepresentativePredecessorExecutableIdentitiesBound);
        Run("whole_chain_positive", WholeChainPositive);
        Run("whole_chain_identity_deterministic", WholeChainIdentityDeterministic);
        Run("whole_chain_upstream_mutation_sensitive", WholeChainUpstreamMutationSensitive);
        Run("no_application_or_future_stage_authority_surface", NoApplicationOrFutureStageAuthoritySurface);

        Console.WriteLine();
        Console.WriteLine($"STAGE 6 CROSS-STAGE INTEGRATION VERIFIER V2: {_passed}/{_passed + _failed} PASS");
        Console.WriteLine($"Failures: {_failed}");
        Console.WriteLine($"INTEGRATED CROSS-STAGE EVIDENCE SHA-256: {_integratedIdentity}");
        Console.WriteLine("Explicit Stage 0A/0B/0C/1/2/3/4/5 to Stage 6 bindings and fail-closed mutations were evaluated.");
        Console.WriteLine("Full historical executable regression remains a separate mandatory harness layer and is not replaced by this verifier.");
        Console.WriteLine("No Stage 6 closure, Stage 7 authority, Application authority, deployment authority, external-connectivity authority, or financial/trading authority is created.");
        return _failed == 0 ? 0 : 1;
    }

    private static void Stage0AStage6ExactClosureAndAuthorityBinding()
    {
        var root = FindRepositoryRoot();
        var closure = Path.Combine(root, "docs", "governance", "GOV-049_STAGE_0A_GOVERNED_PREPARATION_CLOSURE.md");
        var acceptance = Path.Combine(root, "docs", "canonical-records", "owner-decisions", "stage6", "Stage6-CrossStage-Integration-Validation-Plan-Acceptance-20260811", "OWNER-ACCEPTANCE-STAGE6-CROSS-STAGE-INTEGRATION-VALIDATION-PLAN-v0.2.md");
        Require(File.Exists(closure), "STAGE0A_GOV049_CLOSURE_MISSING");
        Require(File.ReadAllText(closure).Contains("Stage 0A is therefore closed as complete", StringComparison.Ordinal), "STAGE0A_GOV049_CLOSURE_MEANING_MISSING");
        Require(IsSha256(HashFile(closure)), "STAGE0A_GOV049_HASH_INVALID");
        Require(File.Exists(acceptance) && IsSha256(HashFile(acceptance)), "CURRENT_CROSS_STAGE_OWNER_AUTHORITY_RECORD_INVALID");

        var valid = BuildEnablingContext();
        Require(valid.IsValid, "CURRENT_GOVERNED_ENABLING_CONTEXT_INVALID");
        Require(!(valid with { AuthorityDecision = "GOV-999" }).IsValid, "UNKNOWN_AUTHORITY_BECAME_VALID");
    }

    private static void Stage0BStage6EnablingPrimitivesCompatible()
    {
        var root = FindRepositoryRoot();
        var closure = Path.Combine(root, "docs", "governance", "GOV-053_STAGE_0B_CLOSURE.md");
        Require(File.Exists(closure), "STAGE0B_GOV053_CLOSURE_MISSING");
        Require(IsSha256(HashFile(closure)), "STAGE0B_GOV053_HASH_INVALID");

        var context = BuildEnablingContext();
        var fixedTime = new FixedTimeProvider(T0);
        var randomness = new WindowsCryptographicRandomnessProvider();
        var time = new WindowsFoundationTimeProvider(fixedTime, context.RuntimeEpochId, fixedTime.GetUtcNow().AddMinutes(-5), 50_000);
        var identifiers = new FoundationIdentifierProvider(time, randomness);
        var observation = time.Observe(context);
        Require(observation.Disposition == FoundationDisposition.Succeeded, "STAGE0B_TIME_SUCCESSOR_REJECTED");
        Require(observation.Quality == ClockQuality.VerifiedLocalBuild, "STAGE0B_TIME_QUALITY_MISMATCH");
        var random = randomness.Produce(new("cross-stage-rnd-1", "crypto-key", 32, false, context));
        Require(random.Disposition == FoundationDisposition.Succeeded && random.Material is { Length: 32 }, "STAGE0B_RANDOMNESS_SUCCESSOR_REJECTED");
        var id = identifiers.Issue(new IdentifierRequest("id:cross-stage:1", "falcon.foundation.operation", "subject:stage6-cross-stage", "internal-foundation", context));
        Require(id.Disposition == FoundationDisposition.Succeeded && id.Identifier is { Length: > 0 }, "STAGE0B_IDENTIFIER_SUCCESSOR_REJECTED");
        Require(id.Identifier![14] == '7', "STAGE0B_IDENTIFIER_NOT_UUIDV7");
    }

    private static void Stage0BStage6NoncanonicalIdentityFailsClosed()
    {
        Require(Rejected(() => _ = new ApplicationPrincipalId("app a")), "NONCANONICAL_APPLICATION_IDENTITY_ACCEPTED");
        Require(Rejected(() => _ = new ResourceClassId("cpu core")), "NONCANONICAL_RESOURCE_IDENTITY_ACCEPTED");
    }

    private static void Stage0CStage6InvalidEnablingAuthorityFailsClosed()
    {
        var valid = BuildEnablingContext();
        var invalid = valid with { AuthorityDecision = "GOV-999" };
        var fixedTime = new FixedTimeProvider(T0);
        var randomness = new WindowsCryptographicRandomnessProvider();
        var time = new WindowsFoundationTimeProvider(fixedTime, valid.RuntimeEpochId, fixedTime.GetUtcNow().AddMinutes(-5), 50_000);
        var identifiers = new FoundationIdentifierProvider(time, randomness);
        Require(!invalid.IsValid, "INVALID_ENABLING_AUTHORITY_CONTEXT_BECAME_VALID");
        Require(randomness.Produce(new("rnd-invalid", "crypto-key", 32, false, invalid)).Disposition == FoundationDisposition.Rejected, "INVALID_ENABLING_AUTHORITY_PRODUCED_RANDOMNESS");
        Require(identifiers.Issue(new IdentifierRequest("id:invalid", "falcon.foundation.operation", "subject:invalid", "internal-foundation", invalid)).Disposition == FoundationDisposition.Rejected, "INVALID_ENABLING_AUTHORITY_PRODUCED_IDENTIFIER");
    }

    private static void Stage1Stage6ControlledSolutionBoundary()
    {
        var paths = ControlledSolutionPaths();
        Require(ControlledBoundaryValid(paths), "CURRENT_CONTROLLED_SOLUTION_BOUNDARY_INVALID");
        const string crossStage = "verification/Falcon.Stage6.CrossStageIntegration.Verifier/Falcon.Stage6.CrossStageIntegration.Verifier.csproj";
        Require(paths.Count(path => StringComparer.Ordinal.Equals(path, crossStage)) == 1, "CROSS_STAGE_VERIFIER_NOT_IN_CONTROLLED_SOLUTION_EXACTLY_ONCE");
        Require(paths.Contains("src/Foundation.Contracts/Foundation.Contracts.csproj", StringComparer.Ordinal), "FOUNDATION_CONTRACTS_MISSING_FROM_CONTROLLED_SOLUTION");
        Require(paths.Contains("src/Foundation.State/Foundation.State.csproj", StringComparer.Ordinal), "FOUNDATION_STATE_MISSING_FROM_CONTROLLED_SOLUTION");
        Require(paths.Contains("src/Foundation.Authority/Foundation.Authority.csproj", StringComparer.Ordinal), "FOUNDATION_AUTHORITY_MISSING_FROM_CONTROLLED_SOLUTION");
    }

    private static void Stage1Stage6OwnershipBoundaryMutationDetected()
    {
        var mutated = ControlledSolutionPaths().Concat(new[] { "applications/forbidden/Fake.csproj" }).ToArray();
        Require(!ControlledBoundaryValid(mutated), "SYNTHETIC_APPLICATION_PROJECT_LEAKAGE_NOT_DETECTED");
        var referenceMutation = ControlledSolutionPaths().Concat(new[] { "reference/forbidden/Fake.csproj" }).ToArray();
        Require(!ControlledBoundaryValid(referenceMutation), "SYNTHETIC_REFERENCE_PROJECT_LEAKAGE_NOT_DETECTED");
    }

    private static void Stage2Stage6ContractEvidenceBinding()
    {
        Require(!string.IsNullOrWhiteSpace(ContractVersions.Con010), "STAGE2_CON010_VERSION_MISSING");
        var envelope = CreateInboundEnvelope("authority:stage2-contract-proof", "evidence:stage2-contract-proof", "{\"resource_request\":\"stage6-proof\"}");
        Require(CanonicalMessagingValidator.Validate(envelope).IsValid, "STAGE2_CANONICAL_CONTRACT_REJECTED");
        Require(IsSha256(CanonicalMessagingDigest.ComputeEnvelopeSha256(envelope)), "STAGE2_CANONICAL_DIGEST_INVALID");
    }

    private static void Stage2Stage6SchemaMismatchFailsClosed()
    {
        Require(EnvelopeRejected(schemaVersion: string.Empty), "EMPTY_SCHEMA_VERSION_ACCEPTED");
    }

    private static void Stage3Stage6DependencyGovernanceBinding()
    {
        var result = ValidateDependencyGraph(BuildDependencyRequest());
        Require(result.Success, "STAGE3_DEPENDENCY_GRAPH_REJECTED:" + result.ReasonCode);
        Require(StringComparer.Ordinal.Equals(result.GraphDecision, "DEPENDENCY_GRAPH_VALIDATED"), "STAGE3_GRAPH_DECISION_INVALID");
        Require(StringComparer.Ordinal.Equals(result.ActivationOrderDecision, "ACTIVATION_ORDER_VALIDATED"), "STAGE3_ACTIVATION_ORDER_INVALID");
        Require(IsSha256(result.GraphDigest), "STAGE3_GRAPH_DIGEST_INVALID");
        Require(!string.IsNullOrWhiteSpace(result.DecisionIdentity), "STAGE3_DECISION_IDENTITY_MISSING");
    }

    private static void Stage3Stage6MissingGraphVersionFailsClosed()
    {
        var invalid = BuildDependencyRequest() with { GraphVersion = string.Empty };
        var result = ValidateDependencyGraph(invalid);
        Require(!result.Success && StringComparer.Ordinal.Equals(result.ReasonCode, "MISSING_GRAPH_VERSION"), "MISSING_GRAPH_VERSION_DID_NOT_FAIL_CLOSED:" + result.ReasonCode);
    }

    private static void Stage3Stage6UnavailableDependencyFailsClosed()
    {
        var valid = BuildDependencyRequest();
        var subject = (ExternalDependencySubjectEvidence)valid.Subjects.Single();
        var unavailable = subject with { AvailabilityResult = "UNAVAILABLE" };
        var mutated = valid with { Subjects = new DependencySubjectEvidence[] { unavailable } };
        mutated = mutated with { ManifestSurface = mutated.ManifestSurface with { CanonicalDigest = ComputeDependencyGraphDigest(mutated) } };
        var result = ValidateDependencyGraph(mutated);
        Require(!result.Success, "UNAVAILABLE_STAGE3_DEPENDENCY_ACCEPTED");
    }

    private static void Stage4Stage6AuthorityAllowBinding()
    {
        var result = EvaluateAuthority(false);
        Require(StringComparer.Ordinal.Equals(result.Decision, AuthorityDecision.Allow), "STAGE4_AUTHORITY_DID_NOT_ALLOW_VALID_CONTEXT");
        Require(StringComparer.Ordinal.Equals(result.Reason, AuthorityReason.Allowed), "STAGE4_AUTHORITY_ALLOW_REASON_INVALID");
        var transition = ApplyFoundationReduction("authority-" + StableToken(result.DecisionId), "stage4-allow", "evidence-stage4-" + StableToken(result.DecisionId));
        Require(transition.Accepted.AcceptedSnapshot.GetRequiredAllocation(AppA, Cpu).Allocation.Amount == 10m, "STAGE4_ALLOWED_AUTHORITY_NOT_BOUND_TO_STAGE6_MUTATION");
    }

    private static void Stage4Stage6RevokedDelegationFailsClosed()
    {
        var result = EvaluateAuthority(true);
        Require(StringComparer.Ordinal.Equals(result.Decision, AuthorityDecision.Deny), "REVOKED_STAGE4_DELEGATION_ALLOWED");
        Require(StringComparer.Ordinal.Equals(result.Reason, AuthorityReason.DelegationRevoked), "REVOKED_STAGE4_DELEGATION_WRONG_REASON");
    }

    private static void Stage4Stage6ExpiredResourceAuthorityFailsClosed()
    {
        var before = Allocations();
        var expired = new FoundationResourceMutationAuthority("authority-expired", new ResourceScopeId("scope-mutation"), new[] { AppA, AppB }, new[] { Cpu }, new[] { ResourceDecisionKind.Reduce }, 1, Evidence("expired-authority"), T0.AddHours(-2), T0.AddHours(-1));
        var intent = BuildReductionIntent(before, expired, "expired", "evidence-expired");
        var batch = new ResourceEffectBatch("batch-reduce-expired", new[] { ResourceEffectOperation.ForFoundation(intent) });
        Require(Rejected(() => new ResourceMutationProcessor().ApplyFoundationAllocationMutations(before, batch.BatchId, new[] { intent }, new SuccessAdapter(), T0, Effective(before))), "EXPIRED_STAGE6_RESOURCE_AUTHORITY_WAS_APPLIED");
    }

    private static void Stage5Stage6InboundMessageContractValid()
    {
        var auth = EvaluateAuthority(false);
        var envelope = CreateInboundEnvelope("authority:" + StableToken(auth.DecisionId), "evidence:stage5-inbound", "{\"application\":\"app-a\",\"resource\":\"cpu\",\"requested\":5}");
        Require(CanonicalMessagingValidator.Validate(envelope).IsValid, "STAGE5_INBOUND_RESOURCE_MESSAGE_INVALID");
        Require(IsSha256(CanonicalMessagingDigest.ComputeEnvelopeSha256(envelope)), "STAGE5_INBOUND_MESSAGE_DIGEST_INVALID");
    }

    private static void Stage5Stage6MissingMessageAuthorityFailsClosed()
    {
        Require(EnvelopeRejected(authority: string.Empty), "MISSING_STAGE5_MESSAGE_AUTHORITY_ACCEPTED");
    }

    private static void Stage5Stage6OutboundSignalContractValid()
    {
        var auth = EvaluateAuthority(false);
        var transition = ApplyFoundationReduction("authority-" + StableToken(auth.DecisionId), "stage5-outbound", "evidence-stage5-outbound");
        var signal = CreateComplianceSignal(transition);
        var envelope = CreateOutboundEnvelope(signal, "authority:" + StableToken(auth.DecisionId), transition.Accepted.AcceptedSnapshot.IdentitySha256);
        Require(CanonicalMessagingValidator.Validate(envelope).IsValid, "STAGE5_OUTBOUND_STAGE6_SIGNAL_MESSAGE_INVALID");
        Require(IsSha256(CanonicalMessagingDigest.ComputeEnvelopeSha256(envelope)), "STAGE5_OUTBOUND_STAGE6_SIGNAL_DIGEST_INVALID");
    }

    private static void Stage5Stage6ReplayStaleBasisCannotRemutateCurrentTruth()
    {
        var auth = EvaluateAuthority(false);
        var first = ApplyFoundationReduction("authority-" + StableToken(auth.DecisionId), "replay-base", "evidence-replay-base");
        var staleAuthority = new FoundationResourceMutationAuthority("authority-" + StableToken(auth.DecisionId), new ResourceScopeId("scope-mutation"), new[] { AppA, AppB }, new[] { Cpu }, new[] { ResourceDecisionKind.Reduce }, 1, Evidence("replay-authority"), T0.AddHours(-1), T0.AddHours(1));
        var staleIntent = BuildReductionIntent(first.Before, staleAuthority, "replay-stale", "evidence-replay-stale");
        var staleBatch = new ResourceEffectBatch("batch-reduce-replay-stale", new[] { ResourceEffectOperation.ForFoundation(staleIntent) });
        Require(Rejected(() => new ResourceMutationProcessor().ApplyFoundationAllocationMutations(first.Accepted.AcceptedSnapshot, staleBatch.BatchId, new[] { staleIntent }, new SuccessAdapter(), T0.AddMinutes(1), Effective(first.Accepted.AcceptedSnapshot))), "REPLAY_OR_STALE_STAGE5_BASIS_REMUTATED_CURRENT_STAGE6_TRUTH");
    }

    private static void Stage6ZeroApplicationStateValid()
        => Require(new ApplicationResourceStateProjectionSet(Epoch, T0, Array.Empty<ApplicationResourceStateProjection>()).Projections.Count == 0, "ZERO_APPLICATION_STAGE6_STATE_INVALID");

    private static void Stage6CrossApplicationIsolationPreserved()
    {
        var allocations = Allocations(20m, 30m);
        Require(allocations.GetRequiredAllocation(AppA, Cpu).Allocation.Amount == 20m, "APP_A_ALLOCATION_WRONG");
        Require(allocations.GetRequiredAllocation(AppB, Cpu).Allocation.Amount == 30m, "APP_B_ALLOCATION_WRONG");
        Require(Rejected(() => allocations.GetRequiredAllocation(new ApplicationPrincipalId("app-c"), Cpu)), "UNKNOWN_APPLICATION_CONSUMED_ANOTHER_APPLICATION_GRANT");
    }

    private static void Stage6ProtectedFloorReserveViolationRejected()
        => Require(Rejected(() => Allocations(45m, 45m)), "PROTECTED_FLOOR_OR_RESERVE_OVERCOMMIT_ACCEPTED");

    private static void RepresentativePredecessorExecutableIdentitiesBound()
    {
        var hashes = RepresentativeExecutableHashes();
        Require(hashes.Count == 9, "REPRESENTATIVE_EXECUTABLE_HASH_SET_INCOMPLETE");
        Require(hashes.Values.All(IsSha256), "REPRESENTATIVE_EXECUTABLE_HASH_INVALID");
        Require(hashes.Values.Distinct(StringComparer.Ordinal).Count() == hashes.Count, "REPRESENTATIVE_EXECUTABLE_HASH_COLLISION_OR_DUPLICATE_BINDING");
    }

    private static void WholeChainPositive()
    {
        _integratedIdentity = BuildWholeChainIdentity(false);
        Require(IsSha256(_integratedIdentity), "WHOLE_CHAIN_IDENTITY_INVALID");
    }

    private static void WholeChainIdentityDeterministic()
    {
        var first = BuildWholeChainIdentity(false);
        var second = BuildWholeChainIdentity(false);
        Require(StringComparer.Ordinal.Equals(first, second), "WHOLE_CHAIN_IDENTITY_NONDETERMINISTIC");
        _integratedIdentity = first;
    }

    private static void WholeChainUpstreamMutationSensitive()
    {
        var baseline = BuildWholeChainIdentity(false);
        var mutated = BuildWholeChainIdentity(true);
        Require(!StringComparer.Ordinal.Equals(baseline, mutated), "UPSTREAM_STAGE5_MUTATION_DID_NOT_CHANGE_WHOLE_CHAIN_IDENTITY");
        _integratedIdentity = baseline;
    }

    private static void NoApplicationOrFutureStageAuthoritySurface()
    {
        var types = new[] { typeof(FoundationResourceTruthSnapshot), typeof(ApplicationResourceAllocationSnapshot), typeof(ResourceMutationProcessor), typeof(ApplicationResourceStateProjection), typeof(ApplicationResourceLoadSheddingSignal), typeof(ResourceIntegrationCoherenceSet) };
        var names = types.SelectMany(type => new[] { type.FullName ?? type.Name }.Concat(type.GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly).Select(member => member.Name))).ToArray();
        var forbidden = new[] { "FSATS", "FSARM", "Trading", "Broker", "Strategy", "Stage7", "WP11", "Deploy", "FinancialAuthority" };
        Require(forbidden.All(term => names.All(name => !name.Contains(term, StringComparison.OrdinalIgnoreCase))), "APPLICATION_OR_FUTURE_STAGE_AUTHORITY_SURFACE_LEAKED");
    }

    private static string BuildWholeChainIdentity(bool mutateStage5)
    {
        var root = FindRepositoryRoot();
        var context = BuildEnablingContext();
        var fixedTime = new FixedTimeProvider(T0);
        var randomness = new WindowsCryptographicRandomnessProvider();
        var time = new WindowsFoundationTimeProvider(fixedTime, context.RuntimeEpochId, fixedTime.GetUtcNow().AddMinutes(-5), 50_000);
        var identifiers = new FoundationIdentifierProvider(time, randomness);
        var issued = identifiers.Issue(new IdentifierRequest("id:whole-chain", "falcon.foundation.operation", "subject:whole-chain", "internal-foundation", context));
        Require(issued.Disposition == FoundationDisposition.Succeeded, "WHOLE_CHAIN_STAGE0_IDENTIFIER_FAILED");

        var dependency = ValidateDependencyGraph(BuildDependencyRequest());
        Require(dependency.Success && IsSha256(dependency.GraphDigest), "WHOLE_CHAIN_STAGE3_DEPENDENCY_FAILED");
        var authority = EvaluateAuthority(false);
        Require(StringComparer.Ordinal.Equals(authority.Decision, AuthorityDecision.Allow), "WHOLE_CHAIN_STAGE4_AUTHORITY_FAILED");

        var inboundPayload = mutateStage5 ? "{\"application\":\"app-a\",\"resource\":\"cpu\",\"requested\":6}" : "{\"application\":\"app-a\",\"resource\":\"cpu\",\"requested\":5}";
        var inbound = CreateInboundEnvelope("authority:" + StableToken(authority.DecisionId), "evidence:stage3:" + dependency.GraphDigest[..16].ToLowerInvariant(), inboundPayload);
        Require(CanonicalMessagingValidator.Validate(inbound).IsValid, "WHOLE_CHAIN_STAGE5_INBOUND_FAILED");
        var inboundDigest = CanonicalMessagingDigest.ComputeEnvelopeSha256(inbound);

        var transition = ApplyFoundationReduction("authority-" + StableToken(authority.DecisionId), "whole-chain-" + inboundDigest[..12].ToLowerInvariant(), "evidence-" + inboundDigest[..16].ToLowerInvariant());
        var signal = CreateComplianceSignal(transition);
        Require(signal.SignalClass == TechnicalLoadSheddingSignalClass.ComplianceReductionRequired, "WHOLE_CHAIN_STAGE6_SIGNAL_NOT_COMPLIANCE_REQUIRED");
        var outbound = CreateOutboundEnvelope(signal, "authority:" + StableToken(authority.DecisionId), transition.Accepted.AcceptedSnapshot.IdentitySha256);
        Require(CanonicalMessagingValidator.Validate(outbound).IsValid, "WHOLE_CHAIN_STAGE5_OUTBOUND_FAILED");
        var outboundDigest = CanonicalMessagingDigest.ComputeEnvelopeSha256(outbound);

        var hashes = RepresentativeExecutableHashes();
        var canonical = string.Join("\n", new[]
        {
            "stage0a=" + HashFile(Path.Combine(root, "docs", "governance", "GOV-049_STAGE_0A_GOVERNED_PREPARATION_CLOSURE.md")),
            "stage0b=" + HashFile(Path.Combine(root, "docs", "governance", "GOV-053_STAGE_0B_CLOSURE.md")),
            "stage0b_subject=" + identifiers.SubjectId,
            "stage0c_epoch=" + context.RuntimeEpochId,
            "stage1_solution=" + HashFile(Path.Combine(root, "Falcon.Foundation.ControlledProjectFoundation.slnx")),
            "stage2_contract=" + ContractVersions.Con010,
            "stage3_graph=" + dependency.GraphDigest,
            "stage3_decision=" + dependency.DecisionIdentity,
            "stage4_decision=" + authority.DecisionId,
            "stage5_inbound=" + inboundDigest,
            "stage6_snapshot=" + transition.Accepted.AcceptedSnapshot.IdentitySha256,
            "stage5_outbound=" + outboundDigest,
            "representative_executables=" + string.Join(";", hashes.OrderBy(pair => pair.Key, StringComparer.Ordinal).Select(pair => pair.Key + "=" + pair.Value))
        });
        return Sha256(canonical);
    }

    private static FoundationAuthorityContext BuildEnablingContext()
        => new(FoundationBoundary.Authority, FoundationBoundary.Environment, FoundationBoundary.DeploymentProfile, "epoch:stage6-cross-stage:1", "stage6-cross-stage-verifier", "RVES-STG6-CROSS-STAGE-001", true);

    private static string[] ControlledSolutionPaths()
    {
        var solution = XDocument.Load(Path.Combine(FindRepositoryRoot(), "Falcon.Foundation.ControlledProjectFoundation.slnx"));
        return solution.Root?.Elements("Project").Select(project => (project.Attribute("Path")?.Value ?? string.Empty).Replace('\\', '/')).ToArray() ?? Array.Empty<string>();
    }

    private static bool ControlledBoundaryValid(IEnumerable<string> paths)
        => paths.All(path => !path.StartsWith("applications/", StringComparison.OrdinalIgnoreCase) && !path.StartsWith("reference/", StringComparison.OrdinalIgnoreCase));

    private static DependencyGraphRequest BuildDependencyRequest()
    {
        const string graphId = "stage6-cross-stage-dependency-graph";
        var subject = new ExternalDependencySubjectEvidence
        {
            SubjectKind = DependencySubjectKind.External,
            SubjectKey = new DependencySubjectKey("foundation-resource-governance", "1.0"),
            EvidenceReference = "evidence/cross-stage/resource-governance",
            Owner = "Falcon Foundation",
            Source = "accepted-stage6-resource-governance",
            IntegrityDigest = Sha256("accepted-stage6-resource-governance"),
            AvailabilityResult = "AVAILABLE",
            ContainmentEvidence = "foundation-contained",
            EffectiveTime = T0.AddHours(-1),
            Expiry = T0.AddHours(1)
        };
        var request = new DependencyGraphRequest
        {
            GraphId = graphId,
            GraphVersion = "1.0",
            RequesterIdentity = "stage6-cross-stage-verifier",
            AuthoritySource = "OWNER-STAGE6-CROSS-STAGE-v0.2",
            ObservationTime = T0,
            ManifestSurface = new ManifestSurfaceRecord("graph-manifest-stage6-cross-stage", ContractVersions.Con010, "CANDIDATE_MANIFEST", graphId, "graph-evidence-stage6-cross-stage", "SEPARATE", "INTACT", "OWNER-STAGE6-CROSS-STAGE-v0.2", "cross-stage-validation", string.Empty, T0.AddMinutes(-10), T0.AddHours(2)),
            DelegationEvidence = new DelegationRecord("delegation-stage6-cross-stage", "1.0", "Project Owner", "stage6-cross-stage-verifier", "dependency graph validation;activation-order validation", "delegation-chain-stage6-cross-stage", "OWNER-STAGE6-CROSS-STAGE-v0.2", "cross-stage-validation", "GRANTED", "revoked-by-owner", T0.AddMinutes(-30), T0.AddHours(2)),
            Subjects = new DependencySubjectEvidence[] { subject },
            Dependencies = Array.Empty<Foundation.DependencyGovernance.DependencyDeclaration>(),
            ProposedActivationOrder = new[] { subject.SubjectKey }
        };
        return request with { ManifestSurface = request.ManifestSurface with { CanonicalDigest = ComputeDependencyGraphDigest(request) } };
    }

    private static DependencyValidationResult ValidateDependencyGraph(DependencyGraphRequest request)
        => new DependencyGovernanceValidator().Validate(request);

    private static string ComputeDependencyGraphDigest(DependencyGraphRequest request)
    {
        var method = typeof(DependencyGovernanceValidator).GetMethod("SerializeCandidateGraphRequest", BindingFlags.NonPublic | BindingFlags.Static) ?? throw new InvalidOperationException("DEPENDENCY_GRAPH_SERIALIZER_NOT_FOUND");
        var serialized = (string?)method.Invoke(null, new object[] { request }) ?? throw new InvalidOperationException("DEPENDENCY_GRAPH_SERIALIZER_EMPTY");
        return Sha256(serialized);
    }

    private static AuthorityResult EvaluateAuthority(bool revoked)
    {
        var engine = new DefaultDenyAuthorityEngine();
        return engine.Evaluate(
            new AuthorityRequest("request/cross-stage/001", "actor/foundation-resource-controller", "resource.mutate", "foundation.resource-governance", "governed-resource-mutation", "foundation:resource:mutate", "foundation-control-plane", "foundation-internal", "FIT", "correlation/cross-stage/001", T0.AddMinutes(-1), T0.AddMinutes(30)),
            new AuthorityEvaluationContext(
                new AuthorityPolicy("policy/foundation-resource-governance", "1.0.0", "authority/owner-approved", T0.AddDays(-1), T0.AddDays(1), new[] { "actor/foundation-resource-controller" }, new[] { "resource.mutate" }, new[] { "foundation.resource-governance" }, new[] { "governed-resource-mutation" }, new[] { "foundation:resource" }, new[] { "foundation-internal" }, false),
                new DelegationEvidence("delegation/cross-stage/001", "actor/foundation-resource-controller", "authority/owner-approved", new[] { "foundation:resource" }, T0.AddDays(-1), T0.AddHours(1), revoked),
                new FitnessEvidence("actor/foundation-resource-controller", "FIT", true, T0.AddMinutes(-5), T0.AddMinutes(20), "evidence/fitness/cross-stage/001"),
                T0,
                "evidence/authority/cross-stage/001"));
    }

    private static CanonicalFilEnvelope CreateInboundEnvelope(string authority, string provenance, string payload, string schemaVersion = "1.0")
        => CanonicalFilEnvelope.Create(new MessageIdentity("msg:00001001"), FilMessageKind.Command, FilMessageClassification.Operational, "falcon.foundation.resource.request.v1", new SchemaIdentity("schema:falcon.foundation.resource.request"), schemaVersion, new ProducerIdentityReference("application.app-a/resource-requester"), new RecipientScopeReference("foundation.resource-governance/request"), new CorrelationIdentity("correlation:00001001"), new CausationIdentity("causation:00001000"), new AuthorityReference(authority), new ProvenanceReference(provenance), new IdempotencyIdentity("idempotency:00001001"), new DeliveryAttemptIdentity("attempt:00001001"), new RetryLineageIdentity("retry-lineage:00001001"), new CanonicalMessageTime(T0.AddMinutes(-2), T0.AddMinutes(30)), CanonicalOutcome.Unknown("processing_not_yet_attempted"), payload);

    private static CanonicalFilEnvelope CreateOutboundEnvelope(ApplicationResourceLoadSheddingSignal signal, string authority, string snapshotIdentity)
        => CanonicalFilEnvelope.Create(new MessageIdentity("msg:00001002"), FilMessageKind.Event, FilMessageClassification.Operational, "falcon.foundation.resource.state-signal.v1", new SchemaIdentity("schema:falcon.foundation.resource.state-signal"), "1.0", new ProducerIdentityReference("foundation.resource-governance/state-signal"), new RecipientScopeReference("application.app-a/resource-state"), new CorrelationIdentity("correlation:00001001"), new CausationIdentity("causation:00001001"), new AuthorityReference(authority), new ProvenanceReference("evidence:stage6:" + snapshotIdentity[..16].ToLowerInvariant()), new IdempotencyIdentity("idempotency:00001002"), new DeliveryAttemptIdentity("attempt:00001002"), new RetryLineageIdentity("retry-lineage:00001002"), new CanonicalMessageTime(T0, T0.AddMinutes(30)), CanonicalOutcome.Unknown("publication_only"), "{\"signal_class\":\"" + signal.SignalClass + "\",\"compliant_target\":" + (signal.CompliantCapacityTarget?.Amount.ToString(CultureInfo.InvariantCulture) ?? "null") + "}");

    private static bool EnvelopeRejected(string schemaVersion = "1.0", string authority = "authority:valid")
    {
        try { return !CanonicalMessagingValidator.Validate(CreateInboundEnvelope(authority, "evidence:invalid-test", "{}", schemaVersion)).IsValid; }
        catch { return true; }
    }

    private static ResourceQuantity Q(decimal amount) => new(amount, "units");
    private static ResourceEffectiveLifetime Lifetime() => new(T0.AddHours(-2), null, true);
    private static ResourceEvidenceReference Evidence(string id, DateTimeOffset? at = null) => new(new ResourceEvidenceId(id), new ResourceScopeId("scope-" + id), at ?? T0.AddMinutes(-40), Epoch);
    private static FoundationResourceTruthSnapshot Truth() => new(Epoch, T0.AddMinutes(-30), new[] { new FoundationResourceClassTruth(Cpu, Q(100), Q(10), Q(10), Evidence("truth", T0.AddMinutes(-31))) }, true);

    private static ApplicationResourceAllocationSnapshot Allocations(decimal appA = 20m, decimal appB = 20m)
        => new(Truth(), T0.AddMinutes(-20), new[]
        {
            new ApplicationResourceAllocation(GrantA, AppA, Cpu, Q(appA), Q(Math.Max(appA, 30m)), Q(Math.Max(appA, 40m)), Lifetime(), Evidence("allocation-a")),
            new ApplicationResourceAllocation(GrantB, AppB, Cpu, Q(appB), Q(Math.Max(appB, 30m)), Q(Math.Max(appB, 40m)), Lifetime(), Evidence("allocation-b"))
        }, true);

    private static ResourceCoordinationEnvelope Envelope(ApplicationResourceAllocationSnapshot allocation)
        => new("envelope-authority", new ResourceScopeId("scope-coordination"), "coordinator-1", "aggregate-resource-coordinator", 1, 1, "fence-1", allocation,
            new[]
            {
                new ResourceCoordinationEnvelopeMember(AppA, GrantA, Cpu, Q(10), Q(10), Q(20), new ResourcePreemptionEligibilityBinding(GrantA, AppA, Cpu, ResourceReclaimability.Reclaimable, Lifetime(), Evidence("binding-a"))),
                new ResourceCoordinationEnvelopeMember(AppB, GrantB, Cpu, Q(10), Q(10), Q(20), new ResourcePreemptionEligibilityBinding(GrantB, AppB, Cpu, ResourceReclaimability.Reclaimable, Lifetime(), Evidence("binding-b")))
            }, Evidence("envelope"), T0.AddMinutes(-18), T0.AddHours(1));

    private static EffectiveResourceDistributionSnapshot Effective(ApplicationResourceAllocationSnapshot allocation)
        => new(allocation, Envelope(allocation), T0.AddMinutes(-5), Array.Empty<BorrowedEffectiveCapacitySegment>());

    private static FoundationAllocationMutationIntent BuildReductionIntent(ApplicationResourceAllocationSnapshot before, FoundationResourceMutationAuthority authority, string suffix, string evidenceId)
        => new("reduce-a-" + suffix, ResourceDecisionKind.Reduce, AppA, GrantA, Cpu, Q(10), Q(20), Q(30), authority, null, before.IdentitySha256, new CorrelationId("corr-" + suffix), new CausationId("cause-" + suffix), Evidence(evidenceId), T0.AddMinutes(-4), T0.AddMinutes(30));

    private static (ApplicationResourceAllocationSnapshot Before, AcceptedFoundationAllocationMutation Accepted, ResourceEffectBatch Batch) ApplyFoundationReduction(string authorityId, string suffix, string evidenceId)
    {
        var before = Allocations();
        var authority = new FoundationResourceMutationAuthority(authorityId, new ResourceScopeId("scope-mutation"), new[] { AppA, AppB }, new[] { Cpu }, new[] { ResourceDecisionKind.Reduce, ResourceDecisionKind.Revoke, ResourceDecisionKind.Restore }, 1, Evidence(evidenceId), T0.AddHours(-1), T0.AddHours(1));
        var intent = BuildReductionIntent(before, authority, suffix, "reduce-intent-" + suffix);
        var batch = new ResourceEffectBatch("batch-reduce-" + suffix, new[] { ResourceEffectOperation.ForFoundation(intent) });
        var accepted = new ResourceMutationProcessor().ApplyFoundationAllocationMutations(before, batch.BatchId, new[] { intent }, new SuccessAdapter(), T0, Effective(before));
        return (before, accepted, batch);
    }

    private static ApplicationResourceLoadSheddingSignal CreateComplianceSignal((ApplicationResourceAllocationSnapshot Before, AcceptedFoundationAllocationMutation Accepted, ResourceEffectBatch Batch) transition)
    {
        var basis = AcceptedResourceCapacityTransitionBasis.FromFoundationMutation(transition.Before, transition.Accepted, transition.Batch, AppA, Cpu);
        var projection = ApplicationResourceStateProjectionBuilder.CreateDirect(transition.Accepted.AcceptedSnapshot, AppA, Cpu, basis.AcceptedAt, null, null, null, basis, null);
        return ApplicationResourceLoadSheddingSignalFactory.Create(projection, basis.AcceptedAt);
    }

    private static IReadOnlyDictionary<string, string> RepresentativeExecutableHashes()
    {
        var root = FindRepositoryRoot();
        var paths = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["stage0b"] = "verification/Falcon.Stage0B.Verifier/bin/Release/net10.0/Falcon.Stage0B.Verifier.dll",
            ["stage0c"] = "verification/Falcon.Stage0C.Verifier/bin/Release/net10.0/Falcon.Stage0C.Verifier.dll",
            ["stage2"] = "verification/Falcon.Stage2.WP04.Verifier/bin/Release/net10.0/Falcon.Stage2.WP04.Verifier.dll",
            ["stage3"] = "verification/Falcon.Stage3.WP04.Verifier/bin/Release/net10.0/Falcon.Stage3.WP04.Verifier.dll",
            ["stage4"] = "verification/Falcon.Stage4.WP01.Verifier/bin/Release/net10.0/Falcon.Stage4.WP01.Verifier.dll",
            ["stage5-messaging"] = "verification/Falcon.Stage5.WP01.Verifier/bin/Release/net10.0/Falcon.Stage5.WP01.Verifier.dll",
            ["stage5-delivery"] = "verification/Falcon.Stage5.WP06.Verifier/bin/Release/net10.0/Falcon.Stage5.WP06.Verifier.dll",
            ["stage5-events"] = "verification/Falcon.Stage5.WP07.Verifier/bin/Release/net10.0/Falcon.Stage5.WP07.Verifier.dll",
            ["stage6"] = "verification/Falcon.Stage6.WP10.Verifier/bin/Release/net10.0/Falcon.Stage6.WP10.Verifier.dll"
        };
        return paths.ToDictionary(pair => pair.Key, pair => HashFile(Path.Combine(root, pair.Value.Replace('/', Path.DirectorySeparatorChar))), StringComparer.Ordinal);
    }

    private static string FindRepositoryRoot()
    {
        var current = new DirectoryInfo(Directory.GetCurrentDirectory());
        while (current is not null)
        {
            if (File.Exists(Path.Combine(current.FullName, "Falcon.Foundation.ControlledProjectFoundation.slnx"))) return current.FullName;
            current = current.Parent;
        }
        throw new InvalidOperationException("REPOSITORY_ROOT_NOT_FOUND");
    }

    private static string HashFile(string path)
    {
        if (!File.Exists(path)) throw new InvalidOperationException("MISSING_FILE_FOR_HASH:" + path);
        return Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path)));
    }

    private static string Sha256(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value ?? string.Empty)));
    private static bool IsSha256(string? value) => value is { Length: 64 } && value.All(ch => char.IsDigit(ch) || ch is >= 'A' and <= 'F');
    private static string StableToken(string value) => Sha256(value)[..16].ToLowerInvariant();

    private static bool Rejected(Action action)
    {
        try { action(); return false; }
        catch { return true; }
    }

    private sealed class SuccessAdapter : IResourceEffectAdapter
    {
        public ResourceEffectApplicationResult Apply(ResourceEffectBatch batch, DateTimeOffset appliedAt)
            => new(batch.IdentitySha256, true, false, batch.Operations.Select(operation => operation.OperationId), Evidence("effect", appliedAt), appliedAt);
    }

    private static void Run(string name, Action action)
    {
        try { action(); _passed++; Console.WriteLine("PASS " + name); }
        catch (Exception exception) { _failed++; Console.WriteLine("FAIL " + name + ": " + exception.GetType().Name + ": " + exception.Message); }
    }

    private static void Require(bool condition, string reason)
    {
        if (!condition) throw new InvalidOperationException(reason);
    }
}