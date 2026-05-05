using BankSystemApi.Application.Abstractions.Persistence;
using BankSystemApi.Application.Activities;
using BankSystemApi.Application.Contracts.Users;
using BankSystemApi.Application.Contracts.Users.Operations;
using BankSystemApi.Application.Specifications;
using BankSystemApi.Domain.Users;
using Itmo.Dev.Platform.Common.DateTime;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace BankSystemApi.Application.Services;

public sealed class UserService : IUserService
{
    private readonly IPersistenceContext _context;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<UserService> _logger;

    public UserService(IPersistenceContext context, IDateTimeProvider dateTimeProvider, ILogger<UserService> logger)
    {
        _context = context;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
    }

    public async Task<AddUser.Response> AddAsync(AddUser.Request request, CancellationToken cancellationToken)
    {
        using Activity? activity = UserServiceActivity.ActivitySource.StartActivity();
        activity?.SetTag("user.id", request.AuthorizationId);

        User? user = await _context.UserRepository
            .FindByAuthorizationIdAsync(request.AuthorizationId, cancellationToken);
        if (user is not null)
        {
            _logger.LogWarning(
                "Unauthorized access attempt: User ID '{UserId}' is not exist.",
                request.AuthorizationId);
            return new AddUser.Response.Success();
        }

        user = new User(UserId.Default, request.AuthorizationId, _dateTimeProvider.Current);

        await _context.UserRepository.TryAddAsync([user], cancellationToken);

        _logger.LogInformation("User {UserId} successfully added", user.AuthorizationId);

        return new AddUser.Response.Success();
    }
}