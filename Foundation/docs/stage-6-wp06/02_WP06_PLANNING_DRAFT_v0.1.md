# Stage 6 WP-06 — Planning Draft v0.1

**Title:** Additional Resource Request and Decision Boundary  
**Status:** DRAFT / OWNER REVIEW REQUIRED  
**Stage:** 6 — Foundation Resource Governance and Operational Pressure Control  
**Work Package:** WP-06  
**Planning Version:** v0.1  
**Owner Acceptance:** NOT YET  
**Implementation Authority:** NOT GRANTED  
**Runtime Activation:** NOT GRANTED  
**Date:** 2026-08-10

## 1. Purpose

WP-06 defines the generic Foundation runtime boundary for requesting additional governed resources and producing exact Foundation resource decisions.

It consumes accepted WP-01 through WP-05 truth and preserves the future FSARM aggregate-coordinator requester model without importing WP-07 redistribution execution or WP-08 load-shedding projection behavior.

## 2. Authority boundary

WP-06 SHALL:

- receive and validate additional-resource requests;
- bind each request to an exact requester identity, represented scope and current Foundation truth;
- determine whether a request is admissible for Foundation decision processing;
- produce an exact resource decision within Foundation resource authority;
- preserve evidence, deterministic identity, correlation, causation, effective lifetime, expiry and reconstructability;
- fail closed when request identity, authority, scope, evidence, timing or predecessor truth is invalid.

WP-06 SHALL NOT:

- execute Application-internal redistribution;
- perform WP-07 reclamation, redistribution, rebalance or restoration execution;
- implement WP-08 load shedding;
- create Application business priority;
- convert pressure into authority;
- create financial, trading, broker, market-data or capital authority;
- make FSARM a Foundation principal or replacement Application principal.

## 3. Requester models

WP-06 shall support at least these generic governed requester forms:

### 3.1 Direct Application requester

A conforming admitted Application may request additional Foundation-governed capacity for itself when authorized to use the request boundary.

The request must preserve exact Application identity and current grant/allocation/ceiling context.

### 3.2 Delegated aggregate coordinator requester

A separately governed coordinator such as the prospective FSARM model may submit an aggregate additional-resource request for an exact bounded set of constituent Applications.

The coordinator request SHALL NOT erase constituent identities or create an opaque pooled Application principal.

The request must bind:

- exact coordinator identity;
- exact delegation/coordination scope;
- exact constituent Application identities;
- permitted resource classes;
- effective and expiry boundaries;
- fencing/supersession state;
- current Foundation-authoritative grants/ceilings for represented constituents;
- residual unmet-need evidence.

## 4. Internal redistribution first

For a delegated aggregate coordinator request, WP-06 shall require evidence that the requester has applied or evaluated the valid internal coordination path before escalating residual demand to Foundation.

The controlling semantics are:

`INTERNAL_REDISTRIBUTION_FIRST`

`FOUNDATION_ADDITIONAL_REQUEST_SECOND`

This does not authorize WP-06 to execute the internal redistribution itself.

WP-07 remains the separately gated location for bounded internal redistribution and Foundation-authoritative mutation separation.

## 5. Required request content

Each accepted WP-06 request shall bind at minimum:

- `ResourceRequestId`;
- requester identity and requester kind;
- exact target resource class;
- requested additional quantity;
- current resource epoch;
- current applicable grant/allocation/quota/ceiling identity or identities;
- current pressure state when applicable;
- current priority and technical criticality bindings where applicable;
- residual unmet-need evidence;
- correlation identity;
- causation identity;
- request evidence reference;
- request creation/observation time;
- requested effective lifetime or bounded duration;
- delegation/coordination evidence when the requester represents more than itself;
- predecessor snapshot identities required for reconstructability.

A request is evidence for consideration only. It is not a grant.

## 6. Residual need rule

WP-06 shall distinguish requested quantity from proven residual need.

For aggregate-coordinator requests, the request must establish why the proven remaining need cannot be satisfied safely inside the valid coordination envelope without a Foundation-authoritative change.

For direct Application requests, the request must establish its exact current allocation/ceiling context and additional need without implying entitlement.

`REQUESTED_RESOURCE != GRANTED_RESOURCE`

`REQUEST_EVIDENCE != AUTHORITY`

## 7. Foundation decision model

WP-06 shall produce one exact attributable decision identity for each accepted request evaluation.

It shall reuse the canonical `ResourceDecisionKind` values established in WP-01:

- `Grant`
- `PartialGrant`
- `Cap`
- `Deny`
- `Defer`
- `Revoke`
- `Reduce`
- `Restore`

Not every canonical decision kind must necessarily mutate state inside WP-06. The exact state-mutation/execution placement must preserve WP-07 ownership where applicable.

The decision record shall bind:

- exact `ResourceDecisionId`;
- exact source `ResourceRequestId`;
- requester identity;
- represented constituent scope when applicable;
- resource class;
- requested quantity;
- decided quantity or cap when applicable;
- decision kind;
- effective lifetime;
- decision authority/evidence;
- predecessor truth identities;
- correlation and causation;
- deterministic identity;
- supersession/revocation/fencing material where applicable.

## 8. Decision constraints

A WP-06 decision SHALL NOT:

- exceed authoritative Foundation resource truth;
- violate protection floors or recovery reserves;
- silently exceed an Application ceiling;
- erase per-Application attribution;
- let requester priority or pressure self-mint authority;
- treat technical criticality as business authority;
- treat coordinator scope as ownership of Foundation resources;
- treat a request as entitlement;
- use stale or mismatched predecessor truth;
- cross resource epochs without explicit valid transition evidence.

