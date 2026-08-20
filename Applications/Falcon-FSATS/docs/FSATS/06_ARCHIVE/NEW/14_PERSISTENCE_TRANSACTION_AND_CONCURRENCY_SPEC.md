# FSATS Specialized Implementation Architecture — Persistence, Transaction and Concurrency Specification

**Package:** `FSATS-SIA-v0.1`
**Status:** `DESIGN_CANDIDATE`

## 1. Purpose

Eliminate coding-worker discretion over authoritative state consistency, transaction boundaries, concurrency, event publication, recovery and data ownership.

## 2. Persistence Technology Decision

Candidate default for authoritative Application business state:

```text
PostgreSQL-compatible relational persistence
+ application-owned schemas/databases
+ append-only event/evidence tables where required
+ transactional outbox/inbox
```

This is an Application implementation design choice, not a Foundation requirement. A later technology change must preserve every semantic in this file and requires architecture review if observable behavior changes.

In-memory caches are non-authoritative unless a specific state is explicitly declared ephemeral/reconstructable.

## 3. Application Isolation

Default deployment boundary:

```text
APP-TRD -> own database or dedicated DB role/schema
APP-PMA -> own database or dedicated DB role/schema
APP-GRD -> own database or dedicated DB role/schema
APP-SIM -> own database or dedicated DB role/schema
APP-RSC candidate -> own database or dedicated DB role/schema
```

No Application receives credentials permitting direct read/write of another Application's database.

Cross-Application data is received through contracts and materialized as consumer-owned projections if needed.

## 4. Canonical Aggregate Persistence Pattern

Every authoritative aggregate row contains at least:

```text
AggregateId
AggregateType
AggregateVersion        bigint, monotonic starting at 1
CurrentState
StatePayload / normalized columns as defined by aggregate
LastEventId
UpdatedAtRef            Foundation time/audit reference
IntegrityDigest where required
```

Optimistic update pattern:

```sql
UPDATE ...
SET ..., aggregate_version = expected_version + 1
WHERE aggregate_id = @id
  AND aggregate_version = @expected_version;
```

Affected rows != 1 -> concurrency conflict, never last-write-wins.

## 5. Concurrency Strategies by Aggregate

| Aggregate | Strategy | Partition key |
|---|---|---|
| Trading Account profile/state | single writer + optimistic version | TradingAccountId |
| Universe snapshot | immutable build then publish | UniverseSnapshotId |
| Feature snapshot/cache | immutable/content-addressed | feature/input key |
| TradeProposal decision key | single writer per decision key | DecisionKey |
| RiskDecision | immutable append-only | RiskDecisionId |
| PortfolioState | serialized per TradingAccountId | TradingAccountId |
| CapitalState / reservations | SERIALIZABLE transaction or advisory single-writer per account+asset | TradingAccountId + Asset/Currency |
| OrderChain | single writer / optimistic version | OrderChainId |
| Position | serialized per TradingAccountId+InstrumentId | Account+Instrument |
| Provider registry/profile | single writer per ProviderId | ProviderId |
| Quota bucket | atomic reservation serialized per ProviderAccount+Bucket | ProviderAccountId+QuotaBucketId |
| Provider route state | single writer per ProviderRouteId | RouteId |
| GuardianIncident | single writer per IncidentId | IncidentId |
| ProtectionDirective | immutable identity + append outcome stream | DirectiveId |
| CrisisEpisode | single writer per Guardian scope | CrisisScopeId |
| SimulationRun | single run coordinator | SimulationRunId |
| Simulation Evidence | immutable once frozen | SimulationEvidenceId |
| FSARM ResourcePicture | immutable snapshots | ResourcePictureId |
| FSARM ResourcePlan | immutable plan + serialized effect stream | ResourcePlanId |
| Coordinator epoch | compare-and-set/fenced authoritative record | FSATS coordination scope |
| Awareness candidate | single writer per CandidateArtifactId | CandidateArtifactId |

