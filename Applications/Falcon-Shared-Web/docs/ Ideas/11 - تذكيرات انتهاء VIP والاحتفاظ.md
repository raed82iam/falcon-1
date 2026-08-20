# Shared Falcon Web - تذكيرات انتهاء VIP والاحتفاظ

**Status:** `DRAFT / DISCUSSION ONLY`

هذا الملف يسجل اتجاه UX/Product الخاص بتذكير المستخدم بعد انتهاء VIP وخلال فترة الاحتفاظ المؤقت بمحتواه وإعداداته Premium. لا يمنح implementation/runtime/billing authority.

---

## 1. المبدأ

قرار الـOwner الحالي هو أن المستخدم يجب أن يُذكَّر بشكل واضح خلال فترة الاحتفاظ حتى لا ينسى أن لديه بيانات وإعدادات Premium محفوظة مؤقتًا ويمكنه استعادتها إذا عاد إلى VIP.

يشمل ذلك حاليًا، بحسب نوع البيانات المؤهلة للاحتفاظ:

- Presets / Layouts المحفوظة؛
- تفضيلات إشعارات VIP التي أصبحت غير فعالة بعد الرجوع إلى Standard؛
- أي إعداد Premium آخر يُعتمد لاحقًا صراحة ضمن نفس سياسة الاحتفاظ.

الهدف تجاري وخدمي في نفس الوقت:

- نُظهر للمستخدم أننا مهتمون بأن يحافظ على عمله ويرجع إلى VIP إذا أراد؛
- لا نضغط عليه ولا نجبره على الاشتراك؛
- التذكير لا يتحول إلى إزعاج أو Dark Pattern؛
- قرار الاشتراك يبقى للمستخدم بالكامل.

```text
REMIND != FORCE
REMIND != AUTO-SUBSCRIBE
RETENTION WARNING != THREAT
USER CHOICE = PRESERVED
```

---

## 2. فترة الاحتفاظ

الفترة المعتمدة حاليًا للتخطيط تعتمد على نوع علاقة المستخدم السابقة بـVIP:

- **انتهاء VIP Trial المجاني الأول بدون اشتراك مدفوع لاحق:** فترة الاحتفاظ بالحالة Premium المؤهلة تبقى **60 يومًا**.
- **إلغاء/انتهاء اشتراك VIP مدفوع فعلي:** فترة الاحتفاظ بالحالة Premium المؤهلة تكون **120 يومًا**.

خلال فترة الاحتفاظ المناسبة للحالة تكون بيانات/إعدادات Premium المؤهلة للاحتفاظ مقفلة أو غير فعالة حسب طبيعتها، لكنها غير محذوفة. إذا عاد المستخدم إلى VIP قبل نهاية المهلة، يستعيد الوصول إلى الحالة المحفوظة وفق entitlement الفعلي. إذا انتهت المهلة بدون استعادة VIP، تُحذف البيانات/الإعدادات المؤهلة للحذف وفق السياسة النهائية.

```text
FREE VIP TRIAL ENDS
    ↓
60-DAY RETENTION WINDOW
    ↓
VIP RESTORED → RESTORE ELIGIBLE SAVED PREMIUM STATE
OR
DEADLINE REACHED → DELETE ELIGIBLE RETAINED PREMIUM STATE
```

```text
PAID VIP SUBSCRIPTION ENDS / IS CANCELLED
    ↓
120-DAY RETENTION WINDOW
    ↓
VIP RESTORED → RESTORE ELIGIBLE SAVED PREMIUM STATE
OR
DEADLINE REACHED → DELETE ELIGIBLE RETAINED PREMIUM STATE
```

إذا عاد المستخدم إلى VIP بعد أن تم حذف الحالة القديمة عند انتهاء مهلة الاحتفاظ المطبقة على حالته، يبدأ من جديد بالنسبة للإعدادات التي تم حذفها ولا يُفترض وجود استعادة لحالة لم تعد محفوظة.

```text
RETENTION EXPIRED + DATA DELETED
→
LATER VIP RETURN = NEW SETUP FOR DELETED PREMIUM STATE
```

### 2.1 فصل VIP Trial المجاني عن إلغاء اشتراك VIP فعلي

قرار الـOwner الحالي هو أن Falcon لا يخلط بين حالتين مختلفتين تجاريًا وUX:

1. مستخدم جديد حصل على **VIP Trial مجاني لأول شهر** ثم انتهت التجربة ولم يشترك.
2. مستخدم قام لاحقًا بعمل **اشتراك VIP فعلي** ثم ألغى/أنهى هذا الاشتراك.

