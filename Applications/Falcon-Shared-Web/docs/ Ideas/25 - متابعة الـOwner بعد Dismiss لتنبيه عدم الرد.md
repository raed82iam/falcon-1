# Shared Falcon Web - متابعة الـOwner بعد Dismiss لتنبيه عدم الرد

**Status:** `DRAFT / DISCUSSION ONLY`

هذا الملف يثبت قرار الـOwner بخصوص ما يحدث بعد Dismiss لتنبيه عدم رد المستخدم داخل Incident عالي الأولوية.

لا يمنح هذا الملف implementation/runtime/trading/authorization authority.

---

## 1. قرار الـOwner

عندما يظهر High Alert بسبب أن المستخدم شاهد رسالة Incident ولم يرد خلال خمس دقائق، يستطيع الـOwner عمل Dismiss للتنبيه.

بعد الـDismiss لا يعاد إظهار نفس تنبيه عدم الرد تلقائيًا لمجرد استمرار الصمت.

الـDismiss هنا يعني أن الـOwner استلم إشارة المتابعة وبدأ أو سيتولى المتابعة البشرية مع العميل خارج Incident Chat.

```text
OWNER_DISMISS -> NO_AUTOMATIC_REPEAT_OF_SAME_NO_REPLY_ALERT
OWNER_DISMISS != USER_REPLIED
OWNER_DISMISS != INCIDENT_RESOLVED
```

---

## 2. المتابعة بعد الـDismiss

بعد Dismiss توجد مسارات طبيعية:

1. العميل يعود إلى Falcon ويكمل Incident Conversation مع الـAI.
2. الـOwner ينجح في التواصل مع العميل عبر قناة عادية خارج Falcon Incident Chat ثم يبلغ Falcon بنتيجة التواصل إذا كانت ذات صلة.
3. الـOwner لا يستطيع الوصول إلى العميل بعد عدة محاولات، فيبلغ Falcon بعدم إمكانية التواصل وعدد محاولات الاتصال.

لا يدخل الـOwner إلى محادثة الـIncident ولا يرسل رسائل داخلها بصفته Owner.

---

## 3. كيف يسجل الـOwner نتيجة المتابعة

الـOwner يستطيع فتح منصة الحوار الخاصة به داخل Falcon والإشارة إلى Incident محدد، ثم يبلغ Falcon بنتيجة المتابعة البشرية، مثل:

`Incident <id>: تمت محاولة الاتصال بالعميل خمس مرات ولم نستطع الوصول إليه.`

Falcon يحفظ هذا كـ **Owner-reported Incident Log entry** مرتبط بنفس الـIncident، بحيث يستطيع الـOwner الرجوع إليه لاحقًا من سجل الحادث.

لا يلزم في الوضع الحالي أن يسجل الـOwner كل مكالمة منفردة إذا لم يوجد مصدر تقني مستقل لها؛ يكفي أن يسجل النتيجة التي يقرر توثيقها، بما فيها عدد المحاولات الذي يبلغه هو.

يمكن أن يتضمن السجل اليدوي:

- Incident identity.
- عدد محاولات التواصل كما أبلغ عنها الـOwner.
- النتيجة العامة كما أبلغ عنها الـOwner: تم التواصل / لم يتم التواصل.
- وقت تسجيل التصريح داخل Falcon.
- هوية الـOwner الذي سجل التصريح.
- ملاحظة مختصرة مرتبطة بالحادث إذا احتاجها السياق.

```text
OWNER_DIALOGUE_INPUT -> ATTRIBUTABLE_OWNER_REPORTED_INCIDENT_LOG
OWNER_REPORTED_ATTEMPT_COUNT = PRESERVED_AS_REPORTED
OWNER_REPORTED_INCIDENT_LOG = OWNER_REVIEWABLE_LATER
```

---

## 4. مصدر الدليل في الوضع الحالي

في الوضع الحالي لا يوجد Call Log تقني أو تكامل اتصالات يثبت محاولات الاتصال آليًا.

لذلك يعتمد Falcon حاليًا على **تصريح الـOwner نفسه** بخصوص نتيجة المتابعة وعدد محاولات التواصل.

