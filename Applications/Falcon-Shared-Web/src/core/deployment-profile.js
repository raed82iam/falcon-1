/**
 * Vendor-neutral deployment capability model for Shared Falcon Web.
 *
 * This module describes capabilities the Web runtime requires. It does not
 * select, configure, authorize, or deploy any external provider.
 */

export const DeploymentCapability = Object.freeze({
  EDGE_DNS: 'EDGE_DNS',
  EDGE_CDN: 'EDGE_CDN',
  EDGE_WAF: 'EDGE_WAF',
  TLS_TERMINATION: 'TLS_TERMINATION',
  COMPUTE: 'COMPUTE',
  OBJECT_STORAGE: 'OBJECT_STORAGE',
  SECRETS: 'SECRETS',
  OBSERVABILITY: 'OBSERVABILITY'
});

export const BindingState = Object.freeze({
  UNBOUND: 'UNBOUND',
  CANDIDATE: 'CANDIDATE',
  CONFIGURED: 'CONFIGURED',
  VERIFIED: 'VERIFIED'
});

const CAPABILITIES = Object.freeze(Object.values(DeploymentCapability));
const STATES = new Set(Object.values(BindingState));
const PROVIDER_BINDING_KEYS = new Set(['providerId', 'state', 'reference']);
const UNBOUND_BINDING = Object.freeze({ providerId: null, state: BindingState.UNBOUND, reference: null });

function assertNonEmptyString(value, name) {
  if (typeof value !== 'string' || value.trim() === '') {
    throw new TypeError(`${name} must be a non-empty string`);
  }
  return value.trim();
}

function validateBindingShape(input) {
  if (!input || typeof input !== 'object' || Array.isArray(input)) {
    throw new TypeError('deployment binding must be an object');
  }

  const forbidden = /secret|password|token|private.?key|credential/i;
  for (const key of Object.keys(input)) {
    if (forbidden.test(key)) {
      throw new TypeError(`deployment binding must not contain secret material: ${key}`);
    }
    if (!PROVIDER_BINDING_KEYS.has(key)) {
      throw new TypeError(`unsupported deployment binding field: ${key}`);
    }
  }
}

export function createProviderBinding(input = {}) {
  validateBindingShape(input);
  const { providerId, state = BindingState.CANDIDATE, reference = null } = input;
  const normalizedProviderId = assertNonEmptyString(providerId, 'providerId');
  if (!STATES.has(state) || state === BindingState.UNBOUND) {
    throw new TypeError(`unsupported provider binding state: ${state}`);
  }
  if (reference !== null && (typeof reference !== 'string' || reference.trim() === '')) {
    throw new TypeError('reference must be null or a non-empty string');
  }

  return Object.freeze({
    providerId: normalizedProviderId,
    state,
    reference: reference?.trim() ?? null
  });
}

export function createDeploymentProfile(bindings = {}) {
  if (!bindings || typeof bindings !== 'object' || Array.isArray(bindings)) {
    throw new TypeError('deployment profile bindings must be an object');
  }

  for (const capability of Object.keys(bindings)) {
    if (!CAPABILITIES.includes(capability)) {
      throw new TypeError(`unsupported deployment capability: ${capability}`);
    }
  }

  const profile = {};
  for (const capability of CAPABILITIES) {
    const candidate = bindings[capability];
    profile[capability] = !candidate || candidate.state === BindingState.UNBOUND
      ? UNBOUND_BINDING
      : createProviderBinding(candidate);
  }
  return Object.freeze(profile);
}

export function replaceProvider(profile, capability, binding) {
  if (!CAPABILITIES.includes(capability)) throw new TypeError(`unsupported deployment capability: ${capability}`);

  const boundOnly = {};
  for (const item of CAPABILITIES) {
    const current = profile?.[item];
    if (current && current.state !== BindingState.UNBOUND) boundOnly[item] = current;
  }

  boundOnly[capability] = binding;
  return createDeploymentProfile(boundOnly);
}

export function capabilityIsUsable(profile, capability) {
  const binding = profile?.[capability];
  return Boolean(binding && binding.state === BindingState.VERIFIED);
}

export const unboundDeploymentProfile = createDeploymentProfile();
