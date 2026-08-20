using System;
using Foundation.Contracts;
using Foundation.Reconciliation;

internal static class Program
{
    private static readonly DateTimeOffset T0 = new(2026, 8, 15, 19, 0, 0, TimeSpan.Zero);

    private static int Main()
    {
        try
        {
            VerifyValidRecoveryComplete();
            VerifyDeterministicIdentity();
            VerifyMutationSensitivity();
            VerifyReleasePartialDenied();
            VerifyReleaseUncertainRemainsUncertain();
            VerifyReleaseIdentityMismatch();
            VerifyRecoveryCaseMismatch();
            VerifySubjectMismatch();
            VerifyIdentityRecheckUntrusted();
            VerifyConfigurationRecheckUntrusted();
            VerifyDependencyRecheckUntrusted();
            VerifySecurityRecheckUntrusted();
            VerifyLifecycleRequestInvalid();
            VerifyLifecycleSourceInvalid();
            VerifyLifecycleTargetInvalid();
            VerifyLifecycleAuthorityBindingMismatch();
            VerifyLifecycleDependencyBindingMismatch();
            VerifyLifecycleRequestBeforeRelease();
            VerifyLifecycleResultRequestMismatch();
            VerifyLifecycleResultRejected();
            VerifyLifecycleResultActualStateMismatch();
            VerifyLifecycleCompletionBeforeRelease();
            VerifyNewAuthorityRequestInvalid();
            VerifyNewAuthorityActionMismatch();
            VerifyNewAuthorityResourceMismatch();
            VerifyNewAuthorityPurposeMismatch();
            VerifyNewAuthorityScopeMismatch();
            VerifyNewAuthorityCorrelationMismatch();
            VerifyNewAuthorityRequestBeforeLifecycle();
            VerifyNewAuthorityResultInvalid();
            VerifyNewAuthorityResultRequestMismatch();
            VerifyOldAuthorityReuseDenied();
            VerifyNewAuthorityDeniedAfterLifecycle();
            VerifyNewAuthorityExpired();
            VerifyObservationRequiredModeMissing();
            VerifyObservationUntrusted();
            VerifyObservationFailed();
            VerifyObservationInProgressRestricted();
            VerifyObservationExitCompletes();
            VerifyDirectRunningWithoutValidatedReleaseDenied();
            VerifyClosureEvidencePreserved();
            VerifyNoFsaOrApplicationBusinessLeakage();

            Console.WriteLine("STAGE9_WP09_VERIFIER = PASS");
            Console.WriteLine("CHECKS = 42/42");
            Console.WriteLine("VALID_WP08_RELEASE_FACT_REQUIRED = YES");
            Console.WriteLine("SYS002 = LIFECYCLE_TRANSITION_OWNER");
            Console.WriteLine("AUT001 = NEW_AUTHORITY_DECISION_OWNER");
            Console.WriteLine("LIFECYCLE_TRANSITION != NEW_AUTHORITY_DECISION");
            Console.WriteLine("OLD_AUTHORITY_REUSE = DENIED");
            Console.WriteLine("RECOVERY_GUARD_OBSERVATION = GOVERNED");
            Console.WriteLine("OBSERVATION_BYPASS = DENIED");
            Console.WriteLine("RECOVERY_COMPLETE_REQUIRES_GOVERNED_EVIDENCE = YES");
            Console.WriteLine("STAGE13_FSA_CONTROLLED_REVIVAL_SURFACE = NONE");
            Console.WriteLine("APPLICATION_BUSINESS_RECOVERY = NOT_IMPLEMENTED");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("STAGE9_WP09_VERIFIER = FAIL");
            Console.Error.WriteLine(ex.Message);
            return 1;
        }
    }

    private static ProtectiveRestrictionReleaseFact Release(
        ProtectiveRestrictionReleaseClassification classification = ProtectiveRestrictionReleaseClassification.Released) =>
        new(
            classification,
            classification == ProtectiveRestrictionReleaseClassification.Released
                ? ProtectiveRestrictionReleaseReason.Pass
                : "test-release-reason",
            "case:001",
            "subject:001",
            "restriction:001",
            "restriction-integrity:001",
            "release-authorization:001",
            "readiness:001",
            "validation:001",
            "guardian-condition:001",
            "reconciliation:001",
            "security:001",
            "dependency:001",
            "risk:001",
            "risk-profile:001",
            "enforcement:001",
            T0.AddMinutes(1));

