using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Foundation.Admission;
using Foundation.ApplicationLifecycle;
using Foundation.ArtifactPublication;
using Foundation.Contracts.ResourceGovernance;
using Foundation.State.ResourceGovernance;

namespace Falcon.Stage15.ApplicationRuntimeHosting.Verifier;

internal static class Program
{
    private static readonly DateTimeOffset Now = new(2026, 8, 17, 3, 30, 0, TimeSpan.Zero);
    private static int _checks;

    private static int Main()
    {
        try
        {
            VerifyZeroApplicationState();
            VerifyTwoApplicationCoexistenceAndIsolation();
            VerifyRegistrationFailClosedCases();
            VerifyActivationAuthorityFailClosedCases();
            VerifyRemovalBackToZero();
            VerifyPredecessorBindingAdapters();
            VerifyNoLaterStageSurfaceLeakage();

            Check(_checks >= 90, $"insufficient Stage 15 coverage: {_checks}");

            Console.WriteLine("STAGE15_APPLICATION_RUNTIME_HOSTING_VERIFIER = PASS");
            Console.WriteLine($"CHECKS = {_checks}/{_checks}");
            Console.WriteLine("WP01_RUNTIME_HOST_IDENTITY_ZERO_APPLICATION = PASS");
            Console.WriteLine("WP02_EXACT_PREREQUISITE_BINDING = PASS");
            Console.WriteLine("WP03_RUNTIME_REGISTRATION = PASS");
            Console.WriteLine("WP04_SEPARATE_ACTIVATION_AUTHORITY = PASS");
            Console.WriteLine("WP05_CAPABILITY_ISOLATION = PASS");
            Console.WriteLine("WP06_SUSPENSION_ISOLATION = PASS");
            Console.WriteLine("WP07_REMOVAL_BACK_TO_ZERO = PASS");
            Console.WriteLine("WP08_FAILURE_CONTAINMENT_COEXISTENCE = PASS");
            Console.WriteLine("WP09_INTEGRATED_HARDENING = PASS");
            Console.WriteLine("ZERO_APPLICATION_OPERATION = VALID");
            Console.WriteLine("APPLICATION_PRESENCE != FOUNDATION_PREREQUISITE");
            Console.WriteLine("ADMISSION != ACTIVATION");
            Console.WriteLine("ARTIFACT_CONSUMPTION != ACTIVATION");
            Console.WriteLine("RESOURCE_GRANT != ACTIVATION");
            Console.WriteLine("REGISTERED != ACTIVE");
            Console.WriteLine("ACTIVATION != BUSINESS_AUTHORITY");
            Console.WriteLine("APPLICATION_FAILURE != FOUNDATION_FAILURE");
            Console.WriteLine("APPLICATION_PRIVATE_CAPABILITY != CROSS_APPLICATION_ACCESS");
            Console.WriteLine("STAGE15 != ENVIRONMENT_REALIZATION");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("STAGE15_APPLICATION_RUNTIME_HOSTING_VERIFIER = FAIL");
            Console.Error.WriteLine(ex.ToString());
            return 1;
        }
    }

    private static void VerifyZeroApplicationState()
    {
        var host = new ApplicationRuntimeHost("foundation-runtime-host");
        var first = host.Snapshot(Now);
        var second = host.Snapshot(Now);

        Check(first.ZeroApplicationStateValid, "zero-Application state not valid");
        Check(first.Slots.Count == 0, "new host not empty");
        Check(first.ProjectionIdentity == second.ProjectionIdentity, "zero-Application projection not deterministic");
        Check(first.HostIdentity == "foundation-runtime-host", "host identity mismatch");
        Check(!first.CarriesDeploymentAuthority, "host projection carries deployment authority");
        Check(!first.CarriesBusinessAuthority, "host projection carries business authority");
        Check(first.ProjectionIdentity.StartsWith("sha256/", StringComparison.Ordinal), "host projection identity not SHA-256");
    }

