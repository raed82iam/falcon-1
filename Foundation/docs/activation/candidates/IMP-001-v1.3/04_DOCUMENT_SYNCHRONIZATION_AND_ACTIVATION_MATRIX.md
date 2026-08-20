# Document Synchronization and Activation Matrix

**Status:** COMPLETE CANDIDATE SUPPORTING RECORD / PENDING OWNER ACTIVATION  
**Package:** IMP-001 v1.3 successor candidate

## Purpose

Define which governing surfaces must be synchronized before IMP-001 v1.3 can become canonical, which surfaces remain unchanged, and the exact coordinated activation actions.

## Matrix

| Surface | Current treatment | Activation-package disposition |
|---|---|---|
| `IMP-001 v1.2` | Current controlling plan | Preserve historical copy; supersede only by explicit coordinated activation of v1.3 |
| `IMP-001 v1.3 PROPOSED` | Candidate | Ready for final package Red-Team and Owner activation decision |
| Falcon Vision | Supreme authority | No change |
| Falcon Constitution | Binding authority | No change |
| GOV-001 Documentation Authority | Active | No semantic change |
| Specification Tree / SPEC-000 | Active | No semantic change required by Master Plan; planned subjects remain not effective |
| Contract Registry | Active | No immediate semantic change; future Stage Contracts remain prospective |
| ADR Index / relevant ADRs | Active | No immediate new ADR required for plan activation; future realization ADRs remain Stage-specific |
| FRS-001 v1.0 | Active | No meaning change; Stage 10 remains FRS closure point |
| ROADMAP-001 v2.9 | Active current roadmap | Preserve as history; candidate v3.0 prepared for coordinated activation |
| ROADMAP-001 v3.0 PROPOSED | Candidate | Activate with IMP-001 v1.3 if Owner approves package |
| TRC-001 v1.3 | Active current trace | Preserve as history; candidate v1.4 prepared for coordinated activation |
| TRC-001 v1.4 PROPOSED | Candidate | Activate with IMP-001 v1.3 if Owner approves package |
| VPL-BST-* | Historical/bootstrap verification | Preserve; no relabeling |
| VPL-000 / VPL-001..008 | FRS verification family | Preserve meaning; corrected Stage mapping recorded in candidate TRC/VPL impact record |
| Future post-FRS VPLs | Not yet authored | Define prospectively during Stage 11-17 design; absence does not block Master Plan activation because exact requirements are not yet effective |
| AWR-001 v2.1 | Active with stale candidate footer | documentary-only remediation candidate prepared; no requirement meaning change |
| README current-state | Current summary | Update at coordinated activation only; preserve implementation authorization state |
| FCR Issue headers | Shared planning transport | Keep Owner-approved planning targets/review triggers synchronized; open state is not implementation authority or Master Plan blocker |
| Stage 0A-5 closure records | Historical accepted records | No change |
| Stage 6 WP01-04 closure records | Historical accepted records | No change |
| Stage 6 WP05-10 status | Unauthorized future work | Preserve NOT AUTHORIZED |
| Owner planning acceptance record | Accepted planning input | Preserve and reference |

## Exact activation-time synchronization

If the Project Owner approves canonical activation, the activation transaction SHALL:

1. publish/activate the approved `IMP-001 v1.3` canonical successor;
2. preserve `IMP-001 v1.2` historical lineage;
3. publish/activate `ROADMAP-001 v3.0` and preserve v2.9 history;
4. publish/activate `TRC-001 v1.4` and preserve v1.3 history;
5. execute the governed AWR-001 documentary-only consistency correction without changing normative meaning;
6. synchronize README/current-state references to the new controlling plan while keeping Stage 6 WP-05+ and Stage 7+ implementation unauthorized;
7. synchronize any plan/roadmap/trace pointer or registry metadata whose exact current reference becomes stale solely because of the successor versions;
8. preserve FRS-001 and VPL-000 meaning unchanged;
9. preserve all accepted Stage/WP closure records unchanged;
10. record the Owner activation decision and effective documentary instant;
11. perform post-activation repository consistency verification.

## Atomicity rule

Activation SHALL NOT leave a mixed state where:

- IMP uses Stage 0A-17 while ROADMAP remains the historical pre-correction roadmap;
- TRC lacks the corrected Stage mapping;
- README claims a different controlling plan;
- AWR-001 simultaneously claims active and proposed-only status;
- FRS-001 is silently broadened;
- historical closures are relabeled;
- future Stages appear authorized merely because the plan is active.

If coordinated activation cannot be completed safely, rollback SHALL restore previous canonical documentary pointers while preserving failed activation evidence.

## Blocker closure status

- ROADMAP discovery/read: `CLOSED`.
- TRC discovery/read: `CLOSED`.
- ROADMAP successor candidate: `CLOSED / PREPARED`.
- TRC successor candidate: `CLOSED / PREPARED`.
- VPL mapping: `CLOSED`.
- unresolved-matter roadmap disposition: `CLOSED`.
- Contract/ADR/index impact review: `CLOSED`.
- AWR-001 documentary-remediation preparation: `CLOSED / PREPARED`.
- constitutional compliance review: package item 12.
- final whole-package Red-Team: package item 13.

`DOCUMENT_SYNCHRONIZATION_BLOCKERS_OPEN = 0`

`PACKAGE_READY_FOR_FINAL_REVIEW = YES`

Canonical activation still requires explicit Project Owner activation after the final package Red-Team passes.
