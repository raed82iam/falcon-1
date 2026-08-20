# دليل الربط الفعلي بين FSATS وFalcon Foundation وShared Web وترتيب الملفات على الجهاز

**الإصدار:** 2026-08-19  
**الجمهور:** Project Owner، مهندس التكامل، مطور FSATS، مهندس Shared Web، ومهندس التشغيل الذي سيجهز الجهاز قبل الربط الفعلي.  
**النطاق:** ترتيب ملفات Falcon على الجهاز وفهم مسارات الربط الفعلية بين Falcon Foundation وFSATS وShared Falcon Web Application.  
**حالة FSATS الحالية:** `FULL_PLUG_READY_PREFLIGHT = VERIFIED_BY_COMPOSITION`، لكن `ACTUAL_ADMISSION` و`ACTUAL_CANONICAL_RUNTIME_REGISTRATION` و`RUNTIME_ACTIVATION` ما زالت غير مصرح بها وغير منفذة.  
**مرجع FSATS الحالي:** FCR-0254.  

> هذا الدليل يشرح كيف نرتب الجهاز وكيف ستتم الروابط عندما يعطي Project Owner صلاحية التنفيذ الفعلي. لا يعتبر هذا الملف وحده صلاحية لـAdmission أو Runtime Registration أو Activation أو Deployment أو Provider/Broker connectivity أو Paper/Live.

---

# 1. الصورة الكاملة

Falcon على الجهاز يجب أن يبقى مكوّنًا من وحدات منفصلة، وليس source tree واحدًا نخلط فيه كل شيء:

```text
FALCON FOUNDATION
    = المنصة التقنية والحَوْكمة والاستضافة والسلطات التقنية العامة

FSATS
    = نظام التداول المكوّن من خمس Applications مستقلة

SHARED FALCON WEB APPLICATION
    = Shared Application مستقلة لواجهة المستخدم والعرض والتفاعل
```

القاعدة:

```text
FOUNDATION SOURCE -/-> FSATS SOURCE TREE
FOUNDATION SOURCE -/-> WEB SOURCE TREE
FSATS SOURCE       -/-> FOUNDATION SOURCE TREE
WEB SOURCE         -/-> FOUNDATION SOURCE TREE
WEB SOURCE         -/-> FSATS SOURCE TREE
FSATS SOURCE       -/-> WEB SOURCE TREE
```

الربط الحقيقي يتم بالعقود والـManifests والـevidence والـroutes والـruntime bindings، وليس بنسخ source files بين المجلدات.

---

# 2. الروابط الثلاثة التي يجب فهمها

لدينا ثلاث علاقات مستقلة:

## 2.1 FSATS ↔ Foundation

```text
FSATS APPLICATION
    -> MANIFEST
    -> FOUNDATION VALIDATION
    -> ADMISSION
    -> EXACT ARTIFACT / LIFECYCLE / RESOURCE BINDINGS
    -> RUNTIME REGISTRATION
    -> SEPARATE ACTIVATION AUTHORITY
```

## 2.2 Shared Web ↔ Foundation

Shared Web هي Falcon Application/Shared Application مستقلة وتستهلك Foundation public/governed contracts عند الحاجة.

المسار المنطقي:

```text
SHARED WEB
    -> WEB MANIFEST / GOVERNED DECLARATIONS
    -> FOUNDATION PUBLIC CONTRACTS / ROUTES
    -> GOVERNED RUNTIME BINDINGS
    -> SEPARATE ACTIVATION / DEPLOYMENT / CONNECTIVITY AUTHORITY
```

لا يجوز أن تعتمد واجهة Web مباشرة على Foundation internals.

## 2.3 Shared Web ↔ FSATS

Web لا تدخل إلى Trading أو FSAPMA أو Guardian internals مباشرة.

المسار المقبول:

```text
FSATS PUBLIC PROJECTION / REQUEST CONTRACT
        |
        v
GOVERNED CROSS-WORKSTREAM CONTRACT / ROUTE
        |
        v
WEB-OWNED ADAPTER / PORT
        |
        v
WEB PRESENTATION / USER INTERACTION
```

والاتجاه العكسي للطلبات:

