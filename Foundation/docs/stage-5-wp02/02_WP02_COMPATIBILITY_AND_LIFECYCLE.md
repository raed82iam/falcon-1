# Stage 5 WP-02 Compatibility and Lifecycle

## Compatibility classifications

The only compatibility classifications are:

- `Exact`
- `Backward`
- `Forward`
- `Incompatible`

`Exact` is implicit only when the same registered schema identity and exact version are compared.

Cross-version compatibility must be explicitly declared. If no rule exists, evaluation fails closed with `compatibility_rule_undeclared`.

`Backward` and `Forward` are explicit compatible relationships.

`Incompatible` is an explicit resolved relationship that is not compatible.

WP-02 does not infer compatibility by reading or interpreting Application payload semantics.

## Rule conflicts

For one directed relation:

`SchemaIdentity + FromVersion + ToVersion`

only one compatibility rule may exist.

An identical second rule is rejected as a duplicate.
A different second rule is rejected as a conflict.

## Lifecycle

Allowed lifecycle progression is:

`Registered -> Active -> Deprecated -> Retired`

No skipping, reversal, or no-op transition is accepted.

Lifecycle is registry truth only. It does not activate an Application, route, transport, subscription, or runtime.


## Deterministic Snapshot Replay

A registry snapshot is a deterministic replay boundary. Reconstructing an
`InMemorySchemaRegistry` from a valid captured snapshot SHALL reproduce the
same revision, ordered entries, compatibility rules, and canonical SHA-256.

Replay SHALL fail closed if snapshot identity relationships are internally
inconsistent.
