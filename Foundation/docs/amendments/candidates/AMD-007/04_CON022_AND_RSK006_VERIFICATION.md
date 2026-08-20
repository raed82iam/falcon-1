# CON-022 and RSK-006 Verification

**Status:** Proposed

## CON-022

CON-022 is the correct generic Application Guardian Protection Request boundary because it:

- is independent of Trading;
- prohibits business payload;
- treats a request as non-commanding;
- requires identity, authority, integrity, evidence, expiry, replay, and independent FFG evaluation;
- supports reject, investigate, narrow, accept, strengthen, and provisional containment;
- separates decision from execution and Platform release from domain release.

Required v1.1 completion before activation:

- canonical request codes `REQUEST_TECHNICAL_INVESTIGATION`, `REQUEST_INCREASED_MONITORING`, `REQUEST_RESOURCE_PROTECTION`, `REQUEST_PRIORITY_PROTECTION`, `REQUEST_TRAFFIC_RESTRICTION`, `REQUEST_COMPONENT_ISOLATION`, `REQUEST_APPLICATION_ISOLATION`, `REQUEST_PLATFORM_CONTAINMENT`, and `REQUEST_PLATFORM_SAFE`;
- explicit acknowledgment and dead-letter outcomes;
- delivery-priority authority;
- CON-024 registration prerequisite;
- CON-031 audit/evidence binding;
- response-time violation and emergency-route handling;
- exact schema and FIL route profile.

CON-022 is therefore **correct but incomplete for activation**. It should be versioned to v1.1 after Owner approval; it should not be replaced.

## RSK-006

RSK-006 is the correct primary Specification identifier for Trading Guardian because:

- the Specification Tree assigns loss containment, safe states, limits, and crisis protection to RSK;
- Trading Guardian’s primary governed truth is Trading-domain risk and protection;
- architectural location in the Applications environment does not transfer primary subject ownership to APP;
- APP-002 governs the generic Application Guardian boundary, while RSK-006 specializes Trading protection;
- FIN and Broker Execution remain execution/financial-operation owners, not Guardian owners.

RSK-006 remains a proposed reservation pending Owner approval and registry admission. Its activation requires Trading Suite, Trading Risk, Broker Execution, FSATA, FSAOL, Manifest, and authority dependencies.

