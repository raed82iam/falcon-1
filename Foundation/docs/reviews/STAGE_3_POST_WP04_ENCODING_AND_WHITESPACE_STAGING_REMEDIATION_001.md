# Stage 3 Post-WP-04 Encoding and Whitespace Staging Remediation 001

## Status

**PASS**

## Context

The first controlled baseline staging operation validated the uploaded 819-file candidate and staged the intended repository snapshot. The standard `git diff --cached --check` command then reported findings.

The findings had two distinct causes:

1. CRLF bytes were staged without Git text normalization because the staging command forced `core.autocrlf=false`.
2. Existing Markdown documents intentionally use exactly two trailing spaces for Markdown hard line breaks and, in some historical documents, an extra blank line at end of file.

A separate static inspection also identified legacy encoding damage in five documentary files.

## Encoding remediation

The following files were repaired:

1. `docs/04_SPECIFICATION_TREE.md`
   - repaired seven layers of reversible UTF-8/Windows-1252 mojibake in the canonical tree diagram;
   - preserved the current canonical content and relationships;
   - restored box-drawing characters such as `├──`, `│`, and `└──`.

2. `docs/05_LEGACY_MIGRATION_MAP.md`
3. `docs/specifications/core/README.md`
4. `docs/activation/candidates/CDA-AMD008-001/administrative/Core_README_v1.1_PROPOSED.md`
5. `docs/activation/candidates/CDA-AMD008-001/administrative/GOV-002_v1.1_PROPOSED.md`
   - replaced the damaged token `Falconï¿½s` with `Falcon’s`.

## Staging remediation

- The previous index state was removed with a mixed reset that preserved all working-tree files.
- The complete snapshot was restaged with Git text normalization enabled.
- No source, JSON, TXT, project, or solution file retained a `git diff --cached --check` finding.
- Remaining findings were accepted only when both conditions held:
  - the path ended in `.md`; and
  - the finding was either:
    - exactly two trailing spaces used as a Markdown hard line break; or
    - a new blank line at end of a Markdown file.

## Validation

- HEAD remained unchanged.
- Branch remained `main`.
- No commit was created.
- No tag was created.
- No push was performed.
- Duplicate governance identifiers remained zero.
- `invalid-intermediate` files remained absent.
- WP-05 remained `ON HOLD`.

## Authority boundary

This remediation does not:

- authorize WP-05;
- authorize deployment;
- authorize runtime activation;
- authorize external connectivity;
- authorize financial activity; or
- change any accepted technical scope.

## Result

The corrected staged snapshot is suitable for commit review, subject to review of the staged inventory and final manifest.
