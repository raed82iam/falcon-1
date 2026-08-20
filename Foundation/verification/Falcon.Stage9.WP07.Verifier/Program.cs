using System;
using Foundation.Authority;
using Foundation.Contracts;
using Foundation.Reconciliation;

internal static class Program
{
    private static readonly DateTimeOffset T0 = new(2026, 8, 15, 18, 0, 0, TimeSpan.Zero);
    private const string ReleaseAction = "release-action:controlled-restriction";
    private const string ReleaseResource = "release-resource:subject:001:restriction:001";
    private const string ReleasePurpose = "release-purpose:controlled-recovery";
    private const string ReleaseScope = "release-scope:subject:001";

    private static int Main()
    {
        try
        {
            VerifyValidAut001BackedAuthorization();
            VerifyDeterministicIdentity();
            VerifyMutationSensitivity();
            VerifyReadinessNotReadyDenied();
            VerifyReadinessIdentityMismatchDenied();
            VerifyRecoveryCaseMismatchDenied();
            VerifyPlanMismatchDenied();
            VerifyDeclaredReleaseAuthorityMismatchDenied();
            VerifySubjectCannotBeReleaseAuthority();
            VerifyGuardianCannotBeReleaseAuthority();
            VerifyRepairActorCannotBeReleaseAuthority();
            VerifyIndependentVerifierCannotBeReleaseAuthority();
            VerifyAuthorityActorMismatchDenied();
            VerifyAuthorityActionMismatchDenied();
            VerifyAuthorityResourceMismatchDenied();
            VerifyAuthorityPurposeMismatchDenied();
            VerifyAuthorityScopeMismatchDenied();
            VerifyReadinessCorrelationMismatchDenied();
            VerifyAuthorityResultRequestMismatchDenied();
            VerifyAut001DenialRemainsDenied();
            VerifyRevokedDelegationDenied();
            VerifyAmbiguousPolicyDenied();
            VerifyRestrictionChangeDenied();
            VerifyNewerStricterRestrictionDenied();
            VerifyReconciliationSnapshotChangeDenied();
            VerifySecuritySnapshotChangeDenied();
            VerifyDependencySnapshotChangeDenied();
            VerifyResidualRiskSnapshotChangeDenied();
            VerifyUntrustedMaterialSnapshotUncertain();
            VerifyNoReleaseExecutionOrLifecycleSurface();
            VerifyApplicationNeutrality();

            Console.WriteLine("STAGE9_WP07_VERIFIER = PASS");
            Console.WriteLine("CHECKS = 31/31");
            Console.WriteLine("RT9_002 = PASS");
            Console.WriteLine("RELEASE_AUTHORIZATION != RELEASE_EXECUTION");
            Console.WriteLine("ROLE_LABEL != AUTHORITY");
            Console.WriteLine("STALE_READINESS_OR_TRUST_SNAPSHOT = DENIED");
            Console.WriteLine("NEWER_STRICTER_RESTRICTION_INVALIDATES_RELEASE_AUTHORIZATION");
            Console.WriteLine("AUT001 = RELEASE_AUTHORITY_EVALUATOR");
            Console.WriteLine("LIFECYCLE_OR_RESTRICTION_RELEASE_SURFACE = NONE");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("STAGE9_WP07_VERIFIER = FAIL");
            Console.Error.WriteLine(ex.Message);
            return 1;
        }
    }

    private static RecoveryReleaseReadinessDecision Readiness(
        RecoveryReleaseReadinessClassification classification = RecoveryReleaseReadinessClassification.ReadyForReleaseDecision) =>
        new(
            classification,
            classification == RecoveryReleaseReadinessClassification.ReadyForReleaseDecision
                ? RecoveryReleaseReadinessReason.Pass
                : "test-readiness-reason",
            "case:001",
            "plan:001",
            "handoff:001",
            "reconciliation:001",
            "validation:001",
            "restriction:001",
            "restriction-integrity:001",
            "evidence:guardian",
            "evidence:security",
            "evidence:dependency",
            "evidence:residual-risk",
            "risk-profile:authorized",
            "release-authority:001",
            T0.AddMinutes(3));

