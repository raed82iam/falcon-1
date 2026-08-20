# BLD-001 — Foundation Toolchain and Build Baseline Catalog

**Version:** 1.0  
**Status:** Approved  
**Catalog Snapshot Date:** 2026-07-25  
**Owner:** Falcon Foundation Governance  
**Governing Authority:** Falcon Vision; Falcon Constitution; GOV-001; ADR-I001; ADR-I002; ADR-I003; ADR-I007  
**Applicable Specifications:** SEC-001; SEC-002; FCE-001  
**Related Documents:** ROADMAP-001; PIPE-001; ENV-001; TRC-001  
**Supersedes:** None  
**Superseded By:** BLD-001 v1.1 under GOV-033  
**Implementation Authority:** Not Granted

## 1. Purpose

This Catalog defines the exact Foundation toolchain and build baseline admitted for producing, analyzing, testing, inspecting, and describing Falcon Foundation artifacts.

It prevents:

- floating or implicit tools;
- developer-machine authority;
- silent version drift;
- unverified downloads;
- provider-specific build meaning;
- tool substitution without governance;
- network-dependent compilation;
- unverifiable build provenance; and
- promotion of artifacts built outside an Approved baseline.

This Catalog selects governed values. PIPE-001 defines pipeline behavior. ENV-001 defines executable environments. Neither this Catalog nor tool availability authorizes implementation.

## 2. Foundational Rule

> **Every tool that can affect a governed artifact, result, evaluation, or Claim is a governed dependency.**

A tool is not admitted because it is installed, popular, supplied by a platform, or produced by a trusted vendor.

Admission requires:

- exact identity;
- exact version;
- Approved source;
- verified content digest;
- applicable license and security review;
- declared purpose;
- declared execution environment;
- preserved provenance;
- bounded authority; and
- conformance evidence.

## 3. Catalog States

| State | Meaning |
|---|---|
| `DRAFT` | Proposed value with no authority |
| `APPROVED` | Value approved for preparation and verification |
| `ACTIVE` | Value authorized for governed execution in a named Environment Profile |
| `DEPRECATED` | Existing use may continue only within an approved transition |
| `RETIRED` | New use prohibited; historical evidence retained |
| `FORBIDDEN` | Use prohibited, including cached or locally installed copies |

Approval does not imply Activation.

No entry becomes `ACTIVE` until ENV-001 binds it to exact platform artifacts, digests, source evidence, and verified execution conditions.

## 4. Foundation Baseline Identity

| Field | Value |
|---|---|
| Baseline ID | `FALCON-BUILD-FOUNDATION-1` |
| Catalog Version | `BLD-001 v1.0` |
| Lifecycle | `APPROVED` |
| Language | C# 14 |
| Target Framework | `.NET 10` / `net10.0` |
| Initial Operating-System Families | Windows; Linux |
| Initial Processor Architecture | x86-64 |
| Financial Connectivity | Prohibited |
| Production Credentials | Prohibited |
| Market Data | Prohibited |
| Capital Authority | None |

The baseline SHALL NOT be identified by a mutable label such as `latest`, `stable`, `LTS`, `current`, or a moving container tag.

## 5. Canonical Tool Registry

### 5.1 Runtime, Compiler, and Build

| Tool ID | Governed value | Version binding | Approved source | Lifecycle |
|---|---|---|---|---|
| `BLD-DOTNET-SDK` | .NET SDK | `10.0.302` | Microsoft .NET official distribution | `APPROVED` |
| `BLD-DOTNET-RUNTIME` | .NET Runtime | `10.0.10` | Microsoft .NET official distribution | `APPROVED` |
| `BLD-CSHARP` | C# language | `14.0` | Compiler bundled in .NET SDK `10.0.302` | `APPROVED` |
| `BLD-ROSLYN` | C# compiler | Exact compiler payload bundled in SDK `10.0.302` | Microsoft .NET SDK payload | `APPROVED` |
| `BLD-MSBUILD` | MSBuild | Exact payload bundled in SDK `10.0.302` | Microsoft .NET SDK payload | `APPROVED` |
| `BLD-NUGET` | NuGet client and restore engine | Exact payload bundled in SDK `10.0.302` | Microsoft .NET SDK payload | `APPROVED` |

Bundled-tool identity is the identity of the exact verified SDK distribution plus its recorded inventory. A separately installed compiler, MSBuild, or NuGet client is not equivalent.

