import { TruthState, FreshnessState, CompletenessState, AvailabilityState } from '../contracts.js';
import { consumeFoundationRecoveryFilProjection } from './foundation-fil-public-runtime-v1.js';

const RELEASE_STATES = new Set(['UNAVAILABLE','NOT_STARTED','NOT_READY','READY_FOR_RELEASE_DECISION','NOT_AUTHORIZED','AUTHORIZED','EXECUTING','NOT_EXECUTED','EXECUTED','RELEASED','DENIED','UNKNOWN']);
const RECOVERY_STATES = new Set(['UNAVAILABLE','NOT_STARTED','IN_PROGRESS','VERIFICATION_REQUIRED','VERIFIED','FAILED','UNKNOWN']);
const FOUNDATION_RECOVERY_STATES = new Set([
  'InitiationPending','AuthorizedForAssessment','PlanAuthorizationPending','PlanAuthorized',
  'RestorationInProgress','RestorationReported','ReconciliationPending','ValidationPending',
  'ValidationFailed','ReadyForReleaseDecision','ReleaseDenied','ReleaseAuthorized',
  'ReintroductionPending','RecoveryGuardObservation','RecoveredWithRestrictedAuthority',
  'RecoveryComplete','Aborted','Escalated'
]);
const FOUNDATION_RESTORATION_OUTCOMES = new Set(['Requested','Attempted','Completed','Failed','Partial']);
const TRUTH = new Set(Object.values(TruthState));
const FRESHNESS = new Set(Object.values(FreshnessState));
const COMPLETENESS = new Set(Object.values(CompletenessState));
const AVAILABILITY = new Set(Object.values(AvailabilityState));

const pick = (value, camel, pascal) => value?.[camel] ?? value?.[pascal];
const token = value => typeof value === 'string' && value.length > 0 && value.trim() === value && !/[\u0000-\u001f\s]/u.test(value);
const sha256Artifact = value => typeof value === 'string' && /^sha256\/[0-9a-f]{64}$/iu.test(value);

function optionalText(value) {
  return value === null || value === undefined || value === '' ? null : String(value);
}

function enumValue(value, allowed, name, fallback) {
  const candidate = value ?? fallback;
  if (!allowed.has(candidate)) throw new TypeError(`${name} is invalid`);
  return candidate;
}

function freezeList(value) {
  return Object.freeze(Array.isArray(value) ? value.map(item => String(item)) : []);
}

function unavailable(reason = null) {
  return Object.freeze({
    truth: TruthState.UNAVAILABLE,
    freshness: FreshnessState.UNAVAILABLE,
    completeness: CompletenessState.UNKNOWN,
    availability: AvailabilityState.UNAVAILABLE,
    recoveryState: 'UNAVAILABLE',
    restorationOutcome: null,
    releaseDecisionReadiness: 'UNAVAILABLE',
    releaseAuthorizationState: 'UNAVAILABLE',
    releaseExecutionState: 'UNAVAILABLE',
    reintroductionState: 'UNAVAILABLE',
    lifecycleState: null,
    recoveryCaseId: null,
    projectionIdentity: null,
    evidenceReferences: Object.freeze([]),
    asOfTime: null,
    validUntil: null,
    source: null,
    presentationOnly: true,
    mayAuthorizeRelease: false,
    mayExecuteRelease: false,
    mayChangeLifecycle: false,
    businessAuthorityGranted: false,
    bindingFailureReason: reason
  });
}

/**
 * Historical normalized presentation adapter retained for already-verified
 * Web behavior and fixtures. It never creates authority.
 */
export function adaptFoundationStage9Projection(input) {
  if (!input || typeof input !== 'object') return unavailable();

  const truth = enumValue(input.truth, TRUTH, 'truth', TruthState.UNAVAILABLE);
  const freshness = enumValue(input.freshness, FRESHNESS, 'freshness', FreshnessState.UNKNOWN);
  const completeness = enumValue(input.completeness, COMPLETENESS, 'completeness', CompletenessState.UNKNOWN);
  const availability = enumValue(input.availability, AVAILABILITY, 'availability', AvailabilityState.UNKNOWN);
  const recoveryState = enumValue(input.recoveryState, RECOVERY_STATES, 'recoveryState', 'UNKNOWN');
  const releaseDecisionReadiness = enumValue(input.releaseDecisionReadiness, RELEASE_STATES, 'releaseDecisionReadiness', 'UNKNOWN');
  const releaseAuthorizationState = enumValue(input.releaseAuthorizationState, RELEASE_STATES, 'releaseAuthorizationState', 'UNKNOWN');
  const releaseExecutionState = enumValue(input.releaseExecutionState, RELEASE_STATES, 'releaseExecutionState', 'UNKNOWN');

  if (availability === AvailabilityState.UNAVAILABLE || truth === TruthState.UNAVAILABLE) return unavailable();
  if ((releaseExecutionState === 'RELEASED' || releaseExecutionState === 'EXECUTED') && releaseAuthorizationState !== 'AUTHORIZED') {
    throw new TypeError('release execution cannot imply missing release authorization');
  }
  if (releaseAuthorizationState === 'AUTHORIZED' && releaseDecisionReadiness === 'NOT_READY') {
    throw new TypeError('release authorization cannot coexist with NOT_READY decision readiness');
  }

  return Object.freeze({
    truth,
    freshness,
    completeness,
    availability,
    recoveryState,
    restorationOutcome: optionalText(input.restorationOutcome),
    releaseDecisionReadiness,
    releaseAuthorizationState,
    releaseExecutionState,
    reintroductionState: optionalText(input.reintroductionState) ?? 'UNKNOWN',
    lifecycleState: optionalText(input.lifecycleState),
    recoveryCaseId: optionalText(input.recoveryCaseId),
    projectionIdentity: optionalText(input.projectionIdentity),
    evidenceReferences: freezeList(input.evidenceReferences),
    asOfTime: optionalText(input.asOfTime),
    validUntil: optionalText(input.validUntil),
    source: optionalText(input.source),
    presentationOnly: true,
    mayAuthorizeRelease: false,
    mayExecuteRelease: false,
    mayChangeLifecycle: false,
    businessAuthorityGranted: false,
    bindingFailureReason: null
  });
}