    private static void VerifyTwoApplicationCoexistenceAndIsolation()
    {
        var host = new ApplicationRuntimeHost("foundation-runtime-host-coexistence");
        var appA = Registration(
            "runtime-a",
            "appa",
            "1.0.0",
            new[]
            {
                new RuntimeCapabilityDeclaration("cap-shared-a", RuntimeCapabilityVisibility.SharedDeclared, false),
                new RuntimeCapabilityDeclaration("cap-private-a", RuntimeCapabilityVisibility.Private, true)
            },
            Array.Empty<string>());
        var appB = Registration(
            "runtime-b",
            "appb",
            "2.0.0",
            new[]
            {
                new RuntimeCapabilityDeclaration("cap-shared-b", RuntimeCapabilityVisibility.SharedDeclared, false),
                new RuntimeCapabilityDeclaration("cap-private-b", RuntimeCapabilityVisibility.Private, true)
            },
            new[] { "cap-shared-a" });

        var regA = host.Register(appA);
        var regB = host.Register(appB);

        Check(regA.Registered, "Application A registration failed");
        Check(regB.Registered, "Application B registration failed");
        Check(regA.Reason == "RUNTIME_REGISTERED_NOT_ACTIVATED", "registration A reason incorrect");
        Check(regB.Reason == "RUNTIME_REGISTERED_NOT_ACTIVATED", "registration B reason incorrect");
        Check(!regA.ActivationAuthorized && !regB.ActivationAuthorized, "registration granted activation");
        Check(!regA.DeploymentAuthorized && !regB.DeploymentAuthorized, "registration granted deployment");
        Check(!regA.BusinessAuthorityGranted && !regB.BusinessAuthorityGranted, "registration granted business authority");

        var registered = host.Snapshot(Now);
        Check(registered.Slots.Count == 2, "two Applications did not coexist");
        Check(registered.Slots.All(slot => slot.State == RuntimeSlotState.Registered), "registration silently activated an Application");

        var actA = host.Activate("runtime-a", Authority(RuntimeAuthorityAction.Activate, "runtime-a", "appa", "1.0.0"), Now);
        var actB = host.Activate("runtime-b", Authority(RuntimeAuthorityAction.Activate, "runtime-b", "appb", "2.0.0"), Now);

        Check(actA.Accepted && actA.ResultingState == RuntimeSlotState.Active, "Application A activation failed");
        Check(actB.Accepted && actB.ResultingState == RuntimeSlotState.Active, "Application B activation failed");
        Check(!actA.DeploymentAuthorized && !actB.DeploymentAuthorized, "activation granted deployment");
        Check(!actA.BusinessAuthorityGranted && !actB.BusinessAuthorityGranted, "activation granted business authority");

        var shared = host.ResolveCapability("runtime-b", "cap-shared-a");
        Check(shared.Available, "declared shared capability unavailable");
        Check(shared.ProviderRuntimeInstanceId == "runtime-a", "shared capability wrong runtime provider");
        Check(shared.ProviderApplicationIdentity == "appa", "shared capability wrong Application provider");

        var privateCross = host.ResolveCapability("runtime-b", "cap-private-a");
        Check(!privateCross.Available, "cross-Application private capability access allowed");
        Check(privateCross.Reason == "CAPABILITY_ACCESS_DENIED", "private capability denial reason incorrect");

        var privateSelf = host.ResolveCapability("runtime-a", "cap-private-a");
        Check(privateSelf.Available, "private capability unavailable to owner");
        Check(privateSelf.Reason == "PRIVATE_SELF_CAPABILITY", "private self capability reason incorrect");

        var undeclaredShared = host.ResolveCapability("runtime-a", "cap-shared-b");
        Check(!undeclaredShared.Available, "undeclared shared capability access allowed");

        var isolateA = host.Isolate("runtime-a", Authority(RuntimeAuthorityAction.Isolate, "runtime-a", "appa", "1.0.0"), Now);
        Check(isolateA.Accepted && isolateA.ResultingState == RuntimeSlotState.Isolated, "Application A isolation failed");

        var unavailable = host.ResolveCapability("runtime-b", "cap-shared-a");
        Check(!unavailable.Available, "isolated provider remained available");
        Check(unavailable.Reason == "CAPABILITY_UNAVAILABLE", "isolated capability reason incorrect");

        var snapshot = host.Snapshot(Now);
        var slotA = snapshot.Slots.Single(slot => slot.RuntimeInstanceId == "runtime-a");
        var slotB = snapshot.Slots.Single(slot => slot.RuntimeInstanceId == "runtime-b");
        Check(slotA.State == RuntimeSlotState.Isolated, "Application A isolation state missing");
        Check(slotB.State == RuntimeSlotState.Active, "Application B affected by Application A isolation");
        Check(snapshot.Slots.Count == 2, "Application failure/isolation collapsed host");
    }

