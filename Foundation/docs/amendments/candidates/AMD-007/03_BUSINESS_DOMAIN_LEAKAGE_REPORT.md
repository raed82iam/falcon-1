# Business-Domain Leakage Report

**Status:** Proposed  
**Scanned Scope:** active Core Specifications, Contracts, Foundation records, ADRs, and `src`

## Classification Method

Occurrences were classified by semantic meaning, not keyword alone. For example:

- `field order` is technical ordering;
- `byte position` is technical location;
- `key inventory` is security inventory;
- a statement prohibiting Trading logic is legitimate architecture explanation.

## Confirmed Leakage Requiring Migration

| File / section | Occurrence | Classification | Treatment |
|---|---|---|---|
| `AWR-001 v1.0` §§1,2,4,6 | financial state, exposure, capital/decision context | Forbidden Foundation business knowledge | preserve history; activate FSA-only v2.0 |
| `AUT-002 v1.0` §§1,3,6 | threatens capital; trade/allocate capital; pursue profit | mixed Application-domain protection | preserve history; activate AUT-002 v2.1 and RSK-006 |
| `FDN-005` §§2,3,5 | broker, venue, order, live capital, Trading, portfolio, prediction | Foundation-specific bootstrap policy containing financial examples | version to generic prohibited external/business-authority paths; retain historical evidence |

## Legitimate References

- Vision/Constitution capital obligations cited by Foundation ADRs are legitimate governing context.
- prohibitions such as “Kernel SHALL NOT contain market policy” are legitimate boundary statements.
- Stage 0 financial-isolation statements are legitimate evidence and restrictions, not business logic.
- references to future capital consequence in security/evolution are cross-domain impact classifications, not Foundation ownership.
- `Decision Ledger` is a governed Falcon document term.

## False-Positive Technical Terms

`order`, `position`, `loss`, `inventory`, `subscription`, `market`, and `strategy` frequently mean field ordering, byte position, information loss, asset inventory, event subscription, ecosystem marketplace, or technical approach.

## Source Audit

No business-domain logic was found in `src`.

Matches for `OrderBy`, `Disposition`, and similar terms are programming/technical vocabulary. No trade, capital, portfolio, broker, invoice, patient, customer, or Accounting behavior exists.

## Required Rule

Foundation schemas and code SHALL reject Application-specific branching such as:

- `if application == trading`;
- `if payload == capital`;
- `if application == accounting`.

Foundation may route only by governed identity, capability, Contract, authority, classification, and technical metadata.

