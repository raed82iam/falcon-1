# FSATS Part 2 — Reopened Red-Team Remediation Evidence

**Status:** `REMEDIATION_MATERIALIZED / EXECUTABLE_VALIDATION_BLOCKED_EXTERNALLY / OWNER_CLOSURE_NOT_ELIGIBLE`  
**Branch:** `application-development`  
**Reopened Remediation Source Candidate:** `83a696b4ee77a63f5b26a41301ebc618e843a4c1`  
**Original Reopened Findings:** `2 Critical / 4 High / 3 Medium`  
**Part 3 Authority:** `NOT_GRANTED / NOT_STARTED`  
**Runtime / Paper / Live Authority:** `NOT_GRANTED`

## 1. Purpose

This record preserves the exact remediation performed after Part 2 was reopened by a later Red-Team review. It does not rewrite or invalidate the earlier historical Part 2 PASS records; those records remain valid only for the exact commits they reviewed.

This record does not claim executable PASS, Owner acceptance, runtime readiness, Paper readiness, Live readiness, or Part 3 authority.

## 2. Reopened Findings and Remediation

### C-01 — CapitalReservationLedger aggregate and concurrency over-reservation

Remediation:
- serialized reservation admission/release/snapshot state;
- aggregate reservation accounting per currency before admission;
- checked decimal overflow fail-closed;
- rejected duplicate reservation identity atomically;
- rejected empty/whitespace ReservationId;
- rejected uninitialized/default currency identity.

Adversarial coverage includes:
- concurrent `8 + 8` requests against available `10`, exactly one may succeed;
- concurrent duplicate ReservationId, exactly one may succeed;
- currency isolation;
- invalid reservation identity rejection;
- uninitialized currency rejection.

### C-02 — Guardian duplicate destructive/protective dispatch race

Remediation:
- introduced per-idempotency serialization before route dispatch;
- duplicate same logical command receives the prior outcome without redispatch;
- changed command semantics under the same idempotency identity fail with `IDEMPOTENCY_CONFLICT`;
- logical command fingerprint intentionally excludes transport-attempt metadata such as message/delivery-attempt/retry/evidence references so a legitimate retry does not become a false semantic conflict;
- caller cancellation is propagated and is not cached as route truth or idempotency outcome.

Adversarial coverage includes concurrent duplicates, transport-attempt retries, semantic conflicts, and cancellation retry independence.

### H-01 — Trading / FSAPMA / Guardian event-ingress atomicity

Remediation:
- duplicate identity check, per-ordering-key sequence decision, ordering update and event recording now occur inside one serialized acceptance state transition in each of Trading, FSAPMA and Guardian.

Adversarial coverage includes:
- 32-way concurrent duplicate consumption;
- two different EventIds racing with the same ordering key and sequence, where exactly one may be accepted.

### H-02 — incomplete CON-023 Manifest declarations

Remediation across all five Applications:
- package provenance, integrity and compatibility policy;
- lifecycle and owned boundary;
- dependencies, Foundation capability/contract declarations;
- provided capabilities and consumers;
- permissions and authority requests;
- security, persistence, communication, configuration and evidence policy;
- lifecycle, health and failure containment;
- self-development, Guardian/protection and rollback;
- explicit Application resource profile with minimum-safe, desired, ceiling/useful-bound, pressure, reclaimability, degraded/shedding and restoration semantics without fabricating Foundation grant numbers;
- explicit Safety Continuity policy;
- explicit AI repair/recovery policy;
- explicit replacement/removal reconciliation policy.

All runtime/egress/binding authority flags remain false where applicable.

### H-03 — mutable Manifest collection backing

Remediation:
- Manifest collection values use read-only wrappers rather than externally exposed arrays.

Adversarial coverage reflects over every `IReadOnlyList<string>` Manifest property and fails if array backing is exposed or mutation succeeds.

### H-04 — Guardian route exception/null/failure truth handling

Remediation:
- non-cancellation route exceptions become attributable `ReconciliationRequired` outcomes;
- null outcomes become `ReconciliationRequired / NULL_ROUTE_OUTCOME`;
- outcome identity mismatch becomes `ReconciliationRequired / ROUTE_OUTCOME_BINDING_MISMATCH`;
- outcome evidence includes logical request fingerprint and evidence reference;
- caller cancellation is preserved as cancellation and does not poison later idempotent attempts.

### M-01 — Awareness candidate identity/evidence/lineage binding

Remediation:
- binds candidate identity, origin, MSA/LSA/CSA identity, candidate digest, evidence identity/digest, lineage, parent identity and parent candidate identity into a SHA-256 binding;
- tampering any bound field with a stale binding fails closed;
- topology parent relationships are verified against the accepted Trading awareness topology;
- exact FSA runtime destination identity is **not fabricated** by the Application workstream. `FSA_LOGICAL_REVIEW_TIER` is preserved as the accepted conceptual review tier while exact Foundation destination/interface binding remains explicitly `PENDING_FCR_0030_EXACT_FOUNDATION_DESTINATION_BINDING`.

### M-02 — stale FSATS root status

