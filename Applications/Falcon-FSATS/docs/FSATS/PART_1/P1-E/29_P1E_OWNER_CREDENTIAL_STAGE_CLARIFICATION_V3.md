# P1-E — Owner Credential-Stage Clarification V3

**Status:** `OWNER-DIRECTED SEMANTIC CLARIFICATION / CONTROLS PRIOR CREDENTIAL WORDING PROSPECTIVELY`  
**Implementation Authority:** `NOT_GRANTED`  
**Runtime Authority:** `NOT_GRANTED`

## Reason

After the reviewed P1-E V2 target, the Project Owner clarified the user credential boundary through the FCR-0081 reconciliation:

```text
FSATS_SUBSCRIPTION != AUTOMATED_TRADING
ADVISORY_USE != EXECUTION_AUTHORITY
ADVISORY_USE != USER_BROKER_CREDENTIAL_REQUIREMENT
USER_BROKER_API_CREDENTIALS = AUTOMATED_TRADING_ENABLEMENT_REQUIREMENT_WHEN_APPLICABLE
```

This clarification is controlling for P1-E and shall not be interpreted as requiring user-supplied provider/broker credentials for ordinary advisory/non-execution use.

## Corrected Credential Semantics

1. Bare FSATS subscription SHALL NOT require user-supplied broker/API credentials.
2. Advisory, consultation, analysis, recommendations, information/results viewing and other non-execution FSATS use SHALL NOT require user-supplied broker/API credentials.
3. User-supplied broker/API credentials SHALL be requested only when the user explicitly enables an automated-trading/execution capability that requires them.
4. Trading Execution owns the Application semantic consumption of governed user broker-execution credential references for automated execution.
5. FSAPMA may consume governed provider/service credential references needed for operational-data acquisition, but this does NOT establish a general requirement that an ordinary advisory/non-execution user personally supply FSAPMA provider credentials.
6. Service/provider credentials, user broker credentials and any future user provider credentials are separate credential classes and SHALL NOT be merged merely because a vendor, account or API technology overlaps.
7. Secret/key/token bytes SHALL NOT be embedded in the Application Manifest, ordinary logs, reusable Shared Web state or browser-visible state.
8. The Manifest may declare only the semantic credential-reference dependency, consuming capability, scope, lifecycle requirement and fail-closed/degraded behavior.
9. User input delivery does not create credential validity, connectivity authority, execution authority or runtime authority.
10. Exact secure storage, transfer, validation and Foundation runtime egress realization remain separately governed and outside this P1-E design closure.

## Compatibility Effect

This clarification narrows the prior P1-E V2 credential wording. It does not alter:

- the five-Application FSATS topology;
- APP-RSC ownership/scope;
- P1-D primitive ownership;
- Safety Continuity V2;
- AI Repair / Controlled Recovery V3;
- Foundation ownership of generic security/credential infrastructure;
- implementation/runtime authority state.

Because it is a semantic clarification after the V2 review, P1-E requires a new exact semantic freeze and fresh Architecture/Consistency + Red-Team/integration verification before closure.