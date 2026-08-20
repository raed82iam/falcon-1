# FMOF / FSAPMA Hardening — Awareness-Driven Provider Capability-Gap Research and Discovery

**Package:** `FSATS-FMOF-PROPOSAL-001`  
**Applies To:** `00` + `01` + `02` + `03` + `04` of this proposal package  
**Decision Type:** `PROJECT OWNER DIRECTED DESIGN HARDENING / AWARENESS-DRIVEN CAPABILITY DISCOVERY`  
**Classification:** `DCC-3 — MATERIAL_DOMAIN_CHANGE`  
**Status:** `DESIGN_CHANGE_HARDENING / OUTSIDE_CURRENT_R7_FREEZE / NOT_OWNER_ACCEPTED / NOT_CLOSED`  
**Branch:** `application-development`  
**Workspace:** `applications/docs/FSATS/NEW-2/`  
**Implementation Authority:** `NOT_GRANTED`  
**Runtime Authority:** `NOT_GRANTED`  
**Research Internet Egress Authority:** `NOT_GRANTED BY THIS RECORD`  
**Provider / Broker Connectivity Authority:** `NOT_GRANTED`  
**Credential Provisioning Authority:** `NOT_GRANTED`  
**Paper / Tiny Live / Live / Deployment Authority:** `NOT_GRANTED`  

---

# 1. Purpose

This hardening records the Project Owner's clarification that a currently unsatisfied zero-cost Data Product requirement is not merely an operational dead end.

When FSAPMA cannot find a currently certified zero-cost route that satisfies the exact semantics of a `DataProductRequest`, the immediate operational truth remains unavailable or degraded as applicable, but the capability gap may also become an Awareness-owned research and improvement problem.

The responsible Application Awareness entity may identify the gap, investigate why it exists, initiate governed research, discover candidate free providers or candidate lawful zero-cost compositions, and submit an evidence-backed improvement proposal through the normal origin-aware review chain.

The central correction is:

```text
NO CURRENT CERTIFIED ZERO-COST ROUTE
!=
NO POSSIBLE ZERO-COST SOLUTION EXISTS
```

But also:

```text
RESEARCH DISCOVERY
!=
CERTIFIED PROVIDER ROUTE
!=
RUNTIME ACTIVATION
```

---

# 2. Controlling Owner Intent

The current personal evaluation objective remains aggressively `FREE_FIRST`.

Falcon should not stop learning merely because the currently onboarded provider portfolio cannot satisfy a request.

If an exact requirement is valuable and currently unavailable at `CostCeiling = 0`, Falcon should be capable of asking:

> Is there another legitimate free provider, free plan, official public source, broker-entitled source, or legally compatible zero-cost combination that can satisfy the exact semantic requirement?

This investigation belongs inside the relevant Application Awareness responsibility and shall not be converted into an ungoverned provider-selection shortcut.

Example:

```text
RequiredProduct = CONSOLIDATED_NBBO
TemporalRequirement = REALTIME_REQUIRED
CostCeiling = 0

CurrentCertifiedRoutes = NONE
```

The correct behavior is not simply:

```text
RETURN UNAVAILABLE
STOP THINKING FOREVER
```

The hardened behavior is:

```text
CURRENT OPERATIONAL REQUEST
    -> HONESTLY RETURN UNAVAILABLE / DEGRADED AS DECLARED

AND, WHEN RESEARCH-ELIGIBLE:
    -> RECORD CAPABILITY GAP
    -> INITIATE GOVERNED AWARENESS RESEARCH
    -> SEARCH FOR EXACT ZERO-COST SOLUTION
    -> PRODUCE EVIDENCE-BACKED CANDIDATE
    -> REVIEW / CERTIFY / ONBOARD THROUGH GOVERNED PATH
    -> ONLY A LATER AUTHORIZED ROUTE MAY BECOME OPERATIONALLY ELIGIBLE
```

---

# 3. Immediate Operational Truth and Improvement Truth Are Separate

A live or operational Data Product request must never wait indefinitely for an Awareness research cycle to invent a new provider.

