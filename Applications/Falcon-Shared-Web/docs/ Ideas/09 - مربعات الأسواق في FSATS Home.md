# Shared Falcon Web - مربعات الأسواق في FSATS Home

**Status:** `DRAFT / DISCUSSION ONLY`

هذا الملف يسجل قرار تجربة عرض الأسواق داخل `FSATS Home`. هذا تخطيط UX/Product فقط ولا يمنح implementation/runtime/execution authority.

---

## 1. القرار الحالي

بدل أن تكون الأسواق الحالية محصورة في Tabs ثابتة مثل `US Market | Crypto`، تظهر الأسواق المتاحة للمستخدم على شكل **Market Cards / مربعات سوق** داخل `FSATS Home`.

كل مربع يمثل سوقًا متاحًا حاليًا في FSATS ويعرض له صورة عامة مختصرة، بما فيها **المؤشر العام/المرجع العام المناسب لذلك السوق عندما يكون هذا المعنى متاحًا بصورة authoritative من الجهة المالكة**.

مثال UX تصوري فقط:

```text
┌─────────────────┐  ┌─────────────────┐
│ US Market       │  │ Crypto          │
│ General Index   │  │ Market Overview │
│ Current Status  │  │ Current Status  │
└─────────────────┘  └─────────────────┘
```

هذه الأمثلة لا تثبت أسماء المؤشرات أو الحقول النهائية، ولا تمنح الـWeb صلاحية تحديد market/business semantics من عنده.

---

## 2. صفحة رئيسية خاصة بكل سوق

عندما يضغط المستخدم على `Market Card`، لا ينتقل مباشرة إلى قائمة الأسهم/الأصول فقط.

يفتح أولًا **Market Home** خاصة بذلك السوق، وتكون مساحة العمل الرئيسية فيها مخصصة لفهم السوق نفسه وما يراه FSATS مهمًا داخله.

الاتجاه الحالي:

```text
MARKET CARD
     ↓
MARKET HOME
     ├─ Main Workspace
     │    ├─ Market Summary
     │    ├─ Opportunities
     │    └─ Analysis
     │
     └─ Asset Sidebar
          ├─ Stocks / Assets in this Market
          └─ Search
```

يعني:

- الصفحة الرئيسية للسوق تعرض **ملخص السوق** في المساحة الرئيسية.
- تعرض **الفرص** التي يوفرها FSATS بصورة authoritative عندما تكون متاحة.
- تعرض **التحليلات** المرتبطة بهذا السوق حسب المخرجات الفعلية للجهة المالكة.
- تبقى على الجانب قائمة بالأسهم/الأصول التابعة للسوق حتى يستطيع المستخدم التنقل بينها بسهولة.
- تحتوي القائمة الجانبية على **Search** حتى لا يضطر المستخدم للبحث يدويًا داخل قائمة طويلة.

الهدف أن يشعر المستخدم أنه داخل **مساحة تداول خاصة بالسوق** وليس مجرد جدول أصول، مع بقاء الوصول إلى أي سهم/أصل سريعًا وواضحًا.

```text
MARKET HOME = MARKET CONTEXT + DISCOVERY
ASSET SIDEBAR = NAVIGATION / DISCOVERY
ASSET SIDEBAR != MARKET OR ANALYSIS AUTHORITY
```

التفاصيل الدقيقة لترتيب الأصول، الفلاتر، المفضلة، pagination/virtualization، وما يظهر بجانب كل أصل لم تُحسم بعد.

---

## 3. فتح تفاصيل السهم/الأصل داخل نفس مساحة السوق

عندما يختار المستخدم سهمًا/أصلًا من `Asset Sidebar`، لا يغادر إلى صفحة منفصلة تفصل المستخدم عن السوق.

تُستبدل محتويات **Main Workspace** بتفاصيل السهم/الأصل المختار، بينما تبقى `Asset Sidebar` ظاهرة في مكانها مع قائمة الأصول وخاصية البحث.

```text
MARKET HOME
     ├─ Main Workspace
     │      ↓ User selects an asset
     │   ASSET DETAIL / ANALYSIS
     │
     └─ Asset Sidebar
          ├─ Remains Visible
          ├─ Stocks / Assets in this Market
          └─ Search
```

وبالتالي يستطيع المستخدم الانتقال بسرعة بين الأصول بدون فقدان سياق السوق أو الرجوع للخلف في كل مرة.

المبدأ الحالي:

```text
SELECT ASSET
=
CHANGE MAIN WORKSPACE CONTENT

SELECT ASSET
!=
LEAVE MARKET WORKSPACE
```

### ترتيب المحتوى داخل Asset Detail

عند فتح السهم/الأصل، تكون الأولوية البصرية في المساحة الرئيسية كما يلي:

```text
ASSET DETAIL
     ↓
PRICE + PRICE STATUS
     ↓
CHART
     ↓
FALCON SUMMARY
     ↓
FALCON ANALYSIS / OPINION
```

