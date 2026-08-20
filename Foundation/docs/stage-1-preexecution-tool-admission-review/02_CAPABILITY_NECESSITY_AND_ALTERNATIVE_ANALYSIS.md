# 02 - Capability Necessity and Alternative Analysis

| Alternative ID | Capability ID | Candidate method or component | Admission state | External package required | Deterministic | Offline capable | Meets requirement | Security impact | Rejection reason | Disposition |
|---|---|---|---|---|---|---|---|---|---|---|
| ALT-001 | S1-TCAP-001 | Active .NET SDK payload with governed locked restore | Active admitted mechanism | No | Yes | Yes | Yes | Preserves governed restore boundary | None | `SATISFIED_BY_ACTIVE_ADMITTED_CAPABILITY` |
| ALT-002 | S1-TCAP-002 | SDK analyzers and governed command design | Active admitted mechanism | No | Yes | Yes | Yes | Does not widen authority | None | `SATISFIED_BY_ACTIVE_ADMITTED_CAPABILITY` |
| ALT-003 | S1-TCAP-003 | Repository inspection plus architecture policy | Active admitted mechanism | No | Yes | Yes | Yes | Preserves boundary | None | `SATISFIED_BY_ACTIVE_ADMITTED_CAPABILITY` |
| ALT-004 | S1-TCAP-004 | Documentary security and path-exclusion review | Active admitted mechanism | No | Yes | Yes | Yes | Preserves non-financial boundary | None | `SATISFIED_BY_ACTIVE_ADMITTED_CAPABILITY` |
| ALT-005 | S1-TCAP-005 | Microsoft.Testing.Platform + MSTest + Microsoft.NET.Test.Sdk | Not admitted for Stage 1 execution use | Yes | Yes if admitted | Yes if admitted | Potentially yes | Unknown until exact admission evidence exists | Behavioral testing is deferred to a later stage | `DEFERRED_TO_LATER_STAGE` |
| ALT-006 | S1-TCAP-006 | Traceability and evidence documentation design | Active admitted mechanism | No | Yes | Yes | Yes | Preserves evidence boundary | None | `SATISFIED_BY_ACTIVE_ADMITTED_CAPABILITY` |
| ALT-007 | S1-TCAP-007 | Current-state reconciliation and manifest review | Active admitted mechanism | No | Yes | Yes | Yes | Preserves manifest boundary | None | `SATISFIED_BY_ACTIVE_ADMITTED_CAPABILITY` |
| ALT-008 | S1-TCAP-008 | SDK-bound restore behavior under ADR-I002 and BLD-001 | Active admitted mechanism | No | Yes | Yes | Yes | Preserves dependency control | None | `SATISFIED_BY_ACTIVE_ADMITTED_CAPABILITY` |
| ALT-009 | S1-TCAP-009 | Deferred SBOM design and SPDX schema identity | Active admitted design boundary | No | Yes | Yes | Not yet required as execution output | Low | Stage 1 exit gate does not clearly require generated SBOM | `DEFERRED_TO_LATER_STAGE` |
| ALT-010 | S1-TCAP-010 | Constitutional review documents | Active admitted mechanism | No | Yes | Yes | Yes | Preserves authority separation | None | `SATISFIED_BY_ACTIVE_ADMITTED_CAPABILITY` |

## Conclusion

The current baseline satisfies most outcomes through active admitted
mechanisms.

The remaining behavioral-testing and generated-SBOM items are deferred to the
first stage that actually requires them.
