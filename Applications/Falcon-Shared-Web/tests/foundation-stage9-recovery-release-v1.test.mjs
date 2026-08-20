import test from 'node:test';
import assert from 'node:assert/strict';
import { createHash } from 'node:crypto';
import { FoundationRecoveryFilProfileV1, consumeFoundationRecoveryFilProjection } from '../src/adapters/foundation-fil-public-runtime-v1.js';
import { adaptFoundationStage9Projection, adaptFoundationRecoveryOperationalProjection, createFoundationStage9RecoveryAdapter } from '../src/adapters/foundation-stage9-recovery-release-v1.js';

const now=Date.parse('2026-08-17T20:00:00Z');
const artifactSha=`sha256/${'A'.repeat(64)}`;
const evidenceReference='evidence:foundation:recovery-projection';
const sourceProvenance='provenance:foundation:runtime';
const expectedArtifactBinding=Object.freeze({sha256:artifactSha,evidenceReference,sourceProvenance});

const projection=Object.freeze({
  ProjectionIdentity:`sha256/${'D'.repeat(64)}`,
  RecoveryCaseIdentity:'recovery-case:alpha',
  RecoveryState:'ReadyForReleaseDecision',
  RestorationOutcome:'Completed',
  ReadyForReleaseDecision:true,
  ReleaseAuthorization:'NotAuthorized',
  ReleaseExecution:'NotExecuted',
  Reintroduction:'NotStarted',
  LifecycleState:'Restricted',
  EvidenceReference:'evidence:recovery:alpha',
  ObservedAt:'2026-08-17T19:55:00Z',
  ValidUntil:'2026-08-17T20:05:00Z',
  Complete:true,
  Freshness:'Current',
  PresentationOnly:true,
  CarriesReleaseExecutionAuthority:false,
  CarriesLifecycleAuthority:false,
  CarriesBusinessAuthority:false
});

const sha256=value=>createHash('sha256').update(value,'utf8').digest('hex').toUpperCase();
const append=(builder,name,value)=>`${builder}${name.length}:${name}=${Buffer.byteLength(value,'utf8')}:${value}\n`;

function makeRecoveryFilPacket(overrides={}) {
  const payload=overrides.payload ?? JSON.stringify(overrides.projection ?? projection);
  const payloadSha256=sha256(payload);
  const artifact=(overrides.artifactSha256 ?? artifactSha).toUpperCase();
  const evidence=overrides.evidenceReference ?? evidenceReference;
  const provenance=overrides.sourceProvenance ?? sourceProvenance;
  const route=overrides.routeIdentity ?? FoundationRecoveryFilProfileV1.routeIdentity;
  const recipient=overrides.recipientScope ?? FoundationRecoveryFilProfileV1.recipientScope;

  let canonical='';
  canonical=append(canonical,'route_identity',route);
  canonical=append(canonical,'message_type',FoundationRecoveryFilProfileV1.messageType);
  canonical=append(canonical,'schema_id',FoundationRecoveryFilProfileV1.schemaIdentity);
  canonical=append(canonical,'schema_version',FoundationRecoveryFilProfileV1.contractVersion);
  canonical=append(canonical,'producer',FoundationRecoveryFilProfileV1.producer);
  canonical=append(canonical,'recipient_scope',recipient);
  canonical=append(canonical,'message_kind','4');
  canonical=append(canonical,'classification','1');
  canonical=append(canonical,'transport_authority',FoundationRecoveryFilProfileV1.transportAuthority);
  canonical=append(canonical,'source_provenance',provenance);
  canonical=append(canonical,'artifact_id',FoundationRecoveryFilProfileV1.artifactId);
  canonical=append(canonical,'artifact_version',FoundationRecoveryFilProfileV1.contractVersion);
  canonical=append(canonical,'artifact_sha256',artifact);
  canonical=append(canonical,'evidence_reference',evidence);
  canonical=append(canonical,'compatibility_identity',FoundationRecoveryFilProfileV1.compatibilityIdentity);
  canonical=append(canonical,'artifact_state','1');
  canonical=append(canonical,'payload_sha256',payloadSha256);
  const bindingIdentity=`sha256/${sha256(canonical)}`;

  return {
    Accepted:overrides.accepted ?? true,
    ActivationAuthorized:overrides.activationAuthorized ?? false,
    ExecutionAuthorized:overrides.executionAuthorized ?? false,
    BusinessAuthorityGranted:overrides.businessAuthorityGranted ?? false,
    Envelope:{
      MessageId:{Value:'message:recovery:1'},
      MessageKind:overrides.messageKind ?? 4,
      Classification:overrides.classification ?? 1,
      MessageType:overrides.messageType ?? FoundationRecoveryFilProfileV1.messageType,
      SchemaId:{Value:overrides.schemaId ?? FoundationRecoveryFilProfileV1.schemaIdentity},
      SchemaVersion:overrides.schemaVersion ?? FoundationRecoveryFilProfileV1.contractVersion,
      Producer:{Value:overrides.producer ?? FoundationRecoveryFilProfileV1.producer},
      RecipientScope:{Value:recipient},
      CorrelationId:{Value:'correlation:recovery:1'},
      CausationId:{Value:'causation:recovery:1'},
      Authority:{Value:overrides.authority ?? FoundationRecoveryFilProfileV1.transportAuthority},
      Provenance:{Value:overrides.envelopeProvenance ?? `projection-binding:${bindingIdentity}`},
      IdempotencyId:{Value:'idempotency:recovery:1'},
      DeliveryAttemptId:{Value:'delivery:recovery:1'},
      RetryLineageId:{Value:'retry:recovery:1'},
      Time:{CreatedAt:'2026-08-17T19:59:00Z',ExpiresAt:'2026-08-17T20:05:00Z'},
      Outcome:{Code:1,Reason:'authoritative_public_runtime_projection'},
      Payload:payload,
      PayloadSha256:overrides.payloadSha256 ?? payloadSha256
    },
    Binding:{
      BindingIdentity:overrides.bindingIdentity ?? bindingIdentity,
      RouteIdentity:route,
      ArtifactId:FoundationRecoveryFilProfileV1.artifactId,
      ArtifactVersion:FoundationRecoveryFilProfileV1.contractVersion,
      ArtifactSha256:artifact,
      EvidenceReference:evidence,
      CompatibilityIdentity:FoundationRecoveryFilProfileV1.compatibilityIdentity,
      SourceProvenance:provenance,
      PayloadSha256:payloadSha256
    }
  };
}

