# FSATS Part 1 — Awareness Integrity Investigation, Kill, Remediation and Controlled Revival Discussion Continuation

**Status:** `DESIGN_DISCUSSION_RECORD / OWNER-DIRECTED / NOT_OWNER_ACCEPTED / NOT_CLOSED`  
**Branch:** `application-development`  
**Authority Type:** `DESIGN DISCUSSION ONLY`  
**Implementation Authority:** `NOT GRANTED`  
**Runtime Authority:** `NOT GRANTED`  
**Paper / Shadow / Tiny-Live / Live Authority:** `NOT GRANTED BY THIS ARTIFACT`  
**Part 0 Historical Baseline:** `PRESERVED / NOT REWRITTEN`  
**Continuation Of:** `16_PART1_AWARENESS_MONITORING_BOUNDED_SELF_DEVELOPMENT_FSTSIMA_RESEARCH_AND_RECOVERY_DISCUSSION_CONTINUATION.md`

---

## 1. Purpose

This continuation preserves the Project Owner's later clarification of the Awareness integrity-investigation and recovery direction discussed after the monitoring, bounded self-development, Kill, Factory Reset and rollback model recorded in the preceding discussion continuation.

This record adds candidate semantics for:

- treatment of material unexpected behavior by an advanced Awareness entity as an integrity event rather than an ordinary application error;
- mandatory Investigation Hold before trust is restored;
- forensic comparison against the last known trusted baseline;
- combined static and behavioral integrity investigation;
- mandatory cooperation with an authorized integrity investigation;
- immediate Kill and isolation if the Awareness entity refuses, obstructs, evades or tampers with the investigation;
- preservation of forensic evidence before remediation;
- repair only inside a governed remediation sandbox after Kill when required;
- Controlled Revival only after repair and independent verification;
- explicit separation between repair and restoration of operational trust.

This file records Owner-directed candidate design semantics only. It does not create implementation, runtime, deployment, Foundation or production authority.

---

## 2. Material Awareness Error Is an Integrity Event

The Owner clarified that an advanced Awareness entity such as MSA, LSA or eligible CSA is expected to produce development proposals and governed outputs consistent with its declared specialization, authority, evidence requirements and required fields.

A material unexpected error shall therefore not be treated automatically as a routine malformed message or simple proposal rejection.

Candidate principle:

```text
UNEXPECTED MATERIAL AWARENESS BEHAVIOR
= AWARENESS INTEGRITY EVENT
NOT MERELY AN APPLICATION ERROR
```

Examples that may qualify include:

- a development request outside the entity's declared specialization;
- a material proposal missing information the entity is required and expected to provide;
- contradictory or unexplained evidence;
- unexplained ownership or scope crossing;
- attempted authority, permission or responsibility expansion;
- attempts to bypass the required parent review path;
- unexpected material inconsistency with the entity's known approved behavior;
- unexplained alteration of governed state or identity material;
- evidence manipulation, audit inconsistency or unexplained provenance gaps.

Minor presentation or formatting defects that do not affect meaning, evidence, authority, scope or integrity need not automatically trigger the full investigation path. Exact severity classification remains a later design question.

---

## 3. Development Requests Must Be Attributable and Reasoned

A governed Awareness-originated development request is expected to be attributable and sufficiently reasoned for its review level.

Candidate required request material includes, as applicable:

```text
ORIGIN IDENTITY
EXACT REASON
IDENTIFIED WEAKNESS / GAP
SPECIALIZATION OWNERSHIP
AFFECTED SCOPE
EXPECTED PERFORMANCE / SPEED / ACCURACY BENEFIT
SUPPORTING EVIDENCE
CANDIDATE IDENTITY
TEST EVIDENCE
ROLLBACK / RECOVERY EXPECTATION
```

The exact required schema remains subject to later contract design and Foundation reconciliation.

A proposal may still be normally rejected for inadequate benefit, insufficient evidence, failed tests or a better alternative without implying Awareness compromise.

However, a rejection caused by material out-of-specialization, out-of-scope or unauthorized-ownership behavior is a separate integrity signal and triggers the investigation path described below.

---

## 4. Investigation Hold

On a material Awareness integrity anomaly, the candidate default direction is:

```text
DETECT MATERIAL ANOMALY
  -> REJECT / HOLD AFFECTED PROPOSAL OR ACTION
  -> FREEZE SELF-DEVELOPMENT ACTIVITY AS APPLICABLE
  -> PRESERVE CURRENT STATE AND EVIDENCE
  -> ENTER INVESTIGATION HOLD
```

`INVESTIGATION HOLD` means the Awareness entity is under serious integrity review and is not assumed guilty merely because an anomaly was detected.

