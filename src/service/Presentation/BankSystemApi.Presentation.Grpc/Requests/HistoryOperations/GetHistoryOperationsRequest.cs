using BankSystemApi.Presentation.Grpc.Validators;
using System.ComponentModel.DataAnnotations;

namespace BankSystemApi.HistoryOperations.Grpc;

public sealed partial class GetHistoryOperationsRequest : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        return GuidValidator.Validate(UserId)
            .Concat(PageSizeValidator.Validate(Pagination.PageSize));
    }
}