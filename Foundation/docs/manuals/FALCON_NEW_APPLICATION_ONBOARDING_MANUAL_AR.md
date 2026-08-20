# دليل إضافة برنامج جديد إلى Falcon Foundation — النسخة العربية

**الفئة المستهدفة:** المهندسون المعماريون، مالكو التطبيقات، المطورون، المراجعون، فرق التشغيل، وفرق التكامل البشرية  
**الهدف:** شرح أدق التفاصيل العملية لإضافة Application جديد إلى Falcon Foundation بعد أن تصبح Live وSealed، بدون تعديل Foundation نفسها.  
**وضع Foundation:** LIVE / SEALED / APPLICATION-NEUTRAL  
**المراجع الحاكمة الأساسية:** Falcon Vision؛ Falcon Constitution؛ APP-001؛ CON-023؛ FDN-006؛ FDN-007؛ والعقود وقواعد التشغيل المعتمدة ذات الصلة.  

---

# 1. لماذا هذا الدليل موجود؟

هذا الدليل مخصص لأي شخص سيأتي مستقبلًا ويريد إضافة برنامج جديد إلى Falcon.

الفكرة الأساسية التي يجب فهمها من البداية هي:

> البرنامج الجديد هو الذي يتكيف مع Falcon Foundation، وليس Falcon Foundation هي التي تتغير من أجل البرنامج الجديد.

بعد أن تصبح Foundation Live وSealed، إضافة Application جديد لا تعتبر تطويرًا للـFoundation.

هي عملية تخص الـApplication نفسه وتشمل:

- التصميم؛
- التعريف؛
- الـManifest؛
- التحقق؛
- Admission؛
- Runtime Registration؛
- الصلاحيات؛
- الموارد؛
- الأمان؛
- Lifecycle؛
- التشغيل؛
- التحديث؛
- التعافي؛
- الإزالة.

Foundation يجب أن تبقى ثابتة ومحايدة تجاه نوع البرنامج.

يمكن أن يكون البرنامج المستقبلي مثلًا:

- محاسبة؛
- لوجستيات؛
- أبحاث؛
- تقارير؛
- ذكاء اصطناعي؛
- اتصالات؛
- إدارة مستندات؛
- مراقبة؛
- برنامج طبي؛
- برنامج مالي؛
- إدارة محافظ؛
- Workflow Automation؛
- أو أي مجال جديد لم يكن موجودًا وقت بناء Foundation.

Foundation لا تحتاج أن تعرف من قبل ما هو المجال التجاري للبرنامج.

هي تحتاج فقط أن تعرف وتتحقق من:

- من أنت؟
- ما هي نسختك؟
- من يملكك؟
- ماذا تحتاج؟
- ماذا توفر؟
- ما هي الصلاحيات التي تطلبها؟
- ما هي الموارد التي تحتاجها؟
- كيف تتواصل؟
- كيف تفشل بأمان؟
- كيف يتم تشغيلك؟
- كيف يتم إيقافك؟
- كيف يتم تحديثك؟
- كيف يتم التعافي منك؟
- كيف يتم استبدالك؟
- كيف تتم إزالتك؟
- وما هي الصلاحية الحقيقية التي لديك فعلًا؟

---

# 2. القاعدة التي يجب ألا تُنسى أبدًا

بعد الـLive Seal:

```text
NEW APPLICATION -> MUST ADAPT TO FOUNDATION
FOUNDATION -> MUST NOT ADAPT TO NEW APPLICATION
```

بمعنى أبسط:

**البرنامج الجديد يدخل على النظام الموجود، وليس النظام يتفصّل على مقاس البرنامج الجديد.**

ولهذا يمنع مستقبلًا:

