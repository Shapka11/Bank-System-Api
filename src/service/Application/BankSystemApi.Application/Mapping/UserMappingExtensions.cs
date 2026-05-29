using BankSystemApi.Application.Contracts.Users.Models;
using BankSystemApi.Domain.Users;

namespace BankSystemApi.Application.Mapping;

public static class UserMappingExtensions
{
    public static UserDto MapToDto(this User user)
        => new(user.Id.Value, user.AuthorizationId, user.CreatedAt);
}