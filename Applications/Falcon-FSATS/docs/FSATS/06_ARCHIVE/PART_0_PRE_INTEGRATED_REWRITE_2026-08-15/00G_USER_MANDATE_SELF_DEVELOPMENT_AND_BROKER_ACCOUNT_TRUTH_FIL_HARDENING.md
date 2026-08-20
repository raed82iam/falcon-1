# FSATS Market Qualification Candidate - User-Mandate Self-Development and Broker/Account Truth FIL Hardening

**Package:** `FSATS-MARKET-QUALIFICATION-PROPOSAL-001`  
**Applies To:** `00 + 00A + 00B + 00C + 00D + 00E + 00F`  
**Decision Type:** `OWNER-DIRECTED SEMANTIC HARDENING / USER-MANDATE SELF-DEVELOPMENT / BROKER-ACCOUNT TRUTH / FIL EDGE NORMALIZATION`  
**Status:** `CONTROLLING CANDIDATE HARDENING / NOT_OWNER_ACCEPTED / NOT_CLOSED`  
**Branch:** `application-development`  
**Implementation / Runtime / Provider / Broker / Credential / Research-Egress / Advisory / Manual-Execution / Paper / Tiny Live / Live / Deployment Authority:** `NOT_GRANTED`

---

# 1. Purpose

This record materializes two Project Owner clarifications discovered after the R4 review gate:

1. Application self-development may improve capabilities so Falcon can serve a valid `User Trading Mandate (UTM)`, but self-development shall never modify, widen, bypass, reinterpret, or optimize away the UTM itself.
2. Broker support and exact client-account support are separate truths. Falcon shall not infer that an exact user account can perform an action merely because the broker product generally supports it. Broker-specific external facts shall be normalized through Falcon Internal Language (`FIL`) rather than leaking broker-native semantics into Trading business logic.

This file adds specificity to the seven-file R4 candidate without rewriting R4 history. Where this record refines `00F` or earlier NEW-3 wording, this record controls for the new candidate semantic state.

The R4 Architecture/Consistency and Red-Team results remain historical evidence for their exact reviewed seven-file freeze only.

---

# 2. Self-Development Serves the UTM but Cannot Rewrite It

The accepted Application Awareness boundary already limits self-development to improving the same already-authorized responsibility. This hardening applies that rule explicitly to the User Trading Mandate.

Mandatory invariant:

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

A better method inside the user's valid mandate may be developed as an isolated candidate and separately validated. A more permissive mandate may only come from a valid newer user authority state through the governed UTM change path.

Self-development success, profitability, confidence, repeated use, research evidence, broker capability, or technical feasibility does not create a broader UTM.

---

# 3. Horizon Adaptation Example

If the effective user mandate contains:

```text
MAX_POSITION_HOLDING = 7 DAYS
```

and the current Strategy Catalog contains only a strategy whose validated Intended Use requires a materially longer holding horizon, Falcon shall not mutate either the UTM or the strategy semantics merely to make them appear compatible.

Allowed direction:

```text
USER MAX HOLD = 7 DAYS
+
CURRENT STRATEGY MODE IS NOT VALIDATED FOR <= 7 DAYS

-> IDENTIFY CAPABILITY GAP
-> DEVELOP ISOLATED <= 7-DAY CANDIDATE IF JUSTIFIED
-> FSTSIMA / APPLICABLE VALIDATION
-> EVIDENCE
-> GOVERNED ADOPTION PATH
```

Forbidden shortcuts:

```text
CHANGE USER MAX HOLD TO 30 DAYS
```

or:

```text
FORCE-CLOSE A 30-DAY STRATEGY AT DAY 7
AND CALL THE ORIGINAL STRATEGY COMPLIANT
```

unless the central strategy has a separately validated <=7-day operating mode with its own Intended Use evidence.

---

# 4. Advisory-Only Self-Development Boundary

Where the effective interaction mode is `ADVISORY_ONLY`, self-development may improve capabilities that remain inside advisory authority, including as applicable:

- analysis quality;
- strategy applicability;
- recommendation quality;
- uncertainty calibration;
- Risk assessment;
- portfolio-impact assessment;
- explanation and attribution.

It shall not develop or activate a hidden execution bypass merely because automated execution would improve convenience or performance.

Mandatory invariant:

```text
EFFECTIVE MODE = ADVISORY_ONLY

SELF_DEVELOPMENT MAY IMPROVE ADVICE

BUT

SELF_DEVELOPMENT CANNOT CREATE,
SIMULATE,
OR SMUGGLE IN
EXECUTION AUTHORITY.
```

Discovery of a new broker, a new broker feature, a new API function, or a technically possible execution path does not change the user's UTM and does not by itself change the market's permitted interaction mode.

---

# 5. Broker Truth Is Layered

