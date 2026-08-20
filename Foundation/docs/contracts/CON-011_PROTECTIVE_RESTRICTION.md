# CON-011 — Protective Restriction Contract

**Identifier:** CON-011  
**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-24  
**Approval Record:** GOV-008  
**Owner:** Falcon Protection Authority  
**Governing Specifications:** AUT-001, AUT-002, SYS-002, OPS-003  
**Applicable ADRs:** ADR-F008  
**Supersedes:** None  
**Superseded By:** None

## 1. Purpose

This Contract defines the binding restriction issued by Guardian and consumed by Authority Engine, Lifecycle, enforcement points, Recovery, and authorized reviewers.

## 2. Participants

- **Issuer:** Guardian operating within an approved mandate.
- **Authority Enforcer:** Authority Engine.
- **State Enforcer:** Lifecycle.
- **Execution Enforcer:** every declared affected enforcement point.
- **Subject:** the component, capability, action, resource, or scope restricted.
- **Recovery Authority:** authorized repair coordinator.
- **Release Authority:** authority permitted to approve release after independent validation.

## 3. Restriction Record

Every restriction SHALL contain:

- restriction ID and version;
- Guardian identity and mandate reference;
- subject identity and affected scope;
- trigger class, evidence references, and decision time;
- protective mode and consequence class;
- prohibited actions;
- explicitly permitted Safe-state actions;
- authority revocations or constraints;
- required Lifecycle target;
- persistence and restart behavior;
- enforcement-point identities;
- issue, effective, and review time;
- release conditions;
- required independent-verification class;
- release-authority identity or role;
- correlation and causation;
- security classification; and
- integrity evidence.

## 4. Authoritative Output

Guardian owns the authoritative restriction. Authority Engine owns resulting authorization decisions. Lifecycle owns resulting lifecycle state. An enforcement point owns its enforcement result. None may rewrite another authority’s fact.

## 5. Obligations

- **CON-011-REQ-001:** A restriction SHALL derive from a valid Guardian mandate and attributable trigger evidence.
- **CON-011-REQ-002:** Authority Engine SHALL deny or constrain affected authority no later than the material execution boundary.
- **CON-011-REQ-003:** Lifecycle SHALL evaluate the required protective transition independently from the subject’s self-report.
- **CON-011-REQ-004:** Unknown restriction or revocation state SHALL fail closed for the affected action.
- **CON-011-REQ-005:** The subject SHALL NOT modify, suppress, expire, release, or bypass its restriction.
- **CON-011-REQ-006:** Unresolved restriction SHALL survive restart with integrity evidence.
- **CON-011-REQ-007:** Safe-state permission SHALL use an explicit allowlist; absence from the allowlist means denial.
- **CON-011-REQ-008:** Repair completion SHALL NOT imply restriction release.
- **CON-011-REQ-009:** Release SHALL require satisfied conditions, independent validation, authorized approval, controlled Lifecycle transition, and a new authority decision.
- **CON-011-REQ-010:** Every issue, enforcement, failure, change, and release result SHALL produce reconstructable evidence.
- **CON-011-REQ-011:** A superseding stricter restriction MAY narrow permitted behavior; a weaker restriction SHALL NOT displace a stronger controlling restriction without lawful release.
- **CON-011-REQ-012:** Expiry by passage of time alone SHALL NOT restore `NORMAL`.

## 6. Rejection and Failure

Invalid mandate, unknown Guardian identity, integrity failure, unsupported restriction version, ambiguous scope, missing release conditions, or unknown enforcement state SHALL prevent permissive interpretation.

## 7. Security and Evidence

Restriction transport SHALL preserve identity, priority authority, integrity, and classification. Protective communication failure SHALL be observable and affected actions SHALL fail closed.

## 8. Compatibility and Evolution

Released restriction meaning is immutable. Corrections or scope changes create linked records. Breaking field meaning requires a new Contract version.

## 9. Acceptance Examples

- valid restriction within mandate: enforced;
- restriction signed by unknown Guardian: rejected and affected scope fails closed;
- subject requests self-release: denied;
- repair succeeds without independent validation: restriction retained;
- valid independent validation and release authority: controlled release begins but authority is not restored until a new decision.

## 10. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner | Approved | GOV-008 | 2026-07-24 |