```text
CURRENT_CONTACT_EVIDENCE_SOURCE = OWNER_REPORTED
OWNER_CONTACT_ATTEMPT_RECORD = ATTRIBUTABLE_HUMAN_FOLLOW_UP_RECORD
NO_TELEPHONY_CALL_LOG_INTEGRATION = CURRENT_STATE
```

هذا السجل هو **Owner-reported record** وليس دليلًا تقنيًا مستقلًا على أن مكالمة حدثت أو على مدتها أو نتيجتها الفعلية خارج Falcon.

```text
OWNER_REPORTED_CONTACT_ATTEMPTS != TELEPHONY_VERIFIED_CALL_LOG
OWNER_REPORTED_CONTACT_RESULT != INDEPENDENT_COMMUNICATION_PROOF
OWNER_REPORT != BROKER_CONFIRMED_TRUTH
OWNER_CONTACT_SUCCESS != INCIDENT_RESOLVED
OWNER_CONTACT_FAILURE != USER_REFUSAL
```

---

## 5. رسالة للعميل عند عودته

إذا كان العميل Offline ولم نستطع الوصول إليه، أو كان Online وشاهد رسالة الـIncident ولم يرد ثم حاول الـOwner التواصل معه خارجيًا ولم ينجح، فإن Falcon يحتفظ بمتابعة الحادث.

عندما يدخل العميل إلى Falcon في المرة التالية، يظهر له تنبيه/رسالة مرتبطة بنفس الـIncident توضح باختصار ووضوح:

- أنه تمت محاولة التواصل معه.
- عدد المحاولات كما سجله الـOwner.
- أن التواصل لم ينجح حتى تلك اللحظة.
- Incident number / identity.
- سبب المتابعة بصياغة بشرية مستندة إلى الحقيقة authoritative للحادث، مثل أن الاتصال بالبروكر/الحساب كان غير متاح أو أن الحادث كان يتطلب متابعة.
- تاريخ ووقت بدء الحالة أو الحادث عندما تكون هذه الحقيقة متاحة من المصدر المالك.
- أن Falcon ما زال بانتظار تحديث/تواصل من العميل لمواصلة التعامل مع الحادث.

مثال UX غير ملزم للنص النهائي:

`حاولنا التواصل معك 5 مرات ولم نتمكن من الوصول إليك بخصوص Incident رقم <id>. بدأ الحادث بتاريخ <date> الساعة <time> وكان متعلقًا بعدم إمكانية التواصل مع البروكر/الحساب. ننتظر تحديثك لمواصلة المتابعة.`

إذا كان الوصف التجاري الدقيق للحادث مختلفًا، يستخدم Web الوصف authoritative القادم من الجهة المالكة ولا يخترع سببًا من نفسه.

```text
CUSTOMER_RETURN_AFTER_FAILED_OWNER_CONTACT -> SHOW_INCIDENT_FOLLOW_UP_NOTICE
NOTICE_ATTEMPT_COUNT = OWNER_REPORTED_VALUE
NOTICE_INCIDENT_IDENTITY = SAME_INCIDENT
NOTICE_BUSINESS_REASON = ORIGIN_OWNED_AUTHORITATIVE_SEMANTICS
```

هذه الرسالة لا تعني أن الحادث ما زال بالضرورة في نفس الحالة القديمة إذا تغيرت الحقيقة authoritative أثناء غياب العميل. عند عودته يجب أن تعرض الواجهة **الحالة الحالية الصحيحة** مع الحفاظ على سجل محاولات التواصل السابقة.

```text
HISTORICAL_CONTACT_FAILURE_RECORD != CURRENT_INCIDENT_STATE
LATEST_AUTHORITATIVE_INCIDENT_STATE_WINS
```

### 5.1 ظهور الرسالة مرة واحدة فقط

الـOwner قرر أن رسالة فشل التواصل تظهر للعميل **مرة واحدة فقط** عند أول Login لاحق بعد تسجيل فشل التواصل.

لا يعاد إجبار العميل على رؤية نفس الرسالة في كل Login لاحق، ولا يشترط أن يرد على الرسالة نفسها لكي تعتبر الرسالة قد أدت غرضها كإشعار.

