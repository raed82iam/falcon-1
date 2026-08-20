# 07 — Master Plan V2 Red Team and Remediation

**Date:** 2026-08-17  
**Review scope:** `MASTER_WEB_PLAN_V2_2026-08-17/**`  
**Review type:** architecture + authority + Owner-decision coverage + FCR consistency + execution completeness  
**Status:** `RED_TEAM_COMPLETE / NO_OPEN_CRITICAL_OR_HIGH / CANDIDATE_READY_FOR_OWNER_REVIEW`

# 1. Review objective

Attack the new Master Plan as if a later Web implementer might misuse ambiguous wording to:

- absorb another workstream's authority;
- silently drop an Owner idea/decision;
- treat historical status as current;
- activate AI/runtime without required contract;
- overstate FCR completion;
- turn product direction into runtime entitlement authority;
- skip validation/Red Team/Owner acceptance;
- allow Web AI autonomous development beyond the Owner-approved model.

# 2. Severity summary

| Severity | Found | Open after remediation |
|---|---:|---:|
| CRITICAL | 0 | 0 |
| HIGH | 0 | 0 |
| MEDIUM | 3 | 0 |
| LOW | 1 | 0 |

# 3. Findings and remediation

## RT-PLAN-001 — MEDIUM — Incident Owner-decision coverage was summarized too broadly

### Finding
The main Product Blueprint correctly included persistent incident chronology, stress adaptation, customer manual-close guidance, Support takeover, screenshots, voice and closure summary, but several explicit Owner-settled details were not stated literally enough for an execution master plan.

### Mandatory additive clarification
The following are part of WP-15 and the Product Blueprint even if not repeated elsewhere:

1. **Owner-as-Support fallback**  
   During the current operating model, the Project Owner may act as authorized human Support when no other authorized Support person is available, subject to the same authoritative Support identity/capability requirements.

2. **No-Support-available behavior**  
   If nobody authorized is available, Falcon tells the customer Support is unavailable, keeps the Support request active, and continues only the Guardian/Application-requested precautionary guidance that remains within Falcon/Web authority.

3. **Broker connectivity explanation**  
   When Application/Guardian identifies a broker/API connectivity incident, Falcon explains truthfully that Falcon is not the broker and that the current issue concerns broker connectivity/state as supplied by the owning Application/Guardian. It may explain that connectivity failures can also occur in direct broker use, but it must not guess the actual broker state or incident cause.

4. **Customer manual-close warning persistence**  
   If a frightened customer wants to close positions personally, Falcon explains the risk clearly and repeats the warning sufficiently to establish understanding before guiding the customer's own broker-UI action. This does not become a Falcon Trading decision or broker execution.

5. **Critical-incident side topics**  
   Falcon may answer a natural trust/context side question briefly and truthfully, then return to the critical incident task without exposing unnecessary internals.

### Disposition
`REMEDIATED_BY_THIS_CONTROLLING_PLAN_CLARIFICATION`.

---

## RT-PLAN-002 — MEDIUM — Web AI runtime model/provider selection was not explicit enough as unresolved

### Finding
Idea 06 explicitly left the final AI model/provider unresolved. The Master Plan defines MSA/LSA responsibilities but could be misread as if a runtime model/provider had already been selected.

### Mandatory additive clarification

```text
WEB_AI_RESPONSIBILITY_MODEL = OWNER_APPROVED
WEB_AI_RUNTIME_MODEL = NOT_SELECTED_BY_THIS_PLAN
WEB_AI_MODEL_PROVIDER = NOT_SELECTED_BY_THIS_PLAN
RESPONSIBILITY_MODEL_APPROVAL != MODEL/PROVIDER APPROVAL
```

WP-12/WP-14 shall evaluate the exact runtime/model/provider only after architecture, privacy, security, cost, locality, latency, availability, licensing/terms and Falcon-governance constraints are known. A model/provider choice must not change MSA/LSA authority boundaries.

### Disposition
`REMEDIATED_BY_THIS_CONTROLLING_PLAN_CLARIFICATION`.

---

## RT-PLAN-003 — MEDIUM — Historical wireframe/mockup discipline could be lost because implementation already exists

### Finding
Idea 01 required Wireframes and Visual Design/Mockups before implementation. Since current Web implementation is already advanced, the Master Plan correctly avoids restarting the entire site, but it did not explicitly preserve the design checkpoint for **new or materially reworked** surfaces.

### Mandatory additive clarification

For any new major surface or material UX restructuring introduced by WPs 03-19:

```text
REQUIREMENT / USER FLOW
→ TARGETED WIREFRAME
→ UX / AUTHORITY REVIEW
→ VISUAL MOCKUP WHEN MATERIAL
→ IMPLEMENTATION
→ BROWSER VERIFICATION
```

Do not create wireframes merely to recreate already-implemented, already-reviewed surfaces without a material design change. This is a targeted design gate, not a restart of the Web architecture.

### Disposition
`REMEDIATED_BY_THIS_CONTROLLING_PLAN_CLARIFICATION`.

---

## RT-PLAN-004 — LOW — Historical VIP product direction could be mistaken for live entitlement authority

### Finding
The Product Blueprint intentionally preserves the one-month VIP trial, seven-day warning, Standard limitations, Falcon Schools VIP direction and Standard one-Strategy/one-asset direction from the historical Owner product planning. The plan already states that exact entitlement/runtime contracts remain external, but the distinction deserves an explicit anti-regression rule.

### Clarification

```text
VIP_PRODUCT_DIRECTION != CURRENT_RUNTIME_ENTITLEMENT_TRUTH
STANDARD_PRODUCT_DIRECTION != CURRENT_RUNTIME_ENTITLEMENT_TRUTH
WEB_UI_RULE_CANDIDATE != ENTITLEMENT_AUTHORITY
```

Web may design the UX against the Owner product direction, but activation/enforcement must bind the authoritative entitlement/subscription contract when materialized.

### Disposition
`CLARIFIED / NO OPEN FINDING`.

# 4. High-risk areas reviewed with no open finding

## Web MSA self-maintenance vs Foundation repair authority

PASS.

The plan limits self-maintenance to explicit pre-authorized Web-local operations and preserves:

```text
HEALTH_PROJECTION != REPAIR_AUTHORITY
WEB_MSA_SELF_MAINTENANCE != GENERAL_REPAIR_AUTHORITY
```

## Web AI autonomous self-development

PASS.

The plan preserves the Owner-approved rule:

```text
WEB_MSA_RESEARCH_FOR_SELF_DEVELOPMENT = DISABLED
OWNER_DIRECT_REQUEST_REQUIRED_FOR_WEB_DEVELOPMENT = TRUE
LSA_SELF_DEVELOPMENT = DISABLED
```

## Cross-workstream implementation authority

PASS.

Owner requests entering through Web are routed by ownership. Web does not gain Foundation/FSATS write authority.

## New Web AI target registration / Kill binding

PASS WITH EXTERNAL DEPENDENCY EXPLICIT.

WP-12 requires exact AI registration/containment/Kill/FSA applicability reconciliation and FCR creation when generic support is missing. Production-bound Web AI activation is not assumed.

## Support takeover

PASS after RT-PLAN-001 clarification.

Human Support remains explicit and Falcon LSA remains silent customer-facing during active takeover.

## FCR historical/current state confusion

PASS.

The Master Plan states that fresh Issue body/header controls and that the matrix is only a dated execution snapshot.

## Demo/live confusion

PASS.

Preview vs Authoritative remains mutually exclusive and old PASS results cannot migrate to changed commits.

## Emergency/Kill authority

PASS.

Web request/presentation does not become Foundation Kill/release authority.

# 5. Final Red Team decision

```text
MASTER_PLAN_ARCHITECTURE = PASS
MASTER_PLAN_OWNER_DECISION_COVERAGE = PASS_AFTER_REMEDIATION
MASTER_PLAN_AUTHORITY_BOUNDARIES = PASS
MASTER_PLAN_FCR_MODEL = PASS
MASTER_PLAN_WEB_AI_BOUNDARY = PASS
MASTER_PLAN_INCIDENT_BOUNDARY = PASS_AFTER_REMEDIATION
MASTER_PLAN_TEST_AND_RED_TEAM_LIFECYCLE = PASS
MASTER_PLAN_HISTORICAL_IDEA_COVERAGE = PASS
MASTER_PLAN_IMPLEMENTATION_AUTHORITY_EXPANSION = NOT_FOUND
MASTER_PLAN_RUNTIME_AUTHORITY_FABRICATION = NOT_FOUND
OPEN_CRITICAL = 0
OPEN_HIGH = 0
OPEN_MEDIUM = 0
OPEN_LOW = 0
```

# 6. Current state after Red Team

The Master Plan V2 is now **ready for Project Owner review as a planning candidate**.

It is not yet `OWNER_ACCEPTED` and not `CLOSED`.

If the Project Owner requests any material change to this plan, apply the change and perform a fresh consistency/Red-Team review of the changed planning candidate before asking for final Owner acceptance.
