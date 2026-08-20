# STG-0C-REM-PRV-001 — Active Provider Realization Plan

**Identifier:** STG-0C-REM-PRV-001  
**Version:** 1.0  
**Status:** Approved  
**Approval Date:** 2026-07-27  
**Approval Record:** GOV-058  
**Provider Implementation:** Granted for enumerated enabling realizations only  
**Provider Activation:** Requires individual evidence-backed decisions

## 1. Purpose

Define the minimum replaceable local Provider realizations without binding Falcon to Windows, .NET APIs, a vendor, or a future cloud platform.

## 2. Provider Rules

- components consume only Falcon Provider Contracts;
- platform APIs remain behind Falcon Adapters;
- no external dependency unless separately admitted and documented;
- no Layer Boundary leakage;
- no vendor lock-in;
- unsupported capabilities and out-of-purpose use fail closed;
- candidate, active, and operational lifecycles remain distinct;
- each Provider receives an individual Profile, evidence case, and decision.

## 3. Source Providers

Randomness shall use an approved operating-system cryptographic source through an Adapter, record source capabilities without exposing material, reject caller entropy, and provide no deterministic fallback.

Time shall use the Falcon Time Provider Contract, report source, Runtime Epoch, quality, uncertainty, verification age, and capabilities. Local Windows clock alone shall not claim `VERIFIED` unless permitted by the approved Deployment Profile and evidence.

## 4. Dependent Providers

Identifier requires active Time and Randomness and issues identifiers only through the Falcon Identifier Provider Contract.

Crypto enforces governed Profile, Domain ID, Purpose ID, canonical context, key usage, nonce rules, and non-export custody.

Secret and Certificate/Identity Providers depend on active Crypto, Time, trust, revocation, and custody conditions.

## 5. Oracle Cloud Portability

Provider Contracts and Profiles shall allow later OCI implementations without changing consumers. This plan does not access or test OCI.
