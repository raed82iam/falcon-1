# STG-0B-EXIT-001 — Stage 0B Exit and Cleanup Criteria

**Identifier:** STG-0B-EXIT-001  
**Version:** 1.0  
**Status:** Approved  
**Proposal Date:** 2026-07-26  
**Approval Date:** 2026-07-26  
**Governing Authority:** STG-0B-PROP-001; STG-0B-EVD-001; STG-0B-VER-001  
**Approval Record:** GOV-051  
**Stage 0C Authority:** Not Granted

## 1. Purpose

This candidate defines completion, failure, stop, cleanup, and preservation rules for Stage 0B.

## 2. Completion Criteria

Stage 0B may be assessed complete only when:

- every authorized Candidate ID has a final finding;
- all mandatory evidence is `COMPLETE` and integrity-valid;
- applicable VPL-BST-003 through VPL-BST-005 results are preserved;
- every candidate remains visibly non-active;
- dependencies and provenance are complete;
- financial isolation is `FINANCIALLY_ISOLATED`;
- synthetic-material custody and disposition are complete;
- material Challenges are resolved;
- uncertainty is explicit;
- repository status and changes are recorded;
- cleanup is verified;
- and an independent completion assessment is produced.

## 3. Failure and Stop Criteria

Stage 0B shall be `FAILED` or `STOPPED` when:

- authority is absent or exceeded;
- a prohibited candidate or behavior is created;
- evidence is incomplete or invalid;
- a candidate cannot be independently evaluated;
- secret custody fails;
- financial isolation is not proven;
- a candidate is represented as active;
- an unapproved dependency or network destination is required;
- or a governing rule is violated.

## 4. Cleanup

Cleanup shall:

- remove ephemeral builds and temporary outputs;
- destroy ephemeral test secrets and keys;
- close the candidate-verification environment;
- preserve approved non-secret evidence;
- preserve source and artifacts only as explicitly governed candidates;
- confirm no background process remains;
- confirm no listener, scheduled task, service, or cloud resource was created;
- and record unresolved residue.

## 5. Repository Closure

The final record shall identify:

- repository state before and after;
- all created, changed, and removed files;
- candidate artifact digests;
- evidence commit identity;
- unresolved risks;
- and any preserved candidate material.

## 6. Exit States

```text
STAGE_0B_COMPLETE — STAGE_0C MAY BE PROPOSED
STAGE_0B_INCOMPLETE — REMEDIATION REQUIRES AUTHORITY
STAGE_0B_FAILED — CANDIDATES SHALL NOT ADVANCE
STAGE_0B_STOPPED — AUTHORITY REVIEW REQUIRED
```

## 7. Non-Progression Rule

Completion, successful verification, or a conforming candidate shall not authorize:

- candidate Acceptance;
- Provider or Profile Activation;
- Stage 0C;
- Stage 1;
- production;
- cloud deployment;
- or financial activity.

Stage 0C requires a separate proposal, complete evidence review, competent Acceptance and Activation authorities, and explicit Project Owner approval.
