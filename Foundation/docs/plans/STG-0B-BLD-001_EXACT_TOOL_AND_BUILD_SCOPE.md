# STG-0B-BLD-001 — Exact Tool and Build Scope

**Identifier:** STG-0B-BLD-001  
**Version:** 1.0  
**Status:** Approved  
**Proposal Date:** 2026-07-26  
**Approval Date:** 2026-07-26  
**Governing Authority:** BLD-001 v1.1; ADR-I002; STG-0B-SCP-001  
**Approval Record:** GOV-051  
**Build Authority:** Granted for Stage 0B candidate verification only

## 1. Purpose

This candidate limits the toolchain and build operations eligible for Stage 0B.

## 2. Candidate Tool Baseline

| Tool Class | Observed Candidate | Permitted Purpose |
|---|---|---|
| .NET SDK | 10.0.302 | Candidate compile, test, and SDK-contained analysis |
| Git | 2.55.0.windows.3 | Repository identity, status, diff, and evidence |
| Windows PowerShell | 5.1.26100.8875 | Bounded orchestration and evidence capture |
| .NET BCL | SDK-associated | Preferred implementation dependency |

Observed presence is not admission. Exact executable paths, publisher evidence where available, versions, and SHA-256 digests shall be recorded before use.

## 3. Acquisition Policy

No tool, SDK, workload, package, extension, container image, or external dependency may be installed or downloaded under this candidate.

If an approved Stage 0B case cannot proceed using the admitted local baseline, it shall stop and request a separate acquisition decision.

## 4. Dependency Policy

- Prefer the .NET Base Class Library before introducing any external dependency.
- External packages are excluded from the initial Stage 0B candidate scope.
- Platform access shall occur behind Falcon Contracts or Adapters.
- No dependency may cross a layer boundary.
- No vendor lock-in is permitted.
- Dependency versions and provenance shall be deterministic and recorded.

## 5. Build Intent

The only permitted Build Intent would be:

```text
STAGE_0B_CANDIDATE_VERIFICATION
```

It shall not be represented as Developer Build, Release Candidate, Release, Hotfix, Production, or Emergency Recovery.

## 6. Permitted Build Operations If Approved

- compile an enumerated candidate;
- compile isolated verification fixtures;
- execute candidate-only unit and Contract checks;
- execute applicable VPL-BST-003 through VPL-BST-005 cases;
- produce non-release candidate artifacts;
- create SBOM or provenance evidence using admitted capabilities;
- and calculate integrity digests.

## 7. Prohibited Build Outcomes

Stage 0B shall not produce:

- a Falcon executable runtime;
- a release candidate;
- a production package;
- a deployable cloud image;
- a signed production artifact;
- an operational Provider;
- or an Activation-ready claim without independent Stage 0C review.

## 8. Reproducibility

Each build case shall preserve exact inputs, tool identities, configuration, environment, dependency resolution, outputs, and digests.

An undeclared restore, workload acquisition, network access, generated dependency, or version drift shall invalidate the case.
