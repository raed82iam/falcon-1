function required(value, name) {
  if (!value) throw new TypeError(`${name} is required`);
  return value;
}

function freezeCopy(value) {
  if (value == null || typeof value !== 'object') return value;
  if (value instanceof Blob) return value;
  if (Array.isArray(value)) return Object.freeze(value.map(freezeCopy));
  const out = {};
  for (const [key,item] of Object.entries(value)) out[key] = freezeCopy(item);
  return Object.freeze(out);
}

export function createIncidentPersistencePort({ saveRecord, loadRecord, appendEvent, putArtifact, getArtifact, loadEvents = null, commitRecordAndEvent = null, commitArtifactAndEvents = null } = {}) {
  for (const [name,fn] of Object.entries({ saveRecord, loadRecord, appendEvent, putArtifact, getArtifact })) {
    if (typeof fn !== 'function') throw new TypeError(`${name} must be a function`);
  }
  for (const [name,fn] of Object.entries({ loadEvents, commitRecordAndEvent, commitArtifactAndEvents })) {
    if (fn !== null && typeof fn !== 'function') throw new TypeError(`${name} must be a function`);
  }
  return Object.freeze({ saveRecord, loadRecord, appendEvent, putArtifact, getArtifact, loadEvents, commitRecordAndEvent, commitArtifactAndEvents });
}

