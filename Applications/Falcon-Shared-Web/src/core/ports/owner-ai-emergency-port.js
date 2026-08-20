import { TruthState } from '../../contracts.js';
import { deepFreeze } from '../immutable.js';

/**
 * Web-owned consumer boundary for FCR-0224/FCR-0225 after Foundation Stage 13
 * WP-01 acceptance/closure.
 *
 * The types below mirror only the public behavior Shared Web must request and
 * present. This module does not import Foundation internals, define a runtime
 * route, authorize a Kill, execute containment, or expose release/revival.
 */
export const OwnerAiEmergencyTargetScope = Object.freeze({
  ONE_AI_COMPONENT:'ONE_AI_COMPONENT',
  ONE_CSA:'ONE_CSA',
  ONE_LSA_OR_BRANCH_AI_SCOPE:'ONE_LSA_OR_BRANCH_AI_SCOPE',
  ONE_MSA_OR_APPLICATION_AI_SCOPE:'ONE_MSA_OR_APPLICATION_AI_SCOPE',
  FSA:'FSA',
  DEFINED_AI_GROUP:'DEFINED_AI_GROUP',
  ALL_AI:'ALL_AI'
});

export const OwnerAiEmergencyAction = Object.freeze({
  RESTRICT:'RESTRICT',
  SUSPEND:'SUSPEND',
  ISOLATE:'ISOLATE',
  KILL:'KILL',
  GLOBAL_AI_KILL:'GLOBAL_AI_KILL'
});

export const OwnerAiEmergencyOutcome = Object.freeze({
  REQUEST_SENT:'REQUEST_SENT',
  ACTION_ACCEPTED:'ACTION_ACCEPTED',
  ACTION_COMPLETED:'ACTION_COMPLETED',
  DENIED:'DENIED',
  FAILED:'FAILED',
  UNKNOWN:'UNKNOWN',
  UNAVAILABLE:'UNAVAILABLE'
});

export const OwnerAiEmergencyPortMethods = Object.freeze([
  'targetInventory',
  'targetState',
  'submitAction',
  'actionOutcome',
  'safeCoreState'
]);

const targetScopes = new Set(Object.values(OwnerAiEmergencyTargetScope));
const actions = new Set(Object.values(OwnerAiEmergencyAction));
const outcomes = new Set(Object.values(OwnerAiEmergencyOutcome));
const unavailable = extra => Object.freeze({ truth:TruthState.UNAVAILABLE, ...extra });
const nonEmpty = value => typeof value === 'string' && value.trim().length > 0;
const isCurrent = value => value === TruthState.CURRENT || value === 'CURRENT';

export function createUnavailableOwnerAiEmergencyPort() {
  return Object.freeze({
    async targetInventory() { return unavailable({ items:[], reasonCode:'WEB_AI_KILL_RUNTIME_BINDING_UNAVAILABLE' }); },
    async targetState() { return unavailable({ target:null, reasonCode:'WEB_AI_KILL_RUNTIME_BINDING_UNAVAILABLE' }); },
    async submitAction() {
      return unavailable({
        outcome:OwnerAiEmergencyOutcome.UNAVAILABLE,
        requestId:null,
        correlationId:null,
        reasonCode:'WEB_AI_KILL_REQUEST_TRANSPORT_UNAVAILABLE'
      });
    },
    async actionOutcome() {
      return unavailable({
        outcome:OwnerAiEmergencyOutcome.UNAVAILABLE,
        requestId:null,
        correlationId:null,
        reasonCode:'WEB_AI_KILL_OUTCOME_BINDING_UNAVAILABLE'
      });
    },
    async safeCoreState() { return unavailable({ reasonCode:'WEB_SAFE_CORE_PROJECTION_BINDING_UNAVAILABLE' }); }
  });
}

