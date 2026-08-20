# FSATS Market Qualification R5 - Project Owner Review Gate

**Package:** `FSATS-MARKET-QUALIFICATION-PROPOSAL-001`  
**Candidate Semantic Freeze:** `d1f4bc411e6aba46c08a8784f7d2f95c5311e9c7`  
**Architecture / Consistency:** `PASS`  
**Architecture Review:** `01D_FRESH_ARCHITECTURE_AND_CONSISTENCY_REVIEW_R5.md`  
**Fresh Red-Team:** `155 / 155 PASS`  
**Red-Team Review:** `02B_FRESH_RED_TEAM_REVIEW_R5.md`  
**Critical Open:** `0`  
**High Open:** `0`  
**Medium Open:** `0`  
**Post-Review Semantic Change:** `NONE`  
**Status:** `READY_FOR_PROJECT_OWNER_FINAL_REVIEW / NOT_OWNER_ACCEPTED / NOT_CLOSED`  
**Implementation / Runtime / Provider / Broker / Credential / Advisory / Manual-Execution / Research-Egress / Paper / Tiny Live / Live / Deployment Authority:** `NOT_GRANTED`

---

# 1. Exact Candidate Presented to Owner

The exact semantic candidate presented for final Owner decision is the unchanged eight-file set at commit:

```text
d1f4bc411e6aba46c08a8784f7d2f95c5311e9c7
```

Files:

```text
00_GOVERNED_MARKET_QUALIFICATION_AND_EXPANSION_LIFECYCLE_CANDIDATE.md
00A_PRE_REVIEW_AUTHORITY_AND_OWNER_COMMAND_RUNTIME_HARDENING.md
00B_PRE_REVIEW_MARKET_ACCESS_SCOPE_AND_VALUE_COMPLETENESS_HARDENING.md
00C_PRE_RED_TEAM_BOUNDED_AUTONOMY_RESOURCE_COST_AND_RESEARCH_SECURITY_HARDENING.md
00D_PRE_REVIEW_CANDIDATE_ISOLATION_AND_CROSS_MARKET_REGRESSION_HARDENING.md
00E_PRE_REVIEW_REQUEST_IDENTITY_IDEMPOTENCY_AND_REPLAY_HARDENING.md
00F_USER_TRADING_MANDATE_HORIZON_AND_ADVISORY_ONLY_MARKET_HARDENING.md
00G_USER_MANDATE_SELF_DEVELOPMENT_AND_BROKER_ACCOUNT_TRUTH_FIL_HARDENING.md
```

Git comparison from the semantic freeze through completed R5 review shows only the R5 review artifacts added after the freeze:

```text
01D_FRESH_ARCHITECTURE_AND_CONSISTENCY_REVIEW_R5.md
02B_FRESH_RED_TEAM_REVIEW_R5.md
```

No semantic candidate file changed after the R5 freeze.

The R4 gate remains preserved as historical evidence for the prior seven-file candidate and is not rewritten.

---

# 2. R4 User Trading Mandate Semantics Remain Preserved

R5 retains the R4 prospective User Trading Mandate model:

```text
USER CHOOSES HOW FALCON MAY PARTICIPATE WITH THE USER'S CAPITAL.
FALCON CHOOSES HOW BEST TO OPERATE INSIDE THAT MANDATE.
FALCON SHALL NOT EXPAND THE USER'S MANDATE.
```

The UTM remains versioned, attributable and scoped by user/portfolio/broker/account/market as applicable.

Interaction Mode and Trading Horizon Policy remain separate axes.

---

# 3. New Self-Development / UTM Rule

R5 adds the explicit rule:

```text
SELF_DEVELOPMENT MAY ADAPT CAPABILITIES
TO SERVE A VALID USER TRADING MANDATE

BUT

SELF_DEVELOPMENT SHALL NOT MODIFY,
WIDEN,
BYPASS,
REINTERPRET,
OR OPTIMIZE AWAY
THE USER TRADING MANDATE.
```

Example preserved by the candidate:

```text
USER MAX HOLD = 7 DAYS
+
CURRENT STRATEGY REQUIRES LONGER HOLD

-> DEVELOP A SEPARATELY VALIDATED <= 7-DAY CANDIDATE IF JUSTIFIED

NOT

-> CHANGE USER MANDATE
-> OR SILENTLY TRUNCATE THE ORIGINAL LONGER-HORIZON STRATEGY
```

For `ADVISORY_ONLY`, self-development may improve analysis/advice/Risk assessment but may not create a hidden execution path.

---

# 4. Broker General Capability and Exact Client Account Capability Are Separate Truths

R5 explicitly distinguishes:

```text
BROKER_GENERAL_CAPABILITY
!=
EXACT_CLIENT_ACCOUNT_CAPABILITY
!=
CURRENT_BROKER_OPERATIONAL_STATE
!=
USER_TRADING_MANDATE
```

`BROKER_GENERAL_CAPABILITY` represents evidence-backed product/interface support at the broker level.

`EXACT_CLIENT_ACCOUNT_CAPABILITY` represents the exact user/broker-account/environment eligibility, permissions, restrictions and entitlements when authoritative account evidence exists.

`CURRENT_BROKER_OPERATIONAL_STATE` represents time-sensitive execution constraints where trustworthy broker evidence exists.

Therefore:

```text
BROKER SUPPORTS X
!=
EVERY CLIENT ACCOUNT MAY USE X
```

---

# 5. Public API, Private Account Access and Credentials Are Separate

R5 preserves these mandatory distinctions:

```text
PUBLIC BROKER PRODUCT INFORMATION
!= PRIVATE CLIENT ACCOUNT INFORMATION

API DOCUMENTATION / SPECIFICATION
!= CLIENT CREDENTIAL

POSSESSION OF A CREDENTIAL
!= TRADING AUTHORITY
```

Broker/user credentials or authorization artifacts are externally issued/authorized and separately governed.

Falcon does not fabricate missing private credentials, and public broker documentation does not grant access to a client account.

No actual credential or secret value is created or stored by this design candidate.

---

# 6. Broker Truth Has Source, Scope and Freshness

R5 requires material broker/account capability claims to preserve enough evidence to establish as applicable:

- broker/product identity;
- user/account identity when account-specific;
- environment;
- capability/restriction;
- source class/evidence reference;
- observed/retrieved time;
- version/revision when knowable;
- freshness/revalidation rule;
- capability state.

The existing capability states remain:

```text
SUPPORTED
UNSUPPORTED
CONDITIONALLY_SUPPORTED
UNKNOWN / UNVERIFIED
```

and:

```text
UNKNOWN != SUPPORTED
```

---

# 7. Broker Integration Is Generic in Falcon but Broker-Specific at the Edge

R5 does not assume every broker exposes the same endpoint, authentication flow, protocol or event model.

The architecture preserves:

```text
COMMON FALCON TRUTH MODEL
DOES NOT REQUIRE
COMMON EXTERNAL BROKER MECHANISM
```

A broker integration may use the separately governed mechanism actually supported by that broker.

Falcon is therefore not architecturally bound to an Alpaca-specific external model.

---

# 8. FIL Remains the Canonical Internal Language

R5 does not introduce a new language or a new Broker Application.

The broker-specific translator/adapter is treated as the external edge translation around the already chosen FIL model:

```text
FALCON TRADING BUSINESS SEMANTICS
-> FIL CANONICAL INTENT
-> BROKER-SPECIFIC EDGE TRANSLATION
-> BROKER NATIVE INTERFACE
```

and:

```text
BROKER NATIVE RESPONSE / EVIDENCE
-> BROKER-SPECIFIC EDGE TRANSLATION
-> FIL-NORMALIZED TRUTH
-> T-LSA-09 EXECUTION / RECONCILIATION
```

The translator does not own business authority, credentials, FIL platform governance or outcome truth.

---

# 9. FIL Is Protected From Broker-Specific Pollution

A broker adding a new field/function does not automatically force a new global FIL semantic.

If the broker provides a new external implementation of an existing Falcon concept, the edge translation may change while FIL stays stable.

If a genuinely new generic Falcon execution concept is discovered, a governed FIL extension may be proposed separately.

Broker-specific quirks do not automatically become global Falcon language.

---

# 10. Effective Trading Authority Is Refined

R5 refines the R4 broker/account terms as:

```text
EFFECTIVE_TRADING_AUTHORITY
=
APPLICABLE_USER_TRADING_MANDATE
INTERSECT MARKET_RULES
INTERSECT MARKET_ACCESS / REGULATORY_CONSTRAINTS
INTERSECT BROKER_GENERAL_CAPABILITY
INTERSECT EXACT_CLIENT_ACCOUNT_CAPABILITY
INTERSECT CURRENT_BROKER_OPERATIONAL_STATE
INTERSECT STRATEGY_VALIDATED_INTENDED_USE
INTERSECT TRADING_HORIZON_POLICY
INTERSECT UNIFIED_RISK
INTERSECT CAPITAL_AVAILABILITY / RESERVATION
INTERSECT GUARDIAN_RESTRICTIONS
INTERSECT CURRENT_SYSTEM_AUTHORITY
```

No term can manufacture another term.

A broker/account may support more than the user permits, but the UTM remains the ceiling.

---

# 11. Restrictive Evidence and New Positive Capabilities

R5 protects against stale permissive broker state.

