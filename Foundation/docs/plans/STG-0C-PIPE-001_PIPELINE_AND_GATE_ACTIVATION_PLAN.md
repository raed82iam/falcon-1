# STG-0C-PIPE-001 — Pipeline and Gate Activation Plan

**Identifier:** STG-0C-PIPE-001  
**Version:** 1.0  
**Status:** Approved  
**Prepared:** 2026-07-27  
**Approval Date:** 2026-07-27  
**Approval Record:** GOV-055  
**Governing Authority:** GOV-054; STG-0C-PROP-001; PIPE-001; VPL-BST-007  
**Pipeline and Gate Activation:** Not Granted

## 1. Purpose

This candidate defines evaluation of one exact Pipeline Definition and one exact Gate Profile for Foundation verification.

## 2. Pipeline Case

Before execution, a future approved run shall bind:

- approved Build Intent;
- exact Pipeline Definition and Gate Profile;
- immutable Evidence Requirement Set snapshot;
- Build Baseline, environment, source, policy, and configuration identities;
- Verification Session and Root Evidence Set identities;
- authority and jurisdiction;
- and failure, stop, cleanup, and preservation rules.

## 3. Evidence and Promotion Rules

- Promotion decisions shall reference exactly one Root Verification Evidence Set.
- Individual sessions shall not serve directly as promotion evidence.
- Only the completeness states permitted by the governing Build Intent may pass; Stage 0C Activation requires `COMPLETE`.
- Missing, invalid, stale, conflicted, or uncertain material shall not be treated as success.
- Evidence corrections create superseding records, never historical edits.
- No producer, transformer, aggregator, signer, or evaluator may be the sole authority declaring completeness and promotion readiness.
- Gate rules cannot be weakened by the subject they govern.

## 4. Ordering and Repeatability

Message arrival order shall not be assumed unless the governing Contract explicitly guarantees it. Derived deterministic evaluations shall be reproducible from preserved evidence, rules, and Evaluation Context.

## 5. Current Effect

No Pipeline, Gate, runner, promotion, or execution is active.
