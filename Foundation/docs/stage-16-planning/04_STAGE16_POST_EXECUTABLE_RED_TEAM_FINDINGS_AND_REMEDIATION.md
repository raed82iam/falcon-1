# Stage 16 Post-Executable Red Team Findings and Remediation

Date: 2026-08-17
Stage: **Stage 16 — Authoritative Identity, Authentication, Session and MFA Runtime**

## Trigger

Exact candidate `6342ea5fbff82184567e773f27003c5be85fbf5b` completed the full governed executable chain successfully:

```text
RESTORE = PASS
RELEASE_BUILD = PASS / 0 WARNINGS / 0 ERRORS
ARCHITECTURE = PASS
SECURITY = PASS / 0 FINDINGS
PREDECESSOR_REGRESSIONS_THROUGH_STAGE15 = PASS
STAGE16_VERIFIER = 42/42 PASS TWICE
DETERMINISTIC_RERUN = PASS
TRACKED_WORKTREE = CLEAN
REMOTE_CANDIDATE_STABLE = PASS
```

The Stage 16 completion gate also requires a post-executable Architecture/Consistency review and broad Red Team after executable evidence exists. That review was therefore performed before claiming technical completion or handing FCR-0152 to Web.

## Finding 1 — undefined authentication-assurance enum escalation

Severity: **HIGH**

The tested source compared `AuthenticationAssurance` values numerically against a minimum assurance policy but did not first reject undefined enum values. In C#, a caller can construct an enum value outside the declared set, for example an underlying numeric value of `999`. Such a value could compare above `High` and incorrectly satisfy a minimum-assurance check.

### Remediation

- assertion ingestion rejects undefined `AuthenticationAssurance` values with `ASSERTION_ASSURANCE_INVALID`;
- session policy rejects undefined minimum-assurance values with `SESSION_MINIMUM_ASSURANCE_INVALID`;
- identity status and authenticator state enums are also validated fail-closed at their write boundaries;
- adversarial verifier coverage now explicitly injects undefined enum values.

## Finding 2 — MFA proof had no bounded freshness window

Severity: **HIGH**

The tested implementation made MFA proof single-use for session issuance, but an unconsumed proof had no maximum permitted age. A proof could therefore remain unused for an arbitrarily long period and later satisfy a high-assurance session request.

### Remediation

`SessionPolicy` now includes an explicit `MaximumMfaAge`.

For MFA-required session issuance:

```text
MaximumMfaAge > 0
MfaReceipt.VerifiedAt <= SessionIssueRequest.IssuedAt
SessionIssueRequest.IssuedAt - MfaReceipt.VerifiedAt <= MaximumMfaAge
MFA proof is still single-use for session issuance
```

Invalid freshness policy and stale proof both fail closed. The verifier now contains an explicit stale-MFA-proof rejection case.

## Finding 3 — FCR-0152 recovery-reference obligation was not explicitly implemented

Severity: **HIGH / COMPLETENESS**

The Stage 16 entry reconciliation explicitly included `MFA enrollment/challenge/recovery reference handling` in the FCR-0152 gap. The tested candidate provided enrollment, challenge verification and revocation but no explicit recovery-reference workflow.

### Remediation

Provider-neutral recovery evidence was added without introducing secret bytes or network/provider connectivity:

- `VerifiedMfaRecovery`
- `MfaRecoveryReceipt`
- `IdentityRuntime.RecoverMfaAuthenticator(...)`

Recovery requires:

- exact Falcon identity;
- exact active predecessor authenticator reference;
- a different opaque replacement authenticator reference;
- replacement authenticator type;
- explicit verification evidence;
- bounded issue/expiry time;
- positive recovery verification;
- one-time recovery identity.

On success the predecessor authenticator is revoked atomically and the replacement opaque reference becomes active. Replayed recovery IDs, cross-identity recovery, unverified recovery, non-opaque replacement references, duplicate replacement references and recovery from a non-active predecessor fail closed.

Recovery does not restore business authority, does not infer Project Owner identity, and stores no authenticator secret material.

## Preserved boundaries

```text
AUTHENTICATION != AUTHORIZATION
MFA_PASSED != BUSINESS_AUTHORITY
MFA_RECOVERY != BUSINESS_AUTHORITY
MFA_RECOVERY != OWNER_IDENTITY
ROLE_FACT != AUTHORITY_DECISION
EXTERNAL_ASSERTION_VERIFIED != FALCON_IDENTITY_LINKED
SESSION_ISSUED != BUSINESS_AUTHORITY
SECRET_BYTES = OUTSIDE_IDENTITY_RUNTIME_STATE
LIVE_PROVIDER_CONNECTIVITY = OUTSIDE_STAGE16
WEB_SESSION_PRESENTATION != AUTHORITY_ISSUANCE
ZERO_APPLICATION_OPERATION = VALID
```

No Architecture or Security rule was weakened. No accepted Stage 15-or-earlier production behavior was modified.

## Validation consequence

Because production runtime source, Stage 16 verifier source and the Stage 16 Architecture guard changed after the successful `6342ea5...` executable run, that successful run is retained as historical evidence but cannot validate the remediated executable candidate.

A complete governed executable revalidation is mandatory again against the exact new candidate, including:

- exact remote/local candidate identity;
- SDK pin;
- structural/ownership checks;
- Release build;
- Architecture;
- Security;
- predecessor regressions through Stage 15;
- expanded Stage 16 verifier twice;
- deterministic rerun;
- clean tracked worktree;
- exact remote candidate stability.

Only after that exact candidate passes may the post-executable Architecture/Consistency and Red Team be finalized and FCR-0152 move to `FOUNDATION_IMPLEMENTED` / `Waiting On: WEB`.
