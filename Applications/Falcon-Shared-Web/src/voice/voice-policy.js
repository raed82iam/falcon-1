export const VoiceProvider = Object.freeze({
  SPEECH_TO_TEXT:'WHISPER_CPP_LOCAL',
  TEXT_TO_SPEECH:'PIPER_LOCAL'
});

export const VoicePolicy = Object.freeze({
  voiceMessageSilenceAutoStop:false,
  liveVoiceSilenceToleranceMs:15_000,
  explicitStopRequired:true,
  liveVoiceExplicitOptInRequired:true,
  hiddenMicrophoneActivationAllowed:false,
  paidRemoteApiAllowed:false,
  localOnlyPreferred:true
});

export function shouldFalconReplyAfterSilence({ silenceStartedAt, now, customerExplicitlyEndedTurn=false } = {}) {
  if (customerExplicitlyEndedTurn === true) return true;
  if (!silenceStartedAt || !now) return false;
  const elapsed = new Date(now).getTime() - new Date(silenceStartedAt).getTime();
  return Number.isFinite(elapsed) && elapsed >= VoicePolicy.liveVoiceSilenceToleranceMs;
}

export function voiceMessageShouldAutoStop() {
  return VoicePolicy.voiceMessageSilenceAutoStop;
}

export function localVoiceProviderPlan() {
  return Object.freeze({
    speechToText: VoiceProvider.SPEECH_TO_TEXT,
    textToSpeech: VoiceProvider.TEXT_TO_SPEECH,
    paidRemoteApiAllowed:false,
    routeBindingState:'GOVERNED_LOCAL_RUNTIME_BINDING_REQUIRED',
    secretRequiredInBrowser:false
  });
}
