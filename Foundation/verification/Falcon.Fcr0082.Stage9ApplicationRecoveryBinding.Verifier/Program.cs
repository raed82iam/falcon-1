using System;
using System.Security.Cryptography;
using System.Text;
using Foundation.ArtifactPublication;
using Foundation.Contracts;

namespace Falcon.Fcr0082.Stage9ApplicationRecoveryBinding.Verifier;

internal static class Program
{
    private static int _checks;
    private static readonly DateTimeOffset Now = new(2026, 8, 18, 20, 0, 0, TimeSpan.Zero);

    private static int Main()
    {
        try
        {
            var route = PublicRuntimeProjectionProfiles.RecoveryOperationalForFsats();

            Check(PublicRuntimeProjectionProfiles.IsCanonicalRecoveryOperationalForFsats(route), "canonical FSATS recovery route rejected");
            Check(route.RouteIdentity == PublicRuntimeProjectionProfiles.RecoveryApplicationRouteIdentity, "route identity mismatch");
            Check(route.MessageType == PublicRuntimeProjectionProfiles.RecoveryMessageType, "message type mismatch");
            Check(route.SchemaId.Value == PublicRuntimeProjectionProfiles.RecoverySchemaIdentity, "schema identity mismatch");
            Check(route.SchemaVersion == PublicRuntimeProjectionProfiles.ContractVersion, "schema version mismatch");
            Check(route.Producer.Value == PublicRuntimeProjectionProfiles.FoundationProducerIdentity, "producer mismatch");
            Check(route.RecipientScope.Value == PublicRuntimeProjectionProfiles.FsatsRecipientIdentity, "recipient mismatch");
            Check(route.MessageKind == FilMessageKind.Event, "message kind must remain Event");
            Check(route.Classification == FilMessageClassification.Operational, "classification mismatch");
            Check(route.TransportAuthority.Value == PublicRuntimeProjectionProfiles.ProjectionOnlyAuthority, "transport authority mismatch");
            Check(route.Provenance.Value == PublicRuntimeProjectionProfiles.RecoveryApplicationProvenanceReference, "provenance mismatch");
            Check(route.ArtifactId == PublicRuntimeProjectionProfiles.RecoveryArtifactId, "artifact id mismatch");
            Check(route.ArtifactVersion == PublicRuntimeProjectionProfiles.ContractVersion, "artifact version mismatch");
            Check(route.ArtifactSha256 == PublicRuntimeProjectionProfiles.RecoveryApplicationArtifactSha256, "artifact digest mismatch");
            Check(route.EvidenceReference == PublicRuntimeProjectionProfiles.RecoveryApplicationEvidenceReference, "evidence reference mismatch");
            Check(route.CompatibilityIdentity == PublicRuntimeProjectionProfiles.CompatibilityIdentity, "compatibility identity mismatch");
            Check(route.ArtifactState == PublicProjectionArtifactState.Published, "artifact not Published");

            var canonicalDigest = "sha256/" + Convert.ToHexString(
                SHA256.HashData(Encoding.UTF8.GetBytes(PublicRuntimeProjectionProfiles.RecoveryApplicationCanonicalPayload)));
            Check(canonicalDigest == PublicRuntimeProjectionProfiles.RecoveryApplicationArtifactSha256, "canonical payload digest mismatch");

            var truth = new RecoveryOperationalTruth(
                "recovery-case:fcr0082",
                "ReadyForReleaseDecision",
                "Completed",
                true,
                ReleaseAuthorizationProjectionState.NotAuthorized,
                ReleaseExecutionProjectionState.NotExecuted,
                ReintroductionProjectionState.NotStarted,
                "Restricted",
                "evidence:recovery:fcr0082",
                Now,
                Now.AddMinutes(5),
                true);

            var projectionDecision = RecoveryOperationalProjectionRuntime.Build(truth, Now.AddMinutes(1));
            Check(projectionDecision.Accepted && projectionDecision.Projection is not null, "valid recovery projection rejected");
            var projection = projectionDecision.Projection!;
            Check(projection.Freshness == RecoveryProjectionFreshness.Current, "current projection marked stale");
            Check(projection.ReadyForReleaseDecision, "readiness lost");
            Check(projection.ReleaseAuthorization == ReleaseAuthorizationProjectionState.NotAuthorized, "readiness promoted to release authorization");
            Check(projection.ReleaseExecution == ReleaseExecutionProjectionState.NotExecuted, "readiness promoted to release execution");
            Check(projection.Reintroduction == ReintroductionProjectionState.NotStarted, "readiness promoted to lifecycle reintroduction");
            Check(projection.PresentationOnly, "projection must remain presentation/consumption only");
            Check(!projection.CarriesReleaseExecutionAuthority, "projection carries release execution authority");
            Check(!projection.CarriesLifecycleAuthority, "projection carries lifecycle authority");
            Check(!projection.CarriesBusinessAuthority, "projection carries business authority");

            var stale = RecoveryOperationalProjectionRuntime.Build(truth, Now.AddMinutes(10));
            Check(stale.Accepted && stale.Projection is not null && stale.Projection.Freshness == RecoveryProjectionFreshness.Stale, "stale projection not marked stale");
            Check(stale.Projection!.ProjectionIdentity == projection.ProjectionIdentity, "projection identity changed with evaluation time");

            Check(!RecoveryOperationalProjectionRuntime.Build(truth with { ReadyForReleaseDecision = false }, Now).Accepted, "contradictory readiness accepted");
            Check(!RecoveryOperationalProjectionRuntime.Build(truth with { RecoveryState = "ReleaseAuthorized" }, Now).Accepted, "authorization inferred from state without exact authorization projection");
            Check(!RecoveryOperationalProjectionRuntime.Build(truth with { RecoveryState = "RecoveryComplete", ReleaseAuthorization = ReleaseAuthorizationProjectionState.Authorized, ReleaseExecution = ReleaseExecutionProjectionState.Executed, Reintroduction = ReintroductionProjectionState.Restricted }, Now).Accepted, "recovery complete accepted with restricted reintroduction");

            const string payload = "{\"recoveryCaseIdentity\":\"recovery-case:fcr0082\",\"recoveryState\":\"ReadyForReleaseDecision\",\"readyForReleaseDecision\":true,\"releaseAuthorization\":\"NotAuthorized\",\"releaseExecution\":\"NotExecuted\",\"reintroduction\":\"NotStarted\",\"presentationOnly\":true,\"carriesBusinessAuthority\":false}";
            var transport = PublicRuntimeProjectionTransport.Build(
                route,
                payload,
                new MessageIdentity("message:fcr0082:1"),
                new CorrelationIdentity("correlation:fcr0082:1"),
                new CausationIdentity("causation:fcr0082:stage9"),
                new IdempotencyIdentity("idempotency:fcr0082:1"),
                new DeliveryAttemptIdentity("delivery:fcr0082:1"),
                new RetryLineageIdentity("retry:fcr0082:1"),
                Now,
                Now.AddMinutes(5));

            Check(transport.Accepted && transport.Envelope is not null && transport.Binding is not null, "canonical recovery transport rejected");
            Check(CanonicalMessagingValidator.Validate(transport.Envelope!).IsValid, "generated FIL envelope invalid");
            Check(!transport.ActivationAuthorized, "projection transport authorized activation");
            Check(!transport.ExecutionAuthorized, "projection transport authorized execution");
            Check(!transport.BusinessAuthorityGranted, "projection transport granted business authority");
            Check(transport.Binding!.RouteIdentity == PublicRuntimeProjectionProfiles.RecoveryApplicationRouteIdentity, "binding route mismatch");
            Check(transport.Binding.ArtifactId == PublicRuntimeProjectionProfiles.RecoveryArtifactId, "binding artifact id mismatch");
            Check(transport.Binding.ArtifactVersion == PublicRuntimeProjectionProfiles.ContractVersion, "binding artifact version mismatch");
            Check(transport.Binding.ArtifactSha256 == PublicRuntimeProjectionProfiles.RecoveryApplicationArtifactSha256.ToUpperInvariant(), "binding digest mismatch");
            Check(transport.Binding.EvidenceReference == PublicRuntimeProjectionProfiles.RecoveryApplicationEvidenceReference, "binding evidence mismatch");
            Check(transport.Binding.CompatibilityIdentity == PublicRuntimeProjectionProfiles.CompatibilityIdentity, "binding compatibility mismatch");
            Check(transport.Binding.SourceProvenance == PublicRuntimeProjectionProfiles.RecoveryApplicationProvenanceReference, "binding provenance mismatch");

            Check(!PublicRuntimeProjectionProfiles.IsCanonicalRecoveryOperationalForFsats(route with { RouteIdentity = "route:foundation:recovery:application:v2" }), "mutated route accepted as canonical");
            Check(!PublicRuntimeProjectionProfiles.IsCanonicalRecoveryOperationalForFsats(route with { RecipientScope = new RecipientScopeReference("other-application") }), "mutated recipient accepted as canonical");
            Check(!PublicRuntimeProjectionProfiles.IsCanonicalRecoveryOperationalForFsats(route with { ArtifactSha256 = "sha256/AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA" }), "mutated digest accepted as canonical");
            Check(!PublicRuntimeProjectionProfiles.IsCanonicalRecoveryOperationalForFsats(route with { EvidenceReference = "evidence:other" }), "mutated evidence accepted as canonical");
            Check(!PublicRuntimeProjectionProfiles.IsCanonicalRecoveryOperationalForFsats(route with { Provenance = new ProvenanceReference("commit/other") }), "mutated provenance accepted as canonical");
            Check(!PublicRuntimeProjectionProfiles.IsCanonicalRecoveryOperationalForFsats(route with { MessageKind = FilMessageKind.Command }), "command mutation accepted as canonical");
            Check(!PublicRuntimeProjectionProfiles.IsCanonicalRecoveryOperationalForFsats(route with { TransportAuthority = new AuthorityReference("authority:business") }), "authority mutation accepted as canonical");
            Check(!PublicRuntimeProjectionProfiles.IsCanonicalRecoveryOperationalForFsats(route with { ArtifactState = PublicProjectionArtifactState.Superseded }), "superseded artifact accepted as canonical");
            Check(!PublicRuntimeProjectionProfiles.IsCanonicalRecoveryOperationalForFsats(null), "null profile accepted as canonical");

            Console.WriteLine("FCR0082_STAGE9_APPLICATION_RUNTIME_BINDING_VERIFIER = PASS");
            Console.WriteLine($"CHECKS = {_checks}/{_checks}");
            Console.WriteLine("FOUNDATION_STAGE9_PUBLIC_BINDING = CANONICAL_RECOVERY_PROJECTION_PROFILE");
            Console.WriteLine("APPLICATION_RECIPIENT = fsats");
            Console.WriteLine("TRANSPORT = FIL_EVENT_PROJECTION_ONLY");
            Console.WriteLine("LIVE_ROUTE_ACTIVATION = NOT_GRANTED");
            Console.WriteLine("READY_FOR_RELEASE_DECISION != RELEASE_AUTHORIZATION");
            Console.WriteLine("RELEASE_AUTHORIZATION != RELEASE_EXECUTION");
            Console.WriteLine("LIFECYCLE_TRANSITION != NEW_AUTHORITY_DECISION");
            Console.WriteLine("APPLICATION_BUSINESS_RECOVERY = APPLICATION_OWNED");
            Console.WriteLine("STAGE13_FSA_CONTROLLED_REVIVAL != STAGE9_GENERIC_RECOVERY");
            Console.WriteLine("TECHNICAL_CONSUMPTION != RUNTIME_AUTHORITY");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("FCR0082_STAGE9_APPLICATION_RUNTIME_BINDING_VERIFIER = FAIL");
            Console.Error.WriteLine(ex.ToString());
            return 1;
        }
    }

    private static void Check(bool condition, string message)
    {
        _checks++;
        if (!condition)
            throw new InvalidOperationException(message);
    }
}
