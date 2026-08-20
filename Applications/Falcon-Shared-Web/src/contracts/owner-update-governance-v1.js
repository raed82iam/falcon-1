export const OwnerUpdateClass = Object.freeze({
  UNKNOWN:'Unknown',
  MAINTENANCE:'Maintenance',
  MODEL_REFRESH:'ModelRefresh',
  PARAMETER_TUNING:'ParameterTuning',
  STRATEGY_REVISION:'StrategyRevision',
  DATA_SOURCE_CHANGE:'DataSourceChange',
  PRESENTATION_ONLY_SUGGESTION:'PresentationOnlySuggestion',
  BUSINESS_RULE_CHANGE:'BusinessRuleChange',
  RISK_RULE_CHANGE:'RiskRuleChange',
  EXECUTION_BEHAVIOR_CHANGE:'ExecutionBehaviorChange',
  AUTHORITY_OR_SECURITY_CHANGE:'AuthorityOrSecurityChange',
  DEPLOYMENT_OR_ADOPTION_CHANGE:'DeploymentOrAdoptionChange',
  AI_SELF_DEVELOPMENT:'AiSelfDevelopment'
});

export const OwnerUpdateAuthoritySource = Object.freeze({
  UNSPECIFIED:'Unspecified',
  APPLICATION:'Application',
  ARTIFICIAL_INTELLIGENCE:'ArtificialIntelligence',
  OWNER_VIA_SHARED_WEB:'OwnerViaSharedWeb'
});

export const ClassificationAuthoritySource = Object.freeze({
  UNSPECIFIED:'Unspecified',
  PRODUCER_SELF_CLAIM:'ProducerSelfClaim',
  GOVERNED_APPLICATION_CLASSIFIER:'GovernedApplicationClassifier'
});

export const OwnerUpdateImpact = Object.freeze({
  UNKNOWN:'Unknown', LOW:'Low', MODERATE:'Moderate', HIGH:'High', CRITICAL:'Critical'
});

export const OwnerRollbackMode = Object.freeze({ FULL:'Full', BOUNDED_PARTIAL:'BoundedPartial' });
export const OwnerRollbackLifecycle = Object.freeze({
  RECEIVED:'Received', ACCEPTED:'Accepted', REJECTED:'Rejected', EXECUTION_STARTED:'ExecutionStarted',
  EXECUTION_COMPLETED:'ExecutionCompleted', EXECUTION_FAILED:'ExecutionFailed',
  POST_VALIDATION_REQUIRED:'PostRollbackValidationRequired',
  POST_VALIDATION_COMPLETED:'PostRollbackValidationCompleted',
  POST_VALIDATION_FAILED:'PostRollbackValidationFailed'
});

const ELIGIBLE_CLASSES = new Set([
  OwnerUpdateClass.MAINTENANCE,
  OwnerUpdateClass.MODEL_REFRESH,
  OwnerUpdateClass.PARAMETER_TUNING,
  OwnerUpdateClass.PRESENTATION_ONLY_SUGGESTION
]);

const ALL_CLASSES = new Set(Object.values(OwnerUpdateClass));
const IMPACTS = new Set(Object.values(OwnerUpdateImpact));
const token = value => typeof value === 'string' && value.trim().length > 0 && value.trim() === value;
const sha256Hex = value => typeof value === 'string' && /^[0-9a-f]{64}$/iu.test(value);
const list = value => Array.isArray(value) ? value : [];
const distinctNonBlank = values => Array.isArray(values) && values.every(token) && new Set(values).size === values.length;

function pick(value,...names) {
  for (const name of names) if (value && value[name] !== undefined) return value[name];
  return undefined;
}

function normalizeEnum(value, mapping = null) {
  if (typeof value === 'string') return value;
  if (!Number.isInteger(value) || !mapping) return value;
  return mapping[value] ?? value;
}

