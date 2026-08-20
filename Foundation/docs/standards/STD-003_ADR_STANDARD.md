# Architecture Decision Record Standard

**Identifier:** STD-003  
**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-24  
**Approval Record:** GOV-003
**Owner:** Falcon Standards Authority

## 1. Purpose

This Standard defines when an ADR is required, what it contains, and how its history is preserved.

## 2. ADR Trigger

An ADR is required when a decision:

- establishes or changes an enduring architectural boundary;
- assigns or transfers material authority or ownership;
- selects between consequential alternatives with long-lived tradeoffs;
- introduces a system-wide dependency or communication rule;
- changes a compatibility or replacement policy;
- interprets a Specification in a way with durable architectural effect; or
- supersedes an accepted architectural decision.

Routine implementation within an approved design does not require an ADR.

## 3. Required Metadata

Every ADR shall state:

- identifier;
- title;
- status;
- date;
- decision owner;
- scope;
- affected Specifications;
- applicable Standards;
- related ADRs;
- supersedes;
- superseded by; and
- decision record.

## 4. Required Content

Every ADR shall contain:

1. context;
2. decision drivers;
3. constraints from higher authority;
4. considered alternatives;
5. decision;
6. consequences;
7. risks and mitigations;
8. compatibility and transition;
9. verification of conformance; and
10. approval.

## 5. Decision Rule

An ADR records one decision. It shall be specific enough to govern the declared scope and narrow enough to avoid becoming a general design document.

## 6. Immutability

An accepted ADR is immutable in decision meaning.

Corrections may repair formatting or factual reference errors without altering the decision. A changed decision requires a new ADR that explicitly supersedes the prior ADR.

## 7. Prohibited Content

An ADR shall not contain:

- constitutional amendments;
- requirements that belong in a Specification;
- artifact rules that belong in a Standard;
- task lists or delivery plans;
- unresolved brainstorming presented as a decision;
- progress tracking;
- source code as the decision itself; or
- retrospective alteration of accepted history.

## 8. Statuses

- **Draft:** being developed;
- **Proposed:** submitted for decision;
- **Accepted:** approved and current;
- **Rejected:** considered and not adopted;
- **Deprecated:** retained during controlled withdrawal; and
- **Superseded:** replaced by a named later ADR.
