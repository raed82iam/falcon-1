# 10 - Stage 1 Failure, Stop, Recovery, and Rollback Plan

| Condition ID | Trigger | Immediate stop action | Preserved evidence | Rollback target | Restart prerequisite | Owner escalation required |
|---|---|---|---|---|---|---|
| C-01 | required manifest expires | stop execution and quarantine outputs | manifest snapshot and evidence | pre-Stage-1 repository snapshot or commit identity | refreshed manifest status | Yes |
| C-02 | environment identity drift | stop execution and isolate environment | environment identity record | pre-Stage-1 repository snapshot or commit identity | exact environment revalidation | Yes |
| C-03 | tool version drift | stop execution | tool identity record and digest | pre-Stage-1 repository snapshot or commit identity | exact toolchain restatement | Yes |
| C-04 | source or digest mismatch | stop execution and quarantine outputs | source and digest comparison evidence | pre-Stage-1 repository snapshot or commit identity | corrected source or digest identity | Yes |
| C-05 | unapproved package acquisition | stop acquisition and reject inputs | acquisition log and provenance evidence | pre-Stage-1 repository snapshot or commit identity | package admission approval | Yes |
| C-06 | network access occurs in a prohibited path | stop execution | network trace and environment evidence | pre-Stage-1 repository snapshot or commit identity | corrected offline boundary | Yes |
| C-07 | Authority Instrument invalidity | stop execution and deny authority use | issued instrument, draft history, and review evidence | pre-Stage-1 repository snapshot or commit identity | valid bounded instrument | Yes |
| C-08 | evidence export failure | stop execution | export failure log and partial evidence | pre-Stage-1 repository snapshot or commit identity | export path correction | Yes |
| C-09 | incomplete cleanup | stop closure | cleanup audit evidence | pre-Stage-1 repository snapshot or commit identity | complete cleanup evidence | Yes |
| C-10 | residual generated artifacts | stop completion | artifact inventory and cleanup report | pre-Stage-1 repository snapshot or commit identity | clean artifact state | Yes |

## Rollback rule

The rollback target is an identified pre-Stage-1 repository snapshot or commit
identity. This package does not create that snapshot.