## 6. Capital Reservation Transaction

The following MUST commit atomically in one authoritative Trading transaction:

1. lock/serialize current `CapitalState` for account + settlement asset;
2. verify expected CapitalState version;
3. verify reservation idempotency key has no conflicting prior reservation;
4. compute current `AvailableUnreserved`;
5. validate requested held amount <= available under exact policy;
6. insert/update `CapitalReservation` to HELD;
7. reduce available/unreserved projection or increase held total;
8. append reservation event;
9. append outbox event for downstream projection/analytics if needed;
10. commit.

Failure at any step rolls back all steps.

`READ_AVAILABLE -> SEND_ORDER -> LATER_RESERVE` is forbidden.

## 7. Order Submission Transaction Boundary

Before network broker dispatch:

One Trading DB transaction SHALL:

1. verify ExecutionIntent expected version/state;
2. verify current proposal/risk/reservation references remain compatible;
3. create durable `OrderAttemptId` + broker client idempotency key;
4. transition intent to SUBMITTING;
5. append execution event/evidence;
6. commit.

Only after commit may the external broker adapter be called.

This guarantees crash recovery knows whether an attempt may have been sent.

After broker response, a new transaction reconciles the result. Network call is never inside a DB transaction lock.

## 8. Ambiguous Broker Recovery

On process crash/timeout after durable SUBMITTING state:

```text
restart
-> load SUBMITTING/AMBIGUOUS attempts
-> query broker by durable client/broker identity
-> reconcile
-> only resubmit if broker semantics and evidence prove no prior accepted order
```

No automatic "retry on exception" middleware may bypass this state-specific logic.

## 9. Fill and Capital Atomicity

A reconciled fill affects:

- OrderChain fill ledger/state;
- Position ledger/state;
- capital reservation consumption/release;
- cash/asset capital state;
- portfolio/exposure projection;
- outbox events.

These effects belong to a Trading consistency boundary.

Preferred candidate: one Trading database transaction with deterministic lock order:

```text
CapitalState(account, asset sorted canonical)
-> Position(account, instrument)
-> OrderChain
-> CapitalReservation
-> append ledgers/outbox
```

This avoids distributed transactions between T-LSA-08 and T-LSA-09 while preserving logical LSA ownership through ports/modules.

If future storage separation makes this impossible, the replacement design must provide equivalent invariant protection with explicit saga/reconciliation and requires new architecture review.

## 10. Deterministic Lock Ordering

Whenever one transaction must lock multiple aggregate keys:

1. sort by aggregate class fixed order;
2. within class sort canonical identifier bytes/string ordinal;
3. acquire locks only in that order.

No code may dynamically lock arbitrary peer order based on call sequence.

## 11. Transactional Outbox

Every authoritative state transition that must publish an internal/cross-App event writes an outbox row in the same transaction.

Outbox fields:

```text
OutboxId
ApplicationId
AggregateId/Version
Event/ContractFamilyId
BusinessSchemaVersion
Payload
PayloadDigest
Correlation/Causation refs
CreatedAtRef
PublishState = PENDING | IN_FLIGHT | PUBLISHED | DEAD_LETTER
AttemptCount
LastOutcomeRef
```

Publisher uses Foundation delivery semantics. `PUBLISHED` means accepted publication/delivery boundary as defined by Foundation contract, not consumer business effect.

## 12. Inbox / Consumer Idempotency

Cross-Application consumer persists inbox deduplication before or atomically with business effect where exactly-once business effect is required.

Inbox key includes:

```text
ProducerApplicationId
ContractFamilyId
FoundationMessage/EventIdentity
BusinessPayloadDigest
```

Same identity + same digest -> idempotent duplicate.
Same identity + different digest -> integrity conflict; do not process.

Inbox retention must be at least the maximum replay/idempotency horizon for the contract plus safety margin, or permanently retained where the contract identity may be replayed historically into a protected environment.

