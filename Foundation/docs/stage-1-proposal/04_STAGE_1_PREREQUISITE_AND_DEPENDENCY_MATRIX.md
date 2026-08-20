# 04 - Stage 1 Prerequisite and Dependency Matrix

## Prerequisite matrix

| Prerequisite ID | Requirement | Canonical source | Dependency | Evidence required | Severity | Owner decision required | Status |
|---|---|---|---|---|---|---|---|
| P-01 | Stage 0 is complete and closed | `docs/reviews/STAGE_SEQUENCE_STATE_RECONCILIATION_001.md` | None | closed current-state reconciliation | High | No | SATISFIED |
| P-02 | Project Stage 1 proposal authority exists and is exhausted | Owner proposal-authority decision record | P-01 | proposal authority decision record | High | No | SATISFIED |
| P-03 | A bounded Foundation Implementation Authority Instrument draft exists and conforms to CON-012, IMP-001, Constitution, and Document Authority | `docs/stage-1-proposal/14_STAGE_1_FOUNDATION_IMPLEMENTATION_AUTHORITY_INSTRUMENT_DRAFT.md` | P-02 | complete instrument draft | High | No | SATISFIED |
| P-04 | Issuance and acceptance of the Foundation Implementation Authority Instrument remain pending Owner decision | Owner decision record | P-03 | issuance and acceptance fields | High | Yes | OWNER_DECISION_REQUIRED |
| P-05 | Explicit Owner Stage 1 scope authorization remains pending Owner decision | Owner execution-authority decision record | P-04 | exact Stage 1 scope authorization record | High | Yes | OWNER_DECISION_REQUIRED |
| P-06 | Immediate pre-execution Manifest revalidation is required before any execution request | Activated manifest set and execution-time review rule | P-05 | manifest revalidation report and exact manifest identities | High | No | SATISFIED |
| P-07 | Active enabling Provider Profiles exist for the scoped baseline | effective Activation Manifests for ACT-FCE-001 through ACT-GATE-001 | P-01 | manifest identities, activation status, and digests | High | No | SATISFIED |
| P-08 | Active Foundation build environment exists | `docs/governance/GOV-059_STAGE_0C_COMPLETION_AND_CLOSURE.md`, `docs/environments/ENV-001_FOUNDATION_BUILD_AND_VERIFICATION_ENVIRONMENT_PROFILE.md` | P-07 | environment identity and digest | High | No | SATISFIED |
| P-09 | Active Build Baseline exists | `docs/catalogs/BLD-001_FOUNDATION_TOOLCHAIN_AND_BUILD_BASELINE_CATALOG.md` | P-07 | baseline identity and digest | High | No | SATISFIED |
| P-10 | Active Pipeline Definition exists | `docs/specifications/foundation/PIPE-001_FOUNDATION_PIPELINE_SPECIFICATION.md` | P-09 | pipeline identity and digest | High | No | SATISFIED |
| P-11 | Active Gate Profile exists | `docs/specifications/foundation/PIPE-001_FOUNDATION_PIPELINE_SPECIFICATION.md` | P-10 | gate identity and digest | High | No | SATISFIED |
| P-12 | Machine-readable TRC expansion exists | `docs/traceability/TRC-001_FOUNDATION_REQUIREMENT_TO_VERIFICATION_TRACEABILITY_MATRIX.md` | P-10 | trace identity and digest | High | No | SATISFIED |
| P-13 | Exact contract and schema baseline exists | `docs/governance/GOV-059_STAGE_0C_COMPLETION_AND_CLOSURE.md`, activated contract baseline | P-02 | canonical contract inventory and versions | High | No | SATISFIED |
| P-14 | Isolated non-financial build environment exists | `docs/environments/ENV-001_FOUNDATION_BUILD_AND_VERIFICATION_ENVIRONMENT_PROFILE.md` | P-08 | environment isolation and no-finance evidence | High | No | SATISFIED |
| P-15 | No unresolved constitutional conflict exists | `docs/02_FALCON_CONSTITUTION.md`, `docs/03_DOCUMENT_AUTHORITY.md` | P-01 | constitutional review evidence | High | No | SATISFIED |
| P-16 | No release-blocking challenge exists | `docs/releases/FRS-001_FOUNDATION_RELEASE.md`, `docs/traceability/TRC-001_FOUNDATION_REQUIREMENT_TO_VERIFICATION_TRACEABILITY_MATRIX.md` | P-15 | challenge-status evidence | Medium | No | SATISFIED |
| P-17 | No release-blocking security issue exists | `docs/adrs/ADR-I007_FOUNDATION_BUILD_VERIFICATION_AND_PROMOTION_PIPELINE.md`, `docs/adrs/ADR-F001_FOUNDATION_EXECUTION_AND_ISOLATION_MODEL.md` | P-14 | security review evidence | High | No | SATISFIED |
| P-18 | Exact implementation baseline exists | `docs/plans/IMP-001_FOUNDATION_IMPLEMENTATION_WORK_PLAN.md` | P-09, P-10, P-11, P-12 | exact implementation baseline identity | High | No | SATISFIED |
| P-19 | Bounded Foundation Implementation Authority Instrument issuance and acceptance are required before execution | Owner decision package | P-03 | Owner issuance and acceptance record | High | Yes | OWNER_DECISION_REQUIRED |

