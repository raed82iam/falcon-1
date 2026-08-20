# FSATS V1.4 Part 0 / P0-E — Start and Scope Control Record

**Status:** `P0E_STARTED`
**Scope:** canonical Application identity, CON-023 manifest, and APP-001 lifecycle design contract only
**Branch:** `application-development`
**Predecessor state:** P0-A through P0-D `OWNER_ACCEPTED_AND_CLOSED`
**P0-E final Owner acceptance:** `NOT_GRANTED`

## 1. Authority and scope

The Project Owner explicitly authorized progression to the next Part 0 work package after closing P0-D. This record starts P0-E only.

P0-E SHALL bind the accepted P0-C topology to complete canonical Application/MSA/LSA identity and CON-023 manifest-design obligations without changing the accepted P0-C ownership, branch, parent, or awareness-jurisdiction semantics.

P0-E does not authorize P0-F or later work packages, implementation, runtime attachment, admission, activation, external connectivity, Paper, Tiny Live, Live, deployment, production adoption, or Foundation modification.

## 2. Governing anchors

P0-E shall be evaluated against at least:

- accepted P0-C topology and awareness jurisdiction;
- accepted P0-D Foundation/Application boundary;
- APP-001 v1.1;
- CON-023 v1.1;
- ADR-I012 and ADR-I015;
- current Foundation contract/lifecycle semantics;
- applicable open FCR dependencies.

The P0-C planning labels are not canonical manifest IDs. P0-E may assign documentary canonical identities only when the assignment preserves the exact accepted P0-C owner, responsibility, parent, branch and awareness semantics.

## 3. Required P0-E outputs

P0-E shall define for all six accepted Applications:

1. one canonical immutable Application ID;
2. one canonical Package ID namespace/rule;
3. canonical MSA identity;
4. canonical LSA identities for all 38 accepted major branches;
5. CSA declaration/eligibility rule without inventing CSA instances;
6. purpose and owned business boundary;
7. prohibited Foundation responsibilities;
8. version and package provenance/integrity obligations;
9. dependencies and compatible-version declaration rules;
10. required Foundation contracts/services;
11. provided capability declaration rules and consumer binding;
12. permissions, authority requests and security profile;
13. resource minimum/ceiling/priority/degraded behavior declarations;
14. persistence, communication, configuration and evidence declarations;
15. health/failure-containment interfaces;
16. full APP-001 lifecycle behavior;
17. self-development origin-aware route;
18. Guardian/protection interface declaration;
19. update/migration/rollback/corrective-action/removal obligations;
20. FCR/fail-closed handling for unavailable Foundation dependencies.

## 4. Canonical identity rules

- Display name is not Application identity.
- Application ID is not Package ID.
- Application ID is not runtime instance ID.
- MSA/LSA identity is not Application authority.
- FSATS is not an Application ID namespace owner and does not receive its own Application identity.
- Package updates shall not silently mutate Application identity.
- Purpose/ownership change is a material identity change and requires governed review.
- No canonical identity assignment may create a route, permission, resource entitlement, lifecycle state or production authority.

## 5. Historical manifest candidate treatment

`07_MANIFEST_CANDIDATES.md` is historical design input only. It predates the final accepted P0-C topology and contains superseded branch names/responsibility groupings and incomplete treatment of Shared Web/Communication manifests.

P0-E shall preserve useful historical intent only where consistent with accepted P0-C/P0-D semantics. No old identity or branch list is canonical merely because it appeared in `07`.

## 6. Exit gate

P0-E may pass only when a downstream implementer can construct each of the six complete CON-023 manifests without inventing identity, ownership, required lifecycle declarations, awareness parentage, permission defaults, Foundation ownership, or missing-field authority.

```text
P0E = IN_PROGRESS
P0F_THROUGH_P0L = NOT_STARTED
RUNTIME / PAPER / TINY_LIVE / LIVE = NOT_GRANTED
DEPLOYMENT / PRODUCTION_ADOPTION = NOT_GRANTED
```
