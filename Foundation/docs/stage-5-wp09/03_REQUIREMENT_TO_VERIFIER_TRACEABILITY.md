# Stage 5 WP-09 — Requirement-to-Verifier Traceability

**Date:** 2026-08-08  
**Status:** TRACEABILITY DEFINED / POST-IMPLEMENTATION HARDENING APPLIED

## Purpose

Define the minimum verifier obligations required before WP-09 may be considered technically complete. This document does not grant closure or later-stage authority.

## Traceability matrix

| ID | Requirement | Verifier obligation |
|---|---|---|
| WP09-R01 | Exact subject identity/version binding | Accept exact identity/version; reject subject substitution, missing identity and ambiguous generation |
| WP09-R02 | Valid lifecycle authority required | Reject missing, stale, revoked, mismatched or scope-insufficient lifecycle authority |
| WP09-R03 | Manifest prerequisite binding | Accept exact governed manifest identity/digest; reject mismatch, missing or stale prerequisite |
| WP09-R04 | Required dependency continuity | Accept satisfied required dependencies; reject unavailable, ambiguous or incompatible required dependency |
| WP09-R05 | Contract/schema compatibility continuity | Reject replacement/upgrade that breaks governed required compatibility |
| WP09-R06 | No silent authority expansion | Reject any candidate whose permissions/authority exceed the exact separately valid authority basis |
| WP09-R07 | No protected-control weakening | Reject security/control-profile weakening not separately authorized |
| WP09-R08 | Deterministic attach eligibility | Same canonical inputs must produce the same attach decision identity |
| WP09-R09 | Upgrade/replacement exact current-to-target binding | Reject target substitution, wrong current generation, ambiguous replacement target, or a target whose governed compatibility/version-policy evidence rejects the progression. Foundation SHALL NOT invent or interpret SemVer ordering itself. |
| WP09-R10 | Replacement history preservation | Verify old/new generations remain distinguishable and historical evidence is not rewritten |
| WP09-R11 | Drain-required semantics | Produce REQUIRE_DRAIN when a governed transition requires draining and completion evidence is absent |
| WP09-R12 | Drain evidence integrity | Reject stale, revoked, invalid or ambiguous drain-completion evidence; missing or valid-but-incomplete evidence may produce REQUIRE_DRAIN where draining is required |
| WP09-R13 | Drain truth is technical only | Positive drain evidence must not claim Application business completion |
| WP09-R14 | Safe detachment eligibility | Allow only when mandatory lifecycle/dependency obligations are satisfied |
| WP09-R15 | Hidden coupling rejection | Reject safe-removal claim when required declarations/evidence expose unresolved hidden coupling |
| WP09-R16 | Removal does not erase accountability | Verify lifecycle history/evidence remains reconstructable after removal decision |
| WP09-R17 | Rollback exact-target binding | Reject ambiguous, invalid, revoked or wrong-generation rollback target |
| WP09-R18 | Rollback cannot recreate revoked authority | Reject rollback whose target depends on authority no longer valid |
| WP09-R19 | Impossible transition rejection | Reject regressive or impossible lifecycle order/state transitions |
| WP09-R20 | Correlation/causation preservation | Preserve canonical correlation/causation identities without creating authority |
| WP09-R21 | Deterministic decision identity | Same canonical request/prerequisites/governance evidence yields same decision identity |
| WP09-R22 | Fail-closed malformed input | Missing mandatory lifecycle fields or malformed enums/identities fail closed |
| WP09-R23 | Package presence is not authority | Verify discovery/package/compatibility facts never create lifecycle or business authority |
| WP09-R24 | Attachment is not runtime activation | Verify ALLOW attach does not produce or imply deployment/runtime activation authority |
| WP09-R25 | Application business opacity | Architecture/static verifier rejects Trading/Risk/strategy/broker/provider/market-specific logic in WP-09 production |
| WP09-R26 | FCR-0011 boundary preserved | Verify lifecycle transition cannot widen a declared non-Live authority profile; no egress enforcement is implemented |
| WP09-R27 | FCR-0012 boundary preserved | Verify lifecycle evidence is generic and no FSA/Owner timer/autonomous-promotion governance is implemented |
| WP09-R28 | Predecessor semantics not redefined | Verify WP-09 consumes predecessor evidence but does not duplicate/redefine WP-03 through WP-08 semantics |
| WP09-R29 | WP-10 boundary preserved | Static verifier rejects integrated Stage 5 closure/deployment orchestration/WP-10 behavior |
| WP09-R30 | Zero-Application validity | Architecture review confirms Foundation remains valid without any concrete Application implementation |

## Scenario families

The dedicated WP-09 verifier SHALL cover positive and negative cases across:

1. attach;
2. upgrade/replacement, including explicit governed version-progression rejection evidence;
3. drain-required and drain-complete paths;
4. safe detach/removal;
5. rollback/recovery direction;
6. stale/revoked/ambiguous evidence;
7. permission/security expansion attempts;
8. deterministic rerun behavior;
9. Application-neutrality and later-WP boundaries.

The implemented verifier currently contains 49 stable named scenarios. Scenario names SHALL remain individually reported.

## Required regression gates

Before Owner review readiness, validation must include:

- clean Release build;
- Architecture tests;
- Security tests;
- Baseline Integrity;
- all accepted Stage 2, Stage 3 and Stage 4 predecessor verifiers;
- Stage 5 WP-01 through WP-08 predecessor verifiers;
- WP-09 verifier execution;
- deterministic WP-09 rerun;
- final HEAD/worktree integrity check.

## Closure rule

Passing these verifier obligations is necessary but not sufficient for WP-09 closure. Independent post-implementation review, FCR/completeness reconciliation and explicit Owner acceptance/closure remain mandatory.

`WP10 = UNAUTHORIZED` remains unchanged.
