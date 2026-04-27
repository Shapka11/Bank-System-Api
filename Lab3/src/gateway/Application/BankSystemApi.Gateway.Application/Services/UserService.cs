using BankSystemApi.Gateway.Application.Abstractions.Users;
using BankSystemApi.Gateway.Application.Abstractions.Users.Operations;
using BankSystemApi.Gateway.Application.Contracts.Users;
using BankSystemApi.Gateway.Application.Contracts.Users.Operations;
using Microsoft.Extensions.Caching.Hybrid;

namespace BankSystemApi.Gateway.Application.Services;

public sealed class UserService : IUserService
{
    private readonly IUserClient _userClient;
    private readonly HybridCache _cache;

    public UserService(IUserClient userClient, HybridCache cache)
    {
        _userClient = userClient;
        _cache = cache;
    }

    public async Task<AddUserResponse> AddAsync(AddUserRequest request, CancellationToken cancellationToken)
    {
        string cacheKey = $"UserAuthId:{request.AuthorizationId.ToString()}";

        var options = new HybridCacheEntryOptions
        {
            Expiration = TimeSpan.FromMinutes(5),
            LocalCacheExpiration = TimeSpan.FromMinutes(1),
        };

        await _cache.GetOrCreateAsync(
            cacheKey,
            (request, service: this),
            static async (state, ct) =>
            {
                var clientRequest = new AddUser.Request(state.request.AuthorizationId);
                AddUser.Response response = await state.service._userClient.AddAsync(clientRequest, ct);

                if (response is AddUser.Response.Failure failure)
                {
                    throw new ArgumentException(failure.ErrorMessage);
                }

                return string.Empty;
            },
            options: options,
            cancellationToken: cancellationToken);

        return new AddUserResponse.Success();
    }
}