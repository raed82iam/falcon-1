# FSATS Application FCR Binding Formal Red Team Review

Date: 2026-08-18
Scope: FCR-0008, 0009, 0010, 0011, 0012, 0013, 0014, 0030, 0031
Excluded hold: FCR-0082

## Final source-review result

```text
CRITICAL = 0
HIGH = 0
MEDIUM = 0
OPEN_LOW_PRODUCT_RUNTIME = 0
SOURCE_RED_TEAM = PASS
EXECUTABLE_VERIFICATION = PENDING
```

## Adversarial questions exercised

The review challenged the implementation for stale/wrong Foundation candidate identity, wrong artifact/version/compatibility/source contract, stale/future resource observations, missing evidence, malformed digest, resource-authority smuggling, load-shedding execution inference, wrong FSA destination, changed candidate/evidence integrity, Owner-silence approval, business-judgment leakage into FSA, authority-expansion smuggling, production-adoption/runtime inference, research-purpose confusion, operational-provider/research/broker purpose collapse, FSA direct Internet, non-Live use of Live routes/credentials, direct connection execution, broker order/Live authority smuggling, QoS business/Fast-Track/deployment authority, and cross-Application Awareness drift.

## Findings found and fixed during Red Team

### RT-01 — Stage 11 exact candidate not originally pinned
Severity before fix: HIGH.

The first QoS binding draft preserved authority separation but did not require the exact accepted Stage 11 executable candidate. It was hardened to require:

`165ce895ea059510e9b1a1a29c8d15254a18c283`

Wrong/stale candidate identity now fails closed for Trading, TradingGuardian and FSAPMA QoS bindings.

### RT-02 — FSAPMA operational-provider purpose initially too permissive
Severity before fix: HIGH.

The provider binding originally accepted any non-empty purpose. It now requires exact `OPERATIONAL_PROVIDER_DATA` and exact Application identity `FSAPMA`; research/broker purpose substitution fails closed.

### RT-03 — Trading broker purpose initially too permissive
Severity before fix: HIGH.

The broker binding now requires exact `BROKER_EXECUTION`; provider/research purpose substitution fails closed.

### RT-04 — Cross-Awareness verification initially exercised only Trading
Severity before fix: MEDIUM.

The dedicated verifier was expanded to reference all five Awareness projects and exercise FSA peer bindings plus research egress bindings across Trading, FSAPMA, TradingGuardian, FSTSimA and APP-RSC.

## Boundaries deliberately not crossed

- No network connection is opened.
- No provider or broker credential secret is stored or consumed; only governed credential references are represented.
- No route binding claims connection execution.
- No Paper/Shadow/TinyLive/Live activation is granted.
- No FSA acceptance, Owner adoption, release, deployment or business authority is granted.
- No Foundation source is modified.
- No Shared Web source is modified.
- FCR-0082 remains on explicit HOLD.

## Residual gate

The remaining gate is executable, not semantic: exact final Application HEAD must pass restore, Release build, `dotnet test`, Architecture, Security, the new Foundation Binding verifier, all existing governed verifiers, exact HEAD confirmation and clean tracked status. A source Red Team PASS is not substituted for executable PASS.
