# CON-020 — Bootstrap Execution Context Contract

**Identifier:** CON-020  
**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-25  
**Approval Record:** GOV-029  
**Owner:** Falcon Contract Authority  
**Governing Authority:** Falcon Vision; Falcon Constitution; GOV-AUT-001; AUT-001; SEC-001; SEC-002; ENV-001; ADR-I008; AMD-003; AMD-003-IR-001; CON-012; CON-013  
**Admission Authority:** CON-000  
**Implementation Authority:** Not Granted

## 1. Purpose

This Contract defines the bounded execution context in which Foundation preparation and enabling-provider candidate verification may occur before Falcon operational identity, time, custody, environments, and Pipeline are active.

A Bootstrap Execution Context permits only the work stated by its Authority Instrument. It is not a Falcon operational Security Context, an active Environment Profile, implementation authority, or financial authority.

## 2. Participants

- **Context Issuer:** the competent authority issuing the context.
- **Authority Holder:** the identified human or governed runner permitted to use it.
- **Environment Controller:** the participant establishing and isolating the candidate environment.
- **Bootstrap Harness:** the bounded orchestration participant.
- **Candidate Subject:** the exact tool, Provider, profile, or artifact under preparation or verification.
- **External Bootstrap Control:** the independent source of environment, input, execution, and evidence identity.
- **Protective Authority:** Guardian or another competent authority enforcing stop conditions.
- **Evidence Authority:** the participant preserving context and execution evidence.

No participant acquires authority from access, technical capability, or possession of the context.

## 3. Context Manifest

Every immutable Context Manifest SHALL contain:

- Context ID and version;
- Context class: `PREPARATION` or `CANDIDATE_PROVIDER_VERIFY`;
- lifecycle state;
- Authority Instrument ID and complete Authority Chain;
- Authority Holder identity;
- environment candidate identity;
- external environment instance ID marked `BOOTSTRAP_EXTERNAL_ID`;
- Runtime Epoch or external continuity boundary;
- exact candidate subjects and versions;
- exact permitted actions;
- exact prohibited actions;
- tool and dependency bundle identities and digests;
- input and output classifications;
- data, secret, key, certificate, and identity classes;
- network and storage boundaries;
- bootstrap identity, time, and evidence mechanisms;
- synthetic-material requirements;
- start, expiry, and maximum duration;
- resource and consequence limits;
- stop conditions;
- cleanup and destruction obligations;
- evidence export destination;
- challenge and review path;
- governing policy and profile versions;
- canonical digest; and
- integrity protection.

## 4. Permitted Scope

Where explicitly listed in the Authority Instrument, a context MAY permit:

- exact tool and dependency acquisition and verification;
- offline-bundle preparation;
- candidate image and environment preparation;
- capability probing;
- non-behavioral provisioning definitions;
- evidence-capture preparation;
- canonical encoding and Trust Object primitives required by candidate verification;
- construction and testing of explicitly enumerated enabling Provider candidates;
- machine-readable traceability expansion support;
- bootstrap harness execution;
- isolated fault injection; and
- evidence export for independent evaluation.

## 5. Prohibited Scope

A Bootstrap Execution Context SHALL NOT permit:

- general Falcon Core or domain implementation;
- Guardian, trading, portfolio, broker, strategy, or financial behavior;
- production data, credentials, roots, keys, certificates, or secrets;
- financial connectivity;
- Profile Activation;
- release promotion;
- operational identity or `VERIFIED` time issuance;
- operational trust or authority;
- candidate self-approval;
- unrestricted network or storage access;
- scope expansion through retry or discovery; or
- use after expiry, revocation, stop, or unresolved material challenge.

## 6. External Identity and Time

Before Provider Activation:

- environment and execution identities SHALL be external and marked `BOOTSTRAP_EXTERNAL_ID`;
- time observations SHALL be marked `BOOTSTRAP_EXTERNAL`;
- issuer, scheme, source, environment, resolution, uncertainty, and continuity limits SHALL be preserved;
- external values SHALL NOT be represented as Falcon operational identity, Runtime Epoch, or `VERIFIED` time; and
- later Falcon identifiers SHALL cross-link rather than replace historical external identifiers.