export function createIndexedDbIncidentPersistence({ indexedDBImpl = globalThis.indexedDB, databaseName = 'falcon-shared-web-incidents-v1' } = {}) {
  if (!indexedDBImpl) {
    const unavailable = async () => ({ ok:false, reason:'INDEXEDDB_UNAVAILABLE' });
    return createIncidentPersistencePort({ saveRecord:unavailable, loadRecord:unavailable, appendEvent:unavailable, putArtifact:unavailable, getArtifact:unavailable, loadEvents:unavailable, commitRecordAndEvent:unavailable, commitArtifactAndEvents:unavailable });
  }

  let openPromise = null;
  function open() {
    if (openPromise) return openPromise;
    openPromise = new Promise((resolve,reject) => {
      const request = indexedDBImpl.open(databaseName, 1);
      request.addEventListener('upgradeneeded', () => {
        const db = request.result;
        if (!db.objectStoreNames.contains('records')) db.createObjectStore('records', { keyPath:'incidentId' });
        if (!db.objectStoreNames.contains('events')) {
          const store = db.createObjectStore('events', { keyPath:'eventId' });
          store.createIndex('incidentId', 'incidentId', { unique:false });
        }
        if (!db.objectStoreNames.contains('artifacts')) db.createObjectStore('artifacts', { keyPath:'artifactId' });
      });
      request.addEventListener('success', () => resolve(request.result));
      request.addEventListener('error', () => reject(request.error ?? new Error('INDEXEDDB_OPEN_FAILED')));
    });
    return openPromise;
  }

  async function transact(storeName, mode, operation) {
    try {
      const db = await open();
      return await new Promise((resolve,reject) => {
        const tx = db.transaction(storeName, mode);
        const store = tx.objectStore(storeName);
        const request = operation(store);
        request.addEventListener('success', () => resolve({ ok:true, value:request.result ?? null }));
        request.addEventListener('error', () => reject(request.error ?? new Error('INDEXEDDB_REQUEST_FAILED')));
        tx.addEventListener('abort', () => reject(tx.error ?? new Error('INDEXEDDB_TRANSACTION_ABORTED')));
      });
    } catch (error) {
      return { ok:false, reason:'INCIDENT_PERSISTENCE_FAILED', errorName:error?.name ?? 'Error' };
    }
  }

  async function saveRecord(record) {
    required(record?.incidentId, 'record.incidentId');
    const value = { ...record, persistedAt:new Date().toISOString() };
    const result = await transact('records','readwrite',store=>store.put(value));
    return result.ok ? { ok:true, record:freezeCopy(value) } : result;
  }

  async function loadRecord(incidentId) {
    required(incidentId, 'incidentId');
    const result = await transact('records','readonly',store=>store.get(incidentId));
    return result.ok ? { ok:true, record:result.value ? freezeCopy(result.value) : null } : result;
  }

  async function appendEvent(event) {
    required(event?.eventId, 'event.eventId');
    required(event?.incidentId, 'event.incidentId');
    const value = { ...event, persistedAt:new Date().toISOString() };
    const result = await transact('events','readwrite',store=>store.put(value));
    return result.ok ? { ok:true, event:freezeCopy(value) } : result;
  }

  async function loadEvents(incidentId) {
    required(incidentId, 'incidentId');
    try {
      const db = await open();
      return await new Promise((resolve,reject) => {
        const tx = db.transaction('events','readonly');
        const index = tx.objectStore('events').index('incidentId');
        const request = index.getAll(incidentId);
        request.addEventListener('success', () => {
          const events = (request.result ?? []).map(({ persistedAt, ...event }) => event).sort((a,b) => new Date(a.timestamp).getTime() - new Date(b.timestamp).getTime());
          resolve({ ok:true, events:freezeCopy(events) });
        });
        request.addEventListener('error', () => reject(request.error ?? new Error('INDEXEDDB_EVENT_READ_FAILED')));
        tx.addEventListener('abort', () => reject(tx.error ?? new Error('INDEXEDDB_TRANSACTION_ABORTED')));
      });
    } catch (error) {
      return { ok:false, reason:'INCIDENT_PERSISTENCE_FAILED', errorName:error?.name ?? 'Error' };
    }
  }

  async function commitRecordAndEvent(record, event) {
    required(record?.incidentId, 'record.incidentId');
    required(event?.eventId, 'event.eventId');
    required(event?.incidentId, 'event.incidentId');
    if (record.incidentId !== event.incidentId) throw new TypeError('record/event incidentId mismatch');
    try {
      const db = await open();
      const persistedAt = new Date().toISOString();
      const recordValue = { ...record, persistedAt };
      const eventValue = { ...event, persistedAt };
      return await new Promise((resolve,reject) => {
        const tx = db.transaction(['records','events'],'readwrite');
        tx.objectStore('records').put(recordValue);
        tx.objectStore('events').put(eventValue);
        tx.addEventListener('complete', () => resolve({ ok:true, record:freezeCopy(recordValue), event:freezeCopy(eventValue) }));
        tx.addEventListener('abort', () => reject(tx.error ?? new Error('INDEXEDDB_TRANSACTION_ABORTED')));
        tx.addEventListener('error', () => reject(tx.error ?? new Error('INDEXEDDB_TRANSACTION_FAILED')));
      });
    } catch (error) {
      return { ok:false, reason:'INCIDENT_PERSISTENCE_FAILED', errorName:error?.name ?? 'Error' };
    }
  }

  async function commitArtifactAndEvents({ artifact, events } = {}) {
    required(artifact?.artifactId, 'artifact.artifactId');
    required(artifact?.incidentId, 'artifact.incidentId');
    required(artifact?.kind, 'artifact.kind');
    if (!(artifact.blob instanceof Blob)) throw new TypeError('artifact.blob must be a Blob');
    if (!Array.isArray(events) || events.length < 1) throw new TypeError('events must contain at least one event');
    for (const event of events) {
      required(event?.eventId, 'event.eventId');
      required(event?.incidentId, 'event.incidentId');
      if (event.incidentId !== artifact.incidentId) throw new TypeError('artifact/event incidentId mismatch');
    }
    try {
      const db = await open();
      const persistedAt = new Date().toISOString();
      const artifactValue = { ...artifact, metadata:{ ...(artifact.metadata ?? {}) }, persistedAt };
      const eventValues = events.map(event => ({ ...event, persistedAt }));
      return await new Promise((resolve,reject) => {
        const tx = db.transaction(['artifacts','events'],'readwrite');
        tx.objectStore('artifacts').put(artifactValue);
        const eventStore = tx.objectStore('events');
        for (const event of eventValues) eventStore.put(event);
        tx.addEventListener('complete', () => resolve({ ok:true, artifactId:artifact.artifactId, events:freezeCopy(eventValues) }));
        tx.addEventListener('abort', () => reject(tx.error ?? new Error('INDEXEDDB_TRANSACTION_ABORTED')));
        tx.addEventListener('error', () => reject(tx.error ?? new Error('INDEXEDDB_TRANSACTION_FAILED')));
      });
    } catch (error) {
      return { ok:false, reason:'INCIDENT_PERSISTENCE_FAILED', errorName:error?.name ?? 'Error' };
    }
  }

  async function putArtifact({ artifactId, incidentId, kind, blob, metadata = {} } = {}) {
    required(artifactId, 'artifactId');
    required(incidentId, 'incidentId');
    required(kind, 'kind');
    if (!(blob instanceof Blob)) throw new TypeError('blob must be a Blob');
    const value = { artifactId, incidentId, kind, blob, metadata:{ ...metadata }, persistedAt:new Date().toISOString() };
    const result = await transact('artifacts','readwrite',store=>store.put(value));
    return result.ok ? { ok:true, artifactId } : result;
  }

  async function getArtifact(artifactId) {
    required(artifactId, 'artifactId');
    const result = await transact('artifacts','readonly',store=>store.get(artifactId));
    return result.ok ? { ok:true, artifact:result.value ? freezeCopy(result.value) : null } : result;
  }

  return createIncidentPersistencePort({ saveRecord, loadRecord, appendEvent, putArtifact, getArtifact, loadEvents, commitRecordAndEvent, commitArtifactAndEvents });
}
