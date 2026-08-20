# دليل مستخدم Falcon Self-Aware Trading System (FSATS)

**الإصدار:** 2026-08-19  
**اللغة:** العربية  
**الفئة المستهدفة:** المالك، المتداول، المشغل، المشرف، المراجع، وأي مستخدم غير مطور  
**النظام:** Falcon Self-Aware Trading System (FSATS)  
**الوضع الحالي:** تنفيذ Applications وتحضير الـOnboarding تم التحقق منه تقنيًا، لكن الـFoundation Admission الفعلي، والـCanonical Runtime Registration، والـActivation، واتصال الـProviders/Brokers، وPaper/Shadow/Tiny-Live/Live، والـDeployment ما تزال قرارات وصلاحيات منفصلة ولا يعتبر هذا الدليل تفويضًا لها.

> هذا الدليل يشرح للمستخدم ما هو FSATS، كيف يفهم حالاته ومخرجاته، وكيف يتعامل معه بشكل صحيح. هذا الدليل لا يمنح صلاحية تداول أو تفعيل أو نشر، ولا يعتبر نصيحة مالية.

---

# 1. ما هو FSATS؟

FSATS هو نظام Falcon الذاتي الوعي للتداول. صُمم حتى يحلل، يحاكي، يحمي، ينسق، ويستعد لتنفيذ عمليات التداول ضمن حوكمة واضحة وفصل صارم بين:

- التحليل؛
- التوصية؛
- الصلاحية؛
- التنفيذ.

FSATS ليس برنامجًا واحدًا ضخمًا. هو System Boundary محكوم يتكون من خمس Applications مستقلة:

```text
1. Trading Application
2. FSAPMA — Falcon Self-Aware Provider Management Application
3. Falcon Trading Guardian Application
4. FSTSimA — Falcon Trading Simulation Application
5. APP-RSC — FSATS Resource Coordination Application
```

FSATS نفسه Boundary غير مالك وغير Runtime owner. كل Application تملك مسؤولياتها الخاصة.

---

# 2. وظيفة كل Application

## 2.1 Trading Application

Trading Application تملك منطق التداول وذكاء التداول داخل FSATS، ومنها:

- فهم السوق؛
- اختيار وتنسيق الاستراتيجيات؛
- تقييم الفرص؛
- Broker-account-scoped trading workflow؛
- إعداد قرارات التداول؛
- تجهيز أوامر التنفيذ عندما تكون الصلاحية موجودة؛
- حفظ Trading state وevidence؛
- تجميع القرار التداولي ضمن حدود المخاطر والصلاحيات.

قرار التداول لا يعني أن أمرًا أُرسل للبروكر.

```text
TRADING_DECISION != BROKER_EXECUTION
```

## 2.2 FSAPMA

FSAPMA تملك إدارة Providers التشغيلية الخاصة بـFSATS، مثل:

- معرفة capabilities لكل Provider؛
- تقييم ملاءمة الـProvider؛
- متابعة quotas وrate limits؛
- route readiness؛
- health/failure handling؛
- تنسيق operational market-data providers.

FSAPMA لا تصبح Trading لمجرد أن Trading يستهلك بيانات منها.

```text
PROVIDER_DATA != TRADING_AUTHORITY
```

## 2.3 Trading Guardian

Trading Guardian هو Application الحماية داخل نطاق التداول. وظيفته مراقبة شروط الحماية ودعم:

- restriction؛
- containment؛
- protection state؛
- safe operating decisions.

الحماية ليست استراتيجية تداول وليست Broker Authority.

```text
PROTECTION != TRADING_STRATEGY
PROTECTION != BROKER_AUTHORITY
```

## 2.4 FSTSimA

FSTSimA هو Application المحاكاة المستقلة والمحكومة. يدعم:

- simulation؛
- deterministic scenarios؛
- fault injection؛
- Digital City validation؛
- calibration؛
- replay؛
- non-Live qualification evidence.

نتيجة المحاكاة ليست حقيقة تشغيلية في السوق الحقيقي.

```text
SIMULATION_RESULT != LIVE_MARKET_TRUTH
SIMULATION_PASS != PAPER_AUTHORITY
SIMULATION_PASS != LIVE_AUTHORITY
```

## 2.5 APP-RSC