```text
USER ACTION IN WEB
        |
        v
WEB REQUEST CONTRACT
        |
        v
FSATS OWNED ENDPOINT / APPLICATION DECISION
        |
        v
OUTCOME / PROJECTION BACK TO WEB
```

احفظ دائمًا:

```text
PRESENTATION != AUTHORITY
REQUEST_SENT != ACTION_ACCEPTED != ACTION_COMPLETED
WEB_DISPLAY_DATA != FSATS_OPERATIONAL_TRUTH
WEB_PROVIDER_ROUTE != FSAPMA_PROVIDER_ROUTE
```

---

# 3. الوضع الحالي قبل تنزيل كل شيء على الجهاز

بالنسبة لـFSATS، الحالة الموثقة هي:

```text
FOUNDATION_GENERIC_ADMISSION_RUNTIME_PATH   = EXECUTABLE_PROVEN
APPLICATION_EXACT_REQUEST_MATERIALIZATION   = EXECUTABLE_VERIFIED
FOUNDATION_EXACT_STATIC_GATE_RECONCILIATION = PASS_5_OF_5
FULL_PLUG_READY_CONTRACT_PREFLIGHT           = VERIFIED
FULL_PLUG_READY_PREFLIGHT                    = VERIFIED_BY_COMPOSITION
FOUNDATION_CHANGE_REQUIRED                   = FALSE
APPLICATION_REDESIGN_REQUIRED                = FALSE
```

لكن:

```text
ACTUAL_ADMISSION                      = NOT_AUTHORIZED / NOT_EXECUTED
ACTUAL_CANONICAL_RUNTIME_REGISTRATION = NOT_AUTHORIZED / NOT_EXECUTED
RUNTIME_ACTIVATION                    = NOT_AUTHORIZED / NOT_EXECUTED
DEPLOYMENT                            = NOT_AUTHORIZED / NOT_EXECUTED
PROVIDER_BROKER_CONNECTIVITY          = NOT_AUTHORIZED / NOT_EXECUTED
PAPER_LIVE_BUSINESS_AUTHORITY         = NOT_AUTHORIZED / NOT_EXECUTED
```

Shared Web له workstream وحالة runtime/deployment/connectivity مستقلة، لذلك لا نستنتج حالته من FSATS أو Foundation.

قبل الربط النهائي يجب دائمًا قراءة current `web-development` HEAD وحالة Web/FCRs الخاصة به.

---

# 4. ترتيب الجهاز الموصى به

لأن Foundation وFSATS وWeb موجودون في نفس GitHub repository لكن على branches مختلفة، لا تستخدم working copy واحدة وتبدل branch كل مرة.

استخدم **ثلاثة checkouts مستقلة**:

```text
C:\Falcon\
│
├── Foundation\
│   └── Falcon-Foundation\
│       └── checkout: foundation-development
│
├── Applications\
│   ├── Falcon-FSATS\
│   │   └── checkout: application-development
│   │
│   └── Falcon-Shared-Web\
│       └── checkout: web-development
│
├── Runtime\
│   ├── Foundation\
│   ├── FSATS\
│   ├── Web\
│   ├── Artifacts\
│   ├── Evidence\
│   ├── State\
│   └── Logs\
│
├── Test\
│   ├── Foundation\
│   ├── FSATS\
│   └── Web\
│
└── Backups\
```

هذا layout موصى به لإدارة الجهاز، وليس contract ملزم داخل Foundation.

```text
RECOMMENDED_WINDOWS_LAYOUT != FOUNDATION_CONTRACT_REQUIREMENT
```

---

# 5. Foundation checkout

المكان المقترح:

```text
C:\Falcon\Foundation\Falcon-Foundation\
```

المصدر:

```text
Repository: raed82iam/Falcon
Branch: foundation-development
```

المناطق المهمة تبقى كما هي:

```text
src/Foundation.Admission/
src/Foundation.ApplicationManifest/
src/Foundation.ContractRegistry/
src/Foundation.ApplicationLifecycle/
src/Foundation.ArtifactPublication/
src/Foundation.ApplicationRuntimeHosting/
src/Foundation.Authority/
src/Foundation.Evidence/
src/Foundation.Contracts/
src/Foundation.SelfAwareness/
src/Foundation.Guardian/
src/Foundation.IdentityRuntime/
verification/
tests/
docs/foundation/
docs/manuals/
```

