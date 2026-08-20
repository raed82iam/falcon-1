export const WebAwarenessSubject = Object.freeze({
  MSA:'SHARED_WEB_MSA',
  CUSTOMER_SUPPORT_LSA:'SHARED_WEB_CUSTOMER_SUPPORT_LSA'
});

export const WebOwnership = Object.freeze({
  WEB:'WEB',
  APPLICATION:'APPLICATION',
  FOUNDATION:'FOUNDATION',
  GOVERNANCE:'GOVERNANCE',
  UNKNOWN:'UNKNOWN'
});

const WEB_SCOPE_PREFIXES = Object.freeze([
  'applications/shared/web/'
]);

const FOREIGN_HINTS = Object.freeze({
  FOUNDATION:['foundation','kernel','shared service','lifecycle','service catalog','dependency manager'],
  APPLICATION:['fsats','trading','strategy','school','guardian','broker','portfolio','risk rule','execution'],
  GOVERNANCE:['constitution','architecture board','governance rule','amend constitution']
});

const WEB_HINTS = Object.freeze(['web ','website','shared web','واجهة الويب','الموقع']);

function normalized(value) {
  return String(value ?? '').trim().toLowerCase();
}

function hintedOwners(request) {
  const matches=new Set();
  for (const [owner,hints] of Object.entries(FOREIGN_HINTS)) {
    if (hints.some(hint=>request.includes(hint))) matches.add(WebOwnership[owner]);
  }
  if (WEB_HINTS.some(hint=>request.includes(hint))) matches.add(WebOwnership.WEB);
  return matches;
}

export function classifyOwnerRequestOwnership({ text = '', targetPath = null } = {}) {
  const path=normalized(targetPath);
  if (path) {
    if (WEB_SCOPE_PREFIXES.some(prefix=>path.startsWith(prefix))) return WebOwnership.WEB;
    if (path.startsWith('applications/')) return WebOwnership.APPLICATION;
    if (path.startsWith('src/foundation') || path.startsWith('docs/foundation') || path.includes('/foundation.')) return WebOwnership.FOUNDATION;
  }

  const request=normalized(text);
  if (!request) return WebOwnership.UNKNOWN;
  const matches=hintedOwners(request);
  if (matches.size !== 1) return WebOwnership.UNKNOWN;
  return [...matches][0];
}

export function decideWebMsaDevelopment({ ownerDirectRequest = false, ownership = WebOwnership.UNKNOWN, operation = 'DEVELOPMENT' } = {}) {
  if (operation !== 'DEVELOPMENT') {
    return Object.freeze({ allowed:false, reason:'UNSUPPORTED_OPERATION', authorityCreated:false });
  }
  if (ownerDirectRequest !== true) {
    return Object.freeze({ allowed:false, reason:'OWNER_DIRECT_REQUEST_REQUIRED', authorityCreated:false });
  }
  if (ownership !== WebOwnership.WEB) {
    return Object.freeze({ allowed:false, reason:'FOREIGN_OR_UNKNOWN_SCOPE_MUST_BE_ROUTED', authorityCreated:false });
  }
  return Object.freeze({
    allowed:true,
    reason:'OWNER_DIRECT_WEB_REQUEST',
    authorityCreated:false,
    validationRequired:true,
    redTeamRequired:true
  });
}

export function decideWebMsaResearch({ purpose } = {}) {
  const normalizedPurpose=normalized(purpose);
  if (['self-development','web-development','redesign','evolution','autonomous-improvement'].includes(normalizedPurpose)) {
    return Object.freeze({ allowed:false, reason:'WEB_MSA_RESEARCH_FOR_SELF_DEVELOPMENT_DISABLED', authoritativeTruthCreated:false });
  }
  return Object.freeze({ allowed:false, reason:'WEB_MSA_GENERAL_RESEARCH_NOT_AUTHORIZED', authoritativeTruthCreated:false });
}

export function decideCustomerSupportLsaResearch({ purpose } = {}) {
  const normalizedPurpose=normalized(purpose);
  if (['customer-support','incident-support','customer-assistance'].includes(normalizedPurpose)) {
    return Object.freeze({
      allowed:true,
      reason:'CUSTOMER_SUPPORT_ASSISTANCE_ONLY',
      authoritativeTruthCreated:false,
      developmentAuthorityCreated:false
    });
  }
  return Object.freeze({ allowed:false, reason:'LSA_RESEARCH_OUTSIDE_SUPPORT_SCOPE', authoritativeTruthCreated:false, developmentAuthorityCreated:false });
}

export const WebAwarenessInvariants = Object.freeze({
  autonomousSelfDevelopment:false,
  researchForSelfDevelopment:false,
  ownerDirectRequestRequiredForWebDevelopment:true,
  foreignWorkstreamImplementation:false,
  customerSupportLsaSelfDevelopment:false,
  customerSupportResearchOnly:true,
  selfAwarenessCreatesAuthority:false,
  requestSentEqualsAccepted:false,
  acceptedEqualsCompleted:false,
  ambiguousOwnershipFailsClosed:true
});

export const __test = Object.freeze({ hintedOwners });
