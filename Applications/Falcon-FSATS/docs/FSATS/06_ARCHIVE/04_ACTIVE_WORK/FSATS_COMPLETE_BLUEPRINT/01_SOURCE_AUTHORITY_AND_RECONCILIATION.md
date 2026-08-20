# FSATS Complete Blueprint — Source, Authority and Reconciliation

**Candidate:** `FSATS-CB-v0.1`
**Status:** `DESIGN_CANDIDATE / NOT_OWNER_ACCEPTED / NOT_CLOSED`
**Implementation Authority:** `NOT GRANTED`

## 1. Governing Order

The candidate applies the following precedence for this design cycle:

```text
FALCON VISION
> FALCON CONSTITUTION
> VALID CURRENT OWNER / GOVERNANCE DECISIONS
> CURRENT APPROVED FOUNDATION SPECIFICATIONS / CONTRACTS / ADRS
> CURRENT LIVE FCR DISPOSITIONS FOR CROSS-WORKSTREAM STATE
> CURRENT ACCEPTED FSATS DESIGN
> CURRENT DESIGN CANDIDATES
> HISTORICAL / ARCHIVE / V1.3 MATERIAL
> EXTERNAL ENGINEERING EVIDENCE
> IMPLEMENTATION CONVENIENCE
```

No lower source silently amends a higher source.

## 2. Mandatory Current Sources Applied

### Falcon Vision and Constitution

Controlling principles include:

- Protect Capital before Manage Capital before Grow Capital.
- intelligence is a means, not an authority source.
- self-awareness does not create authority.
- self-maintenance does not permit self-governance.
- evolution cannot redefine Falcon purpose, expand its own power or weaken obligations.
- authority must be explicit, attributable, bounded, interruptible and revocable.
- observation, analysis, recommendation, decision, authorization and action remain distinguishable.
- high-consequence changes require independent approval and evidence.
- trust/security degradation reduces authority rather than increasing it.

### APP-001 v1.1

Applied invariants:

- each Falcon Application is an independent governed Plug-in Application;
- exactly one MSA per Application;
- exactly one LSA per declared major branch;
- CSA only for eligible intelligent components;
- Application failure must remain contained;
- Foundation use occurs through declared contracts;
- no direct access to another Application's internals;
- self-development uses origin-correct escalation;
- FSA performs final OS-governance/compatibility review only;
- FSA review does not grant implementation, deployment or production adoption.

### CON-023 v1.1

Every Application design must be materializable into a Manifest declaring at least:

- immutable Application/package identity and provenance;
- purpose and business ownership boundary;
- dependencies and Foundation services;
- capabilities and consumers;
- permissions, authority requests and security profile;
- resources, minimums, ceilings, priorities and degraded behavior;
- persistence, communication, configuration and evidence requirements;
- lifecycle/update/rollback/removal behavior;
- health and failure containment;
- MSA, LSAs and CSA eligibility;
- self-development origin/evidence/review route;
- Guardian/protection interface.

Undeclared authority, dependency, route, resource or permission is denied.

### ADR-I012

Foundation remains Application-neutral and valid with zero Applications. FSATS shall not receive a Foundation special case. Application business payload meaning remains Application-owned. Cross-Application interaction uses declared governed contracts/routes.

### ADR-I015

Awareness hierarchy and jurisdiction are fixed:

```text
FSA = FOUNDATION / OS JURISDICTION
MSA = ONE COMPLETE APPLICATION
LSA = ONE MAJOR APPLICATION BRANCH
CSA = ONE ELIGIBLE INTELLIGENT COMPONENT
```

Awareness rank does not create authority or cross-owner access.

### AWR-006 / AWR-007 / AWR-008

The candidate preserves exact MSA, LSA and CSA responsibility boundaries and origin-aware proposal routes. CSA is deliberately sparse rather than attached to every technical component.

### EVO-001

The candidate preserves change classification, isolated candidate construction, independent validation, Safe Evolution Envelope, Shadow/Canary separation, rollback evidence, post-change Fitness to Operate and suspension of evolution when safeguards degrade.

### ADR-I001

Application implementation planning aligns with the accepted Falcon runtime profile unless a later Application-specific accepted decision validly supersedes it:

```text
PRIMARY LANGUAGE = C#
RUNTIME FAMILY = .NET 10 LTS
OS TARGET = WINDOWS + LINUX
```

This blueprint does not itself authorize a toolchain installation, package choice or code implementation.

## 3. Current FSATS Accepted Source

The accepted Part 0 baseline remains current authority until the Owner explicitly accepts a later superseding design.

The accepted Part 0 includes the accepted Awareness amendment and preserves:

- four independent FSATS Applications;
- 4 MSA / 31 LSA topology;
- FSAPMA sole operational external-data gateway;
- independent Trading Guardian;
- independent non-Live FSTSimA;
- contract-first cross-Application communication;
- Unified Risk and capital protection;
- evidence, replay and reconciliation requirements;
- governed awareness/self-development;
- Owner silence not creating authority.

