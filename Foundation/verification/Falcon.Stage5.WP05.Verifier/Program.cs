using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Foundation.ApplicationManifest;
using Foundation.Authority;
using Foundation.Contracts;
using Foundation.MessageAdmission;
using Foundation.MessageRouting;
using Foundation.SchemaRegistry;

namespace Falcon.Stage5.WP05.Verifier;

internal static class Program
{
    private const string MessageType = "falcon.reference.operation.v1";
    private const string OtherMessageType = "falcon.reference.other.v1";
    private const string SchemaId = "schema:falcon.reference.operation";
    private const string AdmissionAuthorityRef = "authority:message-admission/reference";
    private const string RouteAuthorityRef = "authority:route/reference";
    private const string AdmissionEffectiveScope = "scope:message-admission/reference";
    private const string RouteEffectiveScope = "scope:route/reference";
    private const string Purpose = "purpose:governed-route/reference";
    private const string Producer = "application-neutral-producer/reference";
    private const string Recipient = "application-neutral-recipient/reference";
    private const string Consumer = "consumer:reference";
    private static readonly DateTimeOffset AdmissionObservation = new(2026, 8, 7, 18, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset RoutingObservation = new(2026, 8, 7, 18, 1, 0, TimeSpan.Zero);
    private static readonly string SchemaDigest = new('A', 64);

    private static int Main()
    {
        var scenarios = new (string Name, Action Test)[]
        {
            ("single_governed_route_selected", SingleGovernedRouteSelected),
            ("two_independent_applications_route_independently", TwoApplicationsRouteIndependently),
            ("null_context_rejected", NullContextRejected),
            ("rejected_admission_cannot_route", RejectedAdmissionCannotRoute),
            ("expired_admission_cannot_route", ExpiredAdmissionCannotRoute),
            ("missing_message_type_binding_rejected", MissingMessageTypeBindingRejected),
            ("message_type_binding_identity_mismatch_rejected", MessageTypeBindingIdentityMismatchRejected),
            ("empty_registry_fails_closed", EmptyRegistryFailsClosed),
            ("unknown_manifest_route_registration_rejected", UnknownManifestRegistrationRejected),
            ("manifest_digest_mismatch_route_registration_rejected", ManifestDigestMismatchRegistrationRejected),
            ("manifest_application_mismatch_route_registration_rejected", ManifestApplicationMismatchRegistrationRejected),
            ("manifest_consumer_undeclared_route_registration_rejected", ManifestConsumerUndeclaredRegistrationRejected),
            ("manifest_communication_undeclared_route_registration_rejected", ManifestCommunicationUndeclaredRegistrationRejected),
            ("manifest_communication_invalid_route_registration_rejected", ManifestCommunicationInvalidRegistrationRejected),
            ("route_authority_malformed_registration_rejected", RouteAuthorityMalformedRegistrationRejected),
            ("route_authority_binding_mismatch_registration_rejected", RouteAuthorityBindingMismatchRegistrationRejected),
            ("route_authority_denied_registration_rejected", RouteAuthorityDeniedRegistrationRejected),
            ("source_binding_mismatch_rejected", SourceBindingMismatchRejected),
            ("destination_binding_mismatch_rejected", DestinationBindingMismatchRejected),
            ("consumer_binding_mismatch_rejected", ConsumerBindingMismatchRejected),
            ("message_type_mismatch_rejected", MessageTypeMismatchRejected),
            ("route_purpose_mismatch_rejected", RoutePurposeMismatchRejected),
            ("admission_manifest_binding_mismatch_rejected", AdmissionManifestBindingMismatchRejected),
            ("isolated_route_rejected", IsolatedRouteRejected),
            ("unavailable_route_rejected", UnavailableRouteRejected),
            ("future_route_authority_rejected", FutureRouteAuthorityRejected),
            ("expired_route_authority_rejected", ExpiredRouteAuthorityRejected),
            ("source_endpoint_isolation_rejected", SourceEndpointIsolationRejected),
            ("destination_endpoint_isolation_rejected", DestinationEndpointIsolationRejected),
            ("unknown_endpoint_state_fails_closed", UnknownEndpointStateRejected),
            ("isolated_route_does_not_poison_eligible_route", IsolatedRouteDoesNotPoisonEligibleRoute),
            ("isolated_endpoint_does_not_poison_other_endpoint", IsolatedEndpointDoesNotPoisonOtherEndpoint),
            ("multiple_eligible_routes_fail_ambiguous", MultipleEligibleRoutesFailAmbiguous),
            ("duplicate_route_identity_rejected", DuplicateRouteIdentityRejected),
            ("equivalent_inputs_same_decision_identity", EquivalentInputsSameDecisionIdentity),
            ("route_evidence_mutation_changes_identity", RouteEvidenceMutationChangesIdentity),
            ("route_authority_evidence_mutation_changes_identity", RouteAuthorityEvidenceMutationChangesIdentity),
            ("message_type_binding_evidence_mutation_changes_identity", BindingEvidenceMutationChangesIdentity),
            ("endpoint_state_evidence_mutation_changes_identity", EndpointEvidenceMutationChangesIdentity),
            ("observation_time_mutation_changes_identity", ObservationMutationChangesIdentity),
            ("registry_mutation_changes_rejection_identity", RegistryMutationChangesRejectionIdentity),
            ("registry_order_does_not_change_selected_identity", RegistryOrderNeutrality),
            ("route_decision_surface_is_immutable", DecisionSurfaceImmutable),
            ("decision_and_registry_identities_are_sha256", DecisionAndRegistryIdentitySha256),
            ("routing_surface_has_no_wp06_plus_operations", NoLaterWpOperations),
            ("routing_does_not_dispatch", RoutingDoesNotDispatch),
            ("routing_does_not_deliver", RoutingDoesNotDeliver),
            ("routing_does_not_retry", RoutingDoesNotRetry),
            ("payload_business_semantics_remain_opaque", PayloadRemainsOpaque),
            ("fsats_receives_no_special_treatment", FsatsNoSpecialTreatment),
            ("zero_application_foundation_remains_valid", ZeroApplicationFoundationValid)
        };

        var failures = new List<string>();
        foreach (var scenario in scenarios)
        {
            try
            {
                scenario.Test();
                Console.WriteLine($"PASS {scenario.Name}");
            }
            catch (Exception exception)
            {
                failures.Add($"{scenario.Name}: {exception.GetType().Name}: {exception.Message}");
                Console.WriteLine($"FAIL {scenario.Name}: {exception.Message}");
            }
        }

        Console.WriteLine($"RESULT {scenarios.Length - failures.Count}/{scenarios.Length} PASS");
        Console.WriteLine(failures.Count == 0
            ? "STAGE 5 WP-05 SERVICE BUS DYNAMIC ROUTING AND ISOLATION VERIFIER: PASS"
            : "STAGE 5 WP-05 SERVICE BUS DYNAMIC ROUTING AND ISOLATION VERIFIER: FAIL");
        foreach (var failure in failures) Console.Error.WriteLine($"DETAIL {failure}");
        return failures.Count == 0 ? 0 : 1;
    }

    private static void SingleGovernedRouteSelected() => AssertSelected(Evaluate(RoutingFixture()));

    private static void TwoApplicationsRouteIndependently()
    {
        var alpha = RoutingFixture("application.alpha", "manifest:alpha", "producer:alpha", "recipient:alpha", "consumer:alpha", "route:alpha");
        var beta = RoutingFixture("application.beta", "manifest:beta", "producer:beta", "recipient:beta", "consumer:beta", "route:beta");
        var a = Evaluate(alpha);
        var b = Evaluate(beta);
        AssertSelected(a); AssertSelected(b);
        Assert(a.DecisionId != b.DecisionId, "independent_applications_collapsed");
    }

    private static void NullContextRejected()
    {
        var fixture = CreateAdmission();
        AssertRejected(new RouteSelectionEvaluator().Evaluate(null, new RouteRegistry(fixture.ManifestRegistry)), RouteSelectionReason.InvalidContext);
    }

    private static void RejectedAdmissionCannotRoute()
    {
        var fixture = CreateAdmission(admissionAuthorityDecision: AuthorityDecision.Deny);
        var registry = new RouteRegistry(fixture.ManifestRegistry);
        Assert(registry.Register(Route(fixture)).Accepted, "governed_route_registration_failed");
        AssertRejected(new RouteSelectionEvaluator().Evaluate(Context(fixture.Result), registry), RouteSelectionReason.MessageNotAdmitted);
    }

    private static void ExpiredAdmissionCannotRoute()
    {
        var fixture = CreateAdmission(messageExpiry: AdmissionObservation.AddMinutes(2));
        AssertRejected(new RouteSelectionEvaluator().Evaluate(Context(fixture.Result, observation: AdmissionObservation.AddMinutes(2)), Registry(fixture, Route(fixture))), RouteSelectionReason.AdmissionExpiredForRouting);
    }

    private static void MissingMessageTypeBindingRejected()
    {
        var fixture = CreateAdmission();
        var context = new RouteSelectionContext(fixture.Result, null, Purpose, RoutingObservation, null, Evidence("routing/decision"));
        AssertRejected(new RouteSelectionEvaluator().Evaluate(context, Registry(fixture, Route(fixture))), RouteSelectionReason.MessageTypeBindingMissing);
    }

    private static void MessageTypeBindingIdentityMismatchRejected()
    {
        var fixture = CreateAdmission();
        AssertRejected(new RouteSelectionEvaluator().Evaluate(Context(fixture.Result, bindingDecisionId: "message-admission/sha256/FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF"), Registry(fixture, Route(fixture))), RouteSelectionReason.MessageTypeBindingMismatch);
    }

    private static void EmptyRegistryFailsClosed()
    {
        var fixture = CreateAdmission();
        AssertRejected(new RouteSelectionEvaluator().Evaluate(Context(fixture.Result), new RouteRegistry(fixture.ManifestRegistry)), RouteSelectionReason.NoDeclaredRoute);
    }

    private static void UnknownManifestRegistrationRejected()
    {
        var fixture = CreateAdmission();
        var registry = new RouteRegistry(fixture.ManifestRegistry);
        var route = Route(fixture, manifestId: "manifest:unknown", manifestDigest: new string('B', 64));
        AssertRegistrationRejected(registry.Register(route), RouteRegistrationReason.ManifestUnknown);
    }

    private static void ManifestDigestMismatchRegistrationRejected()
    {
        var fixture = CreateAdmission();
        AssertRegistrationRejected(new RouteRegistry(fixture.ManifestRegistry).Register(Route(fixture, manifestDigest: new string('B', 64))), RouteRegistrationReason.ManifestDigestMismatch);
    }

    private static void ManifestApplicationMismatchRegistrationRejected()
    {
        var fixture = CreateAdmission();
        AssertRegistrationRejected(new RouteRegistry(fixture.ManifestRegistry).Register(Route(fixture, applicationId: "application.other")), RouteRegistrationReason.ManifestApplicationMismatch);
    }

    private static void ManifestConsumerUndeclaredRegistrationRejected()
    {
        var fixture = CreateAdmission();
        AssertRegistrationRejected(new RouteRegistry(fixture.ManifestRegistry).Register(Route(fixture, consumer: "consumer:other")), RouteRegistrationReason.ManifestConsumerUndeclared);
    }

    private static void ManifestCommunicationUndeclaredRegistrationRejected()
    {
        var fixture = CreateAdmission();
        AssertRegistrationRejected(new RouteRegistry(fixture.ManifestRegistry).Register(Route(fixture, messageType: OtherMessageType)), RouteRegistrationReason.ManifestCommunicationUndeclared);
    }

    private static void ManifestCommunicationInvalidRegistrationRejected()
    {
        var fixture = CreateAdmission();
        var alternate = RegisterAlternateManifest(fixture, "manifest:inbound", CommunicationDirection.Inbound, CommunicationRole.Consumer, MessageType);
        var route = Route(fixture, manifestId: alternate.ManifestId, manifestDigest: alternate.Digest);
        AssertRegistrationRejected(new RouteRegistry(fixture.ManifestRegistry).Register(route), RouteRegistrationReason.ManifestCommunicationInvalid);
    }

    private static void RouteAuthorityMalformedRegistrationRejected()
    {
        var fixture = CreateAdmission();
        var malformed = new AuthorityResult("request:route/1", "decision:route/1", AuthorityDecision.Allow, RouteEffectiveScope, "", "1.0",
            "conditions:reference", "bounded", AuthorityReason.Allowed, RoutingObservation.AddMinutes(-2), RoutingObservation.AddMinutes(20), "evidence:route-authority/result");
        var route = Route(fixture, authorityResult: malformed);
        AssertRegistrationRejected(new RouteRegistry(fixture.ManifestRegistry).Register(route), RouteRegistrationReason.AuthorityMalformed);
    }

    private static void RouteAuthorityBindingMismatchRegistrationRejected()
    {
        var fixture = CreateAdmission();
        var route = Route(fixture, authorizedPurpose: "purpose:other");
        AssertRegistrationRejected(new RouteRegistry(fixture.ManifestRegistry).Register(route), RouteRegistrationReason.AuthorityBindingMismatch);
    }

    private static void RouteAuthorityDeniedRegistrationRejected()
    {
        var fixture = CreateAdmission();
        var route = Route(fixture, routeAuthorityDecision: AuthorityDecision.Deny, routeAuthorityEffectiveScope: "NONE");
        AssertRegistrationRejected(new RouteRegistry(fixture.ManifestRegistry).Register(route), RouteRegistrationReason.AuthorityDenied);
    }

    private static void SourceBindingMismatchRejected()
    {
        var fixture = CreateAdmission();
        var alternate = CreateAdmission("application.other", "manifest:other", "producer:other", fixture.Recipient, fixture.Consumer);
        AssertRejected(new RouteSelectionEvaluator().Evaluate(Context(fixture.Result), Registry(alternate, Route(alternate))), RouteSelectionReason.SourceBindingMismatch);
    }

    private static void DestinationBindingMismatchRejected()
    {
        var fixture = CreateAdmission();
        var alternate = CreateAdmission(fixture.ApplicationId, "manifest:alternate-recipient", fixture.Producer, "recipient:other", fixture.Consumer);
        AssertRejected(new RouteSelectionEvaluator().Evaluate(Context(fixture.Result), Registry(alternate, Route(alternate))), RouteSelectionReason.DestinationBindingMismatch);
    }

    private static void ConsumerBindingMismatchRejected()
    {
        var fixture = CreateAdmission();
        var alternate = CreateAdmission(fixture.ApplicationId, "manifest:alternate-consumer", fixture.Producer, fixture.Recipient, "consumer:other");
        AssertRejected(new RouteSelectionEvaluator().Evaluate(Context(fixture.Result), Registry(alternate, Route(alternate))), RouteSelectionReason.ConsumerBindingMismatch);
    }

    private static void MessageTypeMismatchRejected()
    {
        var fixture = CreateAdmission();
        AssertRejected(new RouteSelectionEvaluator().Evaluate(Context(fixture.Result, messageType: OtherMessageType), Registry(fixture, Route(fixture))), RouteSelectionReason.MessageTypeMismatch);
    }

    private static void RoutePurposeMismatchRejected()
    {
        var fixture = CreateAdmission();
        AssertRejected(new RouteSelectionEvaluator().Evaluate(Context(fixture.Result, purpose: "purpose:other"), Registry(fixture, Route(fixture))), RouteSelectionReason.RoutePurposeMismatch);
    }

    private static void AdmissionManifestBindingMismatchRejected()
    {
        var fixture = CreateAdmission();
        var alternate = RegisterAlternateManifest(fixture, "manifest:alternate", CommunicationDirection.Outbound, CommunicationRole.Producer, MessageType);
        var route = Route(fixture, manifestId: alternate.ManifestId, manifestDigest: alternate.Digest);
        AssertRejected(new RouteSelectionEvaluator().Evaluate(Context(fixture.Result), Registry(fixture, route)), RouteSelectionReason.ManifestBindingMismatch);
    }

    private static void IsolatedRouteRejected()
    {
        var fixture = CreateAdmission();
        AssertRejected(new RouteSelectionEvaluator().Evaluate(Context(fixture.Result), Registry(fixture, Route(fixture, state: RouteState.Isolated))), RouteSelectionReason.RouteIsolated);
    }

    private static void UnavailableRouteRejected()
    {
        var fixture = CreateAdmission();
        AssertRejected(new RouteSelectionEvaluator().Evaluate(Context(fixture.Result), Registry(fixture, Route(fixture, state: RouteState.Unavailable))), RouteSelectionReason.RouteUnavailable);
    }

    private static void FutureRouteAuthorityRejected()
    {
        var fixture = CreateAdmission();
        var route = Route(fixture, routeAuthorityDecisionTime: RoutingObservation.AddMinutes(1), routeAuthorityExpiry: RoutingObservation.AddMinutes(20));
        AssertRejected(new RouteSelectionEvaluator().Evaluate(Context(fixture.Result), Registry(fixture, route)), RouteSelectionReason.RouteAuthorityNotYetEffective);
    }

    private static void ExpiredRouteAuthorityRejected()
    {
        var fixture = CreateAdmission();
        var route = Route(fixture, routeAuthorityDecisionTime: RoutingObservation.AddMinutes(-10), routeAuthorityExpiry: RoutingObservation);
        AssertRejected(new RouteSelectionEvaluator().Evaluate(Context(fixture.Result), Registry(fixture, route)), RouteSelectionReason.RouteAuthorityExpired);
    }

    private static void SourceEndpointIsolationRejected()
    {
        var fixture = CreateAdmission();
        var route = Route(fixture);
        var states = new[] { Endpoint(route.SourceEndpoint.Value, RouteEndpointState.Isolated), Endpoint(route.DestinationEndpoint.Value, RouteEndpointState.Eligible) };
        AssertRejected(new RouteSelectionEvaluator().Evaluate(Context(fixture.Result, endpointStates: states), Registry(fixture, route)), RouteSelectionReason.SourceEndpointIneligible);
    }

    private static void DestinationEndpointIsolationRejected()
    {
        var fixture = CreateAdmission();
        var route = Route(fixture);
        var states = new[] { Endpoint(route.SourceEndpoint.Value, RouteEndpointState.Eligible), Endpoint(route.DestinationEndpoint.Value, RouteEndpointState.Isolated) };
        AssertRejected(new RouteSelectionEvaluator().Evaluate(Context(fixture.Result, endpointStates: states), Registry(fixture, route)), RouteSelectionReason.DestinationEndpointIneligible);
    }

    private static void UnknownEndpointStateRejected()
    {
        var fixture = CreateAdmission();
        var route = Route(fixture);
        var states = new[] { Endpoint(route.DestinationEndpoint.Value, RouteEndpointState.Eligible) };
        AssertRejected(new RouteSelectionEvaluator().Evaluate(Context(fixture.Result, endpointStates: states), Registry(fixture, route)), RouteSelectionReason.SourceEndpointIneligible);
    }

    private static void IsolatedRouteDoesNotPoisonEligibleRoute()
    {
        var fixture = CreateAdmission();
        var blocked = Route(fixture, routeId: "route:isolated", state: RouteState.Isolated);
        var eligible = Route(fixture, routeId: "route:eligible", sourceEndpoint: "endpoint:source/eligible", destinationEndpoint: "endpoint:destination/eligible");
        var decision = new RouteSelectionEvaluator().Evaluate(Context(fixture.Result), Registry(fixture, blocked, eligible));
        AssertSelected(decision); AssertEqual("route:eligible", decision.RouteId, "wrong_route_selected");
    }

    private static void IsolatedEndpointDoesNotPoisonOtherEndpoint()
    {
        var fixture = CreateAdmission();
        var blocked = Route(fixture, routeId: "route:blocked", sourceEndpoint: "endpoint:source/blocked", destinationEndpoint: "endpoint:destination/blocked");
        var eligible = Route(fixture, routeId: "route:eligible", sourceEndpoint: "endpoint:source/eligible", destinationEndpoint: "endpoint:destination/eligible");
        var states = new[]
        {
            Endpoint("endpoint:source/blocked", RouteEndpointState.Eligible), Endpoint("endpoint:destination/blocked", RouteEndpointState.Isolated),
            Endpoint("endpoint:source/eligible", RouteEndpointState.Eligible), Endpoint("endpoint:destination/eligible", RouteEndpointState.Eligible)
        };
        var decision = new RouteSelectionEvaluator().Evaluate(Context(fixture.Result, endpointStates: states), Registry(fixture, blocked, eligible));
        AssertSelected(decision); AssertEqual("route:eligible", decision.RouteId, "endpoint_isolation_leaked");
    }

    private static void MultipleEligibleRoutesFailAmbiguous()
    {
        var fixture = CreateAdmission();
        var first = Route(fixture, routeId: "route:first");
        var second = Route(fixture, routeId: "route:second", sourceEndpoint: "endpoint:source/2", destinationEndpoint: "endpoint:destination/2");
        AssertRejected(new RouteSelectionEvaluator().Evaluate(Context(fixture.Result), Registry(fixture, first, second)), RouteSelectionReason.RouteAmbiguous);
    }

    private static void DuplicateRouteIdentityRejected()
    {
        var fixture = CreateAdmission();
        var registry = new RouteRegistry(fixture.ManifestRegistry);
        var route = Route(fixture);
        Assert(registry.Register(route).Accepted, "initial_route_registration_failed");
        AssertRegistrationRejected(registry.Register(route), RouteRegistrationReason.DuplicateIdentity);
    }

    private static void EquivalentInputsSameDecisionIdentity()
    {
        var fixture = RoutingFixture();
        var first = Evaluate(fixture); var second = Evaluate(fixture);
        AssertSelected(first); AssertEqual(first.DecisionId, second.DecisionId, "equivalent_routing_inputs_not_deterministic");
    }

    private static void RouteEvidenceMutationChangesIdentity()
    {
        var fixture = CreateAdmission();
        var first = new RouteSelectionEvaluator().Evaluate(Context(fixture.Result), Registry(fixture, Route(fixture, routeEvidence: "evidence:route/a")));
        var second = new RouteSelectionEvaluator().Evaluate(Context(fixture.Result), Registry(fixture, Route(fixture, routeEvidence: "evidence:route/b")));
        AssertSelected(first); AssertSelected(second); Assert(first.DecisionId != second.DecisionId, "route_evidence_not_bound");
    }

    private static void RouteAuthorityEvidenceMutationChangesIdentity()
    {
        var fixture = CreateAdmission();
        var first = new RouteSelectionEvaluator().Evaluate(Context(fixture.Result), Registry(fixture, Route(fixture, routeAuthorityBindingEvidence: "evidence:route-authority/a")));
        var second = new RouteSelectionEvaluator().Evaluate(Context(fixture.Result), Registry(fixture, Route(fixture, routeAuthorityBindingEvidence: "evidence:route-authority/b")));
        AssertSelected(first); AssertSelected(second); Assert(first.DecisionId != second.DecisionId, "route_authority_evidence_not_bound");
    }

    private static void BindingEvidenceMutationChangesIdentity()
    {
        var fixture = CreateAdmission();
        var registry = Registry(fixture, Route(fixture));
        var first = new RouteSelectionEvaluator().Evaluate(Context(fixture.Result, bindingEvidence: "evidence:binding/a"), registry);
        var second = new RouteSelectionEvaluator().Evaluate(Context(fixture.Result, bindingEvidence: "evidence:binding/b"), registry);
        AssertSelected(first); AssertSelected(second); Assert(first.DecisionId != second.DecisionId, "message_type_binding_evidence_not_bound");
    }

    private static void EndpointEvidenceMutationChangesIdentity()
    {
        var fixture = CreateAdmission(); var route = Route(fixture);
        var a = new[] { Endpoint(route.SourceEndpoint.Value, RouteEndpointState.Eligible, "evidence:endpoint/source-a"), Endpoint(route.DestinationEndpoint.Value, RouteEndpointState.Eligible) };
        var b = new[] { Endpoint(route.SourceEndpoint.Value, RouteEndpointState.Eligible, "evidence:endpoint/source-b"), Endpoint(route.DestinationEndpoint.Value, RouteEndpointState.Eligible) };
        var first = new RouteSelectionEvaluator().Evaluate(Context(fixture.Result, endpointStates: a), Registry(fixture, route));
        var second = new RouteSelectionEvaluator().Evaluate(Context(fixture.Result, endpointStates: b), Registry(fixture, route));
        AssertSelected(first); AssertSelected(second); Assert(first.DecisionId != second.DecisionId, "endpoint_state_evidence_not_bound");
    }

    private static void ObservationMutationChangesIdentity()
    {
        var fixture = CreateAdmission(); var registry = Registry(fixture, Route(fixture));
        var first = new RouteSelectionEvaluator().Evaluate(Context(fixture.Result, observation: RoutingObservation), registry);
        var second = new RouteSelectionEvaluator().Evaluate(Context(fixture.Result, observation: RoutingObservation.AddSeconds(1)), registry);
        AssertSelected(first); AssertSelected(second); Assert(first.DecisionId != second.DecisionId, "observation_time_not_bound");
    }

    private static void RegistryMutationChangesRejectionIdentity()
    {
        var fixture = CreateAdmission();
        var registry = Registry(fixture, Route(fixture, routeId: "route:first"), Route(fixture, routeId: "route:second", sourceEndpoint: "endpoint:source/2", destinationEndpoint: "endpoint:destination/2"));
        var first = new RouteSelectionEvaluator().Evaluate(Context(fixture.Result), registry);
        AssertRejected(first, RouteSelectionReason.RouteAmbiguous);
        Assert(registry.Register(Route(fixture, routeId: "route:third", sourceEndpoint: "endpoint:source/3", destinationEndpoint: "endpoint:destination/3")).Accepted, "third_route_registration_failed");
        var second = new RouteSelectionEvaluator().Evaluate(Context(fixture.Result), registry);
        AssertRejected(second, RouteSelectionReason.RouteAmbiguous);
        Assert(first.DecisionId != second.DecisionId, "registry_snapshot_not_bound_to_rejection_identity");
    }

    private static void RegistryOrderNeutrality()
    {
        var fixture = CreateAdmission();
        var selected = Route(fixture, routeId: "route:selected");
        var unrelatedA = Route(fixture, routeId: "route:z", producer: "producer:z", authorizedProducer: "producer:z");
        var unrelatedB = Route(fixture, routeId: "route:a", producer: "producer:a", authorizedProducer: "producer:a");
        var first = new RouteSelectionEvaluator().Evaluate(Context(fixture.Result), Registry(fixture, unrelatedA, selected, unrelatedB));
        var second = new RouteSelectionEvaluator().Evaluate(Context(fixture.Result), Registry(fixture, unrelatedB, selected, unrelatedA));
        AssertSelected(first); AssertSelected(second); AssertEqual(first.DecisionId, second.DecisionId, "registry_order_changed_decision_identity");
    }

    private static void DecisionSurfaceImmutable()
    {
        var writable = typeof(RouteDecision).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(property => property.SetMethod is { IsPublic: true }).ToArray();
        Assert(writable.Length == 0, "route_decision_has_public_setter");
    }

    private static void DecisionAndRegistryIdentitySha256()
    {
        var result = Evaluate(RoutingFixture());
        AssertSha256(result.DecisionId, "routing_decision_identity_invalid");
        AssertSha256(result.RegistrySnapshotDigest, "registry_snapshot_digest_invalid");
    }

    private static void NoLaterWpOperations()
    {
        string[] prohibited = { "Send", "Dispatch", "Enqueue", "Deliver", "Acknowledge", "Retry", "DeadLetter", "Backpressure", "FlowControl", "Publish", "Subscribe", "Encrypt", "Decrypt", "Attach", "Detach", "Activate", "Execute" };
        var names = typeof(RouteSelectionEvaluator).Assembly.GetExportedTypes().SelectMany(type => type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)).Select(method => method.Name).ToArray();
        foreach (var forbidden in prohibited) Assert(!names.Any(name => name.Contains(forbidden, StringComparison.OrdinalIgnoreCase)), $"later_wp_operation_exposed:{forbidden}");
    }

