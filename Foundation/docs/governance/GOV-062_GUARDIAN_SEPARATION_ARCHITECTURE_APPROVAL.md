# Guardian Separation Architecture Approval

**Identifier:** GOV-062  
**Version:** 1.0  
**Status:** Approved  
**Decision Date:** 2026-07-27  
**Decision Authority:** رائد عموره, Project Owner and current Falcon Constitutional Authority  
**Subject:** AMD-006 v0.1 Guardian separation architecture  
**Architectural Decision:** Approved  
**Documentary Activation:** Deferred  
**Stage 1 Authority:** Not Granted  
**Stage 1 Proposal Authority:** Not Granted  
**Implementation Authority:** Not Granted  
**Verification Execution Authority:** Not Granted  
**Operational Activation Authority:** Not Granted  
**Production, Cloud, and Financial Authority:** Not Granted

## 1. Approval Declaration

> **موافق على اعتماد AMD-006 v0.1 وADR-I011 وAUT-002 v2.1 وRSK-006 وCON-022 وجميع وثائق الدعم التابعة للحزمة، مع اعتماد الفصل بين Falcon Foundation Guardian وTrading Guardian، وبين Platform Safe Mode وTrading Safe Mode، دون بدء Stage 1 أو تنفيذ أو تفعيل أي مكوّن أو تشغيل أي خطة تحقق.**

## 2. Decision

The Project Owner approves:

- AMD-006 v0.1;
- ADR-I011 as the Accepted Guardian separation architecture;
- AUT-002 v2.1 as the Approved refined successor design for Falcon Foundation Guardian;
- RSK-006 as the Approved Trading Guardian design and identifier reservation;
- CON-022 as the Approved Application Guardian Protection Request Contract design and identifier reservation;
- the separation of Platform and Trading protective modes;
- all AMD-006 assessments, matrices, migration rules, diagrams, change sets, prerequisites, and supporting documentation; and
- VPL-GDN-002 as an Approved verification plan whose execution is not authorized.

## 3. Approved Boundary

- FFG protects Falcon Foundation and owns Platform protective modes.
- Trading Guardian protects the Trading Application Suite and owns Trading-domain protective modes within its mandate.
- Trading Guardian may understand authorized Trading-domain meaning.
- FFG SHALL NOT interpret Application business meaning.
- Application Guardians may request cross-Application technical protection.
- only FFG may impose cross-Application technical isolation or Platform protective modes.
- Trading Guardian SHALL NOT become Broker Execution.
- FSA retains Foundation awareness, diagnosis, verification, and bounded repair.
- AUT-001 retains authority validation.
- competent execution mechanisms retain ownership of their actions.

## 4. Independent Safe Modes

`PLATFORM_NORMAL` SHALL NOT imply `TRADING_NORMAL`.

`TRADING_NORMAL` SHALL NOT override a Platform restriction.

Platform and Trading restriction release remain separate, evidence-based, and authority-bound decisions.

## 5. Documentary State

This approval establishes binding architectural meaning but does not activate the described subjects.

AUT-002 v1.0 remains the current effective Guardian Specification.

AUT-002 v2.0 remains preserved as an Approved non-effective successor design under GOV-060.

AUT-002 v2.1, RSK-006, and CON-022 remain non-effective until:

1. required Contract, Catalog, Manifest, registry, Tree, glossary, baseline, and traceability updates are prepared;
2. unresolved Guardian authority and technical policy decisions are approved;
3. historical preservation and cross-document consistency are verified; and
4. the Project Owner approves a separate documentary activation record.

No Approved historical document is silently overwritten.

## 6. Permitted Follow-on Documentation

GOV-062 permits preparation for Owner review only of:

- technical-criticality governance;
- Application and Trading Suite Manifest Contracts;
- CON-011 successor treatment;
- Platform and Trading mode catalogs;
- trigger, survival-set, consequence, and release matrices;
- FFG high availability and independent stop-channel decisions;
- maximum autonomous containment duration and irreversible-action quorum;
- missing Trading Application boundary Specifications; and
- the coordinated documentary activation package.

Preparation does not equal approval, implementation, verification execution, or activation.

## 7. Preserved Prohibitions

GOV-062 SHALL NOT authorize:

- Stage 1 discussion, proposal preparation, commencement, or execution;
- Falcon or Guardian code creation or modification;
- FFG or Trading Guardian implementation, activation, deployment, or operation;
- VPL-GDN-002 or any other verification-plan execution;
- cross-Application isolation;
- Platform or Trading Safe Mode activation;
- local or cloud deployment;
- OCI preparation or use;
- production activity;
- broker, market-data, account, capital, or financial connection; or
- any financial activity.

## 8. Mandatory Stage 1 Stop

Falcon remains before Stage 1.

Before any Stage 1 discussion, proposal preparation, or action, the Project Owner SHALL receive a clear notice that Stage 1 is a new implementation phase with new scope and risk. Stage 1 requires a separate explicit approval.

## 9. Approval Record

| Role | Decision | Name | Date |
|---|---|---|---|
| Project Owner and current Falcon Constitutional Authority | Approved architecture; deferred activation; prohibited Stage 1 and verification execution | رائد عموره | 2026-07-27 |

