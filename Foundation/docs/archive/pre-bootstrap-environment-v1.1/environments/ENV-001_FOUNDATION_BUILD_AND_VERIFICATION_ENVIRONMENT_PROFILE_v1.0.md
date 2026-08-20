# ENV-001 — Foundation Build and Verification Environment Profile

**Version:** 1.0  
**Status:** Approved  
**Profile Snapshot Date:** 2026-07-25  
**Owner:** Falcon Foundation Governance  
**Governing Authority:** Falcon Vision; Falcon Constitution; GOV-001; ADR-F001; ADR-I007  
**Applicable Baseline:** FRS-001; BLD-001 v1.0; PIPE-001 v1.0; TRC-001 v1.0  
**Applicable Specifications:** SYS-001; SYS-007; SYS-008; SEC-001; SEC-002; FCE-001  
**Related Documents:** CRY-001; IDN-001; TIM-001; DESIGN-SEC-001; VPL-000 through VPL-008  
**Supersedes:** None  
**Superseded By:** None  
**Implementation Authority:** Not Granted

## 1. Purpose

This Profile defines the exact classes, constraints, identity, isolation, trust, capabilities, data, network, time, evidence, and lifecycle required of environments used to build and verify Falcon Foundation.

It ensures that:

- developer machines are not build authorities;
- mutable cloud labels are not environment identities;
- Windows and Linux are independently verified;
- build and verification cannot reach financial systems;
- external acquisition is separated from isolated execution;
- tools and operating systems are content-identified;
- time and secrets are governed;
- evidence survives disposable runners; and
- environment drift cannot silently alter a result.

## 2. Scope

ENV-001 governs:

- dependency-acquisition environments;
- Windows build and verification environments;
- Linux build and verification environments;
- independent reproducibility environments;
- evidence export;
- simulated internal networks;
- local PostgreSQL verification;
- synthetic test identities and secrets;
- time-source capabilities;
- environment manifests;
- isolation;
- cleanup; and
- environment change.

## 3. Non-Scope

ENV-001 does not:

- select tools beyond BLD-001;
- define Pipeline semantics;
- replace TRC-001;
- authorize implementation;
- authorize a production environment;
- authorize financial connectivity;
- authorize market data;
- authorize real credentials;
- authorize a Crypto, Identifier, Time, Build, or Gate Profile;
- require one cloud, hypervisor, CI provider, or hardware vendor; or
- claim that an environment currently exists.

## 4. Foundational Rule

> **An environment is a governed build input, not background infrastructure.**

No environment is trusted because it is:

- local;
- corporate;
- cloud-hosted;
- newly created;
- patched;
- vendor-managed;
- isolated by assertion; or
- previously successful.

Trust requires identity, provenance, integrity, capability evidence, scoped validity, and acceptance.

## 5. Environment Classes

| Environment Class | Purpose | External network | Artifact authority |
|---|---|---|---|
| `ACQUISITION` | Acquire and verify approved tools and dependencies | Allowlisted and monitored | Cannot produce official candidate |
| `WINDOWS_BUILD_VERIFY` | Build and verify Windows candidate | Denied after sealed input import | May produce governed Windows candidate |
| `LINUX_BUILD_VERIFY` | Build and verify Linux candidate | Denied after sealed input import | May produce governed Linux candidate |
| `WINDOWS_REPRODUCIBILITY` | Independent Windows rebuild | Denied | Cannot replace original candidate |
| `LINUX_REPRODUCIBILITY` | Independent Linux rebuild | Denied | Cannot replace original candidate |
| `SECURITY_INVESTIGATION` | Bounded analysis of quarantined material | Denied by default | Cannot produce promotable candidate |
| `EVIDENCE_REVIEW` | Review immutable exported evidence | No candidate execution | Cannot mutate evidence or candidate |

One runtime instance SHALL have exactly one Environment Class.

An instance SHALL NOT change class during its lifecycle.