    private static void VerifyRegistrationFailClosedCases()
    {
        var host = new ApplicationRuntimeHost("registration-attacks");
        var valid = Registration(
            "runtime-main",
            "appmain",
            "1.0.0",
            new[] { new RuntimeCapabilityDeclaration("cap-main", RuntimeCapabilityVisibility.Private, true) },
            Array.Empty<string>());

        Check(!host.Register(valid with
        {
            ArtifactConsumption = valid.ArtifactConsumption with { AcceptedForTechnicalConsumption = false }
        }).Registered, "rejected Stage 14 artifact accepted");

        Check(!host.Register(valid with
        {
            ArtifactConsumption = valid.ArtifactConsumption with { ActivationAuthorized = true }
        }).Registered, "Stage 14 artifact carrying activation authority accepted");

        Check(!host.Register(valid with
        {
            ExpectedArtifactExactIdentity = "sha256/" + new string('B', 64)
        }).Registered, "wrong exact artifact identity accepted");

        Check(!host.Register(valid with
        {
            Admission = valid.Admission with { Admitted = false }
        }).Registered, "rejected admission accepted");

        Check(!host.Register(valid with
        {
            Admission = valid.Admission with { ApplicationIdentity = "otherapp" }
        }).Registered, "wrong admission Application identity accepted");

        Check(!host.Register(valid with
        {
            Admission = valid.Admission with { ApplicationVersion = "9.9.9" }
        }).Registered, "wrong admission version accepted");

        Check(!host.Register(valid with
        {
            LifecycleEligibility = valid.LifecycleEligibility with { ApplicationIdentity = "otherapp" }
        }).Registered, "wrong lifecycle subject accepted");

        Check(!host.Register(valid with
        {
            LifecycleEligibility = valid.LifecycleEligibility with { Eligible = false }
        }).Registered, "ineligible lifecycle attach accepted");

        Check(!host.Register(valid with
        {
            LifecycleEligibility = valid.LifecycleEligibility with { Kind = RuntimeLifecycleEligibilityKind.DetachOrRemove }
        }).Registered, "removal lifecycle decision used for registration");

        Check(!host.Register(valid with { ResourceGrants = Array.Empty<RuntimeResourceGrantBinding>() }).Registered,
            "registration without Stage 6 resource grants accepted");

        var wrongResource = GrantBinding(Grant("otherapp", "memory", "grantwrong", Now.AddMinutes(-5), Now.AddHours(1)));
        Check(!host.Register(valid with { ResourceGrants = new[] { wrongResource } }).Registered,
            "resource grant for another Application accepted");

        var expired = GrantBinding(Grant("appmain", "memory", "grantexpired", Now.AddHours(-2), Now.AddHours(-1)));
        Check(!host.Register(valid with { ResourceGrants = new[] { expired } }).Registered,
            "expired resource grant accepted");

        var future = GrantBinding(Grant("appmain", "memory", "grantfuture", Now.AddMinutes(-1), Now.AddHours(1), Now.AddMinutes(5)));
        Check(!host.Register(valid with { ResourceGrants = new[] { future } }).Registered,
            "future resource evidence accepted");

        var invalidLimits = valid.ResourceGrants[0] with { Allocation = 40m, Quota = 20m, Ceiling = 30m };
        Check(!host.Register(valid with { ResourceGrants = new[] { invalidLimits } }).Registered,
            "invalid resource grant limits accepted");

        var duplicateCaps = valid with
        {
            ProvidedCapabilities = new[]
            {
                new RuntimeCapabilityDeclaration("cap-main", RuntimeCapabilityVisibility.Private, true),
                new RuntimeCapabilityDeclaration("cap-main", RuntimeCapabilityVisibility.Private, true)
            }
        };
        Check(!host.Register(duplicateCaps).Registered, "duplicate capability declaration accepted");

        var duplicateRequired = valid with { RequiredCapabilities = new[] { "cap-x", "cap-x" } };
        Check(!host.Register(duplicateRequired).Registered, "duplicate required capability accepted");

        var accepted = host.Register(valid);
        Check(accepted.Registered, "valid registration failed after attacks");
        Check(!host.Register(valid).Registered, "duplicate runtime instance accepted");

        var alias = Registration(
            "runtime-alias",
            "appmain",
            "1.0.0",
            new[] { new RuntimeCapabilityDeclaration("cap-alias", RuntimeCapabilityVisibility.Private, true) },
            Array.Empty<string>());
        Check(!host.Register(alias).Registered, "same Application hosted twice through runtime alias");

        var exclusiveConflict = Registration(
            "runtime-other",
            "appother",
            "1.0.0",
            new[] { new RuntimeCapabilityDeclaration("cap-main", RuntimeCapabilityVisibility.Private, true) },
            Array.Empty<string>());
        Check(!host.Register(exclusiveConflict).Registered, "duplicate exclusive capability ownership accepted");
    }