```text
FREE VIP TRIAL EXPIRY
!=
PAID VIP SUBSCRIPTION CANCELLATION
```

والفصل يشمل الآن مدة الاحتفاظ نفسها:

```text
FREE VIP TRIAL RETENTION = 60 DAYS
PAID VIP CANCELLATION RETENTION = 120 DAYS
```

المستخدم الذي سبق له أن اشترك فعليًا في VIP يبقى معروفًا كتاريخ/حالة تجارية سابقة مؤهلة لأن تعاملها Falcon مستقبلًا كـ **Former Paid VIP Subscriber** أو ما يعادلها في النموذج النهائي، حتى لو ألغى الاشتراك وأصبح Standard.

الغرض من هذا السجل هو الحفاظ على تاريخ العلاقة التجارية وتمكين الـOwner مستقبلًا من اختيار هذا النوع من المستخدمين ضمن جمهور مستهدف لحملة أو عرض، إذا أراد ذلك. وجود السجل وحده لا ينشئ عرضًا ولا خصمًا ولا حملة تلقائية.

```text
PREVIOUS PAID SUBSCRIBER
!=
FIRST-TIME TRIAL USER

PREMIUM STATE DELETED AFTER RETENTION
!=
PAID SUBSCRIPTION HISTORY ERASED

FORMER PAID VIP STATUS
!=
AUTOMATIC OFFER
```

أما مدة حفظ سجل الاشتراك التجاري السابق نفسه، وسياسة الحذف/الخصوصية لذلك السجل، فتبقى مسائل منفصلة عن مدة الاحتفاظ بالحالة Premium ولم تُحسم نهائيًا بعد.

### 2.2 العروض والخصومات تُدار من صفحة المالك

قرار الـOwner الحالي هو أن العروض والخصومات وحملات العودة إلى VIP **لا تُنشأ تلقائيًا بواسطة منطق الاحتفاظ أو جدول التذكيرات**.

الـOwner ينشئ العرض أو الحملة لاحقًا من صفحة/مركز المالك، ويحدد المستخدمين أو الفئة المستهدفة، ثم يطلب من Falcon نشر العرض إلى ذلك الجمهور وفق الصلاحيات والقنوات المعتمدة لاحقًا.

```text
RETENTION SCHEDULE != OFFER ENGINE
FORMER VIP STATUS != AUTO-DISCOUNT
OWNER CREATES OFFER
OWNER SELECTS TARGET AUDIENCE
OWNER REQUESTS PUBLISH
```

وبالتالي لا يرتبط تذكير 90 أو 30 أو 7 أو 1 يوم بخصم افتراضي أو عرض إلزامي. يمكن أن يظهر عرض مع أي مستخدم فقط عندما تكون هناك حملة أنشأها الـOwner واختار لها ذلك المستخدم أو الفئة التي ينتمي إليها.

### 2.3 استهداف الجمهور من صفحة المالك

اعتمد الـOwner أن صفحة/مركز المالك يجب أن تدعم في التخطيط طريقتين للاستهداف عند إنشاء عرض أو حملة:

1. **استهداف فردي:** اختيار مستخدم واحد أو أكثر بشكل صريح.
2. **استهداف حسب فئة:** اختيار مجموعة مستخدمين مؤهلين بحسب تصنيف معتمد، مثل `Former Paid VIP` أو `Standard` أو `VIP`، وأي فئات أخرى تُعتمد لاحقًا.

```text
OWNER TARGETING = INDIVIDUAL USERS OR ELIGIBLE USER SEGMENTS
INDIVIDUAL TARGETING != SEGMENT TARGETING
TARGET SELECTION != AUTO-PUBLISH
OWNER PUBLISH REQUEST REQUIRED
```

اختيار المستخدم أو الفئة لا يعني النشر تلقائيًا. يبقى إنشاء العرض، تحديد الجمهور، ومطالبة Falcon بالنشر أفعالًا منفصلة من جهة الـOwner.

### 2.4 معاينة حجم الجمهور قبل النشر

اعتمد الـOwner أن صفحة/مركز المالك يجب أن تعرض **عدد المستخدمين الذين ينطبق عليهم الاستهداف الحالي قبل تنفيذ النشر**.

