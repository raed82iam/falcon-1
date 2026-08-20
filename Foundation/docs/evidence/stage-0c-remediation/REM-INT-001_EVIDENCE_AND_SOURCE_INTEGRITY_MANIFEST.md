# Stage 0C Remediation Evidence and Source Integrity Manifest

**Manifest ID:** REM-INT-001  
**Version:** 1.0  
**Status:** Recorded  
**Authority:** GOV-058  
**Algorithm:** SHA-256

## Evidence Tree

**Root:** `docs/evidence/stage-0c-remediation`  
**Included Files:** 29  
**Evidence Tree Root SHA-256:** `D925EBE6E6FECE3D2FC8A4971398FB9F9F24F4271B70D7784BE8200B75947A57`

The root was computed by:

1. excluding this Integrity Manifest;
2. recursively enumerating every file;
3. converting each path to a forward-slash relative path;
4. sorting paths ordinally;
5. computing each file SHA-256;
6. encoding each record as `relative-path|UPPERCASE-SHA256` followed by LF;
7. hashing the complete UTF-8 record stream.

## Governed Source and Profile Set

**Included Objects:** 9  
**Governed Source Root SHA-256:** `0836779ED76A69D86FA4668CE90BD65394A53EDB1750CDF565ED8FAEF637BB33`

| Object | SHA-256 |
|---|---|
| `src/Falcon.Foundation.Enabling/FoundationContracts.cs` | `189E1360FD47FCAEE2D18412FA982ECFB4B07B2EB787E13FE431F1EE79479CFA` |
| `src/Falcon.Foundation.Enabling/IdentityTimeAndRandomness.cs` | `DBC9075C5CD530FBC6B4D1FF60AA06D4D33A664CFE56E69FEABB35C2D2C42F4E` |
| `src/Falcon.Foundation.Enabling/SecurityProviders.cs` | `0A0A1450DFAE3D993DED8A7F1D1777E5CB7D609AF09B071F26272B6640A663D0` |
| `src/Falcon.Foundation.Enabling/VerificationPipeline.cs` | `9C0C24E922F757AB9A65D6B565FBB3D1C5F12C7CFCBDCB0C0822FEA90FF14472` |
| `verification/Falcon.Stage0C.RemediationVerifier/Program.cs` | `1A03FF1FC6FBE45F98FFCB5F88C8B9B6D64B0A345C0908508DA25084414CFD95` |
| `foundation/activation/environment-profile.json` | `2183D3DC2F3428A9CF51EE12678468B753BB8BEF3FE2D46EED5D5F14910C15A8` |
| `foundation/activation/build-baseline.json` | `1D68D112924559D97FAA4084839302EA6191DF78F51138BAF90EA569A989284B` |
| `foundation/activation/pipeline-definition.json` | `ED1BD85F92E0155A540A40DBAAA73A6590EC7DF9A1E9A7A9076CD1590F2CF0D0` |
| `foundation/activation/gate-profile.json` | `0D3B5F1CD875B51A665A9B284125761B09AF1FD2DD7F40C148733987749E7055` |

This Manifest does not include its own digest. The preserving repository commit establishes its object identity.

Any correction or change requires a new Manifest version and complete preservation of this record.
