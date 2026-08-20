# FSATS SIA — Immutable Audit Provenance Graph Specification v1.0

**Package:** `FSATS-SIA-v0.1`
**Status:** `SEMANTIC REMEDIATION / DESIGN CANDIDATE`
**Triggered By:** `AC-EVID-001`
**Purpose:** materialize the Owner-directed Immutable Audit Provenance Graph without creating a hidden FSATS state owner or replacing authoritative business/evidence stores.

## 1. Core Principle

The graph answers:

```text
WHAT evidence/state/decision led to WHAT later evidence/state/action,
under WHICH authority/policy/version,
and through WHICH exact causal/provenance links?
```

It is an immutable provenance/evidence **index and relation model**.

```text
PROVENANCE_GRAPH != AUTHORITATIVE_BUSINESS_STATE
PROVENANCE_GRAPH != FOUNDATION_DECISION_LEDGER
PROVENANCE_GRAPH != AUTHORITY
PROVENANCE_GRAPH != EVENT_TRANSPORT
```

Business owners remain authoritative for their aggregates. Foundation evidence/Decision Ledger semantics remain Foundation-owned where applicable.

## 2. Ownership Model — Federated Application Shards

FSATS grouping owns no mutable graph database.

Each Application owns one local immutable provenance shard for nodes/edges whose `SubjectOwnerApplicationId` is that Application:

```text
APP-TRD -> Trading provenance shard
APP-PMA -> Provider/Data provenance shard
APP-GRD -> Guardian provenance shard
APP-SIM -> Simulation/validation provenance shard
APP-RSC -> resource provenance shard only if APP-RSC is accepted
```

A cross-FSATS graph is a **reconstructed read-only view** over these immutable shards plus referenced Foundation/Owner/external evidence identities.

No Application may edit a foreign shard.

## 3. Node Record

Every graph node:

```text
ProvenanceNodeId
NodeType
SubjectOwnerApplicationId
SubjectId
SubjectVersion/Sequence?
SubjectDigest
EffectiveTimeRef
RecordedTimeRef
AuthorityRef?
Policy/Profile/ModelVersionRefs[]
EvidenceRefs[]
ExternalEvidenceRefs[]
DataClassification
NodePayloadReference
PreviousNodeId?                 // exact same subject lineage where applicable
NodeCanonicalDigest
```

The graph stores minimal canonical indexing metadata and references the authoritative payload/evidence. It SHALL NOT duplicate secrets/full raw market payloads merely for graph completeness.

## 4. Canonical Node ID

```text
CanonicalNodeMaterial =
  UTF8_NFC(NodeType)
  || 0x00 || UTF8_NFC(SubjectOwnerApplicationId)
  || 0x00 || UTF8_NFC(SubjectId)
  || 0x00 || UTF8_NFC(SubjectVersionOrSequenceCanonicalText)
  || 0x00 || SubjectDigestBytes

ProvenanceNodeId = "pnode:" + lowercase_hex(SHA256(CanonicalNodeMaterial))
```

Canonical numeric/version text uses the owning canonical type representation, no locale.

Same material -> same ID. Same semantic identity with different digest/version -> different node; conflict/supersession is represented by edges, not overwrite.

## 5. Required Initial Node Types

### Data / analysis

```text
DATA_PRODUCT_OBSERVATION
DATA_QUALITY_ASSESSMENT
FEATURE_SNAPSHOT
UNIVERSE_SNAPSHOT
MARKET_ALLOCATION_SNAPSHOT
PROVIDER_ROUTE_DECISION
```

### Strategy / decision / capital

```text
STRATEGY_EVALUATION
ENSEMBLE_DECISION
TRADE_PROPOSAL
RISK_DECISION
CAPITAL_COMPETITION
CAPITAL_RESERVATION
CAPITAL_RELEASE
```

### Execution / portfolio

```text
ORDER_INTENT
ORDER_ATTEMPT
BROKER_ORDER_EVENT
FILL_EVENT
POSITION_STATE
PORTFOLIO_CAPITAL_STATE
RECONCILIATION_DECISION
```

### Guardian

```text
GUARDIAN_SIGNAL
GUARDIAN_INCIDENT
PROTECTION_DIRECTIVE
PROTECTION_EFFECT_OUTCOME
PROTECTION_RELEASE
CRISIS_STATE
```

