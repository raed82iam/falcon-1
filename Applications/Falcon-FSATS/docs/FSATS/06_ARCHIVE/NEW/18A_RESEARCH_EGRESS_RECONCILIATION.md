# FSATS SIA — Awareness Research-Egress Reconciliation

**Package:** `FSATS-SIA-v0.1`
**Status:** `DESIGN_CANDIDATE / CONTROLLING RESEARCH-BOUNDARY REMEDIATION`
**Governing Inputs:** accepted Part 0 Awareness amendment; FCR-0008; FCR-0012; ADR-I015

## 1. Purpose

Correct an over-restrictive interpretation in file 18 before semantic freeze.

File 18 correctly preserved the accepted Trading-specific rule that `MSA-TRD` has no direct Internet access and that Trading Awareness research routes through governed FSTSimA research/sandbox behavior.

However, file 18 also stated too broadly that other Application Awareness should use FSTSimA rather than direct governed research egress. That statement is not supported as a universal accepted rule:

- the accepted Trading Awareness amendment intentionally resolved the Trading MSA direct-Internet question;
- non-Trading Application direct-research policy was not globally prohibited by that Trading-specific amendment;
- FCR-0008 explicitly records a future generic governed research-only Internet egress need for Falcon Applications using MSA/LSA/eligible CSA;
- FCR-0008 is `ACCEPTED_FOR_PLANNING`, `Waiting On: NONE`, target Foundation Stage 12, and grants no current runtime capability;
- FSA direct Internet remains separately prohibited under the current FCR-0012 Owner requirement.

This file is the controlling SIA reconciliation for research egress.

## 2. Universal Separation

For every Application/Awareness entity:

```text
RESEARCH_EGRESS != OPERATIONAL_DATA_EGRESS
RESEARCH_RESULT != OPERATIONAL_DATA_PRODUCT
RESEARCH_RESULT != BUSINESS_AUTHORITY
RESEARCH_RESULT != PRODUCTION_ADOPTION
```

Research exists for learning, discovery, evaluation and candidate development only.

Operational market/reference/provider data remains FSAPMA-owned and follows the operational provider-data path.

## 3. Current Runtime Availability

At this SIA semantic candidate:

```text
FCR-0008 RUNTIME CAPABILITY = NOT_IMPLEMENTED / NOT_AVAILABLE
DIRECT GOVERNED APPLICATION RESEARCH EGRESS = NOT_CURRENTLY_USABLE
LOCAL INTERNET WORKAROUND = FORBIDDEN
```

No Application may implement arbitrary direct `HttpClient`, browser automation, shell download or provider SDK access as a substitute for the future Foundation Stage 12 research egress boundary.

## 4. Trading Awareness Rule — Explicit Current Prohibition

The accepted Part 0 Awareness amendment remains controlling:

```text
MSA-TRD DIRECT INTERNET = PROHIBITED
TRADING AWARENESS TRUSTED-RUNTIME INTERNET = PROHIBITED
```

Trading research route, when the required Foundation capability exists:

```text
Trading CSA/LSA/MSA research need
-> Trading origin-correct proposal/request
-> governed FSTSimA specialized research/sandbox activity
-> Foundation Stage 12 research-only egress boundary
-> destination/tool/identity/purpose policy
-> quarantined research material + provenance
-> FSTSimA analysis/simulation/adversarial validation
-> evidence package
-> Trading origin/parent/MSA evaluation
```

Raw Internet content SHALL NOT feed Trading operational decision/execution directly.

This route is intentionally stronger than the generic future egress allowance because it preserves the accepted Trading-specific containment decision.

## 5. FSAPMA Awareness Future Research Eligibility

For APP-PMA MSA/LSA/eligible CSA, the SIA does **not** impose the Trading-specific FSTSimA-only route as a universal rule.

When and only when Foundation Stage 12/FCR-0008 capability exists, a specific APP-PMA Awareness entity MAY be granted direct **governed research-only** egress if all of these are explicitly declared/authorized:

```text
Exact Application/Awareness identity
ResearchPurposeClass
Allowed tool/profile
Allowed destination/domain/category policy
Read/write candidate scope
Resource ceiling
Data classification
Evidence/provenance capture
Revocation path
No operational-provider use through the research channel
No active business-state mutation from research result
```

