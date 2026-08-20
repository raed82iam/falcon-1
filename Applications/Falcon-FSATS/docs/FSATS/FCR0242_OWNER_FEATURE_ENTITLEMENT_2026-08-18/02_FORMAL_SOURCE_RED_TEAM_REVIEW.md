# FCR-0242 Formal Source Red Team Review

**Date:** 2026-08-18  
**Scope:** exact FCR-0242 source semantics after remediation  
**Result:** PASS at source-review level / executable validation pending

## Attack surface

The review attacked the entitlement boundary as an adversarial consumer/producer path rather than assuming cooperative inputs.

### Identity spoofing

Attacks:
- local `PROJECT_OWNER` role treated as authoritative truth;
- producer self-asserts Owner identity;
- commercial VIP customer substituted for Project Owner;
- revoked Owner identity reused;
- superseded Owner session reused;
- expired/replayed Owner session reused.

Disposition: FAIL CLOSED.

### Catalog spoofing and staleness

Attacks:
- wrong compatibility identity;
- malformed catalog digest;
- duplicate feature identity;
- future-dated catalog;
- expired catalog;
- mutation to a newer catalog while reusing an older decision.

Disposition: FAIL CLOSED or mandatory re-evaluation.

### Commercial lifecycle contamination

Attacks:
- Project Owner represented as commercial VIP subscription;
- trial semantics applied;
- seven-day warning applied;
- Standard downgrade applied;
- upgrade prompt applied;
- Standard feature lock applied.

Disposition: REJECTED.

### Authority smuggling

Attacks attempt to use feature entitlement to mint:
- action authorization;
- trading execution authority;
- broker authority;
- Foundation authority;
- Kill authority;
- runtime activation;
- deployment authority.

Disposition: REJECTED. Accepted decisions return every authority grant false.

### Feature-access undergrant attack

Initial implementation incorrectly excluded a customer-facing VIP feature when the feature required separate action/trading/broker authority. This would have violated the requirement for full VIP feature access by confusing feature visibility with authority to execute the feature.

Remediation applied:

```text
FEATURE_ACCESS != REQUIRED_ACTION_AUTHORITY
```

The Project Owner may receive the customer-facing feature surface while the downstream action remains independently authorized or denied.

### Stale catalog replay attack

Initial implementation carried catalog observation time but no expiry, allowing an otherwise valid old catalog to remain reusable indefinitely.

Remediation applied:
- exact catalog expiry added;
- expired catalogs fail closed;
- entitlement decision expiry is bounded by the earlier of Owner identity/session expiry and catalog expiry.

## Future feature test intent

The dedicated verifier includes a catalog-version mutation adding a new VIP customer-facing feature and requires it to become included after re-evaluation while preserving all authority grants as false.

## Transport attack

The review searched for an existing exact entitlement transport but none was identified. The implementation does not invent one and does not use semantic contract availability as transport authority.

```text
SEMANTIC_CONTRACT_AVAILABLE != LIVE_TRANSPORT_AUTHORIZED
```

## Residual findings after remediation

```text
CRITICAL = 0
HIGH = 0
MEDIUM = 0
LOW_PRODUCT_RUNTIME = 0
```

## Verification caveat

This Red Team is a fresh formal source-level review. The dedicated executable verifier and full Application governed runner are registered but have not yet been executed against the final exact HEAD. No executable PASS is claimed until the planned full Application validation is performed.