    private static RecoveryReleaseTrustSnapshot Snapshot() =>
        new(
            "restriction:001",
            "restriction-integrity:001",
            false,
            "reconciliation:001",
            true,
            true,
            "evidence:security",
            true,
            true,
            "evidence:dependency",
            true,
            true,
            "evidence:residual-risk",
            "risk-profile:authorized",
            true,
            true,
            true);

    private static RecoveryReleaseAuthorizationInput Input(RecoveryReleaseReadinessDecision readiness) =>
        new(
            "case:001",
            "plan:001",
            "subject:001",
            "guardian:001",
            "repair:001",
            "verifier:001",
            "release-authority:001",
            readiness.Identity,
            ReleaseAction,
            ReleaseResource,
            ReleasePurpose,
            ReleaseScope,
            Snapshot(),
            T0.AddMinutes(4));

    private static AuthorityRequest Request(
        RecoveryReleaseReadinessDecision readiness,
        string actor = "release-authority:001",
        string action = ReleaseAction,
        string resource = ReleaseResource,
        string purpose = ReleasePurpose,
        string scope = ReleaseScope,
        string? correlation = null) =>
        new(
            "authority-request:release:001",
            actor,
            action,
            resource,
            purpose,
            scope,
            "operating-context:recovery",
            "security-context:trusted",
            "FIT",
            correlation ?? readiness.Identity,
            T0.AddMinutes(3),
            T0.AddMinutes(30));

    private static AuthorityResult Aut001(
        AuthorityRequest request,
        bool revoked = false,
        bool ambiguous = false,
        bool denyAction = false)
    {
        var policy = new AuthorityPolicy(
            "policy:release:001",
            "1",
            "authority-provenance:release:001",
            T0,
            T0.AddHours(1),
            new[] { request.ActorIdentity },
            new[] { denyAction ? "other-action" : request.Action },
            new[] { request.Resource },
            new[] { request.Purpose },
            new[] { request.RequestedScope },
            new[] { request.SecurityContext },
            ambiguous);

        var delegation = new DelegationEvidence(
            "delegation:release:001",
            request.ActorIdentity,
            policy.AuthorityProvenance,
            new[] { request.RequestedScope },
            T0,
            T0.AddHours(1),
            revoked);

        var fitness = new FitnessEvidence(
            request.ActorIdentity,
            request.RequiredFitnessToOperate,
            true,
            T0.AddMinutes(2),
            T0.AddHours(1),
            "evidence:fitness:release");

        var context = new AuthorityEvaluationContext(
            policy,
            delegation,
            fitness,
            T0.AddMinutes(4),
            "evidence:authority-evaluation:release");

        return new DefaultDenyAuthorityEngine().Evaluate(request, context);
    }

    private static RecoveryReleaseAuthorizationDecision Evaluate(
        RecoveryReleaseReadinessDecision? readiness = null,
        RecoveryReleaseAuthorizationInput? input = null,
        AuthorityRequest? request = null,
        AuthorityResult? result = null)
    {
        var r = readiness ?? Readiness();
        var i = input ?? Input(r);
        var q = request ?? Request(r);
        var a = result ?? Aut001(q);
        return RecoveryReleaseAuthorizationEvaluator.Evaluate(r, q, a, i);
    }

    private static void VerifyValidAut001BackedAuthorization()
    {
        var d = Evaluate();
        Require(d.Classification == RecoveryReleaseAuthorizationClassification.Authorized,
            "valid AUT-001-backed release authorization was not authorized");
        Require(d.Reason == RecoveryReleaseAuthorizationReason.Pass,
            "valid release authorization reason was not RELEASE_AUTHORIZED");
        Require(d.Identity.Length == 64, "release authorization identity is not SHA-256 length");
    }

    private static void VerifyDeterministicIdentity()
    {
        Require(Evaluate().Identity == Evaluate().Identity,
            "same trusted release authorization inputs produced different identities");
    }

    private static void VerifyMutationSensitivity()
    {
        var r = Readiness();
        var a = Evaluate(r);
        var i = Input(r) with
        {
            CurrentTrustSnapshot = Snapshot() with { ResidualRiskEvidenceIdentity = "evidence:residual-risk:mutated" }
        };
        var b = Evaluate(r, i);
        Require(a.Identity != b.Identity, "material trust mutation did not change release authorization identity");
    }

