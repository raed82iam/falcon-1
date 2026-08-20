# CON-022 — Application Guardian Protection Request

**Identifier:** CON-022 — Proposed Reservation  
**Version:** Proposed 1.0  
**Status:** Approved Contract Design — Not Effective  
**Approval Record:** GOV-062  
**Governing Specifications:** Proposed AUT-002 v2.1; proposed RSK-006  
**Governing ADR:** Proposed ADR-I011  
**Stage 1 Authority:** Not Granted

## 1. Purpose

This Contract defines the technical request by which an authorized Application Guardian asks FFG to investigate or impose Foundation or cross-Application technical protection.

The request is not a command, authority grant, business payload, or proof that the requested action is justified.

## 2. Participants

- **Requester:** Approved Application Guardian.
- **Receiver and decision owner:** FFG.
- **Authority validator:** AUT-001.
- **Evidence contributors:** FSA and competent technical observers.
- **Execution owners:** competent Foundation mechanisms.

## 3. Request Fields

Every request SHALL contain:

- Request ID and Contract version;
- requester Guardian and Application Suite identity;
- requester authority and mandate reference;
- affected Application/capability;
- suspected technical source;
- observed technical effect;
- severity, urgency, confidence, and uncertainty;
- required technical capabilities;
- requested action and scope;
- evidence references;
- technical-criticality reference;
- maximum response time;
- requested duration;
- proposed release conditions;
- trusted timestamp and correlation ID;
- integrity and replay-protection data; and
- FIL classification and routing metadata.

## 4. Business-Payload Prohibition

The Contract SHALL NOT carry trade value, portfolio value, position detail, order detail, customer detail, accounting record, patient record, inventory record, strategy, or other Application business payload.

A requester SHALL translate domain danger into the minimum technical effect required for FFG evaluation.

## 5. Requested Actions

Allowed request classes include:

- investigate;
- increase monitoring;
- preserve technical capability;
- protect or prioritize resources;
- restrict traffic;
- isolate component;
- isolate Application;
- enter Platform Containment; and
- enter Platform Safe Mode.

The requester does not acquire authority to perform the requested action.

## 6. FFG Responses

Responses SHALL distinguish:

- `REQUEST_ACCEPTED`;
- `REQUEST_ACCEPTED_WITH_NARROWER_SCOPE`;
- `REQUEST_ACCEPTED_WITH_STRONGER_ACTION`;
- `INVESTIGATION_STARTED`;
- `MORE_EVIDENCE_REQUIRED`;
- `REQUEST_REJECTED`;
- `PROVISIONAL_CONTAINMENT_APPLIED`;
- `COMPONENT_ISOLATED`;
- `APPLICATION_ISOLATED`;
- `PLATFORM_CONTAINMENT_ACTIVATED`;
- `PLATFORM_SAFE_ACTIVATED`; and
- `NO_TECHNICAL_THREAT_CONFIRMED`.

A response describes FFG’s decision state. It does not prove execution success.

## 7. Validation and Decision

FFG SHALL independently validate requester identity and authority, request integrity and freshness, evidence integrity, technical effect, suspected source, dependencies, criticality, feasibility, proportionality, reversibility, affected Applications, current Platform mode, and conflicting requests.

Unknown or invalid identity, authority, integrity, version, scope, evidence, or replay state SHALL reject permissive interpretation.

## 8. Provisional Containment

FFG MAY impose pre-authorized provisional containment when delay may cause severe harm.

The response SHALL record authority, scope, evidence, start, expiry or review time, reversibility, escalation, and unresolved uncertainty.

Expiry SHALL not silently release unresolved danger; it triggers the governing fail-safe and review policy.

## 9. Lifecycle

```text
CREATED
  → VALIDATED | REJECTED
  → INVESTIGATING | DECIDED
  → DIRECTIVE_ISSUED | NO_ACTION
  → EXECUTION_OBSERVED
  → RECOVERY_COORDINATING
  → CLOSED | SUPERSEDED | ESCALATED
```

Request, decision, directive, execution, recovery, and release SHALL remain separate records.

## 10. Time, Ordering, and Replay

Requests SHALL expire according to consequence policy. Arrival order SHALL NOT be assumed unless the Contract profile explicitly guarantees it. Duplicate requests SHALL be reconciled by Request ID and correlation identity without duplicate effects.

## 11. FIL and Service Bus

The request and response SHALL use an Approved FIL envelope, authenticated sender/receiver identities, authorized protected Service Bus route, integrity protection, expiry, replay defense, classification, and evidence correlation.

Transport does not create authority or change Contract meaning.

## 12. Abuse Prevention

Requests SHALL be rate-limited, attributable, independently reviewable, and challengeable.

Repeated unsupported or conflicting requests SHALL be observable as fault, compromise, or policy violation. FFG SHALL NOT rely exclusively on the requester.

## 13. Release Coordination

FFG controls Platform restriction release. The Application Guardian controls its domain restriction within mandate.

The Contract SHALL correlate Foundation and domain recovery evidence without merging their decisions.

## 14. Evidence

Evidence SHALL preserve request, validation, decision, independent sources, contradictions, directive, execution outcome, affected scope, provisional status, recovery, releases, challenges, and integrity/provenance.

## 15. Compatibility and Versioning

Unknown major versions SHALL be rejected. Minor evolution may add optional fields without changing authority, business-payload prohibition, response meaning, or failure semantics.

## 16. Normative Requirements

- **CON-022-REQ-001:** A request SHALL be authenticated, authorized, attributable, fresh, integrity-protected, and evidence-linked.
- **CON-022-REQ-002:** A request SHALL NOT command FFG or create cross-Application authority.
- **CON-022-REQ-003:** Business payload SHALL be prohibited.
- **CON-022-REQ-004:** FFG SHALL independently validate every request.
- **CON-022-REQ-005:** FFG MAY reject, investigate, narrow, accept, or strengthen the requested action.
- **CON-022-REQ-006:** Decision and execution outcomes SHALL remain distinct.
- **CON-022-REQ-007:** Ordering SHALL not be assumed and duplicate effects SHALL be prevented.
- **CON-022-REQ-008:** Platform and domain releases SHALL remain separate.
- **CON-022-REQ-009:** Abuse and unsupported repeated requests SHALL be observable.
- **CON-022-REQ-010:** Every lifecycle transition SHALL be reconstructable.
