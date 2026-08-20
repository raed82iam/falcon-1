import test from 'node:test';
import assert from 'node:assert/strict';
import { createBrowserMicrophone } from '../src/voice/browser-microphone.js';

class FakeRecorder {
  constructor(stream) {
    this.stream = stream;
    this.state = 'inactive';
    this.mimeType = 'audio/webm';
    this.listeners = new Map();
  }
  addEventListener(name, fn) { this.listeners.set(name, fn); }
  start() { this.state = 'recording'; }
  stop() {
    this.listeners.get('dataavailable')?.({ data:new Blob(['voice'], { type:'audio/webm' }) });
    this.state = 'inactive';
    this.listeners.get('stop')?.();
  }
}

test('browser microphone starts only after explicit request and reports no silence auto-stop', async () => {
  let requested = 0;
  let trackStopped = false;
  const mediaDevices = { getUserMedia:async constraints => {
    requested += 1;
    assert.deepEqual(constraints, { audio:true });
    return { getTracks:() => [{ stop:() => { trackStopped = true; } }] };
  }};
  const mic = createBrowserMicrophone({ mediaDevices, MediaRecorderImpl:FakeRecorder });
  assert.equal(mic.state(), 'IDLE');
  const started = await mic.start();
  assert.equal(requested, 1);
  assert.equal(started.ok, true);
  assert.equal(started.silenceAutoStop, false);
  const stopped = await mic.stop();
  assert.equal(stopped.ok, true);
  assert.equal(stopped.blob.size > 0, true);
  assert.equal(trackStopped, true);
});

test('permission denial fails closed without recording', async () => {
  const error = new Error('denied'); error.name = 'NotAllowedError';
  const mic = createBrowserMicrophone({ mediaDevices:{ getUserMedia:async () => { throw error; } }, MediaRecorderImpl:FakeRecorder });
  assert.deepEqual(await mic.start(), { ok:false, reason:'MIC_PERMISSION_DENIED' });
});
