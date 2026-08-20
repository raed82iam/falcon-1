# Stage 3 WP-05 Bootstrap and Lifecycle Control Design

## Canonical bootstrap policy

`BootstrapPolicyCatalog` owns one immutable Stage 3 WP-05 policy. The validation request contains evidence only and cannot provide its own expected authority, environment, source, scope, time provider, graph identity, graph version, graph digest, activation position, or authority boundary.

The policy binds the accepted WP-04 dependency graph digest:

`BA6CEF2A5E86EE12FA47A9A2CE31EF89B424BFF43EFEF05214788B086295D44E`

## Bootstrap validity

The accepted decision retains the earliest expiry among policy, subject admission, context, provenance, time provider, dependency evidence, restriction, and release evidence. Entry to `INITIALIZING` or bootstrap `RESTRICTED` fails closed at or after that boundary.

## Identity ledger

The lifecycle controller reserves each non-empty request, transition, and event identity at the first call, before contract and subject validation. A later attempt cannot reuse an identity consumed by an accepted or rejected request.

## Lifecycle evidence bundle

Each transition carries records for:

- authority decision;
- admitted time provider;
- dependency readiness when required;
- protective restriction when imposed or active;
- controlled release when entering recovery from restriction;
- independent recovery validation before returning to `READY`.

The infrastructure validates each record and computes a canonical evidence-bundle digest. Internal booleans exist only after those validations and are not part of the public request surface.

## Lifecycle model

The canonical model is version `1.1`. It retains the previous states and adds `STOPPED → RECOVERING`. When restriction remains active, this transition requires a release record bound to the active restriction, exact transition request, release authority, and new authority decision.

`RECOVERING → READY` requires an effective independent recovery record bound to the subject, bootstrap context, transition request, and authority decision.

`RETIRED` remains terminal, restart attempts remain bounded, and rejected transitions emit no success event.
