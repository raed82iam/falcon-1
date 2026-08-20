# CON-013 — Delegation and Revocation Contract

**Identifier:** CON-013  
**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-25  
**Approval Record:** GOV-026  
**Owner:** Falcon Contract Authority  
**Governing Authority:** Falcon Vision; Falcon Constitution; GOV-AUT-001; AUT-001 v1.1; SEC-002; ADR-I007; ADR-I008; AMD-003; AMD-003-IR-001; CON-012  
**Admission Authority:** CON-000  
**Implementation Authority:** Not Granted

## 1. Purpose

This Contract defines how existing authority is delegated, accepted, constrained, redelegated, suspended, revoked, expired, terminated, propagated, challenged, and reconstructed.

Delegation transfers a bounded ability to exercise existing authority. It does not transfer or create jurisdiction.

## 2. Participants

- **Delegator:** the Authority Holder lawfully delegating authority.
- **Delegate:** the identified proposed recipient.
- **Delegation Authority:** the competent authority governing delegation where separate approval is required.
- **Authority Registry:** the governed participant preserving delegation state.
- **Authority Evaluator:** the participant verifying the effective chain.
- **Revocation Authority:** the competent authority permitted to revoke.
- **Protective Authority:** the authority permitted to suspend or restrict for protection.
- **Review Authority:** the competent independent authority resolving material challenges.

## 3. Authority

A delegation is valid only when:

- jurisdiction already exists;
- the delegator holds active, delegable authority;
- the governing Instrument permits the intended delegation;
- the delegated scope does not exceed the delegator's exercisable scope;
- the delegation preserves every upstream restriction;
- the Delegate is eligible;
- required acceptance and independence are satisfied; and
- the complete delegation chain is preserved.

## 4. Delegation Request

A Delegation Request SHALL contain:

- request ID;
- proposed Delegation ID;
- delegator identity;
- delegate identity;
- source Authority Instrument ID and version;
- jurisdiction ID;
- decision classes;
- action, subject, resource, and purpose scope;
- environment scope;
- consequence ceiling;
- effective time and expiry;
- conditions, constraints, and prohibitions;
- redelegation rule;
- oversight and reporting obligations;
- suspension, revocation, and termination rules;
- independence requirements;
- evidence obligations;
- reason;
- correlation and causation;
- time observation; and
- governing context reference.

## 5. Delegation Result

A Delegation Result SHALL contain:

- request ID;
- Delegation ID;
- `ACCEPTED` or `REJECTED`;
- effective delegated scope;
- preserved upstream constraints;
- delegation depth;
- redelegation permission and limit;
- lifecycle state;
- effective time and expiry;
- decision authority;
- governing policy and context;
- reasons and conditions;
- canonical digest;
- evidence reference; and
- acceptance reference where required.

An accepted result SHALL be represented by an Authority Instrument governed by CON-012.

## 6. Delegate Acceptance

Where delegation creates accountability or active duties, the Delegate SHALL explicitly accept it before activation.

Acceptance SHALL:

- identify the exact Delegation ID and version;
- confirm identity;
- confirm understood scope and obligations;
- state acceptance or rejection;
- be time-bounded;
- preserve evidence; and
- not modify the proposed authority.

Silence, possession, access, or performance SHALL NOT constitute acceptance.

## 7. Redelegation

Redelegation is denied by default.

When expressly allowed:

- every upstream Instrument SHALL permit it;
- the new scope SHALL be narrower than or equal to every applicable upstream scope;
- all upstream constraints SHALL remain binding;
- delegation depth SHALL remain within the approved limit;
- expiry SHALL be no later than the earliest upstream expiry;
- the final acting delegate SHALL be identifiable;
- every upstream competent authority SHALL retain applicable revocation power; and
- the complete chain SHALL remain reconstructable.

No number or combination of delegation layers may increase authority.

## 8. Suspension and Revocation

A suspension or revocation request SHALL identify:

- target Delegation ID and version;
- requesting actor;
- competent authority;
- action requested;
- reason;
- effective time;
- propagation scope;
- emergency status where applicable;
- evidence;
- notification obligations; and
- review path.

Suspension prevents exercise without erasing the delegation record.

Revocation withdraws delegated authority according to the governing Instrument.

Emergency protective suspension MAY precede ordinary review only when explicitly authorized, minimally scoped, time-bounded, evidenced, and promptly reviewed.

## 9. Propagation

Suspension, revocation, expiry, termination, restriction, or loss of trust in an upstream authority SHALL propagate to all dependent delegation branches that rely on the affected scope.

Propagation SHALL:

- be monotonic toward equal or narrower authority;
- identify every known dependent branch;
- prevent new exercise immediately according to consequence policy;
- preserve in-flight action policy explicitly;
- produce propagation evidence;
- reconcile delayed or unreachable participants; and
- never silently restore authority.

Unknown propagation status SHALL cause restriction.

## 10. Restoration

Revoked authority SHALL NOT be restored by reversing or deleting the revocation record.

Restoration requires:

- a new Authority Instrument or explicitly authorized restoration Instrument;
- proof that the triggering condition is resolved or contained;
- verification of current jurisdiction, chain, identity, conditions, and policy;
- evaluation of residual uncertainty;
- the required independent confirmation;
- a newly declared scope; and
- immutable linkage to prior events.

## 11. Preconditions

Before activation:

- delegator identity and source authority SHALL be verified;
- the source SHALL be `ACTIVE` or explicitly delegable within `RESTRICTED`;
- jurisdiction SHALL be valid;
- delegation rights SHALL be explicit;
- scope SHALL be bounded;
- all upstream constraints SHALL be preserved;
- the Delegate SHALL be eligible and accepted where required;
- conflicts and independence conditions SHALL be governed;
- no higher restriction SHALL prohibit activation; and
- the record SHALL pass integrity and canonical validation.

## 12. Postconditions

After acceptance:

- the Delegation ID and lifecycle are unambiguous;
- the effective authority is no broader than its source;
- every upstream constraint remains discoverable and enforceable;
- suspension, revocation, expiry, and termination paths are known;
- the complete chain is reconstructable;
- applicable participants can discover the current state; and
- immutable evidence is preserved.

## 13. Invariants

- Delegation SHALL NOT create jurisdiction.
- A delegate SHALL receive no more authority than the delegator can lawfully exercise.
- Redelegation SHALL be denied unless explicitly allowed.
- Upstream constraints SHALL survive every delegation layer.
- Downstream authority SHALL NOT outlive its source.
- Delegator accountability SHALL remain unless higher authority explicitly provides otherwise.
- Revocation SHALL NOT erase history.
- A revoked branch SHALL NOT reactivate through retry, cache, replay, or stale state.
- Unknown state SHALL NOT produce permission.
- Delegation SHALL NOT bypass separation of duties or independent review.

## 14. Rejection and Failure

A delegation SHALL be rejected or restricted when:

- jurisdiction or source authority cannot be verified;
- the delegator lacks delegation rights;
- proposed scope exceeds the source;
- a prohibited decision class is included;
- required acceptance is absent;
- redelegation is not explicitly allowed;
- delegation depth exceeds its limit;
- independence or conflict-of-interest requirements fail;
- the source is non-operative;
- integrity, time, or evidence is materially uncertain;
- revocation status cannot be determined; or
- a higher protective restriction applies.

Failure to propagate a material restriction SHALL trigger protective escalation and evidence preservation.

## 15. Compatibility

- Delegation semantics SHALL remain independent of transport, storage, runtime, or vendor.
- Unknown mandatory fields or unsupported governing versions SHALL cause rejection or restriction.
- New optional fields MAY be ignored only when declared non-material by governing policy.
- A Delegation ID SHALL NOT be reassigned or repurposed.
- Correction SHALL create a new immutable version with lineage.
- Cached delegation state SHALL obey approved maximum-age, time-quality, and revocation rules.

