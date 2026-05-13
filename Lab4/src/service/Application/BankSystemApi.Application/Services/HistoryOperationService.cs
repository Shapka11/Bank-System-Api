using BankSystemApi.Application.Abstractions.Persistence;
using BankSystemApi.Application.Abstractions.Persistence.Queries;
using BankSystemApi.Application.Activities;
using BankSystemApi.Application.Contracts.HistoryOperations;
using BankSystemApi.Application.Contracts.HistoryOperations.Operations;
using BankSystemApi.Application.Mapping;
using BankSystemApi.Application.Specifications;
using BankSystemApi.Domain.Accounts;
using BankSystemApi.Domain.HistoryOperations;
using BankSystemApi.Domain.Users;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace BankSystemApi.Application.Services;

internal sealed class HistoryOperationService : IHistoryOperationService
{
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
        using Activity? activity = HistoryOperationServiceActivity.ActivitySource.StartActivity();
        activity?.SetTag("account.id", request.AccountId);
        activity?.SetTag("user.id", request.UserId);

        User? user = await _context.UsersRepository.FindByAuthorizationIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning("Unauthorized access attempt: User ID '{UserId}' is not exist.", request.UserId);
            return new GetHistoryOperations.Response.Unauthorized(request.UserId);
        }

        var accountId = new AccountId(request.AccountId);
        Account? account = await _context.AccountsRepository.FindAccountByIdAsync(accountId, cancellationToken);
        if (account is null)
        {
            _logger.LogWarning("Account with id {AccountId} not found.", accountId.Value);
            return new GetHistoryOperations.Response.AccountNotFound(accountId.Value);
        }

        if (account.UserId != user.Id)
        {
            _logger.LogWarning(
                "Account {AccountId} does to belong to the user {UserId}",
                accountId.Value,
                user.Id.Value);
            return new GetHistoryOperations.Response.Forbidden("Account is not this users");
        }

        HistoryOperationId? idCursor = request.PageToken?.Id is not null
            ? new HistoryOperationId(request.PageToken.Value.Id)
            : null;

        var query = HistoryOperationQuery.Build(builder => builder
            .WithAccountId(accountId)
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
}