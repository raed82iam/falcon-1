# FCR-0008 - Non-Live Simulation Application Isolation and Egress Guard

**Status:** PROPOSED APPLICATION REQUIREMENT INPUT  
**Classification:** `PLANNED_CAPABILITY_INPUT`  
**Requester:** Falcon Self-Aware Trading Simulator Application (FSTSimA) / FSATS validation workflow  
**Foundation modification authority:** NOT GRANTED

## Requested Foundation capability

A generic governed Application-isolation and permission boundary suitable for non-Live simulation/validation Applications that must reuse admitted Falcon logic and contracts while being technically prevented from obtaining Live authority or accidentally reaching Live external endpoints.

## Exact FSATS use case

Final V1.3 defines FSTSimA as an independent non-Live Falcon Application outside FSATS operational authority. It runs the same Strategy, Risk, Portfolio, Guardian, Provider Controller, Execution and ledger logic through simulation clocks and external adapters, but must remain isolated from Paper/Live production state and credentials.

Required generic outcomes include:

- independent Application identity, lifecycle and resource allocation;
- separate credentials, stores, namespaces and authority scopes from Paper/Live Applications;
- explicit non-Live external-connectivity policy;
- egress denial/allow-list behavior that prevents accidental Live broker/provider endpoint access;
- no inheritance of Trading Application permissions, credentials or execution authority;
- declared simulation/replay routes that cannot be mistaken for authoritative production routes;
- evidence of denied unauthorized egress and denied production-state mutation;
- independently attributable security/permission decisions and audit evidence;
- safe replacement/removal without weakening isolation of FSATS or other Applications.

## Foundation evidence checked

APP-001, CON-023, ADR-I012 and ADR-I015 establish independent Application identity, permissions, lifecycle, isolation and governed communication. They define the correct architectural ownership boundary.

The remaining FSATS need is the concrete generic runtime enforcement behavior proving a non-Live Application cannot accidentally acquire or exercise Live network/credential/route authority.

## Observed gap

`PLANNED / RUNTIME ENFORCEMENT INTERFACE NOT YET CONFIRMED AVAILABLE`.

## Application-side alternatives

FSTSimA SHALL use simulation-specific clocks, provider/broker/exchange/account adapters and non-production configuration. It SHALL NOT implement a fake Foundation permission system, reuse Live credentials, or rely only on application code conventions as the safety boundary.

## Required boundary outcome

A generic Foundation-governed Application permission/isolation mechanism through which a non-Live Application can be admitted with explicit denied Live authority and enforceable egress/credential/route separation.

## Blocking impact

- Does NOT block V1.4 architecture/design.
- Does NOT block isolated specification or test-harness design.
- Blocks any claim that FSTSimA is runtime-safe against accidental Live access until enforceable Foundation isolation/egress controls are available and verified.

## Authority rule

This FCR is a request/design input only and grants no Foundation modification, implementation, deployment, external connectivity, Paper, Tiny Live or Live authority.