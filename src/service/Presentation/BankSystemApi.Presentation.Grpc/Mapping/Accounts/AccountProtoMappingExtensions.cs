using BankSystemApi.Application.Contracts.Accounts.Models;
using Google.Protobuf.WellKnownTypes;
using Google.Type;

namespace BankSystemApi.Presentation.Grpc.Mapping.Accounts;

public static class AccountProtoMappingExtensions
{
    public static ProtoAccount MapToProto(this AccountDto dto) =>
        new(
            dto.Id,
            dto.UserId,
            dto.Type.MapToProto(),
            dto.Number,
            dto.Password,
            new Money { DecimalValue = dto.Balance },
            dto.CreatedAt.ToTimestamp(),
            dto.UpdatedAt.ToTimestamp());
}