# ADR-I001 — Foundation Runtime and Language

**Identifier:** ADR-I001  
**Version:** 1.0  
**Status:** Accepted  
**Date:** 2026-07-24  
**Decision Owner:** Falcon Project Owner  
**Scope:** FRS-001 implementation runtime, primary language, and extension execution boundary  
**Affected Specifications:** SYS-001, SEC-001, PLG-001, FRS-001  
**Applicable Standards:** STD-003, STD-007, STD-012  
**Related ADRs:** ADR-F001, ADR-F003, ADR-F006  
**Supersedes:** None  
**Superseded By:** None  
**Decision Record:** Project Owner approval recorded on 2026-07-24

## 1. Context

FRS-001 requires a supported, maintainable, secure, cross-platform runtime and one primary implementation language before source work begins.

The choice must support long-lived Core development and governed replaceability without treating dynamic loading as an adequate security boundary.

## 2. Decision Drivers

- maintainability and development clarity;
- memory-safe managed execution for ordinary Core behavior;
- mature security, cryptography, concurrency, diagnostics, and testing support;
- official long-term servicing;
- Windows and Linux operation;
- reproducible builds and dependency control;
- controlled dynamic capability loading; and
- process isolation for capabilities whose trust or consequence requires it.

## 3. Higher-Authority Constraints

The decision remains subordinate to the Vision, Constitution, SYS-001, SEC-001, ADR-F001, FRS-001, and the approved plug-and-play governance.

The runtime cannot create authority, weaken isolation, or make an untrusted capability safe merely by loading it successfully.

## 4. Alternatives Considered

### Rust as the universal Foundation language

Rust offers strong memory-safety and low-level control. It was not selected as the universal Foundation language because its implementation and maintenance complexity is not justified for the first governed, non-financial release.

### Multiple first-class Foundation languages

This was rejected because it would multiply toolchains, dependency surfaces, build evidence, operational knowledge, and compatibility burden before a demonstrated need exists.

### C# on .NET LTS

This was selected for its balance of safety, maintainability, mature platform capabilities, official support, and cross-platform operation.

## 5. Decision

Falcon Foundation SHALL use:

- **Primary language:** C#;
- **Runtime:** .NET 10 LTS;
- **Runtime policy:** latest approved security patch within the .NET 10 LTS line;
- **Required operating systems:** Windows and Linux;
- **Build mode:** reproducible SDK-based build under a version-pinned toolchain; and
- **Language scope:** one primary source language for FRS-001.

A second implementation language SHALL NOT enter FRS-001 without a separate Accepted ADR demonstrating necessity, ownership, security, build provenance, operational support, and exit strategy.

Trusted, low-consequence replaceable capabilities MAY use approved .NET contracts in a governed load context where their passport and isolation classification permit it.

An untrusted, third-party, high-consequence, or independently containable capability SHALL execute in a separate process or stronger approved isolation boundary and communicate through FIL. Dynamic assembly loading is a dependency boundary; it is not a security boundary.

The .NET runtime, SDK, compiler, and packages SHALL remain subject to dependency provenance, patch, vulnerability, and reproducibility controls.

This decision does not authorize ASP.NET, a user interface, network exposure, financial behavior, third-party plugin execution, or production deployment.

## 6. Consequences

- Foundation engineers use one primary language and toolchain.
- Falcon can operate on Windows and Linux.
- Managed runtime safety and diagnostics reduce ordinary implementation risk.
- Runtime patch governance becomes mandatory.
- High-risk plugins cannot rely on in-process unloading for security isolation.
- Rust or another language remains possible later only through evidence and an ADR.

## 7. Risks and Mitigations

- **Runtime compromise or vulnerability:** pin the supported LTS line, track security advisories, and apply approved current patches.
- **Managed-runtime resource abuse:** enforce process, time, memory, and cancellation boundaries proportionate to capability consequence.
- **False plugin isolation:** require separate-process FIL communication for untrusted or high-consequence capabilities.
- **Platform-specific drift:** require Windows and Linux verification for portable Foundation behavior.
- **Premature polyglot complexity:** prohibit a second language without an Accepted ADR.

## 8. Compatibility and Transition

This is the first Falcon1 runtime decision. Future runtime replacement requires a superseding ADR and evidence that Contracts, authoritative state, evidence, security, and rollback remain preserved.

## 9. Conformance Evidence

Conformance requires:

- an exact .NET 10 SDK and runtime identity;
- current approved security patch evidence;
- deterministic dependency restoration;
- reproducible Windows and Linux builds;
- no second source language in FRS-001;
- process-isolation proof for any capability classified as requiring it; and
- absence of financial and external production paths.

## 10. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner and current Constitutional Authority | Accepted | رائد عموره — “موافق على C# و.NET 10 LTS كأساس لـFalcon Foundation.” | 2026-07-24 |
