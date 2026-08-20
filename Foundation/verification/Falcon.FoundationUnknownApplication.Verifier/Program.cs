using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Foundation.Admission;
using Foundation.ApplicationRuntimeHosting;
using Foundation.ContractRegistry;
using Foundation.Contracts;

namespace Falcon.FoundationUnknownApplication.Verifier;

internal static class Program
{
    private const string UnknownApplication = "unknown-application-proof-7f3c9a";
    private const string UnknownOwner = "owner:unknown-application-proof";
    private static int _checks;

    private static int Main()
    {
        try
        {
            VerifyApplicationNeutralProjectionBinding();
            VerifyUnknownApplicationAdmission();
            VerifyUnknownApplicationRuntimeHosting();
            VerifyFailClosedAdmissionMatrix();
            VerifyApplicationVersionIsNotHardcoded();

            Check(_checks >= 34, $"insufficient unknown Application coverage: {_checks}");

            Console.WriteLine("FOUNDATION_UNKNOWN_APPLICATION_VERIFIER = PASS");
            Console.WriteLine($"CHECKS = {_checks}/{_checks}");
            Console.WriteLine($"UNKNOWN_APPLICATION_IDENTITY = {UnknownApplication}");
            Console.WriteLine("APPLICATION_NAME_ALLOWLIST = NOT_REQUIRED");
            Console.WriteLine("APPLICATION_VERSION_ALLOWLIST = NOT_REQUIRED");
            Console.WriteLine("MANIFEST_AND_FOUNDATION_CONTRACTS = REQUIRED");
            Console.WriteLine("ADMISSION_TO_RUNTIME_HOSTING = PROVEN");
            Console.WriteLine("TAMPERED_MANIFEST = FAIL_CLOSED");
            Console.WriteLine("INVALID_FOUNDATION_REFERENCE = FAIL_CLOSED");
            Console.WriteLine("PROVIDER_BOUNDARY_BYPASS = FAIL_CLOSED");
            Console.WriteLine("TECHNICAL_CONSUMPTION != RUNTIME_AUTHORITY");
            Console.WriteLine("RUNTIME_REGISTRATION != ACTIVATION");
            Console.WriteLine("RUNTIME_REGISTRATION != DEPLOYMENT_AUTHORITY");
            Console.WriteLine("RUNTIME_REGISTRATION != BUSINESS_AUTHORITY");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("FOUNDATION_UNKNOWN_APPLICATION_VERIFIER = FAIL");
            Console.Error.WriteLine(ex.ToString());
            return 1;
        }
    }

    private static void VerifyApplicationNeutralProjectionBinding()
    {
        var unknownRoute = PublicRuntimeProjectionProfiles.RecoveryOperationalForApplication(UnknownApplication);

        Check(
            PublicRuntimeProjectionProfiles.IsCanonicalRecoveryOperationalForApplication(unknownRoute, UnknownApplication),
            "unknown Application recovery projection rejected");
        Check(unknownRoute.RecipientScope.Value == UnknownApplication, "unknown Application recipient identity lost");
        Check(unknownRoute.RouteIdentity == PublicRuntimeProjectionProfiles.RecoveryApplicationRouteIdentity, "generic recovery route identity changed");
        Check(unknownRoute.ArtifactSha256 == PublicRuntimeProjectionProfiles.RecoveryApplicationArtifactSha256, "generic recovery artifact identity changed");
        Check(unknownRoute.TransportAuthority.Value == PublicRuntimeProjectionProfiles.ProjectionOnlyAuthority, "generic route gained authority");
        Check(unknownRoute.MessageKind == FilMessageKind.Event, "generic route is not event-only");
        Check(
            !PublicRuntimeProjectionProfiles.IsCanonicalRecoveryOperationalForApplication(unknownRoute, "different-application"),
            "recipient substitution accepted");
        Check(
            !PublicRuntimeProjectionProfiles.IsCanonicalRecoveryOperationalForFsats(unknownRoute),
            "unknown Application route falsely accepted as FSATS route");

        var fsatsRoute = PublicRuntimeProjectionProfiles.RecoveryOperationalForFsats();
        Check(PublicRuntimeProjectionProfiles.IsCanonicalRecoveryOperationalForFsats(fsatsRoute), "FCR-0082 FSATS compatibility broken");
        Check(
            PublicRuntimeProjectionProfiles.IsCanonicalRecoveryOperationalForApplication(
                fsatsRoute,
                PublicRuntimeProjectionProfiles.FsatsRecipientIdentity),
            "FSATS compatibility alias does not delegate to generic canonical profile");

        var blankRejected = false;
        try
        {
            _ = PublicRuntimeProjectionProfiles.RecoveryOperationalForApplication("   ");
        }
        catch (ArgumentException)
        {
            blankRejected = true;
        }
        Check(blankRejected, "blank Application recipient accepted");
    }

