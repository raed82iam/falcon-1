# FSATS Market Qualification Candidate — Bounded Autonomy, Resource, Cost and Research-Security Hardening

**Package:** `FSATS-MARKET-QUALIFICATION-PROPOSAL-001`  
**Applies To:** `00 + 00A + 00B`  
**Decision Type:** `PRE-RED-TEAM THREAT-MODEL HARDENING`  
**Status:** `CONTROLLING CANDIDATE HARDENING / NOT_OWNER_ACCEPTED / NOT_CLOSED`  
**Branch:** `application-development`  
**Implementation / Runtime / Research-Egress / Paper / Tiny Live / Live / Deployment Authority:** `NOT_GRANTED`

---

# 1. Purpose

This hardening closes bounded-autonomy and resource/security failure modes identified after the R1 Architecture/Consistency review but before formal Red-Team execution.

Because this file changes the candidate semantic set after the R1 reviewed freeze, the R1 Architecture/Consistency PASS is historical for its exact freeze and SHALL NOT be presented as current PASS evidence for the expanded candidate.

A new semantic freeze and fresh Architecture/Consistency + fresh Red-Team are required.

---

# 2. Qualification Must Be Bounded and Convergent

A Market Qualification Job SHALL NOT run forever merely because the Awareness entities can continue generating ideas.

The future governed qualification plan shall include bounded controls such as, as applicable:

- maximum concurrent candidate experiments;
- maximum active research branches;
- resource/capacity ceiling;
- cost ceiling;
- candidate-generation rate limits;
- retry/retest limits or evidence-based convergence rules;
- duplicate candidate/gap deduplication;
- cooldown/research refresh intervals;
- stopping criteria;
- hold criteria;
- cancellation authority;
- owner-requested stop/pause;
- stale-job expiry/revalidation;
- preservation of the best current evidence when work is paused.

The exact numeric values are future governed policy/configuration and SHALL NOT be invented here.

Mandatory invariant:

```text
MORE IDEAS != PERMISSION FOR UNBOUNDED COMPUTE / RESEARCH / CANDIDATE GENERATION
```

If evidence is not converging inside the authorized envelope, the correct result may be:

```text
HOLD_RETEST
INSUFFICIENT_EVIDENCE
RESOURCE_LIMIT_REACHED
OWNER_DECISION_REQUIRED
```

rather than an infinite loop.

---

# 3. Target-Market Scope Lock

The qualification mandate is bound to the exact `TargetMarketIdentity`, `TargetAssetClass`, Intended Use and explicitly authorized related scope.

Awareness may discover that another market, asset class, instrument family or leverage model would be useful, but it SHALL NOT silently add it to the active qualification mandate.

Conceptually:

```text
DISCOVERED ADJACENT OPPORTUNITY
-> RECORD / RECOMMEND
-> SEPARATE OWNER / GOVERNANCE SCOPE DECISION WHEN MATERIAL
```

not:

```text
MARKET X REQUEST
-> AUTONOMOUSLY EXPAND TO MARKET Y + OPTIONS + FUTURES + LEVERAGE
```

A necessary narrower sub-scope inside Market X may be investigated only when it remains within the original mandate and does not create a new material authority class.

---

# 4. Cost Ceiling Cannot Self-Expand

`MarketQualificationRequest.CostCeiling` is an authority/policy constraint, not a target for Awareness to optimize around by spending more.

The qualification process SHALL NOT autonomously:

- buy a provider plan;
- accept a paid upgrade;
- add a paid data product;
- create a paid cloud/service commitment;
- change `CostCeiling`;
- convert a free trial into a paid subscription;
- treat future expected profit as authority to spend.

If an exact required capability cannot be satisfied inside the current cost ceiling:

```text
COST_CEILING_BLOCKED
-> REPORT EXACT MISSING CAPABILITY / BENEFIT / COST EVIDENCE
-> REQUEST SEPARATE OWNER DECISION IF A PAID OPTION IS WORTH CONSIDERING
```

For the current Owner-only free-first evaluation profile, a zero-cost ceiling remains zero unless explicitly changed by the Owner through a separate governed decision.

---

# 5. Qualification Mandate Does Not Manufacture Tool / Code-Write Authority

`ADD MARKET X` authorizes the bounded qualification objective, but each actor may execute only actions for which it already has valid permissions and tool/write authority.

Mandatory distinction:

```text
OWNER AUTHORIZES MARKET-QUALIFICATION OBJECTIVE
!=
EVERY AI AUTOMATICALLY RECEIVES CODE-WRITE / INTERNET / SECRET / CREDENTIAL / DEPLOYMENT PERMISSION
```

If candidate engineering requires a permission/tool that is not already governed and authorized, the workflow SHALL:

- design/research within current authority where possible;
- report the missing capability/permission;
- hold the affected candidate action;
- request the appropriate separately governed authorization when required.

No Awareness entity may self-grant tools or write access.

---

# 6. External Research Content Is Untrusted Input

When future research egress is authorized, external content used during market qualification SHALL remain untrusted research evidence until it passes the applicable governed research boundary.

The process shall preserve as applicable:

- source/provenance;
- retrieval time/version where knowable;
- destination/source policy;
- quarantine/sandbox handling;
- content/security/integrity inspection;
- prompt-injection / instruction-confusion resistance where AI consumes external text;
- separation of external claims from accepted internal truth;
- cross-checking of material market/provider/broker rules against authoritative/primary evidence where available;
- explicit `UNKNOWN` when evidence cannot be established.

External webpages/documents SHALL NOT be allowed to issue Falcon instructions, widen authority, change goals, alter cost ceilings, supply credentials, or bypass Owner/governance.

```text
EXTERNAL CONTENT = EVIDENCE INPUT
EXTERNAL CONTENT != AUTHORITY
```

---

# 7. Qualification Work Cannot Starve Protected Operation

Non-Live research/simulation/qualification work SHALL remain subordinate to governed resource protection and higher-priority active obligations.

The workflow SHALL NOT assume that a market qualification job has priority over:

- capital protection;
- Guardian/crisis obligations;
- open-position safety/reconciliation;
- required live operational data paths;
- security/integrity containment;
- minimum-safe Application operation;
- Foundation protected resource floors.

When resource pressure requires it, eligible qualification work may be throttled, paused, shed, checkpointed or resumed according to the then-current governed resource model.

Pausing qualification SHALL NOT corrupt or rewrite already accepted evidence.

```text
MARKET QUALIFICATION IMPORTANCE != PROTECTED LIVE/SAFETY PRIORITY OVERRIDE
```

This record does not choose a Foundation resource algorithm and does not accept the unaccepted FSARM candidate by implication.

---

# 8. Candidate Explosion / Duplicate Research Control

Repeated failures SHALL NOT create unlimited equivalent candidate branches.

Future implementation shall support semantic deduplication/fingerprinting sufficient to recognize materially equivalent:

- market-rule questions;
- provider capability gaps;
- strategy adaptation hypotheses;
- Risk failure modes;
- broker/execution gaps;
- scenario families.

A repeated observation may strengthen recurrence evidence without spawning an unbounded new research tree.

---

# 9. Owner Stop / Pause / Narrow Authority

The Owner must be able to stop, pause or narrow the qualification mandate without that request being treated as a suggestion.

The exact runtime enforcement path depends on the future governed Owner command/control boundary, but the Application semantics shall support at least conceptually:

```text
CONTINUE
PAUSE
CANCEL
NARROW_SCOPE
REQUEST_MORE_EVIDENCE
```

A cancelled qualification retains attributable historical evidence and does not silently resume as an active mandate.

---

# 10. New Reviewed-Candidate Composition

The new semantic set requiring fresh review is:

```text
00_GOVERNED_MARKET_QUALIFICATION_AND_EXPANSION_LIFECYCLE_CANDIDATE.md
+
00A_PRE_REVIEW_AUTHORITY_AND_OWNER_COMMAND_RUNTIME_HARDENING.md
+
00B_PRE_REVIEW_MARKET_ACCESS_SCOPE_AND_VALUE_COMPLETENESS_HARDENING.md
+
00C_PRE_RED_TEAM_BOUNDED_AUTONOMY_RESOURCE_COST_AND_RESEARCH_SECURITY_HARDENING.md
```

The prior R1 review remains historical evidence only for commit `7c1f3b30711d449d13c98436b5775a909c927200`.

Fresh Architecture/Consistency and fresh Red-Team must bind to the exact new freeze containing `00/00A/00B/00C` unchanged.

---

# 11. Non-Grant

This hardening does not grant implementation, runtime, research egress, code-write permission, tools, secrets, credentials, provider/broker connectivity, spending authority, Paper, Tiny Live, Live, deployment or market admission.
