# Baseline Integrity Verification and Red-Team Plan

## Required clean gates

1. exact SDK identity check;
2. clean restore using repository `NuGet.Config`;
3. clean Release build;
4. zero warnings;
5. zero errors;
6. Architecture Tests;
7. hardened Security Tests;
8. Stage 0C remediation verifier;
9. Stage 2 WP-01 through WP-04 verifiers;
10. Stage 3 WP-01 through WP-05 verifiers;
11. Baseline Integrity verifier run 1;
12. Baseline Integrity verifier run 2 from the same unchanged DLL;
13. byte-identical complete output;
14. repository status and exact changed-path check;
15. `git fsck`.

## Documentary proofs

- all 27 canonical targets present;
- all 27 expose intended versions and active lifecycle metadata;
- zero proposal/pending markers on those active canonical targets except quoted historical discussion;
- zero mojibake tokens on active canonical surfaces;
- SPEC and TREE IDs equal;
- one canonical path per ID;
- all active registry owners and authority fields resolved;
- no archive/candidate/historical byte modified.

## Runtime negative proofs

- null top-level and nested requests fail closed;
- crafted delimiter identities do not collide;
- rejected AdmissionId cannot be reused;
- rejected RegistrationId cannot be reused;
- concurrent duplicate operations produce one accepted state change at most;
- all snapshots are deterministic and ordered;
- future time evidence rejected;
- negative/overflow uncertainty rejected;
- concurrent identifier continuity returns one stable result;
- stale key and secret references cannot rotate/revoke current material;
- active key material cannot be zeroed during use;
- returned byte arrays cannot mutate internal state;
- certificate subject substring attacks rejected;
- non-hex digests rejected;
- undeclared/duplicate evidence rejected;
- duplicate provider profiles rejected;
- security test fails when launched from an unrelated working directory;
- unreadable governed files produce a security finding;
- zero-file scan cannot pass.

## WP-06 seam regression proofs

- WP-04 consumes actual WP-02 admission output;
- one canonical graph identity is used;
- WP-05 consumes exact WP-04 decision vocabulary and event reference;
- plug-in registration state remains `NOT_APPLICABLE`;
- no downstream gate runs after an upstream rejection.

## Independent review

The reviewer binds directly to clean Release assemblies and independently attacks:

- composite-key collision;
- first-observation identity replay;
- null/malformed public input;
- concurrency races;
- stale cryptographic references;
- mutable byte aliasing;
- future time;
- evidence-set injection;
- security-scanner wrong-root and unreadable-file bypass;
- WP-02/WP-04/WP-05 synthetic evidence substitution.

Any reproduced finding or new blocking finding stops closure.
