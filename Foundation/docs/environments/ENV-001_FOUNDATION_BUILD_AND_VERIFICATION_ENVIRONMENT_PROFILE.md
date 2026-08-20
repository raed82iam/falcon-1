# ENV-001 — Foundation Build and Verification Environment Profile

**Identifier:** ENV-001  
**Version:** 1.1  
**Status:** Approved  
**Effective Date:** 2026-07-25  
**Approval and Activation Record:** GOV-038  
**Amendment Package:** AMD-003  
**Owner:** Falcon Foundation Governance  
**Governing Authority:** Falcon Vision; Falcon Constitution; ADR-I007; ADR-I008; AMD-003; SEC-002; GOV-AUT-001; AUT-001 v1.1; CON-012; CON-020; CON-021; BLD-001 v1.1; IDN-001 v1.1; TIM-001 v1.1; CRY-001 v1.1; DESIGN-SEC-001 v1.1; VPL-BST-006  
**Initial Foundation Platform:** Windows  
**Planned First Cloud Direction:** Oracle Cloud Infrastructure, subject to separate admission and Activation  
**Implementation Authority:** Not Granted  
**Supersedes:** ENV-001 v1.0  
**Superseded By:** None

## 1. Purpose

ENV-001 v1.1 defines the governed environment model for Foundation preparation, enabling-Provider candidate verification, build verification, reproducibility, evidence review, and future platform transition.

It preserves ENV-001 v1.0, makes Windows the ordered first Foundation platform, introduces the AMD-003 bootstrap environment boundary, and records Oracle Cloud Infrastructure as the intended first future cloud direction without activating or binding Falcon to it.

## 2. Preserved Profile

Unless expressly amended here, ENV-001 v1.0 remains controlling for:

- environment identity, lifecycle, Manifests, isolation, provisioning, and cleanup;
- exact content and provenance requirements;
- network, storage, data, identity, secret, time, and custody controls;
- acquisition, sealed execution, evidence export, reproducibility, and failure rules;
- Windows Server 2025 LTSC and Ubuntu Server 24.04.4 LTS profile families;
- PostgreSQL verification isolation;
- provider and hypervisor replaceability;
- all requirements `ENV-001-REQ-001` through `ENV-001-REQ-030`; and
- all deliberate activation blocks.

No Environment Profile or Activation Manifest becomes active through this amendment.

## 3. Ordered Deployment Strategy

Foundation environment work SHALL proceed in this order:

1. **Windows Foundation:** prepare and verify the first Foundation environment on the approved Windows family.
2. **Independent Windows Reproducibility:** prove that Windows results do not depend on one mutable machine.
3. **Portable Contract Verification:** prove that Falcon environment meaning remains independent of Windows-specific types and mechanisms.
4. **Future Platform Admission:** admit Linux, Oracle Cloud, or another platform only through its own evidence and Activation decision.

This ordering prioritizes one controlled starting point. It does not weaken cross-platform design.

Linux remains an approved future profile family but is not required to block the first Windows Foundation environment unless a governing verification obligation explicitly requires cross-platform evidence for the subject under review.

## 4. Deployment Identity

Falcon SHALL distinguish:

- Falcon system identity;
- Environment Profile identity;
- Environment Instance identity;
- operating-system identity;
- host or hypervisor identity;
- cloud tenancy and account identity;
- region and location identity;
- network and storage identity;
- workload identity; and
- Runtime Epoch identity.

None of these identities may be inferred from another.

Changing the operating system, host, hypervisor, cloud tenancy, compartment, region, image, update state, network boundary, storage boundary, workload identity, or material configuration creates a new environment subject requiring reevaluation.

## 5. Environment Classes

ENV-001 v1.1 recognizes the existing v1.0 classes and adds:

| Environment Class | Purpose | Lifecycle effect |
|---|---|---|
| `PREPARATION` | Governed acquisition, image preparation, offline-bundle construction, and evidence-capture preparation | Cannot produce an active environment or official Foundation artifact |
| `CANDIDATE_PROVIDER_VERIFY` | Isolated verification of enabling Provider and Adapter candidates | Cannot provide operational identity, time, custody, trust, or authority |

