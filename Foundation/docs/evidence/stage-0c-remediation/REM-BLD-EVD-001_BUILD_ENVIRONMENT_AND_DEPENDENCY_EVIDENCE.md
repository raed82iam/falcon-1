# Stage 0C Remediation Build and Environment Evidence

**Evidence ID:** REM-BLD-EVD-001  
**Version:** 1.0  
**Status:** Satisfied  
**Authority:** GOV-058

## Tool Baseline

| Tool | Version | SHA-256 |
|---|---|---|
| .NET SDK | 10.0.302 | `4377F10C78400F0370B88156773DE9843C07F14E65B3232005EC3179EF38D463` |
| Git | 2.55.0.windows.3 | `7B7971DD13F0C3A284E538601F2F9770B3A87DFACCB5FB52D68141C67ED22364` |
| Windows PowerShell | 5.1.26100.8875 | `7600FFE12DA441FE89D035B13801E8E91D064BC544A27B19A5CF49F6AB8B18F5` |
| Platform | Windows NT 10.0.26200.0 | Bootstrap external observation |

## Build

```text
Build succeeded.
0 Warning(s)
0 Error(s)
```

The Foundation Enabling library and remediation verifier target `net10.0`, use deterministic Release settings inherited from `Directory.Build.props`, and use only the .NET Base Class Library.

Dependency inspection reported no packages. Repository `NuGet.Config` has no package source.

## Temporary Artifact Identities

| Artifact | SHA-256 |
|---|---|
| `Falcon.Foundation.Enabling.dll` | `794AC0D238F7A29C406433376FF91F3B5B865F7F54C21A395F695AF146805081` |
| `Falcon.Stage0C.RemediationVerifier.dll` | `B450194B5D272AEF33820A2C7D085B72000CB943AA0C0833518063CA73C71D51` |

The binaries were verification outputs and were removed during cleanup. Source and governed profiles are authoritative.

## Isolation

CLI home, application data, package cache, build outputs, and temporary material remained inside the repository. First-run experience, telemetry, and development-certificate generation were disabled.

No installation, download, external package source, cloud endpoint, or financial endpoint was used.
