# Shared Falcon Web Deployment Portability

**Status:** ACTIVE IMPLEMENTATION RULE  
**Branch:** `web-development`  
**Scope:** `applications/shared/web/**`  
**Runtime deployment authority:** NOT GRANTED BY THIS DOCUMENT

## Purpose

Shared Falcon Web shall depend on deployment capabilities, not on permanent vendor identity.

Initial operational candidates may use Cloudflare for edge capabilities and OCI for compute/hosting, but neither vendor is part of Falcon identity, business truth, or constitutional authority.

```text
FALCON_DEPENDS_ON_CAPABILITY != FALCON_DEPENDS_ON_VENDOR
CURRENT_PROVIDER != PERMANENT_PROVIDER
PROVIDER_REPLACEMENT != APPLICATION_REDESIGN
```

## Capability model

The Web deployment model names the capabilities it needs:

- Edge DNS
- Edge CDN
- Edge WAF
- TLS termination
- Compute
- Object storage
- Secrets capability
- Observability

Provider bindings are replaceable adapters/configuration choices outside presentation and business semantics.

The implementation model lives at:

`src/core/deployment-profile.js`

## Initial candidates

Current initial operational direction, subject to separate deployment authority and later verification:

```text
EDGE_DNS / EDGE_CDN / EDGE_WAF / TLS_TERMINATION
→ initial candidate: Cloudflare

COMPUTE / HOSTING
→ initial candidate: Oracle Cloud Infrastructure (OCI)
```

These are candidates, not permanent architecture dependencies.

A later move to another suitable provider must not require changing:

- Falcon Vision or Constitution;
- Shared Web feature code;
- FSATS business semantics;
- Foundation internals;
- user-facing application contracts.

Only the relevant deployment binding/configuration and provider-specific operational adapter should change, unless a materially different capability contract is deliberately approved.

## Fail-closed rule

A provider binding has explicit lifecycle state:

```text
UNBOUND
CANDIDATE
CONFIGURED
VERIFIED
```

Only `VERIFIED` may be treated by Web deployment policy as usable.

A selected provider name, configured account, DNS record, or successful login does not prove a verified production capability.

```text
PROVIDER_SELECTED != DEPLOYMENT_AUTHORIZED
PROVIDER_CONFIGURED != PROVIDER_VERIFIED
DNS_RESOLVES != APPLICATION_HEALTHY
EDGE_ACCEPTS_REQUEST != ORIGIN_TRUSTED
UI_AVAILABLE != FOUNDATION_OR_APPLICATION_HEALTHY
```

## Secret boundary

Provider credentials, API tokens, private keys, passwords, and secret material shall not be embedded in the provider-neutral deployment profile, source code, UI, or ordinary repository configuration.

The profile may hold only non-secret provider identity/reference metadata required to bind a capability. Secret storage and retrieval must use the separately governed secret-management capability when runtime deployment is authorized.

## Replacement acceptance test

A provider replacement is architecturally acceptable only when all of the following remain true:

1. required capability behavior is preserved or improved;
2. Falcon authority boundaries remain unchanged;
3. no business truth migrates into the provider;
4. no UI code gains provider-specific dependency;
5. secrets remain outside source/presentation state;
6. failure behavior remains fail-closed;
7. observability and rollback evidence are available;
8. security and performance requirements pass verification;
9. provider exit remains practical;
10. governed deployment authority exists before production activation.

## Current implementation boundary

This document and `src/core/deployment-profile.js` implement portability policy only.

They do not deploy Cloudflare, OCI, another provider, production DNS, WAF rules, containers, secrets, certificates, or live runtime connectivity.

Production deployment remains a separate governed gate.
