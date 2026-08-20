# AMD-008 v0.2 Focused Change Summary

**Status:** APPROVED PENDING COORDINATED ACTIVATION  
**Approval Record:** GOV-063  
**Owner Approval Statement:** Issued and recorded in GOV-063  
**Activation:** Not Authorized  
**Stage 1:** Blocked

## Corrections Applied

### Mandatory LSA Ownership

The prior optional wording was removed.

Approved rule pending coordinated activation:

> Every major Application branch SHALL own exactly one LSA responsible for that branch.

CSA remains optional and is permitted only for eligible intelligent components.

### Origin-Aware Self-Development

The fixed CSA-first diagram was replaced by four origin-aware paths:

```text
CSA → Parent LSA → Application MSA → FSA
LSA → Application MSA → FSA
Application MSA → FSA
FSA → separate Foundation self-development governance and approval lifecycle
```

No entity may be inserted below the actual proposal origin.

### FSA Authority Clarification

AMD-008 now cross-references GOV-AUT-001 and GOV-001.

FSA performs final OS-governance and compatibility review only. Its outcome is not:

- documentary activation;
- implementation approval;
- deployment approval;
- production adoption; or
- authority expansion.

Final activation and adoption require separately authorized Project Owner and governance decisions defined by Falcon governance.

## Documents Revised

- AMD-008 README and impact report;
- ADR-I015;
- AWR-006 v2.0;
- AWR-007 v2.0;
- AWR-008 v1.1;
- APP-001 v1.1;
- CON-023 v1.1;
- requirements traceability and validation;
- Owner Approval Package.

## Preserved Boundaries

No historical or active document was modified during preparation of AMD-008 v0.2. At that preparation point, no approval statement had yet been issued. GOV-063 subsequently recorded the Project Owner's architectural approval without granting documentary activation, Stage 1 preparation, verification execution, implementation, deployment, external connection, production action, or financial activity.