The earlier `BROKER_CAPABILITY` and `ACCOUNT_CAPABILITY` terms in `00F` are refined into distinct evidence classes.

Mandatory distinctions:

```text
BROKER_GENERAL_CAPABILITY
!=
EXACT_CLIENT_ACCOUNT_CAPABILITY
!=
CURRENT_BROKER_OPERATIONAL_STATE
!=
USER_TRADING_MANDATE
```

## 5.1 Broker General Capability

`BROKER_GENERAL_CAPABILITY` describes what a broker product/interface generally supports for the relevant product, market, environment or service class, as evidence permits.

Examples may include:

- supported markets/asset classes;
- supported order semantics;
- fractional-share support;
- extended-hours support;
- streaming/interface support;
- available account/product types;
- product-level restrictions or feature conditions;
- documented authentication/interface mechanisms.

General support is not proof that every client account is eligible to use the capability.

## 5.2 Exact Client Account Capability

`EXACT_CLIENT_ACCOUNT_CAPABILITY` describes what the exact authenticated user/broker-account/environment combination is currently eligible or permitted to use, as authoritative evidence permits.

It may include as applicable:

- exact account identity;
- environment such as Paper or Live;
- account type/class;
- trading permissions;
- account-specific restrictions;
- entitlements;
- feature enablement/disablement;
- exact market/product eligibility;
- broker-account operational restrictions relevant to execution.

An account capability from one user, account, environment or portfolio shall not be reused as capability truth for another.

## 5.3 Current Broker Operational State

`CURRENT_BROKER_OPERATIONAL_STATE` represents time-sensitive broker/account facts where the broker exposes trustworthy evidence, such as:

- service availability/degradation;
- temporary trading restriction;
- current operational quota/rate state;
- session/interface availability;
- temporary feature suspension;
- other time-sensitive execution constraints.

This state is not a substitute for broker general capability or account eligibility. It is an additional current constraint.

---

# 6. Public Broker Information, Private Account Access and Credentials Are Different Things

Mandatory distinctions:

```text
PUBLIC BROKER PRODUCT INFORMATION
!=
PRIVATE CLIENT ACCOUNT INFORMATION

API DOCUMENTATION / SPECIFICATION
!=
CLIENT CREDENTIAL

POSSESSION OF A CREDENTIAL
!=
TRADING AUTHORITY

BROKER GENERAL SUPPORT
!=
EXACT ACCOUNT SUPPORT
```

Broker/user credentials, authorization grants, OAuth tokens, API keys, gateway sessions or equivalent access artifacts are issued or authorized by the external broker/user process. Falcon does not fabricate, infer, extract, or self-create missing client credentials.

The existence of a public developer API or public documentation does not grant access to a private client account.

The existence of authenticated account access does not by itself grant autonomous trading. It only enables the separately authorized system to obtain or exercise whatever exact account access that credential/session legitimately permits, subject to every other Falcon authority gate.

No actual secret or credential value is defined or stored by this design candidate.

---

# 7. Source, Provenance, Freshness and Scope of Broker Truth

Every material broker/account capability claim used for qualification or future execution-readiness shall be attributable enough to distinguish its source and scope.

Conceptually, evidence shall preserve as applicable:

```text
BrokerIdentity
BrokerProductOrInterfaceIdentity
UserIdentity when account-specific
BrokerAccountIdentity when account-specific
Environment
CapabilityOrRestrictionIdentity
ObservedOrRetrievedAt
SourceClass
SourceEvidenceRef
VersionOrRevision when knowable
FreshnessOrRevalidationRule
CapabilityState
```

`CapabilityState` shall preserve at least the accepted semantic distinctions:

```text
SUPPORTED
UNSUPPORTED
CONDITIONALLY_SUPPORTED
UNKNOWN / UNVERIFIED
```

`UNKNOWN != SUPPORTED` remains controlling.

Evidence source classes may include, as separately authorized and applicable:

```text
AUTHORITATIVE_PUBLIC_BROKER_PRODUCT_EVIDENCE
AUTHENTICATED_EXACT_ACCOUNT_EVIDENCE
BROKER_RUNTIME_OPERATIONAL_EVIDENCE
GOVERNED_RESEARCH_EVIDENCE
```

Research evidence does not automatically become operational account truth.

---

# 8. Broker Truth Refresh Does Not Assume One Universal API Shape

Falcon shall not assume that all brokers expose the same endpoint, protocol, field names, authentication model, capability endpoint or event model.

Mandatory principle:

```text
COMMON FALCON TRUTH MODEL
DOES NOT REQUIRE
COMMON EXTERNAL BROKER MECHANISM
```

Each separately admitted broker integration may establish its evidence through the mechanisms actually supported by that broker, such as a REST interface, authenticated account interface, OAuth flow, gateway/session model, event stream, documented specification, or another separately governed method.

