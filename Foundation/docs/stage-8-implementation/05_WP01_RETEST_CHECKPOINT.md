# Stage 8 WP-01 Replacement Retest Checkpoint

Stage 8 WP-01 remains technically open pending executable retest.

The initial exact candidate failed only at the WP-01 verifier after restore, Release build, Architecture, Security, and Stage 7 cross-stage predecessor regression had passed.

The verifier-layer defect has been remediated without changing `Foundation.Guardian` production behavior.

The replacement exact candidate is the commit containing this checkpoint. Validation shall use an exact detached checkout and shall require:

1. clean exact checkout;
2. controlled restore;
3. controlled Release build;
4. Architecture PASS;
5. Security PASS;
6. Stage 7 Cross-Stage predecessor regression PASS;
7. Stage 8 WP-01 verifier PASS twice at 12/12;
8. identical verifier output;
9. stable Guardian/verifier/Architecture/Security binary hashes;
10. final exact HEAD and clean worktree.

No Owner closure is requested for WP-01. On technical PASS, the authorized cadence proceeds automatically to WP-02.