One runtime instance SHALL have exactly one immutable class.

A `PREPARATION` or `CANDIDATE_PROVIDER_VERIFY` instance SHALL NOT be reclassified, promoted, or reused as an active Foundation, production, or financial environment.

## 6. Environment Profile States

| Profile or direction | State under v1.1 |
|---|---|
| Windows Foundation build and verification family | `APPROVED`, not `ACTIVE`; ordered first |
| Windows independent reproducibility family | `APPROVED`, not `ACTIVE`; required before Windows promotion claims |
| Linux Foundation build and reproducibility families | `APPROVED`, not `ACTIVE`; future admission track |
| Oracle Cloud Infrastructure | Planned direction only; no admitted executable Profile |
| Preparation and Provider-candidate classes | Defined and governed; no instance or Manifest active |

Catalog or Profile Approval SHALL NOT imply that an image, runner, host, tenancy, cloud service, region, network, storage system, or workload is valid or active.

## 7. Windows Foundation Boundary

The first Foundation environment candidate SHALL use the Windows family already established by ENV-001 v1.0 and the exact Activation-bound image, servicing state, tools, configuration, and controls required by BLD-001.

Windows-specific details SHALL remain environment or Adapter details.

They SHALL NOT enter:

- Falcon Contracts;
- canonical identifiers or timestamps;
- Trust Object meaning;
- authority or jurisdiction;
- Pipeline semantics;
- evidence semantics;
- failure meaning; or
- ordinary component behavior.

Passing Windows verification establishes only scoped Windows validity.

## 8. Developer Workstation Boundary

The Project Owner’s or developer’s Windows workstation MAY be used for:

- document authoring and review;
- non-authoritative inspection;
- preparation planning; and
- launching separately authorized environment preparation.

It SHALL NOT, merely by possession of the repository or tools:

- become a governed runner;
- issue Falcon operational identity or verified time;
- become a cryptographic custody boundary;
- produce an official candidate;
- establish reproducibility;
- activate an Environment Profile;
- connect Falcon to financial systems; or
- acquire implementation authority.

A governed Windows environment SHALL be independently identified, isolated, provisioned, verified, and activated.

## 9. Bootstrap Execution Context

Before any Environment Profile is active, every preparation or candidate-provider execution SHALL reference:

- one competent Authority Instrument;
- one immutable CON-020 Bootstrap Execution Context;
- an exact environment candidate identity;
- an external environment instance identity marked `BOOTSTRAP_EXTERNAL_ID`;
- exact tools, inputs, outputs, network, storage, and data boundaries;
- external bootstrap time and identity;
- synthetic-only security and test material;
- stop, cleanup, and evidence obligations; and
- one applicable VPL-BST plan.

The Bootstrap Execution Context is authority-bounded preparation context. It is not an active Falcon Environment Profile.

## 10. Preparation Environment

A `PREPARATION` environment MAY perform only actions explicitly granted by its Authority Instrument, including:

- acquiring exact approved tool candidates;
- verifying source, digest, signature, license, and provenance;
- constructing sealed offline bundles;
- preparing content-identified image candidates;
- preparing non-behavioral provisioning definitions;
- preparing synthetic fixtures;
- preparing evidence-capture mechanisms; and
- exporting preparation evidence.

It SHALL NOT:

- build general Falcon functionality;
- activate a Provider or Environment Profile;
- issue operational Trust Objects;
- use production credentials or data;
- access financial endpoints; or
- convert preparation outputs into trusted inputs without independent admission.

## 11. Candidate Provider Verification Environment

A `CANDIDATE_PROVIDER_VERIFY` environment MAY exercise only the enabling Providers and Adapters named by the Authority Instrument.

It SHALL:

- use the exact candidate Provider and Adapter identities;
- use synthetic, disposable, non-financial material;
- isolate candidates from operational dependencies;
- preserve candidate and external-control observations separately;
- satisfy applicable VPL-BST obligations;
- prohibit self-certification and self-Activation;
- quarantine on material failure or uncertainty; and
- verify cleanup independently.

