# Shared Falcon Web - استعادة وتحديث API Key و Secret بأمان

**Status:** `DRAFT / DISCUSSION ONLY`

هذا الملف يكمّل تخطيط الحوادث الموجهة وإرشاد الحوادث حسب البروكر. وهو يحدد UX مملوكًا للـShared Web عندما يطلب الـApplication semanticًا تجاريًا يفيد بأن بيانات اتصال البروكر تحتاج renewal / replacement / revalidation.

لا يمنح هذا الملف credential authority أو trading authority أو runtime implementation authority.

---

## 1. القاعدة الأساسية

إذا كانت المشكلة تحتاج تحديث `API Key` أو `Secret`، فالـShared Web لا يطلب من المستخدم كتابة السر في الشات أو التنبيه ولا يخترع خطوات من الذاكرة.

المسار:

```text
APPLICATION IDENTIFIES CREDENTIAL-RECOVERY NEED
-> WEB RESOLVES EXACT BROKER / ACCOUNT / ENVIRONMENT
-> WEB VERIFIES CURRENT BROKER-SPECIFIC OFFICIAL GUIDANCE WHEN PRECISE NAVIGATION IS NEEDED
-> WEB GUIDES USER STEP BY STEP
-> USER ENTERS SECRET ONLY IN GOVERNED SECURE FALCON CREDENTIAL FORM OUTSIDE CHAT
-> NEW CREDENTIAL IS STAGED AS PENDING VALIDATION
-> APPLICATION VALIDATES THE EXACT ACCOUNT/ENVIRONMENT BINDING
-> ONLY AFTER AUTHORITATIVE SUCCESS IS THE NEW CREDENTIAL PROMOTED
-> OLD STORED CREDENTIAL IS THEN SECURELY REMOVED UNDER THE GOVERNED CREDENTIAL LIFECYCLE
-> APPLICATION RECONCILIATION
-> RELEASE ONLY WHEN AUTHORITATIVELY ALLOWED
```

---

## 2. قبل تدوير أو إنشاء Key جديد

قبل توجيه المستخدم لإجراء destructive أو irreversible مثل revoke/rotate، يتحقق الـSmart Web من السياق الحالي للبروكر عند الحاجة، بما يشمل قدر الإمكان من المصادر الرسمية الحالية:

- صفحة API / Developer / Credential Management الرسمية.
- هل الحساب Live أم Paper/Sandbox.
- permissions/scopes المطلوبة.
- IP allowlist أو restrictions إن وجدت.
- هل الـSecret يظهر مرة واحدة فقط.
- هل إنشاء Key جديد يلغي القديم فورًا أم يسمح بالتوازي.
- أثر revoke/rotation على الاتصال الحالي.
- وجود outage عام أو مشكلة broker-wide قد تجعل تدوير المفتاح إجراءً غير مناسب.

إذا لم يمكن التحقق من مسار واجهة البروكر الحالي، لا يخترع Web اسم زر أو مكانًا دقيقًا.

```text
UNKNOWN BROKER UI PATH != SAFE TO INVENT
```

---

## 3. لغة المستخدم أثناء الحادث

لا نرعب المستخدم برسائل تقنية أو أمنية متراكمة.

مثال بشري:

`يبدو أن المشكلة مرتبطة ببيانات الاتصال مع البروكر. رح نمشي معك خطوة بخطوة لتحديثها بأمان.`

ثم خطوة واحدة في كل مرة.

مثال:

`افتح إعدادات API في حساب البروكر. لا تغيّر أي شيء لسه، ولما توصل خبرني.`

وبعد التأكد من المكان الصحيح:

`ممتاز. هلا بنراجع نوع المفتاح والصلاحيات المطلوبة قبل إنشاء أو استبدال أي بيانات.`

---

## 4. مكان إدخال الـAPI Key والـSecret

الـSecret لا يدخل أبدًا في:

- Notification body.
- ordinary chat.
- screenshot مطلوب من Falcon.
- FCR.
- contact/outreach record.
- ordinary Web state أو user-visible history.

### قرار الـOwner: الإدخال يكون خارج الشات دائمًا

عند الحاجة لتحديث معلومات البروكر، الشات يظل **مرشدًا فقط** ولا يتحول إلى credential-entry surface.

يتم استخدام أحد مسارين Web مملوكين وآمنين، وكلاهما خارج الشات:

1. **Secure Broker Credential Popup / Dialog**
   - يظهر كنافذة مستقلة فوق واجهة Falcon، وليس جزءًا من رسائل الشات.
   - مخصص للبروكر والحساب والبيئة المتأثرة.
   - يعرض فقط الحقول التي يتطلبها البروكر المحدد.
   - مناسب أثناء incident-guided recovery حتى لا نخرج المستخدم من السياق.