A material current conflict/restriction may reduce affected trust and force narrower/unknown/reconciliation handling.

Conversely:

```text
NEW BROKER FEATURE DISCOVERED
!=
FALCON MAY USE IT
```

A materially new capability may require evaluation, FIL/execution compatibility analysis, candidate work, FSTSimA/applicable validation, evidence and governed adoption/readiness before use.

---

# 12. Market Qualification Result Is More Precise

The Owner-facing qualification result may now distinguish:

```text
Broker General Capability
Exact Client Account Capability Path
Authenticated Account Connection Requirement
Current Operational Constraints
Maximum Evidence-Backed Interaction Mode
```

A private execution-account connection is not automatically required to prove valid advisory-only value when execution is outside the intended study.

But an exact-account execution readiness claim cannot rely on public broker documentation alone when account-specific eligibility is material.

---

# 13. Ownership Preserved

R5 creates no new Application, LSA or Awareness tier.

It preserves:

- T-LSA-01 user/account/environment/broker-account context and readiness;
- T-LSA-02 Market Profile/market-rule ownership;
- T-LSA-06 strategy eligibility and decision construction;
- T-LSA-07 Unified Risk;
- T-LSA-08 portfolio/capital;
- T-LSA-09 execution business semantics, broker/account capability interpretation and execution reconciliation;
- T-LSA-12 isolated candidate evolution without UTM mutation;
- FSTSimA S-LSA-04 broker/exchange/execution non-Live simulation;
- FSAPMA provider/data ownership;
- Guardian independent protection;
- P1-K as future contract/FIL/event/route materialization work;
- Foundation generic security/secret/egress/lifecycle/communication/platform ownership where applicable.

T-LSA-09 does not become owner of Foundation FIL platform governance.

---

# 14. Fresh Review Result

Architecture/Consistency R5:

```text
RESULT = PASS
CRITICAL = 0
HIGH = 0
MEDIUM = 0
```

Fresh static Red-Team R5:

```text
R4 FRESH REGRESSION = 120 / 120 PASS
NEW R5 ATTACKS = 35 / 35 PASS
TOTAL = 155 / 155 PASS
FAIL = 0
OPEN CRITICAL/HIGH/MEDIUM = 0
```

The new R5 attacks cover:

- broker-product support vs exact account support;
- cross-user/account/environment capability leakage;
- Paper vs Live separation;
- credentials vs authority;
- stale/revoked access evidence;
- broker truth freshness and conflicts;
- new broker feature discovery;
- non-Alpaca protocol differences;
- FIL translation truth inflation;
- unsupported semantic emulation;
- FIL vendor pollution;
- T-LSA-09 / Foundation FIL ownership confusion;
- UTM-changing self-development;
- advisory execution bypass;
- mode-specific qualification readiness.

No semantic change occurred after the R5 semantic freeze.

---

# 15. Current Non-Authority

Even if the Project Owner accepts R5, documentary acceptance alone will not grant:

- implementation;
- runtime UTM storage/enforcement;
- provider/broker connectivity;
- authenticated user account access;
- credential creation/import/storage/use;
- operational advisory runtime;
- user-confirmed execution runtime;
- autonomous execution;
- research Internet egress;
- Paper;
- Tiny Live;
- Live;
- deployment;
- market admission;
- legal/licensing authority;
- automatic self-development promotion;
- Foundation FIL/platform ownership to Trading.

Every future runtime capability remains separately governed.

---

# 16. Exact Owner Decision Required

The pending final decision is:

```text
ACCEPT the exact reviewed Market Qualification R5 semantic freeze

d1f4bc411e6aba46c08a8784f7d2f95c5311e9c7

as the controlling prospective FSATS Market Qualification design for:

1. governed User Trading Mandates by user/portfolio/broker/account/market scope;
2. separate Interaction Mode and Trading Horizon Policy;
3. mandate-version/stale-authority protection;
4. advisory-only operation semantics;
5. market qualification that preserves advisory value when automated execution is unavailable/prohibited;
6. mode-specific market readiness outcomes;
7. explicit prohibition on self-development changing/widening/bypassing the UTM;
8. separate Broker General Capability, Exact Client Account Capability and Current Broker Operational State truths;
9. source/provenance/freshness requirements for broker/account capability evidence;
10. broker-specific external integration normalized through existing FIL semantics;
11. credential/access material remaining separate from Trading authority;
12. refined effective-authority intersection and Owner-facing broker/account qualification reporting.
```

or:

```text
REQUEST CHANGES
```

If the Owner requests any further semantic change, the changed candidate must receive a new semantic freeze and fresh Architecture/Consistency + fresh Red-Team before final acceptance.

No Owner acceptance is recorded by this gate itself.
