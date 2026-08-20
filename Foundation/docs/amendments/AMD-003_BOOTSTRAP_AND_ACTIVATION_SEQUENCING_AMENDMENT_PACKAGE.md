# AMD-003 — Bootstrap and Activation Sequencing Amendment Package

**Identifier:** AMD-003  
**Version:** 1.0  
**Status:** Approved  
**Date:** 2026-07-25  
**Owner:** Falcon Foundation Governance  
**Governing Authority:** Falcon Vision; Falcon Constitution; GOV-001; ADR-I008  
**Affected Documents:** IMP-001; BLD-001; IDN-001; TIM-001; CRY-001; DESIGN-SEC-001; ENV-001; PIPE-001; TRC-001; ROADMAP-001; FRS-001 Readiness Report  
**Related Documents:** GOV-AUT-001; GOV-SEC-001; SEC-001; SEC-002; FCE-001  
**Supersedes:** None  
**Superseded By:** None  
**Implementation Authority:** Not Granted

## 1. Purpose

This Amendment Package applies ADR-I008 consistently across the Foundation documents whose current prerequisites form circular bootstrap and activation dependencies.

It establishes a lawful path to:

- prepare exact tools and environments;
- build enabling-provider candidates;
- verify candidates before Activation;
- activate only independently proven profiles;
- activate the Foundation build and verification path; and
- grant bounded FRS-001 implementation authority afterward.

This Package changes sequencing and authority boundaries. It does not weaken safety, evidence, security, non-financial isolation, or independent review.

## 2. Problem Being Corrected

The current baseline requires:

- active environments before implementation;
- implemented Providers before their Activation;
- active Providers before environment Activation;
- active Pipeline and traceability before producing their executable forms; and
- implementation authority both before and after the prerequisites needed to justify it.

These requirements are individually protective but collectively circular.

The correction distinguishes:

```text
Preparation
≠
Candidate Construction
≠
Verification Execution
≠
Profile Activation
≠
Foundation Implementation
≠
Operational Use
```

## 3. Amendment Principles

1. No candidate becomes trusted by being executable.
2. No preparation artifact becomes Falcon operational state.
3. Bootstrap identity and time remain `BOOTSTRAP_EXTERNAL`.
4. Provider candidates may be subjects under test before Activation.
5. Preparation environments do not require the Falcon Providers they are created to enable.
6. Foundation build environments require active Providers only where their declared Gate Profile uses them as trusted dependencies.
7. Every authority stage requires its own Authority Instrument.
8. Every Activation Decision remains exact, scoped, evidence-based, and independently reviewable.
9. No amendment grants financial, production, or operational authority.
10. Historical approved versions remain preserved.

## 4. Version Transition Matrix

| Document | Current | Amended | Amendment type |
|---|---:|---:|---|
| IMP-001 | 1.1 | 1.2 | Entry gates and staged work-plan sequencing |
| BLD-001 | 1.0 | 1.1 | Preparation and candidate build scopes |
| IDN-001 | 1.0 | 1.1 | Bootstrap identity boundary |
| TIM-001 | 1.0 | 1.1 | Bootstrap time boundary |
| CRY-001 | 1.0 | 1.1 | Candidate cryptographic verification |
| DESIGN-SEC-001 | 1.0 | 1.1 | Synthetic candidate custody and provider testing |
| ENV-001 | 1.0 | 1.1 | Preparation and candidate-provider environment profiles |
| PIPE-001 | 1.0 | 1.1 | Bootstrap intents, harness, and activation sequence |
| TRC-001 | 1.1 | 1.2 | ADR-I008 and staged-authority trace extension |
| ROADMAP-001 | 2.6 | 2.7 | Corrected repair and activation backlog |
| FRS-001 Readiness Report | 3.20 | 4.0 | Reassessment against corrected prerequisites |

Affected Contracts and VPLs SHALL be reviewed. Any semantic change requires its own versioned amendment; no Contract or VPL is silently changed by AMD-003.

## 5. Common Terms Added to Affected Documents

### 5.1 Foundation Preparation Authority

Bounded authority to acquire, verify, package, provision, and evidence tools and environment candidates without implementing Falcon behavior.

### 5.2 Enabling-Provider Candidate Authority

Bounded authority to construct only the Provider, canonical, evidence, trace, and bootstrap-harness candidates explicitly enumerated by its Authority Instrument.

### 5.3 Verification Execution Authority

Authority to execute a candidate as a subject under test and preserve observations without activating or promoting it.