    private static void VerifyReadinessNotReadyDenied()
    {
        var r = Readiness(RecoveryReleaseReadinessClassification.NotReady);
        var d = Evaluate(r, Input(r), Request(r));
        Require(d.Classification == RecoveryReleaseAuthorizationClassification.Denied &&
                d.Reason == RecoveryReleaseAuthorizationReason.ReadinessNotReady,
            "NOT_READY became release authorization");
    }

    private static void VerifyReadinessIdentityMismatchDenied()
    {
        var r = Readiness();
        var d = Evaluate(r, Input(r) with { RecoveryReadinessIdentity = "readiness:wrong" });
        Require(d.Reason == RecoveryReleaseAuthorizationReason.ReadinessIdentityMismatch,
            "readiness identity mismatch was accepted");
    }

    private static void VerifyRecoveryCaseMismatchDenied()
    {
        var r = Readiness();
        var d = Evaluate(r, Input(r) with { RecoveryCaseIdentity = "case:wrong" });
        Require(d.Reason == RecoveryReleaseAuthorizationReason.RecoveryBindingMismatch,
            "recovery case mismatch was accepted");
    }

    private static void VerifyPlanMismatchDenied()
    {
        var r = Readiness();
        var d = Evaluate(r, Input(r) with { AuthorizedRecoveryPlanIdentity = "plan:wrong" });
        Require(d.Reason == RecoveryReleaseAuthorizationReason.RecoveryBindingMismatch,
            "authorized recovery plan mismatch was accepted");
    }

    private static void VerifyDeclaredReleaseAuthorityMismatchDenied()
    {
        var r = Readiness();
        var d = Evaluate(r, Input(r) with { DeclaredReleaseAuthorityIdentity = "release-authority:wrong" });
        Require(d.Reason == RecoveryReleaseAuthorizationReason.ReleaseAuthorityMismatch,
            "declared release authority mismatch was accepted");
    }

    private static void VerifySubjectCannotBeReleaseAuthority()
    {
        var r = Readiness() with { DeclaredReleaseAuthorityIdentity = "subject:001" };
        var i = Input(r) with { DeclaredReleaseAuthorityIdentity = "subject:001" };
        var q = Request(r, actor: "subject:001");
        var d = Evaluate(r, i, q, Aut001(q));
        Require(d.Reason == RecoveryReleaseAuthorizationReason.ReleaseAuthorityRoleConflict,
            "subject became release authority");
    }

    private static void VerifyGuardianCannotBeReleaseAuthority()
    {
        var r = Readiness() with { DeclaredReleaseAuthorityIdentity = "guardian:001" };
        var i = Input(r) with { DeclaredReleaseAuthorityIdentity = "guardian:001" };
        var q = Request(r, actor: "guardian:001");
        var d = Evaluate(r, i, q, Aut001(q));
        Require(d.Reason == RecoveryReleaseAuthorizationReason.ReleaseAuthorityRoleConflict,
            "Guardian became self-release authority");
    }

    private static void VerifyRepairActorCannotBeReleaseAuthority()
    {
        var r = Readiness() with { DeclaredReleaseAuthorityIdentity = "repair:001" };
        var i = Input(r) with { DeclaredReleaseAuthorityIdentity = "repair:001" };
        var q = Request(r, actor: "repair:001");
        var d = Evaluate(r, i, q, Aut001(q));
        Require(d.Reason == RecoveryReleaseAuthorizationReason.ReleaseAuthorityRoleConflict,
            "repair actor became release authority");
    }

    private static void VerifyIndependentVerifierCannotBeReleaseAuthority()
    {
        var r = Readiness() with { DeclaredReleaseAuthorityIdentity = "verifier:001" };
        var i = Input(r) with { DeclaredReleaseAuthorityIdentity = "verifier:001" };
        var q = Request(r, actor: "verifier:001");
        var d = Evaluate(r, i, q, Aut001(q));
        Require(d.Reason == RecoveryReleaseAuthorizationReason.ReleaseAuthorityRoleConflict,
            "independent verifier became release authority");
    }

    private static void VerifyAuthorityActorMismatchDenied()
    {
        var r = Readiness();
        var q = Request(r, actor: "role-label-only:release-authority");
        var d = Evaluate(r, Input(r), q, Aut001(q));
        Require(d.Reason == RecoveryReleaseAuthorizationReason.AuthorityRequestMismatch,
            "role label spoofing bypassed exact declared release authority identity");
    }

