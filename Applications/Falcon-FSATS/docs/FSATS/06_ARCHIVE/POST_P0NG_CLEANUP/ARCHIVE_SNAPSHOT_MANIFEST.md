# FSATS Post-P0-NG Cleanup — Archive Snapshot Manifest

**Status:** `HISTORICAL_ARCHIVE / IMMUTABLE SNAPSHOT REFERENCE`  
**Cleanup Date:** `2026-08-09`  
**Active Branch:** `application-development`  
**Archive Snapshot Branch:** `archive/fsats-pre-p0ng-cleanup-20260809`  
**Archive Snapshot Commit:** `6cd48974fcfab6fe0f276828eb75fff2f6fad040`  
**P0-NG Accepted Semantic Freeze:** `c1184c8b8ea42eb9e7ee38484a52bba5ab47f8fb`

## Purpose

This manifest records the exact repository snapshot preserved before the Project Owner-authorized post-P0-NG active-tree cleanup.

The archive branch preserves the full pre-cleanup FSATS tree byte-for-byte. It is the recovery source for any superseded file removed from the active `application-development` branch during this cleanup.

The cleanup is organizational/documentary only. It does not erase Git history and does not grant implementation or runtime authority.

## Owner-Authorized Cleanup Rule

After final P0-NG acceptance and closure, the active FSATS surface must retain only:

- `applications/README.md`;
- `applications/FCR_WORKFLOW.md`;
- `applications/FSATS/README.md`;
- `applications/FSATS/WORKSTREAM_RULES.md`;
- the current accepted P0-NG design under `applications/docs/FSATS/03_CURRENT_APPROVED_DESIGN/PART_0/P0-NG/`;
- historical material under archive.

All other superseded FSATS active-tree material is archival.

## Full Snapshot Preservation

The archive branch/commit preserves, among other things, the exact pre-cleanup state of:

- `applications/FSATS/PART1/**`;
- `applications/FSATS/src/**`;
- `applications/FSATS/verification/**`;
- `applications/FSATS/Falcon.FSATS.Part1.slnx`;
- `applications/docs/FSATS/README.md`;
- `applications/docs/FSATS/01_FALCON_VISION.md`;
- `applications/docs/FSATS/02_FALCON_CONSTITUTION.md`;
- predecessor P0-A through P0-K current-approved directories;
- predecessor `_CROSS_CUTTING` accepted amendments;
- Owner acceptance/change/reopen/closure records;
- Architecture/Consistency/Red-Team review evidence;
- review candidates;
- temporary `applications/docs/FSATS/new ` working material;
- all pre-existing `06_ARCHIVE/**` content.

## Physical Archive Copy Completed for Historical Part 1

Historical Part 1 implementation artifacts were additionally moved byte-for-byte into:

`applications/docs/FSATS/06_ARCHIVE/POST_P0NG_CLEANUP/APPLICATIONS_FSATS_ROOT/`

This includes its solution file, Part 1 records, source, Foundation binding, primitives, application shells, verification projects and verification script.

The active `applications/FSATS/` directory is therefore intended to contain only:

- `README.md`;
- `WORKSTREAM_RULES.md`.

## Active Current Approved Design

The only active current-approved Part 0 design after cleanup is:

`applications/docs/FSATS/03_CURRENT_APPROVED_DESIGN/PART_0/P0-NG/`

Its README binds the exact accepted semantic freeze and final Owner closure.

## Recovery Rule

If a historical byte is ever required for audit, provenance or investigation:

1. read it from `06_ARCHIVE/**` where physically present;
2. otherwise read the exact pre-cleanup path from archive branch `archive/fsats-pre-p0ng-cleanup-20260809` at commit `6cd48974fcfab6fe0f276828eb75fff2f6fad040`;
3. do not restore a historical file to the active design surface without new governed authority.

## Non-Authority

```text
ARCHIVE_PRESERVATION != CURRENT_AUTHORITY
HISTORICAL_OWNER_RECORD != CURRENT_OWNER_DECISION
HISTORICAL_REVIEW_PASS != CURRENT_REVIEW_PASS
ARCHIVED_IMPLEMENTATION != IMPLEMENTATION_AUTHORITY
ARCHIVED_RUNTIME_CODE != RUNTIME_AUTHORITY
```
