# AUT-002 Migration Plan

**Status:** Approved Plan — Execution Not Authorized  
**Approval Record:** GOV-062

## 1. Preserved History

| Artifact | Treatment |
|---|---|
| AUT-002 v1.0 | preserve Approved content, effective date, GOV-003, and current effect |
| ADR-I010 | preserve Accepted decision and GOV-060 |
| AUT-002 v2.0 | preserve Approved successor design and deferred activation |
| AUT-002 v2.1 | Proposed refinement under AMD-006 |
| RSK-006 | Proposed new Trading Guardian Specification |
| CON-022 | Proposed new boundary Contract |

## 2. Responsibility Classification

- Foundation technical responsibilities migrate to AUT-002 v2.1.
- Trading-domain responsibilities migrate to RSK-006.
- shared principles remain duplicated only as boundary obligations with one owning source identified.
- authority validation remains AUT-001.
- enforcement remains CON-011/ADR-F008 and competent execution owners.

## 3. Required Activation Package

Before supersession:

1. Owner approval of AMD-006, ADR-I011, AUT-002 v2.1, RSK-006, and CON-022.
2. versioned CON-011 treatment.
3. technical-criticality and Safe Mode catalogs.
4. Application and Trading Suite Manifest Contracts.
5. registry, Tree, glossary, index, trace, and baseline updates.
6. consistency review with GOV-060 and GOV-061.
7. historical integrity manifest.
8. separate documentary activation decision.

## 4. Atomic Transition

The activation decision SHALL:

- mark AUT-002 v1.0 Superseded;
- make AUT-002 v2.1 current;
- register RSK-006 and CON-022;
- preserve AUT-002 v2.0 as a non-effective Approved design superseded by v2.1;
- activate all required cross-references together; and
- grant no implementation or operational authority.

## 5. Rollback

Before runtime implementation, documentary rollback means revoking the activation decision through competent governance and restoring the last internally consistent documentary baseline without rewriting history.
