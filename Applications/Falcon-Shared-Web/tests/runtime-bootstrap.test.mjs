import test from 'node:test';
import assert from 'node:assert/strict';
import { AuthResult, WebSurfaceGrant } from '../src/auth.js';
import { DataSourceMode } from '../src/core/data-source-profile.js';
import { RuntimePortMethods } from '../src/core/runtime-port.js';
import { createWebRuntimeBootstrap, readInjectedWebRuntimeBindings } from '../src/composition/runtime-bootstrap.js';

const previewData=Object.freeze({ apps:[], fsatsApps:[], catalog:[], advisoryMarkets:[], ownerProviderActions:[], owner:{}, services:[], incidents:[], portfolio:{}, positions:[], trades:[], alerts:[], incidentConversation:{}, detailedAnalysis:null });
const noop=async()=>({ok:true});
const persistencePort=Object.freeze({
  saveRecord:noop,loadRecord:noop,appendEvent:noop,putArtifact:noop,getArtifact:noop,loadEvents:noop,
  commitRecordAndEvent:noop,commitArtifactAndEvents:noop
});

function providerBindings(){
  return {
    'FCR-0173':{routePolicyBound:true,exactRouteVerified:true},
    'FCR-0174':{routePolicyBound:true,exactRouteVerified:true,channelCredentialRequired:false},
    'FCR-0175':{routePolicyBound:true,exactRouteVerified:true},
    'FCR-0176':{routePolicyBound:true,exactRouteVerified:true,credentialReference:'credref:web:alpaca-iex'},
    'FCR-0177':{routePolicyBound:true,exactRouteVerified:true,credentialReference:'credref:web:finnhub'},
    'FCR-0196':{routePolicyBound:true,exactRouteVerified:true,credentialReference:'credref:web:alpaca-universe'},
    'FCR-0197':{routePolicyBound:true,exactRouteVerified:true,credentialReference:'credref:web:alpaca-history'},
    'FCR-0198':{routePolicyBound:true,exactRouteVerified:true},
    'FCR-0199':{routePolicyBound:true,exactRouteVerified:true},
    'FCR-0200':{routePolicyBound:true,exactRouteVerified:true}
  };
}

const browserVerification=Object.freeze({
  documentAvailable:true,keyboardEvents:true,focusManagement:true,rtlLayout:true,ltrLayout:true,mobileViewport:true,
  indexedDb:true,microphoneApi:true,blobApi:true,objectUrlApi:true
});

const incident=Object.freeze({
  principalId:'principal:web:user:1',tenantId:'tenant:1',sessionId:'session:1',
  productionPersistenceBinding:Object.freeze({
    authoritative:true,tenantScoped:true,businessAuthorityGranted:false,
    tenantNamespace:'tenant:1/incidents',evidenceReference:'evidence:web:incident:persistence:1',port:persistencePort
  }),
  screenshotScanner:Object.freeze({scanScreenshot:async()=>({state:'PASS'})}),
  supportTransportPort:Object.freeze({requestSupport:async()=>({accepted:false})}),
  localVoiceRuntime:Object.freeze({transcribeWithWhisperCpp:async()=>({text:''}),synthesizeWithPiper:async()=>new Blob([])})
});

function runtimePort(){
  return Object.freeze(Object.fromEntries(RuntimePortMethods.map(method=>[method,async()=>({truth:'UNAVAILABLE'})])));
}

function authAdapter(){
  return Object.freeze({
    signIn:async()=>({state:AuthResult.UNAVAILABLE}),
    signInWithProvider:async()=>({state:AuthResult.UNAVAILABLE}),
    verifyMfa:async()=>({state:AuthResult.UNAVAILABLE})
  });
}

function completeBindings(){
  return {
    mode:DataSourceMode.AUTHORITATIVE,
    authoritativeData:{
      ...previewData,
      owner:{health:'CURRENT'},
      sourceKind:'AUTHORITATIVE_PUBLIC_CONTRACTS',
      transportAuthorityCreated:false
    },
    authAdapter:authAdapter(),
    runtimePort:runtimePort(),
    provider:{
      webPrincipalId:'principal:web:shared-falcon-web',
      webServiceRole:'service-role:web:presentation-market-data',
      bindingsByFcr:providerBindings()
    },
    incident,
    browserVerification,
    ownerFsatsAccess:{
      available:true,fullVipFeatureSet:true,futureVipIncluded:true,commercialSubscription:false,trial:false,
      actionAuthorizationGranted:false,tradingExecutionAuthorityGranted:false,brokerAuthorityGranted:false,
      foundationAuthorityGranted:false,killAuthorityGranted:false,runtimeActivationAuthorized:false,deploymentAuthorized:false
    },
    applicationAccess:{fsats:{entitled:true,current:true,businessAuthorityGranted:false}}
  };
}

