import test from 'node:test';
import assert from 'node:assert/strict';
import { createFoundationRuntimePortBinding } from '../src/adapters/foundation-runtime-port-v1.js';
import { assertRuntimePort, createUnavailableRuntimePort } from '../src/core/runtime-port.js';

function recoveryAdapter() {
  return Object.freeze({
    async readRecoveryProjection(reference) {
      return Object.freeze({
        truth:'CURRENT',
        reference,
        presentationOnly:true,
        mayAuthorizeRelease:false,
        mayExecuteRelease:false,
        mayChangeLifecycle:false
      });
    }
  });
}

test('Foundation runtime binding exposes recovery projection only through stable Web runtime port', async () => {
  const port = createFoundationRuntimePortBinding({
    baseRuntimePort:createUnavailableRuntimePort(),
    recoveryAdapter:recoveryAdapter()
  });

  assertRuntimePort(port);
  const recovery = await port.recoveryOperational('owner-recovery-view');
  assert.equal(recovery.truth,'CURRENT');
  assert.equal(recovery.reference,'owner-recovery-view');
  assert.equal(recovery.presentationOnly,true);
  assert.equal(recovery.mayAuthorizeRelease,false);
  assert.equal(recovery.mayExecuteRelease,false);
  assert.equal(recovery.mayChangeLifecycle,false);
  assert.equal((await port.systemOverview('owner-system-view')).truth,'UNAVAILABLE');
});

test('Stage 14 operational projection remains fail closed when exact governed adapter is absent', async () => {
  const port = createFoundationRuntimePortBinding({
    baseRuntimePort:createUnavailableRuntimePort(),
    recoveryAdapter:recoveryAdapter()
  });

  const system = await port.systemOverview('owner-system-view');
  assert.deepEqual(system, { truth:'UNAVAILABLE' });
});

test('Foundation runtime binding can compose an already-governed operational adapter without owning its contract', async () => {
  const operationalAdapter = Object.freeze({
    async readOperationalProjection(reference) {
      return Object.freeze({
        truth:'CURRENT',
        reference,
        foundationIdentity:'foundation',
        releaseState:'READY',
        healthState:'HEALTHY',
        authorityState:'GOVERNED',
        lifecycleState:'RUNNING',
        applicationCount:0,
        evidenceReference:'evidence:stage14',
        presentationOnly:true,
        carriesExecutionAuthority:false,
        carriesBusinessAuthority:false
      });
    }
  });

  const port = createFoundationRuntimePortBinding({
    baseRuntimePort:createUnavailableRuntimePort(),
    recoveryAdapter:recoveryAdapter(),
    operationalAdapter
  });

  const system = await port.systemOverview('owner-system-view');
  assert.equal(system.truth,'CURRENT');
  assert.equal(system.reference,'owner-system-view');
  assert.equal(system.applicationCount,0, 'zero applications is valid operational truth');
  assert.equal(system.presentationOnly,true);
  assert.equal(system.carriesExecutionAuthority,false);
  assert.equal(system.carriesBusinessAuthority,false);
});

test('Foundation runtime binding rejects malformed optional operational adapter instead of inventing transport', () => {
  assert.throws(
    () => createFoundationRuntimePortBinding({
      baseRuntimePort:createUnavailableRuntimePort(),
      recoveryAdapter:recoveryAdapter(),
      operationalAdapter:{}
    }),
    /operationalAdapter\.readOperationalProjection must be a function when provided/
  );
});

test('Foundation runtime binding rejects missing recovery adapter instead of inventing transport', () => {
  assert.throws(
    () => createFoundationRuntimePortBinding({ baseRuntimePort:createUnavailableRuntimePort() }),
    /recoveryAdapter\.readRecoveryProjection is required/
  );
});