### 5.2 Static Analysis and Dependency Security

| Tool ID | Governed value | Version binding | Purpose | Lifecycle |
|---|---|---|---|---|
| `BLD-NET-ANALYZERS` | Microsoft .NET analyzers | Exact analyzer payload bundled in SDK `10.0.302` | Correctness, quality, security, and platform analysis | `APPROVED` |
| `BLD-ROSLYN-NULLABLE` | C# nullable analysis | Compiler behavior bundled in SDK `10.0.302` | Null-safety analysis | `APPROVED` |
| `BLD-NUGET-AUDIT` | NuGet vulnerability audit | Exact restore/audit behavior bundled in SDK `10.0.302` | Direct and transitive dependency vulnerability assessment | `APPROVED` |
| `BLD-SECRET-SCANNER` | Secret scanner | No implementation admitted | Repository and artifact secret detection | `DRAFT` |
| `BLD-BINARY-SCANNER` | Published-artifact vulnerability scanner | No implementation admitted | Runtime, operating-system, and binary composition assessment | `DRAFT` |

The two `DRAFT` entries are mandatory Gate capabilities, not optional omissions. No Release Candidate baseline may become `ACTIVE` until exact tools are selected and approved by a BLD-001 amendment.

No analyzer result becomes authoritative merely because the analyzer is Microsoft-supplied or SDK-bundled.

### 5.3 Test Platform

| Tool ID | Governed value | Exact version | Approved source | Lifecycle |
|---|---|---|---|---|
| `BLD-TEST-PLATFORM` | Microsoft.Testing.Platform | `2.3.2` | NuGet.org official package source | `APPROVED` |
| `BLD-MSTEST` | MSTest aggregate package | `4.3.2` | NuGet.org official package source | `APPROVED` |
| `BLD-TEST-SDK-COMPAT` | Microsoft.NET.Test.Sdk | `18.8.1` | NuGet.org official package source | `APPROVED_WITH_RESTRICTION` |
| `BLD-COVERAGE` | Governed coverage collector | No implementation admitted | To be selected by amendment | `DRAFT` |

Microsoft.Testing.Platform is the canonical Foundation test execution model.

`Microsoft.NET.Test.Sdk` SHALL NOT be added by default. It is admitted only as an exact compatibility dependency where a governed test adapter proves that it is required and cannot yet execute directly through Microsoft.Testing.Platform.

Coverage percentage is evidence about exercised code. It is not evidence that required behavior, safety, or security is complete.

### 5.4 SBOM and Provenance

| Tool ID | Governed value | Exact version | Governed output | Lifecycle |
|---|---|---|---|---|
| `BLD-SBOM` | Microsoft SBOM Tool | `4.1.5` | SPDX `3.0.1` | `APPROVED` |
| `BLD-PROVENANCE` | Falcon governed provenance generator | No implementation admitted | Canonical provenance manifest governed by PIPE-001 and SEC-002 | `DRAFT` |
| `BLD-ATTESTATION-SIGNER` | Evidence and provenance signing tool | No implementation admitted | Signed Trust Objects under SEC-002 and DESIGN-SEC-001 | `DRAFT` |

Falcon SHALL make no SLSA conformance Claim until the applicable SLSA requirements are independently verified against the exact pipeline and evidence set.

An SBOM is an inventory Claim. It is not by itself proof of safety, provenance, license acceptability, or absence of vulnerabilities.

### 5.5 Persistence Verification

| Tool ID | Governed value | Exact version | Approved source | Lifecycle |
|---|---|---|---|---|
| `BLD-POSTGRESQL-SERVER` | PostgreSQL Server | `18.4` | PostgreSQL official distribution or an ENV-001-approved vendor build | `APPROVED` |
| `BLD-POSTGRESQL-CLIENT` | PostgreSQL client tools | `18.4` | Same verified distribution family as the server profile | `APPROVED` |

PostgreSQL is a Foundation realization choice, not a domain assumption.

No Kernel, Core policy, Contract, or governed state meaning may depend on PostgreSQL-specific types, syntax, extensions, errors, transaction identifiers, ordering, timing, or operational behavior.

Persistence verification SHALL include unavailable, delayed, interrupted, ambiguous-commit, duplicate-effect, restart, reconciliation, and evidence-loss conditions.

