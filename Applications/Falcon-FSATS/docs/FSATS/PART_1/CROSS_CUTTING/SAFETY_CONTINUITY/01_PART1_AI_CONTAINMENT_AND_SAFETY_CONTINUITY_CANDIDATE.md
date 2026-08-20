# FSATS Part 1 — AI Containment and Safety Continuity Candidate

**Status:** `OWNER-DIRECTED CANDIDATE / NOT_OWNER_ACCEPTED / NOT_CLOSED`  
**Branch:** `application-development`  
**Scope:** `PART_1 CROSS-CUTTING / APPLICATION-SIDE SAFETY CONTINUITY`  
**Implementation Authority:** `NOT GRANTED`  
**Runtime Authority:** `NOT GRANTED`

## 1. Purpose

This record materializes the Project Owner's 2026-08-14 direction that Falcon FSATS shall not use an all-or-nothing `AI_ON / APPLICATION_DOWN` safety model.

It defines Application-side requirements for:

- minimum-necessary containment of faulty or untrusted intelligent scope;
- safe expansion of containment when the trust blast radius is unknown;
- continued AI-independent protection of existing obligations and financial exposure;
- deterministic degraded operation after AI restriction or Kill;
- prevention of orphaned positions, orders, protection duties, resource obligations or other live responsibilities;
- controlled recovery and revival without silent trust restoration.

This record does not define Foundation internals or Shared Web internals. Generic Foundation/FSA continuity is handed to Foundation through `FCR-0082`. Shared Web continuity is handed directly to Web through `FCR-0083`. Exact external communication contracts remain tracked by `FCR-0080`.

## 2. Governing Safety Invariants

The following are mandatory candidate invariants:

```text
MINIMUM NECESSARY CONTAINMENT

UNKNOWN TRUST BLAST RADIUS
-> EXPAND CONTAINMENT UNTIL A TRUSTWORTHY BOUNDARY IS PROVEN

AI KILL != APPLICATION KILL
AI KILL != AUTOMATIC FSATS SHUTDOWN
AI FAILURE/KILL MUST NOT ORPHAN EXISTING EXPOSURE

NO OPEN POSITION,
NO PENDING ORDER,
NO PARTIAL FILL,
NO PROTECTION OBLIGATION,
NO CAPITAL OBLIGATION,
NO LIVE RESOURCE/PROVIDER OBLIGATION
MAY BECOME OWNERLESS OR UNMONITORED
SOLELY BECAUSE AN AI ENTITY WAS KILLED.
```

A function whose safe operation depends on killed/untrusted intelligence and has no independent trusted fallback SHALL fail closed.

A function that can continue through a separately governed, deterministic, attributable and independently trusted safety path MAY continue in degraded mode.

## 3. Containment Is Trust-Scoped, Not Merely Process-Scoped

Falcon SHALL contain the smallest scope that can be proven sufficient to remove the affected trust risk.

Potential containment targets include, where the actual architecture supports them:

```text
INTELLIGENT COMPONENT
CSA
LSA
MSA
APPLICATION AI SET
MULTI-APPLICATION / FSATS AI SET
```

These are containment scopes, not automatic escalation steps.

A lower scope is permitted only when evidence proves the fault and its material effects are contained there.

If shared state, authority state, common model/memory, evidence integrity, dependency integrity, or other trust-bearing material may have been affected outside the initially identified scope, Falcon SHALL NOT assume local damage merely because the first visible symptom was local.

```text
PROVEN LOCAL FAULT
-> LOCAL CONTAINMENT MAY BE SUFFICIENT

UNKNOWN / PROPAGATED TRUST DAMAGE
-> CONTAINMENT SCOPE MUST EXPAND
```

Containment expansion remains bounded by evidence and shall not become arbitrary shutdown of unaffected trusted scope.

## 4. Kill Semantics and Separation From Application Lifecycle

Application-side `AI KILL` means removal of operational trust from the affected intelligent scope and cessation/isolation of its affected operation according to the authoritative enforcement path.

It does not by itself mean:

- suspension of the whole Application;
- isolation of the whole Application;
- termination of all FSATS Applications;
- liquidation of all positions;
- cancellation of every protective order;
- destruction of evidence;
- automatic authority transfer to a sibling AI/component;
- automatic trust restoration after restart.

Application lifecycle suspension/isolation/removal and Falcon-wide emergency shutdown remain distinct higher-order actions owned by their applicable authorities.

## 5. Safety Continuity Mode

When intelligent capability is unavailable, killed, isolated or materially untrusted, the affected Application SHALL enter an explicit degraded state when safe continuation is possible.

Candidate semantic name:

```text
SAFETY_CONTINUITY_MODE
```

In this mode:

```text
NEW INTELLIGENT RISK      = DENIED UNLESS A SEPARATELY TRUSTED AUTHORIZED PATH EXISTS
RISK EXPANSION            = DENIED
AI SELF-DEVELOPMENT       = DENIED FOR AFFECTED/UNTRUSTED SCOPE
AFFECTED AI OUTPUT        = DENIED
EXISTING OBLIGATIONS      = ACTIVELY MONITORED/PROTECTED
RECONCILIATION            = CONTINUES WHERE TRUSTED
AUDIT/EVIDENCE            = CONTINUES WHERE TRUSTED
OWNER/GUARDIAN CONTROLS   = CONTINUE WHERE INDEPENDENT AND TRUSTED
DETERMINISTIC PROTECTION  = CONTINUES WHERE INDEPENDENT AND TRUSTED
```

Safety Continuity is not normal business mode and is not a substitute intelligent decision-maker.

## 6. Risk-Monotonic Degraded Authority

Where a degraded action is allowed without the killed/untrusted intelligence, it SHALL be risk-monotonic unless a separately governed emergency rule proves otherwise.

Candidate invariant:

```text
DEGRADED ACTION MAY PRESERVE OR REDUCE RISK.
DEGRADED ACTION SHALL NOT SILENTLY INCREASE RISK.
```

Examples:

```text
POSITION SIZE     -> MAY DECREASE / SHALL NOT INCREASE
MAX LOSS          -> MAY TIGHTEN / SHALL NOT WIDEN
RISK LIMIT        -> MAY REDUCE / SHALL NOT EXPAND
NEW POSITION      -> NORMALLY DENIED
NEW LEVERAGE      -> DENIED
PROTECTIVE ACTION -> MAY RESTORE THE LAST VALID TRUSTED PROTECTION ENVELOPE
```

This rule prevents the safety fallback from becoming an ungoverned replacement intelligence.

## 7. Mandatory Position Safety Envelope

Before a Trading position/exposure is allowed to become Live in any future authorized execution stage, Trading SHALL have enough governed information to answer:

> If the relevant intelligence becomes unavailable immediately after execution, how will this exposure remain monitored, bounded and recoverable?

The future exact schema is P1-D/P1-K material, but the semantic envelope SHALL include as applicable:

- position/exposure identity;
- account/portfolio/instrument identity;
- quantity/notional;
- current protection owner;
- maximum authorized loss or equivalent risk boundary;
- current protective-order state;
- last trusted Risk decision/version;
- last trusted protection epoch/version;
- permitted operations without AI;
- emergency reduction/exit condition;
- reconciliation state;
- broker capability constraints;
- market capability constraints;
- data/truth freshness requirements;
- recovery path and evidence identity.

If Falcon cannot establish an adequate AI-independent protection/continuity plan for a proposed exposure:

```text
NEW EXPOSURE = DENIED
```

## 8. Existing Exposure After AI Kill

AI Kill SHALL NOT automatically liquidate all existing positions.

Existing exposure SHALL be classified using trusted non-killed controls and authoritative state into at least equivalent semantic classes:

```text
PROTECTED_AND_VERIFIED
PROTECTION_REPAIRABLE
STATE_OR_PROTECTION_UNKNOWN
SAFE_EXIT_REQUIRED
```

### 8.1 Protected and Verified

If the position is reconciled, its protection is valid, required state is trustworthy and continued holding remains inside the pre-existing safety envelope, it MAY remain open under Safety Continuity Mode.

### 8.2 Protection Repairable

If exposure truth is known but a protection element is missing/invalid, an independently authorized deterministic safety path MAY restore the last valid trusted protection or reduce risk according to the governed envelope.

It SHALL NOT ask killed/untrusted intelligence to invent a new risk policy.

### 8.3 State or Protection Unknown

Unknown broker/order/fill/position truth SHALL freeze new risk and enter reconciliation.

```text
UNKNOWN EXPOSURE TRUTH
-> NO BLIND RETRY
-> FREEZE NEW RISK
-> RECONCILE
-> ESTABLISH AUTHORITATIVE TRUTH
-> PROTECT / REDUCE / EXIT AS GOVERNED
```

### 8.4 Safe Exit Required

If a live exposure cannot remain adequately monitored/protected without the killed capability, or if the trusted safety envelope requires exit, a separately authorized deterministic safe-exit path SHALL reduce/close the exposure according to current authoritative market/broker truth.

`CLOSE_REQUEST != ZERO_EXPOSURE`; reconciliation remains mandatory.

## 9. Broker/Market-Native Protection

Where supported and governed, FSATS SHOULD use broker/exchange-native protective capability so basic position protection does not depend solely on an in-memory AI process remaining alive.

However:

```text
BROKER_PROTECTION != COMPLETE FALCON SAFETY
```

FSATS SHALL still reconcile broker truth, partial fills, cancel/replace races, stale orders, duplicate outcomes and unsupported capability differences.