/**
 * Builds Web's request intent corresponding to Foundation AiKillRequest semantics.
 * It deliberately does not claim authority or transport. The caller supplies the
 * exact target identity/type from current authoritative inventory and later sends
 * this object only through a separately governed runtime adapter.
 */
export function buildOwnerAiEmergencyRequest({ requestId, correlationId, session, target, action, blastRadius, requestedAt, expiresAt } = {}) {
  const eligibility = canPrepareOwnerAiEmergencySubmission({ session, target, action, blastRadius });
  if (!eligibility.allowed) throw new TypeError(eligibility.reasonCode);
  for (const [name,value] of Object.entries({ requestId, correlationId, requestedAt, expiresAt })) {
    if (!nonEmpty(value)) throw new TypeError(`${name} is required`);
  }
  return deepFreeze({
    requestId:requestId.trim(),
    actorIdentity:session.principalId,
    ingress:'WEB_OWNER',
    action,
    targetScope:target.scope,
    targetId:target.id,
    correlationId:correlationId.trim(),
    requestTime:requestedAt,
    expiry:expiresAt,
    blastRadiusConfirmation:{
      authoritative:true,
      freshness:'CURRENT',
      targetIds:[...blastRadius.targetIds]
    },
    webAuthorizationClaim:false,
    executionClaim:false,
    releaseOrRevivalRequested:false
  });
}

/**
 * Binds an authoritative Foundation decision projection for Web presentation.
 * Accepted is not completed. Unknown/denied decisions never acquire impacted
 * targets, and GLOBAL_AI_KILL must preserve Safe Core and must not authorize a
 * Falcon shutdown. Contradictory payloads fail closed instead of being normalized.
 */
export function bindOwnerAiEmergencyDecision(input) {
  if (!input || typeof input !== 'object') throw new TypeError('decision is required');
  const requiredStrings = ['requestId','correlationId','targetId','action','reason'];
  for (const field of requiredStrings) if (!nonEmpty(input[field])) throw new TypeError(`${field} is required`);
  if (!actions.has(input.action)) throw new TypeError('unsupported action');
  if (input.outcome !== undefined && !outcomes.has(input.outcome)) throw new TypeError('unsupported outcome');
  if (typeof input.accepted !== 'boolean') throw new TypeError('accepted must be boolean');
  if (!Array.isArray(input.impactedTargetIds)) throw new TypeError('impactedTargetIds must be an array');
  if (input.impactedTargetIds.some(id => !nonEmpty(id))) throw new TypeError('impactedTargetIds require exact identities');
  if (new Set(input.impactedTargetIds).size !== input.impactedTargetIds.length) throw new TypeError('duplicate impacted target identity');

  if (!input.accepted && input.impactedTargetIds.length !== 0) throw new TypeError('denied decision cannot widen blast radius');
  if (input.accepted && input.impactedTargetIds.length === 0) throw new TypeError('accepted decision requires impacted AI');
  if (input.action === OwnerAiEmergencyAction.GLOBAL_AI_KILL) {
    if (input.safeCorePreserved !== true) throw new TypeError('GLOBAL_AI_KILL must preserve Falcon Safe Core');
    if (input.falconShutdownAuthorized !== false) throw new TypeError('GLOBAL_AI_KILL must not authorize Falcon shutdown');
  }
  if (input.action === OwnerAiEmergencyAction.KILL && input.accepted === true && input.releaseRequiresGovernedRecovery !== true) {
    throw new TypeError('accepted KILL must require governed recovery before release');
  }
  if (input.targetCooperationRequired === true) throw new TypeError('target AI cooperation must not be required');

  const outcome = input.outcome ?? (input.accepted ? OwnerAiEmergencyOutcome.ACTION_ACCEPTED : OwnerAiEmergencyOutcome.DENIED);
  if (outcome === OwnerAiEmergencyOutcome.ACTION_COMPLETED && input.completed !== true) {
    throw new TypeError('ACTION_COMPLETED requires authoritative completion evidence');
  }

  return deepFreeze({
    requestId:input.requestId,
    correlationId:input.correlationId,
    targetId:input.targetId,
    action:input.action,
    accepted:input.accepted,
    outcome,
    reason:input.reason,
    impactedTargetIds:[...input.impactedTargetIds],
    safeCorePreserved:input.safeCorePreserved === true,
    falconShutdownAuthorized:input.falconShutdownAuthorized === true,
    stopRequired:input.stopRequired === true,
    suspensionRequired:input.suspensionRequired === true,
    isolationRequired:input.isolationRequired === true,
    authorityRevocationRequired:input.authorityRevocationRequired === true,
    evidenceFreezeRequired:input.evidenceFreezeRequired === true,
    releaseRequiresGovernedRecovery:input.releaseRequiresGovernedRecovery === true,
    targetCooperationRequired:false,
    reviewDeadline:input.reviewDeadline ?? null,
    evidenceReference:input.evidenceReference ?? null,
    incidentReference:input.incidentReference ?? null,
    completed:input.completed === true,
    presentationOnly:true,
    webAuthorized:false,
    webExecuted:false,
    releaseAvailable:false
  });
}

