# Shared Falcon Web - عقود بيانات الشارت مع FSATS

**Status:** `DRAFT / DISCUSSION ONLY`

هذا الملف يثبت حدود Shared Web واحتياجاته العابرة للـworkstreams بخصوص بيانات الشارت وFalcon Schools/Strategies overlays وطلب تحليل أصل خارج القائمة الحالية.

لا يمنح هذا الملف implementation/runtime/trading authority.

---

## 1. حد الملكية

Shared Web يملك العرض والتفاعل فقط.

لا يملك:

- اختيار مزود البيانات.
- التحقق من جودة أو دقة مزود البيانات.
- إدارة quotas أو routing للمزودين.
- بناء market-data truth.
- منطق مدارس Falcon أو استراتيجياته.
- منطق تحليل FSATS للأصول.
- صلاحية تحديد applicability لمدرسة أو استراتيجية على أصل معين.
- صلاحية إدخال أصل إلى trading/analysis universe من نفسه.

هذه المعاني تبقى عند FSATS/FSAPMA والـApplication المالكة.

```text
WEB_DISPLAY != PROVIDER_MANAGEMENT
WEB_DISPLAY != MARKET_DATA_TRUTH
WEB_OVERLAY_RENDERING != SCHOOL_OR_STRATEGY_LOGIC
WEB_REQUEST_TRANSPORT != ANALYSIS_AUTHORITY
```

---

## 2. بيانات السعر والشارت

الشارت يحتاج أكثر من السعر اللحظي.

Shared Web يحتاج من الـApplication عقدًا canonical يغطي:

- طلب أصل محدد.
- طلب أصل قد يكون خارج القائمة/الـuniverse الذي يديره FSATS حاليًا.
- historical time series.
- timeframe/resolution.
- bounded history range.
- OHLC and Volume where applicable or the canonical normalized alternative supplied by Application.
- current/live continuation after historical data.
- corrections, gaps, stale/unknown/unavailable/degraded and partial-result semantics.

Shared Web لا يذهب مباشرة إلى provider ولا يحدد للـFSAPMA من أي provider يجلب البيانات.

```text
CUSTOMER_VIEW_REQUEST != TRADING_UNIVERSE_ADMISSION
WEB_CHART_DATA_REQUEST != PROVIDER_SELECTION_AUTHORITY
```

تم فتح `FCR-0125`، ورد الـApplication وحدد العقد canonical لطلبات الشارت، historical/current continuation، truth/freshness/gap/correction semantics، وon-demand display خارج Trading universe بدون universe admission.

حالة التخطيط الحالية:

```text
APPLICATION_CONTRACT_DEFINED = YES
WEB_PLANNING_COMPATIBILITY = VERIFIED
WEB_IMPLEMENTATION = NOT AUTHORIZED / NOT COMPLETED
FCR-0125 = OPEN / Waiting On: WEB
```

---

## 3. صيغة الرسائل والـprocedure

الـApplication حدد للعقد الحالي عائلات الرسائل الموجودة بين Web وFSAPMA/Trading، وحدد identities التخطيطية التالية للشارت:

```text
FSATS.WebChartDataRequest.v1
FSATS.WebChartHistoricalProjection.v1
FSATS.WebChartUpdateProjection.v1
```

ويشمل ذلك request/correlation identity، instrument identity، timeframe/range، historical bars/series، current continuation، truth/freshness، gaps، corrections، partial/unavailable/unsupported/needs-clarification/rejected states.

Shared Web لا يخترع provider route ولا message business semantics من نفسه.

---

## 4. Falcon Schools وStrategies على الشارت

عند اختيار المستخدم School أو Strategy للعرض، Shared Web يستلم projection من Trading يخبره **ماذا يرسم** بدون أن يعيد حساب منطق المدرسة أو الاستراتيجية.

الـApplication حدد identities التخطيطية التالية:

```text
FSATS.WebTradingOverlayRequest.v1
FSATS.WebTradingOverlayProjection.v1
FSATS.WebTradingOverlayUpdate.v1
```

وحدد provider-neutral render primitives قابلة للرسم مثل:

- `POINT`
- `PRICE_LEVEL`
- `HORIZONTAL_LINE`
- `VERTICAL_LINE`
- `ZONE`
- `SERIES`
- `MARKER`
- `ANNOTATION`

كما حدد authoritative applicability/result states وتحديث/تصحيح/invalidate/remove/status lifecycle.

