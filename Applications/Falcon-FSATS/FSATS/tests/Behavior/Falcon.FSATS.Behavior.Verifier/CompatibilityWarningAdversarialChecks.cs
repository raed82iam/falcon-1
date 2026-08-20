using TC = Falcon.FSATS.Trading.Contracts;

internal static class CompatibilityWarningAdversarialChecks
{
    internal static void Run()
    {
        var contractIdFields = typeof(TC.WebOnDemandAnalysisContractIds)
            .GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
        foreach (var field in contractIdFields.Where(x => x.Name is "ProjectionV1" or "CommandV1"))
        {
            if (field.IsDefined(typeof(ObsoleteAttribute), inherit: false))
                throw new InvalidOperationException($"HISTORICAL_CONTRACT_ID_COMPATIBILITY_FIELD_IS_OBSOLETE_WARNING:{field.Name}");
        }

        if (typeof(TC.WebOnDemandAnalysisProjection).IsDefined(typeof(ObsoleteAttribute), inherit: false))
            throw new InvalidOperationException("HISTORICAL_ANALYSIS_PROJECTION_TYPE_IS_OBSOLETE_WARNING");
        if (typeof(TC.WebOnDemandAnalysisCommand).IsDefined(typeof(ObsoleteAttribute), inherit: false))
            throw new InvalidOperationException("HISTORICAL_ANALYSIS_COMMAND_TYPE_IS_OBSOLETE_WARNING");

        var historicalConstructor = typeof(TC.WebOnDemandAnalysisRequest)
            .GetConstructors()
            .SingleOrDefault(x => x.GetParameters().Length == 6);
        if (historicalConstructor is null)
            throw new InvalidOperationException("HISTORICAL_ANALYSIS_REQUEST_CONSTRUCTOR_COMPATIBILITY_SURFACE_MISSING");
        if (historicalConstructor.IsDefined(typeof(ObsoleteAttribute), inherit: false))
            throw new InvalidOperationException("HISTORICAL_ANALYSIS_REQUEST_CONSTRUCTOR_IS_OBSOLETE_WARNING");
    }
}
