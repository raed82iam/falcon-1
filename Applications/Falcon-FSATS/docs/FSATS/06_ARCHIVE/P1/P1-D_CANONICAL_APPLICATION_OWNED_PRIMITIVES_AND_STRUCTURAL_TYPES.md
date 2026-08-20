# P1-D — Canonical Application-Owned Primitives and Structural Types

**Status:** `DESIGN_CANDIDATE / NOT_OWNER_ACCEPTED / NOT_CLOSED`  
**Implementation Authority:** `NOT_GRANTED`  
**Runtime Authority:** `NOT_GRANTED`  
**Organizational Source:** `04_ACTIVE_WORK/PART_1/01_PART1NG_WORK_PACKAGE_DECOMPOSITION.md`

### Objective
Define reusable Application-owned primitives without duplicating Foundation-owned semantics.

### Required rule
For every candidate primitive:

```text
FOUNDATION_OWNED_SEMANTIC?
 -> CONSUME FOUNDATION SEMANTIC

APPLICATION_BUSINESS_OWNED?
 -> DEFINE APPLICATION PRIMITIVE

DOMAIN_WRAPPER_AROUND_FOUNDATION_ID?
 -> ALLOW ONLY WITH EXPLICIT MAPPING
```

### Candidate Application-owned families
- user/account/market/instrument/provider/broker/environment/domain identifiers where Application-owned;
- money/quantity/price/exposure representations with exact unit/currency semantics;
- confidence/quality/fitness representations where Application-owned;
- bounded result/reason categories;
- FSARM resource-intent/value types only where not owned by Foundation, including Application-local minimum-safe, desired, reclaimable and workload-priority evidence representations.

### Forbidden scope
- reimplementation of Foundation identity, FIL, Service Bus, Manifest, security, lifecycle, total-resource or event-system ownership;
- cloning Foundation time/correlation/causation/evidence semantics under new names;
- one “common” type that collapses distinct business meanings merely because storage shape matches.

### Closure criteria
All shared primitives have one owner, exact semantics, serialization rules, invalid-state rules, equality rules and negative fixtures.
