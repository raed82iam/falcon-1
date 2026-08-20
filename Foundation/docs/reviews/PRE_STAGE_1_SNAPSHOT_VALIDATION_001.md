# Pre-Stage-1 Snapshot Validation

## Validation results

| Check | Result | Evidence |
|---|---|---|
| CRC | PASS | ZIP opens and validates |
| test extraction | PASS | extracted to temporary directory outside the repository |
| total entry count | PASS | `596` |
| unique entry count | PASS | `596` |
| duplicate full paths | PASS | `0` |
| conflicting entries | PASS | `0` |
| absolute paths | PASS | `0` |
| path traversal | PASS | `0` |
| invalid entry names | PASS | `0` |
| unexpected zero-byte files | PASS | `0` |
| included source-file count | PASS | `596` |
| included source-byte count | PASS | `4668443` |
| uncompressed ZIP-byte count | PASS | `4668443` |
| exclusion count by rule | PASS | all excluded files were intentional sidecars or generic archive/build artifacts |
| every ZIP entry maps to exactly one source file | PASS | repository-relative path preservation confirmed |
| every included source file maps to exactly one ZIP entry | PASS | one-to-one mapping confirmed |

## Required file presence

All required governance, FIAI, proposal, and review records listed by the
corrective decision are present.

## Determination

The corrected baseline snapshot is structurally valid and restorable.