المقصود أن المستخدم يرى **السعر والرسم البياني أولًا** حتى يبدأ من حقيقة السوق المرئية، ثم يأتي تحته **ملخص Falcon وتحليله/رأيه** عندما تكون هذه المخرجات متاحة بصورة authoritative من FSATS.

```text
MARKET FACTS FIRST
FALCON INTERPRETATION SECOND
```

وجود تحليل أو رأي Falcon لا يحول الـWeb إلى مالك للتحليل أو توصية مستقلة، ولا يجوز للواجهة اختراع رأي أو حالة غير مقدمة من الجهة المالكة.

### التحكم بالفترة الزمنية للرسم

يجب أن يستطيع المستخدم تغيير الفترة الزمنية للرسم **بسهولة ومن نفس مكان الرسم**، بدون الدخول في شاشة إعدادات منفصلة.

مثال UX مرئي مبدئي فقط:

```text
1D | 1W | 1M | 3M | 1Y | MORE
```

الأسماء الدقيقة للفترات ليست ثابتة بعد، وقد تختلف بحسب ما يدعمه السوق/الأصل ومصدر البيانات، لكن مبدأ UX ثابت:

```text
CHANGE TIMEFRAME = DIRECT / FAST / VISIBLE
CHANGE TIMEFRAME != OPEN COMPLEX SETTINGS
```

اختيار المستخدم لفترة زمنية يغير عرض الرسم فقط ضمن البيانات المتاحة authoritative، ولا يعني تغيير استراتيجية أو قرار تداول أو صلاحية تنفيذ.

### مدارس واستراتيجيات Falcon كطبقات على الشارت

يجب أن يستطيع المستخدم اختيار **مدرسة أو استراتيجية واحدة أو أكثر** من المدارس والاستراتيجيات التي يعرّفها FSATS، وعرض نتائجها/عناصرها البصرية على نفس الرسم عندما تكون هذه المخرجات متاحة من Application بصورة governed وauthoritative.

المستخدم ليس مقيدًا باختيار واحد فقط. هو يقرر إن كان يريد:

- مدرسة واحدة؛
- استراتيجيتين مثلًا؛
- أكثر من مدرسة/استراتيجية في نفس الوقت؛
- أو لا شيء منها والعودة إلى الرسم الأساسي النظيف.

تصور UX مبدئي:

```text
CHART LAYERS

Falcon Schools / Strategies
[x] School A
[x] Strategy B
[ ] Strategy C
[x] School D

Indicators
[x] RSI
[ ] MACD
[x] Moving Average
[ + Add Indicator ]
```

كل طبقة مفعلة يجب أن تكون قابلة للتمييز بصريًا وإخفائها/إظهارها بسهولة، حتى لا يتحول الشارت إلى فوضى غير مفهومة عندما يختار المستخدم عدة طبقات.

```text
ONE OVERLAY = ALLOWED
MULTIPLE OVERLAYS = ALLOWED
USER CHOOSES THE COMBINATION
```

اختيار عدة مدارس/استراتيجيات معًا هو **عرض تحليلي متزامن** وليس دمجًا تلقائيًا لقراراتها ولا إثباتًا لاتفاقها.

```text
MULTIPLE STRATEGY OVERLAYS
!=
ONE COMBINED TRADING DECISION
```

إذا كانت مدرستان أو استراتيجيتان تعرضان إشارات أو مستويات متعارضة، يجب الحفاظ على الاختلاف بصريًا وعدم جعل الـWeb يوحّدها من عنده.

### المؤشرات الإضافية التي يختارها المستخدم

بالإضافة إلى طبقات مدارس واستراتيجيات Falcon، يملك المستخدم حرية إضافة مؤشرات فنية إضافية على الرسم إذا أراد، ضمن الأدوات التي يوفرها الشارت فعليًا.

الفكرة:

```text
FALCON SCHOOL / STRATEGY LAYERS
+
USER-SELECTED TECHNICAL INDICATORS
=
CUSTOM CHART WORKSPACE
```

إضافة المستخدم لمؤشر لا تغيّر تحليل Falcon الأصلي ولا تجعل المؤشر جزءًا من استراتيجية Falcon تلقائيًا.

```text
USER ADDS INDICATOR
!=
FALCON STRATEGY CHANGED
```

الـWeb لا يملك تعريف المدارس أو الاستراتيجيات أو قواعدها أو حساباتها. هو يوفر تجربة الاختيار والعرض فقط، بينما الهوية والمعنى والمخرجات التحليلية تأتي من FSATS.

### إضافة وإزالة الطبقات والحفظ الاختياري كقالب قابل لإعادة الاستخدام

المستخدم يملك حرية إضافة أو إزالة مدارس Falcon، الاستراتيجيات، والمؤشرات من الشارت في أي وقت ضمن الأدوات المتاحة له.

لا يتم افتراض أن كل تعديل يجب أن يُحفظ تلقائيًا كإعداد دائم. عندما يبني المستخدم تركيبة يريد الاحتفاظ بها، تسأله الواجهة بصورة واضحة إن كان يريد **حفظ هذا التكوين كقالب Chart Layout / Preset** ليتمكن من تطبيقه لاحقًا على سهم/أصل آخر.

