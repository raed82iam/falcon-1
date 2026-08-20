# ADR-I002 — Repository and Dependency Policy

**Identifier:** ADR-I002  
**Version:** 1.0  
**Status:** Accepted  
**Date:** 2026-07-24  
**Decision Owner:** Falcon Project Owner  
**Scope:** FRS-001 repository structure, dependency admission, isolation, provenance, versioning, and replacement  
**Affected Specifications:** SYS-001, SEC-001, PLG-001, FRS-001  
**Applicable Standards:** STD-003, STD-007, STD-009, STD-012  
**Related ADRs:** ADR-F001, ADR-F003, ADR-F006, ADR-I001  
**Supersedes:** None  
**Superseded By:** None  
**Decision Record:** Project Owner approval recorded on 2026-07-24

## 1. Context

FRS-001 requires a canonical repository and a dependency policy before source work begins.

Falcon must remain maintainable, reviewable, reproducible, and capable of replacing external technology without allowing vendor-specific types or behavior to become part of its stable meaning.

## 2. Decision Drivers

- one auditable source of truth for Foundation implementation;
- clear ownership and dependency direction;
- minimal third-party attack and supply-chain surface;
- deterministic restoration and reproducible builds;
- prevention of external-type leakage across boundaries;
- replaceability of libraries, products, services, and providers;
- explicit provenance, license, vulnerability, and update controls; and
- exclusion of financial capability from FRS-001.

## 3. Higher-Authority Constraints

This decision remains subordinate to the Vision, Constitution, approved Contracts and Specifications, FRS-001, and the accepted Foundation ADRs.

A repository structure or dependency cannot create authority, bypass FIL, weaken isolation, alter an approved Contract, or introduce financial behavior into Foundation.

## 4. Alternatives Considered

### Separate repositories for every Foundation concern

This was rejected for FRS-001 because it would increase version coordination, compatibility management, provenance, and verification burden before independent release ownership is required.

### Unrestricted direct use of external libraries

This was rejected because external types and behaviors could spread through Falcon, make replacement expensive, and allow dependency choices to become de facto architecture.

### Mandatory Adapter for every external-library call

This was rejected because purely local, low-consequence uses can produce ceremonial abstraction without meaningful isolation. Such exceptions remain narrow, documented, and reviewable.

### One canonical repository with governed dependency boundaries

This was selected because it provides one review and verification boundary while preserving logical separation, dependency direction, and future extraction.

## 5. Decision

### 5.1 Canonical Repository

FRS-001 SHALL be implemented in one canonical Foundation repository and one canonical .NET solution.

The solution SHALL be divided into clearly owned projects whose dependency direction follows Falcon authority and boundary rules. Stable Falcon Contracts and FIL definitions SHALL remain separated from implementation-specific concerns.

Repository co-location does not grant trust, authority, visibility, or permission to cross a Falcon boundary.

### 5.2 BCL First

The .NET Base Class Library (BCL) SHALL be preferred before introducing any external dependency when it satisfies the requirement without weakening security, reliability, maintainability, or conformance.

### 5.3 Dependency Admission

Every external dependency SHALL have:

- a defined purpose and accountable owner;
- an approved source and verified provenance;
- an exact, non-floating version;
- compatible license evidence;
- security, vulnerability, and supply-chain review;
- documented operational and security consequences;
- an update and removal policy; and
- a demonstrated absence of unauthorized financial scope.

No dependency is admitted solely for convenience or popularity.

### 5.4 External Dependency Isolation

An external dependency SHALL be isolated behind a Falcon-owned Contract or Adapter when:

- it affects Falcon behavior or domain-independent Core logic;
- its types, errors, lifecycle, or semantics could escape its owning location;
- it performs security-sensitive, persistence, transport, identity, cryptographic, or operational work; or
- replacement would otherwise require changes outside the owning boundary.

A simple local use MAY be exempt only when it remains contained, documented, reviewable, and creates no architectural coupling. The exemption SHALL identify its scope and justification.

