# Shared Falcon Web — Owner Decisions: Incident Conversation

**Date:** 2026-08-16
**Branch:** `web-development`
**Scope:** Shared Web customer-facing incident interaction only
**Status:** `OWNER_DECISIONS_RECORDED / IMPLEMENTATION_RECONCILIATION_REQUIRED`

This record captures explicit Project Owner decisions for the Shared Web incident experience. It is a Web-owned decision record and does not transfer Trading/Application business-truth authority to Shared Web.

## 1. Authority boundary

Application/Guardian decides the incident business meaning, affected positions/orders, required customer information/action, priority, protection state, simulator/shadow semantics, and whether customer intervention is required.

Shared Web decides how to communicate and guide the customer without changing the Application/Guardian meaning.

```text
WEB_RESPONSE_TRANSPORT != TRADING_DECISION_AUTHORITY
WEB_GUIDANCE != BROKER_EXECUTION_AUTHORITY
SIMULATOR_ESTIMATE != BROKER_TRUTH
SCREENSHOT_OBSERVED != BROKER_CONFIRMED_TRUTH
```

## 2. One persistent incident conversation

A customer-facing incident is handled as one coherent persistent Incident Conversation, not fragmented into multiple customer incident tabs for consequences of the same incident.

The complete incident interaction is retained from A to Z in the same incident context, including the ordered timeline of permitted interaction artifacts.

The retained incident conversation includes, as applicable:

- Falcon/customer text messages;
- voice interaction and its transcript/context;
- mixed voice + text chronology;
- guided steps and customer responses;
- permitted screenshots and screenshot-related guidance;
- Support escalation and explicit takeover events;
- Support/customer messages during takeover;
- Falcon state transitions relevant to the customer-facing conversation;
- incident resolution communication;
- final incident summary.

Voice and text are not alternatives for retention. If both occur, both remain associated with the same incident chronology.

Security/privacy/retention-duration/deletion/export implementation must still obey the governing security/data contracts and must not persist prohibited secrets or credentials.

## 3. Customer communication under stress

Falcon adapts communication automatically when the customer appears highly stressed or unfocused:

- shorter sentences;
- one step at a time;
- confirmation after each step;
- reduced cognitive load;
- simple choices;
- calm, human reassurance.

Reassurance must remain truthful and may use only Application/Guardian-supplied current facts. Web must not invent numerical guarantees or protection truth.

## 4. Broker connectivity incident explanation

When Application/Guardian identifies a broker/API connectivity incident, Falcon explains that Falcon is not the broker and that the current problem concerns broker connectivity/state according to the supplied incident semantics.

Falcon may explain that similar connectivity issues can occur even when a person trades directly, but it must not misrepresent the actual incident cause or broker state.

## 5. Customer asks to close positions independently

If a frightened customer asks to close positions, Falcon warns clearly that a rushed or incorrect close can create or realize losses and that Falcon's objective is to protect/manage the situation rather than escalate risk.

Falcon should repeat the warning sufficiently to ensure the customer understands the risk.

If the customer remains explicit and persistent that they personally want to close positions through the broker, this remains the customer's own decision. Falcon may guide the customer through the broker UI step-by-step so the customer can perform the action themselves.

This guidance does not convert Falcon into the broker and does not make the decision Falcon's trading decision.

```text
CUSTOMER_DECISION != FALCON_TRADING_DECISION
GUIDANCE_TO_BROKER_UI != BROKER_EXECUTION
```

## 6. Support request and takeover

If the customer asks for human Support, Falcon should attempt to route the same Incident Conversation to an available authorized Support person.

Routing preference is the first authorized available person. During the current phase, the Project Owner may act as Support if no other Support person is available.

If nobody is available, Falcon tells the customer that Support is currently unavailable, continues the Guardian/Application-requested precautionary guidance within Falcon's authority, and keeps the Support request active for the earliest available authorized person.

Example intent:

> Support is currently unavailable. As a precaution, please continue with me on these required points, and I will keep the Support request active so they can contact/join you at the earliest available opportunity.

