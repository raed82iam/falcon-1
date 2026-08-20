# FSATS V1.4 Part 0 / P0-E — Owner, Capability and Unresolved-Field Hardening

**Status:** `EFFECTIVE_FOR_CURRENT_P0E_CANDIDATE`
**Parent candidate:** `76_P0E_CANONICAL_APPLICATION_IDENTITY_AND_MANIFEST_CONTRACT.md`
**Remediates:** `RT-P0E-01` through `RT-P0E-04`
**P0-E final Owner acceptance:** `NOT_GRANTED`

## 1. Canonical Application-owner role identities

CON-023 `Owner` SHALL use the exact canonical Application-owner role identity below unless a later governed ownership change is explicitly approved.

| Application | Canonical Owner Role ID |
|---|---|
| `falcon.app.trading.guardian` | `falcon.role.app-owner.trading.guardian` |
| `falcon.app.trading.fsapma` | `falcon.role.app-owner.trading.fsapma` |
| `falcon.app.trading.core` | `falcon.role.app-owner.trading.core` |
| `falcon.app.validation.fstsima` | `falcon.role.app-owner.validation.fstsima` |
| `falcon.app.shared.communication` | `falcon.role.app-owner.shared.communication` |
| `falcon.app.shared.web` | `falcon.role.app-owner.shared.web` |

These are governance role identities, not human usernames, deployment identities, signing identities, runtime principals, MSA identities or the Project Owner identity.

A human/agent may act for an owner role only through separately governed authority. Changing the canonical owner role is a material ownership change, not an ordinary deployment or package update.

## 2. P0-E provided-capability family inventory

P0-F defines exact cross-Application contracts, schemas, message families and routes. It SHALL NOT invent a new business capability owner inconsistent with this inventory.

### Guardian

Provided capability families:

- `guardian.protection-state`
- `guardian.protection-command-intent`
- `guardian.threat-crisis-assessment`
- `guardian.recovery-release-state`
- `guardian.protection-evidence`

Intended external consumer classes:

- Trading Application;
- FSAPMA where protection/degraded-provider action is explicitly contracted;
- Shared Web for presentation only where explicitly contracted;
- Shared Communication for notification/report delivery only where explicitly contracted;
- FSA/Foundation governance interfaces only through declared governance/evidence contracts, never Trading business ownership.

### FSAPMA

Provided capability families:

- `provider.capability-truth`
- `provider.data-product`
- `provider.selection-fallback-outcome`
- `provider.data-quality-lineage`
- `provider.capacity-quota-cost-state`
- `provider.onboarding-role-evidence`

Intended external consumer classes:

- Trading Application;
- Guardian where provider/data quality is protection-relevant and explicitly contracted;
- FSTSimA only through non-Live/test-specific governed interfaces where applicable;
- Shared Web/Communication only for presentation/notification use through declared contracts.

FSAPMA does not provide authoritative broker execution/order/fill/position capability truth.

### Trading

Provided capability families:

- `trading.business-readiness`
- `trading.decision-evidence`
- `trading.risk-state-summary`
- `trading.portfolio-position-projection`
- `trading.execution-status`
- `trading.learning-performance-evidence`

Intended external consumer classes:

- Guardian for protection/reconciliation evidence as explicitly contracted;
- Shared Web for presentation/interaction projection;
- Shared Communication for notifications/reports;
- FSTSimA only through non-Live validation interfaces;
- FSA/Foundation governance interfaces only through declared evidence/governance contracts.

Trading does not expose private mutable strategy/Risk/position internals as a cross-Application shared state surface.

### FSTSimA

Provided capability families:

- `simulation.validation-evidence`
- `simulation.fidelity-assessment`
- `simulation.scenario-result`
- `simulation.replay-result`
- `simulation.failure-stress-result`

Intended external consumer classes:

- owning candidate/Application review chains;
- Trading/Guardian/FSAPMA only as non-authoritative validation evidence consumers;
- FSA/Owner governance review interfaces where explicitly contracted.

FSTSimA provides no Live authority or operational source truth.

### Shared Communication

Provided capability families:

- `communication.delivery-outcome`
- `communication.recipient-acknowledgement`
- `communication.escalation-state`
- `communication.external-channel-evidence`

Intended external consumer classes:

- requesting Applications that originated the communication request;
- Shared Web where user-facing acknowledgement/status presentation is explicitly contracted.

Communication does not provide source business truth or mutate source state.

### Shared Web

Provided capability families:

- `web.user-command-intent`
- `web.user-consent-evidence`
- `web.presentation-interaction-state`

Intended external consumer classes:

- the exact authoritative Application targeted by the user command through a P0-F contract;
- FSA Owner-governance interface only as presentation/interaction transport when separately governed; Shared Web never becomes the authority source.

Shared Web provides no Trading/provider/Guardian/Foundation business authority.

## 3. Consumer declaration rule

An intended consumer class is not route authority and does not grant access.

P0-F SHALL bind each allowed capability family to exact producer/requester, consumer/responder, schema, authority, permission, route and failure semantics.

A capability family or consumer class absent from this P0-E inventory requires governed semantic review before it may become a CON-023 provided-capability declaration.

## 4. Unresolved-field provenance model

No authority-bearing Manifest field may be represented as ordinary `TBD`, `TODO`, empty, wildcard, `latest`, unlimited, auto-discovered-by-convenience or implementation-defined.

When P0-E cannot yet supply an exact value because an authorized later Part or Foundation disposition owns that decision/evidence, the field SHALL be represented in design as:

```text
UNRESOLVED_FIELD
OWNER_SOURCE = <exact later Part / Foundation contract / FCR / evidence authority>
RESOLUTION_GATE = <validation | admission | activation | feature enablement>
DEFAULT = NONE
FAILURE = FAIL_CLOSED
```

This is a controlled unresolved dependency, not permission for implementation invention.

## 5. Field-provenance matrix

| Manifest field family | Authoritative source for exact value | Gate if unresolved |
|---|---|---|
| Application ID / Package ID / MSA / LSA | P0-E accepted record | `BLOCK_VALIDATION` |
| Owner role ID | P0-E accepted record | `BLOCK_VALIDATION` |
| Application/Package version | exact release/package build process under later implementation authority | `BLOCK_VALIDATION` |
| Package/Manifest digest and provenance | exact build/release evidence | `BLOCK_VALIDATION` |
| Foundation contract IDs/versions | current approved Foundation contract registry + compatibility evidence | `BLOCK_VALIDATION` |
| Cross-Application capability family | P0-E accepted record | `BLOCK_VALIDATION` if owner/family absent |
| Cross-Application schema/message/route/permission IDs | P0-F accepted design + Foundation capability/FCR disposition | `BLOCK_ADMISSION_OR_FEATURE` according to dependency |
| Guardian command/protection contract detail | P0-I + P0-F + applicable Foundation/FCR disposition | `BLOCK_PROTECTED_FEATURE` |
| FSAPMA operational-data contract detail | P0-G + P0-F + applicable Foundation/FCR disposition | `BLOCK_OPERATIONAL_DATA_FEATURE` |
| Trading business detail needed by Manifest | P0-H | `BLOCK_RELEVANT_FEATURE` |
| technical resource minimum/normal/ceiling/priority | P0-J evidence + SYS-006/Foundation admission policy | `BLOCK_ACTIVATION` |
| latency/QoS declarations | P0-J + FCR-0009/Foundation disposition | `BLOCK_CLAIMED_QOS_FEATURE` |
| resource-pressure/load-shedding interface | P0-J + FCR-0010/Foundation disposition | `BLOCK_FOUNDATION_AWARE_LOAD_SHEDDING` |
| FSTSimA non-Live credential/egress enforcement | P0-K + FCR-0011/Foundation disposition | `BLOCK_OPERATIONAL_CONNECTION` |
| research-only awareness Internet egress | P0-C/P0-D semantics + FCR-0008/Foundation disposition | `BLOCK_RESEARCH_EGRESS_FEATURE` |
| bounded autonomous Owner/FSA control plane | P0-C accepted semantics + FCR-0012/Foundation disposition | `BLOCK_AUTONOMOUS_PROMOTION_FEATURE` |
| exact external credentials/secrets | governed security/secret-provider authority at deployment | `BLOCK_ACTIVATION_OR_FEATURE`; never stored in Manifest |
| persistence sizing/retention values where not already governed | owning later design + Foundation storage/security policy | `BLOCK_RELEVANT_STATEFUL_FEATURE` |
| health thresholds/business readiness thresholds | owning Application design + Foundation health contract where applicable | `BLOCK_RELEVANT_FEATURE` |

## 6. Resource-specific hardening

No implementation may choose:

- unlimited CPU/memory/storage/network;
- host maximum;
- equal split among Applications;
- first-come-first-served;
- copied values from another Application;
- values inferred from trading capital;

as Manifest resource requirements without P0-J evidence and Foundation-governed admission/resource policy.

Until exact values are bound:

```text
RESOURCE_REQUIREMENTS = UNRESOLVED_BLOCKING_ACTIVATION
ACTIVATION = DENIED
```

Design/test tooling may use explicitly non-authoritative test allocations, but those values SHALL NOT become canonical Manifest requirements by repetition.

## 7. Permission/security/route hardening

Permission and route identities SHALL originate from accepted P0-F/P0-I/P0-K designs plus available Foundation authority/contracts as applicable.

An Application implementation SHALL NOT mint a local identifier and then treat its existence as a Foundation permission/route.

```text
LOCAL_PERMISSION_NAME != FOUNDATION_AUTHORITY
LOCAL_ROUTE_NAME != ADMITTED_ROUTE
CONFIGURED_ENDPOINT != AUTHORIZED_EGRESS
```

If the exact required Foundation capability remains open through an FCR, the relevant feature remains disabled/fail-closed according to the field-provenance matrix.

## 8. Owner, signer, operator and awareness separation

For every package/evidence record, the following identities SHALL remain distinguishable:

```text
APPLICATION_OWNER_ROLE
PROJECT_OWNER / GOVERNANCE AUTHORITY
PACKAGE_PRODUCER
PACKAGE_SIGNER / PROVENANCE IDENTITY
DEPLOYMENT OPERATOR
RUNTIME INSTANCE
APPLICATION MSA
APPLICATION LSAs
```

Possession of one identity or role creates none of the others.

## 9. Effective interpretation

`76` and `76A` SHALL be read together.

Where `76` defers exact values to later design/implementation evidence, `76A` makes the source and fail-closed gate explicit. No deferred field may be filled by convenience.

```text
RT-P0E-01 = REMEDIATED_PENDING_FRESH_REVIEW
RT-P0E-02 = REMEDIATED_PENDING_FRESH_REVIEW
RT-P0E-03 = REMEDIATED_PENDING_FRESH_REVIEW
RT-P0E-04 = REMEDIATED_PENDING_FRESH_REVIEW
P0E_FRESH_RED_TEAM = REQUIRED
P0E_FINAL_OWNER_ACCEPTANCE = NOT_GRANTED
```
