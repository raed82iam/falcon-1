# HANDOVER — Shared Falcon Web Continuity

**Status:** `CONTINUITY / DISCUSSION-PLANNING HANDOVER`

هذا الملف مخصص لنقل العمل إلى جلسة ChatGPT جديدة بدون فقدان السياق أو إعادة بناء القرارات من الذاكرة.

> **تعامل مع الجلسة الجديدة كاستمرار مباشر لنفس Shared Falcon Web workstream، وليس كبداية جديدة.**

---

## 1. Repository / Branch / Write Boundary

Repository:

`raed82iam/Falcon`

Writable branch:

`web-development`

Writable subtree only:

`applications/shared/web/**`

ممنوع الكتابة على:

- `foundation-development`
- `application-development`
- `main`
- `reference/fsats-v1.3-scratch`
- أي Foundation/Application-owned path خارج `applications/shared/web/**`

`applications/shared/web/WORKSTREAM_RULES.md` هو مرجع إلزامي و**READ ONLY** للـWeb worker. لا تعدله أو تنقله أو تعيد تفسيره لتجاوزه.

---

## 2. أول شيء قبل أي تحليل أو سؤال أو اقتراح

لا تعتمد على ذاكرة المحادثة أو هذا الـhandover وحده.

اعمل fresh source-first review من GitHub الحالي.

اقرأ بالكامل، وليس ملخصًا، على الأقل:

1. `applications/shared/web/WORKSTREAM_RULES.md`
2. `applications/shared/web/README.md`
3. `applications/README.md`
4. Current Falcon Vision
5. Current Falcon Constitution
6. Current effective `APP-001`
7. Current effective `CON-023`
8. Current effective `ADR-I012`
9. Current effective `ADR-I015`
10. Applicable current Foundation contracts / authority evidence
11. Applicable current FSATS/Application design evidence
12. **كل الملفات الموجودة حاليًا تحت `applications/shared/web/docs/ Ideas/**` قراءة كاملة من أول سطر لآخر سطر، بما فيها هذا الملف.**

لا تستخدم snippets أو filenames كبديل عن القراءة الكاملة عندما تكون الوثيقة مطلوبة لفهم القرار.

---

## 3. FCR Rule — مهم جدًا

### A. قبل كل رد للـOwner

اعمل **Live repository-wide FCR check** قبل كل Owner-facing response.

### B. عند بداية الجلسة الجديدة

الـOwner طلب صراحة أن تقرأ **كل FCRs الموجودة في repository قراءة كاملة، حتى الـFCR الذي ليس موجّهًا للـWeb**.

لذلك في بداية جلسة الاستمرار:

1. اكتشف كل GitHub Issues التي تمثل FCRs في `raed82iam/Falcon`.
2. اقرأ **Body كامل** لكل FCR.
3. اقرأ **كل comments / handoffs / evidence كاملًا** لكل FCR، وليس آخر تعليق فقط.
4. لا تتجاهل FCR لأنه `Waiting On: FOUNDATION` أو `APPLICATION` أو `OWNER` أو `NONE`.
5. افهم علاقاته مع FCRs الأخرى والـreview triggers قبل الاستمرار.

الهدف ليس أن يتدخل Web في ملكية الآخرين، بل أن لا يفوّت معلومة تؤثر على التصميم أو الحدود أو المستقبل.

### C. `Waiting On: WEB`

إذا وجدت أي FCR حاليًا `Waiting On: WEB`:

- اقرأ الـIssue body كاملًا؛
- اقرأ جميع comments/evidence كاملة؛
- افحص إذا في `HANDOFF_UPDATE_REQUIRED: Waiting On=WEB` أحدث من الـbody؛
- إذا عندك جواب evidence-backed، رد عبر FCR قبل أو مع ردك للـOwner؛
- إذا لا تملك معلومة ضرورية، ابدأ ردك للـOwner بذكر **المعلومة الناقصة بالضبط**؛
- لا تخمن ولا تتجاهل handoff تعرف كيف تجيبه.

### D. باقي القيم

- `Waiting On: APPLICATION` → لا تجيب عن business/domain decision نيابة عن Application.
- `Waiting On: FOUNDATION` → لا تصلح Foundation ولا تجيب نيابة عنها.
- `Waiting On: OWNER` → اطلب قرار Owner فقط.
- `Waiting On: NONE` → لا تفترض أن التعليقات التاريخية غير مهمة؛ اقرأها عند session intake كما طلب الـOwner.

