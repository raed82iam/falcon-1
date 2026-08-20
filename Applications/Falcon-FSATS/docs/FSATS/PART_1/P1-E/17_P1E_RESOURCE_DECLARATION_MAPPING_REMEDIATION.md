# FSATS Part 1 — P1-E Resource Declaration Mapping Remediation

**Status:** `SEMANTIC_REMEDIATION / CONTROLS P1-E RESOURCE-DECLARATION INTERPRETATION / NOT_OWNER_ACCEPTED / NOT_CLOSED`  
**Related Finding:** `AC-P1E-001`  
**Implementation Authority:** `NOT GRANTED`  
**Runtime Authority:** `NOT GRANTED`

## 1. Purpose

This record remediates the P1-E Round-1 Architecture/Consistency finding concerning exact mapping of the CON-023-required Application Manifest resource declarations to the Foundation-owned authoritative resource-governance model.

It supplements the frozen P1-E candidate and controls the interpretation of its resource declaration sections where necessary.

## 2. Required Manifest Resource Declarations

Every Falcon Application Manifest SHALL declare the CON-023-required resource fields, including:

- resource requirements;
- minimums;
- ceilings;
- priorities;
- degraded behavior.

The declaration SHALL preserve the following ownership separation.

## 3. Application-Declared Resource Ceiling

```text
APPLICATION_DECLARED_RESOURCE_CEILING
=
APPLICATION MAXIMUM USEFUL / REQUESTABLE RESOURCE BOUND
OR OTHER EXACT APPLICATION REQUIREMENT CEILING
```

This is an Application-owned declaration of what the Application can legitimately use/request under its business and operational design.

It is not the Foundation authoritative grant ceiling.

```text
APPLICATION_DECLARED_RESOURCE_CEILING
!= FOUNDATION_AUTHORITATIVE_RESOURCE_CEILING
```

Foundation may grant a lower ceiling, cap, reduce, revoke or otherwise govern resource authority according to the current Foundation resource-governance contract.

## 4. Application-Declared Priority / Criticality Evidence

Every Application Manifest SHALL declare the priority/criticality information required by CON-023 as Application-owned business-need evidence and requested priority semantics.

This may include, as applicable:

- business consequence of starvation;
- minimum-safe obligation;
- protection/live-critical requirement;
- degradation consequence;
- deferrability;
- reclaimability;
- restoration urgency;
- requested priority class or other governed priority evidence.

This declaration informs FSARM and Foundation resource-governance decisions but does not create authoritative Foundation priority.

```text
APPLICATION_DECLARED_PRIORITY_EVIDENCE
!= FOUNDATION_AUTHORITATIVE_PRIORITY
```

Foundation retains the final authoritative priority/criticality decision where Foundation governance owns that decision.

## 5. FSARM Interpretation

FSARM MAY consume Application-declared resource requirements, minimums, declared ceilings and priority/criticality evidence as inputs to its bounded FSATS-wide coordination strategy.

FSARM SHALL NOT reinterpret those declarations as authority to:

- create Foundation resources;
- mutate Foundation authoritative grants or ceilings;
- override Foundation protected floors/reserves;
- mint Foundation priority/criticality authority;
- expand an Application beyond its governed authority.

The current governing sequence remains:

```text
APPLICATION DECLARATION / CURRENT EVIDENCE
-> FSARM INTERNAL COORDINATION
-> PROVEN RESIDUAL NEED
-> GOVERNED FOUNDATION REQUEST
-> FOUNDATION AUTHORITATIVE DECISION
```

## 6. Controlling Mapping

For current P1-E interpretation:

```text
CON-023 RESOURCE REQUIREMENTS
-> APPLICATION DECLARATION

CON-023 MINIMUMS
-> APPLICATION-DECLARED MINIMUM / MINIMUM-SAFE REQUIREMENT

CON-023 CEILINGS
-> APPLICATION-DECLARED MAXIMUM USEFUL / REQUESTABLE CEILING
-> NOT FOUNDATION AUTHORITATIVE CEILING

CON-023 PRIORITIES
-> APPLICATION-DECLARED PRIORITY / CRITICALITY REQUIREMENT OR EVIDENCE
-> NOT FOUNDATION AUTHORITATIVE PRIORITY

CON-023 DEGRADED BEHAVIOR
-> APPLICATION-OWNED DEGRADATION / SHEDDING / RECOVERY DECLARATION
```

## 7. Foundation Ownership Preserved

Nothing in this remediation changes the current Foundation boundary.

Foundation remains canonical total-resource truth and final resource authority, including authoritative grants/ceilings, protected floors/reserves and Foundation-governed priority/criticality decisions as applicable.

```text
FSARM != FOUNDATION_RESOURCE_GOVERNANCE
APPLICATION_RESOURCE_DECLARATION != FOUNDATION_RESOURCE_AUTHORITY
```

## 8. Review Reset

This record is a semantic remediation after the first P1-E freeze.

Therefore the first freeze and Round-1 review cannot be used as final PASS evidence for the remediated semantic set.

Required next sequence:

```text
P1-E CANDIDATE
+ THIS REMEDIATION
-> NEW SEMANTIC FREEZE
-> FRESH ARCHITECTURE / CONSISTENCY REVIEW
-> FRESH RED-TEAM
-> OWNER REVIEW
```
