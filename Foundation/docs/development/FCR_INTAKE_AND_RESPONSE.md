# Foundation FCR Intake and Response Workflow

This document defines how the Falcon Foundation workstream receives, triages, responds to, and resolves Foundation Capability Requests (FCRs) raised by Applications.

## Shared control point

The repository-wide shared FCR operating protocol is GitHub Issue #1:

`FCR Shared Registry and Operating Protocol`

GitHub Issues are the neutral transport between Application workstreams and the Foundation workstream. Foundation SHALL NOT rely on Application branch-local documents alone as the official intake channel for a Foundation-impacting request.

## Intake rule

Foundation SHALL review open repository issues with the `[FCR-` title prefix as the official FCR inbox.

An FCR is a request for Foundation disposition. It is not implementation authority, baseline acceptance, Work Package authorization, or permission to modify an accepted Foundation state.

## Mandatory handoff tracking

The Issue body is the canonical current-state header. Issue comments are the chronological audit trail.

Before the Foundation workstream decides whether it has FCR work to do, it SHALL inspect these top fields in every open FCR Issue:

- `Status:`
- `Waiting On:`
- `Next Required Action:`

If `Waiting On: FOUNDATION`, Foundation SHALL inspect the latest relevant Issue comments/evidence and perform or disposition the stated next action. Foundation SHALL NOT rely only on the original request body or on the lifecycle status line.

If `Waiting On: APPLICATION`, Foundation SHALL treat the FCR as awaiting Application action unless new Foundation evidence materially changes the handoff.

After Foundation posts a substantive triage, clarification request, implementation update, evidence handoff, or disposition, it SHALL update the Issue body handoff fields:

- More Application clarification/verification required -> `Waiting On: APPLICATION`
- Further Foundation planning/implementation/reconciliation required -> `Waiting On: FOUNDATION`
- Explicit Owner decision required -> `Waiting On: OWNER`
- FCR fully closed -> `Waiting On: NONE`

When Foundation has provided `EXISTS` evidence or reports `FOUNDATION_IMPLEMENTED` and Application verification is required, the Issue body SHALL say `Waiting On: APPLICATION` and state the exact verification expected in `Next Required Action`.

If Foundation cannot edit the Issue body, its comment SHALL include:

`HANDOFF_UPDATE_REQUIRED: Waiting On=<FOUNDATION|APPLICATION|OWNER|NONE>; Next Required Action=<text>`

The handoff is not fully synchronized until the Issue body reflects it.

## Required initial triage

For each submitted FCR, Foundation SHALL determine whether the request is sufficiently evidenced and classify its disposition as one of:

- `EXISTS`
- `ACCEPTED_FOR_PLANNING`
- `NEEDS_CLARIFICATION`
- `DEFERRED`
- `REJECTED`

Before accepting a gap, Foundation SHOULD first verify whether the requested externally observable behavior already exists in an approved contract, capability, specification, or implementation.

## Foundation response requirements

A Foundation response SHALL identify, as applicable:

- Foundation branch
- Foundation commit or accepted baseline reference
- governing contract/specification/capability path
- whether the capability already exists or requires governed planning
- compatibility constraints
- verification evidence
- any required prospective Work Package or Owner authorization

If the request is rejected, the Foundation response SHALL explain the governing reason without redesigning the requesting Application.

## Authority boundary

Foundation SHALL NOT:

- treat an FCR as authority to implement code or modify an accepted baseline;
- modify Application files while responding to an FCR;
- prescribe a redesign of the requesting Application merely because Foundation currently lacks a capability;
- mark the Application side verified on behalf of the requesting Application;
- infer urgency, silence, technical ability, or an Application request as delegated authority.

If implementation is required, Foundation must use its normal prospective governance process and obtain the exact authority required for the relevant Work Package or bounded remediation.

## FCR lifecycle

The permitted documentary lifecycle is:

`SUBMITTED -> FOUNDATION_TRIAGE -> disposition`

Possible dispositions:

- `EXISTS`
- `ACCEPTED_FOR_PLANNING`
- `NEEDS_CLARIFICATION`
- `DEFERRED`
- `REJECTED`

When governed Foundation implementation is later completed:

`FOUNDATION_IMPLEMENTED -> APPLICATION_VERIFIED -> CLOSED`

`ACCEPTED_FOR_PLANNING` SHALL NOT be interpreted as `FOUNDATION_IMPLEMENTED`.

## EXISTS disposition

When Foundation determines the requested capability already exists, the response SHALL provide exact evidence sufficient for the Application to verify it, including contract/specification/capability references and, when relevant, implementation or verification evidence.

After this evidence handoff, set `Waiting On: APPLICATION` unless additional Foundation action is still required.

## ACCEPTED_FOR_PLANNING disposition

Use this state when the Application has demonstrated a valid Foundation need but no current approved capability fully satisfies it.

The FCR then becomes an input to governed Foundation planning. It does not bypass the Foundation roadmap, Work Package boundaries, review gates, or Owner authorization.

While the next required work remains Foundation-owned, keep `Waiting On: FOUNDATION` and state the exact bounded next action.

## NEEDS_CLARIFICATION disposition

Use this state when the request does not yet contain enough evidence to establish the gap, expected external behavior, impact, or compatibility requirement.

Foundation SHALL request only the clarification needed to determine disposition and set `Waiting On: APPLICATION`.

## Closure rule

An FCR that required Foundation implementation is not closed when Foundation finishes implementation. The requesting Application must independently verify compatibility first.

Final closure therefore requires:

- documented Foundation disposition;
- Foundation implementation evidence when implementation was required;
- Application verification evidence;
- no unresolved blocking incompatibility within the FCR scope.

Closing an FCR does not itself close a Foundation Work Package, accept an Application baseline, or authorize later work.

## Workstream isolation

Foundation writes remain on `foundation-development`.

Application writes remain on their Application workstream, currently including `application-development`.

The GitHub Issue thread is the shared coordination record between them. The Issue body handoff header tells which workstream must inspect that thread next.
