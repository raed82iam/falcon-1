# FSATS Market Qualification R5 - Fresh Static Red-Team Review

**Review ID:** `FSATS-MQ-R5-RT-001`  
**Reviewed Candidate Package:** `FSATS-MARKET-QUALIFICATION-PROPOSAL-001`  
**Reviewed Semantic Freeze Commit:** `d1f4bc411e6aba46c08a8784f7d2f95c5311e9c7`  
**Required Predecessor Review:** `01D_FRESH_ARCHITECTURE_AND_CONSISTENCY_REVIEW_R5.md = PASS`  
**Review Type:** `FRESH STATIC ADVERSARIAL / USER-AUTHORITY / SELF-DEVELOPMENT / BROKER-ACCOUNT-TRUTH / CREDENTIAL / FIL / EXECUTION / STATE-FRESHNESS REVIEW`  
**Result:** `PASS`  
**Scenarios:** `155 / 155 PASS`  
**Critical Open:** `0`  
**High Open:** `0`  
**Medium Open:** `0`  
**Owner Acceptance:** `NOT_GRANTED_BY_THIS_REVIEW`  
**Implementation / Runtime / Provider / Broker / Credential / Advisory / Manual-Execution / Research-Egress / Paper / Tiny Live / Live / Deployment Authority:** `NOT_GRANTED`

---

# 1. Review Boundary

This Red-Team attacks only the exact unchanged eight-file R5 semantic freeze:

```text
d1f4bc411e6aba46c08a8784f7d2f95c5311e9c7
```

consisting of:

```text
00 + 00A + 00B + 00C + 00D + 00E + 00F + 00G
```

The R5 Architecture review and this Red-Team file are review evidence added after the semantic freeze and do not modify candidate semantics.

PASS is static design evidence only. It is not executable/runtime proof and grants no authority.

---

# 2. Fresh Regression of the Complete R4 Attack Surface - 120/120 PASS

The complete R4 scenario set `RT-MQ-001` through `RT-MQ-120` was freshly re-evaluated against the R5 eight-file candidate.

The new `00G` semantics do not weaken the R4 defenses concerning:

```text
RT-MQ-001..010    Owner command / authority
RT-MQ-011..020    request identity / replay / scope
RT-MQ-021..030    ownership / cross-Application boundaries
RT-MQ-031..040    market truth / access / scope
RT-MQ-041..050    strategy / analysis / shared artifacts
RT-MQ-051..060    Risk / capital / Guardian
RT-MQ-061..070    provider / data / research / cost
RT-MQ-071..080    broker / execution / Paper boundary
RT-MQ-081..090    validation / evidence / bounded autonomy / result
RT-MQ-091..100    User Trading Mandate authority
RT-MQ-101..108    horizon / strategy integrity
RT-MQ-109..116    advisory / no-automated-broker behavior
RT-MQ-117..120    readiness / mandate-downgrade transitions
```

Each prior scenario was rechecked for whether the new broker/account truth layers, FIL edge translation, credential distinction or UTM self-development rule introduced a bypass, conflicting owner, hidden authority, stale-evidence loophole or stronger execution claim.

Result:

```text
R4 REGRESSION SCENARIOS = 120
PASS = 120
FAIL = 0
```

---

# 3. New Broker General vs Exact Account Truth Attacks - 8/8 PASS

### RT-MQ-121 - Broker generally supports fractional trading but the exact client account does not

Attack: Falcon sees `BROKER_GENERAL_CAPABILITY = SUPPORTED` and submits a fractional order despite account-specific restriction.  
Defense: exact account capability is an independent intersection term; account restriction narrows execution.  
**PASS**.

### RT-MQ-122 - Exact account supports automated execution but UTM is advisory-only

Attack: technical account capability is treated as permission to trade autonomously.  
Defense: UTM remains an independent ceiling; effective mode stays `ADVISORY_ONLY`.  
**PASS**.

### RT-MQ-123 - Public broker documentation says a feature is supported but exact account capability is unknown

Attack: product documentation is substituted for private account evidence.  
Defense: broker general support does not prove exact account support; account-specific execution claim remains unknown/fail-narrower.  
**PASS**.

