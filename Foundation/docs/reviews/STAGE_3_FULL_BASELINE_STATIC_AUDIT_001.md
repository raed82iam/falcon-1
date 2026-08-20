# Stage 3 Full Falcon Baseline Static Audit 001

**Status:** Accepted as the static basis for controlled remediation  
**Audit date:** 2026-08-03  
**Source commit:** `888fb661e9e32f253ea891c5d793d9852caf200d`  
**Source tree:** `b2f9e5fc1439e4382bfb7484fd714e6d483bf2a9`  
**Source review package SHA-256:** `3AEE49CB86408D13088E6C938A1297C37203A9B3A3DF534CBCFA96B677228C91`  
**Authority:** GOV-099; GOV-099-CORR-001

## Result

- Total static findings: `41`
- Blocking: `9`
- High: `24`
- Medium: `6`
- Low: `2`

The review package and frozen WP-05 Git identity passed integrity checks. The current baseline requires remediation before WP-06 implementation.

The .NET SDK was not available in the independent static-audit environment. Every code finding must therefore be confirmed or disproved by the governed Windows build, regression, concurrency, and independent-challenge workflow.

## Security-root clarification

The audit does not claim that the existing security test is proven to return a false PASS when launched from `C:\Users\raeda`. The confirmed defect is working-directory dependence: the test can inspect the wrong root or fail for a misleading path reason. The general scanner also silently skips unreadable files. Phase C must make root resolution deterministic and unreadable governed files fail closed.

## Findings

