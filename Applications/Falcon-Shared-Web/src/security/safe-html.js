const ESCAPE_MAP = Object.freeze({
  '&':'&amp;',
  '<':'&lt;',
  '>':'&gt;',
  '"':'&quot;',
  "'":'&#39;'
});

export function escapeHtml(value) {
  return String(value ?? '').replace(/[&<>"']/g, ch => ESCAPE_MAP[ch]);
}

export function escapeAttr(value) {
  return escapeHtml(value);
}

export function safeText(value, fallback='—') {
  const resolved = value === null || value === undefined || value === '' ? fallback : value;
  return escapeHtml(resolved);
}
