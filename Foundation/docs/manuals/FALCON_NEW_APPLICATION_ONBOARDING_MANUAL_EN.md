# Falcon New Application Onboarding Manual — English

**Audience:** Human architects, Application owners, developers, reviewers, operators, and integration teams  
**Purpose:** Explain, in practical human terms, exactly how to add a new Falcon Application to the sealed live Falcon Foundation without modifying Foundation.  
**Foundation Mode:** LIVE / SEALED / APPLICATION-NEUTRAL  
**Primary Governing References:** Falcon Vision; Falcon Constitution; APP-001; CON-023; FDN-006; FDN-007; applicable accepted Foundation contracts and runtime rules.  

---

# 1. What this manual is for

This manual explains how a human team should prepare a brand-new Falcon Application so that Falcon Foundation can safely host it.

The key rule is simple:

> The new Application must fit Falcon Foundation. Falcon Foundation must not be changed to fit the new Application.

Once Foundation is live and sealed, onboarding a new Application is not a Foundation development activity. It is an Application design, declaration, validation, and integration activity.

You should think of Falcon Foundation as a stable operating platform with published rules. A new Application may be completely new in business purpose, but it must speak the same governed hosting language.

Examples of valid future Applications may include:

- Accounting
- Logistics
- Research
- Reporting
- AI orchestration
- Communication
- Document processing
- Monitoring
- Medical-domain software
- Portfolio tools
- Workflow automation
- Any other future domain

Foundation does not need advance knowledge of the business domain.

Foundation only needs the Application to prove:

- who it is;
- what version it is;
- who owns it;
- what it needs;
- what it provides;
- what it is allowed to request;
- what resources it needs;
- how it communicates;
- how it fails safely;
- how it starts, stops, updates, recovers, and is removed;
- what authority it actually holds;
- and that every required declaration is valid and attributable.

---

# 2. The one rule you must never forget

After Foundation is live and sealed:

```text
NEW APPLICATION -> MUST ADAPT TO FOUNDATION
FOUNDATION -> MUST NOT ADAPT TO NEW APPLICATION
```

This means the following are prohibited:

- asking Foundation to add your Application name;
- asking Foundation to add your version;
- asking Foundation to create a special runtime branch for your Application;
- asking Foundation to skip a Manifest rule;
- asking Foundation to weaken admission;
- asking Foundation to relax security;
- asking Foundation to bypass lifecycle rules;
- asking Foundation to auto-grant resources;
- asking Foundation to treat registration as activation;
- asking Foundation to let your Application self-authorize;
- asking Foundation to expose secret bytes directly;
- asking Foundation to add a hidden provider shortcut;
- asking Foundation to weaken fail-closed behavior;
- asking Foundation to add business logic for your domain.

If your Application cannot work without such a change, the Application design is not ready for Falcon Foundation.

---

# 3. The three possible onboarding outcomes

At the end of the onboarding process, your team must reach exactly one of these outcomes.

## 3.1 READY_FOR_FOUNDATION_ADMISSION

Use this when the Application fully satisfies the published Foundation rules and evidence is complete.

## 3.2 APPLICATION_REDESIGN_REQUIRED

Use this when the Application can probably be corrected on its own side without touching Foundation.

Typical examples:

- Manifest incomplete;
- wrong resource request;
- provider adapter designed incorrectly;
- secret handling is unsafe;
- authority boundaries are unclear;
- lifecycle design is incomplete;
- communication bypasses approved transport;
- MSA/LSA ownership is unclear.

## 3.3 INCOMPATIBLE_WITH_SEALED_FOUNDATION

Use this when the Application requires a Foundation change in order to work.

There is no outcome called:

```text
CHANGE_FOUNDATION_FOR_THIS_APPLICATION
```

---

# 4. Before you write code

Do not begin by coding business logic.

First define the Application as a governed subject.

Create a design record containing, at minimum:

1. Application name
2. Application identity
3. Application version
4. owner
5. purpose
6. business boundary
7. what the Application explicitly does not own
8. major internal branches
9. MSA identity
10. LSA identities for major branches
11. optional CSA eligibility
12. provider dependencies
13. Foundation dependencies
14. shared Application dependencies
15. external systems
16. data ownership
17. authority requests
18. resource needs
19. failure behavior
20. recovery behavior
21. update behavior
22. removal behavior

