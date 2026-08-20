# STG-0C-SEQ-001 — Activation Dependency and Decision Sequence

**Identifier:** STG-0C-SEQ-001  
**Version:** 1.0  
**Status:** Approved  
**Prepared:** 2026-07-27  
**Approval Date:** 2026-07-27  
**Approval Record:** GOV-055  
**Governing Authority:** GOV-054; STG-0C-PROP-001  
**Stage 0C Authority:** Governed by STG-0C-AUTH-001

## 1. Purpose

This candidate freezes the only permitted Stage 0C sequence and prevents circular Activation.

## 2. Sequence

| Order | Decision Group | Entry Condition | Result |
|---|---|---|---|
| A | Authority and evidence reconstruction | Approved Stage 0C package | Reconstructed baseline only |
| B1 | Canonical Encoding realization | A complete | Individual decision |
| B2 | Trust Object primitives | B1 applicable and valid | Individual decision |
| B3 | Machine-readable trace source expansion | B1–B2 applicable and valid | Individual decision |
| C1 | Randomness Provider Profile | Required foundations valid | Individual decision |
| C2 | Time Provider Profile | C1 dependency disposition explicit | Individual decision |
| D1 | Identifier Provider Profile | Active Time and Randomness dependencies | Individual decision |
| D2 | Cryptographic Provider Adapter Profile | Active Randomness and approved custody | Individual decision |
| D3 | Secret Provider Profile | Active cryptographic custody | Individual decision |
| D4 | Certificate and Identity Provider Profile | Active Crypto, Secret, Time, trust, and revocation dependencies | Individual decision |
| E | Windows Foundation build-verification environment | Required Provider dispositions complete | Individual decision |
| F1 | Build Baseline | Exact environment active for scope | Individual decision |
| F2 | Trace expansion | Trace prerequisites active | Individual decision |
| F3 | Pipeline Definition | Build and trace prerequisites active | Individual decision |
| F4 | Gate Profile and requirement generation | Pipeline prerequisites active | Individual decision |
| G | Foundation Implementation Readiness case | Every subject accounted for | Recommendation only |

## 3. Rules

- No step may borrow authority from a later step.
- A dependency’s Activation never activates its consumer.
- A failed, restricted, expired, revoked, uncertain, or unaccepted prerequisite blocks dependent evaluation.
- Parallel evaluation is permitted only where no authority, custody, evidence, or semantic dependency exists.
- Reordering requires a new approved package; convenience does not justify deviation.
- Group Activation and implied Activation are prohibited.

## 4. Failure

Any broken prerequisite stops the affected branch. Unaffected documentary assessment may continue only where the governing authority expressly permits it.

## 5. Current Effect

None. No sequence step may be executed before separate approval.
