# دليل Project Owner — Falcon Shared Web

**الفئة:** Project Owner  
**اللغة:** العربية  
**النطاق:** واجهات الـOwner والتحكم والعرض والحوكمة داخل Shared Falcon Web

## 1. مبدأ أساسي

الـOwner في الويب لديه واجهات خاصة للعرض وإرسال الطلبات، لكن Shared Web نفسه لا يتحول إلى مصدر Authority. الواجهة لا تعتبر أي طلب منفذًا لمجرد أنه أُرسل.

القاعدة الحاكمة:

`REQUEST_SENT != ACTION_ACCEPTED != ACTION_COMPLETED`

## 2. صفحات الـOwner

الـroute registry الحالي يحتوي صفحات Owner التالية:

- Owner Home `#/owner-home`
- Owner Command Center `#/owner`
- Applications `#/owner-apps`
- Incidents `#/owner-incidents`
- Approvals `#/owner-approvals`
- AI Emergency `#/owner-ai-emergency`
- Provider Actions `#/owner-provider-actions`
- Users `#/owner-users`
- Audit `#/owner-audit`
- Settings `#/owner-settings`
- Simulator `#/owner-simulator`

وجود route لا يمنح صلاحية. يلزم session authoritative وOwner surface grant صالح.

## 3. Owner Home

Owner Home هو مدخل الـOwner. منه يمكن الانتقال إلى الأسطح الإدارية المتاحة. لا يتم تحويل role وحده إلى صلاحية، ولا يجوز دخول customer workspace لمجرد أن المستخدم Project Owner بدون entitlement منفصل.

## 4. Command Center

Command Center هو سطح عرض وطلب وليس محرك تنفيذ مباشر. يمكن أن يعرض:

- System health
- Applications
- Users
- Incidents
- Approvals
- System overview
- Owner interactions
- Audit information
- Settings
- Simulator access

إذا لم تصل projection authoritative، تبقى الحالة unavailable. لا يتم اختراع users أو timestamps أو health states.

## 5. Applications

يعرض حالة التطبيقات حسب الحقيقة الموردة من المصادر الحاكمة. الويب لا يقرر تشغيل Application ولا يقبل admission أو activation من نفسه.

## 6. Incidents والدعم

يمكن للـOwner متابعة incident information والتفاعل معها ضمن capabilities المربوطة.

مبادئ مهمة:

- Screenshot observed لا يساوي broker-confirmed truth.
- Support takeover لا يعني portfolio control.
- Support message لا يعني business authorization.
- takeover يحتاج capability authoritative وصريح.
- Falcon يبقى مراقبًا صامتًا أثناء takeover المصرح به بدل نقل authority.

## 7. Approvals والحوكمة

واجهة approvals تحفظ الفصل بين:

- proposal
- eligibility
- Owner decision
- accepted request
- completed outcome

أي proposal متغير ماديًا يجب إعادة تقييمه. لا يجوز اعتبار self-classification أو self-approval من producer موافقة Owner.

## 8. Owner Update Governance

الويب يدعم request families محكومة للـOwner مثل:

- policy management
- standing preapproval evaluation
- rollback order

كلها تمر عبر binding/transport حاكم. الويب لا يخترع Foundation decision ولا execution outcome.

`REGISTERED != ACTIVATED`

## 9. AI Emergency

Owner AI Emergency يسمح بإرسال intent محكوم عند توفر session ومعلومات target/blast radius authoritative.

قواعد حاسمة:

- target المفقود أو المبهم = fail closed.
- targeted Kill لا يستخدم `ALL_AI`.
- Global AI Kill يجب أن يحافظ على Falcon Safe Core.
- Global AI Kill ليس Falcon shutdown.
- accepted لا تعني completed.
- release/revival ليست سلطة Web محلية.

## 10. Provider Actions

واجهة provider actions لا تعرض plaintext secrets. أي credential يجب أن يكون opaque credential reference وليس secret bytes داخل Web state.

تهيئة route لا تعني تشغيل الاتصال:

`ROUTE_POLICY_BOUND != CONNECTION_EXECUTED`

## 11. Users وAudit

- لا يتم اختراع users.
- لا يتم اختراع audit timestamps.
- أي projection غير متاحة تبقى unavailable.
- النصوص غير الموثوقة يتم output-encode قبل العرض.

## 12. Simulator

Simulator هو Owner-only presentation surface. لا يجوز أن تتحول simulator truth إلى broker truth أو live truth.

## 13. Owner مقابل Customer

Owner surface وCustomer surface منفصلان.

- Owner role وحده لا يفتح customer FSATS workspace.
- Customer entitlement منفصل.
- Support role لا يفتح Owner surface.
- surface grants يجب أن تكون authoritative ومناسبة للroute.

## 14. الربط مع Foundation

Shared Web أصبح full plug-ready من جهة التحضير:

- Admission candidate جاهز.
- Runtime registration template جاهز.
- Full plug-ready preflight verified by composition.
- Foundation change غير مطلوب.

لكن:

- Actual Admission غير منفذ.
- Runtime Registration غير منفذ.
- Activation غير منفذ.
- Deployment غير منفذ.
- Provider connectivity غير منفذ.

القيم التشغيلية الحالية يتم ربطها فقط وقت العملية الفعلية من مصادر authoritative.

## 15. ما الذي لا يفعله Owner Manual؟

هذا الدليل لا يمنح صلاحية جديدة ولا يغير Falcon Constitution أو Foundation أو Application contracts. هو يشرح السلوك الحالي للواجهة فقط.
