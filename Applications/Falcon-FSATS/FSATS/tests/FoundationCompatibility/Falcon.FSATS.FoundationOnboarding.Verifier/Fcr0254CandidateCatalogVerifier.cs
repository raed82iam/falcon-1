using System.Runtime.CompilerServices;

internal static class Fcr0254CandidateCatalogVerifier
{
    [ModuleInitializer]
    internal static void Verify()
    {
        var checks = new List<(string Name, bool Pass)>();
        void Check(string name, bool pass) => checks.Add((name, pass));

        var pairs = Fcr0254CandidateCatalog.All;
        Check("exactly five request pairs materialized", pairs.Count == 5);
        Check("five unique admission identities", pairs.Select(x => x.AdmissionRequest.Identity).Distinct(StringComparer.Ordinal).Count() == 5);
        Check("five unique admission ids", pairs.Select(x => x.AdmissionRequest.AdmissionId).Distinct(StringComparer.Ordinal).Count() == 5);
        Check("five unique runtime instance candidates", pairs.Select(x => x.RuntimeRegistrationRequest.RuntimeInstanceId).Distinct(StringComparer.Ordinal).Count() == 5);

        foreach (var pair in pairs)
        {
            var admission = pair.AdmissionRequest;
            var manifest = admission.Manifest;
            var runtime = pair.RuntimeRegistrationRequest;

            Check(admission.Identity + " admission kind", admission.AdmissionKind == "APPLICATION");
            Check(admission.Identity + " identity binding", admission.Identity == manifest.ApplicationIdentity && admission.Version == manifest.ApplicationVersion && admission.Owner == manifest.ApplicationOwner);
            Check(admission.Identity + " CON-023 exact baseline", admission.ContractId == "CON-023" && admission.ContractVersion == "1.1" && admission.AuthoritySource == "CON-000 / CON-023");
            Check(admission.Identity + " APP-001 exact baseline", manifest.RequiredFoundationSpecifications.Count == 1 && manifest.RequiredFoundationSpecifications[0].Identity == "APP-001" && manifest.RequiredFoundationSpecifications[0].Version == "1.0");
            Check(admission.Identity + " manifest digest deterministic", admission.ManifestDigest.Length == 64 && admission.ManifestDigest == manifest.ComputeFoundationCanonicalDigest());
            Check(admission.Identity + " provenance digest present", admission.ProvenanceDigest.Length == 64 && !string.IsNullOrWhiteSpace(admission.ProvenanceContent));
            Check(admission.Identity + " bootstrap defined", admission.BootstrapContextState == "DEFINED");
            Check(admission.Identity + " provider boundary fail closed", !string.IsNullOrWhiteSpace(admission.ProviderBoundary) && !admission.ProviderBoundary.Contains("bypass", StringComparison.OrdinalIgnoreCase) && !admission.ProviderBoundary.Contains("unapproved", StringComparison.OrdinalIgnoreCase));
            Check(admission.Identity + " one MSA", manifest.MsaDeclarations.Count == 1);
            Check(admission.Identity + " branch and LSA cardinality", manifest.MajorBranchDeclarations.Count > 0 && manifest.MajorBranchDeclarations.Count == manifest.LsaDeclarations.Count);
            Check(admission.Identity + " branch ownership exact", manifest.MajorBranchDeclarations.All(branch => manifest.LsaDeclarations.Count(lsa => lsa.BranchName == branch.BranchName && lsa.ResponsibleLsa == branch.ResponsibleLsa) == 1));
            Check(admission.Identity + " provided capabilities nonempty", manifest.ProvidedCapabilities.Count > 0 && manifest.ProvidedCapabilities.All(value => !string.IsNullOrWhiteSpace(value)));
            Check(admission.Identity + " permissions nonempty", manifest.RequestedPermissions.Count > 0 && manifest.RequestedPermissions.All(value => !string.IsNullOrWhiteSpace(value.Name) && !string.IsNullOrWhiteSpace(value.Scope) && !string.IsNullOrWhiteSpace(value.Rationale)));
            Check(admission.Identity + " authority requests remain requests", manifest.AuthorityRequests.Count > 0 && manifest.AuthorityRequests.All(value => value.Scope == "REQUEST_ONLY_NO_IMPLICIT_GRANT"));

            Check(admission.Identity + " runtime identity binding", runtime.ApplicationIdentity == admission.Identity && runtime.ApplicationVersion == admission.Version);
            Check(admission.Identity + " runtime does not execute", !runtime.ExecutesRegistration);
            Check(admission.Identity + " runtime grants no stronger authority", !runtime.GrantsActivation && !runtime.GrantsDeployment && !runtime.GrantsProduction && !runtime.GrantsBusinessAuthority);
            Check(admission.Identity + " artifact activation remains false", !runtime.ArtifactConsumption.ActivationAuthorized && !runtime.ArtifactConsumption.DeploymentAuthorized && !runtime.ArtifactConsumption.ProductionAuthorized && !runtime.ArtifactConsumption.BusinessAuthorityGranted && !runtime.ArtifactConsumption.SilentUpgradePerformed);
            Check(admission.Identity + " exact artifact binds at execution", ValidBinding(runtime.ExpectedArtifactExactIdentity));
            Check(admission.Identity + " technical consumption binds at execution", ValidBinding(runtime.ArtifactConsumption.AcceptedForTechnicalConsumption));
            Check(admission.Identity + " admission truth binds at execution", ValidBinding(runtime.Admission.Admitted) && ValidBinding(runtime.Admission.EvidenceIdentity));
            Check(admission.Identity + " lifecycle truth binds at execution", runtime.LifecycleEligibility.Kind == "Attach" && ValidBinding(runtime.LifecycleEligibility.Eligible) && ValidBinding(runtime.LifecycleEligibility.CurrentVersion) && ValidBinding(runtime.LifecycleEligibility.DecisionIdentity));
            Check(admission.Identity + " current resource grants bind at execution", runtime.ResourceGrants.Count == 1 && ValidBinding(runtime.ResourceGrants[0].CurrentFoundationResourceGrants));
            Check(admission.Identity + " observed time binds at execution", ValidBinding(runtime.ObservedAt));
            Check(admission.Identity + " runtime capabilities concrete", runtime.ProvidedCapabilities.Count > 0 && runtime.ProvidedCapabilities.All(value => !string.IsNullOrWhiteSpace(value.CapabilityId) && value.Visibility == "Private" && !value.Exclusive));
        }

        var failed = checks.Where(x => !x.Pass).ToArray();
        if (failed.Length > 0)
        {
            Console.Error.WriteLine($"FCR0254 REQUEST MATERIALIZATION VERIFIER: FAIL ({checks.Count - failed.Length}/{checks.Count})");
            foreach (var failure in failed) Console.Error.WriteLine(" - " + failure.Name);
            throw new InvalidOperationException("FCR-0254 request candidate materialization verification failed.");
        }

        Console.WriteLine($"FCR0254 REQUEST MATERIALIZATION VERIFIER: PASS ({checks.Count}/{checks.Count})");
        Console.WriteLine("REQUEST_PAIRS=5 / ADMISSION_CANDIDATES=5 / RUNTIME_REGISTRATION_TEMPLATES=5");
        Console.WriteLine("RUNTIME_CURRENT_EVIDENCE=AUTHORITATIVE_BIND_AT_EXECUTION_ONLY / ACTUAL_ADMISSION=NOT_EXECUTED / ACTUAL_REGISTRATION=NOT_EXECUTED");
    }

    private static bool ValidBinding(BindAtExecution binding)
        => binding is not null
           && !string.IsNullOrWhiteSpace(binding.Field)
           && !string.IsNullOrWhiteSpace(binding.AuthoritativeSource)
           && !string.IsNullOrWhiteSpace(binding.Reason);
}
