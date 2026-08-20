# Stage 5 WP-03 — Implementation Boundary

**Status:** Active implementation boundary  
**Authority:** Stage5-WP03-Implementation-Authorization-20260807-172400  

## In scope

WP-03 may implement only the Application Communication Manifest declaration and validation surface, including immutable Manifest structures, schema-reference validation through WP-02, deterministic canonicalization and SHA-256 binding, bounded Manifest registration/resolution, fixtures, verifier coverage, architecture/security coverage, and documentation.

## Required dependencies

The WP-03 production project may depend only on:

- `Foundation.Contracts` for accepted WP-01 canonical messaging primitives; and
- `Foundation.SchemaRegistry` for accepted WP-02 schema identity/version resolution.

No reverse dependency from those accepted predecessors to WP-03 is permitted.

## Explicitly out of scope

WP-03 shall not implement:

- WP-04 message admission;
- WP-05 Service Bus routing;
- WP-06 delivery semantics or flow control;
- WP-07 event publication/journaling;
- WP-08 cryptographic message protection;
- WP-09 Application attachment, upgrade, replacement, draining, or detachment;
- WP-10 integrated closure;
- runtime communication execution;
- Application activation;
- business payload interpretation;
- FSA, MSA, LSA, CSA, Guardian, or recovery-governance implementation;
- modifications under `applications/**` or `reference/**`.

## Fail-closed rule

Ambiguous, malformed, duplicate, conflicting, unresolved, or non-canonical required declarations are rejected. Technical validity never implies authority.

## Workstream

All implementation occurs on `foundation-development`. `application-development` and `reference/fsats-v1.3-scratch` remain read-only/out of scope.
