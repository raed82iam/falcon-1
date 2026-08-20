# TIM-001 — Foundation Time and Clock-Quality Catalog

**Identifier:** TIM-001  
**Version:** 1.1  
**Status:** Approved  
**Effective Date:** 2026-07-25  
**Approval and Activation Record:** GOV-035  
**Amendment Package:** AMD-003  
**Owner:** Falcon Specification Authority  
**Governing Authority:** ADR-I006; ADR-I008; AMD-003; IMP-001 v1.2; CON-008 v1.1; CON-015; CON-020; CON-021; VPL-BST-004  
**Implementation Authority:** Not Granted
**Supersedes:** TIM-001 v1.0  
**Superseded By:** None

## 1. Purpose

This Catalog defines how preparation and candidate-verification time is observed before the Falcon Time Provider is active, and the exact boundary after which CON-015 operational Time Observations become mandatory.

It prevents external timestamps, platform clocks, runner times, and candidate output from being misrepresented as Falcon `VERIFIED` time.

## 2. Preserved Decisions

Unless explicitly amended here, TIM-001 v1.0 remains controlling for:

- UTC, monotonic, duration, Time Observation, Temporal Boundary, and uncertainty semantics;
- Time Profile lifecycle;
- Clock Quality states and scoped meaning;
- Temporal Evaluation Outcomes;
- Clock Capabilities and Source Classes;
- leap behavior;
- canonical encoding ownership by FCE-001;
- Time Provider obligations;
- Runtime Epoch rules;
- Temporal Decision Profiles;
- Clock Quality verification;
- uncertainty evaluation;
- precision and resolution;
- holdover and offline operation;
- validity, expiry, causality, and ordering;
- evidence, failure, and restoration; and
- the rule that `VERIFIED` is earned by evidence and never self-declared.

No existing Time Profile, Quality state, Source Class, capability, or threshold is changed.

## 3. External Bootstrap Time Observation

Before the Falcon Time Provider is active, preparation and candidate-verification evidence MAY carry an External Bootstrap Time Observation classified:

```text
BOOTSTRAP_EXTERNAL
```

Every observation SHALL contain:

- external observation ID;
- classification `BOOTSTRAP_EXTERNAL`;
- observing environment identity;
- source identity and source class description;
- source mechanism and version;
- observed wall time;
- monotonic observation where available;
- wall-time resolution;
- monotonic resolution where available;
- known uncertainty or explicit `UNKNOWN`;
- continuity boundary or external epoch identity;
- leap behavior where known;
- observation purpose;
- applicable CON-020 Context;
- provenance and custody;
- known limitations;
- canonical protected representation; and
- explicit statement that Falcon Clock Quality is not established.

`BOOTSTRAP_EXTERNAL` is an evidence classification. It is not a Falcon Clock Quality state or active Time Profile.

## 4. Separation of Wall and Monotonic Time

External wall and monotonic observations SHALL remain separate fields.

They SHALL NOT be:

- combined into one ambiguous timestamp;
- converted into Falcon UTC or Runtime Epoch semantics;
- compared across external continuity boundaries;
- used to prove causality;
- used to establish authoritative ordering; or
- assigned Falcon Clock Quality.

A monotonic value without a known external continuity boundary is not comparable beyond its single observation context.

## 5. Permitted Bootstrap Uses

External bootstrap time MAY support:

- acquisition chronology;
- tool and bundle preparation evidence;
- environment-provisioning chronology;
- candidate execution duration;
- evidence transfer and custody chronology;
- timeout and cleanup controls within one proven external continuity boundary;
- bounded validity review by a competent human or authority; and
- comparison with other external observations where their dependency and uncertainty are declared.

External bootstrap time SHALL NOT by itself establish:

- certificate or credential validity requiring Falcon `VERIFIED` time;
- FIL freshness, expiry, or replay windows;
- operational authority validity;
- operational identifier time;
- Falcon Runtime Epoch continuity;
- release or Profile Activation validity;
- business-event occurrence time;
- financial ordering;
- causal order; or
- unrestricted trust.

## 6. Bootstrap Temporal Decisions

Where a preparation or candidate decision requires time before Provider Activation:

- the decision SHALL declare the temporal purpose;
- the external source and uncertainty SHALL remain visible;
- the full uncertainty interval SHALL be evaluated;
- external dependency and common-root risk SHALL be disclosed;
- content identity and non-temporal evidence SHALL be preferred where possible;
- a competent authority SHALL accept any residual temporal uncertainty;
- the decision SHALL remain bounded to preparation or candidate scope; and
- uncertainty SHALL produce the conservative outcome.

If the required decision cannot be made without Falcon `VERIFIED` time, it SHALL be deferred or denied.

## 7. Candidate Time Provider

A Time Provider candidate MAY produce observations only:

- under an Enabling-Provider Candidate Authority Instrument;
- inside a `CANDIDATE_PROVIDER_VERIFY` CON-020 context;
- under a candidate-only Time Profile;
- with controlled or synthetic sources;
- for non-production subjects;
- as the subject of VPL-BST-004;
- with independent external reference observations; and
- with explicit `CANDIDATE` output lifecycle.

Candidate observations SHALL NOT:

- receive operational `VERIFIED` quality;
- validate the candidate's own source, quality, completeness, or Activation;
- satisfy an active Provider dependency;
- validate production certificates, secrets, messages, identities, authority, or artifacts;
- become operational evidence; or
- escape the candidate environment as trusted time.

## 8. Two-Control Verification

VPL-BST-004 SHALL distinguish:

1. **External Bootstrap Control:** identifies environment, execution, source inputs, fault injection, and evidence independently of the candidate.
2. **Candidate Control:** produces the time behavior and evidence claimed by the candidate.

When both controls share a host, source, hypervisor, provider, network, oscillator, actor, or root of trust:

- the dependency SHALL be declared;
- the observations SHALL not be called independent;
- uncertainty SHALL include applicable shared risk; and
- `VERIFIED` Activation Claims SHALL require the active Deployment Profile's approved evidence.

## 9. Activation Boundary

The Falcon Time Provider becomes mandatory only after:

1. CON-015 is active as the governing Contract;
2. the exact Provider candidate, Time Profile, Source Set, and environment are identified;
3. VPL-BST-004 produces `PASS`;
4. canonical UTC and Runtime Epoch behavior are verified;
5. uncertainty, leap behavior, source conflict, drift, discontinuity, and failure cases pass;
6. required evidence is complete and valid;
7. independent evaluation is accepted for the declared Temporal Decision Profiles;
8. a competent Profile Activation Authority issues an Activation Decision; and
9. the exact Provider Profile and effective boundary become `ACTIVE`.

After that boundary:

- every new Falcon runtime SHALL obtain governed time through CON-015;
- direct platform, runtime, database, hypervisor, cloud, device, or network clock use is prohibited;
- `BOOTSTRAP_EXTERNAL` and candidate time SHALL not satisfy operational Time Observation fields;
- operational Clock Quality SHALL follow TIM-001 and the active Deployment Profile; and
- Provider failure SHALL cause quality reduction or rejection, not external fallback.

## 10. Import and Historical Reconciliation

When external bootstrap evidence enters Falcon-governed custody after Provider Activation:

- original external time SHALL remain unchanged;
- Falcon import time SHALL be recorded separately;
- external and Falcon observation IDs SHALL be cross-linked;
- source, environment, resolution, uncertainty, continuity, and limitations SHALL remain visible;
- no Falcon Clock Quality SHALL be assigned retroactively;
- no external time shall become `VERIFIED`;
- transformations SHALL preserve original bytes and evidence;
- ordering Claims SHALL remain bounded by original uncertainty; and
- the import event SHALL conform to CON-008 and CON-021.

Import time is not occurrence time.

## 11. Activation Evidence Reevaluation

After Time Provider Activation, earlier bootstrap and candidate Activation evidence SHALL be reevaluated only where Falcon time semantics are material.

Reevaluation SHALL:

- preserve the original decision and evidence;
- identify the exact temporal Claim under review;
- use current Falcon Time Observations only for the reevaluation event;
- never alter the original external timestamp;
- state whether the original decision remains valid for its declared scope;
- create a new Derived Evaluation and lineage;
- require the competent Evaluation and Acceptance Authorities; and
- restrict the subject when residual temporal uncertainty is unacceptable.

Reevaluation SHALL NOT manufacture historical `VERIFIED` time.

## 12. Provider Failure After Activation

After the operational boundary, Provider or source failure SHALL:

- downgrade Clock Quality according to the active profile;
- increase uncertainty where required;
- create a new Runtime Epoch when continuity is lost;
- reject temporal decisions whose requirements are no longer met;
- prevent direct or external clock fallback;
- preserve evidence;
- notify Health Monitoring and Self-Awareness;
- notify Guardian where authority, security, capital, or evidence may be harmed; and
- require governed restoration.

Provider return, plausible values, source agreement, restart, or elapsed time SHALL not automatically restore `VERIFIED`.

## 13. Security and Replay Boundary

Before operational Time Provider Activation:

- bootstrap environments SHALL not perform operational FIL expiry or replay decisions;
- candidate tests MAY simulate such decisions only with synthetic messages and explicit expected results;
- external wall time SHALL not authorize certificate, token, key, or authority validity;
- uncertainty SHALL never broaden a validity window; and
- absence of trustworthy time SHALL reduce authority.

After Activation, security-validity decisions SHALL use the applicable active Temporal Decision Profile and full uncertainty interval.

## 14. Evidence

Time evidence SHALL additionally preserve:

- bootstrap or candidate classification;
- external source and environment;
- source mechanism and version;
- wall and monotonic observations separately;
- external continuity boundary;
- resolution, uncertainty, and leap behavior;
- candidate Profile and subject;
- shared dependency disclosures;
- Activation boundary;
- import and cross-link events;
- reevaluation and lineage;
- Quality downgrade and restoration decisions; and
- responsible authorities.

