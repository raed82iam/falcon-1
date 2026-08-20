using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Foundation.ApplicationManifest;
using Foundation.Authority;
using Foundation.Contracts;
using Foundation.MessageAdmission;
using Foundation.MessageDelivery;
using Foundation.MessageRouting;
using Foundation.SchemaRegistry;

namespace Falcon.Stage5.WP06.Verifier;

internal static class Program
{
    private const string MessageType = "falcon.reference.delivery.v1";
    private const string SchemaId = "schema:falcon.reference.delivery";
    private const string AdmissionAuthorityRef = "authority:message-admission/wp06";
    private const string RouteAuthorityRef = "authority:route/wp06";
    private const string AdmissionScope = "scope:message-admission/wp06";
    private const string RouteScope = "scope:route/wp06";
    private const string RoutePurpose = "purpose:governed-route/wp06";
    private static readonly DateTimeOffset AdmissionTime = new(2026, 8, 8, 0, 0, 0, TimeSpan.Zero);
    private static readonly DateTimeOffset RoutingTime = AdmissionTime.AddMinutes(1);
    private static readonly DateTimeOffset DeliveryTime = AdmissionTime.AddMinutes(2);
    private static readonly string SchemaDigest = new('A', 64);

    private static int Main()
    {
        var scenarios = new (string Name, Action Test)[]
        {
            ("initial_dispatch_eligible", InitialDispatchEligible),
            ("transport_dispatch_observation_recorded", DispatchObservationRecorded),
            ("recipient_acknowledgement_is_transport_status_only", RecipientAcknowledgementIsTransportOnly),
            ("retryable_failure_allows_bounded_retry", RetryableFailureAllowsRetry),
            ("at_most_once_prohibits_retry", AtMostOnceProhibitsRetry),
            ("best_effort_prohibits_retry", BestEffortProhibitsRetry),
            ("retry_limit_exhaustion_deadletters", RetryLimitExhaustionDeadletters),
            ("expiry_blocks_initial_dispatch", ExpiryBlocksInitialDispatch),
            ("expiry_blocks_retry", ExpiryBlocksRetry),
            ("idempotency_required_missing_deadletters", IdempotencyMissingDeadletters),
            ("idempotency_binding_mismatch_deadletters", IdempotencyMismatchDeadletters),
            ("valid_idempotency_binding_allows_retry", ValidIdempotencyAllowsRetry),
            ("destination_unknown_defers", DestinationUnknownDefers),
            ("destination_unavailable_defers_before_limit", DestinationUnavailableDefers),
            ("destination_unavailable_terminal_at_limit", DestinationUnavailableTerminal),
            ("route_capacity_pressure_defers", RoutePressureDefers),
            ("producer_capacity_pressure_defers", ProducerPressureDefers),
            ("normal_traffic_preserves_elevated_reserve", NormalTrafficPreservesReserve),
            ("protective_traffic_can_use_reserved_capacity", ProtectiveTrafficUsesReserve),
            ("protective_traffic_cannot_exceed_global_limit", ProtectiveTrafficCannotExceedGlobalLimit),
            ("elevated_traffic_requires_authority_binding", ElevatedTrafficRequiresAuthority),
            ("normal_traffic_rejects_hidden_elevated_authority", NormalTrafficRejectsHiddenAuthority),
            ("malformed_priority_authority_rejected", MalformedPriorityAuthorityRejected),
            ("denied_priority_authority_rejected", DeniedPriorityAuthorityRejected),
            ("future_priority_authority_rejected", FuturePriorityAuthorityRejected),
            ("expired_priority_authority_rejected", ExpiredPriorityAuthorityRejected),
            ("priority_authority_policy_binding_mismatch_rejected", PriorityAuthorityBindingMismatchRejected),
            ("policy_route_binding_mismatch_rejected", PolicyRouteMismatchRejected),
            ("pressure_route_binding_mismatch_rejected", PressureRouteMismatchRejected),
            ("predecessor_admission_binding_mismatch_rejected", PredecessorAdmissionMismatchRejected),
            ("previous_outcome_lineage_mismatch_rejected", PreviousOutcomeLineageMismatchRejected),
            ("acknowledged_attempt_cannot_retry", AcknowledgedAttemptCannotRetry),
            ("terminal_failure_deadletters", TerminalFailureDeadletters),
            ("ordering_none_rejects_key", OrderingNoneRejectsKey),
            ("per_key_ordering_requires_key", PerKeyOrderingRequiresKey),
            ("equivalent_inputs_same_decision_identity", EquivalentInputsSameIdentity),
            ("pressure_mutation_changes_decision_identity", PressureMutationChangesIdentity),
            ("policy_evidence_mutation_changes_decision_identity", PolicyEvidenceMutationChangesIdentity),
            ("observation_time_mutation_changes_decision_identity", ObservationTimeMutationChangesIdentity),
            ("outcome_evidence_mutation_changes_outcome_identity", OutcomeEvidenceMutationChangesIdentity),
            ("outcome_time_cannot_precede_dispatch_decision", OutcomeCannotPrecedeDecision),
            ("delivery_decision_surface_is_immutable", DeliveryDecisionSurfaceImmutable),
            ("delivery_outcome_surface_is_immutable", DeliveryOutcomeSurfaceImmutable),
            ("decision_and_outcome_identities_are_sha256", IdentitiesAreSha256),
            ("delivery_surface_has_no_wp07_plus_operations", NoLaterWpOperations),
            ("payload_business_semantics_remain_opaque", PayloadBusinessSemanticsOpaque),
            ("fsats_receives_no_special_treatment", FsatsReceivesNoSpecialTreatment),
            ("two_applications_pressure_isolated", TwoApplicationsPressureIsolated),
            ("outcome_identity_binds_exact_delivery_decision", OutcomeIdentityBindsDeliveryDecision),
            ("canonical_envelope_required", CanonicalEnvelopeRequired),
            ("canonical_envelope_binding_mismatch_rejected", CanonicalEnvelopeMismatchRejected),
            ("correlation_causation_preserved_in_decision_and_outcome", CorrelationCausationPreserved),
            ("malformed_pressure_authority_rejected", MalformedPressureAuthorityRejected),
            ("denied_pressure_authority_rejected", DeniedPressureAuthorityRejected),
            ("future_pressure_authority_rejected", FuturePressureAuthorityRejected),
            ("expired_pressure_authority_rejected", ExpiredPressureAuthorityRejected),
            ("pressure_authority_limit_mismatch_rejected", PressureAuthorityLimitMismatchRejected),
            ("future_pressure_observation_rejected", FuturePressureObservationRejected)
        };

        var failures = new List<string>();
        foreach (var scenario in scenarios)
        {
            try
            {
                scenario.Test();
                Console.WriteLine($"PASS {scenario.Name}");
            }
            catch (Exception ex)
            {
                failures.Add($"{scenario.Name}: {ex.GetType().Name}: {ex.Message}");
                Console.WriteLine($"FAIL {scenario.Name}: {ex.Message}");
            }
        }

        Console.WriteLine($"RESULT {scenarios.Length - failures.Count}/{scenarios.Length} PASS");
        Console.WriteLine(failures.Count == 0
            ? "STAGE 5 WP-06 SERVICE BUS DELIVERY SEMANTICS AND FLOW CONTROL VERIFIER: PASS"
            : "STAGE 5 WP-06 SERVICE BUS DELIVERY SEMANTICS AND FLOW CONTROL VERIFIER: FAIL");
        foreach (var failure in failures) Console.Error.WriteLine($"DETAIL {failure}");
        return failures.Count == 0 ? 0 : 1;
    }

