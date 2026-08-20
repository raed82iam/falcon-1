using System;
using System.Runtime.CompilerServices;
using Foundation.ApplicationManifest;
using Foundation.Authority;
using Foundation.Contracts;
using Foundation.MessageRouting;
using Foundation.SchemaRegistry;

namespace Falcon.Stage5.WP05.Verifier;

internal static class ManifestAuthorityDeclarationGate
{
    [ModuleInitializer]
    internal static void Run()
    {
        const string schemaId = "schema:falcon.wp05.authority-declaration-gate";
        const string messageType = "falcon.wp05.authority.declaration.gate.v1";
        const string applicationId = "application.wp05.authority-gate";
        const string manifestId = "manifest:wp05-authority-gate";
        const string consumer = "consumer:wp05-authority-gate";
        const string producer = "producer:wp05-authority-gate";
        const string recipient = "recipient:wp05-authority-gate";
        const string routeId = "route:wp05-authority-gate";
        const string routeAuthorityRef = "authority:route/wp05-authority-gate";
        const string admissionAuthorityRef = "authority:admission/wp05-authority-gate";
        const string routePurpose = "purpose:wp05-authority-gate";
        const string routeScope = "scope:route/wp05-authority-gate";

        var schemaRegistry = new InMemorySchemaRegistry();
        var schemaRegistration = schemaRegistry.Register(new SchemaDefinition(
            new SchemaIdentity(schemaId),
            "1.0",
            new SchemaOwnerReference("owner:schema/wp05-authority-gate"),
            new string('A', 64),
            new ProvenanceReference("evidence:schema/wp05-authority-gate")));
        if (!schemaRegistration.Accepted)
            throw new InvalidOperationException("wp05_authority_gate_schema_registration_failed");

        var manifestRegistry = new InMemoryApplicationCommunicationManifestRegistry(schemaRegistry);
        var manifest = new ApplicationCommunicationManifest(
            new ManifestIdentity(manifestId),
            "1.0",
            new ApplicationIdentityReference(applicationId),
            "1.0",
            new ApplicationOwnerReference("owner:application/wp05-authority-gate"),
            new[] { new ManifestReference("CON-004"), new ManifestReference("CON-023") },
            new[] { new ManifestReference("service:fil") },
            new[] { new ManifestReference("capability:wp05-authority-gate") },
            new[] { new ManifestReference(consumer) },
            new[] { new AuthorityReference(admissionAuthorityRef) },
            new[] { new ManifestReference("security:wp05-authority-gate") },
            new[] { new ManifestReference("dependency:wp05-authority-gate") },
            new[] { new ManifestReference("configuration:wp05-authority-gate") },
            new[] { new ProvenanceReference("evidence:manifest/wp05-authority-gate") },
            new[]
            {
                new ManifestLifecycleDeclaration(ManifestLifecycleEvent.ApplicationVersionChange, ManifestApplicabilityRule.RequiresRevalidation),
                new ManifestLifecycleDeclaration(ManifestLifecycleEvent.Update, ManifestApplicabilityRule.RequiresRevalidation),
                new ManifestLifecycleDeclaration(ManifestLifecycleEvent.Replacement, ManifestApplicabilityRule.Invalidated),
                new ManifestLifecycleDeclaration(ManifestLifecycleEvent.Suspension, ManifestApplicabilityRule.RemainsApplicable),
                new ManifestLifecycleDeclaration(ManifestLifecycleEvent.Removal, ManifestApplicabilityRule.Invalidated)
            },
            new[]
            {
                new CommunicationDeclaration(
                    messageType,
                    FilMessageKind.Command,
                    FilMessageClassification.Operational,
                    new ManifestSchemaReference(new SchemaIdentity(schemaId), "1.0"),
                    CommunicationDirection.Outbound,
                    CommunicationRole.Producer)
            });

        var manifestRegistration = manifestRegistry.Register(manifest);
        if (!manifestRegistration.Accepted || string.IsNullOrWhiteSpace(manifestRegistration.ManifestSha256))
            throw new InvalidOperationException("wp05_authority_gate_manifest_registration_failed");

        var observation = new DateTimeOffset(2026, 8, 7, 20, 0, 0, TimeSpan.Zero);
        var authorityResult = new AuthorityResult(
            "request:route/wp05-authority-gate",
            "decision:route/wp05-authority-gate",
            AuthorityDecision.Allow,
            routeScope,
            "policy:route/wp05-authority-gate",
            "1.0",
            "conditions:wp05-authority-gate",
            "BOUNDED_TO_ROUTE",
            AuthorityReason.Allowed,
            observation.AddMinutes(-1),
            observation.AddMinutes(10),
            "evidence:authority-result/wp05-authority-gate");

        var routeIdentity = new RouteIdentity(routeId);
        var authorityBinding = new RouteAuthorityBinding(
            new AuthorityReference(routeAuthorityRef),
            authorityResult,
            routeIdentity,
            "1.0",
            new ProducerIdentityReference(producer),
            new ApplicationIdentityReference(applicationId),
            new RecipientScopeReference(recipient),
            new ManifestReference(consumer),
            messageType,
            routePurpose,
            routeScope,
            new ProvenanceReference("evidence:authority-binding/wp05-authority-gate"));

        var route = new RouteDeclaration(
            routeIdentity,
            "1.0",
            new ManifestIdentity(manifestId),
            "1.0",
            manifestRegistration.ManifestSha256,
            new ProducerIdentityReference(producer),
            new ApplicationIdentityReference(applicationId),
            new RecipientScopeReference(recipient),
            new ManifestReference(consumer),
            messageType,
            new RouteEndpointIdentity("endpoint:source/wp05-authority-gate"),
            new RouteEndpointIdentity("endpoint:destination/wp05-authority-gate"),
            routePurpose,
            RouteState.Eligible,
            authorityBinding,
            new ProvenanceReference("evidence:route/wp05-authority-gate"));

        var registration = new RouteRegistry(manifestRegistry).Register(route);
        if (registration.Accepted ||
            !string.Equals(registration.Reason, RouteRegistrationReason.ManifestAuthorityUndeclared, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                $"wp05_manifest_authority_declaration_gate_failed:accepted={registration.Accepted}:reason={registration.Reason}");
        }

        Console.WriteLine("PASS manifest_authority_declaration_gate");
    }
}
