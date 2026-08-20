# FSATS V1.4 Part 0 / P0-C — Start and Scope-Control Record

**Status:** `DESIGN_REVIEW_IN_PROGRESS`  
**Scope:** `Part 0 / P0-C only`  
**Branch:** `application-development`  
**Owner authorization:** explicit instruction to begin the next Part 0 work package  
**P0-A:** `OWNER_ACCEPTED_AND_CLOSED`  
**P0-B:** `OWNER_ACCEPTED_AND_CLOSED`  
**P0-C final Owner acceptance:** `NOT_GRANTED`  
**P0-D through P0-L:** `NOT_STARTED`  
**Part 1:** `FROZEN_PENDING_PART0_REMEDIATION`  
**Part 2 through Part 10:** `NOT_AUTHORIZED`

## 1. Objective

P0-C establishes a non-ambiguous proposed Application topology, ownership map, major-branch map, MSA/LSA placement, and CSA eligibility model for the current FSATS V1.4 design.

P0-C SHALL answer only topology, ownership and awareness-jurisdiction questions. It SHALL NOT design detailed manifests, cross-Application contracts, provider behavior, Trading algorithms, Guardian playbooks, performance internals, simulation gates, or implementation code reserved for later work packages.

## 2. Governing inputs

P0-C is controlled by the final accepted P0-A source/review model and the final accepted P0-B concept/disposition set, including the P0-B downstream obligation `OBL-C-01`.

Current Foundation constraints used by P0-C include APP-001, CON-023, ADR-I012 and ADR-I015. In particular:

- every Falcon Application is independently governed and contract-bound;
- every Application declares exactly one MSA;
- every declared major branch owns exactly one LSA;
- CSA is optional and limited to eligible intelligent components;
- awareness rank creates no cross-Application authority;
- FSATS membership creates no reachability, permission, authority, resource ownership or mutable-state ownership;
- direct access to another Application's internals is prohibited.

## 3. Historical V1.3 topology inputs

Accepted P0-B dispositions carried into P0-C include:

- FSATS as a non-owning trading-system/domain boundary;
- Trading Guardian as an independent Application;
- FSAPMA as an independent Application;
- Trading as an independent Application;
- one MSA per Application and one LSA per major branch;
- Guardian four-LSA topology as a retained candidate;
- FSAPMA six-LSA topology as a retained candidate;
- Trading twelve-LSA topology as a retained candidate;
- FSTSimA as an independent non-Live Application with an eight-LSA candidate topology;
- Shared Communication and Shared Web as independent adjacent Applications;
- no hidden FSATS shared runtime/provenance/resource owner.

These are accepted P0-B traceability inputs, not automatic detailed P0-C acceptance. P0-C must still prove the topology against current ownership and major-branch criteria.

## 4. Mandatory P0-C questions

P0-C SHALL explicitly determine:

1. whether FSATS remains a non-owning system/domain boundary;
2. which Applications are current members of the FSATS trading domain and which are adjacent Shared/Validation Applications;
3. one accountable owner for every current major responsibility represented in the topology;
4. exactly one MSA per declared Application;
5. exactly one LSA per declared major branch;
6. CSA eligibility without automatic CSA proliferation;
7. whether any branch duplicates Foundation or another Application owner;
8. whether any topology edge implies hidden cross-Application access or authority;
9. whether historical fixed LSA counts remain justified by coherent major responsibilities;
10. what topology differences from V1.3 are material and require explicit reporting.

## 5. Non-authority

P0-C does not authorize P0-D or later work, Part 1 implementation, Foundation modification, runtime connectivity, provider/broker activation, Paper, Tiny Live, Live, deployment or production adoption.

## 6. Current state

```text
P0-A = OWNER_ACCEPTED_AND_CLOSED
P0-B = OWNER_ACCEPTED_AND_CLOSED
P0-C = DESIGN_REVIEW_IN_PROGRESS
P0-C_OWNER_FINAL_ACCEPTANCE = NOT_GRANTED
P0-D_THROUGH_P0-L = NOT_STARTED
PART1 = FROZEN_PENDING_PART0_REMEDIATION
PART2_THROUGH_PART10 = NOT_AUTHORIZED
RUNTIME / PAPER / TINY_LIVE / LIVE = NOT_GRANTED
```