    private static RecoveryReintroductionTrustEvidence Rechecks(
        bool identityCurrent = true,
        bool identityTrusted = true,
        bool configCurrent = true,
        bool configTrusted = true,
        bool dependencyCurrent = true,
        bool dependencyTrusted = true,
        bool securityCurrent = true,
        bool securityTrusted = true) =>
        new(
            "identity-evidence:001",
            identityCurrent,
            identityTrusted,
            "configuration-evidence:001",
            configCurrent,
            configTrusted,
            "dependency-evidence:001",
            dependencyCurrent,
            dependencyTrusted,
            "security-evidence:001",
            securityCurrent,
            securityTrusted);

    private static RecoveryObservationEvidence Observation(
        RecoveryObservationMode mode = RecoveryObservationMode.Heightened,
        bool current = true,
        bool trusted = true,
        bool satisfactory = true,
        bool exitAuthorized = true) =>
        new(
            mode,
            "observation-evidence:001",
            current,
            trusted,
            satisfactory,
            exitAuthorized,
            exitAuthorized ? "observation-exit-evidence:001" : string.Empty,
            T0.AddMinutes(6));

    private static LifecycleTransitionRequest LifecycleRequest(
        ProtectiveRestrictionReleaseFact release,
        string source = "RECOVERING",
        string target = "READY") =>
        new(
            "lifecycle-request:001",
            release.SubjectIdentity,
            source,
            target,
            "recovery-coordinator:001",
            release.ReleaseAuthorizationIdentity,
            "controlled-reintroduction",
            "dependency-evidence:001",
            T0.AddMinutes(2),
            T0.AddMinutes(30));

    private static LifecycleTransitionResult LifecycleResult(
        LifecycleTransitionRequest request,
        string decision = "ACCEPTED") =>
        new(
            request.TransitionRequestId,
            "lifecycle-transition:001",
            decision,
            request.AuthoritativeSourceState,
            request.RequestedTargetState,
            decision == "ACCEPTED" ? request.RequestedTargetState : request.AuthoritativeSourceState,
            decision == "ACCEPTED" ? "controlled-reintroduction-complete" : "controlled-reintroduction-denied",
            "lifecycle-validation-evidence:001",
            T0.AddMinutes(3),
            "lifecycle-event:001");

    private static AuthorityRequest AuthorityRequest(LifecycleTransitionResult lifecycle) =>
        new(
            "authority-request:new:001",
            "authority-requester:001",
            "restore-operational-authority",
            "subject:001",
            "controlled-recovery-reintroduction",
            "restricted-operational-scope",
            "recovery-reintroduction",
            "security-context:new:001",
            "fitness:validated-recovery",
            lifecycle.TransitionId,
            T0.AddMinutes(4),
            T0.AddMinutes(30));

    private static AuthorityResult AuthorityResult(
        AuthorityRequest request,
        string decision = "ALLOW") =>
        new(
            request.RequestId,
            "authority-decision:new:001",
            decision,
            decision == "ALLOW" ? request.RequestedScope : "NONE",
            "AUT-001",
            "1.1",
            "validated-recovery-and-controlled-lifecycle-transition",
            "recovery-guard-constraints",
            decision == "ALLOW" ? "new-restoration-authority-allowed" : "new-restoration-authority-denied",
            T0.AddMinutes(5),
            T0.AddMinutes(30),
            "authority-evidence:new:001");

    private static RecoveryReintroductionInput Input(
        ProtectiveRestrictionReleaseFact release,
        bool observationRequired = true,
        RecoveryObservationEvidence? observation = null,
        RecoveryReintroductionTrustEvidence? rechecks = null) =>
        new(
            release.RecoveryCaseIdentity,
            release.SubjectIdentity,
            release.Identity,
            "authority-decision:old-restricted:001",
            "restore-operational-authority",
            "controlled-recovery-reintroduction",
            "restricted-operational-scope",
            rechecks ?? Rechecks(),
            observationRequired,
            observation ?? Observation(),
            "residual-risk:001",
            "data-loss:none:001",
            "capability-loss:none:001",
            "approvals:001",
            "follow-up:001",
            T0.AddMinutes(7));

