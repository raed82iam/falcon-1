# FSATS Part 1 — AI Repair and Controlled Recovery Authority Matrix Candidate

**Status:** `OWNER-DIRECTED CANDIDATE / NOT_OWNER_ACCEPTED / NOT_CLOSED`  
**Branch:** `application-development`  
**Scope:** `PART_1 CROSS-CUTTING / APPLICATION-SIDE AI REPAIR AND RECOVERY`  
**Implementation Authority:** `NOT GRANTED`  
**Runtime Authority:** `NOT GRANTED`

## 1. Purpose

This record materializes the Project Owner's 2026-08-14 direction for what shall happen after an FSATS intelligent component, CSA, LSA, MSA or wider Application AI scope becomes faulty, untrusted, isolated or killed.

It supplements the current AI Containment and Safety Continuity candidate. Safety Continuity protects current obligations while repair/recovery occurs. Repair/recovery does not suspend those protection duties.

## 2. Governing Recovery Sequence

```text
DETECT
-> CONTAIN
-> INVESTIGATE
-> REPAIR IN ISOLATION
-> INDEPENDENT VALIDATION
-> CONTROLLED REVIVAL
```

These stages SHALL remain distinguishable. The affected intelligent subject SHALL NOT collapse them into one self-controlled loop.

```text
DETECTION != CONTAINMENT
CONTAINMENT != INVESTIGATION
INVESTIGATION != REPAIR
REPAIR != VALIDATION
VALIDATION != RELEASE
RESTART != CONTROLLED REVIVAL
```

## 3. Core Separation Rule

A killed or materially untrusted intelligent subject SHALL NOT be the sole authoritative actor that:

- investigates itself;
- selects its own trusted evidence;
- repairs itself inside trusted active runtime;
- validates its own repair;
- restores its own permissions/authority;
- declares its own incident closed;
- releases itself from containment.

It may contribute preserved evidence or non-authoritative diagnostic material when safely available, but trust restoration requires independent governed evidence.

## 4. Detection Sources

Detection may originate from any independently attributable source with legitimate observation capability, including as applicable:

- Monitor AI;
- parent Awareness;
- sibling domain evidence without authority inheritance;
- Guardian/protection evidence;
- deterministic health/integrity checks;
- Foundation lifecycle/security/trust signals;
- FSTSimA validation/fault evidence;
- Project Owner observation or direction.

Detection does not grant repair or release authority.

## 5. Containment Rule

Containment follows the existing Safety Continuity rule:

```text
PROVEN LOCAL FAULT
-> MINIMUM NECESSARY CONTAINMENT

UNKNOWN / PROPAGATED TRUST DAMAGE
-> EXPAND CONTAINMENT UNTIL A TRUSTWORTHY BOUNDARY IS ESTABLISHED
```

Containment SHALL also fence invalidated downstream work and preserve independently trusted protective obligations as already required by the V2 Safety Continuity remediation.

## 6. Investigation Ownership Matrix

### 6.1 Intelligent Component / CSA

Where the fault is proven local to one intelligent component or CSA and the parent LSA remains trusted:

```text
PRIMARY INVESTIGATION = PARENT LSA
APPLICATION-WIDE IMPACT REVIEW = APPLICATION MSA AS MATERIALITY REQUIRES
```

The affected component/CSA does not own release.

### 6.2 LSA

Where an LSA is killed or materially untrusted and the Application MSA remains trusted:

```text
PRIMARY APPLICATION INVESTIGATION = APPLICATION MSA
DOMAIN EVIDENCE = PRESERVED FROM TRUSTED SOURCES
```

No sibling LSA inherits the killed LSA's authority merely because it is available.

### 6.3 MSA

Where the Application MSA itself is killed or materially untrusted:

```text
NO LOWER AWARENESS ENTITY MAY PROMOTE ITSELF INTO MSA AUTHORITY
```

Investigation SHALL use an independent Application recovery path with preserved evidence, applicable Monitor/Guardian/Foundation evidence, isolated remediation tooling and FSTSimA validation where appropriate. Final release follows the Owner/governance matrix below.

### 6.4 Application AI Set

Where multiple Awareness/intelligent components in one Application are affected or common state is untrusted, investigation scope expands to the Application AI set. Non-AI Application safety/continuity functions may remain active only when independently trusted.

### 6.5 Multi-Application / FSATS AI Set

Where the trust blast radius crosses multiple FSATS Applications, each Application retains its own business ownership while the incident is coordinated through governed cross-Application/Foundation boundaries. FSATS itself does not become a hidden runtime principal or repair authority.

## 7. Repair Location and Ownership

Repair SHALL occur outside the trusted active subject in an isolated governed remediation/development environment appropriate to the owning Application.

Candidate repair may be produced by an unaffected authorized parent Awareness entity, authorized Application development/maintenance tooling, or another explicitly governed repair path.

```text
KILLED SUBJECT
-/-> SILENT IN-PLACE TRUSTED-RUNTIME SELF-REPAIR
```

A repair candidate does not restore trust or runtime authority.

## 8. Repair Types

Permitted candidate repair classes include as applicable:

- configuration correction;
- state repair;
- model/state replacement;
- bounded code correction;
- rollback to last trusted state;
- replacement of the affected component;
- Factory Reset/reconstruction when lesser remediation cannot re-establish trust.

The chosen repair must preserve forensic evidence and must not rewrite history.