    private static void RoutingDoesNotDispatch() => AssertNoPublicMethodContaining("dispatch");
    private static void RoutingDoesNotDeliver() => AssertNoPublicMethodContaining("deliver");
    private static void RoutingDoesNotRetry() => AssertNoPublicMethodContaining("retry");

    private static void PayloadRemainsOpaque()
    {
        var trading = CreateAdmission(payload: "{\"market\":\"US\",\"action\":\"BUY\"}");
        var accounting = CreateAdmission(payload: "{\"ledger\":\"receivable\",\"action\":\"POST\"}");
        AssertSelected(new RouteSelectionEvaluator().Evaluate(Context(trading.Result), Registry(trading, Route(trading))));
        AssertSelected(new RouteSelectionEvaluator().Evaluate(Context(accounting.Result), Registry(accounting, Route(accounting))));
    }

    private static void FsatsNoSpecialTreatment()
    {
        var fsats = RoutingFixture("application.fsats", "manifest:fsats", "producer:fsats", "recipient:fsats", "consumer:fsats", "route:fsats");
        var accounting = RoutingFixture("application.accounting", "manifest:accounting", "producer:accounting", "recipient:accounting", "consumer:accounting", "route:accounting");
        AssertSelected(Evaluate(fsats)); AssertSelected(Evaluate(accounting));
    }

