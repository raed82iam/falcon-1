# Stage 4 WP-06 Static Design

WP-06 integrates the accepted Authority, Lifecycle, State, Evidence, commit, anchor, and Restart Reconciliation capabilities.

The lifecycle restart factory invokes `RestartReconciler` before a `LifecycleControlService` instance is returned for continuation. Missing, divergent, corrupted, ambiguous, or challenge-required reconciliation produces no service instance and therefore no continuation surface.

WP-06 does not create a second lifecycle controller, State owner, or Evidence owner.