    private static void InitialDispatchEligible() => AssertDispatch(Evaluate(Fixture()));

    private static void DispatchObservationRecorded()
    {
        var decision = Evaluate(Fixture());
        var outcome = Record(decision, TransportObservationKind.DispatchAccepted, DeliveryTime.AddSeconds(1), "evidence:outcome/dispatch");
        Assert(outcome.Observation == TransportObservationKind.DispatchAccepted, "dispatch_observation_not_recorded");
        Assert(!outcome.IsAcknowledged, "dispatch_must_not_equal_acknowledgement");
    }

    private static void RecipientAcknowledgementIsTransportOnly()
    {
        var decision = Evaluate(Fixture());
        var outcome = Record(decision, TransportObservationKind.RecipientAcknowledged, DeliveryTime.AddSeconds(1), "evidence:outcome/ack");
        Assert(outcome.IsAcknowledged, "recipient_acknowledgement_not_recorded");
        Assert(!typeof(DeliveryAttemptOutcome).GetProperties().Any(p => p.Name.Contains("BusinessSuccess", StringComparison.OrdinalIgnoreCase)),
            "transport_ack_overstates_business_success");
    }

    private static void RetryableFailureAllowsRetry()
    {
        var f = Fixture();
        var previous = Record(Evaluate(f), TransportObservationKind.RetryableFailure, DeliveryTime.AddSeconds(1), "evidence:outcome/retryable");
        AssertRetry(Evaluate(WithContext(f, attempt: 2, previous: previous, idempotency: Idempotency(f))));
    }

    private static void AtMostOnceProhibitsRetry() => AssertNonRetryingGuarantee(DeliveryGuarantee.AtMostOnce);
    private static void BestEffortProhibitsRetry() => AssertNonRetryingGuarantee(DeliveryGuarantee.BestEffort);

    private static void AssertNonRetryingGuarantee(DeliveryGuarantee guarantee)
    {
        var f = Fixture(guarantee: guarantee, maxAttempts: 1);
        var previous = Record(Evaluate(f), TransportObservationKind.RetryableFailure, DeliveryTime.AddSeconds(1), "evidence:outcome/retryable");
        AssertTerminal(Evaluate(WithContext(f, attempt: 2, previous: previous)), MessageDeliveryReason.RetryNotPermitted);
    }

    private static void RetryLimitExhaustionDeadletters()
    {
        var f = Fixture(maxAttempts: 2);
        var out1 = Record(Evaluate(f), TransportObservationKind.RetryableFailure, DeliveryTime.AddSeconds(1), "evidence:outcome/1");
        var second = Evaluate(WithContext(f, attempt: 2, previous: out1, idempotency: Idempotency(f), observation: DeliveryTime.AddSeconds(2)));
        AssertRetry(second);
        var out2 = Record(second, TransportObservationKind.RetryableFailure, DeliveryTime.AddSeconds(3), "evidence:outcome/2");
        AssertDeadLetter(Evaluate(WithContext(f, attempt: 3, previous: out2, idempotency: Idempotency(f), observation: DeliveryTime.AddSeconds(4))),
            MessageDeliveryReason.RetryLimitExhausted);
    }

    private static void ExpiryBlocksInitialDispatch()
    {
        var f = Fixture(messageExpiry: DeliveryTime);
        AssertDecision(Evaluate(f), DeliveryDecisionKind.Expired, MessageDeliveryReason.MessageExpired);
    }

    private static void ExpiryBlocksRetry()
    {
        var f = Fixture(messageExpiry: DeliveryTime.AddSeconds(2));
        var previous = Record(Evaluate(f), TransportObservationKind.RetryableFailure, DeliveryTime.AddSeconds(1), "evidence:outcome/retryable");
        AssertDecision(Evaluate(WithContext(f, attempt: 2, previous: previous, idempotency: Idempotency(f), observation: DeliveryTime.AddSeconds(2))),
            DeliveryDecisionKind.Expired, MessageDeliveryReason.MessageExpired);
    }

    private static void IdempotencyMissingDeadletters()
    {
        var f = Fixture();
        var previous = Record(Evaluate(f), TransportObservationKind.RetryableFailure, DeliveryTime.AddSeconds(1), "evidence:outcome/retryable");
        AssertDeadLetter(Evaluate(WithContext(f, attempt: 2, previous: previous)), MessageDeliveryReason.IdempotencyRequired);
    }

    private static void IdempotencyMismatchDeadletters()
    {
        var f = Fixture();
        var previous = Record(Evaluate(f), TransportObservationKind.RetryableFailure, DeliveryTime.AddSeconds(1), "evidence:outcome/retryable");
        var bad = new DeliveryIdempotencyBinding(
            "route-decision/sha256/FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF",
            f.Admission.DecisionId, "idempotency:bad", "evidence:idempotency/bad");
        AssertDeadLetter(Evaluate(WithContext(f, attempt: 2, previous: previous, idempotency: bad)), MessageDeliveryReason.IdempotencyBindingMismatch);
    }

    private static void ValidIdempotencyAllowsRetry() => RetryableFailureAllowsRetry();

    private static void DestinationUnknownDefers()
    {
        var f = Fixture();
        AssertDecision(Evaluate(WithContext(f, health: DeliveryDestinationHealth.Unknown)), DeliveryDecisionKind.Deferred, MessageDeliveryReason.DestinationUnknown);
    }

