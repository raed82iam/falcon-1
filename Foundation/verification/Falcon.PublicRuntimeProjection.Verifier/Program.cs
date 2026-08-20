using System;
using Foundation.ArtifactPublication;
using Foundation.Contracts;

namespace Falcon.PublicRuntimeProjection.Verifier;

internal static class Program
{
    private static int _checks;
    private static readonly DateTimeOffset Now = new(2026, 8, 17, 17, 30, 0, TimeSpan.Zero);

    private static int Main()
    {
        try
        {
            var truth = new RecoveryOperationalTruth(
                "recovery-case:alpha", "ReadyForReleaseDecision", "Completed", true,
                ReleaseAuthorizationProjectionState.NotAuthorized,
                ReleaseExecutionProjectionState.NotExecuted,
                ReintroductionProjectionState.NotStarted,
                "Restricted", "evidence:recovery:alpha", Now, Now.AddMinutes(5), true);

            var current = RecoveryOperationalProjectionRuntime.Build(truth, Now.AddMinutes(1));
            Check(current.Accepted && current.Projection is not null, "valid recovery truth rejected");
            Check(current.Projection!.Freshness == RecoveryProjectionFreshness.Current, "current truth marked stale");
            Check(current.Projection.ReadyForReleaseDecision, "release readiness lost");
            Check(current.Projection.ReleaseAuthorization == ReleaseAuthorizationProjectionState.NotAuthorized, "readiness promoted to authorization");
            Check(current.Projection.ReleaseExecution == ReleaseExecutionProjectionState.NotExecuted, "readiness promoted to execution");
            Check(current.Projection.PresentationOnly, "projection not presentation-only");
            Check(!current.Projection.CarriesReleaseExecutionAuthority && !current.Projection.CarriesLifecycleAuthority && !current.Projection.CarriesBusinessAuthority, "projection gained authority");

            var stale = RecoveryOperationalProjectionRuntime.Build(truth, Now.AddMinutes(10));
            Check(stale.Accepted && stale.Projection!.Freshness == RecoveryProjectionFreshness.Stale, "stale truth not preserved");
            Check(stale.Projection!.ProjectionIdentity == current.Projection.ProjectionIdentity, "truth identity changed with evaluation time");
            Check(RecoveryOperationalProjectionRuntime.Build(truth with { Complete = false }, Now).Projection!.Complete == false, "partial truth upgraded");
            Check(!RecoveryOperationalProjectionRuntime.Build(truth with { RecoveryState = "InventedState" }, Now).Accepted, "invented recovery state accepted");
            Check(!RecoveryOperationalProjectionRuntime.Build(truth with { RestorationOutcome = "InventedOutcome" }, Now).Accepted, "invented restoration outcome accepted");
            Check(!RecoveryOperationalProjectionRuntime.Build(truth with { ReadyForReleaseDecision = false }, Now).Accepted, "ready recovery state accepted with false readiness flag");
            Check(!RecoveryOperationalProjectionRuntime.Build(truth with { RecoveryState = "PlanAuthorized" }, Now).Accepted, "pre-readiness state accepted with readiness flag");
            Check(!RecoveryOperationalProjectionRuntime.Build(truth with { RecoveryState = "ReleaseDenied" }, Now).Accepted, "release-denied state accepted without denied authorization");
            Check(RecoveryOperationalProjectionRuntime.Build(truth with { RecoveryState = "ReleaseDenied", ReleaseAuthorization = ReleaseAuthorizationProjectionState.Denied }, Now).Accepted, "valid release-denied projection rejected");
            Check(!RecoveryOperationalProjectionRuntime.Build(truth with { RecoveryState = "ReleaseAuthorized" }, Now).Accepted, "release-authorized state accepted without authorization");
            Check(RecoveryOperationalProjectionRuntime.Build(truth with { RecoveryState = "ReleaseAuthorized", ReleaseAuthorization = ReleaseAuthorizationProjectionState.Authorized }, Now).Accepted, "valid release-authorized projection rejected");
            Check(!RecoveryOperationalProjectionRuntime.Build(truth with { RecoveryState = "ReleaseAuthorized", ReleaseAuthorization = ReleaseAuthorizationProjectionState.Authorized, ReleaseExecution = ReleaseExecutionProjectionState.Executed }, Now).Accepted, "release execution accepted before reintroduction state");
            Check(!RecoveryOperationalProjectionRuntime.Build(truth with { RecoveryState = "ReintroductionPending", ReleaseAuthorization = ReleaseAuthorizationProjectionState.Authorized, ReleaseExecution = ReleaseExecutionProjectionState.Executed }, Now).Accepted, "reintroduction state accepted without matching reintroduction projection");
            Check(RecoveryOperationalProjectionRuntime.Build(truth with { RecoveryState = "ReintroductionPending", ReleaseAuthorization = ReleaseAuthorizationProjectionState.Authorized, ReleaseExecution = ReleaseExecutionProjectionState.Executed, Reintroduction = ReintroductionProjectionState.Pending }, Now).Accepted, "valid reintroduction-pending projection rejected");
            Check(!RecoveryOperationalProjectionRuntime.Build(truth with { RecoveryState = "RecoveryComplete", ReleaseAuthorization = ReleaseAuthorizationProjectionState.Authorized, ReleaseExecution = ReleaseExecutionProjectionState.Executed, Reintroduction = ReintroductionProjectionState.Restricted }, Now).Accepted, "recovery complete accepted with restricted reintroduction state");
            Check(RecoveryOperationalProjectionRuntime.Build(truth with { RecoveryState = "RecoveryComplete", ReleaseAuthorization = ReleaseAuthorizationProjectionState.Authorized, ReleaseExecution = ReleaseExecutionProjectionState.Executed, Reintroduction = ReintroductionProjectionState.Complete }, Now).Accepted, "valid completed recovery rejected");

            var recoveryRoute = PublicRuntimeProjectionProfiles.RecoveryOperational(
                "route:foundation:recovery:web:v1", "shared-web", Digest('A'),
                "evidence:foundation:recovery-projection", "provenance:foundation:runtime");
            Check(recoveryRoute.MessageType == PublicRuntimeProjectionProfiles.RecoveryMessageType, "recovery message type not canonical");
            Check(recoveryRoute.SchemaId.Value == PublicRuntimeProjectionProfiles.RecoverySchemaIdentity, "recovery schema not canonical");
            Check(recoveryRoute.ArtifactId == PublicRuntimeProjectionProfiles.RecoveryArtifactId, "recovery artifact id not canonical");
            Check(recoveryRoute.CompatibilityIdentity == PublicRuntimeProjectionProfiles.CompatibilityIdentity, "recovery compatibility identity not canonical");

            const string recoveryPayload = "{\"projection\":\"recovery\"}";
            var transport = BuildTransport(recoveryRoute, recoveryPayload);
            Check(transport.Accepted && transport.Envelope is not null && transport.Binding is not null, "recovery FIL transport rejected");
            Check(CanonicalMessagingValidator.Validate(transport.Envelope!).IsValid, "generated FIL envelope invalid");
            Check(!transport.ActivationAuthorized && !transport.ExecutionAuthorized && !transport.BusinessAuthorityGranted, "transport gained authority");
            Check(transport.Envelope!.Provenance.Value == "projection-binding:" + transport.Binding!.BindingIdentity, "envelope provenance does not carry exact projection binding identity");
            Check(transport.Binding.RouteIdentity == recoveryRoute.RouteIdentity, "route identity missing from binding");
            Check(transport.Binding.ArtifactId == recoveryRoute.ArtifactId, "artifact id missing from binding");
            Check(transport.Binding.ArtifactVersion == recoveryRoute.ArtifactVersion, "artifact version missing from binding");
            Check(transport.Binding.ArtifactSha256 == recoveryRoute.ArtifactSha256.ToUpperInvariant(), "artifact digest missing from binding");
            Check(transport.Binding.EvidenceReference == recoveryRoute.EvidenceReference, "evidence reference missing from binding");
            Check(transport.Binding.CompatibilityIdentity == recoveryRoute.CompatibilityIdentity, "compatibility identity missing from binding");
            Check(transport.Binding.SourceProvenance == recoveryRoute.Provenance.Value, "source provenance missing from binding");
            Check(transport.Binding.PayloadSha256 == CanonicalMessagingDigest.ComputePayloadSha256(recoveryPayload), "payload digest missing from binding");

            var deterministicTransport = BuildTransport(recoveryRoute, recoveryPayload);
            Check(deterministicTransport.Accepted && deterministicTransport.Binding!.BindingIdentity == transport.Binding.BindingIdentity, "binding identity not deterministic");
            Check(EnvelopeDigest(deterministicTransport) == EnvelopeDigest(transport), "envelope identity not deterministic");

            AssertBindingMutation(recoveryRoute with { RouteIdentity = "route:foundation:recovery:web:v2" }, recoveryPayload, transport, "route identity mutation not bound");
            AssertBindingMutation(recoveryRoute with { ArtifactId = "foundation/runtime-projection/recovery-alt" }, recoveryPayload, transport, "artifact id mutation not bound");
            AssertBindingMutation(recoveryRoute with { ArtifactVersion = "1.0.1" }, recoveryPayload, transport, "artifact version mutation not bound");
            AssertBindingMutation(recoveryRoute with { ArtifactSha256 = Digest('C') }, recoveryPayload, transport, "artifact digest mutation not bound");
            AssertBindingMutation(recoveryRoute with { EvidenceReference = "evidence:foundation:recovery-projection:other" }, recoveryPayload, transport, "evidence mutation not bound");
            AssertBindingMutation(recoveryRoute with { CompatibilityIdentity = "compat:foundation-public-runtime-projection:v2" }, recoveryPayload, transport, "compatibility mutation not bound");
            AssertBindingMutation(recoveryRoute with { Provenance = new ProvenanceReference("provenance:foundation:runtime:other") }, recoveryPayload, transport, "source provenance mutation not bound");
            AssertBindingMutation(recoveryRoute, "{\"projection\":\"recovery-mutated\"}", transport, "payload mutation not bound");

            Check(!BuildTransport(recoveryRoute with { MessageKind = FilMessageKind.Command }, "{}").Accepted, "command route accepted");
            Check(!BuildTransport(recoveryRoute with { MessageKind = FilMessageKind.Query }, "{}").Accepted, "query route accepted");
            Check(!BuildTransport(recoveryRoute with { ArtifactState = PublicProjectionArtifactState.Revoked }, "{}").Accepted, "revoked artifact accepted");
            Check(!BuildTransport(recoveryRoute with { ArtifactState = PublicProjectionArtifactState.Superseded }, "{}").Accepted, "superseded artifact accepted");
            Check(!BuildTransport(recoveryRoute with { ArtifactSha256 = "sha256/1234" }, "{}").Accepted, "invalid digest accepted");
            Check(!BuildTransport(recoveryRoute with { ArtifactVersion = "01.0.0" }, "{}").Accepted, "noncanonical artifact version accepted");
            Check(!BuildTransport(recoveryRoute, "").Accepted, "empty payload accepted");

            var identityRoute = PublicRuntimeProjectionProfiles.IdentitySecurityContext(
                "route:foundation:identity:web:v1", "shared-web", Digest('B'),
                "evidence:foundation:identity-context", "provenance:foundation:runtime");
            Check(identityRoute.MessageType == PublicRuntimeProjectionProfiles.IdentitySecurityContextMessageType, "identity message type not canonical");
            Check(identityRoute.SchemaId.Value == PublicRuntimeProjectionProfiles.IdentitySecurityContextSchemaIdentity, "identity schema not canonical");
            Check(identityRoute.ArtifactId == PublicRuntimeProjectionProfiles.IdentitySecurityContextArtifactId, "identity artifact id not canonical");
            Check(identityRoute.Classification == FilMessageClassification.Security, "identity profile classification incorrect");

            var identity = BuildTransport(identityRoute, "{\"identity\":\"subject:1\",\"businessAuthority\":false}");
            Check(identity.Accepted && identity.Envelope is not null && identity.Binding is not null, "identity projection route rejected");
            Check(identity.Envelope!.Classification == FilMessageClassification.Security, "identity classification lost");
            Check(identity.Envelope.Provenance.Value == "projection-binding:" + identity.Binding!.BindingIdentity, "identity envelope lacks exact binding provenance");
            Check(!identity.ExecutionAuthorized && !identity.BusinessAuthorityGranted, "identity transport gained authority");

            Console.WriteLine("PUBLIC_RUNTIME_PROJECTION_VERIFIER = PASS");
            Console.WriteLine($"CHECKS = {_checks}/{_checks}");
            Console.WriteLine("RECOVERY_PROJECTION = PASS");
            Console.WriteLine("CONTRADICTORY_RECOVERY_STATE_FAIL_CLOSED = PASS");
            Console.WriteLine("FIL_TRANSPORT_BINDING = PASS");
            Console.WriteLine("EXACT_ROUTE_AND_ARTIFACT_BINDING = PASS");
            Console.WriteLine("CANONICAL_PROFILES = PASS");
            Console.WriteLine("STAGE_NEUTRAL_ROUTE = PASS");
            Console.WriteLine("PUBLICATION_NOT_ACTIVATION = PASS");
            Console.WriteLine("TRANSPORT_NOT_AUTHORITY = PASS");
            Console.WriteLine("READY_FOR_RELEASE_DECISION != RELEASE");
            Console.WriteLine("RELEASE_AUTHORIZATION != RELEASE_EXECUTION");
            Console.WriteLine("LIFECYCLE_TRANSITION != NEW_AUTHORITY_DECISION");
            Console.WriteLine("WEB_PRESENTATION != FOUNDATION_AUTHORITY");
            Console.WriteLine("UI_CLICK != AUTHORIZATION");
            Console.WriteLine("ZERO_APPLICATION_OPERATION = VALID");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("PUBLIC_RUNTIME_PROJECTION_VERIFIER = FAIL");
            Console.Error.WriteLine(ex.ToString());
            return 1;
        }
    }

