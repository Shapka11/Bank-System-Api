using Bsa.Gateway.Application.Abstractions.Accounts;
using Bsa.Gateway.Application.Abstractions.Accounts.Operations;
using Bsa.Gateway.Application.Contracts.Accounts;
using Bsa.Gateway.Application.Contracts.Accounts.Operations;
using Bsa.Gateway.Application.Mapping;
using System.Diagnostics;

namespace Bsa.Gateway.Application.Services;

public sealed class AccountService : IAccountService
{
    private readonly IAccountClient _accountClient;

    public AccountService(IAccountClient accountClient)
    {
        _accountClient = accountClient;
    }

    public async Task<CreateAccountResponse> CreateAsync(CreateAccountRequest request, CancellationToken cancellationToken)
    {
        var clientRequest = new CreateAccount.Request(request.Id, request.AccountNumber, request.Password);
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
        var clientRequest = new Deposit.Request(request.Id, request.Amount);
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
        var clientRequest = new Withdraw.Request(request.Id, request.Amount);
        Withdraw.Response clientResponse = await _accountClient.WithdrawAsync(clientRequest, cancellationToken);

        return clientResponse switch
        {
            Withdraw.Response.Failure failure => new WithdrawResponse.Failure(failure.ErrorMessage),
            Withdraw.Response.Success success => new WithdrawResponse.Success(success.BankAccount.MapToDto()),
            _ => throw new UnreachableException(),
        };
    }

    public async Task<GetBalanceResponse> GetBalanceAsync(GetBalanceRequest request, CancellationToken cancellationToken)
    {
        var clientRequest = new GetBalance.Request(request.Id);
        GetBalance.Response clientResponse = await _accountClient.GetBalanceAsync(clientRequest, cancellationToken);

        return clientResponse switch
        {
            GetBalance.Response.Failure failure => new GetBalanceResponse.Failure(failure.ErrorMessage),
            GetBalance.Response.Success success => new GetBalanceResponse.Success(success.Money),
            _ => throw new UnreachableException(),
        };
    }
}