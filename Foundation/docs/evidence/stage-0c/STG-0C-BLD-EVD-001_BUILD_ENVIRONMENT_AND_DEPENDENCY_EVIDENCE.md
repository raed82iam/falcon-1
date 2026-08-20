# Stage 0C Build, Environment, and Dependency Evidence

**Evidence ID:** STG-0C-BLD-EVD-001  
**Version:** 1.0  
**Status:** Satisfied for verification; not an Activation decision  
**Recorded:** 2026-07-27  
**Authority:** GOV-055; GOV-056  
**Bootstrap Source Commit:** `dcc352cac570c8b2231ffa05a8ea381d79241138`

## Environment

| Item | Identity |
|---|---|
| Platform | Local Windows |
| Environment class | Foundation build verification only |
| .NET SDK | 10.0.302 |
| .NET executable SHA-256 | `4377F10C78400F0370B88156773DE9843C07F14E65B3232005EC3179EF38D463` |
| Git | 2.55.0.windows.3 |
| Git executable SHA-256 | `7B7971DD13F0C3A284E538601F2F9770B3A87DFACCB5FB52D68141C67ED22364` |
| Windows PowerShell | 5.1.26100.8875 |
| PowerShell executable SHA-256 | `7600FFE12DA441FE89D035B13801E8E91D064BC544A27B19A5CF49F6AB8B18F5` |

Identity and time remain `BOOTSTRAP_EXTERNAL_ID` and `BOOTSTRAP_EXTERNAL`. No Falcon-native identity or verified Falcon clock was claimed.

## Remediation Result

After STG-0C-STOP-001 and GOV-056:

- CLI home, application data, and package cache were confined to repository `.stage0c`;
- repository `NuGet.Config` cleared all package sources;
- first-run experience, telemetry, and development-certificate generation were disabled;
- restore completed without package acquisition;
- dependency inspection reported no packages;
- no external configuration or network source was used.

## Build Results

| Build | Result |
|---|---|
| Stage 0B candidate baseline | Succeeded; 0 warnings; 0 errors |
| Stage 0C verifier | Succeeded; 0 warnings; 0 errors |

The Stage 0C verifier assembly before cleanup had SHA-256:

`9A0CA1EB289D8669CE9C6D96147E599B824BE968077FB3060029A2F4536EBA33`

## Boundaries

No external package, installation, download, cloud endpoint, financial endpoint, credential, production material, or Falcon operational behavior was used.

Successful build establishes verification evidence only.
