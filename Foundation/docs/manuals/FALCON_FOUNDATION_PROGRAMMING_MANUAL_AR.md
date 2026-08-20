# دليل برمجة Falcon Foundation

**الإصدار:** 2026-08-19  
**الحالة الحاكمة:** `Stage 0A through Stage 16 = ACCEPTED_AND_CLOSED`  
**الـvalidated executable baseline:** `889a52ddcf492ecfa4f69c3f940d56362163f04f`  
**الحالة التنفيذية:** `FULL FOUNDATION VALIDATION = PASS`؛ جميع الـ88 governed verifiers ناجحة؛ Unknown Application verifier = `42/42 PASS`.  
**سياسة التكامل الحالية:** FDN-006 + FDN-007.  
**الجمهور:** المبرمجون، المعماريون، المراجعون، maintainers، وأي AI يعمل على Foundation أو يستهلك عقودها.

> هذا دليل هندسي موحد. لا يستبدل Falcon Vision أو Falcon Constitution أو ADRs/specifications المعتمدة أو سجلات Owner أو FDN-006 أو FDN-007 أو الكود/الدليل التنفيذي المعتمد. عند التعارض، المرجع الأعلى والدليل التنفيذي الدقيق هما المرجع.

---

# 1. وضع التشغيل وحدود العمل

## 1.1 قبل Live Seal الرسمي

أي تغيير Foundation-owned يتم فقط على `foundation-development` وضمن صلاحية حوكمة صريحة. لا توجد صلاحية كتابة على Application أو Shared Web من Foundation.

## 1.2 بعد Live Seal الرسمي

FDN-007 تصبح قاعدة consumer fit الدائمة:

```text
APPLICATION MUST FIT FOUNDATION
FOUNDATION MUST NOT CHANGE TO FIT APPLICATION
```

أي Application مستقبلية لا تناسب العقد المنشور تعالج نفسها من جهة Application، أو تستخدم capability موجودة من Shared Application، أو تعيد تصميم/تحذف المتطلب، أو تصبح `INCOMPATIBLE_WITH_SEALED_FOUNDATION`.

ممنوع بعد الـSeal إنشاء name/version allowlist أو special runtime branch أو security exception أو schema bypass أو weaker admission gate أو business-domain logic داخل Foundation من أجل Application جديد.

---

# 2. القواعد المعمارية وقواعد Authority

اتجاه الاعتماد المقبول:

```text
Applications
    ↓
Capabilities
    ↓
Shared Services
    ↓
Kernel / Foundation
```

الفواصل الإلزامية:

```text
TECHNICAL_CAPABILITY != AUTHORITY
TECHNICAL_SUCCESS != BUSINESS_AUTHORITY
IDENTITY_FACT != AUTHORITY_DECISION
AUTHENTICATION != AUTHORIZATION
VALID_CONTRACT != ADMISSION
ADMISSION != ACTIVATION
RUNTIME_REGISTRATION != ACTIVATION
RUNTIME_REGISTRATION != DEPLOYMENT_AUTHORITY
RUNTIME_REGISTRATION != BUSINESS_AUTHORITY
PUBLICATION != ACTIVATION
ROUTE_AUTHORIZED != CONNECTION_EXECUTED
CREDENTIAL_REFERENCE_ID != SECRET_BYTES
SELF_AWARENESS != AUTHORITY
READY_FOR_RELEASE_DECISION != RELEASE
RELEASE_AUTHORIZATION != RELEASE_EXECUTION
```

أي حالة unknown/stale/mismatched/revoked/incomplete/ambiguous/unverifiable في مسار يحتاج ثقة أو Authority يجب أن تفشل closed.

Zero-Application operation تبقى invariant مطلوبة.

---

# 3. ترتيب الـSource of Truth

1. Falcon Vision وFalcon Constitution.
2. Document authority وFoundation governance rules.
3. Owner decisions وcanonical closure records.
4. Accepted specifications/ADRs، ومنها APP-001 وCON-023 عند انطباقها.
5. FDN-006 Application Integration and Admission Profile.
6. FDN-007 Live Foundation Seal and Future Application Onboarding Policy.
7. Production contracts/source.
8. Architecture/Security tests وgoverned verifiers.
9. Planning/checkpoint/history documents.
10. V1.3 والـlegacy references عند الحاجة فقط.

`V1.3 = REFERENCE` وليس أعلى Authority.

---

# 4. خريطة المراحل للمبرمج

## Stage 0A
حوكمة، policy corpus، document authority، contracts، architecture rules وvalidation expectations.

## Stage 1
Project/Application models وManifest/configuration. الوصف لا يعطي Activation.

## Stage 2
Registry/Catalog exact. duplicate أو ambiguous identity يفشل closed. Registered لا يعني Active.

