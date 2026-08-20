# FSATS Remediation — Verifier Continuation and R4 Revalidation

Date: 2026-08-17
Target branch: `application-development`
Prior repaired candidate: `281f63773849477139235269d3ac4fc6575b04ce`
Current verifier-repair candidate: `734896fe953f8c5f4bbcca39f5604bcce877fc2c`
Required SDK: `10.0.302`

R2 established exact candidate identity, clean Application ownership boundary, SDK pin, Foundation restore/build PASS, Application restore/build PASS, and Application test-command PASS.

R3 enabled process-scoped PowerShell execution and reached the governed verifier runner. Results:

```text
ARCHITECTURE = PASS
SECURITY = PASS
BEHAVIOR = FAIL
OPERATIONAL_DATA_OUTCOME = PASS
INTEGRATION = PASS
FAILURE = PASS
```

The Behavior failure was `FCR0226_LOCAL_FOUNDATION_CONTROL_PLANE_SUBSTITUTION_PROHIBITED`.

Root cause: the missing-Stage13 substitution check scanned all `applications/**/*.cs` files while the verifier itself contains the lexical probes `namespace Foundation.Authority` and `AiTargetRegistration`, so the verifier matched its own test source. Repair commit `734896fe953f8c5f4bbcca39f5604bcce877fc2c` narrows the scan to production Application source under `applications/FSATS/src/**` and preserves the intended fail-closed prohibition.

R4 must fetch/check out exact candidate `734896fe953f8c5f4bbcca39f5604bcce877fc2c`, keep process-scoped execution-policy bypass, verify SDK `10.0.302`, rebuild `applications/Falcon.Applications.slnx` Release with `--no-restore`, run `applications/ci/Run-Application-Verifiers.ps1` twice, and verify final exact HEAD plus clean tracked tree.

Expected evidence: `C:\FAV\Validation-Evidence-R4.zip`.

A PASS is technical evidence only. Runtime, provider/broker connectivity, Paper/Shadow/Tiny-Live/Live, deployment, and AI release/revival authority remain NOT GRANTED.
