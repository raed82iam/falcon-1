# FCR-0005 — Self-Awareness Research-Only Internet Egress Boundary

**Status:** PROPOSED  
**Requester:** Application workstream  
**Application:** FSATS Applications using MSA/LSA/eligible CSA research capabilities  
**Classification:** `PLANNED_CAPABILITY_INPUT`  
**Date:** 2026-08-07

## Requested Foundation capability

A generic governed research-only Internet egress/security boundary for Application MSA/LSA/eligible CSA entities that permits research, learning, discovery and development while preventing that path from becoming operational-data ingress or live business-decision authority.

## Application use case

FSATS awareness entities may research the Internet to improve their owned capabilities. Operational trading data, provider truth and market truth must remain separate and enter the trading domain through FSAPMA and governed operational contracts.

The research path therefore needs generic controls that preserve room ownership, attribution, security, evidence, isolation and non-operational use.

## Current Foundation evidence checked

- APP-001 Application Boundary and Lifecycle;
- CON-023 Application Contract and Manifest;
- ADR-I012 Plug-and-Play Application Integration Boundary;
- ADR-I015 Application and Awareness Alignment;
- current Foundation security/resource/application-neutral boundaries.

## Observed gap

`PLANNED / NOT YET CONFIRMED AVAILABLE FOR THIS USE CASE`.

FSATS has a concrete requirement for research-only egress but SHALL NOT invent a local Foundation/security bypass.

## Application-side alternatives considered

1. Allow awareness entities unrestricted Internet access — rejected; creates operational-data/security leakage risk.
2. Route awareness research through FSAPMA — rejected; conflates operational trading-data ownership with research/development Internet use.
3. Disable all research Internet use permanently — conflicts with approved self-development intent.

## Required outcome

A generic Foundation-owned boundary should be able to permit attributable research-only egress for an authorized Application awareness entity while enforcing that:

- the requester identity, Application, room and purpose are declared;
- access is scoped, policy-controlled and auditable;
- retrieved material is tagged as research/non-operational evidence;
- research output cannot directly enter live command/decision paths;
- the path cannot bypass Application/room ownership or Foundation security;
- secrets, credentials, privacy and data-exfiltration controls remain Foundation-governed;
- adoption of any resulting improvement still follows isolated testing, evidence, awareness escalation and separate Owner/governance approval.

## Blocking impact

V1.4 architecture and offline/self-contained awareness design can continue.

Future direct Internet research by MSA/LSA/CSA remains blocked until an appropriate Foundation capability/security boundary is available and separately authorized.

## Authority rule

This FCR is a request/design input only. It grants no Foundation modification, Internet connectivity, implementation, deployment, runtime, trading or production authority.
