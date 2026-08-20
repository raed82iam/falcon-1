using System;
using System.Collections.Generic;
using System.Linq;
using Foundation.Reconciliation;

internal static class Program
{
    private static readonly DateTimeOffset T0 = new(2026, 8, 15, 18, 0, 0, TimeSpan.Zero);

    private static int Main()
    {
        try
        {
            VerifyValidRelease();
            VerifyDeterministicIdentity();
            VerifyEnforcementOrderDeterminism();
            VerifyMutationSensitivity();
            VerifyAuthorizationDenied();
            VerifyAuthorizationUncertain();
            VerifyAuthorizationExpired();
            VerifyRecoveryCaseMismatch();
            VerifySubjectMismatch();
            VerifyAuthorizationRestrictionMismatch();
            VerifyAuthorizationIntegrityMismatch();
            VerifyAuthorizationIdentityMismatch();
            VerifyReadinessIdentityMismatch();
            VerifyIndependentValidationMismatch();
            VerifyReleaseConditionMismatch();
            VerifyCurrentRestrictionChanged();
            VerifyNewerStricterRestriction();
            VerifyReconciliationChanged();
            VerifySecurityChanged();
            VerifyDependencyChanged();
            VerifyResidualRiskEvidenceChanged();
            VerifyResidualRiskProfileChanged();
            VerifyResidualRiskOutsideBounds();
            VerifyMaterialTrustUncertain();
            VerifyMissingEnforcementIsPartial();
            VerifyUnknownEnforcementIsUncertain();
            VerifyUntrustedEnforcementIsUncertain();
            VerifyFailedEnforcementIsFailed();
            VerifyStillEnforcedIsPartial();
            VerifyOriginalRestrictionPreserved();
            VerifyNoLifecycleOrAuthorityRestorationSurface();
            VerifyApplicationNeutrality();

            Console.WriteLine("STAGE9_WP08_VERIFIER = PASS");
            Console.WriteLine("CHECKS = 32/32");
            Console.WriteLine("RT9_002 = PASS");
            Console.WriteLine("ORIGINAL_RESTRICTION = IMMUTABLE_HISTORY");
            Console.WriteLine("RELEASE_FACT != SECOND_AUTHORITY_DECISION");
            Console.WriteLine("PARTIAL_ENFORCEMENT != COMPLETE_RELEASE");
            Console.WriteLine("UNKNOWN_ENFORCEMENT = FAIL_CLOSED");
            Console.WriteLine("NEWER_STRICTER_RESTRICTION_INVALIDATES_RELEASE_EXECUTION");
            Console.WriteLine("LIFECYCLE_OR_AUTHORITY_RESTORATION_SURFACE = NONE");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("STAGE9_WP08_VERIFIER = FAIL");
            Console.Error.WriteLine(ex.Message);
            return 1;
        }
    }

    private static RecoveryReleaseReadinessDecision Readiness() =>
        new(
            RecoveryReleaseReadinessClassification.ReadyForReleaseDecision,
            RecoveryReleaseReadinessReason.Pass,
            "case:001",
            "plan:001",
            "handoff:001",
            "reconciliation:001",
            "validation:001",
            "restriction:001",
            "restriction-integrity:001",
            "guardian-condition:001",
            "security:001",
            "dependency:001",
            "risk:001",
            "risk-profile:001",
            "release-authority:001",
            T0.AddMinutes(3));

