# Shared Falcon Web - صلاحية الدعم داخل محادثة الحادث

**Status:** `DRAFT / DISCUSSION ONLY`

هذا الملف يثبت قرار الـOwner الحالي بخصوص حدود دخول الدعم البشري إلى Incident Conversation التي تبدأ بين المستخدم وFalcon AI.

هذا القرار **يستبدل** قرار التخطيط السابق الذي كان يحصر الـOwner في دور `READ-ONLY INCIDENT OBSERVER` فقط.

لا يمنح هذا الملف implementation/runtime/trading/portfolio authorization authority، ولا يمنح الدعم أي صلاحية تنفيذ على حساب العميل أو البروكر.

---

## 1. القرار الأساسي الحالي

الدعم البشري يستطيع مشاهدة Incident Conversation ومتابعة سياقها. وعند الحاجة، يستطيع تنفيذ **Takeover صريح ومعلن** لنفس مسار الحادث والتحدث مباشرة مع العميل باسمه وصفته البشرية، وليس باسم Falcon AI.

الـOwner الحالي قد يشغل دور الدعم في المرحلة الحالية، لكن واجهة العميل تتعامل مع الدور على أنه **Support** حتى يبقى التصميم صالحًا لاحقًا لوجود موظفي دعم مختصين.

```text
SUPPORT_CAN_VIEW_INCIDENT_CONVERSATION = YES
SUPPORT_CAN_TAKE_OVER_INCIDENT_CONVERSATION = YES
SUPPORT_CAN_MESSAGE_CUSTOMER_AFTER_EXPLICIT_TAKEOVER = YES
SUPPORT_MUST_NOT_IMPERSONATE_FALCON_AI = TRUE
OWNER_ROLE_CURRENTLY_MAY_FULFIL_SUPPORT_ROLE = TRUE
```

الـTakeover لا يحدث بصمت. يجب أن يعرف العميل بوضوح أن الطرف الذي يتحدث معه تغيّر من Falcon AI إلى دعم بشري.

```text
FALCON_TO_SUPPORT_TAKEOVER = EXPLICIT_AND_VISIBLE
SUPPORT_IDENTITY_MUST_BE_CLEAR_TO_CUSTOMER = TRUE
```

---

## 2. أوضاع المحادثة

المحادثة قد تمر بالحالات المفاهيمية التالية:

```text
FALCON_ACTIVE
SUPPORT_ESCALATED_FALCON_ACTIVE
SUPPORT_TAKEOVER
```

### `FALCON_ACTIVE`

Falcon AI يتحدث مع العميل بشكل طبيعي داخل Incident Conversation.

### `SUPPORT_ESCALATED_FALCON_ACTIVE`

تم تصعيد الحادث إلى الدعم، لكن Falcon AI يواصل المحادثة مع العميل بشكل طبيعي إلى أن يقرر الدعم تنفيذ Takeover صريح أو يتغير مسار الحادث.

```text
ESCALATED_TO_SUPPORT != FALCON_DISABLED
ESCALATED_TO_SUPPORT != SUPPORT_TAKEOVER
```

### `SUPPORT_TAKEOVER`

الدعم يتحدث مباشرة مع العميل داخل نفس Incident Conversation بهوية بشرية واضحة.

أثناء الـTakeover:

```text
SUPPORT = ACTIVE_HUMAN_PARTICIPANT
FALCON_AI = SILENT_OBSERVER
```

Falcon لا يكتب باسم الدعم، والدعم لا يكتب باسم Falcon.

---

## 3. حدود السلطة

دخول الدعم إلى المحادثة لا يمنحه سلطة على حساب العميل أو محفظته أو البروكر.

```text
SUPPORT_CHAT_PARTICIPATION != CUSTOMER_EXECUTION_AUTHORITY
SUPPORT_TAKEOVER != PORTFOLIO_CONTROL
SUPPORT_MESSAGE != BUSINESS_AUTHORIZATION
OWNER_SUPPORT_ROLE != CUSTOMER_BROKER_AUTHORITY
```

العميل يبقى صاحب الفعل اليدوي على حساب البروكر عندما لا يوجد مسار تنفيذ مخول آخر.

كما أن:

```text
UI_CLICK != BUSINESS_AUTHORIZATION
REQUEST_SENT != REQUEST_ACCEPTED != ACTION_COMPLETED
NOTIFICATION_DELIVERED != INCIDENT_RESOLVED
```

تبقى إلزامية.

---

## 4. التسجيل والمراجعة أثناء Takeover

أثناء محادثة الدعم مع العميل، يبقى Falcon مراقبًا صامتًا، وتبقى المحادثة ضمن سجل الحادث المسموح به لأغراض الاستمرارية، المراجعة، وتحسين طريقة التواصل مستقبلًا ضمن الحوكمة.

قد يشمل سجل الحادث، حسب ما هو مسموح ومفعّل:

- الرسائل المكتوبة.
- التسجيلات الصوتية التي تم إنشاؤها ضمن مسار الحادث المصرح به.
- النصوص المفرغة من الصوت.
- انتقال الطرف المتحدث من Falcon إلى Support والعكس إذا أضيفت عودة صريحة لاحقًا.

لكن:

```text
RECORDED_CONVERSATION != BROKER_TRUTH
SUPPORT_STATEMENT != BROKER_CONFIRMED_TRUTH
CUSTOMER_STATEMENT != BROKER_CONFIRMED_TRUTH
SCREENSHOT_OBSERVED != BROKER_API_CONFIRMED
```

المواد الحساسة مثل raw API Key وSecret وكلمات المرور وOTP وcredential values تبقى محظورة داخل المحادثة والسجل العادي.

---

## 5. التعلم من محادثة الدعم

يمكن لـFalcon استخدام محادثة الدعم مع العميل كمصدر evidence لفهم أسلوب التواصل الأكثر فاعلية مع ذلك العميل، مثل أن العميل يستجيب أفضل للتوجيه المختصر أو يحتاج خطوة واحدة في كل مرة.

لكن هذا التعلم يبقى **خاصًا بالعميل** ولا يتحول إلى حكم نفسي ثابت أو حقيقة موضوعية عن شخصيته.

كما أن التسجيل أو استخراج نمط تواصل لا يسمح بتعديل سلوك الإنتاج تلقائيًا دون دورة Falcon المحكومة الخاصة بالتعلم والتحسين.

```text
OBSERVED_COMMUNICATION_PATTERN != OBJECTIVE_PERSONALITY_FACT
CUSTOMER_SPECIFIC_ADAPTATION != CROSS_CUSTOMER_GENERALIZATION
LEARNING_EVIDENCE != AUTOMATIC_PRODUCTION_SELF_MODIFICATION
```

أي self-improvement يتجاوز التكيف الاتصالي المسموح يحتاج مسار sandbox/evidence/approval المحكوم حسب حدود Falcon الحالية.

---

## 6. التواصل الخارجي يبقى متاحًا

وجود Takeover داخل Incident Conversation لا يلغي قنوات التواصل الخارجية. إذا كان العميل غير موجود داخل Falcon أو لا يرد، يستطيع الدعم استخدام وسائل التواصل المسموح بها، مثل:

- الاتصال الهاتفي.
- البريد الإلكتروني عندما يكون مناسبًا ومسموحًا.
- أي قناة تواصل أخرى معتمدة لاحقًا.

Shared Web يعرض بيانات التواصل المسموح بها ضمن customer/profile/contact scope، مع بقاء هذه البيانات منفصلة عن Trading/Application truth.

```text
CUSTOMER_CONTACT_DATA != TRADING_TRUTH
SUPPORT_EXTERNAL_CONTACT != INCIDENT_RESOLUTION
OWNER_REPORTED_CONTACT_ATTEMPT != TELEPHONY_VERIFIED_CALL_LOG
```

---

## 7. حدود العرض والحقيقة

فتح الدعم للمحادثة أو تنفيذ Takeover أو إرسال رسالة لا يغيّر business truth تلقائيًا.

```text
SUPPORT_VIEWED != INCIDENT_RESOLVED
SUPPORT_TAKEOVER != INCIDENT_RESOLVED
SUPPORT_MESSAGE_SENT != CUSTOMER_ACTION_COMPLETED
CUSTOMER_REPLIED != INCIDENT_RESOLVED
```

الحادث يبقى تابعًا للحالة authoritative القادمة من Application/FSATS بالنسبة للمعنى التجاري والتداولي والتعافي والمصالحة.

Shared Web يملك presentation/interaction mechanics فقط.

---

## 8. العلاقة مع FCR-0095

هذا القرار يقع ضمن Web-owned incident interaction/support participation behavior، ويستبدل التخطيط السابق الذي كان يقول:

```text
OWNER_INCIDENT_CHAT_ACCESS = READ_ONLY
OWNER_IN_CHAT_PARTICIPATION = PROHIBITED
```

القرار الحالي هو:

```text
SUPPORT_INCIDENT_CHAT_ACCESS = VIEW_AND_EXPLICIT_TAKEOVER
SUPPORT_IN_CHAT_PARTICIPATION = ALLOWED_AFTER_EXPLICIT_TAKEOVER
SUPPORT_MUST_NOT_IMPERSONATE_FALCON_AI = TRUE
FALCON_DURING_SUPPORT_TAKEOVER = SILENT_OBSERVER
SUPPORT_TAKEOVER != CUSTOMER_EXECUTION_AUTHORITY
FCR-0095 = REMAINS_OPEN / WAITING_ON_WEB
```

FCR-0095 يبقى مفتوحًا لأن:

```text
PLANNING_COMPLETE != IMPLEMENTATION_COMPLETE
WEB_IMPLEMENTATION != GOVERNED_VERIFICATION
```

هذا التعديل يوثق قرار الـOwner الحالي داخل نفس موضع التخطيط، لكنه لا يدّعي أن FCR-0095 أصبح مغلقًا أو أن governed verification اكتملت.
