# FSATS Part 9 — Scope Reconciliation and Implementation Plan

**Date:** `2026-08-16`  
**Branch:** `application-development`  
**Owner authorization:** explicit authorization to complete Part 9 through executable test  
**Part 8:** `OWNER_ACCEPTED_AND_CLOSED`  
**Part 9 status:** `OWNER_AUTHORIZED / IMPLEMENTATION IN PROGRESS`  
**Part 10:** `NOT_AUTHORIZED`

## 1. Reconciled Part 9 meaning

The historical future-execution inventory identifies Part 9 as the independent FSTSimA / Digital City validation scope. Current accepted source already contains the FSTSimA Application shell, awareness topology, deterministic clock/PRNG, synthetic market generation, broker simulation, fault injection, calibration, validation assessment, durable recovery and non-Live isolation behavior from earlier accepted Parts.

Part 9 therefore SHALL NOT rebuild FSTSimA or silently reopen accepted Parts. Its current delta is to complete a governed Digital City validation layer over the accepted FSTSimA primitives and verify that simulation evidence remains non-operational and non-authoritative.

## 2. Authorized implementation delta

Part 9 completes:

- governed Digital City scenario identity and exact simulation scope binding;
- deterministic scenario execution from explicit seed, tick count, start price and regime;
- deterministic fault ordering independent of caller collection order;
- SHA-256 digest binding across scenario identity, scope, synthetic market output and ordered faults;
- exact evidence identity bound to scope, scenario, seed and digest;
- reproducibility assessment using repeated deterministic execution;
- independent-calibration evidence requirement for qualification recommendation;
- explicit distinction between qualification recommendation and Paper authority;
- explicit permanent false values for operational truth, runtime authority, Paper authority and Live authority in the Part 9 result;
- adversarial verification integrated into the existing FSATS Behavior verifier through a module initializer.

## 3. Existing capability reuse

Part 9 reuses rather than duplicates:

- `SyntheticMarketGenerator`;
- `FaultInjector`;
- `ValidationAssessor`;
- `ISimulationEvidenceSink`;
- existing FSTSimA scope identity and manifest boundary.

No new Falcon Application is created.

## 4. Authority boundaries

```text
DIGITAL_CITY_RESULT != OPERATIONAL_TRUTH
SIMULATION_SUCCESS != RUNTIME_AUTHORITY
QUALIFICATION_RECOMMENDATION != PAPER_AUTHORITY
PAPER_QUALIFICATION_REVIEW != PAPER_ACTIVATION
SIMULATION != LIVE
FSTSIMA != TRADING
FSTSIMA != BROKER_TRUTH
FSTSIMA != PROVIDER_OPERATIONAL_TRUTH
PART9 != DEPLOYMENT_AUTHORITY
```

Part 9 does not grant:

- Application runtime activation;
- provider connectivity;
- broker connectivity or execution;
- Paper activation;
- Shadow activation;
- Tiny-Live or Live activation;
- deployment;
- production adoption;
- final runtime binding for open FCRs.

## 5. FCR reconciliation

FCR-0011 remains open with `Waiting On: APPLICATION`, but its current issue body requires separately authorized final runtime/binding verification against the accepted Stage 12 non-Live egress boundary. Part 9 does not infer that runtime-binding authority from the FCR.

FCR-0224/FCR-0226 and MSA/FSA binding FCRs remain separately governed and are not completed by Digital City implementation.

## 6. Implementation checkpoints

- Digital City coordinator implementation: `ac9a8c2a2c503cc3962d671ada8891ef335f5f54`
- Part 9 adversarial checks: `325e401adacb1dc876bd1b3a84c2c6faea105be2`
- Part 9 verifier bootstrap: `6b7a93e0f2e28156b9e86a81bd79bce2859e644d`
- deterministic fault-order hardening: `d6bdc1ef546a53fe374cc3ab14a7ec4bf096dd7c`
- FSTSimA current governed-state synchronization: `687862aff054f15385706062f720c139d9b63474`

## 7. Exit criteria before Owner acceptance

Part 9 cannot be presented for final Owner acceptance until:

1. exact executable source is fixed;
2. isolated restore/build succeeds;
3. FSATS automated tests/verifiers succeed;
4. Architecture succeeds;
5. Security succeeds;
6. Behavior verifier including Part 9 adversarial checks succeeds;
7. deterministic rerun succeeds;
8. post-executable Architecture/Consistency review succeeds;
9. post-executable broad Red Team has no unresolved blocking finding;
10. exact test evidence is recorded.

Until those exit criteria are satisfied:

```text
PART9_IMPLEMENTATION = PRESENT
PART9_EXECUTABLE_VALIDATION = PENDING
PART9_OWNER_ACCEPTED_AND_CLOSED = NO
```
