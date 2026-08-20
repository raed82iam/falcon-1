# FSATS P0-P7 Fresh Cross-Part Red-Team Review R2

**Date:** `2026-08-15`  
**Exact reviewed source:** `b922ef446dd0b99257acddfedfe81193ac1489fb`

## Adversarial challenges

1. **User/customer identity smuggling into FSATS:** blocked by broker-account-only public business scope.
2. **Two broker accounts collapse because broker/environment match:** blocked by BrokerAccountId in canonical scope, reservations, execution and reconciliation.
3. **Failure in account A poisons account B without evidence:** current containment is account-local unless shared-dependency/broker-wide evidence justifies expansion; unknown locality fails closed rather than fabricating local certainty.
4. **Broker A failure silently falls back to broker B/account B:** prohibited by P0-H and no fallback authority is minted.
5. **Provider name collapses ProviderAccount/ServiceRole/ApiInstance/Endpoint:** blocked by current route identity separation.
6. **A raw URL authorizes network access:** blocked; endpoint/config evidence grants no egress/runtime authority.
7. **Credential secret bytes enter contract/state:** current model uses credential references, not secret bytes.
8. **Higher provider reliability selects an incomplete legacy route:** current-route selector requires full ApiInstance/Endpoint binding.
9. **Same vendor name merges market-data truth with broker-execution authority:** prohibited by FSAPMA vs Trading ownership separation.
10. **Web sends customer ID and asks Trading to resolve it:** prohibited; Web performs mapping before the public request.
11. **Web treats null as zero:** prohibited by nullable fields plus explicit availability/truth/freshness/completeness semantics.
12. **Web collapses ACCEPTED/PARTIALLY_FILLED/FILLED or UNKNOWN_BROKER_OUTCOME/REJECTED:** distinct enums and contract rules preserve the states.
13. **A correction is mistaken for an ordinary newer update:** explicit update kind, correction/supersession lineage and per-account sequence block timestamp-only inference.
14. **Pagination token reused across another account/query/version:** prohibited; continuation token is opaque and scoped to the same logical traversal identity.
15. **Old FSARM candidate regains authority because it sorts first:** blocked by current Part1 reading overlay.
16. **43 predecessor + 22 P1K becomes 65 implied active routes:** prohibited by current lineage rule.
17. **Guardian uses crisis to seize Foundation resources:** prohibited; Guardian publishes evidence to APP-RSC, Foundation retains total-resource authority.
18. **APP-RSC becomes Foundation or FSATS container:** prohibited by P0-J/current topology.
19. **Performance fast path drops Risk/Guardian/capital/dispatch-time gates:** prohibited by P0-J.
20. **Paper/Shadow/FSTSimA pass becomes Tiny-Live/Live authority:** prohibited by P0-K.
21. **1:1 funded scope is interpreted as permission for leverage:** prohibited by P0-H.
22. **Historical closure bytes are rewritten to make later decisions appear earlier:** avoided; current corrections are explicit overlays/additive contracts.
23. **P7 is synthesized from Owner memory/current statement without bytes:** fail-closed; not fabricated.

## R2 Red-Team result

`OPEN CRITICAL = 0`  
`OPEN HIGH IN CURRENT P0-P6 STATIC MODEL = 0`  
`OPEN MEDIUM IN CURRENT P0-P6 STATIC MODEL = 0`  
`P7 CANONICAL-EVIDENCE BLOCKER = 1`

R2 does not claim executable validation. New adversarial source checks are materialized, but no CI/Owner-operated build result exists for the exact reviewed source in this review record.