The convenience `P0/` and `P1/` directories are treated as archive/reference for this design cycle by current Owner direction.

## 4. V1.3 Disposition

FSATS V1.3 is `SCRATCH DESIGN REFERENCE / NOT APPROVED`.

Strong concepts retained for evaluation include:

- Trading Guardian Application;
- FSAPMA;
- Self-Aware Trading Application;
- one MSA per Application with local LSA/CSA;
- US Equities + Crypto Spot initial markets;
- 1:1 funded exposure initial model;
- Paper-first progression;
- central Strategy Catalog / Strategy Controller;
- market capability profiles;
- Unified Risk;
- capital reservation ledger concept;
- provider registry/routing/quota/data-quality;
- simulator/shadow/replay/evidence;
- contract/idempotency/reconciliation/provenance concepts.

V1.3 ownership, Foundation assumptions, hidden system-level state, contracts, authority and runtime routes are not inherited automatically.

## 5. Current FCR Reconciliation

### Application implementation holds

The following remain open and `Waiting On: APPLICATION`, but their required next action is intentionally deferred until corresponding code/bindings/fixtures exist:

- FCR-0004 — Guardian protection command route.
- FCR-0005 — FSAPMA operational market-data delivery.
- FCR-0006 — event evidence/replay delivery.
- FCR-0010 — resource pressure/load-shedding consumption.
- FCR-0031 — FSARM aggregate resource-management consumption.

They do not require premature documentary closure during this design phase.

### Accepted / planned future Foundation capabilities

- FCR-0007 — Stage 6 additional-resource request boundary: accepted and closed.
- FCR-0008 — awareness research-only Internet egress: accepted for future Stage 12 planning; runtime unavailable now.
- FCR-0009 — deadline/QoS-aware transport: accepted for future Stage 11 planning; runtime unavailable now.
- FCR-0011 — FSTSimA non-Live isolation/egress guard: future Stage 12.
- FCR-0013 — FSAPMA operational provider egress/credential-reference boundary: future Stage 12.
- FCR-0014 — broker execution egress/credential-reference boundary: future Stage 12.
- FCR-0016 — canonical cross-workstream Foundation artifact consumption: future Stage 14.

### Foundation-owned open Awareness work

- FCR-0012 — comprehensive FSA governance/integrity/monitoring/evolution control plane: `Waiting On: FOUNDATION`.
- FCR-0030 — exact MSA-to-FSA governed interface/transport: `Waiting On: FOUNDATION`.

Application design may define its required outcomes and fail-closed boundary, but shall not design or implement Foundation internals.

## 6. Stage 6 Source Reconciliation Finding

A material documentary freshness difference exists:

- an older Foundation root README snapshot states that Stage 6 WP-05 through WP-10 were not authorized at its effective date;
- later live FCR evidence records subsequent accepted/implemented Stage 6 progress, including WP-05 through WP-09 Foundation capability chain and accepted Application compatibility checkpoints.

For current cross-workstream planning, this blueprint uses the later live FCR state and referenced Owner/verification evidence as the more recent evidence for the exact FCR-scoped capability claims. It does not reinterpret unrelated Foundation status.

The difference remains visible and shall be rechecked before implementation authorization.

## 7. Current Owner Design-Cycle Direction

The Owner has directed a complete new design synthesis before code:

```text
ARCHIVES / V1.3 = KNOWLEDGE INPUT
CURRENT FOUNDATION + FCR + GOVERNING RULES = AUTHORITY INPUT
BEST COMPATIBLE IDEAS = DESIGN INPUT
INTERNET = OPTIONAL RESEARCH / CHALLENGE INPUT
OWNER REVIEW = REQUIRED BEFORE CODE
```

This direction authorizes preparation and review of this design candidate only. It does not authorize implementation.

## 8. Reconciliation Rules Used by This Candidate

Every retained or new design feature must satisfy all of:

1. purpose is explicit;
2. owner is explicit;
3. non-owner is explicit;
4. authority source is explicit;
5. Foundation dependency is explicit;
6. data/truth source is explicit;
7. failure behavior is explicit;
8. security boundary is explicit;
9. evidence obligation is explicit;
10. testing obligation is explicit;
11. historical lineage is explainable;
12. no hidden runtime principal is created;
13. no AI confidence becomes authority;
14. no technical capability becomes permission;
15. unresolved authority fails closed.

## 9. Candidate Delta Discipline

A design improvement may replace a historical mechanism while preserving the accepted requirement or Owner intent. Every material semantic delta must be classified as:

- `CONSOLIDATION_ONLY`;
- `HARDENING`;
- `MECHANISM_REPLACEMENT_SAME_INTENT`;
- `OWNER_DIRECTED_CHANGE`;
- `NEW_CANDIDATE_CAPABILITY`;
- `FOUNDATION_DEPENDENT`;
- `DEFERRED`.

No semantic delta is considered accepted until explicit Owner acceptance after fresh review.
