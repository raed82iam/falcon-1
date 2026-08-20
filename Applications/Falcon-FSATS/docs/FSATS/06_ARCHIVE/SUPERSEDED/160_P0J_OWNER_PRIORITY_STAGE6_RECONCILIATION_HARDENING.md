# FSATS V1.4 Part 0 / P0-J — Owner Priority and Stage 6 Reconciliation Hardening

**Status:** `CONTROLLING_P0J_SEMANTIC_AMENDMENT_PENDING_FRESH_REVIEW`
**Date:** `2026-08-08`
**Parent authority:** `158_P0J_OWNER_UNFREEZE_AND_REOPEN_RECORD.md`
**Prior P0-J semantic set:** `132`, `133`, `133A`, `133B`, `133C`, `138`, `139`
**Owner priority evidence:** Foundation `docs/stage-6/05_OWNER_PRIORITY_CLARIFICATION_AND_APPLICATION_INPUT_REQUEST.md`
**FCR reconciliation evidence:** FCR-0007 comment `5227067310`; current FCR-0007/FCR-0010 canonical headers

## 1. Purpose

Reconcile P0-J with the later explicit Owner cross-Application resource-priority clarification without changing P0-J's Fast Track, authority, deadline, load-shedding, isolation, or recovery architecture.

This amendment exists because the prior Application statement that no Owner-approved blanket Trading priority existed was superseded by a later explicit Owner design clarification recorded by Foundation.

## 2. Controlling cross-Application priority rule

Trading-related Applications are the highest Owner-approved **Application-priority domain** for Foundation-governed technical resources.

This rule does not place Applications above Foundation survival/control obligations. Foundation-owned minimum resources required for platform survival, protection, authority, health/recovery, evidence/security integrity, governance, revocation and restoration remain protected above Application workloads.

Conceptually:

```text
FOUNDATION_SURVIVAL_AND_CONTROL_FLOORS
        >
OWNER_APPROVED_TRADING_APPLICATION_PRIORITY_DOMAIN
        >
LOWER_PRIORITY_RECLAIMABLE_APPLICATION_WORKLOADS
```

The ordering is a governance/resource-priority rule, not a Trading business-decision rule.

## 3. What the priority rule does and does not grant

The Owner priority rule means Foundation may prefer Trading-related Applications over lower-priority Applications when allocating or rebalancing reclaimable Application resources, including under pressure/crisis/recovery conditions.

Where required for the highest-priority Trading workload, Foundation may reduce or reclaim legitimately reclaimable lower-priority Application allocations, including all such reclaimable allocation under severe/critical conditions.

The rule does **not** grant any Trading Application, user, component, LSA, CSA, MSA, Guardian, strategy or caller authority to:

- self-allocate Foundation resources;
- select the exact granted quantity;
- exceed a Foundation grant/ceiling;
- consume non-reclaimable Foundation survival/control floors;
- consume another Application's allocation directly;
- declare its own Foundation technical-criticality by metadata;
- treat requested capacity as granted capacity;
- bypass Foundation admission/resource-governance decisions;
- bypass Guardian, Risk, capital, execution, contract, security or other required authority gates.

`REQUESTED_RESOURCE != GRANTED_RESOURCE` remains mandatory.

## 4. Trading domain priority is not flat priority

The Owner-approved Trading domain priority does not mean every Trading workload has equal or maximum technical priority.

Within Trading-related Applications, P0-J's purpose/evidence/context-sensitive hierarchy remains controlling Application evidence:

1. Guardian/protection work;
2. reconciliation and authoritative truth recovery;
3. open-position management;
4. required protective/execution-critical and valid near-dispatch work;
5. active watch;
6. candidate evaluation;
7. discovery/enrichment;
8. research/learning/background improvement.

Foundation remains owner of the final technical resource decision and may use governed technical policy/evidence to map Application claims into actual scheduling/allocation behavior.

A discovery burst, research job or user-generated high-volume workload does not become protection-critical merely because it belongs to Trading.

## 5. Multi-user fairness remains mandatory

The Owner cross-Application priority rule does not create user privilege inside Trading.

