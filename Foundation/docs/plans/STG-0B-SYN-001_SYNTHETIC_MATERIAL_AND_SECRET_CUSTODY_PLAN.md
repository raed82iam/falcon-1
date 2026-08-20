# STG-0B-SYN-001 — Synthetic Material and Secret-Custody Plan

**Identifier:** STG-0B-SYN-001  
**Version:** 1.0  
**Status:** Approved  
**Proposal Date:** 2026-07-26  
**Approval Date:** 2026-07-26  
**Governing Authority:** CON-016 through CON-019; CRY-001; DESIGN-SEC-001  
**Approval Record:** GOV-051  
**Synthetic Material Authority:** Granted for Stage 0B only  
**Real Secret Authority:** Not Granted

## 1. Purpose

This candidate governs all identities, keys, secrets, certificates, randomness inputs, time cases, and datasets used by Stage 0B.

## 2. Permitted Material Classes

Only:

- synthetic identifiers;
- synthetic clock observations;
- test-only cryptographic keys;
- test-only secrets;
- test-only certificates and identities;
- deterministic test vectors;
- synthetic entropy cases;
- synthetic evidence;
- and non-financial fixtures.

## 3. Mandatory Classification

Every material object shall declare:

- material identity;
- class;
- purpose;
- candidate scope;
- producer;
- creation method;
- custody location;
- permitted consumers;
- expiration;
- cleanup rule;
- and evidence reference.

All such material shall be classified `TEST_ONLY` and `STAGE_0B_ONLY`.

## 4. Prohibited Material

- production or reusable keys;
- personal credentials;
- GitHub tokens;
- cloud credentials;
- broker, exchange, bank, or custodian credentials;
- real certificates or identities;
- customer data;
- real market or financial data;
- and any material of unknown provenance.

## 5. Domain and Usage Enforcement

Test key material shall be domain-separated by governed purpose and case.

Providers shall reject use outside declared purpose even if the underlying algorithm permits it.

Domain and purpose identifiers shall use governed catalog values. Free-form cryptographic domains are prohibited.

No test root may become an operational root or certify another environment.

## 6. Custody

Secret values shall not appear in:

- source control;
- documentation;
- logs;
- command histories;
- evidence;
- exception text;
- or reports.

Evidence shall record classifications, identifiers, and digests where safe, never secret values.

## 7. Cleanup

Ephemeral material shall be destroyed at case closure.

Test vectors required as non-secret evidence may be preserved only when explicitly classified safe for repository storage.

Cleanup failure shall block Stage 0B closure.
