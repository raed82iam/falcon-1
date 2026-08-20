import { escapeHtml, escapeAttr, safeText } from '../security/safe-html.js';

export const StatusTone = Object.freeze({
  POSITIVE:'positive',
  WARNING:'warning',
  NEGATIVE:'negative',
  NEUTRAL:'neutral',
  UNAVAILABLE:'unavailable'
});

const STATUS_SYMBOL = Object.freeze({
  [StatusTone.POSITIVE]:'✓',
  [StatusTone.WARNING]:'!',
  [StatusTone.NEGATIVE]:'×',
  [StatusTone.NEUTRAL]:'•',
  [StatusTone.UNAVAILABLE]:'?'
});

function normalizeTone(value) {
  return Object.values(StatusTone).includes(value) ? value : StatusTone.NEUTRAL;
}

function requireArray(value,name) {
  if (!Array.isArray(value)) throw new TypeError(`${name} must be an array`);
  return value;
}

export function visuallyHidden(text) {
  const value = String(text ?? '');
  return value ? `<span class="visually-hidden">${escapeHtml(value)}</span>` : '';
}

export function statusBadge({ label, tone = StatusTone.NEUTRAL, accessibleLabel = null } = {}) {
  const normalized = normalizeTone(tone);
  const visible = safeText(label);
  const aria = accessibleLabel ? ` aria-label="${escapeAttr(accessibleLabel)}"` : '';
  return `<span class="ds-status ds-status--${normalized}"${aria}><span class="ds-status__symbol" aria-hidden="true">${STATUS_SYMBOL[normalized]}</span><span${accessibleLabel ? ' aria-hidden="true"' : ''}>${visible}</span></span>`;
}

export function notice({ title = '', body = '', tone = StatusTone.NEUTRAL, role = 'status' } = {}) {
  const normalized = normalizeTone(tone);
  const safeRole = role === 'alert' ? 'alert' : 'status';
  return `<section class="ds-notice ds-notice--${normalized}" role="${safeRole}">${title ? `<strong>${safeText(title)}</strong>` : ''}${body ? `<p>${safeText(body)}</p>` : ''}</section>`;
}

export function disabledControlAttributes(disabled, reason = '') {
  if (!disabled) return '';
  const description = reason ? ` aria-description="${escapeAttr(reason)}"` : '';
  return ` disabled aria-disabled="true"${description}`;
}

export function sectionCard({ title = '', body = '', headingLevel = 2, className = '' } = {}) {
  const level = Number.isInteger(headingLevel) && headingLevel >= 2 && headingLevel <= 4 ? headingLevel : 2;
  const classes = ['ds-card', className].filter(Boolean).map(value => escapeAttr(value)).join(' ');
  return `<section class="${classes}">${title ? `<h${level}>${safeText(title)}</h${level}>` : ''}${body ? `<div class="ds-card__body">${safeText(body)}</div>` : ''}</section>`;
}

export function formField({ id, label, type = 'text', value = '', description = '', required = false, disabled = false } = {}) {
  if (typeof id !== 'string' || !id.trim()) throw new TypeError('id is required');
  const fieldId = escapeAttr(id.trim());
  const descriptionId = description ? `${fieldId}-description` : null;
  const safeType = ['text','email','password','search','number','url','tel'].includes(type) ? type : 'text';
  const describedBy = descriptionId ? ` aria-describedby="${descriptionId}"` : '';
  const requiredAttrs = required ? ' required aria-required="true"' : '';
  const disabledAttrs = disabledControlAttributes(disabled);
  return `<div class="ds-field"><label for="${fieldId}">${safeText(label)}</label><input id="${fieldId}" type="${safeType}" value="${escapeAttr(value)}"${describedBy}${requiredAttrs}${disabledAttrs}>${description ? `<small id="${descriptionId}" class="ds-field__description">${safeText(description)}</small>` : ''}</div>`;
}

export function dataTable({ caption = '', columns = [], rows = [] } = {}) {
  const safeColumns = requireArray(columns,'columns');
  const safeRows = requireArray(rows,'rows');
  if (safeColumns.length === 0) throw new TypeError('columns must not be empty');
  const headers = safeColumns.map(column => `<th scope="col">${safeText(column)}</th>`).join('');
  const body = safeRows.map(row => {
    if (!Array.isArray(row) || row.length !== safeColumns.length) throw new TypeError('each row must match columns length');
    return `<tr>${row.map(value => `<td>${safeText(value)}</td>`).join('')}</tr>`;
  }).join('');
  const regionAttributes = caption ? ` role="region" aria-label="${escapeAttr(caption)}"` : '';
  return `<div class="ds-table-wrap"${regionAttributes}><table class="ds-table">${caption ? `<caption>${safeText(caption)}</caption>` : ''}<thead><tr>${headers}</tr></thead><tbody>${body}</tbody></table></div>`;
}

export const __test = Object.freeze({ normalizeTone });
