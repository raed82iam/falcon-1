using System.Security.Cryptography;
using System.Text;
using T = Falcon.FSATS.Trading.Application;
using P = Falcon.FSATS.FSAPMA.Application;
using G = Falcon.FSATS.TradingGuardian.Application;
using S = Falcon.FSATS.FSTSimA.Application;
using R = Falcon.FSATS.ResourceManagement.Application;

internal sealed record FoundationDependencyCandidate(string Identity, IReadOnlyList<string> CompatibleVersions);
internal sealed record FoundationRequirementCandidate(string Identity, string Version, string Owner, string AuthoritySource);
internal sealed record FoundationServiceRequirementCandidate(string Identity, string Version, string Purpose);
internal sealed record PermissionCandidate(string Name, string Scope, string Rationale);
internal sealed record AuthorityRequestCandidate(string Name, string Scope, string Rationale);
internal sealed record SecurityProfileCandidate(string Name, string Classification, string IsolationModel);
internal sealed record ResourceRequirementsCandidate(string Memory, string Cpu, string Storage, string Network);
internal sealed record LifecycleBehaviorCandidate(
    string Installation,
    string Validation,
    string Registration,
    string Admission,
    string Activation,
    string Update,
    string Suspension,
    string Recovery,
    string Replacement,
    string Removal);
internal sealed record MsaDeclarationCandidate(string Identity, string Owner, string Scope);
internal sealed record MajorBranchDeclarationCandidate(string BranchName, string Purpose, string ResponsibleLsa);
internal sealed record LsaDeclarationCandidate(string BranchName, string ResponsibleLsa, string Scope);

internal sealed record FoundationApplicationManifestCandidate(
    string ManifestId,
    string ApplicationIdentity,
    string ApplicationVersion,
    string ApplicationOwner,
    string ApplicationPurpose,
    string PackageIdentity,
    string PackageVersion,
    string PackageContentOrIntegrityInput,
    IReadOnlyList<FoundationDependencyCandidate> DeclaredDependencies,
    IReadOnlyList<FoundationRequirementCandidate> RequiredFoundationContracts,
    IReadOnlyList<FoundationRequirementCandidate> RequiredFoundationSpecifications,
    IReadOnlyList<FoundationServiceRequirementCandidate> RequiredFoundationServices,
    IReadOnlyList<string> ProvidedCapabilities,
    IReadOnlyList<string> IntendedConsumers,
    IReadOnlyList<PermissionCandidate> RequestedPermissions,
    IReadOnlyList<AuthorityRequestCandidate> AuthorityRequests,
    SecurityProfileCandidate SecurityProfile,
    ResourceRequirementsCandidate MinimumResourceRequirements,
    ResourceRequirementsCandidate ResourceCeilings,
    string DegradedBehavior,
    string PersistenceRequirements,
    string CommunicationRequirements,
    string ConfigurationRequirements,
    string EvidenceRequirements,
    LifecycleBehaviorCandidate LifecycleBehavior,
    string HealthReportingInterface,
    string FailureContainmentInterface,
    bool UsesBranchBasedInternalArchitecture,
    IReadOnlyList<MsaDeclarationCandidate> MsaDeclarations,
    IReadOnlyList<MajorBranchDeclarationCandidate> MajorBranchDeclarations,
    IReadOnlyList<LsaDeclarationCandidate> LsaDeclarations,
    string CsaEligibilityPolicy,
    string SelfDevelopmentOriginAndEscalationPath,
    string GuardianAndProtectionInterface,
    string RollbackOrCorrectiveActionPlan)
{
    public string ComputeFoundationCanonicalDigest() => FoundationCandidateCanonicalizer.ComputeDigest(this);
}

internal sealed record ExactAdmissionRequestCandidate(
    string AdmissionId,
    string AdmissionKind,
    string Identity,
    string Version,
    string Owner,
    string AuthoritySource,
    string ContractId,
    string ContractVersion,
    string ManifestId,
    FoundationApplicationManifestCandidate Manifest,
    string ManifestDigest,
    string ProvenanceId,
    string ProvenanceContent,
    string ProvenanceDigest,
    string BootstrapContextId,
    string BootstrapContextState,
    string ProviderBoundary,
    string DecisionSeed);