The architecture shall not hard-code an Alpaca-specific mechanism as the universal broker model.

Conceptual refresh/revalidation triggers may include, as applicable:

- initial broker/account binding;
- authenticated reconnect;
- account or environment change;
- stale capability snapshot;
- authoritative broker product change evidence;
- unexpected broker rejection conflicting with cached support;
- observed operational restriction/degradation;
- explicit requalification/revalidation.

The exact runtime scheduling, polling, subscriptions and caching policy are future implementation work and are not authorized by this record.

---

# 9. Restrictive Evidence Can Narrow Trust Faster Than Positive Evidence Can Widen Authority

If current broker evidence contradicts a previously trusted capability, Falcon shall not continue acting on the more permissive cached interpretation merely because it is convenient.

Conceptually:

```text
PREVIOUSLY SUPPORTED
+
CURRENT MATERIAL CONFLICT / REJECTION / RESTRICTION

-> REDUCE TRUST
-> UNKNOWN / CONDITIONAL / RESTRICTED AS EVIDENCE JUSTIFIES
-> HOLD AFFECTED NEW EXPOSURE OR RECONCILE
```

A newly discovered positive capability follows a stricter path:

```text
NEW FEATURE DISCOVERED
!=
FALCON MAY USE IT
```

A new or materially changed capability may require:

```text
DISCOVERY
-> EVALUATION
-> FIL / EXECUTION SEMANTIC COMPATIBILITY CHECK
-> ISOLATED CANDIDATE WORK IF NEEDED
-> FSTSIMA / APPLICABLE VALIDATION
-> EVIDENCE
-> GOVERNED ADOPTION / READINESS DECISION
```

No newly discovered broker capability may silently widen UTM, Risk, market scope, account authority, or system authority.

---

# 10. FIL Remains the Canonical Falcon Internal Language

This hardening does not introduce a new architectural language or a new broker-management Application.

FIL remains the Falcon internal contract/language boundary through which broker-specific external semantics are normalized for Falcon use.

Conceptually:

```text
FALCON TRADING BUSINESS SEMANTICS
-> FIL CANONICAL EXECUTION INTENT
-> BROKER-SPECIFIC EDGE TRANSLATION
-> BROKER-NATIVE INTERFACE
```

and on the return path:

```text
BROKER-NATIVE RESPONSE / EVENT / EVIDENCE
-> BROKER-SPECIFIC EDGE TRANSLATION
-> FIL-NORMALIZED BROKER / EXECUTION TRUTH
-> T-LSA-09 EXECUTION / RECONCILIATION
```

The broker-specific translator/adapter is an edge integration realization around FIL. It is not a new source of business authority, a new LSA, a new Application, or a replacement for FIL.

---

# 11. Broker Translation Shall Not Manufacture Truth

The broker-specific translation boundary may translate syntax, protocol, field names and broker-native states into governed FIL semantics.

It shall not:

- invent a broker acknowledgement;
- invent a fill;
- convert a request into an outcome;
- fabricate account eligibility;
- create a credential;
- broaden a UTM;
- broaden Risk authority;
- treat an unknown capability as supported;
- silently emulate an unsupported order semantic when that emulation changes Risk/protection/business meaning without separately reviewed design and authority.

The accepted execution truth distinctions remain controlling:

```text
ORDER_REQUEST != SUBMISSION_ATTEMPT
SUBMISSION_ATTEMPT != BROKER_ACK
BROKER_ACK != FILL
PARTIAL_FILL != FULL_FILL
CANCEL_REQUEST != CANCELLED
```

Broker-native vocabulary shall be mapped only as strongly as the source evidence supports.

---

# 12. FIL Shall Not Become a Catalog of Broker-Specific Quirks

A new broker-specific field or feature does not automatically require a new global FIL semantic.

If the external broker introduces a new way to perform an already represented Falcon semantic, the broker-specific translation may change while the FIL business meaning remains stable.

If the broker introduces a genuinely new business/execution semantic not represented in Falcon, the system shall determine through governed design whether:

- the concept is a generic Falcon semantic worthy of a governed FIL extension; or
- it remains a broker-specific capability/extension that does not belong in the canonical common language.

This prevents FIL from becoming coupled to the accumulated quirks of every broker.

---

# 13. Effective Trading Authority Refinement

The broader `BROKER_CAPABILITY` and `ACCOUNT_CAPABILITY` terms in `00F` are refined conceptually as follows:

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

Where an operational-state term is not applicable or cannot be established, it shall not be invented as permissive truth. The affected readiness/action shall remain bounded by the evidence actually available.

No intersection term can manufacture another term.

Examples:

```text
BROKER GENERALLY SUPPORTS FRACTIONAL TRADING
+
EXACT ACCOUNT DOES NOT SUPPORT IT
-> FRACTIONAL EXECUTION NOT AVAILABLE FOR THAT ACCOUNT
```

