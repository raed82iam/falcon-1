# Shared Falcon Web — Trading UI Benchmark and Design Direction

Date: 2026-08-16  
Branch: `web-development`  
Scope: `applications/shared/web/**`

## Purpose

Use current public trading-platform patterns as external design evidence, then synthesize an original Falcon workspace without copying another vendor's visual identity, markup, artwork, proprietary workflow, or execution semantics.

## Benchmarked platforms

### TradingView Supercharts
Public product/support material shows a chart-first workspace with top chart controls, left drawing tools, right watchlist/news/alerts/tool access, and a bottom panel. The key design lesson is that the chart remains the visual center and adjacent tools stay one action away.

Reference: https://www.tradingview.com/support/solutions/43000746464-getting-started-with-supercharts/

### Robinhood Legend
Current Legend material emphasizes customizable layouts, draggable/resizable widgets, layout templates, linked widgets, and direct chart-centered workflows. The key lesson is modern visual cleanliness with modular composition rather than a dense legacy terminal.

References:
- https://robinhood.com/us/en/legend/
- https://robinhood.com/us/en/support/articles/layouts-on-legend/
- https://robinhood.com/us/en/support/articles/widgets-in-robinhood-legend/

### Webull Desktop
Current Webull Desktop material emphasizes drag/drop workspace composition, resizable widgets, chart command-center behavior, account-risk monitoring, trading-performance views, and research in one workspace. The key lesson is dense capability with strong widget hierarchy.

Reference: https://www.webull.com/trading-platforms/desktop-app

### IBKR Desktop / TWS Mosaic
Current IBKR material emphasizes one-screen multi-asset workflows, customizable panels, portfolio/risk context, watchlists, charting, order/activity monitoring, and linked windows. The key lesson is professional information density and a unified workflow, while avoiding unnecessary complexity for ordinary users.

References:
- https://www.interactivebrokers.com/en/trading/ibkr-desktop.php
- https://www.interactivebrokers.com/en/trading/tws.php

## Falcon synthesis

Falcon should not clone any one competitor. The target composition is:

1. Chart-first visual center.
2. Portfolio/account metrics immediately visible above the analytical workspace.
3. Falcon AI short analysis adjacent to the chart, not hidden on another page only.
4. Strategy/School catalog adjacent but visually subordinate to current analysis.
5. Positions and trades below the primary market workspace.
6. Alerts/Incident surfaces persistent and high-salience when required.
7. Compact side navigation and top application context to preserve screen area.
8. Draggable/resizable widgets remain available as Web-owned preference behavior.
9. Desktop is information-dense; mobile collapses to one coherent vertical workflow.
10. Falcon incident/Guardian/Support interaction is a differentiating safety layer, not a copy of broker execution UX.

## Governance boundaries

The redesign changes presentation hierarchy only. It must preserve:

- `WEB_DISPLAY != BUSINESS_TRUTH_OWNER`
- `UI_CLICK != AUTHORIZATION`
- `WEB_PRESENTATION_DATA != FSATS_OPERATIONAL_DATA`
- `DISPLAYED_OVERLAY != CURRENT_APPLICATION_TRUTH`
- `CATALOG_AVAILABLE != STRATEGY_ACTIVATED`
- `SIMULATOR_ESTIMATE != BROKER_TRUTH`

No external visual benchmark grants provider, broker, execution, identity, Foundation, or Application authority.

## Implemented first pass

The first chart-first Command Center pass is implemented by:

- `src/trading-workspace.css`
- revised default widget order in `src/state.js`
- stylesheet activation in `index.html`

This first pass intentionally uses existing governed Web data surfaces and does not invent new Application truth or live provider bindings.
