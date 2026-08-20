# FSATS Part 1 — AI Containment Red-Team Remediation Candidate

**Status:** `CONTROLLING REMEDIATION CANDIDATE / NOT_OWNER_ACCEPTED / NOT_CLOSED`  
**Controls:** `01_PART1_AI_CONTAINMENT_AND_SAFETY_CONTINUITY_CANDIDATE.md` for the clauses below  
**Implementation Authority:** `NOT GRANTED`  
**Runtime Authority:** `NOT GRANTED`

## 1. Purpose

The first adversarial review of the frozen AI containment/safety-continuity candidate identified two material gaps that must be corrected before a final PASS can be claimed:

1. Kill/containment of an AI source did not explicitly fence already-created downstream work that was queued, cached, scheduled or in flight before the Kill became effective.
2. Safety-continuity state/protection envelopes were not explicitly required to remain reconstructable outside the volatile memory/control of the killed AI scope.

This record adds the required controlling semantics without rewriting the historical first freeze/review evidence.

## 2. Downstream Derived-Action Fencing

Kill/containment SHALL apply not only to future outputs from the affected intelligent scope, but also to attributable downstream work derived from that scope that has not yet reached an irreversible authoritative outcome.

Candidate invariant:

```text
KILLED / UNTRUSTED AI SOURCE
-> NO NEW OUTPUT
-> FENCE INVALIDATED DERIVED WORK
-> REVALIDATE OR CANCEL / REJECT AS GOVERNED
```

Affected downstream work includes as applicable:

- queued trade candidates;
- scheduled strategy actions;
- pending order intents;
- pre-dispatch approvals based on invalidated AI evidence;
- cached recommendations awaiting execution;
- resource-redistribution candidates;
- provider-selection candidates;
- automated follow-up actions;
- other causally attributable work that depends materially on the killed/untrusted scope.

### 2.1 Irreversible/Already Externalized Work

If the derived action may already have crossed an external or irreversible boundary, Falcon SHALL NOT assume that fencing prevented it.

Examples include:

- an order submission whose broker acceptance is uncertain;
- a provider/resource action with ambiguous outcome;
- an external request whose delivery/acceptance state is unknown.

Such work SHALL enter the owning domain's reconciliation path.

```text
KILL ISSUED
!= PROOF THAT PRE-KILL IN-FLIGHT ACTION DID NOT COMPLETE
```

### 2.2 Causation / Epoch Requirement

Future P1-D/P1-K materialization SHALL provide enough identity/version/epoch/correlation/causation information to determine whether a pending action was derived from an invalidated intelligence state.

A stale pre-Kill authority/evidence epoch SHALL NOT be accepted as fresh authority merely because the action was queued before Kill.

### 2.3 No Blanket Cancellation of Protective Work

Fencing SHALL distinguish risk-creating/invalidated work from independently trusted protective work.

```text
KILL AI
-> FENCE INVALIDATED RISK-CREATING DERIVED WORK
-> DO NOT BLINDLY CANCEL VALID INDEPENDENT PROTECTIVE OBLIGATIONS
```

Protective orders/actions are cancelled/replaced only through their owning safety/execution rules and current authoritative truth.

## 3. Safety-Continuity State Must Be Reconstructable

The Position Safety Envelope and equivalent continuity state for other Applications SHALL NOT depend solely on volatile memory, hidden state or self-assertion owned by the AI scope that may be killed.

Candidate invariant:

```text
SAFETY CONTINUITY REQUIRES
TRUSTED RECONSTRUCTABLE STATE
OUTSIDE THE SOLE CONTROL OF THE KILLED SUBJECT
```

The exact persistence mechanism remains subject to P1-E/P1-K/Foundation capabilities, but the Application design must be able to reconstruct as applicable:

- current exposure/obligation identity;
- last trusted Risk/protection decision and epoch;
- current known broker/order/protection state;
- current safety owner;
- permitted degraded operations;
- unresolved reconciliation obligations;
- relevant causation/provenance;
- current containment/Kill identity;
- recovery/Controlled Revival state.

If required continuity state cannot be established after failure/restart:

```text
STATE UNKNOWN
-> NEW RISK DENIED
-> RECONCILIATION / PROTECTION ESCALATION
-> NO ASSUMED SAFE CONTINUATION
```

## 4. P1-L Additional Verification Obligations

P1-L SHALL additionally verify when executable implementation exists:

1. a Kill fences queued/pending risk-creating work derived from the killed intelligence;
2. stale pre-Kill decision/evidence epochs cannot dispatch after containment;
3. an action already possibly externalized is reconciled rather than assumed cancelled;
4. protective orders are not blindly cancelled merely because their originating intelligence was killed when an independent valid protection obligation remains;
5. continuity state can be reconstructed after process loss/restart without consulting untrusted killed-AI memory;
6. missing/unreconstructable continuity state fails closed for new risk and escalates protection/reconciliation.

## 5. Review Consequence

The first semantic freeze `e11b2f61290213d6850be17cb0a8de9929b6304a` and its Architecture/Consistency PASS remain historical evidence for that exact earlier state only.

Because this remediation changes semantics, a new exact semantic freeze and fresh Architecture/Consistency + Red-Team cycle are mandatory.

No implementation or runtime authority is granted.