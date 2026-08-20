# Shared Falcon Web - تنبيه الـOwner عند تأخر رد المستخدم

**Status:** `DRAFT / DISCUSSION ONLY`

هذا الملف يثبت قرار الـOwner بخصوص تصعيد حالة مشاهدة المستخدم لرسالة Incident عالية الأولوية ثم عدم الرد خلال مدة قصيرة.

لا يمنح هذا الملف implementation/runtime/trading/authorization authority.

---

## 1. قرار الـOwner

في الحوادث عالية الأولوية، إذا فتح المستخدم رسالة/خطوة داخل Incident Conversation وأصبح لدينا دليل Web موثوق أنه **شاهد الرسالة**، ثم لم يرسل أي رد أو إدخال جديد خلال مدة انتظار قصيرة، فإن Shared Web يرفع **High Alert للـOwner** على نفس الـIncident.

الهدف ليس افتراض أن المستخدم تجاهل Falcon أو أنه لم يفهم، بل تنبيه الـOwner إلى أن المستخدم شاهد الرسالة ولم يصل رد بعد، كي يقرر بشريًا إن كان يحتاج للتواصل معه عبر القنوات العادية.

```text
MESSAGE_VIEWED + NO_USER_RESPONSE_AFTER_THRESHOLD -> OWNER_HIGH_ALERT
```

---

## 2. ما الذي يعنيه التنبيه

التنبيه للـOwner يعبّر عن حقيقة محدودة فقط:

- الرسالة تم عرضها/مشاهدتها وفق دليل Web المتاح.
- مرّت مدة الانتظار المحددة.
- لم يصل رد جديد من المستخدم داخل Incident Conversation حتى لحظة التنبيه.

ولا يجوز تحويل ذلك إلى استنتاجات عن نية المستخدم أو فهمه.

```text
MESSAGE_VIEWED != USER_ACKNOWLEDGED
MESSAGE_VIEWED != USER_UNDERSTOOD
NO_REPLY != USER_IGNORED
NO_REPLY != USER_REFUSED
OWNER_ALERT != INCIDENT_ESCALATION_TRUTH
```

---

## 3. ما يظهر للـOwner

عند تحقق الشرط، يظهر على الـIncident في Owner Incident Queue / Owner Incident View تنبيه عالي الوضوح، بصياغة بشرية مثل:

`العميل شاهد آخر رسالة، ولم يرسل ردًا حتى الآن.`

ويُفضّل أن يظهر معه:

- وقت مشاهدة آخر رسالة.
- المدة التي مضت بدون رد.
- حالة المستخدم Online/Offline الحالية.
- رابط/دخول مباشر إلى نفس Incident View.
- بيانات التواصل المعتادة المسموح للـOwner رؤيتها.

الـOwner يقرر بعد ذلك إن كان يحتاج للاتصال بالعميل أو متابعته خارج Incident Chat.

---

## 4. إلغاء أو تحديث أو Dismiss التنبيه

إذا أرسل المستخدم ردًا جديدًا قبل انتهاء المدة، لا يُرفع التنبيه.

إذا أرسل ردًا بعد رفع التنبيه، يتم تحديث حالة التنبيه ليعكس أن ردًا جديدًا وصل، ولا يبقى ظاهرًا وكأن المستخدم ما زال صامتًا.

الـOwner يملك أيضًا صلاحية **Dismiss** لهذا الـHigh Alert يدويًا من واجهة الـOwner.

Dismiss هنا يعني فقط إزالة/إخفاء تنبيه المتابعة الحالي من واجهة الـOwner، ولا يعني أن المستخدم رد، ولا أن الحادث حُل، ولا أن الـIncident تغيّرت حالته التجارية أو التشغيلية.

```text
USER_RESPONSE_BEFORE_THRESHOLD -> NO_OWNER_NO_REPLY_ALERT
USER_RESPONSE_AFTER_ALERT -> ALERT_STATE_UPDATED
OWNER_DISMISS_ALERT -> ALERT_PRESENTATION_DISMISSED
OWNER_DISMISS_ALERT != USER_RESPONSE
OWNER_DISMISS_ALERT != INCIDENT_RESOLVED
OWNER_DISMISS_ALERT != BUSINESS_STATE_CHANGE
```

هذا لا يعني أن الحادث حُل.

```text
USER_REPLIED != INCIDENT_RESOLVED
```

---

## 5. نطاق التطبيق

هذه القاعدة خاصة بالحوادث ذات الأولوية العالية التي تستدعي متابعة بشرية محتملة.

لا تُطبق تلقائيًا على كل Notification أو رسالة عادية داخل Falcon.

Shared Web لا يرفع business severity من نفسه؛ هو فقط يعرض تنبيه متابعة مبنيًا على Web interaction evidence ضمن حادث عالي الأولوية أصلًا.

```text
OWNER_NO_REPLY_ALERT = HIGH_PRIORITY_INCIDENT_FOLLOW_UP_SIGNAL
OWNER_NO_REPLY_ALERT != BUSINESS_PRIORITY_RECLASSIFICATION
```

---

## 6. مدة الانتظار

الـOwner ثبت مدة الانتظار على **خمس دقائق**.

يبدأ احتساب المدة من اللحظة التي يملك فيها Shared Web دليلًا موثوقًا أن المستخدم شاهد آخر رسالة/خطوة التي تنتظر منه ردًا داخل Incident Conversation.

إذا مرّت خمس دقائق كاملة ولم يصل أي رد أو إدخال جديد من المستخدم داخل نفس الـIncident، يُرفع High Alert للـOwner.

```text
OWNER_NO_REPLY_ALERT_THRESHOLD = 5_MINUTES
MESSAGE_VIEWED_AT + 5_MINUTES_WITHOUT_USER_RESPONSE -> OWNER_HIGH_ALERT
```

لا يجوز اعتبار الخمس دقائق دليلًا على تجاهل المستخدم أو رفضه أو عدم فهمه؛ هي فقط عتبة متابعة بشرية للـOwner.

---

## 7. العلاقة مع استمرارية المحادثة

هذا القرار لا يغير القاعدة السابقة:

- Refresh لا يبدأ Incident جديد.
- انقطاع الإنترنت لا يبدأ Incident جديد.
- Logout/Login لا يبدأ Incident جديد.
- العودة تفتح نفس Incident Conversation وسجلها السابق.

التنبيه هنا يتعلق فقط بزمن عدم الرد بعد مشاهدة رسالة أثناء Incident مفتوح.

---

## 8. الحالة الحالية

```text
OWNER_HIGH_ALERT_ON_VIEWED_NO_REPLY = PLANNED
ALERT_TRIGGER_THRESHOLD = 5_MINUTES
OWNER_CAN_DISMISS_NO_REPLY_ALERT = YES
OWNER_ALERT_DISMISSAL = PRESENTATION_ONLY
USER_INTENT_INFERENCE = PROHIBITED
OWNER_HUMAN_FOLLOW_UP = ALLOWED_OUTSIDE_INCIDENT_CHAT
IMPLEMENTATION = NOT AUTHORIZED HERE
FCR-0095 = REMAINS_OPEN / WAITING_ON_WEB
```
