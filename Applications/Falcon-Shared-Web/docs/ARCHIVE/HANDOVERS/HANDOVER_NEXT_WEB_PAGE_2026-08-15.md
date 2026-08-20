# HANDOVER — Shared Falcon Web Application

**Date:** `2026-08-15`  
**Repository:** `raed82iam/Falcon`  
**Writable branch:** `web-development`  
**Writable subtree:** `applications/shared/web/**`

This handover is for the next ChatGPT/Web workstream page. Treat it as direct continuity of the current Shared Falcon Web workstream, not as a redesign or a new project.

---

# 1. MANDATORY STARTUP INSTRUCTION FOR THE NEXT PAGE

Before answering the Project Owner or doing any analysis/design/code/review, do all of the following in this exact spirit:

1. Read `applications/shared/web/WORKSTREAM_RULES.md` **completely, from first line to last line**.
2. Read `applications/shared/web/README.md` completely.
3. Read `applications/README.md` completely.
4. Read the current Falcon Vision completely.
5. Read the current Falcon Constitution completely.
6. Locate and read the current effective `APP-001`, `CON-023`, `ADR-I012`, and `ADR-I015` completely when they are applicable to the work being continued.
7. Read **every file under `applications/shared/web/**` completely**, not only this handover and not only filenames that look relevant. This includes planning, Ideas, architecture, implementation, tests, source and checkpoint records.
8. Read the current Application/FSATS references required to understand any contract being consumed. Do not modify them.
9. Read the current Foundation references required to understand any Foundation contract being consumed. Do not modify them.
10. Perform a live repository-wide FCR read before the first response.

## Mandatory FCR instruction

Do **not** read only the FCRs that say `Waiting On: WEB`.

Read the entire repository-wide FCR registry and every issue whose canonical title is `[FCR-xxxx] ...`, including:

- `Waiting On: WEB`
- `Waiting On: APPLICATION`
- `Waiting On: FOUNDATION`
- `Waiting On: NONE`
- closed FCRs when their history affects current semantics

For every open FCR, read:

- the complete current Issue body/header;
- the complete latest relevant comments/evidence;
- the current `Waiting On`;
- `Next Required Action`;
- target Stage/WP/review trigger where present;
- all cross-workstream distinctions and closure conditions.

The Project Owner explicitly requires the next page to understand the whole FCR picture, including FCRs that are not owned by Web, so Web does not accidentally contradict Foundation/Application work in progress.

The shared FCR protocol Issue `#1` must be read fully. The prospective rule currently permits only:

```text
FOUNDATION
APPLICATION
WEB
NONE
```

`Waiting On: OWNER` is prohibited by the current repository-wide Owner clarification. A workstream needing Owner clarification keeps the FCR on its own `Waiting On` and asks the Owner directly.

---

# 2. WRITE / AUTHORITY BOUNDARY

The next Web page may write only:

```text
branch: web-development
path:   applications/shared/web/**
```

Do not write to:

- `foundation-development`
- `application-development`
- `main`
- `reference/fsats-v1.3-scratch`
- Foundation-owned files
- FSATS/Application-owned files
- any path outside `applications/shared/web/**` unless the Project Owner explicitly expands authority.

GitHub Issues are the neutral FCR coordination channel. Reading or commenting in an FCR does not grant cross-workstream source-file authority.

---

# 3. OWNER IMPLEMENTATION AUTHORITY ALREADY GRANTED TO WEB

The Project Owner explicitly authorized complete Shared Web implementation on 2026-08-15:

```text
ابدأ implementation كامل.
```

This authorizes Web-owned implementation inside `applications/shared/web/**`.

It does **not** grant:

- Foundation writes;
- FSATS/Application writes;
- production deployment authority;
- live broker/provider connectivity authority;
- trading/execution authority;
- authority to invent identity/session/MFA truth;
- authority to close implementation-required FCRs before implementation/binding/governed verification is complete.

---

# 4. CURRENT PRODUCT HIERARCHY — DO NOT RE-FLATTTEN IT

The Project Owner clarified the product hierarchy:

```text
FALCON OS
│
├── Falcon Self-Aware Trading System (FSATS)
│   ├── FSATA
│   │   Falcon Self-Aware Trading Application
│   ├── FSAPMA
│   │   Falcon Self-Aware Provider Management Application
│   ├── FTGA
│   │   Falcon Trading Guardian Application
│   ├── FSTSimA
│   │   Falcon Self-Aware Trading Simulation Application
│   └── APP-RSC
│       Falcon Self-Aware Resource Management Application
│
├── future Accounting system/family
├── future Warehouse system/family
└── other future Falcon systems/families
```

Mandatory interpretation:

```text
FALCON_OS != FSATS
FSATS = TRADING_DOMAIN_SYSTEM
FSATS_CHILD_APPLICATION != FALCON_TOP_LEVEL_SYSTEM
NON_TRADING_DOMAIN != FSATS_CHILD
```

Accounting, Warehousing and any future non-Trading domain sit **beside FSATS under Falcon OS**, not inside FSATS.

---

# 5. CURRENT PUBLIC UX DIRECTION

The Project Owner supplied a dark, premium Falcon OS public-interface visual direction. Continue that direction rather than reverting to a generic SaaS layout.

Current page logic:

```text
Falcon Public Home
  -> Falcon OS identity
  -> current top-level Falcon systems
  -> FSATS is current featured Trading system

Applications Page
  -> top-level Falcon systems only

FSATS Public Page
  -> FSATS identity / explanation
  -> five FSATS child Applications
  -> Discover Application / استكشف التطبيق for each
  -> simple animated explainer now
  -> approved short video / richer animated assets can be added later
  -> Sign In
  -> Google sign-in presentation
  -> Microsoft sign-in presentation
  -> Authenticator MFA explanation
  -> Create Falcon Account onboarding presentation
```

Do not advertise future non-Trading systems as operational if they are not authoritatively available.

Do not make regulatory/license claims or show regulatory logos unless actual authority/licensing exists and the Project Owner explicitly authorizes the claim.

---

# 6. NEW USER PHONE REQUIREMENT

Latest Project Owner decision:

Every new customer must provide a phone number during onboarding so Falcon can contact the customer in emergencies and high-priority situations.

Current required distinctions:

```text
NEW_CUSTOMER_ONBOARDING -> PHONE_REQUIRED
PHONE_PURPOSE = EMERGENCY_AND_HIGH_PRIORITY_CONTACT
PHONE_PROVIDED != PHONE_VERIFIED
PHONE_PROVIDED != FALCON_IDENTITY
PHONE_PROVIDED != MFA_FACTOR
PHONE_PROVIDED != BUSINESS_AUTHORITY
PHONE_CONTACT_DATA != BROKER_ACCOUNT_IDENTITY
```

The current UI explicitly explains that the number is for emergency/high-priority contact, is not OTP verification today, and is not an automatic marketing opt-in.

This is customer/contact data owned by the Web/account interaction boundary, not FSATS broker-account business identity.

---

# 7. AUTHENTICATION DIRECTION

Current Project Owner direction includes:

- `Continue with Google`
- `Continue with Microsoft`
- MFA for customers
- MFA for Project Owner
- compatible Authenticator apps such as Google Authenticator / Microsoft Authenticator, without vendor lock-in

Mandatory distinctions:

```text
GOOGLE_ACCOUNT_SIGN_IN != GOOGLE_AUTHENTICATOR_MFA
MICROSOFT_ACCOUNT_SIGN_IN != MICROSOFT_AUTHENTICATOR_MFA
OIDC_AUTHENTICATED != FALCON_IDENTITY
OIDC_AUTHENTICATED != PROJECT_OWNER
MFA_PASSED != BUSINESS_AUTHORITY
EMAIL_MATCH != IDENTITY_LINK_AUTHORIZATION
WEB_SESSION_PRESENTATION != AUTHORITY_ISSUANCE
```

Current `src/auth.js` remains fail-closed and does not invent identity/role.

Live external authentication remains blocked on the authoritative boundary tracked by `FCR-0152`.

