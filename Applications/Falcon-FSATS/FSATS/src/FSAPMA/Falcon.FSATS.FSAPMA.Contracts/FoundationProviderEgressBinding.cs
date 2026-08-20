namespace Falcon.FSATS.FSAPMA.Contracts;
public sealed record FoundationProviderEgressBinding(string FoundationCandidate,string ApplicationId,string ProviderId,string ProviderAccountId,string ServiceRole,string Environment,string Purpose,string Destination,string CredentialReference,string RoutePolicyIdentity,string AuthorityEvidence,string QuotaEntitlementEvidence,bool RouteAuthorized,bool ConnectionExecuted,bool BusinessAuthorityGranted);
public sealed record ProviderEgressBindingDecision(bool Accepted,string ReasonCode,bool ConnectionExecutionAuthorized,bool BusinessAuthorityGranted,bool RuntimeActivationAuthorized);
public static class FoundationProviderEgressGovernance
{
 public const string Stage12Candidate="3e5977da254894afb29f39302cd7791612e44178";
 public const string ApplicationIdentity="FSAPMA";
 public const string OperationalPurpose="OPERATIONAL_PROVIDER_DATA";
 public static ProviderEgressBindingDecision Evaluate(FoundationProviderEgressBinding? x)
 {
  if(x is null||new[]{x.ApplicationId,x.ProviderId,x.ProviderAccountId,x.ServiceRole,x.Environment,x.Purpose,x.Destination,x.CredentialReference,x.RoutePolicyIdentity,x.AuthorityEvidence,x.QuotaEntitlementEvidence}.Any(string.IsNullOrWhiteSpace))return Reject("INCOMPLETE_PROVIDER_ROUTE_IDENTITY");
  if(!StringComparer.Ordinal.Equals(x.FoundationCandidate,Stage12Candidate))return Reject("FOUNDATION_STAGE12_CANDIDATE_MISMATCH");
  if(!StringComparer.Ordinal.Equals(x.ApplicationId,ApplicationIdentity))return Reject("WRONG_APPLICATION_IDENTITY");
  if(!StringComparer.Ordinal.Equals(x.Purpose,OperationalPurpose))return Reject("PURPOSE_SEPARATION_VIOLATION");
  if(x.ConnectionExecuted||x.BusinessAuthorityGranted)return Reject("ROUTE_BINDING_CANNOT_EXECUTE_OR_MINT_BUSINESS_AUTHORITY");
  if(!x.RouteAuthorized)return Reject("ROUTE_NOT_AUTHORIZED");
  return new(true,"FSAPMA_PROVIDER_ROUTE_BINDING_ACCEPTED",false,false,false);
 }
 private static ProviderEgressBindingDecision Reject(string r)=>new(false,r,false,false,false);
}
