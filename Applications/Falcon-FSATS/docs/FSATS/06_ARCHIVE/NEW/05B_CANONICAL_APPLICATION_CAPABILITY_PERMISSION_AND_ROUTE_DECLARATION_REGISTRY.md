# FSATS SIA — Canonical Application Capability, Permission and Route Declaration Registry v1.0

**Package:** `FSATS-SIA-v0.1`
**Status:** `SEMANTIC REMEDIATION / DESIGN CANDIDATE`
**Triggered By:** `AC-GOV-001`
**Governing Boundary:** APP-001 / CON-023 / ADR-I012

## 1. Purpose

Provide exact stable Application-domain declaration identities for CON-023 Manifest materialization. A coding worker SHALL NOT invent capability/permission names or broaden scopes during implementation.

This registry does not grant Foundation permission, route admission, external egress, runtime, financial or Owner authority. It defines what an Application may request/declare subject to Foundation admission and current authority.

## 2. Identity Classes

```text
CapabilityId = falcon.cap.<application-domain>.<capability>
PermissionId = falcon.perm.<application-domain>.<permission>
ExternalPermissionId = falcon.perm.external.<class>.<purpose>
Route declaration = canonical falcon.xapp.* contract-family identity from 12A
Foundation dependency = exact Foundation capability/artifact identity supplied by Foundation/FCR evidence
```

No wildcard `*`, `all`, `any-app`, `fsats-internal` or equivalent broad permission is valid.

Unknown/unregistered ID:

```text
MANIFEST_VALIDATION = REJECT
RUNTIME_USE = DENY
```

## 3. APP-TRD Provided Capabilities

Canonical owner: `falcon.app.trading.core`.

| CapabilityId | Semantic scope |
|---|---|
| `falcon.cap.trading.core.market-universe` | current qualified instrument/universe projections owned by Trading |
| `falcon.cap.trading.core.feature-analysis` | versioned feature/analysis computation from admitted Data Products |
| `falcon.cap.trading.core.strategy-evaluation` | execute admitted StrategyVersions and emit evaluations |
| `falcon.cap.trading.core.trade-proposal` | orchestrate compatible evaluations into TradeProposal/NO_TRADE |
| `falcon.cap.trading.core.unified-risk-decision` | deterministic Trading Risk ALLOW/DENY/REDUCE within current policy |
| `falcon.cap.trading.core.capital-reservation` | reserve/release admitted Trading capital for approved proposals |
| `falcon.cap.trading.core.order-execution` | create/reconcile broker order intents/attempts through admitted broker boundary |
| `falcon.cap.trading.core.position-truth` | authoritative Application business position lifecycle from reconciled fills |
| `falcon.cap.trading.core.portfolio-capital-state` | Trading-owned portfolio/capital business truth |
| `falcon.cap.trading.core.learning-analytics` | non-authoritative learning/analytics/attribution outputs |
| `falcon.cap.trading.core.strategy-evolution-candidates` | isolated strategy/model candidate generation/evaluation packages |
| `falcon.cap.trading.core.resource-demand-reporting` | Trading resource-demand/degradation evidence, not resource authority |
| `falcon.cap.trading.core.presentation-projection` | least-privilege projection for Shared Web |
| `falcon.cap.trading.core.notification-request` | business notification/report request for Shared Communication |

## 4. APP-TRD Consumed Capabilities

| Consumed semantic capability | Owner / binding |
|---|---|
| canonical operational Data Products | APP-PMA via accepted contract family `falcon.xapp.trading.fsapma.trading.core.operational-data-product` |
| provider-service status | APP-PMA via accepted family `falcon.xapp.trading.fsapma.trading.core.provider-service-status` |
| Guardian protection directive/release | APP-GRD via accepted families |
| FSTSimA validation evidence | APP-SIM via accepted validation-evidence family |
| APP-RSC effective-resource state/directive | candidate only via #48/#56 families if APP-RSC accepted |
| Shared Web user intent | accepted P0-F family #24 |
| Shared Communication delivery/recipient outcome | accepted P0-F families #36/#40 |
| Foundation generic communication/lifecycle/resource/evidence | exact admitted Foundation contracts/artifacts; no local substitute |

