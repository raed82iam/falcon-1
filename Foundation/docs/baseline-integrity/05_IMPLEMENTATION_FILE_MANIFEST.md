# Baseline Integrity Implementation File Manifest

**Status:** Controlled implementation in progress  
**Master allowlist:** 71 exact paths  
**Phase A payload:** build/text controls, current canonical documentary targets, governance, planning, and remediation records only.

Later phases may modify only the remaining paths already present in the approved master allowlist.

Every application package must verify:

1. the exact branch, commit, tree, tag, authority, and corrected allowlist;
2. a clean working tree at its phase boundary;
3. payload hashes;
4. changed paths are a subset of the master allowlist;
5. protected historical and non-allowlisted paths remain unchanged;
6. no staging, commit, tag, merge, rebase, or push occurs.
