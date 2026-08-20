# PLG-001 — Capability Passport and Admission

**Identifier:** PLG-001  
**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-24  
**Approval Record:** GOV-003
**Owner:** Falcon Capability Ecosystem Authority  
**Governing Authority:** Constitution Articles 26, 33–36D  
**Affected Domains:** SYS, SEC, AUT, AWR, EVO, OPS, APP, EXT

## 1. Purpose

This Specification defines the identity, evidence, authority, and admission requirements for every replaceable capability entering Falcon.

It establishes the principle:

> Plug, Verify, Govern, Then Play.

## 2. Scope

PLG-001 governs:

- Capability Passports;
- discovery and identity;
- artifact provenance and integrity;
- compatibility;
- declared contracts;
- permissions and resources;
- health, failure, recovery, update, and removal obligations;
- admission stages;
- isolation and observation; and
- promotion or rejection.

## 3. Non-Scope

This Specification does not:

- prescribe one plugin technology;
- make every Core component replaceable;
- grant authority by installation;
- permit direct hidden communication;
- approve a capability based only on compatibility;
- define domain behavior; or
- waive applicable Specifications.

## 4. Capability Passport

Every replaceable capability SHALL declare:

- unique identity and version;
- accountable owner and producer;
- purpose and component class;
- artifact digest, signature, provenance, and build identity;
- provided and consumed contracts;
- required permissions and their purpose;
- data read, written, retained, and exported;
- dependencies and compatibility;
- resource budgets;
- lifecycle and health contract;
- failure, isolation, recovery, and rollback behavior;
- security classification and trust assumptions;
- capital and decision impact;
- autonomy level;
- update, state-migration, removal, and retirement behavior; and
- approval and evidence references.

## 5. Admission Stages

Admission SHALL distinguish:

1. `DISCOVERED`;
2. `IDENTIFIED`;
3. `VERIFIED`;
4. `COMPATIBLE`;
5. `AUTHORIZED`;
6. `ISOLATED`;
7. `OBSERVED`;
8. `PROMOTED`;
9. `RESTRICTED`;
10. `REJECTED`; and
11. `RETIRED`.

No stage implies a later stage.

## 6. Normative Requirements

- **PLG-001-REQ-001:** Falcon SHALL reject a capability without a complete and verifiable Passport.
- **PLG-001-REQ-002:** Capability identity SHALL be bound to the admitted artifact and SHALL NOT rely on a mutable display name.
- **PLG-001-REQ-003:** Artifact provenance, integrity, signature, and dependency evidence SHALL be verified before execution.
- **PLG-001-REQ-004:** Technical compatibility SHALL NOT grant permission or trust.
- **PLG-001-REQ-005:** Permissions SHALL be explicit, purpose-bound, minimal, time-bounded where applicable, and authorized through AUT-001.
- **PLG-001-REQ-006:** A capability SHALL NOT access undeclared data, authority, resources, or communication paths.
- **PLG-001-REQ-007:** Admission SHALL identify the maximum plausible capital, security, integrity, and continuity consequence of capability failure.
- **PLG-001-REQ-008:** Initial execution SHALL occur within isolation proportionate to consequence.
- **PLG-001-REQ-009:** A capability SHALL demonstrate its health, failure, and removal contracts before promotion.
- **PLG-001-REQ-010:** A capability capable of financial recommendation, decision, or action SHALL operate initially without irreversible authority.
- **PLG-001-REQ-011:** Promotion SHALL require evidence from observation and all applicable Specifications.
- **PLG-001-REQ-012:** A capability SHALL remain interruptible, restrictable, and removable through authority independent of the capability.
- **PLG-001-REQ-013:** Update SHALL be treated as admission of a new artifact identity.
- **PLG-001-REQ-014:** State migration SHALL preserve ownership, integrity, compatibility, and rollback obligations.
- **PLG-001-REQ-015:** Removal SHALL revoke permissions, release resources, preserve required state and evidence, and eliminate hidden dependencies.
- **PLG-001-REQ-016:** A capability SHALL NOT make its continued presence a concealed prerequisite for unrelated Falcon operation.
- **PLG-001-REQ-017:** Passport changes SHALL be versioned, attributable, and re-evaluated for admission impact.
- **PLG-001-REQ-018:** Falcon SHALL continuously verify that runtime behavior remains within the admitted Passport.

## 7. Falcon Cells

A replaceable capability MAY operate within a Falcon Cell that provides:

- identity and permission isolation;
- data and communication boundaries;
- resource budgets;
- lifecycle control;
- health and evidence collection;
- independent restriction and termination; and
- bounded failure propagation.

The realization of Falcon Cells requires an ADR. The isolation obligations do not.

## 8. Acceptance Evidence

Approval requires evidence for:

- artifact and Passport binding;
- signature and provenance rejection;
- least-authority enforcement;
- undeclared-access denial;
- isolation and resource containment;
- Shadow admission for financial capability;
- safe update, migration, rollback, and removal; and
- runtime drift detection.

## 9. ADR Candidates

- Plugin packaging and manifest format;
- Falcon Cell isolation technology;
- signing and producer trust model;
- capability discovery mechanism;
- compatibility negotiation; and
- artifact repository.

## 10. Unresolved Matters

- Capability consequence classes.
- Core responsibilities that are constitutionally non-pluggable.
