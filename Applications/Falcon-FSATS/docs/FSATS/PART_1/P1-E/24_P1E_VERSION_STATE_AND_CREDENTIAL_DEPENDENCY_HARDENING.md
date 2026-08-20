# P1-E — Version, State Compatibility and Credential-Dependency Hardening

**Status:** `RED_TEAM_REMEDIATION / CONTROLS_PREVIOUS_TARGET_PROSPECTIVELY`  
**Implementation Authority:** `NOT_GRANTED`  
**Runtime Authority:** `NOT_GRANTED`

## Reason

Fresh adversarial review identified two omissions that could permit an apparently complete Manifest to remain unsafe or ambiguous:

1. package/Application version declarations were not sufficiently bound to persisted-state schema, migration, rollback/corrective-action and recovery compatibility;
2. capability-specific external credential-reference dependencies were not explicitly required in the Manifest despite the resolved FCR-0081 onboarding/credential planning boundary.

## Version and State Compatibility Rule

Each Application Manifest SHALL distinguish and bind, where applicable:

- immutable Application identity;
- Application/package version;
- public contract/schema version dependencies;
- persisted-state schema/version;
- configuration schema/version;
- model/intelligent-state version where governed;
- migration compatibility;
- rollback/corrective-action target compatibility;
- recovery/revival compatibility;
- minimum/maximum compatible dependency versions;
- provenance/integrity evidence for the exact package/state combination.

A package version bump does not by itself make old state compatible.

```text
PACKAGE_COMPATIBLE != STATE_COMPATIBLE
STATE_MIGRATED != TRUST_RESTORED
ROLLBACK_TARGET_EXISTS != ROLLBACK_TARGET_ELIGIBLE_NOW
```

A rollback/recovery target SHALL be currently valid, non-revoked, dependency/security compatible, and verified for the state/configuration schema being restored.

If compatibility cannot be established, affected activation/recovery SHALL fail closed rather than silently discard, coerce or reinterpret state.

## Credential-Reference Dependency Rule

Where an Application capability requires external credential material, its Manifest SHALL declare the semantic credential-reference dependency without embedding secret bytes.

At minimum, the declaration shall identify as applicable:

- consuming Application/capability;
- external provider/broker/service role;
- account/environment scope;
- credential-reference class/issuer namespace when governed;
- whether required at subscription, feature enablement, runtime activation, or optional use;
- required lifecycle/validity state;
- revocation/unavailable/expired behavior;
- failure/degraded behavior;
- non-secret metadata safe for governed projection where later contracts permit it.

Current known FSATS separation remains:

- FSAPMA consumes governed provider credential references for operational-data roles;
- Trading Execution consumes governed broker-execution credential references;
- overlap of vendor/account does not merge these authorities;
- secret/key/token bytes SHALL NOT become Manifest plaintext, ordinary Application logs, Shared Web-owned state or reusable browser-visible state.

Bare FSATS subscription SHALL NOT imply that provider/broker secrets are present or valid.

```text
SUBSCRIBED != CREDENTIAL_READY
CREDENTIAL_REGISTERED != CREDENTIAL_VALID
CREDENTIAL_VALID != RUNTIME_AUTHORITY
```

Exact Foundation secret-storage/runtime egress realization remains outside P1-E and separately governed by the applicable Foundation/FCR path.

## Review Effect

The previous P1-E semantic freeze becomes historical for these remediated omissions. A new semantic freeze and fresh Architecture/Consistency + Red-Team review are mandatory before Owner decision.