    private static void ZeroApplicationFoundationValid()
    {
        var schemas = CreateSchemaRegistry();
        var manifests = new InMemoryApplicationCommunicationManifestRegistry(schemas);
        var admission = new FilMessageAdmissionEvaluator(manifests, schemas).Evaluate(CreateEnvelope(), CreateAdmissionContext());
        AssertRejected(new RouteSelectionEvaluator().Evaluate(Context(admission), new RouteRegistry(manifests)), RouteSelectionReason.MessageNotAdmitted);
    }

    private static RoutingFixtureRecord RoutingFixture(string applicationId = "application.alpha", string manifestId = "manifest:alpha", string producer = Producer, string recipient = Recipient, string consumer = Consumer, string routeId = "route:reference")
    {
        var admission = CreateAdmission(applicationId, manifestId, producer, recipient, consumer);
        return new RoutingFixtureRecord(admission, Registry(admission, Route(admission, routeId: routeId)), Context(admission.Result));
    }

    private static RouteDecision Evaluate(RoutingFixtureRecord fixture) => new RouteSelectionEvaluator().Evaluate(fixture.Context, fixture.Registry);

    private static AdmissionFixture CreateAdmission(
        string applicationId = "application.alpha", string manifestId = "manifest:alpha", string producer = Producer,
        string recipient = Recipient, string consumer = Consumer, bool registerManifest = true,
        DateTimeOffset? messageExpiry = null, string payload = "{\"reference\":\"opaque-payload\"}",
        string admissionAuthorityDecision = AuthorityDecision.Allow)
    {
        var schemas = CreateSchemaRegistry();
        var manifests = new InMemoryApplicationCommunicationManifestRegistry(schemas);
        var manifest = CreateManifest(applicationId, manifestId, consumer, CommunicationDirection.Outbound, CommunicationRole.Producer, MessageType);
        var manifestDigest = ManifestCanonicalization.ComputeSha256(manifest);
        if (registerManifest)
        {
            var registered = manifests.Register(manifest);
            Assert(registered.Accepted, "manifest_registration_failed");
            AssertEqual(manifestDigest, registered.ManifestSha256 ?? string.Empty, "manifest_digest_registration_mismatch");
        }

        var envelope = CreateEnvelope(producer, recipient, messageExpiry, payload);
        var context = CreateAdmissionContext(applicationId, manifestId, producer, recipient, consumer, admissionAuthorityDecision);
        var result = new FilMessageAdmissionEvaluator(manifests, schemas).Evaluate(envelope, context);
        return new AdmissionFixture(result, schemas, manifests, applicationId, manifestId, "1.0", manifestDigest, producer, recipient, consumer);
    }

