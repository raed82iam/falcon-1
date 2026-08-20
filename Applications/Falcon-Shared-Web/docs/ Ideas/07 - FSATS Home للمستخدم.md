# Shared Falcon Web - FSATS Home للمستخدم

**Status:** `DRAFT / DISCUSSION ONLY`

هذا الملف يسجل قرارات تجربة الدخول إلى FSATS للمستخدم العادي بصورة تدريجية، سؤالًا وقرارًا في كل مرة، حتى لا تتداخل التفاصيل قبل حسمها.

هذا تخطيط UX/Product فقط ولا يمنح implementation/runtime/execution authority.

---

## 1. قرار صفحة الدخول الأساسية إلى FSATS

عندما يفتح المستخدم العادي تطبيق `FSATS` من `My Applications`، لا يدخل مباشرة إلى شاشة AI Chat مستقلة.

يدخل أولًا إلى:

`FSATS Home`

وتكون هذه الصفحة هي الصفحة الرئيسية للتطبيق من منظور المستخدم.

القرار الحالي:

```text
My Applications
      ↓
Open FSATS
      ↓
FSATS Home
      ↓
Market Summary + Falcon Insights/Opportunities + AI Assistant
```

يعني:

- يوجد ملخص مفيد عن السوق على صفحة `FSATS Home`.
- يوجد أيضًا قسم واضح يعرض أهم التحليلات/الفرص/الإشارات التي يقدّمها FSATS للمستخدم بصورة authoritative عندما تكون متاحة.
- الـAI Assistant متاح من نفس الصفحة، لكنه لا يحتل الشاشة الرئيسية بشكل دائم.
- المستخدم يستطيع من الصفحة نفسها أن يرى صورة السوق العامة، يلاحظ ما يراه Falcon مهمًا، ثم يفتح الـAI إذا أراد فهم أي شيء أو التعمق فيه.
- التفاصيل الدقيقة لمحتوى الملخص وترتيب العناصر ستُحسم تدريجيًا لاحقًا.

---

## 2. التوجه العام لتجربة الصفحة

الهدف من `FSATS Home` هو أن يشعر المستخدم منذ دخوله أنه موجود داخل **بيئة تداول احترافية**، حتى لو كانت خبرته في التداول محدودة أو صفر.

هذا يعني أن الصفحة يجب أن تجمع بين:

```text
PROFESSIONAL TRADING ENVIRONMENT
+
CLEAR / UNDERSTANDABLE USER EXPERIENCE
```

ولا يعني الاحتراف إغراق المستخدم بالمصطلحات أو الأرقام غير المفهومة.

```text
PROFESSIONAL != HARD TO UNDERSTAND
SIMPLE TO USE != AMATEUR
```

الصفحة يجب أن تبدو غنية واحترافية من ناحية عرض السوق والمعلومات، بينما يبقى الـAI متاحًا لشرح أي معلومة أو مصطلح أو حركة سوق للمستخدم بصورة طبيعية.

---

## 3. أول ملخص يراه المستخدم

القرار الحالي:

**الجزء العلوي من `FSATS Home` لا يعتمد على عنصر واحد فقط. يجب أن يجمع بين صورة السوق العامة وبين أهم تحليلات/فرص Falcon الحالية.**

يعني المستخدم لا يدخل ويرى أسعارًا ومؤشرات فقط، ولا يدخل أيضًا ويرى توصيات Falcon بدون سياق السوق.

الاتجاه الحالي:

```text
MARKET PICTURE
+
FALCON CURRENT INSIGHTS / OPPORTUNITIES
```

الهدف أن يفهم المستخدم في ثوانٍ:

1. ماذا يحدث في السوق؟
2. ما الذي يراه Falcon مهمًا الآن؟

المحتوى الدقيق لم يُحسم بعد، لكن من منظور UX قد يغطي لاحقًا فقط المعلومات التي يوفّرها FSATS بصورة authoritative، مثل:

### Market Picture

- حالة الأسواق ذات الصلة.
- الاتجاه العام أو صورة السوق.
- أهم التحركات أو الأحداث المهمة.
- الأصول/القطاعات/المناطق البارزة عندما تكون هذه semantics متاحة من FSATS.
- التنبيهات أو المخاطر العامة المهمة.
- freshness / timestamp / stale state عندما يلزم.

### Falcon Insights / Opportunities

- أهم التحليلات الحالية.
- فرص أو حالات تستحق الانتباه حسب مخرجات FSATS الفعلية.
- نقاط مراقبة أو تغيّرات مهمة يبرزها التطبيق.
- مستوى الثقة/المخاطر/عدم اليقين عندما توفره الجهة المالكة authoritative.
- مدخل مباشر لمناقشة أي تحليل أو فرصة مع الـAI.

