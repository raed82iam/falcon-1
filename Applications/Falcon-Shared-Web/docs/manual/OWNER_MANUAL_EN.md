# Project Owner Manual — Falcon Shared Web

**Audience:** Project Owner  
**Language:** English  
**Scope:** Owner-facing presentation, control-request, incident, and governance surfaces in Shared Falcon Web

## 1. Core rule

The Owner has dedicated Web surfaces for observation and governed request submission, but Shared Web does not become the authority source simply because the Owner is using it.

Governing distinction:

`REQUEST_SENT != ACTION_ACCEPTED != ACTION_COMPLETED`

## 2. Owner routes

The current route registry includes:

- Owner Home `#/owner-home`
- Owner Command Center `#/owner`
- Applications `#/owner-apps`
- Incidents `#/owner-incidents`
- Approvals `#/owner-approvals`
- AI Emergency `#/owner-ai-emergency`
- Provider Actions `#/owner-provider-actions`
- Users `#/owner-users`
- Audit `#/owner-audit`
- Settings `#/owner-settings`
- Simulator `#/owner-simulator`

A route existing does not grant access. An authoritative session and matching Owner surface grant are required.

## 3. Owner Home

Owner Home is the Owner entry surface. It provides navigation toward governed Owner areas. A role fact alone is not enough to create route authority, and Project Owner status does not automatically unlock customer FSATS surfaces without separate customer entitlement.

## 4. Command Center

The Command Center is a presentation/request surface, not a direct execution engine. It can present governed information such as:

- system health
- applications
- users
- incidents
- approvals
- system overview
- Owner interactions
- audit information
- settings
- simulator access

Unavailable authoritative projections remain unavailable. The Web layer does not fabricate users, timestamps, or healthy states.

## 5. Applications

Application status is presented from supplied governed truth. Shared Web does not locally perform Application admission, activation, or deployment decisions.

## 6. Incidents and Support

The Owner can observe and interact with incidents when the required runtime capabilities are bound.

Important boundaries:

- Screenshot observed is not broker-confirmed truth.
- Support takeover is not portfolio control.
- Support message is not business authorization.
- Takeover requires explicit authoritative Support capability.
- During authorized Support takeover, Falcon can become a silent observer without transferring authority.

## 7. Approvals and governance

The approvals surface preserves separation between:

- proposal
- eligibility
- Owner decision
- accepted request
- completed outcome

A materially changed proposal must be reevaluated. Producer self-classification or self-approval cannot become Owner approval.

## 8. Owner Update Governance

Shared Web supports governed Owner request families such as:

- policy management
- standing preapproval evaluation
- rollback order

These require a governed binding/transport. Web does not invent a Foundation decision or execution outcome.

`REGISTERED != ACTIVATED`

## 9. AI Emergency

Owner AI Emergency can submit governed intent when authoritative Owner session, target, and blast-radius information are available.

Critical rules:

- Missing or ambiguous target fails closed.
- Targeted Kill cannot use `ALL_AI`.
- Global AI Kill must preserve Falcon Safe Core.
- Global AI Kill is not Falcon shutdown.
- Accepted does not mean completed.
- Release/revival is not local Web authority.

## 10. Provider Actions

Provider Actions never requires plaintext secrets in ordinary Web state. Credentialed routes use opaque credential references.

A configured route does not mean a connection has been executed:

`ROUTE_POLICY_BOUND != CONNECTION_EXECUTED`

## 11. Users and Audit

- Users are not fabricated.
- Audit timestamps are not fabricated.
- Missing projections remain unavailable.
- Untrusted text is output-encoded before rendering.

## 12. Simulator

The Simulator is an Owner-only presentation surface. Simulator truth must never be silently promoted to broker truth or live truth.

## 13. Owner versus Customer surfaces

Owner and Customer access remain separate.

- Owner role alone does not unlock customer FSATS routes.
- Customer entitlement is separately governed.
- Support role does not unlock Owner routes.
- Surface grants must be authoritative and route-appropriate.

## 14. Foundation onboarding status

Shared Web is now fully plug-ready from the preparation side:

- Admission candidate is ready.
- Runtime registration template is ready.
- Full plug-ready preflight is verified by composition.
- No Foundation change is required.

But:

- Actual Admission has not been executed.
- Runtime Registration has not been executed.
- Activation has not been executed.
- Deployment has not been executed.
- Provider connectivity has not been executed.

Runtime-current values are supplied only during the later authorized operation from authoritative sources.

## 15. What this manual does not do

This manual grants no new authority and does not amend Falcon Constitution, Foundation contracts, or Application contracts. It documents current Shared Web behavior only.
