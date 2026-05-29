using Microsoft.AspNetCore.Authorization;

namespace BankSystemApi.Gateway.Presentation.Http.Features;

public sealed class FeatureRequirement : IAuthorizationRequirement
{
    public FeatureRequirement(string scope, string featureName)
    {
        FeatureName = featureName;
        Scope = scope;
    }

    public string FeatureName { get; }

    public string Scope { get; }
}