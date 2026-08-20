const defaultLayout = ['portfolio','daily','performance','market','summary','catalog','positions','trades','alerts'];
const defaultPrefs = { hidden: [], sizes: {}, order: defaultLayout };

function safeParse(raw, fallback){
  try { return raw ? JSON.parse(raw) : fallback; } catch { return fallback; }
}

function storageGet(key) {
  try { return globalThis.localStorage?.getItem?.(key) ?? null; }
  catch { return null; }
}

function storageSet(key,value) {
  try { globalThis.localStorage?.setItem?.(key,value); return true; }
  catch { return false; }
}

function storageRemove(key) {
  try { globalThis.localStorage?.removeItem?.(key); return true; }
  catch { return false; }
}

function cloneDefaultPrefs() {
  return {
    hidden:[...defaultPrefs.hidden],
    sizes:{...defaultPrefs.sizes},
    order:[...defaultPrefs.order]
  };
}

export const store = {
  get language(){ return storageGet('falcon.lang') || 'ar'; },
  layout(){
    const parsed = safeParse(storageGet('falcon.dashboard.layout'), {});
    const stored = {
      ...cloneDefaultPrefs(),
      ...parsed,
      hidden:Array.isArray(parsed.hidden) ? [...parsed.hidden] : [],
      sizes:parsed.sizes && typeof parsed.sizes === 'object' && !Array.isArray(parsed.sizes) ? {...parsed.sizes} : {},
      order:Array.isArray(parsed.order) ? [...parsed.order] : [...defaultLayout]
    };
    stored.order = [...stored.order, ...defaultLayout.filter(id => !stored.order.includes(id))];
    return stored;
  },
  saveLayout(layout){ storageSet('falcon.dashboard.layout', JSON.stringify(layout)); },
  resetLayout(){ storageRemove('falcon.dashboard.layout'); return cloneDefaultPrefs(); },
  hideWidget(id){ const p=this.layout(); p.hidden=[...new Set([...p.hidden,id])]; this.saveLayout(p); return p; },
  showWidget(id){ const p=this.layout(); p.hidden=p.hidden.filter(x=>x!==id); this.saveLayout(p); return p; },
  resizeWidget(id, size){ const p=this.layout(); p.sizes={...p.sizes,[id]:size}; this.saveLayout(p); return p; },
  reorderWidget(source,target){
    const p=this.layout(); const order=[...p.order]; const a=order.indexOf(source), b=order.indexOf(target);
    if(a<0||b<0||a===b) return p;
    order.splice(a,1); order.splice(b,0,source); p.order=order; this.saveLayout(p); return p;
  }
};

export const __test = Object.freeze({ storageGet, storageSet, storageRemove, cloneDefaultPrefs });
