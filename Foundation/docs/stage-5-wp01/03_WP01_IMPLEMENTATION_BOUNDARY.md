# Stage 5 WP-01 Implementation Boundary

## Implemented

- canonical typed identities;
- message kind and classification;
- canonical outcome including UNKNOWN;
- canonical UTC time and expiry metadata;
- immutable canonical envelope;
- deterministic canonicalization and SHA-256 identity;
- fail-closed construction and validation;
- application-neutral verification;
- zero-Application and multi-Application fixtures.

## Explicitly not implemented

- Service Bus;
- dynamic routing;
- schema registry or compatibility policy;
- publish/subscribe;
- event journal;
- cryptographic transport;
- Application attachment, draining, replacement, or detachment;
- FSA, MSA, Guardian, or recovery governance;
- WP-02 through WP-10.

The existing `FilEnvelope` remains preserved.