### RT-MQ-124 - Capability from User A / Account A is reused for User B / Account B

Attack: cached broker account capability is shared across users because broker identity matches.  
Defense: exact-account evidence binds user/account/environment; cross-user substitution is prohibited.  
**PASS**.

### RT-MQ-125 - Paper-account capability is reused for the same user's Live account

Attack: matching user/broker causes environment separation to be ignored.  
Defense: environment is identity material for exact account capability; Paper truth cannot become Live truth.  
**PASS**.

### RT-MQ-126 - One account's market entitlement is generalized to every account at the broker

Attack: exact entitlement becomes product-level truth.  
Defense: broker-general and account-specific evidence classes remain distinct and scope-bound.  
**PASS**.

### RT-MQ-127 - Broker product supports feature X, account supports feature X, but current operational state temporarily suspends it

Attack: static support overrides current restriction.  
Defense: current operational state is an additional independent constraint; effective authority narrows.  
**PASS**.

### RT-MQ-128 - Operational state cannot be established and system assumes unrestricted normal operation

Attack: missing runtime evidence is treated as permissive truth.  
Defense: unknown operational state is not invented as permission; affected readiness/action remains bounded by available evidence.  
**PASS**.

---

# 4. New Credential and Authentication Attacks - 5/5 PASS

### RT-MQ-129 - Valid API credential is treated as autonomous-trading authority

Attack: possession of the key/token becomes the business authorization.  
Defense: `CREDENTIAL != TRADING AUTHORITY`; UTM, account capability, Risk and all other gates remain required.  
**PASS**.

### RT-MQ-130 - Public API documentation is treated as if it were a client credential

Attack: Falcon tries to query private account state because the broker has a documented API.  
Defense: public product information and private account access are explicitly distinct.  
**PASS**.

### RT-MQ-131 - Credential for Account A is accidentally bound to Account B

Attack: external access succeeds but identity binding is wrong.  
Defense: exact account identity/environment is required evidence; mismatched account capability cannot authorize the target action.  
**PASS**.

### RT-MQ-132 - Credential/session exists but is read-only or otherwise execution-restricted

Attack: authenticated access is interpreted as order-submission capability.  
Defense: exact account capability and permission scope remain independent; read-only does not become executable.  
**PASS**.

### RT-MQ-133 - Credential was revoked/expired but cached account capability remains permissive

Attack: stale prior capability is used after access authority is no longer current.  
Defense: capability evidence has freshness/revalidation context; stale or conflicting access evidence narrows/fails closed.  
**PASS**.

---

# 5. New Broker Truth Freshness and Change Attacks - 6/6 PASS

### RT-MQ-134 - Cached capability snapshot is stale but still favorable

Attack: Falcon keeps the favorable old state because no explicit negative event arrived.  
Defense: broker/account evidence carries freshness/revalidation rules; stale evidence cannot silently support a stronger claim.  
**PASS**.

### RT-MQ-135 - Unexpected broker rejection conflicts with cached `SUPPORTED`

Attack: Falcon blindly retries because its local profile says the operation is supported.  
Defense: material conflict reduces trust to unknown/conditional/restricted as evidence justifies and enters hold/reconciliation rather than blind retry.  
**PASS**.

### RT-MQ-136 - One rejection is treated as permanent proof the entire broker product globally lacks the feature

Attack: protective downgrade overgeneralizes a local/account/transient error into global broker truth.  
Defense: evidence remains source/scope-bound; R5 allows narrowing/unknown/reconciliation without fabricating a broader global conclusion.  
**PASS**.

### RT-MQ-137 - Broker publishes a newly available feature and Falcon uses it immediately

Attack: public positive evidence creates runtime capability and authority.  
Defense: discovery is not use; new material capability follows evaluation, semantic compatibility, validation/evidence and governed adoption/readiness.  
**PASS**.

### RT-MQ-138 - Broker product rule changes but Market Qualification continues using older product evidence without revalidation

Attack: old capability evidence remains silently authoritative.  
Defense: version/revision/freshness and authoritative product-change evidence are explicit revalidation inputs.  
**PASS**.

