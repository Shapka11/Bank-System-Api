using BankSystemApi.Application.Abstractions.Events.Models;
using Itmo.Dev.Platform.Kafka.Producer;

namespace BankSystemApi.Infrastructure.Kafka.Mapping;

public static class AccountMessageMappingExtensions
{
    public static KafkaProducerMessage<ProtoAccountCreationKey, ProtoAccountCreationValue> ToMessage(
        this CreationAccountEvent evt)
    {
        var key = new ProtoAccountCreationKey
        {
            AccountId = evt.AccountId,
        };

        var value = new ProtoAccountCreationValue
        {
            AccountId = evt.AccountId,
            UserId = evt.UserId,
            AccountType = evt.AccountType.MapToProto(),
        };

        return new KafkaProducerMessage<ProtoAccountCreationKey, ProtoAccountCreationValue>(
            Key: key,
            Value: value);
    }

    private static ProtoAccountType MapToProto(this CreationAccountType type)
    {
        return type switch
        {
            CreationAccountType.Personal => ProtoAccountType.Personal,
            CreationAccountType.Corporate => ProtoAccountType.Corporate,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
        };
    }
}