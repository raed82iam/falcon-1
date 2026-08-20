# Stage 1 WP-01 Verification Evidence

## Host and toolchain evidence

- `.NET SDK`: `10.0.302`
- `.NET Runtime`: `10.0.10`
- C# baseline: `14.0`
- target framework: `net10.0`
- preview SDK selection: `NO`

## NuGet evidence

- isolated validation profile: `C:\falcon\ValidationProfile`
- isolated source list: `0`
- explicit config source list: `0`
- `dotnet nuget locals all --list`: `PASS`
- unauthorized-access errors: `0`

## Boundary evidence

- repository root: `C:\Falcon\Falcon1`
- active baseline unchanged: `PASS`
- exact 13 activation manifests present: `PASS`
- no manifest expired, revoked, suspended, invalidated, or superseded: `PASS`

## Remediation note

This evidence file is a WP-01 summary record. The retrospective verification
supplements now separate present-state confirmation from original execution
proof and keep unrecovered contemporaneous command evidence marked as missing.

