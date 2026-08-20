# Stage 6 WP-06 — Planning v0.2 Red-Team Remediated

**Title:** Additional Resource Request and Decision Boundary  
**Status:** PROPOSED / OWNER REVIEW REQUIRED  
**Stage:** 6 — Foundation Resource Governance and Operational Pressure Control  
**Work Package:** WP-06  
**Planning Version:** v0.2  
**Supersedes for review:** v0.1 draft only  
**Owner Acceptance:** NOT YET  
**Implementation Authority:** NOT GRANTED  
**Runtime Activation:** NOT GRANTED  
**Date:** 2026-08-10

## 1. Purpose

WP-06 defines the generic Foundation runtime boundary for submitting additional-resource requests and producing bounded Foundation request decisions.

It consumes accepted WP-01 through WP-05 truth and supports a generic direct requester plus a separately governed aggregate coordinator/requester model compatible with future FSARM semantics.

WP-06 does not implement WP-07 resource mutation/reclamation/redistribution/rebalance/restoration execution and does not implement WP-08 load-shedding projection or execution.

## 2. Preserved predecessor closures

Stage 6 WP-01 through WP-05 remain `ACCEPTED_AND_CLOSED`.

A WP-06 requirement is not a predecessor defect unless an explicit closure-scope trace proves otherwise.

`PRESERVE_ACCEPTED_CLOSURES = TRUE`

`CLOSURE_DEFECT_REQUIRES_EXPLICIT_TRACE = TRUE`

## 3. Authority boundary

WP-06 SHALL:

- receive and validate an additional-resource request;
- establish exact requester identity and request role;
- bind the request to an exact target Application or exact governed represented constituent set;
- consume current accepted Foundation resource/allocation/priority/criticality/pressure truth as applicable;
- require residual unmet-need evidence at the correct scope;
- produce one exact request decision;
- preserve correlation, causation, evidence, effective lifetime, expiry, deterministic identity, supersession and reconstructability;
- fail closed on invalid, stale, forged, split-brain, mismatched or unauthorized state.

WP-06 SHALL NOT:

- mutate existing allocations through reclamation, redistribution, rebalance or restoration execution;
- execute Application-internal redistribution;
- execute load shedding;
- mint authority from pressure, priority, technical criticality or request urgency;
- make an aggregate coordinator a Foundation principal or replacement Application principal;
- erase constituent identity or accounting;
- create financial, trading, broker, market-data or capital authority.

## 4. Reused canonical primitives

WP-06 reuses accepted WP-01 primitives including:

- `ResourceRequestId`
- `ResourceDecisionId`
- `ResourceGrantId`
- `ResourceDecisionKind`
- `ResourceQuantity`
- `ResourceClassId`
- `ApplicationPrincipalId`
- `CorrelationId`
- `CausationId`
- `ResourceEpochId`
- `ResourceEvidenceReference`
- canonical deterministic identity support

WP-06 shall not duplicate them.

## 5. Requester identity model

### 5.1 Requester principal and requester role are distinct

Every WP-06 request must identify:

- exact requester instance identity;
- exact requester role identity;
- exact authority/delegation evidence permitting that requester role to use the WP-06 boundary;
- exact target/represented Application identity scope.

Requester identity does not create resource authority.

Requester role does not create resource authority.

Delegation is bounded, attributable, reviewable and revocable.

### 5.2 Direct Application requester

A conforming admitted Application may submit an additional-resource request for itself when an accepted authority/delegation permits use of the boundary.

The request must bind the exact Application identity, current grant/allocation/quota/ceiling context and requested resource class.

### 5.3 Delegated aggregate coordinator requester

A separately governed coordinator, compatible with the prospective FSARM model, may submit a request representing an exact bounded constituent set.

The coordinator request must bind:

- exact coordinator instance identity;
- exact coordinator role identity;
- exact delegation/coordination scope identity;
- exact constituent admitted Application identities;
- exact permitted resource classes;
- effective/expiry boundaries;
- supersession and fencing evidence;
- current Foundation-authoritative grants/ceilings for represented constituents;
- residual unmet-need evidence.

The coordinator is not an opaque Application principal, does not own Foundation capacity and may not self-mint Foundation grant/ceiling authority.

## 6. Internal redistribution first

For an aggregate coordinator request:

`INTERNAL_REDISTRIBUTION_FIRST`

`FOUNDATION_ADDITIONAL_REQUEST_SECOND`

WP-06 must receive evidence sufficient to establish that the residual request is not merely bypassing the valid internal coordination path.

This is a request-admission/decision input only.

WP-06 does not execute internal redistribution. The bounded redistribution/execution semantics remain later WP-07 work under separate authority.

## 7. Required request content

Every admitted request shall bind at minimum:

- exact `ResourceRequestId`;
- requester instance identity;
- requester role identity;
- exact request authority/delegation evidence;
- direct target Application or exact represented constituent set;
- target `ResourceClassId`;
- requested additional `ResourceQuantity`;
- current `ResourceEpochId`;
- current applicable grant/allocation/quota/ceiling identities;
- predecessor Foundation resource-truth identity;
- predecessor allocation-truth identity;
- applicable priority/technical-criticality truth identity when used by decision policy;
- applicable pressure-truth identity when used by decision policy;
- residual unmet-need evidence;
- `CorrelationId`;
- `CausationId`;
- request evidence reference;
- observation/creation time;
- requested effective lifetime or bounded duration;
- exact coordinator/delegation/fencing material when aggregate.

A request is evidence for consideration only.

`REQUEST_EVIDENCE != AUTHORITY`

## 8. Residual need rule

WP-06 distinguishes:

- caller-requested quantity;
- proven residual unmet need;
- Foundation-decided additional quantity.

They are not interchangeable.

`REQUESTED_RESOURCE != PROVEN_RESIDUAL_NEED`

`PROVEN_RESIDUAL_NEED != GRANTED_RESOURCE`

`REQUESTED_RESOURCE != GRANTED_RESOURCE`

For a direct Application request, current allocation/ceiling context and evidence of additional need are required.

For an aggregate coordinator request, the residual need must be attributable to the represented scope and must establish why the valid internal coordination path cannot safely satisfy the remaining need without a Foundation-authoritative change.

## 9. WP-06 request decision outcomes

WP-06 request evaluation may produce only the following request-bound outcomes:

- `Grant`
- `PartialGrant`
- `Cap`
- `Deny`
- `Defer`

The canonical WP-01 enum also contains `Revoke`, `Reduce` and `Restore`, but WP-06 SHALL NOT use those as additional-request outcomes or implement their state-mutation semantics.

`Revoke / Reduce / Restore` remain canonical vocabulary for the broader resource-governance lifecycle and their mutation/execution placement is outside WP-06, principally later WP-07 under separate authority.

This restriction prevents WP-06 from silently absorbing WP-07.

## 10. Decision record

Each WP-06 request evaluation shall produce one exact attributable decision record binding:

- exact `ResourceDecisionId`;
- exact source `ResourceRequestId`;
- requester instance identity;
- requester role identity;
- exact direct target or represented constituent set;
- target resource class;
- requested quantity;
- proven residual need;
- decision outcome;
- decided additional quantity or cap where applicable;
- exact decision authority/evidence;
- effective lifetime;
- predecessor truth identities;
- correlation and causation;
- deterministic identity;
- supersession/expiry/fencing material.

A decision record is not allowed to erase the identity of represented constituent Applications.

## 11. Decision constraints

WP-06 request decisions SHALL NOT:

- exceed authoritative Foundation available capacity;
- violate protection floors;
- violate recovery reserves;
- silently reinterpret an existing Application ceiling;
- erase exact Application attribution;
- treat priority as authority;
- treat technical criticality as business authority;
- treat pressure as grant authority;
- treat coordinator scope as resource ownership;
- treat requested quantity as entitlement;
- rely on unavailable, stale or mismatched predecessor truth;
- cross epochs without valid governed transition evidence.

Where fulfilling an approved additional-resource decision requires a Foundation-authoritative mutation beyond the exact WP-06 boundary, the mutation/execution remains separately gated and shall not be silently performed by WP-06.

## 12. Concurrency, fencing and split-brain protection

WP-06 shall reject or fail closed on:

- duplicate active request identities;
- replayed request identity;
- replayed decision identity;
- stale predecessor snapshot identity;
- superseded delegation;
- expired delegation;
- constituent-scope mismatch;
- conflicting active coordinator instances for the same governed coordination scope without valid fencing;
- decision/result substitution across request identities;
- cross-epoch reuse;
- decision use after expiry;
- stale coordinator fencing token/evidence;
- superseded request lineage.

## 13. Reconstructability

Foundation shall be able to reconstruct for every WP-06 request/decision:

- who requested;
- under which exact role and authority;
- which Application or constituent set was represented;
- which resource was requested;
- the current grants/ceilings and predecessor truth used;
- the requested quantity;
- the proven residual need;
- the decision outcome;
- the decided quantity/cap;
- the authority and evidence supporting the decision;
- correlation and causation;
- effective/expiry boundaries;
- supersession/fencing lineage.

Aggregate coordination never permits opaque pooling that destroys constituent accountability.

## 14. Failure behavior

WP-06 fails closed on at least:

- unknown requester instance;
- invalid requester role;
- missing/invalid/expired authority or delegation evidence;
- unknown direct target Application;
- unknown represented constituent;
- constituent outside coordinator scope;
- invalid resource class;
- invalid quantity/unit;
- unavailable/stale Foundation resource truth;
- unavailable/stale allocation/grant/ceiling truth;
- mismatched predecessor identity;
- forged or mismatched priority/criticality/pressure input;
- missing residual-need evidence;
- aggregate escalation without required internal-coordination evidence;
- invalid correlation/causation;
- duplicate/replayed request;
- duplicate/replayed decision;
- cross-epoch evidence mismatch;
- future evidence;
- expired evidence;
- split-brain coordinator state;
- stale fencing evidence;
- decision/request mismatch;
- decision kind outside the permitted WP-06 request-outcome subset.

