# Falcon Foundation Workstream Rules

**Branch:** `foundation-development`  
**Scope:** Falcon Foundation only  

## Core boundary

Foundation work may evolve Foundation-owned code, contracts, governance, tests, verifiers, evidence, and documentation only under existing Falcon authority and closure/remediation rules.

Foundation work SHALL NOT create, redesign, or modify Application-owned business logic under `applications/**` and SHALL NOT rewrite historical/scratch Application references under `reference/**`.

## Application interaction

Foundation remains Application-neutral and valid with zero Applications. FSATS, the Shared Falcon Web Application, and any other Application are consumers of the approved generic Plug-and-Play boundary, not privileged Foundation owners.

Application requests do not create Foundation authority. A Foundation Capability Request (FCR) must be assessed against Falcon Vision, Constitution, architecture, ownership, non-duplication, and current Foundation responsibilities. Foundation may identify an existing capability, accept a generic Foundation need under separate authority, or reject the request as Application-owned.

## Parallel-work rule

The Foundation page/workstream reads Application and Shared Web requirements only as input. It does not write Application files.

The ordinary Application page/workstream reads current Foundation authority/contracts as input. It does not write Foundation files.

The Shared Falcon Web Application page/workstream reads current Foundation and Application authority/contracts as input. It writes only within its separately authorized Web-owned subtree on `web-development` and does not write Foundation or ordinary Application-owned files.

The Foundation workstream SHALL treat both `application-development` and `web-development` as read-only external workstream branches.

Any required cross-boundary change is routed through the Project Owner and the shared FCR protocol rather than performed directly by the other workstream.

## FCR coordination rule

The repository-wide FCR shared protocol currently permits only:

- `Waiting On: FOUNDATION`
- `Waiting On: APPLICATION`
- `Waiting On: WEB`
- `Waiting On: NONE`

`Waiting On: OWNER` is prohibited by the current Project Owner clarification in GitHub Issue #1. If Foundation requires an Owner decision or clarification, the FCR remains `Waiting On: FOUNDATION`; Foundation asks the Owner directly, then Foundation completes the resulting disposition. The same rule applies correspondingly to Application and Web.

When `Waiting On: FOUNDATION`, Foundation owns the immediate next action.

When `Waiting On: APPLICATION`, Foundation SHALL NOT answer the Application-owned business/domain decision on its behalf.

When `Waiting On: WEB`, Foundation SHALL NOT answer the Shared Web-owned design/compatibility decision on its behalf.

`Waiting On: NONE` is reserved for cases where no workstream has a remaining immediate lifecycle obligation under the current FCR disposition.

GitHub Issue #1, `FCR Shared Registry and Operating Protocol`, is the canonical current FCR lifecycle source. If an older Foundation document or historical comment conflicts with its current header/protocol, Issue #1 controls prospectively while the historical record remains unchanged.

FCR participation never grants cross-workstream file-write authority.

## CI boundary

`Falcon Foundation CI` rejects changes on `foundation-development` that touch `applications/**` or `reference/**`, in addition to running the configured build, tests, and current Foundation verifiers.