If you cannot clearly describe these items, you are not ready to integrate with Foundation.

---

# 5. Define the Application identity

Every Application needs a stable identity.

At minimum define:

```text
ApplicationIdentity
ApplicationVersion
ApplicationOwner
ApplicationPurpose
PackageIdentity
PackageVersion
ManifestIdentity
ProvenanceIdentity
```

Do not use vague values such as:

```text
app1
latest
current
team
service
```

Use explicit, attributable values.

Example:

```text
ApplicationIdentity = application/accounting/core
ApplicationVersion = 1.0.0
ApplicationOwner = owner:accounting-platform
ApplicationPurpose = enterprise accounting and ledger workflow
PackageIdentity = package/accounting/core
PackageVersion = 1.0.0
ManifestIdentity = manifest/accounting/core/1.0.0
ProvenanceIdentity = provenance/accounting/core/1.0.0
```

The exact naming format should follow the accepted Falcon conventions used by the consuming workstream.

The important rule is consistency.

The identity used in Manifest, admission, artifact binding, lifecycle evidence, and runtime registration must all refer to the exact same subject and version where exact matching is required.

---

# 6. Define the Application boundary

Write down what the Application owns.

Also write down what it does not own.

Example:

```text
OWNS:
- accounting workflows
- accounting data model
- accounting calculations
- accounting provider adapters
- accounting reports

DOES NOT OWN:
- Falcon Kernel
- Foundation lifecycle authority
- Foundation FSA
- Foundation Guardian
- Foundation resource allocator
- Foundation admission rules
- another Application's business state
```

This protects architecture before implementation begins.

A hosted Application remains an Application.

It does not become Foundation merely because it is important.

---

# 7. Design the awareness structure

For a self-aware Falcon Application, define the awareness structure clearly.

## 7.1 MSA

Every major Falcon Application must have one Application MSA identity.

The MSA understands the Application as a whole.

It does not become FSA.

## 7.2 LSA

Each major branch should have exactly one responsible LSA where the architecture uses branch-level awareness.

Examples:

```text
Application MSA
  ├─ Data LSA
  ├─ Execution LSA
  ├─ Reporting LSA
  └─ Provider LSA
```

## 7.3 CSA

CSA is only for eligible intelligent components.

Do not create CSA everywhere by default.

## 7.4 Boundary rule

```text
CSA -> LSA -> MSA -> FSA REVIEW
```

FSA remains Foundation-level.

The Application cannot place an MSA, LSA, or CSA inside Foundation.

---

# 8. Build the Manifest before integration

The Manifest is not a decorative document.

It is the Application's formal declaration to Foundation.

Your Manifest should be complete enough to answer the following questions.

## 8.1 Identity

- Who are you?
- What version are you?
- Who owns you?
- What is your purpose?

## 8.2 Package and provenance

- What package is being admitted?
- What version?
- What exact integrity/content identity?
- Where did it come from?
- What provenance evidence proves this?

## 8.3 Dependencies

- What Foundation contracts do you require?
- What Foundation specifications do you require?
- What Foundation services do you consume?
- What shared Application capabilities do you consume?
- What versions are compatible?

## 8.4 Capabilities

- What capabilities do you provide?
- Who may consume them?
- Are they private or shared?
- Are any exclusive?

## 8.5 Permissions and authority

- What permissions do you need?
- What authority do you request?
- What actions require separate approval?

Remember:

```text
AUTHORITY_REQUEST != AUTHORITY_GRANT
```

## 8.6 Security

- What security profile applies?
- How are secrets referenced?
- What external connections exist?
- What provider boundaries exist?
- What happens when identity or credentials are invalid?

## 8.7 Resources

- Minimum resource requirements
- Quota expectations
- Maximum ceilings
- priority
- degraded mode
- behavior if resources are reduced

## 8.8 Lifecycle

Declare behavior for:

```text
Install
Validate
Admit
Register
Activate
Update
Suspend
Recover
Replace
Remove
```

## 8.9 Health and failure

