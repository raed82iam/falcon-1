using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Foundation.Contracts.ResourceGovernance;

namespace Falcon.Stage6.WP01.Verifier;

internal static class RequesterInstanceReconciliationChecks
{
    [ModuleInitializer]
    internal static void ValidateRequesterInstancePrimitive()
    {
        var instance = new ResourceRequesterInstanceId("resource-controller.instance-a");
        if (!StringComparer.Ordinal.Equals(instance.Value, "resource-controller.instance-a"))
            throw new InvalidOperationException("Requester instance identity was not preserved exactly.");
        Console.WriteLine("PASS requester_instance_identity_validation");

        if (typeof(ResourceRequesterInstanceId) == typeof(ResourceRequesterRoleId)
            || typeof(ResourceRequesterInstanceId) == typeof(ApplicationPrincipalId))
            throw new InvalidOperationException("Application, requester-role and requester-instance identities must remain distinct types.");
        Console.WriteLine("PASS requester_instance_role_application_identity_separation");

        var authorityLeak = typeof(ResourceRequesterInstanceId)
            .GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Any(member => member.Name.Contains("Authorize", StringComparison.OrdinalIgnoreCase)
                || member.Name.Contains("Grant", StringComparison.OrdinalIgnoreCase)
                || member.Name.Contains("Allow", StringComparison.OrdinalIgnoreCase));
        if (authorityLeak)
            throw new InvalidOperationException("Requester instance identity must not mint or expose resource authority.");
        Console.WriteLine("PASS requester_instance_identity_does_not_create_authority");

        if (typeof(ResourceRequesterInstanceId) == typeof(ResourceEpochId))
            throw new InvalidOperationException("Requester instance identity and fencing/epoch identity must remain distinct.");
        Console.WriteLine("PASS requester_instance_and_epoch_are_distinct");
    }
}
