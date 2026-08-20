# 05 - Stage 1 Architecture Impact Review

## Project dependency direction

The project dependency direction SHALL be inward:

1. solution and build orchestration;
2. contracts and schema boundaries;
3. foundation source projects;
4. infrastructure and adapters;
5. tests and verification assets.

No dependency may flow from infrastructure back into the protected core.

## Protected core boundary

The protected core boundary is the Foundation source and contract surface under
the controlled project foundation. It SHALL remain free of application business
logic, runtime behavior, cloud behavior, and financial behavior.

## Allowed references

Allowed references are:

- canonical vision and constitution;
- document authority;
- active Stage 0 closure records;
- active enabling baseline manifests;
- foundation contracts and specifications required by the proposal;
- verification, evidence, and traceability artifacts that are planned inside
  the project boundary.

## Prohibited references

Prohibited references are:

- application business logic;
- runtime behavior of Falcon applications;
- production, cloud, or financial systems;
- uncontrolled external dependencies;
- any path outside the planned repository boundary.

## Adapter boundary

Any external dependency MUST be isolated behind an adapter boundary and MUST
not become a direct protected-core reference.

## Foundation/Application separation enforcement

Stage 1 enforces separation by:

- placing application behavior outside the controlled project foundation;
- keeping contracts and dependencies explicit;
- forbidding direct dependency direction into prohibited application behavior;
- requiring architecture analysis as a planned deliverable and verification
  target; and
- requiring boundary failure to stop the stage.

## How Stage 1 avoids runtime behavior

Stage 1 defines the repository, solution, build, verification, and evidence
boundary for future work. It does not add Falcon runtime behavior, because the
stage work is restricted to project foundation and reproducible empty-build
design.