## 9. Concurrency, fencing and split-brain protection

WP-06 planning requires explicit rejection of:

- duplicate active request identities;
- replayed decisions;
- stale request snapshots;
- superseded coordinator delegations;
- mismatched constituent scope;
- conflicting coordinator instances for the same governed coordination scope without valid fencing;
- decision/result substitution across requests;
- cross-epoch request/decision reuse;
- decision application after expiry.

The exact fencing mechanism shall remain generic and evidence-based.

## 10. Direct versus aggregate attribution

Foundation shall remain able to reconstruct:

- who requested;
- for which resource class;
- for which exact Application or constituent set;
- under which delegation;
- from which predecessor grants/ceilings/pressure truth;
- what quantity was requested;
- what quantity was decided;
- why the decision was made;
- when it was effective;
- what superseded it.

An aggregate request shall never collapse constituent accountability into an opaque pool.

## 11. Failure behavior

WP-06 shall fail closed on at least:

- unknown requester;
- unauthorized requester role;
- missing/invalid delegation evidence;
- unknown constituent Application;
- constituent outside coordination scope;
- unknown resource class;
- invalid quantity/unit;
- stale or unavailable Foundation resource truth;
- stale/mismatched allocation or ceiling truth;
- invalid priority/criticality binding where required;
- forged or mismatched pressure evidence;
- missing residual-need evidence where required;
- invalid correlation/causation;
- duplicate request identity;
- cross-epoch evidence mismatch;
- future/expired evidence;
- split-brain coordinator state;
- expired/superseded delegation;
- decision identity collision;
- result/request mismatch.

## 12. Deterministic identity and evidence

Request and decision identities shall be deterministic over all authority-relevant and reconstruction-relevant material.

Material changes to requester, represented scope, resource class, quantity, predecessor truth, delegation, timing or decision outcome shall change the corresponding canonical identity.

Evidence must remain attributable and reviewable.

## 13. Zero-Application validity

The Foundation remains valid with zero Applications.

WP-06 requires no Application to exist in order for Foundation itself to remain operational.

An empty requester population is valid and shall not be represented as failure.

## 14. Planned implementation surface

Prospective implementation may introduce generic request/decision contracts and Foundation resource-governance state/decision processing under existing Foundation namespaces and projects, or a separately justified generic Foundation project if architecture review proves necessary.

No project placement is authorized by this draft.

No Application code shall be modified by WP-06 Foundation implementation.

## 15. Verification families

A future WP-06 verifier shall cover at minimum:

1. direct Application positive request;
2. delegated aggregate coordinator positive request;
3. exact requester identity;
4. exact coordinator identity and scope;
5. exact constituent attribution;
6. request/grant/decision identity separation;
7. requested quantity distinct from decided quantity;
8. partial grant semantics;
9. deny semantics;
10. defer semantics;
11. cap semantics;
12. predecessor Foundation resource-truth binding;
13. predecessor allocation/quota/ceiling binding;
14. priority binding without authority inflation;
15. technical criticality binding without business-authority inflation;
16. pressure evidence without authority inflation;
17. residual-need evidence;
18. internal-redistribution-first evidence for aggregate requester;
19. protection-floor preservation;
20. recovery-reserve preservation;
21. stale request rejection;
22. duplicate request rejection;
23. request replay rejection;
24. decision replay rejection;
25. delegation expiry rejection;
26. delegation supersession rejection;
27. constituent-scope mismatch rejection;
28. split-brain/fencing rejection;
29. cross-epoch rejection;
30. future evidence rejection;
31. expired evidence rejection;
32. deterministic request identity;
33. deterministic decision identity;
34. decision/request correlation;
35. causation preservation;
36. Application-neutral production surface;
37. no TARC hard-binding;
38. no FSARM-specific production mechanics beyond generic coordinator contracts;
39. no opaque aggregate pooling;
40. no WP-07 redistribution/reclamation/rebalance/restoration executor;
41. no WP-08 load-shedding executor;
42. zero-Application validity;
43. accepted WP-01 through WP-05 regression preservation;
44. repeatability from identical Release outputs.

## 16. Explicit non-goals

WP-06 does not implement:

- FSARM internal resource redistribution;
- resource reclamation execution;
- rebalance execution;
- restoration execution;
- Application load shedding;
- Guardian Safe State;
- transport QoS;
- external egress;
- FSA governance;
- canonical artifact publication;
- Application hosting;
- environment deployment qualification;
- financial/trading authority.

## 17. Closure prerequisites

Before WP-06 can be proposed for Owner closure, the governed lifecycle shall include:

1. Owner-accepted WP-06 planning;
2. separate explicit Owner implementation authorization;
3. pre-implementation Red-Team;
4. implementation within exact authorized scope;
5. clean Release build;
6. Foundation Architecture verification;
7. Foundation Security verification;
8. regression of accepted Stage 6 WP-01 through WP-05 verifiers;
9. dedicated WP-06 verifier with all accepted test families passing;
10. repeat verifier execution for determinism;
11. post-implementation Red-Team;
12. applicable FCR handoff and Application implementation-compatibility verification;
13. final Owner closure decision.

## 18. Current state

`WP06_PLANNING = DRAFT_v0.1`

`WP06_OWNER_ACCEPTANCE = NOT_YET`

`WP06_IMPLEMENTATION_AUTHORITY = NOT_GRANTED`

`WP07_WP08_IMPLEMENTATION_AUTHORITY = NOT_GRANTED`

`WP01_WP05 = ACCEPTED_AND_CLOSED / PRESERVED`