- How does the Application report health?
- What does degraded mean?
- What does failed mean?
- How is failure contained?
- How does rollback work?

## 8.10 Evidence

- What evidence is created?
- Who owns it?
- How can decisions be reconstructed?
- What proves a specific version was admitted and registered?

---

# 9. Dependency rules

Never declare a Foundation dependency that does not exist.

Every required Foundation reference must resolve to a valid governed identity/version.

Do not write your own fake Foundation contract.

Do not assume a Foundation capability exists because it would be convenient.

If you need a business capability, first ask:

> Is this actually a Foundation responsibility?

Usually business-specific functionality belongs inside the Application or an already-approved Shared Application.

Examples:

```text
Accounting tax calculation -> Application
Trading strategy logic -> Application
Dashboard chart rendering -> Web Application
Provider-specific business transformation -> Application
Kernel lifecycle authority -> Foundation
Foundation admission -> Foundation
Foundation resource governance -> Foundation
```

---

# 10. Resource design

The Application must declare realistic resource needs.

The validated Foundation runtime model enforces a relationship equivalent to:

```text
Allocation <= Quota <= Ceiling
```

Do not design an Application that only works when it receives unlimited resources.

Define:

- minimum viable allocation;
- normal allocation;
- maximum safe ceiling;
- degraded behavior;
- what work stops first under pressure;
- what state must still be preserved;
- what operations are safe during scarcity.

Resource evidence must belong to the same Application identity.

Do not reuse another Application's resource grant.

---

# 11. Capability design

For every capability, declare:

```text
CapabilityIdentity
Owner
Visibility
Consumers
Version
Exclusivity if applicable
Required authority
```

Ask:

- Is it private?
- Is it shared?
- Can another Application consume it?
- Is it exclusive?
- Does consuming it create authority? Usually no.

Remember:

```text
CAPABILITY_AVAILABLE != BUSINESS_AUTHORITY
```

An Application capability does not become a Foundation service merely because many Applications use it.

---

# 12. Communication design

Use governed Falcon communication paths.

Do not invent hidden side channels.

Do not use transport as an authority shortcut.

Keep these distinctions clear:

```text
MESSAGE_ACCEPTED != BUSINESS_ACTION_AUTHORIZED
REQUEST_TRANSPORT != EXECUTION_TRANSPORT
PUBLIC_PROJECTION != CONTROL_REQUEST
PROJECTION_AVAILABLE != CONTROL_AUTHORITY
```

For each message or route define:

- producer;
- consumer;
- schema;
- purpose;
- authority expectation;
- expiry or freshness rule;
- error handling;
- stale-data handling;
- retry behavior;
- evidence behavior.

---

# 13. Provider and external-access design

A provider connection is not authority.

Examples:

```text
API KEY EXISTS != CONNECTION AUTHORIZED
CONNECTION READY != CONNECTION ACTIVATED
DATA RECEIVED != BUSINESS ACTION AUTHORIZED
BROKER CONNECTED != TRADE AUTHORIZED
```

For each provider define:

- provider identity;
- purpose;
- route;
- authentication mode;
- credential reference;
- quota/rate limit;
- failure behavior;
- retry limits;
- timeout behavior;
- stale-data behavior;
- authority boundary;
- what data is allowed to enter the Application;
- what data is prohibited from becoming authoritative.

Do not place secret bytes in normal Application state.

Use governed credential/secret references.

---

# 14. Security design

Before onboarding, answer:

- What is trusted?
- What is untrusted?
- What is authenticated?
- What is authorized?
- What is merely reachable?
- What happens when trust becomes unknown?

The default is fail closed.

Examples of fail-closed conditions:

- missing identity;
- mismatched version;
- tampered Manifest;
- invalid provenance;
- stale resource grant;
- revoked authority;
- unknown dependency;
- invalid provider state;
- invalid credential reference;
- ambiguous runtime authority;
- duplicate runtime identity.

Do not convert unknown into allow.

---

# 15. Lifecycle design in human terms

Do not think of onboarding as one action called "install".

There are separate concepts.

## 15.1 Install

The package exists in the expected installation context.

Install does not mean trusted.

## 15.2 Validate

Declarations and evidence are checked.