    private static void DestinationUnavailableDefers()
    {
        var f = Fixture(maxAttempts: 3);
        AssertDecision(Evaluate(WithContext(f, health: DeliveryDestinationHealth.Unavailable)), DeliveryDecisionKind.Deferred, MessageDeliveryReason.DestinationUnavailable);
    }

    private static void DestinationUnavailableTerminal()
    {
        var f = Fixture(maxAttempts: 1);
        AssertDeadLetter(Evaluate(WithContext(f, health: DeliveryDestinationHealth.Unavailable)), MessageDeliveryReason.DestinationUnavailable);
    }

    private static void RoutePressureDefers()
    {
        var f = Fixture();
        AssertDecision(Evaluate(WithContext(f, pressure: Pressure(f, routeLimit: 2, routeInFlight: 2))), DeliveryDecisionKind.Deferred, MessageDeliveryReason.FlowControlDeferred);
    }

    private static void ProducerPressureDefers()
    {
        var f = Fixture();
        AssertDecision(Evaluate(WithContext(f, pressure: Pressure(f, producerLimit: 2, producerInFlight: 2))), DeliveryDecisionKind.Deferred, MessageDeliveryReason.FlowControlDeferred);
    }

    private static void NormalTrafficPreservesReserve()
    {
        var f = Fixture();
        AssertDecision(Evaluate(WithContext(f, pressure: Pressure(f, globalLimit: 10, globalInFlight: 8, reserved: 2))), DeliveryDecisionKind.Deferred, MessageDeliveryReason.FlowControlDeferred);
    }

    private static void ProtectiveTrafficUsesReserve()
    {
        var f0 = Fixture();
        var f = RePolicy(f0, Policy(f0.Route, trafficClass: DeliveryTrafficClass.Protective,
            authority: PriorityAuthority(f0.Route, DeliveryTrafficClass.Protective)));
        AssertDispatch(Evaluate(WithContext(f, pressure: Pressure(f, globalLimit: 10, globalInFlight: 8, reserved: 2))));
    }

    private static void ProtectiveTrafficCannotExceedGlobalLimit()
    {
        var f0 = Fixture();
        var f = RePolicy(f0, Policy(f0.Route, trafficClass: DeliveryTrafficClass.Protective,
            authority: PriorityAuthority(f0.Route, DeliveryTrafficClass.Protective)));
        AssertDecision(Evaluate(WithContext(f, pressure: Pressure(f, globalLimit: 10, globalInFlight: 10, reserved: 2))), DeliveryDecisionKind.Deferred, MessageDeliveryReason.FlowControlDeferred);
    }

    private static void ElevatedTrafficRequiresAuthority() => AssertThrows<ArgumentNullException>(() =>
        new DeliveryPolicy("policy:delivery/wp06", "1.0", "route-decision/reference", DeliveryGuarantee.AtLeastOnce, 3,
            DeliveryOrderingGuarantee.None, null, true, true, DeliveryTrafficClass.Protective, null, "evidence:policy"));

    private static void NormalTrafficRejectsHiddenAuthority()
    {
        var f = Fixture();
        AssertThrows<ArgumentException>(() => Policy(f.Route, authority: PriorityAuthority(f.Route, DeliveryTrafficClass.Protective)));
    }

    private static void MalformedPriorityAuthorityRejected() => AssertPriorityAuthorityResult(PriorityAuthorityVariant.Malformed, MessageDeliveryReason.PriorityAuthorityMalformed);
    private static void DeniedPriorityAuthorityRejected() => AssertPriorityAuthorityResult(PriorityAuthorityVariant.Denied, MessageDeliveryReason.PriorityAuthorityDenied);
    private static void FuturePriorityAuthorityRejected() => AssertPriorityAuthorityResult(PriorityAuthorityVariant.Future, MessageDeliveryReason.PriorityAuthorityNotYetEffective);
    private static void ExpiredPriorityAuthorityRejected() => AssertPriorityAuthorityResult(PriorityAuthorityVariant.Expired, MessageDeliveryReason.PriorityAuthorityExpired);

    private static void AssertPriorityAuthorityResult(PriorityAuthorityVariant variant, string expectedReason)
    {
        var f0 = Fixture();
        var authority = variant switch
        {
            PriorityAuthorityVariant.Malformed => PriorityAuthority(f0.Route, DeliveryTrafficClass.Protective, malformed: true),
            PriorityAuthorityVariant.Denied => PriorityAuthority(f0.Route, DeliveryTrafficClass.Protective, decision: AuthorityDecision.Deny),
            PriorityAuthorityVariant.Future => PriorityAuthority(f0.Route, DeliveryTrafficClass.Protective, decisionTime: DeliveryTime.AddMinutes(1)),
            PriorityAuthorityVariant.Expired => PriorityAuthority(f0.Route, DeliveryTrafficClass.Protective, expiry: DeliveryTime),
            _ => throw new ArgumentOutOfRangeException(nameof(variant))
        };
        var f = RePolicy(f0, Policy(f0.Route, trafficClass: DeliveryTrafficClass.Protective, authority: authority));
        AssertDecision(Evaluate(f), DeliveryDecisionKind.Rejected, expectedReason);
    }

    private static void PriorityAuthorityBindingMismatchRejected()
    {
        var f0 = Fixture();
        var f = RePolicy(f0, Policy(f0.Route, trafficClass: DeliveryTrafficClass.Protective,
            authority: PriorityAuthority(f0.Route, DeliveryTrafficClass.Protective, authorizedPolicyId: "policy:other")));
        AssertDecision(Evaluate(f), DeliveryDecisionKind.Rejected, MessageDeliveryReason.PriorityAuthorityMismatch);
    }

    private static void PolicyRouteMismatchRejected()
    {
        var f0 = Fixture();
        var policy = new DeliveryPolicy("policy:delivery/wp06", "1.0",
            "route-decision/sha256/FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF",
            DeliveryGuarantee.AtLeastOnce, 3, DeliveryOrderingGuarantee.None, null, true, true,
            DeliveryTrafficClass.Normal, null, "evidence:policy");
        AssertDecision(Evaluate(RePolicy(f0, policy)), DeliveryDecisionKind.Rejected, MessageDeliveryReason.PolicyRouteMismatch);
    }

    private static void PressureRouteMismatchRejected()
    {
        var f = Fixture();
        var badRoute = "route-decision/sha256/FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF";
        var authority = PressureAuthority(f, 10, 3, 3, 1, authorizedRouteDecisionId: badRoute);
        var pressure = new DeliveryPressureSnapshot(badRoute, f.Route.ProducerApplicationId, 10, 0, 3, 0, 3, 0, 1,
            DeliveryTime.AddSeconds(-1), authority, "evidence:pressure/bad");
        AssertDecision(Evaluate(WithContext(f, pressure: pressure)), DeliveryDecisionKind.Rejected, MessageDeliveryReason.PredecessorBindingMismatch);
    }