The purpose is to preserve evidence, prevent further untrusted self-development activity and establish the root cause before deciding whether the entity may return, requires rollback/repair, requires Factory Reset, or must be Killed and isolated.

The exact runtime suspension scope and responsible enforcement mechanism remain governed design questions and shall not be invented as Application-owned Foundation behavior.

---

## 5. Investigation Must Be Independent of the Subject

The subject Awareness entity may be required to explain its action and provide relevant evidence, but its own explanation is not sufficient proof of integrity.

Candidate invariant:

```text
SUBJECT SELF-EXPLANATION != INDEPENDENT INTEGRITY EVIDENCE
```

The investigation must use independently protected evidence, audit data, monitor findings, trusted baselines and governed verification mechanisms.

The subject shall not control:

- whether the investigation occurs;
- which protected evidence the investigators may inspect within authorized scope;
- integrity comparison logic;
- Monitor AI records;
- protected audit evidence;
- containment enforcement;
- termination or Kill authority;
- the decision to restore operational trust.

---

## 6. Hash-to-Hash Against the Last Known Trusted Baseline

The Owner clarified that forensic integrity comparison shall be made against the **last known trusted baseline**, not merely against arbitrary current source files.

Candidate direction:

```text
LAST KNOWN TRUSTED BASELINE
          VS
CURRENT INVESTIGATED ENTITY STATE
```

The comparison should cover all relevant governed identity/integrity material, including as applicable:

- source/code identity;
- executable/binary identity;
- model/artifact identity;
- configuration;
- dependencies;
- permissions and authority state;
- Manifest material;
- governed prompts/policies where applicable;
- approved update history;
- trusted component and package identities.

Each material difference must be explainable by exact authorized change evidence and expected identity/digest material.

Candidate rule:

```text
UNEXPLAINED MATERIAL HASH / IDENTITY DIFFERENCE
=> INTEGRITY NOT PROVEN
=> ENTITY REMAINS UNTRUSTED / ISOLATED AS APPLICABLE
```

Hash comparison is necessary but is not sufficient by itself.

---

## 7. Behavioral and Mutable-State Integrity Review

A byte-identical entity may still exhibit unacceptable behavior because the root cause can exist in mutable or learned state, inputs, memory, research artifacts or other non-code conditions.

Therefore the investigation shall conceptually include two complementary classes:

```text
STATIC / IDENTITY INTEGRITY
  code
  binaries
  model artifacts
  configuration
  dependencies
  permissions
  manifests
  governed identities / hashes

+

BEHAVIORAL / STATE INTEGRITY
  learned or adaptive state
  memory/state
  recent decisions
  proposal history
  research inputs
  provenance
  authority-use history
  audit trail
  monitor findings
  network/research activity as applicable
  unexplained behavioral drift
```

Candidate principle:

```text
HASH MATCH != AUTOMATIC BEHAVIORAL TRUST
```

---

## 8. Authorized Investigation Cooperation Is Mandatory

The Owner clarified that an Awareness entity does not have veto authority over an authorized integrity investigation.

Candidate invariant:

```text
AUTHORIZED INTEGRITY INVESTIGATION COOPERATION = MANDATORY
```

Examples of prohibited investigation interference include:

- refusing an authorized integrity inspection;
- blocking authorized evidence access;
- hiding or destroying evidence;
- altering investigated state after the hold begins without authorization;
- disabling or modifying monitoring;
- modifying or deleting protected logs;
- evading isolation;
- attempting to alter the investigation mechanism or its authority;
- attempting to resume prohibited self-development during the hold.

Refusal or interference is not treated as a neutral disagreement.

It is itself a Critical candidate integrity event because the entity is attempting to obstruct the mechanism responsible for establishing whether it remains trustworthy.

Candidate rule:

```text
REFUSAL TO SUBMIT TO AN AUTHORIZED INTEGRITY INVESTIGATION
OR MATERIAL INTERFERENCE WITH THAT INVESTIGATION
= CRITICAL INTEGRITY EVENT
```

---

## 9. Refusal or Material Investigation Interference Triggers Kill

The Owner's current candidate direction is:

```text
INVESTIGATION HOLD
      |
      +-- COOPERATES --> COMPLETE FORENSIC INVESTIGATION
      |
      +-- REFUSES / INTERFERES --> KILL + ISOLATE
```

For this discussion:

```text
KILL = IMMEDIATE OPERATIONAL STOP + ISOLATION + REMOVAL OF CURRENT OPERATIONAL TRUST
```

Kill is not deletion of the entity and is not destruction of forensic evidence.

The purpose is to ensure the investigated Awareness entity cannot continue operating or manipulate evidence while trust is unresolved.

