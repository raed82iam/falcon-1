# FSATS V1.4 Part 0 / P0-I — Start and Scope Control Record

**Status:** `P0I_AUTHORIZED_AND_STARTED`
**Date:** `2026-08-08`
**Authority:** explicit Project Owner instruction to begin the next Part 0 work package after P0-H closure
**Scope:** P0-I only
**Canonical work-package title:** `Guardian, Crisis, Protection and Resource-Escalation Design`
**Start HEAD:** `b56a5868f84e128dea6ea8c04b0db9a9036c914b`

## 1. Predecessor state

P0-A through P0-H are `OWNER_ACCEPTED_AND_CLOSED`.

P0-I begins from the accepted P0-H Trading Core design and does not reopen or redesign accepted predecessor ownership.

## 2. Authorized objective

P0-I may define and review the Falcon Trading Guardian Application business architecture for:

- protection and crisis-state semantics;
- crisis detection and evidence thresholds;
- restrictions on new exposure and affected Trading scopes;
- continued protection and supervision of open exposure;
- emergency/protective action requests through Trading Execution;
- interaction with Unified Risk, Trading Core, FSAPMA and Shared Applications;
- business recovery/readiness gates;
- Guardian-originated evidenced resource-escalation requests and load-shedding priorities while preserving Foundation resource authority;
- multi-user, multi-market and multi-broker crisis scoping;
- auditability, explainability, fail-closed behavior and anti-authority-leakage.

## 3. Preserved boundaries

P0-I SHALL NOT make Guardian:

- a strategy selector or profit optimizer;
- the owner of Unified Risk normal business decisions;
- a broker transport or direct execution engine;
- an operational market-data provider or provider selector;
- a Foundation lifecycle/resource/security manager;
- an owner of CPU/RAM/platform resource truth or allocation authority;
- an Application-wide hidden super-principal;
- an authority to bypass Owner, regulatory, broker/account, Risk or other separately governing constraints;
- a self-approving code-modification or Live-promotion authority.

Guardian protection authority is strong and independent, but bounded and attributable.

## 4. Existing external dependency truth

P0-I SHALL reuse existing Foundation requests rather than create duplicate capability claims:

- `FCR-0007` — Falcon Trading Guardian Foundation resource escalation request boundary: `ACCEPTED_FOR_PLANNING / OPEN`;
- `FCR-0010` — FSATS Applications resource pressure and load-shedding signals: `ACCEPTED_FOR_PLANNING / OPEN`;
- `FCR-0013` — FSAPMA governed operational-provider egress: `ACCEPTED_FOR_PLANNING / OPEN`;
- `FCR-0014` — Trading Execution governed broker-execution egress: `ACCEPTED_FOR_PLANNING / OPEN`.

Acceptance for planning does not grant runtime implementation or connectivity authority.

## 5. Current non-authorities

P0-I start does NOT authorize:

- P0-J through P0-L;
- Part 1 or later implementation;
- Foundation modification from the Application workstream;
- runtime provider or broker connectivity;
- operational credential use;
- Paper, Tiny Live or Live trading;
- deployment, activation or production adoption.

## 6. Required review sequence

P0-I shall proceed through:

1. canonical candidate definition;
2. fresh architecture/consistency review;
3. fresh adversarial Red-Team;
4. remediation of material findings;
5. fresh post-remediation review;
6. Owner review only after zero unresolved material findings.

`P0I_FINAL_OWNER_ACCEPTANCE = NOT_GRANTED`
