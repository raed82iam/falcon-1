# دليل المهندس والمطور لنظام Falcon Self-Aware Trading System (FSATS)

**الإصدار:** 2026-08-19  
**اللغة:** العربية  
**الفئة المستهدفة:** مهندسو البرمجيات، المعماريون، المراجعون، maintainers، مهندسو الاختبار والتكامل، فرق التشغيل، وأي AI coding agent يعمل على FSATS  
**الفرع:** `application-development`  
**نطاق الكتابة العادي لـFSATS Application:** `applications/**` فقط  
**الوضع الحالي:** Parts 0 through 10 مقبولة ومغلقة من Project Owner. Part 11 onboarding preparation منفذة ومتحقق منها تقنيًا. Foundation أعلنت `FULL_PLUG_READY_PREFLIGHT = VERIFIED_BY_COMPOSITION`، بينما Admission الفعلي، Canonical Runtime Registration، Activation، Provider/Broker connectivity، Paper/Shadow/Tiny-Live/Live، Deployment وBusiness Authority ما تزال محكومة بشكل منفصل وغير مفوضة بمجرد نجاح التحضير.

> هذا الدليل مرجع هندسي مساعد لـFSATS. لا يستبدل Falcon Vision، Falcon Constitution، `applications/FSATS/WORKSTREAM_RULES.md`، APP-001، CON-023، ADR-I012، ADR-I015، FDN-006، FDN-007، قرارات Owner الحالية، FCR headers الحالية، سجلات Parts المعتمدة، الكود، الاختبارات أو exact executable evidence. المرجع الحاكم الأعلى هو المرجع عند التعارض.

---

# 1. قاعدة العمل الأساسية

كل شغل FSATS يتبع:

```text
SOURCE
-> AUTHORITY
-> COMPARE
-> DECIDE
-> CHANGE
```

لا تبدأ من ذاكرة محادثة أو حالة قديمة إذا كان GitHub الحالي متاحًا.

قبل كل رد أو دورة عمل FSATS جوهرية:

1. اعمل fresh broad FCR check؛
2. افحص أي Issue body حالية فيها `Waiting On: APPLICATION` مع أحدث comments ذات الصلة؛
3. اعمل fresh `application-development` HEAD؛
4. اقرأ workstream rules والمراجع الحاكمة المباشرة للنطاق؛
5. ثبت الصلاحية الدقيقة قبل أي تنفيذ.

---

# 2. حدود Repository والملكية

الكتابة العادية لـFSATS تكون داخل:

```text
application-development
applications/**
```

ممنوع الكتابة على:

```text
foundation-development
web-development
main
reference/fsats-v1.3-scratch
applications/shared/web/**
applications/FSATS/WORKSTREAM_RULES.md
```

إلا إذا أعطى Project Owner صلاحية صريحة منفصلة.

مسؤوليات Foundation تبقى read-only من جهة Application workstream. Shared Web مملوكة لـWeb workstream بشكل مستقل.

```text
APPLICATION MUST NOT PATCH FOUNDATION TO FIT FSATS
ORDINARY APPLICATION MUST NOT PATCH SHARED WEB
```

---

# 3. معمارية FSATS الحالية

FSATS هو non-owning/non-runtime system boundary مكوّن من خمس Falcon Applications مستقلة:

```text
FSATS-TRADING
FSATS-FSAPMA
FSATS-TRADING-GUARDIAN
FSATS-FSTSIMA
APP-RSC
```

الـAwareness topology الحالية:

```text
Trading:          MSA=1 / LSA=13 / CSA=3
FSAPMA:           MSA=1 / LSA=6  / CSA=1
Trading Guardian: MSA=1 / LSA=4  / CSA=1
FSTSimA:          MSA=1 / LSA=8  / CSA=2
APP-RSC:          MSA=1 / LSA=3  / CSA=0 initially
TOTAL: 5 Applications / 5 MSA / 34 LSA / 7 CSA
```

حد الـAwareness المعماري:

```text
CSA -> LSA -> MSA -> FSA review where applicable
```

FSA تبقى Foundation-owned. MSA/LSA/CSA تبقى Application-owned.

