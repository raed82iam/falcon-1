# Stage 0B Cleanup and Residue Assessment

**Evidence ID:** STG-0B-CLEAN-001  
**Recorded Date:** 2026-07-26  
**Authority:** GOV-051  
**Status:** Satisfied

## Removed Repository-Local Material

- candidate-library `bin/`;
- candidate-library `obj/`;
- verifier `bin/`;
- verifier `obj/`;
- isolated `.stage0b/` CLI home;
- isolated application-data state;
- isolated package cache;
- repeat-run temporary evidence;
- and ephemeral in-memory keys, secrets, certificates, and randomness.

## Preserved Material

- governed candidate source;
- verification source;
- approved Stage 0B documents;
- original verification result JSON;
- stop and remediation evidence;
- tool, artifact, and source identities;
- security and isolation findings;
- and completion assessment.

## Residue Checks

- No candidate process remains running.
- The .NET MSBuild and compiler servers started by the candidate build were shut down successfully.
- No listener, service, scheduled task, cloud resource, or financial connection was created by Stage 0B.
- No candidate binary remains in the repository workspace.
- No local package cache remains.
- No candidate or test secret remains in the repository.
- The repository working state was clean before creation of the final documentary evidence.

## External Sandbox Residue

The previously reported ASP.NET development certificate is outside Falcon’s repository and custody in the Codex sandbox profile. It is explicitly untrusted and unused. Broad deletion was not performed because it could affect unrelated development certificates.

## Finding

```text
CLEANUP_COMPLETE_WITH_DOCUMENTED_EXTERNAL_SANDBOX_RESIDUE
```

The documented external residue cannot be relied upon by Falcon and does not block the scoped Stage 0B candidate finding.