Remediation:
- root FSATS README now identifies Part 2 as reopened remediation, preserves earlier PASS records as historical exact-target evidence, and does not claim closure or post-remediation executable PASS.

### M-03 — stale Part 1 / FCR navigation snapshot

Remediation:
- Part 1 remains historically closed;
- stale issue-by-issue current-state snapshot was removed from the closed Part 1 index;
- live GitHub Issue headers are explicitly controlling for current FCR state.

## 3. Owner-Directed Additional Part 2 Remediation

### Per-user / per-account operational failure containment

Materialized semantics preserve:

```text
FAILURE_OF_USER_A != FAILURE_OF_USER_B
USER_FAILURE != APPLICATION_FAILURE
APPLICATION_FAILURE != FSATS_FAILURE
```

Operational failure scope carries principal/account/environment/market/provider/provider-account/broker/broker-account/route/affected positions/orders/data-products/failure/truth/recovery scope.

Policy:
- proven local failure -> minimum necessary scoped containment;
- unknown locality -> automatic containment expansion;
- a proven shared dependency may expand impact to peers;
- a local User A failure does not poison User B merely because both use Falcon.

### Broker outage / human-assisted reconciliation

Materialized semantics preserve:

```text
MARKET_PROVIDER_TRUTH != BROKER_ACCOUNT_TRUTH
USER_REPORTED != BROKER_CONFIRMED
SCREENSHOT_OBSERVED != BROKER_API_CONFIRMED
UNKNOWN_SUBMISSION != SAFE_TO_RETRY
RECONNECTION != RECOVERY
```

Broker observations require attributable user/account/broker/account/evidence/time identity before they can be considered broker-authoritative. Unknown submission outcomes require reconciliation and prohibit blind retry. Guided recovery requests require exact principal/account/broker/position/correlation identity. Final risk-increasing resume requires current broker-confirmed reconciled truth.

No Shared Web UX and no broker connectivity were implemented by this remediation.

## 4. Static Follow-Up Findings Discovered During Remediation

The remediation itself was Red-Teamed iteratively. Additional defects found and corrected before this source freeze include:

1. Guardian caller cancellation could have been converted into cached route failure truth.
2. ReservationId could be empty/whitespace.
3. default/uninitialized Currency could bypass constructor-based currency validation.
4. Guardian idempotency fingerprint initially over-bound transport-attempt metadata and would falsely conflict a legitimate retry.
5. Manifest resource/Safety/AI-recovery/removal declarations were initially too generic relative to current P1-E requirements.
6. Awareness lineage fields were initially adjacent labels rather than cryptographically bound.
7. multi-user peer impact initially depended on caller-supplied blast-radius interpretation rather than enforcing unknown-locality expansion in the policy API itself.
8. MSA-to-FSA modeling risked treating `FSA` as an exact runtime identity despite FCR-0030 being a live Foundation hold.
9. broker recovery evidence identities were initially not fail-closed on missing principal/account/broker/evidence identity.

All nine follow-up defects were remediated in the source candidate identified above and adversarial coverage was added.

## 5. Repository Boundary Verification

Comparison from pre-remediation baseline:

```text
BASE = 2d31a1e025ef7ff6957865c20db112a3f9fd7827
SOURCE CANDIDATE = 83a696b4ee77a63f5b26a41301ebc618e843a4c1
AHEAD = 50 commits
BEHIND = 0
```

All changed files are under `applications/**`.

No changed file is under:
- `applications/shared/web/**`;
- Foundation-owned source/docs outside the Application boundary;
- Part 3.

## 6. Executable Validation Blocker

GitHub Application CI run for the exact source candidate:

```text
RUN = 31844474684
HEAD = 83a696b4ee77a63f5b26a41301ebc618e843a4c1
OWNERSHIP JOB = FAILURE BEFORE JOB START
BUILD / VERIFIER JOB = SKIPPED
```

GitHub check annotation states that the job was not started because recent account payments failed or the spending limit needs to be increased.

Therefore:

```text
CI_FAILURE != SOURCE_CODE_TEST_FAILURE
BUILD_NOT_RUN != BUILD_PASS
VERIFIERS_NOT_RUN != VERIFIER_PASS
```

A local clean-checkout validation could not substitute in the current execution environment because the environment has no usable GitHub network resolution and no installed .NET/PowerShell toolchain for this repository.

## 7. Current Disposition

```text
ORIGINAL REOPENED C/H/M FINDINGS = REMEDIATION MATERIALIZED
OWNER-DIRECTED MULTI-USER / BROKER-OUTAGE GAPS = MATERIALIZED
STATIC FOLLOW-UP DEFECTS = REMEDIATED
APPLICATION WRITE BOUNDARY = PRESERVED
FOUNDATION WRITE = NONE
WEB WRITE = NONE
PART 3 = NOT STARTED

EXACT EXECUTABLE VALIDATION = BLOCKED BY EXTERNAL CI BILLING / RUNNER START CONDITION
FINAL ARCHITECTURE PASS = WITHHELD
FINAL RED-TEAM PASS = WITHHELD
PART 2 OWNER CLOSURE = NOT ELIGIBLE YET
```