### 5.6 Runner and Environment Binding

| Tool ID | Governed value | Version authority | Lifecycle |
|---|---|---|---|
| `BLD-WINDOWS-RUNNER` | Isolated Windows x86-64 runner | Exact image, operating system, shell, installed tools, and digest SHALL be defined by ENV-001 | `DRAFT` |
| `BLD-LINUX-RUNNER` | Isolated Linux x86-64 runner | Exact image, distribution, shell, installed tools, and digest SHALL be defined by ENV-001 | `DRAFT` |
| `BLD-CI-ADAPTER` | Automation-provider adapter | Provider-specific and non-authoritative; exact adapter revision SHALL be evidence | `DRAFT` |

The runner is part of the build input.

A provider-hosted label such as `windows-latest` or `ubuntu-latest` is prohibited for governed execution.

CI configuration may invoke the canonical pipeline. It may not redefine Falcon's build, evidence, evaluation, or promotion meaning.

## 6. Approved Sources

Initial Approved source classes are:

| Source ID | Scope | Rule |
|---|---|---|
| `SRC-DOTNET-MICROSOFT` | .NET SDK and runtime | Official Microsoft .NET release distribution only |
| `SRC-NUGET-ORG` | Admitted .NET packages | Exact package version from NuGet.org, acquired into a verified offline bundle |
| `SRC-POSTGRESQL-OFFICIAL` | PostgreSQL source or official binaries | Exact PostgreSQL release from an official source |
| `SRC-MICROSOFT-SBOM` | Microsoft SBOM Tool | Exact signed release from the official Microsoft repository |
| `SRC-FALCON` | Repository-owned tools, rules, schemas, and pipeline definitions | Immutable Falcon Source Revision and governed review |

Mirrors and caches are delivery mechanisms. They do not become provenance authorities.

Every acquired item SHALL retain:

- source identity;
- immutable locator where available;
- version;
- file name;
- size;
- cryptographic digest;
- publisher signature where available;
- signature-verification result;
- acquisition time and Time Observation;
- acquiring identity;
- license evidence;
- vulnerability-assessment reference; and
- admission decision.

## 7. Content Identity

Before Activation, every executable, package, SDK archive, runtime pack, targeting pack, analyzer, database distribution, scanner, and runner image SHALL be bound to an exact cryptographic digest.

The required minimum digest is SHA-256 unless a governing security profile requires a stronger or additional digest.

Publisher signatures SHALL be verified where supplied. A signature does not replace content identity.

Package name and version alone are insufficient.

If identical names and versions resolve to different bytes:

- acquisition fails;
- the content is quarantined;
- prior evidence remains unchanged;
- the contradiction is recorded; and
- Security Authority review is required.

## 8. SDK and Language Rules

The Foundation baseline SHALL use:

- `net10.0`;
- C# language version `14.0`;
- no preview language features;
- exact SDK `10.0.302`;
- roll-forward disabled for governed builds;
- nullable reference analysis enabled;
- deterministic compilation enabled;
- warnings treated as errors;
- portable debug symbols where symbols are produced;
- locked dependency restore;
- restore separated from compilation;
- external network disabled during compilation and testing unless a specific verification profile requires an isolated simulated network; and
- explicit Release configuration for governed candidate artifacts.

Availability of a newer .NET 10 patch SHALL NOT silently change the baseline.

Because supported .NET operation requires current servicing, a newer security or servicing patch SHALL trigger an expedited BLD-001 review. It SHALL NOT be consumed without a versioned baseline amendment and new verification.

## 9. Build Output Profiles

### 9.1 Developer Output

Developer output:

- may be framework-dependent;
- is never promotable;
- may use reduced optional evidence;
- SHALL still use exact dependencies when claiming conformance; and
- SHALL be visibly identified as non-release material.

### 9.2 Governed Candidate Output

A governed candidate SHALL:

- be built for an explicit target platform;
- bind the exact .NET runtime patch;
- use an Approved self-contained or otherwise equivalently runtime-bound profile defined by PIPE-001;
- disable trimming, Native AOT, single-file transformation, and ReadyToRun unless separately admitted;
- contain no machine-specific path, secret, uncontrolled timestamp, or developer identity;
- carry an immutable artifact identity;
- be produced only from verified offline inputs; and
- be the exact artifact verified and promoted.

