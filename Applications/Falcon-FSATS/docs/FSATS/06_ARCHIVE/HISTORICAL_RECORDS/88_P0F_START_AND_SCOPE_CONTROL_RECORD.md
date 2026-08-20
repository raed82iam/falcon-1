# FSATS V1.4 Part 0 / P0-F — Start and Scope Control Record

**Status:** `P0F_AUTHORIZED_AND_STARTED`
**Date:** `2026-08-08`
**Branch:** `application-development`
**Start baseline:** `a081178904fad309f4d526bbde58e0f25254bac4`
**Authority:** explicit Project Owner instruction to begin the next Part 0 work package after P0-E Owner acceptance and closure
**P0-F final Owner acceptance:** `NOT_GRANTED`

## 1. Authorized scope

P0-F is authorized for design/review only:

**Cross-Application Contracts and Information Flow**

P0-F SHALL convert every required cross-Application relationship in the current P0-E scope into explicit bounded contract families and information-flow semantics.

P0-F SHALL NOT implement runtime routes, Foundation capabilities, schemas in code, deployment, external connectivity, Paper, Tiny Live, Live or production adoption.

## 2. Locked predecessor chain

P0-F SHALL preserve and build on:

- P0-A `OWNER_ACCEPTED_AND_CLOSED`;
- P0-B `OWNER_ACCEPTED_AND_CLOSED`;
- P0-C `OWNER_ACCEPTED_AND_CLOSED`;
- P0-D `OWNER_ACCEPTED_AND_CLOSED`;
- P0-E `OWNER_ACCEPTED_AND_CLOSED`.

P0-F SHALL NOT silently change accepted Application ownership, Awareness jurisdiction, Foundation/Application boundaries, canonical identities, Trading-container membership, Shared-Application placement, or APP-001/CON-023 obligations.

Any required semantic change to a closed predecessor must be surfaced as a predecessor-impact finding and cannot be hidden inside a P0-F contract.

## 3. Current architecture subjects

Trading main-business container architecture identity:

- `falcon.container.trading`

Independent Applications inside that container:

- `falcon.app.trading.guardian`
- `falcon.app.trading.fsapma`
- `falcon.app.trading.core`
- `falcon.app.validation.fstsima`

Independent Shared Applications outside that container:

- `falcon.app.shared.communication`
- `falcon.app.shared.web`

The container is not a communication principal. Only independently governed Applications/Foundation authorities may be contract participants.

## 4. Foundation state consumed by P0-F

Fresh Foundation review at P0-F start confirms:

- Stage 5 WP-03 through WP-07 are `ACCEPTED / CLOSED`;
- WP-03 supplies Application communication declaration/validation semantics;
- WP-04 supplies bounded FIL validation/message-admission semantics;
- WP-05 supplies governed route declaration/eligibility/selection semantics;
- WP-06 supplies bounded delivery, retry/idempotency, ordering/pressure and delivery-evidence semantics;
- WP-07 supplies Event System / truthful publication / replay-test-simulation classification and event evidence semantics;
- WP-08 is separately authorized/in progress for cryptographic message protection;
- runtime activation, external connectivity and Application-specific Foundation business behavior remain unauthorized.

P0-F SHALL distinguish accepted Foundation semantic capability from runtime deployment/activation authority.

## 5. Open FCR state at P0-F start

Open Application-originated FCRs relevant to P0-F include:

- FCR-0004 Guardian protection command route — `ACCEPTED_FOR_PLANNING / PARTIAL`;
- FCR-0005 operational market-data delivery — `ACCEPTED_FOR_PLANNING / PARTIAL`;
- FCR-0006 event/evidence/replay delivery — `ACCEPTED_FOR_PLANNING / PARTIAL`, with Foundation communication/event portions now technically satisfied but Application verification still pending;
- FCR-0007 resource escalation request — `ACCEPTED_FOR_PLANNING / PARTIAL`;
- FCR-0008 research-only awareness Internet egress — `ACCEPTED_FOR_PLANNING / PARTIAL`;
- FCR-0009 latency/deadline/QoS transport — `ACCEPTED_FOR_PLANNING / MISSING` historically, with partial Foundation WP-06 semantics now available but overall FCR open;
- FCR-0010 resource pressure/load-shedding signals — `ACCEPTED_FOR_PLANNING / PARTIAL`;
- FCR-0011 FSTSimA non-Live isolation/egress guard — `ACCEPTED_FOR_PLANNING / PARTIAL`;
- FCR-0012 FSA Owner-governance/evolution control plane — `SUBMITTED / MISSING`.

No FCR creates runtime authority.

## 6. Mandatory P0-F contract fields

Every accepted cross-Application contract family SHALL define at minimum:

- canonical P0-F contract-family identity;
- producer/requester Application;
- consumer/responder Application or Foundation authority where applicable;
- exact purpose and business semantic owner;
- message/data-product/event/command/evidence class;
- authority source/classification;
- schema/version rule;
- correlation, causation and provenance requirements;
- idempotency, retry, ordering, correction/supersession rules where applicable;
- freshness/deadline/expiry semantics where applicable;
- security/data classification requirements;
- acknowledgement/outcome semantics where applicable;
- failure/degraded/fail-closed behavior;
- Foundation capability dependency and current status;
- FCR dependency where applicable;
- explicit statement that the contract declaration creates no runtime route or permission.

## 7. Mandatory negative rules

P0-F SHALL reject:

- direct private-memory/database/file access across Applications;
- container-level communication principals;
- MSA/LSA/CSA rank as a cross-Application authority path;
- shared mutable cross-Application business state;
- Web UI action as business authority by itself;
- Communication delivery as mutation of source business truth;
- FSTSimA replay/simulation evidence as Live authority;
- research Internet content as operational trading data;
- provider truth and broker execution truth conflation;
- Foundation transport semantics being treated as Application business success;
- local route/permission identifiers being treated as Foundation authority;
- contract existence being treated as admission, route creation, activation, deployment, Paper or Live authority.

## 8. Review lifecycle

P0-F SHALL follow:

`DESIGN -> ARCHITECTURE/CONSISTENCY REVIEW -> RED-TEAM -> FINDINGS -> REMEDIATION -> FRESH FULL RE-REVIEW -> OWNER REVIEW CANDIDATE`.

A post-change Red-Team is mandatory for every semantic remediation.

## 9. Current authority state

`P0F = AUTHORIZED / IN PROGRESS`

`P0G_THROUGH_P0L = NOT_STARTED`

`IMPLEMENTATION = NOT_AUTHORIZED`

`RUNTIME / PAPER / TINY_LIVE / LIVE / DEPLOYMENT / PRODUCTION = NOT_GRANTED`
