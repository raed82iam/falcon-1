import test from 'node:test';
import assert from 'node:assert/strict';
import { createIncidentTimeline, IncidentActor, IncidentEventType } from '../src/incidents/incident-timeline.js';

test('incident timeline preserves mixed voice and text chronology', () => {
  const timeline = createIncidentTimeline('INC-1');
  timeline.append({ eventId:'e2', timestamp:'2026-08-16T00:00:02.000Z', actor:IncidentActor.CUSTOMER, type:IncidentEventType.VOICE_MESSAGE, payload:{ durationMs:8000 } });
  timeline.append({ eventId:'e1', timestamp:'2026-08-16T00:00:01.000Z', actor:IncidentActor.FALCON, type:IncidentEventType.TEXT_MESSAGE, payload:{ text:'مرحبا' } });
  const events = timeline.snapshot();
  assert.deepEqual(events.map(x => x.eventId), ['e1','e2']);
  assert.equal(events[1].type, IncidentEventType.VOICE_MESSAGE);
});

test('timeline rejects invalid actor and event type', () => {
  const timeline = createIncidentTimeline('INC-2');
  assert.throws(() => timeline.append({ eventId:'e1', timestamp:'2026-08-16T00:00:00.000Z', actor:'UNKNOWN', type:IncidentEventType.TEXT_MESSAGE }), /invalid actor/);
  assert.throws(() => timeline.append({ eventId:'e2', timestamp:'2026-08-16T00:00:00.000Z', actor:IncidentActor.CUSTOMER, type:'UNKNOWN' }), /invalid event type/);
});
