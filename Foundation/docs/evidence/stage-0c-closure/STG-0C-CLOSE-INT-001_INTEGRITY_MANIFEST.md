# Stage 0C Closure Integrity Manifest

**Manifest ID:** STG-0C-CLOSE-INT-001  
**Version:** 1.0  
**Status:** Recorded  
**Algorithm:** SHA-256

## Closure Evidence Tree

**Included Files:** 14  
**Root SHA-256:** `1E1587676B17FEDBF3872331E88318736940BA346E479FCD82278C694693966D`

The root excludes this Integrity Manifest and uses ordinally sorted forward-slash relative paths encoded as:

```text
relative-path|UPPERCASE-SHA256\n
```

The record stream is UTF-8 and ends with LF.

## Decision Integrity

| Decision | SHA-256 |
|---|---|
| `GOV-059_STAGE_0C_COMPLETION_AND_CLOSURE.md` | `E9EF99A9BDCC1FBA2A52F3600DD1221CC6AE094BC081E87C0BDE286FA49B4BEF` |

The preserving repository commit establishes this Manifest’s own identity.

Corrections require a new version and shall preserve this record.
