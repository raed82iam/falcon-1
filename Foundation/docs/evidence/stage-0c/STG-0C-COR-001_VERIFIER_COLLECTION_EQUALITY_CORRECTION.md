# Stage 0C Verifier Correction

**Correction ID:** STG-0C-COR-001  
**Version:** 1.0  
**Status:** Applied — Verification Repeat Required  
**Date:** 2026-07-27  
**Authority:** GOV-055; GOV-056  
**Supersedes Conclusion From:** STG-0C-OBS-FAIL-001; STG-0C-OBS-FAIL-002

## 1. Finding

The first two Stage 0C verifier runs reported `33/34` passed.

The failed case was VPL-BST-006-V10. The serialized Activation case reconstructed correctly, but the verifier used default record equality for collection interface properties. The original object held arrays and the reconstructed object held lists. Default equality compared collection instances and concrete representations rather than their ordered contents.

## 2. Correction

The reconstruction check now compares:

- scalar identity and digest fields directly;
- dependency collections by ordered ordinal content;
- non-authorities by ordered ordinal content;
- and lifecycle state directly.

The governed meaning of the Activation case was not changed.

## 3. Evidence Preservation

The two original failed results are preserved as:

- `STG-0C-OBS-FAIL-001_ACTIVATION_VERIFICATION.json`;
- `STG-0C-OBS-FAIL-002_ACTIVATION_VERIFICATION_REPEAT.json`.

They shall not be represented as passing evidence.

New runs shall receive new evidence identities.

## 4. Scope

This correction changes verifier comparison logic only. It does not activate a subject, alter a Candidate, weaken a Gate, expand authority, or authorize Stage 1.
