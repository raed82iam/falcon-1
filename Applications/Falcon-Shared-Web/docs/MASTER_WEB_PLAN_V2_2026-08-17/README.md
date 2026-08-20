# Shared Falcon Web — Master Plan V2

**Date:** 2026-08-17  
**Branch:** `web-development`  
**Writable scope:** `applications/shared/web/**`  
**Plan status:** `OWNER_ACCEPTED / WP01_COMPLETE / CROSS_WORKSTREAM_RUNTIME_BINDINGS_IN_PROGRESS`  
**Purpose:** consolidate the complete current Shared Falcon Web direction into one execution-ready planning set without erasing historical records.

## Why this folder exists

Shared Web already has substantial implementation, historical Ideas, Owner decisions, FCR reconciliations, Red-Team records, architecture rules, checkpoints, and handovers. The original implementation plan remains useful history, but it no longer contains the whole current Web direction.

This folder is the new **single planning index** for the next Web execution cycle. It does not delete or rewrite prior evidence. It reconciles it.

```text
HISTORICAL IDEA / PLAN
        +
CURRENT OWNER DECISION
        +
CURRENT FCR CONTRACT
        +
CURRENT IMPLEMENTATION EVIDENCE
        +
CURRENT RED-TEAM / VERIFICATION STATE
        ↓
MASTER WEB PLAN V2
```

## Precedence rule

This plan does not supersede Falcon governance or owners of external truth.

When records disagree, use this order:

1. Falcon Vision and Constitution.
2. Current Project Owner decisions.
3. Current FCR Issue body / current governing cross-workstream contract.
4. Current approved Foundation/Application contracts and authority.
5. `applications/shared/web/WORKSTREAM_RULES.md` except where a newer explicit Owner/FCR protocol correction prospectively controls a lifecycle detail.
6. Current Shared Web architecture and accepted/reconciled decisions.
7. This Master Plan V2, including the Red-Team remediation, Owner-acceptance amendment and execution-baseline records in this folder.
8. Historical plans, Ideas, checkpoints and superseded handovers as audit/input history.

Historical documents are preserved. A historical status shall not override a newer canonical status.

## Documents in this plan

1. `00_GOVERNANCE_SCOPE_AND_NON_NEGOTIABLES.md`  
   Defines ownership, authority, truth, security and cross-workstream rules.

2. `01_PRODUCT_SITE_AND_EXPERIENCE_BLUEPRINT.md`  
   Defines the complete product surface: Public Falcon, accounts, My Applications, FSATS workspace, Owner Command Center, AI, incident/support, subscription/VIP direction, mobile and accessibility.

3. `02_WEB_AI_MSA_LSA_AND_OWNER_GATEWAY.md`  
   Defines the Owner-approved Web MSA, the single customer/support LSA, Owner request routing and research/development boundaries.

4. `03_RUNTIME_INTEGRATION_AND_FCR_DEPENDENCY_PLAN.md`  
   Defines Web-owned ports/adapters, current runtime dependencies, provider/data rules and live FCR handoff model.

5. `04_IMPLEMENTATION_WORK_PACKAGES.md`  
   Defines the ordered work packages, entry/exit criteria, dependencies, tests and Owner gates.

6. `05_QUALITY_SECURITY_RED_TEAM_AND_ACCEPTANCE.md`  
   Defines verification, accessibility, localization, security, Red-Team and Owner-acceptance gates.

7. `06_IDEA_DECISION_AND_REQUIREMENT_COVERAGE_REGISTER.md`  
   Maps every current Ideas document and major later Owner/reconciliation decision to the Master Plan so nothing is silently dropped.

8. `07_MASTER_PLAN_RED_TEAM_AND_REMEDIATION.md`  
   Records the planning Red Team, closes the identified coverage ambiguities, and adds controlling clarifications for incident details, Web-AI runtime model/provider selection, targeted wireframes/mockups and entitlement-direction safety.

