# Stage 1 WP-01 Controlled Replay Command Inventory

## Replay session identity

- replay ID: `WP-01-Replay-001`
- governance authorization: `GOV-070`
- repository root: `C:\Falcon\Falcon1`
- execution identity: `laptop-klg53di4\raeda`
- host name: `laptop-klg53di4`
- PowerShell version: governed local desktop session
- dotnet path: `C:\Program Files\dotnet\dotnet.exe`
- SDK version: `10.0.302`
- isolated NuGet profile: `C:\falcon\ValidationProfile`
- current Git HEAD: `095d800e86823b248468ff9f4fa12e6e44647a35`
- current Git tree: `69db1d39ddb5c3dd74c0e9764d85d909f476975f`
- raw evidence directory: `C:\falcon\ExecutionEvidence\Stage1\WP-01-Replay-001`

## Command inventory

| # | Command | Result class | Notes |
|---:|---|---|---|
| 1 | `rg -n --hidden --glob '!**/.git/**' 'GOV-070|STAGE_1_WP01_CONTROLLED_ROLLBACK_AND_REPLAY_AUTHORIZATION' 'C:\Falcon\Falcon1'` | RETROSPECTIVE_READ_ONLY_VERIFICATION | verified next governance ID was unused |
| 2 | `Get-Date -Format 'yyyy-MM-dd HH:mm:ss zzz'` | RETROSPECTIVE_READ_ONLY_VERIFICATION | replay-session anchor timestamp |
| 3 | `Get-FileHash C:\Falcon\Falcon1\Falcon.Foundation.ControlledProjectFoundation.slnx -Algorithm SHA256` | RETROSPECTIVE_READ_ONLY_VERIFICATION | pre-replay hash |
| 4 | `Copy-Item ...` / `Remove-Item ...` / `Copy-Item ...` replay sequence | CONTROLLED_REPLAY_ACTION | bounded rollback then exact replay |
| 5 | `git -C C:\Falcon\Falcon1 status --porcelain=v2 --branch` | RETROSPECTIVE_READ_ONLY_VERIFICATION | post-replay Git state |
| 6 | `Get-Item C:\Falcon\Falcon1\Falcon.Foundation.ControlledProjectFoundation.slnx` | RETROSPECTIVE_READ_ONLY_VERIFICATION | file size and timestamps |
| 7 | `Get-FileHash C:\Falcon\Falcon1\Falcon.Foundation.ControlledProjectFoundation.slnx -Algorithm SHA256` | RETROSPECTIVE_READ_ONLY_VERIFICATION | post-replay hash |

## Raw evidence note

The external raw evidence directory stores the contemporaneous replay-session
materials and preservation copy. This inventory records the commands and
their classification for the replay evidence chain.

