# Falcon Document Authority

**Identifier:** GOV-001  
**Version:** 1.1  
**Status:** Approved  
**Effective Date:** 2026-07-24 14:11  
**Approval Record:** GOV-005
**Authority:** Falcon Constitution

## 1. Purpose

This document defines how Falcon’s document classes relate, where authority resides, and how conflicts are resolved.

It does not define Falcon’s internal architecture. It governs the documents that may define it.

## 2. Authority Model

```text
Falcon Vision
    ↓
Falcon Constitution
    ↓
Governance Authority
    ↓
Specifications ───────── Standards
    ↓                        ↓
Architecture Decision Records
    ↓
Contracts, Designs, Plans, and Operational Policies
    ↓
Implementation, Verification, and Recorded Outcomes
```

Specifications and Standards occupy parallel jurisdictions:

- Specifications govern required Falcon behavior, properties, boundaries, and outcomes.
- Standards govern the required form, quality, evidence, and lifecycle of artifacts and recurring practices.

Neither class may contradict the other. When they appear to conflict, the higher governing authority shall resolve the jurisdiction before work proceeds.

## 3. The Specification Tree

The Specification Tree is not an additional rank of authority.

It is the controlled map of specification domains, identifiers, ownership, dependencies, and coverage. Its authority is administrative: it determines where a requirement belongs and prevents omission, duplication, and uncontrolled specification growth.

The Tree shall not:

- create requirements by listing a title;
- approve a specification;
- change the meaning of a specification;
- replace a specification’s content; or
- override the Vision or Constitution.

## 4. Document Classes

### 4.1 Vision

The Vision defines:

- why Falcon exists;
- Falcon’s permanent identity;
- the Prime Objective;
- enduring philosophy and values; and
- the standard by which all future direction is judged.

The Vision is singular. It is not supplemented by competing vision documents.

### 4.2 Constitution

The Constitution translates the Vision into:

- binding duties;
- permanent prohibitions;
- limits of authority;
- decision and autonomy governance;
- compliance obligations; and
- rules for constitutional continuity.

The Constitution shall not contain replaceable design choices or temporary operating procedure.

### 4.3 Specifications

A Specification defines required truth.

It states what Falcon or a governed part of Falcon shall do, preserve, expose, reject, constrain, record, or demonstrate. It includes measurable acceptance conditions and explicit boundaries.

A Specification shall not prescribe a particular solution unless the solution itself is an approved, necessary requirement.

### 4.4 Standards

A Standard defines required discipline.

It establishes consistent rules for how documents, evidence, decisions, security practices, quality controls, and other recurring concerns are created and assessed.

A Standard shall not become a hidden functional specification.

### 4.5 Architecture Decision Records

An ADR records one consequential architectural decision in its historical context.

It explains the forces considered, the choice made, the alternatives rejected, and the resulting consequences. An accepted ADR is immutable; a later ADR may supersede it.

An ADR shall not:

- amend the Vision or Constitution;
- override a Specification or Standard;
- act as a backlog or implementation plan;
- claim authority outside its declared scope; or
- convert a temporary workaround into permanent architecture without review.

### 4.6 Contracts

A Contract defines the exact observable meaning exchanged across a governed boundary.

It states participants, authority, inputs, outputs, conditions, invariants, rejection, compatibility, security, and evidence. It translates approved Specifications and applicable ADRs into stable boundary semantics without choosing implementation details unnecessarily.

A Contract shall not:

- create behavior absent from an approved Specification;
- grant authority;
- override a Standard or ADR;
- embed a technology choice that requires an ADR;
- treat structural validity as authorization or success; or
- silently change meaning through implementation.

## 5. Conflict Resolution

When documents conflict:

1. stop relying on the disputed lower authority;
2. identify the controlling higher authority;
3. determine whether the conflict is substantive or jurisdictional;
4. contain any material capital, integrity, or safety exposure;
5. correct, supersede, or withdraw the lower document; and
6. preserve a traceable record of the resolution.

Recency alone does not determine authority. A newer lower-ranked document cannot overrule an older higher-ranked document.

## 6. Canonical Source

Every governed document shall have:

- one unique identifier;
- one canonical location;
- one accountable owner;
- one current status;
- one current version; and
- explicit supersession history.

Copies are informative unless expressly designated as synchronized authoritative mirrors.

## 7. Status Vocabulary

The allowed document statuses are:

- **Draft:** incomplete and non-binding;
- **Proposed:** complete enough for formal review;
- **Approved:** binding within its declared scope;
- **Deprecated:** still valid only for controlled transition;
- **Superseded:** replaced and no longer current;
- **Rejected:** reviewed and denied authority; and
- **Archived:** retained only as history.

“Implemented” is not a document authority status. Conformance of implementation is tracked separately.

## 8. Change Rule

No document may acquire authority merely because work was based upon it. Approval must be explicit.

Every material change shall identify:

- the authority permitting the change;
- affected documents and obligations;
- compatibility with the Vision and Constitution;
- the review and approval record;
- migration consequences; and
- the effective date.
