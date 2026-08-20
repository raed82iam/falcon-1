# STAGE 1 Canonical Exit-Gate Determination

## Canonical Stage 1 definition

**Canonical Stage 1 name:** Controlled Project Foundation

**Canonical purpose:** establish the controlled project boundary for Falcon Foundation work without implementing Falcon runtime behavior.

Canonical basis:

- `docs/plans/IMP-001_FOUNDATION_IMPLEMENTATION_WORK_PLAN.md` — section 5, “Stage 0 Replacement”; section 6, “Stage 1 Amendment”
- `docs/releases/FRS-001_FOUNDATION_RELEASE.md` — sections 2, 3, 4, 5, 7, and 8
- `docs/governance/GOV-059_STAGE_0C_COMPLETION_AND_CLOSURE.md` — sections 1, 6, and 7

### Permitted execution scope

The active canonical baseline permits controlled project foundation work that prepares the repository, environment, dependency, evidence, traceability, and boundary structure for Falcon Foundation without becoming Falcon runtime behavior.

Canonical support:

- `IMP-001` section 5.1 through 5.4: Foundation Preparation Authority, Enabling-Provider Candidate Authority, Verification Execution Authority, and Profile Activation Authority are separately bounded.
- `IMP-001` section 6: Stage 1 begins only after the Foundation Implementation Gate passes.

### Prohibited execution scope

Stage 1 SHALL NOT implement Falcon runtime behavior, financial operation, production operation, or unauthorized external connectivity.

Canonical support:

- `FRS-001` section 2, Release Principle: Falcon shall not trade, connect to a broker, allocate capital, run financial intelligence, or claim production financial readiness.
- `IMP-001` sections 5.1, 5.2, 5.3, 5.4, and 6: each stage remains separately authorized and non-financial.
- `GOV-059` sections 1, 6, and 7: Stage 1 proposal, Stage 1 preparation, operational Falcon, production, cloud, financial connection, and financial activity remain unauthorized.

### Mandatory deliverables

Canonical Stage 1 focuses on the controlled project boundary and its governed evidence structure, not on behavioral runtime implementation.

Supported deliverables from the controlling canonical baseline include:

- exact repository and dependency boundary controls (`ADR-I002`);
- reproducible build and verification governance (`ADR-I007`);
- exact runtime/language basis (`ADR-I001`);
- governed pipeline, evidence, and traceability structure (`PIPE-001`, `TRC-001`);
- exact build baseline and environment admission (`BLD-001`, `ENV-001`);
- constitutional and scope compliance evidence (`FRS-001`, `GOV-059`, `TRC-001`).

### Mandatory entry conditions

Before Stage 1 can proceed as a controlled project foundation, the active canonical baseline requires:

- Stage 0 completion and closure (`GOV-059`);
- current non-financial and non-production authority boundaries;
- active governed build, environment, traceability, and pipeline baselines;
- exact dependency admission and lock controls (`ADR-I002`, `BLD-001`);
- exact environment and manifest revalidation (`ENV-001`, `TRC-001`, `PIPE-001`).

### Mandatory exit conditions

The canonical baseline requires a controlled foundation boundary that remains non-operational, non-financial, and reproducible.

Relevant canonical support:

- `FRS-001` section 3, “Required Demonstration”;
- `FRS-001` section 7, “Required Conditions”;
- `IMP-001` section 6, Stage 1 Amendment.

### Mandatory evidence

Required evidence in the controlling canonical baseline includes:

- dependency provenance and lock evidence (`ADR-I002`, `BLD-001`);
- environment admission and manifest evidence (`ENV-001`);
- traceability evidence (`TRC-001`);
- reproducible build and verification evidence design (`ADR-I007`);
- constitutional and scope evidence (`FRS-001`, `GOV-059`).

### Explicitly deferred work

The canonical baseline defers Falcon runtime behavior, production behavior, and financial behavior beyond the controlled project foundation boundary.

Supporting citations:

- `FRS-001` sections 2 and 7;
- `GOV-059` sections 1, 6, and 7;
- `IMP-001` sections 5 and 6.

## Exit-gate requirement table

