import test from 'node:test';
import assert from 'node:assert/strict';
import { createIncidentTimeline, IncidentEventType } from '../src/incidents/incident-timeline.js';
import { createIncidentVoiceController } from '../src/voice/incident-voice-controller.js';

function ids() {
  let n = 0;
  return () => `e${++n}`;
}

test('voice controller appends voice and transcript to the same incident timeline', async () => {
  const timeline = createIncidentTimeline('INC-VOICE-1');
  const mic = {
    start:async () => ({ ok:true, state:'RECORDING', silenceAutoStop:false }),
    stop:async () => ({ ok:true, state:'STOPPED', blob:new Blob(['audio']), mimeType:'audio/webm' })
  };
  const runtime = {
    speechToText:async () => ({ ok:true, text:'فتح معي', confidence:0.99, provider:'WHISPER_CPP_LOCAL' }),
    textToSpeech:async text => ({ ok:true, audioArtifact:new Blob([text]), provider:'PIPER_LOCAL' })
  };
  const controller = createIncidentVoiceController({ incidentId:'INC-VOICE-1', microphone:mic, voiceRuntime:runtime, timeline, now:() => '2026-08-16T00:00:00.000Z', idFactory:ids() });

  assert.equal((await controller.startVoiceMessage()).silenceAutoStop, false);
  const result = await controller.stopVoiceMessage();
  assert.equal(result.ok, true);
  assert.deepEqual(timeline.snapshot().map(x => x.type), [IncidentEventType.VOICE_MESSAGE, IncidentEventType.VOICE_TRANSCRIPT]);
});

test('voice controller uses 15 second patience rule for live guidance', () => {
  const timeline = createIncidentTimeline('INC-VOICE-2');
  const controller = createIncidentVoiceController({
    incidentId:'INC-VOICE-2',
    microphone:{},
    voiceRuntime:{},
    timeline,
    idFactory:ids()
  });
  assert.equal(controller.liveTurnReady({ silenceStartedAt:'2026-08-16T00:00:00.000Z', currentTime:'2026-08-16T00:00:14.999Z' }), false);
  assert.equal(controller.liveTurnReady({ silenceStartedAt:'2026-08-16T00:00:00.000Z', currentTime:'2026-08-16T00:00:15.000Z' }), true);
});
