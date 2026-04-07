using Bsa.Application.Abstractions.Persistence;
using Bsa.Application.Abstractions.Persistence.Queries;
using Bsa.Application.Contracts.HistoryOperations;
using Bsa.Application.Contracts.HistoryOperations.Operations;
using Bsa.Application.Mapping;
using Bsa.Domain.Accounts;
using Bsa.Domain.HistoryOperations;
using Bsa.Domain.Sessions;

namespace Bsa.Application.Services;

public sealed class HistoryOperationService : IHistoryOperationService
{
    private readonly IPersistenceContext _context;

    public HistoryOperationService(IPersistenceContext context)
    {
        _context = context;
    }

    public async Task<GetHistoryOperations.Response> GetAsync(
        GetHistoryOperations.Request request,
        CancellationToken cancellationToken)
    {
        SessionBase? session = await _context.SessionRepository
            .FindSessionByIdAsync(request.Id, cancellationToken);

        if (session is not UserSession userSession)
            return new GetHistoryOperations.Response.Unauthorized(request.Id, "Session is not user");

        AccountId? accountIdCursor = request.PageToken?.Id is not null
            ? new AccountId(request.PageToken.Value.Id)
            : null;

        var query = AccountOperationQuery.Build(builder => builder
            .WithAccountId(userSession.AccountId)
            .WithIdCursor(accountIdCursor)
            .WithPageSize(request.PageSize));

        HistoryOperation[] history = await _context.HistoryOperationsRepository
            .QueryAsync(query, cancellationToken)
            .ToArrayAsync(cancellationToken);

        GetHistoryOperations.PageToken? responsePageToken = history.Length < request.PageSize
            ? null
            : new GetHistoryOperations.PageToken(history.Last().Id.Value);

        return new GetHistoryOperations.Response.Success(history.MapToDto(), responsePageToken);
    }
}