# FSATS SIA — FSTSimA Deterministic Randomness and Numerical Reproducibility Profile v1.0

**Package:** `FSATS-SIA-v0.1`
**Status:** `SEMANTIC REMEDIATION / DESIGN CANDIDATE`
**Triggered By:** `RT-SIM-001`
**Owner:** APP-SIM / S-LSA-01 with S-LSA-02/S-LSA-04/S-LSA-06 consumers

## 1. Purpose

Make a `MasterSeed` and `RunDefinition` produce one exact initial random stream and one reproducible stochastic scenario procedure rather than leaving RNG/distribution choices to implementation libraries.

This profile governs initial FSTSimA stochastic runs. A later RNG/numerical algorithm requires a new profile/version and changes RunDefinition identity.

## 2. Profile Identity

```text
RandomnessProfileId = FSTSIMA-RNG-v1.0
NumericProfileId = FSTSIMA-NUMERIC-v1.0
PRNG = xoshiro256**
SeedExpander = SplitMix64
StreamDerivationHash = SHA-256
FloatingRuntime = IEEE-754 binary64 / .NET 10 System.Double
StochasticOutputQuantization = decimal 1e-10 half-to-even unless a MarketProfile tick/quantity step is coarser
```

Financial/accounting outputs are still converted to the exact decimal/tick/quantity semantics from the canonical domain profile before they can affect simulated order/account ledgers.

## 3. Master Seed Encoding

`MasterSeed` is an unsigned 64-bit integer serialized as exactly 8 bytes unsigned big-endian.

No string parsing, locale or platform-native endian form is part of canonical seed identity.

## 4. Named Stream Seed Derivation

For exact UTF-8 NFC-normalized `StreamName`:

```text
SeedMaterial = MasterSeedBigEndian8
               || byte(0x00)
               || UTF8_NFC(StreamName)

Digest = SHA256(SeedMaterial)   // 32 bytes
```

Interpret Digest as four unsigned 64-bit big-endian words:

```text
d0,d1,d2,d3
```

Each word is mixed through SplitMix64 in canonical order to initialize the four xoshiro state words. If the resulting four-word state is all zero, replace state word 0 with:

```text
0x9E3779B97F4A7C15
```

No unrelated stream creation changes another named stream sequence.

Canonical names are declared by component, e.g.:

```text
market.regime
market.return
market.volume
market.spread
broker.fill
broker.latency
provider.latency
provider.failure
fault.schedule
```

A component may create deterministic child names by appending `.` plus canonical subject identity; names are evidence-bound.

## 5. SplitMix64

Exact unsigned-64 algorithm with wraparound modulo 2^64:

```text
state += 0x9E3779B97F4A7C15
z = state
z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9
z = (z ^ (z >> 27)) * 0x94D049BB133111EB
return z ^ (z >> 31)
```

All shifts are logical unsigned shifts.

## 6. xoshiro256** NextUInt64

State words: `s0,s1,s2,s3` unsigned 64-bit.

```text
result = rotl(s1 * 5, 7) * 9
t = s1 << 17

s2 ^= s0
s3 ^= s1
s1 ^= s2
s0 ^= s3

s2 ^= t
s3 = rotl(s3, 45)

return result
```

`rotl(x,k) = (x << k) | (x >> (64-k))` with unsigned wraparound.

Every call consumes exactly one state advance.

## 7. Uniform U(0,1)

One `NextUInt64` is converted to a binary64 value strictly inside `(0,1)`:

```text
mantissa53 = NextUInt64() >> 11
U = (mantissa53 + 0.5) / 9007199254740992.0   // 2^53
```

This avoids exact 0 and 1 for logarithm/distribution transforms.

## 8. Standard Normal

Every requested standard-normal sample consumes **exactly two** independent uniform calls from the same named stream and returns the first Box-Muller variate:

```text
u1 = U()
u2 = U()
z = sqrt(-2 * ln(u1)) * cos(2 * PI * u2)
```

The sine partner is deliberately discarded and SHALL NOT be cached, so call-count behavior is fixed.

`PI` uses `System.Math.PI` under NumericProfile v1.0.

## 9. Student-t Sampling For Initial Regime Profile

The initial degrees of freedom in 17A are positive integers: 3,5,6,10,12.

To draw Student-t with integer `nu`:

```text
z0 = StandardNormal()
V = 0
repeat exactly nu times:
    zi = StandardNormal()
    V += zi * zi
T = z0 / sqrt(V / nu)
```

One t sample therefore consumes exactly `2*(nu+1)` uniform draws from its named stream.

No Gamma/ChiSquare library call is allowed for v1.0.

## 10. State-Conditioned Return

For a scenario regime with exact calibrated/profile values `MeanReturn`, `Scale` and integer `nu`:

```text
RawReturn = MeanReturn + Scale * StudentT(nu)
QuantizedReturn = round(RawReturn, 10 decimal places, MidpointRounding.ToEven)
```

Market model then converts the return to the scenario price path and quantizes the resulting canonical price to the MarketProfile tick using the declared simulation price-rounding rule.

For risk-increasing synthetic price evolution, tick rounding is to nearest valid tick with midpoint-to-even only for simulation truth generation; simulated broker execution quantity/risk rounding follows the actual Trading/Broker profile rules.

## 11. Other Random Distributions