---

# 4. مسؤوليات كل Application

## 4.1 Trading

تملك Trading-domain intelligence وBroker-account-scoped business workflow، ومنها عادةً:

- market interpretation؛
- strategy selection/orchestration؛
- opportunity evaluation؛
- trading-domain decision logic؛
- broker-account-scoped execution preparation؛
- trading evidence/state؛
- trading-domain recovery/reconciliation semantics.

Trading لا تملك Foundation admission أو Foundation activation أو Foundation resource governance أو Foundation FSA أو Shared Web identity mapping.

## 4.2 FSAPMA

تملك operational provider management الخاص بـFSATS:

- provider capabilities؛
- provider selection/suitability؛
- quota/rate-limit awareness؛
- route readiness؛
- provider failure/degradation؛
- operational market-data coordination.

Provider connectivity لا تمنح Trading Authority.

## 4.3 Trading Guardian

تملك bounded trading-domain protection وcontainment semantics، لكنها لا تملك Foundation Guardian أو Global AI Kill أو Broker Authority أو Strategy logic.

## 4.4 FSTSimA

تملك governed non-Live simulation وDigital City validation، ومنها deterministic scenarios، replay، calibration، fault injection، evidence وqualification outputs.

Simulation output ليست operational truth ولا تمنح Paper/Live authority.

## 4.5 APP-RSC

تملك FSATS-side resource coordination فقط، ويجب أن تحافظ على Foundation كمصدر authoritative للـresource governance والـcurrent grants.

```text
APP-RSC != FOUNDATION_RESOURCE_GOVERNANCE
```

---

# 5. Broker Account Identity Model

الـTrading operating subject الحاكم هو Broker Account.

```text
FSATS_USER_ID = NONE
FSATS_CUSTOMER_ID = NONE
TRADING_OPERATING_SUBJECT = BROKER_ACCOUNT
BROKER_ACCOUNT_IDENTITY = BrokerId + BrokerAccountId
ENVIRONMENT = ADDITIONAL DIMENSION WHERE MATERIAL
```

Shared Web تملك broker-account-to-customer/user/contact mapping.

ممنوع اختراع FSATS-owned customer identity shortcut يكسر هذا النموذج.

---

# 6. فواصل Authority الحاكمة

