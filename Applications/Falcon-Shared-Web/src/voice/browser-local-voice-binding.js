import { createLocalVoiceRuntime } from './local-voice-runtime.js';

/**
 * Explicit binding contract for locally hosted/in-browser Whisper.cpp + Piper.
 *
 * Runtime capability must be injected by the governed Web composition layer.
 * No global-object discovery, remote API URL, credential, fetch fallback or paid
 * voice service is permitted here. Missing runtime capability remains fail-closed.
 */
export function createBrowserLocalVoiceBinding({ runtime = null } = {}) {
  const transcribe = runtime && typeof runtime.transcribeWithWhisperCpp === 'function'
    ? (audioArtifact, context) => runtime.transcribeWithWhisperCpp(audioArtifact, context)
    : undefined;
  const synthesize = runtime && typeof runtime.synthesizeWithPiper === 'function'
    ? (text, context) => runtime.synthesizeWithPiper(text, context)
    : undefined;

  const voiceRuntime = createLocalVoiceRuntime({ transcribe, synthesize });

  function readiness() {
    return Object.freeze({
      speechToText:typeof transcribe === 'function' ? 'READY' : 'UNAVAILABLE',
      textToSpeech:typeof synthesize === 'function' ? 'READY' : 'UNAVAILABLE',
      remotePaidApiUsed:false,
      speechToTextProvider:'WHISPER_CPP_LOCAL',
      textToSpeechProvider:'PIPER_LOCAL',
      bindingMode:'EXPLICIT_COMPOSITION_ONLY'
    });
  }

  return Object.freeze({ voiceRuntime, readiness });
}
