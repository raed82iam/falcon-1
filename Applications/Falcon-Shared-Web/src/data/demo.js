export const demo = {
  portfolio: { value:'$96,250.75', today:'+$2,184.35', total:'+$17,250.40', available:'$23,615.45' },
  positions:[['AAPL','20','$214.32','+$325.40'],['NVDA','15','$925.54','+$412.75'],['BTC','0.50','$67,890','-$56.20']],
  trades:[['AAPL','BUY','10','$214.32','10:24','FILLED'],['NVDA','BUY','5','$925.54','09:58','PARTIALLY_FILLED'],['BTC','SELL','0.1','$67,890','09:41','UNKNOWN_BROKER_OUTCOME']],
  alerts:['AAPL crossed your monitored level','A new FSATS analysis is available','Market data freshness changed'],
  apps:[
    {id:'fsats',name:'Falcon Self-Aware Trading System (FSATS)',kind:'trading',status:'available'}
  ],
  fsatsApps:[
    {id:'fsata',name:'Falcon Self-Aware Trading Application',shortName:'FSATA'},
    {id:'fsapma',name:'Falcon Self-Aware Provider Management Application',shortName:'FSAPMA'},
    {id:'ftga',name:'Falcon Trading Guardian Application',shortName:'FTGA'},
    {id:'fstsim',name:'Falcon Self-Aware Trading Simulation Application',shortName:'FSTSimA'},
    {id:'app-rsc',name:'Falcon Self-Aware Resource Management Application',shortName:'APP-RSC'}
  ],
  catalog:[
    {id:'school-whale',kind:'SCHOOL',name:'Whale Hunting',availability:'AVAILABLE',applicability:'APPLICABLE'},
    {id:'strategy-trend',kind:'STRATEGY',name:'Trend Strategy',availability:'AVAILABLE',applicability:'APPLICABLE'},
    {id:'strategy-breakout',kind:'STRATEGY',name:'Breakout Strategy',availability:'AVAILABLE',applicability:'NOT_APPLICABLE',reason:'Not applicable to the current preview asset/context'}
  ],
  advisoryMarkets:[
    {
      marketId:'SAUDI-EQUITIES-PREVIEW', marketCode:'TASI', displayName:'Saudi Market', operatingMode:'ADVISORY_ONLY',
      supportedOpportunityHorizons:['DAILY','WEEKLY','MONTHLY'], intradayOpportunityEnabled:false,
      executionCapability:'NONE', positionTrackingForAdvisoryOpportunities:false, opportunityFollowUpEnabled:false,
      availability:'AVAILABLE', providerDisplayName:'Preview Source', sourceAccessType:'WEB_URL', dataMode:'DELAYED',
      delayMinutes:15, disclosureText:'Preview source may be delayed by 15 minutes.', preview:true
    }
  ],
  ownerProviderActions:[
    {
      requestId:'PROVIDER-ACTION-PREVIEW-001', marketId:'SAUDI-EQUITIES-PREVIEW', providerId:'FREE-PROVIDER-PREVIEW',
      providerDisplayName:'Free Provider Preview', actionType:'ADD_PROVIDER_CREDENTIAL',
      message:'A free API credential would be required if this preview provider were selected by a governed source.',
      reason:'Preview of the Owner-only action-required presentation.', credentialType:'API_KEY', providerCostClass:'FREE',
      status:'ACTION_REQUIRED', secureEntryRequired:true, chatEntryProhibited:true,
      providerHelpOrSignupUrl:'', providerHelpOrSignupUrlValidation:'UNVALIDATED', preview:true
    }
  ],
  detailedAnalysis:{
    resultState:'PARTIAL',
    summary:'',
    detailedProjection:{
      asOfTime:null,
      horizonViews:[],
      strategyViews:[],
      schoolViews:[],
      synthesis:null
    }
  },
  owner:{health:'98%',apps:'5',users:'18',incidents:'3',approvals:'7'},
  supportIdentityVerified:false,
  services:[['Foundation','CURRENT'],['FSATS','CURRENT'],['Shared Web','CURRENT'],['Communication','CURRENT'],['Provider Management','UNKNOWN']],
  incidents:[['HIGH','Integration source unavailable'],['MEDIUM','Freshness degradation reported'],['LOW','Scheduled verification pending']],
  incidentConversation:{
    incidentId:'INC-DEMO-001',
    priority:'HIGH',
    status:'OPEN',
    mode:'FALCON_ACTIVE',
    resolved:false,
    minimized:false,
    escalatedToSupport:false,
    awaitingCustomerReply:true,
    outstandingAction:'',
    message:'Additional user confirmation is required before reconciliation can continue.',
    messages:[{sender:'FALCON',text:'Additional user confirmation is required before reconciliation can continue.'}],
    viewed:true,
    viewedAt:null,
    repliedAt:null,
    dismissedAt:null,
    voiceListening:false,
    timeline:[]
  }
};
