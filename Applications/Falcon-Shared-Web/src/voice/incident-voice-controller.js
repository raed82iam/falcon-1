import { IncidentActor, IncidentEventType, createIncidentEvent } from '../incidents/incident-timeline.js';
import { inspectIncidentText, inspectVoiceTranscript } from '../incidents/incident-content-safety.js';
import { shouldFalconReplyAfterSilence } from './voice-policy.js';

export function createIncidentVoiceController({ incidentId, microphone, voiceRuntime, timeline, persistence = null, now = () => new Date().toISOString(), idFactory = () => crypto.randomUUID() } = {}) {
  if (!incidentId) throw new TypeError('incidentId is required');
  if (!microphone) throw new TypeError('microphone is required');
  if (!voiceRuntime) throw new TypeError('voiceRuntime is required');
  if (!timeline) throw new TypeError('timeline is required');

  async function persistEventThenAppend(event) {
    if (persistence) {
      const persisted = await persistence.appendEvent(event);
      if (!persisted.ok) return { ok:false, persisted };
    }
    timeline.append(event);
    return { ok:true };
  }

  async function startVoiceMessage(options = {}) { return microphone.start(options); }

  async function ingestRecordedCustomerVoice({ blob, mimeType = blob?.type ?? 'audio/webm', transcriptContext = {}, provenance = 'CUSTOMER_MICROPHONE_CAPTURE' } = {}) {
    if (!(blob instanceof Blob)) return Object.freeze({ ok:false, reason:'VOICE_BLOB_REQUIRED' });
    const transcription = await voiceRuntime.speechToText(blob, { incidentId, ...transcriptContext });
    if (!transcription.ok) return Object.freeze({ ok:false, reason:'VOICE_TRANSCRIPTION_REQUIRED_BEFORE_PERSISTENCE', transcription });

    const safety = inspectVoiceTranscript({ text:transcription.text, confidence:transcription.confidence });
    if (!safety.ok) return Object.freeze({ ok:false, reason:safety.reason, transcription, audioPersisted:false });

    const artifactId = idFactory();
    const voiceEvent = createIncidentEvent({ eventId:idFactory(), incidentId, timestamp:now(), actor:IncidentActor.CUSTOMER, type:IncidentEventType.VOICE_MESSAGE, payload:{ artifactId, mimeType }, provenance });
    const transcriptEvent = createIncidentEvent({
      eventId:idFactory(), incidentId, timestamp:now(), actor:IncidentActor.CUSTOMER,
      type:IncidentEventType.VOICE_TRANSCRIPT,
      payload:{ text:transcription.text, confidence:transcription.confidence ?? null, edited:false, provider:transcription.provider },
      provenance:'LOCAL_STT_TRANSCRIPT'
    });

    if (persistence) {
      if (typeof persistence.commitArtifactAndEvents !== 'function') return Object.freeze({ ok:false, reason:'ATOMIC_MEDIA_PERSISTENCE_UNAVAILABLE' });
      const committed = await persistence.commitArtifactAndEvents({
        artifact:{ artifactId, incidentId, kind:'CUSTOMER_VOICE', blob, metadata:{ mimeType, provider:transcription.provider ?? null } },
        events:[voiceEvent, transcriptEvent]
      });
      if (!committed.ok) return Object.freeze({ ok:false, reason:'VOICE_ATOMIC_PERSISTENCE_FAILED', committed });
    }

    timeline.append(voiceEvent);
    timeline.append(transcriptEvent);
    return Object.freeze({ ok:true, voiceEvent, transcription, transcriptEvent, artifactId });
  }

  async function stopVoiceMessage({ transcriptContext = {} } = {}) {
    const recorded = await microphone.stop();
    if (!recorded.ok) return recorded;
    return ingestRecordedCustomerVoice({ blob:recorded.blob, mimeType:recorded.mimeType, transcriptContext, provenance:'CUSTOMER_MICROPHONE_CAPTURE' });
  }

  async function speakFalcon(text, context = {}) {
    const safety = inspectIncidentText(text);
    if (!safety.ok) return Object.freeze({ ok:false, reason:safety.reason });
    const synthesized = await voiceRuntime.textToSpeech(text, { incidentId, ...context });
    if (!synthesized.ok) return synthesized;

    let artifactId = null;
    const hasAudio = synthesized.audioArtifact instanceof Blob;
    if (hasAudio) artifactId = idFactory();
    const event = createIncidentEvent({ eventId:idFactory(), incidentId, timestamp:now(), actor:IncidentActor.FALCON, type:IncidentEventType.FALCON_VOICE_REPLY, payload:{ text, artifactId, provider:synthesized.provider }, provenance:'LOCAL_TTS_RENDER' });

    if (persistence && hasAudio) {
      if (typeof persistence.commitArtifactAndEvents !== 'function') return Object.freeze({ ok:false, reason:'ATOMIC_MEDIA_PERSISTENCE_UNAVAILABLE' });
      const committed = await persistence.commitArtifactAndEvents({
        artifact:{ artifactId, incidentId, kind:'FALCON_VOICE', blob:synthesized.audioArtifact, metadata:{ provider:synthesized.provider ?? null } },
        events:[event]
      });
      if (!committed.ok) return Object.freeze({ ok:false, reason:'FALCON_VOICE_ATOMIC_PERSISTENCE_FAILED', committed });
      timeline.append(event);
    } else {
      const persisted = await persistEventThenAppend(event);
      if (!persisted.ok) return Object.freeze({ ok:false, reason:'FALCON_VOICE_EVENT_PERSISTENCE_FAILED', persisted });
    }
    return Object.freeze({ ok:true, event, synthesized, artifactId });
  }

  function liveTurnReady({ silenceStartedAt, currentTime = now(), customerExplicitlyEndedTurn = false } = {}) {
    return shouldFalconReplyAfterSilence({ silenceStartedAt, now:currentTime, customerExplicitlyEndedTurn });
  }

  return Object.freeze({ startVoiceMessage, stopVoiceMessage, ingestRecordedCustomerVoice, speakFalcon, liveTurnReady });
}
