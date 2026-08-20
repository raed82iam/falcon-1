# Stage 16 Permanent Public Type Identity Remediation

## Status

`SOURCE_REMEDIATION_COMPLETE / FULL_GOVERNED_EXECUTABLE_REVALIDATION_REQUIRED`

Candidate `903196330f710f611471e99a2f75620a756c96ad` passed exact candidate checks, .NET SDK 10.0.302, Stage 16 structural checks, restore and Release build with zero warnings/errors, then failed the Foundation Architecture gate before Security or predecessor regressions.

The accepted Architecture validator prohibits `Falcon`, `Stage`, or `WP` tokens in permanent public production type identities.

The Stage 16 public types were remediated as follows:

```text
FalconIdentityStatus  -> IdentityStatus
FalconIdentityProfile -> IdentityProfile
FalconSession         -> IdentitySession
```

`FalconIdentityId` semantics remain unchanged. The verifier was rebound and the dedicated Architecture guard requires the permanent names and rejects the old names. No Architecture rule was weakened. No Stage 15 or earlier production behavior was modified.

Mandatory boundaries remain:

```text
AUTHENTICATION != AUTHORIZATION
OIDC_AUTHENTICATED != FALCON_IDENTITY
MFA_PASSED != BUSINESS_AUTHORITY
SESSION_ISSUED != BUSINESS_AUTHORITY
LIVE_PROVIDER_CONNECTIVITY = OUTSIDE_STAGE16
PRODUCTION_AUTHENTICATION_ACTIVATION = NOT_GRANTED
```

The failed candidate remains historical evidence. Full governed executable revalidation of the final stable Foundation branch candidate is required before any Stage 16 implementation-complete claim. Only a complete PASS may proceed to post-executable Architecture/Consistency review, broad Red Team, FCR handoff and closure readiness.