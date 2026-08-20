# P1-G — FSAPMA 6-LSA Code-Ready Decomposition

**Status:** `DESIGN_MATERIALIZATION / OWNER_DIRECTED_COMPLETION_CYCLE`  
**Scope:** `P1-G DESIGN ONLY`  
**Implementation Authority:** `NOT_GRANTED`  
**Runtime Authority:** `NOT_GRANTED`

## 1. Boundary

FSAPMA is the sole FSATS operational external-data/provider-management Application. It owns provider registry, provider capability/entitlement truth, data-product semantics, normalization, provider selection/routing/delivery, data quality/reconciliation, quota/capacity/cost/reliability. It does not own Trading decisions, Trading Risk, broker execution truth, Guardian authority, FSTSimA simulation truth, APP-RSC coordination authority, Foundation resource governance or secret-storage infrastructure.

Physical placement follows P1-C:

```text
FSAPMA.Contracts
FSAPMA.Domain
FSAPMA.Application
FSAPMA.Infrastructure
FSAPMA.Awareness
FSAPMA.Host
```

No consumer may access provider SDK/session/cache/state directly across Application boundaries.

## 2. P-LSA-01 Provider Registry & Onboarding

Components: `ProviderRegistry`, `ProviderProfile`, `ProviderRoleCatalog`, `ProviderOnboardingCase`, `ProviderLifecycleEvaluator`.

Owns provider identity, declared roles, supported markets/data products, onboarding evidence, current provider usability classification and non-secret provider metadata. Provider registration does not imply entitlement, credential readiness, route activation or operational authority.

## 3. P-LSA-02 Data Products, Semantics & Normalization

Components: `DataProductCatalog`, `SchemaSemanticRegistry`, `NormalizationPipeline`, `UnitPrecisionValidator`, `ProducerInstrumentIdentityService`.

Owns provider-facing data-product semantics and producer-owned instrument/data identities. Normalization preserves provenance and source semantics. Trading maps producer identities into Trading-owned instrument identities through governed contracts; FSAPMA never imports Trading domain internals.

Invariant: `NORMALIZED != AUTHORITATIVE_FOR_TRADING_DECISION`.

## 4. P-LSA-03 Provider Capability, Account & Entitlement

Components: `ProviderCapabilityRegistry`, `ProviderAccountContext`, `EntitlementEvaluator`, `CredentialReferenceConsumer`, `CapabilityFreshnessEvaluator`.

Owns truth about whether a provider/account/entitlement can supply a requested data role. Capability states must distinguish supported/unsupported/conditional/unknown and freshness. Secret bytes remain outside ordinary Application state/logs/Manifest. FSAPMA may consume governed provider/service credential references when an operational provider role requires them; this does not create a blanket user credential requirement for advisory users.

## 5. P-LSA-04 Provider Selection, Routing & Delivery

Components: `ProviderController`, `SelectionPolicyEvaluator`, `QuotaAwareRouter`, `DeliveryCoordinator`, `FailoverCoordinator`, `ConsumerSubscriptionRegistry`.

Provider Controller is an operational controller, not an Awareness tier or CSA. It selects/routes among currently admitted/eligible providers based on capability, entitlement, freshness, quality, quota, reliability, cost and current policy. It does not grant provider authority, invent data, or bypass P-LSA-03 eligibility.

Delivery to other Applications occurs only via P1-K governed contracts/routes. Direct in-process consumer coupling is forbidden.

## 6. P-LSA-05 Data Quality, Verification & Reconciliation

Components: `QualityEvaluator`, `CrossProviderVerifier`, `FreshnessMonitor`, `AnomalyDetector`, `CorrectionReconciler`, `LineageRecorder`.

Owns provider/data quality truth, discrepancy state, correction lineage and reconciliation. Conflicting sources are not silently averaged into false certainty. Stale/unknown/conflicted states are explicit and propagated.

## 7. P-LSA-06 Quota, Capacity, Cost & Reliability

Components: `QuotaLedger`, `RateLimitWindowTracker`, `CapacityForecast`, `CostLedger`, `ReliabilityModel`, `WorkloadDegradationPlanner`.

Owns provider-business quota/capacity/cost/reliability semantics, not Foundation technical resource authority. It determines API-call budgets, subscription limits, provider-side throttling risk and reclaimable/degradable workload evidence.

## 8. APP-RSC Resource Interface

FSAPMA exposes attributable evidence for compute/network/memory/workload pressure and provider-workload minimum-safe/desired/reclaimable/degradable requirements to APP-RSC. Provider quota limits remain FSAPMA business constraints and are not converted into Foundation grants.

```text
PROVIDER_QUOTA != FOUNDATION_RESOURCE_QUOTA
FSAPMA_PRESSURE_EVIDENCE != FOUNDATION_GRANT
```

APP-RSC may coordinate eligible FSATS technical resources but cannot choose provider truth, alter quality grades, rewrite entitlements, or fabricate quota capacity.

## 9. External Egress Boundary

All external provider access is explicit, credential-gated, environment-scoped, attributable and fail closed. No direct Internet/provider path is available to Trading, Guardian or APP-RSC merely because FSAPMA can reach a provider.

Provider SDK/library failures, credential expiry/revocation, entitlement change, quota exhaustion, provider outage and schema drift produce explicit states rather than silent fallback.

## 10. Degraded / Safety Behavior

- primary provider failure -> use only an independently eligible alternate according to current policy;
- all eligible providers unavailable -> mark affected data unavailable/unknown and deny claims of freshness;
- quality conflict -> preserve conflicting evidence and reconciliation state;
- quota pressure -> degrade/pause lower-priority data products before violating provider limits;
- APP-RSC resource reduction -> apply FSAPMA-owned shedding order while preserving declared minimum-safe operational data obligations where feasible;
- FSAPMA AI fault -> AI-dependent optimization stops; deterministic provider-state, quota, delivery-safety and evidence functions continue where independently trustworthy;
- recovery follows accepted AI Repair/Controlled Recovery V3.

## 11. Concurrency / Idempotency

Provider registrations, entitlement state, subscription/delivery intents, quota reservations and correction records require stable identity/versioning. Duplicate provider messages and reconnect/replay behavior must be idempotent or explicitly reconciled. No correction may overwrite provenance of the original observation.

## 12. Required Later Implementation Tests

Provider onboarding without entitlement; credential missing/expired/revoked; provider returns malformed schema; stale timestamp; duplicate tick/bar; out-of-order correction; conflicting providers; rate-limit near exhaustion; failover to ineligible provider denied; cost ceiling reached; APP-RSC pressure shedding; provider reconnect replay; FSAPMA AI Kill during active delivery; consumer receives stale/unknown classification; simulation/replay data attempts operational route.

## 13. P1-G Closure Invariants

- exactly six major branches with one accountable LSA each;
- Provider Controller remains inside P-LSA-04;
- no operational provider path bypasses FSAPMA;
- provider/data business truth remains FSAPMA-owned;
- provider quotas/cost do not become Foundation resource authority;
- APP-RSC receives only attributable resource evidence, not provider authority;
- external data provenance/freshness/quality survives normalization and delivery;
- no user broker credential is conflated with provider/service credential dependencies.
