# Stage 6 Scope Discovery and Foundation Gap Review

Status: DESIGN CANDIDATE / NO IMPLEMENTATION AUTHORITY
Date: 2026-08-08
Branch: foundation-development

## 1. Starting authority

Stage 0 through Stage 5 are accepted and closed. The Owner authorized Stage 6 planning and design only. Stage 6 implementation is not yet authorized.

## 2. Canonical gap selected for Stage 6 design

The strongest current-effective Foundation obligation not yet owned by a completed stage is `SYS-006 — Multi-Level Resource Governance`.

SYS-006 assigns Foundation ownership of:
- total-resource truth;
- protection floors;
- recovery reserves;
- Application allocations;
- quotas and ceilings;
- technical priority classes;
- pressure handling; and
- governed redistribution.

It also requires attributable allocation decisions and forbids Applications from exceeding grants, consuming another Application's allocation, converting business preference into Foundation technical criticality, hiding pressure/dependency failure, or self-approving additional Foundation resources.

## 3. Why this is not Stage 5 WP-06 duplication

Stage 5 WP-06 owns Service Bus delivery semantics and bounded flow control after a governed route decision. It consumes pressure/priority evidence for transport behavior, but it does not own system-wide resource inventory, Application allocations, protection floors, recovery reserves, resource grants, resource ceilings or global redistribution.

Stage 6 therefore must become the upstream generic Foundation owner of resource truth and resource-governance decisions consumed by later bounded services such as delivery flow control.

## 4. FCR relevance

Direct/material planning inputs:
- FCR-0007: generic evidenced request boundary for additional Foundation-controlled resources.
- FCR-0010: per-Application resource pressure/allocation state and load-shedding/restoration signals.

Related but not Stage 6 implementation authority:
- FCR-0009 latency/QoS transport may consume resource/pressure truth but remains a distinct transport capability.
- FCR-0011 non-Live isolation may consume resource/security policy but its egress enforcement is distinct.
- FCR-0012 FSA governance may consume resource evidence but does not belong to resource governance itself.
- FCR-0013/FCR-0014 external-service egress and credentials remain separate capabilities.
- FCR-0004/FCR-0005/FCR-0006 remain communication/event/application-integration concerns already partially addressed by Stage 5 and independently governed under Issue #1.

No FCR grants implementation authority.

## 5. Proposed Stage 6 purpose

**Stage 6 — Foundation Resource Governance and Operational Pressure Control**

Purpose: implement the Application-neutral resource truth, allocation, enforcement-state, request, redistribution, pressure and restoration governance required by SYS-006 without creating Application business semantics or a second lifecycle/communication owner.

## 6. Non-scope

Stage 6 design does not include:
- Trading capital allocation, portfolio allocation or financial Risk semantics;
- strategy/market/broker/provider prioritization;
- Application-internal allocation algorithms beyond enforcing the Foundation grant boundary;
- QoS transport redesign owned by a separate capability;
- Internet/external-service egress or credential use;
- FSA autonomous-promotion governance;
- deployment/runtime activation/baseline activation;
- Stage 7 through Stage 9 implementation.

## 7. Required architectural invariants

- `BUSINESS_IMPORTANCE != FOUNDATION_TECHNICAL_CRITICALITY`
- `RESOURCE_REQUEST != RESOURCE_GRANT`
- `RESOURCE_AVAILABILITY != RESOURCE_AUTHORITY`
- `APPLICATION_INTERNAL_PRIORITY != FOUNDATION_PRIORITY_CLASS`
- `PRESSURE_OBSERVED != PERMISSION_TO_EXCEED_CEILING`
- `TEMPORARY_GRANT != PERMANENT_ENTITLEMENT`
- `RECOVERY != SILENT_AUTHORITY_EXPANSION`
- no Application may consume another Application's allocation;
- Foundation remains valid with zero Applications;
- resource decisions must be deterministic, attributable, evidence-bound and fail closed when enforcement truth is unavailable or ambiguous.

## 8. Discovery conclusion

Stage 6 should proceed as a resource-governance stage rooted in current-effective SYS-006, with FCR-0007 and FCR-0010 as direct Application-facing planning inputs. This is a design proposal only until the Owner accepts the Stage 6 design and Work Package map.