Success in this class does not activate the environment or Provider.

## 12. External Bootstrap Identity and Time

Until the applicable Falcon Providers are active:

- environment and execution identity SHALL remain `BOOTSTRAP_EXTERNAL_ID`;
- time observations SHALL remain `BOOTSTRAP_EXTERNAL`;
- Runtime Epoch boundaries SHALL be external and explicit;
- issuer, source, scheme, resolution, uncertainty, and continuity limits SHALL be preserved;
- external identity and time SHALL not be relabeled as Falcon operational values; and
- later Falcon identifiers and time observations SHALL cross-link rather than overwrite historical bootstrap records.

Bootstrap evidence SHALL remain usable only within its declared scope and limitations.

## 13. Environment Evidence

Every preparation and candidate environment case SHALL preserve:

- Authority Instrument;
- CON-020 context;
- environment class, profile candidate, instance, image, and configuration identity;
- OS, host, hypervisor, firmware, update, package, and tool identity;
- network, storage, data, secret, and custody boundaries;
- external bootstrap identity, time, and Runtime Epoch;
- inputs, outputs, actions, faults, and observations;
- candidate and external-control provenance;
- stop-condition disposition;
- cleanup, destruction, quarantine, and residual uncertainty;
- evidence export;
- Challenges and evaluations; and
- explicit non-authorities.

Environment evidence SHALL be preserved under CON-021 and SHALL survive disposable instance destruction.

## 14. Self-Verification Prohibition

No environment candidate, image builder, provisioning tool, host, hypervisor, cloud control plane, candidate Provider, evidence collector, or person preparing the environment SHALL be the sole authority that:

- establishes the environment’s identity;
- validates its isolation or capabilities;
- declares its evidence complete;
- accepts its validity;
- resolves a material Challenge to itself;
- activates the environment; or
- restores it after material failure.

Vendor or platform attestations MAY contribute evidence. They do not establish Falcon acceptance by themselves.

## 15. Failure and Quarantine

Preparation or candidate execution SHALL stop when:

- authority or context is missing, expired, revoked, or exceeded;
- image, configuration, tool, input, or environment identity changes;
- network, storage, data, secret, custody, or evidence isolation fails;
- a production or financial path appears;
- bootstrap identity or time is misclassified;
- a candidate is treated as active;
- evidence cannot be preserved;
- self-certification is attempted; or
- a non-waivable Gate fails.

Affected outputs SHALL be quarantined and SHALL not enter a build, verification, activation, release, production, or financial path.

Return of a host, network, Provider, or cloud service SHALL not restore environment validity automatically.

## 16. Cleanup

Cleanup SHALL:

- terminate the execution and revoke temporary authority;
- revoke synthetic identities, credentials, certificates, keys, and secrets;
- remove temporary inputs, workspaces, plaintext, handles, and routes;
- preserve required evidence;
- verify prohibited locations;
- record incomplete removal or uncertain destruction;
- quarantine affected images, snapshots, disks, or instances where required; and
- receive independent confirmation.

Cleanup completion does not validate or activate the environment.

## 17. Oracle Cloud Direction

Oracle Cloud Infrastructure is the intended first future cloud environment for Falcon.

No OCI executable Environment Profile is established by ENV-001 v1.1.

Before OCI admission, a future governed Profile SHALL define:

- tenancy and compartment model;
- region, availability-domain, and data-residency boundaries;
- workload and administrative identity;
- network, egress, ingress, DNS, and private-endpoint boundaries;
- compute, image, virtualization, container, and orchestration identity;
- storage, backup, snapshot, replication, deletion, and recovery behavior;
- key, secret, certificate, randomness, and custody Adapters;
- logging, evidence export, retention, and independent access;
- time sources and Runtime Epoch behavior;
- service availability, throttling, quota, billing, partition, and control-plane failure;
- provider update, deprecation, migration, and exit policy;
- shared-responsibility and privileged-provider threat assumptions;
- multi-region and failover ordering semantics;
- cost and resource limits;
- cleanup and residual-resource verification; and
- independent Activation evidence.

