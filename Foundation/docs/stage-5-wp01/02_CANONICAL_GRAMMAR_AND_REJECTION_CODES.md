# Stage 5 WP-01 Canonical Grammar and Rejection Codes

## Message type

A message type contains at least two dot-separated segments.

Each segment:

- is non-empty;
- starts with an ASCII letter;
- contains only ASCII letters, digits, `_`, or `-`.

Examples:

- `falcon.reference.operation.v1`
- `foundation.health.notice`

Rejected examples:

- `.operation`
- `operation.`
- `falcon..operation`
- `1falcon.operation`

## Schema version

A schema version contains two or three numeric dot-separated segments.

Rules:

- no empty segment;
- no non-numeric segment;
- no leading zero unless the complete segment is `0`.

Accepted: `1.0`, `1.2.3`  
Rejected: `.1`, `1.`, `1..0`, `01.0`, `1.a`, `1.2.3.4`

## Outcome reason

Outcome reasons are non-empty canonical tokens using lowercase ASCII letters,
digits, `_`, `-`, `.`, `:`, or `/`.

## SHA-256

SHA-256 is exactly 64 uppercase hexadecimal characters.

## Principal rejection codes/messages

- `identifier_required`
- `identifier_not_canonical`
- `identifier_length_invalid`
- `identifier_character_invalid`
- `message_type_requires_namespace`
- `message_type_segment_required`
- `message_type_segment_must_start_with_letter`
- `message_type_segment_character_invalid`
- `schema_version_not_canonical`
- `schema_version_segment_required`
- `schema_version_leading_zero`
- `schema_version_numeric_segments_required`
- `outcome_reason_not_canonical`
- `sha256_length_invalid`
- `sha256_must_be_uppercase_hex`
- `enum_value_not_defined`
- `correlation_and_causation_must_remain_distinct`
- `payload_digest_does_not_match_payload`
