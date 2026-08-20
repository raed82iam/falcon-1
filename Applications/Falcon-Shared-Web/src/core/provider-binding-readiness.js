const nonEmpty = value => typeof value === 'string' && value.trim().length > 0;

const WEB_CREDENTIAL_REFERENCE = /^credref:web:[A-Za-z0-9][A-Za-z0-9._-]*(?::[A-Za-z0-9][A-Za-z0-9._-]*)*$/;

export function isWebCredentialReference(value) {
  return typeof value === 'string'
    && value === value.trim()
    && value.length <= 200
    && WEB_CREDENTIAL_REFERENCE.test(value);
}

export const ProviderBindingDecision = Object.freeze({
  READY_FOR_GOVERNED_VERIFICATION:'READY_FOR_GOVERNED_VERIFICATION',
  FAIL_CLOSED:'FAIL_CLOSED'
});

/**
 * Web-only readiness guard for exact presentation-provider bindings.
 *
 * This does not activate a network connection, create a principal, resolve a
 * credential secret, or grant provider authority. It only answers whether the
 * Web-side binding metadata is complete enough to enter governed verification.
 * Credential-bearing routes accept only a Web-owned opaque credential-reference
 * identity. Raw provider tokens, API keys, passwords and query-string secrets
 * are not credential references and therefore fail closed here.
 */
export function evaluateProviderBindingReadiness({
  route,
  expectedFcr,
  expectedUrl,
  expectedPathTemplate = null,
  webPrincipalId,
  webServiceRole,
  credentialReference = null,
  channelCredentialRequired = null,
  routePolicyBound = false,
  exactRouteVerified = false
} = {}) {
  if (!route || route.foundationDisposition !== 'STAGE12_ACCEPTED_AND_CLOSED') {
    return Object.freeze({ decision:ProviderBindingDecision.FAIL_CLOSED, reason:'FOUNDATION_ROUTE_NOT_READY' });
  }

  if (route.activation !== 'WEB_BINDING_AND_VERIFICATION_PENDING') {
    return Object.freeze({ decision:ProviderBindingDecision.FAIL_CLOSED, reason:'ROUTE_STATE_NOT_BINDING_PENDING' });
  }

  if (!nonEmpty(expectedFcr) || route.fcr !== expectedFcr) {
    return Object.freeze({ decision:ProviderBindingDecision.FAIL_CLOSED, reason:'EXACT_FCR_ROUTE_IDENTITY_MISMATCH' });
  }

  if (!nonEmpty(expectedUrl) || route.url !== expectedUrl) {
    return Object.freeze({ decision:ProviderBindingDecision.FAIL_CLOSED, reason:'EXACT_ROUTE_MISMATCH' });
  }

  const routePathTemplate = route.pathTemplate ?? null;
  if (routePathTemplate !== expectedPathTemplate) {
    return Object.freeze({ decision:ProviderBindingDecision.FAIL_CLOSED, reason:'EXACT_ROUTE_PATH_TEMPLATE_MISMATCH' });
  }

  if (!nonEmpty(webPrincipalId) || !nonEmpty(webServiceRole)) {
    return Object.freeze({ decision:ProviderBindingDecision.FAIL_CLOSED, reason:'WEB_PRINCIPAL_OR_SERVICE_ROLE_MISSING' });
  }

  if (routePolicyBound !== true) {
    return Object.freeze({ decision:ProviderBindingDecision.FAIL_CLOSED, reason:'WEB_ROUTE_POLICY_NOT_BOUND' });
  }

  if (route.credentialMode === 'API_CREDENTIAL_REFERENCE') {
    if (credentialReference === null || credentialReference === undefined || credentialReference === '') {
      return Object.freeze({ decision:ProviderBindingDecision.FAIL_CLOSED, reason:'CREDENTIAL_REFERENCE_REQUIRED' });
    }
    if (!isWebCredentialReference(credentialReference)) {
      return Object.freeze({ decision:ProviderBindingDecision.FAIL_CLOSED, reason:'INVALID_WEB_CREDENTIAL_REFERENCE' });
    }
  } else if (route.credentialMode === 'CHANNEL_DEPENDENT') {
    if (typeof channelCredentialRequired !== 'boolean') {
      return Object.freeze({ decision:ProviderBindingDecision.FAIL_CLOSED, reason:'CHANNEL_AUTH_REQUIREMENT_UNKNOWN' });
    }
    if (channelCredentialRequired) {
      if (credentialReference === null || credentialReference === undefined || credentialReference === '') {
        return Object.freeze({ decision:ProviderBindingDecision.FAIL_CLOSED, reason:'CREDENTIAL_REFERENCE_REQUIRED' });
      }
      if (!isWebCredentialReference(credentialReference)) {
        return Object.freeze({ decision:ProviderBindingDecision.FAIL_CLOSED, reason:'INVALID_WEB_CREDENTIAL_REFERENCE' });
      }
    }
    if (!channelCredentialRequired && credentialReference !== null) {
      return Object.freeze({ decision:ProviderBindingDecision.FAIL_CLOSED, reason:'UNNEEDED_CREDENTIAL_REFERENCE' });
    }
  } else if (route.credentialMode === 'PUBLIC') {
    if (credentialReference !== null) {
      return Object.freeze({ decision:ProviderBindingDecision.FAIL_CLOSED, reason:'UNNEEDED_CREDENTIAL_REFERENCE' });
    }
  } else {
    return Object.freeze({ decision:ProviderBindingDecision.FAIL_CLOSED, reason:'UNKNOWN_CREDENTIAL_MODE' });
  }

  if (exactRouteVerified !== true) {
    return Object.freeze({ decision:ProviderBindingDecision.FAIL_CLOSED, reason:'WEB_GOVERNED_VERIFICATION_PENDING' });
  }

  return Object.freeze({
    decision:ProviderBindingDecision.READY_FOR_GOVERNED_VERIFICATION,
    reason:null,
    connectivityActivated:false
  });
}
