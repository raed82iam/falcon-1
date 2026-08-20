# FDN-004 — Foundation Configuration Catalog

**Identifier:** FDN-004  
**Version:** 1.1  
**Status:** Approved  
**Effective Date:** 2026-08-12  
**Approval Reference:** Explicit Project Owner approval in the Falcon Foundation workstream on 2026-08-12, limited to the Gate 0B additions defined in this version  
**Owner:** Falcon Configuration Authority  
**Governing Authority:** SYS-007; SEC-001; CON-007; ADR-F007; SYS-008 v1.1  
**Supersedes:** FDN-004 v1.0  
**Superseded By:** None

## 1. Purpose

This catalog declares every configuration item permitted in FRS-001. An undeclared item has no Foundation configuration authority.

This version synchronizes the existing Health configuration keys with the approved Stage 7 Gate 0B Health policy without creating new Health, Guardian, Authority, Lifecycle, or Recovery ownership.

## 2. Source Codes

- `D`: explicit safe default;
- `R`: approved release baseline;
- `E`: authorized environment profile;
- `I`: authorized instance override;
- `T`: time-bounded operational override.

Guardian restrictions are enforcement constraints, not configuration sources.

## 3. Catalog

| Key | Owner | Type | Allowed sources | Default policy | Validation | Sensitivity | Activation | Compatibility |
|---|---|---|---|---|---|---|---|---|
| `falcon.release.manifest_ref` | Release Authority | protected reference | R | none | valid CON-010 identity | Restricted | restart-bound | exact |
| `falcon.instance.environment` | Kernel Authority | enum | R,E | none | `foundation-test` only in FRS-001 | Internal | restart-bound | exact |
| `falcon.instance.id` | Security Authority | identity reference | I | none | unique and manifest-bound | Restricted | restart-bound | exact |
| `falcon.trust.anchor_ref` | Security Authority | protected reference | E | none | governed anchor reference | Restricted | restart-bound | exact |
| `falcon.trust.revocation_ref` | Security Authority | protected reference | E,I | none | authorized source and freshness | Restricted | staged | backward-compatible |
| `falcon.identity.issuer_ref` | Security Authority | protected reference | R,E | none | manifest-permitted issuer | Restricted | restart-bound | exact |
| `falcon.secret.provider_ref` | Security Authority | protected reference | E | none | approved provider identity | Restricted | restart-bound | exact |
| `falcon.authority.baseline_ref` | Governance Authority | protected reference | R | none | approved baseline identity | Restricted | restart-bound | exact |
| `falcon.lifecycle.restart_limit` | Lifecycle Authority | integer | R,E,I | safe default `0` | 0–3 for Foundation | Internal | restart-bound | additive |
| `falcon.lifecycle.transition_timeout` | Lifecycle Authority | duration | R,E,I | none | positive, bounded | Internal | restart-bound | additive |
| `falcon.fil.schema_ref` | Communication Authority | protected reference | R | none | FDN-002 schema identity | Internal | restart-bound | exact |
| `falcon.bus.retry_limit` | Communication Authority | integer | R,E,I | safe default `0` | 0–3 | Internal | staged | additive |
| `falcon.bus.message_max_age` | Communication Authority | duration | R,E,I | none | positive and consequence-bounded | Internal | staged | additive |
| `falcon.bus.queue_limit` | Communication Authority | integer | R,E,I | none | positive bounded test value | Internal | restart-bound | additive |
| `falcon.events.retention_class` | Evidence Authority | enum | R,E | none | approved Foundation class | Internal | restart-bound | exact |
| `falcon.evidence.store_ref` | Evidence Authority | protected reference | E | none | isolated approved store | Restricted | restart-bound | exact |
| `falcon.evidence.checkpoint_interval` | Evidence Authority | integer | R,E,I | none | positive bounded count | Internal | staged | additive |
| `falcon.health.freshness_window` | Health Authority | duration | R,E,I | none | positive; VPL-005 compatible; MAY only tighten the applicable SYS-008 freshness bound and SHALL NOT extend rule/source/TIM validity | Internal | staged | additive |
| `falcon.health.clock_tolerance` | Health Authority | duration | R,E | none | bounded by trust profile; SHALL NOT weaken TIM-001 or source clock-validity requirements | Internal | restart-bound | exact |
| `falcon.fitness.required_scope` | Self-Awareness Authority | set | R | none | registered Foundation scopes only | Internal | restart-bound | exact |
| `falcon.guardian.mandate_ref` | Protection Authority | protected reference | R | none | approved FDN-005 identity | Restricted | restart-bound | exact |
| `falcon.guardian.safe_allowlist_ref` | Protection Authority | protected reference | R | none | approved FDN-005 identity | Restricted | restart-bound | exact |
| `falcon.recovery.plan_ref` | Recovery Authority | protected reference | R,I,T | none | approved plan; T expires | Restricted | staged | exact |
| `falcon.verification.mode` | Verification Authority | enum | R,E | `disabled` outside authorized run | `disabled` or `foundation-isolated` | Internal | restart-bound | exact |
| `falcon.external.financial_access` | Constitutional Protection | boolean | R | fixed `false` | must remain `false` | Restricted | prohibited at runtime | exact |