internal sealed record BindAtExecution(string Field, string AuthoritativeSource, string Reason);
internal sealed record RuntimeArtifactConsumptionTemplate(
    BindAtExecution AcceptedForTechnicalConsumption,
    BindAtExecution ExactArtifactIdentity,
    bool ActivationAuthorized,
    bool DeploymentAuthorized,
    bool ProductionAuthorized,
    bool BusinessAuthorityGranted,
    bool SilentUpgradePerformed);
internal sealed record RuntimeAdmissionTemplate(
    BindAtExecution Admitted,
    string ApplicationIdentity,
    string ApplicationVersion,
    BindAtExecution EvidenceIdentity);
internal sealed record RuntimeLifecycleEligibilityTemplate(
    BindAtExecution Eligible,
    string Kind,
    string ApplicationIdentity,
    BindAtExecution CurrentVersion,
    string TargetVersion,
    BindAtExecution DecisionIdentity);
internal sealed record RuntimeResourceGrantTemplate(BindAtExecution CurrentFoundationResourceGrants);
internal sealed record RuntimeCapabilityDeclarationTemplate(string CapabilityId, string Visibility, bool Exclusive);
internal sealed record RuntimeRegistrationRequestTemplate(
    string RuntimeInstanceId,
    string ApplicationIdentity,
    string ApplicationVersion,
    BindAtExecution ExpectedArtifactExactIdentity,
    RuntimeArtifactConsumptionTemplate ArtifactConsumption,
    RuntimeAdmissionTemplate Admission,
    RuntimeLifecycleEligibilityTemplate LifecycleEligibility,
    IReadOnlyList<RuntimeResourceGrantTemplate> ResourceGrants,
    IReadOnlyList<RuntimeCapabilityDeclarationTemplate> ProvidedCapabilities,
    IReadOnlyList<string> RequiredCapabilities,
    BindAtExecution ObservedAt,
    bool ExecutesRegistration,
    bool GrantsActivation,
    bool GrantsDeployment,
    bool GrantsProduction,
    bool GrantsBusinessAuthority);

internal sealed record Fcr0254ApplicationRequestPair(
    ExactAdmissionRequestCandidate AdmissionRequest,
    RuntimeRegistrationRequestTemplate RuntimeRegistrationRequest);

internal sealed record NormalizedApplicationSource(
    string ApplicationId,
    string PackageId,
    string Version,
    string Owner,
    string Purpose,
    string ManifestId,
    string ProviderBoundary,
    IReadOnlyList<string> RequiredFoundationServices,
    string MsaId,
    IReadOnlyList<string> LsaIds,
    string CsaEligibilityPolicy,
    IReadOnlyList<string> ProvidedCapabilities,
    IReadOnlyList<string> IntendedConsumers,
    IReadOnlyList<string> Permissions,
    IReadOnlyList<string> AuthorityRequests,
    IReadOnlyList<string> DeclaredDependencies,
    string SecurityProfile,
    string ResourcePolicy,
    string PersistencePolicy,
    string CommunicationPolicy,
    string ConfigurationPolicy,
    string EvidencePolicy,
    string HealthPolicy,
    string FailureContainmentPolicy,
    string SelfDevelopmentPolicy,
    string GuardianRequirement,
    string ProtectionInterface,
    string RollbackPlan);

internal static class Fcr0254CandidateCatalog
{
    internal const string ReviewedPart11ApplicationHead = "fcbfb38825e42cd0c191646045815c45858e7bd4";
    internal const string ReviewedFoundationHead = "15e6d66ec0d571f1e803f56444acc90c84885312";
    internal const string ApplicationContractId = "CON-023";
    internal const string ApplicationContractVersion = "1.1";
    internal const string ApplicationContractOwner = "Falcon Application Authority";
    internal const string ApplicationContractAuthoritySource = "CON-000 / CON-023";
    internal const string ApplicationSpecificationId = "APP-001";
    internal const string ApplicationSpecificationVersion = "1.0";
    internal const string ApplicationSpecificationOwner = "Falcon Application Authority";
    internal const string ApplicationSpecificationAuthoritySource = "APP-001";