    private static void VerifyActivationAuthorityFailClosedCases()
    {
        var host = new ApplicationRuntimeHost("activation-attacks");
        var request = Registration(
            "runtime-auth",
            "appauth",
            "1.0.0",
            new[] { new RuntimeCapabilityDeclaration("cap-auth", RuntimeCapabilityVisibility.Private, true) },
            Array.Empty<string>());
        Check(host.Register(request).Registered, "activation attack setup failed");

        var valid = Authority(RuntimeAuthorityAction.Activate, "runtime-auth", "appauth", "1.0.0");

        Check(!host.Activate("runtime-auth", valid with { Status = RuntimeAuthorityStatus.Revoked }, Now).Accepted,
            "revoked activation authority accepted");
        Check(!host.Activate("runtime-auth", valid with { Status = RuntimeAuthorityStatus.Ambiguous }, Now).Accepted,
            "ambiguous activation authority accepted");
        Check(!host.Activate("runtime-auth", valid with { Action = RuntimeAuthorityAction.Suspend }, Now).Accepted,
            "wrong action authority accepted");
        Check(!host.Activate("runtime-auth", valid with { RuntimeInstanceId = "runtime-other" }, Now).Accepted,
            "runtime identity substitution accepted");
        Check(!host.Activate("runtime-auth", valid with { ApplicationIdentity = "otherapp" }, Now).Accepted,
            "Application identity substitution accepted");
        Check(!host.Activate("runtime-auth", valid with { ApplicationVersion = "2.0.0" }, Now).Accepted,
            "Application version substitution accepted");
        Check(!host.Activate("runtime-auth", valid with { EffectiveUntil = Now.AddSeconds(-1) }, Now).Accepted,
            "expired activation authority accepted");
        Check(!host.Activate("runtime-auth", valid with { EffectiveFrom = Now.AddSeconds(1) }, Now).Accepted,
            "future activation authority accepted");
        Check(!host.Activate("runtime-auth", valid with { EvidenceIdentity = "" }, Now).Accepted,
            "activation authority without evidence accepted");

        var activation = host.Activate("runtime-auth", valid, Now);
        Check(activation.Accepted, "valid activation rejected");
        Check(activation.ResultingState == RuntimeSlotState.Active, "activation did not produce ACTIVE");
        Check(!activation.DeploymentAuthorized, "activation granted deployment");
        Check(!activation.BusinessAuthorityGranted, "activation granted business authority");
        Check(!host.Activate("runtime-auth", valid, Now).Accepted, "double activation accepted");

        var invalidSuspend = Authority(RuntimeAuthorityAction.Suspend, "runtime-auth", "appauth", "1.0.0") with
        {
            Status = RuntimeAuthorityStatus.Stale
        };
        Check(!host.Suspend("runtime-auth", invalidSuspend, Now).Accepted, "stale suspend authority accepted");

        var suspend = host.Suspend("runtime-auth", Authority(RuntimeAuthorityAction.Suspend, "runtime-auth", "appauth", "1.0.0"), Now);
        Check(suspend.Accepted && suspend.ResultingState == RuntimeSlotState.Suspended, "valid suspend failed");
        Check(!host.ResolveCapability("runtime-auth", "cap-auth").Available, "suspended runtime capability remained available");

        var isolate = host.Isolate("runtime-auth", Authority(RuntimeAuthorityAction.Isolate, "runtime-auth", "appauth", "1.0.0"), Now);
        Check(isolate.Accepted && isolate.ResultingState == RuntimeSlotState.Isolated, "isolation from suspended state failed");
        Check(!host.Isolate("runtime-auth", Authority(RuntimeAuthorityAction.Isolate, "runtime-auth", "appauth", "1.0.0"), Now).Accepted,
            "double isolation accepted");
    }

