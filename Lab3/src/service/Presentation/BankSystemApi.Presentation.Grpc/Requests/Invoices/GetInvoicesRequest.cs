using BankSystemApi.Presentation.Grpc.Validators;
using System.ComponentModel.DataAnnotations;

namespace BankSystemApi.Grpc;

public sealed partial class GetInvoicesRequest : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        return ForeignAccountIds.SelectMany(a => GuidValidator.Validate(a))
            .Concat(PageSizeValidator.Validate(Pagination.PageSize))
            .Concat(GuidValidator.Validate(UserId));
    }
}