const pick=(value,...names)=>{for(const name of names) if(value?.[name]!==undefined) return value[name]; return undefined;};
const scalar=value=>typeof value==='string'?value:pick(value,'value','Value');
const token=value=>typeof value==='string'&&value.length>0&&value.trim()===value&&!/[\u0000-\u001f\s]/u.test(value);
const utcZero=value=>typeof value==='string'&&(/Z$/u.test(value)||/[+-]00:00$/u.test(value))&&Number.isFinite(Date.parse(value))?Date.parse(value):null;
const byteLength=value=>new TextEncoder().encode(value).length;
const append=(builder,name,value)=>`${builder}${name.length}:${name}=${byteLength(value)}:${value}\n`;

const KIND=Object.freeze({COMMAND:1,QUERY:2,RESPONSE:3});
const CLASSIFICATION=2;
const CONTRACT_STATE=1;
const VERSION='1.0.0';
const REQUEST_PRODUCER='shared-web';
const REQUEST_RECIPIENT='foundation.owner-governance';
const RESPONSE_PRODUCER='foundation.runtime';
const RESPONSE_RECIPIENT='shared-web';
const REQUEST_AUTHORITY='authority:transport:owner-command-center-request';
const RESPONSE_AUTHORITY='authority:transport:owner-governance-response';
const REQUEST_TTL_SECONDS=120;
const RESPONSE_TTL_SECONDS=120;
const MAX_DELIVERY_ATTEMPTS=3;

function profile(values){return Object.freeze({contractVersion:VERSION,schemaVersion:VERSION,contractState:CONTRACT_STATE,classification:CLASSIFICATION,requestProducer:REQUEST_PRODUCER,requestRecipientScope:REQUEST_RECIPIENT,responseProducer:RESPONSE_PRODUCER,responseRecipientScope:RESPONSE_RECIPIENT,requestTransportAuthority:REQUEST_AUTHORITY,responseTransportAuthority:RESPONSE_AUTHORITY,requestMaxTtlSeconds:REQUEST_TTL_SECONDS,responseMaxTtlSeconds:RESPONSE_TTL_SECONDS,maxDeliveryAttempts:MAX_DELIVERY_ATTEMPTS,retryRequiresSameIdempotencyIdentity:true,...values});}

export const OwnerPolicyManagementFilProfileV1=profile({familyIdentity:'foundation:owner-governance:standing-policy-management:v1',contractIdentity:'foundation/contracts/standing-owner-policy-management-request-response',compatibilityIdentity:'compat:foundation-owner-policy-management:v1',requestRouteIdentity:'route:foundation:owner-policy-management:web:v1',responseRouteIdentity:'route:foundation:owner-policy-management-result:web:v1',requestMessageKind:KIND.COMMAND,requestMessageType:'Foundation.Authority.StandingOwnerPolicyManagementRequest',responseMessageType:'Foundation.Authority.StandingOwnerPolicyManagementDecision',requestSchemaIdentity:'foundation.authority.standing-owner-policy-management.request',responseSchemaIdentity:'foundation.authority.standing-owner-policy-management.decision',admissionIdentity:'admission:foundation:owner-policy-management:web:v1',evidenceReference:'evidence:fcr-0241:standing-owner-policy-management:v1'});
export const OwnerPreapprovalEvaluationFilProfileV1=profile({familyIdentity:'foundation:owner-governance:standing-preapproval-evaluation:v1',contractIdentity:'foundation/contracts/standing-owner-preapproval-evaluation-request-response',compatibilityIdentity:'compat:foundation-owner-preapproval-evaluation:v1',requestRouteIdentity:'route:foundation:owner-preapproval-evaluation:web:v1',responseRouteIdentity:'route:foundation:owner-preapproval-evaluation-result:web:v1',requestMessageKind:KIND.QUERY,requestMessageType:'Foundation.Authority.WebOwnerPreapprovalProposal',responseMessageType:'Foundation.Authority.WebOwnerDerivedAutoAcceptDecision',requestSchemaIdentity:'foundation.authority.web-owner-preapproval.proposal',responseSchemaIdentity:'foundation.authority.web-owner-preapproval.decision',admissionIdentity:'admission:foundation:owner-preapproval-evaluation:web:v1',evidenceReference:'evidence:fcr-0241:standing-owner-preapproval-evaluation:v1'});
export const OwnerRollbackOrderFilProfileV1=profile({familyIdentity:'foundation:owner-governance:rollback-order:v1',contractIdentity:'foundation/contracts/owner-rollback-order-request-response',compatibilityIdentity:'compat:foundation-owner-rollback-order:v1',requestRouteIdentity:'route:foundation:owner-rollback-order:web:v1',responseRouteIdentity:'route:foundation:owner-rollback-order-result:web:v1',requestMessageKind:KIND.COMMAND,requestMessageType:'Foundation.Authority.OwnerRollbackOrderRequest',responseMessageType:'Foundation.Authority.OwnerRollbackOrderDecision',requestSchemaIdentity:'foundation.authority.owner-rollback-order.request',responseSchemaIdentity:'foundation.authority.owner-rollback-order.decision',admissionIdentity:'admission:foundation:owner-rollback-order:web:v1',evidenceReference:'evidence:fcr-0241:owner-rollback-order:v1'});
export const OwnerGovernanceFilProfilesV1=Object.freeze([OwnerPolicyManagementFilProfileV1,OwnerPreapprovalEvaluationFilProfileV1,OwnerRollbackOrderFilProfileV1]);