    private static void PredecessorAdmissionMismatchRejected()
    {
        var a = Fixture("application.alpha", "manifest:alpha", "producer:alpha", "recipient:alpha", "consumer:alpha", "route:alpha");
        var b = Fixture("application.beta", "manifest:beta", "producer:beta", "recipient:beta", "consumer:beta", "route:beta");
        var context = new DeliveryEvaluationContext(a.Route, b.Admission, a.Envelope, a.Policy, 1, null, null,
            DeliveryDestinationHealth.Healthy, Pressure(a), DeliveryTime, "evidence:delivery/mismatch");
        AssertDecision(new FilMessageDeliveryEvaluator().Evaluate(context), DeliveryDecisionKind.Rejected, MessageDeliveryReason.PredecessorBindingMismatch);
    }

    private static void PreviousOutcomeLineageMismatchRejected()
    {
        var a = Fixture(routeId: "route:a");
        var b = Fixture(routeId: "route:b");
        var previous = Record(Evaluate(b), TransportObservationKind.RetryableFailure, DeliveryTime.AddSeconds(1), "evidence:outcome/b");
        AssertDecision(Evaluate(WithContext(a, attempt: 2, previous: previous, idempotency: Idempotency(a))), DeliveryDecisionKind.Rejected, MessageDeliveryReason.PreviousOutcomeMismatch);
    }

    private static void AcknowledgedAttemptCannotRetry()
    {
        var f = Fixture();
        var previous = Record(Evaluate(f), TransportObservationKind.RecipientAcknowledged, DeliveryTime.AddSeconds(1), "evidence:outcome/ack");
        AssertDecision(Evaluate(WithContext(f, attempt: 2, previous: previous, idempotency: Idempotency(f))), DeliveryDecisionKind.AlreadyAcknowledged, MessageDeliveryReason.AlreadyAcknowledged);
    }

    private static void TerminalFailureDeadletters()
    {
        var f = Fixture();
        var previous = Record(Evaluate(f), TransportObservationKind.TerminalFailure, DeliveryTime.AddSeconds(1), "evidence:outcome/terminal");
        AssertDeadLetter(Evaluate(WithContext(f, attempt: 2, previous: previous)), MessageDeliveryReason.PreviousOutcomeTerminal);
    }

    private static void OrderingNoneRejectsKey() => AssertThrows<ArgumentException>(() =>
        new DeliveryPolicy("policy:delivery/wp06", "1.0", "route-decision/reference", DeliveryGuarantee.AtLeastOnce, 3,
            DeliveryOrderingGuarantee.None, "order:key", true, true, DeliveryTrafficClass.Normal, null, "evidence:policy"));

    private static void PerKeyOrderingRequiresKey() => AssertThrows<ArgumentException>(() =>
        new DeliveryPolicy("policy:delivery/wp06", "1.0", "route-decision/reference", DeliveryGuarantee.AtLeastOnce, 3,
            DeliveryOrderingGuarantee.PerKey, null, true, true, DeliveryTrafficClass.Normal, null, "evidence:policy"));

    private static void EquivalentInputsSameIdentity()
    {
        var f = Fixture();
        AssertEqual(Evaluate(f).DecisionId, Evaluate(f).DecisionId, "equivalent_delivery_inputs_not_deterministic");
    }

    private static void PressureMutationChangesIdentity()
    {
        var f = Fixture();
        var a = Evaluate(f);
        var b = Evaluate(WithContext(f, pressure: Pressure(f, globalInFlight: 1)));
        Assert(a.DecisionId != b.DecisionId, "pressure_mutation_not_bound_to_identity");
    }

    private static void PolicyEvidenceMutationChangesIdentity()
    {
        var f = Fixture();
        var a = Evaluate(f);
        var b = Evaluate(RePolicy(f, Policy(f.Route, evidence: "evidence:delivery/policy-changed")));
        Assert(a.DecisionId != b.DecisionId, "policy_evidence_not_bound_to_identity");
    }

    private static void ObservationTimeMutationChangesIdentity()
    {
        var f = Fixture();
        var a = Evaluate(f);
        var b = Evaluate(WithContext(f, observation: DeliveryTime.AddSeconds(1)));
        Assert(a.DecisionId != b.DecisionId, "observation_time_not_bound_to_identity");
    }

    private static void OutcomeEvidenceMutationChangesIdentity()
    {
        var decision = Evaluate(Fixture());
        var a = Record(decision, TransportObservationKind.DispatchAccepted, DeliveryTime.AddSeconds(1), "evidence:outcome/a");
        var b = Record(decision, TransportObservationKind.DispatchAccepted, DeliveryTime.AddSeconds(1), "evidence:outcome/b");
        Assert(a.OutcomeId != b.OutcomeId, "outcome_evidence_not_bound_to_identity");
    }

    private static void OutcomeCannotPrecedeDecision()
    {
        var decision = Evaluate(Fixture());
        var result = new DeliveryOutcomeRecorder().Record(decision,
            new TransportOutcomeObservation(decision.RouteDecisionId, decision.PolicyId, decision.PolicyVersion,
                decision.AttemptNumber, TransportObservationKind.DispatchAccepted, DeliveryTime.AddSeconds(-1), "evidence:outcome/time-travel"));
        Assert(!result.Accepted, "time_travel_outcome_accepted");
        AssertEqual(DeliveryOutcomeReason.ObservationTimeInvalid, result.Reason, "time_travel_reason_mismatch");
    }

    private static void DeliveryDecisionSurfaceImmutable() => AssertNoPublicSetters(typeof(DeliveryDecision), "delivery_decision");
    private static void DeliveryOutcomeSurfaceImmutable() => AssertNoPublicSetters(typeof(DeliveryAttemptOutcome), "delivery_outcome");

    private static void IdentitiesAreSha256()
    {
        var decision = Evaluate(Fixture());
        AssertSha256(decision.DecisionId, "delivery_decision_id_invalid");
        AssertSha256(decision.PressureSnapshotId, "pressure_snapshot_id_invalid");
        AssertSha256(Record(decision, TransportObservationKind.DispatchAccepted, DeliveryTime.AddSeconds(1), "evidence:outcome").OutcomeId,
            "delivery_outcome_id_invalid");
    }

