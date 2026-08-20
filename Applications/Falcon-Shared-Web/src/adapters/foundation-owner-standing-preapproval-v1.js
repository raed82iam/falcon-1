const text = value => typeof value === 'string' && value.trim().length > 0 && value.trim() === value;
const sha256 = value => typeof value === 'string' && /^sha256\/[0-9a-f]{64}$/iu.test(value);
const shaOrNone = value => value === 'NONE' || sha256(value);
const authorityDecision = value => typeof value === 'string' && /^authority-decision\/sha256\/[0-9a-f]{64}$/iu.test(value);
const authorityDecisionOrNone = value => value === 'NONE' || authorityDecision(value);
const utcDate = value => typeof value === 'string' && (/Z$/u.test(value) || /[+-]00:00$/u.test(value)) && Number.isFinite(Date.parse(value));
const pick=(value,...names)=>{ for(const name of names) if(value?.[name]!==undefined) return value[name]; return undefined; };

export const OwnerDecisionSurfaceIdentity='shared-web:owner-command-center';

export function adaptFoundationAutoAcceptDecision(raw,{now=Date.now()}={}) {
  if (!raw || typeof raw!=='object' || Array.isArray(raw)) throw new TypeError('auto accept decision is required');
  const accepted=pick(raw,'acceptedUnderStandingOwnerPolicy','AcceptedUnderStandingOwnerPolicy');
  const reason=pick(raw,'reason','Reason');
  const decisionIdentity=pick(raw,'decisionIdentitySha256','DecisionIdentitySha256');
  const proposalIdentity=pick(raw,'proposalIdentitySha256','ProposalIdentitySha256');
  const planIdentity=pick(raw,'backupRollbackPlanIdentitySha256','BackupRollbackPlanIdentitySha256');
  const registrationIdentity=pick(raw,'registrationIdentitySha256','RegistrationIdentitySha256');
  const underlyingAuthority=pick(raw,'underlyingAuthorityDecisionId','UnderlyingAuthorityDecisionId');
  const execution=pick(raw,'executionAuthorized','ExecutionAuthorized');
  const deployment=pick(raw,'deploymentAuthorized','DeploymentAuthorized');
  const business=pick(raw,'businessAuthorityGranted','BusinessAuthorityGranted');
  const decisionTime=pick(raw,'decisionTime','DecisionTime');
  const expiry=pick(raw,'expiry','Expiry');
  const evidence=pick(raw,'evidenceReference','EvidenceReference');
  if (typeof accepted!=='boolean' || !text(reason) || !sha256(decisionIdentity) || !shaOrNone(proposalIdentity) || !shaOrNone(planIdentity) || !shaOrNone(registrationIdentity) || !authorityDecisionOrNone(underlyingAuthority) || !text(evidence) || !utcDate(decisionTime) || !utcDate(expiry)) throw new TypeError('auto accept decision is malformed');
  if (accepted && (!sha256(proposalIdentity) || !sha256(planIdentity) || !sha256(registrationIdentity) || !authorityDecision(underlyingAuthority))) throw new TypeError('accepted auto accept decision requires exact identities');
  if (execution!==false || deployment!==false || business!==false) throw new TypeError('auto accept decision leaked execution/deployment/business authority');
  const nowMs=now instanceof Date?now.getTime():Number(now);
  if (!Number.isFinite(nowMs) || Date.parse(decisionTime)>nowMs || Date.parse(expiry)<=nowMs) throw new TypeError('auto accept decision is stale or time-invalid');
  if (accepted && reason!=='WEB_OWNER_DERIVED_AUTO_ACCEPT_ACCEPTED') throw new TypeError('accepted auto accept decision reason is invalid');
  return Object.freeze({
    accepted,reason,decisionIdentity,proposalIdentity,backupRollbackPlanIdentity:planIdentity,registrationIdentity,
    underlyingAuthorityDecisionId:underlyingAuthority,executionAuthorized:false,deploymentAuthorized:false,businessAuthorityGranted:false,
    decisionTime,expiry,evidenceReference:evidence,source:'FOUNDATION_OWNER_STANDING_PREAPPROVAL'
  });
}

export function adaptFoundationPolicyManagementDecision(raw,{now=Date.now()}={}) {
  if (!raw || typeof raw!=='object' || Array.isArray(raw)) throw new TypeError('policy management decision is required');
  const applied=pick(raw,'applied','Applied');
  const reason=pick(raw,'reason','Reason');
  const decisionIdentity=pick(raw,'decisionIdentitySha256','DecisionIdentitySha256');
  const registrationIdentity=pick(raw,'registrationIdentitySha256','RegistrationIdentitySha256');
  const policyId=pick(raw,'policyId','PolicyId');
  const policyVersion=pick(raw,'policyVersion','PolicyVersion');
  const revoked=pick(raw,'revoked','Revoked');
  const decisionTime=pick(raw,'decisionTime','DecisionTime');
  const expiry=pick(raw,'expiry','Expiry');
  const evidence=pick(raw,'evidenceReference','EvidenceReference');
  if (typeof applied!=='boolean' || !text(reason) || !sha256(decisionIdentity) || !shaOrNone(registrationIdentity) || !text(policyId) || !text(policyVersion) || typeof revoked!=='boolean' || !utcDate(decisionTime) || !utcDate(expiry) || !text(evidence)) throw new TypeError('policy management decision is malformed');
  if (applied && !sha256(registrationIdentity)) throw new TypeError('applied policy decision requires exact registration identity');
  if (!applied && registrationIdentity!=='NONE') throw new TypeError('denied policy decision cannot claim registration identity');
  const nowMs=now instanceof Date?now.getTime():Number(now);
  if (!Number.isFinite(nowMs) || Date.parse(decisionTime)>nowMs || Date.parse(expiry)<=nowMs) throw new TypeError('policy management decision is stale or time-invalid');
  if (applied && reason!=='STANDING_OWNER_POLICY_MUTATION_APPLIED') throw new TypeError('applied policy decision reason is invalid');
  return Object.freeze({applied,reason,decisionIdentity,registrationIdentity,policyId,policyVersion,revoked,decisionTime,expiry,evidenceReference:evidence,source:'FOUNDATION_OWNER_POLICY_MANAGEMENT'});
}