The current request is evaluated only against currently certified and authorized routes.

Therefore:

```text
CURRENT_REQUEST_RESULT
=
TRUTH ABOUT CURRENTLY AVAILABLE CERTIFIED CAPABILITY
```

while:

```text
CAPABILITY_GAP_RESEARCH
=
SEPARATE IMPROVEMENT / DISCOVERY LIFECYCLE
```

If no current certified route satisfies the request, the operational response remains one of the explicitly governed states such as:

- `UNAVAILABLE_AT_CURRENT_COST_CEILING`;
- `SEMANTIC_REQUIREMENT_UNSATISFIED`;
- `COVERAGE_DEGRADED`;
- `ENTITLEMENT_UNAVAILABLE`;
- `CAPACITY_UNAVAILABLE`;
- another exact governed reason established by the final design.

Awareness research does not retroactively make the failed request successful.

---

# 4. Capability-Gap Artifact

A missing provider capability should become an attributable first-class artifact rather than a forgotten log message.

Conceptual type:

```text
ProviderCapabilityGap
{
    GapId
    GapFingerprint
    OriginatingRequestType
    RequiredDataProduct
    MarketScope
    InstrumentScope
    RequiredFields
    RequiredFreshness
    RequiredLatency
    RequiredHistoryDepth
    RequiredVenueScope
    RequiredConsolidationSemantics
    RequiredAdjustmentSemantics
    RequiredQualityFloor
    RequiredUsagePurpose
    CostCeiling
    CurrentEligibleRouteCount
    RejectedRouteReasons[]
    BusinessImpact
    CoverageImpact
    RecurrenceCount
    FirstObservedAt
    LastObservedAt
    CurrentResearchState
    EvidenceRefs[]
}
```

The `GapFingerprint` shall represent the semantic gap rather than one vendor name.

Example:

```text
US_EQUITIES
+ REALTIME
+ CONSOLIDATED_NBBO
+ OWNER_ONLY_NONCOMMERCIAL
+ COST_ZERO
```

is one capability gap even if many different providers failed it.

---

# 5. FSAPMA LSA Ownership

The current Part 1 decomposition defines six FSAPMA branches:

1. `P-LSA-01 Provider Registry and Onboarding`
2. `P-LSA-02 Data Products, Semantics and Normalization`
3. `P-LSA-03 Provider Capability, Account and Entitlement`
4. `P-LSA-04 Provider Selection, Routing and Delivery`
5. `P-LSA-05 Data Quality, Verification and Reconciliation`
6. `P-LSA-06 Quota, Capacity, Cost and Reliability`

The capability-gap lifecycle shall preserve those responsibilities rather than create a new unbounded research owner.

## 5.1 P-LSA-04 — Gap Detection at Route Selection

`P-LSA-04` is the natural operational detection point when no eligible route survives the request-first hard gates.

It may produce the exact rejection evidence showing why current routes failed.

It does not gain authority to weaken the request or activate an unknown provider.

## 5.2 P-LSA-02 — Semantic Requirement Truth

`P-LSA-02` owns the exact Data Product semantics needed to determine what would genuinely satisfy the gap.

For example:

```text
IEX_REALTIME
!=
CONSOLIDATED_NBBO_REALTIME
```

A research candidate cannot pass merely because its marketing language uses the word `realtime`.

## 5.3 P-LSA-03 — Capability / Account / Entitlement Evaluation

`P-LSA-03` evaluates whether a discovered candidate actually exposes the required capability under the applicable plan, account and entitlement profile.

It must distinguish technical endpoint existence from entitled capability.

## 5.4 P-LSA-01 — Candidate Provider Registry / Onboarding

A newly discovered provider or newly discovered provider product is not automatically part of the active provider portfolio.

`P-LSA-01` owns the candidate registry/onboarding path needed to move a discovered provider toward later certification.

## 5.5 P-LSA-05 — Quality / Verification / Reconciliation

If a candidate claims to satisfy the semantic requirement, `P-LSA-05` owns the quality and verification evidence needed to challenge that claim.

