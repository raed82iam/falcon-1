# Stage 3 WP-05 Failure, Stop, Recovery, and Rollback

## Immediate stop conditions

Stop on baseline drift, branch drift, staged content, unauthorized changed paths, payload hash mismatch, build warning or error, regression failure, nondeterministic output, failed `git fsck`, or any reproduced independent-review finding.

## Failure behavior

- malformed requests fail closed;
- every supplied non-empty identity remains consumed after rejection;
- rejected transitions do not emit success events;
- invalid or expired authority, time, dependency, release, recovery, or bootstrap evidence fails closed;
- an active restriction blocks non-protective states;
- self-release is impossible through the lifecycle request surface.

## Recovery behavior

A restricted subject may move through protective stop to `STOPPED`. Recovery requires:

1. an allowed `STOPPED → RECOVERING` model transition;
2. an effective release record bound to the active restriction;
3. a new accepted lifecycle authority decision;
4. an effective admitted time record;
5. an exact canonical evidence-bundle digest;
6. independent recovery validation before `RECOVERING → READY`.

## Rollback

Until commit authorization exists, rollback means restoring only the GOV-097 payload paths from the pre-application checkpoint. The frozen post-WP-04 baseline, tag, and closed WP-01 through WP-04 implementations must not be reset, rewritten, or reopened.
