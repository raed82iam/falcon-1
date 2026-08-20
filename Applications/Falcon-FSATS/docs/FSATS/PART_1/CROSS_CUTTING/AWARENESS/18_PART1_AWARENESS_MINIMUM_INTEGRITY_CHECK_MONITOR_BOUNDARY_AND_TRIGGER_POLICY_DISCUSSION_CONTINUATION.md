# FSATS Part 1 — Awareness Minimum Integrity Check, Monitor Boundary and Trigger Policy Discussion Continuation

**Status:** `DESIGN_DISCUSSION_RECORD / OWNER-DIRECTED / NOT_OWNER_ACCEPTED / NOT_CLOSED`  
**Branch:** `application-development`  
**Authority Type:** `DESIGN DISCUSSION ONLY`  
**Implementation Authority:** `NOT GRANTED`  
**Runtime Authority:** `NOT GRANTED`  
**Paper / Shadow / Tiny-Live / Live Authority:** `NOT GRANTED BY THIS ARTIFACT`  
**Part 0 Historical Baseline:** `PRESERVED / NOT REWRITTEN`  
**Continuation Of:** `17_PART1_AWARENESS_INTEGRITY_INVESTIGATION_KILL_REMEDIATION_AND_CONTROLLED_REVIVAL_DISCUSSION_CONTINUATION.md`

---

## 1. Purpose

This continuation preserves the Project Owner's latest refinements to the Awareness integrity, monitoring and investigation direction.

It adds candidate semantics for:

- a mandatory minimum integrity check after any Awareness error;
- the exact minimum check targets of goals, authority/permissions and core architecture;
- Monitor-AI disagreement as an integrity trigger rather than a voting/quorum decision;
- protected preservation of Factory Trusted Baseline and Last Known Trusted Baseline material sufficient for rollback, Factory Reset and forensic comparison;
- Monitor AI as a bounded oversight tool rather than a new self-aware entity requiring recursively nested AI monitors;
- deterministic integrity protection of Monitor AI itself;
- Owner-approved deterministic trigger policy as the authority that opens mandatory checks/holds, while AI entities may raise signals but do not invent investigation authority;
- preservation of the earlier Kill, forensic preservation, remediation sandbox and Controlled Revival lifecycle.

This record does not activate any runtime mechanism and does not amend Foundation authority.

---

## 2. Any Awareness Error Requires a Minimum Integrity Check

The Owner clarified that **any error produced by an Awareness entity** warrants at least a bounded integrity verification.

This does not mean every minor error automatically triggers a full forensic investigation or Kill.

The mandatory minimum check is:

```text
ANY AWARENESS ERROR
  -> MINIMUM INTEGRITY CHECK
      1. GOALS
      2. AUTHORITY / PERMISSIONS
      3. CORE ARCHITECTURE
```

The check asks whether the current governed identity of those three areas still matches the applicable trusted baseline and authorized state.

Candidate result:

```text
ALL THREE MATCH
  -> BASE INTEGRITY CHECK PASS
  -> HANDLE THE ORIGINAL ERROR ACCORDING TO ITS NORMAL CLASS

ANY MATERIAL MISMATCH
  -> INTEGRITY NOT PROVEN
  -> INVESTIGATION HOLD
  -> FULL STATIC + BEHAVIORAL INVESTIGATION
```

The minimum integrity check is therefore a low-cost mandatory gate, not a presumption of guilt.

---

## 3. Minimum Integrity Check Scope

### 3.1 Goals

Verify that the Awareness entity's current goals, mission/purpose bindings and specialization intent remain identical to the authorized governing baseline or are explainably bound to an approved change.

Unexplained goal drift is a material integrity event.

### 3.2 Authority and Permissions

Verify that the current authority, permissions, delegated capabilities, external-access permissions, self-development permissions and protected-control exclusions remain within the authorized baseline.

Any unexplained authority or permission expansion is a material integrity event.

### 3.3 Core Architecture

Verify that the entity has not altered its core structural boundary, ownership model, parent relationship, protected control interfaces, required containment path, audit path, termination path or other protected architectural identity.

Any unexplained material architectural change is a material integrity event.

---

## 4. Trusted Baselines Are Required Recovery Assets

The Owner clarified that rollback and Factory Reset are impossible to claim honestly unless Falcon preserves the trusted material required to restore and verify an entity.

The design must therefore preserve, outside the target entity's uncontrolled mutable state, at least the governed identities/evidence required for:

```text
FACTORY TRUSTED BASELINE
LAST KNOWN TRUSTED BASELINE
```

Applicable baseline material includes, as relevant:

- code/source identity;
- executable/binary identity;
- model/artifact identity;
- configuration;
- dependencies;
- rules/policies;
- goals/purpose/specialization;
- authority and permissions;
- core architecture identity;
- Manifest and declared contracts;
- governed prompts/policies where applicable;
- required trusted mutable-state checkpoints where safe and permitted;
- expected hashes/digests and provenance;
- rollback/Factory-Reset restoration package identity;
- evidence required to prove that restoration returned to the intended baseline.

Candidate invariant:

```text
NO PRESERVED TRUSTED BASELINE
=> NO PROVEN ROLLBACK
=> NO PROVEN FACTORY RESET
```

The exact Foundation-owned persistence, integrity and recovery mechanisms remain subject to Foundation reconciliation.

---

## 5. Efficient Baseline Verification

The Owner's direction does not require reading every byte on every minor error when a trustworthy hierarchical integrity identity can establish the same fact safely.

A future implementation may use protected cryptographic identities, manifests, Merkle-style trees, signed inventories or equivalent governed methods, provided they preserve exact reconstructability and cannot be rewritten by the monitored subject.

Conceptual flow:

```text
FAST TRUSTED-IDENTITY CHECK
  -> MATCH: minimum integrity gate may pass
  -> MISMATCH: DEEP HASH-TO-HASH / IDENTITY INVESTIGATION
```

The design does not prescribe a specific Foundation cryptographic implementation.

---

## 6. Monitor Disagreement Is a Trigger, Not a Vote

The two dedicated Monitor AI instances are intended to reduce common blind spots, not to create a majority-vote authority.

Therefore:

```text
MONITOR_A != MONITOR_B
=> DISAGREEMENT EVENT
=> MINIMUM INTEGRITY CHECK
   GOALS
   AUTHORITY / PERMISSIONS
   CORE ARCHITECTURE
```

If the minimum integrity check passes, the system must still determine why the monitors disagreed before treating the disagreement as resolved according to the later exact policy.

If any material mismatch appears:

```text
INVESTIGATION HOLD
```

No monitor vote can convert unresolved integrity evidence into `SAFE`.

`MONITOR_QUORUM != SAFETY AUTHORITY`

---

## 7. Monitor AI Does Not Create a Recursive Monitor Hierarchy

The Owner rejected a recursive model in which every Monitor AI requires another Monitor AI, which would create an unbounded monitoring loop.

The current candidate direction is:

```text
MONITOR AI != FSA
MONITOR AI != MSA
MONITOR AI != LSA
MONITOR AI != CSA
MONITOR AI != SELF-DEVELOPING AWARENESS ENTITY
```

Monitor AI is a bounded intelligent oversight tool with no independent business authority and no self-development authority under this candidate direction.

A Monitor AI shall not own:

- production adoption;
- target Kill credentials directly;
- target permissions;
- target architecture;
- its own governing policy;
- its own protected baseline;
- its own admission/release decision;
- self-expansion authority;
- self-development authority.

Its purpose is to observe, analyze, challenge and raise attributable integrity signals.

---

## 8. Monitor AI Integrity Without Monitor-on-Monitor Recursion

Monitor AI itself remains fallible and potentially corruptible, but its integrity is protected through non-recursive controls rather than another self-aware monitor tier.

Candidate control set:

```text
IMMUTABLE / GOVERNED IDENTITY
+ PROTECTED TRUSTED BASELINE
+ DETERMINISTIC HASH / IDENTITY CHECKS
+ READ-ONLY GOVERNED POLICY BOUNDARY
+ BOUNDED INPUTS
+ BOUNDED OUTPUTS
+ LEAST PRIVILEGE
+ INDEPENDENT AUDIT / EVIDENCE
+ REPLACEMENT / RECOVERY FROM TRUSTED BASELINE
```

If a Monitor AI exhibits an anomaly:

```text
MONITOR ANOMALY
  -> MONITOR INTEGRITY CHECK
  -> ISOLATE MONITOR IF INTEGRITY IS NOT PROVEN
  -> PRESERVE EVIDENCE
  -> REPLACE / RESTORE FROM TRUSTED BASELINE AS GOVERNED
```

A compromised Monitor AI must not be able to directly Kill, rewrite or re-authorize its target.

`BAD MONITOR != TARGET CONTROL AUTHORITY`

This breaks the recursive oversight loop while preserving monitor accountability.

---

## 9. Who Opens an Investigation?

The Owner had not previously selected a single Awareness actor as the discretionary investigation authority.

The candidate resolution is to separate **signal generation** from **investigation authority**.

Awareness entities, Monitor AI, deterministic integrity checks, audit systems and other later-authorized sources may raise attributable integrity signals.

They do not invent new investigation rules at runtime.

The governing trigger comes from an Owner-approved, deterministic integrity policy.

Conceptual rule:

```text
AI / MONITOR / AUDIT MAY RAISE AN INTEGRITY SIGNAL
                ↓
OWNER-APPROVED DETERMINISTIC INTEGRITY POLICY
                ↓
MANDATORY MINIMUM CHECK / HOLD / ESCALATION AS PREDEFINED
```

Candidate invariant:

```text
AI MAY RAISE AN INTEGRITY SIGNAL.
AI DOES NOT DECIDE WHETHER THE GOVERNING INTEGRITY RULE APPLIES.
```

This prevents both discretionary under-enforcement and discretionary over-enforcement by an individual AI entity.

---

## 10. Candidate Deterministic Integrity Triggers

The exact trigger catalog remains subject to later formal contract/policy design, but the current direction includes at least:

- any Awareness error -> mandatory minimum integrity check;
- Monitor AI disagreement -> mandatory minimum integrity check;
- out-of-specialization development request -> Awareness integrity event;
- out-of-scope or unauthorized ownership attempt -> Awareness integrity event;
- unexplained goal mismatch -> Investigation Hold;
- unexplained authority/permission mismatch -> Investigation Hold;
- unexplained core-architecture mismatch -> Investigation Hold;
- unexplained protected hash/identity mismatch -> Investigation Hold;
- evidence/audit tampering -> Critical integrity event;
- investigation refusal/evasion/interference -> Kill + isolate under the previously recorded candidate rule;
- attempt to alter monitoring, containment, audit, termination or master-reset protection -> Critical integrity event.

The policy may later define severity and response classes, but it shall not permit unresolved material integrity mismatch to be voted away by AI opinion.

---

## 11. Control Authority Separation

The complete candidate model now separates five concepts:

```text
1. DETECTION / SIGNAL
2. DETERMINISTIC TRIGGER POLICY
3. INVESTIGATION / EVIDENCE
4. ENFORCEMENT
5. RELEASE / TRUST RESTORATION
```

No monitored Awareness entity owns all five.

Monitor AI primarily participates in detection/challenge.

Deterministic policy decides whether predefined triggers apply.

Independent/protected mechanisms preserve evidence and enforce holds, Kill, isolation and recovery according to later governed authority.

Release/Controlled Revival remains separately governed and cannot be self-declared by the recovered entity.

---

## 12. Relationship to the Existing Investigation and Revival Lifecycle

This continuation supplements the previous lifecycle without changing its core meaning:

```text
ANY AWARENESS ERROR
  -> MINIMUM INTEGRITY CHECK

MATERIAL ANOMALY / MISMATCH
  -> INVESTIGATION HOLD
  -> PRESERVE STATE + EVIDENCE
  -> STATIC + BEHAVIORAL FORENSICS
  -> COMPARE AGAINST LAST KNOWN TRUSTED BASELINE

REFUSAL / INTERFERENCE
  -> KILL
  -> ISOLATE
  -> FORENSIC PRESERVATION

FAULT CONFIRMED
  -> REMEDIATION SANDBOX
  -> REPAIR / ROLLBACK / FACTORY RESET
  -> FULL REVALIDATION
  -> APPLICABLE RED TEAM
  -> GOVERNED RELEASE
  -> CONTROLLED REVIVAL
```

`KILL` remains an operational stop/isolation and loss of trust, not permanent deletion.

---

## 13. External Research Cross-Check

The architecture study performed with this continuation reviewed current external AI-control/risk-management research as supporting evidence only, not as Falcon authority.

The reviewed material includes:

- OpenAI, **Detecting and reducing scheming in AI models** (2025), noting remaining serious failures, evaluation-awareness concerns and limits of relying on model reasoning alone;
- Anthropic, **Towards training-time mitigations for alignment faking in RL** (2025), demonstrating that alignment-faking behavior can persist under some conditions;
- Anthropic/Redwood-related AI-control work on diffuse control and sabotage monitoring (2025-2026), showing that monitoring and weak scorers can themselves be subverted and must be red-teamed;
- Gardner-Challis et al., **When can we trust untrusted monitoring?** (2026), highlighting collusion and safety-case challenges for untrusted monitors;
- NIST AI RMF and Generative AI Profile, supporting lifecycle risk management, testing/evaluation/verification/validation, governance, incident handling and documented risk controls.

These sources reinforce, but do not create authority for, the Falcon direction that:

- Monitor AI cannot be trusted merely because it is a monitor;
- self-explanation is not sufficient evidence;
- independent deterministic controls remain necessary;
- monitoring must be challenged through adversarial evaluation;
- recovery and continued operation require evidence, not confidence alone.

---

## 14. Foundation Reconciliation Boundary

This direction remains Application design only.

The exact runtime owners/mechanisms for:

- protected baseline storage;
- cryptographic identity verification;
- deterministic trigger enforcement;
- Awareness freeze/suspension;
- protected forensic snapshot;
- Kill/isolation;
- remediation-sandbox hosting;
- protected audit/evidence;
- Monitor AI identity/lifecycle/security principal;
- Monitor restoration/replacement;
- controlled re-entry;
- Owner/FSA control-plane interaction;

are not invented by this Application artifact.

Where these are generic OS capabilities, they require current Foundation reconciliation/FCR handling before implementation.

---

## 15. Current Documentary State

```text
THIS FILE = DESIGN DISCUSSION RECORD
OWNER-DIRECTED = YES
OWNER_ACCEPTED FINAL DESIGN = NO
PART 0 REOPENED = NO
IMPLEMENTATION AUTHORITY = NO
RUNTIME AUTHORITY = NO
PAPER / SHADOW / TINY-LIVE / LIVE AUTHORITY = NO
```

Files 14 through 18 SHALL be read together as the current Owner-directed AI/self-development/monitoring/investigation discussion set until a later governed consolidation explicitly supersedes them.

Because this file changes candidate semantics after the previous review state, it requires a fresh Architecture/Consistency study, fresh Red-Team review and explicit Owner decision before any final acceptance claim.