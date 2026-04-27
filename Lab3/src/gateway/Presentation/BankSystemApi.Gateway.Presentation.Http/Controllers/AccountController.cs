using BankSystemApi.Gateway.Application.Contracts.Accounts;
using BankSystemApi.Gateway.Application.Contracts.Accounts.Operations.Responses;
using BankSystemApi.Gateway.Presentation.Http.Extensions;
using BankSystemApi.Gateway.Presentation.Http.Features;
using BankSystemApi.Gateway.Presentation.Http.Mapping.Accounts;
using BankSystemApi.Gateway.Presentation.Http.Mapping.Accounts.Requests;
using BankSystemApi.Gateway.Presentation.Http.Requests.Accounts;
using BankSystemApi.Gateway.Presentation.Http.Responses.Accounts;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Mime;

namespace BankSystemApi.Gateway.Presentation.Http.Controllers;

[ApiController]
[Route("api/accounts")]
public sealed class AccountController : ControllerBase
{
    private const string Scope = "Accounts";
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpPost("{targetUserId::long}")]
    [AuthorizeFeature(Scope)]
    [EndpointName("CreateAccount")]
    [EndpointSummary("Create a new account")]
    [EndpointDescription("Creates a new account with the specified details.")]
    [ProducesResponseType<CreateAccountResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.ProblemJson)]
    public async Task<ActionResult<CreateAccountHttpResponse>> Create(
        [FromRoute] long targetUserId,
        [FromBody] CreateAccountHttpRequest httpRequest,
        CancellationToken cancellationToken)
    {
        string callerUserId = User.GetUserId();

        Activity.Current.AddUserIdBaggage(callerUserId);

        CreateAccountResponse response = await _accountService.CreateAsync(
            httpRequest.MapToApplication(callerUserId, targetUserId),
            cancellationToken);

        return response switch
        {
            CreateAccountResponse.Success success => Ok(new CreateAccountHttpResponse
            {
                Account = success.Account.MapToModel(),
            }),
            CreateAccountResponse.Failure failure => BadRequest(new ProblemDetails
            {
                Detail = failure.ErrorMessage,
            }),
            _ => throw new UnreachableException(),
        };
    }

    [HttpPost("deposit")]
    [AuthorizeFeature(Scope)]
    [EndpointName("Deposit")]
    [EndpointSummary("Deposit balance")]
    [EndpointDescription("Deposits the specified amount into the account balance.")]
    [ProducesResponseType<DepositHttpResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.ProblemJson)]
    public async Task<ActionResult<DepositHttpResponse>> Deposit(
        [FromBody] DepositHttpRequest httpRequest,
        CancellationToken cancellationToken)
    {
        string userId = User.GetUserId();

        Activity.Current.AddUserIdBaggage(userId);
        Activity.Current.AddAccountIdBaggage(httpRequest.AccountId);

        DepositResponse response = await _accountService.DepositAsync(
            httpRequest.MapToApplication(userId),
            cancellationToken);

        return response switch
        {
            DepositResponse.Success success => Ok(new DepositHttpResponse
            {
                Account = success.Account.MapToModel(),
            }),
            DepositResponse.Failure failure => BadRequest(new ProblemDetails
            {
                Detail = failure.ErrorMessage,
            }),
            _ => throw new UnreachableException(),
        };
    }

    [HttpPost("withdraw")]
    [AuthorizeFeature(Scope)]
    [EndpointName("Withdraw")]
    [EndpointSummary("Withdraw balance")]
    [EndpointDescription("Withdraws the specified amount from the account balance. Validates for sufficient funds.")]
    [ProducesResponseType<WithdrawHttpResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.ProblemJson)]
    public async Task<ActionResult<WithdrawHttpResponse>> Withdraw(
        [FromBody] WithdrawHttpRequest httpRequest,
        CancellationToken cancellationToken)
    {
        string userId = User.GetUserId();

        Activity.Current.AddUserIdBaggage(userId);
        Activity.Current.AddAccountIdBaggage(httpRequest.AccountId);

        WithdrawResponse response = await _accountService.WithdrawAsync(
            httpRequest.MapToApplication(userId),
            cancellationToken);

        return response switch
        {
            WithdrawResponse.Success success => Ok(new WithdrawHttpResponse
            {
                Account = success.Account.MapToModel(),
            }),
            WithdrawResponse.Failure failure => BadRequest(new ProblemDetails
            {
                Detail = failure.ErrorMessage,
            }),
            _ => throw new UnreachableException(),
        };
    }

    [HttpGet("balance")]
    [AuthorizeFeature(Scope)]
    [EndpointName("GetBalance")]
    [EndpointSummary("Get account balance")]
    [EndpointDescription("Returns the current available balance for the specified account.")]
    [ProducesResponseType<GetBalanceHttpResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.ProblemJson)]
    public async Task<ActionResult<GetBalanceHttpResponse>> GetBalance(
        [FromQuery] GetBalanceHttpRequest httpRequest,
        CancellationToken cancellationToken)
    {
        string userId = User.GetUserId();

        Activity.Current.AddUserIdBaggage(userId);
        Activity.Current.AddAccountIdBaggage(httpRequest.AccountId);

        GetBalanceResponse response = await _accountService.GetBalanceAsync(
            httpRequest.MapToApplication(userId),
            cancellationToken);

        return response switch
        {
            GetBalanceResponse.Success success => Ok(new GetBalanceHttpResponse
            {
                Balance = success.Money,
            }),
            GetBalanceResponse.Failure failure => BadRequest(new ProblemDetails
            {
                Detail = failure.ErrorMessage,
            }),
            _ => throw new UnreachableException(),
        };
    }

    [HttpGet]
    [AuthorizeFeature(Scope)]
    [EndpointName("GetAccount")]
    [EndpointSummary("Get all account")]
    [EndpointDescription("Returns users accounts.")]
    [ProducesResponseType<GetAccountsResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.ProblemJson)]
    public async Task<ActionResult<GetAccountsResponse>> GetAccount(
        [FromQuery] GetAccountsHttpRequest httpRequest,
        CancellationToken cancellationToken)
    {
        string userId = User.GetUserId();

        Activity.Current.AddUserIdBaggage(userId);

        GetAccountsResponse response = await _accountService.GetAccountAsync(
            httpRequest.MapToApplication(userId),
            cancellationToken);

        return response switch
        {
            GetAccountsResponse.Success success => Ok(new GetAccountsHttpResponse
            {
                Accounts = success.Accounts.Select(a => a.MapToModel()).ToArray(),
                PageToken = success.PageToken,
            }),
            GetAccountsResponse.Failure failure => BadRequest(new ProblemDetails
            {
                Detail = failure.ErrorMessage,
            }),
            _ => throw new UnreachableException(),
        };
    }
}