Cross-platform artifacts are distinct artifacts and SHALL have distinct identities and evidence.

## 10. Dependency Resolution

Dependency resolution SHALL:

- use explicit versions;
- prohibit floating ranges;
- produce and enforce lock files;
- include the complete transitive closure;
- reject unapproved sources;
- reject source substitution;
- reject version substitution;
- reject digest mismatch;
- evaluate known vulnerabilities;
- evaluate licenses;
- record dependency purpose and owner;
- apply Falcon Adapter and layer-boundary rules; and
- produce a content-identified offline dependency bundle.

The build stage SHALL fail if it attempts to acquire a missing dependency.

No package source, global cache, local feed, environment variable, user profile, or machine configuration may silently alter resolution.

## 11. Analysis Policy

The baseline SHALL enforce:

- compiler diagnostics;
- nullable diagnostics;
- SDK analyzer diagnostics;
- prohibited-API rules;
- layer and dependency-boundary rules;
- generated-code provenance rules;
- secret detection;
- vulnerable-dependency detection;
- unsafe construct review;
- platform compatibility;
- deterministic-build properties; and
- absence of financial connectivity and credentials.

Suppressions SHALL be:

- narrow;
- attributable;
- reasoned;
- versioned;
- reviewable;
- time-bounded when appropriate; and
- visible in the Root Verification Evidence Set.

A broad suppression, disabled rule set, or unexplained baseline file is prohibited.

## 12. Test Policy

Governed tests SHALL execute through the admitted test platform and SHALL:

- identify the exact test assembly and source revision;
- identify the test platform, framework, adapters, and extensions;
- prohibit silent test discovery failure;
- fail on crashed, abandoned, or unreported sessions;
- preserve retries as separate Verification Sessions;
- preserve skipped, excluded, inconclusive, and not-run cases;
- use deterministic seeds where determinism is required;
- record randomized seeds where randomness is permitted;
- isolate tests from production and financial paths;
- avoid dependence on arrival order unless a Contract guarantees it; and
- produce machine-readable immutable evidence.

A later passing retry does not erase an earlier failure.

## 13. SBOM Policy

Every governed candidate SHALL have an SPDX 3.0.1 SBOM produced from:

- the exact candidate artifact;
- locked direct dependencies;
- locked transitive dependencies;
- runtime and framework components;
- bundled native components;
- generated material;
- licenses;
- supplier and provenance data where established; and
- known omissions and uncertainty.

The SBOM SHALL bind to the candidate digest and Pipeline Execution ID.

Corrections produce a new SBOM Trust Object linked to the superseded object. Historical SBOM evidence SHALL NOT be overwritten.

## 14. Provenance Policy

Build provenance SHALL record:

- Source Revision;
- Foundation Baseline;
- Build Intent;
- Pipeline Definition;
- Toolchain Baseline;
- Environment Profile;
- dependency bundle identity;
- Effective Build Configuration;
- runner identity;
- actor and service identities;
- start and completion Time Observations;
- material inputs;
- produced artifact digests;
- Verification Evidence Set;
- limitations and unresolved uncertainty; and
- applicable signatures and custody evidence.

Provenance SHALL be canonical, immutable, attributable, challengeable, and governed as a Trust Object.

No component that produces, transforms, aggregates, or signs provenance may be the sole authority declaring the candidate promotable.

## 15. Database Verification Policy

The exact PostgreSQL server and client patch SHALL be part of the Environment Profile and evidence.

Verification SHALL NOT rely solely on a developer-installed database.

Every database instance used for governed verification SHALL be:

- isolated;
- disposable or reproducibly restorable;
- non-production;
- free of financial data;
- identified by exact profile and digest;
- initialized from governed material;
- constrained by a declared locale, encoding, collation, and time-zone profile;
- observed for startup and health;
- denied uncontrolled network access; and
- destroyed or retained according to evidence policy.

Database success SHALL NOT be acknowledged as Falcon state success until the governing persistence Contract is satisfied.

## 16. Runner Independence

The canonical build and verification meaning SHALL remain repository-owned and provider-independent.

Windows and Linux runners SHALL:

- execute the same governed pipeline semantics;
- receive the same content-identified inputs;
- use isolated filesystems and caches;
- expose exact operating-system and tool identity;
- deny financial and production paths;
- disable undeclared network access;
- preserve evidence outside the mutable workspace; and
- fail closed when required environment evidence is unavailable.

