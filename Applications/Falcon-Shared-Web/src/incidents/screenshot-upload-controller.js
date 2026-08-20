import { validateScreenshotMeta } from '../incidents.js';
import { IncidentActor, IncidentEventType, createIncidentEvent } from './incident-timeline.js';

export function createIncidentScreenshotUploadController({ incidentId, timeline, persistence, scanner = null, now = () => new Date().toISOString(), idFactory = () => crypto.randomUUID() } = {}) {
  if (!incidentId) throw new TypeError('incidentId is required');
  if (!timeline) throw new TypeError('timeline is required');
  if (!persistence) throw new TypeError('persistence is required');

  async function acceptFiles(files) {
    const list = Array.from(files ?? []);
    if (list.length !== 1) return { ok:false, reason:'ONE_SCREENSHOT_AT_A_TIME' };
    const file = list[0];
    if (!(file instanceof Blob)) return { ok:false, reason:'INVALID_SCREENSHOT_FILE' };
    if (!String(file.type ?? '').startsWith('image/')) return { ok:false, reason:'SCREENSHOT_IMAGE_REQUIRED' };
    if (!scanner || typeof scanner.scanScreenshot !== 'function') return { ok:false, reason:'GOVERNED_SCREENSHOT_SCANNER_UNAVAILABLE' };

    const scan = await scanner.scanScreenshot(file, { incidentId });
    const validation = validateScreenshotMeta({ fileCount:1, containsSecret:scan?.containsSecret === true, securityScanState:scan?.state, scanProvenance:scan?.provenance });
    if (!validation.allowed) return { ok:false, reason:validation.reason };

    const artifactId = idFactory();
    const artifact = { artifactId, incidentId, kind:'CUSTOMER_SCREENSHOT', blob:file, metadata:{ mimeType:file.type, scanEvidenceReference:scan?.evidenceReference ?? null } };
    const event = createIncidentEvent({ eventId:idFactory(), incidentId, timestamp:now(), actor:IncidentActor.CUSTOMER, type:IncidentEventType.SCREENSHOT, payload:{ artifactId, mimeType:file.type, scanEvidenceReference:scan?.evidenceReference ?? null }, provenance:'GOVERNED_UPLOAD_SECURITY_SCANNER' });

    if (typeof persistence.commitArtifactAndEvents !== 'function') return { ok:false, reason:'ATOMIC_MEDIA_PERSISTENCE_UNAVAILABLE' };
    const committed = await persistence.commitArtifactAndEvents({ artifact, events:[event] });
    if (!committed.ok) return { ok:false, reason:'SCREENSHOT_ATOMIC_PERSISTENCE_FAILED' };
    timeline.append(event);
    return Object.freeze({ ok:true, event, artifactId });
  }

  return Object.freeze({ acceptFiles });
}