إذا استهدف الـOwner فئة كاملة أو مجموعة من المستخدمين، يجب أن يرى حجم الجمهور الناتج عن اختياره قبل أن يطلب النشر، حتى يكون القرار واعيًا وواضحًا ولا يتم إرسال حملة إلى نطاق أكبر أو أصغر من المقصود دون ملاحظة.

```text
TARGET SELECTED
        ↓
AUDIENCE COUNT PREVIEW
        ↓
OWNER REVIEWS AUDIENCE SIZE
        ↓
OWNER MAY REQUEST PUBLISH

AUDIENCE PREVIEW != PUBLISH
TARGET COUNT SHOWN != DELIVERY COMPLETED
```

معاينة العدد هي معلومة قبل النشر وليست تأكيدًا بأن كل مستخدم سيستلم الرسالة عبر كل قناة. أهلية القناة، موافقات المستخدم، وأي استبعادات أو قيود تشغيلية نهائية تبقى خاضعة للتصميم والحدود المعتمدة لاحقًا.

### 2.5 فتح قائمة المستخدمين من معاينة العدد

اعتمد الـOwner أن **عدد الجمهور المعروض قبل النشر يكون قابلًا للضغط**. عند الضغط عليه، يستطيع الـOwner فتح قائمة المستخدمين الذين ينطبق عليهم الاستهداف الحالي ومراجعتهم قبل طلب النشر.

```text
AUDIENCE COUNT PREVIEW
        ↓ CLICK
TARGETED USER LIST PREVIEW
        ↓
OWNER REVIEWS MEMBERS
        ↓
OWNER MAY RETURN OR REQUEST PUBLISH

OPEN TARGET LIST != PUBLISH
VIEW USER LIST != CHANGE TARGETING
```

فتح القائمة وحده لا يرسل العرض ولا يغيّر الجمهور.

### 2.6 استثناء مستخدمين يدويًا من الجمهور

اعتمد الـOwner أن قائمة المستخدمين الناتجة عن معاينة الجمهور ليست للعرض فقط. يستطيع الـOwner من نفس القائمة **تحديد مستخدم واحد أو أكثر واستبعاده يدويًا من الحملة الحالية قبل النشر**.

إذا كان الاستهداف مبنيًا على فئة كاملة، يبقى أصل الاستهداف هو الفئة المختارة، ثم تُطبّق فوقها الاستثناءات الفردية التي اختارها الـOwner لهذه الحملة.

```text
BASE SEGMENT TARGET
        ↓
AUDIENCE PREVIEW
        ↓
OWNER SELECTS ONE OR MORE USERS
        ↓
EXCLUDE FROM THIS CAMPAIGN
        ↓
RECALCULATE TARGET AUDIENCE
        ↓
UPDATED COUNT + UPDATED USER LIST

SEGMENT TARGET - OWNER EXCLUSIONS = FINAL PRE-PUBLISH AUDIENCE
USER EXCLUDED FROM CAMPAIGN != USER REMOVED FROM SEGMENT
USER EXCLUDED FROM CAMPAIGN != ACCOUNT CHANGED
EXCLUDE != PUBLISH
```

الاستبعاد هنا خاص بجمهور الحملة الحالية، ولا يعني حذف المستخدم من حسابه، ولا تغيير تصنيفه الأساسي مثل `VIP` أو `Standard` أو `Former Paid VIP`، ولا إنشاء قاعدة استبعاد دائمة ما لم يعتمد الـOwner ذلك لاحقًا بشكل منفصل.

### 2.7 إضافة مستخدمين يدويًا خارج الفئة المستهدفة

اعتمد الـOwner أن بإمكانه أيضًا **إضافة مستخدم واحد أو أكثر يدويًا إلى الحملة الحالية حتى لو لم يكونوا أعضاء في الفئة الأساسية المستهدفة**.

بهذا يصبح جمهور الحملة قابلاً للتخصيص على مستوى الأفراد فوق الاستهداف الأساسي: يبدأ من الفئة أو المستخدمين المختارين، ثم يطبق الـOwner الإضافات والاستثناءات اليدوية قبل النشر.

```text
BASE TARGET AUDIENCE
+ OWNER MANUAL INCLUSIONS
- OWNER MANUAL EXCLUSIONS
=
FINAL PRE-PUBLISH AUDIENCE
```

بعد كل إضافة أو استبعاد يدوي، يجب أن تتحدث **معاينة العدد وقائمة المستخدمين** حتى يرى الـOwner الجمهور النهائي الفعلي قبل طلب النشر.