2. **Falcon Broker Settings**
   - يرشد الشات المستخدم إلى صفحة إعدادات البروكر داخل Falcon.
   - المستخدم يعدّل بيانات الاتصال من المسار المعتاد والمخصص لإدارة البروكر.
   - مناسب إذا فضّل المستخدم التعديل من Settings أو إذا كان التحديث ليس جزءًا من حادث تفاعلي مباشر.

كلا المسارين يجب أن يستخدم نفس الـgoverned secure credential handling ولا يجوز إنشاء مسار أضعف أمنيًا فقط لأنه Popup.

### المسار المفضل أثناء الحادث

أثناء incident-guided recovery، الخيار الأفضل افتراضيًا هو:

```text
CHAT GUIDANCE
-> OPEN SECURE BROKER CREDENTIAL POPUP OUTSIDE CHAT
-> USER UPDATES CREDENTIALS
-> RETURN TO INCIDENT FLOW
```

مع وجود خيار واضح مثل:

`فتح إعدادات البروكر`

لينقل المستخدم بدل ذلك إلى صفحة `Falcon Broker Settings` إذا أراد.

بهذا نحافظ على سياق الحادث ونمنع إدخال الأسرار داخل المحادثة، وفي نفس الوقت لا ننشئ نظام إدارة credentials منفصل عن Settings.

واجهة الإدخال تكون broker-specific عند الحاجة، مثل:

- API Key / Key ID.
- Secret.
- Passphrase إن كان البروكر يتطلبها.
- Account/environment reference عندما يلزم.

ولا نظهر حقولًا لا تخص البروكر المحدد.

```text
CHAT_GUIDES != CHAT_COLLECTS_SECRET
SECURE_POPUP != CHAT_MESSAGE
BROKER_SETTINGS != ORDINARY_CHAT
POPUP_AND_SETTINGS = TWO_ENTRY_POINTS_TO_THE_SAME_GOVERNED_CREDENTIAL_CAPABILITY
```

---

## 5. سلوك حقول الأسرار

قرارات UX الأمنية القابلة للتنفيذ لاحقًا تحت العقد الأمني المناسب:

- Secret masked افتراضيًا.
- show/hide control واضح عند الحاجة.
- لا نعرض السر بعد submit كقيمة قابلة للقراءة.
- لا نضع raw secret في notifications أو archives أو contact logs أو ordinary telemetry.
- لا نطلب من المستخدم حفظ السر في Notes أو رسالة أو screenshot غير آمن.
- إذا البروكر يعرض الـSecret مرة واحدة فقط، ننبه المستخدم قبل مغادرة صفحة البروكر أن يكون نموذج Falcon الآمن جاهزًا لاستقبال القيمة.
- أي safe fingerprint / last characters لا يظهر إلا إذا سمح العقد الأمني الحاكم بذلك.

```text
SECRET_IN_CHAT = PROHIBITED
SECRET_IN_NOTIFICATION = PROHIBITED
SECRET_IN_FCR = PROHIBITED
SECRET_IN_REQUESTED_SCREENSHOT = NOT_REQUESTED
```

---

## 6. هوية الحساب قبل أي Reset أو Rotation

المستخدم قد يملك أكثر من حساب عند نفس البروكر، وقد يملك أكثر من API credential مرتبطة بحسابات أو بيئات مختلفة. لذلك اسم البروكر وحده غير كافٍ.

قبل توجيهه لأي reset أو rotation، يعرض الـWeb هوية الحساب المستهدَف بشكل واضح ومأخوذ من السياق الموثوق المتاح لـFalcon، مثل:

- Broker name.
- Broker account number / account identifier كما يتيحه العقد الحاكم.
- Live / Paper / Sandbox environment.
- أي nickname آمن معروف داخل Falcon لتسهيل التعرف، دون أن يحل محل رقم/معرّف الحساب authoritative.

مثال بشري:

`المشكلة مرتبطة بحساب Alpaca رقم ****4821 (Live). تأكد أنك داخل هذا الحساب قبل ما تعمل Reset للـAPI.`

إذا Falcon لا يستطيع تأكيد account identifier الصحيح، لا يطلب من المستخدم تنفيذ reset اعتمادًا على التخمين.

```text
SAME_BROKER != SAME_ACCOUNT
BROKER_NAME_ONLY != SUFFICIENT_CREDENTIAL_TARGET
UNKNOWN_ACCOUNT_TARGET != SAFE_TO_ROTATE
```

