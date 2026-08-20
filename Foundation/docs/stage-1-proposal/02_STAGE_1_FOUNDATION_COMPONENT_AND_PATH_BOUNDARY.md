# 02 - Foundation Component and Path Boundary

## Repository-relative canonical path model

The current Windows workspace path:

- `C:\Falcon\Falcon1\`

is only a local observation. It is not the canonical portable repository
identity.

The proposal SHALL use repository-root-relative canonical paths.

## Path boundary table

| Path ID | Repository-relative path | Existing or planned | Owner | Purpose | Allowed content | Prohibited content | Creation Work Package |
|---|---|---|---|---|---|---|---|
| RP-01 | `./` | Existing | Project Owner | repository root boundary | canonical docs, source, tests, evidence, build assets | production, cloud, financial, or external-controlled paths | WP-01 |
| RP-02 | `./Falcon.Foundation.ControlledProjectFoundation.slnx` | Planned | Project Owner | canonical Stage 1 solution identity | solution definition only | executable artifacts | WP-01 |
| RP-03 | `./src/Falcon.Foundation.Core/` | Planned | Foundation | protected core source surface | foundation source, contracts, and exact implementation inputs | application business logic | WP-02 |
| RP-04 | `./src/Falcon.Foundation.Contracts/` | Planned | Foundation | stable contract surface | contracts and schema definitions | runtime behavior | WP-02 |
| RP-05 | `./src/Falcon.Foundation.Infrastructure/` | Planned | Foundation | adapter and infrastructure boundary | adapters and implementation glue | protected-core logic | WP-02 |
| RP-06 | `./tests/Falcon.Foundation.Tests/` | Planned | Foundation | unit and behavior verification | tests only | source implementation | WP-06 |
| RP-07 | `./tests/Falcon.Foundation.Architecture.Tests/` | Planned | Foundation | architecture enforcement tests | architecture tests only | runtime behavior | WP-06 |
| RP-08 | `./build/` | Planned | Foundation | build scripts and command entrypoints | build scripts only | source implementation | WP-03, WP-05 |
| RP-09 | `./tools/` | Planned | Foundation | tool manifests and tool wrappers | tool manifests only | generated source | WP-03 |
| RP-10 | `./artifacts/locks/` | Planned | Foundation | dependency locks and provenance records | lock files, provenance records | mutable build outputs | WP-04 |
| RP-11 | `./artifacts/evidence/stage-1/` | Planned | Foundation | execution evidence output | immutable evidence outputs | source code, build scripts, ad hoc files | WP-08, WP-09, WP-10, WP-11, WP-13 |
| RP-12 | `./artifacts/traceability/stage-1/` | Planned | Foundation | traceability outputs | traceability exports only | runtime behavior | WP-09 |
| RP-13 | `./artifacts/generated/stage-1/` | Planned | Foundation | generated artifacts | generated artifacts only | secrets or uncontrolled outputs | WP-08, WP-10 |
| RP-14 | `./artifacts/tmp/stage-1/` | Planned | Foundation | temporary outputs | temporary files only | persistent evidence or source | WP-10 |
| RP-15 | `./docs/stage-1-proposal/` | Existing | Project Owner | Stage 1 proposal package | documentary proposal material only | code, build artifacts, or runtime outputs | WP-13 |

## Boundary rules

- Existing Stage 0 source and evidence paths remain preserved.
- No silent move, rename, replacement, or duplication of active enabling
  provider source is allowed.
- No permanent name containing `Stage1` is required by this package.
- No file or directory may be created by this correction task.

## Sources

- `docs/adrs/ADR-I002_REPOSITORY_AND_DEPENDENCY_POLICY.md`
- `docs/adrs/ADR-I001_FOUNDATION_RUNTIME_AND_LANGUAGE.md`
- `docs/adrs/ADR-F001_FOUNDATION_EXECUTION_AND_ISOLATION_MODEL.md`
