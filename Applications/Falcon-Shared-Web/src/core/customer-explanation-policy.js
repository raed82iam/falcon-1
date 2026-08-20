export const CustomerConversationMode = Object.freeze({
  ORDINARY:'ORDINARY',
  INCIDENT:'INCIDENT'
});

function nonEmpty(value) {
  return typeof value === 'string' && value.trim() !== '';
}

export function decideCustomerExplanationAccess({
  session,
  projection,
  conversationMode = CustomerConversationMode.ORDINARY
} = {}) {
  if (!session?.authoritativeSession || !nonEmpty(session.principalId) || !nonEmpty(session.tenantId)) {
    return Object.freeze({ allowed:false, reason:'AUTHORITATIVE_CUSTOMER_SESSION_REQUIRED', executionAuthorityCreated:false });
  }
  if (!projection || typeof projection !== 'object') {
    return Object.freeze({ allowed:false, reason:'AUTHORITATIVE_PROJECTION_REQUIRED', executionAuthorityCreated:false });
  }
  if (!nonEmpty(projection.tenantId) || projection.tenantId !== session.tenantId) {
    return Object.freeze({ allowed:false, reason:'TENANT_MISMATCH', executionAuthorityCreated:false });
  }
  if (!nonEmpty(projection.principalId) || projection.principalId !== session.principalId) {
    return Object.freeze({ allowed:false, reason:'PRINCIPAL_MISMATCH', executionAuthorityCreated:false });
  }
  if (!Object.values(CustomerConversationMode).includes(conversationMode)) {
    return Object.freeze({ allowed:false, reason:'UNSUPPORTED_CONVERSATION_MODE', executionAuthorityCreated:false });
  }
  if (conversationMode === CustomerConversationMode.INCIDENT && !nonEmpty(projection.incidentId)) {
    return Object.freeze({ allowed:false, reason:'INCIDENT_ID_REQUIRED', executionAuthorityCreated:false });
  }

  return Object.freeze({
    allowed:true,
    reason:'BOUND_CUSTOMER_EXPLANATION',
    conversationMode,
    explanationOnly:true,
    analysisAuthorityCreated:false,
    executionAuthorityCreated:false,
    brokerAuthorityCreated:false,
    longTermMemoryWriteAuthorized:false
  });
}

export function normalizeExplanationTruth({ truthState='UNKNOWN', freshnessState='UNKNOWN', limitations=[] } = {}) {
  const current = truthState === 'CURRENT' && freshnessState === 'CURRENT';
  return Object.freeze({
    truthState:String(truthState ?? 'UNKNOWN'),
    freshnessState:String(freshnessState ?? 'UNKNOWN'),
    limitations:Object.freeze(Array.isArray(limitations) ? limitations.map(item=>String(item)) : []),
    mayPresentAsCurrent:current,
    uncertaintyRequired:!current
  });
}

export const CustomerExplanationInvariants = Object.freeze({
  explanationEqualsAnalysisTruthOwner:false,
  explanationEqualsTradingDecision:false,
  ordinaryChatEqualsIncidentConversation:false,
  personalizationEqualsSelfDevelopment:false,
  longTermMemoryImplicitlyAuthorized:false,
  tenantIsolationRequired:true
});
