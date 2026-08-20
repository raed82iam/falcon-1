# FSATS Specialized Implementation Architecture — Security, Authority, Failure, Observability and Configuration Specification

**Package:** `FSATS-SIA-v0.1`
**Status:** `DESIGN_CANDIDATE`

## 1. Purpose

Define the cross-cutting controls that every Application/module must implement consistently. Security, authority, failure behavior, telemetry and configuration are not optional implementation polish; they are part of observable semantics.

## 2. Trust Boundary Model

Trust boundaries exist at minimum between:

1. Foundation and each Application;
2. one Application and another Application;
3. Application host and external provider/broker adapter;
4. operational runtime and FSTSimA/non-Live runtime;
5. active production artifacts and candidate/research artifacts;
6. Awareness target and Monitor AI observation/control surfaces;
7. user/Web intent and target business authority;
8. process memory and persistent authoritative state;
9. configured identity/profile and untrusted external payload.

Crossing a trust boundary requires explicit contract, identity, authority/permission, validation and evidence.

## 3. Authority Decision Model

Every material action is evaluated as:

```text
ACTION_ELIGIBLE =
  ValidActorIdentity
  AND ValidTargetIdentity
  AND ValidAuthorityInstrument
  AND AuthorityCoversAction
  AND AuthorityCoversScope
  AND AuthorityCoversEnvironment
  AND AuthorityIsCurrent
  AND ApplicationLifecycleAllows
  AND SecurityValidationPasses
  AND BusinessPreconditionsPass
```

A missing/unknown element is not TRUE.

Authority is separate from technical capability:

```text
CAN_CALL_METHOD != AUTHORIZED_TO_ACT
ROUTE_EXISTS != AUTHORITY
REGISTERED != AUTHORITY
MODEL_CONFIDENCE != AUTHORITY
OWNER_SILENCE != AUTHORITY
```

## 4. Application Principal Matrix

Initial business principals:

```text
APP-TRD
APP-PMA
APP-GRD
APP-SIM
APP-RSC candidate
Shared Web Application principal (external to this core SIA)
Shared Communication Application principal (external)
User/Human principals through governed authentication/authorization
```

FSATS grouping is not a principal.

## 5. Action Authority Matrix

Legend: `O` owns business action; `C` may consume/request through contract; `N` no authority by role alone.

| Action | Trading | FSAPMA | Guardian | FSTSimA | APP-RSC candidate |
|---|---:|---:|---:|---:|---:|
| create strategy evaluation | O | N | N | non-authoritative simulation only | N |
| create TradeProposal | O | N | N | simulation only | N |
| Risk ALLOW/DENY | O | N | N | simulate only | N |
| reserve Trading capital | O | N | N | simulated ledger only | N |
| submit broker order | O through T09 + future egress | N | N | simulated only | N |
| acquire operational provider data | N | O through future egress | N | simulated/replay only | N |
| normalize Data Product | N | O | N | simulate/validate only | N |
| issue protection directive | C target | C target | O within granted authority | N | N |
| release Guardian restriction | C target | C target | O within recovery authority | N | N |
| validate candidate in simulation | C requester | C requester | C requester | O non-authoritative | C requester |
| coordinate FSATS effective resources | report/comply | report/comply | report/comply | report/comply | O inside envelope |
| grant Foundation resources | N | N | N | N | N |
| alter Foundation lifecycle/permissions | N | N | N | N | N |
| approve production adoption | N | N | N | N | N |

## 6. Environment Authority

Environment classes:

```text
SIMULATION
REPLAY
SHADOW
PAPER
TINY_LIVE
LIVE
```

Each action requiring an operating environment checks an external governed `EnvironmentAuthorityRef`/Owner authority, not only a local config flag.

A package deployed into a Live-capable host remains unable to act Live without explicit effective authority.

No lower environment authority implies higher:

```text
PAPER_AUTHORITY != TINY_LIVE_AUTHORITY
TINY_LIVE_AUTHORITY != LIVE_AUTHORITY
```

