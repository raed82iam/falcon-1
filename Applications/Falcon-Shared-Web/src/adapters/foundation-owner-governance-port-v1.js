import {
  adaptFoundationAutoAcceptDecision,
  adaptFoundationPolicyManagementDecision,
  adaptFoundationRollbackOrderDecision
} from './foundation-owner-standing-preapproval-v1.js';
import {
  createFoundationOwnerGovernanceTransportAdapter
} from './foundation-owner-governance-fil-v1.js';

/**
 * Exact FCR-0241 + FCR-0237/FCR-0238 Web consuming adapter.
 *
 * Layer 1 validates the canonical Foundation FIL request/response family.
 * Layer 2 validates the Foundation-owned semantic decision payload and its
 * authority separations. Neither layer activates a Service Bus route.
 */
export function createFoundationOwnerGovernancePortAdapter({exchange,identityFactory,clock=()=>Date.now()}={}) {
  const transport=createFoundationOwnerGovernanceTransportAdapter({exchange,identityFactory,clock});

  async function consume(call,semanticAdapter) {
    const result=await call();
    if(!result?.accepted) return result;
    const now=Number(clock());
    try {
      return Object.freeze({
        accepted:true,
        reason:'OWNER_GOVERNANCE_TRANSPORT_AND_SEMANTICS_ACCEPTED',
        decision:semanticAdapter(result.payload,{now}),
        transport:Object.freeze({
          messageId:result.envelope.messageId,
          correlationId:result.envelope.correlationId,
          causationId:result.envelope.causationId,
          profileFamily:result.profile.familyIdentity,
          responseRouteIdentity:result.profile.responseRouteIdentity
        })
      });
    } catch(error) {
      return Object.freeze({
        accepted:false,
        reason:'OWNER_GOVERNANCE_SEMANTIC_DECISION_REJECTED',
        detail:error instanceof Error?error.message:'semantic decision rejected',
        decision:null,
        transport:null
      });
    }
  }

  return Object.freeze({
    async manageStandingPolicy(payload) {
      return consume(()=>transport.manageStandingPolicy(payload),adaptFoundationPolicyManagementDecision);
    },
    async evaluateStandingProposal(payload) {
      return consume(()=>transport.evaluateStandingProposal(payload),adaptFoundationAutoAcceptDecision);
    },
    async requestRollback(payload) {
      return consume(()=>transport.requestRollback(payload),adaptFoundationRollbackOrderDecision);
    }
  });
}
