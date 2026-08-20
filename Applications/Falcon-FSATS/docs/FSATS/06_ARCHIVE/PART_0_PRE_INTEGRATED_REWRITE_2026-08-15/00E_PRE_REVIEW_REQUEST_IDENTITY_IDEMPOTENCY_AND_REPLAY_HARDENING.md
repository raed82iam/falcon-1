# FSATS Market Qualification Candidate — Request Identity, Idempotency and Replay Hardening

**Package:** `FSATS-MARKET-QUALIFICATION-PROPOSAL-001`  
**Applies To:** `00 + 00A + 00B + 00C + 00D`  
**Decision Type:** `PRE-REVIEW REQUEST / COMMAND INTEGRITY HARDENING`  
**Status:** `CONTROLLING CANDIDATE HARDENING / NOT_OWNER_ACCEPTED / NOT_CLOSED`  
**Branch:** `application-development`  
**Implementation / Runtime / Paper / Tiny Live / Live / Deployment Authority:** `NOT_GRANTED`

---

# 1. Purpose

This hardening prevents duplicate, replayed, stale, ambiguous or mutated Owner requests from creating unintended qualification jobs or widening scope.

It also binds the final qualification result to the exact request/candidate/evidence state that produced it.

---

# 2. Canonical Target Market Identity

Before substantive qualification begins, the target market identity must be resolved unambiguously enough for the intended use.

Conceptually the resolved identity may include as applicable:

```text
MarketIdentity
Jurisdiction
Venue / Market Group
AssetClass
InstrumentFamilyScope
Currency / Settlement Context
IntendedUse
```

A natural-language label such as `Saudi market`, `US market`, `crypto`, or another broad/ambiguous name SHALL NOT silently bind to a stronger or different exact market/asset scope.

If material ambiguity cannot be resolved from the governed request/context:

```text
AMBIGUOUS_TARGET_MARKET
-> NO SUBSTANTIVE CANDIDATE ENGINEERING
-> REQUEST EXACT OWNER CLARIFICATION / NARROWING
```

A resolution step may normalize known aliases only when it preserves the Owner's actual intent and remains attributable.

---

# 3. Immutable Qualification Request Identity

Every admitted qualification mandate shall have an immutable attributable identity such as:

```text
QualificationRequestId
RequestVersion
OwnerCommandEvidenceRef
CanonicalTargetMarketFingerprint
RequestedQualificationCeiling
AuthorityScopeFingerprint
CreatedAt
```

The exact future schema is implementation work, but the semantic requirement is mandatory.

A materially changed request is a new request version/identity and SHALL NOT silently inherit prior command authority.

---

# 4. Idempotency / Duplicate Delivery

Repeated delivery of the same exact authenticated/admitted Owner request SHALL NOT automatically create multiple independent qualification jobs.

Conceptual rule:

```text
SAME REQUEST IDENTITY + SAME AUTHORITY/SCOPE FINGERPRINT
-> IDEMPOTENT OBSERVATION / EXISTING JOB REFERENCE
```

unless the Owner explicitly creates a separate independent qualification run.

Duplicate transport delivery is not new authority.

---

# 5. Replay Protection

A stale or replayed historical Owner command SHALL NOT resurrect a completed, rejected, cancelled, expired or superseded qualification mandate.

The future command/job boundary shall preserve sufficient identity/state/evidence to distinguish:

```text
NEW VALID REQUEST
DUPLICATE DELIVERY
AUTHORIZED RETEST / REOPEN
STALE REPLAY
SUPERSEDED REQUEST
CANCELLED REQUEST
EXPIRED REQUEST
```

Only an explicitly valid current authority path may reopen/retest after closure/cancellation when required.

---

# 6. Request Mutation During Qualification

If the Owner materially changes:

- target market;
- asset/instrument scope;
- qualification ceiling;
- cost ceiling;
- capital/exposure model;
- intended use;
- allowed data/provider/broker scope;
- another material authority condition;

then the active job SHALL record a new request version or a separately governed amendment.

Affected evidence must be re-evaluated for staleness.

```text
MATERIAL REQUEST CHANGE
!=
SAME JOB WITH SILENTLY EDITED HISTORY
```

---

# 7. Job Identity and Concurrency

Each active Market Qualification Job shall bind to one exact admitted request identity and current semantic fingerprint.

Multiple jobs for the same market are permitted only when their purpose/scope/authority/evidence separation is explicit and safe.

The system SHALL prevent accidental duplicate jobs from racing to produce conflicting Owner-facing readiness claims.

Conflicting active results must remain attributable and reconciled rather than selecting the more favorable result.

---

# 8. Result Binding

Every terminal Owner-facing qualification result shall bind to at least conceptually:

```text
QualificationRequestId
RequestVersion
CanonicalTargetMarketFingerprint
CandidateVersionSet
EvidencePackageIdentity
ApplicationEvaluationIdentity
ResultState
ResultCreatedAt
EvidenceFreshnessContext
```

Therefore:

```text
RESULT FOR REQUEST A
!=
AUTHORITY / READINESS FOR MUTATED REQUEST B
```

A result shall not be reused for a materially wider market/asset/intended-use scope without requalification.

---

# 9. Owner Stop / Cancel Replay Rule

A valid Owner cancel/stop state has precedence over a delayed duplicate of the earlier start request.

Conceptually:

```text
START REQUEST RECEIVED
-> JOB ACTIVE
-> OWNER CANCEL VALIDLY RECORDED
-> JOB CANCELLED
-> DELAYED DUPLICATE START ARRIVES
-> REMAINS CANCELLED / DUPLICATE REJECTED
```

The delayed message does not recreate authority.

---

# 10. Evidence / Audit Requirement

The future implementation shall preserve enough correlation/causation/audit evidence to reconstruct:

- original Owner wording;
- normalized/admitted request;
- exact target market resolution;
- authority/scope decision;
- duplicate/replay handling;
- amendments/reopens;
- job state transitions;
- candidate/evidence lineage;
- final result;
- subsequent Owner decision.

No historical command/result record is rewritten to conceal a prior scope or state.

---

# 11. Reviewed-Candidate Composition Update

The semantic set requiring a new fresh review is now:

```text
00 + 00A + 00B + 00C + 00D + 00E
```

All earlier Architecture/Consistency reviews remain historical evidence for their exact earlier freezes only.

Fresh Architecture/Consistency and fresh Red-Team must bind to the exact commit containing all six semantic candidate files unchanged.

---

# 12. Non-Grant

This hardening grants no runtime Owner-command path, authentication capability, implementation, research egress, provider/broker connectivity, market admission, Paper, Tiny Live, Live or deployment authority.
