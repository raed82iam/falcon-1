# RSK-006 — Trading Guardian

**Identifier:** RSK-006 — Proposed Reservation  
**Version:** Proposed 1.0  
**Status:** Approved Design — Not Effective  
**Approval Record:** GOV-062  
**Domain:** Risk and Protection  
**Architectural Location:** Trading Application Suite, outside Falcon Foundation  
**Owner:** Trading Protection Authority — Proposed  
**Stage 1 Authority:** Not Granted

## 1. Purpose

Trading Guardian (TG) is the independent Application-level protection authority for the Trading Application Suite.

TG may understand trading-domain conditions necessary to protect capital, exposure, orders, positions, execution safety, and Trading continuity.

TG does not protect Falcon Foundation or directly control another Application.

## 2. Position

TG SHALL be independently deployable and mandatory for Trading Suite activation. It SHALL not reside inside Foundation or depend entirely on FSATA lifecycle.

Removal of Trading Suite MAY remove TG without reducing Foundation completeness.

## 3. Scope

TG governs:

- Trading protective modes;
- restriction of new exposure and Trading authority;
- protective management of existing positions;
- Trading protection requests to FFG;
- Trading-domain recovery assessment;
- persistence of Trading restrictions; and
- protection evidence.

## 4. Non-Scope

TG SHALL NOT:

- become Broker Execution;
- perform broker-facing action directly;
- modify Foundation or another Application;
- isolate another Application;
- activate Platform Safe Mode;
- reallocate Foundation resources;
- override FFG or AUT-001;
- approve its own authority expansion; or
- interpret another Application’s business records.

## 5. Trading Knowledge

Within Approved access and minimization rules, TG MAY understand capital-protection state, exposure, open positions, pending orders, Stop Loss, Take Profit, execution readiness and uncertainty, risk-limit availability, portfolio protection, Trading authority, and Trading recovery.

Possession of knowledge SHALL NOT create execution or cross-Application authority.

## 6. Mandatory Dependency

Trading Suite activation requires verified TG identity, Approved version, available authority, loaded policy, evidence path, FFG communication, execution/risk communication, and recovery readiness.

When TG is missing or untrusted:

- new positions and exposure increase are prohibited;
- existing positions may receive only Approved restricted protective management;
- `TRADING_NORMAL` is prohibited;
- restrictions survive restart; and
- competent authorities are notified.

## 7. Trading Modes

- `TRADING_NORMAL`: ordinary Approved Trading authority.
- `TRADING_HEIGHTENED`: increased evidence and protection readiness.
- `TRADING_RESTRICTED`: no unsupported new exposure; selected protective actions remain.
- `TRADING_SAFE`: new Trading decisions and exposure increase stop; only authorized protection capabilities remain.
- `TRADING_RECOVERY_GUARD`: progressive Trading restoration under verification.

TG modes do not change Platform modes.

## 8. Protective Actions

Within mandate TG MAY:

- deny new positions or exposure increase;
- allow cancellation, closing, and approved protective orders;
- restrict selected Trading capabilities;
- require additional evidence;
- request protective execution;
- request emergency closure when authorized; and
- issue Trading restrictions through AUT-001 and competent execution paths.

Broker Execution performs broker-facing action and reports actual outcome and uncertainty.

## 9. Protection Requests

TG MAY request from FFG technical investigation, monitoring, capability preservation, resource protection, traffic restriction, component/Application isolation, Platform Containment, or Platform Safe Mode through CON-022.

TG SHALL provide minimal technical effect, evidence, urgency, and requested treatment. Capital, portfolio, order, position, customer, or strategy details SHALL NOT be included unless a separately Approved Contract explicitly requires and protects them; CON-022 does not.

## 10. Relationships

- **FSATA:** a Trading Application whose failure SHALL NOT by itself remove TG.
- **FSAOL:** may supply governed Trading awareness; it cannot release TG restrictions.
- **Trading Risk:** owns risk evaluation and limit facts; TG owns protective restriction within mandate.
- **Broker Execution:** executes authorized broker-facing protection; TG does not.
- **MSA/LSAs:** supply or receive minimized awareness; awareness rank does not create protection authority.
- **FFG:** independently decides Platform and cross-Application protection.
- **AUT-001:** validates every Trading restriction and protection request.

These relationships remain boundary requirements until their missing Specifications are approved.

## 11. Recovery and Release

Trading restriction reduction requires Trading protection, execution, risk, exposure-management, authority, and evidence conditions appropriate to consequence.

TG controls its restriction condition within mandate; independent verification and a separate release authority are required where consequence policy demands.

`PLATFORM_NORMAL` does not establish `TRADING_NORMAL`. TG cannot override an active Platform restriction.

## 12. Failure and Compromise

TG loss or uncertainty prohibits new exposure. Existing positions enter the safest Approved protective management available.

A compromised TG SHALL be isolatable, unable to release itself conclusively, unable to issue cross-Application commands, and unable to erase evidence.

## 13. Normative Requirements

- **RSK-006-REQ-001:** TG SHALL be outside Foundation and independently deployable.
- **RSK-006-REQ-002:** TG SHALL be mandatory for Trading Suite activation.
- **RSK-006-REQ-003:** TG SHALL remain available independently of any single Trading Application where technically possible.
- **RSK-006-REQ-004:** Missing or untrusted TG SHALL prohibit new exposure.
- **RSK-006-REQ-005:** TG SHALL own Trading modes and restrictions only within its mandate.
- **RSK-006-REQ-006:** TG SHALL NOT perform broker-facing execution.
- **RSK-006-REQ-007:** TG SHALL request cross-Application protection only through CON-022.
- **RSK-006-REQ-008:** TG SHALL NOT directly isolate another Application or activate a Platform mode.
- **RSK-006-REQ-009:** TG SHALL minimize business information disclosed to Foundation.
- **RSK-006-REQ-010:** Trading and Platform recovery SHALL remain independent.

## 14. Acceptance Evidence

Acceptance requires independent survival from FSATA failure, new-exposure denial, protective existing-position handling, CON-022 requests, inability to isolate another Application, broker-execution separation, restart persistence, independent Trading recovery, and complete evidence reconstruction.

## 15. Unresolved Matters

Trading Suite Manifest, FSATA, FSAOL, Trading Risk, Broker Execution, Provider Management, Trading authority, risk-limit, protective-order, and emergency-closure Specifications are not yet Approved.
