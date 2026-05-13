using BankSystemApi.Gateway.Application.Abstractions.Users.Operations;

namespace BankSystemApi.Gateway.Application.Abstractions.Users;

public interface IUserClient
{
    Task<AddUser.Response> AddAsync(AddUser.Request request, CancellationToken cancellationToken);
}