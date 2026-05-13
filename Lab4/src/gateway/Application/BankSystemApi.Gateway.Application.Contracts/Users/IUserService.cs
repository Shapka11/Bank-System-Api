using BankSystemApi.Gateway.Application.Contracts.Users.Operations;

namespace BankSystemApi.Gateway.Application.Contracts.Users;

public interface IUserService
{
    Task<AddUserResponse> AddAsync(AddUserRequest request, CancellationToken cancellationToken);
}