## 5.6 P-LSA-06 — Zero-Cost, Quota and Reliability Viability

`P-LSA-06` evaluates whether the candidate is genuinely usable within the current cost ceiling and whether its quota/capacity/reliability profile makes it operationally meaningful.

A nominally free product with unusable quota or a short-lived promotional trial does not silently become durable free capacity.

---

# 6. LSA / CSA Awareness Role

The Project Owner explicitly intends the relevant LSA/CSA Awareness to be capable of researching a solution when its owned responsibility lacks a needed capability.

This hardening therefore requires:

```text
RESPONSIBLE LSA
    MAY IDENTIFY CAPABILITY GAP
    MAY INITIATE RESEARCH NEED
    MAY EVALUATE DOMAIN RELEVANCE
    MAY PROPOSE IMPROVEMENT

ELIGIBLE CSA, IF DECLARED FOR THE COMPONENT
    MAY IDENTIFY A MORE SPECIALIZED GAP
    MAY RESEARCH / DEVELOP A CANDIDATE WITHIN ITS OWN RESPONSIBILITY
    MAY PRODUCE EVIDENCE
    MAY PROPOSE THROUGH ITS PARENT LSA
```

CSA eligibility shall remain explicit and governed. This record does not manufacture a new CSA identity or declare every FSAPMA component CSA-eligible.

Provider Controller itself remains an operational controller inside `P-LSA-04` and does not become a CSA merely because it detects a missing route.

---

# 7. Awareness Research Does Not Mean Unrestricted Direct Internet

Current Falcon Awareness authority allows governed research, but research access itself remains subject to approved boundaries.

Therefore:

```text
LSA_OR_CSA_CAN_OWN_THE_RESEARCH_PROBLEM
!=
LSA_OR_CSA_HAS_UNRESTRICTED_DIRECT_INTERNET
```

External research must use the applicable governed research-only path when that capability is authorized and available.

For the current FSATS direction, the bounded FSTSimA research/sandbox path is the intended Trading-domain research mechanism where applicable, while Foundation `FCR-0008` remains the future generic research-only Internet-egress dependency and `FCR-0011` remains the non-Live isolation/egress dependency.

This proposal does not claim those runtime capabilities are currently implemented or authorized.

Conceptual separation:

```text
FSAPMA LSA / eligible CSA
    owns the provider capability problem
            |
            v
GOVERNED RESEARCH REQUEST
            |
            v
AUTHORIZED RESEARCH / SANDBOX PATH
            |
            v
EXTERNAL RESEARCH EVIDENCE
            |
            v
FSAPMA AWARENESS EVALUATION
```

The research mechanism is a supporting evidence path, not the owner of FSAPMA provider semantics.

---

# 8. Provider Capability-Gap Research Request

Conceptual request:

```text
ProviderCapabilityGapResearchRequest
{
    ResearchRequestId
    GapId
    OriginatingApplicationId
    OriginatingMSAId
    OriginatingLSAId
    OriginatingCSAId? 
    RequiredDataProduct
    ExactSemanticRequirements
    CostCeiling
    CurrentUseProfile
    Markets
    RequiredFreshness
    RequiredCoverage
    RequiredQuality
    CurrentKnownFailures[]
    SearchObjectives[]
    ForbiddenSubstitutions[]
    ResearchPriority
    EvidenceRequirements
    Expiry / RevalidationRequirement
}
```

For the current Owner-only evaluation phase, the request should explicitly carry the applicable use profile, including as relevant:

```text
OWNER_ONLY
PERSONAL
NON_COMMERCIAL
INTERNAL_USE
NO_EXTERNAL_DISPLAY
NO_REDISTRIBUTION
COST_CEILING_ZERO
```

A candidate must be evaluated for that exact use, not for an imaginary generic license.

---

# 9. What the Awareness Research May Search For

The research scope may include, within authority:

1. a completely new provider not currently in the thirteen-provider historical candidate set;
2. a free plan from a known provider that was not previously certified;
3. a newly introduced free endpoint or Data Product;
4. an official public source;
5. a broker/account entitlement that supplies the needed product without consuming protected execution capacity;
6. a legally permitted free exchange/feed path;
7. a zero-cost provider combination that collectively satisfies a composite requirement without corrupting semantics;
8. a new batch/stream/cache strategy that makes an already entitled free capability practical without bypassing quotas;
9. a semantically valid alternative data product if the original consumer explicitly allows that degradation;
10. evidence that no currently legitimate zero-cost solution exists.

Research may discover possibilities. It may not declare operational authority.

---

# 10. Exact Semantics Remain a Hard Gate During Research

Research is not allowed to solve a difficult requirement by renaming a weaker product.

For example:

```text
REQUIREMENT = CONSOLIDATED_NBBO + REALTIME
```

Candidates that provide only:

- IEX-only quotes;
- delayed consolidated quotes;
- derived reference price;
- single-venue top-of-book;
- EOD data;
- marketing language without documented semantics;

shall not be represented as satisfying the exact requirement.

The research result may say:

```text
NO EXACT FREE MATCH FOUND
```

or:

```text
FREE PARTIAL ALTERNATIVE FOUND
BUT REQUIRES CONSUMER-APPROVED DEGRADATION
```

It may not silently mutate the request.

---

# 11. Multi-Source Composition Rule

Research may investigate whether multiple zero-cost sources can lawfully and semantically compose a required Data Product.

But composition is permitted only if the canonical Data Product semantics define such composition as valid.

Mandatory invariant:

```text
MULTIPLE_WEAKER_FEEDS
DO_NOT AUTOMATICALLY SUM TO
ONE STRONGER FEED
```

Examples:

```text
IEX + ANOTHER SINGLE VENUE
!= AUTOMATIC CONSOLIDATED_NBBO
```

and:

```text
OPEN FROM PROVIDER_A
+ HIGH FROM PROVIDER_B
+ VOLUME FROM PROVIDER_C
!= ONE CANONICAL BAR
```

unless an explicitly governed product construction specification proves the resulting semantics are valid, attributable and reconstructable.

---

# 12. Candidate Research Evidence Package

A candidate provider or candidate product shall not be considered serious merely because a webpage says `free` or `realtime`.

The research package should collect, as applicable:

- provider identity;
- official product/plan identity;
- official documentation references;
- endpoint/product description;
- supported markets/instruments;
- exact fields;
- freshness/delay semantics;
- venue/consolidation semantics;
- historical depth;
- session coverage;
- transport options;
- API-call limits;
- weighted-credit model;
- concurrency limits;
- WebSocket limits;
- batch limits;
- quota reset model;
- account/key/tenant quota-domain semantics;
- pricing and free-tier conditions;
- trial versus durable free status;
- personal/non-commercial/internal-use terms;
- display/non-display terms;
- redistribution terms;
- storage/retention rights;
- derived-data restrictions;
- automated/trading-strategy restrictions;
- upstream source/venue lineage where knowable;
- provider health/reputation evidence where appropriate;
- correction/revision semantics;
- certification risks;
- expiry/revalidation date.

Unknown fields shall remain `UNKNOWN` rather than being inferred optimistically.

---

# 13. Candidate States

A discovered provider/product should move through explicit proposal states conceptually such as:

```text
DISCOVERED_RESEARCH_CANDIDATE
    -> EVIDENCE_INCOMPLETE
    -> EVIDENCE_READY
    -> DOMAIN_REVIEWED
    -> ONBOARDING_CANDIDATE
    -> CERTIFYING
    -> CERTIFIED_NOT_ACTIVE
    -> GOVERNED_ACTIVATION_ELIGIBLE
    -> ACTIVE
```

with failure/restriction states including:

```text
SEMANTICALLY_INCOMPATIBLE
RIGHTS_RESTRICTED
COST_INCOMPATIBLE
QUOTA_INSUFFICIENT
QUALITY_INSUFFICIENT
UNRELIABLE
DUPLICATE_NO_INCREMENTAL_VALUE
STALE_RESEARCH
REJECTED
```

