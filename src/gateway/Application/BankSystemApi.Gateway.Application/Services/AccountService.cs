using BankSystemApi.Gateway.Application.Abstractions.Accounts;
using BankSystemApi.Gateway.Application.Abstractions.Accounts.Operations;
using BankSystemApi.Gateway.Application.Contracts.Accounts;
using BankSystemApi.Gateway.Application.Contracts.Accounts.Operations.Requests;
using BankSystemApi.Gateway.Application.Contracts.Accounts.Operations.Responses;
using BankSystemApi.Gateway.Application.Mapping.Accounts;
using System.Diagnostics;

namespace BankSystemApi.Gateway.Application.Services;

public sealed class AccountService : IAccountService
{
    private readonly IAccountClient _accountClient;

    public AccountService(IAccountClient accountClient)
    {
        _accountClient = accountClient;
    }

    public async Task<CreateAccountResponse> CreateAsync(
        CreateAccountRequest request,
        CancellationToken cancellationToken)
    {
        var clientRequest = new CreateAccount.Request(
            request.CallerUserId,
            request.TargetUserId,
            request.AccountNumber,
            request.Password,
            request.AccountType.MapToBankModel());
        CreateAccount.Response clientResponse = await _accountClient.CreateAsync(clientRequest, cancellationToken);

        return clientResponse switch
        {
            CreateAccount.Response.Failure failure => new CreateAccountResponse.Failure(failure.ErrorMessage),
            CreateAccount.Response.Success success => new CreateAccountResponse.Success(success.BankAccount.MapToDto()),
            _ => throw new UnreachableException(),
        };
    }

    public async Task<DepositResponse> DepositAsync(DepositRequest request, CancellationToken cancellationToken)
    {
        var clientRequest = new Deposit.Request(request.UserId, request.AccountId, request.Amount);
        Deposit.Response clientResponse = await _accountClient.DepositAsync(clientRequest, cancellationToken);

        return clientResponse switch
        {
            Deposit.Response.Failure failure => new DepositResponse.Failure(failure.ErrorMessage),
            Deposit.Response.Success success => new DepositResponse.Success(success.BankAccount.MapToDto()),
            _ => throw new UnreachableException(),
        };
    }

    public async Task<WithdrawResponse> WithdrawAsync(WithdrawRequest request, CancellationToken cancellationToken)
    {
        var clientRequest = new Withdraw.Request(request.UserId, request.AccountId, request.Amount);
        Withdraw.Response clientResponse = await _accountClient.WithdrawAsync(clientRequest, cancellationToken);

        return clientResponse switch
        {
            Withdraw.Response.Failure failure => new WithdrawResponse.Failure(failure.ErrorMessage),
            Withdraw.Response.Success success => new WithdrawResponse.Success(success.BankAccount.MapToDto()),
            _ => throw new UnreachableException(),
        };
    }

    public async Task<GetBalanceResponse> GetBalanceAsync(
        GetBalanceRequest request,
        CancellationToken cancellationToken)
    {
        var clientRequest = new GetBalance.Request(request.UserId, request.AccountId);
        GetBalance.Response clientResponse = await _accountClient.GetBalanceAsync(clientRequest, cancellationToken);

        return clientResponse switch
        {
            GetBalance.Response.Failure failure => new GetBalanceResponse.Failure(failure.ErrorMessage),
            GetBalance.Response.Success success => new GetBalanceResponse.Success(success.Money),
            _ => throw new UnreachableException(),
        };
    }

    public async Task<GetAccountsResponse> GetAccountAsync(
        GetAccountsRequest request,
        CancellationToken cancellationToken)
    {
        var clientRequest = new GetAccounts.Request(request.UserId, request.PageSize, request.PageToken);
        GetAccounts.Response clientResponse = await _accountClient.GetAsync(clientRequest, cancellationToken);

        return clientResponse switch
        {
            GetAccounts.Response.Failure failure => new GetAccountsResponse.Failure(failure.ErrorMessage),
            GetAccounts.Response.Success success => new GetAccountsResponse.Success(
                success.BankAccounts.Select(ba => ba.MapToDto()).ToArray(),
                success.PageToken),
            _ => throw new UnreachableException(),
        };
    }
}