    private static void VerifyRemovalBackToZero()
    {
        var host = new ApplicationRuntimeHost("removal-host");
        var request = Registration(
            "runtime-remove",
            "appremove",
            "3.0.0",
            new[] { new RuntimeCapabilityDeclaration("cap-remove", RuntimeCapabilityVisibility.Private, true) },
            Array.Empty<string>());

        Check(host.Register(request).Registered, "removal setup registration failed");
        Check(host.Activate("runtime-remove", Authority(RuntimeAuthorityAction.Activate, "runtime-remove", "appremove", "3.0.0"), Now).Accepted,
            "removal setup activation failed");

        var validRemoval = LifecycleRemovalBinding("appremove", "3.0.0");
        var wrongRemoval = validRemoval with { ApplicationIdentity = "otherapp" };

        Check(!host.Remove(
            "runtime-remove",
            Authority(RuntimeAuthorityAction.Remove, "runtime-remove", "appremove", "3.0.0"),
            wrongRemoval,
            Now).Accepted,
            "wrong lifecycle removal subject accepted");

        Check(!host.Remove(
            "runtime-remove",
            Authority(RuntimeAuthorityAction.Remove, "runtime-remove", "appremove", "3.0.0") with { Action = RuntimeAuthorityAction.Activate },
            validRemoval,
            Now).Accepted,
            "wrong removal authority action accepted");

        var removed = host.Remove(
            "runtime-remove",
            Authority(RuntimeAuthorityAction.Remove, "runtime-remove", "appremove", "3.0.0"),
            validRemoval,
            Now);

        Check(removed.Accepted && removed.ResultingState == RuntimeSlotState.Removed, "valid removal failed");
        Check(!removed.DeploymentAuthorized, "removal granted deployment authority");
        Check(!removed.BusinessAuthorityGranted, "removal granted business authority");

        var snapshot = host.Snapshot(Now);
        Check(snapshot.Slots.Count == 0, "removal did not return host to zero Applications");
        Check(snapshot.ZeroApplicationStateValid, "post-removal zero state invalid");
        Check(!host.ResolveCapability("runtime-remove", "cap-remove").Available, "removed runtime capability remained available");
        Check(!host.Remove(
            "runtime-remove",
            Authority(RuntimeAuthorityAction.Remove, "runtime-remove", "appremove", "3.0.0"),
            validRemoval,
            Now).Accepted,
            "second removal accepted");
    }

