# Stage 0C Remediation Candidate Manifest Catalog

**Catalog ID:** CM-REM-001  
**Version:** 1.0  
**Status:** Issued  
**Authority Chain:** GOV-057 → GOV-058  
**Bootstrap Context:** Local Windows remediation under GOV-058  
**Root Evidence Set:** RVES-STG-0C-REM-001  
**Environment Classification:** `BOOTSTRAP_EXTERNAL_ID` until ACT-ENV-001 is separately activated

Each entry below is an independent `CANDIDATE_MANIFEST`. Catalog containment does not merge subjects or grant group Activation.

| Manifest ID | Subject | Candidate Realization | Canonical SHA-256 |
|---|---|---|---|
| CM-REM-RND-001 | ACT-RND-001 | `WindowsCryptographicRandomnessProvider`; `FALCON-RANDOM-WINDOWS-CSPRNG-1` | `DBC9075C5CD530FBC6B4D1FF60AA06D4D33A664CFE56E69FEABB35C2D2C42F4E` |
| CM-REM-TIM-001 | ACT-TIM-001 | `WindowsFoundationTimeProvider`; `FALCON-TIME-WINDOWS-LOCAL-BUILD-1` | `DBC9075C5CD530FBC6B4D1FF60AA06D4D33A664CFE56E69FEABB35C2D2C42F4E` |
| CM-REM-IDN-001 | ACT-IDN-001 | `FoundationIdentifierProvider`; `FALCON-ID-UUID7-1` | `DBC9075C5CD530FBC6B4D1FF60AA06D4D33A664CFE56E69FEABB35C2D2C42F4E` |
| CM-REM-CRY-001 | ACT-CRY-001 | `FoundationCryptographicAdapter`; `FALCON-CRYPTO-BCL-1` | `0A0A1450DFAE3D993DED8A7F1D1777E5CB7D609AF09B071F26272B6640A663D0` |
| CM-REM-SEC-001 | ACT-SEC-001 | `FoundationSecretProvider`; `FALCON-SECRET-EPHEMERAL-1` | `0A0A1450DFAE3D993DED8A7F1D1777E5CB7D609AF09B071F26272B6640A663D0` |
| CM-REM-CID-001 | ACT-CID-001 | `FoundationCertificateIdentityProvider`; `FALCON-CERT-LOCAL-TRUST-1` | `0A0A1450DFAE3D993DED8A7F1D1777E5CB7D609AF09B071F26272B6640A663D0` |
| CM-REM-ENV-001 | ACT-ENV-001 | `environment-profile.json` | `2183D3DC2F3428A9CF51EE12678468B753BB8BEF3FE2D46EED5D5F14910C15A8` |
| CM-REM-BLD-001 | ACT-BLD-001 | `build-baseline.json` | `1D68D112924559D97FAA4084839302EA6191DF78F51138BAF90EA569A989284B` |
| CM-REM-TRC-001 | ACT-TRC-001 | `REM-TRACE-003.json`; 953 atomic requirements | `C0B201D5EAB2950C87F15D6DD5955AD5BB5EE47366EE5CA426B68C9CE77E7E15` |
| CM-REM-PIPE-001 | ACT-PIPE-001 | `pipeline-definition.json` | `ED1BD85F92E0155A540A40DBAAA73A6590EC7DF9A1E9A7A9076CD1590F2CF0D0` |
| CM-REM-GATE-001 | ACT-GATE-001 | `gate-profile.json` | `0D3B5F1CD875B51A665A9B284125761B09AF1FD2DD7F40C148733987749E7055` |

## Common Candidate Constraints

Every entry:

- has lifecycle `CANDIDATE`;
- is local Foundation verification only;
- depends only on the exact admitted source, Profile, environment, and evidence;
- contains no persistent key, secret, certificate private material, or credential;
- cannot self-activate, self-restore, or expand its authority;
- preserves restriction, revocation, expiry, and replacement;
- declares `NO_OPERATIONAL_AUTHORITY`, `NO_STAGE_1`, `NO_PRODUCTION`, `NO_CLOUD`, and `NO_FINANCIAL_AUTHORITY`.

Repository object identity and the preserving commit establish Catalog integrity.
