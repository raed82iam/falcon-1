import { createOwnerCommandCenterFeature } from '../features/owner-command-center/owner-command-center.js';
import { createOwnerProviderActionsFeature } from '../features/owner-provider-actions/owner-provider-actions.js';
import { createOwnerAiEmergencyFeature } from '../features/owner-ai-emergency/owner-ai-emergency.js';
import { createOwnerUpdateGovernanceFeature } from '../features/owner-approvals/owner-update-governance.js';
import { createOwnerHomeFeature } from '../features/owner-home/owner-home.js';

function requireFunction(value,name) {
  if (typeof value !== 'function') throw new TypeError(`${name} must be a function`);
  return value;
}

const unavailableEmergencyModel = Object.freeze({
  targets:Object.freeze([]),
  selectedTarget:null,
  selectedAction:'KILL',
  blastRadius:null,
  decision:null,
  transportAvailable:false
});

/**
 * Web-owned composition boundary for Project Owner presentation surfaces.
 *
 * These views remain presentation/request surfaces. This composition creates
 * no Foundation, Trading, Kill, provider, deployment, rollback-execution or
 * business authority.
 */
export function createOwnerSurfaces({
  t,
  language,
  workspace,
  data,
  session,
  ownerGovernanceModel = null,
  ownerFsatsAccess = null,
  ownerAiEmergencyModel = null
} = {}) {
  const translate = requireFunction(t,'t');
  const currentLanguage = requireFunction(language,'language');
  const renderWorkspace = requireFunction(workspace,'workspace');
  const currentSession = requireFunction(session,'session');
  if (!data || typeof data !== 'object') throw new TypeError('data is required');

  const commandCenter = createOwnerCommandCenterFeature({
    t:translate,
    language:currentLanguage,
    workspace:renderWorkspace,
    data,
    supportAuthorization:currentSession
  });

  const { ownerHome } = createOwnerHomeFeature({
    language:currentLanguage,
    session:currentSession,
    ownerFsatsAccess
  });

  const { ownerProviderActionsPage } = createOwnerProviderActionsFeature({
    t:translate,
    language:currentLanguage,
    workspace:renderWorkspace,
    actions:data.ownerProviderActions
  });

  const { ownerApprovalsPage } = createOwnerUpdateGovernanceFeature({
    language:currentLanguage,
    workspace:renderWorkspace,
    model:ownerGovernanceModel ?? {
      transportAvailable:false,
      policies:[],
      proposals:[],
      history:[]
    }
  });

  function ownerAiEmergencyPage() {
    return createOwnerAiEmergencyFeature({
      t:translate,
      language:currentLanguage,
      workspace:renderWorkspace,
      session:currentSession(),
      model:ownerAiEmergencyModel ?? unavailableEmergencyModel
    }).page();
  }

  const { ownerApprovals: _legacyOwnerApprovals, ...commandCenterWithoutLegacyApprovals } = commandCenter;
  void _legacyOwnerApprovals;

  return Object.freeze({
    ownerHome,
    ...commandCenterWithoutLegacyApprovals,
    ownerApprovals:ownerApprovalsPage,
    ownerProviderActionsPage,
    ownerAiEmergencyPage
  });
}
