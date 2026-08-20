/** Reusable, domain-neutral Shared Web presentation primitives. */

const ICONS = Object.freeze({
  home: '⌂',
  apps: '▦',
  market: '◉',
  portfolio: '◫',
  activity: '↻',
  ai: '✦',
  bell: '◉',
  gear: '⚙',
  shield: '◇',
  user: '◎',
  audit: '≡',
  warning: '△'
});

function escapeHtml(value) {
  return String(value ?? '')
    .replaceAll('&', '&amp;')
    .replaceAll('<', '&lt;')
    .replaceAll('>', '&gt;')
    .replaceAll('"', '&quot;')
    .replaceAll("'", '&#39;');
}

export function icon(name) {
  const glyph = ICONS[name] ?? '•';
  return `<span class="icon" aria-hidden="true">${glyph}</span>`;
}

export function demoBadge(label) {
  return `<div class="demo-badge">${escapeHtml(label)}</div>`;
}

export function displayText(value, fallback = '—') {
  return value === null || value === undefined || value === ''
    ? fallback
    : escapeHtml(value);
}

export function toneClass(value) {
  return String(value ?? '').trim().startsWith('-') ? 'negative' : 'positive';
}

export const __test = Object.freeze({ escapeHtml });