function foundationEnum(value, mapping, name) {
  if (Number.isInteger(value) && mapping[value]) return mapping[value];
  if (typeof value === 'string') {
    const direct = Object.values(mapping).find(item => item === value);
    if (direct) return direct;
  }
  throw new TypeError(`${name} is invalid`);
}

const AUTHORIZATION = Object.freeze({1:'NotAuthorized',2:'Denied',3:'Authorized'});
const EXECUTION = Object.freeze({1:'NotExecuted',2:'Executed'});
const REINTRODUCTION = Object.freeze({1:'NotStarted',2:'Pending',3:'Observation',4:'Restricted',5:'Complete'});
const FOUNDATION_FRESHNESS = Object.freeze({1:'Current',2:'Stale'});

function expectedReady(recoveryState) {
  return new Set(['ReadyForReleaseDecision','ReleaseDenied','ReleaseAuthorized','ReintroductionPending','RecoveryGuardObservation','RecoveredWithRestrictedAuthority','RecoveryComplete']).has(recoveryState);
}

function expectedAuthorization(recoveryState) {
  if (recoveryState === 'ReleaseDenied') return 'Denied';
  if (new Set(['ReleaseAuthorized','ReintroductionPending','RecoveryGuardObservation','RecoveredWithRestrictedAuthority','RecoveryComplete']).has(recoveryState)) return 'Authorized';
  return 'NotAuthorized';
}

function expectedExecution(recoveryState) {
  return new Set(['ReintroductionPending','RecoveryGuardObservation','RecoveredWithRestrictedAuthority','RecoveryComplete']).has(recoveryState)
    ? 'Executed'
    : 'NotExecuted';
}

function expectedReintroduction(recoveryState) {
  return ({
    ReintroductionPending:'Pending',
    RecoveryGuardObservation:'Observation',
    RecoveredWithRestrictedAuthority:'Restricted',
    RecoveryComplete:'Complete'
  })[recoveryState] ?? 'NotStarted';
}

/**
 * Exact consumer for Foundation RecoveryOperationalProjection.
 * Preserves Foundation truth and only translates it into Web presentation
 * states after the canonical FIL packet has been verified.
 */
