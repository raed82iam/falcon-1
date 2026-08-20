# BLD-001 — Foundation Toolchain and Build Baseline Catalog

**Identifier:** BLD-001  
**Version:** 1.1  
**Status:** Approved  
**Effective Date:** 2026-07-25  
**Approval and Activation Record:** GOV-033  
**Amendment Package:** AMD-003  
**Owner:** Falcon Foundation Governance  
**Governing Authority:** ADR-I008; AMD-003; IMP-001 v1.2; CON-010 v1.1; CON-012; CON-020; CON-021; VPL-BST-000 through VPL-BST-008  
**Implementation Authority:** Not Granted
**Supersedes:** BLD-001 v1.0  
**Superseded By:** None

## 1. Purpose

This Catalog divides BLD-001 into separately governed preparation, enabling-provider candidate, and Foundation build baselines.

It permits exact tool acquisition and candidate preparation without treating acquired tools, prepared environments, or executable candidates as active Falcon build authority.

## 2. Preserved Decisions

Unless explicitly amended here, BLD-001 v1.0 remains controlling for:

- `.NET 10` and `net10.0`;
- C# 14 without preview features;
- exact SDK, runtime, compiler, MSBuild, NuGet, analyzer, test, SBOM, and PostgreSQL values;
- exact versions, sources, digests, inventories, lock files, and offline inputs;
- BCL-first and dependency-governance rules;
- Adapter and layer-boundary isolation;
- no vendor lock-in;
- deterministic and reproducible build rules;
- SBOM, provenance, evidence, and Trust Object rules;
- Windows and Linux verification;
- absence of financial connectivity, data, credentials, and capital authority; and
- the deliberate blocks for tools whose exact implementations are not yet admitted.

No version or digest is silently changed by this amendment.

## 3. Baseline Classes

BLD-001 v1.1 SHALL define:

| Baseline ID | Purpose | Initial lifecycle | Maximum authority |
|---|---|---|---|
| `FALCON-BUILD-PREPARATION-1` | acquire, verify, inventory, and bundle exact tools and dependencies | `APPROVED`, not `ACTIVE` | Foundation Preparation Authority |
| `FALCON-BUILD-ENABLING-PROVIDERS-1` | build only enumerated enabling-provider candidates and required bootstrap primitives | `APPROVED`, not `ACTIVE` | Enabling-Provider Candidate Authority |
| `FALCON-BUILD-FOUNDATION-1` | build the bounded FRS-001 Foundation implementation | remains `APPROVED`, not `ACTIVE` | Foundation Implementation Authority |

Each baseline SHALL have a separate identity, Activation Manifest, Environment Profile, Gate Profile, evidence case, Activation Decision, scope, and revocation path.

Activation of one baseline SHALL NOT activate another.

## 4. Build Intent

Every Pipeline or bootstrap-harness execution SHALL declare exactly one Approved Build Intent before execution.

The initial Build Intent registry is:

| Build Intent | Permitted baseline | Purpose | Promotable |
|---|---|---|---|
| `PREPARATION_BUILD` | Preparation | acquire, verify, inventory, and bundle | No |
| `PROVIDER_CANDIDATE_BUILD` | Enabling Providers | create exact test candidates | No |
| `VERIFICATION_BUILD` | applicable verified subject | execute bounded verification | No, unless a later policy explicitly defines an eligible artifact |
| `DEVELOPER_BUILD` | declared developer profile | local feedback only | No |
| `RELEASE_CANDIDATE_BUILD` | Foundation | produce an exact promotion candidate | Eligible only after all Gates |
| `HOTFIX_BUILD` | separately governed baseline | bounded urgent correction | No automatic promotion |
| `EMERGENCY_RECOVERY_BUILD` | separately governed recovery profile | containment or recovery artifact | No automatic promotion or operation |

The declared Build Intent SHALL determine the applicable Evidence Requirement Set and Gate Profile.

A Build Intent SHALL NOT:

- waive a Gate;
- expand an Authority Instrument;
- change an environment class;
- convert a candidate to active;
- make evidence complete;
- grant promotion; or
- grant operational or financial authority.

## 5. Preparation Build

`FALCON-BUILD-PREPARATION-1` MAY:

- acquire only exact cataloged or explicitly proposed tools;
- verify publisher evidence, signatures, digests, licenses, and vulnerabilities;
- record exact inventories;
- construct content-identified offline bundles;
- prepare candidate runner images;
- execute non-behavioral capability probes;
- produce candidate Manifests; and
- produce `BOOTSTRAP_EXTERNAL` evidence.

It SHALL require:

- a valid Foundation Preparation Authority Instrument;
- a `PREPARATION` CON-020 context;
- an admitted preparation environment;
- allowlisted acquisition sources;
- external identity and time;
- independent digest verification;
- complete provenance and custody; and
- VPL-BST-001 and VPL-BST-002.

It SHALL NOT compile or package general Falcon behavior.

## 6. Enabling-Provider Candidate Build

`FALCON-BUILD-ENABLING-PROVIDERS-1` MAY build only the exact subjects enumerated by IMP-001 v1.2 Stage 0B and its Authority Instrument.

It SHALL require:

- an active applicable Preparation baseline or independently verified equivalent inputs;
- a valid Enabling-Provider Candidate Authority Instrument;
- a `CANDIDATE_PROVIDER_VERIFY` CON-020 context;
- exact source revision and candidate subject list;
- synthetic-only data and security material;
- content-identified offline dependencies;
- candidate and independent-control evidence;
- explicit non-operational artifact marking; and
- applicable VPL-BST-003, VPL-BST-004, and VPL-BST-005 execution.

Candidate artifacts SHALL:

- carry `CANDIDATE` lifecycle;
- identify their candidate environment and Build Intent;
- remain non-promotable and non-operational;
- never protect production material;
- never serve as trusted inputs to their own completeness or Activation decision; and
- be quarantined or destroyed according to the evidence case after testing.

## 7. Foundation Build

`FALCON-BUILD-FOUNDATION-1` SHALL require:

- active enabling Provider Profiles;
- an active `FOUNDATION_BUILD_VERIFY` Environment Profile;
- active exact Build Baseline;
- active Pipeline Definition and Gate Profile;
- authoritative machine-readable TRC expansion;
- a valid Foundation Implementation Authority Instrument;
- the exact Approved CON-001 through CON-021 baseline;
- exact offline tool and dependency bundles;
- complete Build Intent-specific Evidence Requirement Set; and
- current Guardian, authority, security, and trust state.

Bootstrap identity, time, custody, tools, or evidence mechanisms SHALL NOT substitute for active Falcon Providers after this boundary.

## 8. Tool Acquisition Boundary

Tool acquisition is classified as Preparation, not Falcon behavior implementation.

Network use is permitted only in the declared acquisition boundary and SHALL be:

- allowlisted;
- isolated from governed compilation and testing;
- attributable;
- recorded with external identity and time;
- followed by signature, digest, license, and vulnerability verification;
- converted into a sealed offline bundle; and
- independently checked before use.

An acquired item is not admitted merely because acquisition succeeded.

Governed candidate and Foundation builds SHALL fail when attempting undeclared acquisition.

## 9. Tool Identity and Digest Completion

BLD-001 v1.1 SHALL preserve the rule that every artifact-affecting tool requires exact content identity before Activation.

For each tool, dependency, runner image, scanner, collector, generator, signer, database distribution, and Adapter, the Activation case SHALL include:

- Tool ID;
- exact semantic version where applicable;
- original file or package identity;
- size;
- SHA-256 digest or stronger profile-required digest;
- publisher identity and signature result;
- source and immutable locator where available;
- license disposition;
- vulnerability disposition;
- purpose and permitted Build Intents;
- supported Environment Profiles;
- capability evidence;
- replacement and rollback rule;
- lifecycle; and
- responsible authority.

An unresolved mandatory tool remains a blocking condition for every Build Intent that requires it.

It SHALL NOT block a narrower Preparation action that exists specifically to identify and evaluate that tool, when the Authority Instrument and Gate Profile permit that action.

## 10. Bootstrap Harness

Before PIPE-001 is active, a bootstrap harness MAY:

- provision an exact candidate environment;
- import sealed inputs;
- invoke declared acquisition, build, or verification commands;
- collect immutable raw evidence;
- export evidence;
- compare digests; and
- stop execution.

