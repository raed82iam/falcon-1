const unavailable = (operation, extra = {}) => Object.freeze({
  available:false,
  operation,
  reason:'OWNER_UPDATE_GOVERNANCE_TRANSPORT_UNAVAILABLE',
  ...extra
});

/**
 * Web-owned stable boundary for FCR-0237/FCR-0238 Owner update governance.
 *
 * This port never implements Foundation authority or Application business
 * rollback. Falcon-native request/response transport is injected behind it.
 */
export const OwnerUpdateGovernancePortMethods = Object.freeze([
  'standingPolicies',
  'proposalInbox',
  'autoAcceptedHistory',
  'evaluateStandingProposal',
  'manageStandingPolicy',
  'requestRollback',
  'rollbackStatus'
]);

export function assertOwnerUpdateGovernancePort(port) {
  if (!port || typeof port !== 'object') throw new TypeError('Owner update governance port must be an object');
  for (const method of OwnerUpdateGovernancePortMethods) {
    if (typeof port[method] !== 'function') throw new TypeError(`Owner update governance port is missing method: ${method}`);
  }
  return port;
}

export function createUnavailableOwnerUpdateGovernancePort() {
  return Object.freeze({
    async standingPolicies() { return unavailable('standingPolicies',{items:Object.freeze([])}); },
    async proposalInbox() { return unavailable('proposalInbox',{items:Object.freeze([])}); },
    async autoAcceptedHistory() { return unavailable('autoAcceptedHistory',{items:Object.freeze([])}); },
    async evaluateStandingProposal() { return unavailable('evaluateStandingProposal'); },
    async manageStandingPolicy() { return unavailable('manageStandingPolicy'); },
    async requestRollback() { return unavailable('requestRollback'); },
    async rollbackStatus() { return unavailable('rollbackStatus'); }
  });
}

export function createOwnerUpdateGovernancePortBinding(candidate = null) {
  return candidate === null
    ? createUnavailableOwnerUpdateGovernancePort()
    : assertOwnerUpdateGovernancePort(candidate);
}

/**
 * Decorates a complete Web-owned governance port with the three exact
 * Foundation FCR-0241 command/query transports. Read models and rollback
 * execution/status remain owned by their existing authoritative sources.
 *
 * Transport availability does not activate a route and does not create Owner,
 * execution, deployment, rollback-execution or business authority.
 */
export function createOwnerUpdateGovernanceTransportPort({basePort=null,transportAdapter}={}) {
  const base=basePort??createUnavailableOwnerUpdateGovernancePort();
  assertOwnerUpdateGovernancePort(base);
  if(!transportAdapter||typeof transportAdapter.manageStandingPolicy!=='function'||typeof transportAdapter.evaluateStandingProposal!=='function'||typeof transportAdapter.requestRollback!=='function') {
    throw new TypeError('FCR-0241 transportAdapter must implement manageStandingPolicy, evaluateStandingProposal, and requestRollback');
  }
  return assertOwnerUpdateGovernancePort(Object.freeze({
    ...base,
    async manageStandingPolicy(request){return transportAdapter.manageStandingPolicy(request);},
    async evaluateStandingProposal(request){return transportAdapter.evaluateStandingProposal(request);},
    async requestRollback(request){return transportAdapter.requestRollback(request);}
  }));
}