Shell scripts and provider workflows are Adapters. They are not the Pipeline authority.

## 17. Reproducibility

A deterministic claim requires byte-identical output for identical declared inputs and environment.

A reproducibility claim requires:

- independently established clean environments;
- independently acquired or verified identical input bundles;
- exact toolchain and environment identity;
- no shared mutable build cache;
- preserved comparison evidence; and
- byte-for-byte identity where the format permits it.

An unexplained difference produces `FAIL` or `INCONCLUSIVE`.

Differences SHALL NOT be hidden by deleting fields after the build unless that normalization is itself an Approved, deterministic, governed production step applied before artifact identity is established.

## 18. Tool Upgrade and Replacement

A tool upgrade, downgrade, rebuild, source change, signing-key change, runner-image change, or digest change SHALL create a new baseline candidate.

The change requires:

- reason and scope;
- security and compatibility review;
- dependency and license review;
- cross-platform verification;
- deterministic and reproducibility evidence;
- comparison against the prior baseline;
- known behavioral differences;
- rollback plan;
- independent evaluation;
- Approval; and
- explicit Activation.

Emergency security replacement may shorten timing. It SHALL NOT remove identity, evidence, authority, or non-financial safeguards.

No Vendor Lock-in applies. Replacing a tool SHALL NOT require redefining Falcon requirements, Contracts, evidence meaning, or promotion authority.

## 19. Failure Policy

The pipeline SHALL fail or become blocked when:

- a required tool is absent;
- a tool is not the exact admitted version;
- a digest or signature is invalid;
- the source is unapproved;
- a dependency is floating or unlocked;
- an external acquisition occurs during isolated execution;
- a required analysis capability is not admitted;
- test discovery or reporting is incomplete;
- the runner identity is ambiguous;
- the database profile differs;
- SBOM or provenance generation is incomplete;
- evidence cannot be preserved;
- the artifact differs from the verified artifact; or
- an authority or jurisdiction condition is unproven.

Locally installed alternatives SHALL NOT be used as fallback.

## 20. Known Deliberate Blocks

The following remain unresolved and prevent `FALCON-BUILD-FOUNDATION-1` from becoming `ACTIVE`:

1. exact Windows runner profile;
2. exact Linux runner profile;
3. exact secret scanner;
4. exact published-artifact vulnerability scanner;
5. exact coverage collector;
6. exact provenance generator;
7. exact attestation signer;
8. acquired binary and package digests;
9. complete license and vulnerability admission evidence;
10. PIPE-001;
11. TRC-001;
12. ENV-001; and
13. explicit implementation authority.

These are safety controls, not editorial omissions.

## 21. Catalog Requirements

- **BLD-001-REQ-001:** Every governed tool SHALL have exact identity, version, source, digest, purpose, and lifecycle.
- **BLD-001-REQ-002:** Mutable version labels SHALL be prohibited.
- **BLD-001-REQ-003:** Bundled tools SHALL be identified through the exact verified SDK payload and inventory.
- **BLD-001-REQ-004:** Governed builds SHALL use .NET SDK `10.0.302`, runtime `10.0.10`, C# `14.0`, and `net10.0` until amended.
- **BLD-001-REQ-005:** Governed SDK roll-forward and preview language features SHALL be prohibited.
- **BLD-001-REQ-006:** Dependency acquisition SHALL be separated from isolated execution.
- **BLD-001-REQ-007:** Dependencies SHALL be exact, locked, source-approved, content-identified, and available offline.
- **BLD-001-REQ-008:** Compiler warnings and admitted analyzer diagnostics SHALL fail the governed build unless an Approved suppression applies.
- **BLD-001-REQ-009:** Microsoft.Testing.Platform `2.3.2` SHALL be the canonical test execution model.
- **BLD-001-REQ-010:** MSTest `4.3.2` SHALL be the initial Foundation test framework.
- **BLD-001-REQ-011:** Microsoft.NET.Test.Sdk `18.8.1` SHALL be used only as a justified compatibility dependency.
- **BLD-001-REQ-012:** Governed candidates SHALL have an SPDX `3.0.1` SBOM generated by Microsoft SBOM Tool `4.1.5`.
- **BLD-001-REQ-013:** PostgreSQL verification SHALL use server and client version `18.4`.
- **BLD-001-REQ-014:** PostgreSQL details SHALL NOT cross Falcon Persistence Contracts.
- **BLD-001-REQ-015:** Windows and Linux runner identity SHALL be exact and SHALL NOT use mutable provider labels.
- **BLD-001-REQ-016:** CI providers and shell wrappers SHALL remain non-authoritative Adapters.
- **BLD-001-REQ-017:** A cache SHALL NOT become an authoritative dependency source.
- **BLD-001-REQ-018:** Test retries SHALL preserve every Verification Session and SHALL NOT erase prior failure.
- **BLD-001-REQ-019:** SBOM and provenance SHALL bind to the exact candidate artifact.
- **BLD-001-REQ-020:** Tool-produced Claims SHALL remain subject to independent evaluation and challenge.
- **BLD-001-REQ-021:** A tool change SHALL create a new baseline candidate and require governed Approval and Activation.
- **BLD-001-REQ-022:** Missing mandatory tool capability SHALL block Release Candidate activation.
- **BLD-001-REQ-023:** BLD-001 Approval SHALL NOT activate tools, runners, environments, implementation, or financial use.