## 13. Event / Evidence Ledgers

Append-only ledgers are required for:

- Risk decisions;
- capital reservations/consumption/release;
- broker order attempts/fills/corrections;
- Guardian incidents/directives/outcomes;
- FSARM resource plans/actions/outcomes;
- awareness candidates/reviews;
- simulation frozen evidence references;
- configuration/policy activation history where material.

Correction creates new row/event with relation to corrected item. DELETE/UPDATE of historical event payload is forbidden except narrowly governed data-retention/privacy redaction that preserves immutable evidence identity and audit proof.

## 14. Snapshot Reconstruction

For event-backed aggregates, periodic snapshots may accelerate load but SHALL contain:

```text
AggregateId
AggregateVersion
LastAppliedEventId/Sequence
SnapshotPayload
SnapshotDigest
SchemaVersion
```

On load verifier may reconstruct from event stream and compare digest/state for sampled/high-consequence aggregates.

Snapshot mismatch -> `RECONCILIATION_REQUIRED`; never silently trust snapshot.

## 15. Provider Quota Reservation Transaction

Atomic quota reservation per bucket:

```text
load current quota bucket with row lock/version
apply deterministic refill/window roll
subtract active reservations/consumed units
if requested units <= available:
  create reservation
  persist new bucket state/event/outbox
else:
  reject with QUOTA_INSUFFICIENT
commit
```

Network provider dispatch occurs only after quota reservation commit.

A provider-reported authoritative quota reset/correction creates a new quota evidence update; local time does not fabricate refill where provider semantics require server evidence.

## 16. Provider Data Persistence

FSAPMA stores:

- raw payload only where required for audit/reconciliation and retention policy permits;
- normalized immutable observations;
- source/provenance mappings;
- quality/reconciliation results;
- provider route/quota/reliability state.

Raw payload storage is quarantined/untrusted and never queried by Trading directly.

Normalized observation identity/digest is immutable. Correction creates successor.

## 17. Guardian Persistence

Incident and directive state use event + current projection.

Directive publication transaction:

1. validate incident/authority/scope;
2. insert immutable directive;
3. append incident relationship event;
4. write outbox protection command;
5. commit;
6. publish asynchronously.

Guardian cannot mark target effect confirmed inside the publish transaction.

## 18. FSTSimA Persistence

Simulation control DB stores run definitions/lifecycle/checkpoints/evidence indexes.

Large datasets/results may use object/blob storage behind content-addressed digests; DB stores authoritative metadata/digest/provenance.

Frozen evidence artifact bytes are immutable/content-addressed.

A run checkpoint is valid only when:

- component states included by declared checkpoint schema;
- random stream states captured;
- scheduler queue state captured;
- model/version/input identities captured;
- checkpoint digest verified.

## 19. FSARM Persistence

APP-RSC candidate stores:

- immutable ResourcePictureSnapshot;
- immutable CoordinationEnvelope projections/refs;
- Application resource reports;
- ResourcePlan aggregate/action stream;
- target outcomes/effect confirmations;
- Foundation request/outcome refs;
- CoordinatorEpoch/fence state;
- restoration/evidence ledger.

### Coordinator epoch update

Candidate safe rule:

```text
CAS current_epoch -> current_epoch + 1
with exact coordinator instance identity and lease/activation evidence
```

If no approved distributed lease/activation primitive exists, only one configured active instance is permitted; multi-instance leader failover remains disabled rather than improvised.

## 20. Optimistic Concurrency Failure Handling

On version conflict:

- do not retry blindly for high-consequence command;
- reload current aggregate;
- re-evaluate command preconditions against new state;
- if command is still semantically valid and idempotency identity permits, create a new deterministic attempt;
- otherwise return `CONCURRENCY_PRECONDITION_CHANGED` or relevant rejection.

Analytics/read projection updates may use bounded automatic retry when recomputation from immutable inputs is safe.

## 21. Isolation Levels