```text
MANUAL INCLUDE != USER ADDED TO SEGMENT
MANUAL INCLUDE != ACCOUNT CLASSIFICATION CHANGED
MANUAL INCLUDE != PUBLISH
AUDIENCE EDIT != DELIVERY
```

إضافة المستخدم يدويًا تخص الحملة الحالية فقط، ولا تغيّر تصنيفه الأساسي ولا تجعله عضوًا دائمًا في الفئة التي تم استهدافها. كما أن وجوده في الجمهور لا يتجاوز أهلية القناة أو موافقات الاتصال أو أي قيود ملزمة تُعتمد لاحقًا.

### 2.8 عدم حفظ الجمهور المعدّل كمجموعة مستقلة

اعتمد الـOwner أنه **لا توجد حاجة حاليًا لحفظ الجمهور الناتج عن الإضافات والاستبعادات اليدوية كـ Audience/Group مستقل لإعادة استخدامه في حملات مستقبلية**.

تظل هذه التعديلات مرتبطة بالحملة الحالية فقط. إذا احتاج الـOwner في حملة مستقبلية إلى جمهور مشابه، يختار الفئة أو المستخدمين المناسبين من جديد ويطبق التعديلات المطلوبة لتلك الحملة.

```text
CAMPAIGN AUDIENCE EDITS = CURRENT CAMPAIGN ONLY
MANUAL INCLUDE / EXCLUDE != SAVED AUDIENCE
NO AUTOMATIC REUSABLE GROUP CREATION
```

هذا لا يمنع وجود الفئات الأساسية المعتمدة مثل `VIP` أو `Standard` أو `Former Paid VIP`. القرار يخص فقط عدم إنشاء Group جديد دائم من النسخة المعدلة يدويًا لجمهور حملة معينة.

### 2.9 تأكيد نهائي قبل النشر

اعتمد الـOwner أن الضغط على إجراء النشر لا يرسل الحملة مباشرة. قبل أي نشر فعلي، تظهر **خطوة تأكيد نهائية** تعرض على الأقل:

- **اسم العرض/الحملة** الذي سيتم نشره؛
- **عدد المستخدمين النهائي** بعد تطبيق الفئة الأساسية وكل الإضافات والاستبعادات اليدوية الحالية؛
- **معاينة محتوى العرض/الحملة نفسه** كما سيظهر للمستخدم، حتى يستطيع الـOwner مراجعة النص والمحتوى للمرة الأخيرة قبل النشر.

بعد مراجعة هذه البيانات والمحتوى، يستطيع الـOwner إما الرجوع للتعديل أو تنفيذ تأكيد النشر الصريح.

```text
FINAL AUDIENCE RESOLVED
        ↓
OWNER REQUESTS PUBLISH
        ↓
FINAL CONFIRMATION / REVIEW
  - OFFER / CAMPAIGN NAME
  - FINAL USER COUNT
  - CAMPAIGN CONTENT PREVIEW
        ↓
OWNER REVIEWS CONTENT + AUDIENCE
        ↓
OWNER CONFIRMS PUBLISH
        ↓
PUBLISH REQUEST MAY PROCEED

PUBLISH CLICK != FINAL CONFIRMATION
FINAL REVIEW SHOWN != PUBLISHED
CONTENT PREVIEW != DELIVERY
OWNER CONFIRMATION REQUIRED
```

المعاينة النهائية يجب أن تعكس المحتوى الحالي للحملة الذي سيُطلب نشره، وليست مجرد نسخة قديمة أو ملخص غير مطابق. إذا رجع الـOwner وعدّل المحتوى أو الجمهور، يجب أن تعكس خطوة التأكيد التالية النسخة المحدثة قبل السماح بتأكيد النشر.

العدد المعروض في خطوة التأكيد هو العدد النهائي المعروف للـUX قبل طلب النشر، لكنه لا يُعامل كإثبات تسليم ولا كضمان أن كل قناة ستقبل الإرسال. أي أهلية قناة، موافقة مستخدم، أو قيد ملزم يبقى منفصلًا ويجب أن يُحترم في التنفيذ النهائي.

هذا القرار يحدد UX/Product intent فقط. آلية إنشاء الحملات، شروط الاستهداف، الموافقات، النشر، القياس، الصلاحيات، القنوات والتنفيذ الفعلي ستُحسم لاحقًا ضمن تصميم صفحة المالك والحدود التشغيلية ذات الصلة.

---

## 3. جدول التذكيرات المعتمد