```text
FAILED_CONTACT_NOTICE_DISPLAY = ONCE_ON_FIRST_SUBSEQUENT_LOGIN
NOTICE_VIEWED_ONCE -> NO_FORCED_REPEAT_OF_SAME_NOTICE
NOTICE_REPLY_REQUIRED_FOR_NOTICE_DISMISSAL = NO
NOTICE_NOT_REPEATED != INCIDENT_RESOLVED
```

سجل الرسالة ومحاولات التواصل يبقى محفوظًا داخل Incident Log للرجوع إليه، حتى بعد انتهاء العرض الإجباري للرسالة.

---

## 6. حالة الحساب بعد تعذر التواصل

الـOwner قرر أن حساب العميل يبقى **Inactive** بعد تعذر التواصل، ولا يعود Active لمجرد أن رسالة فشل التواصل ظهرت مرة واحدة أو لأن العميل شاهدها فقط.

العودة المطلوبة هي أن يدخل العميل مرة أخرى إلى Falcon **ويتواصل مع Falcon** لاستكمال المتابعة. إلى أن يحدث ذلك، تبقى حالة الحساب Inactive وفق هذا القرار التخطيطي.

```text
FAILED_CONTACT_FOLLOW_UP -> CUSTOMER_ACCOUNT_INACTIVE
FIRST_LOGIN_AND_NOTICE_VIEW_ONLY != ACCOUNT_REACTIVATION
CUSTOMER_REENTERS_AND_CONTACTS_FALCON -> REACTIVATION_ELIGIBLE_FOR_GOVERNED_FLOW
NO_CUSTOMER_CONTACT -> ACCOUNT_REMAINS_INACTIVE
```

هذا الملف يثبت قرار المنتج/UX فقط. **المعنى التنفيذي الدقيق لـInactive، جهة امتلاك Account lifecycle، شروط reactivation، وما الذي يكون محظورًا أو مسموحًا أثناء Inactive يجب ربطه لاحقًا بالعقود والسلطة الحاكمة المناسبة قبل التنفيذ.** لا يجوز للـWeb أن يخترع من نفسه أثرًا على trading authority أو broker state أو entitlement لم يحدده المالك المختص.

```text
WEB_INACTIVE_PRESENTATION != TRADING_ACCOUNT_STATE
WEB_INACTIVE_PRESENTATION != BROKER_ACCOUNT_STATE
ACCOUNT_REACTIVATION_UI != AUTHORITY_TO_REACTIVATE_WITHOUT_GOVERNED_RULE
```

---

## 7. Offline و Online بلا رد

تنطبق رسالة المتابعة في الحالتين التاليتين:

### الحالة A - العميل Offline

- العميل غير موجود داخل Falcon أثناء الحادث/المتابعة.
- الـOwner يحاول التواصل خارجيًا.
- يفشل التواصل ويسجل عدد المحاولات.
- عند أول دخول لاحق للعميل، يرى رسالة المتابعة المرتبطة بنفس Incident مرة واحدة.
- يبقى الحساب Inactive إلى أن يدخل العميل ويتواصل مع Falcon وفق القاعدة أعلاه.

### الحالة B - العميل Online وشاهد الرسالة لكنه لم يرد

- Web لديه دليل موثوق أن العميل شاهد رسالة الـIncident.
- يمر Threshold الخمس دقائق ويظهر High Alert للـOwner.
- الـOwner يعمل Dismiss ويتولى المتابعة البشرية.
- يحاول التواصل خارجيًا ولا ينجح.
- يسجل النتيجة وعدد المحاولات.
- إذا عاد العميل للمحادثة لاحقًا أو دخل في جلسة لاحقة، تظهر له رسالة المتابعة المرتبطة بنفس Incident مرة واحدة، مع استمرار نفس Incident Conversation وفق قاعدة الاستمرارية السابقة.
- يبقى الحساب Inactive إلى أن يعود العميل ويتواصل مع Falcon.

```text
OFFLINE_UNREACHABLE -> SAME_FOLLOW_UP_RECORD_PATTERN
ONLINE_VIEWED_NO_REPLY_AND_UNREACHABLE -> SAME_FOLLOW_UP_RECORD_PATTERN
```