Candidate examples:

- research provider API changes/documentation for future adapter candidates;
- study data-quality methods;
- study quota/reliability modeling techniques.

Forbidden through research channel:

- fetching current quotes/trades to satisfy operational Data Product demand;
- bypassing FSAPMA operational provider routing/quota/certification;
- acquiring credentials/entitlements outside governed credential boundary;
- turning research webpage/API response directly into current provider capability truth without certification.

## 6. Guardian Awareness Future Research Eligibility

For APP-GRD MSA/LSA/eligible CSA, future direct governed research-only egress MAY be eligible under FCR-0008 after explicit manifest/permission/tool/destination authorization.

Allowed purpose examples:

- research incident-detection methods;
- research reliability/safety techniques;
- research public technical standards or security advisories relevant to future candidates.

Forbidden:

- treating Internet content as a current operational incident trigger without the normal governed operational evidence path;
- issuing a Guardian directive because a research source says an event occurred;
- using research channel to bypass APP-PMA for operational market/provider truth;
- acquiring new protection authority from external material.

## 7. FSTSimA Awareness Future Research Eligibility

APP-SIM is the preferred specialized environment for research that requires:

- external method discovery;
- dataset/method investigation;
- candidate experimentation;
- adversarial scenario construction;
- strategy/model validation;
- Trading-contained research.

When FCR-0008 exists, eligible FSTSimA MSA/LSA/CSA may use direct governed research-only egress under exact permission/tool/destination policy.

This does not create operational provider/broker access and does not weaken FCR-0011 non-Live isolation.

Any external dataset acquired for validation becomes an immutable, classified research/validation input with exact provenance and license/usage constraints before use.

## 8. APP-RSC Candidate Research Policy

If APP-RSC is later Owner-accepted, its initial SIA research policy is:

```text
APP-RSC DIRECT RESEARCH EGRESS = DISABLED BY DEFAULT
```

Reason: initial FSARM resource algorithms are deterministic governance/control logic, and no present implementation requirement proves a need for direct Internet research in trusted APP-RSC operation.

A future research need would require a separate APP-RSC Awareness eligibility/purpose decision under the generic FCR-0008 boundary. It cannot be inferred from APP-RSC existence.

## 9. FSA Rule

Current FCR-0012 Owner requirement remains controlling:

```text
FSA DIRECT INTERNET = PROHIBITED
```

Applications SHALL NOT use this SIA to prescribe an alternative FSA internal research mechanism. Any Foundation-side research capability for FSA is a separate Foundation/governance matter.

## 10. LSA / CSA Eligibility

Generic FCR-0008 names Applications using MSA/LSA/eligible CSA research capability.

Therefore future direct governed research egress is not automatically MSA-only. An LSA or eligible CSA may receive a narrower research permission if:

- exact identity and parent chain are declared;
- research purpose is within its responsibility;
- destination/tool scope is narrower than or equal to Application policy;
- candidate write scope is limited to owned isolated assets;
- MSA remains able to evaluate Application-wide implications;
- no authority expansion occurs.

A deterministic/passive component cannot gain CSA merely to obtain Internet access.

## 11. Research Tool Profile

Every future research-capable entity uses a versioned `ResearchToolProfile`:

```text
ResearchToolProfileId
ApplicationId
AwarenessEntityId
AllowedResearchPurposes[]
AllowedToolIds/Versions[]
AllowedDestinationPolicyId
AllowedContentTypes
MaximumRequest/ResponseBytes
MaximumConcurrency
Rate/ResourceCeilings
DownloadPolicy
ExecutableContentPolicy
CredentialPolicy = NO_APPLICATION_OPERATIONAL_CREDENTIALS
QuarantinePolicy
Malware/ContentInspectionPolicy
ProvenanceCapturePolicy
RetentionPolicy
CandidateWriteScopes[]
RevocationAuthorityRef
EvidencePolicy
```

Missing profile or mismatched entity => no egress.

## 12. Research Material State

External research material lifecycle:

```text
RECEIVED_UNTRUSTED
-> QUARANTINED
-> INSPECTED
-> PROVENANCE_BOUND
-> ELIGIBLE_FOR_RESEARCH_ANALYSIS
-> CANDIDATE_EVIDENCE
```

Possible terminal/side states:

```text
REJECTED_UNSAFE
REJECTED_PROVENANCE_INSUFFICIENT
REJECTED_OUT_OF_SCOPE
EXPIRED
SUPERSEDED
```

`ELIGIBLE_FOR_RESEARCH_ANALYSIS` does not mean operational/trusted production input.

## 13. Operationalization Barrier

If research discovers a useful method/data source/tool:

```text
RESEARCH DISCOVERY
-> candidate design/proposal
-> isolated implementation/experiment
-> FSTSimA or equivalent governed validation as applicable
-> parent/LSA/MSA review
-> FSA compatibility review where required
-> Owner/governance decision
-> separate Application update/implementation/deployment lifecycle
```

No copy/paste or downloaded artifact may cross directly into active production code/config/model state.

## 14. Provider Documentation Special Case

FSAPMA may research provider documentation as research evidence, but operational provider certification remains a distinct process:

```text
RESEARCH DOC FINDING
!= CERTIFIED PROVIDER CAPABILITY
```

Certification requires the exact current official evidence/fixture/profile process from file 16.

## 15. Security / Privacy

Research egress SHALL prevent:

- secret/API-key leakage into queries/prompts/URLs;
- sending account/position/user-sensitive data unless a separately approved sanitized research profile explicitly allows a bounded class;
- arbitrary executable download/run in trusted runtime;
- destination redirects escaping policy;
- DNS/URL indirection bypassing destination policy;
- downloaded content obtaining local credential/network authority;
- research content manipulating system instructions/policies without validation.

## 16. Research Evidence

Every material research session/result used in a candidate records:

```text
ResearchSessionId
AwarenessEntityId
Purpose
Tool/Profile version
Destination evidence
Request classification/digest
Response/content digest
RetrievedAt
Provenance/source metadata
Quarantine/inspection outcome
Derived findings
Candidate refs
Limitations/conflicts
```

## 17. Failure Rules

- Stage 12 capability unavailable -> research request unavailable/fail closed, not local bypass.
- destination not allowed -> deny.
- redirect escapes policy -> deny.
- content inspection/provenance fails -> keep/reject quarantine.
- operational data request on research channel -> reject + integrity/policy signal if material.
- research result conflicts with current canonical operational truth -> do not overwrite operational truth; retain as research discrepancy for investigation.

## 18. Reconciliation Result

This file supersedes only the over-broad non-Trading research restriction in file 18.

Controlling result:

```text
TRADING MSA DIRECT INTERNET = PROHIBITED
TRADING AWARENESS RESEARCH = FSTSimA-CONTAINED ROUTE
NON-TRADING APPLICATION AWARENESS DIRECT GOVERNED RESEARCH = FUTURE ELIGIBILITY POSSIBLE, NOT CURRENT CAPABILITY
FSTSimA = PREFERRED SPECIALIZED RESEARCH/VALIDATION ENVIRONMENT
APP-RSC INITIAL DIRECT RESEARCH = DISABLED
FSA DIRECT INTERNET = PROHIBITED
RESEARCH != OPERATIONAL DATA
FCR-0008 RUNTIME = FUTURE / FAIL CLOSED NOW
```

## 19. Verification Families

Verifier SHALL reject:

1. Trading MSA direct Internet permission;
2. Trading CSA/LSA raw direct egress bypassing FSTSimA accepted route;
3. non-Trading direct research usage before FCR-0008 runtime capability exists;
4. research egress with no exact Awareness identity/profile/purpose;
5. operational market/provider data obtained through research channel;
6. research result used directly as operational authority/data;
7. operational credentials exposed to research tool;
8. unquarantined downloaded artifact executed in trusted runtime;
9. FSA direct Internet declaration;
10. deterministic component granted CSA only to obtain egress;
11. APP-RSC direct egress enabled by default;
12. provider documentation finding represented as certified current capability;
13. research artifact promoted without candidate/validation/governance lifecycle.