## Stage 3
Technical lifecycle state machine deterministic. Transition لا يصدر Authority جديدة.

## Stage 4
Runtime lifecycle: start/stop/restart. Restart ليس Recovery، وruntime start ليس business readiness.

## Stage 5
FIL production transport، schema/compatibility، delivery، dependency/evidence، plug-and-play boundaries. لا تعمل private authority side-channel خارج FIL المحكوم.

## Stage 6
Resource governance. حافظ على:

```text
0 <= Allocation <= Quota <= Ceiling
REQUESTED_RESOURCE != GRANTED_RESOURCE
RESOURCE_PROJECTION != RESOURCE_AUTHORITY
```

## Stage 7
Health/FSA technical fitness. Foundation technical self-model فقط، وApplication business judgment يبقى خارجها.

## Stage 8
Guardian وSafe State. Protection/containment لا تعطي release authority.

## Stage 9
Controlled recovery وindependent release. افصل repair/recovery/readiness/release authorization/release execution.

## Stage 10
FRS-001 reconstruction/review ضد الحالة الحاكمة الحالية.

## Stage 11
QoS/deadlines/observability. latency أو priority facts لا تصبح business authority.

## Stage 12
External-access governance. تقييم exact route authorization حسب identity/role/environment/purpose/destination/credential reference. الـevaluator لا ينفذ socket/provider/broker.

## Stage 13
FSA governance وAI Kill. FSA تراقب وتراجع ضمن حدود، والـindependent Foundation authority تملك Kill enforcement. protected properties لا تتغير عبر ordinary evolution.

## Stage 14
Canonical artifact publication. Identity/version/digest/evidence/compatibility/provenance ثابتة. branch HEAD المتحرك ليس runtime identity.

## Stage 15
Application runtime hosting عام ومحايد. Registration الناجح لا يعطي Activation أو deployment أو business authority.

## Stage 16
Identity/session/MFA runtime. Falcon identity وexternal links وreplay protection وMFA/session semantics بدون تحويل security context إلى business authority.

---

# 5. الـhardening النهائي بعد Stage 16، وليس Stage 17

تم تثبيت المسار العام لأي Application عبر:

- generic `PublicRuntimeProjectionProfiles`؛
- Unknown Application admission + real runtime hosting proof؛
- عدم الحاجة لأي Application name/version whitelist؛
- canonical AI Kill artifact publication؛
- exact public-runtime projection transport.

Unknown Application proof:

```text
UNKNOWN_APPLICATION_IDENTITY = unknown-application-proof-7f3c9a
APPLICATION_VERSION = 999.123.456-test
APPLICATION_NAME_ALLOWLIST = NOT_REQUIRED
APPLICATION_VERSION_ALLOWLIST = NOT_REQUIRED
MANIFEST_AND_FOUNDATION_CONTRACTS = REQUIRED
ADMISSION_TO_RUNTIME_HOSTING = PROVEN
TAMPERED_MANIFEST = FAIL_CLOSED
INVALID_FOUNDATION_REFERENCE = FAIL_CLOSED
PROVIDER_BOUNDARY_BYPASS = FAIL_CLOSED
CHECKS = 42/42 PASS
```

هذه هي أهم evidence أن Foundation تستطيع استضافة Application مستقبلية بدون معرفة business domain الخاص بها مسبقًا.

---

# 6. نموذج Admission وRuntime Registration

المسار الطبيعي:

```text
APPLICATION / PLUG-IN
    -> Manifest declaration
    -> Foundation validation
    -> Admission decision
    -> exact artifact/lifecycle/resource binding
    -> runtime registration
    -> separate activation authority
```

Admission تتحقق من exact identity/version/owner، Manifest digest، provenance digest، declarations، dependencies/contracts/specifications/services، provider boundary، permissions/authority requests، وdeterministic evidence.

Runtime registration تحتاج فوق ذلك exact runtime instance، artifact binding، positive admission evidence لنفس identity/version، eligible lifecycle Attach evidence، current resource grants، وvalid capability declarations.

نتيجة التسجيل يجب أن تبقى:

```text
RUNTIME_REGISTERED_NOT_ACTIVATED
CarriesDeploymentAuthority = false
CarriesBusinessAuthority = false
```

لا تستنتج Authority أقوى من نجاح Gate أسبق.

---

# 7. FDN-006 كعقد Consumer

عند دمج أي Application، تعامل مع FDN-006 كعقد Foundation-side للمواضيع التالية:

- identity/version/owner/purpose؛
- Manifest/provenance؛
- dependencies/contracts/specifications/services؛
- capabilities/consumers/exclusivity؛
- permissions/authority requests؛
- resources/ceilings؛
- security/provider boundaries/credential references؛
- lifecycle/update/recovery/removal؛
- health/failure containment؛
- FSA/MSA/LSA/CSA placement؛
- fail-closed semantics؛
- admission/registration/activation separation.

