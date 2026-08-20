import { VoicePolicy } from './voice-policy.js';

export function createLiveVoiceSession({
  mediaDevices = globalThis.navigator?.mediaDevices,
  AudioContextImpl = globalThis.AudioContext ?? globalThis.webkitAudioContext,
  MediaRecorderImpl = globalThis.MediaRecorder,
  requestAnimationFrameImpl = globalThis.requestAnimationFrame?.bind(globalThis),
  cancelAnimationFrameImpl = globalThis.cancelAnimationFrame?.bind(globalThis),
  now = () => performance.now(),
  speechThreshold = 0.025
} = {}) {
  let stream = null;
  let recorder = null;
  let audioContext = null;
  let analyser = null;
  let frameId = null;
  let chunks = [];
  let speechDetected = false;
  let silenceStartedAt = null;
  let active = false;
  let onTurnComplete = null;

  function levelFromAnalyser() {
    const data = new Uint8Array(analyser.fftSize);
    analyser.getByteTimeDomainData(data);
    let sumSquares = 0;
    for (const sample of data) {
      const normalized = (sample - 128) / 128;
      sumSquares += normalized * normalized;
    }
    return Math.sqrt(sumSquares / data.length);
  }

  function schedule() {
    if (!active || !requestAnimationFrameImpl || !analyser) return;
    frameId = requestAnimationFrameImpl(tick);
  }

  async function completeTurn(reason) {
    if (!active) return;
    active = false;
    if (frameId != null && cancelAnimationFrameImpl) cancelAnimationFrameImpl(frameId);
    frameId = null;
    const completion = await stopRecorder(reason);
    if (typeof onTurnComplete === 'function') onTurnComplete(completion);
  }

  function tick() {
    if (!active) return;
    const level = levelFromAnalyser();
    const current = now();
    if (level >= speechThreshold) {
      speechDetected = true;
      silenceStartedAt = null;
    } else if (speechDetected) {
      if (silenceStartedAt == null) silenceStartedAt = current;
      if (current - silenceStartedAt >= VoicePolicy.liveVoiceSilenceToleranceMs) {
        void completeTurn('FIFTEEN_SECOND_SILENCE_AFTER_SPEECH');
        return;
      }
    }
    schedule();
  }

  async function start({ turnComplete } = {}) {
    if (active) return { ok:false, reason:'LIVE_VOICE_ALREADY_ACTIVE' };
    if (!mediaDevices?.getUserMedia || !AudioContextImpl || !MediaRecorderImpl || !requestAnimationFrameImpl) {
      return { ok:false, reason:'LIVE_VOICE_BROWSER_CAPABILITY_UNAVAILABLE' };
    }
    try {
      stream = await mediaDevices.getUserMedia({ audio:true });
      audioContext = new AudioContextImpl();
      const source = audioContext.createMediaStreamSource(stream);
      analyser = audioContext.createAnalyser();
      analyser.fftSize = 512;
      source.connect(analyser);
      chunks = [];
      recorder = new MediaRecorderImpl(stream);
      recorder.addEventListener('dataavailable', event => { if (event?.data && event.data.size > 0) chunks.push(event.data); });
      recorder.start();
      speechDetected = false;
      silenceStartedAt = null;
      onTurnComplete = turnComplete ?? null;
      active = true;
      schedule();
      return { ok:true, state:'LISTENING', silenceToleranceMs:VoicePolicy.liveVoiceSilenceToleranceMs };
    } catch (error) {
      await teardown();
      return { ok:false, reason:error?.name === 'NotAllowedError' ? 'MIC_PERMISSION_DENIED' : 'LIVE_VOICE_START_FAILED' };
    }
  }

  function stopRecorder(reason) {
    if (!recorder || recorder.state !== 'recording') return Promise.resolve({ ok:false, reason:'LIVE_VOICE_NOT_RECORDING' });
    return new Promise(resolve => {
      recorder.addEventListener('stop', async () => {
        const mimeType = recorder.mimeType || chunks[0]?.type || 'audio/webm';
        const blob = new Blob(chunks, { type:mimeType });
        await teardown();
        resolve({ ok:true, state:'TURN_COMPLETE', reason, blob, mimeType, speechDetected });
      }, { once:true });
      recorder.stop();
    });
  }

  async function endTurnExplicitly() {
    if (!active) return { ok:false, reason:'LIVE_VOICE_NOT_ACTIVE' };
    active = false;
    if (frameId != null && cancelAnimationFrameImpl) cancelAnimationFrameImpl(frameId);
    frameId = null;
    const completion = await stopRecorder('CUSTOMER_EXPLICIT_TURN_END');
    return completion;
  }

  async function stopSession() {
    active = false;
    if (frameId != null && cancelAnimationFrameImpl) cancelAnimationFrameImpl(frameId);
    frameId = null;
    if (recorder?.state === 'recording') recorder.stop();
    await teardown();
    return { ok:true, state:'STOPPED' };
  }

  async function teardown() {
    stream?.getTracks?.().forEach(track => track.stop());
    try { await audioContext?.close?.(); } catch {}
    stream = null;
    recorder = null;
    audioContext = null;
    analyser = null;
    chunks = [];
    speechDetected = false;
    silenceStartedAt = null;
  }

  function state() { return active ? 'LISTENING' : 'IDLE'; }
  return Object.freeze({ start, endTurnExplicitly, stopSession, state });
}