    private static RecoveryReleaseAuthorizationDecision Authorization(
        RecoveryReleaseReadinessDecision readiness,
        RecoveryReleaseAuthorizationClassification classification = RecoveryReleaseAuthorizationClassification.Authorized) =>
        new(
            classification,
            classification == RecoveryReleaseAuthorizationClassification.Authorized
                ? RecoveryReleaseAuthorizationReason.Pass
                : "test-authorization-reason",
            readiness.RecoveryCaseIdentity,
            readiness.AuthorizedRecoveryPlanIdentity,
            "subject:001",
            readiness.CurrentControllingRestrictionIdentity,
            readiness.CurrentRestrictionIntegrityEvidenceIdentity,
            readiness.Identity,
            readiness.DeclaredReleaseAuthorityIdentity,
            "request:release:001",
            "authority-decision:001",
            "policy:release:001",
            "1.0",
            "conditions:release",
            "constraints:release",
            "authority-evidence:001",
            readiness.ResidualRiskEvidenceIdentity,
            readiness.ResidualRiskProfileIdentity,
            T0.AddMinutes(4),
            T0.AddMinutes(30));

    private static RecoveryReleaseTrustSnapshot Snapshot(
        bool newer = false,
        bool reconciliationCurrent = true,
        bool reconciliationTrusted = true,
        bool securityCurrent = true,
        bool securityTrusted = true,
        bool dependencyCurrent = true,
        bool dependencyTrusted = true,
        bool riskCurrent = true,
        bool riskTrusted = true,
        bool riskWithinBounds = true) =>
        new(
            "restriction:001",
            "restriction-integrity:001",
            newer,
            "reconciliation:001",
            reconciliationCurrent,
            reconciliationTrusted,
            "security:001",
            securityCurrent,
            securityTrusted,
            "dependency:001",
            dependencyCurrent,
            dependencyTrusted,
            "risk:001",
            "risk-profile:001",
            riskCurrent,
            riskTrusted,
            riskWithinBounds);

    private static ProtectiveEnforcementReleaseAcknowledgement Ack(
        string id,
        ProtectiveEnforcementReleaseState state = ProtectiveEnforcementReleaseState.Released,
        bool current = true,
        bool trusted = true) =>
        new(id, state, "evidence:" + id, current, trusted, T0.AddMinutes(5));

    private static ProtectiveRestrictionReleaseExecutionInput Input(
        RecoveryReleaseReadinessDecision readiness,
        RecoveryReleaseAuthorizationDecision authorization) =>
        new(
            readiness.RecoveryCaseIdentity,
            authorization.SubjectIdentity,
            readiness.CurrentControllingRestrictionIdentity,
            readiness.CurrentRestrictionIntegrityEvidenceIdentity,
            authorization.Identity,
            readiness.Identity,
            readiness.IndependentValidationIdentity,
            readiness.GuardianConditionEvidenceIdentity,
            Snapshot(),
            new[] { "enforcer:authority", "enforcer:execution", "enforcer:lifecycle" },
            new[] { Ack("enforcer:authority"), Ack("enforcer:execution"), Ack("enforcer:lifecycle") },
            T0.AddMinutes(6));

    private static ProtectiveRestrictionReleaseFact Execute(
        ProtectiveRestrictionReleaseExecutionInput? input = null,
        RecoveryReleaseReadinessDecision? readiness = null,
        RecoveryReleaseAuthorizationDecision? authorization = null)
    {
        var r = readiness ?? Readiness();
        var a = authorization ?? Authorization(r);
        return ProtectiveRestrictionReleaseExecutor.Execute(r, a, input ?? Input(r, a));
    }

    private static void VerifyValidRelease()
    {
        var fact = Execute();
        Require(fact.Classification == ProtectiveRestrictionReleaseClassification.Released, "valid release did not classify RELEASED");
        Require(fact.Reason == ProtectiveRestrictionReleaseReason.Pass, "valid release reason mismatch");
        Require(fact.Identity.Length == 64, "release fact identity is not SHA-256 length");
    }

    private static void VerifyDeterministicIdentity() =>
        Require(Execute().Identity == Execute().Identity, "same release input produced different identity");