The exact runtime state machine remains future design work.

This hardening fixes the semantic distinction, not implementation details.

---

# 14. Origin-Aware Review Path

A research candidate follows the Awareness chain matching its actual origin.

If an eligible CSA originates the provider-capability proposal:

```text
CSA
-> Parent LSA
-> FSAPMA MSA
-> FSA OS-GOVERNANCE / COMPATIBILITY REVIEW
-> SEPARATE OWNER / GOVERNANCE ADOPTION DECISION
```

If an LSA originates it directly:

```text
LSA
-> FSAPMA MSA
-> FSA OS-GOVERNANCE / COMPATIBILITY REVIEW
-> SEPARATE OWNER / GOVERNANCE ADOPTION DECISION
```

If the FSAPMA MSA originates it:

```text
FSAPMA MSA
-> FSA OS-GOVERNANCE / COMPATIBILITY REVIEW
-> SEPARATE OWNER / GOVERNANCE ADOPTION DECISION
```

Research success does not shorten this chain.

---

# 15. Activation Remains Separate From Research and Certification

The strongest invariant in this hardening is:

```text
FOUND_PROVIDER
!=
CAN_USE_PROVIDER

DOCUMENTED_PROVIDER
!=
CERTIFIED_PROVIDER

CERTIFIED_PROVIDER
!=
ACTIVE_PROVIDER
```

A newly researched candidate shall not:

- receive production credentials automatically;
- receive broker credentials automatically;
- create a new runtime route automatically;
- enter the canonical route selector automatically;
- purchase a paid plan automatically;
- accept terms automatically;
- change the cost ceiling automatically;
- claim redistribution/display rights automatically;
- bypass Owner/governance review where required.

Only a separately governed and authorized onboarding/certification/activation path may make it eligible for operational selection.

---

# 16. Research Priority

Not every missing optional field deserves immediate research effort.

The responsible Awareness should be able to prioritize capability gaps using evidence such as:

- recurrence frequency;
- number of affected instruments/markets;
- impact on protection or open-position safety;
- impact on opportunity quality;
- impact on coverage debt;
- impact on confidence;
- existence of a valid degraded substitute;
- current resource budget;
- current research capacity;
- expected benefit of finding a solution;
- probability that a free solution exists;
- staleness of previous research.

Protection-critical capability gaps outrank convenience/research gaps.

Research priority shall not override explicit authority or Foundation resource grants.

---

# 17. Research-Deduplication and Storm Prevention

A repeated route failure across thousands of instruments must not create thousands of duplicate Internet research jobs.

The gap shall be deduplicated by semantic `GapFingerprint`.

Conceptual controls:

```text
SAME GAP FINGERPRINT
-> ONE ACTIVE RESEARCH CASE
-> MANY AFFECTED REQUEST REFERENCES
```

The design should support:

- cooldowns;
- re-research intervals;
- evidence staleness triggers;
- explicit material-change triggers;
- research resource ceilings;
- cancellation when the gap is resolved;
- merging duplicate findings;
- bounded retries.

This preserves system resources and prevents Awareness from becoming a research storm generator.

---

# 18. Current Thirteen-Provider Portfolio Is Not a Ceiling

The historical V1.3 thirteen-provider set remains the current core candidate portfolio because it contains useful complementary roles.

It is not a permanent closed universe.

```text
13_PROVIDER_CORE_CANDIDATE_PORTFOLIO
!=
MAXIMUM_PROVIDER_COUNT
```

Awareness-driven discovery may propose:

```text
PROVIDER_14
PROVIDER_15
...
```

when evidence shows that a new provider materially improves required capability, free capacity, semantic coverage, quality, resilience, independence or cost efficiency.

The provider registry is therefore extensible by governed proposal, not fixed by historical vendor count.

---

# 19. Provider Removal / Replacement Can Also Be Awareness-Driven

The same mechanism works in the opposite direction.

Awareness may discover that a current candidate provider:

- removed its free plan;
- stopped providing realtime data;
- materially changed quota;
- changed upstream lineage;
- became unreliable;
- lost a needed market;
- changed terms so Falcon's use is no longer permitted;
- became redundant with no incremental value;
- became inferior to a better zero-cost alternative.

It may then propose restriction, replacement, downgrade or removal through the governed lifecycle.

Provider membership is evidence-driven, not sentimental.

---

# 20. No-Free-Solution Outcome

Research may legitimately conclude that no exact zero-cost solution can currently be established.

That outcome must be preserved honestly.

```text
RESEARCH_COMPLETE
+ NO LEGITIMATE EXACT ZERO-COST MATCH
=
CAPABILITY GAP REMAINS OPEN
```

Falcon may then, as separately governed options:

- continue operating with the declared unavailable state;
- use an explicitly permitted degraded product;
- reduce confidence or affected scope;
- avoid strategies that require the missing semantic;
- schedule future re-research;
- present an evidence-backed recommendation to the Project Owner concerning a paid capability or changed cost ceiling.

No Awareness entity may purchase service or alter the Owner's `CostCeiling = 0` policy by itself.

---

# 21. Example — Consolidated NBBO Realtime at Zero Cost

Initial request:

```text
DataProduct = TOP_OF_BOOK_QUOTE
Market = US_EQUITIES
Consolidation = CONSOLIDATED_NBBO
TemporalRequirement = REALTIME_REQUIRED
CostCeiling = 0
UseProfile = OWNER_ONLY_NONCOMMERCIAL_INTERNAL
```

Current routing result:

```text
Alpaca IEX
-> REJECT: VENUE_SCOPE_TOO_NARROW

Delayed consolidated source
-> REJECT: FRESHNESS_INSUFFICIENT

Derived reference price source
-> REJECT: SEMANTICS_NOT_NBBO

CurrentExactEligibleRoutes = 0
```

Immediate operational result:

```text
UNAVAILABLE_AT_CURRENT_COST_CEILING
```

Awareness consequence:

```text
P-LSA-04 detects exact route gap
        |
        v
ProviderCapabilityGap created
        |
        v
Responsible FSAPMA Awareness evaluates research value
        |
        v
Governed research path searches for:
- another provider with genuine free consolidated NBBO realtime
- an eligible broker/account entitlement with safe spare capacity
- an official/free source
- a legally and semantically valid zero-cost composition
        |
        v
Candidate evidence package
        |
        v
P-LSA-02 semantics challenge
P-LSA-03 capability/entitlement challenge
P-LSA-05 quality verification
P-LSA-06 cost/quota/reliability challenge
P-LSA-01 onboarding candidate
        |
        v
Origin-aware Awareness review
        |
        v
Separate governed certification/activation decision
```

If no candidate passes, the gap remains unresolved rather than being papered over.

---

# 22. Separation From Operational Provider Data

Provider research and provider operational data acquisition remain different flows.

```text
RESEARCH FLOW
Internet / documentation / discovery evidence
-> governed research boundary
-> provenance / quarantine / validation
-> Awareness evaluation
-> proposal

OPERATIONAL FLOW
certified provider endpoint
-> FSAPMA operational provider acquisition
-> canonical Data Product
-> FMOF / Trading consumer
```

Research content shall never be injected directly into operational Trading truth merely because it was found online.

---

# 23. Red-Team Requirements Introduced by This Hardening

Future fresh Red-Team review of this proposal package shall challenge at least:

1. provider marketing says `realtime` but feed is delayed;
2. provider says `free` but product is trial-only;
3. hidden credit-card or automatic paid conversion;
4. LSA/CSA attempts to activate its own discovered provider;
5. candidate terms prohibit automated/non-display strategy use;
6. candidate rights are valid only for display;
7. multiple free feeds are falsely composed into consolidated NBBO;
8. same-upstream sources are presented as independent confirmation;
9. API keys are multiplied to evade one provider quota domain;
10. research job storm caused by repeated route failures;
11. stale research evidence survives a provider plan change;
12. malicious/fake provider documentation contaminates certification;
13. consumer requirement is silently weakened to make a candidate pass;
14. broker spare market-data use steals execution/account capacity;
15. newly discovered provider bypasses P-LSA-01 onboarding;
16. `CostCeiling = 0` is silently changed by Awareness;
17. research path is mistaken for operational-data path;
18. external research input is treated as authoritative market truth;
19. CSA crosses outside its declared component responsibility;
20. LSA bypasses FSAPMA MSA / FSA / Owner review;
21. no exact free solution exists but Falcon fabricates one;
22. an obsolete free provider remains active after terms/capability change.