- طلب إضافة اسم البرنامج داخل Foundation؛
- طلب إضافة رقم نسخته إلى allowlist؛
- طلب special case خاص فيه؛
- طلب bypass لشرط Manifest؛
- طلب تخفيف شروط Admission؛
- طلب استثناء أمني؛
- طلب تجاوز lifecycle؛
- طلب موارد بدون governed grant؛
- اعتبار Registration = Activation؛
- السماح للبرنامج أن يعطي نفسه صلاحيات؛
- تخزين secret bytes داخل state عادية؛
- إضافة provider shortcut داخل Foundation؛
- تخفيف fail-closed؛
- إضافة business logic داخل Foundation من أجل البرنامج.

إذا احتاج البرنامج أي واحدة من هذه الأشياء حتى يعمل، فالمشكلة في تصميم البرنامج وليست في Foundation.

---

# 3. النتائج الثلاث الممكنة فقط

في نهاية مراجعة البرنامج الجديد، يجب أن تكون النتيجة واحدة فقط من ثلاث:

## 3.1 READY_FOR_FOUNDATION_ADMISSION

يعني البرنامج مستوفي كل الشروط اللازمة، ويمكن إدخاله إلى مرحلة Admission.

## 3.2 APPLICATION_REDESIGN_REQUIRED

يعني البرنامج نفسه يحتاج تعديل قبل أن يصبح متوافقًا.

أمثلة:

- Manifest ناقص؛
- الموارد غير منطقية؛
- provider adapter مصمم غلط؛
- secrets مخزنة بطريقة غير آمنة؛
- authority boundaries غير واضحة؛
- lifecycle ناقص؛
- التواصل يعمل bypass؛
- MSA/LSA غير واضحة.

## 3.3 INCOMPATIBLE_WITH_SEALED_FOUNDATION

يعني البرنامج لا يستطيع العمل إلا إذا تغيرت Foundation نفسها.

في هذه الحالة البرنامج **غير مناسب** للـFoundation الحالية.

لا يوجد خيار رابع اسمه:

```text
CHANGE_FOUNDATION_FOR_THIS_APPLICATION
```

---

# 4. قبل كتابة أي كود

لا تبدأ من الـbusiness logic.

ابدأ بتعريف البرنامج كـgoverned Application.

يجب أن يكون عندك ملف أو سجل تصميم يحدد على الأقل:

1. اسم البرنامج
2. Application Identity
3. Application Version
4. المالك
5. الهدف
6. Business Boundary
7. ما الذي يملكه
8. ما الذي لا يملكه
9. الفروع الرئيسية
10. MSA
11. LSAs
12. CSAs إن وجدت
13. Providers
14. Foundation dependencies
15. Shared Application dependencies
16. External systems
17. Data ownership
18. Permissions
19. Authority requests
20. Resource needs
21. Failure behavior
22. Recovery behavior
23. Update behavior
24. Replacement behavior
25. Removal behavior

إذا لم تستطع شرح هذه الأشياء بوضوح، فأنت لست جاهزًا لإدخال البرنامج على Foundation.

---

# 5. تعريف هوية البرنامج

كل Application يجب أن تكون له هوية ثابتة وواضحة.

الحد الأدنى:

```text
ApplicationIdentity
ApplicationVersion
ApplicationOwner
ApplicationPurpose
PackageIdentity
PackageVersion
ManifestIdentity
ProvenanceIdentity
```

لا تستخدم أسماء غامضة مثل:

```text
app1
latest
current
newapp
service
team
```

مثال جيد:

```text
ApplicationIdentity = application/accounting/core
ApplicationVersion = 1.0.0
ApplicationOwner = owner:accounting-platform
ApplicationPurpose = enterprise accounting and ledger workflow
PackageIdentity = package/accounting/core
PackageVersion = 1.0.0
ManifestIdentity = manifest/accounting/core/1.0.0
ProvenanceIdentity = provenance/accounting/core/1.0.0
```

المهم ليس المثال نفسه، بل أن تكون الهوية:

- واضحة؛
- ثابتة؛
- attributable؛
- غير قابلة للتبديل الصامت؛
- متطابقة عبر Manifest وAdmission وRuntime evidence.

---

# 6. تحديد حدود البرنامج

اكتب بوضوح ماذا يملك البرنامج وماذا لا يملك.

مثال:

```text
OWNS:
- accounting workflows
- accounting data
- accounting calculations
- accounting provider adapters
- accounting reports

DOES NOT OWN:
- Falcon Kernel
- Foundation admission
- Foundation lifecycle authority
- Foundation FSA
- Foundation Guardian
- Foundation resource governance
- state of another Application
```

هذه الخطوة مهمة جدًا لأنها تمنع تسرب المسؤوليات بين الطبقات.

البرنامج مهما كان مهمًا يبقى Application.

لا يصبح Foundation لأنه يستخدم Foundation.

---

# 7. تصميم الـSelf-Awareness

إذا كان البرنامج Self-Aware، يجب تحديد الهيكل بوضوح.

## 7.1 MSA

يجب أن يكون هناك MSA واحد للتطبيق الرئيسي.

هو المسؤول عن فهم صورة البرنامج كاملة.

لكنه لا يصبح FSA.

## 7.2 LSA

كل major branch يجب أن يكون له LSA مسؤول واحد عندما تكون البنية معتمدة على branch-level awareness.

مثال:

```text
Application MSA
  ├─ Data LSA
  ├─ Provider LSA
  ├─ Reporting LSA
  └─ Execution LSA
```

## 7.3 CSA

CSA تستخدم فقط للمكونات الذكية المؤهلة لذلك.

لا تعمل CSA لكل component بشكل عشوائي.

## 7.4 الحدود

```text
CSA -> LSA -> MSA -> FSA REVIEW
```

FSA تبقى في Foundation فقط.

MSA/LSA/CSA تبقى داخل Application.

---

# 8. الـManifest

الـManifest ليس ملفًا شكليًا.

هو الإعلان الرسمي للبرنامج أمام Foundation.

يجب أن يجيب عن كل الأسئلة التالية.

## 8.1 الهوية

- من أنت؟
- ما نسختك؟
- من يملكك؟
- ما هدفك؟

## 8.2 Package وProvenance

- ما هي الحزمة؟
- ما نسختها؟
- ما الـcontent identity؟
- ما دليل integrity؟
- من أين جاءت؟
- ما دليل provenance؟

## 8.3 Dependencies

حدد:

- Foundation contracts المطلوبة؛
- Foundation specifications المطلوبة؛
- Foundation services المطلوبة؛
- Shared Application capabilities المطلوبة؛
- compatible versions؛
- external dependencies.

## 8.4 Capabilities

لكل capability:

- ما هي؟
- من يملكها؟
- من يستطيع استهلاكها؟
- private أم shared؟
- exclusive أم لا؟

## 8.5 Permissions وAuthority

حدد كل permission وكل authority request.

لا تخلط بينهم.

```text
AUTHORITY_REQUEST != AUTHORITY_GRANT
```

طلب الصلاحية لا يعني امتلاكها.

## 8.6 Security

حدد:

- security profile؛
- trust boundaries؛
- secret handling؛
- provider boundaries؛
- network boundaries؛
- behavior عند invalid identity؛
- behavior عند revoked authority؛
- behavior عند missing credentials.

## 8.7 Resources

حدد:

- minimum resources؛
- normal resources؛
- maximum ceiling؛
- priority؛
- degraded behavior؛
- ما الذي يتوقف أولًا عند نقص الموارد.

## 8.8 Lifecycle

يجب تغطية:

```text
Install
Validate
Admit
Register
Activate
Update
Suspend
Recover
Replace
Remove
```

## 8.9 Health وFailure

حدد:

- كيف يبلغ عن health؛
- متى يصبح degraded؛
- متى يصبح failed؛
- كيف يتم containment؛
- كيف يتم rollback؛
- كيف يتم recovery.