هذه أمثلة UX فقط وليست قائمة contract fields نهائية، ولا يحق للـWeb اختراع market/business truth أو opportunity semantics من عنده.

---

## 4. العلاقة مع تجربة AI الحالية

هذا القرار يكمل ملف:

`06 - محادثة المستخدم مع AI.md`

ويثبت موضع الدخول الأساسي فقط:

```text
FSATS HOME = USER LANDING PAGE
MARKET PICTURE = PRIMARY MARKET CONTEXT
FALCON INSIGHTS / OPPORTUNITIES = PRIMARY INTELLIGENCE CONTEXT
AI ASSISTANT = PERSISTENTLY AVAILABLE, USER-OPENED EXPERIENCE
```

ولا يغيّر القواعد المحسومة سابقًا:

- المستخدم قد تكون معرفته بالتداول صفر.
- الاستكشاف والاستشارة والتحليل لا تتطلب user broker/API credentials.
- Automated Trading مسار منفصل يحتاج opt-in واضح.
- AI Chat لا يخلق execution authority.

الـAI يساعد المستخدم على فهم ما يراه في الصفحة، مثل أن يسأل:

```text
ليش السوق نازل اليوم؟
ليش Falcon شايف هاي الفرصة مهمة؟
شو معنى هالمؤشر؟
شو أهم شي لازم أنتبهله هون؟
فسرلي هالحركة بطريقة بسيطة.
```

ثم يعتمد الرد على المعلومات/التحليل authoritative القادمة من FSATS، وليس على Web-created business truth.

---

## 5. Professional Trading Feel مع مستخدم مبتدئ

يجب الحفاظ على معادلة أساسية:

```text
LOOK AND FEEL = PROFESSIONAL TRADING ENVIRONMENT
INTERACTION = NATURAL / GUIDED / NOVICE-FRIENDLY
```

المستخدم المحترف يجب ألا يشعر أن الواجهة لعبة تعليمية مبسطة أكثر من اللازم، والمستخدم المبتدئ يجب ألا يشعر أنه دخل قمرة قيادة لا يعرف كيف يبدأ منها.

الـAI هو الجسر بين الاثنين:

```text
RICH MARKET INFORMATION
+
FALCON ANALYSIS / OPPORTUNITY CONTEXT
        ↓
USER CAN EXPLORE DIRECTLY
        OR
OPEN AI ASSISTANT
        ↓
AI EXPLAINS / GUIDES / REQUESTS FSATS ANALYSIS
```

---

## 6. AI Assistant Presence Across FSATS

قرار الـOwner الحالي:

**الـAI Assistant يكون متاحًا في جميع صفحات FSATS، وليس فقط في `FSATS Home`.**

الطريقة المرشحة:

```text
ANY FSATS PAGE
      ↓
Persistent AI entry / icon
      ↓
Open Side Panel
      ↓
Continue conversation while page context remains visible
      ↓
Optional Expand
      ↓
Full-Page AI Experience
```

القواعد الحالية:

- الوضع الافتراضي هو `Side Panel` يمكن فتحه وإغلاقه عند الحاجة.
- إغلاق اللوحة لا يعني إنهاء المحادثة أو حذف السياق تلقائيًا.
- الـSide Panel يسمح للمستخدم بالبقاء في صفحة السوق/التحليل/portfolio أو غيرها أثناء الحديث مع الـAI.
- المستخدم يستطيع تكبير الـAI إلى `Full Page` عندما يحتاج مساحة أكبر للمحادثة أو عرض تحليل طويل أو جداول/رسوم.
- يجب أن يستطيع الرجوع من `Full Page` إلى الصفحة السابقة بدون فقدان المحادثة ضمن سياسة الـsession/history التي ستُحسم لاحقًا.
- وجود الـAI في كل الصفحات لا يعني أن له صلاحية على كل ما هو معروض. الوصول للبيانات والطلبات يبقى تابعًا لصلاحيات المستخدم والعقود authoritative.

### Automatic Page Context مع User Override

قرار الـOwner الحالي:

**عندما يفتح المستخدم الـAI من صفحة ذات سياق واضح، يعرف الـAI تلقائيًا سياق الصفحة الحالية، مع بقاء حرية المستخدم في السؤال عن شيء آخر.**

مثال:

```text
User is viewing: Apple / AAPL
        ↓
Open AI Side Panel
        ↓
AI context includes current page = Apple / AAPL
        ↓
User: شو رأيك فيه؟
        ↓
AI understands "فيه" = Apple / AAPL
```

لكن هذا السياق هو `default context` فقط وليس قيدًا على المحادثة:

```text
CURRENT PAGE CONTEXT = DEFAULT CONVERSATION CONTEXT
!=
ONLY ALLOWED CONVERSATION TOPIC
```

لذلك يستطيع المستخدم أن يقول مثلًا:

```text
طيب اترك Apple، احكيلي عن Microsoft.
```