    internal static IReadOnlyList<Fcr0254ApplicationRequestPair> All { get; } = BuildAll();

    private static IReadOnlyList<Fcr0254ApplicationRequestPair> BuildAll()
    {
        var sources = new[]
        {
            new NormalizedApplicationSource(
                T.TradingManifest.Current.ApplicationId, T.TradingManifest.Current.PackageId, T.TradingManifest.Current.Version,
                T.TradingManifest.Current.Owner, T.TradingManifest.Current.Purpose, T.TradingFoundationOnboarding.Current.ManifestId,
                T.TradingFoundationOnboarding.Current.ProviderBoundary, T.TradingFoundationOnboarding.Current.RequiredFoundationServices,
                T.TradingManifest.Current.MsaId, T.TradingManifest.Current.LsaIds, T.TradingManifest.Current.CsaEligibilityPolicy,
                T.TradingManifest.Current.ProvidedCapabilities, T.TradingManifest.Current.DeclaredConsumers, T.TradingManifest.Current.Permissions,
                T.TradingManifest.Current.AuthorityRequests, T.TradingManifest.Current.DeclaredDependencies, T.TradingManifest.Current.SecurityProfile,
                T.TradingManifest.Current.ResourcePolicy, T.TradingManifest.Current.PersistencePolicy, T.TradingManifest.Current.CommunicationPolicy,
                T.TradingManifest.Current.ConfigurationPolicy, T.TradingManifest.Current.EvidencePolicy, T.TradingManifest.Current.HealthPolicy,
                T.TradingManifest.Current.FailureContainmentPolicy, T.TradingManifest.Current.SelfDevelopmentPolicy,
                T.TradingManifest.Current.GuardianRequirement, T.TradingManifest.Current.ProtectionInterface, T.TradingManifest.Current.RollbackPlan),
            new NormalizedApplicationSource(
                P.FSAPMAManifest.Current.ApplicationId, P.FSAPMAManifest.Current.PackageId, P.FSAPMAManifest.Current.Version,
                P.FSAPMAManifest.Current.Owner, P.FSAPMAManifest.Current.Purpose, P.FSAPMAFoundationOnboarding.Current.ManifestId,
                P.FSAPMAFoundationOnboarding.Current.ProviderBoundary, P.FSAPMAFoundationOnboarding.Current.RequiredFoundationServices,
                P.FSAPMAManifest.Current.MsaId, P.FSAPMAManifest.Current.LsaIds, P.FSAPMAManifest.Current.CsaEligibilityPolicy,
                P.FSAPMAManifest.Current.ProvidedCapabilities, P.FSAPMAManifest.Current.DeclaredConsumers, P.FSAPMAManifest.Current.Permissions,
                P.FSAPMAManifest.Current.AuthorityRequests, P.FSAPMAManifest.Current.DeclaredDependencies, P.FSAPMAManifest.Current.SecurityProfile,
                P.FSAPMAManifest.Current.ResourcePolicy, P.FSAPMAManifest.Current.PersistencePolicy, P.FSAPMAManifest.Current.CommunicationPolicy,
                P.FSAPMAManifest.Current.ConfigurationPolicy, P.FSAPMAManifest.Current.EvidencePolicy, P.FSAPMAManifest.Current.HealthPolicy,
                P.FSAPMAManifest.Current.FailureContainmentPolicy, P.FSAPMAManifest.Current.SelfDevelopmentPolicy,
                P.FSAPMAManifest.Current.GuardianRequirement, P.FSAPMAManifest.Current.ProtectionInterface, P.FSAPMAManifest.Current.RollbackPlan),
            new NormalizedApplicationSource(
                G.TradingGuardianManifest.Current.ApplicationId, G.TradingGuardianManifest.Current.PackageId, G.TradingGuardianManifest.Current.Version,
                G.TradingGuardianManifest.Current.Owner, G.TradingGuardianManifest.Current.Purpose, G.TradingGuardianFoundationOnboarding.Current.ManifestId,
                G.TradingGuardianFoundationOnboarding.Current.ProviderBoundary, G.TradingGuardianFoundationOnboarding.Current.RequiredFoundationServices,
                G.TradingGuardianManifest.Current.MsaId, G.TradingGuardianManifest.Current.LsaIds, G.TradingGuardianManifest.Current.CsaEligibilityPolicy,
                G.TradingGuardianManifest.Current.ProvidedCapabilities, G.TradingGuardianManifest.Current.DeclaredConsumers, G.TradingGuardianManifest.Current.Permissions,
                G.TradingGuardianManifest.Current.AuthorityRequests, G.TradingGuardianManifest.Current.DeclaredDependencies, G.TradingGuardianManifest.Current.SecurityProfile,
                G.TradingGuardianManifest.Current.ResourcePolicy, G.TradingGuardianManifest.Current.PersistencePolicy, G.TradingGuardianManifest.Current.CommunicationPolicy,
                G.TradingGuardianManifest.Current.ConfigurationPolicy, G.TradingGuardianManifest.Current.EvidencePolicy, G.TradingGuardianManifest.Current.HealthPolicy,
                G.TradingGuardianManifest.Current.FailureContainmentPolicy, G.TradingGuardianManifest.Current.SelfDevelopmentPolicy,
                G.TradingGuardianManifest.Current.GuardianRequirement, G.TradingGuardianManifest.Current.ProtectionInterface, G.TradingGuardianManifest.Current.RollbackPlan),
            new NormalizedApplicationSource(
                S.FSTSimAManifest.Current.ApplicationId, S.FSTSimAManifest.Current.PackageId, S.FSTSimAManifest.Current.Version,
                S.FSTSimAManifest.Current.Owner, S.FSTSimAManifest.Current.Purpose, S.FSTSimAFoundationOnboarding.Current.ManifestId,
                S.FSTSimAFoundationOnboarding.Current.ProviderBoundary, S.FSTSimAFoundationOnboarding.Current.RequiredFoundationServices,
                S.FSTSimAManifest.Current.MsaId, S.FSTSimAManifest.Current.LsaIds, S.FSTSimAManifest.Current.CsaEligibilityPolicy,
                S.FSTSimAManifest.Current.ProvidedCapabilities, S.FSTSimAManifest.Current.DeclaredConsumers, S.FSTSimAManifest.Current.Permissions,
                S.FSTSimAManifest.Current.AuthorityRequests, S.FSTSimAManifest.Current.DeclaredDependencies, S.FSTSimAManifest.Current.SecurityProfile,
                S.FSTSimAManifest.Current.ResourcePolicy, S.FSTSimAManifest.Current.PersistencePolicy, S.FSTSimAManifest.Current.CommunicationPolicy,
                S.FSTSimAManifest.Current.ConfigurationPolicy, S.FSTSimAManifest.Current.EvidencePolicy, S.FSTSimAManifest.Current.HealthPolicy,
                S.FSTSimAManifest.Current.FailureContainmentPolicy, S.FSTSimAManifest.Current.SelfDevelopmentPolicy,
                S.FSTSimAManifest.Current.GuardianRequirement, S.FSTSimAManifest.Current.ProtectionInterface, S.FSTSimAManifest.Current.RollbackPlan),
            new NormalizedApplicationSource(
                R.ResourceManagementManifest.Current.ApplicationId, R.ResourceManagementManifest.Current.PackageId, R.ResourceManagementManifest.Current.Version,
                R.ResourceManagementManifest.Current.Owner, R.ResourceManagementManifest.Current.Purpose, R.ResourceManagementFoundationOnboarding.Current.ManifestId,
                R.ResourceManagementFoundationOnboarding.Current.ProviderBoundary, R.ResourceManagementFoundationOnboarding.Current.RequiredFoundationServices,
                R.ResourceManagementManifest.Current.MsaId, R.ResourceManagementManifest.Current.LsaIds, R.ResourceManagementManifest.Current.CsaEligibilityPolicy,
                R.ResourceManagementManifest.Current.ProvidedCapabilities, R.ResourceManagementManifest.Current.DeclaredConsumers, R.ResourceManagementManifest.Current.Permissions,
                R.ResourceManagementManifest.Current.AuthorityRequests, R.ResourceManagementManifest.Current.DeclaredDependencies, R.ResourceManagementManifest.Current.SecurityProfile,
                R.ResourceManagementManifest.Current.ResourcePolicy, R.ResourceManagementManifest.Current.PersistencePolicy, R.ResourceManagementManifest.Current.CommunicationPolicy,
                R.ResourceManagementManifest.Current.ConfigurationPolicy, R.ResourceManagementManifest.Current.EvidencePolicy, R.ResourceManagementManifest.Current.HealthPolicy,
                R.ResourceManagementManifest.Current.FailureContainmentPolicy, R.ResourceManagementManifest.Current.SelfDevelopmentPolicy,
                R.ResourceManagementManifest.Current.GuardianRequirement, R.ResourceManagementManifest.Current.ProtectionInterface, R.ResourceManagementManifest.Current.RollbackPlan)
        };

        return Array.AsReadOnly(sources.Select(BuildPair).ToArray());
    }