    private static void NoLaterWpOperations()
    {
        string[] prohibited = { "Publish", "Subscribe", "Replay", "Encrypt", "Decrypt", "Sign", "Attach", "Detach", "Activate", "Install", "Upgrade", "RemoveApplication" };
        var names = typeof(FilMessageDeliveryEvaluator).Assembly.GetExportedTypes()
            .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
            .Select(m => m.Name).ToArray();
        foreach (var token in prohibited)
            Assert(!names.Any(n => n.Contains(token, StringComparison.OrdinalIgnoreCase)), $"later_wp_operation_exposed:{token}");
    }

    private static void PayloadBusinessSemanticsOpaque()
    {
        AssertDispatch(Evaluate(Fixture(payload: "{\"market\":\"US\",\"action\":\"BUY\"}")));
        AssertDispatch(Evaluate(Fixture(payload: "{\"ledger\":\"receivable\",\"action\":\"POST\"}")));
    }

    private static void FsatsReceivesNoSpecialTreatment()
    {
        AssertDispatch(Evaluate(Fixture("application.fsats", "manifest:fsats", "producer:fsats", "recipient:fsats", "consumer:fsats", "route:fsats")));
        AssertDispatch(Evaluate(Fixture("application.accounting", "manifest:accounting", "producer:accounting", "recipient:accounting", "consumer:accounting", "route:accounting")));
    }

    private static void TwoApplicationsPressureIsolated()
    {
        var a = Fixture("application.alpha", "manifest:alpha", "producer:alpha", "recipient:alpha", "consumer:alpha", "route:alpha");
        var b = Fixture("application.beta", "manifest:beta", "producer:beta", "recipient:beta", "consumer:beta", "route:beta");
        AssertDecision(Evaluate(WithContext(a, pressure: Pressure(a, routeLimit: 1, routeInFlight: 1))), DeliveryDecisionKind.Deferred, MessageDeliveryReason.FlowControlDeferred);
        AssertDispatch(Evaluate(b));
    }

    private static void OutcomeIdentityBindsDeliveryDecision()
    {
        var f = Fixture();
        var d1 = Evaluate(f);
        var d2 = Evaluate(WithContext(f, observation: DeliveryTime.AddMilliseconds(1)));
        var t = DeliveryTime.AddSeconds(1);
        var o1 = Record(d1, TransportObservationKind.DispatchAccepted, t, "evidence:outcome/shared");
        var o2 = Record(d2, TransportObservationKind.DispatchAccepted, t, "evidence:outcome/shared");
        Assert(d1.DecisionId != d2.DecisionId, "fixture_delivery_decisions_not_distinct");
        Assert(o1.OutcomeId != o2.OutcomeId, "outcome_identity_not_bound_to_exact_delivery_decision");
    }

    private static void CanonicalEnvelopeRequired()
    {
        var f = Fixture();
        var context = new DeliveryEvaluationContext(f.Route, f.Admission, null, f.Policy, 1, null, null,
            DeliveryDestinationHealth.Healthy, Pressure(f), DeliveryTime, "evidence:delivery/no-envelope");
        AssertDecision(new FilMessageDeliveryEvaluator().Evaluate(context), DeliveryDecisionKind.Rejected, MessageDeliveryReason.EnvelopeRequired);
    }

    private static void CanonicalEnvelopeMismatchRejected()
    {
        var f = Fixture();
        var altered = Envelope(f.Envelope.Producer.Value, f.Envelope.RecipientScope.Value, f.Envelope.Time.ExpiresAt,
            f.Envelope.Payload, correlation: "correlation:wp06/altered", causation: "causation:wp06/altered");
        var context = new DeliveryEvaluationContext(f.Route, f.Admission, altered, f.Policy, 1, null, null,
            DeliveryDestinationHealth.Healthy, Pressure(f), DeliveryTime, "evidence:delivery/envelope-mismatch");
        AssertDecision(new FilMessageDeliveryEvaluator().Evaluate(context), DeliveryDecisionKind.Rejected, MessageDeliveryReason.EnvelopeBindingMismatch);
    }

    private static void CorrelationCausationPreserved()
    {
        var f = Fixture(correlation: "correlation:wp06/preserve", causation: "causation:wp06/source");
        var decision = Evaluate(f);
        var correlationId = f.Envelope.CorrelationId?.Value ?? throw new InvalidOperationException("fixture_correlation_missing");
        var causationId = f.Envelope.CausationId?.Value ?? throw new InvalidOperationException("fixture_causation_missing");
        AssertEqual(correlationId, decision.CorrelationId, "correlation_not_preserved_in_delivery_decision");
        AssertEqual(causationId, decision.CausationId, "causation_not_preserved_in_delivery_decision");
        var outcome = Record(decision, TransportObservationKind.DispatchAccepted, DeliveryTime.AddSeconds(1), "evidence:trace/outcome");
        AssertEqual(decision.CorrelationId, outcome.CorrelationId, "correlation_not_preserved_in_outcome");
        AssertEqual(decision.CausationId, outcome.CausationId, "causation_not_preserved_in_outcome");
    }

    private static void MalformedPressureAuthorityRejected() => AssertPressureAuthorityVariant(PressureAuthorityVariant.Malformed, MessageDeliveryReason.PressureAuthorityMalformed);
    private static void DeniedPressureAuthorityRejected() => AssertPressureAuthorityVariant(PressureAuthorityVariant.Denied, MessageDeliveryReason.PressureAuthorityDenied);
    private static void FuturePressureAuthorityRejected() => AssertPressureAuthorityVariant(PressureAuthorityVariant.Future, MessageDeliveryReason.PressureAuthorityNotYetEffective);
    private static void ExpiredPressureAuthorityRejected() => AssertPressureAuthorityVariant(PressureAuthorityVariant.Expired, MessageDeliveryReason.PressureAuthorityExpired);

    private static void AssertPressureAuthorityVariant(PressureAuthorityVariant variant, string expectedReason)
    {
        var f = Fixture();
        var authority = variant switch
        {
            PressureAuthorityVariant.Malformed => PressureAuthority(f, 10, 3, 3, 1, malformed: true),
            PressureAuthorityVariant.Denied => PressureAuthority(f, 10, 3, 3, 1, decision: AuthorityDecision.Deny),
            PressureAuthorityVariant.Future => PressureAuthority(f, 10, 3, 3, 1, decisionTime: DeliveryTime.AddMinutes(1)),
            PressureAuthorityVariant.Expired => PressureAuthority(f, 10, 3, 3, 1, expiry: DeliveryTime),
            _ => throw new ArgumentOutOfRangeException(nameof(variant))
        };
        var pressure = new DeliveryPressureSnapshot(f.Route.DecisionId, f.Route.ProducerApplicationId,
            10, 0, 3, 0, 3, 0, 1, DeliveryTime.AddSeconds(-1), authority, "evidence:pressure/variant");
        AssertDecision(Evaluate(WithContext(f, pressure: pressure)), DeliveryDecisionKind.Rejected, expectedReason);
    }

