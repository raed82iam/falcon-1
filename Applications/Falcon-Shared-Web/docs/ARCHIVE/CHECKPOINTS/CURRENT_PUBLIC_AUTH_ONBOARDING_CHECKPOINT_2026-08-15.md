# Shared Falcon Web — Current Public, Discovery, Authentication and Onboarding Checkpoint

**Status:** `CURRENT WEB IMPLEMENTATION CHECKPOINT / OWNER-DIRECTED`  
**Date:** `2026-08-15`  
**Branch:** `web-development`  
**Writable scope:** `applications/shared/web/**`

This checkpoint reconciles the Project Owner decisions from the current Shared Web session with the existing Shared Web implementation and planning artifacts. It is not Foundation authority, Application business authority, production deployment authority, or FCR closure evidence by itself.

## 1. Public product hierarchy now used by Web

```text
FALCON OS
│
├── Falcon Self-Aware Trading System (FSATS)
│   ├── FSATA
│   ├── FSAPMA
│   ├── FTGA
│   ├── FSTSimA
│   └── APP-RSC
│
├── future Accounting system/family
├── future Warehouse system/family
└── other future Falcon systems/families
```

Mandatory presentation distinctions:

```text
FALCON_OS != FSATS
FSATS = TRADING_DOMAIN_SYSTEM
FSATS_CHILD_APPLICATION != FALCON_TOP_LEVEL_SYSTEM
NON_TRADING_DOMAIN != FSATS_CHILD
```

The Falcon public home therefore presents FSATS as the current top-level Trading system. The five FSATS Applications are not flattened into Falcon OS sibling cards. Their detailed discovery experience lives inside the FSATS public page.

## 2. Current public page direction

The accepted visual/product direction remains the dark premium Falcon OS public interface supplied by the Project Owner, with Falcon OS as the umbrella identity and FSATS as the current featured system.

Current implementation organization:

```text
Falcon Public Home
  -> Falcon OS identity / purpose
  -> current top-level system catalog
  -> FSATS featured discovery entry

Applications Page
  -> top-level Falcon systems only
  -> hierarchy explanation

FSATS Public Page
  -> FSATS hero / purpose
  -> five FSATS child Applications
  -> simple discovery/explainer content for every child Application
  -> Sign In
  -> Google / Microsoft sign-in presentation
  -> Authenticator MFA explanation
  -> new-account onboarding presentation
```

## 3. FSATS child Application discovery

The FSATS page now presents the five Project Owner-defined Applications:

1. `FSATA` — Falcon Self-Aware Trading Application
2. `FSAPMA` — Falcon Self-Aware Provider Management Application
3. `FTGA` — Falcon Trading Guardian Application
4. `FSTSimA` — Falcon Self-Aware Trading Simulation Application
5. `APP-RSC` — Falcon Self-Aware Resource Management Application

Each Application receives:

- its full name and short identity;
- short, plain-language explanatory copy;
- `Discover Application / استكشف التطبيق` disclosure;
- a lightweight animated explanatory flow;
- explicit wording that the public explainer does not claim unavailable live runtime capability;
- room for final approved animation/video assets later.

Motion must respect `prefers-reduced-motion`.

## 4. Sign-in and authentication presentation

Current Web presentation includes:

- username/email + password where an authoritative local credential flow eventually exists;
- `Continue with Google`;
- `Continue with Microsoft`;
- Authenticator-based MFA explanation for customers and Project Owner;
- fail-closed behavior while the authoritative identity/session/MFA boundary is unavailable.

Mandatory distinctions:

```text
OIDC_AUTHENTICATED != FALCON_IDENTITY
OIDC_AUTHENTICATED != PROJECT_OWNER
MFA_PASSED != BUSINESS_AUTHORITY
EMAIL_MATCH != IDENTITY_LINK_AUTHORIZATION
WEB_SESSION_PRESENTATION != AUTHORITY_ISSUANCE
```

Live Google/Microsoft connectivity, session issuance, role truth, MFA secret custody and production authentication remain dependent on the authoritative boundary tracked by `FCR-0152`.

## 5. New-user phone requirement

Project Owner decision:

> Every new customer shall provide a phone number during onboarding so Falcon has an emergency/high-priority contact path.

Current Web requirement:

```text
NEW_CUSTOMER_ONBOARDING -> PHONE_REQUIRED
PHONE_PURPOSE = EMERGENCY_AND_HIGH_PRIORITY_CONTACT
PHONE_PROVIDED != PHONE_VERIFIED
PHONE_PROVIDED != FALCON_IDENTITY
PHONE_PROVIDED != MFA_FACTOR
PHONE_PROVIDED != BUSINESS_AUTHORITY
```

The user-facing explanation states that the number is required for emergency/high-priority contact, is not automatically a marketing opt-in, and is not OTP verification today.

Phone contact data remains distinct from FSATS broker-account identity and Application-required business information.

## 6. OTP decision

OTP delivery is deferred for future review.

Candidate future channels preserved in Ideas file 33:

- Telegram
- WhatsApp
- SMS

Current state:

```text
OTP_SERVICE = DEFERRED
OTP_PROVIDER_SELECTION = NONE
PHONE_REQUIRED_FOR_CONTACT = YES
PHONE_OTP_VERIFICATION = NOT_CURRENTLY_ACTIVE
```

Google/Microsoft sign-in and Authenticator MFA direction are not canceled by this OTP deferral.

## 7. Implementation evidence from this reconciliation

Public hierarchy implementation:

- `applications/shared/web/src/features/falcon-public/falcon-public.js`
- commit `7e81d830d489c28c9ed8afd38dd355441f4ef1ca`

FSATS discovery + onboarding implementation:

- `applications/shared/web/src/features/fsats-public/fsats-public.js`
- commit `eee1f432f4f16755cad57329e40d3c2b2594c235`

Presentation styling:

- `applications/shared/web/src/extensions.css`
- commit `868636f37096b07c61bd10a36962d4b55255b4e5`

Focused tests updated:

- `applications/shared/web/tests/fsats-public.test.mjs`
- commit `cef36ca568d406020e3d64cb687ab51846c131ef`

- `applications/shared/web/tests/falcon-public.test.mjs`
- commit `9562428ba60d68161fede7b5316a2132a4519c4f`

Earlier related planning/authentication decisions remain preserved in:

- `docs/ Ideas/02 - خريطة الموقع والشاشات.md`
- `docs/ Ideas/05 - الحسابات والاشتراكات والتنقل.md`
- `docs/ Ideas/31 - صفحات استكشاف الأنظمة والتطبيقات.md`
- `docs/ Ideas/32 - تسجيل الدخول والهوية والمصادقة متعددة العوامل.md`
- `docs/ Ideas/33 - OTP المستقبلي وقنوات التحقق.md`

## 8. Verification truth

The source and focused tests were updated to cover the Owner-directed presentation rules. A Git commit or test-file update is not by itself proof that runtime binding, production authentication, Foundation integration, or full governed Web verification is complete.

Open implementation-required FCRs must remain open until their actual implementation/binding/verification obligations are satisfied.

## 9. Next implementation direction

Continue the existing Shared Web implementation decomposition without redesigning accepted decisions:

1. preserve and harden Falcon Public / FSATS Public / onboarding surfaces;
2. continue Web-owned user feature-slice extraction and governed bindings;
3. complete `Waiting On: WEB` FCR implementation obligations one by one;
4. consume Foundation/Application responses only through governed contracts/FCRs;
5. run fresh Architecture / Security / Red Team / accessibility verification at the appropriate checkpoint;
6. never claim production/live authentication or other runtime truth before authoritative binding and verification exist.