Approval, import, signing, plausible agreement, or later Provider Activation SHALL not upgrade historical Clock Quality.

## 15. Requirements Added

- **TIM-001-REQ-031:** Pre-Activation time SHALL be classified `BOOTSTRAP_EXTERNAL` and SHALL not establish Falcon Clock Quality.
- **TIM-001-REQ-032:** Every bootstrap observation SHALL preserve source, environment, mechanism, wall and monotonic values, resolution, uncertainty, continuity, provenance, and limitations.
- **TIM-001-REQ-033:** `BOOTSTRAP_EXTERNAL` SHALL not be a Falcon Time Profile or Clock Quality state.
- **TIM-001-REQ-034:** External wall and monotonic observations SHALL remain separate and SHALL not cross unproven continuity boundaries.
- **TIM-001-REQ-035:** Bootstrap temporal decisions SHALL evaluate uncertainty conservatively and remain bounded to preparation or candidate scope.
- **TIM-001-REQ-036:** A decision requiring Falcon `VERIFIED` time SHALL be deferred or denied before Provider Activation.
- **TIM-001-REQ-037:** Candidate Provider output SHALL remain synthetic or non-production, `CANDIDATE`, isolated, and incapable of self-Activation.
- **TIM-001-REQ-038:** Shared source and control dependencies SHALL be declared and SHALL not be mislabeled as independent.
- **TIM-001-REQ-039:** CON-015 operational time SHALL become mandatory only at the recorded Provider Activation boundary.
- **TIM-001-REQ-040:** After Activation, direct or external clock fallback SHALL be prohibited.
- **TIM-001-REQ-041:** Import SHALL preserve external time and record Falcon import time separately without retroactive Quality assignment.
- **TIM-001-REQ-042:** Reevaluation SHALL preserve original evidence and SHALL not manufacture historical `VERIFIED` time.
- **TIM-001-REQ-043:** Provider failure SHALL downgrade or reject temporal decisions and create a new Runtime Epoch when continuity is lost.
- **TIM-001-REQ-044:** Provider return SHALL not automatically restore `VERIFIED` quality or authority.
- **TIM-001-REQ-045:** Approval of TIM-001 v1.1 SHALL not activate a Time Profile, Source, or Provider or establish `VERIFIED` time.

## 16. Conformance Evidence Added

Activation requires evidence that:

- external observations retain correct classification and complete metadata;
- wall and monotonic values remain separate;
- cross-boundary monotonic comparison is rejected;
- decisions requiring `VERIFIED` time are blocked before Activation;
- uncertainty-overlapping boundaries produce conservative results;
- candidate observations cannot escape as operational time;
- shared source dependencies are detected and disclosed;
- VPL-BST-004 controls the candidate case;
- the exact Activation boundary is reconstructable;
- post-Activation runtimes use only CON-015;
- direct and external clock fallback is prevented;
- imported evidence preserves external time and separate import time;
- reevaluation creates new lineage without historical Quality upgrade;
- Provider failure causes Quality downgrade, restriction, and new epoch where required; and
- restoration requires new evidence and competent authority.

## 17. Required Before Time Provider Activation

The operational Time Profile SHALL remain non-active until:

1. TIM-001 v1.1 is Approved;
2. CON-015 is Approved and applicable;
3. FCE-001 canonical timestamp encoding is active for the declared scope;
4. IDN-001 and CON-014 can issue Provider, Source, Observation, and Runtime Epoch identifiers where required;
5. the exact Provider, Source Set, environment, configuration, and artifact identities are known;
6. VPL-BST-004 produces `PASS`;
7. all applicable Temporal Decision Profiles and failure cases are verified;
8. the Evidence Requirement Set is complete;
9. validity and security review are accepted;
10. a competent Authority Instrument permits Activation; and
11. a separate exact Activation Decision is recorded.

## 18. Supersession

- TIM-001 v1.1 supersedes v1.0;
- existing Time Profiles, Clock Quality states, capabilities, source classes, and thresholds remain unchanged;
- no existing external, candidate, platform, database, or directly observed time is grandfathered;
- historical evidence retains its original origin, uncertainty, continuity, and limitations;
- no Time Provider, Source, or Profile becomes active through the Catalog amendment; and
- no `VERIFIED` time is established by the version change.

## 19. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner and current Constitutional Authority | Approved and Activated | GOV-035 | 2026-07-25 |

This Approval activates TIM-001 v1.1 as the controlling Catalog and archives v1.0.

It does not:

- activate a Time Provider, Source, or Profile;
- establish Falcon `VERIFIED` time;
- convert or upgrade external or candidate time;
- authorize candidate construction or verification execution;
- authorize implementation;
- authorize production;
- authorize financial connectivity; or
- authorize financial activity.
