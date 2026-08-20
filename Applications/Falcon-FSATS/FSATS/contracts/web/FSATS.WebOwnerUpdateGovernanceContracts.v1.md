# FSATS Web Owner Update Governance Contracts v1

**Status:** APPLICATION-IMPLEMENTED SEMANTIC CONTRACT CANDIDATE  
**Owning side for FSATS proposal semantics:** FSATS Application workstream  
**Owner policy and Shared Web consumption ownership:** Project Owner / Shared Falcon Web workstream  
**Runtime authority:** NOT GRANTED

## Purpose

This contract defines the Application-owned semantic boundary requested by FCR-0238 for FSATS/Application/AI update proposals presented to Shared Falcon Web Command Center.

The contract defines the canonical proposal taxonomy, minimum review floor, exact proposal/evidence package, rollback package, returned Owner-derived disposition binding, and Owner rollback request/result semantics.

It does **not** let an Application or AI decide that its own proposal is auto-accepted. The Project Owner, through the current separately governed Shared Web policy, owns that decision.

## Controlling invariants

```text
APPLICATION_AI_PROPOSAL != OWNER_DECISION
APPLICATION_AI_ELIGIBILITY_METADATA != AUTO_ACCEPT
APPLICATION_AI_SELF_APPROVAL = FORBIDDEN
APPLICATION_AI_SELF_ROLLBACK_AUTHORITY = FORBIDDEN
AI_SELF_LABEL != GOVERNED_APPLICATION_CLASSIFICATION
APPLICATION_SELF_LABEL != GOVERNED_APPLICATION_CLASSIFICATION
STANDING_PREAPPROVAL_ELIGIBLE != PROPOSAL_ACCEPTED
PROPOSAL_ACCEPTED != ACTION_AUTHORIZED
ACTION_AUTHORIZED != EXECUTION_AUTHORIZED
EXECUTION_AUTHORIZED != DEPLOYMENT_AUTHORIZED
DEPLOYMENT_AUTHORIZED != RUNTIME_ACTIVATION_AUTHORIZED
OWNER_SILENCE != OWNER_APPROVAL
ROLLBACK_REQUEST != ROLLBACK_ACCEPTED != ROLLBACK_COMPLETED
ROLLBACK_COMPLETED != AUTHORITY_RESTORED
ROLLBACK_COMPLETED != TRUST_RESTORED
SELF_AWARENESS != AUTHORITY
```

Unclassified, ambiguous, malformed, high-impact, stale-evidence or policy-unmatched proposals fail closed to:

```text
MANUAL_OWNER_REVIEW_REQUIRED
```

## Canonical update taxonomy and minimum review floor

The Application-owned taxonomy is stable and machine-readable through `WebOwnerUpdateClass`.

| Update class | Minimum Application review floor |
|---|---|
| `Maintenance` | Eligible for standing-preapproval evaluation when all other guards pass |
| `ModelRefresh` | Eligible for standing-preapproval evaluation when all other guards pass |
| `ParameterTuning` | Eligible for standing-preapproval evaluation when all other guards pass |
| `PresentationOnlySuggestion` | Eligible for standing-preapproval evaluation when all other guards pass |
| `StrategyRevision` | Manual Owner review required |
| `DataSourceChange` | Manual Owner review required |
| `BusinessRuleChange` | Manual Owner review required |
| `RiskRuleChange` | Manual Owner review required |
| `ExecutionBehaviorChange` | Manual Owner review required |
| `AuthorityOrSecurityChange` | Manual Owner review required |
| `DeploymentOrAdoptionChange` | Manual Owner review required |
| `AiSelfDevelopment` | Manual Owner review required, with applicable FSA evidence requirements preserved |
| `Unknown` | Invalid / manual fail-closed |

This is a **minimum review floor**, not an Owner approval list. Shared Web may apply a stricter current Owner policy. It may never use this table to weaken a manual floor.

A nominally low-risk class is also forced to manual review when the proposal declares high or critical impact or any business, risk, execution, security, authority or deployment behavior change. This prevents a producer from gaining a lower review path merely by choosing a friendly class label.