    private static void VerifyAuthorityActionMismatchDenied()
    {
        var r = Readiness();
        var q = Request(r, action: "other-action");
        var d = Evaluate(r, Input(r), q, Aut001(q));
        Require(d.Reason == RecoveryReleaseAuthorizationReason.AuthorityRequestMismatch,
            "release action mismatch was accepted");
    }

    private static void VerifyAuthorityResourceMismatchDenied()
    {
        var r = Readiness();
        var q = Request(r, resource: "other-resource");
        var d = Evaluate(r, Input(r), q, Aut001(q));
        Require(d.Reason == RecoveryReleaseAuthorizationReason.AuthorityRequestMismatch,
            "release resource mismatch was accepted");
    }

    private static void VerifyAuthorityPurposeMismatchDenied()
    {
        var r = Readiness();
        var q = Request(r, purpose: "other-purpose");
        var d = Evaluate(r, Input(r), q, Aut001(q));
        Require(d.Reason == RecoveryReleaseAuthorizationReason.AuthorityRequestMismatch,
            "release purpose mismatch was accepted");
    }

    private static void VerifyAuthorityScopeMismatchDenied()
    {
        var r = Readiness();
        var q = Request(r, scope: "other-scope");
        var d = Evaluate(r, Input(r), q, Aut001(q));
        Require(d.Reason == RecoveryReleaseAuthorizationReason.AuthorityRequestMismatch,
            "release scope mismatch was accepted");
    }

    private static void VerifyReadinessCorrelationMismatchDenied()
    {
        var r = Readiness();
        var q = Request(r, correlation: "readiness:wrong");
        var d = Evaluate(r, Input(r), q, Aut001(q));
        Require(d.Reason == RecoveryReleaseAuthorizationReason.AuthorityRequestMismatch,
            "authority request was not bound to exact readiness identity");
    }

    private static void VerifyAuthorityResultRequestMismatchDenied()
    {
        var r = Readiness();
        var q = Request(r);
        var a = Aut001(q) with { RequestId = "authority-request:wrong" };
        var d = Evaluate(r, Input(r), q, a);
        Require(d.Reason == RecoveryReleaseAuthorizationReason.AuthorityResultMismatch,
            "authority result/request mismatch was accepted");
    }

    private static void VerifyAut001DenialRemainsDenied()
    {
        var r = Readiness();
        var q = Request(r);
        var d = Evaluate(r, Input(r), q, Aut001(q, denyAction: true));
        Require(d.Classification == RecoveryReleaseAuthorizationClassification.Denied &&
                d.Reason == RecoveryReleaseAuthorizationReason.AuthorityDenied,
            "AUT-001 DENY was converted into release authorization");
    }

    private static void VerifyRevokedDelegationDenied()
    {
        var r = Readiness();
        var q = Request(r);
        var d = Evaluate(r, Input(r), q, Aut001(q, revoked: true));
        Require(d.Classification == RecoveryReleaseAuthorizationClassification.Denied &&
                d.Reason == RecoveryReleaseAuthorizationReason.AuthorityDenied,
            "revoked AUT-001 delegation was accepted");
    }

    private static void VerifyAmbiguousPolicyDenied()
    {
        var r = Readiness();
        var q = Request(r);
        var d = Evaluate(r, Input(r), q, Aut001(q, ambiguous: true));
        Require(d.Classification == RecoveryReleaseAuthorizationClassification.Denied &&
                d.Reason == RecoveryReleaseAuthorizationReason.AuthorityDenied,
            "ambiguous AUT-001 policy was accepted");
    }

    private static void VerifyRestrictionChangeDenied()
    {
        var r = Readiness();
        var i = Input(r) with
        {
            CurrentTrustSnapshot = Snapshot() with { CurrentControllingRestrictionIdentity = "restriction:new" }
        };
        var d = Evaluate(r, i);
        Require(d.Reason == RecoveryReleaseAuthorizationReason.RestrictionChanged,
            "changed controlling restriction was accepted");
    }

