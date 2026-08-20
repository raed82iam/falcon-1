import { DataSourceMode } from '../core/data-source-profile.js';
import { createIncidentPersistencePort, createIndexedDbIncidentPersistence } from './incident-persistence.js';

const requiredMethods = Object.freeze([
  'saveRecord','loadRecord','appendEvent','putArtifact','getArtifact','loadEvents','commitRecordAndEvent','commitArtifactAndEvents'
]);

function unavailablePort(reason) {
  const unavailable = async () => Object.freeze({ ok:false, reason });
  return createIncidentPersistencePort({
    saveRecord:unavailable,
    loadRecord:unavailable,
    appendEvent:unavailable,
    putArtifact:unavailable,
    getArtifact:unavailable,
    loadEvents:unavailable,
    commitRecordAndEvent:unavailable,
    commitArtifactAndEvents:unavailable
  });
}

function validProductionBinding(binding) {
  if (!binding || typeof binding !== 'object') return false;
  if (binding.authoritative !== true) return false;
  if (binding.tenantScoped !== true) return false;
  if (binding.businessAuthorityGranted !== false) return false;
  if (typeof binding.tenantNamespace !== 'string' || binding.tenantNamespace.trim() === '') return false;
  if (typeof binding.evidenceReference !== 'string' || binding.evidenceReference.trim() === '') return false;
  return requiredMethods.every(name => typeof binding.port?.[name] === 'function');
}

/**
 * Select incident persistence without allowing an authoritative runtime to
 * silently fall back to browser-local preview storage.
 */
export function createIncidentPersistenceBinding({
  dataSourceMode,
  indexedDBImpl = globalThis.indexedDB,
  productionBinding = null
} = {}) {
  if (!Object.values(DataSourceMode).includes(dataSourceMode)) {
    throw new TypeError('unsupported incident persistence dataSourceMode');
  }

  if (dataSourceMode === DataSourceMode.PREVIEW) {
    return Object.freeze({
      port:createIndexedDbIncidentPersistence({ indexedDBImpl }),
      mode:'PREVIEW_LOCAL_INDEXEDDB',
      authoritative:false,
      tenantScoped:false,
      productionReady:false,
      reason:null
    });
  }

  if (!validProductionBinding(productionBinding)) {
    return Object.freeze({
      port:unavailablePort('PRODUCTION_TENANT_SCOPED_INCIDENT_PERSISTENCE_REQUIRED'),
      mode:'AUTHORITATIVE_FAIL_CLOSED',
      authoritative:false,
      tenantScoped:false,
      productionReady:false,
      reason:'PRODUCTION_TENANT_SCOPED_INCIDENT_PERSISTENCE_REQUIRED'
    });
  }

  return Object.freeze({
    port:productionBinding.port,
    mode:'AUTHORITATIVE_TENANT_SCOPED',
    authoritative:true,
    tenantScoped:true,
    productionReady:true,
    tenantNamespace:productionBinding.tenantNamespace,
    evidenceReference:productionBinding.evidenceReference,
    reason:null
  });
}