| ID | Severity | Area | Finding | Required remediation |
|---|---|---|---|---|
| DOC-001 | BLOCKING | Documentary activation | Eighteen of the 27 activated canonical targets still expose proposed, pending, missing, or older lifecycle metadata. | Reconcile only current canonical targets against GOV-092/GOV-093/GOV-094 and the completion manifest; preserve candidate and archive evidence. |
| DOC-002 | HIGH | Encoding | Nine active canonical targets contain mojibake such as —, →, and ’. | Repair UTF-8 text on active canonical surfaces and add encoding enforcement. |
| DOC-003 | HIGH | Registry ownership | Active SPEC-000 rows still contain UNKNOWN or pending-owner language for activated surfaces. | Resolve current canonical owner and authority metadata from the activated source and governance records. |
| DOC-004 | MEDIUM | Repository overview | Root README still describes an older documentary closure state and does not reconcile Stage 3 WP-05 or the baseline-remediation hold. | Update the active overview without rewriting historical evidence. |
| BLD-001 | HIGH | Toolchain pinning | No global.json pins the documented .NET SDK 10.0.302. | Add an exact SDK policy with fail-closed roll-forward behavior. |
| BLD-002 | HIGH | Language version | Directory.Build.props uses LangVersion=latest while governance states C# 14.0. | Pin the approved language version. |
| BLD-003 | HIGH | Text reproducibility | No .gitattributes or .editorconfig governs UTF-8 and line endings. | Add repository-wide encoding and line-ending controls. |
| BLD-004 | LOW | Repository hygiene | .gitignore is minimal and does not cover stage-local evidence and common IDE/build caches. | Harden ignores without hiding governed repository content. |
| SEC-001 | HIGH | Security gate root | Security tests derive RepositoryRoot from the current working directory, so they can inspect the wrong location or fail for a misleading path reason instead of deterministically proving the Falcon repository. | Resolve the repository root deterministically from the executable or solution markers and fail if the Falcon root cannot be established. |
| SEC-002 | BLOCKING | Security gate silent skip | Unreadable files return null and are silently skipped. | Treat unreadable candidate files as findings. |
| SEC-003 | HIGH | Security gate coverage | Security scanning omits verification projects and root build configuration files. | Scan all governed source, tests, verification, and root configuration surfaces. |
| SEC-004 | LOW | Security gate identity | The security output still identifies itself as WP-07. | Correct the gate identity and assert nonzero/minimum scan counts. |
| REG-001 | HIGH | Contract registry identity | ContractId@Version is a delimiter-composed string key and can collide for crafted values. | Use a structured ordinal key. |
| REG-002 | HIGH | Contract registry state | Mutable registry dictionaries are unsynchronized and snapshots are not explicitly ordered. | Lock mutations/lookups and emit sorted immutable snapshots. |
| REG-003 | HIGH | Contract registry boundary | Public operations can throw on null inputs instead of returning deterministic rejection. | Apply null-safe fail-closed results. |
| ADM-001 | BLOCKING | Admission identity replay | AdmissionId is reserved only after validation, allowing reuse after rejection. | Reserve every non-empty AdmissionId at first observation. |
| ADM-002 | HIGH | Admission concurrency | Admission identity and subject sets are unsynchronized. | Use one lock around stateful evaluation and snapshot operations. |
| ADM-003 | HIGH | Admission subject identity | Colon-composed subject keys and evidence IDs are collision-prone. | Use canonical length-prefixed or structured identity binding. |
| ADM-004 | MEDIUM | Admission decision seed | DecisionSeed is only checked for non-emptiness and is not bound to the decision. | Bind it canonically or remove it through an approved compatibility decision. |
| ADM-005 | MEDIUM | Admission declarations | Several declaration collections do not consistently reject duplicates. | Reject duplicate semantic declarations deterministically. |
| CAT-001 | BLOCKING | Registration identity replay | RegistrationId is recorded only after validation, allowing reuse after rejection. | Reserve every non-empty RegistrationId at first observation. |
| CAT-002 | HIGH | Service Catalog concurrency | Mutable registration, catalog, history, and sequence state is unsynchronized. | Serialize state changes and return immutable ordered snapshots. |
| CAT-003 | MEDIUM | Envelope purpose | Envelope purpose uses loose phrase containment rather than one canonical purpose. | Require exact governed purpose vocabulary. |
| CON-001 | HIGH | Contract validators | Public contract validators assume non-null top-level records and may throw. | Return deterministic failed validation for null and malformed nested values. |
| BOOT-001 | HIGH | Bootstrap public boundary | Bootstrap and lifecycle public entry points can throw for null top-level requests. | Return canonical fail-closed decisions without success events. |
| EN-001 | HIGH | Authority context | FoundationAuthorityContext.IsValid can dereference null runtime/evidence identifiers. | Make authority-context validation total and fail closed. |
| EN-002 | HIGH | Time provider | Future verification timestamps can qualify, negative uncertainty is not rejected, and uncertainty arithmetic can overflow. | Reject future/negative/unbounded evidence and use checked arithmetic. |
| EN-003 | HIGH | Identifier provider | Blank request/subject values are accepted and concurrent continuity is not atomic. | Validate identifiers and use atomic request continuity. |
| CRY-001 | BLOCKING | Stale key/secret references | Rotate and Revoke resolve only by reference ID, so stale versions can affect the current value. | Bind ID, version, domain, and purpose exactly. |
| CRY-002 | HIGH | Key custody concurrency | Use, rotate, revoke, and dispose can race while key bytes are zeroed. | Synchronize lifetime operations and prevent zeroing during active use. |
| CRY-003 | HIGH | Mutable byte exposure | Payload/result records expose mutable byte arrays. | Clone at boundaries and avoid returning mutable internal material. |
| CERT-001 | HIGH | Certificate identity | Expected subject is matched by substring containment. | Use the exact admitted subject identity/profile rule. |
| EVD-001 | HIGH | Evidence digest validation | Several digest checks validate length but not hexadecimal form. | Validate canonical uppercase/lowercase hexadecimal consistently. |
| EVD-002 | HIGH | Evidence set membership | Evidence for undeclared requirements and duplicate evidence identities are not rejected comprehensively. | Require exact requirement membership and unique evidence IDs. |
| EVD-003 | HIGH | Evidence integrity | Completeness trusts a caller-supplied IntegrityValid boolean. | Bind integrity to verifier-produced evidence or independently recompute it. |
| EVD-004 | MEDIUM | Environment profile | Six provider entries can satisfy the count even when duplicated or unexpected. | Require the exact unique approved provider-profile set. |
| WP06-001 | BLOCKING | WP-02 to WP-04 seam | WP-04's golden graph manually creates AdmissionDecision instead of using AdmissionControl.Evaluate output. | Use the real admission decision and evidence identity. |
| WP06-002 | BLOCKING | WP-04 to WP-05 graph identity | WP-04 and WP-05 bind different graph IDs while sharing a digest. | Create one canonical graph identity and regenerate its digest. |
| WP06-003 | BLOCKING | WP-04 to WP-05 evidence vocabulary | WP-05 expects generic states and a synthetic reference instead of WP-04's exact decisions and event. | Carry exact WP-04 output vocabulary and references. |
| WP06-004 | BLOCKING | End-to-end request replay | Admission and registration rejected IDs are reusable. | Covered by first-observation remediation and end-to-end negative tests. |
| WP06-005 | MEDIUM | Plug-in registration semantics | Initial WP-06 wording can imply the plug-in itself must be catalog-registered. | Register the required Foundation service; keep plug-in bootstrap registration NOT_APPLICABLE. |

## Decision

WP-06 remains on hold. The frozen WP-05 commit and tag remain immutable historical evidence. GOV-099 authorizes remediation only within its corrected exact 71-path allowlist and does not authorize commit or tag.
