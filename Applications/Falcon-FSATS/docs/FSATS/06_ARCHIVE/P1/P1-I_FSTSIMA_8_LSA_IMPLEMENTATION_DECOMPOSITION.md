# P1-I — FSTSimA 8-LSA Implementation Decomposition

**Status:** `DESIGN_CANDIDATE / NOT_OWNER_ACCEPTED / NOT_CLOSED`  
**Implementation Authority:** `NOT_GRANTED`  
**Runtime Authority:** `NOT_GRANTED`  
**Organizational Source:** `04_ACTIVE_WORK/PART_1/01_PART1NG_WORK_PACKAGE_DECOMPOSITION.md`

### Objective
Create the code-ready non-Live simulation and validation Application architecture.

### Required branch coverage
1. S-LSA-01 Simulation Time and Scenario
2. S-LSA-02 Market Environment Simulation
3. S-LSA-03 Provider and External Service Simulation
4. S-LSA-04 Broker, Exchange and Execution Simulation
5. S-LSA-05 Account, Capital and Settlement Simulation
6. S-LSA-06 Fault, Latency and Crisis Injection
7. S-LSA-07 Fidelity and Calibration
8. S-LSA-08 Oracle, Evidence, Reproducibility and Validation Assessment

### Required separation
```text
S-LSA-07 = FIDELITY_MEASUREMENT_AND_CALIBRATION
S-LSA-08 = INDEPENDENT_ASSESSMENT_OF_FIDELITY_AND_OVERALL_VALIDATION_EVIDENCE
```

### FSARM reclaimability requirement
FSTSimA SHALL explicitly declare current minimum-safe resource floor, pause/degradation semantics, reclaimable capacity and restoration rules. Non-live simulation/experimentation capacity may be highly reclaimable when it is not required for an active higher-priority obligation.

Example design intent:

```text
GUARDIAN_CRISIS_NEED
+ FSTSIMA_RECLAIMABLE_CAPACITY
 -> FSARM MAY REDUCE/PAUSE ELIGIBLE FSTSIMA WORK
 -> REALLOCATE EXISTING CAPACITY TO GUARDIAN
```

This is resource control only and does not allow Guardian or FSARM to alter simulation evidence or validation truth.

### Required outputs
Scenario model, deterministic/reproducible time model, simulator interfaces, oracle/evidence model, fidelity/calibration model, fault injection model, replay classification, FSARM resource/degradation/restoration interface and non-Live isolation requirements.

### Closure criteria
Simulation evidence cannot become Live authority, promotion authority or hidden operational traffic, and resource reclamation cannot corrupt accepted evidence truth.