## 6. Profile Lifecycle

| State | Meaning |
|---|---|
| `DRAFT` | Proposed profile with no execution authority |
| `APPROVED` | Profile definition approved for preparation and verification |
| `ACTIVE` | Exact activation manifest accepted for governed use |
| `SUSPENDED` | New use prohibited pending investigation |
| `DEPRECATED` | Transition permitted under an Approved plan |
| `RETIRED` | New use prohibited; historical evidence retained |
| `FORBIDDEN` | Use prohibited |

Profile Approval does not activate an image or runner.

## 7. Foundation Environment Profiles

| Profile ID | Environment Class | Operating-system family | Architecture | Lifecycle |
|---|---|---|---|---|
| `FALCON-ENV-ACQUIRE-1` | `ACQUISITION` | Governed Linux or Windows acquisition host | x86-64 | `PROPOSED` |
| `FALCON-ENV-WIN-BUILD-1` | `WINDOWS_BUILD_VERIFY` | Windows Server 2025 LTSC | x86-64 | `PROPOSED` |
| `FALCON-ENV-LNX-BUILD-1` | `LINUX_BUILD_VERIFY` | Ubuntu Server 24.04.4 LTS | AMD64 | `PROPOSED` |
| `FALCON-ENV-WIN-REPRO-1` | `WINDOWS_REPRODUCIBILITY` | Windows Server 2025 LTSC | x86-64 | `PROPOSED` |
| `FALCON-ENV-LNX-REPRO-1` | `LINUX_REPRODUCIBILITY` | Ubuntu Server 24.04.4 LTS | AMD64 | `PROPOSED` |
| `FALCON-ENV-EVIDENCE-1` | `EVIDENCE_REVIEW` | Provider-independent governed review host | x86-64 | `PROPOSED` |

No initial profile is `ACTIVE`.

## 8. Why Virtual Machines Are the Foundation Boundary

Foundation Windows and Linux verification SHALL execute in disposable virtual machines or an independently proven isolation technology with equivalent operating-system fidelity.

Virtual-machine isolation is selected initially because verification must observe:

- operating-system behavior;
- process identity;
- filesystem permissions;
- service lifecycle;
- local IPC;
- clock and monotonic behavior;
- restart behavior;
- database behavior;
- key-custody boundaries; and
- shutdown and recovery.

Containers MAY be used inside an Approved environment for bounded dependencies, but SHALL NOT replace required host-level Windows or Linux verification unless separately proven and approved.

The hypervisor or provider remains replaceable.

## 9. Operating-System Baselines

### 9.1 Windows

| Property | Required value |
|---|---|
| Product | Windows Server 2025 |
| Servicing Channel | Long-Term Servicing Channel |
| Edition | Standard Core or Datacenter Core as declared by Activation Manifest |
| Architecture | x86-64 |
| Base OS family | Build 26100 |
| GUI | Not required; Server Core preferred |
| Update state | Exact cumulative update and resulting build required in Activation Manifest |
| Image identity | Exact file or image digest required |
| Source | Microsoft official distribution or a separately approved verified enterprise mirror |

The OS family is stable. The exact cumulative security update is activation-bound.

An Activation Manifest SHALL NOT use an image older than the applicable Approved security baseline.

### 9.2 Linux

| Property | Required value |
|---|---|
| Distribution | Ubuntu Server |
| Release | 24.04.4 LTS, Noble Numbat |
| Architecture | AMD64 |
| Image type | Server install or governed minimal server image |
| GUI | Prohibited |
| Package state | Exact package closure required in Activation Manifest |
| Image identity | Exact ISO, cloud-image, or root-image digest required |
| Source | Canonical official distribution or a separately approved verified enterprise mirror |

The release number alone is insufficient. Exact installed package versions and image digest are mandatory.

### 9.3 Servicing Rule

The profile SHALL NOT embed a permanently frozen security patch as timeless policy.

Instead, every Activation Manifest SHALL bind:

- OS product and release;
- exact build or package closure;
- exact update identifiers;
- installation source;
- image digest;
- publisher signature where available;
- vulnerability assessment;
- activation time;
- validity period; and
- superseded activation.

A servicing change creates a new Activation Manifest and requires environment re-verification.

## 10. Shell and Runner Interface

PowerShell `7.6.3` is the initial cross-platform orchestration shell candidate.

Its lifecycle under ENV-001 is `APPROVED`, not `ACTIVE`.

The exact Windows and Linux PowerShell distribution files SHALL be:

- separately content-identified;
- acquired from the official PowerShell release;
- signature-verified where applicable;
- admitted under BLD-001;
- imported through the sealed dependency bundle; and
- bound in the Activation Manifest.

Operating-system-provided shells MAY bootstrap the approved runner only.

They SHALL NOT redefine Pipeline semantics.

No user profile, shell profile, startup script, alias, module path, or interactive preference may influence governed execution.

## 11. Environment Identity

Every runtime instance SHALL receive an Environment Instance ID through the Falcon Identifier Provider Contract.

It SHALL bind:

- Environment Profile ID and version;
- Activation Manifest ID;
- Environment Class;
- OS identity;
- image digest;
- update or package closure;
- architecture;
- virtualization technology and version;
- host or provider identity;
- virtual hardware profile;
- firmware mode;
- secure-boot state where supported;
- storage identity;
- network policy;
- toolchain identity;
- time profile;
- Runtime Epoch ID;
- creation identity;
- creation Time Observation;
- destruction outcome; and
- parent image provenance.

Instance identity does not establish validity.

## 12. Activation Manifest

An Activation Manifest is an immutable Trust Object that converts one Approved logical profile into one exact executable environment candidate.

It SHALL contain:

- Activation Manifest ID;
- profile identity;
- lifecycle;
- intended Build Intents;
- exact OS artifact;
- exact OS update state;
- exact package closure;
- exact tool closure;
- exact BLD-001 baseline;
- exact virtual hardware;
- exact network rules;
- exact storage rules;
- exact identity and access rules;
- exact time-source configuration;
- exact logging and evidence export;
- exact data set identities;
- exact secret class;
- exact hardening policy;
- image-construction provenance;
- vulnerability state;
- known limitations;
- canonical digest;
- approval;
- validity period; and
- supersession.

Mutable image tags are prohibited.

## 13. Virtual Hardware Profile

Initial minimum per build or verification instance:

| Resource | Minimum |
|---|---|
| Virtual CPU | 4 x86-64 logical processors |
| Memory | 16 GiB |
| System disk | 100 GiB |
| Evidence staging disk | 20 GiB, separate logical volume |
| Network adapters | One policy-controlled adapter or none |
| GPU | None |
| USB or removable media | Prohibited |
| Shared clipboard | Prohibited |
| Shared host folders | Prohibited |
| Audio, camera, and location | Prohibited |

Resource changes SHALL be recorded.

Performance results are valid only for their declared virtual hardware.

## 14. Provisioning

Instances SHALL be:

- created from a content-identified base image;
- provisioned by a versioned, reviewable definition;
- non-interactive;
- free of inherited user state;
- free of developer tools not admitted by BLD-001;
- free of credentials not declared by the profile;
- verified before input admission;
- single-execution by default; and
- destroyed after governed export and cleanup verification.

A preexisting developer workstation SHALL NOT be transformed into a governed runner.

Snapshot restore MAY accelerate creation only when the snapshot itself is content-identified, immutable, provenance-preserved, vulnerability-assessed, and included in the Activation Manifest.

## 15. Trust Bootstrap

Before accepting sealed inputs, the environment SHALL verify:

- base-image digest;
- provisioning-definition digest;
- Activation Manifest;
- OS identity;
- update state;
- toolchain inventory;
- network policy;
- storage isolation;
- time capability;
- logging capability;
- evidence export path;
- synthetic-data boundary;
- absence of production credentials;
- absence of financial connectivity;
- authority and jurisdiction; and
- current profile lifecycle.