    private static void VerifyPredecessorBindingAdapters()
    {
        var artifactDecision = new ArtifactConsumptionDecision(
            true,
            "EXACT_ARTIFACT_CONSUMPTION_ACCEPTED",
            "sha256/" + new string('C', 64),
            false,
            false,
            false,
            false,
            false);
        var artifactBinding = ArtifactBinding(artifactDecision);
        Check(artifactBinding.AcceptedForTechnicalConsumption, "Stage 14 adapter lost accepted consumption");
        Check(!artifactBinding.ActivationAuthorized, "Stage 14 adapter invented activation authority");
        Check(!artifactBinding.DeploymentAuthorized, "Stage 14 adapter invented deployment authority");
        Check(!artifactBinding.BusinessAuthorityGranted, "Stage 14 adapter invented business authority");

        var admissionDecision = new AdmissionDecision(
            "admission-adapter",
            "ADMITTED",
            "admission accepted",
            "CON-023",
            "1.0",
            "evidence-admission-adapter");
        var admissionBinding = AdmissionBinding(admissionDecision, "appadapter", "4.0.0");
        Check(admissionBinding.Admitted, "Admission adapter lost admitted state");
        Check(admissionBinding.ApplicationIdentity == "appadapter", "Admission adapter identity mismatch");
        Check(admissionBinding.ApplicationVersion == "4.0.0", "Admission adapter version mismatch");
        Check(admissionBinding.EvidenceIdentity == admissionDecision.EvidenceId, "Admission adapter evidence mismatch");

        var lifecycleDecision = LifecycleAttachDecision("appadapter", "4.0.0");
        var lifecycleBinding = LifecycleAttachBinding(lifecycleDecision);
        Check(lifecycleBinding.Eligible, "Lifecycle adapter lost eligibility");
        Check(lifecycleBinding.Kind == RuntimeLifecycleEligibilityKind.Attach, "Lifecycle adapter kind mismatch");
        Check(lifecycleBinding.ApplicationIdentity == "appadapter", "Lifecycle adapter subject mismatch");
        Check(lifecycleBinding.TargetVersion == "4.0.0", "Lifecycle adapter target version mismatch");

        var grant = Grant("appadapter", "memory", "grantadapter", Now.AddMinutes(-2), Now.AddHours(1));
        var grantBinding = GrantBinding(grant);
        Check(grantBinding.ApplicationIdentity == "appadapter", "Stage 6 adapter Application identity mismatch");
        Check(grantBinding.GrantIdentity == "grantadapter", "Stage 6 adapter grant identity mismatch");
        Check(grantBinding.ResourceClassIdentity == "memory", "Stage 6 adapter resource class mismatch");
        Check(grantBinding.Allocation == 10m && grantBinding.Quota == 20m && grantBinding.Ceiling == 30m,
            "Stage 6 adapter limits mismatch");
        Check(grantBinding.EvidenceIdentity == "evidence-grantadapter", "Stage 6 adapter evidence mismatch");
    }

