using System;
using Foundation.ArtifactPublication;
using Foundation.Contracts;

namespace Falcon.Fcr0239.OperationalProjectionProfile.Verifier;

internal static class Program
{
    private static int _checks;
    private static readonly DateTimeOffset Now = new(2026, 8, 18, 0, 0, 0, TimeSpan.Zero);

    private static int Main()
    {
        try
        {
            var truth = new FoundationOperationalTruth(
                "foundation:falcon",
                "ACCEPTED_AND_CLOSED_THROUGH_STAGE16",
                "HEALTHY",
                "GOVERNED",
                "ACTIVE_SCOPED",
                0,
                "evidence:foundation:operational:stage14",
                Now);

            var built = ArtifactPublicationRuntime.BuildOperationalProjection(truth);
            Check(built.Accepted && built.Projection is not null, "valid Stage 14 operational truth rejected");
            Check(built.Projection!.ApplicationCount == 0, "zero-Application state not preserved");
            Check(built.Projection.PresentationOnly, "operational projection is not presentation-only");
            Check(!built.Projection.CarriesExecutionAuthority, "operational projection carries execution authority");
            Check(!built.Projection.CarriesBusinessAuthority, "operational projection carries business authority");
            Check(built.Projection.ObservedAt == Now, "operational observed-at truth changed");
            Check(built.Projection.EvidenceReference == truth.EvidenceReference, "operational evidence reference changed");

            Check(!ArtifactPublicationRuntime.BuildOperationalProjection(truth with { FoundationIdentity = "" }).Accepted, "missing Foundation identity accepted");
            Check(!ArtifactPublicationRuntime.BuildOperationalProjection(truth with { FoundationReleaseState = "" }).Accepted, "missing release state accepted");
            Check(!ArtifactPublicationRuntime.BuildOperationalProjection(truth with { HealthState = "" }).Accepted, "missing health state accepted");
            Check(!ArtifactPublicationRuntime.BuildOperationalProjection(truth with { AuthorityState = "" }).Accepted, "missing authority state accepted");
            Check(!ArtifactPublicationRuntime.BuildOperationalProjection(truth with { LifecycleState = "" }).Accepted, "missing lifecycle state accepted");
            Check(!ArtifactPublicationRuntime.BuildOperationalProjection(truth with { EvidenceReference = "" }).Accepted, "missing evidence accepted");
            Check(!ArtifactPublicationRuntime.BuildOperationalProjection(truth with { ApplicationCount = -1 }).Accepted, "negative application count accepted");
            Check(!ArtifactPublicationRuntime.BuildOperationalProjection(truth with { ObservedAt = default }).Accepted, "missing observation time accepted");

            var route = PublicRuntimeProjectionProfiles.FoundationOperationalForSharedWeb(
                Digest('D'),
                "evidence:foundation:stage14:operational-projection",
                "provenance:foundation:runtime:stage14-operational");

            Check(route.RouteIdentity == PublicRuntimeProjectionProfiles.FoundationOperationalSharedWebRouteIdentity, "operational route identity not canonical");
            Check(route.MessageType == PublicRuntimeProjectionProfiles.FoundationOperationalMessageType, "operational message type not canonical");
            Check(route.SchemaId.Value == PublicRuntimeProjectionProfiles.FoundationOperationalSchemaIdentity, "operational schema not canonical");
            Check(route.SchemaVersion == PublicRuntimeProjectionProfiles.ContractVersion, "operational schema version not canonical");
            Check(route.Producer.Value == PublicRuntimeProjectionProfiles.FoundationProducerIdentity, "operational producer not canonical");
            Check(route.RecipientScope.Value == PublicRuntimeProjectionProfiles.SharedWebRecipientIdentity, "operational recipient not canonical");
            Check(route.MessageKind == FilMessageKind.Event, "operational route not Event");
            Check(route.Classification == FilMessageClassification.Operational, "operational classification incorrect");
            Check(route.TransportAuthority.Value == PublicRuntimeProjectionProfiles.ProjectionOnlyAuthority, "operational transport authority incorrect");
            Check(route.ArtifactId == PublicRuntimeProjectionProfiles.FoundationOperationalArtifactId, "operational artifact id not canonical");
            Check(route.ArtifactVersion == PublicRuntimeProjectionProfiles.ContractVersion, "operational artifact version not canonical");
            Check(route.CompatibilityIdentity == PublicRuntimeProjectionProfiles.CompatibilityIdentity, "operational compatibility identity not canonical");
            Check(route.ArtifactState == PublicProjectionArtifactState.Published, "operational artifact not published");

            const string payload = "{\"foundationIdentity\":\"foundation:falcon\",\"applicationCount\":0,\"presentationOnly\":true,\"carriesExecutionAuthority\":false,\"carriesBusinessAuthority\":false}";
            var transport = BuildTransport(route, payload);
            Check(transport.Accepted && transport.Envelope is not null && transport.Binding is not null, "operational FIL transport rejected");
            Check(CanonicalMessagingValidator.Validate(transport.Envelope!).IsValid, "operational FIL envelope invalid");
            Check(!transport.ActivationAuthorized, "transport grants activation authority");
            Check(!transport.ExecutionAuthorized, "transport grants execution authority");
            Check(!transport.BusinessAuthorityGranted, "transport grants business authority");
            Check(transport.Envelope!.Provenance.Value == "projection-binding:" + transport.Binding!.BindingIdentity, "operational envelope provenance not exact binding identity");
            Check(transport.Binding.RouteIdentity == route.RouteIdentity, "route identity missing from binding");
            Check(transport.Binding.ArtifactId == route.ArtifactId, "artifact id missing from binding");
            Check(transport.Binding.ArtifactSha256 == route.ArtifactSha256.ToUpperInvariant(), "artifact digest missing from binding");
            Check(transport.Binding.EvidenceReference == route.EvidenceReference, "evidence reference missing from binding");
            Check(transport.Binding.CompatibilityIdentity == route.CompatibilityIdentity, "compatibility identity missing from binding");
            Check(transport.Binding.SourceProvenance == route.Provenance.Value, "source provenance missing from binding");
            Check(transport.Binding.PayloadSha256 == CanonicalMessagingDigest.ComputePayloadSha256(payload), "payload digest missing from binding");

            var rerun = BuildTransport(route, payload);
            Check(rerun.Accepted && rerun.Binding!.BindingIdentity == transport.Binding.BindingIdentity, "operational binding not deterministic");
            Check(CanonicalMessagingDigest.ComputeEnvelopeSha256(rerun.Envelope!) == CanonicalMessagingDigest.ComputeEnvelopeSha256(transport.Envelope!), "operational FIL envelope not deterministic");

            Check(!BuildTransport(route with { RouteIdentity = "route:web:invented:operational" }, payload).Binding!.BindingIdentity.Equals(transport.Binding.BindingIdentity, StringComparison.Ordinal), "route mutation not bound");
            Check(!BuildTransport(route with { ArtifactId = "foundation/runtime-projection/invented" }, payload).Binding!.BindingIdentity.Equals(transport.Binding.BindingIdentity, StringComparison.Ordinal), "artifact mutation not bound");
            Check(!BuildTransport(route with { EvidenceReference = "evidence:other" }, payload).Binding!.BindingIdentity.Equals(transport.Binding.BindingIdentity, StringComparison.Ordinal), "evidence mutation not bound");
            Check(!BuildTransport(route with { Provenance = new ProvenanceReference("provenance:other") }, payload).Binding!.BindingIdentity.Equals(transport.Binding.BindingIdentity, StringComparison.Ordinal), "provenance mutation not bound");
            Check(!BuildTransport(route, payload + " ").Binding!.BindingIdentity.Equals(transport.Binding.BindingIdentity, StringComparison.Ordinal), "payload mutation not bound");

            Check(!BuildTransport(route with { ArtifactState = PublicProjectionArtifactState.Revoked }, payload).Accepted, "revoked operational artifact accepted");
            Check(!BuildTransport(route with { ArtifactState = PublicProjectionArtifactState.Superseded }, payload).Accepted, "superseded operational artifact accepted");
            Check(!BuildTransport(route with { MessageKind = FilMessageKind.Command }, payload).Accepted, "operational Command accepted");
            Check(!BuildTransport(route with { MessageKind = FilMessageKind.Query }, payload).Accepted, "operational Query accepted");
            Check(!BuildTransport(route with { ArtifactSha256 = "sha256/1234" }, payload).Accepted, "invalid operational artifact digest accepted");
            Check(!BuildTransport(route, "").Accepted, "empty operational payload accepted");

            Console.WriteLine("FCR0239_OPERATIONAL_PROJECTION_PROFILE_VERIFIER = PASS");
            Console.WriteLine($"CHECKS = {_checks}/{_checks}");
            Console.WriteLine($"ROUTE = {PublicRuntimeProjectionProfiles.FoundationOperationalSharedWebRouteIdentity}");
            Console.WriteLine($"MESSAGE_TYPE = {PublicRuntimeProjectionProfiles.FoundationOperationalMessageType}");
            Console.WriteLine($"SCHEMA = {PublicRuntimeProjectionProfiles.FoundationOperationalSchemaIdentity}");
            Console.WriteLine($"ARTIFACT = {PublicRuntimeProjectionProfiles.FoundationOperationalArtifactId}");
            Console.WriteLine("ZERO_APPLICATION_OPERATION = VALID");
            Console.WriteLine("NO_SOURCE_VALUE != ZERO");
            Console.WriteLine("WEB_PRESENTATION != FOUNDATION_AUTHORITY");
            Console.WriteLine("PROJECTION_PRESENT != SYSTEM_ACTION_AUTHORIZED");
            Console.WriteLine("PUBLICATION != ACTIVATION");
            Console.WriteLine("FIL_ENVELOPE_AVAILABLE != LIVE_SERVICE_BUS_ROUTE_ACTIVATED");
            Console.WriteLine("PLUG_AND_PLAY != IMPLICIT_TRUST");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("FCR0239_OPERATIONAL_PROJECTION_PROFILE_VERIFIER = FAIL");
            Console.Error.WriteLine(ex.ToString());
            return 1;
        }
    }

    private static PublicRuntimeProjectionTransportDecision BuildTransport(PublicRuntimeProjectionRoute route, string payload) =>
        PublicRuntimeProjectionTransport.Build(
            route,
            payload,
            new MessageIdentity("message:foundation-operational:1"),
            new CorrelationIdentity("correlation:foundation-operational:1"),
            new CausationIdentity("causation:foundation-operational:1"),
            new IdempotencyIdentity("idempotency:foundation-operational:1"),
            new DeliveryAttemptIdentity("delivery:foundation-operational:1"),
            new RetryLineageIdentity("retry:foundation-operational:1"),
            Now,
            Now.AddMinutes(5));

    private static string Digest(char value) => "sha256/" + new string(value, 64);

    private static void Check(bool condition, string message)
    {
        _checks++;
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
}