    private static Fcr0254ApplicationRequestPair BuildPair(NormalizedApplicationSource source)
    {
        var branches = source.LsaIds.Select(lsa => new MajorBranchDeclarationCandidate(
            "branch:" + lsa.ToLowerInvariant(),
            "Accepted FSATS major branch represented by its responsible LSA.",
            lsa)).ToArray();
        var lsas = branches.Select(branch => new LsaDeclarationCandidate(
            branch.BranchName,
            branch.ResponsibleLsa,
            "Application-local major-branch awareness only; no Foundation authority.")).ToArray();

        var manifest = new FoundationApplicationManifestCandidate(
            source.ManifestId,
            source.ApplicationId,
            source.Version,
            source.Owner,
            source.Purpose,
            source.PackageId,
            source.Version,
            "application-development@" + ReviewedPart11ApplicationHead + "|" + source.PackageId + "|EXACT_BUILD_ARTIFACT_IDENTITY_REQUIRED_AT_EXECUTION",
            new[] { new FoundationDependencyCandidate("CON-001", new[] { "1.0" }) },
            new[] { new FoundationRequirementCandidate(ApplicationContractId, ApplicationContractVersion, ApplicationContractOwner, ApplicationContractAuthoritySource) },
            new[] { new FoundationRequirementCandidate(ApplicationSpecificationId, ApplicationSpecificationVersion, ApplicationSpecificationOwner, ApplicationSpecificationAuthoritySource) },
            source.RequiredFoundationServices.Select(service => new FoundationServiceRequirementCandidate(service, "1.0", "Required by accepted FSATS Application declaration.")).ToArray(),
            source.ProvidedCapabilities.ToArray(),
            source.IntendedConsumers.ToArray(),
            source.Permissions.Select(permission => new PermissionCandidate(permission, "APPLICATION_DECLARED_SCOPE", "Imported from accepted FSATS Application manifest; request does not grant authority.")).ToArray(),
            source.AuthorityRequests.Select(authority => new AuthorityRequestCandidate(authority, "REQUEST_ONLY_NO_IMPLICIT_GRANT", "Imported from accepted FSATS Application manifest; Foundation/Owner decision remains separate.")).ToArray(),
            new SecurityProfileCandidate("profile:" + source.ApplicationId.ToLowerInvariant(), "INTERNAL", "ISOLATED_APPLICATION_BOUNDARY|" + source.SecurityProfile),
            new ResourceRequirementsCandidate("FOUNDATION_ADMITTED_MINIMUM_REQUIRED", "FOUNDATION_ADMITTED_MINIMUM_REQUIRED", "FOUNDATION_ADMITTED_MINIMUM_REQUIRED", "FOUNDATION_ADMITTED_MINIMUM_REQUIRED"),
            new ResourceRequirementsCandidate("FOUNDATION_ADMITTED_CEILING_REQUIRED", "FOUNDATION_ADMITTED_CEILING_REQUIRED", "FOUNDATION_ADMITTED_CEILING_REQUIRED", "FOUNDATION_ADMITTED_CEILING_REQUIRED"),
            source.ResourcePolicy,
            source.PersistencePolicy,
            source.CommunicationPolicy,
            source.ConfigurationPolicy,
            source.EvidencePolicy,
            new LifecycleBehaviorCandidate(
                "PACKAGE_PRESENT_PREPARATION_ONLY",
                "FOUNDATION_NON_MUTATING_VALIDATION_REQUIRED",
                "OWNER_AUTHORITY_REQUIRED_FOR_ACTUAL_REGISTRATION",
                "CANDIDATE_PREPARED_NOT_ADMITTED",
                "NOT_AUTHORIZED",
                "GOVERNED_UPDATE_ONLY",
                "SEPARATE_AUTHORITY_REQUIRED",
                "EVIDENCE_AND_SEPARATE_RECOVERY_AUTHORITY_REQUIRED",
                "SEPARATE_AUTHORITY_REQUIRED",
                "SEPARATE_AUTHORITY_REQUIRED"),
            source.HealthPolicy,
            source.FailureContainmentPolicy,
            true,
            new[] { new MsaDeclarationCandidate(source.MsaId, source.Owner, "Application-only MSA; FSA remains Foundation/OS-level.") },
            branches,
            lsas,
            source.CsaEligibilityPolicy,
            source.SelfDevelopmentPolicy,
            source.GuardianRequirement + " | " + source.ProtectionInterface,
            source.RollbackPlan);

        var provenanceContent = string.Join("|", new[]
        {
            "FCR-0254",
            source.ApplicationId,
            source.Version,
            "reviewed-part11-head=" + ReviewedPart11ApplicationHead,
            "foundation-head=" + ReviewedFoundationHead,
            "PREPARATION_ONLY_NO_ACTUAL_LINK"
        });

        var admission = new ExactAdmissionRequestCandidate(
            "admission-candidate:fcr0254:" + source.ApplicationId.ToLowerInvariant(),
            "APPLICATION",
            source.ApplicationId,
            source.Version,
            source.Owner,
            ApplicationContractAuthoritySource,
            ApplicationContractId,
            ApplicationContractVersion,
            source.ManifestId,
            manifest,
            manifest.ComputeFoundationCanonicalDigest(),
            "provenance:fcr0254:" + source.ApplicationId.ToLowerInvariant(),
            provenanceContent,
            Sha256(provenanceContent),
            "bootstrap:fcr0254:" + source.ApplicationId.ToLowerInvariant(),
            "DEFINED",
            source.ProviderBoundary,
            "decision-seed:fcr0254:" + source.ApplicationId.ToLowerInvariant() + ":" + source.Version);

        var artifactBinding = new BindAtExecution("ExpectedArtifactExactIdentity", "Foundation-governed exact technical artifact consumption evidence", "Must identify the exact build artifact accepted for technical consumption at the authorized execution instant.");
        var admissionBinding = new BindAtExecution("Admission", "Foundation AdmissionControl state-producing decision", "Must come from actual positive admission evidence; candidate preparation cannot self-admit.");
        var lifecycleBinding = new BindAtExecution("LifecycleEligibility", "Foundation lifecycle authority", "Attach eligibility and decision identity must be current and authoritative when actual registration is authorized.");
        var resourceBinding = new BindAtExecution("ResourceGrants", "Foundation Resource Governance", "At least one current grant is required by runtime hosting; allocation/quota/ceiling and evidence time must not be fabricated.");
        var observedAtBinding = new BindAtExecution("ObservedAt", "Foundation authoritative time at execution", "Registration freshness checks require the actual execution observation time.");

        var registration = new RuntimeRegistrationRequestTemplate(
            "runtime-candidate:fcr0254:" + source.ApplicationId.ToLowerInvariant(),
            source.ApplicationId,
            source.Version,
            artifactBinding,
            new RuntimeArtifactConsumptionTemplate(artifactBinding with { Field = "ArtifactConsumption.AcceptedForTechnicalConsumption" }, artifactBinding, false, false, false, false, false),
            new RuntimeAdmissionTemplate(admissionBinding with { Field = "Admission.Admitted" }, source.ApplicationId, source.Version, admissionBinding with { Field = "Admission.EvidenceIdentity" }),
            new RuntimeLifecycleEligibilityTemplate(lifecycleBinding with { Field = "LifecycleEligibility.Eligible" }, "Attach", source.ApplicationId, lifecycleBinding with { Field = "LifecycleEligibility.CurrentVersion" }, source.Version, lifecycleBinding with { Field = "LifecycleEligibility.DecisionIdentity" }),
            new[] { new RuntimeResourceGrantTemplate(resourceBinding) },
            source.ProvidedCapabilities.Select(capability => new RuntimeCapabilityDeclarationTemplate(capability, "Private", false)).ToArray(),
            source.DeclaredDependencies.ToArray(),
            observedAtBinding,
            ExecutesRegistration: false,
            GrantsActivation: false,
            GrantsDeployment: false,
            GrantsProduction: false,
            GrantsBusinessAuthority: false);

        return new Fcr0254ApplicationRequestPair(admission, registration);
    }

