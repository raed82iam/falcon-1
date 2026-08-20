import test from 'node:test';
import assert from 'node:assert/strict';
import {
  RuntimePortMethods,
  assertRuntimePort,
  createUnavailableRuntimePort
} from '../src/core/runtime-port.js';

test('unavailable runtime port is complete and fail-closed', async () => {
  const port = createUnavailableRuntimePort();
  assertRuntimePort(port);

  for (const method of RuntimePortMethods) {
    const result = await port[method]();
    assert.equal(result.truth, 'UNAVAILABLE');
  }
});

test('runtime port rejects incomplete adapters', () => {
  assert.throws(
    () => assertRuntimePort({ applications() {} }),
    /missing method/
  );
});

test('runtime port accepts a complete governed adapter shape', () => {
  const candidate = Object.fromEntries(
    RuntimePortMethods.map(method => [method, async () => ({ truth: 'CURRENT' })])
  );

  assert.equal(assertRuntimePort(candidate), candidate);
});
