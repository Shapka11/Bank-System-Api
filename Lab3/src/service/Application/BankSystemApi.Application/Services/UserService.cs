using BankSystemApi.Application.Abstractions.Persistence;
using BankSystemApi.Application.Contracts.Users;
using BankSystemApi.Application.Contracts.Users.Operations;
using BankSystemApi.Application.Specifications;
using BankSystemApi.Domain.Users;
using Itmo.Dev.Platform.Common.DateTime;
using Microsoft.Extensions.Logging;

namespace BankSystemApi.Application.Services;

public sealed partial class UserService : IUserService
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
        User? user = await _context.UserRepository
            .FindByAuthorizationIdAsync(request.AuthorizationId, cancellationToken);
        if (user is not null)
        {
            LogUnauthorizedAttempt(request.AuthorizationId);
            return new AddUser.Response.Success();
        }

        user = new User(UserId.Default, request.AuthorizationId, _dateTimeProvider.Current);

        await _context.UserRepository.TryAddAsync([user], cancellationToken);

        LogUserAddedSuccess(user.AuthorizationId);

        return new AddUser.Response.Success();
    }

    [LoggerMessage(
        LogLevel.Warning,
        "Unauthorized access attempt: User ID '{UserId}' is not exist.")]
    public partial void LogUnauthorizedAttempt(Guid userId);

    [LoggerMessage(
        LogLevel.Information,
        "User {UserId} successfully added")]
    public partial void LogUserAddedSuccess(Guid userId);
}