| Exit Requirement ID | Exact canonical requirement | Source | Mandatory in Stage 1 | Requires Falcon runtime or behavioral execution | Requires controlled build or verification command execution | Requires external package | Required evidence | Interpretation confidence |
|---|---|---|---|---|---|---|---|---|
| ER-01 | repository boundary and canonical solution identity | `IMP-001` section 5.1; `ADR-I002` | Yes | No | Yes | No | boundary map, repository-relative paths | EXPLICIT |
| ER-02 | deterministic empty build | `IMP-001` section 6; `FRS-001` section 3 | Yes | No | Yes | No | reproducible empty-build design evidence | EXPLICIT |
| ER-03 | dependency restore and lock integrity | `ADR-I002`; `BLD-001` | Yes | No | Yes | No | locked versions, provenance, approved sources | EXPLICIT |
| ER-04 | formatting, compiler, and static analysis | `ADR-I007` section 5.19–5.22; `IMP-001` section 6 | Yes | No | Yes | No | governed command design and analysis rules | STRONGLY_IMPLIED |
| ER-05 | project-reference and architecture-boundary verification | `ADR-I002`; `ADR-F001`; `TRC-001` | Yes | No | Yes | No | dependency graph and boundary evidence | EXPLICIT |
| ER-06 | secret review and prohibited-path exclusion | `ADR-I002`; `ADR-I007` sections 5.21 and 5.25; `FRS-001` section 3 | Yes | No | Yes | No | inspection and exclusion evidence | EXPLICIT |
| ER-07 | security review | `FRS-001` sections 3 and 7; `ADR-I007` section 5.21 | Yes | No | Yes | No | security-review evidence | STRONGLY_IMPLIED |
| ER-08 | financial-path exclusion | `FRS-001` sections 2 and 7; `GOV-059` section 6 | Yes | No | Yes | No | no-financial-path proof | EXPLICIT |
| ER-09 | environment revalidation | `ENV-001`; `IMP-001` section 5.3; `PIPE-001` | Yes | No | Yes | No | environment admission and manifest revalidation evidence | EXPLICIT |
| ER-10 | Activation Manifest revalidation | `ENV-001`; `TRC-001`; `PIPE-001` | Yes | No | Yes | No | manifest revalidation evidence | EXPLICIT |
| ER-11 | evidence output | `TRC-001`; `ADR-I007` | Yes | No | Yes | No | evidence-location and retention design | EXPLICIT |
| ER-12 | traceability output | `TRC-001` | Yes | No | Yes | No | trace output design and preservation | EXPLICIT |
| ER-13 | artifact identity | `TRC-001`; `ADR-I007` | Yes | No | Yes | No | artifact identity rules | EXPLICIT |
| ER-14 | behavioral unit tests | `IMP-001`; `FRS-001`; `ADR-I007` | No | Yes | Yes | Yes | not canonically required for Stage 1 exit gate | NOT_SUPPORTED |
| ER-15 | behavioral integration tests | `IMP-001`; `FRS-001`; `ADR-I007` | No | Yes | Yes | Yes | not canonically required for Stage 1 exit gate | NOT_SUPPORTED |
| ER-16 | test-result collection | `ADR-I007` | No | Yes only if behavioral tests are later required | Yes only if behavioral tests are later required | Yes, if actually used | test-results evidence model | AMBIGUOUS |
| ER-17 | coverage | `ADR-I007` | No | Yes only if behavioral tests are later required | Yes only if behavioral tests are later required | Yes, if actually used | coverage evidence | AMBIGUOUS |
| ER-18 | generated SBOM | `ADR-I007` section 5.26; `BLD-001` | No for this exit gate | No | No | Yes if later admitted | SBOM evidence | DEFERRED |
| ER-19 | provenance generation | `ADR-I007` section 5.26; `BLD-001` | Yes for documentation and governance | No | Yes | No | provenance binding to artifact and dependency inputs | STRONGLY_IMPLIED |
| ER-20 | attestation or signing | `ADR-I007` sections 5.26 and 5.27 | No for this exit gate | No | No | Yes if used | attestation evidence | DEFERRED |

## Behavioral test determination

**Does Stage 1 implement Falcon runtime or business behavior?**  
`NO`

