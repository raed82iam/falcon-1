# Stage 1 WP-02 Failed Replay Evidence Directory Inventory

## Classification

`HISTORICAL_FAILED_REPLAY_EVIDENCE`

## Directory

`C:\falcon\ExecutionEvidence\Stage1\WP-02-Replay-001`

## Inventory

| Relative path | Bytes | Created/Last write | SHA-256 | Format | Purpose | Command record | Mandatory fields |
|---|---:|---|---|---|---|---|---|
| `probe.txt` | 7 | writable probe marker | not required for classification | text | writability probe | no | no |
| `replay-summary.json` | present | summary metadata | not required for classification | JSON | replay summary | no | no |
| `PreReplayArtifact/Falcon.Foundation.Contracts.csproj` | present | preserved artifact copy | present in directory | XML | preserved artifact copy | no | no |
| `PreReplayArtifact/Falcon.Foundation.ControlledProjectFoundation.slnx` | present | preserved artifact copy | present in directory | SLNX/XML | preserved artifact copy | no | no |
| `PreReplayArtifact/Falcon.Foundation.Core.csproj` | present | preserved artifact copy | present in directory | XML | preserved artifact copy | no | no |
| `PreReplayArtifact/Falcon.Foundation.Infrastructure.csproj` | present | preserved artifact copy | present in directory | XML | preserved artifact copy | no | no |

## Conclusion

The directory exists, is writable, and contains preserved artifact copies and summary material, but it does not contain contemporaneous raw command records. It is therefore historical failed replay evidence, not a complete original execution record.

