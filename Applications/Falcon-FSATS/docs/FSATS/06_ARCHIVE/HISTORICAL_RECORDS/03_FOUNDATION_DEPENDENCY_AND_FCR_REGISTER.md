# FSATS V1.4 PROPOSED - Foundation Dependency and FCR Register

## Status

**Status:** `PART 0 ALIGNMENT REGISTER / OWNER REVIEW REQUIRED`  
**Authority:** design only; no Foundation modification or runtime authority.

## Canonical FCR channel

Repository FCR workflow is governed by `applications/FCR_WORKFLOW.md` and GitHub Issue #1 (`FCR Shared Registry and Operating Protocol`).

Every confirmed Foundation-impacting gap is raised as its own GitHub Issue. The canonical FCR ID is derived from the GitHub issue number. Application-side markdown files under `applications/_foundation-capability-requests/` that used manually allocated `FCR-0001..0008` identifiers predate the shared registry and are **legacy local draft evidence only**. They SHALL NOT be used as canonical FCR identities.

## Classification rule

For the shared GitHub channel, confirmed gaps are classified only as:

- `MISSING`;
- `PARTIAL`;
- `INCOMPATIBLE`.

A planned-but-not-yet-available Foundation capability is represented as `PARTIAL` when a governing boundary exists but the required concrete contract/runtime behavior is not yet available or confirmed, and as `MISSING` when no suitable generic capability has been identified.

## Part 0 dependency register

| ID | Foundation dependency | Current classification | V1.4 treatment | Canonical FCR |
|---|---|---|---|---|
| FD-001 | Application boundary/lifecycle | AVAILABLE | APP-001 governs each independent Application | none |
| FD-002 | Application Manifest semantics | AVAILABLE | CON-023 governs declarations | none |
| FD-003 | Awareness ownership/alignment | AVAILABLE | ADR-I015 governs MSA/LSA/CSA/FSA placement | none |
| FD-004 | Plug-and-Play integration boundary | AVAILABLE | ADR-I012 governs Foundation/Application neutrality | none |
| FD-005 | Per-Application technical resource governance | AVAILABLE with concrete interface gaps | SYS-006; no FSATS-wide technical pool | FCR-0007 / FCR-0010 |
| FD-006 | Canonical communication/FIL design baseline | AVAILABLE as governing design dependency | preserve declared semantics; do not overclaim runtime availability | none unless a specific gap appears |
| FD-007 | Schema governance/registry design baseline | AVAILABLE as governing design dependency | preserve V1.3 schema intent and bind current registry/version rules | none unless a specific gap appears |
| FD-008 | Application communication manifest runtime integration | PARTIAL | dependent runtime integration blocked | FCR-0004 / FCR-0005 / FCR-0006 / FCR-0009 as applicable |
| FD-009 | Governed cross-Application routing | PARTIAL | declare route families/failure behavior now | FCR-0004 / FCR-0005 / FCR-0006 / FCR-0009 |
| FD-010 | Dynamic Service Bus routing/runtime behavior | PARTIAL | do not implement local Foundation substitute | FCR-0004 / FCR-0005 / FCR-0009 |
| FD-011 | Delivery semantics: retry/idempotency/duplicate/order/correction/flow | PARTIAL | preserve V1.3 contract requirements | FCR-0004 / FCR-0005 / FCR-0006 / FCR-0009 |
| FD-012 | Event publication/journal/replay transport | PARTIAL | preserve reconstructability/replay isolation | FCR-0006 |
| FD-013 | Communication cryptographic protection | PARTIAL / dependent on Foundation disposition | specify security outcomes, not Foundation implementation | carried by relevant FCRs; add more if a concrete gap appears |
| FD-014 | Dynamic route attach/drain/detach lifecycle | PARTIAL / not yet confirmed | specify safe lifecycle expectations | update relevant canonical FCR when detailed route lifecycle requires it |
| FD-015 | Trading Guardian to Foundation Guardian/resource escalation | PARTIAL | Guardian requests; Foundation approves/denies/allocates | FCR-0007 |
| FD-016 | Governed research-only Internet egress for MSA/LSA/eligible CSA | PARTIAL | preserve V1.3 operational-vs-research separation | FCR-0008 |
| FD-017 | Latency/deadline/QoS-aware Application transport | MISSING | preserve V1.3 Fast Track across Application boundaries without safety bypass | FCR-0009 |
| FD-018 | Application resource-pressure visibility/load-shedding signals | PARTIAL | each Application sheds only inside own allocation | FCR-0010 |
| FD-019 | Non-Live Application isolation, permission separation and egress guard for FSTSimA | PARTIAL | preserve independent simulator with enforceable denial of Live credentials/routes/endpoints | FCR-0011 |

## Canonical submitted FCRs

1. **FCR-0004 / GitHub Issue #4 - Falcon Trading Guardian - governed protection command route**  
   Governed Guardian to Trading/FSAPMA scoped protection route.

2. **FCR-0005 / GitHub Issue #5 - FSAPMA - operational market-data delivery contract**  
   Normalized operational trading-data delivery from FSAPMA to authorized consumers.

3. **FCR-0006 / GitHub Issue #6 - FSATS Applications - event evidence and replay delivery**  
   Reconstructability, causation/correlation, replay-safe evidence and simulation evidence transport.

4. **FCR-0007 / GitHub Issue #7 - Falcon Trading Guardian - Foundation resource escalation request boundary**  
   Evidenced resource request while Foundation retains all allocation authority.

5. **FCR-0008 / GitHub Issue #8 - Falcon Awareness - research-only Internet egress boundary**  
   Governed research/learning/development egress separated from operational trading data.

6. **FCR-0009 / GitHub Issue #9 - FSATS Applications - latency deadline and QoS aware transport**  
   Deadline propagation, bounded overload behavior and observable tail latency required to preserve V1.3 Fast Track across Application boundaries.

7. **FCR-0010 / GitHub Issue #10 - FSATS Applications - resource pressure and load-shedding signals**  
   Own-allocation pressure visibility and request outcomes needed for deterministic Application-owned degradation/load shedding.

8. **FCR-0011 / GitHub Issue #11 - FSTSimA - enforce non-Live isolation and egress guard**  
   Enforceable separation of FSTSimA from Live credentials/routes/endpoints while preserving authorized replay/simulation inputs.

All eight are currently `SUBMITTED`. Foundation triage/disposition belongs to the Foundation workstream through the shared GitHub channel.

## FCR disposition rule

An FCR Issue is a request/evidence channel only. It does not assert Foundation failure, prescribe Foundation implementation, grant Foundation modification authority, or grant Application implementation/deployment/Paper/Tiny Live/Live authority.

Foundation may respond according to the lifecycle in Issue #1. Application verification is required before an implementation-required FCR can close.

## Blocking policy

A Foundation gap blocks only the dependent future runtime integration/claim unless explicitly classified otherwise. It does not block unrelated V1.4 design work.

FSATS SHALL NOT weaken a preserved V1.3 safety/performance requirement merely to avoid raising an FCR.
