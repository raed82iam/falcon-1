import test from 'node:test';
import assert from 'node:assert/strict';
import { escapeHtml, safeText } from '../src/security/safe-html.js';
import { normalizeRoute } from '../src/platform/navigation/routes.js';
import { createNotificationsFeature } from '../src/features/notifications/notifications.js';
import { createPortfolioFeature } from '../src/features/portfolio/portfolio.js';
import { createActivityFeature } from '../src/features/activity/activity.js';
import { createAiFeature } from '../src/features/ai/ai.js';
import { createOwnerCommandCenterFeature } from '../src/features/owner-command-center/owner-command-center.js';

const payloads=[
  '<img src=x onerror=alert(1)><script>alert(2)</script>',
  '<svg><animate onbegin=alert(1) attributeName=x dur=1s></animate></svg>',
  '\"><iframe srcdoc="<script>alert(1)</script>">',
  '&lt;script&gt;alert(1)&lt;/script&gt;',
  'مرحبا <b onclick=alert(1)>RTL</b>'
];
const t=key=>key;
const workspace=(body)=>body;

function assertEncoded(html) {
  assert.doesNotMatch(html,/<(?:script|img|svg|iframe)\b/i);
  assert.doesNotMatch(html,/<[^>]+\s(?:onerror|onclick|onbegin)\s*=/i);
}

test('central HTML escaping encodes markup-sensitive characters',()=>{
  assert.equal(escapeHtml("<>&\"'"),'&lt;&gt;&amp;&quot;&#39;');
  assert.equal(safeText(null),'—');
});

test('unknown or hostile hash routes normalize to the public home route',()=>{
  assert.equal(normalizeRoute('#/<script>alert(1)</script>'),'home');
  assert.equal(normalizeRoute('#/owner?x=<img>'),'home');
});

for (const payload of payloads) {
  test(`incident/customer/support text is encoded: ${payload.slice(0,20)}`,()=>{
    const html=createNotificationsFeature({t,workspace,data:{incidentConversation:{priority:'HIGH',status:'OPEN',message:payload,messages:[{sender:'CUSTOMER',text:payload}],timeline:[{at:payload,label:payload,source:payload}],outstandingAction:payload,awaitingCustomerReply:true}}}).notificationsPage();
    assertEncoded(html);
  });
}

test('portfolio and activity projection strings cannot become markup',()=>{
  const payload=payloads[0];
  assertEncoded(createPortfolioFeature({t,workspace,data:{portfolio:{value:payload,today:payload,available:payload},positions:[[payload,payload,payload,payload]]}}).portfolioPage());
  assertEncoded(createActivityFeature({t,workspace,data:{trades:[[payload,'BUY',payload,payload,payload,'REQUESTED']]}}).activityPage());
});

test('AI analysis strings cannot become markup',()=>{
  const payload=payloads[1];
  const html=createAiFeature({t,language:()=> 'en',workspace,data:{detailedAnalysis:{summary:payload,detailedProjection:{horizonViews:[{label:payload,summary:payload}],strategyViews:[{name:payload,summary:payload}],schoolViews:[{name:payload,summary:payload}],synthesis:{summary:payload},asOfTime:payload}}}}).aiPage();
  assertEncoded(html);
});

test('Owner projections cannot become markup and takeover stays disabled without authoritative support capability',()=>{
  const payload=payloads[2];
  const feature=createOwnerCommandCenterFeature({t,language:()=> 'en',workspace,data:{owner:{health:payload,apps:'1',users:'1',incidents:'1',approvals:'0'},services:[[payload,'CURRENT']],incidents:[['HIGH',payload]]},supportAuthorization:()=>null});
  const html=feature.ownerIncidents()+feature.owner();
  assertEncoded(html);
  assert.match(feature.ownerIncidents(),/data-support-takeover="0" disabled/);
});
