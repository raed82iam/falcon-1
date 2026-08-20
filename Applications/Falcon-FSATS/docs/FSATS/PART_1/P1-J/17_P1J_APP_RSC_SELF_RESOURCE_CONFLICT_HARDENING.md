# P1-J — APP-RSC Self-Resource and Conflict-of-Interest Hardening

**Status:** `PRE_REVIEW_HARDENING / CONTROLLING_DELTA`  
**Implementation Authority:** `NOT_GRANTED`

APP-RSC is itself an independently admitted Falcon Application and therefore SHALL have its own CON-023 Resource Profile, Foundation grant/allocation identity, minimum-safe/degraded behavior and resource evidence.

Its own coordination overhead SHALL NOT be hidden outside the FSATS resource picture.

Mandatory rules:

```text
APP_RSC_COORDINATION_COST != FREE_RESOURCE
APP_RSC_SELF_REPORTED_NEED != SELF_GRANTED_RESOURCE
APP_RSC_SELF_PRIORITY != FOUNDATION_PRIORITY
```

APP-RSC may account for its own current consumption, coordination workload, minimum-safe requirement, pressure and restoration need when calculating the complete FSATS residual resource requirement. However:

- its own Foundation-authoritative grant/ceiling/floor remains externally governed by Foundation;
- APP-RSC SHALL NOT unilaterally raise its own protected minimum or priority;
- APP-RSC SHALL NOT reclaim sibling resources merely to improve its own convenience/throughput where a safer degraded coordination mode exists;
- any self-impacting redistribution decision shall remain attributable and independently challengeable;
- loss of APP-RSC capacity should first degrade nonessential coordination analytics/optimization before violating required fencing, evidence, current-envelope tracking or safe handoff behavior;
- if APP-RSC cannot maintain its own minimum-safe coordination function, new redistribution decisions fail closed and constituent Applications remain within their current valid resource truth.

Anti-gaming/verifier coverage SHALL include APP-RSC falsely elevating its own urgency, hiding its own consumption, transferring its own pressure to a sibling, or requesting Foundation capacity without including its own coordination overhead transparently.
