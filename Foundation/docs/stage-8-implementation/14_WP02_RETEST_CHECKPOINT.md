# Stage 8 WP-02 Retest Checkpoint

**Stage:** 8
**WP:** 02
**Date:** 2026-08-14
**Branch:** `foundation-development`

## Purpose

Freeze the post-remediation candidate for exact executable retest after the verifier-only nullable-flow correction.

## Required retest

The retest SHALL verify:

1. exact candidate checkout and clean worktree;
2. controlled restore and Release build;
3. Architecture validation;
4. Security validation;
5. Stage 7 Cross-Stage predecessor regression;
6. Stage 8 WP-01 regression (12/12);
7. Stage 8 WP-02 verifier twice (17/17 each);
8. deterministic identical WP-02 output;
9. Guardian/WP01/WP02/Architecture/Security binary hash stability;
10. exact final HEAD and clean worktree.

A successful retest creates a technical checkpoint only. It does not close WP-02 or Stage 8 by Owner decision. Under the Owner-authorized Stage 8 cadence, PASS permits automatic continuity into WP-03.
