# FSATS Part 6 — Scope and Work-Package Baseline

**Status:** `OWNER_AUTHORIZED / APPLICATION_DEFINED_CURRENT_SCOPE`  
**Branch:** `application-development`  
**Runtime authority:** `NOT_GRANTED`

## Mission

Part 6 establishes **Application-Owned Configuration, Policy Binding, Environment Isolation, and Safe Reconfiguration** for all five independent FSATS Applications.

The mission is to make configuration an explicit, typed, attributable and fail-closed Application-owned input without allowing configuration values, feature toggles, environment names, policy references, or reload/reconfiguration events to manufacture runtime, lifecycle, Foundation, external-egress, capital-risk, protection-release, or deployment authority.

## Why Part 6 Exists

CON-023 requires every Falcon Application to declare configuration requirements. Parts 2 through 5 established execution/data/protection/resource/simulation truth, restart durability, lifecycle evolution, stale-authority fencing, and health/readiness truth. The next Application-owned gap is safe configuration and policy binding across those established semantics.

Historical blueprint material also identified typed, versionable, environment-scoped configuration as a desired implementation property, but Part 6 adopts only the portions compatible with current authority and the five-Application architecture.

## Prime Invariants

```text
CONFIG_PRESENT != AUTHORIZED
CONFIG_VALID != ACTIVE
CONFIG_VALID != ADMITTED
CONFIG_CHANGE != AUTHORITY_EXPANSION
CONFIG_RELOAD != TRUST_RESTORATION
ENVIRONMENT_NAME != ENVIRONMENT_AUTHORITY
FEATURE_ENABLED_IN_CONFIG != FEATURE_AUTHORIZED
POLICY_REFERENCE != POLICY_AUTHORITY
SECRET_REFERENCE != SECRET_BYTES
UNKNOWN_CONFIG_VERSION != COMPATIBLE
STALE_CONFIG_EPOCH != CURRENT_CONFIGURATION
CROSS_APPLICATION_CONFIG_PROJECTION != CROSS_APPLICATION_OWNERSHIP
ROLLBACK_CONFIG != BUSINESS_STATE_ROLLBACK
ALL_CONFIG_GREEN != OWNER_APPROVAL
```

Unknown, stale, malformed, contradictory, integrity-failed, environment-crossing, or authority-expanding configuration SHALL fail closed for the affected capability.

## Canonical Applications

```text
FSATS-TRADING
FSATS-FSAPMA
FSATS-TRADING-GUARDIAN
FSATS-FSTSIMA
APP-RSC
```

FSATS itself remains a non-owning/non-runtime system boundary and SHALL NOT become a shared mutable configuration owner.

## Work Packages

### P6-A — Common Configuration Identity and Integrity Envelope

Each Application configuration evaluation shall express locally:

- exact Application identity;
- configuration identity;
- configuration version;
- configuration epoch;
- payload/configuration digest;
- evidence/provenance identity;
- evidence-integrity state;
- environment identity where material;
- compatibility state;
- explicit authority refusal.

Malformed identities, undefined enum values, missing digest/evidence, epoch mismatch and non-valid evidence integrity fail closed.

### P6-B — Trading Configuration and Policy Binding

Trading configuration remains broker-account centric and evaluates at minimum:

- exact `BrokerId + BrokerAccountId + Environment`;
- current configuration epoch;
- Risk policy reference;
- Strategy policy reference;
- execution enablement request as a separately authorized concern;
- cross-account scope expansion attempts;
- secret-byte prohibition;
- configuration compatibility and migration need.

Trading SHALL NOT introduce CustomerId/UserId ownership.

A configuration value cannot authorize broker execution or capital-risk increase.

### P6-C — FSAPMA Provider Configuration and Secret-Reference Boundary

FSAPMA evaluates at minimum:

- ProviderId;
- ProviderAccountId;
- ServiceRole;
- Environment;
- current configuration epoch;
- capability/profile policy reference;
- quota/entitlement policy reference;
- governed credential reference identity;
- secret-byte prohibition;
- provider-egress enablement request as a separate-authority concern;
- compatibility/migration state.

A credential reference may be named; secret bytes remain outside ordinary Application configuration.

### P6-D — Trading Guardian Protection Configuration

Guardian evaluates at minimum:

- exact protected target identity;
- protection policy reference;
- configuration epoch;
- compatibility/migration state;
- attempts to weaken deterministic hard protections;
- attempts to permit self-release from containment/restriction;
- attempts to convert configuration into Foundation protection-route authority.

Configuration SHALL NOT weaken valid hard protection or create self-release authority.

### P6-E — APP-RSC Resource-Coordination Configuration

APP-RSC evaluates at minimum:

- coordinator/configuration epoch;
- Foundation envelope/reference identity;
- Application-internal resource profile reference;
- safety-floor policy reference;
- compatibility/migration state;
- attempts to mint, increase or reinterpret Foundation grants/ceilings/floors.

APP-RSC may configure coordination behavior only inside separately admitted Foundation resource authority.

### P6-F — FSTSimA Simulation Configuration and Environment Isolation

FSTSimA evaluates at minimum:

- simulation profile reference;
- run-classification policy reference;
- environment identity;
- current configuration epoch;
- compatibility/migration state;
- replay/synthetic/operational classification preservation;
- any attempt to configure Live/production egress or convert simulation configuration into operational qualification authority.

