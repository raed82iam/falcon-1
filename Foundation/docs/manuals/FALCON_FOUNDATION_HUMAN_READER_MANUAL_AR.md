# دليل Falcon Foundation للقارئ البشري

**الإصدار:** 2026-08-19  
**الحالة الحاكمة:** `Stage 0A through Stage 16 = ACCEPTED_AND_CLOSED`  
**وضع Foundation الحالي:** مكتملة تقنيًا وجاهزة للدخول في Live Seal رسمي، لكن هذا الدليل لا يعلن وحده التفعيل أو الـdeployment أو أي business authority.  
**الـvalidated executable baseline:** `889a52ddcf492ecfa4f69c3f940d56362163f04f`  
**نتيجة التحقق التنفيذي:** `FULL FOUNDATION VALIDATION = PASS`؛ جميع الـ88 governed verifiers ناجحة؛ Unknown Application verifier = `42/42 PASS`.  
**المراجع الحالية للتكامل المستقبلي:** `FDN-006` و`FDN-007`.  
**الجمهور:** المالك، مدير المشروع، المراجع، المعماري، فريق التشغيل، وأي شخص يريد فهم Foundation بدون قراءة الكود.

> هذا الملف يشرح الحالة الحالية بلغة بشرية. لا يستبدل Falcon Vision أو Falcon Constitution أو سجلات Owner الكانونية أو العقود/ADRs المعتمدة أو FDN-006 أو FDN-007 أو الكود الإنتاجي. عند التعارض، المرجع الحاكم الأعلى والدليل التنفيذي المعتمد هما المرجع.

---

# 1. ما هي Falcon Foundation؟

Falcon Foundation هي الأرضية التقنية الثابتة التي تعمل فوقها Falcon Applications. وظيفتها أن توفر قواعد تشغيل وحوكمة تقنية عامة، بينما يبقى منطق العمل داخل Applications.

Foundation تملك أمورًا مثل:

- هوية المشروع/Application والـManifest؛
- Registry والعقود الكانونية؛
- lifecycle وruntime lifecycle؛
- FIL والاتصالات؛
- dependency/evidence governance؛
- resource governance والضغط؛
- health وFoundation self-awareness وtechnical fitness؛
- Guardian والحماية والاحتواء وSafe State؛
- recovery وindependent release؛
- external-access authorization وcredential references؛
- canonical artifact publication؛
- generic Application runtime hosting؛
- identity/authentication/session/MFA؛
- FSA governance وindependent AI Kill enforcement.

Foundation لا تملك منطق التداول أو المحاسبة أو الطب أو الرسم البياني أو أي business-domain logic خاص بتطبيق.

الترتيب المعماري يبقى:

```text
Applications
    ↓
Capabilities
    ↓
Shared Services
    ↓
Kernel / Foundation
```

والحد الدائم هو:

```text
APPLICATION BUSINESS LOGIC = APPLICATION OWNED
FOUNDATION TECHNICAL GOVERNANCE = FOUNDATION OWNED
```

---

# 2. أهم القواعد

الـFoundation مبنية على فصل صارم بين أشياء تبدو متشابهة لكنها ليست نفس الشيء:

```text
TECHNICAL_CAPABILITY != AUTHORITY
TECHNICAL_SUCCESS != BUSINESS_AUTHORITY
AUTHENTICATION != AUTHORIZATION
REGISTERED != ACTIVATED
ADMITTED != ACTIVATED
PUBLISHED != ACTIVATED
ROUTE_AUTHORIZED != CONNECTION_EXECUTED
READY_FOR_RELEASE_DECISION != RELEASE
RELEASE_AUTHORIZATION != RELEASE_EXECUTION
CREDENTIAL_REFERENCE_ID != SECRET_BYTES
SELF_AWARENESS != AUTHORITY
```

ومعها قواعد ثابتة:

- Architecture First.
- Vision وConstitution أعلى من سهولة التنفيذ.
- الـAuthority يجب أن تكون صريحة، محدودة، قابلة للتتبع والمراجعة، minimal، وحالية.
- unknown/stale/mismatched/revoked/unverifiable state يفشل closed عندما تكون الثقة أو الصلاحية مطلوبة.
- Foundation صحيحة حتى لو كان عدد Applications صفرًا.
- Application لا تسيطر على Application ثانية لمجرد أن الاثنين hosted على نفس Foundation.
- FSA تبقى فقط على مستوى Foundation/OS، وMSA/LSA/CSA تبقى داخل Applications.

---

# 3. ماذا بُني عبر المراحل؟

## Stage 0A
حوكمة، عقود، document authority، التعامل مع legacy/reference، وقاعدة Architecture-first.

## Stage 1
Project/Application models وManifest وDTO/configuration. الوصف لا يساوي صلاحية تشغيل.