APP-RSC ينسق احتياجات الموارد داخل نطاق FSATS بين Applications الخمس، ويساعد على:

- فهم resource demand؛
- coordination؛
- degraded behavior؛
- المحافظة على حدود الموارد داخل FSATS.

APP-RSC ليس Foundation Resource Governance.

```text
APP-RSC != FOUNDATION_RESOURCE_GOVERNANCE
```

---

# 3. الـSelf-Awareness داخل FSATS

FSATS يستخدم Self-Awareness محكوم ومحدود.

التركيب الحالي:

```text
Trading:          1 MSA / 13 LSA / 3 CSA
FSAPMA:           1 MSA /  6 LSA / 1 CSA
Trading Guardian: 1 MSA /  4 LSA / 1 CSA
FSTSimA:          1 MSA /  8 LSA / 2 CSA
APP-RSC:          1 MSA /  3 LSA / 0 CSA initially

TOTAL: 5 Applications / 5 MSA / 34 LSA / 7 CSA
```

المعاني:

- **MSA** تفهم Application كاملة.
- **LSA** تفهم Major Branch داخل Application.
- **CSA** تفهم Component ذكي مؤهل.
- **FSA** موجودة على مستوى Foundation وليست داخل FSATS Applications.

المسار المفاهيمي:

```text
CSA -> LSA -> MSA -> FSA review when applicable
```

Self-Awareness لا تعني Authority.

```text
SELF_AWARENESS != AUTHORITY
```

---

# 4. هوية التشغيل التي يجب أن يفهمها المستخدم

FSATS في التداول يتعامل مع الـBroker Account كهوية تشغيل أعمال أساسية، وليس مع User ID داخلي في FSATS.

الهوية الأساسية:

```text
BrokerId + BrokerAccountId
```

وقد تكون Environment بعدًا إضافيًا عندما تكون مهمة.

FSATS لا يملك:

- Customer identity؛
- username؛
- contact mapping؛
- broker-account-to-customer mapping.

هذه تبقى ضمن Shared Web boundary.

هذا يمنع خلط حساب Broker بحساب آخر.

---

# 5. كيف تقرأ حالات FSATS؟

FSATS يفصل بين حالات قد تبدو متشابهة لكنها ليست واحدة.

## 5.1 Ready لا تعني Running

```text
RUNTIME_READY != RUNTIME_AUTHORIZED
ACTIVATION_ELIGIBLE != ACTIVE
ADMISSION_READY != ADMITTED
```

## 5.2 Registered لا تعني Active

```text
RUNTIME_REGISTERED != ACTIVATED
```

## 5.3 وجود Route لا يعني فتح Connection

```text
ROUTE_EXISTS != CONNECTION_AUTHORIZED
ROUTE_AUTHORIZED != CONNECTION_EXECUTED
```

## 5.4 وصول البيانات لا يعني السماح بالتداول

```text
DATA_ACCESS != BUSINESS_AUTHORITY
PROVIDER_CONNECTIVITY != EXECUTION_AUTHORITY
```

## 5.5 إرسال الطلب لا يعني نجاح الفعل

```text
REQUEST_SENT != ACTION_ACCEPTED != ACTION_COMPLETED
```

إذا الواجهة عرضت حالة من هذه الحالات، افهمها حرفيًا ولا تعتبرها خطوة أخرى ضمنيًا.

---

# 6. الحالة الحالية للنظام

في الحالة الحالية الموثقة:

- Parts 0 through 10 مقبولة ومغلقة رسميًا من Project Owner.
- Part 11 تم تنفيذ وتحقيق Application-side onboarding preparation فيها تقنيًا.
- تم تجهيز 5 Admission candidate packages.
- تم تجهيز 5 Runtime Registration templates.
- Foundation تحققت من generic plug-ready preparation by composition.
- لا يوجد Foundation redesign مطلوب.
- لا يوجد Application redesign مطلوب للحزمة الحالية.

لكن حاليًا:

```text
ACTUAL_ADMISSION                      = NOT AUTHORIZED / NOT EXECUTED
ACTUAL_CANONICAL_RUNTIME_REGISTRATION = NOT AUTHORIZED / NOT EXECUTED
RUNTIME_ACTIVATION                    = NOT AUTHORIZED / NOT EXECUTED
PROVIDER_CONNECTIVITY                 = NOT AUTHORIZED / NOT EXECUTED
BROKER_CONNECTIVITY                   = NOT AUTHORIZED / NOT EXECUTED
PAPER                                 = NOT AUTHORIZED
SHADOW                                = NOT AUTHORIZED
TINY-LIVE                             = NOT AUTHORIZED
LIVE                                  = NOT AUTHORIZED
DEPLOYMENT                            = NOT AUTHORIZED
```

المعنى للمستخدم بسيط: **النظام جاهز تقنيًا في نطاق التحضير، لكنه ليس مفعلًا Live تلقائيًا.**

---

# 7. ماذا يتوقع المستخدم من الواجهة؟

Shared Web Application هي طبقة العرض والتفاعل مع المستخدم. FSATS يبقى Backend trading-domain boundary.

حسب Features التي يتم تفعيلها بشكل منفصل، قد ترى مستقبلًا شاشات أو عناصر لـ:

- System Status؛
- Broker Account context؛
- Market/Instrument information؛
- Trading analysis؛
- Strategy outputs؛
- Guardian protection state؛
- Simulation and qualification results؛
- Resource/degradation status؛
- Provider status؛
- Alerts؛
- Decisions؛
- Evidence؛
- Owner-controlled actions؛
- Audit/History.

وجود شاشة أو زر لا يمنح صلاحية.

```text
PRESENTATION != AUTHORITY
```

---

# 8. كيف تفهم Analysis وRecommendation وAuthority وExecution؟

هذه أربع طبقات منفصلة:

## Analysis

ماذا يرى أو يستنتج Falcon؟

## Recommendation

ماذا يقترح Falcon؟

## Authority

هل الفعل المقترح مسموح فعلًا؟

## Execution

هل الفعل المسموح تم تنفيذه فعلًا وتم تأكيده؟

مثال:

```text
Analysis: opportunity detected
Recommendation: BUY candidate
Authority: not granted
Execution: none
```

هذا **ليس Trade**.

---

# 9. حالات Guardian

قد يعرض Guardian حالات حماية مثل:

- Normal؛
- Degraded؛
- Restricted؛
- Contained؛
- Safe State؛
- حالات أخرى محكومة.

افهم دائمًا:

```text
SAFE_STATE != NORMAL_OPERATION
CONTAINED != RELEASED
PROTECTIVE_RESTRICTION != BUSINESS_AUTHORITY
```

لا تحاول تجاوز Restriction فقط لأن فرصة السوق تبدو جيدة.

---

# 10. Simulation وDigital City

FSTSimA منفصل عمدًا عن التشغيل الحقيقي.

قد يعرض Evidence تخص:

- strategies؛
- failure scenarios؛
- degraded operation؛
- deterministic replay؛
- calibration؛
- fault injection؛
- scenario comparison؛
- Digital City validation.

استخدم النتائج لفهم السلوك وqualification evidence.

لا تعتبر نجاح Simulation ضمانًا لنتيجة Live.

---

# 11. Providers وMarket Data

عندما يتم تفويض Provider functionality مستقبلًا، FSAPMA ينسق الوصول حسب:

- capabilities؛
- quota؛
- route؛
- health؛
- failure rules.

افهم:

```text
PROVIDER_AVAILABLE != PROVIDER_AUTHORIZED
PROVIDER_AUTHORIZED != CONNECTION_EXECUTED
DATA_RECEIVED != TRADE_AUTHORIZED
```

أي Data stale/unknown/unavailable/invalid/conflicting يجب أن تبقى معروضة كحالها، ولا تتحول بصمت إلى Current Truth.

---

# 12. Broker Execution

تنفيذ أمر عند Broker Boundary منفصل.

حتى لو Trading عندها قرار صحيح والبيانات متاحة:

```text
BROKER_ROUTE_EXISTS != ORDER_AUTHORIZED
ORDER_AUTHORIZED != ORDER_SENT
ORDER_SENT != BROKER_ACCEPTED
BROKER_ACCEPTED != FILLED
```

عندما يتم تمكين Broker execution مستقبلًا، يجب أن تعرض الواجهة هذه الحالات بشكل منفصل.

---

