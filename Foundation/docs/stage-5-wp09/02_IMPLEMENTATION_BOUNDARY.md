# Stage 5 WP-09 — Implementation Boundary

**Date:** 2026-08-08  
**Status:** BOUNDARY DEFINED

## Production boundary

WP-09 production SHALL implement only a Foundation-owned, Application-neutral lifecycle decision/evidence capability for attachment, upgrade/replacement, draining, rollback/recovery direction, safe detachment and removal.

## Inputs WP-09 may consume

Only governed generic evidence/identities, including where applicable:

- subject/package identity and version;
- accepted Manifest identity/digest;
- lifecycle authority/delegation evidence;
- dependency snapshot/availability evidence;
- contract/schema compatibility evidence;
- current lifecycle generation/state;
- declared permission/security/control profile identity;
- drain requirement and drain-completion evidence;
- rollback target/generation evidence;
- predecessor decision/evidence identities.

## Outputs WP-09 may produce

Only bounded lifecycle decisions/evidence, including:

- attach eligibility/rejection;
- upgrade/replacement eligibility/rejection;
- drain-required decision;
- safe-detach/removal eligibility/rejection;
- rollback/recovery-required direction;
- deterministic lifecycle decision identity and evidence.

## Forbidden outputs/claims

WP-09 SHALL NOT output or claim:

- business approval or business-success truth;
- Trading/Risk/strategy/market/broker/provider decisions;
- runtime activation/deployment approval;
- external egress/connectivity approval;
- credential authority;
- new resource authority;
- new communication authority;
- new Owner/FSA delegation;
- integrated Stage 5 acceptance/closure.

## Cross-package invariants

- lifecycle transitions may preserve or reduce valid authority, never silently expand it;
- old and new generations must remain explicitly distinguishable;
- replacement must not rewrite historical evidence;
- removal must not erase accountability;
- package/discovery/compatibility facts are not authority;
- hidden coupling that prevents safe removal is a rejection condition, not a reason to absorb Application logic into Foundation.

## Application-owned state boundary

Application-private business state, business migrations, positions/orders, strategies, Risk state, provider/broker state and domain-specific recovery remain Application-owned. Foundation may require generic evidence that declared lifecycle obligations are satisfied, but SHALL NOT interpret or mutate Application business state.

## FCR boundary

- FCR-0011: limited cross-cutting constraint only; no Live/non-Live egress enforcement implementation.
- FCR-0012: limited cross-cutting lifecycle evidence consumer only; no FSA/Owner autonomous-evolution control-plane implementation.
- FCR-0004 through FCR-0010 and FCR-0013 through FCR-0014: outside WP-09 implementation scope.

## Later-stage boundary

WP-10 and Stage 6 through Stage 9 remain unauthorized. No WP-09 artifact may be interpreted as prospective authority for those stages.
