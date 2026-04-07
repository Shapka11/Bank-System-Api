using Google.Protobuf.Collections;

namespace Bsa.Gateway.Infrastructure.BankService.Mapping;

public static class EnumerableProtoMappingExtensions
{
    public static RepeatedField<T> MapToProto<T>(this IEnumerable<T> data)
        => new() { data };
}