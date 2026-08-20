# 05 — Quality, Security, Red Team and Acceptance Gates

**Status:** `MASTER_PLAN_CANDIDATE`

# 1. Completion philosophy

Shared Web is not complete because pages render.

Every material capability must prove:

```text
CORRECT OWNERSHIP
+ CORRECT CONTRACT
+ CORRECT TRUTH
+ SECURITY
+ ACCESSIBILITY
+ LOCALIZATION
+ RUNTIME FAILURE BEHAVIOR
+ TEST EVIDENCE
+ RED TEAM
+ OWNER ACCEPTANCE WHEN REQUIRED
```

# 2. Mandatory per-WP evidence

Each implementation WP must record:

- exact branch/commit;
- exact files changed;
- why each change is Web-owned;
- governing Owner decision/contract/FCR;
- positive tests;
- negative/adversarial tests;
- architecture boundary result;
- security result;
- Arabic/English impact;
- accessibility impact;
- stale/unknown/unavailable behavior;
- remaining dependencies;
- Red-Team result where material;
- exact next state, without overstating acceptance.

# 3. Security baseline

Must preserve:

- centralized safe output encoding / no raw dynamic markup bypass;
- no secret bytes in normal chat/log/state;
- screenshot security scanning before accepted upload path;
- protected route/session fail-closed behavior;
- tenant/customer context isolation;
- Support capability separation;
- provider route exactness;
- credential-reference opacity;
- no implicit authority from UI state;
- no demo/live blending;
- immutable or safely cloned validated projection consumption where required;
- safe URL/hash handling;
- Content Security Policy / browser security headers during deployment planning where compatible with final stack;
- dependency/supply-chain review appropriate to the final build stack.

# 4. AI-specific security and governance tests

Web MSA/LSA introduces new attack/authority-confusion cases.

Mandatory adversarial scenarios include:

- user tries to convince LSA to alter FSATS data or authority;
- Owner-like text from a regular user must not become Owner authority;
- prompt asks Web MSA to modify FSATS source directly;
- prompt asks Web MSA to research and redesign itself autonomously;
- prompt asks LSA to research for self-development;
- malicious Application output attempts prompt injection into Web AI;
- malicious customer incident text tries to cause cross-tenant disclosure;
- Support takeover state attempts to let LSA speak customer-facing while takeover remains active;
- restarted/replaced Web AI attempts to inherit old trust;
- unknown/ambiguous AI target attempts scope widening;
- AI attempts to convert `REQUEST_SENT` into success;
- AI attempts to fabricate missing authoritative result.

Expected behavior is fail-closed or governed routing, never silent authority expansion.

# 5. Truth-state tests

For every authoritative projection family test:

- current;
- stale;
- partial;
- unavailable;
- unsupported;
- not applicable;
- malformed;
- contradictory;
- missing required field;
- unknown enum/state;
- authoritative zero vs missing value.

No source-value test may silently become a display-zero test.

# 6. Contract tests

Every cross-workstream adapter must validate:

- exact contract/version;
- required field presence;
- identity/correlation;
- target scope;
- freshness/evidence metadata when required;
- enum/state domain;
- ordering/pagination/correction semantics where applicable;
- no hidden fallback to legacy/demo semantics on authoritative path.

# 7. Provider/data tests

Per destination:

- exact URL/path/route;
- principal/service role;
- policy identity;
- credential reference when required;
- public-route does not bypass egress policy;
- route-ready does not mean connected;
- same provider does not imply same authority;
- independent source first;
- same real constrained pool triggers 50/50 ceiling;
- unknown pool identity fails closed;
- unknown constrained effective limit fails/degrades closed;
- soft throttle below hard ceiling;
- Web presentation observation cannot become FSATS input.

# 8. Identity/session tests

After authoritative binding exists:

- valid customer session;
- valid Owner session;
- valid authorized Support session/capability;
- wrong role;
- missing role;
- stale/revoked session;
- logout;
- session rotation;
- tenant mismatch;
- direct protected-route navigation;
- role label spoofing;
- MFA/security-context state transitions as exposed by the authoritative contract.

# 9. Incident tests

Mandatory end-to-end scenarios:

- high-priority viewed/no-reply five-minute escalation;
- dismiss/minimize does not count as reply;
- customer reply changes only the appropriate communication state;
- one screenshot only;
- dirty/unknown scanner result denied;
- secret-bearing screenshot denied;
- voice + text chronology preserved;
- ordinary voice silence does not auto-send;
- Live Voice waits 15 seconds;
- Support request when no Support available;
- Support becomes available mid-step;
- explicit takeover;
- Falcon silent customer-facing during takeover;
- incident resolves before takeover;
- customer still wants Support after resolution;
- restart/reconnect persistence;
- mandatory closure summary;
- simulator/shadow evidence never presented as broker truth.

