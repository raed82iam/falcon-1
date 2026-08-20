using System.Reflection;
using System.Runtime.Loader;
using RA = Falcon.FSATS.ResourceManagement.Application;

if (args.Length != 5)
{
    Console.Error.WriteLine("Usage: Falcon.FSATS.FoundationCompatibility.Verifier <Foundation.Contracts.dll> <Foundation.State.dll> <Foundation.MessageRouting.dll> <Foundation.MessageDelivery.dll> <Foundation.EventSystem.dll>");
    return 2;
}

var failures = new List<string>();
var checks = 0;

var contractsPath = Path.GetFullPath(args[0]);
var statePath = Path.GetFullPath(args[1]);
var routingPath = Path.GetFullPath(args[2]);
var deliveryPath = Path.GetFullPath(args[3]);
var eventPath = Path.GetFullPath(args[4]);
var required = new[] { contractsPath, statePath, routingPath, deliveryPath, eventPath };
if (required.Any(path => !File.Exists(path)))
{
    Console.Error.WriteLine("FOUNDATION COMPATIBILITY VERIFIER: FAIL (required Foundation assemblies missing)");
    return 2;
}

var probeDirectories = required
    .Select(Path.GetDirectoryName)
    .Where(path => !string.IsNullOrWhiteSpace(path))
    .Cast<string>()
    .Distinct(StringComparer.OrdinalIgnoreCase)
    .ToArray();

AssemblyLoadContext.Default.Resolving += (context, assemblyName) =>
{
    foreach (var directory in probeDirectories)
    {
        var candidate = Path.Combine(directory, assemblyName.Name + ".dll");
        if (File.Exists(candidate)) return context.LoadFromAssemblyPath(candidate);
    }
    return null;
};

var contractsAssembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(contractsPath);
var stateAssembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(statePath);
var routingAssembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(routingPath);
var deliveryAssembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(deliveryPath);
var eventAssembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(eventPath);

// Stage 6 resource boundary.
CheckType(contractsAssembly, "Foundation.Contracts.ResourceGovernance.ApplicationPrincipalId");
CheckType(contractsAssembly, "Foundation.Contracts.ResourceGovernance.ResourceClassId");
CheckType(contractsAssembly, "Foundation.Contracts.ResourceGovernance.ResourceGrantId");
CheckType(contractsAssembly, "Foundation.Contracts.ResourceGovernance.ResourceRequestId");
CheckType(contractsAssembly, "Foundation.Contracts.ResourceGovernance.ResourceDecisionId");
CheckType(contractsAssembly, "Foundation.Contracts.ResourceGovernance.ResourceEpochId");
CheckEnum(contractsAssembly, "Foundation.Contracts.ResourceGovernance.ResourcePressureState", Enum.GetNames<RA.FoundationPressureState>().Where(x => x != nameof(RA.FoundationPressureState.Unavailable)).ToArray());
CheckEnum(contractsAssembly, "Foundation.Contracts.ResourceGovernance.ResourceDecisionKind", Enum.GetNames<RA.FoundationResourceDecisionKind>());

CheckProperties(stateAssembly, "Foundation.State.ResourceGovernance.ApplicationResourceStateProjection",
    "ApplicationId", "ResourceClassId", "EpochId", "GrantId", "Allocation", "Quota", "Ceiling", "EffectiveCapacity",
    "PressureAvailable", "PressureState", "UtilizationBasisPoints", "PreemptionEligibleForConsideration",
    "DecisionReference", "AcceptedCapacityBasis", "ObservedAt", "IdentitySha256");
CheckEnum(stateAssembly, "Foundation.State.ResourceGovernance.TechnicalLoadSheddingSignalClass", Enum.GetNames<RA.FoundationLoadSheddingClass>());
CheckProperties(stateAssembly, "Foundation.State.ResourceGovernance.ApplicationResourceLoadSheddingSignal",
    "ApplicationId", "ResourceClassId", "SignalClass", "CompliantCapacityTarget", "RequiredReduction",
    "AcceptedCapacityBasisIdentitySha256", "GeneratedAt", "IdentitySha256");
CheckType(stateAssembly, "Foundation.State.ResourceGovernance.AdditionalResourceDecisionRecord");
CheckType(stateAssembly, "Foundation.State.ResourceGovernance.ApplicationResourceStateProjectionBuilder");
CheckType(stateAssembly, "Foundation.State.ResourceGovernance.ApplicationResourceLoadSheddingSignalFactory");
CheckType(stateAssembly, "Foundation.State.ResourceGovernance.ResourceCoordinationEnvelope");

// Stage 5 canonical messaging envelope required by FCR-0004/0005/0006.
CheckEnum(contractsAssembly, "Foundation.Contracts.FilMessageKind", "Command", "Query", "Response", "Event", "Notice");
CheckEnum(contractsAssembly, "Foundation.Contracts.FilMessageClassification", "Operational", "Governance", "Evidence", "Health", "Security", "Administrative");
CheckProperties(contractsAssembly, "Foundation.Contracts.CanonicalFilEnvelope",
    "MessageId", "MessageKind", "Classification", "MessageType", "SchemaId", "SchemaVersion", "Producer", "RecipientScope",
    "CorrelationId", "CausationId", "Authority", "Provenance", "IdempotencyId", "DeliveryAttemptId", "RetryLineageId",
    "Time", "Outcome", "Payload", "PayloadSha256");
