# Stage 5 WP-02 Schema Registry Contract

## Status

Implementation artifact for authorized Stage 5 WP-02 only.

## Purpose

WP-02 establishes an application-neutral schema registry that binds:

- canonical `SchemaIdentity` from WP-01;
- canonical schema version;
- one declared schema owner across all registered versions of the same schema identity;
- canonical definition SHA-256;
- provenance;
- lifecycle state;
- explicit compatibility rules.

The registry does not interpret Application payload meaning and does not grant publish, subscribe, routing, execution, business, or Owner authority.

## Registration identity

A registration is uniquely addressed by:

`SchemaIdentity + SchemaVersion`

The registered definition additionally binds:

`Owner + DefinitionSha256 + Provenance`

A second registration of the exact same definition is rejected as a duplicate.
A second registration for the same identity/version with changed owner, digest, or provenance is rejected as a conflict. A new version of an existing schema identity with a different owner is also rejected; ownership cannot drift across versions.

## Resolution

Resolution is exact by `SchemaIdentity + SchemaVersion`.

Unknown identities or versions fail closed with `schema_version_unknown`.

No best-match, latest-version, nearest-version, or permissive fallback exists in WP-02.
