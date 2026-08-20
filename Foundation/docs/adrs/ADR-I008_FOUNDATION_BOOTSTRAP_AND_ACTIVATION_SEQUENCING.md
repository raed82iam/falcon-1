# ADR-I008 — Foundation Bootstrap and Activation Sequencing

**Identifier:** ADR-I008  
**Version:** 1.0  
**Status:** Accepted  
**Date:** 2026-07-25  
**Decision Owner:** Falcon Project Owner  
**Scope:** Foundation preparation, enabling-provider construction, verification, profile activation, implementation authorization, and operational authority  
**Affected Specifications:** SYS-001; SEC-001; SEC-002; AUT-001; AUT-002; PIPE-001; FRS-001  
**Affected Governed Documents:** IMP-001; BLD-001; IDN-001; TIM-001; CRY-001; DESIGN-SEC-001; ENV-001; TRC-001; ROADMAP-001  
**Applicable Standards:** STD-001; STD-003; STD-004; STD-005; STD-007; STD-008; STD-009; STD-013  
**Related ADRs:** ADR-F001 through ADR-F008; ADR-I001 through ADR-I007  
**Supersedes:** None  
**Superseded By:** None  
**Decision Record:** GOV-023

## 1. Context

The Approved Foundation documents correctly require verified environments, providers, tools, evidence, authority, and independent activation before unrestricted use.

Their current activation prerequisites, however, create a closed dependency:

```text
Implementation requires a verified environment
        ↓
Environment Activation requires active Identifier, Time, and custody profiles
        ↓
Provider Activation requires implementation and verification in an environment
        ↓
Implementation and verification require an active environment
```

A similar cycle exists between:

- BLD-001 tool acquisition and environment admission;
- PIPE-001 activation and Provider Contracts;
- TRC-001 executable expansion and Pipeline Activation;
- cryptographic provider verification and active custody; and
- explicit implementation authority and the prerequisites required before that authority can be granted.

The cycle cannot be resolved by treating a candidate as active, trusting a developer machine, weakening a prerequisite, or granting broad implementation authority prematurely.

Falcon requires an explicit bootstrap sequence that permits bounded preparation and verification without converting preparation into operational trust.

## 2. Decision Drivers

- preserve every protective and evidentiary requirement;
- remove circular activation dependencies;
- separate preparation from Falcon behavior implementation;
- separate candidate construction from candidate trust;
- permit providers to be tested before they are active;
- prevent bootstrap identity and time from becoming Falcon operational authority;
- preserve non-financial isolation;
- maintain authority jurisdiction and separation;
- support Windows and Linux;
- remain provider-independent;
- preserve exact evidence and provenance;
- prevent temporary bootstrap mechanisms from becoming permanent dependencies; and
- make the first lawful action unambiguous.

## 3. Higher-Authority Constraints

This decision remains subordinate to:

- Falcon Vision;
- Falcon Constitution;
- GOV-001;
- GOV-AUT-001;
- GOV-SEC-001;
- SEC-001;
- SEC-002;
- AUT-001;
- AUT-002;
- FRS-001; and
- all non-waivable safety, authority, evidence, and financial-isolation obligations.

This ADR may order existing obligations. It may not weaken them.

> **Bootstrap creates a path to verification. It does not create trust.**

## 4. Alternatives Considered

### 4.1 Activate Providers by Declaration

Rejected because Approval without verification would convert policy intent into operational trust.

### 4.2 Use the Developer Machine as the Bootstrap Authority

Rejected because inherited state, tools, credentials, time, caches, and connectivity are not independently controlled or reconstructable.

### 4.3 Grant Full Implementation Authority Immediately

Rejected because broad authority would exceed the minimum purpose and could allow Falcon behavior before its protective and evidentiary foundation exists.

### 4.4 Require Every Dependency to Be Active Before Any Preparation

Rejected because it preserves the circular dependency and makes lawful progress impossible.

### 4.5 Governed Staged Bootstrap

Selected because it permits the minimum necessary preparation, candidate construction, and verification while keeping every candidate non-operational until independently activated.

## 5. Decision

Falcon SHALL use a staged authority and activation sequence.

The stages are:

```text
Approved Documentation Baseline
        ↓
Foundation Preparation Authority
        ↓
Bootstrap Environment Candidates
        ↓
Enabling-Provider Candidate Authority
        ↓
Provider and Tool Verification
        ↓
Profile Activation Decisions
        ↓
Foundation Implementation Authority
        ↓
FRS-001 Candidate Implementation
        ↓
Release Verification and Promotion
        ↓
Operational Authority, if separately granted
```

