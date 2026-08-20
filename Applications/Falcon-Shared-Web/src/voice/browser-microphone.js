import { VoicePolicy } from './voice-policy.js';

export function createBrowserMicrophone({ mediaDevices = globalThis.navigator?.mediaDevices, MediaRecorderImpl = globalThis.MediaRecorder } = {}) {
  let stream = null;
  let recorder = null;
  let chunks = [];

  async function start({ mimeType } = {}) {
    if (!mediaDevices?.getUserMedia) return { ok:false, reason:'MICROPHONE_UNAVAILABLE' };
    if (!MediaRecorderImpl) return { ok:false, reason:'MEDIA_RECORDER_UNAVAILABLE' };
    if (recorder?.state === 'recording') return { ok:false, reason:'ALREADY_RECORDING' };

    try {
      stream = await mediaDevices.getUserMedia({ audio:true });
      chunks = [];
      const options = mimeType ? { mimeType } : undefined;
      recorder = new MediaRecorderImpl(stream, options);
      recorder.addEventListener('dataavailable', event => {
        if (event?.data && event.data.size > 0) chunks.push(event.data);
      });
      recorder.start();
      return { ok:true, state:'RECORDING', silenceAutoStop:VoicePolicy.voiceMessageSilenceAutoStop };
    } catch (error) {
      return { ok:false, reason:error?.name === 'NotAllowedError' ? 'MIC_PERMISSION_DENIED' : 'MIC_START_FAILED' };
    }
  }

  async function stop() {
    if (!recorder || recorder.state !== 'recording') return { ok:false, reason:'NOT_RECORDING' };

    return await new Promise(resolve => {
      recorder.addEventListener('stop', () => {
        const type = recorder.mimeType || chunks[0]?.type || 'audio/webm';
        const blob = new Blob(chunks, { type });
        stream?.getTracks?.().forEach(track => track.stop());
        stream = null;
        recorder = null;
        chunks = [];
        resolve({ ok:true, state:'STOPPED', blob, mimeType:type });
      }, { once:true });
      recorder.stop();
    });
  }

  function cancel() {
    if (recorder?.state === 'recording') recorder.stop();
    stream?.getTracks?.().forEach(track => track.stop());
    stream = null;
    recorder = null;
    chunks = [];
    return { ok:true, state:'CANCELLED' };
  }

  function state() {
    return recorder?.state === 'recording' ? 'RECORDING' : 'IDLE';
  }

  return Object.freeze({ start, stop, cancel, state });
}
