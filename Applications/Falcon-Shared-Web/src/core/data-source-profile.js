export const DataSourceMode = Object.freeze({
  PREVIEW:'PREVIEW',
  AUTHORITATIVE:'AUTHORITATIVE'
});

const unavailableData = Object.freeze({
  portfolio:Object.freeze({}),
  positions:Object.freeze([]),
  trades:Object.freeze([]),
  alerts:Object.freeze([]),
  apps:Object.freeze([]),
  fsatsApps:Object.freeze([]),
  catalog:Object.freeze([]),
  advisoryMarkets:Object.freeze([]),
  ownerProviderActions:Object.freeze([]),
  owner:Object.freeze({health:'UNAVAILABLE',apps:'—',users:'—',incidents:'—',approvals:'—'}),
  services:Object.freeze([]),
  incidents:Object.freeze([]),
  incidentConversation:Object.freeze({priority:'HIGH',status:'OPEN',message:'',resolved:false}),
  detailedAnalysis:null
});

export function createWebDataSource({ mode, previewData=null, authoritativeData=null } = {}) {
  if (!Object.values(DataSourceMode).includes(mode)) throw new TypeError('unsupported data source mode');
  if (mode === DataSourceMode.PREVIEW) {
    if (!previewData || typeof previewData !== 'object') throw new TypeError('previewData is required in PREVIEW mode');
    if (authoritativeData !== null) throw new TypeError('PREVIEW mode must not coexist with authoritativeData');
    return Object.freeze({ mode, data:previewData, authoritative:false, preview:true });
  }

  if (previewData !== null) throw new TypeError('AUTHORITATIVE mode must not receive previewData');
  if (!authoritativeData || typeof authoritativeData !== 'object') {
    return Object.freeze({ mode, data:unavailableData, authoritative:false, preview:false, unavailable:true });
  }
  return Object.freeze({ mode, data:authoritativeData, authoritative:true, preview:false, unavailable:false });
}