No stage inherits the authority of a later stage.

## 6. Authority Classes

### 6.1 Foundation Preparation Authority

Foundation Preparation Authority permits only:

- acquiring exact approved tools and dependencies;
- verifying downloads, signatures, licenses, and provenance;
- preparing content-identified offline bundles;
- creating candidate Windows and Linux images;
- preparing isolated network and storage controls;
- preparing evidence-capture infrastructure;
- creating non-behavioral provisioning definitions;
- producing Activation Manifest candidates;
- probing platform capabilities; and
- producing preparation evidence.

It SHALL NOT permit:

- implementation of Falcon Core behavior;
- implementation of financial behavior;
- activation of a Profile;
- issuance of Falcon operational identity;
- creation of Falcon operational authority;
- use of production data or credentials;
- connection to financial systems; or
- promotion of an artifact.

Preparation outputs are candidates and evidence only.

### 6.2 Enabling-Provider Candidate Authority

Enabling-Provider Candidate Authority permits bounded construction and verification of only the primitives required to remove bootstrap dependency:

- Identifier Provider candidate;
- Time Provider candidate;
- randomness provider Adapter candidate;
- cryptographic provider Adapter candidate;
- Secret Provider candidate;
- Certificate and Identity Provider candidate;
- canonical encoding support required by those candidates;
- Trust Object and evidence primitives required to evaluate them;
- machine-readable TRC expansion support;
- Pipeline bootstrap harness; and
- isolated verification fixtures.

Every permitted subject SHALL be enumerated in the Authority Instrument.

This authority SHALL NOT permit general Kernel, Service Bus, Guardian, self-awareness, financial, plugin, trading, portfolio, strategy, or production implementation.

### 6.3 Verification Execution Authority

Verification Execution Authority permits:

- executing candidate tools, providers, profiles, and environments as subjects under test;
- generating controlled positive and negative observations;
- fault injection;
- cross-platform verification;
- reproducibility comparison;
- challenge testing;
- evidence preservation; and
- evaluation by appointed authorities.

It does not make the subject `ACTIVE`.

### 6.4 Profile Activation Authority

Profile Activation Authority permits a competent authority to change one exact profile or Activation Manifest to `ACTIVE` only after:

- every prerequisite is satisfied;
- the candidate identity is exact;
- verification evidence is complete;
- validity is assessed for the declared scope;
- jurisdiction is verified;
- conflicts are disclosed;
- independent review is satisfied; and
- the Activation Decision is immutable.

Activation applies only to the exact subject, scope, version, environment, and validity conditions stated by the decision.

### 6.5 Foundation Implementation Authority

Foundation Implementation Authority permits implementation of the bounded FRS-001 work plan only after the enabling Foundation is sufficiently verified and active for that purpose.

It SHALL identify:

- authorized source scope;
- authorized stages;
- excluded capabilities;
- applicable environment;
- applicable Pipeline and Gate Profile;
- authority holder;
- start and expiry;
- stop conditions;
- evidence obligations; and
- revocation path.

### 6.6 Operational Authority

Operational Authority permits use of a verified artifact in a declared operational environment.

It remains separate from:

- Preparation Authority;
- Candidate Implementation Authority;
- Verification Authority;
- Activation Authority;
- Promotion Authority; and
- Foundation Implementation Authority.

FRS-001 grants no financial Operational Authority.

## 7. Bootstrap Trust Boundary

Bootstrap mechanisms SHALL be treated as external verification infrastructure, not as Falcon operational services.

They MAY provide:

- environment instance identifiers;
- provisioning transaction identifiers;
- acquisition timestamps;
- platform monotonic observations;
- artifact digests;
- external signatures;
- runner attestations; and
- evidence-transfer receipts.

They SHALL be labeled:

```text
BOOTSTRAP_EXTERNAL
```

They SHALL NOT be represented as:

- Falcon operational identifiers;
- Falcon `VERIFIED` time;
- Falcon Runtime Epoch identity;
- Falcon authority;
- Falcon Trust Object validity by themselves;
- Falcon Profile Activation; or
- evidence of financial fitness.

## 8. Bootstrap Identity

