# Owner Direction — Standing Auto-Accept and Owner-Initiated Rollback

**Date:** 2026-08-18  
**Workstream:** Shared Falcon Web Application  
**Branch:** `web-development`  
**Status:** `OWNER_DIRECTION_RECORDED / CROSS_WORKSTREAM_GOVERNANCE_PENDING_FCR0237_FCR0238`

## Owner intent

The Project Owner requires the Falcon Command Center to support an editable standing approval list for future update proposals, with a strict Web-mediated decision path.

The key rule is:

```text
APPLICATION_OR_AI_PROPOSAL -> WEB_COMMAND_CENTER_MATCH_EVALUATION -> OWNER-DERIVED_AUTO_ACCEPT_DECISION
```

Applications and AIs SHALL NOT self-declare or self-issue Owner auto-accept authority.

```text
AI_SELF_DECLARED_AUTO_ACCEPT = FORBIDDEN
APPLICATION_SELF_DECLARED_AUTO_ACCEPT = FORBIDDEN
AI_OR_APPLICATION_POLICY_MATCH_CLAIM != OWNER_APPROVAL
WEB_MATCH_EVALUATION != WEB_MINTED_FOUNDATION_AUTHORITY
```

The Application/AI may provide proposal metadata, evidence, classification, validation results and a backup/rollback plan. It may not decide that the proposal is accepted because it believes it matches a standing policy.

## Standing approval evaluation

The Command Center shall evaluate a proposal against the current Owner standing approval policy/list and its governing authority evidence.

Auto-accept is allowed only when all required conditions are satisfied exactly, including the cross-workstream authority and semantic contracts being governed through FCR-0237 and FCR-0238.

The Web evaluation must fail closed for any ambiguity, broader scope, higher risk, stale/superseded policy, revoked policy, missing evidence, mismatched owner/application/AI identity, materially changed proposal, or missing/invalid backup plan.

```text
OWNER_SILENCE != OWNER_APPROVAL
STANDING_POLICY_MATCH = EXPLICIT_PREAUTHORIZED_OWNER_DECISION_PATH
NO_EXACT_MATCH -> MANUAL_OWNER_REVIEW
```

## Mandatory backup / rollback plan

No proposal may qualify for standing auto-accept unless it carries a valid, proposal-specific backup/rollback plan that satisfies the governed contract.

At minimum the plan must identify the applicable pre-change baseline/version, affected scope, rollback method, required evidence, validation state and any safety restrictions required before rollback can be requested or executed.

```text
AUTO_ACCEPT_WITHOUT_VALID_BACKUP_PLAN = FORBIDDEN
BACKUP_PLAN_PRESENT != ROLLBACK_AUTHORIZED
ROLLBACK_PLAN_AVAILABLE != ROLLBACK_EXECUTED
```

## Auto-accepted history in Command Center

The Command Center shall provide an Owner-visible history of auto-accepted proposals. Each record should preserve at minimum:

- proposal/update identity;
- originating Application/AI identity;
- proposal class and affected scope;
- standing policy identity/version used for the match;
- evidence and validation references;
- backup/rollback plan identity;
- auto-accept decision identity/time;
- current resulting state where authoritative truth is available;
- rollback eligibility/status where authoritative truth is available.

The history is an audit/presentation surface, not an authority source by itself.

## Owner-initiated rollback path

Rollback authority SHALL NOT originate from the Application or AI simply because a rollback plan exists.

The required path is:

```text
OWNER_IN_COMMAND_CENTER
    -> WEB_ISSUES_ATTRIBUTABLE_OWNER_ROLLBACK_ORDER
    -> OWNING_GOVERNED_TARGET_RECEIVES_ORDER
    -> TARGET/PLATFORM_VALIDATES_CURRENT_AUTHORITY_AND_SAFETY
    -> ACCEPT / REJECT
    -> EXECUTION IF SEPARATELY AUTHORIZED
    -> RESULT/EVIDENCE RETURNED TO WEB
```

The Web may prepare and transmit the attributable Owner rollback order through the governed route. The Web does not itself become the rollback executor or owning business authority.

```text
AI_SELF_INITIATED_ROLLBACK = FORBIDDEN_UNLESS_SEPARATELY_GOVERNED_FOR_AN_UNRELATED_AUTOMATIC_RECOVERY_CASE
APPLICATION_SELF_INITIATED_OWNER_ROLLBACK = FORBIDDEN
ROLLBACK_REQUESTED != ROLLBACK_ACCEPTED
ROLLBACK_ACCEPTED != ROLLBACK_COMPLETED
WEB_ROLLBACK_ORDER != WEB_ROLLBACK_EXECUTION_AUTHORITY
```

An ordinary Owner rollback order for an auto-accepted proposal must originate from the Owner through the Web Command Center path. Narrow separately governed automatic recovery/repair semantics are not to be generalized into Owner rollback authority.

## Authority separation

Rollback of code/model/configuration does not silently restore or change independent authority domains.

```text
ROLLBACK != AUTHORITY_RESTORATION
ROLLBACK != TRUST_RESTORATION
ROLLBACK != LIVE_ACTIVATION
ROLLBACK != CREDENTIAL_REACTIVATION
ROLLBACK != KILL_RELEASE_OR_REVIVAL
ROLLBACK != DEPLOYMENT_AUTHORITY
```

Any such independent state remains subject to its owning governed authority and recovery/release path.

## Cross-workstream dependencies

This Owner direction is recorded now for implementation in the Shared Web plan, but automatic acceptance shall not be implemented as authority-bearing runtime behavior until the required cross-workstream contracts are governed.

- **FCR-0237 / FOUNDATION:** standing Owner pre-approval policy, delegation/authority provenance, exact matching, revocation/version semantics, backup-plan/rollback-order authority and high-consequence exclusions.
- **FCR-0238 / APPLICATION:** canonical update/proposal taxonomy, governed classification, eligibility metadata, backup/rollback metadata and Application-side receipt/outcome semantics.

Until those contracts are complete:

```text
AUTO_ACCEPT_RUNTIME_AUTHORITY = NOT_YET_AUTHORIZED
OWNER_ROLLBACK_ORDER_RUNTIME_BINDING = NOT_YET_AUTHORIZED
WEB_DESIGN_AND_PLANNING = AUTHORIZED_WITHIN_EXISTING_WEB_SCOPE
```

## Implementation placement

This requirement belongs to the Owner Command Center / Owner approvals and development-report workflow. It shall be reconciled into the current Master Web Plan before implementation of the affected approval surfaces.

The intended product model is:

```text
MANUAL OWNER APPROVALS
+ STANDING OWNER APPROVAL POLICIES
+ AUTO-ACCEPTED HISTORY
+ MANDATORY BACKUP/ROLLBACK PLAN
+ OWNER-INITIATED ROLLBACK ORDERS
```

This record preserves the Owner direction so it is not lost or reinterpreted later.