## Manifest validity matrix

All consumed enabling subjects are time-bounded and shall be revalidated
immediately before any execution request.

| Manifest ID | File path | Current status | Activated scope | Effective date | Expiry / review boundary | Digest | Dependent manifests | Revalidation required at execution |
|---|---|---|---|---|---|---|---|---|
| ACT-FCE-001 | `docs/evidence/stage-0c/manifests/AM-FCE-001_CANONICAL_ENCODING_ACTIVATION_MANIFEST.md` | ACTIVE | evidence encoding and validation | 2026-07-27 | 2026-08-10 | `9E6E980FDF25CDB9B1462B3A85DD82DF6199D25433C536657E062E8E442A501D` | `RVES-STG-0C-001`, `CM-FCE-001` | Yes |
| ACT-TRUST-001 | `docs/evidence/stage-0c/manifests/AM-TRUST-001_TRUST_OBJECT_PRIMITIVES_ACTIVATION_MANIFEST.md` | ACTIVE | Trust Object construction and validation | 2026-07-27 | 2026-08-10 | `B87C8246A525F7555988A3EEA60AF3F724A7216FE2F5D505DA8F1499E9F266E0` | `RVES-STG-0C-001`, `CM-TRUST-001` | Yes |
| ACT-RND-001 | `docs/evidence/stage-0c-closure/manifests/AM-REM-RND-001-E_RANDOMNESS_EFFECTIVE.md` | ACTIVE | randomness provider profile | 2026-07-27 | 2026-08-10 | `50C14FA56A7D6411AA0CA99E6F4442E43EAAA70036C987922537DA3213374999` | `RVES-STG-0C-REM-001` | Yes |
| ACT-TIM-001 | `docs/evidence/stage-0c-closure/manifests/AM-REM-TIM-001-E_TIME_EFFECTIVE.md` | ACTIVE | time provider profile | 2026-07-27 | 2026-08-10 | `EA452B6912215D7B216FDB2949354F2A8C700A77395C0BEB1A0F081E68B8016A` | `RVES-STG-0C-REM-001` | Yes |
| ACT-IDN-001 | `docs/evidence/stage-0c-closure/manifests/AM-REM-IDN-001-E_IDENTIFIER_EFFECTIVE.md` | ACTIVE | identifier provider profile | 2026-07-27 | 2026-08-10 | `766846443876D09AEC542942F154C1123B86D3ED0988C3C7BFB5534365ADA42E` | `RVES-STG-0C-REM-001` | Yes |
| ACT-CRY-001 | `docs/evidence/stage-0c-closure/manifests/AM-REM-CRY-001-E_CRYPTO_EFFECTIVE.md` | ACTIVE | cryptographic provider adapter profile | 2026-07-27 | 2026-08-10 | `5E0732ED7DA9647EB97836E16B1969B41BAC96DE9AE09672CD480EF435A7E6D4` | `RVES-STG-0C-REM-001` | Yes |
| ACT-SEC-001 | `docs/evidence/stage-0c-closure/manifests/AM-REM-SEC-001-E_SECRET_EFFECTIVE.md` | ACTIVE | secret provider profile | 2026-07-27 | 2026-08-10 | `F0137C694556CC6A5D72DCA2898E1A9AB5F3581CFFC81DA75A154EC0210D5C92` | `RVES-STG-0C-REM-001` | Yes |
| ACT-CID-001 | `docs/evidence/stage-0c-closure/manifests/AM-REM-CID-001-E_CERTIFICATE_IDENTITY_EFFECTIVE.md` | ACTIVE | certificate and identity provider profile | 2026-07-27 | 2026-08-10 | `FFC5C5968989F98E21A9CAB381879DEEAAEADDEDDF61EAB6C9B1323F117EB69B` | `RVES-STG-0C-REM-001` | Yes |
| ACT-ENV-001 | `docs/evidence/stage-0c-closure/manifests/AM-REM-ENV-001-E_ENVIRONMENT_EFFECTIVE.md` | ACTIVE | local Windows foundation environment | 2026-07-27 | 2026-08-10 | `9B130549A372547238F4D3E283F7C80C1C89E8F1E26FA21E9A9108274026F391` | `RVES-STG-0C-REM-001` | Yes |
| ACT-BLD-001 | `docs/evidence/stage-0c-closure/manifests/AM-REM-BLD-001-E_BUILD_BASELINE_EFFECTIVE.md` | ACTIVE | build baseline | 2026-07-27 | 2026-08-10 | `14EC422C02948C8B0EC52B3D16B5F4FB3429AA8B115F13822DA822EA9EF5B80E` | `RVES-STG-0C-REM-001` | Yes |
| ACT-TRC-001 | `docs/evidence/stage-0c-closure/manifests/AM-REM-TRC-001-E_TRACE_EFFECTIVE.md` | ACTIVE | trace expansion | 2026-07-27 | 2026-08-10 | `3FDB8B4279FCCC2C0C20CE4189ED4843ADE947C58516D2F2FC88B5321AA55341` | `RVES-STG-0C-REM-001` | Yes |
| ACT-PIPE-001 | `docs/evidence/stage-0c-closure/manifests/AM-REM-PIPE-001-E_PIPELINE_EFFECTIVE.md` | ACTIVE | pipeline definition | 2026-07-27 | 2026-08-10 | `DAFBA401781EDFC418AC27E7AB9E9A8C18328AC6830D77EBA8FFB4236B102046` | `RVES-STG-0C-REM-001` | Yes |
| ACT-GATE-001 | `docs/evidence/stage-0c-closure/manifests/AM-REM-GATE-001-E_GATE_EFFECTIVE.md` | ACTIVE | gate profile | 2026-07-27 | 2026-08-10 | `D07827A0672E82E5FE3B1C6E2BFE372B80E65854B815B72CF5C61969AC6D7F0D` | `RVES-STG-0C-REM-001` | Yes |

## Dependency notes

- No prerequisite is satisfied by Stage 1 execution evidence.
- All evidence paths are documentary and canonical.
- Execution may begin only while every required manifest remains active and
  unchanged.
- Any expired, revoked, changed, or invalidated manifest blocks Stage 1.
- Owner authorization does not override an expired or invalid manifest.
- P-19 is the remaining execution-authority decision point.