## 7. Candidate Isolation

Candidate subjects SHALL:

- remain content-identified;
- run only in their declared context;
- receive synthetic and non-production material;
- have no operational dependency or authority path;
- not validate their own Activation;
- not serve as the sole control for their own evidence;
- export only governed evidence and permitted outputs; and
- be removed, retained, or quarantined according to declared cleanup policy.

## 8. Execution Request and Result

An execution request SHALL identify the Context, Authority Instrument, actor, candidate subject, exact action, inputs, tools, environment, expected outputs, time, correlation, and evidence obligations.

An execution result SHALL contain:

- execution ID;
- `COMPLETED`, `REJECTED`, `FAILED`, or `STOPPED`;
- exact context and subject identities;
- observed actions and outputs;
- external identity and time references;
- isolation and boundary disposition;
- stop-condition disposition;
- cleanup disposition;
- evidence-set reference;
- bounded reasons; and
- no claim of Activation or operational fitness.

## 9. Stop and Failure

Execution SHALL stop and preserve evidence when:

- authority is missing, expired, revoked, or exceeded;
- the context or subject identity changes;
- scope expands beyond the manifest;
- production material or financial connectivity appears;
- isolation, provenance, integrity, or evidence retention fails;
- bootstrap identity or time is misclassified;
- a candidate is treated as active;
- self-certification is attempted;
- a non-waivable Gate fails; or
- a material Challenge remains unresolved.

Failure SHALL NOT broaden authority or trigger an unapproved fallback environment.

## 10. Compatibility and Evidence

The Contract SHALL remain independent of operating system, runtime, container, hypervisor, cloud, orchestration tool, storage, network, and vendor.

Platform-specific details SHALL remain in the governed environment or Adapter profile.

Evidence SHALL preserve the complete Context Manifest, authority, actor, environment, tools, inputs, outputs, external identity and time, isolation, execution observations, failures, cleanup, exports, challenges, and responsible authorities.

## 11. Normative Requirements

- **CON-020-REQ-001:** Every preparation or candidate execution SHALL reference one immutable Bootstrap Execution Context.
- **CON-020-REQ-002:** Every Context SHALL reference a valid bounded Authority Instrument.
- **CON-020-REQ-003:** Permitted subjects and actions SHALL be enumerated exactly.
- **CON-020-REQ-004:** Unlisted subjects and actions SHALL be prohibited.
- **CON-020-REQ-005:** Bootstrap identity and time SHALL remain external and explicitly classified.
- **CON-020-REQ-006:** Candidate subjects and materials SHALL remain isolated and non-operational.
- **CON-020-REQ-007:** Production and financial data, credentials, connectivity, and consequences SHALL be prohibited.
- **CON-020-REQ-008:** Candidate output SHALL NOT establish Activation, trust, authority, or operational fitness.
- **CON-020-REQ-009:** External Bootstrap Control SHALL remain distinguishable from candidate-produced observations.
- **CON-020-REQ-010:** Material scope, authority, isolation, provenance, integrity, or evidence failure SHALL stop execution.
- **CON-020-REQ-011:** Failure SHALL NOT expand scope or invoke an unapproved fallback.
- **CON-020-REQ-012:** Cleanup and evidence export SHALL be explicit and attributable.
- **CON-020-REQ-013:** Context expiry, suspension, or revocation SHALL prevent new execution.
- **CON-020-REQ-014:** Execution evidence SHALL be immutable and reconstructable.
- **CON-020-REQ-015:** Platform details SHALL NOT redefine the Contract.

## 12. Acceptance Examples

Acceptance requires bounded preparation; bounded Provider-candidate verification; rejection of unlisted tools, actions, and subjects; external identity and time classification; synthetic-material enforcement; network and storage isolation; production and financial path detection; expiry and revocation; stop-condition enforcement; failure without fallback; cleanup; evidence export; independent reconstruction; and inability to claim Activation.

## 13. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner and current Constitutional Authority | Approved | GOV-029 | 2026-07-25 |

This Approval admits CON-020 as a governed Foundation Contract. It does not issue a Context or Authority Instrument, create an environment, execute preparation, authorize implementation, or authorize financial activity.
