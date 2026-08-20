# STAGE_1_REPOSITORY_RELOCATION_COMPARISON_REPORT_001

## Comparison scope

This report compares the previous OneDrive-controlled repository root against
the new `C:\falcon\Falcon1` repository root using read-only evidence only.

## Root status

| Root | Status | Evidence |
|---|---|---|
| Previous root `C:\Users\raeda\OneDrive\Desktop\Falcon\Falcon1` | `OLD_ROOT_ABSENT_AFTER_MOVE` | `Test-Path` returned `False` |
| New root `C:\falcon\Falcon1` | present and readable | directory exists; `.git` exists; required directories exist |

## New-root validation

- repository-relative structure is preserved;
- no files were flattened;
- duplicate repository-relative paths were not observed in the read-only checks;
- no path-traversal objects were observed in the root scan;
- no reparse-point files were observed in the root scan;
- no cloud-only placeholder files were observed in the root scan;
- no inaccessible files were observed in the root scan;
- Git metadata exists at `C:\falcon\Falcon1\.git`.

## Tree metrics

- total file count: `665`
- total directory count: `194`
- total byte count: `8919205`
- zero-byte file count: `0`
- reparse-point count: `0`
- cloud-only placeholder count: `0`
- inaccessible file count: `0`

## Critical records present

- `docs/governance/GOV-064_STAGE_1_CONDITIONAL_AUTHORITY_AND_PRE_EXECUTION_VALIDATION.md`
- `docs/governance/GOV-065_STAGE_1_BASELINE_IDENTITY_CORRECTION_AND_FIAI_LIFECYCLE_NORMALIZATION.md`
- `docs/governance/FIAI-STAGE1-001_AUTHORITY_INSTRUMENT.md`
- `docs/governance/FIAI-STAGE1-001_ISSUANCE_RECORD.md`
- `docs/governance/FIAI-STAGE1-001_ACCEPTANCE_RECORD.md`
- `docs/governance/FIAI-STAGE1-001_EXECUTION_SCOPE_AUTHORIZATION.md`
- `docs/governance/FIAI-STAGE1-001_LIFECYCLE_CORRECTION_RECORD.md`
- `docs/reviews/PRE_STAGE_1_BASELINE_ID.md`
- `docs/reviews/STAGE_1_CONDITIONAL_AUTHORITY_EFFECTIVENESS_VALIDATION.md`
- `docs/reviews/STAGE_1_NUGET_HOST_PERMISSION_DIAGNOSIS_001.md`
- `docs/stage-1-proposal/README.md`
- all 13 exact Activation Manifests

## Exact Activation Manifests present

`13/13`

## Comparison conclusion

Because the old root is absent after the owner-performed move, a live tree-to-tree
byte comparison cannot be completed from the current workspace state.

No unexplained content mismatch was discovered in the new root.

## Relocation result

`RELOCATION_VALIDATED_NEW_ROOT_ACCEPTED`

