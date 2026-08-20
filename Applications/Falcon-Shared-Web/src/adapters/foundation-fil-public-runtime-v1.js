const COMMON = Object.freeze({
  contractVersion:'1.0.0',
  producer:'foundation.runtime',
  recipientScope:'shared-web',
  transportAuthority:'authority:transport:projection-only',
  compatibilityIdentity:'compat:foundation-public-runtime-projection:v1',
  messageKind:4,
  artifactState:1
});

const IDENTITY_PROFILE = Object.freeze({
  ...COMMON,
  routeIdentity:'route:foundation:identity:web:v1',
  messageType:'Foundation.Security.IdentityContextProjection',
  schemaIdentity:'foundation.security.identity-context',
  artifactId:'foundation/runtime-projection/identity-security-context',
  classification:5,
  acceptedReason:'FOUNDATION_IDENTITY_FIL_ACCEPTED'
});

const RECOVERY_PROFILE = Object.freeze({
  ...COMMON,
  routeIdentity:'route:foundation:recovery:web:v1',
  messageType:'Foundation.Operational.RecoveryProjection',
  schemaIdentity:'foundation.operational.recovery',
  artifactId:'foundation/runtime-projection/recovery',
  classification:1,
  acceptedReason:'FOUNDATION_RECOVERY_FIL_ACCEPTED'
});

const pick = (value, ...names) => {
  for (const name of names) if (value && value[name] !== undefined) return value[name];
  return undefined;
};

const scalar = value => typeof value === 'string' ? value : pick(value, 'value', 'Value');
const nonEmpty = value => typeof value === 'string' && value.length > 0 && value.trim() === value && !/[\u0000-\u001f\s]/u.test(value);
const sha256Artifact = value => typeof value === 'string' && /^sha256\/[0-9a-f]{64}$/iu.test(value);
const sha256HexValue = value => typeof value === 'string' && /^[0-9a-f]{64}$/iu.test(value);
const version = value => typeof value === 'string' && /^(0|[1-9]\d*)\.(0|[1-9]\d*)(?:\.(0|[1-9]\d*))?$/u.test(value);

function enumNumber(value, names) {
  if (Number.isInteger(value)) return value;
  if (typeof value !== 'string') return null;
  const key = value.toUpperCase();
  return names[key] ?? null;
}

const MESSAGE_KIND = Object.freeze({ EVENT:4, RESPONSE:3, NOTICE:5, COMMAND:1, QUERY:2 });
const CLASSIFICATION = Object.freeze({ OPERATIONAL:1, SECURITY:5 });
const OUTCOME = Object.freeze({ SUCCEEDED:1, SUCCESS:1 });

function reject(reason) {
  return Object.freeze({ accepted:false, reason, projection:null, profile:null, binding:null });
}

function utcInstant(value) {
  if (typeof value !== 'string') return null;
  const time = Date.parse(value);
  return Number.isFinite(time) ? time : null;
}

function byteLength(value) {
  return new TextEncoder().encode(value).length;
}

function append(builder, name, value) {
  return `${builder}${name.length}:${name}=${byteLength(value)}:${value}\n`;
}

async function sha256Hex(value) {
  const subtle = globalThis.crypto?.subtle;
  if (!subtle) throw new Error('CRYPTO_UNAVAILABLE');
  const digest = await subtle.digest('SHA-256', new TextEncoder().encode(value));
  return Array.from(new Uint8Array(digest), byte => byte.toString(16).padStart(2, '0')).join('').toUpperCase();
}