لا تنسخ FSATS أو Web إلى `src/Foundation.*`.

---

# 6. FSATS checkout

المكان المقترح:

```text
C:\Falcon\Applications\Falcon-FSATS\
```

المصدر:

```text
Repository: raed82iam/Falcon
Branch: application-development
```

المناطق المهمة:

```text
applications/FSATS/
applications/docs/FSATS/
applications/docs/مانيوال/
applications/ci/
applications/Falcon.Applications.slnx
```

داخل هذا checkout، `applications/shared/web/**` ليس مصدر Web التشغيلي الذي سنبني عليه. Web له checkout مستقل من `web-development`.

---

# 7. Shared Web checkout

المكان المقترح:

```text
C:\Falcon\Applications\Falcon-Shared-Web\
```

المصدر:

```text
Repository: raed82iam/Falcon
Branch: web-development
```

المجال المملوك للـWeb هو:

```text
applications/shared/web/**
```

أهم المناطق داخله تعتمد على البنية الحالية للـWeb، ومنها source وtests وdocs وruntime/presentation contracts الخاصة به.

لا تنسخ Web files داخل FSATS ولا داخل Foundation.

Shared Web تظل Application مستقلة وقابلة للاستبدال، وليست UI ملتصقة بـFoundation internals.

---

# 8. تطبيقات FSATS الخمسة

FSATS نفسه boundary غير مالك وغير runtime entity مستقل.

الـruntime onboarding يكون لهذه الخمس Applications:

```text
1. FSATS-TRADING
2. FSATS-FSAPMA
3. FSATS-TRADING-GUARDIAN
4. FSATS-FSTSIMA
5. APP-RSC
```

التوبولوجي الحالي:

```text
Trading:          MSA=1 / LSA=13 / CSA=3
FSAPMA:           MSA=1 / LSA=6  / CSA=1
Trading Guardian: MSA=1 / LSA=4  / CSA=1
FSTSimA:          MSA=1 / LSA=8  / CSA=2
APP-RSC:          MSA=1 / LSA=3  / CSA=0 initially

TOTAL = 5 Applications / 5 MSA / 34 LSA / 7 CSA
```

كل واحدة تحصل على admission/runtime identity مستقلة.

Shared Web لا تصبح Application سادسة داخل FSATS. هي Shared Falcon Application مستقلة خارج FSATS domain boundary.

---

# 9. ملفات FSATS التحضيرية للربط

كل FSATS Application لديها `FoundationOnboardingDeclaration.cs` خاص بها:

```text
applications/FSATS/src/Trading/Falcon.FSATS.Trading.Application/FoundationOnboardingDeclaration.cs
applications/FSATS/src/FSAPMA/Falcon.FSATS.FSAPMA.Application/FoundationOnboardingDeclaration.cs
applications/FSATS/src/TradingGuardian/Falcon.FSATS.TradingGuardian.Application/FoundationOnboardingDeclaration.cs
applications/FSATS/src/FSTSimA/Falcon.FSATS.FSTSimA.Application/FoundationOnboardingDeclaration.cs
applications/FSATS/src/ResourceManagement/Falcon.FSATS.ResourceManagement.Application/FoundationOnboardingDeclaration.cs
```

FCR-0254 materialization موجودة في:

```text
applications/FSATS/tests/FoundationCompatibility/
Falcon.FSATS.FoundationOnboarding.Verifier/
Fcr0254CandidateCatalog.cs
```

والـverifier:

```text
applications/FSATS/tests/FoundationCompatibility/
Falcon.FSATS.FoundationOnboarding.Verifier/
Fcr0254CandidateCatalogVerifier.cs
```

هذه تبقى Application-owned ولا تنسخ إلى Foundation.

---

# 10. Shared Web integration files

لا نثبت أسماء ملفات Web runtime النهائية هنا كأنها immutable، لأن Web workstream ما زال يتغير مستقلاً.

عند تجهيز الجهاز النهائي نقرأ current Web HEAD ثم نحدد exact current:

```text
Web Manifest / admission declaration
Web public ports/contracts
Web presentation/runtime adapters
Web Foundation binding profile
Web FSATS cross-workstream contracts
Web provider presentation route policy
Web tests/checks
```

المرجع هو `web-development` current source، وليس نسخة قديمة من Web موجودة داخل checkout آخر.

---

# 11. كيف يرتبط Web بـFSATS بدون كسر الحدود؟

Web مسؤول عن العرض والتفاعل، بينما FSATS مسؤول عن trading-domain truth والقرارات التشغيلية الخاصة به.

مثال صحيح:

```text
Trading / Guardian / FSAPMA / APP-RSC
        |
        | governed projection / response
        v
Cross-workstream contract
        |
        v
Shared Web adapter
        |
        v
Screen / dashboard / notification
```

وللطلبات:

```text
User clicks action
        |
        v
Web creates governed request
        |
        v
FSATS owning Application evaluates
        |
        +--> ACCEPTED / DENIED / FAILED / PENDING
        |
        v
Web displays exact outcome
```

ممنوع:

```text
Web UI -> direct Trading internal method
Web UI -> direct Guardian internal state mutation
Web UI -> direct APP-RSC internal resource mutation
Web UI -> direct Foundation internal authority method
```

---

# 12. Provider data: Web مقابل FSAPMA

هذه نقطة مهمة أثناء ترتيب runtime/config على الجهاز.

Shared Web قد يكون له presentation-only provider connectivity خاص به.

FSAPMA له operational provider integration الخاص بـFSATS.

لا تخلط credentials أو routes أو state بينهم.

```text
WEB_PROVIDER_ROUTE != FSAPMA_PROVIDER_ROUTE
WEB_PROVIDER_CREDENTIAL != FSAPMA_PROVIDER_CREDENTIAL
WEB_PRESENTATION_DATA != FSATS_OPERATIONAL_DATA
WEB_RAW_DISPLAY_DATA -/-> FSATS_ANALYSIS_INPUT
```

لذلك إذا أنشأنا folders/configs مستقبلًا، يجب فصل Web provider bindings عن FSAPMA provider bindings.

---

# 13. Broker account والـWeb

FSATS لا يملك customer/user identity.

الـTrading operating subject هو Broker Account:

```text
BrokerId + BrokerAccountId
```

Shared Web يملك broker-account-to-customer/user/contact mapping حسب العقد الحاكم.

لذلك لا نضع customer mapping داخل Trading source أو Foundation لمجرد تسهيل UI.

---

# 14. runtime-current FSATS bindings التي لا يجوز اختلاقها

عند actual FSATS Registration، هذه القيم تأتي من authoritative Foundation sources وقت العملية:

```text
EXACT_STAGE14_ARTIFACT_IDENTITY
POSITIVE_CANONICAL_ADMISSION_EVIDENCE
LIFECYCLE_ATTACH_ELIGIBILITY_AND_DECISION_IDENTITY
CURRENT_FOUNDATION_RESOURCE_GRANTS
AUTHORITATIVE_OBSERVED_AT
```

القاعدة:

```text
RUNTIME_CURRENT_EVIDENCE = BIND_AT_OPERATION
```

نفس المبدأ يطبق على أي Web runtime-current Foundation binding: لا نخمن authority/evidence من source readiness.

---

# 15. build outputs والـArtifacts

لا تنقل DLLs أو Web build outputs يدويًا إلى Foundation source tree.

```text
BUILD_OUTPUT_EXISTS != CANONICAL_ARTIFACT
BRANCH_HEAD != RUNTIME_ARTIFACT_IDENTITY
COPY_TO_FOLDER != FOUNDATION_CONSUMPTION
```

مجلد staging محلي ممكن يكون:

```text
C:\Falcon\Runtime\Artifacts\
    Foundation\
    FSATS\
    Web\
```

لكن هذا تنظيم محلي، والـcanonical identity/evidence هي الحاكمة عند runtime binding.

---

# 16. Evidence والـLogs

افصل evidence التشغيلية عن source:

```text
C:\Falcon\Runtime\Evidence\Foundation\
C:\Falcon\Runtime\Evidence\FSATS\
C:\Falcon\Runtime\Evidence\Web\

C:\Falcon\Runtime\Logs\Foundation\
C:\Falcon\Runtime\Logs\FSATS\
C:\Falcon\Runtime\Logs\Web\
```