    private static void VerifyUnknownApplicationAdmission()
    {
        var control = new AdmissionControl(new CanonicalAdmissionBaselineProvider());
        var request = ValidRequest(UnknownApplication, "37.11.5", "admission:unknown:positive");

        var validation = control.Validate(request);
        Check(validation.Success, $"valid unknown Application validation failed: {validation.Message}");

        var decision = control.Evaluate(request);
        Check(decision.Decision == "ADMITTED", $"valid unknown Application was not admitted: {decision.ReasonCode}");
        Check(decision.EvidenceId.StartsWith("ADMISSION-EVIDENCE:", StringComparison.Ordinal), "admission evidence identity missing");

        var duplicate = control.Evaluate(request with { AdmissionId = "admission:unknown:duplicate" });
        Check(duplicate.Decision == "REJECTED", "duplicate unknown Application identity/version accepted");
        Check(duplicate.ReasonCode == "duplicate application or plug-in identity", "duplicate rejection reason changed");
    }

    private static void VerifyUnknownApplicationRuntimeHosting()
    {
        const string version = "37.11.5-runtime";
        var control = new AdmissionControl(new CanonicalAdmissionBaselineProvider());
        var request = ValidRequest(UnknownApplication, version, "admission:unknown:runtime");
        var admission = control.Evaluate(request);

        Check(admission.Decision == "ADMITTED", "runtime proof Application admission failed");

        var observedAt = new DateTimeOffset(2026, 8, 19, 4, 0, 0, TimeSpan.Zero);
        var route = PublicRuntimeProjectionProfiles.RecoveryOperationalForApplication(UnknownApplication);
        var exactArtifactIdentity = string.Join(
            "|",
            route.ArtifactId,
            route.ArtifactVersion,
            route.ArtifactSha256,
            route.CompatibilityIdentity);

        var registration = new RuntimeRegistrationRequest(
            "runtime:" + UnknownApplication,
            UnknownApplication,
            version,
            exactArtifactIdentity,
            new RuntimeArtifactConsumptionBinding(
                true,
                exactArtifactIdentity,
                false,
                false,
                false,
                false,
                false),
            new RuntimeAdmissionBinding(
                true,
                UnknownApplication,
                version,
                admission.EvidenceId),
            new RuntimeLifecycleEligibilityBinding(
                true,
                RuntimeLifecycleEligibilityKind.Attach,
                UnknownApplication,
                "NONE",
                version,
                "lifecycle:unknown-application:attach"),
            new[]
            {
                new RuntimeResourceGrantBinding(
                    "grant:unknown-application:cpu",
                    UnknownApplication,
                    "resource:cpu",
                    1m,
                    2m,
                    4m,
                    observedAt.AddMinutes(-5),
                    observedAt.AddHours(1),
                    observedAt.AddMinutes(-1),
                    "evidence:unknown-application:resource")
            },
            new[]
            {
                new RuntimeCapabilityDeclaration(
                    "capability:unknown-proof",
                    RuntimeCapabilityVisibility.Private,
                    false)
            },
            Array.Empty<string>(),
            observedAt);

        var host = new ApplicationRuntimeHost("foundation:unknown-application-proof-host");
        var registered = host.Register(registration);

        Check(registered.Registered, $"admitted unknown Application failed runtime registration: {registered.Reason}");
        Check(registered.ApplicationIdentity == UnknownApplication, "runtime host changed unknown Application identity");
        Check(registered.ApplicationVersion == version, "runtime host changed unknown Application version");
        Check(!registered.ActivationAuthorized, "runtime registration silently granted activation");
        Check(!registered.DeploymentAuthorized, "runtime registration silently granted deployment");
        Check(!registered.BusinessAuthorityGranted, "runtime registration silently granted business authority");

        var snapshot = host.Snapshot(observedAt.AddSeconds(1));
        Check(snapshot.ZeroApplicationStateValid, "runtime host lost zero-Application validity invariant");
        Check(!snapshot.CarriesDeploymentAuthority, "runtime host snapshot carries deployment authority");
        Check(!snapshot.CarriesBusinessAuthority, "runtime host snapshot carries business authority");
        Check(snapshot.Slots.Count == 1, "unknown Application runtime slot missing");
        Check(snapshot.Slots[0].ApplicationIdentity == UnknownApplication, "unknown Application identity missing from runtime projection");
        Check(snapshot.Slots[0].State == RuntimeSlotState.Registered, "runtime registration unexpectedly activated Application");

        var mismatchHost = new ApplicationRuntimeHost("foundation:unknown-application-negative-host");
        var mismatchedAdmission = registration with
        {
            RuntimeInstanceId = "runtime:unknown-application:mismatch",
            Admission = registration.Admission with { ApplicationIdentity = "different-application" }
        };
        var rejected = mismatchHost.Register(mismatchedAdmission);
        Check(!rejected.Registered, "runtime host accepted mismatched admission binding");
        Check(rejected.Reason == "INVALID_ADMISSION_BINDING", "runtime host mismatch rejection reason changed");
    }