## 8.10 Evidence

حدد:

- ما الأدلة التي تنتج؛
- من يملكها؛
- كيف يتم reconstruct للقرار؛
- كيف نعرف بالضبط أي version تم Admission لها؛
- كيف نعرف أي version تم تسجيلها Runtime.

---

# 9. قواعد Dependencies

ممنوع أن تعلن Dependency غير موجودة وتفترض أن Foundation ستوفرها لاحقًا.

أي required Foundation reference يجب أن تكون موجودة ومعروفة ومقبولة.

لا تنشئ عقد Foundation وهمي من جهة Application.

قبل طلب capability اسأل:

> هل هذه فعلًا مسؤولية Foundation؟

في أغلب الحالات، business-specific functionality يجب أن تبقى داخل Application.

أمثلة:

```text
Tax calculation -> Application
Trading strategy -> Application
Chart rendering -> Web Application
Provider business transformation -> Application
Foundation admission -> Foundation
Kernel lifecycle -> Foundation
Foundation resource governance -> Foundation
```

---

# 10. تصميم الموارد

لا تبنِ برنامجًا يحتاج موارد غير محدودة.

القاعدة التشغيلية:

```text
Allocation <= Quota <= Ceiling
```

حدد مسبقًا:

- minimum viable allocation؛
- normal allocation؛
- maximum ceiling؛
- degraded mode؛
- ماذا يتوقف تحت الضغط؛
- ماذا يجب أن يبقى شغالًا؛
- ما البيانات التي يجب الحفاظ عليها؛
- كيف يمنع الانهيار الكامل.

Resource grant يجب أن تكون للـApplication نفسها، وليس لبرنامج آخر.

---

# 11. تصميم Capabilities

لكل capability سجل:

```text
CapabilityIdentity
Owner
Version
Visibility
Consumers
Exclusivity
RequiredAuthority
```

حدد هل هي:

- private؛
- shared؛
- exclusive؛
- reusable.

ولا تنسَ:

```text
CAPABILITY_AVAILABLE != BUSINESS_AUTHORITY
```

حتى لو استخدمتها عشرة Applications، هذا لا يجعلها Foundation service تلقائيًا.

---

# 12. تصميم الاتصالات

التواصل يجب أن يستخدم المسارات المعتمدة.

ممنوع hidden side channels.

ممنوع اعتبار transport طريقًا لتجاوز authority.

احفظ دائمًا:

```text
MESSAGE_ACCEPTED != BUSINESS_ACTION_AUTHORIZED
REQUEST_TRANSPORT != EXECUTION_TRANSPORT
PUBLIC_PROJECTION != CONTROL_REQUEST
PROJECTION_AVAILABLE != CONTROL_AUTHORITY
```

لكل message أو route حدد:

- producer؛
- consumer؛
- schema؛
- purpose؛
- authority expectation؛
- freshness؛
- expiry؛
- retry؛
- timeout؛
- stale-data handling؛
- evidence.

---

# 13. Providers والاتصال الخارجي

وجود Provider لا يعني وجود صلاحية.

احفظ:

```text
API KEY EXISTS != CONNECTION AUTHORIZED
CONNECTION READY != CONNECTION ACTIVATED
DATA RECEIVED != BUSINESS ACTION AUTHORIZED
BROKER CONNECTED != TRADE AUTHORIZED
```

لكل provider حدد:

- provider identity؛
- purpose؛
- route؛
- authentication؛
- credential reference؛
- quota؛
- rate limit؛
- timeout؛
- retry؛
- stale behavior؛
- authority boundary؛
- allowed data؛
- prohibited data use.

لا تخزن password أو token أو private key أو secret bytes داخل ordinary Application state.

استخدم governed secret/credential references.

---

# 14. تصميم الأمان

قبل Admission يجب أن تعرف:

- ما الموثوق؟
- ما غير الموثوق؟
- من authenticated؟
- من authorized؟
- ما الذي هو reachable فقط؟
- ماذا يحدث إذا أصبح trust مجهولًا؟

القاعدة الأساسية:

```text
UNKNOWN -> DENY
```

أمثلة على الحالات التي يجب أن تفشل مغلقة:

- missing identity؛
- version mismatch؛
- owner substitution؛
- Manifest tampering؛
- provenance mismatch؛
- stale resource grant؛
- revoked authority؛
- unknown dependency؛
- invalid provider state؛
- invalid credential reference؛
- ambiguous runtime authority؛
- duplicate runtime identity.

---

# 15. فهم Lifecycle بطريقة بشرية

لا تعتبر إضافة البرنامج عملية واحدة اسمها Install.

هناك مراحل منفصلة.

## 15.1 Install

الحزمة موجودة.

لا يعني أنها trusted.

## 15.2 Validate

يتم التحقق من declarations والأدلة.

لا يعني Admission.

## 15.3 Admit

Foundation تقبل البرنامج كـgoverned subject.

لا يعني أنه يعمل.

## 15.4 Register

يتم تسجيله داخل runtime hosting.

لا يعني أنه Active.

## 15.5 Activate

تحتاج صلاحية منفصلة.

## 15.6 Update

التحديث المادي يحتاج versioning وrevalidation حسب الحاجة.

## 15.7 Suspend

يوقف النشاط الطبيعي مع الحفاظ على الحالة والأدلة المطلوبة.

## 15.8 Recover

يتم التعافي ضمن خطة وصلاحيات واضحة.

## 15.9 Replace

الاستبدال ليس overwrite صامتًا.

## 15.10 Remove

تتم الإزالة بدون تدمير Foundation أو التطبيقات الأخرى.

---

# 16. أهم الفواصل في الصلاحيات

كل شخص يعمل على onboarding يجب أن يفهم هذه الأسطر:

```text
VALID_CONTRACT != ADMISSION
ADMISSION != ACTIVATION
ADMISSION != DEPLOYMENT_AUTHORITY
ADMISSION != BUSINESS_AUTHORITY
TECHNICAL_CONSUMPTION != RUNTIME_AUTHORITY
RUNTIME_REGISTRATION != ACTIVATION
RUNTIME_REGISTRATION != DEPLOYMENT_AUTHORITY
RUNTIME_REGISTRATION != PRODUCTION_AUTHORITY
RUNTIME_REGISTRATION != BUSINESS_AUTHORITY
ROUTE_EXISTS != CONNECTION_AUTHORIZED
PROVIDER_CONNECTIVITY != EXECUTION_AUTHORITY
DATA_ACCESS != BUSINESS_AUTHORITY
REQUEST_SENT != ACTION_ACCEPTED != ACTION_COMPLETED
CREDENTIAL_REFERENCE_ID != SECRET_BYTES
```

إذا الفريق لا يفهمها، لا تكمل onboarding.

---

# 17. تجهيز Admission Package

قبل Admission جهز package كامل يحتوي:

1. Application identity
2. Application version
3. Application owner
4. package identity
5. package version
6. Manifest
7. Manifest integrity/digest evidence
8. provenance
9. dependency list
10. Foundation contract references
11. Foundation specification references
12. service declarations
13. capability declarations
14. permission declarations
15. authority requests
16. security profile
17. provider boundaries
18. resource requirements
19. lifecycle declarations
20. health declarations
21. failure behavior
22. rollback plan
23. MSA/LSA/CSA declarations
24. test evidence
25. reviewer evidence

ممنوع الاعتماد على أن Foundation ستخمن القيم الناقصة.

---

# 18. المراجعة البشرية قبل Admission

اعمل مراجعة Architecture كاملة.

## Identity

- هل الهوية ثابتة؟
- هل owner واضح؟
- هل version واضحة؟

