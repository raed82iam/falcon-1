import test from 'node:test';
import assert from 'node:assert/strict';
import { createFsatsWorkspaceFeature } from '../src/features/fsats-workspace/fsats-workspace.js';

const labels={dashboard:'Dashboard',chart:'Chart',hide:'Hide',schools:'Schools',strategies:'Strategies',totalValue:'Portfolio Value',todayPL:'Today P/L',totalPL:'Total P/L',positions:'Positions',recentTrades:'Recent Trades',quickSummary:'Summary',askFalcon:'Ask Falcon',notifications:'Notifications',manageWidgets:'Manage',resetLayout:'Reset',restore:'Restore','orderSide_—':'—',orderState_UNKNOWN_BROKER_OUTCOME:'Unknown'};
const t=key=>labels[key]??key;
const workspace=body=>body;
const store={layout:()=>({order:['portfolio','market','positions','trades'],hidden:[]})};

test('workspace renders Application overlay separately from chart display data',()=>{
  const data={
    portfolio:{totalEquity:null},positions:[],trades:[],alerts:[],
    tradingOverlay:{renderable:true,applicability:'APPLICABLE',truth:'CURRENT',asOfTime:'2026-08-16T12:00:00Z',elements:[{id:'E1',type:'PRICE_LEVEL',label:'Target',price:125.5}]}
  };
  const html=createFsatsWorkspaceFeature({t,language:()=> 'en',workspace,store,data,catalogMarkup:()=>'',previewMode:false}).dashboardPage();
  assert.match(html,/FSATS Trading overlay/);
  assert.match(html,/Target/);
  assert.match(html,/125.5/);
  assert.match(html,/Market display data and Trading overlays are separate sources/);
  assert.match(html,/Portfolio Value/);
  assert.match(html,/>—<\/strong>/);
  assert.doesNotMatch(html,/>null</);
  assert.match(html,/No positions available from source/);
  assert.match(html,/No activity or trades available from source/);
});

test('non-applicable overlay remains non-rendering with reason',()=>{
  const data={
    portfolio:{},positions:[],trades:[],alerts:[],
    tradingOverlay:{renderable:false,applicability:'NOT_APPLICABLE',truth:'CURRENT',reasonCode:'NOT_FOR_CONTEXT',elements:[]}
  };
  const html=createFsatsWorkspaceFeature({t,language:()=> 'en',workspace,store,data,catalogMarkup:()=>'',previewMode:false}).dashboardPage();
  assert.match(html,/NOT_APPLICABLE/);
  assert.match(html,/NOT_FOR_CONTEXT/);
  assert.doesNotMatch(html,/FSATS Trading overlay<\/b><span class="status-chip">APPLICABLE/);
});

test('authoritative catalog groups strategies by supplied School metadata only',()=>{
  const catalogStore={layout:()=>({order:['catalog'],hidden:[]})};
  const data={
    portfolio:{},positions:[],trades:[],alerts:[],
    catalog:[
      {id:'S1',name:'Trend',schoolId:'SC1',schoolName:'Technical',applicability:'APPLICABLE',enabled:true,reason:null},
      {id:'S2',name:'Breakout',schoolId:'SC1',schoolName:'Technical',applicability:'NOT_APPLICABLE',enabled:false,reason:'NOT_FOR_ASSET'}
    ]
  };
  const html=createFsatsWorkspaceFeature({t,language:()=> 'en',workspace,store:catalogStore,data,catalogMarkup:()=> 'LEGACY',previewMode:false}).dashboardPage();
  assert.match(html,/Technical/);
  assert.match(html,/School group from Trading catalog/);
  assert.match(html,/Trend/);
  assert.match(html,/Breakout/);
  assert.match(html,/NOT_FOR_ASSET/);
  assert.match(html,/Breakout[\s\S]*disabled aria-disabled="true"/);
  assert.match(html,/not invented School applicability/);
  assert.doesNotMatch(html,/LEGACY/);
});