const CLASS_BY_NUMBER = Object.freeze({
  0:'Unknown',1:'Maintenance',2:'ModelRefresh',3:'ParameterTuning',4:'StrategyRevision',5:'DataSourceChange',
  6:'PresentationOnlySuggestion',7:'BusinessRuleChange',8:'RiskRuleChange',9:'ExecutionBehaviorChange',
  10:'AuthorityOrSecurityChange',11:'DeploymentOrAdoptionChange',12:'AiSelfDevelopment'
});
const CLASSIFIER_BY_NUMBER = Object.freeze({0:'Unspecified',1:'ProducerSelfClaim',2:'GovernedApplicationClassifier'});
const IMPACT_BY_NUMBER = Object.freeze({0:'Unknown',1:'Low',2:'Moderate',3:'High',4:'Critical'});
const AUTHORITY_BY_NUMBER = Object.freeze({0:'Unspecified',1:'Application',2:'ArtificialIntelligence',3:'OwnerViaSharedWeb'});
const ROLLBACK_MODE_BY_NUMBER = Object.freeze({0:'Full',1:'BoundedPartial'});
const ROLLBACK_LIFECYCLE_BY_NUMBER = Object.freeze({
  0:'Received',1:'Accepted',2:'Rejected',3:'ExecutionStarted',4:'ExecutionCompleted',5:'ExecutionFailed',
  6:'PostRollbackValidationRequired',7:'PostRollbackValidationCompleted',8:'PostRollbackValidationFailed'
});

export function normalizeOwnerUpdateProposal(raw = {}) {
  const evidence = pick(raw,'evidence','Evidence') ?? {};
  const behavior = pick(raw,'behaviorImpact','BehaviorImpact') ?? {};
  const rollback = pick(raw,'rollbackPlan','RollbackPlan') ?? {};
  return Object.freeze({
    proposalId:String(pick(raw,'proposalId','ProposalId') ?? ''),
    proposalVersion:String(pick(raw,'proposalVersion','ProposalVersion') ?? ''),
    changeIdentity:String(pick(raw,'changeIdentity','ChangeIdentity') ?? ''),
    materialFingerprintSha256:String(pick(raw,'materialFingerprintSha256','MaterialFingerprintSha256') ?? ''),
    owningApplicationIdentity:String(pick(raw,'owningApplicationIdentity','OwningApplicationIdentity') ?? ''),
    producerAiIdentity:pick(raw,'producerAiIdentity','ProducerAiIdentity') ?? null,
    updateClass:normalizeEnum(pick(raw,'updateClass','UpdateClass'),CLASS_BY_NUMBER),
    updateClassVersion:String(pick(raw,'updateClassVersion','UpdateClassVersion') ?? ''),
    classificationAuthoritySource:normalizeEnum(pick(raw,'classificationAuthoritySource','ClassificationAuthoritySource'),CLASSIFIER_BY_NUMBER),
    impact:normalizeEnum(pick(raw,'impact','Impact'),IMPACT_BY_NUMBER),
    environment:String(normalizeEnum(pick(raw,'environment','Environment')) ?? ''),
    requestedLifecyclePhase:String(normalizeEnum(pick(raw,'requestedLifecyclePhase','RequestedLifecyclePhase')) ?? ''),
    affectedScopes:Object.freeze(list(pick(raw,'affectedScopes','AffectedScopes')).map(String)),
    behaviorImpact:Object.freeze({
      businessBehaviorChanges:pick(behavior,'businessBehaviorChanges','BusinessBehaviorChanges') === true,
      riskBehaviorChanges:pick(behavior,'riskBehaviorChanges','RiskBehaviorChanges') === true,
      executionBehaviorChanges:pick(behavior,'executionBehaviorChanges','ExecutionBehaviorChanges') === true,
      securityBehaviorChanges:pick(behavior,'securityBehaviorChanges','SecurityBehaviorChanges') === true,
      authorityBehaviorChanges:pick(behavior,'authorityBehaviorChanges','AuthorityBehaviorChanges') === true,
      deploymentBehaviorChanges:pick(behavior,'deploymentBehaviorChanges','DeploymentBehaviorChanges') === true
    }),
    evidence:Object.freeze({
      classificationEvidenceReference:String(pick(evidence,'classificationEvidenceReference','ClassificationEvidenceReference') ?? ''),
      testEvidenceReference:String(pick(evidence,'testEvidenceReference','TestEvidenceReference') ?? ''),
      sandboxEvidenceReference:String(pick(evidence,'sandboxEvidenceReference','SandboxEvidenceReference') ?? ''),
      fsaReviewRequired:pick(evidence,'fsaReviewRequired','FsaReviewRequired') === true,
      fsaReviewSatisfied:pick(evidence,'fsaReviewSatisfied','FsaReviewSatisfied') === true,
      fsaEvidenceReference:pick(evidence,'fsaEvidenceReference','FsaEvidenceReference') ?? null
    }),
    previousStateIdentity:String(pick(raw,'previousStateIdentity','PreviousStateIdentity') ?? ''),
    lineageReference:String(pick(raw,'lineageReference','LineageReference') ?? ''),
    materiallyChangesPriorProposal:pick(raw,'materiallyChangesPriorProposal','MateriallyChangesPriorProposal') === true,
    supersedesProposalId:pick(raw,'supersedesProposalId','SupersedesProposalId') ?? null,
    rollbackPlan:Object.freeze({
      planId:String(pick(rollback,'planId','PlanId') ?? ''),
      planVersion:String(pick(rollback,'planVersion','PlanVersion') ?? ''),
      proposalId:String(pick(rollback,'proposalId','ProposalId') ?? ''),
      changeIdentity:String(pick(rollback,'changeIdentity','ChangeIdentity') ?? ''),
      previousStateIdentity:String(pick(rollback,'previousStateIdentity','PreviousStateIdentity') ?? ''),
      targetScopes:Object.freeze(list(pick(rollback,'targetScopes','TargetScopes')).map(String)),
      fullRollbackSupported:pick(rollback,'fullRollbackSupported','FullRollbackSupported') === true,
      partialRollbackTargets:Object.freeze(list(pick(rollback,'partialRollbackTargets','PartialRollbackTargets')).map(String)),
      prerequisites:Object.freeze(list(pick(rollback,'prerequisites','Prerequisites')).map(String)),
      knownNonReversibleEffects:Object.freeze(list(pick(rollback,'knownNonReversibleEffects','KnownNonReversibleEffects')).map(String)),
      dataOrSchemaMigrationImplications:Object.freeze(list(pick(rollback,'dataOrSchemaMigrationImplications','DataOrSchemaMigrationImplications')).map(String)),
      compatibilityConstraints:Object.freeze(list(pick(rollback,'compatibilityConstraints','CompatibilityConstraints')).map(String)),
      current:pick(rollback,'current','Current') === true,
      compatible:pick(rollback,'compatible','Compatible') === true,
      validated:pick(rollback,'validated','Validated') === true,
      validationEvidenceReference:String(pick(rollback,'validationEvidenceReference','ValidationEvidenceReference') ?? ''),
      expectedRollbackResult:String(pick(rollback,'expectedRollbackResult','ExpectedRollbackResult') ?? ''),
      recoveryObservationSteps:Object.freeze(list(pick(rollback,'recoveryObservationSteps','RecoveryObservationSteps')).map(String)),
      evidenceReference:String(pick(rollback,'evidenceReference','EvidenceReference') ?? '')
    })
  });
}