Failure prevents governed execution.

The instance SHALL NOT conclusively verify itself. Independent control-plane or attested evidence is required for material identity Claims.

## 16. Network Zones

| Zone ID | Purpose | External access |
|---|---|---|
| `NET-ACQUIRE` | Allowlisted acquisition | Approved sources only |
| `NET-SEALED` | Official build and deterministic verification | None |
| `NET-SIM` | Simulated internal services and fault injection | Isolated private segment only |
| `NET-EVIDENCE` | Controlled evidence export | Declared receiver only |
| `NET-DENY` | Quarantine and security investigation | None |

No route may exist to:

- brokers;
- exchanges;
- financial institutions;
- production Falcon;
- production databases;
- production message buses;
- market-data providers;
- notification systems capable of operational action;
- corporate credential stores not explicitly required;
- public Internet during isolated execution; or
- any capital-bearing system.

DNS resolution outside the active allowlist is prohibited.

## 17. Acquisition Network

`FALCON-ENV-ACQUIRE-1` SHALL:

- use a default-deny egress policy;
- allow only Approved source endpoints;
- verify TLS and source identity;
- record requests and responses needed for provenance;
- download exact versions only;
- verify digests and signatures;
- evaluate licenses and vulnerabilities;
- quarantine contradictions;
- construct a sealed content-addressed bundle; and
- transfer only the sealed bundle to build environments.

The acquisition environment SHALL NOT produce the official candidate.

## 18. Isolated Execution Network

Official build, Contract, security, integration, fault, VPL, and reproducibility execution SHALL begin in `NET-SEALED`.

External egress and ingress SHALL be denied.

When a test requires communication:

- it SHALL use `NET-SIM`;
- every endpoint SHALL belong to the same isolated verification case;
- endpoint identities SHALL be synthetic and declared;
- Internet forwarding SHALL be impossible;
- message capture SHALL be evidence;
- reorder, duplicate, delay, loss, and interruption SHALL be controllable; and
- teardown SHALL be verified.

A successful external connection attempt is a Pipeline failure.

## 19. Evidence Export

Evidence export SHALL:

- occur only after evidence identity and digest are established;
- use an explicitly authorized receiver;
- preserve canonical bytes;
- preserve directory and manifest relationships;
- be read-only at the receiver where practical;
- record sender and receiver identities;
- record Time Observations and Clock Quality;
- verify transferred digests;
- prevent return traffic from introducing build inputs; and
- fail explicitly on partial or ambiguous transfer.

Evidence export is not evidence acceptance.

If export fails, the environment SHALL preserve the evidence volume where safely possible and mark the execution incomplete.

## 20. Filesystem and Storage

Each instance SHALL have isolated:

- system storage;
- source input;
- dependency bundle;
- build workspace;
- test workspace;
- database storage;
- secret storage;
- evidence staging; and
- temporary storage.

Rules:

- host filesystem mounts are prohibited;
- user home state SHALL NOT be imported;
- path identity SHALL be deterministic where build output depends on it;
- case-sensitivity assumptions SHALL be tested;
- Windows and Linux path semantics SHALL remain distinct;
- temporary material SHALL be bounded;
- disk-full conditions SHALL be tested;
- evidence storage SHALL not share mutable candidate paths;
- deletion SHALL occur only after export verification; and
- destruction evidence SHALL be preserved.

## 21. Source and Dependency Input

The environment SHALL accept only:

- one immutable Source Revision bundle;
- one immutable dependency bundle;
- one Pipeline Definition;
- one Evidence Requirement Set;
- one Effective Build Configuration;
- one BLD-001 baseline;
- one TRC-001 snapshot;
- one Gate Profile; and
- declared synthetic test data.

Every input SHALL have:

- identity;
- digest;
- provenance;
- authority;
- scope;
- schema where applicable; and
- admission result.

An input mismatch requires a new Pipeline Execution or explicit failure.

## 22. Data Policy

Only:

- synthetic data;
- generated test fixtures;
- public non-sensitive standards vectors;
- content-identified open test corpora; and
- explicitly approved anonymized non-financial samples

may enter Foundation verification.

Prohibited data includes:

- real customer data;
- real account data;
- portfolio data;
- order data;
- position data;
- broker data;
- private market feeds;
- personal data not required for a governed security case;
- production logs;
- production configuration;
- real secrets; and
- any data that could create financial consequence.

Synthetic data SHALL be clearly labeled and incapable of being mistaken for production authority.

## 23. Identity and Access

Every actor, service, tool, environment, and evidence receiver SHALL have a distinct declared identity.

Rules:

- least privilege;
- default deny;
- no shared human accounts;
- no interactive administrator use during governed execution;
- no inherited host identity;
- no production federation;
- no production certificates;
- no production API tokens;
- bounded service identities;
- short validity;
- explicit revocation; and
- complete access evidence.

Host or CI identity SHALL NOT silently become Falcon authority.

## 24. Secrets

Only synthetic or verification-specific secrets are permitted.

They SHALL:

- be generated or issued for one Pipeline Execution or bounded profile;
- use declared purpose and domain;
- remain outside source and logs;
- be delivered through the Approved custody boundary;
- be inaccessible to unrelated stages;
- have short validity;
- be revoked after execution;
- be destroyed with evidence;
- never protect production value; and
- never be reused as a Falcon production root.

Environment-variable delivery is prohibited for material secrets unless a separately Approved profile proves equivalent protection.

No real cryptographic root is authorized by ENV-001.

## 25. Cryptographic Custody

Cryptographic verification SHALL use only a compatible Approved custody candidate under DESIGN-SEC-001.

The environment SHALL expose:

- provider identity;
- custody profile;
- root separation;
- allowed operations;
- non-exportability claims;
- key lifecycle;
- rotation behavior;
- revocation;
- failure behavior;
- evidence;
- cleanup; and
- independent restoration rules.

Because no custody profile is active, cryptographic execution remains blocked.

## 26. Time and Clock Quality

Every governed environment SHALL obtain time through the Falcon Time Provider Contract.

The Activation Manifest SHALL declare:

- Time Profile;
- Temporal Decision Profiles;
- source instances;
- source independence;
- source authentication;
- maximum uncertainty;
- verification age;
- resolution;
- leap behavior;
- holdover;
- Runtime Epoch rules;
- monotonic capabilities;
- fault-injection capability; and
- evidence export.

No single source establishes `VERIFIED` unless the active profile explicitly permits it with equivalent evidence.

Build output SHALL NOT depend on uncontrolled wall-clock time.

Time verification SHALL support:

- rollback;
- forward jump;
- uncertainty growth;
- contradiction;
- stale verification;
- loss of source;
- new Runtime Epoch;
- monotonic reset;
- leap-policy failure; and
- restoration.

Because no Time Profile is active, governed time-dependent execution remains blocked.

## 27. PostgreSQL Verification

PostgreSQL `18.4` SHALL run only as an isolated verification dependency.

Its instance SHALL have:

- exact distribution identity;
- exact package or image digest;
- isolated storage;
- synthetic credentials;
- synthetic data;
- no external route;
- declared encoding;
- declared locale;
- declared collation;
- UTC configuration;
- deterministic initialization material;
- bounded resources;
- startup evidence;
- health evidence;
- query and transaction evidence where required;
- fault controls;
- restart controls;
- cleanup evidence; and
- no architectural authority outside Persistence Contracts.

PostgreSQL success does not establish Falcon persistence success.

Verification SHALL include ambiguous commit, interruption, restart, reconciliation, duplicate-effect prevention, unavailable evidence, and storage exhaustion.