function normalizeEnvelope(raw) {
  const time = pick(raw, 'time', 'Time') ?? {};
  const outcome = pick(raw, 'outcome', 'Outcome') ?? {};
  return Object.freeze({
    messageId:scalar(pick(raw,'messageId','MessageId')),
    messageKind:enumNumber(pick(raw,'messageKind','MessageKind'), MESSAGE_KIND),
    classification:enumNumber(pick(raw,'classification','Classification'), CLASSIFICATION),
    messageType:pick(raw,'messageType','MessageType'),
    schemaId:scalar(pick(raw,'schemaId','SchemaId')),
    schemaVersion:pick(raw,'schemaVersion','SchemaVersion'),
    producer:scalar(pick(raw,'producer','Producer')),
    recipientScope:scalar(pick(raw,'recipientScope','RecipientScope')),
    correlationId:scalar(pick(raw,'correlationId','CorrelationId')),
    causationId:scalar(pick(raw,'causationId','CausationId')) ?? null,
    authority:scalar(pick(raw,'authority','Authority')),
    provenance:scalar(pick(raw,'provenance','Provenance')),
    idempotencyId:scalar(pick(raw,'idempotencyId','IdempotencyId')),
    deliveryAttemptId:scalar(pick(raw,'deliveryAttemptId','DeliveryAttemptId')),
    retryLineageId:scalar(pick(raw,'retryLineageId','RetryLineageId')),
    createdAt:pick(time,'createdAt','CreatedAt'),
    expiresAt:pick(time,'expiresAt','ExpiresAt'),
    outcomeCode:enumNumber(pick(outcome,'code','Code'), OUTCOME),
    outcomeReason:pick(outcome,'reason','Reason'),
    payload:pick(raw,'payload','Payload'),
    payloadSha256:pick(raw,'payloadSha256','PayloadSha256')
  });
}

function normalizeBinding(raw) {
  return Object.freeze({
    bindingIdentity:pick(raw,'bindingIdentity','BindingIdentity'),
    routeIdentity:pick(raw,'routeIdentity','RouteIdentity'),
    artifactId:pick(raw,'artifactId','ArtifactId'),
    artifactVersion:pick(raw,'artifactVersion','ArtifactVersion'),
    artifactSha256:pick(raw,'artifactSha256','ArtifactSha256'),
    evidenceReference:pick(raw,'evidenceReference','EvidenceReference'),
    compatibilityIdentity:pick(raw,'compatibilityIdentity','CompatibilityIdentity'),
    sourceProvenance:pick(raw,'sourceProvenance','SourceProvenance'),
    payloadSha256:pick(raw,'payloadSha256','PayloadSha256')
  });
}

async function expectedBindingIdentity(profile, binding, payloadSha256) {
  let canonical = '';
  canonical = append(canonical, 'route_identity', profile.routeIdentity);
  canonical = append(canonical, 'message_type', profile.messageType);
  canonical = append(canonical, 'schema_id', profile.schemaIdentity);
  canonical = append(canonical, 'schema_version', profile.contractVersion);
  canonical = append(canonical, 'producer', profile.producer);
  canonical = append(canonical, 'recipient_scope', profile.recipientScope);
  canonical = append(canonical, 'message_kind', String(profile.messageKind));
  canonical = append(canonical, 'classification', String(profile.classification));
  canonical = append(canonical, 'transport_authority', profile.transportAuthority);
  canonical = append(canonical, 'source_provenance', binding.sourceProvenance);
  canonical = append(canonical, 'artifact_id', profile.artifactId);
  canonical = append(canonical, 'artifact_version', profile.contractVersion);
  canonical = append(canonical, 'artifact_sha256', binding.artifactSha256.toUpperCase());
  canonical = append(canonical, 'evidence_reference', binding.evidenceReference);
  canonical = append(canonical, 'compatibility_identity', profile.compatibilityIdentity);
  canonical = append(canonical, 'artifact_state', String(profile.artifactState));
  canonical = append(canonical, 'payload_sha256', payloadSha256);
  return `sha256/${await sha256Hex(canonical)}`;
}