async function sha256Hex(value){if(!globalThis.crypto?.subtle) throw new Error('CRYPTO_UNAVAILABLE');const digest=await globalThis.crypto.subtle.digest('SHA-256',new TextEncoder().encode(value));return Array.from(new Uint8Array(digest),b=>b.toString(16).padStart(2,'0')).join('').toUpperCase();}

export async function ownerGovernanceProfileIdentity(profileValue){
  if(!OwnerGovernanceFilProfilesV1.includes(profileValue)) throw new TypeError('canonical Owner governance profile is required');
  let canonical='';
  const pairs=[['family',profileValue.familyIdentity],['contract',profileValue.contractIdentity],['contract_version',profileValue.contractVersion],['compatibility',profileValue.compatibilityIdentity],['contract_state',String(profileValue.contractState)],['request_route',profileValue.requestRouteIdentity],['response_route',profileValue.responseRouteIdentity],['request_kind',String(profileValue.requestMessageKind)],['request_type',profileValue.requestMessageType],['response_type',profileValue.responseMessageType],['request_schema',profileValue.requestSchemaIdentity],['response_schema',profileValue.responseSchemaIdentity],['schema_version',profileValue.schemaVersion],['request_producer',profileValue.requestProducer],['request_recipient',profileValue.requestRecipientScope],['response_producer',profileValue.responseProducer],['response_recipient',profileValue.responseRecipientScope],['classification',String(profileValue.classification)],['request_authority',profileValue.requestTransportAuthority],['response_authority',profileValue.responseTransportAuthority],['admission',profileValue.admissionIdentity],['request_ttl_seconds',String(profileValue.requestMaxTtlSeconds)],['response_ttl_seconds',String(profileValue.responseMaxTtlSeconds)],['max_delivery_attempts',String(profileValue.maxDeliveryAttempts)],['retry_same_idempotency',profileValue.retryRequiresSameIdempotencyIdentity?'true':'false'],['evidence',profileValue.evidenceReference]];
  for(const [name,value] of pairs) canonical=append(canonical,name,value);
  return `sha256/${await sha256Hex(canonical)}`;
}

