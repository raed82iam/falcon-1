# STAGE_1_REPOSITORY_RELOCATION_OLD_PATH_REFERENCE_INVENTORY_001

## Purpose

This inventory lists old-root references found in the relocated repository and
classifies whether each reference is historical evidence or a current-state
reference that should be updated to the new root.

## Inventory

| File | Line | Referenced path | Reference type | Required treatment |
|---|---:|---|---|---|
| `docs\reviews\STAGE_1_NUGET_HOST_PERMISSION_DIAGNOSIS_001.md` | 67 | `C:\Users\raeda\OneDrive\Desktop\Falcon\Falcon1\NuGet.Config` | `CURRENT_STATE_MUST_UPDATE` | update to `C:\Falcon\Falcon1\NuGet.Config` |
| `docs\reviews\STAGE_1_NUGET_HOST_PERMISSION_DIAGNOSIS_001.md` | 118 | `C:\Users\raeda\OneDrive\Desktop\Falcon\Falcon1\NuGet.Config` | `CURRENT_STATE_MUST_UPDATE` | update to `C:\Falcon\Falcon1\NuGet.Config` |
| `docs\reviews\STAGE_1_NUGET_HOST_PERMISSION_DIAGNOSIS_001.md` | 119 | `C:\Users\raeda\OneDrive\Desktop\Falcon\Falcon1\NuGet.Config` | `CURRENT_STATE_MUST_UPDATE` | update to `C:\Falcon\Falcon1\NuGet.Config` |
| `docs\stage-1-proposal\02_STAGE_1_FOUNDATION_COMPONENT_AND_PATH_BOUNDARY.md` | 7 | `C:\Users\raeda\OneDrive\Desktop\Falcon\Falcon1\` | `CURRENT_STATE_MUST_UPDATE` | update to `C:\Falcon\Falcon1\` |
| `docs\plans\STG-0B-BEC-001_CANDIDATE_BOOTSTRAP_EXECUTION_CONTEXT.md` | 21 | `C:\Users\raeda\OneDrive\Desktop\Falcon\Falcon1` | `HISTORICAL_EVIDENCE_PRESERVE` | preserve as historical context |
| `docs\plans\STG-0A-BEC-001_BOOTSTRAP_EXECUTION_CONTEXT.md` | 25 | `C:\Users\raeda\OneDrive\Desktop\Falcon\Falcon1` | `HISTORICAL_EVIDENCE_PRESERVE` | preserve as historical context |
| `docs\evidence\stage-0a\STG-0A-EVD-001-01_AUTHORITY_AND_CONTEXT.md` | 15 | `C:\Users\raeda\OneDrive\Desktop\Falcon\Falcon1` | `HISTORICAL_EVIDENCE_PRESERVE` | preserve as historical context |
| `docs\evidence\stage-0a\STG-0A-EVD-001-03_REPOSITORY_STATUS_BEFORE.md` | 9 | `C:/Users/raeda/OneDrive/Desktop/Falcon/Falcon1` | `HISTORICAL_EVIDENCE_PRESERVE` | preserve as historical context |
| `docs\reviews\PRE_STAGE_1_BASELINE_ID.md` | 6 | `C:\Users\raeda\OneDrive\Desktop\Falcon\Falcon_pre_stage1_execution_baseline_v3.zip` | `HISTORICAL_EVIDENCE_PRESERVE` | preserve historical snapshot identity |
| `docs\reviews\PRE_STAGE_1_BASELINE_ID.md` | 8 | `C:\Users\raeda\OneDrive\Desktop\Falcon\Falcon1\` | `HISTORICAL_EVIDENCE_PRESERVE` | preserve historical repository root cut |
| `docs\reviews\PRE_STAGE_1_BASELINE_ID.md` | 48 | `C:\Users\raeda\OneDrive\Desktop\Falcon\Falcon_pre_stage1_execution_baseline.zip` | `HISTORICAL_EVIDENCE_PRESERVE` | preserve rejected baseline identity |
| `docs\reviews\PRE_STAGE_1_SNAPSHOT_CONTENT_INVENTORY_001.md` | 6 | `C:\Users\raeda\OneDrive\Desktop\Falcon\Falcon_pre_stage1_execution_baseline_v3.zip` | `HISTORICAL_EVIDENCE_PRESERVE` | preserve snapshot identity |
| `docs\reviews\PRE_STAGE_1_SNAPSHOT_CONTENT_INVENTORY_001.md` | 7 | `C:\Users\raeda\OneDrive\Desktop\Falcon\Falcon1\` | `HISTORICAL_EVIDENCE_PRESERVE` | preserve historical repository root cut |
| `docs\reviews\STAGE_1_PREEXECUTION_BASELINE_CORRECTION_001.md` | 5 | `C:\Users\raeda\OneDrive\Desktop\Falcon\Falcon_pre_stage1_execution_baseline.zip` | `HISTORICAL_EVIDENCE_PRESERVE` | preserve rejected baseline identity |
| `docs\reviews\STAGE_1_PREEXECUTION_BASELINE_CORRECTION_001.md` | 45 | `C:\Users\raeda\OneDrive\Desktop\Falcon\Falcon_pre_stage1_execution_baseline_v2_4.zip` | `HISTORICAL_EVIDENCE_PRESERVE` | preserve prior rejected identity |

## Summary

- current-state references that require update: `4`
- historical references preserved: `11`
- unsupported or ambiguous references: `0`

