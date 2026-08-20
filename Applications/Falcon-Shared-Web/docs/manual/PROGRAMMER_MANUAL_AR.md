# دليل المبرمج — Falcon Shared Web

**الفئة:** Programmer / Web Engineer / Maintainer  
**اللغة:** العربية  
**النطاق:** شرح بنية Shared Falcon Web، خصائصه، حدوده، طريقة تطويره واختباره وربطه

## 1. هوية التطبيق

الاسم: `falcon-shared-web`  
الإصدار الحالي في `package.json`: `0.1.0`  
نوع الوحدات: ES Modules (`"type": "module"`)  
المسار المملوك لهذا الـworkstream:

`applications/shared/web/**`

Shared Web هو Application مستقل داخل Falcon. وظيفته الأساسية هي public/customer/owner/support presentation + governed request submission. لا يملك Foundation ولا FSATS operational truth ولا deployment ولا connectivity ولا trading/business authority.

## 2. الحدود المعمارية

القواعد الأساسية التي يجب ألا يكسرها أي تعديل:

- Presentation ليست Authority.
- Web لا يستورد Foundation internals أو ordinary Application internals مباشرة.
- Web لا يخلق session أو entitlement من role فقط.
- Web market data لا تتحول إلى FSATS operational input.
- Web provider credential لا تساوي FSAPMA credential ولا customer broker credential.
- Credential reference ID لا يساوي secret bytes.
- Registered لا يساوي Activated.
- Route policy bound لا يساوي connection executed.
- Request sent لا يساوي action accepted ولا action completed.

## 3. أهم مجلدات المصدر

### `src/core/`

منطق boundaries والسياسات والـports والـpreflight، مثل:

- runtime port
- provider binding/profile/readiness
- market-data plan
- Web provider runtime policy
- Web incident runtime policy
- Web runtime preflight
- Web awareness model
- Owner request router
- Foundation plug-ready preflight

### `src/core/ports/`

واجهات Web المستقرة تجاه مصادر الحقيقة الخارجية:

- FSATS runtime port
- Falcon system runtime port
- Web market-data port
- Owner AI emergency port
- Owner update governance port
- incident support transport port

القاعدة: port يحدد shape وحدود الاستخدام، ولا يعني وجود transport فعلي.

### `src/adapters/`

Adapters تحول العقود الحاكمة إلى projections مناسبة للويب بدون نقل authority. تشمل Foundation FIL/identity/recovery/governance وFSATS portfolio/analysis/incident/Owner entitlement adapters.

### `src/composition/`

طبقة التركيب:

- `runtime-bootstrap.js` لبداية Preview/Authoritative بشكل fail closed
- `app-context.js`
- `fsats-authoritative-data.js`
- `shell.js`
- `incident-ui-runtime.js`
- `app-view-registry.js`
- `app-ui-bindings.js`
- `owner-surfaces.js`

هذه الطبقة تجمع dependencies، لكنها لا تخلق truth مفقودة.

### `src/features/`

واجهات الميزات المستقلة، منها:

- Falcon public
- FSATS public
- My Applications
- FSATS workspace
- Portfolio
- Activity
- Markets
- Advisory Markets
- AI
- Notifications
- Settings
- Catalog
- Owner Home
- Owner Command Center
- Owner Approvals
- Owner Provider Actions
- Owner AI Emergency
- Customer Incident

### `src/incidents/`

منطق incident timeline، content safety، persistence، controller، accessibility، screenshot handling.

### `src/voice/`

سياسة الصوت، browser microphone، local voice runtime، live voice session، incident voice controller.

### `src/security/`

`safe-html.js` وحدود output encoding المركزية.

### `src/design-system/`

presentation helpers وUI primitives التي تحفظ semantics وaccessibility.

### `tests/`

Node test suite الحالية. كل ملفات `*.test.mjs` تُشغل عبر `node --test`.

### `tools/`

أدوات verification مثل browser verification server.

### `governance/`

مواد admission وplug-ready machine-readable:

- `SHARED_WEB_APPLICATION_ADMISSION_MANIFEST_V1.json`
- `WEB_FOUNDATION_PLUG_READY_PREPARATION_V1.json`

## 4. نظام الـRoutes

الـroute registry موجود في:

`src/platform/navigation/routes.js`

### Public

- `home`
- `apps`
- `login`
- `register`
- `fsats`

### User

- `my-apps`
- `trader`
- `markets`
- `advisory-markets`
- `portfolio`
- `activity`
- `ai`
- `notifications`
- `settings`