function normalizeEnvelope(raw={}){const time=pick(raw,'time','Time')??{};const outcome=pick(raw,'outcome','Outcome')??{};return Object.freeze({messageId:scalar(pick(raw,'messageId','MessageId')),messageKind:pick(raw,'messageKind','MessageKind'),classification:pick(raw,'classification','Classification'),messageType:pick(raw,'messageType','MessageType'),schemaId:scalar(pick(raw,'schemaId','SchemaId')),schemaVersion:pick(raw,'schemaVersion','SchemaVersion'),producer:scalar(pick(raw,'producer','Producer')),recipientScope:scalar(pick(raw,'recipientScope','RecipientScope')),correlationId:scalar(pick(raw,'correlationId','CorrelationId')),causationId:scalar(pick(raw,'causationId','CausationId'))??null,authority:scalar(pick(raw,'authority','Authority')),provenance:scalar(pick(raw,'provenance','Provenance')),idempotencyId:scalar(pick(raw,'idempotencyId','IdempotencyId')),deliveryAttemptId:scalar(pick(raw,'deliveryAttemptId','DeliveryAttemptId')),retryLineageId:scalar(pick(raw,'retryLineageId','RetryLineageId')),createdAt:pick(time,'createdAt','CreatedAt'),expiresAt:pick(time,'expiresAt','ExpiresAt'),outcomeCode:pick(outcome,'code','Code'),outcomeReason:pick(outcome,'reason','Reason'),payload:pick(raw,'payload','Payload'),payloadSha256:pick(raw,'payloadSha256','PayloadSha256')});}
function enumNumber(value,map){if(Number.isInteger(value)) return value;if(typeof value!=='string') return null;return map[value.toUpperCase()]??null;}
const KIND_MAP=Object.freeze({COMMAND:1,QUERY:2,RESPONSE:3});
const CLASS_MAP=Object.freeze({GOVERNANCE:2});
const OUTCOME_MAP=Object.freeze({UNKNOWN:0,SUCCEEDED:1,SUCCESS:1,FAILED:2,REJECTED:3});
function validWindow(createdAt,expiresAt,maxTtlSeconds,nowMs){const created=utcZero(createdAt),expires=utcZero(expiresAt);return created!==null&&expires!==null&&created<=nowMs&&expires>nowMs&&expires>created&&(expires-created)<=maxTtlSeconds*1000;}
function decisionFlags(packet){return {accepted:pick(packet,'accepted','Accepted'),routeAvailable:pick(packet,'routeAvailable','RouteAvailable'),routeActivated:pick(packet,'routeActivated','RouteActivated'),routeAuthorized:pick(packet,'routeAuthorized','RouteAuthorized'),connectionExecuted:pick(packet,'connectionExecuted','ConnectionExecuted'),executionAuthorized:pick(packet,'executionAuthorized','ExecutionAuthorized'),businessAuthorityGranted:pick(packet,'businessAuthorityGranted','BusinessAuthorityGranted')};}
function reject(reason){return Object.freeze({accepted:false,reason,payload:null,envelope:null,profile:null});}

export async function buildOwnerGovernanceRequest(profileValue,payloadObject,identity,{createdAt,expiresAt,now=Date.now()}={}){
  if(!OwnerGovernanceFilProfilesV1.includes(profileValue)) return Object.freeze({built:false,reason:'OWNER_GOVERNANCE_PROFILE_NOT_CANONICAL'});
  if(!payloadObject||typeof payloadObject!=='object'||Array.isArray(payloadObject)) return Object.freeze({built:false,reason:'OWNER_GOVERNANCE_REQUEST_PAYLOAD_INVALID'});
  const required=['messageId','correlationId','idempotencyId','deliveryAttemptId','retryLineageId'];
  if(!identity||required.some(key=>!token(identity[key]))||(identity.causationId!=null&&!token(identity.causationId))) return Object.freeze({built:false,reason:'OWNER_GOVERNANCE_REQUEST_IDENTITY_INVALID'});
  if(identity.causationId===identity.correlationId) return Object.freeze({built:false,reason:'OWNER_GOVERNANCE_REQUEST_CAUSATION_INVALID'});
  const created=createdAt??new Date().toISOString();const expiry=expiresAt??new Date(Date.parse(created)+REQUEST_TTL_SECONDS*1000).toISOString();
  const nowMs=now instanceof Date?now.getTime():Number(now);
  if(!Number.isFinite(nowMs)||!validWindow(created,expiry,profileValue.requestMaxTtlSeconds,nowMs)) return Object.freeze({built:false,reason:'OWNER_GOVERNANCE_REQUEST_FRESHNESS_INVALID'});
  const payload=JSON.stringify(payloadObject);let digest,profileIdentity;
  try{digest=await sha256Hex(payload);profileIdentity=await ownerGovernanceProfileIdentity(profileValue);}catch{return Object.freeze({built:false,reason:'OWNER_GOVERNANCE_CRYPTO_UNAVAILABLE'});}
  const envelope=Object.freeze({messageId:identity.messageId,messageKind:profileValue.requestMessageKind,classification:profileValue.classification,messageType:profileValue.requestMessageType,schemaId:profileValue.requestSchemaIdentity,schemaVersion:profileValue.schemaVersion,producer:profileValue.requestProducer,recipientScope:profileValue.requestRecipientScope,correlationId:identity.correlationId,causationId:identity.causationId??null,authority:profileValue.requestTransportAuthority,provenance:`request-profile:${profileIdentity.slice(7)}`,idempotencyId:identity.idempotencyId,deliveryAttemptId:identity.deliveryAttemptId,retryLineageId:identity.retryLineageId,time:Object.freeze({createdAt:created,expiresAt:expiry}),outcome:Object.freeze({code:0,reason:'transport_request_pending'}),payload,payloadSha256:digest});
  return Object.freeze({built:true,reason:'WEB_OWNER_GOVERNANCE_REQUEST_BUILT',envelope,profileIdentitySha256:profileIdentity,routeIdentity:profileValue.requestRouteIdentity,admissionIdentity:profileValue.admissionIdentity,evidenceReference:profileValue.evidenceReference,profile:profileValue});
}

