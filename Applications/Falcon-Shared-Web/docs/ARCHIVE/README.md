# Shared Falcon Web Documentation Archive

This archive preserves historical Shared Web records that are useful as evidence but are not the current authority for ongoing work.

## Archive rule

A record belongs here when it is clearly one of the following:

- an old page handover;
- an implementation/WP progress checkpoint superseded by later current state;
- an FCR checkpoint whose current lifecycle truth now lives in the GitHub Issue body;
- a historical Red Team snapshot superseded by later remediation/review;
- a reconciliation snapshot superseded by later current plan/runtime state.

Archiving does **not** mean deleting history or declaring the content false. It means the record is historical evidence and must not be mistaken for the current working baseline.

## Historical-link preservation

Some historical documents remain at their legacy root paths even though they are archive-classified. This is intentional where moving them would break old handovers, FCR comments, or evidence references.

Therefore:

```text
PHYSICAL_LOCATION != CURRENT_AUTHORITY
```

The authoritative classification is maintained in:

`../CURRENT/DOCUMENT_CLASSIFICATION_2026-08-19.md`

Three older handovers were physically copied into `HANDOVERS/` and their old paths now contain archive-pointer stubs. The large 2026-08-16 handover remains at its legacy path for reference preservation but is classified historical/archive, not current.

## Categories

- `HANDOVERS/` — old continuation/page handovers.
- `CHECKPOINTS/` — implementation/WP/browser/progress snapshots.
- `FCR_CHECKPOINTS/` — documentary FCR binding/reconciliation snapshots superseded by current Issue bodies.
- `RED_TEAM_HISTORY/` — historical Red Team and remediation snapshots.
- `RECONCILIATIONS/` — historical reconciliation documents superseded by later current state.

Not every archive-classified legacy record must be physically moved if doing so would damage historical references. The classification index is the controlling map.

## Current documents

Do not work from this directory by default.

Start from:

`../README.md`

and:

`../CURRENT/README.md`

The current accepted plan remains:

`../MASTER_WEB_PLAN_V2_2026-08-17/`
