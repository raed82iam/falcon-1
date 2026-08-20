# Stage 0B Build, Tool, Dependency, and Artifact Evidence

**Evidence ID:** STG-0B-BLD-EVD-001  
**Recorded Date:** 2026-07-26  
**Authority:** GOV-051; GOV-052  
**Source Commit:** `f250ec5c06602042204bb307b3915ebe0176c165`  
**Status:** Satisfied

## Build Result

```text
Build succeeded.
0 Warning(s)
0 Error(s)
```

Both projects targeted `net10.0` and were built in `Release` configuration.

## Tool Identity

| Tool | Version | Executable SHA-256 |
|---|---|---|
| .NET SDK | 10.0.302 | `4377F10C78400F0370B88156773DE9843C07F14E65B3232005EC3179EF38D463` |
| Git | 2.55.0.windows.3 | `7B7971DD13F0C3A284E538601F2F9770B3A87DFACCB5FB52D68141C67ED22364` |
| Windows PowerShell | 5.1.26100.8875 | `7600FFE12DA441FE89D035B13801E8E91D064BC544A27B19A5CF49F6AB8B18F5` |

## Dependency Finding

The dependency inspection reported:

```text
Falcon.Stage0B.Candidates [net10.0]: No packages were found.
Falcon.Stage0B.Verifier [net10.0]: No packages were found.
```

- Package sources were cleared by repository `NuGet.Config`.
- No package, workload, tool, SDK, extension, or container was downloaded or installed.
- The implementation uses only the admitted .NET SDK and Base Class Library.

## Candidate Artifact Identity Before Cleanup

| Candidate Artifact | SHA-256 |
|---|---|
| `Falcon.Stage0B.Candidates.dll` | `B5DEB0D54B6801841A76D5C744668A2A493BB765C8520A085182660072638E28` |
| `Falcon.Stage0B.Verifier.dll` | `1AF3C9B727101BF49EAB8216B1AA1B6EFE9B516450A31FDF82BAF61591FBCCED` |
| `STG-0B-OBS-001_VERIFICATION_RESULTS.json` | `451A35543562F78399688ABCB63B605BB34253C8A4B5B5E5B73154360C0CEDDA` |

The binary artifacts were temporary candidate outputs. Their identities were preserved and the files were removed under STG-0B-CLEAN-001.

The authoritative candidate source is preserved by the Source Commit. No release, deployable image, operational runtime, or production artifact was produced.

## Isolation Result

The successful restore and build used:

- repository-local isolated CLI home;
- repository-local application-data path;
- repository-local package cache;
- disabled first-run experience;
- disabled telemetry;
- disabled ASP.NET development-certificate generation;
- and no network package source.

No further external-boundary event occurred.