    private static InMemorySchemaRegistry CreateSchemaRegistry()
    {
        var registry = new InMemorySchemaRegistry();
        Assert(registry.Register(new SchemaDefinition(new SchemaIdentity(SchemaId), "1.0", new SchemaOwnerReference("owner:schema/reference"), SchemaDigest, Evidence("schema/reference"))).Accepted, "schema_registration_failed");
        return registry;
    }

    private static ApplicationCommunicationManifest CreateManifest(string applicationId, string manifestId, string consumer, CommunicationDirection direction, CommunicationRole role, string messageType) =>
        new(new ManifestIdentity(manifestId), "1.0", new ApplicationIdentityReference(applicationId), "1.0", new ApplicationOwnerReference("owner:application/reference"),
            new[] { new ManifestReference("CON-004"), new ManifestReference("CON-023") },
            new[] { new ManifestReference("service:fil"), new ManifestReference("service:authority") },
            new[] { new ManifestReference("capability:reference") }, new[] { new ManifestReference(consumer) },
            new[] { new AuthorityReference(AdmissionAuthorityRef), new AuthorityReference(RouteAuthorityRef) },
            new[] { new ManifestReference("security:reference") }, new[] { new ManifestReference("dependency:reference") },
            new[] { new ManifestReference("configuration:reference") }, new[] { Evidence("manifest/reference") }, Lifecycle(),
            new[] { new CommunicationDeclaration(messageType, FilMessageKind.Command, FilMessageClassification.Operational,
                new ManifestSchemaReference(new SchemaIdentity(SchemaId), "1.0"), direction, role) });