    private static void PressureAuthorityLimitMismatchRejected()
    {
        var f = Fixture();
        var authority = PressureAuthority(f, 9, 3, 3, 1);
        var pressure = new DeliveryPressureSnapshot(f.Route.DecisionId, f.Route.ProducerApplicationId,
            10, 0, 3, 0, 3, 0, 1, DeliveryTime.AddSeconds(-1), authority, "evidence:pressure/limit-mismatch");
        AssertDecision(Evaluate(WithContext(f, pressure: pressure)), DeliveryDecisionKind.Rejected, MessageDeliveryReason.PressureAuthorityMismatch);
    }

    private static void FuturePressureObservationRejected()
    {
        var f = Fixture();
        var authority = PressureAuthority(f, 10, 3, 3, 1);
        var pressure = new DeliveryPressureSnapshot(f.Route.DecisionId, f.Route.ProducerApplicationId,
            10, 0, 3, 0, 3, 0, 1, DeliveryTime.AddSeconds(1), authority, "evidence:pressure/future-observation");
        AssertDecision(Evaluate(WithContext(f, pressure: pressure)), DeliveryDecisionKind.Rejected, MessageDeliveryReason.PressureObservationTimeInvalid);
    }

    private static FixtureRecord Fixture(
        string applicationId = "application.alpha",
        string manifestId = "manifest:alpha",
        string producer = "producer:reference/wp06",
        string recipient = "recipient:reference/wp06",
        string consumer = "consumer:reference/wp06",
        string routeId = "route:reference/wp06",
        DeliveryGuarantee guarantee = DeliveryGuarantee.AtLeastOnce,
        int maxAttempts = 3,
        DateTimeOffset? messageExpiry = null,
        string payload = "{\"reference\":\"opaque-payload\"}",
        string correlation = "correlation:wp06/0001",
        string causation = "causation:wp06/0000")
    {
        var schemas = new InMemorySchemaRegistry();
        Assert(schemas.Register(new SchemaDefinition(new SchemaIdentity(SchemaId), "1.0",
            new SchemaOwnerReference("owner:schema/wp06"), SchemaDigest, Evidence("schema/wp06"))).Accepted, "schema_registration_failed");

        var manifests = new InMemoryApplicationCommunicationManifestRegistry(schemas);
        var manifest = Manifest(applicationId, manifestId, consumer);
        var manifestRegistration = manifests.Register(manifest);
        Assert(manifestRegistration.Accepted && manifestRegistration.ManifestSha256 is not null, "manifest_registration_failed");
        var manifestDigest = manifestRegistration.ManifestSha256 ?? throw new InvalidOperationException("manifest_digest_missing_after_accepted_registration");

        var envelope = Envelope(producer, recipient, messageExpiry, payload, correlation, causation);
        var admission = new FilMessageAdmissionEvaluator(manifests, schemas).Evaluate(
            envelope, AdmissionContext(applicationId, manifestId, producer, recipient, consumer));
        Assert(admission.IsAdmitted, $"admission_failed:{admission.Reason}");

        var routeRegistry = new RouteRegistry(manifests);
        Assert(routeRegistry.Register(Route(manifestDigest, applicationId, manifestId, producer, recipient, consumer, routeId)).Accepted,
            "route_registration_failed");
        var route = new RouteSelectionEvaluator().Evaluate(
            new RouteSelectionContext(admission,
                new RoutingMessageTypeBinding(admission.DecisionId, MessageType, Evidence("routing/message-type-binding")),
                RoutePurpose, RoutingTime, null, Evidence("routing/decision")), routeRegistry);
        Assert(route.Decision == RouteSelectionDecision.Selected, $"route_selection_failed:{route.Reason}");

        var policy = Policy(route, guarantee, maxAttempts);
        var seed = new FixtureRecord(envelope, admission, route, policy, null!);
        return seed with { Context = Context(seed) };
    }

    private static FixtureRecord RePolicy(FixtureRecord f, DeliveryPolicy policy)
    {
        var updated = f with { Policy = policy };
        return updated with { Context = Context(updated) };
    }

    private static FixtureRecord WithContext(
        FixtureRecord f,
        int attempt = 1,
        DeliveryAttemptOutcome? previous = null,
        DeliveryIdempotencyBinding? idempotency = null,
        DeliveryDestinationHealth health = DeliveryDestinationHealth.Healthy,
        DeliveryPressureSnapshot? pressure = null,
        DateTimeOffset? observation = null) =>
        f with { Context = Context(f, attempt, previous, idempotency, health, pressure, observation) };

    private static DeliveryEvaluationContext Context(
        FixtureRecord f,
        int attempt = 1,
        DeliveryAttemptOutcome? previous = null,
        DeliveryIdempotencyBinding? idempotency = null,
        DeliveryDestinationHealth health = DeliveryDestinationHealth.Healthy,
        DeliveryPressureSnapshot? pressure = null,
        DateTimeOffset? observation = null) =>
        new(f.Route, f.Admission, f.Envelope, f.Policy, attempt, previous, idempotency, health,
            pressure ?? Pressure(f), observation ?? DeliveryTime, "evidence:delivery/decision");

    private static DeliveryPolicy Policy(
        RouteDecision route,
        DeliveryGuarantee guarantee = DeliveryGuarantee.AtLeastOnce,
        int maxAttempts = 3,
        DeliveryOrderingGuarantee ordering = DeliveryOrderingGuarantee.None,
        string? orderingKey = null,
        bool retryRequiresIdempotency = true,
        bool deadLetter = true,
        DeliveryTrafficClass trafficClass = DeliveryTrafficClass.Normal,
        DeliveryPolicyAuthorityBinding? authority = null,
        string evidence = "evidence:delivery/policy") =>
        new("policy:delivery/wp06", "1.0", route.DecisionId, guarantee, maxAttempts, ordering, orderingKey,
            retryRequiresIdempotency, deadLetter, trafficClass, authority, evidence);