Application-side adapters مسموحة فقط إذا كانت تحول Application إلى عقد Foundation الموجود بدون تغيير semantics في Foundation.

---

# 8. قاعدة FDN-007 بعد Live Seal

بعد الـLive Seal الرسمي، لا تستخدم النموذج التاريخي:

```text
Application gap -> Foundation FCR -> Foundation code change
```

لـnew-Application fit.

النموذج الصحيح:

```text
Application requirement
    -> افحص published Foundation/Shared Application contracts
    -> adapt داخل Application إذا كان valid
    -> redesign/remove للمتطلب غير المدعوم
    -> إذا بقي يتطلب Foundation change: INCOMPATIBLE_WITH_SEALED_FOUNDATION
```

FCRs التاريخية تبقى audit records، وpre-Live-Seal reconciliation FCRs تغلق حسب evidence. لكنها ليست escape hatch دائم لتغيير Foundation بسبب consumer جديد.

---

# 9. Discipline التحقق

لأي executable Foundation change مصرح به فعليًا قبل Seal، حافظ على السلسلة:

```text
fresh authority/FCR/HEAD reconciliation
-> minimal Foundation-owned implementation
-> restore/build
-> Architecture verification
-> Security verification
-> affected-stage verifier
-> predecessor/cross-stage regressions
-> deterministic rerun عند الحاجة
-> clean tracked tree
-> stable exact candidate
-> Architecture/Consistency review
-> broad Red Team
-> Owner closure عند الحاجة
```

بعد Live Seal، ordinary Application onboarding لا يجب أن يشغل هذه الدورة لأنه لا يجب أصلًا أن يغير Foundation.

---

# 10. أسئلة Red Team للمبرمج

```text
هل missing identity تستطيع تمر؟
هل يمكن تغيير version/digest/provenance ويبقى PASS؟
هل stale evidence تصبح current؟
هل unknown تصبح healthy/authorized؟
هل registration تتحول إلى activation؟
هل publication تتحول إلى deployment؟
هل provider connectivity تتحول إلى execution authority؟
هل credential reference تكشف secret bytes؟
هل business-specific semantic تسربت إلى Foundation؟
هل Foundation تحتاج Application موجودة حتى تكون صحيحة؟
هل Application تستطيع أخذ resources/capabilities/authority من Application أخرى؟
هل MSA/LSA/CSA عبرت إلى FSA/Foundation scope؟
هل Application جديدة تستطيع فرض Foundation special case؟
```

أي forbidden path يجب أن تبقى مستحيلة أو fail closed.

---

# 11. Current readiness boundary

```text
STAGE_0A_THROUGH_STAGE_16 = ACCEPTED_AND_CLOSED
FULL_FOUNDATION_VALIDATION = PASS
GOVERNED_VERIFIERS = 88/88 PASS
UNKNOWN_APPLICATION_CHECKS = 42/42 PASS
FOUNDATION_READY_FOR_APPLICATION_CONSUMPTION = YES
FOUNDATION_CURRENTLY_REQUIRES_STAGE17 = NO
FOUNDATION_APPLICATION_NEUTRAL = YES
```

ولا تخلط:

```text
TESTED != DEPLOYED
READY_FOR_APPLICATION_CONSUMPTION != APPLICATION_ACTIVATED
FOUNDATION_READY != LIVE_TRADING_AUTHORITY
```

---

# 12. خريطة المراجع

- `docs/03_DOCUMENT_AUTHORITY.md`
- `docs/development/FOUNDATION_WORKSTREAM_RULES.md`
- `docs/foundation/FDN-006_APPLICATION_INTEGRATION_AND_ADMISSION_PROFILE.md`
- `docs/foundation/FDN-007_LIVE_FOUNDATION_SEAL_AND_FUTURE_APPLICATION_ONBOARDING_POLICY.md`
- `docs/manuals/FALCON_NEW_APPLICATION_ONBOARDING_MANUAL_AR.md`
- `docs/manuals/FALCON_NEW_APPLICATION_ONBOARDING_MANUAL_EN.md`
- CON-023 والعقود/ADRs المعتمدة ذات الصلة
- `src/Foundation.*`
- `tests/Falcon.Foundation.Architecture.Tests/`
- `tests/Falcon.Foundation.Security.Tests/`
- `verification/`

**القاعدة النهائية للمبرمج:** exact identity، explicit authority، evidence-bound decisions، Application neutrality، وfail-closed. بعد Live Seal ممنوع تعديل Foundation لمجرد أن Application جديدة صُممت حول عقد مختلف.