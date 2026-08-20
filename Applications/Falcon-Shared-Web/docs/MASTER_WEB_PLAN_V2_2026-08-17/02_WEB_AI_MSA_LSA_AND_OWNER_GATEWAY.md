# 02 — Web AI, MSA, Single LSA and Owner Gateway

**Status:** `OWNER-APPROVED RESPONSIBILITY MODEL / IMPLEMENTATION RECONCILIATION REQUIRED`  
**Source decision:** `OWNER_DIRECTION_WEB_MSA_LSA_2026-08-17.md`

# 1. Purpose

Shared Falcon Web uses a deliberately limited awareness model that is different from FSATS self-development behavior.

The Web AI exists to make Falcon understandable and operable to humans, to maintain the Web within approved bounds, to execute Web-owned development only when directly ordered by the Project Owner, and to route requests to the correct Falcon owner.

# 2. Awareness topology

Initial Web awareness model:

```text
SHARED FALCON WEB
│
├── 1 × WEB MSA
│
└── 1 × CUSTOMER INTERACTION & SUPPORT LSA
```

No additional Web LSA/CSA is introduced by this Master Plan without a later governed need and Owner decision.

# 3. Web MSA responsibilities

The Web MSA is the main Web awareness entity.

It may:

- understand Shared Web end-to-end within Web scope;
- observe Web health/readiness from available authoritative and Web-local evidence;
- diagnose Web-owned failures;
- perform bounded pre-authorized self-maintenance inside Web scope;
- identify whether a Project Owner request is Web-owned, FSATS-owned, Foundation-owned or another Application-owned request;
- preserve and structure Owner intent;
- route/handoff non-Web requests through the governed owning-workstream mechanism;
- execute Web-owned development only after a direct authenticated Project Owner instruction;
- validate and report Web-owned change results;
- present incoming Falcon reports/results to the Owner without becoming their source authority.

# 4. Web MSA self-maintenance vs development

This distinction is mandatory:

```text
WEB_MSA_SELF_MAINTENANCE != WEB_MSA_SELF_DEVELOPMENT
```

Self-maintenance may be autonomous only inside explicitly defined, pre-authorized Web-local operations.

Examples of candidate maintenance operations, each requiring exact implementation governance before activation, may include:

- restart/reinitialize a Web-local non-authority component;
- re-establish a Web-local adapter/session to an already-authorized route;
- clear invalid transient Web-local state;
- restore a known-good Web presentation configuration;
- isolate a failing Web-local optional feature while preserving core Web availability;
- emit evidence and alert the Owner when maintenance cannot safely restore service.

Self-maintenance must not silently become source-code evolution, architecture change, entitlement change, security-policy change, authority change or external-owner repair.

# 5. Web development rule

The Web AI does not autonomously research for self-development and does not decide to redesign/develop itself because it found a better approach.

```text
WEB_MSA_AUTONOMOUS_SELF_DEVELOPMENT = DISABLED
WEB_MSA_RESEARCH_FOR_SELF_DEVELOPMENT = DISABLED
OWNER_DIRECT_REQUEST_REQUIRED_FOR_WEB_DEVELOPMENT = TRUE
```

Example:

```text
OWNER: "Replace the Web logo with the image I uploaded."
→ WEB-OWNED
→ WEB MSA MAY EXECUTE THROUGH GOVERNED WEB DEVELOPMENT PATH
```

A direct Owner development request still requires the applicable Web validation, security, tests, Red Team and evidence. The Owner request is not a reason to skip safety checks.

# 6. Cross-workstream Owner requests

Example:

```text
OWNER: "Improve FSATS opportunity analysis."
→ NOT WEB-OWNED
→ WEB MSA PRESERVES OWNER INTENT
→ ROUTES/HANDS OFF TO FSATS OWNER
→ FSATS OWNS DOMAIN RESEARCH / DEVELOPMENT PROCESS
→ RESULTS/REPORT RETURN THROUGH GOVERNED BOUNDARY
→ WEB PRESENTS TO OWNER
```

Web MSA does not edit FSATS source or become FSATS awareness merely because the command arrived through the Web chat.

Likewise:

```text
FOUNDATION-OWNED REQUEST -> ROUTE TO FOUNDATION
OTHER APPLICATION-OWNED REQUEST -> ROUTE TO THAT APPLICATION
```

# 7. Compound request splitting

If one natural-language Owner request spans multiple ownership domains, the Web MSA must split the work by ownership while preserving one parent Owner request/correlation.

Example:

```text
"Change the Trading page layout and change how FSATS calculates confidence."

WEB PART:
- page/layout presentation

FSATS PART:
- confidence calculation/business logic
```

The Web portion may proceed inside Web authority. The FSATS portion is routed to FSATS. Completion of one portion must not be shown as completion of the whole compound request.

# 8. Single Customer Interaction & Support LSA

The only initial Web LSA is dedicated to human conversation and support.

Responsibilities:

- converse naturally with the customer;
- explain information/analysis/results supplied by FSATS and other authoritative Falcon owners;
- simplify complex results without changing their meaning;
- adapt communication style to the individual customer;
- support beginner exploration and education;
- support the customer during incidents;
- follow the persistent Incident Conversation lifecycle;
- assist human Support with context when permitted;
- perform governed research only when needed to help the customer with an incident or directly related support need;
- maintain strict customer/session isolation.

# 9. Personalization is not self-development

The LSA may adapt:

- Arabic vs English;
- formal vs casual customer-facing style where appropriate;
- short vs detailed explanation;
- novice vs advanced explanation level;
- preferred information order;
- step-by-step pacing;
- stress-aware communication during incidents;
- permitted interaction preferences.

But:

```text
CUSTOMER_PERSONALIZATION != SELF_DEVELOPMENT
CUSTOMER_STYLE_ADAPTATION != AUTHORITY_EXPANSION
LSA_SELF_DEVELOPMENT = DISABLED
```

Personalization data must remain within governed privacy/retention/tenancy rules.

# 10. Explanation boundary

The LSA explains source truth. It does not re-score or reinvent it.

```text
LSA_EXPLANATION != FSATS_ANALYSIS
WEB_AI_SUMMARY != NEW_TRADING_CONCLUSION
```

If FSATS supplies 72% confidence, the LSA may explain what that means in context, but it cannot silently replace it with a higher/lower confidence.

If the source is stale, partial, unsupported or unavailable, the explanation must carry that limitation.

# 11. User research boundary

The LSA may conduct governed research specifically for customer support during an incident or directly related support need.

Examples:

- current official broker/provider documentation;
- current service status page;
- current official troubleshooting steps;
- current browser/platform support guidance.

Rules:

```text
WEB_LSA_RESEARCH = CUSTOMER_SUPPORT_ASSISTANCE_ONLY
WEB_LSA_RESEARCH_FOR_SELF_DEVELOPMENT = DISABLED
RESEARCH_RESULT != FALCON_AUTHORITATIVE_TRUTH
RESEARCH_RESULT != FSATS_ANALYSIS_TRUTH
RESEARCH_RESULT != DEVELOPMENT_AUTHORITY
```

Research egress must use the governed research-only path when materialized, with target/purpose/audit/revocation controls. The LSA must favor official/current sources for broker/provider instructions.

# 12. Incident behavior

During normal Falcon-led incident guidance, the LSA may:

- explain supplied incident meaning;
- collect customer observations;
- ask one useful question/step at a time;
- adapt to stress;
- guide screenshot/description choice;
- guide the customer through broker UI if the customer explicitly chooses to act personally;
- keep chronology coherent across text/voice;
- keep Support request active where required;
- prepare the closure explanation from authoritative incident facts.

It does not become Guardian, broker or execution authority.

# 13. Human Support takeover

When an authorized human Support takeover becomes active:

```text
CUSTOMER-FACING FALCON LSA OUTPUT = SILENT
HUMAN SUPPORT IDENTITY = EXPLICIT
```

The LSA may remain available behind the scenes only for permitted assistance such as:

- summarizing the incident chronology;
- surfacing already-authorized evidence;
- research assistance within the support-only research rule;
- preparing draft explanations for Support.

It must not send customer-facing messages during takeover unless the takeover state is explicitly ended/released under the governing interaction contract.

# 14. Owner Gateway experience

The Project Owner should have one natural human-facing entry point:

```text
OWNER
→ WEB AI / OWNER GATEWAY
```

The Owner does not need to know which internal Application/awareness entity should receive every request.

The Gateway should maintain:

- original Owner wording;
- structured interpretation;
- target owner/workstream;
- target Application/system/entity when known;
- scope and blast radius for sensitive requests;
- request identity/correlation;
- current lifecycle state;
- evidence and returned result;
- clear next Owner decision when applicable.

# 15. Sensitive request confirmation

Natural language must not collapse authority stages.

Before an authority-bearing request, the Gateway should make scope clear.

Example:

```text
I understood this as:
Authorize <exact candidate> for <exact next stage>.
This does not authorize later stages.
```

Exact confirmation UX depends on the governing contract and risk level.

# 16. FSATS self-development reporting through Web

FSATS self-development is deliberately different from Web AI development.

Where FSATS contracts permit research/self-development, FSATS owns that process and its governed sandbox/validation lifecycle.

Shared Web's role is to be the Owner-facing presentation/review surface for the resulting governed proposal/report when an appropriate cross-workstream contract exists.

Web does not fabricate missing report fields or assume that a sandbox PASS equals adoption.

```text
FSATS_RESEARCH != WEB_RESEARCH
FSATS_SANDBOX_PASS != OWNER_APPROVAL
FSA_REVIEW != OWNER_ADOPTION
WEB_PRESENTS_REPORT != WEB_OWNS_DEVELOPMENT
```

The exact report-delivery/runtime contract must be reconciled/created through the owning workstreams rather than invented by Web.

# 17. AI identity / Kill / lifecycle integration

The Web MSA and Web LSA are executable AI subjects and must not exist outside Falcon's governed AI inventory/containment model.

Before production-bound AI activation, exact contracts must cover:

- logical AI target identity;
- owning Application = Shared Web;
- hierarchy/lineage;
- runtime instance identity/generation;
- registration state;
- Kill enforcement binding;
- release/trust state;
- replacement/restart semantics;
- evidence identity;
- resource/governance binding where required.

Rules:

```text
WEB_AI != ITS_KILL_AUTHORITY
RESTART != TRUST_RESTORATION
REPLACEMENT_INSTANCE != AUTOMATIC_AUTHORITY_INHERITANCE
```

If current Foundation target registration only covers FSATS Application AI targets, Web shall raise the required governed Foundation FCR rather than invent registration locally.

# 18. Degraded behavior

If Web MSA/LSA is unavailable or contained:

- deterministic public pages may continue if trustworthy;
- protected non-AI views may continue if authoritative session and source truth remain valid;
- emergency/Owner minimum control must degrade truthfully;
- no fake AI answer is shown;
- the Web must clearly state that AI assistance is unavailable while keeping safe non-AI functionality where possible.

# 19. No implicit expansion

This model does not grant Web AI:

- Foundation repair authority;
- FSATS code write authority;
- Trading/business decision authority;
- broker/provider control authority;
- credential custody;
- Kill/release authority;
- autonomous product redesign;
- autonomous research for Web self-development.