    private static AlternateManifest RegisterAlternateManifest(AdmissionFixture fixture, string manifestId, CommunicationDirection direction, CommunicationRole role, string messageType)
    {
        var manifest = CreateManifest(fixture.ApplicationId, manifestId, fixture.Consumer, direction, role, messageType);
        var result = fixture.ManifestRegistry.Register(manifest);
        Assert(result.Accepted, "alternate_manifest_registration_failed");
        return new AlternateManifest(manifestId, result.ManifestSha256 ?? throw new InvalidOperationException("alternate_manifest_digest_missing"));
    }

    private static CanonicalFilEnvelope CreateEnvelope(string producer = Producer, string recipient = Recipient, DateTimeOffset? expiry = null, string payload = "{\"reference\":\"opaque-payload\"}") =>
        CanonicalFilEnvelope.Create(new MessageIdentity("msg:wp05/0001"), FilMessageKind.Command, FilMessageClassification.Operational,
            MessageType, new SchemaIdentity(SchemaId), "1.0", new ProducerIdentityReference(producer), new RecipientScopeReference(recipient),
            new CorrelationIdentity("correlation:wp05/0001"), new CausationIdentity("causation:wp05/0000"),
            new AuthorityReference(AdmissionAuthorityRef), Evidence("message/reference"), new IdempotencyIdentity("idempotency:wp05/0001"),
            new DeliveryAttemptIdentity("attempt:wp05/0001"), new RetryLineageIdentity("retry-lineage:wp05/0001"),
            new CanonicalMessageTime(AdmissionObservation.AddMinutes(-5), expiry ?? AdmissionObservation.AddMinutes(30)),
            CanonicalOutcome.Unknown("processing_not_yet_attempted"), payload);

