# 08 — Owner Acceptance and Final Planning Amendment

**Date:** 2026-08-17  
**Branch:** `web-development`  
**Scope:** Shared Falcon Web Master Plan V2 planning baseline  
**Decision:** `OWNER_ACCEPTED_WITH_FINAL_INCORPORATED_AMENDMENTS`

# 1. Owner acceptance

The Project Owner explicitly accepted the Shared Falcon Web Master Plan V2 as a whole on 2026-08-17, subject only to the two amendments recorded in this document.

All other Master Plan V2 scope, sequencing, boundaries, quality gates, Red-Team requirements and implementation direction are accepted without further planning changes at this checkpoint.

This acceptance closes the planning-approval gate only. It does not close the future implementation baseline, does not authorize production deployment, and does not create cross-workstream authority.

```text
MASTER_PLAN_V2 = OWNER_ACCEPTED
IMPLEMENTATION_BASELINE = NOT_YET_OWNER_CLOSED
PRODUCTION_DEPLOYMENT = NOT_AUTHORIZED_BY_THIS_ACCEPTANCE
```

# 2. Amendment A — Owner landing page after Sign In

The previous planning direction that routed the Project Owner directly from successful Sign In to Falcon Command Center is superseded.

The accepted Owner experience is now:

```text
OWNER SIGN IN
→ AUTHORITATIVE FALCON IDENTITY / SESSION / OWNER ROLE
→ FALCON OWNER HOME
→ OWNER CHOOSES DESTINATION
```

`FALCON OWNER HOME` is the Owner's Falcon-wide launch surface. It presents the Owner-accessible top-level Falcon destinations as clear launch cards or equivalent navigation surfaces.

Initial direction includes:

- **Falcon Command Center** — Falcon-wide operational/governance/control and attention surface;
- **FSATS** — direct entry to the FSATS product experience;
- **future Falcon top-level systems/applications** — added to this Owner Home as they become real and governed;
- other separately governed Owner-accessible Falcon destinations where appropriate.

The Owner is not forced through Falcon Command Center merely to enter FSATS or another Falcon system.

Falcon Command Center may appear beside Falcon systems in the Owner Home, but it must remain visibly identified as Falcon's management/governance surface rather than being falsely represented as a business-domain system equivalent to FSATS.

The Owner Home should also surface a compact Falcon-wide attention indicator so urgent Owner attention is not hidden merely because the Owner chooses to enter FSATS instead of Command Center.

```text
OWNER_HOME = NAVIGATION_AND_ATTENTION_SURFACE
OWNER_HOME != AUTHORITY_ENGINE
OWNER_HOME != COMMAND_CENTER
OWNER_HOME != FSATS
NAVIGATION_VISIBILITY != ACTION_AUTHORIZATION
```

This amendment prospectively supersedes any older Master Plan / Ideas wording that says:

```text
PROJECT OWNER -> FALCON COMMAND CENTER DIRECTLY
```

for post-authentication landing behavior.

# 3. Amendment B — Permanent Project Owner FSATS feature access

The Project Owner shall have permanent access to all current and future customer-facing FSATS VIP feature capabilities that are available in the deployed FSATS product experience.

This access is not a commercial VIP subscription and is not subject to:

- one-month VIP trial expiry;
- seven-day downgrade warning;
- downgrade to Standard;
- ordinary customer upgrade prompts;
- ordinary Standard feature locks that would remove VIP product capability from the Project Owner.

The Project Owner's access is an Owner-specific Falcon/FSATS entitlement relationship, not a fabricated paid subscription record.

```text
PROJECT_OWNER_FSATS_FEATURE_ACCESS = FULL_VIP_FEATURE_SET_OR_GREATER
PROJECT_OWNER_ACCESS != COMMERCIAL_VIP_SUBSCRIPTION
OWNER_ACCESS != TRIAL
OWNER_ACCESS != STANDARD_DOWNGRADE_TARGET
```

Important authority boundary:

Full FSATS product-feature access does not by itself grant or bypass separately governed Trading, broker, execution, Kill, Foundation, secret, deployment or business-action authority.

```text
FEATURE_ACCESS != ACTION_AUTHORIZATION
FEATURE_ACCESS != TRADING_EXECUTION_AUTHORITY
FEATURE_ACCESS != BROKER_AUTHORITY
FEATURE_ACCESS != FOUNDATION_AUTHORITY
FEATURE_ACCESS != KILL_AUTHORITY
```

The eventual authoritative entitlement/runtime contract shall encode this Owner-specific access without pretending that the Owner is an ordinary paying VIP customer.

# 4. Master Plan reconciliation effect

The following Master Plan areas are prospectively controlled by this amendment where older wording conflicts:

- Product/Site Experience: Sign In routing, Owner Home, My Applications/Owner navigation, Command Center relationship;
- Subscription/Standard/VIP direction: Project Owner exception and permanent full FSATS product-feature access;
- WP-05 authentication/routing UX;
- WP-06 account/subscription/tier UX;
- WP-17 Owner Command Center relationship to the Falcon Owner Home;
- final browser/role/entitlement/adversarial testing.

# 5. Required implementation tests added by this amendment

The final implementation shall prove at minimum:

1. authenticated Project Owner lands on Falcon Owner Home, not forced directly into Command Center;
2. Owner can enter FSATS directly from Owner Home;
3. Owner can enter Command Center directly from Owner Home;
4. future top-level Falcon destinations can be added without redesigning the authentication journey;
5. urgent Owner attention remains visible from Owner Home;
6. Owner FSATS access is not downgraded by trial/subscription expiry logic;
7. Owner receives all governed customer-facing FSATS VIP product features;
8. Owner feature access does not silently create execution/business authority;
9. non-Owner customers remain governed by their actual subscription/entitlement state;
10. role or display-label spoofing cannot obtain Owner feature access.

# 6. Planning status after amendment

The planning Red Team previously completed with no open findings. These two amendments were reviewed for architectural consistency against the current Shared Web boundary and do not change cross-workstream ownership.

They alter Owner navigation and Owner-specific FSATS product entitlement semantics only.

```text
OPEN_PLANNING_CRITICAL = 0
OPEN_PLANNING_HIGH = 0
OWNER_REQUESTED_AMENDMENTS = INCORPORATED
MASTER_PLAN_V2_STATUS = OWNER_ACCEPTED
NEXT_PHASE = WP-01 CURRENT SOURCE_AND_CONTRACT_BASELINE_INVENTORY
```