FSTSimA configuration cannot create Live authority.

### P6-G — Feature Toggles and Authority Refusal

Feature toggles are bounded configuration declarations only.

```text
FEATURE_CONFIGURED_ON != FEATURE_AUTHORIZED
FEATURE_CONFIGURED_OFF != REQUIRED_SAFETY_DISABLED
```

A toggle that would increase authority, enable external side effects, disable a mandatory safety fence, or cross environment boundaries requires separate governed authority and cannot be applied by configuration alone.

### P6-H — Safe Reconfiguration / Compatibility / Migration Gate

Configuration changes distinguish at least:

```text
Compatible
MigrationRequired
Incompatible
Unknown
```

For a proposed configuration transition, preserve:

- source configuration identity/version/epoch;
- target configuration identity/version/epoch;
- target digest/provenance;
- compatibility result;
- required migration evidence where applicable;
- authority-change detection;
- environment-change detection;
- rollback/corrective-action declaration.

```text
MIGRATION_REQUIRED + NO_VALIDATED_MIGRATION_EVIDENCE -> NOT_READY
INCOMPATIBLE -> REJECT
UNKNOWN -> REJECT
AUTHORITY_EXPANSION -> REQUIRES_SEPARATE_AUTHORITY
ENVIRONMENT_ESCALATION -> REQUIRES_SEPARATE_AUTHORITY
```

### P6-I — Bounded Configuration Projection Contract

Materialize a declaration-only Application configuration projection suitable for future governed consumers without creating shared mutable configuration state.

Producer owns configuration business meaning. Consumers receive bounded projection only.

```text
PROJECTION_CONSUMPTION != INTERNAL_CONFIG_ACCESS
PROJECTION_PRESENT != CONFIG_CURRENT
PROJECTION_VALID != AUTHORITY_GRANTED
```

No Shared Web implementation is included in Part 6.

### P6-J — Integrated Adversarial Verification

Executable checks shall challenge at minimum:

- wrong/malformed Application identity;
- missing ConfigId/version/digest/evidence;
- invalid evidence-integrity or compatibility enum;
- stale configuration epoch;
- unknown/incompatible configuration accepted;
- migration-required transition without migration evidence;
- environment crossing represented as ordinary reconfiguration;
- feature toggle treated as authority;
- Trading cross-account expansion;
- broker execution enabled by config alone;
- provider egress enabled by config alone;
- provider secret-byte configuration;
- Guardian hard-protection weakening/self-release;
- APP-RSC Foundation grant minting;
- FSTSimA Live escalation;
- config rollback treated as business-state rollback;
- configuration result granting runtime, deployment or Part 7 authority.

## Required Implementation Shape

Part 6 preserves maintainability and replaceability:

- one small local deterministic configuration evaluator per Application;
- no shared mutable FSATS configuration owner;
- no direct project reference to another Application's internals;
- typed records/enums and deterministic reason codes;
- no network/database/Foundation implementation dependency;
- declaration-only cross-boundary projection contract;
- configuration evaluation separated from side-effect execution;
- earlier Part 3/4/5 durability, lifecycle and health semantics remain authoritative inputs rather than being reimplemented.

## Explicit Exclusions

Part 6 does **not** authorize:

- Foundation configuration/lifecycle/security internals;
- Foundation source modification;
- Shared Web source modification;
- external provider/broker egress;
- secrets/credential bytes;
- production persistence/configuration service binding;
- actual hot-reload side effects;
- runtime activation;
- Paper, Shadow, Tiny-Live, Live;
- deployment;
- FSA internals or MSA-to-FSA runtime transport;
- Part 7 through Part 10.

## Exit Criteria

Part 6 becomes eligible for Owner final closure only after:

1. P6-A through P6-I implementation is complete under `applications/**`.
2. all five Applications have deterministic local configuration evaluators.
3. configuration never grants runtime/Foundation/external-egress authority.
4. Trading remains broker-account centric and cross-account scope cannot expand through config.
5. provider credential references remain distinct from secret bytes.
6. Guardian mandatory protection cannot be weakened/released through config.
7. APP-RSC cannot mint or expand Foundation resource authority.
8. FSTSimA configuration cannot create Live/production authority.
9. stale/unknown/incompatible/integrity-failed configuration fails closed.
10. migration-required changes need validated migration evidence.
11. Release build passes.
12. direct Part 6 adversarial verification passes.
13. governed Application verifier suite passes twice on the same exact source.
14. final validation HEAD is exact and tree clean.
15. fresh post-executable Architecture/Consistency review passes.
16. fresh broad post-executable Red-Team passes with `0 Critical / 0 High / 0 Medium` open findings.
17. Project Owner explicitly accepts and closes Part 6.

## Current Authority State

```text
PART 5 = OWNER_ACCEPTED_AND_CLOSED
PART 6 = OWNER_AUTHORIZED_AND_ACTIVE
PART 6 SCOPE = DEFINED
PART 6 IMPLEMENTATION = AUTHORIZED
PART 7 THROUGH PART 10 = NOT_AUTHORIZED
RUNTIME = NOT_AUTHORIZED
PROVIDER / BROKER CONNECTIVITY = NOT_AUTHORIZED
PAPER / SHADOW / TINY-LIVE / LIVE / DEPLOYMENT = NOT_AUTHORIZED
```
