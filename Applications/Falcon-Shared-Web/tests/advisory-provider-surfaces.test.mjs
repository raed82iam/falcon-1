import test from 'node:test';
import assert from 'node:assert/strict';
import { createAdvisoryMarketsFeature } from '../src/features/advisory-markets/advisory-markets.js';
import { createOwnerProviderActionsFeature } from '../src/features/owner-provider-actions/owner-provider-actions.js';

const t = key => key;
const workspace = content => content;

test('advisory market keeps advisory-only boundaries visible', () => {
  const feature = createAdvisoryMarketsFeature({
    t,
    language:()=>'en',
    workspace,
    markets:[{
      marketId:'M1', marketCode:'TASI', displayName:'Saudi Market', operatingMode:'ADVISORY_ONLY',
      supportedOpportunityHorizons:['DAILY','WEEKLY','MONTHLY'], intradayOpportunityEnabled:false,
      executionCapability:'NONE', positionTrackingForAdvisoryOpportunities:false, opportunityFollowUpEnabled:false,
      availability:'AVAILABLE', providerDisplayName:'Example', sourceAccessType:'WEB_URL', delayMinutes:15
    }]
  });
  const html = feature.advisoryMarketsPage();
  assert.match(html,/ADVISORY ONLY/);
  assert.match(html,/AVAILABLE/);
  assert.match(html,/Execution<\/b>NONE/);
  assert.match(html,/Data delayed 15 minutes/);
  assert.match(html,/Chart source is presentation-only/);
});

test('unavailable advisory source remains explicitly unavailable with reason', () => {
  const feature=createAdvisoryMarketsFeature({
    t,language:()=> 'en',workspace,
    markets:[{
      marketId:'M2',marketCode:'TEST',operatingMode:'ADVISORY_ONLY',supportedOpportunityHorizons:['DAILY'],
      intradayOpportunityEnabled:false,executionCapability:'NONE',positionTrackingForAdvisoryOpportunities:false,
      opportunityFollowUpEnabled:false,availability:'UNAVAILABLE',reason:'NO_SUITABLE_FREE_PROVIDER_FOUND'
    }]
  });
  const html=feature.advisoryMarketsPage();
  assert.match(html,/UNAVAILABLE/);
  assert.match(html,/NO_SUITABLE_FREE_PROVIDER_FOUND/);
  assert.match(html,/does not upgrade it to current or available/);
});

test('invalid advisory state is not presented as clean advisory mode', () => {
  const feature = createAdvisoryMarketsFeature({
    t,
    language:()=>'en',
    workspace,
    markets:[{
      marketId:'M1', operatingMode:'ADVISORY_ONLY', supportedOpportunityHorizons:['DAILY'],
      intradayOpportunityEnabled:true, executionCapability:'NONE',
      positionTrackingForAdvisoryOpportunities:false, opportunityFollowUpEnabled:false
    }]
  });
  assert.match(feature.advisoryMarketsPage(),/Operational claim suppressed/);
});

test('owner provider action never renders plaintext secret input', () => {
  const feature = createOwnerProviderActionsFeature({
    t,
    language:()=>'en',
    workspace,
    actions:[{
      requestId:'R1', marketId:'M1', providerId:'P1', providerDisplayName:'Provider',
      actionType:'ADD_PROVIDER_CREDENTIAL', credentialType:'API_KEY', providerCostClass:'FREE',
      status:'ACTION_REQUIRED', secureEntryRequired:true, chatEntryProhibited:true
    }]
  });
  const html = feature.ownerProviderActionsPage();
  assert.match(html,/Secure entry only/);
  assert.match(html,/Add credential securely/);
  assert.match(html,/disabled/);
  assert.doesNotMatch(html,/type=["']password["']/);
  assert.doesNotMatch(html,/type=["']text["']/);
  assert.match(html,/does not authorize provider connectivity/);
});