export function isValidRollbackPlan(plan, proposal) {
  if (!plan || !proposal) return false;
  if (![plan.planId,plan.planVersion,plan.proposalId,plan.changeIdentity,plan.previousStateIdentity,plan.validationEvidenceReference,plan.expectedRollbackResult,plan.evidenceReference].every(token)) return false;
  if (!distinctNonBlank(plan.targetScopes) || plan.targetScopes.length === 0) return false;
  for (const values of [plan.partialRollbackTargets,plan.prerequisites,plan.knownNonReversibleEffects,plan.dataOrSchemaMigrationImplications,plan.compatibilityConstraints]) {
    if (!distinctNonBlank(values)) return false;
  }
  if (!distinctNonBlank(plan.recoveryObservationSteps) || plan.recoveryObservationSteps.length === 0) return false;
  if (plan.current !== true || plan.compatible !== true || plan.validated !== true) return false;
  if (plan.proposalId !== proposal.proposalId || plan.changeIdentity !== proposal.changeIdentity || plan.previousStateIdentity !== proposal.previousStateIdentity) return false;
  return proposal.affectedScopes.every(scope => plan.targetScopes.includes(scope));
}

export function validateOwnerUpdateProposal(raw) {
  const proposal = normalizeOwnerUpdateProposal(raw);
  const required = [proposal.proposalId,proposal.proposalVersion,proposal.changeIdentity,proposal.owningApplicationIdentity,proposal.updateClassVersion,proposal.environment,proposal.requestedLifecyclePhase,proposal.previousStateIdentity,proposal.lineageReference];
  if (!required.every(token) || !sha256Hex(proposal.materialFingerprintSha256)) return Object.freeze({valid:false,reason:'INVALID_PROPOSAL_OR_ROLLBACK',proposal});
  if (!ALL_CLASSES.has(proposal.updateClass) || proposal.updateClass === OwnerUpdateClass.UNKNOWN) return Object.freeze({valid:false,reason:'INVALID_PROPOSAL_OR_ROLLBACK',proposal});
  if (proposal.classificationAuthoritySource !== ClassificationAuthoritySource.GOVERNED_APPLICATION_CLASSIFIER) return Object.freeze({valid:false,reason:'INVALID_PROPOSAL_OR_ROLLBACK',proposal});
  if (!IMPACTS.has(proposal.impact) || proposal.impact === OwnerUpdateImpact.UNKNOWN) return Object.freeze({valid:false,reason:'INVALID_PROPOSAL_OR_ROLLBACK',proposal});
  if (!distinctNonBlank(proposal.affectedScopes) || proposal.affectedScopes.length === 0) return Object.freeze({valid:false,reason:'INVALID_PROPOSAL_OR_ROLLBACK',proposal});
  const e=proposal.evidence;
  if (![e.classificationEvidenceReference,e.testEvidenceReference,e.sandboxEvidenceReference].every(token)) return Object.freeze({valid:false,reason:'INVALID_PROPOSAL_OR_ROLLBACK',proposal});
  const aiSelfDevelopment=proposal.updateClass===OwnerUpdateClass.AI_SELF_DEVELOPMENT || (token(proposal.producerAiIdentity) && e.fsaReviewRequired);
  if (aiSelfDevelopment && (!e.fsaReviewRequired || !e.fsaReviewSatisfied || !token(e.fsaEvidenceReference))) return Object.freeze({valid:false,reason:'INVALID_PROPOSAL_OR_ROLLBACK',proposal});
  if (proposal.materiallyChangesPriorProposal && !token(proposal.supersedesProposalId)) return Object.freeze({valid:false,reason:'INVALID_PROPOSAL_OR_ROLLBACK',proposal});
  if (!isValidRollbackPlan(proposal.rollbackPlan,proposal)) return Object.freeze({valid:false,reason:'INVALID_PROPOSAL_OR_ROLLBACK',proposal});
  return Object.freeze({valid:true,reason:'VALID_GOVERNED_APPLICATION_PROPOSAL',proposal});
}