## 7. Support becomes available during an active step

If Support becomes available while the customer is in the middle of a useful step with Falcon, Falcon does not abruptly interrupt the customer.

Falcon tells the customer Support is available and offers the choice:

- finish the current step with Falcon, then transfer; or
- transfer to Support immediately.

The customer chooses.

An actual Support takeover must be explicit and visible. During takeover Falcon is silent in the customer-facing conversation and Support is clearly identified as human Support.

## 8. Incident resolved before requested Support takeover

If the incident is resolved before a previously requested Support transfer occurs, Falcon does not silently cancel the customer's request.

Falcon tells the customer the problem is resolved and Support is available, then asks whether the customer still wants to speak with Support to understand the problem and the fix.

If the customer says yes, transfer. If no, no transfer is required.

## 9. Screenshot / description choice

When the customer sees something unexpected or unclear, Falcon offers both options without pressure:

- describe what is visible; or
- send a screenshot.

Screenshots are one at a time and must not contain credentials/secrets. Screenshot evidence never becomes broker-confirmed truth merely because Falcon or Support viewed it.

## 10. Incident closure summary is mandatory

Every completed customer-facing incident must provide a short customer-readable closure summary based only on authoritative supplied information.

The summary includes, where supplied/available:

- what happened;
- incident start/end timing;
- affected positions/orders;
- relevant state of those affected items;
- FSTSimA/shadow-monitoring analysis period, including from-time and to-time where Application supplies them;
- resulting simulator/shadow assessment, clearly labeled as simulator evidence rather than broker truth;
- what corrective/recovery action occurred;
- current resolved/remaining-follow-up state.

Shared Web must not invent affected trades, protection classification, simulator timing, or simulator outcome. Those remain Application-owned semantics and are tracked through FCR-0201 where still unresolved.

## 11. Side topics during a critical incident

Falcon may answer a customer's natural side question briefly and truthfully, especially trust-related questions such as what Falcon is doing, then gently return to the critical incident task. Falcon must not expose unnecessary internal details or invent Guardian/Application state.

## 12. Voice guidance

Where useful, Falcon may offer live voice guidance and guide the customer interactively step-by-step based on what the customer reports or sees.

The customer may use voice, text, or both during the same Incident Conversation. The incident record preserves the chronological continuity of those permitted interaction modes.

### 12.1 Ordinary voice-message recording

Silence-based automatic stop is prohibited.

```text
VOICE_MESSAGE_SILENCE_AUTO_STOP = DISABLED
```

A customer's pause or silence must not end or send an ordinary voice message automatically. The customer explicitly ends/sends the recording through the supported voice control interaction.

### 12.2 Live Voice Guidance patience rule

During Live Voice Guidance, Falcon must give the customer enough time to think, navigate the broker/site UI, read what is on screen, and answer without being rushed.

```text
LIVE_VOICE_GUIDANCE_SILENCE_TOLERANCE_BEFORE_FALCON_REPLY = 15_SECONDS
```

A short pause does not mean the customer's turn is over. Falcon waits through up to 15 seconds of continuous silence before replying, unless the customer explicitly ends their turn earlier. If the customer resumes speaking before the 15-second interval expires, Falcon continues listening and does not interrupt.

This 15-second patience rule applies to Live Voice Guidance turn-taking, not to ordinary voice-message auto-stop, which remains disabled.

## 13. Existing FCR dependencies

- FCR-0095 remains the governing cross-workstream incident interaction/binding record and requires Web implementation plus governed verification before closure eligibility.
- FCR-0201 remains Application-owned for exact affected-position and FSTSimA shadow-monitoring projection semantics. Web must not infer those Trading/Application facts.

## 14. Anti-repeat rule for Web planning

These Owner decisions are not to be re-asked as open UX questions unless:

1. the Owner explicitly reopens one of them;
2. a newer authoritative contract creates a real incompatibility; or
3. implementation reveals a genuinely new unresolved decision not answered by this record or governing references.

Before asking the Owner a new Incident UX question, Shared Web must first read this decision record and the current relevant FCR state.
