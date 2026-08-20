# P1-H — Trading Guardian 4-LSA Implementation Decomposition

**Status:** `DESIGN_CANDIDATE / NOT_OWNER_ACCEPTED / NOT_CLOSED`  
**Implementation Authority:** `NOT_GRANTED`  
**Runtime Authority:** `NOT_GRANTED`  
**Organizational Source:** `04_ACTIVE_WORK/PART_1/01_PART1NG_WORK_PACKAGE_DECOMPOSITION.md`

### Objective
Create the code-ready independent protection/crisis Application architecture.

### Required branch coverage
1. G-LSA-01 Protection Observation and Incident Qualification
2. G-LSA-02 Protection Scope, Restriction and Command Governance
3. G-LSA-03 Crisis State, Survival and Protection Coordination
4. G-LSA-04 Reconciliation, Recovery and Protection Evidence

### FSARM crisis interaction
Guardian MAY provide attributable crisis/protection resource need, urgency, minimum-safe requirement and consequence-of-starvation evidence to FSARM.

Guardian SHALL NOT directly seize resources from another Application or become Foundation Resource Governance. FSARM performs any governed resource redistribution based on current policy/evidence.

During a crisis, FSARM may preferentially reallocate eligible resources toward Guardian protection obligations when this is justified by current consequence-aware evidence and protected minimums.

### Required outputs
Protection-state model, incident model, command/directive model, expiry/scope/idempotency rules, crisis state machine, recovery evidence, self-health/fail-safe behavior, exact FSARM resource interaction, exact cross-App contract bindings and negative authority tests.

### Closure criteria
Guardian cannot become Trading Risk, execution truth, provider truth, FSARM, Foundation resource authority or a general Application supervisor.
