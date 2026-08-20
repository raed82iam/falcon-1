# FSATS V1.4 Part 0 - GitHub Canonical Review Source Policy

**Status:** `ACTIVE_FOR_PART0_REMEDIATION`  
**Scope:** FSATS V1.4 Part 0 design remediation only  
**Owner direction date:** `2026-08-07`  
**Owner clarification incorporated:** `2026-08-08`  
**Writable branch:** `application-development`

## 1. Governing review rule

For Part 0 remediation, GitHub repository content is the direct review source whenever the governing or historical artifact exists there.

No architectural conclusion may rely only on assistant memory, conversation recollection, an uncited summary, or reconstructed interpretation when the actual source can be reviewed.

Part 0 SHALL distinguish:

1. governing constraints and current Falcon/Foundation rules;
2. explicit Owner objectives and decisions;
3. historical design references, including FSATS V1.3;
4. engineering, Architecture, Security and Red-Team assessment; and
5. implementation/evidence state.

These are not interchangeable authority levels.

## 2. Repository source classes

### A. Current Falcon governing sources

Current Vision, Constitution, governance, effective Specifications and Standards, accepted ADRs, approved Contracts and applicable canonical architecture documentation SHALL be read from their current authoritative repository locations before semantic reliance.

Registry/index presence is discovery evidence, not a substitute for the underlying artifact semantics.

### B. Current Foundation realization/evidence sources

Foundation implementation commits, verification results, closure records and other evidence may establish current realization, capability availability or accepted implementation state.

They SHALL NOT silently redefine governing meaning.

Mutable Foundation state SHALL be freshly revalidated when relied upon by a Part 0 work package.

### C. FSATS V1.3 historical design reference

The historical FSATS V1.3 reference source is controlled through:

- read-only branch: `reference/fsats-v1.3-scratch`;
- complete package inventory under `23`, `23A`, `23B`, and `23C`;
- package SHA-256: `d7fbde9fa0a584d9bb77f388016c9deb5fecdc30b0b9c3c0c7087743b32ac223`;
- 289 ZIP entries, 273 files, 16 directory entries.

FSATS V1.3 is `HISTORICAL_DESIGN_REFERENCE`.

It SHALL be reviewed to preserve knowledge and prevent accidental omission, but it is not a binding current architecture baseline and has no veto authority over a better justified V1.4 design.

### D. Current FSATS V1.4 remediation work

All authorized Part 0 remediation records and proposed design artifacts are written only on `application-development` and only under the authorized `applications/**` boundary.

## 3. Mandatory source handling

Before a Part 0 Work Package reaches Owner-review-candidate state, it SHALL:

1. identify and review relevant Vision/Constitution constraints;
2. identify and review applicable current governance, Specifications, Standards, ADRs and Contracts;
3. identify and review the relevant V1.3 source artifacts from the complete controlled inventory;
4. identify relevant explicit Owner directions/corrections;
5. verify current Foundation-facing rules and, separately, current implementation/evidence/capability state;
6. freshly verify relevant FCR state;
7. evaluate the V1.3 approach and credible alternatives;
8. select and justify the proposed V1.4 design;
9. record material differences from V1.3;
10. assign one explicit disposition where a material V1.3 item is involved; and
11. record downstream implementation and verifier/evidence obligations.

## 4. V1.3 disposition rule

The permitted disposition vocabulary is:

- `RETAINED`
- `IMPROVED`
- `MODIFIED_FOR_CURRENT_ARCHITECTURE_ALIGNMENT`
- `REPLACED_BY_BETTER_DESIGN`
- `REMOVED_WITH_JUSTIFICATION`
- `OWNER_DIRECTION`
- `OWNER_DECISION_REQUIRED`

Silence is not a disposition.

## 5. Better-design / no-uncontrolled-redesign rule

Part 0 SHALL avoid both blind preservation and uncontrolled redesign.

The required sequence is:

```text
UNDERSTAND THE V1.3 SOLUTION
→ UNDERSTAND THE PROBLEM / OUTCOME IT ADDRESSED
→ REVIEW CURRENT FALCON / FOUNDATION CONSTRAINTS
→ EVALUATE ALTERNATIVES
→ CHOOSE THE STRONGEST JUSTIFIED DESIGN
→ RECORD MATERIAL DIFFERENCES AND TRADE-OFFS
```