### 5.5 No Boundary Leakage

External-library types, interfaces, exceptions, models, and vendor-specific semantics SHALL NOT cross a Falcon layer or authority boundary.

Layers and components SHALL interact through Falcon-owned Contracts. Kernel and Core policy SHALL NOT directly depend on Infrastructure-specific libraries or provider implementations.

Translation into and out of external representations belongs to the Adapter or owning boundary.

### 5.6 Replaceability

An external dependency SHALL be replaceable without changing Falcon Core logic, stable Contracts, or layers that do not own that dependency.

Tests SHALL verify Falcon behavior at the Falcon-owned boundary rather than encode a vendor implementation as system meaning.

### 5.7 No Vendor Lock-in

Falcon SHALL NOT depend on a vendor, library, product, or service in a manner that makes replacement require redesign of the system.

Any necessary exception SHALL be explicitly documented and approved with:

- its necessity and bounded scope;
- affected Falcon boundaries;
- security, continuity, cost, and migration risks;
- a viable exit strategy; and
- a review or expiry condition.

### 5.8 Version, Provenance, and Update Control

Dependency versions and approved sources SHALL be centrally controlled and locked for deterministic restoration. Floating versions are prohibited.

The repository SHALL produce and retain a dependency inventory, source and version record, license record, and software bill of materials for released artifacts.

Dependency changes SHALL be reviewed, scanned, tested, and promoted through the same evidence-bearing process as source changes. Updates SHALL NOT enter an approved baseline automatically.

### 5.9 Least Privilege

An external dependency receives no inherent permission to access the network, file system, processes, secrets, credentials, configuration, or privileged Falcon state.

Any required access SHALL be explicit, minimal, boundary-controlled, and verifiable.

### 5.10 Foundation Scope

FRS-001 SHALL contain no trading, broker, exchange, live-market-data, portfolio, order, position, or real-capital dependency.

This decision does not authorize source implementation, external connectivity, package installation, plugin admission, or production deployment.

## 6. Consequences

- Foundation has one canonical implementation and verification boundary.
- Falcon meaning remains expressed through Falcon-owned Contracts.
- External technology remains subordinate and replaceable.
- Dependency admission and upgrades require evidence and ownership.
- Adapters add justified boundary code where external behavior matters.
- Narrow local exceptions are possible but cannot remain invisible.
- Vendor-specific convenience cannot override long-term independence.

## 7. Risks and Mitigations

- **Excessive abstraction:** require Adapters where coupling or consequence exists; permit documented, contained local exceptions.
- **Hidden dependency leakage:** enforce boundary analysis, project dependency rules, and Contract-level tests.
- **Supply-chain compromise:** use approved sources, exact versions, provenance, inventory, scanning, and controlled promotion.
- **Stale dependencies:** assign ownership and review updates without automatic baseline promotion.
- **Vendor lock-in by accumulation:** require replacement boundaries and an approved exit strategy for any exception.
- **Repository becoming structurally monolithic:** preserve project ownership and dependency direction; permit future extraction through a separate ADR when justified.

## 8. Compatibility and Transition

This is the first Falcon1 repository and dependency decision. A move to multiple repositories, a material change in dependency admission, or an approved vendor-lock-in exception requires an Accepted ADR or higher-authority record as applicable.

## 9. Conformance Evidence

Conformance requires:

- one identified canonical repository and solution;
- an explicit project dependency map;
- automated detection of prohibited dependency direction and boundary leakage;
- exact dependency versions and approved package sources;
- dependency inventory, provenance, licenses, and software bill of materials;
- vulnerability and supply-chain scan results;
- documented external-dependency Adapters and local-use exceptions;
- replacement tests for consequential dependency boundaries;
- documented and approved exit plans for any vendor-lock-in exception; and
- proof that no financial dependency or external financial path exists.

## 10. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner and current Constitutional Authority | Accepted | رائد عموره — “موافق على سياسة المستودع والاعتماديات بصيغتها النهائية.” | 2026-07-24 |
