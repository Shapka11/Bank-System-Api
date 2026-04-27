using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BankSystemApi.Gateway.Presentation.Http.Features;

public sealed class FeatureHandler : AuthorizationHandler<FeatureRequirement>
{
    private readonly IConfiguration _configuration;

    public FeatureHandler(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, FeatureRequirement requirement)
    {
        FeatureRoles? featureRoles = _configuration.GetSection("FeatureRoles").Get<FeatureRoles>();
        if (featureRoles is null)
        {
            return Task.CompletedTask;
        }

        if (featureRoles.TryGetValue(requirement.Scope, out Dictionary<string, List<string>>? scopeFeatures) is false)
        {
            return Task.CompletedTask;
        }

        if (scopeFeatures.TryGetValue(requirement.FeatureName, out List<string>? allowedRoles) is false)
        {
            return Task.CompletedTask;
        }

        IEnumerable<string> userRoles = context.User.FindAll(ClaimTypes.Role).Select(c => c.Value);

        if (userRoles.Any(userRole => allowedRoles.Contains(userRole)))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}