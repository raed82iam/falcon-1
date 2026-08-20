# Shared Falcon Web - OTP المستقبلي وقنوات التحقق

**Status:** `FUTURE IDEA / DEFERRED / NOT CURRENT IMPLEMENTATION`
**Date:** 2026-08-15
**Branch:** `web-development`
**Scope:** `applications/shared/web/**`

يسجل هذا الملف قرار الـProject Owner بتأجيل خدمة OTP في الوقت الحالي، مع الاحتفاظ بها كفكرة مستقبلية يمكن تفعيلها لاحقًا بعد المراجعة والاعتماد المناسبين.

القرار الحالي:

```text
OTP_SERVICE = DEFERRED
OTP_CURRENT_IMPLEMENTATION = NOT_AUTHORIZED_AS_ACTIVE_PRODUCT_FLOW
OTP_FUTURE_REVIEW = REQUIRED_BEFORE_ACTIVATION
```

---

## 1. الهدف المستقبلي

قد يستخدم Falcon لاحقًا رمز تحقق لمرة واحدة `OTP` كعامل تحقق إضافي أو كقناة استعادة/تأكيد، حسب السياسة الأمنية المعتمدة وقتها.

الاستخدامات المحتملة مستقبلًا تشمل:

- تأكيد تسجيل الدخول عندما تتطلب السياسة ذلك.
- Step-Up authentication لبعض الإجراءات الحساسة.
- recovery أو verification إضافي عندما يكون ذلك مسموحًا.
- تأكيد ربط قناة تواصل بحساب Falcon.

لكن وجود OTP لا يمنح صلاحية أو دورًا أو entitlement من نفسه.

```text
OTP_VERIFIED != FALCON_IDENTITY
OTP_VERIFIED != PROJECT_OWNER
OTP_VERIFIED != BUSINESS_AUTHORITY
OTP_VERIFIED != ENTITLEMENT
```

---

## 2. قنوات التوصيل المرشحة مستقبلًا

القنوات التي نوقشت كخيارات مستقبلية:

1. `Telegram`
2. `WhatsApp`
3. `SMS`

الاتجاه المفضل مبدئيًا عند إعادة فتح هذه الفكرة هو تقييم Telegram أولًا من ناحية سهولة الاستخدام والتكلفة، مع إبقاء WhatsApp وSMS كبدائل محتملة حسب التوفر والمتطلبات والتنظيم والتكلفة في وقت التنفيذ.

هذا ليس اعتمادًا نهائيًا لأي قناة.

```text
TELEGRAM = FUTURE_CANDIDATE
WHATSAPP = FUTURE_CANDIDATE
SMS = FUTURE_CANDIDATE
CURRENT_PROVIDER_SELECTION = NONE
```

---

## 3. منع Vendor Lock-in

إذا تم تنفيذ OTP لاحقًا، لا يربط Falcon نفسه مباشرة بمزوّد واحد أو قناة واحدة.

النمط المستقبلي المرشح:

```text
Falcon Authentication Boundary
        ↓
OTP Delivery Port / Verification Contract
        ↓
Channel Adapter
        ├── Telegram Adapter
        ├── WhatsApp Adapter
        ├── SMS Adapter
        └── Future Adapter
```

Falcon يعتمد على capability/contract وليس على اسم مزود بعينه.

```text
FALCON_DEPENDS_ON_OTP_CAPABILITY
FALCON_DOES_NOT_DEPEND_ON_OTP_VENDOR
```

---

## 4. الحدود الأمنية

عند إعادة فتح الفكرة مستقبلًا يجب الحفاظ على الأقل على التالي:

- الرمز قصير العمر.
- One-time use فقط.
- rate limiting ومحاولات محدودة.
- منع replay.
- عدم تسجيل OTP في logs أو analytics أو chat.
- عدم تخزين secret material داخل Web-owned reusable state.
- ربط التحقق بمحاولة/session محددة وليس بالرقم أو الحساب بشكل عام.
- recovery لا يصبح bypass أضعف من المصادقة الأساسية.
- Owner accounts تتطلب حماية مساوية أو أقوى من العملاء، وليس مسارًا أخف.
- نتيجة القناة ليست Falcon authority بحد ذاتها.

---

## 5. العلاقة مع المصادقة الحالية

قرار التأجيل هذا لا يلغي الاتجاه الحالي لـ:

- `Continue with Google`.
- `Continue with Microsoft`.
- Authenticator-based MFA / TOTP ضمن الحدود authoritative.
- مستقبل Passkeys/WebAuthn إذا تم اعتماده لاحقًا.

OTP عبر Telegram/WhatsApp/SMS يبقى capability إضافية مستقبلية فقط ولا يتم إدخاله الآن كمسار تشغيلي مطلوب.

---

## 6. شرط إعادة التفعيل مستقبلًا

قبل تنفيذ أو تفعيل OTP يجب عمل مراجعة جديدة تشمل على الأقل:

- الحاجة الفعلية والـUX.
- الأمان والتهديدات.
- التكلفة والتوفر حسب الدول المستهدفة.
- الخصوصية والامتثال.
- مقارنة القنوات والمزودين الحاليين وقتها.
- authoritative identity/session/MFA boundary.
- fail-closed behavior.
- Architecture / Security / Red Team verification المناسبة.
- Owner approval قبل production activation.

---

هذا الملف يحفظ الفكرة فقط للمستقبل. لا ينشئ تكامل Telegram أو WhatsApp أو SMS، ولا يمنح production connectivity أو identity/session/MFA authority.
