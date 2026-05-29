using BankSystemApi.Accounts.Grpc;
using BankSystemApi.Application.Contracts.Accounts;
using BankSystemApi.Application.Contracts.Accounts.Operations;
using BankSystemApi.Presentation.Grpc.Mapping.Accounts;
using BankSystemApi.Presentation.Grpc.Mapping.Accounts.Requests;
using Google.Type;
using Grpc.Core;
using System.Text.Json;

namespace BankSystemApi.Presentation.Grpc.Controllers;

public sealed class AccountController : AccountService.AccountServiceBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    public override async Task<ProtoCreateAccountResponse> Create(
        ProtoCreateAccountRequest request,
        ServerCallContext context)
    {
        CreateAccount.Response applicationResponse = await _accountService.CreateAsync(
            request.MapToApplication(),
            context.CancellationToken);

        return applicationResponse switch
        {
            CreateAccount.Response.Success success => new ProtoCreateAccountResponse(success.Account.MapToProto()),
            CreateAccount.Response.Unauthorized unauthorized => throw new RpcException(new Status(
                StatusCode.Unauthenticated,
                $"User {unauthorized.UserId} is not authenticated")),
            CreateAccount.Response.AccountAlreadyExists accountAlreadyExists => throw new RpcException(new Status(
                StatusCode.AlreadyExists,
                $"Account {accountAlreadyExists.AccountNumber} already exists")),
            CreateAccount.Response.ReachedAccountLimit reachedLimit => throw new RpcException(new Status(
                StatusCode.OutOfRange,
                reachedLimit.ErrorMessage)),
            _ => throw new RpcException(new Status(StatusCode.Internal, "Unknown response type")),
        };
    }

    public override async Task<ProtoDepositResponse> Deposit(ProtoDepositRequest request, ServerCallContext context)
    {
        Deposit.Response applicationResponse = await _accountService.DepositAsync(
            request.MapToApplication(),
            context.CancellationToken);

        return applicationResponse switch
        {
            Deposit.Response.Success success => new ProtoDepositResponse(success.Account.MapToProto()),
            Deposit.Response.Unauthorized unauthorized => throw new RpcException(new Status(
                StatusCode.Unauthenticated,
                $"User {unauthorized.UserId} is not authenticated")),
            Deposit.Response.AccountNotFound accountNotFound => throw new RpcException(new Status(
                StatusCode.NotFound,
                $"Account {accountNotFound.AccountId} not found")),
            Deposit.Response.Forbidden forbidden => throw new RpcException(new Status(
                StatusCode.PermissionDenied,
                forbidden.ErrorMessage)),
            _ => throw new RpcException(new Status(StatusCode.Internal, "Unknown response type")),
        };
    }

    public override async Task<ProtoWithdrawResponse> Withdraw(ProtoWithdrawRequest request, ServerCallContext context)
    {
        Withdraw.Response applicationResponse = await _accountService.WithdrawAsync(
            request.MapToApplication(),
            context.CancellationToken);

        return applicationResponse switch
        {
            Withdraw.Response.Success success => new ProtoWithdrawResponse(success.Account.MapToProto()),
            Withdraw.Response.Unauthorized unauthorized => throw new RpcException(new Status(
                StatusCode.Unauthenticated,
                $"User {unauthorized.UserId} is not authenticated")),
            Withdraw.Response.AccountNotFound accountNotFound => throw new RpcException(new Status(
                StatusCode.NotFound,
                $"Account {accountNotFound.AccountId} not found")),
            Withdraw.Response.InsufficientFunds insufficientFunds => throw new RpcException(new Status(
                StatusCode.FailedPrecondition,
                insufficientFunds.ErrorMessage)),
            Withdraw.Response.Forbidden forbidden => throw new RpcException(new Status(
                StatusCode.PermissionDenied,
                forbidden.ErrorMessage)),
            _ => throw new RpcException(new Status(StatusCode.Internal, "Unknown response type")),
        };
    }

    public override async Task<ProtoGetBalanceResponse> GetBalance(
        ProtoGetBalanceRequest request,
        ServerCallContext context)
    {
        GetBalance.Response applicationResponse = await _accountService.GetBalanceAsync(
            request.MapToApplication(),
            context.CancellationToken);

        return applicationResponse switch
        {
            GetBalance.Response.Success success => new ProtoGetBalanceResponse(new Money
            {
                DecimalValue = success.Balance,
            }),
            GetBalance.Response.Unauthorized unauthorized => throw new RpcException(new Status(
                StatusCode.Unauthenticated,
                $"User {unauthorized.UserId} is not authenticated")),
            GetBalance.Response.AccountNotFound accountNotFound => throw new RpcException(new Status(
                StatusCode.NotFound,
                $"Account {accountNotFound.AccountId} not found")),
            GetBalance.Response.Forbidden forbidden => throw new RpcException(new Status(
                StatusCode.PermissionDenied,
                forbidden.ErrorMessage)),
            _ => throw new RpcException(new Status(StatusCode.Internal, "Unknown response type")),
        };
    }

    public override async Task<ProtoGetAccountsResponse> Get(
        ProtoGetAccountsRequest request,
        ServerCallContext context)
    {
        GetAccounts.Response applicationResponse = await _accountService.GetAsync(
            request.MapToApplication(),
            context.CancellationToken);

        return applicationResponse switch
        {
            GetAccounts.Response.Success success => new ProtoGetAccountsResponse(
                success.Accounts.Select(a => a.MapToProto()),
                success.PageToken is not null ? JsonSerializer.Serialize(success.PageToken.Value) : null),
            GetAccounts.Response.Unauthorized unauthorized => throw new RpcException(new Status(
                StatusCode.Unauthenticated,
                $"User {unauthorized.UserId} is not authenticated")),
            _ => throw new RpcException(new Status(StatusCode.Internal, "Unknown response type")),
        };
    }
}