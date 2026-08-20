using System.Runtime.CompilerServices;
using Foundation.Contracts;
using Foundation.SelfAwareness;

namespace Falcon.Stage7.WP05.Verifier;

internal static class Wp05ChallengeAuthorizationFixtures
{
    [ModuleInitializer]
    internal static void Run()
    {
        var t = Wp05FixtureSupport.T;
        var challenge = Wp05FixtureSupport.Challenge();
        Wp05FixtureSupport.Require(
            EvidenceAwarenessRuntime.ValidateChallenge(challenge, t).Result == ValidationResult.Pass,
            "WP05 coverage: valid challenge rejected.");

        var withoutAuthorization = challenge with { AuthorizationEvidenceReference = string.Empty };
        Wp05FixtureSupport.Require(
            EvidenceAwarenessRuntime.ValidateChallenge(withoutAuthorization, t).Result != ValidationResult.Pass,
            "WP05 coverage: challenge without authorization evidence accepted.");
    }
}
