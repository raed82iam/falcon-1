import test from 'node:test';
import assert from 'node:assert/strict';
import { createLocalSafeOwnerAuthAdapter } from '../src/local-safe-owner-auth.js';

const config={environment:'LOCAL_SAFE_DEMO',ownerEmail:'owner@example.test',businessAuthorityGranted:false,tradingAuthorityGranted:false,brokerAuthorityGranted:false};

test('local safe demo admits only the configured Owner recovery email without financial authority',async()=>{
  const auth=createLocalSafeOwnerAuthAdapter(config);
  assert.equal((await auth.signIn({username:'other@example.test'})).state,'REJECTED');
  const owner=await auth.signIn({username:'OWNER@example.test'});
  assert.equal(owner.state,'AUTHENTICATED');
  assert.equal(owner.role,'PROJECT_OWNER');
  assert.deepEqual(owner.surfaceGrants,['OWNER']);
  assert.equal(owner.recoveryMethod,'EMAIL_ONLY');
  assert.equal(owner.businessAuthorityGranted,false);
  assert.equal(owner.tradingAuthorityGranted,false);
  assert.equal(owner.brokerAuthorityGranted,false);
});

test('local safe demo rejects missing authority locks',()=>{
  assert.throws(()=>createLocalSafeOwnerAuthAdapter({...config,tradingAuthorityGranted:true}),/cannot receive/);
});
