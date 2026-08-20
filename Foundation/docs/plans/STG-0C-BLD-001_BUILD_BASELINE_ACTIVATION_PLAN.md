# STG-0C-BLD-001 — Build Baseline Activation Plan

**Identifier:** STG-0C-BLD-001  
**Version:** 1.0  
**Status:** Approved  
**Prepared:** 2026-07-27  
**Approval Date:** 2026-07-27  
**Approval Record:** GOV-055  
**Governing Authority:** GOV-054; STG-0C-PROP-001; BLD-001  
**Build Authority:** Not Granted

## 1. Purpose

This candidate defines how one exact Foundation Build Baseline may be evaluated for scoped Activation.

## 2. Baseline Content

The baseline shall bind:

- declared Build Intent;
- source and repository identity;
- admitted SDK, runtime, compiler, tools, dependencies, and sources;
- lock and dependency-resolution state;
- target platform and configuration;
- deterministic and reproducibility settings;
- analyzers, warnings, security checks, and policy versions;
- environment and Provider Profile identities;
- output inventory and content digests;
- provenance, SBOM, trace, and Evidence Requirement Set;
- and explicit exclusions.

## 3. Rules

- Prefer the .NET Base Class Library before external dependency introduction.
- Every external dependency requires documented admission, isolation through a Falcon Adapter when architectural coupling could arise, and no Layer Boundary leakage.
- No vendor lock-in or platform identity may become Falcon identity.
- The build shall fail closed on unapproved dependency resolution, source drift, missing evidence, unexpected output, or Gate weakening.
- Successful compilation is not Activation or readiness.

## 4. Current Effect

No build may run and no Build Baseline is active.
