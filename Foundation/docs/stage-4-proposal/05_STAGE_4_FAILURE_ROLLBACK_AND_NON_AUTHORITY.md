# Stage 4 Failure, Rollback, and Non-Authority Plan

## Fail-Closed Rules

The following must never produce inferred permission or inferred success:

- missing actor identity;
- missing authority provenance;
- expired or revoked authority;
- ambiguous policy;
- stale source state;
- competing state version;
- missing owner;
- conflicting owner;
- uncertain write outcome;
- missing evidence;
- integrity failure;
- restart divergence.

## Rollback Model

### Code rollback before WP acceptance

A future implementation authority must define exact file restoration and evidence invalidation.

### Accepted state rollback

An accepted authoritative state revision is never deleted or rewritten.

Reversal requires a new governed compensating action with:

- new request;
- new authority decision;
- new state version;
- explicit link to the prior accepted fact;
- complete evidence.

### Evidence correction

Evidence is not edited.

A correction is a new integrity-linked record.

## Non-Authorities

Stage 4 planning does not authorize:

- source-code implementation;
- project or solution modification;
- tests or verifier creation;
- new dependencies;
- storage-technology selection;
- staging, commit, tag, merge, rebase, push, or movement of `main`;
- deployment or runtime activation;
- external connectivity;
- broker or market-data access;
- trading or financial activity;
- Stage 5 work.

## Residual Risks for Later Implementation Review

- the initial persistence provider may prove only local deterministic behavior;
- distributed consensus is not a Stage 4 claim;
- Stage 5 messaging has not yet been implemented;
- Guardian enforcement remains separate;
- independent recovery release remains separate;
- high-load production performance requires separately scoped evidence.
