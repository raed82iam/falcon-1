# FSATS V1.4 Part 0 / P0-J — Start and Scope Control Record

**Status:** `P0J_AUTHORIZED_AND_STARTED`
**Date:** `2026-08-08`
**Authority:** explicit Project Owner instruction to begin the next Part 0 work package after P0-I closure
**Scope:** P0-J only
**Canonical work-package title:** `Fast Track, Performance, Priority and Load-Shedding Design`
**Start HEAD:** `f447fe6d9ef47d784c5362da7db3f2b8dbb9af36`

## 1. Predecessor state

P0-A through P0-I are `OWNER_ACCEPTED_AND_CLOSED`.

P0-J begins from the accepted Application topology, cross-Application contracts, FSAPMA operational-data architecture, Trading Core, Guardian protection/resource-escalation model and all previously accepted anti-reimplementation boundaries.

## 2. Authorized objective

P0-J may define and review:

- latency-sensitive Trading/Guardian/FSAPMA business fast-path semantics;
- deadline propagation and expiry;
- priority classes and queue discipline;
- bounded queues/backpressure;
- Application-owned load shedding;
- protection of open-position, Risk, Guardian, reconciliation and execution-critical work under pressure;
- safe use of precomputed/versioned snapshots;
- latency-class eligibility and feasibility;
- performance/tail-latency evidence;
- same-Application colocation without authority leakage;
- cross-Application performance dependencies through governed Foundation transport;
- resource-pressure response within admitted Application allocations.

## 3. Preserved non-bypass boundaries

Fast Track SHALL NOT bypass:

- exact user/account/environment scope;
- FSAPMA operational-data truth classification;
- strategy applicability/feasibility;
- Unified Risk;
- Trading capital reservation and intent binding;
- Guardian restrictions;
- Owner/user/subscription controls;
- broker/account/capability binding;
- authority, correlation, causation and idempotency;
- reconciliation;
- Foundation cross-Application routing/delivery/admission/security boundaries.

Performance does not create authority.

## 4. External dependency truth

- `FCR-0009 = ACCEPTED_FOR_PLANNING / OPEN` — latency/deadline/QoS-aware transport; non-blocking for Part 0 design, blocking for claims that cross-Application Fast Track runtime behavior is complete.
- `FCR-0010 = ACCEPTED_FOR_PLANNING / OPEN` — resource-pressure/load-shedding signals; non-blocking for Part 0 design, blocking for Foundation-aware runtime load shedding/resource claims.

No FCR planning disposition is runtime implementation authority.

## 5. Non-authorities

P0-J start does NOT authorize:

- P0-K or P0-L;
- Part 1 or later implementation;
- modification of Foundation from the Application workstream;
- runtime provider/broker connectivity;
- Paper, Tiny Live or Live operation;
- deployment or production adoption.

## 6. Review sequence

P0-J shall proceed through candidate definition, fresh architecture/consistency review, adversarial Red-Team, remediation if required, fresh current-version review and Owner review candidate preparation.

`P0J_FINAL_OWNER_ACCEPTANCE = NOT_GRANTED`