```text
USER_SELECTED_OVERLAY != APPLICATION_CONFIRMED_APPLICABILITY
WEB_RENDER_ATTEMPT != STRATEGY_ACTIVATION_AUTHORITY
NOT_APPLICABLE != SILENTLY_APPLIED
DISPLAYED_OVERLAY != CURRENT_APPLICATION_TRUTH
```

بالتالي، **البيانات الراجعة من Trading كافية تخطيطيًا ليعكسها Shared Web بصريًا على الشارت** بدون إعادة حساب منطق School/Strategy.

حالة التخطيط الحالية:

```text
APPLICATION_OVERLAY_CONTRACT_DEFINED = YES
WEB_PLANNING_COMPATIBILITY = VERIFIED
WEB_IMPLEMENTATION = NOT AUTHORIZED / NOT COMPLETED
FCR-0126 = OPEN / Waiting On: WEB
```

---

## 5. طلب تحليل أصل خارج القائمة الحالية

طلب العميل لتحليل سهم/أصل غير موجود في القائمة الحالية ليس مجرد طلب market data للشارت.

الـApplication حدد مسارًا Trading-owned مستقلًا لطلب التحليل وإرجاع النتيجة authoritative بدون أن يحلل Web الأصل بنفسه وبدون أن يعتبر الطلب إدخالًا تلقائيًا إلى universe.

```text
FSATS.WebOnDemandAnalysisRequest.v1
FSATS.WebOnDemandAnalysisResult.v1
```

```text
CUSTOMER_ANALYSIS_REQUEST != TRADING_UNIVERSE_ADMISSION
CUSTOMER_ANALYSIS_REQUEST != STRATEGY_ACTIVATION
ON_DEMAND_ANALYSIS != SILENT_UNIVERSE_MUTATION
```

حالة التخطيط الحالية:

```text
APPLICATION_ON_DEMAND_ANALYSIS_CONTRACT_DEFINED = YES
WEB_PLANNING_COMPATIBILITY = VERIFIED
WEB_IMPLEMENTATION = NOT AUTHORIZED / NOT COMPLETED
FCR-0127 = OPEN / Waiting On: WEB
```

---

## 6. المؤشرات الفنية القياسية داخل واجهة التداول

قرار الـOwner الحالي:

المؤشرات الفنية القياسية البحتة التي تعتمد على بيانات الشارت المستلمة، مثل RSI وMACD وSMA وEMA وBollinger Bands وما يشابهها من مؤشرات قياسية، يمكن حسابها داخل Shared Web/واجهة التداول من الـOHLCV authoritative الذي يستلمه Web.

الهدف هو جعل الشارت سريعًا وتفاعليًا عند تغيير period/settings بدون إرسال business-analysis request إلى Trading مع كل تعديل بصري.

لكن هذا الحساب يبقى **حساب عرض تقني فقط** ولا يخلق تحليل Falcon أو قرار تداول أو Strategy/School semantics.

```text
STANDARD_TECHNICAL_INDICATOR_CALCULATION = WEB_PRESENTATION_CAPABILITY
STANDARD_INDICATOR_VALUE != FALCON_TRADING_ANALYSIS
STANDARD_INDICATOR_VALUE != BUY_SELL_DECISION
STANDARD_INDICATOR_VALUE != STRATEGY_ACTIVATION
FALCON_SPECIFIC_ANALYSIS_OR_OVERLAY = APPLICATION_OWNED
```

إذا أصبح مؤشر ما في المستقبل Falcon-specific أو جزءًا من School/Strategy/analysis semantics، لا يحسبه Web كحقيقة مستقلة؛ يستلمه من الجهة Application المالكة.

---

## 7. قائمة Schools/Strategies المتاحة للعرض

Shared Web **لا يحتفظ بقائمة ثابتة hard-coded** لمدارس Falcon أو استراتيجياته.

الـApplication رد على `FCR-0128` وحدد dynamic catalog/discovery contract بالـidentities التالية:

```text
FSATS.WebStrategyCatalogRequest.v1
FSATS.WebStrategyCatalogProjection.v1
FSATS.WebStrategyCatalogUpdate.v1
```

Trading/T-LSA-06 هو authoritative owner للـStrategy Registry/Controller والـSchools/Strategies المسجلة مركزيًا.

الـcatalog يدعم stable identity، نوع العنصر `SCHOOL | STRATEGY`، display/localization reference، version/effective/as-of، truth state، availability state، discovery-level market/asset scope، entitlement reference عند الحاجة، applicability-check requirement، replacement/reason semantics، والتحديثات الديناميكية مثل add/update/availability/deprecate/retire/replace/remove/status.