### Resource

```text
RESOURCE_DEMAND_REPORT
RESOURCE_PICTURE
RESOURCE_COORDINATION_PLAN
RESOURCE_DIRECTIVE
RESOURCE_EFFECT_OUTCOME
FOUNDATION_RESOURCE_REQUEST
FOUNDATION_RESOURCE_DECISION_REF
RESOURCE_RESTORATION
```

Resource node types are instantiated only when APP-RSC is accepted/implemented as applicable.

### Simulation / validation / evolution

```text
SIMULATION_RUN_DEFINITION
SIMULATION_RESULT
VALIDATION_ASSESSMENT
CANDIDATE_ARTIFACT
AWARENESS_PROPOSAL
MONITOR_FINDING
INTEGRITY_CHECK
OWNER_DECISION_REF
ACTIVE_ARTIFACT_VERSION
```

### Configuration / authority

```text
CONFIG_ACTIVATION
MANIFEST_VERSION
AUTHORITY_INSTRUMENT_REF
POLICY_VERSION
```

## 6. Edge Record

```text
ProvenanceEdgeId
EdgeType
FromNodeId
ToNodeId
OwnerApplicationId
EdgeEffectiveTimeRef
ReasonCode?
EvidenceRefs[]
EdgeCanonicalDigest
```

An edge points from prerequisite/source/context toward derived/result/action except where the edge semantics explicitly indicate supersession.

## 7. Canonical Edge ID

```text
EdgeMaterial =
  UTF8_NFC(EdgeType)
  ||0x00|| FromNodeId
  ||0x00|| ToNodeId
  ||0x00|| UTF8_NFC(OwnerApplicationId)
  ||0x00|| canonical EdgeEffectiveTimeRef
  ||0x00|| canonical ReasonCodeOrEmpty

ProvenanceEdgeId = "pedge:" + lowercase_hex(SHA256(EdgeMaterial))
```

Same ID/different digest = integrity conflict.

## 8. Initial Edge Types And Exact Meaning

```text
DERIVED_FROM
```
`To` was deterministically/analytically derived using `From` as an input/evidence source.

```text
CAUSED_BY
```
`From` is an accepted causal predecessor whose occurrence/action directly caused the `To` transition/effect according to owning state machine.

```text
AUTHORIZED_BY
```
`To` action/decision depended on the exact authority/policy node/ref represented by `From`. This edge does not imply the authority alone caused the action.

```text
CONSTRAINED_BY
```
`From` Risk/Guardian/resource/market/config state limited the possible `To` outcome.

```text
SELECTED_FROM
```
`To` is a selection/aggregation outcome from candidate/input `From` nodes, e.g. provider route or capital competition.

```text
EXECUTED_AS
```
A business intent/decision `From` materialized into the execution object `To`, e.g. TradeProposal/RiskDecision chain -> OrderIntent.

```text
EFFECT_OF
```
`To` is a confirmed/reconciled effect attributable to action/directive/request `From`.

```text
VALIDATED_BY
```
`From` candidate/run/artifact was assessed by validation node `To`. Validation does not imply adoption.

```text
ADOPTED_BY_DECISION
```
`From` candidate/artifact became eligible/current because of explicit Owner/governance decision node/ref `To` plus separate activation lifecycle. This edge is recorded only when that decision truly exists.

```text
SUPERSEDES
```
`From` is the newer authoritative/corrective node that supersedes older `To`. Older node remains immutable.

```text
CORRECTS
```
`From` is an explicit correction of erroneous/incomplete `To` evidence/state representation; not deletion.

```text
CORRELATES_WITH
```
Non-causal analytical relationship. SHALL NEVER be interpreted as `CAUSED_BY`.

```text
REPLAYS
```
Simulation/replay node `To` reuses the preserved source/evidence represented by `From`; operational authority is not inherited.

```text
REFERENCES_EXTERNAL
```
Application node references an immutable Foundation/Owner/provider/broker evidence identity without importing foreign ownership.

## 9. Edge Direction Examples