## Mandatory governed classification

Every proposal carries a classification authority source. Standing-preapproval evaluation requires:

```text
ClassificationAuthoritySource = GovernedApplicationClassifier
```

`ProducerSelfClaim` and unspecified classification sources are invalid. Producer Applications/AIs supply facts and evidence; they do not mint Owner authority.

## Mandatory update proposal package

Every proposal crossing this boundary carries at least:

- exact immutable `ProposalId` and `ProposalVersion`;
- exact `ChangeIdentity`;
- exact 64-hex material fingerprint;
- exact owning Application identity;
- producing AI identity where applicable;
- canonical update class and class version;
- governed classification authority source;
- impact classification;
- target environment;
- requested lifecycle phase;
- exact affected scopes/components;
- explicit business/risk/execution/security/authority/deployment behavior-change flags;
- classification evidence;
- test evidence;
- sandbox evidence;
- FSA review requirement/satisfaction/evidence where applicable;
- exact previous accepted/applied state identity;
- lineage/history reference;
- material-change/supersession metadata;
- exact rollback plan.

A materially changed proposal must identify the superseded proposal. It receives a new change identity/material fingerprint and must be re-evaluated against the **current** Owner policy.

## AI self-development evidence

AI self-development proposals do not bypass FSA governance. Where AI self-development applies, the proposal is valid only with the required and satisfied FSA review/evidence chain. Its taxonomy floor remains manual Owner review.

```text
AI_SELF_DEVELOPMENT != OWNER_PREAPPROVAL
FSA_REVIEW != OWNER_APPROVAL
```

## Mandatory rollback / backup plan

Every proposal carries a rollback plan describing the exact recovery semantics even when the proposal ultimately requires manual Owner review.

The plan carries:

- exact plan identity and version;
- exact proposal identity;
- exact change identity;
- exact previous-state identity;
- exact target scopes;
- whether whole-change rollback is supported;
- bounded partial-rollback targets where supported;
- rollback prerequisites;
- known non-reversible effects;
- data/schema migration implications;
- compatibility constraints;
- current/stale state;
- compatibility state;
- validation state;
- rollback validation evidence;
- expected rollback result;
- required recovery/observation steps;
- plan evidence.

Stale, incompatible or unvalidated plans invalidate the proposal package for standing-preapproval evaluation.

Partial rollback targets must be non-blank, unique, and explicitly declared by the exact plan. The plan must cover every affected proposal scope.

A non-reversible proposal fails to manual review unless the **current exact Owner-via-Web policy rule** explicitly allows that exact non-reversible update class/version. Application/AI cannot grant that exception itself.

## Owner standing pre-approval policy snapshot

The Application contract consumes an external current policy snapshot only when it declares:

- policy identity and version;
- authority source `OwnerViaSharedWeb`;
- authority evidence reference;
- one or more exact class/version rules;
- an explicit Owner-owned non-reversible allowance when applicable.

Application- or AI-originated policy authority is rejected.

Even after an exact rule match, the Application result is only:

```text
StandingPreApprovalEligible
```

and grants none of:

- proposal acceptance;
- action authority;
- execution authority;
- deployment authority;
- runtime-activation authority.

Shared Web Command Center applies the Owner-authored current policy/list and any Owner conditions/comments in Web scope.

## Returned Owner-derived disposition

Application/AI consumes an Owner-derived disposition as an **external** decision. It is current only when it exactly binds:

- decision identity;
- proposal identity and version;
- change identity;
- material fingerprint;
- current Owner policy identity and version;
- authority source `OwnerViaSharedWeb`;
- authority evidence.

Any material proposal change, supersession, fingerprint mismatch, or policy-version mismatch invalidates reuse of the prior disposition and requires fresh evaluation.

```text
PRIOR_OWNER_MATCH + CHANGED_PROPOSAL != CURRENT_OWNER_MATCH
PRIOR_OWNER_MATCH + CHANGED_POLICY != CURRENT_OWNER_MATCH
```

## Owner rollback request

Rollback initiation/authorization comes only from an Owner rollback request through Shared Web Command Center.

