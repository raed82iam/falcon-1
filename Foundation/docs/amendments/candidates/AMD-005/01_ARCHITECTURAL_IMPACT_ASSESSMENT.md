# AMD-005 Architectural Impact Assessment

**Status:** Approved Assessment  
**Approval Record:** GOV-060  
**Assessment Scope:** Documentation and authority boundaries only

## 1. Finding

The submitted Foundation Guardian concept is directionally consistent with Falcon’s Vision and Constitution, but it materially narrows the meaning of the currently Approved `AUT-002 v1.0`.

`AUT-002 v1.0` describes one Falcon-wide Guardian that receives threats to capital, constitutional invariants, integrity, and safe continuity. The proposed model requires two distinct protection jurisdictions:

1. FFG protects Foundation integrity and technical continuity without business interpretation.
2. A future Application Guardian may protect capital, exposure, positions, orders, and Application-domain continuity.

The existing Approved document therefore cannot be silently reinterpreted. A versioned successor and governed migration are required.

## 2. Compatibility with Falcon Vision

The proposal supports the Prime Objective because technical platform protection is a prerequisite for trustworthy capital protection. It does not weaken the ordering:

1. Protect Capital.
2. Manage Capital.
3. Grow Capital.

The split assigns technical and business protection to competent authorities. It does not demote capital protection or convert Foundation into a business-domain authority.

## 3. Compatibility with AMD-004

AMD-004 states that final Guardian architecture is outside its scope. AMD-005 supplies that missing decision without changing the proposed awareness hierarchy.

The packages are compatible only if:

- FSA and FFG remain separate;
- FSA owns awareness, diagnosis, technical verification, and bounded repair;
- FFG owns Foundation protective restriction and Platform Safe Mode;
- FSA cannot release FFG restrictions or alter FFG jurisdiction;
- FFG cannot modify FSA or own Self-Repair or Self-Evolution;
- either may isolate the other only through independently authorized controls and evidence;
- awareness rank is not emergency authority; and
- activation of AMD-005 is coordinated with the terminology adopted for AMD-004.

AMD-005 does not approve AMD-004 and AMD-004 does not approve AMD-005.

## 4. Documents Requiring Successor Treatment After Approval

At minimum:

- `AUT-002` requires a v2.0 successor.
- `CON-011` requires review to distinguish Foundation restrictions from future Application protection restrictions.
- `FDN-005` requires terminology alignment for Platform Safe Mode and technical criticality.
- `ADR-F008` requires confirmation that its enforcement boundary applies to FFG restrictions.
- `VPL-006` requires a successor or versioned expansion.
- the Core index, Specification Registry, Specification Tree, glossary, and cross-references require controlled update.

No such Approved document is changed by AMD-005 v0.1.

## 5. Necessary Corrections to the Submitted Draft

The candidate specification:

- assigns the successor to existing identifier `AUT-002` rather than inventing an unregistered identifier;
- uses **event-driven**, not “eventfully”;
- treats technical criticality as governed metadata, never as a Guardian inference from business content;
- distinguishes technical priority from commercial or financial importance;
- prevents FFG from reading business payload while allowing it to consume governed technical validation outcomes;
- makes all intervention authority explicit, scoped, expiring, attributable, and auditable;
- prevents restart, failover, or time passage from clearing restrictions;
- makes release a separate authorized decision;
- prevents mutual FSA/FFG supervision from becoming self-validation;
- reserves irreversible actions and long autonomous containment for consequence-class governance; and
- leaves unresolved realization details to future ADRs instead of presenting them as already decided.

## 6. Principal Risks

| Risk | Required control |
|---|---|
| FFG becomes a universal authority | jurisdiction, mandate, consequence class, and expiry enforcement |
| Technical criticality becomes disguised business priority | approved technical registry and prohibition on payload interpretation |
| FSA and FFG form a circular trust pair | independent evidence and third-party authority for material disputes |
| Guardian compromise releases restrictions | protected restriction state and independent enforcement |
| Safe Mode becomes an undefined shutdown | governed survival set and explicit Platform modes |
| Application Guardian leaks business meaning into Foundation | abstract technical protection request Contract |
| Emergency authority persists indefinitely | review intervals, maximum autonomous duration, and escalated renewal |
| FFG bypasses component ownership | directives executed by the competent owning mechanism |

## 7. Assessment Result

The concept is architecturally suitable as a **candidate successor**, subject to Owner approval and later controlled activation.

It is not suitable for direct insertion into Approved documentation without the versioning, migration, authority, Contract, and evidence controls defined by AMD-005.