فيتحول موضوع المحادثة حسب طلبه الواضح دون إجباره على مغادرة الصفحة أولًا.

القواعد الحالية:

- الـAI يلتقط تلقائيًا السياق المسموح من الصفحة الحالية، مثل الأصل/السوق/التحليل/العنصر الذي يشاهده المستخدم، عندما توفره FSATS بصورة governed وauthoritative.
- لا يجب على المستخدم إعادة اسم السهم أو العنصر في كل سؤال متابعة إذا كان المقصود واضحًا من الصفحة والسياق الجاري.
- المستخدم يستطيع override للسياق في أي لحظة بكلام طبيعي.
- تغيير موضوع المحادثة لا يعني تلقائيًا تغيير الصفحة المفتوحة في الواجهة.
- انتقال المستخدم لاحقًا إلى صفحة أخرى يمكن أن يقدم سياق صفحة جديدًا، لكن قواعد دمج هذا السياق مع conversation history ستُحسم في الـwireframes/contracts لاحقًا.
- الـWeb لا يرسل للـAI معلومات مخفية أو غير مصرح بها لمجرد أنها موجودة تقنيًا في الصفحة أو session.
- إذا كان السياق غامضًا، يجب على الـAI طلب clarification بسيط بدل التخمين.

المبدأ:

```text
AI KNOWS WHERE THE USER IS
BUT THE USER CHOOSES WHAT TO TALK ABOUT
```

و:

```text
PAGE CONTEXT != USER INTENT
USER EXPLICIT INTENT OVERRIDES PAGE CONTEXT
```

### Non-Intrusive Context Invitation

قرار الـOwner الحالي:

**عند انتقال المستخدم إلى صفحة أصل/شركة مثل Microsoft، لا يبدأ الـAI بالكلام أو التقييم من تلقاء نفسه. بدل ذلك يظهر اقتراح صغير وغير مزعج مثل:**

```text
اسألني عن Microsoft
```

الهدف من هذا السلوك هو إعطاء المستخدم إحساسًا بالأمان والسيطرة على التجربة:

```text
AI IS AVAILABLE
+
AI UNDERSTANDS CONTEXT
+
USER CHOOSES WHEN CONVERSATION STARTS
```

القواعد الحالية:

- فتح صفحة شركة أو أصل لا يطلق تلقائيًا رسالة محادثة جديدة من الـAI.
- لا يصدر الـAI تقييمًا تلقائيًا مثل `اختيارك ممتاز` لمجرد فتح الصفحة.
- يمكن عرض contextual prompt صغير فقط، مرتبط بسياق الصفحة الحالية، مثل `اسألني عن Microsoft`.
- المستخدم هو من يقرر إن كان يريد فتح الـSide Panel وبدء الحوار.
- إذا بدأ المستخدم الحوار، يستطيع الـAI استخدام سياق الصفحة المصرح به لتقديم شرح أو تحليل أو مقارنة أو طلب دراسة أعمق من FSATS.
- أي تقييم إيجابي/سلبي أو recommendation يجب أن يعتمد على مخرجات FSATS authoritative، وليس على مجرد navigation event.

المبدأ:

```text
CONTEXT AWARENESS != UNSOLICITED ADVICE
PAGE OPEN != RECOMMENDATION
USER CONTROL > AI INTERRUPTION
```

المبدأ العام للوجود:

```text
AI ALWAYS AVAILABLE
!=
AI ALWAYS OCCUPYING THE SCREEN
```

و:

```text
SIDE PANEL FOR QUICK / CONTEXTUAL HELP
FULL PAGE FOR DEEP CONVERSATION / ANALYSIS
```

---

## 7. ما لم يُحسم بعد

لم نحسم بعد داخل `FSATS Home` وتجربة الـAI:

- ما هي العناصر الدقيقة داخل `Market Picture`.
- ما هي العناصر الدقيقة داخل `Falcon Insights / Opportunities`.
- كيف يتم ترتيب الاثنين بصريًا في أول View.
- مكان زر/أيقونة فتح الـAI بالضبط داخل shell.
- العرض الافتراضي للـSide Panel.
- exact governed page-context payload وآلية ربطه بالمحادثة.
- كيف يتم دمج page context الجديد مع conversation history عند تنقل المستخدم بين الصفحات.
- الشكل البصري الدقيق للـcontextual prompt مثل `اسألني عن Microsoft`.
- هل توجد بطاقات سريعة للأسواق أو watchlist أو portfolio أو recommendations.
- ما الذي يظهر لمستخدم جديد ليس لديه أي portfolio أو trading history.
- ما الذي يتغير بعد تفعيل Automated Trading.

هذه النقاط ستُحسم واحدة واحدة مع الـProject Owner.

---

هذا الملف لا يعني `APPROVED` ولا `IMPLEMENTATION AUTHORIZED`.