## 7. Credential Security

Applications store only credential references/aliases supported by the future Foundation credential boundary.

Forbidden:

- plaintext API key/secret in source;
- secret in manifest/business DB/log/event/error/metrics;
- credential copied between Applications;
- FSTSimA access to production credential reference;
- credential passed through generic user/config payload;
- automatic fallback to a different credential/account without exact profile/authority mapping.

FCR-0013/0014 remain external egress/credential capability dependencies.

## 8. External Input Validation

Untrusted inputs include:

- provider payloads;
- broker responses/webhooks;
- user intents;
- model/research artifacts;
- external files/datasets;
- cross-App payload until Foundation + business validation complete;
- configuration supplied outside trusted deployment pipeline.

Validation sequence:

```text
size/structural limits
-> encoding/parse
-> schema/version
-> identity/scope
-> integrity/provenance
-> authority/classification
-> business invariants
-> only then domain processing
```

Parser failure must not expose stack/secret data to external caller.

## 9. Input Size / Resource Abuse

Every endpoint/consumer defines maximum:

- message/payload bytes;
- collection item counts;
- string lengths;
- nested depth;
- decompressed size;
- batch count;
- concurrent requests;
- historical window request.

Oversize input is rejected before expensive model/parsing work where possible.

## 10. Data Classification

Initial classes:

```text
PUBLIC_MARKET_DATA
LICENSED_MARKET_DATA
BUSINESS_INTERNAL
FINANCIAL_ACCOUNT_SENSITIVE
USER_PII
CREDENTIAL_SECRET
SECURITY_AUTHORITY_SENSITIVE
INCIDENT_FORENSIC_SENSITIVE
MODEL_CANDIDATE_INTERNAL
GOVERNANCE_EVIDENCE
```

Each contract/persistence/config field declares a class where non-obvious.

Secrets never enter normal telemetry. PII/account data telemetry uses redacted/tokenized stable references only when operationally required.

## 11. Least Privilege

Each Application/adapter/database role gets only required capabilities.

Examples:

- Trading provider-data consumer can receive declared Data Products, not call FSAPMA provider adapters;
- Guardian receives bounded Trading safety projection, not full portfolio DB;
- Web receives presentation projection, not broker credentials;
- Communication receives template parameters/recipient selector, not Trading database;
- FSTSimA consumes sanitized/replay fixtures, not production secrets;
- APP-RSC consumes resource reports, not business-state databases.

## 12. Protection Against Confused Deputy

Any privileged Application action requested by another Application/user verifies:

- requester identity;
- exact requested target/scope;
- requester authority to request that class;
- owner Application's own policy/authority;
- no scope expansion during translation.

Examples:

- Web `user-intent` cannot ask Guardian to issue arbitrary protection command unless user/Guardian business policy separately authorizes it;
- Trading cannot label data demand "critical" to obtain Foundation resources;
- Guardian resource consequence evidence cannot be translated into Foundation technical criticality by APP-RSC.

## 13. Replay and Cross-Environment Protection

Every operational command/request carrying business effect includes sufficient identity/idempotency/classification/expiry to reject:

- old operational replay;
- REPLAY/SIMULATION/TEST payload on operational route;
- Paper command on Live account;
- command from wrong Application/environment;
- stale protection/resource coordinator epoch;
- duplicate conflicting payload.

Captured production messages used in FSTSimA are reclassified at the transport/business boundary and cannot retain operational authority.

## 14. Failure Taxonomy

Common failure classes:

```text
VALIDATION_FAILURE            // deterministic bad input/precondition
AUTHORITY_FAILURE             // missing/invalid/out-of-scope authority
SECURITY_FAILURE              // integrity/authentication/confidentiality/trust failure
DEPENDENCY_UNAVAILABLE        // required service/provider/Foundation unavailable
TRANSIENT_EXTERNAL_FAILURE    // retry may be safe under exact state/contract
RESOURCE_OVERLOAD             // local/resource capacity insufficient
AMBIGUOUS_EXTERNAL_OUTCOME    // effect may or may not have occurred
CONCURRENCY_CONFLICT          // state changed from expected version
STATE_CONFLICT                // contradictory authoritative evidence
RECONCILIATION_REQUIRED       // cannot establish current truth safely
BUSINESS_REJECTION            // valid request denied by business rules
POLICY_INCOMPATIBLE           // config/schema/strategy/profile incompatibility
INTEGRITY_INCIDENT            // protected identity/evidence/architecture anomaly
INTERNAL_DEFECT               // unexpected implementation error
```

## 15. Failure Disposition

Every failure maps to one of:

```text
REJECT_TERMINAL
RETRY_BOUNDED
DEGRADE
FAIL_CLOSED
RECONCILE_BEFORE_RETRY
ESCALATE_PROTECTION
ESCALATE_INTEGRITY
PAUSE_SCOPE
```

A generic exception handler SHALL NOT choose disposition without the owning operation's failure policy.

## 16. Retry Matrix

### Generally retryable when contract says safe

- transient provider read request with idempotent semantics;
- Foundation publication/delivery attempt under accepted bounded retry;
- read-only current-state query;
- deterministic analytics rebuild.

### Reconcile before retry

- broker order submission with unknown outcome;
- cancel/replace unknown effect;
- capital/resource effect where commit/external outcome uncertain;
- Guardian command target effect unknown when repeating action can cause a second business effect.

### Never retry as transient success path

- authority denied;
- signature/integrity failure;
- schema incompatibility;
- invalid state transition;
- stale/superseded command;
- policy hard rejection;
- unsupported market/broker capability.

## 17. Fail-Closed by Consequence

New risk/funds/protection release/resource expansion fail closed on material unknown.

Risk-reducing actions may use an explicitly safer degraded path when:

- actor authority remains valid;
- target identity is exact;
- action cannot increase exposure beyond current state;
- external semantics permit safe idempotent handling;
- evidence is preserved.

"Fail closed" does not mean blindly freeze all safety-reducing exits if safe exit is possible.

## 18. Error Reason Code Standard

Reason code format:

```text
<DOMAIN>_<SUBSYSTEM>_<REASON>
```

Examples:

```text
TRD_RISK_LIMIT_EXCEEDED
TRD_CAPITAL_INSUFFICIENT_UNRESERVED
TRD_EXEC_BROKER_OUTCOME_AMBIGUOUS
PMA_ROUTE_QUOTA_INSUFFICIENT
PMA_DATA_SOURCE_CONFLICT
GRD_DIRECTIVE_AUTHORITY_INVALID
GRD_EFFECT_RECONCILIATION_REQUIRED
SIM_RUN_INPUT_DIGEST_MISMATCH
RSC_ENVELOPE_STALE
RSC_TARGET_EFFECT_NOT_CONFIRMED
AWR_PROTECTED_ARCHITECTURE_MISMATCH
```

Rules:

- stable machine code separate from human message;
- no dynamic secret/value embedded in code;
- unknown reason is explicit `*_UNKNOWN`, not blank;
- same semantic reason keeps meaning across versions.

## 19. Evidence Requirement

Every high-consequence decision records:

- who/what acted;
- exact input snapshot IDs;
- exact policy/model/config versions;
- authority/permission refs;
- decision/outcome;
- reason codes;
- rejected alternatives where required for reconstructability;
- correlation/causation;
- resulting aggregate version;
- relevant external evidence refs.

At minimum high-consequence includes:

- Risk decision;
- capital reservation/release;
- broker submission/cancel/replace/fill reconciliation;
- Guardian incident/directive/release;
- FSARM redistribution/Foundation request/restoration;
- strategy/model promotion candidate review;
- Awareness integrity hold/recovery recommendation;
- Application config/policy activation affecting authority/risk/contracts.

## 20. Observability Principles

```text
METRIC != EVIDENCE OF AUTHORITY
LOG != AUTHORITATIVE STATE
TRACE != BUSINESS SUCCESS
HEALTHY_PROCESS != FIT_TO_TRADE
```

