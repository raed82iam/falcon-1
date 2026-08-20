using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Foundation.ApplicationManifest;
using Foundation.Authority;
using Foundation.Contracts;
using Foundation.MessageRouting;

namespace Falcon.Stage5.WP05.Verifier;

internal static class RouteAuthorityTemporalIdentityGate
{
    [ModuleInitializer]
    internal static void Run()
    {
        var method = typeof(RouteSelectionEvaluator).GetMethod(
            "CanonicalRegistry",
            BindingFlags.NonPublic | BindingFlags.Static)
            ?? throw new InvalidOperationException("wp05_canonical_registry_method_missing");

        var baseline = CreateRoute(
            new DateTimeOffset(2026, 8, 7, 17, 59, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 8, 7, 18, 20, 0, TimeSpan.Zero));

        var expiryMutation = CreateRoute(
            new DateTimeOffset(2026, 8, 7, 17, 59, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 8, 7, 18, 21, 0, TimeSpan.Zero));

        var decisionTimeMutation = CreateRoute(
            new DateTimeOffset(2026, 8, 7, 17, 58, 0, TimeSpan.Zero),
            new DateTimeOffset(2026, 8, 7, 18, 20, 0, TimeSpan.Zero));

        var baselineCanonical = Invoke(method, baseline);
        var expiryCanonical = Invoke(method, expiryMutation);
        var decisionTimeCanonical = Invoke(method, decisionTimeMutation);

        if (string.Equals(baselineCanonical, expiryCanonical, StringComparison.Ordinal))
            throw new InvalidOperationException("wp05_route_authority_expiry_not_bound_to_registry_identity");

        if (string.Equals(baselineCanonical, decisionTimeCanonical, StringComparison.Ordinal))
            throw new InvalidOperationException("wp05_route_authority_decision_time_not_bound_to_registry_identity");

        Console.WriteLine("PASS route_authority_temporal_identity_gate");
    }

    private static string Invoke(MethodInfo method, RouteDeclaration route) =>
        method.Invoke(null, new object[] { new[] { route } }) as string
        ?? throw new InvalidOperationException("wp05_canonical_registry_result_missing");

    private static RouteDeclaration CreateRoute(DateTimeOffset decisionTime, DateTimeOffset expiry)
    {
        const string routeId = "route:temporal-gate";
        const string routeVersion = "1.0";
        const string producer = "producer:temporal-gate";
        const string application = "application.temporal-gate";
        const string recipient = "recipient:temporal-gate";
        const string consumer = "consumer:temporal-gate";
        const string messageType = "falcon.reference.temporal.v1";
        const string purpose = "purpose:temporal-gate";
        const string scope = "scope:temporal-gate";

        var authorityResult = new AuthorityResult(
            "request:route/temporal-gate",
            "decision:route/temporal-gate",
            AuthorityDecision.Allow,
            scope,
            "policy:route-selection",
            "1.0",
            "conditions:temporal-gate",
            "BOUNDED_TO_ROUTE",
            AuthorityReason.Allowed,
            decisionTime,
            expiry,
            "evidence:route-authority/temporal-gate");

        var authorityBinding = new RouteAuthorityBinding(
            new AuthorityReference("authority:route/temporal-gate"),
            authorityResult,
            new RouteIdentity(routeId),
            routeVersion,
            new ProducerIdentityReference(producer),
            new ApplicationIdentityReference(application),
            new RecipientScopeReference(recipient),
            new ManifestReference(consumer),
            messageType,
            purpose,
            scope,
            new ProvenanceReference("evidence:route-authority/binding-temporal-gate"));

        return new RouteDeclaration(
            new RouteIdentity(routeId),
            routeVersion,
            new ManifestIdentity("manifest:temporal-gate"),
            "1.0",
            new string('A', 64),
            new ProducerIdentityReference(producer),
            new ApplicationIdentityReference(application),
            new RecipientScopeReference(recipient),
            new ManifestReference(consumer),
            messageType,
            new RouteEndpointIdentity("endpoint:source/temporal-gate"),
            new RouteEndpointIdentity("endpoint:destination/temporal-gate"),
            purpose,
            RouteState.Eligible,
            authorityBinding,
            new ProvenanceReference("evidence:route/temporal-gate"));
    }
}
