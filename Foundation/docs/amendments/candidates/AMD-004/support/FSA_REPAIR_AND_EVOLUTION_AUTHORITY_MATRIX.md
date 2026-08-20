# FSA Repair and Evolution Authority Matrix

**Status:** Approved Supporting Architecture  
**Approval Record:** GOV-061  
**Authority:** Proposed ADR-I009 and AWR-001 v2.0  
**Execution Authority:** Not Granted

## 1. Classification Rule

```text
Restores exact previously Approved trusted meaning
    → Self-Repair

Creates or changes governed meaning
    → Self-Evolution
```

Uncertainty about classification SHALL default to Self-Evolution and no activation.

## 2. Authority Matrix

| Action | FSA may perform autonomously | Owner notification | Independent validation | Owner approval | Digital City | Automatic rollback | Result |
|---|---:|---:|---:|---:|---:|---:|---|
| observe and diagnose Foundation condition | yes | when material | risk-based | no | no | no | awareness evidence |
| contain or isolate under existing authority | yes | material cases | post-action where required | no if pre-authorized | risk-based | no | restricted state |
| restart using Approved playbook | yes | per playbook | post-repair evidence | no if pre-authorized | no | not applicable | restored Approved state |
| reconnect Approved dependency | yes | per playbook | post-repair evidence | no if pre-authorized | no | not applicable | restored Approved state |
| reload Approved configuration | yes | per playbook | integrity verification | no if pre-authorized | risk-based | yes to prior Approved config | restored Approved state |
| fail over to Approved standby | yes | yes | post-repair verification | no if pre-authorized | risk-based | yes | restored Approved state |
| restore Approved version/backup/baseline | yes | yes | mandatory where material | no if pre-authorized | risk-based | yes | restored Approved state |
| repair Guardian runtime using Approved playbook | yes, technical scope only | mandatory | mandatory | no if pre-authorized | normally required for shared protection | yes | restored Guardian technical readiness |
| release Guardian restriction | prohibited | not applicable | not applicable | cannot be granted to FSA by this package | not applicable | not applicable | Guardian/competent release authority |
| change Guardian mandate or jurisdiction | prohibited | not applicable | mandatory governance | mandatory separate decision | required before activation where applicable | not applicable | separate Guardian architecture |
| investigate weakness | yes under analysis authority | when material | challenge required | no | no | no | investigation |
| research and compare designs | yes in isolated scope | per policy | reproducibility required | no | no | no | candidate research |
| create candidate code/config/schema/test | yes in Approved isolation | yes at proposal threshold | mandatory | mandatory before admission | consequence-based | not applicable | non-authoritative candidate |
| test candidate in Sandbox | yes under separate execution authority | result included | independent evidence mandatory | no for test execution | no | not applicable | validation evidence |
| test candidate in Digital City | yes under separate execution authority | result included | independent evidence mandatory | no for test execution | mandatory for listed impact | not applicable | system-impact evidence |
| issue FSA conformance recommendation | yes | included in package | independently reviewable | no | dependent on candidate | not applicable | recommendation only |
| approve FSA-created candidate | prohibited | not applicable | mandatory | Owner/competent authority | as required | not applicable | governed decision |
| deploy or activate candidate | prohibited without separate authority | mandatory | mandatory | mandatory | as required | pre-authorized | deployment mechanisms |
| register candidate as trusted baseline | prohibited without competent authority | mandatory | post-adoption verification | mandatory or delegated baseline authority | as required | yes | new trusted state |
| rollback failed candidate to last Approved state | yes when pre-authorized conditions apply | mandatory | restoration verification | conditions approved in advance | evidence may derive from Digital City/Canary | yes | Self-Repair |
| modify Application business logic/state | prohibited | not applicable | not applicable | separate Application process required | separate | not applicable | outside FSA scope |
| expand FSA jurisdiction | prohibited | Owner decision required | governance review | mandatory | not applicable | not applicable | separate authority decision |
| evolve FSA itself | candidate creation only | mandatory | enhanced independent validation | mandatory | mandatory | mandatory | candidate successor |

## 3. Repair Outcomes

- `REPAIRED_AND_VERIFIED`
- `REPAIRED_WITH_RESTRICTIONS`
- `REPAIR_INCOMPLETE`
- `REPAIR_FAILED`
- `ISOLATED_PENDING_REVIEW`
- `ESCALATION_REQUIRED`

## 4. Prohibited Changes

FSA shall never autonomously change:

- Vision or Constitution;
- architecture or jurisdiction;
- Approved Specification or Contract meaning;
- security, audit, evidence, isolation, or recovery obligations;
- Guardian authority, mandate, or restriction release;
- Project Owner reserved authority;
- Application business logic, users, data, financial objects, decisions, or policies;
- candidate status to trusted or production.

## 5. Shared Component Rule

A component is eligible only when authoritative ownership records establish Foundation ownership.

“Shared,” “reusable,” “infrastructure-like,” or “used by Foundation” does not by itself establish Foundation ownership.
