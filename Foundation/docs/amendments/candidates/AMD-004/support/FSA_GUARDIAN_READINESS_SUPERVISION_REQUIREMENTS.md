# FSA Guardian Readiness Supervision Requirements

**Status:** Approved Supporting Architecture  
**Approval Record:** GOV-061  
**Scope:** FSA-side technical awareness, repair, and evolution boundary only  
**Guardian Architecture Authority:** Not Established by this document

## 1. Purpose

Define how FSA supervises technical readiness of a Foundation-owned Guardian capability without changing Guardian’s authority, mandate, placement, or final architecture.

## 2. Readiness Evidence

FSA SHALL use periodic and event-driven evidence for applicable:

- runtime availability and heartbeat;
- artifact version and integrity;
- configuration and protection-policy loading;
- Authority Engine connectivity;
- Safe Mode activation path;
- isolation path;
- restriction persistence;
- evidence-recording path;
- failover and standby readiness;
- independent stop-channel readiness;
- Recovery Guard readiness;
- dependency readiness.

Guardian self-report SHALL NOT be the exclusive evidence.

Safe readiness tests and independent observations SHALL be used where possible.

## 3. Readiness States

- `READY`
- `READY_WITH_CONSTRAINTS`
- `DEGRADED`
- `UNAVAILABLE`
- `INTEGRITY_FAILURE`
- `REPAIR_REQUIRED`
- `INDEPENDENT_PROTECTION_REQUIRED`
- `UNKNOWN`

Unknown SHALL NOT be treated as Ready.

## 4. Degraded Response

When readiness is insufficient, FSA SHALL:

- represent protection as degraded or unavailable;
- preserve all active Guardian restrictions;
- inform Authority Engine, Security, Lifecycle, Recovery, and competent governance;
- restrict Foundation activity requiring Guardian protection;
- activate Approved independent technical protection paths within authority;
- initiate bounded repair only under an Approved Guardian Repair Playbook;
- verify restored technical readiness;
- preserve evidence and escalate failed repair.

## 5. Permitted Repair

Under Approved authority and playbook, FSA MAY:

- restart Guardian runtime;
- isolate a corrupted instance;
- fail over to an Approved standby;
- reload Approved Guardian configuration and policy;
- restore an Approved Guardian artifact version;
- reconnect Approved dependencies;
- restore Approved evidence or restriction-persistence mechanisms;
- verify readiness.

## 6. Controlled Guardian Evolution

FSA MAY create and test a candidate improved Guardian technical version in Approved isolation.

Independent validation, continuity of protection, Digital City evaluation, and Project Owner approval are mandatory before admission.

## 7. Prohibitions

FSA SHALL NOT:

- change Guardian jurisdiction, authority, mandate, policy meaning, or release conditions;
- remove or release Guardian restrictions;
- declare Guardian business or risk policy;
- approve, activate, or deploy a Guardian candidate it created;
- disable independent protection during repair or evolution;
- validate its own Guardian repair conclusively where independent evidence is required;
- decide final Guardian architecture.

## 8. Acceptance Evidence

Evidence shall demonstrate:

- independent readiness observation;
- detection of false Guardian self-health;
- preservation of active restrictions;
- fail-closed behavior under unknown readiness;
- Approved-version repair;
- failed-repair escalation;
- inability to alter mandate or release restrictions;
- candidate isolation and non-activation;
- continuity of independent protection.