لـ**VIP Trial المجاني** ذي فترة الاحتفاظ 60 يومًا، يبقى جدول التذكيرات المعتمد حاليًا:

```text
30 DAYS REMAINING
        ↓
7 DAYS REMAINING
        ↓
1 DAY REMAINING
        ↓
RETENTION DEADLINE
```

أما للمستخدم الذي سبق له **اشتراك VIP مدفوع فعلي** ثم ألغى/أنهى الاشتراك، ومع فترة الاحتفاظ الممتدة إلى 120 يومًا، اعتمد الـOwner جدول تذكير مستقل:

```text
PAID VIP RETENTION = 120 DAYS

90 DAYS REMAINING
        ↓
30 DAYS REMAINING
        ↓
7 DAYS REMAINING
        ↓
1 DAY REMAINING
        ↓
RETENTION DEADLINE
```

وبذلك يبقى الفصل واضحًا:

```text
FREE TRIAL REMINDER SCHEDULE
!=
FORMER PAID VIP REMINDER SCHEDULE
```

كل تذكير يجب أن يوضح بشكل مفهوم:

- أن بيانات/إعدادات Premium المؤهلة ما زالت محفوظة مؤقتًا؛
- عدد الأيام المتبقية قبل الحذف؛
- أن العودة إلى VIP قبل الموعد تعيد الوصول إلى الحالة المحفوظة وفق entitlement الفعلي؛
- مسارًا اختياريًا إلى الاشتراك أو صفحة المقارنة بين VIP وStandard.

هذه التذكيرات ليست ضغطًا على المستخدم ولا تمنح اشتراكًا تلقائيًا ولا تغيّر موعد الحذف بمجرد تجاهل الرسالة أو إغلاقها.

```text
REMINDER = INFORMATION + OPTIONAL RETURN PATH
REMINDER DISMISSED != DATA DELETED
REMINDER SHOWN != SUBSCRIPTION CONSENT
REMINDER != OWNER OFFER
```

وتبقى قنوات هذه التذكيرات، بما فيها البريد الإلكتروني، خاضعة لتفضيلات وموافقات المستخدم الحالية كما هو محدد أدناه؛ اعتماد نقطة تذكير لا يعني تلقائيًا السماح باستخدام كل قناة اتصال.

---

## 4. قنوات التذكير واختيار المستخدم

قرار الـOwner الحالي هو أن البريد الإلكتروني **ليس قناة إلزامية** لتذكيرات VIP أو الاحتفاظ.

إذا كان المستخدم قد فعّل إشعارات البريد الإلكتروني لهذه الفئة من التنبيهات، يمكن إرسال تذكيرات الـVIP/retention إليه عبر البريد إلى جانب التنبيه داخل تجربة Falcon بحسب التصميم النهائي.

إذا لم يكن المستخدم قد فعّل إشعارات البريد الإلكتروني، **لا تُرسل له هذه التذكيرات عبر البريد**.

```text
EMAIL REMINDERS = USER OPT-IN
EMAIL NOT ENABLED = NO VIP / RETENTION EMAIL
IN-APP REMINDER != EMAIL CONSENT
ACCOUNT EMAIL EXISTS != EMAIL NOTIFICATION CONSENT
```

يجب أن يستطيع المستخدم تغيير تفضيل إشعارات البريد لاحقًا وفق إعدادات الإشعارات النهائية، ولا يجوز اعتبار إنشاء الحساب أو وجود عنوان بريد في الحساب موافقة تلقائية على رسائل VIP التذكيرية.

هذا القرار يخص رسائل التذكير/الترقية الاختيارية من منظور UX. الرسائل التي قد تكون مطلوبة قانونيًا أو أمنيًا أو تشغيليًا في المستقبل تبقى فئة منفصلة وتُحسم وفق متطلباتها الخاصة.

---

## 5. UX Guardrails

```text
HELPFUL REMINDER != PRESSURE
SUBSCRIBE CTA != MANDATORY ACTION
DISMISS != LOSS OF DATA
NO RESPONSE != CONSENT
EMAIL ADDRESS != MARKETING CONSENT
```

يجب أن يستطيع المستخدم تجاهل/إغلاق التذكير العادي بدون أن يُعامل ذلك كموافقة أو رفض نهائي، وبدون أن يؤدي مجرد إغلاق الرسالة إلى حذف البيانات مبكرًا.

هذا الملف تخطيط UX/Product فقط، ولا يحدد آلية الإشعارات أو البريد أو Push أو exact retention implementation.