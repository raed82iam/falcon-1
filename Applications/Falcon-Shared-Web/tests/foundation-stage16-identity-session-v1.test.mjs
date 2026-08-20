import test from 'node:test';
import assert from 'node:assert/strict';
import { createHash } from 'node:crypto';
import { AuthResult, WebSurfaceGrant, canAccessRoute, routeAuthenticatedIdentity } from '../src/auth.js';
import { consumeFoundationIdentityFilProjection, FoundationIdentityFilProfileV1 } from '../src/adapters/foundation-fil-public-runtime-v1.js';
import { adaptFoundationStage16SecurityContext, createFoundationStage16SessionAdapter } from '../src/adapters/foundation-stage16-identity-session-v1.js';

const now = Date.parse('2026-08-17T20:00:00Z');
const artifactSha = `sha256/${'B'.repeat(64)}`;
const evidenceReference = 'evidence:foundation:identity-context';
const sourceProvenance = 'provenance:foundation:runtime';
const expectedArtifactBinding = Object.freeze({ sha256:artifactSha, evidenceReference, sourceProvenance });

const projection = Object.freeze({
  FalconIdentityId:'falcon-owner-1',
  AuthenticationMethod:'OIDC',
  Assurance:'High',
  SessionId:'session-stage16-1',
  TrustBoundary:'WEB',
  IssuedAt:'2026-08-17T19:55:00Z',
  ExpiresAt:'2026-08-17T21:00:00Z',
  Revoked:false,
  ProvenanceEvidenceId:'evidence-stage16-session-1',
  RoleIds:['PROJECT_OWNER'],
  EntitlementIds:['FSATS_OWNER_FEATURE_ACCESS'],
  GrantsBusinessAuthority:false
});

const ownerBinding = Object.freeze({
  authorized:true,
  falconIdentityId:'falcon-owner-1',
  sessionId:'session-stage16-1',
  role:'PROJECT_OWNER',
  surfaceGrants:[WebSurfaceGrant.OWNER],
  applications:['FSATS'],
  capabilities:['INCIDENT_SUPPORT_TAKEOVER'],
  evidenceId:'web-access-evidence-1'
});

const sha256 = value => createHash('sha256').update(value,'utf8').digest('hex').toUpperCase();
const append = (builder,name,value) => `${builder}${name.length}:${name}=${Buffer.byteLength(value,'utf8')}:${value}\n`;

