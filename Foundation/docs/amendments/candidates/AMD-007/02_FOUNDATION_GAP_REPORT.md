# Foundation Gap Report

**Status:** Proposed

## Material Gaps

| File / section | Current wording/state | Missing requirement and correction | Owner | Severity | Stage 1 impact | Required artifacts |
|---|---|---|---|---|---|---|
| `AWR-001 v1.0` §§1–6 | unified financial and Foundation awareness | replace active meaning with FSA-only AWR-001 v2.0 | AWR | Critical | blocks domain independence | GOV-061 activation package |
| `AUT-002 v1.0` §§1–7 | capital, trade, profit mixed with platform Guardian | activate AUT-002 v2.1 and RSK-006 separation | AUT/RSK | Critical | blocks Guardian implementation | ADR-I011, AUT-002 v2.1, RSK-006 |
| `FDN-005` §§2–5 | prohibited financial path and Trading-specific allowlist examples | version to generic prohibited external/business-authority path; move Trading policy outward | Foundation Protection | High | would leak domain policy | FDN-005 v1.1, release matrix |
| `SPEC-000` | APP-001 only Planned; no APP-002/RSK-006/CON-022 | register proposed successors only after Owner approval and controlled change | Specification Authority | High | admission undefined | registry change |
| `APP-001` | Planned, no content | define package, suite, lifecycle, admission, isolation, rollback, removal | APP | Critical | Plug-and-Play blocked | proposed APP-001 |
| Application Manifest | absent | define technical-only manifest and Guardian declaration | APP/SYS | Critical | installation and isolation blocked | proposed CON-023 |
| Guardian registration | absent | define identity, mandate, capability, health, request authority | APP/AUT | High | generic Guardians blocked | proposed APP-002/CON-024 |
| `SYS-003` | Candidate Migration | proposed SYS-003 now defines Service Catalog registration and technical-only metadata | SYS | High | discovery/readiness blocked until approval | proposed SYS-003 successor |
| `SYS-006` | Candidate Migration | proposed SYS-006 now separates ordinary and emergency resource authority | SYS | High | containment blocked until approval | proposed SYS-006 successor, ADR-I013 |
| technical criticality | minimum classes proposed, no governed catalog | define authority, immutable class meaning, admission review, conflict handling | SYS/AUT | Critical | FFG priority unsafe | ADR-I013 plus catalog |
| Platform Safe survival set | complete proposed OPS-005 prepared | obtain Owner approval and independent review | AUT/OPS | High | blocked until approval | ADR-I013, OPS-005 |
| Runtime/Lifecycle/Bus/FIL protection | generic actions previously implicit | CON-025 through CON-028 now propose exact boundaries | SYS | High | blocked until approval | proposed Contracts |
| restriction-state persistence | obligations previously distributed | CON-029 now proposes canonical state, integrity, reconciliation, restoration gate | OPS/SYS | High | blocked until approval | proposed CON-029 |
| Security/authority enforcement | distributed | CON-030 now proposes restriction enforcement boundary | SEC/AUT | High | blocked until approval | proposed CON-030 |
| audit/evidence | generic CON-008 only | CON-031 now proposes Guardian-specific evidence obligations | OPS/SEC | High | blocked until approval | proposed CON-031 |
| HA, independent stop, compromise | unresolved | ADR-I014, AUT-003, CON-032 now propose complete policy/boundary | AUT/OPS/SEC | Critical | blocked until approval | proposed artifacts |
| consequence/release classes | unresolved | AUT-003 and GDN-001 now propose classes and release authority | AUT/GOV | Critical | blocked until approval and deployment values | proposed artifacts |
| Owner Center | requirements approved, no realization | preserve as prerequisite for candidate decisions | GOV/SEC | High | Self-Evolution blocked | design/authority/profile |
| Repair Playbooks | absent | define approved action, scope, retry, evidence, rollback | EVO/AWR | High | Self-Repair blocked | Contract/catalog |

## Stage 0 Finding

No runtime Guardian, FSA, Application, or business-domain implementation exists in Stage 0 source. No source migration is required. The gap is documentary and architectural.