OCI service names, SDK types, resource identifiers, credentials, errors, and lifecycle semantics SHALL remain outside Falcon Contracts.

## 18. Cloud Portability

The future OCI Profile SHALL preserve replacement by another cloud or private environment.

No cloud environment may be activated unless:

- Falcon meaning remains provider-neutral;
- data and evidence can be recovered in governed form;
- identities and authorities can be re-established without inheritance from the old provider;
- keys and secrets can be rotated or replaced safely;
- historical evidence remains verifiable;
- migration uncertainty is contained;
- old resources can be revoked, retired, or recorded as residual;
- service loss cannot trigger weaker fallback; and
- an exit plan has been independently evaluated.

Cloud convenience SHALL NOT override capital protection, security, evidence, authority, or recoverability.

## 19. Activation Boundary

An Environment Profile may become `ACTIVE` only after:

1. the logical Profile is Approved;
2. one exact immutable Activation Manifest exists;
3. image, OS, servicing, package, tool, configuration, network, storage, identity, time, custody, and evidence identities are complete;
4. the appropriate VPL-BST plans pass;
5. required Provider dependencies are active for the exact scope;
6. isolation, non-financial boundaries, failure, recovery, cleanup, and evidence export are independently verified;
7. the Evidence Requirement Set is complete;
8. material Challenges are resolved;
9. validity and completeness are accepted by competent authorities;
10. an Activation Authority acts within jurisdiction; and
11. a separate exact Activation Decision is issued.

Activation applies only to the exact Profile, Manifest, image, platform, environment, purpose, scope, validity conditions, and time bounds named.

Windows Activation SHALL NOT activate Linux or OCI. Future OCI Activation SHALL NOT retroactively validate Windows or Linux.

## 20. Self-Awareness and Environment Evolution

Self-Awareness MAY:

- observe drift, health, capability, threat, cost, dependency, and lifecycle state;
- diagnose bounded environment faults;
- recommend patching, migration, scaling, replacement, or restriction;
- prepare a change proposal;
- initiate only separately authorized maintenance workflows; and
- verify that required independent evidence exists.

Self-Awareness SHALL NOT:

- expand its jurisdiction or authority;
- change the Vision, Constitution, or governing policy;
- activate, promote, migrate, or restore an environment by itself;
- accept its own validity Claims;
- conceal uncertainty or residual resources;
- bypass Guardian, Security Authority, Environment Activation Authority, or independent verification; or
- connect to financial systems without separate authority.

Environment evolution remains proposal → bounded preparation → independent verification → competent acceptance → exact Activation → monitored rollback capability.

## 21. Requirements Added