    private static DeliveryPolicyAuthorityBinding PriorityAuthority(
        RouteDecision route,
        DeliveryTrafficClass trafficClass,
        string decision = AuthorityDecision.Allow,
        DateTimeOffset? decisionTime = null,
        DateTimeOffset? expiry = null,
        bool malformed = false,
        string authorizedPolicyId = "policy:delivery/wp06")
    {
        var result = new AuthorityResult(
            "request:delivery-policy/wp06",
            "decision:delivery-policy/wp06",
            decision,
            MessageDeliveryPurpose.GovernedDeliveryPolicy,
            malformed ? "" : "policy:service-bus-delivery",
            "1.0",
            "conditions:bounded-delivery-priority",
            decision == AuthorityDecision.Allow ? "BOUNDED_TO_TECHNICAL_DELIVERY_POLICY" : "NO_ELEVATED_PRIORITY",
            decision == AuthorityDecision.Allow ? AuthorityReason.Allowed : AuthorityReason.DefaultDeny,
            decisionTime ?? DeliveryTime.AddMinutes(-1),
            expiry ?? DeliveryTime.AddMinutes(20),
            "evidence:delivery-priority/result");
        return new DeliveryPolicyAuthorityBinding("authority:delivery-policy/wp06", result,
            authorizedPolicyId, "1.0", route.DecisionId, trafficClass,
            MessageDeliveryPurpose.GovernedDeliveryPolicy, "evidence:delivery-priority/binding");
    }

    private static DeliveryPressureAuthorityBinding PressureAuthority(
        FixtureRecord f,
        int globalLimit,
        int routeLimit,
        int producerLimit,
        int reserved,
        string decision = AuthorityDecision.Allow,
        DateTimeOffset? decisionTime = null,
        DateTimeOffset? expiry = null,
        bool malformed = false,
        string? authorizedRouteDecisionId = null,
        string? authorizedApplicationId = null)
    {
        var result = new AuthorityResult(
            "request:delivery-pressure/wp06",
            "decision:delivery-pressure/wp06",
            decision,
            MessageDeliveryPurpose.GovernedPressureTruth,
            malformed ? "" : "policy:foundation-resource-governance",
            "1.0",
            "conditions:bounded-attributable-pressure-truth",
            decision == AuthorityDecision.Allow ? "BOUNDED_TO_DELIVERY_PRESSURE_LIMITS" : "NO_PRESSURE_AUTHORITY",
            decision == AuthorityDecision.Allow ? AuthorityReason.Allowed : AuthorityReason.DefaultDeny,
            decisionTime ?? DeliveryTime.AddMinutes(-1),
            expiry ?? DeliveryTime.AddMinutes(20),
            "evidence:resource-governance/pressure-result");
        return new DeliveryPressureAuthorityBinding(
            "authority:resource-governance/wp06",
            result,
            authorizedApplicationId ?? f.Route.ProducerApplicationId,
            authorizedRouteDecisionId ?? f.Route.DecisionId,
            globalLimit,
            routeLimit,
            producerLimit,
            reserved,
            MessageDeliveryPurpose.GovernedPressureTruth,
            "restore_when_foundation_pressure_state_rebalances",
            "evidence:resource-governance/pressure-binding");
    }

    private static DeliveryPressureSnapshot Pressure(
        FixtureRecord f,
        int globalLimit = 10,
        int globalInFlight = 0,
        int routeLimit = 3,
        int routeInFlight = 0,
        int producerLimit = 3,
        int producerInFlight = 0,
        int reserved = 1)
    {
        var authority = PressureAuthority(f, globalLimit, routeLimit, producerLimit, reserved);
        return new DeliveryPressureSnapshot(f.Route.DecisionId, f.Route.ProducerApplicationId,
            globalLimit, globalInFlight, routeLimit, routeInFlight, producerLimit, producerInFlight, reserved,
            DeliveryTime.AddSeconds(-1), authority, "evidence:pressure/reference");
    }

    private static DeliveryIdempotencyBinding Idempotency(FixtureRecord f) =>
        new(f.Route.DecisionId, f.Admission.DecisionId, "idempotency:wp06/0001", "evidence:idempotency/wp06");

    private static DeliveryDecision Evaluate(FixtureRecord f) => new FilMessageDeliveryEvaluator().Evaluate(f.Context);

    private static DeliveryAttemptOutcome Record(DeliveryDecision decision, TransportObservationKind observation, DateTimeOffset time, string evidence)
    {
        var result = new DeliveryOutcomeRecorder().Record(decision,
            new TransportOutcomeObservation(decision.RouteDecisionId, decision.PolicyId, decision.PolicyVersion,
                decision.AttemptNumber, observation, time, evidence));
        Assert(result.Accepted, $"delivery_outcome_record_failed:{result.Reason}");
        return result.Outcome ?? throw new InvalidOperationException("delivery_outcome_missing");
    }

    private static ApplicationCommunicationManifest Manifest(string applicationId, string manifestId, string consumer) =>
        new(new ManifestIdentity(manifestId), "1.0", new ApplicationIdentityReference(applicationId), "1.0",
            new ApplicationOwnerReference("owner:application/wp06"),
            new[] { new ManifestReference("CON-004"), new ManifestReference("CON-023") },
            new[] { new ManifestReference("service:fil"), new ManifestReference("service:authority") },
            new[] { new ManifestReference("capability:delivery/reference") },
            new[] { new ManifestReference(consumer) },
            new[] { new AuthorityReference(AdmissionAuthorityRef), new AuthorityReference(RouteAuthorityRef) },
            new[] { new ManifestReference("security:reference") },
            new[] { new ManifestReference("dependency:reference") },
            new[] { new ManifestReference("configuration:reference") },
            new[] { Evidence("manifest/wp06") }, Lifecycle(),
            new[] { new CommunicationDeclaration(MessageType, FilMessageKind.Command, FilMessageClassification.Operational,
                new ManifestSchemaReference(new SchemaIdentity(SchemaId), "1.0"), CommunicationDirection.Outbound, CommunicationRole.Producer) });

    private static CanonicalFilEnvelope Envelope(
        string producer,
        string recipient,
        DateTimeOffset? expiry,
        string payload,
        string correlation = "correlation:wp06/0001",
        string causation = "causation:wp06/0000") =>
        CanonicalFilEnvelope.Create(new MessageIdentity("msg:wp06/0001"), FilMessageKind.Command, FilMessageClassification.Operational,
            MessageType, new SchemaIdentity(SchemaId), "1.0", new ProducerIdentityReference(producer),
            new RecipientScopeReference(recipient), new CorrelationIdentity(correlation), new CausationIdentity(causation),
            new AuthorityReference(AdmissionAuthorityRef), Evidence("message/wp06"), new IdempotencyIdentity("idempotency:wp06/0001"),
            new DeliveryAttemptIdentity("attempt:wp06/0001"), new RetryLineageIdentity("retry-lineage:wp06/0001"),
            new CanonicalMessageTime(AdmissionTime.AddMinutes(-5), expiry ?? AdmissionTime.AddMinutes(30)),
            CanonicalOutcome.Unknown("processing_not_yet_attempted"), payload);