## 16. Evidence

Evidence SHALL preserve:

- request and result;
- source Authority Instrument;
- complete Authority Chain;
- delegator and delegate identities;
- acceptance;
- effective scope;
- all upstream conditions and restrictions;
- lifecycle events;
- propagation attempts and outcomes;
- delayed or failed propagation;
- exercises affected by a state change;
- challenges and resolutions;
- restoration;
- governing policy and context;
- time quality;
- integrity proof; and
- accountable authorities.

Delegation evidence is governed as Trust Objects under SEC-002.

## 17. Security

- Delegation, revocation, and restoration messages SHALL be authenticated, integrity-protected, replay-resistant, and time-bounded.
- Confidential fields SHALL be protected according to classification.
- Rollback, stale-cache use, equivocation, duplicate effects, and forged lineage SHALL be detected and rejected.
- Revocation information SHALL receive availability and propagation protection proportionate to consequence.
- Signature validity SHALL NOT substitute for jurisdiction, competence, current state, or policy validity.
- A compromised participant SHALL NOT restore its own unrestricted authority in a material case.

## 18. Normative Requirements

- **CON-013-REQ-001:** Every delegation SHALL reference an existing valid jurisdiction and source Authority Instrument.
- **CON-013-REQ-002:** Delegated authority SHALL NOT exceed the delegator's exercisable authority.
- **CON-013-REQ-003:** Every upstream constraint SHALL remain binding through all delegation layers.
- **CON-013-REQ-004:** Redelegation SHALL be denied unless explicitly permitted by every applicable upstream Instrument.
- **CON-013-REQ-005:** Delegation SHALL NOT create, widen, merge, transfer, or reinterpret jurisdiction.
- **CON-013-REQ-006:** Material accountability SHALL require explicit Delegate acceptance where governing policy requires it.
- **CON-013-REQ-007:** Silence or technical capability SHALL NOT constitute acceptance.
- **CON-013-REQ-008:** Downstream authority SHALL terminate or restrict no later than its controlling source.
- **CON-013-REQ-009:** Upstream adverse state SHALL propagate to every dependent branch.
- **CON-013-REQ-010:** Unknown revocation or propagation status SHALL reduce authority.
- **CON-013-REQ-011:** Revocation SHALL preserve history and prevent stale or replayed reactivation.
- **CON-013-REQ-012:** Restoration SHALL require new authority evidence and required independent confirmation.
- **CON-013-REQ-013:** Delegation SHALL NOT bypass independence, separation-of-duty, or challenge requirements.
- **CON-013-REQ-014:** Delegation and lifecycle evidence SHALL be immutable, attributable, and reconstructable.
- **CON-013-REQ-015:** A material propagation failure SHALL trigger protective escalation.
- **CON-013-REQ-016:** A delegate SHALL NOT alter the rules governing its own delegation.

## 19. Acceptance Examples

Acceptance requires verified examples showing:

- valid bounded delegation;
- rejection without jurisdiction;
- rejection when the delegator lacks delegation rights;
- rejection of wider purpose, action, environment, consequence, or duration;
- explicit acceptance where required;
- default denial of redelegation;
- permitted redelegation preserving all upstream constraints;
- propagation of suspension, revocation, expiry, and termination;
- restriction when propagation status is unknown;
- rejection of stale, replayed, or modified delegation state;
- immediate prevention of new exercise after material revocation;
- restoration through a new independently confirmed decision;
- preservation of historical records; and
- complete Authority Chain reconstruction.

## 20. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner and current Constitutional Authority | Approved | GOV-026 | 2026-07-25 |

This Approval admits CON-013 as a governed Foundation Contract. It does not delegate, suspend, revoke, restore, or activate authority; authorize implementation; or authorize financial activity.
