# Foundation Post-Stage16 FCR Follow-up Implementation Plan

Status: OWNER_AUTHORIZED_FOR_BOUNDED_IMPLEMENTATION
Date: 2026-08-18
FCRs: FCR-0010, FCR-0031, FCR-0237

## Governance placement

This is bounded post-Stage16 compatibility/publication work. It does not create Stage 17 and does not reopen accepted Stage 6, Stage 14, Stage 16, or their Owner closures.

## FCR-0010 / FCR-0031

Source-first review confirmed that accepted Stage 6 already exposes the relevant resource projection contracts, including `ApplicationResourceStateProjection`, `ApplicationResourceStateProjectionSet`, `AggregateResourceStateProjection`, and load-shedding/pressure semantics. The missing capability is immutable canonical publication identity for exact Application consumption through the accepted Stage 14 artifact-consumption substrate.

Implementation scope:

1. Extend `CanonicalFoundationArtifacts` with stage-neutral resource projection descriptors.
2. Bind descriptors to immutable Stage 6 accepted source/evidence, not a moving branch.
3. Keep `Foundation.ArtifactPublication` dependency-free.
4. Verify exact ID/version/digest/evidence/compatibility consumption and mutation rejection.
5. Preserve:

```text
RESOURCE_STATE_PROJECTION != RESOURCE_AUTHORITY
LOAD_SHEDDING_SIGNAL != LOAD_SHEDDING_EXECUTOR
REQUESTED_RESOURCE != PROVEN_RESIDUAL_NEED != GRANTED_RESOURCE
APP_RSC != FOUNDATION_RESOURCE_GOVERNANCE
CANONICAL_ARTIFACT_CONSUMPTION != RUNTIME_ACTIVATION
TECHNICAL_CONSUMPTION != BUSINESS_AUTHORITY
```

## FCR-0237

Existing `AuthorityPolicy`, `DelegationEvidence`, `FitnessEvidence`, and `DefaultDenyAuthorityEngine` are reused. No second authority engine will be introduced.

Implementation scope:

1. Add a stage-neutral `StandingOwnerPreapprovalProfile` and evaluator in `Foundation.Authority`.
2. Require explicit Owner-attributable authority provenance, policy/delegation identity, version, validity window, revocation, exact actor/Application/update/resource/purpose/scope/environment/security-context match, candidate immutable identity, evidence, correlation, and bounded risk tier.
3. Route an exact matched candidate through the existing default-deny authority engine.
4. Bind the final preapproval decision identity to the candidate identity/version/digest plus the underlying authority decision identity.
5. Always return no execution, deployment, or business authority.
6. Keep the following classes manual-only under ordinary standing preapproval:

```text
AI_KILL
RELEASE
CONTROLLED_REVIVAL
LIVE_TRADING_ACTIVATION
CREDENTIAL_OR_SECURITY_CHANGE
AUTHORITY_EXPANSION
DEPLOYMENT
CONSTITUTION_OR_GOVERNANCE_CHANGE
```

These classes require their separately governed authority path and cannot be enabled merely by adding them to a Web-maintained accepted list.

## Verification

Add one bounded verifier covering both follow-ups. Required adversarial cases include wrong artifact ID/version/digest/evidence/compatibility, moving-authority assumptions, missing/revoked/expired preapproval, wrong actor/Application/class/resource/purpose/scope/environment/security context, excessive risk, mutated candidate digest/version, manual-only classes, missing fitness/evidence, and proof that policy match does not grant execution/deployment/business authority.

Any executable change after a PASS requires governed revalidation. Final FCR handoff occurs only after executable verification and post-executable Architecture/Consistency + Red Team review.