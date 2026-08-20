# STG-0C-VER-001 — Verification and Independent Review Plan

**Identifier:** STG-0C-VER-001  
**Version:** 1.0  
**Status:** Approved  
**Prepared:** 2026-07-27  
**Approval Date:** 2026-07-27  
**Approval Record:** GOV-055  
**Governing Authority:** GOV-054; STG-0C-PROP-001; VPL-BST-006 through VPL-BST-008  
**Verification Authority:** Granted within Stage 0C only

## 1. Purpose

This candidate defines the future execution boundary for VPL-BST-006, VPL-BST-007, and VPL-BST-008.

## 2. Verification Scope

- VPL-BST-006: exact environment identity, capability, isolation, dependencies, authority, restriction, revocation, expiry, restoration, wrong-subject/profile, and reconstruction.
- VPL-BST-007: atomic trace, frozen obligations, evidence lineage, completeness, negative and mutation cases, Gate integrity, self-promotion prevention, and independent reconstruction.
- VPL-BST-008: end-to-end reconstruction of authorities, subjects, tools, evidence, evaluation contexts, validity, completeness, reviews, decisions, corrections, restrictions, and non-authorities.

Applicable Provider Contracts and negative cases remain mandatory.

## 3. Independence

Independent review shall be organizationally or procedurally separate from material production and from the authority whose decision is challenged. Low-impact exceptions require explicit governing policy; none is assumed here.

## 4. Evaluation Record

Each evaluation shall declare Evaluation Mode, Nature, Authority, Context ID, inputs, rules, tool or human identity, outcome, uncertainty, scope, and reproducibility classification.

AI evaluation shall declare its Nature and is `JUDGMENT_BASED` unless deterministic behavior is demonstrated under a governed execution profile.

## 5. Stop Rule

Missing authority, evidence, independence, exact identity, admitted tool, isolation, or reconstructability stops the affected case. Passing verification does not activate anything.

## 6. Current Effect

No VPL or other verification may run under this candidate.
