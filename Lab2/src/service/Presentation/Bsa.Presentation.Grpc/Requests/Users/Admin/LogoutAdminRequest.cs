using Bsa.Presentation.Grpc.Validators;
using System.ComponentModel.DataAnnotations;

namespace Bsa.CsharpBackend.Grpc;

public sealed partial class LogoutAdminRequest : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        return GuidValidator.Validate(Id);
    }
}