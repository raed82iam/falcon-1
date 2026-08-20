# GOV-070 Stage 1 WP-01 Controlled Rollback and Replay Authorization

## Decision

Approved for a controlled rollback and evidence-complete replay of WP-01 only.

## Authority preserved

- Stage 1 remains `STARTED_IN_PROGRESS`.
- Stage 1 execution authority remains `GRANTED_ACTIVE`.
- WP-02 remains unauthorized.
- FIAI-STAGE1-001 is not expanded.
- No new Stage 1 start is created.

## Purpose

Produce a separate canonical replay evidence chain for WP-01 without altering
the original historical execution record.

## Exact replay subject

`Falcon.Foundation.ControlledProjectFoundation.slnx`

## Permitted operations

- preserve the original WP-01 records as historical evidence;
- preserve the prior independent reviews;
- preserve GOV-068 and GOV-069;
- preserve the original WP-01 artifact externally before rollback;
- remove only the WP-01 solution artifact from the repository;
- recreate only the same solution artifact content;
- capture replay commands contemporaneously;
- create replay evidence records.

## Prohibited operations

- no WP-02 execution;
- no modification of any other implementation artifact;
- no modification of FIAI issuance, acceptance, or activation records;
- no reconstruction of original command history as if it were contemporaneous;
- no new Stage 1 start;
- no authority expansion.

## Evidence-capture requirements

- external raw-evidence directory:
  `C:\falcon\ExecutionEvidence\Stage1\WP-01-Replay-001`
- concurrent command capture required for the replay session;
- retrospective evidence must remain labeled retrospective when used.

