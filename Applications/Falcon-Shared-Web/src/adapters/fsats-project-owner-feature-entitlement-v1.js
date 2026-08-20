import { isAuthoritativeSession } from '../auth.js';

export const OWNER_FSATS_ENTITLEMENT = Object.freeze({
  id:'fsats.entitlement.project-owner.full-vip-or-greater',
  version:'1.0.0',
  catalogCompatibilityIdentity:'compat:fsats-customer-feature-catalog:v1',
  acceptedReason:'PROJECT_OWNER_FULL_VIP_OR_GREATER_FEATURE_ENTITLEMENT_GRANTED'
});

const nonEmpty=value=>typeof value==='string'&&value.trim().length>0;
const sha256=value=>typeof value==='string'&&/^[0-9A-F]{64}$/u.test(value);
const instant=value=>{
  if(!nonEmpty(value)) return null;
  const ms=Date.parse(value);
  return Number.isFinite(ms)?ms:null;
};

function unavailable(reason='OWNER_FSATS_ENTITLEMENT_UNAVAILABLE'){
  return Object.freeze({
    available:false,
    reason,
    fullVipFeatureSet:false,
    commercialSubscription:false,
    trial:false,
    grantedFeatureIds:Object.freeze([]),
    evidenceExpiresAt:null
  });
}

/**
 * Consume the Application-owned FCR-0242 entitlement decision.
 *
 * The decision grants feature visibility/navigation only. It never grants
 * Trading, broker, Foundation, Kill, deployment, runtime or business authority.
 * Transport is intentionally outside this adapter and remains separately governed.
 */
export function adaptProjectOwnerFsatsFeatureEntitlement(decision, session, { now=Date.now() }={}){
  if(!decision||typeof decision!=='object') return unavailable();
  if(!isAuthoritativeSession(session)||session.role!=='PROJECT_OWNER'||!session.surfaceGrants?.includes('OWNER')) {
    return unavailable('AUTHORITATIVE_PROJECT_OWNER_SESSION_REQUIRED');
  }

  const nowMs=now instanceof Date?now.getTime():Number(now);
  const evaluatedAt=instant(decision.EvaluatedAt??decision.evaluatedAt);
  const evidenceExpiresAt=instant(decision.EvidenceExpiresAt??decision.evidenceExpiresAt);
  if(!Number.isFinite(nowMs)||evaluatedAt===null||evidenceExpiresAt===null||evaluatedAt>nowMs||evidenceExpiresAt<=nowMs||evidenceExpiresAt<=evaluatedAt) {
    return unavailable('OWNER_FSATS_ENTITLEMENT_NOT_CURRENT');
  }

  const accepted=decision.Accepted??decision.accepted;
  const reason=decision.ReasonCode??decision.reasonCode;
  const entitlementId=decision.EntitlementId??decision.entitlementId;
  const entitlementVersion=decision.EntitlementVersion??decision.entitlementVersion;
  const subjectId=decision.SubjectId??decision.subjectId;
  const sessionId=decision.SessionId??decision.sessionId;
  const governanceVersion=decision.OwnerIdentityGovernanceVersion??decision.ownerIdentityGovernanceVersion;
  const catalogId=decision.CatalogId??decision.catalogId;
  const catalogVersion=decision.CatalogVersion??decision.catalogVersion;
  const catalogSha256=decision.CatalogSha256??decision.catalogSha256;
  const features=decision.GrantedFeatureIds??decision.grantedFeatureIds;

  if(accepted!==true||reason!==OWNER_FSATS_ENTITLEMENT.acceptedReason) return unavailable('OWNER_FSATS_ENTITLEMENT_NOT_ACCEPTED');
  if(entitlementId!==OWNER_FSATS_ENTITLEMENT.id||entitlementVersion!==OWNER_FSATS_ENTITLEMENT.version) return unavailable('OWNER_FSATS_ENTITLEMENT_IDENTITY_MISMATCH');
  if(subjectId!==session.principalId||sessionId!==session.sessionId) return unavailable('OWNER_FSATS_SESSION_BINDING_MISMATCH');
  if(!nonEmpty(governanceVersion)||!nonEmpty(session.ownerIdentityGovernanceVersion)||governanceVersion!==session.ownerIdentityGovernanceVersion) return unavailable('OWNER_IDENTITY_GOVERNANCE_VERSION_MISMATCH');
  if(!nonEmpty(catalogId)||!nonEmpty(catalogVersion)||!sha256(catalogSha256)) return unavailable('OWNER_FSATS_CATALOG_IDENTITY_INVALID');
  if(!Array.isArray(features)) return unavailable('OWNER_FSATS_FEATURE_SET_INVALID');
  const unique=new Set();
  for(const featureId of features){
    if(!nonEmpty(featureId)||unique.has(featureId)) return unavailable('OWNER_FSATS_FEATURE_SET_INVALID');
    unique.add(featureId);
  }

  const requiredFalse=[
    'CommercialSubscriptionRequired','TrialApplies','SevenDayWarningApplies','StandardDowngradeApplies','UpgradePromptApplies','StandardFeatureLockApplies',
    'ActionAuthorizationGranted','TradingExecutionAuthorityGranted','BrokerAuthorityGranted','FoundationAuthorityGranted','KillAuthorityGranted','RuntimeActivationAuthorized','DeploymentAuthorized'
  ];
  for(const key of requiredFalse){
    const camel=key[0].toLowerCase()+key.slice(1);
    if((decision[key]??decision[camel])!==false) return unavailable(`OWNER_FSATS_AUTHORITY_OR_COMMERCIAL_BOUNDARY_VIOLATION:${key}`);
  }
  if((decision.IncludesCurrentAndFutureVipCustomerFeatures??decision.includesCurrentAndFutureVipCustomerFeatures)!==true) return unavailable('OWNER_FSATS_FULL_VIP_RULE_MISSING');

  return Object.freeze({
    available:true,
    reason,
    fullVipFeatureSet:true,
    futureVipIncluded:true,
    commercialSubscription:false,
    trial:false,
    entitlementId,
    entitlementVersion,
    catalogId,
    catalogVersion,
    catalogSha256,
    grantedFeatureIds:Object.freeze([...features]),
    evidenceExpiresAt:new Date(evidenceExpiresAt).toISOString(),
    actionAuthorizationGranted:false,
    tradingExecutionAuthorityGranted:false,
    brokerAuthorityGranted:false,
    foundationAuthorityGranted:false,
    killAuthorityGranted:false,
    runtimeActivationAuthorized:false,
    deploymentAuthorized:false
  });
}

export const __test=Object.freeze({unavailable,instant,sha256});
