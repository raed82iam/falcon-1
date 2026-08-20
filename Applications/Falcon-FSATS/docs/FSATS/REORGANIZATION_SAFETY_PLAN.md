# FSATS Documentation Reorganization Safety Plan

Status: OWNER_AUTHORIZED / PRE-MOVE SAFETY CHECKPOINT

This record protects the current Application documentation before reorganization.

## Owner-approved target structure

```text
applications/docs/FSATS/
├── 03_CURRENT_APPROVED_DESIGN/
│   └── PART_0/
│       ├── P0-A/
│       ├── P0-B/
│       ├── P0-C/
│       ├── P0-D/
│       ├── P0-E/
│       ├── P0-F/
│       ├── P0-G/
│       ├── P0-H/
│       ├── P0-I/
│       ├── P0-J/
│       └── _CROSS_CUTTING/
├── 04_OWNER_DECISIONS/
│   ├── ACCEPTANCE/
│   ├── CHANGES/
│   ├── REOPEN/
│   └── CLOSURE/
├── 05_RED_TEAM_AND_REVIEWS/
│   ├── P0-A/
│   ├── P0-B/
│   ├── P0-C/
│   ├── P0-D/
│   ├── P0-E/
│   ├── P0-F/
│   ├── P0-G/
│   ├── P0-H/
│   ├── P0-I/
│   ├── P0-J/
│   └── _PART_0_CROSS_CUTTING/
└── 06_ARCHIVE/
    ├── SUPERSEDED/
    ├── FAILED_CANDIDATES/
    ├── OLD_BINDINGS/
    └── HISTORICAL_RECORDS/
```

## Non-negotiable safety rules

1. Reorganization is path-only unless a separate documentation fix is explicitly recorded.
2. Existing document bytes must not be rewritten merely to move or classify a file.
3. No unique document may be deleted.
4. Uncertain classification is safer than an invented classification. Any uncertain artifact must remain preserved and be placed in `06_ARCHIVE/HISTORICAL_RECORDS/` until authority evidence resolves it.
5. `CURRENT_APPROVED_DESIGN` must contain only artifacts that can be traced to Owner acceptance/closure and the effective post-change design chain.
6. Owner decisions remain separate from design artifacts.
7. Red-Team/review reports remain separate from accepted design artifacts.
8. Earlier bindings, failed review candidates, and superseded states remain preserved as history.
9. P0-A requires special authority tracing because multiple P0-A versions/acceptance cycles exist.
10. The original `application-development` state must be checkpointed before any bulk move.

## Validation gate

The reorganization is acceptable only when all of the following are true:

- no unique source document is lost;
- every source document is represented exactly once in the final classification or explicitly retained as an unresolved historical artifact;
- current-design paths are distinguishable from historical paths;
- Owner decisions are not mistaken for design content;
- Red-Team reports are not mistaken for accepted design content;
- repository navigation points to the new structure;
- P0-A effective accepted design is established from evidence, not filename guessing.

This record authorizes organization only. It does not authorize architectural redesign, semantic rewriting, status invention, or deletion of historical evidence.
