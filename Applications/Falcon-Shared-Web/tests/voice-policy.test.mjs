import test from 'node:test';
import assert from 'node:assert/strict';
import { VoicePolicy, shouldFalconReplyAfterSilence, voiceMessageShouldAutoStop, localVoiceProviderPlan } from '../src/voice/voice-policy.js';

 test('ordinary voice message never auto-stops on silence', () => {
  assert.equal(voiceMessageShouldAutoStop(), false);
  assert.equal(VoicePolicy.explicitStopRequired, true);
});

test('live voice waits through less than 15 seconds of silence', () => {
  assert.equal(shouldFalconReplyAfterSilence({
    silenceStartedAt:'2026-08-16T00:00:00.000Z',
    now:'2026-08-16T00:00:14.999Z'
  }), false);
});

test('live voice may reply after 15 seconds of silence', () => {
  assert.equal(shouldFalconReplyAfterSilence({
    silenceStartedAt:'2026-08-16T00:00:00.000Z',
    now:'2026-08-16T00:00:15.000Z'
  }), true);
});

test('explicit customer end can finish turn earlier', () => {
  assert.equal(shouldFalconReplyAfterSilence({ customerExplicitlyEndedTurn:true }), true);
});

test('voice provider plan is free local-only', () => {
  const plan = localVoiceProviderPlan();
  assert.equal(plan.speechToText, 'WHISPER_CPP_LOCAL');
  assert.equal(plan.textToSpeech, 'PIPER_LOCAL');
  assert.equal(plan.paidRemoteApiAllowed, false);
  assert.equal(plan.secretRequiredInBrowser, false);
});
