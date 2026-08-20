import {
  validateOwnerUpdateProposal,
  evaluateStandingPreapprovalEligibility
} from './owner-update-governance-v1.js';

const TRUE_CLAIM_KEYS = Object.freeze([
  'autoAccepted','AutoAccepted','AUTO_ACCEPTED',
  'ownerApproved','OwnerApproved','OWNER_APPROVED',
  'rollbackAuthorized','RollbackAuthorized','ROLLBACK_AUTHORIZED',
  'producerClaimsAutoAccept','ProducerClaimsAutoAccept',
  'producerClaimsRollbackAuthority','ProducerClaimsRollbackAuthority'
]);

function hasProducerAuthorityClaim(raw) {
  if (!raw || typeof raw !== 'object' || Array.isArray(raw)) return false;
  return TRUE_CLAIM_KEYS.some(key => raw[key] === true || raw[key] === 'true' || raw[key] === 'AUTO_ACCEPTED' || raw[key] === 'OWNER_APPROVED' || raw[key] === 'ROLLBACK_AUTHORIZED');
}

/**
 * Cross-workstream ingress guard for Application/AI proposals.
 * Producers provide facts only and cannot attach Owner authority outcomes.
 */
export function validateOwnerProposalIngress(raw) {
  if (hasProducerAuthorityClaim(raw)) {
    return Object.freeze({
      valid:false,
      reason:'APPLICATION_AI_SELF_APPROVAL_FORBIDDEN',
      proposal:null
    });
  }
  return validateOwnerUpdateProposal(raw);
}

export function evaluateOwnerProposalIngress(rawProposal,rawPolicy) {
  const ingress=validateOwnerProposalIngress(rawProposal);
  if (!ingress.valid) {
    return Object.freeze({
      disposition:'MANUAL_OWNER_REVIEW_REQUIRED',
      reason:ingress.reason,
      proposal:ingress.proposal
    });
  }
  return evaluateStandingPreapprovalEligibility(rawProposal,rawPolicy);
}

export const __test = Object.freeze({ hasProducerAuthorityClaim, TRUE_CLAIM_KEYS });