export async function consumeOwnerGovernanceResponse(packet,profileValue,request,{now=Date.now()}={}){
  if(!OwnerGovernanceFilProfilesV1.includes(profileValue)) return reject('OWNER_GOVERNANCE_PROFILE_NOT_CANONICAL');
  if(!request?.built||!request.envelope||request.profile!==profileValue) return reject('OWNER_GOVERNANCE_BUILT_REQUEST_REQUIRED');
  if(!packet||typeof packet!=='object') return reject('OWNER_GOVERNANCE_RESPONSE_MISSING');
  const flags=decisionFlags(packet);
  if(flags.accepted!==true||flags.routeAvailable!==true) return reject('OWNER_GOVERNANCE_RESPONSE_NOT_ACCEPTED');
  if(flags.routeActivated!==false||flags.routeAuthorized!==false||flags.connectionExecuted!==false||flags.executionAuthorized!==false||flags.businessAuthorityGranted!==false) return reject('OWNER_GOVERNANCE_RESPONSE_AUTHORITY_VIOLATION');
  let expectedProfileIdentity;try{expectedProfileIdentity=await ownerGovernanceProfileIdentity(profileValue);}catch{return reject('OWNER_GOVERNANCE_CRYPTO_UNAVAILABLE');}
  if(pick(packet,'profileIdentitySha256','ProfileIdentitySha256')!==expectedProfileIdentity) return reject('OWNER_GOVERNANCE_PROFILE_IDENTITY_MISMATCH');
  const e=normalizeEnvelope(pick(packet,'envelope','Envelope'));
  if(enumNumber(e.messageKind,KIND_MAP)!==KIND.RESPONSE) return reject('OWNER_GOVERNANCE_RESPONSE_KIND_MISMATCH');
  if(enumNumber(e.classification,CLASS_MAP)!==CLASSIFICATION) return reject('OWNER_GOVERNANCE_RESPONSE_CLASSIFICATION_MISMATCH');
  if(e.messageType!==profileValue.responseMessageType||e.schemaId!==profileValue.responseSchemaIdentity||e.schemaVersion!==profileValue.schemaVersion) return reject('OWNER_GOVERNANCE_RESPONSE_SCHEMA_MISMATCH');
  if(e.producer!==profileValue.responseProducer||e.recipientScope!==profileValue.responseRecipientScope||e.authority!==profileValue.responseTransportAuthority) return reject('OWNER_GOVERNANCE_RESPONSE_ENDPOINT_MISMATCH');
  if(e.correlationId!==request.envelope.correlationId||e.causationId!==request.envelope.messageId) return reject('OWNER_GOVERNANCE_RESPONSE_REQUEST_BINDING_MISMATCH');
  for(const value of [e.messageId,e.correlationId,e.causationId,e.idempotencyId,e.deliveryAttemptId,e.retryLineageId]) if(!token(value)) return reject('OWNER_GOVERNANCE_RESPONSE_IDENTITY_INVALID');
  const outcomeCode=enumNumber(e.outcomeCode,OUTCOME_MAP);
  if(![0,1,2,3].includes(outcomeCode)||typeof e.outcomeReason!=='string'||e.outcomeReason.length===0||e.outcomeReason.trim()!==e.outcomeReason) return reject('OWNER_GOVERNANCE_RESPONSE_OUTCOME_INVALID');
  const nowMs=now instanceof Date?now.getTime():Number(now);
  if(!Number.isFinite(nowMs)||!validWindow(e.createdAt,e.expiresAt,profileValue.responseMaxTtlSeconds,nowMs)) return reject('OWNER_GOVERNANCE_RESPONSE_NOT_CURRENT');
  if(!validWindow(request.envelope.time.createdAt,request.envelope.time.expiresAt,profileValue.requestMaxTtlSeconds,nowMs)) return reject('OWNER_GOVERNANCE_REQUEST_NO_LONGER_CURRENT');
  const requestCreated=utcZero(request.envelope.time.createdAt),responseCreated=utcZero(e.createdAt);if(responseCreated<requestCreated) return reject('OWNER_GOVERNANCE_RESPONSE_PRECEDES_REQUEST');
  if(typeof e.payload!=='string'||e.payload.length===0||typeof e.payloadSha256!=='string'||!/^[0-9a-f]{64}$/iu.test(e.payloadSha256)) return reject('OWNER_GOVERNANCE_RESPONSE_PAYLOAD_INVALID');
  let digest;try{digest=await sha256Hex(e.payload);}catch{return reject('OWNER_GOVERNANCE_CRYPTO_UNAVAILABLE');}if(digest!==e.payloadSha256.toUpperCase()) return reject('OWNER_GOVERNANCE_RESPONSE_PAYLOAD_DIGEST_MISMATCH');
  if(e.provenance!==`response-profile:${expectedProfileIdentity.slice(7)}`) return reject('OWNER_GOVERNANCE_RESPONSE_PROVENANCE_MISMATCH');
  let payload;try{payload=JSON.parse(e.payload);}catch{return reject('OWNER_GOVERNANCE_RESPONSE_JSON_INVALID');}if(!payload||typeof payload!=='object'||Array.isArray(payload)) return reject('OWNER_GOVERNANCE_RESPONSE_OBJECT_INVALID');
  return Object.freeze({accepted:true,reason:'WEB_OWNER_GOVERNANCE_RESPONSE_ACCEPTED',payload:Object.freeze(payload),envelope:e,profile:profileValue});
}

