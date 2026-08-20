# P1-K — Governed Contract, FIL, Event and Route Materialization

**Status:** `DESIGN_CANDIDATE / NOT_OWNER_ACCEPTED / NOT_CLOSED`  
**Implementation Authority:** `NOT_GRANTED`  
**Runtime Authority:** `NOT_GRANTED`  
**Organizational Source:** `04_ACTIVE_WORK/PART_1/01_PART1NG_WORK_PACKAGE_DECOMPOSITION.md`

### Objective
Materialize the governed cross-Application communication graph after FSARM impact reconciliation without activating runtime routes.

### Required outputs for every governed family
- immutable family ID;
- exact producer/consumer Applications or admitted system-level role where Foundation permits;
- direction;
- purpose/business meaning;
- authority class;
- security class;
- payload/schema identity and versioning rule;
- FIL envelope binding;
- Service Bus route/delivery binding where applicable;
- correlation/causation/idempotency rule;
- observation/effective/expiry/deadline rule as applicable;
- ordering/duplicate/correction rule;
- replay/test/operational classification;
- acceptance/rejection rules;
- Foundation route/event/delivery dependencies;
- positive and negative fixtures.

### Baseline rule
The accepted Part 0 43/43 contract baseline remains preserved. Any new or changed cross-Application family required by FSARM is an explicit prospective semantic delta and SHALL be reconciled, counted and freshly reviewed rather than silently inserted into the accepted historical 43/43 record.

### Closure criteria
The complete current contract graph can be generated/validated deterministically from one canonical declaration set while remaining declaration-only and preserving historical accepted contract evidence.
