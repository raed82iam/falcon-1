function interactiveFocusLost(documentRef) {
  const active=documentRef?.activeElement ?? null;
  return !active || active===documentRef?.body || active===documentRef?.documentElement;
}

function language(documentRef) {
  return documentRef?.documentElement?.lang === 'en' ? 'en' : 'ar';
}

/**
 * Browser-only accessibility synchronization for the customer incident surface.
 * It changes presentation/focus semantics only and creates no incident authority.
 */
export function synchronizeIncidentAccessibility(documentRef=globalThis.document) {
  if (!documentRef?.querySelector) return Object.freeze({ state:'NO_DOCUMENT' });

  const lang=language(documentRef);
  const dialog=documentRef.querySelector('.customer-incident-modal[role="dialog"]');
  const minimized=documentRef.querySelector('[data-incident-expand]');
  const reply=documentRef.querySelector('[data-incident-text]');

  if (dialog) {
    dialog.setAttribute('tabindex','-1');
    dialog.setAttribute('aria-describedby','incident-security-note');
    const security=documentRef.querySelector('.incident-security-note');
    if (security && !security.id) security.id='incident-security-note';

    documentRef.querySelectorAll('.customer-incident-modal button:disabled').forEach(control => {
      control.setAttribute('aria-disabled','true');
    });

    if (reply && !reply.getAttribute('aria-label')) {
      reply.setAttribute('aria-label',lang==='ar'?'اكتب ردك للحادثة':'Type your incident reply');
    }

    if (interactiveFocusLost(documentRef) && typeof dialog.focus === 'function') dialog.focus();
    return Object.freeze({ state:'DIALOG', focused:documentRef.activeElement===dialog });
  }

  if (minimized) {
    if (!minimized.getAttribute('aria-expanded')) minimized.setAttribute('aria-expanded','false');
    if (interactiveFocusLost(documentRef) && typeof minimized.focus === 'function') minimized.focus();
    return Object.freeze({ state:'MINIMIZED', focused:documentRef.activeElement===minimized });
  }

  return Object.freeze({ state:'NO_INCIDENT_SURFACE' });
}