    private static void VerifyFailClosedAdmissionMatrix()
    {
        var control = new AdmissionControl(new CanonicalAdmissionBaselineProvider());
        var valid = ValidRequest(UnknownApplication, "37.11.5", "admission:unknown:negative-base");

        var tamperedManifest = valid.Manifest with { ApplicationPurpose = "tampered-purpose" };
        ExpectRejected(control.Validate(valid with { Manifest = tamperedManifest }), "tampered manifest with stale digest");

        ExpectRejected(
            control.Validate(valid with { Owner = "owner:substituted" }),
            "owner substitution");

        ExpectRejected(
            control.Validate(valid with { ContractId = "CON-999" }),
            "unknown contract identity");

        ExpectRejected(
            control.Validate(valid with { ProviderBoundary = "bypass-unapproved-provider" }),
            "provider boundary bypass");

        ExpectRejected(
            control.Validate(valid with { ProvenanceDigest = new string('0', 64) }),
            "provenance digest substitution");

        ExpectRejected(
            control.Validate(valid with { AdmissionKind = "TRADING-APPLICATION" }),
            "unsupported admission kind");

        var badDependencyManifest = valid.Manifest with
        {
            DeclaredDependencies = new[]
            {
                new DependencyDeclaration("CON-999", new[] { "1.0" })
            }
        };
        ExpectRejected(
            control.Validate(valid with
            {
                Manifest = badDependencyManifest,
                ManifestDigest = badDependencyManifest.ComputeDigest()
            }),
            "unregistered Foundation dependency");

        var badSpecificationManifest = valid.Manifest with
        {
            RequiredFoundationSpecifications = new[]
            {
                new FoundationRequirement("APP-999", "1.0", "Falcon Application Authority", "APP-001")
            }
        };
        ExpectRejected(
            control.Validate(valid with
            {
                Manifest = badSpecificationManifest,
                ManifestDigest = badSpecificationManifest.ComputeDigest()
            }),
            "invalid Foundation specification reference");
    }

    private static void VerifyApplicationVersionIsNotHardcoded()
    {
        var control = new AdmissionControl(new CanonicalAdmissionBaselineProvider());
        var request = ValidRequest(
            "unknown-application-proof-second-identity",
            "999.123.456-test",
            "admission:unknown:arbitrary-version");

        var validation = control.Validate(request);
        Check(validation.Success, $"application-defined version was incorrectly hardcoded: {validation.Message}");
        var decision = control.Evaluate(request);
        Check(decision.Decision == "ADMITTED", "application-defined version rejected after successful validation");
    }

