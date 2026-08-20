# STG-0C-SEC-001 — Active Custody, Identity, and Test-Material Separation Plan

**Identifier:** STG-0C-SEC-001  
**Version:** 1.0  
**Status:** Approved  
**Prepared:** 2026-07-27  
**Approval Date:** 2026-07-27  
**Approval Record:** GOV-055  
**Governing Authority:** GOV-054; STG-0C-PROP-001; ADR-I005; CRY-001; DESIGN-SEC-001  
**Active-Material Creation Authority:** Not Granted

## 1. Purpose

This candidate prevents Stage 0B synthetic or bootstrap material from entering active Foundation custody.

## 2. Permanent Separation

The following shall never be promoted: synthetic keys or secrets, deterministic entropy, test nonces, roots, certificates, identities, trust anchors, Runtime Epochs, bootstrap identity or time, and verification-only material.

Bootstrap observations retain `BOOTSTRAP_EXTERNAL_ID` and `BOOTSTRAP_EXTERNAL` classifications. They may be linked but never reclassified as active Falcon identity or verified Falcon time.

## 3. Future Active Custody Conditions

If separately authorized, fresh material shall be created:

- inside an approved active custody boundary;
- using active Provider Profiles and approved cryptographic domains and purposes;
- with canonical Domain Context from FCE-001;
- with independent root material where independent compromise boundaries are required;
- under non-export, least-authority, rotation, revocation, expiry, audit, and destruction rules;
- without exposure in source, ordinary configuration, environment variables, commands, logs, evidence, or unrestricted memory.

Cryptographic Providers shall reject usage outside declared purpose and protection profile.

## 4. Domain Governance

Domain and Purpose IDs shall come only from the governed catalog. IDs are immutable, never reassigned or repurposed, and deprecated only through catalog versioning.

## 5. Failure Policy

Unknown custody, provenance, integrity, identity, time, or revocation state is restrictive. There shall be no plaintext, weak, platform-default, or convenience fallback.

## 6. Current Effect

No active key, secret, certificate, identity, root, trust anchor, or custody boundary may be created.