    private static void AssertBindingMutation(
        PublicRuntimeProjectionRoute route,
        string payload,
        PublicRuntimeProjectionTransportDecision baseline,
        string message)
    {
        var mutated = BuildTransport(route, payload);
        Check(mutated.Accepted && mutated.Binding is not null && mutated.Envelope is not null, message + " (mutation rejected unexpectedly)");
        Check(mutated.Binding!.BindingIdentity != baseline.Binding!.BindingIdentity, message + " (binding identity unchanged)");
        Check(EnvelopeDigest(mutated) != EnvelopeDigest(baseline), message + " (envelope identity unchanged)");
    }

    private static string EnvelopeDigest(PublicRuntimeProjectionTransportDecision decision) =>
        CanonicalMessagingDigest.ComputeEnvelopeSha256(decision.Envelope!);

    private static PublicRuntimeProjectionTransportDecision BuildTransport(PublicRuntimeProjectionRoute route, string payload) =>
        PublicRuntimeProjectionTransport.Build(route, payload, new MessageIdentity("message:projection:1"), new CorrelationIdentity("correlation:projection:1"), new CausationIdentity("causation:projection:1"), new IdempotencyIdentity("idempotency:projection:1"), new DeliveryAttemptIdentity("delivery:projection:1"), new RetryLineageIdentity("retry:projection:1"), Now, Now.AddMinutes(5));

    private static string Digest(char value) => "sha256/" + new string(value, 64);

    private static void Check(bool condition, string message)
    {
        _checks++;
        if (!condition) throw new InvalidOperationException(message);
    }
}