## Architecture

- هل هذا فعلًا Application؟
- هل تسربت مسؤولية من Foundation إلى Application؟
- هل حاول Application أخذ مسؤولية Foundation؟

## Authority

- هل technical capability تم تفسيرها كصلاحية؟
- هل route تم تفسيرها كصلاحية؟
- هل credential تم تفسيرها كصلاحية؟

## Resources

- هل الموارد bounded؟
- هل degraded mode موجود؟

## Security

- هل secrets يتم التعامل معها صح؟
- هل unknown trust يؤدي إلى deny؟

## Lifecycle

- هل يمكن update بدون silent upgrade؟
- هل يمكن suspend؟
- recover؟
- replace؟
- remove؟

## Isolation

- هل يستطيع البرنامج أن يفشل بدون إسقاط Foundation؟
- هل يستطيع أن يفشل بدون إسقاط Application ثانية؟

## Awareness

- هل FSA بقيت في Foundation؟
- هل MSA/LSA/CSA بقيت داخل Application؟

أي إجابة غير واضحة تعني أن التصميم يحتاج تعديل قبل Admission.

---

# 19. ماذا تتحقق Foundation أثناء Admission؟

Foundation تتحقق من أشياء مثل:

- admission kind؛
- required identity؛
- Manifest binding؛
- Manifest digest؛
- provenance؛
- bootstrap context؛
- provider boundary؛
- canonical contract linkage؛
- dependencies؛
- specifications؛
- permissions؛
- authority requests؛
- deterministic evidence.

إذا فشل أي شيء، صحح البرنامج.

لا تعدل Foundation.

---

# 20. Runtime Registration

بعد Admission ما زال في Gate منفصل.

تأكد من:

- runtime instance identity unique؛
- Application identity exact؛
- version exact؛
- artifact binding exact؛
- admission binding exact؛
- lifecycle attach valid؛
- resource grants valid؛
- capabilities valid؛
- no exclusive capability conflict؛
- registration result = Registered only.

بعدها اسأل:

```text
REGISTERED? YES
ACTIVE? ONLY IF SEPARATELY AUTHORIZED
```

---

# 21. مراجعة Activation

قبل Activation تحقق من:

- subject الصحيح؛
- version الصحيحة؛
- action الصحيح؛
- authority الحالية؛
- authority غير revoked؛
- lifecycle صحيح؛
- resources صحيحة؛
- security صحيحة؛
- لا يوجد restriction يمنع التشغيل؛
- evidence كاملة.

ممنوع Activation فقط لأن:

- التست نجح؛
- package موجودة؛
- developer قال جاهز؛
- provider رد؛
- route اشتغلت؛
- owner لم يعترض؛
- النسخة القديمة كانت تعمل.

---

# 22. تحديث البرنامج

عند أي material update:

1. أعطِ version جديدة؛
2. حدّث package identity/version؛
3. حدّث Manifest؛
4. أعد integrity evidence؛
5. أعد provenance evidence؛
6. راجع dependencies؛
7. راجع Foundation references؛
8. راجع permissions؛
9. راجع authority requests؛
10. راجع resources؛
11. راجع security؛
12. راجع provider boundaries؛
13. أعد الاختبارات؛
14. أعد Admission/revalidation حسب المطلوب؛
15. احصل على runtime/lifecycle authority الجديدة عند الحاجة؛
16. احتفظ rollback؛
17. لا تمسح evidence القديمة.

ممنوع silent upgrade.

---

# 23. إزالة البرنامج

البرنامج الجيد يجب أن يكون قابلًا للإزالة.

قبل الإزالة:

- أوقف new work؛
- احفظ evidence؛
- reconcile state؛
- حرر resources؛
- revoke Application authority؛
- revoke secret access؛
- detach runtime بشكل governed؛
- لا تحذف shared dependency تخص برامج أخرى؛
- تأكد Foundation healthy؛
- تأكد Applications الأخرى healthy.