Candidate defaults:

- ordinary aggregate single-row transition: `READ COMMITTED` + optimistic version or explicit row lock;
- capital/quota allocation across competing reservations: `SERIALIZABLE` or equivalent deterministic single-writer serialization;
- immutable snapshot build: transactionally consistent snapshot/read boundary appropriate to source DB;
- analytics: snapshot/read replica allowed only where freshness/consistency requirement declares it.

No use of `READ UNCOMMITTED` for authoritative financial/protection/resource decisions.

## 22. Schema Migration

Every persistent schema migration declares:

```text
MigrationId
FromSchemaVersion
ToSchemaVersion
CompatibleAppVersionRange
ForwardTransformation
Rollback/CorrectivePlan
DataValidationQueries/Verifier
ExpectedRow/Invariant effects
Downtime/online behavior
Evidence digest
```

Breaking migration requires staged Application lifecycle/update governance.

Migrations do not rewrite historical semantic meaning. If a field meaning changes, use a new field/version/mapping rather than reinterpret old bytes.

## 23. Backup / Recovery

Application recovery requires:

- point-in-time recoverable authoritative database or equivalent;
- content-addressed frozen evidence backup;
- verified restore procedure;
- post-restore event/snapshot consistency validation;
- cross-App inbox/outbox reconciliation;
- broker/provider/external authoritative reconciliation before resuming risk-increasing actions;
- stale credentials/authority/resource state revalidation.

Restored DB timestamp does not imply current market/account/authority truth.

## 24. Retention

Candidate policy classes:

```text
R1_IMMUTABLE_GOVERNANCE_EVIDENCE       long-term governed retention
R2_FINANCIAL_EXECUTION_LEDGER          regulatory/business retention policy
R3_PROTECTION_INCIDENT_EVIDENCE        long-term incident retention
R4_MODEL_EXPERIMENT_EVIDENCE           until superseded + defined audit horizon
R5_RAW_PROVIDER_PAYLOAD                bounded by need/cost/privacy/license
R6_HIGH_VOLUME_NORMALIZED_MARKET_DATA  data-tier/archive policy
R7_TRANSIENT_CACHE                     reconstructable, short TTL
```

Exact durations are configuration/legal-policy values, not invented by code. Absence of approved duration means do not destructively purge authoritative evidence.

## 25. Persistence Failure Taxonomy

```text
PERSISTENCE_UNAVAILABLE
PERSISTENCE_TIMEOUT
PERSISTENCE_CONCURRENCY_CONFLICT
PERSISTENCE_INTEGRITY_CONFLICT
PERSISTENCE_SCHEMA_INCOMPATIBLE
PERSISTENCE_EVENT_SEQUENCE_GAP
PERSISTENCE_SNAPSHOT_MISMATCH
PERSISTENCE_OUTBOX_STALLED
PERSISTENCE_INBOX_DIGEST_CONFLICT
PERSISTENCE_RECOVERY_REQUIRED
```

Risk-increasing actions fail closed on authoritative persistence unavailability/integrity conflict.

## 26. Verification Families

Verifier/tests SHALL cover:

1. no cross-App DB credentials/access;
2. optimistic version conflicts;
3. capital double-reservation race;
4. quota oversubscription race;
5. order dispatch crash-before/after commit;
6. ambiguous broker recovery;
7. duplicate fill idempotency;
8. fill/capital/position atomicity;
9. deterministic lock order/deadlock stress;
10. outbox state vs business state atomicity;
11. inbox same-ID/different-digest conflict;
12. event history immutability;
13. snapshot reconstruction/digest mismatch;
14. Guardian directive publication vs effect separation;
15. FSTSimA checkpoint completeness/reproducibility;
16. FSARM epoch fencing/persistence;
17. migration forward/rollback validation;
18. restore then external reconciliation before new risk;
19. cache loss cannot lose authoritative truth;
20. no binary floating financial persistence conversion.
