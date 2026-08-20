import { safeText } from '../../security/safe-html.js';

function requireFunction(value, name) {
  if (typeof value !== 'function') throw new TypeError(`${name} must be a function`);
  return value;
}

/**
 * Web-owned presentation-only settings surface.
 *
 * Language preference is Web UX state. This feature does not mutate Falcon,
 * FSATS, provider, identity, trading or business authority.
 */
export function createSettingsFeature({ t, language, workspace, localize } = {}) {
  const translate = requireFunction(t, 't');
  const currentLanguage = requireFunction(language, 'language');
  const renderWorkspace = requireFunction(workspace, 'workspace');
  const local = requireFunction(localize, 'localize');

  function settingsPage() {
    const lang = currentLanguage();
    return renderWorkspace(`<div class="page-head"><h1>${safeText(translate('settings'))}</h1></div><section class="widget form-settings"><label>${safeText(translate('language'))}<select data-language-select><option value="ar" ${lang==='ar'?'selected':''}>العربية</option><option value="en" ${lang==='en'?'selected':''}>English</option></select></label><p class="muted">${safeText(local('يتم تطبيق اتجاه الواجهة من اليمين إلى اليسار أو العكس على كامل الواجهة، ويتم حفظ اختيارك.','RTL/LTR applies to the complete interface and your preference is saved.'))}</p></section>`,'settings');
  }

  return Object.freeze({ settingsPage });
}
