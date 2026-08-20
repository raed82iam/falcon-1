const unavailable = Object.freeze({
  accepted:false,
  delivered:false,
  authorityGranted:false,
  transportAvailable:false,
  reason:'SUPPORT_TRANSPORT_UNAVAILABLE',
  requestId:null,
  evidenceReference:null
});

export function createUnavailableIncidentSupportTransportPort() {
  return Object.freeze({
    async requestSupport() { return unavailable; }
  });
}

export function validateSupportTransportDecision(decision, request = {}) {
  if (!decision || typeof decision !== 'object') return unavailable;
  if (decision.accepted !== true || decision.delivered !== true) {
    return Object.freeze({ ...unavailable, reason:decision.reason ?? 'SUPPORT_REQUEST_NOT_ACCEPTED' });
  }
  if (decision.authorityGranted !== false) {
    return Object.freeze({ ...unavailable, reason:'SUPPORT_TRANSPORT_AUTHORITY_LEAK' });
  }
  if (typeof decision.requestId !== 'string' || decision.requestId.trim() === '') {
    return Object.freeze({ ...unavailable, reason:'SUPPORT_REQUEST_ID_REQUIRED' });
  }
  if (typeof decision.evidenceReference !== 'string' || decision.evidenceReference.trim() === '') {
    return Object.freeze({ ...unavailable, reason:'SUPPORT_TRANSPORT_EVIDENCE_REQUIRED' });
  }
  if (decision.incidentId !== request.incidentId) {
    return Object.freeze({ ...unavailable, reason:'SUPPORT_TRANSPORT_INCIDENT_MISMATCH' });
  }
  if (decision.principalId !== request.principalId || decision.sessionId !== request.sessionId) {
    return Object.freeze({ ...unavailable, reason:'SUPPORT_TRANSPORT_SESSION_MISMATCH' });
  }
  return Object.freeze({
    accepted:true,
    delivered:true,
    authorityGranted:false,
    transportAvailable:true,
    reason:null,
    requestId:decision.requestId,
    evidenceReference:decision.evidenceReference,
    incidentId:decision.incidentId,
    principalId:decision.principalId,
    sessionId:decision.sessionId
  });
}
