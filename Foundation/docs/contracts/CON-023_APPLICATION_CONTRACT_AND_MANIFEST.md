# CON-023 — Falcon Application Contract and Manifest

**Version:** 1.1  
**Status:** Approved  
**Approval Record:** GOV-063  
**Documentary Activation:** Active  
**Activation Record:** GOV-092; GOV-093; GOV-094  
**Effective Documentary Instant:** 2026-07-31 22:54:57 +03:00  
**Identifier:** CON-023  

## Required Declaration

Every Falcon Application SHALL declare:

- immutable Application identity, version, owner, and purpose;
- package identity, provenance, integrity, compatibility, and lifecycle state;
- owned business boundary and prohibited Foundation responsibilities;
- dependencies and compatible versions;
- required Foundation services and contracts;
- provided capabilities and consumers;
- permissions, authority requests, and security profile;
- resource requirements, minimums, ceilings, priorities, and degraded behavior;
- persistence, communication, configuration, and evidence requirements;
- installation, validation, registration, admission, activation, update, suspension, recovery, replacement, and removal behavior;
- health reporting and failure-containment interfaces;
- the single MSA identity;
- every major Application branch and exactly one responsible LSA identity for each branch;
- optional CSA identities and eligibility policy for eligible intelligent components only;
- self-development origin, ownership, evidence, origin-aware escalation path, and review interfaces;
- Application Guardian requirement and protection interface;
- rollback or approved corrective-action plan.

## Rules

- undeclared capability, dependency, route, permission, resource, or authority SHALL be denied;
- contract validity SHALL not imply admission, authority, activation, business approval, or production approval;
- Foundation SHALL treat business payloads as opaque except where a separately governed security inspection rule applies;
- purpose or ownership changes are material identity changes and require governed review;
- Application-provided capability SHALL NOT become a Foundation service merely through reuse;
- the Manifest and every lifecycle decision SHALL remain immutable, attributable, reconstructable, and independently challengeable.

The Manifest SHALL select the path matching the actual proposal origin:

- `CSA → Parent LSA → Application MSA → FSA`;
- `LSA → Application MSA → FSA`;
- `Application MSA → FSA`;
- `FSA → separate Foundation self-development governance and approval lifecycle` for Foundation-originated proposals.

It SHALL NOT insert a lower awareness entity beneath the actual origin. FSA review SHALL be recorded as OS-governance and compatibility review only and SHALL NOT be represented as documentary activation, implementation approval, deployment approval, or production adoption. Final activation and adoption references SHALL identify separately authorized Project Owner and governance decisions under GOV-AUT-001 and GOV-001.
