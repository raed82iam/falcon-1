# Pre-Stage-1 Post-Relocation Snapshot Validation

## Validation scope

This report validates the post-relocation baseline archive created from C:\falcon\Falcon1.

## Archive validation results

| Metric | Result |
|---|---:|
| CRC | PASS |
| Test extraction | PASS |
| Total entries | 665 |
| Unique entries | 665 |
| Duplicate full paths | 0 |
| Conflicting entries | 0 |
| Absolute paths | 0 |
| Path traversal entries | 0 |
| Invalid entry names | 0 |
| Included source-file count | 665 |
| Excluded source-file count | 4 |
| Included source-byte count | 5352884 |
| Uncompressed ZIP entry-byte count | 5352884 |
| final ZIP file-byte count | 5497398 |
| Unexpected zero-byte files | 0 |
| Repository-relative path integrity | PASS |
| Baseline digest verification | PASS |
| Self-referential baseline records inside ZIP | 0 |

## Exact 13 Activation Manifests

- docs/evidence/stage-0c/manifests/AM-FCE-001_CANONICAL_ENCODING_ACTIVATION_MANIFEST.md
- docs/evidence/stage-0c/manifests/AM-TRUST-001_TRUST_OBJECT_PRIMITIVES_ACTIVATION_MANIFEST.md
- docs/evidence/stage-0c-closure/manifests/AM-REM-BLD-001-E_BUILD_BASELINE_EFFECTIVE.md
- docs/evidence/stage-0c-closure/manifests/AM-REM-CID-001-E_CERTIFICATE_IDENTITY_EFFECTIVE.md
- docs/evidence/stage-0c-closure/manifests/AM-REM-CRY-001-E_CRYPTO_EFFECTIVE.md
- docs/evidence/stage-0c-closure/manifests/AM-REM-ENV-001-E_ENVIRONMENT_EFFECTIVE.md
- docs/evidence/stage-0c-closure/manifests/AM-REM-GATE-001-E_GATE_EFFECTIVE.md
- docs/evidence/stage-0c-closure/manifests/AM-REM-IDN-001-E_IDENTIFIER_EFFECTIVE.md
- docs/evidence/stage-0c-closure/manifests/AM-REM-PIPE-001-E_PIPELINE_EFFECTIVE.md
- docs/evidence/stage-0c-closure/manifests/AM-REM-RND-001-E_RANDOMNESS_EFFECTIVE.md
- docs/evidence/stage-0c-closure/manifests/AM-REM-SEC-001-E_SECRET_EFFECTIVE.md
- docs/evidence/stage-0c-closure/manifests/AM-REM-TIM-001-E_TIME_EFFECTIVE.md
- docs/evidence/stage-0c-closure/manifests/AM-REM-TRC-001-E_TRACE_EFFECTIVE.md

## Git state evidence

- Git repository root: C:/falcon/Falcon1
- Git inside work tree: true
- Git HEAD: 095d800e86823b248468ff9f4fa12e6e44647a35
- Git tree: 69db1d39ddb5c3dd74c0e9764d85d909f476975f
- Git branch: main
- Git working tree: DIRTY
- Git remote: origin https://github.com/raed82iam/Falcon.git (fetch) / origin https://github.com/raed82iam/Falcon.git (push)
- Git connectivity: PASS

## Authority-state preservation

- FIAI issuance = ISSUED
- FIAI acceptance = ACCEPTED
- scope authorization = CONDITIONALLY_GRANTED_NOT_EFFECTIVE
- FIAI lifecycle = SUSPENDED
- Stage 1 execution authority = NOT_EFFECTIVE
- Stage 1 execution started = NO
- Stage 1 implementation performed = NO

## Validation summary

- new repository-root mismatch = 0
- self-referential baseline records inside ZIP = 0
- duplicate ZIP paths = 0
- conflicting ZIP entries = 0
- exact Activation Manifests present = 13/13
- CRC failures = 0
- test-extraction failures = 0
- baseline digest verification = PASS
- historical v3 evidence modified = 0
- Git mutations = 0
- Stage 1 implementation actions = 0
- restore/build/test commands = 0
- FIAI lifecycle transitions = 0
- invalid UTF-8 = 0
- mojibake = 0
- replacement characters = 0



