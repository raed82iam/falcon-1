export const IncidentEventType = Object.freeze({
  INCIDENT_OPENED:'INCIDENT_OPENED',
  TEXT_MESSAGE:'TEXT_MESSAGE',
  VOICE_MESSAGE:'VOICE_MESSAGE',
  VOICE_TRANSCRIPT:'VOICE_TRANSCRIPT',
  FALCON_VOICE_REPLY:'FALCON_VOICE_REPLY',
  SCREENSHOT:'SCREENSHOT',
  GUIDED_STEP:'GUIDED_STEP',
  CUSTOMER_REPLY:'CUSTOMER_REPLY',
  SUPPORT_REQUESTED:'SUPPORT_REQUESTED',
  SUPPORT_AVAILABLE:'SUPPORT_AVAILABLE',
  SUPPORT_TAKEOVER:'SUPPORT_TAKEOVER',
  SUPPORT_RELEASE:'SUPPORT_RELEASE',
  INCIDENT_STATE_UPDATE:'INCIDENT_STATE_UPDATE',
  SIMULATOR_UPDATE:'SIMULATOR_UPDATE',
  INCIDENT_RESOLVED:'INCIDENT_RESOLVED',
  CLOSURE_SUMMARY:'CLOSURE_SUMMARY'
});

export const IncidentActor = Object.freeze({
  CUSTOMER:'CUSTOMER',
  FALCON:'FALCON',
  SUPPORT:'SUPPORT',
  APPLICATION:'APPLICATION',
  GUARDIAN:'GUARDIAN',
  SYSTEM:'SYSTEM'
});

function requiredString(value, name) {
  if (typeof value !== 'string' || value.length === 0) throw new TypeError(`${name} is required`);
  return value;
}

export function createIncidentEvent({ eventId, incidentId, timestamp, actor, type, payload = {}, provenance = 'WEB_RECORDED' } = {}) {
  requiredString(eventId, 'eventId');
  requiredString(incidentId, 'incidentId');
  requiredString(timestamp, 'timestamp');
  if (!Object.values(IncidentActor).includes(actor)) throw new TypeError('invalid actor');
  if (!Object.values(IncidentEventType).includes(type)) throw new TypeError('invalid event type');
  return Object.freeze({ eventId, incidentId, timestamp, actor, type, payload:structuredClone(payload), provenance });
}

export function createIncidentTimeline(incidentId, initialEvents = []) {
  requiredString(incidentId, 'incidentId');
  let events = initialEvents.map(event => createIncidentEvent({ ...event, incidentId }));

  function append(event) {
    const next = createIncidentEvent({ ...event, incidentId });
    events = [...events, next].sort((a,b) => new Date(a.timestamp).getTime() - new Date(b.timestamp).getTime());
    return next;
  }

  function snapshot() {
    return Object.freeze(events.map(event => Object.freeze(structuredClone(event))));
  }

  return Object.freeze({ incidentId, append, snapshot });
}
