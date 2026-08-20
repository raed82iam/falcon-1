# FSATS V1.4 Part 0 / P0-E — FSTSimA Trading-Container Placement Hardening

**Status:** `OWNER_DIRECTED_SEMANTIC_HARDENING_APPLIED`  
**Scope:** corrects FSTSimA placement without changing its Application independence, non-Live authority, accepted branches, MSA/LSA topology, or Foundation lifecycle requirements  
**P0-E final Owner acceptance:** `NOT_GRANTED`

## 1. Owner clarification

The Project Owner clarified that Falcon Self-Aware Trading Simulation Application (FSTSimA) exists only to serve the Trading business domain. Therefore it belongs inside the current Trading main-business container (FSATA/FSATS trading container) rather than being modeled as an adjacent container-level Application outside the Trading container.

This is a containment/classification correction only. It does not merge FSTSimA into the Trading Application.

## 2. Correct Trading-container membership

The current Trading main-business container contains four independent Falcon Applications:

1. Falcon Trading Guardian Application;
2. Falcon Self-Aware Provider Management Application (FSAPMA);
3. Falcon Self-Aware Trading Application;
4. Falcon Self-Aware Trading Simulation Application (FSTSimA).

Shared Web Application and Shared Communication Application remain independent Shared Applications outside the Trading container and may serve other authorized future main-business containers.

## 3. Container is not an Application

The Trading container is an architectural/business grouping boundary only.

It SHALL NOT own:

- a CON-023 Application Manifest;
- an MSA, LSA or CSA hierarchy above its member Applications;
- mutable runtime state;
- Foundation lifecycle state;
- Foundation resources;
- credentials;
- routes or permissions;
- admission/activation authority;
- business authority inherited by member Applications.

Container membership SHALL NOT create reachability, authority, shared memory, shared resources, shared credentials or shared awareness jurisdiction.

## 4. FSTSimA remains an independent Application

Moving FSTSimA inside the Trading container SHALL NOT change the following accepted rules:

- FSTSimA retains its own canonical Application identity, Package identity and MSA;
- FSTSimA retains exactly eight accepted major branches and eight LSAs;
- FSTSimA has its own APP-001 lifecycle and CON-023 Manifest;
- FSTSimA has its own Foundation resource allocation and permissions;
- FSTSimA does not inherit Trading Application permissions, credentials, routes, state or authority;
- FSTSimA remains non-Live by design;
- FSTSimA cannot obtain Live broker credentials/routes merely because it shares the Trading container;
- FSTSimA simulation/replay/test evidence cannot become Live-authoritative truth by containment;
- FSTSimA remains separately failure-contained and removable without converting Trading into a simulation owner.

## 5. Relationship to the Trading Application

The Trading Application and FSTSimA are sibling Applications inside the same Trading container.

Any exchange between them remains cross-Application communication and therefore requires declared governed contracts and Foundation-supported routes. Direct internal access remains forbidden.

FSTSimA may validate Trading components, candidates and behavior only through governed interfaces/evidence. It does not become the owner of Trading strategy, Trading Risk, Trading execution or Trading production state.

The Trading Application does not become owner of FSTSimA simulation state, simulation clock, simulated providers/brokers/accounts, fault injection or fidelity/oracle evidence.

## 6. Shared Applications remain outside main-business containers

Shared Web Application and Shared Communication Application remain independent shared Falcon Applications.

They SHALL NOT be structurally owned by the Trading container merely because Trading currently consumes them.

Future authorized main-business containers such as Accounting, Sales or Warehouse/Inventory may consume Shared Applications through governed contracts without transferring Shared Application ownership to any consuming container.

## 7. Falcon OS extensibility remains open

This correction does not establish a maximum number of Falcon Applications or main-business containers.

Falcon OS may admit future Applications and future main-business containers through normal governed APP-001/CON-023 lifecycle, authority, dependency, resource and security processes without requiring Foundation redesign merely because the domain is new.

“Open-ended extensibility” SHALL NOT mean auto-admission, inherited permission or bypass of governance.

## 8. Canonical P0-E interpretation after this correction

For the current P0-E design scope:

- Trading main-business container Applications = 4;
- independent Shared Applications currently in scope = 2;
- total current canonical Application identities being specified by P0-E = 6;
- Falcon OS maximum Application count = not bounded by P0-E;
- Falcon OS maximum main-business-container count = not bounded by P0-E.

The total of six is therefore a current design-scope inventory, not a Falcon OS architectural maximum.

## 9. Review requirement

Because this is a semantic placement correction, a fresh Architecture/Consistency and Red-Team review of the changed P0-E candidate is mandatory before Owner final acceptance.

```text
P0E_FSTSIMA_CONTAINER_PLACEMENT = OWNER_CLARIFIED_AND_APPLIED
P0E_FRESH_POST_CHANGE_REVIEW = REQUIRED
P0E_FINAL_OWNER_ACCEPTANCE = NOT_GRANTED
P0F_THROUGH_P0L = NOT_STARTED
```