## 28. Toolchain Admission

The exact BLD-001 baseline SHALL be imported through the sealed bundle.

The environment SHALL reject:

- a different .NET SDK;
- SDK roll-forward;
- a different runtime patch;
- global NuGet sources;
- uncontrolled package caches;
- unapproved analyzers;
- local test adapters;
- mutable tool tags;
- undeclared PATH entries;
- user-installed modules;
- auto-update;
- telemetry that exports governed material; and
- any tool absent from the Activation Manifest.

Tool presence does not equal tool validity.

## 29. Telemetry and External Reporting

Operating-system, shell, SDK, database, security tool, crash reporter, package manager, and CI telemetry SHALL be:

- disabled where possible;
- blocked by network policy;
- declared where not removable;
- evaluated for sensitive output;
- included in the threat model; and
- proven unable to export governed material during isolated execution.

An update notifier SHALL NOT download or alter tools.

## 30. Logging and Evidence

Environment evidence SHALL include:

- instance and Activation Manifest identity;
- provisioning evidence;
- base-image and update identity;
- installed package inventory;
- tool inventory;
- process inventory;
- environment variables with secrets redacted and presence proven safely;
- network policy and flow evidence;
- filesystem and mount evidence;
- identity and access evidence;
- time and epoch evidence;
- resource state;
- isolation checks;
- financial-boundary checks;
- stage entry and exit;
- failures;
- export;
- cleanup; and
- destruction.

Redaction SHALL NOT destroy the ability to prove that prohibited secret exposure did not occur.

## 31. Reproducibility Separation

Reproducibility environments SHALL:

- be independently provisioned;
- use independently verified image instances;
- share no mutable workspace;
- share no mutable cache;
- receive the same content-identified inputs;
- use the same logical profile;
- use exact compatible activation manifests;
- produce independent evidence;
- be operated under separate execution identity; and
- compare results only after artifact identities exist.

The original builder SHALL NOT be the sole authority declaring reproducibility.

## 32. Provider and Hypervisor Independence

ENV-001 does not require a specific:

- cloud;
- CI service;
- hypervisor;
- hardware vendor;
- image-building product;
- network product; or
- secret store.

An Adapter MAY realize the profile only when it preserves:

- exact identity;
- isolation;
- capabilities;
- evidence;
- failure semantics;
- cleanup;
- authority separation; and
- portability.

Provider attestations are inputs to evaluation, not self-establishing truth.

## 33. Failure Policy

The environment SHALL fail admission or suspend execution when:

- image identity differs;
- OS update state differs;
- a package or tool differs;
- provisioning provenance is incomplete;
- network isolation is unproven;
- an external route exists;
- production identity or data is detected;
- time requirements are unproven;
- evidence storage is unavailable;
- disk, memory, or process limits invalidate the case;
- telemetry may export material;
- host sharing is detected;
- unauthorized interactive access occurs;
- cryptographic custody is inactive;
- PostgreSQL identity differs;
- cleanup cannot be verified; or
- a material Claim is challenged and unresolved.

Failure SHALL:

- preserve evidence;
- stop affected authority;
- quarantine candidate outputs;
- notify Health Monitoring where applicable;
- prevent promotion;
- record uncertainty;
- avoid fallback to a developer machine; and
- require a new or governed recovery path.

## 34. Cleanup and Destruction

After evidence export:

1. transferred evidence digests SHALL be verified;
2. candidate artifacts SHALL remain bound to their evidence;
3. synthetic identities and secrets SHALL be revoked;
4. processes SHALL terminate;
5. simulated networks SHALL be removed;
6. database storage SHALL be destroyed;
7. temporary and workspace storage SHALL be destroyed;
8. snapshots created for the execution SHALL be destroyed unless governed evidence requires retention;
9. environment destruction SHALL be verified independently; and
10. destruction evidence SHALL be added to the verification case.

Destruction failure produces a security incident and prevents silent reuse.