export function normalizeStandingPolicy(raw = {}) {
  return Object.freeze({
    policyId:String(pick(raw,'policyId','PolicyId') ?? ''),
    policyVersion:String(pick(raw,'policyVersion','PolicyVersion') ?? ''),
    authoritySource:normalizeEnum(pick(raw,'authoritySource','AuthoritySource'),AUTHORITY_BY_NUMBER),
    authorityEvidenceReference:String(pick(raw,'authorityEvidenceReference','AuthorityEvidenceReference') ?? ''),
    rules:Object.freeze(list(pick(raw,'rules','Rules')).map(rule=>Object.freeze({
      updateClass:normalizeEnum(pick(rule,'updateClass','UpdateClass'),CLASS_BY_NUMBER),
      updateClassVersion:String(pick(rule,'updateClassVersion','UpdateClassVersion') ?? ''),
      allowNonReversibleChange:pick(rule,'allowNonReversibleChange','AllowNonReversibleChange') === true
    })))
  });
}

export function isValidStandingPolicy(raw) {
  const policy=normalizeStandingPolicy(raw);
  if (![policy.policyId,policy.policyVersion,policy.authorityEvidenceReference].every(token)) return false;
  if (policy.authoritySource !== OwnerUpdateAuthoritySource.OWNER_VIA_SHARED_WEB || policy.rules.length===0) return false;
  const keys=[];
  for (const rule of policy.rules) {
    if (!ALL_CLASSES.has(rule.updateClass) || rule.updateClass===OwnerUpdateClass.UNKNOWN || !token(rule.updateClassVersion)) return false;
    keys.push(`${rule.updateClass}:${rule.updateClassVersion}`);
  }
  return new Set(keys).size===keys.length;
}