---

## 7. Safe staged credential replacement

قرار الـOwner هو عدم استبدال الـcredential المخزن القديم لحظة إدخال الجديد.

المسار المخطط:

1. الـcredential الحالي في Falcon يبقى محفوظًا مؤقتًا تحت lifecycle آمن ولا يُحذف عند إدخال القيمة الجديدة.
2. القيمة الجديدة تُحفظ كـ **candidate / pending validation** وليست Active بمجرد Submit.
3. تُربط صراحة بالبروكر + account identifier + environment الصحيح.
4. تُرسل عبر المسار الحاكم إلى الـApplication لاختبارها ضد الحساب المقصود.
5. الـApplication يعيد نتيجة authoritative مثل valid/invalid/mismatch/insufficient-permissions/unavailable وفق عقده.
6. إذا فشل الاختبار، لا يتم حذف الـcredential القديم المخزن ولا يتم اعتبار الجديد Active.
7. إذا نجح الاختبار وربط الـApplication الجديد بالحساب الصحيح، يتم Promote للجديد وفق credential lifecycle الحاكم.
8. بعد نجاح الـpromotion فقط، يتم التخلص الآمن من النسخة القديمة المخزنة وفق سياسة الأسرار الحاكمة.
9. حل حادث التداول نفسه قد يظل يحتاج reconciliation إضافية قبل إعلان `تم حل المشكلة` إذا كانت Application semantics تتطلب ذلك.

مهم: إذا كان الـbroker نفسه يلغي الـAPI القديم فور تنفيذ Reset، فإن بقاء نسخة الـcredential القديمة داخل Falcon **لا يعني أنها ما زالت صالحة للاستخدام أو أنها rollback path**. هي فقط لا تُحذف من التخزين قبل إثبات البديل الجديد حسب الـlifecycle الحاكم.

```text
NEW_CREDENTIAL_SUBMITTED != NEW_CREDENTIAL_ACTIVE
NEW_CREDENTIAL_PENDING_VALIDATION != OLD_CREDENTIAL_DELETED
BROKER_RESET_MAY_INVALIDATE_OLD_CREDENTIAL_EXTERNALLY
OLD_CREDENTIAL_RETAINED_IN_FALCON != OLD_CREDENTIAL_USABLE_AT_BROKER
APPLICATION_VALIDATION_PASS -> ELIGIBLE_FOR_PROMOTION
PROMOTION_COMPLETE -> OLD_STORED_CREDENTIAL_ELIGIBLE_FOR_SECURE_REMOVAL
```

---

## 8. Guided credential rotation

المسار البشري الافتراضي:

1. تأكيد البروكر **ورقم/معرّف الحساب والبيئة المتأثرة**.
2. عرض الحساب المقصود للمستخدم بوضوح قبل أي Reset.
3. التحقق أن credential recovery هو الإجراء المطلوب فعلًا، وعدم افتراضه بسبب outage عام.
4. فتح صفحة إدارة API الرسمية للبروكر بإرشاد محدث.
5. مراجعة permissions/scopes المطلوبة قبل إنشاء المفتاح.
6. شرح أثر revoke/rotate قبل أي إجراء غير قابل للعكس.
7. إنشاء/تدوير المفتاح وفق مسار البروكر الحالي.
8. فتح Secure Broker Credential Popup خارج الشات، أو إرشاد المستخدم إلى Falcon Broker Settings.
9. إدخال `API Key` و`Secret` فقط في المسار الأمني المختار.
10. حفظ الجديد كـPending Candidate وعدم حذف القديم المخزن.
11. إرسال candidate للـApplication للتحقق من الحساب/البيئة والصلاحية.
12. إظهار حالة `جاري التحقق من بيانات الاتصال الجديدة`.
13. عند failure: إبقاء القديم المخزن وعدم ترقية الجديد، ثم إرشاد المستخدم لتصحيح السبب.
14. عند validation success: Promote للجديد وفق المسار الحاكم، ثم إزالة القديم المخزن بأمان.
15. انتظار إعادة الاتصال والمصالحة authoritative reconciliation إذا كانت مطلوبة.
16. لا نعلن أن الحادث انتهى إلا عندما يقرر الـApplication ذلك وفق recovery/release semantics.

---

## 9. حالات الحقيقة التي يجب ألا تختلط

```text
CREDENTIAL_VALUE_SUBMITTED
!= CREDENTIAL_VALIDATED
!= CREDENTIAL_PROMOTED
!= BROKER_CONNECTION_RESTORED
!= ACCOUNT_AND_EXECUTION_RECONCILIATION_COMPLETE
!= AUTOMATED_TRADING_RELEASED
```

