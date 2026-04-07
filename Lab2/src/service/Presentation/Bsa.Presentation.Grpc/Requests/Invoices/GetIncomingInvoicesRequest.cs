using Bsa.Presentation.Grpc.Validators;
using System.ComponentModel.DataAnnotations;

namespace Bsa.CsharpBackend.Grpc;

public sealed partial class GetIncomingInvoicesRequest : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        return SenderAccountNumbers.SelectMany(a => AccountNumberValidator.Validate(a))
            .Concat(PageSizeValidator.Validate(Pagination.PageSize));
    }
}