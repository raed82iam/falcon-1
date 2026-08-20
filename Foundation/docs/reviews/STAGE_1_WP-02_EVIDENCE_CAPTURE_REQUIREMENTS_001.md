# Stage 1 WP-02 Evidence Capture Requirements

## Raw evidence directory

`C:\falcon\ExecutionEvidence\Stage1\WP-02-Execution-001`

## Command capture fields

- ordered command number;
- exact command;
- start timestamp;
- completion timestamp;
- execution identity;
- working directory;
- process-scoped environment;
- exit code;
- complete stdout;
- complete stderr;
- created files;
- modified files;
- deleted files;
- command-record SHA-256.

## Commands expected to be captured during WP-02

| Command class | Expected |
|---|---|
| repository boundary inspection | yes |
| solution/project creation command(s) | yes |
| dependency-graph inspection | yes |
| read-only verification commands | yes |
| any required evidence-generation command | yes |

## Evidence boundary

This review defines the capture requirements only. It does not create the
evidence runner or execute WP-02.