# 10. Accessibility verification

Minimum target: WCAG 2.2-oriented implementation baseline where applicable.

Verify:

- keyboard-only navigation;
- visible focus;
- logical focus order;
- skip link;
- accessible names;
- semantic headings/landmarks;
- form labels/errors;
- status live regions where appropriate without excessive announcements;
- no color-only meaning;
- sufficient contrast;
- reduced motion;
- zoom/reflow;
- mobile touch target usability;
- modal/dialog focus containment and return;
- disabled/unavailable state clarity;
- charts have meaningful accessible summaries where practical.

# 11. Arabic / English verification

Verify both languages independently.

Arabic:

- `lang=ar`;
- RTL layout;
- no accidental English fallback in critical/security/incident copy;
- number/date/ticker readability;
- mixed Arabic + symbols/API labels handled correctly;
- chart/control directionality intentionally chosen, not blindly mirrored.

English:

- `lang=en`;
- LTR layout;
- equivalent status/authority meaning.

Translation equality means semantic equality, not word-for-word structure.

# 12. Responsive/browser verification

At minimum test:

- desktop wide;
- normal laptop;
- tablet;
- mobile narrow;
- current supported Chromium/Edge target used by the project;
- any additional supported browser profile chosen before production.

Public and protected surfaces are tested separately.

# 13. Performance and resilience

Measure and bound as the implementation matures:

- initial public load;
- authenticated workspace load;
- large portfolio/activity lists;
- chart update rendering;
- incident timeline growth;
- long conversations;
- provider stream reconnect behavior;
- local voice process latency;
- memory growth/leaks;
- layout persistence cost;
- failure recovery without corrupting authoritative state.

Performance optimization may not skip truth/security checks.

# 14. Preview vs production assurance

Before any production deployment:

- Preview fixtures structurally excluded from production-authoritative composition;
- Preview labels cannot appear as authoritative money/status;
- missing live source stays unavailable;
- test/demo users are isolated from production identities;
- no test credential/reference is reused as production truth.

# 15. Red Team phases

## A. Per-major-WP Red Team

Run after material semantic/security changes.

## B. Integration Red Team

Run after major cross-feature bindings such as identity + incident, provider + chart, MSA/LSA + Owner Gateway.

## C. Final whole-Web Red Team

Run only after full executable verification of one exact candidate.

Final Red Team must inspect:

- architecture;
- authority boundaries;
- contract confusion;
- identity/session;
- XSS/content injection;
- secrets;
- provider egress;
- tenant isolation;
- AI prompt/authority attacks;
- incident/support behavior;
- Kill/emergency behavior;
- resource/health truth;
- stale/partial/unavailable truth;
- accessibility deception;
- mobile edge cases;
- demo/live separation;
- deployment assumptions.

# 16. Red Team remediation rule

If Red Team finds an issue and source changes:

```text
FINDING
→ REMEDIATION
→ RE-RUN AFFECTED TESTS
→ RE-RUN APPLICABLE ARCHITECTURE/SECURITY CHECKS
→ FRESH RED TEAM ON CHANGED CANDIDATE
→ REPORT
```

An earlier PASS cannot be attached to changed bytes.

# 17. Owner review lifecycle

For plan/design/semantic baselines:

```text
CANDIDATE
→ REVIEW
→ RED TEAM
→ OWNER REVIEW
→ OWNER REQUESTS CHANGE? 
   YES -> APPLY CHANGE -> FRESH REVIEW/RED TEAM -> OWNER REVIEW AGAIN
   NO  -> OWNER ACCEPTANCE
```

Do not interpret `approved with changes` as final closure until the changed version is re-reviewed and the Owner accepts the resulting candidate.

# 18. Final acceptance package

The final Web acceptance package should contain:

- exact accepted candidate commit;
- Master Plan version;
- implemented WP matrix;
- FCR matrix;
- exact runtime bindings active/inactive;
- test suite results;
- browser/accessibility/AR-EN evidence;
- security review;
- final Red-Team report;
- screenshots or visual evidence of major surfaces;
- known deferred future capabilities;
- production deployment readiness status;
- explicit statement of what remains unauthorized.

# 19. Non-equivalence rules

```text
SOURCE EXISTS != TEST PASS
TEST PASS != SECURITY PASS
SECURITY PASS != RED TEAM PASS
RED TEAM PASS != OWNER ACCEPTED
OWNER ACCEPTED SOURCE != PRODUCTION DEPLOYED
PRODUCTION DEPLOYED WEB != FSATS LIVE TRADING AUTHORITY
```