    private static MessageAdmissionContext CreateAdmissionContext(
        string applicationId = "application.alpha", string manifestId = "manifest:alpha", string producer = Producer,
        string recipient = Recipient, string consumer = Consumer, string decision = AuthorityDecision.Allow)
    {
        var result = new AuthorityResult("request:authority/wp05-admission", "decision:authority/wp05-admission", decision,
            decision == AuthorityDecision.Allow ? AdmissionEffectiveScope : "NONE", "policy:message-admission", "1.0",
            "conditions:reference", decision == AuthorityDecision.Allow ? "BOUNDED_TO_EFFECTIVE_SCOPE" : "NO_EXECUTION_AUTHORITY",
            decision == AuthorityDecision.Allow ? AuthorityReason.Allowed : AuthorityReason.DefaultDeny,
            AdmissionObservation.AddMinutes(-2), AdmissionObservation.AddMinutes(20), "evidence:authority/admission-result");
        var binding = new MessageAuthorityBinding(new AuthorityReference(AdmissionAuthorityRef), result,
            new ProducerIdentityReference(producer), new ApplicationIdentityReference(applicationId), new RecipientScopeReference(recipient),
            MessageAdmissionPurpose.FilMessageAdmission, decision == AuthorityDecision.Allow ? AdmissionEffectiveScope : "NONE", Evidence("authority/admission-binding"));
        return new MessageAdmissionContext(
            new MessageProducerBinding(new ProducerIdentityReference(producer), new ApplicationIdentityReference(applicationId), new ManifestIdentity(manifestId), Evidence("producer/binding")),
            "1.0", new MessageRecipientBinding(new RecipientScopeReference(recipient), new ManifestReference(consumer), Evidence("recipient/binding")),
            AdmissionObservation, binding, Evidence("admission/reference"));
    }