One user's candidate/discovery load shall not starve another user's open-position protection, reconciliation or other higher-purpose obligations. User identity alone does not create higher technical priority unless a separate explicit authorized policy says so.

## 6. Guardian/resource escalation relationship

Guardian may submit attributable emergency/protection resource-escalation requests through the governed Application boundary defined by P0-I/FCR-0007 when the minimum safe Trading protection set cannot be maintained from current admitted capacity.

Guardian does not allocate resources. Foundation may grant, partially grant, cap, deny, reduce, revoke, rebalance or restore according to its authority and the Owner-approved cross-Application priority rule.

Ordinary Trading/FSAPMA capacity requests remain attributable to the exact owning Application. Internal LSAs/CSAs/components may provide evidence but are not Foundation resource principals.

## 7. Pressure/load-shedding relationship

FCR-0010 remains the planning dependency for the broader Application-facing Foundation resource-pressure/allocation boundary.

Until the relevant Foundation capability is implemented and verified:

- Applications shall not fabricate Foundation allocation/pressure truth;
- no missing pressure signal implies extra capacity;
- load shedding remains bounded to actual admitted/granted capacity;
- lower-priority Trading work sheds before protection/truth obligations;
- inability to preserve minimum safe protection triggers restriction/escalation rather than pretending normal operation continues.

## 8. Fast Track relationship

This amendment does not change the distinction:

```text
FAST != IMPORTANT != AUTHORIZED
CRITICAL != NEAR_TRADE_FAST_TRACK
```

Near-Trade Fast Track remains a governed time-sensitive execution shape. Owner-approved Trading domain priority does not allow Fast Track to skip mutable authority/truth gates or convert an expired opportunity into valid work.

## 9. Stage 6 / FCR compatibility

Current reconciliation truth relevant to this amendment:

- FCR-0007 Application declaration was accepted by Foundation with Owner-priority reconciliation; no further Application clarification is currently required.
- FCR-0010 Application pressure/load-shedding semantics were reconciled into Stage 6 planning; runtime implementation remains future Foundation work.
- FCR-0016 artifact publication/consumption is a separate Application-neutral Foundation capability family and does not alter P0-J priority semantics.

Open FCRs do not create runtime authority merely by being accepted for planning.

## 10. Historical statement supersession

Any earlier P0-J/Application statement equivalent to:

`BLANKET_TRADING_PRIORITY_OVER_OTHER_APPLICATIONS = NOT_OWNER_APPROVED`

is superseded by the later explicit Owner priority clarification.

This supersession is narrow. All surrounding anti-self-promotion, Foundation ownership, fairness, isolation, fail-closed, deadline, load-shedding and protected-capacity rules remain in force.

## 11. Required fresh review

Because this is a semantic amendment after P0-J reopen, prior P0-J PASS reports remain provenance only for their reviewed bytes.

Before Owner closure, the amended P0-J semantic set requires:

1. exact candidate binding;
2. fresh Architecture/Consistency review;
3. fresh adversarial Red-Team;
4. fresh P0-A through P0-J integration/regression review;
5. remediation and rerun for any finding.

## 12. Candidate state

```text
P0J_OWNER_PRIORITY_CONFLICT = REMEDIATED_IN_CANDIDATE
P0J_FAST_TRACK_SEMANTICS_CHANGED = NO
P0J_FOUNDATION_RESOURCE_AUTHORITY_CHANGED = NO
P0J_TRADING_APPLICATION_DOMAIN_PRIORITY = OWNER_APPROVED_HIGHEST_APPLICATION_DOMAIN
FOUNDATION_SURVIVAL_CONTROL_FLOORS = PRESERVED_ABOVE_APPLICATION_WORKLOADS
P0J_FRESH_REVIEW = REQUIRED
P0J_FINAL_OWNER_ACCEPTANCE = NOT_GRANTED
P0J_FINAL_CLOSURE = NOT_GRANTED
P0K_THROUGH_P0L = NOT_AUTHORIZED
RUNTIME / PAPER / TINY_LIVE / LIVE = NOT_GRANTED
```