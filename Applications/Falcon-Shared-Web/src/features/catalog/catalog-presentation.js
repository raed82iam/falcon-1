import { safeText } from '../../security/safe-html.js';
import { presentCatalog } from '../../presenters.js';

function requireFunction(value, name) {
  if (typeof value !== 'function') throw new TypeError(`${name} must be a function`);
  return value;
}

/**
 * Presentation-only School/Strategy catalog renderer.
 *
 * Applicability, entitlement and strategy truth remain Application-owned.
 * This feature only renders the supplied governed presentation model.
 */
export function createCatalogPresentation({ t, language, catalog } = {}) {
  const translate = requireFunction(t, 't');
  const currentLanguage = requireFunction(language, 'language');
  if (!Array.isArray(catalog)) throw new TypeError('catalog must be an array');

  function catalogMarkup() {
    return presentCatalog(catalog).map(item => {
      const kind = item.kind === 'SCHOOL' ? translate('schools') : item.kind === 'STRATEGY' ? translate('strategies') : item.kind;
      const reason = item.presentation.reason
        ? (currentLanguage() === 'ar' ? 'غير قابل للتطبيق على الأصل أو السياق الحالي.' : item.presentation.reason)
        : '';
      return `<div class="catalog-item ${item.presentation.enabled?'':'disabled'}"><div><b>${safeText(item.name)}</b><small>${safeText(kind)}</small></div><button ${item.presentation.enabled?'':'disabled'}>${item.presentation.enabled?'+':'×'}</button>${reason?`<p>${safeText(reason)}</p>`:''}</div>`;
    }).join('');
  }

  return Object.freeze({ catalogMarkup });
}
