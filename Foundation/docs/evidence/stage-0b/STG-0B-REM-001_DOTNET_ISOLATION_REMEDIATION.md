# Stage 0B .NET Isolation Remediation

**Evidence ID:** STG-0B-REM-001  
**Recorded Date:** 2026-07-26  
**Authority:** GOV-052  
**Status:** Authorized for Execution

## Controls

The resumed .NET execution shall set:

- `DOTNET_CLI_HOME` to a repository-local ignored directory;
- `APPDATA` to a repository-local ignored directory;
- `NUGET_PACKAGES` to a repository-local ignored directory;
- `DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1`;
- `DOTNET_CLI_TELEMETRY_OPTOUT=1`;
- `DOTNET_GENERATE_ASPNET_CERTIFICATE=false`;
- and `NUGET_XMLDOC_MODE=skip`.

The repository-local `NuGet.Config` clears all package sources.

## Acceptance

Remediation succeeds only when restore, build, and verification require no external package source, installation, download, certificate generation, or path outside the declared isolated state.