Telemetry supports operations; authoritative decisions remain in governed state/evidence.

## 21. Logging

Structured logs contain:

```text
TimestampRef
Severity
ApplicationId
LSA/Component
EventCode
CorrelationRef
CausationRef where applicable
Subject tokenized ref when needed
ReasonCode
StateVersionRef?
EvidenceRef?
```

No free-form sensitive dump by default.

Forbidden log data:

- secrets/tokens/passwords;
- full broker/provider credentials;
- raw PII unless explicitly approved/redacted;
- full high-volume market payload except quarantined diagnostic capture with separate retention/access;
- model prompt/research contents containing secrets without classification/redaction.

## 22. Metrics

Metric naming:

```text
falcon_fsats_<application>_<subsystem>_<metric>_<unit?>
```

Required categories:

### Trading

- decision latency distribution;
- proposal no-trade/rejection counts by bounded reason category;
- risk deny/reduction rates;
- capital reserved/available ratios (aggregated, no account IDs as metric labels);
- order ambiguity/reconciliation counts;
- fill/slippage/fee metrics;
- strategy calibration/drawdown aggregates;
- data-quality dependency degradation.

### FSAPMA

- provider route health/latency;
- Data Product quality/freshness;
- quota headroom/reservation failures;
- reconciliation conflicts/gaps;
- provider failover counts.

### Guardian

- signal/incident/directive counts by bounded category;
- directive effect latency;
- effect failure/partial/reconciliation counts;
- crisis level duration;
- false-positive/false-negative review metrics where available.

### FSTSimA

- run throughput/duration;
- reproducibility failures;
- fidelity metrics;
- scenario coverage;
- checkpoint/reclaim metrics.

### APP-RSC candidate

- resource deficit by class aggregated;
- redistribution actions/effect latency;
- reclaim confirmation failures;
- Foundation request partial/deny counts;
- restoration duration;
- stale report/envelope events.

### Awareness

- candidate counts/status;
- calibration/drift findings;
- Monitor disagreements;
- integrity checks/holds;
- research/experiment evidence coverage.

## 23. Metric Cardinality

Forbidden default labels:

- InstrumentId;
- OrderId/OrderChainId;
- PositionId;
- UserId;
- raw Provider request ID;
- CandidateArtifactId.

Use bounded category/profile labels. High-cardinality detail belongs in logs/traces/evidence queries.

## 24. Distributed Tracing

Cross-Application calls preserve Foundation correlation/causation. Internal spans add Application/LSA/component but do not replace canonical identities.

Trace sampling must never be the only evidence for high-consequence action. Required evidence persists independently even if trace not sampled.

## 25. Health Model

Each component reports technical health:

```text
HEALTHY
DEGRADED
UNAVAILABLE
UNKNOWN
```

Application business readiness is separately derived by MSA/business controllers.

Examples:

- Trading process HEALTHY but market data CONFLICTED => NOT READY for new risk;
- FSAPMA process HEALTHY but all provider routes quota-blocked => Data Product UNAVAILABLE;
- Guardian process DEGRADED may require stronger protection posture;
- APP-RSC process unavailable => no new cross-App redistribution.

## 26. SLO/SLA Boundary

SLO metrics/targets are versioned operational policy. They SHALL NOT be invented from architecture prose where no Owner-accepted numeric target exists.

The implementation must measure the required distributions from day one. Promotion policies can then bind exact thresholds through governed configuration/evidence.

Missing SLO target does not permit unbounded behavior; local queue/timeouts still have safe finite configuration constraints.

## 27. Configuration Ownership

Configuration is divided:

```text
STATIC_BUILD_CONFIG
DEPLOYMENT_CONFIG
BUSINESS_POLICY_CONFIG
MARKET_PROFILE_CONFIG
PROVIDER_BROKER_CERTIFICATION_CONFIG
STRATEGY_MODEL_CONFIG
SECURITY_AUTHORITY_CONFIG
RESOURCE_CONFIG
OBSERVABILITY_CONFIG
```

