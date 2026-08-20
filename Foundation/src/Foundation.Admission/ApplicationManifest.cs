using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Foundation.Admission;

public sealed record DependencyDeclaration(string Identity, IReadOnlyList<string> CompatibleVersions);

public sealed record FoundationRequirement(string Identity, string Version, string Owner, string AuthoritySource);

public sealed record FoundationServiceRequirement(string Identity, string Version, string Purpose);

public sealed record PermissionDeclaration(string Name, string Scope, string Rationale);

public sealed record AuthorityRequest(string Name, string Scope, string Rationale);

public sealed record SecurityProfile(string Name, string Classification, string IsolationModel);

public sealed record ResourceRequirements(string Memory, string Cpu, string Storage, string Network);

public sealed record LifecycleBehavior(
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

public sealed record MsaDeclaration(string Identity, string Owner, string Scope);

public sealed record MajorBranchDeclaration(string BranchName, string Purpose, string ResponsibleLsa);

public sealed record LsaDeclaration(string BranchName, string ResponsibleLsa, string Scope);

public sealed record ApplicationManifest(
    string ManifestId,
    string ApplicationIdentity,
    string ApplicationVersion,
    string ApplicationOwner,
    string ApplicationPurpose,
    string PackageIdentity,
    string PackageVersion,
    string PackageContentOrIntegrityInput,
    IReadOnlyList<DependencyDeclaration> DeclaredDependencies,
    IReadOnlyList<FoundationRequirement> RequiredFoundationContracts,
    IReadOnlyList<FoundationRequirement> RequiredFoundationSpecifications,
    IReadOnlyList<FoundationServiceRequirement> RequiredFoundationServices,
    IReadOnlyList<string> ProvidedCapabilities,
    IReadOnlyList<string> IntendedConsumers,
    IReadOnlyList<PermissionDeclaration> RequestedPermissions,
    IReadOnlyList<AuthorityRequest> AuthorityRequests,
    SecurityProfile SecurityProfile,
    ResourceRequirements MinimumResourceRequirements,
    ResourceRequirements ResourceCeilings,
    string DegradedBehavior,
    string PersistenceRequirements,
    string CommunicationRequirements,
    string ConfigurationRequirements,
    string EvidenceRequirements,
    LifecycleBehavior LifecycleBehavior,
    string HealthReportingInterface,
    string FailureContainmentInterface,
    bool UsesBranchBasedInternalArchitecture,
    IReadOnlyList<MsaDeclaration> MsaDeclarations,
    IReadOnlyList<MajorBranchDeclaration> MajorBranchDeclarations,
    IReadOnlyList<LsaDeclaration> LsaDeclarations,
    string CsaEligibilityPolicy,
    string SelfDevelopmentOriginAndEscalationPath,
    string GuardianAndProtectionInterface,
    string RollbackOrCorrectiveActionPlan)
{
    public string CanonicalText() => ApplicationManifestCanonicalizer.Serialize(this);

    public string ComputeDigest() => ApplicationManifestCanonicalizer.ComputeDigest(this);
}

public static class ApplicationManifestCanonicalizer
{
    public static string ComputeDigest(ApplicationManifest? manifest)
        => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(Serialize(manifest))));

    public static string Serialize(ApplicationManifest? manifest)
    {
        if (manifest is null) return string.Empty;
        var builder = new StringBuilder();
        Append(builder, "ManifestId", manifest.ManifestId);
        Append(builder, "ApplicationIdentity", manifest.ApplicationIdentity);
        Append(builder, "ApplicationVersion", manifest.ApplicationVersion);
        Append(builder, "ApplicationOwner", manifest.ApplicationOwner);
        Append(builder, "ApplicationPurpose", manifest.ApplicationPurpose);
        Append(builder, "PackageIdentity", manifest.PackageIdentity);
        Append(builder, "PackageVersion", manifest.PackageVersion);
        Append(builder, "PackageContentOrIntegrityInput", manifest.PackageContentOrIntegrityInput);
        Append(builder, "DeclaredDependencies", SerializeDependencies(manifest.DeclaredDependencies));
        Append(builder, "RequiredFoundationContracts", SerializeFoundationRequirements(manifest.RequiredFoundationContracts));
        Append(builder, "RequiredFoundationSpecifications", SerializeFoundationRequirements(manifest.RequiredFoundationSpecifications));
        Append(builder, "RequiredFoundationServices", SerializeFoundationServices(manifest.RequiredFoundationServices));
        Append(builder, "ProvidedCapabilities", SerializeStrings(manifest.ProvidedCapabilities));
        Append(builder, "IntendedConsumers", SerializeStrings(manifest.IntendedConsumers));
        Append(builder, "RequestedPermissions", SerializePermissions(manifest.RequestedPermissions));
        Append(builder, "AuthorityRequests", SerializeAuthorities(manifest.AuthorityRequests));
        Append(builder, "SecurityProfile", SerializeSecurityProfile(manifest.SecurityProfile));
        Append(builder, "MinimumResourceRequirements", SerializeResources(manifest.MinimumResourceRequirements));
        Append(builder, "ResourceCeilings", SerializeResources(manifest.ResourceCeilings));
        Append(builder, "DegradedBehavior", manifest.DegradedBehavior);
        Append(builder, "PersistenceRequirements", manifest.PersistenceRequirements);
        Append(builder, "CommunicationRequirements", manifest.CommunicationRequirements);
        Append(builder, "ConfigurationRequirements", manifest.ConfigurationRequirements);
        Append(builder, "EvidenceRequirements", manifest.EvidenceRequirements);
        Append(builder, "LifecycleBehavior", SerializeLifecycle(manifest.LifecycleBehavior));
        Append(builder, "HealthReportingInterface", manifest.HealthReportingInterface);
        Append(builder, "FailureContainmentInterface", manifest.FailureContainmentInterface);
        Append(builder, "UsesBranchBasedInternalArchitecture", manifest.UsesBranchBasedInternalArchitecture ? "true" : "false");
        Append(builder, "MsaDeclarations", SerializeMsaDeclarations(manifest.MsaDeclarations));
        Append(builder, "MajorBranchDeclarations", SerializeMajorBranches(manifest.MajorBranchDeclarations));
        Append(builder, "LsaDeclarations", SerializeLsaDeclarations(manifest.LsaDeclarations));
        Append(builder, "CsaEligibilityPolicy", manifest.CsaEligibilityPolicy);
        Append(builder, "SelfDevelopmentOriginAndEscalationPath", manifest.SelfDevelopmentOriginAndEscalationPath);
        Append(builder, "GuardianAndProtectionInterface", manifest.GuardianAndProtectionInterface);
        Append(builder, "RollbackOrCorrectiveActionPlan", manifest.RollbackOrCorrectiveActionPlan);
        return builder.ToString();
    }

    private static string SerializeDependencies(IReadOnlyList<DependencyDeclaration>? dependencies)
        => JoinSorted((dependencies ?? Array.Empty<DependencyDeclaration>())
            .Select(d => d is null ? "<null>" : $"{Escape(d.Identity)}|{SerializeStrings(d.CompatibleVersions)}"));

    private static string SerializeFoundationRequirements(IReadOnlyList<FoundationRequirement>? requirements)
        => JoinSorted((requirements ?? Array.Empty<FoundationRequirement>())
            .Select(r => r is null ? "<null>" : $"{Escape(r.Identity)}|{Escape(r.Version)}|{Escape(r.Owner)}|{Escape(r.AuthoritySource)}"));

    private static string SerializeFoundationServices(IReadOnlyList<FoundationServiceRequirement>? services)
        => JoinSorted((services ?? Array.Empty<FoundationServiceRequirement>())
            .Select(service => service is null ? "<null>" : $"{Escape(service.Identity)}|{Escape(service.Version)}|{Escape(service.Purpose)}"));

    private static string SerializeStrings(IReadOnlyList<string>? values)
        => JoinSorted((values ?? Array.Empty<string>()).Select(Escape));

    private static string SerializePermissions(IReadOnlyList<PermissionDeclaration>? permissions)
        => JoinSorted((permissions ?? Array.Empty<PermissionDeclaration>())
            .Select(permission => permission is null ? "<null>" : $"{Escape(permission.Name)}|{Escape(permission.Scope)}|{Escape(permission.Rationale)}"));

    private static string SerializeAuthorities(IReadOnlyList<AuthorityRequest>? authorities)
        => JoinSorted((authorities ?? Array.Empty<AuthorityRequest>())
            .Select(authority => authority is null ? "<null>" : $"{Escape(authority.Name)}|{Escape(authority.Scope)}|{Escape(authority.Rationale)}"));

    private static string SerializeSecurityProfile(SecurityProfile? profile)
        => profile is null ? "<null>" : $"{Escape(profile.Name)}|{Escape(profile.Classification)}|{Escape(profile.IsolationModel)}";

    private static string SerializeResources(ResourceRequirements? resources)
        => resources is null ? "<null>" : $"{Escape(resources.Memory)}|{Escape(resources.Cpu)}|{Escape(resources.Storage)}|{Escape(resources.Network)}";

    private static string SerializeLifecycle(LifecycleBehavior? lifecycle)
        => lifecycle is null ? "<null>" : string.Join("|", new[]
        {
            Escape(lifecycle.Installation),
            Escape(lifecycle.Validation),
            Escape(lifecycle.Registration),
            Escape(lifecycle.Admission),
            Escape(lifecycle.Activation),
            Escape(lifecycle.Update),
            Escape(lifecycle.Suspension),
            Escape(lifecycle.Recovery),
            Escape(lifecycle.Replacement),
            Escape(lifecycle.Removal)
        });

    private static string SerializeMsaDeclarations(IReadOnlyList<MsaDeclaration>? declarations)
        => JoinSorted((declarations ?? Array.Empty<MsaDeclaration>())
            .Select(declaration => declaration is null ? "<null>" : $"{Escape(declaration.Identity)}|{Escape(declaration.Owner)}|{Escape(declaration.Scope)}"));

    private static string SerializeMajorBranches(IReadOnlyList<MajorBranchDeclaration>? branches)
        => JoinSorted((branches ?? Array.Empty<MajorBranchDeclaration>())
            .Select(branch => branch is null ? "<null>" : $"{Escape(branch.BranchName)}|{Escape(branch.Purpose)}|{Escape(branch.ResponsibleLsa)}"));

    private static string SerializeLsaDeclarations(IReadOnlyList<LsaDeclaration>? declarations)
        => JoinSorted((declarations ?? Array.Empty<LsaDeclaration>())
            .Select(declaration => declaration is null ? "<null>" : $"{Escape(declaration.BranchName)}|{Escape(declaration.ResponsibleLsa)}|{Escape(declaration.Scope)}"));

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
