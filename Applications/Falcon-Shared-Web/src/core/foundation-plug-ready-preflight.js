const Expected = Object.freeze({
  applicationIdentity:'FALCON_SHARED_WEB',
  manifestIdentity:'SHARED_WEB_APPLICATION_ADMISSION_MANIFEST_V1',
  manifestVersion:'1.0',
  con023:'1.1',
  con001:'1.0',
  integrationProfiles:Object.freeze(['FDN-006@1.0','FDN-007@1.0']),
  credentialFcrs:Object.freeze(['FCR-0176','FCR-0177','FCR-0196','FCR-0197']),
  publicProviderFcrs:Object.freeze(['FCR-0173','FCR-0174','FCR-0175','FCR-0198','FCR-0199','FCR-0200'])
});

const forbiddenAuthorityFlags = Object.freeze([
  'activationRequested',
  'deploymentRequested',
  'connectivityRequested',
  'productionUseRequested',
  'businessAuthorityRequested',
  'tradingAuthorityRequested'
]);

const requiredNoLinkFlags = Object.freeze([
  'actualAdmissionExecuted',
  'actualCanonicalRuntimeRegistrationExecuted',
  'runtimeActivationExecuted',
  'deploymentExecuted',
  'providerConnectivityExecuted',
  'productionUseAuthorized',
  'businessAuthorityGranted',
  'tradingAuthorityGranted'
]);

function sameMembers(actual, expected) {
  if (!Array.isArray(actual) || actual.length !== expected.length) return false;
  return [...actual].sort().every((value,index) => value === [...expected].sort()[index]);
}

function allBindingModesAreOperationTime(candidate) {
  const keys = [
    'exactArtifactIdentityBindingMode',
    'positiveCanonicalAdmissionEvidenceBindingMode',
    'lifecycleAttachEligibilityAndDecisionIdentityBindingMode',
    'resourceGrantBindingMode',
    'observedAtBindingMode'
  ];
  return keys.every(key => candidate?.[key] === 'AUTHORITATIVE_AT_OPERATION');
}

function noSecretMaterial(preparation) {
  const text = JSON.stringify(preparation ?? {});
  const secretFieldPattern = /"(?:password|secret|apiKey|privateKey|accessToken|refreshToken)"\s*:/i;
  return preparation?.secretSafety?.rawSecretBytesPresent === false
    && preparation?.secretSafety?.credentialReferencesAreOpaqueIdentifiersOnly === true
    && preparation?.secretSafety?.ordinaryWebStateMayContainSecretBytes === false
    && !secretFieldPattern.test(text);
}

export function evaluateWebFoundationPlugReadyPreparation(preparation, manifest) {
  const checks = Object.freeze({
    preparationIdentity:preparation?.preparationIdentity === 'WEB_FOUNDATION_PLUG_READY_PREPARATION_V1',
    requestPairShape:preparation?.application?.admissionCandidateCount === 1
      && preparation?.application?.runtimeRegistrationTemplateCount === 1
      && preparation?.application?.requestPairCount === 1,
    manifestIdentity:manifest?.application?.identity === Expected.applicationIdentity
      && manifest?.manifestIdentity === Expected.manifestIdentity
      && manifest?.manifestVersion === Expected.manifestVersion
      && preparation?.admissionCandidate?.applicationIdentity === manifest?.application?.identity
      && preparation?.admissionCandidate?.manifestIdentity === manifest?.manifestIdentity
      && preparation?.admissionCandidate?.manifestVersion === manifest?.manifestVersion,
    contracts:preparation?.foundationContractBaseline?.applicationContract?.identity === 'CON-023'
      && preparation?.foundationContractBaseline?.applicationContract?.version === Expected.con023
      && preparation?.foundationContractBaseline?.genericApplicationDependency?.identity === 'CON-001'
      && preparation?.foundationContractBaseline?.genericApplicationDependency?.version === Expected.con001
      && sameMembers(preparation?.foundationContractBaseline?.integrationProfiles, Expected.integrationProfiles),
    foundationNeutrality:preparation?.foundationContractBaseline?.genericAdmissionRuntimePath === 'EXISTING_SEALED_FOUNDATION_CAPABILITY'
      && preparation?.foundationContractBaseline?.foundationChangeRequired === false
      && manifest?.foundationNeutrality?.foundationChangeRequiredForWebFit === false
      && manifest?.foundationNeutrality?.webMustAdaptToSealedFoundation === true,
    bindAtOperation:allBindingModesAreOperationTime(preparation?.admissionCandidate)
      && preparation?.runtimeRegistrationTemplate?.registrationBindingMode === 'AUTHORITATIVE_AT_OPERATION'
      && preparation?.runtimeRegistrationTemplate?.runtimePortBindingMode === 'AUTHORITATIVE_AT_OPERATION'
      && preparation?.preflightSemantics?.runtimeCurrentValues === 'BIND_AT_OPERATION'
      && preparation?.preflightSemantics?.missingBindAtOperationValuesInvalidatePreparation === false
      && preparation?.preflightSemantics?.missingBindAtOperationValuesPreventActualOperation === true,
    credentialSeparation:sameMembers(
      preparation?.bindAtOperation?.credentialReferences?.map(item => item?.fcr),
      Expected.credentialFcrs
    )
      && preparation?.bindAtOperation?.credentialReferences?.every(item => item?.secretBytesAllowed === false) === true
      && sameMembers(preparation?.bindAtOperation?.publicProviderRoutesWithoutCredentialReference, Expected.publicProviderFcrs),
    noAuthority:forbiddenAuthorityFlags.every(key => preparation?.runtimeRegistrationTemplate?.[key] === false)
      && requiredNoLinkFlags.every(key => preparation?.mandatoryNoLinkState?.[key] === false),
    secretSafety:noSecretMaterial(preparation),
    manifestFailClosed:manifest?.securityAndIsolation?.failClosed === true
      && manifest?.securityAndIsolation?.ordinaryWebStateMayContainSecretBytes === false
      && manifest?.declarationSemantics?.admissionIsActivation === false
      && manifest?.declarationSemantics?.registrationIsActivation === false
  });

  const failedChecks = Object.entries(checks).filter(([,value]) => value !== true).map(([name]) => name);
  const ready = failedChecks.length === 0;

  return Object.freeze({
    status:ready ? 'FULL_PLUG_READY_PREFLIGHT_VERIFIED_BY_COMPOSITION' : 'PLUG_READY_PREFLIGHT_BLOCKED',
    webPreparation:ready ? 'READY' : 'BLOCKED',
    foundationGenericCapability:ready ? 'READY' : 'UNVERIFIED',
    fullPlugReadyContractPreflight:ready ? 'VERIFIED' : 'BLOCKED',
    fullPlugReadyPreflight:ready ? 'VERIFIED_BY_COMPOSITION' : 'BLOCKED',
    runtimeCurrentValues:'BIND_AT_OPERATION',
    actualAdmissionExecuted:false,
    actualCanonicalRuntimeRegistrationExecuted:false,
    activationExecuted:false,
    deploymentExecuted:false,
    connectivityExecuted:false,
    businessAuthorityGranted:false,
    tradingAuthorityGranted:false,
    checks,
    failedChecks:Object.freeze(failedChecks)
  });
}
