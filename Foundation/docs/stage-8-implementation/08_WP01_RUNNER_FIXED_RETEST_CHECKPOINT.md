# Stage 8 WP-01 Runner-Fixed Retest Checkpoint

Status: FROZEN_FOR_EXACT_EXECUTABLE_RETEST
Date: 2026-08-14
Branch: foundation-development

This checkpoint follows the runner-only path-quoting failure documented in `07_WP01_RUNNER_PATH_QUOTING_FAILURE_REMEDIATION_V1.md`.

No production source or verifier semantics changed after exact candidate `3c1e3fa6231ed0ff81ace5f30b4c4373c7d217d9`. The only repository change before this checkpoint is documentary evidence of the runner defect and this freeze record.

The replacement executable validation must:

1. clone the repository fresh;
2. checkout this exact checkpoint commit;
3. verify a clean worktree;
4. restore and build the controlled Foundation solution in Release;
5. run Architecture validation;
6. run Security validation;
7. run Stage 7 Cross-Stage predecessor regression;
8. run Stage 8 WP-01 verifier twice using a path-safe native invocation;
9. require `STAGE8_WP01_VERIFIER = PASS` and `CHECKS = 12/12` on both runs;
10. require identical verifier output;
11. require Guardian/verifier/Architecture/Security binary hash stability;
12. require final exact HEAD and clean worktree.

No Owner closure is requested at this checkpoint. On PASS, Stage 8 continues automatically to WP-02 under the existing Owner implementation authorization.

`WP01_RETEST_STATE = READY`
`PRODUCTION_CODE_DRIFT_SINCE_PRIOR_CANDIDATE = NONE`
`VERIFIER_SEMANTIC_DRIFT_SINCE_PRIOR_CANDIDATE = NONE`
`NEXT_ON_PASS = WP02_AUTOMATIC_CONTINUITY`
