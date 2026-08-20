import { assertRuntimePort, createUnavailableRuntimePort } from '../core/runtime-port.js';

/**
 * Web-owned composition adapter for Foundation public runtime projections.
 *
 * It decorates the stable aggregate runtime port. Foundation transports stay
 * behind their exact consuming adapters, while presentation sees only the
 * Web-owned runtime boundary. Contract availability never implies route or
 * deployment activation.
 *
 * Stage 14 operational truth may be supplied only through the governed
 * FCR-0239 exact FIL/public-runtime consumer adapter. When that adapter is not
 * composed, systemOverview remains fail-closed through the base runtime port.
 * This layer never invents route/schema/artifact identities itself.
 */
export function createFoundationRuntimePortBinding({
  baseRuntimePort = null,
  recoveryAdapter,
  operationalAdapter = null
} = {}) {
  const base = baseRuntimePort ?? createUnavailableRuntimePort();
  assertRuntimePort(base);
  if (!recoveryAdapter || typeof recoveryAdapter.readRecoveryProjection !== 'function') {
    throw new TypeError('recoveryAdapter.readRecoveryProjection is required');
  }
  if (operationalAdapter !== null && typeof operationalAdapter?.readOperationalProjection !== 'function') {
    throw new TypeError('operationalAdapter.readOperationalProjection must be a function when provided');
  }

  return assertRuntimePort(Object.freeze({
    ...base,
    async systemOverview(reference) {
      if (!operationalAdapter) return base.systemOverview(reference);
      return operationalAdapter.readOperationalProjection(reference);
    },
    async recoveryOperational(reference) {
      return recoveryAdapter.readRecoveryProjection(reference);
    }
  }));
}
