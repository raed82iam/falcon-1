# FSATS V1.4 Part 0 / P0-F — Exact Current Candidate Binding

**Status:** `EXACT_CANDIDATE_BOUND_FOR_FINAL_REVIEW`
**P0-F final Owner acceptance:** `NOT_GRANTED`

## 1. Exact semantic candidate

The exact P0-F semantic set submitted to final Architecture/Consistency and Red-Team review is:

1. `89_P0F_CANONICAL_CROSS_APPLICATION_CONTRACT_AND_INFORMATION_FLOW_CANDIDATE.md`
   - Blob: `643ac78f8f8ba96eed78158e0c40a648e20e6d2d`
2. `89A_P0F_EXACT_SHARED_APPLICATION_AND_COVERAGE_HARDENING.md`
   - Blob: `8bcaa41f39eaa41bd56108195e9d2f4a0bb0bfe5`
3. `89B_P0F_BILATERAL_DECLARATION_AUTHORITY_SECURITY_AND_VERSION_HARDENING.md`
   - Blob: `66a5d89ce0ef46ef6e4b768f23ce785d47d81788`
4. `89C_P0F_END_TO_END_INFORMATION_FLOW_AND_SECURITY_BINDING_HARDENING.md`
   - Blob: `36baea2d8253fa955962a8b01aa44236f7a56ecd`

These records SHALL be read together as one candidate.

## 2. Governing predecessor state

P0-A through P0-E are `OWNER_ACCEPTED_AND_CLOSED`.

The candidate must preserve:

- `falcon.container.trading` as non-owning architecture grouping;
- Guardian, FSAPMA, Trading and FSTSimA as four independent Applications inside that container;
- Shared Web and Shared Communication as independent Shared Applications outside the container;
- one MSA per Application and accepted P0-C/P0-E awareness/ownership boundaries;
- P0-D Foundation anti-reimplementation boundary;
- P0-E canonical identities, owner roles, Manifest/lifecycle and unresolved-field rules.

## 3. Current Foundation snapshot interpretation

Final P0-F review uses the fresh Foundation documentary state in which Stage 5 WP-03 through WP-07 are accepted and closed, WP-08 is authorized/in progress, and runtime activation/external connectivity/Application-specific Foundation behavior remain unauthorized.

Open FCR state remains independently controlling where applicable.

## 4. Candidate-change rule

Any semantic modification to 89/89A/89B/89C after the blob identities above invalidates the final review result and requires a fresh binding and fresh review.

`EXACT_BYTES_BOUND = TRUE`

`FINAL_REVIEW_MAY_PROCEED = TRUE`

`OWNER_ACCEPTANCE = NOT_GRANTED`
