# Stage 1 Host Toolchain Revalidation

## Validation identity

- Execution identity: `laptop-klg53di4\raeda`
- Working root: `C:\Falcon\Falcon1`
- Administrator elevation required: `NO`

## Host and toolchain results

| Command | Result |
|---|---|
| `where.exe dotnet` | `C:\Program Files\dotnet\dotnet.exe` |
| `dotnet --info` | PASS |
| `dotnet --list-sdks` | PASS |
| `dotnet --list-runtimes` | PASS |
| `dotnet msbuild -version` | PASS |
| `dotnet nuget list source --format detailed` | PASS |
| `dotnet nuget list source --format detailed --configfile "C:\falcon\ValidationProfile\Roaming\NuGet\NuGet.Config"` | PASS |
| `dotnet nuget locals all --list` | PASS |

## Toolchain facts

- .NET SDK: `10.0.302`
- .NET Host/Runtime: `10.0.10`
- MSBuild: `18.6.11+35b593beb`
- C# baseline: `14.0`
- Target framework baseline: `net10.0`
- Preview SDK selection: `NO`

## NuGet boundary facts

- isolated config sources: `0`
- external package sources used: `0`
- package downloads: `0`
- restore activity: `0`
- build activity: `0`
- test activity: `0`
- unauthorized-access errors: `0`

## Conclusion

The host toolchain validation passed under the isolated, process-scoped NuGet profile.