Each config key declares an owner and mutability.

## 28. Canonical Config Record

```text
ConfigKey
ConfigVersion
OwnerApplication/Authority
Type
Unit
AllowedRange/Enum
Default?                         // only if safe/default is explicitly defined
Required = bool
EnvironmentScope
EffectiveFrom
MutableAtRuntime = bool
RequiresRestart = bool
RequiresRevalidation = bool
SecurityClassification
ValueDigest
Authority/ChangeDecisionRef
```

A missing required config prevents affected subsystem readiness.

## 29. Runtime Config Change

Material runtime config changes use:

```text
PROPOSED
-> VALIDATED
-> AUTHORIZED
-> STAGED
-> ACTIVATED
-> VERIFIED
```

Risk/authority/contract/strategy semantic changes cannot be hot-edited as ordinary config merely because the storage system supports it.

A rejected/failed activation keeps prior valid config active where safe, or fails affected subsystem closed.

## 30. Config Categories Requiring New Semantic Version/Review

At minimum:

- Risk limits/order;
- Guardian incident/protection policy;
- strategy formulas/thresholds that change behavior materially;
- market exposure/short/leverage policy;
- contract/schema compatibility;
- FSARM coordination priority/protected-minimum policy;
- Awareness protected properties/self-development policy;
- Monitor AI governing profile;
- production environment authority mapping.

## 31. Config Drift

At startup and periodically for protected config:

- compare active digest/version to admitted manifest/deployment/governance identity;
- unexplained material change => integrity/degraded/fail-closed according to scope;
- do not silently "adopt current file as new baseline".

## 32. Feature Flags

Feature flags cannot bypass governance.

Allowed: enable an already admitted compatible implementation path under declared authority.

Forbidden:

- `EnableLive=true` creating Live authority;
- `SkipRisk=true`;
- `DisableGuardian=true`;
- `IgnoreSchemaMismatch=true`;
- `AllowAnyProvider=true`;
- `DisableEvidence=true`.

Any flag that changes authority/protection must itself be governed as authority/policy, not an ordinary feature flag.

## 33. Threat Model Required Scenarios

Security verifier/red-team SHALL challenge:

- forged Application identity;
- forged Guardian directive;
- replayed valid old directive;
- Paper->Live classification swap;
- replay/simulation message to operational route;
- provider malicious/malformed payload;
- broker duplicate/out-of-order/corrected status;
- Web user-intent privilege escalation;
- Communication recipient-response confused deputy;
- cross-App DB/internal service access;
- secret leakage via logs/errors/config;
- stale Foundation/resource/authority state;
- FSARM coordinator epoch rollback/split brain;
- Application resource minimum inflation;
- CSA candidate trying to change protected policy;
- Monitor disabling/tampering;
- dependency downgrade/version confusion;
- payload decompression/size abuse;
- queue exhaustion/retry storm;
- configuration drift/rollback;
- evidence deletion/correction concealment.

## 34. Security/Failure Verification Families

Verifier SHALL prove at minimum:

1. default deny on undeclared route/permission/action;
2. environment authority separation;
3. no secret in source/config/log test fixtures;
4. trust-boundary input validation order;
5. size/depth limits;
6. exact replay/classification rejection;
7. confused-deputy protection;
8. authority failure not retried as transient;
9. ambiguous external outcome reconciled;
10. risk-increasing unknown fails closed;
11. safe risk-reducing degraded path remains governed;
12. stable reason codes;
13. high-consequence evidence completeness;
14. telemetry not treated authoritative;
15. metric cardinality bounded;
16. health != business readiness;
17. required config has owner/type/range/version;
18. protected config drift detected;
19. material config requires governed activation;
20. feature flags cannot create authority/disable safety;
21. FSTSimA no production credentials;
22. APP-RSC no Foundation grant mutation;
23. Monitor/CSA cannot alter protected controls;
24. threat scenarios have negative fixtures.