    private static RouteDeclaration Route(
        AdmissionFixture fixture,
        string routeId = "route:reference", string routeVersion = "1.0",
        string? manifestId = null, string? manifestDigest = null,
        string? producer = null, string? applicationId = null, string? recipient = null, string? consumer = null,
        string messageType = MessageType, string purpose = Purpose, RouteState state = RouteState.Eligible,
        string sourceEndpoint = "endpoint:source/reference", string destinationEndpoint = "endpoint:destination/reference",
        string routeEvidence = "evidence:route/reference", string routeAuthorityDecision = AuthorityDecision.Allow,
        DateTimeOffset? routeAuthorityDecisionTime = null, DateTimeOffset? routeAuthorityExpiry = null,
        string routeAuthorityEffectiveScope = RouteEffectiveScope, string routeAuthorityBindingEvidence = "evidence:route-authority/binding",
        string? authorizedRouteId = null, string? authorizedProducer = null, string? authorizedApplication = null,
        string? authorizedRecipient = null, string? authorizedConsumer = null, string? authorizedMessageType = null,
        string? authorizedPurpose = null, AuthorityResult? authorityResult = null)
    {
        var resolvedProducer = producer ?? fixture.Producer;
        var resolvedApplication = applicationId ?? fixture.ApplicationId;
        var resolvedRecipient = recipient ?? fixture.Recipient;
        var resolvedConsumer = consumer ?? fixture.Consumer;
        var result = authorityResult ?? new AuthorityResult("request:route/wp05", "decision:route/wp05", routeAuthorityDecision,
            routeAuthorityDecision == AuthorityDecision.Allow ? routeAuthorityEffectiveScope : "NONE", "policy:route-selection", "1.0",
            "conditions:reference", routeAuthorityDecision == AuthorityDecision.Allow ? "BOUNDED_TO_ROUTE" : "NO_ROUTE_AUTHORITY",
            routeAuthorityDecision == AuthorityDecision.Allow ? AuthorityReason.Allowed : AuthorityReason.DefaultDeny,
            routeAuthorityDecisionTime ?? RoutingObservation.AddMinutes(-2), routeAuthorityExpiry ?? RoutingObservation.AddMinutes(20), "evidence:route-authority/result");
        var authority = new RouteAuthorityBinding(new AuthorityReference(RouteAuthorityRef), result,
            new RouteIdentity(authorizedRouteId ?? routeId), routeVersion,
            new ProducerIdentityReference(authorizedProducer ?? resolvedProducer), new ApplicationIdentityReference(authorizedApplication ?? resolvedApplication),
            new RecipientScopeReference(authorizedRecipient ?? resolvedRecipient), new ManifestReference(authorizedConsumer ?? resolvedConsumer),
            authorizedMessageType ?? messageType, authorizedPurpose ?? purpose,
            routeAuthorityDecision == AuthorityDecision.Allow ? routeAuthorityEffectiveScope : "NONE", EvidenceFromValue(routeAuthorityBindingEvidence));
        return new RouteDeclaration(new RouteIdentity(routeId), routeVersion, new ManifestIdentity(manifestId ?? fixture.ManifestId), fixture.ManifestVersion,
            manifestDigest ?? fixture.ManifestDigest, new ProducerIdentityReference(resolvedProducer), new ApplicationIdentityReference(resolvedApplication),
            new RecipientScopeReference(resolvedRecipient), new ManifestReference(resolvedConsumer), messageType,
            new RouteEndpointIdentity(sourceEndpoint), new RouteEndpointIdentity(destinationEndpoint), purpose, state, authority, EvidenceFromValue(routeEvidence));
    }

