import test from 'node:test';
import assert from 'node:assert/strict';
import { webcrypto } from 'node:crypto';
import {
  FoundationOperationalFilProfileV1,
  adaptFoundationOperationalProjection,
  createFoundationStage14OperationalAdapter
} from '../src/adapters/foundation-stage14-operational-v1.js';

const subtle = webcrypto.subtle;
const profile = FoundationOperationalFilProfileV1;
const now = Date.parse('2026-08-18T00:30:00Z');
const expectedArtifactBinding = Object.freeze({
  sha256:'sha256/' + 'A'.repeat(64),
  evidenceReference:'evidence/foundation-operational-v1',
  sourceProvenance:'commit/' + 'B'.repeat(40)
});

const byteLength = value => new TextEncoder().encode(value).length;
const append = (builder,name,value) => `${builder}${name.length}:${name}=${byteLength(value)}:${value}\n`;
async function sha256Hex(value) {
  const digest = await subtle.digest('SHA-256',new TextEncoder().encode(value));
  return Array.from(new Uint8Array(digest),byte=>byte.toString(16).padStart(2,'0')).join('').toUpperCase();
}

function projection(overrides={}) {
  return {
    ProjectionIdentity:'sha256/' + 'C'.repeat(64),
    FoundationIdentity:'falcon.foundation',
    FoundationReleaseState:'ACCEPTED_AND_CLOSED',
    HealthState:'HEALTHY',
    AuthorityState:'GOVERNED',
    LifecycleState:'RUNNING',
    ApplicationCount:0,
    EvidenceReference:'evidence/stage14/operational',
    ObservedAt:'2026-08-18T00:20:00Z',
    PresentationOnly:true,
    CarriesExecutionAuthority:false,
    CarriesBusinessAuthority:false,
    ...overrides
  };
}

async function packetFor(payloadObject, overrides={}) {
  const payload = JSON.stringify(payloadObject);
  const payloadSha256 = await sha256Hex(payload);
  const binding = {
    routeIdentity:profile.routeIdentity,
    artifactId:profile.artifactId,
    artifactVersion:profile.contractVersion,
    artifactSha256:expectedArtifactBinding.sha256,
    evidenceReference:expectedArtifactBinding.evidenceReference,
    compatibilityIdentity:profile.compatibilityIdentity,
    sourceProvenance:expectedArtifactBinding.sourceProvenance,
    payloadSha256
  };
  let canonical='';
  canonical=append(canonical,'route_identity',profile.routeIdentity);
  canonical=append(canonical,'message_type',profile.messageType);
  canonical=append(canonical,'schema_id',profile.schemaIdentity);
  canonical=append(canonical,'schema_version',profile.contractVersion);
  canonical=append(canonical,'producer',profile.producer);
  canonical=append(canonical,'recipient_scope',profile.recipientScope);
  canonical=append(canonical,'message_kind',String(profile.messageKind));
  canonical=append(canonical,'classification',String(profile.classification));
  canonical=append(canonical,'transport_authority',profile.transportAuthority);
  canonical=append(canonical,'source_provenance',binding.sourceProvenance);
  canonical=append(canonical,'artifact_id',profile.artifactId);
  canonical=append(canonical,'artifact_version',profile.contractVersion);
  canonical=append(canonical,'artifact_sha256',binding.artifactSha256.toUpperCase());
  canonical=append(canonical,'evidence_reference',binding.evidenceReference);
  canonical=append(canonical,'compatibility_identity',profile.compatibilityIdentity);
  canonical=append(canonical,'artifact_state',String(profile.artifactState));
  canonical=append(canonical,'payload_sha256',payloadSha256);
  binding.bindingIdentity=`sha256/${await sha256Hex(canonical)}`;

  const base={
    Accepted:true,
    ActivationAuthorized:false,
    ExecutionAuthorized:false,
    BusinessAuthorityGranted:false,
    Envelope:{
      MessageId:'msg-operational-1',
      MessageKind:'Event',
      Classification:'Operational',
      MessageType:profile.messageType,
      SchemaId:profile.schemaIdentity,
      SchemaVersion:profile.contractVersion,
      Producer:profile.producer,
      RecipientScope:profile.recipientScope,
      CorrelationId:'corr-operational-1',
      CausationId:null,
      Authority:profile.transportAuthority,
      Provenance:`projection-binding:${binding.bindingIdentity}`,
      IdempotencyId:'idem-operational-1',
      DeliveryAttemptId:'delivery-operational-1',
      RetryLineageId:'retry-operational-1',
      Time:{ CreatedAt:'2026-08-18T00:10:00Z', ExpiresAt:'2026-08-18T00:40:00Z' },
      Outcome:{ Code:'Succeeded', Reason:'FOUNDATION_OPERATIONAL_PROJECTION_AVAILABLE' },
      Payload:payload,
      PayloadSha256:payloadSha256
    },
    Binding:binding
  };
  return { ...base, ...overrides };
}