## 5. APP-PMA Provided Capabilities

Owner: `falcon.app.trading.fsapma`.

```text
falcon.cap.trading.fsapma.provider-registry
falcon.cap.trading.fsapma.provider-certification
falcon.cap.trading.fsapma.provider-route-selection
falcon.cap.trading.fsapma.quota-capacity-management
falcon.cap.trading.fsapma.raw-input-quarantine
falcon.cap.trading.fsapma.data-normalization
falcon.cap.trading.fsapma.data-quality-reconciliation
falcon.cap.trading.fsapma.canonical-data-product-delivery
falcon.cap.trading.fsapma.provider-service-status
falcon.cap.trading.fsapma.resource-demand-reporting
falcon.cap.trading.fsapma.presentation-projection
falcon.cap.trading.fsapma.notification-request
```

None grants Foundation network/resource authority.

## 6. APP-PMA Consumed Capabilities

- Trading Data Product requirement family #1;
- Guardian provider-protection command/release semantics via families #8-10 as defined in 12A/09A;
- FSTSimA validation/non-Live inputs/evidence families #13/#14/#17;
- APP-RSC candidate resource families #45/#49/#53/#57 only if accepted;
- Shared Web user intent #23;
- Shared Communication outcomes #35/#39;
- future Foundation external provider egress/credential capability under FCR-0013;
- Foundation communication/evidence/resource projections via exact admitted contracts.

## 7. APP-GRD Provided Capabilities

Owner: `falcon.app.trading.guardian`.

```text
falcon.cap.trading.guardian.protection-observation
falcon.cap.trading.guardian.incident-qualification
falcon.cap.trading.guardian.protection-directive
falcon.cap.trading.guardian.protection-release
falcon.cap.trading.guardian.crisis-state
falcon.cap.trading.guardian.recovery-assessment
falcon.cap.trading.guardian.resource-demand-reporting
falcon.cap.trading.guardian.presentation-projection
falcon.cap.trading.guardian.notification-request
```

Guardian capability names do not imply unlimited command authority; every directive still requires exact authority instrument/scope/policy.

## 8. APP-GRD Consumed Capabilities

- Trading safety projection and command outcome families #5/#6;
- FSAPMA provider-integrity projection/outcome families #9/#10;
- FSTSimA validation evidence #16;
- APP-RSC candidate resource state/directive #50/#58 and report/outcome #46/#54 if accepted;
- Shared Web user intent #22;
- Shared Communication outcome/response #34/#38;
- Foundation lifecycle/security/communication/evidence interfaces as exact admitted dependencies.

## 9. APP-SIM Provided Capabilities

Owner: `falcon.app.validation.fstsima`.

```text
falcon.cap.validation.fstsima.deterministic-simulation
falcon.cap.validation.fstsima.market-scenario-simulation
falcon.cap.validation.fstsima.provider-service-simulation
falcon.cap.validation.fstsima.broker-execution-simulation
falcon.cap.validation.fstsima.account-capital-simulation
falcon.cap.validation.fstsima.fault-crisis-injection
falcon.cap.validation.fstsima.fidelity-calibration
falcon.cap.validation.fstsima.independent-validation-evidence
falcon.cap.validation.fstsima.research-candidate-sandbox
falcon.cap.validation.fstsima.resource-demand-reporting
falcon.cap.validation.fstsima.presentation-projection
falcon.cap.validation.fstsima.notification-request
```

Every capability remains non-Live/non-authoritative for operational business truth.

## 10. APP-SIM Consumed Capabilities

- Trading/Guardian/FSAPMA validation-input families #11-13;
- FSAPMA non-Live Data Product input #14;
- APP-RSC candidate resource directives/state #51/#59, report/outcome #47/#55 if accepted;
- Shared Web user intent #25;
- Shared Communication outcome/response #37/#41;
- future research-only egress FCR-0008 and non-Live isolation FCR-0011 when Foundation capability exists;
- no production broker/provider credentials.