test('no injected runtime preserves explicit Preview composition',()=>{
  const bootstrap=createWebRuntimeBootstrap({previewData});
  assert.equal(bootstrap.mode,DataSourceMode.PREVIEW);
  assert.equal(bootstrap.dataSource.preview,true);
  assert.equal(bootstrap.authoritative,false);
  assert.deepEqual(bootstrap.blockers,['PREVIEW_MODE']);
});

test('partial Authoritative binding fails closed instead of falling back to Preview',()=>{
  const bootstrap=createWebRuntimeBootstrap({bindings:{mode:DataSourceMode.AUTHORITATIVE},previewData});
  assert.equal(bootstrap.mode,DataSourceMode.AUTHORITATIVE);
  assert.equal(bootstrap.ready,false);
  assert.equal(bootstrap.dataSource.preview,false);
  assert.equal(bootstrap.dataSource.unavailable,true);
  assert.equal(bootstrap.ownerFsatsAccess,null);
  assert.equal(bootstrap.applicationAccess,null);
  assert.ok(bootstrap.blockers.includes('AUTHORITATIVE_PUBLIC_CONTRACT_DATA_REQUIRED'));
  assert.ok(bootstrap.blockers.includes('AUTHORITATIVE_AUTH_ADAPTER_REQUIRED'));
  assert.ok(bootstrap.blockers.includes('GOVERNED_RUNTIME_PORT_REQUIRED'));
});

test('unmarked arbitrary object cannot masquerade as authoritative contract data',()=>{
  const bindings=completeBindings();
  bindings.authoritativeData={owner:{health:'CURRENT'}};
  const bootstrap=createWebRuntimeBootstrap({bindings,previewData});
  assert.equal(bootstrap.ready,false);
  assert.ok(bootstrap.blockers.includes('AUTHORITATIVE_PUBLIC_CONTRACT_DATA_REQUIRED'));
  assert.equal(bootstrap.dataSource.unavailable,true);
});

test('complete governed binding crosses the composition gate without creating business authority',()=>{
  const bootstrap=createWebRuntimeBootstrap({bindings:completeBindings(),previewData});
  assert.equal(bootstrap.ready,true);
  assert.equal(bootstrap.authoritative,true);
  assert.equal(bootstrap.dataSource.authoritative,true);
  assert.equal(bootstrap.preflight.connectivityActivated,false);
  assert.equal(bootstrap.preflight.deploymentAuthorized,false);
  assert.equal(bootstrap.preflight.businessAuthorityGranted,false);
  assert.equal(bootstrap.preflight.tradingAuthorityGranted,false);
  assert.equal(bootstrap.ownerFsatsAccess.actionAuthorizationGranted,false);
  assert.equal(bootstrap.applicationAccess.fsats.businessAuthorityGranted,false);
});

test('raw secret-shaped configuration fields are rejected at the composition boundary',()=>{
  assert.throws(()=>createWebRuntimeBootstrap({bindings:{mode:DataSourceMode.AUTHORITATIVE,apiKey:'raw-secret'},previewData}),/must not contain secret material/);
  assert.throws(()=>readInjectedWebRuntimeBindings({__FALCON_WEB_RUNTIME_BINDINGS__:{provider:{clientSecret:'raw-secret'}}}),/must not contain secret material/);
});

test('presentation data is not misclassified as configuration merely because it contains sensitive-looking field names',()=>{
  const bindings=completeBindings();
  bindings.authoritativeData={...bindings.authoritativeData,incidents:[{apiKey:'REDACTED_BY_UPSTREAM'}]};
  assert.equal(readInjectedWebRuntimeBindings({__FALCON_WEB_RUNTIME_BINDINGS__:bindings}),bindings);
});

test('top-level authority-escalation flags cannot be smuggled through the binding envelope',()=>{
  assert.throws(()=>createWebRuntimeBootstrap({bindings:{mode:DataSourceMode.AUTHORITATIVE,deploymentAuthorized:true},previewData}),/cannot grant authority through Web runtime binding/);
  assert.throws(()=>readInjectedWebRuntimeBindings({__FALCON_WEB_RUNTIME_BINDINGS__:{tradingAuthorityGranted:true}}),/cannot grant authority through Web runtime binding/);
});

test('opaque credential references remain allowed',()=>{
  const bindings=completeBindings();
  bindings.credentialReferences={'FCR-0176':'credref:web:alpaca-iex'};
  assert.equal(readInjectedWebRuntimeBindings({__FALCON_WEB_RUNTIME_BINDINGS__:bindings}),bindings);
});

test('Owner surface access still requires an authoritative session and explicit grant',()=>{
  const bootstrap=createWebRuntimeBootstrap({bindings:completeBindings(),previewData});
  const session={
    state:AuthResult.AUTHENTICATED,authoritativeSession:true,principalId:'owner:1',sessionId:'session:owner:1',
    role:'PROJECT_OWNER',surfaceGrants:[WebSurfaceGrant.OWNER],businessAuthorityGranted:false
  };
  assert.equal(bootstrap.ready,true);
  assert.equal(session.businessAuthorityGranted,false);
});
