# STG-0B-ENV-001 — Windows Candidate-Verification Environment Profile

**Identifier:** STG-0B-ENV-001  
**Version:** 1.0  
**Status:** Approved  
**Proposal Date:** 2026-07-26  
**Approval Date:** 2026-07-26  
**Governing Authority:** ENV-001 v1.1; CON-020; STG-0B-BEC-001  
**Approval Record:** GOV-051  
**Candidate Environment Authority:** Granted for Stage 0B only  
**Activation Authority:** Not Granted

## 1. Purpose

This candidate defines the environment class needed to construct and verify Stage 0B candidates locally.

It does not activate a Falcon Environment Profile.

## 2. Environment Class

```text
CANDIDATE_PROVIDER_VERIFY
```

This is a temporary, non-operational, financially sterile, local Windows environment class.

## 3. Declared Platform

| Property | Candidate Value |
|---|---|
| OS family | Windows |
| Architecture | X64 |
| Repository | Local Falcon repository |
| Runtime toolchain | .NET 10 SDK candidate baseline |
| Network | Denied by default |
| Financial connectivity | Prohibited |
| Cloud connectivity | Prohibited |
| Operational status | Never operational |

Exact observed versions and executable digests shall be captured at entry.

## 4. Isolation

The environment shall be isolated from:

- production;
- personal secret stores;
- cloud accounts;
- financial accounts and services;
- operational datasets;
- active Falcon environments;
- and undeclared directories.

## 5. Portability

Windows is the first candidate-verification platform, not Falcon’s identity.

Candidate behavior shall remain portable to future Linux and Oracle Cloud environments through Falcon Contracts and Adapters.

No personal path, Windows API, registry detail, or filesystem convention may enter Falcon’s governing semantics.

## 6. Environment Evidence

The case shall record:

- environment identity;
- OS and architecture;
- tool identities and digests;
- repository baseline;
- network policy;
- filesystem boundary;
- runtime epoch;
- external identity and time classification;
- environment variables by name and classification without secret values;
- and cleanup outcome.

## 7. Lifecycle

```text
PROPOSED → AUTHORIZED → PREPARED → IN_USE → CLOSED
```

This lifecycle applies only to the candidate-verification case.

It shall not use the Falcon operational Activation lifecycle and shall not become an active Falcon environment.
