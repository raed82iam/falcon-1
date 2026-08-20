namespace Falcon.FSATS.FSAPMA.Contracts;

public enum DataTruthState { Current, Stale, Conflicted, Unknown, Unavailable, Correction }
public readonly record struct ProviderId(string Value);
public readonly record struct ProviderAccountId(string Value);
public readonly record struct ApiInstanceId(string Value);
public readonly record struct ProviderEndpointId(string Value);
public readonly record struct ProducerInstrumentId(string Provider, string Value);
public readonly record struct DataProductId(string Value);
public readonly record struct ObservationId(string Value);

public sealed record OperationalDataProjection(
    ObservationId ObservationId,
    ProviderId Provider,
    ProducerInstrumentId Instrument,
    DataProductId Product,
    decimal Value,
    DateTimeOffset ObservedAt,
    DateTimeOffset ReceivedAt,
    DataTruthState Truth,
    string Provenance,
    string SchemaVersion,
    ProviderAccountId ProviderAccount = default,
    string Environment = "",
    string ServiceRole = "",
    string CredentialReference = "",
    ApiInstanceId ApiInstance = default,
    ProviderEndpointId Endpoint = default)
{
    // Historical Part-2 completeness surface retained for accepted executable compatibility.
    public bool HasCompleteProviderRouteIdentity
        => !string.IsNullOrWhiteSpace(Provider.Value) && !string.IsNullOrWhiteSpace(ProviderAccount.Value) && !string.IsNullOrWhiteSpace(Environment) && !string.IsNullOrWhiteSpace(ServiceRole) && !string.IsNullOrWhiteSpace(CredentialReference);

    // Current integrated P0-G contract. New runtime-admission work must use this stronger predicate.
    public bool HasCurrentProviderRouteIdentity
        => HasCompleteProviderRouteIdentity
           && !string.IsNullOrWhiteSpace(ApiInstance.Value)
           && !string.IsNullOrWhiteSpace(Endpoint.Value);

    public string ProviderRouteNamespace
        => string.Join('|',
            Part(Provider.Value.Trim().ToUpperInvariant()),
            Part(ProviderAccount.Value.Trim()),
            Part(Environment.Trim().ToUpperInvariant()),
            Part(ServiceRole.Trim().ToUpperInvariant()),
            Part(ApiInstance.Value?.Trim() ?? string.Empty),
            Part(Endpoint.Value?.Trim() ?? string.Empty),
            Part(CredentialReference.Trim()));

    private static string Part(string value) => Uri.EscapeDataString(value);
}

public sealed record DataQualityCorrection(ObservationId OriginalObservation, ObservationId CorrectionObservation, DataTruthState NewTruth, string ReasonCode, DateTimeOffset EffectiveAt);
public sealed record ProviderResourceEvidence(string ApplicationId, string ResourceClass, decimal CurrentConsumption, decimal MinimumSafeRequirement, decimal DesiredCapacity, decimal ReclaimableCapacity, string DegradationOptions, DateTimeOffset ObservedAt);