## 22. Required Conformance Evidence

Activation requires evidence that:

- exact SDK selection rejects a different installed SDK;
- roll-forward cannot occur;
- preview language features fail;
- restore uses only Approved sources and exact lock files;
- digest mismatch fails;
- the isolated build cannot access external networks;
- local and global caches cannot change dependency resolution;
- warnings and analyzer violations fail;
- test discovery failure cannot appear as success;
- retry history remains preserved;
- Windows and Linux runners expose exact immutable identity;
- PostgreSQL version and environment mismatch fail;
- PostgreSQL-specific assumptions do not cross the Persistence boundary;
- SBOM contents bind to the exact candidate;
- provenance can reconstruct declared inputs and outputs;
- secret and vulnerability scanning capabilities are present and exact;
- independently rebuilt permitted outputs are identical;
- provider adapters cannot weaken gates;
- incomplete evidence prevents promotion; and
- no path reaches financial systems, credentials, data, or capital.

## 23. Required Before Activation

`FALCON-BUILD-FOUNDATION-1` SHALL remain non-active until:

1. BLD-001 is Approved;
2. every mandatory tool has an `APPROVED` exact implementation;
3. exact distribution and package digests are recorded;
4. PIPE-001 is Approved;
5. TRC-001 is Approved;
6. ENV-001 is Approved for Windows and Linux;
7. tool licenses and vulnerabilities are evaluated;
8. offline dependency bundles are reproducibly created;
9. deterministic and reproducibility verification passes;
10. SBOM and provenance verification passes;
11. runner isolation and non-financial boundaries pass;
12. independent Evidence Completeness Authority confirms a complete case;
13. a competent authority accepts the baseline within declared jurisdiction; and
14. explicit implementation authority is granted.

## 24. Source Verification Record

The version choices in this proposed snapshot were checked on 2026-07-25 against:

- Microsoft .NET official support and download records;
- Microsoft Learn documentation for `dotnet test` and Microsoft.Testing.Platform;
- NuGet.org package indexes for Microsoft.Testing.Platform, MSTest, and Microsoft.NET.Test.Sdk;
- Microsoft SBOM Tool official release records;
- SPDX `3.0.1`;
- PostgreSQL official versioning and documentation records; and
- the Falcon Approved ADR baseline.

Source verification establishes the reported version. It does not admit unverified bytes.

## 25. Foundational Rules

> **The developer machine is not a build authority.**

> **A version label identifies intent; a verified digest identifies content.**

> **Acquisition may use the network. Governed build and verification use only verified inputs.**

> **A tool produces observations and results. It does not grant truth, completeness, acceptance, or promotion.**

> **The exact artifact verified is the only artifact eligible for promotion.**

## 26. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner and current Constitutional Authority | Approved | GOV-019 | 2026-07-25 |

Approval of BLD-001 approves the selected catalog values and the stated deliberate blocks.

It does not:

- activate `FALCON-BUILD-FOUNDATION-1`;
- install or download a tool;
- authorize implementation;
- approve PIPE-001, TRC-001, or ENV-001;
- permit use of a `DRAFT` tool;
- authorize financial connectivity;
- authorize production use; or
- authorize promotion of an artifact.