## 15. Deterministic identity

Request identity shall change when any authority- or reconstruction-relevant request material changes.

Decision identity shall change when any authority- or reconstruction-relevant decision material changes.

Order-independent collections such as represented constituent identities shall have one canonical ordering before identity computation.

## 16. Zero-Application validity

Foundation remains operational with zero Applications.

An empty requester population is valid.

WP-06 does not make any Application a Foundation prerequisite.

## 17. Application neutrality and FSARM compatibility

Production Foundation code must remain generic and Application-neutral.

The future FSARM model may consume generic aggregate-coordinator contracts when separately authorized, but Foundation production types shall not hard-bind to `FSARM`, `TARC`, Trading, broker, strategy or market semantics.

FSARM compatibility is achieved through generic coordinator/requester contracts and evidence, not business-specific Foundation code.

## 18. Planned implementation surface

Prospective implementation may add generic request/decision contracts and Foundation resource-governance decision processing to existing generic Foundation projects, or introduce a separately justified Foundation project if architecture review proves it necessary.

No placement is authorized by this planning record.

No Application source modification is authorized by WP-06 Foundation implementation.

## 19. Minimum verification families

A future dedicated WP-06 verifier shall cover at minimum:

1. direct Application positive request;
2. aggregate coordinator positive request;
3. requester instance identity validation;
4. requester role identity validation;
5. requester role/instance separation;
6. requester identity does not create authority;
7. exact direct Application attribution;
8. exact constituent attribution;
9. exact coordinator scope binding;
10. request/grant/decision identity separation;
11. requested quantity vs residual need separation;
12. residual need vs decided quantity separation;
13. `Grant` positive path;
14. `PartialGrant` positive path;
15. `Cap` positive path;
16. `Deny` positive path;
17. `Defer` positive path;
18. reject `Revoke/Reduce/Restore` as WP-06 request outcomes;
19. predecessor Foundation resource-truth binding;
20. predecessor allocation/grant/ceiling binding;
21. protection-floor preservation;
22. recovery-reserve preservation;
23. priority does not mint authority;
24. technical criticality does not mint business authority;
25. pressure does not mint authority;
26. residual-need evidence required;
27. aggregate internal-redistribution-first evidence required;
28. delegation scope validation;
29. delegation expiry rejection;
30. delegation supersession rejection;
31. constituent-scope mismatch rejection;
32. stale fencing rejection;
33. split-brain coordinator rejection;
34. stale predecessor rejection;
35. duplicate request rejection;
36. request replay rejection;
37. decision replay rejection;
38. cross-epoch rejection;
39. future evidence rejection;
40. expired evidence rejection;
41. deterministic request identity;
42. deterministic decision identity;
43. canonical constituent ordering;
44. request-decision correlation;
45. causation preservation;
46. decision/request mismatch rejection;
47. Application-neutral production surface;
48. no TARC hard-binding;
49. no FSARM business-specific production mechanics;
50. no opaque aggregate pool;
51. no WP-07 reclamation/redistribution/rebalance/restoration executor;
52. no WP-08 load-shedding executor;
53. zero-Application validity;
54. accepted WP-01 through WP-05 regression preservation;
55. clean Release build;
56. Architecture gate;
57. Security gate;
58. repeat dedicated verifier run from the same Release outputs.

## 20. Explicit non-goals

WP-06 does not implement:

- internal FSARM redistribution execution;
- Foundation reclamation execution;
- redistribution execution;
- rebalance execution;
- restoration execution;
- load shedding;
- Guardian Safe State;
- transport QoS;
- external egress;
- FSA governance;
- canonical artifact publication;
- Application runtime hosting;
- environment qualification;
- financial/trading authority.

## 21. Closure prerequisites

Before Owner closure can be requested:

1. Owner accepts exact WP-06 planning artifact;
2. Owner separately authorizes WP-06 implementation;
3. pre-implementation Red-Team passes;
4. implementation remains inside exact scope;
5. clean Release build passes;
6. Foundation Architecture gate passes;
7. Foundation Security gate passes;
8. accepted Stage 6 WP-01 through WP-05 verifier regression passes;
9. dedicated WP-06 verifier passes all accepted families;
10. dedicated verifier repeat passes from same Release outputs;
11. final integrity is preserved;
12. post-implementation Red-Team passes;
13. applicable FCRs are handed to Application for implementation-compatibility verification;
14. Application compatibility ACK is received or incompatibilities are remediated;
15. Owner issues explicit final closure decision.

## 22. Current state

`WP06_PLANNING = PROPOSED_v0.2`

`WP06_OWNER_ACCEPTANCE = NOT_YET`

`WP06_IMPLEMENTATION_AUTHORITY = NOT_GRANTED`

`WP07_WP08_IMPLEMENTATION_AUTHORITY = NOT_GRANTED`

`WP01_WP05 = ACCEPTED_AND_CLOSED / PRESERVED`