## 4. Health Configuration Semantics

### 4.1 `falcon.health.freshness_window`

This key is an optional governed configuration ceiling used only to make an applicable SYS-008 Health freshness requirement stricter.

The effective permitted freshness for an evidence relation remains the strictest applicable bound among:

1. authoritative source validity or expiry;
2. the governing SYS-008 Health rule freshness profile;
3. `falcon.health.freshness_window`, when configured and stricter; and
4. governing temporal/clock validity under TIM-001.

Therefore:

```text
CONFIGURATION MAY TIGHTEN HEALTH FRESHNESS
CONFIGURATION SHALL NOT LOOSEN HEALTH FRESHNESS
CONFIGURATION SHALL NOT EXTEND SOURCE VALIDITY
CONFIGURATION SHALL NOT OVERRIDE TIM-001 WITH A LOOSER VALUE
```

A missing `falcon.health.freshness_window` does not erase the governing SYS-008 freshness profile. The key has no safe default because the normative profile remains owned by SYS-008 rather than by configuration fallback.

A configured value that is looser than the governing Health rule/source/TIM bound SHALL be rejected or ignored for positive reliance according to the applicable configuration-admission behavior; it SHALL NOT make the Health rule looser.

### 4.2 `falcon.health.clock_tolerance`

This key may express an authorized Health-specific tolerance only within the stricter governing trust/time boundary.

It SHALL NOT:

- relax TIM-001 clock-quality requirements;
- convert unacceptable clock quality into acceptable evidence;
- extend evidence beyond source expiry; or
- create an alternate time authority.

### 4.3 Freshness Profile Identity

The canonical initial SYS-008 freshness profiles are:

- `HFP-CRITICAL`;
- `HFP-FAST`;
- `HFP-STANDARD`;
- `HFP-SLOW`;
- `HFP-SOURCE_BOUND`; and
- `HFP-EVENT_BOUND`.

FDN-004 does not redefine the profile values or subject mappings. SYS-008 remains the governing Health policy source.

### 4.4 Runtime Feasibility Boundary

Configuration SHALL NOT be used to conceal an infeasible SYS-008 profile-to-source mapping.

If an accepted source cannot satisfy a proposed Health freshness profile without unauthorized predecessor modification or pathological resource/polling impact, the implementation SHALL return the affected mapping to governed Health policy review. It SHALL NOT loosen the value through configuration as a workaround.

## 5. Universal Rules

1. Every effective value SHALL expose source, version, authority, validation, effective time, and integrity.
2. A source not listed for the key is rejected.
3. Missing required values fail closed for the affected scope.
4. Secrets are protected references only.
5. A `T` override requires purpose, scope, owner, approval, activation, expiry, and rollback evidence.
6. No source can set `falcon.external.financial_access` to `true` in FRS-001.
7. Partial material activation triggers rollback or restriction according to the separately governed protective path.
8. Configuration SHALL NOT create Health, Fitness, Guardian, Authority, Lifecycle, Recovery, or FSA ownership that does not already exist in the governing Specification/Contract.
9. Health configuration SHALL NOT weaken SYS-008, CON-006, source-validity, or TIM-001 fail-closed behavior.

## 6. Approval

| Role | Decision | Name or Reference | Date |
|---|---|---|---|
| Project Owner | Approved | Explicit Owner approval of the Gate 0B additions only | 2026-08-12 |

## 7. Change History

| Version | Date | Change |
|---|---|---|
| 1.0 | 2026-07-24 | Initial approved Foundation Configuration Catalog. |
| 1.1 | 2026-08-12 | Added the Owner-approved Gate 0B synchronization for Health freshness/clock configuration: stricter-only freshness ceiling, SYS-008 profile ownership, TIM/source precedence, and prohibition on configuration-based policy weakening. |