---

# 24. Relationship to Current FCR State

This hardening relies on no currently implemented research Internet capability.

Current relevant future Foundation dependencies remain separately governed:

- `FCR-0008` — research-only Internet egress for Application Awareness, including MSA/LSA/eligible CSA use;
- `FCR-0011` — FSTSimA non-Live isolation and egress guard.

Their current planning status does not create runtime research authority.

This proposal therefore defines the Application business/research semantics now while remaining fail closed until the required Foundation capability and later authority exist.

---

# 25. Controlling Interpretation of `03`

The statement in `03_V1_3_PROVIDER_ARCHITECTURE_RECONCILIATION_AND_FREE_FIRST_CAPABILITY_ROUTING_PROPOSAL.md` that FSAPMA returns an explicit unavailable/degraded result when no zero-cost route satisfies the exact request remains correct for the **immediate operational request**.

This hardening adds the missing second consequence:

```text
NO CURRENT ZERO-COST ROUTE
=
IMMEDIATE HONEST UNAVAILABLE/DEGRADED OPERATIONAL RESULT
+
OPTIONAL / PRIORITIZED AWARENESS-DRIVEN CAPABILITY-GAP RESEARCH LIFECYCLE
```

The two consequences are complementary, not contradictory.

---

# 26. Final Invariants

```text
REQUEST DEFINES THE NEED
PROVIDER DOES NOT DEFINE THE NEED
```

```text
NO CURRENT PROVIDER
!=
NO POSSIBLE PROVIDER
```

```text
UNAVAILABLE NOW
!=
STOP LEARNING
```

```text
AWARENESS MAY RESEARCH A BETTER SOLUTION
AWARENESS MAY NOT SELF-ACTIVATE THAT SOLUTION
```

```text
FREE-FIRST
!=
SEMANTICS-LAST
```

```text
FREE
!=
AUTHORIZED
```

```text
DISCOVERED
!=
CERTIFIED
!=
ACTIVE
```

```text
RESEARCH INPUT
!=
OPERATIONAL MARKET TRUTH
```

```text
LSA / CSA RESEARCH RESPONSIBILITY
!=
UNRESTRICTED INTERNET AUTHORITY
```

---

# 27. Required Future Review Before Acceptance

This is a new semantic hardening outside the current accepted R7 freeze.

Required lifecycle remains:

```text
EXACT CANDIDATE PACKAGE
-> FRESH ARCHITECTURE / CONSISTENCY REVIEW
-> FRESH RED-TEAM REVIEW
-> REMEDIATE AND REPEAT IF SEMANTICS CHANGE
-> PROJECT OWNER REVIEW
-> EXPLICIT OWNER ACCEPTANCE
```

No earlier review PASS shall be represented as current review evidence for this newly added semantic scope.

---

# 28. Non-Grant

This record does not:

- alter the accepted R7 freeze;
- authorize implementation;
- authorize runtime activation;
- authorize direct Internet access by an LSA, CSA or MSA;
- claim `FCR-0008` or `FCR-0011` is implemented;
- authorize provider discovery traffic in trusted runtime;
- authorize provider or broker connectivity;
- authorize credential creation/provisioning;
- authorize acceptance of provider terms;
- authorize a new provider route;
- authorize paid service;
- change the zero-cost policy;
- make every FSAPMA component CSA-eligible;
- change Provider Controller into a CSA;
- authorize Paper, Tiny Live, Live or deployment;
- constitute overall Owner acceptance of FMOF.

It records the Project Owner-directed candidate semantics for Awareness-driven resolution of provider capability gaps.