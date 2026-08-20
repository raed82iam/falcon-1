import { routeAuthenticatedIdentity } from '../auth.js';
import { escapeAttr } from '../security/safe-html.js';
import { synchronizeIncidentAccessibility } from '../incidents/incident-accessibility.js';

function requireFunction(value,name) {
  if (typeof value !== 'function') throw new TypeError(`${name} must be a function`);
  return value;
}

function setLanguageResilient(i18n,next) {
  try {
    i18n.set(next);
    return true;
  } catch {
    return false;
  }
}

function syncLanguageAccessibility(documentRef,i18n) {
  const lang = i18n.lang === 'en' ? 'en' : 'ar';
  if (documentRef?.documentElement) {
    documentRef.documentElement.lang = lang;
    documentRef.documentElement.dir = lang === 'ar' ? 'rtl' : 'ltr';
  }
  const skipLink=documentRef?.querySelector?.('[data-skip-link]');
  if (skipLink) skipLink.textContent = lang === 'ar' ? 'تجاوز إلى المحتوى' : 'Skip to content';
}

function moveWidgetByKeyboard(store,widgetId,direction) {
  const prefs=store.layout();
  const order=[...prefs.order];
  const index=order.indexOf(widgetId);
  if (index < 0) return false;
  const targetIndex=direction === 'up' ? index - 1 : direction === 'down' ? index + 1 : index;
  if (targetIndex < 0 || targetIndex >= order.length || targetIndex === index) return false;
  store.reorderWidget(widgetId,order[targetIndex]);
  return true;
}

/** Browser DOM bindings for the Shared Web application shell. */
export function bindAppUi({
  documentRef = globalThis.document,
  navigate,
  i18n,
  render,
  auth,
  setSession,
  store,
  incidentRuntime
} = {}) {
  const go = requireFunction(navigate,'navigate');
  const rerender = requireFunction(render,'render');
  const updateSession = requireFunction(setSession,'setSession');
  if (!i18n || typeof i18n.set !== 'function' || typeof i18n.lang !== 'string') throw new TypeError('i18n is required');
  if (!auth || typeof auth.signIn !== 'function') throw new TypeError('auth.signIn is required');
  if (!store || typeof store !== 'object') throw new TypeError('store is required');
  if (!incidentRuntime || typeof incidentRuntime.bindActions !== 'function') throw new TypeError('incidentRuntime.bindActions is required');

  syncLanguageAccessibility(documentRef,i18n);

  documentRef?.querySelectorAll?.('[data-nav]')?.forEach(el => el.addEventListener('click', () => go(el.dataset.nav)));
  documentRef?.querySelectorAll?.('[data-language]')?.forEach(el => el.addEventListener('click', () => {
    setLanguageResilient(i18n,i18n.lang === 'ar' ? 'en' : 'ar');
    rerender();
  }));
  documentRef?.querySelector?.('[data-language-select]')?.addEventListener('change', event => {
    setLanguageResilient(i18n,event.target.value);
    rerender();
  });
  documentRef?.querySelector?.('[data-focus-login]')?.addEventListener('click', () => {
    documentRef?.querySelector?.('#login-card')?.scrollIntoView({ behavior:'smooth' });
  });

  documentRef?.querySelector?.('[data-auth-submit]')?.addEventListener('click', async () => {
    const result = await auth.signIn({
      username:documentRef?.querySelector?.('#login-user')?.value ?? '',
      password:documentRef?.querySelector?.('#login-pass')?.value ?? ''
    });
    const destination = routeAuthenticatedIdentity(result);
    if (destination) {
      updateSession(result);
      go(destination);
      return;
    }
    updateSession(null);
    const status = documentRef?.querySelector?.('#auth-status');
    if (status) status.hidden = false;
  });

  documentRef?.querySelectorAll?.('[data-hide]')?.forEach(el => el.addEventListener('click', () => {
    store.hideWidget(el.dataset.hide);
    rerender();
  }));
  documentRef?.querySelectorAll?.('[data-show]')?.forEach(el => el.addEventListener('click', () => {
    store.showWidget(el.dataset.show);
    rerender();
  }));
  documentRef?.querySelector?.('[data-reset]')?.addEventListener('click', () => {
    store.resetLayout();
    rerender();
  });
  documentRef?.querySelectorAll?.('[data-size]')?.forEach(el => el.addEventListener('click', () => {
    const id = escapeAttr(el.dataset.size ?? '');
    documentRef?.querySelector?.(`[data-widget="${id}"]`)?.classList.toggle('wide');
  }));

  const manageButton=documentRef?.querySelector?.('[data-manage]');
  const restorePanel=documentRef?.querySelector?.('.restore-panel');
  if (manageButton) {
    if (!restorePanel) {
      manageButton.disabled=true;
      manageButton.setAttribute?.('aria-disabled','true');
    } else {
      manageButton.addEventListener('click',()=>{
        restorePanel.scrollIntoView?.({behavior:'smooth',block:'nearest'});
        restorePanel.querySelector?.('button')?.focus?.();
      });
    }
  }

  const grid = documentRef?.querySelector?.('#dashboard-grid');
  grid?.querySelectorAll?.('[data-widget]')?.forEach(widget => {
    widget.draggable = true;
    widget.tabIndex = widget.tabIndex >= 0 ? widget.tabIndex : 0;
    widget.setAttribute?.('aria-keyshortcuts','Alt+ArrowUp Alt+ArrowDown');
    widget.addEventListener('dragstart', event => event.dataTransfer.setData('text/widget', widget.dataset.widget));
    widget.addEventListener('dragover', event => event.preventDefault());
    widget.addEventListener('drop', event => {
      event.preventDefault();
      store.reorderWidget(event.dataTransfer.getData('text/widget'),widget.dataset.widget);
      rerender();
    });
    widget.addEventListener('keydown',event=>{
      if (!event.altKey || !['ArrowUp','ArrowDown'].includes(event.key)) return;
      event.preventDefault();
      const moved=moveWidgetByKeyboard(store,widget.dataset.widget,event.key === 'ArrowUp' ? 'up' : 'down');
      if (moved) rerender();
    });
  });

  incidentRuntime.bindActions();
  synchronizeIncidentAccessibility(documentRef);
}

export const __test = Object.freeze({ setLanguageResilient, syncLanguageAccessibility, moveWidgetByKeyboard });
