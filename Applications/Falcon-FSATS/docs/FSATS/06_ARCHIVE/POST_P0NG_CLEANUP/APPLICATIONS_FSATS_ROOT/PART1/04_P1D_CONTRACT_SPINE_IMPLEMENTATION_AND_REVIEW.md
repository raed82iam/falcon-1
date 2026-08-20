# FSATS V1.4 Part 1 - P1-D Contract Spine Implementation and Review

**Work package:** `P1-D`
**Scope:** Cross-Application contract family declarations for the three core FSATS Applications
**State:** `IMPLEMENTATION_COMPLETE / SOURCE_REVIEW_PASS / EXECUTION_VALIDATION_PENDING_P1-F`
**Application branch:** `application-development`

## 1. Scope completed

P1-D implements declaration-only contract-family metadata for the currently authorized three core FSATS Applications:

- Trading Guardian;
- FSAPMA;
- Trading Application.

The spine records:

- canonical contract-family identity;
- source Application and source role;
- target Application and target role;
- traffic context;
- whether the family is latency-sensitive;
- canonical Foundation FCR dependencies where the required runtime capability remains unimplemented.

It does not execute, route, publish, deliver, admit or authorize messages.

## 2. Core declared contract families

The current core declaration set includes:

1. Trading -> FSAPMA market-data requirement request;
2. FSAPMA -> Trading normalized operational market-data delivery family;
3. Guardian -> Trading protection-command family;
4. Guardian -> FSAPMA provider-protection-command family;
5. Trading -> Guardian trading safety-state evidence projection;
6. FSAPMA -> Guardian provider operational-status evidence projection.

These are identities/declarations only. Their runtime realization remains gated by the relevant Foundation capabilities and later authorized Parts.

## 3. Fail-closed declaration rules

P1-D now rejects:

- undefined endpoint roles;
- undefined traffic contexts;
- source/target self-routes inside this cross-Application spine;
- invalid role pairs;
- malformed FCR identifiers;
- duplicate FCR identifiers within one contract declaration.

Permitted role pairs are only:

- Producer -> Consumer;
- Requester -> Responder.

## 4. Foundation/FCR boundary

The spine preserves these current dependencies:

- FCR-0004 for Guardian protection-command runtime capability;
- FCR-0005 for operational market-data delivery runtime capability;
- FCR-0006 for evidence/event/replay-capable delivery semantics where applicable;
- FCR-0009 for latency/deadline/QoS-aware cross-Application transport.

`ACCEPTED_FOR_PLANNING` is not treated as implementation authority. The declaration exists so later runtime wiring can fail closed until Foundation capability is approved, implemented and Application-verified.

## 5. Fast Track preservation

Every currently declared core contract family marked latency-sensitive binds FCR-0009.

This preserves the Part 0 requirement that cross-Application Fast Track behavior must not silently degrade into a latency-unaware route. P1-D itself does not assign Foundation technical priority, create queues or implement QoS.

## 6. Directional ownership

The contract spine preserves key ownership direction:

- normalized operational market data originates from FSAPMA, not Trading;
- protection commands originate from Trading Guardian, not Trading or FSAPMA;
- evidence/state projections flow toward Guardian without exposing sibling private memory.

No direct database/state access is introduced.

## 7. Dedicated verifier

Dedicated verifier project:

`applications/FSATS/verification/Falcon.FSATS.Part1.ContractSpine.Verifier/`

The verifier defines 14 gates:

1. contract IDs unique;
2. endpoints limited to known core Applications;
3. no self-route;
4. role pairs valid;
5. traffic context defined;
6. FCR IDs canonical;
7. FCR IDs unique per contract;
8. latency-sensitive contracts bind FCR-0009;
9. protection contracts bind FCR-0004;
10. market-data contracts bind FCR-0005;
11. evidence projections bind FCR-0006;
12. normalized market-data direction is FSAPMA -> Trading;
13. protection direction originates at Guardian;
14. no runtime send/route/publish/deliver/execute/activate method surface on the declaration registry.

## 8. Source-level Red-Team

Attacks reviewed:

- arbitrary string role typo creating ambiguous semantics;
- Producer/Producer or Requester/Consumer role mismatch;
- hidden self-route masquerading as cross-Application communication;
- duplicate FCR normalization hiding declaration defects;
- Trading becoming an operational market-data source;
- non-Guardian component originating protection commands;
- latency-sensitive route losing FCR-0009 dependency;
- declaration registry growing runtime execution methods;
- FCR planning status being interpreted as route authority.

Disposition:

`PASS / NO OPEN P0-CRITICAL SOURCE-DESIGN FINDING`

During review, free-text endpoint roles were replaced by a governed enum, self-routes were rejected, and duplicate FCR declarations were changed from silent normalization to fail-closed rejection.

## 9. Execution-validation boundary

P1-D is not claimed as build-executed or verifier-executed yet.

P1-F must execute:

- clean Release build;
- dedicated P1-D verifier;
- integrated Part 1 verifier;
- architecture/security review;
- final Red-Team rerun.

## 10. P1-D disposition

`IMPLEMENTATION_COMPLETE`

`SOURCE_REVIEW = PASS`

`EXECUTION_VALIDATION = PENDING_P1-F`

P1-E remains independently blocked pending the approved Foundation WP-03 consumption boundary. Part 1 remains open.
