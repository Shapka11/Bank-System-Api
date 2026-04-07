using Bsa.Presentation.Grpc.Validators;
using System.ComponentModel.DataAnnotations;

namespace Bsa.CsharpBackend.Grpc;

public sealed partial class DepositRequest : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        return GuidValidator.Validate(Id);
    }
}