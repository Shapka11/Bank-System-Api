using BankSystemApi.Accounts.Grpc;
using BankSystemApi.Gateway.Application.Abstractions.Accounts;
using BankSystemApi.Gateway.Application.Abstractions.Accounts.Operations;
using BankSystemApi.Gateway.Infrastructure.BankService.Activities;
using BankSystemApi.Gateway.Infrastructure.BankService.Extensions;
using BankSystemApi.Gateway.Infrastructure.BankService.Mapping;
using Google.Type;
using System.Diagnostics;

namespace BankSystemApi.Gateway.Infrastructure.BankService.Clients;

public sealed class AccountClient : IAccountClient
{
    private readonly AccountService.AccountServiceClient _accountClient;

    public AccountClient(AccountService.AccountServiceClient accountClient)
    {
        _accountClient = accountClient;
    }

    public async Task<CreateAccount.Response> CreateAsync(
        CreateAccount.Request request,
        CancellationToken cancellationToken)
    {
        using Activity? activity = AccountClientActivity.ActivitySource.StartActivity();
        activity.AddUserIdBaggage(request.CallerUserId);

        var clientRequest = new ProtoCreateAccountRequest(
            request.CallerUserId.ToString(),
            request.TargetUserId,
            request.AccountNumber,
            request.Password,
            request.AccountType.MapToProto());

        ProtoCreateAccountResponse clientResponse = await _accountClient.CreateAsync(
            clientRequest,
            cancellationToken: cancellationToken);

        return new CreateAccount.Response.Success(clientResponse.Account.MapToModel());
    }

    public async Task<Deposit.Response> DepositAsync(Deposit.Request request, CancellationToken cancellationToken)
    {
        using Activity? activity = AccountClientActivity.ActivitySource.StartActivity();
        activity.AddUserIdBaggage(request.UserId);
        activity.AddAccountIdBaggage(request.AccountId);

        var clientRequest = new ProtoDepositRequest(
            request.UserId.ToString(),
            request.AccountId,
            new Money { DecimalValue = request.Amount });

        ProtoDepositResponse clientResponse = await _accountClient.DepositAsync(
            clientRequest,
            cancellationToken: cancellationToken);

        return new Deposit.Response.Success(clientResponse.Account.MapToModel());
    }

    public async Task<Withdraw.Response> WithdrawAsync(Withdraw.Request request, CancellationToken cancellationToken)
    {
        using Activity? activity = AccountClientActivity.ActivitySource.StartActivity();
        activity.AddUserIdBaggage(request.UserId);
        activity.AddAccountIdBaggage(request.AccountId);

        var clientRequest = new ProtoWithdrawRequest(
            request.UserId.ToString(),
            request.AccountId,
            new Money { DecimalValue = request.Amount });

        ProtoWithdrawResponse clientResponse = await _accountClient.WithdrawAsync(
            clientRequest,
            cancellationToken: cancellationToken);

        return new Withdraw.Response.Success(clientResponse.Account.MapToModel());
    }

    public async Task<GetBalance.Response> GetBalanceAsync(
        GetBalance.Request request,
        CancellationToken cancellationToken)
    {
        using Activity? activity = AccountClientActivity.ActivitySource.StartActivity();
        activity.AddUserIdBaggage(request.UserId);
        activity.AddAccountIdBaggage(request.AccountId);

        var clientRequest = new ProtoGetBalanceRequest(request.UserId.ToString(), request.AccountId);

        ProtoGetBalanceResponse clientResponse = await _accountClient.GetBalanceAsync(
            clientRequest,
            cancellationToken: cancellationToken);

        return new GetBalance.Response.Success(clientResponse.Balance.DecimalValue);
    }

    public async Task<GetAccounts.Response> GetAsync(GetAccounts.Request request, CancellationToken cancellationToken)
    {
        using Activity? activity = AccountClientActivity.ActivitySource.StartActivity();
        activity.AddUserIdBaggage(request.UserId);

        var clientRequest = new ProtoGetAccountsRequest(
            request.UserId.ToString(),
            new ProtoPagination(request.PageSize, request.PageToken));

        ProtoGetAccountsResponse clientResponse = await _accountClient.GetAsync(
            clientRequest,
            cancellationToken: cancellationToken);

        return new GetAccounts.Response.Success(
            clientResponse.Accounts.Select(a => a.MapToModel()).ToArray(),
            clientResponse.PageToken);
    }
}