## 9. Independent Validation

Before Controlled Revival, validation SHALL be proportionate to consequence and may include as applicable:

- identity/hash/provenance validation;
- architecture/ownership validation;
- authority/permission validation;
- static integrity validation;
- behavioral validation;
- regression testing;
- FSTSimA scenario/fault/replay testing;
- security review;
- fresh Red-Team/adversarial validation for material/high-consequence repairs;
- dependency/state/reconciliation validation;
- proof that stale pre-Kill work remains fenced;
- proof that Safety Continuity obligations remained protected.

The repaired subject SHALL NOT be the sole validator of its own repair.

## 10. Owner Intervention Matrix

### Level R1 — Minor / Proven Local / Pre-Authorized

Eligible only when all of the following hold:

- fault boundary is proven local;
- goals/purpose, authority/permissions and protected architecture remain intact;
- no evidence integrity issue exists;
- no cross-scope contamination exists;
- repair type is already pre-authorized;
- deterministic validation is complete and passes;
- no material business/financial safety consequence remains unresolved.

Candidate behavior:

```text
AUTO CONTAIN
-> AUTO DIAGNOSE
-> PRE-AUTHORIZED REPAIR
-> COMPLETE VALIDATION
-> RESTRICTED/PROBATIONARY RETURN IF POLICY ALLOWS
-> OWNER NOTIFICATION + FULL EVIDENCE
```

This record does not itself grant the pre-authorization. Exact future policy must explicitly define it.

### Level R2 — Material AI Fault

Examples include material model/state corruption, nontrivial code repair, meaningful behavior drift, or repair that changes material intelligent state while the trust boundary is still established.

Candidate behavior:

```text
AUTO CONTAIN
-> INVESTIGATE
-> PREPARE REPAIR
-> INDEPENDENTLY VALIDATE
-> OWNER APPROVAL REQUIRED BEFORE CONTROLLED REVIVAL
```

The Owner approves/rejects revival; the Owner is not expected to perform manual technical repair.

### Level R3 — Critical / Unknown Trust Failure

Mandatory Owner/governance involvement before release when any of the following are material:

- goals/purpose changed or uncertain;
- authority/permissions expanded or uncertain;
- protected core architecture changed or uncertain;
- evidence/provenance tampering;
- monitoring/audit/containment interference;
- unknown or cross-Application trust blast radius;
- Guardian deterministic safety path trust lost;
- trusted baseline unavailable or suspect;
- Factory Reset required;
- repair requires architectural/governance change;
- no independent validation path can establish trust.

```text
CONTAINMENT + SAFETY CONTINUITY
-> OWNER/GOVERNANCE DECISION REQUIRED
-> NO SILENT AUTOMATIC REVIVAL
```

## 11. Owner Role

Candidate invariant:

```text
OWNER ATTENTION != OWNER MANUAL REPAIR
```

For material/critical incidents, Falcon should present the Owner with an attributable decision package containing at least:

- what failed;
- detected scope and trust blast radius;
- what was contained;
- what remains safe/available;
- open financial/operational obligations;
- root-cause conclusion or unresolved uncertainty;
- repair performed/proposed;
- exact changed artifacts/state;
- validation results;
- residual risk/limitations;
- recommended recovery decision;
- exact decision requested from the Owner.

## 12. Safety Continuity During Repair

Repair/recovery work SHALL NOT orphan existing exposure or other obligations.

While affected AI remains unavailable:

```text
NEW RISK = DENIED / RESTRICTED AS GOVERNED
EXISTING EXPOSURE = MONITORED / PROTECTED
RECONCILIATION = ACTIVE WHERE TRUSTED
HARD PROTECTION = ACTIVE WHERE INDEPENDENTLY TRUSTED
AUDIT/EVIDENCE = PRESERVED
OWNER CONTROL = PRESERVED WHERE INDEPENDENTLY TRUSTED
```

The repair process may take longer than the protection response. Capital protection therefore proceeds independently of repair completion.

## 13. Controlled Revival

Controlled Revival requires successful completion of the required remediation and validation chain and the correct release authority for the incident level.

```text
RESTARTED != RECOVERED
REPAIRED != TRUSTED
TESTED != RELEASED
REVIVED != NORMAL
```

Initial return SHOULD be restricted/probationary where consequence warrants, with explicit observation and rollback/recontainment ability.

## 14. Work-Package Impact

This candidate shall be materialized later without changing current ownership:

- P1-D: repair/recovery identities, state/epoch/evidence primitives;
- P1-E: manifest degraded/recovery/repair declarations;
- P1-F/G/H/I/J: exact per-Application impact and ownership;
- P1-K: governed repair/recovery status, evidence and Owner-decision contract families as applicable;
- P1-L: executable failure, repair, restart, replay, validation and revival fixtures.

## 15. Foundation and Web Boundaries

Foundation owns generic lifecycle/security/trust/containment and FSA internal recovery semantics. FCR-0082 records the generic continuity planning boundary; FCR-0012/FCR-0030 preserve FSA-specific recovery/governance.

Shared Web owns Web-local presentation and resilience only. The repair/recovery method has been handed to Web through FCR-0083 for Web-side planning consumption. Web does not become repair authority for FSATS/FSA.

## 16. Non-Grant

This candidate creates no implementation, runtime, deployment, provider/broker, Paper, Tiny Live or Live authority.
