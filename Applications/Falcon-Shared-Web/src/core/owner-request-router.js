import { WebOwnership, classifyOwnerRequestOwnership } from './web-awareness-model.js';

const SENSITIVE_TERMS = Object.freeze([
  'kill','credential','api key','secret','deployment','live trading','authority','constitution','rollback'
]);

function nonEmpty(value,name) {
  if (typeof value !== 'string' || value.trim() === '') throw new TypeError(`${name} is required`);
  return value.trim();
}

function splitCompound(text) {
  return text
    .split(/(?:\n+|;|\band then\b|\bthen\b|\bوبعدين\b|\bثم\b)/i)
    .map(item=>item.trim())
    .filter(Boolean);
}

function sensitive(text) {
  const normalized=text.toLowerCase();
  return SENSITIVE_TERMS.some(term=>normalized.includes(term));
}

function routeState(owner) {
  if (owner === WebOwnership.WEB) return 'WEB_OWNED_PENDING_GOVERNED_EXECUTION';
  if ([WebOwnership.APPLICATION,WebOwnership.FOUNDATION,WebOwnership.GOVERNANCE].includes(owner)) return 'FOREIGN_HANDOFF_REQUIRED';
  return 'OWNER_CLARIFICATION_REQUIRED';
}

export function buildOwnerRequestRoutingPlan({ requestId, ownerMessage, targetPath = null } = {}) {
  const id=nonEmpty(requestId,'requestId');
  const message=nonEmpty(ownerMessage,'ownerMessage');
  const parts=splitCompound(message);
  const items=parts.map((text,index)=>{
    const owner=classifyOwnerRequestOwnership({text,targetPath:index===0?targetPath:null});
    return Object.freeze({
      itemId:`${id}:${index+1}`,
      text,
      owner,
      state:routeState(owner),
      sensitiveConfirmationRequired:sensitive(text),
      requestSent:false,
      actionAccepted:false,
      actionCompleted:false,
      executionAuthorityCreated:false
    });
  });

  return Object.freeze({
    requestId:id,
    ownerMessage:message,
    compound:items.length>1,
    items:Object.freeze(items),
    transportAvailable:false,
    requestSent:false,
    actionAccepted:false,
    actionCompleted:false,
    authorityCreated:false
  });
}

export function applyRoutedHandoffReceipt(plan,{itemId,correlationId,accepted=false}={}) {
  if (!plan || !Array.isArray(plan.items)) throw new TypeError('routing plan is required');
  nonEmpty(itemId,'itemId');
  nonEmpty(correlationId,'correlationId');
  const match=plan.items.find(item=>item.itemId===itemId);
  if (!match) throw new TypeError('unknown itemId');
  if (match.owner === WebOwnership.UNKNOWN) throw new TypeError('unknown-owner item cannot receive routed handoff receipt');
  return Object.freeze({
    itemId,
    correlationId,
    requestSent:true,
    actionAccepted:accepted === true,
    actionCompleted:false,
    executionAuthorityCreated:false
  });
}
