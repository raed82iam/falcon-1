# Shared Falcon Web - تصحيح واجهة Falcon العامة وOwner Command Center

**Status:** `DRAFT / DISCUSSION ONLY`

هذا الملف يسجل توضيح الـOwner بتاريخ 2026-08-15 بعد مراجعة `02 - خريطة الموقع والشاشات.md`، ويصحح أي تصور بصري قد يخلط بين واجهة التداول وواجهة التحكم العامة في Falcon.

---

## 1. Owner Command Center ليس Trading Dashboard

الـProject Owner ليس متداولًا داخل واجهة الـOwner، بل Controller للنظام.

```text
OWNER COMMAND CENTER != TRADING DASHBOARD
OWNER ROLE = SYSTEM CONTROLLER / GOVERNED DECISION SURFACE
```

لذلك الصفحة الرئيسية للـOwner لا تعرض افتراضيًا:

- أسعار أسهم.
- Portfolio performance للمستخدمين كعنصر بصري رئيسي.
- Asset allocation استثماري.
- Watchlists أو Trading charts.
- واجهة شراء/بيع أو أي تجربة trader.

وجود FSATS كتطبيق داخل Falcon لا يحول Owner Command Center إلى واجهة تداول.

إذا احتاج الـOwner فتح Application معين للمراجعة أو المتابعة، يتم ذلك من `Applications` وفق الصلاحيات والعقود، لكن الصفحة الرئيسية للـOwner تبقى Falcon-wide control surface.

---

## 2. محور Owner Command Center

الاتجاه الصحيح يظل كما حددته خريطة الموقع:

```text
STATUS -> ATTENTION -> ACTION
```

وتتمحور الواجهة حول:

- الحالة العامة للنظام.
- حالة Applications ككيانات مستقلة.
- ما يحتاج انتباه أو قرار الـOwner.
- Health / degraded / contained / unknown states عندما تأتي authoritative.
- Users / access management من منظور UX المسموح.
- Activity / audit / evidence.
- Falcon Control عندما تكون controls موجودة ومصرح بها.
- Emergency / degraded control وفق العقود authoritative.
- Settings الخاصة بالنطاق المناسب.

---

## 3. Owner Chat / Ask Falcon

الـOwner يحتاج مساحة واضحة للمحادثة مع Falcon كنظام، وليس Chat خاص بالتداول فقط.

```text
OWNER ASK FALCON = SYSTEM-LEVEL CONVERSATIONAL GATEWAY
OWNER ASK FALCON != TRADING CHAT BY DEFAULT
```

المحادثة يمكنها عرض/حفظ سياق الطلب، التفسير structured، الجهة المستهدفة، الحالة، النتيجة، والـevidence حسب العقود الحالية والمستقبلية.

الـWeb لا يحول المحادثة إلى سلطة تنفيذ مستقلة، وتظل الحدود مثل:

```text
REQUEST_SENT != ACTION_ACCEPTED != ACTION_COMPLETED
UI_CLICK != AUTHORITY
```

---

## 4. Public Falcon Home قبل تسجيل الدخول

الصفحة العامة ليست صفحة FSATS وليست صفحة تداول.

```text
PUBLIC FALCON HOME = FALCON PLATFORM HOME
PUBLIC FALCON HOME != FSATS HOME
PUBLIC FALCON HOME != TRADING LANDING PAGE
```

Falcon منصة تحتوي Applications متعددة، لذلك الواجهة العامة تقدم Falcon نفسه أولًا، ثم تعرض Applications الحالية والمتاحة/المستقبلية بصورة مستقلة.

الاتجاه المرشح:

```text
FALCON
Self-Aware Autonomous Financial Operating System

[ Sign In ] [ Create Account ]

Applications
- FSATS / Trading
- Other current Falcon Applications
- Future / Candidate Applications when appropriate
```

لا تكون الرسالة البصرية العامة محصورة في الأسهم أو الشارت أو التداول، حتى لو كان FSATS أهم Application حاليًا.

---

## 5. العلاقة مع User Home

بعد تسجيل الدخول:

```text
REGULAR USER -> MY APPLICATIONS
PROJECT OWNER -> FALCON COMMAND CENTER
```

`My Applications` للمستخدم العادي تعرض التطبيقات التي يشترك بها وتسمح بفتحها.

`Falcon Command Center` للـOwner تعرض النظام وحالته واحتياجاته، وليس portfolio أو trading workspace.

---

## 6. Visual Direction

يبقى الاتجاه البصري الحالي:

- Graphite / Charcoal.
- هادئ وواضح.
- ألوان قوية فقط للحالات والتنبيهات المهمة.
- Public Home واسع ومؤسسي ويقدم منصة Falcon وتطبيقاتها.
- Owner Command Center كثيف بالمعلومة التشغيلية لكن غير مزدحم ببيانات تداول غير لازمة.
- Owner Chat عنصر رئيسي واضح.

هذه المرحلة Planning/Discussion فقط، ولا تمنح implementation/runtime authority.