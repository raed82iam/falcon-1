import { TruthState } from '../contracts.js';
import {
  FsatsRuntimePortMethods,
  createUnavailableFsatsRuntimePort
} from './ports/fsats-runtime-port.js';
import {
  FalconSystemRuntimePortMethods,
  createUnavailableFalconSystemRuntimePort
} from './ports/falcon-system-runtime-port.js';
import {
  WebMarketDataPortMethods,
  createUnavailableWebMarketDataPort
} from './ports/web-market-data-port.js';

/**
 * Shared Web aggregate runtime port.
 *
 * Presentation depends on this Web-owned boundary, never on Foundation or
 * Application internals. The aggregate preserves separate owning contract
 * families while giving composition one validated runtime dependency.
 */
export const RuntimePortMethods = Object.freeze([
  ...FalconSystemRuntimePortMethods,
  ...FsatsRuntimePortMethods,
  ...WebMarketDataPortMethods
]);

export function assertRuntimePort(port) {
  if (!port || typeof port !== 'object') {
    throw new TypeError('Runtime port must be an object.');
  }

  for (const method of RuntimePortMethods) {
    if (typeof port[method] !== 'function') {
      throw new TypeError(`Runtime port is missing method: ${method}`);
    }
  }

  return port;
}

export function unavailableProjection(extra = {}) {
  return Object.freeze({ truth: TruthState.UNAVAILABLE, ...extra });
}

export function createUnavailableRuntimePort() {
  return Object.freeze({
    ...createUnavailableFalconSystemRuntimePort(),
    ...createUnavailableFsatsRuntimePort(),
    ...createUnavailableWebMarketDataPort()
  });
}
