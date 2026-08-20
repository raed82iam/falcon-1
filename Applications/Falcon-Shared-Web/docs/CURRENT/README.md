# Shared Falcon Web - Current Working Documentation

This directory is the Owner-facing pointer to current Shared Web documentation. It does not duplicate the canonical documents; it tells you exactly which records are current.

## Start here

Full current-vs-historical classification:

`DOCUMENT_CLASSIFICATION_2026-08-19.md`

## Current accepted plan

`../MASTER_WEB_PLAN_V2_2026-08-17/`

Owner acceptance/amendment:

`../MASTER_WEB_PLAN_V2_2026-08-17/08_OWNER_ACCEPTANCE_AND_FINAL_PLANNING_AMENDMENT_2026-08-17.md`

## Current architecture and implementation references

- `../IMPLEMENTATION_ARCHITECTURE.md`
- `../IMPLEMENTATION_BEST_IN_CLASS_RULE.md`
- `../DEPLOYMENT_PORTABILITY.md`

## Current manuals

Manual index:

`../manual/README.md`

Arabic and English manuals are maintained for:

- Standard User
- VIP User
- Project Owner
- Programmer / Maintainer

## Current runtime / plug-ready references

- `../PRETEST_RUNTIME_COMPLETION_CHECKPOINT_2026-08-19.md`
- `../PROVIDER_BINDING_PROFILE_CHECKPOINT_2026-08-18.md`
- `WEB_FOUNDATION_FULL_PLUG_READY_PREPARATION_2026-08-19.md`

## Active Owner decisions/directions

- `../OWNER_DECISIONS_INCIDENT_CONVERSATION_2026-08-16.md`
- `../OWNER_DECISION_PROVIDER_QUOTA_SPLIT_2026-08-16.md`
- `../OWNER_DECISION_SHARED_VISUAL_ASSET_2026-08-17.md`
- `../OWNER_DIRECTION_STANDING_AUTO_ACCEPT_AND_ROLLBACK_2026-08-18.md`
- `../OWNER_DIRECTION_WEB_MSA_LSA_2026-08-17.md`
- `../TRADING_UI_BENCHMARK_AND_DESIGN_DIRECTION_2026-08-16.md`
- `../VOICE_IMPLEMENTATION_LOCAL_FREE_2026-08-16.md`

## Current operational truth

Current FCR lifecycle state lives in the GitHub Issue body, not in old FCR checkpoint files.

Last exact executable plug-ready Web baseline before documentation-only manual/index commits:

`38c5db80adc52e6555ebe8aee821d83659c513d3`

Verified evidence on that executable candidate:

```text
npm test = PASS 479/479
npm run check = PASS
WORKTREE = CLEAN
FULL_PLUG_READY_CONTRACT_PREFLIGHT = VERIFIED
FULL_PLUG_READY_PREFLIGHT = VERIFIED_BY_COMPOSITION
RED_TEAM = PASS
ARCHITECTURE_AUDIT = PASS
SECURITY_FAIL_CLOSED_AUDIT = PASS
CRITICAL/HIGH/MEDIUM/LOW = 0/0/0/0
```

Current repository HEAD may be newer because manuals/documentation are being added. Those documentation-only commits do not execute the actual Foundation link.

Actual Admission, canonical Runtime Registration, activation, deployment, provider connectivity, production use, business authority, and trading authority remain unexecuted unless separately authorized and performed.