---

# 8. OTP IS DEFERRED

The Project Owner decided not to implement OTP delivery now.

Preserved future candidate channels:

1. Telegram
2. WhatsApp
3. SMS

Current state:

```text
OTP_SERVICE = DEFERRED
CURRENT_OTP_PROVIDER = NONE
PHONE_REQUIRED_FOR_CONTACT = YES
PHONE_OTP_VERIFICATION = NOT_ACTIVE
```

Do not silently reactivate OTP. Revisit only after fresh current research, security/privacy/cost comparison, architecture/security/Red Team review and Project Owner approval.

---

# 9. CURRENT IMPLEMENTATION EVIDENCE FROM THE LATEST SESSION

Latest reconciliation commits on `web-development`:

```text
7e81d830d489c28c9ed8afd38dd355441f4ef1ca
  web: align Falcon public catalog with FSATS hierarchy

eee1f432f4f16755cad57329e40d3c2b2594c235
  web: complete FSATS public discovery and phone onboarding presentation

868636f37096b07c61bd10a36962d4b55255b4e5
  web: style FSATS discovery and emergency-contact onboarding

cef36ca568d406020e3d64cb687ab51846c131ef
  web: test FSATS discovery and emergency contact onboarding

9562428ba60d68161fede7b5316a2132a4519c4f
  web: test Falcon-to-FSATS public hierarchy

69cbcc4267d17f9ad20ab57969a18cb46f0db285
  web: record reconciled public auth onboarding checkpoint
```

Primary current checkpoint:

`applications/shared/web/docs/CURRENT_PUBLIC_AUTH_ONBOARDING_CHECKPOINT_2026-08-15.md`

Important earlier authentication/OTP planning:

- `docs/ Ideas/32 - تسجيل الدخول والهوية والمصادقة متعددة العوامل.md`
- `docs/ Ideas/33 - OTP المستقبلي وقنوات التحقق.md`

Important discovery planning:

- `docs/ Ideas/31 - صفحات استكشاف الأنظمة والتطبيقات.md`

Important hierarchy/navigation planning:

- `docs/ Ideas/02 - خريطة الموقع والشاشات.md`
- `docs/ Ideas/05 - الحسابات والاشتراكات والتنقل.md`

But the next page must read **all** Shared Web files, not only these highlighted files.

---

# 10. CURRENT FCR PICTURE TO RECHECK LIVE

Do not trust this handover as a substitute for a live FCR read. The next page must query the repository again.

At the latest checkpoint, Web-owned implementation-required FCRs included:

- `FCR-0095` — Guardian/customer targeted notification and incident interaction — `Waiting On: WEB`
- `FCR-0125` — chart market-data request/presentation binding — `Waiting On: WEB`
- `FCR-0126` — Strategy/School overlay presentation binding — `Waiting On: WEB`
- `FCR-0127` — on-demand analysis presentation binding — `Waiting On: WEB`
- `FCR-0128` — dynamic Strategy/School catalog discovery binding — `Waiting On: WEB`
- `FCR-0130` — detailed AI analysis presentation binding — `Waiting On: WEB`
- `FCR-0133` — portfolio/positions/activity/performance presentation binding — `Waiting On: WEB`

Foundation-owned examples that Web must understand but must not act for:

- `FCR-0076` — generic emergency/recovery/control dependencies — `Waiting On: FOUNDATION`
- `FCR-0152` — authoritative Falcon identity/session/MFA boundary — `Waiting On: FOUNDATION`

Example with no immediate action:

- `FCR-0077` — Application-owned Web/emergency planning inputs — latest known `Waiting On: NONE`

These are examples only. The next page must read **every FCR, including those not listed here and those not Web-owned**.

Latest Web comments added during this handover preparation:

- FCR-0095 implementation checkpoint comment `5302337898`
- FCR-0152 phone/OTP clarification comment `5302338871`

Do not close either merely because these comments exist.

---

# 11. EXISTING WEB IMPLEMENTATION BEFORE THIS SESSION THAT MUST BE PRESERVED

The current Web implementation already contains and must preserve, after fresh file reading:

- Web-owned deployment portability/profile boundary;
- Falcon public feature;
- FSATS public feature;
- My Applications feature;
- FSATS workspace feature;
- public/workspace shells;
- route registry and presentation context;
- fail-closed auth adapter;
- portfolio/activity/markets/AI/notifications/Owner pages still partly inline in `src/app.js`;
- Owner Command Center direction;
- bilingual Arabic/English / RTL-LTR direction;
- incident interaction rules;
- chart/analysis/catalog/portfolio FCR distinctions;
- vendor-neutral infrastructure principle;
- current development/demo truth separation.

Do not remove prior safeguards just to simplify code.

---

# 12. IMPORTANT SECURITY / TRUTH BOUNDARIES TO PRESERVE

Examples that must survive all future refactors:

```text
UI_CLICK != AUTHORIZATION
REQUEST_SENT != ACTION_ACCEPTED != ACTION_COMPLETED
WEB_DISPLAY != BUSINESS_TRUTH_OWNER
NO_SOURCE_VALUE != ZERO
STALE != CURRENT
PARTIAL != COMPLETE
DISPLAYED_OVERLAY != CURRENT_APPLICATION_TRUTH
CATALOG_AVAILABLE != STRATEGY_ACTIVATED
CUSTOMER_ANALYSIS_REQUEST != EXECUTION_AUTHORITY
AI_CHAT_EXPLANATION != ANALYSIS_TRUTH_OWNER
NOTIFICATION_DELIVERED != INCIDENT_RESOLVED
SCREENSHOT_OBSERVED != BROKER_API_CONFIRMED
HUMAN_REPORTED_BROKER_STATE != BROKER_CONFIRMED_TRUTH
BROKER_ACCOUNT_IDENTITY != CUSTOMER_IDENTITY
```

Credentials never go into chat or reusable Web logs/state.

Owner incident-chat observation remains observer-only. Do not let Owner type as the AI or impersonate the system.

---

# 13. VERIFICATION STATUS

GitHub commits and focused test-file updates exist. The model-side container could not clone GitHub to execute `npm test` / `npm run check` because that execution environment had no DNS access to `github.com` at the time of handover preparation.

The latest queried GitHub combined-status list for commit `69cbcc4267d17f9ad20ab57969a18cb46f0db285` returned no CI statuses.

Therefore the next page must **not claim full executable verification has passed** merely from this handover.

At the next suitable executable environment/checkpoint:

```text
cd applications/shared/web
npm test
npm run check
```

Then perform the applicable architecture/security/accessibility/Red Team verification before declaring governed completion.

---

# 14. HOW TO CONTINUE WITH THE PROJECT OWNER

The Owner does not want the next page to restart the design interview or repeatedly ask settled questions.

Continue by:

1. live FCR check before every response;
2. read current source before changes;
3. consume any `Waiting On: WEB` action when enough evidence exists;
4. implement the next Web-owned slice inside the authorized subtree;
5. use FCR when another owner must act;
6. ask the Owner exactly one question only when a material Owner decision is genuinely required;
7. make obvious reversible low-risk UX decisions yourself;
8. after writes inspect the real diff/commit and report actual evidence;
9. keep runtime/live claims fail-closed until authoritative bindings exist.

Do not ask the Owner what to work on next if the repository/FCR state already tells Web what remains.

The current natural continuation after consolidating the public/auth/onboarding surfaces is to continue the outstanding Web feature/binding implementation obligations, especially the open `Waiting On: WEB` FCR families, while preserving the current public hierarchy and onboarding decisions.

---

# 15. FIRST RESPONSE EXPECTATION FOR THE NEXT PAGE

After completing the required full reading, the next page should tell the Owner concisely:

- that the full Shared Web source/planning files were read;
- that the complete repository-wide FCR set was read, including FCRs not owned by Web;
- the exact current `Waiting On: WEB` obligations discovered live;
- whether any newer Foundation/Application response changes the handover assumptions;
- the exact next Web-owned implementation action it will continue.

Do not claim this until the reading was actually performed.
