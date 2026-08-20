# FSATS Market Qualification Candidate — Pre-Review Market Access, Scope and Value Completeness Hardening

**Package:** `FSATS-MARKET-QUALIFICATION-PROPOSAL-001`  
**Applies To:** `00` + `00A` of this NEW-3 package  
**Decision Type:** `PRE-REVIEW COMPLETENESS HARDENING`  
**Status:** `CONTROLLING CANDIDATE HARDENING / NOT_OWNER_ACCEPTED / NOT_CLOSED`  
**Branch:** `application-development`  
**Implementation / Runtime / Paper / Tiny Live / Live / Deployment Authority:** `NOT_GRANTED`

---

# 1. Purpose

This record makes explicit several market-qualification dimensions that are implicit in the accepted Market Profile / Risk / execution model but are too important to leave hidden inside `as applicable` wording.

Where this record adds specificity to `00`, it controls for the reviewed NEW-3 candidate set.

---

# 2. Market Access and Rule Qualification

A new-market qualification shall study and preserve as applicable:

- exchange/venue access rules;
- participant/account eligibility constraints;
- jurisdiction/market-access restrictions relevant to the intended use;
- trading-session and auction rules;
- permitted/prohibited order and transaction types;
- short-sale/borrow/margin restrictions where applicable;
- price-limit, halt, circuit-breaker and exceptional-market rules;
- settlement-cycle and settlement-failure implications;
- custody/funding/currency-conversion constraints where material;
- market fees, transaction charges, taxes/withholding or equivalent costs where material to Trading economics or Risk;
- corporate-action/issuer-event mechanics where relevant;
- market-data usage/entitlement restrictions that affect the intended use.

Unknown material access/rule conditions shall remain `UNKNOWN` and block or narrow readiness rather than being guessed.

This is Trading/market qualification evidence. It does not make Falcon the legal or regulatory authority and does not bypass any external legal/licensing requirement.

---

# 3. Unsupported Exposure / Instrument Model Gate

The current accepted initial Trading scope remains bounded to its admitted funded-exposure model and current market/instrument scope.

If Market X can only be used as intended by requiring a materially new capability such as:

- leverage beyond the admitted funded model;
- margin borrowing;
- options;
- futures;
- derivatives;
- leveraged tokens/equivalents;
- uncovered shorting/borrow obligations;
- another materially different capital/exposure model;

then qualification SHALL NOT silently add that authority.

The correct result is conceptually:

```text
MARKET TECHNICALLY INTERESTING
BUT REQUIRED EXPOSURE / INSTRUMENT CAPABILITY OUTSIDE CURRENT AUTHORIZED SCOPE
-> SEPARATE SCOPE EXPANSION REQUIRED
-> CURRENT MARKET QUALIFICATION CANNOT CLAIM READY_FOR_PAPER_REVIEW FOR THAT OUT-OF-SCOPE USE
```

A narrower in-scope use may be qualified only if it is semantically valid and explicitly represented as narrower.

---

# 4. Economic and Operational Value Case

Technical compatibility alone does not require Falcon to add a market.

The qualification package should include an evidence-backed, uncertainty-aware market value case as applicable, considering:

- opportunity breadth/frequency;
- liquidity and capacity;
- strategy opportunity fit;
- diversification/correlation contribution;
- expected data/provider/broker cost burden;
- execution friction;
- capital efficiency inside the authorized model;
- operational complexity;
- resource demand;
- protection/reconciliation burden;
- reliability/dependency burden;
- residual uncertainty and downside.

This value case SHALL NOT be represented as a guaranteed profitability forecast.

Mandatory distinction:

```text
MARKET IS TECHNICALLY SUPPORTABLE
!=
MARKET IS WORTH ADMITTING
```

Trading MSA may recommend rejection even after technical qualification if the evidence-supported benefit does not justify the operational/risk burden.

---

# 5. Readiness Consequence

`READY_FOR_PAPER_REVIEW` requires that material access/rule/scope constraints be either:

- known and compatible with the exact intended Paper use; or
- explicitly bounded in a way that does not make the readiness claim false.

It also requires that no hidden out-of-scope exposure model is necessary for the claimed intended use.

A material unknown, legal/access incompatibility, unsupported instrument/exposure dependency, or unacceptable value/risk tradeoff can produce:

```text
HOLD_RETEST
INSUFFICIENT_EVIDENCE
SCOPE_EXPANSION_REQUIRED
MARKET_ACCESS_INCOMPATIBLE
NOT_READY
REJECT_MARKET_CANDIDATE
```

---

# 6. Owner-Facing Summary Addition

The final Market Qualification result shall summarize, as applicable:

```text
Market Access / Rules:
  COMPATIBLE / CONDITIONAL / BLOCKED / UNKNOWN

Exposure / Instrument Scope:
  WITHIN CURRENT SCOPE / NARROWED / SEPARATE EXPANSION REQUIRED

Operational / Economic Value Case:
  SUPPORTS ADMISSION REVIEW / MARGINAL / DOES NOT JUSTIFY ADMISSION / INSUFFICIENT EVIDENCE
```

The summary shall identify the basis and material uncertainty behind these labels.

---

# 7. Reviewed-Candidate Composition Update

The candidate semantic set for fresh review is now:

```text
00_GOVERNED_MARKET_QUALIFICATION_AND_EXPANSION_LIFECYCLE_CANDIDATE.md
+
00A_PRE_REVIEW_AUTHORITY_AND_OWNER_COMMAND_RUNTIME_HARDENING.md
+
00B_PRE_REVIEW_MARKET_ACCESS_SCOPE_AND_VALUE_COMPLETENESS_HARDENING.md
```

Fresh review must bind to the exact unchanged commit containing all three files.

---

# 8. Non-Grant

This hardening does not grant legal status, licensing, market access, leverage, derivatives, shorting, implementation, runtime connectivity, Paper, Tiny Live, Live, deployment or market admission authority.