    private static MessageAdmissionContext AdmissionContext(string applicationId, string manifestId, string producer, string recipient, string consumer)
    {
        var authority = new AuthorityResult("request:admission/wp06", "decision:admission/wp06", AuthorityDecision.Allow,
            AdmissionScope, "policy:message-admission", "1.0", "conditions:reference", "BOUNDED_TO_EFFECTIVE_SCOPE",
            AuthorityReason.Allowed, AdmissionTime.AddMinutes(-1), AdmissionTime.AddMinutes(20), "evidence:admission-authority/result");
        return new MessageAdmissionContext(
            new MessageProducerBinding(new ProducerIdentityReference(producer), new ApplicationIdentityReference(applicationId),
                new ManifestIdentity(manifestId), Evidence("producer/binding")), "1.0",
            new MessageRecipientBinding(new RecipientScopeReference(recipient), new ManifestReference(consumer), Evidence("recipient/binding")),
            AdmissionTime,
            new MessageAuthorityBinding(new AuthorityReference(AdmissionAuthorityRef), authority,
                new ProducerIdentityReference(producer), new ApplicationIdentityReference(applicationId),
                new RecipientScopeReference(recipient), MessageAdmissionPurpose.FilMessageAdmission, AdmissionScope,
                Evidence("admission-authority/binding")), Evidence("admission/wp06"));
    }

    private static RouteDeclaration Route(string manifestDigest, string applicationId, string manifestId,
        string producer, string recipient, string consumer, string routeId)
    {
        var authorityResult = new AuthorityResult("request:route/wp06", "decision:route/wp06", AuthorityDecision.Allow,
            RouteScope, "policy:route-selection", "1.0", "conditions:reference", "BOUNDED_TO_ROUTE",
            AuthorityReason.Allowed, RoutingTime.AddMinutes(-1), RoutingTime.AddMinutes(20), "evidence:route-authority/result");
        var authority = new RouteAuthorityBinding(new AuthorityReference(RouteAuthorityRef), authorityResult,
            new RouteIdentity(routeId), "1.0", new ProducerIdentityReference(producer),
            new ApplicationIdentityReference(applicationId), new RecipientScopeReference(recipient),
            new ManifestReference(consumer), MessageType, RoutePurpose, RouteScope, Evidence("route-authority/binding"));
        return new RouteDeclaration(new RouteIdentity(routeId), "1.0", new ManifestIdentity(manifestId), "1.0", manifestDigest,
            new ProducerIdentityReference(producer), new ApplicationIdentityReference(applicationId),
            new RecipientScopeReference(recipient), new ManifestReference(consumer), MessageType,
            new RouteEndpointIdentity("endpoint:source/wp06"), new RouteEndpointIdentity("endpoint:destination/wp06"),
            RoutePurpose, RouteState.Eligible, authority, Evidence("route/wp06"));
    }

    private static ManifestLifecycleDeclaration[] Lifecycle() =>
    [
        new(ManifestLifecycleEvent.ApplicationVersionChange, ManifestApplicabilityRule.RequiresRevalidation),
        new(ManifestLifecycleEvent.Update, ManifestApplicabilityRule.RequiresRevalidation),
        new(ManifestLifecycleEvent.Replacement, ManifestApplicabilityRule.Invalidated),
        new(ManifestLifecycleEvent.Suspension, ManifestApplicabilityRule.RemainsApplicable),
        new(ManifestLifecycleEvent.Removal, ManifestApplicabilityRule.Invalidated)
    ];

    private static ProvenanceReference Evidence(string suffix) => new($"evidence:{suffix}");

    private static void AssertDispatch(DeliveryDecision d) => AssertDecision(d, DeliveryDecisionKind.DispatchEligible, MessageDeliveryReason.DispatchEligible);
    private static void AssertRetry(DeliveryDecision d) => AssertDecision(d, DeliveryDecisionKind.RetryEligible, MessageDeliveryReason.RetryEligible);

    private static void AssertDeadLetter(DeliveryDecision d, string cause)
    {
        Assert(d.Decision == DeliveryDecisionKind.DeadLetter, $"expected_deadletter:{d.Decision}:{d.Reason}");
        Assert(d.Reason.Contains(cause, StringComparison.Ordinal), $"deadletter_reason_missing_cause:{d.Reason}");
    }

    private static void AssertTerminal(DeliveryDecision d, string cause)
    {
        Assert(d.IsTerminal, $"expected_terminal:{d.Decision}:{d.Reason}");
        Assert(d.Reason.Contains(cause, StringComparison.Ordinal), $"terminal_reason_missing_cause:{d.Reason}");
    }

    private static void AssertDecision(DeliveryDecision d, DeliveryDecisionKind expectedDecision, string expectedReason)
    {
        AssertEqual(expectedDecision, d.Decision, "delivery_decision_mismatch");
        AssertEqual(expectedReason, d.Reason, "delivery_reason_mismatch");
    }

    private static void AssertNoPublicSetters(Type type, string label)
    {
        var writable = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.SetMethod is { IsPublic: true }).ToArray();
        Assert(writable.Length == 0, $"{label}_has_public_setter");
    }

    private static void AssertSha256(string value, string message)
    {
        Assert(value.Length == 64, message + ":length");
        Assert(value.All(c => c is >= '0' and <= '9' || c is >= 'A' and <= 'F'), message + ":format");
    }

    private static void AssertThrows<T>(Action action) where T : Exception
    {
        try { action(); }
        catch (T) { return; }
        throw new InvalidOperationException($"expected_exception:{typeof(T).Name}");
    }

    private static void Assert(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    private static void AssertEqual<T>(T expected, T actual, string message)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
            throw new InvalidOperationException($"{message}:expected={expected}:actual={actual}");
    }

    private enum PriorityAuthorityVariant { Malformed, Denied, Future, Expired }
    private enum PressureAuthorityVariant { Malformed, Denied, Future, Expired }

    private sealed record FixtureRecord(
        CanonicalFilEnvelope Envelope,
        MessageAdmissionResult Admission,
        RouteDecision Route,
        DeliveryPolicy Policy,
        DeliveryEvaluationContext Context);
}
