using Bsa.Presentation.Grpc.Validators;
using System.ComponentModel.DataAnnotations;

namespace Bsa.CsharpBackend.Grpc;

public sealed partial class LoginUserRequest : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        return AccountNumberValidator.Validate(AccountNumber)
            .Concat(PasswordValidator.Validate(Password));
    }
}