export function createFoundationOwnerGovernanceTransportAdapter({exchange,identityFactory,clock=()=>Date.now()}={}){
  if(typeof exchange!=='function') throw new TypeError('exchange is required');if(typeof identityFactory!=='function') throw new TypeError('identityFactory is required');
  async function invoke(profileValue,payload){const startedMs=Number(clock());if(!Number.isFinite(startedMs)) throw new TypeError('clock must return epoch milliseconds');const identity=identityFactory(profileValue,payload);const createdAt=new Date(startedMs).toISOString();const expiresAt=new Date(startedMs+profileValue.requestMaxTtlSeconds*1000).toISOString();const request=await buildOwnerGovernanceRequest(profileValue,payload,identity,{createdAt,expiresAt,now:startedMs});if(!request.built) return reject(request.reason);const packet=await exchange(Object.freeze({profile:profileValue,request}));const observedMs=Number(clock());if(!Number.isFinite(observedMs)) return reject('OWNER_GOVERNANCE_OBSERVATION_TIME_INVALID');return consumeOwnerGovernanceResponse(packet,profileValue,request,{now:observedMs});}
  return Object.freeze({async manageStandingPolicy(payload){return invoke(OwnerPolicyManagementFilProfileV1,payload);},async evaluateStandingProposal(payload){return invoke(OwnerPreapprovalEvaluationFilProfileV1,payload);},async requestRollback(payload){return invoke(OwnerRollbackOrderFilProfileV1,payload);}});
}