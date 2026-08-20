# FSATS SIA — FSTSimA Digest-to-PRNG State Initialization Clarification

**Package:** `FSATS-SIA-v0.1`
**Status:** `SEMANTIC REMEDIATION / DESIGN CANDIDATE`
**Triggered By:** `AC-SIM-001`
**Controls:** Section 4 of `10A_FSTSIMA_DETERMINISTIC_RANDOMNESS_AND_NUMERICS_PROFILE.md` where interpretation differs

## 1. Purpose

Remove the final ambiguity in mapping one SHA-256 named-stream digest to the four-word initial xoshiro256** state.

## 2. Digest Words

Given:

```text
Digest = SHA256(MasterSeedBigEndian8 || 0x00 || UTF8_NFC(StreamName))
```

split the 32 digest bytes into four consecutive unsigned 64-bit **big-endian** words:

```text
d0 = Digest[0..7]
d1 = Digest[8..15]
d2 = Digest[16..23]
d3 = Digest[24..31]
```

## 3. Independent SplitMix64 Expansion

Each digest word gets an **independent** temporary SplitMix64 state.

Exact pseudocode:

```text
for i in 0..3:
    temp = d_i
    s_i = SplitMix64_Next(ref temp)
```

where `SplitMix64_Next` is exactly the algorithm in 10A Section 5:

```text
state = state + 0x9E3779B97F4A7C15  (mod 2^64)
z = state
z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9  (mod 2^64)
z = (z ^ (z >> 27)) * 0x94D049BB133111EB  (mod 2^64)
return z ^ (z >> 31)
```

There is **no chained SplitMix state across d0,d1,d2,d3**.

The post-call `temp` values are discarded. Only `s0..s3` initialize xoshiro256**.

## 4. All-Zero Protection

After independent mixing:

```text
if (s0 | s1 | s2 | s3) == 0:
    s0 = 0x9E3779B97F4A7C15
```

`s1..s3` remain zero in that exceptional case.

No second hash/reseed operation is performed.

## 5. Stream Identity

Changing any of:

- MasterSeed;
- Unicode-normalized StreamName bytes;
- RandomnessProfileId;

changes stream identity/evidence.

Changing unrelated stream names does not change this stream.

## 6. Golden Vector Generation

The independently reviewed reference implementation for golden vectors SHALL implement Sections 2-4 literally.

The reference implementation is not permitted to choose between seed-expansion interpretations because this file has already fixed the interpretation.

## 7. Negative Fixtures

Verifier SHALL reject vectors produced by:

- native-endian digest words;
- one chained SplitMix state across all four words;
- direct digest words with no SplitMix mixing;
- SplitMix sequence seeded only from d0;
- platform hash code instead of SHA-256;
- non-NFC StreamName bytes.

## 8. Finding Disposition

```text
AC-SIM-001 = REMEDIATED_AT_DESIGN_CANDIDATE_LEVEL
DIGEST_TO_XOSHIRO_STATE = EXACT
```