## Stage 2
Registry/Catalog دقيق. التسجيل أو الاكتشاف لا يعني Activation.

## Stage 3
Technical lifecycle state machine وانتقالات deterministic. الانتقال لا يخلق authority جديدة.

## Stage 4
Runtime lifecycle تقني: start/stop/restart وحالات التشغيل بدون خلطها مع business activation أو deployment.

## Stage 5
FIL production transport، schemas، compatibility، delivery، dependency/evidence، وحدود plug-and-play.

## Stage 6
Foundation resource truth، quotas، pressure/degradation، defer/deny/load-shedding.

## Stage 7
Health، evidence awareness، technical fitness وFoundation self-model، بدون business judgment.

## Stage 8
Guardian، protective restriction، containment/isolation وPlatform Safe State.

## Stage 9
Controlled recovery وفصل repair/recovery/readiness/release authorization/release execution.

## Stage 10
إعادة بناء ومراجعة FRS-001 release semantics ضد الحالة الحالية بدل الاعتماد على legacy assumptions.

## Stage 11
Deadlines/expiry وQoS observability مثل p50/p95/p99 بدون تحويل QoS إلى business authority.

## Stage 12
External-access authorization دقيق حسب principal/role/environment/purpose/destination/credential reference. يقرر صلاحية المسار تقنيًا لكنه لا ينفذ الاتصال.

## Stage 13
FSA governance، independent monitors، investigation hold، trusted baselines، remediation sandbox، Controlled Revival، bounded evolution review، وindependent AI Kill.

## Stage 14
Canonical artifact publication بهوية وversion وSHA-256 وevidence وcompatibility/provenance، مع public read-only projections.

## Stage 15
Generic Application runtime hosting مع بقاء Foundation Application-neutral ومع الحفاظ على zero-Application operation.

## Stage 16
Falcon identity/authentication/session/MFA runtime مع replay protection وربط خارجي صريح، بدون تحويل login إلى business authority.

---

# 4. أهم hardening نهائي: Unknown Application proof

بعد Stage 16 تم عمل hardening عام بدون اختراع Stage 17.

تم اختبار Application صناعية لم تكن Foundation تعرفها مسبقًا باسم:

`unknown-application-proof-7f3c9a`

وبنسخة arbitrary:

`999.123.456-test`

ونجحت في المسار العام حتى real runtime hosting بدون name allowlist أو version allowlist.

النتيجة أثبتت:

```text
APPLICATION_NAME_ALLOWLIST = NOT_REQUIRED
APPLICATION_VERSION_ALLOWLIST = NOT_REQUIRED
MANIFEST_AND_FOUNDATION_CONTRACTS = REQUIRED
ADMISSION_TO_RUNTIME_HOSTING = PROVEN
TAMPERED_MANIFEST = FAIL_CLOSED
INVALID_FOUNDATION_REFERENCE = FAIL_CLOSED
PROVIDER_BOUNDARY_BYPASS = FAIL_CLOSED
RUNTIME_REGISTRATION != ACTIVATION
RUNTIME_REGISTRATION != DEPLOYMENT_AUTHORITY
RUNTIME_REGISTRATION != BUSINESS_AUTHORITY
```

وهذا هو السبب الذي يجعل Foundation مناسبة لبرامج مستقبلية مثل Accounting أو Logistics أو Research أو Medical أو Web أو أي Domain آخر، بدون أن تعرف منطق هذا البرنامج مسبقًا.

---

# 5. FDN-006: عقد التكامل العام

`docs/foundation/FDN-006_APPLICATION_INTEGRATION_AND_ADMISSION_PROFILE.md`

هو المرجع الكانوني لشروط دخول أي Application على Foundation.

يغطي:

- الهوية والنسخة والمالك؛
- Manifest وprovenance؛
- dependencies؛
- Foundation contracts/specifications/services؛
- capabilities/consumers؛
- permissions وauthority requests؛
- resource grants/ceilings؛
- security والـcredential references؛
- lifecycle/recovery/removal؛
- health/failure containment؛
- MSA/LSA/CSA placement؛
- admission/runtime registration/separate activation؛
- fail-closed behavior.

المسار الطبيعي:

```text
APPLICATION DESIGN
    -> MANIFEST
    -> FOUNDATION VALIDATION
    -> ADMISSION
    -> ARTIFACT / LIFECYCLE / RESOURCE BINDING
    -> RUNTIME REGISTRATION
    -> SEPARATE ACTIVATION AUTHORITY
    -> ACTIVE ONLY WHEN ALL GATES PASS
```

---

# 6. FDN-007: ماذا يحدث بعد Live Seal؟

`docs/foundation/FDN-007_LIVE_FOUNDATION_SEAL_AND_FUTURE_APPLICATION_ONBOARDING_POLICY.md`

يحدد طريقة العمل الدائمة بعد إعلان Foundation رسميًا Live وSealed.