## 35. Environment Change

A change to any:

- OS release or update;
- base image;
- package;
- tool;
- provisioning definition;
- hypervisor behavior;
- virtual hardware;
- network rule;
- identity mechanism;
- secret mechanism;
- time source;
- storage;
- database;
- logging;
- evidence export; or
- cleanup behavior

creates a new Activation Manifest candidate.

Material change requires:

- impact analysis;
- TRC-001 update where applicable;
- threat review;
- vulnerability review;
- cross-platform verification;
- reproducibility verification;
- Approval; and
- explicit Activation.

## 36. Environment Requirements

- **ENV-001-REQ-001:** Every governed environment SHALL have one immutable Environment Class and one Environment Instance ID.
- **ENV-001-REQ-002:** Every executable environment SHALL bind to one Approved Profile and one exact Activation Manifest.
- **ENV-001-REQ-003:** Mutable image and runner labels SHALL be prohibited.
- **ENV-001-REQ-004:** Windows verification SHALL use the declared Windows Server 2025 LTSC family and exact activation-bound update.
- **ENV-001-REQ-005:** Linux verification SHALL use Ubuntu Server 24.04.4 LTS AMD64 and exact activation-bound package closure.
- **ENV-001-REQ-006:** Exact image, OS, package, tool, and configuration digests SHALL be recorded.
- **ENV-001-REQ-007:** Developer machines SHALL NOT produce governed candidates.
- **ENV-001-REQ-008:** Dependency acquisition SHALL remain separate from official build and verification.
- **ENV-001-REQ-009:** Official execution SHALL deny external network access.
- **ENV-001-REQ-010:** Simulated networks SHALL remain isolated from public, corporate, production, and financial systems.
- **ENV-001-REQ-011:** No production credential, identity, secret, configuration, log, or data SHALL enter Foundation execution.
- **ENV-001-REQ-012:** Only declared synthetic or explicitly Approved non-financial test data SHALL be used.
- **ENV-001-REQ-013:** Host folders, clipboard, removable media, and inherited user state SHALL be prohibited.
- **ENV-001-REQ-014:** Every input SHALL be immutable, content-identified, provenance-preserved, and admitted.
- **ENV-001-REQ-015:** The exact BLD-001 toolchain SHALL be enforced and alternatives rejected.
- **ENV-001-REQ-016:** PostgreSQL `18.4` SHALL remain isolated behind Falcon Persistence Contracts.
- **ENV-001-REQ-017:** Time SHALL be obtained only through the Falcon Time Provider Contract.
- **ENV-001-REQ-018:** Time-dependent execution SHALL remain blocked while no compatible Time Profile is active.
- **ENV-001-REQ-019:** Cryptographic execution SHALL remain blocked while no compatible custody profile is active.
- **ENV-001-REQ-020:** Material verification secrets SHALL be synthetic, purpose-bound, short-lived, and independently destroyed.
- **ENV-001-REQ-021:** Environment evidence SHALL survive disposable runner destruction.
- **ENV-001-REQ-022:** Evidence export SHALL preserve exact bytes, identities, provenance, and transfer verification.
- **ENV-001-REQ-023:** Reproducibility environments SHALL be independently provisioned without shared mutable state.
- **ENV-001-REQ-024:** Provider and hypervisor adapters SHALL NOT redefine Pipeline or evidence meaning.
- **ENV-001-REQ-025:** Environment failure SHALL quarantine outputs and prevent promotion.
- **ENV-001-REQ-026:** Cleanup and destruction SHALL be verified and evidenced.
- **ENV-001-REQ-027:** A material environment change SHALL create a new Activation Manifest.
- **ENV-001-REQ-028:** Environment validity SHALL be scoped, time-bounded, and independently accepted.
- **ENV-001-REQ-029:** No Profile or Activation Manifest SHALL become active without complete verification evidence.
- **ENV-001-REQ-030:** Approval of ENV-001 SHALL NOT activate an environment or authorize implementation.

