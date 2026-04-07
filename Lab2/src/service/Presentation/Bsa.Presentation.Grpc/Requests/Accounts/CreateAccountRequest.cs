using Bsa.Presentation.Grpc.Validators;
using System.ComponentModel.DataAnnotations;

namespace Bsa.CsharpBackend.Grpc;

public sealed partial class CreateAccountRequest : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        return GuidValidator.Validate(Id)
            .Concat(PasswordValidator.Validate(Password))
            .Concat(AccountNumberValidator.Validate(AccountNumber));
    }
}