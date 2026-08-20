# WP-05 Scope, Authority, and Baseline

WP-05 is authorized by `GOV-096` and is anchored to the frozen post-WP-04 commit, tree, tag, and committed manifest listed there.

## In scope

- bootstrap-context gating;
- subject admission and service-registration evidence binding;
- bootstrap provenance validation;
- admitted time-provider validation;
- WP-04 dependency graph and activation-order evidence binding;
- protective restriction and controlled release handling;
- immutable lifecycle-state model;
- explicit lifecycle transition evaluation;
- transition-attempt and accepted-event evidence;
- deterministic independent WP-05 verification.

## Out of scope

- WP-06 end-to-end chain execution;
- runtime hosting or process orchestration;
- Service Bus, Event Bus, or FIL transport;
- persistence realization outside in-memory verification;
- networking, external providers, cloud, brokers, market data, and financial behavior;
- deployment or production activation.

## Historical evidence exclusion

`docs/reviews/wp05-evidence/` existed in the frozen baseline and contains historical format/restore remnants. It is not treated as WP-05 execution evidence and is not modified or relied upon by this work package.