حتى لو كان هذا آخر Application، Foundation يجب أن تبقى صحيحة بنيويًا.

---

# 24. إذا احتاج البرنامج شيئًا غير موجود في Foundation

هذه أهم فقرة في المانيوال.

بعد Live Seal لا يوجد Foundation FCR لتغيير Foundation من أجل البرنامج الجديد.

استخدم هذا القرار:

```text
هل الحاجة Business-specific؟
    نعم -> ضعها داخل Application إذا المعمارية تسمح.

هل يوجد Shared Application معتمد يقدمها؟
    نعم -> استخدمه حسب عقده.

هل يمكن Application-side Adapter يحولها إلى contract موجود؟
    نعم -> ابنِ Adapter داخل جهة Application.

هل يمكن إعادة تصميم feature أو حذفها؟
    نعم -> أعد تصميمها أو احذفها.

هل ما زال البرنامج يحتاج تغيير Foundation؟
    نعم -> INCOMPATIBLE_WITH_SEALED_FOUNDATION
```

Foundation لا تتغير.

---

# 25. الأخطاء الأكثر شيوعًا

## الخطأ 1: "برنامجنا خاص"

بالنسبة للـFoundation، لا يوجد Application خاصة.

## الخطأ 2: "الـAPI شغالة، إذًا عندنا authority"

غلط.

## الخطأ 3: "تم Registration، شغله"

غلط.

## الخطأ 4: "نحتاج secret، خلينا نحطه في config"

غلط إذا كان secret bytes داخل ordinary state.

## الخطأ 5: "نضيف استثناء صغير جدًا في Foundation"

ممنوع بعد الـLive Seal.

## الخطأ 6: "Capability تستخدمها برامج كثيرة، نخليها Foundation"

الاستخدام المتكرر لا يغير الملكية المعمارية تلقائيًا.

## الخطأ 7: "Unknown يعني غالبًا مسموح"

غلط.

```text
UNKNOWN -> DENY
```

## الخطأ 8: "التحديث backward-compatible، لا نحتاج version جديدة"

غلط إذا كان material update حسب القواعد الحاكمة.

---

# 26. Checklist بشرية كاملة

قبل إعلان البرنامج جاهز، يجب أن تكون كل النقاط الإلزامية المطبقة = YES.

## Identity

- [ ] Application identity واضحة.
- [ ] Application version واضحة.
- [ ] owner واضح.
- [ ] purpose واضح.
- [ ] package identity/version واضحة.
- [ ] Manifest identity واضحة.
- [ ] provenance identity واضحة.

## Architecture

- [ ] Application boundary موثقة.
- [ ] ما لا يملكه البرنامج موثق.
- [ ] Foundation responsibilities لم تتسرب إليه.
- [ ] MSA محددة.
- [ ] LSAs محددة عند الحاجة.
- [ ] CSA فقط للمكونات المؤهلة.
- [ ] FSA بقيت خارج Application.

## Manifest

- [ ] dependencies declared.
- [ ] Foundation contracts declared.
- [ ] specifications declared.
- [ ] services declared.
- [ ] capabilities declared.
- [ ] consumers declared.
- [ ] permissions declared.
- [ ] authority requests declared.
- [ ] security profile declared.
- [ ] resource profile declared.
- [ ] lifecycle declared.
- [ ] health declared.
- [ ] failure behavior declared.
- [ ] rollback declared.

## Integrity

- [ ] Manifest content exact.
- [ ] Manifest digest exact.
- [ ] provenance attributable.
- [ ] provenance digest exact.

## Providers وSecrets

- [ ] provider boundary declared.
- [ ] لا يوجد bypass.
- [ ] credential references صحيحة.
- [ ] secret bytes ليست في ordinary state.

## Resources

- [ ] minimums محددة.
- [ ] quota مفهومة.
- [ ] ceiling محترمة.
- [ ] degraded mode موجود.

