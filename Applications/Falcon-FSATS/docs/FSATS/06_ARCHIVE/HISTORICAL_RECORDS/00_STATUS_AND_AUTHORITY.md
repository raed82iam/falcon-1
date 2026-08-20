# FSATS V1.4 PROPOSED — Status and Authority

**Branch:** `application-development`  
**Status:** `PART 0 ALIGNMENT PACKAGE / OWNER REVIEW REQUIRED`  
**Implementation authority:** `NOT GRANTED`  
**Deployment authority:** `NOT GRANTED`  
**Paper / Tiny Live / Live authority:** `NOT GRANTED`

## Purpose

This package defines the current-Falcon alignment of the final FSATS V1.3 Application architecture into FSATS V1.4.

V1.4 is a compatibility/alignment release, not a greenfield redesign.

```text
FSATS V1.4
= Final FSATS V1.3 Architecture
+ Current Falcon Foundation Alignment
+ Post-V1.3 Owner Clarifications
- Structures already superseded/removed in final V1.3
```

It is a design/planning package only. It does not authorize implementation, code generation, runtime activation, deployment, external connectivity, broker access, market-data access, Paper execution, Tiny Live, Live, paid services or financial activity.

## Governing boundaries

- `application-development` is the only writable Application branch for this package.
- Ordinary writes remain under `applications/**` only.
- `foundation-development` is read-only authority for this workstream.
- `reference/fsats-v1.3-scratch` remains read-only historical storage/provenance for the preserved V1.3 material.
- **Final V1.3 is the FSATS Application-architecture migration baseline.**
- Current Foundation authority governs Foundation/Application integration semantics and wins where an actual conflict exists.
- A mature V1.3 trading capability is not removed merely because it is absent from a shorter V1.4 summary.
- Foundation capabilities not available or not confirmed SHALL NOT be represented as available runtime capabilities.
- Confirmed Foundation gaps SHALL use `applications/FCR_WORKFLOW.md` and separate GitHub Issues rather than Foundation modification from this workstream.

## V1.3 treatment

Final FSATS V1.3 is treated as:

- the accepted FSATS Application-architecture migration baseline for V1.4 alignment;
- binding source for mature trading architecture, safety hardening, performance/Fast Track intent, Application boundaries and final internal supersessions unless a documented delta applies;
- historical validation/provenance evidence for V1.3 itself;
- not Foundation authority;
- not current implementation, deployment or runtime authority.

A V1.3 feature changes in V1.4 only when one of these is demonstrated:

1. current Foundation integration conflict;
2. later explicit Owner correction;
3. final V1.3 already superseded/removed it;
4. material Red-Team finding requiring a documented delta.

Silence in V1.4 is not deletion.

## Preserved topology

Inside FSATS operational boundary:

- Trading Guardian: 4 LSA rooms;
- FSAPMA: 6 LSA rooms;
- Trading Application: 12 LSA rooms.

The 4 + 6 + 12 topology is preserved from final V1.3. Part 0 verifies Foundation compatibility; it does not reopen the rooms as a new design exercise.

FSTSimA remains an independent adjacent non-Live Application with 1 MSA + 8 LSAs. Web and Communication remain independent Shared Applications.

## FCR authority

Canonical FCR identities are GitHub Issue-derived under repository Issue #1. Current submitted Part 0 FCRs are FCR-0004 through FCR-0011. Legacy manually numbered local FCR markdown files are evidence only and are not canonical identities.

An FCR is a request for Foundation disposition, not authority to modify Foundation or to implement the Application.

## Approval gate

Part 0 may reach `READY_FOR_OWNER_REVIEW` after its final Architecture Review and Red-Team rerun contain no unresolved P0/Critical design finding.

Even Owner acceptance of Part 0 does not itself authorize implementation. A separate explicit Owner implementation authorization is required before Part 1 code begins.