Validation does not mean admitted.

## 15.3 Admit

Foundation accepts the candidate as a governed admitted subject.

Admission does not mean running.

## 15.4 Register

The Application is placed into runtime hosting as a registered subject.

Registration does not mean active.

## 15.5 Activate

Separate authority permits the Application to become active.

## 15.6 Update

A material update must be revalidated as required.

## 15.7 Suspend

The Application may be prevented from normal operation while preserving governed state/evidence.

## 15.8 Recover

Recovery must use bounded approved evidence and authority.

## 15.9 Replace

Replacement is not a silent overwrite.

## 15.10 Remove

Removal must leave Foundation valid and preserve required evidence.

---

# 16. The critical authority separations

Every team member should understand these lines:

```text
VALID_CONTRACT != ADMISSION
ADMISSION != ACTIVATION
ADMISSION != DEPLOYMENT_AUTHORITY
ADMISSION != BUSINESS_AUTHORITY
TECHNICAL_CONSUMPTION != RUNTIME_AUTHORITY
RUNTIME_REGISTRATION != ACTIVATION
RUNTIME_REGISTRATION != DEPLOYMENT_AUTHORITY
RUNTIME_REGISTRATION != PRODUCTION_AUTHORITY
RUNTIME_REGISTRATION != BUSINESS_AUTHORITY
ROUTE_EXISTS != CONNECTION_AUTHORIZED
PROVIDER_CONNECTIVITY != EXECUTION_AUTHORITY
DATA_ACCESS != BUSINESS_AUTHORITY
REQUEST_SENT != ACTION_ACCEPTED != ACTION_COMPLETED
CREDENTIAL_REFERENCE_ID != SECRET_BYTES
```

If someone on the team cannot explain these distinctions, the onboarding review is not finished.

---

# 17. Prepare the admission package

Before requesting admission, prepare a complete integration package containing:

1. exact Application identity;
2. exact version;
3. exact package identity;
4. Manifest;
5. Manifest digest/integrity evidence;
6. provenance identity/content/digest;
7. dependency list;
8. Foundation contract references;
9. Foundation specification references;
10. required service declarations;
11. capability declarations;
12. permission declarations;
13. authority requests;
14. security profile;
15. provider boundary declarations;
16. resource requirements;
17. lifecycle declarations;
18. health/failure declarations;
19. rollback plan;
20. MSA/LSA/CSA declarations;
21. test evidence;
22. reviewer evidence.

Do not submit an incomplete package and expect Foundation to infer missing details.

---

# 18. Pre-admission human review

Before running the admission path, perform a human architecture review.

Ask:

### Identity
- Is identity stable?
- Is ownership explicit?
- Is version exact?

### Architecture
- Is this really an Application?
- Did any Application responsibility leak into Foundation?
- Did any Foundation responsibility leak into the Application?

### Authority
- Is any technical capability being treated as authority?
- Is any route being treated as permission?
- Is any credential being treated as permission?

### Resources
- Are needs bounded?
- Can the Application degrade safely?

### Security
- Are secrets referenced correctly?
- Does unknown trust fail closed?

### Lifecycle
- Can the Application update without silent upgrade?
- Can it be suspended?
- Can it recover?
- Can it be removed?

### Isolation
- Can it fail without taking Foundation down?
- Can it fail without taking unrelated Applications down?

### Awareness
- Is FSA kept in Foundation?
- Are MSA/LSA/CSA kept inside the Application?

If any answer is unclear, stop and redesign before admission.

---

# 19. Admission execution expectations

The exact implementation may be automated, but humans should understand what Foundation is proving.

Foundation verifies things such as:

- supported admission kind;
- required identity fields;
- exact Manifest binding;
- digest integrity;
- provenance integrity;
- bootstrap context;
- provider boundary;
- canonical contract linkage;
- dependency resolution;
- Foundation references;
- permissions;
- authority declarations;
- deterministic evidence.

If these fail, the correct action is to correct the Application package.

Do not modify Foundation.

---

# 20. Runtime registration expectations

After admission, runtime registration still requires exact evidence.

Humans should verify:

- runtime instance identity is unique;
- Application identity matches admission;
- Application version matches admission;
- artifact binding is exact;
- admission evidence is positive and exact;
- lifecycle attach eligibility is valid;
- resource grants are current;
- capabilities are valid;
- no exclusive capability conflict exists;
- registration result is registered-only.

After registration, ask:

```text
Is it registered? YES
Is it active? NOT UNLESS SEPARATELY AUTHORIZED
```

---

# 21. Activation review

Activation requires separate authority.

Before activation verify:

- correct subject;
- correct version;
- correct action;
- current authority;
- non-revoked authority;
- correct lifecycle state;
- valid resource state;
- valid security state;
- no active restriction;
- required evidence present.

Never activate because:

- the test passed;
- the package is installed;
- the developer says it is ready;
- the route works;
- the provider responds;
- the Owner is silent;
- it worked in an earlier version.

---

# 22. Update and upgrade procedure

For every material update:

1. assign a new Application version;
2. update package identity/version as required;
3. update Manifest;
4. regenerate integrity evidence;
5. regenerate provenance evidence;
6. re-evaluate dependencies;
7. re-evaluate Foundation references;
8. re-evaluate permissions;
9. re-evaluate authority requests;
10. re-evaluate resources;
11. re-evaluate security;
12. re-evaluate provider boundaries;
13. re-run required tests;
14. re-run applicable admission/revalidation;
15. obtain new runtime/lifecycle authority where required;
16. preserve rollback capability;
17. preserve old evidence.

Never silently replace one version with another.

---

# 23. Removal procedure

A well-designed Application must be removable.

Before removal:

- stop new work;
- preserve required evidence;
- reconcile state;
- release resources;
- revoke Application-specific authority;
- revoke secret access;
- detach runtime registration as governed;
- confirm no shared dependency is incorrectly removed;
- confirm Foundation remains healthy;
- confirm unrelated Applications remain healthy.

The last Application may be removed and Foundation must still remain structurally valid.

---

# 24. What to do if your Application needs something Foundation does not provide

Do not request a Foundation change after live seal.

Use this decision path:

```text
Does the Application need a business-specific capability?
    YES -> implement inside Application if architecture allows.

Does an approved Shared Application already provide it?
    YES -> consume that capability under its contract.

Can an Application-side adapter translate the need into existing Foundation contracts?
    YES -> build adapter on Application side.

Can the feature be redesigned or removed?
    YES -> redesign/remove.

Still requires Foundation source/contract/runtime change?
    YES -> INCOMPATIBLE_WITH_SEALED_FOUNDATION.
```

Foundation remains unchanged.

---

# 25. Common mistakes

## Mistake 1: "Our Application is special"

No Application is special to Foundation.

## Mistake 2: "The API works, so we have authority"

Wrong.

## Mistake 3: "We are registered, so start running"

Wrong.

## Mistake 4: "We need a secret, so store it in config"

Wrong unless the governed secret mechanism explicitly permits the representation.

## Mistake 5: "Let's add one tiny Foundation exception"

Prohibited after live seal.

## Mistake 6: "This shared capability should become Foundation"

Reuse does not change architectural ownership.

## Mistake 7: "Unknown means probably okay"

Wrong. Unknown means deny when trust/authority is required.

## Mistake 8: "We can update in place because it is backward compatible"

Compatibility does not remove versioning and validation obligations.

---

# 26. Human-ready onboarding checklist

Before declaring the Application ready, answer YES to every applicable item.

## Identity

- [ ] Application identity is explicit.
- [ ] Application version is explicit.
- [ ] owner is explicit.
- [ ] purpose is explicit.
- [ ] package identity/version are explicit.
- [ ] Manifest identity is explicit.
- [ ] provenance identity is explicit.

## Architecture

- [ ] Application boundary is documented.
- [ ] prohibited Foundation responsibilities are documented.
- [ ] MSA is defined.
- [ ] major LSAs are defined where applicable.
- [ ] CSA use is limited to eligible components.
- [ ] FSA remains outside Application.

## Manifest