function validProfile(profile) {
  return Boolean(profile
    && nonEmpty(profile.routeIdentity)
    && nonEmpty(profile.messageType)
    && nonEmpty(profile.schemaIdentity)
    && version(profile.contractVersion)
    && nonEmpty(profile.producer)
    && nonEmpty(profile.recipientScope)
    && nonEmpty(profile.transportAuthority)
    && nonEmpty(profile.artifactId)
    && nonEmpty(profile.compatibilityIdentity)
    && profile.messageKind === MESSAGE_KIND.EVENT
    && [CLASSIFICATION.OPERATIONAL, CLASSIFICATION.SECURITY].includes(profile.classification)
    && profile.artifactState === 1);
}

export const FoundationIdentityFilProfileV1 = IDENTITY_PROFILE;
export const FoundationRecoveryFilProfileV1 = RECOVERY_PROFILE;

export async function consumeFoundationPublicRuntimeProjection(packet, profile, expectedArtifact, { now = Date.now() } = {}) {
  if (!validProfile(profile)) return reject('FIL_PROFILE_INVALID');
  if (!packet || typeof packet !== 'object') return reject('FIL_PACKET_MISSING');

  const decisionAccepted = pick(packet,'accepted','Accepted');
  const activationAuthorized = pick(packet,'activationAuthorized','ActivationAuthorized');
  const executionAuthorized = pick(packet,'executionAuthorized','ExecutionAuthorized');
  const businessAuthorityGranted = pick(packet,'businessAuthorityGranted','BusinessAuthorityGranted');
  if (decisionAccepted !== true) return reject('FIL_TRANSPORT_DECISION_NOT_ACCEPTED');
  if (activationAuthorized !== false || executionAuthorized !== false || businessAuthorityGranted !== false) return reject('FIL_TRANSPORT_AUTHORITY_CONTRACT_VIOLATION');

  const envelope = normalizeEnvelope(packet.envelope ?? packet.Envelope);
  const binding = normalizeBinding(packet.binding ?? packet.Binding);
  if (!expectedArtifact || typeof expectedArtifact !== 'object') return reject('EXPECTED_ARTIFACT_BINDING_REQUIRED');

  if (envelope.messageKind !== profile.messageKind) return reject('FIL_MESSAGE_KIND_MISMATCH');
  if (envelope.classification !== profile.classification) return reject('FIL_CLASSIFICATION_MISMATCH');
  if (envelope.messageType !== profile.messageType) return reject('FIL_MESSAGE_TYPE_MISMATCH');
  if (envelope.schemaId !== profile.schemaIdentity) return reject('FIL_SCHEMA_ID_MISMATCH');
  if (envelope.schemaVersion !== profile.contractVersion) return reject('FIL_SCHEMA_VERSION_MISMATCH');
  if (envelope.producer !== profile.producer) return reject('FIL_PRODUCER_MISMATCH');
  if (envelope.recipientScope !== profile.recipientScope) return reject('FIL_RECIPIENT_MISMATCH');
  if (envelope.authority !== profile.transportAuthority) return reject('FIL_TRANSPORT_AUTHORITY_MISMATCH');
  if (envelope.outcomeCode !== 1 || !nonEmpty(envelope.outcomeReason)) return reject('FIL_OUTCOME_INVALID');
  for (const value of [envelope.messageId,envelope.correlationId,envelope.idempotencyId,envelope.deliveryAttemptId,envelope.retryLineageId]) {
    if (!nonEmpty(value)) return reject('FIL_IDENTITY_FIELD_INVALID');
  }
  if (envelope.causationId !== null && (!nonEmpty(envelope.causationId) || envelope.causationId === envelope.correlationId)) return reject('FIL_CAUSATION_INVALID');

  const nowMs = now instanceof Date ? now.getTime() : Number(now);
  const createdAt = utcInstant(envelope.createdAt);
  const expiresAt = utcInstant(envelope.expiresAt);
  if (!Number.isFinite(nowMs) || createdAt === null || expiresAt === null || createdAt > nowMs || expiresAt <= nowMs || expiresAt <= createdAt) return reject('FIL_MESSAGE_NOT_CURRENT');
  if (typeof envelope.payload !== 'string' || envelope.payload.length === 0) return reject('FIL_PAYLOAD_MISSING');
  if (!sha256HexValue(envelope.payloadSha256)) return reject('FIL_PAYLOAD_DIGEST_INVALID');

  let payloadDigest;
  try { payloadDigest = await sha256Hex(envelope.payload); }
  catch { return reject('FIL_CRYPTO_UNAVAILABLE'); }
  if (payloadDigest !== envelope.payloadSha256.toUpperCase()) return reject('FIL_PAYLOAD_DIGEST_MISMATCH');

  if (binding.routeIdentity !== profile.routeIdentity) return reject('FIL_ROUTE_IDENTITY_MISMATCH');
  if (binding.artifactId !== profile.artifactId) return reject('FIL_ARTIFACT_ID_MISMATCH');
  if (binding.artifactVersion !== profile.contractVersion || !version(binding.artifactVersion)) return reject('FIL_ARTIFACT_VERSION_MISMATCH');
  if (!sha256Artifact(binding.artifactSha256)) return reject('FIL_ARTIFACT_DIGEST_INVALID');
  if (binding.compatibilityIdentity !== profile.compatibilityIdentity) return reject('FIL_COMPATIBILITY_MISMATCH');
  if (!nonEmpty(binding.evidenceReference) || !nonEmpty(binding.sourceProvenance)) return reject('FIL_BINDING_EVIDENCE_INVALID');
  if (binding.payloadSha256?.toUpperCase() !== payloadDigest) return reject('FIL_BINDING_PAYLOAD_DIGEST_MISMATCH');

  const expectedSha = expectedArtifact.sha256 ?? expectedArtifact.artifactSha256;
  const expectedEvidence = expectedArtifact.evidenceReference;
  const expectedProvenance = expectedArtifact.sourceProvenance ?? expectedArtifact.provenanceReference;
  if (!sha256Artifact(expectedSha) || binding.artifactSha256.toUpperCase() !== expectedSha.toUpperCase()) return reject('FIL_EXPECTED_ARTIFACT_DIGEST_MISMATCH');
  if (!nonEmpty(expectedEvidence) || binding.evidenceReference !== expectedEvidence) return reject('FIL_EXPECTED_EVIDENCE_MISMATCH');
  if (!nonEmpty(expectedProvenance) || binding.sourceProvenance !== expectedProvenance) return reject('FIL_EXPECTED_PROVENANCE_MISMATCH');

  let computedBinding;
  try { computedBinding = await expectedBindingIdentity(profile, binding, payloadDigest); }
  catch { return reject('FIL_CRYPTO_UNAVAILABLE'); }
  if (binding.bindingIdentity !== computedBinding) return reject('FIL_BINDING_IDENTITY_MISMATCH');
  if (envelope.provenance !== `projection-binding:${computedBinding}`) return reject('FIL_ENVELOPE_PROVENANCE_MISMATCH');

  let projection;
  try { projection = JSON.parse(envelope.payload); }
  catch { return reject('FIL_PAYLOAD_JSON_INVALID'); }
  if (!projection || Array.isArray(projection) || typeof projection !== 'object') return reject('FIL_PROJECTION_OBJECT_INVALID');

  return Object.freeze({ accepted:true, reason:profile.acceptedReason, projection:Object.freeze(projection), profile, binding:Object.freeze(binding) });
}

export const consumeFoundationIdentityFilProjection = (packet, expectedArtifact, options = {}) =>
  consumeFoundationPublicRuntimeProjection(packet, IDENTITY_PROFILE, expectedArtifact, options);

export const consumeFoundationRecoveryFilProjection = (packet, expectedArtifact, options = {}) =>
  consumeFoundationPublicRuntimeProjection(packet, RECOVERY_PROFILE, expectedArtifact, options);