A better design may replace V1.3 when justified.

An old design may remain when it is still the strongest justified design.

## 6. Owner direction and Vision / Constitution conflict handling

The Project Owner may direct architectural/design changes.

If a requested direction appears to conflict with Falcon Vision or Constitution, the review SHALL:

- identify the apparent conflict;
- explain it clearly;
- identify the intended Owner outcome;
- propose compliant alternatives intended to produce the same or closest legitimate result;
- describe material trade-offs; and
- return the matter to the Owner for decision through the proper governance process.

A previous V1.3 design is not by itself a reason to reject an Owner direction.

## 7. Mandatory post-change review cycle

If the Owner reviews a candidate and requests any semantic change, that review is conditional rather than final acceptance.

The changed artifact SHALL complete:

```text
DESIGN REMEDIATION
→ FRESH ARCHITECTURE / CONSISTENCY RE-REVIEW
→ FRESH RED-TEAM REVIEW OF THE CHANGED VERSION
→ POST-CHANGE REPORT
→ OWNER FINAL REVIEW
```

Only explicit Owner approval after that cycle establishes final acceptance/closure.

A previous Red-Team result SHALL NOT be reused as proof for a semantically changed artifact.

## 8. GitHub-read and identity verification

Every work package SHALL record enough evidence to reconstruct what was reviewed, including where material:

- repository;
- branch;
- commit/snapshot identity;
- path;
- version/status;
- blob/digest where available;
- review instant where useful; and
- freshness revalidation for mutable state.

A package inventory path is not proof that content was reviewed.

Historical V1.3 validation reports are evidence of package/delivery facts and SHALL NOT replace semantic review of underlying design artifacts.

## 9. Branch protection rule

Application work SHALL NOT write to:

- `reference/fsats-v1.3-scratch`;
- `foundation-development`;
- `main`.

Those sources remain read-only from this workstream unless separately and explicitly authorized by the Owner under the proper workstream.

## 10. Relationship to Part 0 Work Packages

This policy is a mandatory input to P0-A and every later Part 0 remediation Work Package.

No P0-B through P0-L final acceptance is valid unless its conclusions are traceable to the source classes, review cycle, and difference-reporting rules defined here and in P0-A.

## 11. Authority boundary and current operational status

This policy authorizes no Part 1 remediation implementation, Part 2+ implementation, Foundation modification, external connectivity, provider activation, broker activation, Paper, Tiny Live, Live, deployment, production adoption, or paid-service purchase.

The block below is an operational status snapshot only. Updating it to reflect later verified state does not alter Sections 1–10.

```text
PART0_REMEDIATION = ACTIVE
P0A = OWNER_ACCEPTED_AND_CLOSED
P0B = PASS_READY_FOR_OWNER_REVIEW
P0B_CONTENT_REVIEW = COMPLETE
P0B_273_FILE_COVERAGE = PASS
P0B_MATERIAL_CONCEPT_COVERAGE = 120 / 120
P0B_DIFFERENCE_COMPLETENESS = PASS
P0B_ALTERNATIVE_CHALLENGE = PASS
P0B_DOWNSTREAM_PROOF_OBLIGATIONS = 120 / 120
P0B_ARCHITECTURE_REVIEW = PASS
P0B_RED_TEAM = PASS
P0B_CRITICAL = 0
P0B_HIGH = 0
P0B_MEDIUM = 0
P0B_LOW = 0
P0B_TOTAL_ERRORS = 0
P0B_FINAL_OWNER_ACCEPTANCE = NOT_GRANTED
P0C_THROUGH_P0L = NOT_STARTED
PART1 = FROZEN_PENDING_PART0_REMEDIATION
PART2_THROUGH_PART10 = NOT_AUTHORIZED
RUNTIME / PAPER / TINY_LIVE / LIVE = NOT_GRANTED
DEPLOYMENT / PRODUCTION_ADOPTION = NOT_GRANTED
FOUNDATION_MODIFICATION_FROM_APPLICATION_WORKSTREAM = NOT_AUTHORIZED
```
