# Documentary Rollback Plan

**Status:** Proposed Plan  
**Rollback Authority:** Not Granted

## Principle

Rollback is a new governed documentary decision. It is never deletion, silent pointer reversal, history rewriting, or automatic reactivation of a predecessor.

## Triggers

- Critical post-activation inconsistency;
- missing or corrupted canonical artifact;
- digest mismatch;
- duplicate identity/version;
- authority or constitutional conflict;
- mixed old/new awareness hierarchy;
- broken registry/index/tree coherence;
- inability to prove complete atomic activation.

## Proposed Rollback Sequence

1. freeze reliance on the new documentary baseline;
2. preserve activation and discrepancy evidence;
3. obtain a Project Owner rollback decision naming the exact baseline;
4. stage restored canonical pointers to the complete pre-activation manifest;
5. verify all restored digests and links;
6. atomically publish the entire restored pre-activation canonical documentary baseline, including governing content, Contracts, Specifications, ADR index, registries, tree, glossary pointers, trace, roadmap, and readiness records;
7. retain the failed activation baseline as immutable history;
8. issue an independent post-rollback consistency report.

## Limit

Rollback cannot make AWR-001 v1.0 or any predecessor current merely by deleting successors. Current status is restored only through the explicit rollback record and complete baseline manifest.

No code, runtime state, deployment, external integration, production state, or financial state is within this plan.

## Newly Admitted File Disposition

New canonical files introduced by the failed activation SHALL NOT be deleted. Under the rollback decision they are copied to:

`docs/archive/rolled-back-cda-amd008-001/<original-canonical-relative-path>`

Their canonical current pointers are removed only as part of the atomic restored baseline, and their metadata/history records the failed activation and rollback. The candidate and activation manifests remain immutable.
