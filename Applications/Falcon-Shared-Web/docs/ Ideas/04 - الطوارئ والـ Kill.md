# Shared Falcon Web - الطوارئ والـ Kill

**Status:** `DRAFT / DISCUSSION ONLY`

هذا الملف يشرح تجربة الطوارئ من منظور الـOwner، وليس تنفيذ Kill Enforcer نفسه.

## الهدف

إذا خرج AI عن السيطرة، يجب أن يكون عند الـOwner طريق واضح لفهم الحالة وطلب containment/kill بدون الاعتماد على تعاون الـAI المستهدف.

## المسار الطبيعي

`Owner -> Shared Web -> Foundation-owned emergency control/enforcer -> target AI`

## المسار المستقل المرشح

`Owner -> Falcon Emergency Control mobile -> independent emergency path -> Foundation-owned enforcer -> target AI`

الهدف أن تعطل Shared Web لا يجعل الـOwner أعمى أو غير قادر على طلب الإيقاف، عندما تكون البنية المستقلة الحقيقية موجودة.

## Shared Web Emergency UX

الواجهة قد تحتاج مستقبلًا إلى عرض:
- Falcon Core reachability/health.
- Emergency control readiness.
- AI state.
- Web state.
- Kill/containment state.
- آخر emergency command/result.
- الحالة التي تحتاج Owner decision.

كل قيمة يجب أن تأتي من authoritative source. `UNKNOWN` يبقى `UNKNOWN`.

## Falcon Emergency Control للموبايل

الموبايل ليس Dashboard كامل.

وظيفته المرشحة:
- يعرض أقل قدر لازم من الحالة لفهم الطوارئ.
- يرسل Owner emergency request.
- يعرض قبول/رفض/تأكيد containment عندما تكون هذه الحقيقة متاحة.
- لا يملك authority بنفسه.
- لا يعيد AI للحياة بنفسه.

## ماذا يعني Kill مبدئيًا

حسب Foundation planning الحالي، التنفيذ التفصيلي Falcon-wide ما زال غير مكتمل، لكن المقصود المرشح يتضمن حسب السلطة النهائية:
- وقف التنفيذ المتأثر.
- سحب أو تعليق authority/trust.
- العزل من trusted operation.
- منع self-development للهدف المتأثر.
- قطع egress المتأثر عند الحاجة.
- حفظ forensic/audit evidence.
- عدم السماح للـAI أن يعيد نفسه أو authority تلقائيًا.

## ما يبقى شغالًا أثناء containment

Shared Web لا يقرر هذا من عنده.

FSATS planning الحالي يقول إن business truth المهمة لا تُمسح بسبب AI containment، ومنها حسب الحاجة:
- capital / exposure truth.
- positions.
- orders / execution / reconciliation.
- Risk restrictions.
- Guardian directives.
- provider/data protection state.
- attributable evidence.

أي deterministic/non-AI capability يستمر فقط إذا كان موثوقًا، غير متأثر، ومصرحًا به ومطلوبًا لحماية safe state.

## APP-RSC وأمان الموارد

في التخطيط الحالي APP-RSC Candidate يضيف احتياجًا محتملاً لعرض:
- availability / trust.
- resource coordination state.
- coordination epoch / fencing.
- Foundation-envelope freshness.
- protected minimum status.
- resource recovery readiness.

لكن exact schemas/routes/states ما زالت unresolved ولا يجوز للـWeb اختراعها.

## Recovery بعد Kill

المبدأ:

`KILL / ISOLATE -> PRESERVE EVIDENCE -> ROOT CAUSE -> REMEDIATION -> VALIDATION -> OWNER-AUTHORIZED CONTROLLED REVIVAL`

`RESTARTED != RECOVERED`

`REPAIRED != TRUSTED`

الـrevival لا يكون تلقائيًا ولا بقرار الـAI المستهدف.

## الحالة الحالية

`FCR-0076 = ACCEPTED_FOR_PLANNING / Waiting On: NONE`

الـgeneric Falcon-wide unified Kill runtime/control plane ما زال `MISSING / UNASSIGNED` ولا توجد Implementation Authority.

`FCR-0077 = ACCEPTED_FOR_PLANNING / Waiting On: NONE`

الـApplication أعطى planning rules لكن exact target/component matrix وemergency projection contracts ما زالت unresolved.

هذا الملف لا يمنح تنفيذ أو تشغيل أو deployment authority.