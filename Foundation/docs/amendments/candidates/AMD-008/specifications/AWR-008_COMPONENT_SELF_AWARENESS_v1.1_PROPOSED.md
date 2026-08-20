# AWR-008 — Component Self-Awareness

**Identifier:** AWR-008  
**Version:** 1.1  
**Status:** APPROVED PENDING COORDINATED ACTIVATION  
**Approval Record:** GOV-063  
**Activation:** Not Authorized

## Eligibility

CSA is optional. Health reporting alone does not establish self-awareness.

A component MAY have CSA only when governed eligibility evidence establishes meaningful self-development value, including specialized intelligence, self-evaluation, learning or research capability, owned improvement opportunities, and safe candidate testing.

Deterministic validators, passive structures, simple storage adapters, basic configuration loaders, and other ordinary infrastructure components SHOULD NOT have CSA.

## Boundary

CSA SHALL:

- belong to one eligible intelligent component, one major branch, one LSA, and one Application;
- understand only its component condition, performance, limitations, evidence, and improvement opportunities;
- modify only isolated candidate assets it owns under separate authority;
- submit every completed production-bound proposal to its parent LSA.

CSA SHALL NOT expand responsibility, alter another owner's assets, bypass LSA/MSA/FSA review, increase its authority, or activate its candidate.

The production-bound route for a CSA-originated proposal is:

```text
CSA → Parent LSA → Application MSA → FSA
```

This route applies only when CSA is the actual origin. CSA remains optional and SHALL NOT be invented or inserted beneath an LSA- or MSA-originated proposal. FSA review is limited to OS governance and compatibility and does not grant activation, implementation, deployment, or production adoption authority under GOV-AUT-001 and GOV-001.