    private static RouteSelectionContext Context(MessageAdmissionResult admission, string messageType = MessageType, string purpose = Purpose,
        string? bindingDecisionId = null, string bindingEvidence = "evidence:routing/message-type-binding", DateTimeOffset? observation = null,
        IEnumerable<RouteEndpointStateBinding>? endpointStates = null) =>
        new(admission, new RoutingMessageTypeBinding(bindingDecisionId ?? admission.DecisionId, messageType, EvidenceFromValue(bindingEvidence)),
            purpose, observation ?? RoutingObservation, endpointStates, Evidence("routing/decision"));

    private static RouteEndpointStateBinding Endpoint(string endpoint, RouteEndpointState state, string evidence = "evidence:endpoint/state") =>
        new(new RouteEndpointIdentity(endpoint), state, EvidenceFromValue(evidence));

    private static RouteRegistry Registry(AdmissionFixture fixture, params RouteDeclaration[] routes)
    {
        var registry = new RouteRegistry(fixture.ManifestRegistry);
        foreach (var route in routes) Assert(registry.Register(route).Accepted, "route_registration_failed");
        return registry;
    }

    private static ManifestLifecycleDeclaration[] Lifecycle() => new[]
    {
        new ManifestLifecycleDeclaration(ManifestLifecycleEvent.ApplicationVersionChange, ManifestApplicabilityRule.RequiresRevalidation),
        new ManifestLifecycleDeclaration(ManifestLifecycleEvent.Update, ManifestApplicabilityRule.RequiresRevalidation),
        new ManifestLifecycleDeclaration(ManifestLifecycleEvent.Replacement, ManifestApplicabilityRule.Invalidated),
        new ManifestLifecycleDeclaration(ManifestLifecycleEvent.Suspension, ManifestApplicabilityRule.RemainsApplicable),
        new ManifestLifecycleDeclaration(ManifestLifecycleEvent.Removal, ManifestApplicabilityRule.Invalidated)
    };

    private static ProvenanceReference Evidence(string suffix) => new($"evidence:{suffix}");
    private static ProvenanceReference EvidenceFromValue(string value) => new(value);

    private static void AssertNoPublicMethodContaining(string fragment)
    {
        var found = typeof(RouteSelectionEvaluator).Assembly.GetExportedTypes()
            .SelectMany(type => type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
            .Any(method => method.Name.Contains(fragment, StringComparison.OrdinalIgnoreCase));
        Assert(!found, $"prohibited_public_operation_found:{fragment}");
    }

    private static void AssertSelected(RouteDecision result)
    {
        Assert(result.Decision == RouteSelectionDecision.Selected, $"expected_selected:{result.Reason}");
        AssertEqual(RouteSelectionReason.RouteSelected, result.Reason, "selected_reason_mismatch");
    }

    private static void AssertRejected(RouteDecision result, string reason)
    {
        Assert(result.Decision == RouteSelectionDecision.Rejected, "expected_rejected");
        AssertEqual(reason, result.Reason, "rejection_reason_mismatch");
    }

    private static void AssertRegistrationRejected(RouteRegistrationResult result, string reason)
    {
        Assert(!result.Accepted, "expected_route_registration_rejected");
        AssertEqual(reason, result.Reason, "route_registration_reason_mismatch");
    }

    private static void AssertSha256(string value, string message)
    {
        Assert(value.Length == 64, message + ":length");
        Assert(value.All(c => c is >= '0' and <= '9' || c is >= 'A' and <= 'F'), message + ":format");
    }

    private static void Assert(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    private static void AssertEqual<T>(T expected, T actual, string message)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual)) throw new InvalidOperationException($"{message}:expected={expected}:actual={actual}");
    }

    private sealed record AdmissionFixture(
        MessageAdmissionResult Result,
        InMemorySchemaRegistry SchemaRegistry,
        InMemoryApplicationCommunicationManifestRegistry ManifestRegistry,
        string ApplicationId,
        string ManifestId,
        string ManifestVersion,
        string ManifestDigest,
        string Producer,
        string Recipient,
        string Consumer);

    private sealed record RoutingFixtureRecord(AdmissionFixture Admission, RouteRegistry Registry, RouteSelectionContext Context);
    private sealed record AlternateManifest(string ManifestId, string Digest);
}