وأيضًا:

```text
ROTATED_KEY != INCIDENT_RESOLVED
RECONNECT != RECOVERY
```

بعد submit، النص البشري الصحيح يكون مثل:

`تم استلام بيانات الاتصال الجديدة بأمان. الآن نتحقق منها على الحساب الصحيح.`

وليس:

`تم حل المشكلة.`

---

## 10. إذا فشل التحقق

لا نعيد عرض الـSecret ولا نطلب نسخه في المحادثة.

نظهر السبب البشري إذا كان المصدر الموثوق يستطيع توفيره بأمان، مثل:

- `بيانات الاتصال غير صالحة.`
- `هذه البيانات لا تخص الحساب المتوقع.`
- `الصلاحيات المطلوبة غير مفعلة.`
- `المفتاح مرتبط ببيئة مختلفة.`
- `يوجد تقييد على عنوان IP.`
- `البروكر غير متاح حاليًا.`
- `تعذر تحديد سبب المشكلة حتى الآن.`

الـWeb يعرض المعنى ولا يخترع سببًا غير مثبت.

---

## 11. Page-aware credential guidance

إذا أرسل المستخدم screenshot أو كان Web يعرف من سياق الرحلة أنه وصل إلى صفحة معينة، لا نعيده للبداية.

مثال:

`ممتاز، أنت في صفحة API Management. قبل إنشاء مفتاح جديد، خلينا نتأكد أن الحساب الظاهر هو الحساب المطلوب.`

إذا تغيرت واجهة البروكر أو لم نعرف الموقع الحالي:

`افتح القسم الرسمي في حسابك المخصص لإدارة API أو Developer Keys.`

بدل اختراع اسم زر قديم.

---

## 12. الفصل بين Web و Application

Application يحدد semanticًا أن credential recovery/revalidation مطلوب، ويحدد النتيجة التجارية التي يحتاجها ليكمل reconciliation، ويملك نتيجة اختبار صلاحية الـcredential وربطها بالحساب/البيئة المستهدفة حسب العقد الحاكم.

Shared Web يملك:

- broker-specific navigation guidance.
- عرض broker/account/environment الصحيح للمستخدم قبل reset.
- current official UI/documentation lookup عندما يلزم.
- Secure Broker Credential Popup presentation خارج الشات.
- Falcon Broker Settings credential-management presentation.
- human wording.
- step-by-step flow.
- acknowledgements and progress states.
- تمثيل حالة candidate/pending validation للمستخدم دون اختراع نتيجة الاختبار.

ولا يقرر Web وحده أن credential الجديد صالح، أو أن تغيير المفتاح يعيد التداول، أو أن الحادث انتهى.

```text
APPLICATION CREDENTIAL RECOVERY REQUIREMENT != WEB CREDENTIAL ENTRY UX
WEB CREDENTIAL SUBMISSION != CREDENTIAL VALIDATION
WEB CREDENTIAL SUBMISSION != TRADING RELEASE AUTHORITY
```

---

## 13. الحالة الحالية

```text
SECURE_CREDENTIAL_RECOVERY_UX = PLANNED
EXACT_BROKER_ACCOUNT_ENVIRONMENT_TARGET = REQUIRED_BEFORE_ROTATION
SECRET_ENTRY_SURFACE = OUTSIDE_CHAT_ONLY
INCIDENT_DEFAULT = SECURE_BROKER_CREDENTIAL_POPUP
ALTERNATIVE_ENTRY = FALCON_BROKER_SETTINGS
POPUP_AND_SETTINGS = SAME_GOVERNED_CREDENTIAL_CAPABILITY
NEW_CREDENTIAL = PENDING_UNTIL_APPLICATION_VALIDATION
OLD_STORED_CREDENTIAL = RETAIN_UNTIL_NEW_PROMOTION_SUCCEEDS
BROKER_SIDE_OLD_KEY_VALIDITY = MUST_NOT_BE_INFERRED_FROM_LOCAL_RETENTION
BROKER-SPECIFIC_LIVE_NAVIGATION = MUST_BE_GROUNDED_IN_CURRENT_TRUSTED_BROKER_GUIDANCE_WHEN_USED
RAW_SECRET_IN_CHAT_NOTIFICATION_FCR = PROHIBITED_BY_THIS_UX_PLAN
IMPLEMENTATION = NOT_AUTHORIZED_HERE
FCR-0095 = REMAINS_OPEN / WAITING_ON_WEB
```