```text
DATA_PRODUCT_OBSERVATION -> FEATURE_SNAPSHOT                 DERIVED_FROM
FEATURE_SNAPSHOT -> STRATEGY_EVALUATION                     DERIVED_FROM
STRATEGY_EVALUATION -> ENSEMBLE_DECISION                    SELECTED_FROM
ENSEMBLE_DECISION -> TRADE_PROPOSAL                         DERIVED_FROM
TRADE_PROPOSAL -> RISK_DECISION                             DERIVED_FROM
RISK_DECISION -> CAPITAL_RESERVATION                        AUTHORIZED_BY / CONSTRAINED_BY as applicable
CAPITAL_RESERVATION -> ORDER_INTENT                         EXECUTED_AS
ORDER_ATTEMPT -> BROKER_ORDER_EVENT                         EFFECT_OF only when reconciled causation established
BROKER_ORDER_EVENT -> FILL_EVENT                            CAUSED_BY where broker lineage proves it
FILL_EVENT -> POSITION_STATE                                CAUSED_BY
GUARDIAN_INCIDENT -> PROTECTION_DIRECTIVE                   CAUSED_BY + AUTHORIZED_BY policy/authority nodes
PROTECTION_DIRECTIVE -> PROTECTION_EFFECT_OUTCOME           EFFECT_OF
CANDIDATE_ARTIFACT -> VALIDATION_ASSESSMENT                 VALIDATED_BY
CANDIDATE_ARTIFACT -> OWNER_DECISION_REF                    ADOPTED_BY_DECISION only on actual accepted decision
```

An implementation may record multiple edge types between relevant nodes when each semantic relation is independently true.

## 10. High-Consequence Minimum Graph Closure

Before a high-consequence action can be represented as provenance-complete, the graph must contain or externally reference at minimum:

### Trade order path

```text
required Data Product observations
-> feature snapshot
-> strategy evaluation(s)
-> ensemble/TradeProposal
-> RiskDecision
-> capital reservation/competition record if applicable
-> OrderIntent
-> OrderAttempt
-> broker event/fill
-> position/capital state
```

plus policy/authority/config/model version nodes/refs.

### Guardian

```text
signal/evidence -> incident -> authority/policy -> directive -> target outcome -> release/recovery if later
```

### APP-RSC

```text
constituent reports -> ResourcePicture -> coordination plan -> directives/actions -> confirmed effect -> Foundation request/decision when needed -> restoration
```

### Candidate promotion

```text
candidate -> simulation/validation evidence -> MSA/FSA review refs -> explicit Owner/governance decision -> active-artifact activation record
```

Missing required predecessor => graph completeness state `INCOMPLETE`; the graph SHALL NOT fabricate a link.

## 11. Cross-Application Edges

An Application may create a local edge from its own node to a foreign immutable node/evidence reference only when the cross-App contract carried exact foreign evidence identity/provenance sufficient to bind it.

It SHALL NOT write the foreign node payload into the foreign shard.

For reconstruction:

- if foreign shard/node is available and digest matches, link resolves `RESOLVED`;
- if only immutable external reference exists, link state `EXTERNAL_REFERENCE_RESOLVED`;
- if reference/digest missing/conflicted, link state `UNRESOLVED/CONFLICTED`.

High-consequence provenance requiring a conflicted foreign predecessor is not complete.

## 12. Foundation / Owner References

Foundation/Owner decisions are represented by immutable reference nodes in the local shard only as references to the authoritative source:

```text
SubjectOwnerApplicationId = local Application
NodeType = OWNER_DECISION_REF / AUTHORITY_INSTRUMENT_REF / FOUNDATION_RESOURCE_DECISION_REF
NodePayloadReference = authoritative external identity/commit/evidence reference
SubjectDigest = digest of exact referenced decision/evidence when supplied
```

The Application copy/reference is not the authority source and cannot rewrite it.

## 13. Append-Only / Correction

No graph node/edge is updated in place to conceal prior bytes.

Correction pattern:

```text
OLD NODE remains
NEW corrected node appended
NEW -> OLD edge type CORRECTS or SUPERSEDES
current projection selects latest valid authoritative lineage
```

Deletion is permitted only for separately governed retention of non-required diagnostic material; high-consequence provenance index/evidence retention follows the stricter audit/evidence policy and cannot be erased by the subject AI/business component.

## 14. Graph Shard Persistence

Each Application shard uses append-only tables/log structures:

```text
provenance_nodes
provenance_edges
provenance_external_refs
provenance_checkpoint_roots
```

