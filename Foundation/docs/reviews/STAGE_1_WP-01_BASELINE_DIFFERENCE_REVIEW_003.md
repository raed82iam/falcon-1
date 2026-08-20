# Stage 1 WP-01 Baseline Difference Review 003

## Difference classification

| File / area | Classification | Notes |
|---|---|---|
| `Falcon.Foundation.ControlledProjectFoundation.slnx` | `WP01_CONTROLLED_REPLAY_ARTIFACT` | controlled replay artifact |
| `docs/governance/GOV-070_STAGE_1_WP01_CONTROLLED_ROLLBACK_AND_REPLAY_AUTHORIZATION.md` | `WP01_CONTROLLED_REPLAY_GOVERNANCE` | controlled replay authorization |
| `docs/reviews/STAGE_1_WP-01_PRE_REPLAY_ARTIFACT_IDENTITY_001.md` | `WP01_CONTROLLED_REPLAY_EVIDENCE` | pre-replay identity record |
| `docs/reviews/STAGE_1_WP-01_CONTROLLED_REPLAY_*` | `WP01_CONTROLLED_REPLAY_EVIDENCE` | replay evidence chain |
| pre-existing modified and untracked repository files shown by Git | `PRE_EXISTING_GOVERNED_DIRTY_CONTENT` | not modified by the replay |

## Unexplained changes

`0`

## Summary

The replay maintained the exact canonical artifact and introduced only the
authorized replay evidence chain.

