using System;
using System.Globalization;

namespace Foundation.Authority;

public sealed record RegisteredStandingOwnerPreapprovalDecision(
    bool AcceptedUnderStandingPreapproval,
    string Reason,
    string DecisionIdentitySha256,
    string RegistrationIdentitySha256,
    string ProfileIdentitySha256,
    string CandidateIdentitySha256,
    string UnderlyingAuthorityDecisionId,
    bool ExecutionAuthorized,
    bool DeploymentAuthorized,
    bool BusinessAuthorityGranted,
    DateTimeOffset DecisionTime,
    DateTimeOffset Expiry,
    string EvidenceReference);

public sealed class RegisteredStandingOwnerPreapprovalEvaluator
{
    private readonly StandingOwnerPreapprovalRegistry _registry;
    private readonly StandingOwnerPreapprovalEvaluator _profileEvaluator = new();

    public RegisteredStandingOwnerPreapprovalEvaluator(StandingOwnerPreapprovalRegistry registry)
    {
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
    }

    internal RegisteredStandingOwnerPreapprovalDecision Evaluate(
        string policyId,
        StandingOwnerPreapprovalCandidate? candidate,
        FitnessEvidence? fitness,
        DateTimeOffset observationTime)
    {
        if (!_registry.TryGet(policyId, out var registration) || registration is null)
        {
            return Deny(
                "STANDING_OWNER_PREAPPROVAL_REGISTRATION_REQUIRED",
                "NONE",
                "NONE",
                "NONE",
                observationTime,
                observationTime.AddTicks(1),
                "NONE");
        }

        if (registration.Revoked)
        {
            return Deny(
                StandingOwnerPreapprovalReason.ProfileRevoked,
                registration.IdentitySha256,
                registration.Profile.IdentitySha256,
                "NONE",
                observationTime,
                registration.Profile.Expiry > observationTime ? registration.Profile.Expiry : observationTime.AddTicks(1),
                registration.MutationAuthorization.MutationEvidenceReference);
        }

        var evaluation = _profileEvaluator.Evaluate(registration.Profile, candidate, fitness, observationTime);
        var decisionIdentity = StandingOwnerPreapprovalEvaluator.ComputeSha256(string.Join("\n",
            "registeredPreapproval.registration=" + registration.IdentitySha256,
            "registeredPreapproval.profileDecision=" + evaluation.DecisionIdentitySha256,
            "registeredPreapproval.accepted=" + (evaluation.AcceptedUnderStandingPreapproval ? "true" : "false"),
            "registeredPreapproval.reason=" + evaluation.Reason,
            "registeredPreapproval.observation=" + observationTime.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture),
            "registeredPreapproval.executionAuthorized=false",
            "registeredPreapproval.deploymentAuthorized=false",
            "registeredPreapproval.businessAuthorityGranted=false"));

        return new RegisteredStandingOwnerPreapprovalDecision(
            evaluation.AcceptedUnderStandingPreapproval,
            evaluation.Reason,
            decisionIdentity,
            registration.IdentitySha256,
            evaluation.ProfileIdentitySha256,
            evaluation.CandidateIdentitySha256,
            evaluation.UnderlyingAuthorityDecisionId,
            false,
            false,
            false,
            evaluation.DecisionTime,
            evaluation.Expiry,
            registration.MutationAuthorization.MutationEvidenceReference);
    }

    private static RegisteredStandingOwnerPreapprovalDecision Deny(
        string reason,
        string registrationIdentity,
        string profileIdentity,
        string candidateIdentity,
        DateTimeOffset decisionTime,
        DateTimeOffset expiry,
        string evidenceReference)
    {
        var decisionIdentity = StandingOwnerPreapprovalEvaluator.ComputeSha256(string.Join("\n",
            "registeredPreapproval.registration=" + registrationIdentity,
            "registeredPreapproval.profile=" + profileIdentity,
            "registeredPreapproval.candidate=" + candidateIdentity,
            "registeredPreapproval.accepted=false",
            "registeredPreapproval.reason=" + reason,
            "registeredPreapproval.observation=" + decisionTime.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)));

        return new RegisteredStandingOwnerPreapprovalDecision(
            false,
            reason,
            decisionIdentity,
            registrationIdentity,
            profileIdentity,
            candidateIdentity,
            "NONE",
            false,
            false,
            false,
            decisionTime,
            expiry,
            evidenceReference);
    }
}