    private static void VerifyEnforcementOrderDeterminism()
    {
        var r = Readiness();
        var a = Authorization(r);
        var first = Input(r, a);
        var reordered = first with
        {
            ExpectedEnforcementPointIdentities = first.ExpectedEnforcementPointIdentities.Reverse().ToArray(),
            EnforcementAcknowledgements = first.EnforcementAcknowledgements.Reverse().ToArray()
        };
        Require(Execute(first, r, a).Identity == Execute(reordered, r, a).Identity,
            "enforcement set ordering changed release identity");
    }

    private static void VerifyMutationSensitivity()
    {
        var r = Readiness();
        var a = Authorization(r);
        var original = Input(r, a);
        var mutated = original with
        {
            EnforcementAcknowledgements = new[] { Ack("enforcer:authority"), Ack("enforcer:execution"), new ProtectiveEnforcementReleaseAcknowledgement("enforcer:lifecycle", ProtectiveEnforcementReleaseState.Released, "evidence:mutated", true, true, T0.AddMinutes(5)) }
        };
        Require(Execute(original, r, a).Identity != Execute(mutated, r, a).Identity,
            "material enforcement evidence mutation did not change identity");
    }

    private static void VerifyAuthorizationDenied()
    {
        var r = Readiness(); var a = Authorization(r, RecoveryReleaseAuthorizationClassification.Denied);
        var d = Execute(Input(r, a), r, a);
        Require(d.Classification == ProtectiveRestrictionReleaseClassification.Failed && d.Reason == ProtectiveRestrictionReleaseReason.AuthorizationNotGranted,
            "denied authorization executed release");
    }

    private static void VerifyAuthorizationUncertain()
    {
        var r = Readiness(); var a = Authorization(r, RecoveryReleaseAuthorizationClassification.Uncertain);
        var d = Execute(Input(r, a), r, a);
        Require(d.Classification == ProtectiveRestrictionReleaseClassification.Uncertain,
            "uncertain authorization did not remain uncertain");
    }

    private static void VerifyAuthorizationExpired()
    {
        var r = Readiness(); var a = Authorization(r) with { AuthorityExpiry = T0.AddMinutes(5) };
        var input = Input(r, a) with { ExecutionTime = T0.AddMinutes(6) };
        var d = Execute(input, r, a);
        Require(d.Reason == ProtectiveRestrictionReleaseReason.AuthorizationExpired, "expired release authorization executed");
    }

    private static void VerifyRecoveryCaseMismatch() => BindingMutation(i => i with { RecoveryCaseIdentity = "case:wrong" }, "case mismatch");
    private static void VerifySubjectMismatch() => BindingMutation(i => i with { SubjectIdentity = "subject:wrong" }, "subject mismatch");
    private static void VerifyAuthorizationRestrictionMismatch() => BindingMutation(i => i with { OriginalRestrictionIdentity = "restriction:wrong" }, "authorization restriction mismatch");
    private static void VerifyAuthorizationIntegrityMismatch() => BindingMutation(i => i with { OriginalRestrictionIntegrityEvidenceIdentity = "restriction-integrity:wrong" }, "authorization integrity mismatch");
    private static void VerifyAuthorizationIdentityMismatch() => BindingMutation(i => i with { ReleaseAuthorizationIdentity = "authorization:wrong" }, "authorization identity mismatch");

    private static void BindingMutation(Func<ProtectiveRestrictionReleaseExecutionInput, ProtectiveRestrictionReleaseExecutionInput> mutate, string label)
    {
        var r = Readiness(); var a = Authorization(r); var d = Execute(mutate(Input(r, a)), r, a);
        Require(d.Reason == ProtectiveRestrictionReleaseReason.AuthorizationBindingMismatch, label + " was accepted");
    }

    private static void VerifyReadinessIdentityMismatch()
    {
        var r = Readiness(); var a = Authorization(r); var d = Execute(Input(r, a) with { RecoveryReadinessIdentity = "readiness:wrong" }, r, a);
        Require(d.Reason == ProtectiveRestrictionReleaseReason.AuthorizationBindingMismatch, "readiness identity mismatch accepted");
    }

