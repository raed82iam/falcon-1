import test from 'node:test';
import assert from 'node:assert/strict';
import { buildOwnerRequestRoutingPlan, applyRoutedHandoffReceipt } from '../src/core/owner-request-router.js';

test('compound Owner request splits into independently owned items',()=>{
  const plan=buildOwnerRequestRoutingPlan({
    requestId:'req-1',
    ownerMessage:'Change the Web logo; then improve FSATS opportunity analysis'
  });
  assert.equal(plan.compound,true);
  assert.equal(plan.items.length,2);
  assert.equal(plan.items[0].owner,'WEB');
  assert.equal(plan.items[1].owner,'APPLICATION');
  assert.equal(plan.items[1].state,'FOREIGN_HANDOFF_REQUIRED');
  assert.equal(plan.actionCompleted,false);
});

test('sensitive request requires confirmation and creates no authority',()=>{
  const plan=buildOwnerRequestRoutingPlan({requestId:'req-2',ownerMessage:'Rollback the Web deployment'});
  assert.equal(plan.items[0].sensitiveConfirmationRequired,true);
  assert.equal(plan.items[0].executionAuthorityCreated,false);
  assert.equal(plan.authorityCreated,false);
});

test('routing receipt preserves request sent != accepted != completed',()=>{
  const plan=buildOwnerRequestRoutingPlan({requestId:'req-3',ownerMessage:'Improve FSATS opportunity analysis'});
  const receipt=applyRoutedHandoffReceipt(plan,{itemId:'req-3:1',correlationId:'corr-1',accepted:false});
  assert.equal(receipt.requestSent,true);
  assert.equal(receipt.actionAccepted,false);
  assert.equal(receipt.actionCompleted,false);
  assert.equal(receipt.executionAuthorityCreated,false);
});

test('unknown request cannot be treated as routed work',()=>{
  const plan=buildOwnerRequestRoutingPlan({requestId:'req-4',ownerMessage:'Do the thing'});
  assert.equal(plan.items[0].owner,'UNKNOWN');
  assert.equal(plan.items[0].state,'OWNER_CLARIFICATION_REQUIRED');
  assert.throws(()=>applyRoutedHandoffReceipt(plan,{itemId:'req-4:1',correlationId:'corr-4'}),/unknown-owner/);
});

test('mixed Web and Application hints fail closed instead of choosing first keyword match',()=>{
  const plan=buildOwnerRequestRoutingPlan({requestId:'req-5',ownerMessage:'Change the Web portfolio widget'});
  assert.equal(plan.items[0].owner,'UNKNOWN');
  assert.equal(plan.items[0].state,'OWNER_CLARIFICATION_REQUIRED');
  assert.equal(plan.items[0].executionAuthorityCreated,false);
});

test('exact Web target path overrides ambiguous prose without expanding scope',()=>{
  const plan=buildOwnerRequestRoutingPlan({
    requestId:'req-6',
    ownerMessage:'Change the Web portfolio widget',
    targetPath:'applications/shared/web/src/features/portfolio/portfolio.js'
  });
  assert.equal(plan.items[0].owner,'WEB');
  assert.equal(plan.items[0].state,'WEB_OWNED_PENDING_GOVERNED_EXECUTION');
  assert.equal(plan.items[0].executionAuthorityCreated,false);
});
