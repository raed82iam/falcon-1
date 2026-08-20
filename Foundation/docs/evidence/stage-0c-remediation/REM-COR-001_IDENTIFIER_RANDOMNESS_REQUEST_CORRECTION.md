# Stage 0C Remediation Correction — Identifier Randomness Request

**Correction ID:** REM-COR-001  
**Version:** 1.0  
**Status:** Applied — Verification Repeat Required  
**Date:** 2026-07-27  
**Authority:** GOV-058  
**Preserved Failed Observations:** REM-OBS-FAIL-001; REM-OBS-FAIL-002

## Finding

The first two remediation verification runs produced `46/47` passed.

REM-IDN-V01 failed because the Identifier Provider requested 10 bytes from the active Randomness Provider while the security Profile enforces a minimum request size of 16 bytes.

## Decision

The Randomness Provider policy remains unchanged.

The Identifier Provider now requests 16 bytes and consumes only the ten bytes required by UUIDv7 after the timestamp fields.

This preserves the stronger Provider boundary and keeps identifier construction behind the Falcon Identifier Provider Contract.

## Evidence Preservation

The failed verification and trace observations are preserved under `REM-OBS-FAIL-001/002` and `REM-TRACE-FAIL-001/002`.

New verification runs shall use new evidence identities.

## Non-Authority

This correction does not activate a subject, weaken a Gate, authorize Stage 1, or expand the remediation scope.
