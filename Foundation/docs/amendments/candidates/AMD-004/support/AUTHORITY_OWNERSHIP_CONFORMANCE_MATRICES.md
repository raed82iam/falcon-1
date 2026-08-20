# Awareness Authority, Ownership, and Conformance Matrices

**Status:** Approved Supporting Architecture  
**Approval Record:** GOV-061

## 1. Ownership Matrix

| Subject | Authoritative Owner | FSA Visibility | MSA Visibility | LSA Visibility | CSA Visibility |
|---|---|---|---|---|---|
| Foundation Self Model | FSA | Full | summary when needed | technical summary | none by default |
| Foundation Technical Fitness | FSA | Full | result and constraints | result and constraints | result when applicable |
| Applications ecosystem model | MSA | minimal conformance/impact summary | Full | relevant ecosystem context | none by default |
| One Application Self Model | LSA | technical/impact summary | governed summary | Full | owned-component contribution |
| One component Self Model | eligible CSA | conformance evidence when required | cross-Application impact only | Full relevant evidence | Full |
| Application users/customers | Application | no business ownership | aggregate only when required | Full within authority | only owned need |
| Broker accounts/credentials | Application/domain owner | opaque technical reference only | aggregate impact only | Full within authority | only owned need |
| Portfolio/order/position | Application/FIN/CAP owner | opaque impact only | ecosystem summary | Full within authority | only owned specialization |
| FIL envelope | Communication Authority | Full technical integrity | permitted metadata | permitted metadata | permitted metadata |
| Application payload meaning | Application/domain owner | none | summary only | Full for parent | owned component portion |
| Guardian restriction | Guardian | reflected as authoritative input | relevant restriction | relevant restriction | relevant restriction |
| Authority decision | Authority Engine | reflected as authoritative input | relevant decision | relevant decision | relevant decision |
| FSA conformance outcome | FSA | Full | result/conditions | result/conditions | result/conditions |

## 2. Authority Matrix

| Decision | FSA | MSA | LSA | CSA | Other competent authority |
|---|---|---|---|---|---|
| Foundation Technical Fitness | Owns assessment | No | No | No | Authority Engine decides permitted action |
| Applications collective readiness | Receives summary | Owns assessment | Contributes | Contributes through LSA | Application governance accepts reliance |
| Application fitness | Receives impact only | Receives summary | Owns assessment | Contributes | Application authority decides action |
| Component fitness | Receives impact only | Cross-impact only | Governs reliance | Owns bounded assessment | Parent Application accepts |
| Falcon conformance admission | Owns conformance assessment | Supplies evidence | Supplies evidence | Supplies evidence | Architecture/Owner/Security/etc. retain separate approvals |
| Protective restriction | May request | May request | May request | May escalate | Guardian owns |
| Operational authorization | Supplies evidence | Supplies evidence | Supplies evidence | Supplies evidence | Authority Engine owns |
| Constitutional amendment | No | No | No | No | Reserved constitutional authority |
| FSA authority expansion | No self-approval | No | No | No | Project Owner/competent governance |
| Business approval | No | Coordinates only | Application scope | Component proposal only | Application/business authority |
| Financial/risk approval | No | Summary only | Domain assessment | Specialized evidence | CAP/RSK/FIN authorities |
| Deployment approval | No unless separately delegated | No | No | No | Release/deployment authority |

## 3. Conformance Matrix

| Check | FSA | Evidence Owner | Separate Acceptance Required |
|---|---|---|---|
| Vision and Constitution compatibility | verifies conformance | proposal owner + governance evidence | Project Owner/constitutional authority where reserved |
| Architecture compliance | verifies applicable evidence | Architecture authority | Architecture Board where constituted |
| Ownership boundary | verifies | owning authorities | competent governance for dispute |
| Authority boundary | verifies instrument and scope | Authority Engine/governance | competent authority |
| Security | verifies required result exists | Security Authority | Security acceptance where required |
| FIL/Service Bus integrity | verifies technical conformance | Communication Authority | operational admission authority |
| Application business correctness | prohibited | Application/domain authority | Application/business approval |
| Risk or trading correctness | prohibited | RSK/FIN/Application | competent financial/risk authority |
| Evidence completeness | verifies required case | Evidence Completeness Authority | Promotion Authority |
| Recovery and rollback | verifies required plan/evidence | Recovery/owner | Recovery and release authorities |

## 4. Escalation Matrix

| Condition | First Owner | Escalation |
|---|---|---|
| component weakness | CSA | LSA |
| Application incident | LSA | MSA; FSA if Foundation impact |
| cross-Application conflict | MSA | owning LSAs; FSA if platform impact |
| Foundation integrity failure | FSA | Guardian/Security/Authority/Recovery as applicable |
| unauthorized change | FSA or detecting authority | Security, Guardian, governance |
| business-risk breach | LSA/domain authority | MSA, RSK, Guardian where mandate applies |
| awareness contradiction | tier owning the assertion | next awareness tier and independent challenge |
