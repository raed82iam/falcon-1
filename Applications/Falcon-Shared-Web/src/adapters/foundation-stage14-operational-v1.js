import { TruthState, FreshnessState, CompletenessState, AvailabilityState } from '../contracts.js';
import { consumeFoundationPublicRuntimeProjection } from './foundation-fil-public-runtime-v1.js';

export const FoundationOperationalFilProfileV1 = Object.freeze({
  contractVersion:'1.0.0',
  producer:'foundation.runtime',
  recipientScope:'shared-web',
  transportAuthority:'authority:transport:projection-only',
  compatibilityIdentity:'compat:foundation-public-runtime-projection:v1',
  messageKind:4,
  artifactState:1,
  routeIdentity:'route:foundation:operational:web:v1',
  messageType:'Foundation.Operational.FoundationProjection',
  schemaIdentity:'foundation.operational.foundation',
  artifactId:'foundation/runtime-projection/operational',
  classification:1,
  acceptedReason:'FOUNDATION_OPERATIONAL_FIL_ACCEPTED'
});

const pick = (value, camel, pascal) => value?.[camel] ?? value?.[pascal];
const referenceToken = value => typeof value === 'string' && value.length > 0 && value.trim() === value && !/[\u0000-\u001f\s]/u.test(value);
const text = value => typeof value === 'string' && value.trim().length > 0 && value.trim() === value && !/[\u0000-\u001f]/u.test(value);
const sha256Artifact = value => typeof value === 'string' && /^sha256\/[0-9a-f]{64}$/iu.test(value);

function unavailable(reason = null) {
  return Object.freeze({
    truth:TruthState.UNAVAILABLE,
    freshness:FreshnessState.UNAVAILABLE,
    completeness:CompletenessState.UNKNOWN,
    availability:AvailabilityState.UNAVAILABLE,
    foundationIdentity:null,
    foundationReleaseState:null,
    healthState:null,
    authorityState:null,
    lifecycleState:null,
    applicationCount:null,
    projectionIdentity:null,
    evidenceReferences:Object.freeze([]),
    asOfTime:null,
    source:null,
    presentationOnly:true,
    mayRepair:false,
    mayAllocateResources:false,
    mayChangeLifecycle:false,
    businessAuthorityGranted:false,
    bindingFailureReason:reason
  });
}

/**
 * Exact Shared Web presentation adapter for FoundationOperationalProjection.
 *
 * It validates the accepted Stage 14 projection after the canonical FIL
 * envelope/binding has been verified. The projection is presentation truth
 * only and cannot create Foundation, repair, resource, lifecycle or business
 * authority inside Web.
 */
export function adaptFoundationOperationalProjection(raw, { now = Date.now() } = {}) {
  if (!raw || typeof raw !== 'object' || Array.isArray(raw)) throw new TypeError('Foundation operational projection is required');

  const projectionIdentity = pick(raw,'projectionIdentity','ProjectionIdentity');
  const foundationIdentity = pick(raw,'foundationIdentity','FoundationIdentity');
  const foundationReleaseState = pick(raw,'foundationReleaseState','FoundationReleaseState');
  const healthState = pick(raw,'healthState','HealthState');
  const authorityState = pick(raw,'authorityState','AuthorityState');
  const lifecycleState = pick(raw,'lifecycleState','LifecycleState');
  const applicationCount = pick(raw,'applicationCount','ApplicationCount');
  const evidenceReference = pick(raw,'evidenceReference','EvidenceReference');
  const observedAt = pick(raw,'observedAt','ObservedAt');
  const presentationOnly = pick(raw,'presentationOnly','PresentationOnly');
  const carriesExecutionAuthority = pick(raw,'carriesExecutionAuthority','CarriesExecutionAuthority');
  const carriesBusinessAuthority = pick(raw,'carriesBusinessAuthority','CarriesBusinessAuthority');

  if (!sha256Artifact(projectionIdentity)) throw new TypeError('projectionIdentity is invalid');
  for (const [name,value] of [
    ['foundationIdentity',foundationIdentity],
    ['foundationReleaseState',foundationReleaseState],
    ['healthState',healthState],
    ['authorityState',authorityState],
    ['lifecycleState',lifecycleState],
    ['evidenceReference',evidenceReference]
  ]) {
    if (!text(value)) throw new TypeError(`${name} is invalid`);
  }
  if (!Number.isInteger(applicationCount) || applicationCount < 0) throw new TypeError('applicationCount is invalid');
  if (presentationOnly !== true || carriesExecutionAuthority !== false || carriesBusinessAuthority !== false) {
    throw new TypeError('operational projection authority contract violated');
  }

  const observedMs = Date.parse(observedAt);
  const nowMs = now instanceof Date ? now.getTime() : Number(now);
  if (!Number.isFinite(observedMs) || !Number.isFinite(nowMs) || observedMs > nowMs) {
    throw new TypeError('operational projection time is invalid');
  }

  return Object.freeze({
    truth:TruthState.CURRENT,
    freshness:FreshnessState.CURRENT,
    completeness:CompletenessState.COMPLETE,
    availability:AvailabilityState.AVAILABLE,
    foundationIdentity,
    foundationReleaseState,
    healthState,
    authorityState,
    lifecycleState,
    applicationCount,
    projectionIdentity,
    evidenceReferences:Object.freeze([evidenceReference]),
    asOfTime:observedAt,
    source:'FOUNDATION_GOVERNED_FIL_PROJECTION',
    presentationOnly:true,
    mayRepair:false,
    mayAllocateResources:false,
    mayChangeLifecycle:false,
    businessAuthorityGranted:false,
    bindingFailureReason:null
  });
}

export function createFoundationStage14OperationalAdapter({ filProjectionSource, expectedArtifactBinding, now = () => Date.now() } = {}) {
  if (typeof filProjectionSource !== 'function') throw new TypeError('filProjectionSource must be a function');
  if (!expectedArtifactBinding || typeof expectedArtifactBinding !== 'object') throw new TypeError('expectedArtifactBinding must be an object');
  if (typeof now !== 'function') throw new TypeError('now must be a function');

  return Object.freeze({
    async readOperationalProjection(reference) {
      if (!referenceToken(reference)) return unavailable('OPERATIONAL_PROJECTION_REFERENCE_INVALID');
      const packet = await filProjectionSource(reference);
      if (!packet) return unavailable('FOUNDATION_OPERATIONAL_FIL_PROJECTION_UNAVAILABLE');

      const consumed = await consumeFoundationPublicRuntimeProjection(
        packet,
        FoundationOperationalFilProfileV1,
        expectedArtifactBinding,
        { now:now() }
      );
      if (!consumed.accepted) return unavailable(consumed.reason);

      try {
        return adaptFoundationOperationalProjection(consumed.projection,{ now:now() });
      } catch (error) {
        return unavailable(`FOUNDATION_OPERATIONAL_PROJECTION_REJECTED:${error.message}`);
      }
    }
  });
}