---

## 8. عدم إعادة التنبيه للـOwner

بعد الـDismiss، استمرار عدم رد المستخدم لا يعيد نفس High Alert دوريًا.

إذا عاد المستخدم ورد، تتحدث واجهة الـIncident بالحقيقة الجديدة.

إذا أبلغ الـOwner أنه لم يستطع التواصل، تحفظ نتيجة المتابعة وعدد المحاولات كسجل Owner-reported ويظل الحادث على حالته authoritative من الجهة المالكة له.

```text
DISMISSED_NO_REPLY_ALERT = HUMAN_FOLLOW_UP_OWNED
NO_PERIODIC_REALERT_FOR_SAME_SILENCE = YES
```

أي Incident جديد أو تغير authoritative جديد قد ينتج تنبيهًا جديدًا وفق قواعده الخاصة، لكنه ليس إعادة تدوير لنفس تنبيه الصمت القديم.

---

## 9. تطوير مستقبلي محتمل

يمكن مستقبلًا، إذا تقرر واعتمد تكامل مناسب مع نظام اتصالات أو مزود Call Log، إضافة دليل تقني مستقل لمحاولات الاتصال وربطه بالـIncident.

لكن هذا غير موجود حاليًا، ولا يجوز للتخطيط الحالي أن يدعي وجوده أو يعتمد عليه.

```text
FUTURE_CALL_LOG_INTEGRATION = POSSIBLE_FUTURE_CHANGE_ONLY
CURRENT_CALL_LOG_INTEGRATION = NONE
```

أي تطوير مستقبلي يحتاج تخطيطًا وصلاحية واعتمادًا مستقلًا في وقته.

---

## 10. حدود الصلاحية

الـOwner هنا يدير المتابعة البشرية فقط.

```text
OWNER_FOLLOW_UP != TRADING_AUTHORITY
OWNER_CONTACT_RECORD != BUSINESS_STATE_OVERRIDE
OWNER_REPORTED_CONTACT_RESULT != INCIDENT_RESOLUTION_AUTHORITY
OWNER_REPORTED_ATTEMPT_COUNT != TELEPHONY_VERIFIED_CALL_COUNT
CUSTOMER_RETURN_NOTICE != INCIDENT_RESOLUTION
NOTICE_VIEW != CUSTOMER_CONTACT
```

الحل النهائي للحادث يبقى مرتبطًا بالحقيقة authoritative ومعايير الإغلاق/الاستعادة المملوكة للجهة المختصة.

---

## 11. الحالة الحالية

```text
OWNER_CAN_DISMISS_NO_REPLY_ALERT = PLANNED
SAME_ALERT_AUTO_REPEAT_AFTER_DISMISS = NO
OWNER_EXTERNAL_CONTACT_FOLLOW_UP = PLANNED
CURRENT_CONTACT_EVIDENCE_SOURCE = OWNER_REPORTED
TELEPHONY_VERIFIED_CALL_LOG = NOT_AVAILABLE
OWNER_CONTACT_ATTEMPT_COUNT_RECORD = PLANNED
OWNER_CONTACT_RESULT_RECORD = PLANNED
OWNER_REPORTED_INCIDENT_LOG = PLANNED
OWNER_CAN_REVIEW_INCIDENT_LOG_LATER = PLANNED
CUSTOMER_RETURN_FAILED_CONTACT_NOTICE = PLANNED
FAILED_CONTACT_NOTICE_DISPLAY = ONCE_ON_FIRST_SUBSEQUENT_LOGIN
NOTICE_REPLY_REQUIRED = NO
CUSTOMER_ACCOUNT_AFTER_FAILED_CONTACT = INACTIVE_UNTIL_REENTRY_AND_CONTACT
OFFLINE_AND_ONLINE_NO_REPLY_CASES = COVERED
OWNER_CAN_JOIN_INCIDENT_CHAT = NO
FUTURE_CALL_LOG_INTEGRATION = POSSIBLE_FUTURE_CHANGE_ONLY
IMPLEMENTATION = NOT AUTHORIZED HERE
FCR-0095 = REMAINS_OPEN / WAITING_ON_WEB
```
