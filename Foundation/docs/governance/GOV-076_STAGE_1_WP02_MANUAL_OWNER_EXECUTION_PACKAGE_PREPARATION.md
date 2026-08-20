# GOV-076 — Stage 1 WP-02 Manual Owner Execution Package Preparation

Status: PROPOSED
Authority: Package-preparation only
Stage 1 state: STARTED_IN_PROGRESS
Stage 1 execution authority: GRANTED_ACTIVE
WP-03 authority: PROHIBITED

## Decision

This record prepares a manual Owner-executed WP-02 replay package because the governed Codex-run replay path is not enforceable in the current environment and the identified bypass cause remains `DIRECT_EDITING_TOOL_BYPASSED_RUNNER`.

The package-preparation authority granted here is limited to documentary preparation of a manual execution package and related qualification evidence. It does not authorize live WP-02 execution, does not change Stage 1 state, and does not grant any implementation, deployment, or WP-03 authority.

## Mandatory constraints

- Manual Owner execution remains the next approved path.
- The live Falcon repository must not be modified by any replay package validation or qualification activity in this task.
- The manual package must remain outside `C:\Falcon\Falcon1`.
- The package must be validated on external scratch only.
- The canonical WP-02 title used in future records must be `WP-02 â€” Establish project ownership and dependency direction`.
- The mojibake form `WP-02 Ã¢â‚¬â€ Establish project ownership and dependency direction` is preserved only as a historical defect reference and must not be used as the canonical title.

## Qualification expectations

- Include the accepted WP-01 solution copy byte-for-byte.
- Include the currently accepted WP-02 expected artifacts byte-for-byte.
- Include the qualified evidence runner byte-for-byte.
- Include a deterministic replay script, a validator, immutable manifest files, and Owner instructions.
- Fail closed on any package, digest, identity, or path mismatch.
- Preserve the current Stage 1 state as `STARTED_IN_PROGRESS`.

## Next governed action

Owner manual execution only, using the external package prepared under this record.
