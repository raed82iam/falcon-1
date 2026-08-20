using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Foundation.Contracts.ResourceGovernance;

namespace Falcon.Stage6.WP01.Verifier;

internal static class RequesterRoleReconciliationChecks
{
    [ModuleInitializer]
    internal static void ValidateRequesterRolePrimitive()
    {
        var role = new ResourceRequesterRoleId("resource-controller.primary");
        if (!StringComparer.Ordinal.Equals(role.Value, "resource-controller.primary"))
            throw new InvalidOperationException("Requester role identity was not preserved exactly.");
        Console.WriteLine("PASS requester_role_identity_validation");

        if (typeof(ResourceRequesterRoleId) == typeof(ApplicationPrincipalId))
            throw new InvalidOperationException("Application identity and requester role identity must remain distinct types.");
        Console.WriteLine("PASS application_identity_and_requester_role_are_distinct");

        var authorityLeak = typeof(ResourceRequesterRoleId)
            .GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Any(member => member.Name.Contains("Authorize", StringComparison.OrdinalIgnoreCase)
                || member.Name.Contains("Grant", StringComparison.OrdinalIgnoreCase)
                || member.Name.Contains("Allow", StringComparison.OrdinalIgnoreCase));
        if (authorityLeak)
            throw new InvalidOperationException("Requester role identity must not mint or expose resource authority.");
        Console.WriteLine("PASS requester_role_identity_does_not_create_authority");
    }
}