تصور UX مبدئي:

```text
CURRENT CHART CONFIGURATION
  ├─ School A
  ├─ Strategy B
  ├─ Strategy C
  ├─ RSI
  └─ Moving Average

[ Save this setup? ]
        ↓ Yes
SAVE AS USER CHART PRESET
        ↓
AVAILABLE TO APPLY ON ANOTHER ASSET
```

المبدأ:

```text
EDIT CURRENT CHART = FREE / USER CONTROLLED
SAVE PRESET = EXPLICIT USER CHOICE
SAVED PRESET = REUSABLE ON OTHER ASSETS
```

عدم اختيار الحفظ لا يعني أن الواجهة يجب أن تحول التكوين تلقائيًا إلى preset دائم.

القالب المحفوظ يمثل **اختيارات عرض المستخدم** فقط. لا يحول مجموعة المدارس/الاستراتيجيات إلى استراتيجية Falcon جديدة، ولا يمنح أي صلاحية تداول أو تنفيذ.

```text
SAVED CHART PRESET
!=
NEW FALCON STRATEGY
!=
TRADING AUTHORITY
```

التفاصيل الدقيقة لاسم القالب، عدد القوالب، تعديل/حذف/إعادة تسمية القوالب، وهل يمكن جعله Default، وكيفية التعامل مع مدرسة/استراتيجية أو مؤشر غير متاح لأصل أو سوق آخر، ستُحسم لاحقًا في التصميم التفصيلي.

التفاصيل الدقيقة لأسماء المدارس والاستراتيجيات، شكل كل overlay، عدد الطبقات القصوى تقنيًا، ترتيبها، الشفافية والألوان، تعارض الرسومات، وقائمة المؤشرات المدعومة ستُحسم لاحقًا عند التصميم التفصيلي والعقود الفعلية.

---

## 4. التوسع عند إضافة سوق جديد

الواجهة يجب أن تكون قابلة للتوسع بدون إعادة تصميم الصفحة عند كل إضافة سوق جديدة.

```text
CURRENT MARKET ADDED TO FSATS
        ↓
MARKET BECOMES AVAILABLE TO USER
        ↓
NEW MARKET CARD APPEARS IN FSATS HOME
```

يعني عند إضافة سوق جديد مستقبلًا، يظهر له مربع جديد ضمن نفس منطقة الأسواق بدل الحاجة إلى تعديل نموذج التنقل الأساسي يدويًا كفكرة UX.

```text
NEW MARKET = NEW MARKET CARD
NOT
NEW MARKET = REDESIGN FSATS HOME
```

التفاصيل الدقيقة لكيفية اكتشاف الأسواق المتاحة، ترتيبها، صلاحيات ظهورها، حالة الاشتراك، مصدر مؤشرها العام، وحالات stale/unknown/unavailable ستُحسم لاحقًا عبر العقود والجهة المالكة.

---

## 5. الهدف من Market Cards

الهدف أن يرى المستخدم من النظرة الأولى **خريطة الأسواق التي يدعمها Falcon حاليًا** بدل أن يشعر أنه داخل واجهة مبنية حول سوق واحد فقط.

الـMarket Cards تعطي:

- صورة سريعة عن كل سوق متاح.
- نقطة دخول طبيعية للسوق الذي يريد المستخدم استكشافه.
- قابلية توسع مستقبلية مع إضافة أسواق جديدة.
- إحساسًا بأن FSATS منصة تداول متعددة الأسواق وليست صفحة ثابتة لسوق محدد.

---

## 6. Truth Boundary

الـWeb يعرض فقط market status / index / summary / opportunities / analysis الذي يصل بصورة governed وauthoritative من FSATS.

```text
MARKET CARD = PRESENTATION
MARKET CARD != MARKET TRUTH OWNER
MARKET CARD != RECOMMENDATION

MARKET HOME = PRESENTATION / NAVIGATION
MARKET HOME != BUSINESS TRUTH OWNER

ASSET DETAIL VIEW = PRESENTATION
ASSET DETAIL VIEW != ASSET/TRADING TRUTH OWNER

SCHOOL / STRATEGY OVERLAY = APPLICATION-OWNED ANALYTICAL OUTPUT PRESENTATION
OVERLAY SELECTION != STRATEGY AUTHORITY
USER INDICATOR != FALCON STRATEGY SEMANTICS
SAVED CHART PRESET = USER PRESENTATION PREFERENCE
SAVED CHART PRESET != TRADING OR STRATEGY AUTHORITY
```

إذا كانت بيانات سوق أو أصل أو طبقة تحليلية ما غير متاحة أو قديمة أو غير معروفة، لا يتم تحويلها بصريًا إلى حالة طبيعية أو حديثة من عند الـWeb.

---

هذا الملف لا يعني `APPROVED` ولا `IMPLEMENTATION AUTHORIZED`.