- **ENV-001-REQ-031:** Windows SHALL be the ordered first Foundation platform without becoming a permanent Falcon dependency.
- **ENV-001-REQ-032:** Linux SHALL remain a future approved profile family and SHALL require its own scoped Activation.
- **ENV-001-REQ-033:** Oracle Cloud Infrastructure SHALL remain a planned future direction until a separate executable Profile is approved and activated.
- **ENV-001-REQ-034:** `PREPARATION` and `CANDIDATE_PROVIDER_VERIFY` instances SHALL remain immutable, non-operational classes.
- **ENV-001-REQ-035:** Every preparation or candidate execution SHALL reference a competent Authority Instrument and immutable CON-020 context.
- **ENV-001-REQ-036:** Bootstrap environment identity and time SHALL remain external and explicitly classified.
- **ENV-001-REQ-037:** A developer workstation SHALL NOT become a governed runner or artifact authority by possession or convenience.
- **ENV-001-REQ-038:** Windows-specific types and mechanisms SHALL remain outside Falcon Contracts and ordinary component behavior.
- **ENV-001-REQ-039:** Preparation outputs SHALL require independent admission before use as governed inputs.
- **ENV-001-REQ-040:** Provider-candidate environments SHALL use only exact candidates and synthetic non-promotable material.
- **ENV-001-REQ-041:** Environment candidates, preparers, builders, hosts, hypervisors, and cloud providers SHALL not conclusively validate or activate themselves.
- **ENV-001-REQ-042:** Material authority, identity, isolation, financial-boundary, provenance, integrity, or evidence failure SHALL stop execution and quarantine outputs.
- **ENV-001-REQ-043:** Cleanup SHALL preserve residual uncertainty and receive independent confirmation.
- **ENV-001-REQ-044:** Environment evidence SHALL conform to CON-021 and survive instance destruction.
- **ENV-001-REQ-045:** An OCI Profile SHALL define tenancy, identity, network, storage, custody, evidence, time, failure, recovery, migration, and exit boundaries before admission.
- **ENV-001-REQ-046:** Cloud-specific types, identifiers, credentials, errors, and lifecycle semantics SHALL remain behind replaceable Adapters.
- **ENV-001-REQ-047:** Every cloud Profile SHALL have an independently evaluated exit path before Activation.
- **ENV-001-REQ-048:** Activation SHALL be exact, scoped, independently supported, and separately authorized for each deployment profile.
- **ENV-001-REQ-049:** Self-Awareness SHALL not activate, promote, migrate, restore, or widen environment authority by itself.
- **ENV-001-REQ-050:** Approval of ENV-001 v1.1 SHALL not create, provision, execute, connect, or activate an environment or authorize implementation.

## 22. Required Before the First Windows Activation

The first Windows Environment Profile SHALL remain non-active until:

1. ENV-001 v1.1 is Approved;
2. BLD-001 v1.1 and applicable bootstrap Contracts and VPL-BST plans are Approved;
3. the exact Windows image, servicing state, packages, tools, and digests are known;
4. preparation occurs under an Authority Instrument and CON-020 context;
5. the Windows candidate is independently isolated and identified;
6. external bootstrap identity, time, and evidence are preserved;
7. applicable Identifier, Time, security, and evidence dependencies are active or an approved bootstrap boundary explicitly limits the case;
8. Windows capability, negative, failure, recovery, cleanup, and reproducibility obligations pass;
9. the Root Verification Evidence Set is complete;
10. material Challenges are resolved; and
11. a competent exact Activation Decision is issued.

## 23. Required Before Future OCI Admission

OCI SHALL remain unadmitted until:

1. a separate OCI Environment Profile is proposed and approved;
2. the exact OCI services and Adapter boundaries are identified;
3. security, identity, time, network, storage, evidence, recovery, portability, and exit designs are approved;
4. Oracle-specific behavior is proven not to redefine Falcon Contracts;
5. the complete cloud threat and shared-responsibility model is accepted;
6. isolated positive, negative, failure, recovery, migration, and cleanup verification passes;
7. the Evidence Requirement Set is complete;
8. material Challenges are resolved; and
9. a competent exact OCI Activation Decision is issued.

## 24. Supersession

With this Approval:

- ENV-001 v1.1 supersedes v1.0;
- every v1.0 requirement not expressly amended remains controlling;
- Windows becomes the ordered first Foundation environment track;
- Linux remains a future independently activated profile family;
- OCI becomes the recorded first planned cloud direction, not an active Profile;
- no image, runner, host, virtual machine, cloud tenancy, cloud service, Provider, secret, route, or environment is created; and
- implementation and operational authority remain ungranted.

## 25. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner and current Constitutional Authority | Approved and Activated | GOV-038 | 2026-07-25 |

This Approval adopts ENV-001 v1.1 as the controlling Foundation Environment Profile and archives v1.0.

It does not:

- create or activate a Windows, Linux, or Oracle Cloud environment;
- download an image or tool;
- provision infrastructure;
- issue an Authority Instrument;
- execute preparation, verification, build, or deployment;
- activate an Identifier, Time, Security, or other Provider;
- authorize implementation;
- authorize production;
- authorize financial connectivity; or
- authorize financial activity.
