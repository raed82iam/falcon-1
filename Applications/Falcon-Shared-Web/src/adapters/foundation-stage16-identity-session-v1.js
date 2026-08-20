import { AuthResult, WebSurfaceGrant } from '../auth.js';
import { consumeFoundationIdentityFilProjection } from './foundation-fil-public-runtime-v1.js';

const nonEmpty = value => typeof value === 'string' && value.trim().length > 0;
const asArray = value => Array.isArray(value) ? value : null;
const uniqueNonEmpty = value => {
  const items = asArray(value);
  if (!items) return null;
  const seen = new Set();
  for (const item of items) {
    if (!nonEmpty(item) || seen.has(item)) return null;
    seen.add(item);
  }
  return Object.freeze([...items]);
};

function parseInstant(value) {
  if (!nonEmpty(value)) return null;
  const time = Date.parse(value);
  return Number.isFinite(time) ? time : null;
}

function normalizeProjection(raw = {}) {
  return Object.freeze({
    falconIdentityId:raw.falconIdentityId ?? raw.FalconIdentityId ?? null,
    authenticationMethod:raw.authenticationMethod ?? raw.AuthenticationMethod ?? null,
    assurance:raw.assurance ?? raw.Assurance ?? null,
    sessionId:raw.sessionId ?? raw.SessionId ?? null,
    trustBoundary:raw.trustBoundary ?? raw.TrustBoundary ?? null,
    issuedAt:raw.issuedAt ?? raw.IssuedAt ?? null,
    expiresAt:raw.expiresAt ?? raw.ExpiresAt ?? null,
    revoked:raw.revoked ?? raw.Revoked ?? null,
    provenanceEvidenceId:raw.provenanceEvidenceId ?? raw.ProvenanceEvidenceId ?? null,
    roleIds:raw.roleIds ?? raw.RoleIds ?? null,
    entitlementIds:raw.entitlementIds ?? raw.EntitlementIds ?? null,
    grantsBusinessAuthority:raw.grantsBusinessAuthority ?? raw.GrantsBusinessAuthority ?? null
  });
}

function reject(reason) {
  return Object.freeze({
    state:AuthResult.REJECTED,
    authoritativeSession:false,
    principalId:null,
    sessionId:null,
    role:null,
    applications:[],
    capabilities:[],
    surfaceGrants:[],
    businessAuthorityGranted:false,
    reason
  });
}