Broker/market capability is profile-specific. FSATS SHALL NOT assume every broker/market supports bracket/OCO/stop semantics identically.

Unknown capability remains unsupported until proven:

```text
UNKNOWN != SUPPORTED
```

## 10. Trading Responsibilities

Trading shall preserve separation between intelligent business judgment and deterministic continuity protection.

After affected Trading intelligence is killed/untrusted, the following categories SHALL stop or fail closed unless a separate trusted authorized path exists:

- new intelligent opportunity discovery;
- new AI-generated entry decisions;
- AI strategy selection/orchestration affected by the Kill;
- adaptive risk expansion;
- AI-driven portfolio expansion;
- affected learning/evolution/self-development;
- any decision dependent on invalidated evidence from the killed scope.

The following categories SHALL remain eligible to continue when independently trusted and authorized:

- position inventory;
- order/fill/position reconciliation;
- hard exposure/loss limits;
- preservation/repair of valid protective controls;
- deterministic risk reduction;
- attributable audit/evidence;
- Owner/Guardian restriction consumption;
- safe exit when required;
- truthful degraded-state reporting.

P1-F SHALL materialize the exact 13-LSA impact without changing LSA ownership.

## 11. Trading Guardian Responsibilities

The Trading Guardian shall preserve a strict distinction between intelligent protection analysis and non-AI deterministic safety enforcement.

Candidate invariant:

```text
GUARDIAN INTELLIGENCE
!=
GUARDIAN DETERMINISTIC SAFETY KERNEL / HARD PROTECTION PATH
```

The exact implementation name/topology is P1-H material and is not fixed by this record.

The required outcome is that killing Guardian AI shall not automatically remove independently trusted hard protections needed to prevent unacceptable risk.

A deterministic Guardian safety path may own only bounded protective behavior such as deny/freeze/restrict/reconcile/reduce/exit according to pre-governed authority. It SHALL NOT become a new strategy engine, profit optimizer, autonomous policy writer or replacement MSA.

If Guardian's own trust blast radius includes the deterministic protection path, that path cannot be presumed trusted merely because it is non-AI; containment and Foundation lifecycle/security controls apply normally.

## 12. FSAPMA Responsibilities

If provider-selection or other FSAPMA intelligence is killed/untrusted, FSAPMA MAY continue only with independently trusted deterministic provider/data controls already inside approved authority.

Eligible degraded behavior may include:

- preserve a currently valid provider binding where still trustworthy;
- hard quota enforcement;
- schema/data-quality validation;
- freshness enforcement;
- deterministic failover only where pre-governed;
- truthful unavailable/unknown status.

If trustworthy operational data cannot be established:

```text
AFFECTED TRADING FUNCTION = FAIL CLOSED
```

FSAPMA shall not fabricate or infer data truth from unavailable intelligence.

## 13. FSTSimA Responsibilities

Killed/untrusted FSTSimA intelligence SHALL NOT contaminate trusted production decision paths.

Affected experiments/research/validation outputs are unavailable until their trust is restored or independently revalidated.

Existing production authority SHALL NOT be expanded merely because FSTSimA is unavailable.

Safe isolation, evidence preservation and later controlled recovery remain required.

## 14. APP-RSC Responsibilities

If APP-RSC intelligent optimization/awareness is killed or materially untrusted:

- no new intelligent cross-Application redistribution may occur through the affected scope;
- no sibling Application inherits resource-coordination authority;
- Foundation-authoritative grants/ceilings/floors/truth remain unchanged;
- last valid Foundation envelope and protected minima may be consumed only as allowed by existing valid authority;
- deterministic protective throttling/degradation may continue only if independently trusted and pre-governed;
- stale/unknown/revoked Foundation state fails closed;
- resource truth/evidence remains attributable and reconstructable.

This record does not change FCR-0031 or Foundation resource ownership.

## 15. Monitor AI Failure

Monitor AI is oversight, not the safety kernel and not business authority.

Failure/Kill of one Monitor AI SHALL NOT automatically Kill its target or the whole Application.

However, loss of required monitoring coverage SHALL become an attributable degraded/integrity condition. The affected target's allowable authority may be reduced according to the final governed monitoring policy.

Remaining monitor output SHALL NOT be treated as proof that lost independent coverage is unnecessary.

## 16. No Authority Inheritance

After any containment/Kill:

```text
MISSING AUTHORITY != AVAILABLE AUTHORITY FOR SOMEONE ELSE
```

No sibling AI, LSA, MSA, operational controller, Guardian component, Web surface or Application may assume the killed scope's authority merely because the original holder is unavailable.

Any substitute path must already possess separately governed authority or be explicitly authorized through the correct process.

## 17. Owner-Facing Kill Scope and Evidence

Every future Kill/containment action SHALL be attributable enough to identify as applicable:

- Kill/containment identity;
- target identity/scope;
- trigger/evidence identity;
- affected identities;
- affected authorities/permissions;
- preserved functions;
- stopped/degraded functions;
- open obligations/exposure state;
- current safety owner for each retained obligation;
- trust-blast-radius assessment;
- reconciliation requirements;
- recovery/remediation path;
- Controlled Revival state.

Exact contract/schema/UI fields remain P1-D/P1-K/Web-owned material.

## 18. Recovery and Controlled Revival

Restart, process recreation, model reload or technical health recovery SHALL NOT by itself restore operational trust after Kill or material integrity containment.

Required conceptual sequence:

```text
CONTAIN / KILL
-> PRESERVE EVIDENCE
-> ESTABLISH ROOT CAUSE / TRUST BOUNDARY
-> REMEDIATE / ROLLBACK / REPLACE AS GOVERNED
-> STATIC + BEHAVIORAL REVALIDATION AS APPLICABLE
-> AUTHORITY / PERMISSION REVALIDATION
-> DEPENDENCY / STATE RECONCILIATION
-> CONTROLLED REVIVAL
```

No incident state or historical evidence is erased merely because the component returns to service.

## 19. Foundation and Shared Web Boundaries

Foundation owns generic OS lifecycle/security/trust containment, FSA internals and Falcon-wide platform continuity semantics.

This Application record does not prescribe Foundation internals.

Current handoff:

```text
FCR-0082 = Waiting On FOUNDATION
```

Shared Web owns its presentation/interaction/Web-local resilience. It does not own FSATS/FSA/Guardian/Trading truth or Kill authority.

Current direct Web handoff:

```text
FCR-0083 = Waiting On WEB
```

Exact cross-workstream communication contracts remain:

```text
FCR-0080 = Waiting On FOUNDATION
```

These external pending handoffs do not block this Application-side design/review cycle. They block only claims that final Foundation/Web/external runtime realization is complete.

## 20. Manifest and Contract Impact

P1-E SHALL eventually declare per Application, as applicable:

- degraded behavior;
- failure-containment interface;
- safety-continuity/recovery expectations;
- required Guardian/protection interface;
- relevant Kill/restriction dependencies without inventing Foundation fields.

P1-K SHALL eventually materialize exact Application-owned contract families for:

- safety/degraded status;
- position/order/protection continuity where externally consumed;
- Kill/containment status/outcome where Application-owned;
- reconciliation state;
- recovery/Controlled Revival status;
- required Foundation/Web bindings after applicable FCR disposition.

## 21. P1-L Verification Requirements

P1-L SHALL require executable evidence, when implementation authority later exists, proving at minimum:

1. localized AI failure can be contained without automatic whole-Application shutdown when trust remains local;
2. unknown trust blast radius expands containment rather than assuming safety;
3. killed AI output cannot create new risk;
4. no live exposure becomes unmonitored/ownerless due to AI Kill;
5. existing protected positions remain monitored without requiring killed intelligence;
6. unknown broker/order/fill truth freezes new risk and reconciles before further action;
7. degraded actions cannot silently increase risk;
8. Guardian AI loss does not automatically remove independently trusted hard protections;
9. loss of safety-kernel trust itself fails closed/expands containment;
10. no authority inheritance after Kill;
11. broker-native protection differences are capability-profile verified;
12. safety continuity survives restart/failover tests where applicable;
13. incident/evidence history survives remediation/revival;
14. Controlled Revival is required before killed intelligence regains trust;
15. external Foundation/Web dependencies are not falsely represented as implemented before their evidence exists.

## 22. Work-Package Impact

This cross-cutting candidate shall be consumed prospectively by:

```text
P1-D  structural types/primitives
P1-E  Manifest/degraded/lifecycle declarations
P1-F  Trading 13-LSA decomposition
P1-G  FSAPMA 6-LSA decomposition
P1-H  Guardian 4-LSA decomposition
P1-I  FSTSimA 8-LSA decomposition
P1-J  APP-RSC resource continuity behavior
P1-K  contracts/events/routes
P1-L  integrated failure/security/readiness verification
```

It does not close or replace any of those WPs by itself.

## 23. Non-Authority

This record is design only.

It grants no:

- implementation authority;
- runtime route activation;
- provider/broker connectivity;
- Paper/Shadow/Tiny Live/Live authority;
- deployment authority;
- Foundation implementation authority;
- Shared Web implementation authority.

## 24. Review Lifecycle

Because this is a semantic Part 1 change, it requires:

```text
CANDIDATE
-> EXACT SEMANTIC FREEZE
-> FRESH ARCHITECTURE / CONSISTENCY REVIEW
-> FRESH RED-TEAM REVIEW
-> PROJECT OWNER FINAL REVIEW
-> EXPLICIT OWNER DECISION
```

No earlier review PASS is reused for this new semantic scope.