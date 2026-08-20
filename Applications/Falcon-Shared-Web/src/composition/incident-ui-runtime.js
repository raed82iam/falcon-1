import { createCustomerIncidentFeature } from '../features/incidents/customer-incident.js';
import {
  adaptAffectedPositionProjection,
  adaptAffectedOrderProjection,
  adaptEmergencyShadowProjection
} from '../adapters/fsats-incident-followup-v1.js';
import { createBrowserMicrophone } from '../voice/browser-microphone.js';
import { createBrowserLocalVoiceBinding } from '../voice/browser-local-voice-binding.js';
import { createIncidentVoiceController } from '../voice/incident-voice-controller.js';
import { createLiveVoiceSession } from '../voice/live-voice-session.js';
import { createIndexedDbIncidentPersistence } from '../incidents/incident-persistence.js';
import { createIncidentController } from '../incidents/incident-controller.js';
import { createIncidentScreenshotUploadController } from '../incidents/screenshot-upload-controller.js';
import { createUnavailableIncidentSupportTransportPort, validateSupportTransportDecision } from '../core/ports/incident-support-transport-port.js';

function requireFunction(value, name) {
  if (typeof value !== 'function') throw new TypeError(`${name} must be a function`);
  return value;
}

function safeAdapt(collection, adapter) {
  if (!Array.isArray(collection)) return [];
  return collection.flatMap(item => {
    try { return [adapter(item)]; } catch { return []; }
  });
}

