# Stage 6 — FSARM / FCR-0031 Reconciliation v0.2 Remediated

**Status:** FOUNDATION RECONCILIATION CANDIDATE / REMEDIATED / NO IMPLEMENTATION AUTHORITY  
**Supersedes for review:** `13_FSARM_FCR0031_STAGE6_RECONCILIATION.md`  
**Red-Team basis:** `14_FSARM_FCR0031_RECONCILIATION_RED_TEAM_v0.1.md`  
**Controlling new input:** FCR-0031  
**Related FCRs:** FCR-0010, FCR-0007

## 1. Reconciliation decision

FCR-0031 is accepted as a prospective FSATS integration change. It does not reopen accepted Stage 6 WP-01 through WP-04 and does not require changing the IMP-001 v1.3 Stage 6 WP sequence.

The Foundation model shall support FSARM without converting FSARM into a replacement Application principal, a Foundation principal, or a new owner of Foundation resource truth.

## 2. Canonical FSARM role model

`FSARM_ROLE = DELEGATED_AGGREGATE_RESOURCE_COORDINATOR`

FSARM is a bounded FSATS-side coordinator role over an exact governed coordination scope.

It SHALL bind:

- one exact coordinator identity;
- one exact coordination-scope identity;
- an explicit set of constituent admitted Application identities;
- exact resource classes for which coordination is permitted;
- effective period/expiry;
- delegation/authorization evidence;
- fencing/single-active-coordinator state;
- revocation/supersession evidence.

FSARM SHALL NOT replace constituent Application identities. It SHALL NOT become an opaque resource owner. It SHALL NOT own Foundation resource truth.

`FSARM != APPLICATION_PRINCIPAL_REPLACEMENT`

`FSARM != FOUNDATION_RESOURCE_OWNER`

`FSARM != OPAQUE_RESOURCE_POOL_OWNER`

## 3. Constituent truth preservation

Every constituent Application remains independently admitted, attributable, accountable and isolated.

Foundation shall preserve exact per-Application:

- allocation/grant/ceiling truth;
- protected minimums where governed;
- reclaimability constraints;
- pressure/enforcement truth;
- evidence and accounting identity.

Any aggregate FSATS coordination view must be derivable from constituent truth and must not erase it.

`AGGREGATE_VIEW_DERIVES_FROM_CONSTITUENT_TRUTH = TRUE`

## 4. Two-layer resource model

### Layer A — Foundation authoritative grants and ceilings

Foundation retains authoritative:

- total-resource truth;
- constituent Application grants and ceilings;
- protected Foundation floors/reserves;
- Foundation-governed resource priority/technical criticality;
- final grant/cap/deny/reduce/revoke/reclaim/rebalance/restore decisions.

FSARM may not mutate these authoritative records.

### Layer B — FSARM internal effective distribution

Within a separately authorized coordination envelope, FSARM may govern internal effective distribution/reservation/consumption availability across constituent Applications without changing Foundation authoritative grants/ceilings.

Any internal redistribution SHALL:

- remain within the exact authorized coordination envelope;
- preserve aggregate capacity bounds;
- preserve protected minimums/non-reclaimable constraints;
- preserve isolation/security constraints;
- remain attributable before and after the change;
- be deterministic/reconstructable;
- support bounded restoration;
- fail closed if coordination authority is missing, stale, expired, revoked, ambiguous or split-brain.

If a desired action requires a change to Foundation-authoritative grants/ceilings or exceeds the authorized envelope, FSARM SHALL use the separately governed Foundation request/decision boundary.

`INTERNAL_EFFECTIVE_DISTRIBUTION != FOUNDATION_GRANT`

`INTERNAL_REDISTRIBUTION_FIRST = TRUE`

`FOUNDATION_ADDITIONAL_REQUEST_SECOND = TRUE`

## 5. Priority-domain separation

FSATS may own internal workload urgency/degradation policy and may provide attributable evidence to FSARM.

Foundation remains authoritative for Foundation-governed Application resource priority and Foundation technical criticality.

