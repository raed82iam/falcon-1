import test from 'node:test';
import assert from 'node:assert/strict';
import {
  BindingState,
  DeploymentCapability,
  capabilityIsUsable,
  createDeploymentProfile,
  createProviderBinding,
  replaceProvider,
  unboundDeploymentProfile
} from '../src/core/deployment-profile.js';

test('deployment profile fails closed when no provider is bound', () => {
  for (const capability of Object.values(DeploymentCapability)) {
    assert.equal(unboundDeploymentProfile[capability].state, BindingState.UNBOUND);
    assert.equal(capabilityIsUsable(unboundDeploymentProfile, capability), false);
  }
});

test('provider identity is replaceable without changing capability identity', () => {
  const initial = createDeploymentProfile({
    [DeploymentCapability.EDGE_WAF]: { providerId: 'provider-a', state: BindingState.VERIFIED }
  });
  const replacement = replaceProvider(initial, DeploymentCapability.EDGE_WAF, {
    providerId: 'provider-b',
    state: BindingState.VERIFIED
  });

  assert.equal(initial[DeploymentCapability.EDGE_WAF].providerId, 'provider-a');
  assert.equal(replacement[DeploymentCapability.EDGE_WAF].providerId, 'provider-b');
  assert.equal(capabilityIsUsable(initial, DeploymentCapability.EDGE_WAF), true);
  assert.equal(capabilityIsUsable(replacement, DeploymentCapability.EDGE_WAF), true);
  assert.equal(replacement[DeploymentCapability.COMPUTE].state, BindingState.UNBOUND);
});

test('candidate or configured provider is not treated as verified runtime capability', () => {
  for (const state of [BindingState.CANDIDATE, BindingState.CONFIGURED]) {
    const profile = createDeploymentProfile({
      [DeploymentCapability.COMPUTE]: { providerId: 'compute-provider', state }
    });
    assert.equal(capabilityIsUsable(profile, DeploymentCapability.COMPUTE), false);
  }
});

test('provider binding rejects secret material and unknown provider-specific fields', () => {
  assert.deepEqual(createProviderBinding({ providerId: 'edge-provider' }), {
    providerId: 'edge-provider',
    state: BindingState.CANDIDATE,
    reference: null
  });

  assert.throws(
    () => createProviderBinding({ providerId: 'edge-provider', apiToken: 'do-not-store' }),
    /must not contain secret material/
  );
  assert.throws(
    () => createProviderBinding({ providerId: 'edge-provider', vendorRegion: 'region-1' }),
    /unsupported deployment binding field/
  );
  assert.throws(() => createProviderBinding({ providerId: '' }), /non-empty string/);
});

test('profile rejects unknown capabilities instead of silently accepting provider-specific features', () => {
  assert.throws(
    () => createDeploymentProfile({ PROVIDER_MAGIC_EDGE_MODE: { providerId: 'provider-a' } }),
    /unsupported deployment capability/
  );
});