## 11. Candidate APP-RSC Provided Capabilities

Only if D01 Owner-accepted.

Owner: `falcon.app.resource.fsarm`.

```text
falcon.cap.resource.fsarm.aggregate-resource-picture
falcon.cap.resource.fsarm.coordination-envelope-consumption
falcon.cap.resource.fsarm.internal-redistribution-plan
falcon.cap.resource.fsarm.degradation-rebalance
falcon.cap.resource.fsarm.foundation-additional-request
falcon.cap.resource.fsarm.restoration-plan
falcon.cap.resource.fsarm.effective-resource-state
falcon.cap.resource.fsarm.resource-evidence
```

Explicitly forbidden capability identities:

```text
falcon.cap.resource.fsarm.foundation-resource-grant
falcon.cap.resource.fsarm.foundation-resource-truth
falcon.cap.resource.fsarm.foundation-criticality-owner
```

They SHALL NOT be registered.

## 12. Candidate APP-RSC Consumed Capabilities

- exact constituent resource-demand families #44-47;
- exact constituent effect-outcome families #52-55;
- Foundation Stage6 Application-facing resource-state/load-shedding and resource request/decision contracts supplied by Foundation;
- no Trading/Provider/Guardian business internals.

## 13. Internal Application Permissions

Permissions authorize a component/LSA to invoke an Application-owned port only within the same Application. They do not cross an Application trust boundary.

Canonical initial pattern:

```text
falcon.perm.<app>.read.<owned-projection>
falcon.perm.<app>.command.<owned-port>
falcon.perm.<app>.write.<owned-aggregate>
falcon.perm.<app>.candidate.write.<component-scope>
```

The exact initial privileged write/command permissions are:

### Trading

```text
falcon.perm.trading.core.command.risk-evaluate             -> T-LSA-06 caller, T-LSA-07 owner
falcon.perm.trading.core.command.capital-reserve           -> T-LSA-07 approved flow, T-LSA-08 owner
falcon.perm.trading.core.command.execution-create-intent   -> T-LSA-08 approved flow, T-LSA-09 owner
falcon.perm.trading.core.write.risk-decision               -> T-LSA-07 only
falcon.perm.trading.core.write.capital-reservation         -> T-LSA-08 only
falcon.perm.trading.core.write.order-position-state        -> T-LSA-09 only
falcon.perm.trading.core.candidate.write.strategy          -> exact T-LSA-12/eligible CSA candidate workspace only
```

### FSAPMA

```text
falcon.perm.trading.fsapma.write.provider-registry         -> P-LSA-01 only
falcon.perm.trading.fsapma.write.data-product-definition   -> P-LSA-02 only
falcon.perm.trading.fsapma.write.entitlement               -> P-LSA-03 only
falcon.perm.trading.fsapma.command.route-select            -> P-LSA-04 only
falcon.perm.trading.fsapma.write.quality-reconciliation    -> P-LSA-05 only
falcon.perm.trading.fsapma.write.quota-capacity-state      -> P-LSA-06 only
```

### Guardian

```text
falcon.perm.trading.guardian.write.incident                -> G-LSA-01 only
falcon.perm.trading.guardian.command.issue-directive       -> G-LSA-02 only after authority validation
falcon.perm.trading.guardian.write.crisis-state            -> G-LSA-03 only
falcon.perm.trading.guardian.command.release-protection    -> G-LSA-04/G-LSA-02 governed recovery route, exact policy
```

### FSTSimA

```text
falcon.perm.validation.fstsima.write.run-definition        -> S-LSA-01 only
falcon.perm.validation.fstsima.write.scenario-state        -> S-LSA-01/S02 exact owners
falcon.perm.validation.fstsima.write.execution-sim-state   -> S-LSA-04 only
falcon.perm.validation.fstsima.write.calibration-candidate -> S-LSA-07 only
falcon.perm.validation.fstsima.write.validation-assessment -> S-LSA-08 only
```

### APP-RSC candidate

