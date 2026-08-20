# CON-022 — Application Guardian Protection Request

**Identifier:** CON-022 — Proposed Reservation  
**Version:** Proposed 1.1  
**Status:** Proposed  
**Refines:** CON-022 v1.0 architectural candidate  
**Stage 1 Authority:** Not Granted

## Purpose

Define the generic, domain-independent request by which a CON-024-registered Application Guardian asks FFG for technical investigation or protection.

The request is not a command, authority grant, execution result, or proof of danger.

## Request Types

- `REQUEST_TECHNICAL_INVESTIGATION`;
- `REQUEST_INCREASED_MONITORING`;
- `REQUEST_RESOURCE_PROTECTION`;
- `REQUEST_PRIORITY_PROTECTION`;
- `REQUEST_TRAFFIC_RESTRICTION`;
- `REQUEST_COMPONENT_ISOLATION`;
- `REQUEST_APPLICATION_ISOLATION`;
- `REQUEST_PLATFORM_CONTAINMENT`;
- `REQUEST_PLATFORM_SAFE`.

## Mandatory Request

Request ID/version/type, correlation/causation, registered Guardian/Suite identity, CON-024 registration, authority/mandate, affected technical capability, suspected source, observed technical effect, severity, urgency, required capability, requested scope, evidence references, approved technical-criticality reference, confidence, uncertainty, maximum response time, requested duration, proposed release conditions, trusted time/uncertainty, FIL/security profile, priority authority, integrity, replay, and delivery-attempt identity.

## Business-Payload Prohibition

No trade, portfolio, position, order, invoice, patient, inventory, customer, strategy, or other domain record or meaning may be carried. The Guardian supplies the minimum technical effect and protected evidence references.

## Validation

FFG SHALL independently validate registration, identity, authority, integrity, freshness, replay, evidence, technical source/effect, dependencies, criticality, feasibility, proportionality, reversibility, conflicts, Platform state, and consequences.

Invalid or unknown mandatory state fails closed.

## Decision Responses

`REQUEST_ACCEPTED`, `REQUEST_ACCEPTED_WITH_NARROWER_SCOPE`, `REQUEST_ACCEPTED_WITH_STRONGER_ACTION`, `INVESTIGATION_STARTED`, `MORE_EVIDENCE_REQUIRED`, `REQUEST_REJECTED`, `PROVISIONAL_CONTAINMENT_APPLIED`, `NO_TECHNICAL_THREAT_CONFIRMED`.

Directive, execution, recovery, and release results SHALL use their competent Contracts and SHALL not be conflated with the response.

## Delivery Outcomes

Every request requires attributable `ACK_ACCEPTED_FOR_EVALUATION`, `ACK_REJECTED`, `ACK_DUPLICATE`, `ACK_EXPIRED`, or `ACK_UNDELIVERABLE`.

Undeliverable or repeatedly failing requests enter a governed dead-letter path preserving original identity, authority, integrity, attempts, expiry, and evidence. Dead-letter presence does not authorize action.

## Priority and Emergency Route

Priority requires separate authority. Emergency-route eligibility SHALL be explicit in CON-024 and the active profile. The route may reduce latency but SHALL NOT bypass identity, authority, integrity, replay, business-payload prohibition, FFG evaluation, or evidence.

Response-time violation triggers escalation and evidence; it does not automatically accept the request.

## Provisional Containment

Only pre-authorized consequence classes may use provisional containment. Scope, authority, evidence, start, review/expiry, reversibility, escalation, and persisted restriction state are mandatory. Review delay cannot make it permanent or silently release it.

## Ordering and Duplicates

Arrival order is not assumed. Request ID, correlation, state version, and idempotency prevent duplicate effects. Reordered, superseded, or conflicting requests remain visible and are reconciled explicitly.

## Audit and Evidence

CON-031 SHALL preserve every attempt, acknowledgment, dead-letter record, validation, independent evidence, contradiction, decision, directive reference, execution reference, recovery, release correlation, challenge, and final disposition.

## Release

FFG owns Platform restriction conditions. The Application Guardian owns its domain restriction conditions. Separate competent authorities authorize releases. Neither normal state implies the other.

## Compatibility

Unknown major versions fail closed. Minor versions may add optional non-authority fields only. No version may weaken registration, authority, payload prohibition, independent evaluation, or evidence.

## Acceptance

All request types; invalid registration/authority/integrity/expiry/replay; duplicate/reorder; acknowledgment/dead-letter; unauthorized priority; emergency route; unsupported request; narrow/strong action; provisional expiry; abuse/rate limits; separate execution/release; and payload-exclusion proof.

