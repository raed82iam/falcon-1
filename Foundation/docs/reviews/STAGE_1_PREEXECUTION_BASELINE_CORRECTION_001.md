# Stage 1 Pre-Execution Baseline Correction

## 1. Rejected baseline identity

**Path:** `C:\Users\raeda\OneDrive\Desktop\Falcon\Falcon_pre_stage1_execution_baseline.zip`  
**Verified SHA-256:** `6089D2E66B16FC4E74AC2F6827144CF665D67AA88999FD7C585A08BEB7E91BF8`  
**File size:** `1309221`  
**Creation instant:** `2026-07-30 16:49:24 +03:00`

## 2. Structural defects in the rejected snapshot

- ZIP entries = `586`
- directory entries = `0`
- unique entry names = `533`
- additional duplicate entries = `53`
- duplicated basenames = `32`
- duplicated basenames with conflicting contents = `31`
- repository-relative paths preserved = `NO`

The rejected ZIP was physically readable, but it was not a valid restorable
repository snapshot because entries were stored by basename instead of by
repository-relative path.

## 3. Missing pre-baseline records in the rejected snapshot

The rejected snapshot did not include:

1. `docs/governance/GOV-064_STAGE_1_CONDITIONAL_AUTHORITY_AND_PRE_EXECUTION_VALIDATION.md`
2. `docs/governance/FIAI-STAGE1-001_AUTHORITY_INSTRUMENT.md`
3. `docs/governance/FIAI-STAGE1-001_ISSUANCE_RECORD.md`
4. `docs/governance/FIAI-STAGE1-001_ACCEPTANCE_RECORD.md`
5. `docs/governance/FIAI-STAGE1-001_EXECUTION_SCOPE_AUTHORIZATION.md`

## 4. Status

`REJECTED_PRE_STAGE_1_BASELINE_INVALID_STRUCTURE`

The rejected snapshot must not be used as the effective
`PRE_STAGE_1_BASELINE_ID`.

## 5. Supersession relationship

The rejected baseline is superseded by the corrected snapshot:

`C:\Users\raeda\OneDrive\Desktop\Falcon\Falcon_pre_stage1_execution_baseline_v2_4.zip`

## 6. Corrected-snapshot summary

The corrected snapshot preserves repository-relative paths, keeps same-basename
files separate by directory, and includes the required pre-baseline records.
