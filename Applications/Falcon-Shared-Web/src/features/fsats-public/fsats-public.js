import { displayText } from '../../design-system/presentation.js';

function requireFunction(value, name) {
  if (typeof value !== 'function') throw new TypeError(`${name} must be a function`);
  return value;
}

const ISOLATED_PRESENTATION_FALLBACK = Object.freeze([
  Object.freeze({ id:'fsata', shortName:'FSATA', name:'Falcon Self-Aware Trading Application' }),
  Object.freeze({ id:'fsapma', shortName:'FSAPMA', name:'Falcon Self-Aware Provider Management Application' }),
  Object.freeze({ id:'ftga', shortName:'FTGA', name:'Falcon Trading Guardian Application' }),
  Object.freeze({ id:'fstsim', shortName:'FSTSimA', name:'Falcon Self-Aware Trading Simulation Application' }),
  Object.freeze({ id:'app-rsc', shortName:'APP-RSC', name:'Falcon Self-Aware Resource Management Application' })
]);

const APP_COPY = Object.freeze({
  fsata: {
    ar:'تطبيق التداول الرئيسي داخل FSATS. يجمع تجربة المستخدم الخاصة بالتحليل والمتابعة واستخدام قدرات التداول التي يتيحها النظام ضمن الصلاحيات الفعلية.',
    en:'The primary Trading Application inside FSATS. It brings together the user experience for analysis, monitoring and governed Trading capabilities that are actually available.'
  },
  fsapma: {
    ar:'تطبيق إدارة مزودي البيانات والخدمات داخل منظومة التداول. يساعد FSATS على التعامل مع مصادره من خلال حدود وعقود واضحة بدل ربط الواجهة مباشرة بمزوّد بعينه.',
    en:'The provider-management Application for the Trading system. It lets FSATS work with governed provider capabilities without coupling the Web interface directly to one vendor.'
  },
  ftga: {
    ar:'تطبيق الحماية والرقابة الخاص بالتداول. يركز على عرض حالات الحماية والتنبيهات والنتائج الموثوقة بدون أن تتحول الواجهة نفسها إلى سلطة Guardian.',
    en:'The Trading protection and oversight Application. It exposes governed protection state, alerts and outcomes without turning the Web interface into Guardian authority.'
  },
  fstsim: {
    ar:'تطبيق المحاكاة داخل FSATS لاختبار السلوك والسيناريوهات في بيئة منفصلة عن الحقيقة التشغيلية الحية. العرض الحالي تعريفي ولا يدّعي وجود Runtime حي.',
    en:'The FSATS simulation Application for testing behavior and scenarios separately from live operational truth. The current public presentation is explanatory and does not claim a live runtime.'
  },
  'app-rsc': {
    ar:'تطبيق إدارة الموارد الخاص بـFSATS. يساعد النظام على فهم احتياجاته واستهلاكه وحالته المرتبطة بالموارد عبر الحدود الموثوقة، بدون أن تصبح واجهة الويب مالكة للموارد.',
    en:'The FSATS resource-management Application. It helps present resource needs, consumption and related state through governed boundaries without making Web the resource authority.'
  }
});

function normalizeApplications(value) {
  if (!Array.isArray(value)) throw new TypeError('fsatsApps must be an array');
  return value.map(app => Object.freeze({
    id:String(app?.id ?? ''),
    shortName:String(app?.shortName ?? ''),
    name:String(app?.name ?? '')
  }));
}

