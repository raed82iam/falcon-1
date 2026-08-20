# Shared Falcon Web - ملخص المستخدم داخل Owner Incident View

**Status:** `DRAFT / DISCUSSION ONLY`

هذا الملف يثبت قرار الـOwner بخصوص المعلومات التي تظهر له عند فتح Incident يخص مستخدمًا داخل Shared Web.

لا يمنح هذا الملف implementation/runtime/trading/authorization authority.

---

## 1. قرار الـOwner

عند فتح الـOwner لأي Incident خاص بمستخدم، لا يكفي عرض Online/Offline فقط.

يجب أن يظهر له **ملخص شامل ومركّز عن المستخدم والحادث** بحيث يعرف بسرعة من هو المستخدم، ما المشكلة، كيف يمكن التواصل معه، وما آخر حالة اتصال مع Falcon.

---

## 2. المعلومات الأساسية الظاهرة

يتضمن Owner Incident View، ضمن ما تسمح به صلاحيات الـOwner والسياسات الحاكمة، على الأقل:

- اسم المستخدم.
- حالة المستخدم الحالية: Online أو Offline.
- آخر وقت معروف كان فيه المستخدم Online / آخر وجود معروف داخل Falcon.
- وصف أو عنوان واضح للمشكلة / Incident الحالي.
- الحالة الحالية للحادث كما هي معروضة من authoritative source.
- رقم الهاتف / رقم التواصل المسجل.
- البريد الإلكتروني المسجل.
- Application/Domain الذي وقع فيه الحادث.
- broker/account/environment reference عندما تكون ذات صلة ومسموحًا بعرضها.
- أي معلومات تعريفية أو تشغيلية أخرى عن المستخدم يحتاجها الـOwner لفهم الحالة والتواصل معه، بشرط أن تكون ضمن صلاحياته وسياسة الخصوصية والأمن.

الفكرة أن يحصل الـOwner على صورة مختصرة لكن كافية بدل أن يبحث في عدة شاشات أثناء الحادث.

---

## 3. إذا كان المستخدم Online

إذا كان المستخدم Online:

- يظهر مؤشر Online بوضوح.
- يظهر آخر وقت/حالة حضور المتاحة.
- يظهر ملخص المستخدم والحادث في أعلى الـView.
- يمكن للـOwner مشاهدة محادثة الـIncident بين المستخدم وFalcon AI وفق القرار السابق.
- تبقى المحادثة read-only بالنسبة للـOwner.

```text
OWNER_CAN_VIEW_INCIDENT_CONVERSATION
OWNER_CANNOT_JOIN_INCIDENT_CONVERSATION
```

---

## 4. إذا كان المستخدم Offline

إذا كان المستخدم Offline:

- يظهر Offline بوضوح.
- يظهر آخر وقت معروف كان فيه Online.
- تظهر بيانات التواصل، وخاصة رقم الهاتف والإيميل.
- يظهر ملخص المشكلة والحالة الحالية للحادث.
- يستطيع الـOwner استخدام قنوات التواصل العادية خارج محادثة Falcon إذا احتاج للوصول إلى المستخدم.

---

## 5. عدم اختراع حالات فهم أو Acknowledgement للمستخدم

لا يجوز لـShared Web أو Falcon AI أن يحول سلوك المستخدم داخل Incident Conversation إلى حالات مفترضة من نوع:

- `USER_ACKNOWLEDGED`
- `USER_UNDERSTOOD`
- `USER_COMPLIED`
- `USER_IGNORED`

لمجرد أن المستخدم فتح الرسالة أو تأخر في الرد.

المستخدم قد يقرأ الرسالة ثم ينتقل فعليًا إلى تطبيق أو موقع البروكر حتى يتحقق من الحساب، وقد يعود بعد ذلك ليجيب. لذلك الصمت أو التأخر في الرد لا يكشف بشكل موثوق ما إذا كان المستخدم فهم، تجاهل، نفذ، أو ما زال يتحقق.

يمكن للواجهة عرض الوقائع التي تعرفها فعلًا فقط، مثل:

- أن الرسالة ظهرت/فُتحت إذا كان ذلك مدعومًا بقياس تقني موثوق.
- أن المستخدم أرسل ردًا فعليًا عندما يرسل ردًا.
- النص الفعلي للمحادثة.
- Online/Offline وآخر وقت معروف للحضور.
- authoritative incident state القادم من الجهة المالكة.

لكن لا يتم تحويل هذه الوقائع إلى تفسير نفسي أو تشغيلي لسلوك المستخدم.

```text
MESSAGE_VIEWED != USER_ACKNOWLEDGED
MESSAGE_VIEWED != USER_UNDERSTOOD
NO_REPLY != USER_IGNORED
DELAYED_REPLY != USER_NONCOMPLIANCE
USER_RESPONSE != INCIDENT_RESOLVED
```

الـOwner يرى المحادثة والوقائع ويستخدم حكمه البشري ليقرر هل يحتاج التواصل مع المستخدم عبر القنوات العادية.

```text
AI_PRESENTS_OBSERVABLE_FACTS
OWNER_MAKES_HUMAN_CONTACT_JUDGMENT
```

---

## 6. الخصوصية والأمان

"كل معلومات المستخدم" هنا تعني كل المعلومات **المناسبة لهذا السياق والمسموح للـOwner رؤيتها**، وليس الأسرار أو المواد الأمنية الحساسة.

لا يجوز أن يظهر ضمن هذا الملخص:

- raw API Keys.
- Secrets.
- كلمات مرور.
- hidden authentication material.
- أي بيانات حساسة غير لازمة للحادث أو غير مخولة للـOwner.

```text
OWNER_INCIDENT_CONTEXT = BROAD_USER_CONTEXT_WITHIN_AUTHORITY
OWNER_INCIDENT_CONTEXT != UNRESTRICTED_SECRET_ACCESS
```

---

## 7. حدود الصلاحية

عرض معلومات المستخدم لا يغير صلاحيات الـOwner على الحادث أو حساب التداول.

```text
OWNER_VISIBILITY != CUSTOMER_EXECUTION_AUTHORITY
OWNER_CONTACT_ACCESS != TRADING_AUTHORITY
OWNER_VIEW != BUSINESS_STATE_OVERRIDE
```

الـOwner يرى، يفهم، ويتواصل عند الحاجة، لكنه لا يصبح طرفًا في محادثة الـAI الخاصة بالمستخدم ولا يكتسب سلطة تنفيذ تداول بسبب هذه الشاشة.

---

## 8. الحالة الحالية

```text
OWNER_INCIDENT_USER_SUMMARY = PLANNED
ONLINE_OFFLINE_STATUS = PLANNED
LAST_KNOWN_ONLINE_TIME = PLANNED
CONTACT_PHONE = PLANNED
CONTACT_EMAIL = PLANNED
INCIDENT_SUMMARY = PLANNED
AUTHORIZED_USER_CONTEXT = PLANNED
OWNER_INCIDENT_CONVERSATION_MODE = READ_ONLY
USER_ACKNOWLEDGEMENT_INFERENCE = PROHIBITED
USER_UNDERSTANDING_INFERENCE = PROHIBITED
OWNER_HUMAN_CONTACT_JUDGMENT = PLANNED
IMPLEMENTATION = NOT AUTHORIZED HERE
FCR-0095 = REMAINS_OPEN / WAITING_ON_WEB
```