## Capabilities

- [ ] provided capabilities valid.
- [ ] required capabilities valid.
- [ ] visibility valid.
- [ ] exclusivity checked.

## Admission

- [ ] Foundation references تحل بشكل canonical.
- [ ] dependencies resolve.
- [ ] identity/version/owner تطابق Manifest.
- [ ] admission evidence ناجحة.

## Runtime

- [ ] runtime identity unique.
- [ ] artifact binding exact.
- [ ] admission binding exact.
- [ ] lifecycle attach valid.
- [ ] resource grant valid.
- [ ] registration = Registered only.

## Authority

- [ ] activation separately authorized.
- [ ] deployment separately authorized عند الحاجة.
- [ ] production separately authorized عند الحاجة.
- [ ] business authority separately governed.
- [ ] لا يوجد self-granted authority.

## Isolation وRecovery

- [ ] failure contained.
- [ ] Applications الأخرى معزولة.
- [ ] Foundation تبقى صحيحة بدون البرنامج.
- [ ] recovery plan موجودة.
- [ ] removal plan موجودة.

## Live Seal

- [ ] لا يحتاج تعديل Foundation.
- [ ] لا يحتاج special case في Foundation.
- [ ] لا يحتاج Foundation-directed FCR.
- [ ] يستطيع العمل بالكامل عبر published Foundation contracts.

إذا كل شيء المطلوب = YES، تصبح النتيجة:

```text
READY_FOR_FOUNDATION_ADMISSION
```

---

# 27. Template لسجل Onboarding

استخدم سجلًا مشابهًا لهذا:

```text
APPLICATION ONBOARDING RECORD

Application Identity:
Application Version:
Application Owner:
Application Purpose:
Package Identity:
Package Version:
Manifest Identity:
Provenance Identity:

Major Branches:
MSA:
LSAs:
CSAs:

Foundation Contracts Required:
Foundation Specifications Required:
Foundation Services Required:
Shared Application Capabilities Required:
External Providers:

Permissions Requested:
Authorities Requested:
Resource Minimums:
Resource Ceilings:
Security Profile:

Failure Mode:
Degraded Mode:
Recovery Plan:
Rollback Plan:
Removal Plan:

Manifest Verification: PASS / FAIL
Provenance Verification: PASS / FAIL
Dependency Resolution: PASS / FAIL
Security Review: PASS / FAIL
Resource Review: PASS / FAIL
Capability Review: PASS / FAIL
Admission Result: PASS / FAIL / NOT RUN
Runtime Registration Result: PASS / FAIL / NOT RUN
Activation Authority: PRESENT / ABSENT / NOT APPLICABLE

Foundation Modification Required: NO
Foundation Special Case Required: NO
Foundation FCR Required: NO

Final Human Classification:
READY_FOR_FOUNDATION_ADMISSION
or
APPLICATION_REDESIGN_REQUIRED
or
INCOMPATIBLE_WITH_SEALED_FOUNDATION

Reviewer:
Date:
Evidence References:
```

---

# 28. شرح نهائي بسيط جدًا

إضافة برنامج جديد إلى Falcon لا تعني نسخ كوده داخل Foundation.

البرنامج يبقى Application مستقل.

هو يقدم للـFoundation:

- هويته؛
- نسخته؛
- Manifest؛
- الأدلة؛
- dependencies؛
- capabilities؛
- resource needs؛
- security profile؛
- lifecycle؛
- authority requests.

Foundation تتحقق منه باستخدام نفس القواعد العامة لأي Application.

نوع البرنامج قد يتغير كل سنة.

لكن عقد Foundation لا يتغير من أجله.

القاعدة النهائية الدائمة:

> صمم البرنامج الجديد ليتوافق مع Falcon Foundation. لا تعدل Falcon Foundation الحية لكي تتوافق مع البرنامج الجديد.
