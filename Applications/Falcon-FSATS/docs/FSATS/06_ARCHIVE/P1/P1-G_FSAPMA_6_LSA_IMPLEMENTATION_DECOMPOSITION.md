# P1-G — FSAPMA 6-LSA Implementation Decomposition

**Status:** `DESIGN_CANDIDATE / NOT_OWNER_ACCEPTED / NOT_CLOSED`  
**Implementation Authority:** `NOT_GRANTED`  
**Runtime Authority:** `NOT_GRANTED`  
**Organizational Source:** `04_ACTIVE_WORK/PART_1/01_PART1NG_WORK_PACKAGE_DECOMPOSITION.md`

### Objective
Create the code-ready implementation architecture for the sole operational external-data/provider-management Application.

### Required branch coverage
1. P-LSA-01 Provider Registry and Onboarding
2. P-LSA-02 Data Products, Semantics and Normalization
3. P-LSA-03 Provider Capability, Account and Entitlement
4. P-LSA-04 Provider Selection, Routing and Delivery
5. P-LSA-05 Data Quality, Verification and Reconciliation
6. P-LSA-06 Quota, Capacity, Cost and Reliability

Provider Controller remains an operational controller inside P-LSA-04 and is not a CSA.

### FSARM requirement
FSAPMA SHALL expose attributable resource need, current consumption, minimum-safe live-data requirements, reclaimable/degradable workload, pressure and restoration evidence to FSARM without transferring provider/data business authority to FSARM.

### Required outputs
Provider registry model, Data Product model, capability/entitlement truth, routing/controller model, quality/reconciliation model, quota/capacity/cost model, internal state boundaries, FSARM resource interface, external-egress gate, delivery contract binding and complete verifier plan.

### Closure criteria
No provider-specific operational data path bypasses FSAPMA, and provider quota/capacity semantics never become Foundation technical-resource authority or FSARM business authority.