    private static RecoveryReintroductionDecision Evaluate(
        ProtectiveRestrictionReleaseFact? release = null,
        LifecycleTransitionRequest? lifecycleRequest = null,
        LifecycleTransitionResult? lifecycleResult = null,
        AuthorityRequest? authorityRequest = null,
        AuthorityResult? authorityResult = null,
        RecoveryReintroductionInput? input = null)
    {
        var r = release ?? Release();
        var lr = lifecycleRequest ?? LifecycleRequest(r);
        var lres = lifecycleResult ?? LifecycleResult(lr);
        var ar = authorityRequest ?? AuthorityRequest(lres);
        var ares = authorityResult ?? AuthorityResult(ar);
        var i = input ?? Input(r);
        return RecoveryReintroductionEvaluator.Evaluate(r, lr, lres, ar, ares, i);
    }

    private static void VerifyValidRecoveryComplete()
    {
        var d = Evaluate();
        Require(d.Classification == RecoveryReintroductionClassification.RecoveryComplete, "valid reintroduction did not complete");
        Require(d.Reason == RecoveryReintroductionReason.Complete, "valid reintroduction reason mismatch");
        Require(d.Identity.Length == 64, "reintroduction identity is not SHA-256 length");
    }

    private static void VerifyDeterministicIdentity() =>
        Require(Evaluate().Identity == Evaluate().Identity, "same governed inputs produced different reintroduction identity");

    private static void VerifyMutationSensitivity()
    {
        var r = Release();
        var i1 = Input(r);
        var i2 = i1 with { FollowUpObligationsIdentity = "follow-up:mutated" };
        Require(Evaluate(release: r, input: i1).Identity != Evaluate(release: r, input: i2).Identity,
            "material closure-evidence mutation did not change identity");
    }

    private static void VerifyReleasePartialDenied()
    {
        var r = Release(ProtectiveRestrictionReleaseClassification.Partial);
        var d = Evaluate(release: r, input: Input(r));
        Require(d.Classification == RecoveryReintroductionClassification.Failed && d.Reason == RecoveryReintroductionReason.ReleaseNotComplete,
            "partial WP-08 release entered Lifecycle reintroduction");
    }

    private static void VerifyReleaseUncertainRemainsUncertain()
    {
        var r = Release(ProtectiveRestrictionReleaseClassification.Uncertain);
        var d = Evaluate(release: r, input: Input(r));
        Require(d.Classification == RecoveryReintroductionClassification.Uncertain && d.Reason == RecoveryReintroductionReason.ReleaseNotComplete,
            "uncertain WP-08 release was softened");
    }

    private static void VerifyReleaseIdentityMismatch() => InputBindingMutation(i => i with { ProtectiveRestrictionReleaseFactIdentity = "release-fact:wrong" }, "release identity mismatch");
    private static void VerifyRecoveryCaseMismatch() => InputBindingMutation(i => i with { RecoveryCaseIdentity = "case:wrong" }, "recovery case mismatch");
    private static void VerifySubjectMismatch() => InputBindingMutation(i => i with { SubjectIdentity = "subject:wrong" }, "subject mismatch");

    private static void InputBindingMutation(Func<RecoveryReintroductionInput, RecoveryReintroductionInput> mutate, string label)
    {
        var r = Release();
        var d = Evaluate(release: r, input: mutate(Input(r)));
        Require(d.Reason == RecoveryReintroductionReason.ReleaseBindingMismatch, label + " was accepted");
    }

    private static void VerifyIdentityRecheckUntrusted() => TrustMutation(Rechecks(identityTrusted: false), "identity trust");
    private static void VerifyConfigurationRecheckUntrusted() => TrustMutation(Rechecks(configTrusted: false), "configuration trust");
    private static void VerifyDependencyRecheckUntrusted() => TrustMutation(Rechecks(dependencyTrusted: false), "dependency trust");
    private static void VerifySecurityRecheckUntrusted() => TrustMutation(Rechecks(securityTrusted: false), "security trust");

    private static void TrustMutation(RecoveryReintroductionTrustEvidence evidence, string label)
    {
        var r = Release();
        var d = Evaluate(release: r, input: Input(r, rechecks: evidence));
        Require(d.Classification == RecoveryReintroductionClassification.Uncertain && d.Reason == RecoveryReintroductionReason.ReintroductionTrustInvalid,
            label + " failure did not fail closed");
    }

