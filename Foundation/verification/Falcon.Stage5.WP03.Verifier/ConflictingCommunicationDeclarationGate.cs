using System;
using System.Runtime.CompilerServices;
using Foundation.ApplicationManifest;
using Foundation.Contracts;
using Foundation.SchemaRegistry;

namespace Falcon.Stage5.WP03.Verifier;

internal static class ConflictingCommunicationDeclarationGate
{
    private static readonly string DigestA = new('A', 64);

    [ModuleInitializer]
    internal static void Verify()
    {
        var schemas = new InMemorySchemaRegistry();
        var registration = schemas.Register(new SchemaDefinition(
            new SchemaIdentity("schema.sample"),
            "1.0",
            new SchemaOwnerReference("owner.schema"),
            DigestA,
            new ProvenanceReference("evidence.schema")));

        if (!registration.Accepted)
        {
            throw new InvalidOperationException($"red_team_schema_registration_failed:{registration.Reason}");
        }

        var schema = new ManifestSchemaReference(new SchemaIdentity("schema.sample"), "1.0");
        var outbound = new CommunicationDeclaration(
            "Foundation.Sample.Conflict",
            FilMessageKind.Command,
            FilMessageClassification.Operational,
            schema,
            CommunicationDirection.Outbound,
            CommunicationRole.Producer);
        var inbound = new CommunicationDeclaration(
            "Foundation.Sample.Conflict",
            FilMessageKind.Command,
            FilMessageClassification.Operational,
            schema,
            CommunicationDirection.Inbound,
            CommunicationRole.Consumer);

        var manifest = new ApplicationCommunicationManifest(
            new ManifestIdentity("manifest.redteam.conflict"),
            "1.0",
            new ApplicationIdentityReference("app.redteam"),
            "1.0",
            new ApplicationOwnerReference("owner.redteam"),
            new[] { new ManifestReference("contract.sample") },
            new[] { new ManifestReference("service.sample") },
            new[] { new ManifestReference("capability.sample") },
            new[] { new ManifestReference("consumer.sample") },
            new[] { new AuthorityReference("authority.request") },
            new[] { new ManifestReference("security.profile") },
            new[] { new ManifestReference("dependency.sample") },
            new[] { new ManifestReference("configuration.sample") },
            new[] { new ProvenanceReference("evidence.manifest") },
            new[]
            {
                new ManifestLifecycleDeclaration(ManifestLifecycleEvent.ApplicationVersionChange, ManifestApplicabilityRule.RequiresRevalidation),
                new ManifestLifecycleDeclaration(ManifestLifecycleEvent.Update, ManifestApplicabilityRule.RequiresRevalidation),
                new ManifestLifecycleDeclaration(ManifestLifecycleEvent.Replacement, ManifestApplicabilityRule.Invalidated),
                new ManifestLifecycleDeclaration(ManifestLifecycleEvent.Suspension, ManifestApplicabilityRule.RemainsApplicable),
                new ManifestLifecycleDeclaration(ManifestLifecycleEvent.Removal, ManifestApplicabilityRule.Invalidated)
            },
            new[] { outbound, inbound });

        var validation = ApplicationCommunicationManifestValidator.Validate(manifest, schemas);
        if (validation.IsValid || !string.Equals(validation.Code, "CONFLICTING_COMMUNICATION_DECLARATION", StringComparison.Ordinal))
        {
            throw new InvalidOperationException($"conflicting_communication_binding_not_rejected:{validation.Code}");
        }

        Console.WriteLine("PASS conflicting_communication_binding_rejected");
    }
}