function makeFilPacket(overrides = {}) {
  const payload = overrides.payload ?? JSON.stringify(projection);
  const payloadSha256 = sha256(payload);
  const artifact = (overrides.artifactSha256 ?? artifactSha).toUpperCase();
  const evidence = overrides.evidenceReference ?? evidenceReference;
  const provenance = overrides.sourceProvenance ?? sourceProvenance;
  const route = overrides.routeIdentity ?? FoundationIdentityFilProfileV1.routeIdentity;
  const recipient = overrides.recipientScope ?? FoundationIdentityFilProfileV1.recipientScope;

  let canonical='';
  canonical=append(canonical,'route_identity',route);
  canonical=append(canonical,'message_type',FoundationIdentityFilProfileV1.messageType);
  canonical=append(canonical,'schema_id',FoundationIdentityFilProfileV1.schemaIdentity);
  canonical=append(canonical,'schema_version',FoundationIdentityFilProfileV1.contractVersion);
  canonical=append(canonical,'producer',FoundationIdentityFilProfileV1.producer);
  canonical=append(canonical,'recipient_scope',recipient);
  canonical=append(canonical,'message_kind','4');
  canonical=append(canonical,'classification','5');
  canonical=append(canonical,'transport_authority',FoundationIdentityFilProfileV1.transportAuthority);
  canonical=append(canonical,'source_provenance',provenance);
  canonical=append(canonical,'artifact_id',FoundationIdentityFilProfileV1.artifactId);
  canonical=append(canonical,'artifact_version',FoundationIdentityFilProfileV1.contractVersion);
  canonical=append(canonical,'artifact_sha256',artifact);
  canonical=append(canonical,'evidence_reference',evidence);
  canonical=append(canonical,'compatibility_identity',FoundationIdentityFilProfileV1.compatibilityIdentity);
  canonical=append(canonical,'artifact_state','1');
  canonical=append(canonical,'payload_sha256',payloadSha256);
  const bindingIdentity=`sha256/${sha256(canonical)}`;

  return {
    Accepted:overrides.accepted ?? true,
    ActivationAuthorized:overrides.activationAuthorized ?? false,
    ExecutionAuthorized:overrides.executionAuthorized ?? false,
    BusinessAuthorityGranted:overrides.businessAuthorityGranted ?? false,
    Envelope:{
      MessageId:{Value:'message:identity:1'},
      MessageKind:overrides.messageKind ?? 4,
      Classification:overrides.classification ?? 5,
      MessageType:overrides.messageType ?? FoundationIdentityFilProfileV1.messageType,
      SchemaId:{Value:overrides.schemaId ?? FoundationIdentityFilProfileV1.schemaIdentity},
      SchemaVersion:overrides.schemaVersion ?? FoundationIdentityFilProfileV1.contractVersion,
      Producer:{Value:overrides.producer ?? FoundationIdentityFilProfileV1.producer},
      RecipientScope:{Value:recipient},
      CorrelationId:{Value:'correlation:identity:1'},
      CausationId:{Value:'causation:identity:1'},
      Authority:{Value:overrides.authority ?? FoundationIdentityFilProfileV1.transportAuthority},
      Provenance:{Value:overrides.envelopeProvenance ?? `projection-binding:${bindingIdentity}`},
      IdempotencyId:{Value:'idempotency:identity:1'},
      DeliveryAttemptId:{Value:'delivery:identity:1'},
      RetryLineageId:{Value:'retry:identity:1'},
      Time:{CreatedAt:'2026-08-17T19:59:00Z',ExpiresAt:'2026-08-17T20:05:00Z'},
      Outcome:{Code:1,Reason:'authoritative_public_runtime_projection'},
      Payload:payload,
      PayloadSha256:overrides.payloadSha256 ?? payloadSha256
    },
    Binding:{
      BindingIdentity:overrides.bindingIdentity ?? bindingIdentity,
      RouteIdentity:route,
      ArtifactId:FoundationIdentityFilProfileV1.artifactId,
      ArtifactVersion:FoundationIdentityFilProfileV1.contractVersion,
      ArtifactSha256:artifact,
      EvidenceReference:evidence,
      CompatibilityIdentity:FoundationIdentityFilProfileV1.compatibilityIdentity,
      SourceProvenance:provenance,
      PayloadSha256:payloadSha256
    }
  };
}

test('canonical Stage 16 FIL transport decision is accepted and yields Security Context projection', async () => {
  const result=await consumeFoundationIdentityFilProjection(makeFilPacket(),expectedArtifactBinding,{now});
  assert.equal(result.accepted,true);
  assert.equal(result.projection.FalconIdentityId,'falcon-owner-1');
  assert.equal(result.profile.routeIdentity,'route:foundation:identity:web:v1');
});

test('FIL transport decision must be accepted and must grant no activation execution or business authority', async () => {
  assert.equal((await consumeFoundationIdentityFilProjection(makeFilPacket({accepted:false}),expectedArtifactBinding,{now})).reason,'FIL_TRANSPORT_DECISION_NOT_ACCEPTED');
  assert.equal((await consumeFoundationIdentityFilProjection(makeFilPacket({activationAuthorized:true}),expectedArtifactBinding,{now})).reason,'FIL_TRANSPORT_AUTHORITY_CONTRACT_VIOLATION');
  assert.equal((await consumeFoundationIdentityFilProjection(makeFilPacket({executionAuthorized:true}),expectedArtifactBinding,{now})).reason,'FIL_TRANSPORT_AUTHORITY_CONTRACT_VIOLATION');
  assert.equal((await consumeFoundationIdentityFilProjection(makeFilPacket({businessAuthorityGranted:true}),expectedArtifactBinding,{now})).reason,'FIL_TRANSPORT_AUTHORITY_CONTRACT_VIOLATION');
});

