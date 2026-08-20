import { IncidentActor, IncidentEventType, createIncidentEvent, createIncidentTimeline } from './incident-timeline.js';
import { inspectIncidentText } from './incident-content-safety.js';

function requiredString(value, name) {
  if (typeof value !== 'string' || value.length === 0) throw new TypeError(`${name} is required`);
  return value;
}

function clone(value) { return structuredClone(value); }

function mergeEvents(...collections) {
  const byId = new Map();
  for (const collection of collections) {
    for (const event of Array.isArray(collection) ? collection : []) {
      if (event?.eventId) byId.set(event.eventId, event);
    }
  }
  return [...byId.values()].sort((a,b) => new Date(a.timestamp).getTime() - new Date(b.timestamp).getTime());
}

export function createIncidentController({ incidentId, persistence, initialRecord = {}, now = () => new Date().toISOString(), idFactory = () => crypto.randomUUID() } = {}) {
  requiredString(incidentId, 'incidentId');
  if (!persistence) throw new TypeError('persistence is required');

  let timeline = createIncidentTimeline(incidentId, initialRecord.timeline ?? []);
  let record = {
    incidentId,
    priority:initialRecord.priority ?? 'HIGH',
    status:initialRecord.status ?? 'OPEN',
    mode:initialRecord.mode ?? 'FALCON_ACTIVE',
    resolved:initialRecord.resolved === true,
    minimized:initialRecord.minimized === true,
    supportRequested:initialRecord.supportRequested === true,
    supportAvailable:initialRecord.supportAvailable === true,
    supportDisplayName:initialRecord.supportDisplayName ?? null,
    closureSummary:initialRecord.closureSummary ?? null,
    openedAt:initialRecord.openedAt ?? now(),
    resolvedAt:initialRecord.resolvedAt ?? null,
    timeline:timeline.snapshot()
  };

  async function persistStandalone(nextRecord) {
    const saved = await persistence.saveRecord(nextRecord);
    if (!saved.ok) return saved;
    record = { ...nextRecord, timeline:timeline.snapshot() };
    return { ok:true, record:snapshot() };
  }

  async function append({ actor, type, payload = {}, provenance = 'WEB_RECORDED', recordPatch = {} } = {}) {
    const event = createIncidentEvent({ eventId:idFactory(), incidentId, timestamp:now(), actor, type, payload, provenance });
    const nextTimeline = createIncidentTimeline(incidentId, [...timeline.snapshot(), event]);
    const nextRecord = { ...record, ...clone(recordPatch), timeline:nextTimeline.snapshot() };

    let persisted;
    if (typeof persistence.commitRecordAndEvent === 'function') {
      persisted = await persistence.commitRecordAndEvent(nextRecord, event);
    } else {
      const eventResult = await persistence.appendEvent(event);
      if (!eventResult.ok) return { ok:false, reason:'INCIDENT_EVENT_PERSISTENCE_FAILED', persisted:eventResult };
      const recordResult = await persistence.saveRecord(nextRecord);
      if (!recordResult.ok) return { ok:false, reason:'INCIDENT_RECORD_PERSISTENCE_FAILED', persisted:recordResult };
      persisted = { ok:true };
    }

    if (!persisted.ok) return { ok:false, reason:'INCIDENT_ATOMIC_PERSISTENCE_FAILED', persisted };
    timeline = nextTimeline;
    record = nextRecord;
    return { ok:true, event, record:snapshot() };
  }

  async function initialize() {
    const existing = await persistence.loadRecord(incidentId);
    const storedEvents = typeof persistence.loadEvents === 'function' ? await persistence.loadEvents(incidentId) : { ok:true, events:[] };
    if (!existing.ok) return existing;
    if (!storedEvents.ok) return storedEvents;

    if (existing.record) {
      record = { ...record, ...clone(existing.record) };
      const recovered = mergeEvents(existing.record.timeline, storedEvents.events);
      timeline = createIncidentTimeline(incidentId, recovered);
      record = { ...record, timeline:timeline.snapshot() };
      if (recovered.length !== (existing.record.timeline ?? []).length) {
        const repaired = await persistence.saveRecord(record);
        if (!repaired.ok) return { ok:false, reason:'INCIDENT_TIMELINE_RECOVERY_PERSISTENCE_FAILED', persisted:repaired };
      }
      return { ok:true, restored:true, record:snapshot() };
    }

    if ((storedEvents.events ?? []).length > 0) {
      timeline = createIncidentTimeline(incidentId, storedEvents.events);
      record = { ...record, timeline:timeline.snapshot() };
      const repaired = await persistence.saveRecord(record);
      if (!repaired.ok) return { ok:false, reason:'INCIDENT_RECORD_RECOVERY_FAILED', persisted:repaired };
      return { ok:true, restored:true, recoveredFromJournal:true, record:snapshot() };
    }

    const opened = await append({ actor:IncidentActor.APPLICATION, type:IncidentEventType.INCIDENT_OPENED, payload:{ priority:record.priority }, provenance:'APPLICATION_INCIDENT_REQUEST' });
    return opened.ok ? { ok:true, restored:false, record:snapshot() } : opened;
  }

  async function sendCustomerText(text) {
    const value = String(text ?? '').trim();
    if (!value) return { ok:false, reason:'EMPTY_MESSAGE' };
    const safety = inspectIncidentText(value);
    if (!safety.ok) return safety;
    return append({ actor:IncidentActor.CUSTOMER, type:IncidentEventType.TEXT_MESSAGE, payload:{ text:value }, provenance:'CUSTOMER_TEXT' });
  }

  async function addFalconMessage(text, provenance = 'FALCON_CUSTOMER_MESSAGE') {
    const value = String(text ?? '').trim();
    if (!value) return { ok:false, reason:'EMPTY_MESSAGE' };
    const safety = inspectIncidentText(value);
    if (!safety.ok) return safety;
    return append({ actor:IncidentActor.FALCON, type:IncidentEventType.TEXT_MESSAGE, payload:{ text:value }, provenance });
  }

  async function requestSupport() {
    return append({ actor:IncidentActor.CUSTOMER, type:IncidentEventType.SUPPORT_REQUESTED, payload:{}, provenance:'CUSTOMER_SUPPORT_REQUEST', recordPatch:{ supportRequested:true } });
  }

  async function setSupportAvailable(available, displayName = null) {
    const patch = { supportAvailable:available === true, supportDisplayName:displayName ?? record.supportDisplayName };
    if (!available) return persistStandalone({ ...record, ...patch, timeline:timeline.snapshot() });
    return append({ actor:IncidentActor.SYSTEM, type:IncidentEventType.SUPPORT_AVAILABLE, payload:{ displayName:patch.supportDisplayName }, provenance:'WEB_SUPPORT_AVAILABILITY', recordPatch:patch });
  }

  async function startSupportTakeover({ principalId, displayName } = {}) {
    requiredString(principalId, 'principalId');
    requiredString(displayName, 'displayName');
    return append({
      actor:IncidentActor.SUPPORT,
      type:IncidentEventType.SUPPORT_TAKEOVER,
      payload:{ principalId, displayName },
      provenance:'AUTHORIZED_SUPPORT_TAKEOVER',
      recordPatch:{ mode:'SUPPORT_TAKEOVER', supportAvailable:true, supportDisplayName:displayName }
    });
  }

  async function releaseSupport() {
    return append({ actor:IncidentActor.SUPPORT, type:IncidentEventType.SUPPORT_RELEASE, payload:{}, provenance:'AUTHORIZED_SUPPORT_RELEASE', recordPatch:{ mode:'FALCON_ACTIVE' } });
  }

  async function setMinimized(minimized) {
    return persistStandalone({ ...record, minimized:minimized === true, timeline:timeline.snapshot() });
  }

  async function applyApplicationState(payload) {
    return append({ actor:IncidentActor.APPLICATION, type:IncidentEventType.INCIDENT_STATE_UPDATE, payload:clone(payload ?? {}), provenance:'APPLICATION_INCIDENT_STATE' });
  }

  async function applySimulatorUpdate(payload) {
    return append({ actor:IncidentActor.APPLICATION, type:IncidentEventType.SIMULATOR_UPDATE, payload:clone(payload ?? {}), provenance:'FSTSIMA_PROJECTION' });
  }

  async function resolveWithSummary(summary) {
    if (!summary || typeof summary !== 'object') return { ok:false, reason:'CLOSURE_SUMMARY_REQUIRED' };
    const requiredFields = ['problem','affectedItems','simulatorWindow','restoration','remainingFollowup'];
    for (const field of requiredFields) {
      if (!(field in summary)) return { ok:false, reason:`CLOSURE_SUMMARY_MISSING_${field.toUpperCase()}` };
    }

    // Persist the mandatory summary before the state can become RESOLVED. If the
    // final resolution commit fails, the incident remains open rather than
    // claiming closure without durable closure evidence.
    const summaryEvent = await append({
      actor:IncidentActor.FALCON,
      type:IncidentEventType.CLOSURE_SUMMARY,
      payload:clone(summary),
      provenance:'APPLICATION_SUPPLIED_CLOSURE_SUMMARY',
      recordPatch:{ closureSummary:clone(summary) }
    });
    if (!summaryEvent.ok) return summaryEvent;

    const resolvedAt = now();
    return append({
      actor:IncidentActor.APPLICATION,
      type:IncidentEventType.INCIDENT_RESOLVED,
      payload:{ resolvedAt },
      provenance:'APPLICATION_INCIDENT_RESOLUTION',
      recordPatch:{ status:'RESOLVED', resolved:true, resolvedAt, closureSummary:clone(summary) }
    });
  }

  function snapshot() {
    return Object.freeze(clone({ ...record, timeline:timeline.snapshot() }));
  }

  function currentTimeline() { return timeline; }

  return Object.freeze({ currentTimeline, initialize, snapshot, sendCustomerText, addFalconMessage, requestSupport, setSupportAvailable, startSupportTakeover, releaseSupport, setMinimized, applyApplicationState, applySimulatorUpdate, resolveWithSummary });
}
