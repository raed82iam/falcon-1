# Broad Red Team — Application Code vs Application Documentation

**Date:** `2026-08-16`  
**Branch:** `application-development`  
**Reviewed source checkpoint:** `eb3a4b2b05db03c1fb4c924689f1da39280f3805`  
**Review type:** Application code ↔ Application documentation ↔ live FCR consistency Red Team  
**Status:** `STATIC_RED_TEAM_FINDINGS_OPEN / EXECUTABLE_VALIDATION_BLOCKED_BY_GITHUB_ACTIONS_BILLING`

## 1. Objective

Attempt to falsify the claim that current Falcon FSATS Application code, current Application documentation, live FCR state and authority ceilings describe the same system state.

The review specifically attacked:

- stale lifecycle/documentary state;
- code metadata drifting from accepted/current documentation;
- accidental runtime/provider/broker/Paper/Live authority;
- cross-workstream ownership leakage;
- restart/recovery authority resurrection;
- Web/Application identity or truth-boundary collapse;
- Foundation/Application authority collapse;
- stale FCR handoff or FCR identity;
- recently remediated FSAPMA quota-pool semantics.

## 2. Governing and current-state sources reviewed

At minimum this pass refreshed and reconciled:

- `applications/README.md`;
- `applications/FSATS/README.md`;
- `applications/FSATS/WORKSTREAM_RULES.md`;
- `applications/FSATS/contracts/runtime-readiness/FSATS.ApplicationRuntimeReadiness.v1.md`;
- Part 8 closure-readiness record `applications/docs/FSATS/PART_8/11_PART8_OWNER_CLOSURE_READINESS.md`;
- current manifests for Trading, FSAPMA, Trading Guardian, FSTSimA and APP-RSC;
- current Trading runtime-readiness evaluator;
- current FSAPMA provider/quota domain source;
- live repository FCR protocol (Issue #1);
- current open FCRs whose header is `Waiting On: APPLICATION`, including Stage 11/12/13 consumer-binding obligations and FCR-0082 hold semantics;
- FCR-0220 and the current quota-pool remediation evidence;
- Issue #203 and its current Web/Application contract handoff.

This is a post-Part-8 technical-state static review. It does not infer Part 8 Owner closure, runtime binding authority, deployment authority, provider/broker connectivity or production activation.

## 3. Positive findings / controls that held

### RT-APP-01 — Runtime authority smuggling

Attack: search current Application manifests/readiness surfaces for implicit runtime activation after Parts 7/8 technical completion.

Result: `BLOCKED`.

All five Application manifests still keep their runtime-authority flags false. Trading external egress remains false, FSAPMA provider egress remains false, Guardian protection route remains unbound, FSTSimA operational egress/Paper authority remain false, and APP-RSC canonical Foundation resource binding remains false.

Trading Part 7 readiness continues to return `GrantsRuntimeAuthority = false`, including its otherwise eligible-for-admission-review state.

### RT-APP-02 — Part 8 technical completion becomes runtime authority

Attack: interpret Part 8 review/learning readiness as deployment, adoption or runtime authority.

Result: `BLOCKED`.

Part 8 closure-readiness explicitly preserves:

```text
GrantsAdoptionAuthority = false
GrantsDeploymentAuthority = false
GrantsRuntimeAuthority = false
```

and separately states that Part 8 does not authorize provider/broker connectivity, runtime binding, Paper/Shadow/Tiny-Live/Live, production deployment or later Parts.

### RT-APP-03 — Broker/customer identity collapse

Attack: locate a current Application manifest/readiness path that requires FSATS customer/user identity as the Trading operating subject.

Result: `BLOCKED` in the reviewed current code surfaces.

Trading remains broker-account scoped and expressly prohibits customer/user identity. The current runtime-readiness evaluator rejects `ContainsCustomerOrUserIdentity`.

### RT-APP-04 — FSAPMA quota multiplication through route/key/account identity

Attack: use different route/account/API-instance/credential-reference identities to manufacture upstream capacity.

Result: `BLOCKED` by current quota-pool remediation.

Current `QuotaLedger` separates `ProviderRouteIdentity` from `ProviderQuotaPoolId`. Unknown scope conservatively collapses under `UNKNOWN_PROVIDER_SCOPE:<PROVIDER>`; explicitly shared pools consume one counter; explicitly proven independent pools may remain independent. Shared Web/FSAPMA half-ceiling behavior is opt-in only for the exact actual constrained shared pool.

### RT-APP-05 — Web shared quota becomes shared authority/data truth

Attack: use a shared provider pool to merge Web provider route/credential/data truth into FSAPMA operational truth.

Result: `NO SUCH AUTHORITY MERGE FOUND` in the reviewed Application source.

Quota-pool identity affects capacity accounting only. Application provider route identity and credential reference remain separate.

### RT-APP-06 — FSTSimA accidental Live/Paper authority

Attack: infer operational or Paper authority from current simulator manifest state.

Result: `BLOCKED`.

`RuntimeAuthorized=false`, `OperationalEgressAuthorized=false`, and `PaperAuthority=false` remain explicit.

### RT-APP-07 — APP-RSC becomes Foundation resource authority

Attack: use APP-RSC manifest semantics to mint Foundation total-resource/grant/ceiling truth.

Result: `BLOCKED` in the reviewed manifest/current boundary.

APP-RSC remains FSATS-only, explicitly prohibits Foundation governance ownership, and keeps canonical Foundation binding unbound.

## 4. Open findings

### RT-APP-DOC-01 — Current workspace READMEs materially lag the actual Part 8 state

**Severity:** `MEDIUM`  
**Class:** Documentary / governance current-state inconsistency

Both current workspace READMEs still claim:

```text
PART 8 THROUGH PART 10 = NOT_AUTHORIZED
```

and later restate:

```text
PART 8 = NOT_AUTHORIZED
```

However the current repository also contains the Part 8 closure-readiness record stating that Part 8 completed its authorized WP-01..WP-06 scope, exact executable validation passed on its governed candidate, post-executable Architecture/Consistency and Broad Red Team passed, final audit passed, and only explicit Project Owner acceptance/closure remained unrecorded.

This is not an over-authorization defect. It is the opposite: the README current-state summaries are stale and no longer describe what has already been authorized and technically completed.

**Impact:** future operators/reviewers/automation can make the wrong source/authority comparison, treat legitimate Part 8 artifacts as unauthorized, or choose the wrong next-state gate.

**Required disposition:** update current-state README summaries to distinguish:

```text
PART 8 = AUTHORIZED_SCOPE_TECHNICALLY_COMPLETE / READY_FOR_OWNER_ACCEPTANCE_AND_CLOSURE
PART 8 OWNER ACCEPTED_AND_CLOSED = NOT_YET_RECORDED
PART 9 / PART 10 = NOT_AUTHORIZED
RUNTIME = NOT_AUTHORIZED
```

Do not rewrite historical Part records.

### RT-APP-CODEDOC-02 — All five `Current` Application manifests still advertise Part 3 package/lifecycle metadata

**Severity:** `MEDIUM`  
**Class:** Code ↔ documentary lifecycle metadata drift

The `Current` manifests for:

- Trading;
- FSAPMA;
- Trading Guardian;
- FSTSimA;
- APP-RSC

all still use:

```text
Version = 0.1.0-part3
LifecycleState = PART3_DURABILITY_IMPLEMENTATION_ONLY_NOT_RUNTIME_ACTIVE
```

while the accepted/current Application codebase has subsequently completed Parts 4, 5, 6 and 7, and Part 8 technical implementation/review/audit readiness.

The safety ceiling is still correct because all runtime/binding authority flags remain false. Therefore this is not classified as a runtime vulnerability.

The concern is identity/provenance/lifecycle truth: these records are named `Current`, their manifest contracts describe exact package/version/provenance and lifecycle information, and current FCR-0226 explicitly relies on exact Application package/version/runtime identity as a future Kill-target binding input. A stale Part-3 lifecycle label can become ambiguous or misleading evidence at a future admission/binding boundary.

**Required disposition:** before modifying the accepted manifests, perform a bounded impact review that decides whether:

1. `Version` / `LifecycleState` are intentionally frozen package-generation metadata from Part 3, in which case that intentional immutability must be documented explicitly and current governed state must live in a separate field/projection; or
2. these are intended to be the current manifest identity/lifecycle values, in which case the five manifests and their adversarial tests/evidence must be updated under a governed semantic amendment.

No silent edit is authorized by this Red Team record.

### RT-FCR-REG-03 — Issue #203 violates the canonical FCR identifier rule

**Severity:** `MEDIUM`  
**Class:** Cross-workstream documentary/governance identity inconsistency

Repository Issue #1 requires:

```text
GitHub Issue #203 -> FCR-0203
```

but Issue #203 is currently titled and historically referenced as:

```text
[FCR-0201] Shared Web - incident affected-position and FSTSimA shadow-monitoring projection semantics
```

The Application `FSATS/README.md` also repeats `FCR-0201`, propagating the non-canonical identifier.

**Application code impact:** none found.

**Runtime impact:** none found.

**Disposition:** Application posted a governance warning directly on Issue #203 during this Red Team. Cross-workstream/Web/FCR governance should reconcile the current canonical identity/title without rewriting historical comments as if they never existed.

### RT-APP-DOC-04 — Current README FCR snapshot is incomplete/stale relative to live handoffs

**Severity:** `MEDIUM`  
**Class:** Documentary operational-state drift

The current READMEs emphasize FCR-0082 and the misidentified `FCR-0201`, but live current Issue headers now include multiple immediate Application obligations after Foundation Stage 11/12/13 completion, including at least:

```text
FCR-0008
FCR-0009
FCR-0011
FCR-0012
FCR-0013
FCR-0014
FCR-0030
FCR-0082
FCR-0226
```

Their meanings differ. Some are separately authorized future runtime-binding verification obligations, while FCR-0082 explicitly remains an Application HOLD. `Waiting On: APPLICATION` itself does not grant implementation authority.

**Impact:** a reader relying on current README state rather than live Issue state may miss a required re-review or incorrectly infer the next Application work queue.

**Required disposition:** README should either maintain a current exhaustive handoff snapshot or explicitly state that the list is non-exhaustive and direct operators to live Issue headers. It must not present a stale subset as current controlling state.

## 5. FCR-specific checks

The Red Team re-read the live FCR protocol and current `Waiting On: APPLICATION` issue headers. No reviewed current FCR grants Application runtime implementation merely because it is waiting on Application.

Preserved holds include:

```text
FCR-0082: APPLICATION HOLD / do not materialize Stage 9 canonical runtime binding without separately authorized scope
FCR-0008/0009/0011/0013/0014: separately authorized final runtime/binding verification pending
FCR-0012/0030: Stage 13 Foundation side complete; Application MSA/FSA consuming binding/verification pending
FCR-0226: Foundation Kill target/containment capability available; exact Application AI runtime binding and governed verification pending, but runtime activation/release not granted by the FCR
```

The code/document findings above do not clear any of those holds.

## 6. Severity summary

```text
CRITICAL = 0
HIGH     = 0
MEDIUM   = 4
LOW      = 0
```

Open Medium findings:

1. stale Part 8 current-state text in `applications/README.md` and `applications/FSATS/README.md`;
2. Part-3 lifecycle/version metadata still exposed by all five `Current` Application manifests;
3. Issue #203 canonical FCR identifier mismatch (`FCR-0201` vs required `FCR-0203`);
4. README current-FCR snapshot materially lags current live Application handoffs.

## 7. Closed-baseline / authority impact

```text
RUNTIME_AUTHORITY_LEAK_FOUND = NO
PROVIDER_CONNECTIVITY_LEAK_FOUND = NO
BROKER_CONNECTIVITY_LEAK_FOUND = NO
PAPER_LIVE_AUTHORITY_LEAK_FOUND = NO
FOUNDATION_WRITE_PERFORMED = NO
WEB_WRITE_PERFORMED = NO
PART8_RUNTIME_AUTHORITY_CHANGED = NO
PART9_OR_PART10_AUTHORIZED = NO
```

README documentary corrections can be handled inside Application ownership, but the five-manifest lifecycle question should not be silently rewritten because it may affect accepted package identity/provenance semantics. Perform impact/Architecture review first.

Issue #203 is cross-workstream governance and is not unilaterally rewritten by Application.

## 8. Executable validation limitation

The current post-quota-remediation GitHub Actions runner is unavailable because GitHub reports an account Billing/Spending-limit condition before the runner starts. Therefore this Red Team does **not** claim a fresh whole-tree executable build/test/verifier PASS at the final documentation HEAD.

Previously recorded exact executable results for accepted Parts remain historical evidence and are not reclassified by this infrastructure condition.

```text
STATIC_CODE_DOCUMENT_RED_TEAM = COMPLETE
STATIC_FINDINGS = 4_MEDIUM
FRESH_FINAL_HEAD_EXECUTABLE_VALIDATION = BLOCKED_BY_GITHUB_ACTIONS_BILLING
```

## 9. Conclusion

The current Application remains fail-closed on runtime authority in the reviewed safety-critical surfaces. The new FSAPMA quota-pool hardening is consistent with current FCR-0220 semantics at static-review level.

The broad code/document comparison is **not clean**, because current-state documentation and current manifest metadata have drifted behind the actual governed Application progression, and the repository contains one canonical FCR identity inconsistency.

These are governance/identity consistency defects rather than confirmed runtime safety violations, but they should be resolved before the next runtime-binding or admission work because those future gates depend on exact current identity, provenance, lifecycle and live FCR truth.
