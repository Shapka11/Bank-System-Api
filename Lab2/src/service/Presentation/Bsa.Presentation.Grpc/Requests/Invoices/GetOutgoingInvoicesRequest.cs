using Bsa.Presentation.Grpc.Validators;
using System.ComponentModel.DataAnnotations;

namespace Bsa.CsharpBackend.Grpc;

public sealed partial class GetOutgoingInvoicesRequest : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        return ReceiverAccountNumbers.SelectMany(a => AccountNumberValidator.Validate(a))
            .Concat(PageSizeValidator.Validate(Pagination.PageSize));
    }
}