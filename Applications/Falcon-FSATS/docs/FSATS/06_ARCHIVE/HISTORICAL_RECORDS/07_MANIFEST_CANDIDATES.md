# FSATS V1.4 PROPOSED — CON-023 Manifest Alignment Candidates

## Status

**Status:** `PART 0 ALIGNMENT CANDIDATE / OWNER REVIEW REQUIRED`  
**Authority:** design only; no implementation, deployment, external connectivity, Paper, Tiny Live, or Live authority.

These candidates align the final V1.3 Application model to current APP-001 / CON-023 / ADR-I012 / ADR-I015 / SYS-006 authority.

## Canonical Application identity rule

FSATS itself is a non-owning Trading System Boundary. It is not a Falcon Application and owns no MSA, Foundation lifecycle, Foundation resource allocation, credentials, hidden persistence, or runtime principal identity.

Three independent Falcon Applications exist inside the FSATS operational boundary:

1. Falcon Trading Guardian Application.
2. Falcon Self-Aware Provider Management Application (FSAPMA).
3. Falcon Self-Aware Trading Application.

The final V1.3 package also defines independent adjacent Applications outside FSATS operational authority:

4. Falcon Self-Aware Trading Simulator Application (FSTSimA), independent non-Live validation Application.
5. Falcon Web Application, independent Shared Application.
6. Falcon Communication Application, independent Shared Application.

The latter three SHALL NOT be absorbed into FSATS merely because they serve trading workflows.

# A. Falcon Trading Guardian Application

**Application identity:** `falcon.trading.guardian`  
**Purpose:** trading-domain crisis detection, scoped protection, open-position protection, safe-mode business control, recovery and reconciliation.  
**Business boundary:** trading protection only. Excludes Foundation lifecycle, admission, security governance, total-resource governance, inter-Application transport ownership, and direct access to another Application's internals.  
**MSA:** `MSA-GUARDIAN`  
**Major branches / LSAs:** exactly four:

1. Crisis Detection and Severity LSA.
2. Incident Command and Safe Mode LSA.
3. Open Position Protection LSA.
4. Recovery and Reconciliation LSA.

**Awareness locality:** Guardian MSA understands Guardian only. Each LSA understands its own branch and child eligible CSAs only. CSA is limited to one eligible intelligent component. Binding operational protection actions are issued by Guardian operational controllers through governed contracts, never by awareness hierarchy as a hidden integration path.  
**Provided capabilities:** scoped trading protection commands, crisis posture, open-position protection requirements, recovery-readiness state, incident evidence.  
**Primary consumers:** Trading Application and FSAPMA through declared contracts.  
**Foundation dependencies:** APP-001 lifecycle; CON-023 Manifest; governed communication/routing; schema; identity/permissions; security; evidence; health; persistence; dependency governance; per-Application resources.  
**Resource model:** Guardian receives its own Foundation allocation and distributes only that allocation internally. During a broad evidenced trading threat it may request additional resources from the Foundation-owned Guardian/resource boundary; it never self-allocates Foundation resources.  
**Containment rule:** smallest-safe-scope containment is mandatory. A user/account-local issue remains local where safely possible; broader restrictions require evidence of broader risk.  
**Persistence:** Guardian policy/configuration, protection state, incident evidence, recovery/reconciliation evidence.  
**Failure/degraded behavior:** Guardian uncertainty shall never create additional trading authority. Target Applications must have declared safe behavior for unavailable/stale Guardian state.  
**Self-development:** CSA → owning LSA → Guardian MSA → FSA OS-compatibility review where applicable → separate Owner/governance adoption.  
**Rollback:** versioned policy/playbook rollback and safe restrictive fallback.

# B. Falcon Self-Aware Provider Management Application (FSAPMA)

**Application identity:** `falcon.trading.fsapma`  
**Purpose:** external operational information and provider services required specifically for trading.  
**Business boundary:** provider identity/capability, canonical trading-data products, provider selection/routing business logic, external quota/capacity/cost knowledge, data quality/reconciliation, broker/account capability. Excludes Foundation networking, Service Bus, total-resource governance, security governance, or general Falcon Internet ownership.  
**MSA:** `MSA-FSAPMA`  
**Major branches / LSAs:** exactly six:

1. Provider Registry and Onboarding LSA.
2. Data Product and Semantics LSA.
3. Provider Selection and Routing LSA.
4. Quota, Capacity, and Cost LSA.
5. Data Quality and Reconciliation LSA.
6. Broker and Account Capability LSA.

**Operational-data rule:** operational data used in Paper or Live decisions enters through FSAPMA and governed contracts. Awareness Internet research is a separate non-operational path.  
**Awareness locality:** FSAPMA MSA understands FSAPMA only; each LSA owns one room; each eligible CSA owns one component/capability. Awareness does not call provider APIs or perform operational controller work.  
**Provided capabilities:** normalized trading data, provider capability truth, provider selection/fallback outcomes, quality/freshness/lineage state, external quota/capacity knowledge, broker/account capability truth.  
**Primary consumers:** Trading Application and Guardian through declared contracts.  
**Foundation dependencies:** lifecycle/Manifest, communication/routing, schema, security, external connectivity permission, evidence, health, persistence, dependency governance, per-Application resources.  
**Resource model:** independent Foundation allocation. Provider API quotas and free-tier capacity are FSAPMA business/provider state and are not Foundation technical resources.  
**Failure/degraded behavior:** stale, conflicting, unavailable, or insufficient provider truth must be surfaced explicitly and reduce downstream authority where required.  
**Self-development:** CSA → owning LSA → FSAPMA MSA → FSA compatibility review where applicable → separate Owner/governance adoption.  
**Rollback:** versioned provider/profile/routing policy rollback with safe degraded mode.

