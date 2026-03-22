using Bsa.Cli.Application.Abstractions.Admin;
using Bsa.Cli.Application.Abstractions.Admin.Operations;
using Bsa.Cli.Infrastructure.ClientService.Admins.Models;
using Refit;

namespace Bsa.Cli.Infrastructure.ClientService.Admins;

public sealed class AdminClient : IAdminClient
{
    private readonly IRefitAdminClient _client;

    public AdminClient(IRefitAdminClient client)
    {
        _client = client;
    }

    public async Task<LoginAdminQuery.Result> LoginAdminAsync(
        LoginAdminQuery.Request request,
        CancellationToken cancellationToken)
    {
        var httpRequest = new LoginAdminRequest(request.Password);
        IApiResponse<LoginAdminResponse> response = await _client.LoginAsync(httpRequest, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            LoginAdminResponse? loginAdminResponse = response.Content;
            if (loginAdminResponse is null)
                return new LoginAdminQuery.Result.Failure("Empty response");

            Guid sessionId = loginAdminResponse.Id;

            return new LoginAdminQuery.Result.Success(sessionId);
        }

        string errorMessage = response.Error?.Content ?? response.ReasonPhrase ?? "Unknown error";
        return new LoginAdminQuery.Result.Failure(errorMessage);
    }

    public async Task<LogoutAdminQuery.Result> LogoutAdminAsync(
        LogoutAdminQuery.Request request,
        CancellationToken cancellationToken)
    {
        var httpRequest = new LogoutAdminRequest(request.SessionId);
        IApiResponse response = await _client.LogoutAsync(httpRequest, cancellationToken);

        if (response.IsSuccessful)
        {
            return new LogoutAdminQuery.Result.Success();
        }

        string errorMessage = response.Error?.Content ?? response.ReasonPhrase ?? "Unknown error";
        return new LogoutAdminQuery.Result.Failure(errorMessage);
    }

    public async Task<CreateAccountQuery.Result> CreateAccountAsync(
        CreateAccountQuery.Request request,
        CancellationToken cancellationToken)
    {
        var httpRequest = new CreateAccountRequest(request.Id, request.AccountNumber, request.Password);
        IApiResponse response = await _client.CreateAccountAsync(httpRequest, cancellationToken);

        if (response.IsSuccessful)
        {
            return new CreateAccountQuery.Result.Success();
        }

        string errorMessage = response.Error?.Content ?? response.ReasonPhrase ?? "Unknown error";
        return new CreateAccountQuery.Result.Failure(errorMessage);
    }
}