- [ ] dependencies declared.
- [ ] Foundation contracts declared.
- [ ] Foundation specifications declared.
- [ ] Foundation services declared.
- [ ] capabilities declared.
- [ ] consumers declared.
- [ ] permissions declared.
- [ ] authority requests declared.
- [ ] security profile declared.
- [ ] resources declared.
- [ ] lifecycle declared.
- [ ] failure behavior declared.
- [ ] rollback declared.

## Integrity

- [ ] Manifest content is exact.
- [ ] Manifest digest is exact.
- [ ] provenance is attributable.
- [ ] provenance digest is exact.

## Providers and secrets

- [ ] provider boundary declared.
- [ ] no provider bypass exists.
- [ ] credential references are used correctly.
- [ ] secret bytes are not stored in ordinary state.

## Resources

- [ ] resource minimums are defined.
- [ ] quotas are understood.
- [ ] ceilings are respected.
- [ ] degraded mode exists.

## Capabilities

- [ ] provided capabilities valid.
- [ ] required capabilities valid.
- [ ] visibility valid.
- [ ] exclusive conflicts checked.

## Admission

- [ ] canonical Foundation references resolve.
- [ ] dependencies resolve.
- [ ] identity/owner/version match Manifest.
- [ ] admission evidence passes.

## Runtime

- [ ] runtime instance identity unique.
- [ ] artifact binding exact.
- [ ] admission binding exact.
- [ ] lifecycle attach evidence valid.
- [ ] resource grant evidence valid.
- [ ] registration result is registered-only.

## Authority

- [ ] activation separately authorized.
- [ ] deployment separately authorized where applicable.
- [ ] production separately authorized where applicable.
- [ ] business authority separately governed.
- [ ] no self-granted authority exists.

## Isolation and recovery

- [ ] Application failure is contained.
- [ ] unrelated Applications remain isolated.
- [ ] Foundation remains valid without this Application.
- [ ] recovery plan exists.
- [ ] removal plan exists.

## Live seal

- [ ] onboarding requires no Foundation modification.
- [ ] onboarding requires no Foundation special-case.
- [ ] onboarding requires no Foundation-directed FCR.
- [ ] Application can operate entirely against published Foundation contracts.

If every applicable item is YES, the human review may classify the candidate as:

```text
READY_FOR_FOUNDATION_ADMISSION
```

---

# 27. Recommended onboarding record template

Use a record similar to this:

```text
APPLICATION ONBOARDING RECORD

Application Identity:
Application Version:
Application Owner:
Application Purpose:
Package Identity:
Package Version:
Manifest Identity:
Provenance Identity:

Major Branches:
MSA:
LSAs:
CSAs:

Foundation Contracts Required:
Foundation Specifications Required:
Foundation Services Required:
Shared Application Capabilities Required:
External Providers:

Permissions Requested:
Authorities Requested:
Resource Minimums:
Resource Ceilings:
Security Profile:

Failure Mode:
Degraded Mode:
Recovery Plan:
Rollback Plan:
Removal Plan:

Manifest Verification: PASS / FAIL
Provenance Verification: PASS / FAIL
Dependency Resolution: PASS / FAIL
Security Review: PASS / FAIL
Resource Review: PASS / FAIL
Capability Review: PASS / FAIL
Admission Result: PASS / FAIL / NOT RUN
Runtime Registration Result: PASS / FAIL / NOT RUN
Activation Authority: PRESENT / ABSENT / NOT APPLICABLE

Foundation Modification Required: NO
Foundation Special Case Required: NO
Foundation FCR Required: NO

Final Human Classification:
READY_FOR_FOUNDATION_ADMISSION
or
APPLICATION_REDESIGN_REQUIRED
or
INCOMPATIBLE_WITH_SEALED_FOUNDATION

Reviewer:
Date:
Evidence References:
```

---

# 28. Final human explanation

A future Falcon Application is not "installed into Foundation" by copying code into Foundation.

It is a separately governed Application that presents a valid identity, Manifest, evidence, dependencies, capabilities, resource needs, security profile, lifecycle behavior, and authority requests to a stable Foundation substrate.

Foundation then validates and hosts it according to the same generic rules used for every other Application.

The business domain may change.

The Foundation contract does not.

The permanent human rule is:

> Design the Application to fit Falcon Foundation. Never redesign live Falcon Foundation to fit the Application.