# C. Falcon Self-Aware Trading Application

**Application identity:** `falcon.trading.application`  
**Purpose:** market understanding, analysis, schools, strategies, decision orchestration, trading risk, portfolio/capital, execution/positions, learning, analytics and strategy evolution.  
**Business boundary:** trading business logic only. Excludes Foundation lifecycle, admission, security, transport, total-resource governance, and provider acquisition ownership.  
**MSA:** `MSA-TRADING`  
**Major branches / LSAs:** exactly twelve:

1. Operations, Account, and Environment.
2. Market and Instrument Universe.
3. Analysis Frameworks.
4. Classical Trading School.
5. Opportunity Hunting School.
6. Strategy Orchestration and Decision.
7. Unified Risk Management.
8. Portfolio and Capital Management.
9. Execution and Position Lifecycle.
10. Trading Learning and Knowledge.
11. Trading Analytics and Attribution.
12. Strategy Evolution and Experimentation.

**Awareness locality:** CSA sees one eligible component; LSA sees one room through declared evidence; Trading MSA sees Trading through LSA reports. No SA enters sibling private state or performs another owner's operational work. All MSA/LSA/CSA entities remain outside the synchronous hot path.  
**Initial preserved V1.3 baseline:** US Equities + Crypto Spot; cash-funded 1:1; long-only initial authority; 2 schools; 10 strategy models; INTRADAY and SHORT_SWING implementation-eligible; MEDIUM_TERM and LONG_TERM disabled unless separately activated.  
**Primary dependencies:** FSAPMA for operational external trading data; Guardian for governed protection state/commands; Shared Web and Communication through declared external contracts; FSTSimA for non-Live validation evidence through governed interfaces.  
**Required Foundation dependencies:** lifecycle/Manifest, identity/permissions, communication/routing, schema, security, persistence, evidence, health, dependency governance, per-Application resources.  
**Resource model:** independent Foundation allocation. Trading may distribute its admitted allocation internally. Financial capital, cash, buying power and reservation ledgers are business state and never Foundation CPU/RAM/network/storage authority.  
**Fast Track:** preserve V1.3 deadline propagation, precomputed immutable snapshots, bounded priority paths, Fast Risk/feasibility guards, tail-latency measurement and load shedding. Fast Track never bypasses Guardian, Risk, authority, evidence, or reconciliation controls.  
**Failure/degraded behavior:** stale operational data, Guardian uncertainty, route loss, reconciliation mismatch, insufficient authority or insufficient truth must reduce authority and may prohibit new exposure.  
**Self-development:** origin-aware CSA/LSA/MSA escalation to FSA compatibility review where applicable, followed by separate Owner/governance adoption.  
**Rollback:** versioned strategy/model/configuration rollback, safe restriction posture, reconciliation before resumption.

# D. Falcon Self-Aware Trading Simulator Application (FSTSimA)

**Application identity:** `falcon.trading.simulator`  
**Position:** independent non-Live Falcon Application outside FSATS operational authority.  
**Purpose:** controlled, reproducible, adversarial and progressively realistic validation of Falcon trading components using the same production logic/contracts with simulation-specific clocks and external adapters.  
**MSA:** `MSA-SIMULATION`  
**Major branches / LSAs:** eight:

1. Simulation Time and Scenario.
2. Market Environment Simulation.
3. Provider and External Service Simulation.
4. Broker, Exchange, and Execution Simulation.
5. Account, Capital, and Settlement Simulation.
6. Fault, Latency, and Crisis Injection.
7. Fidelity and Calibration.
8. Evidence, Oracle, and Reproducibility.

**Authority prohibitions:** no Live credentials, no Live order authority, no production-state mutation, no self-fidelity approval, no candidate activation/promotion, no weakening of Guardian or Risk.  
**Isolation:** separate credentials, networks, stores, namespaces, clocks and authority scopes from Paper/Live; egress controls must prevent accidental Live endpoint access.  
**Foundation alignment:** requires its own APP-001 lifecycle, CON-023 Manifest, allocation, permissions, routes, persistence, health and failure containment. It is not a sub-room of Trading and does not inherit Trading authority.

# E. Falcon Web Application

**Position:** independent Shared Falcon Application outside FSATS.  
**V1.4 treatment:** preserve V1.3 Web capability/boundary as an external dependency. Its full current-Foundation Manifest alignment belongs to the Shared Application workstream. FSATS may declare presentation/control contracts only and SHALL NOT absorb Web identity, authentication, session, API gateway, entitlement or security ownership.

# F. Falcon Communication Application

**Position:** independent Shared Falcon Application outside FSATS.  
**V1.4 treatment:** preserve V1.3 Communication capability/boundary as an external dependency. Its full current-Foundation Manifest alignment belongs to the Shared Application workstream. FSATS may issue governed notification/report requests only and SHALL NOT absorb channel delivery, recipient routing, delivery state or communication-security ownership.

## Part 0 remaining Manifest closure

Before V1.4 design acceptance, the core FSATS Applications and FSTSimA candidates must bind or explicitly mark pending:

- exact package/application versions;
- exact Foundation contract identifiers available at closure time;
- requested permissions/security profile;
- resource minimums/ceilings/degraded behavior;
- persistence requirements;
- communication route families and schemas;
- health/failure-containment behavior;
- update/rollback/removal behavior;
- FCR dependencies for capabilities not yet available.

No unresolved field may be silently filled by implementation convenience.