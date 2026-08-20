# FSATS P0-NG — Web / Communication Identity and Contract-Coverage Reconciliation

**Status:** `FINAL_CONSOLIDATION_SEMANTIC_RECONCILIATION / NOT_FINAL_OWNER_CLOSED`  
**Affected Scope:** `P0-C + P0-F topology/contract interpretation only`  
**Implementation Authority:** `NOT GRANTED`

---

## 1. Finding

The final-consolidation review identified a potential ambiguity between:

1. P0-C's placement rule that a Web or Communication Application whose primary responsibility is trading-specific belongs inside the FSATS system boundary; and
2. the exact accepted P0-F 43-family baseline, whose current contract participants are explicitly `Shared Web` and `Shared Communication` for the applicable presentation/interaction/notification families.

Without clarification, a reader could incorrectly infer either:

- that the 43-family set already contains separate Trading Web / Trading Communication Application identities; or
- that a future trading-specific Web/Communication Application may reuse the Shared Application identity/contracts without a new governed declaration.

Both interpretations are invalid.

---

## 2. Controlling Interpretation

P0-C item `trading-specific Web / Communication Applications where responsibility remains trading-specific` is a **classification and placement rule**, not an assertion that a separately instantiated Trading Web Application and Trading Communication Application are already participants in the exact current 43-family contract graph.

The exact current 43-family P0-F baseline explicitly uses:

```text
SHARED_WEB
SHARED_COMMUNICATION
```

as the exact counterparties for the families materialized in P0-F sections 5.5 through 5.11.

Therefore:

```text
CURRENT_43_SHARED_WEB_IDENTITY
!= FUTURE_OR_SEPARATELY_INSTANTIATED_TRADING_WEB_IDENTITY

CURRENT_43_SHARED_COMMUNICATION_IDENTITY
!= FUTURE_OR_SEPARATELY_INSTANTIATED_TRADING_COMMUNICATION_IDENTITY
```

---

## 3. Placement Rule Preserved

The accepted FSATS classification rule remains:

- an Application whose primary enduring responsibility is trading-specific belongs inside the FSATS system boundary;
- a generic domain-independent Application intended for Falcon-wide reuse belongs in Shared Applications outside FSATS;
- both remain independent Falcon Applications under APP-001/CON-023 where instantiated as Applications;
- system-boundary placement never creates authority or hidden coupling.

This reconciliation does not move a trading-specific responsibility outside FSATS.

---

## 4. Exact Current Contract-Bearing Interpretation

For the exact 43-family baseline, the relevant current Application identities are interpreted literally from the accepted P0-F source:

- Trading;
- Trading Guardian;
- FSAPMA;
- FSTSimA;
- Shared Web;
- Shared Communication.

The 43-family baseline does not silently create additional Trading Web / Trading Communication identities.

```text
EXACT_43_PARTICIPANT_NAMES = LITERAL
NO_IMPLICIT_APPLICATION_IDENTITY = ALLOWED
```

---

## 5. Future / Separately Instantiated Trading-Specific Web or Communication Application

If a separately governed Trading Web Application or Trading Communication Application is later instantiated because its responsibility is materially trading-specific:

1. it SHALL live inside the FSATS system boundary;
2. it SHALL have its own immutable Application identity;
3. it SHALL declare its own MSA and qualified LSAs under P0-C/APP-001/CON-023;
4. it SHALL NOT reuse `Shared Web` or `Shared Communication` identity as an alias;
5. every cross-Application interaction SHALL receive an exact explicit P0-F contract family/edge;
6. the new family/edge may reuse a metadata/security profile but not another business contract identity;
7. both participants SHALL declare the relationship bilaterally;
8. Foundation/FCR/runtime readiness SHALL be re-evaluated;
9. fresh architecture/security/authority review SHALL occur for the material topology/contract change;
10. no addition is authorized merely because P0-C describes the placement rule.

The 43-family set is a current exact minimum baseline, not a permanent maximum.

---

## 6. Shared Application Consumption Rule

FSATS may consume Shared Web / Shared Communication only through the exact governed P0-F contracts.

Shared reuse does not mean:

- FSATS owns the Shared Application;
- the Shared Application owns Trading business semantics;
- Web click equals business authorization;
- notification delivery equals business outcome;
- Shared Application identity may be treated as a generic wildcard for any future Web/Communication Application.

---

## 7. Invariants

```text
TRADING_SPECIFIC_RESPONSIBILITY -> FSATS_PLACEMENT_IF_INSTANTIATED
GENERIC_FALCON_WIDE_RESPONSIBILITY -> SHARED_APPLICATION_PLACEMENT
PLACEMENT != INSTANTIATION
PLACEMENT != CONTRACT_EXISTENCE
CURRENT_43_SHARED_WEB != IMPLIED_TRADING_WEB
CURRENT_43_SHARED_COMMUNICATION != IMPLIED_TRADING_COMMUNICATION
NEW_APPLICATION_IDENTITY -> EXPLICIT_NEW_OR_REVISED_CONTRACT_GRAPH
SHARED_PROFILE_REUSE != CONTRACT_IDENTITY_REUSE
```

---

## 8. Forbidden Interpretations

Invalid interpretations include:

- “P0-C mentions trading-specific Web, therefore Trading Web is already one of the 43 participants”;
- “Shared Web can be renamed Trading Web without architecture impact”;
- “Shared Communication covers every future trading-specific communication workflow automatically”;
- “the 43-family count forbids adding a new exact family when a new Application is legitimately instantiated”;
- “placing an Application inside FSATS creates authority or cross-Application access”.

---

## 9. Final Consolidation Requirement

Fresh Architecture/Consistency and Red-Team review SHALL treat this reconciliation as controlling for the final-consolidation candidate.

If the candidate is finally Owner-accepted/closed, this meaning SHALL be integrated directly into the final Current Approved P0-C and P0-F documents so the active accepted design does not depend on this separate reconciliation record.