**FCR coordination != cross-workstream write authority.**

---

## 4. طريقة العمل مع الـOwner

الـOwner طلب أسلوبًا واضحًا جدًا لأن كثرة الأسئلة والنقاط تشتته.

اعمل هكذا:

```text
اقرأ المصادر كاملة
↓
افهم القرار الحالي
↓
إذا أعطى Owner جوابًا جوهريًا، وثقه داخل Web-owned docs المناسبة
↓
اشرح النتيجة باختصار
↓
اسأل سؤالًا واحدًا بسيطًا فقط
↓
انتظر الجواب
```

قواعد التواصل:

- عربي بسيط ومباشر.
- سؤال واحد فقط كل مرة.
- لا تجمع 5 أو 10 قرارات في سؤال واحد.
- لا تعطيه wall of text إذا ما طلب.
- لا تطلب منه إعادة شرح شيء موجود في الملفات أو FCRs ويمكنك قراءته بنفسك.
- إذا الموضوع التقني مملوك للـApplication أو Foundation، استخدم FCR بدل ما تحمّل الـOwner إعادة بناء الصورة التقنية.
- لما يعطي Owner تعديلًا، طبقه في التوثيق أولًا ثم كمل الحوار من النقطة التالية.
- لا تعتبر commit أو PASS أو كتابة الوثيقة Owner Acceptance نهائي.

---

## 5. Authority / Phase Boundary الحالية

الحالة الحالية في هذا المسار هي **Ideas / Discussion / Planning**.

لا يوجد من هذا الـhandover أي authority لـ:

- implementation code؛
- runtime activation؛
- deployment؛
- trading execution؛
- provider/broker connectivity؛
- Foundation changes؛
- Application business logic changes.

```text
PLANNING != IMPLEMENTATION AUTHORITY
WEB PRESENTATION != BUSINESS AUTHORITY
VISIBLE UI != AUTHORIZED ACTION
REQUEST SENT != REQUEST ACCEPTED != ACTION COMPLETED
```

Shared Web يعرض وينظم ويجمع طلب المستخدم ضمن العقود المستقبلية، لكنه لا يصبح مالك Trading/Strategy/Risk/Provider/Broker/Foundation truth.

---

## 6. أهم قاعدة Ownership

```text
Generic + intentionally reusable across Falcon
→ Shared Falcon Web

Primarily domain-specific
→ Owning Falcon Application
```

FSATS-specific market/strategy/school/trading semantics تبقى Application-owned حتى لو ظهرت على Web.

Falcon-wide account/navigation/notification preference shell يمكن تخطيطه داخل Shared Web، لكن authoritative identity/entitlement/event truth تأتي من المالك النهائي عند materialization.

---

## 7. Current UX/Product checkpoint — اقرأ الملفات للحصول على التفاصيل الكاملة

هذه ليست بديلًا عن قراءة `Ideas/**`، فقط checkpoint يمنع الضياع قبل بدء القراءة:

- Falcon public home منفصل عن ordinary User Home وعن Owner Command Center.
- One Falcon Account → Multiple Application Subscriptions.
- Owner authoritative sign-in routing → Owner Command Center.
- FSATS ordinary user lands on `FSATS Home`.
- FSATS Home يعرض Market Cards ويضيف card عند إضافة سوق جديد.
- Market Home يحوي summary/opportunities/analyses مع asset sidebar + search.
- اختيار asset يغير main workspace فقط ويبقي القائمة الجانبية.
- Asset detail: price/status → chart → Falcon summary → Falcon analysis/opinion.
- AI موجود على صفحات FSATS كـcollapsible side panel ويمكن تكبيره full page.
- AI يعرف authorized page context لكنه لا يبدأ unsolicited advice، والـuser explicit intent يتغلب على page context.
- AI quick suggestions ديناميكية واختيارية مع free-text دائمًا.
- user-supplied broker/API credentials مطلوبة فقط إذا اختار automated trading، وليس للاستشارة/التحليل.
- Chart يسمح technical indicators + Application-defined strategy overlays.
- Falcon analytical Schools خاصية مميزة و`VIP only` بعد trial.
- Standard بعد trial: حتى 4 technical indicators نشطة، وStrategy واحدة فقط على Asset واحد على مستوى الاستخدام كله.
- Standard لا يحفظ Presets/Layouts جديدة.
- VIP يستطيع advanced chart/preset/layout capabilities حسب entitlement النهائي.
- New FSATS user يحصل على شهر VIP مجاني.
- قبل انتهاء trial بـ7 أيام يظهر تنبيه + subscribe + compare VIP/Standard.
- Premium saved Presets/Layouts retained locked لمدة 60 يوم بعد VIP ثم تحذف إذا لم يعد VIP، مع reminders عند 30 / 7 / 1 يوم متبقي.
- VIP notification preferences تتبع نفس 60-day retention عند downgrade.
- `VIP Notifications = Standard Notifications + VIP-only categories`.
- Notification settings مركزية داخل Falcon Account، مع per-Application/per-category user control.
- Email reminders تُرسل فقط إذا المستخدم فعّل email notification channel/category المطلوبة.
- **أحدث Owner decision:** أثناء شهر VIP المجاني الأول، الإشعارات المتاحة داخل Falcon تكون `ON by default`، والمستخدم يستطيع إطفاءها لاحقًا. هذا لا يعني email consent؛ البريد يبقى opt-in حسب قرار الـOwner السابق.

للتفاصيل والحواف والحدود، اقرأ الملفات نفسها كاملة.

---

## 8. Current FCR snapshot عند إنشاء هذا الـhandover

هذه لقطة تاريخية فقط، وليست مصدر truth للجلسة القادمة. يجب عمل fresh check جديد.

عند إنشاء هذا الملف:

- `FCR-0077` body = `Status: APPLICATION_VERIFIED`, `Waiting On: NONE`.
- `FCR-0076` body = `Status: ACCEPTED_FOR_PLANNING`, `Waiting On: FOUNDATION`.

التعليقات التاريخية داخل الاثنين تحتوي handoffs أقدم، لذلك لا تعتمد على header وحده عند session intake. الـOwner طلب قراءة كل FCRs وكل comments كاملًا.

---

## 9. التوثيق أثناء الحوار

كل Owner clarification جوهري لا تتركه فقط في chat.

حدّث الملف الأنسب داخل:

`applications/shared/web/docs/ Ideas/**`

مع القواعد التالية:

- fresh fetch قبل كل update؛
- preserve current content؛
- لا تمسح قرار سابق إلا إذا Owner explicitly superseded it؛
- وضح candidate/discussion status؛
- لا تدعي runtime/implementation authority؛
- commit على `web-development` فقط؛
- بعد commit أخبر الـOwner باختصار بالقرار والـcommit SHA.

إذا القرار يمس أكثر من موضوع، حدّث الملفات المتأثرة sequentially، وليس بنفس path بشكل parallel.

---

## 10. كيف تبدأ الجلسة الجديدة بعد القراءة

بعد أن تنهي القراءة الكاملة المطلوبة:

1. لا تعيد على الـOwner كل ما قرأته إلا إذا طلب تقريرًا.
2. لا تسأله أن يعيد المعلومات السابقة.
3. قل له باختصار إن continuity/source/FCR review اكتمل وأنك جاهز تكمل من نفس النقطة.
4. بعدها استمر **بنفس أسلوب سؤال واحد في كل مرة**.

آخر نقطة حوارية مثبتة قبل هذا الـhandover:

> أثناء VIP trial الإشعارات داخل Falcon تكون شغالة افتراضيًا، والمستخدم هو الذي يطفئ ما لا يريده لاحقًا، مع بقاء البريد الإلكتروني خاضعًا لتفعيل المستخدم.

ابدأ بالسؤال التالي المنطقي فقط بعد أن تكمل القراءة الكاملة، ولا تقفز إلى implementation.

---

# Prime Continuity Rule

```text
FRESH REPOSITORY STATE
↓
READ ALL REQUIRED SOURCES COMPLETELY
↓
READ ALL FCRs + ALL COMMENTS COMPLETELY
↓
ESTABLISH CURRENT AUTHORITY
↓
CONTINUE EXISTING DECISIONS
↓
ONE OWNER QUESTION AT A TIME
↓
PERSIST EACH MATERIAL ANSWER
```

**لا تبدأ من الذاكرة، ولا تعيد التصميم، ولا تختصر القراءة المطلوبة.**