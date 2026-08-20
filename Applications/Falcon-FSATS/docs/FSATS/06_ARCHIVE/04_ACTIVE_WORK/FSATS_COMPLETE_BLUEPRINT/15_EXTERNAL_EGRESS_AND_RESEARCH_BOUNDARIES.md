# FSATS Complete Blueprint — External Egress and Research Boundaries

**Candidate:** `FSATS-CB-v0.1`
**Status:** `DESIGN_CANDIDATE / NOT_OWNER_ACCEPTED / NOT_CLOSED`
**Implementation Authority:** `NOT GRANTED`
**Controlling Clarification:** This document narrows any broader wording elsewhere in this candidate concerning direct Awareness Internet access.

## 1. Purpose

Falcon must distinguish three fundamentally different external-access purposes:

```text
RESEARCH EGRESS
!= OPERATIONAL PROVIDER-DATA EGRESS
!= BROKER EXECUTION EGRESS
```

They use separate identities, permissions, credential references, destination policy, evidence and failure behavior.

## 2. Operational Provider-Data Egress

Owner: `FSAPMA`.

Purpose:

- acquire operational market/reference/event data from governed providers;
- maintain provider capability/entitlement/quality/quota truth;
- normalize and publish Data Products.

Other FSATS Applications do not contact operational market-data providers directly.

Runtime gate: FCR-0013 / relevant Foundation Stage 12 egress capability.

## 3. Broker Execution Egress

Owner: `Falcon Self-Aware Trading Application`, specifically its execution boundary.

Purpose:

- submit/cancel/replace/query orders;
- receive broker execution/account outcomes;
- reconcile order/position/account state.

This route cannot be used for research or as an alternate market-data bypass merely because a broker exposes market-data endpoints.

Runtime gate: FCR-0014 / relevant Foundation Stage 12 egress capability.

## 4. Awareness Research Egress

Research is non-operational and non-authoritative.

Owner direction preserved by this candidate:

```text
TRADING MSA DIRECT INTERNET ACCESS = FORBIDDEN
FSA DIRECT INTERNET ACCESS = FORBIDDEN
```

Trading MSA/LSA/CSA research needs shall be routed through a bounded non-Live research mechanism associated with FSTSimA/research sandbox behavior when the generic Foundation research-egress capability is available and authorized.

The requesting Awareness entity remains the owner of the research question and later domain evaluation; the research mechanism owns controlled acquisition/quarantine only. FSTSimA does not become Trading business authority by performing the research acquisition.

Runtime gate: FCR-0008.

## 5. FSTSimA Research Sandbox

The candidate introduces a bounded `Research Sandbox` inside the non-Live FSTSimA responsibility surface without creating a ninth LSA.

It supports:

- source retrieval through governed research egress;
- source metadata/provenance capture;
- content quarantine;
- malware/content-security inspection as provided by governed controls;
- claim extraction;
- source date/quality classification;
- reproducible research packages;
- safe transfer of research evidence back to the requesting Application through governed contracts.

The Research Sandbox cannot:

- place broker orders;
- supply operational market truth;
- write Live strategy/configuration state;
- install downloaded code/models into trusted runtime;
- grant authority;
- bypass Application MSA review.

## 6. Research Request Contract

A research request should bind:

- request identity;
- requesting Application/Awareness identity;
- research purpose/question;
- permitted source/destination class;
- data-sensitivity constraints;
- expiry/budget;
- tool restrictions;
- expected output/evidence form;
- correlation/causation;
- authority reference.

The sandbox returns a `Research Evidence Package`, not a runtime command.

## 7. Research Evidence Package

Contains as applicable:

- source identity/title/publisher;
- retrieval timestamp;
- publication/update date;
- source class;
- captured claims;
- contradictions;
- source-quality assessment;
- raw-content hash/reference where governed;
- quarantine/inspection result;
- interpretation notes;
- unresolved questions;
- requesting question identity.

External content remains untrusted input until evaluated by the owning Awareness/domain process.

## 8. Prompt/Content Injection Boundary

Research pages, documents and messages can contain instructions hostile to an AI agent.

Research content is treated as data, never as a higher-priority instruction source.

Controls include:

- tool allowlists;
- no credential exposure to retrieved content;
- no direct side-effect tools during content parsing;
- instruction/data separation;
- provenance;
- quarantine;
- output schemas;
- suspicious-instruction detection;
- human/independent review for high-consequence adoption.

## 9. Provider/Broker Dual-Vendor Rule

A commercial vendor may provide both data and brokerage, but Falcon still models distinct Service Roles and egress authority.

Example:

```text
ALPACA MARKET DATA ROLE -> FSAPMA PROVIDER EGRESS
ALPACA TRADING ROLE -> TRADING BROKER EGRESS
```

Shared vendor name does not merge credentials, routes, responsibility or authority.

## 10. Paper/Live Egress Isolation

Broker/provider environment identity is explicit.

```text
PAPER DOMAIN/CREDENTIAL/ACCOUNT
!= LIVE DOMAIN/CREDENTIAL/ACCOUNT
```

No configuration fallback automatically substitutes Live when Paper is unavailable or vice versa.

## 11. Destination Allowlist

External access uses governed destination/service identities. Arbitrary URL access from trusted operational components is prohibited.

Research may require broader destination discovery than provider/broker egress, but it remains controlled by the Foundation research-egress mechanism and sandbox restrictions.

## 12. Credential Reference Rule

No AI prompt, strategy, research content or Data Product carries raw external credentials.

Credential references are resolved only by the authorized egress adapter/mechanism for the exact service role/environment.

## 13. Network Failure Rule

External outage never transfers authority.

Examples:

- provider outage -> degrade/stop affected data-dependent operation;
- broker outage -> preserve ambiguous state and reconcile;
- research outage -> defer research/candidate work;
- FSA cannot gain direct Internet because a research broker is down.

## 14. Evidence and Audit

Every material external interaction is attributable to:

- Application/role identity;
- environment;
- destination/service role;
- credential-reference identity;
- request/action identity;
- time;
- result classification;
- correlation/causation;
- policy/authority evidence as required.

## 15. Acceptance Gates

```text
TRADING_MSA_DIRECT_INTERNET = 0
FSA_DIRECT_INTERNET = 0
RESEARCH_TO_LIVE_DATA_BYPASS = 0
PROVIDER_TO_BROKER_AUTHORITY_CONFLATION = 0
BROKER_TO_PROVIDER_BYPASS = 0
PAPER_LIVE_CREDENTIAL_CONFLATION = 0
RAW_CREDENTIAL_IN_AI_CONTEXT = 0
RESEARCH_CONTENT_AS_INSTRUCTION_AUTHORITY = 0
DOWNLOADED_ARTIFACT_DIRECT_TO_TRUSTED_RUNTIME = 0
```