test('canonical Stage14 FIL projection preserves zero Application count and presentation-only truth',async()=>{
  const packet=await packetFor(projection());
  const adapter=createFoundationStage14OperationalAdapter({
    filProjectionSource:async()=>packet,
    expectedArtifactBinding,
    now:()=>now
  });
  const result=await adapter.readOperationalProjection('foundation-operational-current');
  assert.equal(result.truth,'CURRENT');
  assert.equal(result.applicationCount,0);
  assert.equal(result.healthState,'HEALTHY');
  assert.equal(result.presentationOnly,true);
  assert.equal(result.mayRepair,false);
  assert.equal(result.mayAllocateResources,false);
  assert.equal(result.mayChangeLifecycle,false);
  assert.equal(result.businessAuthorityGranted,false);
  assert.equal(result.bindingFailureReason,null);
});

test('projection carrying execution or business authority is rejected fail closed',()=>{
  assert.throws(()=>adaptFoundationOperationalProjection(projection({CarriesExecutionAuthority:true}),{now}),/authority contract violated/);
  assert.throws(()=>adaptFoundationOperationalProjection(projection({CarriesBusinessAuthority:true}),{now}),/authority contract violated/);
});

test('future observed time is rejected instead of being presented as current truth',()=>{
  assert.throws(()=>adaptFoundationOperationalProjection(projection({ObservedAt:'2026-08-18T01:00:00Z'}),{now}),/time is invalid/);
});

test('wrong FIL route binding is rejected fail closed',async()=>{
  const packet=await packetFor(projection());
  packet.Binding={...packet.Binding,RouteIdentity:'route:foundation:wrong:web:v1'};
  delete packet.Binding.routeIdentity;
  const adapter=createFoundationStage14OperationalAdapter({filProjectionSource:async()=>packet,expectedArtifactBinding,now:()=>now});
  const result=await adapter.readOperationalProjection('foundation-operational-current');
  assert.equal(result.truth,'UNAVAILABLE');
  assert.equal(result.bindingFailureReason,'FIL_ROUTE_IDENTITY_MISMATCH');
});

test('payload mutation after digest/binding construction is rejected fail closed',async()=>{
  const packet=await packetFor(projection());
  packet.Envelope.Payload=JSON.stringify(projection({HealthState:'COMPROMISED'}));
  const adapter=createFoundationStage14OperationalAdapter({filProjectionSource:async()=>packet,expectedArtifactBinding,now:()=>now});
  const result=await adapter.readOperationalProjection('foundation-operational-current');
  assert.equal(result.truth,'UNAVAILABLE');
  assert.equal(result.bindingFailureReason,'FIL_PAYLOAD_DIGEST_MISMATCH');
});

test('transport cannot smuggle activation, execution or business authority',async()=>{
  for (const flag of ['ActivationAuthorized','ExecutionAuthorized','BusinessAuthorityGranted']) {
    const packet=await packetFor(projection());
    packet[flag]=true;
    const adapter=createFoundationStage14OperationalAdapter({filProjectionSource:async()=>packet,expectedArtifactBinding,now:()=>now});
    const result=await adapter.readOperationalProjection('foundation-operational-current');
    assert.equal(result.truth,'UNAVAILABLE');
    assert.equal(result.bindingFailureReason,'FIL_TRANSPORT_AUTHORITY_CONTRACT_VIOLATION');
  }
});

test('unavailable source and invalid reference remain unavailable without invented truth',async()=>{
  const adapter=createFoundationStage14OperationalAdapter({filProjectionSource:async()=>null,expectedArtifactBinding,now:()=>now});
  assert.equal((await adapter.readOperationalProjection('valid-reference')).bindingFailureReason,'FOUNDATION_OPERATIONAL_FIL_PROJECTION_UNAVAILABLE');
  assert.equal((await adapter.readOperationalProjection('bad reference')).bindingFailureReason,'OPERATIONAL_PROJECTION_REFERENCE_INVALID');
});
