using BankSystemApi.Presentation.Grpc.Validators;
using System.ComponentModel.DataAnnotations;

namespace BankSystemApi.Grpc;

public sealed partial class GetBalanceRequest : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        return GuidValidator.Validate(UserId)
            .Concat(GuidValidator.Validate(AccountId));
    }
}