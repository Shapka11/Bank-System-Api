using Bsa.Cli.Application.Abstractions.User;
using Bsa.Cli.Application.Abstractions.User.Models;
using Bsa.Cli.Application.Abstractions.User.Operations;
using Bsa.Cli.Infrastructure.ClientService.Users.Models;
using Bsa.Cli.Infrastructure.ClientService.Users.Options;
using Microsoft.Extensions.Options;
using Refit;

namespace Bsa.Cli.Infrastructure.ClientService.Users;

public sealed class UserClient : IUserClient
{
    private readonly IRefitUserClient _client;
    private readonly IOptionsMonitor<UserClientOptions> _options;

    public UserClient(IRefitUserClient client, IOptionsMonitor<UserClientOptions> options)
    {
        _client = client;
        _options = options;
    }

    public async Task<LoginUserQuery.Result> LoginAsync(LoginUserQuery.Request request, CancellationToken cancellationToken)
    {
        var httpRequest = new LoginUserRequest(request.AccountNumber, request.Password);
        IApiResponse<LoginUserResponse> response = await _client.LoginAsync(httpRequest, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            LoginUserResponse? loginUserResponse = response.Content;
            if (loginUserResponse is null)
                return new LoginUserQuery.Result.Failure("Empty response");

            Guid sessionId = loginUserResponse.Id;

            return new LoginUserQuery.Result.Success(sessionId);
        }

        string errorMessage = response.Error?.Content ?? response.ReasonPhrase ?? "Unknown error";
        return new LoginUserQuery.Result.Failure(errorMessage);
    }

    public async Task<LogoutUserQuery.Result> LogoutAsync(LogoutUserQuery.Request request, CancellationToken cancellationToken)
    {
        var httpRequest = new LogoutUserRequest(request.SessionId);
        IApiResponse response = await _client.LogoutAsync(httpRequest, cancellationToken);

        if (response.IsSuccessful)
        {
            return new LogoutUserQuery.Result.Success();
        }

        string errorMessage = response.Error?.Content ?? response.ReasonPhrase ?? "Unknown error";
        return new LogoutUserQuery.Result.Failure(errorMessage);
    }

    public async Task<DepositQuery.Result> DepositAsync(DepositQuery.Request request, CancellationToken cancellationToken)
    {
        var httpRequest = new DepositRequest(request.Id, request.Amount);
        IApiResponse response = await _client.DepositAsync(httpRequest, cancellationToken);

        if (response.IsSuccessful)
        {
            return new DepositQuery.Result.Success();
        }

        string errorMessage = response.Error?.Content ?? response.ReasonPhrase ?? "Unknown error";
        return new DepositQuery.Result.Failure(errorMessage);
    }

    public async Task<WithdrawQuery.Result> WithdrawAsync(WithdrawQuery.Request request, CancellationToken cancellationToken)
    {
        var httpRequest = new WithdrawRequest(request.Id, request.Amount);
        IApiResponse response = await _client.WithdrawAsync(httpRequest, cancellationToken);

        if (response.IsSuccessful)
        {
            return new WithdrawQuery.Result.Success();
        }

        string errorMessage = response.Error?.Content ?? response.ReasonPhrase ?? "Unknown error";
        return new WithdrawQuery.Result.Failure(errorMessage);
    }

    public async Task<GetBalanceQuery.Result> GetBalanceAsync(
        GetBalanceQuery.Request request,
        CancellationToken cancellationToken)
    {
        var httpRequest = new GetBalanceRequest(request.Id);
        IApiResponse<GetBalanceResponse> response = await _client.GetBalanceAsync(httpRequest, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            GetBalanceResponse? depositResponce = response.Content;
            if (depositResponce is null)
                return new GetBalanceQuery.Result.Failure("Empty response");

            return new GetBalanceQuery.Result.Success(depositResponce.Money);
        }

        string errorMessage = response.Error?.Content ?? response.ReasonPhrase ?? "Unknown error";
        return new GetBalanceQuery.Result.Failure(errorMessage);
    }

    public async Task<GetHistoryQuery.Result> GetHistoryAsync(
        GetHistoryQuery.Request request,
        CancellationToken cancellationToken)
    {
        var history = new List<AccountOperationEntity>();
        string? currentPageToken = null;
        int pageSize = _options.CurrentValue.PageSize;

        do
        {
            IApiResponse<GetHistoryResponse> response = await _client.GetHistoryAsync(
                new GetHistoryRequest(request.Id, currentPageToken, pageSize),
                cancellationToken);

            if (response.IsSuccessStatusCode is false)
                return new GetHistoryQuery.Result.Failure(response.Error?.Content ?? response.ReasonPhrase ?? "Unknown error");

            if (response.Content is null)
                return new GetHistoryQuery.Result.Failure("Empty response");

            history.AddRange(response.Content.History);
            currentPageToken = response.Content.PageToken;
        }
        while (currentPageToken is not null);

        return new GetHistoryQuery.Result.Success(history);
    }
}