    private static void VerifyLifecycleRequestInvalid()
    {
        var r = Release();
        var lr = LifecycleRequest(r) with { TransitionRequestId = string.Empty };
        Require(Evaluate(release: r, lifecycleRequest: lr).Reason == RecoveryReintroductionReason.LifecycleRequestInvalid,
            "invalid Lifecycle request accepted");
    }

    private static void VerifyLifecycleSourceInvalid() => LifecycleRequestMutation((r, q) => q with { AuthoritativeSourceState = "RUNNING" }, "invalid source state");
    private static void VerifyLifecycleTargetInvalid() => LifecycleRequestMutation((r, q) => q with { RequestedTargetState = "STOPPED" }, "invalid target state");
    private static void VerifyLifecycleAuthorityBindingMismatch() => LifecycleRequestMutation((r, q) => q with { AuthorityReference = "release-authorization:wrong" }, "release authority binding");
    private static void VerifyLifecycleDependencyBindingMismatch() => LifecycleRequestMutation((r, q) => q with { DependencyContext = "dependency:wrong" }, "dependency binding");
    private static void VerifyLifecycleRequestBeforeRelease() => LifecycleRequestMutation((r, q) => q with { RequestTime = T0, Expiry = T0.AddMinutes(30) }, "pre-release Lifecycle request");

    private static void LifecycleRequestMutation(Func<ProtectiveRestrictionReleaseFact, LifecycleTransitionRequest, LifecycleTransitionRequest> mutate, string label)
    {
        var r = Release();
        var lr = mutate(r, LifecycleRequest(r));
        var lres = LifecycleResult(lr);
        var ar = AuthorityRequest(lres);
        var ares = AuthorityResult(ar);
        var d = RecoveryReintroductionEvaluator.Evaluate(r, lr, lres, ar, ares, Input(r));
        Require(d.Reason == RecoveryReintroductionReason.LifecycleBindingMismatch, label + " was accepted");
    }

    private static void VerifyLifecycleResultRequestMismatch()
    {
        var r = Release(); var lr = LifecycleRequest(r); var lres = LifecycleResult(lr) with { RequestId = "lifecycle-request:wrong" };
        var ar = AuthorityRequest(lres); var ares = AuthorityResult(ar);
        var d = RecoveryReintroductionEvaluator.Evaluate(r, lr, lres, ar, ares, Input(r));
        Require(d.Reason == RecoveryReintroductionReason.LifecycleBindingMismatch, "Lifecycle result request mismatch accepted");
    }

    private static void VerifyLifecycleResultRejected()
    {
        var r = Release(); var lr = LifecycleRequest(r); var lres = LifecycleResult(lr, "REJECTED");
        var ar = AuthorityRequest(lres); var ares = AuthorityResult(ar);
        var d = RecoveryReintroductionEvaluator.Evaluate(r, lr, lres, ar, ares, Input(r));
        Require(d.Reason == RecoveryReintroductionReason.LifecycleTransitionNotAccepted, "rejected Lifecycle transition progressed");
    }

    private static void VerifyLifecycleResultActualStateMismatch()
    {
        var r = Release(); var lr = LifecycleRequest(r); var lres = LifecycleResult(lr) with { ActualResultingState = "RECOVERING" };
        var ar = AuthorityRequest(lres); var ares = AuthorityResult(ar);
        var d = RecoveryReintroductionEvaluator.Evaluate(r, lr, lres, ar, ares, Input(r));
        Require(d.Reason == RecoveryReintroductionReason.LifecycleBindingMismatch, "Lifecycle actual-state mismatch accepted");
    }

    private static void VerifyLifecycleCompletionBeforeRelease()
    {
        var r = Release(); var lr = LifecycleRequest(r); var lres = LifecycleResult(lr) with { CompletionTime = T0 };
        var ar = AuthorityRequest(lres); var ares = AuthorityResult(ar);
        var d = RecoveryReintroductionEvaluator.Evaluate(r, lr, lres, ar, ares, Input(r));
        Require(d.Reason is RecoveryReintroductionReason.LifecycleBindingMismatch or RecoveryReintroductionReason.LifecycleTransitionBeforeRelease,
            "Lifecycle completion before release accepted");
    }

    private static void VerifyNewAuthorityRequestInvalid()
    {
        var r = Release(); var lr = LifecycleRequest(r); var lres = LifecycleResult(lr); var ar = AuthorityRequest(lres) with { RequestId = string.Empty };
        var ares = AuthorityResult(ar with { RequestId = "authority-request:new:001" });
        var d = RecoveryReintroductionEvaluator.Evaluate(r, lr, lres, ar, ares, Input(r));
        Require(d.Reason == RecoveryReintroductionReason.NewAuthorityRequestInvalid, "invalid new authority request accepted");
    }