The harness SHALL NOT:

- claim PIPE-001 conformance;
- create governing Pipeline meaning;
- alter the Evidence Requirement Set after execution starts;
- evaluate its own promotion readiness;
- weaken a Gate;
- activate itself;
- produce a promotable FRS-001 artifact; or
- replace required independent authorities.

The harness SHALL later enter VPL-BST-007 verification and Activation.

## 11. Evidence Origin

Preparation and candidate evidence SHALL conform to CON-008 v1.1 and CON-021.

Evidence SHALL distinguish:

- external acquisition and runner observations;
- candidate-produced build observations;
- independent verification observations;
- Falcon-native evidence after Provider Activation;
- Derived Evaluations;
- completeness decisions; and
- Activation decisions.

Rebuilding after Provider Activation SHALL NOT rewrite or reclassify historical bootstrap evidence.

## 12. Candidate and Active Inputs

A governed execution SHALL accept only inputs whose lifecycle is permitted by its Build Intent.

| Input lifecycle | Preparation | Provider Candidate | Foundation |
|---|---|---|---|
| `BOOTSTRAP_EXTERNAL` | permitted when declared | permitted as external control | prohibited as trusted runtime dependency |
| `CANDIDATE` | permitted as subject | permitted as subject | prohibited as active dependency |
| `ACTIVE` | permitted when applicable | permitted when applicable | required where the Gate Profile declares it |
| `SUSPENDED`, `REVOKED`, `RETIRED`, `FORBIDDEN` | prohibited except isolated authorized analysis | prohibited except isolated authorized analysis | prohibited |
| unknown or conflicted | restrictive | restrictive | prohibited |

Candidate or external input SHALL NOT satisfy an active dependency requirement.

## 13. Build Manifest

Every build SHALL produce or reference a CON-010 v1.1 Manifest containing:

- Manifest class;
- Build Intent;
- Build Scope;
- Authority Instrument;
- source revision;
- toolchain and dependency bundle;
- Environment Profile;
- external bootstrap source identities where applicable;
- Provider Profile identities and states;
- Pipeline or harness identity;
- Evidence Requirement Set;
- produced artifact identities and digests;
- evidence-set references;
- lifecycle;
- constraints;
- non-authorities; and
- integrity protection.

A Preparation or Candidate Manifest SHALL NOT be represented as a Foundation Baseline or Release Manifest.

## 14. Failure and Stop Rules

In addition to BLD-001 v1.0 failure rules, execution SHALL stop when:

- Build Intent is absent, unknown, changed after start, or inconsistent with authority;
- the Authority Instrument is missing, expired, revoked, or exceeded;
- the CON-020 context is absent or mismatched;
- a candidate subject or tool differs from the Manifest;
- bootstrap and Falcon evidence or identity are confused;
- a candidate is treated as active;
- active Provider state becomes untrustworthy;
- production or financial material or connectivity appears;
- synthetic-material isolation fails;
- the bootstrap harness attempts Pipeline, completeness, Activation, or promotion authority;
- evidence origin cannot be established; or
- a required VPL-BST Gate is not `PASS`.

Failure SHALL preserve evidence and SHALL NOT trigger local-tool, global-cache, online, weak-security, or later-stage fallback.

## 15. Activation Sequence

The build baselines SHALL be considered in this order:

1. verify and activate the minimum `PREPARATION` environment;
2. complete exact tool and dependency identities;
3. verify `FALCON-BUILD-PREPARATION-1`;
4. activate only the exact Preparation Build baseline;
5. produce and verify enabling-provider candidates;
6. activate required enabling Provider Profiles;
7. verify and activate applicable Foundation build environments;
8. verify and activate `FALCON-BUILD-FOUNDATION-1`;
9. verify and activate the machine-readable trace expansion;
10. verify and activate PIPE-001 and applicable Gate Profile; and
11. consider Foundation Implementation Authority separately.

An order change requires an Approved dependency analysis proving equivalent protection and no circular trust.

## 16. Deliberate Blocks

The unresolved items in BLD-001 v1.0 remain visible.

They SHALL be evaluated by affected scope:

- exact scanners, coverage collector, provenance generator, signer, runner images, package digests, and admission evidence block every Build Intent that requires them;
- unresolved Foundation-only tools do not prohibit a narrower authorized Preparation action used to select and verify them;
- no unresolved mandatory capability may be silently marked optional; and
- no baseline becomes `ACTIVE` until all of its mandatory blocks are resolved.

## 17. Requirements Added

- **BLD-001-REQ-024:** Every execution SHALL declare one Approved Build Intent before execution.
- **BLD-001-REQ-025:** Build Intent SHALL determine the applicable baseline, Gate Profile, and Evidence Requirement Set.
- **BLD-001-REQ-026:** Build Intent SHALL NOT expand authority, waive a Gate, establish completeness, or grant promotion.
- **BLD-001-REQ-027:** Preparation, enabling-provider candidate, and Foundation build baselines SHALL have distinct identities and Activation decisions.
- **BLD-001-REQ-028:** Tool acquisition SHALL remain separated from governed compilation and testing.
- **BLD-001-REQ-029:** Preparation MAY identify and evaluate unresolved mandatory tools only under bounded authority and context.
- **BLD-001-REQ-030:** Provider-candidate builds SHALL be limited to enumerated Stage 0B subjects.
- **BLD-001-REQ-031:** Candidate outputs SHALL remain non-operational, non-promotable, and isolated from production material.
- **BLD-001-REQ-032:** The bootstrap harness SHALL NOT claim Pipeline, completeness, Activation, or promotion authority.
- **BLD-001-REQ-033:** Preparation and candidate evidence SHALL preserve immutable Evidence Origin.
- **BLD-001-REQ-034:** Candidate and external inputs SHALL NOT satisfy active Foundation dependency requirements.
- **BLD-001-REQ-035:** Every build SHALL bind a CON-010 v1.1 Manifest to its Build Intent, authority, inputs, profiles, outputs, evidence, and non-authorities.
- **BLD-001-REQ-036:** Foundation builds SHALL use active enabling Providers and SHALL NOT fall back to bootstrap mechanisms.
- **BLD-001-REQ-037:** Authority, context, lifecycle, isolation, or evidence failure SHALL stop execution.
- **BLD-001-REQ-038:** Activation of one Build baseline SHALL NOT activate another.
- **BLD-001-REQ-039:** Approval of BLD-001 v1.1 SHALL NOT issue authority, acquire tools, execute builds, or activate a baseline.

## 18. Required Conformance Evidence Added

Activation requires evidence that:

- Build Intent is mandatory and immutable after execution begins;
- an intent cannot select a broader Authority Instrument or Gate;
- each baseline has a distinct identity and Manifest;
- preparation can acquire and seal exact inputs without compiling Falcon behavior;
- undeclared network acquisition fails;
- a Provider-candidate build rejects an unlisted subject;
- production security and financial material is detected and blocks execution;
- candidate artifacts cannot satisfy active dependencies;
- external and candidate evidence remains correctly classified;
- a bootstrap harness cannot decide completeness, Activation, or promotion;
- Foundation build rejects inactive, suspended, revoked, stale, or unknown Providers;
- Foundation build does not fall back to bootstrap identity, time, security, or evidence;
- unresolved tools block only their applicable intents without becoming silently optional;
- baseline Activation order is reconstructable; and
- no Build outcome grants operational or financial authority.

## 19. Supersession

- BLD-001 v1.1 supersedes v1.0;
- all original exact tool and version selections remain unchanged;
- all original deliberate blocks remain visible and scope-aware;
- historical evidence retains its original classification and authority;
- no existing local installation, cache, bundle, image, or candidate is grandfathered; and
- every actual baseline remains non-active until separately verified and activated.

## 20. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner and current Constitutional Authority | Approved and Activated | GOV-033 | 2026-07-25 |

This Approval activates BLD-001 v1.1 as the controlling Catalog and archives v1.0.

It does not:

- issue an Authority Instrument;
- acquire, install, or execute a tool;
- create an environment or offline bundle;
- execute a build or verification plan;
- activate a tool, Provider, environment, Pipeline, Gate Profile, or Build baseline;
- authorize FRS-001 implementation;
- authorize production;
- authorize financial connectivity; or
- authorize financial activity.