أي evidence حاكمة يجب أن تكون:

```text
exact
identity-bound
version-bound
attributable
current where required
reconstructable
```

وجود ملف local لوحده لا يجعله canonical evidence.

---

# 17. Secrets وCredentials

لا تخزن:

```text
API secrets
passwords
private keys
provider tokens
broker secrets
```

داخل ordinary source/config/state.

القاعدة المشتركة:

```text
CREDENTIAL_REFERENCE_ID != SECRET_BYTES
```

ويجب أيضًا فصل credential references حسب المالك:

```text
WEB PROVIDER CREDENTIAL REFERENCE
FSAPMA PROVIDER CREDENTIAL REFERENCE
BROKER CREDENTIAL REFERENCE
```

ولا يعاد استخدام واحد كأنه الآخر.

---

# 18. ترتيب FSATS actual Admission/Registration عندما يأذن Owner

## 18.1 Freeze exact inputs

```text
current application-development HEAD
current foundation-development HEAD
current FCR-0254
APP-001 / CON-023 / FDN-006 / FDN-007
five FSATS manifests
candidate catalog
```

## 18.2 Build / exact artifact identity

تحديد exact accepted artifact لكل Application.

## 18.3 Admission

كل Application بشكل مستقل.

## 18.4 Lifecycle Attach

current eligibility لنفس identity/version.

## 18.5 Resource Grants

current Foundation grants لنفس Application identity.

## 18.6 Runtime Registration

```text
ApplicationRuntimeHost.Register(...)
```

والنجاح يجب أن ينتج:

```text
RUNTIME_REGISTERED_NOT_ACTIVATED
```

## 18.7 Stop

لا Activation تلقائي.

---

# 19. ترتيب Web runtime binding عندما يأذن Owner

Web لها gate مستقلة عن FSATS.

عند الوصول إليها:

```text
fresh web-development HEAD
fresh Foundation HEAD
current Web Manifest/admission declarations
current Web FCR states
current Web browser/runtime validation
current authoritative identity/credential references
current Foundation route/runtime evidence
```

ثم يتم الربط فقط وفق عقود Web الحالية.

نجاح FSATS Registration لا يعني Web Registered، والعكس صحيح.

```text
FSATS_REGISTERED != WEB_REGISTERED
WEB_REGISTERED != FSATS_REGISTERED
WEB_ACTIVE != FSATS_ACTIVE
```

---

# 20. الشكل المنطقي الكامل

```text
                     +---------------------------+
                     |     Falcon Foundation     |
                     | Admission / Runtime Host  |
                     | Authority / Evidence      |
                     +-------------+-------------+
                                   |
                   governed hosting/contracts
                                   |
              +--------------------+--------------------+
              |                                         |
              v                                         v
+-----------------------------+          +-----------------------------+
|            FSATS            |          |      Shared Falcon Web      |
| 5 independent Applications  |          | Shared presentation app     |
| Trading / FSAPMA / Guardian |          | UI / requests / projections |
| FSTSimA / APP-RSC           |          +-------------+---------------+
+--------------+--------------+                        ^
               |                                       |
               | governed domain contracts             |
               +---------------------------------------+
```

لا يوجد سهم يعني ownership transfer.

---

# 21. ما لا تفعله عند تجهيز الجهاز

```text
1. لا تنسخ FSATS داخل Foundation source
2. لا تنسخ Web داخل Foundation source
3. لا تنسخ Foundation داخل FSATS أو Web
4. لا تدمج branches الثلاثة لمجرد التشغيل
5. لا تجعل Web يعتمد مباشرة على FSATS internals
6. لا تجعل Web يعتمد مباشرة على Foundation internals
7. لا تعتبر build = Admission
8. لا تعتبر Admission = Registration
9. لا تعتبر Registration = Activation
10. لا تعتبر Web route = FSAPMA route
11. لا تعيد استخدام credentials بين Web/FSAPMA/Broker
12. لا تستخدم branch HEAD كruntime artifact identity
13. لا تعمل silent upgrade
14. لا تضع secrets في ordinary files
15. لا تعتبر نجاح Web UI دليلًا على نجاح business action
```