export function adaptFoundationStage16SecurityContext(rawProjection, accessBinding = null, { now = Date.now() } = {}) {
  const projection = normalizeProjection(rawProjection);
  const roleIds = uniqueNonEmpty(projection.roleIds);
  const entitlementIds = uniqueNonEmpty(projection.entitlementIds);
  const issuedAt = parseInstant(projection.issuedAt);
  const expiresAt = parseInstant(projection.expiresAt);
  const nowMs = now instanceof Date ? now.getTime() : Number(now);

  if (!nonEmpty(projection.falconIdentityId)) return reject('FALCON_IDENTITY_MISSING');
  if (!nonEmpty(projection.authenticationMethod)) return reject('AUTHENTICATION_METHOD_MISSING');
  if (projection.assurance === null || projection.assurance === undefined || String(projection.assurance).trim() === '') return reject('ASSURANCE_MISSING');
  if (!nonEmpty(projection.sessionId)) return reject('SESSION_ID_MISSING');
  if (!nonEmpty(projection.trustBoundary)) return reject('TRUST_BOUNDARY_MISSING');
  if (!nonEmpty(projection.provenanceEvidenceId)) return reject('PROVENANCE_EVIDENCE_MISSING');
  if (!roleIds) return reject('ROLE_FACTS_INVALID');
  if (!entitlementIds) return reject('ENTITLEMENT_FACTS_INVALID');
  if (!Number.isFinite(nowMs) || issuedAt === null || expiresAt === null || issuedAt > nowMs || expiresAt <= nowMs || expiresAt <= issuedAt) return reject('SESSION_NOT_CURRENT');
  if (projection.revoked !== false) return reject('SESSION_REVOKED_OR_UNKNOWN');
  if (projection.grantsBusinessAuthority !== false) return reject('BUSINESS_AUTHORITY_CONTRACT_VIOLATION');

  if (!accessBinding || accessBinding.authorized !== true) return reject('WEB_ACCESS_BINDING_REQUIRED');
  if (accessBinding.falconIdentityId !== projection.falconIdentityId) return reject('WEB_ACCESS_IDENTITY_MISMATCH');
  if (accessBinding.sessionId !== projection.sessionId) return reject('WEB_ACCESS_SESSION_MISMATCH');
  if (!nonEmpty(accessBinding.evidenceId)) return reject('WEB_ACCESS_EVIDENCE_MISSING');

  const surfaceGrants = uniqueNonEmpty(accessBinding.surfaceGrants);
  const applications = uniqueNonEmpty(accessBinding.applications ?? []);
  const capabilities = uniqueNonEmpty(accessBinding.capabilities ?? []);
  if (!surfaceGrants || surfaceGrants.some(grant => !Object.values(WebSurfaceGrant).includes(grant))) return reject('WEB_SURFACE_GRANTS_INVALID');
  if (!applications) return reject('WEB_APPLICATION_BINDING_INVALID');
  if (!capabilities) return reject('WEB_CAPABILITY_BINDING_INVALID');

  const role = accessBinding.role ?? null;
  if (role !== null && !nonEmpty(role)) return reject('WEB_ROLE_BINDING_INVALID');
  if (role === 'PROJECT_OWNER' && !roleIds.includes('PROJECT_OWNER')) return reject('OWNER_ROLE_FACT_MISSING');
  if (role === 'CUSTOMER' && !roleIds.includes('CUSTOMER')) return reject('CUSTOMER_ROLE_FACT_MISSING');
  if (surfaceGrants.includes(WebSurfaceGrant.OWNER) && role !== 'PROJECT_OWNER') return reject('OWNER_SURFACE_ROLE_MISMATCH');
  if (surfaceGrants.includes(WebSurfaceGrant.CUSTOMER) && role !== 'CUSTOMER') return reject('CUSTOMER_SURFACE_ROLE_MISMATCH');

  const ownerIdentityGovernanceVersion = nonEmpty(accessBinding.ownerIdentityGovernanceVersion)
    ? accessBinding.ownerIdentityGovernanceVersion
    : null;

  return Object.freeze({
    state:AuthResult.AUTHENTICATED,
    provider:null,
    authoritativeSession:true,
    principalId:projection.falconIdentityId,
    sessionId:projection.sessionId,
    role,
    applications,
    capabilities,
    surfaceGrants,
    businessAuthorityGranted:false,
    ownerIdentityGovernanceVersion,
    authenticationMethod:projection.authenticationMethod,
    assurance:projection.assurance,
    trustBoundary:projection.trustBoundary,
    issuedAt:projection.issuedAt,
    expiresAt:projection.expiresAt,
    provenanceEvidenceId:projection.provenanceEvidenceId,
    roleFacts:roleIds,
    entitlementFacts:entitlementIds,
    webAccessEvidenceId:accessBinding.evidenceId
  });
}

/**
 * Final Falcon-native Stage 16 consuming adapter.
 *
 * filProjectionSource returns the canonical FIL transport packet produced by
 * Foundation PublicRuntimeProjectionTransport. expectedArtifactBinding is the
 * exact Stage 14/canonical consumption identity selected by the Web runtime.
 * No live route activation is inferred by this adapter.
 */
export function createFoundationStage16SessionAdapter({ filProjectionSource, expectedArtifactBinding, webAccessResolver, now = () => Date.now() } = {}) {
  if (typeof filProjectionSource !== 'function') throw new TypeError('filProjectionSource must be a function');
  if (!expectedArtifactBinding || typeof expectedArtifactBinding !== 'object') throw new TypeError('expectedArtifactBinding must be an object');
  if (typeof webAccessResolver !== 'function') throw new TypeError('webAccessResolver must be a function');
  if (typeof now !== 'function') throw new TypeError('now must be a function');

  return Object.freeze({
    async resumeSession(sessionReference) {
      if (!nonEmpty(sessionReference)) return reject('SESSION_REFERENCE_MISSING');
      const packet = await filProjectionSource(sessionReference);
      if (!packet) return reject('FOUNDATION_FIL_PROJECTION_UNAVAILABLE');

      const consumed = await consumeFoundationIdentityFilProjection(packet, expectedArtifactBinding, { now:now() });
      if (!consumed.accepted) return reject(consumed.reason);

      const normalized = normalizeProjection(consumed.projection);
      const accessBinding = await webAccessResolver(Object.freeze({
        falconIdentityId:normalized.falconIdentityId,
        sessionId:normalized.sessionId,
        roleIds:uniqueNonEmpty(normalized.roleIds) ?? [],
        entitlementIds:uniqueNonEmpty(normalized.entitlementIds) ?? [],
        filProfile:consumed.profile,
        filBindingIdentity:consumed.binding.bindingIdentity
      }));
      return adaptFoundationStage16SecurityContext(consumed.projection, accessBinding, { now:now() });
    }
  });
}
