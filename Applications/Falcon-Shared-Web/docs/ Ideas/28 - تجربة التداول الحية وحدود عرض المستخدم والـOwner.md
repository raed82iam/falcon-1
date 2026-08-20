# Shared Falcon Web - تجربة التداول الحية وحدود عرض المستخدم والـOwner

**Status:** `DRAFT / DISCUSSION ONLY`

هذا الملف يسجل قرارات الـOwner الخاصة بحدود العرض داخل FSATS من منظور Shared Web فقط. لا يمنح implementation/runtime/trading authority ولا يغير ملكية البيانات أو التنفيذ داخل FSATS.

---

## 1. Web لا يملك أو يعرض Broker-account internals للمستخدم العادي

قرار الـOwner:

بالنسبة لواجهة المستخدم العادي، Shared Web لا يحتاج أن يعرف أو يبرز من أي Broker Account جاءت حالة السهم أو المركز ما دام الـApplication يعيد البيانات والحالة التي يجب عرضها.

```text
WEB NEEDS DISPLAYABLE AUTHORITATIVE RESULT
!=
WEB NEEDS BROKER-ACCOUNT INTERNAL DETAIL
```

إذا احتاجت FSATS داخليًا إلى BrokerId / BrokerAccountId للتنفيذ أو reconciliation أو incident handling فهذه تبقى Application-owned semantics، وليست مبررًا لإظهارها للمستخدم في تجربة التداول العادية.

الاستثناء الوحيد هو عندما يقرر Contract authoritative أن هوية حساب/وسيط محدد ضرورية فعلًا لتجربة مستخدم معينة، مثل incident guidance أو credential settings. عندها يعرض Web فقط ما يلزم لذلك السيناريو.

---

## 2. المستخدم العادي يرى Live Trading Experience فقط

قرار الـOwner:

المستخدم العادي يتعامل مع واجهة FSATS على أنها تجربة التداول الحية الخاصة به. Shared Web لا يعرض له simulator/shadow/paper internals ولا يشرح له أي backend simulation path.

```text
REGULAR USER UX = LIVE TRADING EXPERIENCE
SIMULATOR / SHADOW / PAPER INTERNALS = NOT REGULAR-USER PRESENTATION
```

الـWeb لا يقرر كيف تحقق الـApplication البيانات أو من أي API/provider/broker جاءت. وظيفته عرض النتيجة الحالية التي يستلمها من الجهة authoritative.

```text
WEB PRESENTS
APPLICATION / FSAPMA / BROKER PATHS FULFILL
WEB != PROVIDER/BROKER ROUTING OWNER
```

---

## 3. Simulator visibility = Owner only

قرار الـOwner:

Simulator-related views، بما فيها أي Shadow/Paper/Simulation operational details، تُعرض فقط داخل Owner-facing surfaces عندما تكون العقود والصلاحيات authoritative تسمح بذلك.

```text
SIMULATOR VIEW -> OWNER ONLY
REGULAR USER -> NO SIMULATOR MODE PRESENTATION
```

هذا لا يمنح الـOwner execution authority ولا يغير أي Foundation/Application authority boundary. هو قرار presentation scope فقط.

---

## 4. Asset / Position presentation boundary

Shared Web يهتم بما يحتاج المستخدم أن يراه عن الأصل أو مركزه:

- live/current asset data التي يعيدها المصدر authoritative؛
- position state / quantity / cost / P&L / relevant status إذا وفرها Application contract؛
- current orders/activity/result states إذا وفرها Application contract؛
- Falcon summary / analysis presentation حسب العقود الحالية.

ولا يجعل broker-account identity عنصرًا افتراضيًا في التصميم.

---

## 5. General ownership boundary

```text
APPLICATION = TRADING / POSITION / ORDER / EXECUTION TRUTH OWNER
FSAPMA = OPERATIONAL MARKET-DATA OWNER
SHARED WEB = PRESENTATION / INTERACTION OWNER
OWNER-ONLY SIMULATOR PRESENTATION != SIMULATOR AUTHORITY
```

هذه المرحلة Planning/Discussion فقط.
