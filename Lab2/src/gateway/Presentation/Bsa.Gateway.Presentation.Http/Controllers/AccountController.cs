using Bsa.Gateway.Application.Contracts.Accounts;
using Bsa.Gateway.Application.Contracts.Accounts.Operations;
using Bsa.Gateway.Presentation.Http.Mapping.Accounts;
using Bsa.Gateway.Presentation.Http.Mapping.Accounts.Requests;
using Bsa.Gateway.Presentation.Http.Requests.Accounts;
using Bsa.Gateway.Presentation.Http.Responses.Accounts;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Mime;

namespace Bsa.Gateway.Presentation.Http.Controllers;

[ApiController]
[Route("api/accounts")]
public sealed class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpPost]
    [EndpointName("CreateAccount")]
    [EndpointSummary("Create a new account")]
    [EndpointDescription("Creates a new account with the specified details.")]
    [ProducesResponseType<CreateAccountResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.ProblemJson)]
    public async Task<ActionResult<CreateAccountHttpResponse>> Create(
        [FromBody] CreateAccountHttpRequest httpRequest,
        CancellationToken cancellationToken)
    {
        CreateAccountResponse response = await _accountService.CreateAsync(
            httpRequest.MapToApplication(),
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
    [EndpointName("Deposit")]
    [EndpointSummary("Deposit balance")]
    [EndpointDescription("Deposits the specified amount into the account balance.")]
    [ProducesResponseType<DepositHttpResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.ProblemJson)]
    public async Task<ActionResult<DepositHttpResponse>> Deposit(
        [FromBody] DepositHttpRequest httpRequest,
        CancellationToken cancellationToken)
    {
        DepositResponse response = await _accountService.DepositAsync(
            httpRequest.MapToApplication(),
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
    [EndpointName("Withdraw")]
    [EndpointSummary("Withdraw balance")]
    [EndpointDescription("Withdraws the specified amount from the account balance. Validates for sufficient funds.")]
    [ProducesResponseType<WithdrawHttpResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.ProblemJson)]
    public async Task<ActionResult<WithdrawHttpResponse>> Withdraw(
        [FromBody] WithdrawHttpRequest httpRequest,
        CancellationToken cancellationToken)
    {
        WithdrawResponse response = await _accountService.WithdrawAsync(
            httpRequest.MapToApplication(),
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
    [EndpointName("GetBalance")]
    [EndpointSummary("Get account balance")]
    [EndpointDescription("Returns the current available balance for the specified account.")]
    [ProducesResponseType<GetBalanceHttpResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.ProblemJson)]
    public async Task<ActionResult<GetBalanceHttpResponse>> GetBalance(
        [FromQuery] GetBalanceHttpRequest httpRequest,
        CancellationToken cancellationToken)
    {
        GetBalanceResponse response = await _accountService.GetBalanceAsync(
            httpRequest.MapToApplication(),
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
}