    private static void VerifyNewerStricterRestrictionDenied()
    {
        var r = Readiness();
        var i = Input(r) with
        {
            CurrentTrustSnapshot = Snapshot() with { NewerOrStricterRestrictionPresent = true }
        };
        var d = Evaluate(r, i);
        Require(d.Reason == RecoveryReleaseAuthorizationReason.NewerStricterRestriction,
            "newer/stricter restriction did not invalidate release authorization");
    }

    private static void VerifyReconciliationSnapshotChangeDenied()
    {
        var r = Readiness();
        var i = Input(r) with
        {
            CurrentTrustSnapshot = Snapshot() with { RecoveryReconciliationIdentity = "reconciliation:new" }
        };
        var d = Evaluate(r, i);
        Require(d.Reason == RecoveryReleaseAuthorizationReason.ReconciliationChanged,
            "changed reconciliation snapshot was accepted");
    }

    private static void VerifySecuritySnapshotChangeDenied()
    {
        var r = Readiness();
        var i = Input(r) with
        {
            CurrentTrustSnapshot = Snapshot() with { SecurityStateEvidenceIdentity = "evidence:security:new" }
        };
        var d = Evaluate(r, i);
        Require(d.Reason == RecoveryReleaseAuthorizationReason.SecurityStateChanged,
            "changed security snapshot was accepted");
    }

    private static void VerifyDependencySnapshotChangeDenied()
    {
        var r = Readiness();
        var i = Input(r) with
        {
            CurrentTrustSnapshot = Snapshot() with { DependencyStateEvidenceIdentity = "evidence:dependency:new" }
        };
        var d = Evaluate(r, i);
        Require(d.Reason == RecoveryReleaseAuthorizationReason.DependencyStateChanged,
            "changed dependency snapshot was accepted");
    }

    private static void VerifyResidualRiskSnapshotChangeDenied()
    {
        var r = Readiness();
        var i = Input(r) with
        {
            CurrentTrustSnapshot = Snapshot() with { ResidualRiskWithinAuthorizedBounds = false }
        };
        var d = Evaluate(r, i);
        Require(d.Reason == RecoveryReleaseAuthorizationReason.ResidualRiskChanged,
            "residual risk outside authorized bounds was accepted");
    }

    private static void VerifyUntrustedMaterialSnapshotUncertain()
    {
        var r = Readiness();
        var i = Input(r) with
        {
            CurrentTrustSnapshot = Snapshot() with { SecurityStateTrusted = false }
        };
        var d = Evaluate(r, i);
        Require(d.Classification == RecoveryReleaseAuthorizationClassification.Uncertain &&
                d.Reason == RecoveryReleaseAuthorizationReason.MaterialTrustUncertain,
            "untrusted material trust snapshot became release authorization");
    }

    private static void VerifyNoReleaseExecutionOrLifecycleSurface()
    {
        foreach (var property in typeof(RecoveryReleaseAuthorizationDecision).GetProperties())
        {
            Require(!property.Name.Contains("Executed", StringComparison.OrdinalIgnoreCase),
                "release authorization exposes execution result");
            Require(!property.Name.Contains("Lifecycle", StringComparison.OrdinalIgnoreCase),
                "release authorization exposes Lifecycle transition");
            Require(!property.Name.Contains("RestrictionReleased", StringComparison.OrdinalIgnoreCase),
                "release authorization exposes restriction release fact");
            Require(!property.Name.Contains("OperationalAuthority", StringComparison.OrdinalIgnoreCase),
                "release authorization exposes restored operational authority");
        }
    }

    private static void VerifyApplicationNeutrality()
    {
        foreach (var reference in typeof(RecoveryReleaseAuthorizationDecision).Assembly.GetReferencedAssemblies())
        {
            var name = reference.Name ?? string.Empty;
            Require(!name.Contains("Application", StringComparison.OrdinalIgnoreCase), "Application dependency leaked into release authorization");
            Require(!name.Contains("Trading", StringComparison.OrdinalIgnoreCase), "Trading dependency leaked into release authorization");
            Require(!name.Contains("Web", StringComparison.OrdinalIgnoreCase), "Web dependency leaked into release authorization");
            Require(!name.Contains("SelfAwareness", StringComparison.OrdinalIgnoreCase), "Stage13/FSA dependency leaked into release authorization");
        }
    }

    private static void Require(bool condition, string message)
    {
        if (!condition)
            throw new InvalidOperationException(message);
    }
}
