# Application Workstream FCR Workflow

This document defines how Falcon Application workstreams, including the dedicated Shared Falcon Web Application workstream, use Foundation Capability Requests (FCRs) without crossing ownership boundaries.

## Shared control point

The repository-wide shared FCR operating protocol is GitHub Issue #1:

`FCR Shared Registry and Operating Protocol`

GitHub Issues are the neutral transport between:

- `foundation-development`
- `application-development`
- `web-development`

No workstream gains write authority over another workstream's owned files merely by participating in the FCR channel.

## When an Application must raise an FCR

Raise an FCR when an Application has evidence that a required Foundation capability or contract behavior is:

- `MISSING`
- `PARTIAL`
- `INCOMPATIBLE`

Do not modify Foundation files to close the gap from an Application or Web branch.

## Canonical identity

Each FCR is a separate GitHub Issue. After creation, derive the canonical FCR ID from the GitHub issue number:

`FCR-<issue-number padded to four digits>`

Then update the issue title to:

`[FCR-XXXX] <Requesting Application/Workstream> - <short capability request>`

## Required request content

Every Application/Web FCR SHALL include:

- Status: `SUBMITTED`
- Waiting On: `FOUNDATION`
- Next Required Action
- Requesting Application/Workstream
- Requesting branch
- Requesting commit/evidence reference
- Classification: `MISSING`, `PARTIAL`, or `INCOMPATIBLE`
- Blocking: `BLOCKING` or `NON_BLOCKING`
- Requested Foundation capability or contract behavior
- Evidence of the gap
- Application/Web impact
- Required behavior without prescribing Foundation internals
- Relevant Foundation references if known

## Mandatory handoff tracking

The Issue body is the canonical current-state header. Issue comments are the chronological audit trail.

Before a workstream decides whether it has FCR work to do, it SHALL inspect open FCR Issue bodies and read:

- `Status:`
- `Waiting On:`
- `Next Required Action:`

Permitted `Waiting On` values are:

- `FOUNDATION`
- `APPLICATION`
- `WEB`
- `OWNER`
- `NONE`

If `Waiting On: APPLICATION`, the ordinary Application workstream SHALL inspect the latest relevant comments/evidence and perform or disposition the stated next action.

If `Waiting On: WEB`, the Shared Falcon Web Application workstream SHALL inspect the latest relevant comments/evidence and perform or disposition the stated next action. The ordinary Application workstream SHALL NOT answer on behalf of Web unless explicitly authorized by the Project Owner.

If `Waiting On: FOUNDATION`, Application and Web workstreams SHALL treat the FCR as awaiting Foundation action unless they have new material evidence requiring an update.

If `Waiting On: OWNER`, no workstream may substitute its own decision for the Project Owner.

After a substantive response, clarification, verification result, or new gap, the responding workstream SHALL update the Issue body handoff fields so the next actor is explicit.

Typical handoffs:

- Foundation action required -> `Waiting On: FOUNDATION`
- ordinary Application action required -> `Waiting On: APPLICATION`
- Shared Web action required -> `Waiting On: WEB`
- Owner decision required -> `Waiting On: OWNER`
- fully closed -> `Waiting On: NONE`

If a workstream cannot edit the Issue body, its comment SHALL include:

`HANDOFF_UPDATE_REQUIRED: Waiting On=<FOUNDATION|APPLICATION|WEB|OWNER|NONE>; Next Required Action=<text>`

The handoff is not fully synchronized until the Issue body reflects it.

## Cross-workstream ownership boundary

Ordinary Application work SHALL NOT:

- edit Foundation files to satisfy an FCR;
- edit Shared Web-owned files to satisfy a Web handoff;
- prescribe Foundation or Shared Web internal implementation;
- infer Foundation implementation authority from FCR acceptance;
- mark Foundation implementation complete without Foundation evidence.

Shared Web work SHALL NOT:

- edit Foundation files;
- edit FSATS or other Application-owned files;
- answer an Application-owned business decision on behalf of its owning Application;
- invent a local fake Foundation substitute;
- treat FCR participation as cross-workstream write authority.

Foundation SHALL NOT modify Application or Shared Web-owned files while resolving an FCR.

Each workstream MAY describe the exact externally observable behavior, contract semantics, capability, compatibility need, or evidence required from another owning workstream.

## Expected Foundation dispositions

Foundation may respond with:

- `EXISTS`
- `ACCEPTED_FOR_PLANNING`
- `NEEDS_CLARIFICATION`
- `DEFERRED`
- `REJECTED`

`ACCEPTED_FOR_PLANNING` is not implementation authority.

## After Foundation response

When Foundation provides an implementation or existing-capability reference, the requesting workstream SHALL verify compatibility and record:

- requesting branch and commit
- consuming design or implementation evidence
- verification result
- remaining incompatibilities, if any

When Foundation hands evidence to the ordinary Application workstream for verification, use `Waiting On: APPLICATION`.

When Foundation hands evidence to Shared Web for verification, use `Waiting On: WEB`.

Only after requesting-workstream verification may an implementation-required FCR progress to `APPLICATION_VERIFIED` and then `CLOSED`.

## Request template

```text
Status: SUBMITTED
Waiting On: FOUNDATION
Next Required Action: Foundation triage and disposition of this submitted FCR.
Requesting Application/Workstream:
Requesting Branch: application-development | web-development
Requesting Commit/Evidence:
Classification: MISSING | PARTIAL | INCOMPATIBLE
Blocking: BLOCKING | NON_BLOCKING

Requested Foundation Capability / Contract Behavior:

Evidence of Gap:

Application/Web Impact:

Required Behavior (do not prescribe Foundation internals):

Relevant Foundation References (if known):

Foundation Triage:
Foundation Disposition:
Foundation Evidence/Commit:
Requesting Workstream Verification:
Closure Evidence:
```

## FSATS rule

For FSATS compatibility work, a verified Foundation gap SHALL become an FCR instead of being used as a reason to redesign or remove an otherwise valid trading-domain responsibility. Preserve the Application design by default and request only the minimum Foundation capability or contract behavior needed for compatibility.

## Shared Web rule

The Shared Falcon Web Application is a Shared Application workstream. Its FCR participation is coordination-only. It may request generic Foundation capability or contract behavior and may request Application-owned domain clarification, but it SHALL NOT modify Foundation, FSATS, or other Application-owned files to satisfy those needs.