### 5.4 Profile Activation Authority

Authority to activate one exact verified Profile or Activation Manifest within declared jurisdiction and scope.

### 5.5 Foundation Implementation Authority

Authority to implement the bounded FRS-001 plan after its enabling environment, Providers, tools, trace, and Pipeline are ready for that declared scope.

### 5.6 Bootstrap External Evidence

Externally established preparation evidence that preserves identity, time, provenance, integrity, and scope but does not claim Falcon operational identity, Falcon `VERIFIED` time, or Falcon authority.

## 6. IMP-001 v1.2 Amendment

### 6.1 Entry Gate Correction

The implementation gate SHALL be split into:

1. **Preparation Entry Gate**
2. **Enabling-Provider Candidate Gate**
3. **Foundation Implementation Gate**

### 6.2 Stage 0A — Governed Preparation

**Falcon behavior code permitted:** No.

Permitted:

- exact tool acquisition;
- image and runner candidate preparation;
- offline bundle construction;
- platform capability probes;
- evidence infrastructure preparation;
- synthetic data preparation;
- Activation Manifest candidates; and
- preparation evidence.

Required:

- Accepted ADR-I008;
- Approved AMD-003;
- Approved Authority Instrument Contract;
- issued Foundation Preparation Authority Instrument;
- explicit non-financial boundary;
- bootstrap evidence classification; and
- stop and revocation rules.

### 6.3 Stage 0B — Enabling Candidates

**General Falcon behavior code permitted:** No.

Only enumerated enabling candidates may be constructed:

- Identifier Provider;
- Time Provider;
- randomness Adapter;
- cryptographic Provider Adapter;
- Secret Provider;
- Certificate and Identity Provider;
- FCE support;
- Trust Object and evidence primitives;
- machine-readable TRC expansion;
- bootstrap harness; and
- isolated verification fixtures.

Required:

- Enabling-Provider Candidate Authority Instrument;
- Approved applicable Provider Contracts;
- active candidate-verification environment;
- exact BLD candidate tool scope;
- synthetic-only keys, secrets, identities, and data; and
- independent evidence path.

### 6.4 Stage 0C — Enabling Activation

This stage:

- evaluates candidates;
- activates the minimum Providers and profiles;
- activates Foundation environment manifests;
- activates the applicable BLD baseline scope;
- activates machine-readable TRC use;
- activates the applicable PIPE Gate Profile; and
- produces the Foundation Implementation Readiness case.

### 6.5 Stage 1 Renaming

Existing Stage 1 becomes:

```text
Stage 1 — Controlled FRS-001 Project Foundation
```

It may begin only after a bounded Foundation Implementation Authority Instrument is issued.

### 6.6 Unchanged Boundaries

IMP-001 continues to prohibit:

- financial logic;
- market connectivity;
- production credentials;
- live data;
- trading;
- portfolio behavior;
- unrestricted self-evolution; and
- operational authority.

## 7. BLD-001 v1.1 Amendment

### 7.1 Build Scope Classes

BLD-001 SHALL distinguish:

| Scope | Permitted output | Promotion |
|---|---|---|
| `PREPARATION` | Verified tool and dependency bundles | None |
| `ENABLING_CANDIDATE` | Enumerated Provider and bootstrap candidates | None |
| `FOUNDATION_CANDIDATE` | FRS-001 candidate artifacts | PIPE-001 governed only |
| `REPRODUCIBILITY` | Independent comparison outputs | None |

### 7.2 Activation Correction

BLD-001's blanket requirement for Foundation Implementation Authority before any Activation is replaced with:

- Preparation scope requires Foundation Preparation Authority.
- Enabling-candidate scope requires Enabling-Provider Candidate Authority.
- Foundation-candidate scope requires Foundation Implementation Authority.
- Promotion requires independent Promotion Authority.

### 7.3 Tool Acquisition

Exact tool acquisition, signature verification, digest recording, license assessment, vulnerability assessment, and offline bundling are classified as Preparation, not Falcon behavior implementation.

### 7.4 Remaining Blocks

All unresolved mandatory tools, digests, licenses, vulnerabilities, reproducibility, SBOM, provenance, and isolation evidence remain blocking for their applicable scope.

No missing tool is waived.

## 8. IDN-001 v1.1 Amendment

### 8.1 Bootstrap Identifier Class

Add:

```text
BOOTSTRAP_EXTERNAL_ID
```

It is:

- external provenance only;
- scoped to preparation and verification;
- issued by a declared external mechanism;
- not a Falcon operational ID;
- not authority;
- not logical subject creation; and
- never reclassified as a Falcon ID.

### 8.2 Cross-Linking

After Falcon Identifier Provider Activation:

- new operational objects use Falcon identifiers;
- historical bootstrap objects retain original IDs;
- Falcon objects may reference bootstrap provenance;
- replacement and rewriting are prohibited; and
- continuity SHALL NOT be inferred across the two schemes without an explicit governed relationship.

### 8.3 Activation Correction

The Identifier Provider may be constructed and tested as a candidate under Enabling-Provider Candidate Authority before its Profile is `ACTIVE`.

Operational issuance remains prohibited until:

- the Identifier Provider Contract is Approved;
- the Profile is independently activated; and
- the applicable environment is authorized.

## 9. TIM-001 v1.1 Amendment

### 9.1 Bootstrap Time Quality

Add:

```text
BOOTSTRAP_EXTERNAL
```

This is an evidence classification, not Falcon Clock Quality.

It SHALL declare:

- source;
- environment;
- observed UTC where available;
- monotonic observation where available;
- resolution;
- known uncertainty;
- verification limitations;
- continuity boundary; and
- provenance.

### 9.2 Prohibitions

Bootstrap time SHALL NOT:

- claim `VERIFIED`;
- establish Falcon Runtime Epoch;
- authorize security validity requiring Falcon time;
- activate a Profile;
- establish authoritative ordering; or
- be silently upgraded after Time Provider Activation.

### 9.3 Candidate Verification

The Time Provider may execute as a candidate under external bootstrap observation before it is `ACTIVE`.

It SHALL be independently tested for:

- source authentication;
- uncertainty;
- rollback;
- forward jump;
- contradiction;
- resolution;
- epoch transition;
- monotonic behavior;
- leap behavior;
- holdover;
- failure; and
- recovery.

### 9.4 Activation Correction

Only operational runtime use requires the active Falcon Time Provider.

Preparation and candidate verification environments use the ADR-I008 bootstrap time boundary.

## 10. CRY-001 v1.1 Amendment

### 10.1 Candidate Cryptographic Use

Before FALCON-CRYPTO-1 is `ACTIVE`, cryptographic provider candidates may perform bounded verification operations using:

- synthetic keys;
- test-only independent roots;
- test domains;
- test certificates;
- test secrets;
- controlled nonce inputs;
- isolated stores; and
- non-financial environments.

### 10.2 Absolute Prohibitions

Candidate cryptography SHALL NOT:

- protect production material;
- use production roots;
- establish operational Falcon identity;
- sign promotable production authority;
- activate itself;
- certify its own completeness; or
- transfer bootstrap material into active custody.

### 10.3 Activation Correction

Explicit Foundation Implementation Authority is not required to execute a cryptographic candidate under test.

Required instead:

- Enabling-Provider Candidate Authority;
- Verification Execution Authority;
- Approved Provider Contracts;
- exact candidate identity;
- synthetic-only material;
- candidate environment;
- evidence; and
- independent Activation Decision.

Operational cryptographic use continues to require an `ACTIVE` profile and applicable authority.

## 11. DESIGN-SEC-001 v1.1 Amendment

### 11.1 Provider Lifecycle

Provider realization SHALL distinguish:

```text
DESIGN_SELECTED
        ↓
CANDIDATE_BUILT
        ↓
UNDER_VERIFICATION
        ↓
VERIFIED_FOR_SCOPE
        ↓
ACTIVE
```

These states SHALL NOT replace Catalog lifecycle states. They describe realization evidence.

### 11.2 Candidate Custody

Candidate custody:

- uses test-only roots;
- remains isolated;
- prohibits production import;
- prohibits ordinary secret export;
- records every operation;
- supports destruction verification;
- cannot self-activate; and
- cannot become production custody through migration.

### 11.3 Provider Contracts

Candidate construction SHALL NOT begin until applicable Cryptographic, Secret, Certificate and Identity, Identifier, Time, and randomness Contracts or governed boundaries are Approved.

### 11.4 Activation Correction

Platform capability probes and negative provider tests may run under candidate authority before custody is active.

Active custody remains required when an FRS-001 Gate relies on cryptographic results as accepted protection.

## 12. ENV-001 v1.1 Amendment

### 12.1 New Environment Classes

Add:

| Environment Class | Purpose | Falcon Providers required |
|---|---|---|
| `PREPARATION` | Acquire, verify, provision, and evidence candidates | No |
| `CANDIDATE_PROVIDER_VERIFY` | Execute enabling Providers as subjects under test | No active candidate dependency; external bootstrap controls required |
| `FOUNDATION_BUILD_VERIFY` | Build and verify FRS-001 | Yes, as assigned by active Gate Profile |

### 12.2 New Profiles

Add:

- `FALCON-ENV-PREP-WIN-1`
- `FALCON-ENV-PREP-LNX-1`
- `FALCON-ENV-PROVIDER-WIN-1`
- `FALCON-ENV-PROVIDER-LNX-1`

All begin `PROPOSED`.

### 12.3 Activation Correction

Preparation profiles may be activated when:

- exact images and tools are identified;
- isolation is proven;
- non-financial boundaries are proven;
- bootstrap identity and time are configured;
- evidence export is proven;
- cleanup is proven;
- Foundation Preparation Authority exists; and
- an exact Activation Decision is issued.

They do not require active Falcon Identifier, Time, or custody profiles.

Candidate-provider profiles require:

- active Preparation profile or equivalent verified inputs;
- exact candidate bundle;
- external bootstrap controls;
- synthetic-only data and secrets;
- Candidate Authority;
- Verification Execution Authority; and
- independent evidence.

Foundation build profiles continue to require active Falcon Providers assigned by their Gate Profile.

### 12.4 Environment Identity Correction

Before Identifier Provider Activation, Environment Instance identity is external bootstrap identity and SHALL be labeled `BOOTSTRAP_EXTERNAL_ID`.

After Activation, new Foundation runtime instances use Falcon operational identity.

## 13. PIPE-001 v1.1 Amendment

### 13.1 New Build Intents

Add:

| Build Intent | Purpose | Promotion |
|---|---|---|
| `FOUNDATION_PREPARATION` | Prepare verified tools, bundles, and environments | None |
| `ENABLING_PROVIDER_CANDIDATE` | Build and verify only enumerated enabling candidates | None |
| `PIPELINE_BOOTSTRAP_VERIFICATION` | Verify the canonical Pipeline and Gate realization | None |

### 13.2 Bootstrap Gate Profiles

Add proposed profiles:

- `FALCON-GATE-PREPARATION-1`
- `FALCON-GATE-ENABLING-PROVIDER-1`
- `FALCON-GATE-PIPELINE-BOOTSTRAP-1`

They remain non-active until separately approved and activated.

### 13.3 Bootstrap Harness

The bootstrap harness may:

- provision;
- import sealed inputs;
- invoke candidate verification;
- collect raw evidence;
- export evidence; and
- compare content identities.

It may not:

- claim PIPE-001 conformance;
- decide completeness;
- decide Activation;
- decide Promotion;
- mutate evidence;
- weaken a Gate; or
- create promotable FRS-001 artifacts.

### 13.4 Pipeline Activation Correction

The Pipeline may be verified as a candidate before it is active.

Its Activation case SHALL use:

- external bootstrap identity and time;
- active environment scope appropriate to candidate verification;
- exact BLD candidate scope;
- machine-readable TRC candidate;
- complete evidence;
- independent evaluation;
- competent Activation Authority; and
- no financial path.

Foundation Implementation Authority is required only after the enabling Pipeline scope is active.

## 14. TRC-001 v1.2 Amendment

TRC-001 SHALL add trace subjects for:

- ADR-I008;
- Foundation Preparation Authority;
- Enabling-Provider Candidate Authority;
- Verification Execution Authority;
- Profile Activation Authority;
- Foundation Implementation Authority;
- bootstrap external identity;
- bootstrap external time;
- candidate provider execution;
- bootstrap harness;
- staged environment profiles;
- synthetic custody;
- Activation Decisions;
- prohibited authority inheritance; and
- no silent grandfathering.

Until new requirement-bearing Contracts and policies are Approved, the atomic requirement count remains 776.

New approved requirement identifiers SHALL be added in the same approval package that creates them.

## 15. ROADMAP-001 v2.7 Amendment

ROADMAP-001 SHALL record at minimum:

1. GOV-001 v1.2.
2. Authority Instrument Contract.
3. Identifier Provider Contract.
4. Time Provider Contract.
5. Cryptographic Provider Contract.
6. Secret Provider Contract.
7. Certificate and Identity Provider Contract.
8. Randomness Provider boundary.
9. Jurisdiction Catalog.
10. Decision-Class and Consequence-Class Catalog.
11. Delegation and Revocation Contracts.
12. Challenge and Independent Review Policy.
13. FCE schema and vector registry.
14. Trust Object lifecycle and retention policies.
15. Exact BLD tool completion.
16. Bootstrap Gate Profiles.
17. Environment Activation Manifests.
18. Machine-readable TRC expansion.
19. Authority appointments and instruments.
20. Updated IMP-001.
21. Updated Readiness Report.
22. Document-control metadata remediation.

Every entry SHALL state whether it is required before:

- preparation;
- candidate construction;
- candidate verification;
- profile Activation;
- Foundation implementation;
- Promotion; or
- operational use.

## 16. FRS-001 Readiness Report v4.0 Amendment

The report SHALL:

- preserve `Not Yet Authorized for Implementation`;
- distinguish documentation readiness from preparation readiness;
- distinguish preparation readiness from implementation readiness;
- list every unresolved BLD tool;
- list every missing Provider Contract;
- list every missing authority record;
- list every inactive Profile;
- record the bootstrap sequencing correction;
- record metadata remediation;
- identify exact approval decisions still required;
- state whether bounded Preparation Authority may be considered; and
- prohibit broad implementation authorization until the corrected gate passes.

The v4.0 report remains an assessment, not an authority instrument.

## 17. Contract and VPL Review

The following SHALL be reviewed:

- CON-001 Core Identity;
- CON-002 Authority Decision;
- CON-008 Evidence and Logging;
- CON-009 Security Context;
- CON-010 Foundation Baseline Manifest;
- VPL-001 Trusted Bootstrap;
- VPL-007 Controlled Recovery;
- VPL-008 Evidence Reconstruction; and
- VPL-000 Master Plan.

Review SHALL determine whether they require:

- semantic amendment;
- bootstrap classification extension;
- only new dependent Contracts; or
- no change.

No change is presumed.

## 18. Approval and Activation Mechanics

Approval of AMD-003 SHALL authorize:

- creation of amended document versions;
- preservation of superseded versions;
- coordinated activation after validation;
- registry and index updates;
- trace updates; and
- creation of the corrected readiness assessment.

Activation SHALL occur only when:

1. every amended document exists in final form;
2. cross-document terminology matches;
3. no circular prerequisite remains;
4. Contract and VPL impact decisions are recorded;
5. TRC mappings are current;
6. registries identify exact versions;
7. prior versions are archived;
8. independent document review passes; and
9. the Project Owner explicitly approves Activation.

Approval of the package does not automatically activate the amended documents.

## 19. Verification Checklist

The coordinated amendment SHALL prove:

- preparation can begin without Falcon Provider Activation;
- preparation cannot implement Falcon behavior;
- candidate Providers can run only as subjects under test;
- candidate Providers cannot activate themselves;
- bootstrap IDs cannot become Falcon operational IDs;
- bootstrap time cannot become Falcon `VERIFIED` time;
- candidate cryptography cannot use production material;
- Foundation build environments still require active Providers;
- Pipeline bootstrap cannot decide completeness or Promotion;
- each authority stage has an exact instrument;
- failure stops rather than widens authority;
- historical evidence is preserved;
- no document silently weakens another;
- no financial capability enters scope; and
- the revised dependency graph is acyclic.

## 20. Known Follow-On Work

AMD-003 does not itself author:

- Provider Contracts;
- Authority Instrument Contract;
- authority catalogs;
- security procedures;
- exact tool selections;
- Activation Manifests;
- Gate Profile contents;
- machine-readable trace artifacts;
- metadata remediation; or
- Authority appointments.

They remain separately governed Roadmap work.

## 21. Foundational Rules

> **The bootstrap path must be usable before it can be trusted, but it must never be trusted merely because it is usable.**

> **Every temporary mechanism remains temporary by identity, scope, and authority.**

> **No candidate activates itself.**

> **No stage borrows authority from the stage it enables.**

## 22. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner and current Constitutional Authority | Approved | GOV-024 | 2026-07-25 |

Approval authorizes authoring and coordinated review of the amended document versions defined by this Package.

It does not:

- activate an amended version;
- issue an Authority Instrument;
- select or appoint an authority holder;
- download or install a tool;
- create an environment;
- implement a Provider;
- implement Falcon behavior;
- authorize production;
- authorize financial connectivity; or
- authorize financial activity.