# 13. الحماية والمخاطر

FSATS مصمم لحماية رأس المال وليس فقط للبحث عن الفرص.

لذلك قد ترى Hold أو Restriction أو Deny أو Degraded بدل السماح عند وجود غموض.

القاعدة:

```text
UNKNOWN OR AMBIGUOUS AUTHORITY -> DENY / HOLD
```

هذا ليس ضعفًا في النظام، بل جزء من الحماية المقصودة.

---

# 14. التحديثات والإصدارات

أي Material Change في FSATS يجب أن يمر Versioning وRevalidation حسب نطاقه.

هذا يعني للمستخدم:

- PASS قديم لا يغطي تغييرًا دلاليًا جديدًا؛
- Version جديدة قد تحتاج Verification جديدة؛
- Silent Upgrade ليس shortcut مقبولًا؛
- Release وActivation قرارات منفصلة.

```text
COMPATIBLE_UPDATE != SILENT_UPGRADE_AUTHORITY
```

---

# 15. ماذا تفعل إذا رأيت Unknown أو Stale أو Degraded أو Denied؟

- **Unknown:** المعلومة المطلوبة غير مثبتة.
- **Stale:** الدليل قديم أو لم يعد Fresh بما يكفي.
- **Degraded:** التشغيل محصور في Safe Reduced Envelope.
- **Denied:** الطلب لم يمر Gate محكوم.
- **Held:** التقدم متوقف عمدًا لحين Evidence أو Authority.

لا تفسرها كأنها موافقة غير معلنة.

```text
UNKNOWN != YES
STALE != CURRENT
DENIED != RETRY UNTIL ACCEPTED
```

---

# 16. Checklist للمستخدم قبل الاعتماد على أي Action مهم

تأكد من أن الواجهة توضح، حسب الحاجة:

- Broker Account الصحيح؛
- Market/Instrument الصحيح؛
- Data state الحالية؛
- Strategy/Decision state؛
- Guardian state؛
- Resource state؛
- Provider state؛
- Broker route state؛
- Authority state؛
- Execution state؛
- Broker confirmation؛
- Evidence/Audit reference.

إذا عنصر Authority-critical غير معروف، لا تعتبره Approved.

---

# 17. ماذا لا يَعِد FSATS؟

FSATS لا يَعِد بـ:

- ربح مضمون؛
- Prediction مثالي؛
- Provider availability دائم؛
- Broker availability دائم؛
- Zero market risk؛
- أن Simulation = Live performance؛
- أن Technical Success = Authority؛
- أن Request = Completed Action.

الهدف هو Trading operation محكوم، قابل للتفسير، قابل للتدقيق، ومحدود الصلاحيات.

---

# 18. قاموس سريع

**FSATS:** Falcon Self-Aware Trading System.  
**Trading:** Application ذكاء التداول وWorkflow التداول.  
**FSAPMA:** Application إدارة Providers التشغيلية.  
**Trading Guardian:** Application الحماية داخل Domain التداول.  
**FSTSimA:** Application المحاكاة وDigital City validation.  
**APP-RSC:** Application تنسيق موارد FSATS.  
**MSA:** وعي Application كاملة.  
**LSA:** وعي Major Branch.  
**CSA:** وعي Component ذكي مؤهل.  
**FSA:** Foundation Self-Awareness، خارج Applications.  
**Admission:** قبول Foundation لApplication candidate، وليس Activation.  
**Runtime Registration:** تسجيل تقني داخل Foundation hosting، وليس Activation.  
**Activation:** صلاحية منفصلة للتشغيل الفعلي.  
**Fail Closed:** عند الغموض في Evidence أو Authority تكون النتيجة Deny/Hold بدل السماح.

---

# 19. القاعدة النهائية للمستخدم

اسأل دائمًا أربع أسئلة منفصلة:

```text
ماذا يعرف Falcon؟
ماذا يقترح Falcon؟
ماذا يملك Falcon صلاحية أن يفعل؟
ماذا نفذ Falcon فعلًا وتم تأكيده؟
```

إذا كانت الإجابات مختلفة، لا تخلط بينها.

**FSATS مصمم حتى يجعل هذا الفصل واضحًا، محكومًا، قابلًا للتفسير، وقابلًا للتدقيق.**