function appCards(applications, arabic) {
  const discover = arabic ? 'استكشف التطبيق' : 'Discover Application';
  const visual = arabic ? 'شرح مرئي مبسط' : 'Simple visual explainer';
  const note = arabic
    ? 'الشرح هنا تعريفي. الفيديو أو الصور النهائية تضاف لاحقًا بدون ادعاء قدرة تشغيلية غير موجودة.'
    : 'This is explanatory presentation. Final video or animated assets can be added later without implying unavailable runtime capability.';

  return applications.map(app => {
    const copy = APP_COPY[app.id] ?? {
      ar:'تطبيق مستقل داخل FSATS. تفاصيله التشغيلية تبقى مملوكة للمصدر المخول.',
      en:'An independent Application inside FSATS. Its operational details remain owned by the authoritative source.'
    };
    const shortName = displayText(app.shortName);
    const name = displayText(app.name);
    return `<article class="app-card fsats-child-card"><span class="status-chip">${shortName}</span><h3>${name}</h3><p>${displayText(copy[arabic ? 'ar' : 'en'])}</p><details class="discover-panel"><summary>${displayText(discover)}</summary><div class="explainer-media" role="img" aria-label="${displayText(visual)}"><span>INPUT</span><i>→</i><span>${shortName}</span><i>→</i><span>RESULT</span></div><p class="muted tiny">${displayText(note)}</p></details></article>`;
  }).join('');
}

/**
 * Public FSATS product, child-Application discovery, sign-in and onboarding presentation.
 *
 * Authentication remains behind the Web auth adapter. This renderer never
 * invents identity, entitlement, licensing, trading authority, or runtime truth.
 */
