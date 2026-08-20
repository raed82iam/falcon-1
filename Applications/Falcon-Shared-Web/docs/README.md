# Shared Falcon Web Documentation Index

**Branch:** `web-development`  
**Scope:** `applications/shared/web/**`  
**Owner-facing purpose:** show immediately what is current, what is historical, where the manuals are, and where to continue work.

## CURRENT STATUS

```text
CURRENT_APPROVED_PLAN = MASTER_WEB_PLAN_V2_2026-08-17
LAST_EXECUTABLE_PLUG_READY_BASELINE = 38c5db80adc52e6555ebe8aee821d83659c513d3
NODE_TESTS = PASS_479_OF_479
NPM_CHECK = PASS
FULL_PLUG_READY_CONTRACT_PREFLIGHT = VERIFIED
FULL_PLUG_READY_PREFLIGHT = VERIFIED_BY_COMPOSITION
FOUNDATION_CHANGE_REQUIRED = FALSE
RUNTIME_CURRENT_VALUES = BIND_AT_OPERATION
ACTUAL_FOUNDATION_LINK = NOT_EXECUTED
```

The repository HEAD may move after this point for documentation-only commits. Documentation-only movement does not invalidate the executable source result when no executable source changed, but any material admission-critical source change must trigger the governed re-review defined by FCR-0253.

## 1. CURRENT CANONICAL PLAN

The accepted plan currently governing Shared Web work is:

`MASTER_WEB_PLAN_V2_2026-08-17/`

Owner acceptance is recorded in:

`MASTER_WEB_PLAN_V2_2026-08-17/08_OWNER_ACCEPTANCE_AND_FINAL_PLANNING_AMENDMENT_2026-08-17.md`

This is the current planning baseline. Older implementation plans, handovers and checkpoints do not supersede it.

## 2. CURRENT WORKING REFERENCES

These records remain active references unless explicitly superseded by a later Owner decision, current FCR state, or the accepted Master Plan V2:

- `IMPLEMENTATION_ARCHITECTURE.md`
- `IMPLEMENTATION_BEST_IN_CLASS_RULE.md`
- `DEPLOYMENT_PORTABILITY.md`
- `PRETEST_RUNTIME_COMPLETION_CHECKPOINT_2026-08-19.md`
- `PROVIDER_BINDING_PROFILE_CHECKPOINT_2026-08-18.md`
- `OWNER_DECISIONS_INCIDENT_CONVERSATION_2026-08-16.md`
- `OWNER_DECISION_PROVIDER_QUOTA_SPLIT_2026-08-16.md`
- `OWNER_DECISION_SHARED_VISUAL_ASSET_2026-08-17.md`
- `OWNER_DIRECTION_STANDING_AUTO_ACCEPT_AND_ROLLBACK_2026-08-18.md`
- `OWNER_DIRECTION_WEB_MSA_LSA_2026-08-17.md`
- `TRADING_UI_BENCHMARK_AND_DESIGN_DIRECTION_2026-08-16.md`
- `VOICE_IMPLEMENTATION_LOCAL_FREE_2026-08-16.md`

A short Owner-facing current map is maintained at:

`CURRENT/README.md`

## 3. MANUALS

Current operating and engineering manuals live under:

`manual/`

Manual index:

`manual/README.md`

It contains Arabic and English manuals for:

- Standard User
- VIP User
- Project Owner
- Programmer / Maintainer

Manuals explain current behavior; they do not create runtime, deployment, business, trading, or cross-workstream authority.

## 4. ARCHIVE

Historical records live under:

`ARCHIVE/`

Archive means preserved evidence/history, not current authority. Historical records are not deleted merely because they are archived.

Archive categories:

- `ARCHIVE/HANDOVERS/`
- `ARCHIVE/CHECKPOINTS/`
- `ARCHIVE/FCR_CHECKPOINTS/`
- `ARCHIVE/RED_TEAM_HISTORY/`
- `ARCHIVE/RECONCILIATIONS/`

See `ARCHIVE/README.md` for the classification rule.

## 5. IDEAS

Ideas remain non-authoritative until incorporated into a governed current plan or explicit Owner decision.

Current Ideas folder:

` Ideas/`

The leading space in this historical directory name is preserved for now to avoid an unnecessary rename during documentary cleanup.

## 6. FCR CURRENT STATE

GitHub Issue bodies are the canonical current state for open FCRs. FCR checkpoint documents in this repository are historical evidence once a newer canonical Issue body exists.

Repository-level FCR protocol: GitHub Issue #1.

Permitted `Waiting On` values are only:

```text
FOUNDATION
APPLICATION
WEB
NONE
```

`Waiting On: OWNER` is not permitted.

## 7. AUTHORITY ORDER

When records conflict, do not choose by filename date alone. Apply current Falcon authority and explicit supersession. At minimum:

1. Falcon Vision / Falcon Constitution / governing cross-workstream authority.
2. Explicit current Owner decisions.
3. Current canonical FCR Issue body for FCR lifecycle state.
4. Accepted Shared Web Master Plan V2 and incorporated amendments.
5. Current architecture/runtime reference documents.
6. Current manuals for usage and maintenance explanation.
7. Historical archive records for evidence only.

## 8. WORKING RULE

For normal continuation, start here:

1. `README.md`
2. `CURRENT/README.md`
3. `MASTER_WEB_PLAN_V2_2026-08-17/`
4. `manual/README.md` when you need operating or programming instructions
5. current open FCR Issue bodies
6. only consult `ARCHIVE/` when historical evidence is needed

Do not infer implementation, runtime, deployment, production, business, or trading authority from documentation placement alone.