CheckType(contractsAssembly, "Foundation.Contracts.CanonicalMessagingValidator");

// Governed route capability used by Guardian commands and operational data delivery.
CheckEnum(routingAssembly, "Foundation.MessageRouting.RouteState", "Eligible", "Isolated", "Unavailable");
CheckEnum(routingAssembly, "Foundation.MessageRouting.RouteSelectionDecision", "Selected", "Rejected");
CheckType(routingAssembly, "Foundation.MessageRouting.RouteAuthorityBinding");
CheckType(routingAssembly, "Foundation.MessageRouting.RouteDeclaration");
CheckType(routingAssembly, "Foundation.MessageRouting.RouteDecision");
CheckType(routingAssembly, "Foundation.MessageRouting.RouteRegistry");

// Delivery truth, idempotency/retry and protective traffic support.
CheckEnum(deliveryAssembly, "Foundation.MessageDelivery.DeliveryTrafficClass", "Normal", "Protective", "Revocation");
CheckEnum(deliveryAssembly, "Foundation.MessageDelivery.DeliveryDecisionKind", "DispatchEligible", "RetryEligible", "Deferred", "DeadLetter", "Rejected", "Expired", "AlreadyAcknowledged");
CheckType(deliveryAssembly, "Foundation.MessageDelivery.DeliveryIdempotencyBinding");
CheckType(deliveryAssembly, "Foundation.MessageDelivery.DeliveryPolicy");
CheckType(deliveryAssembly, "Foundation.MessageDelivery.DeliveryAttemptOutcome");
CheckType(deliveryAssembly, "Foundation.MessageDelivery.TransportOutcomeObservation");

// Event/evidence/replay truth separation required by FCR-0006.
CheckEnum(eventAssembly, "Foundation.EventSystem.EventTruthClassification",
    "AuthoritativeOperational", "Replay", "Test", "Simulation", "NonAuthoritativeEvidence");
CheckEnum(eventAssembly, "Foundation.EventSystem.EventRelationKind", "None", "ReplayOf", "CorrectionOf", "Supersedes");
CheckEnum(eventAssembly, "Foundation.EventSystem.EventPublicationDecisionKind", "Published", "Duplicate", "Rejected");
CheckType(eventAssembly, "Foundation.EventSystem.EventSubscription");
CheckType(eventAssembly, "Foundation.EventSystem.EventPublicationRequest");
CheckType(eventAssembly, "Foundation.EventSystem.PublishedEvent");

if (failures.Count > 0)
{
    Console.Error.WriteLine($"FOUNDATION COMPATIBILITY VERIFIER: FAIL ({checks - failures.Count}/{checks})");
    foreach (var failure in failures) Console.Error.WriteLine(" - " + failure);
    return 1;
}

Console.WriteLine($"FOUNDATION COMPATIBILITY VERIFIER: PASS ({checks}/{checks})");
Console.WriteLine($"ContractsAssembly={contractsAssembly.GetName().Name}");
Console.WriteLine($"StateAssembly={stateAssembly.GetName().Name}");
Console.WriteLine($"RoutingAssembly={routingAssembly.GetName().Name}");
Console.WriteLine($"DeliveryAssembly={deliveryAssembly.GetName().Name}");
Console.WriteLine($"EventAssembly={eventAssembly.GetName().Name}");
Console.WriteLine("Scope=TEST_ONLY_STRUCTURAL_COMPATIBILITY / NO_RUNTIME_BINDING_AUTHORITY");
return 0;

void CheckType(Assembly assembly, string fullName)
{
    checks++;
    if (assembly.GetType(fullName, false, false) is null) failures.Add($"Missing Foundation type: {fullName}");
}

void CheckEnum(Assembly assembly, string fullName, params string[] expectedNames)
{
    checks++;
    var type = assembly.GetType(fullName, false, false);
    if (type is null || !type.IsEnum)
    {
        failures.Add($"Missing Foundation enum: {fullName}");
        return;
    }

    var actual = Enum.GetNames(type).ToHashSet(StringComparer.Ordinal);
    foreach (var expected in expectedNames)
        if (!actual.Contains(expected)) failures.Add($"Foundation enum {fullName} missing value {expected}");
}

void CheckProperties(Assembly assembly, string fullName, params string[] names)
{
    checks++;
    var type = assembly.GetType(fullName, false, false);
    if (type is null)
    {
        failures.Add($"Missing Foundation type: {fullName}");
        return;
    }

    var actual = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(x => x.Name).ToHashSet(StringComparer.Ordinal);
    foreach (var name in names)
        if (!actual.Contains(name)) failures.Add($"Foundation type {fullName} missing property {name}");
}