    private static void VerifyNewAuthorityActionMismatch() => AuthorityRequestMutation(a => a with { Action = "wrong-action" }, "action mismatch");
    private static void VerifyNewAuthorityResourceMismatch() => AuthorityRequestMutation(a => a with { Resource = "subject:wrong" }, "resource mismatch");
    private static void VerifyNewAuthorityPurposeMismatch() => AuthorityRequestMutation(a => a with { Purpose = "wrong-purpose" }, "purpose mismatch");
    private static void VerifyNewAuthorityScopeMismatch() => AuthorityRequestMutation(a => a with { RequestedScope = "wrong-scope" }, "scope mismatch");
    private static void VerifyNewAuthorityCorrelationMismatch() => AuthorityRequestMutation(a => a with { Correlation = "lifecycle-transition:wrong" }, "correlation mismatch");
    private static void VerifyNewAuthorityRequestBeforeLifecycle() => AuthorityRequestMutation(a => a with { RequestTime = T0.AddMinutes(2), Expiry = T0.AddMinutes(30) }, "pre-Lifecycle authority request");

    private static void AuthorityRequestMutation(Func<AuthorityRequest, AuthorityRequest> mutate, string label)
    {
        var r = Release(); var lr = LifecycleRequest(r); var lres = LifecycleResult(lr); var ar = mutate(AuthorityRequest(lres)); var ares = AuthorityResult(ar);
        var d = RecoveryReintroductionEvaluator.Evaluate(r, lr, lres, ar, ares, Input(r));
        Require(d.Reason == RecoveryReintroductionReason.NewAuthorityBindingMismatch, label + " was accepted");
    }

    private static void VerifyNewAuthorityResultInvalid()
    {
        var r = Release(); var lr = LifecycleRequest(r); var lres = LifecycleResult(lr); var ar = AuthorityRequest(lres); var ares = AuthorityResult(ar) with { DecisionId = string.Empty };
        var d = RecoveryReintroductionEvaluator.Evaluate(r, lr, lres, ar, ares, Input(r));
        Require(d.Reason == RecoveryReintroductionReason.NewAuthorityResultInvalid, "invalid new authority result accepted");
    }

    private static void VerifyNewAuthorityResultRequestMismatch()
    {
        var r = Release(); var lr = LifecycleRequest(r); var lres = LifecycleResult(lr); var ar = AuthorityRequest(lres); var ares = AuthorityResult(ar) with { RequestId = "authority-request:wrong" };
        var d = RecoveryReintroductionEvaluator.Evaluate(r, lr, lres, ar, ares, Input(r));
        Require(d.Reason == RecoveryReintroductionReason.NewAuthorityBindingMismatch, "authority result request mismatch accepted");
    }

    private static void VerifyOldAuthorityReuseDenied()
    {
        var r = Release(); var lr = LifecycleRequest(r); var lres = LifecycleResult(lr); var ar = AuthorityRequest(lres); var ares = AuthorityResult(ar) with { DecisionId = "authority-decision:old-restricted:001" };
        var d = RecoveryReintroductionEvaluator.Evaluate(r, lr, lres, ar, ares, Input(r));
        Require(d.Reason == RecoveryReintroductionReason.OldAuthorityReuseDenied, "old pre-restriction authority was reused");
    }

    private static void VerifyNewAuthorityDeniedAfterLifecycle()
    {
        var r = Release(); var lr = LifecycleRequest(r); var lres = LifecycleResult(lr); var ar = AuthorityRequest(lres); var ares = AuthorityResult(ar, "DENY");
        var d = RecoveryReintroductionEvaluator.Evaluate(r, lr, lres, ar, ares, Input(r));
        Require(d.Classification == RecoveryReintroductionClassification.Failed && d.Reason == RecoveryReintroductionReason.NewAuthorityDenied,
            "Lifecycle success was treated as authority after AUT-001 denial");
    }

    private static void VerifyNewAuthorityExpired()
    {
        var r = Release(); var lr = LifecycleRequest(r); var lres = LifecycleResult(lr); var ar = AuthorityRequest(lres);
        var ares = AuthorityResult(ar) with { Expiry = T0.AddMinutes(6) };
        var d = RecoveryReintroductionEvaluator.Evaluate(r, lr, lres, ar, ares, Input(r));
        Require(d.Reason == RecoveryReintroductionReason.NewAuthorityExpired, "expired new authority accepted");
    }

