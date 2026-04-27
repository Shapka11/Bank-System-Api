using Microsoft.AspNetCore.Authorization;
using System.Runtime.CompilerServices;

namespace BankSystemApi.Gateway.Presentation.Http.Features;

public sealed class AuthorizeFeatureAttribute : AuthorizeAttribute
{
    public AuthorizeFeatureAttribute(string scope, [CallerMemberName] string? featureName = null)
    {
        Scope = scope;
        FeatureName = featureName;
        Policy = $"{Prefix}:{Scope}:{FeatureName}";
    }

    public static string Prefix => "Prefix";

    public string Scope { get; }

    public string? FeatureName { get; }
}