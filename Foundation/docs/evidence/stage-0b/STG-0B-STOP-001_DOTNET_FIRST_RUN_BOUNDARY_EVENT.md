# Stage 0B Stop Record — .NET First-Run Boundary Event

**Evidence ID:** STG-0B-STOP-001  
**Observed Date:** 2026-07-26  
**Authority:** GOV-051  
**Status:** Stopped Pending Authority Review  
**Stage 0B Activation:** Not Granted

## 1. Intended Action

The authorized action was a local restore and build using:

- the already-present .NET 10.0.302 SDK;
- a repository-local `NuGet.Config` with all package sources cleared;
- no external packages;
- and no intended network access.

## 2. Observed Event

The first .NET CLI invocation:

- attempted to access the user-level NuGet configuration outside the declared repository boundary;
- was denied access to that external configuration;
- reported creation of an ASP.NET Core HTTPS development certificate in the Codex sandbox user profile;
- failed restore;
- and therefore could not build either Stage 0B project.

## 3. Containment

- Stage 0B execution stopped immediately.
- No package or tool was downloaded.
- No external package was restored.
- No candidate assembly was successfully built.
- No verification plan was executed.
- No Falcon component or candidate was activated.
- No cloud or financial connection occurred.
- No real secret or financial material entered the repository.
- Generated repository-local `bin` and `obj` residue was classified for cleanup and exclusion.

## 4. Boundary Assessment

The attempted external configuration access and automatic certificate side effect were not declared by the approved Bootstrap Execution Context.

The event therefore requires explicit review before execution may continue.

## 5. Proposed Remediation

If separately approved, continuation would:

- use a repository-contained isolated .NET CLI home;
- use a repository-contained isolated application-data path;
- use a repository-contained package cache;
- disable telemetry and first-run experience;
- disable automatic ASP.NET development-certificate generation;
- keep all package sources cleared;
- use no installation or download;
- clean repository-local generated residue;
- rerun restore and build within the isolated boundary;
- and preserve all results as Stage 0B evidence.

The development certificate reported in the external Codex sandbox profile shall not be trusted, imported, referenced, or used by Falcon.

## 6. Current Finding

```text
STAGE 0B STOPPED — AUTHORITY REVIEW REQUIRED
```
