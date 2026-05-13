using BankSystemApi.Application.Abstractions.Persistence;
using BankSystemApi.Application.Abstractions.Persistence.Results;
using BankSystemApi.Application.Activities;
using BankSystemApi.Application.Contracts.Users;
using BankSystemApi.Application.Contracts.Users.Operations;
using BankSystemApi.Application.Mapping;
using BankSystemApi.Application.Specifications;
using BankSystemApi.Domain.Users;
using Itmo.Dev.Platform.Common.DateTime;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace BankSystemApi.Application.Services;

internal sealed class UserService : IUserService
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

        var user = new User(UserId.Default, request.AuthorizationId, _dateTimeProvider.Current);

        AddUserResult result = await _context.UsersRepository.TryAddAsync([user], cancellationToken);
        if (result is AddUserResult.Success success)
        {
            _logger.LogInformation(
                "User {AuthorizationId} successfully added, with id = '{UserId}'",
                user.AuthorizationId,
                user.Id);

            return new AddUser.Response.Success(success.User.MapToDto());
        }

        if (result is AddUserResult.AlreadyExist)
        {
            User? dbUser = await _context.UsersRepository
                .FindByAuthorizationIdAsync(request.AuthorizationId, cancellationToken);
            if (dbUser is null)
            {
                _logger.LogError(
                    "Database inconsistency: User {AuthId} reported as existing but not found.",
                    request.AuthorizationId);

                throw new InvalidOperationException(
                    $"User with AuthId {request.AuthorizationId} not found after AlreadyExist result.");
            }

            _logger.LogInformation(
                "Unauthorized access attempt: User ID '{UserId}' is exist.",
                request.AuthorizationId);

            return new AddUser.Response.Success(user.MapToDto());
        }

        throw new UnreachableException();
    }
}