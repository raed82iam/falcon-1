# Stage 5 WP-09 — Implementation Design

**Date:** 2026-08-08  
**Status:** DESIGN DEFINED  
**Authority:** Stage 5 WP-09 implementation authorization

## 1. Design objective

Provide one Application-neutral Foundation lifecycle decision capability for replaceable governed units without creating business authority, deployment authority, runtime activation, external connectivity or Application-specific semantics.

The lifecycle layer answers only whether a declared lifecycle transition is technically/governance-eligible under already valid evidence and authority.

## 2. Governed lifecycle model

The design SHALL model a bounded lifecycle with explicit states sufficient to represent:

- detached/not-attached;
- attachment candidate;
- attached;
- draining;
- replacement/upgrade candidate;
- replacing/upgrading;
- rollback-required/recovery-directed;
- removal candidate;
- detached/removed.

Exact implementation names may differ, but impossible or regressive transitions must fail closed.

## 3. Lifecycle command model

A lifecycle request SHALL carry or reference at minimum:

- lifecycle request identity;
- subject identity;
- current version;
- requested target version where applicable;
- requested transition kind;
- requesting authority identity and authority evidence identity;
- manifest identity/version/digest;
- dependency snapshot identity;
- contract/schema compatibility evidence identity where applicable;
- current lifecycle state/version;
- protected-control/security profile identity where applicable;
- drain requirement and drain evidence identity where applicable;
- rollback target identity where applicable;
- correlation/causation identities;
- evidence observation identity/order metadata.

No business payload interpretation is permitted.

## 4. Decision model

The lifecycle component SHALL produce deterministic decisions such as:

- `ALLOW`
- `REJECT`
- `REQUIRE_DRAIN`
- `REQUIRE_ROLLBACK`

A positive lifecycle decision only establishes lifecycle eligibility for the exact requested transition. It SHALL NOT mean:

- business approval;
- deployment approval;
- runtime activation;
- permission expansion;
- external connectivity;
- successful Application behavior;
- successful migration of Application-owned business state.

## 5. Attachment design

Attachment eligibility requires exact subject identity and version, valid accepted manifest evidence, required dependency availability, compatible declared contracts/schemas, valid lifecycle authority and absence of prohibited authority expansion.

Attachment SHALL fail closed when any required prerequisite is missing, ambiguous, stale/revoked, inconsistent or conflicting.

## 6. Upgrade/replacement design

Replacement/upgrade SHALL be evaluated against both the currently attached subject and the candidate target.

Required protections include:

- exact current-to-target version binding;
- no hidden identity substitution;
- compatibility with required dependencies/contracts;
- no undeclared permission/authority expansion;
- no protected-control/security weakening;
- deterministic replacement identity;
- explicit rollback target where rollback is required;
- preservation of authoritative evidence linking old and new generations.

An upgrade MAY reduce authority where separately valid governance directs it. It SHALL NOT silently widen authority.

## 7. Draining design

Where safe detachment/replacement requires draining, the lifecycle layer SHALL distinguish:

- drain required;
- drain in progress/insufficient evidence;
- drain complete and attributable;
- drain evidence invalid/stale/ambiguous.

WP-09 consumes truthful predecessor evidence where needed but does not redefine WP-06 delivery semantics or WP-07 event truth.

Drain completion means only the governed technical drain criterion is satisfied. It does not assert Application business completion.

## 8. Safe detachment/removal design

Safe removal requires proving that mandatory lifecycle/dependency obligations are satisfied or that a governed containment/rollback direction exists.

Removal SHALL reject:

- unresolved mandatory dependents that make safe removal impossible;
- incomplete required draining;
- ambiguous subject generation;
- hidden coupling discovered in required declarations/evidence;
- removal that would bypass protected controls;
- removal requested without valid authority.

Foundation does not decide how an Application disposes of its private business state. It only validates the generic declared lifecycle obligations required for safe separation.

## 9. Rollback/recovery design

Rollback is a bounded lifecycle direction, not automatic authority to restore arbitrary prior behavior.

Rollback SHALL bind:

- exact failed/current transition identity;
- exact prior trusted lifecycle generation;
- rollback target identity/version;
- valid rollback authority/evidence where required;
- compatibility/security/authority constraints that remain applicable.

Rollback SHALL fail closed when the target is ambiguous, invalid, revoked or would recreate authority that is no longer valid.

## 10. Evidence and determinism

Every decision SHALL be attributable and reconstructable from canonical input identities.

Decision identity SHALL be deterministic for the same canonical inputs and governing evidence.

Evidence SHALL distinguish:

- request;
- prerequisite facts;
- lifecycle decision;
- transition outcome evidence;
- rollback/recovery direction;
- later correction/supersession where supported by accepted predecessor evidence models.

## 11. Security and authority invariants

- lifecycle capability SHALL NOT mint authority;
- package discovery SHALL NOT imply admission or attachment;
- compatibility SHALL NOT imply authorization;
- attachment SHALL NOT imply runtime activation;
- replacement SHALL NOT imply permission inheritance beyond exact valid authority;
- removal SHALL NOT erase accountability or historical evidence;
- revoked/stale authority SHALL fail closed;
- subject/version substitution SHALL fail closed;
- protected-control weakening SHALL fail closed.

## 12. Application-neutrality

The implementation SHALL contain no Trading-specific, broker-specific, provider-specific, Risk-specific, strategy-specific, portfolio-specific or market-specific logic.

Lifecycle decisions operate on declared generic identities, contracts, permissions, dependencies, security/control profiles and evidence only.

## 13. FCR boundary

`FCR-0011` and `FCR-0012` are limited cross-cutting consumers/constraints only. Their missing runtime capabilities are not implemented by WP-09.

All other reviewed FCRs through `FCR-0014` remain outside WP-09 ownership.

## 14. Later-work boundary

WP-09 does not perform integrated Stage 5 closure, deployment orchestration, runtime activation or WP-10 behavior.

`WP10 = UNAUTHORIZED` remains binding.