    private static void VerifyObservationRequiredModeMissing()
    {
        var r = Release();
        var d = Evaluate(release: r, input: Input(r, observation: Observation(RecoveryObservationMode.None)));
        Require(d.Reason == RecoveryReintroductionReason.ObservationRequired, "required observation bypassed");
    }

    private static void VerifyObservationUntrusted()
    {
        var r = Release();
        var d = Evaluate(release: r, input: Input(r, observation: Observation(trusted: false)));
        Require(d.Classification == RecoveryReintroductionClassification.Uncertain && d.Reason == RecoveryReintroductionReason.ObservationUncertain,
            "untrusted observation was treated as normal");
    }

    private static void VerifyObservationFailed()
    {
        var r = Release();
        var d = Evaluate(release: r, input: Input(r, observation: Observation(satisfactory: false)));
        Require(d.Classification == RecoveryReintroductionClassification.Failed && d.Reason == RecoveryReintroductionReason.ObservationFailed,
            "failed observation fabricated recovery");
    }

    private static void VerifyObservationInProgressRestricted()
    {
        var r = Release();
        var d = Evaluate(release: r, input: Input(r, observation: Observation(exitAuthorized: false)));
        Require(d.Classification == RecoveryReintroductionClassification.RecoveredWithRestrictedAuthority && d.Reason == RecoveryReintroductionReason.ObservationInProgress,
            "in-progress Recovery Guard was called complete recovery");
    }

    private static void VerifyObservationExitCompletes()
    {
        var r = Release();
        var d = Evaluate(release: r, input: Input(r, observationRequired: true, observation: Observation(RecoveryObservationMode.RecoveryGuard, exitAuthorized: true)));
        Require(d.Classification == RecoveryReintroductionClassification.RecoveryComplete,
            "governed Recovery Guard exit did not permit completion");
    }

    private static void VerifyDirectRunningWithoutValidatedReleaseDenied()
    {
        var r = Release(ProtectiveRestrictionReleaseClassification.Failed);
        var lr = LifecycleRequest(r, target: "RUNNING");
        var lres = LifecycleResult(lr);
        var ar = AuthorityRequest(lres);
        var ares = AuthorityResult(ar);
        var d = RecoveryReintroductionEvaluator.Evaluate(r, lr, lres, ar, ares, Input(r));
        Require(d.Classification == RecoveryReintroductionClassification.Failed && d.Reason == RecoveryReintroductionReason.ReleaseNotComplete,
            "direct RUNNING transition bypassed validated release");
    }

    private static void VerifyClosureEvidencePreserved()
    {
        var d = Evaluate();
        Require(d.ResidualRiskEvidenceIdentity == "residual-risk:001", "residual risk not preserved");
        Require(d.DataLossDeclarationIdentity == "data-loss:none:001", "data-loss declaration not preserved");
        Require(d.CapabilityLossDeclarationIdentity == "capability-loss:none:001", "capability-loss declaration not preserved");
        Require(d.ApprovalEvidenceIdentity == "approvals:001", "approval evidence not preserved");
        Require(d.FollowUpObligationsIdentity == "follow-up:001", "follow-up obligations not preserved");
    }

    private static void VerifyNoFsaOrApplicationBusinessLeakage()
    {
        var typeNames = new[]
        {
            typeof(RecoveryReintroductionDecision).FullName ?? string.Empty,
            typeof(RecoveryReintroductionEvaluator).FullName ?? string.Empty,
            typeof(RecoveryObservationEvidence).FullName ?? string.Empty
        };

        foreach (var value in typeNames)
        {
            Require(!value.Contains("FSA", StringComparison.OrdinalIgnoreCase), "FSA-specific implementation leaked into WP-09");
            Require(!value.Contains("FactoryReset", StringComparison.OrdinalIgnoreCase), "Factory Reset leaked into WP-09");
            Require(!value.Contains("ControlledRevival", StringComparison.OrdinalIgnoreCase), "Controlled Revival leaked into WP-09");
            Require(!value.Contains("Trading", StringComparison.OrdinalIgnoreCase), "Application business semantics leaked into WP-09");
        }
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }
}
