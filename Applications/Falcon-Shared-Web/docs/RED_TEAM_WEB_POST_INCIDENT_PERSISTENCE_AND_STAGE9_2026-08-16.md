# Shared Falcon Web Fresh Red Team

Date: 2026-08-16
Scope: material Web changes after `RED_TEAM_REMEDIATION_REVIEW_2026-08-15.md`
Reviewed branch: `web-development`
Reviewed source checkpoint: `82e671d7868dcabb015e73b053e4f784b75aeee5`

## Result

```text
SOURCE_REVIEW = PASS_WITH_OPEN_EXTERNAL_OR_RUNTIME_BINDING_BLOCKERS
CRITICAL = 0
HIGH_SOURCE_DEFECTS_UNREMEDIATED = 0
MEDIUM_SOURCE_DEFECTS_UNREMEDIATED = 1
GOVERNED_EXECUTABLE_VERIFICATION = PENDING
FCR_CLOSURE_ELIGIBILITY = NO
PRODUCTION_READINESS = NO
```

This Red Team does not claim `npm test` or `npm run check` passed. The current execution environment cannot resolve `github.com`, and the repository has no Web-specific GitHub Actions workflow on `web-development`; the existing workflow is Application-only and triggers on `application-development`.

## Material areas challenged

- persistent A-to-Z Incident Conversation;
- text/voice mixed chronology;
- local voice recording/transcription persistence order;
- Live Voice silence semantics;
- screenshot fail-closed scanner behavior;
- Support availability and explicit takeover boundaries;
- durable incident state transitions;
- mandatory closure summary ordering;
- affected position/order and FSTSimA projection boundaries;
- portfolio null/no-source semantics;
- strategy/catalog and on-demand analysis contract validation;
- Stage 9 recovery/release presentation authority boundaries;
- stale/partial/unavailable truth preservation;
- executable verification coverage.

## Findings remediated during this Red Team

### RT-WEB-20260816-01 — HIGH — incident state could outrun durable evidence

Original problem: Incident controller mutations such as Support takeover changed in-memory state before durable event persistence completed. A storage failure could therefore leave the UI claiming takeover without a persisted takeover event.

Remediation:

- IndexedDB now supports atomic `record + event` commit across `records` and `events` stores.
- Incident controller computes the next timeline/record before commit and publishes it in memory only after durable success.
- Support request, Support availability, explicit takeover, release, and resolution now follow durable commit success.
- Resolution cannot become `RESOLVED` until the mandatory closure summary has been durably recorded.

Disposition: `REMEDIATED_IN_SOURCE`.

### RT-WEB-20260816-02 — HIGH — event journal was written but not replayed on refresh

Original problem: events were stored independently but `initialize()` restored only the record-embedded timeline. A record-save failure after a successful event write could leave durable evidence outside the reconstructed timeline.

Remediation:

- persistence now exposes `loadEvents(incidentId)`;
- initialization merges record timeline and durable event journal by `eventId`;
- journal-only events repair the embedded record timeline;
- a record can be recovered from the durable journal instead of silently discarding events.

Disposition: `REMEDIATED_IN_SOURCE`.

### RT-WEB-20260816-03 — HIGH — Voice/Screenshot timeline publication preceded event durability

Original problem: voice and screenshot controllers appended events to the in-memory timeline before event persistence succeeded.

Remediation:

- voice and screenshot events are now created, persisted, and only then appended to the visible in-memory timeline.
- failed event persistence can no longer make the visible timeline claim an event was durably accepted.

Disposition: `REMEDIATED_IN_SOURCE`.

### RT-WEB-20260816-04 — HIGH — invented Stage 9 runtime field

Original problem discovered during review: the initial Web helper assumed a field named `systemOverview.stage9RecoveryRelease`. No current Foundation-published Web projection contract established that field.

Remediation:

- the invented field/path helper was removed;
- the Stage 9 module is now only a presentation validator for a future governed Web-consumable projection;
- it explicitly does not assume runtime method, property name, URL, transport, Foundation internal type, or endpoint.

Disposition: `REMEDIATED_IN_SOURCE`.

### RT-WEB-20260816-05 — MEDIUM — syntax check omitted newly added adapters

Original problem: `npm run check` did not include the new strategy/analysis adapter or Stage 9 adapter.

Remediation: both files were added to the exact `node --check` chain.

Disposition: `REMEDIATED_IN_SOURCE`.

## Open finding

### RT-WEB-20260816-06 — MEDIUM — persisted media artifact can become orphaned if its event write fails

Current sequence intentionally requires the artifact itself to persist before the event may reference it. If artifact persistence succeeds and the subsequent event persistence fails, the artifact remains in IndexedDB without a timeline reference because the current persistence port has no governed delete/rollback operation for artifacts.

Impact:

- no false customer-facing timeline claim occurs after the remediation above;
- however, orphaned local media may remain until an explicit cleanup/retention mechanism exists.

Required remediation before production readiness:

- add a governed artifact rollback/delete or transactional artifact+event commit;
- bind cleanup/retention to the authoritative privacy/data policy;
- do not invent retention duration in Web.

Classification: `MEDIUM / PRODUCTION_BLOCKING_PRIVACY_HOUSEKEEPING`.

## External / governed binding blockers that are not Web source defects

### Identity/session/tenancy

The current incident IndexedDB database is browser-local and is not yet bound to an authoritative Falcon principal/tenant namespace. Real identity/session/MFA remains governed separately. Until the authoritative identity/session contract exists and is bound, production incident persistence must remain fail-closed and must not be treated as multi-user production storage.

### Stage 9 runtime projection

Foundation Stage 9 is accepted and closed, but the current evidence does not publish an exact Web-consumable recovery/release runtime projection schema or route. Web therefore must not invent one. The new Stage 9 validator preserves authority boundaries but does not make FCR-0076 closure-eligible by itself.

Permanent distinctions preserved:

```text
REPAIR_SUCCESS != RELEASE
RESTART != RECOVERY
READY_FOR_RELEASE_DECISION != RELEASE
RELEASE_AUTHORIZATION != RELEASE_EXECUTION
LIFECYCLE_TRANSITION != NEW_AUTHORITY_DECISION
WEB_PRESENTATION != FOUNDATION_AUTHORITY
UI_CLICK != AUTHORIZATION
```

### Executable verification

Exact governed verification remains required:

```text
cd applications/shared/web
npm test
npm run check
```

No PASS is claimed until those exact commands complete successfully against the exact reviewed branch tip in a real checkout/runner.

## Incident-specific Owner decision reconciliation

The older ordinary AI-chat planning statement that conversation persistence/history remains unresolved cannot be applied to customer-facing Incident Conversations. Incident A-to-Z multimodal persistence is Owner-settled. Ordinary advisory AI-chat retention/deletion/export/tenancy policy remains separately governed.

Reconciliation record:

`applications/shared/web/docs/AI_CHAT_AND_INCIDENT_PERSISTENCE_RECONCILIATION_2026-08-16.md`

## Final Red Team disposition

```text
WEB_INCIDENT_DURABILITY_SOURCE_BLOCKERS = REMEDIATED_EXCEPT_ORPHAN_ARTIFACT_CLEANUP
WEB_STAGE9_AUTHORITY_LEAKAGE = NONE_FOUND_AFTER_INVENTED_FIELD_REMOVAL
WEB_EXECUTABLE_GOVERNED_VERIFICATION = PENDING
WEB_FCR_CLOSURE_ELIGIBILITY = NO
WEB_PRODUCTION_READINESS = NO
```