test('missing governed Stage 9 projection fails closed as unavailable',()=>{
  const result=adaptFoundationStage9Projection(null);
  assert.equal(result.truth,'UNAVAILABLE');
  assert.equal(result.freshness,'UNAVAILABLE');
  assert.equal(result.availability,'UNAVAILABLE');
  assert.equal(result.mayAuthorizeRelease,false);
  assert.equal(result.mayExecuteRelease,false);
  assert.equal(result.mayChangeLifecycle,false);
});

test('historical Stage 9 presentation preserves stale and partial truth exactly',()=>{
  const result=adaptFoundationStage9Projection({
    truth:'LAST_KNOWN',freshness:'STALE',completeness:'PARTIAL',availability:'DEGRADED',
    recoveryState:'VERIFICATION_REQUIRED',releaseDecisionReadiness:'NOT_READY',
    releaseAuthorizationState:'UNKNOWN',releaseExecutionState:'NOT_STARTED',
    lifecycleState:'RESTRICTED',recoveryCaseId:'rc-9',evidenceReferences:['ev-1'],
    asOfTime:'2026-08-16T06:00:00Z',source:'FOUNDATION_GOVERNED_PROJECTION'
  });
  assert.equal(result.freshness,'STALE');
  assert.equal(result.completeness,'PARTIAL');
  assert.deepEqual(result.evidenceReferences,['ev-1']);
});

test('canonical Foundation recovery FIL packet is accepted and remains presentation-only',async()=>{
  const consumed=await consumeFoundationRecoveryFilProjection(makeRecoveryFilPacket(),expectedArtifactBinding,{now});
  assert.equal(consumed.accepted,true);
  assert.equal(consumed.profile.routeIdentity,'route:foundation:recovery:web:v1');
  const result=adaptFoundationRecoveryOperationalProjection(consumed.projection,{now});
  assert.equal(result.truth,'CURRENT');
  assert.equal(result.recoveryState,'ReadyForReleaseDecision');
  assert.equal(result.releaseDecisionReadiness,'READY_FOR_RELEASE_DECISION');
  assert.equal(result.releaseAuthorizationState,'NOT_AUTHORIZED');
  assert.equal(result.releaseExecutionState,'NOT_EXECUTED');
  assert.equal(result.reintroductionState,'NotStarted');
  assert.equal(result.presentationOnly,true);
  assert.equal(result.mayAuthorizeRelease,false);
  assert.equal(result.mayExecuteRelease,false);
  assert.equal(result.businessAuthorityGranted,false);
});