function hasHighImpactBehaviorChange(impact) {
  return Object.values(impact).some(value=>value===true);
}

/**
 * Mirrors the Application-owned FCR-0238 minimum review floor.
 * This function NEVER returns Owner approval. It only decides whether the exact
 * proposal is semantically eligible to be sent to the Foundation-owned Owner
 * standing-policy evaluator.
 */
export function evaluateStandingPreapprovalEligibility(rawProposal,rawPolicy) {
  const checked=validateOwnerUpdateProposal(rawProposal);
  if (!checked.valid) return Object.freeze({ disposition:'MANUAL_OWNER_REVIEW_REQUIRED', reason:checked.reason, proposal:checked.proposal });
  const proposal=checked.proposal;
  if (!ELIGIBLE_CLASSES.has(proposal.updateClass)) return Object.freeze({disposition:'MANUAL_OWNER_REVIEW_REQUIRED',reason:'MANUAL_OWNER_REVIEW_REQUIRED',proposal});
  if (proposal.impact!==OwnerUpdateImpact.LOW || hasHighImpactBehaviorChange(proposal.behaviorImpact)) return Object.freeze({disposition:'MANUAL_OWNER_REVIEW_REQUIRED',reason:'MATERIAL_OR_HIGH_IMPACT_CHANGE_REQUIRES_MANUAL_OWNER_REVIEW',proposal});
  if (!isValidStandingPolicy(rawPolicy)) return Object.freeze({disposition:'MANUAL_OWNER_REVIEW_REQUIRED',reason:'MANUAL_OWNER_REVIEW_REQUIRED',proposal});
  const policy=normalizeStandingPolicy(rawPolicy);
  const rule=policy.rules.find(item=>item.updateClass===proposal.updateClass && item.updateClassVersion===proposal.updateClassVersion);
  if (!rule) return Object.freeze({disposition:'MANUAL_OWNER_REVIEW_REQUIRED',reason:'MANUAL_OWNER_REVIEW_REQUIRED',proposal,policy});
  const nonReversible=proposal.rollbackPlan.knownNonReversibleEffects.length>0;
  if ((!proposal.rollbackPlan.fullRollbackSupported && !nonReversible) || (!proposal.rollbackPlan.fullRollbackSupported && !rule.allowNonReversibleChange) || (nonReversible && !rule.allowNonReversibleChange)) {
    return Object.freeze({disposition:'MANUAL_OWNER_REVIEW_REQUIRED',reason:'MANUAL_OWNER_REVIEW_REQUIRED',proposal,policy});
  }
  return Object.freeze({
    disposition:'STANDING_PREAPPROVAL_ELIGIBLE_FOR_OWNER_DECISION',
    reason:'OWNER_STANDING_POLICY_MATCH',
    proposal,
    policy,
    proposalAcceptanceGranted:false,
    executionAuthorityGranted:false,
    deploymentAuthorityGranted:false,
    runtimeActivationAuthorityGranted:false
  });
}

export function normalizeOwnerDerivedDisposition(raw={}) {
  return Object.freeze({
    decisionId:String(pick(raw,'decisionId','DecisionId') ?? ''),
    state:String(normalizeEnum(pick(raw,'state','State')) ?? ''),
    proposalId:String(pick(raw,'proposalId','ProposalId') ?? ''),
    proposalVersion:String(pick(raw,'proposalVersion','ProposalVersion') ?? ''),
    changeIdentity:String(pick(raw,'changeIdentity','ChangeIdentity') ?? ''),
    materialFingerprintSha256:String(pick(raw,'materialFingerprintSha256','MaterialFingerprintSha256') ?? ''),
    policyId:String(pick(raw,'policyId','PolicyId') ?? ''),
    policyVersion:String(pick(raw,'policyVersion','PolicyVersion') ?? ''),
    authoritySource:normalizeEnum(pick(raw,'authoritySource','AuthoritySource'),AUTHORITY_BY_NUMBER),
    authorityEvidenceReference:String(pick(raw,'authorityEvidenceReference','AuthorityEvidenceReference') ?? '')
  });
}