Before the Falcon Identifier Provider is active, bootstrap objects MAY use externally issued content-identified or random identifiers under the bootstrap environment.

Bootstrap identifiers SHALL:

- declare the issuing mechanism;
- declare scheme and version;
- preserve issuance provenance;
- remain scoped to preparation and verification;
- never be reclassified as Falcon operational IDs;
- never establish logical subject identity;
- never establish authority;
- remain collision-checked; and
- be cross-linked to later Falcon identifiers without replacement.

After Identifier Provider Activation, new Falcon operational objects SHALL use the Falcon Identifier Provider Contract.

Historical bootstrap IDs remain preserved as external provenance.

## 9. Bootstrap Time

Before the Falcon Time Provider is active, preparation evidence MAY use external time observations.

Every bootstrap time observation SHALL:

- identify its source;
- identify the observing environment;
- declare resolution;
- declare known uncertainty where available;
- declare that Falcon Clock Quality is not established;
- declare Runtime Epoch or external continuity limits;
- preserve monotonic and wall time separately;
- avoid security-validity conclusions requiring Falcon `VERIFIED` time; and
- remain `BOOTSTRAP_EXTERNAL`.

Where time validity cannot be proven, the decision SHALL rely on:

- content identity;
- explicit human or authority review;
- bounded execution;
- external trusted evidence; or
- conservative denial.

After Time Provider Activation, affected activation evidence SHALL be reevaluated where Falcon time semantics are material.

## 10. Candidate Use Rule

A candidate Provider MAY be executed as the subject under verification before it is `ACTIVE`.

It SHALL NOT:

- verify its own activation conclusively;
- serve as a trusted dependency for its own completeness decision;
- issue operational authority;
- protect production material;
- replace the external bootstrap evidence source;
- release its own restriction; or
- be used outside the candidate verification environment.

Candidate output is observation, not accepted truth.

## 11. Two-Control Verification

Every enabling candidate SHALL be evaluated through at least two distinct control perspectives:

1. **External Bootstrap Control:** establishes environment, input, tool, execution, and evidence identity independently of the candidate.
2. **Falcon Candidate Control:** produces the behavior and evidence that the candidate claims to provide.

Where both controls rely on one root, provider, host, or actor, the dependency SHALL be disclosed and the result SHALL NOT claim independence.

## 12. Bootstrap Environment Profiles

ENV-001 SHALL distinguish:

- `PREPARATION` environment profile;
- `CANDIDATE_PROVIDER_VERIFY` environment profile;
- `FOUNDATION_BUILD_VERIFY` environment profile;
- `REPRODUCIBILITY` environment profile; and
- `EVIDENCE_REVIEW` environment profile.

Only `PREPARATION` and `CANDIDATE_PROVIDER_VERIFY` profiles may be activated before Falcon Identifier, Time, and custody profiles.

Their activation SHALL use external bootstrap identity and time rules defined by this ADR.

`FOUNDATION_BUILD_VERIFY` SHALL require the active Falcon providers assigned by its Gate Profile.

## 13. Toolchain Bootstrap

BLD-001 tool and runner preparation MAY occur under Foundation Preparation Authority.

The process SHALL:

- use an allowlisted acquisition environment;
- obtain exact declared versions;
- verify publisher evidence;
- record content digests;
- evaluate licenses and vulnerabilities;
- construct an offline bundle;
- preserve provenance; and
- prohibit use as an official Falcon build until the applicable Build Baseline is activated.

Tool acquisition is not Falcon implementation.

Tool-specific configuration or Adapter implementation requires the appropriate Candidate Authority.

## 14. Pipeline Bootstrap

Before the canonical Pipeline is active, a bootstrap harness MAY:

- provision candidate environments;
- import sealed inputs;
- invoke verification commands;
- collect immutable raw evidence;
- export evidence;
- compare digests; and
- support independent review.

It SHALL NOT:

- claim to be PIPE-001 conformance;
- decide Evidence Completeness;
- decide promotion;
- weaken a Gate;
- transform failed evidence into pass; or
- produce a promotable FRS-001 artifact.

The bootstrap harness itself SHALL later enter the PIPE-001 verification and activation case.

## 15. Traceability Bootstrap

The machine-readable TRC expansion MAY be constructed under Enabling-Provider Candidate Authority.

Before it becomes authoritative for Pipeline execution:

- every atomic requirement SHALL expand exactly once;
- source versions and locations SHALL bind exactly;
- forward and reverse mappings SHALL be independently reviewed;
- canonical encoding SHALL be verified;
- schema validation SHALL pass;
- omissions and duplicates SHALL fail;
- the expansion artifact SHALL be content-identified; and
- TRC-001 Activation requirements SHALL be satisfied.

The human-readable Approved TRC-001 remains governing during bootstrap.

## 16. Cryptographic Bootstrap

Cryptographic provider and custody candidates MAY be tested before activation using only:

- synthetic keys;
- non-production roots;
- test-only certificates;
- test-only secrets;
- isolated provider stores;
- declared bootstrap entropy;
- controlled nonce inputs;
- bounded test domains; and
- non-financial environments.

No bootstrap key, secret, certificate, root, or identity SHALL be promoted into production custody.

Candidate custody cannot certify itself.

## 17. Activation Order

The initial order SHALL be:

1. Approve this ADR and its amendment package.
2. Issue Foundation Preparation Authority Instrument.
3. Prepare exact acquisition, Windows, Linux, and evidence-review candidates.
4. Resolve BLD-001 mandatory tool identities and digests.
5. Approve required Provider Contracts.
6. Issue Enabling-Provider Candidate Authority Instrument.
7. Construct and verify FCE, Trust Object, evidence, Identifier, Time, randomness, and custody candidates.
8. Independently evaluate candidate evidence.
9. Activate the minimum Identifier, Time, and custody profiles required for Foundation verification.
10. Verify and activate applicable ENV-001 profiles.
11. Verify and activate the BLD-001 baseline.
12. Produce and verify the machine-readable TRC expansion.
13. Verify and activate the applicable PIPE-001 Gate Profile and Pipeline Definition.
14. Issue bounded Foundation Implementation Authority.
15. Implement FRS-001 stages under IMP-001.
16. Execute complete verification and independent promotion review.

An order change requires a recorded dependency analysis proving equivalent protection and absence of circularity.

## 18. Authority Instruments

Every authority stage SHALL use a valid Authority Instrument under GOV-AUT-001.

The instrument SHALL define:

- instrument identity;
- jurisdiction;
- authority class;
- holder;
- purpose;
- exact permitted subjects;
- exact prohibited subjects;
- environments;
- tools;
- data and secret classes;
- start;
- expiry;
- revocation;
- stop conditions;
- evidence;
- review;
- delegation;
- redelegation prohibition unless explicitly allowed; and
- acceptance by the holder.

Silence, tool access, repository access, environment ownership, or Project Owner conversation context SHALL NOT substitute for the required instrument once operational preparation begins.

## 19. Separation of Duties

For material activation:

- candidate producer SHALL NOT be the sole evaluator;
- environment preparer SHALL NOT be the sole environment validity authority;
- evidence collector SHALL NOT be the sole completeness authority;
- Security Authority SHALL NOT be the sole reviewer of its own material action;
- Promotion Authority SHALL NOT replace Activation Authority;
- Project Owner authority SHALL remain constrained by the Constitution and recorded jurisdiction; and
- Guardian restrictions SHALL remain independently effective.

During the founding stage, one human may hold multiple roles only when:

- concentration is explicitly recorded;
- automated and evidentiary controls remain separated;
- prohibited role combinations remain prohibited;
- material self-review is independently challenged; and
- no document falsely claims organizational independence.

## 20. Failure and Stop Rules

Preparation or candidate work SHALL stop when:

- authority is absent, expired, revoked, or exceeded;
- scope expands beyond the instrument;
- a financial path appears;
- production data or credentials are detected;
- environment isolation fails;
- bootstrap and Falcon identity are confused;
- bootstrap time is represented as Falcon `VERIFIED` time;
- a candidate is used as active;
- evidence cannot be preserved;
- a digest or provenance claim conflicts;
- candidate self-certification is detected;
- a non-waivable Gate fails; or
- a material Challenge remains unresolved.

Failure SHALL preserve evidence and SHALL NOT trigger broader authority.

## 21. No Silent Grandfathering

Artifacts, tools, scripts, images, providers, identifiers, timestamps, keys, evidence, or decisions created before this ADR:

- SHALL NOT become accepted merely because they already exist;
- SHALL be classified by origin and scope;
- SHALL enter the appropriate admission and verification path;
- MAY be retained as historical or exploratory material; and
- SHALL be rejected where identity, provenance, integrity, or authority cannot be established.

## 22. Required Amendment Package

