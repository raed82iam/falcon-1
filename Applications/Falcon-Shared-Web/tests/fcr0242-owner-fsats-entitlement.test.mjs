import test from 'node:test';
import assert from 'node:assert/strict';
import { adaptProjectOwnerFsatsFeatureEntitlement } from '../src/adapters/fsats-project-owner-feature-entitlement-v1.js';
import { AuthResult, WebSurfaceGrant, canAccessRoute, hasVerifiedOwnerFsatsFeatureAccess } from '../src/auth.js';

const now=Date.parse('2026-08-18T06:50:00Z');

const session=Object.freeze({
  state:AuthResult.AUTHENTICATED,
  authoritativeSession:true,
  principalId:'falcon-owner-001',
  sessionId:'owner-session-001',
  role:'PROJECT_OWNER',
  surfaceGrants:[WebSurfaceGrant.OWNER],
  businessAuthorityGranted:false,
  ownerIdentityGovernanceVersion:'owner-governance-v7'
});

const canonical=()=>({
  Accepted:true,
  ReasonCode:'PROJECT_OWNER_FULL_VIP_OR_GREATER_FEATURE_ENTITLEMENT_GRANTED',
  EntitlementId:'fsats.entitlement.project-owner.full-vip-or-greater',
  EntitlementVersion:'1.0.0',
  SubjectId:'falcon-owner-001',
  SessionId:'owner-session-001',
  OwnerIdentityGovernanceVersion:'owner-governance-v7',
  CatalogId:'fsats-customer-features',
  CatalogVersion:'2026.08.18.1',
  CatalogSha256:'A'.repeat(64),
  GrantedFeatureIds:['analysis.full','portfolio.full','strategy.vip'],
  IncludesCurrentAndFutureVipCustomerFeatures:true,
  CommercialSubscriptionRequired:false,
  TrialApplies:false,
  SevenDayWarningApplies:false,
  StandardDowngradeApplies:false,
  UpgradePromptApplies:false,
  StandardFeatureLockApplies:false,
  ActionAuthorizationGranted:false,
  TradingExecutionAuthorityGranted:false,
  BrokerAuthorityGranted:false,
  FoundationAuthorityGranted:false,
  KillAuthorityGranted:false,
  RuntimeActivationAuthorized:false,
  DeploymentAuthorized:false,
  EvaluatedAt:'2026-08-18T06:45:00Z',
  EvidenceExpiresAt:'2026-08-18T07:15:00Z'
});

test('canonical FCR0242 decision grants Owner feature navigation without action authority',()=>{
  const access=adaptProjectOwnerFsatsFeatureEntitlement(canonical(),session,{now});
  assert.equal(access.available,true);
  assert.equal(access.fullVipFeatureSet,true);
  assert.equal(access.futureVipIncluded,true);
  assert.equal(access.commercialSubscription,false);
  assert.equal(access.tradingExecutionAuthorityGranted,false);
  assert.equal(hasVerifiedOwnerFsatsFeatureAccess(access),true);
  assert.equal(canAccessRoute('trader',session,{ownerFsatsAccess:access}),true);
});

test('Project Owner role alone never unlocks customer-facing FSATS routes',()=>{
  assert.equal(canAccessRoute('trader',session),false);
  assert.equal(canAccessRoute('portfolio',session,{ownerFsatsAccess:null}),false);
});

test('session, governance version and freshness are exact fail-closed bindings',()=>{
  for(const mutate of [
    d=>{d.SubjectId='other-owner';},
    d=>{d.SessionId='other-session';},
    d=>{d.OwnerIdentityGovernanceVersion='owner-governance-v8';},
    d=>{d.EvidenceExpiresAt='2026-08-18T06:49:59Z';}
  ]){
    const d=canonical(); mutate(d);
    const access=adaptProjectOwnerFsatsFeatureEntitlement(d,session,{now});
    assert.equal(access.available,false);
    assert.equal(hasVerifiedOwnerFsatsFeatureAccess(access),false);
    assert.equal(canAccessRoute('trader',session,{ownerFsatsAccess:access}),false);
  }
});

test('commercial lifecycle or authority leakage rejects the entitlement decision',()=>{
  for(const field of [
    'CommercialSubscriptionRequired','TrialApplies','SevenDayWarningApplies','StandardDowngradeApplies','UpgradePromptApplies','StandardFeatureLockApplies',
    'ActionAuthorizationGranted','TradingExecutionAuthorityGranted','BrokerAuthorityGranted','FoundationAuthorityGranted','KillAuthorityGranted','RuntimeActivationAuthorized','DeploymentAuthorized'
  ]){
    const d=canonical(); d[field]=true;
    assert.equal(adaptProjectOwnerFsatsFeatureEntitlement(d,session,{now}).available,false,field);
  }
});

test('catalog and granted feature identity fail closed on malformed or duplicate values',()=>{
  const badSha=canonical(); badSha.CatalogSha256='abc';
  assert.equal(adaptProjectOwnerFsatsFeatureEntitlement(badSha,session,{now}).available,false);

  const duplicate=canonical(); duplicate.GrantedFeatureIds=['same','same'];
  assert.equal(adaptProjectOwnerFsatsFeatureEntitlement(duplicate,session,{now}).available,false);

  const missingFutureRule=canonical(); missingFutureRule.IncludesCurrentAndFutureVipCustomerFeatures=false;
  assert.equal(adaptProjectOwnerFsatsFeatureEntitlement(missingFutureRule,session,{now}).available,false);
});