export function isCurrentOwnerDisposition(rawDisposition,rawProposal,rawPolicy) {
  const checked=validateOwnerUpdateProposal(rawProposal);
  if (!checked.valid || !isValidStandingPolicy(rawPolicy)) return false;
  const d=normalizeOwnerDerivedDisposition(rawDisposition);
  const p=checked.proposal;
  const policy=normalizeStandingPolicy(rawPolicy);
  return token(d.decisionId)
    && d.authoritySource===OwnerUpdateAuthoritySource.OWNER_VIA_SHARED_WEB
    && token(d.authorityEvidenceReference)
    && d.proposalId===p.proposalId && d.proposalVersion===p.proposalVersion
    && d.changeIdentity===p.changeIdentity && d.materialFingerprintSha256===p.materialFingerprintSha256
    && d.policyId===policy.policyId && d.policyVersion===policy.policyVersion;
}

export function normalizeRollbackRequest(raw={}) {
  return Object.freeze({
    requestId:String(pick(raw,'requestId','RequestId') ?? ''),
    proposalId:String(pick(raw,'proposalId','ProposalId') ?? ''),
    proposalVersion:String(pick(raw,'proposalVersion','ProposalVersion') ?? ''),
    changeIdentity:String(pick(raw,'changeIdentity','ChangeIdentity') ?? ''),
    planId:String(pick(raw,'planId','PlanId') ?? ''),
    planVersion:String(pick(raw,'planVersion','PlanVersion') ?? ''),
    authoritySource:normalizeEnum(pick(raw,'authoritySource','AuthoritySource'),AUTHORITY_BY_NUMBER),
    authorityEvidenceReference:String(pick(raw,'authorityEvidenceReference','AuthorityEvidenceReference') ?? ''),
    mode:normalizeEnum(pick(raw,'mode','Mode'),ROLLBACK_MODE_BY_NUMBER),
    boundedTargets:Object.freeze(list(pick(raw,'boundedTargets','BoundedTargets')).map(String))
  });
}

export function isValidRollbackRequest(rawRequest,rawProposal) {
  const checked=validateOwnerUpdateProposal(rawProposal);
  if (!checked.valid) return false;
  const r=normalizeRollbackRequest(rawRequest), p=checked.proposal;
  if (!token(r.requestId) || r.authoritySource!==OwnerUpdateAuthoritySource.OWNER_VIA_SHARED_WEB || !token(r.authorityEvidenceReference)) return false;
  if (r.proposalId!==p.proposalId || r.proposalVersion!==p.proposalVersion || r.changeIdentity!==p.changeIdentity || r.planId!==p.rollbackPlan.planId || r.planVersion!==p.rollbackPlan.planVersion) return false;
  if (r.mode===OwnerRollbackMode.FULL) return p.rollbackPlan.fullRollbackSupported && r.boundedTargets.length===0;
  return r.mode===OwnerRollbackMode.BOUNDED_PARTIAL && r.boundedTargets.length>0 && distinctNonBlank(r.boundedTargets) && r.boundedTargets.every(target=>p.rollbackPlan.partialRollbackTargets.includes(target));
}

export function isValidRollbackTransition(from,to) {
  return new Set([
    'Received>Accepted','Received>Rejected','Accepted>ExecutionStarted','ExecutionStarted>ExecutionCompleted','ExecutionStarted>ExecutionFailed',
    'ExecutionCompleted>PostRollbackValidationRequired','PostRollbackValidationRequired>PostRollbackValidationCompleted','PostRollbackValidationRequired>PostRollbackValidationFailed'
  ]).has(`${normalizeEnum(from,ROLLBACK_LIFECYCLE_BY_NUMBER)}>${normalizeEnum(to,ROLLBACK_LIFECYCLE_BY_NUMBER)}`);
}

export const __test = Object.freeze({ ELIGIBLE_CLASSES, distinctNonBlank, hasHighImpactBehaviorChange });
