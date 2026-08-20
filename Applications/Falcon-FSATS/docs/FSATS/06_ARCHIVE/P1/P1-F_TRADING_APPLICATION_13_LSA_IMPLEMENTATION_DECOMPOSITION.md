# P1-F — Trading Application 13-LSA Implementation Decomposition

**Status:** `DESIGN_CANDIDATE / NOT_OWNER_ACCEPTED / NOT_CLOSED`  
**Implementation Authority:** `NOT_GRANTED`  
**Runtime Authority:** `NOT_GRANTED`  
**Organizational Source:** `04_ACTIVE_WORK/PART_1/01_PART1NG_WORK_PACKAGE_DECOMPOSITION.md`

### Objective
Create a code-ready decomposition of the Falcon Self-Aware Trading Application while preserving exact Trading ownership and the new FSARM separation.

### Required branch coverage
1. T-LSA-01 Operations, Account & Environment
2. T-LSA-02 Market & Instrument Universe
3. T-LSA-03 Analysis Frameworks
4. T-LSA-04 Classical Trading School
5. T-LSA-05 Opportunity Hunting School
6. T-LSA-06 Strategy Orchestration & Decision
7. T-LSA-07 Unified Risk Management
8. T-LSA-08 Portfolio & Capital Management
9. T-LSA-09 Execution & Position Lifecycle
10. T-LSA-10 Trading Learning & Knowledge
11. T-LSA-11 Trading Analytics & Attribution
12. T-LSA-12 Strategy Evolution & Experimentation
13. T-LSA-13 Trading Resource Management

### Resource separation

```text
T_LSA13 = TRADING_RESOURCE_AWARENESS_AND_EVALUATION
T_LSA13 != FSARM
```

T-LSA-13 SHALL understand Trading-side current allocation, demand, pressure, minimum-safe requirement, reclaimability, shedding effects and additional need evidence, and shall report attributable Trading resource evidence to FSARM.

Trading MSA/LSAs/CSAs/components SHALL NOT independently bypass FSARM to request FSATS resource reallocation or additional Foundation resources.

### Required outputs
For every LSA: components, state ownership, internal interfaces, data inputs/outputs, concurrency model, failure/degraded behavior, security boundary, resource profile, tests and Foundation dependencies, plus exact Trading-to-FSARM evidence/decision interfaces.

### Closure criteria
No Trading responsibility is orphaned, duplicated, pushed into Guardian/FSAPMA/Foundation, or hidden behind a generic “Trading Engine” owner; T-LSA-13 remains awareness/evaluation while FSARM owns FSATS-wide operational resource coordination.
