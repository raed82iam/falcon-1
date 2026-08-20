# CON-024 — Application Guardian Registration

**Identifier:** CON-024 — Proposed Reservation  
**Version:** Proposed 1.0  
**Status:** Proposed  
**Stage 1 Authority:** Not Granted

## Registration

Registration SHALL bind Guardian ID/version, Suite/domain, owner, artifact/provenance, authority instrument, permitted domain restrictions, CON-022 request classes, health/evidence Contracts, FFG route, standby/failover, recovery, compromise, expiry/revocation, and Manifest reference.

## Rules

- registration does not grant authority beyond the validated instrument;
- a Guardian cannot register itself conclusively;
- FFG and Service Catalog store technical metadata only;
- unsupported, stale, revoked, compromised, or mismatched registration SHALL fail closed;
- request authority SHALL be explicitly enumerated;
- Guardian removal SHALL revoke request authority and reconcile active restrictions;
- history SHALL remain immutable and challengeable.