    private static AdmissionRequest ValidRequest(string applicationIdentity, string version, string admissionId)
    {
        var baseline = new CanonicalAdmissionBaselineProvider().GetCurrentBaseline();
        var applicationContract = baseline.ApplicationContract;
        var applicationBoundary = baseline.ApplicationBoundary;

        var manifest = new ApplicationManifest(
            "manifest:" + applicationIdentity + ":" + version,
            applicationIdentity,
            version,
            UnknownOwner,
            "Synthetic unknown Application used only to prove Foundation application-neutral admission semantics.",
            "package:" + applicationIdentity,
            version,
            "sha256/package-proof-input",
            new[]
            {
                new DependencyDeclaration("CON-001", new[] { "1.0" })
            },
            new[]
            {
                new FoundationRequirement(
                    applicationContract.ContractId,
                    applicationContract.Version,
                    applicationContract.Owner,
                    applicationContract.AuthoritySource)
            },
            new[]
            {
                new FoundationRequirement(
                    applicationBoundary.ContractId,
                    applicationBoundary.Version,
                    applicationBoundary.Owner,
                    applicationBoundary.AuthoritySource)
            },
            new[]
            {
                new FoundationServiceRequirement("foundation.runtime-host", "1.0", "bounded runtime hosting")
            },
            new[] { "capability:unknown-proof" },
            new[] { "consumer:unknown-proof" },
            new[]
            {
                new PermissionDeclaration("permission:proof", "scope:proof", "unknown Application admission verifier")
            },
            new[]
            {
                new Foundation.Admission.AuthorityRequest("authority:proof", "scope:proof", "request only; no implicit grant")
            },
            new SecurityProfile("profile:unknown-proof", "INTERNAL", "ISOLATED"),
            new ResourceRequirements("1", "1", "1", "1"),
            new ResourceRequirements("10", "10", "10", "10"),
            "fail closed and remain inactive",
            "bounded verifier persistence",
            "FIL-governed communication only",
            "explicit configuration only",
            "canonical evidence required",
            new LifecycleBehavior(
                "declared",
                "validated",
                "registered",
                "admitted",
                "separate-authority-required",
                "validated-update",
                "authority-required",
                "evidence-required",
                "authority-required",
                "authority-required"),
            "health:unknown-proof",
            "containment:unknown-proof",
            false,
            new[]
            {
                new MsaDeclaration("msa:" + applicationIdentity, UnknownOwner, "application-only")
            },
            Array.Empty<MajorBranchDeclaration>(),
            Array.Empty<LsaDeclaration>(),
            "CSA only when explicitly declared by the Application",
            "CSA->LSA->MSA->FSA only where applicable and separately authorized",
            "Foundation protection remains authoritative for Foundation boundaries",
            "rollback to inactive package state");

        const string provenanceContent = "synthetic unknown Application provenance proof";

        return new AdmissionRequest(
            admissionId,
            "APPLICATION",
            applicationIdentity,
            version,
            UnknownOwner,
            applicationContract.AuthoritySource,
            applicationContract.ContractId,
            applicationContract.Version,
            manifest.ManifestId,
            manifest,
            manifest.ComputeDigest(),
            "provenance:" + applicationIdentity + ":" + version,
            provenanceContent,
            Sha256(provenanceContent),
            "bootstrap:unknown-proof",
            "DEFINED",
            "provider-boundary:governed",
            "decision-seed:" + applicationIdentity + ":" + version);
    }

    private static void ExpectRejected(AdmissionValidationResult result, string scenario)
        => Check(!result.Success, scenario + " was accepted");

    private static string Sha256(string value)
        => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));

    private static void Check(bool condition, string message)
    {
        _checks++;
        if (!condition)
            throw new InvalidOperationException(message);
    }

    private sealed class CanonicalAdmissionBaselineProvider : IAdmissionBaselineProvider
    {
        public AdmissionBaselineSnapshot GetCurrentBaseline()
        {
            var canonicalRegistry = ContractRegistry.CreateCanonical();
            var effective = canonicalRegistry.Entries.ToArray();
            var applicationContract = effective.Single(entry =>
                string.Equals(entry.ContractId, "CON-023", StringComparison.Ordinal) &&
                string.Equals(entry.Version, "1.1", StringComparison.Ordinal));

            var applicationBoundary = new ContractRegistryEntry(
                "APP-001",
                "1.0",
                "Falcon Application Authority",
                "APP-001",
                "docs/application-boundary/APP-001",
                "canonical application boundary specification",
                "ACCEPTED",
                "REGISTERED");

            return new AdmissionBaselineSnapshot(
                effective,
                applicationContract,
                applicationBoundary,
                applicationContract.Owner,
                applicationContract.AuthoritySource,
                "ACCEPTED",
                "REGISTERED",
                "ACCEPTED",
                "REGISTERED");
        }
    }
}