test('Foundation recovery projection preserves stale and partial truth without upgrading it',()=>{
  const result=adaptFoundationRecoveryOperationalProjection({...projection,Complete:false,Freshness:'Stale'},{now});
  assert.equal(result.truth,'LAST_KNOWN');
  assert.equal(result.freshness,'STALE');
  assert.equal(result.completeness,'PARTIAL');
});

test('Foundation recovery contradictions and authority leakage fail closed',()=>{
  assert.throws(()=>adaptFoundationRecoveryOperationalProjection({...projection,ReadyForReleaseDecision:false},{now}),/ready-for-release/);
  assert.throws(()=>adaptFoundationRecoveryOperationalProjection({...projection,ReleaseAuthorization:'Authorized'},{now}),/authorization/);
  assert.throws(()=>adaptFoundationRecoveryOperationalProjection({...projection,CarriesLifecycleAuthority:true},{now}),/authority contract/);
  assert.throws(()=>adaptFoundationRecoveryOperationalProjection({...projection,RecoveryState:'RecoveryComplete',ReleaseAuthorization:'Authorized',ReleaseExecution:'Executed',Reintroduction:'Restricted'},{now}),/reintroduction/);
});

test('recovery FIL binding rejects route recipient payload artifact and transport-authority mutation',async()=>{
  assert.equal((await consumeFoundationRecoveryFilProjection(makeRecoveryFilPacket({routeIdentity:'route:foundation:recovery:web:v2'}),expectedArtifactBinding,{now})).reason,'FIL_ROUTE_IDENTITY_MISMATCH');
  assert.equal((await consumeFoundationRecoveryFilProjection(makeRecoveryFilPacket({recipientScope:'other-web'}),expectedArtifactBinding,{now})).reason,'FIL_RECIPIENT_MISMATCH');
  const payloadMutation=makeRecoveryFilPacket(); payloadMutation.Envelope.Payload=JSON.stringify({...projection,RecoveryCaseIdentity:'recovery-case:attacker'});
  assert.equal((await consumeFoundationRecoveryFilProjection(payloadMutation,expectedArtifactBinding,{now})).reason,'FIL_PAYLOAD_DIGEST_MISMATCH');
  assert.equal((await consumeFoundationRecoveryFilProjection(makeRecoveryFilPacket({artifactSha256:`sha256/${'C'.repeat(64)}`}),expectedArtifactBinding,{now})).reason,'FIL_EXPECTED_ARTIFACT_DIGEST_MISMATCH');
  assert.equal((await consumeFoundationRecoveryFilProjection(makeRecoveryFilPacket({executionAuthorized:true}),expectedArtifactBinding,{now})).reason,'FIL_TRANSPORT_AUTHORITY_CONTRACT_VIOLATION');
});

test('control-message substitution and expired FIL message are rejected',async()=>{
  assert.equal((await consumeFoundationRecoveryFilProjection(makeRecoveryFilPacket({messageKind:1}),expectedArtifactBinding,{now})).reason,'FIL_MESSAGE_KIND_MISMATCH');
  const stale=makeRecoveryFilPacket(); stale.Envelope.Time.ExpiresAt='2026-08-17T19:59:59Z';
  assert.equal((await consumeFoundationRecoveryFilProjection(stale,expectedArtifactBinding,{now})).reason,'FIL_MESSAGE_NOT_CURRENT');
});

test('Stage 9 runtime adapter consumes exact canonical FIL packet and fails closed when unavailable',async()=>{
  let reference=null;
  const adapter=createFoundationStage9RecoveryAdapter({
    filProjectionSource:async value=>{reference=value; return makeRecoveryFilPacket();},
    expectedArtifactBinding,
    now:()=>now
  });
  const result=await adapter.readRecoveryProjection('owner-emergency-recovery-view');
  assert.equal(reference,'owner-emergency-recovery-view');
  assert.equal(result.source,'FOUNDATION_GOVERNED_FIL_PROJECTION');
  assert.equal(result.recoveryCaseId,'recovery-case:alpha');

  const unavailableAdapter=createFoundationStage9RecoveryAdapter({
    filProjectionSource:async()=>null,
    expectedArtifactBinding,
    now:()=>now
  });
  const unavailable=await unavailableAdapter.readRecoveryProjection('owner-emergency-recovery-view');
  assert.equal(unavailable.truth,'UNAVAILABLE');
  assert.equal(unavailable.bindingFailureReason,'FOUNDATION_RECOVERY_FIL_PROJECTION_UNAVAILABLE');
});