    private static void VerifyNoLaterStageSurfaceLeakage()
    {
        var assembly = typeof(ApplicationRuntimeHost).Assembly;
        var exported = assembly.GetExportedTypes();
        var stage15Names = exported
            .Where(type => type.Name.StartsWith("Runtime", StringComparison.Ordinal) || type.Name == nameof(ApplicationRuntimeHost) || type.Name == nameof(CapabilityResolutionDecision))
            .Select(type => type.FullName ?? type.Name)
            .ToArray();

        Check(!stage15Names.Any(name => name.Contains("Windows", StringComparison.OrdinalIgnoreCase)), "Windows realization leaked into Stage 15");
        Check(!stage15Names.Any(name => name.Contains("Linux", StringComparison.OrdinalIgnoreCase)), "Linux realization leaked into Stage 15");
        Check(!stage15Names.Any(name => name.Contains("Container", StringComparison.OrdinalIgnoreCase)), "container realization leaked into Stage 15");
        Check(!stage15Names.Any(name => name.Contains("ProcessHost", StringComparison.OrdinalIgnoreCase)), "process realization leaked into Stage 15");
        Check(!stage15Names.Any(name => name.Contains("Deployment", StringComparison.OrdinalIgnoreCase)), "deployment surface leaked into Stage 15");
        Check(!stage15Names.Any(name => name.Contains("Oidc", StringComparison.OrdinalIgnoreCase)), "OIDC semantics leaked into Stage 15");
        Check(!stage15Names.Any(name => name.Contains("Mfa", StringComparison.OrdinalIgnoreCase)), "MFA semantics leaked into Stage 15");
        Check(!stage15Names.Any(name => name.Contains("Trading", StringComparison.OrdinalIgnoreCase)), "trading semantics leaked into Stage 15");
        Check(!stage15Names.Any(name => name.Contains("Broker", StringComparison.OrdinalIgnoreCase)), "broker semantics leaked into Stage 15");
        Check(!stage15Names.Any(name => name.Contains("Strategy", StringComparison.OrdinalIgnoreCase)), "strategy semantics leaked into Stage 15");

        var methods = typeof(ApplicationRuntimeHost).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        Check(!methods.Any(method => method.Name.Contains("Deploy", StringComparison.OrdinalIgnoreCase)), "deployment method leaked into Stage 15");
        Check(!methods.Any(method => method.Name.Contains("Connect", StringComparison.OrdinalIgnoreCase)), "connectivity method leaked into Stage 15");
        Check(!methods.Any(method => method.Name.Contains("Authenticate", StringComparison.OrdinalIgnoreCase)), "authenticator method leaked into Stage 15");
    }

    private static RuntimeRegistrationRequest Registration(
        string runtimeId,
        string appId,
        string version,
        IReadOnlyList<RuntimeCapabilityDeclaration> provided,
        IReadOnlyList<string> required)
    {
        var artifactDecision = new ArtifactConsumptionDecision(
            true,
            "EXACT_ARTIFACT_CONSUMPTION_ACCEPTED",
            "sha256/" + new string('A', 64),
            false,
            false,
            false,
            false,
            false);
        var admissionDecision = new AdmissionDecision(
            "admission-" + appId,
            "ADMITTED",
            "admission accepted",
            "CON-023",
            "1.0",
            "evidence-admission-" + appId);
        var lifecycleDecision = LifecycleAttachDecision(appId, version);
        var grant = Grant(appId, "memory", "grant-" + appId, Now.AddMinutes(-10), Now.AddHours(2));

        return new RuntimeRegistrationRequest(
            runtimeId,
            appId,
            version,
            artifactDecision.ExactArtifactIdentity,
            ArtifactBinding(artifactDecision),
            AdmissionBinding(admissionDecision, appId, version),
            LifecycleAttachBinding(lifecycleDecision),
            new[] { GrantBinding(grant) },
            provided,
            required,
            Now);
    }

    private static RuntimeArtifactConsumptionBinding ArtifactBinding(ArtifactConsumptionDecision decision)
        => new(
            decision.AcceptedForTechnicalConsumption,
            decision.ExactArtifactIdentity,
            decision.ActivationAuthorized,
            decision.DeploymentAuthorized,
            decision.ProductionAuthorized,
            decision.BusinessAuthorityGranted,
            decision.SilentUpgradePerformed);

    private static RuntimeAdmissionBinding AdmissionBinding(AdmissionDecision decision, string appId, string version)
        => new(
            string.Equals(decision.Decision, "ADMITTED", StringComparison.Ordinal),
            appId,
            version,
            decision.EvidenceId);

    private static RuntimeLifecycleEligibilityBinding LifecycleAttachBinding(LifecycleDecision decision)
        => new(
            decision.Kind == LifecycleDecisionKind.Allowed && decision.Transition == LifecycleTransitionKind.Attach,
            RuntimeLifecycleEligibilityKind.Attach,
            decision.SubjectIdentity,
            decision.CurrentVersion,
            decision.TargetVersion,
            decision.DecisionIdentity);

    private static RuntimeLifecycleEligibilityBinding LifecycleRemovalBinding(string appId, string version)
    {
        var decision = LifecycleDetachDecision(appId, version);
        return new RuntimeLifecycleEligibilityBinding(
            decision.Kind == LifecycleDecisionKind.Allowed && decision.Transition == LifecycleTransitionKind.DetachOrRemove,
            RuntimeLifecycleEligibilityKind.DetachOrRemove,
            decision.SubjectIdentity,
            decision.CurrentVersion,
            decision.TargetVersion,
            decision.DecisionIdentity);
    }