```text
EXACT ACCOUNT SUPPORTS AUTOMATED EXECUTION
+
UTM = ADVISORY_ONLY
-> EFFECTIVE MODE REMAINS ADVISORY_ONLY
```

```text
VALID CREDENTIAL EXISTS
+
EXACT ACCOUNT PERMISSION = READ_ONLY
-> NEW ORDER SUBMISSION NOT AUTHORIZED
```

---

# 14. Market Qualification Must Report Broker and Account Truth Separately

The Owner-facing Market Qualification result shall not collapse broker product support and exact-account support into one ambiguous `Broker = Supported` label.

As applicable to the requested intended use, the result shall distinguish at least conceptually:

```text
Broker General Capability:
  SUPPORTED / CONDITIONAL / UNSUPPORTED / UNKNOWN

Exact Client Account Capability Path:
  VERIFIED / CONDITIONAL / UNAVAILABLE / UNVERIFIED / NOT_REQUIRED_FOR_CURRENT_ADVISORY_STUDY

Authenticated Account Connection Requirement:
  REQUIRED_FOR_NEXT_STAGE / NOT_REQUIRED_FOR_CURRENT_STUDY / NOT_AVAILABLE / NOT_AUTHORIZED

Current Operational Constraints:
  KNOWN / CONDITIONAL / UNKNOWN / NOT_APPLICABLE

Maximum Evidence-Backed Interaction Mode:
  <bounded result>
```

For advisory-only qualification, absence of a private execution-account connection does not automatically destroy valid advisory value when the required data/access/advisory evidence is independently sufficient.

For an execution-readiness claim tied to an exact account, public broker-product documentation alone is not enough when account-specific eligibility/restrictions are material.

---

# 15. Ownership Mapping

This hardening creates no new Application, LSA or Awareness tier.

Current ownership remains:

- `T-LSA-01` owns Trading-side user/account/environment context, broker-account bindings as business context and account readiness awareness/evaluation.
- `T-LSA-02` owns market/venue facts, Market Profiles and market-specific access/constraint facts.
- `T-LSA-06` consumes the resulting current constraints for strategy eligibility and decision construction.
- `T-LSA-07` remains Unified Risk owner.
- `T-LSA-08` remains portfolio/capital owner.
- `T-LSA-09` owns Trading execution business semantics, broker order lifecycle, broker/account capability interpretation required for execution, FIL-normalized execution truth and reconciliation.
- `T-LSA-12` may coordinate isolated Trading candidate evolution but cannot alter UTM or promote its own candidate.
- `FSTSimA S-LSA-04` remains the broker/exchange/execution simulation and validation branch for non-Live evidence as applicable.
- `FSAPMA` remains provider/data business owner and is not converted into broker-execution owner.
- `Guardian` may restrict/protect but cannot widen UTM or broker/account capability.
- `P1-K` remains the future Part 1 work-package home for governed contract/FIL/event/route materialization. This record does not implement P1-K.
- Foundation remains owner of generic security, credential/secret governance, lifecycle, communication, egress and other OS/platform controls. Application design shall not invent Foundation internals.

---

# 16. Historical Review Boundary and New Review Requirement

The exact R4 semantic freeze remains:

```text
8b06940513e8ffba97d62a2589cd584e250ed7e8
```

Its Architecture/Consistency `PASS` and Red-Team `120/120 PASS` remain historically valid for the seven-file R4 state.

Because this `00G` file changes candidate semantics after that review:

```text
R4 PASS != CURRENT PASS FOR 00G
R4 OWNER GATE != FINAL GATE FOR THE NEW SEMANTIC STATE
```

The new candidate composition is:

```text
00
+ 00A
+ 00B
+ 00C
+ 00D
+ 00E
+ 00F
+ 00G
```

This exact eight-file state requires a new semantic freeze, fresh Architecture/Consistency review, fresh Red-Team review, and a new Project Owner review gate before final acceptance.

---

# 17. Explicit Non-Grant

This hardening grants no:

- implementation authority;
- runtime UTM storage/enforcement;
- provider connectivity;
- broker connectivity;
- authenticated client-account connection;
- credential creation/import/storage/use;
- research Internet egress;
- operational advisory service;
- manual-confirmation execution;
- autonomous execution;
- Paper;
- Tiny Live;
- Live;
- deployment;
- market admission;
- legal/licensing authority;
- new FIL runtime contract by implication;
- P1-F or P1-K implementation authority;
- self-development production adoption.

```text
DOCUMENTED BROKER / ACCOUNT TRUTH MODEL
!=
BROKER CONNECTION AUTHORIZED
```

and:

```text
SELF-DEVELOPMENT CAPABILITY
!=
SELF-GOVERNANCE AUTHORITY
```
