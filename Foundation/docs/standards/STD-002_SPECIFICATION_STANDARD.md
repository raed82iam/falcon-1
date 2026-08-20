# Specification Standard

**Identifier:** STD-002  
**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-24  
**Approval Record:** GOV-003
**Owner:** Falcon Standards Authority

## 1. Purpose

This Standard defines the mandatory structure and quality rules for Falcon Specifications.

## 2. Required Structure

Every Specification shall contain:

1. document metadata;
2. purpose;
3. constitutional basis;
4. scope;
5. non-scope;
6. terms and definitions;
7. actors and authorities;
8. normative requirements;
9. states and invariants, when applicable;
10. failure and degraded behavior;
11. evidence and acceptance criteria;
12. dependencies and affected domains;
13. security, risk, and continuity obligations;
14. unresolved matters;
15. approval record; and
16. change history.

Sections that do not apply shall state why; they shall not be silently omitted.

## 3. Requirement Rules

Every normative requirement shall:

- have a unique stable requirement identifier;
- contain one primary obligation;
- identify the responsible subject;
- be testable, inspectable, or otherwise demonstrable;
- avoid ambiguous terms unless those terms are defined;
- state relevant conditions and boundaries; and
- trace to a higher authority or justified dependent requirement.

Example:

```text
DEC-001-REQ-014

Falcon SHALL record the material assumptions used by a capital-affecting decision before that decision is authorized.
```

## 4. Separation of Content

A Specification shall distinguish:

- requirement;
- rationale;
- example;
- note;
- unresolved question; and
- acceptance evidence.

Rationale and examples shall not create hidden requirements.

## 5. Quality Gates

A Specification shall not be approved when:

- its authority conflicts with the Vision or Constitution;
- its subject has no clear owner;
- requirements duplicate or contradict another source of truth;
- material requirements lack acceptance evidence;
- failure behavior is omitted for a high-consequence subject;
- uncertainty is presented as settled fact;
- future ideas are mixed with binding requirements; or
- approval depends on an undefined document.

## 6. Decomposition

A Specification may be divided when the parent:

- defines the common authority and invariants;
- assigns non-overlapping child scopes;
- preserves complete traceability; and
- remains understandable without duplicating child content.

Decomposition shall not be used to hide system-wide risk across documents.
