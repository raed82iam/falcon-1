import { localVoiceProviderPlan } from './voice-policy.js';

export function createLocalVoiceRuntime({ transcribe, synthesize } = {}) {
  const plan = localVoiceProviderPlan();

  async function speechToText(audioArtifact, context = {}) {
    if (typeof transcribe !== 'function') {
      return Object.freeze({ ok:false, reason:'LOCAL_WHISPER_CPP_BINDING_UNAVAILABLE', provider:plan.speechToText });
    }
    const result = await transcribe(audioArtifact, context);
    return Object.freeze({ ...result, provider:plan.speechToText, remotePaidApiUsed:false });
  }

  async function textToSpeech(text, context = {}) {
    if (typeof synthesize !== 'function') {
      return Object.freeze({ ok:false, reason:'LOCAL_PIPER_BINDING_UNAVAILABLE', provider:plan.textToSpeech });
    }
    const result = await synthesize(text, context);
    return Object.freeze({ ...result, provider:plan.textToSpeech, remotePaidApiUsed:false });
  }

  return Object.freeze({ plan, speechToText, textToSpeech });
}