/** Web-owned customer incident UI/runtime composition. */
export function createIncidentUiRuntime({
  data,
  language,
  localize,
  render,
  session,
  documentRef = globalThis.document,
  urlApi = globalThis.URL,
  AudioCtor = globalThis.Audio,
  BlobCtor = globalThis.Blob,
  persistencePort = null,
  indexedDBImpl = globalThis.indexedDB,
  screenshotScanner = null,
  localVoiceRuntime = null,
  supportTransportPort = null
} = {}) {
  if (!data || typeof data !== 'object') throw new TypeError('data is required');
  const currentLanguage = requireFunction(language, 'language');
  const local = requireFunction(localize, 'localize');
  const requestRender = requireFunction(render, 'render');
  const currentSession = requireFunction(session, 'session');

  const { incidentSurface } = createCustomerIncidentFeature({ language:currentLanguage });
  const microphone = createBrowserMicrophone();
  const liveVoiceSession = createLiveVoiceSession();
  const persistence = persistencePort ?? createIndexedDbIncidentPersistence({ indexedDBImpl });
  const localVoice = createBrowserLocalVoiceBinding({ runtime:localVoiceRuntime });
  const supportTransport = supportTransportPort ?? createUnavailableIncidentSupportTransportPort();

  let recording = false;
  let summaryDismissed = false;
  let incidentController = null;
  let incidentVoiceController = null;
  let screenshotController = null;

  function conversation() {
    const current = incidentController?.snapshot?.() ?? data.incidentConversation ?? null;
    if (!current) return null;
    if (summaryDismissed && current.resolved === true) return null;
    return current;
  }

  function markup() {
    const current = conversation();
    if (!current) return '';
    return incidentSurface({
      conversation:current,
      affectedPositions:safeAdapt(data.incidentAffectedPositions, adaptAffectedPositionProjection),
      affectedOrders:safeAdapt(data.incidentAffectedOrders, adaptAffectedOrderProjection),
      shadowMonitoring:safeAdapt(data.incidentShadowMonitoring, adaptEmergencyShadowProjection),
      voiceReadiness:localVoice.readiness()
    });
  }

  function setStatus(message) {
    const note = documentRef?.querySelector?.('.incident-security-note');
    if (note && message) note.textContent = message;
  }

  async function refresh() {
    requestRender();
    await Promise.resolve();
    const timeline = documentRef?.querySelector?.('.incident-timeline');
    timeline?.scrollTo?.({ top:timeline?.scrollHeight ?? 0, behavior:'smooth' });
  }

  async function initialize() {
    const initial = data.incidentConversation;
    if (!initial?.incidentId) return Object.freeze({ ok:false, reason:'NO_INCIDENT' });

    incidentController = createIncidentController({ incidentId:initial.incidentId, persistence, initialRecord:initial });
    const initialized = await incidentController.initialize();
    if (!initialized.ok) return initialized;

    incidentVoiceController = createIncidentVoiceController({
      incidentId:initial.incidentId,
      microphone,
      voiceRuntime:localVoice.voiceRuntime,
      timeline:incidentController.currentTimeline(),
      persistence
    });
    screenshotController = createIncidentScreenshotUploadController({
      incidentId:initial.incidentId,
      timeline:incidentController.currentTimeline(),
      persistence,
      scanner:screenshotScanner
    });
    requestRender();
    return Object.freeze({ ok:true });
  }

  function bindActions() {
    documentRef?.querySelector?.('[data-incident-minimize]')?.addEventListener('click', async () => {
      microphone.cancel();
      await liveVoiceSession.stopSession();
      recording = false;
      await incidentController?.setMinimized(true);
      requestRender();
    });
    documentRef?.querySelector?.('[data-incident-expand]')?.addEventListener('click', async () => {
      await incidentController?.setMinimized(false);
      requestRender();
    });
    documentRef?.querySelector?.('[data-incident-dismiss-summary]')?.addEventListener('click', () => {
      summaryDismissed = true;
      requestRender();
    });

    documentRef?.querySelector?.('[data-incident-send]')?.addEventListener('click', async () => {
      const input = documentRef?.querySelector?.('[data-incident-text]');
      const result = await incidentController?.sendCustomerText(input?.value ?? '');
      if (!result?.ok) {
        setStatus(result?.reason === 'SECRET_DETECTED'
          ? local('رفضت الرسالة لأنها تبدو أنها تحتوي بيانات سرية.','Message rejected because it appears to contain a secret.')
          : local('تعذر حفظ الرسالة.','Could not persist the message.'));
        return;
      }
      if (input) input.value = '';
      await refresh();
    });
    documentRef?.querySelector?.('[data-incident-text]')?.addEventListener('keydown', event => {
      if (event.key === 'Enter' && !event.shiftKey) {
        event.preventDefault();
        documentRef?.querySelector?.('[data-incident-send]')?.click();
      }
    });

    documentRef?.querySelector?.('[data-support-request]')?.addEventListener('click', async () => {
      const activeSession=currentSession();
      const current=conversation();
      if (!current?.incidentId || !activeSession?.authoritativeSession || !activeSession?.principalId || !activeSession?.sessionId) {
        setStatus(local('تعذر إرسال طلب الدعم لأن جلسة Falcon الموثوقة غير متاحة.','Support request cannot be sent because an authoritative Falcon session is unavailable.'));
        return;
      }
      const request=Object.freeze({ incidentId:current.incidentId, principalId:activeSession.principalId, sessionId:activeSession.sessionId });
      const rawDecision=await supportTransport.requestSupport(request);
      const decision=validateSupportTransportDecision(rawDecision,request);
      if (!decision.accepted) {
        setStatus(local('لم يتم إرسال طلب الدعم. قناة الدعم المصرح بها غير متاحة حاليًا.','Support request was not delivered. The governed Support transport is currently unavailable.'));
        return;
      }
      const result = await incidentController?.requestSupport();
      if (result?.ok) await refresh();
    });

    documentRef?.querySelector?.('[data-incident-voice-start]')?.addEventListener('click', async () => {
      const result = await incidentVoiceController?.startVoiceMessage();
      if (!result?.ok) {
        setStatus(local('تعذر تشغيل المايك أو Whisper.cpp المحلي غير جاهز.','Microphone or local Whisper.cpp is not ready.'));
        return;
      }
      recording = true;
      const start = documentRef?.querySelector?.('[data-incident-voice-start]');
      const stop = documentRef?.querySelector?.('[data-incident-voice-stop]');
      if (start) start.hidden = true;
      if (stop) stop.hidden = false;
      setStatus(local('التسجيل شغال. السكوت لا يوقف التسجيل. اضغط إيقاف وإرسال عند الانتهاء.','Recording is active. Silence does not stop it. Press stop/send when finished.'));
    });
    documentRef?.querySelector?.('[data-incident-voice-stop]')?.addEventListener('click', async () => {
      if (!recording) return;
      recording = false;
      const result = await incidentVoiceController?.stopVoiceMessage({ transcriptContext:{ language:currentLanguage() } });
      if (!result?.ok) {
        setStatus(local(`لم يتم حفظ الصوت: ${result?.reason ?? 'UNKNOWN'}.`,`Voice was not persisted: ${result?.reason ?? 'UNKNOWN'}.`));
        requestRender();
        return;
      }
      await refresh();
    });

    documentRef?.querySelector?.('[data-live-voice-start]')?.addEventListener('click', async () => {
      const result = await liveVoiceSession.start({ turnComplete:async completion => {
        if (!completion?.ok || !completion.speechDetected) {
          setStatus(local('انتهى الدور بدون كلام واضح، ولم يتم حفظ صوت فارغ.','Turn ended without clear speech; empty audio was not persisted.'));
          return;
        }
        const ingested = await incidentVoiceController?.ingestRecordedCustomerVoice({
          blob:completion.blob,
          mimeType:completion.mimeType,
          transcriptContext:{ language:currentLanguage(), mode:'LIVE_VOICE' },
          provenance:'CUSTOMER_LIVE_VOICE'
        });
        if (!ingested?.ok) {
          setStatus(local(`تعذر حفظ دور Live Voice: ${ingested?.reason ?? 'UNKNOWN'}.`,`Could not persist Live Voice turn: ${ingested?.reason ?? 'UNKNOWN'}.`));
          return;
        }
        await refresh();
      }});
      if (!result.ok) {
        setStatus(local('تعذر بدء Live Voice. تحقق من إذن المايك ودعم المتصفح.','Could not start Live Voice. Check microphone permission and browser support.'));
        return;
      }
      setStatus(local('Live Voice يستمع الآن. Falcon لن يقاطعك، وينتظر 15 ثانية سكوت متواصل بعد كلامك قبل إنهاء دورك.','Live Voice is listening. Falcon will not interrupt and waits 15 continuous seconds of silence after speech before ending your turn.'));
    });

    documentRef?.querySelector?.('[data-incident-screenshot]')?.addEventListener('click', () => {
      const picker = documentRef.createElement('input');
      picker.type = 'file';
      picker.accept = 'image/*';
      picker.multiple = false;
      picker.addEventListener('change', async () => {
        const result = await screenshotController?.acceptFiles(picker.files);
        if (!result?.ok) {
          setStatus(local(`لم يتم حفظ الصورة: ${result?.reason ?? 'UNKNOWN'}.`,`Screenshot was not persisted: ${result?.reason ?? 'UNKNOWN'}.`));
          return;
        }
        await refresh();
      }, { once:true });
      picker.click();
    });

    documentRef?.querySelectorAll?.('[data-audio-artifact]')?.forEach(button => button.addEventListener('click', async () => {
      const artifactId = button.dataset.audioArtifact;
      if (!artifactId) return;
      const result = await persistence.getArtifact(artifactId);
      const blob = result?.artifact?.blob;
      if (!BlobCtor || !(blob instanceof BlobCtor)) {
        setStatus(local('ملف الصوت غير متاح محليًا.','Audio artifact is not locally available.'));
        return;
      }
      const url = urlApi.createObjectURL(blob);
      const audio = new AudioCtor(url);
      audio.addEventListener('ended', () => urlApi.revokeObjectURL(url), { once:true });
      audio.addEventListener('error', () => urlApi.revokeObjectURL(url), { once:true });
      await audio.play();
    }));

    documentRef?.querySelectorAll?.('[data-support-takeover]')?.forEach(button => button.addEventListener('click', async () => {
      if (button.disabled || !incidentController) return;
      const activeSession = currentSession();
      if (!activeSession?.authoritativeSession || !activeSession?.principalId || !Array.isArray(activeSession.capabilities) || !activeSession.capabilities.includes('INCIDENT_SUPPORT_TAKEOVER')) return;
      const displayName = activeSession.displayName ?? local('الدعم','Support');
      const result = await incidentController.startSupportTakeover({ principalId:activeSession.principalId, displayName });
      if (result.ok) requestRender();
    }));
  }

  return Object.freeze({ markup, initialize, bindActions });
}