## 37. Conformance Evidence

Conformance requires evidence that:

- a different image digest fails admission;
- a different Windows update or Linux package fails admission;
- mutable runner labels are rejected;
- developer-machine state cannot enter;
- host filesystem sharing is unavailable;
- external network attempts fail;
- simulated services cannot route externally;
- financial endpoints are unreachable;
- production credentials and data are absent;
- sealed inputs cannot be changed;
- a different SDK or package source fails;
- telemetry cannot export material;
- time rollback and uncertainty are controllable;
- inactive Time and custody profiles block affected execution;
- PostgreSQL failure and ambiguous commit are reproducible;
- evidence survives runner destruction;
- failed export produces incomplete status;
- cleanup revokes identities and destroys secrets;
- reproducibility runners share no mutable state;
- Windows and Linux evidence remains distinct;
- provider replacement preserves semantics; and
- no environment self-establishes its own trust.

## 38. Known Deliberate Blocks

At version 1.0:

- all Environment Profiles remain `PROPOSED`;
- no Activation Manifest exists;
- exact Windows cumulative update is not activated;
- exact Windows image digest is not recorded;
- exact Ubuntu image digest and package closure are not recorded;
- PowerShell `7.6.3` files and digests are not acquired;
- BLD-001 unresolved tools remain unresolved;
- no Time Profile is active;
- no cryptographic custody profile is active;
- no Identifier Provider is active;
- no environment authority is appointed;
- no Pipeline Gate Profile is active;
- no machine-readable TRC expansion exists; and
- no implementation authority exists.

These blocks prevent governed build or verification execution.

## 39. Required Before Activation

No Environment Profile becomes `ACTIVE` until:

1. ENV-001 is Approved;
2. BLD-001, PIPE-001, and TRC-001 remain Approved;
3. all mandatory BLD-001 tool entries are resolved;
4. exact OS images, updates, packages, tools, and digests are recorded;
5. image construction and provisioning provenance is complete;
6. network isolation is independently verified;
7. non-financial boundary is independently verified;
8. synthetic identity, data, and secret procedures are Approved;
9. compatible Identifier and Time Provider profiles are active;
10. compatible cryptographic custody is active for cryptographic stages;
11. evidence export and destruction are verified;
12. Windows and Linux positive and negative cases pass;
13. reproducibility separation passes;
14. a complete Root Verification Evidence Set is accepted;
15. an Activation Authority acts within declared jurisdiction; and
16. explicit implementation authority is granted.

## 40. Source Verification Record

The OS and shell selections in this proposed snapshot were checked on 2026-07-25 against:

- Microsoft Windows Server official release and lifecycle records;
- Canonical Ubuntu 24.04.4 official release records;
- Microsoft .NET 10 operating-system guidance; and
- the official PowerShell `7.6.3` release record.

Version verification does not admit executable bytes.

## 41. Foundational Rules

> **The environment is part of the evidence.**

> **Isolation is proven by controls and observations, not by a network label.**

> **A disposable runner may disappear; its evidence may not.**

> **No production identity, data, secret, route, or consequence enters Foundation verification.**

> **Exact environment—or no governed result.**

## 42. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner and current Constitutional Authority | Approved | GOV-022 | 2026-07-25 |

Approval approves:

- the environment model;
- Windows Server 2025 LTSC and Ubuntu Server 24.04.4 LTS families;
- PowerShell `7.6.3` as the non-active orchestration-shell candidate;
- acquisition, isolation, network, data, identity, secret, time, PostgreSQL, evidence, reproducibility, cleanup, and change rules; and
- the deliberate activation blocks.

It does not:

- activate an Environment Profile;
- create or download an image;
- install PowerShell or another tool;
- activate time or cryptographic custody;
- execute a Pipeline;
- authorize implementation;
- authorize promotion;
- authorize production; or
- authorize financial activity.