هذه Invariants معمارية وليست تفضيلات:

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
LIFECYCLE_TRANSITION != NEW_AUTHORITY_DECISION
READY_FOR_RELEASE_DECISION != RELEASE
RELEASE_AUTHORIZATION != RELEASE_EXECUTION
RESTART != RECOVERY
ROUTE_EXISTS != CONNECTION_AUTHORIZED
ROUTE_AUTHORIZED != CONNECTION_EXECUTED
PROVIDER_CONNECTIVITY != EXECUTION_AUTHORITY
DATA_ACCESS != BUSINESS_AUTHORITY
REQUEST_SENT != ACTION_ACCEPTED != ACTION_COMPLETED
CREDENTIAL_REFERENCE_ID != SECRET_BYTES
SELF_AWARENESS != AUTHORITY
```

أي Design أو Code path يدمج واحدًا من هذه الفواصل يحتاج مراجعة فورية.

---

# 7. حالة الـParts الحالية

الحالة الوثائقية المعتمدة:

```text
PART 0  = OWNER_ACCEPTED_AND_CLOSED
PART 1  = OWNER_ACCEPTED_AND_CLOSED
PART 2  = OWNER_ACCEPTED_AND_CLOSED
PART 3  = OWNER_ACCEPTED_AND_CLOSED
PART 4  = OWNER_ACCEPTED_AND_CLOSED
PART 5  = OWNER_ACCEPTED_AND_CLOSED
PART 6  = OWNER_ACCEPTED_AND_CLOSED
PART 7  = OWNER_ACCEPTED_AND_CLOSED
PART 8  = OWNER_ACCEPTED_AND_CLOSED
PART 9  = OWNER_ACCEPTED_AND_CLOSED
PART 10 = OWNER_ACCEPTED_AND_CLOSED
```

Part 11 هي Owner-authorized Runtime Onboarding / Admission & Binding preparation scope مستقلة. تنفيذ Application-side وExact Request Materialization متحقق منه تقنيًا، لكن لا يجوز وصف Part 11 بأنها Owner-accepted-and-closed إلا إذا أعطى Owner هذا القرار صراحة.

---

# 8. معمارية Part 11 Onboarding

المسار العام:

```text
APPLICATION DECLARATION
-> FOUNDATION VALIDATION
-> ADMISSION DECISION
-> CANONICAL ARTIFACT / LIFECYCLE / RESOURCE BINDING
-> RUNTIME REGISTRATION
-> SEPARATE ACTIVATION AUTHORITY
-> ACTIVE ONLY WHEN ALL APPLICABLE GATES PASS
```

كل Application من الخمس لديها Foundation onboarding declaration من جهة Application مربوطة بـManifest identity الحالية وبـAwareness topology.

المراجع المطلوبة:

```text
CON-023 = 1.1
APP-001 = 1.0
BootstrapContextState = DEFINED
```

كل Declaration تشترط:

```text
ExactArtifactIdentityRequired = true
PositiveAdmissionEvidenceRequired = true
LifecycleAttachEligibilityRequired = true
CurrentFoundationResourceGrantRequired = true
RuntimeRegistrationMayAuthorizeActivation = false
RuntimeRegistrationMayAuthorizeDeployment = false
RuntimeRegistrationMayAuthorizeProduction = false
RuntimeRegistrationMayGrantBusinessAuthority = false
SilentUpgradeAllowed = false
ExternalConnectivityActivated = false
PaperAuthorityGranted = false
LiveAuthorityGranted = false
```

---

# 9. FCR-0254 Exact Request Materialization

Application workstream جهزت بالضبط:

```text
5 AdmissionRequest candidates
5 RuntimeRegistrationRequest templates
5 request pairs
```

المسارات الأساسية:

```text
applications/FSATS/tests/FoundationCompatibility/
  Falcon.FSATS.FoundationOnboarding.Verifier/
    Fcr0254CandidateCatalog.cs
    Fcr0254CandidateCatalogVerifier.cs
