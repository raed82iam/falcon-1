import {
  assertRuntimePort,
  createUnavailableRuntimePort
} from './core/runtime-port.js';

/**
 * Runtime adapter factory.
 *
 * Production transport is deliberately not invented here. When governed
 * Foundation/Application routes become available, their transport adapter
 * must implement the stable Web-owned runtime port and pass validation before
 * the presentation layer consumes it.
 */
export function createRuntimeAdapter(candidate = null) {
  if (candidate === null) {
    return createUnavailableRuntimePort();
  }

  return assertRuntimePort(candidate);
}
