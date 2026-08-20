# AMD-005 Registry and Migration Change Set

**Status:** Approved Change Set — Execution Deferred  
**Approval Record:** GOV-060

## 1. Identifier Decision

`AUT-002` SHALL remain the Guardian specification identifier.

If approved, `AUT-002 v2.0` becomes the proposed successor for Falcon Foundation Guardian. `AUT-002 v1.0` remains historically immutable.

No identifier is assigned to a future Application Guardian by AMD-005.

## 2. Required Controlled Updates After Approval

The activation change set SHALL update:

- Specification Registry;
- Core specification index;
- Specification Tree;
- glossary;
- governance cross-references;
- Foundation Release baseline references;
- `CON-011`;
- `FDN-005`;
- `ADR-F008` impact note;
- `VPL-006`; and
- AMD-004 cross-references if AMD-004 is approved.

## 3. Semantic Migration

| Existing term | Successor meaning |
|---|---|
| Guardian in Foundation technical context | Falcon Foundation Guardian |
| Guardian threat to capital | reserved for future Application/domain protection authority |
| `SAFE` in general lifecycle context | remains lifecycle state; not automatically identical to `PLATFORM_SAFE` |
| protective restriction | must declare jurisdiction and issuer kind |
| application priority | technical criticality only when consumed by FFG |

## 4. Compatibility Rule

Existing references SHALL NOT be bulk-renamed without semantic review. Each reference must be classified as:

- Foundation technical protection;
- Application/domain protection;
- shared abstract principle; or
- ambiguous and requiring Owner review.

## 5. Activation Order

1. Approve ADR-I010 and AUT-002 v2.0.
2. Approve supporting authority and criticality decisions.
3. Prepare versioned successors for affected Contracts and verification plans.
4. Update registries and cross-references atomically.
5. preserve `AUT-002 v1.0` as Superseded historical evidence.

No step in this file grants execution authority.
