# MDR-001 — Foundation Document-Control Metadata Remediation

**Identifier:** MDR-001  
**Version:** 1.0  
**Status:** Approved and Implemented  
**Effective Date:** 2026-07-25  
**Approval and Implementation Record:** GOV-044  
**Owner:** Falcon Document Authority  
**Governing Authority:** GOV-001 v1.2; AMD-003; ROADMAP-001 v2.7; FRS-001 Readiness Report v4.0  
**Remediation Class:** Non-semantic document-control correction  
**Implementation Authority:** Not Granted

## 1. Purpose

This remediation aligns canonical Foundation document metadata and locations with GOV-001 v1.2 without changing approved meaning, requirements, decisions, status, authority, or technical content.

## 2. Audit Result

The controlled audit found:

- 157 Markdown files in active non-archive, non-candidate document locations;
- 62 governed documents whose identifier is present in the title and filename but absent from an explicit `Identifier` metadata field;
- 15 accepted ADRs that also lack an explicit `Version` field;
- one Approved AMD-003 Impact Review remaining in a candidate location;
- one founding Authority Registry whose approval basis is implicit in GOV-003 rather than explicitly recorded in metadata;
- two specification README files and one ADR template that are guidance or templates, not approved governed documents; and
- no evidence that these metadata conditions changed document meaning.

## 3. Remediation Scope

### 3.1 Explicit Identifier

Add an explicit `Identifier` field matching the immutable identifier already present in the title and canonical filename to:

- ADR-F001 through ADR-F008;
- ADR-I001 through ADR-I007;
- CON-001 through CON-007, CON-009, and CON-011;
- FDN-001 through FDN-005;
- GOV-AUT-001;
- GOV-SEC-001;
- SEC-PLAN-001;
- FRS-001 Foundation Release Specification;
- AUT-001;
- AUT-002;
- AWR-001;
- DEC-006;
- EVO-001;
- FCE-001;
- OPS-003;
- OPS-004;
- PLG-001;
- RSK-005;
- SEC-001;
- SEC-002;
- SYS-001;
- SYS-002;
- SYS-005;
- SYS-007 through SYS-011; and
- VPL-000 through VPL-008.

The inserted value SHALL exactly match the existing immutable title identifier.

### 3.2 ADR Version

Add:

```text
Version: 1.0
```

to ADR-F001 through ADR-F008 and ADR-I001 through ADR-I007.

This records their existing initial accepted version.

It does not create a new ADR version or alter an accepted decision.

### 3.3 AMD-003 Impact Review Canonical Location

Move the already Approved:

```text
AMD-003-IR-001 — Contract and VPL Impact Review
```

from the candidate location to:

```text
docs/amendments/reviews/
```

The filename, identifier, version, status, approval record, content, and meaning SHALL remain unchanged.

No candidate copy SHALL remain after the move.

### 3.4 Authority Registry Approval Basis

Add to GOV-000:

```text
Approval Basis: GOV-003 Foundation Ratification Record
```

This makes the existing founding ratification basis explicit.

It does not create or expand authority.

### 3.5 Guidance and Template Classification

The following SHALL remain outside governed approval counts:

- `docs/specifications/README.md`;
- `docs/specifications/core/README.md`; and
- `docs/adrs/ADR-TEMPLATE.md`.

They are guidance or templates.

They SHALL NOT receive fabricated governed identifiers, versions, approval records, or authority status through this remediation.

## 4. No-Version-Bump Rule

The remediation SHALL NOT increment affected document versions because it:

- copies an identifier already authoritative in the title;
- records the existing initial ADR version;
- records an existing ratification basis;
- corrects one canonical location; and
- changes no normative or decision content.

Any discovered change beyond these exact operations SHALL stop and require a separate versioned proposal.

## 5. Prohibited Changes

MDR-001 SHALL NOT:

- change a requirement;
- change an ADR decision;
- change a Contract;
- change a governed value;
- change an owner or authority;
- change status;
- change an effective date;
- change an approval decision;
- change supersession;
- change scope;
- change technical meaning;
- add implementation authority;
- activate any subject; or
- close Foundation or AMD-003.

## 6. Validation

After remediation, validation SHALL prove:

- every governed active document in scope has one explicit Identifier;
- every accepted Foundation ADR has `Version: 1.0`;
- every inserted Identifier matches title and filename;
- no duplicate canonical identifier is created;
- the Approved Impact Review exists only in its canonical review location;
- GOV-000 references GOV-003 as its existing approval basis;
- guidance and template files remain non-governing;
- all changed files remain valid UTF-8;
- body content after the metadata block remains unchanged;
- active status counts remain unchanged;
- requirement identifiers and counts remain unchanged;
- Approval and Implementation Authority states remain unchanged; and
- repository-wide links to the moved Impact Review are updated only where required.

## 7. Failure Rule

If:

- a title, filename, and intended identifier disagree;
- an ADR version cannot be established as 1.0;
- a document’s approval basis is materially ambiguous;
- a link update changes meaning;
- a duplicate identifier exists; or
- body content would need alteration,

remediation for that subject SHALL stop and be reported separately.

## 8. Effect of Approval

Approval of MDR-001 would authorize the exact non-semantic corrections in Section 3 and their mechanical validation.

It would not:

- approve new document meaning;
- approve a new technical version;
- change Foundation requirements;
- close the metadata audit before validation;
- close AMD-003;
- close Foundation;
- issue an Authority Instrument;
- authorize preparation or candidate execution;
- authorize implementation;
- authorize production;
- authorize cloud deployment;
- authorize financial connectivity; or
- authorize financial activity.

## 9. Approval

| Role | Decision | Name or Record | Date |
|---|---|---|---|
| Project Owner and current Constitutional Authority | Approved for execution | GOV-044 | 2026-07-25 |
