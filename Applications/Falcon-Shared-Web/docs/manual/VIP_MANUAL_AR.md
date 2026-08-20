# دليل مستخدم VIP — Falcon Shared Web

**الفئة:** VIP User  
**اللغة:** العربية  
**النطاق:** واجهات العميل نفسها مع شرح وضع VIP كما هو معرف حاليًا في Shared Web

## 1. ما معنى VIP حاليًا؟

الـWeb يعرف مستويين للعرض: `STANDARD` و`VIP`. لكن وجود بطاقة VIP أو اسم VIP في الواجهة لا يكفي وحده لإثبات أن الحساب VIP.

حتى تعتبر VIP فعليًا، يجب أن تصل بيانات authoritative تصرح أن tier `VIP`:

- `authoritative = true`
- `entitled = true`
- `current = true`

إذا لم تصل هذه الحقيقة، تبقى VIP مقفلة أو غير متاحة.

القاعدة: `TIER_VISIBLE ≠ ENTITLED ≠ ACTION_AUTHORIZED`.

## 2. ماذا يختلف عن Standard User؟

من ناحية الـWeb الحالية، VIP ليست صلاحية مفتوحة ولا role إداري. VIP هي subscription/access tier presentation.

هذا يعني:

- VIP لا تفتح Owner pages.
- VIP لا تمنح Business Authority.
- VIP لا تمنح Trading Authority.
- VIP لا تتجاوز authentication أو entitlement checks.
- VIP لا تحول route موجود إلى تنفيذ مسموح.

أي ميزات إضافية حقيقية لـVIP يجب أن تأتي من contract authoritative. الويب لا يخترعها.

## 3. الصفحات التي قد يستخدمها VIP

عندما تكون الجلسة والصلاحيات صحيحة، يستخدم VIP نفس customer workspace الأساسي:

- My Applications `#/my-apps`
- FSATS workspace `#/trader`
- Markets `#/markets`
- Advisory Markets `#/advisory-markets`
- Portfolio `#/portfolio`
- Activity `#/activity`
- AI `#/ai`
- Notifications `#/notifications`
- Settings `#/settings`

الميزة الإضافية، إن وجدت، يجب أن تكون مربوطة ببيانات entitlement authoritative وليس باسم VIP فقط.

## 4. السعر والترقية والتجربة

Shared Web لا يفترض:

- سعر VIP
- مدة الاشتراك
- Trial
- Upgrade path
- خصومات
- أولوية دعم
- حدود تداول خاصة

إذا لم تصل هذه التفاصيل من contract authoritative، تظهر كغير متاحة.

## 5. الأسواق والبيانات

VIP لا يغير قاعدة البيانات التشغيلية:

- Web market data = Presentation only.
- Web data لا تصبح FSATS operational input.
- ظهور بيانات أسرع أو أكثر لا يعني تنفيذ صفقة.
- provider route لا تساوي connectivity authority.

## 6. AI والتحليل

حتى لو كان لدى VIP وصول أوسع إلى العرض لاحقًا، تبقى قواعد truth نفسها:

- Current + Complete فقط يسمحان بالتفاصيل الكاملة.
- Stale / Partial / Needs Clarification لا تظهر كتحليل كامل.
- الويب لا يحول analysis إلى execution.

## 7. الدعم والحوادث

أي مستوى دعم إضافي لـVIP يجب أن يأتي من contract أو runtime capability authoritative. لا يتم افتراض priority support لمجرد وجود tier VIP.

مزايا incident/support الحالية تبقى خاضعة للجلسة والهوية والـtransport والـpersistence والـsecurity scanner والـvoice runtime عند الحاجة.

## 8. الأمان

- لا تشارك credentials أو secrets.
- لا تعتبر VIP تجاوزًا لقواعد الأمن.
- لا تعتبر VIP صلاحية للوصول إلى Owner أو Support internals.
- لا تعتبر VIP صلاحية تداول.
- لا تعتبر بطاقة VIP في الواجهة دليلًا على entitlement إذا لم تكن authoritative.

## 9. الحالة الحالية

VIP موجود كـsupported subscription tier في الواجهة، لكن التفاصيل التجارية الدقيقة ومزايا VIP الخاصة لا يتم اختراعها من Web. عندما يأتي contract authoritative، تعرض الواجهة الحقيقة كما هي.
