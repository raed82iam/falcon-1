# Guardian Architecture Impact Assessment

**Status:** Approved Assessment  
**Approval Record:** GOV-062  
**Assessment Scope:** Pre-Stage 1 documentary architecture

## 1. Executive Finding

The required separation is compatible with Falcon Vision, Constitution, GOV-060, and GOV-061.

AMD-005 correctly defined FFG and reserved a future Application Guardian, but did not fully specify Trading Guardian or the cross-Application protection-request boundary. AMD-006 completes that work without reopening FSA naming, Self-Repair, or Controlled Self-Evolution.

## 2. Current-State Findings

| Subject | Current state | Impact |
|---|---|---|
| AUT-002 v1.0 | Approved and effective | mixes technical, capital, and Falcon-wide protection; preserve unchanged |
| AUT-002 v2.0 | Approved successor design; activation deferred by GOV-060 | retain historically; propose v2.1 refinement |
| ADR-I010 | Accepted architecture; activation deferred | remains valid; ADR-I011 extends it to Trading Guardian |
| AWR-001 v2.0 and AWR-006–008 | Approved designs; activation deferred by GOV-061 | compatible; terminology and escalation references require later activation alignment |
| CON-011 | Approved Foundation protective-restriction Contract | requires versioned jurisdiction field and issuer-kind review |
| FDN-005 and ADR-F008 | Approved Foundation protection/enforcement sources | remain applicable to FFG; Trading restriction treatment requires explicit separation |
| APP-001, SYS-003, SYS-006 | planned or candidate migration | required before Application admission, service catalog, and resource governance can be realized |
| FSATA, FSAOL, Trading Risk, Broker Execution, Provider Management | no registered complete specifications found | relationships can be bounded, but behavior cannot be invented |
| Stage 0 source and evidence | enabling providers and trust primitives only | no Guardian, FSA, Trading Guardian, or trading runtime exists to migrate |

## 3. Mixed Responsibilities

### Foundation technical protection

- technical containment and isolation;
- Platform Safe Mode;
- FIL, Service Bus, Runtime, Persistence, Security, and shared-resource protection;
- restriction persistence;
- cross-Application technical conflict resolution.

### Trading-domain protection

- capital-protection state;
- exposure, orders, positions, protective orders, and execution uncertainty;
- Trading authority restriction;
- Trading Safe Mode and Trading-domain recovery.

### Shared Guardian principles

- explicit mandate;
- proportionate restriction;
- independent evidence;
- persistent restrictions;
- immutable intervention history;
- independent challenge;
- authorized release.

Shared principles do not create shared jurisdiction.

## 4. Conflicts and Gaps

1. `SAFE` currently appears as a general lifecycle/protection term; Platform and Trading modes require qualified names.
2. No Contract exists for an Application Guardian to request cross-Application protection.
3. Technical criticality is referenced but lacks an activated governed catalog.
4. No approved Trading Suite Manifest defines Trading Guardian as mandatory.
5. No approved Application Manifest Contract declares Guardian dependency, degraded modes, or technical criticality.
6. No registered Trading Guardian Specification exists.
7. Cross-Application isolation authority is implicit in current Guardian wording and must become explicit.
8. Resource ownership remains with future SYS-006; FFG may direct emergency protection but may not schedule ordinary resources.
9. FSATA independence cannot be tested because FSATA is not yet specified.
10. Trading Guardian cannot execute broker actions directly; Broker Execution remains a missing separately governed specification.

## 5. Required Migration

- accept ADR-I011;
- approve AUT-002 v2.1 as successor design;
- register and approve RSK-006 and CON-022;
- prepare CON-011 v1.1 or successor with explicit Guardian jurisdiction;
- define Application and Trading Suite Manifest requirements;
- define technical criticality catalog and conflict policy;
- prepare Safe Mode and release-authority catalogs;
- align registries, Tree, glossary, diagrams, traceability, and verification plans;
- activate the complete documentary change set atomically through a separate decision.

## 6. Stage 1 Impact

AMD-006 adds prerequisites; it does not authorize Stage 1.

Before implementation, Falcon requires approved specifications for Application admission, Trading Suite boundaries, Trading Risk, Broker Execution, relevant awareness layers, Service Catalog, Resource Governance, and the Guardian Contracts and catalogs listed here.

## 7. Result

No constitutional conflict was found. The architecture can proceed to Owner review as a Proposed documentary correction.