    private static void VerifyIndependentValidationMismatch()
    {
        var r = Readiness(); var a = Authorization(r); var d = Execute(Input(r, a) with { IndependentValidationIdentity = "validation:wrong" }, r, a);
        Require(d.Reason == ProtectiveRestrictionReleaseReason.ReadinessBindingMismatch, "independent validation mismatch accepted");
    }

    private static void VerifyReleaseConditionMismatch()
    {
        var r = Readiness(); var a = Authorization(r); var d = Execute(Input(r, a) with { ReleaseConditionSatisfactionIdentity = "condition:wrong" }, r, a);
        Require(d.Reason == ProtectiveRestrictionReleaseReason.ReadinessBindingMismatch, "release condition mismatch accepted");
    }

    private static void VerifyCurrentRestrictionChanged()
    {
        var r = Readiness(); var a = Authorization(r); var i = Input(r, a) with { CurrentTrustSnapshot = Snapshot() with { CurrentControllingRestrictionIdentity = "restriction:new" } };
        Require(Execute(i, r, a).Reason == ProtectiveRestrictionReleaseReason.RestrictionChanged, "changed restriction executed");
    }

    private static void VerifyNewerStricterRestriction()
    {
        var r = Readiness(); var a = Authorization(r); var i = Input(r, a) with { CurrentTrustSnapshot = Snapshot(newer: true) };
        Require(Execute(i, r, a).Reason == ProtectiveRestrictionReleaseReason.NewerStricterRestriction, "newer stricter restriction executed");
    }

    private static void VerifyReconciliationChanged() => SnapshotMutation(s => s with { RecoveryReconciliationIdentity = "reconciliation:new" }, ProtectiveRestrictionReleaseReason.ReconciliationChanged, "reconciliation change");
    private static void VerifySecurityChanged() => SnapshotMutation(s => s with { SecurityStateEvidenceIdentity = "security:new" }, ProtectiveRestrictionReleaseReason.SecurityChanged, "security change");
    private static void VerifyDependencyChanged() => SnapshotMutation(s => s with { DependencyStateEvidenceIdentity = "dependency:new" }, ProtectiveRestrictionReleaseReason.DependencyChanged, "dependency change");
    private static void VerifyResidualRiskEvidenceChanged() => SnapshotMutation(s => s with { ResidualRiskEvidenceIdentity = "risk:new" }, ProtectiveRestrictionReleaseReason.ResidualRiskChanged, "risk evidence change");
    private static void VerifyResidualRiskProfileChanged() => SnapshotMutation(s => s with { ResidualRiskProfileIdentity = "risk-profile:new" }, ProtectiveRestrictionReleaseReason.ResidualRiskChanged, "risk profile change");
    private static void VerifyResidualRiskOutsideBounds() => SnapshotMutation(s => s with { ResidualRiskWithinAuthorizedBounds = false }, ProtectiveRestrictionReleaseReason.ResidualRiskChanged, "risk bounds change");

    private static void SnapshotMutation(Func<RecoveryReleaseTrustSnapshot, RecoveryReleaseTrustSnapshot> mutate, string reason, string label)
    {
        var r = Readiness(); var a = Authorization(r); var i = Input(r, a) with { CurrentTrustSnapshot = mutate(Snapshot()) };
        Require(Execute(i, r, a).Reason == reason, label + " was accepted");
    }

    private static void VerifyMaterialTrustUncertain()
    {
        var r = Readiness(); var a = Authorization(r); var i = Input(r, a) with { CurrentTrustSnapshot = Snapshot(securityTrusted: false) };
        var d = Execute(i, r, a);
        Require(d.Classification == ProtectiveRestrictionReleaseClassification.Uncertain && d.Reason == ProtectiveRestrictionReleaseReason.MaterialTrustUncertain,
            "untrusted current material state executed release");
    }

