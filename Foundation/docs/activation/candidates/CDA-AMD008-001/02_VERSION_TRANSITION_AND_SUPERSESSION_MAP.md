# Version Transition and Supersession Map

**Status:** Proposed  
**Activation:** Not Authorized

## Transition Map

| ID | Current version/status | Target version/status at activation | Supersession |
|---|---|---|---|
| AWR-001 | v1.0 Approved/effective | complete v2.1 successor, Proposed; eventual Approved/effective state requires separate decision | v1.0 becomes Superseded only after complete successor approval and CDA activation |
| AWR-006 | no canonical active version | v2.0 Approved/effective | AMD-004 design remains historical, never rewritten |
| AWR-007 | no canonical active version | v2.0 Approved/effective | AMD-004 design remains historical |
| AWR-008 | no canonical active version | v1.1 Approved/effective | AMD-004 design remains historical |
| APP-001 | Planned registry entry | v1.1 Approved/effective | Planned entry is replaced administratively, not archived as a document |
| CON-023 | absent | v1.1 Approved/effective | no predecessor |
| SYS-003 | Candidate Migration registry entry | v1.1 Approved/effective | registry state changes; legacy sources remain history |
| SYS-004 | Candidate Migration registry entry | v1.0 Approved/effective | registry state changes |
| SYS-006 | Candidate Migration registry entry | v1.1 Approved/effective | registry state changes |
| ADR-I009 | Accepted; activation deferred; historical design | remains immutable historical decision | ADR-I015 becomes current architecture; index records ADR-I009 as superseded by ADR-I015 without editing ADR-I009 |
| ADR-I015 | approved pending activation | document `Status: Approved`; ADR decision disposition `Accepted`; Documentary Activation `Active` | none |

## Supersession Rules

1. Supersession occurs only at the effective instant named in a future Project Owner activation record.
2. No predecessor is edited in place, deleted, or silently reinterpreted.
3. The successor metadata, canonical index, registry, and immutable activation manifest record lineage.
4. A preserved predecessor copy is stored under an explicit archive path before canonical pointer change.
5. An ADR's historical content remains immutable; its index disposition may identify a later superseding ADR.
6. A Planned or Candidate Migration registry entry has no content to archive; its registry history is preserved through the superseded registry version.
7. Partial activation is forbidden. Any missing target, digest mismatch, duplicate ID, broken link, or inconsistent status aborts the transition.
8. Approval, document effectiveness, runtime activation, implementation authority, and production authority remain separate.

## Status Normalization

GOV-001 permits `Approved` as a document status and treats activation separately.

At activation:

- canonical successor documents use `Status: Approved`;
- `Documentary Activation: Active` is recorded separately;
- `Approval Record: GOV-063`;
- `Activation Record: <future Owner record>`;
- candidate package records retain their decision-time classification and provenance.

ADR-I009 retains its original document metadata and text. ADR-000 v2.7 alone records its historical decision disposition as superseded by ADR-I015. `Accepted` is an ADR decision disposition, not the document authority status.

This normalization is prospective and does not change GOV-063's wording.