### Owner

- `owner-home`
- `owner`
- `owner-apps`
- `owner-incidents`
- `owner-approvals`
- `owner-ai-emergency`
- `owner-provider-actions`
- `owner-users`
- `owner-audit`
- `owner-settings`
- `owner-simulator`

Unknown route يتم تطبيعه إلى public home بدل تنفيذ مسار غير معروف.

## 5. Authentication وAuthorization

`src/auth.js` وFoundation identity/session adapters يطبقان fail-closed behavior.

قواعد مهمة:

- عدم وجود authoritative identity = لا يتم اختراع مستخدم.
- role وحده لا يخلق route authority.
- Owner route يحتاج Owner surface grant.
- Customer route يحتاج Customer access/entitlement المناسب.
- Project Owner لا يدخل customer FSATS تلقائيًا.
- Support/unknown roles لا تدخل Owner أو customer surfaces من inference.
- business-authority-bearing Web session تعتبر invalid.

## 6. Preview مقابل Authoritative

`runtime-bootstrap.js` يحمي هذا الفصل:

- Preview يحتاج preview data واضحة.
- Authoritative mode لا يقبل preview data.
- partial authoritative binding لا يرجع إلى Preview بصمت.
- arbitrary object غير موسوم لا يمكنه التنكر كـauthoritative contract data.
- raw secret-shaped configuration يتم رفضها.
- opaque credential references مسموحة عندما تكون مطلوبة.

## 7. Standard وVIP

`src/features/my-applications/subscription-presentation.js` يدعم tier IDs:

- `STANDARD`
- `VIP`

لكن entitlement لا يُستنتج من الاسم. tier تعتبر متاحة فقط عندما تكون authoritative + entitled + current.

لا تخترع في Web:

- pricing
- trial
- upgrade
- VIP benefits

## 8. Market Data وProvider Routes

Shared Web لديه provider presentation routes منفصلة عن FSAPMA operational provider routes.

Credential references مطلوبة فقط لـ:

- FCR-0176 Alpaca IEX
- FCR-0177 Finnhub
- FCR-0196 Alpaca assets
- FCR-0197 Alpaca bars

Public no-credential routes:

- FCR-0173 Binance trade stream
- FCR-0174 Coinbase public feed
- FCR-0175 Bybit public spot
- FCR-0198 Binance exchangeInfo
- FCR-0199 Binance klines
- FCR-0200 Binance miniTicker

Secret bytes ممنوعة في ordinary Web state.

## 9. Portfolio وActivity truth rules

- nullable أو unavailable values لا تتحول إلى zero.
- broker outcome غير المعروف يبقى unknown.
- pagination metadata يفشل مغلقًا إذا كان contradictory.
- activity lifecycle يحافظ على PARTIALLY_FILLED مقابل FILLED.
- simulator/shadow truth لا تتحول إلى broker truth.

## 10. AI presentation

الويب يعرض نتيجة Application فقط:

- `CURRENT + COMPLETE` يسمح بالتفاصيل الكاملة.
- stale = details محدودة.
- partial = لا full detail.
- needs clarification = لا يتم الادعاء أن identity resolved.
- disagreement لا يتم إخفاؤه بتحويله إلى COMPLETE.

## 11. Incident runtime

Production incident readiness تحتاج bindings authoritative لـ:

- principal/tenant/session
- tenant-scoped persistence
- governed screenshot scanner
- governed Support transport
- local Whisper.cpp/Piper runtime

المفقود لا يستبدل بـpreview fallback في authoritative runtime.

مبادئ:

- credentials في chat = reject.
- screenshot secrets = reject.
- screenshot بدون governed scan evidence = fail closed.
- Support takeover يحتاج capability صريح.
- support transport = transport only وليس authority.

## 12. Voice

- browser microphone يبدأ بعد طلب صريح.
- ordinary voice message لا يتوقف تلقائيًا بسبب silence.
- live voice يستخدم patience rule قبل Falcon reply.
- local voice runtime injected explicitly.
- لا يوجد remote fallback إذا كانت السياسة local-only.

## 13. Owner Governance

Owner governance ports تدعم request families محكومة، منها policy management، standing preapproval evaluation، rollback order.

لا تسمح بأن ينتج Web:

- self approval
- hidden auto accept
- rollback execution claim
- restored authority من status فقط

## 14. AI Emergency

قواعد الكود:

- target ambiguity = fail closed.
- `ALL_AI` للGlobal AI Kill فقط.
- Global AI Kill يحافظ على Safe Core ولا يعني Falcon shutdown.
- accepted outcome لا يساوي completed outcome.
- release/revival ليست control محلي داخل Web.

## 15. XSS وContent Safety

النص غير الموثوق يجب أن يمر عبر output encoding helpers. الاختبارات تغطي payloads HTML/SVG/iframe وغيرها على incident، portfolio، activity، AI وOwner projections.

لا تضف `innerHTML` من input غير encoded. إذا احتجت markup ديناميكي استخدم primitives/helpers الموجودة أو encode قبل التركيب.

## 16. Accessibility وLocalization

يجب الحفاظ على:

- العربية والإنجليزية
- RTL للعربية
- keyboard-native navigation
- visible focus
- skip link
- accessible labels
- semantic headings/regions
- reduced motion
- forced colors support
- mobile viewport containment

## 17. الاختبارات

من داخل:

`applications/shared/web`

شغّل:

```powershell
npm.cmd test
npm.cmd run check
npm.cmd run verify:browser
```

`npm test` يشغل كل `tests/*.test.mjs`.  
`npm run check` يشغل `node --check` على ملفات المصدر الحرجة.  
`verify:browser` يشغل browser verification server للمراجعة اليدوية/المتصفح.

آخر plug-ready executable verification المثبت قبل إضافة هذه manuals كان:

```text
HEAD = 38c5db80adc52e6555ebe8aee821d83659c513d3
TESTS = 479
PASS = 479
FAIL = 0
npm run check = PASS
WORKTREE = CLEAN
```

إضافة manuals هي documentation-only، لكنها تغير Git HEAD ولا تغير executable source.

## 18. Foundation Plug-Ready

Shared Web materializes exactly:

```text
APPLICATION = FALCON_SHARED_WEB
ADMISSION_CANDIDATES = 1
RUNTIME_REGISTRATION_TEMPLATES = 1
REQUEST_PAIRS = 1
```

Baseline:

- CON-023 1.1
- CON-001 1.0 dependency
- FDN-006 1.0
- FDN-007 1.0

Full plug-ready contract preflight = verified.  
Full plug-ready preflight = verified by composition.  
Foundation change required = false.

القيم التالية bind-at-operation وليست preparation gaps:

- exact Web artifact identity
- canonical admission evidence
- lifecycle attach eligibility/decision identity
- resource grants
- observed-at
- provider service principal/role
- opaque credential references
- principal/tenant/session
- production persistence
- screenshot scanner
- Support transport
- local voice runtime

## 19. Actual Link boundary

حتى لو كان plug-ready كاملًا:

- Actual Admission ليس منفذًا.
- Canonical Runtime Registration ليس منفذًا.
- Activation ليس منفذًا.
- Deployment ليس منفذًا.
- Connectivity ليست منفذة.
- Business/Trading authority غير ممنوحة.

لا تغير هذه الحالات من Web بدون العملية الحاكمة المصرح بها.

## 20. كيف تضيف Feature جديدة بشكل صحيح

1. حدد surface ownership: Public/User/Owner.
2. حدد مصدر الحقيقة والـcontract.
3. إذا data خارجية، عرف port أولًا.
4. اكتب adapter يحافظ على authority separation.
5. feature تعرض projection فقط.
6. composition يحقن dependencies ولا يخترعها.
7. أضف fail-closed tests للحالة المفقودة/المعطوبة/المزيفة.
8. أضف security/XSS/accessibility tests حسب الحاجة.
9. شغّل `npm test` و`npm run check`.
10. إذا UI تغيرت، نفذ browser verification.
11. أي cross-workstream dependency تُدار عبر FCR ولا تُحل بتعديل Foundation/Application من Web.

## 21. وثائق العمل

ابدأ من:

- `docs/README.md`
- `docs/CURRENT/README.md`
- `docs/MASTER_WEB_PLAN_V2_2026-08-17/`
- `docs/manual/`

الـFCR Issue body هو canonical current lifecycle state، والتعليقات هي chronological audit trail.

## 22. قاعدة الصيانة الذهبية

إذا كان أمامك خيار بين جعل الواجهة تبدو Live وبين الحفاظ على الحقيقة، اختر الحقيقة. Shared Web يجب أن يفشل مغلقًا، يعرض unavailable/stale/partial بصراحة، ويحافظ على الفصل بين العرض والصلاحية حتى لو كان ذلك أقل بريقًا بصريًا.
