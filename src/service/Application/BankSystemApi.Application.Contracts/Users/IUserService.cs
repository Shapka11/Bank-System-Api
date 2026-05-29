using BankSystemApi.Application.Contracts.Users.Operations;

namespace BankSystemApi.Application.Contracts.Users;

public interface IUserService
{
    Task<AddUser.Response> AddAsync(AddUser.Request request, CancellationToken cancellationToken);

    Task<GetUsers.Response> GetAsyns(GetUsers.Request request, CancellationToken cancellationToken);
}