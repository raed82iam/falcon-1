# Shared Falcon Web — Local Free Voice Implementation

**Date:** 2026-08-16
**Branch:** `web-development`
**Scope:** `applications/shared/web/**`
**Status:** `IMPLEMENTATION_STARTED / LOCAL_BINDING_REQUIRED`

## Owner-approved cost rule

Voice must not depend on a paid per-minute or subscription API for the current Falcon Web implementation.

```text
VOICE_RUNTIME_COST_TARGET = 0_SAR_API_USAGE
SPEECH_TO_TEXT_PROVIDER = WHISPER_CPP_LOCAL
TEXT_TO_SPEECH_PROVIDER = PIPER_LOCAL
PAID_REMOTE_VOICE_API = PROHIBITED_FOR_CURRENT_IMPLEMENTATION
```

The browser captures microphone audio only after explicit customer action and normal browser permission. No hidden microphone activation is allowed.

## Voice-message behavior

```text
VOICE_MESSAGE_SILENCE_AUTO_STOP = DISABLED
VOICE_MESSAGE_END = EXPLICIT_CUSTOMER_ACTION
```

Silence alone never stops or sends an ordinary voice message.

## Live Voice Guidance behavior

```text
LIVE_VOICE_EXPLICIT_OPT_IN = REQUIRED
LIVE_VOICE_GUIDANCE_SILENCE_TOLERANCE_BEFORE_FALCON_REPLY = 15_SECONDS
```

Falcon waits through up to 15 seconds of continuous silence before replying. If the customer resumes speech during that interval, Falcon continues listening. The customer may explicitly end the turn earlier.

## Architecture

```text
Customer microphone
  -> BrowserMicrophone
  -> VoiceArtifact
  -> LocalVoiceRuntime.speechToText
  -> WHISPER_CPP_LOCAL binding
  -> transcript
  -> Falcon conversation
  -> LocalVoiceRuntime.textToSpeech
  -> PIPER_LOCAL binding
  -> audio reply
```

The Web code owns browser capture, customer controls, incident chronology, and provider-neutral local-runtime ports. It does not invent a cross-Application transport route or bypass Foundation/runtime authority.

## Incident retention

Voice artifacts, transcripts, text messages, support events, screenshots that pass policy, guided steps, and closure information remain associated with the same persistent Incident Conversation chronology. Secrets and credentials remain prohibited.

## Current implementation boundary

The browser microphone controller, voice policy, local voice runtime abstraction, and incident timeline model are implemented in Web source. Exact executable binding to installed `whisper.cpp` and Piper processes remains a governed local-runtime integration step and is fail-closed when unavailable.