export function adaptFoundationRollbackOrderDecision(raw,{now=Date.now()}={}) {
  if (!raw || typeof raw!=='object' || Array.isArray(raw)) throw new TypeError('rollback order decision is required');
  const state=pick(raw,'state','State');
  const reason=pick(raw,'reason','Reason');
  const decisionIdentity=pick(raw,'decisionIdentitySha256','DecisionIdentitySha256');
  const rollbackAuthorized=pick(raw,'rollbackAuthorized','RollbackAuthorized');
  const rollbackExecuted=pick(raw,'rollbackExecuted','RollbackExecuted');
  const authorityRestored=pick(raw,'authorityRestored','AuthorityRestored');
  const trustRestored=pick(raw,'trustRestored','TrustRestored');
  const decisionTime=pick(raw,'decisionTime','DecisionTime');
  const expiry=pick(raw,'expiry','Expiry');
  const evidence=pick(raw,'evidenceReference','EvidenceReference');
  if (!['Requested','Accepted','Rejected',0,1,2].includes(state) || !text(reason) || !sha256(decisionIdentity) || typeof rollbackAuthorized!=='boolean' || typeof rollbackExecuted!=='boolean' || typeof authorityRestored!=='boolean' || typeof trustRestored!=='boolean' || !utcDate(decisionTime) || !utcDate(expiry) || !text(evidence)) throw new TypeError('rollback order decision is malformed');
  if (rollbackExecuted || authorityRestored || trustRestored) throw new TypeError('rollback authorization cannot imply execution or restoration');
  const accepted=state==='Accepted'||state===1;
  if (accepted && (!rollbackAuthorized || reason!=='ROLLBACK_ORDER_ACCEPTED_FOR_SEPARATE_EXECUTION')) throw new TypeError('rollback accepted decision contract violated');
  if (!accepted && rollbackAuthorized) throw new TypeError('rollback rejected/requested decision cannot authorize rollback');
  const nowMs=now instanceof Date?now.getTime():Number(now);
  if (!Number.isFinite(nowMs) || Date.parse(decisionTime)>nowMs || Date.parse(expiry)<=nowMs) throw new TypeError('rollback order decision is stale or time-invalid');
  return Object.freeze({state:accepted?'Accepted':(state==='Requested'||state===0?'Requested':'Rejected'),reason,decisionIdentity,rollbackAuthorized,rollbackExecuted:false,authorityRestored:false,trustRestored:false,decisionTime,expiry,evidenceReference:evidence,source:'FOUNDATION_OWNER_ROLLBACK_ORDER'});
}

export function adaptFoundationRollbackStatus(raw) {
  if (!raw || typeof raw!=='object' || Array.isArray(raw)) throw new TypeError('rollback status is required');
  const decisionIdentity=pick(raw,'rollbackOrderDecisionIdentitySha256','RollbackOrderDecisionIdentitySha256');
  const executionState=pick(raw,'executionState','ExecutionState');
  const executorIdentity=pick(raw,'executorIdentity','ExecutorIdentity');
  const evidence=pick(raw,'resultEvidenceReference','ResultEvidenceReference');
  const observedAt=pick(raw,'observedAt','ObservedAt');
  const authorityRestored=pick(raw,'authorityRestored','AuthorityRestored');
  const trustRestored=pick(raw,'trustRestored','TrustRestored');
  const credentialsRestored=pick(raw,'credentialsRestored','CredentialsRestored');
  const liveRestored=pick(raw,'liveTradingAuthorityRestored','LiveTradingAuthorityRestored');
  const killRestored=pick(raw,'killReleaseRevivalAuthorityRestored','KillReleaseRevivalAuthorityRestored');
  if (!sha256(decisionIdentity) || !['NotStarted','InProgress','Completed','Failed',0,1,2,3].includes(executionState) || !text(executorIdentity) || !text(evidence) || !utcDate(observedAt)) throw new TypeError('rollback status is malformed');
  if ([authorityRestored,trustRestored,credentialsRestored,liveRestored,killRestored].some(value=>value===true)) throw new TypeError('rollback status cannot silently restore separate authority or trust');
  const state=typeof executionState==='number'?['NotStarted','InProgress','Completed','Failed'][executionState]:executionState;
  return Object.freeze({rollbackOrderDecisionIdentity:decisionIdentity,executionState:state,executorIdentity,resultEvidenceReference:evidence,observedAt,authorityRestored:false,trustRestored:false,credentialsRestored:false,liveTradingAuthorityRestored:false,killReleaseRevivalAuthorityRestored:false,source:'FOUNDATION_OWNER_ROLLBACK_STATUS'});
}