حالات truth تشمل:

```text
CURRENT
STALE
UNKNOWN
UNAVAILABLE
```

وحالات availability تشمل:

```text
AVAILABLE
TEMPORARILY_UNAVAILABLE
DEPRECATED
RETIRED
REPLACED
UNKNOWN
```

### قرار الـOwner لعرض العناصر غير المتوافقة

عند فتح selector الخاص بالـSchools/Strategies:

- العناصر الموجودة authoritative في Falcon catalog تبقى ظاهرة في القائمة.
- العنصر المتوافق مع الأصل/السوق الحالي يكون قابلًا للاختيار وفق entitlement وحالة الـApplication.
- العنصر الموجود في Falcon لكنه غير متوافق مع الأصل/السوق الحالي يبقى **ظاهرًا ولكن Disabled**.
- بجانب العنصر Disabled يظهر سبب بشري واضح يشرح عدم التوافق، اعتمادًا على السبب/الحالة التي يرجعها الـApplication، وليس على استنتاج Web.
- لا نخفي العنصر فقط لأنه غير متوافق مع الأصل الحالي، لأن إظهاره يساعد المستخدم يعرف قدرات Falcon الموجودة ويعرف لماذا لا يستطيع استخدامها هنا.
- العناصر التي لم تعد موجودة authoritative، أو أزيلت/retired بطريقة تجعلها غير قابلة للعرض وفق contract الحالي، لا يحتفظ Web بها كأنها capability حالية.

```text
CATALOG_PRESENT + NOT_APPLICABLE_TO_CURRENT_ASSET -> VISIBLE_DISABLED_WITH_REASON
DISABLED_SELECTOR_ITEM != UNAVAILABLE_FROM_FALCON_CATALOG
DISABLED_SELECTOR_ITEM != STRATEGY_ACTIVATED
WEB_DISABLED_REASON != WEB_INVENTED_APPLICABILITY
CATALOG_PRESENT != APPLICABLE_TO_CURRENT_ASSET
CATALOG_AVAILABLE != STRATEGY_ACTIVATED
SELECTOR_VISIBLE != TRADE_AUTHORIZED
```

FCR-0128 يحدد ما هو موجود ويمكن عرضه في الـselector، بينما FCR-0126 يبقى المرجع authoritative للحكم النهائي على asset-specific applicability ولـoverlay projection بعد الاختيار.

حالة التخطيط الحالية:

```text
APPLICATION_DYNAMIC_CATALOG_CONTRACT_DEFINED = YES
WEB_PLANNING_CONSUMPTION = COMPLETE
WEB_PLANNING_COMPATIBILITY = VERIFIED
WEB_SELECTOR_POLICY = VISIBLE_DISABLED_WITH_REASON_FOR_INCOMPATIBLE_ITEMS
WEB_IMPLEMENTATION = NOT AUTHORIZED / NOT COMPLETED
FCR-0128 = OPEN / Waiting On: WEB
```

---

## 8. الفصل بين أنواع البيانات

بيانات السوق الأساسية، وFalcon analytical overlays، ونتيجة التحليل، والمؤشرات الفنية القياسية المحسوبة محليًا تبقى منفصلة semanticًا حتى لو جمعها Web في نفس الشارت.

```text
MARKET_DATA_SERIES != SCHOOL_STRATEGY_PROJECTION
SCHOOL_STRATEGY_PROJECTION != ANALYSIS_RESULT
STANDARD_WEB_INDICATOR != FALCON_ANALYSIS
WEB_COMPOSITION != BUSINESS_SEMANTIC_MERGE
```

Shared Web يعرضها، لكنه لا يملك حقيقة market data أو School/Strategy/analysis semantics.

---

## 9. شرط قبل التنفيذ

الـApplication رد تخطيطيًا على FCR-0125 وFCR-0126 وFCR-0127 وFCR-0128 وتم التحقق من توافق التخطيط، لكن implementation-ready binding يبقى غير مخول وغير مكتمل.

```text
DIRECT_PROVIDER_BINDING_FROM_SHARED_WEB = NO
WEB_INVENTED_APPLICATION_CONTRACT = NO
WEB_HARD_CODED_FALCON_STRATEGY_CATALOG = NO
IMPLEMENTATION_AUTHORITY = NOT_GRANTED
```

هذه المرحلة Planning/Discussion فقط.