```text
falcon.perm.resource.fsarm.write.resource-picture          -> R-LSA-01
falcon.perm.resource.fsarm.command.redistribute             -> R-LSA-02 under current envelope
falcon.perm.resource.fsarm.command.foundation-request       -> R-LSA-03 via Foundation contract
falcon.perm.resource.fsarm.write.restoration-evidence       -> R-LSA-03
```

No same-App permission permits bypassing the owning LSA's business preconditions.

## 14. External Permission Registry

These are declaration requests only; current runtime availability remains governed by Foundation/FCR + Owner environment authority.

```text
falcon.perm.external.provider.operational-data-egress
  requester = APP-PMA only
  Foundation gate = FCR-0013 / Stage12
  environment = explicit admitted operational environment

falcon.perm.external.broker.order-execution-egress
  requester = APP-TRD only
  Foundation gate = FCR-0014 / Stage12
  environment = PAPER | TINY_LIVE | LIVE only with exact separate authority

falcon.perm.external.research.research-only-egress
  requester = exact eligible Application Awareness entity
  Foundation gate = FCR-0008 / Stage12
  Trading direct route additionally prohibited by accepted Trading Awareness rule; Trading uses FSTSimA-contained research route

falcon.perm.external.fsa.msa-submission
  requester = exact Application MSA adapter seam
  Foundation gate = FCR-0030/FCR-0012
  no production-adoption authority
```

No Application may register a generic Internet permission.

## 15. Environment Scope

Every permission declaration includes exact environment set.

Default:

- internal read/write/compute business permissions exist only while Application lifecycle admits that component and do not imply financial operation;
- broker execution permission is environment-specific and requires a separate effective EnvironmentAuthorityRef;
- simulation/replay permissions cannot be reused for operational routes;
- research permission cannot be reused as provider operational data permission.

## 16. Cross-Application Route Declaration

A Manifest route declaration SHALL reference the canonical `falcon.xapp.*` business contract family from 12A plus exact direction/role:

```text
ProvidedContractFamilyId
ConsumedContractFamilyId
AllowedEnvironmentClasses
ExpectedCounterpartyApplicationId or unresolved external-owner gate for Shared Web/Communication
RequiredFoundationCommunicationDeclarationRef
```

The SIA does **not** create a second `CapabilityId` that substitutes for the canonical contract family at the cross-App boundary.

## 17. Foundation Capability Consumption

Applications reference Foundation capabilities only by exact immutable Foundation identity supplied by current accepted Foundation evidence/binding.

If FCR-0016/artifact-consumption prevents build binding:

```text
DESIGN_IDENTITY may be declared
BUILD_REFERENCE = FAIL_CLOSED / PENDING
```

No copied Foundation source or invented `falcon.cap.foundation.*` ID inside Application registry.

## 18. Permission Reduction / Revocation

Effective permissions are the intersection of:

```text
Manifest-declared permission
Foundation admission/permission state
Application lifecycle state
Environment authority
Guardian restriction where applicable
security/integrity restrictions
current credential/resource eligibility
```

No local cache may retain a broader permission after authoritative revocation/version change.

## 19. Verification Families

Verifier SHALL reject:

1. unknown capability/permission ID;
2. wildcard/broad all-App permission;
3. duplicate capability owner;
4. APP-TRD provider operational egress permission;
5. APP-PMA broker execution permission;
6. APP-SIM production credentials/egress;
7. APP-RSC Foundation grant/truth capability;
8. internal component write outside owner LSA;
9. candidate write permission targeting active artifacts;
10. generic Internet permission;
11. environment classification mismatch;
12. cross-App route not referencing canonical 12A family;
13. route declaration with guessed Shared Web/Communication ID;
14. local Foundation capability ID/source copy;
15. revoked permission remaining effective.

## 20. Finding Disposition

```text
AC-GOV-001 = REMEDIATED_AT_DESIGN_CANDIDATE_LEVEL
CANONICAL_CAPABILITY_IDS = MATERIALIZED
CANONICAL_PERMISSION_IDS = MATERIALIZED
EXTERNAL_PERMISSION_GATES = MATERIALIZED
CROSS_APP_ROUTE_IDENTITY = 12A CANONICAL FAMILY
```
