# FSATS Part 6 — Exact Executable Candidate Freeze

**Status:** `FROZEN_FOR_OWNER_OPERATED_EXECUTABLE_VALIDATION`  
**Exact executable candidate:** `697d48b6a3e2532747e68bcf5439d808a1e1f29f`  
**Branch:** `application-development`

## Freeze Decision

The exact source/test commit for Part 6 executable validation is:

```text
697d48b6a3e2532747e68bcf5439d808a1e1f29f
```

Later branch commits are documentary review/state records only unless a semantic remediation explicitly supersedes this freeze.

## Candidate Application Source

```text
applications/FSATS/src/Trading/Falcon.FSATS.Trading.Application/OperationalConfiguration.cs
applications/FSATS/src/FSAPMA/Falcon.FSATS.FSAPMA.Application/OperationalConfiguration.cs
applications/FSATS/src/TradingGuardian/Falcon.FSATS.TradingGuardian.Application/OperationalConfiguration.cs
applications/FSATS/src/ResourceManagement/Falcon.FSATS.ResourceManagement.Application/OperationalConfiguration.cs
applications/FSATS/src/FSTSimA/Falcon.FSATS.FSTSimA.Application/OperationalConfiguration.cs
```

## Candidate Contract

```text
applications/FSATS/contracts/configuration/FSATS.ApplicationConfigurationProjection.v1.md
```

## Candidate Verification Source

```text
applications/FSATS/tests/Behavior/Falcon.FSATS.Behavior.Verifier/Part6ConfigurationAdversarialChecks.cs
applications/FSATS/tests/Behavior/Falcon.FSATS.Behavior.Verifier/Part6VerifierBootstrap.cs
```

## Required Validation

```text
EXACT DETACHED HEAD = 697d48b6a3e2532747e68bcf5439d808a1e1f29f
INITIAL TREE = CLEAN
RESTORE = PASS
RELEASE BUILD = PASS
DIRECT BEHAVIOR / PART 6 ADVERSARIAL = PASS
DIRECT FAILURE = PASS
GOVERNED APPLICATION VERIFIERS RUN 1 = PASS
GOVERNED APPLICATION VERIFIERS RUN 2 = PASS
FINAL HEAD = EXACT
FINAL TREE = CLEAN
```

A failure becomes Part 6 evidence requiring diagnosis/remediation and, if semantics change, a new exact candidate and fresh reviews. It does not reopen closed Parts 0 through 5.

No validation result grants Part 6 Owner closure, Part 7, runtime, external connectivity, Paper/Live, or deployment authority.