No component may call a platform generic `Random`/third-party distribution without a declared algorithm profile.

Initial permitted transforms:

### Bernoulli(p)

```text
U() < p
```

with `p` exact decimal converted once to binary64 under NumericProfile v1.0 and recorded in scenario config.

### Uniform range [a,b)

```text
a + (b-a) * U()
```

then output quantized by owning model profile.

### Integer categorical

Given exact decimal probabilities summing to 1.0 in canonical state order:

- construct cumulative thresholds in that fixed order;
- draw U;
- select first cumulative threshold strictly greater than U;
- final category absorbs only decimal representation remainder after exact profile validation.

No alias-method/library implementation in v1.0.

## 12. Markov Regime Transition Draw

Use the transition matrix order exactly as listed in 17A:

```text
RANGE_LOW_VOL
TREND_NORMAL
HIGH_VOL
LIQUIDITY_STRESS
CRISIS_DISLOCATION
```

At each transition decision:

1. take current-state row;
2. validate exact decimal probabilities sum to 1.00;
3. draw one U from `market.regime` stream;
4. scan cumulative probability in the canonical state order;
5. choose first threshold > U.

One transition consumes exactly one random value.

## 13. Canonical Simulation Event Priority

For events with identical `SimulationEffectiveTime`, scheduler uses numeric priority:

```text
0   AUTHORITY_SECURITY_LIFECYCLE_FIXTURE
10  FAULT_INJECTION_START_END
20  MARKET_SESSION_HALT_CORPORATE_ACTION
30  MARKET_DATA_BOOK_QUOTE_TRADE_BAR
40  PROVIDER_SERVICE_RESPONSE_OR_FAILURE
50  BROKER_EXCHANGE_ORDER_EVENT
60  ACCOUNT_CAPITAL_SETTLEMENT_EVENT
70  APPLICATION_DECISION_SCHEDULED_EVENT
80  GUARDIAN_RESOURCE_COORDINATION_FIXTURE
90  ANALYTICS_OBSERVATION
100 EVIDENCE_CHECKPOINT_INTERNAL
```

Then `SourceSequence`, then canonical `EventId` ordinal identity as defined in file 10.

Priority is simulation ordering only, not business authority.

## 14. Numeric Comparison / Quantization

Stochastic model intermediate binary64 calculations are permitted inside FSTSimA only under this profile.

Before a stochastic model output becomes a governed scenario event field used by another component:

- dimensionless return/probability/model score: round to 10 decimal places half-to-even;
- price: canonical MarketProfile tick;
- quantity: canonical quantity step;
- time duration/latency: integer microseconds unless a coarser contract unit is declared;
- money/accounting: canonical decimal Money rules.

Golden evidence compares canonicalized outputs, not non-canonical CPU register intermediates.

## 15. Runtime / Library Binding

Initial executable verifier shall pin:

```text
TargetFramework = net10.0
LanguageVersion = 14.0
RandomnessProfileId = FSTSIMA-RNG-v1.0
NumericProfileId = FSTSIMA-NUMERIC-v1.0
```

Use of hardware/vectorized or compiler options that materially alter canonical numeric output is forbidden unless golden vectors still pass exactly after canonical quantization.

## 16. Checkpoint State

Every FSTSimA checkpoint stores, per named random stream:

```text
StreamName
RandomnessProfileId
s0,s1,s2,s3
DrawCountUInt64
```

Resume verifies profile and exact state. Missing/mismatched stream state invalidates checkpoint reproducibility.

Adding a new unrelated stream to a successor scenario does not change the stored state of existing stream names.

## 17. Golden Vector Requirements

The executable verifier SHALL include immutable vectors for:

1. SplitMix64 known seed sequence;
2. xoshiro256** first N UInt64 outputs from known state;
3. named-stream SHA-256 derivation from fixed master seed/name;
4. first N U(0,1) outputs to exact binary64 bit pattern;
5. first N Box-Muller outputs after canonical 1e-10 quantization;
6. Student-t outputs for nu 3,5,6,10,12 from fixed seeds;
7. Markov regime transition sequence from fixed seed/matrix;
8. scheduler equal-time ordering;
9. checkpoint/resume continuation identical to uninterrupted run;
10. unrelated stream insertion does not change existing stream results.

Golden expected values are generated once by an independently reviewed reference implementation and stored with exact profile digest before implementation closure.

## 18. Forbidden Behaviors

Verifier/review shall reject:

- `System.Random` default RNG in evidence-bearing stochastic run;
- cryptographic RNG for model draws unless a future profile explicitly defines it;
- third-party Student-t/Gamma distribution replacing v1 procedure;
- cached Box-Muller sine partner;
- native-endian seed serialization;
- locale-sensitive stream name/number conversion;
- platform hash code for stream seeding;
- stochastic result without profile identity;
- checkpoint that omits RNG state/draw count;
- wall-clock time or process scheduling influencing random draw order.

## 19. Finding Disposition

```text
RT-SIM-001 = REMEDIATED_AT_DESIGN_CANDIDATE_LEVEL
PRNG = EXACT
STREAM_DERIVATION = EXACT
DISTRIBUTION_DRAW_COUNTS = EXACT
EVENT_PRIORITY = EXACT
NUMERIC_CANONICALIZATION = EXACT
```