A request is valid at this semantic boundary only when it binds the exact:

- request identity;
- proposal identity/version;
- change identity;
- plan identity/version;
- `OwnerViaSharedWeb` authority source;
- authority evidence;
- full or bounded-partial request mode;
- bounded targets when partial.

Application- and AI-originated rollback-command authority are rejected.

Full rollback is valid only when the exact plan declares whole-change rollback support. Bounded partial rollback is valid only for targets already declared by that exact plan.

## Rollback lifecycle/result contract

The result contract preserves distinct lifecycle states:

```text
Received
Accepted
Rejected
ExecutionStarted
ExecutionCompleted
ExecutionFailed
PostRollbackValidationRequired
PostRollbackValidationCompleted
PostRollbackValidationFailed
```

The allowed state transitions are fail-closed and prevent skipping from request receipt directly to execution completion.

Every result carries exact result/request/proposal/change/plan identity, previous-state lineage, resulting-state identity, history reference and evidence.

The rollback result contract carries **no implied restoration** of authority, trust, credentials, provider/broker connectivity, Live state, AI Kill release/revival or deployment state. Those require their separately governed owning authority and evidence.

## Machine-readable implementation

Executable contracts and guards:

```text
applications/FSATS/src/Trading/Falcon.FSATS.Trading.Contracts/WebOwnerUpdateGovernanceContracts.cs
```

Dedicated adversarial verifier:

```text
applications/FSATS/tests/Behavior/Falcon.FSATS.OwnerUpdateGovernance.Verifier/
```

Registered in the governed Application build/verifier surfaces:

```text
applications/Falcon.Applications.slnx
applications/ci/Run-Application-Verifiers.ps1
```

## Architecture and consistency review

The implementation preserves the existing responsibility split:

```text
FSATS APPLICATION
  -> owns canonical proposal taxonomy, proposal facts/evidence and rollback business semantics

OWNER VIA SHARED WEB
  -> owns standing pre-approval policy, Owner-derived disposition and rollback order

SHARED WEB
  -> presents/matches the current Owner policy and transports Owner decisions/orders
```

No Foundation ownership is moved into Application. No Shared Web implementation is written by the Application workstream. The Application side exposes semantics only.

The contract deliberately does not create deployment or runtime authority. Environment and requested lifecycle phase are descriptive proposal facts, not grants.

## Red-team acceptance set

The dedicated verifier exercises the following adversarial families:

- exact low-impact canonical class + exact Owner-via-Web policy match;
- eligibility cannot mint acceptance/execution/deployment/runtime authority;
- silence/absent Owner policy fails closed;
- AI/Application policy self-minting fails closed;
- producer self-classification fails closed;
- unknown class fails closed;
- high-impact proposal cannot hide behind a low-risk class;
- execution-changing proposal cannot hide behind maintenance classification;
- missing affected scope fails closed;
- missing sandbox evidence fails closed;
- stale/incompatible/unvalidated rollback plans fail closed;
- wrong previous-state binding fails closed;
- duplicate partial targets fail closed;
- non-reversible update requires explicit exact Owner policy allowance;
- material change without supersession identity fails closed;
- AI self-development without FSA evidence fails closed;
- Owner-derived disposition requires exact proposal/change/fingerprint/current-policy binding;
- stale policy/fingerprint invalidates prior match;
- only Owner-via-Web may authorize rollback;
- full rollback and bounded partial rollback bind exact plan scope;
- undeclared partial target fails closed;
- rollback result preserves exact lineage/correlation;
- rollback lifecycle rejects skipped states.

## Authority boundary

This contract and its verifier create no runtime activation, AI release, deployment, provider connectivity, broker connectivity, Paper, Shadow, Tiny-Live or Live authority.

## FCR synchronization

This contract is the FSATS Application-side implementation candidate for FCR-0238.

Handoff to `Waiting On: WEB` is permitted only after fresh executable validation against the exact current Application implementation. If the verification infrastructure cannot execute, the FCR remains `Waiting On: APPLICATION` with the infrastructure blocker recorded rather than claiming a false PASS.
