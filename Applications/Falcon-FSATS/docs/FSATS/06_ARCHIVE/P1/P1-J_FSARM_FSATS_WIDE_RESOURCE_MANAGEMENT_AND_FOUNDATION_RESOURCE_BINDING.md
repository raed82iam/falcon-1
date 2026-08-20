# P1-J — FSARM: FSATS-Wide Resource Management and Foundation Resource Binding

**Status:** `DESIGN_CANDIDATE / NOT_OWNER_ACCEPTED / NOT_CLOSED`  
**Implementation Authority:** `NOT_GRANTED`  
**Runtime Authority:** `NOT_GRANTED`  
**Organizational Source:** `04_ACTIVE_WORK/PART_1/01_PART1NG_WORK_PACKAGE_DECOMPOSITION.md`

### Objective
Materialize FSARM as the single FSATS-wide operational resource-management authority for the Trading System resource envelope, subject to Foundation reconciliation under FCR-0031.

### Prime rules

```text
INTERNAL_REDISTRIBUTION_FIRST
FOUNDATION_ADDITIONAL_REQUEST_SECOND
REQUESTED_RESOURCE != GRANTED_RESOURCE
```

### Required FSARM responsibilities
- maintain attributable current resource picture across Trading, FSAPMA, Guardian and FSTSimA;
- maintain current effective allocation and consumption;
- receive minimum-safe / survival requirement;
- receive desired allocation and pressure/urgency evidence;
- identify reclaimable capacity and degradation/shedding eligibility;
- reserve/rebalance/reclaim/throttle/shed/suspend eligible workloads;
- preserve protected live-critical and safety obligations according to governed current evidence;
- perform controlled staged restoration;
- calculate proven remaining deficit after safe internal redistribution;
- request additional resources from Foundation Resource Governance only for the remaining evidenced deficit when the governed Foundation boundary exists and is authorized;
- consume Foundation grant/partial/cap/deny/reduce/revoke/reclaim/rebalance/restore outcomes without treating a request as a grant;
- preserve exact per-Application attribution/accountability/isolation.

### Priority principle
FSARM SHALL NOT rely on one permanent Application ranking. It SHALL evaluate active obligation, consequence of starvation, minimum-safe requirement, reclaimability, current pressure, current protection state and admitted resource policy.

Design intent preferentially protects, when applicable, capital protection, crisis handling, reconciliation, open-position safety and required operational data paths before simulation, experimentation, discovery, analytics, research and other deferrable work.

### Foundation authority separation

```text
FSARM != FOUNDATION_RESOURCE_GOVERNANCE
```

Foundation remains sole total-resource truth and final grant/cap/deny/reduce/revoke/reclaim/rebalance/restore authority.

### Non-authorities
FSARM does not own Trading decisions, Unified Risk, Guardian commands, provider/data truth, simulation/validation truth, lifecycle authority, FSA governance, security authority or Owner authority.

### Required outputs
- FSARM structural identity and placement;
- FSARM state model;
- per-Application resource contract/profile;
- minimum-safe and protected-floor semantics;
- reclaimability/degradation classes;
- dynamic priority evidence model;
- internal redistribution decision model;
- remaining-deficit calculation model;
- Foundation request/outcome binding;
- pressure/revocation/restoration model;
- fencing/split-brain/fail-closed rules;
- complete positive/negative/adversarial verifier plan;
- explicit FCR-0031 reconciliation evidence.

### Closure criteria
FSARM can redistribute existing FSATS capacity safely and accountably before seeking additional Foundation capacity; no Application can bypass it for FSATS resource control; no FSARM action can create Foundation resources or business authority; FCR-0031 Foundation reconciliation is incorporated and Application-verified where required.