Primary key = canonical NodeId/EdgeId.

Same key/same digest replay is idempotent.
Same key/different digest = integrity conflict.

Graph append participates atomically with the authoritative state/evidence transaction through outbox/evidence binding where same local transaction is possible. If graph indexing is asynchronous, the authoritative event contains the exact pending provenance material/digest so graph reconstruction is deterministic and missing index material is detectable.

## 15. Merkle Checkpoint Root

At governed checkpoints per Application shard:

1. select all node/edge canonical digests appended since prior checkpoint in deterministic order:
   - Nodes sorted by ProvenanceNodeId ordinal;
   - Edges sorted by ProvenanceEdgeId ordinal;
2. leaf = SHA256(`N` or `E` discriminator byte || canonical digest bytes);
3. build binary Merkle tree; if odd leaf count, duplicate the final leaf at that level;
4. parent = SHA256(0x01 || left || right);
5. empty checkpoint root = SHA256(0x00).

Checkpoint record:

```text
ShardApplicationId
CheckpointSequence
PreviousCheckpointRoot
CoveredFrom/ToSequence
MerkleRoot
CreatedAt
EvidenceRefs
```

The chain makes later omission/mutation detectable. It does not replace authoritative source evidence.

## 16. Required Graph Queries

Implementation must support deterministic queries:

```text
ancestors(node, edge-type filter, max depth)
descendants(node, edge-type filter, max depth)
causal_path(from,to)
why(node)                       // required provenance parents + policy/authority refs
what_changed(old,new)           // supersession/correction lineage
all_effects(action/directive)
all_inputs(decision/proposal)
validation_lineage(candidate)
```

Queries are read-only and bounded/paginated.

## 17. Causal vs Correlation Safety

`CORRELATES_WITH` edges are always excluded from default `causal_path`.

No AI/model may promote a correlation edge to `CAUSED_BY` without separate causal/business/state evidence satisfying the owning domain rule.

Graph UI/reporting must visibly distinguish them.

## 18. Graph Completeness State

For a target node:

```text
COMPLETE
INCOMPLETE
CONFLICTED
EXTERNAL_REFERENCE_PENDING
UNKNOWN
```

Completeness is evaluated against the node-type required predecessor profile/version.

`INCOMPLETE` does not mean the business action did not happen. It means provenance evidence is incomplete and must be reconciled/escalated according to consequence.

## 19. Security / Privacy

The graph stores identifiers/digests/references, not secrets.

Sensitive payload access remains governed by the source evidence store classification/permission.

A graph edge cannot become a side channel to another Application's private payload.

## 20. Awareness / Monitor Usage

MSA/LSA/CSA/Monitor may consume provenance graph projections for:

- explaining outcomes;
- detecting missing/conflicting evidence;
- drift/integrity investigation;
- candidate validation;
- root-cause analysis.

They cannot delete/rewrite authoritative graph history or use graph accessibility to expand business permissions.

## 21. Verification Families

Verifier SHALL cover:

1. exact node/edge ID canonicalization;
2. same ID/different digest conflict;
3. all required initial node/edge types registered;
4. high-consequence trade path closure;
5. Guardian path closure;
6. APP-RSC path closure if accepted;
7. candidate adoption path requires real Owner decision ref;
8. cross-App foreign-shard write denied;
9. external reference digest mismatch conflict;
10. correction/supersession append-only history;
11. causal query excludes CORRELATES_WITH;
12. graph not accepted as business-state authority;
13. graph indexing loss detectable/reconstructable from authoritative evidence;
14. Merkle root deterministic and mutation-sensitive;
15. checkpoint chain tamper detection;
16. secret payload not copied into graph;
17. replay/simulation nodes cannot inherit operational authority through graph edges.

## 22. Finding Disposition

```text
AC-EVID-001 = REMEDIATED_AT_DESIGN_CANDIDATE_LEVEL
IMMUTABLE_AUDIT_PROVENANCE_GRAPH = MATERIALIZED
OWNERSHIP = FEDERATED PER APPLICATION / NO HIDDEN FSATS PRINCIPAL
BUSINESS_STATE_OWNERSHIP = UNCHANGED
CAUSAL_AND_CORRELATION_EDGES = SEPARATE
```
