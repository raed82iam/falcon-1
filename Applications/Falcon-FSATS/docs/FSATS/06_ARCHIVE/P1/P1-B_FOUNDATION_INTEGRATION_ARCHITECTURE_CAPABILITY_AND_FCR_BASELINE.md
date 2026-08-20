# P1-B — Foundation Integration Architecture, Capability and FCR Baseline

**Status:** `DESIGN_CANDIDATE / NOT_OWNER_ACCEPTED / NOT_CLOSED`  
**Implementation Authority:** `NOT_GRANTED`  
**Runtime Authority:** `NOT_GRANTED`  
**Organizational Source:** `04_ACTIVE_WORK/PART_1/01_PART1NG_WORK_PACKAGE_DECOMPOSITION.md`

### Objective
Establish the Foundation-side contracts, boundaries, capability state and fail-closed behavior before physical build topology is closed.

### Required outputs
- Foundation integration profile for each FSATS Application and FSARM;
- APP-001 / CON-023 / ADR-I012 / ADR-I015 binding matrix;
- lifecycle/admission/catalog/dependency-governance bindings;
- FIL / Service Bus / event / evidence / security bindings;
- resource-governance binding including FCR-0031;
- current design-time, build-time, runtime-capability and runtime-authority state for each dependency;
- current FCR mapping and review triggers;
- fail-closed behavior for every unresolved or unavailable Foundation dependency;
- explicit prohibition of Application-local Foundation substitutes.

### Closure criteria
No physical Application/project/controller design depends on an unknown Foundation counterpart, invented capability or unresolved authority-bearing binding without an explicit FCR/fail-closed gate.