export function adaptFoundationRecoveryOperationalProjection(raw, { now = Date.now() } = {}) {
  if (!raw || typeof raw !== 'object' || Array.isArray(raw)) throw new TypeError('Foundation recovery projection is required');

  const projectionIdentity = pick(raw,'projectionIdentity','ProjectionIdentity');
  const recoveryCaseIdentity = pick(raw,'recoveryCaseIdentity','RecoveryCaseIdentity');
  const recoveryState = pick(raw,'recoveryState','RecoveryState');
  const restorationOutcome = pick(raw,'restorationOutcome','RestorationOutcome');
  const ready = pick(raw,'readyForReleaseDecision','ReadyForReleaseDecision');
  const authorization = foundationEnum(pick(raw,'releaseAuthorization','ReleaseAuthorization'), AUTHORIZATION, 'releaseAuthorization');
  const execution = foundationEnum(pick(raw,'releaseExecution','ReleaseExecution'), EXECUTION, 'releaseExecution');
  const reintroduction = foundationEnum(pick(raw,'reintroduction','Reintroduction'), REINTRODUCTION, 'reintroduction');
  const lifecycleState = pick(raw,'lifecycleState','LifecycleState');
  const evidenceReference = pick(raw,'evidenceReference','EvidenceReference');
  const observedAt = pick(raw,'observedAt','ObservedAt');
  const validUntil = pick(raw,'validUntil','ValidUntil');
  const complete = pick(raw,'complete','Complete');
  const freshness = foundationEnum(pick(raw,'freshness','Freshness'), FOUNDATION_FRESHNESS, 'freshness');
  const presentationOnly = pick(raw,'presentationOnly','PresentationOnly');
  const carriesReleaseExecutionAuthority = pick(raw,'carriesReleaseExecutionAuthority','CarriesReleaseExecutionAuthority');
  const carriesLifecycleAuthority = pick(raw,'carriesLifecycleAuthority','CarriesLifecycleAuthority');
  const carriesBusinessAuthority = pick(raw,'carriesBusinessAuthority','CarriesBusinessAuthority');

  if (!sha256Artifact(projectionIdentity)) throw new TypeError('projectionIdentity is invalid');
  if (!token(recoveryCaseIdentity)) throw new TypeError('recoveryCaseIdentity is invalid');
  if (!FOUNDATION_RECOVERY_STATES.has(recoveryState)) throw new TypeError('recoveryState is invalid');
  if (!FOUNDATION_RESTORATION_OUTCOMES.has(restorationOutcome)) throw new TypeError('restorationOutcome is invalid');
  if (typeof ready !== 'boolean') throw new TypeError('readyForReleaseDecision is invalid');
  if (!token(lifecycleState) || !token(evidenceReference)) throw new TypeError('lifecycle/evidence identity is invalid');
  if (typeof complete !== 'boolean') throw new TypeError('complete is invalid');
  if (presentationOnly !== true || carriesReleaseExecutionAuthority !== false || carriesLifecycleAuthority !== false || carriesBusinessAuthority !== false) {
    throw new TypeError('recovery projection authority contract violated');
  }

  const observedMs = Date.parse(observedAt);
  const validUntilMs = Date.parse(validUntil);
  const nowMs = now instanceof Date ? now.getTime() : Number(now);
  if (!Number.isFinite(observedMs) || !Number.isFinite(validUntilMs) || !Number.isFinite(nowMs) || validUntilMs <= observedMs) {
    throw new TypeError('recovery projection time is invalid');
  }
  if (ready !== expectedReady(recoveryState)) throw new TypeError('ready-for-release state mismatch');
  if (authorization !== expectedAuthorization(recoveryState)) throw new TypeError('release authorization state mismatch');
  if (execution !== expectedExecution(recoveryState)) throw new TypeError('release execution state mismatch');
  if (reintroduction !== expectedReintroduction(recoveryState)) throw new TypeError('reintroduction state mismatch');

  const effectiveFreshness = freshness === 'Stale' || nowMs > validUntilMs ? FreshnessState.STALE : FreshnessState.CURRENT;
  const releaseDecisionReadiness = ready ? 'READY_FOR_RELEASE_DECISION' : 'NOT_READY';
  const releaseAuthorizationState = authorization === 'Authorized' ? 'AUTHORIZED' : authorization === 'Denied' ? 'DENIED' : 'NOT_AUTHORIZED';
  const releaseExecutionState = execution === 'Executed' ? 'EXECUTED' : 'NOT_EXECUTED';

  return Object.freeze({
    truth: effectiveFreshness === FreshnessState.CURRENT ? TruthState.CURRENT : TruthState.LAST_KNOWN,
    freshness: effectiveFreshness,
    completeness: complete ? CompletenessState.COMPLETE : CompletenessState.PARTIAL,
    availability: AvailabilityState.AVAILABLE,
    recoveryState,
    restorationOutcome,
    releaseDecisionReadiness,
    releaseAuthorizationState,
    releaseExecutionState,
    reintroductionState: reintroduction,
    lifecycleState,
    recoveryCaseId: recoveryCaseIdentity,
    projectionIdentity,
    evidenceReferences: Object.freeze([evidenceReference]),
    asOfTime: observedAt,
    validUntil,
    source: 'FOUNDATION_GOVERNED_FIL_PROJECTION',
    presentationOnly: true,
    mayAuthorizeRelease: false,
    mayExecuteRelease: false,
    mayChangeLifecycle: false,
    businessAuthorityGranted: false,
    bindingFailureReason: null
  });
}

export function createFoundationStage9RecoveryAdapter({ filProjectionSource, expectedArtifactBinding, now = () => Date.now() } = {}) {
  if (typeof filProjectionSource !== 'function') throw new TypeError('filProjectionSource must be a function');
  if (!expectedArtifactBinding || typeof expectedArtifactBinding !== 'object') throw new TypeError('expectedArtifactBinding must be an object');
  if (typeof now !== 'function') throw new TypeError('now must be a function');

  return Object.freeze({
    async readRecoveryProjection(reference) {
      if (!token(reference)) return unavailable('RECOVERY_PROJECTION_REFERENCE_INVALID');
      const packet = await filProjectionSource(reference);
      if (!packet) return unavailable('FOUNDATION_RECOVERY_FIL_PROJECTION_UNAVAILABLE');

      const consumed = await consumeFoundationRecoveryFilProjection(packet, expectedArtifactBinding, { now:now() });
      if (!consumed.accepted) return unavailable(consumed.reason);

      try {
        return adaptFoundationRecoveryOperationalProjection(consumed.projection, { now:now() });
      } catch (error) {
        return unavailable(`FOUNDATION_RECOVERY_PROJECTION_REJECTED:${error.message}`);
      }
    }
  });
}