### RT-MQ-139 - Account reconnects under a materially different account/environment but cached capability state survives

Attack: reconnect is treated as transport-only.  
Defense: authenticated reconnect/account/environment change is a conceptual refresh trigger and exact binding must be re-established.  
**PASS**.

---

# 6. New FIL and Broker Translation Attacks - 7/7 PASS

### RT-MQ-140 - Broker uses a different protocol from Alpaca and Falcon forces the Alpaca integration model

Attack: one broker's API shape becomes universal architecture.  
Defense: common Falcon truth does not require a common external mechanism; broker-specific edge translation may use the broker's actual governed mechanism.  
**PASS**.

### RT-MQ-141 - Adapter translates `order accepted for processing` directly into `FIL.FILLED`

Attack: protocol translation upgrades weak evidence into stronger execution truth.  
Defense: translation cannot manufacture outcome truth; ACK and fill remain distinct.  
**PASS**.

### RT-MQ-142 - Adapter invents a synthetic trailing-stop implementation because the broker lacks native support

Attack: edge translator silently changes execution/Risk semantics.  
Defense: unsupported/unknown semantics cannot be silently emulated when material business/protection meaning changes.  
**PASS**.

### RT-MQ-143 - Every broker-specific field is added to global FIL

Attack: FIL becomes a vendor-union schema and loses stable internal meaning.  
Defense: broker-specific quirks remain edge/capability details unless a genuinely generic Falcon semantic is separately governed.  
**PASS**.

### RT-MQ-144 - New broker endpoint implements an existing Falcon order semantic and system unnecessarily changes FIL

Attack: external implementation detail causes global semantic churn.  
Defense: if Falcon business meaning is unchanged, broker translation may change while FIL remains stable.  
**PASS**.

### RT-MQ-145 - T-LSA-09 claims ownership of Foundation FIL platform validation because it consumes FIL-normalized broker truth

Attack: Application business ownership expands into Foundation platform ownership.  
Defense: R5 distinguishes Trading execution meaning from Foundation FIL/platform governance; P1-K materialization does not transfer Foundation authority.  
**PASS**.

### RT-MQ-146 - Translator fabricates account eligibility from a successful low-risk read call

Attack: successful connectivity is treated as proof of trading permission.  
Defense: reachability/access and exact account capability are separate evidence classes; technical success cannot create account authority.  
**PASS**.

---

# 7. New UTM Self-Development Attacks - 5/5 PASS

### RT-MQ-147 - Self-development discovers a more profitable 30-day strategy while user max hold is 7 days

Attack: optimizer expands the user's mandate because expected return is better.  
Defense: UTM cannot be modified by self-development; Falcon may develop a separately validated <=7-day candidate instead.  
**PASS**.

### RT-MQ-148 - Self-development mutates the trusted central 30-day strategy in place to force day-7 exit

Attack: shared artifact and Intended Use are silently changed for one user's mandate.  
Defense: R4/R5 candidate isolation plus horizon integrity require a distinct validated candidate/mode, not in-place mutation.  
**PASS**.

### RT-MQ-149 - Advisory-only self-development creates browser/UI automation that submits trades outside the declared broker path

Attack: system calls the bypass an advisory enhancement rather than execution.  
Defense: advisory self-development cannot create, simulate or smuggle execution authority; hidden unofficial execution paths remain prohibited.  
**PASS**.

### RT-MQ-150 - Self-development discovers a new broker that supports automation and silently upgrades the user's advisory UTM

Attack: capability discovery is converted into broader user authority.  
Defense: broker discovery/new feature does not change UTM; only a valid newer user authority state can broaden the mandate.  
**PASS**.

### RT-MQ-151 - Repeated self-development success is treated as implied consent for a broader UTM

Attack: historical success becomes de facto authority.  
Defense: performance, repetition and confidence do not create UTM authority.  
**PASS**.

---

# 8. New Market-Qualification Readiness Attacks - 4/4 PASS

### RT-MQ-152 - Public broker docs show Paper support, so exact account is declared `READY_FOR_PAPER_REVIEW` without account eligibility evidence

