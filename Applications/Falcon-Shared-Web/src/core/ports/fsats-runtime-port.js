import { TruthState, AnalysisResultState } from '../../contracts.js';

/**
 * Web-owned FSATS runtime projection boundary.
 *
 * Method names describe Web needs only. Implementations must bind through governed
 * public Application contracts and must not expose FSATS internals or provider/broker internals.
 *
 * `chart` is retained only as the Application-provided chart compatibility surface
 * defined by FCR-0125. Ordinary raw presentation market data belongs to the separate
 * WebMarketDataPort and MUST NOT be routed through FSATS by default.
 */
export const FsatsRuntimePortMethods = Object.freeze([
  'portfolio',
  'activity',
  'chart',
  'tradingOverlay',
  'strategyCatalog',
  'onDemandAnalysis',
  'detailedAnalysis',
  'incidents'
]);

const unavailable = extra => Object.freeze({ truth: TruthState.UNAVAILABLE, ...extra });

export function createUnavailableFsatsRuntimePort() {
  return Object.freeze({
    async portfolio() { return unavailable(); },
    async activity() { return unavailable({ items: [] }); },
    async chart() { return unavailable({ bars: [], compatibilityOnly: true }); },
    async tradingOverlay() { return unavailable({ items: [] }); },
    async strategyCatalog() { return unavailable({ items: [] }); },
    async onDemandAnalysis() { return unavailable({ resultState: AnalysisResultState.UNAVAILABLE }); },
    async detailedAnalysis() { return unavailable({ resultState: AnalysisResultState.UNAVAILABLE }); },
    async incidents() { return unavailable({ items: [] }); }
  });
}
