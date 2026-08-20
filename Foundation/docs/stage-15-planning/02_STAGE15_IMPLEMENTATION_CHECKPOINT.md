# Stage 15 Implementation Checkpoint

**Stage:** 15 — Application Runtime Hosting, Admission, Activation and Capability Isolation  
**State:** SOURCE IMPLEMENTED / PREDECESSOR COMPATIBILITY REMEDIATED / GOVERNED EXECUTABLE REVALIDATION PENDING  
**Initial pre-checkpoint source HEAD:** `8bd5f4f0a71ff9486d0ae249a8f4cb34f1f0d019`

## Final implementation placement

```text
Production component: Foundation.ApplicationRuntimeHosting
Stage 15 source: src/Foundation.ApplicationRuntimeHosting/Stage15ApplicationRuntimeHost.cs
Verifier: verification/Falcon.Stage15.ApplicationRuntimeHosting.Verifier/
Permanent production project: src/Foundation.ApplicationRuntimeHosting/Foundation.ApplicationRuntimeHosting.csproj
Production ProjectReferences: ZERO
Foundation.ApplicationLifecycle public/compilation surface: RESTORED_TO_PRE_STAGE15_BASELINE
```

The initial implementation compiled Stage 15 runtime activation types into `Foundation.ApplicationLifecycle`. Full governed predecessor regression proved that placement incompatible with the accepted Stage 5 WP-09 public-surface invariant. Stage 10 VPL-008 exposed the incompatibility through VPL-004.

The runtime host source was moved byte-for-byte into the independent `Foundation.ApplicationRuntimeHosting` production boundary. No Stage 15 runtime behavior was changed by the move. Architecture Guard now explicitly requires and validates that project, including zero production ProjectReferences.

## Executable-validation history

### Attempt 1

Candidate `98e23ab83580ae4d8f393a9fe137e43bdb2833ab` stopped at Release build because analyzer CA2014 rejected a four-byte `stackalloc` inside the canonical-hash loop.

Disposition: fixed without changing hash semantics by allocating the four-byte prefix once outside the loop.

### Attempt 2

Candidate `e0f1206fb30e5081a8c067085fe37a22c061f152` passed:

```text
RESTORE = PASS
RELEASE_BUILD = PASS
ARCHITECTURE = PASS
SECURITY = PASS / 0 FINDINGS
STAGE6 = PASS / 26/26
STAGE7 = PASS / 10/10
STAGE8 = PASS / 35/35
STAGE9 = PASS / 38/38
```

It then stopped at Stage 10 VPL-008 `36/38`, with failures `VPL-004-exit-pass` and `VPL-004-pass-marker`.

Root cause: VPL-004 runs the accepted Stage 5 integration chain. Stage 5 WP-09 reflects over the `Foundation.ApplicationLifecycle` exported surface and correctly rejects activation/deployment API expansion. The Stage 15 placement, not the predecessor verifier, was wrong.

Disposition: preserve Stage 5 and Stage 10 unchanged; isolate Stage 15 public runtime-host surface in `Foundation.ApplicationRuntimeHosting`.

## Implemented work packages

```text
WP-01 Runtime Host Identity and Zero-Application State = SOURCE PRESENT
WP-02 Exact Prerequisite Binding = SOURCE PRESENT
WP-03 Runtime Registration = SOURCE PRESENT
WP-04 Separate Activation Authority = SOURCE PRESENT
WP-05 Capability Isolation = SOURCE PRESENT
WP-06 Suspension and Isolation = SOURCE PRESENT
WP-07 Removal and Return to Zero = SOURCE PRESENT
WP-08 Failure Containment and Coexistence = VERIFIER PRESENT
WP-09 Integrated Hardening and Closure Verification = PENDING FULL REVALIDATION
```

## Implemented boundaries

```text
ZERO_APPLICATION_OPERATION = VALID
APPLICATION_PRESENCE != FOUNDATION_PREREQUISITE
ADMISSION != ACTIVATION
ARTIFACT_CONSUMPTION != ACTIVATION
RESOURCE_GRANT != ACTIVATION
REGISTERED != ACTIVE
ACTIVATION != BUSINESS_AUTHORITY
APPLICATION_FAILURE != FOUNDATION_FAILURE
APPLICATION_PRIVATE_CAPABILITY != CROSS_APPLICATION_ACCESS
STAGE15 != ENVIRONMENT_REALIZATION
STAGE15_RUNTIME_HOST != STAGE5_APPLICATION_LIFECYCLE_PUBLIC_SURFACE
```

## Predecessor reuse

The Stage 15 verifier binds real accepted predecessor outputs into Stage 15 runtime evidence:

- Stage 14 `ArtifactConsumptionDecision` -> `RuntimeArtifactConsumptionBinding`;
- Admission `AdmissionDecision` -> `RuntimeAdmissionBinding`;
- Application Lifecycle `LifecycleDecision` -> `RuntimeLifecycleEligibilityBinding`;
- Stage 6 `ApplicationResourceAllocation` -> `RuntimeResourceGrantBinding`.

No predecessor engine is duplicated inside Stage 15. The production runtime-host project itself has zero ProjectReferences.

## Adversarial coverage present in verifier

- rejected or authority-carrying artifact-consumption result;
- wrong exact artifact identity;
- rejected/mismatched Admission evidence;
- wrong/ineligible lifecycle attach evidence;
- missing, wrong-Application, expired, future-evidence or invalid-limit resource grants;
- duplicate runtime instance and duplicate current Application alias;
- duplicate/exclusive capability conflict;
- private cross-Application capability access;
- undeclared shared capability access;
- revoked, ambiguous, stale, wrong-action, wrong-runtime, wrong-Application, wrong-version, expired or future activation authority;
- double activation and double isolation;
- isolated/suspended provider availability;
- wrong lifecycle removal evidence;
- two-Application coexistence and one-Application isolation containment;
- complete removal back to zero Applications;
- Stage 16 process/container/OS/deployment leakage;
- OIDC/MFA and trading/business semantic leakage.

## Out-of-scope current Foundation FCRs

FCR-0076 and FCR-0152 remain separate `Waiting On: FOUNDATION` obligations with `UNASSIGNED / REQUIRES_GOVERNED_PLANNING` targets. Stage 15 does not absorb them.

## Validation required before any PASS claim

1. isolated exact candidate checkout;
2. .NET SDK 10.0.302;
3. structural proof of independent RuntimeHosting assembly and restored ApplicationLifecycle assembly;
4. restore;
5. Release build of governed solution;
6. Architecture;
7. Security;
8. explicit Stage 5 WP-09 and WP-10 regressions;
9. predecessor regressions Stage 6 through Stage 14;
10. Stage 15 verifier run 1;
11. Stage 15 verifier run 2;
12. deterministic rerun;
13. mandatory marker verification;
14. exact local/remote candidate equality;
15. clean tracked worktree.

No executable PASS, Stage 15 acceptance, deployment authority, production activation or Stage 16 authority is claimed by this checkpoint.
