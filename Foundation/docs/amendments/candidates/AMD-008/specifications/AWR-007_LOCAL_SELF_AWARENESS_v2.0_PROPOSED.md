# AWR-007 — Local Self-Awareness

**Identifier:** AWR-007  
**Version:** 2.0  
**Status:** APPROVED PENDING COORDINATED ACTIVATION  
**Approval Record:** GOV-063  
**Activation:** Not Authorized

## Purpose

Define awareness for exactly one major branch inside one Falcon Application.

Every major Application branch SHALL own exactly one LSA responsible for that branch.

## Requirements

LSA SHALL:

- belong to one declared major Application branch and one parent MSA;
- understand branch state, purpose, owned components, dependencies, performance, limitations, failures, and improvement opportunities;
- govern awareness and proposal aggregation for eligible CSAs in that branch;
- evaluate branch-level value, safety, readiness, ownership, and evidence;
- route an LSA-originated production-bound proposal to its parent MSA and then FSA;
- route a CSA-originated production-bound proposal to its parent MSA only after LSA review, and then to FSA;
- remain functional within its admitted resource and permission boundary.

LSA SHALL NOT:

- represent the complete Application;
- control another branch or Application;
- bypass MSA or FSA;
- modify Foundation or another owner's assets;
- create authority through awareness;
- approve its own production adoption.

Branch boundaries, ownership, dependencies, communication, and recovery behavior SHALL be declared by the Application Contract.

No CSA SHALL be inserted below an LSA-originated proposal. FSA review is the final OS-governance and compatibility review only; it does not constitute activation, implementation, deployment, or production adoption under GOV-AUT-001 and GOV-001.
