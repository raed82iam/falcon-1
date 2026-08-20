# P1-C — Repository, Solution, Project and Package Topology

**Status:** `DESIGN_CANDIDATE / NOT_OWNER_ACCEPTED / NOT_CLOSED`  
**Implementation Authority:** `NOT_GRANTED`  
**Runtime Authority:** `NOT_GRANTED`  
**Organizational Source:** `04_ACTIVE_WORK/PART_1/01_PART1NG_WORK_PACKAGE_DECOMPOSITION.md`

### Objective
Define the physical build structure only after the governing Foundation integration surfaces are known.

### Required design decisions
- exact solution/workspace structure;
- independently identifiable/buildable top-level Application boundaries;
- exact structural home for FSARM without silently making the non-owning FSATS boundary an Application or hidden runtime principal;
- project/package naming and stable ownership;
- dependency direction rules;
- public versus internal project boundaries;
- shared Application-owned libraries only where genuine semantic commonality exists;
- prohibition of `FSATS` as an ungoverned runtime owning project/principal;
- no direct project reference that bypasses a governed cross-Application contract;
- no Foundation source copying.

### Required outputs
- canonical project/package tree;
- project ownership matrix;
- allowed dependency matrix;
- forbidden dependency matrix;
- package/versioning rules;
- replacement/removal impact map;
- FSARM structural placement decision contingent on Foundation reconciliation where required.

### Closure criteria
Every future source file has an unambiguous owning project/package and no topology creates hidden cross-Application or Foundation authority.
