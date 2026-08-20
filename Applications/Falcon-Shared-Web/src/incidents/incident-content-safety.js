const SECRET_PATTERNS = Object.freeze([
  /\bsk-[A-Za-z0-9_-]{12,}\b/i,
  /\bapi[_ -]?key\b\s*[:=]\s*[^\s]{6,}/i,
  /\bsecret\b\s*[:=]\s*[^\s]{6,}/i,
  /\bpassword\b\s*[:=]\s*[^\s]{4,}/i,
  /\bpasswd\b\s*[:=]\s*[^\s]{4,}/i,
  /\btoken\b\s*[:=]\s*[^\s]{8,}/i,
  /\bbearer\s+[A-Za-z0-9._~+\/-]+=*/i,
  /\b(?:access|refresh)[_ -]?token\b\s*[:=]\s*[^\s]{8,}/i
]);

export const IncidentContentSafetyReason = Object.freeze({
  CLEAN:'CLEAN',
  SECRET_DETECTED:'SECRET_DETECTED',
  TRANSCRIPT_UNAVAILABLE:'TRANSCRIPT_UNAVAILABLE',
  TRANSCRIPT_CONFIDENCE_UNCERTAIN:'TRANSCRIPT_CONFIDENCE_UNCERTAIN'
});

export function inspectIncidentText(value) {
  const text = String(value ?? '');
  for (const pattern of SECRET_PATTERNS) {
    if (pattern.test(text)) return Object.freeze({ ok:false, reason:IncidentContentSafetyReason.SECRET_DETECTED });
  }
  return Object.freeze({ ok:true, reason:IncidentContentSafetyReason.CLEAN });
}

export function inspectVoiceTranscript({ text, confidence, minimumConfidence = 0.55 } = {}) {
  if (typeof text !== 'string' || !text.trim()) {
    return Object.freeze({ ok:false, reason:IncidentContentSafetyReason.TRANSCRIPT_UNAVAILABLE });
  }
  if (typeof confidence === 'number' && Number.isFinite(confidence) && confidence < minimumConfidence) {
    return Object.freeze({ ok:false, reason:IncidentContentSafetyReason.TRANSCRIPT_CONFIDENCE_UNCERTAIN });
  }
  return inspectIncidentText(text);
}