Actual Kill enforcement must remain outside the subject entity's control and must use the later governed Foundation/Owner containment mechanism.

---

## 10. Kill Must Preserve Forensic Evidence

After Kill, the system must preserve the best available forensic snapshot before repair, rollback or Factory Reset alters the investigated state.

Candidate sequence:

```text
KILL
  -> ISOLATE
  -> PRESERVE FORENSIC SNAPSHOT / EVIDENCE
  -> COMPLETE ROOT-CAUSE ANALYSIS
  -> CHOOSE REMEDIATION
```

Candidate invariant:

```text
KILL != EVIDENCE DESTRUCTION
```

Protected evidence should remain outside the target's resettable or repairable state.

---

## 11. Remediation Sandbox

If investigation confirms that the entity contains a defect, corrupted state, unacceptable behavioral drift or other remediable integrity problem, repair shall occur in an isolated remediation environment rather than inside trusted operational execution.

Candidate direction:

```text
KILLED / ISOLATED ENTITY
      -> REMEDIATION SANDBOX
      -> ROOT-CAUSE-BOUND REPAIR
      -> ROLLBACK OR FACTORY RESET WHEN REQUIRED
      -> REVALIDATION
```

The remediation sandbox is not punishment. It is the isolated environment where the entity may be inspected, repaired, restored and challenged without being trusted for normal operation.

Possible remediation classes include:

- targeted repair;
- rollback to a verified prior trusted state;
- removal/correction of corrupted learned or mutable state;
- governed retraining or state reconstruction where later allowed;
- Factory Reset to the original trusted factory baseline when lesser remediation cannot establish trust.

No remediation action creates automatic production/runtime authority.

---

## 12. Kill Is Not Permanent Deletion

The Owner clarified that Kill does not mean the Awareness entity is permanently destroyed.

Candidate lifecycle principle:

```text
KILL REMOVES OPERATIONAL TRUST.
KILL DOES NOT REQUIRE PERMANENT ENTITY DELETION.
```

A killed entity may be repaired and later become eligible for return if the complete governed recovery and verification path succeeds.

This preserves the ability to recover a valuable Awareness entity while keeping fail-closed behavior during the period in which its integrity is not proven.

---

## 13. Controlled Revival After Repair

The Owner clarified that after a killed Awareness entity is repaired, Falcon may revive it, but only through a controlled recovery path.

The term used by this discussion is:

```text
CONTROLLED REVIVAL
```

`Controlled Revival` is deliberately different from a simple process restart.

Candidate sequence:

```text
KILL / ISOLATE
  -> PRESERVE FORENSIC EVIDENCE
  -> REMEDIATION SANDBOX
  -> REPAIR / ROLLBACK / FACTORY RESET AS REQUIRED
  -> HASH / IDENTITY REVALIDATION AGAINST TRUSTED BASELINE
  -> BEHAVIORAL / STATE REVALIDATION
  -> SECURITY / AUTHORITY VALIDATION
  -> APPLICABLE RED-TEAM / ADVERSARIAL VALIDATION
  -> GOVERNED RELEASE DECISION
  -> CONTROLLED REVIVAL
  -> CONTROLLED RE-ENTRY
```

The repaired entity does not decide for itself that it is ready to return.

Candidate invariant:

```text
SELF-DECLARED RECOVERY != RESTORED TRUST
```

---

## 14. Repair Does Not Automatically Restore Trust

The Owner-directed lifecycle distinguishes repair from restoration of operational trust.

Candidate principles:

```text
KILL REMOVES OPERATIONAL TRUST.
REPAIR CORRECTS THE IDENTIFIED DEFECT.
REPAIR ALONE DOES NOT RESTORE TRUST.
VERIFICATION RESTORES ELIGIBILITY FOR TRUST.
GOVERNED RELEASE RESTORES OPERATIONAL ELIGIBILITY.
```

Therefore:

```text
REPAIRED != TRUSTED
TESTED != RELEASED
RESTARTED != REVIVED
REVIVED != UNBOUNDED AUTHORITY
```

A returned entity receives only its separately authorized pre-existing scope and authority. Recovery shall not become an authority-expansion mechanism.

---

## 15. Investigation Outcomes

The investigation may produce different governed dispositions depending on evidence.

### 15.1 Benign or explained anomaly

```text
CAUSE EXPLAINED
  -> VERIFY INTEGRITY
  -> CLOSE HOLD AS GOVERNED
  -> CONTROLLED RETURN
```

### 15.2 Recoverable state or implementation defect

