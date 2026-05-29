using BankSystemApi.Gateway.Application.Contracts.Users;
using BankSystemApi.Gateway.Application.Contracts.Users.Operations;
using BankSystemApi.Gateway.Presentation.Http.Features;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Security.Claims;

namespace BankSystemApi.Gateway.Presentation.Http.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPresentationHttp(this IServiceCollection collection)
    {
        collection.AddControllers();
        return collection;
    }

    public static IServiceCollection AddServerSettingsAuthentication(
        this IServiceCollection collection,
        ConfigurationManager configurations)
    {
        collection
            .AddAuthentication(auth =>
            {
                auth.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                auth.DefaultScheme = "composite";
            })
            .AddPolicyScheme(
                "composite",
                "composite",
                options =>
                {
                    options.ForwardDefaultSelector = context =>
                    {
                        if (context.Request.Headers.Authorization.ToString().StartsWith("Bearer"))
                        {
                            return JwtBearerDefaults.AuthenticationScheme;
                        }

                        return CookieAuthenticationDefaults.AuthenticationScheme;
                    };
                })
            .AddCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            })
            .AddOpenIdConnect(oidc =>
            {
                oidc.Authority = configurations["Authentication:IdentityProviderUri"];
                oidc.ClientId = configurations["Authentication:ClientId"];
                oidc.ClientSecret = configurations["Authentication:ClientSecret"];

                oidc.ResponseType = "code";
                oidc.SaveTokens = true;

                oidc.RequireHttpsMetadata = false; // debug

                oidc.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(10),
                };

                oidc.Events = new OpenIdConnectEvents
                {
                    OnTokenValidated = async context => await ProcessUserSync(context.Principal, context.HttpContext),
                };
            })
            .AddJwtBearer(jwt =>
            {
                jwt.Authority = configurations["Authentication:IdentityProviderUri"];
                jwt.Audience = "account";
                jwt.ClaimsIssuer = "master";

                jwt.RequireHttpsMetadata = false; // debug

                jwt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(10),
                };
            });

        return collection;
    }

    public static IServiceCollection AddSwagger(this IServiceCollection collection, ConfigurationManager configurations)
    {
        collection.AddSwaggerGen(swagger =>
        {
            swagger.AddSecurityDefinition(
                "oidc",
                new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        AuthorizationCode = new OpenApiOAuthFlow
                        {
                            AuthorizationUrl =
                                new Uri(
                                    $"{configurations["Authentication:IdentityProviderUri"]}/protocol/openid-connect/auth"),
                            TokenUrl = new Uri(
                                $"{configurations["Authentication:IdentityProviderUri"]}/protocol/openid-connect/token"),
                        },
                    },
                });

            swagger.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecuritySchemeReference("oidc", doc),
                    ["openid", "profile"]
                },
            });
        });

        return collection;
    }

    public static IServiceCollection AddServerSettingsAuthorization(this IServiceCollection collection)
    {
        collection.AddSingleton<IAuthorizationHandler, FeatureHandler>();
        collection.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();

        return collection;
    }

    private static async Task ProcessUserSync(ClaimsPrincipal? principal, HttpContext httpContext)
    {
        string? authorizationId = principal?.FindFirst("sub")?.Value
                                  ?? principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (authorizationId is null)
            return;

        IUserService userService = httpContext.RequestServices.GetRequiredService<IUserService>();
        var request = new AddUserRequest(Guid.Parse(authorizationId));
        await userService.AddAsync(request, CancellationToken.None);
    }
}