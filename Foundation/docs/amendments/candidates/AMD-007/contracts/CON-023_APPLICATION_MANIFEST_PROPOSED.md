# CON-023 — Application Manifest

**Identifier:** CON-023 — Proposed Reservation  
**Version:** Proposed 1.0  
**Status:** Proposed  
**Stage 1 Authority:** Not Granted

## Required Fields

Application ID, Suite ID, version, owner, technical identity, package digest/provenance, required Foundation services, FIL Contracts, Service Bus routes, resources, storage, Runtime, proposed technical criticality, recovery priority, downtime, isolation/restart/failover/degraded/safe-shutdown policies, Health/Lifecycle/Security Contracts, authority requests, Guardian requirement/ID/version/health/request authority/failure policy, FFG communication, upgrade, rollback, uninstallation, evidence, and compatibility.

## Rules

- fields SHALL describe technical integration, never business payload;
- structural validity SHALL not imply authority or admission;
- technical criticality is proposed until approved;
- undeclared dependency, route, permission, storage, or authority SHALL be denied;
- version changes affecting meaning require governed compatibility review;
- Manifest state and admission decision SHALL remain distinct and reconstructable.

