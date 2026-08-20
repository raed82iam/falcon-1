# Stage 6 Owner Priority Clarification and Application Input Request

Status: RECORDED / DESIGN INPUT
Date: 2026-08-08
Branch: foundation-development

## Owner clarification

The Owner clarified that Trading-related Applications are to receive the highest cross-Application resource priority in Falcon.

Falcon Foundation is not a Trading-only platform. Future Applications may include Accounting, Warehouse and other business systems, all of which may consume Foundation-governed technical resources.

Under pressure, Foundation may reclaim technical resources from lower-priority Applications and redistribute them to the highest-priority Trading workload. Under severe/critical conditions, all legitimately reclaimable lower-priority Application allocation may be reduced or reclaimed when required for the highest-priority Trading workload.

This does not authorize destruction of the Foundation control plane. Foundation survival/protection, Authority, Health/Recovery, evidence/security integrity and the minimum resources required to govern/revoke/restore Applications remain protected control resources.

## Existing Trading-side evidence already found

The Application workstream already records:
- FCR-0007: Trading Guardian -> Foundation resource escalation request boundary;
- FCR-0010: per-Application resource pressure/allocation and load-shedding signals;
- CA-008 in the FSATS Cross-Application Contract Matrix: Application Resource Escalation Request from Guardian to Foundation Guardian/resource authority;
- Trading Guardian does not allocate Foundation resources; Foundation retains resource decision authority.

## Missing Application-owned details

Foundation does not yet have sufficient authoritative Application detail to decide:
- whether Guardian is the only FSATS principal allowed to request emergency resources;
- whether Trading or FSAPMA may submit ordinary/non-emergency capacity requests;
- which internal roles may originate evidence for an Application-level request;
- exact message families and fields for ordinary request, emergency escalation, pressure/allocation projection, grant/deny/cap, rebalance/restoration and revocation/reduction;
- the internal Trading degradation/protection hierarchy;
- how each Trading Application reacts to Foundation reduction, reclamation, denial, temporary grant, revocation and restoration.

Those are Application-owned semantics and must not be guessed by Foundation.

## Clarification requests sent

Foundation posted design-clarification requests in the existing canonical FCR channels:
- FCR-0007 / Issue #7 comment id 5226836415;
- FCR-0010 / Issue #10 comment id 5226837250.

The requests ask the Application designer to provide exact `application-development` commit/file evidence for the missing details.

## Stage 6 treatment

Stage 6 design is updated now to preserve the Owner's cross-Application priority rule while leaving Trading-internal resource/degradation semantics opaque and pending Application declaration.

No new FCR is created by Foundation because the repository FCR protocol defines FCRs as Application requests to Foundation. The correct reverse-direction action is a Foundation clarification request on the existing relevant FCR records.

This record grants no Stage 6 implementation authority.