```

والـ5 Foundation onboarding declarations موجودة داخل Projects الخاصة بكل Application.

الحزمة تتعمد ترك runtime-current facts بدون تزوير إلى لحظة التشغيل المصرح به.

Bind-at-operation inputs:

```text
EXACT_STAGE14_ARTIFACT_IDENTITY
POSITIVE_CANONICAL_ADMISSION_EVIDENCE
LIFECYCLE_ATTACH_ELIGIBILITY_AND_DECISION_IDENTITY
CURRENT_FOUNDATION_RESOURCE_GRANTS
AUTHORITATIVE_OBSERVED_AT
```

ممنوع اختلاق هذه القيم للراحة أو لاجتياز Test.

---

# 10. حالة Foundation Handoff الحالية

Foundation أكدت أن generic admission/runtime-hosting capability موجودة وأن حزمة التحضير الحالية plug-ready by composition.

الحكم الحالي:

```text
FOUNDATION_GENERIC_ADMISSION_RUNTIME_PATH   = EXECUTABLE_PROVEN
APPLICATION_EXACT_REQUEST_MATERIALIZATION   = EXECUTABLE_VERIFIED
FOUNDATION_EXACT_STATIC_GATE_RECONCILIATION = PASS_5_OF_5
FULL_PLUG_READY_CONTRACT_PREFLIGHT          = VERIFIED
FULL_PLUG_READY_PREFLIGHT                   = VERIFIED_BY_COMPOSITION
FOUNDATION_CHANGE_REQUIRED                  = FALSE
APPLICATION_REDESIGN_REQUIRED               = FALSE
```

لكن Actual Operation ما زالت Held:

```text
ACTUAL_ADMISSION                      = NOT_AUTHORIZED / NOT_EXECUTED
ACTUAL_CANONICAL_RUNTIME_REGISTRATION = NOT_AUTHORIZED / NOT_EXECUTED
RUNTIME_ACTIVATION                    = NOT_AUTHORIZED / NOT_EXECUTED
DEPLOYMENT                            = NOT_AUTHORIZED / NOT_EXECUTED
PROVIDER_BROKER_CONNECTIVITY          = NOT_AUTHORIZED / NOT_EXECUTED
PAPER_LIVE_BUSINESS_AUTHORITY         = NOT_AUTHORIZED / NOT_EXECUTED
```

ممنوع تحويل Readiness إلى Runtime State داخل Code أو Docs أو Tests أو Status Messages.

---

# 11. Host Egress Boundary

Host projects يجب أن تحافظ على disabled egress حتى يوجد تفويض وربط فعلي محكوم.

المبدأ الحالي:

```text
Trading Host -> DisabledBrokerExecutionPort until governed broker execution binding
FSAPMA Host  -> DisabledProviderEgressPort until governed provider egress binding
APP-RSC Host -> DisabledFoundationResourcePort until governed Foundation resource binding
```

ممنوع استبدال Disabled ports بـLive network implementation فقط لأن Provider/Broker code موجود.

```text
TECHNICAL_COMPATIBILITY != RUNTIME_BINDING_AUTHORITY
```

---

# 12. قاعدة التكامل مع Foundation

Foundation Application-neutral ومبنية على Published Contracts.

القاعدة الدائمة:

```text
APPLICATION MUST FIT FOUNDATION
FOUNDATION MUST NOT CHANGE TO FIT APPLICATION
```

إذا FSATS لا تتوافق مع Published Foundation Contract:

1. تحقق من العقد والكود الحالي؛
2. حدد هل الحل Application-side adaptation؛
3. استخدم Application-side adapter فقط إذا حافظ على Foundation semantics؛
4. أعد تصميم/احذف Unsupported behavior إذا لزم؛
5. لا تنشئ Fake Foundation service محلي؛
6. لا تعدل Foundation من `application-development`.

---

# 13. Manifest Discipline

كل Application Manifest هي Authority-critical declaration وليست README.

حافظ على exact alignment بين:

- Application identity؛
- version؛
- owner؛
- package identity/version؛
- Manifest identity؛
- provenance؛
- dependencies؛
- contracts/specifications/services؛
- permissions؛
- authority requests؛
- provider boundary؛
- resources؛
- lifecycle؛
- awareness topology.

أي Manifest semantic change قد يبطل Evidence قديمة.

```text
SEMANTIC_MANIFEST_CHANGE -> FRESH REVIEW REQUIRED
```

---

# 14. قواعد Awareness Engineering

MSA وLSA وCSA تبقى داخل حدودها.

```text
ONE MSA PER CURRENT MAJOR APPLICATION
LSA OWNERSHIP = MAJOR BRANCH
CSA = ELIGIBLE INTELLIGENT COMPONENT ONLY
FSA = FOUNDATION ONLY
```

ممنوع تحويل MSA إلى Foundation control plane ثاني.

ممنوع CSA توافق على Authority expansion لنفسها.

Self-improvement proposals تبقى محكومة بالـsandbox/evidence/governance وOwner authority عند الحاجة.

---

# 15. Strategy Architecture

الاستراتيجيات تدار مركزيًا داخل Trading Application بدل تكرارها مرة لكل سوق.

يوجد Strategy Controller وStrategy Self-Awareness لترشيح وتهيئة الاستراتيجيات مقابل Market-specific properties وconstraints.

Market Models تصف مثلًا:

- required data؛
- indicators؛
- timeframes؛
- liquidity rules؛
- execution constraints؛
- market restrictions؛
- strategy suitability.

ممنوع نسخ نفس Strategy implementation لكل Market فقط لتغيير توافقها.

---

# 16. Provider Architecture

FSAPMA هي operational FSATS data gateway. Provider-specific logic يجب أن يبقى خلف حدود FSAPMA.

حافظ على:

```text
WEB_PROVIDER_ROUTE != FSAPMA_PROVIDER_ROUTE
WEB_PRESENTATION_DATA != FSATS_OPERATIONAL_DATA
PUBLIC_PROVIDER_ENDPOINT != UNRESTRICTED_EGRESS_AUTHORITY
```

Provider Controller يمكن أن يفهم capabilities/quota/selection، لكنه لا يخلق صلاحية اتصال أو تداول.

---

# 17. Simulation Architecture

FSTSimA يجب أن تبقى non-Live إلا بتفويض منفصل.

حافظ على Determinism عند الحاجة:

- exact scenario identity؛
- exact seed؛
- exact fault ordering؛
- exact digest/evidence binding؛
- reproducibility assessment؛
- calibration gates؛
- explicit non-operational classification.

ممنوع تحويل Simulation output إلى Live Truth بدون Contract محكوم يسمح بذلك صراحة.

---

# 18. Resource Architecture

APP-RSC تنسق Resource demand داخل FSATS لكنها تستهلك Foundation resource truth ولا تخترعها.

Invariants:

```text
REQUESTED_RESOURCE != GRANTED_RESOURCE
RESOURCE_PROJECTION != RESOURCE_AUTHORITY
0 <= Allocation <= Quota <= Ceiling
```

ممنوع استخدام Grant خاصة بـApplication كأنها Grant لـApplication أخرى.

Current runtime resource evidence يجب أن تأتي من Foundation عند لحظة التشغيل المصرح بها.

---

# 19. Security وSecret Handling

ممنوع تخزين Secret Bytes داخل ordinary Application state.

استخدم governed credential references.

حافظ على:

```text
CREDENTIAL_REFERENCE_ID != SECRET_BYTES
CREDENTIAL_REFERENCE != CREDENTIAL_AUTHORITY
AUTHENTICATION != AUTHORIZATION
```

Lexical security scanning هي Defense-in-depth وليست Proof أن كل Egress مستحيل. Runtime route governance وArchitecture fences وDependency review وIntegration verification تبقى مطلوبة.

---

# 20. Failure وRecovery

Failure handling يجب أن يبقى bounded، deterministic، وقابل للتفسير.

ممنوع Shortcuts مثل:

```text
RESTART => RECOVERED
REPAIR_SUCCESS => RELEASED
```

Recovery وRelease مفاهيم منفصلة.

المسار المفاهيمي:

```text
fault/containment
-> assessment
-> governed recovery plan
-> restoration
-> reconciliation
-> validation
-> ready-for-release-decision
-> separate release authorization
-> separate release execution
-> observation
```

---

# 21. Update Discipline

Material Semantic Update تبطل PASS القديمة للنطاق المتغير.

المسار المطلوب:

```text
Semantic Change
-> Fresh Architecture / Consistency Review
-> Fresh Red Team
-> Owner Review
```

إذا Red Team remediation غيّرت semantics مرة أخرى، تعيد الدورة.

ممنوع تقديم PASS قديمة كEvidence current بعد تغير bytes/semantics.

---

# 22. Validation Model

Governed Application validation suite في Part 11 تشمل 10 Verifiers:

- Architecture؛
- Security؛
- Behavior؛
- Operational Data Outcome؛
- Owner Update Governance؛
- Foundation Binding؛
- Owner Feature Entitlement؛
- Foundation Onboarding / FCR-0254 Materialization؛
- Integration؛
- Failure.

Exact Part 11 evidence الحالية أثبتت على الـtested candidate:

```text
Architecture = PASS
Security = PASS
Behavior = PASS 40/40
Operational Data Outcome = PASS 16/16
Owner Update Governance = PASS 44/44
Foundation Binding = PASS 67/67
Owner Feature Entitlement = PASS 44/44
FCR-0254 Materialization = PASS 129/129
Foundation Onboarding = PASS 27/27
Integration = PASS 31/31
Failure = PASS 12/12
Application verifiers = PASS 10/10 twice
Cross-branch onboarding = PASS 20/20
FAILED_CHECKS = 0
```

Technical PASS لا تصنع Owner Acceptance أو Runtime Authority.

---

# 23. Red-Team Checklist

عند أي Semantic Change اسأل:

```text
هل missing identity تستطيع تمر؟
هل version/digest/provenance substitution تمر؟
هل stale evidence تصبح current؟
هل unknown تصبح authorized؟
هل Broker Account تأخذ State من Broker Account ثانية؟
هل Application تستخدم resource grant لـApplication أخرى؟
هل Registration تتحول إلى Activation؟
هل Provider route تتحول إلى Execution Authority؟
هل Web presentation data تصبح FSATS operational truth؟
هل Simulation تصبح Live truth؟
هل Guardian protection تصبح Strategy Authority؟
هل APP-RSC تصبح Foundation resource authority؟
هل MSA/LSA/CSA تتعدى إلى FSA scope؟
هل Secret Bytes تدخل ordinary state؟
هل Component توافق على Authority expansion لنفسها؟
هل Restart تتجاوز Recovery؟
هل Successful Test تتحول إلى Owner Approval؟
```

أي Forbidden Path يجب أن يبقى مستحيلًا أو Fail Closed.

---

# 24. FCR Protocol للمهندس

GitHub Issue body هي Canonical Current State لكل FCR. Comments هي Audit History.

Permitted `Waiting On` values:

```text
FOUNDATION
APPLICATION
WEB
NONE
```

`Waiting On: OWNER` ممنوع.

إذا FCR الحالية `Waiting On: APPLICATION`، افحص Body الحالية وأحدث Comments قبل الشغل المعتمد عليها.

ممنوع إغلاق Foundation-owned implementation claim من Application بدون Foundation evidence.

FCR Status لا تنشئ Runtime Authority.

---

# 25. Documentation Discipline

Historical Records لا يعاد كتابتها فقط لتبدو Repository نظيفة.

استخدم:

```text
Historical Record
+ Later Controlling Correction / Amendment / Supersession
```

عندما يلزم تصحيح semantics مع حفظ التاريخ.

Documentation-only commit لا يجوز وصفها كأن Executable Suite اشتغلت على نفس Commit إذا لم يحدث ذلك.

---

# 26. Practical Change Checklist

قبل الكتابة:

- fresh FCR check؛
- fresh HEAD؛
- read current governing sources؛
- exact scope/authority؛
- owning Application؛
- Foundation dependency verification؛
- latest Architecture/Red-Team evidence.

أثناء التنفيذ:

- ابق داخل Owning Paths؛
- حافظ على Identity/Authority boundaries؛
- أضف أو حدّث Tests للـsemantic behavior؛
- Fail Closed عند Unknown authority-critical state؛
- لا تنقل State ownership سرًا بين Applications.

بعد التنفيذ:

- inspect diff؛
- restore/build/test؛
- applicable governed verifiers؛
- deterministic rerun عند الحاجة؛
- clean working tree؛
- fresh Architecture/Consistency؛
- fresh Red Team؛
- report exact tested commit/evidence؛
- لا تبالغ في Owner Acceptance أو Activation أو Deployment أو Live Authority.

---

# 27. من أين تبدأ في Repository؟

Primary navigation:

```text
applications/README.md
applications/FSATS/README.md
applications/FSATS/WORKSTREAM_RULES.md
applications/docs/FSATS/
applications/FSATS/src/
applications/FSATS/tests/
applications/ci/
```

Part 11 control document:

```text
applications/docs/FSATS/PART_11/00_PART11_RUNTIME_ONBOARDING_AUTHORIZATION_AND_SCOPE.md
```

Current request materialization verifier:

```text
applications/FSATS/tests/FoundationCompatibility/
Falcon.FSATS.FoundationOnboarding.Verifier/
```

---

# 28. القاعدة الهندسية النهائية

عندما تقرر أين ينتمي Change، اسأل:

```text
من يملك Truth؟
من يملك Authority؟
من يملك Execution؟
من يملك Presentation؟
أي Exact Identity تتأثر؟
ما هي Evidence التي تعبر Boundary؟
ما الذي يجب أن يبقى مستحيلًا؟
هل حافظنا على حدود Applications الخمس؟
هل حافظنا على Foundation neutrality؟
هل حافظنا على Fail-Closed behavior؟
```

إذا الإجابات غير صريحة، التصميم غير جاهز للتنفيذ.

**Architecture first. Exact identity. Explicit authority. Evidence-bound decisions. Fail closed. No hidden authority transfer.**