# 03 - Tool Candidate and Source Review

| Candidate ID | Capability ID | Product or component | Publisher | Exact version | Artifact identity | Local or canonical source evidence | License identity | Platform | Runtime dependency | Network requirement | Offline capability | Output format | Candidate status |
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
| CAND-001 | S1-TCAP-005 | Controlled test execution stack: Microsoft.Testing.Platform + MSTest + Microsoft.NET.Test.Sdk | NOT_APPLICABLE_TO_STAGE_1 | NOT_EVALUATED_STAGE_1_NOT_APPLICABLE | NOT_EVALUATED_STAGE_1_NOT_APPLICABLE | `docs/stage-1-proposal/07_STAGE_1_ENVIRONMENT_TOOLCHAIN_AND_RESOURCE_PLAN.md` | NOT_EVALUATED_STAGE_1_NOT_APPLICABLE | Windows Foundation build environment | .NET SDK-bound | No external connectivity permitted | Offline use would require governed admission | test results and structured evidence | `NOT_APPLICABLE_TO_STAGE_1` |

## Source review

- The candidate stack is represented as one controlled verification capability,
  not as three separate capability gaps.
- The source evidence shows the package names and versions, but the canonical
  exit gate does not require a new Stage 1 admission decision for behavioral
  testing.