    private static RuntimeResourceGrantBinding GrantBinding(ApplicationResourceAllocation grant)
        => new(
            grant.GrantId.Value,
            grant.ApplicationId.Value,
            grant.ResourceClassId.Value,
            grant.Allocation.Amount,
            grant.Quota.Amount,
            grant.Ceiling.Amount,
            grant.Lifetime.EffectiveFrom,
            grant.Lifetime.EffectiveUntil,
            grant.Evidence.ObservedAt,
            grant.Evidence.EvidenceId.Value);

    private static ApplicationResourceAllocation Grant(
        string appId,
        string resourceClass,
        string grantId,
        DateTimeOffset effectiveFrom,
        DateTimeOffset effectiveUntil,
        DateTimeOffset? evidenceObservedAt = null)
        => new(
            new ResourceGrantId(grantId),
            new ApplicationPrincipalId(appId),
            new ResourceClassId(resourceClass),
            new ResourceQuantity(10m, "unit"),
            new ResourceQuantity(20m, "unit"),
            new ResourceQuantity(30m, "unit"),
            new ResourceEffectiveLifetime(effectiveFrom, effectiveUntil, false),
            new ResourceEvidenceReference(
                new ResourceEvidenceId("evidence-" + grantId),
                new ResourceScopeId("scope-" + appId),
                evidenceObservedAt ?? effectiveFrom,
                new ResourceEpochId("epoch-stage15")));

    private static RuntimeAuthorityEvidence Authority(RuntimeAuthorityAction action, string runtimeId, string appId, string version)
        => new(
            "authority-" + action.ToString().ToLowerInvariant() + "-" + runtimeId,
            RuntimeAuthorityStatus.Valid,
            action,
            runtimeId,
            appId,
            version,
            Now.AddMinutes(-5),
            Now.AddMinutes(30),
            "evidence-authority-" + action.ToString().ToLowerInvariant() + "-" + runtimeId);

    private static LifecycleDecision LifecycleAttachDecision(string appId, string version)
        => LifecycleDecisionFor(appId, LifecycleTransitionKind.Attach, LifecycleState.Detached, string.Empty, version);

    private static LifecycleDecision LifecycleDetachDecision(string appId, string version)
        => LifecycleDecisionFor(appId, LifecycleTransitionKind.DetachOrRemove, LifecycleState.Attached, version, string.Empty);

    private static LifecycleDecision LifecycleDecisionFor(
        string appId,
        LifecycleTransitionKind transition,
        LifecycleState currentState,
        string currentVersion,
        string targetVersion)
    {
        var authority = new LifecycleAuthorityEvidence(
            "lifecycle-authority-" + appId + "-" + transition,
            LifecycleEvidenceStatus.Valid,
            appId,
            transition,
            currentVersion,
            targetVersion);
        var valid = new LifecycleEvidence("lifecycle-evidence-" + appId + "-" + transition, LifecycleEvidenceStatus.Valid);
        var continuity = new LifecycleContinuityEvidence(
            "continuity-" + appId + "-" + transition,
            LifecycleEvidenceStatus.Valid,
            true,
            true,
            true,
            true,
            true);
        var drain = new LifecycleDrainEvidence(false, false, string.Empty, LifecycleEvidenceStatus.Valid);
        var request = new LifecycleRequest(
            "lifecycle-request-" + appId + "-" + transition,
            appId,
            transition,
            currentState,
            currentVersion,
            targetVersion,
            authority,
            valid,
            valid,
            valid,
            valid,
            continuity,
            drain,
            null,
            "correlation-" + appId,
            "causation-" + appId);

        return new ApplicationLifecycleEvaluator().Evaluate(request);
    }

    private static void Check(bool condition, string message)
    {
        _checks++;
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
}
