# FSATS V1.4 PROPOSED - Cross-Application Contract Matrix

## Status

**Status:** `PART 0 ALIGNMENT CANDIDATE / OWNER REVIEW REQUIRED`  
**Authority:** design only. Runtime routing is not assumed available until the relevant Foundation capability is authorized, implemented and admitted.

## Contract boundary rule

Cross-Application contracts preserve V1.3 business semantics but SHALL use current Foundation-declared routes. No component, LSA, CSA or Application may create direct private-memory/database/file coupling across Application boundaries.

| ID | Route family | Producer | Consumer | Purpose | Required properties | Foundation dependency / canonical FCR |
|---|---|---|---|---|---|---|
| CA-001 | Market Data Requirement | Trading | FSAPMA | request provider-independent data/capability/subscription outcome | identity, schema, authority, correlation, idempotency, expiry, evidence | FCR-0005 / FCR-0009 when latency-sensitive |
| CA-002 | Normalized Operational Market Data | FSAPMA | Trading | deliver normalized operational trading data | freshness, quality, provenance, duplicate/correction handling, degradation, deadline/traffic class where applicable | FCR-0005 / FCR-0009 |
| CA-003 | Provider/Data Degradation Event | FSAPMA | Guardian | surface provider/data integrity or availability degradation | severity, affected capability/scope, freshness/integrity state, evidence, expiry | FCR-0006 / FCR-0009 where urgent |
| CA-004 | Trading Protection Command | Guardian | Trading | apply governed scoped restriction/protection action | explicit authority, target user/account/market/system scope, Guardian epoch, expiry, idempotency, rejection semantics, evidence, fail-closed behavior | FCR-0004 / FCR-0009 |
| CA-005 | Trading Safety State Projection | Trading | Guardian | provide bounded state required for Guardian assessment | scoped projection, provenance, freshness, deadline where relevant, evidence, no hidden internal access | FCR-0006 / FCR-0009 |
| CA-006 | Provider Protection Command | Guardian | FSAPMA | apply scoped provider-use restriction/protection behavior | explicit authority/scope, expiry, idempotency, evidence, no Foundation-network ownership | FCR-0004 / FCR-0009 |
| CA-007 | Provider Operational Status Projection | FSAPMA | Guardian | provider health/degraded-mode status | freshness, confidence, lineage, affected scope, evidence | FCR-0006 / FCR-0009 where urgent |
| CA-008 | Application Resource Escalation Request | Guardian | Foundation Guardian / resource authority | request additional technical resources for an affected Application during broad evidenced trading danger | affected Application, reason, resource type, requested limits/duration, urgency evidence, restoration conditions, attributable decision result | FCR-0007 / FCR-0010 |
| CA-009 | User/Operator Presentation Projection | Guardian / FSAPMA / Trading | Falcon Web Application | present authorized state/status/evidence | least privilege, read/write separation, auth/security boundary, no direct internal access | external Shared Application contract; further FCR only if a confirmed gap appears |
| CA-010 | User/Operator Command Request | Falcon Web Application | authorized FSATS Application | submit a user/operator business command | authenticated identity, role/entitlement, explicit target Application, authority, anti-replay, idempotency, evidence, rejection semantics | current Foundation auth/route dependencies; further FCR only if a confirmed gap appears |
| CA-011 | Notification/Report Request | Guardian / FSAPMA / Trading / FSTSimA | Communication Application | request operator/user notification or report delivery | recipient abstraction, severity, idempotency, evidence, delivery result, no channel ownership in producer | external Shared Application contract |
| CA-012 | Simulation Run Input / Component Package Reference | authorized owner | FSTSimA | provide approved versions/config/scenario inputs for non-Live simulation | immutable identities/digests, explicit non-Live authority, no production credentials, replay-safe context | FCR-0011 plus ordinary manifest/route dependencies |
| CA-013 | Simulation Experience and Evidence Package | FSTSimA | Trading / Guardian / FSAPMA improvement workflows | return reproducible simulation evidence/candidate findings | run manifest, fidelity level, seed/clock/config/version digests, truth-oracle evidence, non-authoritative-for-live marker | FCR-0006 / FCR-0011 |
| CA-014 | Calibration Comparison Input | authorized Paper/Tiny-Live evidence owner | FSTSimA | compare simulator prediction with separately authorized real-environment evidence | read-only evidence reference, authority context, no production mutation, provenance | FCR-0006 / FCR-0011 |
| CA-015 | Research Egress Request | MSA / LSA / eligible CSA through owning Application boundary | Foundation-governed research egress boundary | research/learning/development only | room/Application attribution, purpose, policy, destination controls, evidence, no operational trading-data classification | FCR-0008 |

## Contract invariants

1. Application-level governed boundaries own cross-Application ingress/egress. Internal components do not become independent cross-Application principals without explicit justification.
2. Foundation treats business payload meaning as opaque except where a separately governed inspection responsibility applies.
3. Route existence, compatibility, registration or technical reachability never grants business authority.
4. Operational, replay, simulation and research contexts must be unambiguously distinguishable.
5. Expired, unauthoritative, malformed, incompatible, stale or insufficiently evidenced protection/control messages fail closed.
6. Trading must define safe degraded behavior for unavailable/stale FSAPMA or Guardian routes before implementation authorization.
7. FSTSimA outputs are evidence only and cannot become live side effects or candidate promotion authority.
8. Research Internet output is not operational trading truth and cannot bypass FSAPMA.
9. Deadline/traffic-class metadata may preserve Fast Track but cannot weaken authority, Risk, Guardian, evidence or reconciliation gates.
10. Every route must eventually bind to exact schema/version, permissions/security, retry/ordering/correction semantics, evidence and lifecycle behavior before runtime use.

## V1.3 contract migration treatment

The 34 V1.3 Integration Contract artifacts remain migration inputs. Business semantics are preserved unless explicitly superseded, while obsolete Foundation service/FSA/resource/route assumptions are rebound to current APP-001, CON-023, ADR-I012, ADR-I015, SYS-006 and applicable canonical FCR outcomes.

## Canonical FCR mapping

- FCR-0004 / Issue #4: Guardian protection command routes.
- FCR-0005 / Issue #5: operational market-data delivery.
- FCR-0006 / Issue #6: event/evidence/replay and simulation-evidence delivery.
- FCR-0007 / Issue #7: Trading Guardian to Foundation Guardian/resource escalation.
- FCR-0008 / Issue #8: research-only awareness Internet egress.
- FCR-0009 / Issue #9: latency/deadline/QoS-aware Application transport.
- FCR-0010 / Issue #10: Application resource-pressure/load-shedding signals.
- FCR-0011 / Issue #11: non-Live FSTSimA isolation, permission separation and egress guard.

No FCR grants runtime or Foundation modification authority.
