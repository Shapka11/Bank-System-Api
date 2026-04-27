using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace BankSystemApi.Gateway.Presentation.Http.Features;

public sealed class AuthorizationPolicyProvider : IAuthorizationPolicyProvider
{
    private readonly DefaultAuthorizationPolicyProvider _defaultPolicyProvider;

    public AuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
    {
        _defaultPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
    }

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (policyName.StartsWith($"{AuthorizeFeatureAttribute.Prefix}:", StringComparison.OrdinalIgnoreCase))
        {
            string[] parts = policyName.Split(':');

            if (parts.Length == 3)
            {
                string scope = parts[1];
                string featureName = parts[2];

                AuthorizationPolicy policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddRequirements(new FeatureRequirement(scope, featureName))
                    .Build();

                return Task.FromResult<AuthorizationPolicy?>(policy);
            }
        }

        return _defaultPolicyProvider.GetPolicyAsync(policyName);
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
    {
        return _defaultPolicyProvider.GetDefaultPolicyAsync();
    }

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
    {
        return _defaultPolicyProvider.GetFallbackPolicyAsync();
    }
}