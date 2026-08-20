# Stage 6 WP-10 — Executable Validation Manifest Digest Failure Analysis

Status: CLASSIFIED / REMEDIATED IN WP-10 EVIDENCE PACKAGE
Date: 2026-08-11
Failed executable-validation candidate: `8031f1f1f21d83bded886fe6bfaa80ba19b92429`
Manifest remediation commit: `d6f8f33eae5af031ae2f83a9050cd32c12df1c4b`
Classification: `WP10_VERIFIER_OR_EVIDENCE_PACKAGE_DEFECT`

## 1. Failure summary

Exact executable validation against candidate `8031f1f1f21d83bded886fe6bfaa80ba19b92429` progressed successfully through the controlled sequence far beyond the earlier build defect.

The observed run established:

- exact candidate checkout: PASS;
- clean-tree preflight: PASS;
- exact .NET SDK `10.0.302`: PASS;
- Restore: PASS;
- Release Build: PASS;
- Foundation Architecture: PASS;
- Foundation Security: PASS;
- Stage 6 WP-01 verifier: PASS;
- Stage 6 WP-02 verifier: PASS;
- Stage 6 WP-03 verifier: PASS;
- Stage 6 WP-04 verifier: PASS;
- Stage 6 WP-05 verifier: PASS;
- Stage 6 WP-06 verifier: PASS;
- Stage 6 WP-07 verifier: PASS;
- Stage 6 WP-08 verifier: PASS;
- Stage 6 WP-09 verifier: PASS;
- WP-10 V3 immutable-history binding preflight: PASS.

WP-10 V3 Run 1 then failed inside the V2 closure-manifest validation with:

```text
FAIL closure_manifest_valid: Canonical closure SHA-256 mismatch for WP-07: docs/canonical-records/owner-decisions/stage6/Stage6-WP07-Final-Closure-20260810/OWNER-CLOSURE-STAGE6-WP07.md
```

The WP-10 verifier result was `27/28 PASS`, with this as the sole failed scenario.

The first supplied failed-run transcript SHA-256 was:

`E1252D4F0F2C4284A1E39FD9465D9D2AB16F2546C8AE5FFAFE65DA6BD33C6EE1`

A subsequent clean rerun reproduced the same WP-07 manifest-digest failure. Its supplied transcript SHA-256 was:

`6E5A34C979599B8EE53C7FBF8B53E50F963DC59DF8015B2140784070B8488C26`

## 2. Why this is not a predecessor closure defect

The WP-10 V3 immutable-history preflight passed before the V2 manifest validation failed.

That preflight verifies, for each predecessor closure represented in the manifest, that:

- the recorded closure decision commit exists;
- the accepted technical baseline commit exists;
- both are ancestors of the validation HEAD;
- the recorded closure path was added at the recorded closure decision commit; and
- the closure record blob at validation HEAD is byte-identical to the blob at the recorded closure decision commit.

Therefore the observed WP-07 digest mismatch does not indicate that the WP-07 closure record was rewritten after Owner closure.

No WP-07 production source, verifier, accepted baseline, closure decision, or canonical closure record was changed by this remediation.

`WP07_CLOSURE_REOPENED = FALSE`

## 3. Exact WP-07 evidence-package defect

The WP-10 closure manifest had recorded this WP-07 canonical closure digest:

`D03A43E096022D3D259177A56BA1CF627C1E0317E307A71A5CBF9E72C381E208`

Fresh byte-level verification of the exact canonical GitHub file bytes for:

`docs/canonical-records/owner-decisions/stage6/Stage6-WP07-Final-Closure-20260810/OWNER-CLOSURE-STAGE6-WP07.md`

established the actual SHA-256:

`E114D0A1D40C2714A69C30B02902A4F194D14E9E1CE8878C64D92F1C04ABA764`

The manifest value was therefore incorrect.

The canonical closure record itself remains unchanged.

## 4. Proactive WP-08 verification

Because the verifier fails closed on the first canonical closure digest mismatch, the failing run stopped its positive manifest validation at WP-07 and did not yet establish the later WP-08 digest binding.

Before requesting another machine rerun, Foundation proactively verified the exact WP-08 canonical closure bytes.

The WP-10 closure manifest had recorded:

`612409B7A3D7D0394BADE497801E1B10A3B82F30883F35963EBE0237D0A975F7`

for:

`docs/canonical-records/owner-decisions/stage6/Stage6-WP08-Final-Closure-20260810/OWNER-CLOSURE-STAGE6-WP08.md`

Fresh byte-level verification established the actual SHA-256:

`1B349114429EB6D4995D188105B6F3D639492BBA53AEA5FBFDB36D6FF6C5EC5E`

Therefore WP-08 contained the same WP-10 manifest evidence-package defect.

The canonical WP-08 closure record itself remains unchanged.

`WP08_CLOSURE_REOPENED = FALSE`

## 5. WP-09 control check

The next canonical closure record, WP-09, was also checked proactively.

Its exact canonical file-byte SHA-256 is:

`9647358F619488A6C817817324181CD24896CF1058F8634037F82A3C914AC8B9`

This exactly matches the value already recorded in the WP-10 closure manifest.

WP-01 through WP-06 were already traversed successfully by the positive `closure_manifest_valid` path before the WP-07 exception was raised.

Thus the currently established manifest correction set is exactly WP-07 and WP-08.

## 6. Classification

The accepted WP-10 plan defines failures involving WP-10's own verifier/evidence package as:

`WP10_VERIFIER_OR_EVIDENCE_PACKAGE_DEFECT`

This failure fits that category because:

- predecessor closure blobs are preserved by immutable-history verification;
- predecessor production semantics are unchanged;
- predecessor verifiers WP-01 through WP-09 pass in the integrated run;
- the defect is confined to incorrect byte-digest values recorded in the WP-10 closure manifest.

The accepted WP-10 plan permits direct remediation of WP-10 verifier/evidence-package defects under the already granted WP-10 implementation authority.

No new Owner authority is required for this correction.

## 7. Remediation

The only semantic data corrections made in `STAGE6_CLOSURE_MANIFEST.tsv` are:

```text
WP-07 closure_evidence_sha256
FROM D03A43E096022D3D259177A56BA1CF627C1E0317E307A71A5CBF9E72C381E208
TO   E114D0A1D40C2714A69C30B02902A4F194D14E9E1CE8878C64D92F1C04ABA764

WP-08 closure_evidence_sha256
FROM 612409B7A3D7D0394BADE497801E1B10A3B82F30883F35963EBE0237D0A975F7
TO   1B349114429EB6D4995D188105B6F3D639492BBA53AEA5FBFDB36D6FF6C5EC5E
```

No closure locator, closure decision commit, accepted technical baseline, executable evidence digest, Red-Team disposition, Application compatibility disposition, predecessor source, predecessor verifier, or predecessor closure record was changed.

## 8. Required next gate

A fresh static Red-Team must review this remediation before a new executable-validation candidate is frozen.

After static PASS, the complete exact executable-validation sequence must run again from the beginning against the exact new candidate commit.

No partial continuation from Step 13 is accepted.

`WP10_EXECUTABLE_VALIDATION = NOT_YET_PASS`

`WP10_OWNER_CLOSURE = NOT_YET`

`STAGE6_OWNER_CLOSURE = NOT_YET`

`STAGE7_AUTHORITY = NOT_GRANTED`