export function createFsatsPublicFeature({ t, language, publicShell, icon, fsatsApps = [] } = {}) {
  const translate = requireFunction(t, 't');
  const currentLanguage = requireFunction(language, 'language');
  const shell = requireFunction(publicShell, 'publicShell');
  const renderIcon = requireFunction(icon, 'icon');
  const supplied = normalizeApplications(fsatsApps);
  const applications = supplied.length > 0 ? supplied : normalizeApplications(ISOLATED_PRESENTATION_FALLBACK);

  function fsatsLanding() {
    const arabic = currentLanguage() === 'ar';
    const previewLabel = arabic ? 'معاينة واجهة المستخدم' : 'Preview user workspace';
    const unavailable = arabic
      ? 'ربط تسجيل الدخول التشغيلي غير متاح بعد. لن يخترع الويب هوية أو صلاحية.'
      : 'Operational sign-in binding is not available yet. Web will not invent identity or authority.';
    const googleLabel = arabic ? 'المتابعة باستخدام Google' : 'Continue with Google';
    const microsoftLabel = arabic ? 'المتابعة باستخدام Microsoft' : 'Continue with Microsoft';
    const mfaTitle = arabic ? 'حماية إضافية للحساب' : 'Extra account protection';
    const mfaText = arabic
      ? 'بعد تسجيل الدخول، قد يُطلب عامل Authenticator. Google Authenticator وMicrosoft Authenticator أمثلة متوافقة، بدون ربط Falcon بعلامة واحدة.'
      : 'After sign-in, an Authenticator factor may be required. Google Authenticator and Microsoft Authenticator are compatible examples without locking Falcon to one brand.';
    const appsTitle = arabic ? 'تطبيقات نظام FSATS' : 'FSATS System Applications';
    const appsText = arabic
      ? 'FSATS هو نظام التداول، وهذه التطبيقات تعمل داخله. أي نظام غير تداولي مستقبلي يبقى بجانب FSATS تحت Falcon OS وليس تحته.'
      : 'FSATS is the Trading system and these Applications live inside it. Future non-Trading systems remain siblings of FSATS under Falcon OS, not children of FSATS.';
    const createTitle = arabic ? 'إنشاء حساب Falcon' : 'Create Falcon Account';
    const createText = arabic ? 'ابدأ بحساب Falcon واحد، ثم فعّل الأنظمة المتاحة لك.' : 'Start with one Falcon Account, then enable the systems available to you.';
    const phoneLabel = arabic ? 'رقم الهاتف للتواصل الطارئ' : 'Phone number for emergency contact';
    const phoneHelp = arabic
      ? 'مطلوب للتواصل معك في الحالات الطارئة والتنبيهات عالية الأولوية. لا يعتبر الرقم تحقق OTP حاليًا ولا يُستخدم للتسويق تلقائيًا.'
      : 'Required so Falcon can contact you during emergencies and high-priority incidents. Providing the number is not OTP verification today and does not automatically opt you into marketing.';
    const createUnavailable = arabic
      ? 'إنشاء الحساب التشغيلي وربط الهوية ما زال يحتاج الـauthoritative authentication boundary. هذه الواجهة لا تنشئ حسابًا حقيقيًا من نفسها.'
      : 'Operational account creation and identity binding still require the authoritative authentication boundary. This presentation does not create a real account by itself.';

    return shell(`<section class="fsats-landing"><div class="fsats-copy"><span class="eyebrow">FALCON • FSATS</span><h1>${translate('fsatsHero')}</h1><p>${translate('fsatsText')}</p><div class="hero-actions"><button class="primary" data-focus-login>${translate('start')}</button><button class="secondary preview-action" data-nav="trader">${previewLabel}</button></div><div class="feature-strip"><span>${renderIcon('ai')} ${translate('falconAI')}</span><span>${renderIcon('shield')} ${translate('risk')}</span><span>${renderIcon('market')} ${translate('markets')}</span></div></div><div class="login-card" id="login-card"><h2>${translate('signIn')}</h2><p>${translate('welcome')}</p><div class="federated-auth"><button class="secondary full auth-provider" data-auth-provider="GOOGLE" aria-label="${googleLabel}"><span aria-hidden="true">G</span>${googleLabel}</button><button class="secondary full auth-provider" data-auth-provider="MICROSOFT" aria-label="${microsoftLabel}"><span aria-hidden="true">M</span>${microsoftLabel}</button></div><div class="or">${arabic ? 'أو' : 'or'}</div><label>${translate('email')}<input id="login-user" name="username" autocomplete="username" placeholder="name@example.com"></label><label>${translate('password')}<input id="login-pass" name="password" type="password" autocomplete="current-password"></label><div class="form-row"><label class="check"><input type="checkbox" name="remember-me"> ${translate('remember')}</label><button class="link">${translate('forgot')}</button></div><button class="primary full" data-auth-submit>${translate('signIn')}</button><p class="auth-status" id="auth-status" hidden>${unavailable}</p><section class="mfa-note" aria-label="${mfaTitle}"><strong>${mfaTitle}</strong><p>${mfaText}</p></section></div></section><section class="section fsats-applications"><div class="section-head"><div><h2>${appsTitle}</h2><p>${appsText}</p></div></div><div class="apps-grid">${appCards(applications,arabic)}</div></section><section class="section onboarding-section"><div class="section-head"><div><h2>${createTitle}</h2><p>${createText}</p></div></div><div class="onboarding-grid"><form class="account-card" aria-describedby="account-create-status"><label>${arabic ? 'الاسم' : 'Name'}<input name="full-name" autocomplete="name" placeholder="${arabic ? 'الاسم الكامل' : 'Full name'}"></label><label>${translate('email')}<input name="email" type="email" autocomplete="email" placeholder="name@example.com"></label><label>${phoneLabel}<input name="emergency-phone" type="tel" autocomplete="tel" inputmode="tel" required placeholder="+966 5X XXX XXXX"></label><p class="field-help">${phoneHelp}</p><button class="primary full" type="button" disabled aria-disabled="true">${translate('createAccount')}</button><p class="auth-status" id="account-create-status">${createUnavailable}</p></form><aside class="onboarding-truth"><strong>${arabic ? 'فصل مهم' : 'Important separation'}</strong><p>PHONE_PROVIDED ≠ PHONE_VERIFIED ≠ FALCON_IDENTITY ≠ BUSINESS_AUTHORITY</p><p>${arabic ? 'خدمة OTP عبر Telegram أو WhatsApp أو SMS مؤجلة للمستقبل ولا تُفعل هنا.' : 'OTP delivery through Telegram, WhatsApp or SMS is deferred for future review and is not activated here.'}</p></aside></div></section>`, 'fsats');
  }

  return Object.freeze({ fsatsLanding });
}
