using BankSystemApi.Presentation.Grpc.Validators;
using System.ComponentModel.DataAnnotations;

namespace BankSystemApi.Invoices.Grpc;

public sealed partial class GetInvoicesRequest : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        return PageSizeValidator.Validate(Pagination.PageSize)
            .Concat(GuidValidator.Validate(UserId));
    }
}