القاعدة:

```text
APPLICATION MUST FIT FOUNDATION
FOUNDATION MUST NOT CHANGE TO FIT APPLICATION
```

بعد الـLive Seal، البرنامج الجديد لا يحصل على:

- Foundation patch؛
- special case؛
- name/version allowlist؛
- weaker admission/lifecycle/security/resource rule؛
- Foundation-directed FCR لمجرد أنه لا يناسب العقد الحالي.

إذا لم يقدر البرنامج على التوافق، النتيجة تكون واحدة من:

```text
READY_FOR_FOUNDATION_ADMISSION
APPLICATION_REDESIGN_REQUIRED
INCOMPATIBLE_WITH_SEALED_FOUNDATION
```

ولا يوجد خيار اسمه:

`CHANGE_FOUNDATION_FOR_THIS_APPLICATION`

للتطبيق العملي استخدم:

- `FALCON_NEW_APPLICATION_ONBOARDING_MANUAL_AR.md`
- `FALCON_NEW_APPLICATION_ONBOARDING_MANUAL_EN.md`

---

# 7. هل Foundation جاهزة؟

على الـvalidated executable baseline:

```text
FULL_FOUNDATION_VALIDATION = PASS
GOVERNED_VERIFIERS = 88/88 PASS
UNKNOWN_APPLICATION = 42/42 PASS
POST_FIX_RED_TEAM_ACTIONABLE_FINDINGS = 0
FOUNDATION_READY_FOR_APPLICATION_CONSUMPTION = YES
FOUNDATION_REQUIRES_STAGE17 = NO
```

هذا يعني أنه لا يوجد نقص تنفيذي معروف داخل Foundation نفسها يمنعها من أن تكون generic hosting substrate.

لكن هذا لا يعني تلقائيًا:

```text
FALCON_LIVE = YES
APPLICATION_ACTIVATION = AUTHORIZED
PRODUCTION_DEPLOYMENT = AUTHORIZED
PROVIDER_CONNECTIVITY = ACTIVE
BROKER_CONNECTIVITY = ACTIVE
LIVE_TRADING = AUTHORIZED
```

كل هذه قرارات منفصلة.

---

# 8. حالة التوافق قبل Live Seal

Application العادي أنهى FCR-0252 وتم التحقق من توافقه مع FDN-006/FDN-007 على exact current HEAD بدون أي تعديل مطلوب في Foundation.

Shared Web ما زال مسؤولًا عن إكمال FCR-0253 داخل نطاق Web نفسه. أي gap متبقٍ عند Web هو Web-owned ولا يتحول إلى Foundation patch فقط لتسهيل التوافق.

Foundation نفسها لا تعتمد على Web وتبقى valid مع zero Applications.

---

# 9. قاموس سريع

**Admission:** قبول Foundation لمرشح مستوفٍ للشروط. لا يعني Activation.  
**Runtime Registration:** تسجيل تقني في hosting. لا يعني Activation.  
**Authority:** صلاحية صريحة ومحكومة لفعل شيء.  
**Artifact:** مخرج/عقد منشور بهوية ونسخة وبصمة ودليل محدد.  
**FIL:** لغة وحدود الاتصال المحكومة داخل Falcon.  
**FSA:** Foundation Self-Awareness، فقط على مستوى Foundation/OS.  
**MSA/LSA/CSA:** مستويات وعي داخل Applications.  
**Guardian:** حماية واحتواء تقني مستقل.  
**Fail closed:** عند الغموض أو نقص الدليل تكون النتيجة منع/hold وليس سماحًا.  
**Live Seal:** حالة تشغيلية تصبح فيها Foundation substrate ثابتة منشورة ويجب على البرامج الجديدة التكيف معها.

---

# 10. المراجع الحاكمة

- Falcon Vision.
- Falcon Constitution.
- `docs/03_DOCUMENT_AUTHORITY.md`.
- `docs/development/FOUNDATION_WORKSTREAM_RULES.md`.
- `docs/foundation/FDN-006_APPLICATION_INTEGRATION_AND_ADMISSION_PROFILE.md`.
- `docs/foundation/FDN-007_LIVE_FOUNDATION_SEAL_AND_FUTURE_APPLICATION_ONBOARDING_POLICY.md`.
- CON-023 والعقود/ADRs المعتمدة ذات الصلة.
- `docs/canonical-records/owner-decisions/`.
- `src/Foundation.*`.
- `verification/` و`tests/`.

**القاعدة النهائية:** Foundation تعرف كيف تستضيف Application محكومة بدون أن تعرف business domain الخاص بها مسبقًا. Application هي التي تثبت هويتها ومتطلباتها وأدلتها ومواردها وصلاحياتها، ولا تتحول النجاحات التقنية إلى authority ضمنيًا.