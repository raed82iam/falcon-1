# AI Repair / Controlled Recovery Bounded Auto-Recovery Hardening V3

**Status:** `CONTROLLING HARDENING CANDIDATE / NOT_OWNER_ACCEPTED / NOT_CLOSED`

This record supplements records 09, 12 and 13 and remediates Red-Team V2 HIGH-01.

## 1. R1 Automatic Recovery Must Be Bounded

R1 automatic restoration SHALL operate only inside an explicit governed attempt envelope.

The future exact policy shall define as applicable:

- maximum automatic attempts;
- recurrence window;
- same/similar-fault correlation rule;
- minimum probation/observation requirements;
- escalation threshold;
- cooldown/backoff where appropriate;
- evidence retained per attempt;
- conditions that immediately prohibit further R1 attempts.

This design does not guess the final numeric limits.

## 2. Recurrence Escalation

Repeated recurrence of the same or materially similar fault SHALL NOT be treated as a sequence of unrelated fresh R1 incidents.

```text
REPEATED / CORRELATED R1 FAILURE
-> EXIT R1
-> DEEPER INVESTIGATION
-> R2 OR R3 AS MATERIALITY REQUIRES
```

Failed probation counts as recovery failure rather than proof that the previous repair succeeded.

## 3. Attempt History Is Trust-Bearing State

Automatic-recovery attempt history SHALL be attributable, reconstructable and outside the sole control of the affected intelligent subject.

Restart/reload SHALL NOT reset the attempt count or erase recurrence evidence.

```text
PROCESS RESTART != RECOVERY-ATTEMPT HISTORY RESET
```

## 4. Common-Cause / Blast-Radius Rule

Repeated faults may indicate a broader dependency, shared-state, model, configuration, security or architecture cause.

When evidence suggests common-cause damage, containment and investigation scope SHALL expand according to the existing trust-blast-radius rule rather than repeatedly repairing the first visible component.

## 5. Safety Continuity During Exhaustion/Escalation

When the R1 automatic envelope is exhausted:

- affected intelligent authority remains contained/untrusted;
- no new risk may be created through the affected scope;
- existing financial/operational obligations remain protected through independently trusted Safety Continuity paths;
- escalation to R2/R3 occurs without blind revival;
- Owner receives the accumulated incident/recovery evidence when Owner decision is required.

## 6. No Self-Reset of Recovery Policy

The affected subject SHALL NOT modify, reset, widen or reinterpret its own recovery-attempt envelope to remain eligible for R1.

Any attempt to interfere with recovery accounting, containment, evidence or escalation is an integrity event and may require R3 handling.

## 7. Non-Grant

No numeric retry policy, runtime automatic-repair authority, implementation or deployment authority is granted by this candidate.