Acceptance of this ADR authorizes preparation, but not automatic activation, of coordinated amendments to:

- IMP-001;
- BLD-001;
- IDN-001;
- TIM-001;
- CRY-001;
- DESIGN-SEC-001;
- ENV-001;
- PIPE-001;
- TRC-001;
- ROADMAP-001;
- FRS-001 Readiness Report;
- GOV-000 where authority assignments are recorded; and
- affected Contracts and verification plans.

The amendment package SHALL:

- replace circular prerequisites with staged prerequisites;
- preserve every protective outcome;
- define bootstrap evidence classification;
- define candidate execution without activation;
- define the exact point at which Falcon Providers become mandatory;
- add traceability;
- preserve prior versions; and
- require separate Approval before activation.

## 23. Consequences

### Positive

- lawful preparation can begin without broad Falcon implementation authority;
- provider candidates can be tested before activation;
- bootstrap identity and time remain explicit and non-operational;
- exact tools and environments can be prepared safely;
- circular dependencies are removed;
- activation remains evidence-based and independent;
- implementation authority becomes narrower and clearer; and
- operational authority remains fully separate.

### Costs

- additional Authority Instruments are required;
- bootstrap evidence must be preserved and later reconciled;
- more than one environment profile is required;
- candidate and active states must remain distinguishable;
- activation sequencing requires explicit coordination; and
- some verification must be repeated after provider activation.

## 24. Risks and Mitigations

| Risk | Mitigation |
|---|---|
| Preparation becomes hidden implementation | Exact permitted subject list and stop rules |
| Bootstrap services become permanent | `BOOTSTRAP_EXTERNAL` classification and no silent grandfathering |
| Candidate is treated as trusted | Candidate Use Rule and independent activation |
| Same person holds several roles | Recorded concentration, prohibited combinations, independent evidence and challenge |
| External time or IDs gain Falcon meaning | Explicit non-equivalence and preserved cross-linking |
| Verification is circular | External and candidate control perspectives |
| Crypto tests leak into production | Synthetic-only custody and absolute non-promotion of bootstrap material |
| Pipeline harness becomes authority | Harness limited to orchestration and raw evidence |
| Broad authority persists | Expiry, revocation, stage scope, and new instrument per stage |

## 25. Conformance Evidence

Conformance requires evidence that:

- preparation can occur without Falcon behavior implementation;
- a provider candidate can be tested without being active;
- bootstrap IDs cannot pass as Falcon operational IDs;
- bootstrap time cannot pass as Falcon `VERIFIED` time;
- no bootstrap key can enter production custody;
- candidate self-activation is denied;
- authority outside the stage instrument is denied;
- developer-machine state cannot become official input;
- financial routes, data, credentials, and consequences remain absent;
- environment and tool candidates remain content-identified;
- a failed candidate remains non-active;
- profile activation references complete independent evidence;
- later Foundation implementation uses active enabling providers;
- the bootstrap harness cannot decide promotion; and
- historical bootstrap evidence remains reconstructable.

## 26. Required Before Any Preparation Execution

No preparation execution may begin until:

1. ADR-I008 is Accepted;
2. the coordinated amendment package is Approved;
3. GOV-001 document-class clarification is completed or an explicit governing exception is recorded;
4. the Authority Instrument Contract is Approved;
5. a Foundation Preparation Authority Instrument is issued and accepted;
6. the non-financial boundary is explicit;
7. bootstrap identity, time, evidence, and secret classifications are approved;
8. evidence retention and challenge paths are approved;
9. affected TRC-001 mappings are current; and
10. the Project Owner explicitly authorizes the bounded preparation stage.

## 27. Foundational Rules

> **Preparation is not implementation.**

> **Candidate execution is not activation.**

> **Activation is not operational permission.**

> **Bootstrap evidence identifies the path; it does not establish Falcon operational truth.**

> **No stage may borrow authority from the stage it is intended to enable.**

## 28. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner and current Constitutional Authority | Accepted | GOV-023 | 2026-07-25 |

Acceptance adopts the staged bootstrap and activation decision and authorizes preparation of its coordinated document amendments.

It does not:

- issue Foundation Preparation Authority;
- issue Candidate Implementation Authority;
- activate a Profile;
- download or install a tool;
- create an environment;
- authorize Falcon behavior implementation;
- authorize production;
- authorize financial connectivity; or
- authorize financial activity.
