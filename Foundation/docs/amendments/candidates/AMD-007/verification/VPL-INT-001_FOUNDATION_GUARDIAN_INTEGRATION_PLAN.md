# VPL-INT-001 — Foundation and Guardian Integration Plan

**Status:** Proposed — Execution Not Authorized

## Scenarios

1. Foundation with no Applications remains complete.
2. Trading Suite only.
3. Accounting Suite only without Foundation redesign.
4. Trading and Accounting coexist.
5. Accounting resource/traffic fault affects Trading.
6. Trading Guardian submits CON-022 isolation request.
7. FFG rejects unsupported request.
8. FFG chooses narrower containment.
9. FFG chooses stronger Platform protection.
10. FSA repairs Foundation damage from Approved state.
11. FSA creates isolated improved candidate.
12. Owner rejects candidate and nothing activates.
13. Owner approves candidate for canary only.
14. canary failure triggers rollback.
15. required Application Guardian unavailable blocks full Suite activation.
16. FFG compromised and independently isolated.
17. FSA compromised and restricted without self-validation.
18. restart/failover during active Platform and domain restrictions.
19. Trading Suite removal leaves Foundation complete.
20. future Application Guardian registers without FFG/FSA redesign.

## Mandatory Assertions

- no Foundation business payload access;
- no Application-specific branch in Foundation;
- technical criticality requires approved admission;
- request, decision, directive, execution, recovery, and release remain separate;
- FIL/Service Bus enforce identity, authority, integrity, expiry, replay, rate, dead-letter, priority, acknowledgment, and evidence;
- restrictions persist before normal authority returns;
- Platform normality does not establish domain normality.

## Evidence

Each scenario requires a complete immutable Evidence Set containing obligations, environment, identities, Manifests, authorities, messages, independent observations, decisions, execution results, state transitions, persistence/restart, releases, challenges, and payload-exclusion proof.

Approval of this plan does not authorize execution.