test('FIL identity binding fails closed on route recipient payload artifact and provenance mutation', async () => {
  assert.equal((await consumeFoundationIdentityFilProjection(makeFilPacket({routeIdentity:'route:foundation:identity:web:v2'}),expectedArtifactBinding,{now})).reason,'FIL_ROUTE_IDENTITY_MISMATCH');
  assert.equal((await consumeFoundationIdentityFilProjection(makeFilPacket({recipientScope:'other-web'}),expectedArtifactBinding,{now})).reason,'FIL_RECIPIENT_MISMATCH');
  const payloadMutation=makeFilPacket(); payloadMutation.Envelope.Payload=JSON.stringify({...projection,FalconIdentityId:'attacker'});
  assert.equal((await consumeFoundationIdentityFilProjection(payloadMutation,expectedArtifactBinding,{now})).reason,'FIL_PAYLOAD_DIGEST_MISMATCH');
  assert.equal((await consumeFoundationIdentityFilProjection(makeFilPacket({artifactSha256:`sha256/${'C'.repeat(64)}`}),expectedArtifactBinding,{now})).reason,'FIL_EXPECTED_ARTIFACT_DIGEST_MISMATCH');
  const provenanceMutation=makeFilPacket({envelopeProvenance:'projection-binding:sha256/INVALID'});
  assert.equal((await consumeFoundationIdentityFilProjection(provenanceMutation,expectedArtifactBinding,{now})).reason,'FIL_ENVELOPE_PROVENANCE_MISMATCH');
});

test('FIL control-message substitution and stale envelope are rejected', async () => {
  assert.equal((await consumeFoundationIdentityFilProjection(makeFilPacket({messageKind:1}),expectedArtifactBinding,{now})).reason,'FIL_MESSAGE_KIND_MISMATCH');
  const stale=makeFilPacket(); stale.Envelope.Time.ExpiresAt='2026-08-17T19:59:59Z';
  assert.equal((await consumeFoundationIdentityFilProjection(stale,expectedArtifactBinding,{now})).reason,'FIL_MESSAGE_NOT_CURRENT');
});

test('Stage 16 Security Context becomes authoritative Web session only with exact Web access binding', () => {
  const session=adaptFoundationStage16SecurityContext(projection,ownerBinding,{now});
  assert.equal(session.state,AuthResult.AUTHENTICATED);
  assert.equal(session.authoritativeSession,true);
  assert.equal(session.businessAuthorityGranted,false);
  assert.equal(routeAuthenticatedIdentity(session),'owner-home');
  assert.equal(canAccessRoute('owner-home',session),true);
  assert.equal(canAccessRoute('owner',session),true);
  assert.equal(canAccessRoute('trader',session),false);
});

test('role fact without separately governed Web access binding fails closed', () => {
  const result=adaptFoundationStage16SecurityContext(projection,null,{now});
  assert.equal(result.state,AuthResult.REJECTED);
  assert.equal(result.reason,'WEB_ACCESS_BINDING_REQUIRED');
});

test('expired revoked or business-authority-bearing contexts are rejected', () => {
  assert.equal(adaptFoundationStage16SecurityContext({...projection,ExpiresAt:'2026-08-17T19:59:59Z'},ownerBinding,{now}).reason,'SESSION_NOT_CURRENT');
  assert.equal(adaptFoundationStage16SecurityContext({...projection,Revoked:true},ownerBinding,{now}).reason,'SESSION_REVOKED_OR_UNKNOWN');
  assert.equal(adaptFoundationStage16SecurityContext({...projection,GrantsBusinessAuthority:true},ownerBinding,{now}).reason,'BUSINESS_AUTHORITY_CONTRACT_VIOLATION');
});

test('final runtime adapter consumes canonical FIL packet and passes binding identity to Web access resolver', async () => {
  let requestedReference=null;
  let seenBindingIdentity=null;
  const adapter=createFoundationStage16SessionAdapter({
    filProjectionSource:async reference => { requestedReference=reference; return makeFilPacket(); },
    expectedArtifactBinding,
    webAccessResolver:async facts => { seenBindingIdentity=facts.filBindingIdentity; return ownerBinding; },
    now:()=>now
  });
  const session=await adapter.resumeSession('opaque-web-session-reference');
  assert.equal(requestedReference,'opaque-web-session-reference');
  assert.match(seenBindingIdentity,/^sha256\/[0-9A-F]{64}$/u);
  assert.equal(session.state,AuthResult.AUTHENTICATED);
  assert.equal(session.businessAuthorityGranted,false);
});

test('final runtime adapter remains fail closed when FIL projection is unavailable', async () => {
  const adapter=createFoundationStage16SessionAdapter({
    filProjectionSource:async()=>null,
    expectedArtifactBinding,
    webAccessResolver:async()=>ownerBinding,
    now:()=>now
  });
  const result=await adapter.resumeSession('opaque-web-session-reference');
  assert.equal(result.state,AuthResult.REJECTED);
  assert.equal(result.reason,'FOUNDATION_FIL_PROJECTION_UNAVAILABLE');
});
