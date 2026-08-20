# Stage 1 WP-02 Replay 002 Failed Evidence Inventory

## Classification

`HISTORICAL_FAILED_REPLAY_002_EVIDENCE`

## Directory

`C:\falcon\ExecutionEvidence\Stage1\WP-02-Replay-002`

## Inventory

| Relative path | Kind | Bytes | SHA-256 | Notes |
|---|---|---:|---|---|
| `Session/` | directory | 0 | NOT_APPLICABLE | replay session container |
| `Session/session-identity.json` | file | 623 | `B95A7190EAE0E4795DFB9359033E3CE5C49A5B229171D445D849899621559F7E` | session identity |
| `Runner/` | directory | 0 | NOT_APPLICABLE | runner location |
| `QualificationScratch/` | directory | 0 | NOT_APPLICABLE | replay scratch |
| `QualificationScratch/replay002.ps1` | file | 2952 | `3B6C26D5D3BA45E20B2B16D6BE6DF1B55B6F0A95A0A49A3D0E97D6F4A1A3A7EF` | replay driver script |
| `RawCommands/` | directory | 0 | NOT_APPLICABLE | raw command root |
| `RawCommands/0001/` | directory | 0 | NOT_APPLICABLE | capture-readiness command |
| `RawCommands/0001/command.txt` | file | 70 | present | command record |
| `RawCommands/0001/environment.json` | file | 526 | present | environment record |
| `RawCommands/0001/file-digests.json` | file | 2 | present | file digests record |
| `RawCommands/0001/metadata.json` | file | 457 | present | command metadata |
| `RawCommands/0001/record-manifest.json` | file | 352 | present | record manifest |
| `RawCommands/0001/record-manifest.sha256` | file | 64 | present | manifest digest |
| `RawCommands/0001/stderr.capture.txt` | file | 0 | present | empty stderr capture |
| `RawCommands/0001/stderr.txt` | file | 3 | present | stderr |
| `RawCommands/0001/stdout.txt` | file | 18 | present | stdout |
| `RawCommands/0002/` | directory | 0 | NOT_APPLICABLE | replay script command |
| `RawCommands/0002/command.txt` | file | 127 | present | command record |
| `RawCommands/0002/effects-before.json` | file | 262 | present | effects snapshot |
| `RawCommands/0002/effects-after.json` | file | 262 | present | effects snapshot |
| `RawCommands/0002/effects-difference.json` | file | 841 | present | effects diff |
| `RawCommands/0002/environment.json` | file | 530 | present | environment record |
| `RawCommands/0002/file-digests.json` | file | 2 | present | file digests record |
| `RawCommands/0002/metadata.json` | file | 462 | present | command metadata |
| `RawCommands/0002/record-manifest.json` | file | 378 | present | record manifest |
| `RawCommands/0002/record-manifest.sha256` | file | 64 | present | manifest digest |
| `RawCommands/0002/stderr.capture.txt` | file | 0 | present | empty stderr capture |
| `RawCommands/0002/stderr.txt` | file | 3 | present | stderr |
| `RawCommands/0002/stdout.txt` | file | 1135 | present | stdout |
| `RawCommands/0003/` | directory | 0 | NOT_APPLICABLE | post-replay validation command |
| `RawCommands/0003/command.txt` | file | 127 | present | command record |
| `RawCommands/0003/effects-before.json` | file | 262 | present | effects snapshot |
| `RawCommands/0003/effects-after.json` | file | 262 | present | effects snapshot |
| `RawCommands/0003/effects-difference.json` | file | 841 | present | effects diff |
| `RawCommands/0003/environment.json` | file | 530 | present | environment record |
| `RawCommands/0003/file-digests.json` | file | 2 | present | file digests record |
| `RawCommands/0003/metadata.json` | file | 462 | present | command metadata |
| `RawCommands/0003/record-manifest.json` | file | 378 | present | record manifest |
| `RawCommands/0003/record-manifest.sha256` | file | 64 | present | manifest digest |
| `RawCommands/0003/stderr.capture.txt` | file | 0 | present | empty stderr capture |
| `RawCommands/0003/stderr.txt` | file | 3 | present | stderr |
| `RawCommands/0003/stdout.txt` | file | 1135 | present | stdout |
| `Effects/` | directory | 0 | NOT_APPLICABLE | empty evidence area |
| `PreReplayArtifact/` | directory | 0 | NOT_APPLICABLE | preserved pre-replay artifacts |
| `PreReplayArtifact/Falcon.Foundation.ControlledProjectFoundation.slnx` | file | 287 | `90671013285C9BD4CCD8426192BA955226168927A0142D0EFAA2B5151C638C76` | preserved solution copy |
| `PreReplayArtifact/Falcon.Foundation.Core.csproj` | file | 321 | `2CF3697400723B34CFDC7897BDCF4D509C676F08B7087E4FB35D172A2CB982C1` | preserved Core copy |
| `PreReplayArtifact/Falcon.Foundation.Contracts.csproj` | file | 331 | `0964A12F1ECD02B606EF7DA64915293033F38CD65FC2FF3D2A6ADE913A76784F` | preserved Contracts copy |
| `PreReplayArtifact/Falcon.Foundation.Infrastructure.csproj` | file | 562 | `A86FEEBCCD7E110158E0EE70CCDEA89926A74452EF2ECC3115561141AE452D63` | preserved Infrastructure copy |
| `Validation/` | directory | 0 | NOT_APPLICABLE | validation scratch |

## Conclusion

The directory is historical failed replay evidence: it preserves artifacts and raw records, but the evidence chain is incomplete for a pass because the replay evidence model was bypassed at the repository-editing layer.

