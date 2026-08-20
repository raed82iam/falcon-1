import { safeText } from '../../security/safe-html.js';
import { decideAnalysisPresentation } from './analysis-presentation-policy.js';

function requireFunction(value, name) {
  if (typeof value !== 'function') throw new TypeError(`${name} must be a function`);
  return value;
}

const text = (value, fallback) => value === null || value === undefined || value === '' ? fallback : value;
const list = value => Array.isArray(value) ? value : [];
const boundedExplanation = (value, fallback) => {
  if (typeof value === 'string' && value.trim()) return value;
  if (!value || typeof value !== 'object') return fallback;
  for (const key of ['explanation','summary','reason','description','label','id']) {
    if (typeof value[key] === 'string' && value[key].trim()) return value[key];
  }
  return fallback;
};

/** Customer AI analysis presentation for the authenticated FSATS workspace. */
export function createAiFeature({ t, language, workspace, data = {} } = {}) {
  const translate = requireFunction(t, 't');
  const currentLanguage = requireFunction(language, 'language');
  const renderWorkspace = requireFunction(workspace, 'workspace');
  const local = (ar,en) => currentLanguage() === 'ar' ? ar : en;

  function aiPage() {
    const onDemand = data.onDemandAnalysis ?? null;
    const detailed = data.detailedAnalysis
      ?? onDemand?.analysisProjection?.detailedAssetAnalysis
      ?? onDemand?.projection?.detailedAssetAnalysis
      ?? null;
    const presentation=decideAnalysisPresentation({onDemand,detailed});
    const projection = detailed?.detailedProjection ?? detailed ?? {};
    const horizons = list(projection.horizonViews);
    const strategies = list(projection.strategyViews);
    const schools = list(projection.schoolViews);
    const synthesis = projection.synthesis ?? null;
    const detailsAllowed=presentation.showDetails===true;
    const summaryAllowed=presentation.showSummary===true;
    const requestState=onDemand?.resultState ?? onDemand?.state ?? null;
    const summary = detailsAllowed
      ? (projection.summary ?? detailed?.summary ?? onDemand?.summary ?? null)
      : requestState === 'PARTIAL'
        ? (onDemand?.summary ?? null)
        : null;

    const horizonMarkup = horizons.length
      ? horizons.map(h=>`<div class="analysis-block"><div class="analysis-block-head"><b>${safeText(text(h.horizonId ?? h.label ?? h.horizon,translate('noAnalysisValue')))}</b>${h.resultState?`<span class="status-chip">${safeText(h.resultState)}</span>`:''}</div><p>${safeText(text(h.conclusion ?? h.summary ?? h.view,translate('noAnalysisValue')))}</p>${h.confidenceOrStrength==null?'':`<small>${safeText(local('الثقة/القوة','Confidence/strength'))}: ${safeText(h.confidenceOrStrength)}</small>`}${list(h.limitations).length?`<ul>${list(h.limitations).map(x=>`<li>${safeText(x)}</li>`).join('')}</ul>`:''}</div>`).join('')
      : `<div><b>${safeText(translate('analysisShort'))} / ${safeText(translate('analysisMedium'))} / ${safeText(translate('analysisLong'))}</b><p>${safeText(translate('noAnalysisValue'))}</p></div>`;

    const strategyMarkup = strategies.length
      ? strategies.map(s=>`<li><div><b>${safeText(text(s.strategyId ?? s.name ?? s.strategyName,'—'))}</b>${s.applicabilityState?` <span class="status-chip">${safeText(s.applicabilityState)}</span>`:''}</div><span>${safeText(text(s.conclusion ?? s.summary ?? s.result,translate('noAnalysisValue')))}</span>${s.freshnessState?`<small>${safeText(s.truthState ?? 'UNKNOWN')} · ${safeText(s.freshnessState)}</small>`:''}</li>`).join('')
      : `<li>${safeText(translate('noAnalysisValue'))}</li>`;
    const schoolMarkup = schools.length
      ? schools.map(s=>`<li><div><b>${safeText(text(s.schoolId ?? s.name ?? s.schoolName,'—'))}</b>${s.applicabilityState?` <span class="status-chip">${safeText(s.applicabilityState)}</span>`:''}</div><span>${safeText(text(s.perspectiveOrConclusion ?? s.summary ?? s.result,translate('noAnalysisValue')))}</span>${s.freshnessState?`<small>${safeText(s.truthState ?? 'UNKNOWN')} · ${safeText(s.freshnessState)}</small>`:''}</li>`).join('')
      : `<li>${safeText(translate('noAnalysisValue'))}</li>`;

    const synthesisText = typeof synthesis === 'string'
      ? synthesis
      : synthesis?.boundedCombinedExplanation ?? synthesis?.summary ?? translate('noAnalysisValue');
    const disagreements = list(synthesis?.disagreements);
    const conflicts = list(synthesis?.unresolvedConflicts);
    const synthesisState = synthesis?.synthesisState ?? null;
    const requestLimitations = list(onDemand?.limitations);
    const structuredFallback = local('تم توفير اختلاف منظم؛ التفاصيل غير التفسيرية محجوبة عن واجهة العميل.','A structured disagreement was supplied; non-explanatory metadata is hidden from the customer UI.');
    const policyNotice=presentation.reason
      ? `<div class="attention"><b>${safeText(local('حدود عرض التحليل','Analysis presentation limit'))}</b><p>${safeText(presentation.reason)}</p></div>`
      : '';
    const detailedMarkup=detailsAllowed
      ? `<div class="analysis-placeholder">${horizonMarkup}<div><b>${safeText(translate('strategies'))}</b><ul>${strategyMarkup}</ul></div><div><b>${safeText(translate('schools'))}</b><ul>${schoolMarkup}</ul></div><div><b>${safeText(translate('synthesis'))}${synthesisState?` · ${safeText(synthesisState)}`:''}</b><p>${safeText(synthesisText)}</p>${disagreements.length?`<div class="attention"><b>${safeText(local('اختلافات جوهرية','Material disagreements'))}</b><ul>${disagreements.map(x=>`<li>${safeText(boundedExplanation(x,structuredFallback))}</li>`).join('')}</ul></div>`:''}${conflicts.length?`<div class="attention"><b>${safeText(local('تعارضات غير محلولة','Unresolved conflicts'))}</b><ul>${conflicts.map(x=>`<li>${safeText(boundedExplanation(x,structuredFallback))}</li>`).join('')}</ul></div>`:''}</div></div>`
      : policyNotice;
    const summaryText=summaryAllowed && summary
      ? summary
      : local('لا يمكن عرض تفاصيل التحليل الحالية بهذه الحالة. سأبقي البيانات الناقصة أو القديمة أو غير المكتملة واضحة.','Current analysis details cannot be shown in this state. Missing, stale, or incomplete truth remains explicit.');

    return renderWorkspace(`<div class="chat-layout"><section class="chat-main"><div class="chat-title"><h1>${safeText(translate('falconAI'))}</h1><span class="status-chip">FSATS</span></div>${requestState?`<div class="analysis-request-state"><b>${safeText(local('حالة طلب التحليل','Analysis request state'))}</b><span class="status-chip">${safeText(requestState)}</span>${onDemand?.asOfTime?`<small>${safeText(onDemand.asOfTime)}</small>`:''}${requestLimitations.length?`<ul>${requestLimitations.map(x=>`<li>${safeText(x)}</li>`).join('')}</ul>`:''}</div>`:''}<div class="message assistant"><b>Falcon</b><p>${safeText(summaryText)}</p></div>${detailedMarkup}${detailsAllowed?`<details class="analysis-full"><summary>${safeText(translate('showFullAnalysis'))}</summary><p>${safeText(translate('analysisTruthNotice'))}</p>${projection.asOfTime ? `<small>${safeText(projection.asOfTime)}</small>` : ''}</details>`:''}<form class="chat-input" data-analysis-form><input data-analysis-instrument placeholder="${safeText(translate('askFalcon'))}…"><button class="primary" type="button" data-analysis-request disabled aria-disabled="true">➤</button></form><p class="muted tiny" data-analysis-request-status>${safeText(local('إرسال الطلب التشغيلي يبقى مقفولًا حتى يتوفر runtime route مخوّل.','Operational request transport remains fail-closed until an authorized runtime route exists.'))}</p></section></div>`,'ai');
  }

  return Object.freeze({ aiPage });
}