`FSARM_INTERNAL_URGENCY != FOUNDATION_APPLICATION_PRIORITY`

`FSARM_INTERNAL_URGENCY != FOUNDATION_TECHNICAL_CRITICALITY`

No Application, Guardian, MSA, LSA, CSA or FSARM caller may self-promote Foundation priority or technical criticality.

## 6. Guardian/protection separation

Guardian protection/crisis evidence may contribute to FSARM internal resource urgency only within admitted resource policy.

It SHALL NOT create:

- direct Foundation requester authority for Guardian;
- Foundation technical criticality by assertion;
- Stage 8 Guardian/Safe-State authority;
- lifecycle authority;
- permission to violate Foundation floors, Application protected minimums, non-reclaimable constraints or isolation.

`GUARDIAN_CRISIS_EVIDENCE != FOUNDATION_RESOURCE_AUTHORITY`

## 7. WP-05 disposition

WP-05 remains a Foundation truth-derivation/observation boundary only.

Preserved:

- pressure truth;
- preemption eligibility truth without execution;
- observed enforcement truth without mutation;
- exact Application/global scope separation;
- freshness/expiry/supersession/evidence;
- transition stability;
- fail-closed unavailable state;
- WP-01..WP-04 consumption;
- zero-Application validity.

Required amendment is minimal:

- remove TARC-hard-bound future compatibility language;
- replace it with generic authorized consumer/coordinator compatibility;
- preserve constituent Application truth when aggregate FSARM coordination is later authorized;
- do not implement FSARM identity, request decisions, redistribution execution or load-shedding execution in WP-05.

## 8. WP-06 disposition

WP-06 shall later define the generic governed request/decision boundary supporting an authorized aggregate coordinator role such as FSARM.

It must preserve exact constituent attribution and prove why additional Foundation capacity is needed after applicable internal redistribution evaluation.

WP-06 remains separately gated and unauthorized for implementation.

## 9. WP-07 disposition

WP-07 shall later define the execution boundary for reclamation/redistribution/rebalance/restoration.

It must explicitly separate:

- Foundation-authoritative resource mutations; and
- bounded FSARM internal effective-distribution actions inside a valid coordination envelope.

WP-07 remains separately gated and unauthorized for implementation.

## 10. WP-08 disposition

WP-08 shall later expose safe attributable resource-state/load-shedding signals to individual Applications and authorized aggregate coordinators.

No aggregate view may include unrelated Applications or erase constituent identity.

WP-08 remains separately gated and unauthorized for implementation.

## 11. Current WP-05 code

Current unvalidated implementation through `a8e1dc1befa85b451f9a2a6cfa75e26d544860a8` remains paused.

No executable validation claim is accepted for it.

It may be reused only after file-level reconciliation against the amended WP-05 successor and fresh implementation authorization.

## 12. Governance consequence

Because the accepted WP-05 planning package contained a material cross-workstream TARC-specific compatibility assumption, an amended Owner planning acceptance is required before implementation resumes.

The safest authority interpretation is also to require renewed explicit WP-05 implementation authorization after the amended plan is accepted.

`PRIOR_WP05_PLANNING_ACCEPTANCE = HISTORICALLY_VALID`

`WP05_AMENDED_PLANNING_ACCEPTANCE_REQUIRED = YES`

`PRIOR_IMPLEMENTATION_AUTHORITY_AUTO_CARRIES_FORWARD = NO`

`WP05_IMPLEMENTATION_RESUME = NO`

## 13. Acceptance markers

`FCR0031_RECONCILIATION_v0.2 = REMEDIATED_CANDIDATE`

`WP01_WP04_CLOSURES = PRESERVED`

`IMP001_STAGE_SEQUENCE_CHANGE_REQUIRED = NO`

`FSARM_ROLE_MODEL = DELEGATED_AGGREGATE_COORDINATOR`

`FOUNDATION_RESOURCE_AUTHORITY = PRESERVED`

`OPAQUE_AGGREGATE_POOL = PROHIBITED`

`WP05_PLAN_AMENDMENT_REQUIRED = YES`
