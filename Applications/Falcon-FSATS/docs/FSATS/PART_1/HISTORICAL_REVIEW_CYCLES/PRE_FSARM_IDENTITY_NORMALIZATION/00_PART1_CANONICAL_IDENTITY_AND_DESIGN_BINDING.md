# FSATS Part 1 — Canonical Identity and Design Binding

**Status:** `DESIGN_CANDIDATE / NOT_OWNER_ACCEPTED / NOT_CLOSED`  
**Branch:** `application-development`  
**Authority Type:** `DESIGN ONLY`  
**Implementation Authority:** `NOT GRANTED`  
**Runtime Authority:** `NOT GRANTED`  
**Canonical Current Identity:** `PART 1`  
**Historical Archived Identity:** `Historical Part 1`  

## 1. Owner Identity Direction

The Project Owner selected the simpler current identity `Part 1` for the new implementation-architecture and build-readiness design.

The earlier current-work candidate label `Part 1-NG` was provisional only and was never Owner-accepted or closed.

The older Owner-closed implementation package preserved in archive remains historical evidence and SHALL be referred to as `Historical Part 1`. It is not the current build baseline.

No historical record is rewritten.

## 2. Exact Semantic Successor Binding

The current Part 1 design incorporates the semantic content frozen at:

`359b157fa82a1b489b6501ae9a5ae83887210237`

from these four design files:

1. `PART_1_NG/00_PART1NG_MASTER_DESIGN_AND_SCOPE.md`
2. `PART_1_NG/01_PART1NG_WORK_PACKAGE_DECOMPOSITION.md`
3. `PART_1_NG/02_PART1NG_DEPENDENCY_FCR_AND_PARALLELIZATION_MODEL.md`
4. `PART_1_NG/03_PART1NG_PART0_TRACEABILITY_AND_COMPLETENESS_REGISTER.md`

Those frozen semantics remain the design substance except for the exact identity normalization defined in this record.

## 3. Exact Identity Normalization

The only intended semantic change is:

```text
PART 1-NG  -> PART 1
Part 1-NG  -> Part 1
P1NG-A     -> P1-A
P1NG-B     -> P1-B
P1NG-C     -> P1-C
P1NG-D     -> P1-D
P1NG-E     -> P1-E
P1NG-F     -> P1-F
P1NG-G     -> P1-G
P1NG-H     -> P1-H
P1NG-I     -> P1-I
P1NG-J     -> P1-J
P1NG-K     -> P1-K
P1NG-L     -> P1-L
PART1NG_DESIGN_CLOSED -> PART1_DESIGN_CLOSED
```

No responsibility, ownership, topology, dependency, contract, FCR, security, performance, readiness, authority or non-authority meaning is changed by this normalization.

## 4. Canonical Part 1 Work Packages

```text
P1-A  Authority / Baseline / Historical Compatibility / Scope Lock
P1-B  Repository / Solution / Project / Package Topology
P1-C  Canonical Application-Owned Primitives
P1-D  Application Identity / Manifest / Lifecycle Materialization
P1-E  Trading 13-LSA + TARC Decomposition
P1-F  FSAPMA 6-LSA Decomposition
P1-G  Guardian 4-LSA Decomposition
P1-H  FSTSimA 8-LSA Decomposition
P1-I  Exact 43-Contract Schema/Event/Route Declaration Materialization
P1-J  Foundation Binding / FCR / Fail-Closed Consumption Plan
P1-K  Verification / Security / Failure / Performance Architecture
P1-L  Integrated Build DAG / Parallelization / Implementation-Readiness Gate
```

The WP count is derived from the current responsibility decomposition and is not fixed governance.

## 5. FCR Operating Rule

Part 1 does not wait for Foundation capabilities that are not required for design work that can safely proceed.

For every FCR:

- Application SHALL read and acknowledge substantive Foundation information;
- if the remaining action is Foundation-owned, Application SHALL return the handoff to Foundation;
- if the Application-side answer is expected in a future Part/WP, Application SHALL identify that expected Part/WP and request Foundation acknowledgement of the mapping;
- unavailable Foundation capability remains explicitly `FAIL_CLOSED` for affected build/runtime slices;
- unrelated Part 1 design work continues independently;
- `ACCEPTED_FOR_PLANNING != IMPLEMENTED != AVAILABLE_FOR_RUNTIME`.

## 6. Historical Compatibility Rule

Historical Part 1 artifacts may be reused only artifact-by-artifact after fresh proof of current semantic, topology, contract, Foundation, authority and security compatibility.

```text
HISTORICAL_PASS != CURRENT_COMPATIBILITY
REUSE_REQUIRES_FRESH_PROOF
```

## 7. Current Design Lifecycle

Because the identity normalization is a semantic change, the prior Architecture/Consistency and Red-Team results are historical review evidence only for the predecessor candidate.

The current Part 1 candidate SHALL receive:

```text
CURRENT SUCCESSOR SEMANTIC FREEZE
-> FRESH ARCHITECTURE / CONSISTENCY REVIEW
-> FRESH RED TEAM
-> OWNER REVIEW
-> EXPLICIT OWNER ACCEPTANCE / CLOSURE
```

No current Part 1 acceptance or closure is claimed by this record.

## 8. Non-Grant

This design work does not grant:

- implementation;
- runtime activation;
- external provider connectivity;
- broker connectivity;
- credential use;
- Paper, Shadow, Tiny Live or Live operation;
- deployment;
- autonomous promotion.

`PART1_DESIGN_READY != IMPLEMENTATION_AUTHORIZED`.
