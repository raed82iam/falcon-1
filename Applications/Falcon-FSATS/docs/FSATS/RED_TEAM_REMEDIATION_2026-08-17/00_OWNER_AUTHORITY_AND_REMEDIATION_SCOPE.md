# FSATS Owner Authority and Remediation Scope

Date: 2026-08-17
Branch: `application-development`
Writable scope: `applications/**`

## Owner authority

The Project Owner explicitly authorized the Falcon Application workstream to:

1. remediate all findings from the fresh full FSATS Red Team conducted on 2026-08-17;
2. implement the Application-side work required by FCR-0226 where that binding is not already implemented;
3. add the required adversarial tests and verification evidence;
4. update Application-owned documentation and FCR coordination evidence as required by `applications/FSATS/WORKSTREAM_RULES.md`.

## Authority ceiling

This authority does **not** grant:

- production runtime activation;
- provider connectivity activation;
- broker connectivity activation;
- Paper, Shadow, Tiny-Live, or Live activation;
- deployment authority;
- AI release or revival authority;
- Foundation write authority;
- Shared Web write authority.

## Governing separations

```text
REMEDIATION_AUTHORITY != RUNTIME_AUTHORITY
FCR0226_IMPLEMENTATION_AUTHORITY != AI_RELEASE_AUTHORITY
APPLICATION_AI_BUSINESS_SEMANTICS = APPLICATION_OWNED
FOUNDATION_KILL_ENFORCEMENT = FOUNDATION_OWNED
SELF_AWARENESS != SELF_GOVERNANCE
AI_RESTART != AUTHORITY_RESTORATION
```

## Fresh Red Team findings in scope

The authorized remediation covers the following findings:

### High

1. FSAPMA quota accounting must support multiple constrained quota dimensions on the same request with atomic reservation semantics.
2. APP-RSC additional-resource request handling must reject `Revoke`, `Reduce`, and `Restore` on the WP-06 request path.
3. Guardian protection dispatch must not expose a production-bindable raw-command path that can bypass the governed protection envelope.

### Medium

4. Digital City calibration evidence must be attributable evidence, not an unbound caller boolean, and qualification-relevant evidence must participate in artifact integrity.
5. Invalid Digital City scenario enum values must fail closed and the verifier must actually test that condition.
6. Durable integrity digests must cover semantically relevant effective-time fields.
7. APP-RSC Foundation resource-state projections must enforce bounded freshness rather than treating any same-epoch historical projection as current.

### Low hardening

8. Long-lived in-memory idempotency registries require governed bounded-retention behavior rather than unbounded growth.
9. Decimal analytics paths must fail closed on arithmetic overflow instead of escaping as uncontrolled exceptions.
10. Security verification assurance must explicitly distinguish lexical egress scanning from proof of all possible egress mechanisms.

## FCR-0226

FCR-0226 remains a separately governed Application-side binding obligation. The authorized implementation must bind the exact accepted Application AI inventory and runtime identities to the Foundation Stage 13 WP-01 target registration / containment contract and adversarially verify at least:

- no replacement-identity bypass;
- no silent restart restoration;
- no delegation restoration after containment;
- no alternate AI route-around;
- no hidden AI continuation through degraded fallback;
- no self-release;
- no evidence destruction;
- no fabricated AI business result when required AI truth is unavailable.

Implementation and verification of FCR-0226 do not activate production AI runtime and do not grant release/revival authority.
