# Stage 8 WP-05 CON-011 Publication Failure Remediation V1

## Failure evidence
The exact executable WP-05 candidate `046d16355040f1598d89b69a929c31aa45043c6f` passed controlled restore/build, Architecture, Security, Stage 7 regression, and Stage 8 WP-01 through WP-04 regression. WP-05 then failed on its first semantic assertion: the Guardian-published `CON-011 RestrictionRecord` did not validate.

## Root cause
`GuardianRestrictionContractPublisher` emitted `RestrictionRecord.Result = ACTIVE`.

The canonical `CON-011` validator permits only `IMPOSED` or `REJECTED` and requires a valid protective record to be `IMPOSED`.

Therefore the producer violated the already-governed contract vocabulary. The validator was correct and was not weakened.

## Remediation
The publisher now emits `Result = IMPOSED` for a successfully validated Guardian protective restriction.

No change was made to:
- `CON-011` schema or validator semantics;
- Guardian restriction severity mapping;
- Authority enforcement semantics;
- Lifecycle protective enforcement semantics;
- Stage 9 recovery/release boundary.

## Boundary
`ACTIVE` remains an internal Guardian restriction status concept where applicable. `IMPOSED` is the canonical `CON-011` contract result for a restriction that has been validly imposed.

`INTERNAL_STATUS != CONTRACT_RESULT`

## Required retest
The complete WP-05 exact executable validation shall be rerun on the replacement candidate. WP-05 is not a technical checkpoint until the full chain passes.