9. `08_OWNER_ACCEPTANCE_AND_FINAL_PLANNING_AMENDMENT_2026-08-17.md`  
   Records explicit Owner acceptance of the Master Plan V2 and the final two incorporated amendments: Falcon Owner Home post-sign-in routing and permanent Project Owner access to the full FSATS VIP product-feature set without commercial VIP/trial downgrade semantics.

10. `09_WP01_CURRENT_SOURCE_AND_CONTRACT_BASELINE_INVENTORY_2026-08-17.md`  
    Completes WP-01 with the exact current source/contract/FCR inventory, current-vs-missing classification, Owner-amendment implementation gaps, executable-evidence freshness rule, and WP-01 Red Team.

11. `../OWNER_DIRECTION_STANDING_AUTO_ACCEPT_AND_ROLLBACK_2026-08-18.md`  
    Records the Owner-required Command Center standing approval list, Web-only Owner-derived auto-accept decision path, mandatory proposal-specific backup/rollback plan, auto-accepted history, and Owner-initiated rollback-order flow. Runtime authority remains gated by FCR-0237 and FCR-0238.

## Red-Team, Owner-acceptance and execution status

Planning Red Team result:

```text
OPEN_CRITICAL = 0
OPEN_HIGH = 0
OPEN_MEDIUM = 0
OPEN_LOW = 0
```

Final Owner planning decision:

```text
OWNER_REQUESTED_FINAL_AMENDMENTS = INCORPORATED
MASTER_PLAN_V2 = OWNER_ACCEPTED
OWNER_STANDING_AUTO_ACCEPT_AND_ROLLBACK_DIRECTION = RECORDED_FOR_FUTURE_IMPLEMENTATION
```

Current execution state:

```text
WP00 = OWNER_ACCEPTED_AND_COMPLETE
WP01 = COMPLETE
WP01_RED_TEAM = PASS
WP01_OPEN_CRITICAL_HIGH_MEDIUM_LOW = 0/0/0/0
CURRENT_EXACT_SOURCE_FULL_EXECUTABLE_RECHECK = REQUIRED_BEFORE_NEW_CURRENT-CANDIDATE_FULL_PASS_CLAIM

FCR0235 = FOUNDATION_CONTRACT_COMPLETE / WEB_EXACT_IDENTITY_FIL_BINDING_IMPLEMENTED / TARGETED_PASS / FULL_CURRENT_HEAD_SUITE_PENDING
FCR0152 = STAGE16_ACCEPTED_AND_CLOSED / WEB_EXACT_IDENTITY_SESSION_MFA_FIL_BINDING_IMPLEMENTED / TARGETED_PASS / FULL_CURRENT_HEAD_SUITE_PENDING
FCR0076 = FOUNDATION_RECOVERY_PROJECTION_COMPLETE / WEB_EXACT_RECOVERY_FIL_AND_RUNTIME_PORT_BINDING_IMPLEMENTED / TARGETED_PASS / FULL_CURRENT_HEAD_SUITE_PENDING

FCR0237 = FOUNDATION_STANDING_OWNER_PREAPPROVAL_AUTHORITY_PROFILE_PLANNING_PENDING
FCR0238 = APPLICATION_UPDATE_TAXONOMY_AND_ELIGIBILITY_CONTRACT_PENDING
OWNER_AUTO_ACCEPT_RUNTIME = BLOCKED_UNTIL_FCR0237_AND_FCR0238_GOVERNED_HANDOFF
OWNER_ROLLBACK_ORDER_RUNTIME_BINDING = BLOCKED_UNTIL_GOVERNED_HANDOFF

IMMEDIATE_NEXT_ACTION = FCR0169_STAGE14_OPERATIONAL_PROJECTION_WEB_BINDING_AND_VERIFICATION
THEN_RESUME = WP-02_ARCHITECTURE_DECOMPOSITION_AND_COMPOSITION_STABILIZATION
```