export function canPrepareOwnerAiEmergencySubmission({ session, target, action, blastRadius } = {}) {
  const authoritativeOwner = Boolean(
    session
    && session.authoritativeSession === true
    && session.role === 'PROJECT_OWNER'
    && nonEmpty(session.principalId)
  );
  if (!authoritativeOwner) return Object.freeze({ allowed:false, reasonCode:'AUTHORITATIVE_OWNER_SESSION_REQUIRED' });
  if (!target || !nonEmpty(target.id)) return Object.freeze({ allowed:false, reasonCode:'EXACT_TARGET_REQUIRED' });
  if (!targetScopes.has(target.scope)) return Object.freeze({ allowed:false, reasonCode:'AUTHORITATIVE_TARGET_SCOPE_REQUIRED' });
  if (target.truth !== undefined && !isCurrent(target.truth)) return Object.freeze({ allowed:false, reasonCode:'CURRENT_AUTHORITATIVE_TARGET_STATE_REQUIRED' });
  if (target.freshness !== undefined && !isCurrent(target.freshness)) return Object.freeze({ allowed:false, reasonCode:'CURRENT_AUTHORITATIVE_TARGET_STATE_REQUIRED' });
  if (!actions.has(action)) return Object.freeze({ allowed:false, reasonCode:'GOVERNED_ACTION_REQUIRED' });
  if (!blastRadius || blastRadius.authoritative !== true || blastRadius.freshness !== 'CURRENT' || !Array.isArray(blastRadius.targetIds)) {
    return Object.freeze({ allowed:false, reasonCode:'CURRENT_AUTHORITATIVE_BLAST_RADIUS_REQUIRED' });
  }
  if (blastRadius.targetIds.some(id => !nonEmpty(id)) || new Set(blastRadius.targetIds).size !== blastRadius.targetIds.length) {
    return Object.freeze({ allowed:false, reasonCode:'EXACT_BLAST_RADIUS_IDENTITIES_REQUIRED' });
  }
  if (action === OwnerAiEmergencyAction.GLOBAL_AI_KILL) {
    if (target.scope !== OwnerAiEmergencyTargetScope.ALL_AI) return Object.freeze({ allowed:false, reasonCode:'GLOBAL_KILL_REQUIRES_ALL_AI_TARGET' });
    if (blastRadius.targetIds.length === 0) return Object.freeze({ allowed:false, reasonCode:'GLOBAL_KILL_REQUIRES_NONEMPTY_AI_CENSUS' });
  } else if (target.scope === OwnerAiEmergencyTargetScope.ALL_AI) {
    return Object.freeze({ allowed:false, reasonCode:'ALL_AI_TARGET_REQUIRES_GLOBAL_AI_KILL' });
  }
  return Object.freeze({ allowed:true, reasonCode:null });
}
