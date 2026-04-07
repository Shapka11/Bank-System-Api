using Bsa.Presentation.Grpc.Validators;
using System.ComponentModel.DataAnnotations;

namespace Bsa.CsharpBackend.Grpc;

public sealed partial class GetHistoryOperationRequest : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        return GuidValidator.Validate(Id)
            .Concat(PageSizeValidator.Validate(Pagination.PageSize));
    }
}