Canonical support:

- `FRS-001` section 2: the Foundation Release shall not trade, connect to a broker, allocate capital, or run financial intelligence.
- `GOV-059` section 6: Stage 1 proposal, Stage 1 preparation, operational Falcon, production, cloud, financial connection, and financial activity are not authorized.
- `IMP-001` section 6: Stage 1 remains the Controlled Project Foundation and is bounded by the Foundation Implementation Gate.

**Does the canonical Stage 1 exit gate explicitly require behavioral test execution?**  
`NO_NOT_REQUIRED`

Rationale:

- The controlling canonical baseline requires governed build, dependency, environment, traceability, evidence, architecture, secret, security, and financial-exclusion controls.
- It does not explicitly make behavioral test execution a mandatory Stage 1 exit condition for the Controlled Project Foundation itself.
- The canonical material that does mention test execution does so in the context of broader governed verification or later-stage verification planning, not as a universal requirement for this Stage 1 boundary.

## Test-stack component determination

| Component | Function | Canonical requirement served | Classification | Canonical evidence | Admission implication |
|---|---|---|---|---|---|
| Microsoft.Testing.Platform 2.3.2 | test-runner orchestration | behavioral test execution, if later required | `DEFERRED_TO_LATER_STAGE` | `ADR-I007` section 5.19 and 5.22; `IMP-001` section 6 | no Stage 1 admission requirement shown |
| MSTest 4.3.2 | test framework | behavioral test execution, if later required | `DEFERRED_TO_LATER_STAGE` | `ADR-I007` section 5.19 and 5.22 | no Stage 1 admission requirement shown |
| Microsoft.NET.Test.Sdk 18.8.1 | SDK test host integration | behavioral test execution, if later required | `DEFERRED_TO_LATER_STAGE` | `ADR-I007` section 5.19, 5.22, 5.26 | no Stage 1 admission requirement shown |

## Other tool-capability determinations

Stage 1 requires the following only to the extent needed for the controlled foundation exit gate:

- dedicated secret scanner: `NO`
- dedicated security scanner: `NO`
- dedicated dependency or vulnerability scanner: `NO`
- generated SBOM tool: `NO` for Stage 1 exit gate, `DEFERRED` as a later governed need
- architecture-test framework: `NO` as a dedicated external tool requirement
- coverage collector: `NO`
- provenance generator: `NO` as a dedicated Stage 1 external-tool admission requirement
- attestation signer: `NO`

The canonical baseline requires the evidence and governance outcomes, but it does not make each of those outcomes depend on a newly admitted dedicated external tool for Stage 1.

## Impact on existing review packages

**`S1-TCAP-005 = MANDATORY_CAPABILITY_GAP`** → `NOT_SUPPORTED`

Reason:

- the canonical exit gate does not make behavioral test execution a mandatory Stage 1 exit condition for Controlled Project Foundation;
- the existing admitted mechanisms satisfy the actual Stage 1 exit gate without proving a dedicated behavioral-test tool stack;
- behavioral testing is deferred to the first stage that actually requires behavioral runtime evidence.

**`CAND-001`** → `NOT_APPLICABLE`

Reason:

- the candidate test stack is not a required Stage 1 admission object when the canonical exit gate is satisfied by already admitted mechanisms;
- no new Stage 1 tool admission decision is required on the canonical baseline for this specific question.

## Final determination

**Project Stage 1:** `CONTROLLED_PROJECT_FOUNDATION`

**Falcon runtime behavior implemented in Stage 1:** `NO`

**Behavioral test execution canonically required:** `NO_NOT_REQUIRED`

**S1-TCAP-005 classification:** `NOT_SUPPORTED`

**CAND-001 classification:** `NOT_APPLICABLE`

**Dedicated test tool admission required:** `NO`

**Dedicated secret scanner required:** `NO`

**Dedicated security scanner required:** `NO`

**Generated SBOM tool required:** `NO`

**Final determination:** `NO_NEW_STAGE_1_TEST_TOOL_ADMISSION_REQUIRED`

Canonical documents modified:
NO

Proposal packages modified:
NO

Implementation performed:
NO
