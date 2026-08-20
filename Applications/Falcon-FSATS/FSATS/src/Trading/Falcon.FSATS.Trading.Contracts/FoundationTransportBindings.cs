namespace Falcon.FSATS.Trading.Contracts;

public sealed record FoundationQosBinding(string FoundationCandidate,string DeadlineId,string TrafficClass,string EvidenceReference,DateTimeOffset ObservedAt,bool TechnicalRouteAuthorized,bool BusinessAuthorityGranted,bool DeploymentAuthorized);
public sealed record FoundationBrokerEgressBinding(string FoundationCandidate,string BrokerId,string BrokerAccountId,string Environment,string Purpose,string Destination,string CredentialReference,string AuthorityEvidence,string RoutePolicyIdentity,bool RouteAuthorized,bool ConnectionExecuted,bool OrderAuthorityGranted,bool LiveAuthorityGranted);
public sealed record FoundationBindingDecision(bool Accepted,string ReasonCode,bool RuntimeActivationAuthorized,bool BusinessAuthorityGranted,bool ConnectionExecutionAuthorized,bool LiveAuthorityGranted);

public static class FoundationTradingBindings
{
    public const string Stage11Candidate="165ce895ea059510e9b1a1a29c8d15254a18c283";
    public const string Stage12Candidate="3e5977da254894afb29f39302cd7791612e44178";
    public const string BrokerExecutionPurpose="BROKER_EXECUTION";
    public static FoundationBindingDecision EvaluateQos(FoundationQosBinding? x)
    {
        if(x is null||string.IsNullOrWhiteSpace(x.DeadlineId)||string.IsNullOrWhiteSpace(x.TrafficClass)||string.IsNullOrWhiteSpace(x.EvidenceReference)||x.ObservedAt==default)return Reject("INVALID_QOS_BINDING");
        if(!StringComparer.Ordinal.Equals(x.FoundationCandidate,Stage11Candidate))return Reject("FOUNDATION_STAGE11_CANDIDATE_MISMATCH");
        if(x.BusinessAuthorityGranted||x.DeploymentAuthorized)return Reject("QOS_OR_OBSERVABILITY_CANNOT_MINT_BUSINESS_OR_DEPLOYMENT_AUTHORITY");
        return new(true,"QOS_DEADLINE_OBSERVABILITY_BOUND",false,false,false,false);
    }
    public static FoundationBindingDecision EvaluateBroker(FoundationBrokerEgressBinding? x)
    {
        if(x is null||string.IsNullOrWhiteSpace(x.BrokerId)||string.IsNullOrWhiteSpace(x.BrokerAccountId)||string.IsNullOrWhiteSpace(x.Environment)||string.IsNullOrWhiteSpace(x.Purpose)||string.IsNullOrWhiteSpace(x.Destination)||string.IsNullOrWhiteSpace(x.CredentialReference)||string.IsNullOrWhiteSpace(x.AuthorityEvidence)||string.IsNullOrWhiteSpace(x.RoutePolicyIdentity))return Reject("INCOMPLETE_BROKER_ROUTE_IDENTITY");
        if(!StringComparer.Ordinal.Equals(x.FoundationCandidate,Stage12Candidate))return Reject("FOUNDATION_STAGE12_CANDIDATE_MISMATCH");
        if(!StringComparer.Ordinal.Equals(x.Purpose,BrokerExecutionPurpose))return Reject("PURPOSE_SEPARATION_VIOLATION");
        if(x.ConnectionExecuted||x.OrderAuthorityGranted||x.LiveAuthorityGranted)return Reject("ROUTE_BINDING_CANNOT_EXECUTE_OR_MINT_ORDER_LIVE_AUTHORITY");
        if(!x.RouteAuthorized)return Reject("ROUTE_NOT_AUTHORIZED");
        return new(true,"BROKER_ROUTE_BINDING_ACCEPTED",false,false,false,false);
    }
    private static FoundationBindingDecision Reject(string r)=>new(false,r,false,false,false,false);
}
