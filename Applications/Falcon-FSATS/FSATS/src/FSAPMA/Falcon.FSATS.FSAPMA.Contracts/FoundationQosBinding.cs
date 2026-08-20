namespace Falcon.FSATS.FSAPMA.Contracts;
public sealed record FoundationQosBinding(string FoundationCandidate,string DeadlineId,string TrafficClass,string EvidenceReference,DateTimeOffset ObservedAt,bool BusinessAuthorityGranted,bool DeploymentAuthorized,bool FastTrackBusinessAuthorityGranted);
public sealed record QosBindingDecision(bool Accepted,string ReasonCode,bool BusinessAuthorityGranted,bool DeploymentAuthorized);
public static class FoundationQosGovernance
{
 public const string Stage11Candidate="165ce895ea059510e9b1a1a29c8d15254a18c283";
 public static QosBindingDecision Evaluate(FoundationQosBinding? x)
 {
  if(x is null||string.IsNullOrWhiteSpace(x.DeadlineId)||string.IsNullOrWhiteSpace(x.TrafficClass)||string.IsNullOrWhiteSpace(x.EvidenceReference)||x.ObservedAt==default)return R("INVALID_QOS_BINDING");
  if(!StringComparer.Ordinal.Equals(x.FoundationCandidate,Stage11Candidate))return R("FOUNDATION_STAGE11_CANDIDATE_MISMATCH");
  if(x.BusinessAuthorityGranted||x.DeploymentAuthorized||x.FastTrackBusinessAuthorityGranted)return R("QOS_CANNOT_MINT_BUSINESS_DEPLOYMENT_OR_FAST_TRACK_AUTHORITY");
  return new(true,"QOS_DEADLINE_OBSERVABILITY_BOUND",false,false);
 }
 private static QosBindingDecision R(string r)=>new(false,r,false,false);
}
