# Stage 16 Implementation Plan and Pre-Implementation Red Team

Date: 2026-08-17
Stage: **Stage 16 — Authoritative Identity, Authentication, Session and MFA Runtime**

## Implementation plan

### WP-01 — Independent identity-runtime ownership

Create `Foundation.IdentityRuntime` as an independent production assembly with its own namespace and zero production `ProjectReference` dependencies.

### WP-02 — Provider-neutral verified external assertion ingestion

Accept only an assertion that already carries explicit verifier evidence, provider/issuer/audience/subject identity, assertion identity, nonce, issue/expiry time and a positive cryptographic-verification result. Stage 16 does not perform network OIDC connectivity.

### WP-03 — Explicit Falcon identity and external-identity link registry

Falcon identity is created/maintained independently from external identity. Linking requires exact provider + issuer + external subject and exact Falcon identity. Email/name/phone are never linking authority. Duplicate/ambiguous links fail closed.

### WP-04 — Falcon identity status and role/entitlement facts

Identity status and declared role/entitlement identifiers are authoritative identity facts only. They are not business permissions. Disabled/revoked identities cannot create new sessions.

### WP-05 — MFA opaque authenticator references and replay-safe challenge proof

Store only an opaque authenticator reference and type. Reject secret-like material. MFA verification requires exact challenge identity, exact authenticator reference, identity match, issue/expiry bounds and one-time challenge consumption.

### WP-06 — Session issuance, rotation, revocation and logout

Sessions bind exact Falcon identity, exact authentication evidence, required authentication assurance, MFA evidence when policy requires it, issue/expiry and a unique session identity. Rotation revokes the predecessor. Revoked/expired/replaced sessions fail closed.

### WP-07 — Security Context projection

Produce a bounded downstream Security Context projection carrying authenticated subject identity, authentication method/assurance, session identity, trust boundary, issue/expiry, revocation state and provenance/evidence. Projection is authentication/trust truth only and cannot mint downstream authorization.

### WP-08 — High-assurance policy

A caller may require MFA and a minimum authentication assurance for session issuance. Owner identity is never inferred from provider/email/MFA; Owner role remains an authoritative Falcon identity fact and downstream authorization remains separate.

### WP-09 — Architecture and security guards

Guard:

- exact assembly and namespace ownership;
- zero production project references;
- absence of live HTTP/OIDC provider clients;
- absence of persisted secret/password/token/OTP-seed properties;
- absence of predecessor namespace ownership leakage.

### WP-10 — Integrated Stage 16 verifier

Adversarially verify all positive and negative paths, run twice, compare deterministic output, then run the full governed predecessor chain.

## Pre-implementation Red Team

The following attacks are mandatory Stage 16 adversarial cases.

| Attack | Required result |
|---|---|
| Email auto-link to Falcon identity | fail closed |
| Display-name/phone auto-link | fail closed |
| Same external subject linked to two Falcon identities | reject |
| Same provider subject with wrong issuer | no link |
| Unverified provider assertion | reject |
| Missing verifier evidence | reject |
| Wrong audience | reject when expected audience is supplied |
| Expired assertion | reject |
| Future-issued assertion outside accepted clock | reject |
| Replayed assertion ID | reject |
| Replayed nonce/assertion pair | reject |
| Provider assertion contains Owner/admin role | must not create Falcon role or authority |
| MFA passed but Falcon identity disabled | session deny |
| MFA passed but no explicit Falcon identity link | session deny |
| MFA challenge replay | reject |
| Wrong authenticator reference for challenge | reject |
| Wrong identity uses another identity's authenticator | reject |
| Secret-like authenticator value | reject |
| Session fixation by duplicate session ID | reject |
| Session rotation leaves old session usable | fail, old must be revoked |
| Revoked session reused | reject |
| Logged-out session reused | reject |
| Expired session projected as current | reject |
| Stale external assertion used for new session | reject |
| Identity disabled after session issuance | projection must fail closed/revoke effective trust |
| External provider role becomes business authority | prohibited |
| Falcon role becomes business authority | prohibited by boundary |
| Web-supplied Owner flag | ignored/not represented as identity authority |
| Security Context treated as authenticator | prohibited by API ownership |
| Session presentation treated as authority issuance | prohibited |
| Live HTTP/provider network dependency | prohibited |
| Secret/password/token/OTP seed persistence | prohibited |
| Cross-provider subject collision | isolated by provider+issuer+subject |
| Ambiguous/duplicate identity link | fail closed |
| Wrong trust-boundary projection | reject |
| Missing provenance/evidence | reject |
| Missing correlation/session evidence | reject |
| Authentication success without required MFA | deny session |
| MFA success without sufficient authentication assurance | deny session |
| Session rotation changes Falcon identity | reject |
| Session rotation weakens assurance | reject |
| Session rotation drops required MFA evidence | reject |
| Revoked authenticator used for new MFA proof | reject |
| Re-enrollment silently reactivates revoked authenticator identity | reject duplicate/ref state conflict |
| No Applications registered/running | Foundation identity runtime remains valid |

## Required invariants

```text
AUTHENTICATION != AUTHORIZATION
EXTERNAL_ASSERTION_VERIFIED != FALCON_IDENTITY_LINKED
FALCON_IDENTITY_LINKED != SESSION_ISSUED
SESSION_ISSUED != BUSINESS_AUTHORITY
MFA_PASSED != BUSINESS_AUTHORITY
ROLE_FACT != AUTHORITY_DECISION
EMAIL_MATCH != IDENTITY_LINK_AUTHORIZATION
WEB_SESSION_PRESENTATION != SESSION_ISSUANCE
SESSION_ROTATION -> PREDECESSOR_REVOKED
REVOKED_SESSION != CURRENT_SESSION
EXPIRED_SESSION != CURRENT_SESSION
REPLAYED_ASSERTION = DENY
REPLAYED_MFA_CHALLENGE = DENY
SECRET_BYTES = OUTSIDE_IDENTITY_RUNTIME_STATE
LIVE_PROVIDER_CONNECTIVITY = OUTSIDE_STAGE16
ZERO_APPLICATION_OPERATION = VALID
```

## Pre-implementation result

No design-level Critical/High/Medium blocker remains after the above fail-closed controls are made mandatory. Implementation is authorized by the Owner's explicit Stage 16 command, but technical acceptance remains blocked on governed executable validation.
