import test from 'node:test';
import assert from 'node:assert/strict';
import { createIncidentController } from '../src/incidents/incident-controller.js';

function ids() { let n = 0; return () => `event-${++n}`; }
function basePersistence(overrides = {}) {
  return {
    loadRecord: async () => ({ ok: true, record: null }),
    loadEvents: async () => ({ ok: true, events: [] }),
    saveRecord: async record => ({ ok: true, record }),
    appendEvent: async event => ({ ok: true, event }),
    commitRecordAndEvent: async (record, event) => ({ ok: true, record, event }),
    ...overrides
  };
}

test('failed durable support takeover leaves Falcon mode active', async () => {
  let commits = 0;
  const persistence = basePersistence({
    commitRecordAndEvent: async (record, event) => {
      commits += 1;
      if (commits === 1) return { ok: true, record, event };
      return { ok: false, reason: 'PERSISTENCE_FAILURE' };
    }
  });
  const controller = createIncidentController({ incidentId: 'INC-DUR-1', persistence, now: () => '2026-08-16T06:00:00Z', idFactory: ids() });
  assert.equal((await controller.initialize()).ok, true);
  const result = await controller.startSupportTakeover({ principalId: 'support-1', displayName: 'Support One' });
  assert.equal(result.ok, false);
  assert.equal(controller.snapshot().mode, 'FALCON_ACTIVE');
});

test('incident remains open when durable resolution commit fails', async () => {
  let commits = 0;
  const persistence = basePersistence({
    commitRecordAndEvent: async (record, event) => {
      commits += 1;
      if (commits === 3) return { ok: false, reason: 'PERSISTENCE_FAILURE' };
      return { ok: true, record, event };
    }
  });
  const controller = createIncidentController({ incidentId: 'INC-DUR-2', persistence, now: () => '2026-08-16T06:00:00Z', idFactory: ids() });
  await controller.initialize();
  const result = await controller.resolveWithSummary({ problem: 'Connectivity issue', affectedItems: [], simulatorWindow: { from: null, to: null }, restoration: 'Recovered', remainingFollowup: 'None' });
  assert.equal(result.ok, false);
  assert.equal(controller.snapshot().resolved, false);
  assert.equal(controller.snapshot().status, 'OPEN');
  assert.ok(controller.snapshot().closureSummary);
});

test('initialize repairs record timeline from durable event journal', async () => {
  const journalEvent = { eventId: 'journal-1', incidentId: 'INC-DUR-3', timestamp: '2026-08-16T06:01:00Z', actor: 'CUSTOMER', type: 'TEXT_MESSAGE', payload: { text: 'hello' }, provenance: 'CUSTOMER_TEXT' };
  let repairedRecord = null;
  const persistence = basePersistence({
    loadRecord: async () => ({ ok: true, record: { incidentId: 'INC-DUR-3', status: 'OPEN', timeline: [] } }),
    loadEvents: async () => ({ ok: true, events: [journalEvent] }),
    saveRecord: async record => { repairedRecord = record; return { ok: true, record }; }
  });
  const controller = createIncidentController({ incidentId: 'INC-DUR-3', persistence, idFactory: ids() });
  const result = await controller.initialize();
  assert.equal(result.ok, true);
  assert.equal(result.restored, true);
  assert.equal(controller.snapshot().timeline.length, 1);
  assert.equal(controller.snapshot().timeline[0].eventId, 'journal-1');
  assert.equal(repairedRecord.timeline.length, 1);
});