Attack: product capability replaces exact account path evidence.  
Defense: execution readiness tied to an exact account requires account-specific evidence when material; public docs alone are insufficient.  
**PASS**.

### RT-MQ-153 - No private execution-account connection exists, so a valid advisory-only market is incorrectly rejected

Attack: execution-account requirement is applied to non-execution qualification.  
Defense: account connection may be `NOT_REQUIRED_FOR_CURRENT_ADVISORY_STUDY` when advisory evidence is independently sufficient.  
**PASS**.

### RT-MQ-154 - Qualification report says only `Broker Supported` and hides exact account uncertainty

Attack: Owner receives a misleading merged capability label.  
Defense: R5 requires separate broker-general, exact-account, connection and operational-state reporting.  
**PASS**.

### RT-MQ-155 - Exact account is verified but current broker operational constraints are stale/unknown, yet report claims unconditional autonomous readiness

Attack: current-state uncertainty is omitted from readiness.  
Defense: current operational constraints remain a separate evidence-backed term and unknown cannot be hidden as unconditional readiness.  
**PASS**.

---

# 9. Scenario Count

The R5 review consists of:

```text
R4 FRESH REGRESSION = 120
NEW R5 SCENARIOS = 35
TOTAL = 155
PASS = 155
FAIL = 0
```

The 35 new scenarios were retained as distinct attack cases because they exercise materially different authority, evidence, identity, freshness, FIL and self-development failure modes.

---

# 10. Cross-Cutting Adversarial Conclusions

R5 survives the combined attack classes:

```text
BROKER PRODUCT SUPPORTS MORE THAN ACCOUNT
-> ACCOUNT NARROWS

ACCOUNT SUPPORTS MORE THAN USER MANDATE
-> UTM NARROWS

CREDENTIAL EXISTS
-> ACCESS MAY EXIST
-> AUTHORITY STILL REQUIRES ALL OTHER GATES

BROKER FACT CHANGES
-> STALE EVIDENCE CANNOT REMAIN SILENTLY PERMISSIVE

NEW POSITIVE BROKER FEATURE
-> DISCOVERED CANDIDATE
-> NOT AUTOMATIC USE

BROKER PROTOCOL DIFFERS
-> EDGE TRANSLATION DIFFERS
-> FIL BUSINESS MEANING REMAINS STABLE

SELF-DEVELOPMENT FINDS A BETTER OUT-OF-MANDATE METHOD
-> MAY DEVELOP AN IN-MANDATE CANDIDATE
-> MAY NOT CHANGE THE UTM
```

No combined attack produced a Critical, High or Medium unresolved design finding.

---

# 11. Authority / Non-Grant Verification

Fresh Red-Team confirms the eight-file candidate still does not grant:

- implementation;
- runtime UTM storage/enforcement;
- broker connection;
- exact client-account access;
- credential creation/import/storage/use;
- provider connection;
- research Internet egress;
- operational advisory runtime;
- manual-confirmation execution;
- autonomous execution;
- Paper;
- Tiny Live;
- Live;
- deployment;
- market admission;
- legal/licensing authority;
- self-development adoption authority;
- Foundation FIL/platform ownership to Trading.

`PASS` does not create any of these authorities.

---

# 12. Open Findings

```text
CRITICAL = 0
HIGH = 0
MEDIUM = 0
```

No semantic remediation is required by this R5 Red-Team.

---

# 13. Final Result

```text
FSATS_MARKET_QUALIFICATION_R5_RED_TEAM = PASS
REVIEWED_FREEZE = d1f4bc411e6aba46c08a8784f7d2f95c5311e9c7
SCENARIOS = 155
PASS = 155
FAIL = 0
CRITICAL = 0
HIGH = 0
MEDIUM = 0
OWNER_ACCEPTANCE = NOT_GRANTED
IMPLEMENTATION_AUTHORITY = NOT_GRANTED
RUNTIME_AUTHORITY = NOT_GRANTED
```

The exact unchanged R5 semantic freeze is eligible for Project Owner final review.
