# ADR-F008 — Safe-State Enforcement Boundary

**Identifier:** ADR-F008  
**Version:** 1.0  
**Status:** Accepted  
**Date:** 2026-07-24  
**Decision Owner:** Falcon Project Owner  
**Scope:** Foundation protective restriction, Safe-state enforcement, and controlled release  
**Affected Specifications:** AUT-001, AUT-002, SYS-001, SYS-002, OPS-003, RSK-005, FRS-001  
**Applicable Standards:** STD-003  
**Related ADRs:** ADR-F001, ADR-F002, ADR-F003, ADR-F006, ADR-F007  
**Supersedes:** None  
**Superseded By:** None  
**Decision Record:** Project Owner approval recorded on 2026-07-24

## 1. Context

Guardian protection is meaningful only if a component subject to restriction cannot ignore, bypass, outlive, or locally reverse it. A lifecycle label alone does not prevent action, and a transport notice alone does not revoke authority.

FRS-001 must prove an independently enforceable path from a credible protective condition to immediate authority restriction, safe lifecycle transition, preserved minimum protection, controlled recovery, and independently validated release.

## 2. Decision Drivers

- stop prohibited action at an enforceable boundary;
- keep Guardian independent from the actor it constrains;
- apply default denial when enforcement state is unknown;
- coordinate authority restriction and lifecycle containment;
- preserve essential monitoring, evidence, security, and recovery capability;
- persist unresolved restrictions across restart;
- prevent recovery or the restricted actor from approving its own release;
- contain unaffected safe operation where trustworthy; and
- preserve complete evidence of imposition, enforcement, and release.

## 3. Higher-Authority Constraints

This decision is constrained by:

- the Vision priority of capital protection over opportunity and growth;
- constitutional requirements for bounded authority, independent protection, safe degradation, evidence, and controlled recovery;
- AUT-001 requirements for default denial, immediate revocation, and enforcement of lawful Guardian restrictions;
- AUT-002 requirements for proportionate protection, independent evidence, persistent restrictions, and authorized release;
- SYS-001 and SYS-002 requirements for restricted Core operation and authoritative lifecycle transitions;
- OPS-003 requirements for independent validation and prohibition of self-release;
- RSK-005 requirements for independently enforceable capital-safety limits in future financial releases; and
- FRS-001 requirements for independent Guardian restriction, Safe state, and controlled recovery.

## 4. Alternatives Considered

### 4.1 Voluntary component compliance

Guardian could notify a component and rely on it to stop itself.

This was rejected because the component may be failed, compromised, unreachable, or the source of the danger.

### 4.2 Lifecycle state as the only enforcement

Lifecycle could mark a component restricted and assume all actions cease.

This was rejected because state representation does not by itself revoke permissions or block already reachable execution paths.

### 4.3 Independent restriction with layered enforcement

Guardian issues a binding protective restriction. Authority Engine revokes or constrains permission, enforcement points deny affected actions, and Lifecycle establishes the corresponding protective state.

This alternative was selected because it separates detection, authority enforcement, state transition, recovery, and release while preventing the subject from controlling its own restriction.

## 5. Decision

Guardian SHALL own the authoritative protective restriction within its approved mandate. A restriction SHALL identify trigger, evidence, authority, scope, consequence, prohibited and permitted activity, issue time, persistence, and release conditions.

When a mandatory protective condition is established, Guardian SHALL issue a binding restriction through a protected control path independent of the subject to the degree technically possible.

The Authority Engine SHALL immediately incorporate the restriction into authorization decisions and revoke or constrain affected delegated authority. Every material execution boundary SHALL enforce a current authority decision or an approved subordinate enforcement result before permitting governed action.

Lifecycle SHALL transition the affected component or scope to the corresponding protective state: `RESTRICTED`, `SUSPENDED`, `STOPPED`, `FAILED`, or another state explicitly governed by SYS-002. Lifecycle state and authority restriction SHALL be correlated but remain distinct evidence.

The restricted subject SHALL NOT be able to:

- suppress, weaken, replace, expire, or release its restriction;
- rely on previously granted authority after applicable revocation;
- bypass enforcement through direct communication, configuration, retry, replay, restart, maintenance, or recovery;
- represent restart or disappearance of the trigger as proof of safety; or
- disable the evidence required to review the intervention.

