# Owner Decision — Shared Falcon Visual Asset

Date: 2026-08-17
Workstream: Shared Falcon Web Application
Branch: `web-development`
Status: OWNER_APPROVED

## Decision

The Project Owner approved the square 1:1 Falcon OS visual supplied in the current Web design review as the common Falcon visual identity for Shared Web pages that require a primary image or illustration.

Canonical Web asset:

`applications/shared/web/src/assets/falcon-shared-visual.jpg`

## Required use

- When a Shared Web page requires a primary hero image, feature illustration, empty-state visual, discovery image, or comparable branded visual, use the canonical shared Falcon visual unless the Project Owner explicitly approves a different image for that surface.
- Do not create a different Falcon/bird illustration per page by default.
- Pages that do not need imagery SHALL NOT receive decorative imagery merely to satisfy this decision.
- Reuse may crop or scale the image responsively, but SHALL preserve its square 1:1 source identity and shall not distort its aspect ratio.
- Existing page layout, business semantics, authority boundaries, runtime truth and accessibility requirements are not changed by this visual decision.
- This decision grants visual/presentation authority only. It grants no deployment, runtime activation, external connectivity, trading, Foundation or Application authority.

## Current binding

The Falcon OS public landing Hero uses the canonical shared visual through:

`./src/assets/falcon-shared-visual.jpg`

This supersedes use of the prior generated/vector Falcon Hero artwork for the public landing image.
