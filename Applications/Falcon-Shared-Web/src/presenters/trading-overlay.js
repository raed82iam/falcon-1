import { ApplicabilityState, TruthState } from '../contracts.js';

const allowedElements=new Set(['POINT','PRICE_LEVEL','HORIZONTAL_LINE','VERTICAL_LINE','ZONE','SERIES','MARKER','ANNOTATION']);

/**
 * Converts an Application-owned overlay projection into Web rendering primitives.
 * Web never reconstructs Strategy/School logic from these elements.
 */
export function presentTradingOverlay(projection) {
  if (!projection || typeof projection !== 'object') {
    return Object.freeze({ renderable:false, applicability:ApplicabilityState.UNAVAILABLE, truth:TruthState.UNKNOWN, elements:[] });
  }

  const applicability=projection.applicabilityState ?? ApplicabilityState.UNKNOWN;
  const truth=projection.truthState ?? TruthState.UNKNOWN;
  if (applicability !== ApplicabilityState.APPLICABLE && applicability !== ApplicabilityState.PARTIAL) {
    return Object.freeze({
      renderable:false,
      projectionId:projection.overlayProjectionId ?? null,
      applicability,
      truth,
      reasonCode:projection.reasonCode ?? null,
      elements:[]
    });
  }

  const source=Array.isArray(projection.elements) ? projection.elements : [];
  const elements=source.map((element,index)=>{
    if (!allowedElements.has(element?.type)) throw new TypeError(`Unsupported overlay element type at index ${index}: ${element?.type}`);
    if (typeof element.id !== 'string' || element.id.length===0) throw new TypeError(`Overlay element ${index} requires stable id`);
    return Object.freeze({
      id:element.id,
      type:element.type,
      state:element.state ?? null,
      time:element.time ?? null,
      timeStart:element.timeStart ?? null,
      timeEnd:element.timeEnd ?? null,
      price:element.price ?? null,
      priceStart:element.priceStart ?? null,
      priceEnd:element.priceEnd ?? null,
      value:element.value ?? null,
      points:Array.isArray(element.points) ? Object.freeze([...element.points]) : null,
      label:element.label ?? null,
      tooltip:element.tooltip ?? null
    });
  });

  return Object.freeze({
    renderable:true,
    projectionId:projection.overlayProjectionId ?? null,
    subjectKind:projection.overlaySubjectKind ?? null,
    subjectId:projection.overlaySubjectId ?? null,
    instrument:projection.resolvedInstrumentIdentity ?? null,
    timeframe:projection.timeframe ?? null,
    range:projection.range ?? null,
    applicability,
    truth,
    asOfTime:projection.asOfTime ?? null,
    projectionVersion:projection.projectionVersion ?? null,
    reasonCode:projection.reasonCode ?? null,
    elements:Object.freeze(elements)
  });
}

export function applyOverlayUpdate(current, update) {
  if (!current?.projectionId || update?.overlayProjectionId !== current.projectionId) {
    return Object.freeze({ accepted:false, reason:'OVERLAY_PROJECTION_ID_MISMATCH', current });
  }
  const allowedUpdates=new Set(['ADD','UPDATE','CORRECT','INVALIDATE','REMOVE','STATUS']);
  if (!allowedUpdates.has(update.updateType)) return Object.freeze({ accepted:false, reason:'UNSUPPORTED_UPDATE_TYPE', current });
  return Object.freeze({ accepted:true, reason:null, update:Object.freeze({ ...update }) });
}
