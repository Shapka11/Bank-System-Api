using Bsa.Presentation.Grpc.Validators;
using System.ComponentModel.DataAnnotations;

namespace Bsa.CsharpBackend.Grpc;

public sealed partial class CreateInvoiceRequest : IValidatableObject
{
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        return AccountNumberValidator.Validate(SenderAccountNumber)
            .Concat(AccountNumberValidator.Validate(ReceiverAccountNumber))
            .Concat(GuidValidator.Validate(SessionId));
    }
}