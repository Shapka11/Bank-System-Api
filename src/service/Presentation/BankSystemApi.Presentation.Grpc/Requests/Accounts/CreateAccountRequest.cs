using BankSystemApi.Presentation.Grpc.Validators;
using System.ComponentModel.DataAnnotations;

namespace BankSystemApi.Accounts.Grpc;

public sealed partial class CreateAccountRequest : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        return GuidValidator.Validate(CallerUserId)
            .Concat(PasswordValidator.Validate(Password))
            .Concat(AccountNumberValidator.Validate(AccountNumber));
    }
}