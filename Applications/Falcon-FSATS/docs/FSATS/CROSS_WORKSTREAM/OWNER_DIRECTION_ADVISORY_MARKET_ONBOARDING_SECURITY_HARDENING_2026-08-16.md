# FSATS Advisory Market Onboarding — Provider URL and Owner Credential-Action Security Hardening

**Date:** `2026-08-16`  
**Status:** `CONTROLLING_HARDENING_FOR_CURRENT_OWNER_DIRECTION / PLANNING_ONLY`  
**Applies To:** `OWNER_DIRECTION_ADVISORY_MARKET_ONBOARDING_FREE_PROVIDER_AND_WEB_PRESENTATION_2026-08-16.md`  
**Runtime Authority:** `NOT_GRANTED`

## 1. Reason

Fresh adversarial review identified a security ambiguity in the planning semantics: a provider discovery result may contain a provider help/signup URL, but discovery alone must not convert an externally discovered URL into a trusted Owner navigation target.

This record hardens that ambiguity before cross-workstream handoff.

## 2. Controlling Rule

Any provider help/signup URL or chart source URL received through discovery is metadata only until independently validated against the governed provider identity and destination policy applicable to its use.

```text
DISCOVERED_PROVIDER_URL != TRUSTED_DESTINATION
PROVIDER_HELP_OR_SIGNUP_URL != AUTOMATICALLY_CLICKABLE_TRUSTED_URL
CHART_SOURCE_URL != WEB_PROVIDER_ROUTE_AUTHORITY
URL_DISCOVERY != DESTINATION_AUTHORIZATION
```

Shared Web shall not treat an Application-supplied discovery URL as authority to connect, embed, fetch, navigate, submit credentials, or disclose secrets.

## 3. Owner Credential Action

The Owner-facing request may identify the provider and explain that a free API key is required, but any link offered to help the Owner create the key must carry an explicit validation state.

Conceptual additional fields:

```text
providerHelpOrSignupUrl = OPTIONAL
providerHelpOrSignupUrlValidation = UNVALIDATED | VALIDATED | REJECTED
providerHelpOrSignupUrlPurpose = PROVIDER_ACCOUNT_OR_API_KEY_SETUP
```

Required behavior:

```text
UNVALIDATED -> DISPLAY_AS_UNTRUSTED_METADATA_OR_HIDE_ACTION_LINK
VALIDATED -> MAY_BE_PRESENTED_SUBJECT_TO_WEB_SECURITY_POLICY
REJECTED -> MUST_NOT_BE_OFFERED_AS_NAVIGATION_TARGET
```

The API key value itself must never be requested or entered in ordinary chat or an ordinary Application/Web business payload.

## 4. Chart Source URL

A chart-source URL supplied by FSATS identifies a candidate source for Web presentation planning only. Web-owned provider access remains subject to its exact destination governance, security, credential, terms, and verification requirements.

A URL cannot bootstrap its own authorization.

## 5. Effective Semantic Set

The effective current planning semantics are the combination of:

1. `OWNER_DIRECTION_ADVISORY_MARKET_ONBOARDING_FREE_PROVIDER_AND_WEB_PRESENTATION_2026-08-16.md`; and
2. this security hardening record.

Where the earlier record could be read as making a discovered help/signup URL directly trusted, this hardening controls and requires explicit URL validation state.

## 6. Authority Boundary

This hardening does not authorize provider connectivity, Web navigation behavior, secret storage, runtime routes, Part 8, or deployment. It adds a fail-closed planning requirement only.