Current FCR bodies always supersede older planning snapshots. The Master Plan priority rule requires a material current `Waiting On: WEB` obligation to be handled before unrelated major source refactoring. A verification-only environment limitation on one completed source binding does not block independent Web-owned binding work that can proceed safely and fail-closed.

The Red-Team remediation record and the final Owner-acceptance amendment record are both part of the Master Plan V2. The amendment record prospectively controls where earlier planning wording conflicts with the accepted Owner Home or Owner FSATS feature-access decisions.

The Owner standing auto-accept / rollback direction is now also a controlling future implementation requirement for the Owner Command Center and WP-18 approval/audit scope. It does not itself create runtime authority. The intended path is:

```text
APPLICATION_OR_AI_PROPOSAL
-> WEB_COMMAND_CENTER_EXACT_POLICY_MATCH
-> OWNER-DERIVED_AUTO_ACCEPT_DECISION
```

and rollback is:

```text
OWNER_IN_COMMAND_CENTER
-> WEB_ATTRIBUTABLE_OWNER_ROLLBACK_ORDER
-> GOVERNED_TARGET_VALIDATION
-> ACCEPT_OR_REJECT
-> SEPARATELY_AUTHORIZED_EXECUTION
-> RESULT_AND_EVIDENCE_BACK_TO_WEB
```

Applications/AIs may supply proposal classification, evidence, validation and backup/rollback plans, but they SHALL NOT self-declare Owner auto-accept or Owner rollback authority.

## Master objective

Build Shared Falcon Web as Falcon's reusable, human-facing interaction layer that is:

- simple for a new customer;
- powerful for an experienced user;
- operationally clear for the Project Owner;
- safe and truthful during incidents;
- bilingual Arabic/English with RTL/LTR quality;
- accessible and responsive;
- modular and replaceable;
- fail-closed when authoritative truth or authority is missing;
- capable of presenting and routing Falcon-wide requests without absorbing another workstream's business authority;
- prepared for multiple future Falcon systems, not hard-coded to Trading alone.

## Prime product distinction

```text
SHARED WEB = HUMAN EXPERIENCE + PRESENTATION + GOVERNED REQUEST TRANSPORT + WEB-OWNED MAINTENANCE/DEVELOPMENT

SHARED WEB != FOUNDATION
SHARED WEB != FSATS BUSINESS LOGIC
SHARED WEB != GUARDIAN
SHARED WEB != BROKER
SHARED WEB != PROVIDER AUTHORITY
SHARED WEB != KILL AUTHORITY
```

## Current implementation truth

Shared Web is already materially implemented. This plan is **not** permission to rewrite it from scratch.

The execution rule is:

```text
PRESERVE VERIFIED GOOD_BEHAVIOR
→ RECONCILE NEW DECISIONS
→ COMPLETE MISSING BINDINGS
→ ADD NEW WEB AI MODEL
→ HARDEN UX / ACCESSIBILITY / SECURITY
→ GOVERNED VERIFICATION
→ RED TEAM
→ OWNER REVIEW
```

## Authority created by this plan

None beyond existing Owner-authorized Web implementation scope.

This planning acceptance and WP-01 completion do not themselves create:

- production deployment authority;
- external connectivity authority;
- Foundation/Application write authority;
- broker/execution authority;
- secret-storage authority;
- Kill execution authority;
- automatic Owner approval authority before FCR-0237/FCR-0238 governance;
- rollback execution authority;
- final implementation-baseline closure.

```text
PLAN_OWNER_ACCEPTED != IMPLEMENTATION_COMPLETE
IMPLEMENTATION_COMPLETE != GOVERNED_VERIFICATION_PASS
VERIFICATION_PASS != IMPLEMENTATION_BASELINE_OWNER_ACCEPTED
IMPLEMENTATION_BASELINE_OWNER_ACCEPTED != PRODUCTION_DEPLOYMENT_AUTHORIZED
```
