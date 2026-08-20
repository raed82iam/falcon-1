# Migration, Compatibility, and Cross-Reference Note

**Status:** Approved Change Set — Execution Deferred  
**Approval Record:** GOV-061

## 1. Migration Principle

This correction is a versioned semantic migration, not a text replacement.

Approved history remains immutable. New documents establish future meaning only after approval.

## 2. Historical Mapping

| Existing reference | Meaning review | Proposed mapping |
|---|---|---|
| AWR-001 in Foundation persistence, communication, crypto, time, health | Foundation operational awareness | FSA |
| “Self Model” in AWR-001 v1.0 | mixed Foundation and business awareness | split into Foundation Self Model plus MSA/LSA/CSA models |
| “Fitness to Operate” in Foundation contracts | Foundation technical fitness unless domain explicitly states otherwise | FSA Foundation Technical Fitness |
| financial state/material exposure | Application/domain awareness | MSA summary, relevant LSA, CAP/FIN/RSK |
| decision and business context | Application/domain awareness | LSA/MSA and DEC/domain owner |
| component competence and improvement | intelligent component awareness | CSA under LSA |

## 3. Documents Requiring Later Versioned Successors

After package approval, prepare controlled successors for:

- `docs/04_SPECIFICATION_TREE.md`;
- `docs/06_FALCON_SELF_AWARE_SYSTEM_CONCEPT_AR.md`;
- `docs/specifications/SPEC-000_REGISTRY.md`;
- `docs/specifications/core/README.md`;
- `docs/releases/FRS-001_FOUNDATION_RELEASE.md` only if the active release description requires naming clarification;
- `docs/contracts/CON-006_HEALTH_AND_FITNESS.md`;
- `docs/foundation/FDN-001_STATE_AUTHORITY_AND_PERSISTENCE_CATALOG.md`;
- `docs/foundation/FDN-002_FIL_INTERACTION_AND_SCHEMA_CATALOG.md`;
- `docs/foundation/FDN-004_FOUNDATION_CONFIGURATION_CATALOG.md`;
- applicable DEC, EVO, RSK, STD, traceability, and verification references.

Accepted ADRs shall not be overwritten. ADR-I009 provides the mapping.

## 4. Contract Compatibility

Existing Foundation contracts remain valid only within their approved Foundation scope.

Required future contract work:

- Foundation Technical Fitness contract;
- Applications Ecosystem Readiness summary;
- Application Fitness summary;
- Component Assessment summary;
- cross-tier escalation and challenge;
- FSA conformance request and outcome;
- awareness-tier identity, provenance, privacy, and freshness.

No business schema shall be added to the Foundation FIL schema merely to support awareness.

## 5. Data Compatibility

No operational awareness data exists in Stage 0; therefore no runtime state migration is required.

Future state shall use distinct owners and stores or logically enforceable ownership boundaries:

- FSA Foundation Self Model;
- MSA ecosystem model;
- LSA Application model;
- CSA component model.

Cross-tier copies are derived summaries, not competing authoritative truth.

## 6. Compatibility Invariants

- AUT-001 remains authority-decision owner.
- AUT-002 remains protective-restriction owner.
- SYS-008 remains health-assessment owner.
- Security Authority remains security-policy owner.
- Foundation FIL and Service Bus remain business-meaning neutral.
- Application users and business data remain Application-owned.
- Stage 0 enabling providers remain unaffected.
- No Approved history is rewritten.

## 7. Stage 1 Prerequisite Register

| Prerequisite | Status | Blocks |
|---|---|---|
| Owner approval of ADR-I009 | Pending | all awareness implementation |
| Owner approval of AWR-001 v2.0 | Pending | FSA implementation |
| Owner approval of AWR-006 | Pending | MSA implementation |
| Owner approval of AWR-007 | Pending | LSA implementation |
| Owner approval of AWR-008 | Pending | CSA implementation |
| registry/tree/glossary activation | Pending | authoritative use of new terms |
| CON-006 successor | Required after architecture approval | cross-tier fitness integration |
| FDN-001/002/004 successors | Required after architecture approval | Foundation state, messaging, configuration |
| FSA conformance outcome catalog | Unresolved | conformance implementation |
| Falcon Architecture Board status | Owner decision required | architecture-review boundary |
| awareness boundary verification | Planned candidate | implementation acceptance |
| Foundation Repair Playbook Contract and catalog | Not created | repair implementation |
| repair authority and post-repair verification Contracts | Not created | autonomous repair |
| candidate-development environment and authority profile | Not created | candidate creation |
| Sandbox and Digital City governance | Not created | candidate validation |
| Owner Communication and Approval Center | Proposed only | governed Owner decision |
| candidate lifecycle and outcome catalogs | Proposed only | evolution workflow |
| post-adoption verification and rollback profile | Not created | trusted baseline transition |
| Guardian final architecture | Explicitly deferred | full Guardian implementation |
| separate Stage 1 authority | Not Granted | all Stage 1 planning and execution |

## 8. Compatibility Result

The proposed hierarchy is compatible with the Stage 0 technical baseline because Stage 0 did not implement operational self-awareness. Compatibility is conditional on Owner approval and controlled successor activation.

The repair/evolution extension is also compatible because it grants no current execution authority. It requires new Contracts, playbooks, environments, evidence, and authority instruments before any repair or candidate-development action.
