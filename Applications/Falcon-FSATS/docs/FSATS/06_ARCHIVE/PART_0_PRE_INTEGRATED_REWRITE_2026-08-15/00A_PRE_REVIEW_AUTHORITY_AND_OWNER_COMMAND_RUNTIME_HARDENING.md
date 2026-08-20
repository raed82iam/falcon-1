# FSATS Market Qualification Candidate — Pre-Review Authority and Owner-Command Runtime Hardening

**Package:** `FSATS-MARKET-QUALIFICATION-PROPOSAL-001`  
**Applies To:** `00_GOVERNED_MARKET_QUALIFICATION_AND_EXPANSION_LIFECYCLE_CANDIDATE.md`  
**Decision Type:** `PRE-REVIEW SEMANTIC HARDENING`  
**Status:** `CONTROLLING CANDIDATE HARDENING / NOT_OWNER_ACCEPTED / NOT_CLOSED`  
**Branch:** `application-development`  
**Implementation Authority:** `NOT_GRANTED`  
**Runtime / Paper / Tiny Live / Live / Deployment Authority:** `NOT_GRANTED`

---

# 1. Purpose

This record closes two authority ambiguities identified before the formal Architecture/Consistency review of the new-market qualification candidate:

1. the R7 `DevelopmentChangeClassification (DCC)` model is itself still an unaccepted candidate and therefore cannot be presented as current accepted authority;
2. the Application-side meaning of an Owner command can be defined now, but a future Shared-Web/browser/mobile runtime command path cannot be claimed to exist while the generic Owner-authentication and inbound-command admission boundary remains unresolved/partial.

Where this record conflicts with wording in `00`, this record controls for the reviewed NEW-3 candidate set.

---

# 2. DCC Classification Is Candidate-Conditional, Not Current Accepted Law

`00` labels the new-market qualification as:

```text
DCC-3 — MATERIAL_DOMAIN_CHANGE
```

That classification is correct **under the current R7 candidate classification model**, because a new market changes market/domain scope and may affect Risk, capital, broker/execution and other protected dimensions.

However, R7 remains:

```text
SEMANTIC_FREEZE_R7 / NOT_OWNER_ACCEPTED / NOT_CLOSED
```

Therefore the controlling statement is:

```text
IF THE R7 DCC MODEL IS LATER OWNER-ACCEPTED WITHOUT A CONFLICTING CHANGE
THEN NEW-MARKET QUALIFICATION / MARKET-SCOPE EXPANSION = DCC-3 MINIMUM
```

Until then, the authoritative current accepted rule is independent of the DCC label:

```text
NEW MARKET = MATERIAL GOVERNED TRADING SCOPE EXPANSION
-> REQUIRES EXPLICIT BOUNDED AUTHORITY
-> REQUIRES APPLICABLE MARKET PROFILE / RISK / EXECUTION / VALIDATION / FOUNDATION-DEPENDENCY / OWNER-GOVERNANCE REVIEW
-> DOES NOT SELF-ADMIT OR SELF-PROMOTE
```

No worker may use the unaccepted R7 DCC vocabulary as a source of authority.

The NEW-3 candidate may use `DCC-3` only as a **candidate cross-reference/classification prediction** against R7, not as accepted governance.

---

# 3. No 24-Hour / No-Veto Path for This Market Expansion Candidate

Even if R7 is later accepted, its own candidate semantics classify DCC-3/4/5 as not timer/no-veto eligible.

Therefore NEW-3 preserves:

```text
OWNER SILENCE != MARKET ADOPTION
TIMER EXPIRY != MARKET ADOPTION
QUALIFICATION PASS != MARKET ADOPTION
FSA REVIEW != MARKET ADOPTION
```

The Project Owner's `ADD MARKET X` instruction authorizes only the bounded qualification mandate described by NEW-3.

It does not pre-delegate later Paper/Tiny Live/Live or market-admission authority.

---

# 4. Application Meaning vs Runtime Command Transport

NEW-3 defines the Application-owned business meaning of:

```text
ADD MARKET X
```

as:

```text
START BOUNDED NON-LIVE MARKET QUALIFICATION
```

This semantic definition does **not** establish that a Shared-Web/browser/mobile runtime command path currently exists.

The current cross-workstream state includes:

- `FCR-0077` — Shared Web/Application planning coordination for the Owner conversational workflow;
- `FCR-0076` — Foundation-side generic Owner authentication / Web-browser inbound command-admission capability is partial and remains separately governed.

Therefore:

```text
APPLICATION KNOWS WHAT THE COMMAND MEANS
!=
RUNTIME WEB COMMAND PATH EXISTS
```

and:

```text
UI TEXT / AI NORMALIZATION / CHAT MESSAGE
!=
AUTHENTICATED OWNER AUTHORITY
!=
ADMITTED APPLICATION COMMAND
```

---

# 5. Runtime Fail-Closed Rule

Before any future authority-bearing conversational implementation may execute `ADD MARKET X`, the system must have the applicable governed capabilities/contracts for at least:

- Owner identity/authentication and attribution;
- authorization context;
- inbound Web/browser/mobile command admission;
- exact target Application identity;
- immutable/correlated original Owner wording and normalized command representation;
- Application-owned interpretation/result semantics;
- rejection/fail-closed behavior;
- evidence/audit/correlation;
- high-consequence step-up authorization where required;
- revocation/expiry where applicable.

If the generic Foundation boundary or exact Application/Web binding is unavailable:

```text
RUNTIME OWNER COMMAND EXECUTION = FAIL CLOSED / NOT AVAILABLE
```

The design candidate may still exist and be reviewed statically.

---

# 6. Web AI Cannot Manufacture Authority

Shared Web may eventually use Web-owned AI to normalize, classify or present Owner intent, but it SHALL NOT:

- change `ADD MARKET X` into `START PAPER MARKET X`;
- add provider/broker connectivity permission;
- add credentials or egress authority;
- silently change the target market/asset class;
- widen qualification ceiling;
- convert a recommendation into an authorization;
- convert Owner silence into approval.

Mandatory invariant:

```text
WEB NORMALIZATION = PRESENTATION / TRANSPORT SUPPORT
WEB NORMALIZATION != TRADING BUSINESS AUTHORITY
```

---

# 7. Reviewed-Candidate Composition

The candidate to be frozen for fresh review is now the semantic composition of:

```text
00_GOVERNED_MARKET_QUALIFICATION_AND_EXPANSION_LIFECYCLE_CANDIDATE.md
+
00A_PRE_REVIEW_AUTHORITY_AND_OWNER_COMMAND_RUNTIME_HARDENING.md
```

The earlier single-file commit is not the final reviewed freeze.

A fresh Architecture/Consistency review and fresh Red-Team review must bind to the exact commit containing both files unchanged.

---

# 8. Non-Grant

This hardening grants no:

- R7 acceptance;
- NEW-3 acceptance;
- implementation;
- runtime command path;
- research egress;
- provider/broker connectivity;
- Paper;
- Tiny Live;
- Live;
- deployment;
- market admission;
- autonomous promotion.

```text
DOCUMENTED OWNER INTENT != IMPLEMENTED AUTHORITY PATH
```
