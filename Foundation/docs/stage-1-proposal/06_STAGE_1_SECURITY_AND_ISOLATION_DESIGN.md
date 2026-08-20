# 06 - Stage 1 Security and Isolation Design

| Boundary | Allowed | Prohibited | Enforcement design | Verification scenario | Stop condition |
|---|---|---|---|---|---|
| Filesystem | repo-root-only reads and writes inside the controlled boundary | writes outside the repository; writes into production/cloud/financial paths | path allowlist and canonical boundary review | VS-02, VS-09, VS-17 | any path escapes the repository boundary |
| Process | one controlled local build or verification process at a time | hidden background services; uncontrolled child processes | explicit command invocation and process inventory | VS-04, VS-11, VS-12 | process provenance cannot be attributed |
| Network | no external network for Stage 1 execution preparation unless explicitly admitted by a canonical source, and none for the empty-build proof path | internet dependency, package download during proof, external connectivity | offline restore and isolated verification design | VS-04, VS-06, VS-17 | any required network access appears |
| Package acquisition | deterministic, pinned, provenance-checked acquisition only | ad hoc restore from uncontrolled sources | lock files and provenance records | VS-04, VS-05 | unpinned or unprovenanced dependency appears |
| Credentials and secrets | none in Stage 1 core boundary; only explicit test placeholders when approved by canonical source | real credentials, stored secrets, secret leakage | secret exclusion rules and scans | VS-09, VS-10, VS-17 | any secret is present or suspected |
| Test data | synthetic, minimal, non-financial, non-production data only | live capital data, customer data, production data | synthetic-data rule and directory segregation | VS-09, VS-17 | non-synthetic data appears |
| Financial endpoints | none | broker, venue, market, bank, wallet, or capital endpoint | endpoint exclusion proof | VS-17, VS-18 | any financial endpoint or path exists |
| Environment identity | exact local environment identity, versioned and documented | implicit host identity or ambiguous runtime identity | ENV-001 identity controls | VS-03, VS-04, VS-18 | environment identity is not exact |
| Evidence storage | immutable evidence outputs under `artifacts/evidence/stage-1/` | mutable evidence stores; self-overwriting evidence | immutable output naming and retention | VS-15, VS-16 | evidence cannot be reconstructed |
| Generated files | only generated artifacts explicitly listed in the proposal | generated source outside approved outputs | generated-output allowlist | VS-14, VS-15 | unexpected generated artifact appears |
| Temporary files | isolated temporary outputs under `artifacts/tmp/stage-1/` | temp files in source or uncontrolled directories | temporary-path segregation and cleanup | VS-14, VS-16 | temp files escape their sandbox |
| Developer workstation | a local workstation may host the review and controlled build inputs only | workstation as hidden authority, runner, or production surrogate | workstation classification and scope checks | VS-01, VS-18 | workstation becomes a governed runtime |
| Active Foundation environment | only the exact active Foundation build-verification environment approved by ENV-001 | unapproved environment mutation | environment admission and identity checks | VS-03, VS-04, VS-11 | the environment is not exact |

## Security rule

The design is fail-closed. If any boundary cannot be proven, Stage 1 execution
must not proceed.

