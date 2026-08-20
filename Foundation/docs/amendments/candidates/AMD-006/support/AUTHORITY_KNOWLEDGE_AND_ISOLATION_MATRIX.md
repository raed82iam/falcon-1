# Authority, Knowledge, and Isolation Matrix

**Status:** Approved Supporting Architecture  
**Approval Record:** GOV-062

| Concern | Trading Guardian | FFG | FSA | AUT-001 | Execution owner |
|---|---|---|---|---|---|
| Trading safety | decides within mandate | no business judgment | no business judgment | validates authority | executes authorized action only |
| Platform safety | may request | decides within mandate | supplies evidence | validates authority | executes owned action |
| Trading restriction | owns condition | cannot release | may supply technical evidence | enforces authority effect | applies at boundary |
| Platform restriction | cannot release | owns condition | verifies repair | enforces authority effect | applies at boundary |
| other Application isolation | may request | decides | investigates | validates | Runtime/Lifecycle/Bus/Resources execute |
| broker-facing action | requests protection | no authority | no authority | validates | Broker Execution owns |
| Foundation repair | no authority | protects during repair | diagnoses/verifies under playbook | validates | competent Foundation owner |

## Knowledge

| Information | Trading Guardian | FFG |
|---|---|---|
| capital, exposure, positions, orders | allowed when authorized | prohibited |
| Trading execution/risk readiness | allowed | technical summary only |
| Application/component identity | allowed | allowed |
| technical criticality and dependencies | allowed | allowed from governed source |
| another Application’s business records | prohibited | prohibited |
| technical envelope, integrity, route, resource effect | allowed as relevant | allowed |

## Isolation

| Target | Trading Guardian | FFG |
|---|---|---|
| Trading capability | restrict within mandate | may impose Platform restriction |
| Trading Application runtime | request or Trading-local action if explicitly authorized | may isolate technically |
| another Application | request only | may isolate technically |
| Foundation component | request only | may isolate within mandate |
| FFG | challenge/request independent isolation | cannot conclusively self-isolate/recover |

No awareness or knowledge row creates authority.
