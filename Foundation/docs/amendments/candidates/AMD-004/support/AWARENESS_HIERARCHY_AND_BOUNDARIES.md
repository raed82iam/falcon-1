# Awareness Hierarchy and Boundary Diagrams

**Status:** Approved Supporting Architecture  
**Approval Record:** GOV-061  
**Authority:** Proposed ADR-I009

## 1. Awareness Hierarchy

```mermaid
flowchart TD
    FSA["FSA<br/>Falcon Foundation awareness<br/>Falcon conformance gate"]
    MSA["MSA<br/>Falcon Applications ecosystem awareness"]
    LSA["LSA<br/>One Application or Approved Operating Layer"]
    CSA["CSA<br/>One eligible intelligent component"]

    CSA -->|"bounded assessment and escalation"| LSA
    LSA -->|"governed Application summary"| MSA
    MSA -->|"abstract ecosystem and Foundation-impact summary"| FSA
    FSA -->|"conformance conditions and technical constraints"| MSA
```

The arrows do not transfer ownership or authority.

## 2. Foundation/Application Boundary

```mermaid
flowchart LR
    subgraph Foundation["Falcon Foundation"]
        FSA["FSA"]
        Kernel["Kernel"]
        FIL["FIL"]
        Bus["Service Bus"]
        Security["Security"]
        Authority["Authority Engine"]
        Guardian["Guardian"]
        Runtime["Runtime / Lifecycle"]
        Persistence["Foundation Persistence"]
    end

    subgraph Applications["Falcon Applications Environment"]
        MSA["MSA"]
        subgraph AppA["Application A"]
            LSA1["LSA"]
            CSA1["CSA"]
            Business1["Application-owned users and business state"]
        end
        subgraph AppB["Application B"]
            LSA2["LSA"]
            Business2["Application-owned users and business state"]
        end
    end

    LSA1 -->|"minimal summary"| MSA
    LSA2 -->|"minimal summary"| MSA
    MSA -->|"technical impact / conformance evidence"| FSA
    FSA -->|"technical condition"| MSA
    CSA1 --> LSA1
    FIL <-->|"generic governed envelopes"| Applications
    Bus <-->|"transport without business interpretation"| Applications
    FSA -.->|"no business-data ownership"| Business1
    FSA -.->|"no business-data ownership"| Business2
```

## 3. Change-Conformance Flow

```mermaid
flowchart TD
    Proposal["CSA / LSA / MSA / Foundation change proposal"]
    Owner["Owning authority assessment"]
    Impact["Cross-tier impact and evidence"]
    FSA["FSA Falcon conformance review"]
    Other["Required Security / Architecture / Risk / Owner reviews"]
    Admission["Separately authorized admission"]
    Reject["Reject / request evidence / correction"]

    Proposal --> Owner
    Owner --> Impact
    Impact --> FSA
    FSA -->|"conforming or conditional"| Other
    FSA -->|"non-conforming or incomplete"| Reject
    Other -->|"all required approvals present"| Admission
```

FSA is the final self-awareness and Falcon conformance gate. It is not the final authority for every approval class.

## 4. Governing Boundary Statements

- CSA understands the component.
- LSA understands the Application.
- MSA understands the Falcon Applications ecosystem.
- FSA protects Falcon itself and assesses conformance for admission.
- Higher awareness rank does not acquire lower-tier ownership.
- Foundation protects Application infrastructure without interpreting Application business.
- FIL carries meaning; the owning Application owns that meaning.
- Guardian protects; Authority Engine authorizes; FSA assesses awareness and conformance.

## 5. Bounded Self-Repair Flow

```mermaid
flowchart TD
    Detect["FSA detects Foundation failure"]
    Classify{"Approved Repair Playbook<br/>and trusted target state?"}
    Contain["Contain / isolate / preserve evidence"]
    Authorize["Verify repair authority and preconditions"]
    Repair["Restore previously Approved trusted state"]
    Verify["Post-repair verification"]
    Result{"Verified?"}
    Return["Gradual return with recorded result"]
    Escalate["Restrict, escalate, or open evolution investigation"]

    Detect --> Classify
    Classify -->|"No"| Contain --> Escalate
    Classify -->|"Yes"| Authorize --> Repair --> Verify --> Result
    Result -->|"Yes"| Return
    Result -->|"No"| Escalate
```

## 6. Controlled Self-Evolution Flow

```mermaid
flowchart TD
    Evidence["Sustained weakness evidence"]
    Insufficient["Self-Repair shown insufficient"]
    Candidate["FSA creates distinct isolated candidate"]
    Sandbox["Sandbox validation"]
    City{"Digital City required?"}
    Digital["Digital City validation"]
    Independent["Independent validation"]
    Package["Immutable Owner Approval Package"]
    Owner{"Explicit Owner decision"}
    Deploy["Separately authorized staged deployment"]
    Post["Post-adoption verification"]
    Trusted["Authorized trusted baseline"]
    Reject["Reject / defer / request changes or evidence"]
    Rollback["Restore last Approved trusted state"]

    Evidence --> Insufficient --> Candidate --> Sandbox --> City
    City -->|"Yes"| Digital --> Independent
    City -->|"No, documented"| Independent
    Independent --> Package --> Owner
    Owner -->|"Approved scope"| Deploy --> Post
    Owner -->|"Not approved"| Reject
    Post -->|"All conditions pass"| Trusted
    Post -->|"Approved rollback condition"| Rollback
```

> FSA may build the proposed replacement. FSA may not appoint the proposed replacement.

## 7. Guardian Readiness Relationship

```mermaid
flowchart LR
    Evidence["Independent readiness evidence"] --> FSA["FSA technical readiness supervision"]
    Guardian["Guardian self-report"] --> FSA
    FSA -->|"degraded/unknown"| Authority["Authority Engine restrictions"]
    FSA -->|"repair request under Approved playbook"| Repair["Guardian technical repair"]
    Repair --> Verify["Independent readiness verification"]
    Guardian -->|"authoritative protective restriction"| Enforcement["Enforcement and Lifecycle"]
    FSA -.->|"cannot change mandate or release restriction"| Guardian
```
