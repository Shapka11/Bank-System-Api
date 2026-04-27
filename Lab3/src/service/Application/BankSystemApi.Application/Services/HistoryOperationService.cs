using BankSystemApi.Application.Abstractions.Persistence;
using BankSystemApi.Application.Contracts.HistoryOperations;
using BankSystemApi.Application.Contracts.HistoryOperations.Operations;
using BankSystemApi.Application.Mapping;
using BankSystemApi.Application.Specifications;
using BankSystemApi.Domain.Accounts;
using BankSystemApi.Domain.HistoryOperations;
using BankSystemApi.Domain.Users;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using OperationHistoryQuery = BankSystemApi.Application.Abstractions.Persistence.Queries.OperationHistoryQuery;

namespace BankSystemApi.Application.Services;

public sealed partial class HistoryOperationService : IHistoryOperationService
{
    private static readonly ActivitySource ActivitySource =
        new("BankSystemApi.Application.Services.HistoryOperationService");

    private readonly IPersistenceContext _context;
    private readonly ILogger<HistoryOperationService> _logger;

    public HistoryOperationService(IPersistenceContext context, ILogger<HistoryOperationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<GetHistoryOperations.Response> GetAsync(
        GetHistoryOperations.Request request,
        CancellationToken cancellationToken)
    {
        using Activity? activity = ActivitySource.StartActivity();
        activity?.SetTag("account.id", request.AccountId);
        activity?.SetTag("user.id", request.UserId);

        User? user = await _context.UserRepository.FindByAuthorizationIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            LogUnauthorizedAttempt(request.UserId);
            return new GetHistoryOperations.Response.Unauthorized(request.UserId);
        }

        var accountId = new AccountId(request.AccountId);
        Account? account = await _context.AccountsRepository.FindAccountByIdAsync(accountId, cancellationToken);
        if (account is null)
        {
            LogAccountNotFound(accountId.Value);
            return new GetHistoryOperations.Response.AccountNotFound(accountId.Value);
        }

        if (account.UserId != user.Id)
        {
            LogAccountAccessForbidden(accountId.Value, user.Id.Value);
            return new GetHistoryOperations.Response.Forbidden("Account is not this users");
        }

        HistoryOperationId? idCursor = request.PageToken?.Id is not null
            ? new HistoryOperationId(request.PageToken.Value.Id)
            : null;

        var query = OperationHistoryQuery.Build(builder => builder
            .WithAccountId(new AccountId(request.AccountId))
            .WithIdCursor(idCursor)
            .WithPageSize(request.PageSize));

        HistoryOperation[] history = await _context.HistoryOperationsRepository
            .QueryAsync(query, cancellationToken)
            .ToArrayAsync(cancellationToken);

        GetHistoryOperations.PageToken? responsePageToken = history.Length < request.PageSize
            ? null
            : new GetHistoryOperations.PageToken(history.Last().Id.Value);

        return new GetHistoryOperations.Response.Success(
            history.Select(h => h.MapToDto()).ToArray(),
            responsePageToken);
    }

    [LoggerMessage(
        LogLevel.Warning,
        "Unauthorized access attempt: User ID '{UserId}' is not exist.")]
    public partial void LogUnauthorizedAttempt(Guid userId);

    [LoggerMessage(
        LogLevel.Warning,
        "Account with id {AccountId} not found.")]
    public partial void LogAccountNotFound(Guid accountId);

    [LoggerMessage(
        LogLevel.Warning,
        "Account {AccountId} does to belong to the user {UserId}")]
    public partial void LogAccountAccessForbidden(Guid accountId, long userId);
}