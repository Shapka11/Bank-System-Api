using BankSystemApi.Presentation.Grpc.Validators;
using System.ComponentModel.DataAnnotations;

namespace BankSystemApi.Grpc;

public sealed partial class CreateInvoiceRequest : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        return GuidValidator.Validate(SenderAccountId)
            .Concat(GuidValidator.Validate(ReceiverAccountId))
            .Concat(GuidValidator.Validate(UserId));
    }
}