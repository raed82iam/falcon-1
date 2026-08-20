# Document Control Standard

**Identifier:** STD-001  
**Version:** 1.0  
**Status:** Approved  
**Effective Date:** 2026-07-24  
**Approval Record:** GOV-003
**Owner:** Falcon Standards Authority

## 1. Required Metadata

Every governed document shall state:

- identifier;
- title;
- version;
- status;
- accountable owner;
- governing authority;
- approval record or approval reference;
- effective date when approved; and
- supersession relationship when applicable.

## 2. Canonicality

Every governed document shall have one canonical location. A registry shall point to that location.

Uncontrolled copies shall be marked informative. A copy that cannot demonstrate synchronization shall not be treated as authoritative.

## 3. Versioning

Versions shall use:

- major change for changed meaning, obligation, or compatibility;
- minor change for backward-compatible clarification or addition; and
- correction change for non-semantic repair.

Version changes do not confer approval.

## 4. Approval

Approval shall identify:

- the approving authority;
- the approved version;
- the decision date;
- unresolved conditions, if any; and
- the effective date.

A placeholder, author declaration, or status typed into the document is not sufficient evidence of approval.

## 5. Supersession

An approved document shall not be silently overwritten when its governing meaning changes.

The replacement shall identify what it supersedes. The prior document shall identify its successor and remain preserved as history.

## 6. Dates

Dates shall use ISO 8601:

```text
YYYY-MM-DD
```

Unknown dates shall be marked `Pending`; placeholders such as `YYYY-MM-DD` are prohibited in approved documents.

## 7. Reviews

Review evidence shall be proportionate to document authority and consequence. Vision and Constitution changes require their own governing rules. Specifications, Standards, and ADRs require review by authorities competent in every materially affected domain.