    private static void VerifyMissingEnforcementIsPartial()
    {
        var r = Readiness(); var a = Authorization(r); var i = Input(r, a) with { EnforcementAcknowledgements = new[] { Ack("enforcer:authority"), Ack("enforcer:execution") } };
        var d = Execute(i, r, a);
        Require(d.Classification == ProtectiveRestrictionReleaseClassification.Partial && d.Reason == ProtectiveRestrictionReleaseReason.EnforcementEvidenceMissing,
            "missing enforcement acknowledgement became complete release");
    }

    private static void VerifyUnknownEnforcementIsUncertain() => EnforcementState(ProtectiveEnforcementReleaseState.Unknown, true, true, ProtectiveRestrictionReleaseClassification.Uncertain, ProtectiveRestrictionReleaseReason.EnforcementEvidenceUncertain);
    private static void VerifyUntrustedEnforcementIsUncertain() => EnforcementState(ProtectiveEnforcementReleaseState.Released, true, false, ProtectiveRestrictionReleaseClassification.Uncertain, ProtectiveRestrictionReleaseReason.EnforcementEvidenceUncertain);
    private static void VerifyFailedEnforcementIsFailed() => EnforcementState(ProtectiveEnforcementReleaseState.Failed, true, true, ProtectiveRestrictionReleaseClassification.Failed, ProtectiveRestrictionReleaseReason.EnforcementFailed);
    private static void VerifyStillEnforcedIsPartial() => EnforcementState(ProtectiveEnforcementReleaseState.StillEnforced, true, true, ProtectiveRestrictionReleaseClassification.Partial, ProtectiveRestrictionReleaseReason.EnforcementPartial);

    private static void EnforcementState(ProtectiveEnforcementReleaseState state, bool current, bool trusted, ProtectiveRestrictionReleaseClassification classification, string reason)
    {
        var r = Readiness(); var a = Authorization(r); var i = Input(r, a) with
        {
            EnforcementAcknowledgements = new[] { Ack("enforcer:authority"), Ack("enforcer:execution", state, current, trusted), Ack("enforcer:lifecycle") }
        };
        var d = Execute(i, r, a);
        Require(d.Classification == classification && d.Reason == reason, "enforcement failure classification was softened");
    }

    private static void VerifyOriginalRestrictionPreserved()
    {
        var d = Execute();
        Require(d.OriginalRestrictionIdentity == "restriction:001" && d.OriginalRestrictionIntegrityEvidenceIdentity == "restriction-integrity:001",
            "release fact did not preserve original restriction identity/integrity evidence");
    }

    private static void VerifyNoLifecycleOrAuthorityRestorationSurface()
    {
        foreach (var property in typeof(ProtectiveRestrictionReleaseFact).GetProperties())
        {
            Require(!property.Name.Contains("LifecycleTransition", StringComparison.OrdinalIgnoreCase), "release fact exposes Lifecycle transition");
            Require(!property.Name.Contains("AuthorityRestored", StringComparison.OrdinalIgnoreCase), "release fact exposes authority restoration");
            Require(!property.Name.Contains("NewAuthorityDecision", StringComparison.OrdinalIgnoreCase), "release fact exposes new authority decision");
        }
    }

    private static void VerifyApplicationNeutrality()
    {
        foreach (var reference in typeof(ProtectiveRestrictionReleaseFact).Assembly.GetReferencedAssemblies())
        {
            var name = reference.Name ?? string.Empty;
            Require(!name.Contains("Application", StringComparison.OrdinalIgnoreCase), "Application dependency leaked into WP08");
            Require(!name.Contains("Trading", StringComparison.OrdinalIgnoreCase), "Trading dependency leaked into WP08");
            Require(!name.Contains("Web", StringComparison.OrdinalIgnoreCase), "Web dependency leaked into WP08");
            Require(!name.Contains("SelfAwareness", StringComparison.OrdinalIgnoreCase), "Stage13/FSA dependency leaked into WP08");
        }
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }
}