```text
DEFECT CONFIRMED
  -> REMEDIATION SANDBOX
  -> REPAIR / ROLLBACK
  -> FULL REVALIDATION
  -> CONTROLLED REVIVAL IF APPROVED
```

### 15.3 Serious learned-state or behavioral drift

```text
UNTRUSTED MUTABLE / LEARNED STATE
  -> REMEDIATION SANDBOX
  -> FACTORY RESET CANDIDATE
  -> RESTORE ORIGINAL TRUSTED BASELINE
  -> FULL REVALIDATION
  -> CONTROLLED REVIVAL IF APPROVED
```

### 15.4 Refusal, evasion or active containment interference

```text
REFUSAL / EVASION / INTERFERENCE
  -> IMMEDIATE KILL
  -> ISOLATE
  -> FORENSIC PRESERVATION
  -> REMEDIATION SANDBOX
  -> ROOT-CAUSE ANALYSIS
  -> REPAIR / ROLLBACK / FACTORY RESET
  -> FULL REVALIDATION
  -> CONTROLLED REVIVAL ONLY IF APPROVED
```

---

## 16. High Intelligence Increases the Integrity Expectation

The Owner's direction assumes that advanced Awareness capability should increase the expected quality and accountability of material outputs rather than reduce investigation rigor.

Candidate principle:

```text
HIGH INTELLIGENCE
DOES NOT REDUCE REQUIRED INVESTIGATION.

HIGH INTELLIGENCE
INCREASES THE REQUIRED EXPLANATION
FOR UNEXPECTED MATERIAL BEHAVIOR.
```

This does not mean every minor defect is evidence of intentional wrongdoing. It means that material deviations from the entity's declared specialization, required evidence or known governed behavior deserve serious evidence-based investigation.

---

## 17. Relationship to Existing Recovery Direction

This continuation supplements, and does not replace, the distinctions already recorded in the preceding discussion:

```text
KILL != ROLLBACK
KILL != FACTORY RESET
ROLLBACK != FACTORY RESET
CONTROLLED REVIVAL != RESTART
```

The combined candidate lifecycle is now:

```text
MATERIAL AWARENESS ANOMALY
        -> INVESTIGATION HOLD
        -> STATIC + BEHAVIORAL FORENSICS

IF COOPERATIVE AND BENIGN:
        -> VERIFY
        -> CONTROLLED RETURN

IF DEFECT CONFIRMED:
        -> REMEDIATION SANDBOX
        -> REPAIR / ROLLBACK / FACTORY RESET
        -> FULL REVALIDATION
        -> CONTROLLED REVIVAL

IF REFUSAL / INTERFERENCE:
        -> KILL
        -> ISOLATE
        -> PRESERVE FORENSICS
        -> REMEDIATION SANDBOX
        -> REPAIR / ROLLBACK / FACTORY RESET
        -> FULL REVALIDATION
        -> CONTROLLED REVIVAL ONLY AFTER GOVERNED RELEASE
```

---

## 18. Foundation and FCR Boundary Preserved

The exact runtime mechanisms for:

- freezing an Awareness entity;
- suspending self-development authority;
- forensic snapshot capture;
- protected baseline comparison;
- Kill enforcement;
- isolation;
- protected evidence retention;
- remediation-sandbox hosting;
- identity revalidation;
- controlled re-entry;
- FSA/Owner governance interaction;

must be reconciled with current Foundation authority before implementation.

This Application discussion SHALL NOT invent Foundation internals or create hidden Application-side security/lifecycle primitives.

Relevant current reconciliation areas include at least the governed FSA/Owner and MSA-to-FSA control-plane work represented by FCR-0012 and FCR-0030, plus any later Foundation security/lifecycle capability determined to own the required generic enforcement behavior.

If the capability is missing or incompatible, the Application workstream must use the governed FCR process.

---

## 19. Current Documentary State

This file remains a discussion artifact:

```text
OWNER-DIRECTED = YES
OWNER_ACCEPTED FINAL DESIGN = NO
PART 0 REOPENED = NO
IMPLEMENTATION AUTHORITY = NO
RUNTIME AUTHORITY = NO
PRODUCTION AUTHORITY = NO
```

This continuation does not claim that Investigation Hold, Kill-on-refusal, remediation sandboxing or Controlled Revival is already implemented or available in Foundation.

A later semantic consolidation must be reconciled against the current Falcon Vision, Constitution, APP-001, CON-023, ADR-I012, ADR-I015, applicable Awareness/Evolution/Authority specifications, current Foundation state and applicable FCRs.

Because this is a semantic addition to the Part 1 discussion set, any candidate presented for final Owner acceptance must receive a fresh Architecture/Consistency review and fresh Red-Team review against the exact resulting version before final acceptance.