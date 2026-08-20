# Shared Web AI Chat and Incident Persistence Reconciliation

Date: 2026-08-16
Status: CURRENT WEB RECONCILIATION

## Purpose

This record reconciles the older planning note `docs/ Ideas/06 - محادثة المستخدم مع AI.md` with the later explicit Project Owner decisions recorded in `OWNER_DECISIONS_INCIDENT_CONVERSATION_2026-08-16.md`.

The older Ideas document remains useful for ordinary advisory AI-chat planning, but its broad statement that conversation persistence/history ownership is unresolved must not be applied to a customer-facing Incident Conversation.

## Binding distinction

```text
ORDINARY USER AI CHAT
!=
CUSTOMER-FACING INCIDENT CONVERSATION
```

### Customer-facing Incident Conversation

The following is settled by explicit Owner decision and is not an open UX question:

- one persistent Incident Conversation exists per customer-facing incident;
- the incident interaction is retained from A to Z in the same incident context;
- text and voice coexist in one chronological record when both are used;
- transcripts/context, permitted screenshot interactions, guided steps, customer responses, Support escalation/takeover events, relevant state transitions, resolution communication, and the mandatory closure summary remain part of that incident record;
- credentials, secrets, and reusable authentication material must not be persisted;
- Web persistence does not convert customer statements, transcripts, screenshots, or simulator evidence into broker/business truth;
- retention duration, deletion/export mechanics, privacy enforcement, and tenancy controls must obey governing security/data contracts. A missing governing mechanism must fail closed rather than erase the Owner-settled requirement for incident continuity/audit.

Canonical Owner decision record:

`applications/shared/web/docs/OWNER_DECISIONS_INCIDENT_CONVERSATION_2026-08-16.md`

### Ordinary advisory AI Chat

The broader non-incident AI Chat remains governed by its own contracts and may still have unresolved details, including:

- exact long-term conversation retention duration;
- exact deletion/export mechanics;
- exact tenancy/storage implementation;
- exact permitted reuse of prior chats as AI context/memory;
- final audit/privacy policy;
- final security/data-controller contract binding.

Shared Web must not silently generalize the Incident Conversation persistence decision into an unlimited or undefined retention policy for all ordinary AI Chat.

## Anti-regression rule

When the older Ideas document and the current Owner incident decision appear to conflict:

```text
INCIDENT-SPECIFIC OWNER DECISION CONTROLS INCIDENT UX
OLDER GENERAL CHAT PLANNING CONTINUES ONLY OUTSIDE THAT INCIDENT-SPECIFIC SCOPE
```

Do not re-ask whether incident text, incident voice, mixed voice/text chronology, Support messages, or the mandatory incident closure summary are persisted. Those decisions are settled unless the Project Owner explicitly reopens them or a newer authoritative governing contract creates a real incompatibility.
