using Bsa.Cli.Infrastructure.ClientService.Users.Models;
using Refit;

namespace Bsa.Cli.Infrastructure.ClientService.Users;

public interface IRefitUserClient
{
    [Post("/api/user/login")]
    Task<IApiResponse<LoginUserResponse>> LoginAsync(
        [Body] LoginUserRequest request,
        CancellationToken cancellationToken);

    [Post("/api/user/logout")]
    Task<IApiResponse> LogoutAsync(
        [Body] LogoutUserRequest request,
        CancellationToken cancellationToken);

    [Post("/api/user/deposit")]
    Task<IApiResponse> DepositAsync(
        [Body] DepositRequest request,
        CancellationToken cancellationToken);

    [Post("/api/user/withdraw")]
    Task<IApiResponse> WithdrawAsync(
        [Body] WithdrawRequest request,
        CancellationToken cancellationToken);

    [Get("/api/user/balance")]
    Task<IApiResponse<GetBalanceResponse>> GetBalanceAsync(
        [Query] GetBalanceRequest request,
        CancellationToken cancellationToken);

    [Get("/api/user/history")]
    Task<IApiResponse<GetHistoryResponse>> GetHistoryAsync(
        [Query] GetHistoryRequest request,
        CancellationToken cancellationToken);
}