    private static string Sha256(string value)
        => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
}

internal static class FoundationCandidateCanonicalizer
{
    internal static string ComputeDigest(FoundationApplicationManifestCandidate manifest)
        => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(Serialize(manifest))));

    internal static string Serialize(FoundationApplicationManifestCandidate manifest)
    {
        var builder = new StringBuilder();
        Append(builder, "ManifestId", manifest.ManifestId);
        Append(builder, "ApplicationIdentity", manifest.ApplicationIdentity);
        Append(builder, "ApplicationVersion", manifest.ApplicationVersion);
        Append(builder, "ApplicationOwner", manifest.ApplicationOwner);
        Append(builder, "ApplicationPurpose", manifest.ApplicationPurpose);
        Append(builder, "PackageIdentity", manifest.PackageIdentity);
        Append(builder, "PackageVersion", manifest.PackageVersion);
        Append(builder, "PackageContentOrIntegrityInput", manifest.PackageContentOrIntegrityInput);
        Append(builder, "DeclaredDependencies", JoinSorted(manifest.DeclaredDependencies.Select(d => $"{Escape(d.Identity)}|{JoinSorted(d.CompatibleVersions.Select(Escape))}")));
        Append(builder, "RequiredFoundationContracts", JoinSorted(manifest.RequiredFoundationContracts.Select(r => $"{Escape(r.Identity)}|{Escape(r.Version)}|{Escape(r.Owner)}|{Escape(r.AuthoritySource)}")));
        Append(builder, "RequiredFoundationSpecifications", JoinSorted(manifest.RequiredFoundationSpecifications.Select(r => $"{Escape(r.Identity)}|{Escape(r.Version)}|{Escape(r.Owner)}|{Escape(r.AuthoritySource)}")));
        Append(builder, "RequiredFoundationServices", JoinSorted(manifest.RequiredFoundationServices.Select(s => $"{Escape(s.Identity)}|{Escape(s.Version)}|{Escape(s.Purpose)}")));
        Append(builder, "ProvidedCapabilities", JoinSorted(manifest.ProvidedCapabilities.Select(Escape)));
        Append(builder, "IntendedConsumers", JoinSorted(manifest.IntendedConsumers.Select(Escape)));
        Append(builder, "RequestedPermissions", JoinSorted(manifest.RequestedPermissions.Select(p => $"{Escape(p.Name)}|{Escape(p.Scope)}|{Escape(p.Rationale)}")));
        Append(builder, "AuthorityRequests", JoinSorted(manifest.AuthorityRequests.Select(a => $"{Escape(a.Name)}|{Escape(a.Scope)}|{Escape(a.Rationale)}")));
        Append(builder, "SecurityProfile", $"{Escape(manifest.SecurityProfile.Name)}|{Escape(manifest.SecurityProfile.Classification)}|{Escape(manifest.SecurityProfile.IsolationModel)}");
        Append(builder, "MinimumResourceRequirements", SerializeResources(manifest.MinimumResourceRequirements));
        Append(builder, "ResourceCeilings", SerializeResources(manifest.ResourceCeilings));
        Append(builder, "DegradedBehavior", manifest.DegradedBehavior);
        Append(builder, "PersistenceRequirements", manifest.PersistenceRequirements);
        Append(builder, "CommunicationRequirements", manifest.CommunicationRequirements);
        Append(builder, "ConfigurationRequirements", manifest.ConfigurationRequirements);
        Append(builder, "EvidenceRequirements", manifest.EvidenceRequirements);
        Append(builder, "LifecycleBehavior", string.Join("|", new[] { manifest.LifecycleBehavior.Installation, manifest.LifecycleBehavior.Validation, manifest.LifecycleBehavior.Registration, manifest.LifecycleBehavior.Admission, manifest.LifecycleBehavior.Activation, manifest.LifecycleBehavior.Update, manifest.LifecycleBehavior.Suspension, manifest.LifecycleBehavior.Recovery, manifest.LifecycleBehavior.Replacement, manifest.LifecycleBehavior.Removal }.Select(Escape)));
        Append(builder, "HealthReportingInterface", manifest.HealthReportingInterface);
        Append(builder, "FailureContainmentInterface", manifest.FailureContainmentInterface);
        Append(builder, "UsesBranchBasedInternalArchitecture", manifest.UsesBranchBasedInternalArchitecture ? "true" : "false");
        Append(builder, "MsaDeclarations", JoinSorted(manifest.MsaDeclarations.Select(m => $"{Escape(m.Identity)}|{Escape(m.Owner)}|{Escape(m.Scope)}")));
        Append(builder, "MajorBranchDeclarations", JoinSorted(manifest.MajorBranchDeclarations.Select(b => $"{Escape(b.BranchName)}|{Escape(b.Purpose)}|{Escape(b.ResponsibleLsa)}")));
        Append(builder, "LsaDeclarations", JoinSorted(manifest.LsaDeclarations.Select(l => $"{Escape(l.BranchName)}|{Escape(l.ResponsibleLsa)}|{Escape(l.Scope)}")));
        Append(builder, "CsaEligibilityPolicy", manifest.CsaEligibilityPolicy);
        Append(builder, "SelfDevelopmentOriginAndEscalationPath", manifest.SelfDevelopmentOriginAndEscalationPath);
        Append(builder, "GuardianAndProtectionInterface", manifest.GuardianAndProtectionInterface);
        Append(builder, "RollbackOrCorrectiveActionPlan", manifest.RollbackOrCorrectiveActionPlan);
        return builder.ToString();
    }

    private static string SerializeResources(ResourceRequirementsCandidate r)
        => $"{Escape(r.Memory)}|{Escape(r.Cpu)}|{Escape(r.Storage)}|{Escape(r.Network)}";

    private static string JoinSorted(IEnumerable<string> values)
        => string.Join(";", values.OrderBy(value => value, StringComparer.Ordinal));

    private static string Escape(string? value)
        => (value ?? string.Empty)
            .Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("\r", "\\r", StringComparison.Ordinal)
            .Replace("\n", "\\n", StringComparison.Ordinal)
            .Replace("|", "\\|", StringComparison.Ordinal)
            .Replace(";", "\\;", StringComparison.Ordinal)
            .Replace("=", "\\=", StringComparison.Ordinal);

    private static void Append(StringBuilder builder, string name, string value)
    {
        builder.Append(name);
        builder.Append('=');
        builder.Append(Escape(value ?? string.Empty));
        builder.Append('\n');
    }
}
