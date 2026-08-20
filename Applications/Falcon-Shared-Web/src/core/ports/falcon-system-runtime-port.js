import { TruthState } from '../../contracts.js';

/**
 * Web-owned Falcon system projection boundary.
 *
 * These methods expose only the system/Application projections Shared Web needs.
 * They do not grant Foundation authority or permit access to Foundation internals.
 */
export const FalconSystemRuntimePortMethods = Object.freeze([
  'applications',
  'systemOverview',
  'recoveryOperational'
]);

const unavailable = extra => Object.freeze({ truth: TruthState.UNAVAILABLE, ...extra });

export function createUnavailableFalconSystemRuntimePort() {
  return Object.freeze({
    async applications() { return unavailable({ items: [] }); },
    async systemOverview() { return unavailable(); },
    async recoveryOperational() {
      return unavailable({
        presentationOnly:true,
        mayAuthorizeRelease:false,
        mayExecuteRelease:false,
        mayChangeLifecycle:false
      });
    }
  });
}