When the Authority Engine, Guardian restriction state, identity, required evidence, or enforcement communication cannot be trusted, affected governed action SHALL be denied. An approved local enforcement point MAY preserve a narrower fail-closed restriction but SHALL NOT preserve broader permission on unknown revocation state.

Safe state SHALL preserve only the minimum explicitly permitted capabilities required for protection, observation, security, evidence preservation, containment, controlled shutdown, and recovery. Nonessential activity and every activity that could increase the affected consequence SHALL remain denied.

Unresolved restrictions SHALL survive component and Falcon restart through protected durable state and integrity evidence.

Recovery MAY repair and validate the affected subject but SHALL NOT release the restriction it is recovering from. Release SHALL require:

1. evidence that the trigger is resolved or acceptably contained;
2. reconciliation of authoritative state and integrity;
3. validation independent of the repair action to the required consequence level;
4. authorization by the declared release authority;
5. Guardian acceptance of the satisfied release conditions within its mandate;
6. a controlled Lifecycle transition; and
7. restoration of authority through a new attributable decision.

FRS-001 SHALL demonstrate this boundary without financial activity. Future capital-affecting execution SHALL add independent Capital Safety Plane enforcement and external safeguards as governed by RSK-005.

This decision does not select an operating-system mechanism, policy engine product, process layout, emergency interface, or future broker-side control.

## 6. Consequences

- Guardian restrictions become enforceable rather than advisory.
- Permission revocation and lifecycle containment reinforce each other.
- The restricted component cannot approve or perform its own release.
- Unknown enforcement state fails closed for affected action.
- Safe state preserves protection and recovery without claiming normal operation.
- Restriction state must be durable, attributable, and available through a protected path.
- Every future execution boundary must prove that it cannot bypass current protective authority.
- Some unaffected operation may continue only when isolation and protection are trustworthy.

## 7. Risks and Mitigations

- **Risk:** Guardian could impose an incorrect or excessive restriction.  
  **Mitigation:** Require declared mandate, evidence, proportional scope, independent oversight, and attributable review; protection remains favored under severe uncertainty.

- **Risk:** The control path could fail during the condition it must contain.  
  **Mitigation:** Protect the path independently, preserve fail-closed enforcement, and deny affected action when restriction state is unknown.

- **Risk:** Cached permission could outlive a new restriction.  
  **Mitigation:** Bind cached authority to explicit age and revocation rules; prohibit broader permission when revocation state is unknown.

- **Risk:** Safe state could disable the tools needed for recovery.  
  **Mitigation:** Maintain a minimum explicit allowlist for protection, evidence, observation, security, containment, and recovery.

- **Risk:** Recovery could become an indirect bypass.  
  **Mitigation:** Separate repair, validation, release authorization, Lifecycle transition, and restored authority.

## 8. Compatibility and Transition

No prior Falcon1 ADR is superseded.

Before implementation authorization, FRS-001 shall define the protective mandate matrix, minimum Safe-state allowlist, restriction Contract, release-authority matrix, enforcement points, and verification plan. These artifacts shall remain non-financial.

Future live-capital releases require additional ADRs for Capital Safety Plane enforcement, external safeguards, open-position behavior, and independent stop channels. They shall preserve the enforcement and release separation established here.

## 9. Conformance Evidence

Conformance shall be demonstrated by evidence that:

- a mandatory Guardian condition produces an attributable binding restriction;
- affected authority is revoked before or at the material execution boundary;
- the restricted subject cannot execute through direct, replayed, retried, restarted, maintenance, configuration, or recovery paths;
- Lifecycle enters the required protective state without treating the state label as sole enforcement;
- unknown restriction or revocation state denies affected action;
- unresolved restriction survives restart;
- minimum protective, evidence, security, and recovery capability remains available;
- failure or compromise of the restricted subject cannot remove the restriction;
- recovery cannot approve its own completion; and
- release requires independent validation, authorized approval, controlled transition, and a new authority decision.

## 10. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner and current Constitutional Authority | Accepted | رائد عموره — “موافق على القرار الثامن” | 2026-07-24 |
