# ADR-F007 — Configuration Source and Precedence

**Identifier:** ADR-F007  
**Version:** 1.0  
**Status:** Accepted  
**Date:** 2026-07-24  
**Decision Owner:** Falcon Project Owner  
**Scope:** Foundation configuration sources, resolution, activation, and precedence  
**Affected Specifications:** SYS-007, SEC-001, CON-007, FRS-001  
**Applicable Standards:** STD-003, STD-013  
**Related ADRs:** ADR-F002, ADR-F005, ADR-F006, ADR-F008  
**Supersedes:** None  
**Superseded By:** None  
**Decision Record:** Project Owner approval recorded on 2026-07-24

## 1. Context

Falcon behavior depends on configuration whose origin, ownership, priority, validity, and effective value must remain knowable. Undeclared precedence, hidden overrides, ordinary-file secrets, or silent fallback could change system behavior without legitimate authority or make a past decision impossible to reconstruct.

FRS-001 requires one deterministic source hierarchy that supports controlled operational variation without permitting configuration to amend higher authority or weaken protective controls.

## 2. Decision Drivers

- resolve one reproducible effective value for every configuration item;
- preserve higher-authority and safety constraints;
- distinguish approved baseline values from environment and temporary overrides;
- make every effective value and source attributable;
- prevent configuration from creating authority;
- keep secrets outside ordinary configuration;
- support validated activation, rollback, expiry, and reconstruction; and
- fail safely when material configuration is missing, invalid, conflicting, or uncertain.

## 3. Higher-Authority Constraints

This decision is constrained by:

- the Vision priority of protection before operation or growth;
- constitutional requirements for legitimate authority, explicit change, safety, evidence, and governed evolution;
- SYS-007 requirements for deterministic precedence, visible effective values, validation, authorization, secure secrets, rollback, and safe failure;
- SEC-001 requirements for least authority, secret protection, integrity, and attributable privileged actions;
- CON-007 requirements for configuration definition, effective-value evidence, activation, and reconstruction; and
- FRS-001 requirements for governed effective configuration and trusted bootstrap.

## 4. Alternatives Considered

### 4.1 Last-loaded source wins

Configuration values could be applied in incidental loading order.

This was rejected because the effective result would depend on execution detail rather than declared authority.

### 4.2 One editable configuration source

All values could be maintained in one mutable source.

This was rejected because approved baseline, environment-specific values, emergency changes, and secrets would lose distinct ownership and lifecycle.

### 4.3 Governed layered resolution

Configuration is resolved from a fixed hierarchy. Each item declares which sources may supply or override it, and higher-authority constraints remain non-overridable.

This alternative was selected because it provides controlled flexibility with deterministic and reconstructable behavior.

## 5. Decision

FRS-001 SHALL resolve configuration through the following ordered source hierarchy, from lowest to highest permitted precedence:

1. **Explicit safe default:** a schema-defined value used only when the item permits a default.
2. **Approved release baseline:** the value bound to the verified Foundation release.
3. **Authorized environment profile:** a value approved for the declared operating environment.
4. **Authorized instance override:** a scoped value approved for one Falcon instance.
5. **Time-bounded operational override:** an exceptional, attributable value with explicit purpose, scope, activation mode, and expiry.

Every configuration item SHALL declare which of these sources are allowed. A higher-listed source SHALL NOT override an item unless the item’s governed definition permits that source and the change authority is valid.

Vision, Constitution, Specifications, Standards, Contracts, release invariants, security restrictions, and capital-protection constraints are not configuration layers. No configuration value or precedence rule may amend, disable, or bypass them or create new authority.

Guardian restrictions are protective enforcement constraints applied to the resolved configuration and operating authority. They MAY reduce, suspend, or deny permitted operation but SHALL NOT be displaced by any configuration source. Relaxation of a Guardian restriction requires the separately governed validation and authority defined for recovery.

Resolution SHALL produce one immutable effective-configuration snapshot containing each key, effective value or protected reference, source, source version, precedence, authority, validation result, effective time, and integrity identity.

Before activation, Falcon SHALL validate:

- source identity and integrity;
- schema, type, range, and compatibility;
- item ownership and allowed source;
- change authority;
- higher-authority constraints;
- cross-item invariants;
- required secret-reference availability without exposing the secret; and
- declared activation mode.

Material activation SHALL be atomic within its declared scope or SHALL remain unaccepted. Partial activation that could create inconsistent behavior SHALL be detected and shall cause rollback or protective restriction.

Time-bounded overrides SHALL expire automatically. Expiry SHALL restore the next valid value in the hierarchy through a recorded activation; it SHALL NOT silently convert the override into a permanent value.

Secrets SHALL be represented only by protected references resolved through an approved secret mechanism. Secret values SHALL NOT enter configuration snapshots, ordinary files, messages, or logs.

Missing, unknown, unsupported, invalid, unauthorized, conflicting, expired, integrity-failed, or unreconstructable material configuration SHALL prevent affected unrestricted operation. Falcon SHALL NOT silently fall back to a less protective value.

This decision does not select a configuration product, file format, secret provider, distribution protocol, or user interface.

## 6. Consequences

- The same valid sources resolve to the same effective configuration.
- Every effective value has a visible source and authority.
- Approved baseline and environment-specific values remain distinguishable.
- Temporary changes expire and remain historically reconstructable.
- Guardian protection cannot be disabled by configuration precedence.
- Components can evolve independently because each configuration item has explicit ownership and compatibility.
- Configuration schemas, cross-item validation, activation scope, and change authority require deliberate governance.
- Some changes may be rejected or delayed rather than applied partially.

## 7. Risks and Mitigations

- **Risk:** Too many override layers could make behavior difficult to understand.  
  **Mitigation:** Limit the hierarchy to the five declared sources and expose the complete effective snapshot and provenance.

- **Risk:** A temporary override could remain indefinitely.  
  **Mitigation:** Require expiry and automatic restoration through an evidenced activation.

- **Risk:** A valid individual value could violate a system-wide invariant.  
  **Mitigation:** Validate higher-authority and cross-item constraints before activation.

- **Risk:** Partial distribution could create conflicting effective behavior.  
  **Mitigation:** Require atomic activation within the material scope or protective restriction and rollback.

- **Risk:** Secrets could leak through snapshots or evidence.  
  **Mitigation:** Store protected references only and redact secret resolution details.

## 8. Compatibility and Transition

No prior Falcon1 ADR is superseded.

Every FRS-001 configuration item shall be cataloged under CON-007 with its owner, allowed sources, default policy, validation, sensitivity, activation mode, and compatibility before implementation authorization.

Future remote configuration, dynamic policy, or multi-environment distribution requires a later ADR when it introduces a new authority or trust boundary. It shall preserve this precedence meaning and effective-snapshot evidence.

## 9. Conformance Evidence

Conformance shall be demonstrated by evidence that:

- identical valid sources produce an identical effective snapshot;
- each effective value exposes its permitted source, version, and authority;
- an unauthorized source or override is rejected;
- a higher-precedence value cannot violate a higher-authority constraint;
- missing, invalid, unknown, expired, or integrity-failed material values prevent affected unrestricted operation;
- time-bounded overrides expire and restore the next valid value with evidence;
- partial material activation is detected and contained;
- rollback restores a known valid snapshot without deleting failed-change evidence;
- Guardian restriction cannot be relaxed through configuration; and
- historical effective configuration can be reconstructed for every FRS-001 scenario.

## 10. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner and current Constitutional Authority | Accepted | رائد عموره — “موافق على القرار السابع” | 2026-07-24 |
