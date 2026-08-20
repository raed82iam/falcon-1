# Shared Falcon Web - Best-in-Class Implementation Rule

**Status:** ACTIVE IMPLEMENTATION RULE  
**Date:** 2026-08-15  
**Branch:** `web-development`  
**Scope:** `applications/shared/web/**`

## Owner direction

For every material Web architecture, engineering, security, accessibility, maintainability, UX, testing, integration, and operational design decision, Shared Falcon Web SHALL seek the best available solution in the field rather than defaulting to the easiest, most familiar, or merely popular option.

This rule is subordinate to and constrained by Falcon's governing authority chain. A technically fashionable or high-performing solution SHALL NOT be adopted if it conflicts with the Falcon Vision, Falcon Constitution, accepted governance, applicable specifications, FCR boundaries, workstream ownership, or explicit Owner decisions.

## Decision order

```text
FALCON VISION
    ↓
FALCON CONSTITUTION
    ↓
GOVERNING POLICIES / SPECIFICATIONS / ADRs / WORKSTREAM RULES
    ↓
AUTHORITATIVE FOUNDATION + APPLICATION CONTRACTS
    ↓
CURRENT EVIDENCE + EXTERNAL BEST-IN-CLASS RESEARCH
    ↓
ALTERNATIVE COMPARISON
    ↓
SELECT BEST FIT FOR FALCON
    ↓
IMPLEMENT
    ↓
VERIFY / TEST / RED TEAM
```

## Required research behavior

Before a material technical choice is frozen, Shared Falcon Web SHALL, where the choice can materially affect quality, maintainability, security, resilience, accessibility, performance, or long-term evolution:

1. review current authoritative Falcon constraints first;
2. research current primary/official technical sources and standards;
3. compare credible alternatives rather than accepting the first workable option;
4. evaluate maintainability, modularity, security, performance, accessibility, interoperability, testability, replacement cost, long-term support, vendor/framework lock-in, and Falcon Foundation compatibility;
5. prefer evidence over popularity, trend, or convenience;
6. record important tradeoffs where the decision is architectural or difficult to reverse;
7. reject any external pattern that weakens Falcon authority boundaries or truth semantics;
8. re-evaluate adopted approaches when materially better evidence or technology appears, subject to governed change.

## Current external engineering baseline

The Web workstream will use current authoritative standards and official guidance as inputs, including as applicable:

- W3C WCAG 2.2 as the accessibility baseline, with Level AA as the default Web target unless a stronger Falcon requirement applies;
- OWASP ASVS 5.x as a Web application security verification reference;
- NIST Secure Software Development Framework (SSDF) as a secure-development lifecycle reference;
- official framework/runtime/platform documentation for any adopted technology;
- current browser/platform standards rather than proprietary assumptions where practical.

These external sources are advisory engineering evidence. They do not outrank Falcon governance.

## Architectural consequences

Shared Falcon Web SHALL remain:

- modular in capability;
- explicit at Foundation/Application boundaries;
- replaceable where practical;
- maintainable without hidden coupling;
- testable independently from live authoritative systems;
- fail-closed when authoritative identity, truth, or authority is unavailable;
- free of fabricated business semantics;
- compatible with governed Falcon contracts rather than Foundation/Application internals;
- capable of evolving without requiring broad rewrites for ordinary feature changes.

```text
BEST_IN_CLASS != MOST_POPULAR
BEST_IN_CLASS != MOST_COMPLEX
BEST_IN_CLASS != NEWEST
BEST_IN_CLASS != FASTEST_TO_IMPLEMENT
BEST_IN_CLASS = BEST EVIDENCE-BASED FIT THAT PRESERVES FALCON GOVERNANCE AND LONG-TERM QUALITY
```

## Verification rule

A material implementation choice is not considered complete merely because it works locally. It must also be reviewable against:

- constitutional/governance compliance;
- workstream ownership;
- authoritative contract correctness;
- maintainability/modularity;
- security;
- accessibility where applicable;
- automated verification where practical;
- failure-state behavior;
- Red Team review before final Owner acceptance for the relevant implementation scope.