---

# 22. Backup قبل الربط

احتفظ قبل أي actual operation بـ:

```text
exact Foundation commit
exact Application commit
exact Web commit
FSATS five Manifest identities/digests
Web Manifest identity/digest
artifact identities/digests
Admission evidence
Lifecycle evidence
Resource grant evidence
Web route/runtime evidence
runtime registration decisions
validation logs
Architecture/Red-Team evidence
```

مجلد محلي مقترح:

```text
C:\Falcon\Backups\<date-and-operation>\
```

---

# 23. الاختبارات قبل الربط

## FSATS

```text
restore/build/test = PASS
Application verifiers = PASS
Foundation onboarding verifier = PASS
FCR-0254 materialization verifier = PASS
cross-branch compatibility = PASS
working tree = CLEAN
Architecture / Consistency = CLEAN
Red Team = CLEAN
```

## Web

يجب استخدام الـcurrent Web verification suite من `web-development` وقتها، بما فيها source/check/browser/runtime gates المطبقة على exact Web HEAD.

لا نعتمد على رقم tests قديم كأنه دائم.

## Foundation

نستخدم current Foundation governed validation/evidence المطلوبة للعملية الفعلية.

---

# 24. بعد Registration

إذا نجحت FSATS Applications في registration، الحالة تكون:

```text
5 FSATS APPLICATIONS = REGISTERED_NOT_ACTIVATED
```

إذا نجحت Web registration بشكل مستقل:

```text
SHARED WEB = REGISTERED_NOT_ACTIVATED
```

ولا يعني أي منهما:

```text
Deployment = YES
Provider Connectivity = YES
Broker Connectivity = YES
Paper = YES
Live = YES
```

كل gate لها Authority منفصلة.

---

# 25. الترتيب النهائي للجهاز

```text
C:\Falcon\
│
├── Foundation\
│   └── Falcon-Foundation\              # foundation-development
│       ├── src\
│       ├── tests\
│       ├── verification\
│       └── docs\
│
├── Applications\
│   ├── Falcon-FSATS\                   # application-development
│   │   └── applications\
│   │       ├── FSATS\
│   │       ├── docs\
│   │       └── ci\
│   │
│   └── Falcon-Shared-Web\              # web-development
│       └── applications\
│           └── shared\
│               └── web\
│
├── Runtime\
│   ├── Foundation\
│   ├── FSATS\
│   ├── Web\
│   ├── Artifacts\
│   ├── Evidence\
│   ├── State\
│   └── Logs\
│
├── Test\
│   ├── Foundation\
│   ├── FSATS\
│   └── Web\
│
└── Backups\
```

---

# 26. ما نحتاجه يوم الربط الفعلي

عندما يعطي Project Owner الصلاحية، نبدأ بـfresh reconciliation للثلاثة:

```text
1. exact current Foundation checkout
2. exact current FSATS/Application checkout
3. exact current Shared Web checkout
4. clean trees
5. supported SDK/runtime/toolchain
6. exact current manifests
7. current FCR states
8. exact artifacts
9. authoritative lifecycle/resource/route evidence
10. authoritative observation time
```

ثم ننفذ فقط الـgates المصرح بها.

---

# 27. الخلاصة

الترتيب الصحيح ليس:

```text
ONE BIG FALCON SOURCE FOLDER
```

بل:

```text
FOUNDATION CHECKOUT
+
FSATS CHECKOUT
+
SHARED WEB CHECKOUT
+
SEPARATE RUNTIME / TEST / EVIDENCE AREAS
```

والربط يكون:

```text
Foundation <-> FSATS
    عبر Manifest / Admission / Artifact / Lifecycle / Resources / Runtime Host

Foundation <-> Shared Web
    عبر Web Manifest / public Foundation contracts / governed runtime routes

FSATS <-> Shared Web
    عبر public domain projections / request-response contracts / Web-owned adapters
```

**القاعدة النهائية:** لا تخلط الملفات حتى تجعل النظام يعمل. رتّب كل workstream في مكانه الصحيح، ودع العقود والأدلة والـruntime bindings هي التي تربط Falcon ببعضه.